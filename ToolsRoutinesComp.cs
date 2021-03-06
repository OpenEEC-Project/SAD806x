﻿using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace SAD806x
{
    public static class ToolsRoutinesComp
    {
        public static string skeletonToBytes(string skeleton)
        {
            Regex regExp = new Regex("[^a-fA-F0-9]");
            return regExp.Replace(skeleton, "");
        }

        public static SortedList getRoutinesComparisonSkeleton(ref SADBin sadBin)
        {
            SortedList slResult = new SortedList();

            if (sadBin == null) return slResult;
            if (!sadBin.isDisassembled) return slResult;

            try
            {
                SADBank sadBank = sadBin.Bank8;
                while (sadBank != null)
                {
                    RoutineSkeleton rsSkt = null;
                    string sSkeleton = string.Empty;

                    int opeIndex = 0;
                    bool newCall = true;

                    int LastOperationAddressInt = -1;

                    while (true)
                    {
                        Operation currentOpe = (Operation)sadBank.slOPs.GetByIndex(opeIndex);
                        if (currentOpe == null) break;

                        LastOperationAddressInt = currentOpe.AddressInt;

                        // Bank Change Ops case - Ignored
                        bool bBankChangeOpe = false;
                        if (currentOpe.OriginalOpArr.Length > 0)
                        {
                            switch (currentOpe.OriginalOpArr[0].ToLower())
                            {
                                case "10":
                                case "f4":
                                case "f5":
                                case "f6":
                                case "fd":
                                    bBankChangeOpe = true;
                                    break;
                            }
                        }
                        if (bBankChangeOpe)
                        {
                            opeIndex++;
                            if (opeIndex > sadBank.slOPs.Count - 1) break;
                            continue;
                        }

                        if (newCall)
                        {
                            rsSkt = new RoutineSkeleton();
                            rsSkt.BankNum = sadBank.Num;
                            rsSkt.AddressInt = currentOpe.AddressInt;
                            Call cCall = (Call)sadBin.Calibration.slCalls[currentOpe.UniqueAddress];
                            rsSkt.ShortLabel = (cCall == null) ? string.Empty : cCall.ShortLabel;
                            rsSkt.Label = (cCall == null) ? string.Empty : cCall.Label;
                            rsSkt.alOperations = new ArrayList();
                            rsSkt.alMixedElements = new ArrayList();
                            sSkeleton = string.Empty;
                            newCall = false;
                        }

                        // Functions routines with parameters specificity
                        //  Parameters are generated in fake Operations for Better Routine Matching
                        if (currentOpe.CallArgs != null)
                        {
                            if (currentOpe.CallArgs.Length > 0)
                            {
                                if (currentOpe.alCalibrationElems != null)
                                {
                                    if (currentOpe.alCalibrationElems.Count > 0)
                                    {
                                        Routine rRoutine = (Routine)sadBin.Calibration.slRoutines[Tools.UniqueAddress(currentOpe.ApplyOnBankNum, currentOpe.AddressJumpInt)];
                                        if (rRoutine != null)
                                        {
                                            if (rRoutine.Type == RoutineType.FunctionByte || rRoutine.Type == RoutineType.FunctionWord)
                                            {
                                                if (rRoutine.IOs != null)
                                                {
                                                    if (rRoutine.IOs.Length > 0)
                                                    {
                                                        // Fake Operation for CalElem
                                                        Operation fakeCalElemOpe = new Operation(sadBank.Num, sadBank.AddressInternalInt);
                                                        CalibrationElement calElem = (CalibrationElement)currentOpe.alCalibrationElems[0];
                                                        fakeCalElemOpe.alCalibrationElems = currentOpe.alCalibrationElems;
                                                        fakeCalElemOpe.OriginalOpArr = new string[] { "a1", calElem.Address.Substring(2, 2), calElem.Address.Substring(0, 2), rRoutine.IOs[0].AddressRegister };
                                                        rsSkt.alOperations.Add(fakeCalElemOpe);

                                                        // Fake Operation for Parameter
                                                        Operation fakeParamOpe = new Operation(sadBank.Num, sadBank.AddressInternalInt);
                                                        string fParamValue = "ff";
                                                        string fParamReg = "ff";
                                                        string fOpCode = "a";
                                                        if (rRoutine.Type == RoutineType.FunctionByte) fOpCode = "b";
                                                        foreach (CallArgument cArg in currentOpe.CallArguments)
                                                        {
                                                            if (cArg.DecryptedValue != calElem.Address)
                                                            {
                                                                fParamValue = cArg.DecryptedValue;
                                                                fParamReg = cArg.OutputRegisterAddress;
                                                                break;
                                                            }
                                                        }
                                                        if (fParamValue.Length % 2 == 1) fParamValue = "0" + fParamValue;
                                                        switch (fParamValue.Length)
                                                        {
                                                            case 2:
                                                                fOpCode += "0";
                                                                fakeParamOpe.OriginalOpArr = new string[] { fOpCode, fParamValue, fParamReg };
                                                                break;
                                                            case 4:
                                                                fOpCode += "3";
                                                                fakeParamOpe.OriginalOpArr = new string[] { fOpCode, "1", fParamValue.Substring(2, 2), fParamValue.Substring(0, 2), fParamReg };
                                                                break;
                                                            default:
                                                                fOpCode += "0";
                                                                fakeParamOpe.OriginalOpArr = new string[] { fOpCode, "ff", fParamReg };
                                                                break;
                                                        }
                                                        rsSkt.alOperations.Add(fakeParamOpe);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        rsSkt.alOperations.Add(currentOpe);

                        if (currentOpe.alCalibrationElems != null)
                        {
                            foreach (CalibrationElement calElem in currentOpe.alCalibrationElems)
                            {
                                if (calElem.isStructure) rsSkt.alMixedElements.Add(calElem.StructureElem);
                                else if (calElem.isTable) rsSkt.alMixedElements.Add(calElem.TableElem);
                                else if (calElem.isFunction) rsSkt.alMixedElements.Add(calElem.FunctionElem);
                                else if (calElem.isScalar) rsSkt.alMixedElements.Add(calElem.ScalarElem);
                            }
                        }
                        foreach (OperationParam opeParam in currentOpe.OperationParams)
                        {
                            if (opeParam.EmbeddedParam == null) continue;
                            if (opeParam.EmbeddedParam.GetType() == typeof(Scalar)
                                || opeParam.EmbeddedParam.GetType() == typeof(Function)
                                || opeParam.EmbeddedParam.GetType() == typeof(Table)
                                || opeParam.EmbeddedParam.GetType() == typeof(Structure)) rsSkt.alMixedElements.Add(opeParam.EmbeddedParam);
                        }
                        for (int iAddrAdder = 1; iAddrAdder < currentOpe.OriginalOpArr.Length; iAddrAdder++)
                        {
                            string uniqueAddress = Tools.UniqueAddress(currentOpe.BankNum, currentOpe.AddressInt + iAddrAdder);
                            object includedElem = null;
                            if (includedElem == null) includedElem = sadBin.Calibration.slExtScalars[uniqueAddress];
                            if (includedElem == null) includedElem = sadBin.Calibration.slExtFunctions[uniqueAddress];
                            if (includedElem == null) includedElem = sadBin.Calibration.slExtTables[uniqueAddress];
                            if (includedElem == null) includedElem = sadBin.Calibration.slExtStructures[uniqueAddress];
                            if (includedElem != null) rsSkt.alMixedElements.Add(includedElem);
                        }

                        bool endCall = false;
                        int opCode = Convert.ToInt32(currentOpe.OriginalOpArr[0], 16);
                        if (opCode == 0xf0 || opCode == 0xf1) endCall = true;  // Returns
                        else if (opCode == 0xe7) endCall = true;               // Jump

                        if (endCall)
                        {
                            rsSkt.LastOperationAddressInt = currentOpe.AddressInt;
                            rsSkt.setSkeleton();
                            newCall = true;
                            slResult.Add(rsSkt.UniqueAddress, rsSkt);
                            rsSkt = null;
                        }

                        opeIndex++;

                        if (opeIndex > sadBank.slOPs.Count - 1) break;
                    }

                    // To End the Call
                    if (rsSkt != null)
                    {
                        rsSkt.LastOperationAddressInt = LastOperationAddressInt;
                        rsSkt.setSkeleton();
                        slResult.Add(rsSkt.UniqueAddress, rsSkt);
                    }

                    if (sadBank.Num == 0) sadBank = null;
                    else if (sadBank.Num == 9) sadBank = sadBin.Bank0;
                    else if (sadBank.Num == 1) sadBank = sadBin.Bank9;
                    else if (sadBank.Num == 8) sadBank = sadBin.Bank1;
                }

                return slResult;
            }
            catch
            {
                return new SortedList();
            }
            finally
            {
                GC.Collect();
            }
        }

        public static SortedList getRoutinesComparisonSkeleton(string sktFilePath)
        {
            StreamReader sReader = null;
            SortedList slResult = null;
            RoutineSkeleton rsSkt = null;
            string sSkeleton = string.Empty;
            bool validSkt = false;

            try
            {
                slResult = new SortedList();
                sReader = File.OpenText(sktFilePath);

                while (!sReader.EndOfStream)
                {
                    string sLine = sReader.ReadLine();
                    if (sLine == null) continue;
                    if (sLine == string.Empty) continue;

                    sLine = sLine.Trim();
                    if (sLine.StartsWith("++"))
                    {
                        try
                        {
                            sLine = sLine.Replace("++", string.Empty).Trim();
                            string uniqueAddressHex = sLine.Substring(0, sLine.IndexOf("-")).Trim();

                            rsSkt = new RoutineSkeleton();
                            rsSkt.BankNum = Convert.ToInt32(uniqueAddressHex.Substring(0, uniqueAddressHex.IndexOf(" ")).Trim(), 16);
                            rsSkt.AddressInt = Convert.ToInt32(uniqueAddressHex.Substring(uniqueAddressHex.IndexOf(" ") + 1).Trim(), 16) - SADDef.EecBankStartAddress;
                            rsSkt.ShortLabel = sLine.Substring(sLine.IndexOf("-") + 1).Trim();
                            if (rsSkt.ShortLabel.Contains("-"))
                            {
                                rsSkt.Label = rsSkt.ShortLabel.Substring(rsSkt.ShortLabel.IndexOf("-") + 1).Trim();
                                rsSkt.ShortLabel = rsSkt.ShortLabel.Substring(0, rsSkt.ShortLabel.IndexOf("-")).Trim();
                            }
                            else
                            {
                                rsSkt.Label = rsSkt.ShortLabel;
                            }
                        }
                        catch
                        {
                            rsSkt = null;
                        }

                        sSkeleton = string.Empty;
                        continue;
                    }

                    if (sLine.StartsWith("--") && rsSkt != null)
                    {
                        try
                        {
                            sLine = sLine.Replace("--", string.Empty).Trim();
                            string uniqueAddressHex = sLine.Substring(0, sLine.IndexOf("-")).Trim();
                            int opsNumber = Convert.ToInt32(sLine.Substring(sLine.IndexOf("-") + 1).Trim());

                            rsSkt.setSkeleton(sSkeleton);
                            rsSkt.LastOperationAddressInt = Convert.ToInt32(uniqueAddressHex.Substring(uniqueAddressHex.IndexOf(" ") + 1).Trim(), 16) - SADDef.EecBankStartAddress;

                            if (rsSkt.Skeleton.Split('\n').Length - 1 == opsNumber) slResult.Add(rsSkt.UniqueAddress, rsSkt);
                        }
                        catch
                        {
                        }
                        rsSkt = null;
                        continue;
                    }

                    if (sLine.StartsWith("##") && rsSkt == null)
                    {
                        validSkt = true;
                        break;
                    }

                    if (rsSkt != null)
                    {
                        sSkeleton += sLine.Replace("\t", string.Empty).Trim() + "\n";
                        continue;
                    }
                }

                try { sReader.Close(); }
                catch { }
                try { sReader.Dispose(); }
                catch { }
                sReader = null;

                if (validSkt) return slResult;
                else return new SortedList();
            }
            catch
            {
                return new SortedList();
            }
            finally
            {
                try { sReader.Close(); }
                catch { }
                try { sReader.Dispose(); }
                catch { }
                sReader = null;

                GC.Collect();
            }
        }

        public static void exportRoutinesComparisonSkeleton(ref SortedList sktSkeleton, string sktFilePath)
        {
            Exception errorException = null;

            StreamWriter sWri = null;

            try
            {
                int callCounts = 0;
                int maxOpsCount = 0;
                string maxOpsCallUniqueAddressHex = string.Empty;

                sWri = new StreamWriter(sktFilePath, false, new UTF8Encoding(false));

                foreach (RoutineSkeleton rsSkt in sktSkeleton.Values)
                {
                    callCounts++;
                    if (rsSkt.OpsNumber > maxOpsCount)
                    {
                        maxOpsCount = rsSkt.OpsNumber;
                        maxOpsCallUniqueAddressHex = rsSkt.UniqueAddressHex;
                    }

                    sWri.WriteLine("++ " + rsSkt.UniqueAddressHex + " - " + rsSkt.FullLabel);
                    sWri.WriteLine(rsSkt.Skeleton);
                    sWri.WriteLine("-- " + Tools.UniqueAddressHex(rsSkt.BankNum, rsSkt.LastOperationAddressInt) + " - " + rsSkt.OpsNumber.ToString() + "\r\n");
                }

                sWri.WriteLine("## Routines(" + callCounts.ToString() + "), Maximum Operations(" + maxOpsCount.ToString() + " starting at " + maxOpsCallUniqueAddressHex + ")");
            }
            catch (Exception ex)
            {
                errorException = ex;
            }
            finally
            {
                try { sWri.Close(); }
                catch { }
                try { sWri.Dispose(); }
                catch { }
                sWri = null;

                GC.Collect();
            }

            if (errorException != null) throw errorException;
        }

        public static void compareRoutinesComparisonSkeletons(ref SortedList slRoutinesSkt1, ref SortedList slRoutinesSkt2, ref SADBin sadBin1, ref SADBin sadBin2, int minOpsNumber, double gapTolerance, double levTolerance)
        {
            //int minOpsNumber = 3;      // 3 ops for minimum
            //double gapTolerance = 0.1; // 10%
            //double levTolerance = 0.8; // 80%

            foreach (RoutineSkeleton rsSkt1 in slRoutinesSkt1.Values)
            {
                // Routines Matchings
                try
                {
                    if (rsSkt1.OpsNumber <= minOpsNumber) continue; // No interest

                    foreach (RoutineSkeleton rsLSkt2 in slRoutinesSkt2.Values)
                    {
                        if (rsLSkt2.OpsNumber <= minOpsNumber) continue; // No interest

                        int opsGap = Math.Abs(rsSkt1.OpsNumber - rsLSkt2.OpsNumber);
                        if (opsGap / rsSkt1.OpsNumber > gapTolerance || opsGap / rsLSkt2.OpsNumber > gapTolerance) continue; // More than Gap Tolerance

                        double dTolerance = (1.0 - levTolerance) * (rsSkt1.OpsNumber + rsLSkt2.OpsNumber) / 2;

                        //int lDist = LevenshteinDistance(rsSkt1.Bytes, rsSkt2.Bytes);
                        int lDist = DamerauLevenshteinDistance(rsSkt1.Bytes, rsLSkt2.Bytes, (int)dTolerance);
                        if (lDist < 0) continue; // More than Levenshtein Distance Tolerance
                        if (lDist > dTolerance) continue; // More than Levenshtein Distance Tolerance

                        if (rsSkt1.alMatches == null) rsSkt1.alMatches = new ArrayList();
                        if (rsLSkt2.alMatches == null) rsLSkt2.alMatches = new ArrayList();

                        double dChances = 0.0;
                        if (dTolerance == 0.0) dChances = 0.0;
                        else dChances = 1.0 - ((1.0 - levTolerance) * lDist / dTolerance);

                        rsSkt1.alMatches.Add(new object[] { rsLSkt2, dChances });
                        rsLSkt2.alMatches.Add(new object[] { rsSkt1, dChances });
                    }

                    if (rsSkt1.alMatches == null) continue;
                    if (rsSkt1.alMatches.Count != 1) continue;    // Managed only when matching is 100% sure
                    if (rsSkt1.alOperations == null) continue; // Managed only when Operations are included
                    RoutineSkeleton rsSkt2 = (RoutineSkeleton)((object[])rsSkt1.alMatches[0])[0];
                    if (rsSkt2.alOperations == null) continue; // Managed only when Operations are included

                    // Only accessible for Binary vs Binary comparison
                    object[] newRSkteletons = getMatchingRoutinesSkeletons(rsSkt1, rsSkt2, (double)((object[])rsSkt1.alMatches[0])[1], minOpsNumber, gapTolerance, levTolerance);
                    if (newRSkteletons == null) continue;   // No way to obtain identical Skeletons

                    RoutineSkeleton rsMSkt1 = (RoutineSkeleton)newRSkteletons[0];
                    RoutineSkeleton rsMSkt2 = (RoutineSkeleton)newRSkteletons[1];

                    // Perfectly matching Skeleton for current binary is now rsMSkt1
                    // Perfectly matching Skeleton for compare binary is now rsMSkt2
                    // Analysis has to be done on rsMSkt1 and rsMSkt2
                    // !!! But PossibleMatching lists have to be filled on rsSkt1 and rsSkt2 to propagate the result

                    if (rsMSkt1.alOperations.Count != rsMSkt2.alOperations.Count) continue;

                    rsSkt1.slPossibleMatchingRegisters = new SortedList();
                    rsSkt1.slPossibleMatchingMixedElements = new SortedList();

                    rsSkt2.slPossibleMatchingRegisters = new SortedList();
                    rsSkt2.slPossibleMatchingMixedElements = new SortedList();

                    for (int iSkOpeNum = 0; iSkOpeNum < rsMSkt1.alOperations.Count; iSkOpeNum++)
                    {
                        Operation sk1Ope = (Operation)rsMSkt1.alOperations[iSkOpeNum];
                        Operation sk2Ope = (Operation)rsMSkt2.alOperations[iSkOpeNum];

                        // 20200908 - Calibration Elements, OtherElems at the same place
                        //              new OtherElems/IncludedElemens Mngt added
                        ArrayList alSk1Elems = new ArrayList();
                        if (sk1Ope.alCalibrationElems != null) alSk1Elems.AddRange(sk1Ope.alCalibrationElems);
                        foreach (OperationParam opeParam in sk1Ope.OperationParams)
                        {
                            if (opeParam.EmbeddedParam == null) continue;
                            if (opeParam.EmbeddedParam.GetType() == typeof(Scalar)
                                || opeParam.EmbeddedParam.GetType() == typeof(Function)
                                || opeParam.EmbeddedParam.GetType() == typeof(Table)
                                || opeParam.EmbeddedParam.GetType() == typeof(Structure)) alSk1Elems.Add(opeParam.EmbeddedParam);
                        }
                        for (int iAddrAdder = 1; iAddrAdder < sk1Ope.OriginalOpArr.Length; iAddrAdder++)
                        {
                            string uniqueAddress = Tools.UniqueAddress(sk1Ope.BankNum, sk1Ope.AddressInt + iAddrAdder);
                            object includedElem = null;
                            if (includedElem == null) includedElem = sadBin1.Calibration.slExtScalars[uniqueAddress];
                            if (includedElem == null) includedElem = sadBin1.Calibration.slExtFunctions[uniqueAddress];
                            if (includedElem == null) includedElem = sadBin1.Calibration.slExtTables[uniqueAddress];
                            if (includedElem == null) includedElem = sadBin1.Calibration.slExtStructures[uniqueAddress];
                            if (includedElem != null) alSk1Elems.Add(includedElem);
                        }

                        ArrayList alSk2Elems = new ArrayList();
                        if (sk2Ope.alCalibrationElems != null) alSk2Elems.AddRange(sk2Ope.alCalibrationElems);
                        foreach (OperationParam opeParam in sk2Ope.OperationParams)
                        {
                            if (opeParam.EmbeddedParam == null) continue;
                            if (opeParam.EmbeddedParam.GetType() == typeof(Scalar)
                                || opeParam.EmbeddedParam.GetType() == typeof(Function)
                                || opeParam.EmbeddedParam.GetType() == typeof(Table)
                                || opeParam.EmbeddedParam.GetType() == typeof(Structure)) alSk2Elems.Add(opeParam.EmbeddedParam);
                        }
                        for (int iAddrAdder = 1; iAddrAdder < sk2Ope.OriginalOpArr.Length; iAddrAdder++)
                        {
                            string uniqueAddress = Tools.UniqueAddress(sk2Ope.BankNum, sk2Ope.AddressInt + iAddrAdder);
                            object includedElem = null;
                            if (includedElem == null) includedElem = sadBin2.Calibration.slExtScalars[uniqueAddress];
                            if (includedElem == null) includedElem = sadBin2.Calibration.slExtFunctions[uniqueAddress];
                            if (includedElem == null) includedElem = sadBin2.Calibration.slExtTables[uniqueAddress];
                            if (includedElem == null) includedElem = sadBin2.Calibration.slExtStructures[uniqueAddress];
                            if (includedElem != null) alSk2Elems.Add(includedElem);
                        }

                        for (int iElem = 0; iElem < alSk1Elems.Count || iElem < alSk2Elems.Count; iElem++)
                        {
                            object sk1Elem = null;
                            object sk2Elem = null;
                            string sk1ElemUniqueAddress = string.Empty;
                            string sk2ElemUniqueAddress = string.Empty;
                            if (iElem < alSk1Elems.Count) sk1Elem = alSk1Elems[iElem];
                            if (iElem < alSk2Elems.Count) sk2Elem = alSk2Elems[iElem];
                            if (sk1Elem == null && sk2Elem == null) break;
                            if (sk1Elem != null)
                            {
                                if (sk1Elem.GetType() == typeof(CalibrationElement)) sk1ElemUniqueAddress = ((CalibrationElement)sk1Elem).UniqueAddress;
                                else if (sk1Elem.GetType() == typeof(Scalar)) sk1ElemUniqueAddress = ((Scalar)sk1Elem).UniqueAddress;
                                else if (sk1Elem.GetType() == typeof(Function)) sk1ElemUniqueAddress = ((Function)sk1Elem).UniqueAddress;
                                else if (sk1Elem.GetType() == typeof(Table)) sk1ElemUniqueAddress = ((Table)sk1Elem).UniqueAddress;
                                else if (sk1Elem.GetType() == typeof(Structure)) sk1ElemUniqueAddress = ((Structure)sk1Elem).UniqueAddress;
                            }
                            if (sk2Elem != null)
                            {
                                if (sk2Elem.GetType() == typeof(CalibrationElement)) sk2ElemUniqueAddress = ((CalibrationElement)sk2Elem).UniqueAddress;
                                else if (sk2Elem.GetType() == typeof(Scalar)) sk2ElemUniqueAddress = ((Scalar)sk2Elem).UniqueAddress;
                                else if (sk2Elem.GetType() == typeof(Function)) sk2ElemUniqueAddress = ((Function)sk2Elem).UniqueAddress;
                                else if (sk2Elem.GetType() == typeof(Table)) sk2ElemUniqueAddress = ((Table)sk2Elem).UniqueAddress;
                                else if (sk2Elem.GetType() == typeof(Structure)) sk2ElemUniqueAddress = ((Structure)sk2Elem).UniqueAddress;
                            }
                            if (sk1ElemUniqueAddress == string.Empty)
                            {
                                // Forcing ElemUniqueAddress at sk1Ope address + 1, it could should be an included element
                                if (sk1Ope.OriginalOpArr.Length > 1) sk1ElemUniqueAddress = Tools.UniqueAddress(sk1Ope.BankNum, sk1Ope.AddressInt + 1);
                            }
                            else if (sk2ElemUniqueAddress == string.Empty)
                            {
                                // Forcing ElemUniqueAddress at sk2Ope address + 1, it could should be an included element
                                if (sk2Ope.OriginalOpArr.Length > 1) sk2ElemUniqueAddress = Tools.UniqueAddress(sk2Ope.BankNum, sk2Ope.AddressInt + 1);
                            }
                            if (sk1ElemUniqueAddress != string.Empty && sk2ElemUniqueAddress != string.Empty)
                            {
                                if (rsSkt1.slPossibleMatchingMixedElements.ContainsKey(sk1ElemUniqueAddress + "." + sk2ElemUniqueAddress))
                                {
                                    ((object[])rsSkt1.slPossibleMatchingMixedElements[sk1ElemUniqueAddress + "." + sk2ElemUniqueAddress])[2] = (int)((object[])rsSkt1.slPossibleMatchingMixedElements[sk1ElemUniqueAddress + "." + sk2ElemUniqueAddress])[2] + 1;
                                }
                                else
                                {
                                    rsSkt1.slPossibleMatchingMixedElements.Add(sk1ElemUniqueAddress + "." + sk2ElemUniqueAddress, new object[] { sk1ElemUniqueAddress, sk2ElemUniqueAddress, 1 });
                                }
                                if (rsSkt2.slPossibleMatchingMixedElements.ContainsKey(sk2ElemUniqueAddress + "." + sk1ElemUniqueAddress))
                                {
                                    ((object[])rsSkt2.slPossibleMatchingMixedElements[sk2ElemUniqueAddress + "." + sk1ElemUniqueAddress])[2] = (int)((object[])rsSkt2.slPossibleMatchingMixedElements[sk2ElemUniqueAddress + "." + sk1ElemUniqueAddress])[2] + 1;
                                }
                                else
                                {
                                    rsSkt2.slPossibleMatchingMixedElements.Add(sk2ElemUniqueAddress + "." + sk1ElemUniqueAddress, new object[] { sk2ElemUniqueAddress, sk1ElemUniqueAddress, 1 });
                                }
                            }
                        }

                        // Call Parameters
                        //  => For Register Only
                        if (sk1Ope.CallArgsArr != null && sk2Ope.CallArgsArr != null)
                        {
                            for (int iCallArg = 0; iCallArg < sk1Ope.CallArgsArr.Length && iCallArg < sk2Ope.CallArgsArr.Length; iCallArg++)
                            {
                                // Possible values samples : 12, abcd, R12, [abcd], R12++, [R12], [R12+abcd], ...

                                object[] arrPointersValues1 = Tools.InstructionPointersValues(sk1Ope.CallArgsArr[iCallArg]);
                                //  Returns 0,1 - 0 Not Pointer, 1 Pointer - Int
                                //          Pointer / Value 1   - String format
                                //          Pointer / Value 1   - Integer format
                                //          Pointer / Value 2   - String if exists
                                //          Pointer / Value 2   - Integer format if exists
                                if (!(bool)arrPointersValues1[0]) continue;

                                object[] arrPointersValues2 = Tools.InstructionPointersValues(sk2Ope.CallArgsArr[iCallArg]);
                                if (!(bool)arrPointersValues2[0]) continue;

                                string sk1Reg = arrPointersValues1[1].ToString();
                                if (arrPointersValues1.Length > 3) sk1Reg = arrPointersValues1[3].ToString();
                                string sk2Reg = arrPointersValues2[1].ToString();
                                if (arrPointersValues2.Length > 3) sk2Reg = arrPointersValues2[3].ToString();

                                if (rsSkt1.slPossibleMatchingRegisters.ContainsKey(sk1Reg + "." + sk2Reg))
                                {
                                    ((object[])rsSkt1.slPossibleMatchingRegisters[sk1Reg + "." + sk2Reg])[2] = (int)((object[])rsSkt1.slPossibleMatchingRegisters[sk1Reg + "." + sk2Reg])[2] + 1;
                                }
                                else
                                {
                                    rsSkt1.slPossibleMatchingRegisters.Add(sk1Reg + "." + sk2Reg, new object[] { sk1Reg, sk2Reg, 1, null });
                                }
                                if (rsSkt2.slPossibleMatchingRegisters.ContainsKey(sk2Reg + "." + sk1Reg))
                                {
                                    ((object[])rsSkt2.slPossibleMatchingRegisters[sk2Reg + "." + sk1Reg])[2] = (int)((object[])rsSkt2.slPossibleMatchingRegisters[sk2Reg + "." + sk1Reg])[2] + 1;
                                }
                                else
                                {
                                    rsSkt2.slPossibleMatchingRegisters.Add(sk2Reg + "." + sk1Reg, new object[] { sk2Reg, sk1Reg, 1, null });
                                }
                            }
                        }

                        // Possible for Elements, not for Registers
                        if (sk1Ope.OperationParams.Length != sk2Ope.OperationParams.Length) continue;

                        // CalculatedParams
                        //  => For Register Only
                        for (int iParam = 0; iParam < sk1Ope.OperationParams.Length && iParam < sk2Ope.OperationParams.Length; iParam++)
                        {
                            // Possible values samples : 12, abcd, R12, [abcd], R12++, [R12], [R12+abcd], ...

                            object[] arrPointersValues1 = Tools.InstructionPointersValues(sk1Ope.OperationParams[iParam].CalculatedParam);
                            //  Returns 0,1 - 0 Not Pointer, 1 Pointer - Int
                            //          Pointer / Value 1   - String format
                            //          Pointer / Value 1   - Integer format
                            //          Pointer / Value 2   - String if exists
                            //          Pointer / Value 2   - Integer format if exists
                            if (!(bool)arrPointersValues1[0]) continue;

                            object[] arrPointersValues2 = Tools.InstructionPointersValues(sk2Ope.OperationParams[iParam].CalculatedParam);
                            if (!(bool)arrPointersValues2[0]) continue;

                            if ((int)arrPointersValues1[2] >= SADDef.EecBankStartAddress) continue;
                            if ((int)arrPointersValues2[2] >= SADDef.EecBankStartAddress) continue;

                            string sk1Reg = arrPointersValues1[1].ToString();
                            if (arrPointersValues1.Length > 3)
                            {
                                if ((int)arrPointersValues1[4] >= SADDef.EecBankStartAddress) continue;
                                // Greater than 0x58 to keep only real adders
                                if ((int)arrPointersValues1[2] > 0x58)
                                {
                                    sk1Reg += SADDef.AdditionSeparator + arrPointersValues1[3].ToString();
                                }
                                else
                                {
                                    sk1Reg = arrPointersValues1[3].ToString();
                                }
                            }
                            string sk2Reg = arrPointersValues2[1].ToString();
                            if (arrPointersValues2.Length > 3)
                            {
                                if ((int)arrPointersValues2[4] >= SADDef.EecBankStartAddress) continue;
                                // Greater than 0x58 to keep only real adders
                                if ((int)arrPointersValues2[2] > 0x58)
                                {
                                    sk2Reg += SADDef.AdditionSeparator + arrPointersValues2[3].ToString();
                                }
                                else
                                {
                                    sk2Reg = arrPointersValues2[3].ToString();
                                }
                            }

                            if (rsSkt1.slPossibleMatchingRegisters.ContainsKey(sk1Reg + "." + sk2Reg))
                            {
                                ((object[])rsSkt1.slPossibleMatchingRegisters[sk1Reg + "." + sk2Reg])[2] = (int)((object[])rsSkt1.slPossibleMatchingRegisters[sk1Reg + "." + sk2Reg])[2] + 1;
                            }
                            else
                            {
                                rsSkt1.slPossibleMatchingRegisters.Add(sk1Reg + "." + sk2Reg, new object[] { sk1Reg, sk2Reg, 1, null });
                            }

                            if (rsSkt2.slPossibleMatchingRegisters.ContainsKey(sk2Reg + "." + sk1Reg))
                            {
                                ((object[])rsSkt2.slPossibleMatchingRegisters[sk2Reg + "." + sk1Reg])[2] = (int)((object[])rsSkt2.slPossibleMatchingRegisters[sk2Reg + "." + sk1Reg])[2] + 1;
                            }
                            else
                            {
                                rsSkt2.slPossibleMatchingRegisters.Add(sk2Reg + "." + sk1Reg, new object[] { sk2Reg, sk1Reg, 1, null });
                            }

                            //BitFlags cases
                            SortedList slBitFlags = null;
                            switch (sk1Ope.OriginalInstruction.ToLower())
                            {
                                case "jb":
                                case "jnb":
                                    int iBf1 = Convert.ToInt32(sk1Ope.OriginalOpArr[0], 16) - 0x30;
                                    if (iBf1 > 7) iBf1 -= 7;
                                    int iBf2 = Convert.ToInt32(sk2Ope.OriginalOpArr[0], 16) - 0x30;
                                    if (iBf2 > 7) iBf2 -= 7;

                                    slBitFlags = (SortedList)((object[])rsSkt1.slPossibleMatchingRegisters[sk1Reg + "." + sk2Reg])[3];
                                    if (slBitFlags == null)
                                    {
                                        slBitFlags = new SortedList();
                                        ((object[])rsSkt1.slPossibleMatchingRegisters[sk1Reg + "." + sk2Reg])[3] = slBitFlags;
                                    }
                                    if (slBitFlags.ContainsKey(iBf1.ToString() + "." + iBf2.ToString()))
                                    {
                                        ((object[])slBitFlags[iBf1.ToString() + "." + iBf2.ToString()])[2] = (int)((object[])slBitFlags[iBf1.ToString() + "." + iBf2.ToString()])[2] + 1;
                                    }
                                    else
                                    {
                                        slBitFlags.Add(iBf1.ToString() + "." + iBf2.ToString(), new object[] { iBf1, iBf2, 1 });
                                    }

                                    slBitFlags = (SortedList)((object[])rsSkt2.slPossibleMatchingRegisters[sk2Reg + "." + sk1Reg])[3];
                                    if (slBitFlags == null)
                                    {
                                        slBitFlags = new SortedList();
                                        ((object[])rsSkt2.slPossibleMatchingRegisters[sk2Reg + "." + sk1Reg])[3] = slBitFlags;
                                    }
                                    if (slBitFlags.ContainsKey(iBf2.ToString() + "." + iBf1.ToString()))
                                    {
                                        ((object[])slBitFlags[iBf2.ToString() + "." + iBf1.ToString()])[2] = (int)((object[])slBitFlags[iBf2.ToString() + "." + iBf1.ToString()])[2] + 1;
                                    }
                                    else
                                    {
                                        slBitFlags.Add(iBf2.ToString() + "." + iBf1.ToString(), new object[] { iBf2, iBf1, 1 });
                                    }
                                    break;
                            }
                            // 20200912 - orrb & an2b mngt for slPossibleMatchingBitFlags only
                            switch (sk1Ope.OriginalOpArr[0])
                            {
                                case "91":  // orrb
                                case "71":  // an2b
                                    int iBf1 = -1;
                                    int iBf2 = -1;
                                    BitArray arrBit1 = new BitArray(new int[] { Convert.ToInt32(sk1Ope.OriginalOpArr[1], 16) });
                                    BitArray arrBit2 = new BitArray(new int[] { Convert.ToInt32(sk2Ope.OriginalOpArr[1], 16) });
                                    for (int iBf = 0; iBf < 8; iBf++)
                                    {
                                        if (arrBit1.Get(iBf) == (sk1Ope.OriginalInstruction == "91")) iBf1 = iBf;
                                        if (arrBit2.Get(iBf) == (sk2Ope.OriginalInstruction == "91")) iBf2 = iBf;
                                        if (iBf1 != -1 && iBf2 != -1) break;
                                    }

                                    slBitFlags = (SortedList)((object[])rsSkt1.slPossibleMatchingRegisters[sk1Reg + "." + sk2Reg])[3];
                                    if (slBitFlags == null)
                                    {
                                        slBitFlags = new SortedList();
                                        ((object[])rsSkt1.slPossibleMatchingRegisters[sk1Reg + "." + sk2Reg])[3] = slBitFlags;
                                    }
                                    if (slBitFlags.ContainsKey(iBf1.ToString() + "." + iBf2.ToString()))
                                    {
                                        ((object[])slBitFlags[iBf1.ToString() + "." + iBf2.ToString()])[2] = (int)((object[])slBitFlags[iBf1.ToString() + "." + iBf2.ToString()])[2] + 1;
                                    }
                                    else
                                    {
                                        slBitFlags.Add(iBf1.ToString() + "." + iBf2.ToString(), new object[] { iBf1, iBf2, 1 });
                                    }

                                    slBitFlags = (SortedList)((object[])rsSkt2.slPossibleMatchingRegisters[sk2Reg + "." + sk1Reg])[3];
                                    if (slBitFlags == null)
                                    {
                                        slBitFlags = new SortedList();
                                        ((object[])rsSkt2.slPossibleMatchingRegisters[sk2Reg + "." + sk1Reg])[3] = slBitFlags;
                                    }
                                    if (slBitFlags.ContainsKey(iBf2.ToString() + "." + iBf1.ToString()))
                                    {
                                        ((object[])slBitFlags[iBf2.ToString() + "." + iBf1.ToString()])[2] = (int)((object[])slBitFlags[iBf2.ToString() + "." + iBf1.ToString()])[2] + 1;
                                    }
                                    else
                                    {
                                        slBitFlags.Add(iBf2.ToString() + "." + iBf1.ToString(), new object[] { iBf2, iBf1, 1 });
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public static object[] getMatchingRoutinesSkeletons(RoutineSkeleton rsSkt1, RoutineSkeleton rsSkt2, double startingChances, int minOpsNumber, double gapTolerance, double levTolerance)
        {
            //int minOpsNumber = 3;      // 3 ops for minimum
            //double gapTolerance = 0.1; // 10%
            //double levTolerance = 0.8; // 80%

            RoutineSkeleton resSkt1 = rsSkt1.Clone();
            RoutineSkeleton resSkt2 = rsSkt2.Clone();

            resSkt1.alMatches = new ArrayList(new object[] { resSkt2, 1.00 });
            resSkt2.alMatches = new ArrayList(new object[] { resSkt1, 1.00 });

            if (startingChances == 1.0) return new object[] { resSkt1, resSkt2 };    // Already 100%

            int prevlDist = DamerauLevenshteinDistance(resSkt1.Bytes, resSkt2.Bytes, (int)((1.0 - levTolerance) * (rsSkt1.OpsNumber + rsSkt2.OpsNumber) / 2));

            if (prevlDist < 2) return new object[] { resSkt1, resSkt2 };             // Only 0 to 1 character different, it is OK

            int numOpe1 = 0;
            int numOpe2 = 0;

            // Principle, remove operations while distance between Skeleton is not 0
            while (true)
            {
                if (numOpe1 >= resSkt1.alOperations.Count) break;
                if (numOpe2 >= resSkt2.alOperations.Count) break;

                Operation currentOpe = (Operation)resSkt1.alOperations[numOpe1];
                if (currentOpe == null)
                {
                    numOpe1++;
                    continue;
                }
                if (currentOpe.OriginalOpArr.Length <= 0)
                {
                    numOpe1++;
                    continue;
                }
                string instOpe1 = rsSkt1.getOperationSkeleton(currentOpe);

                currentOpe = (Operation)resSkt2.alOperations[numOpe2];
                if (currentOpe == null)
                {
                    numOpe2++;
                    continue;
                }
                if (currentOpe.OriginalOpArr.Length <= 0)
                {
                    numOpe2++;
                    continue;
                }
                string instOpe2 = rsSkt2.getOperationSkeleton(currentOpe);

                if (instOpe1 == instOpe2)
                {
                    numOpe1++;
                    numOpe2++;
                    continue;
                }

                int lDist = 0;
                bool bGoodResult = true;
                object removedObject1 = null;

                while (bGoodResult && numOpe1 < resSkt1.alOperations.Count)
                {
                    removedObject1 = resSkt1.alOperations[numOpe1];
                    resSkt1.alOperations.RemoveAt(numOpe1);
                    resSkt1.setSkeleton();

                    lDist = DamerauLevenshteinDistance(resSkt1.Bytes, resSkt2.Bytes, (int)((1.0 - levTolerance) * (rsSkt1.OpsNumber + rsSkt2.OpsNumber) / 2));
                    if (lDist < 0) bGoodResult = false; // More than Levenshtein Distance Tolerance
                    if (lDist >= prevlDist) bGoodResult = false;

                    if (bGoodResult)
                    {
                        prevlDist = lDist;
                        if (prevlDist < 2) break;
                    }
                }
                if (prevlDist < 2) break;
                if (bGoodResult) continue;

                resSkt1.alOperations.Insert(numOpe1, removedObject1);
                resSkt1.setSkeleton();
                numOpe1++;

                bGoodResult = true;
                object removedObject2 = null;

                while (bGoodResult && numOpe2 < resSkt2.alOperations.Count)
                {
                    removedObject2 = resSkt2.alOperations[numOpe2];
                    resSkt2.alOperations.RemoveAt(numOpe2);
                    resSkt2.setSkeleton();

                    lDist = DamerauLevenshteinDistance(resSkt1.Bytes, resSkt2.Bytes, (int)((1.0 - levTolerance) * (rsSkt1.OpsNumber + rsSkt2.OpsNumber) / 2));
                    if (lDist < 0) bGoodResult = false; // More than Levenshtein Distance Tolerance
                    if (lDist >= prevlDist) bGoodResult = false;

                    if (bGoodResult)
                    {
                        prevlDist = lDist;
                        if (prevlDist < 2) break;
                    }
                }
                if (prevlDist < 2) break;
                if (bGoodResult) continue;

                resSkt2.alOperations.Insert(numOpe2, removedObject2);
                resSkt2.setSkeleton();
                numOpe2++;
            }

            if (prevlDist < 2) return new object[] { resSkt1, resSkt2 };        // Only 0 to 1 character different, it is OK

            // Now we try to match only a part
            // at least 66% should be good
            int minGoodOpsNumber = (int)((double)resSkt1.alOperations.Count * 2.0 / 3.0);
            if (resSkt1.alOperations.Count > resSkt2.alOperations.Count) minGoodOpsNumber = (int)((double)resSkt2.alOperations.Count * 2.0 / 3.0);

            if (minGoodOpsNumber == 0 || minGoodOpsNumber < minOpsNumber) return null;
            
            // Matching the beginning first
            RoutineSkeleton resSkt1Part = resSkt1.Clone();
            RoutineSkeleton resSkt2Part = resSkt2.Clone();

            resSkt1Part.alMatches = new ArrayList(new object[] { resSkt1Part, 1.00 });
            resSkt2Part.alMatches = new ArrayList(new object[] { resSkt2Part, 1.00 });

            resSkt1Part.alOperations.RemoveRange(minGoodOpsNumber - 1, resSkt1Part.alOperations.Count - minGoodOpsNumber);
            resSkt2Part.alOperations.RemoveRange(minGoodOpsNumber - 1, resSkt2Part.alOperations.Count - minGoodOpsNumber);
            resSkt1Part.setSkeleton();
            resSkt2Part.setSkeleton();

            for (int opeIndex = minGoodOpsNumber; opeIndex < resSkt1.alOperations.Count && opeIndex < resSkt2.alOperations.Count; opeIndex++)
            {
                int lDist = DamerauLevenshteinDistance(resSkt1Part.Bytes, resSkt2Part.Bytes, (int)((1.0 - levTolerance) * (rsSkt1.OpsNumber + rsSkt2.OpsNumber) / 2));

                if (lDist > 1 || lDist < 0) // More than 1 character different or more than Tolerance, it is not good, no way to go further
                {
                    if (opeIndex == minGoodOpsNumber)
                    // Minimum requirement has not been reached
                    {
                        resSkt1Part = null;
                        resSkt2Part = null;
                    }
                    else
                    // Previous version is validated as good
                    {
                        resSkt1Part.alOperations.RemoveAt(resSkt1Part.alOperations.Count - 1);
                        resSkt2Part.alOperations.RemoveAt(resSkt2Part.alOperations.Count - 1);
                        resSkt1Part.setSkeleton();
                        resSkt2Part.setSkeleton();
                    }
                    break;
                }

                // Only 0 to 1 character different, it is OK and time to add other operations
                resSkt1Part.alOperations.Add(resSkt1.alOperations[opeIndex]);
                resSkt2Part.alOperations.Add(resSkt2.alOperations[opeIndex]);
                resSkt1Part.setSkeleton();
                resSkt2Part.setSkeleton();

                // Impossible to reach max opeIndex because it was not validated integrally before
            }

            // Validated with minimum required
            if (resSkt1Part != null && resSkt2Part != null) return new object[] { resSkt1Part, resSkt2Part };

            // Matching the end, because the beginning is not working
            resSkt1Part = resSkt1.Clone();
            resSkt2Part = resSkt2.Clone();

            resSkt1Part.alMatches = new ArrayList(new object[] { resSkt1Part, 1.00 });
            resSkt2Part.alMatches = new ArrayList(new object[] { resSkt2Part, 1.00 });

            resSkt1Part.alOperations.RemoveRange(0, resSkt1Part.alOperations.Count - minGoodOpsNumber);
            resSkt2Part.alOperations.RemoveRange(0, resSkt2Part.alOperations.Count - minGoodOpsNumber);
            resSkt1Part.setSkeleton();
            resSkt2Part.setSkeleton();

            for (int opeIndex = minGoodOpsNumber; opeIndex < resSkt1.alOperations.Count && opeIndex < resSkt2.alOperations.Count; opeIndex++)
            {
                int lDist = DamerauLevenshteinDistance(resSkt1Part.Bytes, resSkt2Part.Bytes, (int)((1.0 - levTolerance) * (rsSkt1.OpsNumber + rsSkt2.OpsNumber) / 2));

                if (lDist > 1 || lDist < 0) // More than 1 character different or more than Tolerance, it is not good, no way to go further
                {
                    if (opeIndex == minGoodOpsNumber)
                    // Minimum requirement has not been reached
                    {
                        resSkt1Part = null;
                        resSkt2Part = null;
                    }
                    else
                    // Previous version is validated as good
                    {
                        resSkt1Part.alOperations.RemoveAt(0);
                        resSkt2Part.alOperations.RemoveAt(0);
                        resSkt1Part.setSkeleton();
                        resSkt2Part.setSkeleton();
                    }
                    break;
                }

                // Only 0 to 1 character different, it is OK and time to add other operations
                resSkt1Part.alOperations.Insert(0, resSkt1.alOperations[resSkt1.alOperations.Count - opeIndex - 1]);
                resSkt2Part.alOperations.Insert(0, resSkt2.alOperations[resSkt2.alOperations.Count - opeIndex - 1]);
                resSkt1Part.setSkeleton();
                resSkt2Part.setSkeleton();

                // Impossible to reach max opeIndex because it was not validated integrally before
            }

            // Validated with minimum required
            if (resSkt1Part != null && resSkt2Part != null) return new object[] { resSkt1Part, resSkt2Part };

            // No way to manage it for now
            
            return null;
        }

        public static void exportReportRoutinesComparisonSkeletons(string reportPath, ref SortedList slRoutinesSkt1, string binaryFileName, ref SortedList slRoutinesSkt2, string sktFilePath2, int minOpsNumber, double gapTolerance, double levTolerance, ref SortedList slRegistersReporting, ref SortedList slStructuresReporting, ref SortedList slTablesReporting, ref SortedList slFunctionsReporting, ref SortedList slScalarsReporting)
        {
            Exception errorException = null;

            StreamWriter sWri = null;

            try
            {
                FileInfo fiFI = new FileInfo(sktFilePath2);
                string skt2FileName = fiFI.Name;
                fiFI = null;

                int rwMatching = 0;
                int maxMatchedRoutines = 0;

                sWri = new StreamWriter(reportPath, false, new UTF8Encoding(false));

                sWri.WriteLine("Source Binary : " + binaryFileName);
                sWri.WriteLine("\tCompared skeleton : " + skt2FileName + "\n");

                sWri.WriteLine("\t\tMinimum Operations Count : " + minOpsNumber.ToString());
                sWri.WriteLine("\t\tOperations Count Gap Tolerance % : " + string.Format("{0:0.00}", gapTolerance * 100.0));
                sWri.WriteLine("\t\tDamerau Levenshtein Distance Tolerance % : " + string.Format("{0:0.00}", levTolerance * 100.0) + "\n");

                sWri.WriteLine("Routines Possible matches :\n");

                foreach (RoutineSkeleton rsMainSkt in slRoutinesSkt1.Values)
                {
                    if (rsMainSkt.alMatches == null) continue;
                    if (rsMainSkt.alMatches.Count == 0) continue;

                    rwMatching++;
                    if (rsMainSkt.alMatches.Count > maxMatchedRoutines) maxMatchedRoutines = rsMainSkt.alMatches.Count;

                    sWri.WriteLine("++ " + rsMainSkt.UniqueAddressHex + ((rsMainSkt.FullLabel == string.Empty) ? string.Empty : " - " + rsMainSkt.FullLabel));
                    if (rsMainSkt.alMixedElements != null)
                    {
                        if (rsMainSkt.alMixedElements.Count > 0)
                        {
                            sWri.WriteLine("\t** Elements : " + rsMainSkt.alMixedElements.Count.ToString());
                        }
                    }
                    foreach (object[] arrRes in rsMainSkt.alMatches)
                    {
                        RoutineSkeleton rsSubSkt = (RoutineSkeleton)arrRes[0];
                        double dProximity = (double)arrRes[1];
                        sWri.WriteLine("\t++ " + rsSubSkt.UniqueAddressHex + ((rsSubSkt.FullLabel == string.Empty) ? string.Empty : " - " + rsSubSkt.FullLabel));
                        sWri.WriteLine("\t\t% Chances : " + string.Format("{0:0.00}", dProximity * 100.0));
                        if (rsSubSkt.alMixedElements != null)
                        {
                            if (rsSubSkt.alMixedElements.Count > 0)
                            {
                                sWri.WriteLine("\t\t** Elements : " + rsSubSkt.alMixedElements.Count.ToString());
                            }
                        }
                        sWri.WriteLine("\t-- " + Tools.UniqueAddressHex(rsSubSkt.BankNum, rsSubSkt.LastOperationAddressInt));
                    }
                    sWri.WriteLine("-- " + Tools.UniqueAddressHex(rsMainSkt.BankNum, rsMainSkt.LastOperationAddressInt) + " - Operations Count(" + rsMainSkt.OpsNumber + ")\n");
                }

                if (slStructuresReporting != null)
                {
                    if (slStructuresReporting.Count > 0)
                    {
                        sWri.WriteLine("\n\nStructures Possible matches :\n");

                        foreach (object[] EMatch in slStructuresReporting.Values)
                        {
                            string curType = EMatch[0].ToString();
                            string curUniqueAddressHex = EMatch[1].ToString();
                            string curFullLabel = EMatch[2].ToString();
                            SortedList slMatches = (SortedList)EMatch[3];

                            sWri.WriteLine("++ " + curUniqueAddressHex + " - " + curFullLabel);
                            foreach (string[] ECMatch in slMatches.Values)
                            {
                                string cmpType = ECMatch[0].ToString();
                                string cmpUniqueAddressHex = ECMatch[1].ToString();
                                string cmpFullLabel = ECMatch[2].ToString();
                                string cmpOccs = ECMatch[3].ToString();

                                sWri.WriteLine("\t++ " + cmpUniqueAddressHex + " - " + cmpFullLabel);
                                sWri.WriteLine("\t\tOccurrences : " + cmpOccs);
                                if (curType != cmpType) sWri.WriteLine("\t\tDifferent type : " + cmpType.ToLower());
                                sWri.WriteLine("\t--");
                            }
                            sWri.WriteLine("--\n");
                        }
                    }
                }
                if (slTablesReporting != null)
                {
                    if (slTablesReporting.Count > 0)
                    {
                        sWri.WriteLine("\n\nTables Possible matches :\n");

                        foreach (object[] EMatch in slTablesReporting.Values)
                        {
                            string curType = EMatch[0].ToString();
                            string curUniqueAddressHex = EMatch[1].ToString();
                            string curFullLabel = EMatch[2].ToString();
                            SortedList slMatches = (SortedList)EMatch[3];

                            sWri.WriteLine("++ " + curUniqueAddressHex + " - " + curFullLabel);
                            foreach (string[] ECMatch in slMatches.Values)
                            {
                                string cmpType = ECMatch[0].ToString();
                                string cmpUniqueAddressHex = ECMatch[1].ToString();
                                string cmpFullLabel = ECMatch[2].ToString();
                                string cmpOccs = ECMatch[3].ToString();

                                sWri.WriteLine("\t++ " + cmpUniqueAddressHex + " - " + cmpFullLabel);
                                sWri.WriteLine("\t\tOccurrences : " + cmpOccs);
                                if (curType != cmpType) sWri.WriteLine("\t\tDifferent type : " + cmpType.ToLower());
                                sWri.WriteLine("\t--");
                            }
                            sWri.WriteLine("--\n");
                        }
                    }
                }
                if (slFunctionsReporting != null)
                {
                    if (slFunctionsReporting.Count > 0)
                    {
                        sWri.WriteLine("\n\nFunctions Possible matches :\n");

                        foreach (object[] EMatch in slFunctionsReporting.Values)
                        {
                            string curType = EMatch[0].ToString();
                            string curUniqueAddressHex = EMatch[1].ToString();
                            string curFullLabel = EMatch[2].ToString();
                            SortedList slMatches = (SortedList)EMatch[3];

                            sWri.WriteLine("++ " + curUniqueAddressHex + " - " + curFullLabel);
                            foreach (string[] ECMatch in slMatches.Values)
                            {
                                string cmpType = ECMatch[0].ToString();
                                string cmpUniqueAddressHex = ECMatch[1].ToString();
                                string cmpFullLabel = ECMatch[2].ToString();
                                string cmpOccs = ECMatch[3].ToString();

                                sWri.WriteLine("\t++ " + cmpUniqueAddressHex + " - " + cmpFullLabel);
                                sWri.WriteLine("\t\tOccurrences : " + cmpOccs);
                                if (curType != cmpType) sWri.WriteLine("\t\tDifferent type : " + cmpType.ToLower());
                                sWri.WriteLine("\t--");
                            }
                            sWri.WriteLine("--\n");
                        }
                    }
                }
                if (slScalarsReporting != null)
                {
                    if (slScalarsReporting.Count > 0)
                    {
                        sWri.WriteLine("\n\nScalars Possible matches :\n");

                        foreach (object[] EMatch in slScalarsReporting.Values)
                        {
                            string curType = EMatch[0].ToString();
                            string curUniqueAddressHex = EMatch[1].ToString();
                            string curFullLabel = EMatch[2].ToString();
                            SortedList slMatches = (SortedList)EMatch[3];

                            sWri.WriteLine("++ " + curUniqueAddressHex + " - " + curFullLabel);
                            foreach (string[] ECMatch in slMatches.Values)
                            {
                                string cmpType = ECMatch[0].ToString();
                                string cmpUniqueAddressHex = ECMatch[1].ToString();
                                string cmpFullLabel = ECMatch[2].ToString();
                                string cmpOccs = ECMatch[3].ToString();

                                sWri.WriteLine("\t++ " + cmpUniqueAddressHex + " - " + cmpFullLabel);
                                sWri.WriteLine("\t\tOccurrences : " + cmpOccs);
                                if (curType != cmpType) sWri.WriteLine("\t\tDifferent type : " + cmpType.ToLower());
                                sWri.WriteLine("\t--");
                            }
                            sWri.WriteLine("--\n");
                        }
                    }
                }
                if (slRegistersReporting != null)
                {
                    if (slRegistersReporting.Count > 0)
                    {
                        sWri.WriteLine("\n\nRegisters Possible matches :\n");

                        foreach (object[] EMatch in slRegistersReporting.Values)
                        {
                            string curFullLabel = EMatch[0].ToString();
                            SortedList slMatches = (SortedList)EMatch[1];

                            sWri.WriteLine("++ " + curFullLabel);
                            foreach (string[] ECMatch in slMatches.Values)
                            {
                                string cmpFullLabel = ECMatch[0].ToString();
                                string cmpOccs = ECMatch[1].ToString();

                                sWri.WriteLine("\t++ " + cmpFullLabel);
                                sWri.WriteLine("\t\tOccurrences : " + cmpOccs);
                                sWri.WriteLine("\t--");
                            }
                            sWri.WriteLine("--\n");
                        }
                    }
                }

                sWri.WriteLine("## Routines with matching (" + rwMatching.ToString() + "), Maximum matched routines(" + maxMatchedRoutines.ToString() + ")");
            }
            catch (Exception ex)
            {
                errorException = ex;
            }
            finally
            {
                try { sWri.Close(); }
                catch { }
                try { sWri.Dispose(); }
                catch { }
                sWri = null;

                GC.Collect();
            }

            if (errorException != null) throw errorException;
        }

        /// <summary>
        ///     Calculate the difference between 2 strings using the Levenshtein distance algorithm
        /// </summary>
        /// <param name="source1">First string</param>
        /// <param name="source2">Second string</param>
        /// <returns></returns>
        public static int LevenshteinDistance(string source1, string source2) //O(n*m)
        {
            var source1Length = source1.Length;
            var source2Length = source2.Length;

            var matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix[i, 0] = i++) { }
            for (var j = 0; j <= source2Length; matrix[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            return matrix[source1Length, source2Length];
        }

        /*
        public static int GetHammingDistance(string s, string t)
        {
            if (s.Length != t.Length)
            {
                throw new Exception("Strings must be equal length");
            }

            int distance =
                s.ToCharArray()
                .Zip(t.ToCharArray(), (c1, c2) => new { c1, c2 })
                .Count(m => m.c1 != m.c2);

            return distance;
        }
        */

        public static int DamerauLevenshteinDistance(string s, string t)
        {
            var bounds = new { Height = s.Length + 1, Width = t.Length + 1 };

            int[,] matrix = new int[bounds.Height, bounds.Width];

            for (int height = 0; height < bounds.Height; height++) { matrix[height, 0] = height; };
            for (int width = 0; width < bounds.Width; width++) { matrix[0, width] = width; };

            for (int height = 1; height < bounds.Height; height++)
            {
                for (int width = 1; width < bounds.Width; width++)
                {
                    int cost = (s[height - 1] == t[width - 1]) ? 0 : 1;
                    int insertion = matrix[height, width - 1] + 1;
                    int deletion = matrix[height - 1, width] + 1;
                    int substitution = matrix[height - 1, width - 1] + cost;

                    int distance = Math.Min(insertion, Math.Min(deletion, substitution));

                    if (height > 1 && width > 1 && s[height - 1] == t[width - 2] && s[height - 2] == t[width - 1])
                    {
                        distance = Math.Min(distance, matrix[height - 2, width - 2] + cost);
                    }

                    matrix[height, width] = distance;
                }
            }

            return matrix[bounds.Height - 1, bounds.Width - 1];
        }

        /// Computes and returns the Damerau-Levenshtein edit distance between two strings, 
        /// i.e. the number of insertion, deletion, sustitution, and transposition edits
        /// required to transform one string to the other. This value will be >= 0, where 0
        /// indicates identical strings. Comparisons are case sensitive, so for example, 
        /// "Fred" and "fred" will have a distance of 1. This algorithm is basically the
        /// Levenshtein algorithm with a modification that considers transposition of two
        /// adjacent characters as a single edit.
        /// http://blog.softwx.net/2015/01/optimizing-damerau-levenshtein_15.html
        /// </summary>
        /// <remarks>See http://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
        /// Note that this is based on Sten Hjelmqvist's "Fast, memory efficient" algorithm, described
        /// at http://www.codeproject.com/Articles/13525/Fast-memory-efficient-Levenshtein-algorithm.
        /// This version differs by including some optimizations, and extending it to the Damerau-
        /// Levenshtein algorithm.
        /// Note that this is the simpler and faster optimal string alignment (aka restricted edit) distance
        /// that difers slightly from the classic Damerau-Levenshtein algorithm by imposing the restriction
        /// that no substring is edited more than once. So for example, "CA" to "ABC" has an edit distance
        /// of 2 by a complete application of Damerau-Levenshtein, but a distance of 3 by this method that
        /// uses the optimal string alignment algorithm. See wikipedia article for more detail on this
        /// distinction.
        /// </remarks>
        /// <param name="s">String being compared for distance.</param>
        /// <param name="t">String being compared against other string.</param>
        /// <param name="maxDistance">The maximum edit distance of interest.</param>
        /// <returns>int edit distance, >= 0 representing the number of edits required
        /// to transform one string to the other, or -1 if the distance is greater than the specified maxDistance.</returns>
        public static int DamerauLevenshteinDistance(string s, string t, int maxDistance)
        {
            if (String.IsNullOrEmpty(s)) return ((t ?? "").Length <= maxDistance) ? (t ?? "").Length : -1;
            if (String.IsNullOrEmpty(t)) return (s.Length <= maxDistance) ? s.Length : -1;

            // if strings of different lengths, ensure shorter string is in s. This can result in a little
            // faster speed by spending more time spinning just the inner loop during the main processing.
            if (s.Length > t.Length)
            {
                var temp = s; s = t; t = temp; // swap s and t
            }
            int sLen = s.Length; // this is also the minimun length of the two strings
            int tLen = t.Length;

            // suffix common to both strings can be ignored
            while ((sLen > 0) && (s[sLen - 1] == t[tLen - 1])) { sLen--; tLen--; }

            int start = 0;
            if ((s[0] == t[0]) || (sLen == 0))
            { // if there's a shared prefix, or all s matches t's suffix
                // prefix common to both strings can be ignored
                while ((start < sLen) && (s[start] == t[start])) start++;
                sLen -= start; // length of the part excluding common prefix and suffix
                tLen -= start;

                // if all of shorter string matches prefix and/or suffix of longer string, then
                // edit distance is just the delete of additional characters present in longer string
                if (sLen == 0) return (tLen <= maxDistance) ? tLen : -1;

                t = t.Substring(start, tLen); // faster than t[start+j] in inner loop below
            }
            int lenDiff = tLen - sLen;
            if ((maxDistance < 0) || (maxDistance > tLen))
            {
                maxDistance = tLen;
            }
            else if (lenDiff > maxDistance) return -1;

            var v0 = new int[tLen];
            var v2 = new int[tLen]; // stores one level further back (offset by +1 position)
            int j;
            for (j = 0; j < maxDistance; j++) v0[j] = j + 1;
            for (; j < tLen; j++) v0[j] = maxDistance + 1;

            int jStartOffset = maxDistance - (tLen - sLen);
            bool haveMax = maxDistance < tLen;
            int jStart = 0;
            int jEnd = maxDistance;
            char sChar = s[0];
            int current = 0;
            for (int i = 0; i < sLen; i++)
            {
                char prevsChar = sChar;
                sChar = s[start + i];
                char tChar = t[0];
                int left = i;
                current = left + 1;
                int nextTransCost = 0;
                // no need to look beyond window of lower right diagonal - maxDistance cells (lower right diag is i - lenDiff)
                // and the upper left diagonal + maxDistance cells (upper left is i)
                jStart += (i > jStartOffset) ? 1 : 0;
                jEnd += (jEnd < tLen) ? 1 : 0;
                for (j = jStart; j < jEnd; j++)
                {
                    int above = current;
                    int thisTransCost = nextTransCost;
                    nextTransCost = v2[j];
                    v2[j] = current = left; // cost of diagonal (substitution)
                    left = v0[j];    // left now equals current cost (which will be diagonal at next iteration)
                    char prevtChar = tChar;
                    tChar = t[j];
                    if (sChar != tChar)
                    {
                        if (left < current) current = left;   // insertion
                        if (above < current) current = above; // deletion
                        current++;
                        if ((i != 0) && (j != 0)
                            && (sChar == prevtChar)
                            && (prevsChar == tChar))
                        {
                            thisTransCost++;
                            if (thisTransCost < current) current = thisTransCost; // transposition
                        }
                    }
                    v0[j] = current;
                }
                if (haveMax && (v0[i + lenDiff] > maxDistance)) return -1;
            }
            return (current <= maxDistance) ? current : -1;
        }
    }
}
