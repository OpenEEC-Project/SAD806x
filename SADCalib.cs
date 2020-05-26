using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace SAD806x
{
    public class SADCalib
    {
        public int AddressBinInt = -1;
        public int AddressBinEndInt = -1;

        public int AddressBankInt = -1;
        public int AddressBankEndInt = -1;

        public int BankNum = -1;

        public int BankAddressInt = -1;
        public int BankAddressEndInt = -1;

        public int BankAddressBinInt = -1;
        public int BankAddressBinEndInt = -1;

        public SADInfo Info = null;

        public SortedList slEecRegisters = null;
        public SortedList slOPCodes = null;

        public SortedList slRbases = null;
        public SortedList slRconst = null;
        public SortedList slRegisters = null;
        public SortedList slCalibrationElements = null;
        public SortedList slExtTables = null;
        public SortedList slExtFunctions = null;
        public SortedList slExtScalars = null;
        public SortedList slExtStructures = null;
        public SortedList slStructuresAnalysis = null;
        public SortedList slCalls = null;
        public SortedList slRoutines = null;
        public SortedList slAdditionalVectorsLists = null;
        public SortedList slAdditionalVectors = null;
        public SortedList slUnknownCalibParts = null;

        public SortedList slMatchingSignatures = null;
        public SortedList slUnMatchedSignatures = null;

        public ArrayList alTablesScalers = null;
        
        // Accelerators
        public ArrayList alMainCallsUniqueAddresses = null;
        public ArrayList alArgsCallsUniqueAddresses = null;
        public ArrayList alArgsModesCallsUniqueAddresses = null;
        public ArrayList alRoutinesCodesOpsAddresses = null;

        // Keep in Memory after load
        public ArrayList alLoadCalibrationElementsUniqueAddresses = null;
        public ArrayList alLoadExtTablesUniqueAddresses = null;
        public ArrayList alLoadExtFunctionsUniqueAddresses = null;
        public ArrayList alLoadExtScalarsUniqueAddresses = null;
        public ArrayList alLoadStructuresUniqueAddresses = null;
        public ArrayList alLoadCallsUniqueAddresses = null;
        public ArrayList alLoadRoutinesCodesOpsAddresses = null;

        private bool loaded = false;
        private bool is8061 = false;
        private bool isEarly = false;
        private bool isPilot = false;
        private string[] arrBytes = null;
        private string sBytes = string.Empty;

        public int Size { get { return AddressBankEndInt - AddressBankInt + 1; } }
        public int AddressInternalInt { get { return 0; } }
        public int AddressInternalEndInt { get { return Size - 1; } }

        public string AddressBin { get { return string.Format("{0:x5}", AddressBinInt); } }
        public string AddressBinEnd { get { return string.Format("{0:x5}", AddressBinEndInt); } }
        public string AddressBank { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressBankInt); } }
        public string AddressBankEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressBankEndInt); } }
        public string AddressInternal { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInternalInt); } }
        public string AddressInternalEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInternalEndInt); } }

        public bool isLoaded { get { return loaded; } }

        public SADCalib()
        {
            slRconst = new SortedList();
            slRegisters = new SortedList();
            slCalibrationElements = new SortedList();
            slExtTables = new SortedList();
            slExtFunctions = new SortedList();
            slExtScalars = new SortedList();
            slExtStructures = new SortedList();
            slStructuresAnalysis = new SortedList();
            slCalls = new SortedList();
            slRoutines = new SortedList();
            slAdditionalVectorsLists = new SortedList();
            slAdditionalVectors = new SortedList();
            slUnknownCalibParts = new SortedList();

            slMatchingSignatures = new SortedList();
            slUnMatchedSignatures = new SortedList();

            alTablesScalers = new ArrayList();

            alMainCallsUniqueAddresses = new ArrayList();
            alArgsCallsUniqueAddresses = new ArrayList();
            alArgsModesCallsUniqueAddresses = new ArrayList();
            alRoutinesCodesOpsAddresses = new ArrayList();

            alLoadCalibrationElementsUniqueAddresses = new ArrayList();
            alLoadExtTablesUniqueAddresses = new ArrayList();
            alLoadExtFunctionsUniqueAddresses = new ArrayList();
            alLoadExtScalarsUniqueAddresses = new ArrayList();
            alLoadStructuresUniqueAddresses = new ArrayList();
            alLoadCallsUniqueAddresses = new ArrayList();
            alLoadRoutinesCodesOpsAddresses = new ArrayList();
        }
        
        public void Load(SortedList slGivenRbases, ref SADBank calibBank, bool bIs8061, bool bIsEarly, bool bIsPilot, ref string[] arrBinBytes)
        {
            int minRbasesAddress = Int32.MaxValue;
            int maxRbasesAddress = Int32.MinValue;
            string minRbaseCodeByAddress = string.Empty;
            string maxRbaseCodeByAddress = string.Empty;

            slRbases = slGivenRbases;

            if (calibBank == null) return;
            
            if (slRbases.Count == 0)
            {
                minRbasesAddress = -1;
                maxRbasesAddress = -1;

                AddressBankInt = calibBank.AddressInternalInt;
                AddressBankEndInt = calibBank.AddressInternalEndInt;

                AddressBinInt = calibBank.AddressBinInt;
                AddressBinEndInt = calibBank.AddressBinEndInt;
            }
            else
            {
                foreach (RBase rBase in slRbases.Values)
                {
                    if (minRbasesAddress > rBase.AddressBankInt)
                    {
                        minRbasesAddress = rBase.AddressBankInt;
                        minRbaseCodeByAddress = rBase.Code;
                    }
                    if (maxRbasesAddress < rBase.AddressBankEndInt)
                    {
                        maxRbasesAddress = rBase.AddressBankEndInt;
                        maxRbaseCodeByAddress = rBase.Code;
                    }
                }
                AddressBankInt = minRbasesAddress;
                AddressBankEndInt = maxRbasesAddress;

                AddressBinInt = calibBank.AddressBinInt + AddressBankInt;
                AddressBinEndInt = calibBank.AddressBinInt + maxRbasesAddress;
            }

            BankNum = calibBank.Num;

            BankAddressInt = calibBank.AddressInternalInt;
            BankAddressEndInt = calibBank.AddressInternalEndInt;

            BankAddressBinInt = calibBank.AddressBinInt;
            BankAddressBinEndInt = calibBank.AddressBinEndInt;

            is8061 = bIs8061;
            isEarly = bIsEarly;
            isPilot = bIsPilot;

            arrBytes = new string[Size];
            for (int iPos = 0; iPos < Size; iPos++) arrBytes[iPos] = arrBinBytes[AddressBinInt + iPos];

            sBytes = string.Join("", arrBytes);

            // RBase End Address Calibration Element, Except for Early 8061
            if (!is8061 || !isEarly)
            {
                foreach (RBase rBase in slRbases.Values)
                {
                    CalibrationElement calElem = new CalibrationElement(BankNum, rBase.Code);
                    calElem.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code;
                    calElem.AddressInt = rBase.AddressBankInt;
                    calElem.AddressBinInt = calElem.AddressInt + BankAddressBinInt;
                    calElem.ScalarElem = new Scalar();
                    calElem.ScalarElem.BankNum = calElem.BankNum;
                    calElem.ScalarElem.AddressInt = calElem.AddressInt;
                    calElem.ScalarElem.AddressBinInt = calElem.AddressBinInt;
                    calElem.ScalarElem.RBase = calElem.RBase;
                    calElem.ScalarElem.RBaseCalc = calElem.RBaseCalc;
                    calElem.ScalarElem.Byte = false;
                    calElem.ScalarElem.Word = true;
                    calElem.ScalarElem.Signed = false;
                    calElem.ScalarElem.InitialValue = rBase.AddressBankEnd.Substring(2, 2) + SADDef.GlobalSeparator + rBase.AddressBankEnd.Substring(0, 2);
                    calElem.ScalarElem.ValueInt = rBase.AddressBankEndInt + SADDef.EecBankStartAddress;
                    calElem.ScalarElem.Label = SADDef.LongLabelScalarRBaseEndNextAdr.Replace("%1%", calElem.RBaseCalc);
                    calElem.ScalarElem.ShortLabel = SADDef.ShortLabelScalarRBaseEndNextAdr.Replace("%1%", calElem.RBaseCalc);

                    slCalibrationElements.Add(calElem.UniqueAddress, calElem);
                    alLoadCalibrationElementsUniqueAddresses.Add(calElem.UniqueAddress);
                    calElem = null;
                }
            }

            loaded = true;
        }

        public void processCalibrationInit()
        {
            SortedList slLoadCalibrationElements = new SortedList();
            SortedList slLoadExtTables = new SortedList();
            SortedList slLoadExtFunctions = new SortedList();
            SortedList slLoadExtScalars = new SortedList();
            SortedList slLoadExtStructures = new SortedList();
            SortedList slLoadCalls = new SortedList();

            foreach (string uniqueAddress in alLoadCalibrationElementsUniqueAddresses) slLoadCalibrationElements.Add(uniqueAddress, slCalibrationElements[uniqueAddress]);
            foreach (string uniqueAddress in alLoadCallsUniqueAddresses) slLoadCalls.Add(uniqueAddress, slCalls[uniqueAddress]);
            foreach (string uniqueAddress in alLoadExtFunctionsUniqueAddresses) slLoadExtFunctions.Add(uniqueAddress, slExtFunctions[uniqueAddress]);
            foreach (string uniqueAddress in alLoadExtScalarsUniqueAddresses) slLoadExtScalars.Add(uniqueAddress, slExtScalars[uniqueAddress]);
            foreach (string uniqueAddress in alLoadExtTablesUniqueAddresses) slLoadExtTables.Add(uniqueAddress, slExtTables[uniqueAddress]);
            foreach (string uniqueAddress in alLoadStructuresUniqueAddresses) slLoadExtStructures.Add(uniqueAddress, slExtStructures[uniqueAddress]);

            slCalibrationElements = new SortedList();
            slExtTables = new SortedList();
            slExtFunctions = new SortedList();
            slExtScalars = new SortedList();
            slExtStructures = new SortedList();
            slStructuresAnalysis = new SortedList();
            slCalls = new SortedList();
            slRoutines = new SortedList();
            slAdditionalVectorsLists = new SortedList();
            slAdditionalVectors = new SortedList();
            slUnknownCalibParts = new SortedList();

            slMatchingSignatures = new SortedList();
            slUnMatchedSignatures = new SortedList();

            alTablesScalers = new ArrayList();

            alMainCallsUniqueAddresses = new ArrayList();
            alArgsCallsUniqueAddresses = new ArrayList();
            alArgsModesCallsUniqueAddresses = new ArrayList();
            alRoutinesCodesOpsAddresses = new ArrayList();

            foreach (string uniqueAddress in alLoadCalibrationElementsUniqueAddresses) slCalibrationElements.Add(uniqueAddress, slLoadCalibrationElements[uniqueAddress]);
            foreach (string uniqueAddress in alLoadCallsUniqueAddresses) slCalls.Add(uniqueAddress, slLoadCalls[uniqueAddress]);
            foreach (string uniqueAddress in alLoadExtFunctionsUniqueAddresses) slExtFunctions.Add(uniqueAddress, slLoadExtFunctions[uniqueAddress]);
            foreach (string uniqueAddress in alLoadExtScalarsUniqueAddresses) slExtScalars.Add(uniqueAddress, slLoadExtScalars[uniqueAddress]);
            foreach (string uniqueAddress in alLoadExtTablesUniqueAddresses) slExtTables.Add(uniqueAddress, slLoadExtTables[uniqueAddress]);
            foreach (string uniqueAddress in alLoadStructuresUniqueAddresses) slExtStructures.Add(uniqueAddress, slLoadExtStructures[uniqueAddress]);
            foreach (object rcoRco in alLoadRoutinesCodesOpsAddresses) alRoutinesCodesOpsAddresses.Add(rcoRco);

            slLoadCalibrationElements = null;
            slLoadExtTables = null;
            slLoadExtFunctions = null;
            slLoadExtScalars = null;
            slLoadExtStructures = null;
            slLoadCalls = null;

            GC.Collect();
        }

        public bool isLoadCreated(string uniqueAddress)
        {
            if (alLoadCalibrationElementsUniqueAddresses.Contains(uniqueAddress)) return true;
            if (alLoadCallsUniqueAddresses.Contains(uniqueAddress)) return true;
            if (alLoadExtFunctionsUniqueAddresses.Contains(uniqueAddress)) return true;
            if (alLoadExtScalarsUniqueAddresses.Contains(uniqueAddress)) return true;
            if (alLoadExtTablesUniqueAddresses.Contains(uniqueAddress)) return true;
            if (alLoadStructuresUniqueAddresses.Contains(uniqueAddress)) return true;

            return false;
        }

        public void processCalibrationElementsRegisters(ref SADS6x S6x)
        {
            SortedList slUpdatedRegisters = new SortedList();

            // Tables Scalers Creation and Registers Creation
            foreach (CalibrationElement calElem in slCalibrationElements.Values)
            {
                if (calElem.isTable)
                {
                    foreach (RoutineCallInfoTable ciCi in calElem.TableElem.RoutinesCallsInfos)
                    {
                        int tScalerIndex = -1;
                        TableScaler tScaler = null;
                        Register rReg = null;
                        if (ciCi.ColsScalerRegister != string.Empty || ciCi.ColsScalerFunctionUniqueAddress != string.Empty)
                        {
                            tScalerIndex = getTablesScalersIndex(ciCi.ColsScalerRegister, ciCi.ColsScalerFunctionUniqueAddress);
                            if (tScalerIndex >= 0)
                            {
                                tScaler = (TableScaler)alTablesScalers[tScalerIndex];
                                tScaler.addRegister(ciCi.ColsScalerRegister);
                                tScaler.addFunction(ciCi.ColsScalerFunctionUniqueAddress);
                                tScaler.addColsScaledTable(calElem.TableElem.UniqueAddress);
                            }
                            else
                            {
                                tScaler = new TableScaler();
                                tScaler.addRegister(ciCi.ColsScalerRegister);
                                tScaler.addFunction(ciCi.ColsScalerFunctionUniqueAddress);
                                tScaler.addColsScaledTable(calElem.TableElem.UniqueAddress);
                                alTablesScalers.Add(tScaler);
                            }
                            tScaler = null;
                        }
                        if (ciCi.RowsScalerRegister != string.Empty || ciCi.RowsScalerFunctionUniqueAddress != string.Empty)
                        {
                            tScalerIndex = getTablesScalersIndex(ciCi.RowsScalerRegister, ciCi.RowsScalerFunctionUniqueAddress);
                            if (tScalerIndex >= 0)
                            {
                                tScaler = (TableScaler)alTablesScalers[tScalerIndex];
                                tScaler.addRegister(ciCi.RowsScalerRegister);
                                tScaler.addFunction(ciCi.RowsScalerFunctionUniqueAddress);
                                tScaler.addRowsScaledTable(calElem.TableElem.UniqueAddress);
                            }
                            else
                            {
                                tScaler = new TableScaler();
                                tScaler.addRegister(ciCi.RowsScalerRegister);
                                tScaler.addFunction(ciCi.RowsScalerFunctionUniqueAddress);
                                tScaler.addRowsScaledTable(calElem.TableElem.UniqueAddress);
                                alTablesScalers.Add(tScaler);
                            }
                            tScaler = null;
                        }
                        if (ciCi.ColsScalerRegister != string.Empty)
                        {
                            rReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCi.ColsScalerRegister)];
                            if (rReg == null)
                            {
                                rReg = new Register(ciCi.ColsScalerRegister);
                                slRegisters.Add(rReg.UniqueAddress, rReg);
                            }
                            if (rReg.Links == null) rReg.Links = new RegisterLinks();
                            rReg.Links.addTableLink(calElem.TableElem.UniqueAddress, true, false, false, false);
                            if (!slUpdatedRegisters.ContainsKey(rReg.UniqueAddress)) slUpdatedRegisters.Add(rReg.UniqueAddress, rReg);
                            rReg = null;
                        }
                        if (ciCi.RowsScalerRegister != string.Empty)
                        {
                            rReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCi.RowsScalerRegister)];
                            if (rReg == null)
                            {
                                rReg = new Register(ciCi.RowsScalerRegister);
                                slRegisters.Add(rReg.UniqueAddress, rReg);
                            }
                            if (rReg.Links == null) rReg.Links = new RegisterLinks();
                            rReg.Links.addTableLink(calElem.TableElem.UniqueAddress, false, true, false, false);
                            if (!slUpdatedRegisters.ContainsKey(rReg.UniqueAddress)) slUpdatedRegisters.Add(rReg.UniqueAddress, rReg);
                            rReg = null;
                        }
                        if (ciCi.OutputRegister != string.Empty)
                        {
                            rReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCi.OutputRegister)];
                            if (rReg == null)
                            {
                                rReg = new Register(ciCi.OutputRegister);
                                slRegisters.Add(rReg.UniqueAddress, rReg);
                            }
                            if (rReg.Links == null) rReg.Links = new RegisterLinks();
                            rReg.Links.addTableLink(calElem.TableElem.UniqueAddress, false, false, true, false);
                            if (!slUpdatedRegisters.ContainsKey(rReg.UniqueAddress)) slUpdatedRegisters.Add(rReg.UniqueAddress, rReg);
                            rReg = null;
                        }
                        if (ciCi.OutputRegisterByte != string.Empty)
                        {
                            rReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCi.OutputRegisterByte)];
                            if (rReg == null)
                            {
                                rReg = new Register(ciCi.OutputRegisterByte);
                                slRegisters.Add(rReg.UniqueAddress, rReg);
                            }
                            if (rReg.Links == null) rReg.Links = new RegisterLinks();
                            rReg.Links.addTableLink(calElem.TableElem.UniqueAddress, false, false, false, true);
                            if (!slUpdatedRegisters.ContainsKey(rReg.UniqueAddress)) slUpdatedRegisters.Add(rReg.UniqueAddress, rReg);
                            rReg = null;
                        }
                    }
                }
            }

            // Tables Scalers Functions matching and Missing Register Creation
            foreach (CalibrationElement calElem in slCalibrationElements.Values)
            {
                if (calElem.isFunction)
                {
                    foreach (RoutineCallInfoFunction ciCi in calElem.FunctionElem.RoutinesCallsInfos)
                    {
                        int tScalerIndex = -1;
                        TableScaler tScaler = null;
                        Register rReg = null;
                        if (ciCi.OutputRegister != string.Empty)
                        {
                            tScalerIndex = getTablesScalersIndex(ciCi.OutputRegister, string.Empty);
                            if (tScalerIndex >= 0)
                            {
                                tScaler = (TableScaler)alTablesScalers[tScalerIndex];
                                tScaler.addFunction(calElem.FunctionElem.UniqueAddress);
                                tScaler = null;
                            }
                            rReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCi.OutputRegister)];
                            if (rReg == null)
                            {
                                rReg = new Register(ciCi.OutputRegister);
                                slRegisters.Add(rReg.UniqueAddress, rReg);
                            }
                            // Unit part
                            if (rReg.S6xRegister != null) calElem.FunctionElem.OutputUnits = rReg.S6xRegister.Units;
                            if (tScalerIndex >= 0 && calElem.FunctionElem.OutputUnits == string.Empty) calElem.FunctionElem.OutputUnits = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.SCALER_SAMPLE).Units;
                            // Links part
                            if (rReg.Links == null) rReg.Links = new RegisterLinks();
                            rReg.Links.addFunctionLink(calElem.FunctionElem.UniqueAddress, false, true, false);
                            if (!slUpdatedRegisters.ContainsKey(rReg.UniqueAddress)) slUpdatedRegisters.Add(rReg.UniqueAddress, rReg);
                            rReg = null;
                        }
                        if (ciCi.OutputRegisterByte != string.Empty)
                        {
                            tScalerIndex = getTablesScalersIndex(ciCi.OutputRegisterByte, string.Empty);
                            if (tScalerIndex >= 0)
                            {
                                tScaler = (TableScaler)alTablesScalers[tScalerIndex];
                                tScaler.addFunction(calElem.FunctionElem.UniqueAddress);
                                tScaler = null;
                            }
                            rReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCi.OutputRegisterByte)];
                            if (rReg == null)
                            {
                                rReg = new Register(ciCi.OutputRegisterByte);
                                slRegisters.Add(rReg.UniqueAddress, rReg);
                            }
                            // Unit part
                            if (rReg.S6xRegister != null) calElem.FunctionElem.OutputUnits = rReg.S6xRegister.Units;
                            if (tScalerIndex >= 0 && calElem.FunctionElem.OutputUnits == string.Empty) calElem.FunctionElem.OutputUnits = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.SCALER_SAMPLE).Units;
                            // Links part
                            if (rReg.Links == null) rReg.Links = new RegisterLinks();
                            rReg.Links.addFunctionLink(calElem.FunctionElem.UniqueAddress, false, false, true);
                            if (!slUpdatedRegisters.ContainsKey(rReg.UniqueAddress)) slUpdatedRegisters.Add(rReg.UniqueAddress, rReg);
                            rReg = null;
                        }
                        if (ciCi.InputRegister != string.Empty)
                        {
                            rReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCi.InputRegister)];
                            if (rReg == null)
                            {
                                rReg = new Register(ciCi.InputRegister);
                                slRegisters.Add(rReg.UniqueAddress, rReg);
                            }
                            // Unit part
                            if (rReg.S6xRegister != null) calElem.FunctionElem.InputUnits = rReg.S6xRegister.Units;
                            // Links part
                            if (rReg.Links == null) rReg.Links = new RegisterLinks();
                            rReg.Links.addFunctionLink(calElem.FunctionElem.UniqueAddress, true, false, false);
                            if (!slUpdatedRegisters.ContainsKey(rReg.UniqueAddress)) slUpdatedRegisters.Add(rReg.UniqueAddress, rReg);
                            rReg = null;
                        }
                    }
                }
            }

            // S6x Registers Creation, Information Update and Registers Links creation
            foreach (Register rReg in slUpdatedRegisters.Values)
            {
                if (rReg.S6xRegister == null) rReg.S6xRegister = (S6xRegister)S6x.slProcessRegisters[rReg.UniqueAddress];
                if (rReg.S6xRegister == null)
                {
                    rReg.S6xRegister = new S6xRegister(rReg.Address);
                    rReg.S6xRegister.isRBase = false;
                    rReg.S6xRegister.isRConst = false;
                    rReg.S6xRegister.AutoConstValue = false;
                    rReg.S6xRegister.ConstValue = null;
                    rReg.S6xRegister.Label = Tools.RegisterInstruction(rReg.Address);
                    S6x.slProcessRegisters.Add(rReg.UniqueAddress, rReg.S6xRegister);
                    if (S6x.slRegisters.ContainsKey(rReg.UniqueAddress)) S6x.slRegisters[rReg.UniqueAddress] = rReg.S6xRegister;
                    else S6x.slRegisters.Add(rReg.UniqueAddress, rReg.S6xRegister);
                }
                if (rReg.RBase != null)
                {
                    rReg.S6xRegister.isRBase = true;
                    rReg.S6xRegister.ConstValue = Convert.ToString(SADDef.EecBankStartAddress + rReg.RBase.AddressBankInt, 16);
                    rReg.S6xRegister.AutoConstValue = true;
                }
                else if (rReg.RConst != null)
                {
                    rReg.S6xRegister.isRConst = true;
                    rReg.S6xRegister.ConstValue = rReg.RConst.Value;
                    rReg.S6xRegister.AutoConstValue = true;
                }
                // Creating Registers Links
                // Links are already initialized
                foreach (RegisterFunctionLink rfLink in rReg.Links.FunctionsLinks.Values)
                {
                    CalibrationElement calElem = (CalibrationElement)slCalibrationElements[rfLink.FunctionUniqueAddress];
                    if (calElem == null) continue;
                    if (!calElem.isFunction) continue;
                    if (calElem.FunctionElem.RoutinesCallsInfos == null) continue;
                    foreach (RoutineCallInfoFunction ciCI in calElem.FunctionElem.RoutinesCallsInfos)
                    {
                        int tScalerIndex = -1;
                        Register lReg = null;

                        if (rfLink.isInputRegister && ciCI.InputRegister == rReg.Address)
                        {
                            if (ciCI.OutputRegister != string.Empty)
                            {
                                tScalerIndex = getTablesScalersIndex(ciCI.OutputRegister, string.Empty);
                                lReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCI.OutputRegister)];
                                if (lReg != null) rReg.Links.addRegisterLink(lReg.UniqueAddress, false, true, calElem.UniqueAddress, tScalerIndex >= 0);
                                lReg = null;
                            }
                            if (ciCI.OutputRegisterByte != string.Empty)
                            {
                                tScalerIndex = getTablesScalersIndex(ciCI.OutputRegisterByte, string.Empty);
                                lReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCI.OutputRegisterByte)];
                                if (lReg != null) rReg.Links.addRegisterLink(lReg.UniqueAddress, false, true, calElem.UniqueAddress, tScalerIndex >= 0);
                                lReg = null;
                            }
                        }
                        if (rfLink.isOutputRegister && ciCI.OutputRegister == rReg.Address)
                        {
                            if (ciCI.InputRegister != string.Empty)
                            {
                                tScalerIndex = getTablesScalersIndex(ciCI.OutputRegister, string.Empty);
                                lReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCI.InputRegister)];
                                if (lReg != null) rReg.Links.addRegisterLink(lReg.UniqueAddress, true, false, calElem.UniqueAddress, tScalerIndex >= 0);
                                lReg = null;
                            }
                        }
                        if (rfLink.isOutputRegisterByte && ciCI.OutputRegisterByte == rReg.Address)
                        {
                            if (ciCI.InputRegister != string.Empty)
                            {
                                tScalerIndex = getTablesScalersIndex(ciCI.OutputRegisterByte, string.Empty);
                                lReg = (Register)slRegisters[Tools.RegisterUniqueAddress(ciCI.InputRegister)];
                                if (lReg != null) rReg.Links.addRegisterLink(lReg.UniqueAddress, true, false, calElem.UniqueAddress, tScalerIndex >= 0);
                                lReg = null;
                            }
                        }
                    }

                    if (rReg.S6xRegister.Units == null || rReg.S6xRegister.Units == string.Empty)
                    {
                        foreach (RegisterRegisterLink rrLink in rReg.Links.RegistersLinks.Values)
                        {
                            if (rrLink.isSourceScaler) rReg.S6xRegister.Units = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.SCALER_SAMPLE).Units;
                        }
                    }
                }
            }
            slUpdatedRegisters = null;

            // Applying Scalers on Tables
            foreach (TableScaler tScaler in alTablesScalers)
            {
                if (tScaler.InputFunctionsUniqueAddresses.Count > 0)
                {
                    // Scalers Assignment
                    foreach (string tableUniqueAddress in tScaler.ColsScaledTablesUniqueAddresses)
                    {
                        CalibrationElement calElem = (CalibrationElement)slCalibrationElements[tableUniqueAddress];
                        if (calElem != null)
                        {
                            if (calElem.isTable)
                            {
                                // One Function only is choosen, the first one even if more are available
                                calElem.TableElem.ColsScalerUniqueAddress = tScaler.InputFunctionsUniqueAddresses[0].ToString();
                            }
                        }
                        calElem = null;
                    }
                    foreach (string tableUniqueAddress in tScaler.RowsScaledTablesUniqueAddresses)
                    {
                        CalibrationElement calElem = (CalibrationElement)slCalibrationElements[tableUniqueAddress];
                        if (calElem != null)
                        {
                            if (calElem.isTable)
                            {
                                // One Function only is choosen, the first one even if more are available
                                calElem.TableElem.RowsScalerUniqueAddress = tScaler.InputFunctionsUniqueAddresses[0].ToString();
                            }
                        }
                        calElem = null;
                    }
                }
            }
        }

        public void readCalibrationElements(ref SADS6x S6x)
        {
            CalibrationElement calElem = null;
            TableScaler tScaler = null;
            string currentRbaseCode = string.Empty;
            int currentRbaseEndAddress = -1;
            int tScalerIndex = -1;

            // S6x Elements Added to Calibration Element or Overwrite them
            foreach (S6xTable s6xTable in S6x.slProcessTables.Values)
            {
                if (!s6xTable.isCalibrationElement) continue;
                if (s6xTable.BankNum != BankNum) continue;

                calElem = (CalibrationElement)slCalibrationElements[s6xTable.UniqueAddress];
                if (calElem == null) calElem = new CalibrationElement(BankNum, s6xTable.UniqueAddressHex);
                
                // No Conflict => Update with S6x Information, will be managed at read
                if (calElem.isTable)
                {
                    calElem.TableElem.S6xTable = s6xTable;
                    continue;
                }

                // Conflict Mngt
                calElem.ScalarElem = null;
                calElem.FunctionElem = null;
                calElem.StructureElem = null;
                calElem.TableElem = null;

                calElem.RBaseCalc = s6xTable.UniqueAddressHex;
                calElem.AddressInt = s6xTable.AddressInt - BankAddressInt;
                calElem.AddressBinInt = calElem.AddressInt + BankAddressBinInt;
                calElem.TableElem = new Table();
                calElem.TableElem.S6xTable = s6xTable;
                calElem.TableElem.BankNum = calElem.BankNum;
                calElem.TableElem.AddressInt = calElem.AddressInt;
                calElem.TableElem.AddressBinInt = calElem.AddressBinInt;
                calElem.TableElem.RBase = calElem.RBase;
                calElem.TableElem.RBaseCalc = calElem.RBaseCalc;
                calElem.TableElem.WordOutput = s6xTable.WordOutput;
                // S6x Information will be managed at read

                if (!slCalibrationElements.ContainsKey(calElem.UniqueAddress)) slCalibrationElements.Add(calElem.UniqueAddress, calElem);

                // Table Scalers Mngt
                if (s6xTable.ColsScalerAddress != null && s6xTable.ColsScalerAddress != string.Empty)
                {
                    tScalerIndex = getTablesScalersIndex(string.Empty, s6xTable.ColsScalerAddress);
                    if (tScalerIndex >= 0)
                    {
                        tScaler = (TableScaler)alTablesScalers[tScalerIndex];
                        tScaler.addFunction(s6xTable.ColsScalerAddress);
                        tScaler.addColsScaledTable(s6xTable.UniqueAddress);
                        alTablesScalers[tScalerIndex] = tScaler;
                    }
                    else
                    {
                        tScaler = new TableScaler();
                        tScaler.addFunction(s6xTable.ColsScalerAddress);
                        tScaler.addColsScaledTable(s6xTable.UniqueAddress);
                        alTablesScalers.Add(tScaler);
                    }
                    tScaler = null;
                }
                if (s6xTable.RowsScalerAddress != null && s6xTable.RowsScalerAddress != string.Empty)
                {
                    tScalerIndex = getTablesScalersIndex(string.Empty, s6xTable.RowsScalerAddress);
                    if (tScalerIndex >= 0)
                    {
                        tScaler = (TableScaler)alTablesScalers[tScalerIndex];
                        tScaler.addFunction(s6xTable.RowsScalerAddress);
                        tScaler.addRowsScaledTable(s6xTable.UniqueAddress);
                        alTablesScalers[tScalerIndex] = tScaler;
                    }
                    else
                    {
                        tScaler = new TableScaler();
                        tScaler.addFunction(s6xTable.RowsScalerAddress);
                        tScaler.addRowsScaledTable(s6xTable.UniqueAddress);
                        alTablesScalers.Add(tScaler);
                    }
                    tScaler = null;
                }
                calElem = null;
            }

            foreach (S6xFunction s6xFunction in S6x.slProcessFunctions.Values)
            {
                if (!s6xFunction.isCalibrationElement) continue;
                if (s6xFunction.BankNum != BankNum) continue;

                calElem = (CalibrationElement)slCalibrationElements[s6xFunction.UniqueAddress];
                if (calElem == null) calElem = new CalibrationElement(BankNum, s6xFunction.UniqueAddressHex);

                // No Conflict => Update with S6x Information, will be managed at read
                if (calElem.isFunction)
                {
                    calElem.FunctionElem.S6xFunction = s6xFunction;
                    continue;
                }

                // Conflict Mngt
                calElem.ScalarElem = null;
                calElem.FunctionElem = null;
                calElem.StructureElem = null;
                calElem.TableElem = null;

                calElem.RBaseCalc = s6xFunction.UniqueAddressHex;
                calElem.AddressInt = s6xFunction.AddressInt - BankAddressInt;
                calElem.AddressBinInt = calElem.AddressInt + BankAddressBinInt;
                calElem.FunctionElem = new Function();
                calElem.FunctionElem.S6xFunction = s6xFunction;
                calElem.FunctionElem.BankNum = calElem.BankNum;
                calElem.FunctionElem.AddressInt = calElem.AddressInt;
                calElem.FunctionElem.AddressBinInt = calElem.AddressBinInt;
                calElem.FunctionElem.RBase = calElem.RBase;
                calElem.FunctionElem.RBaseCalc = calElem.RBaseCalc;
                // S6x Information will be managed at read

                if (!slCalibrationElements.ContainsKey(calElem.UniqueAddress)) slCalibrationElements.Add(calElem.UniqueAddress, calElem);

                calElem = null;
            }

            foreach (S6xScalar s6xScalar in S6x.slProcessScalars.Values)
            {
                if (!s6xScalar.isCalibrationElement) continue;
                if (s6xScalar.BankNum != BankNum) continue;

                calElem = (CalibrationElement)slCalibrationElements[s6xScalar.UniqueAddress];
                if (calElem == null) calElem = new CalibrationElement(BankNum, s6xScalar.UniqueAddressHex);

                // No Conflict => Update with S6x Information, will be managed at read
                if (calElem.isScalar)
                {
                    calElem.ScalarElem.S6xScalar = s6xScalar;
                    continue;
                }

                // Conflict Mngt
                calElem.ScalarElem = null;
                calElem.FunctionElem = null;
                calElem.StructureElem = null;
                calElem.TableElem = null;
                
                calElem.RBaseCalc = s6xScalar.UniqueAddressHex;
                calElem.AddressInt = s6xScalar.AddressInt - BankAddressInt;
                calElem.AddressBinInt = calElem.AddressInt + BankAddressBinInt;
                calElem.ScalarElem = new Scalar();
                calElem.ScalarElem.S6xScalar = s6xScalar;
                calElem.ScalarElem.BankNum = calElem.BankNum;
                calElem.ScalarElem.AddressInt = calElem.AddressInt;
                calElem.ScalarElem.AddressBinInt = calElem.AddressBinInt;
                calElem.ScalarElem.RBase = calElem.RBase;
                calElem.ScalarElem.RBaseCalc = calElem.RBaseCalc;
                // S6x Information will be managed at read

                if (!slCalibrationElements.ContainsKey(calElem.UniqueAddress)) slCalibrationElements.Add(calElem.UniqueAddress, calElem);

                calElem = null;
            }

            foreach (S6xStructure s6xStructure in S6x.slProcessStructures.Values)
            {
                if (!s6xStructure.isCalibrationElement) continue;
                if (s6xStructure.BankNum != BankNum) continue;

                calElem = (CalibrationElement)slCalibrationElements[s6xStructure.UniqueAddress];
                if (calElem == null) calElem = new CalibrationElement(BankNum, s6xStructure.UniqueAddressHex);

                // No Conflict => Update with S6x Information, will be managed at read
                if (calElem.isStructure)
                {
                    calElem.StructureElem.S6xStructure = s6xStructure;
                    continue;
                }

                // Conflict Mngt
                calElem.ScalarElem = null;
                calElem.FunctionElem = null;
                calElem.StructureElem = null;
                calElem.TableElem = null;
                
                calElem.RBaseCalc = s6xStructure.UniqueAddressHex;
                calElem.AddressInt = s6xStructure.AddressInt - BankAddressInt;
                calElem.AddressBinInt = calElem.AddressInt + BankAddressBinInt;
                calElem.StructureElem = new Structure();
                calElem.StructureElem.S6xStructure = s6xStructure;
                calElem.StructureElem.BankNum = calElem.BankNum;
                calElem.StructureElem.AddressInt = calElem.AddressInt;
                calElem.StructureElem.AddressBinInt = calElem.AddressBinInt;
                calElem.StructureElem.RBase = calElem.RBase;
                calElem.StructureElem.RBaseCalc = calElem.RBaseCalc;
                // S6x Information will be managed at read

                if (!slCalibrationElements.ContainsKey(calElem.UniqueAddress)) slCalibrationElements.Add(calElem.UniqueAddress, calElem);

                calElem = null;
            }

            // Translations for non S6x Calibration Elements
            int iCountTables = 0;
            int iCountFunctions = 0;
            int iCountScalars = 0;
            int iCountStructures = 0;
            // Duplicated Labels Mngt (only for Calibration Elements)
            ArrayList alShortLabels = new ArrayList();
            ArrayList alLabels = new ArrayList();
            // First Run for Duplicates mngt by storing Labels and Short Labels (S6x ones)
            foreach (CalibrationElement cElem in slCalibrationElements.Values)
            {
                if (cElem.isTable)
                {
                    if (cElem.TableElem.S6xTable != null)
                    {
                        if (cElem.TableElem.ShortLabel != string.Empty) alShortLabels.Add(cElem.TableElem.ShortLabel.ToLower());
                        if (cElem.TableElem.Label != string.Empty) alLabels.Add(cElem.TableElem.Label.ToLower());
                    }
                }
                else if (cElem.isFunction)
                {
                    if (cElem.FunctionElem.S6xFunction != null)
                    {
                        if (cElem.FunctionElem.ShortLabel != string.Empty) alShortLabels.Add(cElem.FunctionElem.ShortLabel.ToLower());
                        if (cElem.FunctionElem.Label != string.Empty) alLabels.Add(cElem.FunctionElem.Label.ToLower());
                    }
                }
                else if (cElem.isScalar)
                {
                    if (cElem.ScalarElem.S6xScalar != null)
                    {
                        if (cElem.ScalarElem.ShortLabel != string.Empty) alShortLabels.Add(cElem.ScalarElem.ShortLabel.ToLower());
                        if (cElem.ScalarElem.Label != string.Empty) alLabels.Add(cElem.ScalarElem.Label.ToLower());
                    }
                }
                else if (cElem.isStructure)
                {
                    if (cElem.StructureElem.S6xStructure != null)
                    {
                        if (cElem.StructureElem.ShortLabel != string.Empty) alShortLabels.Add(cElem.StructureElem.ShortLabel.ToLower());
                        if (cElem.StructureElem.Label != string.Empty) alLabels.Add(cElem.StructureElem.Label.ToLower());
                    }
                }
            }

            // Main Run for Labels and Duplicates mngt
            foreach (CalibrationElement cElem in slCalibrationElements.Values)
            {
                string otherAddressLabel = string.Empty;
                int iDuplicatedNumber = 0;
                string sDuplicatedValue = string.Empty;

                // Searching in Other Addresses first
                S6xOtherAddress s6xOther = (S6xOtherAddress)S6x.slProcessOtherAddresses[cElem.UniqueAddress];
                if (s6xOther != null)
                {
                    if (s6xOther.OutputLabel && !s6xOther.hasDefaultLabel) otherAddressLabel = s6xOther.Label;
                    
                    // S6x Process CleanUp
                    S6x.slProcessOtherAddresses.Remove(s6xOther.UniqueAddress);
                    // 20200308 - PYM - It stays in S6x definition, other addresses are no more in conflict on addresses
                    //S6x.slOtherAddresses.Remove(s6xOther.UniqueAddress);

                    s6xOther = null;
                }

                if (cElem.isTable)
                {
                    if (otherAddressLabel != string.Empty)
                    {
                        cElem.TableElem.Label = otherAddressLabel;
                        cElem.TableElem.ShortLabel = otherAddressLabel;
                    }
                    else
                    {
                        iCountTables++;
                        if (cElem.TableElem.S6xTable == null && cElem.TableElem.Label == string.Empty)
                        {
                            if (S6x.Properties.NoNumbering)
                            {
                                cElem.TableElem.Label = SADDef.LongTablePrefix + cElem.Address;
                                cElem.TableElem.ShortLabel = SADDef.ShortTablePrefix + SADDef.NamingShortBankSeparator + cElem.Address;
                            }
                            else
                            {
                                cElem.TableElem.Label = SADDef.LongTablePrefix + string.Format("{0:d3}", iCountTables);
                                cElem.TableElem.ShortLabel = SADDef.ShortTablePrefix + string.Format("{0:d3}", iCountTables);
                            }
                            iDuplicatedNumber = 0;
                            sDuplicatedValue = cElem.TableElem.Label;
                            if (sDuplicatedValue != string.Empty)
                            {
                                while (alLabels.Contains(cElem.TableElem.Label.ToLower()))
                                {
                                    iDuplicatedNumber++;
                                    cElem.TableElem.Label = sDuplicatedValue + "_" + iDuplicatedNumber.ToString();
                                }
                            }
                            iDuplicatedNumber = 0;
                            sDuplicatedValue = cElem.TableElem.ShortLabel;
                            if (sDuplicatedValue != string.Empty)
                            {
                                while (alShortLabels.Contains(cElem.TableElem.ShortLabel.ToLower()))
                                {
                                    iDuplicatedNumber++;
                                    cElem.TableElem.ShortLabel = sDuplicatedValue + "_" + iDuplicatedNumber.ToString();
                                }
                            }
                        }
                    }
                    if (cElem.TableElem.ShortLabel != string.Empty) alShortLabels.Add(cElem.TableElem.ShortLabel.ToLower());
                    if (cElem.TableElem.Label != string.Empty) alLabels.Add(cElem.TableElem.Label.ToLower());
                }
                else if (cElem.isFunction)
                {
                    if (otherAddressLabel != string.Empty)
                    {
                        cElem.FunctionElem.Label = otherAddressLabel;
                        cElem.FunctionElem.ShortLabel = otherAddressLabel;
                    }
                    else
                    {
                        iCountFunctions++;
                        if (cElem.FunctionElem.S6xFunction == null && cElem.FunctionElem.Label == string.Empty)
                        {
                            if (S6x.Properties.NoNumbering)
                            {
                                cElem.FunctionElem.Label = SADDef.LongFunctionPrefix + cElem.Address;
                                cElem.FunctionElem.ShortLabel = SADDef.ShortFunctionPrefix + SADDef.NamingShortBankSeparator + cElem.Address;
                            }
                            else
                            {
                                cElem.FunctionElem.Label = SADDef.LongFunctionPrefix + string.Format("{0:d3}", iCountFunctions);
                                cElem.FunctionElem.ShortLabel = SADDef.ShortFunctionPrefix + string.Format("{0:d3}", iCountFunctions);
                            }
                            iDuplicatedNumber = 0;
                            sDuplicatedValue = cElem.FunctionElem.Label;
                            if (sDuplicatedValue != string.Empty)
                            {
                                while (alLabels.Contains(cElem.FunctionElem.Label.ToLower()))
                                {
                                    iDuplicatedNumber++;
                                    cElem.FunctionElem.Label = sDuplicatedValue + "_" + iDuplicatedNumber.ToString();
                                }
                            }
                            iDuplicatedNumber = 0;
                            sDuplicatedValue = cElem.FunctionElem.ShortLabel;
                            if (sDuplicatedValue != string.Empty)
                            {
                                while (alShortLabels.Contains(cElem.FunctionElem.ShortLabel.ToLower()))
                                {
                                    iDuplicatedNumber++;
                                    cElem.FunctionElem.ShortLabel = sDuplicatedValue + "_" + iDuplicatedNumber.ToString();
                                }
                            }
                        }
                    }
                    if (cElem.FunctionElem.ShortLabel != string.Empty) alShortLabels.Add(cElem.FunctionElem.ShortLabel.ToLower());
                    if (cElem.FunctionElem.Label != string.Empty) alLabels.Add(cElem.FunctionElem.Label.ToLower());
                }
                else if (cElem.isScalar)
                {
                    if (otherAddressLabel != string.Empty)
                    {
                        cElem.ScalarElem.Label = otherAddressLabel;
                        cElem.ScalarElem.ShortLabel = otherAddressLabel;
                    }
                    else
                    {
                        iCountScalars++;
                        if (cElem.ScalarElem.S6xScalar == null && cElem.ScalarElem.Label == string.Empty)
                        {
                            if (S6x.Properties.NoNumbering)
                            {
                                cElem.ScalarElem.ShortLabel = SADDef.ShortScalarPrefix + SADDef.NamingShortBankSeparator + cElem.Address;
                            }
                            else
                            {
                                cElem.ScalarElem.ShortLabel = SADDef.ShortScalarPrefix + string.Format("{0:d4}", iCountScalars);
                            }
                            iDuplicatedNumber = 0;
                            sDuplicatedValue = cElem.ScalarElem.Label;
                            if (sDuplicatedValue != string.Empty)
                            {
                                while (alLabels.Contains(cElem.ScalarElem.Label.ToLower()))
                                {
                                    iDuplicatedNumber++;
                                    cElem.ScalarElem.Label = sDuplicatedValue + "_" + iDuplicatedNumber.ToString();
                                }
                            }
                            iDuplicatedNumber = 0;
                            sDuplicatedValue = cElem.ScalarElem.ShortLabel;
                            if (sDuplicatedValue != string.Empty)
                            {
                                while (alShortLabels.Contains(cElem.ScalarElem.ShortLabel.ToLower()))
                                {
                                    iDuplicatedNumber++;
                                    cElem.ScalarElem.ShortLabel = sDuplicatedValue + "_" + iDuplicatedNumber.ToString();
                                }
                            }
                        }
                    }
                    if (cElem.ScalarElem.ShortLabel != string.Empty) alShortLabels.Add(cElem.ScalarElem.ShortLabel.ToLower());
                    if (cElem.ScalarElem.Label != string.Empty) alLabels.Add(cElem.ScalarElem.Label.ToLower());
                }
                else if (cElem.isStructure)
                {
                    if (otherAddressLabel != string.Empty)
                    {
                        cElem.StructureElem.Label = otherAddressLabel;
                        cElem.StructureElem.ShortLabel = otherAddressLabel;
                    }
                    else
                    {
                        iCountStructures++;
                        if (cElem.StructureElem.S6xStructure == null && cElem.StructureElem.Label == string.Empty)
                        {
                            if (S6x.Properties.NoNumbering)
                            {
                                cElem.StructureElem.Label = SADDef.LongStructurePrefix + cElem.Address;
                                cElem.StructureElem.ShortLabel = SADDef.ShortStructurePrefix + SADDef.NamingShortBankSeparator + cElem.Address;
                            }
                            else
                            {
                                cElem.StructureElem.Label = SADDef.LongStructurePrefix + string.Format("{0:d3}", iCountStructures);
                                cElem.StructureElem.ShortLabel = SADDef.ShortStructurePrefix + string.Format("{0:d4}", iCountStructures);
                            }
                            iDuplicatedNumber = 0;
                            sDuplicatedValue = cElem.StructureElem.Label;
                            if (sDuplicatedValue != string.Empty)
                            {
                                while (alLabels.Contains(cElem.StructureElem.Label.ToLower()))
                                {
                                    iDuplicatedNumber++;
                                    cElem.StructureElem.Label = sDuplicatedValue + "_" + iDuplicatedNumber.ToString();
                                }
                            }
                            iDuplicatedNumber = 0;
                            sDuplicatedValue = cElem.StructureElem.ShortLabel;
                            if (sDuplicatedValue != string.Empty)
                            {
                                while (alShortLabels.Contains(cElem.StructureElem.ShortLabel.ToLower()))
                                {
                                    iDuplicatedNumber++;
                                    cElem.StructureElem.ShortLabel = sDuplicatedValue + "_" + iDuplicatedNumber.ToString();
                                }
                            }
                        }
                    }
                    if (cElem.StructureElem.ShortLabel != string.Empty) alShortLabels.Add(cElem.StructureElem.ShortLabel.ToLower());
                    if (cElem.StructureElem.Label != string.Empty) alLabels.Add(cElem.StructureElem.Label.ToLower());
                }
            }
            alLabels = null;
            alShortLabels = null;

            // Data Read for Table Scalers in priority to better recognize Cols & Rows Count on Tables later on
            foreach (TableScaler tsTs in alTablesScalers)
            {
                foreach (string functionUniqueAddress in tsTs.InputFunctionsUniqueAddresses)
                {
                    calElem = (CalibrationElement)slCalibrationElements[functionUniqueAddress];
                    if (calElem != null)
                    {
                        if (calElem.isFunction)
                        {
                            currentRbaseEndAddress = AddressBankEndInt;
                            foreach (RBase rBase in slRbases.Values)
                            {
                                if (calElem.AddressInt >= rBase.AddressBankInt && calElem.AddressInt <= rBase.AddressBankEndInt)
                                {
                                    currentRbaseEndAddress = rBase.AddressBankEndInt;
                                    break;
                                }
                            }
                            if (!calElem.FunctionElem.HasValues) readFunction(ref calElem.FunctionElem, currentRbaseEndAddress, true);
                        }
                    }
                    calElem = null;
                }
            }

            // Data Read for all Calibration Elements
            for (int iCalElem = 0; iCalElem < slCalibrationElements.Count; iCalElem++)
            {
                calElem = (CalibrationElement)slCalibrationElements.GetByIndex(iCalElem);

                if (currentRbaseCode != calElem.RBase)
                {
                    currentRbaseCode = calElem.RBase;
                    currentRbaseEndAddress = AddressBankEndInt;
                    if (slRbases.ContainsKey(currentRbaseCode)) currentRbaseEndAddress = ((RBase)slRbases[calElem.RBase]).AddressBankEndInt;
                }

                if (calElem.isTable)
                {
                    if (!calElem.TableElem.HasValues) readTable(ref calElem.TableElem, currentRbaseEndAddress);
                }
                else if (calElem.isFunction)
                {
                    if (!calElem.FunctionElem.HasValues) readFunction(ref calElem.FunctionElem, currentRbaseEndAddress, false);
                }
                else if (calElem.isScalar)
                {
                    if (!calElem.ScalarElem.HasValue) readScalar(ref calElem.ScalarElem);
                }
                else if (calElem.isStructure)
                {
                    if (!calElem.StructureElem.HasValues) readStructure(ref calElem.StructureElem);
                }
            }
        }

        private void readStructure(ref Structure structure)
        {
            string[] arrBytes = null;

            // Manages non calculated AddressBinInt
            if (structure.AddressBinInt == -1 && structure.BankNum == BankNum) structure.AddressBinInt = structure.AddressInt + BankAddressBinInt;

            if (structure.S6xStructure != null)
            {
                structure.StructDefString = structure.S6xStructure.StructDef;
                structure.Number = structure.S6xStructure.Number;
            }

            if (structure.Number > 0)
            {
                arrBytes = getBytesArray(structure.AddressInt, structure.Number * structure.MaxSizeSingle);
                structure.Read(ref arrBytes, structure.Number);
            }
        }

        private void readScalar(ref Scalar scalar)
        {
            string sValue = string.Empty;

            // Manages non calculated AddressBinInt
            if (scalar.AddressBinInt == -1 && scalar.BankNum == BankNum) scalar.AddressBinInt = scalar.AddressInt + BankAddressBinInt;

            if (scalar.S6xScalar != null)
            {
                scalar.Byte = scalar.S6xScalar.Byte;
                scalar.Word = !scalar.S6xScalar.Byte;
                scalar.Signed = scalar.S6xScalar.Signed;
                scalar.UnSigned = !scalar.S6xScalar.Signed;
            }
            
            if (scalar.Word)
            {
                sValue = getWord(scalar.AddressInt, true);
                if (scalar.Signed) scalar.ValueInt = Convert.ToInt16(sValue, 16);
                else scalar.ValueInt = Convert.ToInt32(sValue, 16);
                scalar.InitialValue = sValue.Substring(2, 2) + SADDef.GlobalSeparator + sValue.Substring(0, 2);
            }
            else
            {
                sValue = getByte(scalar.AddressInt);
                if (scalar.Signed) scalar.ValueInt = Convert.ToSByte(sValue, 16);
                else scalar.ValueInt = Convert.ToInt16(sValue, 16);
                scalar.InitialValue = sValue;
            }

            // Scale Mngt Low Level

            // Forced Scale Expression
            foreach (RoutineCallInfoScalar ciCI in scalar.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputScalar != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputScalar.ForcedScaleExpression != null && ciCI.RoutineInputOutput.S6xInputScalar.ForcedScaleExpression != string.Empty)
                        {
                            scalar.ScaleExpression = ciCI.RoutineInputOutput.S6xInputScalar.ForcedScaleExpression;
                            scalar.ScalePrecision = ciCI.RoutineInputOutput.S6xInputScalar.ForcedScalePrecision;
                            break;
                        }
                    }
                }
            }
        }

        private void readFunction(ref Function function, int currentRbaseEndAddress, bool isScaler)
        {
            int iAddress = 0;
            int colNum = 0;
            int forcedRowsNum = -1;
            ArrayList alLines = null;
            ScalarLine lLine = null;
            int colsNum = 2;
            int highestValue = 0;
            int lowestValue = 0;
            bool lowestValueReached = false;

            // Manages non calculated AddressBinInt
            if (function.AddressBinInt == -1 && function.BankNum == BankNum) function.AddressBinInt = function.AddressInt + BankAddressBinInt;

            if (function.S6xFunction != null)
            {
                function.ByteInput = function.S6xFunction.ByteInput;
                function.ByteOutput = function.S6xFunction.ByteOutput;
                function.SignedInput = function.S6xFunction.SignedInput;
                function.SignedOutput = function.S6xFunction.SignedOutput;
            }

            forcedRowsNum = -1;
            foreach (RoutineCallInfoFunction ciCI in function.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputFunction != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputFunction.ForcedRowsNumber != null && ciCI.RoutineInputOutput.S6xInputFunction.ForcedRowsNumber != string.Empty)
                        {
                            try
                            {
                                forcedRowsNum = Convert.ToInt32(ciCI.RoutineInputOutput.S6xInputFunction.ForcedRowsNumber);
                                break;
                            }
                            catch { }
                        }
                    }
                }
            }
            // 20181112 - S6xElementSignatureSource mngt
            if (forcedRowsNum < 0 && function.S6xElementSignatureSource != null)
            {
                if (function.S6xElementSignatureSource.Function != null)
                {
                    if (function.S6xElementSignatureSource.Function.RowsNumber > 0) forcedRowsNum = function.S6xElementSignatureSource.Function.RowsNumber;
                }
            }

            alLines = new ArrayList();
            iAddress = function.AddressInt;

            colNum = 0;
            while (true)
            {
                if (function.S6xFunction == null)
                {

                    if (!(iAddress <= currentRbaseEndAddress && (function.AddressInt == iAddress || !slCalibrationElements.ContainsKey(Tools.UniqueAddress(function.BankNum, iAddress)))))
                    {
                        if (forcedRowsNum <= 0 || forcedRowsNum < alLines.Count) break;

                        // Forced from Signature overrides this control
                        if (function.S6xElementSignatureSource == null) break;
                        if (function.S6xElementSignatureSource.Function == null) break;
                        if (function.S6xElementSignatureSource.Function.RowsNumber != forcedRowsNum) break;
                    }

                    if (forcedRowsNum > 0)
                    {
                        if (alLines.Count >= forcedRowsNum) break;
                    }
                }
                else
                {
                    if (alLines.Count >= function.S6xFunction.RowsNumber) break;
                    if (colNum == 0 && function.ByteInput && iAddress > AddressBankEndInt) break;
                    if (colNum == 1 && function.ByteOutput && iAddress > AddressBankEndInt) break;
                    if (colNum == 0 && !function.ByteInput && iAddress + 1 > AddressBankEndInt) break;
                    if (colNum == 1 && !function.ByteOutput && iAddress + 1 > AddressBankEndInt) break;
                }

                if (colNum == 0) lLine = new ScalarLine(function.BankNum, iAddress, 2);

                lLine.Scalars[colNum] = new Scalar();
                lLine.Scalars[colNum].BankNum = function.BankNum;
                lLine.Scalars[colNum].AddressInt = iAddress;
                if (colNum == 0) lLine.Scalars[colNum].Byte = function.ByteInput;
                else lLine.Scalars[colNum].Byte = function.ByteOutput;
                lLine.Scalars[colNum].Word = !lLine.Scalars[colNum].Byte;
                if (colNum == 0) lLine.Scalars[colNum].Signed = function.SignedInput;
                else lLine.Scalars[colNum].Signed = function.SignedOutput;
                lLine.Scalars[colNum].UnSigned = !lLine.Scalars[colNum].Signed;
                readScalar(ref lLine.Scalars[colNum]);

                // Function Signed Input Remapping
                //      Routine information is not accurate, because it can work with signed and unsigned input, first read give the truth
                if (alLines.Count == 0 && colNum == 0)
                {
                    if (function.ByteInput)
                    {
                        if (lLine.Scalars[colNum].InitialValue == "7f" && function.SignedInput == false)
                        {
                            function.SignedInput = true;
                            lLine.Scalars[colNum].Signed = true;
                            lLine.Scalars[colNum].ValueInt = 127;
                        }
                        else if (lLine.Scalars[colNum].InitialValue == "ff" && function.SignedInput == true)
                        {
                            function.SignedInput = false;
                            lLine.Scalars[colNum].Signed = false;
                            lLine.Scalars[colNum].ValueInt = 255;
                        }

                        if (function.SignedInput)
                        {
                            highestValue = 127;     // 0x7f
                            lowestValue = -128;     // 0x80
                        }
                        else
                        {
                            lowestValue = 0;
                            highestValue = 255;     // 0xff
                        }
                    }
                    else
                    {
                        if (lLine.Scalars[colNum].InitialValue == "ff" + SADDef.GlobalSeparator +  "7f" && function.SignedInput == false)
                        {
                            function.SignedInput = true;
                            lLine.Scalars[colNum].Signed = true;
                            lLine.Scalars[colNum].ValueInt = 32767;
                        }
                        else if (lLine.Scalars[colNum].InitialValue == "ff" + SADDef.GlobalSeparator +  "ff" && function.SignedInput == true)
                        {
                            function.SignedInput = false;
                            lLine.Scalars[colNum].Signed = false;
                            lLine.Scalars[colNum].ValueInt = 65535;
                        }

                        if (function.SignedInput)
                        {
                            highestValue = 32767;   // 0x7fff
                            lowestValue = -32768;   // 0x8000
                        }
                        else
                        {
                            lowestValue = 0;
                            highestValue = 65535;   // 0xffff
                        }
                    }
                }

                if (function.S6xFunction == null && forcedRowsNum <= 0)
                {
                    // Specific Exit Conditions - Removed by S6x or Forced Rows Number (from S6x too)
                    //      - First Column can not contain its top value
                    //      - First Column can go back when its lowest value has been reached
                    if (colNum == 0 && alLines.Count > 0)
                    {
                        if (lowestValueReached)
                        {
                            if (lLine.Scalars[colNum].ValueInt > lowestValue) break;
                        }
                        if (highestValue == lLine.Scalars[colNum].ValueInt) break;
                        if (!lowestValueReached)
                        {
                            if (lowestValue == lLine.Scalars[colNum].ValueInt) lowestValueReached = true;
                        }
                    }
                }

                iAddress++;
                if (lLine.Scalars[colNum].Word) iAddress++;

                if (colNum < colsNum - 1)
                {
                    colNum++;
                }
                else
                {
                    colNum = 0;
                    alLines.Add(lLine);
                }
            }

            function.Lines = (ScalarLine[])alLines.ToArray(typeof(ScalarLine));
            alLines = null;

            // Scale Mngt Low Level

            // Forced Scale Expression - S6x Too
            bool foundInputScale = false;
            bool foundOutputScale = false;
            foreach (RoutineCallInfoFunction ciCI in function.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputFunction != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputFunction.ForcedInputScaleExpression != null && ciCI.RoutineInputOutput.S6xInputFunction.ForcedInputScaleExpression != string.Empty)
                        {
                            function.InputScaleExpression = ciCI.RoutineInputOutput.S6xInputFunction.ForcedInputScaleExpression;
                            function.InputScalePrecision = ciCI.RoutineInputOutput.S6xInputFunction.ForcedInputScalePrecision;
                            foundInputScale = true;
                        }
                        if (ciCI.RoutineInputOutput.S6xInputFunction.ForcedOutputScaleExpression != null && ciCI.RoutineInputOutput.S6xInputFunction.ForcedOutputScaleExpression != string.Empty)
                        {
                            function.OutputScaleExpression = ciCI.RoutineInputOutput.S6xInputFunction.ForcedOutputScaleExpression;
                            function.OutputScalePrecision = ciCI.RoutineInputOutput.S6xInputFunction.ForcedOutputScalePrecision;
                            foundOutputScale = true;
                        }
                    }
                }
                if (foundInputScale && foundOutputScale) break;
            }

            // Otherwise or in addition based on identified registers
            Register rInputKnownReg = null;
            Register rOutputKnownReg = null;
            foreach (RoutineCallInfoFunction ciCI in function.RoutinesCallsInfos)
            {
                if (rInputKnownReg == null)
                {
                    if (ciCI.InputRegister != string.Empty)
                    {
                        rInputKnownReg = (Register)slRegisters[(new S6xRegister(ciCI.InputRegister)).UniqueAddress];
                        if (rInputKnownReg != null)
                        {
                            if (rInputKnownReg.S6xRegister != null)
                            {
                                if (rInputKnownReg.S6xRegister.ScaleExpression != null && rInputKnownReg.S6xRegister.ScaleExpression != string.Empty)
                                {
                                    if (rInputKnownReg.S6xRegister.ScaleExpression.Trim().ToLower() != "x" && (function.InputScaleExpression.Trim().ToLower() == "x" || function.InputScaleExpression == string.Empty))
                                    {
                                        function.InputScaleExpression = rInputKnownReg.S6xRegister.ScaleExpression;
                                        function.InputScalePrecision = rInputKnownReg.S6xRegister.ScalePrecision;
                                        if (function.InputScalePrecision < SADDef.DefaultScaleMinPrecision) function.InputScalePrecision = SADDef.DefaultScaleMinPrecision;
                                        if (function.InputScalePrecision > SADDef.DefaultScaleMaxPrecision) function.InputScalePrecision = SADDef.DefaultScaleMaxPrecision;
                                        foundInputScale = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (ciCI.OutputRegister != string.Empty)
                {
                    rOutputKnownReg = (Register)slRegisters[(new S6xRegister(ciCI.OutputRegister)).UniqueAddress];
                    if (rOutputKnownReg != null)
                    {
                        if (rOutputKnownReg.S6xRegister != null)
                        {
                            if (rOutputKnownReg.S6xRegister.ScaleExpression != null && rOutputKnownReg.S6xRegister.ScaleExpression != string.Empty)
                            {
                                if (rOutputKnownReg.S6xRegister.ScaleExpression.Trim().ToLower() != "x" && (function.OutputScaleExpression.Trim().ToLower() == "x" || function.OutputScaleExpression == string.Empty))
                                {
                                    function.OutputScaleExpression = rOutputKnownReg.S6xRegister.ScaleExpression;
                                    function.OutputScalePrecision = rOutputKnownReg.S6xRegister.ScalePrecision;
                                    if (function.OutputScalePrecision < SADDef.DefaultScaleMinPrecision) function.OutputScalePrecision = SADDef.DefaultScaleMinPrecision;
                                    if (function.OutputScalePrecision > SADDef.DefaultScaleMaxPrecision) function.OutputScalePrecision = SADDef.DefaultScaleMaxPrecision;
                                    foundOutputScale = true;
                                }
                            }
                        }
                    }
                }

                if (rInputKnownReg != null && rOutputKnownReg != null) break;
            }

            // S6x Scale Expression Overrides previous values
            if (function.S6xFunction != null)
            {
                if (function.S6xFunction.InputScaleExpression != null && function.S6xFunction.InputScaleExpression != string.Empty)
                {
                    function.InputScaleExpression = function.S6xFunction.InputScaleExpression;
                    function.InputScalePrecision = function.S6xFunction.InputScalePrecision;
                    foundInputScale = true;
                }
                if (function.S6xFunction.OutputScaleExpression != null && function.S6xFunction.OutputScaleExpression != string.Empty)
                {
                    function.OutputScaleExpression = function.S6xFunction.OutputScaleExpression;
                    function.OutputScalePrecision = function.S6xFunction.OutputScalePrecision;
                    foundOutputScale = true;
                }
            }
            
            // Scaler Mngt
            if (!isScaler) return;

            int maxValue = 0;
            int scaleValue = 0x1000;
            foreach (ScalarLine sLine in function.Lines)
            {
                if (sLine.Scalars.Length == 2)
                {
                    while (sLine.Scalars[1].ValueInt % scaleValue != 0 && scaleValue > 0x1) scaleValue /= 0x10;
                    if (sLine.Scalars[1].ValueInt > maxValue) maxValue = sLine.Scalars[1].ValueInt;
                }
            }
            // S6x Scale Expression or Forced Scale Expression
            if (foundOutputScale)
            {
                try
                {
                    function.ScalerItemsNum = (int)Tools.ScaleValue(maxValue, function.OutputScaleExpression, true) + 1;
                    return;
                }
                catch { }
            }

            // Last check on all values
            while (scaleValue > 0x1)
            {
                int originalScaleValue = scaleValue;
                foreach (ScalarLine sLine in function.Lines)
                {
                    if (sLine.Scalars.Length == 2)
                    {
                        while (sLine.Scalars[1].ValueInt % scaleValue != 0 && scaleValue > 0x1) scaleValue /= 0x10;
                    }
                }
                if (scaleValue == originalScaleValue) break;
                scaleValue /= 0x10;
            }
            function.ScalerItemsNum = maxValue / scaleValue + 1;
            function.OutputScaleExpression = "X/" + scaleValue.ToString();
            function.OutputScalePrecision = 0;
        }

        private void readTable(ref Table table, int currentRbaseEndAddress)
        {
            int iAddress = 0;
            int colNum = 0;
            int forcedRowsNum = -1;
            int scalerRowsNum = -1;
            ArrayList alLines = null;
            ScalarLine lLine = null;

            // Manages non calculated AddressBinInt
            if (table.AddressBinInt == -1 && table.BankNum == BankNum) table.AddressBinInt = table.AddressInt + BankAddressBinInt;

            if (table.S6xTable != null)
            {
                table.ColsNumber = table.S6xTable.ColsNumber;
                table.WordOutput = table.S6xTable.WordOutput;
                table.SignedOutput = table.S6xTable.SignedOutput;
                if (table.S6xTable.ColsScalerAddress != null && table.S6xTable.ColsScalerAddress != string.Empty)
                {
                    table.ColsScalerUniqueAddress = table.S6xTable.ColsScalerAddress;
                }
                if (table.S6xTable.RowsScalerAddress != null && table.S6xTable.RowsScalerAddress != string.Empty)
                {
                    table.RowsScalerUniqueAddress = table.S6xTable.RowsScalerAddress;
                }
            }

            if (table.ColsNumber <= 0)
            {
                if (table.ColsScalerUniqueAddress != null && table.ColsScalerUniqueAddress != string.Empty)
                {
                    CalibrationElement calElem = (CalibrationElement)slCalibrationElements[table.ColsScalerUniqueAddress];
                    if (calElem != null)
                    {
                        if (calElem.isFunction) table.ColsNumber = calElem.FunctionElem.ScalerItemsNum;
                    }
                    calElem = null;
                }
            }
            scalerRowsNum = -1;
            if (table.RowsScalerUniqueAddress != null && table.RowsScalerUniqueAddress != string.Empty)
            {
                CalibrationElement calElem = (CalibrationElement)slCalibrationElements[table.RowsScalerUniqueAddress];
                if (calElem != null)
                {
                    if (calElem.isFunction) scalerRowsNum = calElem.FunctionElem.ScalerItemsNum;
                }
                calElem = null;
            }

            forcedRowsNum = -1;
            foreach (RoutineCallInfoTable ciCI in table.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputTable != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputTable.ForcedRowsNumber != null && ciCI.RoutineInputOutput.S6xInputTable.ForcedRowsNumber != string.Empty)
                        {
                            try
                            {
                                forcedRowsNum = Convert.ToInt32(ciCI.RoutineInputOutput.S6xInputTable.ForcedRowsNumber);
                                break;
                            }
                            catch { }
                        }
                    }
                }
            }
            // 20181112 - S6xElementSignatureSource mngt
            if (forcedRowsNum < 0 && table.S6xElementSignatureSource != null)
            {
                if (table.S6xElementSignatureSource.Table != null)
                {
                    if (table.S6xElementSignatureSource.Table.RowsNumber > 0) forcedRowsNum = table.S6xElementSignatureSource.Table.RowsNumber;
                }
            }

            if (table.ColsNumber <= 0)
            {
                table.Lines = new ScalarLine[] { };
                return;
            }

            alLines = new ArrayList();
            iAddress = table.AddressInt;

            colNum = 0;
            while (true)
            {
                if (table.S6xTable == null)
                {
                    if (table.WordOutput)
                    {
                        if (iAddress + 1 > currentRbaseEndAddress) break;
                        if (table.AddressInt != iAddress && slCalibrationElements.ContainsKey(Tools.UniqueAddress(table.BankNum, iAddress))) break;
                        if (slCalibrationElements.ContainsKey(Tools.UniqueAddress(table.BankNum, iAddress + 1))) break;
                    }
                    else
                    {
                        //if (!(iAddress <= currentRbaseEndAddress && (table.AddressInt == iAddress || !slCalibrationElements.ContainsKey(Tools.UniqueAddress(table.BankNum, iAddress))))) break;
                        if (iAddress >= currentRbaseEndAddress) break;
                        if (table.AddressInt != iAddress && slCalibrationElements.ContainsKey(Tools.UniqueAddress(table.BankNum, iAddress))) break;
                    }
                    if (forcedRowsNum > 0)
                    {
                        if (alLines.Count >= forcedRowsNum) break;
                    }
                    else if (scalerRowsNum > 0)
                    {
                        if (alLines.Count >= scalerRowsNum) break;
                    }
                }
                else
                {
                    if (alLines.Count >= table.S6xTable.RowsNumber) break;
                    if (iAddress > AddressBankEndInt) break;
                    if (table.WordOutput && iAddress + 1 > AddressBankEndInt) break;
                }

                if (colNum == 0) lLine = new ScalarLine(table.BankNum, iAddress, table.ColsNumber);

                lLine.Scalars[colNum] = new Scalar();
                lLine.Scalars[colNum].BankNum = table.BankNum;
                lLine.Scalars[colNum].AddressInt = iAddress;
                lLine.Scalars[colNum].Byte = !table.WordOutput;
                lLine.Scalars[colNum].Word = table.WordOutput;
                lLine.Scalars[colNum].Signed = table.SignedOutput;
                lLine.Scalars[colNum].UnSigned = !table.SignedOutput;
                readScalar(ref lLine.Scalars[colNum]);

                iAddress++;
                if (lLine.Scalars[colNum].Word) iAddress++;

                if (colNum < table.ColsNumber - 1)
                {
                    colNum++;
                }
                else
                {
                    colNum = 0;
                    alLines.Add(lLine);
                }
            }

            table.Lines = (ScalarLine[])alLines.ToArray(typeof(ScalarLine));
            alLines = null;

            // Scale Mngt Low Level

            // Forced Scale Expression
            foreach (RoutineCallInfoTable ciCI in table.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputTable != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputTable.ForcedCellsScaleExpression != null && ciCI.RoutineInputOutput.S6xInputTable.ForcedCellsScaleExpression != string.Empty)
                        {
                            table.CellsScaleExpression = ciCI.RoutineInputOutput.S6xInputTable.ForcedCellsScaleExpression;
                            table.CellsScalePrecision = ciCI.RoutineInputOutput.S6xInputTable.ForcedCellsScalePrecision;
                            break;
                        }
                    }
                }
            }
        }

        public void readUnknownCalibElements(ref SADS6x S6x)
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
            // External Rbases Addresses
            iAddress = BankAddressInt;
            foreach (RBase rBase in slRbases.Values)
            {
                if (iAddress < rBase.AddressBankInt)
                {
                    int startAddress = iAddress;
                    int endAddress = rBase.AddressBankInt - 1;
                    if (slIgnoredRanges.ContainsKey(startAddress))
                    {
                        if (((int[])slIgnoredRanges[startAddress])[1] < endAddress) slIgnoredRanges[startAddress] = new int[] { startAddress, endAddress };
                    }
                    else
                    {
                        slIgnoredRanges.Add(startAddress, new int[] { startAddress, endAddress });
                    }
                }
                iAddress = rBase.AddressBankEndInt + 1;
            }
            if (iAddress < BankAddressEndInt)
            {
                if (slIgnoredRanges.ContainsKey(iAddress))
                {
                    if (((int[])slIgnoredRanges[iAddress])[1] < BankAddressEndInt) slIgnoredRanges[iAddress] = new int[] { iAddress, BankAddressEndInt };
                }
                else
                {
                    slIgnoredRanges.Add(iAddress, new int[] { iAddress, BankAddressEndInt });
                }
            }

            // Calibration Elements Addresses
            foreach (CalibrationElement calElem in slCalibrationElements.Values)
            {
                if (calElem.StructureElem != null)
                {
                    // Vectors Lists are ignored - They have no output only Vectors have an output
                    if (calElem.StructureElem.isVectorsList) continue;
                    // Same thing for Sub Structures
                    if (calElem.StructureElem.ParentStructure != null) continue;
                }
                
                int startAddress = calElem.AddressInt;
                int endAddress = calElem.AddressEndInt;
                if (slIgnoredRanges.ContainsKey(startAddress))
                {
                    if (((int[])slIgnoredRanges[startAddress])[1] < endAddress) slIgnoredRanges[startAddress] = new int[] { startAddress, endAddress };
                }
                else
                {
                    slIgnoredRanges.Add(startAddress, new int[] { startAddress, endAddress });
                }
            }

            // Non Calibration Elements Tables - Possibly in wrong place
            foreach (Table extObject in slExtTables.Values)
            {
                if (extObject.BankNum == BankNum)
                {
                    if (isCalibrationAddress(extObject.AddressInt))
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
            }

            // Non Calibration Elements Functions - Possibly in wrong place
            foreach (Function extObject in slExtFunctions.Values)
            {
                if (extObject.BankNum == BankNum)
                {
                    if (isCalibrationAddress(extObject.AddressInt))
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
            }

            // Non Calibration Elements Scalars - Possibly in wrong place
            foreach (Scalar extObject in slExtScalars.Values)
            {
                if (extObject.BankNum == BankNum)
                {
                    if (isCalibrationAddress(extObject.AddressInt))
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
            }

            // Structures
            foreach (Structure sStruct in slExtStructures.Values)
            {
                // Vectors Lists are ignored - They have no output only Vectors have an output
                if (sStruct.isVectorsList) continue;
                // Same thing for Sub Structures
                if (sStruct.ParentStructure != null) continue;

                if (sStruct.BankNum == BankNum)
                {
                    if (isCalibrationAddress(sStruct.AddressInt))
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

            iAddress = AddressBankInt;
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
                        if (oAddress.BankNum == BankNum && oAddress.AddressInt >= iAddress && oAddress.AddressInt < ignoredRange[0])
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
                            slUnknownCalibParts.Add(Tools.UniqueAddress(BankNum, iAddress), new UnknownCalibPart(BankNum, iAddress, iAddress + iSize - 1, initialValues));
                            iAddress = iOtherAddress;
                        }
                    }
                    iSize = ignoredRange[0] - iAddress;
                    initialValues = getBytesArray(iAddress, iSize);
                    slUnknownCalibParts.Add(Tools.UniqueAddress(BankNum, iAddress), new UnknownCalibPart(BankNum, iAddress, iAddress + iSize - 1, initialValues));
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
            if (iAddress <= AddressBankEndInt)
            {
                // S6x Other Addresses
                iOtherAddress = -1;
                foreach (S6xOtherAddress oAddress in S6x.slProcessOtherAddresses.Values)
                {
                    if (oAddress.BankNum == BankNum && oAddress.AddressInt >= iAddress && oAddress.AddressInt <= AddressBankEndInt)
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
                        slUnknownCalibParts.Add(Tools.UniqueAddress(BankNum, iAddress), new UnknownCalibPart(BankNum, iAddress, iAddress + iSize - 1, initialValues));
                        iAddress = iOtherAddress;
                    }
                }
                iSize = AddressBankEndInt - iAddress + 1;
                initialValues = getBytesArray(iAddress, iSize);
                slUnknownCalibParts.Add(Tools.UniqueAddress(BankNum, iAddress), new UnknownCalibPart(BankNum, iAddress, AddressBankEndInt, initialValues));
            }
        }

        public void processCallTranslations(ref SADS6x S6x)
        {
            Call cCall = null;
            Routine rRoutine = null;
            Vector vect = null;
            bool bUpdate = false;

            for (int iCall = 0; iCall < slCalls.Count; iCall++)
            {
                cCall = (Call)slCalls.GetByIndex(iCall);
                switch (cCall.CallType)
                {
                    case CallType.ShortJump:
                    case CallType.Goto:
                    case CallType.Skip:
                    case CallType.Unknown:
                        // S6x Forces a Main Call
                        if (S6x.slProcessRoutines.ContainsKey(cCall.UniqueAddress)) alMainCallsUniqueAddresses.Add(cCall.UniqueAddress);
                        else if (S6x.slProcessOtherAddresses.ContainsKey(cCall.UniqueAddress)) alMainCallsUniqueAddresses.Add(cCall.UniqueAddress);
                        break;
                    default:
                        alMainCallsUniqueAddresses.Add(cCall.UniqueAddress);
                        break;
                }

                if (cCall.isIntVector)
                {
                    // Initial Translation Already Done
                    bUpdate = false;
                }
                else if (cCall.isVector)
                {
                    // Initial Translation not managed
                    if (S6x.Properties.NoNumbering)
                    {
                        cCall.Label = SADDef.ShortCallPrefix + SADDef.NamingShortBankSeparator + cCall.UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator);
                    }
                    else
                    {
                        cCall.Label = SADDef.ShortCallPrefix + string.Format("{0:d4}", alMainCallsUniqueAddresses.Count);
                    }
                    cCall.ShortLabel = cCall.Label;
                    bUpdate = true;
                }
                else if (cCall.isRoutine)
                {
                    // Initial Translation to be synchronized
                    rRoutine = (Routine)slRoutines[cCall.UniqueAddress];
                    if (rRoutine.Code == RoutineCode.Unknown && (rRoutine.Type == RoutineType.Other || rRoutine.Type == RoutineType.Unknown))
                    // Unmanaged Routines
                    {
                        if (S6x.Properties.NoNumbering)
                        {
                            cCall.Label = SADDef.ShortCallPrefix + SADDef.NamingShortBankSeparator + cCall.UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator);
                        }
                        else
                        {
                            cCall.Label = SADDef.ShortCallPrefix + string.Format("{0:d4}", alMainCallsUniqueAddresses.Count);
                        }
                        cCall.ShortLabel = cCall.Label;
                    }
                    else
                    // Managed Routines
                    {
                        cCall.Label = rRoutine.Label;
                        cCall.ShortLabel = rRoutine.ShortLabel;
                    }
                    rRoutine = null;
                    bUpdate = true;
                }
                else if (cCall.isFake)
                {
                    // Fake Call
                    // Normally they should not appear in slCalls
                    if (S6x.Properties.NoNumbering)
                    {
                        cCall.Label = SADDef.ShortCallFakePrefix + SADDef.NamingShortBankSeparator + cCall.UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator);
                    }
                    else
                    {
                        cCall.Label = SADDef.ShortCallFakePrefix + string.Format("{0:d4}", alMainCallsUniqueAddresses.Count);
                    }
                    cCall.ShortLabel = cCall.Label;
                    bUpdate = true;
                }
                else if (cCall.AddressInt == 0)
                {
                    // Bank Start Call
                    cCall.ShortLabel = SADDef.ShortLabelCallBankStartTemplate.Replace("%1%", cCall.BankNum.ToString());
                    cCall.Label = SADDef.LongLabelCallBankStartTemplate.Replace("%1%", cCall.BankNum.ToString());
                    bUpdate = true;
                }
                else
                {
                    // Initial Translation not managed
                    switch (cCall.CallType)
                    {
                        case CallType.ShortJump:
                        case CallType.Goto:
                        case CallType.Skip:
                        case CallType.Unknown:
                            break;
                        default:
                            if (S6x.Properties.NoNumbering)
                            {
                                cCall.Label = SADDef.ShortCallPrefix + SADDef.NamingShortBankSeparator + cCall.UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator);
                            }
                            else
                            {
                                cCall.Label = SADDef.ShortCallPrefix + string.Format("{0:d4}", alMainCallsUniqueAddresses.Count);
                            }
                            cCall.ShortLabel = cCall.Label;
                            bUpdate = true;
                            break;
                    }
                }

                // S6x Translation Override - Priority to Call Translation
                if (S6x.slProcessRoutines.ContainsKey(cCall.UniqueAddress))
                {
                    cCall.S6xRoutine = (S6xRoutine)S6x.slProcessRoutines[cCall.UniqueAddress];
                    bUpdate = true;
                }
                else if (S6x.slProcessOtherAddresses.ContainsKey(cCall.UniqueAddress))
                {
                    if (((S6xOtherAddress)S6x.slProcessOtherAddresses[cCall.UniqueAddress]).OutputLabel && !((S6xOtherAddress)S6x.slProcessOtherAddresses[cCall.UniqueAddress]).hasDefaultLabel)
                    {
                        cCall.Label = ((S6xOtherAddress)S6x.slProcessOtherAddresses[cCall.UniqueAddress]).Label;
                        cCall.ShortLabel = cCall.Label;
                    }

                    // S6x CleanUp
                    S6x.slProcessOtherAddresses.Remove(cCall.UniqueAddress);
                    // 20200308 - PYM - It stays in S6x definition, other addresses are no more in conflict on addresses
                    //S6x.slOtherAddresses.Remove(cCall.UniqueAddress);
                }

                if (bUpdate) slCalls[cCall.UniqueAddress] = cCall;

                cCall = null;
            }

            // Additional Vectors Update
            for (int iPos = 0; iPos < slAdditionalVectors.Count; iPos++)
            {
                vect = (Vector)slAdditionalVectors.GetByIndex(iPos);
                if (alMainCallsUniqueAddresses.Contains(vect.UniqueAddress))
                {
                    cCall = (Call)slCalls[vect.UniqueAddress];
                    vect.Label = cCall.Label;
                    vect.ShortLabel = cCall.ShortLabel;
                    vect.Comments = cCall.Comments;
                    slAdditionalVectors[vect.UniqueSourceAddress] = vect;
                    cCall = null;
                }
                vect = null;
            }
        }

        // Used to check conflicts inside Calibration and inside elements
        //      Not outside Rbases
        //      Another Element is not between initialCalElemAddress and jumpAddress
        public bool isCalElemAddressInConflict(int calElemAddress, int initialCalElemAddress)
        {
            CalibrationElement calElem = null;
            int calElemIndex = -1;

            // initialCalElemAddress should be an existing valid Calibration Element Address
            if (!slCalibrationElements.ContainsKey(Tools.UniqueAddress(BankNum, initialCalElemAddress))) return false;
            
            // Jump outside Bank Addresses => KO
            if (calElemAddress < AddressBankInt || calElemAddress > AddressBankEndInt) return true;

            // Jump to the same Address => OK
            if (initialCalElemAddress == calElemAddress) return false;

            // Jump to a recognized Address => OK
            if (slCalibrationElements.ContainsKey(Tools.UniqueAddress(BankNum, calElemAddress))) return false;

            // Jump outside Rbase part => KO
            foreach (RBase rBase in slRbases.Values)
            {
                if (calElemAddress < rBase.AddressBankInt || calElemAddress > rBase.AddressBankEndInt) return true;
            }
            // Jump to Additional Vector Source Address => KO
            foreach (Vector addVect in slAdditionalVectors.Values)
            {
                if (calElemAddress >= addVect.SourceAddressInt && calElemAddress <= addVect.SourceAddressInt + 1) return true;
            }

            calElemIndex = slCalibrationElements.IndexOfKey(Tools.UniqueAddress(BankNum, initialCalElemAddress));
            if (calElemAddress > initialCalElemAddress)
            {
                calElemIndex++;
                while (calElemIndex < slCalibrationElements.Count)
                {
                    calElem = (CalibrationElement)slCalibrationElements.GetByIndex(calElemIndex);
                    if (calElem.AddressInt > initialCalElemAddress && calElem.AddressInt < calElemAddress)
                    {
                        calElem = null;
                        return true;
                    }
                    if (calElemAddress < calElem.AddressInt) break;
                    calElemIndex++;
                }
                calElem = null;
                return false;
            }
            else
            {
                calElemIndex--;
                while (calElemIndex >= 0)
                {
                    calElem = (CalibrationElement)slCalibrationElements.GetByIndex(calElemIndex);
                    if (calElem.AddressInt > calElemAddress && calElem.AddressInt < initialCalElemAddress)
                    {
                        calElem = null;
                        return true;
                    }
                    if (calElemAddress > calElem.AddressInt) break;
                    calElemIndex--;
                }
                calElem = null;
                return false;
            }
        }

        public RBase getRbaseByAddress(string sAddress)
        {
            foreach (RBase rBase in slRbases.Values)
            {
                if (rBase.AddressBank == sAddress) return rBase;
            }
            return null;
        }

        public RBase getRbaseByAddress(int addressInBank)
        {
            foreach (RBase rBase in slRbases.Values)
            {
                if (addressInBank >= rBase.AddressBankInt && addressInBank <= rBase.AddressBankEndInt) return rBase;
            }
            return null;
        }

        public bool isCalibrationAddress(int addressInBank)
        {
            if (slRbases.Count == 0) return true;

            foreach (RBase rBase in slRbases.Values)
            {
                if (addressInBank >= rBase.AddressBankInt && addressInBank <= rBase.AddressBankEndInt) return true;
            }

            return false;
        }

        public int getTablesScalersIndex(string registerAddress, string functionAddress)
        {
            for (int iIndex = 0; iIndex < alTablesScalers.Count; iIndex++)
            {
                if (registerAddress != null && registerAddress != string.Empty)
                {
                    foreach (string scalerAddress in ((TableScaler)alTablesScalers[iIndex]).InputRegistersAddresses)
                    {
                        if (scalerAddress == registerAddress) return iIndex;
                    }
                }
                else if (functionAddress != null && functionAddress != string.Empty)
                {
                    foreach (string scalerAddress in ((TableScaler)alTablesScalers[iIndex]).InputFunctionsUniqueAddresses)
                    {
                        if (scalerAddress == functionAddress) return iIndex;
                    }
                }
            }
            return -1;
        }

        public string getBytes(int startPos, int len) { return Tools.getBytes(startPos - AddressBankInt, len, ref arrBytes); }

        public string[] getBytesArray(int startPos, int len) { return Tools.getBytesArray(startPos - AddressBankInt, len, ref arrBytes); }

        public string getByte(int startPos) { return Tools.getByte(startPos - AddressBankInt, ref arrBytes); }

        public string getByte(string startPos) { return Tools.getByte(Convert.ToInt32(startPos, 16) - SADDef.EecBankStartAddress - AddressBankInt, ref arrBytes); }

        public string getWord(int startPos, bool lsbFirst) { return Tools.getWord(startPos - AddressBankInt, lsbFirst, ref arrBytes); }

        public string getWord(string startPos, bool lsbFirst) { return Tools.getWord(Convert.ToInt32(startPos, 16) - SADDef.EecBankStartAddress - AddressBankInt, lsbFirst, ref arrBytes); }

        public int getByteInt(int startPos, bool signed) { return Tools.getByteInt(startPos - AddressBankInt, signed, ref arrBytes); }

        public int getByteInt(string startPos, bool signed) { return Tools.getByteInt(Convert.ToInt32(startPos, 16) - SADDef.EecBankStartAddress - AddressBankInt, signed, ref arrBytes); }

        public int getWordInt(int startPos, bool signed, bool lsbFirst) { return Tools.getWordInt(startPos - AddressBankInt, signed, lsbFirst, ref arrBytes); }

        public int getWordInt(string startPos, bool signed, bool lsbFirst) { return Tools.getWordInt(Convert.ToInt32(startPos, 16) - SADDef.EecBankStartAddress - AddressBankInt, signed, lsbFirst, ref arrBytes); }

        public object[] getBytesFromSignature(string sSignature) { return Tools.getBytesFromSignature(sSignature, ref sBytes); }
    }
}
