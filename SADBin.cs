using System;
using System.Collections;
using System.Text;
using System.IO;

namespace SAD806x
{
    public class SADBin
    {
        public SADS6x S6x = null;

        public SADCalib Calibration = null;

        public SADBank Bank8 = null;
        public SADBank Bank1 = null;
        public SADBank Bank9 = null;
        public SADBank Bank0 = null;

        public bool isCorrupted = false;
        
        private string binaryFilePath = string.Empty;
        private string binaryFileName = string.Empty;
        private long binaryFileSize = -1;

        private string[] arrBytes = null;

        private bool is8061 = false;
        private bool isEarly = false;
        private bool isPilot = false;

        private bool loaded = false;
        private bool disassembled = false;
        
        private DateTime loadStartTime = DateTime.MinValue;
        private DateTime loadEndTime = DateTime.MinValue;
        private DateTime disassemblyStartTime = DateTime.MinValue;
        private DateTime disassemblyEndTime = DateTime.MinValue;
        private DateTime unknownElemsStartTime = DateTime.MinValue;
        private DateTime unknownElemsEndTime = DateTime.MinValue;
        private DateTime outputStartTime = DateTime.MinValue;
        private DateTime outputEndTime = DateTime.MinValue;

        public string BinaryFilePath { get { return binaryFilePath; } }
        public string BinaryFileName { get { return binaryFileName; } }
        public long BinaryFileSize { get { return binaryFileSize; } }

        public bool isValid
        {
            get
            {
                if (Bank8 == null) return false;
                if (is8061) return true;
                if (Bank1 == null) return false;
                if (Bank0 == null && Bank9 == null) return true;
                if (Bank0 != null && Bank9 != null) return true;
                return false;
            }
        }
        
        public int LoadTimeSeconds { get { return (int)(loadEndTime - loadStartTime).TotalSeconds; } }
        public int DisassemblyTimeSeconds { get { return (int)(disassemblyEndTime - disassemblyStartTime).TotalSeconds; } }
        public int UnknownElemsTimeSeconds { get { return (int)(unknownElemsEndTime - unknownElemsStartTime).TotalSeconds; } }
        public int OutputTimeSeconds { get { return (int)(outputEndTime - outputStartTime).TotalSeconds; } }

        public bool isLoaded { get { return loaded; } }
        public bool isDisassembled { get { return disassembled; } }

        public string[] getBinBytes { get { return (string[])arrBytes.Clone(); } }

        public int ProgressStatus = 0;
        public string ProgressLabel = string.Empty;

        public string[] Errors = null;

        public SADBin(string binFilePath, string s6xFilePath)
        {
            ProgressStatus = 0;
            ProgressLabel = string.Empty;

            loadStartTime = DateTime.Now;
            loadEndTime = DateTime.Now;

            if (!File.Exists(binFilePath)) return;

            binaryFilePath = binFilePath;
            binaryFileName = new FileInfo(binaryFilePath).Name;
            binaryFileSize = new FileInfo(binaryFilePath).Length;

            S6x = new SADS6x(s6xFilePath);

            Calibration = new SADCalib();

            readBin();
            readBanks();
            readCalibration();
            readChecksum();
            readVidBlock();

            // S6x Banks Properties Mngt
            SortedList slBanksInfos = null;
            try { slBanksInfos = Calibration.Info.slBanksInfos; }
            catch {slBanksInfos = null;}
            S6x.MatchBanksProperties(ref slBanksInfos);
            slBanksInfos = null;
            
            loaded = true;
            loadEndTime = DateTime.Now;

            if (isValid)
            {
                ProgressStatus = 100;
                ProgressLabel = "Bin Loaded.";
            }
            else
            {
                ProgressStatus = -1;
                ProgressLabel = "Invalid Bin.";
            }
        }

        private void readBin()
        {
            FileStream fsFS = null;
            long iSize = 0;
            int iByte;

            if (!File.Exists(binaryFilePath)) return;

            iSize = new FileInfo(binaryFilePath).Length;
            if (iSize > 256 * 1024)
            {
                arrBytes = new string[] { };
                return;
            }

            arrBytes = new string[iSize];

            fsFS = new FileStream(binaryFilePath, FileMode.Open);
            for (int iPos = 0; (iByte = fsFS.ReadByte()) != -1; iPos++) arrBytes[iPos] = string.Format("{0:x2}", iByte);
            fsFS.Close();
            fsFS.Dispose();
            fsFS = null;
        }

        private void readBanks()
        {
            // Search patterns
            string sPattern = string.Empty;
            string sPrevPattern = string.Empty;
            ArrayList validPrevPatternEnds = null;
            bool validPrevPattern = false;

            // List of Banks ordered by place in binary
            ArrayList banksInfos = new ArrayList();

            // Main Banks information
            int bankNum = -1;
            int bankStartAddress = -1;
            int bankEndAddress = -1;

            // Used at Bank identification
            int bank8StartAddress = -1;
            bool foundBank1 = false;
            bool foundBank0 = false;
            bool foundBank9 = false;

            // Used for identifying 8061 vs 8065
            string levNum = string.Empty;

            // Defective or Early 8065 searches
            int startPos = -1;
            int endPos = -1;

            validPrevPatternEnds = new ArrayList();
            for (int iPe = 0; iPe < SADDef.Bank_Prev_Pattern_Possible_Bytes.Length; iPe++)
            {
                string sPe = string.Empty;
                for (int iCt = 0; iCt < SADDef.Bank_Prev_Pattern_String_Length / 2; iCt++) sPe += SADDef.Bank_Prev_Pattern_Possible_Bytes[iPe];
                validPrevPatternEnds.Add(sPe.ToLower());
            }
            for (int iLb = 0; iLb < SADDef.Bank_Prev_Pattern_Possible_Fixed_Last_Bytes.Length; iLb++)
            {
                for (int iPe = 0; iPe < SADDef.Bank_Prev_Pattern_Possible_Bytes.Length; iPe++)
                {
                    string sPe = string.Empty;
                    for (int iCt = 0; iCt < (SADDef.Bank_Prev_Pattern_String_Length - SADDef.Bank_Prev_Pattern_Possible_Fixed_Last_Bytes[iLb].Length) / 2; iCt++) sPe += SADDef.Bank_Prev_Pattern_Possible_Bytes[iPe];
                    sPe += SADDef.Bank_Prev_Pattern_Possible_Fixed_Last_Bytes[iLb];
                    validPrevPatternEnds.Add(sPe.ToLower());
                }
            }

            sPrevPattern = string.Empty;
            for (int iPos = 0; iPos < arrBytes.Length; iPos+=16)
            {
                sPattern = getBytes(iPos, 20);
                if (sPattern.Length < 16) break;
                if (sPrevPattern == string.Empty)
                {
                    validPrevPattern = true;
                }
                else
                {
                    validPrevPattern = false;
                    foreach (string patEnd in validPrevPatternEnds)
                    {
                        if (sPrevPattern.EndsWith(patEnd))
                        {
                            validPrevPattern = true;
                            break;
                        }
                    }
                }
                if (validPrevPattern)
                {
                    sPrevPattern = sPattern.Substring(0, 16);
                    if (sPattern.StartsWith(SADDef.Bank_8_9_0_SigStart.ToLower()))
                    // Bank 8 9 0
                    {
                        if (sPattern.StartsWith(SADDef.Bank_9_0_SigStart.ToLower()))
                        // Bank 9 0
                        {
                            if (!foundBank0 && (sPattern.EndsWith(SADDef.Bank_0_SigEnd_1.ToLower()) || sPattern.EndsWith(SADDef.Bank_0_SigEnd_2.ToLower())))
                            // Bank 0
                            {
                                banksInfos.Add(new int[] { 0, iPos, SADDef.Bank_Max_Size });
                                iPos += SADDef.Bank_Min_Size - 16;
                                sPrevPattern = string.Empty;
                                foundBank0 = true;
                            }
                            else if (!foundBank9 && (sPattern.EndsWith(SADDef.Bank_9_SigEnd_1.ToLower()) || sPattern.EndsWith(SADDef.Bank_9_SigEnd_2.ToLower())))
                            // Bank 9
                            {
                                banksInfos.Add(new int[] { 9, iPos, SADDef.Bank_Max_Size });
                                iPos += SADDef.Bank_Min_Size - 16;
                                sPrevPattern = string.Empty;
                                foundBank9 = true;
                            }
                            else if (!foundBank0)
                            // Bank 0 In Priority
                            {
                                banksInfos.Add(new int[] { 0, iPos, SADDef.Bank_Max_Size });
                                iPos += SADDef.Bank_Min_Size - 16;
                                sPrevPattern = string.Empty;
                                foundBank0 = true;
                            }
                            else if (!foundBank9)
                            // Bank 9
                            {
                                banksInfos.Add(new int[] { 9, iPos, SADDef.Bank_Max_Size });
                                iPos += SADDef.Bank_Min_Size - 16;
                                sPrevPattern = string.Empty;
                                foundBank9 = true;
                            }
                        }
                        else if (sPattern.StartsWith(SADDef.Bank_8_Early_SigStart.ToLower()))
                        // Bank 8 Early
                        {
                            is8061 = true;
                            isEarly = true;
                            banksInfos.Add(new int[] { 8, iPos, SADDef.Bank_Max_Size });
                            bank8StartAddress = iPos;
                            iPos += SADDef.Bank_Min_Size - 16;
                            sPrevPattern = string.Empty;
                        }
                        else if (bank8StartAddress < 0)
                        // Bank 8
                        {
                            banksInfos.Add(new int[] { 8, iPos, SADDef.Bank_Max_Size });
                            bank8StartAddress = iPos;
                            iPos += SADDef.Bank_Min_Size - 16;
                            sPrevPattern = string.Empty;
                        }
                    }
                    // Bank 1
                    else if (!foundBank1 && (sPattern.StartsWith(SADDef.Bank_1_SigStart.ToLower())))
                    {
                        banksInfos.Add(new int[] { 1, iPos, SADDef.Bank_Max_Size });
                        iPos += SADDef.Bank_Min_Size - 16;
                        sPrevPattern = string.Empty;
                        foundBank1 = true;
                    }
                }
                else
                {
                    sPrevPattern = sPattern.Substring(0, 16);
                }
            }

            // Bank 8 was not found, could be corrupted on the first bytes
            if (banksInfos.Count == 0)
            {
                isCorrupted = true;

                startPos = 0;
                endPos = arrBytes.Length;

                sPrevPattern = string.Empty;
                for (int iPos = 0; iPos < arrBytes.Length; iPos += 16)
                {
                    sPattern = getBytes(iPos, 20);
                    if (sPattern.Length < 16) break;
                    if (sPrevPattern == string.Empty)
                    {
                        validPrevPattern = true;
                    }
                    else
                    {
                        validPrevPattern = false;
                        foreach (string patEnd in validPrevPatternEnds)
                        {
                            if (sPrevPattern.EndsWith(patEnd))
                            {
                                validPrevPattern = true;
                                break;
                            }
                        }
                    }
                    if (validPrevPattern)
                    {
                        sPrevPattern = sPattern.Substring(0, 16);

                        if (getBytes(iPos + 0x20, 2) == "0801")
                        // 8061 Fixed Levels Number & Calibs Number
                        {
                            arrBytes[0] = "ff";
                            arrBytes[1] = "fa";
                            banksInfos.Add(new int[] { 8, iPos, SADDef.Bank_Max_Size });
                            bank8StartAddress = iPos;
                            iPos += SADDef.Bank_Min_Size - 16;
                            sPrevPattern = string.Empty;
                            is8061 = true;
                        }
                        else if (getBytes(iPos + 0x60, 2) == "0801")
                        // 8065 Fixed Levels Number & Calibs Number
                        {
                            arrBytes[0] = "ff";
                            arrBytes[1] = "fa";
                            banksInfos.Add(new int[] { 8, iPos, SADDef.Bank_Max_Size });
                            bank8StartAddress = iPos;
                            iPos += SADDef.Bank_Min_Size - 16;
                            sPrevPattern = string.Empty;
                        }
                        else if (getBytes(iPos + 0x3, 1) == "21")
                        // 8061 Early Short Jump
                        {
                            arrBytes[0] = "ff";
                            arrBytes[1] = "fa";
                            banksInfos.Add(new int[] { 8, iPos, SADDef.Bank_Max_Size });
                            bank8StartAddress = iPos;
                            iPos += SADDef.Bank_Min_Size - 16;
                            sPrevPattern = string.Empty;
                            is8061 = true;
                            isEarly = true;
                        }
                    }
                    else
                    {
                        sPrevPattern = sPattern.Substring(0, 16);
                    }
                }
            }

            // Searching for Levels Number to define 8061 or 8065
            //      Check done on Calibs Number Value
            //      Same values can not be in same place on 8065, because they are Int Vectors
            //      Vector address 01XX is not possible, 1FXX is not possible, 20XX is possibe
            if (!is8061 && bank8StartAddress >= 0)
            {
                foreach (object[] resAdr in SADDef.ReservedAddresses8061Bank8)
                {
                    if (resAdr[3].ToString() == "CALNUM")
                    {
                        if (Tools.getByteInt(getBytes(bank8StartAddress + Convert.ToInt32(resAdr[1]), 1), false) < 0x20) is8061 = true;
                        break;
                    }
                }
            }

            // Searching for Bank 1 on Early and Pilot 8065 binaries
            // Searching for following Bank End VID Block fixed parts
            //  43 6F 70 79 72 69 67 68 74 - Copyright (Ascii)
            // Searching for following Bank Start for Early 8065
            //  XX XX FF FF FF FF FF FF FF FF XX XX FF FF FF FF
            //  ex: 84 31 00 00 00 00 00 00 00 00 33 2F FF FF FF FF
            //  ex: F8 2C 00 00 00 00 00 00 00 00 0F 7E FF FF FF FF
            //  ex: 0E 20 FF FF FF FF FF FF FF FF 19 4F FF FF FF FF
            if (!is8061 && !foundBank1 && !foundBank0 && !foundBank9)
            {
                int bank1EndPos = -1;
                
                if (bank8StartAddress >= SADDef.Bank_Min_Size)
                // Bank 1 could start before Bank 8
                {
                    startPos = bank8StartAddress;
                    endPos = SADDef.Bank_Min_Size;

                    // Searching for Bank End VID Block fixed parts
                    for (int iPos = startPos; iPos >= endPos; iPos -= SADDef.Bank_Min_Size / 4)
                    {
                        if (getBytes(iPos - 0x9d, 9) == SADDef.Bank_1_9_Copyright_Ascii.ToLower()) // Copyright (Ascii)
                        {
                            bank1EndPos = iPos - 1;
                            break;
                        }
                    }
                }
                else
                // Bank 1 could start after Bank 8
                {
                    startPos = bank8StartAddress + SADDef.Bank_Max_Size + SADDef.Bank_Min_Size;
                    endPos = arrBytes.Length;

                    // Searching for Bank End VID Block fixed parts
                    for (int iPos = startPos; iPos <= endPos; iPos += SADDef.Bank_Min_Size / 4)
                    {
                        if (getBytes(iPos - 0x9d, 9) == SADDef.Bank_1_9_Copyright_Ascii.ToLower()) // Copyright (Ascii)
                        {
                            bank1EndPos = iPos - 1;
                            break;
                        }
                    }
                }

                if (bank1EndPos >= 0)
                {
                    startPos = bank1EndPos - SADDef.Bank_Min_Size + 1;
                    while (bank1EndPos - startPos <= SADDef.Bank_Max_Size - 1 && startPos >= 0)
                    {
                        sPattern = getBytes(startPos, 16);
                        if (sPattern.Substring(0, 4) != new string('0', 4) & sPattern.Substring(0, 4) != new string('f', 4) & sPattern.Substring(20, 4) != new string('0', 4) & sPattern.Substring(20, 4) != new string('f', 4))
                        // Early 8065 Bank Start
                        {
                            if ((sPattern.Substring(4, 16) == new string('0', 16) || sPattern.Substring(4, 16) == new string('f', 16)))
                            {
                                if (sPattern.Substring(24, 8) == new string('0', 8) || sPattern.Substring(24, 8) == new string('f', 8))
                                {
                                    if (startPos > bank8StartAddress) banksInfos.Add(new int[] { 1, startPos, bank1EndPos - startPos + 1 });
                                    else banksInfos.Insert(0, new int[] { 1, startPos, bank1EndPos - startPos + 1 });
                                    foundBank1 = true;
                                    isEarly = true;
                                    break;
                                }
                            }
                        }
                        else if (sPattern.Substring(0, 20) == new string('f', 20) && sPattern.Substring(20, 4) != new string('f', 4) && sPattern.Substring(24, 8) == new string('f', 8))
                        // Empty Bank Start, It is a Pilot 8065 Bank Start
                        {
                            if (startPos > bank8StartAddress) banksInfos.Add(new int[] { 1, startPos, bank1EndPos - startPos + 1 });
                            else banksInfos.Insert(0, new int[] { 1, startPos, bank1EndPos - startPos + 1 });
                            foundBank1 = true;
                            isPilot = true;
                            break;
                        }
                        startPos -= SADDef.Bank_Min_Size / 4;
                    }
                }
            }

            // Last chance Searching for Early 8061 Bank Start
            if (!is8061 && !foundBank1 && !foundBank0 && !foundBank9)
            {
                // Bank 1 could start after Bank 8
                startPos = bank8StartAddress + SADDef.Bank_Max_Size;
                endPos = arrBytes.Length;
                if (bank8StartAddress >= SADDef.Bank_Min_Size)
                // Bank 1 could start at the beginning of Binary
                {
                    startPos = 0;
                    endPos = bank8StartAddress;
                }
                for (int iPos = startPos; iPos < endPos; iPos += 16)
                {
                    sPattern = getBytes(iPos, 16);
                    if (sPattern.Length == 32)
                    {
                        if (sPattern.Substring(0, 4) != new string('0', 4) & sPattern.Substring(0, 4) != new string('f', 4) & sPattern.Substring(20, 4) != new string('0', 4) & sPattern.Substring(20, 4) != new string('f', 4))
                        // Early 8065 Bank Start
                        {
                            if ((sPattern.Substring(4, 16) == new string('0', 16) || sPattern.Substring(4, 16) == new string('f', 16)))
                            {
                                if (sPattern.Substring(24, 8) == new string('0', 8) || sPattern.Substring(24, 8) == new string('f', 8))
                                {
                                    if (iPos > bank8StartAddress) banksInfos.Add(new int[] { 1, iPos, SADDef.Bank_Min_Size });
                                    else banksInfos.Insert(0, new int[] { 1, iPos, SADDef.Bank_Min_Size });
                                    foundBank1 = true;
                                    isEarly = true;
                                }
                            }
                        }
                    }
                }
            }
            
            // Creating Bank objects
            for (int iBankPos = 0; iBankPos < banksInfos.Count; iBankPos++)
            {
                bankNum = ((int[])banksInfos[iBankPos])[0];
                bankStartAddress = ((int[])banksInfos[iBankPos])[1];
                if (is8061 || isEarly || isPilot) bankEndAddress = bankStartAddress + ((int[])banksInfos[iBankPos])[2] - 1;
                else if (iBankPos == banksInfos.Count - 1) bankEndAddress = arrBytes.Length - 1;
                else bankEndAddress = ((int[])banksInfos[iBankPos + 1])[1] - 1;
                if (bankEndAddress - bankStartAddress > SADDef.Bank_Max_Size - 1) bankEndAddress = bankStartAddress + SADDef.Bank_Max_Size - 1;
                if (bankEndAddress > arrBytes.Length - 1) bankEndAddress = arrBytes.Length - 1;
                
                switch (bankNum)
                {
                    case 0:
                        Bank0 = new SADBank(0, bankStartAddress, bankEndAddress, is8061, isEarly, isPilot, ref Calibration, ref arrBytes);
                        break;
                    case 1:
                        Bank1 = new SADBank(1, bankStartAddress, bankEndAddress, is8061, isEarly, isPilot, ref Calibration, ref arrBytes);
                        break;
                    case 8:
                        Bank8 = new SADBank(8, bankStartAddress, bankEndAddress, is8061, isEarly, isPilot, ref Calibration, ref arrBytes);
                        break;
                    case 9:
                        Bank9 = new SADBank(9, bankStartAddress, bankEndAddress, is8061, isEarly, isPilot, ref Calibration, ref arrBytes);
                        break;
                }
            }

            banksInfos = null;
            validPrevPatternEnds = null;
        }

        private void readCalibration()
        {
            SortedList slRbases = null;

            string[] firstRBaseResSign = null;
            object[] sigResult = null;

            string levelsNumResAddress = string.Empty;
            string firstRBaseResAddress = string.Empty;

            int rBaseCodeInt = -1;

            Calibration.Info = new SADInfo();
            Calibration.Info.is8061 = is8061;
            Calibration.Info.isEarly = isEarly;
            Calibration.Info.isPilot = isPilot;

            slRbases = new SortedList();

            if (Bank8 != null) Calibration.Info.slBanksInfos.Add(8, new string[] { Bank8.Num.ToString(), Bank8.AddressBin, Bank8.AddressBinEnd, Bank8.AddressInternal, Bank8.AddressInternalEnd });
            if (Bank1 != null) Calibration.Info.slBanksInfos.Add(1, new string[] { Bank1.Num.ToString(), Bank1.AddressBin, Bank1.AddressBinEnd, Bank1.AddressInternal, Bank1.AddressInternalEnd });
            if (Bank9 != null) Calibration.Info.slBanksInfos.Add(9, new string[] { Bank9.Num.ToString(), Bank9.AddressBin, Bank9.AddressBinEnd, Bank9.AddressInternal, Bank9.AddressInternalEnd });
            if (Bank0 != null) Calibration.Info.slBanksInfos.Add(0, new string[] { Bank0.Num.ToString(), Bank0.AddressBin, Bank0.AddressBinEnd, Bank0.AddressInternal, Bank0.AddressInternalEnd });

            if (Bank8 == null) return;

            foreach (ReservedAddress rAddress in Bank8.slReserved.Values)
            {
                switch (rAddress.Type)
                {
                    case ReservedAddressType.CheckSum:
                        Calibration.Info.CheckSum = rAddress.ValueInt;
                        break;
                    case ReservedAddressType.SmpBase:
                        Calibration.Info.SmpBaseAddress = rAddress.ValueInt;
                        break;
                    case ReservedAddressType.CcExeTime:
                        Calibration.Info.CcExeTime = rAddress.ValueInt;
                        break;
                    case ReservedAddressType.LevelNum:
                        Calibration.Info.LevelsNum = rAddress.ValueInt;
                        levelsNumResAddress = rAddress.Address;
                        break;
                    case ReservedAddressType.CalNum:
                        Calibration.Info.CalibsNum = rAddress.ValueInt;
                        break;
                    case ReservedAddressType.CalPointer:
                        if (firstRBaseResAddress == string.Empty) firstRBaseResAddress = rAddress.Address;
                        break;
                }
            }

            // Early 8061
            if (is8061 && isEarly) firstRBaseResSign = new string[] { };
            // 8061
            if (is8061) firstRBaseResSign = new string[] {SADDef.Info_8061_FirstRBase_Signature_1.ToLower(), SADDef.Info_8061_FirstRBase_Signature_2.ToLower(), SADDef.Info_8061_FirstRBase_Signature_3.ToLower()};
            // 8065
            else firstRBaseResSign = new string[] { SADDef.Info_8065_FirstRBase_Signature_1.ToLower(), SADDef.Info_8065_FirstRBase_Signature_2.ToLower(), SADDef.Info_8065_FirstRBase_Signature_3.ToLower() };

            foreach (string sig in firstRBaseResSign)
            {
                sigResult = Bank8.getBytesFromSignature(sig.Replace("%cpoi%", Tools.LsbFirst(firstRBaseResAddress)).Replace("%lnum%", Tools.LsbFirst(levelsNumResAddress)).Replace("%frbase%", ".."));
                if (sigResult.Length > 0)
                {
                    sigResult = (object[])sigResult[0];
                    try
                    {
                        string rBaseCode = ((string)sigResult[1]).Substring(sig.Replace("%cpoi%", Tools.LsbFirst(firstRBaseResAddress)).Replace("%lnum%", Tools.LsbFirst(levelsNumResAddress)).IndexOf("%frbase%"), 2);
                        rBaseCodeInt = Convert.ToInt32(rBaseCode, 16);
                        break;
                    }
                    catch
                    {
                        rBaseCodeInt = -1;
                    }
                }
                if (rBaseCodeInt >= 0) break;

            }
            sigResult = null;

            // Early 8061 - No RBase, a fake One is created by default
            //      Calibration Part always start at 0x2400
            //      End will be calculated based on next First found Operation after ProcessBank
            //      It will also apply on its single fake RBase at this moment
            if (is8061 && isEarly)
            {
                RBase rBase = new RBase();
                rBase.Code = "si";              // Single
                rBase.AddressBankInt = 0x400;   // Fixed Value
                rBase.AddressBankEndInt = Bank8.AddressInternalEndInt;  // Will be corrected later on after Process Bank

                rBase.BankNum = 8;
                rBase.AddressBinInt = rBase.AddressBankInt + Bank8.AddressBinInt;

                slRbases.Add(rBase.Code, rBase);
            }
            else
            {
                foreach (ReservedAddress rAddress in Bank8.slReserved.Values)
                {
                    if (rAddress.Type == ReservedAddressType.CalPointer)
                    {
                        RBase rBase = new RBase();
                        if (rBaseCodeInt <= 0)
                        {
                            rBase.Code = "?" + rAddress.Value(16) + "?";
                        }
                        else
                        {
                            rBase.Code = string.Format("{0:x2}", rBaseCodeInt);
                            rBaseCodeInt += 2;
                        }

                        rBase.AddressBankInt = rAddress.ValueInt - SADDef.EecBankStartAddress;

                        // 8061 && 8065 Pilot - Calibration in Bank 8
                        // 8065 - RBases defined in Bank 8 for Calibration in Bank 1
                        // 8061 & 8065 - First value of Calibration Part is next Calibration Part Address - End is next Part Address - 1
                        if (is8061 || isPilot)
                        {
                            rBase.BankNum = 8;
                            rBase.AddressBinInt = rBase.AddressBankInt + Bank8.AddressBinInt;
                            rBase.AddressBankEndInt = Bank8.getWordInt(rBase.AddressBankInt, false, true) - 1 - SADDef.EecBankStartAddress;
                        }
                        else
                        {
                            rBase.BankNum = 1;
                            if (Bank1 != null)
                            {
                                rBase.AddressBinInt = rBase.AddressBankInt + Bank1.AddressBinInt;
                                rBase.AddressBankEndInt = Bank1.getWordInt(rBase.AddressBankInt, false, true) - 1 - SADDef.EecBankStartAddress;
                            }
                        }

                        slRbases.Add(rBase.Code, rBase);
                    }
                }
            }

            if (is8061 || isPilot) Calibration.Load(slRbases, ref Bank8, is8061, isEarly, isPilot, ref arrBytes);
            else Calibration.Load(slRbases, ref Bank1, is8061, isEarly, isPilot, ref arrBytes);
        }

        // Checksum Calculation Identification & Calculation
        private void readChecksum()
        {
            Scalar scalarStartAddress = null;
            Scalar scalarEndAddress = null;
            Structure sStruct = null;

            string[] checkSumCalcSign = null;
            object[] sigResult = null;
            int sigMode = -1;

            int checkSumOpeRoutineAddress = -1;
            
            int checkSumPointerAddress1 = -1;
            int checkSumPointerAddress2 = -1;
            
            int checkSumAddress1 = -1;
            int checkSumAddress2 = -1;

            string tmpAddress = string.Empty;
            int cnt = 0;

            Calibration.Info.isCheckSumConfirmed = false;
            Calibration.Info.isCheckSumValid = false;
            Calibration.Info.correctedChecksum = -1;

            if (!isValid) return;

            // Checksum Calculation Identification
            if (is8061)
            // 8061
            {
                Bank8.CheckSumStartAddress = -1;
                Bank8.CheckSumEndAddress = -1;
                scalarStartAddress = null;
                scalarEndAddress = null;
                sStruct = null;

                checkSumCalcSign = new string[] { SADDef.Info_8061_CheckSumCalc_Signature_1.ToLower(), SADDef.Info_8061_CheckSumCalc_Signature_2.ToLower(), SADDef.Info_8061_CheckSumCalc_Signature_3.ToLower(), SADDef.Info_8061_CheckSumCalc_Signature_4.ToLower() };
                sigMode = 0;
                foreach (string sig in checkSumCalcSign)
                {
                    sigMode++;
                    sigResult = Bank8.getBytesFromSignature(sig.Replace("%cscstartp%", "....").Replace("%cscendp%", "...."));
                    if (sigResult.Length > 0)
                    {
                        sigResult = (object[])sigResult[0];
                        try
                        {
                            if (sigMode == 1)
                            {
                                checkSumOpeRoutineAddress = (int)sigResult[0];

                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.IndexOf("%cscstartp%"), 4));
                                checkSumPointerAddress1 = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress;
                                checkSumAddress1 = Bank8.getWordInt(checkSumPointerAddress1, false, true) - SADDef.EecBankStartAddress;
                                
                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.Replace("%cscstartp%", "....").IndexOf("%cscendp%"), 4));
                                checkSumPointerAddress2 = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress;
                                checkSumAddress2 = Bank8.getWordInt(checkSumPointerAddress2, false, true) - SADDef.EecBankStartAddress;

                                if (checkSumAddress2 > checkSumAddress1)
                                {
                                    Bank8.CheckSumStartAddress = checkSumAddress1;
                                    Bank8.CheckSumEndAddress = checkSumAddress2;
                                    scalarStartAddress = new Scalar(8, checkSumPointerAddress1, false, false);
                                    scalarStartAddress.AddressBinInt = scalarStartAddress.AddressInt + Bank8.AddressBinInt;
                                    scalarEndAddress = new Scalar(8, checkSumPointerAddress2, false, false);
                                    scalarEndAddress.AddressBinInt = scalarEndAddress.AddressInt + Bank8.AddressBinInt;
                                }
                                else
                                {
                                    Bank8.CheckSumStartAddress = checkSumAddress2;
                                    Bank8.CheckSumEndAddress = checkSumAddress1;
                                    scalarStartAddress = new Scalar(8, checkSumPointerAddress2, false, false);
                                    scalarStartAddress.AddressBinInt = scalarStartAddress.AddressInt + Bank8.AddressBinInt;
                                    scalarEndAddress = new Scalar(8, checkSumPointerAddress1, false, false);
                                    scalarEndAddress.AddressBinInt = scalarEndAddress.AddressInt + Bank8.AddressBinInt;
                                }
                                sigResult = null;
                                break;
                            }
                            else if (sigMode == 2 || sigMode == 3)
                            {
                                checkSumOpeRoutineAddress = (int)sigResult[0];

                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.IndexOf("%cscstartp%"), 4));
                                checkSumPointerAddress1 = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress;
                                checkSumAddress1 = Bank8.getWordInt(checkSumPointerAddress1, false, true) - SADDef.EecBankStartAddress;

                                checkSumPointerAddress2 = checkSumPointerAddress1 + 2;
                                checkSumAddress2 = Bank8.getWordInt(checkSumPointerAddress2, false, true) - SADDef.EecBankStartAddress + 1;

                                if (checkSumAddress2 > checkSumAddress1)
                                {
                                    Bank8.CheckSumStartAddress = checkSumAddress1;
                                    Bank8.CheckSumEndAddress = checkSumAddress2;
                                    scalarStartAddress = new Scalar(8, checkSumPointerAddress1, false, false);
                                    scalarStartAddress.AddressBinInt = scalarStartAddress.AddressInt + Bank8.AddressBinInt;
                                    scalarEndAddress = new Scalar(8, checkSumPointerAddress2, false, false);
                                    scalarEndAddress.AddressBinInt = scalarEndAddress.AddressInt + Bank8.AddressBinInt;
                                }
                                else
                                {
                                    Bank8.CheckSumStartAddress = checkSumAddress2;
                                    Bank8.CheckSumEndAddress = checkSumAddress1;
                                    scalarStartAddress = new Scalar(8, checkSumPointerAddress2, false, false);
                                    scalarStartAddress.AddressBinInt = scalarStartAddress.AddressInt + Bank8.AddressBinInt;
                                    scalarEndAddress = new Scalar(8, checkSumPointerAddress1, false, false);
                                    scalarEndAddress.AddressBinInt = scalarEndAddress.AddressInt + Bank8.AddressBinInt;
                                }
                                sigResult = null;
                                break;
                            }
                            else if (sigMode == 4)
                            {
                                checkSumOpeRoutineAddress = (int)sigResult[0];

                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.IndexOf("%cscstartp%"), 4));
                                checkSumPointerAddress1 = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress;

                                sStruct = new Structure();
                                sStruct.BankNum = 8;
                                sStruct.AddressInt = checkSumPointerAddress1;
                                sStruct.AddressBinInt = sStruct.AddressInt + Bank8.AddressBinInt;
                                sStruct.ShortLabel = SADDef.ShortLabelStructCheckSumAdr;
                                sStruct.Label = SADDef.LongLabelStructCheckSumAdr;
                                sStruct.StructDefString = "WordHex,WordHex";

                                checkSumAddress1 = Bank8.getWordInt(checkSumPointerAddress1, false, true) - SADDef.EecBankStartAddress;
                                checkSumAddress2 = checkSumAddress1;

                                cnt = 0;
                                while (cnt < 100 && checkSumPointerAddress1 <= Bank8.AddressInternalEndInt - 3)
                                {
                                    cnt++;
                                    checkSumPointerAddress1 += 2;
                                    checkSumPointerAddress2 = Bank8.getWordInt(checkSumPointerAddress1, false, true) - SADDef.EecBankStartAddress;
                                    if (checkSumPointerAddress2 <= checkSumAddress2) break;
                                    checkSumAddress2 = checkSumPointerAddress2;
                                }
                                // Invalid Structure
                                if (cnt >= 100) sStruct = null;
                                else
                                // Valid Structure
                                {
                                    Bank8.CheckSumStartAddress = checkSumAddress1;
                                    Bank8.CheckSumEndAddress = checkSumAddress2;
                                    sStruct.Number = (checkSumPointerAddress1 - sStruct.AddressInt) / 4;
                                }
                                sigResult = null;
                                break;
                            }
                        }
                        catch { }
                    }
                }
            }
            else if (isPilot)
            // Pilot 8065
            {
                Bank8.CheckSumStartAddress = -1;
                Bank8.CheckSumEndAddress = -1;

                checkSumCalcSign = new string[] { SADDef.Info_8065_Pilot_CheckSumCalc_Signature_Banks_8.ToLower() };
                sigMode = 0;
                foreach (string sig in checkSumCalcSign)
                {
                    sigMode++;
                    sigResult = Bank8.getBytesFromSignature(sig.Replace("%cscstartp%", "...."));
                    if (sigResult.Length > 0)
                    {
                        sigResult = (object[])sigResult[0];
                        try
                        {
                            if (sigMode == 1)
                            {
                                checkSumOpeRoutineAddress = (int)sigResult[0];

                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.IndexOf("%cscstartp%"), 4));
                                checkSumPointerAddress1 = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress;
                                checkSumAddress1 = Bank8.getWordInt(checkSumPointerAddress1, false, true) - SADDef.EecBankStartAddress;

                                checkSumPointerAddress2 = checkSumPointerAddress1 + 2;
                                checkSumAddress2 = Bank8.getWordInt(checkSumPointerAddress2, false, true) - SADDef.EecBankStartAddress + 1;

                                if (checkSumAddress2 > checkSumAddress1)
                                {
                                    Bank8.CheckSumStartAddress = checkSumAddress1;
                                    Bank8.CheckSumEndAddress = checkSumAddress2;
                                    scalarStartAddress = new Scalar(8, checkSumPointerAddress1, false, false);
                                    scalarStartAddress.AddressBinInt = scalarStartAddress.AddressInt + Bank8.AddressBinInt;
                                    scalarEndAddress = new Scalar(8, checkSumPointerAddress2, false, false);
                                    scalarEndAddress.AddressBinInt = scalarEndAddress.AddressInt + Bank8.AddressBinInt;
                                }
                                else
                                {
                                    Bank8.CheckSumStartAddress = checkSumAddress2;
                                    Bank8.CheckSumEndAddress = checkSumAddress1;
                                    scalarStartAddress = new Scalar(8, checkSumPointerAddress2, false, false);
                                    scalarStartAddress.AddressBinInt = scalarStartAddress.AddressInt + Bank8.AddressBinInt;
                                    scalarEndAddress = new Scalar(8, checkSumPointerAddress1, false, false);
                                    scalarEndAddress.AddressBinInt = scalarEndAddress.AddressInt + Bank8.AddressBinInt;
                                }
                                sigResult = null;
                                break;
                            }
                        }
                        catch { }
                    }
                }
            }
            else if (isEarly)
            // Early 8065
            {
                Bank8.CheckSumStartAddress = -1;
                Bank8.CheckSumEndAddress = -1;
                Bank1.CheckSumStartAddress = -1;
                Bank1.CheckSumEndAddress = -1;

                checkSumCalcSign = new string[] { SADDef.Info_8065_Early_CheckSumCalc_Signature_Banks_8_1.ToLower(), SADDef.Info_8065_Early_CheckSumCalc_Signature_Bank_8.ToLower(), SADDef.Info_8065_Early_CheckSumCalc_Signature_Bank_1.ToLower() };
                sigMode = 0;
                foreach (string sig in checkSumCalcSign)
                {
                    sigMode++;
                    sigResult = Bank8.getBytesFromSignature(sig.Replace("%cscstartp%", "....").Replace("%cscendp%", "....").Replace("%cscendp1%", "....").Replace("%cscendp8%", "...."));
                    if (sigResult.Length > 0)
                    {
                        sigResult = (object[])sigResult[0];
                        try
                        {
                            if (sigMode == 1)
                            // Banks 8 & 1
                            {
                                checkSumOpeRoutineAddress = (int)sigResult[0];

                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.IndexOf("%cscstartp%"), 4));
                                Bank8.CheckSumStartAddress = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress;
                                Bank1.CheckSumStartAddress = Bank8.CheckSumStartAddress;

                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.Replace("%cscstartp%", "....").IndexOf("%cscendp1%"), 4));
                                Bank1.CheckSumEndAddress = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress + 1;
                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.Replace("%cscstartp%", "....").Replace("%cscendp1%", "....").IndexOf("%cscendp8%"), 4));
                                Bank8.CheckSumEndAddress = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress + 1;

                                sigResult = null;
                                break;
                            }
                            else if (sigMode == 2)
                            // Bank 8
                            {
                                checkSumOpeRoutineAddress = (int)sigResult[0];

                                Bank8.CheckSumStartAddress = 0;
                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.IndexOf("%cscendp%"), 4));
                                Bank8.CheckSumEndAddress = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress + 1;
                            }
                            else if (sigMode == 3)
                            // Bank 1
                            {
                                Bank1.CheckSumStartAddress = 0;
                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.IndexOf("%cscendp%"), 4));
                                Bank1.CheckSumEndAddress = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress + 1;
                            }
                        }
                        catch { }
                    }
                    sigResult = null;
                }
            }
            else
            // 8065
            {
                scalarStartAddress = null;
                scalarEndAddress = null;

                foreach (int bankNum in Calibration.Info.slBanksInfos.Keys)
                {
                    switch (bankNum)
                    {
                        case 1:
                            checkSumCalcSign = new string[] { SADDef.Info_8065_CheckSumCalc_Signature_Bank_1.ToLower() };
                            break;
                        default:
                            checkSumCalcSign = new string[] { SADDef.Info_8065_CheckSumCalc_Signature_Banks_8_0_9.ToLower() };
                            break;
                    }

                    sigMode = 0;
                    foreach (string sig in checkSumCalcSign)
                    {
                        sigMode++;
                        sigResult = Bank8.getBytesFromSignature(sig.Replace("%cscstartp%", "....").Replace("%cscendp%", "....").Replace("%cscbank%", string.Format("{0:x2}", bankNum)));
                        if (sigResult.Length > 0)
                        {
                            sigResult = (object[])sigResult[0];
                            try
                            {
                                if (checkSumOpeRoutineAddress < 0) checkSumOpeRoutineAddress = (int)sigResult[0];

                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.IndexOf("%cscstartp%"), 4));
                                checkSumAddress1 = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress;

                                tmpAddress = Tools.LsbFirst(((string)sigResult[1]).Substring(sig.Replace("%cscstartp%", tmpAddress).Replace("%cscbank%", "..").IndexOf("%cscendp%"), 4));
                                checkSumAddress2 = Convert.ToInt32(tmpAddress, 16) - SADDef.EecBankStartAddress + 1;

                                if (checkSumAddress2 < checkSumAddress1)
                                {
                                    checkSumPointerAddress1 = checkSumAddress1;
                                    checkSumAddress1 = checkSumAddress2;
                                    checkSumAddress2 = checkSumPointerAddress1;
                                }

                                switch (bankNum)
                                {
                                    case 8:
                                        Bank8.CheckSumStartAddress = checkSumAddress1;
                                        Bank8.CheckSumEndAddress = checkSumAddress2;
                                        break;
                                    case 1:
                                        Bank1.CheckSumStartAddress = checkSumAddress1;
                                        Bank1.CheckSumEndAddress = checkSumAddress2;
                                        break;
                                    case 9:
                                        Bank9.CheckSumStartAddress = checkSumAddress1;
                                        Bank9.CheckSumEndAddress = checkSumAddress2;
                                        break;
                                    case 0:
                                        Bank0.CheckSumStartAddress = checkSumAddress1;
                                        Bank0.CheckSumEndAddress = checkSumAddress2;
                                        break;
                                }
                            }
                            catch { }
                        }
                        sigResult = null;
                    }
                }
            }

            if (scalarStartAddress != null && scalarEndAddress != null)
            {
                scalarStartAddress.Label = SADDef.LongLabelScalarCheckSumStartAdr;
                scalarStartAddress.ShortLabel = SADDef.ShortLabelScalarCheckSumStartAdr;
                scalarEndAddress.Label = SADDef.LongLabelScalarCheckSumEndAdr;
                scalarEndAddress.ShortLabel = SADDef.ShortLabelScalarCheckSumEndAdr;
                Calibration.slExtScalars.Add(scalarStartAddress.UniqueAddress, scalarStartAddress);
                Calibration.slExtScalars.Add(scalarEndAddress.UniqueAddress, scalarEndAddress);
                Calibration.alLoadExtScalarsUniqueAddresses.Add(scalarStartAddress.UniqueAddress);
                Calibration.alLoadExtScalarsUniqueAddresses.Add(scalarEndAddress.UniqueAddress);
                scalarStartAddress = null;
                scalarEndAddress = null;
            }

            if (sStruct != null)
            {
                Calibration.slExtStructures.Add(sStruct.UniqueAddress, sStruct);
                Calibration.alLoadStructuresUniqueAddresses.Add(sStruct.UniqueAddress);
                sStruct = null;
            }

            // Preparing for Checksum Routine Creation
            if (checkSumOpeRoutineAddress >= 0)
            {
                bool addRoutinesCodesOpsAddress = true;
                foreach (object[] rcoRco in Calibration.alRoutinesCodesOpsAddresses)
                {
                    if ((RoutineCode)rcoRco[0] == RoutineCode.Checksum)
                    {
                        addRoutinesCodesOpsAddress = false;
                        break;
                    }
                }
                if (addRoutinesCodesOpsAddress)
                {
                    Calibration.alRoutinesCodesOpsAddresses.Add(new object[] { RoutineCode.Checksum, 8, checkSumOpeRoutineAddress });
                    Calibration.alLoadRoutinesCodesOpsAddresses.Add(new object[] { RoutineCode.Checksum, 8, checkSumOpeRoutineAddress });
                }
            }

            Calibration.Info.isCheckSumConfirmed = true;
            UInt16 checkSumCalc = 0;
            // CheckSum Calculation
            foreach (int bankNum in Calibration.Info.slBanksInfos.Keys)
            {
                switch (bankNum)
                {
                    case 8:
                        if (Bank8.CheckSumStartAddress >= 0 && Bank8.CheckSumEndAddress >= 0 && Bank8.CheckSumEndAddress <= Bank8.AddressInternalEndInt)
                        {
                            for (int iPos = Bank8.CheckSumStartAddress; iPos <= Bank8.CheckSumEndAddress - 1; iPos += 2) checkSumCalc += (UInt16)Bank8.getWordInt(iPos, false, true);
                        }
                        else 
                        {
                            Bank8.CheckSumStartAddress = Bank8.AddressInternalInt;
                            Bank8.CheckSumEndAddress = Bank8.AddressInternalEndInt;
                            Calibration.Info.isCheckSumConfirmed = false;
                        }
                        break;
                    case 1:
                        // 8065 Pilot Specific Case - No Checksum on Bank 1
                        if (isPilot) break;
                        if (Bank1.CheckSumStartAddress >= 0 && Bank1.CheckSumEndAddress >= 0 && Bank1.CheckSumEndAddress <= Bank1.AddressInternalEndInt)
                        {
                            for (int iPos = Bank1.CheckSumStartAddress; iPos <= Bank1.CheckSumEndAddress - 1; iPos += 2) checkSumCalc += (UInt16)Bank1.getWordInt(iPos, false, true);
                        }
                        else
                        {
                            Bank1.CheckSumStartAddress = Bank1.AddressInternalInt;
                            Bank1.CheckSumEndAddress = Bank1.AddressInternalEndInt;
                            Calibration.Info.isCheckSumConfirmed = false;
                        }
                        break;
                    case 9:
                        if (Bank9.CheckSumStartAddress >= 0 && Bank9.CheckSumEndAddress >= 0 && Bank9.CheckSumEndAddress <= Bank9.AddressInternalEndInt)
                        {
                            for (int iPos = Bank9.CheckSumStartAddress; iPos <= Bank9.CheckSumEndAddress - 1; iPos += 2) checkSumCalc += (UInt16)Bank9.getWordInt(iPos, false, true);
                        }
                        else
                        {
                            Bank9.CheckSumStartAddress = Bank9.AddressInternalInt;
                            Bank9.CheckSumEndAddress = Bank9.AddressInternalEndInt;
                            Calibration.Info.isCheckSumConfirmed = false;
                        }
                        break;
                    case 0:
                        if (Bank0.CheckSumStartAddress >= 0 && Bank0.CheckSumEndAddress >= 0 && Bank0.CheckSumEndAddress <= Bank0.AddressInternalEndInt)
                        {
                            for (int iPos = Bank0.CheckSumStartAddress; iPos <= Bank0.CheckSumEndAddress - 1; iPos += 2) checkSumCalc += (UInt16)Bank0.getWordInt(iPos, false, true);
                        }
                        else
                        {
                            Bank0.CheckSumStartAddress = Bank0.AddressInternalInt;
                            Bank0.CheckSumEndAddress = Bank0.AddressInternalEndInt;
                            Calibration.Info.isCheckSumConfirmed = false;
                        }
                        break;
                }
            }
            Calibration.Info.isCheckSumValid = false;
            if (Calibration.Info.isCheckSumConfirmed)
            {
                Calibration.Info.isCheckSumValid = (checkSumCalc == 0);
                Calibration.Info.correctedChecksum = Calibration.Info.CheckSum;
                if (!Calibration.Info.isCheckSumValid)
                {
                    checkSumCalc -= (UInt16)Calibration.Info.CheckSum;
                    checkSumCalc = (UInt16)(0 - checkSumCalc);
                    Calibration.Info.correctedChecksum = checkSumCalc;
                }
            }
        }

        // VID Block Reading
        private void readVidBlock()
        {
            int vidAddressAdder = 0;

            if (!isValid) return;

            // VID Block
            Calibration.Info.VidBankNum = -1;
            vidAddressAdder = 0;

            switch (Calibration.Info.slBanksInfos.Count)
            {
                case 1:
                    if (!isEarly && Bank8.CheckSumEndAddress > 0x5fff && Bank8.CheckSumEndAddress <= 0x7fff && Bank8.AddressInternalEndInt >= Bank8.CheckSumEndAddress) Calibration.Info.VidBankNum = 8;
                    break;
                case 2:
                    Calibration.Info.VidBankNum = 1;
                    if (Bank1.AddressInternalEndInt < 0xdf9d)
                    {
                        // Early 8065 use same VID Block than 8065, but addresses are lowered with 0x6000, base on Bank Size
                        vidAddressAdder = Bank1.AddressInternalEndInt - SADDef.Bank_Max_Size + 1;
                    }
                    break;
                case 4:
                    Calibration.Info.VidBankNum = 9;
                    if (Bank9.AddressInternalEndInt < 0xdf9d)
                    {
                        vidAddressAdder = Bank9.AddressInternalEndInt - SADDef.Bank_Max_Size + 1;
                    }
                    break;
            }
            if (Calibration.Info.VidBankNum >= 0)
            {
                object[] vidDef = null;

                if (is8061) vidDef = SADDef.Info_8061_VID_Block_Addresses;
                else vidDef = SADDef.Info_8065_VID_Block_Addresses;

                foreach (object[] vidPart in vidDef)
                {
                    string[] arrRes = null;
                    string strRes = string.Empty;
                    int intRes = -1;
                    bool bRes = false;
                    ReservedAddress rAddress = null;
                    int vidBankAddressBin = -1;

                    switch (Calibration.Info.VidBankNum)
                    {
                        case 0:
                            arrRes = Bank0.getBytesArray((int)vidPart[0] + vidAddressAdder, (int)vidPart[1]);
                            vidBankAddressBin = Bank0.AddressBinInt;
                            break;
                        case 1:
                            arrRes = Bank1.getBytesArray((int)vidPart[0] + vidAddressAdder, (int)vidPart[1]);
                            vidBankAddressBin = Bank1.AddressBinInt;
                            break;
                        case 8:
                            arrRes = Bank8.getBytesArray((int)vidPart[0] + vidAddressAdder, (int)vidPart[1]);
                            vidBankAddressBin = Bank8.AddressBinInt;
                            break;
                        case 9:
                            arrRes = Bank9.getBytesArray((int)vidPart[0] + vidAddressAdder, (int)vidPart[1]);
                            vidBankAddressBin = Bank9.AddressBinInt;
                            break;
                    }

                    switch (vidPart[2].ToString())
                    {
                        case "ASCII":
                            foreach (string sByte in arrRes)
                            {
                                if (sByte != string.Empty && sByte.ToLower() != vidPart[4].ToString().ToLower()) strRes += Convert.ToChar(Convert.ToByte(sByte, 16));
                            }
                            rAddress = new ReservedAddress(Calibration.Info.VidBankNum, (int)vidPart[0] + vidAddressAdder, (int)vidPart[1], ReservedAddressType.Ascii);
                            rAddress.AddressBinInt = rAddress.AddressInt + vidBankAddressBin;
                            rAddress.ValueString = strRes;
                            for (int iByte = 0; iByte < arrRes.Length; iByte++) rAddress.InitialValue += arrRes[iByte] + SADDef.GlobalSeparator;
                            rAddress.InitialValue = rAddress.InitialValue.Substring(0, rAddress.InitialValue.Length - SADDef.GlobalSeparator.ToString().Length);
                            rAddress.Label = vidPart[5].ToString();
                            rAddress.Comments = vidPart[6].ToString();
                            switch (vidPart[3].ToString())
                            {
                                case "STRATEGY":
                                    // Short Strategy Specificity
                                    if (strRes.Length > 2)
                                    {
                                        if (strRes.EndsWith("."))
                                        {
                                            rAddress.Size--;
                                            strRes = strRes.Substring(0, strRes.Length - 1);
                                            rAddress.ValueString = strRes;
                                            rAddress.InitialValue = rAddress.InitialValue.Substring(0, rAddress.InitialValue.Length - 2 - SADDef.GlobalSeparator.Length);
                                        }
                                        Calibration.Info.VidStrategy = strRes.Substring(0, strRes.Length - 2);
                                        Calibration.Info.VidStrategyVersion = strRes.Substring(strRes.Length - 2);
                                    }
                                    break;
                                case "COPYRIGHT":
                                    Calibration.Info.VidCopyright = strRes;
                                    break;
                                case "VIN":
                                    Calibration.Info.VidVIN = strRes;
                                    break;
                                case "SERIAL":
                                    Calibration.Info.VidSerial = strRes;
                                    break;
                            }
                            break;
                        case "HEX":
                            foreach (string sByte in arrRes) strRes += sByte;
                            rAddress = new ReservedAddress(Calibration.Info.VidBankNum, (int)vidPart[0] + vidAddressAdder, (int)vidPart[1], ReservedAddressType.Hex);
                            rAddress.AddressBinInt = rAddress.AddressInt + vidBankAddressBin;
                            rAddress.ValueString = strRes;
                            for (int iByte = 0; iByte < arrRes.Length; iByte++) rAddress.InitialValue += arrRes[iByte] + SADDef.GlobalSeparator;
                            rAddress.InitialValue = rAddress.InitialValue.Substring(0, rAddress.InitialValue.Length - SADDef.GlobalSeparator.ToString().Length);
                            rAddress.Label = vidPart[4].ToString();
                            rAddress.Comments = vidPart[5].ToString();
                            switch (vidPart[3].ToString())
                            {
                                case "PATSCODE":
                                    Calibration.Info.VidPatsCode = strRes;
                                    break;
                            }
                            break;
                        case "FLAG":
                            if (arrRes[0].ToLower() != vidPart[4].ToString().ToLower()) bRes = true;
                            rAddress = new ReservedAddress(Calibration.Info.VidBankNum, (int)vidPart[0] + vidAddressAdder, (int)vidPart[1], ReservedAddressType.Byte);
                            rAddress.AddressBinInt = rAddress.AddressInt + vidBankAddressBin;
                            if (bRes) rAddress.ValueInt = 1;
                            else rAddress.ValueInt = 0;
                            rAddress.InitialValue += arrRes[0];
                            rAddress.Label = vidPart[5].ToString();
                            rAddress.Comments = vidPart[6].ToString();
                            switch (vidPart[3].ToString())
                            {
                                case "VIDENABLED":
                                    Calibration.Info.VidEnabled = bRes;
                                    break;
                            }
                            break;
                        case "WORD":
                            if (arrRes[0].ToLower() == "ff" && arrRes[1].ToLower() == "ff") intRes = 0;
                            else intRes = (int)(Tools.getWordInt(arrRes, false, true) * Convert.ToDouble(vidPart[4]));
                            rAddress = new ReservedAddress(Calibration.Info.VidBankNum, (int)vidPart[0] + vidAddressAdder, (int)vidPart[1], ReservedAddressType.Word);
                            rAddress.AddressBinInt = rAddress.AddressInt + vidBankAddressBin;
                            rAddress.ValueInt = intRes;
                            for (int iByte = 0; iByte < arrRes.Length; iByte++) rAddress.InitialValue += arrRes[iByte] + SADDef.GlobalSeparator;
                            rAddress.InitialValue = rAddress.InitialValue.Substring(0, rAddress.InitialValue.Length - SADDef.GlobalSeparator.ToString().Length);
                            rAddress.Label = vidPart[5].ToString();
                            rAddress.Comments = vidPart[6].ToString();
                            switch (vidPart[3].ToString())
                            {
                                case "REVMILE":
                                    Calibration.Info.VidRevMile = intRes;
                                    break;
                                case "RTAXLE":
                                    Calibration.Info.VidRtAxle = intRes;
                                    break;
                            }
                            break;
                    }

                    if (rAddress != null)
                    {
                        switch (Calibration.Info.VidBankNum)
                        {
                            case 0:
                                Bank0.slReserved.Add(rAddress.UniqueAddress, rAddress);
                                break;
                            case 1:
                                Bank1.slReserved.Add(rAddress.UniqueAddress, rAddress);
                                break;
                            case 8:
                                Bank8.slReserved.Add(rAddress.UniqueAddress, rAddress);
                                break;
                            case 9:
                                Bank9.slReserved.Add(rAddress.UniqueAddress, rAddress);
                                break;
                        }
                    }
                }

                vidDef = null;
            }
        }

        private void readDefEecRegisters()
        {
            EecRegister eecReg = null;
            object[] eecRegisters = null;
            bool newEecReg = false;

            if (Calibration.slEecRegisters != null) return;

            // Eec Registers
            Calibration.slEecRegisters = new SortedList();
            if (is8061) eecRegisters = SADDef.EecRegisters8061;
            else eecRegisters = SADDef.EecRegisters8065;
            foreach (string[] oEecReg in eecRegisters)
            {
                newEecReg = !Calibration.slEecRegisters.Contains(oEecReg[0].ToLower());
                if (newEecReg) eecReg = new EecRegister(oEecReg[0].ToLower());
                else eecReg = (EecRegister)Calibration.slEecRegisters[oEecReg[0].ToLower()];

                switch (oEecReg[1].ToUpper())
                {
                    case "RO":
                    case "WO":
                        if (eecReg.Check == EecRegisterCheck.None) eecReg.Check = EecRegisterCheck.ReadWrite;
                        else if (eecReg.Check == EecRegisterCheck.DataType) eecReg.Check = EecRegisterCheck.Both;
                        break;
                    case "RW_B":
                    case "RW_W":
                        if (eecReg.Check == EecRegisterCheck.None) eecReg.Check = EecRegisterCheck.DataType;
                        else if (eecReg.Check == EecRegisterCheck.ReadWrite) eecReg.Check = EecRegisterCheck.Both;
                        break;
                    case "RO_B":
                    case "WO_B":
                    case "RO_W":
                    case "WO_W":
                        eecReg.Check = EecRegisterCheck.Both;
                        break;
                }

                switch (oEecReg[1].ToUpper())
                {
                    case "RW":
                        eecReg.TranslationReadByte = oEecReg[2];
                        eecReg.TranslationReadWord = oEecReg[2];
                        eecReg.TranslationWriteByte = oEecReg[2];
                        eecReg.TranslationWriteWord = oEecReg[2];
                        eecReg.CommentsReadByte = oEecReg[3];
                        eecReg.CommentsReadWord = oEecReg[3];
                        eecReg.CommentsWriteByte = oEecReg[3];
                        eecReg.CommentsWriteWord = oEecReg[3];
                        break;
                    case "RW_B":
                        eecReg.TranslationReadByte = oEecReg[2];
                        eecReg.TranslationWriteByte = oEecReg[2];
                        eecReg.CommentsReadByte = oEecReg[3];
                        eecReg.CommentsWriteByte = oEecReg[3];
                        break;
                    case "RW_W":
                        eecReg.TranslationReadWord = oEecReg[2];
                        eecReg.TranslationWriteWord = oEecReg[2];
                        eecReg.CommentsReadWord = oEecReg[3];
                        eecReg.CommentsWriteWord = oEecReg[3];
                        break;
                    case "RO":
                        eecReg.TranslationReadByte = oEecReg[2];
                        eecReg.TranslationReadWord = oEecReg[2];
                        eecReg.CommentsReadByte = oEecReg[3];
                        eecReg.CommentsReadWord = oEecReg[3];
                        break;
                    case "WO":
                        eecReg.TranslationWriteByte = oEecReg[2];
                        eecReg.TranslationWriteWord = oEecReg[2];
                        eecReg.CommentsWriteByte = oEecReg[3];
                        eecReg.CommentsWriteWord = oEecReg[3];
                        break;
                    case "RO_B":
                        eecReg.TranslationReadByte = oEecReg[2];
                        eecReg.CommentsReadByte = oEecReg[3];
                        break;
                    case "RO_W":
                        eecReg.TranslationReadWord = oEecReg[2];
                        eecReg.CommentsReadWord = oEecReg[3];
                        break;
                    case "WO_B":
                        eecReg.TranslationWriteByte = oEecReg[2];
                        eecReg.CommentsWriteByte = oEecReg[3];
                        break;
                    case "WO_W":
                        eecReg.TranslationWriteWord = oEecReg[2];
                        eecReg.CommentsWriteWord = oEecReg[3];
                        break;
                }
                if (newEecReg) Calibration.slEecRegisters.Add(eecReg.Code, eecReg);
                else Calibration.slEecRegisters[eecReg.Code] = eecReg;

                eecReg = null;
            }
            eecRegisters = null;
        }

        private void readDefOPCodes()
        {
            // OP Codes
            SADOPCode opCode = null;

            if (Calibration.slOPCodes != null) return;
            
            Calibration.slOPCodes = new SortedList();
            for (int iOPCode = 0; iOPCode <= 255; iOPCode++)
            {
                opCode = new SADOPCode(iOPCode, is8061);
                if (opCode.isActive) Calibration.slOPCodes.Add(opCode.OPCode, opCode);
                opCode = null;
            }
        }

        private void ProcessBinInit()
        {
            disassembled = false;
            
            disassemblyStartTime = DateTime.MinValue;
            disassemblyEndTime = DateTime.MinValue;
            unknownElemsStartTime = DateTime.MinValue;
            unknownElemsEndTime = DateTime.MinValue;
            outputStartTime = DateTime.MinValue;
            outputEndTime = DateTime.MinValue;

            Errors = null;
            
            if (Bank8 != null) Bank8.processBankInit();
            if (Bank1 != null) Bank1.processBankInit();
            if (Bank9 != null) Bank9.processBankInit();
            if (Bank0 != null) Bank0.processBankInit();
            
            Calibration.processCalibrationInit();

            S6x.processS6xInit();

            GC.Collect();
        }
        
        public void processBin()
        {
            ArrayList alErrors = null;

            ProgressStatus = 0;
            ProgressLabel = string.Empty;

            ProcessBinInit();

            disassemblyStartTime = DateTime.Now;

            Errors = null;
            alErrors = new ArrayList();
        
            // Eec Registers
            readDefEecRegisters();

            // OP Codes
            readDefOPCodes();

            // Starting with S6x Signature detection
            foreach (S6xSignature s6xSig in S6x.slProcessSignatures.Values) s6xSig.Information = null;
            
            //  On All Banks instead of doing it Bank after Bank, to be sure Matching Signatures are filled before cross Banks Calls
            if (Bank8 != null) Bank8.processSignatures(ref S6x);
            if (Bank1 != null) Bank1.processSignatures(ref S6x);
            if (Bank9 != null) Bank9.processSignatures(ref S6x);
            if (Bank0 != null) Bank0.processSignatures(ref S6x);

            // Signatures to be Ignored - Detected more than one time in one Bank or Globally
            foreach (MatchingSignature msMS in Calibration.slMatchingSignatures.Values)
            {
                if (msMS.S6xSignature.Ignore) Calibration.slUnMatchedSignatures.Remove(msMS.UniqueMatchingStartAddress);
            }

            // Bank Process
            if (Bank8 != null)
            {
                Bank8.processBank(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                ProgressStatus += 60 / (Calibration.Info.slBanksInfos.Count * 1);
                ProgressLabel += "Bank 8 Processed.";
            }
            if (Bank1 != null)
            {
                Bank1.processBank(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                ProgressStatus += 60 / (Calibration.Info.slBanksInfos.Count * 2);
                ProgressLabel += "\r\nBank 1 Processed.";
            }
            if (Bank9 != null)
            {
                Bank9.processBank(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                ProgressStatus += 60 / (Calibration.Info.slBanksInfos.Count * 3);
                ProgressLabel += "\r\nBank 9 Processed.";
            }
            if (Bank0 != null)
            {
                Bank0.processBank(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                ProgressStatus += 60 / (Calibration.Info.slBanksInfos.Count * 4);
                ProgressLabel += "\r\nBank 0 Processed.";
            }

            // 8061 Early - End of Calibration Part Definition
            //      Calibration Part always start at 0x2400
            //      End is calculated based on next First found Operation
            //      It applies also on its single fake RBase
            if (is8061 && isEarly && Bank8 != null)
            {
                int opeAddress = Calibration.AddressBankInt;
                while (opeAddress >= 0)
                {
                    opeAddress--;
                    if (Bank8.slOPs.ContainsKey(Tools.UniqueAddress(8, opeAddress))) break;
                }
                if (opeAddress >= 0)
                {
                    try
                    {
                        Calibration.AddressBankEndInt = ((Operation)Bank8.slOPs.GetByIndex(Bank8.slOPs.IndexOfKey(Tools.UniqueAddress(8, opeAddress)) + 1)).AddressInt - 1;
                        ((RBase)Calibration.slRbases.GetByIndex(0)).AddressBankEndInt = Calibration.AddressBankEndInt;
                    }
                    catch { }
                }
            }

            // Recursive Process to manage multiple steps after elements detection
            //  Includes :
            //      Operations Conflict Errors
            //      Find Additional Calibration Elements
            //      Calibration Elements Identification
            //      Calibration Elements Related Registers Identification (Tables and Functions)
            //      Find Non Calibration Elements (Table, Functions, Scalars, Structures outside Calibration Part) // NOT MANAGED FOR NOW
            //      Process Non Calibration Elements (Table, Functions, Scalars outside Calibration Part)
            processBinRecursive(ref alErrors);

            // Calibration Elements Related Registers Processing (Tables Scalers, Register Creations, ...)
            Calibration.processCalibrationElementsRegisters(ref S6x);

            // Calibration Elements Read, Dispatch and Translate
            Calibration.readCalibrationElements(ref S6x);

            // Elements Translation in Operations
            if (Bank8 != null) Bank8.processElemTranslations(ref S6x);
            if (Bank1 != null) Bank1.processElemTranslations(ref S6x);
            if (Bank9 != null) Bank9.processElemTranslations(ref S6x);
            if (Bank0 != null) Bank0.processElemTranslations(ref S6x);

            // Calibration Elements Errors
            processBinCalibCheck(ref alErrors);

            ProgressStatus += 20;
            ProgressLabel = "Banks Processed.";
            ProgressLabel += "\r\nCalibration Elements Processed.";

            // Call Translations
            Calibration.processCallTranslations(ref S6x);
            if (Bank8 != null) Bank8.processCallTranslations(ref S6x);
            if (Bank1 != null) Bank1.processCallTranslations(ref S6x);
            if (Bank9 != null) Bank9.processCallTranslations(ref S6x);
            if (Bank0 != null) Bank0.processCallTranslations(ref S6x);

            ProgressStatus += 10;
            ProgressLabel += "\r\nTranslations Processed.";

            unknownElemsStartTime = DateTime.Now;

            // Unknown Operation Store
            if (Bank8 != null) Bank8.readUnknownOpElements(ref S6x);
            if (Bank1 != null) Bank1.readUnknownOpElements(ref S6x);
            if (Bank9 != null) Bank9.readUnknownOpElements(ref S6x);
            if (Bank0 != null) Bank0.readUnknownOpElements(ref S6x);

            // Unknown Calibration Store
            Calibration.readUnknownCalibElements(ref S6x);

            unknownElemsEndTime = DateTime.Now;

            disassembled = true;
            disassemblyEndTime = DateTime.Now;

            ProgressStatus = 100;
            ProgressLabel = "Disassembly done.";

            if (alErrors.Count > 0)
            {
                ProgressLabel = "Disassembly done with errors.";
                Errors = (string[])alErrors.ToArray(typeof(string));
            }
            alErrors = null;
        }

        // Recursive Process to manage multiple steps after elements detection
        //  Includes :
        //      Operations Conflict Errors
        //      Find Additional Calibration Elements
        //      Calibration Elements Identification
        //      Calibration Elements Related Registers Identification (Tables and Functions)
        //      Find Non Calibration Elements (Table, Functions, Scalars, Structures outside Calibration Part) // NOT MANAGED FOR NOW
        //      Process Non Calibration Elements (Table, Functions, Scalars outside Calibration Part)
        private void processBinRecursive(ref ArrayList alErrors)
        {
            // Operations Conflict Errors
            processBinOpeConflictCheck(ref alErrors);

            // Find Additional Calibration Elements
            if (Bank8 != null) Bank8.findAdditionalCalibrationElements(ref S6x);
            if (Bank1 != null) Bank1.findAdditionalCalibrationElements(ref S6x);
            if (Bank9 != null) Bank9.findAdditionalCalibrationElements(ref S6x);
            if (Bank0 != null) Bank0.findAdditionalCalibrationElements(ref S6x);

            // Calibration Elements Identification
            if (Bank8 != null) Bank8.processCalibrationElements(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
            if (Bank1 != null) Bank1.processCalibrationElements(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
            if (Bank9 != null) Bank9.processCalibrationElements(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
            if (Bank0 != null) Bank0.processCalibrationElements(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);

            // Calibration Elements Related Registers Identification (Tables and Functions)
            if (Bank8 != null) Bank8.processCalibrationElementsRegisters(ref S6x);
            if (Bank1 != null) Bank1.processCalibrationElementsRegisters(ref S6x);
            if (Bank9 != null) Bank9.processCalibrationElementsRegisters(ref S6x);
            if (Bank0 != null) Bank0.processCalibrationElementsRegisters(ref S6x);

            // Find Non Calibration Elements (Table, Functions, Scalars, Structures outside Calibration Part) // NOT MANAGED FOR NOW
            if (Bank8 != null) Bank8.findOtherElements(ref S6x);
            if (Bank1 != null) Bank1.findOtherElements(ref S6x);
            if (Bank9 != null) Bank9.findOtherElements(ref S6x);
            if (Bank0 != null) Bank0.findOtherElements(ref S6x);

            // Process Non Calibration Elements (Table, Functions, Scalars outside Calibration Part)
            if (Bank8 != null) Bank8.processOtherElements(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
            if (Bank1 != null) Bank1.processOtherElements(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
            if (Bank9 != null) Bank9.processOtherElements(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
            if (Bank0 != null) Bank0.processOtherElements(ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);

            // New vectors have been identifed in elements (especially structures)
            // Additional Vectors provided from S6x Vectors Lists and Structures including Vectors
            bool newGeneratedOperations = false;
            foreach (Structure sStruct in Calibration.slExtStructures.Values)
            {
                if (sStruct.S6xStructure != null) continue; // Already processed
                if (sStruct.ParentStructure != null) continue; // Will be Processed at Parent Level
                if (!sStruct.isValid || sStruct.isEmpty) continue;
                if (!sStruct.containsOtherVectorsAddresses) continue;
                if (sStruct.Lines == null) continue;
                if (sStruct.Lines.Count == 0) continue;

                // SADBank.processOtherVectors returns true if new operations have been generated
                if (Bank8 != null) newGeneratedOperations |= Bank8.processOtherVectors(sStruct.GetOtherVectorAddresses(8), ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                if (Bank1 != null) newGeneratedOperations |= Bank1.processOtherVectors(sStruct.GetOtherVectorAddresses(1), ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                if (Bank9 != null) newGeneratedOperations |= Bank9.processOtherVectors(sStruct.GetOtherVectorAddresses(9), ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
                if (Bank0 != null) newGeneratedOperations |= Bank0.processOtherVectors(sStruct.GetOtherVectorAddresses(0), ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x);
            }

            if (newGeneratedOperations) processBinRecursive(ref alErrors);
        }

        // Operations Conflict Errors Mngt
        private void processBinOpeConflictCheck(ref ArrayList alErrors)
        {
            Operation prevOpe = null;
            string errorFormat = string.Empty;

            errorFormat = "Operations Conflict : {0,1} vs {1,1}";

            alErrors.Clear();
            
            if (Bank8 != null)
            {
                prevOpe = null;
                foreach (Operation ope in Bank8.slOPs.Values)
                {
                    if (prevOpe != null)
                    {
                        if (ope.AddressInt >= prevOpe.AddressInt && ope.AddressInt < prevOpe.AddressNextInt)
                        {
                            if (prevOpe.OriginalOp.StartsWith("fe") && (ope.AddressInt - prevOpe.AddressInt == 1)) ope.isFEConflict = true;
                            else  alErrors.Add(string.Format(errorFormat, prevOpe.UniqueAddressHex, ope.UniqueAddressHex));
                        }
                    }
                    prevOpe = ope;
                }
            }

            if (Bank1 != null)
            {
                prevOpe = null;
                foreach (Operation ope in Bank1.slOPs.Values)
                {
                    if (prevOpe != null)
                    {
                        if (ope.AddressInt >= prevOpe.AddressInt && ope.AddressInt < prevOpe.AddressNextInt)
                        {
                            if (prevOpe.OriginalOp.StartsWith("fe") && (ope.AddressInt - prevOpe.AddressInt == 1)) ope.isFEConflict = true;
                            else alErrors.Add(string.Format(errorFormat, prevOpe.UniqueAddressHex, ope.UniqueAddressHex));
                        }
                    }
                    prevOpe = ope;
                }
            }

            if (Bank9 != null)
            {
                prevOpe = null;
                foreach (Operation ope in Bank9.slOPs.Values)
                {
                    if (prevOpe != null)
                    {
                        if (ope.AddressInt >= prevOpe.AddressInt && ope.AddressInt < prevOpe.AddressNextInt)
                        {
                            if (prevOpe.OriginalOp.StartsWith("fe") && (ope.AddressInt - prevOpe.AddressInt == 1)) ope.isFEConflict = true;
                            else alErrors.Add(string.Format(errorFormat, prevOpe.UniqueAddressHex, ope.UniqueAddressHex));
                        }
                    }
                    prevOpe = ope;
                }
            }

            if (Bank0 != null)
            {
                prevOpe = null;
                foreach (Operation ope in Bank0.slOPs.Values)
                {
                    if (prevOpe != null)
                    {
                        if (ope.AddressInt >= prevOpe.AddressInt && ope.AddressInt < prevOpe.AddressNextInt)
                        {
                            if (prevOpe.OriginalOp.StartsWith("fe") && (ope.AddressInt - prevOpe.AddressInt == 1)) ope.isFEConflict = true;
                            else alErrors.Add(string.Format(errorFormat, prevOpe.UniqueAddressHex, ope.UniqueAddressHex));
                        }
                    }
                    prevOpe = ope;
                }
            }

            prevOpe = null;
        }

        // Operations Conflict Errors Mngt
        private void processBinCalibCheck(ref ArrayList alErrors)
        {
            CalibrationElement prevCalElem = null;
            string errorConflictFormat = string.Empty;
            string errorInvalidFormat = string.Empty;
            int countInvalidCalElemType = 0;
            string sampleInvalidCalElemType = string.Empty;

            errorConflictFormat = "Calibration Elements Conflict : {0,1} vs {1,1}";

            prevCalElem = null;
            foreach (CalibrationElement calElem in Calibration.slCalibrationElements.Values)
            {
                // Conflict
                if (prevCalElem != null)
                {
                    bool bAddConflictError = false;
                    if (calElem.AddressInt >= prevCalElem.AddressInt && calElem.AddressInt <= prevCalElem.AddressEndInt)
                    {
                        bAddConflictError = true;
                        // Possibly Included Structure (Essentially for Adder Structure)
                        if (calElem.isStructure) bAddConflictError = !(calElem.StructureElem.isVectorsList || calElem.StructureElem.Defaulted || prevCalElem.isStructure);
                    }
                    if (bAddConflictError) alErrors.Add(string.Format(errorConflictFormat, prevCalElem.UniqueAddressHex, calElem.UniqueAddressHex));
                }
                prevCalElem = calElem;

                // Issue
                if (calElem.isTypeIdentified)
                {
                    if (calElem.isScalar)
                    {
                        if (calElem.ScalarElem.Byte == false && calElem.ScalarElem.Word == false)
                        {
                            alErrors.Add(string.Format("Invalid Scalar (No Type) : {0,1}", calElem.UniqueAddressHex));
                        }
                    }
                    else if (calElem.isFunction)
                    {
                        if (calElem.FunctionElem.Lines.Length == 0)
                        {
                            alErrors.Add(string.Format("Invalid Function (No line) : {0,1}", calElem.UniqueAddressHex));
                        }
                    }
                    else if (calElem.isTable)
                    {
                        if (calElem.TableElem.ColsNumber <= 0)
                        {
                            alErrors.Add(string.Format("Invalid Table (No column) : {0,1}", calElem.UniqueAddressHex));
                        }
                        else if (calElem.TableElem.Lines.Length == 0)
                        {
                            alErrors.Add(string.Format("Invalid Table (No line) : {0,1}", calElem.UniqueAddressHex));
                        }
                    }
                }
                else
                {
                    countInvalidCalElemType++;
                    if (countInvalidCalElemType <= 5) sampleInvalidCalElemType += " " + calElem.Address;
                }
            }

            if (countInvalidCalElemType > 0)
            {
                alErrors.Add(string.Format("{0,1} Invalid Calibration Element(s) (Struct ?)\r\n    ex: {1,1}", countInvalidCalElemType, sampleInvalidCalElemType.Trim().Replace(" ", SADDef.GlobalSeparator.ToString())));
            }

        }

        public void processOutputText(string outputFilePath)
        {
            SortedList slElements = null;
            TextWriter txWriter = null;
            Element elem = null;
            ArrayList alErrors = null;
            object[] previousElementOutputResult = null;
            int bankNum = -1;

            ProgressStatus = 0;
            ProgressLabel = string.Empty;
        
            outputStartTime = DateTime.Now;

            Errors = null;
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
                    
                    try { slElements.Add(ope.UniqueAddressHex, new Element(ope, MergedType.Operation));}
                    catch { alErrors.Add(ope.UniqueAddressHex); } 

                }
                foreach (UnknownOpPart unkOpPart in Bank8.slUnknownOpParts.Values)
                {
                    foreach (UnknownOpPartLine uLine in unkOpPart.Lines)
                    {
                        try { slElements.Add(uLine.UniqueAddressHex, new Element(uLine, MergedType.UnknownOperationLine));}
                        catch { alErrors.Add(uLine.UniqueAddressHex); } 
                    }
                }
            }
            if (Bank1 != null)
            {
                foreach (ReservedAddress rAddress in Bank1.slReserved.Values)
                {
                    try { slElements.Add(rAddress.UniqueAddressHex, new Element(rAddress, MergedType.ReservedAddress));}
                    catch { alErrors.Add(rAddress.UniqueAddressHex); } 
                }
                // Managed by ReservedAddresses
                //foreach (Vector vect in Bank1.slIntVectors.Values) slElements.Add(vect.UniqueAddressHex, new Element(vect, MergedType.Vector));
                foreach (Operation ope in Bank1.slOPs.Values)
                {
                    try { slElements.Add(ope.UniqueAddressHex, new Element(ope, MergedType.Operation));}
                    catch { alErrors.Add(ope.UniqueAddressHex); } 
                }
                foreach (UnknownOpPart unkOpPart in Bank1.slUnknownOpParts.Values)
                {
                    foreach (UnknownOpPartLine uLine in unkOpPart.Lines)
                    {
                        try { slElements.Add(uLine.UniqueAddressHex, new Element(uLine, MergedType.UnknownOperationLine));}
                        catch { alErrors.Add(uLine.UniqueAddressHex); } 
                    }
                }
            }
            if (Bank9 != null)
            {
                foreach (ReservedAddress rAddress in Bank9.slReserved.Values)
                {
                    try { slElements.Add(rAddress.UniqueAddressHex, new Element(rAddress, MergedType.ReservedAddress));}
                    catch { alErrors.Add(rAddress.UniqueAddressHex); } 
                }
                // Managed by ReservedAddresses
                //foreach (Vector vect in Bank9.slIntVectors.Values) slElements.Add(vect.UniqueAddressHex, new Element(vect, MergedType.Vector));
                foreach (Operation ope in Bank9.slOPs.Values)
                {
                    try { slElements.Add(ope.UniqueAddressHex, new Element(ope, MergedType.Operation));}
                    catch { alErrors.Add(ope.UniqueAddressHex); } 
                }
                foreach (UnknownOpPart unkOpPart in Bank9.slUnknownOpParts.Values)
                {
                    foreach (UnknownOpPartLine uLine in unkOpPart.Lines)
                    {
                        try { slElements.Add(uLine.UniqueAddressHex, new Element(uLine, MergedType.UnknownOperationLine));}
                        catch { alErrors.Add(uLine.UniqueAddressHex); } 
                    }
                }
            }
            if (Bank0 != null)
            {
                foreach (ReservedAddress rAddress in Bank0.slReserved.Values)
                {
                    try { slElements.Add(rAddress.UniqueAddressHex, new Element(rAddress, MergedType.ReservedAddress));}
                    catch { alErrors.Add(rAddress.UniqueAddressHex); } 
                }
                // Managed by ReservedAddresses
                //foreach (Vector vect in Bank0.slIntVectors.Values) slElements.Add(vect.UniqueAddressHex, new Element(vect, MergedType.Vector));
                foreach (Operation ope in Bank0.slOPs.Values)
                {
                    try { slElements.Add(ope.UniqueAddressHex, new Element(ope, MergedType.Operation));}
                    catch { alErrors.Add(ope.UniqueAddressHex); } 
                }
                foreach (UnknownOpPart unkOpPart in Bank0.slUnknownOpParts.Values)
                {
                    foreach (UnknownOpPartLine uLine in unkOpPart.Lines)
                    {
                        try { slElements.Add(uLine.UniqueAddressHex, new Element(uLine, MergedType.UnknownOperationLine));}
                        catch { alErrors.Add(uLine.UniqueAddressHex); } 
                    }
                }
            }

            foreach (Vector vect in Calibration.slAdditionalVectors.Values)
            {
                try { slElements.Add(vect.UniqueSourceAddressHex, new Element(vect, MergedType.Vector));}
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

                try { slElements.Add(calElem.UniqueAddressHex, new Element(calElem, MergedType.CalibrationElement));}
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

            ProgressStatus += 50;
            ProgressLabel += "Elements Processed.";

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

                processOutputTextElement(ref txWriter, ref elem, ref previousElementOutputResult, ref S6x);

                elem = null;

                if (iElem > 0 && iElem % 0x4000 == 0) txWriter.Flush();
            }

            processOutputTextFooter(ref txWriter);

            txWriter.Close();
            txWriter.Dispose();
            txWriter = null;

            outputEndTime = DateTime.Now;

            ProgressStatus = 100;
            ProgressLabel = "Output done.";

            if (alErrors.Count > 0)
            {
                ProgressLabel = "Output done with errors.";
                Errors = (string[])alErrors.ToArray(typeof(string));
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
            else  col2 = string.Format("{0,30}", "None");
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

        private void processOutputTextElement(ref TextWriter txWriter, ref Element elem, ref object[] previousResult, ref SADS6x S6x)
        {
            MergedType prevType = MergedType.Unknown;
            Call cCall = null;
            Vector vVect = null;
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
                                sValues2 =  cCall.Label;
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
                        processOutputTextElementHeaderComments(ref txWriter, cCall.Comments);
                        cCall = null;
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
                            if (elem.CalElement.FunctionElem.isInputScaled) sValues = scalarLine.Scalars[0].ValueScaled(elem.CalElement.FunctionElem.getInputScaleExpression);
                            else sValues = scalarLine.Scalars[0].Value(10);
                            if (elem.CalElement.FunctionElem.isOutputScaled) sValues2 = scalarLine.Scalars[1].ValueScaled(elem.CalElement.FunctionElem.getOutputScaleExpression);
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
                                    if (elem.CalElement.TableElem.isCellsScaled) sValues2 += string.Format(SADDef.GlobalSeparator + "{0,9}", scalar.ValueScaled(elem.CalElement.TableElem.getCellsScaleExpression));
                                    else sValues2 += string.Format(SADDef.GlobalSeparator + "{0,6}", scalar.Value(10));
                                }
                                else
                                {
                                    sValues += string.Format(SADDef.GlobalSeparator + "{0,3}", scalar.Value(16));
                                    if (elem.CalElement.TableElem.isCellsScaled) sValues2 += string.Format(SADDef.GlobalSeparator + "{0,7}", scalar.ValueScaled(elem.CalElement.TableElem.getCellsScaleExpression));
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
                    processOutputTextElementLabel(ref txWriter, elem.ExtScalar.FullLabel);
                    processOutputTextElementOtherAddress(ref txWriter, elem.ExtScalar.UniqueAddress);
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
                        processOutputTextElementOtherAddress(ref txWriter, scalarLine.UniqueAddress);

                        if (elem.ExtFunction.isInputScaled) sValues = scalarLine.Scalars[0].ValueScaled(elem.ExtFunction.getInputScaleExpression);
                        else sValues = scalarLine.Scalars[0].Value(10);
                        if (elem.ExtFunction.isOutputScaled) sValues2 = scalarLine.Scalars[1].ValueScaled(elem.ExtFunction.getOutputScaleExpression);
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
                                if (elem.ExtTable.isCellsScaled) sValues2 += string.Format(SADDef.GlobalSeparator + "{0,9}", scalar.ValueScaled(elem.ExtTable.getCellsScaleExpression));
                                else sValues2 += string.Format(SADDef.GlobalSeparator + "{0,6}", scalar.Value(10));
                            }
                            else
                            {
                                sValues += string.Format(SADDef.GlobalSeparator + "{0,3}", scalar.Value(16));
                                if (elem.ExtTable.isCellsScaled) sValues2 += string.Format(SADDef.GlobalSeparator + "{0,7}", scalar.ValueScaled(elem.ExtTable.getCellsScaleExpression));
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

        public Operation[] getElementRelatedOps(string uniqueAddress, bool forElem)
        {
            Operation[] opsResult = null;
            Operation ope = null;
            bool unknownElement = true;
            int bankNum = -1;

            if (forElem)
            {
                if (unknownElement) if (Calibration.slCalibrationElements.ContainsKey(uniqueAddress)) unknownElement = false;
                if (unknownElement) if (Calibration.slExtTables.ContainsKey(uniqueAddress)) unknownElement = false;
                if (unknownElement) if (Calibration.slExtFunctions.ContainsKey(uniqueAddress)) unknownElement = false;
                if (unknownElement) if (Calibration.slExtScalars.ContainsKey(uniqueAddress)) unknownElement = false;
                if (unknownElement) if (Calibration.slExtStructures.ContainsKey(uniqueAddress)) unknownElement = false;

                if (unknownElement) return new Operation[] { };

                if (opsResult == null && Bank8 != null) opsResult = Bank8.getElementRelatedOps(uniqueAddress);
                if (opsResult == null && Bank1 != null) opsResult = Bank1.getElementRelatedOps(uniqueAddress);
                if (opsResult == null && Bank9 != null) opsResult = Bank9.getElementRelatedOps(uniqueAddress);
                if (opsResult == null && Bank0 != null) opsResult = Bank0.getElementRelatedOps(uniqueAddress);

                if (opsResult == null) return new Operation[] { };
            }
            else
            {
                // Vectors Lists => First Vector
                if (Calibration.slAdditionalVectors.ContainsKey(uniqueAddress)) uniqueAddress = ((Vector)Calibration.slAdditionalVectors[uniqueAddress]).UniqueAddress;
                
                bankNum = Convert.ToInt32(uniqueAddress.Substring(0, 1));
                switch (bankNum)
                {
                    case 8:
                        if (Bank8 == null) return new Operation[] { };
                        if (!Bank8.slOPs.ContainsKey(uniqueAddress)) return new Operation[] { };
                        ope = (Operation)Bank8.slOPs[uniqueAddress];
                        opsResult = Bank8.getFollowingOPs(ope.AddressInt, 32, 0, true, true, false, false, false, false, false, false);
                        ope = null;
                        break;
                    case 1:
                        if (Bank1 == null) return new Operation[] { };
                        if (!Bank1.slOPs.ContainsKey(uniqueAddress)) return new Operation[] { };
                        ope = (Operation)Bank1.slOPs[uniqueAddress];
                        opsResult = Bank1.getFollowingOPs(ope.AddressInt, 32, 0, true, true, false, false, false, false, false, false);
                        ope = null;
                        break;
                    case 9:
                        if (Bank9 == null) return new Operation[] { };
                        if (!Bank9.slOPs.ContainsKey(uniqueAddress)) return new Operation[] { };
                        ope = (Operation)Bank9.slOPs[uniqueAddress];
                        opsResult = Bank9.getFollowingOPs(ope.AddressInt, 32, 0, true, true, false, false, false, false, false, false);
                        ope = null;
                        break;
                    case 0:
                        if (Bank0 == null) return new Operation[] { };
                        if (!Bank0.slOPs.ContainsKey(uniqueAddress)) return new Operation[] { };
                        ope = (Operation)Bank0.slOPs[uniqueAddress];
                        opsResult = Bank0.getFollowingOPs(ope.AddressInt, 32, 0, true, true, false, false, false, false, false, false);
                        ope = null;
                        break;
                }
            }

            return opsResult;
        }

        public bool isCalibrationAddress(int addressInBin)
        {
            if (isLoaded)
            {
                if (addressInBin >= Calibration.AddressBinInt && addressInBin <= Calibration.AddressBinEndInt)
                {
                    return Calibration.isCalibrationAddress(addressInBin - Calibration.BankAddressBinInt);
                }
            }
            return false;
        }

        public bool isBankAddress(int bankNum, int address)
        {
            switch (bankNum)
            {
                case 8:
                    if (Bank8 != null) return address >= Bank8.AddressInternalInt && address <= Bank8.AddressInternalEndInt;
                    break;
                case 1:
                    if (Bank1 != null) return address >= Bank1.AddressInternalInt && address <= Bank1.AddressInternalEndInt;
                    break;
                case 9:
                    if (Bank9 != null) return address >= Bank9.AddressInternalInt && address <= Bank9.AddressInternalEndInt;
                    break;
                case 0:
                    if (Bank0 != null) return address >= Bank0.AddressInternalInt && address <= Bank0.AddressInternalEndInt;
                    break;
            }
            return false;
        }

        public int getBankNum(int addressBin)
        {
            if (Bank8 != null) if (addressBin >= Bank8.AddressBinInt && addressBin <= Bank8.AddressBinEndInt) return 8;
            if (Bank1 != null) if (addressBin >= Bank1.AddressBinInt && addressBin <= Bank1.AddressBinEndInt) return 1;
            if (Bank9 != null) if (addressBin >= Bank9.AddressBinInt && addressBin <= Bank9.AddressBinEndInt) return 9;
            if (Bank0 != null) if (addressBin >= Bank0.AddressBinInt && addressBin <= Bank0.AddressBinEndInt) return 0;
            return -1;
        }

        public int getBankBinAddress(int bankNum)
        {
            switch (bankNum)
            {
                case 8:
                    if (Bank8 != null) return Bank8.AddressBinInt;
                    break;
                case 1:
                    if (Bank1 != null) return Bank1.AddressBinInt;
                    break;
                case 9:
                    if (Bank9 != null) return Bank9.AddressBinInt;
                    break;
                case 0:
                    if (Bank0 != null) return Bank0.AddressBinInt;
                    break;
            }
            return -1;
        }

        public string getBytes(int startPos, int len) { return Tools.getBytes(startPos, len, ref arrBytes); }

        public string[] getBytesArray(int startPos, int len) { return Tools.getBytesArray(startPos, len, ref arrBytes); }
    }
}
