using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace SAD806x
{
    public class SADBank
    {
        public int Num = -1;

        public int AddressBinInt = -1;
        public int AddressBinEndInt = -1;

        public int CheckSumStartAddress = -1;
        public int CheckSumEndAddress = -1;

        private SADCalib Calibration = null;

        public SortedList slReserved = null;
        public SortedList slOPs = null;
        public SortedList slOPsOrigins = null;
        public SortedList slIntVectors = null;
        public SortedList slUnknownOpParts = null;

        // Accelerators
        private ArrayList alPushVectorOPsUniqueAddresses = null;
        private ArrayList alAltStackVectorOPsUniqueAddresses = null;
        private ArrayList alVectorListsOPsUniqueAddresses = null;
        private ArrayList alCalibElemOPsUniqueAddresses = null;
        private ArrayList alPossibleOtherElemOPsUniqueAddresses = null;
        private ArrayList alPossibleKnownElemOPsUniqueAddresses = null;
        private ArrayList alPossibleCalElemRBaseStructUniqueAddresses = null;
        private ArrayList alElemGotoOPsUniqueAddresses = null;
        private ArrayList alCallOPsUniqueAddresses = null;
        private SortedList slRoutineOPsCalibElemUniqueAddresses = null;

        private bool is8061 = false;
        private bool isEarly = false;
        private bool isPilot = false;
        private string[] arrBytes = null;
        private string sBytes = string.Empty;

        private int minResAdr = -1;
        private int maxResAdr = -1;

        public int Size { get { return AddressBinEndInt - AddressBinInt + 1; } }
        public int AddressInternalInt { get { return 0; } }
        public int AddressInternalEndInt { get { return Size - 1; } }

        public string AddressBin { get { return string.Format("{0:x5}", AddressBinInt); } }
        public string AddressBinEnd { get { return string.Format("{0:x5}", AddressBinEndInt); } }
        public string AddressInternal { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInternalInt); } }
        public string AddressInternalEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInternalEndInt); } }

        public SADBank(int num, int startAddress, int endAddress, bool bIs8061, bool bIsEarly, bool bIsPilot, ref SADCalib calibration, ref string[] arrBinBytes)
        {
            Num = num;
            AddressBinInt = startAddress;

            if (endAddress < arrBinBytes.Length) AddressBinEndInt = endAddress;
            else AddressBinEndInt = arrBinBytes.Length - 1;
            if (AddressBinEndInt - AddressBinInt + 1 > SADDef.Bank_Max_Size) AddressBinEndInt = AddressBinInt + SADDef.Bank_Max_Size - 1;

            is8061 = bIs8061;
            isEarly = bIsEarly;
            isPilot = bIsPilot;

            Calibration = calibration;

            arrBytes = new string[Size];
            for (int iPos = AddressInternalInt; iPos < Size; iPos++) arrBytes[iPos] = arrBinBytes[AddressBinInt + iPos];

            sBytes = string.Join("", arrBytes);

            slReserved = new SortedList();
            slIntVectors = new SortedList();

            slOPs = new SortedList();
            slOPsOrigins = new SortedList();
            slUnknownOpParts = new SortedList();

            alPushVectorOPsUniqueAddresses = new ArrayList();
            alAltStackVectorOPsUniqueAddresses = new ArrayList();
            alVectorListsOPsUniqueAddresses = new ArrayList();
            alCalibElemOPsUniqueAddresses = new ArrayList();
            alPossibleOtherElemOPsUniqueAddresses = new ArrayList();
            alPossibleKnownElemOPsUniqueAddresses = new ArrayList();
            alPossibleCalElemRBaseStructUniqueAddresses = new ArrayList();
            alElemGotoOPsUniqueAddresses = new ArrayList();
            alCallOPsUniqueAddresses = new ArrayList();
            slRoutineOPsCalibElemUniqueAddresses = new SortedList();

            readReservedAddresses();
            readIntVectors();
        }

        public void readReservedAddresses()
        {
            object[] resAdrDef = null;
            int iNumber = -1;
            int iAddress = -1;
            int iSize = -1;
            ReservedAddressType rType = ReservedAddressType.Fill;
            string shortLabel = string.Empty;
            string label = string.Empty;
            string comments = string.Empty;

            ReservedAddress rAddress = null;
            string sValue = string.Empty;

            int iLevelsNum = -1;
            int iCalibsNum = -1;

            switch (Num)
            {
                case 8:
                    if (is8061 && isEarly) resAdrDef = SADDef.ReservedAddressesEarly8061Bank8;
                    else if (is8061) resAdrDef = SADDef.ReservedAddresses8061Bank8;
                    else resAdrDef = SADDef.ReservedAddresses8065Bank8;
                    break;
                case 1:
                    if (isEarly || isPilot) resAdrDef = new object[] { };
                    else resAdrDef = SADDef.ReservedAddresses8065Bank1;
                    break;
                case 9:
                    resAdrDef = SADDef.ReservedAddresses8065Bank9;
                    break;
                case 0:
                    resAdrDef = SADDef.ReservedAddresses8065Bank0;
                    break;
            }

            // Number, Relative Address, Size, Type, Translation, Comments
            foreach (object[] resAdr in resAdrDef)
            {
                iNumber = Convert.ToInt32(resAdr[0]);
                iAddress = Convert.ToInt32(resAdr[1]);
                iSize = Convert.ToInt32(resAdr[2]);
                shortLabel = resAdr[3].ToString().ToUpper();
                switch (shortLabel)
                {
                    case "ROMSIZE":
                        rType = ReservedAddressType.RomSize;
                        break;
                    case "CHECKSUM":
                        rType = ReservedAddressType.CheckSum;
                        break;
                    case "SMPBASEADR":
                        rType = ReservedAddressType.SmpBase;
                        break;
                    case "CCEXETIME":
                        rType = ReservedAddressType.CcExeTime;
                        break;
                    case "INTVECTORADR":
                        rType = ReservedAddressType.IntVector;
                        break;
                    case "LEVNUM":
                        rType = ReservedAddressType.LevelNum;
                        break;
                    case "CALNUM":
                        rType = ReservedAddressType.CalNum;
                        break;
                    case "RBASEADR":
                        rType = ReservedAddressType.CalPointer;
                        break;
                    case "WORD":
                        rType = ReservedAddressType.Word;
                        break;
                    case "BYTE":
                        rType = ReservedAddressType.Byte;
                        break;
                    default:
                        rType = ReservedAddressType.Fill;
                        break;
                }
                label = resAdr[4].ToString();
                comments = resAdr[5].ToString();

                if (rType == ReservedAddressType.CalPointer)
                {
                    if (iLevelsNum == -1) iLevelsNum = 8;
                    if (iCalibsNum == -1) iCalibsNum = 1;
                    iNumber = iLevelsNum * iCalibsNum;

                    //PATCH - For non standard Binaries
                    if (iNumber > 8) iNumber = 8;
                }

                for (int iNum = 0; iNum < iNumber; iNum++)
                {
                    rAddress = new ReservedAddress(Num, iAddress + iNum * iSize, iSize, rType);
                    rAddress.AddressBinInt = rAddress.AddressInt + AddressBinInt;
                    rAddress.ShortLabel = shortLabel;
                    if (iNumber > 1) rAddress.ShortLabel += "_" + string.Format("{0:d2}", iNum + 1);
                    rAddress.Label = label;
                    rAddress.Comments = comments;
                    sValue = getBytes(rAddress.AddressInt, rAddress.Size);
                    if (iSize <= 2) rAddress.ValueInt = Convert.ToInt32(Tools.LsbFirst(sValue), 16);
                    else rAddress.ValueInt = -1;
                    rAddress.InitialValue = string.Empty;
                    for (int iByte = 0; iByte < rAddress.Size; iByte++) rAddress.InitialValue += sValue.Substring(iByte * 2, 2) + SADDef.GlobalSeparator;
                    rAddress.InitialValue = rAddress.InitialValue.Substring(0, rAddress.InitialValue.Length - SADDef.GlobalSeparator.ToString().Length);
                    slReserved.Add(rAddress.UniqueAddress, rAddress);

                    if (rType == ReservedAddressType.LevelNum) iLevelsNum = rAddress.ValueInt;
                    if (rType == ReservedAddressType.CalNum) iCalibsNum = rAddress.ValueInt;
                }
            }

            if (slReserved.Count > 0)
            {
                minResAdr = ((ReservedAddress)slReserved.GetByIndex(0)).AddressInt;
                maxResAdr = ((ReservedAddress)slReserved.GetByIndex(slReserved.Count - 1)).AddressEndInt;
            }
            else
            {
                minResAdr = AddressInternalInt;
                maxResAdr = AddressInternalEndInt;
            }
        }
        
        public void readIntVectors()
        {
            object[] intVectorsDef = null;
            int iVect = 0;

            if (is8061) intVectorsDef = SADDef.IntVectors_8061;
            else intVectorsDef = SADDef.IntVectors_8065;

            foreach (ReservedAddress rAddress in slReserved.Values)
            {
                if (rAddress.Type == ReservedAddressType.IntVector)
                {
                    Vector intVector = new Vector();
                    intVector.SourceBankNum = Num;
                    intVector.ApplyOnBankNum = Num;
                    intVector.Number = iVect + 1;
                    intVector.SourceAddressInt = rAddress.AddressInt;
                    intVector.AddressInt = rAddress.ValueInt - SADDef.EecBankStartAddress;
                    intVector.InitialValue = rAddress.InitialValue;
                    intVector.isValid = true;   // Valid by Default, normally Addresses are not pointer to Reserved or RBases Addresses
                    foreach (object[] intVectorDef in intVectorsDef)
                    {
                        if (intVector.Number == Convert.ToInt32(intVectorDef[0]))
                        {
                            intVector.ShortLabel = intVectorDef[1].ToString();
                            intVector.Label = intVectorDef[2].ToString();
                            intVector.Comments = intVectorDef[3].ToString();
                            break;
                        }
                    }
                    if (!slIntVectors.ContainsKey(intVector.UniqueSourceAddress)) slIntVectors.Add(intVector.UniqueSourceAddress, intVector);
                    iVect++;
                }
            }
            intVectorsDef = null;
        }

        public void processBankInit()
        {
            slOPs = new SortedList();
            slOPsOrigins = new SortedList();
            slUnknownOpParts = new SortedList();

            alPushVectorOPsUniqueAddresses = new ArrayList();
            alAltStackVectorOPsUniqueAddresses = new ArrayList();
            alVectorListsOPsUniqueAddresses = new ArrayList();
            alCalibElemOPsUniqueAddresses = new ArrayList();
            alPossibleOtherElemOPsUniqueAddresses = new ArrayList();
            alPossibleKnownElemOPsUniqueAddresses = new ArrayList();
            alPossibleCalElemRBaseStructUniqueAddresses = new ArrayList();
            alElemGotoOPsUniqueAddresses = new ArrayList();
            alCallOPsUniqueAddresses = new ArrayList();
            slRoutineOPsCalibElemUniqueAddresses = new SortedList();

            GC.Collect();
        }
        
        // Bank Process
        public void processBank(ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            // No Operation in Bank 1 for Early 8065
            if (isEarly && Num == 1) return;

            GotoOpParams gopParams = null;

            processOperations(AddressInternalInt, AddressInternalEndInt, CallType.Jump, false, false, Num, AddressInternalInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
            processIntVectors(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);

            // S6x Additional Routines
            //foreach (S6xRoutine s6xRoutine in S6x.slProcessRoutines.Values)
            for (int iRoutine = 0; iRoutine < S6x.slProcessRoutines.Count; iRoutine++)
            {
                S6xRoutine s6xRoutine = (S6xRoutine)S6x.slProcessRoutines.GetByIndex(iRoutine);
                if (s6xRoutine == null) continue;

                if (s6xRoutine.BankNum == Num)
                {
                    if (!slOPs.ContainsKey(s6xRoutine.UniqueAddress))
                    {
                        gopParams = null;
                        processOperations(s6xRoutine.AddressInt, AddressInternalEndInt, CallType.Jump, false, false, Num, s6xRoutine.AddressInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                    }
                }
            }

            // S6x Additional Operations
            //foreach (S6xOperation s6xOpe in S6x.slProcessOperations.Values)
            for (int iOpe = 0; iOpe < S6x.slProcessOperations.Count; iOpe++)
            {
                S6xOperation s6xOpe = (S6xOperation)S6x.slProcessOperations.GetByIndex(iOpe);
                if (s6xOpe == null) continue;

                if (s6xOpe.BankNum == Num)
                {
                    if (!slOPs.ContainsKey(s6xOpe.UniqueAddress))
                    {
                        gopParams = null;
                        processOperations(s6xOpe.AddressInt, AddressInternalEndInt, CallType.Jump, false, false, Num, s6xOpe.AddressInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                    }
                    if (slOPs.ContainsKey(s6xOpe.UniqueAddress)) ((Operation)slOPs[s6xOpe.UniqueAddress]).S6xOperation = s6xOpe;
                }
            }

            // Remaining Matching Signatures Not Macthed with Calls
            //      Backup is done, because items are removed when being processed.
            ArrayList alUnMatchedSignaturesUniqueAddresses = new ArrayList();
            foreach (MatchingSignature mSig in Calibration.slUnMatchedSignatures.Values) alUnMatchedSignaturesUniqueAddresses.Add(mSig.UniqueMatchingStartAddress);
            foreach (string sigUniqueAddress in alUnMatchedSignaturesUniqueAddresses)
            {
                MatchingSignature mSig = (MatchingSignature)Calibration.slUnMatchedSignatures[sigUniqueAddress];
                if (mSig == null) continue;

                if (mSig.BankNum != Num) continue;

                // New Operations can be processed - !slOPs.ContainsKey(mSig.UniqueMatchingStartAddress)
                // Operations already processed, Call not existing, Call can be created - !Calibration.slCalls.ContainsKey(mSig.UniqueMatchingStartAddress)
                // Call existing, but to be promoted for signature identification and routine creation - Calibration.slCalls.ContainsKey(mSig.UniqueMatchingStartAddress)
                // All cases in fact

                gopParams = null;
                processOperations(mSig.MatchingStartAddressInt, AddressInternalEndInt, CallType.Jump, false, false, Num, mSig.MatchingStartAddressInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
            }
            alUnMatchedSignaturesUniqueAddresses = null;

            // Additional Vectors are managed after S6x Objects to include their generated possible Additional Vectors
            processAddVectors(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);

            gopParams = null;
        }

        // Int Vectors Process
        private void processIntVectors(ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            foreach (Vector intVector in slIntVectors.Values)
            {
                GotoOpParams gopParams = null;

                // No Jump Conflict Check
                processOperations(intVector.AddressInt, AddressInternalEndInt, CallType.Jump, true, false, Num, AddressInternalInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                
                // Interrupt Vectors Labels Mngt, if not already done 
                //  will be automatically overriden by S6xRoutine if exists
                if (Calibration.slCalls.ContainsKey(intVector.UniqueAddress))
                {
                    ((Call)Calibration.slCalls[intVector.UniqueAddress]).ShortLabel = intVector.ShortLabel;
                    ((Call)Calibration.slCalls[intVector.UniqueAddress]).Label = intVector.Label;
                }
            }
        }

        // Additional Vectors Process
        private void processAddVectors(ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            Operation ope = null;
            Operation[] prevOps = null;
            string sPushReg = string.Empty;
            int iAddress = -1;
            int iNumber = -1;
            int iNumberAdder = 0;

            // Additional Vectors Pushed
            for (int iPos = 0; iPos < alPushVectorOPsUniqueAddresses.Count; iPos++)
            {
                iAddress = -1;
                iNumber = -1;
                iNumberAdder = 0;
                ope = (Operation)slOPs[alPushVectorOPsUniqueAddresses[iPos].ToString()];

                // CB PART TO BE REVIEWED !!!!
                if (ope.OriginalOPCode.ToLower() == "cb" && ope.BytesNumber == 4)
                // cb,57,9a,47       push  [R56+479a]     push([R56+479a]);                 => First Vector Address in List is found at 479a, but no number
                // or
                // 99,24,c6          cmpb  Rc6,24                                           => 24 is counter
                // db,0b             jc    20ac           if ((uns) Rc6 <= 24) goto 20ac;   => 24 / 2 is Number of vectors, Adder is 1
                // db,0b             jc    20ac           if ((uns) Rc6 < 24) goto 20ac;    => 24 / 2 is Number of vectors, Adder is 0
                // ac,c6,30          ldzbw R30,Rc6        R30 = (uns)Rc6;                   => Optional
                // cb,31,34,8b       push  [R30+8b34]     push([R30+8b34]);                 => First Vector Address in List is found at 8b34
                //      or
                // ad,04,30          ldzbw R30,4          R30 = (uns)4;                     => Last Vector Address Adder => 4 : Number = 3
                // cb,31,34,8b       push  [R30+8b34]     push([R30+8b34]);                 => First Vector Address in List is found at 8b34
                {
                    iAddress = Convert.ToInt32(ope.OriginalOpArr[3] + ope.OriginalOpArr[2], 16) - SADDef.EecBankStartAddress;
                    sPushReg = Convert.ToString(Convert.ToInt32(ope.OriginalOpArr[1], 16) - 1, 16);

                    ope.VectorListAddressInt = iAddress;
                    ope.VectorListBankNum = ope.ReadDataBankNum;
                    alVectorListsOPsUniqueAddresses.Add(ope.UniqueAddress);

                    //prevOps = getPrecedingOPs(ope.AddressInt, 8, 99, true, true, false, false, false, false, false, false);
                    prevOps = getPrecedingOPs(ope.AddressInt, 8, 0);
                    foreach (Operation prevOpe in prevOps)
                    {
                        // To Prevent Issues on Bad Source Ope
                        if (prevOpe == null) break;

                        if (prevOpe.OriginalOPCode.ToLower() == "d9" || prevOpe.OriginalOPCode.ToLower() == "d2")
                        {
                            iNumberAdder = 1;
                        }
                        else if (prevOpe.OriginalOPCode.ToLower() == "db" || prevOpe.OriginalOPCode.ToLower() == "d3")
                        {
                            iNumberAdder = 0;
                        }
                        else if (prevOpe.OriginalOPCode.ToLower() == "ac" && prevOpe.OriginalOpArr[prevOpe.OriginalOpArr.Length - 1] == sPushReg)
                        {
                            sPushReg = prevOpe.OriginalOpArr[1];
                        }
                        else if (prevOpe.OriginalOPCode.ToLower() == "ad" && prevOpe.OriginalOpArr[prevOpe.OriginalOpArr.Length - 1] == sPushReg)
                        {
                            iNumber = Convert.ToInt32(prevOpe.OriginalOpArr[1], 16) / 2 + 1;
                        }
                        else if (prevOpe.OriginalOPCode == "89" && prevOpe.OriginalOpArr[prevOpe.OriginalOpArr.Length - 1] == sPushReg)
                        {
                            iNumber = Convert.ToInt32(prevOpe.OriginalOpArr[2] + prevOpe.OriginalOpArr[1], 16) / 2 + iNumberAdder;
                        }
                        else if (prevOpe.OriginalOPCode == "99" && prevOpe.OriginalOpArr[prevOpe.OriginalOpArr.Length - 1] == sPushReg)
                        {
                            iNumber = Convert.ToInt32(prevOpe.OriginalOpArr[1], 16) / 2 + iNumberAdder;
                        }
                        if (iAddress >= 0 && iNumber >= 0) break;
                    }
                    prevOps = null;
                }
                else if (ope.OriginalOPCode.ToLower() == "ca")
                //99,cc,cf          cmpb  Rcf,cc                                            => cc is counter
                //db,2a             jc    2515           if ((uns) Rcf >= cc) goto 2515;    => cc / 2 is Number of vectors
                //b1,ff,d0          ldb   Rd0,ff         Rd0 = ff;                          => Optional
                //ac,cf,34          ldzbw R34,Rcf        R34 = (uns)Rcf;                    => Optional when Number is greater than 7f
                //65,b8,84,34       ad2w  R34,84b8       R34 += 84b8;                       => First Vector Address in List is found at 84b8
                //c9,e6,24          push  24e6           push(24e6);                        => Optional
                //ca,34             push  [R34]          push([R34]);                       => First Identification
                {
                    sPushReg = ope.OriginalOpArr[1];
                    //prevOps = getPrecedingOPs(ope.AddressInt, 8, 99, true, true, false, false, false, false, false, false);
                    prevOps = getPrecedingOPs(ope.AddressInt, 8, 0);
                    foreach (Operation prevOpe in prevOps)
                    {
                        // To Prevent Issues on Bad Source Ope
                        if (prevOpe == null) break;

                        if ((prevOpe.OriginalOPCode.ToLower() == "d9" || prevOpe.OriginalOPCode.ToLower() == "d2") && iAddress >= 0)
                        {
                            iNumberAdder = 1;
                        }
                        else if ((prevOpe.OriginalOPCode.ToLower() == "db" || prevOpe.OriginalOPCode.ToLower() == "d3") && iAddress >= 0)
                        {
                            iNumberAdder = 0;
                        }
                        else if (prevOpe.OriginalOPCode.ToLower() == "ac" && prevOpe.OriginalOpArr[ope.OriginalOpArr.Length - 1] == sPushReg)
                        {
                            sPushReg = prevOpe.OriginalOpArr[1];
                        }
                        else if (prevOpe.OriginalOPCode == "65" && prevOpe.OriginalOpArr[3] == sPushReg)
                        {
                            iAddress = Convert.ToInt32(prevOpe.OriginalOpArr[2] + prevOpe.OriginalOpArr[1], 16) - SADDef.EecBankStartAddress;
                            prevOpe.VectorListAddressInt = iAddress;
                            prevOpe.VectorListBankNum = ope.ReadDataBankNum;
                            alVectorListsOPsUniqueAddresses.Add(prevOpe.UniqueAddress);
                        }
                        else if (prevOpe.OriginalOPCode == "89" && iAddress >= 0 && prevOpe.OriginalOpArr[prevOpe.OriginalOpArr.Length - 1] == sPushReg)
                        {
                            iNumber = Convert.ToInt32(prevOpe.OriginalOpArr[2] + prevOpe.OriginalOpArr[1], 16) / 2 + iNumberAdder;
                        }
                        else if (prevOpe.OriginalOPCode == "99" && iAddress >= 0 && prevOpe.OriginalOpArr[prevOpe.OriginalOpArr.Length - 1] == sPushReg)
                        {
                            iNumber = Convert.ToInt32(prevOpe.OriginalOpArr[1], 16) / 2 + iNumberAdder;
                        }
                        if (iAddress >= 0 && iNumber >= 0) break;
                    }
                    prevOps = null;
                }

                if (iAddress >= 0)
                {
                    addAdditionalVectorAndProcess(iAddress, iNumber, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                }

                ope = null;
            }

            // Additional Vectors set with ALTSTACK
            for (int iPos = 0; iPos < alAltStackVectorOPsUniqueAddresses.Count; iPos++)
            {
                iAddress = -1;
                iNumber = -1;
                ope = (Operation)slOPs[alAltStackVectorOPsUniqueAddresses[iPos].ToString()];
                // Ope is always a1,..,..,22
                iAddress = Convert.ToInt32(ope.OperationParams[0].InstructedParam, 16) - SADDef.EecBankStartAddress;
                ope.VectorListAddressInt = iAddress;
                ope.VectorListBankNum = ope.ReadDataBankNum;
                alVectorListsOPsUniqueAddresses.Add(ope.UniqueAddress);
                addAdditionalVectorAndProcess(iAddress, iNumber, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                ope = null;
            }

            // Additional Vectors provided from S6x Vectors Lists and Structures including Vectors
            foreach (S6xStructure s6xStruct in S6x.slProcessStructures.Values)
            {
                Structure sStruct = null;
                if (s6xStruct.Structure == null) sStruct = new Structure(s6xStruct);
                else sStruct = s6xStruct.Structure;
                sStruct.Number = s6xStruct.Number;
                if (sStruct.Number <= 0) continue;
                if (sStruct.isVectorsList)
                {
                    if (sStruct.VectorsBankNum != Num) continue;
                    if (Calibration.slAdditionalVectorsLists.ContainsKey(sStruct.UniqueAddress)) continue;
                    if (Calibration.slAdditionalVectors.ContainsKey(sStruct.UniqueAddress)) continue;

                    iAddress = sStruct.AddressInt;
                    iNumber = sStruct.Number;
                    addAdditionalVectorAndProcess(iAddress, iNumber, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                }
                else if (sStruct.containsOtherVectorsAddresses)
                {
                    string[] arrBytes = null;
                    switch (sStruct.BankNum)
                    {
                        case 8:
                            arrBytes = Bank8.getBytesArray(sStruct.AddressInt, sStruct.MaxSizeSingle * sStruct.Number);
                            break;
                        case 1:
                            arrBytes = Bank1.getBytesArray(sStruct.AddressInt, sStruct.MaxSizeSingle * sStruct.Number);
                            break;
                        case 9:
                            arrBytes = Bank9.getBytesArray(sStruct.AddressInt, sStruct.MaxSizeSingle * sStruct.Number);
                            break;
                        case 0:
                            arrBytes = Bank0.getBytesArray(sStruct.AddressInt, sStruct.MaxSizeSingle * sStruct.Number);
                            break;
                    }
                    if (arrBytes == null) continue;
                    sStruct.Read(ref arrBytes, sStruct.Number);
                    arrBytes = null;

                    processOtherVectors(sStruct.GetOtherVectorAddresses(Num), ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                }
            }
        }

        // Process Other Vectors
        //      Returns true if new Operations have been added
        public bool processOtherVectors(int[] iOVAddresses, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            if (iOVAddresses == null) return false;
            if (iOVAddresses.Length == 0) return false;

            int initialOpsCount = slOPs.Count;
            
            foreach (int iOVAddress in iOVAddresses)
            {
                bool bContinue = false;

                // Checks on Address
                if (iOVAddress < AddressInternalInt || iOVAddress > AddressInternalEndInt) continue;

                // Already Processed
                if (slOPs.ContainsKey(Tools.UniqueAddress(Num, iOVAddress))) continue;

                //  8061 Calibration Console or Engineering Console Vectors
                if (is8061)
                {
                    if (iOVAddress + SADDef.EecBankStartAddress >= SADDef.CCMemory8061MinAdress && iOVAddress + SADDef.EecBankStartAddress <= SADDef.CCMemory8061MaxAdress) continue;
                    if (iOVAddress + SADDef.EecBankStartAddress >= SADDef.ECMemory8061MinAdress && iOVAddress + SADDef.EecBankStartAddress <= SADDef.ECMemory8061MaxAdress) continue;
                }

                //  Pointer to Source Address in same range => Not Valid
                if (Calibration.slAdditionalVectors.ContainsKey(Tools.UniqueAddress(Num, iOVAddress))) continue;

                //  Pointer to Reserved Addresses => Not Valid
                foreach (ReservedAddress resAdr in slReserved.Values)
                {
                    if (iOVAddress >= resAdr.AddressInt && iOVAddress <= resAdr.AddressEndInt)
                    {
                        bContinue = true;
                        break;
                    }
                }
                if (bContinue) continue;

                //  Pointer to RBases Addresses => Not Valid, except for early 8061
                if (Num == Calibration.BankNum && (!is8061 || !isEarly))
                {
                    foreach (RBase rBase in Calibration.slRbases.Values)
                    {
                        if (iOVAddress >= rBase.AddressBankInt && iOVAddress <= rBase.AddressBankEndInt)
                        {
                            bContinue = true;
                            break;
                        }
                    }
                }
                if (bContinue) continue;

                // Managed as Operation, should not be added as Additional Vector
                // Conflict Check with Calibration part, except for Early 8061
                if (!isJumpAddressInConflict(iOVAddress, -1) || (is8061 && isEarly))
                {
                    GotoOpParams gopParams = null;
                    processOperations(iOVAddress, AddressInternalEndInt, CallType.Call, false, false, Num, AddressInternalInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                }
            }

            return initialOpsCount != slOPs.Count;
        }

        // Process Signatures
        public void processSignatures(ref SADS6x S6x)
        {
            foreach (S6xSignature sig in S6x.slProcessSignatures.Values)
            {
                if (sig.Signature == null || sig.Signature == string.Empty) continue;

                Signature806xOptions for806x = Signature806xOptions.Undefined;
                if (sig.for806x != null && sig.for806x != string.Empty)
                {
                    try { for806x = (Signature806xOptions)Enum.Parse(typeof(Signature806xOptions), sig.for806x, true); }
                    catch {}
                }
                if (for806x != Signature806xOptions.Undefined)
                {
                    if (is8061 && for806x != Signature806xOptions.for8061Only) continue;
                    else if (!is8061 && for806x != Signature806xOptions.for8065Only) continue;
                }

                SignatureBankOptions forBankNum = SignatureBankOptions.Undefined;
                if (sig.forBankNum != null && sig.forBankNum != string.Empty)
                {
                    try { forBankNum = (SignatureBankOptions)Enum.Parse(typeof(SignatureBankOptions), sig.forBankNum, true); }
                    catch {}
                }
                if (forBankNum != SignatureBankOptions.Undefined)
                {
                    if (Num == 8 && forBankNum != SignatureBankOptions.forBank8Only) continue;
                    else if (Num == 1 && forBankNum != SignatureBankOptions.forBank1Only) continue;
                    else if (Num == 9 && forBankNum != SignatureBankOptions.forBank9Only) continue;
                    else if (Num == 0 && forBankNum != SignatureBankOptions.forBank0Only) continue;
                }

                string cleanedSignature = string.Empty;
                ArrayList alParams = new ArrayList();
                
                cleanedSignature = sig.Signature;

                //Signature Parameters management
                while (cleanedSignature.Contains(SADDef.SignatureParamBytePrefixSuffix))
                {
                    // Parameter is always #XX, a group of 1 byte (..) is created in regular expression to get it later on
                    int startPos = -1;
                    int length = -1;
                    startPos = cleanedSignature.IndexOf(SADDef.SignatureParamBytePrefixSuffix);
                    if (startPos <= cleanedSignature.Length - 2)
                    {
                        if (cleanedSignature.Substring(startPos + 1).Contains(SADDef.SignatureParamBytePrefixSuffix))
                        {
                            length = cleanedSignature.Substring(startPos + 1).IndexOf(SADDef.SignatureParamBytePrefixSuffix) + 2;
                        }
                    }
                    if (startPos >= 0 && length > 0)
                    {
                        string sParam = cleanedSignature.Substring(startPos, length);
                        cleanedSignature = cleanedSignature.Replace(sParam, "(..)");
                        alParams.Add(sParam);
                    }
                    else if (startPos > 0)
                    {
                        cleanedSignature = cleanedSignature.Replace(SADDef.SignatureParamBytePrefixSuffix, "Z");
                    }
                }

                if (sig.Information == null) sig.Information = string.Empty;

                try
                {
                    object[] oMatches = getBytesFromSignature(cleanedSignature);
                    foreach (object[] oMatch in oMatches)
                    {
                        int matchingStartAddress = (int)oMatch[0];
                        string matchingBytes = oMatch[1].ToString();
                        string[] matchingParams = (string[])oMatch[2];

                        sig.Found = true;
                        
                        if (sig.Information != string.Empty) sig.Information += "\r\n";
                        sig.Information += "Signature matches on Bank " + Num + " at " + Convert.ToString(matchingStartAddress + SADDef.EecBankStartAddress, 16);

                        // Multiple Signature Matches cause Signature to be ignored
                        if (oMatches.Length != 1) sig.Ignore = true;
                        if (!sig.Ignore)
                        {
                            foreach (MatchingSignature msMS in Calibration.slMatchingSignatures.Values)
                            {
                                if (msMS.S6xSignature.UniqueKey == sig.UniqueKey) sig.Ignore = true;
                            }
                        }

                        if (!Calibration.slMatchingSignatures.ContainsKey(Tools.UniqueAddress(Num, matchingStartAddress)))
                        {
                            MatchingSignature msMS = new MatchingSignature(Num, matchingStartAddress, matchingBytes, sig);

                            // Signature Parameters
                            if (matchingParams.Length > 0 && matchingParams.Length == alParams.Count)
                            {
                                for (int iParam = 0; iParam < matchingParams.Length; iParam++) msMS.slMatchingParameters.Add(alParams[iParam], matchingParams[iParam]);
                            }

                            Calibration.slMatchingSignatures.Add(msMS.UniqueMatchingStartAddress, msMS);
                            Calibration.slUnMatchedSignatures.Add(msMS.UniqueMatchingStartAddress, msMS);
                            msMS = null;
                        }
                    }
                    oMatches = null;
                }
                catch
                {
                    if (sig.Information != string.Empty) sig.Information += "\r\n";
                    sig.Information += "Invalid Signature";
                }

                alParams = null;
            }
        }
        
        // Process Operations
        private void processOperations(int startAddress, int endAddress, CallType callType, bool intVectorCall, bool addVectorCall, int callerBankNum, int callerAddressInt, ref GotoOpParams gopParams, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            Call cCall = null;
            string sByte = string.Empty;
            string[] arrOPBytes = null;
            SADOPCode sadOPCode = null;
            Operation ope = null;
            Operation prevOpe = null;
            bool applySignedAlt = false;
            bool rBasePart = false;

            // Invalid Start Address
            if (startAddress < AddressInternalInt || startAddress > endAddress) return;

            // Part already Processed, Exit
            if (Calibration.slCalls.ContainsKey(Tools.UniqueAddress(Num, startAddress)))
            {
                cCall = (Call)Calibration.slCalls[Tools.UniqueAddress(Num, startAddress)];

                // Int Vector Call has to be identified
                if (!cCall.isIntVector && intVectorCall)
                {
                    cCall.isIntVector = true;
                    identifyCall(ref cCall, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                }

                // Int Vector Call has to be identified
                if (!cCall.isVector && addVectorCall)
                {
                    cCall.isVector = true;
                    identifyCall(ref cCall, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                }
                
                // Adding Caller can Promote Call Type, if it returns true, Call has to be identified
                if (cCall.AddCaller(callerBankNum, callerAddressInt, callType)) identifyCall(ref cCall, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);

                // Goto Op Params to be found from already identified Call or SCall End (normally a return)
                if (cCall.CallType == CallType.Call || cCall.CallType == CallType.ShortCall)
                {
                    GotoOpParams callBackGopParams = null;
                    switch (cCall.BankNum)
                    {
                        case 8:
                            if (Bank8 != null) if (Bank8.slOPs.ContainsKey(Tools.UniqueAddress(Num, cCall.AddressEndInt))) callBackGopParams = ((Operation)Bank8.slOPs[Tools.UniqueAddress(cCall.BankNum, cCall.AddressEndInt)]).GotoOpParams;
                            break;
                        case 1:
                            if (Bank1 != null) if (Bank1.slOPs.ContainsKey(Tools.UniqueAddress(Num, cCall.AddressEndInt))) callBackGopParams = ((Operation)Bank1.slOPs[Tools.UniqueAddress(cCall.BankNum, cCall.AddressEndInt)]).GotoOpParams;
                            break;
                        case 9:
                            if (Bank9 != null) if (Bank9.slOPs.ContainsKey(Tools.UniqueAddress(Num, cCall.AddressEndInt))) callBackGopParams = ((Operation)Bank9.slOPs[Tools.UniqueAddress(cCall.BankNum, cCall.AddressEndInt)]).GotoOpParams;
                            break;
                        case 0:
                            if (Bank0 != null) if (Bank0.slOPs.ContainsKey(Tools.UniqueAddress(Num, cCall.AddressEndInt))) callBackGopParams = ((Operation)Bank0.slOPs[Tools.UniqueAddress(cCall.BankNum, cCall.AddressEndInt)]).GotoOpParams;
                            break;
                    }
                    if (callBackGopParams != null) gopParams = callBackGopParams.Clone();
                }
                return;
            }

            cCall = new Call(Num, startAddress, callType, callerBankNum, callerAddressInt);
            cCall.isIntVector = intVectorCall;
            cCall.isVector = addVectorCall;

            // Fake Call Detection
            cCall.isFake = false;

            // Calibration Console Fake Call Detection
            //  Engineering Console Fake Call is rejected previously because of its addresses outside bank (0xe000 to 0xffff)
            //  8061 only
            if (!cCall.isFake && is8061)
            {
                if (cCall.AddressInt + SADDef.EecBankStartAddress >= SADDef.CCMemory8061MinAdress && cCall.AddressInt + SADDef.EecBankStartAddress <= SADDef.CCMemory8061MaxAdress)
                // 0xc000 / 0xdfff - Could be a CC Call or a Patch at this place
                //  CC or EC Calls always start with c,d,e or f followed by 00 and end with 6 or 9
                {
                    if (cCall.Address.EndsWith("006") || cCall.Address.EndsWith("009")) cCall.isFake = true;
                }
            }

            // Duplicates
            if (!cCall.isFake)
            {
                cCall.isFake = true;
                sByte = string.Empty;
                for (int iAddress = startAddress; iAddress < arrBytes.Length && iAddress <= endAddress && iAddress < startAddress + SADDef.FakeCallDuplicatesDetectionSize; iAddress++)
                {
                    if (sByte != string.Empty && sByte != arrBytes[iAddress])
                    {
                        cCall.isFake = false;
                        break;
                    }
                    sByte = arrBytes[iAddress];
                }
            }

            if (cCall.isFake) return;

            Calibration.slCalls.Add(cCall.UniqueAddress, cCall);

            for (int iAddress = startAddress; iAddress < arrBytes.Length && iAddress <= endAddress; iAddress++)
            {
                // Reserved Addresses part access try
                if (iAddress >= minResAdr && iAddress <= maxResAdr) break;

                // Rbase part access try
                //  Does not apply for early 8061
                if (!is8061 || !isEarly)
                {
                    if (Num == Calibration.BankNum)
                    {
                        rBasePart = false;
                        foreach (RBase rBase in Calibration.slRbases.Values)
                        {
                            if (iAddress >= rBase.AddressBankInt && iAddress <= rBase.AddressBankEndInt)
                            {
                                rBasePart = true;
                                break;
                            }
                        }
                        if (rBasePart) break;
                    }
                }

                // Op already processed, Part Processed, Exit, when it is a valid Op (not the first Invalid Op of a fake call)
                if ((!applySignedAlt && slOPs.ContainsKey(Tools.UniqueAddress(Num, iAddress))) || (applySignedAlt && slOPs.ContainsKey(Tools.UniqueAddress(Num, iAddress - 1))))
                {
                    if (!applySignedAlt) ope = (Operation)slOPs[Tools.UniqueAddress(Num, iAddress)];
                    else  ope = (Operation)slOPs[Tools.UniqueAddress(Num, iAddress - 1)];

                    // Ope should be a valid one to exit, otherwise it was generated by a fake call which has directly returned
                    //      Security againts loops on itself is managed, when starting ProcessOperations, exit is done when Call has already been processed
                    if (Calibration.slOPCodes.Contains(ope.OriginalOPCode))
                    {
                        // Call End Address Update
                        if (Calibration.slCalls.ContainsKey(Tools.UniqueAddress(ope.BankNum, ope.InitialCallAddressInt)))
                        {
                            cCall.AddressEndInt = ((Call)Calibration.slCalls[Tools.UniqueAddress(ope.BankNum, ope.InitialCallAddressInt)]).AddressEndInt;
                        }
                        else
                        {
                            cCall.AddressEndInt = ope.AddressInt;
                        }

                        if (startAddress != AddressInternalInt) identifyCall(ref cCall, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);

                        // Skip Case
                        if (prevOpe != null)
                        {
                            if (prevOpe.CallType == CallType.Skip) prevOpe.AddressJumpInt = ope.AddressNextInt;
                        }

                        ope = null;

                        return;
                    }
                }

                sByte = arrBytes[iAddress];
                if (!Calibration.slOPCodes.Contains(sByte))
                // Unrecognized OP Code
                {
                    // Reset of Apply Signed Alt Flag (fe)
                    applySignedAlt = false;
                    
                    // Invalid Op after Call Op is considered as Return on Previous Call Operation / Invalid Op is Ignored
                    //      Call is flagged at Fake if invalid op is the same than first call one
                    if (iAddress != cCall.AddressInt && prevOpe != null)
                    {
                        if (prevOpe.CallType != CallType.Unknown && prevOpe.AddressNextInt == iAddress)
                        {
                            ope = prevOpe;
                            if (ope.OriginalOp == sByte) cCall.isFake = true;
                            break;
                        }
                    }

                    ope = new Operation(Num, iAddress);
                    ope.OriginalInstruction = string.Empty;
                    ope.InitialCallAddressInt = startAddress;
                    ope.AddressJumpInt = -1;
                    ope.OriginalOpArr = new string[] {sByte};

                    ope.BytesNumber = 1;
                    // Inside Call (Except for Bank Start Address) - OP will include duplicated Unrecognized OP Codes
                    if (ope.AddressInt != cCall.AddressInt || cCall.AddressInt == AddressInternalInt)
                    {
                        for (int iDupAddress = ope.AddressInt + 1; iDupAddress < arrBytes.Length; iDupAddress++)
                        {
                            if (sByte != arrBytes[iDupAddress]) break;
                            else ope.BytesNumber++;
                        }
                    }
                    iAddress = ope.AddressNextInt - 1;

                    ope.forcedInstruction = ope.OriginalOp;

                    if (ope.BytesNumber > 1) ope.forcedTranslation1 = string.Format("{0:x4} => {1:x4}", SADDef.EecBankStartAddress + ope.AddressInt, SADDef.EecBankStartAddress + ope.AddressNextInt - 1);

                    if (!slOPs.ContainsKey(ope.UniqueAddress)) slOPs.Add(ope.UniqueAddress, ope);

                    // Invalid Call (Except for Bank Start Address) - Call Ends
                    //      Call is flagged at Fake if invalid op is the same than first call one
                    if (ope.AddressInt == cCall.AddressInt && cCall.AddressInt != AddressInternalInt)
                    {
                        if (ope.OriginalOp == sByte) cCall.isFake = true;
                        break;
                    }
                }
                else
                // Recognized OP Code
                {
                    sadOPCode = (SADOPCode)Calibration.slOPCodes[sByte];
                    if (sadOPCode.isSigndAlt)
                    // Signd/Alt Specificity - 1 Op Code to change meaning of next Op
                    {
                        applySignedAlt = true;
                    }
                    else
                    // Standard OP Code mngt
                    {
                        if (sadOPCode.hasVariableParams && iAddress + sadOPCode.paramsNumberBis > AddressInternalEndInt) break;
                        else if (iAddress + sadOPCode.paramsNumber > AddressInternalEndInt) break;
                        
                        if (sadOPCode.hasVariableParams) arrOPBytes = getBytesArray(iAddress, sadOPCode.paramsNumberBis + 1);
                        else arrOPBytes = getBytesArray(iAddress, sadOPCode.paramsNumber + 1);
                        ope = sadOPCode.processOP(iAddress, Num, startAddress, arrOPBytes, ref Calibration, applySignedAlt, ref prevOpe, ref gopParams, ref S6x);

                        applySignedAlt = false;
                        slOPs.Add(ope.UniqueAddress, ope);

                        if (ope.CallType == CallType.Call || ope.CallType == CallType.ShortCall || ope.CallType == CallType.Jump || ope.CallType == CallType.ShortJump || ope.CallType == CallType.Goto)
                        // Call, Jump or Goto Part to be processed
                        {
                            if (ope.ApplyOnBankNum == Num)
                            // Apply on current Bank
                            {
                                // Conflict Check with Calibration part, except for Early 8061
                                if (!isJumpAddressInConflict(ope.AddressJumpInt, ope.AddressInt) || (is8061 && isEarly))
                                {
                                    processOperations(ope.AddressJumpInt, endAddress, ope.CallType, false, false, ope.BankNum, ope.AddressInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                                }
                            }
                            else
                            // Apply on different Bank 
                            {
                                switch (ope.ApplyOnBankNum)
                                {
                                    case 0:
                                        if (Bank0 != null)
                                        {
                                            // Conflict Check with Calibration part
                                            if (!Bank0.isJumpAddressInConflict(ope.AddressJumpInt, -1))
                                            {
                                                Bank0.processOperations(ope.AddressJumpInt, Bank0.AddressInternalEndInt, ope.CallType, false, false, ope.BankNum, ope.AddressInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                                            }
                                        }
                                        break;
                                    case 1:
                                        if (Bank1 != null)
                                        {
                                            // Conflict Check with Calibration part
                                            if (!Bank1.isJumpAddressInConflict(ope.AddressJumpInt, -1))
                                            {
                                                Bank1.processOperations(ope.AddressJumpInt, Bank1.AddressInternalEndInt, ope.CallType, false, false, ope.BankNum, ope.AddressInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                                            }
                                        }
                                        break;
                                    case 8:
                                        if (Bank8 != null)
                                        {
                                            // Conflict Check with Calibration part
                                            if (!Bank8.isJumpAddressInConflict(ope.AddressJumpInt, -1))
                                            {
                                                Bank8.processOperations(ope.AddressJumpInt, Bank8.AddressInternalEndInt, ope.CallType, false, false, ope.BankNum, ope.AddressInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                                            }
                                        }
                                        break;
                                    case 9:
                                        if (Bank9 != null)
                                        {
                                            // Conflict Check with Calibration part
                                            if (!Bank9.isJumpAddressInConflict(ope.AddressJumpInt, -1))
                                            {
                                                Bank9.processOperations(ope.AddressJumpInt, Bank9.AddressInternalEndInt, ope.CallType, false, false, ope.BankNum, ope.AddressInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                                            }
                                        }
                                        break;
                                }
                            }

                            // Call Args Mngt
                            if (ope.CallType == CallType.Call || ope.CallType == CallType.ShortCall)
                            {
                                identifyOpeArgs(ref ope, ref S6x);

                                if (ope.CallArgsNum > 0) sadOPCode.postProcessOpCallArgs(ref ope, ref Calibration, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                            }
                        }

                        // Skip Case
                        if (prevOpe != null)
                        {
                            if (prevOpe.CallType == CallType.Skip) prevOpe.AddressJumpInt = ope.AddressNextInt;
                        }

                        // OPs Origins Management
                        //      was previously executed at the end of SADOPCodes.processOP
                        //      without managing properly Args or Skip
                        bool bNewOrigin = false;
                        SortedList slOrigins = null;
                        
                        bNewOrigin = !slOPsOrigins.ContainsKey(ope.AddressNextInt);
                        if (bNewOrigin) slOrigins = new SortedList();
                        else slOrigins = (SortedList)slOPsOrigins[ope.AddressNextInt];

                        if (!slOrigins.ContainsKey(ope.AddressInt)) slOrigins.Add(ope.AddressInt, ope);
                        if (bNewOrigin) slOPsOrigins.Add(ope.AddressNextInt, slOrigins);
                        slOrigins = null;

                        if (ope.CallType != CallType.Unknown)
                        {
                            bNewOrigin = !slOPsOrigins.ContainsKey(ope.AddressJumpInt);
                            if (bNewOrigin) slOrigins = new SortedList();
                            else slOrigins = (SortedList)slOPsOrigins[ope.AddressJumpInt];

                            if (!slOrigins.ContainsKey(ope.AddressInt)) slOrigins.Add(ope.AddressInt, ope);
                            if (bNewOrigin) slOPsOrigins.Add(ope.AddressJumpInt, slOrigins);
                            slOrigins = null;
                        }

                        // Skip Prev Ope Info to Cancel Break on Return or Jump
                        bool prevSkip = false;
                        if (prevOpe != null)
                        {
                            prevSkip = (prevOpe.CallType == CallType.Skip);
                        }

                        prevOpe = ope;
                        
                        // Accelerators
                        //      Push for Vectors
                        switch (ope.OriginalOPCode.ToLower())
                        {
                            case "ca":
                                if (!alPushVectorOPsUniqueAddresses.Contains(ope.UniqueAddress)) alPushVectorOPsUniqueAddresses.Add(ope.UniqueAddress);
                                break;
                            case "cb":
                                if (ope.BytesNumber == 4)
                                {
                                    if (!alPushVectorOPsUniqueAddresses.Contains(ope.UniqueAddress)) alPushVectorOPsUniqueAddresses.Add(ope.UniqueAddress);
                                }
                                break;
                        }
                        //     AltStack for Vectors 
                        if (!is8061 && ope.OriginalOpArr[0].ToLower() == "a1" && ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] == "22")
                        {
                            if (!alAltStackVectorOPsUniqueAddresses.Contains(ope.UniqueAddress)) alAltStackVectorOPsUniqueAddresses.Add(ope.UniqueAddress);
                        }
                        //      Calib Element Ope
                        if (ope.alCalibrationElems != null)
                        {
                            if (!alCalibElemOPsUniqueAddresses.Contains(ope.UniqueAddress)) alCalibElemOPsUniqueAddresses.Add(ope.UniqueAddress);
                        }
                        //      RBase Calib Element Struct Ope
                        if (ope.CalElemRBaseStructRBase != string.Empty)
                        {
                            if (!alPossibleCalElemRBaseStructUniqueAddresses.Contains(ope.UniqueAddress)) alPossibleCalElemRBaseStructUniqueAddresses.Add(ope.UniqueAddress);
                        }
                        //      Non Calib Element Ope
                        if (ope.OtherElemAddress != string.Empty)
                        {
                            if (!alPossibleOtherElemOPsUniqueAddresses.Contains(ope.UniqueAddress)) alPossibleOtherElemOPsUniqueAddresses.Add(ope.UniqueAddress);
                        }
                        //      Known Element Ope
                        if (ope.KnownElemAddress != string.Empty)
                        {
                            if (!alPossibleKnownElemOPsUniqueAddresses.Contains(ope.UniqueAddress)) alPossibleKnownElemOPsUniqueAddresses.Add(ope.UniqueAddress);
                        }
                        //      Element Goto Ope
                        if (ope.GotoOpParams != null)
                        {
                            // GotoOpParams[Params Ope UniqueAddress, Params Ope Elem UniqueAddress, P1, P2, CY Ope UniqueAddress, CY Mode, CY P1, CY P2]
                            if (ope.GotoOpParams.ElemUniqueAddress != string.Empty)
                            {
                                if (!alElemGotoOPsUniqueAddresses.Contains(ope.UniqueAddress)) alElemGotoOPsUniqueAddresses.Add(ope.UniqueAddress);
                            }
                        }
                        //      Call OPs
                        if (ope.CallType != CallType.Unknown)
                        {
                            if (!alCallOPsUniqueAddresses.Contains(ope.UniqueAddress)) alCallOPsUniqueAddresses.Add(ope.UniqueAddress);
                        }

                        // End of Current Call
                        // Return Op, Part Processed, End of Call
                        // Jump Op, Part Processed, End of Call, if conditional jump other parts are already managed in sub calls
                        if (ope.isReturn || ope.CallType == CallType.Jump || ope.CallType == CallType.ShortJump)
                        {
                            // Skip cancels the End of Current Call
                            if (!prevSkip) break;
                        }

                        // Continues
                        iAddress = ope.AddressNextInt - 1;
                    }
                }
            }

            if (ope != null) cCall.AddressEndInt = ope.AddressInt;

            if (startAddress != AddressInternalInt) identifyCall(ref cCall, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
        }

        // Args Operation
        private void identifyOpeArgs(ref Operation ope, ref SADS6x S6x)
        {
            Operation[] adjacentOps = null;
            Call subCall = null;
            S6xRoutine s6xRoutine = null;
            int iCallArg = -1;
            int iOpeCallArg = -1;
            int iMatchingArgs = -1;

            //  No Call with Args is identified
            if (Calibration.alArgsCallsUniqueAddresses.Count == 0) return;
            
            //  Call and Short Call only
            if (ope.CallType != CallType.Call && ope.CallType != CallType.ShortCall) return;

            // Push are identified as calls, but have no args
            if (ope.OriginalInstruction.ToLower() == "push") return;
            
            // Get Related Call
            subCall = (Call)Calibration.slCalls[Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)];
            if (subCall == null) return;

            // Sub Call Analysis
            switch (subCall.ArgsType)
            {
                case CallArgsType.None:
                case CallArgsType.Unknown:
                    // Not Interesting
                    break;
                case CallArgsType.Variable:
                    // Variable Number Args, always one
                    if (subCall.ArgsStackDepthMax == 1)
                    // Right place to read this number, higher than 1 it will be read in a higher Call, 0 it has already been used
                    {
                        ope.CallArgsNum = 1;
                        // Arguments Copy
                        if (subCall.Arguments != null)
                        {
                            ope.CallArguments = new CallArgument[subCall.Arguments.Length];
                            for (int iArg = 0; iArg < subCall.Arguments.Length; iArg++)
                            {
                                ope.CallArguments[iArg] = subCall.Arguments[iArg].Clone();
                                ope.CallArguments[iArg].Position = iArg;
                            }
                        }
                    }
                    break;
                case CallArgsType.Fixed:
                    // Fixed Number
                    // 20200514 - PYM - This is now the Max Depth, some Args can be at Depth 1 to be used, others no
                    if (subCall.ArgsStackDepthMax >= 1)
                    // Right place to read the args, higher than 1 it will be read in a higher Call, 0 it has already been used
                    {
                        iMatchingArgs = 0;
                        // Arguments Copy
                        if (subCall.Arguments != null)
                        {
                            foreach (CallArgument cArg in subCall.Arguments) if (cArg.StackDepth == 1) iMatchingArgs++;
                            if (iMatchingArgs > 0)
                            {
                                ope.CallArguments = new CallArgument[iMatchingArgs];
                                ope.CallArgsNum = 0;
                                iOpeCallArg = 0;
                                foreach (CallArgument cArg in subCall.Arguments)
                                {
                                    if (cArg.StackDepth != 1) continue;
                                    ope.CallArguments[iOpeCallArg] = cArg.Clone();
                                    ope.CallArguments[iOpeCallArg].Position = iOpeCallArg;
                                    ope.CallArgsNum++;
                                    if (cArg.Word) ope.CallArgsNum++;
                                    iOpeCallArg++;
                                }
                            }
                        }
                    }
                    break;
                case CallArgsType.FixedCyCond:
                    // Fixed Number with Cy Cond
                    // 20200514 - PYM - This is now the Max Depth, some Args can be at Depth 1 to be used, others no
                    if (subCall.ArgsStackDepthMax >= 1)
                    // Right place to read the args, higher than 1 it will be read in a higher Call, 0 it has already been used
                    {
                        iMatchingArgs = 0;
                        // Arguments Copy
                        if (subCall.Arguments != null)
                        {
                            foreach (CallArgument cArg in subCall.Arguments) if (cArg.StackDepth == 1) iMatchingArgs++;
                            if (iMatchingArgs > 0)
                            {
                                ope.CallArguments = new CallArgument[iMatchingArgs];
                                ope.CallArgsNum = 0;
                                iOpeCallArg = 0;
                                foreach (CallArgument cArg in subCall.Arguments)
                                {
                                    if (cArg.StackDepth != 1) continue;
                                    ope.CallArguments[iOpeCallArg] = cArg.Clone();
                                    ope.CallArguments[iOpeCallArg].Position = iOpeCallArg;
                                    ope.CallArgsNum++;
                                    if (cArg.Word) ope.CallArgsNum++;
                                    iOpeCallArg++;
                                }
                                if (subCall.ArgsCondValidated) ope.CallArgsNum += subCall.ArgsNumCondAdder;
                            }
                        }
                    }
                    break;
                case CallArgsType.VariableExternalRegister:
                    // Variable Number Args, coming from External previous Register
                    if (subCall.ArgsStackDepthMax == 1)
                    // Right place to read this number, higher than 1 it will be read in a higher Call, 0 it has already been used
                    {
                        //adjacentOps = getPrecedingOPs(ope.AddressInt, 4, 99, false, false, false, false, false, false, false, false);
                        adjacentOps = getPrecedingOPs(ope.AddressInt, 4, 0);
                        // Reg Copy analysis, searching an easy result
                        foreach (Operation prevOpe in adjacentOps)
                        {
                            if (prevOpe == null) break;
                            switch (prevOpe.OriginalOPCode.ToLower())
                            {
                                case "b1":  // ldb RXX, Loops Number
                                    if (prevOpe.OriginalOpArr.Length == 3)
                                    {
                                        if (prevOpe.OriginalOpArr[ope.OriginalOpArr.Length - 1] == subCall.ArgsVariableExternalRegister)
                                        {
                                            ope.CallArgsNum = subCall.ByteArgsNum * Convert.ToInt32(prevOpe.OriginalOpArr[1], 16);
                                            // Arguments Copy
                                            ope.CallArguments = new CallArgument[ope.CallArgsNum];
                                            for (int iArg = 0; iArg < ope.CallArguments.Length; iArg++)
                                            {
                                                ope.CallArguments[iArg] = new CallArgument();
                                                ope.CallArguments[iArg].Position = iArg;
                                            }
                                            if (subCall.Arguments != null)
                                            {
                                                if (subCall.Arguments.Length > 0)
                                                {
                                                    iCallArg = 0;
                                                    for (int iArg = 0; iArg < ope.CallArguments.Length; iArg++)
                                                    {
                                                        ope.CallArguments[iArg] = subCall.Arguments[iCallArg].Clone();
                                                        ope.CallArguments[iArg].Position = iArg;
                                                        iCallArg++;
                                                        if (iCallArg >= subCall.Arguments.Length) iCallArg = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
                            if (ope.CallArgsNum > 0) break;
                        }
                        adjacentOps = null;
                    }
                    break;
            }

            // S6x Overrides search
            s6xRoutine = (S6xRoutine)S6x.slProcessRoutines[subCall.UniqueAddress];
            if (s6xRoutine != null)
            {
                if (s6xRoutine.ByteArgumentsNumOverride)
                {
                    if (s6xRoutine.ByteArgumentsNum != ope.CallArgsNum)
                    {
                        ope.CallArgsNum = s6xRoutine.ByteArgumentsNum;
                        if (s6xRoutine.InputArguments != null)
                        {
                            ope.CallArguments = new CallArgument[s6xRoutine.InputArguments.Length];
                            for (int iArg = 0; iArg < s6xRoutine.InputArguments.Length; iArg++)
                            {
                                ope.CallArguments[iArg] = new CallArgument();
                                ope.CallArguments[iArg].StackDepth = 1;
                                ope.CallArguments[iArg].Position = iArg;
                                ope.CallArguments[iArg].Word = s6xRoutine.InputArguments[iArg].Word;
                                ope.CallArguments[iArg].Mode = (CallArgsMode)s6xRoutine.InputArguments[iArg].Encryption;

                                ope.CallArguments[iArg].OutputRegisterAddressInt = s6xRoutine.VariableRegisterAddressByEquivalent(s6xRoutine.InputArguments[iArg].Code);
                            }
                        }
                    }
                }
            }
            s6xRoutine = null;

            subCall = null;

            if (ope.CallArgsNum > 0)
            {
                if (ope.AddressNextInt - 1 <= AddressInternalEndInt)
                {
                    ope.CallArgsArr = getBytesArray(ope.AddressNextInt - ope.CallArgsNum, ope.CallArgsNum);
                }
                else
                {
                    ope.CallArgsArr = new string[ope.CallArgsNum];
                    for (int iArg = 0; iArg < ope.CallArgsArr.Length; iArg++) ope.CallArgsArr[iArg] = "00";
                }
            }
        }

        // Used by processAddVectors only
        private void addAdditionalVectorAndProcess(int iAddress, int iNumber, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            ArrayList alRangeVectorsSourceAddresses = null;
            Structure vectList = null;

            int iCount = -1;

            bool bNextVector = false;
            string[] arrValue = null;

            GotoOpParams gopParams = null;

            alRangeVectorsSourceAddresses = new ArrayList();

            vectList = (Structure)Calibration.slExtStructures[Tools.UniqueAddress(Calibration.BankNum, iAddress)];
            if (vectList == null)
            {
                vectList = new Structure();
                vectList.BankNum = Calibration.BankNum;
                vectList.AddressInt = iAddress;
                vectList.AddressBinInt = iAddress + Calibration.BankAddressBinInt;
                vectList.StructDefString = "Vect" + Num.ToString();
                vectList.Number = 0;
            }
            if (vectList.S6xStructure == null && S6x.slProcessStructures.ContainsKey(vectList.UniqueAddress))
            {
                vectList.S6xStructure = (S6xStructure)S6x.slProcessStructures[vectList.UniqueAddress];
            }
            if (vectList.S6xStructure != null) if (vectList.S6xStructure.Number > 0) iNumber = vectList.S6xStructure.Number;

            iCount = 0;
            bNextVector = (iAddress >= 0 && iAddress < Calibration.BankAddressEndInt);
            while (bNextVector)
            {
                Vector addVector = new Vector();
                addVector.SourceBankNum = Calibration.BankNum;
                addVector.SourceAddressInt = iAddress;
                addVector.ApplyOnBankNum = Num;

                arrValue = null;
                switch (addVector.SourceBankNum)
                {
                    case 8:
                        if (Bank8 != null) arrValue = Bank8.getBytesArray(addVector.SourceAddressInt, 2);
                        break;
                    case 1:
                        if (Bank1 != null) arrValue = Bank1.getBytesArray(addVector.SourceAddressInt, 2);
                        break;
                    case 9:
                        if (Bank9 != null) arrValue = Bank9.getBytesArray(addVector.SourceAddressInt, 2);
                        break;
                    case 0:
                        if (Bank0 != null) arrValue = Bank0.getBytesArray(addVector.SourceAddressInt, 2);
                        break;
                }

                if (arrValue == null) break;

                addVector.InitialValue = string.Join(SADDef.GlobalSeparator.ToString(), arrValue);
                addVector.AddressInt = Convert.ToInt32(arrValue[1] + arrValue[0], 16) - SADDef.EecBankStartAddress;
                
                addVector.isValid = true;
                // Valid by Default, but Addresses could be pointer to Reserved, RBases Addresses or Pointer to Source Address in same range

                // In Bank Address
                addVector.isValid = (addVector.AddressInt >= 0 && addVector.AddressInt < arrBytes.Length - 1);

                //  8061 Calibration Console or Engineering Console Vectors
                if (is8061)
                {
                    if (addVector.AddressInt + SADDef.EecBankStartAddress >= SADDef.CCMemory8061MinAdress && addVector.AddressInt + SADDef.EecBankStartAddress <= SADDef.CCMemory8061MaxAdress)
                    {
                        addVector.isValid = false;
                        addVector.Label = SADDef.LongLabelCC8061Vector;
                    }
                    else if (addVector.AddressInt + SADDef.EecBankStartAddress >= SADDef.ECMemory8061MinAdress && addVector.AddressInt + SADDef.EecBankStartAddress <= SADDef.ECMemory8061MaxAdress)
                    {
                        addVector.isValid = false;
                        addVector.Label = SADDef.LongLabelEC8061Vector;
                    }
                }

                //  Pointer to Source Address in same range => Not Valid
                if (addVector.isValid) addVector.isValid = !alRangeVectorsSourceAddresses.Contains(addVector.Address);

                //  Pointer to Reserved Addresses => Not Valid
                if (addVector.isValid)
                {
                    foreach (ReservedAddress resAdr in slReserved.Values)
                    {
                        if (addVector.AddressInt >= resAdr.AddressInt && addVector.AddressInt <= resAdr.AddressEndInt)
                        {
                            addVector.isValid = false;
                            break;
                        }
                    }
                }

                //  Pointer to RBases Addresses => Not Valid, except for early 8061
                if (addVector.isValid)
                {
                    if (addVector.ApplyOnBankNum == Calibration.BankNum && (!is8061 || !isEarly))
                    {
                        foreach (RBase rBase in Calibration.slRbases.Values)
                        {
                            if (addVector.AddressInt >= rBase.AddressBankInt && addVector.AddressInt <= rBase.AddressBankEndInt)
                            {
                                addVector.isValid = false;
                                break;
                            }
                        }
                    }
                }

                bNextVector = (addVector.SourceAddressInt >= 0 && addVector.SourceAddressInt < arrBytes.Length - 1);
                // 2018-06-05 Removed and managed on Vector.isValid information. Check if it is not generating issues, like continuing to read bad vectors.
                //bNextVector &= (addVector.AddressInt >= 0 && addVector.AddressInt < arrBytes.Length - 1);
                
                if (iNumber == -1)
                //  Check when Number was not found
                {
                    // Validity - Including Enginering Console and Calibration Console
                    if (bNextVector) bNextVector = addVector.isValid || addVector.Label == SADDef.LongLabelCC8061Vector || addVector.Label == SADDef.LongLabelEC8061Vector;
                    // Already Existing Element at source address
                    if (bNextVector) bNextVector = !Calibration.slCalls.ContainsKey(addVector.UniqueSourceAddress);
                    if (bNextVector) bNextVector = !Calibration.slCalibrationElements.ContainsKey(addVector.UniqueSourceAddress);
                    if (bNextVector) bNextVector = !Calibration.slExtScalars.ContainsKey(addVector.UniqueSourceAddress);
                    if (bNextVector) bNextVector = !Calibration.slExtFunctions.ContainsKey(addVector.UniqueSourceAddress);
                    if (bNextVector) bNextVector = !Calibration.slExtTables.ContainsKey(addVector.UniqueSourceAddress);
                    if (bNextVector) bNextVector = !Calibration.slExtStructures.ContainsKey(addVector.UniqueSourceAddress);
                    // Already Declared Element at source address
                    if (bNextVector) bNextVector = !S6x.slProcessRoutines.ContainsKey(addVector.UniqueSourceAddress);
                    if (bNextVector) bNextVector = !S6x.slProcessOperations.ContainsKey(addVector.UniqueSourceAddress);
                    if (bNextVector) bNextVector = !S6x.slProcessScalars.ContainsKey(addVector.UniqueSourceAddress);
                    if (bNextVector) bNextVector = !S6x.slProcessFunctions.ContainsKey(addVector.UniqueSourceAddress);
                    if (bNextVector) bNextVector = !S6x.slProcessTables.ContainsKey(addVector.UniqueSourceAddress);
                    if (bNextVector)
                    {
                        if (S6x.slProcessStructures.ContainsKey(addVector.UniqueSourceAddress))
                        {
                            bNextVector = ((S6xStructure)S6x.slProcessStructures[addVector.UniqueSourceAddress]).isVectorsList;
                        }
                    }
                    // Already Existing Element at source address + 1
                    if (bNextVector) bNextVector = !Calibration.slCalls.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    if (bNextVector) bNextVector = !Calibration.slCalibrationElements.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    if (bNextVector) bNextVector = !Calibration.slExtScalars.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    if (bNextVector) bNextVector = !Calibration.slExtFunctions.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    if (bNextVector) bNextVector = !Calibration.slExtTables.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    if (bNextVector) bNextVector = !Calibration.slExtStructures.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    // Already Declared Element at source address + 1
                    if (bNextVector) bNextVector = !S6x.slProcessRoutines.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    if (bNextVector) bNextVector = !S6x.slProcessOperations.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    if (bNextVector) bNextVector = !S6x.slProcessScalars.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    if (bNextVector) bNextVector = !S6x.slProcessFunctions.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    if (bNextVector) bNextVector = !S6x.slProcessTables.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1));
                    if (bNextVector)
                    {
                        if (S6x.slProcessStructures.ContainsKey(Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1)))
                        {
                            bNextVector = ((S6xStructure)S6x.slProcessStructures[Tools.UniqueAddress(addVector.SourceBankNum, addVector.SourceAddressInt + 1)]).isVectorsList;
                        }
                    }
                }

                if (bNextVector)
                {
                    if (!Calibration.slAdditionalVectors.ContainsKey(addVector.UniqueSourceAddress))
                    {
                        addVector.VectList = vectList;
                        Calibration.slAdditionalVectors.Add(addVector.UniqueSourceAddress, addVector);
                        if (!vectList.Vectors.ContainsKey(addVector.UniqueSourceAddress)) vectList.Vectors.Add(addVector.UniqueSourceAddress, addVector);
                        vectList.Number = vectList.Vectors.Count;
                        if (!Calibration.slAdditionalVectorsLists.ContainsKey(vectList.UniqueAddress)) Calibration.slAdditionalVectorsLists.Add(vectList.UniqueAddress, vectList);
                        if (!Calibration.slExtStructures.ContainsKey(vectList.UniqueAddress)) Calibration.slExtStructures.Add(vectList.UniqueAddress, vectList);

                        // For Validity check only
                        alRangeVectorsSourceAddresses.Add(addVector.SourceAddress);

                        // Invalid Vectors should not be processed
                        if (addVector.isValid)
                        {
                            // Jump Conflict Check (Required with Additional Vectors), except for early 8061
                            if (!isJumpAddressInConflict(addVector.AddressInt, -1) || (is8061 && isEarly))
                            {
                                gopParams = null;
                                processOperations(addVector.AddressInt, AddressInternalEndInt, CallType.Call, false, true, Num, AddressInternalInt, ref gopParams, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                            }
                        }
                    }
                    iCount++;
                    if (iCount == iNumber) break;
                }
                iAddress += 2;
            }

            alRangeVectorsSourceAddresses = null;
        }

        private void identifyCall(ref Call cCall, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            switch (cCall.CallType)
            {
                case CallType.ShortJump:
                case CallType.Goto:
                case CallType.Skip:
                case CallType.Unknown:
                    return;
            }

            // Init Call Identification for Constant Registers 
            identifyInitCall(ref cCall, ref S6x);
            // Core Calls Identification from S6x Matching Signature - No S6x Routine creation
            identifyCoreCallS6xSignature(ref cCall, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
            // Call with Args Identification
            identifyCallArgsType(ref cCall);
            // Call Type Identification (Vector, Routine, ...)
            identifyCallType(ref cCall);
            // Routine Code Identification (Preidentified Routines)
            identifyRoutineCode(ref cCall);

            // Call Identification preparation from S6x Matching Signature for S6x Routine
            identifyCallS6xSignature(ref cCall, ref S6x);
            // Call Identification Overriden with S6x Routine
            identifyCallS6xRoutine(ref cCall, ref S6x);
        }

        // Calculate S6xSignature Matching Parameter
        private string getS6xSignatureMatchingParameter(string variableValue, ref SortedList slMatchingParameters, bool pointerResult)
        {
            bool replacedParams = false;
            string[] arrMixedValues = null;

            if (variableValue == null) return string.Empty;
            if (variableValue == string.Empty) return string.Empty;
            
            if (slMatchingParameters != null)
            {
                foreach (string paramKey in slMatchingParameters.Keys)
                {
                    if (!variableValue.Contains(paramKey)) continue;
                    variableValue = variableValue.Replace(paramKey, slMatchingParameters[paramKey].ToString());
                    replacedParams = true;
                }
                if (replacedParams)
                {
                    if (variableValue.Contains(SADDef.VariableValuesSeparator)) arrMixedValues = variableValue.Split(SADDef.VariableValuesSeparator.ToCharArray());
                    else arrMixedValues = new string[] { variableValue };

                    for (int iValue = 0; iValue < arrMixedValues.Length; iValue++)
                    {
                        if (arrMixedValues[iValue].Contains(SADDef.AdditionSeparator))
                        {
                            bool rConstMode = false;
                            int calculatedValue = 0;
                            string[] arrValues = arrMixedValues[iValue].Split(SADDef.AdditionSeparator.ToCharArray());
                            bool errorMode = false;
                            for (int i = 0; i < arrValues.Length; i++)
                            {
                                try
                                {
                                    if (rConstMode && arrValues[i].Length <= 2) calculatedValue += Convert.ToSByte(arrValues[i], 16);
                                    else calculatedValue += Convert.ToInt32(arrValues[i], 16);
                                    if (i == 0)
                                    {
                                        if (Calibration.slRbases.ContainsKey(arrValues[i].ToLower()))
                                        {
                                            calculatedValue = ((RBase)Calibration.slRbases[arrValues[i].ToLower()]).AddressBankInt + SADDef.EecBankStartAddress;
                                        }
                                        else if (Calibration.slRconst.ContainsKey(arrValues[i].ToLower()))
                                        {
                                            calculatedValue = ((RConst)Calibration.slRconst[arrValues[i].ToLower()]).ValueInt;
                                            rConstMode = true;
                                        }
                                    }
                                }
                                catch
                                {
                                    errorMode = true;
                                    break;
                                }
                            }
                            if (!errorMode) arrMixedValues[iValue] = Convert.ToString(calculatedValue, 16);
                        }

                        try
                        {
                            arrMixedValues[iValue] = Convert.ToString(Convert.ToInt32(arrMixedValues[iValue], 16), 16);
                            if (pointerResult) arrMixedValues[iValue] = Tools.PointerTranslation(arrMixedValues[iValue]);
                        }
                        catch { }
                    }

                    if (arrMixedValues.Length == 1) variableValue = arrMixedValues[0];
                    else variableValue = string.Join(SADDef.VariableValuesSeparator, arrMixedValues);
                }
            }

            return variableValue;
        }

        // Core Calls Identification from S6x Matching Signature - No S6x Routine creation
        private void identifyCoreCallS6xSignature(ref Call cCall, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            // Already processed
            if (cCall.isIdentified) return;

            SADFixedSigs.Fixed_Routines fixedRoutineType = SADFixedSigs.Fixed_Routines.UNKNOWN;

            // Array containing managed forced signatures keys and SADFixedSigs.Fixed_Routines for value
            SortedList slSignaturesKeys = new SortedList();
            
            foreach (object[] routineSignature in SADFixedSigs.Fixed_Routines_Signatures)
            {
                fixedRoutineType = (SADFixedSigs.Fixed_Routines)routineSignature[1];
                switch (fixedRoutineType)
                {
                    case SADFixedSigs.Fixed_Routines.CORE_REG_INIT_8065:
                        foreach (Routine roRo in Calibration.slRoutines.Values)
                        {
                            if (roRo.Code == RoutineCode.Init)
                            {
                                // Equivalent to Init Routine
                                fixedRoutineType = SADFixedSigs.Fixed_Routines.UNKNOWN;
                                break;
                            }
                            else if (roRo.Code == RoutineCode.CoreInit)
                            {
                                // Already processed
                                fixedRoutineType = SADFixedSigs.Fixed_Routines.UNKNOWN;
                                break;
                            }

                        }
                        break;
                    default:
                        fixedRoutineType = SADFixedSigs.Fixed_Routines.UNKNOWN;
                        break;
                }
                if (fixedRoutineType == SADFixedSigs.Fixed_Routines.UNKNOWN) continue;
                if (slSignaturesKeys.ContainsKey(routineSignature[0].ToString())) continue;

                slSignaturesKeys.Add(routineSignature[0].ToString(), fixedRoutineType);
            }

            // Nothing to managed anymore
            if (slSignaturesKeys.Count == 0)
            {
                slSignaturesKeys = null;
                return;
            }

            fixedRoutineType = SADFixedSigs.Fixed_Routines.UNKNOWN;
            foreach (MatchingSignature mSig in Calibration.slUnMatchedSignatures.Values)
            {
                if (!mSig.S6xSignature.Forced) continue;                                    // Searching only on forced signatures
                if (!slSignaturesKeys.ContainsKey(mSig.S6xSignature.UniqueKey)) continue;   // Only some signatures are core ones
                if (mSig.BankNum != Num) continue;
                if (!(mSig.MatchingStartAddressInt >= cCall.AddressInt && mSig.MatchingStartAddressInt <= cCall.AddressEndInt)) continue;

                fixedRoutineType = (SADFixedSigs.Fixed_Routines)slSignaturesKeys[mSig.S6xSignature.UniqueKey];

                switch (fixedRoutineType)
                {
                    case SADFixedSigs.Fixed_Routines.CORE_REG_INIT_8065:
                        identifyCoreCallInitS6xSignature(ref cCall, mSig, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                        break;
                }
                break;
            }

            slSignaturesKeys = null;
        }

        // Core Calls Identification from S6x Matching Signature - No S6x Routine creation
        //      Core Init Call / CORE_REG_INIT_8065
        private void identifyCoreCallInitS6xSignature(ref Call cCall, MatchingSignature mSig, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            // Already processed
            if (cCall.isIdentified) return;

            if (mSig.S6xSignature.InternalStructures == null) return;
            if (mSig.S6xSignature.InternalStructures.Length != 2) return;

            cCall.isRoutine = true;

            Routine rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
            rRoutine.Code = RoutineCode.CoreInit;
            rRoutine.SetTranslationComments();
            if (!Calibration.slRoutines.ContainsKey(rRoutine.UniqueAddress)) Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);
            rRoutine = null;
            
            SADBank Bank = null;
            switch (Calibration.BankNum)
            {
                case 8:
                    Bank = Bank8;
                    break;
                case 1:
                    Bank = Bank1;
                    break;
                case 9:
                    Bank = Bank9;
                    break;
                case 0:
                    Bank = Bank0;
                    break;
            }

            int iRConstCheckStartAddr = -1;
            int iRConstCheckIter = -1;
            ArrayList alNewStructures = new ArrayList();
            foreach (S6xRoutineInternalStructure s6xObject in mSig.S6xSignature.InternalStructures)
            {
                s6xObject.VariableBankNum = Bank.Num.ToString();
                s6xObject.VariableAddress = getS6xSignatureMatchingParameter(s6xObject.VariableAddress, ref mSig.slMatchingParameters, false);

                int iAddress = Convert.ToInt32(s6xObject.VariableAddress, 16) - SADDef.EecBankStartAddress;

                if (s6xObject.ShortLabel == SADFixedStructures.GetFixedStructureTemplate(SADFixedStructures.FixedStructures.CORE_REG_INIT_ST1).ShortLabel)
                {
                    // First structure
                    //  Number has to be defined
                    s6xObject.Number = 0;
                    try
                    {
                        while (true)
                        {
                            if (Bank.getWordInt(iAddress + s6xObject.Number * 4, false, true) == 0) break;
                            s6xObject.Number++;
                            if (s6xObject.Number > 32)
                            {
                                s6xObject.Number = 0;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        s6xObject.Number = 0;
                    }
                }
                else if (s6xObject.ShortLabel == SADFixedStructures.GetFixedStructureTemplate(SADFixedStructures.FixedStructures.CORE_REG_INIT_ST2).ShortLabel)
                {
                    // Second structure
                    //  To be duplicated
                    //  Number and Def have to be defined
                    //  RConst have to be detected if any
                    int regTopByte = -1;
                    int iIterations = -1;
                    int iCurrentStruct = -1;

                    try
                    {
                        while (true)
                        {
                            regTopByte = Bank.getByteInt(iAddress, false);
                            if (regTopByte >= 255) break;

                            iIterations = Bank.getByteInt(iAddress + 1, false);

                            if (regTopByte == 0)
                            {
                                iRConstCheckStartAddr = iAddress + 2;
                                iRConstCheckIter = iIterations;
                            }

                            if (iCurrentStruct == -1)
                            {
                                s6xObject.Number = 1;
                                s6xObject.StructDef = "\"Reg top byte \", ByteHex, Empty, Empty, \"Iterations \", Byte\\n\r\n";
                                for (int iIter = 0; iIter < iIterations; iIter++)
                                {
                                    s6xObject.StructDef += "\"Reg \", Empty, \"" + string.Format("{0:x2}", regTopByte) + "\", ByteHex, \"= \", ByteHex";
                                    if (iIter < iIterations - 1) s6xObject.StructDef += "\\n\r\n";
                                }
                                iCurrentStruct++;
                            }
                            else
                            {
                                S6xStructure s6xStruct = new S6xStructure();
                                s6xStruct.BankNum = Bank.Num;
                                s6xStruct.AddressInt = iAddress;
                                s6xStruct.isCalibrationElement = false;
                                s6xStruct.ShortLabel = s6xObject.ShortLabel + "_" + string.Format("{0:x2}", iCurrentStruct + 1);
                                s6xStruct.Label = s6xObject.Label + " Part " + (iCurrentStruct + 2).ToString();
                                s6xStruct.Comments = s6xStruct.ShortLabel + " - " + s6xStruct.Label;
                                s6xStruct.Number = 1;                                            // To be updated by processing
                                s6xStruct.StructDef = "\"Reg top byte \", ByteHex, Empty, Empty, \"Iterations \", Byte\\n\r\n";
                                for (int iIter = 0; iIter < iIterations; iIter++)
                                {
                                    s6xStruct.StructDef += "\"Reg \", Empty, \"" + string.Format("{0:x2}", regTopByte) + "\", ByteHex, \"= \", ByteHex";
                                    if (iIter < iIterations - 1) s6xStruct.StructDef += "\\n\r\n";
                                }
                                alNewStructures.Add(s6xStruct);
                                iCurrentStruct++;
                            }
                            iAddress += 2 + iIterations * 2;
                        }
                    }
                    catch
                    {
                        s6xObject.Number = 0;
                    }
                }
            }
            foreach (S6xStructure s6xStruct in alNewStructures)
            {
                if (!S6x.slProcessStructures.ContainsKey(s6xStruct.UniqueAddress))
                {
                    S6x.slProcessStructures.Add(s6xStruct.UniqueAddress, s6xStruct);
                    if (S6x.slStructures.ContainsKey(s6xStruct.UniqueAddress)) S6x.slStructures[s6xStruct.UniqueAddress] = s6xStruct;
                    else S6x.slStructures.Add(s6xStruct.UniqueAddress, s6xStruct);
                }
            }
            alNewStructures = null;

            // RConst management
            if (iRConstCheckStartAddr > 0 && iRConstCheckIter > 0)
            {
                string[] arrBytes = Bank.getBytesArray(iRConstCheckStartAddr, iRConstCheckIter * 2);
                int iIndex = 0;
                while (true)
                {
                    if (iIndex + 8 > arrBytes.Length) break;
                    int iReg1 = Convert.ToInt32(arrBytes[iIndex], 16);
                    int iReg2 = Convert.ToInt32(arrBytes[iIndex + 2], 16);
                    int iReg3 = Convert.ToInt32(arrBytes[iIndex + 4], 16);
                    int iReg4 = Convert.ToInt32(arrBytes[iIndex + 6], 16);
                    if (iReg1 + 1 != iReg2 || iReg1 + 2 != iReg3 || iReg1 + 3 != iReg4)
                    {
                        iIndex += 2;
                        continue;
                    }
                    int iVal1 = Convert.ToInt32(arrBytes[iIndex + 1], 16);
                    int iVal2 = Convert.ToInt32(arrBytes[iIndex + 3], 16);
                    int iVal3 = Convert.ToInt32(arrBytes[iIndex + 5], 16);
                    int iVal4 = Convert.ToInt32(arrBytes[iIndex + 7], 16);
                    if (iVal1 != iVal3)
                    {
                        iIndex += 2;
                        continue;
                    }
                    if (iVal2 + 1 != iVal4)
                    {
                        iIndex += 2;
                        continue;
                    }

                    RConst rConst = null;
                    Register rReg = null;

                    rConst = new RConst(arrBytes[iIndex], iVal2 * 0x100 + iVal1);
                    if (!Calibration.slRconst.ContainsKey(rConst.Code))
                    {
                        Calibration.slRconst.Add(rConst.Code, rConst);
                        rReg = (Register)Calibration.slRegisters[Tools.RegisterUniqueAddress(rConst.Code)];
                        if (rReg != null) rReg.RConst = rConst;
                    }

                    rConst = new RConst(arrBytes[iIndex + 4], iVal4 * 0x100 + iVal3);
                    if (!Calibration.slRconst.ContainsKey(rConst.Code))
                    {
                        Calibration.slRconst.Add(rConst.Code, rConst);
                        rReg = (Register)Calibration.slRegisters[Tools.RegisterUniqueAddress(rConst.Code)];
                        if (rReg != null) rReg.RConst = rConst;
                    }

                    iIndex += 4;        // First Code will be reprocessed, but it is required when numbers are impair
                }
            }

            // 20200512 - PYM
            // Added to S6xRegisters
            foreach (RConst rConstCpy in Calibration.slRconst.Values)
            {
                S6xRegister s6xReg = (S6xRegister)S6x.slProcessRegisters[Tools.RegisterUniqueAddress(rConstCpy.Code)];
                if (s6xReg == null)
                {
                    s6xReg = new S6xRegister(rConstCpy.Code);
                    s6xReg.Label = Tools.RegisterInstruction(rConstCpy.Code);
                    s6xReg.Comments = "Constant register (RConst) " + s6xReg.Label;
                    S6x.slProcessRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                    if (S6x.slRegisters.ContainsKey(s6xReg.UniqueAddress)) S6x.slRegisters[s6xReg.UniqueAddress] = s6xReg;
                    else S6x.slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                }
                s6xReg.isRBase = false;
                s6xReg.isRConst = true;
                s6xReg.ConstValue = rConstCpy.Value;
                s6xReg.AutoConstValue = true;
            }
        }

        // Call Identification S6x Routine, S6x Internal Elements generation from Matching Signature
        private void identifyCallS6xSignature(ref Call cCall, ref SADS6x S6x)
        {
            string matchedUniqueMatchingStartAddress = string.Empty;

            foreach (MatchingSignature mSig in Calibration.slUnMatchedSignatures.Values)
            {
                if (mSig.BankNum != Num) continue;
                if (!(mSig.MatchingStartAddressInt >= cCall.AddressInt && mSig.MatchingStartAddressInt <= cCall.AddressEndInt)) continue;

                matchedUniqueMatchingStartAddress = mSig.UniqueMatchingStartAddress;

                S6xRoutine s6xRoutine = (S6xRoutine)S6x.slProcessRoutines[cCall.UniqueAddress];
                // S6x Routine is not defined for now - A new one will be created for 
                if (s6xRoutine == null)
                {
                    // Creation only when Routine ShortLabel is defined
                    if (mSig.S6xSignature.ShortLabel != null && mSig.S6xSignature.ShortLabel != string.Empty)
                    {
                        s6xRoutine = new S6xRoutine();
                        s6xRoutine.BankNum = Num;
                        s6xRoutine.AddressInt = cCall.AddressInt;

                        s6xRoutine.Label = mSig.S6xSignature.Label;
                        s6xRoutine.ShortLabel = mSig.S6xSignature.ShortLabel;
                        s6xRoutine.Comments = mSig.S6xSignature.Comments;
                        s6xRoutine.OutputComments = mSig.S6xSignature.OutputComments;

                        if (mSig.S6xSignature.InputArguments != null)
                        {
                            s6xRoutine.InputArguments = new S6xRoutineInputArgument[mSig.S6xSignature.InputArguments.Length];
                            for (int i = 0; i < mSig.S6xSignature.InputArguments.Length; i++) s6xRoutine.InputArguments[i] = mSig.S6xSignature.InputArguments[i].Clone();
                        }
                        else if (cCall.Arguments != null && cCall.ByteArgsNum > 0)
                        {
                            s6xRoutine.InputArguments = new S6xRoutineInputArgument[cCall.Arguments.Length];
                            for (int iArg = 0; iArg < s6xRoutine.InputArguments.Length; iArg++)
                            {
                                s6xRoutine.InputArguments[iArg] = new S6xRoutineInputArgument();
                                s6xRoutine.InputArguments[iArg].Position = iArg + 1;
                                s6xRoutine.InputArguments[iArg].UniqueKey = string.Format("Ra{0:d3}", s6xRoutine.InputArguments[iArg].Position);
                                s6xRoutine.InputArguments[iArg].Encryption = (int)cCall.Arguments[iArg].Mode;
                                s6xRoutine.InputArguments[iArg].Word = cCall.Arguments[iArg].Word;
                                s6xRoutine.InputArguments[iArg].Pointer = s6xRoutine.InputArguments[iArg].Word;
                            }
                        }
                        if (mSig.S6xSignature.InputStructures != null)
                        {
                            s6xRoutine.InputStructures = new S6xRoutineInputStructure[mSig.S6xSignature.InputStructures.Length];
                            for (int i = 0; i < mSig.S6xSignature.InputStructures.Length; i++) s6xRoutine.InputStructures[i] = mSig.S6xSignature.InputStructures[i].Clone();
                        }
                        if (mSig.S6xSignature.InputTables != null)
                        {
                            s6xRoutine.InputTables = new S6xRoutineInputTable[mSig.S6xSignature.InputTables.Length];
                            for (int i = 0; i < mSig.S6xSignature.InputTables.Length; i++) s6xRoutine.InputTables[i] = mSig.S6xSignature.InputTables[i].Clone();
                        }
                        if (mSig.S6xSignature.InputFunctions != null)
                        {
                            s6xRoutine.InputFunctions = new S6xRoutineInputFunction[mSig.S6xSignature.InputFunctions.Length];
                            for (int i = 0; i < mSig.S6xSignature.InputFunctions.Length; i++) s6xRoutine.InputFunctions[i] = mSig.S6xSignature.InputFunctions[i].Clone();
                        }
                        if (mSig.S6xSignature.InputScalars != null)
                        {
                            s6xRoutine.InputScalars = new S6xRoutineInputScalar[mSig.S6xSignature.InputScalars.Length];
                            for (int i = 0; i < mSig.S6xSignature.InputScalars.Length; i++) s6xRoutine.InputScalars[i] = mSig.S6xSignature.InputScalars[i].Clone();
                        }

                        s6xRoutine.ByteArgumentsNum = cCall.ByteArgsNum;
                        if (cCall.ArgsCondValidated) s6xRoutine.ByteArgumentsNum += cCall.ArgsNumCondAdder;
                        if (s6xRoutine.ByteArgumentsNum < 0) s6xRoutine.ByteArgumentsNum = 0;
                        if (cCall.ArgsStackDepthMax != 1) s6xRoutine.ByteArgumentsNum = 0;

                        if (s6xRoutine.InputArguments != null)
                        {
                            s6xRoutine.ByteArgumentsNumOverride = true;
                            s6xRoutine.ByteArgumentsNum = 0;
                            foreach (S6xRoutineInputArgument s6xInput in s6xRoutine.InputArguments)
                            {
                                s6xRoutine.ByteArgumentsNum++;
                                if (s6xInput.Word) s6xRoutine.ByteArgumentsNum++;
                            }
                        }

                        if (s6xRoutine.InputStructures != null)
                        {
                            foreach (S6xRoutineInputStructure s6xInput in s6xRoutine.InputStructures)
                            {
                                s6xInput.VariableAddress = getS6xSignatureMatchingParameter(s6xInput.VariableAddress, ref mSig.slMatchingParameters, true);
                                s6xInput.VariableNumber = getS6xSignatureMatchingParameter(s6xInput.VariableNumber, ref mSig.slMatchingParameters, true);
                                s6xInput.ForcedNumber = getS6xSignatureMatchingParameter(s6xInput.ForcedNumber, ref mSig.slMatchingParameters, false);
                            }
                        }

                        if (s6xRoutine.InputTables != null)
                        {
                            foreach (S6xRoutineInputTable s6xInput in s6xRoutine.InputTables)
                            {
                                s6xInput.VariableAddress = getS6xSignatureMatchingParameter(s6xInput.VariableAddress, ref mSig.slMatchingParameters, true);
                                s6xInput.VariableColsNumberReg = getS6xSignatureMatchingParameter(s6xInput.VariableColsNumberReg, ref mSig.slMatchingParameters, true);
                                s6xInput.VariableColsReg = getS6xSignatureMatchingParameter(s6xInput.VariableColsReg, ref mSig.slMatchingParameters, true);
                                s6xInput.VariableRowsReg = getS6xSignatureMatchingParameter(s6xInput.VariableRowsReg, ref mSig.slMatchingParameters, true);
                                s6xInput.VariableOutput = getS6xSignatureMatchingParameter(s6xInput.VariableOutput, ref mSig.slMatchingParameters, true);
                                s6xInput.ForcedColsNumber = getS6xSignatureMatchingParameter(s6xInput.ForcedColsNumber, ref mSig.slMatchingParameters, false);
                                s6xInput.ForcedRowsNumber = getS6xSignatureMatchingParameter(s6xInput.ForcedRowsNumber, ref mSig.slMatchingParameters, false);
                            }
                        }

                        if (s6xRoutine.InputFunctions != null)
                        {
                            foreach (S6xRoutineInputFunction s6xInput in s6xRoutine.InputFunctions)
                            {
                                s6xInput.VariableAddress = getS6xSignatureMatchingParameter(s6xInput.VariableAddress, ref mSig.slMatchingParameters, true);
                                s6xInput.VariableInput = getS6xSignatureMatchingParameter(s6xInput.VariableInput, ref mSig.slMatchingParameters, true);
                                s6xInput.VariableOutput = getS6xSignatureMatchingParameter(s6xInput.VariableOutput, ref mSig.slMatchingParameters, true);
                                s6xInput.ForcedRowsNumber = getS6xSignatureMatchingParameter(s6xInput.ForcedRowsNumber, ref mSig.slMatchingParameters, false);
                            }
                        }

                        if (s6xRoutine.InputScalars != null)
                        {
                            foreach (S6xRoutineInputScalar s6xInput in s6xRoutine.InputScalars)
                            {
                                s6xInput.VariableAddress = getS6xSignatureMatchingParameter(s6xInput.VariableAddress, ref mSig.slMatchingParameters, true);
                            }
                        }

                        S6x.slProcessRoutines.Add(s6xRoutine.UniqueAddress, s6xRoutine);

                        if (S6x.slRoutines.ContainsKey(s6xRoutine.UniqueAddress)) S6x.slRoutines[s6xRoutine.UniqueAddress] = s6xRoutine;
                        else S6x.slRoutines.Add(s6xRoutine.UniqueAddress, s6xRoutine);

                        if (mSig.S6xSignature.Information != string.Empty) mSig.S6xSignature.Information += "\r\n";
                        mSig.S6xSignature.Information += "Related Routine generated on Bank " + s6xRoutine.BankNum + " at " + s6xRoutine.Address;
                    }
                    else
                    {
                        if (mSig.S6xSignature.Information != string.Empty) mSig.S6xSignature.Information += "\r\n";
                        mSig.S6xSignature.Information += "No routine generated on Bank " + cCall.BankNum + " at " + cCall.Address;
                        mSig.S6xSignature.Information += "\r\nbecause routine short label is empty.";
                    }
                }
                else
                {
                    if (mSig.S6xSignature.Information != string.Empty) mSig.S6xSignature.Information += "\r\n";
                    mSig.S6xSignature.Information += "Related Routine linked on Bank " + s6xRoutine.BankNum + " at " + s6xRoutine.Address;
                    mSig.S6xSignature.Information += "\r\nRelated Routine is " + s6xRoutine.ShortLabel + " - " + s6xRoutine.Label;
                }

                if (s6xRoutine != null)
                {
                    s6xRoutine.SignatureForced = mSig.S6xSignature.Forced;
                    s6xRoutine.SignatureKey = mSig.S6xSignature.UniqueKey;
                }
                s6xRoutine = null;

                // Internal Elements Management by S6x Elements creation when not existing
                if (mSig.S6xSignature.InternalStructures != null)
                {
                    foreach (S6xRoutineInternalStructure s6xInternal in mSig.S6xSignature.InternalStructures)
                    {
                        string variableAddress = getS6xSignatureMatchingParameter(s6xInternal.VariableAddress, ref mSig.slMatchingParameters, false);
                        string variableBankNum = getS6xSignatureMatchingParameter(s6xInternal.VariableBankNum, ref mSig.slMatchingParameters, false);
                        int address = -1;
                        int bankNum = -1;
                        try
                        {
                            address = Convert.ToInt32(variableAddress, 16) - SADDef.EecBankStartAddress;
                            bankNum = Convert.ToInt32(variableBankNum, 16);
                        }
                        catch { }
                        if (address >= 0 && Calibration.Info.slBanksInfos.ContainsKey(bankNum))
                        {
                            S6xStructure s6xElement = new S6xStructure();
                            s6xElement.BankNum = bankNum;
                            s6xElement.AddressInt = address;
                            s6xElement.isCalibrationElement = false;
                            if (s6xElement.BankNum == Calibration.BankNum && Calibration.isCalibrationAddress(s6xElement.AddressInt)) s6xElement.isCalibrationElement = true;

                            s6xElement.StructDef = s6xInternal.StructDef;
                            s6xElement.Number = s6xInternal.Number;

                            s6xElement.Label = s6xInternal.Label;
                            s6xElement.ShortLabel = s6xInternal.ShortLabel;

                            s6xElement.Comments = s6xInternal.Comments;
                            s6xElement.OutputComments = s6xInternal.OutputComments;

                            if (mSig.S6xSignature.Information != string.Empty) mSig.S6xSignature.Information += "\r\n";
                            if (!S6x.slProcessStructures.ContainsKey(s6xElement.UniqueAddress))
                            {
                                S6x.slProcessStructures.Add(s6xElement.UniqueAddress, s6xElement);
                                if (S6x.slStructures.ContainsKey(s6xElement.UniqueAddress))
                                {
                                    mSig.S6xSignature.Information += "Related Structure processed as default on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                                }
                                else
                                {
                                    mSig.S6xSignature.Information += "Related Structure generated on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                                    S6x.slStructures.Add(s6xElement.UniqueAddress, s6xElement);
                                }
                            }
                            else
                            {
                                mSig.S6xSignature.Information += "Related Structure will use its existing definition on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                            }
                        }
                    }
                }
                if (mSig.S6xSignature.InternalTables != null)
                {
                    foreach (S6xRoutineInternalTable s6xInternal in mSig.S6xSignature.InternalTables)
                    {
                        string variableAddress = getS6xSignatureMatchingParameter(s6xInternal.VariableAddress, ref mSig.slMatchingParameters, false);
                        string variableBankNum = getS6xSignatureMatchingParameter(s6xInternal.VariableBankNum, ref mSig.slMatchingParameters, false);
                        string variableColsNum = getS6xSignatureMatchingParameter(s6xInternal.VariableColsNumber, ref mSig.slMatchingParameters, false);
                        int address = -1;
                        int bankNum = -1;
                        int colsNum = -1;
                        try
                        {
                            address = Convert.ToInt32(variableAddress, 16) - SADDef.EecBankStartAddress;
                            bankNum = Convert.ToInt32(variableBankNum, 16);
                            colsNum = Convert.ToInt32(variableColsNum, 16);
                        }
                        catch { }
                        if (address >= 0 && Calibration.Info.slBanksInfos.ContainsKey(bankNum) && colsNum > 0)
                        {
                            S6xTable s6xElement = new S6xTable();
                            s6xElement.BankNum = bankNum;
                            s6xElement.AddressInt = address;
                            s6xElement.isCalibrationElement = false;
                            if (s6xElement.BankNum == Calibration.BankNum && Calibration.isCalibrationAddress(s6xElement.AddressInt)) s6xElement.isCalibrationElement = true;

                            s6xElement.WordOutput = s6xInternal.WordOutput;
                            s6xElement.SignedOutput = s6xInternal.SignedOutput;
                            s6xElement.ColsNumber = colsNum;

                            s6xElement.RowsNumber = s6xInternal.RowsNumber;

                            s6xElement.Label = s6xInternal.Label;
                            s6xElement.ShortLabel = s6xInternal.ShortLabel;

                            s6xElement.Comments = s6xInternal.Comments;
                            s6xElement.OutputComments = s6xInternal.OutputComments;

                            s6xElement.CellsScaleExpression = s6xInternal.CellsScaleExpression;
                            s6xElement.CellsScalePrecision = s6xInternal.CellsScalePrecision;
                            s6xElement.CellsUnits = s6xInternal.CellsUnits;
                            s6xElement.ColsUnits = s6xInternal.ColsUnits;
                            s6xElement.RowsUnits = s6xInternal.RowsUnits;

                            if (mSig.S6xSignature.Information != string.Empty) mSig.S6xSignature.Information += "\r\n";
                            if (!S6x.slProcessTables.ContainsKey(s6xElement.UniqueAddress))
                            {
                                S6x.slProcessTables.Add(s6xElement.UniqueAddress, s6xElement);
                                if (S6x.slTables.ContainsKey(s6xElement.UniqueAddress))
                                {
                                    mSig.S6xSignature.Information += "Related Table processed as default on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                                }
                                else
                                {
                                    mSig.S6xSignature.Information += "Related Table generated on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                                    S6x.slTables.Add(s6xElement.UniqueAddress, s6xElement);
                                }
                            }
                            else
                            {
                                mSig.S6xSignature.Information += "Related Table will use its existing definition on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                            }
                        }
                    }
                }
                if (mSig.S6xSignature.InternalFunctions != null)
                {
                    foreach (S6xRoutineInternalFunction s6xInternal in mSig.S6xSignature.InternalFunctions)
                    {
                        string variableAddress = getS6xSignatureMatchingParameter(s6xInternal.VariableAddress, ref mSig.slMatchingParameters, false);
                        string variableBankNum = getS6xSignatureMatchingParameter(s6xInternal.VariableBankNum, ref mSig.slMatchingParameters, false);
                        int address = -1;
                        int bankNum = -1;
                        try
                        {
                            address = Convert.ToInt32(variableAddress, 16) - SADDef.EecBankStartAddress;
                            bankNum = Convert.ToInt32(variableBankNum, 16);
                        }
                        catch { }
                        if (address >= 0 && Calibration.Info.slBanksInfos.ContainsKey(bankNum))
                        {
                            S6xFunction s6xElement = new S6xFunction();
                            s6xElement.BankNum = bankNum;
                            s6xElement.AddressInt = address;
                            s6xElement.isCalibrationElement = false;
                            if (s6xElement.BankNum == Calibration.BankNum && Calibration.isCalibrationAddress(s6xElement.AddressInt)) s6xElement.isCalibrationElement = true;

                            s6xElement.ByteInput = s6xInternal.ByteInput;
                            s6xElement.ByteOutput = s6xInternal.ByteOutput;
                            s6xElement.SignedInput = s6xInternal.SignedInput;
                            s6xElement.SignedOutput = s6xInternal.SignedOutput;

                            s6xElement.RowsNumber = s6xInternal.RowsNumber;

                            s6xElement.Label = s6xInternal.Label;
                            s6xElement.ShortLabel = s6xInternal.ShortLabel;

                            s6xElement.Comments = s6xInternal.Comments;
                            s6xElement.OutputComments = s6xInternal.OutputComments;

                            s6xElement.InputScaleExpression = s6xInternal.InputScaleExpression;
                            s6xElement.OutputScaleExpression = s6xInternal.OutputScaleExpression;
                            s6xElement.InputScalePrecision = s6xInternal.InputScalePrecision;
                            s6xElement.OutputScalePrecision = s6xInternal.OutputScalePrecision;
                            s6xElement.InputUnits = s6xInternal.InputUnits;
                            s6xElement.OutputUnits = s6xInternal.OutputUnits;

                            if (mSig.S6xSignature.Information != string.Empty) mSig.S6xSignature.Information += "\r\n";
                            if (!S6x.slProcessFunctions.ContainsKey(s6xElement.UniqueAddress))
                            {
                                S6x.slProcessFunctions.Add(s6xElement.UniqueAddress, s6xElement);
                                if (S6x.slFunctions.ContainsKey(s6xElement.UniqueAddress))
                                {
                                    mSig.S6xSignature.Information += "Related Function processed as default on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                                }
                                else
                                {
                                    mSig.S6xSignature.Information += "Related Function generated on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                                    S6x.slFunctions.Add(s6xElement.UniqueAddress, s6xElement);
                                }
                            }
                            else
                            {
                                mSig.S6xSignature.Information += "Related Function will use its existing definition on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                            }
                        }
                    }
                }
                if (mSig.S6xSignature.InternalScalars != null)
                {
                    foreach (S6xRoutineInternalScalar s6xInternal in mSig.S6xSignature.InternalScalars)
                    {
                        string variableAddress = getS6xSignatureMatchingParameter(s6xInternal.VariableAddress, ref mSig.slMatchingParameters, false);
                        string variableBankNum = getS6xSignatureMatchingParameter(s6xInternal.VariableBankNum, ref mSig.slMatchingParameters, false);
                        int address = -1;
                        int bankNum = -1;
                        try
                        {
                            address = Convert.ToInt32(variableAddress, 16) - SADDef.EecBankStartAddress;
                            bankNum = Convert.ToInt32(variableBankNum, 16);
                        }
                        catch { }
                        if (address >= 0 && Calibration.Info.slBanksInfos.ContainsKey(bankNum))
                        {
                            S6xScalar s6xElement = new S6xScalar();
                            s6xElement.BankNum = bankNum;
                            s6xElement.AddressInt = address;
                            s6xElement.isCalibrationElement = false;
                            if (s6xElement.BankNum == Calibration.BankNum && Calibration.isCalibrationAddress(s6xElement.AddressInt)) s6xElement.isCalibrationElement = true;

                            s6xElement.Byte = s6xInternal.Byte;
                            s6xElement.Signed = s6xInternal.Signed;

                            s6xElement.Label = s6xInternal.Label;
                            s6xElement.ShortLabel = s6xInternal.ShortLabel;

                            s6xElement.Comments = s6xInternal.Comments;
                            s6xElement.OutputComments = s6xInternal.OutputComments;
                            s6xElement.InlineComments = s6xInternal.InlineComments;

                            s6xElement.ScaleExpression = s6xInternal.ScaleExpression;
                            s6xElement.ScalePrecision = s6xInternal.ScalePrecision;
                            s6xElement.Units = s6xInternal.Units;

                            if (mSig.S6xSignature.Information != string.Empty) mSig.S6xSignature.Information += "\r\n";
                            if (!S6x.slProcessScalars.ContainsKey(s6xElement.UniqueAddress))
                            {
                                S6x.slProcessScalars.Add(s6xElement.UniqueAddress, s6xElement);
                                if (S6x.slScalars.ContainsKey(s6xElement.UniqueAddress))
                                {
                                    mSig.S6xSignature.Information += "Related Scalar processed as default on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                                }
                                else
                                {
                                    mSig.S6xSignature.Information += "Related Scalar generated on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                                    S6x.slScalars.Add(s6xElement.UniqueAddress, s6xElement);
                                }
                            }
                            else
                            {
                                mSig.S6xSignature.Information += "Related Scalar will use its existing definition on Bank " + s6xElement.BankNum + " at " + s6xElement.Address;
                            }
                        }
                    }
                }

                break;
            }

            if (matchedUniqueMatchingStartAddress != string.Empty)
            {
                Calibration.slUnMatchedSignatures.Remove(matchedUniqueMatchingStartAddress);
            }
        }

        // Call Identification Overriden with S6x Routine
        private void identifyCallS6xRoutine(ref Call cCall, ref SADS6x S6x)
        {
            RoutineIOTable ioTable = null;
            RoutineIOFunction ioFunction = null;
            S6xRoutine s6xRoutine = null;
            S6xRoutineInputTable s6xInputTable = null;
            S6xRoutineInputFunction s6xInputFunction = null;
            S6xRoutine compRoutine = null;
            CallArgument[] callArgs = null;
            Routine rRoutine = null;
            string sRegister = string.Empty;
            int totalAdvancedNum = 0;
            int totalInputsNum = 0;
            int iInput = 0;
            bool compatibleRoutine = false;

            s6xRoutine = (S6xRoutine)S6x.slProcessRoutines[cCall.UniqueAddress];
            if (s6xRoutine == null) return;

            // Direct S6x Routine
            cCall.S6xRoutine = s6xRoutine;

            // Arguments Override
            if (s6xRoutine.ByteArgumentsNumOverride)
            {
                if (s6xRoutine.ByteArgumentsNum == 0)
                {
                    cCall.ArgsType = CallArgsType.None;
                    cCall.Arguments = null;
                }
                else if (cCall.ArgsType == CallArgsType.Unknown) cCall.ArgsType = CallArgsType.Fixed;

                if (cCall.ByteArgsNum != s6xRoutine.ByteArgumentsNum)
                {
                    cCall.ByteArgsNum = s6xRoutine.ByteArgumentsNum;
                    cCall.Arguments = null;
                    if (cCall.ArgsCondValidated) cCall.ByteArgsNum -= cCall.ArgsNumCondAdder;
                    cCall.ArgsStackDepthMax = 1;   // To Force Use of Arguments on all related operations
                }

                // S6x Routine with less information than Call
                if (s6xRoutine.InputArguments == null && cCall.Arguments != null)
                {
                    compRoutine = new S6xRoutine(cCall, null);
                    s6xRoutine.InputArguments = compRoutine.InputArguments;
                    compRoutine = null;
                    for (int iPos = 0; iPos < cCall.Arguments.Length; iPos++)
                    {
                        cCall.Arguments[iPos].S6xRoutineInputArgument = s6xRoutine.InputArguments[iPos];
                    }
                }
                // S6x Routine with more information than Call
                else if (s6xRoutine.InputArguments != null)
                {
                    callArgs = new CallArgument[s6xRoutine.InputArguments.Length];
                    if (cCall.Arguments != null)
                    {
                        for (int iArg = 0; iArg < cCall.Arguments.Length; iArg++)
                        {
                            if (iArg < callArgs.Length) callArgs[iArg] = cCall.Arguments[iArg].Clone();
                        }
                    }
                    for (int iArg = 0; iArg < s6xRoutine.InputArguments.Length; iArg++)
                    {
                        if (callArgs[iArg] == null) callArgs[iArg] = new CallArgument();
                        callArgs[iArg].S6xRoutineInputArgument = s6xRoutine.InputArguments[iArg];
                        callArgs[iArg].StackDepth = 1;
                        callArgs[iArg].Word = s6xRoutine.InputArguments[iArg].Word;
                        callArgs[iArg].Mode = (CallArgsMode)s6xRoutine.InputArguments[iArg].Encryption;
                    }
                    cCall.Arguments = callArgs;

                    // Other Routine Creation when not Existing
                    if (cCall.Arguments.Length > 0)
                    {
                        if (!Calibration.slRoutines.ContainsKey(cCall.UniqueAddress))
                        {
                            cCall.isRoutine = true;
                            rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
                            rRoutine.Type = RoutineType.Other;
                            Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);
                            rRoutine = null;
                        }
                    }
                }
            }
            else if (s6xRoutine.InputArguments != null && cCall.Arguments != null && s6xRoutine.ByteArgumentsNum == cCall.ByteArgsNum)
            {
                if (s6xRoutine.InputArguments.Length == cCall.Arguments.Length)
                {
                    for (int iPos = 0; iPos < cCall.Arguments.Length; iPos++)
                    {
                        cCall.Arguments[iPos].S6xRoutineInputArgument = s6xRoutine.InputArguments[iPos];
                        cCall.Arguments[iPos].Mode = (CallArgsMode)cCall.Arguments[iPos].S6xRoutineInputArgument.Encryption;
                    }
                }
            }

            // Routine Override
            totalInputsNum = 0;
            totalInputsNum += s6xRoutine.InputStructuresNum;
            totalInputsNum += s6xRoutine.InputTablesNum;
            totalInputsNum += s6xRoutine.InputFunctionsNum;
            totalInputsNum += s6xRoutine.InputScalarsNum;

            totalAdvancedNum = 0;
            totalAdvancedNum += totalInputsNum;

            rRoutine = (Routine)Calibration.slRoutines[cCall.UniqueAddress];

            if (rRoutine != null || totalAdvancedNum > 0)
            {
                cCall.isRoutine = true;
                if (rRoutine == null)
                {
                    rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
                    Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);
                }
                else
                {
                    // Routine vs S6xRoutine Compatibility check
                    compRoutine = new S6xRoutine(cCall, rRoutine);
                    compatibleRoutine = true;
                    compatibleRoutine &= compRoutine.InputScalarsNum == s6xRoutine.InputScalarsNum;
                    compatibleRoutine &= compRoutine.InputFunctionsNum == s6xRoutine.InputFunctionsNum;
                    compatibleRoutine &= compRoutine.InputTablesNum == s6xRoutine.InputTablesNum;
                    compatibleRoutine &= compRoutine.InputStructuresNum == s6xRoutine.InputStructuresNum;
                    if (!compatibleRoutine)
                    {
                        s6xRoutine.InputScalars = compRoutine.InputScalars;
                        s6xRoutine.InputFunctions = compRoutine.InputFunctions;
                        s6xRoutine.InputTables = compRoutine.InputTables;
                        s6xRoutine.InputStructures = compRoutine.InputStructures;

                        totalInputsNum = 0;
                        totalInputsNum += s6xRoutine.InputStructuresNum;
                        totalInputsNum += s6xRoutine.InputTablesNum;
                        totalInputsNum += s6xRoutine.InputFunctionsNum;
                        totalInputsNum += s6xRoutine.InputScalarsNum;

                        totalAdvancedNum = 0;
                        totalAdvancedNum += totalInputsNum;
                    }
                    compRoutine = null;
                }

                rRoutine.S6xRoutine = s6xRoutine;

                // Routine Type Definition
                if (s6xRoutine.InputTablesNum == 1 && totalAdvancedNum == 1)
                //  Standard Table Routine Information
                {
                    s6xInputTable = s6xRoutine.InputTables[0];

                    // Find or Create related RoutineIOTable
                    ioTable = null;
                    if (rRoutine.IOs != null)
                    {
                        foreach (RoutineIO ioIO in rRoutine.IOs)
                        {
                            if (ioIO.GetType() == typeof(RoutineIOTable))
                            {
                                ioTable = (RoutineIOTable)ioIO;
                                break;
                            }
                        }
                    }
                    if (ioTable == null) ioTable = new RoutineIOTable();
                    ioTable.S6xInputTable = s6xInputTable;

                    rRoutine.IOs = new RoutineIO[] { ioTable };

                    // Defined Type Overrides Routine Type
                    if (s6xInputTable.WordOutput) rRoutine.Type = RoutineType.TableWord;
                    else rRoutine.Type = RoutineType.TableByte;

                    // Word
                    ioTable.TableWord = s6xInputTable.WordOutput;
                    // Address Register
                    sRegister = Tools.VariableRegisterAddressHex(s6xInputTable.VariableAddress);
                    if (sRegister != string.Empty) ioTable.AddressRegister = sRegister;
                    // Colums Number Register
                    sRegister = Tools.VariableRegisterAddressHex(s6xInputTable.VariableColsNumberReg);
                    if (sRegister != string.Empty) ioTable.TableColNumberRegister = sRegister;
                    // Colums Input Register
                    sRegister = Tools.VariableRegisterAddressHex(s6xInputTable.VariableColsReg);
                    if (sRegister != string.Empty) ioTable.TableColRegister = sRegister;
                    // Rows Input Register
                    sRegister = Tools.VariableRegisterAddressHex(s6xInputTable.VariableRowsReg);
                    if (sRegister != string.Empty) ioTable.TableRowRegister = sRegister;
                    // Output Register
                    sRegister = Tools.VariableRegisterAddressHex(s6xInputTable.VariableOutput);
                    if (sRegister != string.Empty) ioTable.OutputRegister = sRegister;
                    ioTable.TableOutputRegisterByte = ioTable.OutputRegister;
                    // Signed Output
                    ioTable.SignedOutput = s6xInputTable.SignedOutput;
                    ioTable.isSignedOutputDefined = true;

                    // Other S6x Specifities will be managed when routine will be used

                    s6xInputTable = null;
                    ioTable = null;
                }
                else if (s6xRoutine.InputFunctionsNum == 1 && totalAdvancedNum == 1)
                //  Standard Function Routine
                {
                    s6xInputFunction = s6xRoutine.InputFunctions[0];

                    // Find or Create related RoutineIOFunction
                    ioFunction = null;
                    if (rRoutine.IOs != null)
                    {
                        foreach (RoutineIO ioIO in rRoutine.IOs)
                        {
                            if (ioIO.GetType() == typeof(RoutineIOFunction))
                            {
                                ioFunction = (RoutineIOFunction)ioIO;
                                break;
                            }
                        }
                    }
                    if (ioFunction == null) ioFunction = new RoutineIOFunction();
                    ioFunction.S6xInputFunction = s6xInputFunction;

                    rRoutine.IOs = new RoutineIO[] { ioFunction };

                    // Defined Type Overrides Routine Type
                    if (s6xInputFunction.ByteInput) rRoutine.Type = RoutineType.FunctionByte;
                    else rRoutine.Type = RoutineType.FunctionWord;

                    // Byte
                    ioFunction.FunctionByte = s6xInputFunction.ByteInput;
                    // Address Register
                    sRegister = Tools.VariableRegisterAddressHex(s6xInputFunction.VariableAddress);
                    if (sRegister != string.Empty) ioFunction.AddressRegister = sRegister;
                    // Input Register
                    sRegister = Tools.VariableRegisterAddressHex(s6xInputFunction.VariableInput);
                    if (sRegister != string.Empty) ioFunction.FunctionInputRegister = sRegister;
                    // Output Register
                    sRegister = Tools.VariableRegisterAddressHex(s6xInputFunction.VariableOutput);
                    if (sRegister != string.Empty) ioFunction.OutputRegister = sRegister;
                    // Signed Input
                    ioFunction.FunctionSignedInput = s6xInputFunction.SignedInput;
                    ioFunction.isFunctionSignedInputDefined = true;
                    // Signed Output
                    ioFunction.SignedOutput = s6xInputFunction.SignedOutput;
                    ioFunction.isSignedOutputDefined = true;

                    // Other S6x Specifities will be managed when routine will be used

                    s6xInputFunction = null;
                    ioFunction = null;
                }
                else
                {
                    rRoutine.Type = RoutineType.Other;
                    rRoutine.IOs = null;

                    if (totalInputsNum > 0)
                    {
                        iInput = 0;
                        rRoutine.IOs = new RoutineIO[totalInputsNum];
                        if (s6xRoutine.InputStructuresNum > 0)
                        {
                            foreach (S6xRoutineInputStructure s6xInput in s6xRoutine.InputStructures)
                            {
                                rRoutine.IOs[iInput] = new RoutineIOStructure();

                                ((RoutineIOStructure)rRoutine.IOs[iInput]).S6xInputStructure = s6xInput;

                                // Address Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableAddress);
                                if (sRegister != string.Empty) rRoutine.IOs[iInput].AddressRegister = sRegister;
                                // Number Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableNumber);
                                if (sRegister != string.Empty) ((RoutineIOStructure)rRoutine.IOs[iInput]).StructureNumberRegister = sRegister;

                                iInput++;
                            }
                        }
                        if (s6xRoutine.InputTablesNum > 0)
                        {
                            foreach (S6xRoutineInputTable s6xInput in s6xRoutine.InputTables)
                            {
                                rRoutine.IOs[iInput] = new RoutineIOTable();

                                ((RoutineIOTable)rRoutine.IOs[iInput]).S6xInputTable = s6xInput;

                                // Word
                                ((RoutineIOTable)rRoutine.IOs[iInput]).TableWord = s6xInput.WordOutput;
                                // Address Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableAddress);
                                if (sRegister != string.Empty) rRoutine.IOs[iInput].AddressRegister = sRegister;
                                // Colums Number Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableColsNumberReg);
                                if (sRegister != string.Empty) ((RoutineIOTable)rRoutine.IOs[iInput]).TableColNumberRegister = sRegister;
                                // Colums Input Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableColsReg);
                                if (sRegister != string.Empty) ((RoutineIOTable)rRoutine.IOs[iInput]).TableColRegister = sRegister;
                                // Rows Input Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableRowsReg);
                                if (sRegister != string.Empty) ((RoutineIOTable)rRoutine.IOs[iInput]).TableRowRegister = sRegister;
                                // Output Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableOutput);
                                if (sRegister != string.Empty) rRoutine.IOs[iInput].OutputRegister = sRegister;
                                ((RoutineIOTable)rRoutine.IOs[iInput]).TableOutputRegisterByte = rRoutine.IOs[iInput].OutputRegister;
                                // Signed Output
                                rRoutine.IOs[iInput].SignedOutput = s6xInput.SignedOutput;
                                rRoutine.IOs[iInput].isSignedOutputDefined = true;

                                iInput++;
                            }
                        }
                        if (s6xRoutine.InputFunctionsNum > 0)
                        {
                            foreach (S6xRoutineInputFunction s6xInput in s6xRoutine.InputFunctions)
                            {
                                rRoutine.IOs[iInput] = new RoutineIOFunction();

                                ((RoutineIOFunction)rRoutine.IOs[iInput]).S6xInputFunction = s6xInput;

                                // Byte
                                ((RoutineIOFunction)rRoutine.IOs[iInput]).FunctionByte = s6xInput.ByteInput;
                                // Address Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableAddress);
                                if (sRegister != string.Empty) rRoutine.IOs[iInput].AddressRegister = sRegister;
                                // Input Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableInput);
                                if (sRegister != string.Empty) ((RoutineIOFunction)rRoutine.IOs[iInput]).FunctionInputRegister = sRegister;
                                // Output Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableOutput);
                                if (sRegister != string.Empty) rRoutine.IOs[iInput].OutputRegister = sRegister;
                                // Signed Input
                                ((RoutineIOFunction)rRoutine.IOs[iInput]).FunctionSignedInput = s6xInput.SignedInput;
                                ((RoutineIOFunction)rRoutine.IOs[iInput]).isFunctionSignedInputDefined = true;
                                // Signed Output
                                rRoutine.IOs[iInput].SignedOutput = s6xInput.SignedOutput;
                                rRoutine.IOs[iInput].isSignedOutputDefined = true;

                                iInput++;
                            }
                        }
                        if (s6xRoutine.InputScalarsNum > 0)
                        {
                            foreach (S6xRoutineInputScalar s6xInput in s6xRoutine.InputScalars)
                            {
                                rRoutine.IOs[iInput] = new RoutineIOScalar();

                                ((RoutineIOScalar)rRoutine.IOs[iInput]).S6xInputScalar = s6xInput;

                                // Address Register
                                sRegister = Tools.VariableRegisterAddressHex(s6xInput.VariableAddress);
                                if (sRegister != string.Empty) rRoutine.IOs[iInput].AddressRegister = sRegister;

                                // Byte
                                ((RoutineIOScalar)rRoutine.IOs[iInput]).ScalarByte = s6xInput.Byte;
                                // Signed
                                ((RoutineIOScalar)rRoutine.IOs[iInput]).ScalarSigned = s6xInput.Signed;

                                iInput++;
                            }
                        }
                    }

                    // Other S6x Specifities will be managed when routine will be used
                }
            }

            rRoutine = null;
            s6xRoutine = null;
        }

        // Init Call Identification for Constant Registers
        private void identifyInitCall(ref Call cCall, ref SADS6x S6x)
        {
            Operation[] ops = null;
            RConst rConst = null;
            string[] matchingOriginalOps = null;
            int opsMatchIndex = -1;
            int currConstCode = -1;
            int prevConstCode = -1;
            string variableReg = string.Empty;
            int variableValue = 0;
            ArrayList alAddresses = new ArrayList();
            Routine rRoutine = null;
            bool alreadyFoundInitRoutine = false;

            // Already processed
            if (cCall.isIdentified) return;

            // Only one Init Routine
            foreach (Routine roRo in Calibration.slRoutines.Values)
            {
                if (roRo.Code == RoutineCode.Init)
                {
                    alreadyFoundInitRoutine = true;
                    break;
                }
            }
            if (alreadyFoundInitRoutine) return;
            
            // 8065 and late 8061
            ops = getCallOps(ref cCall, 48, 99, true, true, false, false, false, false, false, false);
            matchingOriginalOps = new string[] { "a1,..,..,..", "a1,..,..,..", "a1,..,..,..", "a1,..,..,.." };
            //matchingOriginalOps = new string[] { "11,1b", "a1,..,..,.." };
            opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
            if (opsMatchIndex >= 0)
            {
                for (int iPos = opsMatchIndex; iPos < ops.Length; iPos++)
                {
                    if (variableReg != string.Empty)
                    {
                        if (ops[iPos].OriginalOpArr[0] == "a1" && ops[iPos].OriginalOpArr[3] == variableReg)
                        {
                            alAddresses.Add(Convert.ToInt32(ops[iPos].OriginalOpArr[2] + ops[iPos].OriginalOpArr[1], 16) - SADDef.EecBankStartAddress);
                        }
                        else if (ops[iPos].OriginalOpArr[0] == "65" && ops[iPos].OriginalOpArr[3] == variableReg)
                        {
                            variableValue +=  Convert.ToInt32(ops[iPos].OriginalOpArr[2] + ops[iPos].OriginalOpArr[1], 16);
                        }
                        else if (ops[iPos].OriginalOpArr[0] == "c0" && ops[iPos].OriginalOpArr[2] == variableReg)
                        {
                            rConst.Code = ops[iPos].OriginalOpArr[1];
                            rConst.Addresses = (int[])alAddresses.ToArray(typeof(int));
                            if (!Calibration.slRconst.ContainsKey(rConst.Code))
                            {
                                Calibration.slRconst.Add(rConst.Code, rConst);
                                ((Register)Calibration.slRegisters[Tools.RegisterUniqueAddress(rConst.Code)]).RConst = rConst;
                            }
                            rConst = null;

                            break;
                        }
                        else if (ops[iPos].OriginalOpArr[0] == "a0" && ops[iPos].OriginalOpArr[1] == variableReg)
                        {
                            rConst.Code = ops[iPos].OriginalOpArr[2];
                            rConst.ValueInt = variableValue;
                            if (!Calibration.slRconst.ContainsKey(rConst.Code))
                            {
                                Calibration.slRconst.Add(rConst.Code, rConst);
                                ((Register)Calibration.slRegisters[Tools.RegisterUniqueAddress(rConst.Code)]).RConst = rConst;
                            }
                            rConst = null;

                            break;
                        }
                    }
                    else
                    {
                        if (ops[iPos].OriginalOpArr[0] != "a1") break;

                        // Const are filled in order and are word register with codes separed by 2
                        currConstCode = Convert.ToInt32(ops[iPos].OriginalOpArr[3], 16);
                        if (prevConstCode != -1 && Math.Abs(currConstCode - prevConstCode) != 2)
                        {
                            variableReg = ops[iPos].OriginalOpArr[3];
                            rConst = new RConst("TMP", -1);
                            rConst.AddressBankNum = Calibration.BankNum;
                            variableValue = Convert.ToInt32(ops[iPos].OriginalOpArr[2] + ops[iPos].OriginalOpArr[1], 16);
                            alAddresses.Add(variableValue - SADDef.EecBankStartAddress);
                        }
                        else
                        {
                            prevConstCode = currConstCode;

                            rConst = new RConst(ops[iPos].OriginalOpArr[3], Convert.ToInt32(ops[iPos].OriginalOpArr[2] + ops[iPos].OriginalOpArr[1], 16));
                            if (!Calibration.slRconst.ContainsKey(rConst.Code))
                            {
                                Calibration.slRconst.Add(rConst.Code, rConst);
                                ((Register)Calibration.slRegisters[Tools.RegisterUniqueAddress(rConst.Code)]).RConst = rConst;
                            }
                            rConst = null;
                        }
                    }
                }
                if (Calibration.slRconst.Count > 0)
                {
                    cCall.isRoutine = true;
                    Calibration.slCalls[cCall.UniqueAddress] = cCall;

                    rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
                    rRoutine.Code = RoutineCode.Init;
                    rRoutine.SetTranslationComments();
                    foreach (RConst rConstCom in Calibration.slRconst.Values) rRoutine.Comments += "\r\n" + Tools.PointerTranslation(rConstCom.Code) + " = " + rConstCom.Value;
                    if (!Calibration.slRoutines.ContainsKey(rRoutine.UniqueAddress)) Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);

                    // 20200512 - PYM
                    // Added to S6xRegisters
                    foreach (RConst rConstCpy in Calibration.slRconst.Values)
                    {
                        S6xRegister s6xReg = (S6xRegister)S6x.slProcessRegisters[Tools.RegisterUniqueAddress(rConstCpy.Code)];
                        if (s6xReg == null)
                        {
                            s6xReg = new S6xRegister(rConstCpy.Code);
                            s6xReg.Label = Tools.RegisterInstruction(rConstCpy.Code);
                            s6xReg.Comments = "Constant register (RConst) " + s6xReg.Label;
                            S6x.slProcessRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                            if (S6x.slRegisters.ContainsKey(s6xReg.UniqueAddress)) S6x.slRegisters[s6xReg.UniqueAddress] = s6xReg;
                            else S6x.slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                        }
                        s6xReg.isRBase = false;
                        s6xReg.isRConst = true;
                        s6xReg.ConstValue = rConstCpy.Value;
                        s6xReg.AutoConstValue = true;
                    }
                }
                alAddresses = null;
            }
            ops = null;

            if (rRoutine == null) return;

            // Updating Main Call Operations using RConst
            ops = getCallOps(ref cCall, 128, 0, false, true, false, false, false, false, false, false);
            for (int iPos = 0; iPos < ops.Length; iPos++)
            {
                if (ops[iPos] == null) continue;
                SADOPCode opCode = (SADOPCode)Calibration.slOPCodes[ops[iPos].OriginalOPCode];
                if (opCode == null) continue;

                opCode.postProcessOpRConst(ref ops[iPos], ref Calibration, ref S6x);
            }
            ops = null;

            rRoutine = null;
        }

        // Call with Args Identification
        private void identifyCallArgsType(ref Call cCall)
        {
            Call srcArgsCall = null;
            Operation[] ops = null;
            Operation ope = null;
            string[] matchingOriginalOps = null;
            int opsMatchIndex = -1;
            int opsMatchIndexAdder = -1;

            string argsReg = string.Empty;
            int argsStackDepth = -1;
            int argsStackDepthReducer = -1;
            int argsCount = -1;
            int argsCondAdder = -1;
            int argsCondValue = -1;
            ArrayList alArgs = null;
            ArrayList alArgsDetect = null;
            ArrayList alArgsCond = null;
            bool foundArgsDetect = false;
            string variableExternalLoopFirstAddress = string.Empty;
            string variableExternalRegister = string.Empty;

            if (cCall.isVector || cCall.isIntVector) return;

            // Args Calls Identification
            if (cCall.ArgsType != CallArgsType.Unknown) return;

            // Identification based on Related Existing Args Call
            alArgs = new ArrayList();
            ops = getFollowingOPs(cCall.AddressInt, 8, 99, true, true, false, false, false, true, true, true);
            foreach (Operation callOpe in ops)
            {
                if (callOpe == null) break;
                
                if (callOpe.isReturn) break;                                                        // Ends the search
                //if (callOpe.CallType == CallType.Jump) break;                                     // Ends the search
                if (!is8061 && !isPilot && callOpe.OriginalOPCode.ToLower() == "f3")                // Pop(PSW) Ends the search
                if ((is8061 || isPilot) && callOpe.OriginalOPCode.ToLower() == "cc")                // Pop Ends the search
                srcArgsCall = null;
                switch (callOpe.CallType)
                {
                    case CallType.Call:
                    case CallType.ShortCall:
                    case CallType.Jump:
                    case CallType.ShortJump:
                    case CallType.Goto:
                    case CallType.Skip:
                        if (Calibration.alArgsCallsUniqueAddresses.Contains(Tools.UniqueAddress(callOpe.ApplyOnBankNum, callOpe.AddressJumpInt)))
                        {
                            srcArgsCall = (Call)Calibration.slCalls[Tools.UniqueAddress(callOpe.ApplyOnBankNum, callOpe.AddressJumpInt)];
                        }
                        break;
                    case CallType.Unknown:
                        if (Calibration.alArgsCallsUniqueAddresses.Contains(Tools.UniqueAddress(callOpe.BankNum, callOpe.AddressNextInt)))
                        {
                            srcArgsCall = (Call)Calibration.slCalls[Tools.UniqueAddress(callOpe.BankNum, callOpe.AddressNextInt)];
                        }
                        break;
                }
                if (srcArgsCall != null)
                {
                    if (srcArgsCall.ArgsType != CallArgsType.None && srcArgsCall.ArgsType != CallArgsType.Unknown)
                    {
                        switch (callOpe.CallType)
                        {
                            case CallType.Call:
                            case CallType.ShortCall:
                                if (srcArgsCall.ArgsStackDepthMax > 0)
                                {
                                    if (srcArgsCall.ArgsStackDepthMax == 1 && srcArgsCall.ArgsType == CallArgsType.Variable && callOpe.CallArgsNum == 1)
                                    {
                                        cCall.ArgsStackDepthMax = 1;       // Next Ope will Provide Args
                                        cCall.ArgsType = CallArgsType.Fixed;
                                        //cCall.ArgsModes = srcArgsCall.ArgsModes;
                                        cCall.ArgsModes = null;         // Arg Mode should be analysed
                                        // VariableOutputFirstRegisterAddress Information to be manage Args Sizes and Modes
                                        cCall.ArgsVariableOutputFirstRegisterAddress = srcArgsCall.ArgsVariableOutputFirstRegisterAddress;
                                        cCall.ByteArgsNum += Convert.ToInt32(callOpe.CallArgsArr[0], 16);

                                        // Created by default Word by Word, will be updated if not compatible in Args Mode detection
                                        CallArgument callArg = null;
                                        for (int iArg = 0; iArg < Convert.ToInt32(callOpe.CallArgsArr[0], 16); iArg++)
                                        {
                                            if (iArg % 2 == 0)
                                            {
                                                callArg = new CallArgument();
                                                callArg.StackDepth = 1;
                                                if (cCall.ArgsVariableOutputFirstRegisterAddress >= 0)
                                                {
                                                    callArg.OutputRegisterAddressInt = cCall.ArgsVariableOutputFirstRegisterAddress + iArg;
                                                }
                                            }
                                            else if (callArg != null)
                                            {
                                                callArg.Word = true;
                                                alArgs.Add(callArg);
                                                callArg = null;
                                            }
                                        }
                                        if (callArg != null)
                                        {
                                            alArgs.Add(callArg);
                                            callArg = null;
                                        }

                                        // Last Loop on all available calls adding Args
                                        // Variable Args Calls includes a Push, not other Arg can be added
                                        srcArgsCall = null;                             // To Generated a Break Condition

                                        // Args are Directly added
                                        if (alArgs.Count > 0)
                                        {
                                            cCall.Arguments = new CallArgument[alArgs.Count];
                                            alArgs.CopyTo(cCall.Arguments);
                                            alArgs.Clear();
                                        }
                                        // Args modes to be identified
                                        Operation[] adjacentOps = getFollowingOPs(callOpe.AddressNextInt, 32, 1, true, false, false, false, false, false, false, false);
                                        identifyCallArgsMode(ref cCall, ref adjacentOps, 0, false);
                                        adjacentOps = null;
                                    }
                                    else
                                    {
                                        if (cCall.ArgsStackDepthMax != 1) cCall.ArgsStackDepthMax = srcArgsCall.ArgsStackDepthMax - 1;
                                        cCall.ArgsType = srcArgsCall.ArgsType;
                                        cCall.ArgsModes = srcArgsCall.ArgsModes;
                                        //cCall.ByteArgsNum += srcArgsCall.ByteArgsNum;       // Loop on all available calls adding Args
                                        if (srcArgsCall.Arguments != null)
                                        {
                                            foreach (CallArgument cArg in srcArgsCall.Arguments)
                                            {
                                                CallArgument cpyArg = cArg.Clone();
                                                // This is a Call, StackDepth is reduced
                                                if (cpyArg.StackDepth > 0) cpyArg.StackDepth--;
                                                if (cpyArg.StackDepth > 0)
                                                {
                                                    if (cpyArg.StackDepth == 1)
                                                    {
                                                        cCall.ByteArgsNum++;
                                                        if (cpyArg.Word) cCall.ByteArgsNum++;
                                                    }
                                                    alArgs.Add(cpyArg);
                                                }
                                            }
                                        }
                                    }
                                    if (ope != null && (is8061 || isPilot))
                                    // On 8061 and 8065 Pilot Pop before Call increases Stack Depth
                                    {
                                        if (ope.OriginalInstruction.ToLower() == "pop") cCall.ArgsStackDepthMax++;
                                    }
                                    Calibration.alArgsCallsUniqueAddresses.Add(cCall.UniqueAddress);
                                }
                                break;
                            default:
                                cCall.ArgsStackDepthMax = srcArgsCall.ArgsStackDepthMax;
                                cCall.ArgsType = srcArgsCall.ArgsType;
                                cCall.ArgsModes = srcArgsCall.ArgsModes;
                                //cCall.ByteArgsNum += srcArgsCall.ByteArgsNum;       // Add Args Num of related Call Ope and Exit, this is the last possible Added Value
                                if (srcArgsCall.Arguments != null)
                                {
                                    foreach (CallArgument cArg in srcArgsCall.Arguments)
                                    {
                                        //alArgs.Add(cArg);
                                        CallArgument cpyArg = cArg.Clone();
                                        if (cpyArg.StackDepth > 0)
                                        {
                                            if (cpyArg.StackDepth == 1)
                                            {
                                                cCall.ByteArgsNum++;
                                                if (cpyArg.Word) cCall.ByteArgsNum++;
                                            }
                                            alArgs.Add(cpyArg);
                                        }
                                    }
                                }
                                Calibration.alArgsCallsUniqueAddresses.Add(cCall.UniqueAddress);

                                // Conditionnal Args Num
                                if (srcArgsCall.ArgsType == CallArgsType.FixedCyCond)
                                {
                                    cCall.ArgsNumCondAdder = srcArgsCall.ArgsNumCondAdder;
                                    cCall.ArgsCondValue = srcArgsCall.ArgsCondValue;
                                    if (ope != null)
                                    {
                                        if (ope.OriginalOPCode.ToLower() == "f8")   // clc CY = 0;
                                        {
                                            cCall.ArgsCondValidated = (cCall.ArgsCondValue == 0);
                                        }
                                        else if (ope.OriginalOPCode.ToLower() == "f9")   // stc CY = 1;
                                        {
                                            cCall.ArgsCondValidated = (cCall.ArgsCondValue == 1);
                                        }
                                        else
                                        {
                                            // By Default uses the Source Call Result
                                            cCall.ArgsCondValidated = srcArgsCall.ArgsCondValidated;
                                        }
                                    }
                                }

                                srcArgsCall = null;                             // To Generated a Break Condition
                                break;
                        }
                        ops = null;
                        ope = null;

                        // Used as break condition
                        if (srcArgsCall == null) break;

                        srcArgsCall = null;
                    }
                }
                // 8061 double Pop mngt, we keep previous callOpe to analyse it
                ope = callOpe;
            }
            ope = null;
            ops = null;
            // Return if Call is identified
            if (cCall.ArgsType != CallArgsType.Unknown)
            {
                if (alArgs.Count > 0)
                {
                    cCall.Arguments = new CallArgument[alArgs.Count];
                    alArgs.CopyTo(cCall.Arguments);
                }
                alArgs = null;
                return;
            }

            // Signature Identification for Root Identification

            if (is8061 || isEarly || isPilot)
            // 8061 & Early/Pilot 8065 Args Mode base on Pop
            {
                ops = getCallOps(ref cCall, 16, 99, true, true, false, false, false, true, true, true);

                // Variable Num
                // To be processed before Fixed Num to prevent confusion
                // cc,3c             pop   R3c            R3c = pop();
                // b2,3d,3a          ldb   R3a,[R3c++]    R3a = [R3c++];
                // cc,42             pop   R42            R42 = pop();
                if (is8061 || isPilot) matchingOriginalOps = new string[] { "cc,..", "b2,..,..", "cc,.." };
                else matchingOriginalOps = new string[] { "cc,..", "10,08", "b2,..,..", "cc,.." };
                opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                if (opsMatchIndex >= 0)
                {
                    argsStackDepth = 0;
                    // Because of Signature, other pop before Match Index could add Depth, we search for them
                    for (int iPos = opsMatchIndex - 1; iPos >= 0; iPos--)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        switch (ope.OriginalOPCode.ToLower())
                        {
                            case "cc":
                                argsStackDepth++;   // Each pop adds a Stack Depth
                                break;
                            default:
                                ope = null;         // Break Condition
                                break;
                        }
                        if (ope == null) break;
                    }
                    ope = null;

                    cCall.ArgsStackDepthMax = argsStackDepth + 1;       // First pop, second pop is for its caller and the variable number
                    cCall.ArgsType = CallArgsType.Variable;
                    cCall.ByteArgsNum = -1;
                    cCall.Arguments = new CallArgument[1];
                    cCall.Arguments[0] = new CallArgument();
                    cCall.Arguments[0].StackDepth = cCall.ArgsStackDepthMax;
                    cCall.Arguments[0].Word = false;
                    // Args Mode Defaulted
                    cCall.Arguments[0].Mode = CallArgsMode.Standard;
                    // Args Mode Defaulted
                    cCall.ArgsModes = new CallArgsMode[] { CallArgsMode.Standard };

                    // Because of Signature, First output register is before opsMatchIndex, we are searching for it
                    for (int iPos = opsMatchIndex - 1; iPos >= 0; iPos--)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        switch (ope.OriginalOPCode.ToLower())
                        {
                            case "a1":
                                // First output register address
                                if (cCall.ArgsVariableOutputFirstRegisterAddress == -1)
                                {
                                    cCall.ArgsVariableOutputFirstRegisterAddress = Convert.ToInt32(ope.OperationParams[0].InstructedParam, 16);
                                    ope = null;
                                }
                                break;
                        }
                        if (ope == null) break;
                    }
                    ope = null;
                    
                    Calibration.alArgsCallsUniqueAddresses.Add(cCall.UniqueAddress);
                    ops = null;
                    return;
                }

                // Variable Num with External Register
                // cc,3a                pop   R3a                R3a = pop();         
                // 71,bf,da             an2b  Rda,bf             Rda &= bf;           
                // ae,3b,14             ldzbw R14,[R3a++]        R14 = (uns)[R3a++];                First Loop, first Arg
                // b2,3b,17             ldb   R17,[R3a++]        R17 = [R3a++];                     First Loop, second Arg
                // e0,39,03             djnz  R39,3885           R39--; if (R39 !=  0) goto 3885;   Another Parameter to check if Args should be managed is Sub Call
                // 91,40,da             orrb  Rda,40             Rda |= 40;           
                // 28,10                scall 3897               Sub0037();           
                // e0,38,ef             djnz  R38,3879           R38--; if (R38 !=  0) goto 3879;   Main External Register Loop for Count
                // c8,3a                push  R3a                push(R3a);                         End of Args use
                if (is8061 || isPilot) matchingOriginalOps = new string[] { "cc,..", "71,..,..", "b2,..,.." };
                else matchingOriginalOps = new string[] { "cc,..", "71,..,..", "10,08", "b2,..,.." };
                opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                if (opsMatchIndex < 0)
                {
                    if (is8061 || isPilot) matchingOriginalOps = new string[] { "cc,..", "71,..,..", "ae,..,.." };
                    else matchingOriginalOps = new string[] { "cc,..", "71,..,..", "10,08", "ae,..,.." };
                    opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                }
                if (opsMatchIndex < 0)
                {
                    if (is8061 || isPilot) matchingOriginalOps = new string[] { "cc,..", "71,..,..", "be,..,.." };
                    else matchingOriginalOps = new string[] { "cc,..", "71,..,..", "10,08", "be,..,.." };
                    opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                }
                if (opsMatchIndex >= 0)
                {
                    // Because of Signature, other pop before Match Index could add Depth, we search for them
                    for (int iPos = opsMatchIndex - 1; iPos >= 0; iPos--)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        if (ope.OriginalOPCode.ToLower() != "cc") break;
                        opsMatchIndex--;
                    }
                    ope = null;

                    argsStackDepth = 0;
                    argsCount = 0;
                    alArgs = new ArrayList();
                    alArgsDetect = new ArrayList();
                    variableExternalLoopFirstAddress = string.Empty;
                    variableExternalRegister = string.Empty;

                    for (int iPos = opsMatchIndex; iPos < ops.Length; iPos++)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        switch (ope.OriginalOPCode.ToLower())
                        {
                            case "e0":
                                if (ope.OperationParams.Length == 2)
                                {
                                    if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == variableExternalLoopFirstAddress)
                                    {
                                        variableExternalRegister = ope.OriginalOpArr[1];
                                    }
                                }
                                break;
                            case "cc":
                                // Each pop adds a Stack Depth for CallArgumentDetection
                                argsStackDepth++;
                                alArgsDetect.Add(new CallArgumentDetection(argsStackDepth, ope.OriginalOpArr[ope.OriginalOpArr.Length - 1]));
                                break;
                            case "b2":  // ldb   RXX,[RYY++]
                            case "be":  // ldsbw RXX,[RYY++]
                            case "ae":  // ldzbw RXX,[RYY++]
                                foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                                {
                                    if (cArgDetect.StackReadRegister != string.Format("{0:x2}", Convert.ToInt32(ope.OriginalOpArr[1], 16) - 1)) continue;

                                    argsCount++;
                                    cArgDetect.CountModeOn = true;
                                    int newOutputRegisterAddressInt = Convert.ToInt32(ope.OriginalOpArr[ope.OriginalOpArr.Length - 1], 16);
                                    if (cArgDetect.CallArgument != null)
                                    {
                                        if (cArgDetect.CallArgument.OutputRegisterAddressInt + 1 == newOutputRegisterAddressInt)
                                        {
                                            cArgDetect.CallArgument.Word = true;
                                            newOutputRegisterAddressInt = -1;
                                        }
                                        alArgs.Add(cArgDetect.CallArgument);
                                        cArgDetect.CallArgument = null;
                                    }
                                    if (newOutputRegisterAddressInt >= 0) cArgDetect.CreateCallArgument(newOutputRegisterAddressInt);

                                    if (variableExternalLoopFirstAddress == string.Empty) variableExternalLoopFirstAddress = ope.Address;
                                }
                                break;
                            case "c8":  // push(RXX);
                                int iArgsDetectToRemove = -1;
                                for (int iIndex = 0; iIndex < alArgsDetect.Count; iIndex++)
                                {
                                    CallArgumentDetection cArgDetect = (CallArgumentDetection)alArgsDetect[iIndex];
                                    if (!cArgDetect.CountModeOn) continue;
                                    if (cArgDetect.StackReadRegister != ope.OriginalOpArr[1].ToLower()) continue;

                                    // Remaining Args added before removal
                                    if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);

                                    iArgsDetectToRemove = iIndex;
                                    break;
                                }
                                if (iArgsDetectToRemove >= 0) alArgsDetect.RemoveAt(iArgsDetectToRemove);
                                break;
                        }
                        ope = null;
                        if (alArgsDetect.Count == 0) break;
                    }
                    // Remaining Args added before cleanup
                    foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                    {
                        if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);
                    }
                    alArgsDetect = null;
                    if (argsCount > 0 && variableExternalRegister != string.Empty)
                    {
                        cCall.ArgsStackDepthMax = argsStackDepth;
                        cCall.ArgsType = CallArgsType.VariableExternalRegister;
                        cCall.ByteArgsNum = argsCount;
                        cCall.ArgsVariableExternalRegister = variableExternalRegister;
                        if (alArgs.Count > 0)
                        {
                            // Args Mode Defaulted at Standard
                            foreach (CallArgument cArg in alArgs) cArg.Mode = CallArgsMode.Standard;
                            cCall.Arguments = new CallArgument[alArgs.Count];
                            alArgs.CopyTo(cCall.Arguments);
                        }
                        alArgs = null;
                        // Args Mode Defaulted at Standard
                        cCall.ArgsModes = new CallArgsMode[] { CallArgsMode.Standard };
                        Calibration.alArgsCallsUniqueAddresses.Add(cCall.UniqueAddress);
                        ops = null;
                        return;
                    }
                    alArgs = null;
                }

                // CY Conditional Num
                // f9                   stc                      CY = 1;              
                // cc,18                pop   R18                R18 = pop();         
                // b2,19,1a             ldb   R1a,[R18++]        R1a = [R18++];       
                // b2,19,1b             ldb   R1b,[R18++]        R1b = [R18++];       
                // b2,19,1c             ldb   R1c,[R18++]        R1c = [R18++];       
                // b2,19,1d             ldb   R1d,[R18++]        R1d = [R18++];       
                // d3,05                jnc   446f               if ((uns) R1d < [R18++]) goto 446f;
                // c9,fa,40             push  40fa               push(Sub0058);       
                if (is8061 || isPilot) matchingOriginalOps = new string[] { "f9", "cc,..", "b2,..,.." };
                else matchingOriginalOps = new string[] { "f9", "cc,..", "10,08", "b2,..,.." };
                opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                if (opsMatchIndex < 0)
                {
                    if (is8061 || isPilot) matchingOriginalOps = new string[] { "f9", "cc,..", "ae,..,.." };
                    else matchingOriginalOps = new string[] { "f9", "cc,..", "10,08", "ae,..,.." };
                    opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                }
                if (opsMatchIndex < 0)
                {
                    if (is8061 || isPilot) matchingOriginalOps = new string[] { "f9", "cc,..", "be,..,.." };
                    else matchingOriginalOps = new string[] { "f9", "cc,..", "10,08", "be,..,.." };
                    opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                }
                if (opsMatchIndex >= 0)
                {
                    argsStackDepth = 0;
                    argsCount = 0;
                    argsCondAdder = 0;
                    argsCondValue = 0;
                    alArgs = new ArrayList();
                    alArgsDetect = new ArrayList();
                    alArgsCond = new ArrayList();

                    for (int iPos = opsMatchIndex; iPos < ops.Length; iPos++)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        switch (ope.OriginalOPCode.ToLower())
                        {
                            case "cc":
                                // Each pop adds a Stack Depth for CallArgumentDetection
                                argsStackDepth++;
                                alArgsDetect.Add(new CallArgumentDetection(argsStackDepth, ope.OriginalOpArr[ope.OriginalOpArr.Length - 1]));
                                break;
                            case "b2":  // ldb   RXX,[RYY++]
                            case "be":  // ldsbw RXX,[RYY++]
                            case "ae":  // ldzbw RXX,[RYY++]
                                foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                                {
                                    if (cArgDetect.StackReadRegister != string.Format("{0:x2}", Convert.ToInt32(ope.OriginalOpArr[1], 16) - 1)) continue;

                                    argsCount++;
                                    cArgDetect.CountModeOn = true;
                                    int newOutputRegisterAddressInt = Convert.ToInt32(ope.OriginalOpArr[ope.OriginalOpArr.Length - 1], 16);
                                    if (cArgDetect.CallArgument != null)
                                    {
                                        if (cArgDetect.CallArgument.OutputRegisterAddressInt + 1 == newOutputRegisterAddressInt)
                                        {
                                            cArgDetect.CallArgument.Word = true;
                                            newOutputRegisterAddressInt = -1;
                                        }
                                        alArgs.Add(cArgDetect.CallArgument);
                                        cArgDetect.CallArgument = null;
                                    }
                                    if (newOutputRegisterAddressInt >= 0) cArgDetect.CreateCallArgument(newOutputRegisterAddressInt);
                                }
                                break;
                            case "d3":  // jnc
                            case "db":  // jc
                                if (ope.OriginalOPCode.ToLower() == "d3") argsCondValue = 1;
                                else argsCondValue = 0;
                                // push(SubCall With Args);
                                // Not in Signature but just after Goto
                                foundArgsDetect = false;
                                foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                                {
                                    if (cArgDetect.CountModeOn)
                                    {
                                        foundArgsDetect = true;
                                        break;
                                    }
                                }
                                if (foundArgsDetect)
                                {
                                    ope = (Operation)slOPs[Tools.UniqueAddress(ope.BankNum, ope.AddressNextInt)];
                                    if (ope != null)
                                    {
                                        if (ope.OriginalOPCode.ToLower() == "c9") // push(SubCall With Args);
                                        {
                                            // Push is done on the part without Pop of the call,
                                            // related Call with Args (beginning with Pop) is one Ope higher - 2 Bytes
                                            if (Calibration.alArgsCallsUniqueAddresses.Contains(Tools.UniqueAddress(ope.BankNum, ope.AddressJumpInt - 2)))
                                            {
                                                if (Calibration.slCalls.ContainsKey(Tools.UniqueAddress(ope.BankNum, ope.AddressJumpInt - 2)))
                                                {
                                                    srcArgsCall = (Call)Calibration.slCalls[Tools.UniqueAddress(ope.BankNum, ope.AddressJumpInt - 2)];
                                                    if (srcArgsCall != null)
                                                    {
                                                        argsCondAdder = srcArgsCall.ByteArgsNum;
                                                        if (srcArgsCall.Arguments != null) foreach (CallArgument cArg in srcArgsCall.Arguments) alArgsCond.Add(cArg);
                                                        srcArgsCall = null;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // Remaining Args added before removal and break condition
                                    foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                                    {
                                        if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);
                                    }
                                    alArgsDetect = null;
                                }
                                break;
                        }
                        ope = null;
                        if (alArgsDetect == null) break;
                    }
                    if (alArgsDetect != null)
                    {
                        // Remaining Args added
                        foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                        {
                            if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);
                        }
                        alArgsDetect = null;
                    }
                    if (argsCount > 0)
                    {
                        cCall.ArgsStackDepthMax = argsStackDepth;
                        cCall.ArgsType = CallArgsType.FixedCyCond;
                        cCall.ByteArgsNum = argsCount;
                        cCall.ArgsNumCondAdder = argsCondAdder;
                        cCall.ArgsCondValue = argsCondValue;
                        cCall.ArgsCondValidated = (argsCondValue == 1); //  Because of Signature, CY is at 1
                        if (cCall.ArgsCondValidated) foreach (CallArgument cArg in alArgsCond) alArgs.Add(cArg);
                        alArgsCond = null;
                        if (alArgs.Count > 0)
                        {
                            cCall.Arguments = new CallArgument[alArgs.Count];
                            alArgs.CopyTo(cCall.Arguments);
                        }
                        alArgs = null;
                        // Args Mode Calculation
                        identifyCallArgsMode(ref cCall, ref ops, opsMatchIndex, true);
                        Calibration.alArgsCallsUniqueAddresses.Add(cCall.UniqueAddress);
                        ops = null;
                        return;
                    }
                    alArgs = null;
                }

                // Fixed Num
                // cc,18        pop   R18            R18 = pop();
                // ae,19,14     ldzbw R14,[R18++]    R14 = (uns)[R18++];
                // b2,19,17     ldb   R17,[R18++]    R17 = [R18++];
                if (is8061 || isPilot) matchingOriginalOps = new string[] { "cc,..", "b2,..,.." };
                else matchingOriginalOps = new string[] { "cc,..", "10,08", "b2,..,.." };
                opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                if (opsMatchIndex < 0)
                {
                    if (is8061 || isPilot) matchingOriginalOps = new string[] { "cc,..", "ae,..,.." };
                    else matchingOriginalOps = new string[] { "cc,..", "10,08", "ae,..,.." };
                    opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                }
                if (opsMatchIndex < 0)
                {
                    if (is8061 || isPilot) matchingOriginalOps = new string[] { "cc,..", "be,..,.." };
                    else matchingOriginalOps = new string[] { "cc,..", "10,08", "be,..,.." };
                    opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                }
                if (opsMatchIndex >= 0)
                {
                    // Because of Signature, other pop before Match Index could add Depth, we search for them
                    for (int iPos = opsMatchIndex - 1; iPos >= 0; iPos--)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        if (ope.OriginalOPCode.ToLower() != "cc") break;
                        opsMatchIndex--;
                    }
                    ope = null;

                    argsStackDepth = 0;
                    argsCount = 0;
                    alArgs = new ArrayList();
                    alArgsDetect = new ArrayList();
                    for (int iPos = opsMatchIndex; iPos < ops.Length; iPos++)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        switch (ope.OriginalOPCode.ToLower())
                        {
                            case "cc":
                                // Each pop adds a Stack Depth for CallArgumentDetection
                                argsStackDepth++;
                                alArgsDetect.Add(new CallArgumentDetection(argsStackDepth, ope.OriginalOpArr[ope.OriginalOpArr.Length - 1]));
                                break;
                            case "b2":  // ldb   RXX,[RYY++]
                            case "be":  // ldsbw RXX,[RYY++]
                            case "ae":  // ldzbw RXX,[RYY++]
                                foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                                {
                                    if (cArgDetect.StackReadRegister != string.Format("{0:x2}", Convert.ToInt32(ope.OriginalOpArr[1], 16) - 1)) continue;

                                    argsCount++;
                                    cArgDetect.CountModeOn = true;
                                    int newOutputRegisterAddressInt = Convert.ToInt32(ope.OriginalOpArr[ope.OriginalOpArr.Length - 1], 16);
                                    if (cArgDetect.CallArgument != null)
                                    {
                                        if (cArgDetect.CallArgument.OutputRegisterAddressInt + 1 == newOutputRegisterAddressInt)
                                        {
                                            cArgDetect.CallArgument.Word = true;
                                            newOutputRegisterAddressInt = -1;
                                        }
                                        alArgs.Add(cArgDetect.CallArgument);
                                        cArgDetect.CallArgument = null;
                                    }
                                    if (newOutputRegisterAddressInt >= 0) cArgDetect.CreateCallArgument(newOutputRegisterAddressInt);
                                }
                                break;
                            case "c8":  // push(RXX);
                                int iArgsDetectToRemove = -1;
                                for (int iIndex = 0; iIndex < alArgsDetect.Count; iIndex++)
                                {
                                    CallArgumentDetection cArgDetect = (CallArgumentDetection)alArgsDetect[iIndex];
                                    if (!cArgDetect.CountModeOn) continue;
                                    if (cArgDetect.StackReadRegister != ope.OriginalOpArr[1].ToLower()) continue;

                                    // Remaining Args added before removal
                                    if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);

                                    iArgsDetectToRemove = iIndex;
                                    break;
                                }
                                if (iArgsDetectToRemove >= 0) alArgsDetect.RemoveAt(iArgsDetectToRemove);
                                break;
                        }
                        ope = null;
                        if (alArgsDetect.Count == 0) break;
                    }
                    // Remaining Args added before cleanup
                    foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                    {
                        if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);
                    }
                    alArgsDetect = null;

                    if (argsCount > 0)
                    {
                        cCall.ArgsStackDepthMax = argsStackDepth;
                        cCall.ArgsType = CallArgsType.Fixed;
                        cCall.ByteArgsNum = argsCount;
                        if (alArgs.Count > 0)
                        {
                            cCall.Arguments = new CallArgument[alArgs.Count];
                            alArgs.CopyTo(cCall.Arguments);
                        }
                        alArgs = null;
                        // Args Mode Calculation
                        identifyCallArgsMode(ref cCall, ref ops, opsMatchIndex, true);
                        Calibration.alArgsCallsUniqueAddresses.Add(cCall.UniqueAddress);
                        ops = null;
                        return;
                    }
                    alArgs = null;
                }
                ops = null;
            }
            else
            // 8065 Args Mode based on Stack
            {
                ops = getCallOps(ref cCall, 32, 99, true, true, false, false, false, true, true, true);

                // Variable Args
                matchingOriginalOps = new string[] { "a2,20,..", "..,..,..", "..,..,..", "a3,20,..,..", "b2,..,..", "c3,20,..,..", "a3,20,..,..", "..,..,..", "..,..,..", "a3,20,..,..", "b2,..,..", "c6,..,..", "e0,..,.." };
                opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                if (opsMatchIndex >= 0)
                {
                    cCall.ArgsStackDepthMax = 1;       // First RXX = [STACK+2], other RXX = [STACK+X] is for its caller and the variable number, RXX = [STACK] is for Bank Num
                    cCall.ArgsType = CallArgsType.Variable;
                    cCall.ByteArgsNum = -1;
                    cCall.Arguments = new CallArgument[1];
                    cCall.Arguments[0] = new CallArgument();
                    cCall.Arguments[0].StackDepth = cCall.ArgsStackDepthMax;
                    cCall.Arguments[0].Word = false;
                    // Args Mode Defaulted
                    cCall.Arguments[0].Mode = CallArgsMode.Standard;
                    // Args Mode Defaulted
                    cCall.ArgsModes = new CallArgsMode[] { CallArgsMode.Standard };

                    // Because of Signature, First output register is before opsMatchIndex, we are searching for it
                    for (int iPos = opsMatchIndex - 1; iPos >= 0; iPos--)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        switch (ope.OriginalOPCode.ToLower())
                        {
                            case "a1":
                                // First output register address
                                if (cCall.ArgsVariableOutputFirstRegisterAddress == -1)
                                {
                                    cCall.ArgsVariableOutputFirstRegisterAddress = Convert.ToInt32(ope.OperationParams[0].InstructedParam, 16);
                                    ope = null;
                                }
                                break;
                        }
                        if (ope == null) break;
                    }
                    ope = null;

                    Calibration.alArgsCallsUniqueAddresses.Add(cCall.UniqueAddress);
                    ops = null;
                    return;
                }

                // Fixed Num
                matchingOriginalOps = new string[] { "a3,20,..,..", "a3,20,..,..", "f2", "fa", "..,..,..", "..,..,..", "b2,..,.." };
                opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                if (opsMatchIndex < 0 || ops.Length < opsMatchIndex + opsMatchIndexAdder)
                {
                    matchingOriginalOps = new string[] { "a2,20,..", "fa", "..,..,..", "..,..,..", "a3,20,..,.." };
                    opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                    if (opsMatchIndex < 0 || ops.Length < opsMatchIndex + opsMatchIndexAdder)
                    {
                        matchingOriginalOps = new string[] { "a2,20,..", "f2", "fa", "..,..,..", "..,..,..", "a3,20,..,.." };
                        opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                        if (opsMatchIndex < 0 || ops.Length < opsMatchIndex + opsMatchIndexAdder)
                        {
                            // 20200514 - PYM - Added for 8065 60pin
                            matchingOriginalOps = new string[] { "a2,20,..", "a3,20,..,..", "51,..,..,..", "d7,..", "10,..", "b2,..,.." };
                            opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                        }
                    }
                }
                if (opsMatchIndex >= 0)
                {
                    argsStackDepth = 0;
                    argsStackDepthReducer = 0;
                    argsCount = 0;
                    alArgs = new ArrayList();
                    alArgsDetect = new ArrayList();
                    for (int iPos = opsMatchIndex; iPos < ops.Length; iPos++)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        switch (ope.OriginalOPCode.ToLower())
                        {
                            case "10":
                                // Bank Change for reading does not stop loop
                                break;
                            case "f2":
                                // PushP between RXX =[STACK] and RXX = [STACK+X] forces the STACK to go deeper
                                argsStackDepthReducer++;
                                break;
                            case "a3":
                                // Each RXX = [STACK+X] adds a stack depth, RXX = [STACK] is for Bank Num
                                if (ope.OriginalOpArr[1] == "20")
                                {
                                    argsStackDepth = Convert.ToInt32(ope.OriginalOpArr[2], 16) / 2 - argsStackDepthReducer;
                                    alArgsDetect.Add(new CallArgumentDetection(argsStackDepth, ope.OriginalOpArr[ope.OriginalOpArr.Length - 1]));
                                }
                                break;
                            case "b2":  // ldb   RXX,[RYY++]
                            case "be":  // ldsbw RXX,[RYY++]
                            case "ae":  // ldzbw RXX,[RYY++]
                                foundArgsDetect = false;
                                foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                                {
                                    if (cArgDetect.StackReadRegister != string.Format("{0:x2}", Convert.ToInt32(ope.OriginalOpArr[1], 16) - 1)) continue;

                                    foundArgsDetect = true;

                                    argsCount++;
                                    cArgDetect.CountModeOn = true;
                                    int newOutputRegisterAddressInt = Convert.ToInt32(ope.OriginalOpArr[ope.OriginalOpArr.Length - 1], 16);
                                    if (cArgDetect.CallArgument != null)
                                    {
                                        if (cArgDetect.CallArgument.OutputRegisterAddressInt + 1 == newOutputRegisterAddressInt)
                                        {
                                            cArgDetect.CallArgument.Word = true;
                                            newOutputRegisterAddressInt = -1;
                                        }
                                        alArgs.Add(cArgDetect.CallArgument);
                                        cArgDetect.CallArgument = null;
                                    }
                                    if (newOutputRegisterAddressInt >= 0) cArgDetect.CreateCallArgument(newOutputRegisterAddressInt);
                                }
                                if (!foundArgsDetect)
                                {
                                    // Remaining Args added before removal and break condition
                                    foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                                    {
                                        if (cArgDetect.CountModeOn) foundArgsDetect = true;
                                        if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);
                                    }
                                    if (foundArgsDetect) alArgsDetect = null;
                                }
                                break;
                            default:
                                // Remaining Args added before removal and break condition
                                foundArgsDetect = false;
                                foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                                {
                                    if (cArgDetect.CountModeOn) foundArgsDetect = true;
                                    if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);
                                }
                                if (foundArgsDetect) alArgsDetect = null;
                                break;
                        }
                        ope = null;
                        if (alArgsDetect == null) break;
                    }
                    if (alArgsDetect != null)
                    {
                        // Remaining Args added
                        foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                        {
                            if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);
                        }
                        alArgsDetect = null;
                    }
                    if (argsCount > 0)
                    {
                        cCall.ArgsStackDepthMax = argsStackDepth;
                        cCall.ArgsType = CallArgsType.Fixed;
                        cCall.ByteArgsNum = argsCount;
                        if (alArgs.Count > 0)
                        {
                            cCall.Arguments = new CallArgument[alArgs.Count];
                            alArgs.CopyTo(cCall.Arguments);
                        }
                        alArgs = null;
                        // Args Mode Calculation
                        identifyCallArgsMode(ref cCall, ref ops, opsMatchIndex, true);
                        Calibration.alArgsCallsUniqueAddresses.Add(cCall.UniqueAddress);
                        ops = null;
                        return;
                    }
                    alArgs = null;
                }

                // 20200514 - PYM - Added for 8065 60pin
                // Fixed Num for first 8065 60pin, but not early
                // cc,18        pop   R18            R18 = pop();
                // ae,19,14     ldzbw R14,[R18++]    R14 = (uns)[R18++];
                // b2,19,17     ldb   R17,[R18++]    R17 = [R18++];
                matchingOriginalOps = new string[] { "cc,..", "10,08", "b2,..,.." };
                opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                if (opsMatchIndex < 0)
                {
                    matchingOriginalOps = new string[] { "cc,..", "10,08", "ae,..,.." };
                    opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                }
                if (opsMatchIndex < 0)
                {
                    matchingOriginalOps = new string[] { "cc,..", "10,08", "be,..,.." };
                    opsMatchIndex = Tools.matchOpsOriginalOps(ref ops, matchingOriginalOps);
                }
                if (opsMatchIndex >= 0)
                {
                    // Because of Signature, other pop before Match Index could add Depth, we search for them
                    for (int iPos = opsMatchIndex - 1; iPos >= 0; iPos--)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        if (ope.OriginalOPCode.ToLower() != "cc") break;
                        opsMatchIndex--;
                    }
                    ope = null;

                    argsStackDepth = 0;
                    argsCount = 0;
                    alArgs = new ArrayList();
                    alArgsDetect = new ArrayList();
                    for (int iPos = opsMatchIndex; iPos < ops.Length; iPos++)
                    {
                        ope = ops[iPos];
                        if (ope == null) break;
                        switch (ope.OriginalOPCode.ToLower())
                        {
                            case "cc":
                                // Each pop adds a Stack Depth for CallArgumentDetection
                                argsStackDepth++;
                                alArgsDetect.Add(new CallArgumentDetection(argsStackDepth, ope.OriginalOpArr[ope.OriginalOpArr.Length - 1]));
                                break;
                            case "b2":  // ldb   RXX,[RYY++]
                            case "be":  // ldsbw RXX,[RYY++]
                            case "ae":  // ldzbw RXX,[RYY++]
                                foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                                {
                                    if (cArgDetect.StackReadRegister != string.Format("{0:x2}", Convert.ToInt32(ope.OriginalOpArr[1], 16) - 1)) continue;

                                    argsCount++;
                                    cArgDetect.CountModeOn = true;
                                    int newOutputRegisterAddressInt = Convert.ToInt32(ope.OriginalOpArr[ope.OriginalOpArr.Length - 1], 16);
                                    if (cArgDetect.CallArgument != null)
                                    {
                                        if (cArgDetect.CallArgument.OutputRegisterAddressInt + 1 == newOutputRegisterAddressInt)
                                        {
                                            cArgDetect.CallArgument.Word = true;
                                            newOutputRegisterAddressInt = -1;
                                        }
                                        alArgs.Add(cArgDetect.CallArgument);
                                        cArgDetect.CallArgument = null;
                                    }
                                    if (newOutputRegisterAddressInt >= 0) cArgDetect.CreateCallArgument(newOutputRegisterAddressInt);
                                }
                                break;
                            case "c8":  // push(RXX);
                                int iArgsDetectToRemove = -1;
                                for (int iIndex = 0; iIndex < alArgsDetect.Count; iIndex++)
                                {
                                    CallArgumentDetection cArgDetect = (CallArgumentDetection)alArgsDetect[iIndex];
                                    if (!cArgDetect.CountModeOn) continue;
                                    if (cArgDetect.StackReadRegister != ope.OriginalOpArr[1].ToLower()) continue;

                                    // Remaining Args added before removal
                                    if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);
                                    
                                    iArgsDetectToRemove = iIndex;
                                    break;
                                }
                                if (iArgsDetectToRemove >= 0) alArgsDetect.RemoveAt(iArgsDetectToRemove);
                                break;
                        }
                        ope = null;
                        if (alArgsDetect.Count == 0) break;
                    }
                    // Remaining Args added before cleanup
                    foreach (CallArgumentDetection cArgDetect in alArgsDetect)
                    {
                        if (cArgDetect.CallArgument != null) alArgs.Add(cArgDetect.CallArgument);
                    }
                    alArgsDetect = null;

                    if (argsCount > 0)
                    {
                        cCall.ArgsStackDepthMax = argsStackDepth;
                        cCall.ArgsType = CallArgsType.Fixed;
                        cCall.ByteArgsNum = argsCount;
                        if (alArgs.Count > 0)
                        {
                            cCall.Arguments = new CallArgument[alArgs.Count];
                            alArgs.CopyTo(cCall.Arguments);
                        }
                        alArgs = null;
                        // Args Mode Calculation
                        identifyCallArgsMode(ref cCall, ref ops, opsMatchIndex, true);
                        Calibration.alArgsCallsUniqueAddresses.Add(cCall.UniqueAddress);
                        ops = null;
                        return;
                    }
                    alArgs = null;
                }

                ops = null;
            }

            // Default Unknown for Args
            cCall.ArgsType = CallArgsType.None;
            cCall.ArgsModes = null;
            cCall.Arguments = null;
        }

        private void identifyCallArgsMode(ref Call cCall, ref Operation[] ops, int opsMatchIndex, bool quickSearch)
        {
            Call argsModeCall = null;
            Operation[] adjacentOps = null;
            Operation ope = null;
            CallArgsMode resultArgsMode = CallArgsMode.Standard;
            CallArgsMode possibleArgsMode = CallArgsMode.Unknown;
            ArrayList alArgsModes = null;
            int opsMatchAddress = -1;

            // Already Processed
            if (cCall.ArgsModes != null) return;

            // First RBase is required
            if (Calibration.slRbases.Count == 0)
            {
                cCall.ArgsModes = new CallArgsMode[] { CallArgsMode.Standard };
                if (cCall.Arguments != null) foreach (CallArgument cArg in cCall.Arguments) cArg.Mode = CallArgsMode.Standard;
                return;
            }

            alArgsModes = new ArrayList();

            // Search based on provided Ops
            for (int iPos = opsMatchIndex; iPos < ops.Length; iPos++)
            {
                ope = ops[iPos];
                if (ope == null) break;
                if (ope.isReturn) break;
                if (iPos == opsMatchIndex) opsMatchAddress = ope.AddressInt;
                switch (ope.CallType)
                {
                    case CallType.Call:
                    case CallType.ShortCall:
                        if (!Calibration.alArgsModesCallsUniqueAddresses.Contains(Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)))
                        {
                            if (Calibration.slCalls.ContainsKey(Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)))
                            {
                                argsModeCall = (Call)Calibration.slCalls[Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)];
                                if (argsModeCall.ArgsModes == null)
                                {
                                    identifyCallArgsModeCore(ref argsModeCall);
                                }
                                else
                                {
                                    foreach (CallArgsMode argMode in argsModeCall.ArgsModes) alArgsModes.Add(argMode);
                                    
                                    if (cCall.Arguments != null)
                                    {
                                        if (argsModeCall.Arguments != null)
                                        {
                                            if (cCall.ArgsVariableOutputFirstRegisterAddress >= 0 && ope.CallArguments != null)
                                            // Call Parameters come from Variable Parameters, used Registers are known
                                            {
                                                foreach (CallArgument cArg in cCall.Arguments)
                                                {
                                                    foreach (CallArgument opeArg in ope.CallArguments)
                                                    {
                                                        if (cArg.OutputRegisterAddressInt == opeArg.InputValueInt)
                                                        {
                                                            cArg.Mode = opeArg.Mode;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            // Default Matching
                                            {
                                                foreach (CallArgument cArg in argsModeCall.Arguments)
                                                {
                                                    foreach (CallArgument ccArg in cCall.Arguments)
                                                    {
                                                        if (ccArg.Mode == CallArgsMode.Unknown)
                                                        {
                                                            ccArg.Mode = cArg.Mode;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (CallArgsMode argMode in argsModeCall.ArgsModes)
                                            {
                                                foreach (CallArgument ccArg in cCall.Arguments)
                                                {
                                                    if (ccArg.Mode == CallArgsMode.Unknown)
                                                    {
                                                        ccArg.Mode = argMode;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                
                                argsModeCall = null;
                            }
                        }
                        if (Calibration.alArgsModesCallsUniqueAddresses.Contains(Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)))
                        {
                            argsModeCall = (Call)Calibration.slCalls[Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)];
                            if (argsModeCall != null)
                            {
                                foreach (CallArgsMode argMode in argsModeCall.ArgsModes) alArgsModes.Add(argMode);
                                
                                if (cCall.Arguments != null)
                                {
                                    if (argsModeCall.Arguments != null)
                                    {
                                        foreach (CallArgument cArg in argsModeCall.Arguments)
                                        {
                                            foreach (CallArgument ccArg in cCall.Arguments)
                                            {
                                                if (ccArg.Mode == CallArgsMode.Unknown)
                                                {
                                                    ccArg.Mode = cArg.Mode;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        foreach (CallArgsMode argMode in argsModeCall.ArgsModes)
                                        {
                                            foreach (CallArgument ccArg in cCall.Arguments)
                                            {
                                                if (ccArg.Mode == CallArgsMode.Unknown)
                                                {
                                                    ccArg.Mode = argMode;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            argsModeCall = null;
                        }
                        break;
                }
                switch (ope.OriginalOPCode)
                {
                    // 08,03,3a          shrw  R3a,3          R3a = R3a / 8;    => Mode 3
                    // 08,04,34          shrw  R34,4          R34 = R34 / 10;   => Mode 4
                    case "08":
                        switch (ope.OriginalOpArr[1])
                        {
                            case "01":
                                possibleArgsMode = CallArgsMode.Mode1;
                                break;
                            case "02":
                                possibleArgsMode = CallArgsMode.Mode2;
                                break;
                            case "03":
                                possibleArgsMode = CallArgsMode.Mode3;
                                break;
                            case "04":
                                // Early 8065 use a specific mode through a structure for Addresses and Registers
                                if (isEarly) possibleArgsMode = CallArgsMode.Mode4Struct;
                                else possibleArgsMode = CallArgsMode.Mode4;
                                break;
                        }
                        break;
                    case "67":  // 67,3b,f0,00,36       ad2w  R36,[R3a+f0]       R36 += [R3a+f0];
                        if (ope.OriginalOpArr[2] == ((RBase)Calibration.slRbases.GetByIndex(0)).Code.ToLower())
                        {
                            resultArgsMode = possibleArgsMode;
                        }
                        break;
                }
                ope = null;
                if (resultArgsMode == possibleArgsMode)
                {
                    alArgsModes.Add(resultArgsMode);
                    if (cCall.Arguments != null)
                    {
                        foreach (CallArgument ccArg in cCall.Arguments)
                        {
                            if (ccArg.Mode == CallArgsMode.Unknown)
                            {
                                ccArg.Mode = resultArgsMode;
                                break;
                            }
                        }
                    }
                    cCall.isArgsModeCore = true;

                    resultArgsMode = CallArgsMode.Standard;
                    possibleArgsMode = CallArgsMode.Unknown;
                }
            }

            if (alArgsModes.Count > 0)
            {
                if ((CallArgsMode)alArgsModes[alArgsModes.Count - 1] != CallArgsMode.Standard) alArgsModes.Add(CallArgsMode.Standard);
                cCall.ArgsModes = (CallArgsMode[])alArgsModes.ToArray(typeof(CallArgsMode));
                alArgsModes = null;

                // Remaining Arguments are set to Standard
                if (cCall.Arguments != null)
                {
                    foreach (CallArgument cArg in cCall.Arguments) if (cArg.Mode == CallArgsMode.Unknown) cArg.Mode = CallArgsMode.Standard;
                }
                return;
            }

            alArgsModes = null;

            // Deep Search
            if (quickSearch && opsMatchAddress >= 0)
            {
                adjacentOps = getFollowingOPs(opsMatchAddress, 32, 1, true, false, false, false, false, false, false, false);
                identifyCallArgsMode(ref cCall, ref adjacentOps, 0, false);
                adjacentOps = null;
            }
            else
            {
                cCall.ArgsModes = new CallArgsMode[] { CallArgsMode.Standard };
                // Arguments are set to Standard
                if (cCall.Arguments != null)
                {
                    foreach (CallArgument cArg in cCall.Arguments) if (cArg.Mode == CallArgsMode.Unknown) cArg.Mode = CallArgsMode.Standard;
                }
            }
        }

        private void identifyCallArgsModeCore(ref Call cCall)
        {
            Operation[] ops = null;
            Operation ope = null;
            CallArgsMode argsMode = CallArgsMode.Standard;
            CallArgsMode possibleArgsMode = CallArgsMode.Unknown;
            ArrayList alArgsModes = null;

            // Already Processed
            if (cCall.ArgsModes != null) return;

            cCall.ArgsModes = new CallArgsMode[] { };
            cCall.isArgsModeCore = false;

            // First RBase is required
            if (Calibration.slRbases.Count == 0) return;

            alArgsModes = new ArrayList();
            argsMode = CallArgsMode.Standard;
            possibleArgsMode = CallArgsMode.Unknown;
            ops = getFollowingOPs(cCall.AddressInt, 16, 1, false, false, false, false, false, false, false, false);
            for (int iPos = 0; iPos < ops.Length; iPos++)
            {
                ope = ops[iPos];
                if (ope == null) break;
                if (ope.isReturn) break;
                switch (ope.OriginalOPCode)
                {
                    // 08,03,3a          shrw  R3a,3          R3a = R3a / 8;    => Mode 3
                    // 08,04,34          shrw  R34,4          R34 = R34 / 10;   => Mode 4
                    case "08":
                        switch (ope.OriginalOpArr[1])
                        {
                            case "01":
                                possibleArgsMode = CallArgsMode.Mode1;
                                break;
                            case "02":
                                possibleArgsMode = CallArgsMode.Mode2;
                                break;
                            case "03":
                                possibleArgsMode = CallArgsMode.Mode3;
                                break;
                            case "04":
                                // Early 8065 use a specific mode through a structure for Addresses and Registers
                                if (isEarly) possibleArgsMode = CallArgsMode.Mode4Struct;
                                else possibleArgsMode = CallArgsMode.Mode4;
                                break;
                        }
                        break;
                    case "67":  // 67,3b,f0,00,36       ad2w  R36,[R3a+f0]       R36 += [R3a+f0];
                        if (ope.OriginalOpArr[2] == ((RBase)Calibration.slRbases.GetByIndex(0)).Code.ToLower())
                        {
                            argsMode = possibleArgsMode;
                        }
                        break;
                }
                ope = null;
                if (argsMode == possibleArgsMode)
                {
                    alArgsModes.Add(argsMode);
                    argsMode = CallArgsMode.Standard;
                    possibleArgsMode = CallArgsMode.Unknown;
                }
            }

            if (alArgsModes.Count > 0)
            {
                cCall.ArgsModes = (CallArgsMode[]) alArgsModes.ToArray(typeof(CallArgsMode));
                cCall.isArgsModeCore = true;
                if (!Calibration.alArgsModesCallsUniqueAddresses.Contains(cCall.UniqueAddress)) Calibration.alArgsModesCallsUniqueAddresses.Add(cCall.UniqueAddress);
                Calibration.slCalls[cCall.UniqueAddress] = cCall;
            }
        }

        private void identifyCallType(ref Call cCall)
        {
            Operation[] opsResult = null;
            Routine srcRoutine = null;
            Routine rRoutine = null;
            string[] matchingOpsCodes = null;
            int opsMatchIndex = -1;
            int opsMatchMode = -1;
            int opsMatchAddress = -1;
            int depth = -1;
            int opsCount = -1;

            if (cCall.isIdentified) return;

            // Interrupt Vectors Identification            
            foreach (Vector vector in slIntVectors.Values)
            {
                if (vector.UniqueAddress == cCall.UniqueAddress)
                {
                    cCall.isIntVector = true;
                    cCall.ShortLabel = vector.ShortLabel;
                    cCall.Label = vector.Label;
                    Calibration.slCalls[cCall.UniqueAddress] = cCall;

                    return;
                }
            }

            // Additional Vectors Identification            
            foreach (Vector vector in Calibration.slAdditionalVectors.Values)
            {
                if (vector.UniqueAddress == cCall.UniqueAddress)
                {
                    cCall.isVector = true;
                    Calibration.slCalls[cCall.UniqueAddress] = cCall;

                    return;
                }
            }

            // Short Identification based on Signature Root Identification for Table, Function or Other Routines
            depth = 16;
            opsResult = getFollowingOPs(cCall.AddressInt, depth, 99, true, true, false, false, false, true, true, true);
            foreach (Operation ope in opsResult)
            {
                if (ope == null) break;
                if (ope.isReturn) break;
                // Ope is in an identified routine
                if (Calibration.slRoutines.ContainsKey(Tools.UniqueAddress(ope.BankNum, ope.InitialCallAddressInt)))
                {
                    srcRoutine = (Routine)Calibration.slRoutines[Tools.UniqueAddress(ope.BankNum, ope.InitialCallAddressInt)];
                    break;
                }
                switch (ope.CallType)
                {
                    case CallType.ShortJump:
                    case CallType.Goto:
                    case CallType.Skip:
                        if (Calibration.slRoutines.ContainsKey(Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)))
                        {
                            srcRoutine = (Routine)Calibration.slRoutines[Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)];
                        }
                        break;
                    case CallType.Unknown:
                        if (Calibration.slRoutines.ContainsKey(Tools.UniqueAddress(ope.BankNum, ope.AddressNextInt)))
                        {
                            srcRoutine = (Routine)Calibration.slRoutines[Tools.UniqueAddress(ope.BankNum, ope.AddressNextInt)];
                        }
                        break;
                }
                if (srcRoutine != null) break;
            }

            // Applies only for Typed Routines (Table, Function and Other)
            if (srcRoutine != null) if (srcRoutine.Type == RoutineType.Unknown) srcRoutine = null;
            
            if (srcRoutine != null)
            {
                cCall.isRoutine = true;
                Calibration.slCalls[cCall.UniqueAddress] = cCall;

                rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
                rRoutine.Type = srcRoutine.Type;
                rRoutine.IOs = srcRoutine.CloneIOs();

                srcRoutine = null;

                // Signed / Unsigned Input Identification - Can not be herited
                if (rRoutine.Type == RoutineType.TableByte || rRoutine.Type == RoutineType.TableWord)
                // Table Routine - Always 1 RoutineIOTable
                {
                    if (rRoutine.IOs[0].SignOutputCondRegister != string.Empty)
                    {
                        foreach (Operation ope in opsResult)
                        {
                            if (ope == null) break;
                            // 20171127 - ShortCall Added
                            //if (ope.AddressInt == rRoutine.FirstCondRegisterOpeAddress)
                            // Call to Table Core Routine or First Condition Test Value Reached, 0 condition can be tested
                            if (ope.CallType == CallType.ShortCall || ope.AddressInt == rRoutine.IOs[0].FirstCondRegisterOpeAddress)
                            {
                                rRoutine.IOs[0].setSignedOutput(0);
                                break;
                            }
                            switch (ope.OriginalInstruction.ToLower())
                            {
                                case "an2b":    // &=
                                case "ad2b":
                                case "orrb":
                                case "ldb":
                                    if (rRoutine.IOs[0].SignOutputCondRegister == ope.OriginalOpArr[ope.OriginalOpArr.Length - 1])
                                    {
                                        rRoutine.IOs[0].setSignedOutput(Convert.ToByte(ope.OriginalOpArr[1], 16));
                                    }
                                    break;

                                case "stb":
                                    if (rRoutine.IOs[0].SignOutputCondRegister == ope.OriginalOpArr[1])
                                    {
                                        rRoutine.IOs[0].setSignedOutput(Convert.ToByte(ope.OriginalOpArr[ope.OriginalOpArr.Length - 1], 16));
                                    }
                                    break;
                            }
                            if (rRoutine.IOs[0].isSignedOutputDefined) break;
                        }
                    }
                }
                else if (rRoutine.Type == RoutineType.FunctionByte || rRoutine.Type == RoutineType.FunctionWord)
                // Function Routine - Always 1 RoutineIOFunction
                {
                    if (rRoutine.IOs[0].SignOutputCondRegister != string.Empty && ((RoutineIOFunction)rRoutine.IOs[0]).FunctionSignInputCondRegister != string.Empty)
                    {
                        byte condValue = 0;
                        byte inputCondValue = 0;
                        byte outputCondValue = 0;
                        int inputCondCount = 0;
                        int outputCondCount = 0;
                        string condReg = string.Empty;
                        opsCount = 0;
                        foreach (Operation ope in opsResult)
                        {
                            opsCount++;
                            if (ope == null) break;
                            // First Condition Test Value Reached, current condition can be tested
                            if (ope.AddressInt == rRoutine.IOs[0].FirstCondRegisterOpeAddress)
                            {
                                ((RoutineIOFunction)rRoutine.IOs[0]).setFunctionSignedInput(inputCondValue);
                                rRoutine.IOs[0].setSignedOutput(outputCondValue);
                                break;
                            }
                            switch (ope.OriginalInstruction.ToLower())
                            {
                                case "an2b":    // &=
                                case "ad2b":
                                case "orrb":
                                case "ldb":
                                    condReg = ope.OriginalOpArr[ope.OriginalOpArr.Length - 1];
                                    condValue = Convert.ToByte(ope.OriginalOpArr[1], 16);
                                    if (((RoutineIOFunction)rRoutine.IOs[0]).FunctionSignInputCondRegister == condReg)
                                    {
                                        inputCondCount++;
                                        inputCondValue += condValue;
                                        ((RoutineIOFunction)rRoutine.IOs[0]).setFunctionSignedInput(inputCondValue);
                                    }
                                    if (rRoutine.IOs[0].SignOutputCondRegister == condReg)
                                    {
                                        outputCondCount++;
                                        outputCondValue += condValue;
                                        rRoutine.IOs[0].setSignedOutput(outputCondValue);
                                    }
                                    break;
                                case "stb":
                                    condReg = ope.OriginalOpArr[1];
                                    condValue = Convert.ToByte(ope.OriginalOpArr[ope.OriginalOpArr.Length - 1], 16);
                                    if (((RoutineIOFunction)rRoutine.IOs[0]).FunctionSignInputCondRegister == condReg)
                                    {
                                        ((RoutineIOFunction)rRoutine.IOs[0]).setFunctionSignedInput(condValue);
                                        inputCondCount = 2;
                                    }
                                    if (rRoutine.IOs[0].SignOutputCondRegister == condReg)
                                    {
                                        rRoutine.IOs[0].setSignedOutput(condValue);
                                        outputCondCount = 2;
                                    }
                                    break;
                            }
                            if (opsCount >= 8 || (inputCondCount >= 2 && outputCondCount >= 2)) break;
                        }
                    }
                }

                rRoutine.SetTranslationComments();

                Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);

                opsResult = null;
                rRoutine = null;

                return;
            }

            // Signature Identification for Root Identification

            // Table Routine Identification - 16 Operations Analysis, 32 for Call beginning with Args Mngt
            depth = 32;
            if (cCall.ArgsType == CallArgsType.None || cCall.ArgsType == CallArgsType.Unknown) depth = 16;
            opsResult = getFollowingOPs(cCall.AddressInt, depth, 99, false, true, false, false, false, true, true, true);
            matchingOpsCodes = new string[] { "5c", "74", "b4", "64", "b2", "b2" };
            if (is8061 || isPilot) matchingOpsCodes = new string[] { "5c", "74", "b4", "64", "b2", "b2" };
            opsMatchMode = 1;
            opsMatchIndex = Tools.matchOpsOpCodes(ref opsResult, matchingOpsCodes);
            if (opsMatchIndex < 0)
            {
                matchingOpsCodes = new string[] { "5c", "74", "64", "b2", "b2" };
                if (is8061 || isPilot) matchingOpsCodes = new string[] { "5c", "74", "64", "b2", "b2" };
                opsMatchMode = 2;
                opsMatchIndex = Tools.matchOpsOpCodes(ref opsResult, matchingOpsCodes);
            }
            if (opsMatchIndex >= 0)
            {
                // Validity check and Output Register and Sign Register Identification
                // for Starting next to Main Table Routine
                opsMatchAddress = -1;
                opsCount = 0;
                foreach (Operation ope in opsResult)
                {
                    if (ope == null) break;
                    if (ope.isReturn)
                    {
                        // Not the same Call
                        opsMatchIndex = -1;
                        break;
                    }
                    if (opsCount == opsMatchIndex)
                    {
                        opsMatchAddress = ope.AddressInt;
                        break;
                    }
                    opsCount++;
                }
            }
            if (opsMatchIndex >= 0)
            {
                cCall.isRoutine = true;
                Calibration.slCalls[cCall.UniqueAddress] = cCall;
                
                rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
                rRoutine.Type = RoutineType.TableByte;
                rRoutine.IOs = new RoutineIO[] {new RoutineIOTable()};
                ((RoutineIOTable)rRoutine.IOs[0]).TableWord = false;
                ((RoutineIOTable)rRoutine.IOs[0]).TableRowRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex].OriginalOpArr[1], 16) - 1);
                ((RoutineIOTable)rRoutine.IOs[0]).TableColNumberRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex].OriginalOpArr[2], 16));
                ((RoutineIOTable)rRoutine.IOs[0]).TableColRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 1].OriginalOpArr[1], 16) - 1);
                if (opsMatchMode == 1) rRoutine.IOs[0].AddressRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 3].OriginalOpArr[2], 16));
                else rRoutine.IOs[0].AddressRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 2].OriginalOpArr[2], 16));
                
                if (opsMatchAddress < 0) opsMatchAddress = rRoutine.AddressInt;

                identifyCallTypeTableCoreDetails(ref rRoutine, opsMatchAddress);

                rRoutine.SetTranslationComments();
                
                Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);

                matchingOpsCodes = null;
                opsResult = null;
                rRoutine = null;

                return;
            }
            matchingOpsCodes = null;
            opsResult = null;

            // Table Word Routine Identification - 16 Operations Analysis, 32 for Call beginning with Args Mngt
            //      Visible on Early 8065 and others
            depth = 32;
            if (cCall.ArgsType == CallArgsType.None || cCall.ArgsType == CallArgsType.Unknown) depth = 16;
            opsResult = getFollowingOPs(cCall.AddressInt, depth, 99, false, true, false, false, false, true, true, true);
            matchingOpsCodes = new string[] { "5c", "74", "b4", "09", "64", "b0", "a2", "a2" };
            opsMatchMode = 1;
            opsMatchIndex = Tools.matchOpsOpCodes(ref opsResult, matchingOpsCodes);
            if (opsMatchIndex < 0)
            {
                matchingOpsCodes = new string[] { "5c", "74", "b4", "64", "64", "b0", "a2", "a2" };
                opsMatchMode = 1;
                opsMatchIndex = Tools.matchOpsOpCodes(ref opsResult, matchingOpsCodes);
            }
            if (opsMatchIndex < 0)
            {
                matchingOpsCodes = new string[] { "5c", "b0", "b0", "ac", "64", "09", "64", "a2", "a2" };
                opsMatchMode = 2;
                opsMatchIndex = Tools.matchOpsOpCodes(ref opsResult, matchingOpsCodes);
            }
            if (opsMatchIndex >= 0)
            {
                // Validity check and Output Register and Sign Register Identification
                // for Starting next to Main Table Routine
                opsMatchAddress = -1;
                opsCount = 0;
                foreach (Operation ope in opsResult)
                {
                    if (ope == null) break;
                    if (ope.isReturn)
                    {
                        // Not the same Call
                        opsMatchIndex = -1;
                        break;
                    }
                    if (opsCount == opsMatchIndex)
                    {
                        opsMatchAddress = ope.AddressInt;
                        break;
                    }
                    opsCount++;
                }
            }
            if (opsMatchIndex >= 0)
            {
                cCall.isRoutine = true;
                Calibration.slCalls[cCall.UniqueAddress] = cCall;

                rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
                rRoutine.Type = RoutineType.TableWord;
                rRoutine.IOs = new RoutineIO[] { new RoutineIOTable() };
                ((RoutineIOTable)rRoutine.IOs[0]).TableWord = true;
                switch (opsMatchMode)
                {
                    case 1:
                        ((RoutineIOTable)rRoutine.IOs[0]).TableColNumberRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex].OriginalOpArr[2], 16));
                        ((RoutineIOTable)rRoutine.IOs[0]).TableRowRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex].OriginalOpArr[1], 16) - 1);
                        ((RoutineIOTable)rRoutine.IOs[0]).TableColRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 1].OriginalOpArr[1], 16) - 1);
                        rRoutine.IOs[0].AddressRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 4].OriginalOpArr[2], 16));
                        break;
                    case 2:
                        ((RoutineIOTable)rRoutine.IOs[0]).TableColNumberRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex].OriginalOpArr[2], 16));
                        ((RoutineIOTable)rRoutine.IOs[0]).TableRowRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex].OriginalOpArr[1], 16) - 1);
                        ((RoutineIOTable)rRoutine.IOs[0]).TableColRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 1].OriginalOpArr[1], 16));
                        rRoutine.IOs[0].AddressRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 6].OriginalOpArr[2], 16));
                        break;
                }

                if (opsMatchAddress < 0) opsMatchAddress = rRoutine.AddressInt;

                identifyCallTypeTableCoreDetails(ref rRoutine, opsMatchAddress);

                rRoutine.SetTranslationComments();

                Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);

                matchingOpsCodes = null;
                opsResult = null;
                rRoutine = null;

                return;
            }
            matchingOpsCodes = null;
            opsResult = null;

            // Function Word Routine Identification - 16 Operations Analysis, 32 for Call beginning with Args Mngt
            if (cCall.ArgsType == CallArgsType.None || cCall.ArgsType == CallArgsType.Unknown) depth = 16;
            else depth = 32;
            opsResult = getFollowingOPs(cCall.AddressInt, depth, 99, false, true, false, false, false, true, true, true);
            matchingOpsCodes = new string[] { "a2", "a2", "6a", "6a", "6a" };
            if (is8061 || isPilot) matchingOpsCodes = new string[] { "a2", "a2", "6a", "6a", "6a" };
            opsMatchIndex = Tools.matchOpsOpCodes(ref opsResult, matchingOpsCodes);
            if (opsMatchIndex >= 0)
            {
                // Validity check
                opsCount = 0;
                foreach (Operation ope in opsResult)
                {
                    if (ope == null) break;
                    if (ope.isReturn)
                    {
                        // Not the same Call
                        opsMatchIndex = -1;
                        break;
                    }
                    if (opsCount == opsMatchIndex) break;
                    opsCount++;
                }
            }
            if (opsMatchIndex >= 0)
            {
                cCall.isRoutine = true;
                Calibration.slCalls[cCall.UniqueAddress] = cCall;
                
                rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
                rRoutine.Type = RoutineType.FunctionWord;
                rRoutine.IOs = new RoutineIO[] { new RoutineIOFunction() };
                ((RoutineIOFunction)rRoutine.IOs[0]).FunctionByte = false;
                rRoutine.IOs[0].AddressRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex].OriginalOpArr[1], 16) - 1);
                rRoutine.IOs[0].OutputRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 1].OriginalOpArr[2], 16));
                ((RoutineIOFunction)rRoutine.IOs[0]).FunctionInputRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 3].OriginalOpArr[2], 16));

                // Sign Register Identification
                identifyCallTypeFunctionCoreDetails(ref rRoutine, false);

                rRoutine.SetTranslationComments();
                
                Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);

                matchingOpsCodes = null;
                opsResult = null;
                rRoutine = null;

                return;
            }
            matchingOpsCodes = null;
            opsResult = null;

            // Function Byte Routine Identification - 16 Operations Analysis, 32 for Call beginning with Args Mngt
            if (cCall.ArgsType == CallArgsType.None || cCall.ArgsType == CallArgsType.Unknown) depth = 16;
            else depth = 32;
            opsResult = getFollowingOPs(cCall.AddressInt, depth, 99, false, true, false, false, false, true, true, true);
            matchingOpsCodes = new string[] { "b2", "b2", "7a", "7a", "7a" };
            if (is8061 || isPilot) matchingOpsCodes = new string[] { "b2", "b2", "7a", "7a", "7a" };
            opsMatchIndex = Tools.matchOpsOpCodes(ref opsResult, matchingOpsCodes);
            if (opsMatchIndex >= 0)
            {
                // Validity check
                opsCount = 0;
                foreach (Operation ope in opsResult)
                {
                    if (ope == null) break;
                    if (ope.isReturn)
                    {
                        // Not the same Call
                        opsMatchIndex = -1;
                        break;
                    }
                    if (opsCount == opsMatchIndex) break;
                    opsCount++;
                }
            }
            if (opsMatchIndex >= 0)
            {
                cCall.isRoutine = true;
                Calibration.slCalls[cCall.UniqueAddress] = cCall;
                
                rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
                rRoutine.Type = RoutineType.FunctionByte;
                rRoutine.IOs = new RoutineIO[] { new RoutineIOFunction() };
                ((RoutineIOFunction)rRoutine.IOs[0]).FunctionByte = true;
                rRoutine.IOs[0].AddressRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex].OriginalOpArr[1], 16) - 1);
                rRoutine.IOs[0].OutputRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 1].OriginalOpArr[2], 16));
                ((RoutineIOFunction)rRoutine.IOs[0]).FunctionInputRegister = string.Format("{0:x}", Convert.ToInt32(opsResult[opsMatchIndex + 3].OriginalOpArr[2], 16));

                // Sign Register Identification
                identifyCallTypeFunctionCoreDetails(ref rRoutine, true);

                rRoutine.SetTranslationComments();
                
                Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);

                matchingOpsCodes = null;
                opsResult = null;
                rRoutine = null;

                return;
            }
            matchingOpsCodes = null;
            opsResult = null;

            // Other Routine Identification
            if (cCall.Arguments != null)
            // Encrypted Arguments Call becomes Other Routine
            {
                foreach (CallArgument cArg in cCall.Arguments)
                {
                    switch (cArg.Mode)
                    {
                        case CallArgsMode.Unknown:
                        case CallArgsMode.Standard:
                            break;
                        // Mode1 => Mode4 are related with Calibration Elements
                        default:
                            cCall.isRoutine = true;
                            Calibration.slCalls[cCall.UniqueAddress] = cCall;
                            rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
                            rRoutine.Type = RoutineType.Other;
                            Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);
                            rRoutine = null;
                            return;
                    }
                }
            }
        }

        // Output Register and Sign Register Identification, including Table Core Routine
        private void identifyCallTypeTableCoreDetails(ref Routine rRoutine, int opeStartAddress)
        {
            RoutineIOTable ioTable = null;
            Operation[] opsResult = null;
            SortedList slCoreRoutines = null;
            Call cCore = null;
            Routine rCore = null;
            bool callEnd = false;
            int callPos = 0;

            // Single RoutineIOTable
            ioTable = (RoutineIOTable)rRoutine.IOs[0];

            // Full Call Analysis until a return a jump ope
            // Table Core Routine identification - is a scall in main Table Routine
            slCoreRoutines = new SortedList();
            opsResult = getFollowingOPs(opeStartAddress, 48, 99, true, true, false, false, false, false, false, false);
            callEnd = false;
            foreach (Operation ope in opsResult)
            {
                if (ope == null) break;
                switch (ope.OriginalInstruction.ToLower())
                {
                    case "scall":
                        cCore = (Call)Calibration.slCalls[Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)];
                        if (cCore != null)
                        {
                            if (!slCoreRoutines.ContainsKey(cCore.UniqueAddress))
                            {
                                cCore.isRoutine = true;
                                Calibration.slCalls[cCore.UniqueAddress] = cCore;

                                rCore = new Routine(cCore.BankNum, cCore.AddressInt);
                                rCore.Code = RoutineCode.TableCore;
                                rCore.SetTranslationComments();
                                slCoreRoutines.Add(rCore.UniqueAddress, rCore);
                                rCore = null;
                            }
                            cCore = null;
                        }
                        break;
                    case "jump":
                        callEnd = true;
                        break;
                }
                if (ope.isReturn) callEnd = true;

                if (callEnd) break;
            }
            opsResult = null;

            // Output Register - from Table or Table Core routine end, which should be a Return
            callPos = 0;
            ioTable.OutputRegister = string.Empty;
            while (callPos <= slCoreRoutines.Count)
            {
                if (callPos == slCoreRoutines.Count) cCore = (Call)Calibration.slCalls[rRoutine.UniqueAddress];
                else cCore = (Call)Calibration.slCalls[((Routine)slCoreRoutines.GetByIndex(callPos)).UniqueAddress];
                if (slOPs.ContainsKey(Tools.UniqueAddress(cCore.BankNum, cCore.AddressEndInt)))
                {
                    Operation lastOpe = (Operation)slOPs[Tools.UniqueAddress(cCore.BankNum, cCore.AddressEndInt)];
                    if (!lastOpe.isReturn)
                    // Search for next return
                    {
                        opsResult = getFollowingOPs(lastOpe.AddressInt, 16, 99, false, false, false, false, false, false, false, false);
                        foreach (Operation ope in opsResult)
                        {
                            if (ope == null) break;
                            if (ope.isReturn)
                            {
                                lastOpe = ope;
                                break;
                            }
                        }
                    }
                    if (lastOpe.isReturn)
                    {
                        //opsResult = getPrecedingOPs(lastOpe.AddressInt, 8, 99, false, false, false, false, false, false, false, false);
                        opsResult = getPrecedingOPs(lastOpe.AddressInt, 8, 0);
                        foreach (Operation ope in opsResult)
                        {
                            if (ope == null) break;
                            switch (ope.OriginalInstruction.ToLower())
                            {
                                case "ad2w":
                                case "sb2w":
                                    ioTable.OutputRegister = ope.OriginalOpArr[ope.OriginalOpArr.Length - 1];
                                    break;
                                case "stw":
                                    ioTable.OutputRegister = ope.OriginalOpArr[1];
                                    break;
                                case "incw":
                                    ioTable.OutputRegister = ope.OriginalOpArr[1];
                                    break;
                            }
                            if (ioTable.OutputRegister != string.Empty) break;
                        }
                    }
                    lastOpe = null;
                }
                cCore = null;
                if (ioTable.OutputRegister != string.Empty) break;
                callPos++;
            }
            opsResult = null;

            if (ioTable.OutputRegister != string.Empty)
            {
                if (Convert.ToInt32(ioTable.OutputRegister, 16) % 2 == 0)
                {
                    ioTable.TableOutputRegisterByte = Convert.ToString(Convert.ToInt32(ioTable.OutputRegister, 16) + 1, 16);
                }
                else
                {
                    ioTable.TableOutputRegisterByte = ioTable.OutputRegister;
                    ioTable.OutputRegister = Convert.ToString(Convert.ToInt32(ioTable.TableOutputRegisterByte, 16) - 1, 16);
                }
            }

            // Sign Condition identification from main Core Routine
            if (slCoreRoutines.Count > 0)
            {
                ioTable.FirstCondRegisterOpeAddress = -1;
                ioTable.SignOutputCondRegister = string.Empty;
                foreach (Routine rcRc in slCoreRoutines.Values)
                {
                    cCore = (Call)Calibration.slCalls[rcRc.UniqueAddress];
                    // Main Core Routine ends with a return
                    if (slOPs.ContainsKey(Tools.UniqueAddress(cCore.BankNum, cCore.AddressEndInt)))
                    {
                        Operation lastOpe = (Operation)slOPs[Tools.UniqueAddress(cCore.BankNum, cCore.AddressEndInt)];
                        if (!lastOpe.isReturn)
                        // Search for next return
                        {
                            opsResult = getFollowingOPs(lastOpe.AddressInt, 16, 99, false, false, false, false, false, false, false, false);
                            foreach (Operation ope in opsResult)
                            {
                                if (ope == null) break;
                                if (ope.isReturn)
                                {
                                    lastOpe = ope;
                                    break;
                                }
                            }
                        }
                        if (lastOpe.isReturn)
                        {
                            // 20171128 - Tests on Ops following Condition Jump added to invert or validate them
                            ioTable.SignOutputCondRegister = string.Empty;
                            opsResult = getFollowingOPs(cCore.AddressInt, 8, 99, true, true, false, false, false, false, false, false);
                            foreach (Operation ope in opsResult)
                            {
                                if (ope == null) break;
                                if (ioTable.SignOutputCondRegister == string.Empty)
                                //  Signed Jump Ope Detection
                                {
                                    switch (ope.OriginalInstruction.ToLower())
                                    {
                                        case "jnb":
                                            ioTable.FirstCondRegisterOpeAddress = ope.AddressInt;
                                            ioTable.SignOutputCondRegister = ope.OriginalOpArr[1];
                                            ioTable.SignOutputCondBit = Convert.ToInt32(ope.OriginalOpArr[0], 16) - 0x30;
                                            ioTable.SignedOutputCondValue = 1;
                                            ioTable.UnSignedOutputCondValue = 0;
                                            break;
                                        case "jb":
                                            ioTable.FirstCondRegisterOpeAddress = ope.AddressInt;
                                            ioTable.SignOutputCondRegister = ope.OriginalOpArr[1];
                                            ioTable.SignOutputCondBit = Convert.ToInt32(ope.OriginalOpArr[0], 16) - 0x38;
                                            ioTable.SignedOutputCondValue = 0;
                                            ioTable.UnSignedOutputCondValue = 1;
                                            break;
                                    }
                                }
                                else if (ope.OriginalOPCode == "5c")
                                // 5c (ml3b) following Signed Jump Op without following goto inverts Condition
                                {
                                    if (ioTable.SignedOutputCondValue == 1)
                                    {
                                        ioTable.SignedOutputCondValue = 0;
                                        ioTable.UnSignedOutputCondValue = 1;
                                    }
                                    else
                                    {
                                        ioTable.SignedOutputCondValue = 1;
                                        ioTable.UnSignedOutputCondValue = 0;
                                    }
                                    break;
                                }
                                else if (ope.OriginalOPCode == "68" || ope.OriginalOPCode.ToLower() == "bc")
                                // 68 (sb2w) ends the search
                                // bc (ldsbw) following Signed Jump Op without following goto validates Condition
                                {
                                    break;
                                }
                            }
                        }
                        lastOpe = null;
                    }
                    cCore = null;
                    if (ioTable.SignOutputCondRegister != string.Empty) break;
                }
            }
            opsResult = null;

            // Signed / Unsigned Input Identification
            if (ioTable.SignOutputCondRegister != string.Empty)
            {
                opsResult = getFollowingOPs(rRoutine.AddressInt, 16, 99, true, true, false, false, false, true, true, true);
                foreach (Operation ope in opsResult)
                {
                    if (ope == null) break;
                    switch (ope.OriginalInstruction.ToLower())
                    {
                        case "an2b":    // &=
                        case "ad2b":
                        case "orrb":
                        case "ldb":
                            if (ioTable.SignOutputCondRegister == ope.OriginalOpArr[ope.OriginalOpArr.Length - 1])
                            {
                                ioTable.setSignedOutput(Convert.ToByte(ope.OriginalOpArr[1], 16));
                            }
                            break;

                        case "stb":
                            if (ioTable.SignOutputCondRegister == ope.OriginalOpArr[1])
                            {
                                ioTable.setSignedOutput(Convert.ToByte(ope.OriginalOpArr[ope.OriginalOpArr.Length - 1], 16));
                            }
                            break;
                    }
                    if (ioTable.isSignedOutputDefined) break;
                }
                opsResult = null;
            }

            // Core Routines are stored
            foreach (Routine rcRc in slCoreRoutines.Values)
            {
                if (!Calibration.slRoutines.ContainsKey(rcRc.UniqueAddress)) Calibration.slRoutines.Add(rcRc.UniqueAddress, rcRc);
            }

            ioTable = null;
            slCoreRoutines = null;
        }

        // Sign Register Identification
        private void identifyCallTypeFunctionCoreDetails(ref Routine rRoutine, bool functionByte)
        {
            RoutineIOFunction ioFunction = null;
            Operation[] opsResult = null;
            bool signCondMode = false;
            bool signDirectMode = false;

            // Single RoutineIOTable
            ioFunction = (RoutineIOFunction)rRoutine.IOs[0];

            // Sign Conditions identification from Routine
            ioFunction.FirstCondRegisterOpeAddress = -1;
            ioFunction.SignOutputCondRegister = string.Empty;
            ioFunction.FunctionSignInputCondRegister = string.Empty;

            opsResult = getFollowingOPs(rRoutine.AddressInt, 48, 99, true, true, false, false, false, false, false, false);
            foreach (Operation ope in opsResult)
            {
                if (ope == null) break;
                switch (ope.OriginalInstruction.ToLower())
                {
                    case "jge":
                        if (!signCondMode)
                        {
                            signDirectMode = true;
                            if (!ioFunction.isFunctionSignedInputDefined)
                            {
                                ioFunction.FunctionSignedInput = true;
                                ioFunction.isFunctionSignedInputDefined = true;
                            }
                            else
                            {
                                ioFunction.SignedOutput = true;
                                ioFunction.isSignedOutputDefined = true;
                            }
                        }
                        break;
                    case "jc":
                        if (!signCondMode)
                        {
                            signDirectMode = true;
                            if (!ioFunction.isFunctionSignedInputDefined)
                            {
                                ioFunction.FunctionSignedInput = false;
                                ioFunction.isFunctionSignedInputDefined = true;
                            }
                            else
                            {
                                ioFunction.SignedOutput = false;
                                ioFunction.isSignedOutputDefined = true;
                            }
                        }
                        break;
                    case "jnb":
                        if (!signDirectMode)
                        {
                            signCondMode = true;
                            if (ioFunction.FunctionSignInputCondRegister == string.Empty)
                            {
                                ioFunction.FirstCondRegisterOpeAddress = ope.AddressInt;
                                ioFunction.FunctionSignInputCondRegister = ope.OriginalOpArr[1];
                                ioFunction.FunctionSignInputCondBit = Convert.ToInt32(ope.OriginalOpArr[0], 16) - 0x30;
                                ioFunction.FunctionSignedInputCondValue = 0;
                                ioFunction.FunctionUnSignedInputCondValue = 1;
                            }
                            else
                            {
                                ioFunction.SignOutputCondRegister = ope.OriginalOpArr[1];
                                ioFunction.SignOutputCondBit = Convert.ToInt32(ope.OriginalOpArr[0], 16) - 0x30;
                                ioFunction.SignedOutputCondValue = 0;
                                ioFunction.UnSignedOutputCondValue = 1;
                            }
                        }
                        break;
                    case "jb":
                        if (!signDirectMode)
                        {
                            signCondMode = true;
                            if (ioFunction.FunctionSignInputCondRegister == string.Empty)
                            {
                                ioFunction.FirstCondRegisterOpeAddress = ope.AddressInt;
                                ioFunction.FunctionSignInputCondRegister = ope.OriginalOpArr[1];
                                ioFunction.FunctionSignInputCondBit = Convert.ToInt32(ope.OriginalOpArr[0], 16) - 0x38;
                                ioFunction.FunctionSignedInputCondValue = 1;
                                ioFunction.FunctionUnSignedInputCondValue = 0;
                            }
                            else
                            {
                                ioFunction.SignOutputCondRegister = ope.OriginalOpArr[1];
                                ioFunction.SignOutputCondBit = Convert.ToInt32(ope.OriginalOpArr[0], 16) - 0x38;
                                ioFunction.SignedOutputCondValue = 1;
                                ioFunction.UnSignedOutputCondValue = 0;
                            }
                        }
                        break;
                }
                if (ioFunction.SignOutputCondRegister != string.Empty && ioFunction.FunctionSignInputCondRegister != string.Empty) break;
                if (ioFunction.isSignedOutputDefined && ioFunction.isFunctionSignedInputDefined) break;
            }
            opsResult = null;

            // Signed / Unsigned Input Identification
            if (ioFunction.SignOutputCondRegister != string.Empty && ioFunction.FunctionSignInputCondRegister != string.Empty)
            {
                byte condValue = 0;
                byte inputCondValue = 0;
                byte outputCondValue = 0;
                int inputCondCount = 0;
                int outputCondCount = 0;
                int opsCount = 0;
                string condReg = string.Empty;
                opsResult = getFollowingOPs(rRoutine.AddressInt, 8, 99, true, true, false, false, false, true, true, true);
                foreach (Operation ope in opsResult)
                {
                    opsCount++;
                    if (ope == null) break;
                    // First Condition Test Value Reached, current condition can be tested
                    if (ope.AddressInt == ioFunction.FirstCondRegisterOpeAddress)
                    {
                        ioFunction.setFunctionSignedInput(inputCondValue);
                        ioFunction.setSignedOutput(outputCondValue);
                        break;
                    }
                    switch (ope.OriginalInstruction.ToLower())
                    {
                        case "an2b":    // &=
                        case "ad2b":
                        case "orrb":
                        case "ldb":
                            condReg = ope.OriginalOpArr[ope.OriginalOpArr.Length - 1];
                            condValue = Convert.ToByte(ope.OriginalOpArr[1], 16);
                            if (ioFunction.FunctionSignInputCondRegister == condReg)
                            {
                                inputCondCount++;
                                inputCondValue += condValue;
                                ioFunction.setFunctionSignedInput(inputCondValue);
                            }
                            if (ioFunction.SignOutputCondRegister == condReg)
                            {
                                outputCondCount++;
                                outputCondValue += condValue;
                                ioFunction.setSignedOutput(outputCondValue);
                            }
                            break;
                        case "stb":
                            condReg = ope.OriginalOpArr[1];
                            condValue = Convert.ToByte(ope.OriginalOpArr[ope.OriginalOpArr.Length - 1], 16);
                            if (ioFunction.FunctionSignInputCondRegister == condReg)
                            {
                                ioFunction.setFunctionSignedInput(condValue);
                                inputCondCount = 2;
                            }
                            if (ioFunction.SignOutputCondRegister == condReg)
                            {
                                ioFunction.setSignedOutput(condValue);
                                outputCondCount = 2;
                            }
                            break;
                    }
                    if (opsCount >= 8 || (inputCondCount >= 2 && outputCondCount >= 2)) break;
                }
                opsResult = null;
            }

            ioFunction = null;
        }


        //  Creates Routines when Ope, somewhere in unknown Call has been identified
        private void identifyRoutineCode(ref Call cCall)
        {
            ArrayList alRemoval = new ArrayList();

            if (cCall.isIdentified) return;

            // Object[] RoutineCode, BankNum, Ope.AddressInt
            for (int iRco = 0; iRco < Calibration.alRoutinesCodesOpsAddresses.Count; iRco++)
            {
                object[] rcoRco = (object[])Calibration.alRoutinesCodesOpsAddresses[iRco];

                // Basic Checks to limit impact
                bool validSearch = true;
                if (validSearch) validSearch &= ((int)rcoRco[1] == Num && cCall.BankNum == Num);
                if (validSearch) validSearch &= ((int)rcoRco[2] >= cCall.AddressInt);
                if (validSearch) validSearch &= ((int)rcoRco[2] - cCall.AddressInt < 100);
                if (validSearch) validSearch &= slOPs.ContainsKey(Tools.UniqueAddress((int)rcoRco[1], (int)rcoRco[2]));

                if (validSearch)
                {
                    Call resCall = getPrecedingMainCall((int)rcoRco[2]);
                    if (resCall != null)
                    {
                        if (resCall.UniqueAddress == cCall.UniqueAddress)
                        {
                            cCall.isRoutine = true;
                            Calibration.slCalls[cCall.UniqueAddress] = cCall;

                            if (!Calibration.slRoutines.ContainsKey(cCall.UniqueAddress))
                            {
                                Routine rRoutine = new Routine(cCall.BankNum, cCall.AddressInt);
                                rRoutine.Code = (RoutineCode)rcoRco[0];
                                rRoutine.SetTranslationComments();
                                Calibration.slRoutines.Add(rRoutine.UniqueAddress, rRoutine);
                                rRoutine = null;
                                alRemoval.Add(rcoRco);
                            }
                            break;
                        }
                        resCall = null;
                    }
                }

                rcoRco = null;
            }

            foreach (object[] rcoRco in alRemoval) Calibration.alRoutinesCodesOpsAddresses.Remove(rcoRco);

            alRemoval = null;
        }

        public void findAdditionalCalibrationElements(ref SADS6x S6x)
        {
            // Specific cases without RBase, Function & Tables
            // R36 = 5a12;          
            // R38 = R9c;           
            // UUByteLU();          
            foreach (string uniqueAddress in alCallOPsUniqueAddresses)
            {
                Operation callOpe = (Operation)slOPs[uniqueAddress];

                if (callOpe == null) continue;

                if (callOpe.alCalibrationElems != null)
                {
                    callOpe = null;
                    continue;
                }

                Routine rRoutine = (Routine)Calibration.slRoutines[Tools.UniqueAddress(callOpe.ApplyOnBankNum, callOpe.AddressJumpInt)];

                if (rRoutine == null) continue;

                if (rRoutine.Type == RoutineType.Unknown || rRoutine.IOs == null)
                {
                    rRoutine = null;
                    continue;
                }

                // Parsing all Routine Inputs Outputs
                foreach (RoutineIO ioIO in rRoutine.IOs)
                {
                    // 8 Ops Direct Analysis
                    //Operation[] precedingOperations = getPrecedingOPs(callOpe.AddressInt, 8, 99, false, true, false, false, false, false, false, false);
                    Operation[] precedingOperations = getPrecedingOPs(callOpe.AddressInt, 8, 0);
                    foreach (Operation opResult in precedingOperations)
                    {
                        int iAddress = -1;

                        if (opResult == null) break;
                        if (opResult.alCalibrationElems != null) continue;

                        //20180428
                        //if (opResult.TranslatedParams.Length != 2) continue;
                        if (opResult.OperationParams.Length != 2) continue;

                        //20180428
                        //if (opResult.TranslatedParams[opResult.TranslatedParams.Length - 1] != Tools.RegisterInstruction(ioIO.AddressRegister)) continue;
                        if (opResult.OperationParams[opResult.OperationParams.Length - 1].CalculatedParam != Tools.RegisterInstruction(ioIO.AddressRegister)) continue;

                        //20180428
                        //try { iAddress = Convert.ToInt32(opResult.TranslatedParams[0], 16) - SADDef.EecBankStartAddress; }
                        try { iAddress = Convert.ToInt32(opResult.OperationParams[0].CalculatedParam, 16) - SADDef.EecBankStartAddress; }
                        catch { iAddress = -1; }

                        if (iAddress == -1) continue;

                        // Calibration Element - Other Elements are processed differently
                        foreach (RBase rBase in Calibration.slRbases.Values)
                        {
                            RoutineIOTable ioTable = null;
                            RoutineIOFunction ioFunction = null;
                            RoutineIOStructure ioStructure = null;
                            RoutineIOScalar ioScalar = null;
                            
                            if (iAddress >= rBase.AddressBankInt && iAddress <= rBase.AddressBankEndInt)
                            {
                                CalibrationElement calElem = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(Calibration.BankNum, iAddress)];
                                if (calElem == null)
                                {
                                    calElem = new CalibrationElement(Calibration.BankNum, rBase.Code);
                                    calElem.RBaseCalc = opResult.OperationParams[0].DefaultTranslatedParam;
                                    calElem.AddressInt = iAddress;
                                    calElem.AddressBinInt = calElem.AddressInt + Calibration.BankAddressBinInt;
                                    Calibration.slCalibrationElements.Add(calElem.UniqueAddress, calElem);
                                }

                                if (!calElem.RelatedOpsUniqueAddresses.Contains(opResult.UniqueAddress)) calElem.RelatedOpsUniqueAddresses.Add(opResult.UniqueAddress);
                                if (opResult.alCalibrationElems == null) opResult.alCalibrationElems = new ArrayList();
                                opResult.alCalibrationElems.Add(calElem);

                                if (ioIO.GetType() == typeof(RoutineIOTable))
                                // Table Input
                                {
                                    // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                                    if (!S6x.isS6xProcessTypeConflict(calElem.UniqueAddress, typeof(Table)))
                                    {
                                        bool preferPrecedingProcess = true;
                                        ioTable = (RoutineIOTable)ioIO;
                                        processCalibrationElementTable(ref calElem, ref rRoutine, ref ioTable, opResult.AddressInt, callOpe.UniqueAddress, callOpe.AddressInt, ref preferPrecedingProcess);
                                        ioTable = null;
                                    }
                                }
                                else if (ioIO.GetType() == typeof(RoutineIOFunction))
                                // Function Input
                                {
                                    // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                                    if (!S6x.isS6xProcessTypeConflict(calElem.UniqueAddress, typeof(Function)))
                                    {
                                        ioFunction = (RoutineIOFunction)ioIO;
                                        processCalibrationElementFunction(ref calElem, ref rRoutine, ref ioFunction, opResult.AddressInt, callOpe.UniqueAddress);
                                        ioFunction = null;
                                    }
                                }
                                else if (ioIO.GetType() == typeof(RoutineIOStructure))
                                // Structure Input
                                {
                                    if (!S6x.isS6xProcessTypeConflict(calElem.UniqueAddress, typeof(Structure)))
                                    {
                                        ioStructure = (RoutineIOStructure)ioIO;
                                        processCalibrationElementStructure(ref calElem, ref rRoutine, ref ioStructure, opResult.AddressInt, callOpe.UniqueAddress);
                                        ioStructure = null;
                                    }
                                }
                                else if (ioIO.GetType() == typeof(RoutineIOScalar))
                                // Scalar Input
                                {
                                    if (!S6x.isS6xProcessTypeConflict(calElem.UniqueAddress, typeof(Scalar)))
                                    {
                                        ioScalar = (RoutineIOScalar)ioIO;
                                        processCalibrationElementScalar(ref calElem, ref rRoutine, ref ioScalar, opResult.AddressInt, callOpe.UniqueAddress);
                                        ioScalar = null;
                                    }
                                }
                                break;
                            }
                        }

                        if (opResult.alCalibrationElems != null)
                        {
                            opResult.OtherElemAddress = string.Empty;
                            opResult.KnownElemAddress = string.Empty;
                            if (alPossibleOtherElemOPsUniqueAddresses.Contains(opResult.UniqueAddress)) alPossibleOtherElemOPsUniqueAddresses.Remove(opResult.UniqueAddress);
                            if (alPossibleKnownElemOPsUniqueAddresses.Contains(opResult.UniqueAddress)) alPossibleKnownElemOPsUniqueAddresses.Remove(opResult.UniqueAddress);
                            if (!alCalibElemOPsUniqueAddresses.Contains(opResult.UniqueAddress)) alCalibElemOPsUniqueAddresses.Add(opResult.UniqueAddress);
                            break;
                        }
                    }
                    precedingOperations = null;
                }
                callOpe = null;
            }

            // Specific case RBase Calibration Structure
            // 65,ad,07,34          ad2w  R34,7ad            R34 += 7ad;          
            // 64,84,34             ad2w  R34,R84            R34 += R84;          
            // b2,34,30             ldb   R30,[R34]          R30 = [R34];         // R34 is a structure address 
            //
            // 45,fb,00,30,48       ad3w  R48,R30,fb         R48 = R30 + fb;      
            // 64,82,48             ad2w  R48,R82            R48 += R82;          
            // b2,48,34             ldb   R34,[R48]          R34 = [R48];         // R48 is a structure address 
            //
            // a0,R84,R34           ldw   R34,R84            R34 = R84;
            // 65,ad,07,34          ad2w  R34,7ad            R34 += 7ad;
            //                                               R34 += ...;
            // 9a,34,30             cmpb  R30,[R34]                               // R34 is a structure address 
            
            foreach (string uniqueAddress in alPossibleCalElemRBaseStructUniqueAddresses)
            {
                Operation rbOpe = (Operation)slOPs[uniqueAddress];
                Operation rbAdderOpe = null;

                if (rbOpe == null) continue;

                if (rbOpe.alCalibrationElems != null) continue;

                string regCpy = rbOpe.OriginalOpArr[rbOpe.OriginalOpArr.Length - 1];
                bool regUsedAsPointer = false;
                bool regUsedAsByte = true;
                bool regUsedSigned = false;

                // 1. 64 - Search Down and Up before use of Pointer on regCpy for constant adder
                //    a0 - Search Down before use of Pointer on regCpy for constant adder
                // 2. Validate use of Pointer on regCpy for creating Structure and identify Byte/Word, Signed or Not

                Operation[] followingOperations = getFollowingOPs(rbOpe.AddressInt, 8, 0, false, false, false, false, false, false, false, false);
                for (int iPos = 0; iPos < followingOperations.Length; iPos++)
                {
                    Operation ope = followingOperations[iPos];
                    if (ope == null) break;
                    if (ope.isReturn) break;

                    switch (ope.OriginalOPCode.ToLower())
                    {
                        case "a0":
                            // Reg Cpy is copied in another register
                            // Reg Cpy is replaced
                            if (ope.OriginalOpArr[1] == regCpy)
                            {
                                regCpy = ope.OriginalOpArr[ope.OriginalOpArr.Length - 1];
                                continue;
                            }
                            break;
                        case "65":
                            if (ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] != regCpy) continue;
                            try
                            {
                                int iTemp = Convert.ToInt32(ope.OperationParams[0].CalculatedParam, 16);
                                rbAdderOpe = ope;
                            }
                            catch { }
                            break;
                        default:
                            if (!regUsedAsPointer && ope.OperationParams.Length >= 1)
                            {
                                if (Tools.PointerTranslation(Tools.RegisterInstruction(regCpy)) == ope.OperationParams[0].InstructedParam)
                                {
                                    object[] arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                                    // No Register to manage
                                    regUsedAsPointer = ((bool)arrPointersValues[0]);
                                    arrPointersValues = null;

                                    if (regUsedAsPointer)
                                    {
                                        SADOPCode opCode = (SADOPCode)Calibration.slOPCodes[ope.OriginalOPCode];
                                        if (opCode != null)
                                        {
                                            switch (opCode.Type)
                                            {
                                                case OPCodeType.WordOP:
                                                    regUsedAsByte = false;
                                                    break;
                                            }
                                            switch (opCode.Instruction.ToLower())
                                            {
                                                case "ldsbw":
                                                    regUsedSigned = true;
                                                    break;
                                            }
                                        }
                                        if (ope.OriginalOpArr[0].ToLower() == "fe")
                                        {
                                            regUsedSigned = true;
                                        }
                                    }
                                }
                            }
                            break;
                    }

                    //if (regUsedAsPointer && rbAdderOpe != null) break;
                    if (regUsedAsPointer) break;
                }
                followingOperations = null;

                if (!regUsedAsPointer) continue;

                if (rbAdderOpe == null && rbOpe.OriginalOPCode.ToLower() == "64")
                {
                    regCpy = rbOpe.OriginalOpArr[rbOpe.OriginalOpArr.Length - 1];
                    //Operation[] precedingOperations = getPrecedingOPs(rbOpe.AddressInt, 8, 0, false, false, false, false, false, false, false, false);
                    Operation[] precedingOperations = getPrecedingOPs(rbOpe.AddressInt, 8, 0);
                    for (int iPos = 0; iPos < precedingOperations.Length; iPos++)
                    {
                        Operation ope = precedingOperations[iPos];
                        if (ope == null) break;
                        if (ope.isReturn) break;

                        switch (ope.OriginalOPCode.ToLower())
                        {
                            case "65":
                            case "45":
                                if (ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] != regCpy) continue;
                                try
                                {
                                    int iTemp = Convert.ToInt32(ope.OperationParams[0].CalculatedParam, 16);
                                    rbAdderOpe = ope;
                                }
                                catch { }
                                break;
                            default:
                                break;
                        }

                        if (rbAdderOpe != null) break;
                    }
                    precedingOperations = null;
                }

                if (rbAdderOpe == null) continue;
                if (rbAdderOpe.alCalibrationElems != null) continue;

                int iAddress = ((RBase)Calibration.slRbases[rbOpe.CalElemRBaseStructRBase]).AddressBankInt + Convert.ToInt32(rbAdderOpe.OperationParams[0].CalculatedParam, 16);
                if (!Calibration.isCalibrationAddress(iAddress)) continue;

                CalibrationElement calElem = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(Calibration.BankNum, iAddress)];
                if (calElem == null)
                {
                    calElem = new CalibrationElement(Calibration.BankNum, rbOpe.CalElemRBaseStructRBase);
                    calElem.RBaseCalc = rbOpe.OperationParams[0].InstructedParam + SADDef.AdditionSeparator + rbAdderOpe.OperationParams[0].CalculatedParam;
                    calElem.AddressInt = iAddress;
                    calElem.AddressBinInt = iAddress + Calibration.BankAddressBinInt;
                    Calibration.slCalibrationElements.Add(calElem.UniqueAddress, calElem);

                }
                calElem.RelatedOpsUniqueAddresses.Add(rbAdderOpe.UniqueAddress);

                if (S6x.slProcessScalars.ContainsKey(calElem.UniqueAddress))
                {
                    calElem.ScalarElem = new Scalar((S6xScalar)S6x.slProcessScalars[calElem.UniqueAddress]);
                    calElem.ScalarElem.AddressBinInt = calElem.AddressBinInt;
                    calElem.ScalarElem.RBase = calElem.RBase;
                    calElem.ScalarElem.RBaseCalc = calElem.RBaseCalc;
                }
                else if (S6x.slProcessFunctions.ContainsKey(calElem.UniqueAddress))
                {
                    calElem.FunctionElem = new Function((S6xFunction)S6x.slProcessFunctions[calElem.UniqueAddress]);
                    calElem.FunctionElem.AddressBinInt = calElem.AddressBinInt;
                    calElem.FunctionElem.RBase = calElem.RBase;
                    calElem.FunctionElem.RBaseCalc = calElem.RBaseCalc;
                }
                else if (S6x.slProcessTables.ContainsKey(calElem.UniqueAddress))
                {
                    calElem.TableElem = new Table((S6xTable)S6x.slProcessTables[calElem.UniqueAddress]);
                    calElem.TableElem.AddressBinInt = calElem.AddressBinInt;
                    calElem.TableElem.RBase = calElem.RBase;
                    calElem.TableElem.RBaseCalc = calElem.RBaseCalc;
                }
                else if (S6x.slProcessStructures.ContainsKey(calElem.UniqueAddress))
                {
                    calElem.StructureElem = new Structure((S6xStructure)S6x.slProcessStructures[calElem.UniqueAddress]);
                    calElem.StructureElem.AddressBinInt = calElem.AddressBinInt;
                    calElem.StructureElem.RBase = calElem.RBase;
                    calElem.StructureElem.RBaseCalc = calElem.RBaseCalc;
                }
                else
                {
                    calElem.StructureElem = new Structure();
                    calElem.StructureElem.BankNum = calElem.BankNum;
                    calElem.StructureElem.AddressInt = calElem.AddressInt;
                    calElem.StructureElem.AddressBinInt = calElem.AddressBinInt;
                    calElem.StructureElem.Defaulted = true;
                    calElem.StructureElem.Number = 1;
                    calElem.StructureElem.RBase = calElem.RBase;
                    calElem.StructureElem.RBaseCalc = calElem.RBaseCalc;
                    string structDef = string.Empty;
                    if (regUsedAsByte) structDef = "Byte";
                    else structDef = "Word";
                    if (regUsedSigned) structDef = "S" + structDef;
                    calElem.StructureElem.StructDefString = structDef;
                }

                if (!alCalibElemOPsUniqueAddresses.Contains(rbAdderOpe.UniqueAddress)) alCalibElemOPsUniqueAddresses.Add(rbAdderOpe.UniqueAddress);
                rbAdderOpe.alCalibrationElems = new ArrayList();
                rbAdderOpe.alCalibrationElems.Add(calElem);
                rbAdderOpe.CalElemRBaseStructAdder = true;
            }
        }

        public void processCalibrationElements(ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            Operation ope = null;
            CalibrationElement calibrationElem;
            Operation[] followingOperations = null;
            RoutineIO ioGeneric = null;
            RoutineIOTable ioTable = null;
            RoutineIOFunction ioFunction = null;
            RoutineIOStructure ioStructure = null;
            RoutineIOScalar ioScalar = null;
            bool bAddCalElem = false;

            bool preferPrecedingProcess = true;
            int preferPrecedingProcessCount = -1;
            int preferFollowingProcessCount = -1;

            foreach (string uniqueAddress in alCalibElemOPsUniqueAddresses)
            {
                ope = (Operation)slOPs[uniqueAddress];
                if (ope.alCalibrationElems == null) continue;

                foreach (CalibrationElement opeCalElem in ope.alCalibrationElems)
                {
                    bAddCalElem = false;

                    calibrationElem = null;
                    if (Calibration.slCalibrationElements.ContainsKey(Tools.UniqueAddress(opeCalElem.BankNum, opeCalElem.AddressInt)))
                    {
                        calibrationElem = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(opeCalElem.BankNum, opeCalElem.AddressInt)];
                    }
                    else
                    {
                        // Normally it should never be
                        //  Addition to Operation.alCalibrationElems should always be done after creation in Calibration.slCalibrationElements
                        calibrationElem = opeCalElem;
                        bAddCalElem = true;
                    }

                    if (!calibrationElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) calibrationElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);

                    if (!calibrationElem.isFullyIdentified)
                    {
                        if (!calibrationElem.isTypeIdentified)
                        // Table / Function Identification - Scalars are preidentified except on Other Routines
                        {
                            //Automatic detect
                            for (int iRoutine = 0; iRoutine < Calibration.slRoutines.Count; iRoutine++)
                            {
                                Routine rRoutine = (Routine)Calibration.slRoutines.GetByIndex(iRoutine);
                                if (rRoutine.Type != RoutineType.Unknown && rRoutine.IOs != null)
                                {
                                    if (ope.CallType == CallType.Call || ope.CallType == CallType.ShortCall)
                                    // Args Call Mngt
                                    {
                                        if (ope.AddressJumpInt == rRoutine.AddressInt && ope.ApplyOnBankNum == rRoutine.BankNum)
                                        {
                                            ioGeneric = null;
                                            // Arguments / Inputs Outputs matching
                                            if (ope.CallArguments != null)
                                            {
                                                foreach (CallArgument opeArg in ope.CallArguments)
                                                {
                                                    foreach (RoutineIO ioIO in rRoutine.IOs)
                                                    {
                                                        if (opeArg.OutputRegisterAddress == ioIO.AddressRegister)
                                                        {
                                                            ioGeneric = ioIO;
                                                            break;
                                                        }
                                                    }
                                                    if (ioGeneric != null) break;
                                                }
                                            }
                                            if (ioGeneric == null)
                                            // Default Mngt
                                            {
                                                if (rRoutine.IOs.Length > 0) ioGeneric = rRoutine.IOs[0];
                                            }

                                            if (ioGeneric != null)
                                            {
                                                if (ioGeneric.GetType() == typeof(RoutineIOStructure))
                                                {
                                                    if (S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Structure))) break;

                                                    ioStructure = (RoutineIOStructure)ioGeneric;
                                                    processCalibrationElementStructure(ref calibrationElem, ref rRoutine, ref ioStructure, ope.AddressInt, ope.UniqueAddress);
                                                    ioStructure = null;
                                                }
                                                else if (ioGeneric.GetType() == typeof(RoutineIOTable))
                                                {
                                                    if (S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Table))) break;

                                                    ioTable = (RoutineIOTable)ioGeneric;
                                                    processCalibrationElementTable(ref calibrationElem, ref rRoutine, ref ioTable, ope.AddressInt, ope.UniqueAddress, ope.AddressInt, ref preferPrecedingProcess);
                                                    ioTable = null;
                                                }
                                                else if (ioGeneric.GetType() == typeof(RoutineIOFunction))
                                                {
                                                    if (S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Function))) break;

                                                    ioFunction = (RoutineIOFunction)ioGeneric;
                                                    processCalibrationElementFunction(ref calibrationElem, ref rRoutine, ref ioFunction, ope.AddressInt, ope.UniqueAddress);
                                                    ioFunction = null;
                                                }
                                                else if (ioGeneric.GetType() == typeof(RoutineIOScalar))
                                                {
                                                    if (S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Scalar))) break;

                                                    ioScalar = (RoutineIOScalar)ioGeneric;
                                                    processCalibrationElementScalar(ref calibrationElem, ref rRoutine, ref ioScalar, ope.AddressInt, ope.UniqueAddress);
                                                    ioScalar = null;
                                                }
                                                else
                                                {
                                                    if (S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Scalar))) break;

                                                    processCalibrationElementDefault(ref calibrationElem, ref rRoutine, ref ioGeneric, ope.AddressInt, ope.UniqueAddress);
                                                }
                                            }

                                            /*
                                            switch (rRoutine.Type)
                                            {
                                                // Standard Function Reader Call with Function Address and Input
                                                case RoutineType.FunctionWord:
                                                case RoutineType.FunctionByte:
                                                    // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                                                    if (S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Function)))
                                                    {
                                                        rRoutine = null;        // to break;
                                                    }
                                                    else
                                                    {
                                                        ioFunction = (RoutineIOFunction)rRoutine.IOs[0];
                                                        processCalibrationElementFunction(ref calibrationElem, ref rRoutine, ref ioFunction, ope.AddressInt);
                                                        ioFunction = null;
                                                        bUpdateCalElem = true;
                                                    }
                                                    break;
                                                // Other Routine with Encrypted Elements
                                                case RoutineType.Other:
                                                    if (ope.CallArguments != null)
                                                    {
                                                        foreach (CallArgument opeArg in ope.CallArguments)
                                                        {
                                                            if (opeArg.DecryptedValue == calibrationElem.Address)
                                                            {
                                                                processCalibrationElementOtherRoutine(ref calibrationElem, opeArg, ref rRoutine, ope.AddressInt);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    break;
                                            }
                                            */
                                        }
                                    }
                                    else
                                    // Standard Identification
                                    {
                                        // Parsing all Routine Inputs Outputs
                                        foreach (RoutineIO ioIO in rRoutine.IOs)
                                        {
                                            if (ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] != ioIO.AddressRegister) continue;

                                            // 8 Ops Analysis
                                            followingOperations = getFollowingOPs(ope.AddressNextInt, 8, 99, true, true, true, true, true, true, true, true);
                                            foreach (Operation followingOpResult in followingOperations)
                                            {
                                                if (followingOpResult == null) break;

                                                if (followingOpResult.AddressJumpInt == rRoutine.AddressInt && followingOpResult.ApplyOnBankNum == rRoutine.BankNum)
                                                {
                                                    if (ioIO.GetType() == typeof(RoutineIOStructure))
                                                    {
                                                        if (S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Structure)))
                                                        {
                                                            rRoutine = null;        // to break;
                                                        }
                                                        else
                                                        {
                                                            ioStructure = (RoutineIOStructure)ioIO;
                                                            processCalibrationElementStructure(ref calibrationElem, ref rRoutine, ref ioStructure, ope.AddressInt, followingOpResult.UniqueAddress);
                                                            ioStructure = null;
                                                        }
                                                    }
                                                    else if (ioIO.GetType() == typeof(RoutineIOTable))
                                                    {
                                                        if (S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Table)))
                                                        {
                                                            rRoutine = null;        // to break;
                                                        }
                                                        else
                                                        {
                                                            preferPrecedingProcess = (preferPrecedingProcessCount > preferFollowingProcessCount);

                                                            ioTable = (RoutineIOTable)ioIO;
                                                            processCalibrationElementTable(ref calibrationElem, ref rRoutine, ref ioTable, ope.AddressInt, followingOpResult.UniqueAddress, ope.AddressInt, ref preferPrecedingProcess);
                                                            ioTable = null;

                                                            if (preferPrecedingProcess) preferPrecedingProcessCount++;
                                                            else preferFollowingProcessCount++;
                                                        }
                                                    }
                                                    else if (ioIO.GetType() == typeof(RoutineIOFunction))
                                                    {
                                                        if (S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Function)))
                                                        {
                                                            rRoutine = null;        // to break;
                                                        }
                                                        else
                                                        {
                                                            ioFunction = (RoutineIOFunction)ioIO;
                                                            processCalibrationElementFunction(ref calibrationElem, ref rRoutine, ref ioFunction, ope.AddressInt, followingOpResult.UniqueAddress);
                                                            ioFunction = null;
                                                        }
                                                    }
                                                    else if (ioIO.GetType() == typeof(RoutineIOScalar))
                                                    {
                                                        if (S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Scalar)))
                                                        {
                                                            rRoutine = null;        // to break;
                                                        }
                                                        else
                                                        {
                                                            ioScalar = (RoutineIOScalar)ioIO;
                                                            processCalibrationElementScalar(ref calibrationElem, ref rRoutine, ref ioScalar, ope.AddressInt, followingOpResult.UniqueAddress);
                                                            ioScalar = null;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ioGeneric = ioIO;
                                                        processCalibrationElementDefault(ref calibrationElem, ref rRoutine, ref ioGeneric, ope.AddressInt, followingOpResult.UniqueAddress);
                                                        ioGeneric = null;
                                                    }
                                                    break;
                                                }
                                            }
                                            followingOperations = null;
                                        }
                                    }
                                    if (rRoutine == null) break;
                                }
                                rRoutine = null;
                                if (calibrationElem.isFullyIdentified) break;
                            }
                        }

                        // Table / Function Identification not directly using Table / Function Address Registers
                        if (!calibrationElem.isTypeIdentified) processCalibrationElementSub(ref ope, ref calibrationElem, ref S6x);

                        // To Know if Element is coming from S6x Definition and if it will be necessary to process it with Signatures
                        bool hasS6xDefinition = false;

                        // Identification using S6x Definition, when Type was in Conflict or nothing was detected
                        if (!calibrationElem.isTypeIdentified)
                        {
                            if (S6x.slProcessTables.ContainsKey(calibrationElem.UniqueAddress)) calibrationElem.TableElem = new Table((S6xTable)S6x.slProcessTables[calibrationElem.UniqueAddress]);
                            else if (S6x.slProcessFunctions.ContainsKey(calibrationElem.UniqueAddress)) calibrationElem.FunctionElem = new Function((S6xFunction)S6x.slProcessFunctions[calibrationElem.UniqueAddress]);
                            else if (S6x.slProcessScalars.ContainsKey(calibrationElem.UniqueAddress)) calibrationElem.ScalarElem = new Scalar((S6xScalar)S6x.slProcessScalars[calibrationElem.UniqueAddress]);
                            else if (S6x.slProcessStructures.ContainsKey(calibrationElem.UniqueAddress)) calibrationElem.StructureElem = new Structure((S6xStructure)S6x.slProcessStructures[calibrationElem.UniqueAddress]);

                            hasS6xDefinition = calibrationElem.isTypeIdentified;
                        }

                        // 20181106 - Process is added for MAF Transfer essentially
                        // Last Non Identified Elements will be parsed through predefined signatures
                        if (!hasS6xDefinition) processCalibrationElementSignatureIdentification(ref calibrationElem, ref ope, ref S6x);

                        // Last Non Identified Elements will be identified as Structures
                        //  They are coming from Op Code 45 essentially to be used later on, no way to know their usage now
                        if (!calibrationElem.isTypeIdentified)
                        {
                            // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                            //      Checksum Start Address is detected as structure in Checksum calculation routine, it is not required and can generate output issues
                            if (!S6x.isS6xProcessTypeConflict(calibrationElem.UniqueAddress, typeof(Structure)) && calibrationElem.AddressInt != 0)
                            {
                                calibrationElem.StructureElem = new Structure();
                                calibrationElem.StructureElem.BankNum = calibrationElem.BankNum;
                                calibrationElem.StructureElem.AddressInt = calibrationElem.AddressInt;
                                calibrationElem.StructureElem.AddressBinInt = calibrationElem.AddressBinInt;
                                calibrationElem.StructureElem.RBase = calibrationElem.RBase;
                                calibrationElem.StructureElem.RBaseCalc = calibrationElem.RBaseCalc;

                                // Defaulted
                                calibrationElem.StructureElem.Defaulted = true;
                                calibrationElem.StructureElem.StructDefString = "ByteHex";
                                calibrationElem.StructureElem.Number = 1;
                            }
                        }

                        if (calibrationElem.isTypeIdentified && !calibrationElem.isFullyIdentified)
                        //      => Structure : Structure Definition, Number
                        //      => Scalar : Signed / Unsigned, Byte or Word, Bit Flags
                        {
                            if (calibrationElem.isStructure)
                            // Structure
                            {
                                // Not Managed for now

                                // Find Related Op
                                // Follow it to Find Pointer to Initial Register
                                //ad3w  R32,R8a,f          R32 = 3e95;          
                                //ad2w  R32,R38            R32 += R38;          
                                //ad2b  R34,[R32]          R34 += [R32];        

                                // To Be Managed
                            }
                            else if (calibrationElem.isScalar)
                            // Scalar
                            {
                                processCalibrationElementScalarDetails(ref calibrationElem, ref ope);
                            }
                        }
                    }

                    if (bAddCalElem) Calibration.slCalibrationElements.Add(calibrationElem.UniqueAddress, calibrationElem);
                }
                ope = null;
            }

            // Structures Analysis for Autodetected and Defaulted Structures
            //  Requires Multiples Steps - A Dedicated function is used
            processCalibrationElementStructuresAnalysis(ref Bank0, ref Bank1, ref Bank8, ref Bank9);
        }

        // Structures Analysis for Autodetected and Defaulted Structures
        //  Requires Multiples Steps - A Dedicated function is used
        public void processCalibrationElementStructuresAnalysis(ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9)
        {
            // First Step - Create an Analysis for each structure / each operation using this structure
            foreach (CalibrationElement calElem in Calibration.slCalibrationElements.Values)
            {
                if (!calElem.isStructure) continue;

                Structure sStruct = calElem.StructureElem;
                if (sStruct == null) continue;

                // Only on auto detected structures
                if (sStruct.S6xStructure != null || !sStruct.Defaulted) continue;

                foreach (string opeUniqueAddress in calElem.RelatedOpsUniqueAddresses)
                {
                    // Structure Discovery
                    Operation rOpe = null;
                    if (Bank8 != null && rOpe == null) rOpe = (Operation)Bank8.slOPs[opeUniqueAddress];
                    if (Bank1 != null && rOpe == null) rOpe = (Operation)Bank1.slOPs[opeUniqueAddress];
                    if (Bank9 != null && rOpe == null) rOpe = (Operation)Bank9.slOPs[opeUniqueAddress];
                    if (Bank0 != null && rOpe == null) rOpe = (Operation)Bank0.slOPs[opeUniqueAddress];
                    if (rOpe == null) continue;

                    switch (rOpe.BankNum)
                    {
                        case 8:
                            Bank8.processStructureAnalysis(sStruct.UniqueAddress, rOpe.UniqueAddress, true, ref Bank0, ref Bank1, ref Bank8, ref Bank9);
                            break;
                        case 1:
                            Bank1.processStructureAnalysis(sStruct.UniqueAddress, rOpe.UniqueAddress, true, ref Bank0, ref Bank1, ref Bank8, ref Bank9);
                            break;
                        case 9:
                            Bank9.processStructureAnalysis(sStruct.UniqueAddress, rOpe.UniqueAddress, true, ref Bank0, ref Bank1, ref Bank8, ref Bank9);
                            break;
                        case 0:
                            Bank0.processStructureAnalysis(sStruct.UniqueAddress, rOpe.UniqueAddress, true, ref Bank0, ref Bank1, ref Bank8, ref Bank9);
                            break;
                    }
                    // Same structure can be processed more than one time to get better results
                }
            }

            // Structure Analysis Mngt and Duplicated mngt based on Corrected Address
            //  Done when all structures have been deeply processed
            ArrayList alMovedStructuresUniqueAddresses = new ArrayList();

            foreach (CalibrationElement calElem in Calibration.slCalibrationElements.Values)
            {
                if (!calElem.isStructure) continue;

                Structure sStruct = calElem.StructureElem;
                if (sStruct == null) continue;

                // Only on auto detected structures
                if (sStruct.S6xStructure != null) continue;

                // Resulted Analysis Management including Address Correction
                StructureAnalysis saSA = (StructureAnalysis)Calibration.slStructuresAnalysis[sStruct.UniqueAddress];
                if (saSA != null)
                {
                    if (saSA.isAdderStructure)
                    // Adder Structure is basic but requires Number Validation by reading it until another item
                    {
                        if (sStruct.ParentStructure == null)
                        // If not already defined as a Child from another identical Parent Structure
                        {
                            // Reset at One, like it is originally defaulted, because can be processed many times
                            sStruct.Number = 1;

                            if (saSA.StructNewAddressInt == -1)
                            {
                                // Adder check on 0 value
                                if (Calibration.isCalElemAddressInConflict(saSA.StructAddressInt, saSA.StructAddressInt))
                                {
                                    saSA.StructNewAddressInt = saSA.StructAddressInt + sStruct.MaxSizeSingle;
                                    if (Calibration.isCalElemAddressInConflict(saSA.StructNewAddressInt, saSA.StructAddressInt))
                                    {
                                        saSA.StructNewAddressInt = -1;
                                        continue;
                                    }
                                }
                            }

                            Structure stAdd = new Structure();
                            stAdd.BankNum = sStruct.BankNum;
                            stAdd.AddressInt = sStruct.AddressInt;
                            if (saSA.StructNewAddressInt > saSA.StructAddressInt) stAdd.AddressInt = saSA.StructNewAddressInt;
                            stAdd.StructDefString = sStruct.StructDefString;
                            stAdd.Number = 1;
                            while (stAdd.AddressInt + stAdd.MaxSizeSingle * stAdd.Number < Calibration.AddressBankEndInt + 1)
                            {
                                // Security Limit - 16 for this type
                                if (sStruct.Number >= 16) break;

                                if (!Calibration.isCalibrationAddress(stAdd.AddressInt)) break;

                                if (stAdd.AddressInt != sStruct.AddressInt)
                                {
                                    // Not a Good Idea - No real way to separate 2 structures with identical definition
                                    /*
                                    if (Calibration.slCalibrationElements.ContainsKey(stAdd.UniqueAddress))
                                    {
                                        if (!(CalibrationElement)Calibration.slCalibrationElements[stAdd.UniqueAddress]).isStructure) break;
                                        if (!(CalibrationElement)Calibration.slCalibrationElements[stAdd.UniqueAddress]).StructureElem == null) break;
                                        // Another Structure with exact same type Could be included
                                        if ((CalibrationElement)Calibration.slCalibrationElements[stAdd.UniqueAddress]).StructureElem.StructDefString != sStruct.StructDefString) break;
                                        if (!Calibration.slStructuresAnalysis.ContainsKey(stAdd.UniqueAddress)) break;
                                        if (!((StructureAnalysis)Calibration.slStructuresAnalysis[stAdd.UniqueAddress]).isAdderStructure) break;
                                        (CalibrationElement)Calibration.slCalibrationElements[stAdd.UniqueAddress]).StructureElem.ParentAddressInt = sStruct.AddressInt;
                                        (CalibrationElement)Calibration.slCalibrationElements[stAdd.UniqueAddress]).StructureElem.ParentStructure = sStruct;
                                    }
                                    */
                                    if (Calibration.slCalibrationElements.ContainsKey(stAdd.UniqueAddress)) break;
                                }

                                if (Calibration.isCalElemAddressInConflict(stAdd.AddressInt, sStruct.AddressInt)) break;

                                string[] arrStAddBytes = Calibration.getBytesArray(stAdd.AddressInt, stAdd.MaxSizeSingle * stAdd.Number);

                                if (arrStAddBytes.Length == 0) break;

                                if (stAdd.AddressInt != sStruct.AddressInt) sStruct.Number++;

                                stAdd.Read(ref arrStAddBytes, stAdd.Number);
                                stAdd.AddressInt += stAdd.Size;
                            }
                            stAdd = null;

                            if (sStruct.Number > 1) sStruct.Defaulted = false;
                            if (saSA.StructNewAddressInt != -1 && saSA.StructNewAddressInt != saSA.StructAddressInt) sStruct.ParentAddressInt = saSA.StructNewAddressInt;
                        }
                    }
                    else
                    // Fully Analysed Structure
                    {
                        if (saSA.isNumberCalculationRequired)
                        {
                            saSA.NumberCalculation();
                        }
                        else if (saSA.isStructureReadRequired)
                        // Based on StructureAnalysis.LoopExitFirstItemValue
                        {
                            saSA.Number = 0;
                            int loopExitFirstItemValue = saSA.LoopExitFirstItemValue;
                            StructureItemAnalysis siaSIA = (StructureItemAnalysis)saSA.slItems[0];
                            if (siaSIA != null)
                            {
                                Structure stTmp = new Structure();
                                stTmp.BankNum = saSA.StructBankNum;
                                stTmp.AddressInt = saSA.StructAddressInt;
                                if (saSA.StructNewAddressInt > saSA.StructAddressInt) stTmp.AddressInt = saSA.StructNewAddressInt;
                                stTmp.StructDefString = saSA.ProposedStructureDefString;
                                stTmp.Number = 1;
                                while (stTmp.AddressInt + stTmp.MaxSizeSingle * stTmp.Number < Calibration.AddressBankEndInt + 1)
                                {
                                    // Security Limit
                                    if (saSA.Number > 512)
                                    {
                                        saSA.Number = -1;
                                        break;
                                    }

                                    if (stTmp.AddressInt != sStruct.AddressInt && Calibration.slCalibrationElements.ContainsKey(stTmp.UniqueAddress))
                                    {
                                        break;
                                    }

                                    if (Calibration.isCalElemAddressInConflict(stTmp.AddressInt, sStruct.AddressInt))
                                    {
                                        saSA.Number = -1;
                                        break;
                                    }
                                    string[] arrStTmpBytes = Calibration.getBytesArray(stTmp.AddressInt, stTmp.MaxSizeSingle * stTmp.Number);
                                    if (arrStTmpBytes.Length == 0)
                                    {
                                        saSA.Number = -1;
                                        break;
                                    }
                                    saSA.Number++;
                                    if (siaSIA.isByte && arrStTmpBytes.Length > 0)
                                    {
                                        if (Convert.ToInt32(arrStTmpBytes[0], 16) == saSA.LoopExitFirstItemValue) break;
                                    }
                                    else if (arrStTmpBytes.Length > 1)
                                    {
                                        if (Convert.ToInt32(arrStTmpBytes[1] + arrStTmpBytes[0], 16) == saSA.LoopExitFirstItemValue) break;
                                    }
                                    stTmp.Read(ref arrStTmpBytes, stTmp.Number);
                                    stTmp.AddressInt += stTmp.Size;
                                }
                                stTmp = null;
                            }
                            if (saSA.Number <= 0)
                            {
                                // Detection Error - Reset of related values
                                saSA.Number = -1;
                                saSA.LoopExitFirstItemValue = int.MinValue;
                                saSA.LoopExitFirstItemOpeAddressInt = -1;
                            }
                        }

                        if (saSA.isValid)
                        {
                            sStruct.Defaulted = false;
                            sStruct.Number = saSA.Number;
                            sStruct.StructDefString = saSA.ProposedStructureDefString;
                            if (saSA.StructNewAddressInt != -1 && saSA.StructNewAddressInt != saSA.StructAddressInt) sStruct.ParentAddressInt = saSA.StructNewAddressInt;
                        }
                    }

                    saSA = null;
                }

                if (sStruct.ParentAddressInt == -1) continue;

                // Corrected structures
                sStruct.ParentStructure = ((CalibrationElement)Calibration.slCalibrationElements[sStruct.ParentUniqueAddress]).StructureElem;
                if (sStruct.ParentStructure == null) alMovedStructuresUniqueAddresses.Add(sStruct.UniqueAddress);
            }

            // Creating Structures based on moved ones
            foreach (string uniqueAddress in alMovedStructuresUniqueAddresses)
            {
                CalibrationElement calElem = (CalibrationElement)Calibration.slCalibrationElements[uniqueAddress];
                Structure sStruct = calElem.StructureElem;

                if (sStruct == null) continue;
                if (sStruct.ParentStructure != null) continue;

                sStruct.ParentStructure = ((CalibrationElement)Calibration.slCalibrationElements[sStruct.ParentUniqueAddress]).StructureElem;
                if (sStruct.ParentStructure == null)
                {
                    Structure newStructure = new Structure();
                    newStructure.BankNum = sStruct.BankNum;
                    newStructure.AddressInt = sStruct.ParentAddressInt;
                    newStructure.RBase = sStruct.RBase;
                    newStructure.RBaseCalc = sStruct.RBaseCalc;
                    newStructure.Defaulted = sStruct.Defaulted;
                    newStructure.Number = sStruct.Number;
                    newStructure.StructDefString = sStruct.StructDefString;

                    CalibrationElement newCalElem = new CalibrationElement(newStructure.BankNum, newStructure.RBase);
                    newCalElem.AddressInt = newStructure.AddressInt;
                    newCalElem.AddressBinInt = newCalElem.AddressInt + Calibration.BankAddressBinInt;
                    newCalElem.RBaseCalc = newStructure.RBaseCalc;
                    newCalElem.RelatedOpsUniqueAddresses = calElem.RelatedOpsUniqueAddresses;
                    newCalElem.StructureElem = newStructure;

                    Calibration.slCalibrationElements.Add(newCalElem.UniqueAddress, newCalElem);
                    sStruct.ParentStructure = ((CalibrationElement)Calibration.slCalibrationElements[sStruct.ParentUniqueAddress]).StructureElem;
                    newStructure = null;
                    newCalElem = null;
                }
            }
            alMovedStructuresUniqueAddresses = null;
        }

        private void processCalibrationElementSignatureIdentification(ref CalibrationElement calibrationElem, ref Operation ope, ref SADS6x S6x)
        {
            S6xElementSignature foundSignature = null;

            foreach (S6xElementSignature elementSignature in S6x.slProcessElementsSignatures.Values)
            {
                if (elementSignature.matchSignature(ref ope, this, is8061))
                {
                    foundSignature = elementSignature;
                    break;
                }
            }

            if (foundSignature == null) return;

            foreach (S6xElementSignature elementSignature in S6x.slProcessElementsSignatures.Values)
            {
                if (elementSignature.Skip || elementSignature.Found || elementSignature.Ignore) continue;
                // Set Element Signature as found, if it is the right one, it will not be processed anymore
                if (foundSignature.UniqueKey == elementSignature.UniqueKey) elementSignature.Found = true;
                // otherwise set Element Signature to be ignore, if it has the same Signature Key (Element Short Label), it will not be processed anymore too.
                else if (foundSignature.SignatureKey == elementSignature.SignatureKey) elementSignature.Ignore = true;
            }

            if (foundSignature.Scalar != null)
            {
                calibrationElem.ScalarElem = new Scalar();
                calibrationElem.ScalarElem.BankNum = calibrationElem.BankNum;
                calibrationElem.ScalarElem.AddressInt = calibrationElem.AddressInt;
                calibrationElem.ScalarElem.AddressBinInt = calibrationElem.AddressBinInt;
                calibrationElem.ScalarElem.RBase = calibrationElem.RBase;
                calibrationElem.ScalarElem.RBaseCalc = calibrationElem.RBaseCalc;
                calibrationElem.ScalarElem.ShortLabel = foundSignature.Scalar.ShortLabel;
                calibrationElem.ScalarElem.Label = foundSignature.Scalar.Label;

                calibrationElem.ScalarElem.S6xElementSignatureSource = foundSignature;

                calibrationElem.ScalarElem.Byte = foundSignature.Scalar.Byte;
                calibrationElem.ScalarElem.Word = !calibrationElem.ScalarElem.Byte;
                calibrationElem.ScalarElem.Signed = foundSignature.Scalar.Signed;
                calibrationElem.ScalarElem.UnSigned = !calibrationElem.ScalarElem.Signed;
                if (foundSignature.Scalar.ScaleExpression != null && foundSignature.Scalar.ScaleExpression != string.Empty)
                {
                    calibrationElem.ScalarElem.ScaleExpression = foundSignature.Scalar.ScaleExpression;
                    calibrationElem.ScalarElem.ScalePrecision = foundSignature.Scalar.ScalePrecision;
                }
            }
            else if (foundSignature.Function != null)
            {
                calibrationElem.FunctionElem = new Function();
                calibrationElem.FunctionElem.BankNum = calibrationElem.BankNum;
                calibrationElem.FunctionElem.AddressInt = calibrationElem.AddressInt;
                calibrationElem.FunctionElem.AddressBinInt = calibrationElem.AddressBinInt;
                calibrationElem.FunctionElem.RBase = calibrationElem.RBase;
                calibrationElem.FunctionElem.RBaseCalc = calibrationElem.RBaseCalc;
                calibrationElem.FunctionElem.ShortLabel = foundSignature.Function.ShortLabel;
                calibrationElem.FunctionElem.Label = foundSignature.Function.Label;

                calibrationElem.FunctionElem.S6xElementSignatureSource = foundSignature;

                calibrationElem.FunctionElem.ByteInput = foundSignature.Function.ByteInput;
                calibrationElem.FunctionElem.ByteOutput = foundSignature.Function.ByteOutput;
                if (foundSignature.Function.InputScaleExpression != null && foundSignature.Function.InputScaleExpression != string.Empty)
                {
                    calibrationElem.FunctionElem.InputScaleExpression = foundSignature.Function.InputScaleExpression;
                    calibrationElem.FunctionElem.InputScalePrecision = foundSignature.Function.InputScalePrecision;
                }
                if (foundSignature.Function.OutputScaleExpression != null && foundSignature.Function.OutputScaleExpression != string.Empty)
                {
                    calibrationElem.FunctionElem.OutputScaleExpression = foundSignature.Function.OutputScaleExpression;
                    calibrationElem.FunctionElem.OutputScalePrecision = foundSignature.Function.OutputScalePrecision;
                }
            }
            else if (foundSignature.Table != null)
            {
                calibrationElem.TableElem = new Table();
                calibrationElem.TableElem.BankNum = calibrationElem.BankNum;
                calibrationElem.TableElem.AddressInt = calibrationElem.AddressInt;
                calibrationElem.TableElem.AddressBinInt = calibrationElem.AddressBinInt;
                calibrationElem.TableElem.RBase = calibrationElem.RBase;
                calibrationElem.TableElem.RBaseCalc = calibrationElem.RBaseCalc;
                calibrationElem.TableElem.ShortLabel = foundSignature.Table.ShortLabel;
                calibrationElem.TableElem.Label = foundSignature.Table.Label;

                calibrationElem.TableElem.S6xElementSignatureSource = foundSignature;

                calibrationElem.TableElem.WordOutput = foundSignature.Table.WordOutput;
                calibrationElem.TableElem.SignedOutput = foundSignature.Table.SignedOutput;

                try
                {
                    calibrationElem.TableElem.ColsNumber = Convert.ToInt32(foundSignature.Table.VariableColsNumber);
                }
                catch { }

                if (foundSignature.Table.CellsScaleExpression != null && foundSignature.Table.CellsScaleExpression != string.Empty)
                {
                    calibrationElem.TableElem.CellsScaleExpression = foundSignature.Table.CellsScaleExpression;
                    calibrationElem.TableElem.CellsScalePrecision = foundSignature.Table.CellsScalePrecision;
                }

            }
            else if (foundSignature.Structure != null)
            {
                calibrationElem.StructureElem = new Structure();
                calibrationElem.StructureElem.BankNum = calibrationElem.BankNum;
                calibrationElem.StructureElem.AddressInt = calibrationElem.AddressInt;
                calibrationElem.StructureElem.AddressBinInt = calibrationElem.AddressBinInt;
                calibrationElem.StructureElem.RBase = calibrationElem.RBase;
                calibrationElem.StructureElem.RBaseCalc = calibrationElem.RBaseCalc;
                calibrationElem.StructureElem.ShortLabel = foundSignature.Structure.ShortLabel;
                calibrationElem.StructureElem.Label = foundSignature.Structure.Label;

                calibrationElem.StructureElem.S6xElementSignatureSource = foundSignature;

                calibrationElem.StructureElem.StructDefString = foundSignature.Structure.StructDef;
                calibrationElem.StructureElem.Number = foundSignature.Structure.Number;
            }

            if (foundSignature.Information == null) foundSignature.Information = string.Empty;
            if (foundSignature.Information != string.Empty) foundSignature.Information += "\r\n";
            foundSignature.Information += "Signature has generated element at address " + calibrationElem.UniqueAddressHex;
            
            foundSignature = null;
        }

        private void processCalibrationElementFunction(ref CalibrationElement calibrationElem, ref Routine rRoutine, ref RoutineIOFunction ioFunction, int calElemOpeAddress, string callOpeUniqueAddress)
        {
            string sValue = string.Empty;
            bool updateRoutine = false;

            calibrationElem.FunctionElem = new Function();
            calibrationElem.FunctionElem.BankNum = calibrationElem.BankNum;
            calibrationElem.FunctionElem.AddressInt = calibrationElem.AddressInt;
            calibrationElem.FunctionElem.AddressBinInt = calibrationElem.AddressBinInt;
            calibrationElem.FunctionElem.RBase = calibrationElem.RBase;
            calibrationElem.FunctionElem.RBaseCalc = calibrationElem.RBaseCalc;

            calibrationElem.FunctionElem.ByteInput = ioFunction.FunctionByte;
            calibrationElem.FunctionElem.ByteOutput = ioFunction.FunctionByte;

            RoutineCallInfoFunction ciCi = new RoutineCallInfoFunction();
            ciCi.CalibrationElementOpeUniqueAddress = Tools.UniqueAddress(Num, calElemOpeAddress);
            ciCi.RoutineUniqueAddress = rRoutine.UniqueAddress;
            ciCi.RoutineCallOpeUniqueAddress = callOpeUniqueAddress;
            ciCi.RoutineInputOutput = ioFunction;
            calibrationElem.FunctionElem.RoutinesCallsInfos.Add(ciCi);
            if (!slRoutineOPsCalibElemUniqueAddresses.ContainsKey(ciCi.RoutineCallOpeUniqueAddress))
            {
                slRoutineOPsCalibElemUniqueAddresses.Add(ciCi.RoutineCallOpeUniqueAddress, calibrationElem.FunctionElem.UniqueAddress);
            }
            ciCi = null;

            // Sign of Input was not identified previously, data will provide information
            if (!ioFunction.isFunctionSignedInputDefined)
            {
                if (calibrationElem.FunctionElem.ByteInput)
                {
                    sValue = Calibration.getByte(calibrationElem.FunctionElem.AddressInt);
                    if (sValue == "7f")
                    {
                        ioFunction.FunctionSignedInput = true;
                        ioFunction.isFunctionSignedInputDefined = true;
                        updateRoutine = true;
                    }
                    else if (sValue == "ff")
                    {
                        ioFunction.FunctionSignedInput = false;
                        ioFunction.isFunctionSignedInputDefined = true;
                        updateRoutine = true;
                    }
                }
                else
                {
                    sValue = Calibration.getWord(calibrationElem.FunctionElem.AddressInt, true);
                    if (sValue == "7fff")
                    {
                        ioFunction.FunctionSignedInput = true;
                        ioFunction.isFunctionSignedInputDefined = true;
                        updateRoutine = true;
                    }
                    else if (sValue == "ffff")
                    {
                        ioFunction.FunctionSignedInput = false;
                        ioFunction.isFunctionSignedInputDefined = true;
                        updateRoutine = true;
                    }
                }
            }
            calibrationElem.FunctionElem.SignedInput = ioFunction.FunctionSignedInput;
            calibrationElem.FunctionElem.SignedOutput = ioFunction.SignedOutput;

            if (updateRoutine)
            {
                rRoutine.SetTranslationComments();
                Calibration.slRoutines[rRoutine.UniqueAddress] = rRoutine;
            }
        }

        private void processCalibrationElementTable(ref CalibrationElement calibrationElem, ref Routine rRoutine, ref RoutineIOTable ioTable, int calElemOpeAddress, string callOpeUniqueAddress, int searchOpeStartAddress, ref bool preferPrecedingProcess)
        {
            Operation[] adjacentOperations = null;
            bool processPrecedingOperations = false;
            bool precedingOperationsProcessed = false;
            bool followingOperationsProcessed = false;

            calibrationElem.TableElem = new Table();
            calibrationElem.TableElem.BankNum = calibrationElem.BankNum;
            calibrationElem.TableElem.AddressInt = calibrationElem.AddressInt;
            calibrationElem.TableElem.AddressBinInt = calibrationElem.AddressBinInt;
            calibrationElem.TableElem.RBase = calibrationElem.RBase;
            calibrationElem.TableElem.RBaseCalc = calibrationElem.RBaseCalc;
            
            calibrationElem.TableElem.WordOutput = ioTable.TableWord;
            if (ioTable.isSignedOutputDefined) calibrationElem.TableElem.SignedOutput = ioTable.SignedOutput;
            else calibrationElem.TableElem.SignedOutput = false; // Defaulted to Unsigned if not previously identified

            RoutineCallInfoTable ciCi = new RoutineCallInfoTable();
            ciCi.CalibrationElementOpeUniqueAddress = Tools.UniqueAddress(Num, calElemOpeAddress);
            ciCi.RoutineUniqueAddress = rRoutine.UniqueAddress;
            ciCi.RoutineCallOpeUniqueAddress = callOpeUniqueAddress;
            ciCi.RoutineInputOutput = ioTable;
            calibrationElem.TableElem.RoutinesCallsInfos.Add(ciCi);
            if (!slRoutineOPsCalibElemUniqueAddresses.ContainsKey(ciCi.RoutineCallOpeUniqueAddress))
            {
                slRoutineOPsCalibElemUniqueAddresses.Add(ciCi.RoutineCallOpeUniqueAddress, calibrationElem.TableElem.UniqueAddress);
            }
            ciCi = null;

            calibrationElem.TableElem.ColsNumber = -1;
            // S6x Table Input Fixed Cols Number
            if (ioTable.S6xInputTable != null)
            {
                if (ioTable.S6xInputTable.ForcedColsNumber != null && ioTable.S6xInputTable.ForcedColsNumber != string.Empty)
                {

                    try { calibrationElem.TableElem.ColsNumber = Convert.ToInt32(ioTable.S6xInputTable.ForcedColsNumber); }
                    catch { }
                }
            }
            if (calibrationElem.TableElem.ColsNumber > 0) return;

            // Searching for Cols Number
            while (!(precedingOperationsProcessed && followingOperationsProcessed))
            {
                processPrecedingOperations = false;

                // Cols Number to be found - 16 Ops History backward & forward
                if (!precedingOperationsProcessed && !followingOperationsProcessed)
                {
                    processPrecedingOperations = preferPrecedingProcess;
                }
                else
                {
                    processPrecedingOperations = !precedingOperationsProcessed;
                }

                //if (processPrecedingOperations) adjacentOperations = getPrecedingOPs(searchOpeStartAddress, 16, 99, true, true, true, true, true, true, true, true);
                // 20200515 - PYM
                // Now managing 1 Call Sub Level in processPrecedingOperations mode
                if (processPrecedingOperations) adjacentOperations = getPrecedingOPs(searchOpeStartAddress, 16, 1);
                else adjacentOperations = getFollowingOPs(searchOpeStartAddress, 16, 99, true, true, true, true, true, true, true, true);
                
                foreach (Operation opResult in adjacentOperations)
                {
                    if (opResult == null) continue;
                    // First Ope in Preceding Mode is always the Original Ope Address, no interest
                    if (opResult.AddressInt == searchOpeStartAddress) continue;

                    // Break Conditions
                    // Same Routine Type between Table Address and Col Number Address => Break
                    if (opResult.CallType == CallType.Call || opResult.CallType == CallType.ShortCall)
                    {
                        if (Calibration.slRoutines.ContainsKey(Tools.UniqueAddress(opResult.ApplyOnBankNum, opResult.AddressJumpInt)))
                        {
                            if (((Routine)Calibration.slRoutines[Tools.UniqueAddress(opResult.ApplyOnBankNum, opResult.AddressJumpInt)]).Type == RoutineType.TableByte) break;
                            if (((Routine)Calibration.slRoutines[Tools.UniqueAddress(opResult.ApplyOnBankNum, opResult.AddressJumpInt)]).Type == RoutineType.TableWord) break;
                        }
                    }

                    // Col Number Register Identification
                    if (opResult.OperationParams.Length == 2)
                    {
                        switch (opResult.OriginalInstruction.ToLower())
                        {
                            case "ldb":
                            case "ldw":
                            case "ldzbw":
                                if (opResult.OperationParams[1].InstructedParam == SADDef.ShortRegisterPrefix + ioTable.TableColNumberRegister)
                                {
                                    // Prevent converting a register to a number
                                    try { calibrationElem.TableElem.ColsNumber = Convert.ToInt32(opResult.OperationParams[0].InstructedParam, 16); }
                                    catch { }
                                    break;
                                }
                                break;
                        }
                        if (calibrationElem.TableElem.ColsNumber > 64) calibrationElem.TableElem.ColsNumber = 0;
                        if (calibrationElem.TableElem.ColsNumber > 0) break;
                    }
                }
                adjacentOperations = null;

                if (calibrationElem.isFullyIdentified)
                {
                    preferPrecedingProcess = processPrecedingOperations;
                    break;
                }

                if (processPrecedingOperations) precedingOperationsProcessed = true;
                else followingOperationsProcessed = true;
            }
        }

        private void processCalibrationElementStructure(ref CalibrationElement calibrationElem, ref Routine rRoutine, ref RoutineIOStructure ioStructure, int calElemOpeAddress, string callOpeUniqueAddress)
        {
            calibrationElem.StructureElem = new Structure();
            calibrationElem.StructureElem.BankNum = calibrationElem.BankNum;
            calibrationElem.StructureElem.AddressInt = calibrationElem.AddressInt;
            calibrationElem.StructureElem.AddressBinInt = calibrationElem.AddressBinInt;
            calibrationElem.StructureElem.RBase = calibrationElem.RBase;
            calibrationElem.StructureElem.RBaseCalc = calibrationElem.RBaseCalc;

            RoutineCallInfoStructure ciCi = new RoutineCallInfoStructure();
            ciCi.CalibrationElementOpeUniqueAddress = Tools.UniqueAddress(Num, calElemOpeAddress);
            ciCi.RoutineUniqueAddress = rRoutine.UniqueAddress;
            ciCi.RoutineCallOpeUniqueAddress = callOpeUniqueAddress;
            ciCi.RoutineInputOutput = ioStructure;
            calibrationElem.StructureElem.RoutinesCallsInfos.Add(ciCi);
            if (!slRoutineOPsCalibElemUniqueAddresses.ContainsKey(ciCi.RoutineCallOpeUniqueAddress))
            {
                slRoutineOPsCalibElemUniqueAddresses.Add(ciCi.RoutineCallOpeUniqueAddress, calibrationElem.StructureElem.UniqueAddress);
            }
            ciCi = null;

            // Defaulted
            calibrationElem.StructureElem.Defaulted = true;
            calibrationElem.StructureElem.StructDefString = "ByteHex";
            calibrationElem.StructureElem.Number = 1;

            // No Automatic mngt for now
            // TO BE MANAGED

            if (ioStructure.S6xInputStructure != null)
            {
                calibrationElem.StructureElem.StructDefString = ioStructure.S6xInputStructure.StructDef;
                if (ioStructure.S6xInputStructure.ForcedNumber != null && ioStructure.S6xInputStructure.ForcedNumber != string.Empty) 
                {
                    try { calibrationElem.StructureElem.Number = Convert.ToInt32(ioStructure.S6xInputStructure.ForcedNumber);}
                    catch {}
                }
            }
        }

        private void processCalibrationElementScalar(ref CalibrationElement calibrationElem, ref Routine rRoutine, ref RoutineIOScalar ioScalar, int calElemOpeAddress, string callOpeUniqueAddress)
        {
            Operation[] followingOperations = null;

            calibrationElem.ScalarElem = new Scalar();
            calibrationElem.ScalarElem.BankNum = calibrationElem.BankNum;
            calibrationElem.ScalarElem.AddressInt = calibrationElem.AddressInt;
            calibrationElem.ScalarElem.AddressBinInt = calibrationElem.AddressBinInt;
            calibrationElem.ScalarElem.RBase = calibrationElem.RBase;
            calibrationElem.ScalarElem.RBaseCalc = calibrationElem.RBaseCalc;

            RoutineCallInfoScalar ciCi = new RoutineCallInfoScalar();
            ciCi.CalibrationElementOpeUniqueAddress = Tools.UniqueAddress(Num, calElemOpeAddress);
            ciCi.RoutineUniqueAddress = rRoutine.UniqueAddress;
            ciCi.RoutineCallOpeUniqueAddress = callOpeUniqueAddress;
            ciCi.RoutineInputOutput = ioScalar;
            calibrationElem.ScalarElem.RoutinesCallsInfos.Add(ciCi);
            if (!slRoutineOPsCalibElemUniqueAddresses.ContainsKey(ciCi.RoutineCallOpeUniqueAddress))
            {
                slRoutineOPsCalibElemUniqueAddresses.Add(ciCi.RoutineCallOpeUniqueAddress, calibrationElem.ScalarElem.UniqueAddress);
            }
            ciCi = null;

            // S6x Information
            if (ioScalar.S6xInputScalar != null)
            {
                calibrationElem.ScalarElem.Byte = ioScalar.S6xInputScalar.Byte;
                calibrationElem.ScalarElem.Word = !ioScalar.S6xInputScalar.Byte;
                calibrationElem.ScalarElem.Signed = ioScalar.S6xInputScalar.Signed;
                calibrationElem.ScalarElem.UnSigned = !ioScalar.S6xInputScalar.Signed;
                return;
            }

            // Not able to manage on current Bank
            if (rRoutine.BankNum != Num) return;

            // Searching in Routine how Calibration Element is Used
            followingOperations = getFollowingOPs(rRoutine.AddressInt, 32, 1, true, false, false, false, true, true, true, true);

            // Basic Identification based on Output Register affectation
            // Enough for now
            for (int iPos = 0; iPos < followingOperations.Length; iPos++)
            {
                Operation ope = followingOperations[iPos];
                if (ope == null) break;
                if (ope.isReturn) break;

                for (int instructedParam = 0; instructedParam < ope.OperationParams.Length; instructedParam++)
                {
                    if (ope.OperationParams[instructedParam].InstructedParam == Tools.PointerTranslation(Tools.RegisterInstruction(ioScalar.AddressRegister)) && instructedParam < ope.OperationParams.Length - 1)
                    // Element Address Pointer is Accessed to read it only
                    // A Scalar is now created based on Operation Type
                    {
                        bool isWord = false;
                        bool isSigned = false;

                        SADOPCode opCode = (SADOPCode)Calibration.slOPCodes[ope.OriginalOPCode];
                        switch (opCode.Type)
                        {
                            // Word Scalar
                            case OPCodeType.WordOP:
                                isWord = true;
                                break;
                            // Everything Else is Byte Scalar
                            default:
                                isWord = false;
                                break;
                        }
                        // Signed Information is direct
                        if (opCode.isSigndAlt)
                        {
                            isSigned = true;
                        }
                        else
                        // Searching on Next Ope if it is a Goto one
                        {
                            if (iPos + 1 < followingOperations.Length)
                            {
                                ope = followingOperations[iPos + 1];
                                if (ope != null)
                                {
                                    if (ope.CallType == CallType.Goto)
                                    {
                                        switch (ope.OriginalOPCode.ToLower())
                                        {
                                            // Signed Ones
                                            case "d2":
                                            case "d6":
                                            case "da":
                                            case "de":
                                                isSigned = true;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        opCode = null;
                        ope = null;
                        followingOperations = null;

                        calibrationElem.ScalarElem.Byte = !isWord;
                        calibrationElem.ScalarElem.Word = isWord;
                        calibrationElem.ScalarElem.Signed = isSigned;
                        calibrationElem.ScalarElem.UnSigned = !isSigned;

                        return;
                    }
                }
            }
        }

        // Process Calibration Element related with Other Routine
        //  Tables, Functions, Structures are already managed, it is a Scalar
        private void processCalibrationElementDefault(ref CalibrationElement calibrationElem, ref Routine rRoutine, ref RoutineIO ioGeneric, int calElemOpe, string callOpeUniqueAddress)
        {
            RoutineIOScalar ioScalar = new RoutineIOScalar();
            ioScalar.AddressRegister = ioGeneric.AddressRegister;

            processCalibrationElementScalar(ref calibrationElem, ref rRoutine, ref ioScalar, calElemOpe, callOpeUniqueAddress);

            ioScalar = null;
        }

        // Process Scalar Calibration Element not Fully Identified, Byte / Word, Signed / Unsigned / Bit Flags
        private void processCalibrationElementScalarDetails(ref CalibrationElement calibrationElem, ref Operation calOpe)
        {
            Operation nextOpe = null;
            Operation[] followingOperations = null;
            object[] arrPointersValues = null;
            string regCpy = string.Empty;
            string regCpyTopByte = string.Empty;

            if (!calibrationElem.isScalar) return;

            // Cal Elem is a pointer for address check - Otherwise it should be a structure, not the right place here
            if (calOpe.OperationParams.Length < 2) return;
            if (!calOpe.OperationParams[0].InstructedParam.Contains(SADDef.LongRegisterPointerPrefix)) return;

            // Cmpw / Cmpb case, current ope gives the type, next ope gives the sign, it is enough
            if (calOpe.OriginalInstruction.ToLower() == "cmpw" || calOpe.OriginalInstruction.ToLower() == "cmpb")
            {
                if (!calibrationElem.ScalarElem.Byte && !calibrationElem.ScalarElem.Word)
                {
                    if (calOpe.OriginalInstruction.ToLower() == "cmpw")
                    {
                        calibrationElem.ScalarElem.Byte = false;
                        calibrationElem.ScalarElem.Word = true;
                    }
                    else
                    {
                        calibrationElem.ScalarElem.Byte = true;
                        calibrationElem.ScalarElem.Word = false;
                    }
                }
                nextOpe = (Operation)slOPs[Tools.UniqueAddress(Num, calOpe.AddressNextInt)];
                if (nextOpe != null)
                {
                    if (nextOpe.CallType == CallType.Goto)
                    {
                        switch (nextOpe.OriginalOPCode.ToLower())
                        {
                            // Signed Ones
                            case "d2":
                            case "d6":
                            case "da":
                            case "de":
                                calibrationElem.ScalarElem.Signed = true;
                                break;
                            default:
                                calibrationElem.ScalarElem.Signed = false;
                                break;
                        }
                        calibrationElem.ScalarElem.UnSigned = !calibrationElem.ScalarElem.Signed;
                    }
                }
                return;
            }

            // Register Copy to continue
            arrPointersValues = Tools.InstructionPointersValues(calOpe.OperationParams[calOpe.OperationParams.Length - 1].InstructedParam);
            // No Register to manage
            if (!(bool)arrPointersValues[0]) return;
            if (arrPointersValues.Length > 3) return;
            if (arrPointersValues[1].ToString().Length > 2) return;

            regCpy = arrPointersValues[1].ToString();
            regCpyTopByte = Convert.ToString((int)arrPointersValues[2] + 1, 16);

            arrPointersValues = null;

            // Searching how Calibration Element is Used
            followingOperations = getFollowingOPs(calOpe.AddressNextInt, 8, 1, true, false, true, true, true, true, true, true);

            // Basic Identification based on Output Register affectation
            // Enough for now
            for (int iPos = 0; iPos < followingOperations.Length; iPos++)
            {
                Operation ope = followingOperations[iPos];
                if (ope == null) break;
                if (ope.isReturn) break;

                for (int instructedParam = 0; instructedParam < ope.OperationParams.Length; instructedParam++)
                {
                    if ((ope.OperationParams[instructedParam].InstructedParam.Contains(Tools.RegisterInstruction(regCpy)) || ope.OperationParams[instructedParam].InstructedParam.Contains(Tools.RegisterInstruction(regCpyTopByte))) && instructedParam < ope.OperationParams.Length - 1)
                    // Register is used
                    {
                        bool isWord = false;
                        bool isSigned = false;

                        SADOPCode opCode = (SADOPCode)Calibration.slOPCodes[ope.OriginalOPCode];
                        if (ope.OperationParams[instructedParam].InstructedParam.Contains(Tools.RegisterInstruction(regCpyTopByte)))
                        {
                            isWord = true;
                        }
                        else
                        {
                            switch (opCode.Type)
                            {
                                // Word Scalar
                                case OPCodeType.WordOP:
                                    isWord = true;
                                    break;
                                // Everything Else is Byte Scalar
                                default:
                                    isWord = false;
                                    break;
                            }
                        }
                        if (!calibrationElem.ScalarElem.Byte && !calibrationElem.ScalarElem.Word)
                        {
                            calibrationElem.ScalarElem.Byte = !isWord;
                            calibrationElem.ScalarElem.Word = isWord;
                        }

                        // Signed Information is direct
                        if (opCode.isSigndAlt || opCode.Instruction.ToLower() == "ldsbw")
                        {
                            isSigned = true;
                        }
                        // Scalar could have Bit Flags
                        else if (opCode.Type == OPCodeType.BitByteGotoOP)
                        {
                            if (opCode.OPCodeInt == 0x37 || opCode.OPCodeInt == 0x3f)
                            // Bit 7 is tested, Scalar is signed
                            {
                                isSigned = true;
                            }
                            else
                            // Other Bits are tested, Scalar has Bit Flags and is not signed
                            {
                                isSigned = false;
                                calibrationElem.ScalarElem.isBitFlags = true;
                            }
                        }
                        else
                        // Searching on Next Ope if it is a Goto one
                        {
                            if (iPos + 1 < followingOperations.Length)
                            {
                                ope = followingOperations[iPos + 1];
                                if (ope != null)
                                {
                                    if (ope.CallType == CallType.Goto)
                                    {
                                        switch (ope.OriginalOPCode.ToLower())
                                        {
                                            // Signed Ones
                                            case "d2":
                                            case "d6":
                                            case "da":
                                            case "de":
                                                isSigned = true;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        opCode = null;
                        ope = null;
                        followingOperations = null;

                        calibrationElem.ScalarElem.Signed = isSigned;
                        calibrationElem.ScalarElem.UnSigned = !isSigned;

                        return;
                    }
                }
            }
        }

        //  Process Calibration Element based on Address Register Copy or Sub Call Routine Execution
        //      Second Step for unidentified Elements
        // 45,f6,07,f8,46       ad3w  R46,Rf8,7f6        R46 = 46da;          Reg Cpy = Cal Elem Address
        // 29,28                scall 63ae               Sub534();            Sub Call 1 Level Max Optional
        // ...
        // a0,46,3c             ldw   R3c,R46            R3c = R46;           Reg Address = Reg Cpy
        // ef,37,c4             call  27fd               UTabLU();            Routin Call
        private void processCalibrationElementSub(ref Operation calOpe, ref CalibrationElement calElem, ref SADS6x S6x)
        {
            Operation[] followingOperations = null;
            Operation[] precedingTableOperations = null;
            Operation ope = null;
            ArrayList alCompatibleRoutines = null;
            Routine rRoutine = null;
            RoutineIO ioGeneric = null;
            RoutineIOTable ioTable = null;
            RoutineIOFunction ioFunction = null;
            RoutineIOScalar ioScalar = null;
            RoutineIOStructure ioStructure = null;
            string regCpy = string.Empty;
            int tableCallPos = -1;

            alCompatibleRoutines = new ArrayList();
            regCpy = calOpe.OriginalOpArr[calOpe.OriginalOpArr.Length - 1];

            // Search related Routines on initial register
            foreach (Routine cRoutine in Calibration.slRoutines.Values)
            {
                if (cRoutine.Type != RoutineType.Unknown && cRoutine.IOs != null)
                {
                    foreach (RoutineIO ioIO in cRoutine.IOs)
                    {
                        if (ioIO.GetType() == typeof(RoutineIOTable) || ioIO.GetType() == typeof(RoutineIOFunction))
                        {
                            if (ioIO.AddressRegister == regCpy) alCompatibleRoutines.Add(cRoutine.UniqueAddress);
                        }
                    }
                }
            }

            followingOperations = getFollowingOPs(calOpe.AddressInt, 32, 1, true, true, true, true, true, true, false, true);
            for (int iPos = 0; iPos < followingOperations.Length; iPos++)
            {
                ope = followingOperations[iPos];
                if (ope == null) break;
                if (ope.isReturn) break;        // 20171129 Return Break Condition Added to prevent going outside call

                // Reg Cpy is copied in another register
                if (ope.OriginalOPCode.ToLower() == "a0")
                {
                    // Reg Cpy is replaced
                    if (ope.OriginalOpArr[1] == regCpy)
                    {
                        regCpy = ope.OriginalOpArr[ope.OriginalOpArr.Length - 1];
                        // Search new related Routines on initial or replicated register
                        alCompatibleRoutines.Clear();
                        foreach (Routine cRoutine in Calibration.slRoutines.Values)
                        {
                            if (cRoutine.Type != RoutineType.Unknown && cRoutine.IOs != null)
                            {
                                foreach (RoutineIO ioIO in cRoutine.IOs)
                                {
                                    if (ioIO.GetType() == typeof(RoutineIOTable) || ioIO.GetType() == typeof(RoutineIOFunction))
                                    {
                                        if (ioIO.AddressRegister == regCpy) alCompatibleRoutines.Add(cRoutine.UniqueAddress);
                                    }
                                }
                            }
                        }
                    }
                }
                // 20171129 - All Call Types are compatible
                //if (ope.CallType == CallType.Call || ope.CallType == CallType.ShortCall)
                else if (ope.CallType != CallType.Unknown)
                {
                    if (alCompatibleRoutines.Contains(Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)))
                    {
                        rRoutine = (Routine)Calibration.slRoutines[Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)];
                        // Compatible Routine has always defined IOs
                        foreach (RoutineIO ioIO in rRoutine.IOs)
                        {
                            if (ioIO.AddressRegister != regCpy) continue;

                            if (ioIO.GetType() == typeof(RoutineIOStructure))
                            {
                                if (!S6x.isS6xProcessTypeConflict(calElem.UniqueAddress, typeof(Structure)))
                                {
                                    ioStructure = (RoutineIOStructure)ioIO;
                                    processCalibrationElementStructure(ref calElem, ref rRoutine, ref ioStructure, calOpe.AddressInt, ope.UniqueAddress);
                                    ioStructure = null;
                                }
                                alCompatibleRoutines = null;
                                followingOperations = null;
                                rRoutine = null;
                                ope = null;
                                return;
                            }
                            else if (ioIO.GetType() == typeof(RoutineIOTable))
                            {
                                if (S6x.isS6xProcessTypeConflict(calElem.UniqueAddress, typeof(Table))) return;

                                ioTable = (RoutineIOTable)ioIO;
                                tableCallPos = iPos;
                                // Process Continues for Table based on found Position and InputOutput
                                break;
                            }
                            else if (ioIO.GetType() == typeof(RoutineIOFunction))
                            {
                                if (!S6x.isS6xProcessTypeConflict(calElem.UniqueAddress, typeof(Function)))
                                {
                                    ioFunction = (RoutineIOFunction)ioIO;
                                    processCalibrationElementFunction(ref calElem, ref rRoutine, ref ioFunction, calOpe.AddressInt, ope.UniqueAddress);
                                    ioFunction = null;
                                }
                                alCompatibleRoutines = null;
                                followingOperations = null;
                                rRoutine = null;
                                ope = null;
                                return;
                            }
                            else if (ioIO.GetType() == typeof(RoutineIOScalar))
                            {
                                if (!S6x.isS6xProcessTypeConflict(calElem.UniqueAddress, typeof(Scalar)))
                                {
                                    ioScalar = (RoutineIOScalar)ioIO;
                                    processCalibrationElementScalar(ref calElem, ref rRoutine, ref ioScalar, calOpe.AddressInt, ope.UniqueAddress);
                                    ioScalar = null;
                                }
                                alCompatibleRoutines = null;
                                followingOperations = null;
                                rRoutine = null;
                                ope = null;
                                return;
                            }
                            else
                            {
                                if (!S6x.isS6xProcessTypeConflict(calElem.UniqueAddress, typeof(Scalar)))
                                {
                                    ioGeneric = ioIO;
                                    processCalibrationElementDefault(ref calElem, ref rRoutine, ref ioGeneric, calOpe.AddressInt, ope.UniqueAddress);
                                    ioGeneric = null;
                                }
                                alCompatibleRoutines = null;
                                followingOperations = null;
                                rRoutine = null;
                                ope = null;
                                return;
                            }
                        }
                        // Compatible routine found no need to continue
                        ope = null;
                        break;
                    }
                }
                ope = null;
            }
            alCompatibleRoutines = null;

            // Process Continues for Table based on found Position and InputOutput
            if (tableCallPos >= 0 && ioTable != null)
            {
                // Table Stays to be managed
                calElem.TableElem = new Table();
                calElem.TableElem.BankNum = calElem.BankNum;
                calElem.TableElem.AddressInt = calElem.AddressInt;
                calElem.TableElem.AddressBinInt = calElem.AddressBinInt;
                calElem.TableElem.RBase = calElem.RBase;
                calElem.TableElem.RBaseCalc = calElem.RBaseCalc;

                calElem.TableElem.WordOutput = ioTable.TableWord;
                if (ioTable.isSignedOutputDefined) calElem.TableElem.SignedOutput = ioTable.SignedOutput;
                else calElem.TableElem.SignedOutput = false; // Defaulted to Unsigned if not previously identified

                RoutineCallInfoTable ciCi = new RoutineCallInfoTable();
                ciCi.CalibrationElementOpeUniqueAddress = calOpe.UniqueAddress;
                ciCi.RoutineUniqueAddress = rRoutine.UniqueAddress;
                ciCi.RoutineCallOpeUniqueAddress = followingOperations[tableCallPos].UniqueAddress;
                ciCi.RoutineInputOutput = ioTable;
                calElem.TableElem.RoutinesCallsInfos.Add(ciCi);
                if (!slRoutineOPsCalibElemUniqueAddresses.ContainsKey(ciCi.RoutineCallOpeUniqueAddress))
                {
                    slRoutineOPsCalibElemUniqueAddresses.Add(ciCi.RoutineCallOpeUniqueAddress, calElem.TableElem.UniqueAddress);
                }
                ciCi = null;

                calElem.TableElem.ColsNumber = -1;
                // S6x Table Input Fixed Cols Number
                if (ioTable.S6xInputTable != null)
                {
                    if (ioTable.S6xInputTable.ForcedColsNumber != null && ioTable.S6xInputTable.ForcedColsNumber != string.Empty)
                    {

                        try { calElem.TableElem.ColsNumber = Convert.ToInt32(ioTable.S6xInputTable.ForcedColsNumber); }
                        catch { }
                    }
                }
                if (calElem.TableElem.ColsNumber > 0)
                {
                    ope = null;
                    rRoutine = null;
                    followingOperations = null;
                    return;
                }

                // Searching for Cols Number
                regCpy = SADDef.ShortRegisterPrefix + ioTable.TableColNumberRegister; // Including ShortRegisterPrefix
                for (int iPos = tableCallPos - 1; iPos >= 0; iPos--)
                {
                    ope = followingOperations[iPos];
                    if (ope == null) break;

                    // Col Number Register Identification
                    if (ope.OperationParams.Length == 2)
                    {
                        if (ope.OperationParams[1].InstructedParam == regCpy)
                        {
                            regCpy = ope.OperationParams[0].InstructedParam; // Changing Cols Number register, Including ShortRegisterPrefix for next searches
                            if (!regCpy.StartsWith(SADDef.ShortRegisterPrefix)) // Else Cols Number is in a register, value has to be found previously
                            // Cols Number is directly available
                            {
                                try { calElem.TableElem.ColsNumber = Convert.ToInt32(regCpy, 16); }
                                catch { /* Continues */ }
                            }
                            if (calElem.TableElem.ColsNumber > 0) break;
                        }
                    }
                    ope = null;
                }

                // Last Chance when not Found, searching before Calibration Element Operation
                //precedingTableOperations = getPrecedingOPs(calOpe.AddressInt, 8, 99, false, true, false, false, false, false, false, false);
                precedingTableOperations = getPrecedingOPs(calOpe.AddressInt, 8, 0);
                for (int iPos = 0; iPos < precedingTableOperations.Length; iPos++)
                {
                    ope = precedingTableOperations[iPos];
                    if (ope == null) break;

                    // Col Number Register Identification
                    if (ope.OperationParams.Length == 2)
                    {
                        if (ope.OperationParams[1].InstructedParam == regCpy)
                        {
                            regCpy = ope.OperationParams[0].InstructedParam; // Changing Cols Number register, Including ShortRegisterPrefix for next searches
                            if (!regCpy.StartsWith(SADDef.ShortRegisterPrefix)) // Else Cols Number is in a register, value has to be found previously
                            // Cols Number is directly available
                            {
                                try { calElem.TableElem.ColsNumber = Convert.ToInt32(regCpy, 16); }
                                catch { /* Continues */ }
                            }
                            if (calElem.TableElem.ColsNumber > 0) break;
                        }
                    }
                    ope = null;
                }
                precedingTableOperations = null;
            }

            ope = null;
            rRoutine = null;
            followingOperations = null;
        }

        public void processCalibrationElementsRegisters(ref SADS6x S6x)
        {
            Operation ope = null;
            CalibrationElement calElem;

            foreach (string uniqueAddress in alCalibElemOPsUniqueAddresses)
            {
                ope = (Operation)slOPs[uniqueAddress];
                if (ope.alCalibrationElems == null) continue;
                
                foreach (CalibrationElement opeCalElem in ope.alCalibrationElems)
                {
                    calElem = (CalibrationElement)Calibration.slCalibrationElements[opeCalElem.UniqueAddress];
                    if (calElem != null)
                    {
                        if (calElem.isTable)
                        {
                            RoutineCallInfoTable ciCt = null;
                            foreach (RoutineCallInfoTable ciCi in calElem.TableElem.RoutinesCallsInfos)
                            {
                                if (ciCi.CalibrationElementOpeUniqueAddress == uniqueAddress)
                                {
                                    ciCt = ciCi;
                                    break;
                                }
                            }
                            if (ciCt == null && calElem.TableElem.RoutinesCallsInfos.Count > 0)
                            {
                                RoutineCallInfoTable exCt = (RoutineCallInfoTable)calElem.TableElem.RoutinesCallsInfos[calElem.TableElem.RoutinesCallsInfos.Count - 1];
                                ciCt = new RoutineCallInfoTable();
                                ciCt.CalibrationElementOpeUniqueAddress = uniqueAddress;
                                ciCt.RoutineUniqueAddress = exCt.RoutineUniqueAddress;
                                ciCt.RoutineCallOpeUniqueAddress = string.Empty;
                                ciCt.RoutineInputOutput = exCt.RoutineInputOutput;
                                calElem.TableElem.RoutinesCallsInfos.Add(ciCt);
                                ciCt = null;
                                exCt = null;
                            }

                            processCalibrationElementsRegistersTable(ref calElem.TableElem);
                        }
                        else if (calElem.isFunction)
                        {
                            RoutineCallInfoFunction ciCf = null;
                            foreach (RoutineCallInfoFunction ciCi in calElem.FunctionElem.RoutinesCallsInfos)
                            {
                                if (ciCi.CalibrationElementOpeUniqueAddress == uniqueAddress)
                                {
                                    ciCf = ciCi;
                                    break;
                                }
                            }
                            if (ciCf == null && calElem.FunctionElem.RoutinesCallsInfos.Count > 0)
                            {
                                RoutineCallInfoFunction exCf = (RoutineCallInfoFunction)calElem.FunctionElem.RoutinesCallsInfos[calElem.FunctionElem.RoutinesCallsInfos.Count - 1];
                                ciCf = new RoutineCallInfoFunction();
                                ciCf.CalibrationElementOpeUniqueAddress = uniqueAddress;
                                ciCf.RoutineUniqueAddress = exCf.RoutineUniqueAddress;
                                ciCf.RoutineCallOpeUniqueAddress = string.Empty;
                                ciCf.RoutineInputOutput = exCf.RoutineInputOutput;
                                calElem.FunctionElem.RoutinesCallsInfos.Add(ciCf);
                                ciCf = null;
                                exCf = null;
                            }

                            processCalibrationElementsRegistersFunction(ref calElem.FunctionElem);
                        }
                    }
                    calElem = null;
                }
                ope = null;
            }
        }

        // processCalibrationElementsRegistersFunction
        //      To analyse input/output registers around functions
        //      Base for detecting scalers and other registers
        //      20200512 - PYM - Reviewed to better manage RegCpy vs RegByteCpy
        public void processCalibrationElementsRegistersFunction(ref Function function)
        {
            Operation[] adjacentOperations = null;
            string regCpy = string.Empty;
            string regByteCpy = string.Empty;
            int regInt = -1;
            int cmpIndex = -1;
            int cpyIndex = -1;
            int minRegAdr = 0;
            int maxRegAdr = 0;

            minRegAdr = 0x23;
            maxRegAdr = 0x60;
            if (is8061)
            {
                minRegAdr = 0x11;
                maxRegAdr = 0x60;
            }

            foreach (RoutineCallInfoFunction ciCi in function.RoutinesCallsInfos)
            {
                ciCi.InputRegister = string.Empty;
                ciCi.OutputRegister = string.Empty;
                ciCi.OutputRegisterByte = string.Empty;
                ciCi.OutputRegisterSigned = false;
                Routine rRoutine = (Routine)Calibration.slRoutines[ciCi.RoutineUniqueAddress];
                if (rRoutine == null) continue;
                RoutineIOFunction ioFunction = ciCi.RoutineInputOutput;
                if (ioFunction == null) continue;

                // Registers already provided, from S6x Input essentially
                int tmpRegAdr = -1;
                if (ioFunction.FunctionInputRegister != string.Empty)
                {
                    tmpRegAdr = Convert.ToInt32(ioFunction.FunctionInputRegister, 16);
                    if (tmpRegAdr >= maxRegAdr || tmpRegAdr <= minRegAdr) ciCi.InputRegister = Tools.RegisterInstruction(ioFunction.FunctionInputRegister);
                }
                if (ioFunction.OutputRegister != string.Empty)
                {
                    tmpRegAdr = Convert.ToInt32(ioFunction.OutputRegister, 16);
                    if (tmpRegAdr >= maxRegAdr || tmpRegAdr <= minRegAdr)
                    {
                        ciCi.OutputRegister = Tools.RegisterInstruction(ioFunction.OutputRegister);
                        ciCi.OutputRegisterByte = Tools.RegisterInstruction(Convert.ToString(tmpRegAdr + 1, 16));
                    }
                }
                if (ioFunction.isSignedOutputDefined) ciCi.OutputRegisterSigned = ioFunction.SignedOutput;

                // Everything is already provided, no need to search for it
                if (ciCi.InputRegister != string.Empty && ciCi.OutputRegister != string.Empty && ioFunction.isSignedOutputDefined)
                {
                    rRoutine = null;
                    ioFunction = null;
                    continue;
                }

                // Search is required
                if (ciCi.RoutineCallOpeUniqueAddress == string.Empty && ciCi.CalibrationElementOpeUniqueAddress != string.Empty)
                {
                    Operation calElemOpe = (Operation)slOPs[ciCi.CalibrationElementOpeUniqueAddress];
                    if (calElemOpe != null)
                    {
                        adjacentOperations = getFollowingOPs(calElemOpe.AddressInt, 16, 99, true, true, true, true, true, true, true, true);
                        foreach (Operation postOpe in adjacentOperations)
                        {
                            if (postOpe != null)
                            {
                                if (postOpe.CallType == CallType.Call || postOpe.CallType == CallType.ShortCall)
                                {
                                    if (Calibration.slRoutines.ContainsKey(Tools.UniqueAddress(postOpe.ApplyOnBankNum, postOpe.AddressJumpInt)))
                                    {
                                        if (((Routine)Calibration.slRoutines[Tools.UniqueAddress(postOpe.ApplyOnBankNum, postOpe.AddressJumpInt)]).Type == rRoutine.Type)
                                        {
                                            ciCi.RoutineCallOpeUniqueAddress = postOpe.UniqueAddress;
                                            if (!slRoutineOPsCalibElemUniqueAddresses.ContainsKey(ciCi.RoutineCallOpeUniqueAddress))
                                            {
                                                slRoutineOPsCalibElemUniqueAddresses.Add(ciCi.RoutineCallOpeUniqueAddress, function.UniqueAddress);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        adjacentOperations = null;
                    }

                }

                Operation callOpe = null;
                if ( ciCi.RoutineCallOpeUniqueAddress != string.Empty) callOpe = (Operation)slOPs[ciCi.RoutineCallOpeUniqueAddress];
                
                if (callOpe != null && rRoutine != null)
                {
                    //adjacentOperations = getPrecedingOPs(callOpe.AddressInt, 16, 99, true, true, true, true, true, true, true, true);
                    adjacentOperations = getPrecedingOPs(callOpe.AddressInt, 16, 0);
                    regCpy = SADDef.ShortRegisterPrefix + ioFunction.FunctionInputRegister;
                    regByteCpy = "RXX";
                    if (!ioFunction.FunctionByte)
                    {
                        regInt = Tools.InstructionRegisterAddress(regCpy);
                        if (regInt >= 0) regByteCpy = Tools.RegisterInstruction(string.Format("{0:x2}", regInt + 1));
                    }
                    foreach (Operation prevOpe in adjacentOperations)
                    {
                        if (prevOpe != null)
                        {
                            // Initial Call with Args (FuncAdr,RXX) 2 translated parameters
                            if (prevOpe.AddressInt == callOpe.AddressInt && callOpe.CallArgsParams.Length > 0)
                            {
                                foreach (CallArgument cArg in callOpe.CallArguments)
                                {
                                    if (regCpy == Tools.RegisterInstruction(cArg.OutputRegisterAddress))
                                    {
                                        if (cArg.DecryptedValueInt % 2 == 0)
                                        {
                                            regCpy = Tools.RegisterInstruction(cArg.DecryptedValue);
                                            regByteCpy = Tools.RegisterInstruction(string.Format("{0:x2}", cArg.DecryptedValueInt + 1));
                                        }
                                        else
                                        {
                                            regCpy = Tools.RegisterInstruction(string.Format("{0:x2}", cArg.DecryptedValueInt - 1));
                                            regByteCpy = Tools.RegisterInstruction(cArg.DecryptedValue);
                                        }

                                        // Could be the right Input Register or a Copy
                                        if (cArg.DecryptedValueInt < SADDef.EecBankStartAddress)
                                        {
                                            if (cArg.DecryptedValueInt <= minRegAdr || cArg.DecryptedValueInt >= maxRegAdr) ciCi.InputRegister = regCpy;
                                        }
                                        break;
                                    }
                                }
                            }
                            else if (prevOpe.AddressInt != callOpe.AddressInt)
                            {
                                // Break Conditions - Same Routine Type
                                if (prevOpe.CallType == CallType.Call || prevOpe.CallType == CallType.ShortCall)
                                {
                                    if (Calibration.slRoutines.ContainsKey(Tools.UniqueAddress(prevOpe.ApplyOnBankNum, prevOpe.AddressJumpInt)))
                                    {
                                        if (((Routine)Calibration.slRoutines[Tools.UniqueAddress(prevOpe.ApplyOnBankNum, prevOpe.AddressJumpInt)]).Type == RoutineType.FunctionByte) break;
                                        if (((Routine)Calibration.slRoutines[Tools.UniqueAddress(prevOpe.ApplyOnBankNum, prevOpe.AddressJumpInt)]).Type == RoutineType.FunctionWord) break;
                                    }
                                }
                            }

                            // Input Register Identification
                            if (ciCi.InputRegister == string.Empty)
                            {
                                switch (prevOpe.OriginalInstruction.ToLower())
                                {
                                    case "ldw":
                                    case "ldzbw":
                                    case "ldsbw":
                                    case "ldb":
                                    case "ad2w":
                                    case "ad2b":
                                    case "ad3w":
                                    case "ad3b":
                                    case "stw":
                                    case "stb":
                                        if (prevOpe.OriginalInstruction.ToLower() == "stw" || prevOpe.OriginalInstruction.ToLower() == "stb")
                                        {
                                            cmpIndex = 0;
                                            cpyIndex = prevOpe.OperationParams.Length - 1;
                                        }
                                        else
                                        {
                                            cmpIndex = prevOpe.OperationParams.Length - 1;
                                            cpyIndex = 0;
                                        }
                                        if (prevOpe.OperationParams[cmpIndex].CalculatedParam == regCpy)
                                        {
                                            // Changing register, Including ShortRegisterPrefix for next searches
                                            regInt = Tools.InstructionRegisterAddress(prevOpe.OperationParams[cpyIndex].CalculatedParam);
                                            if (regInt >= 0)
                                            {
                                                regCpy = Tools.RegisterInstruction(string.Format("{0:x2}", regInt));
                                                if (!ioFunction.FunctionByte) regByteCpy = Tools.RegisterInstruction(string.Format("{0:x2}", regInt + 1));
                                                // Could be the right Input Register or a Copy
                                                if (regInt < SADDef.EecBankStartAddress)
                                                {
                                                    if (regInt <= minRegAdr || regInt >= maxRegAdr) ciCi.InputRegister = regCpy;
                                                }
                                            }
                                        }
                                        else if (prevOpe.OperationParams[cmpIndex].CalculatedParam == regByteCpy)
                                        {
                                            // Changing register, Including ShortRegisterPrefix for next searches
                                            regInt = Tools.InstructionRegisterAddress(prevOpe.OperationParams[cpyIndex].CalculatedParam);
                                            if (regInt >= 0)
                                            {
                                                regByteCpy = Tools.RegisterInstruction(string.Format("{0:x2}", regInt));
                                                regCpy = Tools.RegisterInstruction(string.Format("{0:x2}", regInt - 1));
                                                // Could be the right Input Register or a Copy
                                                if (regInt < SADDef.EecBankStartAddress)
                                                {
                                                    if (regInt <= minRegAdr || regInt >= maxRegAdr) ciCi.InputRegister = regByteCpy;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }

                            if (ciCi.InputRegister != string.Empty)
                            {
                                // Clean Up
                                ciCi.InputRegister = ciCi.InputRegister.Replace(SADDef.ShortRegisterPrefix, "").Replace(SADDef.LongRegisterPointerPrefix, "").Replace(SADDef.LongRegisterPointerSuffix, "").Replace("0" + SADDef.AdditionSeparator, "").Replace(SADDef.AdditionSeparator + "0", "");
                                if (ciCi.InputRegister.Contains(SADDef.AdditionSeparator))
                                {
                                    // [Rdc+5b] for example
                                    // It is OK
                                }
                                else
                                {
                                    regInt = Tools.InstructionRegisterAddress(ciCi.InputRegister);
                                    ciCi.InputRegister = string.Empty;
                                    if (regInt >= 0) ciCi.InputRegister = Convert.ToString(regInt, 16);
                                }
                                if (ciCi.InputRegister != string.Empty) break;
                            }
                        }
                    }
                    adjacentOperations = null;

                    // Output Register Detection - After Routine Call
                    adjacentOperations = getFollowingOPs(callOpe.AddressNextInt, 8, 99, true, true, true, true, true, true, true, true);
                    regCpy = SADDef.ShortRegisterPrefix + ioFunction.OutputRegister;
                    regByteCpy = "RXX";
                    if (!ioFunction.FunctionByte)
                    {
                        regInt = Tools.InstructionRegisterAddress(regCpy);
                        if (regInt >= 0) regByteCpy = Tools.RegisterInstruction(string.Format("{0:x2}", regInt + 1));
                    }

                    foreach (Operation postOpe in adjacentOperations)
                    {
                        if (postOpe != null)
                        {
                            // Break Conditions - Same Routine Type
                            if (postOpe.CallType == CallType.Call || postOpe.CallType == CallType.ShortCall)
                            {
                                if (Calibration.slRoutines.ContainsKey(Tools.UniqueAddress(postOpe.ApplyOnBankNum, postOpe.AddressJumpInt)))
                                {
                                    if (((Routine)Calibration.slRoutines[Tools.UniqueAddress(postOpe.ApplyOnBankNum, postOpe.AddressJumpInt)]).Type == RoutineType.FunctionByte) break;
                                    if (((Routine)Calibration.slRoutines[Tools.UniqueAddress(postOpe.ApplyOnBankNum, postOpe.AddressJumpInt)]).Type == RoutineType.FunctionWord) break;
                                }
                            }

                            // Output Register Identification
                            switch (postOpe.OriginalInstruction.ToLower())
                            {
                                case "ldb":
                                case "ldzbw":
                                case "ldsbw":
                                case "stb":
                                    // Only OutputRegister or Only OutputRegisterByte is changed
                                    if (postOpe.OriginalInstruction.ToLower() == "stb")
                                    {
                                        cmpIndex = postOpe.OperationParams.Length - 1;
                                        cpyIndex = 0;
                                    }
                                    else
                                    {
                                        cmpIndex = 0;
                                        cpyIndex = postOpe.OperationParams.Length - 1;
                                    }
                                    if (postOpe.OperationParams[cmpIndex].CalculatedParam == regCpy)
                                    {
                                        if (postOpe.OriginalInstruction.ToLower() == "ldsbw") ciCi.OutputRegisterSigned = true;

                                        // Changing register, Including ShortRegisterPrefix for next searches
                                        regInt = Tools.InstructionRegisterAddress(postOpe.OperationParams[cpyIndex].CalculatedParam);
                                        if (regInt >= 0)
                                        {
                                            regCpy = Tools.RegisterInstruction(string.Format("{0:x2}", regInt));
                                            // Could be the right Output Register or a Copy
                                            if (regInt < SADDef.EecBankStartAddress)
                                            {
                                                if (regInt <= minRegAdr || regInt >= maxRegAdr) ciCi.OutputRegister = regCpy;
                                            }
                                        }
                                    }
                                    else if (postOpe.OperationParams[cmpIndex].CalculatedParam == regByteCpy)
                                    {
                                        // Changing register, Including ShortRegisterPrefix for next searches
                                        regInt = Tools.InstructionRegisterAddress(postOpe.OperationParams[cpyIndex].CalculatedParam);
                                        if (regInt >= 0)
                                        {
                                            regByteCpy = Tools.RegisterInstruction(string.Format("{0:x2}", regInt));
                                            // Could be the right Input Register or a Copy
                                            if (regInt < SADDef.EecBankStartAddress)
                                            {
                                                if (regInt <= minRegAdr || regInt >= maxRegAdr) ciCi.OutputRegisterByte = regByteCpy;
                                            }
                                        }
                                    }
                                    break;
                                case "ldw":
                                case "stw":
                                    // OutputRegister and OutputRegisterByte are changed
                                    if (postOpe.OriginalInstruction.ToLower() == "stw")
                                    {
                                        cmpIndex = postOpe.OperationParams.Length - 1;
                                        cpyIndex = 0;
                                    }
                                    else
                                    {
                                        cmpIndex = 0;
                                        cpyIndex = postOpe.OperationParams.Length - 1;
                                    }
                                    if (postOpe.OperationParams[cmpIndex].CalculatedParam == regCpy)
                                    {
                                        // Changing register, Including ShortRegisterPrefix for next searches
                                        regInt = Tools.InstructionRegisterAddress(postOpe.OperationParams[cpyIndex].CalculatedParam);
                                        if (regInt >= 0)
                                        {
                                            regCpy = Tools.RegisterInstruction(string.Format("{0:x2}", regInt));
                                            if (!ioFunction.FunctionByte) regByteCpy = Tools.RegisterInstruction(string.Format("{0:x2}", regInt + 1));
                                            // Could be the right Output Register or a Copy
                                            if (regInt < SADDef.EecBankStartAddress)
                                            {
                                                if (regInt <= minRegAdr || regInt >= maxRegAdr)
                                                {
                                                    ciCi.OutputRegister = regCpy;
                                                    if (!ioFunction.FunctionByte) ciCi.OutputRegisterByte = regByteCpy;
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }

                            if (ciCi.OutputRegister != string.Empty)
                            {
                                // Clean Up
                                ciCi.OutputRegister = ciCi.OutputRegister.Replace(SADDef.ShortRegisterPrefix, "").Replace(SADDef.LongRegisterPointerPrefix, "").Replace(SADDef.LongRegisterPointerSuffix, "").Replace("0" + SADDef.AdditionSeparator, "").Replace(SADDef.AdditionSeparator + "0", "");
                                if (ciCi.OutputRegister.Contains(SADDef.AdditionSeparator))
                                {
                                    // [Rdc+5b] for example
                                    // It is OK
                                }
                                else
                                {
                                    regInt = Tools.InstructionRegisterAddress(ciCi.OutputRegister);
                                    ciCi.OutputRegister = string.Empty;
                                    if (regInt >= 0) ciCi.OutputRegister = Convert.ToString(regInt, 16);
                                }
                                if (ciCi.OutputRegister != string.Empty) regCpy = "RXX";    // No possible matching anymore
                            }
                            if (ciCi.OutputRegisterByte != string.Empty)
                            {
                                // Clean Up
                                ciCi.OutputRegisterByte = ciCi.OutputRegisterByte.Replace(SADDef.ShortRegisterPrefix, "").Replace(SADDef.LongRegisterPointerPrefix, "").Replace(SADDef.LongRegisterPointerSuffix, "").Replace("0" + SADDef.AdditionSeparator, "").Replace(SADDef.AdditionSeparator + "0", "");
                                if (ciCi.OutputRegisterByte.Contains(SADDef.AdditionSeparator))
                                {
                                    // [Rdc+5b] for example
                                    // It is OK
                                }
                                else
                                {
                                    regInt = Tools.InstructionRegisterAddress(ciCi.OutputRegisterByte);
                                    ciCi.OutputRegisterByte = string.Empty;
                                    if (regInt >= 0) ciCi.OutputRegisterByte = Convert.ToString(regInt, 16);
                                }
                                if (ciCi.OutputRegisterByte != string.Empty) regCpy = "RXX";    // No possible matching anymore
                            }

                            if (ciCi.OutputRegister != string.Empty && ciCi.OutputRegisterByte != string.Empty) break;
                        }
                    }
                    
                    adjacentOperations = null;
                }
            }
        }

        public void processCalibrationElementsRegistersTable(ref Table table)
        {
            Operation[] adjacentOperations = null;
            string regCpyCols = string.Empty;
            string regCpyRows = string.Empty;
            string regCpy = string.Empty;
            string regByteCpy = string.Empty;
            bool validReg1 = false;
            bool validReg2 = false;
            int minRegAdr = 0;
            int maxRegAdr = 0;

            minRegAdr = 0x23;
            maxRegAdr = 0x60;
            if (is8061)
            {
                minRegAdr = 0x11;
                maxRegAdr = 0x60;
            }

            foreach (RoutineCallInfoTable ciCi in table.RoutinesCallsInfos)
            {
                ciCi.ColsScalerRegister = string.Empty;
                ciCi.RowsScalerRegister = string.Empty;
                ciCi.ColsScalerFunctionUniqueAddress = string.Empty;
                ciCi.RowsScalerFunctionUniqueAddress = string.Empty;
                ciCi.OutputRegister = string.Empty;
                ciCi.OutputRegisterByte = string.Empty;
                ciCi.OutputRegisterSigned = false;
                Routine rRoutine = (Routine)Calibration.slRoutines[ciCi.RoutineUniqueAddress];
                if (rRoutine == null) continue;
                RoutineIOTable ioTable = ciCi.RoutineInputOutput;
                if (ioTable == null) continue;

                // Registers already provided, from S6x Input essentially
                int tmpRegAdr = -1;
                if (ioTable.TableColRegister != string.Empty)
                {
                    tmpRegAdr = Convert.ToInt32(ioTable.TableColRegister, 16);
                    if (tmpRegAdr >= maxRegAdr || tmpRegAdr <= minRegAdr) ciCi.ColsScalerRegister = Tools.RegisterInstruction(ioTable.TableColRegister);
                }
                if (ioTable.TableRowRegister != string.Empty)
                {
                    tmpRegAdr = Convert.ToInt32(ioTable.TableRowRegister, 16);
                    if (tmpRegAdr >= maxRegAdr || tmpRegAdr <= minRegAdr) ciCi.RowsScalerRegister = Tools.RegisterInstruction(ioTable.TableRowRegister);
                }
                if (ioTable.OutputRegister != string.Empty)
                {
                    tmpRegAdr = Convert.ToInt32(ioTable.OutputRegister, 16);
                    if (tmpRegAdr >= maxRegAdr || tmpRegAdr <= minRegAdr)
                    {
                        ciCi.OutputRegister = Tools.RegisterInstruction(ioTable.OutputRegister);
                        ciCi.OutputRegisterByte = Tools.RegisterInstruction(Convert.ToString(tmpRegAdr + 1, 16));
                    }
                }
                if (ioTable.isSignedOutputDefined) ciCi.OutputRegisterSigned = ioTable.SignedOutput;

                // Everything is already provided except Function Scaler Address, no way to find them now
                if (ciCi.ColsScalerRegister != string.Empty && ciCi.RowsScalerRegister != string.Empty && ciCi.OutputRegister != string.Empty && ioTable.isSignedOutputDefined)
                {
                    rRoutine = null;
                    ioTable = null;
                    continue;
                }

                if (ciCi.RoutineCallOpeUniqueAddress == string.Empty && ciCi.CalibrationElementOpeUniqueAddress != string.Empty)
                {
                    Operation calElemOpe = (Operation)slOPs[ciCi.CalibrationElementOpeUniqueAddress];
                    if (calElemOpe != null)
                    {
                        adjacentOperations = getFollowingOPs(calElemOpe.AddressInt, 16, 99, true, true, true, true, true, true, true, true);
                        foreach (Operation postOpe in adjacentOperations)
                        {
                            if (postOpe != null)
                            {
                                if (postOpe.CallType == CallType.Call || postOpe.CallType == CallType.ShortCall)
                                {
                                    if (Calibration.slRoutines.ContainsKey(Tools.UniqueAddress(postOpe.ApplyOnBankNum, postOpe.AddressJumpInt)))
                                    {
                                        if (((Routine)Calibration.slRoutines[Tools.UniqueAddress(postOpe.ApplyOnBankNum, postOpe.AddressJumpInt)]).Type == rRoutine.Type)
                                        {
                                            ciCi.RoutineCallOpeUniqueAddress = postOpe.UniqueAddress;
                                            if (!slRoutineOPsCalibElemUniqueAddresses.ContainsKey(ciCi.RoutineCallOpeUniqueAddress))
                                            {
                                                slRoutineOPsCalibElemUniqueAddresses.Add(ciCi.RoutineCallOpeUniqueAddress, table.UniqueAddress);
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        adjacentOperations = null;
                    }

                }

                Operation callOpe = null;
                if (ciCi.RoutineCallOpeUniqueAddress != string.Empty) callOpe = (Operation)slOPs[ciCi.RoutineCallOpeUniqueAddress];

                if (callOpe != null && rRoutine != null)
                {
                    // Input Registers Detection - Before Routine Call
                    //adjacentOperations = getPrecedingOPs(callOpe.AddressInt, 32, 99, true, true, true, true, true, true, true, true);
                    // 20200515 - PYM
                    // Now managing 1 Call Sub Level in getPrecedingOPs mode
                    adjacentOperations = getPrecedingOPs(callOpe.AddressInt, 32, 1);
                    regCpyCols = SADDef.ShortRegisterPrefix + ioTable.TableColRegister;
                    regCpyRows = SADDef.ShortRegisterPrefix + ioTable.TableRowRegister;
                    validReg1 = false;
                    validReg2 = false;
                    foreach (Operation prevOpe in adjacentOperations)
                    {
                        if (prevOpe != null)
                        {
                            // No Break Conditions when another Call To Table is Done - Registers can be shared between Table Routine Calls
                            if (prevOpe.AddressInt != callOpe.AddressInt)
                            {
                                if (prevOpe.CallType == CallType.Call || prevOpe.CallType == CallType.ShortCall)
                                {
                                    if (Calibration.slRoutines.ContainsKey(Tools.UniqueAddress(prevOpe.ApplyOnBankNum, prevOpe.AddressJumpInt)))
                                    {
                                        Routine fRoutine = (Routine)Calibration.slRoutines[Tools.UniqueAddress(prevOpe.ApplyOnBankNum, prevOpe.AddressJumpInt)];
                                        if (fRoutine.IOs != null)
                                        {
                                            RoutineIOFunction ioFunction = null;
                                            foreach (RoutineIO ioIO in fRoutine.IOs)
                                            {
                                                if (ioIO.GetType() == typeof(RoutineIOFunction))
                                                {
                                                    ioFunction = (RoutineIOFunction)ioIO;
                                                    break;
                                                }
                                            }
                                            if (ioFunction != null)
                                            {
                                                if (SADDef.ShortRegisterPrefix + ioFunction.OutputRegister == regCpyCols)
                                                {
                                                    if (slRoutineOPsCalibElemUniqueAddresses.ContainsKey(prevOpe.UniqueAddress))
                                                    {
                                                        ciCi.ColsScalerFunctionUniqueAddress = slRoutineOPsCalibElemUniqueAddresses[prevOpe.UniqueAddress].ToString();
                                                        regCpyCols = "RXX";
                                                    }
                                                }
                                                if (SADDef.ShortRegisterPrefix + ioFunction.OutputRegister == regCpyRows)
                                                {
                                                    if (slRoutineOPsCalibElemUniqueAddresses.ContainsKey(prevOpe.UniqueAddress))
                                                    {
                                                        ciCi.RowsScalerFunctionUniqueAddress = slRoutineOPsCalibElemUniqueAddresses[prevOpe.UniqueAddress].ToString();
                                                        regCpyRows = "RXX";
                                                    }
                                                }
                                                ioFunction = null;
                                            }
                                        }
                                        fRoutine = null;
                                    }
                                }
                            }

                            // Input Registers Identification
                            switch (prevOpe.OriginalInstruction.ToLower())
                            {
                                case "ldw":
                                case "ldzbw":
                                case "ldsbw":
                                case "ldb":
                                case "ad2w":
                                case "ad2b":
                                case "ad3w":
                                case "ad3b":
                                    //20180428
                                    //if (prevOpe.TranslatedParams[prevOpe.TranslatedParams.Length - 1] == regCpyCols)
                                    if (prevOpe.OperationParams[prevOpe.OperationParams.Length - 1].CalculatedParam == regCpyCols)
                                    {
                                        //20180428
                                        //regCpyCols = prevOpe.TranslatedParams[0]; // Changing register, Including ShortRegisterPrefix for next searches
                                        regCpyCols = prevOpe.OperationParams[0].CalculatedParam; // Changing register, Including ShortRegisterPrefix for next searches
                                        if (regCpyCols.StartsWith(SADDef.ShortRegisterPrefix))
                                        // Could be the right Input Register or a Copy
                                        {
                                            try
                                            {
                                                // Below R20 and Over R60 we consider it is the right Register
                                                if (Convert.ToInt32(regCpyCols.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) >= maxRegAdr) ciCi.ColsScalerRegister = regCpyCols;
                                                else if (Convert.ToInt32(regCpyCols.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) <= minRegAdr) ciCi.ColsScalerRegister = regCpyCols;
                                            }
                                            catch { }
                                        }
                                        else if (regCpyCols.StartsWith(SADDef.LongRegisterPointerPrefix))
                                        // Should be the right Input Register
                                        {
                                            ciCi.ColsScalerRegister = regCpyCols;
                                        }
                                    }
                                    //20180428
                                    //else if (prevOpe.TranslatedParams[prevOpe.TranslatedParams.Length - 1] == regCpyRows)
                                    else if (prevOpe.OperationParams[prevOpe.OperationParams.Length - 1].CalculatedParam == regCpyRows)
                                    {
                                        //20180428
                                        //regCpyRows = prevOpe.TranslatedParams[0]; // Changing register, Including ShortRegisterPrefix for next searches
                                        regCpyRows = prevOpe.OperationParams[0].CalculatedParam; // Changing register, Including ShortRegisterPrefix for next searches
                                        if (regCpyRows.StartsWith(SADDef.ShortRegisterPrefix))
                                        // Could be the right Input Register or a Copy
                                        {
                                            try
                                            {
                                                // Below R20 and Over R60 we consider it is the right Register
                                                if (Convert.ToInt32(regCpyRows.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) >= maxRegAdr) ciCi.RowsScalerRegister = regCpyRows;
                                                else if (Convert.ToInt32(regCpyRows.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) <= minRegAdr) ciCi.RowsScalerRegister = regCpyRows;
                                            }
                                            catch { }
                                        }
                                        else if (regCpyRows.StartsWith(SADDef.LongRegisterPointerPrefix))
                                        // Should be the right Input Register
                                        {
                                            ciCi.RowsScalerRegister = regCpyRows;
                                        }
                                    }
                                    break;
                                case "stw":
                                case "stb":
                                    //20180428
                                    //if (prevOpe.TranslatedParams[0] == regCpyCols)
                                    if (prevOpe.OperationParams[0].CalculatedParam == regCpyCols)
                                    {
                                        //20180428
                                        //regCpyCols = prevOpe.TranslatedParams[prevOpe.TranslatedParams.Length - 1]; // Changing register, Including ShortRegisterPrefix for next searches
                                        regCpyCols = prevOpe.OperationParams[prevOpe.OperationParams.Length - 1].CalculatedParam; // Changing register, Including ShortRegisterPrefix for next searches
                                        if (regCpyCols.StartsWith(SADDef.ShortRegisterPrefix))
                                        // Could be the right Input Register or a Copy
                                        {
                                            try
                                            {
                                                // Below R20 and Over R60 we consider it is the right Register
                                                if (Convert.ToInt32(regCpyCols.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) >= maxRegAdr) ciCi.ColsScalerRegister = regCpyCols;
                                                else if (Convert.ToInt32(regCpyCols.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) <= minRegAdr) ciCi.ColsScalerRegister = regCpyCols;
                                            }
                                            catch { }
                                        }
                                        else if (regCpyCols.StartsWith(SADDef.LongRegisterPointerPrefix))
                                        // Should be the right Input Register
                                        {
                                            ciCi.ColsScalerRegister = regCpyCols;
                                        }
                                    }
                                    //20180428
                                    //else if (prevOpe.TranslatedParams[0] == regCpyRows)
                                    else if (prevOpe.OperationParams[0].CalculatedParam == regCpyRows)
                                    {
                                        //20180428
                                        //regCpyRows = prevOpe.TranslatedParams[prevOpe.TranslatedParams.Length - 1]; // Changing register, Including ShortRegisterPrefix for next searches
                                        regCpyRows = prevOpe.OperationParams[prevOpe.OperationParams.Length - 1].CalculatedParam; // Changing register, Including ShortRegisterPrefix for next searches
                                        if (regCpyRows.StartsWith(SADDef.ShortRegisterPrefix))
                                        // Could be the right Input Register or a Copy
                                        {
                                            try
                                            {
                                                // Below R20 and Over R60 we consider it is the right Register
                                                if (Convert.ToInt32(regCpyRows.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) >= maxRegAdr) ciCi.RowsScalerRegister = regCpyRows;
                                                else if (Convert.ToInt32(regCpyRows.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) <= minRegAdr) ciCi.RowsScalerRegister = regCpyRows;
                                            }
                                            catch { }
                                        }
                                        else if (regCpyRows.StartsWith(SADDef.LongRegisterPointerPrefix))
                                        // Should be the right Input Register
                                        {
                                            ciCi.RowsScalerRegister = regCpyRows;
                                        }
                                    }
                                    break;
                            }

                            if (ciCi.ColsScalerRegister != string.Empty)
                            {
                                if (!validReg1)
                                {
                                    // Clean Up
                                    ciCi.ColsScalerRegister = ciCi.ColsScalerRegister.Replace(SADDef.ShortRegisterPrefix, "").Replace(SADDef.LongRegisterPointerPrefix, "").Replace(SADDef.LongRegisterPointerSuffix, "").Replace("0" + SADDef.AdditionSeparator, "").Replace(SADDef.AdditionSeparator + "0", "");
                                    if (ciCi.ColsScalerRegister.Contains(SADDef.AdditionSeparator))
                                    {
                                        // [Rdc+5b] for example
                                    }
                                    else
                                    {
                                        try
                                        {
                                            ciCi.ColsScalerRegister = Convert.ToString(Convert.ToInt32(ciCi.ColsScalerRegister, 16), 16);
                                            regCpyCols = "RXX";
                                            validReg1 = true;
                                        }
                                        catch
                                        {
                                            ciCi.ColsScalerRegister = string.Empty;
                                        }
                                    }
                                }
                            }
                            if (ciCi.RowsScalerRegister != string.Empty)
                            {
                                if (!validReg2)
                                {
                                    // Clean Up
                                    ciCi.RowsScalerRegister = ciCi.RowsScalerRegister.Replace(SADDef.ShortRegisterPrefix, "").Replace(SADDef.LongRegisterPointerPrefix, "").Replace(SADDef.LongRegisterPointerSuffix, "").Replace("0" + SADDef.AdditionSeparator, "").Replace(SADDef.AdditionSeparator + "0", "");
                                    if (ciCi.RowsScalerRegister.Contains(SADDef.AdditionSeparator))
                                    {
                                        // [Rdc+5b] for example
                                    }
                                    else
                                    {
                                        try
                                        {
                                            ciCi.RowsScalerRegister = Convert.ToString(Convert.ToInt32(ciCi.RowsScalerRegister, 16), 16);
                                            regCpyRows = "RXX";
                                            validReg2 = true;
                                        }
                                        catch
                                        {
                                            ciCi.RowsScalerRegister = string.Empty;
                                        }
                                    }
                                }
                            }

                            if ((ciCi.ColsScalerRegister != string.Empty || ciCi.ColsScalerFunctionUniqueAddress != string.Empty) && (ciCi.RowsScalerRegister != string.Empty || ciCi.RowsScalerFunctionUniqueAddress != string.Empty)) break;
                        }
                    }
                    adjacentOperations = null;

                    // Output Register Detection - After Routine Call
                    adjacentOperations = getFollowingOPs(callOpe.AddressNextInt, 8, 99, true, true, true, true, true, true, true, true);
                    regCpy = SADDef.ShortRegisterPrefix + ioTable.OutputRegister;
                    try { regByteCpy = SADDef.ShortRegisterPrefix + Convert.ToString(Convert.ToInt32(ioTable.OutputRegister, 16) + 1, 16); }
                    catch { regByteCpy = "RXX"; }

                    validReg1 = false;
                    validReg2 = false;
                    foreach (Operation postOpe in adjacentOperations)
                    {
                        if (postOpe != null)
                        {
                            // Break Conditions - Same Routine Type
                            if (postOpe.CallType == CallType.Call || postOpe.CallType == CallType.ShortCall)
                            {
                                if (Calibration.slRoutines.ContainsKey(Tools.UniqueAddress(postOpe.ApplyOnBankNum, postOpe.AddressJumpInt)))
                                {
                                    if (((Routine)Calibration.slRoutines[Tools.UniqueAddress(postOpe.ApplyOnBankNum, postOpe.AddressJumpInt)]).Type == RoutineType.TableByte) break;
                                    if (((Routine)Calibration.slRoutines[Tools.UniqueAddress(postOpe.ApplyOnBankNum, postOpe.AddressJumpInt)]).Type == RoutineType.TableWord) break;
                                }
                            }

                            // Output Register Identification
                            switch (postOpe.OriginalInstruction.ToLower())
                            {
                                case "ldw":
                                case "ldzbw":
                                case "ldsbw":
                                    //20180428
                                    //if (postOpe.TranslatedParams[0] == regCpy)
                                    if (postOpe.OperationParams[0].CalculatedParam == regCpy)
                                    {
                                        if (postOpe.OriginalInstruction.ToLower() == "ldsbw") ciCi.OutputRegisterSigned = true;

                                        //20180428
                                        //regCpy = postOpe.TranslatedParams[postOpe.TranslatedParams.Length - 1]; // Changing register, Including ShortRegisterPrefix for next searches
                                        regCpy = postOpe.OperationParams[postOpe.OperationParams.Length - 1].CalculatedParam; // Changing register, Including ShortRegisterPrefix for next searches
                                        if (regCpy.StartsWith(SADDef.ShortRegisterPrefix))
                                        // Could be the right Input Register or a Copy
                                        {
                                            try
                                            {
                                                // Below R20 and Over R60 we consider it is the right Register
                                                if (Convert.ToInt32(regCpy.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) >= maxRegAdr) ciCi.OutputRegister = regCpy;
                                                else if (Convert.ToInt32(regCpy.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) <= minRegAdr) ciCi.OutputRegister = regCpy;
                                            }
                                            catch { }
                                        }
                                        else if (regCpy.StartsWith(SADDef.LongRegisterPointerPrefix))
                                        // Should be the right Input Register
                                        {
                                            ciCi.OutputRegister = regCpy;
                                        }
                                    }
                                    break;
                                case "ldb":
                                    //20180428
                                    //if (postOpe.TranslatedParams[0] == regByteCpy)
                                    if (postOpe.OperationParams[0].CalculatedParam == regByteCpy)
                                    {
                                        //20180428
                                        //regByteCpy = postOpe.TranslatedParams[postOpe.TranslatedParams.Length - 1]; // Changing register, Including ShortRegisterPrefix for next searches
                                        regByteCpy = postOpe.OperationParams[postOpe.OperationParams.Length - 1].CalculatedParam; // Changing register, Including ShortRegisterPrefix for next searches
                                        if (regByteCpy.StartsWith(SADDef.ShortRegisterPrefix))
                                        // Could be the right Input Register or a Copy
                                        {
                                            try
                                            {
                                                // Below R20 and Over R60 we consider it is the right Register
                                                if (Convert.ToInt32(regByteCpy.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) >= maxRegAdr) ciCi.OutputRegisterByte = regByteCpy;
                                                else if (Convert.ToInt32(regByteCpy.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) <= minRegAdr) ciCi.OutputRegisterByte = regByteCpy;
                                            }
                                            catch { }
                                        }
                                        else if (regByteCpy.StartsWith(SADDef.LongRegisterPointerPrefix))
                                        // Should be the right Input Register
                                        {
                                            ciCi.OutputRegisterByte = regByteCpy;
                                        }
                                    }
                                    break;
                                case "stw":
                                    //20180428
                                    //if (postOpe.TranslatedParams[postOpe.TranslatedParams.Length - 1] == regCpy)
                                    if (postOpe.OperationParams[postOpe.OperationParams.Length - 1].CalculatedParam == regCpy)
                                    {
                                        //20180428
                                        //regCpy = postOpe.TranslatedParams[0]; // Changing register, Including ShortRegisterPrefix for next searches
                                        regCpy = postOpe.OperationParams[0].CalculatedParam; // Changing register, Including ShortRegisterPrefix for next searches
                                        if (regCpy.StartsWith(SADDef.ShortRegisterPrefix))
                                        // Could be the right Input Register or a Copy
                                        {
                                            try
                                            {
                                                // Below R20 and Over R60 we consider it is the right Register
                                                if (Convert.ToInt32(regCpy.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) >= maxRegAdr) ciCi.OutputRegister = regCpy;
                                                else if (Convert.ToInt32(regCpy.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) <= minRegAdr) ciCi.OutputRegister = regCpy;
                                            }
                                            catch { }
                                        }
                                        else if (regCpy.StartsWith(SADDef.LongRegisterPointerPrefix))
                                        // Should be the right Input Register
                                        {
                                            ciCi.OutputRegister = regCpy;
                                        }
                                    }
                                    break;
                                case "stb":
                                    //20180428
                                    //if (postOpe.TranslatedParams[postOpe.TranslatedParams.Length - 1] == regByteCpy)
                                    if (postOpe.OperationParams[postOpe.OperationParams.Length - 1].CalculatedParam == regByteCpy)
                                    {
                                        //20180428
                                        //regByteCpy = postOpe.TranslatedParams[0]; // Changing register, Including ShortRegisterPrefix for next searches
                                        regByteCpy = postOpe.OperationParams[0].CalculatedParam; // Changing register, Including ShortRegisterPrefix for next searches
                                        if (regByteCpy.StartsWith(SADDef.ShortRegisterPrefix))
                                        // Could be the right Input Register or a Copy
                                        {
                                            try
                                            {
                                                // Below R20 and Over R60 we consider it is the right Register
                                                if (Convert.ToInt32(regByteCpy.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) >= maxRegAdr) ciCi.OutputRegisterByte = regByteCpy;
                                                else if (Convert.ToInt32(regByteCpy.Substring(1).Replace(SADDef.AdditionSeparator + "0", ""), 16) <= minRegAdr) ciCi.OutputRegisterByte = regByteCpy;
                                            }
                                            catch { }
                                        }
                                        else if (regByteCpy.StartsWith(SADDef.LongRegisterPointerPrefix))
                                        // Should be the right Input Register
                                        {
                                            ciCi.OutputRegisterByte = regByteCpy;
                                        }
                                    }
                                    break;
                            }

                            if (ciCi.OutputRegister != string.Empty)
                            {
                                if (!validReg1)
                                {
                                    // Clean Up
                                    ciCi.OutputRegister = ciCi.OutputRegister.Replace(SADDef.ShortRegisterPrefix, "").Replace(SADDef.LongRegisterPointerPrefix, "").Replace(SADDef.LongRegisterPointerSuffix, "").Replace("0" + SADDef.AdditionSeparator, "").Replace(SADDef.AdditionSeparator + "0", "");
                                    if (ciCi.OutputRegister.Contains(SADDef.AdditionSeparator))
                                    {
                                        // [Rdc+5b] for example
                                        regCpy = "RXX";
                                        validReg1 = true;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            ciCi.OutputRegister = Convert.ToString(Convert.ToInt32(ciCi.OutputRegister, 16), 16);
                                            regCpy = "RXX";
                                            validReg1 = true;
                                        }
                                        catch
                                        {
                                            ciCi.OutputRegister = string.Empty;
                                        }
                                    }
                                }
                            }
                            if (ciCi.OutputRegisterByte != string.Empty)
                            {
                                if (!validReg2)
                                {
                                    // Clean Up
                                    ciCi.OutputRegisterByte = ciCi.OutputRegisterByte.Replace(SADDef.ShortRegisterPrefix, "").Replace(SADDef.LongRegisterPointerPrefix, "").Replace(SADDef.LongRegisterPointerSuffix, "").Replace("0" + SADDef.AdditionSeparator, "").Replace(SADDef.AdditionSeparator + "0", "");
                                    if (ciCi.OutputRegisterByte.Contains(SADDef.AdditionSeparator))
                                    {
                                        // [Rdc+5b] for example
                                        regByteCpy = "RXX";
                                        validReg2 = true;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            ciCi.OutputRegisterByte = Convert.ToString(Convert.ToInt32(ciCi.OutputRegisterByte, 16), 16);
                                            regByteCpy = "RXX";
                                            validReg2 = true;
                                        }
                                        catch
                                        {
                                            ciCi.OutputRegisterByte = string.Empty;
                                        }
                                    }
                                }
                            }

                            if (ciCi.OutputRegister != string.Empty && ciCi.OutputRegisterByte != string.Empty) break;
                        }
                    }

                    adjacentOperations = null;
                }
            }
        }

        public void findOtherElements(ref SADS6x S6x)
        {
            ArrayList alKnownRemoval = null;
            ArrayList alOtherRemoval = null;
            CalibrationElement opeCalElem = null;

            alKnownRemoval = new ArrayList();
            alOtherRemoval = new ArrayList();
            
            // Validating Possible Known Elements
            //      Known elements are 0xaaaa addresses lower or greater than 0x2000
            foreach (string uniqueAddress in alPossibleKnownElemOPsUniqueAddresses)
            {
                bool bKnown = false;
                bool bCheckRoutineCc = false;
                bool bCheckRoutineEc = false;

                int address = -1;
                string elemUniqueAddress = string.Empty;

                object[] arrPointersValues = null;

                Operation ope = (Operation)slOPs[uniqueAddress];

                if (ope == null) continue;

                if (ope.KnownElemAddress == string.Empty) continue;

                address = Convert.ToInt32(ope.KnownElemAddress, 16);

                // Exiting S6x Register, KnwonElemAddress stays in place to be translated, Existing OtherElemAddress does not apply anymore
                if (S6x.slRegisters.ContainsKey(Tools.RegisterUniqueAddress(address)))
                {
                    // Known, should not be managed by OtherElemAddress, later on with KnownElemAddress
                    ope.OtherElemAddress = string.Empty;
                    if (!alOtherRemoval.Contains(ope.UniqueAddress)) alKnownRemoval.Add(ope.UniqueAddress);
                    continue;
                }

                // Unique Address
                elemUniqueAddress = Tools.UniqueAddress(ope.ReadDataBankNum, address - SADDef.EecBankStartAddress);

                bKnown = false;

                // Known Reserved Address
                if (!bKnown) if (slReserved.ContainsKey(elemUniqueAddress)) bKnown = true;

                // Known Element - Adding reference to related Ope by the way
                if (!bKnown)
                {
                    if (Calibration.slCalibrationElements.ContainsKey(elemUniqueAddress))
                    {
                        bKnown = true;
                        if (!((CalibrationElement)Calibration.slCalibrationElements[elemUniqueAddress]).RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                        {
                            ((CalibrationElement)Calibration.slCalibrationElements[elemUniqueAddress]).RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                        }
                    }
                }
                if (!bKnown) if (Calibration.slExtTables.ContainsKey(elemUniqueAddress)) bKnown = true;
                if (!bKnown) if (Calibration.slExtFunctions.ContainsKey(elemUniqueAddress)) bKnown = true;
                if (!bKnown) if (Calibration.slExtScalars.ContainsKey(elemUniqueAddress)) bKnown = true;
                if (!bKnown) if (Calibration.slExtStructures.ContainsKey(elemUniqueAddress)) bKnown = true;

                // Known Ope/Call
                if (!bKnown)
                {
                    if (slOPs.ContainsKey(elemUniqueAddress))
                    {
                        if (ope.OperationParams.Length < 1) bKnown = true;

                        if (!bKnown)
                        {
                            // Specific exception - Structures initiated with adder to address can use an existing ope address
                            arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                            if (!((bool)arrPointersValues[0] && arrPointersValues.Length > 3)) bKnown = true;
                        }
                    }
                }

                // Known Additional Vector Source Address on Calibration Bank
                if (!bKnown)
                {
                    if (ope.ReadDataBankNum == Calibration.BankNum)
                    {
                        foreach (Vector vect in Calibration.slAdditionalVectors.Values)
                        {
                            if (vect.SourceAddress == ope.KnownElemAddress)
                            {
                                bKnown = true;
                                break;
                            }
                        }
                    }
                }

                // Known CC / EC Register ranges - 8061 only
                //  Based on Routine including CC or EC Registers
                if (!bKnown)
                {
                    if (is8061)
                    {
                        if (!bKnown)
                        {
                            if (address >= SADDef.CCMemory8061MinAdress && address <= SADDef.CCMemory8061MaxAdress)
                            {
                                if (ope.OriginalInstruction.ToLower() == "stb" || ope.OriginalInstruction.ToLower() == "stw")
                                // Can only be related with CC or EC
                                {
                                    ope.isCcRelated = true;
                                    bKnown = true;
                                }
                                else
                                // Routine has to be checked for Registers related with CC or EC
                                {
                                    bCheckRoutineCc = true;
                                }
                            }
                            else if (address >= SADDef.ECMemory8061MinAdress && address <= SADDef.ECMemory8061MaxAdress)
                            {
                                if (ope.OriginalInstruction.ToLower() == "stb" || ope.OriginalInstruction.ToLower() == "stw")
                                // Can only be related with CC or EC
                                {
                                    ope.isEcRelated = true;
                                    bKnown = true;
                                }
                                else
                                // Routine has to be checked for Registers related with CC or EC
                                {
                                    bCheckRoutineEc = true;
                                }
                            }
                        }
                        if (bCheckRoutineCc || bCheckRoutineEc)
                        // Reading Routine to find Operations related with CC or EC
                        {
                            Operation[] ops = getFollowingOPs(ope.AddressNextInt, 16, 1, true, true, false, false, false, false, false, false);
                            foreach (Operation opRes in ops)
                            {
                                if (opRes == null) break;
                                if (opRes.isCcRelated || opRes.isEcRelated)
                                {
                                    bKnown = true;
                                    break;
                                }
                                if (opRes.isReturn) break;
                                if (opRes.CallType == CallType.Jump || opRes.CallType == CallType.ShortJump) break;
                            }
                            ops = null;
                            if (!bKnown)
                            {
                                //ops = getPrecedingOPs(ope.AddressInt, 16, 1, true, true, false, false, false, false, false, false);
                                ops = getPrecedingOPs(ope.AddressInt, 16, 0);
                                foreach (Operation opRes in ops)
                                {
                                    if (opRes == null) break;
                                    if (opRes.isCcRelated || opRes.isEcRelated)
                                    {
                                        bKnown = true;
                                        break;
                                    }
                                    if (opRes.isReturn) break;
                                    if (opRes.AddressInt == opRes.InitialCallAddressInt)
                                    {
                                        if (Calibration.slCalls.ContainsKey(opRes.UniqueAddress))
                                        {
                                            switch (((Call)Calibration.slCalls[opRes.UniqueAddress]).CallType)
                                            {
                                                case CallType.Call:
                                                case CallType.ShortCall:
                                                case CallType.Jump:
                                                    bCheckRoutineCc = false;
                                                    bCheckRoutineEc = false;
                                                    break;
                                            }
                                            if (!bCheckRoutineCc && !bCheckRoutineEc) break;
                                        }
                                    }
                                }
                                ops = null;
                            }

                            if (bKnown)
                            {
                                if (bCheckRoutineCc) ope.isCcRelated = true;
                                else if (bCheckRoutineEc) ope.isEcRelated = true;
                            }
                        }
                    }
                }

                if (bKnown)
                // Known, should not be managed by OtherElemAddress, later on with KnownElemAddress
                {
                    ope.OtherElemAddress = string.Empty;
                    if (!alOtherRemoval.Contains(ope.UniqueAddress)) alOtherRemoval.Add(ope.UniqueAddress);
                }
                else
                // Unknown, no need to manage it anymore through KnownElemAddress
                {
                    ope.KnownElemAddress = string.Empty;
                    if (!alKnownRemoval.Contains(ope.UniqueAddress)) alKnownRemoval.Add(ope.UniqueAddress);
                }
            }

            foreach (string uniqueAddress in alKnownRemoval)
            {
                if (alPossibleKnownElemOPsUniqueAddresses.Contains(uniqueAddress)) alPossibleKnownElemOPsUniqueAddresses.Remove(uniqueAddress);
            }
            alKnownRemoval = null;

            foreach (string uniqueAddress in alOtherRemoval)
            {
                if (alPossibleOtherElemOPsUniqueAddresses.Contains(uniqueAddress)) alPossibleOtherElemOPsUniqueAddresses.Remove(uniqueAddress);
            }
            alOtherRemoval = null;

            // Validating Possible Other Elements
            // Stored in Ope.OtherElemAddress
            // Remove Invalid Ope.OtherElemAddress
            //      => Should not be a Reserved Address         => Now Managed with KnownElemAddress
            //      => Should not be an Ope/Call                => Now Managed with KnownElemAddress
            //      => Should not be an Additional Vector Source Address on Calibration Bank    => Now Managed with KnownElemAddress
            //      => Should not be in Calibration Part (BUT KEEP ADDRESS TO CREATE CAL ELEM STRUCT)
            //      => 65, A1 : Initial Assigned Register should be used as Pointer in next Ops, following Calls
            //          ex:
            //              R30 = aaaa;
            //              [1e00] = R30;   => Invalid because just used to be assigned in another register
            //      => [XXXX] : ldw or ldb => Scalar
            //      => [R54+XXXX] : Could be a start of struct
            // 2 Types

            // !!! When using ldw or ldb, address is by default on Calibration Bank,
            //      except if ApplyOnBankNum is different from BankNum !!!

            alKnownRemoval = new ArrayList();
            alOtherRemoval = new ArrayList();

            foreach (string uniqueAddress in alPossibleOtherElemOPsUniqueAddresses)
            {
                if (slOPs.ContainsKey(uniqueAddress))
                {
                    int address = -1;
                    string elemUniqueAddress = string.Empty;

                    // Operation where OtherAddress is used trough register copy (65, a1)
                    string regCpy = string.Empty;
                    string regCpyTb = string.Empty;
                    Operation regCpyOpe = null;

                    Operation ope = (Operation)slOPs[uniqueAddress];

                    if (ope.OtherElemAddress != string.Empty)
                    {
                        address = Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress;
                        elemUniqueAddress = Tools.UniqueAddress(ope.ReadDataBankNum, address);

                        //Elem bank should be available
                        if (ope.OtherElemAddress != string.Empty && !Calibration.Info.slBanksInfos.ContainsKey(ope.ReadDataBankNum)) ope.OtherElemAddress = string.Empty;
                        
                        //Elem address should correspond to its bank
                        if (ope.OtherElemAddress != string.Empty)
                        {
                            if (ope.ReadDataBankNum == Num && (address < AddressInternalInt || address > AddressInternalEndInt))
                            {
                                ope.OtherElemAddress = string.Empty;
                            }
                            else if (ope.ReadDataBankNum == Calibration.BankNum && (address < Calibration.BankAddressInt || address > Calibration.BankAddressEndInt))
                            {
                                ope.OtherElemAddress = string.Empty;
                            }
                        }
                        
                        //Elem Should not be a Reserved Address
                        //      Managed with KnownElemAddress
                        //if (ope.OtherElemAddress != string.Empty && slReserved.ContainsKey(elemUniqueAddress)) ope.OtherElemAddress = string.Empty;
                        
                        //Elem Should not be an Ope/Call
                        //      Managed with KnownElemAddress
                        //if (ope.OtherElemAddress != string.Empty && slOPs.ContainsKey(elemUniqueAddress)) ope.OtherElemAddress = string.Empty;

                        //Elem Should not be an Additional Vector Source Address on Calibration Bank
                        //      Managed with KnownElemAddress
                        /*
                        if (ope.OtherElemAddress != string.Empty && ope.ReadDataBankNum == Calibration.BankNum)
                        {
                            foreach (Vector vect in Calibration.slAdditionalVectors.Values)
                            {
                                if (vect.SourceAddress == ope.OtherElemAddress)
                                {
                                    ope.OtherElemAddress = string.Empty;
                                    break;
                                }
                            }
                        }
                        */

                        //Initial Assigned Register should be used as Pointer in next Ops, following Calls
                        if (ope.OtherElemAddress != string.Empty)
                        {
                            switch (ope.OriginalOPCode.ToLower())
                            {
                                case "65":
                                case "a1":
                                    regCpy = ope.OriginalOpArr[ope.OriginalOpArr.Length - 1];
                                    regCpyTb = string.Empty;
                                    if (Convert.ToInt32(regCpy, 16) % 2 == 0) regCpyTb = Convert.ToString(Convert.ToInt32(regCpy, 16) + 1, 16);
                                    // 20181119 - Moving from getFollowingOPs(16) to getFollowingOpsTree(8) - Possible consequences
                                    //Operation[] ops = getFollowingOPs(ope.AddressNextInt, 16, 1, false, true, true, true, true, true, false, true);
                                    Operation[] ops = getFollowingOpsTree(ope.AddressNextInt, 8, true);
                                    foreach (Operation nextOpe in ops)
                                    {
                                        // 20181119 - Not with getFollowingOpsTree
                                        // Do not use with getFollowingOpsTree
                                        //if (nextOpe == null) break;
                                        //if (nextOpe.isReturn) break;

                                        switch (nextOpe.OriginalInstruction.ToLower())
                                        {
                                            case "stw":
                                            case "stb":
                                                //Invalid because just used to be stored in another register
                                                if (nextOpe.OriginalOpArr[nextOpe.OriginalOpArr.Length - 1] == regCpy)
                                                {
                                                    ope.OtherElemAddress = string.Empty;
                                                    regCpy = string.Empty;    // Break Condition only
                                                    regCpyTb = string.Empty;
                                                }
                                                break;
                                            default:
                                                if (nextOpe.OriginalOPCode.Length == 2)
                                                {
                                                    // Address Register copied in another register for parallelism
                                                    if (nextOpe.OriginalOPCode.ToLower() == "a0")   // ldw
                                                    {
                                                        regCpy = nextOpe.OriginalOpArr[nextOpe.OriginalOpArr.Length - 1];
                                                        regCpyTb = string.Empty;
                                                        if (Convert.ToInt32(regCpy, 16) % 2 == 0) regCpyTb = Convert.ToString(Convert.ToInt32(regCpy, 16) + 1, 16);
                                                    }
                                                    else
                                                    {
                                                        switch (nextOpe.OriginalOPCode.Substring(0, 1).ToLower())
                                                        {
                                                            // Pointer to register or register + adder is used (always in first place)
                                                            //  Many possible operations, always in pointer mode
                                                            case "4":
                                                            case "5":
                                                            case "6":
                                                            case "7":
                                                            case "8":
                                                            case "9":
                                                            case "a":
                                                            case "b":
                                                                switch (nextOpe.OriginalOPCode.Substring(1, 1).ToLower())
                                                                {
                                                                    case "2":
                                                                    case "3":
                                                                    case "6":
                                                                    case "7":
                                                                    case "a":
                                                                    case "b":
                                                                    case "e":
                                                                    case "f":
                                                                        if (ope.OriginalOpArr[0].ToLower() == "fe") // SIGND/ALT
                                                                        {
                                                                            //Valid because used as base pointer for another calculation
                                                                            if (nextOpe.OriginalOpArr[2] == regCpy || nextOpe.OriginalOpArr[2] == regCpyTb) regCpyOpe = nextOpe;
                                                                        }
                                                                        else
                                                                        {
                                                                            //Valid because used as base pointer for another calculation
                                                                            if (nextOpe.OriginalOpArr[1] == regCpy || nextOpe.OriginalOpArr[1] == regCpyTb) regCpyOpe = nextOpe;
                                                                        }
                                                                        break;
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                break;
                                        }
                                        if (regCpy == string.Empty || regCpyOpe != null) break;
                                    }
                                    ops = null;

                                    // No Interesting part was found, it is not an Element
                                    if (regCpyOpe == null)
                                    {
                                        ope.OtherElemAddress = string.Empty;
                                        regCpy = string.Empty;
                                        regCpyTb = string.Empty;
                                    }
                                    break;
                            }
                        }
                    }
                    if (ope.OtherElemAddress == string.Empty)
                    {
                        alOtherRemoval.Add(ope.UniqueAddress);
                    }

                    // 20181108 - First Other Elements will be parsed through predefined signatures, for performance reasons
                    if (ope.OtherElemAddress != string.Empty)
                    {
                        findOtherElementSignatureIdentification(address, ref ope, ref S6x);
                        if (ope.OtherElemAddress == string.Empty) if (!alOtherRemoval.Contains(ope.UniqueAddress)) alOtherRemoval.Add(ope.UniqueAddress);
                        if (ope.KnownElemAddress == string.Empty) if (!alKnownRemoval.Contains(ope.UniqueAddress)) alKnownRemoval.Add(ope.UniqueAddress);
                    }

                    // Then Classically
                    if (ope.OtherElemAddress != string.Empty)
                    {
                        bool assignScalar = false;
                        bool assignStructure = false;
                        bool isByte = false;
                        bool isSigned = false;
                        bool isCalElement = false;

                        SADOPCode regCpyOpCode = null;

                        switch (ope.OriginalOPCode.ToLower())
                        {
                            // Scalar Address on Calibration Bank or Not
                            //a3,01,0e,88,54    ldw   R54,[880e]     R54 = [880e];      => CheckSum_Calculation_Start_Addr Outside Calibration Addresses
                            //a3,01,10,88,56    ldw   R56,[8810]     R56 = [8810];      => CheckSum_Calculation_End_Addres Outside Calibration Addresses
                            //      => a3,01 & b3,01, direct assignment, no additional register except the 00 one, otherwise it is a structure like a list of scalars
                            // Structure Address
                            //      => 65, A1 : Initial Assigned Register should be used as Pointer in next Ops, following Calls
                            //      => [R54+XXXX] : Could be a start of struct
                            case "65":
                            case "a1":
                                assignStructure = true;
                                isByte = true;
                                if (regCpyOpe != null)
                                {
                                    isSigned = regCpyOpe.OriginalOpArr[0].ToLower() == "fe";    // SIGND/ALT
                                    regCpyOpCode = (SADOPCode)Calibration.slOPCodes[regCpyOpe.OriginalOPCode];
                                    if (regCpyOpCode != null)
                                    {
                                        switch (regCpyOpCode.Type)
                                        {
                                            case OPCodeType.WordOP:
                                                isByte = false;
                                                break;
                                            case OPCodeType.MixedOP:
                                                if (regCpyOpCode.Instruction.ToLower() == "ldsbw") isSigned = true;
                                                break;
                                        }
                                        regCpyOpCode = null;
                                    }
                                }
                                break;
                            case "43":  // an3w
                            case "47":  // ad3w
                            case "4b":  // sb3w
                            case "4f":  // ml3w
                            case "63":  // an2w
                            case "67":  // ad2w
                            case "6b":  // sb2w
                            case "6f":  // ml2w
                            case "83":  // orrw
                            case "87":  // xrw
                            case "8b":  // cmpw
                            case "8f":  // divw
                            case "a3":  // ldw
                            case "a7":  // adcw
                            case "ab":  // sbbw
                            case "c3":  // stw
                                if (ope.OriginalOpArr[0].ToLower() == "fe") // SIGND/ALT
                                {
                                    assignScalar = (ope.OriginalOpArr[2] == "01");
                                    isSigned = true;
                                }
                                else
                                {
                                    assignScalar = (ope.OriginalOpArr[1] == "01");
                                    isSigned = false;
                                }
                                assignStructure = !assignScalar;
                                isByte = false;
                                break;
                            case "53":  // an3b
                            case "57":  // ad3b
                            case "5b":  // sb3b
                            case "5f":  // ml3b
                            case "73":  // an2b
                            case "77":  // ad2b
                            case "7b":  // sb2b
                            case "7f":  // ml2b
                            case "93":  // orrb
                            case "97":  // xrb
                            case "9b":  // cmpb
                            case "9f":  // divb
                            case "af":  // ldzbw
                            case "b3":  // ldb
                            case "b7":  // adcb
                            case "bb":  // sbbb
                            case "c7":  // stb
                                if (ope.OriginalOpArr[0].ToLower() == "fe") // SIGND/ALT
                                {
                                    assignScalar = (ope.OriginalOpArr[2] == "01");
                                    isSigned = true;
                                }
                                else
                                {
                                    assignScalar = (ope.OriginalOpArr[1] == "01");
                                    isSigned = false;
                                }
                                assignStructure = !assignScalar;
                                isByte = true;
                                break;
                            case "bf":
                                assignScalar = (ope.OriginalOpArr[1] == "01");
                                assignStructure = !assignScalar;
                                isByte = true;
                                isSigned = true;
                                break;
                        }
                        if (assignScalar)
                        // a3,01 & b3,01, direct assignment only, no additional register except the 00 one, otherwise it is a structure like a list of scalars
                        {
                            Scalar scalar = new Scalar(ope.ReadDataBankNum, address, isByte, isSigned);
                            if (scalar.BankNum == Num) scalar.AddressBinInt = AddressBinInt + scalar.AddressInt;
                            else if (scalar.BankNum == Calibration.BankNum) scalar.AddressBinInt = Calibration.BankAddressBinInt + scalar.AddressInt;
                            else scalar.AddressBinInt = -1; // Will Be Managed in processOtherElements or Calibration.readCalibrationElements

                            isCalElement = (scalar.AddressBinInt >= Calibration.AddressBinInt && scalar.AddressBinInt <= Calibration.AddressBinEndInt);
                            if (isCalElement)
                            // Calibration Element
                            {
                                if (Calibration.slCalibrationElements.ContainsKey(scalar.UniqueAddress))
                                // Calibration Element already created, reference is assigned to Ope
                                {
                                    if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                    opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[scalar.UniqueAddress];
                                    ope.alCalibrationElems.Add(opeCalElem);
                                    if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                    {
                                        opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                    }
                                    opeCalElem = null;
                                }
                                else
                                // Calibration Element creation
                                {
                                    // RBase Calculation
                                    foreach (RBase rBase in Calibration.slRbases.Values)
                                    {
                                        if (scalar.AddressBinInt >= rBase.AddressBinInt && scalar.AddressBinInt <= rBase.AddressBinEndInt)
                                        {
                                            scalar.RBase = rBase.Code;
                                            scalar.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code.ToLower() + SADDef.AdditionSeparator + Convert.ToString(scalar.AddressBinInt - rBase.AddressBinInt, 16);
                                            break;
                                        }
                                    }
                                    if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                    opeCalElem = new CalibrationElement(scalar.BankNum, scalar.RBase);
                                    opeCalElem.AddressBinInt = scalar.AddressBinInt;
                                    opeCalElem.AddressInt = scalar.AddressInt;
                                    opeCalElem.RBaseCalc = scalar.RBaseCalc;
                                    if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                    {
                                        opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                    }
                                 
                                    // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                                    if (!S6x.isS6xProcessTypeConflict(scalar.UniqueAddress, typeof(Scalar)))
                                    {
                                        opeCalElem.ScalarElem = scalar;
                                    }

                                    ope.alCalibrationElems.Add(opeCalElem);
                                    Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                                    opeCalElem = null;
                                }

                                if (!alOtherRemoval.Contains(ope.UniqueAddress)) alOtherRemoval.Add(ope.UniqueAddress);
                                if (!alKnownRemoval.Contains(ope.UniqueAddress)) alKnownRemoval.Add(ope.UniqueAddress);

                                if (!alCalibElemOPsUniqueAddresses.Contains(ope.UniqueAddress)) alCalibElemOPsUniqueAddresses.Add(ope.UniqueAddress);

                                ope.OtherElemAddress = string.Empty;
                                ope.KnownElemAddress = string.Empty;
                            }
                            else
                            // Other Element
                            {
                                // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                                if (!S6x.isS6xProcessTypeConflict(scalar.UniqueAddress, typeof(Scalar)))
                                {
                                    scalar.RBaseCalc = ope.OtherElemAddress;

                                    if (!alKnownRemoval.Contains(ope.UniqueAddress)) alKnownRemoval.Add(ope.UniqueAddress);
                                    ope.KnownElemAddress = string.Empty;

                                    if (Calibration.slExtScalars.ContainsKey(scalar.UniqueAddress))
                                    {
                                        if (!((Scalar)Calibration.slExtScalars[scalar.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                        {
                                            ((Scalar)Calibration.slExtScalars[scalar.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                        }
                                    }
                                    else if (Calibration.slExtStructures.ContainsKey(scalar.UniqueAddress))
                                    {
                                        if (!((Structure)Calibration.slExtStructures[scalar.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                        {
                                            ((Structure)Calibration.slExtStructures[scalar.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                        }
                                    }
                                    else
                                    {
                                        if (!scalar.OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) scalar.OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                        Calibration.slExtScalars.Add(scalar.UniqueAddress, scalar);
                                    }
                                }
                            }

                            scalar = null;
                        }
                        else if (assignStructure && address != 0)
                        // It is a structure like a list of scalar
                        //      Checksum Start Address is detected as structure in Checksum calculation routine, it is not required and can generate output issues
                        {
                            Structure structure = new Structure();
                            structure.BankNum = ope.ReadDataBankNum;
                            structure.AddressInt = address;
                            if (structure.BankNum == Num) structure.AddressBinInt = AddressBinInt + structure.AddressInt;
                            else if (structure.BankNum == Calibration.BankNum) structure.AddressBinInt = Calibration.BankAddressBinInt + structure.AddressInt;
                            else structure.AddressBinInt = -1; // Will Be Managed in processOtherElements or Calibration.readCalibrationElements

                            // Defaulted
                            structure.Defaulted = true;
                            structure.StructDefString = (isByte) ? "ByteHex" : "WordHex";
                            structure.Number = 1;

                            isCalElement = (structure.AddressBinInt >= Calibration.AddressBinInt && structure.AddressBinInt <= Calibration.AddressBinEndInt);
                            if (isCalElement)
                            // Calibration Element
                            {
                                if (Calibration.slCalibrationElements.ContainsKey(structure.UniqueAddress))
                                // Calibration Element already created, reference is assigned to Ope
                                {
                                    if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                    opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[structure.UniqueAddress];
                                    ope.alCalibrationElems.Add(opeCalElem);
                                    if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                    {
                                        opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                    }
                                    opeCalElem = null;
                                }
                                else
                                // Calibration Element creation
                                {
                                    // RBase Calculation
                                    foreach (RBase rBase in Calibration.slRbases.Values)
                                    {
                                        if (structure.AddressBinInt >= rBase.AddressBinInt && structure.AddressBinInt <= rBase.AddressBinEndInt)
                                        {
                                            structure.RBase = rBase.Code;
                                            structure.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code.ToLower() + SADDef.AdditionSeparator + Convert.ToString(structure.AddressBinInt - rBase.AddressBinInt, 16);
                                            break;
                                        }
                                    }
                                    if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                    opeCalElem = new CalibrationElement(structure.BankNum, structure.RBase);
                                    opeCalElem.AddressBinInt = structure.AddressBinInt;
                                    opeCalElem.AddressInt = structure.AddressInt;
                                    opeCalElem.RBaseCalc = structure.RBaseCalc;
                                    if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                    {
                                        opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                    }

                                    // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                                    if (!S6x.isS6xProcessTypeConflict(structure.UniqueAddress, typeof(Structure)))
                                    {
                                        opeCalElem.StructureElem = structure;
                                    }

                                    ope.alCalibrationElems.Add(opeCalElem);
                                    Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                                    opeCalElem = null;
                                }

                                if (!alOtherRemoval.Contains(ope.UniqueAddress)) alOtherRemoval.Add(ope.UniqueAddress);
                                if (!alKnownRemoval.Contains(ope.UniqueAddress)) alKnownRemoval.Add(ope.UniqueAddress);

                                if (!alCalibElemOPsUniqueAddresses.Contains(ope.UniqueAddress)) alCalibElemOPsUniqueAddresses.Add(ope.UniqueAddress);

                                ope.OtherElemAddress = string.Empty;
                                ope.KnownElemAddress = string.Empty;
                            }
                            else
                            // Other Element
                            {
                                // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                                if (!S6x.isS6xProcessTypeConflict(structure.UniqueAddress, typeof(Structure)))
                                {
                                    structure.RBaseCalc = ope.OtherElemAddress;

                                    if (!alKnownRemoval.Contains(ope.UniqueAddress)) alKnownRemoval.Add(ope.UniqueAddress);
                                    ope.KnownElemAddress = string.Empty;

                                    if (Calibration.slExtScalars.ContainsKey(structure.UniqueAddress))
                                    {
                                        if (!((Scalar)Calibration.slExtScalars[structure.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                        {
                                            ((Scalar)Calibration.slExtScalars[structure.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                        }
                                    }
                                    else if (Calibration.slExtStructures.ContainsKey(structure.UniqueAddress))
                                    {
                                        if (!((Structure)Calibration.slExtStructures[structure.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                        {
                                            ((Structure)Calibration.slExtStructures[structure.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                        }
                                    }
                                    else
                                    {
                                        if (!structure.OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) structure.OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                        Calibration.slExtStructures.Add(structure.UniqueAddress, structure);
                                    }
                                }
                            }

                            structure = null;
                        }
                    }
                    ope = null;
                }
            }

            foreach (string uniqueAddress in alKnownRemoval)
            {
                if (alPossibleKnownElemOPsUniqueAddresses.Contains(uniqueAddress)) alPossibleKnownElemOPsUniqueAddresses.Remove(uniqueAddress);
            }
            alKnownRemoval = null;

            foreach (string uniqueAddress in alOtherRemoval)
            {
                if (alPossibleOtherElemOPsUniqueAddresses.Contains(uniqueAddress)) alPossibleOtherElemOPsUniqueAddresses.Remove(uniqueAddress);
            }
            alOtherRemoval = null;
        }

        private void findOtherElementSignatureIdentification(int address, ref Operation ope, ref SADS6x S6x)
        {
            S6xElementSignature foundSignature = null;

            foreach (S6xElementSignature elementSignature in S6x.slProcessElementsSignatures.Values)
            {
                if (elementSignature.matchSignature(ref ope, this, is8061))
                {
                    foundSignature = elementSignature;
                    break;
                }
            }

            if (foundSignature == null) return;

            foreach (S6xElementSignature elementSignature in S6x.slProcessElementsSignatures.Values)
            {
                if (elementSignature.Skip || elementSignature.Found) continue;
                if (foundSignature.UniqueKey == elementSignature.UniqueKey) elementSignature.Found = true;
            }

            bool isCalElement = false;
            CalibrationElement opeCalElem = null;

            if (foundSignature.Scalar != null)
            {
                Scalar scalar = new Scalar();
                scalar.BankNum = ope.ReadDataBankNum;
                scalar.AddressInt = address;
                if (scalar.BankNum == Num) scalar.AddressBinInt = AddressBinInt + scalar.AddressInt;
                else if (scalar.BankNum == Calibration.BankNum) scalar.AddressBinInt = Calibration.BankAddressBinInt + scalar.AddressInt;
                else scalar.AddressBinInt = -1; // Will Be Managed in processOtherElements or Calibration.readCalibrationElements

                scalar.ShortLabel = foundSignature.Scalar.ShortLabel;
                scalar.Label = foundSignature.Scalar.Label;

                scalar.S6xElementSignatureSource = foundSignature;

                scalar.Byte = foundSignature.Scalar.Byte;
                scalar.Word = !scalar.Byte;
                scalar.Signed = foundSignature.Scalar.Signed;
                scalar.UnSigned = !scalar.Signed;

                isCalElement = (scalar.AddressBinInt >= Calibration.AddressBinInt && scalar.AddressBinInt <= Calibration.AddressBinEndInt);
                if (isCalElement)
                // Calibration Element
                {
                    if (Calibration.slCalibrationElements.ContainsKey(scalar.UniqueAddress))
                    // Calibration Element already created, reference is assigned to Ope
                    {
                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                        opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[scalar.UniqueAddress];
                        ope.alCalibrationElems.Add(opeCalElem);
                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                        {
                            opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                        }
                        opeCalElem = null;
                    }
                    else
                    // Calibration Element creation
                    {
                        // RBase Calculation
                        foreach (RBase rBase in Calibration.slRbases.Values)
                        {
                            if (scalar.AddressBinInt >= rBase.AddressBinInt && scalar.AddressBinInt <= rBase.AddressBinEndInt)
                            {
                                scalar.RBase = rBase.Code;
                                scalar.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code.ToLower() + SADDef.AdditionSeparator + Convert.ToString(scalar.AddressBinInt - rBase.AddressBinInt, 16);
                                break;
                            }
                        }
                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                        opeCalElem = new CalibrationElement(scalar.BankNum, scalar.RBase);
                        opeCalElem.AddressBinInt = scalar.AddressBinInt;
                        opeCalElem.AddressInt = scalar.AddressInt;
                        opeCalElem.RBaseCalc = scalar.RBaseCalc;
                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                        {
                            opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                        }

                        // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                        if (!S6x.isS6xProcessTypeConflict(scalar.UniqueAddress, typeof(Scalar)))
                        {
                            opeCalElem.ScalarElem = scalar;
                        }

                        ope.alCalibrationElems.Add(opeCalElem);
                        Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                        opeCalElem = null;
                    }

                    if (!alCalibElemOPsUniqueAddresses.Contains(ope.UniqueAddress)) alCalibElemOPsUniqueAddresses.Add(ope.UniqueAddress);

                    ope.OtherElemAddress = string.Empty;
                    ope.KnownElemAddress = string.Empty;
                }
                else
                // Other Element
                {
                    // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                    if (!S6x.isS6xProcessTypeConflict(scalar.UniqueAddress, typeof(Scalar)))
                    {
                        scalar.RBaseCalc = ope.OtherElemAddress;

                        ope.KnownElemAddress = string.Empty;

                        if (Calibration.slExtScalars.ContainsKey(scalar.UniqueAddress))
                        {
                            if (!((Scalar)Calibration.slExtScalars[scalar.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                            {
                                ((Scalar)Calibration.slExtScalars[scalar.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            }
                        }
                        else if (Calibration.slExtStructures.ContainsKey(scalar.UniqueAddress))
                        {
                            if (!((Structure)Calibration.slExtStructures[scalar.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                            {
                                ((Structure)Calibration.slExtStructures[scalar.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            }
                        }
                        else
                        {
                            if (!scalar.OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) scalar.OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            Calibration.slExtScalars.Add(scalar.UniqueAddress, scalar);
                        }
                    }
                }

                if (foundSignature.Information == null) foundSignature.Information = string.Empty;
                if (foundSignature.Information != string.Empty) foundSignature.Information += "\r\n";
                foundSignature.Information += "Signature has generated element at address " + scalar.UniqueAddressHex;

                scalar = null;
            }
            else if (foundSignature.Function != null)
            {
                Function function = new Function();
                function.BankNum = ope.ReadDataBankNum;
                function.AddressInt = address;
                if (function.BankNum == Num) function.AddressBinInt = AddressBinInt + function.AddressInt;
                else if (function.BankNum == Calibration.BankNum) function.AddressBinInt = Calibration.BankAddressBinInt + function.AddressInt;
                else function.AddressBinInt = -1; // Will Be Managed in processOtherElements or Calibration.readCalibrationElements

                function.ShortLabel = foundSignature.Function.ShortLabel;
                function.Label = foundSignature.Function.Label;

                function.S6xElementSignatureSource = foundSignature;

                function.ByteInput = foundSignature.Function.ByteInput;
                function.ByteOutput = foundSignature.Function.ByteOutput;

                isCalElement = (function.AddressBinInt >= Calibration.AddressBinInt && function.AddressBinInt <= Calibration.AddressBinEndInt);
                if (isCalElement)
                // Calibration Element
                {
                    if (Calibration.slCalibrationElements.ContainsKey(function.UniqueAddress))
                    // Calibration Element already created, reference is assigned to Ope
                    {
                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                        opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[function.UniqueAddress];
                        ope.alCalibrationElems.Add(opeCalElem);
                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                        {
                            opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                        }
                        opeCalElem = null;
                    }
                    else
                    // Calibration Element creation
                    {
                        // RBase Calculation
                        foreach (RBase rBase in Calibration.slRbases.Values)
                        {
                            if (function.AddressBinInt >= rBase.AddressBinInt && function.AddressBinInt <= rBase.AddressBinEndInt)
                            {
                                function.RBase = rBase.Code;
                                function.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code.ToLower() + SADDef.AdditionSeparator + Convert.ToString(function.AddressBinInt - rBase.AddressBinInt, 16);
                                break;
                            }
                        }
                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                        opeCalElem = new CalibrationElement(function.BankNum, function.RBase);
                        opeCalElem.AddressBinInt = function.AddressBinInt;
                        opeCalElem.AddressInt = function.AddressInt;
                        opeCalElem.RBaseCalc = function.RBaseCalc;
                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                        {
                            opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                        }

                        // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                        if (!S6x.isS6xProcessTypeConflict(function.UniqueAddress, typeof(Function)))
                        {
                            opeCalElem.FunctionElem = function;
                        }

                        ope.alCalibrationElems.Add(opeCalElem);
                        Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                        opeCalElem = null;
                    }

                    if (!alCalibElemOPsUniqueAddresses.Contains(ope.UniqueAddress)) alCalibElemOPsUniqueAddresses.Add(ope.UniqueAddress);

                    ope.OtherElemAddress = string.Empty;
                    ope.KnownElemAddress = string.Empty;
                }
                else
                // Other Element
                {
                    // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                    if (!S6x.isS6xProcessTypeConflict(function.UniqueAddress, typeof(Function)))
                    {
                        function.RBaseCalc = ope.OtherElemAddress;

                        ope.KnownElemAddress = string.Empty;

                        if (Calibration.slExtFunctions.ContainsKey(function.UniqueAddress))
                        {
                            if (!((Function)Calibration.slExtFunctions[function.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                            {
                                ((Function)Calibration.slExtFunctions[function.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            }
                        }
                        else if (Calibration.slExtStructures.ContainsKey(function.UniqueAddress))
                        {
                            if (!((Structure)Calibration.slExtStructures[function.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                            {
                                ((Structure)Calibration.slExtStructures[function.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            }
                        }
                        else
                        {
                            if (!function.OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) function.OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            Calibration.slExtFunctions.Add(function.UniqueAddress, function);
                        }
                    }
                }

                if (foundSignature.Information == null) foundSignature.Information = string.Empty;
                if (foundSignature.Information != string.Empty) foundSignature.Information += "\r\n";
                foundSignature.Information += "Signature has generated element at address " + function.UniqueAddressHex;

                function = null;
            }
            else if (foundSignature.Table != null)
            {
                Table table = new Table();
                table.BankNum = ope.ReadDataBankNum;
                table.AddressInt = address;
                if (table.BankNum == Num) table.AddressBinInt = AddressBinInt + table.AddressInt;
                else if (table.BankNum == Calibration.BankNum) table.AddressBinInt = Calibration.BankAddressBinInt + table.AddressInt;
                else table.AddressBinInt = -1; // Will Be Managed in processOtherElements or Calibration.readCalibrationElements

                table.ShortLabel = foundSignature.Table.ShortLabel;
                table.Label = foundSignature.Table.Label;

                table.S6xElementSignatureSource = foundSignature;

                table.WordOutput = foundSignature.Table.WordOutput;
                table.SignedOutput = foundSignature.Table.SignedOutput;

                try
                {
                    table.ColsNumber = Convert.ToInt32(foundSignature.Table.VariableColsNumber);
                }
                catch { }

                isCalElement = (table.AddressBinInt >= Calibration.AddressBinInt && table.AddressBinInt <= Calibration.AddressBinEndInt);
                if (isCalElement)
                // Calibration Element
                {
                    if (Calibration.slCalibrationElements.ContainsKey(table.UniqueAddress))
                    // Calibration Element already created, reference is assigned to Ope
                    {
                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                        opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[table.UniqueAddress];
                        ope.alCalibrationElems.Add(opeCalElem);
                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                        {
                            opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                        }
                        opeCalElem = null;
                    }
                    else
                    // Calibration Element creation
                    {
                        // RBase Calculation
                        foreach (RBase rBase in Calibration.slRbases.Values)
                        {
                            if (table.AddressBinInt >= rBase.AddressBinInt && table.AddressBinInt <= rBase.AddressBinEndInt)
                            {
                                table.RBase = rBase.Code;
                                table.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code.ToLower() + SADDef.AdditionSeparator + Convert.ToString(table.AddressBinInt - rBase.AddressBinInt, 16);
                                break;
                            }
                        }
                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                        opeCalElem = new CalibrationElement(table.BankNum, table.RBase);
                        opeCalElem.AddressBinInt = table.AddressBinInt;
                        opeCalElem.AddressInt = table.AddressInt;
                        opeCalElem.RBaseCalc = table.RBaseCalc;
                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                        {
                            opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                        }

                        // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                        if (!S6x.isS6xProcessTypeConflict(table.UniqueAddress, typeof(Table)))
                        {
                            opeCalElem.TableElem = table;
                        }

                        ope.alCalibrationElems.Add(opeCalElem);
                        Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                        opeCalElem = null;
                    }

                    if (!alCalibElemOPsUniqueAddresses.Contains(ope.UniqueAddress)) alCalibElemOPsUniqueAddresses.Add(ope.UniqueAddress);

                    ope.OtherElemAddress = string.Empty;
                    ope.KnownElemAddress = string.Empty;
                }
                else
                // Other Element
                {
                    // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                    if (!S6x.isS6xProcessTypeConflict(table.UniqueAddress, typeof(Table)))
                    {
                        table.RBaseCalc = ope.OtherElemAddress;

                        ope.KnownElemAddress = string.Empty;

                        if (Calibration.slExtTables.ContainsKey(table.UniqueAddress))
                        {
                            if (!((Table)Calibration.slExtTables[table.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                            {
                                ((Table)Calibration.slExtTables[table.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            }
                        }
                        else if (Calibration.slExtStructures.ContainsKey(table.UniqueAddress))
                        {
                            if (!((Structure)Calibration.slExtStructures[table.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                            {
                                ((Structure)Calibration.slExtStructures[table.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            }
                        }
                        else
                        {
                            if (!table.OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) table.OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            Calibration.slExtTables.Add(table.UniqueAddress, table);
                        }
                    }
                }

                if (foundSignature.Information == null) foundSignature.Information = string.Empty;
                if (foundSignature.Information != string.Empty) foundSignature.Information += "\r\n";
                foundSignature.Information += "Signature has generated element at address " + table.UniqueAddressHex;

                table = null;
            }
            else if (foundSignature.Structure != null)
            {
                Structure structure = new Structure();
                structure.BankNum = ope.ReadDataBankNum;
                structure.AddressInt = address;
                if (structure.BankNum == Num) structure.AddressBinInt = AddressBinInt + structure.AddressInt;
                else if (structure.BankNum == Calibration.BankNum) structure.AddressBinInt = Calibration.BankAddressBinInt + structure.AddressInt;
                else structure.AddressBinInt = -1; // Will Be Managed in processOtherElements or Calibration.readCalibrationElements

                structure.ShortLabel = foundSignature.Structure.ShortLabel;
                structure.Label = foundSignature.Structure.Label;

                structure.S6xElementSignatureSource = foundSignature;

                structure.StructDefString = foundSignature.Structure.StructDef;
                structure.Number = foundSignature.Structure.Number;

                isCalElement = (structure.AddressBinInt >= Calibration.AddressBinInt && structure.AddressBinInt <= Calibration.AddressBinEndInt);
                if (isCalElement)
                // Calibration Element
                {
                    if (Calibration.slCalibrationElements.ContainsKey(structure.UniqueAddress))
                    // Calibration Element already created, reference is assigned to Ope
                    {
                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                        opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[structure.UniqueAddress];
                        ope.alCalibrationElems.Add(opeCalElem);
                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                        {
                            opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                        }
                        opeCalElem = null;
                    }
                    else
                    // Calibration Element creation
                    {
                        // RBase Calculation
                        foreach (RBase rBase in Calibration.slRbases.Values)
                        {
                            if (structure.AddressBinInt >= rBase.AddressBinInt && structure.AddressBinInt <= rBase.AddressBinEndInt)
                            {
                                structure.RBase = rBase.Code;
                                structure.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code.ToLower() + SADDef.AdditionSeparator + Convert.ToString(structure.AddressBinInt - rBase.AddressBinInt, 16);
                                break;
                            }
                        }
                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                        opeCalElem = new CalibrationElement(structure.BankNum, structure.RBase);
                        opeCalElem.AddressBinInt = structure.AddressBinInt;
                        opeCalElem.AddressInt = structure.AddressInt;
                        opeCalElem.RBaseCalc = structure.RBaseCalc;
                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                        {
                            opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                        }

                        // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                        if (!S6x.isS6xProcessTypeConflict(structure.UniqueAddress, typeof(Structure)))
                        {
                            opeCalElem.StructureElem = structure;
                        }

                        ope.alCalibrationElems.Add(opeCalElem);
                        Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                        opeCalElem = null;
                    }

                    if (!alCalibElemOPsUniqueAddresses.Contains(ope.UniqueAddress)) alCalibElemOPsUniqueAddresses.Add(ope.UniqueAddress);

                    ope.OtherElemAddress = string.Empty;
                    ope.KnownElemAddress = string.Empty;
                }
                else
                // Other Element
                {
                    // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                    if (!S6x.isS6xProcessTypeConflict(structure.UniqueAddress, typeof(Structure)))
                    {
                        structure.RBaseCalc = ope.OtherElemAddress;

                        ope.KnownElemAddress = string.Empty;

                        if (Calibration.slExtScalars.ContainsKey(structure.UniqueAddress))
                        {
                            if (!((Scalar)Calibration.slExtScalars[structure.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                            {
                                ((Scalar)Calibration.slExtScalars[structure.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            }
                        }
                        else if (Calibration.slExtStructures.ContainsKey(structure.UniqueAddress))
                        {
                            if (!((Structure)Calibration.slExtStructures[structure.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                            {
                                ((Structure)Calibration.slExtStructures[structure.UniqueAddress]).OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            }
                        }
                        else
                        {
                            if (!structure.OtherRelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) structure.OtherRelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                            Calibration.slExtStructures.Add(structure.UniqueAddress, structure);
                        }
                    }
                }

                if (foundSignature.Information == null) foundSignature.Information = string.Empty;
                if (foundSignature.Information != string.Empty) foundSignature.Information += "\r\n";
                foundSignature.Information += "Signature has generated element at address " + structure.UniqueAddressHex;

                structure = null;
            }
            else
            {
                return;
            }
        }

        public void processOtherElements(ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            int iCount = 0;

            foreach (S6xTable s6xObject in S6x.slProcessTables.Values)
            {
                if (s6xObject.BankNum == Num && !s6xObject.isCalibrationElement)
                {
                    if (Calibration.slExtTables.ContainsKey(s6xObject.UniqueAddress)) Calibration.slExtTables[s6xObject.UniqueAddress] = new Table(s6xObject);
                    else Calibration.slExtTables.Add(s6xObject.UniqueAddress, new Table(s6xObject));
                }
            }
            foreach (S6xFunction s6xObject in S6x.slProcessFunctions.Values)
            {
                if (s6xObject.BankNum == Num && !s6xObject.isCalibrationElement)
                {
                    if (Calibration.slExtFunctions.ContainsKey(s6xObject.UniqueAddress)) Calibration.slExtFunctions[s6xObject.UniqueAddress] = new Function(s6xObject);
                    else Calibration.slExtFunctions.Add(s6xObject.UniqueAddress, new Function(s6xObject));
                }
            }
            foreach (S6xScalar s6xObject in S6x.slProcessScalars.Values)
            {
                if (s6xObject.BankNum == Num && !s6xObject.isCalibrationElement)
                {
                    if (Calibration.slExtScalars.ContainsKey(s6xObject.UniqueAddress)) Calibration.slExtScalars[s6xObject.UniqueAddress] = new Scalar(s6xObject);
                    else Calibration.slExtScalars.Add(s6xObject.UniqueAddress, new Scalar(s6xObject));
                }
            }

            foreach (S6xStructure s6xObject in S6x.slProcessStructures.Values)
            {
                if (s6xObject.BankNum == Num && !s6xObject.isCalibrationElement)
                {
                    if (Calibration.slExtStructures.ContainsKey(s6xObject.UniqueAddress)) Calibration.slExtStructures[s6xObject.UniqueAddress] =  new Structure(s6xObject);
                    else Calibration.slExtStructures.Add(s6xObject.UniqueAddress, new Structure(s6xObject));
                }
            }

            iCount = 0;
            foreach (Table extObject in Calibration.slExtTables.Values)
            {
                // Only on current bank
                if (extObject.BankNum != Num) continue;
                
                // Manages non calculated AddressBinInt
                if (extObject.AddressBinInt == -1) extObject.AddressBinInt = extObject.AddressInt + AddressBinInt;

                string[] arrElemBytes = null;

                if (extObject.S6xTable == null)
                {
                    // Table Discovery and Table Read
                    //  TO BE MANAGED IF REQUIRED

                    // 20181112 - From Element Signature
                    if (extObject.ColsNumber > 0 && extObject.S6xElementSignatureSource != null)
                    {
                        if (extObject.S6xElementSignatureSource.Table != null)
                        {
                            if (extObject.S6xElementSignatureSource.Table.RowsNumber > 0)
                            {
                                arrElemBytes = getBytesArray(extObject.AddressInt, extObject.SizeLine * extObject.S6xElementSignatureSource.Table.RowsNumber);
                            }
                        }
                    }
                }
                else if (extObject.S6xTable.ColsNumber > 0 && extObject.S6xTable.RowsNumber > 0)
                {
                    extObject.ColsNumber = extObject.S6xTable.ColsNumber;
                    arrElemBytes = getBytesArray(extObject.AddressInt, extObject.SizeLine * extObject.S6xTable.RowsNumber);
                }

                if (arrElemBytes != null)
                {
                    extObject.Read(arrElemBytes);
                    arrElemBytes = null;
                }

                // Labelling

                string otherAddressLabel = string.Empty;

                // Searching in Other Addresses first
                S6xOtherAddress s6xOther = (S6xOtherAddress)S6x.slProcessOtherAddresses[extObject.UniqueAddress];
                if (s6xOther != null)
                {
                    if (s6xOther.OutputLabel && !s6xOther.hasDefaultLabel) otherAddressLabel = s6xOther.Label;

                    // S6x CleanUp
                    S6x.slProcessOtherAddresses.Remove(s6xOther.UniqueAddress);
                    // 20200308 - PYM - It stays in S6x definition, other addresses are no more in conflict on addresses
                    //S6x.slOtherAddresses.Remove(s6xOther.UniqueAddress);

                    s6xOther = null;
                }

                if (extObject.S6xTable == null)
                {
                    if (otherAddressLabel != string.Empty)
                    {
                        extObject.Label = otherAddressLabel;
                        extObject.ShortLabel = otherAddressLabel;
                    }
                    else
                    {
                        // Default Translation
                        if (extObject.Label == string.Empty)
                        {
                            if (S6x.Properties.NoNumbering)
                            {
                                extObject.Label = SADDef.LongExtTablePrefix + extObject.UniqueAddressHex.Replace(" ", SADDef.NamingLongBankSeparator);
                                extObject.ShortLabel = SADDef.ShortExtTablePrefix + SADDef.NamingShortBankSeparator + extObject.UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator);
                            }
                            else
                            {
                                iCount++;
                                extObject.Label = SADDef.LongExtTablePrefix + extObject.BankNum.ToString() + SADDef.NamingLongBankSeparator + string.Format("{0:d3}", iCount);
                                extObject.ShortLabel = SADDef.ShortExtTablePrefix + extObject.BankNum.ToString() + SADDef.NamingShortBankSeparator + string.Format("{0:d3}", iCount);
                            }
                        }
                    }
                }
            }

            iCount = 0;
            foreach (Function extObject in Calibration.slExtFunctions.Values)
            {
                // Only on current bank
                if (extObject.BankNum != Num) continue;
                
                // Manages non calculated AddressBinInt
                if (extObject.AddressBinInt == -1) extObject.AddressBinInt = extObject.AddressInt + AddressBinInt;

                string[] arrElemBytes = null;

                if (extObject.S6xFunction == null)
                {
                    // Function Discovery and Function Read
                    //  TO BE MANAGED IF REQUIRED

                    // 20181112 - From Element Signature
                    if (extObject.S6xElementSignatureSource != null)
                    {
                        if (extObject.S6xElementSignatureSource.Function != null)
                        {
                            if (extObject.S6xElementSignatureSource.Function.RowsNumber > 0)
                            {
                                arrElemBytes = getBytesArray(extObject.AddressInt, extObject.SizeLine * extObject.S6xElementSignatureSource.Function.RowsNumber);
                            }
                        }
                    }
                }
                else if (extObject.S6xFunction.RowsNumber > 0)
                {
                    arrElemBytes = getBytesArray(extObject.AddressInt, extObject.SizeLine * extObject.S6xFunction.RowsNumber);
                }

                if (arrElemBytes != null)
                {
                    extObject.Read(arrElemBytes);
                    arrElemBytes = null;
                }

                // Labelling

                string otherAddressLabel = string.Empty;

                // Searching in Other Addresses first
                S6xOtherAddress s6xOther = (S6xOtherAddress)S6x.slProcessOtherAddresses[extObject.UniqueAddress];
                if (s6xOther != null)
                {
                    if (s6xOther.OutputLabel && !s6xOther.hasDefaultLabel) otherAddressLabel = s6xOther.Label;

                    // S6x CleanUp
                    S6x.slProcessOtherAddresses.Remove(s6xOther.UniqueAddress);
                    // 20200308 - PYM - It stays in S6x definition, other addresses are no more in conflict on addresses
                    //S6x.slOtherAddresses.Remove(s6xOther.UniqueAddress);

                    s6xOther = null;
                } 
                
                if (extObject.S6xFunction == null)
                {
                    if (otherAddressLabel != string.Empty)
                    {
                        extObject.Label = otherAddressLabel;
                        extObject.ShortLabel = otherAddressLabel;
                    }
                    else
                    {
                        // Default Translation
                        if (extObject.Label == string.Empty)
                        {
                            if (S6x.Properties.NoNumbering)
                            {
                                extObject.Label = SADDef.LongExtFunctionPrefix + extObject.UniqueAddressHex.Replace(" ", SADDef.NamingLongBankSeparator);
                                extObject.ShortLabel = SADDef.ShortExtFunctionPrefix + SADDef.NamingShortBankSeparator + extObject.UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator);
                            }
                            else
                            {
                                iCount++;
                                extObject.Label = SADDef.LongExtFunctionPrefix + extObject.BankNum.ToString() + SADDef.NamingLongBankSeparator + string.Format("{0:d3}", iCount);
                                extObject.ShortLabel = SADDef.ShortExtFunctionPrefix + extObject.BankNum.ToString() + SADDef.NamingShortBankSeparator + string.Format("{0:d3}", iCount);
                            }
                        }
                    }
                }
            }
            iCount = 0;
            foreach (Scalar extObject in Calibration.slExtScalars.Values)
            {
                // Only on current bank
                if (extObject.BankNum != Num) continue;
                
                // Manages non calculated AddressBinInt
                if (extObject.AddressBinInt == -1) extObject.AddressBinInt = extObject.AddressInt + AddressBinInt;

                string[] arrElemBytes = getBytesArray(extObject.AddressInt, extObject.Size);
                extObject.Read(arrElemBytes);
                arrElemBytes = null;
                
                // Labelling
                
                string otherAddressLabel = string.Empty;

                // Searching in Other Addresses first
                S6xOtherAddress s6xOther = (S6xOtherAddress)S6x.slProcessOtherAddresses[extObject.UniqueAddress];
                if (s6xOther != null)
                {
                    if (s6xOther.OutputLabel && !s6xOther.hasDefaultLabel) otherAddressLabel = s6xOther.Label;

                    // S6x CleanUp
                    S6x.slProcessOtherAddresses.Remove(s6xOther.UniqueAddress);
                    // 20200308 - PYM - It stays in S6x definition, other addresses are no more in conflict on addresses
                    //S6x.slOtherAddresses.Remove(s6xOther.UniqueAddress);

                    s6xOther = null;
                } 
                
                if (extObject.S6xScalar == null)
                {
                    if (otherAddressLabel != string.Empty)
                    {
                        extObject.Label = otherAddressLabel;
                        extObject.ShortLabel = otherAddressLabel;
                    }
                    else
                    {
                        // Default Translation
                        if (extObject.Label == string.Empty)
                        {
                            if (S6x.Properties.NoNumbering)
                            {
                                extObject.ShortLabel = SADDef.ShortExtScalarPrefix + SADDef.NamingShortBankSeparator + extObject.UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator);
                            }
                            else
                            {
                                iCount++;
                                extObject.ShortLabel = SADDef.ShortExtScalarPrefix + extObject.BankNum.ToString() + SADDef.NamingShortBankSeparator + string.Format("{0:d3}", iCount);
                            }
                        }
                    }
                }
            }

            // Structures Analysis for Autodetected and Defaulted Structures
            //  Requires Multiples Steps - A Dedicated function is used
            processOtherElementStructuresAnalysis(ref Bank0, ref Bank1, ref Bank8, ref Bank9);
           
            // Structure Reading / Labelling / Included Management
            iCount = 0;
            foreach (Structure sStruct in Calibration.slExtStructures.Values)
            {
                // Only on current bank
                if (sStruct.BankNum != Num) continue;

                // Manages non calculated AddressBinInt
                if (sStruct.AddressBinInt == -1) sStruct.AddressBinInt = sStruct.AddressInt + AddressBinInt;

                if (sStruct.S6xStructure != null)
                {
                    sStruct.Number = sStruct.S6xStructure.Number;   
                    sStruct.StructDefString = sStruct.S6xStructure.StructDef;
                }

                if (sStruct.isValid && !sStruct.isEmpty && sStruct.Number > 0 && sStruct.ParentStructure == null)
                {
                    string[] arrStructBytes = getBytesArray(sStruct.AddressInt, sStruct.MaxSizeSingle * sStruct.Number);
                    sStruct.Read(ref arrStructBytes, sStruct.Number);
                    arrStructBytes = null;
                }

                // Included structures
                if (sStruct.ParentStructure == null)
                {
                    int sIndex = Calibration.slExtStructures.IndexOfKey(sStruct.UniqueAddress);
                    if (sIndex > 0)
                    {
                        Structure prevStruct = (Structure)Calibration.slExtStructures.GetByIndex(sIndex - 1);
                        if (prevStruct.BankNum == sStruct.BankNum && prevStruct.AddressEndInt > sStruct.AddressInt)
                        {
                            // Same Definition
                            if (prevStruct.StructDefString == sStruct.StructDefString)
                            {
                                sStruct.ParentStructure = (prevStruct.ParentStructure == null) ? prevStruct : prevStruct.ParentStructure;
                                continue;
                            }

                            // Previous structure is a user defined one or stored
                            if (prevStruct.S6xStructure != null)
                            {
                                if (prevStruct.Number > 0)
                                {
                                    if (prevStruct.S6xStructure.Store || prevStruct.S6xStructure.isUserDefined)
                                    {
                                        sStruct.ParentStructure = (prevStruct.ParentStructure == null) ? prevStruct : prevStruct.ParentStructure;
                                        continue;
                                    }
                                }
                            }

                            while (prevStruct.AddressEndInt >= sStruct.AddressInt && prevStruct.Number > 0)
                            {
                                prevStruct.Number--;
                                string[] arrPrevStructBytes = getBytesArray(prevStruct.AddressInt, prevStruct.MaxSizeSingle * prevStruct.Number);
                                prevStruct.Read(ref arrPrevStructBytes, prevStruct.Number);
                                arrPrevStructBytes = null;
                            }
                        }
                    }
                }

                // Labelling
                string otherAddressLabel = string.Empty;

                // Searching in Other Addresses first
                S6xOtherAddress s6xOther = (S6xOtherAddress)S6x.slProcessOtherAddresses[sStruct.UniqueAddress];
                if (s6xOther != null)
                {
                    if (s6xOther.OutputLabel && !s6xOther.hasDefaultLabel) otherAddressLabel = s6xOther.Label;

                    // S6x CleanUp
                    S6x.slProcessOtherAddresses.Remove(s6xOther.UniqueAddress);
                    // 20200308 - PYM - It stays in S6x definition, other addresses are no more in conflict on addresses
                    //S6x.slOtherAddresses.Remove(s6xOther.UniqueAddress);

                    s6xOther = null;
                } 
                
                if (otherAddressLabel != string.Empty)
                {
                    sStruct.Label = otherAddressLabel;
                    sStruct.ShortLabel = otherAddressLabel;
                }
                else
                {
                    // Default Translation
                    if (sStruct.Label == string.Empty)
                    {
                        if (S6x.Properties.NoNumbering)
                        {
                            if (sStruct.isVectorsList && sStruct.Vectors.Count > 0)
                            {
                                sStruct.Label = SADDef.LongVectListPrefix + sStruct.UniqueAddressHex.Replace(" ", SADDef.NamingLongBankSeparator);
                                sStruct.ShortLabel = SADDef.ShortVectListPrefix + SADDef.NamingShortBankSeparator + sStruct.UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator);
                            }
                            else if (sStruct.isVectorsStructure)
                            {
                                sStruct.Label = SADDef.LongVectStructPrefix + sStruct.UniqueAddressHex.Replace(" ", SADDef.NamingLongBankSeparator);
                                sStruct.ShortLabel = SADDef.ShortVectStructPrefix + SADDef.NamingShortBankSeparator + sStruct.UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator);
                            }
                            else
                            {
                                sStruct.Label = SADDef.LongExtStructurePrefix + sStruct.UniqueAddressHex.Replace(" ", SADDef.NamingLongBankSeparator);
                                sStruct.ShortLabel = SADDef.ShortExtStructurePrefix + SADDef.NamingShortBankSeparator + sStruct.UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator);
                            }
                        }
                        else
                        {
                            iCount++;
                            if (sStruct.isVectorsList && sStruct.Vectors.Count > 0)
                            {
                                sStruct.Label = SADDef.LongVectListPrefix + sStruct.BankNum.ToString() + SADDef.NamingLongBankSeparator + string.Format("{0:d3}", iCount);
                                sStruct.ShortLabel = SADDef.ShortVectListPrefix + sStruct.BankNum.ToString() + SADDef.NamingShortBankSeparator + string.Format("{0:d3}", iCount);
                            }
                            else if (sStruct.isVectorsStructure)
                            {
                                sStruct.Label = SADDef.LongVectStructPrefix + sStruct.BankNum.ToString() + SADDef.NamingLongBankSeparator + string.Format("{0:d3}", iCount);
                                sStruct.ShortLabel = SADDef.ShortVectStructPrefix + sStruct.BankNum.ToString() + SADDef.NamingShortBankSeparator + string.Format("{0:d3}", iCount);
                            }
                            else
                            {
                                sStruct.Label = SADDef.LongExtStructurePrefix + sStruct.BankNum.ToString() + SADDef.NamingLongBankSeparator + string.Format("{0:d3}", iCount);
                                sStruct.ShortLabel = SADDef.ShortExtStructurePrefix + sStruct.BankNum.ToString() + SADDef.NamingShortBankSeparator + string.Format("{0:d3}", iCount);
                            }
                        }
                    }
                }
            }
        }

        // Structures Analysis for Autodetected and Defaulted Structures
        //  Requires Multiples Steps - A Dedicated function is used
        public void processOtherElementStructuresAnalysis(ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9)
        {
            // First Step - Create an Analysis for each structure / each operation using this structure
            foreach (Structure sStruct in Calibration.slExtStructures.Values)
            {
                // Only on current bank
                if (sStruct.BankNum != Num) continue;

                // Only on auto detected structures
                if (sStruct.S6xStructure != null || !sStruct.Defaulted) continue;

                foreach (string opeUniqueAddress in sStruct.OtherRelatedOpsUniqueAddresses)
                {
                    // Structure Discovery
                    Operation rOpe = null;
                    if (Bank8 != null && rOpe == null) rOpe = (Operation)Bank8.slOPs[opeUniqueAddress];
                    if (Bank1 != null && rOpe == null) rOpe = (Operation)Bank1.slOPs[opeUniqueAddress];
                    if (Bank9 != null && rOpe == null) rOpe = (Operation)Bank9.slOPs[opeUniqueAddress];
                    if (Bank0 != null && rOpe == null) rOpe = (Operation)Bank0.slOPs[opeUniqueAddress];
                    if (rOpe == null) continue;

                    switch (rOpe.BankNum)
                    {
                        case 8:
                            Bank8.processStructureAnalysis(sStruct.UniqueAddress, rOpe.UniqueAddress, false, ref Bank0, ref Bank1, ref Bank8, ref Bank9);
                            break;
                        case 1:
                            Bank1.processStructureAnalysis(sStruct.UniqueAddress, rOpe.UniqueAddress, false, ref Bank0, ref Bank1, ref Bank8, ref Bank9);
                            break;
                        case 9:
                            Bank9.processStructureAnalysis(sStruct.UniqueAddress, rOpe.UniqueAddress, false, ref Bank0, ref Bank1, ref Bank8, ref Bank9);
                            break;
                        case 0:
                            Bank0.processStructureAnalysis(sStruct.UniqueAddress, rOpe.UniqueAddress, false, ref Bank0, ref Bank1, ref Bank8, ref Bank9);
                            break;
                    }
                    // Same structure can be processed more than one time to get better results
                }
            }

            // Structure Analysis Mngt and Duplicated mngt based on Corrected Address
            //  Done when all structures have been deeply processed
            ArrayList alMovedStructuresUniqueAddresses = new ArrayList();
            foreach (Structure sStruct in Calibration.slExtStructures.Values)
            {
                // Only on current bank
                if (sStruct.BankNum != Num) continue;

                // Only on auto detected structures
                if (sStruct.S6xStructure != null) continue;

                // Resulted Analysis Management including Address Correction
                StructureAnalysis saSA = (StructureAnalysis)Calibration.slStructuresAnalysis[sStruct.UniqueAddress];
                if (saSA != null)
                {
                    if (saSA.isAdderStructure)
                    // Adder Structure is basic but requires Number Validation by reading it until another item
                    {
                        if (sStruct.ParentStructure == null)
                        // If not already defined as a Child from another identical Parent Structure
                        {
                            if (saSA.Number > 0)
                            {
                                sStruct.Number = saSA.Number;
                            }
                            else
                            {
                                // Reset at One, like it is originally defaulted, because can be processed many times
                                sStruct.Number = 1;

                                if (saSA.StructNewAddressInt == -1)
                                {
                                    // Adder check on 0 value
                                    if (isOtherElemAddressInConflict(saSA.StructAddressInt, saSA.StructAddressInt, typeof(Structure)))
                                    {
                                        saSA.StructNewAddressInt = saSA.StructAddressInt + sStruct.MaxSizeSingle;
                                        if (isOtherElemAddressInConflict(saSA.StructNewAddressInt, saSA.StructAddressInt, typeof(Structure)))
                                        {
                                            saSA.StructNewAddressInt = -1;
                                            continue;
                                        }
                                    }
                                }

                                Structure stAdd = new Structure();
                                stAdd.BankNum = sStruct.BankNum;
                                stAdd.AddressInt = sStruct.AddressInt;
                                if (saSA.StructNewAddressInt > saSA.StructAddressInt) stAdd.AddressInt = saSA.StructNewAddressInt;
                                stAdd.StructDefString = sStruct.StructDefString;
                                stAdd.Number = 1;
                                while (stAdd.AddressInt + stAdd.MaxSizeSingle * stAdd.Number < arrBytes.Length)
                                {
                                    // Security Limit - 16 for this type
                                    if (sStruct.Number >= 16) break;

                                    if (isOtherElemAddressInConflict(stAdd.AddressInt, sStruct.AddressInt, typeof(Structure))) break;

                                    string[] arrStAddBytes = getBytesArray(stAdd.AddressInt, stAdd.MaxSizeSingle * stAdd.Number);

                                    if (arrStAddBytes.Length == 0) break;

                                    if (stAdd.AddressInt != sStruct.AddressInt) sStruct.Number++;

                                    stAdd.Read(ref arrStAddBytes, stAdd.Number);
                                    stAdd.AddressInt += stAdd.Size;
                                }
                                stAdd = null;
                            }
                            if (sStruct.Number > 1) sStruct.Defaulted = false;
                            if (saSA.StructNewAddressInt != -1 && saSA.StructNewAddressInt != saSA.StructAddressInt) sStruct.ParentAddressInt = saSA.StructNewAddressInt;
                        }
                    }
                    else
                    // Fully Analysed Structure
                    {
                        if (saSA.isNumberCalculationRequired)
                        {
                            // Structure Definition Based Number 
                            if (saSA.LineSize == -1)
                            {
                                Structure stSt = new Structure();
                                stSt.BankNum = saSA.StructBankNum;
                                stSt.AddressInt = saSA.StructAddressInt;
                                if (saSA.StructNewAddressInt > saSA.StructAddressInt) stSt.AddressInt = saSA.StructNewAddressInt;
                                stSt.StructDefString = saSA.ProposedStructureDefString;
                                stSt.Number = 1;
                                if (!stSt.isEmpty && stSt.isValid && !stSt.containsConditions) saSA.LineSize = stSt.MaxSizeSingle;
                                stSt = null;
                            }

                            // Classical Number Calculation
                            saSA.NumberCalculation();
                        }
                        else if (saSA.isStructureReadRequired)
                        // Based on StructureAnalysis.LoopExitFirstItemValue
                        {
                            saSA.Number = 0;
                            int loopExitFirstItemValue = saSA.LoopExitFirstItemValue;
                            StructureItemAnalysis siaSIA = (StructureItemAnalysis)saSA.slItems[0];
                            if (siaSIA != null)
                            {
                                Structure stTmp = new Structure();
                                stTmp.BankNum = saSA.StructBankNum;
                                stTmp.AddressInt = saSA.StructAddressInt;
                                if (saSA.StructNewAddressInt > saSA.StructAddressInt) stTmp.AddressInt = saSA.StructNewAddressInt;
                                stTmp.StructDefString = saSA.ProposedStructureDefString;
                                stTmp.Number = 1;
                                while (stTmp.AddressInt + stTmp.MaxSizeSingle * stTmp.Number < arrBytes.Length)
                                {
                                    // Security Limit
                                    if (saSA.Number > 512)
                                    {
                                        saSA.Number = -1;
                                        break;
                                    }
                                    if (slOPs.ContainsKey(stTmp.UniqueAddress))
                                    {
                                        saSA.Number = -1;
                                        break;
                                    }
                                    if (isJumpAddressInConflict(stTmp.AddressInt, -1))
                                    {
                                        saSA.Number = -1;
                                        break;
                                    }
                                    string[] arrStTmpBytes = getBytesArray(stTmp.AddressInt, stTmp.MaxSizeSingle * stTmp.Number);
                                    if (arrStTmpBytes.Length == 0)
                                    {
                                        saSA.Number = -1;
                                        break;
                                    }
                                    saSA.Number++;
                                    if (siaSIA.isByte && arrStTmpBytes.Length > 0)
                                    {
                                        if (Convert.ToInt32(arrStTmpBytes[0], 16) == saSA.LoopExitFirstItemValue) break;
                                    }
                                    else if (arrStTmpBytes.Length > 1)
                                    {
                                        if (Convert.ToInt32(arrStTmpBytes[1] + arrStTmpBytes[0], 16) == saSA.LoopExitFirstItemValue) break;
                                    }
                                    stTmp.Read(ref arrStTmpBytes, stTmp.Number);
                                    stTmp.AddressInt += stTmp.Size;
                                }
                                stTmp = null;
                            }
                            if (saSA.Number <= 0)
                            {
                                // Detection Error - Reset of related values
                                saSA.Number = -1;
                                saSA.LoopExitFirstItemValue = int.MinValue;
                                saSA.LoopExitFirstItemOpeAddressInt = -1;
                            }
                        }

                        if (saSA.isValid)
                        {
                            sStruct.Defaulted = false;
                            sStruct.Number = saSA.Number;
                            sStruct.StructDefString = saSA.ProposedStructureDefString;
                            if (saSA.StructNewAddressInt != -1 && saSA.StructNewAddressInt != saSA.StructAddressInt) sStruct.ParentAddressInt = saSA.StructNewAddressInt;
                        }
                    }

                    saSA = null;
                }

                if (sStruct.ParentAddressInt == -1) continue;

                // Corrected structures
                sStruct.ParentStructure = (Structure)Calibration.slExtStructures[sStruct.ParentUniqueAddress];
                if (sStruct.ParentStructure == null) alMovedStructuresUniqueAddresses.Add(sStruct.UniqueAddress);
            }

            // Creating Structures based on moved ones
            foreach (string uniqueAddress in alMovedStructuresUniqueAddresses)
            {
                Structure sStruct = (Structure)Calibration.slExtStructures[uniqueAddress];
                if (sStruct.ParentStructure != null) continue;

                sStruct.ParentStructure = (Structure)Calibration.slExtStructures[sStruct.ParentUniqueAddress];
                if (sStruct.ParentStructure == null)
                {
                    bool bConflict = false;
                    if (!bConflict) if (Calibration.slExtTables.ContainsKey(sStruct.ParentUniqueAddress)) bConflict = true;
                    if (!bConflict) if (Calibration.slExtFunctions.ContainsKey(sStruct.ParentUniqueAddress)) bConflict = true;
                    if (!bConflict) if (Calibration.slExtScalars.ContainsKey(sStruct.ParentUniqueAddress)) bConflict = true;
                    if (!bConflict) if (isOtherElemAddressInsideExtScalar(sStruct.ParentAddressInt, sStruct.ParentAddressInt, typeof(Structure))) bConflict = true;
                    if (!bConflict)
                    {
                        switch (sStruct.BankNum)
                        {
                            case 8:
                                if (Bank8 != null)
                                {
                                    if (Bank8.slOPs.ContainsKey(sStruct.ParentUniqueAddress)) bConflict = true;
                                    if (!bConflict) if (Bank8.isOtherElemAddressInsideOperation(sStruct.ParentAddressInt)) bConflict = true;
                                }
                                break;
                            case 1:
                                if (Bank1 != null)
                                {
                                    if (Bank1.slOPs.ContainsKey(sStruct.ParentUniqueAddress)) bConflict = true;
                                    if (!bConflict) if (Bank1.isOtherElemAddressInsideOperation(sStruct.ParentAddressInt)) bConflict = true;
                                }
                                break;
                            case 9:
                                if (Bank9 != null)
                                {
                                    if (Bank9.slOPs.ContainsKey(sStruct.ParentUniqueAddress)) bConflict = true;
                                    if (!bConflict) if (Bank9.isOtherElemAddressInsideOperation(sStruct.ParentAddressInt)) bConflict = true;
                                }
                                break;
                            case 0:
                                if (Bank0 != null)
                                {
                                    if (Bank0.slOPs.ContainsKey(sStruct.ParentUniqueAddress)) bConflict = true;
                                    if (!bConflict) if (Bank0.isOtherElemAddressInsideOperation(sStruct.ParentAddressInt)) bConflict = true;
                                }
                                break;
                        }
                    }

                    if (bConflict)
                    {
                        sStruct.ParentAddressInt = -1;
                        continue;
                    }

                    Structure newStructure = new Structure();
                    newStructure.BankNum = sStruct.BankNum;
                    newStructure.AddressInt = sStruct.ParentAddressInt;
                    newStructure.RBase = sStruct.RBase;
                    newStructure.RBaseCalc = sStruct.RBaseCalc;
                    newStructure.Defaulted = sStruct.Defaulted;
                    newStructure.Number = sStruct.Number;
                    newStructure.StructDefString = sStruct.StructDefString;

                    Calibration.slExtStructures.Add(newStructure.UniqueAddress, newStructure);
                    sStruct.ParentStructure = (Structure)Calibration.slExtStructures[sStruct.ParentUniqueAddress];
                    newStructure = null;
                }
                sStruct = null;
            }
            alMovedStructuresUniqueAddresses = null;
        }

        // Other Elements Single Structure Analysis
        public void processStructureAnalysis(string structUniqueAddress, string opeUniqueAddress, bool calibrationStructure, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9)
        {
            Structure sStruct = null;
            if (calibrationStructure)
            {
                CalibrationElement calElem = (CalibrationElement)Calibration.slCalibrationElements[structUniqueAddress];
                if (calElem != null) sStruct = calElem.StructureElem;
                calElem = null;
            }
            else sStruct = (Structure)Calibration.slExtStructures[structUniqueAddress];

            Operation srcOpe = (Operation)slOPs[opeUniqueAddress];

            StructureAnalysis saSA = null;

            if (sStruct == null || srcOpe == null)
            {
                sStruct = null;
                srcOpe = null;
                return;
            }

            saSA = new StructureAnalysis();

            object[] arrPointersValues = Tools.InstructionPointersValues(srcOpe.OperationParams[0].InstructedParam);
            if ((bool)arrPointersValues[0] && arrPointersValues.Length > 3)
            {
                // SPECIFIC MODE NOT MANAGED FOR NOW - Address Reg does not contain Address, but is an address adder
                // 93,55,92,47,c9       orrb  Rc9,[R54+4792]     Rc9 |= [R54+OSt8_006];
                // 
                saSA.MainRegister = arrPointersValues[1].ToString();
                saSA.MainRegisterTB = Convert.ToString((int)arrPointersValues[2] + 1, 16);
                saSA.isAdderStructure = true;

                saSA.StructBankNum = sStruct.BankNum;
                saSA.StructAddressInt = sStruct.AddressInt;

                // Adder Check Analysis to find basic adder value (when not 0)
                processStructureAnalysisAdderCheck(ref saSA, ref srcOpe, ref Bank0, ref Bank1, ref Bank8, ref Bank9);

                // No Other Analysis is required - Structure will be processed automatically based on current operation

                if (!Calibration.slStructuresAnalysis.ContainsKey(saSA.StructUniqueAddress)) Calibration.slStructuresAnalysis.Add(saSA.StructUniqueAddress, saSA);

                return;
            }
            else
            {
                arrPointersValues = Tools.InstructionPointersValues(srcOpe.OperationParams[srcOpe.OperationParams.Length - 1].InstructedParam);
                if ((bool)arrPointersValues[0])
                {
                    saSA.MainRegister = arrPointersValues[1].ToString();
                    saSA.MainRegisterTB = Convert.ToString((int)arrPointersValues[2] + 1, 16);
                }
            }
            arrPointersValues = null;

            if (saSA.MainRegister == string.Empty) return;

            int mainRegPosition = 0;
            bool loopProcessed = false;

            saSA.StructBankNum = sStruct.BankNum;
            saSA.StructAddressInt = sStruct.AddressInt;
            saSA.AnalysisBankNum = Num;
            saSA.AnalysisSourceOpeAddressInt = srcOpe.AddressInt;
            saSA.AnalysisStartOpeAddressInt = srcOpe.AddressInt;
            saSA.AnalysisInitOpeAddressInt = srcOpe.AddressInt;

            Operation[] arrOps = null;

            // Source Ope is not the intialization of Address Register - Seaching for new Source
            if (srcOpe.OriginalInstruction.ToLower() != "ldw")
            {
                //arrOps = getPrecedingOPs(srcOpe.AddressInt, 8, 0, false, false, false, false, false, false, false, false);
                arrOps = getPrecedingOPs(srcOpe.AddressInt, 8, 0);
                foreach (Operation ope in arrOps)
                {
                    if (ope == null) break;

                    if (ope.OperationParams.Length == 0) continue;

                    if (saSA.LoopRegister == string.Empty)
                    {
                        if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam != Tools.RegisterInstruction(saSA.MainRegister)) continue;
                    }
                    else
                    {
                        if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam != Tools.RegisterInstruction(saSA.LoopRegister)) continue;
                    }
                    switch (ope.OriginalInstruction.ToLower())
                    {
                        case "clrb":
                        case "clrw":
                            if (saSA.LoopRegister != string.Empty)
                            {
                                saSA.AnalysisStartOpeAddressInt = ope.AddressInt;
                            }
                            break;
                        case "ldb":
                        case "ldw":
                        case "ldzbw":
                            if (saSA.LoopRegister != string.Empty)
                            {
                                switch (ope.OriginalOpArr[0].ToLower())
                                {
                                    case "ad":
                                    case "b1":
                                        saSA.LoopInitValue = Convert.ToInt32(ope.OriginalOpArr[1], 16);
                                        if (saSA.LoopInitValue > 0)
                                        {
                                            saSA.LoopReversed = true;
                                            saSA.Number = saSA.LoopInitValue + 1;
                                        }
                                        break;
                                }
                            }
                            saSA.AnalysisStartOpeAddressInt = ope.AddressInt;
                            break;
                        case "ml3b":
                            if (saSA.LoopRegister == string.Empty)
                            {
                                if (ope.OriginalOpArr[0].ToLower() == "5d")
                                {
                                    saSA.LoopRegister = ope.OriginalOpArr[2];
                                    saSA.LineSize = Convert.ToInt32(ope.OriginalOpArr[1], 16);
                                    saSA.AnalysisInitOpeAddressInt = ope.AddressInt;
                                }
                                else
                                {
                                    saSA.AnalysisStartOpeAddressInt = ope.AddressInt;
                                }
                            }
                            break;
                        default:
                            if (saSA.LoopRegister == string.Empty)
                            {
                                if (ope.OperationParams.Length > 2) saSA.AnalysisStartOpeAddressInt = ope.AddressInt;
                            }
                            break;
                    }
                    if (saSA.AnalysisStartOpeAddressInt != srcOpe.AddressInt) break;
                }
            }

            // Main Structure Analysis Part
            processStructureAnalysisFollowUse(ref saSA, ref mainRegPosition, ref loopProcessed);

            // Searching Missing Structure Parameters when Loop Address is known and when Loop Processed status has not been reached
            if (!loopProcessed && saSA.LoopOpeAddressInt > 0 && ((saSA.ExitAddress == string.Empty && saSA.LoopExitOpeAddressInt == -1) || saSA.LineSize == -1 || saSA.Number == -1))
            {
                arrOps = getFollowingOPs(saSA.LoopOpeAddressInt, 8, 0, true, false, false, false, false, false, false, false);
                foreach (Operation ope in arrOps)
                {
                    if (ope == null) break;
                    if (ope.isReturn) break;
                    if (ope.CallType == CallType.Jump) break;

                    if (ope.AddressInt == srcOpe.AddressInt) continue;
                    if (ope.AddressInt == saSA.AnalysisInitOpeAddressInt) continue;
                    if (ope.OperationParams.Length == 0) continue;

                    bool bBreak = false;

                    // CMPW on addresses - If exitAddress is greater than Struct address, it is its end address, else it is its start address
                    switch (ope.OriginalOPCode.ToLower())
                    {
                        case "8b":
                        case "89":
                            if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == Tools.RegisterInstruction(saSA.MainRegister))
                            {
                                if (ope.OperationParams[0].CalculatedParam != "ffff") // Specific case
                                {
                                    saSA.LoopExitOpeAddressInt = ope.AddressInt;
                                    saSA.ExitAddress = ope.OperationParams[0].CalculatedParam;
                                    bBreak = true;
                                }
                            }
                            break;
                    }

                    // Other Conditions based on first Structure Item value
                    if (saSA.slItems.ContainsKey(0))
                    {
                        foreach (Operation cOpe in ((StructureItemAnalysis)saSA.slItems[0]).ReadCondOperations)
                        {
                            if (cOpe.AddressInt != ope.AddressInt) continue;
                            if (!ope.OriginalOPCode.ToLower().StartsWith("d")) continue;
                            if (ope.GotoOpParams == null) continue;
                            
                            Operation cmpOpe = (Operation)slOPs[ope.GotoOpParams.OpeUniqueAddress];
                            if (cmpOpe == null) continue;
                            if (cmpOpe.OperationParams.Length == 0) continue;
                            foreach (OperationParam opeParam in cmpOpe.OperationParams)
                            {
                                string instructedParam = opeParam.InstructedParam;
                                arrPointersValues = Tools.InstructionPointersValues(instructedParam);
                                if ((bool)arrPointersValues[0]) continue; // Searching for a hard coded value

                                saSA.LoopExitFirstItemOpeAddressInt = cOpe.AddressInt;  // Goto OP not GotoOpParams original one, because it will permit to retreive it
                                switch (ope.OriginalInstruction.ToLower())
                                {
                                    case "jlt":
                                    case "jgt":
                                        if ((int)arrPointersValues[2] > 0xff) saSA.LoopExitFirstItemValue = (int)arrPointersValues[2];
                                        else saSA.LoopExitFirstItemValue = 0xff - (int)arrPointersValues[2];
                                        break;
                                    default:
                                        saSA.LoopExitFirstItemValue = (int)arrPointersValues[2];
                                        break;
                                }
                            }
                            break;
                        }
                    }

                    if (bBreak) break;
                }
                arrOps = null;
            }

            // Sharing same arrays when structure start addresses are different, for the same structure
            foreach (StructureAnalysis saSRC in Calibration.slStructuresAnalysis.Values)
            {
                if (saSA.StructBankNum != saSRC.StructBankNum) continue;
                if (saSA.StructAddressInt == saSRC.StructAddressInt || saSA.StructAddressInt == saSRC.StructNewAddressInt || saSA.StructNewAddressInt == saSRC.StructAddressInt || (saSA.StructNewAddressInt != -1 && saSA.StructNewAddressInt == saSRC.StructNewAddressInt))
                {
                    saSA.DefinitionStrings = saSRC.DefinitionStrings;
                    saSA.DefinitionConds = saSRC.DefinitionConds;
                    break;
                }
            }

            // No additional process is required, but Analysis is stored to share arrays when structure start addresses are different, for the same structure
            if (saSA.slItems.Count == 0)
            {
                if (!Calibration.slStructuresAnalysis.ContainsKey(saSA.StructUniqueAddress)) Calibration.slStructuresAnalysis.Add(saSA.StructUniqueAddress, saSA);
                return;
            }

            // Structure Items Analysis
            foreach (int itemPos in saSA.slItems.Keys)
            {
                StructureItemAnalysis siaSIA = (StructureItemAnalysis)saSA.slItems[itemPos];

                processStructureAnalysisFollowItem(ref saSA, ref siaSIA, true);
            }

            // Sharing same arrays reverse process when structure start addresses are different, for the same structure
            foreach (StructureAnalysis saSRC in Calibration.slStructuresAnalysis.Values)
            {
                if (saSA.StructBankNum != saSRC.StructBankNum) continue;
                if (saSA.StructAddressInt == saSRC.StructAddressInt || saSA.StructAddressInt == saSRC.StructNewAddressInt || saSA.StructNewAddressInt == saSRC.StructAddressInt || (saSA.StructNewAddressInt != -1 && saSA.StructNewAddressInt == saSRC.StructNewAddressInt))
                {
                    saSRC.DefinitionStrings = saSA.DefinitionStrings;
                    saSRC.DefinitionConds = saSA.DefinitionConds;

                    // All related elements - No Break
                }
            }

            if (Calibration.slStructuresAnalysis.ContainsKey(saSA.StructUniqueAddress))
            //  Existing Structure Analysis Upgrade
            {
                StructureAnalysis saOther = (StructureAnalysis)Calibration.slStructuresAnalysis[saSA.StructUniqueAddress];
                
                // If it was an Adder Structure it is not anymore
                saOther.isAdderStructure = false;

                // Already done for Definition Arrays and Items
                if (saSA.ExitAddress != string.Empty)
                {
                    if (saOther.ExitAddress == string.Empty) saOther.ExitAddress = saSA.ExitAddress;
                    else if (Convert.ToInt32(saSA.ExitAddress, 16) > Convert.ToInt32(saOther.ExitAddress, 16)) saOther.ExitAddress = saSA.ExitAddress;
                }
                if (saSA.LineSize > saOther.LineSize) saOther.LineSize = saSA.LineSize;
                if (saSA.LoopExitFirstItemValue != -1) saOther.LoopExitFirstItemValue = saSA.LoopExitFirstItemValue;
                if (saSA.Number > saOther.Number) saOther.Number = saSA.Number;
                if (saSA.StructNewAddressInt != -1 && saSA.StructNewAddressInt < saOther.StructNewAddressInt) saOther.StructNewAddressInt = saSA.StructNewAddressInt;
            }
            else
            //  New Structure Analysis
            {
                Calibration.slStructuresAnalysis.Add(saSA.StructUniqueAddress, saSA);
            }
        }

        // Main Adder Structure Analysis Part
        private void processStructureAnalysisAdderCheck(ref StructureAnalysis saSA, ref Operation srcOpe, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9)
        {
            string mainRegCpy = string.Empty;
            string mainRegCpyTB = string.Empty;

            int adderCurrentValue = 0;
            int adderInitialValue = 0;
            int adderStep = 0;

            bool adderInitialScalar = false;

            int adderForConflict = 0;

            mainRegCpy = saSA.MainRegister;
            mainRegCpyTB = saSA.MainRegisterTB;

            // Conflict Management on Ope for Initial Value
            SADBank structBank = null;
            switch (saSA.StructBankNum)
            {
                case 8:
                    structBank = Bank8;
                    break;
                case 1:
                    structBank = Bank1;
                    break;
                case 9:
                    structBank = Bank9;
                    break;
                case 0:
                    structBank = Bank0;
                    break;
            }
            if (structBank.slOPs.ContainsKey(saSA.StructUniqueAddress))
            {
                adderForConflict = 1;
                if (structBank.getByte(saSA.StructAddressInt + 1).ToLower() == "ff") adderForConflict = 2;
            }
            structBank = null;

            // Basic adder value search (when not 0)
            //Operation[] arrOps = getPrecedingOPs(srcOpe.AddressInt, 8, 0, false, false, false, false, false, false, false, false);
            Operation[] arrOps = getPrecedingOPs(srcOpe.AddressInt, 8, 0);
            foreach (Operation ope in arrOps)
            {
                if (ope == null) break;
                if (ope.isReturn) break;
                if (ope.CallType == CallType.Jump) break;

                if (ope.AddressInt == srcOpe.AddressInt) continue;
                if (ope.OperationParams.Length == 0) continue;

                if (adderInitialValue != 0 || adderStep != 0 || adderInitialScalar) break;
                
                // Main Register Init or Update
                if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == Tools.RegisterInstruction(mainRegCpy) || ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == Tools.RegisterInstruction(mainRegCpyTB))
                {
                    switch (ope.OriginalInstruction.ToLower())
                    {
                        case "ldw":
                        case "ldb":
                        case "ldzbw":
                        case "ldsbw":
                            switch (ope.OriginalOPCode.ToLower())
                            {
                                case "a0":
                                case "b0":
                                case "ac":
                                case "bc":
                                    // Reg Cpy
                                    mainRegCpy = ope.OriginalOpArr[1];
                                    mainRegCpyTB = Convert.ToString(Convert.ToInt32(mainRegCpy, 16) + 1, 16);
                                    continue;
                                case "a1":
                                case "b1":
                                case "ad":
                                    // Simple Value Init
                                    adderInitialValue = Convert.ToInt32(ope.OriginalOpArr[1], 16);
                                    adderCurrentValue += adderInitialValue;
                                    continue;
                                case "bd":
                                    // Simple Value Init
                                    adderInitialValue = Convert.ToSByte(ope.OriginalOpArr[1], 16);
                                    adderCurrentValue += adderInitialValue;
                                    continue;
                            }
                            Scalar sScalar = null;
                            SADBank readBank = null;
                            if (ope.alCalibrationElems != null)
                            {
                                if (ope.alCalibrationElems.Count != 0) sScalar = ((CalibrationElement)ope.alCalibrationElems[0]).ScalarElem;
                            }
                            else if (ope.OtherElemAddress != string.Empty)
                            {
                                sScalar = (Scalar)Calibration.slExtScalars[(Tools.UniqueAddress(ope.ReadDataBankNum, Convert.ToInt32(ope.OtherElemAddress, 16)))];
                            }
                            if (sScalar != null)
                            {
                                switch (sScalar.BankNum)
                                {
                                    case 8:
                                        readBank = Bank8;
                                        break;
                                    case 1:
                                        readBank = Bank1;
                                        break;
                                    case 9:
                                        readBank = Bank9;
                                        break;
                                    case 0:
                                        readBank = Bank0;
                                        break;
                                }
                                if (sScalar.Byte) adderInitialValue = readBank.getByteInt(sScalar.AddressInt, sScalar.Signed);
                                else adderInitialValue = readBank.getWordInt(sScalar.AddressInt, sScalar.Signed, true);
                                adderInitialScalar = true;
                                adderCurrentValue += adderInitialValue;
                                sScalar = null;
                                readBank = null;

                                // Cylinders / Injectors / Sparks number
                                switch (adderInitialValue)
                                {
                                    case 4:
                                    case 6:
                                    case 8:
                                        saSA.Number = 3;
                                        break;
                                }

                                continue;
                            }
                            break;
                        case "clrb":
                        case "clrw":
                            // Initial Value is confirmed at 0
                            adderInitialValue = 0;
                            continue;
                        case "incw":
                        case "incb":
                            if (adderStep == 0)
                            {
                                adderStep = 1;
                                adderCurrentValue++;
                            }
                            continue;
                        case "decw":
                        case "decb":
                            if (adderStep == 0)
                            {
                                adderStep = -1;
                                adderCurrentValue--;
                            }
                            continue;
                        case "ad2w":
                        case "ad2b":
                            switch (ope.OriginalOPCode.ToLower())
                            {
                                case "65":
                                case "75":
                                    if (adderStep == 0)
                                    {
                                        adderStep = Convert.ToInt32(ope.OperationParams[0].InstructedParam, 16);
                                        adderCurrentValue += adderStep;
                                    }
                                    continue;
                            }
                            continue;
                        case "sb2w":
                        case "sb2b":
                            switch (ope.OriginalOPCode.ToLower())
                            {
                                case "69":
                                case "79":
                                    if (adderStep == 0)
                                    {
                                        adderStep = 0 - Convert.ToInt32(ope.OperationParams[0].InstructedParam, 16);
                                        adderCurrentValue += adderStep;
                                    }
                                    continue;
                            }
                            continue;
                    }
                }
            }
            arrOps = null;

            /*
            int adderCurrentValue = 0;
            int adderInitialValue = 0;
            int adderStep = 0;

            bool adderInitialScalar = false;
            */

            if (adderInitialValue != 0 || adderStep != 0)
            {
                saSA.StructNewAddressInt = saSA.StructAddressInt + adderInitialValue + adderStep;
            }
            else if (adderForConflict != 0)
            {
                saSA.StructNewAddressInt = saSA.StructAddressInt + adderForConflict;
            }
        }

        // Main Structure Analysis Part
        private void processStructureAnalysisFollowUse(ref StructureAnalysis saSA, ref int mainRegPosition, ref bool loopProcessed)
        {
            object[] arrPointersValues = null;
            Operation[] arrOps = null;
            string mainRegCpy = string.Empty;
            string mainRegCpyTB = string.Empty;
            SortedList slPossibleLoopRegistersNumbers = null;

            arrOps = getFollowingOpsTree(saSA.AnalysisStartOpeAddressInt, 64, false);
            foreach (Operation ope in arrOps)
            {
                // Do not use with getFollowingOpsTree
                //if (ope == null) break;
                //if (ope.isReturn) break;
                //if (ope.CallType == CallType.Jump) break;

                if (ope.AddressInt == saSA.AnalysisSourceOpeAddressInt) continue;
                if (ope.AddressInt == saSA.AnalysisInitOpeAddressInt) continue;
                if (ope.AddressInt == saSA.LoopOpeAddressInt)
                {
                    if (saSA.slItems.Count > 0) loopProcessed = true;
                    if (saSA.LineSize != -1 && saSA.Number != -1) break;
                    if (saSA.LineSize != -1 && (saSA.ExitAddress != string.Empty || saSA.LoopExitOpeAddressInt != 1)) break;
                    if (loopProcessed) continue;
                }

                // Value Conditional Jump (For finding Exit)
                if (ope.OriginalOPCode.ToLower().StartsWith("d"))
                {
                    foreach (StructureItemAnalysis siaSIA in saSA.slItems.Values)
                    {
                        string sReg = siaSIA.ReadRegister;
                        string sRegTP = string.Empty;

                        if (sReg != string.Empty)
                        // Read Register is Used - GotoOpParams should contain it
                        {
                            if (Convert.ToInt32(sReg, 16) % 2 == 0) sRegTP = Convert.ToString(Convert.ToInt32(sReg, 16) + 1, 16);
                            if (ope.GotoOpParams != null)
                            {
                                string[] interestingGotoOpParams = new string[] { ope.GotoOpParams.OpeDefaultParamTranslation1, ope.GotoOpParams.OpeDefaultParamTranslation2 };
                                foreach (string sParam in interestingGotoOpParams)
                                {
                                    if (sParam.Contains(Tools.RegisterInstruction(sReg)) || (sRegTP != string.Empty && sParam.Contains(Tools.RegisterInstruction(sRegTP))))
                                    {
                                        if (!siaSIA.ReadCondOperations.Contains(ope))
                                        {
                                            siaSIA.ReadCondOperations.Add(ope);
                                            if (ope.GotoOpParams != null)
                                            {
                                                if (!siaSIA.ReadCondCmpOperations.ContainsKey(ope.GotoOpParams.OpeUniqueAddress)) siaSIA.ReadCondCmpOperations.Add(ope.GotoOpParams.OpeUniqueAddress, slOPs[ope.GotoOpParams.OpeUniqueAddress]);
                                            }
                                        }
                                        sReg = string.Empty;
                                        break;
                                    }
                                }
                                if (sReg == string.Empty) break;
                            }
                        }

                        // Main Register is directly used - GotoOpParams Operation should be the operation of the item
                        if (ope.GotoOpParams != null)
                        {
                            if (siaSIA.ReadOperation.UniqueAddress == ope.GotoOpParams.OpeUniqueAddress && !siaSIA.ReadCondOperations.Contains(ope))
                            {
                                siaSIA.ReadCondOperations.Add(ope);
                                if (!siaSIA.ReadCondCmpOperations.ContainsKey(ope.GotoOpParams.OpeUniqueAddress)) siaSIA.ReadCondCmpOperations.Add(ope.GotoOpParams.OpeUniqueAddress, slOPs[ope.GotoOpParams.OpeUniqueAddress]);
                                break;
                            }
                        }
                    }
                }

                if (ope.OperationParams.Length == 0) continue;

                // Main Register Cpy
                switch (ope.OriginalOPCode.ToLower())
                {
                    case "a0":  // ldw
                        if (ope.OriginalOpArr[1].ToLower() == saSA.MainRegister || ope.OriginalOpArr[1].ToLower() == mainRegCpy)
                        {
                            mainRegCpy = ope.OriginalOpArr[2].ToLower();
                            mainRegCpyTB = Convert.ToString(Convert.ToInt32(mainRegCpy, 16) + 1, 16);
                            continue;
                        }
                        break;
                }

                // Conditional Gotos Mngt
                switch (ope.OriginalInstruction.ToLower())
                {
                    case "jb":
                    case "jnb":
                        foreach (StructureItemAnalysis siaSIA in saSA.slItems.Values)
                        {
                            if (siaSIA.ReadRegister == string.Empty) continue;

                            string sReg = siaSIA.ReadRegister;
                            string sRegTP = string.Empty;
                            if (Convert.ToInt32(sReg, 16) % 2 == 0) sRegTP = Convert.ToString(Convert.ToInt32(sReg, 16) + 1, 16);
                            if (sReg == ope.OriginalOpArr[1].ToLower() || sRegTP == ope.OriginalOpArr[1].ToLower())
                            {
                                if (!siaSIA.ReadCondOperations.Contains(ope))
                                {
                                    siaSIA.ReadCondOperations.Add(ope);
                                    if (ope.GotoOpParams != null)
                                    {
                                        if (!siaSIA.ReadCondCmpOperations.ContainsKey(ope.GotoOpParams.OpeUniqueAddress)) siaSIA.ReadCondCmpOperations.Add(ope.GotoOpParams.OpeUniqueAddress, slOPs[ope.GotoOpParams.OpeUniqueAddress]);
                                    }
                                }
                                break;
                            }
                        }
                        break;
                }

                // Address Register ReInit
                if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == Tools.RegisterInstruction(saSA.MainRegister) || (mainRegCpy != string.Empty && ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == Tools.RegisterInstruction(mainRegCpy)))
                {
                    if (ope.OperationParams.Length > 2 && ope.OriginalInstruction.ToLower() != "stw" && ope.OriginalInstruction.ToLower() != "stw") break;
                    else
                    {
                        bool bBreak = false;
                        switch (ope.OriginalInstruction.ToLower())
                        {
                            case "ldw":
                            case "ldb":
                            case "clrb":
                            case "clrw":
                                if (mainRegCpy == string.Empty) bBreak = true;
                                else mainRegCpy = string.Empty;
                                break;
                        }
                        if (bBreak) break;
                    }
                }

                // CMPW/CMPB on addresses or values
                switch (ope.OriginalOPCode.ToLower())
                {
                    case "8b":
                    case "89":
                        // On Address - If exitAddress is greater than Struct address, it is its end address, else it is its start address    
                        if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == Tools.RegisterInstruction(saSA.MainRegister) || (mainRegCpy != string.Empty && ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == Tools.RegisterInstruction(mainRegCpy)))
                        {
                            if (ope.OperationParams[0].CalculatedParam != "ffff") // Specific case
                            {
                                saSA.LoopExitOpeAddressInt = ope.AddressInt;
                                saSA.ExitAddress = ope.OperationParams[0].CalculatedParam;
                            }
                        }
                        continue;
                    case "8a":
                        // Loop(or not) Exit Condition on first Item
                        // On Value - Always compared to register R00
                        if ((ope.OriginalOpArr[1] == saSA.MainRegister && ope.OriginalOpArr[2] == "00") || (ope.OriginalOpArr[1] == mainRegCpy && ope.OriginalOpArr[2] == "00"))
                        {
                            saSA.LoopExitFirstItemOpeAddressInt = ope.AddressInt;
                            saSA.LoopExitFirstItemValue = 0;
                        }
                        continue;
                }

                // Loop Register - Init Search before first Item Read
                if (saSA.LoopRegister == string.Empty)
                {
                    switch (ope.OriginalOPCode.ToLower())
                    {
                        case "b1":  // ldb - Counter used based on djnz 
                            if (slPossibleLoopRegistersNumbers == null) slPossibleLoopRegistersNumbers = new SortedList();
                            // Not a Loop Register
                            if (slPossibleLoopRegistersNumbers.ContainsKey(ope.OriginalOpArr[2])) slPossibleLoopRegistersNumbers.Remove(ope.OriginalOpArr[2]);
                            else slPossibleLoopRegistersNumbers.Add(ope.OriginalOpArr[2], Convert.ToInt32(ope.OriginalOpArr[1], 16));
                            break;
                        case "55":  // ad3b - Counter used based on djnz 
                            if (slPossibleLoopRegistersNumbers == null) slPossibleLoopRegistersNumbers = new SortedList();
                            if (ope.OriginalOpArr[2] == "00")
                            {
                                // Not a Loop Register
                                if (slPossibleLoopRegistersNumbers.ContainsKey(ope.OriginalOpArr[3])) slPossibleLoopRegistersNumbers.Remove(ope.OriginalOpArr[3]);
                                else slPossibleLoopRegistersNumbers.Add(ope.OriginalOpArr[3], Convert.ToInt32(ope.OriginalOpArr[1], 16));
                            }
                            break;
                        case "5d": // ml3b
                            if (saSA.slItems.Count == 0)
                            // Only before first Item Read
                            {
                                saSA.LoopRegister = ope.OriginalOpArr[2];
                                saSA.LineSize = Convert.ToInt32(ope.OriginalOpArr[1], 16);
                            }
                            break;
                    }
                }

                // Loop search / Even if already found with another address
                //  DJNZ part for Counters even if already found with another address
                if (ope.OriginalInstruction.ToLower() == "djnz" && saSA.slItems.Count > 0)
                {
                    // Possible Loop Registers used as counters
                    if (saSA.LoopRegister == string.Empty && slPossibleLoopRegistersNumbers != null)
                    {
                        if (slPossibleLoopRegistersNumbers.ContainsKey(ope.OriginalOpArr[1]))
                        {
                            saSA.LoopRegister = ope.OriginalOpArr[1];
                            saSA.Number = (int)slPossibleLoopRegistersNumbers[saSA.LoopRegister] - 1;
                            saSA.LoopOpeAddressInt = ope.AddressInt;
                            saSA.LoopReversed = true;
                            slPossibleLoopRegistersNumbers = null;
                            loopProcessed = true;
                            continue;
                        }
                    }
                }
                //  Everything else
                if (saSA.LoopOpeAddressInt == -1)
                {
                    if (ope.CallType == CallType.Goto || ope.CallType == CallType.ShortJump)
                    {
                        if (ope.OriginalInstruction.ToLower() == "djnz" && saSA.slItems.Count > 0)
                        {
                            if (ope.OriginalOpArr[1] == saSA.MainRegister || ope.OriginalOpArr[1] == mainRegCpy || ope.OriginalOpArr[1] == saSA.LoopRegister)
                            {
                                saSA.LoopOpeAddressInt = ope.AddressInt;
                                saSA.LoopReversed = true;
                                loopProcessed = true;
                                continue;
                            }
                        }
                        Operation loopOpe = (Operation)slOPs[Tools.UniqueAddress(ope.BankNum, ope.AddressJumpInt)];
                        if (loopOpe == null) continue;
                        
                        // Basic Rule
                        if (loopOpe.AddressInt >= saSA.AnalysisStartOpeAddressInt)
                        {
                            //saSA.LoopOpeAddressInt = loopOpe.AddressInt;
                            saSA.LoopOpeAddressInt = -2;
                            foreach (StructureItemAnalysis siaSIA in saSA.slItems.Values)
                            {
                                if (loopOpe.AddressInt > siaSIA.ReadOperation.AddressInt)
                                {
                                    saSA.LoopOpeAddressInt = -1;
                                    break;
                                }
                            }
                            if (saSA.LoopOpeAddressInt == -2) saSA.LoopOpeAddressInt = loopOpe.AddressInt;
                            else continue;
                        }

                        if (loopOpe.OperationParams.Length == 0) continue;
                        if (loopOpe.OriginalOpArr.Length == 1) continue;

                        // Loop on MainRegister Top Byte (Like Increment)
                        if (loopOpe.OriginalOpArr[1] == saSA.MainRegisterTB || loopOpe.OriginalOpArr[1] == mainRegCpyTB)
                        {
                            saSA.LoopOpeAddressInt = loopOpe.AddressInt;
                            continue;
                        }

                        // Deeper Rule based on registers
                        if (saSA.LoopRegister != string.Empty && loopOpe.OriginalOpArr[loopOpe.OriginalOpArr.Length - 1] != saSA.LoopRegister) continue;
                        if (saSA.LoopRegister == string.Empty)
                        {
                            if (loopOpe.OriginalOpArr[loopOpe.OriginalOpArr.Length - 1] != saSA.MainRegister && loopOpe.OriginalOpArr[loopOpe.OriginalOpArr.Length - 1] != mainRegCpy) continue;
                        }
                        switch (loopOpe.OriginalInstruction.ToLower())
                        {
                            case "ad2w":
                            case "ad2b":
                            case "sb2w":
                            case "sb2b":
                                switch (loopOpe.OriginalOpArr[0].ToLower())
                                {
                                    case "65":
                                    case "75":
                                        saSA.LoopOpeAddressInt = loopOpe.AddressInt;
                                        saSA.LineSize = Convert.ToInt32(loopOpe.OperationParams[0].InstructedParam, 16);
                                        break;
                                    case "69":
                                    case "79":
                                        saSA.LoopOpeAddressInt = loopOpe.AddressInt;
                                        saSA.LineSize = Convert.ToInt32(loopOpe.OperationParams[0].InstructedParam, 16);
                                        saSA.LoopReversed = true;
                                        break;
                                }
                                continue;
                            case "incw":
                            case "incb":
                                saSA.LoopOpeAddressInt = loopOpe.AddressInt;
                                break;
                            case "decw":
                            case "decb":
                                saSA.LoopOpeAddressInt = loopOpe.AddressInt;
                                saSA.LoopReversed = true;
                                break;
                        }
                        continue;
                    }
                }

                // Loop on Loop Register
                if (saSA.LoopOpeAddressInt == -1 && saSA.LoopRegister != string.Empty)
                {
                    if (ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] == saSA.LoopRegister)
                    {
                        if (saSA.LoopOpeAddressInt == -1 && Calibration.slCalls.ContainsKey(ope.UniqueAddress))
                        {
                            saSA.LoopOpeAddressInt = ope.AddressInt;
                        }
                    }
                }

                if (loopProcessed) continue;

                // Increments / Loop
                if (ope.OriginalInstruction.ToLower() == "djnz" && saSA.slItems.Count > 0)
                {
                    if (ope.OriginalOpArr[1] == saSA.MainRegister || ope.OriginalOpArr[1] == mainRegCpy || ope.OriginalOpArr[1] == saSA.LoopRegister)
                    {
                        if (mainRegPosition > 0) mainRegPosition--;
                        saSA.LoopReversed = true;
                        loopProcessed = true;
                    }
                }
                if (ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] == saSA.MainRegister || ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] == mainRegCpy)
                {
                    // Loop
                    if (saSA.LoopOpeAddressInt == -1 && Calibration.slCalls.ContainsKey(ope.UniqueAddress))
                    {
                        if (saSA.slItems.Count == 0) saSA.LoopOpeAddressInt = ope.AddressInt;
                        switch (ope.OriginalInstruction.ToLower())
                        {
                            case "ad2w":
                            case "ad2b":
                            case "sb2w":
                            case "sb2b":
                                switch (ope.OriginalOpArr[0].ToLower())
                                {
                                    case "65":
                                    case "75":
                                        saSA.LineSize = Convert.ToInt32(ope.OperationParams[0].InstructedParam, 16);
                                        if (saSA.slItems.Count == 0) saSA.StructNewAddressInt = saSA.StructAddressInt + saSA.LineSize;
                                        break;
                                    case "69":
                                    case "79":
                                        saSA.LineSize = Convert.ToInt32(ope.OperationParams[0].InstructedParam, 16);
                                        if (saSA.slItems.Count == 0) saSA.StructNewAddressInt = saSA.StructAddressInt - saSA.LineSize;
                                        saSA.LoopReversed = true;
                                        break;
                                }
                                continue;
                        }
                    }

                    // Basic Increment
                    switch (ope.OriginalInstruction.ToLower())
                    {
                        case "incw":
                        case "incb":
                            mainRegPosition++;
                            continue;
                        case "decw":
                        case "decb":
                            if (mainRegPosition > 0) mainRegPosition--;
                            saSA.LoopReversed = true;
                            continue;
                        case "ad2w":
                        case "ad2b":
                            if (ope.AddressInt == saSA.LoopOpeAddressInt) continue;
                            switch (ope.OriginalOpArr[0].ToLower())
                            {
                                case "65":
                                case "75":
                                    mainRegPosition += Convert.ToInt32(ope.OperationParams[0].InstructedParam, 16);
                                    break;
                            }
                            continue;
                    }
                }

                // Reading Structure
                if (ope.OperationParams[0].InstructedParam.StartsWith(SADDef.LongRegisterPointerPrefix) && ope.OperationParams[0].InstructedParam.EndsWith(SADDef.LongRegisterPointerSuffix))
                {
                    arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                    if (!(bool)arrPointersValues[0]) continue;
                    if (arrPointersValues[1].ToString() == saSA.MainRegister || arrPointersValues[1].ToString() == saSA.MainRegisterTB || arrPointersValues[1].ToString() == mainRegCpy || arrPointersValues[1].ToString() == mainRegCpyTB)
                    {
                        StructureItemAnalysis siaSIA = null;
                        if (arrPointersValues.Length <= 3)
                        {
                            if (!saSA.slItems.ContainsKey(mainRegPosition))
                            {
                                siaSIA = new StructureItemAnalysis(mainRegPosition);
                                siaSIA.ReadOperation = ope;
                                saSA.slItems.Add(mainRegPosition, siaSIA);
                                arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam);
                                if ((bool)arrPointersValues[0]) siaSIA.ReadRegister = arrPointersValues[1].ToString();
                                siaSIA = null;
                            }
                        }
                        else if (ope.OtherElemAddress == string.Empty)
                        {
                            int tmpMainRegPosition = mainRegPosition + (int)arrPointersValues[4];
                            if (!saSA.slItems.ContainsKey(tmpMainRegPosition))
                            {
                                siaSIA = new StructureItemAnalysis(tmpMainRegPosition);
                                siaSIA.ReadOperation = ope;
                                saSA.slItems.Add(tmpMainRegPosition, siaSIA);
                                arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam);
                                if ((bool)arrPointersValues[0]) siaSIA.ReadRegister = arrPointersValues[1].ToString();
                                siaSIA = null;
                            }
                        }
                        if (ope.OperationParams[0].InstructedParam.Contains(SADDef.IncrementSuffix))
                        {
                            mainRegPosition++;
                            SADOPCode opCode = (SADOPCode)Calibration.slOPCodes[ope.OriginalOPCode];
                            if (opCode != null)
                            {
                                switch (opCode.Type)
                                {
                                    case OPCodeType.WordOP:
                                        mainRegPosition++;
                                        break;
                                    case OPCodeType.MixedOP:
                                        switch (ope.OriginalInstruction.ToLower())
                                        {
                                            case "ldzbw":
                                            case "ldsbw":
                                                break;
                                            default:
                                                mainRegPosition++;
                                                break;
                                        }
                                        break;
                                }
                                opCode = null;
                            }
                        }
                    }

                    arrPointersValues = null;
                }
            }
            arrOps = null;

            slPossibleLoopRegistersNumbers = null;
        }

        // Structure Item Analysis
        private void processStructureAnalysisFollowItem(ref StructureAnalysis saSA, ref StructureItemAnalysis siaSIA, bool firstTry)
        {
            if (!siaSIA.isBaseIdentified)
            {
                SADOPCode opCode = null;
                if (siaSIA.ReadOperation.OriginalOPCode.ToLower() == "fe")
                {
                    opCode = (SADOPCode)Calibration.slOPCodes[siaSIA.ReadOperation.OriginalOpArr[1]];
                    siaSIA.isSigned = true;
                }
                else opCode = (SADOPCode)Calibration.slOPCodes[siaSIA.ReadOperation.OriginalOpArr[0]];

                if (opCode == null) return;

                switch (opCode.Type)
                {
                    case OPCodeType.WordOP:
                        siaSIA.isByte = false;
                        break;
                    case OPCodeType.MixedOP:
                        if (opCode.Instruction.ToLower() == "ldsbw") siaSIA.isSigned = true;
                        break;
                }
                opCode = null;

                // Top Byte Register Mngt
                if (siaSIA.isByte && siaSIA.ReadRegister != string.Empty && saSA.slItems.ContainsKey(siaSIA.Position - 1))
                {
                    if (((StructureItemAnalysis)saSA.slItems[siaSIA.Position - 1]).ReadRegister != string.Empty)
                    {
                        if (Convert.ToInt32(siaSIA.ReadRegister, 16) % 2 != 0 && Convert.ToInt32(siaSIA.ReadRegister, 16) - Convert.ToInt32(((StructureItemAnalysis)saSA.slItems[siaSIA.Position - 1]).ReadRegister, 16) == 1)
                        {
                            siaSIA.isTopByte = true;
                            ((StructureItemAnalysis)saSA.slItems[siaSIA.Position - 1]).isWordLowByte = true;
                        }
                    }
                }

                siaSIA.isBaseIdentified = true;
                saSA.DefinitionStrings = null;  // To Force DefinitionStrings recalculation
            }

            // Already Identified for this part
            if (siaSIA.isUseIdentified) return;

            if (siaSIA.ReadRegister == string.Empty)
            {
                // Vectors Detection directly on Read Operation
                switch (siaSIA.ReadOperation.OriginalInstruction.ToLower())
                {
                    case "push":
                        siaSIA.forVectorPush = true;    // So it is a Vector
                        siaSIA.VectorBank = siaSIA.ReadOperation.ApplyOnBankNum;
                        siaSIA.isUseIdentified = true;
                        saSA.DefinitionStrings = null;  // To Force DefinitionStrings recalculation
                        return;
                }

                return;
            }

            string regCpy = siaSIA.ReadRegister;
            string regCpyTB = string.Empty;
            if (!siaSIA.isByte) regCpyTB = Convert.ToString(Convert.ToInt32(regCpy, 16) + 1, 16);

            Operation[] arrOps = null;

            // DO NOT USE getFollowingOpsTree, because it will always include Call and ShortCalls
            /*
            // First Try - Following True Conditional Gotos
            if (firstTry) arrOps = getFollowingOpsTree(siaSIA.ReadOperation.AddressInt, 16, true);
            // Second Try - Following False Conditional Gotos
            else arrOps = arrOps = getFollowingOpsTree(siaSIA.ReadOperation.AddressInt, 16, false);
            */

            // First Try - With Jumps
            if (firstTry) arrOps = getFollowingOPs(siaSIA.ReadOperation.AddressInt, 16, 1, true, true, true, true, false, true, true, true);
            // Second Try - Without Jumps
            else arrOps = getFollowingOPs(siaSIA.ReadOperation.AddressInt, 16, 1, true, true, false, false, false, false, false, true);

            foreach (Operation ope in arrOps)
            {
                if (ope == null) break;
                if (ope.isReturn) break;
                if (ope.CallType == CallType.Jump) break;

                if (ope.AddressInt == saSA.AnalysisSourceOpeAddressInt) continue;
                if (ope.AddressInt == saSA.AnalysisInitOpeAddressInt) continue;
                if (ope.AddressInt == saSA.LoopOpeAddressInt) continue;
                if (ope.AddressInt == siaSIA.ReadOperation.AddressInt) continue;

                if (ope.OperationParams.Length == 0) continue;

                bool bBreak = false;

                // Vectors Detection
                switch (ope.OriginalInstruction.ToLower())
                {
                    case "push":
                        if (ope.OperationParams[0].InstructedParam == Tools.RegisterInstruction(regCpy))
                        {
                            siaSIA.forVectorPush = true;    // So it is a Vector
                            siaSIA.VectorBank = ope.ApplyOnBankNum;
                            siaSIA.isUseIdentified = true;
                        }
                        break;
                }
                if (siaSIA.isUseIdentified)
                {
                    saSA.DefinitionStrings = null;  // To Force DefinitionStrings recalculation
                    break;
                }

                // Pointer Detection / Pointer use
                if (ope.OperationParams[0].InstructedParam == Tools.PointerTranslation(Tools.RegisterInstruction(regCpy)) || (siaSIA.PointerRegister != string.Empty && ope.OperationParams[0].InstructedParam == Tools.PointerTranslation(siaSIA.PointerRegister)))
                {
                    switch (ope.OriginalInstruction.ToLower())
                    {
                        case "stb":
                        case "stw":
                        case "pop":
                            siaSIA.forStoring = true;   // So it is a Register
                            siaSIA.isUseIdentified = true;
                            break;
                        case "push":
                            siaSIA.forVectorPush = true;    // So it is a Vector
                            siaSIA.VectorBank = ope.ApplyOnBankNum;
                            siaSIA.isUseIdentified = true;
                            break;
                        default:
                            siaSIA.forReading = true;   // Could be Register or Calibration Element or Other Element (based on value)
                            break;
                    }
                    // Consequences
                    // Pointer is not signed
                    siaSIA.isSigned = false;

                    if (siaSIA.isUseIdentified)
                    {
                        saSA.DefinitionStrings = null;  // To Force DefinitionStrings recalculation
                        break;
                    }

                    // Destination Register Analysis
                    switch (ope.OriginalInstruction.ToLower())
                    {
                        case "cmpb":
                        case "cmpw":
                            siaSIA.isUseIdentified = true;
                            break;
                        case "ldw":
                            siaSIA.PointerRegister = ope.OriginalOpArr[ope.OriginalOpArr.Length - 1];
                            break;
                        case "ad2w":
                        case "ad3w":
                            //a2,32,34             ldw   R34,[R32]          R34 = [R32];            => [R32] is RBase Adder
                            //af,32,06,4c          ldzbw R4c,[R32+6]        R4c = (uns)[R32+6];  
                            //66,4c,34             ad2w  R34,[R4c]          R34 += [R4c];           => [R4c] is RBase
                            // RBase detection
                            //      Searching if register is added to another known one, which is word one.
                            foreach (StructureItemAnalysis siaRBAdder in saSA.slItems.Values)
                            {
                                if (ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] == siaRBAdder.ReadRegister)
                                {
                                    if (siaSIA.isByte)
                                    {
                                        siaSIA.isRBase = true;
                                        siaSIA.calcRBaseRegister = siaRBAdder.ReadRegister;
                                        siaSIA.isUseIdentified = true;
                                        siaRBAdder.forReading = true;
                                        siaRBAdder.isRBaseAdder = true;
                                        siaRBAdder.calcRBaseRegister = siaSIA.calcRBaseRegister;
                                        siaRBAdder.isUseIdentified = true;
                                    }
                                    else if (siaRBAdder.isByte)
                                    {
                                        siaRBAdder.isRBase = true;
                                        siaRBAdder.calcRBaseRegister = siaRBAdder.ReadRegister;
                                        siaSIA.forReading = true;
                                        siaSIA.isRBaseAdder = true;
                                        siaSIA.calcRBaseRegister = siaRBAdder.calcRBaseRegister;
                                        siaSIA.isUseIdentified = true;
                                    }
                                    else
                                    //  By Default (both Words) we consider that RBase is use at then end
                                    {
                                        siaSIA.isRBase = true;
                                        siaSIA.calcRBaseRegister = siaRBAdder.ReadRegister;
                                        siaSIA.isUseIdentified = true;
                                        siaRBAdder.forReading = true;
                                        siaRBAdder.isRBaseAdder = true;
                                        siaRBAdder.calcRBaseRegister = siaSIA.calcRBaseRegister;
                                        siaRBAdder.isUseIdentified = true;
                                    }
                                    break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    if (siaSIA.isUseIdentified)
                    {
                        saSA.DefinitionStrings = null;  // To Force DefinitionStrings recalculation
                        break;
                    }
                }

                // Break Rules
                // Basic ones - Just for memory - Normally Break as already been done
                if (siaSIA.forStoring) break;
                if (siaSIA.forVectorPush) break;
                if (siaSIA.isRBase || siaSIA.isRBaseAdder) break;

                // Loop(or not) Exit Condition on first Item
                // On Value - Always compared to register R00
                // Done in this place because based on copied register
                // Essentially used for Vectors
                if (siaSIA.Position == 0 && !siaSIA.isByte)
                {
                    switch (ope.OriginalOPCode.ToLower())
                    {
                        case "88":
                            if ((ope.OriginalOpArr[1] == siaSIA.ReadRegister && ope.OriginalOpArr[2] == "00") || (ope.OriginalOpArr[2] == siaSIA.ReadRegister && ope.OriginalOpArr[1] == "00"))
                            {
                                saSA.LoopExitFirstItemOpeAddressInt = ope.AddressInt;
                                saSA.LoopExitFirstItemValue = 0;
                            }
                            continue;
                    }
                }

                // RegCpy / PointRegister reinit - Check to be processed after Pointer Detection
                switch (ope.OriginalInstruction.ToLower())
                {
                    case "stb":
                    case "stw":
                        if ((ope.OriginalOpArr[0] == regCpy && siaSIA.PointerRegister == string.Empty) || ope.OriginalOpArr[0] == siaSIA.PointerRegister)
                        {
                            bBreak = true;
                        }
                        break;
                    case "cmpb":
                    case "cmpw":
                        // No Reinit here
                        break;
                    default:
                        if ((ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] == regCpy && siaSIA.PointerRegister == string.Empty) || ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] == siaSIA.PointerRegister)
                        {
                            bBreak = true;
                        }
                        break;
                }
                if (bBreak) break;

                // RegCpy change
                switch (ope.OriginalInstruction.ToLower())
                {
                    case "stb":
                    case "stw":
                        if (ope.OperationParams[ope.OperationParams.Length - 1].CalculatedParam == Tools.RegisterInstruction(regCpy))
                        {
                            regCpy = Tools.InstructionPointersValues(ope.OperationParams[0].CalculatedParam)[1].ToString();
                            if (!siaSIA.isByte) regCpyTB = Convert.ToString(Convert.ToInt32(regCpy, 16) + 1, 16);
                        }
                        break;
                    case "ldb":
                    case "ldw":
                        if (ope.OperationParams[0].CalculatedParam == Tools.RegisterInstruction(regCpy))
                        {
                            regCpy = Tools.InstructionPointersValues(ope.OperationParams[ope.OperationParams.Length - 1].CalculatedParam)[1].ToString();
                            if (!siaSIA.isByte) regCpyTB = Convert.ToString(Convert.ToInt32(regCpy, 16) + 1, 16);
                        }
                        break;
                }
            }
            arrOps = null;

            if (siaSIA.isUseIdentified)
            {
                saSA.DefinitionStrings = null;  // To Force DefinitionStrings recalculation
                return;
            }

            // Second Try
            if (firstTry) processStructureAnalysisFollowItem(ref saSA, ref siaSIA, false);
        }
        
        public void processElemTranslations(ref SADS6x S6x)
        {
            Operation ope = null;
            //SADOPCode opCode = null;
            CalibrationElement calElem = null;
            string elemAddress = string.Empty;
            string elemShortLabel = string.Empty;

            // Calibrations Elements identified by Disassembly Only
            foreach (string uniqueAddress in alCalibElemOPsUniqueAddresses)
            {
                ope = (Operation)slOPs[uniqueAddress];

                if (ope.alCalibrationElems == null) continue;
                foreach (CalibrationElement opeCalElem in ope.alCalibrationElems)
                {
                    calElem = (CalibrationElement)Calibration.slCalibrationElements[opeCalElem.UniqueAddress];
                    elemAddress = calElem.Address;
                    elemShortLabel = string.Empty;
                    if (calElem.isTable) elemShortLabel = calElem.TableElem.ShortLabel;
                    else if (calElem.isFunction) elemShortLabel = calElem.FunctionElem.ShortLabel;
                    else if (calElem.isScalar) elemShortLabel = calElem.ScalarElem.ShortLabel;
                    else if (calElem.isStructure) elemShortLabel = calElem.StructureElem.ShortLabel;
                    calElem = null;

                    if (elemShortLabel != string.Empty && elemShortLabel != null)
                    {
                        ope.TranslationReplacementAddress = elemAddress;
                        ope.TranslationReplacementLabel = elemShortLabel;
                    }
                }
                ope = null;
            }

            // Known Elements identified
            //  Can be S6x Register
            //  Can be Reserved Address
            //  Can be Calibration External Elements
            //  Can be Ope/Call                 => To be Managed Later On
            //  Can be Additional Vector Source Address on Calibration Bank
            //  Can be CC / EC Register ranges - 8061 only
            foreach (string uniqueAddress in alPossibleKnownElemOPsUniqueAddresses)
            {
                int address = -1;
                string elemUniqueAddress = string.Empty;
                
                ope = (Operation)slOPs[uniqueAddress];

                if (ope == null) continue;
                if (ope.KnownElemAddress == string.Empty) continue;

                elemShortLabel = string.Empty;
                address = Convert.ToInt32(ope.KnownElemAddress, 16);

                if (elemShortLabel == string.Empty)
                {
                    // Exiting S6x Register, KnwonElemAddress is translated
                    if (S6x.slRegisters.ContainsKey(Tools.RegisterUniqueAddress(address)))
                    {
                        if (((S6xRegister)S6x.slRegisters[Tools.RegisterUniqueAddress(address)]).MultipleMeanings)
                        {
                            bool byteOpe = true;
                            switch (((SADOPCode)Calibration.slOPCodes[ope.OriginalOPCode]).Type)
                            {
                                case OPCodeType.WordOP:
                                    byteOpe = false;
                                    break;
                                case OPCodeType.MixedOP:
                                    byteOpe = (ope.OriginalInstruction.ToLower() != "stw");
                                    break;
                            }
                            elemShortLabel = ((S6xRegister)S6x.slRegisters[Tools.RegisterUniqueAddress(address)]).Labels(byteOpe);
                        }
                        else
                        {
                            elemShortLabel = ((S6xRegister)S6x.slRegisters[Tools.RegisterUniqueAddress(address)]).Label;
                        }
                    }
                }

                if (elemShortLabel == string.Empty)
                {
                    // Unique address
                    elemUniqueAddress = Tools.UniqueAddress(ope.ReadDataBankNum, address - SADDef.EecBankStartAddress);

                    if (elemShortLabel == string.Empty)
                    {
                        // Known Reserved Address
                        if (slReserved.ContainsKey(elemUniqueAddress))
                        {
                            elemShortLabel = ((ReservedAddress)slReserved[elemUniqueAddress]).ShortLabel;
                        }
                    }

                    if (elemShortLabel == string.Empty)
                    {
                        // Calibration External Elements
                        if (Calibration.slExtStructures.ContainsKey(elemUniqueAddress))
                        {
                            elemShortLabel = ((Structure)Calibration.slExtStructures[elemUniqueAddress]).ShortLabel;
                        }
                        else if (Calibration.slExtTables.ContainsKey(elemUniqueAddress))
                        {
                            elemShortLabel = ((Table)Calibration.slExtTables[elemUniqueAddress]).ShortLabel;
                        }
                        else if (Calibration.slExtFunctions.ContainsKey(elemUniqueAddress))
                        {
                            elemShortLabel = ((Function)Calibration.slExtFunctions[elemUniqueAddress]).ShortLabel;
                        }
                        else if (Calibration.slExtScalars.ContainsKey(elemUniqueAddress))
                        {
                            elemShortLabel = ((Scalar)Calibration.slExtScalars[elemUniqueAddress]).ShortLabel;
                        }
                    }
                    
                    // Known Ope/Call
                    // Not managed at this moment, will be in next methods

                    if (elemShortLabel == string.Empty)
                    {
                        // Known Additional Vector Source Address on Calibration Bank
                        // ======> TO BE REVIEWED, LABEL IS PROBABLY UPADTED AFTER THIS
                        if (ope.ReadDataBankNum == Calibration.BankNum)
                        {
                            foreach (Vector vect in Calibration.slAdditionalVectors.Values)
                            {
                                if (vect.SourceAddress == ope.KnownElemAddress)
                                {
                                    elemShortLabel = vect.ShortLabel;
                                    break;
                                }
                            }
                        }
                    }

                    if (elemShortLabel == string.Empty)
                    {
                        // Known CC / EC Register ranges - 8061 only
                        //  Based on Ope Cc/Ec Flags 
                        if (is8061)
                        {
                            if (ope.isCcRelated || ope.isCcRelated)
                            {
                                if (address >= SADDef.CCMemory8061MinAdress && address <= SADDef.CCMemory8061MaxAdress)
                                {
                                    elemShortLabel = SADDef.CCMemory8061Template.Replace("%LREG%", ope.KnownElemAddress);
                                }
                                else if (address >= SADDef.ECMemory8061MinAdress && address <= SADDef.ECMemory8061MaxAdress)
                                {
                                    elemShortLabel = SADDef.ECMemory8061Template.Replace("%LREG%", ope.KnownElemAddress);
                                }
                            }
                        }
                    }
                }

                if (elemShortLabel != string.Empty && elemShortLabel != null)
                {
                    ope.TranslationReplacementAddress = ope.KnownElemAddress;
                    ope.TranslationReplacementLabel = elemShortLabel;

                }

                ope = null;
            }
            
            // Possible Elements identified by Disassembly Only
            foreach (string uniqueAddress in alPossibleOtherElemOPsUniqueAddresses)
            {
                ope = (Operation)slOPs[uniqueAddress];

                if (ope.OtherElemAddress != string.Empty)
                {
                    elemAddress = ope.OtherElemAddress;
                    elemShortLabel = string.Empty;
                    string calElemUniqueAddress = string.Empty;
                    string elemUniqueAddress = string.Empty;
                    try
                    {
                        calElemUniqueAddress = Tools.UniqueAddress(ope.ReadDataBankNum, Convert.ToInt32(elemAddress, 16) - SADDef.EecBankStartAddress);
                        elemUniqueAddress = Tools.UniqueAddress(ope.ReadDataBankNum, Convert.ToInt32(elemAddress, 16) - SADDef.EecBankStartAddress);
                    }
                    catch
                    {
                        calElemUniqueAddress = string.Empty;
                        elemUniqueAddress = string.Empty;
                    }
                    if (calElemUniqueAddress != string.Empty && elemUniqueAddress != string.Empty)
                    {
                        if (Calibration.slCalibrationElements.ContainsKey(calElemUniqueAddress))
                        {
                            calElem = (CalibrationElement)Calibration.slCalibrationElements[calElemUniqueAddress];
                            if (calElem.isTable) elemShortLabel = calElem.TableElem.ShortLabel;
                            else if (calElem.isFunction) elemShortLabel = calElem.FunctionElem.ShortLabel;
                            else if (calElem.isScalar) elemShortLabel = calElem.ScalarElem.ShortLabel;
                            else if (calElem.isStructure) elemShortLabel = calElem.StructureElem.ShortLabel;
                            calElem = null;
                        }
                        else if (Calibration.slExtTables.ContainsKey(elemUniqueAddress))
                        {
                            elemShortLabel = ((Table)Calibration.slExtTables[elemUniqueAddress]).ShortLabel;
                        }
                        else if (Calibration.slExtFunctions.ContainsKey(elemUniqueAddress))
                        {
                            elemShortLabel = ((Function)Calibration.slExtFunctions[elemUniqueAddress]).ShortLabel;
                        }
                        else if (Calibration.slExtScalars.ContainsKey(elemUniqueAddress))
                        {
                            elemShortLabel = ((Scalar)Calibration.slExtScalars[elemUniqueAddress]).ShortLabel;
                        }
                        else if (Calibration.slExtStructures.ContainsKey(elemUniqueAddress))
                        {
                            elemShortLabel = ((Structure)Calibration.slExtStructures[elemUniqueAddress]).ShortLabel;
                        }
                    }
                    if (elemShortLabel != string.Empty && elemShortLabel != null)
                    {
                        ope.TranslationReplacementAddress = elemAddress;
                        ope.TranslationReplacementLabel = elemShortLabel;
                    }
                }
                ope = null;
            }

            // Elements related with Goto Ops identified by Disassembly Only
            foreach (string uniqueAddress in alElemGotoOPsUniqueAddresses)
            {
                ope = (Operation)slOPs[uniqueAddress];

                if (ope.GotoOpParams != null)
                {
                    // Elem UniqueAddres (CalElem or Other)
                    if (ope.GotoOpParams.OpeUniqueAddress != string.Empty && ope.GotoOpParams.ElemUniqueAddress != string.Empty)
                    {
                        elemAddress = ope.GotoOpParams.ElemAddress;
                        elemShortLabel = string.Empty;
                        Operation relOpe = (Operation)slOPs[ope.GotoOpParams.OpeUniqueAddress];
                        string calElemUniqueAddress = string.Empty;
                        string elemUniqueAddress = string.Empty;
                        if (relOpe != null)
                        {
                            try
                            {
                                calElemUniqueAddress = Tools.UniqueAddress(relOpe.ReadDataBankNum, Convert.ToInt32(elemAddress, 16) - SADDef.EecBankStartAddress);
                                elemUniqueAddress = Tools.UniqueAddress(relOpe.ApplyOnBankNum, Convert.ToInt32(elemAddress, 16) - SADDef.EecBankStartAddress);
                            }
                            catch
                            {
                                calElemUniqueAddress = string.Empty;
                                elemUniqueAddress = string.Empty;
                            }
                        }
                        relOpe = null;

                        if (calElemUniqueAddress != string.Empty && elemUniqueAddress != string.Empty)
                        {
                            if (Calibration.slCalibrationElements.ContainsKey(calElemUniqueAddress))
                            {
                                calElem = (CalibrationElement)Calibration.slCalibrationElements[calElemUniqueAddress];
                                elemAddress = calElem.Address;
                                if (calElem.isTable) elemShortLabel = calElem.TableElem.ShortLabel;
                                else if (calElem.isFunction) elemShortLabel = calElem.FunctionElem.ShortLabel;
                                else if (calElem.isScalar) elemShortLabel = calElem.ScalarElem.ShortLabel;
                                else if (calElem.isStructure) elemShortLabel = calElem.StructureElem.ShortLabel;
                                calElem = null;
                            }
                            else if (Calibration.slExtTables.ContainsKey(elemUniqueAddress))
                            {
                                elemAddress = ((Table)Calibration.slExtTables[elemUniqueAddress]).Address;
                                elemShortLabel = ((Table)Calibration.slExtTables[elemUniqueAddress]).ShortLabel;
                            }
                            else if (Calibration.slExtFunctions.ContainsKey(elemUniqueAddress))
                            {
                                elemAddress = ((Function)Calibration.slExtFunctions[elemUniqueAddress]).Address;
                                elemShortLabel = ((Function)Calibration.slExtFunctions[elemUniqueAddress]).ShortLabel;
                            }
                            else if (Calibration.slExtScalars.ContainsKey(elemUniqueAddress))
                            {
                                elemAddress = ((Scalar)Calibration.slExtScalars[elemUniqueAddress]).Address;
                                elemShortLabel = ((Scalar)Calibration.slExtScalars[elemUniqueAddress]).ShortLabel;
                            }
                            else if (Calibration.slExtStructures.ContainsKey(elemUniqueAddress))
                            {
                                elemAddress = ((Structure)Calibration.slExtStructures[elemUniqueAddress]).Address;
                                elemShortLabel = ((Structure)Calibration.slExtStructures[elemUniqueAddress]).ShortLabel;
                            }
                        }

                        if (elemShortLabel != string.Empty && elemShortLabel != null)
                        {
                            ope.TranslationReplacementAddress = elemAddress;
                            ope.TranslationReplacementLabel = elemShortLabel;
                        }
                    }
                }
                ope = null;
            }        
        }

        // Apply Call translations on Operations, after translations done on with Calibration.processCallTranslations
        public void processCallTranslations(ref SADS6x S6x)
        {
            Operation ope = null;
            Call cCall = null;
            string sTranslation = string.Empty;

            foreach (string uniqueAddress in alCallOPsUniqueAddresses)
            {
                ope = (Operation)slOPs[uniqueAddress];
                
                if (ope == null) continue;

                if (Calibration.alMainCallsUniqueAddresses.Contains(Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)))
                // Main Calls mngt
                {
                    cCall = (Call)Calibration.slCalls[Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)];
                    if (cCall.CallType != CallType.Unknown && cCall.CallType != CallType.Skip)
                    {
                        ope.TranslationReplacementAddress = cCall.Address;
                        ope.TranslationReplacementLabel = cCall.ShortLabel;
                    }
                    cCall = null;
                }
                else if (S6x.slProcessRoutines.ContainsKey(Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)))
                // Not Existing Call, but S6x Declared Routine
                //  Essentially to override fixed translation on Fake Calls
                {
                    ope.TranslationReplacementAddress = ope.AddressJump;
                    ope.TranslationReplacementLabel = ((S6xRoutine)S6x.slProcessRoutines[Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)]).ShortLabel;
                }
                else if (S6x.slProcessOperations.ContainsKey(Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)))
                // Not Existing Call, but S6x Declared Operation
                //  Essentially to override fixed translation on Fake Calls
                {
                    ope.TranslationReplacementAddress = ope.AddressJump;
                    ope.TranslationReplacementLabel = ((S6xOperation)S6x.slProcessOperations[Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)]).ShortLabel;
                }
                else if (is8061)
                // 8061 Calibration Console / Engineering Console mngt
                {
                    switch (ope.CallType)
                    {
                        case CallType.Call:
                        case CallType.Jump:
                            if (ope.AddressJumpInt + SADDef.EecBankStartAddress >= SADDef.CCMemory8061MinAdress && ope.AddressJumpInt + SADDef.EecBankStartAddress <= SADDef.CCMemory8061MaxAdress)
                            {
                                // Specific Case, S6x Registers are allowed to store CC/EC Memory Addresses for Translation
                                if (S6x.slRegisters.ContainsKey(Tools.RegisterUniqueAddress(ope.AddressJumpInt + SADDef.EecBankStartAddress)))
                                {
                                    if (((S6xRegister)S6x.slRegisters[Tools.RegisterUniqueAddress(ope.AddressJumpInt + SADDef.EecBankStartAddress)]).MultipleMeanings)
                                    {
                                        sTranslation = ((S6xRegister)S6x.slRegisters[Tools.RegisterUniqueAddress(ope.AddressJumpInt + SADDef.EecBankStartAddress)]).Labels(false);
                                    }
                                    else
                                    {
                                        sTranslation = ((S6xRegister)S6x.slRegisters[Tools.RegisterUniqueAddress(ope.AddressJumpInt + SADDef.EecBankStartAddress)]).Label;
                                    }
                                }
                                else
                                {
                                    sTranslation = SADDef.CCMemory8061Template.Replace("%LREG%", ope.AddressJump);
                                }
                                
                                ope.TranslationReplacementAddress = ope.AddressJump;
                                ope.TranslationReplacementLabel = sTranslation;
                            }
                            else if (ope.AddressJumpInt + SADDef.EecBankStartAddress >= SADDef.ECMemory8061MinAdress && ope.AddressJumpInt + SADDef.EecBankStartAddress <= SADDef.ECMemory8061MaxAdress)
                            {
                                // Specific Case, S6x Registers are allowed to store CC/EC Memory Addresses for Translation
                                if (S6x.slRegisters.ContainsKey(Tools.RegisterUniqueAddress(ope.AddressJumpInt + SADDef.EecBankStartAddress)))
                                {
                                    if (((S6xRegister)S6x.slRegisters[Tools.RegisterUniqueAddress(ope.AddressJumpInt + SADDef.EecBankStartAddress)]).MultipleMeanings)
                                    {
                                        sTranslation = ((S6xRegister)S6x.slRegisters[Tools.RegisterUniqueAddress(ope.AddressJumpInt + SADDef.EecBankStartAddress)]).Labels(false);
                                    }
                                    else
                                    {
                                        sTranslation = ((S6xRegister)S6x.slRegisters[Tools.RegisterUniqueAddress(ope.AddressJumpInt + SADDef.EecBankStartAddress)]).Label;
                                    }
                                }
                                else
                                {
                                    sTranslation = SADDef.ECMemory8061Template.Replace("%LREG%", ope.AddressJump);
                                }

                                ope.TranslationReplacementAddress = ope.AddressJump;
                                ope.TranslationReplacementLabel = sTranslation;
                            }
                            break;
                    }
                }

                ope = null;
            }

            // Vector Lists
            foreach (string uniqueAddress in alVectorListsOPsUniqueAddresses)
            {
                ope = (Operation)slOPs[uniqueAddress];
                if (ope == null) continue;

                Vector vVect = (Vector)Calibration.slAdditionalVectors[Tools.UniqueAddress(ope.VectorListBankNum, ope.VectorListAddressInt)];
                if (vVect == null) continue;
                //opCode = (SADOPCode)Calibration.slOPCodes[ope.OriginalOPCode];
                //if (vVect.VectList == null) opCode.postProcessOpElemTranslation(ref ope, vVect.SourceAddress, vVect.ShortLabel);
                //else if (vVect.UniqueSourceAddress == vVect.VectList.UniqueAddress) opCode.postProcessOpElemTranslation(ref ope, vVect.SourceAddress, vVect.VectList.ShortLabel);
                //else opCode.postProcessOpElemTranslation(ref ope, vVect.SourceAddress, vVect.ShortLabel);
                //opCode = null;
                sTranslation = vVect.ShortLabel;
                if (vVect.VectList != null) if (vVect.UniqueSourceAddress == vVect.VectList.UniqueAddress) sTranslation = vVect.VectList.ShortLabel;
                ope.TranslationReplacementAddress = vVect.SourceAddress;
                ope.TranslationReplacementLabel = sTranslation;
            }
        }

        public void readUnknownOpElements(ref SADS6x S6x)
        {
            SortedList slIgnoredRanges = null;
            int[] ignoredRange = null;
            int[] prevIgnoredRange = null;
            int ignoredRangeIndex = -1;
            int iAddress = -1;
            int iSize = -1;
            string[] initialValues = null;
            int iOtherAddress = -1;

            slIgnoredRanges = new SortedList();
            // Reserved Addresses
            foreach (ReservedAddress resAdr in slReserved.Values)
            {
                int startAddress = resAdr.AddressInt;
                int endAddress = resAdr.AddressEndInt;
                if (slIgnoredRanges.ContainsKey(startAddress))
                {
                    if (((int[])slIgnoredRanges[startAddress])[1] < endAddress) slIgnoredRanges[startAddress] = new int[] { startAddress, endAddress };
                }
                else
                {
                    slIgnoredRanges.Add(startAddress, new int[] { startAddress, endAddress });
                }
            }

            // RBases Addresses
            if (Num == Calibration.BankNum)
            {
                foreach (RBase rBase in Calibration.slRbases.Values)
                {
                    int startAddress = rBase.AddressBankInt;
                    int endAddress = rBase.AddressBankEndInt;
                    if (slIgnoredRanges.ContainsKey(startAddress))
                    {
                        if (((int[])slIgnoredRanges[startAddress])[1] < endAddress) slIgnoredRanges[startAddress] = new int[] { startAddress, endAddress };
                    }
                    else
                    {
                        slIgnoredRanges.Add(startAddress, new int[] { startAddress, endAddress });
                    }
                }
            }

            // Additional Vectors Source Addresses
            foreach (Vector vector in Calibration.slAdditionalVectors.Values)
            {
                if (vector.SourceBankNum == Num)
                {
                    int startAddress = vector.SourceAddressInt;
                    int endAddress = vector.SourceAddressInt + 1;
                    if (slIgnoredRanges.ContainsKey(startAddress))
                    {
                        if (((int[])slIgnoredRanges[startAddress])[1] < endAddress) slIgnoredRanges[startAddress] = new int[] { startAddress, endAddress };
                    }
                    else
                    {
                        slIgnoredRanges.Add(startAddress, new int[] { startAddress, endAddress });
                    }
                }
            }

            // Call Addresses
            foreach (Call cCall in Calibration.slCalls.Values)
            {
                if (cCall.BankNum == Num && cCall.AddressEndInt >= cCall.AddressInt)
                {
                    int startAddress = cCall.AddressInt;
                    int endAddress = cCall.AddressEndInt;
                    if (slOPs.ContainsKey(Tools.UniqueAddress(cCall.BankNum, cCall.AddressEndInt)))
                    {
                        endAddress = ((Operation)slOPs[Tools.UniqueAddress(cCall.BankNum, cCall.AddressEndInt)]).AddressNextInt - 1;
                    }
                    if (slIgnoredRanges.ContainsKey(startAddress))
                    {
                        if (((int[])slIgnoredRanges[startAddress])[1] < endAddress) slIgnoredRanges[startAddress] = new int[] { startAddress, endAddress };
                    }
                    else
                    {
                        slIgnoredRanges.Add(startAddress, new int[] { startAddress, endAddress });
                    }
                }
            }

            // Non Calibration Elements Tables
            foreach (Table extObject in Calibration.slExtTables.Values)
            {
                if (extObject.BankNum == Num)
                {
                    int startAddress = extObject.AddressInt;
                    int endAddress = extObject.AddressEndInt;
                    if (slIgnoredRanges.ContainsKey(startAddress))
                    {
                        if (((int[])slIgnoredRanges[startAddress])[1] < endAddress) slIgnoredRanges[startAddress] = new int[] { startAddress, endAddress };
                    }
                    else
                    {
                        slIgnoredRanges.Add(startAddress, new int[] { startAddress, endAddress });
                    }
                }
            }

            // Non Calibration Elements Functions
            foreach (Function extObject in Calibration.slExtFunctions.Values)
            {
                if (extObject.BankNum == Num)
                {
                    int startAddress = extObject.AddressInt;
                    int endAddress = extObject.AddressEndInt;
                    if (slIgnoredRanges.ContainsKey(startAddress))
                    {
                        if (((int[])slIgnoredRanges[startAddress])[1] < endAddress) slIgnoredRanges[startAddress] = new int[] { startAddress, endAddress };
                    }
                    else
                    {
                        slIgnoredRanges.Add(startAddress, new int[] { startAddress, endAddress });
                    }
                }
            }

            // Non Calibration Elements Scalars
            foreach (Scalar extObject in Calibration.slExtScalars.Values)
            {
                if (extObject.BankNum == Num)
                {
                    int startAddress = extObject.AddressInt;
                    int endAddress = extObject.AddressEndInt;
                    if (slIgnoredRanges.ContainsKey(startAddress))
                    {
                        if (((int[])slIgnoredRanges[startAddress])[1] < endAddress) slIgnoredRanges[startAddress] = new int[] { startAddress, endAddress };
                    }
                    else
                    {
                        slIgnoredRanges.Add(startAddress, new int[] { startAddress, endAddress });
                    }
                }
            }

            // Structures
            foreach (Structure sStruct in Calibration.slExtStructures.Values)
            {
                // Vectors Lists are ignored - They have no output only Vectors have an output
                if (sStruct.isVectorsList) continue;
                // Same thing for Sub Structures
                if (sStruct.ParentStructure != null) continue;

                if (sStruct.BankNum == Num)
                {
                    int startAddress = sStruct.AddressInt;
                    int endAddress = sStruct.AddressEndInt;
                    if (slIgnoredRanges.ContainsKey(startAddress))
                    {
                        if (((int[])slIgnoredRanges[startAddress])[1] < endAddress) slIgnoredRanges[startAddress] = new int[] { startAddress, endAddress };
                    }
                    else
                    {
                        slIgnoredRanges.Add(startAddress, new int[] { startAddress, endAddress });
                    }
                }
            }

            // Ignored Range Clean Up
            ignoredRange = null;
            prevIgnoredRange = null;
            ignoredRangeIndex = 0;
            while (ignoredRangeIndex < slIgnoredRanges.Count)
            {
                ignoredRange = (int[])slIgnoredRanges.GetByIndex(ignoredRangeIndex);
                if (prevIgnoredRange == null)
                {
                    prevIgnoredRange = ignoredRange;
                    ignoredRangeIndex++;
                }
                else
                {
                    if (prevIgnoredRange[1] >= ignoredRange[1])
                    {
                        slIgnoredRanges.RemoveAt(ignoredRangeIndex);
                    }
                    else
                    {
                        prevIgnoredRange = ignoredRange;
                        ignoredRangeIndex++;
                    }
                }
            }
            ignoredRange = null;
            prevIgnoredRange = null;
            ignoredRangeIndex = 0;

            iAddress = AddressInternalInt;
            ignoredRangeIndex = 0;
            if (slIgnoredRanges.Count == 0) ignoredRange = null;
            else ignoredRange = (int[])slIgnoredRanges.GetByIndex(ignoredRangeIndex);
            while (ignoredRange != null)
            {
                if (iAddress < ignoredRange[0])
                {
                    // S6x Other Addresses
                    iOtherAddress = -1;
                    foreach (S6xOtherAddress oAddress in S6x.slProcessOtherAddresses.Values)
                    {
                        if (oAddress.BankNum == Num && oAddress.AddressInt >= iAddress && oAddress.AddressInt < ignoredRange[0])
                        {
                            iOtherAddress = oAddress.AddressInt;
                            break;
                        }
                    }

                    if (iOtherAddress >= 0)
                    {
                        if (iAddress == iOtherAddress)
                        // Nothing to do Other Address is the Unknown Part
                        {
                            // Nothing
                        }
                        else
                        // Other Address is inside the Unknown Part
                        {
                            iSize = iOtherAddress - iAddress;
                            initialValues = getBytesArray(iAddress, iSize);
                            //20180429
                            //slUnknownOpParts.Add(Tools.UniqueAddress(Num, iAddress), new UnknownCalibPart(Num, iAddress, iAddress + iSize - 1, initialValues));
                            slUnknownOpParts.Add(Tools.UniqueAddress(Num, iAddress), new UnknownOpPart(Num, iAddress, iAddress + iSize - 1, initialValues));
                            iAddress = iOtherAddress;
                        }
                    }
                    iSize = ignoredRange[0] - iAddress;
                    initialValues = getBytesArray(iAddress, iSize);
                    slUnknownOpParts.Add(Tools.UniqueAddress(Num, iAddress), new UnknownOpPart(Num, iAddress, iAddress + iSize - 1, initialValues));
                    iAddress = ignoredRange[1] + 1;
                }
                while (iAddress >= ignoredRange[0] && ignoredRangeIndex < slIgnoredRanges.Count - 1)
                {
                    iAddress = ignoredRange[1] + 1;
                    ignoredRangeIndex++;
                    ignoredRange = (int[])slIgnoredRanges.GetByIndex(ignoredRangeIndex);
                }
                if (iAddress >= ignoredRange[0] && ignoredRangeIndex == slIgnoredRanges.Count - 1)
                {
                    iAddress = ignoredRange[1] + 1;
                    ignoredRange = null;
                    break;
                }
            }
            if (iAddress <= AddressInternalEndInt)
            {
                // S6x Other Addresses
                iOtherAddress = -1;
                foreach (S6xOtherAddress oAddress in S6x.slProcessOtherAddresses.Values)
                {
                    if (oAddress.BankNum == Num && oAddress.AddressInt >= iAddress && oAddress.AddressInt <= AddressInternalEndInt)
                    {
                        iOtherAddress = oAddress.AddressInt;
                        break;
                    }
                }

                if (iOtherAddress >= 0)
                {
                    if (iAddress == iOtherAddress)
                    // Nothing to do Other Address is the Unknown Part
                    {
                        // Nothing
                    }
                    else
                    // Other Address is inside the Unknown Part
                    {
                        iSize = iOtherAddress - iAddress;
                        initialValues = getBytesArray(iAddress, iSize);
                        //20180429
                        //slUnknownOpParts.Add(Tools.UniqueAddress(Num, iAddress), new UnknownCalibPart(Num, iAddress, iAddress + iSize - 1, initialValues));
                        slUnknownOpParts.Add(Tools.UniqueAddress(Num, iAddress), new UnknownOpPart(Num, iAddress, iAddress + iSize - 1, initialValues));
                        iAddress = iOtherAddress;
                    }
                }
                iSize = AddressInternalEndInt - iAddress + 1;
                initialValues = getBytesArray(iAddress, iSize);
                slUnknownOpParts.Add(Tools.UniqueAddress(Num, iAddress), new UnknownOpPart(Num, iAddress, AddressInternalEndInt, initialValues));
            }
        }
        
        public Operation[] getCallOps(ref Call cCall, int maxNumber, int maxSubCallLevel, bool includeJumpsInResult, bool forwardJumpsOnly, bool followCalls, bool followShortCalls, bool followJumps, bool followShortJumps, bool followGotos, bool followSkips)
        {
            Operation[] arrOps = null;
            ArrayList alOps = null;
            Operation ope = null;
            bool followJump = false;
            int opAddress = 0;
            int subCallLevel = 0;
            
            alOps = new ArrayList();
            
            if (! slOPs.ContainsKey(cCall.UniqueAddress)) return new Operation[] {};

            ope = (Operation)slOPs[cCall.UniqueAddress];
//            while (ope.AddressInt < cCall.AddressEndInt && alOps.Count < maxNumber)
            while (alOps.Count < maxNumber)
            {
                followJump = (ope.AddressJumpInt >= 0);
                if (ope.CallType == CallType.Call) followJump &= followCalls;
                if (ope.CallType == CallType.ShortCall) followJump &= followShortCalls;
                if (ope.CallType == CallType.Jump) followJump &= followJumps;
                if (ope.CallType == CallType.ShortJump) followJump &= followShortJumps;
                if (ope.CallType == CallType.Goto) followJump &= followGotos;
                if (ope.CallType == CallType.Skip) followJump &= followSkips;
                // Forward Jumps Only does not apply for Call & Jumps
                switch (ope.CallType)
                {
                    case CallType.Call:
                    case CallType.Jump:
                    case CallType.ShortCall:
                    case CallType.ShortJump:
                        break;
                    default:
                        if (forwardJumpsOnly) followJump &= ope.AddressJumpInt > ope.AddressNextInt;
                        break;
                }
                // Sub Call Level Mngt
                switch (ope.CallType)
                {
                    case CallType.Call:
                    case CallType.ShortCall:
                        followJump &= (subCallLevel < maxSubCallLevel);
                        break;
                }
                if (followJump && ope.ApplyOnBankNum == Num)
                // Jumps only on current Bank
                {
                    if (includeJumpsInResult) alOps.Add(ope);

                    if (opAddress == ope.AddressJumpInt && opAddress == ope.AddressNextInt) break;
                    if (opAddress == ope.AddressJumpInt) opAddress = ope.AddressNextInt;
                    else opAddress = ope.AddressJumpInt;

                    // Sub Call Level Mngt
                    switch (ope.CallType)
                    {
                        case CallType.Call:
                        case CallType.ShortCall:
                            subCallLevel++;
                            break;
                    }
                }
                else
                {
                    opAddress = ope.AddressNextInt;
                    alOps.Add(ope);
                }

                if (ope.isReturn) break;

                if (!slOPs.ContainsKey(Tools.UniqueAddress(Num, opAddress))) break;
                ope = (Operation)slOPs[Tools.UniqueAddress(Num, opAddress)];
            }

            arrOps = (Operation[])alOps.ToArray(typeof(Operation));
            alOps = null;
            return arrOps;
        }

        public Operation[] getFollowingOPs(int opStartAddress, int opsNumber, int maxSubCallLevel, bool includeJumpsInResult, bool forwardJumpsOnly, bool followCalls, bool followShortCalls, bool followJumps, bool followShortJumps, bool followGotos, bool followSkips)
        {
            int opAddress = 0;
            int subCallLevel = 0;
            bool followJump = false;

            Operation[] opsResult = new Operation[opsNumber];

            opAddress = opStartAddress;
            for (int iResult = 0; iResult < opsNumber; iResult++)
            {
                if (!slOPs.ContainsKey(Tools.UniqueAddress(Num, opAddress))) return opsResult;

                Operation ope = (Operation)slOPs[Tools.UniqueAddress(Num, opAddress)];
                followJump = (ope.AddressJumpInt >= 0);
                if (ope.CallType == CallType.Call) followJump &= followCalls;
                if (ope.CallType == CallType.ShortCall) followJump &= followShortCalls;
                if (ope.CallType == CallType.Jump) followJump &= followJumps;
                if (ope.CallType == CallType.ShortJump) followJump &= followShortJumps;
                if (ope.CallType == CallType.Goto) followJump &= followGotos;
                if (ope.CallType == CallType.Skip) followJump &= followSkips;
                // Forward Jumps Only does not apply for Call & Jumps
                switch (ope.CallType)
                {
                    case CallType.Call:
                    case CallType.Jump:
                    case CallType.ShortCall:
                    //case CallType.ShortJump:  2018-05-30 ShortJump is a simple goto when moving backward
                        break;
                    default:
                        if (forwardJumpsOnly) followJump &= ope.AddressJumpInt > ope.AddressNextInt;
                        break;
                }
                // Sub Call Level Mngt
                switch (ope.CallType)
                {
                    case CallType.Call:
                    case CallType.ShortCall:
                        followJump &= (subCallLevel < maxSubCallLevel);
                        break;
                }
                if (followJump && ope.ApplyOnBankNum == Num)
                // Jumps only on current Bank
                {
                    if (includeJumpsInResult) opsResult[iResult] = ope;
                    else iResult--;

                    if (opAddress == ope.AddressJumpInt && opAddress == ope.AddressNextInt) return opsResult;
                    if (opAddress == ope.AddressJumpInt) opAddress = ope.AddressNextInt;
                    else opAddress = ope.AddressJumpInt;

                    // Sub Call Level Mngt
                    switch (ope.CallType)
                    {
                        case CallType.Call:
                        case CallType.ShortCall:
                            subCallLevel++;
                            break;
                    }
                }
                else
                {
                    opsResult[iResult] = ope;
                    opAddress = ope.AddressNextInt;
                }
            }

            return opsResult;
        }
        
        // Parameters removed, because not used at all
        /*
        public Operation[] getPrecedingOPs(int opStartAddress, int opsNumber, int maxSubCallLevel, bool includeJumpsInResult, bool backwardJumpsOnly, bool followCalls, bool followShortCalls, bool followJumps, bool followShortJumps, bool followGotos, bool followSkips)
        {
            ///// TO BE REVIEWED PROPERLY
            
            
            int opAddress = 0;
            //int subCallLevel = 0;
            //bool followJump = false;

            Operation[] opsResult = new Operation[opsNumber];

            opAddress = opStartAddress;
            for (int iResult = 0; iResult < opsNumber; iResult++)
            {
                if (!slOPs.ContainsKey(Tools.UniqueAddress(Num, opAddress))) return opsResult;

                Operation ope = (Operation)slOPs[Tools.UniqueAddress(Num, opAddress)];
                opsResult[iResult] = ope;
                if (! slOPsOrigins.ContainsKey(opAddress)) return opsResult;
                SortedList slOrigins = (SortedList)slOPsOrigins[opAddress];
                if (slOrigins.Count == 0) return opsResult;
                
                // TEMPORARY
                //opAddress = Convert.ToInt32(slOrigins.GetKey(0));

                // Still TEMPORARY - Preferences have to be taken into account
                opAddress = -1;
                int iDistance = Int32.MaxValue;
                foreach (Operation opeOri in slOrigins.Values)
                {
                    if (opeOri.BankNum != Num) continue;
                    if (opStartAddress >= opAddress) 
                    {
                        if (opStartAddress - opeOri.AddressInt < iDistance)
                        {
                            opAddress = opeOri.AddressInt;
                            iDistance = opStartAddress - opAddress;
                        }
                    }
                    else
                    {
                        if (opeOri.AddressInt - opStartAddress < iDistance)
                        {
                            opAddress = opeOri.AddressInt;
                            iDistance = opAddress - opStartAddress;
                        }
                    }
                }
                if (opAddress <= 0) return opsResult;

                /*
                followJump = (ope.AddressJumpInt >= 0); // !!!!!
                if (backwardJumpsOnly) followJump &= ope.AddressJumpInt > ope.AddressNextInt; // !!!!!
                if (ope.isCall) followJump &= followCalls; // !!!!!
                if (ope.isShortCall) followJump &= followShortCalls; // !!!!!
                if (ope.isJump) followJump &= followJumps; // !!!!!
                if (ope.isShortJump) followJump &= followShortJumps; // !!!!!
                if (ope.isGoto) followJump &= followGotos; // !!!!!
                if (ope.isSkip) followJump &= followSkips; // !!!!!
                // Forward Jumps Only does not apply for Call & Jumps
                switch (ope.CallType)
                {
                    case CallType.Call:
                    case CallType.Jump:
                    case CallType.ShortCall:
                    case CallType.ShortJump:
                        break;
                    default:
                        if (forwardJumpsOnly) followJump &= ope.AddressJumpInt > ope.AddressNextInt;
                        break;
                }
                // Sub Call Level Mngt
                switch (ope.CallType)
                {
                    case CallType.Call:
                    case CallType.ShortCall:
                        followJump &= (subCallLevel < maxSubCallLevel);
                        break;
                }
                if (followJump && ope.ApplyOnBankNum == Num)
                // Jumps only on current Bank
                {
                    if (includeJumpsInResult) opsResult[iResult] = ope;
                    else iResult--;

                    if (opAddress == ope.AddressJumpInt && opAddress == ope.AddressNextInt) return opsResult;
                    if (opAddress == ope.AddressJumpInt) opAddress = ope.AddressNextInt;
                    else opAddress = ope.AddressJumpInt;

                    // Sub Call Level Mngt
                    switch (ope.CallType)
                    {
                        case CallType.Call:
                        case CallType.ShortCall:
                            subCallLevel++;
                            break;
                    }
                }
                else
                {
                    opsResult[iResult] = ope;
                    opAddress = ope.AddressNextInt;
                }
                
            }

            return opsResult;
        }
        */

        // Classic version, with really used parameters and variable ops number - Still not reviewed fully
        //  TO BE REVIEWED PROPERLY
        public Operation[] getPrecedingOPs(int opStartAddress, int opsNumberCurrentLevel, int maxSubCallLevel)
        {
            ArrayList alOps = new ArrayList();
            int opAddress = opStartAddress;
            int iOpsNumberCurrentLevel = 0;

            while (true)
            {
                Operation ope = (Operation)slOPs[Tools.UniqueAddress(Num, opAddress)];
                if (ope == null) break;

                alOps.Add(ope);
                if (iOpsNumberCurrentLevel >= opsNumberCurrentLevel) break;
                iOpsNumberCurrentLevel++;
                     
                // Including interesting Call Operations, except if first address is a call, which is the case in some searches
                if (ope.AddressInt != opStartAddress && ope.ApplyOnBankNum == Num && (ope.CallType == CallType.Call || ope.CallType == CallType.ShortCall))
                {
                    alOps.AddRange(getPrecedingCallOPs(ope.AddressJumpInt, maxSubCallLevel - 1));
                }

                SortedList slOrigins = (SortedList)slOPsOrigins[opAddress];
                if (slOrigins == null) break;
                if (slOrigins.Count == 0) break;

                opAddress = -1;
                int iDistance = Int32.MaxValue;
                foreach (Operation opeOri in slOrigins.Values)
                {
                    if (opeOri.BankNum != Num) continue;
                    if (opStartAddress >= opAddress)
                    {
                        if (opStartAddress - opeOri.AddressInt < iDistance)
                        {
                            opAddress = opeOri.AddressInt;
                            iDistance = opStartAddress - opAddress;
                        }
                    }
                    else
                    {
                        if (opeOri.AddressInt - opStartAddress < iDistance)
                        {
                            opAddress = opeOri.AddressInt;
                            iDistance = opAddress - opStartAddress;
                        }
                    }
                }
                if (opAddress <= 0) break;
            }

            return (Operation[])alOps.ToArray(typeof(Operation));
        }

        // Return Call and SubCall Operations in reverse order
        //      For real calls ending with return
        public Operation[] getPrecedingCallOPs(int iAddressCall, int maxSubCallLevel)
        {
            if (maxSubCallLevel < 0) return new Operation[] { };

            Call cCall = (Call)Calibration.slCalls[Tools.UniqueAddress(Num, iAddressCall)];
            if (cCall == null) return new Operation[] { };

            Operation[] callOps = getCallOps(ref cCall, 0xffff, 0, true, false, false, false, false, false, true, true);
            if (callOps == null) return new Operation[] { };
            if (callOps.Length == 0) return new Operation[] { };
            if (callOps[callOps.Length - 1] == null) return new Operation[] { };
            // Only Calls which end with a return are interesting
            if (!callOps[callOps.Length - 1].isReturn) return new Operation[] { };

            ArrayList alOps = new ArrayList();
            // Return instruction is not included
            for (int iCallOpe = callOps.Length - 2; iCallOpe >= 0; iCallOpe--)
            {
                if (callOps[iCallOpe] == null) continue;
                if (callOps[iCallOpe].ApplyOnBankNum == Num && (callOps[iCallOpe].CallType == CallType.Call || callOps[iCallOpe].CallType == CallType.ShortCall))
                {
                    alOps.AddRange(getPrecedingCallOPs(callOps[iCallOpe].AddressJumpInt, maxSubCallLevel - 1));
                }
                alOps.Add(callOps[iCallOpe]);
            }
            return (Operation[])alOps.ToArray(typeof(Operation));
        }
        
        // For Test Purpose only
        public string getOperationsArrayOutput(ref Operation[] arrOps)
        {
            if (arrOps == null) return string.Empty;

            string sResult = string.Empty;
            foreach (Operation ope in arrOps)
            {
                if (ope == null) break;
                sResult += string.Format("{0,6}: {1,-21}{2,-25}{3,-21}\r\n", ope.UniqueAddressHex, ope.OriginalOp, ope.Instruction, ope.Translation1);
                if (ope.CallArgsNum > 0) sResult += string.Format("{0,6}: {1,-46}{2,-21}\r\n", ope.UniqueCallArgsAddressHex, ope.CallArgs, "#args");
            }
            return sResult;
        }

        // Returns a flat list of operations
        //      Returns and Jumps ends tree branches
        //      opsNumber is the size of the branch depth
        //      Skip operations are ignored
        //      Calls and ShortCalls are added in execution order
        //      Conditional Gotos are added based on distance priority (Short or not)
        public Operation[] getFollowingOpsTree(int opStartAddress, int opsNumber, bool trueConditionalGotoPriority)
        {
            ArrayList alIncludedOpsAddresses = new ArrayList();
            OperationNode[] opsTree = getFollowingOpsTree(opStartAddress, opsNumber, ref alIncludedOpsAddresses);
            alIncludedOpsAddresses = null;

            ArrayList alOps = new ArrayList();
            loadOperationsTree(ref opsTree, ref alOps, trueConditionalGotoPriority);
            
            // Debug Only
            //Operation[] arrOps = (Operation[])alOps.ToArray(typeof(Operation));
            //string Debug = getOperationsArrayOutput(ref arrOps);
            
            opsTree = null;

            return (Operation[])alOps.ToArray(typeof(Operation));
        }

        private void loadOperationsTree(ref OperationNode[] opsTree, ref ArrayList alOperations, bool trueConditionalGotoPriority)
        {
            if (opsTree == null) return;

            for (int iNode = 0; iNode < opsTree.Length; iNode++)
            {
                OperationNode opeNode = opsTree[iNode];

                alOperations.Add(opeNode.Operation);
                if (opeNode.Branch == null) continue;

                OperationNode[] opsBranch = opeNode.Branch;

                if (opeNode.Operation.CallType == CallType.Goto && !trueConditionalGotoPriority)
                // Conditional Goto with false Conditional Goto Priority
                {
                    if (opsTree.Length - iNode - 1 > 0)
                    {
                        OperationNode[] opsSubTree = new OperationNode[opsTree.Length - iNode - 1];
                        for (int iSubNode = 0; iSubNode + iNode + 1 < opsTree.Length; iSubNode++) opsSubTree[iSubNode] = opsTree[iSubNode + iNode + 1];
                        loadOperationsTree(ref opsSubTree, ref alOperations, trueConditionalGotoPriority);
                        opsSubTree = null;
                    }
                    loadOperationsTree(ref opsBranch, ref alOperations, trueConditionalGotoPriority);
                    return;
                }

                loadOperationsTree(ref opsBranch, ref alOperations, trueConditionalGotoPriority);
            }
        }

        private OperationNode[] getFollowingOpsTree(int opAddress, int opsNumber, ref ArrayList alIncludedOpsAddresses)
        {
            ArrayList alTree = new ArrayList();

            OperationNode[] opsTree = new OperationNode[opsNumber];

            for (int iResult = 0; iResult < opsNumber; iResult++)
            {
                Operation ope = (Operation)slOPs[Tools.UniqueAddress(Num, opAddress)];
                if (ope == null) break;

                alIncludedOpsAddresses.Add(ope.AddressInt);

                OperationNode opNode = new OperationNode(ref ope);

                if (ope.CallType != CallType.Skip) alTree.Add(opNode);

                // Return gives the end
                if (ope.isReturn) break;
                // Jump gives the end too
                if (ope.CallType == CallType.Jump) break;

                if (ope.AddressJumpInt >= 0 && ope.AddressJumpInt != opAddress && ope.AddressJumpInt != ope.AddressNextInt && ope.CallType != CallType.Skip && ope.ApplyOnBankNum == Num)
                // Valid Jump and only on current Bank
                {

                    if (opsNumber - iResult - 1 > 0 && !alIncludedOpsAddresses.Contains(ope.AddressJumpInt)) opNode.Branch = getFollowingOpsTree(ope.AddressJumpInt, opsNumber - iResult - 1, ref alIncludedOpsAddresses);
                }

                // Preventing loop
                if (opAddress == ope.AddressNextInt) break;

                // Managing Duplicates in Tree
                if (alIncludedOpsAddresses.Contains(ope.AddressNextInt)) break;

                opAddress = ope.AddressNextInt;
            }

            if (alTree.Count == 0) return null;
            else return (OperationNode[])alTree.ToArray(typeof(OperationNode));
        }

        public Call getPrecedingMainCall(int opeAddress)
        {
            //Operation[] adjacentOps = getPrecedingOPs(opeAddress, 32, 99, true, true, false, false, false, false, false, false);
            Operation[] adjacentOps = getPrecedingOPs(opeAddress, 32, 0);
            foreach (Operation ope in adjacentOps)
            {
                if (ope == null) return null;
                if (ope.AddressInt == ope.InitialCallAddressInt)
                // Ope is first Ope in a Call, but could be a Goto or Skip, searching for main Call
                {
                    Call opeCall = (Call)Calibration.slCalls[ope.UniqueAddress];
                    if (opeCall != null)
                    {
                        switch (opeCall.CallType)
                        {
                            case CallType.ShortJump:
                            case CallType.Goto:
                            case CallType.Skip:
                                // Continue
                                break;
                            default:
                                // Found
                                return opeCall;
                        }
                        opeCall = null;
                    }
                }
            }
            adjacentOps = null;
            return null;
        }

        public Operation[] getElementRelatedOps(string elemUniqueAddress)
        {
            Operation ope = null;
            Operation[] prevOps = null;
            Operation[] postOps = null;
            Operation[] opsResult = null;

            if (ope == null)
            {
                foreach (string opeUniqueAddress in alCalibElemOPsUniqueAddresses)
                {
                    ope = (Operation)slOPs[opeUniqueAddress];
                    if (ope.alCalibrationElems == null) continue;
                    bool foundElem = false;
                    foreach (CalibrationElement opeCalElem in ope.alCalibrationElems)
                    {
                        if (opeCalElem.UniqueAddress == elemUniqueAddress)
                        {
                            foundElem = true;
                            break;
                        }
                    }
                    if (foundElem) break;
                    ope = null;
                }
            }
            if (ope == null)
            {
                foreach (string opeUniqueAddress in alPossibleKnownElemOPsUniqueAddresses)
                {
                    ope = (Operation)slOPs[opeUniqueAddress];
                    if (ope.KnownElemAddress != string.Empty)
                    {
                        if (Tools.UniqueAddress(ope.ReadDataBankNum, Convert.ToInt32(ope.KnownElemAddress, 16) - SADDef.EecBankStartAddress) == elemUniqueAddress) break;
                        if (Tools.UniqueAddress(ope.ApplyOnBankNum, Convert.ToInt32(ope.KnownElemAddress, 16) - SADDef.EecBankStartAddress) == elemUniqueAddress) break;
                    }
                    ope = null;
                }
            }
            if (ope == null)
            {
                foreach (string opeUniqueAddress in alPossibleOtherElemOPsUniqueAddresses)
                {
                    ope = (Operation)slOPs[opeUniqueAddress];
                    if (ope.OtherElemAddress != string.Empty)
                    {
                        if (Tools.UniqueAddress(ope.ReadDataBankNum, Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress) == elemUniqueAddress) break;
                        if (Tools.UniqueAddress(ope.ApplyOnBankNum, Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress) == elemUniqueAddress) break;
                    }
                    ope = null;
                }
            }

            if (ope == null) return null;

            //prevOps = getPrecedingOPs(ope.AddressInt, 16, 99, true, true, false, false, false, false, false, false);
            prevOps = getPrecedingOPs(ope.AddressInt, 16, 0);
            postOps = getFollowingOPs(ope.AddressInt, 16, 99, true, true, false, false, false, false, false, false);

            ope = null;

            opsResult = new Operation[prevOps.Length + postOps.Length];
            for (int iPos = prevOps.Length - 1; iPos >= 0; iPos--) opsResult[prevOps.Length - 1 - iPos] = prevOps[iPos];
            for (int iPos = 0; iPos < postOps.Length; iPos++) opsResult[iPos + prevOps.Length - 1] = postOps[iPos];

            prevOps = null;
            postOps = null;

            return opsResult;
        }

        // Used before processing new call to check that address is valid and to prevent creating a bad call
        // Used for Structures Check too
        //      Not inside reserved adresses
        //      Not inside Rbases
        //      Not inside another Operation
        public bool isJumpAddressInConflict(int jumpAddress, int existingOpAddress)
        {
            Operation ope = null;
            int opeIndex = -1;

            // Jump outside Bank Addresses => KO
            if (jumpAddress < AddressInternalInt || jumpAddress > AddressInternalEndInt) return true;

            // Jump to the same Address => OK
            if (existingOpAddress == jumpAddress) return false;

            // Jump to a recognized Address => OK
            if (slOPs.ContainsKey(Tools.UniqueAddress(Num, jumpAddress))) return false;

            // Jump to Reserved Addresses part => KO
            if (jumpAddress >= minResAdr && jumpAddress <= maxResAdr) return true;

            if (Num == Calibration.BankNum)
            {
                // Jump to Rbase part => KO
                foreach (RBase rBase in Calibration.slRbases.Values)
                {
                    if (jumpAddress >= rBase.AddressBankInt && jumpAddress <= rBase.AddressBankEndInt) return true;
                }
                // Jump to Additional Vector Source Address => KO
                foreach (Vector addVect in Calibration.slAdditionalVectors.Values)
                {
                    if (jumpAddress >= addVect.SourceAddressInt && jumpAddress <= addVect.SourceAddressInt + 1) return true;
                }
            }

            // Jump Inside another Operation => KO

            if (existingOpAddress == -1)
            // Used for Calls from another Bank
            {
                if (slOPs.Count == 0) return false;
                opeIndex = (slOPs.Count - 1) / 2;
                existingOpAddress = ((Operation)slOPs.GetByIndex(opeIndex)).AddressInt;
            }
            else
            {
                if (!slOPs.ContainsKey(Tools.UniqueAddress(Num, existingOpAddress))) return true;
                opeIndex = slOPs.IndexOfKey(Tools.UniqueAddress(Num, existingOpAddress));
            }
            if (jumpAddress > existingOpAddress)
            {
                opeIndex++;
                while (opeIndex < slOPs.Count)
                {
                    ope = (Operation)slOPs.GetByIndex(opeIndex);
                    if (jumpAddress > ope.AddressInt && jumpAddress < ope.AddressNextInt)
                    {
                        ope = null;
                        return true;
                    }
                    if (jumpAddress < ope.AddressNextInt) break;
                    opeIndex++;
                }
                ope = null;
                return false;
            }
            else
            {
                opeIndex--;
                while (opeIndex >= 0)
                {
                    ope = (Operation)slOPs.GetByIndex(opeIndex);
                    if (jumpAddress > ope.AddressInt && jumpAddress < ope.AddressNextInt)
                    {
                        ope = null;
                        return true;
                    }
                    if (jumpAddress > ope.AddressInt) break;
                    opeIndex--;
                }
                ope = null;
                return false;
            }
        }

        // Used before processing new structure to check that address is valid and to prevent creating a conflict with an existing element
        //      Not inside ExtScalar
        //      Not inside ExtFunction
        //      Not inside ExtTable
        //      Not inside ExtStructure
        public bool isOtherElemAddressInConflict(int otherElemAddress, int initialOtherElemAddress, Type targetType)
        {
            // Elem outside Bank Addresses => KO
            if (otherElemAddress < AddressInternalInt || otherElemAddress > AddressInternalEndInt) return true;

            // Elem to a recognized Op Address => KO
            if (slOPs.ContainsKey(Tools.UniqueAddress(Num, otherElemAddress))) return true;

            // Elem to Reserved Addresses part => KO
            if (otherElemAddress >= minResAdr && otherElemAddress <= maxResAdr) return true;

            if (Num == Calibration.BankNum)
            {
                // Elem to Rbase part => KO
                foreach (RBase rBase in Calibration.slRbases.Values)
                {
                    if (otherElemAddress >= rBase.AddressBankInt && otherElemAddress <= rBase.AddressBankEndInt) return true;
                }
                // Elem to Additional Vector Source Address => KO
                foreach (Vector addVect in Calibration.slAdditionalVectors.Values)
                {
                    if (otherElemAddress >= addVect.SourceAddressInt && otherElemAddress <= addVect.SourceAddressInt + 1) return true;
                }
            }

            // Elem Inside another Ext Scalar => KO
            if (isOtherElemAddressInsideExtScalar(otherElemAddress, initialOtherElemAddress, targetType)) return true;

            // Elem Inside another Ext Function => KO
            if (isOtherElemAddressInsideExtFunction(otherElemAddress, initialOtherElemAddress, targetType)) return true;

            // Elem Inside another Ext Table => KO
            if (isOtherElemAddressInsideExtTable(otherElemAddress, initialOtherElemAddress, targetType)) return true;

            // Elem Inside another Ext Structure => KO
            if (isOtherElemAddressInsideExtStructure(otherElemAddress, initialOtherElemAddress, targetType)) return true;

            // Elem Inside another Operation => KO
            if (isOtherElemAddressInsideOperation(otherElemAddress)) return true;

            return false;
        }

        // Elem Inside another Ext Scalar
        public bool isOtherElemAddressInsideExtScalar(int otherElemAddress, int initialOtherElemAddress, Type targetType)
        {
            int iIndex = -1;
            int iStep = 0;
            bool bLoopCond = false;

            if (Calibration.slExtScalars.Count > 0)
            {
                if (Calibration.slExtScalars.ContainsKey(Tools.UniqueAddress(Num, otherElemAddress)))
                {
                    if (otherElemAddress != initialOtherElemAddress || typeof(Scalar) != targetType) return true;
                }
                iIndex = (Calibration.slExtScalars.Count - 1) / 2;
                iStep = (otherElemAddress > ((Scalar)Calibration.slExtScalars.GetByIndex(iIndex)).AddressInt) ? 1 : -1;
                iIndex += iStep;
                bLoopCond = (iStep == 1) ? iIndex < Calibration.slExtScalars.Count : iIndex >= 0;
                while (bLoopCond)
                {
                    Scalar oOther = (Scalar)Calibration.slExtScalars.GetByIndex(iIndex);
                    if (otherElemAddress >= oOther.AddressInt && otherElemAddress <= oOther.AddressEndInt) return true;
                    if (iStep == 1 && otherElemAddress < oOther.AddressEndInt) break;
                    else if (iStep == -1 && otherElemAddress > oOther.AddressInt) break;

                    iIndex += iStep;
                    bLoopCond = (iStep == 1) ? iIndex < Calibration.slExtScalars.Count : iIndex >= 0;
                }
            }

            return false;
        }
        
        // Elem Inside another Ext Function
        public bool isOtherElemAddressInsideExtFunction(int otherElemAddress, int initialOtherElemAddress, Type targetType)
        {
            int iIndex = -1;
            int iStep = 0;
            bool bLoopCond = false;

            if (Calibration.slExtFunctions.Count > 0)
            {
                if (Calibration.slExtFunctions.ContainsKey(Tools.UniqueAddress(Num, otherElemAddress)))
                {
                    if (otherElemAddress != initialOtherElemAddress || typeof(Function) != targetType) return true;
                }
                iIndex = (Calibration.slExtFunctions.Count - 1) / 2;
                iStep = (otherElemAddress > ((Function)Calibration.slExtFunctions.GetByIndex(iIndex)).AddressInt) ? 1 : -1;
                iIndex += iStep;
                bLoopCond = (iStep == 1) ? iIndex < Calibration.slExtFunctions.Count : iIndex >= 0;
                while (bLoopCond)
                {
                    Function oOther = (Function)Calibration.slExtFunctions.GetByIndex(iIndex);
                    if (otherElemAddress >= oOther.AddressInt && otherElemAddress <= oOther.AddressEndInt) return true;
                    if (iStep == 1 && otherElemAddress < oOther.AddressEndInt) break;
                    else if (iStep == -1 && otherElemAddress > oOther.AddressInt) break;

                    iIndex += iStep;
                    bLoopCond = (iStep == 1) ? iIndex < Calibration.slExtFunctions.Count : iIndex >= 0;
                }
            }

            return false;
        }

        // Elem Inside another Ext Table
        public bool isOtherElemAddressInsideExtTable(int otherElemAddress, int initialOtherElemAddress, Type targetType)
        {
            int iIndex = -1;
            int iStep = 0;
            bool bLoopCond = false;

            if (Calibration.slExtTables.Count > 0)
            {
                if (Calibration.slExtTables.ContainsKey(Tools.UniqueAddress(Num, otherElemAddress)))
                {
                    if (otherElemAddress != initialOtherElemAddress || typeof(Table) != targetType) return true;
                }
                iIndex = (Calibration.slExtTables.Count - 1) / 2;
                iStep = (otherElemAddress > ((Table)Calibration.slExtTables.GetByIndex(iIndex)).AddressInt) ? 1 : -1;
                iIndex += iStep;
                bLoopCond = (iStep == 1) ? iIndex < Calibration.slExtTables.Count : iIndex >= 0;
                while (bLoopCond)
                {
                    Table oOther = (Table)Calibration.slExtTables.GetByIndex(iIndex);
                    if (otherElemAddress >= oOther.AddressInt && otherElemAddress <= oOther.AddressEndInt) return true;
                    if (iStep == 1 && otherElemAddress < oOther.AddressEndInt) break;
                    else if (iStep == -1 && otherElemAddress > oOther.AddressInt) break;

                    iIndex += iStep;
                    bLoopCond = (iStep == 1) ? iIndex < Calibration.slExtTables.Count : iIndex >= 0;
                }
            }

            return false;
        }

        // Elem Inside another Ext Structure
        public bool isOtherElemAddressInsideExtStructure(int otherElemAddress, int initialOtherElemAddress, Type targetType)
        {
            int iIndex = -1;
            int iStep = 0;
            bool bLoopCond = false;

            if (Calibration.slExtStructures.Count > 0)
            {
                if (Calibration.slExtStructures.ContainsKey(Tools.UniqueAddress(Num, otherElemAddress)))
                {
                    if (otherElemAddress != initialOtherElemAddress || typeof(Structure) != targetType) return true;
                }
                iIndex = (Calibration.slExtStructures.Count - 1) / 2;
                iStep = (otherElemAddress > ((Structure)Calibration.slExtStructures.GetByIndex(iIndex)).AddressInt) ? 1 : -1;
                iIndex += iStep;
                bLoopCond = (iStep == 1) ? iIndex < Calibration.slExtStructures.Count : iIndex >= 0;
                while (bLoopCond)
                {
                    Structure oOther = (Structure)Calibration.slExtStructures.GetByIndex(iIndex);
                    int iAddressInt = oOther.AddressInt;
                    int iAddressEndInt = oOther.AddressEndInt;
                    // Structure not read
                    if (oOther.ParentStructure == null && oOther.ParentAddressInt != -1)
                    {
                        iAddressInt = oOther.ParentAddressInt;
                        if (iAddressEndInt < iAddressInt) iAddressEndInt = iAddressInt;
                    }
                    if (iAddressEndInt == iAddressInt && oOther.Number > 0 && oOther.MaxSizeSingle > 0 && oOther.ParentStructure == null)
                    {
                        iAddressEndInt = iAddressInt + oOther.Number * oOther.MaxSizeSingle - 1;
                    }
                    if (oOther.AddressInt != initialOtherElemAddress && otherElemAddress >= iAddressInt && otherElemAddress <= iAddressEndInt) return true;
                    //if (otherElemAddress >= oOther.AddressInt && otherElemAddress <= iAddressEndInt) return true;
                    if (iStep == 1 && otherElemAddress < iAddressEndInt) break;
                    else if (iStep == -1 && otherElemAddress > iAddressInt) break;

                    iIndex += iStep;
                    bLoopCond = (iStep == 1) ? iIndex < Calibration.slExtStructures.Count : iIndex >= 0;
                }
            }

            return false;
        }

        // Elem Inside another Operation
        public bool isOtherElemAddressInsideOperation(int otherElemAddress)
        {
            int iIndex = -1;
            int iStep = 0;
            bool bLoopCond = false;

            if (slOPs.Count > 0)
            {
                iIndex = (slOPs.Count - 1) / 2;
                if (((Operation)slOPs.GetByIndex(iIndex)).AddressInt == otherElemAddress) return true;
                iStep = (otherElemAddress > ((Operation)slOPs.GetByIndex(iIndex)).AddressInt) ? 1 : -1;
                iIndex += iStep;
                bLoopCond = (iStep == 1) ? iIndex < slOPs.Count : iIndex >= 0;
                while (bLoopCond)
                {
                    Operation ope = (Operation)slOPs.GetByIndex(iIndex);
                    if (otherElemAddress >= ope.AddressInt && otherElemAddress < ope.AddressNextInt) return true;
                    if (iStep == 1 && otherElemAddress < ope.AddressNextInt) break;
                    else if (iStep == -1 && otherElemAddress >= ope.AddressInt) break;

                    iIndex += iStep;
                    bLoopCond = (iStep == 1) ? iIndex < slOPs.Count : iIndex >= 0;
                }
            }

            return false;
        }

        public string getBytes(int startPos, int len) { return Tools.getBytes(startPos, len, ref arrBytes); }

        public string[] getBytesArray(int startPos, int len) { return Tools.getBytesArray(startPos, len, ref arrBytes); }

        public string getByte(int startPos) { return Tools.getByte(startPos, ref arrBytes); }

        public string getByte(string startPos) { return Tools.getByte(startPos, ref arrBytes); }

        public string getWord(int startPos, bool lsbFirst) { return Tools.getWord(startPos, lsbFirst, ref arrBytes); }

        public string getWord(string startPos, bool lsbFirst) { return Tools.getWord(startPos, lsbFirst, ref arrBytes); }

        public int getByteInt(int startPos, bool signed) { return Tools.getByteInt(startPos, signed, ref arrBytes); }

        public int getByteInt(string startPos, bool signed) { return Tools.getByteInt(startPos, signed, ref arrBytes); }

        public int getWordInt(int startPos, bool signed, bool lsbFirst) { return Tools.getWordInt(startPos, signed, lsbFirst, ref arrBytes); }

        public int getWordInt(string startPos, bool signed, bool lsbFirst) { return Tools.getWordInt(startPos, signed, lsbFirst, ref arrBytes); }

        public object[] getBytesFromSignature(string sSignature) { return Tools.getBytesFromSignature(sSignature, ref sBytes); }
    }
}
