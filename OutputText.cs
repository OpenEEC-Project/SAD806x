using System;
using System.Collections;
using System.Text;
using System.IO;

namespace SAD806x
{
    // OuputText - Standard Fixed Version
    public class OutputText
    {
        private SADBin sadBin = null;
        private SettingsLst toSettings = null;
        private string outputFilePath = null;

        // Shortcuts
        private SADCalib Calibration = null;
        private SADBank Bank8 = null;
        private SADBank Bank1 = null;
        private SADBank Bank9 = null;
        private SADBank Bank0 = null;
        private SADS6x S6x = null;

        private string[] arrBytes = null;

        private bool isCorrupted = false;
        private bool is8061 = false;
        private bool isEarly = false;
        private bool isPilot = false;

        private string binaryFileName = string.Empty;

        public OutputText(ref SADBin sbSadBin, string textOutputFilePath, ref SettingsLst slToSettings)
        {
            sadBin = sbSadBin;
            outputFilePath = textOutputFilePath;
            toSettings = slToSettings;

            // Shortcuts
            Calibration = sadBin.Calibration;
            Bank8 = sadBin.Bank8;
            Bank1 = sadBin.Bank1;
            Bank9 = sadBin.Bank9;
            Bank0 = sadBin.Bank0;
            S6x = sadBin.S6x;

            arrBytes = sadBin.getBinBytes;
            
            isCorrupted = sadBin.isCorrupted;
            is8061 = sadBin.is8061;
            isEarly = sadBin.isEarly;
            isPilot = sadBin.isPilot;
        }

        public void processOutputText()
        {
            SortedList slElements = null;
            TextWriter txWriter = null;
            Element elem = null;
            ArrayList alErrors = null;
            object[] previousElementOutputResult = null;
            int bankNum = -1;

            sadBin.ProgressStatus = 0;
            sadBin.ProgressLabel = string.Empty;

            sadBin.OutputStartTime = DateTime.Now;

            sadBin.Errors = null;
            alErrors = new ArrayList();

            // Prepare Elements for Output
            slElements = new SortedList();
            if (Bank8 != null)
            {
                foreach (ReservedAddress rAddress in Bank8.slReserved.Values)
                {
                    try { slElements.Add(rAddress.UniqueAddressHex, new Element(rAddress, MergedType.ReservedAddress)); }
                    catch { alErrors.Add(rAddress.UniqueAddressHex); }
                }
                // Managed by ReservedAddresses
                //foreach (Vector vect in Bank8.slIntVectors.Values) slElements.Add(vect.UniqueAddressHex, new Element(vect, MergedType.Vector));
                foreach (Operation ope in Bank8.slOPs.Values)
                {

                    try { slElements.Add(ope.UniqueAddressHex, new Element(ope, MergedType.Operation)); }
                    catch { alErrors.Add(ope.UniqueAddressHex); }

                }
                foreach (UnknownOpPart unkOpPart in Bank8.slUnknownOpParts.Values)
                {
                    foreach (UnknownOpPartLine uLine in unkOpPart.Lines)
                    {
                        try { slElements.Add(uLine.UniqueAddressHex, new Element(uLine, MergedType.UnknownOperationLine)); }
                        catch { alErrors.Add(uLine.UniqueAddressHex); }
                    }
                }
            }
            if (Bank1 != null)
            {
                foreach (ReservedAddress rAddress in Bank1.slReserved.Values)
                {
                    try { slElements.Add(rAddress.UniqueAddressHex, new Element(rAddress, MergedType.ReservedAddress)); }
                    catch { alErrors.Add(rAddress.UniqueAddressHex); }
                }
                // Managed by ReservedAddresses
                //foreach (Vector vect in Bank1.slIntVectors.Values) slElements.Add(vect.UniqueAddressHex, new Element(vect, MergedType.Vector));
                foreach (Operation ope in Bank1.slOPs.Values)
                {
                    try { slElements.Add(ope.UniqueAddressHex, new Element(ope, MergedType.Operation)); }
                    catch { alErrors.Add(ope.UniqueAddressHex); }
                }
                foreach (UnknownOpPart unkOpPart in Bank1.slUnknownOpParts.Values)
                {
                    foreach (UnknownOpPartLine uLine in unkOpPart.Lines)
                    {
                        try { slElements.Add(uLine.UniqueAddressHex, new Element(uLine, MergedType.UnknownOperationLine)); }
                        catch { alErrors.Add(uLine.UniqueAddressHex); }
                    }
                }
            }
            if (Bank9 != null)
            {
                foreach (ReservedAddress rAddress in Bank9.slReserved.Values)
                {
                    try { slElements.Add(rAddress.UniqueAddressHex, new Element(rAddress, MergedType.ReservedAddress)); }
                    catch { alErrors.Add(rAddress.UniqueAddressHex); }
                }
                // Managed by ReservedAddresses
                //foreach (Vector vect in Bank9.slIntVectors.Values) slElements.Add(vect.UniqueAddressHex, new Element(vect, MergedType.Vector));
                foreach (Operation ope in Bank9.slOPs.Values)
                {
                    try { slElements.Add(ope.UniqueAddressHex, new Element(ope, MergedType.Operation)); }
                    catch { alErrors.Add(ope.UniqueAddressHex); }
                }
                foreach (UnknownOpPart unkOpPart in Bank9.slUnknownOpParts.Values)
                {
                    foreach (UnknownOpPartLine uLine in unkOpPart.Lines)
                    {
                        try { slElements.Add(uLine.UniqueAddressHex, new Element(uLine, MergedType.UnknownOperationLine)); }
                        catch { alErrors.Add(uLine.UniqueAddressHex); }
                    }
                }
            }
            if (Bank0 != null)
            {
                foreach (ReservedAddress rAddress in Bank0.slReserved.Values)
                {
                    try { slElements.Add(rAddress.UniqueAddressHex, new Element(rAddress, MergedType.ReservedAddress)); }
                    catch { alErrors.Add(rAddress.UniqueAddressHex); }
                }
                // Managed by ReservedAddresses
                //foreach (Vector vect in Bank0.slIntVectors.Values) slElements.Add(vect.UniqueAddressHex, new Element(vect, MergedType.Vector));
                foreach (Operation ope in Bank0.slOPs.Values)
                {
                    try { slElements.Add(ope.UniqueAddressHex, new Element(ope, MergedType.Operation)); }
                    catch { alErrors.Add(ope.UniqueAddressHex); }
                }
                foreach (UnknownOpPart unkOpPart in Bank0.slUnknownOpParts.Values)
                {
                    foreach (UnknownOpPartLine uLine in unkOpPart.Lines)
                    {
                        try { slElements.Add(uLine.UniqueAddressHex, new Element(uLine, MergedType.UnknownOperationLine)); }
                        catch { alErrors.Add(uLine.UniqueAddressHex); }
                    }
                }
            }

            foreach (Vector vect in Calibration.slAdditionalVectors.Values)
            {
                try { slElements.Add(vect.UniqueSourceAddressHex, new Element(vect, MergedType.Vector)); }
                catch { alErrors.Add(vect.UniqueSourceAddressHex); }
            }

            foreach (CalibrationElement calElem in Calibration.slCalibrationElements.Values)
            {
                if (!calElem.isTypeIdentified) continue;
                if (calElem.isStructure)
                {
                    if (calElem.StructureElem.isVectorsList) continue; // Will be Managed at Vector Level
                    if (calElem.StructureElem.ParentStructure != null) continue; // Will be Managed on second run
                }

                try { slElements.Add(calElem.UniqueAddressHex, new Element(calElem, MergedType.CalibrationElement)); }
                catch { alErrors.Add(calElem.UniqueAddressHex); }
            }

            // Second Run for Included Structures
            foreach (CalibrationElement calElem in Calibration.slCalibrationElements.Values)
            {
                if (!calElem.isTypeIdentified) continue;
                if (!calElem.isStructure) continue;
                if (calElem.StructureElem.isVectorsList) continue; // Will be Managed at Vector Level
                if (calElem.StructureElem.ParentStructure == null) continue; // Already managed in first run

                Element eElem = (Element)slElements[calElem.StructureElem.ParentStructure.UniqueAddressHex];
                if (eElem != null)
                {
                    if (eElem.IncludedElements == null) eElem.IncludedElements = new SortedList();
                    if (!eElem.IncludedElements.ContainsKey(calElem.AddressInt)) eElem.IncludedElements.Add(calElem.AddressInt, new Element(calElem, MergedType.CalibrationElement));
                    eElem = null;
                }
            }

            foreach (Table extObject in Calibration.slExtTables.Values)
            {
                try { slElements.Add(extObject.UniqueAddressHex, new Element(extObject, MergedType.ExtTable)); }
                catch { alErrors.Add(extObject.UniqueAddressHex); }
            }

            foreach (Function extObject in Calibration.slExtFunctions.Values)
            {
                try { slElements.Add(extObject.UniqueAddressHex, new Element(extObject, MergedType.ExtFunction)); }
                catch { alErrors.Add(extObject.UniqueAddressHex); }
            }

            foreach (Scalar extObject in Calibration.slExtScalars.Values)
            {
                try { slElements.Add(extObject.UniqueAddressHex, new Element(extObject, MergedType.ExtScalar)); }
                catch { alErrors.Add(extObject.UniqueAddressHex); }
            }

            foreach (Structure sStruct in Calibration.slExtStructures.Values)
            {
                if (sStruct.isVectorsList) continue; // Will be Managed at Vector Level
                if (sStruct.ParentStructure != null) continue; // Will be Managed on second run

                try { slElements.Add(sStruct.UniqueAddressHex, new Element(sStruct, MergedType.ExtStructure)); }
                catch { alErrors.Add(sStruct.UniqueAddressHex); }
            }

            // Second Run for Included Structures
            foreach (Structure sStruct in Calibration.slExtStructures.Values)
            {
                if (sStruct.isVectorsList) continue; // Will be Managed at Vector Level
                if (sStruct.ParentStructure == null) continue; // Already managed in first run

                Element eElem = (Element)slElements[sStruct.ParentStructure.UniqueAddressHex];
                if (eElem != null)
                {
                    if (eElem.IncludedElements == null) eElem.IncludedElements = new SortedList();
                    if (!eElem.IncludedElements.ContainsKey(sStruct.AddressInt)) eElem.IncludedElements.Add(sStruct.AddressInt, new Element(sStruct, MergedType.ExtStructure));
                    eElem = null;
                }
            }

            foreach (UnknownCalibPart unkCalibPart in Calibration.slUnknownCalibParts.Values)
            {
                foreach (UnknownCalibPartLine uLine in unkCalibPart.Lines)
                {
                    try { slElements.Add(uLine.UniqueAddressHex, new Element(uLine, MergedType.UnknownCalibrationLine)); }
                    catch { alErrors.Add(uLine.UniqueAddressHex); }
                }
            }

            // Included Elements Calculation
            Element[] prevElems = null;
            foreach (Element incElem in slElements.Values)
            {
                bool bNextElem = false;

                if (!bNextElem) if (prevElems == null) bNextElem = true;

                if (!bNextElem) if (prevElems[0].BankNum != incElem.BankNum) bNextElem = true;

                if (!bNextElem)
                {
                    if (prevElems[0].AddressEndInt < incElem.AddressInt)
                    {
                        bNextElem = true;

                        if (incElem.IncludedElements != null)
                        {
                            foreach (Element subIncElem in incElem.IncludedElements.Values)
                            {
                                for (int iElem = 0; iElem < prevElems.Length; iElem++)
                                {
                                    if (prevElems[iElem].AddressInt > subIncElem.AddressInt) continue;
                                    if (prevElems[iElem].AddressEndInt < subIncElem.AddressInt) continue;

                                    if (prevElems[iElem].IncludedElements == null) prevElems[iElem].IncludedElements = new SortedList();
                                    if (!prevElems[iElem].IncludedElements.ContainsKey(subIncElem.AddressInt)) prevElems[iElem].IncludedElements.Add(subIncElem.AddressInt, subIncElem);
                                    break;
                                }
                            }
                        }
                    }
                }

                if (bNextElem)
                {
                    if (prevElems == null) prevElems = new Element[10];
                    for (int iElem = prevElems.Length - 1; iElem > 0; iElem--) prevElems[iElem] = prevElems[iElem - 1];
                    prevElems[0] = incElem;
                    continue;
                }
                incElem.isIncluded = true;
                if (prevElems[0].IncludedElements == null) prevElems[0].IncludedElements = new SortedList();
                if (!prevElems[0].IncludedElements.ContainsKey(incElem.AddressInt)) prevElems[0].IncludedElements.Add(incElem.AddressInt, incElem);
                if (incElem.AddressEndInt > prevElems[0].AddressEndInt)
                {
                    for (int iElem = prevElems.Length - 1; iElem > 0; iElem--) prevElems[iElem] = prevElems[iElem - 1];
                    prevElems[0] = incElem;
                }
            }
            prevElems = null;

            sadBin.ProgressStatus += 50;
            sadBin.ProgressLabel += "Elements Processed.";

            txWriter = new StreamWriter(outputFilePath, false, Encoding.UTF8);

            // Header
            processOutputTextHeader(ref txWriter);

            // Registers Lists
            if (S6x.Properties.RegListOutput) processOutputTextRegistersList(ref txWriter);

            previousElementOutputResult = null;
            for (int iElem = 0; iElem < slElements.Count; iElem++)
            {
                elem = (Element)slElements.GetByIndex(iElem);

                if (bankNum != elem.BankNum)
                {
                    bankNum = elem.BankNum;
                    if (Calibration.Info.slBanksInfos.ContainsKey(bankNum)) processOutputTextBankHeader(ref txWriter, (string[])Calibration.Info.slBanksInfos[bankNum]);
                }

                processOutputTextElement(ref txWriter, ref elem, ref previousElementOutputResult);

                elem = null;

                if (iElem > 0 && iElem % 0x4000 == 0) txWriter.Flush();
            }

            processOutputTextFooter(ref txWriter);

            txWriter.Close();
            txWriter.Dispose();
            txWriter = null;

            sadBin.OutputEndTime = DateTime.Now;

            sadBin.ProgressStatus = 100;
            sadBin.ProgressLabel = "Output done.";

            if (alErrors.Count > 0)
            {
                sadBin.ProgressLabel = "Output done with errors.";
                sadBin.Errors = (string[])alErrors.ToArray(typeof(string));
            }
            alErrors = null;
        }

        private void processOutputTextHeader(ref TextWriter txWriter)
        {
            string col1 = string.Empty;
            string col2 = string.Empty;
            string sFormat = string.Empty;

            // Disassembly Header
            if (is8061) col1 = "8061";
            else col1 = "8065";
            if (isCorrupted) col1 += "!";
            if (isEarly) col1 += "β";
            if (isPilot) col1 += "α";
            col1 += " Disassembly";

            txWriter.WriteLine(OutputTools.BorderedHeader());
            txWriter.WriteLine(OutputTools.BorderedEmpty());
            txWriter.WriteLine(OutputTools.BorderedTitle(col1));
            txWriter.WriteLine(OutputTools.BorderedEmpty());

            col1 = string.Format("{0,-30}", "Binary File :");
            col2 = string.Format("{0,-30}", "S6x file :");
            txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

            col1 = string.Format("{0,30}", binaryFileName);
            if (S6x.isValid) col2 = string.Format("{0,30}", S6x.FileName);
            else col2 = string.Format("{0,30}", "None");
            txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

            col1 = string.Format("{0,30}", arrBytes.Length.ToString() + " (" + Convert.ToString(arrBytes.Length, 16) + ") bytes");
            txWriter.WriteLine(OutputTools.BorderedText(col1));

            col1 = string.Empty;
            col2 = string.Empty;
            if (Calibration.Info.VidStrategy != string.Empty) col1 = string.Format("{0,30}", Calibration.Info.VidStrategy + "(" + Calibration.Info.VidStrategyVersion + ") Strategy");
            if (Calibration.Info.VidSerial != string.Empty) col2 = string.Format("{0,30}", "Part Number " + Calibration.Info.VidSerial);
            if (col1 != string.Empty || col2 != string.Empty) txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

            col1 = string.Empty;
            col2 = string.Empty;
            if (Calibration.Info.VidVIN != string.Empty) col1 = string.Format("{0,30}", "VIN " + Calibration.Info.VidVIN);
            if (Calibration.Info.VidPatsCode != string.Empty) col2 = string.Format("{0,30}", "PATS " + Calibration.Info.VidPatsCode);
            if (col1 != string.Empty || col2 != string.Empty) txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

            if (!is8061)
            {
                col1 = string.Empty;
                col2 = string.Empty;
                if (Calibration.Info.VidRevMile > 0) col1 = string.Format("{0,30}", "Rev/Mile " + Calibration.Info.VidRevMile.ToString());
                if (Calibration.Info.VidRtAxle > 0) col2 = string.Format("{0,30}", "Rt Axle " + Calibration.Info.VidRtAxle.ToString());
                if (col1 != string.Empty || col2 != string.Empty) txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

                if (Calibration.Info.VidEnabled) col1 = string.Format("{0,30}", "VID Enabled");
                else col1 = string.Format("{0,30}", "VID Disabled");
                txWriter.WriteLine(OutputTools.BorderedText(col1));
            }

            txWriter.WriteLine(OutputTools.BorderedEmpty());

            col1 = string.Format("{0,-30}", "Options :");
            col2 = string.Format("{0,30}", string.Empty);
            txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

            col1 = string.Format("{0,30}", "Default options");
            txWriter.WriteLine(OutputTools.BorderedText(col1));

            txWriter.WriteLine(OutputTools.BorderedEmpty());

            col1 = string.Format("{0,-30}", "CheckSum :");
            col2 = "SMP Base Address :";
            col2 += string.Format("{0," + (30 - col2.Length) + "}", string.Format("{0:x4}", Calibration.Info.SmpBaseAddress));
            txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

            col1 = string.Format("{0,30:x4}", Calibration.Info.CheckSum);
            col2 = "CC Exe Time :";
            col2 += string.Format("{0," + (30 - col2.Length) + "}", string.Format("{0:x4}", Calibration.Info.CcExeTime));
            txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

            if (Calibration.Info.isCheckSumConfirmed)
            {
                if (Calibration.Info.isCheckSumValid) col1 = "Valid";
                else col1 = "Invalid, try " + string.Format("{0:x4}", Calibration.Info.correctedChecksum);
            }
            else
            {
                col1 = "Not Calculated";
            }
            col1 = string.Format("{0,30}", col1);
            // Not available for Early 8061
            if (!is8061 || !isEarly)
            {
                col2 = "Levels Number :";
                col2 += string.Format("{0," + (30 - col2.Length) + "}", Calibration.Info.LevelsNum);
            }
            else
            {
                col2 = string.Format("{0,30}", string.Empty);
            }
            txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

            // Not available for Early 8061
            if (!is8061 || !isEarly)
            {
                col1 = string.Format("{0,30}", string.Empty);
                col2 = "Calibrations Number :";
                col2 += string.Format("{0," + (30 - col2.Length) + "}", Calibration.Info.CalibsNum);
                txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));
            }

            col1 = string.Format("{0,-30}", "Banks :");
            txWriter.WriteLine(OutputTools.BorderedText(col1));

            col1 = string.Empty;
            col2 = string.Empty;
            foreach (string[] bankInfos in Calibration.Info.slBanksInfos.Values)
            {
                if (col1 == string.Empty) col1 = string.Format("{0,14}{1,8}{2,8}", bankInfos[0], bankInfos[1], bankInfos[2]);
                else col2 = string.Format("{0,14}{1,8}{2,8}", bankInfos[0], bankInfos[1], bankInfos[2]);
                if (col1 != string.Empty && col2 != string.Empty)
                {
                    txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));
                    col1 = string.Empty;
                    col2 = string.Empty;
                }
            }
            if (col1 != string.Empty) txWriter.WriteLine(OutputTools.BorderedText(col1));

            txWriter.WriteLine(OutputTools.BorderedEmpty());

            col1 = string.Format("{0,-30}", "RBases :");
            txWriter.WriteLine(OutputTools.BorderedText(col1));

            col1 = string.Empty;
            col2 = string.Empty;
            foreach (RBase rBase in Calibration.slRbases.Values)
            {
                if (col1 == string.Empty) col1 = string.Format("{0,14}{1,8}{2,8}", rBase.Code, rBase.AddressBank, rBase.AddressBankEnd);
                else col2 = string.Format("{0,14}{1,8}{2,8}", rBase.Code, rBase.AddressBank, rBase.AddressBankEnd);
                if (col1 != string.Empty && col2 != string.Empty)
                {
                    txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));
                    col1 = string.Empty;
                    col2 = string.Empty;
                }
            }
            if (col1 != string.Empty) txWriter.WriteLine(OutputTools.BorderedText(col1));

            txWriter.WriteLine(OutputTools.BorderedEmpty());
            txWriter.WriteLine(OutputTools.BorderedFooter());

            // TEMPORARY CALL WITH ARGS
            /*
            txWriter.WriteLine("\r\n<Call with Args>");
            foreach (Call cCall in Calibration.slCalls.Values)
            {
                if (cCall.ArgsType != CallArgsType.None && cCall.ArgsType != CallArgsType.Unknown)
                {
                    sFormat = "{0:-6}: {1,-25}{2,-10}{3,-10}{4,-10}";
                    txWriter.WriteLine(string.Format(sFormat, cCall.UniqueAddressHex, cCall.Label, cCall.ArgsNum, cCall.ArgsType, cCall.ArgsMode));
                }
            }
            txWriter.WriteLine("</Call with Args>\r\n");
            */
        }

        private void processOutputTextFooter(ref TextWriter txWriter)
        {
            string sText = string.Empty;

            txWriter.WriteLine(OutputTools.BorderedHeader());
            txWriter.WriteLine(OutputTools.BorderedEmpty());
            sText = "End of Disassembly";
            txWriter.WriteLine(OutputTools.BorderedTitle(sText));
            txWriter.WriteLine(OutputTools.BorderedEmpty());
            txWriter.WriteLine(OutputTools.BorderedFooter());
        }

        private void processOutputTextRegistersList(ref TextWriter txWriter)
        {
            string col1 = string.Empty;
            string col2 = string.Empty;

            txWriter.WriteLine(OutputTools.BorderedHeader());
            txWriter.WriteLine(OutputTools.BorderedEmpty());
            col1 = "Registers List";
            txWriter.WriteLine(OutputTools.BorderedTitle(col1));
            col1 = string.Empty;

            txWriter.WriteLine(OutputTools.BorderedEmpty());

            // With Comments
            foreach (S6xRegister s6xReg in S6x.slRegisters.Values)
            {
                if (s6xReg.Skip || !s6xReg.Store) continue;
                if (s6xReg.FullLabel == Tools.RegisterInstruction(s6xReg.Address)) continue;
                if (s6xReg.Comments == null || s6xReg.Comments == string.Empty) continue;

                if (s6xReg.MultipleMeanings) col1 = string.Format("{0,-38}", s6xReg.FullLabels(true, true));
                else col1 = string.Format("{0,-38}", s6xReg.FullLabel);

                foreach (string sCom in s6xReg.Comments.Replace("\r", "\n").Replace("\n\n", "\n").Split('\n'))
                {
                    string pCom = sCom;
                    while (pCom.Length > 38)
                    {
                        col2 = string.Format("{0,-38}", pCom.Substring(0, 38));
                        txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));
                        if (col1 != string.Empty) col1 = string.Empty;
                        pCom = pCom.Substring(38);
                    }
                    if (pCom != string.Empty)
                    {
                        col2 = string.Format("{0,-38}", pCom);
                        txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));
                        if (col1 != string.Empty) col1 = string.Empty;
                    }
                }

                // MultipleMeanings second step for Word part
                if (s6xReg.MultipleMeanings)
                {
                    col1 = string.Format("{0,-38}", s6xReg.FullLabels(false, true));
                    foreach (string sCom in s6xReg.Comments.Replace("\r", "\n").Replace("\n\n", "\n").Split('\n'))
                    {
                        string pCom = sCom;
                        while (pCom.Length > 38)
                        {
                            col2 = string.Format("{0,-38}", pCom.Substring(0, 38));
                            txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));
                            if (col1 != string.Empty) col1 = string.Empty;
                            pCom = pCom.Substring(38);
                        }
                        if (pCom != string.Empty)
                        {
                            col2 = string.Format("{0,-38}", pCom);
                            txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));
                            if (col1 != string.Empty) col1 = string.Empty;
                        }
                    }
                }
            }
            col1 = string.Empty;
            col2 = string.Empty;

            // Without Comments
            foreach (S6xRegister s6xReg in S6x.slRegisters.Values)
            {
                if (s6xReg.Skip || !s6xReg.Store) continue;
                if (s6xReg.FullLabel == Tools.RegisterInstruction(s6xReg.Address)) continue;
                if (s6xReg.Comments != null && s6xReg.Comments != string.Empty) continue;

                string sText = string.Empty;

                if (s6xReg.MultipleMeanings) sText = string.Format("{0,-38}", s6xReg.FullLabels(true, true));
                else sText = string.Format("{0,-38}", s6xReg.FullLabel);

                if (col1 == string.Empty) col1 = sText;
                else col2 = sText;

                if (col1 != string.Empty && col2 != string.Empty)
                {
                    txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));
                    col1 = string.Empty;
                    col2 = string.Empty;
                }

                // MultipleMeanings second step for Word part
                if (s6xReg.MultipleMeanings)
                {
                    sText = string.Format("{0,-38}", s6xReg.FullLabels(false, true));

                    if (col1 == string.Empty) col1 = sText;
                    else col2 = sText;

                    if (col1 != string.Empty && col2 != string.Empty)
                    {
                        txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));
                        col1 = string.Empty;
                        col2 = string.Empty;
                    }
                }
            }
            if (col1 != string.Empty) txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

            txWriter.WriteLine(OutputTools.BorderedEmpty());
            txWriter.WriteLine(OutputTools.BorderedFooter());
        }

        private void processOutputTextBankHeader(ref TextWriter txWriter, string[] bankInfo)
        {
            string col1 = string.Empty;
            string col2 = string.Empty;

            txWriter.WriteLine(OutputTools.BorderedHeader());
            txWriter.WriteLine(OutputTools.BorderedEmpty());
            col1 = "Bank " + bankInfo[0];
            txWriter.WriteLine(OutputTools.BorderedTitle(col1));

            txWriter.WriteLine(OutputTools.BorderedEmpty());

            col1 = string.Format("{0,8}{1,8}", bankInfo[1], bankInfo[2]);
            col2 = string.Format("{0,8}{1,8}", bankInfo[3], bankInfo[4]);
            txWriter.WriteLine(OutputTools.BorderedColumns(new string[] { col1, col2 }));

            txWriter.WriteLine(OutputTools.BorderedEmpty());
            txWriter.WriteLine(OutputTools.BorderedFooter());
        }

        private void processOutputTextElementLabel(ref TextWriter txWriter, string label)
        {
            if (label == null || label == string.Empty) return;

            txWriter.WriteLine("\r\n" + label + ":");
        }

        private void processOutputTextElementHeaderComments(ref TextWriter txWriter, string comments)
        {
            if (comments == null || comments == string.Empty) return;

            string[] cLines = comments.Replace("\r\n", "\n").Split('\n');
            int cLineMaxLength = 80;
            foreach (string cLine in cLines) if (cLine.Length > cLineMaxLength) cLineMaxLength = cLine.Length;
            foreach (string cLine in cLines)
            {
                txWriter.WriteLine(string.Format("{0,-4}{1,-" + cLineMaxLength.ToString() + "}{2,4}", "//", cLine, "//"));
            }
            cLines = null;
        }

        private void processOutputTextElementWithComments(ref TextWriter txWriter, string firstLine, string secondLine, string comments)
        {
            if (comments == null || comments == string.Empty)
            {
                txWriter.WriteLine(firstLine);
                if (secondLine != string.Empty) txWriter.WriteLine(secondLine);
                return;
            }

            int elemLines = 1;
            if (secondLine != string.Empty) elemLines++;

            int elemLinesMaxLength = firstLine.Length;
            if (secondLine.Length > elemLinesMaxLength) elemLinesMaxLength = secondLine.Length;

            string[] cLines = comments.Replace("\r\n", "\n").Split('\n');
            int cLineMaxLength = 0;
            foreach (string cLine in cLines) if (cLine.Length > cLineMaxLength) cLineMaxLength = cLine.Length;
            for (int iLine = 0; iLine < cLines.Length; iLine++)
            {
                string elemText = string.Empty;
                if (iLine == 0) elemText = firstLine;
                else if (iLine == 1) elemText = secondLine;

                txWriter.WriteLine(string.Format("{0,-" + elemLinesMaxLength.ToString() + "}{1,10}{2,-" + cLineMaxLength.ToString() + "}{3,4}", elemText, "//  ", cLines[iLine], "//"));
            }
            if (cLines.Length < elemLines) txWriter.WriteLine(secondLine);
            cLines = null;
        }

        private void processOutputValueBitFlagsWithComments(ref TextWriter txWriter, string[][] valueBitFlags)
        {
            string[] arrShortLabels = null;
            string[] arrValues = null;
            int[] arrSizes = null;
            string shortLabels = string.Empty;
            string values = string.Empty;
            string finalFormat = string.Empty;
            int lineSize = 0;
            int leftMargin = 0;

            if (valueBitFlags == null) return;
            if (valueBitFlags.Length != 2) return;
            if (valueBitFlags[0] == null) return;
            if (valueBitFlags[1] == null) return;
            if (valueBitFlags[0].Length == 0) return;
            if (valueBitFlags[1].Length == 0) return;
            if (valueBitFlags[0].Length != valueBitFlags[1].Length) return;

            arrShortLabels = valueBitFlags[0];
            arrValues = valueBitFlags[1];
            arrSizes = new int[arrShortLabels.Length];

            for (int iBf = 0; iBf < arrSizes.Length; iBf++)
            {
                arrSizes[iBf] = arrShortLabels[iBf].Length;
                if (arrValues[iBf].Length > arrSizes[iBf]) arrSizes[iBf] = arrValues[iBf].Length;
            }

            for (int iBf = 0; iBf < arrSizes.Length; iBf++)
            {
                shortLabels += string.Format("{0," + (arrSizes[iBf] + 1).ToString() + "}", arrShortLabels[iBf]);
                values += string.Format("{0," + (arrSizes[iBf] + 1).ToString() + "}", arrValues[iBf]);
            }

            shortLabels = shortLabels.Substring(1);
            values = values.Substring(1);

            lineSize = 80;
            leftMargin = 8;
            while (shortLabels.Length > lineSize - leftMargin) lineSize += 20;
            finalFormat = "{0," + lineSize.ToString() + "}";
            txWriter.WriteLine(string.Format(finalFormat, shortLabels));
            txWriter.WriteLine(string.Format(finalFormat, values));

            arrShortLabels = null;
            arrValues = null;
            arrSizes = null;
        }

        private void processOutputTextElementOtherAddress(ref TextWriter txWriter, string uniqueAddress)
        {
            S6xOtherAddress other = null;

            if (uniqueAddress == string.Empty) return;

            if (!S6x.slProcessOtherAddresses.ContainsKey(uniqueAddress)) return;

            other = (S6xOtherAddress)S6x.slOtherAddresses[uniqueAddress];

            if (other.Label != null && other.Label != string.Empty) processOutputTextElementLabel(ref txWriter, other.Label);
            if (other.OutputComments)
            {
                if (other.Comments != null && other.Comments != string.Empty) processOutputTextElementHeaderComments(ref txWriter, other.Comments);
            }

            other = null;
        }

        private void processOutputTextIncludedElements(ref TextWriter txWriter, ref Element elem, int startAddress, int endAddress)
        {
            if (elem.IncludedElements == null) return;
            for (int iElem = 0; iElem < elem.IncludedElements.Count; iElem++)
            {
                Element incElem = (Element)elem.IncludedElements.GetByIndex(iElem);
                if (incElem == null) continue;
                if (incElem.AddressInt <= endAddress && (incElem.AddressInt >= startAddress || incElem.AddressEndInt >= startAddress))
                {
                    processOutputTextIncludedElement(ref txWriter, elem.Type, ref incElem);
                }
            }
        }

        private void processOutputTextIncludedElement(ref TextWriter txWriter, MergedType parentElemType, ref Element incElem)
        {
            if (incElem == null) return;

            string fullLabel = string.Empty;
            string sValues = string.Empty;
            string valueLine = string.Empty;
            string sFormat = string.Empty;
            string subType = string.Empty;

            switch (incElem.Type)
            {
                case MergedType.CalibrationElement:
                    if (incElem.CalElement.isScalar)
                    {
                        fullLabel = incElem.CalElement.ScalarElem.FullLabel + ":";
                        if (incElem.CalElement.ScalarElem.isScaled) sValues = incElem.CalElement.ScalarElem.ValueScaled();
                        else sValues = incElem.CalElement.ScalarElem.Value(10);
                        subType = "word";
                        if (incElem.CalElement.ScalarElem.Byte) subType = "byte";
                        sFormat = "{0,6}: {1,-8}{2,-" + (20 - subType.Length).ToString() + "}{3,8}{4,10}{5,20}";
                        valueLine = string.Format(sFormat, incElem.CalElement.UniqueAddressHex, incElem.CalElement.ScalarElem.InitialValue, incElem.CalElement.RBaseCalc + " " + incElem.CalElement.ScalarElem.ShortLabel, subType, incElem.CalElement.ScalarElem.Value(16), sValues);
                    }
                    else if (incElem.CalElement.isFunction)
                    {
                        subType = "func";
                        sFormat = "{0,6}: {1,-8}{2,-10}{3,1}";
                        fullLabel = string.Format(sFormat, incElem.CalElement.UniqueAddressHex, incElem.CalElement.RBaseCalc, subType, incElem.CalElement.FunctionElem.FullLabel);
                    }
                    else if (incElem.CalElement.isTable)
                    {
                        subType = "table";
                        sFormat = "{0,6}: {1,-8}{2,-10}{3,1}";
                        fullLabel = string.Format(sFormat, incElem.CalElement.UniqueAddressHex, incElem.CalElement.RBaseCalc, subType, incElem.CalElement.TableElem.FullLabel);
                    }
                    else if (incElem.CalElement.isStructure)
                    {
                        subType = "struct";
                        sFormat = "{0,6}: {1,-8}{2,-10}{3,1}";
                        fullLabel = string.Format(sFormat, incElem.CalElement.UniqueAddressHex, incElem.CalElement.RBaseCalc, subType, incElem.CalElement.StructureElem.FullLabel);
                    }
                    break;
                case MergedType.ExtScalar:
                    fullLabel = incElem.ExtScalar.FullLabel;
                    if (incElem.ExtScalar.isScaled) sValues = incElem.ExtScalar.ValueScaled();
                    else sValues = incElem.ExtScalar.Value(10);
                    subType = "oword";
                    if (incElem.ExtScalar.Byte) subType = "obyte";
                    sFormat = "{0,6}: {1,-8}{2,-" + (20 - subType.Length).ToString() + "}{3,8}{4,10}{5,20}";
                    valueLine = string.Format(sFormat, incElem.ExtScalar.UniqueAddressHex, incElem.ExtScalar.InitialValue, incElem.ExtScalar.ShortLabel, subType, incElem.ExtScalar.Value(16), sValues);
                    break;
                case MergedType.ExtFunction:
                    subType = "ofunc";
                    sFormat = "{0,6}: {1,-10}{2,1}";
                    fullLabel = string.Format(sFormat, incElem.ExtFunction.UniqueAddressHex, subType, incElem.ExtFunction.FullLabel);
                    break;
                case MergedType.ExtTable:
                    subType = "otable";
                    sFormat = "{0,6}: {1,-10}{2,1}";
                    fullLabel = string.Format(sFormat, incElem.ExtTable.UniqueAddressHex, subType, incElem.ExtTable.FullLabel);
                    break;
                case MergedType.ExtStructure:
                    subType = "ostruct";
                    sFormat = "{0,6}: {1,-10}{2,1}";
                    fullLabel = string.Format(sFormat, incElem.ExtStructure.UniqueAddressHex, subType, incElem.ExtStructure.FullLabel);
                    break;
            }

            if (fullLabel != string.Empty) txWriter.WriteLine(string.Format("{0,8}{1,-6}{2,1}", string.Empty, "Inc", fullLabel));
            if (valueLine != string.Empty) txWriter.WriteLine(string.Format("{0,8}{1,-6}{2,1}", string.Empty, "Inc", valueLine));
        }

        private void processOutputTextElement(ref TextWriter txWriter, ref Element elem, ref object[] previousResult)
        {
            MergedType prevType = MergedType.Unknown;
            Call cCall = null;
            Vector vVect = null;
            S6xRoutine s6xRoutine = null;
            string subType = string.Empty;
            string prevSubType = string.Empty;
            string sValues = string.Empty;
            string sValues2 = string.Empty;
            string sFormat = string.Empty;
            string firstLine = string.Empty;
            string secondLine = string.Empty;
            int maxInitialValuesLength = -1;
            int[] maxItemValuesLengths = null;

            if (elem.isIncluded) return;

            if (previousResult != null)
            {
                prevType = (MergedType)previousResult[0];
                prevSubType = (string)previousResult[1];
            }

            if (prevType != elem.Type) txWriter.WriteLine();
            switch (elem.Type)
            {
                case MergedType.UnknownOperationLine:
                    processOutputTextIncludedElements(ref txWriter, ref elem, elem.AddressInt, elem.AddressEndInt);
                    if (elem.UnknownOpPartLine.DuplicatedValues == string.Empty)
                    {
                        subType = "Unknown Operation/Structure";
                        if (subType != prevSubType) txWriter.WriteLine();
                        sFormat = "{0,6}: {1,-" + (SADDef.UnknownOpPartsAggregationSize * 3 + 22).ToString() + "}{2,1}";
                        processOutputTextElementOtherAddress(ref txWriter, elem.UnknownOpPartLine.UniqueAddress);
                        txWriter.WriteLine(string.Format(sFormat, elem.UnknownOpPartLine.UniqueAddressHex, elem.UnknownOpPartLine.Values, subType));
                    }
                    else
                    {
                        subType = "fill";
                        if (subType != prevSubType) txWriter.WriteLine();
                        sFormat = "{0,6} -> {1,-25}{2,-19}{3,-2}";
                        processOutputTextElementOtherAddress(ref txWriter, elem.UnknownOpPartLine.UniqueAddress);
                        txWriter.WriteLine(string.Format(sFormat, elem.UnknownOpPartLine.UniqueAddressHex, elem.UnknownOpPartLine.AddressEnd, subType, elem.UnknownOpPartLine.DuplicatedValues));
                    }
                    break;
                case MergedType.UnknownCalibrationLine:
                    processOutputTextIncludedElements(ref txWriter, ref elem, elem.AddressInt, elem.AddressEndInt);
                    if (elem.UnknownCalibPartLine.DuplicatedValues == string.Empty)
                    {
                        subType = "Unknown Calibration";
                        if (subType != prevSubType) txWriter.WriteLine();
                        sFormat = "{0,6}: {1,-" + (SADDef.UnknownCalibPartsAggregationSize * 3 + 14).ToString() + "}{2,-" + (20 - subType.Length).ToString() + "}{3," + (SADDef.UnknownCalibPartsAggregationSize * 4 + 6).ToString() + "}{4," + (SADDef.UnknownCalibPartsAggregationSize * 5 + 6).ToString() + "}";
                        processOutputTextElementOtherAddress(ref txWriter, elem.UnknownCalibPartLine.UniqueAddress);
                        sValues = string.Empty;
                        foreach (string sValueInt in elem.UnknownCalibPartLine.ValuesInt(16)) sValues += string.Format(SADDef.GlobalSeparator + "{0,3}", sValueInt);
                        if (sValues.Length < 2) sValues = string.Empty;
                        else sValues = sValues.Substring(2);
                        sValues2 = string.Empty;
                        foreach (string sValueInt in elem.UnknownCalibPartLine.ValuesInt(10)) sValues2 += string.Format(SADDef.GlobalSeparator + "{0,4}", sValueInt);
                        if (sValues2.Length < 2) sValues2 = string.Empty;
                        else sValues2 = sValues2.Substring(2);
                        txWriter.WriteLine(string.Format(sFormat, elem.UnknownCalibPartLine.UniqueAddressHex, elem.UnknownCalibPartLine.Values, subType, sValues, sValues2));
                    }
                    else
                    {
                        subType = "fill";
                        if (subType != prevSubType) txWriter.WriteLine();
                        sFormat = "{0,6} -> {1,-25}{2,-19}{3,-2}";
                        processOutputTextElementOtherAddress(ref txWriter, elem.UnknownCalibPartLine.UniqueAddress);
                        txWriter.WriteLine(string.Format(sFormat, elem.UnknownCalibPartLine.UniqueAddressHex, elem.UnknownCalibPartLine.AddressEnd, subType, elem.UnknownCalibPartLine.DuplicatedValues));
                    }
                    break;
                case MergedType.ReservedAddress:
                    subType = elem.ReservedAddress.Type.ToString();
                    if (prevSubType != subType) txWriter.WriteLine();
                    processOutputTextElementOtherAddress(ref txWriter, elem.ReservedAddress.UniqueAddress);
                    switch (elem.ReservedAddress.Type)
                    {
                        case ReservedAddressType.Fill:
                            sFormat = "{0,6}: {1,-46}{2,1}";
                            txWriter.WriteLine(string.Format(sFormat, elem.ReservedAddress.UniqueAddressHex, elem.ReservedAddress.InitialValue, elem.ReservedAddress.Label));
                            break;
                        case ReservedAddressType.IntVector:
                            sFormat = "{0,6}: {1,-21}{2,-6}{3,-19}{4,1}";
                            sValues = string.Empty;
                            sValues2 = string.Empty;
                            cCall = (Call)Calibration.slCalls[Tools.UniqueAddress(elem.BankNum, elem.ReservedAddress.ValueInt - SADDef.EecBankStartAddress)];
                            if (cCall == null)
                            {
                                switch (elem.ReservedAddress.BankNum)
                                {
                                    case 8:
                                        if (Bank8 != null) vVect = (Vector)Bank8.slIntVectors[elem.ReservedAddress.UniqueAddress];
                                        break;
                                    case 1:
                                        if (Bank1 != null) vVect = (Vector)Bank1.slIntVectors[elem.ReservedAddress.UniqueAddress];
                                        break;
                                    case 9:
                                        if (Bank9 != null) vVect = (Vector)Bank9.slIntVectors[elem.ReservedAddress.UniqueAddress];
                                        break;
                                    case 0:
                                        if (Bank0 != null) vVect = (Vector)Bank0.slIntVectors[elem.ReservedAddress.UniqueAddress];
                                        break;
                                }
                                if (vVect != null)
                                {
                                    sValues = vVect.ShortLabel;
                                    sValues2 = vVect.Label;
                                }
                                vVect = null;
                            }
                            else
                            {
                                sValues = cCall.ShortLabel;
                                sValues2 = cCall.Label;
                                cCall = null;
                            }
                            if (sValues != string.Empty)
                            {
                                txWriter.WriteLine(string.Format(sFormat, elem.ReservedAddress.UniqueAddressHex, elem.ReservedAddress.InitialValue, string.Format("{0,4}", elem.ReservedAddress.Value(16)), sValues, sValues2));
                            }
                            break;
                        case ReservedAddressType.RomSize:
                            sFormat = "{0,6}: {1,-21}{2,-6}{3,-19}{4,1}";
                            //sFormat = "{0,6}: {1,-21}{2,-25}{3,1}";
                            txWriter.WriteLine(string.Format(sFormat, elem.ReservedAddress.UniqueAddressHex, elem.ReservedAddress.InitialValue, string.Format("{0,4}", elem.ReservedAddress.Value(16)), elem.ReservedAddress.ShortLabel, elem.ReservedAddress.Label + " (ffff is not defined)"));
                            break;
                        case ReservedAddressType.CalPointer:
                            sFormat = "{0,6}: {1,-21}{2,-6}{3,-19}{4,1}";
                            sValues = Tools.RegisterInstruction(Calibration.getRbaseByAddress(elem.ReservedAddress.Value(16)).Code);
                            txWriter.WriteLine(string.Format(sFormat, elem.ReservedAddress.UniqueAddressHex, elem.ReservedAddress.InitialValue, string.Format("{0,4}", elem.ReservedAddress.Value(16)), elem.ReservedAddress.ShortLabel, elem.ReservedAddress.Label + " " + sValues));
                            break;
                        case ReservedAddressType.Ascii:
                        case ReservedAddressType.Hex:
                            txWriter.WriteLine(elem.ReservedAddress.FullLabel + ":");
                            sFormat = "{0,6}: {1,-47}";
                            txWriter.WriteLine(string.Format(sFormat, elem.ReservedAddress.UniqueAddressHex, elem.ReservedAddress.InitialValue));
                            txWriter.WriteLine(elem.ReservedAddress.ValueString + "\r\n");
                            break;
                        default:
                            sFormat = "{0,6}: {1,-21}{2,-6}{3,-19}{4,1}";
                            //sFormat = "{0,6}: {1,-21}{2,-25}{3,1}";
                            txWriter.WriteLine(string.Format(sFormat, elem.ReservedAddress.UniqueAddressHex, elem.ReservedAddress.InitialValue, string.Format("{0,4}", elem.ReservedAddress.Value(16)), elem.ReservedAddress.ShortLabel, elem.ReservedAddress.Label));
                            break;
                    }
                    break;
                case MergedType.Operation:
                    if (Calibration.alMainCallsUniqueAddresses.Contains(elem.Operation.UniqueAddress))
                    {
                        cCall = (Call)Calibration.slCalls[elem.Operation.UniqueAddress];
                        processOutputTextElementLabel(ref txWriter, cCall.FullLabel);
                        if (cCall.S6xRoutine != null)
                        {
                            if (cCall.S6xRoutine.OutputComments) processOutputTextElementHeaderComments(ref txWriter, cCall.Comments);
                        }
                        cCall = null;
                    }
                    else if (S6x.slProcessRoutines.ContainsKey(elem.Operation.UniqueAddress))
                    {
                        s6xRoutine = (S6xRoutine)S6x.slProcessRoutines[elem.Operation.UniqueAddress];
                        processOutputTextElementLabel(ref txWriter, (s6xRoutine.Label != string.Empty && s6xRoutine.ShortLabel != string.Empty && s6xRoutine.Label != s6xRoutine.ShortLabel) ? s6xRoutine.ShortLabel + " - " + s6xRoutine.Label : s6xRoutine.Label);
                        if (s6xRoutine.OutputComments) processOutputTextElementHeaderComments(ref txWriter, s6xRoutine.Comments);
                        s6xRoutine = null;
                    }
                    else if (prevSubType == "newLine") txWriter.WriteLine();
                    processOutputTextElementOtherAddress(ref txWriter, elem.Operation.UniqueAddress);
                    processOutputTextElementLabel(ref txWriter, elem.Operation.FullLabel);
                    if (elem.Operation.isFEConflict)
                    {
                        firstLine = string.Format("{0,-11}{1,-18}{2,-25}{3,-21}", "//", elem.Operation.Address, elem.Operation.Instruction, elem.Operation.Translation1);
                    }
                    else
                    {
                        firstLine = string.Format("{0,6}: {1,-21}{2,-25}{3,-21}", elem.Operation.UniqueAddressHex, elem.Operation.OriginalOp, elem.Operation.Instruction, elem.Operation.Translation1);
                    }
                    secondLine = string.Empty;
                    if (elem.Operation.CallArgsNum > 0)
                    {
                        secondLine = string.Format("{0,6}: {1,-46}{2,-21}", elem.Operation.UniqueCallArgsAddressHex, elem.Operation.CallArgs, "#args");
                    }
                    else if (elem.Operation.CalElemRBaseStructAdder)
                    {
                        if (elem.Operation.alCalibrationElems != null)
                        {
                            if (elem.Operation.alCalibrationElems.Count > 0)
                            {
                                CalibrationElement rBaseAddCalElem = (CalibrationElement)elem.Operation.alCalibrationElems[0];
                                if (rBaseAddCalElem != null)
                                {
                                    string rBaseAddCalElemShortLabel = string.Empty;
                                    if (rBaseAddCalElem.isScalar) rBaseAddCalElemShortLabel = rBaseAddCalElem.ScalarElem.ShortLabel;
                                    else if (rBaseAddCalElem.isFunction) rBaseAddCalElemShortLabel = rBaseAddCalElem.FunctionElem.ShortLabel;
                                    else if (rBaseAddCalElem.isTable) rBaseAddCalElemShortLabel = rBaseAddCalElem.TableElem.ShortLabel;
                                    else if (rBaseAddCalElem.isStructure) rBaseAddCalElemShortLabel = rBaseAddCalElem.StructureElem.ShortLabel;
                                    secondLine = string.Format("{0,54}{1,1} - {2,1} - {3,1}", string.Empty, rBaseAddCalElem.RBaseCalc, rBaseAddCalElem.Address, rBaseAddCalElemShortLabel);
                                    rBaseAddCalElem = null;
                                }
                            }
                        }
                    }

                    processOutputTextElementWithComments(ref txWriter, firstLine, secondLine, elem.Operation.Comments);
                    processOutputTextIncludedElements(ref txWriter, ref elem, -1, elem.AddressEndInt);

                    if (elem.Operation.isReturn) subType = "newLine";
                    else if (elem.Operation.CallType != CallType.Unknown)
                    {
                        switch (elem.Operation.CallType)
                        {
                            case CallType.Jump:
                            case CallType.ShortJump:
                                subType = "newLine";
                                break;
                        }
                    }
                    break;
                case MergedType.CalibrationElement:
                    if (elem.CalElement.isScalar)
                    {
                        subType = "word";
                        if (elem.CalElement.ScalarElem.Byte) subType = "byte";
                        if (subType != prevSubType) txWriter.WriteLine();
                        sFormat = "{0,6}: {1,-18}{2,-" + (20 - subType.Length).ToString() + "}{3,8}{4,10}{5,20}";
                        processOutputTextElementOtherAddress(ref txWriter, elem.CalElement.UniqueAddress);
                        processOutputTextElementLabel(ref txWriter, elem.CalElement.ScalarElem.FullLabel);
                        if (elem.CalElement.ScalarElem.isBitFlags) processOutputValueBitFlagsWithComments(ref txWriter, elem.CalElement.ScalarElem.ValueBitFlags);
                        if (elem.CalElement.ScalarElem.isScaled) sValues = elem.CalElement.ScalarElem.ValueScaled();
                        else sValues = elem.CalElement.ScalarElem.Value(10);
                        firstLine = string.Format(sFormat, elem.CalElement.UniqueAddressHex, elem.CalElement.ScalarElem.InitialValue, elem.CalElement.RBaseCalc + " " + elem.CalElement.ScalarElem.ShortLabel, subType, elem.CalElement.ScalarElem.Value(16), sValues);
                        processOutputTextElementWithComments(ref txWriter, firstLine, string.Empty, elem.CalElement.ScalarElem.Comments);
                        processOutputTextIncludedElements(ref txWriter, ref elem, elem.AddressInt, elem.AddressEndInt);
                    }
                    else if (elem.CalElement.isFunction)
                    {
                        subType = "func";
                        sFormat = "{0,6}: {1,-18}{2,-" + (20 - subType.Length).ToString() + "}{3,8},{4,9}{5,20},{6,9}";
                        processOutputTextElementLabel(ref txWriter, elem.CalElement.FunctionElem.FullLabel);
                        processOutputTextElementHeaderComments(ref txWriter, elem.CalElement.FunctionElem.Comments);
                        foreach (ScalarLine scalarLine in elem.CalElement.FunctionElem.Lines)
                        {
                            if (elem.CalElement.FunctionElem.isInputScaled) sValues = scalarLine.Scalars[0].ValueScaled(elem.CalElement.FunctionElem.getInputScaleExpression, elem.CalElement.FunctionElem.getInputScalePrecision);
                            else sValues = scalarLine.Scalars[0].Value(10);
                            if (elem.CalElement.FunctionElem.isOutputScaled) sValues2 = scalarLine.Scalars[1].ValueScaled(elem.CalElement.FunctionElem.getOutputScaleExpression, elem.CalElement.FunctionElem.getOutputScalePrecision);
                            else sValues2 = scalarLine.Scalars[1].Value(10);
                            processOutputTextElementOtherAddress(ref txWriter, scalarLine.UniqueAddress);
                            processOutputTextIncludedElements(ref txWriter, ref elem, scalarLine.AddressInt, scalarLine.AddressEndInt);
                            txWriter.WriteLine(string.Format(sFormat, scalarLine.UniqueAddressHex, scalarLine.InitialValue, subType, scalarLine.Scalars[0].Value(16), scalarLine.Scalars[1].Value(16), sValues, sValues2));
                        }
                    }
                    else if (elem.CalElement.isTable)
                    {
                        subType = "table";
                        if (elem.CalElement.TableElem.WordOutput)
                        {
                            if (elem.CalElement.TableElem.isCellsScaled)
                            {
                                sFormat = "{0,6}: {1,-" + (elem.CalElement.TableElem.ColsNumber * 6 + 6).ToString() + "}{2,-" + (17 - subType.Length).ToString() + "}{3,-" + (elem.CalElement.TableElem.ColsNumber * 6 + 6).ToString() + "}{4,-" + (elem.CalElement.TableElem.ColsNumber * 10 + 6).ToString() + "}";
                            }
                            else
                            {
                                sFormat = "{0,6}: {1,-" + (elem.CalElement.TableElem.ColsNumber * 6 + 6).ToString() + "}{2,-" + (17 - subType.Length).ToString() + "}{3,-" + (elem.CalElement.TableElem.ColsNumber * 6 + 6).ToString() + "}{4,-" + (elem.CalElement.TableElem.ColsNumber * 7 + 6).ToString() + "}";
                            }
                        }
                        else
                        {
                            if (elem.CalElement.TableElem.isCellsScaled)
                            {
                                sFormat = "{0,6}: {1,-" + (elem.CalElement.TableElem.ColsNumber * 3 + 6).ToString() + "}{2,-" + (17 - subType.Length).ToString() + "}{3,-" + (elem.CalElement.TableElem.ColsNumber * 4 + 6).ToString() + "}{4,-" + (elem.CalElement.TableElem.ColsNumber * 8 + 6).ToString() + "}";
                            }
                            else
                            {
                                sFormat = "{0,6}: {1,-" + (elem.CalElement.TableElem.ColsNumber * 3 + 6).ToString() + "}{2,-" + (17 - subType.Length).ToString() + "}{3,-" + (elem.CalElement.TableElem.ColsNumber * 4 + 6).ToString() + "}{4,-" + (elem.CalElement.TableElem.ColsNumber * 5 + 6).ToString() + "}";
                            }
                        }
                        processOutputTextElementLabel(ref txWriter, elem.CalElement.TableElem.FullLabel);
                        processOutputTextElementHeaderComments(ref txWriter, elem.CalElement.TableElem.Comments);
                        foreach (ScalarLine scalarLine in elem.CalElement.TableElem.Lines)
                        {
                            sValues = string.Empty;
                            sValues2 = string.Empty;
                            foreach (Scalar scalar in scalarLine.Scalars)
                            {
                                if (elem.CalElement.TableElem.WordOutput)
                                {
                                    sValues += string.Format(SADDef.GlobalSeparator + "{0,5}", scalar.Value(16));
                                    if (elem.CalElement.TableElem.isCellsScaled) sValues2 += string.Format(SADDef.GlobalSeparator + "{0,9}", scalar.ValueScaled(elem.CalElement.TableElem.getCellsScaleExpression, elem.CalElement.TableElem.getCellsScalePrecision));
                                    else sValues2 += string.Format(SADDef.GlobalSeparator + "{0,6}", scalar.Value(10));
                                }
                                else
                                {
                                    sValues += string.Format(SADDef.GlobalSeparator + "{0,3}", scalar.Value(16));
                                    if (elem.CalElement.TableElem.isCellsScaled) sValues2 += string.Format(SADDef.GlobalSeparator + "{0,8}", scalar.ValueScaled(elem.CalElement.TableElem.getCellsScaleExpression, elem.CalElement.TableElem.getCellsScalePrecision));
                                    else sValues2 += string.Format(SADDef.GlobalSeparator + "{0,4}", scalar.Value(10));
                                }
                            }
                            sValues = sValues.Substring(2);
                            sValues2 = sValues2.Substring(2);
                            processOutputTextElementOtherAddress(ref txWriter, scalarLine.UniqueAddress);
                            processOutputTextIncludedElements(ref txWriter, ref elem, scalarLine.AddressInt, scalarLine.AddressEndInt);
                            txWriter.WriteLine(string.Format(sFormat, scalarLine.UniqueAddressHex, scalarLine.InitialValue, subType, sValues, sValues2));
                        }
                    }
                    else if (elem.CalElement.isStructure)
                    {
                        if (elem.CalElement.StructureElem.ParentStructure == null)  // Included / Duplicated elements are not generated
                        {
                            subType = "struct";
                            maxInitialValuesLength = 0;
                            maxItemValuesLengths = new int[] { };
                            foreach (StructureLine sLine in elem.CalElement.StructureElem.Lines)
                            {
                                if (sLine.InitialValue.Length > maxInitialValuesLength) maxInitialValuesLength = sLine.InitialValue.Length;
                                if (sLine.Items.Count > maxItemValuesLengths.Length)
                                {
                                    int[] newLengths = new int[sLine.Items.Count];
                                    for (int iCol = 0; iCol < maxItemValuesLengths.Length; iCol++) newLengths[iCol] = maxItemValuesLengths[iCol];
                                    maxItemValuesLengths = newLengths;
                                }
                                for (int iCol = 0; iCol < sLine.Items.Count; iCol++)
                                {
                                    string sValue = ((StructureItem)sLine.Items[iCol]).Value();
                                    if (sValue.Length > maxItemValuesLengths[iCol]) maxItemValuesLengths[iCol] = sValue.Length;
                                }
                            }

                            processOutputTextElementLabel(ref txWriter, elem.CalElement.StructureElem.FullLabel);
                            processOutputTextElementHeaderComments(ref txWriter, elem.CalElement.StructureElem.Comments);
                            sFormat = "{0,6}: {1,-" + (maxInitialValuesLength + 6).ToString() + "}{2,-" + (17 - subType.Length).ToString() + "}{3,1}";
                            foreach (StructureLine sLine in elem.CalElement.StructureElem.Lines)
                            {
                                sValues = string.Empty;
                                for (int iCol = 0; iCol < sLine.Items.Count; iCol++)
                                {
                                    string sValue = ((StructureItem)sLine.Items[iCol]).Value();
                                    switch (((StructureItem)sLine.Items[iCol]).Type)
                                    {
                                        case StructureItemType.String:
                                            if (sValues.EndsWith(SADDef.GlobalSeparator)) sValues += string.Format("{0," + (maxItemValuesLengths[iCol] + 1).ToString() + "}", sValue);
                                            else sValues += string.Format("{0," + (maxItemValuesLengths[iCol]).ToString() + "}", sValue);
                                            break;
                                        case StructureItemType.Skip:
                                        case StructureItemType.Empty:
                                            break;
                                        default:
                                            if (sValues.EndsWith(SADDef.GlobalSeparator)) sValues += string.Format("{0," + (maxItemValuesLengths[iCol] + 1).ToString() + "}", sValue);
                                            else sValues += string.Format("{0," + (maxItemValuesLengths[iCol]).ToString() + "}", sValue);
                                            if (iCol < sLine.Items.Count - 1) sValues += SADDef.GlobalSeparator;
                                            break;
                                    }
                                }
                                processOutputTextElementOtherAddress(ref txWriter, sLine.UniqueAddress);
                                processOutputTextIncludedElements(ref txWriter, ref elem, sLine.AddressInt, sLine.AddressEndInt);
                                txWriter.WriteLine(string.Format(sFormat, sLine.UniqueAddressHex, sLine.InitialValue, subType, sValues));
                            }
                        }
                    }
                    break;
                case MergedType.ExtScalar:
                    subType = "oword";
                    if (elem.ExtScalar.Byte) subType = "obyte";
                    if (subType != prevSubType) txWriter.WriteLine();
                    processOutputTextElementOtherAddress(ref txWriter, elem.ExtScalar.UniqueAddress);
                    processOutputTextElementLabel(ref txWriter, elem.ExtScalar.FullLabel);
                    if (elem.ExtScalar.isBitFlags) processOutputValueBitFlagsWithComments(ref txWriter, elem.ExtScalar.ValueBitFlags);
                    if (elem.ExtScalar.isScaled) sValues = elem.ExtScalar.ValueScaled();
                    else sValues = elem.ExtScalar.Value(10);
                    sFormat = "{0,6}: {1,-18}{2,-" + (20 - subType.Length).ToString() + "}{3,8}{4,10}{5,20}";
                    firstLine = string.Format(sFormat, elem.ExtScalar.UniqueAddressHex, elem.ExtScalar.InitialValue, elem.ExtScalar.ShortLabel, subType, elem.ExtScalar.Value(16), sValues);
                    processOutputTextElementWithComments(ref txWriter, firstLine, string.Empty, elem.ExtScalar.Comments);
                    processOutputTextIncludedElements(ref txWriter, ref elem, elem.AddressInt, elem.AddressEndInt);
                    break;
                case MergedType.ExtFunction:
                    subType = "ofunc";
                    sFormat = "{0,6}: {1,-18}{2,-" + (20 - subType.Length).ToString() + "}{3,8},{4,9}{5,20},{6,9}";
                    processOutputTextElementLabel(ref txWriter, elem.ExtFunction.FullLabel);
                    processOutputTextElementHeaderComments(ref txWriter, elem.ExtFunction.Comments);
                    foreach (ScalarLine scalarLine in elem.ExtFunction.Lines)
                    {
                        if (elem.ExtFunction.isInputScaled) sValues = scalarLine.Scalars[0].ValueScaled(elem.ExtFunction.getInputScaleExpression, elem.ExtFunction.getInputScalePrecision);
                        else sValues = scalarLine.Scalars[0].Value(10);
                        if (elem.ExtFunction.isOutputScaled) sValues2 = scalarLine.Scalars[1].ValueScaled(elem.ExtFunction.getOutputScaleExpression, elem.ExtFunction.getOutputScalePrecision);
                        else sValues2 = scalarLine.Scalars[1].Value(10);
                        processOutputTextElementOtherAddress(ref txWriter, scalarLine.UniqueAddress);
                        processOutputTextIncludedElements(ref txWriter, ref elem, scalarLine.AddressInt, scalarLine.AddressEndInt);
                        txWriter.WriteLine(string.Format(sFormat, scalarLine.UniqueAddressHex, scalarLine.InitialValue, subType, scalarLine.Scalars[0].Value(16), scalarLine.Scalars[1].Value(16), sValues, sValues2));
                    }
                    break;
                case MergedType.ExtTable:
                    subType = "otable";
                    if (elem.ExtTable.WordOutput)
                    {
                        if (elem.ExtTable.isCellsScaled)
                        {
                            sFormat = "{0,6}: {1,-" + (elem.ExtTable.ColsNumber * 6 + 6).ToString() + "}{2,-" + (17 - subType.Length).ToString() + "}{3,-" + (elem.ExtTable.ColsNumber * 6 + 6).ToString() + "}{4,-" + (elem.ExtTable.ColsNumber * 10 + 6).ToString() + "}";
                        }
                        else
                        {
                            sFormat = "{0,6}: {1,-" + (elem.ExtTable.ColsNumber * 6 + 6).ToString() + "}{2,-" + (17 - subType.Length).ToString() + "}{3,-" + (elem.ExtTable.ColsNumber * 6 + 6).ToString() + "}{4,-" + (elem.ExtTable.ColsNumber * 7 + 6).ToString() + "}";
                        }
                    }
                    else
                    {
                        if (elem.ExtTable.isCellsScaled)
                        {
                            sFormat = "{0,6}: {1,-" + (elem.ExtTable.ColsNumber * 3 + 6).ToString() + "}{2,-" + (17 - subType.Length).ToString() + "}{3,-" + (elem.ExtTable.ColsNumber * 4 + 6).ToString() + "}{4,-" + (elem.ExtTable.ColsNumber * 8 + 6).ToString() + "}";
                        }
                        else
                        {
                            sFormat = "{0,6}: {1,-" + (elem.ExtTable.ColsNumber * 3 + 6).ToString() + "}{2,-" + (17 - subType.Length).ToString() + "}{3,-" + (elem.ExtTable.ColsNumber * 4 + 6).ToString() + "}{4,-" + (elem.ExtTable.ColsNumber * 5 + 6).ToString() + "}";
                        }
                    }
                    processOutputTextElementLabel(ref txWriter, elem.ExtTable.FullLabel);
                    processOutputTextElementHeaderComments(ref txWriter, elem.ExtTable.Comments);
                    foreach (ScalarLine scalarLine in elem.ExtTable.Lines)
                    {
                        sValues = string.Empty;
                        sValues2 = string.Empty;
                        foreach (Scalar scalar in scalarLine.Scalars)
                        {
                            if (elem.ExtTable.WordOutput)
                            {
                                sValues += string.Format(SADDef.GlobalSeparator + "{0,5}", scalar.Value(16));
                                if (elem.ExtTable.isCellsScaled) sValues2 += string.Format(SADDef.GlobalSeparator + "{0,9}", scalar.ValueScaled(elem.ExtTable.getCellsScaleExpression, elem.ExtTable.getCellsScalePrecision));
                                else sValues2 += string.Format(SADDef.GlobalSeparator + "{0,6}", scalar.Value(10));
                            }
                            else
                            {
                                sValues += string.Format(SADDef.GlobalSeparator + "{0,3}", scalar.Value(16));
                                if (elem.ExtTable.isCellsScaled) sValues2 += string.Format(SADDef.GlobalSeparator + "{0,8}", scalar.ValueScaled(elem.ExtTable.getCellsScaleExpression, elem.ExtTable.getCellsScalePrecision));
                                else sValues2 += string.Format(SADDef.GlobalSeparator + "{0,4}", scalar.Value(10));
                            }
                        }
                        sValues = sValues.Substring(2);
                        sValues2 = sValues2.Substring(2);
                        processOutputTextElementOtherAddress(ref txWriter, scalarLine.UniqueAddress);
                        processOutputTextIncludedElements(ref txWriter, ref elem, scalarLine.AddressInt, scalarLine.AddressEndInt);
                        txWriter.WriteLine(string.Format(sFormat, scalarLine.UniqueAddressHex, scalarLine.InitialValue, subType, sValues, sValues2));
                    }
                    break;
                case MergedType.ExtStructure:
                    if (elem.ExtStructure.ParentStructure == null)  // Included / Duplicated elements are not generated
                    {
                        subType = "ostruct";
                        maxInitialValuesLength = 0;
                        maxItemValuesLengths = new int[] { };
                        foreach (StructureLine sLine in elem.ExtStructure.Lines)
                        {
                            if (sLine.InitialValue.Length > maxInitialValuesLength) maxInitialValuesLength = sLine.InitialValue.Length;
                            if (sLine.Items.Count > maxItemValuesLengths.Length)
                            {
                                int[] newLengths = new int[sLine.Items.Count];
                                for (int iCol = 0; iCol < maxItemValuesLengths.Length; iCol++) newLengths[iCol] = maxItemValuesLengths[iCol];
                                maxItemValuesLengths = newLengths;
                            }
                            for (int iCol = 0; iCol < sLine.Items.Count; iCol++)
                            {
                                string sValue = ((StructureItem)sLine.Items[iCol]).Value();
                                if (sValue.Length > maxItemValuesLengths[iCol]) maxItemValuesLengths[iCol] = sValue.Length;
                            }
                        }

                        processOutputTextElementLabel(ref txWriter, elem.ExtStructure.FullLabel);
                        processOutputTextElementHeaderComments(ref txWriter, elem.ExtStructure.Comments);
                        sFormat = "{0,6}: {1,-" + (maxInitialValuesLength + 6).ToString() + "}{2,-" + (17 - subType.Length).ToString() + "}{3,1}";
                        foreach (StructureLine sLine in elem.ExtStructure.Lines)
                        {
                            sValues = string.Empty;
                            for (int iCol = 0; iCol < sLine.Items.Count; iCol++)
                            {
                                string sValue = ((StructureItem)sLine.Items[iCol]).Value();
                                switch (((StructureItem)sLine.Items[iCol]).Type)
                                {
                                    case StructureItemType.String:
                                        if (sValues.EndsWith(SADDef.GlobalSeparator)) sValues += string.Format("{0," + (maxItemValuesLengths[iCol] + 1).ToString() + "}", sValue);
                                        else sValues += string.Format("{0," + (maxItemValuesLengths[iCol]).ToString() + "}", sValue);
                                        break;
                                    case StructureItemType.Skip:
                                    case StructureItemType.Empty:
                                        break;
                                    default:
                                        if (sValues.EndsWith(SADDef.GlobalSeparator)) sValues += string.Format("{0," + (maxItemValuesLengths[iCol] + 1).ToString() + "}", sValue);
                                        else sValues += string.Format("{0," + (maxItemValuesLengths[iCol]).ToString() + "}", sValue);
                                        if (iCol < sLine.Items.Count - 1) sValues += SADDef.GlobalSeparator;
                                        break;
                                }
                            }
                            processOutputTextElementOtherAddress(ref txWriter, sLine.UniqueAddress);
                            processOutputTextIncludedElements(ref txWriter, ref elem, sLine.AddressInt, sLine.AddressEndInt);
                            txWriter.WriteLine(string.Format(sFormat, sLine.UniqueAddressHex, sLine.InitialValue, subType, sValues));
                        }
                    }
                    break;
                case MergedType.Vector:
                    subType = "Vector";
                    if (subType != prevSubType) txWriter.WriteLine();
                    if (elem.Vector.VectList == null)
                    {
                        processOutputTextElementOtherAddress(ref txWriter, elem.Vector.UniqueSourceAddress);
                    }
                    else if (elem.Vector.VectList.UniqueAddress == elem.Vector.UniqueSourceAddress)
                    {
                        processOutputTextElementLabel(ref txWriter, elem.Vector.VectList.FullLabel);
                        processOutputTextElementHeaderComments(ref txWriter, elem.Vector.VectList.Comments);
                    }
                    else
                    {
                        processOutputTextElementOtherAddress(ref txWriter, elem.Vector.UniqueSourceAddress);
                    }
                    sFormat = "{0,6}: {1,-21}{2,-6}{3,-19}{4,1}";
                    processOutputTextIncludedElements(ref txWriter, ref elem, elem.Vector.SourceAddressInt, elem.Vector.SourceAddressInt + 1);
                    txWriter.WriteLine(string.Format(sFormat, elem.Vector.UniqueSourceAddressHex, elem.Vector.InitialValue, elem.Vector.Address, "Bank " + elem.Vector.ApplyOnBankNum + " " + subType, elem.Vector.FullLabel));
                    break;

            }

            previousResult = new object[] { elem.Type, subType };
        }
    }
}
