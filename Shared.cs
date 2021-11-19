using System;
using System.Collections;
using System.Text;

namespace SAD806x
{
    // Statistics
    public enum StatisticsItems
    {
        RegShortcut0x100 = 0,
        RegShortcut0x100SFR = 1
    }

    public enum StatisticsRegisterItems
    {
        Count = 0,
        RegShortcut0x100 = 1,
        Immediate = 2,
        Direct = 3,
        Indirect = 4,
        AutoIncrement = 5,
        ShortIndex = 6,
        LongIndex = 7
    }
    
    // Routine Signature available 806x options
    public enum Signature806xOptions
    {
        Undefined = 0,
        for8061Only = 1,
        for8065Only = 2
    }

    // Routine Signature available Bank options
    public enum SignatureBankOptions
    {
        Undefined = 0,
        forBank8Only = 1,
        forBank1Only = 2,
        forBank9Only = 3,
        forBank0Only = 4,
    }
    
    // SAD Info
    public class SADInfo
    {
        public bool is8061 = false;
        public bool isEarly = false;
        public bool isPilot = false;
        public bool is8065SingleBank = false;

        public bool isCheckSumConfirmed = false;
        public bool isCheckSumValid = false;
        public int correctedChecksum = -1;
        
        public int CheckSum = -1;
        public int SmpBaseAddress = -1;
        public int CcExeTime = -1;

        public int LevelsNum = -1;
        public int CalibsNum = -1;

        public int VidBankNum = -1;
        public string VidStrategy = string.Empty;
        public string VidStrategyVersion = string.Empty;
        public string VidSerial = string.Empty;
        public string VidPatsCode = string.Empty;
        public string VidCopyright = string.Empty;
        public string VidVIN = string.Empty;
        public bool VidEnabled = false;
        public int VidRevMile = -1;
        public int VidRtAxle = -1;

        public SortedList slBanksInfos = null;

        public SADInfo()
        {
            slBanksInfos = new SortedList();
        }
    }
    
    // Merged Elements
    public enum MergedType
    {
        Unknown,
        UnknownOperationLine,
        UnknownCalibrationLine,
        ReservedAddress,
        Operation,
        CalibrationElement,
        ExtTable,
        ExtFunction,
        ExtScalar,
        ExtStructure,
        Vector
    }

    public class Element
    {
        public UnknownOpPartLine UnknownOpPartLine = null;
        public UnknownCalibPartLine UnknownCalibPartLine = null;
        public ReservedAddress ReservedAddress = null;
        public Operation Operation = null;
        public CalibrationElement CalElement = null;
        public Table ExtTable = null;
        public Function ExtFunction = null;
        public Scalar ExtScalar = null;
        public Structure ExtStructure = null;
        public Vector Vector = null;
        public S6xOtherAddress OtherAddress = null;

        public MergedType Type = MergedType.Unknown;

        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressEndInt = -1;

        public bool isIncluded = false;
        public SortedList IncludedElements = null;

        public string Address { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInt); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public Element(object element, MergedType type)
        {
            Type = type;
            switch (Type)
            {
                case MergedType.UnknownOperationLine:
                    UnknownOpPartLine = (UnknownOpPartLine)element;
                    BankNum = UnknownOpPartLine.BankNum;
                    AddressInt = UnknownOpPartLine.AddressInt;
                    AddressEndInt = UnknownOpPartLine.AddressEndInt;
                    break;
                case MergedType.UnknownCalibrationLine:
                    UnknownCalibPartLine = (UnknownCalibPartLine)element;
                    BankNum = UnknownCalibPartLine.BankNum;
                    AddressInt = UnknownCalibPartLine.AddressInt;
                    AddressEndInt = UnknownCalibPartLine.AddressEndInt;
                    break;
                case MergedType.ReservedAddress:
                    ReservedAddress = (ReservedAddress)element;
                    BankNum = ReservedAddress.BankNum;
                    AddressInt = ReservedAddress.AddressInt;
                    AddressEndInt = ReservedAddress.AddressEndInt;
                    break;
                case MergedType.Operation:
                    Operation = (Operation)element;
                    BankNum = Operation.BankNum;
                    AddressInt = Operation.AddressInt;
                    AddressEndInt = Operation.AddressNextInt - 1;
                    break;
                case MergedType.CalibrationElement:
                    CalElement = (CalibrationElement)element;
                    BankNum = CalElement.BankNum;
                    AddressInt = CalElement.AddressInt;
                    AddressEndInt = CalElement.AddressEndInt;
                    break;
                case MergedType.ExtTable:
                    ExtTable = (Table)element;
                    BankNum = ExtTable.BankNum;
                    AddressInt = ExtTable.AddressInt;
                    AddressEndInt = ExtTable.AddressEndInt;
                    break;
                case MergedType.ExtFunction:
                    ExtFunction = (Function)element;
                    BankNum = ExtFunction.BankNum;
                    AddressInt = ExtFunction.AddressInt;
                    AddressEndInt = ExtFunction.AddressEndInt;
                    break;
                case MergedType.ExtScalar:
                    ExtScalar = (Scalar)element;
                    BankNum = ExtScalar.BankNum;
                    AddressInt = ExtScalar.AddressInt;
                    AddressEndInt = ExtScalar.AddressEndInt;
                    break;
                case MergedType.ExtStructure:
                    ExtStructure = (Structure)element;
                    BankNum = ExtStructure.BankNum;
                    AddressInt = ExtStructure.AddressInt;
                    AddressEndInt = ExtStructure.AddressEndInt;
                    break;
                case MergedType.Vector:
                    Vector = (Vector)element;
                    BankNum = Vector.SourceBankNum;
                    AddressInt = Vector.SourceAddressInt;
                    AddressEndInt = Vector.SourceAddressInt + 1;
                    break;
            }
        }
    }

    // Unknwon Parts
    public class UnknownOpPart
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressEndInt = -1;

        public UnknownOpPartLine[] Lines = null;

        public string Address { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInt); } }
        public int Size { get { return AddressEndInt - AddressInt + 1; } }
        public string AddressEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressEndInt); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public UnknownOpPart(int bankNum, int addressInt, int addressEndInt, string[] initialValues)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            AddressEndInt = addressEndInt;

            splitLines(ref initialValues);
        }

        private void splitLines(ref string[] initialValues)
        {
            int startAddress = -1;
            int endAddress = -1;
            bool bDup = false;
            int iCount = -1;

            object[] arrPartLines = Tools.splitPartsLines(AddressInt, ref initialValues);

            Lines = new UnknownOpPartLine[arrPartLines.Length];
            foreach (object[] arrPartLine in arrPartLines)
            {
                iCount++;
                startAddress = Convert.ToInt32(arrPartLine[0]);
                endAddress = Convert.ToInt32(arrPartLine[1]);
                bDup = Convert.ToBoolean(arrPartLine[2]);
                if (bDup)
                {
                    Lines[iCount] = new UnknownOpPartLine(BankNum, startAddress, endAddress, arrPartLine[3].ToString());
                }
                else
                {
                    Lines[iCount] = new UnknownOpPartLine(BankNum, startAddress, endAddress, (string[])arrPartLine[3]);
                }
            }
        }
    }

    public class UnknownOpPartLine
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressEndInt = -1;

        private string[] values = null;
        public string DuplicatedValues = string.Empty;

        public string Address { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInt); } }
        public int Size { get { return AddressEndInt - AddressInt + 1; } }
        public string AddressEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressEndInt); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public string Values
        {
            get
            {
                if (values == null) return string.Empty;
                else if (DuplicatedValues == string.Empty) return string.Join(SADDef.GlobalSeparator.ToString(), values);
                else return DuplicatedValues;
            }
        }

        public UnknownOpPartLine(int bankNum, int addressInt, int addressEndInt, string[] arrValues)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            AddressEndInt = addressEndInt;
            values = arrValues;
        }

        public UnknownOpPartLine(int bankNum, int addressInt, int addressEndInt, string duplicateValues)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            AddressEndInt = addressEndInt;
            DuplicatedValues = duplicateValues;
        }
    }
    
    public class UnknownCalibPart
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressEndInt = -1;

        public UnknownCalibPartLine[] Lines = null;

        public string Address { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInt); } }
        public int Size { get { return AddressEndInt - AddressInt + 1; } }
        public string AddressEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressEndInt); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public UnknownCalibPart(int bankNum, int addressInt, int addressEndInt, string[] initialValues)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            AddressEndInt = addressEndInt;

            splitLines(BankNum, ref initialValues);
        }

        private void splitLines(int bankNum, ref string[] initialValues)
        {
            int startAddress = -1;
            int endAddress = -1;
            bool bDup = false;
            int iCount = -1;

            object[] arrPartLines = Tools.splitPartsLines(AddressInt, ref initialValues);

            Lines = new UnknownCalibPartLine[arrPartLines.Length];
            foreach (object[] arrPartLine in arrPartLines)
            {
                iCount++;
                startAddress = Convert.ToInt32(arrPartLine[0]);
                endAddress = Convert.ToInt32(arrPartLine[1]);
                bDup = Convert.ToBoolean(arrPartLine[2]);
                if (bDup)
                {
                    Lines[iCount] = new UnknownCalibPartLine(bankNum, startAddress, endAddress, arrPartLine[3].ToString());
                }
                else
                {
                    Lines[iCount] = new UnknownCalibPartLine(bankNum, startAddress, endAddress, (string[])arrPartLine[3]);
                }
            }
        }
    }

    public class UnknownCalibPartLine
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressEndInt = -1;

        private string[] values = new string[] {};
        public string DuplicatedValues = string.Empty;

        public string Address { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInt); } }
        public int Size { get { return AddressEndInt - AddressInt + 1; } }
        public string AddressEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressEndInt); } }
        public string Values
        {
            get
            {
                if (values == null) return string.Empty;
                if (DuplicatedValues == string.Empty) return string.Join(SADDef.GlobalSeparator.ToString(), values);
                else return DuplicatedValues;
            }
        }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public UnknownCalibPartLine(int bankNum, int addressInt, int addressEndInt, string[] arrValues)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            AddressEndInt = addressEndInt;
            values = arrValues;
        }

        public UnknownCalibPartLine(int bankNum, int addressInt, int addressEndInt, string duplicateValues)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            AddressEndInt = addressEndInt;
            DuplicatedValues = duplicateValues;
        }

        public string[] ValuesInt(int iBase)
        {
            string[] sValues = null;
            
            if (values == null) return new string[] {};
            
            sValues = new string[values.Length];
            for (int iPos = 0; iPos < sValues.Length; iPos++) sValues[iPos] = Convert.ToString(Convert.ToInt32(values[iPos], 16), iBase);
            return sValues;
        }
    }

    // Reserved Addresses

    public enum ReservedAddressType
    {
        Fill,       // FILL
        Word,       // WORD
        Byte,       // BYTE
        RomSize,    // ROMSIZE
        CheckSum,   // CHECKSUM
        SmpBase,    // SMPBASEADR
        CcExeTime,  // CCEXETIME
        IntVector,  // INTVECTORADR
        LevelNum,   // LEVNUM
        CalNum,     // CALNUM
        CalPointer, // RBASEADR
        Ascii,      // ASCII for VID BLOCK
        Hex         // HEX for VID BLOCK
    }

    public class ReservedAddress
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressBinInt = -1;
        public int Size = 0;
        public ReservedAddressType Type = ReservedAddressType.Fill;

        public int ValueInt = Int32.MinValue;
        public string ValueString = string.Empty;

        public string InitialValue = string.Empty;

        public string ShortLabel = string.Empty;
        public string Label = string.Empty;
        public string Comments = string.Empty;
        
        public string Address { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInt); } }
        public int AddressEndInt { get { return AddressInt + Size - 1; } }
        public string AddressEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressEndInt); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public string FullLabel
        {
            get
            {
                if (Label != string.Empty && ShortLabel != string.Empty && Label != ShortLabel) return ShortLabel + " - " + Label;
                else return Label;
            }
        }

        public ReservedAddress(int bankNum, int addressInt, int size, ReservedAddressType type)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            Size = size;
            Type = type;
        }

        public string Value(int iBase)
        {
            switch (Type)
            {
                case ReservedAddressType.Fill:
                    return "fill";
                case ReservedAddressType.SmpBase:
                case ReservedAddressType.CalPointer:
                case ReservedAddressType.IntVector:
                    iBase = 16;
                    break;
            }
            string sValue = string.Empty;
            sValue = Convert.ToString(ValueInt, iBase);
            if (iBase == 16 && sValue.Length > Size * 2) return sValue.Substring(sValue.Length - Size * 2, Size * 2);
            else return sValue;
        }
    }
    
    // RBase
    public class RBase
    {
        public int BankNum = -1;
        
        public string Code;

        public int AddressBankInt = -1;
        public int AddressBankEndInt = -1;

        public int AddressBinInt = -1;

        public int Size { get { return AddressBankEndInt - AddressBankInt + 1; } }

        public int AddressBinEndInt { get { return AddressBinInt + Size - 1; } }

        public int AddressInternalInt { get { return 0; } }
        public int AddressInternalEndInt { get { return Size - 1; } }

        public string AddressBin { get { return string.Format("{0:x5}", AddressBinInt); } }
        public string AddressBinEnd { get { return string.Format("{0:x5}", AddressBinEndInt); } }
        public string AddressBank { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressBankInt); } }
        public string AddressBankEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressBankEndInt); } }
        public string AddressInternal { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInternalInt); } }
        public string AddressInternalEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInternalEndInt); } }
        
        public RBase()
        {

        }
    }

    // RConst
    public class RConst
    {
        public string Code = string.Empty;
        public int ValueInt = -1;

        public int AddressBankNum = -1;
        
        public int[] Addresses = new int[] {};

        public bool isValue { get { return (Addresses.Length == 0); } }
        public bool isAddresses { get { return (Addresses.Length > 0); } }

        public string Value { get { return Convert.ToString(ValueInt, 16); } }

        public RConst(string code, int value)
        {
            Code = code;
            ValueInt = value;
        }
    }

    // Register Links
    public class RegisterFunctionLink
    {
        public string FunctionUniqueAddress = string.Empty;

        public bool isInputRegister = false;
        public bool isOutputRegister = false;
        public bool isOutputRegisterByte = false;

        public RegisterFunctionLink(string functionUniqueAddress)
        {
            FunctionUniqueAddress = functionUniqueAddress;
        }

        public void SetRegisterRole(bool setInputRegister, bool setOutputRegister, bool setOutputRegisterByte)
        {
            if (setInputRegister) isInputRegister = true;
            if (setOutputRegister) isOutputRegister = true;
            if (setOutputRegisterByte) isOutputRegisterByte = true;
        }
    }

    public class RegisterTableLink
    {
        public string TableUniqueAddress = string.Empty;

        public bool isColumnsScaler = false;
        public bool isRowsScaler = false;

        public bool isOutputRegister = false;
        public bool isOutputRegisterByte = false;

        public RegisterTableLink(string tableUniqueAddress)
        {
            TableUniqueAddress = tableUniqueAddress;
        }

        public void SetRegisterRole(bool setColumnsScaler, bool setRowsScaler, bool setOutputRegister, bool setOutputRegisterByte)
        {
            if (setColumnsScaler) isColumnsScaler = true;
            if (setRowsScaler) isRowsScaler = true;

            if (setOutputRegister) isOutputRegister = true;
            if (setOutputRegisterByte) isOutputRegisterByte = true;
        }
    }

    public class RegisterRegisterLink
    {
        public string RegisterUniqueAddress = string.Empty;

        public bool isSourceThroughFunction = false;
        public bool isDestinationThroughFunction = false;

        public bool isSourceScaler = false;
        public bool isDestinationScaler = false;

        public ArrayList RelatedFunctionsUniqueAddresses = null;

        public RegisterRegisterLink(string registerUniqueAddress)
        {
            RegisterUniqueAddress = registerUniqueAddress;
            RelatedFunctionsUniqueAddresses = new ArrayList();
        }

        public void SetRegisterRole(bool setSourceThroughFunction, bool setDestinationThroughFunction, string relatedFunctionUniqueAddress, bool scalerContext)
        {
            if (setSourceThroughFunction) isSourceThroughFunction = true;
            if (setDestinationThroughFunction) isDestinationThroughFunction = true;

            if (scalerContext && setSourceThroughFunction) isSourceScaler = true;
            if (scalerContext && setDestinationThroughFunction) isDestinationScaler = true;

            if (!RelatedFunctionsUniqueAddresses.Contains(relatedFunctionUniqueAddress)) RelatedFunctionsUniqueAddresses.Add(relatedFunctionUniqueAddress);
        }
    }

    public class RegisterLinks
    {
        private SortedList functionsLinks = null;
        private SortedList tablesLinks = null;
        private SortedList registersLinks = null;

        public SortedList FunctionsLinks { get { return functionsLinks; } }
        public SortedList TablesLinks { get { return tablesLinks; } }
        public SortedList RegistersLinks { get { return registersLinks; } }

        public RegisterLinks()
        {
            functionsLinks = new SortedList();
            tablesLinks = new SortedList();
            registersLinks = new SortedList();
        }

        public void addFunctionLink(string uniqueAddress, bool setInputRegister, bool setOutputRegister, bool setOutputRegisterByte)
        {
            RegisterFunctionLink lLink = (RegisterFunctionLink)functionsLinks[uniqueAddress];
            if (lLink == null)
            {
                lLink = new RegisterFunctionLink(uniqueAddress);
                functionsLinks.Add(uniqueAddress, lLink);
            }
            lLink.SetRegisterRole(setInputRegister, setOutputRegister, setOutputRegisterByte);
        }

        public RegisterFunctionLink getFunctionLink(string uniqueAddress)
        {
            return (RegisterFunctionLink)functionsLinks[uniqueAddress];
        }

        public void addTableLink(string uniqueAddress, bool setColumnsScaler, bool setRowsScaler, bool setOutputRegister, bool setOutputRegisterByte)
        {
            RegisterTableLink lLink = (RegisterTableLink)tablesLinks[uniqueAddress];
            if (lLink == null)
            {
                lLink = new RegisterTableLink(uniqueAddress);
                tablesLinks.Add(uniqueAddress, lLink);
            }
            lLink.SetRegisterRole(setColumnsScaler, setRowsScaler, setOutputRegister, setOutputRegisterByte);

        }

        public RegisterTableLink getTableLink(string uniqueAddress)
        {
            return (RegisterTableLink)tablesLinks[uniqueAddress];
        }

        public void addRegisterLink(string registerUniqueAddress, bool setSourceThroughFunction, bool setDestinationThroughFunction, string relatedFunctionUniqueAddress, bool scalerContext)
        {
            RegisterRegisterLink lLink = (RegisterRegisterLink)registersLinks[registerUniqueAddress];
            if (lLink == null)
            {
                lLink = new RegisterRegisterLink(registerUniqueAddress);
                registersLinks.Add(registerUniqueAddress, lLink);
            }
            lLink.SetRegisterRole(setSourceThroughFunction, setDestinationThroughFunction, relatedFunctionUniqueAddress, scalerContext);

        }

        public RegisterRegisterLink getRegisterLink(string uniqueAddress)
        {
            return (RegisterRegisterLink)registersLinks[uniqueAddress];
        }
    }
    
    // Register
    public class Register
    {
        public int AddressInt = -1;
        public string AdditionalAddress10 = string.Empty;

        public bool is8061KAMRegister = false;
        public bool is8061CCRegister = false;
        public bool is8061ECRegister = false;

        public RBase RBase = null;
        public RConst RConst = null;
        public EecRegister EecRegister = null;

        public S6xRegister S6xRegister = null;

        public RegisterLinks Links = null;
        
        public string Address
        {
            get
            {
                if (isDual) return string.Format("{0:x2}", AddressInt) + SADDef.AdditionSeparator + string.Format("{0:x2}", Convert.ToInt32(AdditionalAddress10));
                else return string.Format("{0:x2}", AddressInt);
            }
        }

        public string UniqueAddress
        {
            get
            {
                if (isDual) return Tools.RegisterUniqueAddress(AddressInt) + SADDef.AdditionSeparator + AdditionalAddress10;
                else return Tools.RegisterUniqueAddress(AddressInt);
            }
        }

        public int UniqueAddressInt
        {
            get
            {
                if (isDual) return AddressInt + Convert.ToInt32(AdditionalAddress10);
                else return AddressInt;
            }
        }

        public bool isDual { get { return !(AdditionalAddress10 == null || AdditionalAddress10 == string.Empty); } }

        public string Instruction { get { return Tools.RegisterInstruction(Address); } } 

        public Register(int addressInt)
        {
            AddressInt = addressInt;
        }

        public Register(string address)
        {
            if (address.Contains(SADDef.AdditionSeparator))
            {
                string regPart1 = address.Substring(0, address.IndexOf(SADDef.AdditionSeparator));
                string regPart2 = address.Replace(regPart1 + SADDef.AdditionSeparator, string.Empty);

                AddressInt = Convert.ToInt32(regPart1, 16);
                AdditionalAddress10 = Convert.ToString(Convert.ToInt32(regPart2, 16), 10);
            }
            else
            {
                AddressInt = Convert.ToInt32(address, 16);
            }
        }
    }

    // EecRegister

    public enum EecRegisterCheck
    {
        DataType,
        ReadWrite,
        Both,
        None
    }

    public class EecRegister
    {
        public string Code = string.Empty;
        public EecRegisterCheck Check = EecRegisterCheck.None;

        public string InstructionTrans = string.Empty;

        public string TranslationReadWord = string.Empty;
        public string TranslationReadByte = string.Empty;
        public string TranslationWriteWord = string.Empty;
        public string TranslationWriteByte = string.Empty;

        public string CommentsReadWord = string.Empty;
        public string CommentsReadByte = string.Empty;
        public string CommentsWriteWord = string.Empty;
        public string CommentsWriteByte = string.Empty;

        public EecRegister(string code)
        {
            Code = code;
            InstructionTrans = Tools.RegisterInstruction(Code);
            Check = EecRegisterCheck.None;
        }
    }

    // Vectors
    public class Vector
    {
        public int SourceBankNum = -1;
        public int ApplyOnBankNum = -1;

        public int SourceAddressInt = -1;
        public int AddressInt = -1;

        public string InitialValue = string.Empty;

        public int Number = -1;

        public bool isValid = false;

        public string Label = string.Empty;
        public string ShortLabel = string.Empty;
        public string Comments = string.Empty;

        public Structure VectList = null;
        
        public string SourceAddress { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + SourceAddressInt); } }
        public string Address { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInt); } }

        public string UniqueSourceAddress { get { return string.Format("{0,1} {1,5}", SourceBankNum, SourceAddressInt); } }
        public string UniqueSourceAddressHex { get { return string.Format("{0,1} {1,4}", SourceBankNum, SourceAddress); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", ApplyOnBankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", ApplyOnBankNum, Address); } }

        public string FullLabel
        {
            get
            {
                if (Label != string.Empty && ShortLabel != string.Empty && Label != ShortLabel) return ShortLabel + " - " + Label;
                else return Label;
            }
        }

        public Vector()
        {

        }
    }

    public class MatchingSignature
    {
        public int BankNum = -1;

        public int MatchingStartAddressInt = -1;
        public string MatchingBytes = string.Empty;

        public SortedList slMatchingParameters = null;

        public S6xSignature S6xSignature = null;

        public string MatchingStartAddress { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + MatchingStartAddressInt); } }

        public string UniqueMatchingStartAddress { get { return string.Format("{0,1} {1,5}", BankNum, MatchingStartAddressInt); } }
        public string UniqueMatchingStartAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, MatchingStartAddress); } }

        public MatchingSignature(int bankNum, int matchingStartAddressInt, string matchingBytes, S6xSignature s6xSignature)
        {
            BankNum = bankNum;
            MatchingStartAddressInt = matchingStartAddressInt;
            MatchingBytes = matchingBytes;
            slMatchingParameters = new SortedList();
            S6xSignature = s6xSignature;
        }
    }

    public class MatchingElementSignature
    {
        public int BankNum = -1;

        public int MatchingEAOPAddressInt = -1;
        public int MatchingStartAddressInt = -1;

        public string MatchingBytes = string.Empty;

        public bool EAOPStart = false;
        public bool EAOPEnd = false;
        public string EAOPBytes = string.Empty;

        public S6xElementSignature S6xElementSignature = null;

        public string MatchingEAOPAddress { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + MatchingEAOPAddressInt); } }
        public string MatchingStartAddress { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + MatchingStartAddressInt); } }

        public string UniqueMatchingAddress { get { return string.Format("{0,1} {1,5}", BankNum, EAOPStart ? MatchingStartAddressInt : MatchingEAOPAddressInt); } }
        public string UniqueMatchingAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, EAOPStart ? MatchingStartAddress : MatchingEAOPAddress); } }

        public MatchingElementSignature(int bankNum, int matchingEAOPAddressInt, int matchingStartAddressInt, string matchingBytes, bool eaopStart, bool eaopEnd, string eaopBytes, S6xElementSignature s6xElementSignature)
        {
            BankNum = bankNum;
            MatchingEAOPAddressInt = matchingEAOPAddressInt;
            MatchingStartAddressInt = matchingStartAddressInt;
            MatchingBytes = matchingBytes;
            EAOPStart = eaopStart;
            EAOPEnd = eaopEnd;
            EAOPBytes = eaopBytes;
            S6xElementSignature = s6xElementSignature;
        }
    }

    public enum CallType
    {
        Unknown,
        Skip,
        Goto,
        ShortCall,
        ShortJump,
        Call,
        Jump
    }

    public enum CallArgsType
    {
        Unknown,
        None,
        Fixed,
        FixedCyCond,
        Variable,
        VariableExternalRegister
    }

    // Call Args Mode
    public enum CallArgsMode
    {
        Unknown = 0,
        Standard = 1,   // Not Element
        Mode0 = 2,      // Element without RBase
        Mode1 = 3,
        Mode2 = 4,
        Mode3 = 5,
        Mode4 = 6,
        Mode4Struct = 7
    }

    public class CallArgumentDetection
    {
        public int StackDepth = -1;
        public string StackReadRegister = string.Empty;

        public bool CountModeOn = false;

        public CallArgument CallArgument = null;
        
        public CallArgumentDetection(int stackDepth, string stackReadRegister)
        {
            StackDepth = stackDepth;
            StackReadRegister = stackReadRegister;
        }

        public void CreateCallArgument(int outputRegisterAddressInt)
        {
            CallArgument = new CallArgument();
            CallArgument.StackDepth = StackDepth;
            CallArgument.StackReadRegister = StackReadRegister;

            CallArgument.OutputRegisterAddressInt = outputRegisterAddressInt;
        }
    }

    public class CallArgument
    {
        public int StackDepth = -1;
        public string StackReadRegister = string.Empty;

        public int OutputRegisterAddressInt = -1;
        public bool Word = false;
        public CallArgsMode Mode = CallArgsMode.Unknown;

        public string OutputRegisterAddress { get { return Convert.ToString(OutputRegisterAddressInt, 16); } }
        public int ByteSize { get { if (Word) return 2; else return 1; } }

        // At Operation level Only
        public int Position = -1;
        public int InputValueInt = -1;
        public int DecryptedValueInt = -1;

        public S6xRoutineInputArgument S6xRoutineInputArgument = null;
        
        public string Code { get { return Tools.ArgumentCode(Position); } }
        public string InputValue { get { return Convert.ToString(InputValueInt, 16); } }
        public string DecryptedValue { get { return Convert.ToString(DecryptedValueInt, 16); } }

        public CallArgument Clone()
        {
            CallArgument clone = new CallArgument();
            clone.StackDepth = StackDepth;
            clone.StackReadRegister = StackReadRegister;
            clone.OutputRegisterAddressInt = OutputRegisterAddressInt;
            clone.Word = Word;
            clone.Mode = Mode;

            clone.Position = Position;
            clone.InputValueInt = InputValueInt;
            clone.DecryptedValueInt = DecryptedValueInt;

            clone.S6xRoutineInputArgument = S6xRoutineInputArgument;

            return clone;
        }
    }
    
    // Callers
    public class Caller
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public CallType CallType = CallType.Unknown;

        public string Address { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInt); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public Caller(int bankNum, int addressInt, CallType callType)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            CallType = callType;
        }
    }
    
    // Calls
    public class Call
    {
        public int BankNum = -1;
        
        public int AddressInt = -1;
        public int AddressEndInt = -1;

        public CallType CallType = CallType.Unknown;
        
        public int ByteArgsNum = 0;
        public int ArgsNumCondAdder = 0;
        public int ArgsStackDepthMax = -1;
        public CallArgsType ArgsType = CallArgsType.Unknown;
        public int ArgsVariableOutputFirstRegisterAddress = -1;
        public string ArgsVariableExternalRegister = string.Empty;
        public int ArgsCondValue = -1;
        public bool ArgsCondValidated = false;
        public CallArgsMode[] ArgsModes = null;
        public CallArgument[] Arguments = null;
        public bool isArgsModeCore = false;

        public bool isIntVector = false;
        public bool isVector = false;
        public bool isRoutine = false;
        public bool isFake = false;

        public ArrayList Callers = new ArrayList();

        private string label = string.Empty;
        private string shortLabel = string.Empty;

        public S6xRoutine S6xRoutine = null;
        
        public int UseCount { get { return Callers.Count; } }
        
        public string Address { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInt); } }
        public string AddressEnd { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressEndInt); } }

        public bool isIdentified { get { return isIntVector || isVector || isRoutine || isFake; } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public string Label
        {
            get
            {
                if (S6xRoutine == null && label == string.Empty)
                {
                    if (isFake) return SADDef.LongCallFakePrefix + UniqueAddressHex;
                    return SADDef.LongCallPrefix + UniqueAddressHex;
                }
                else if (S6xRoutine == null) return label;
                else if (label == string.Empty && (S6xRoutine.Label == null || S6xRoutine.Label == string.Empty))
                {
                    if (isFake) return SADDef.LongCallFakePrefix + UniqueAddressHex;
                    return SADDef.LongCallPrefix + UniqueAddressHex;
                }
                else return S6xRoutine.Label;
            }
            set { label = value; }
        }

        public string ShortLabel
        {
            get
            {
                if (S6xRoutine == null && shortLabel == string.Empty) return Address;
                else if (S6xRoutine == null) return shortLabel;
                else if (shortLabel == string.Empty && (S6xRoutine.ShortLabel == null || S6xRoutine.ShortLabel == string.Empty)) return Address;
                else return S6xRoutine.ShortLabel;
            }
            set { shortLabel = value; }
        }

        public string FullLabel
        {
            get
            {
                if (Label != string.Empty && ShortLabel != string.Empty && Label != ShortLabel) return ShortLabel + " - " + Label;
                else return Label;
            }
        }

        public string Comments
        {
            get
            {
                if (S6xRoutine == null) return string.Empty;
                else if (S6xRoutine.Comments == null) return string.Empty;
                else if (!S6xRoutine.OutputComments) return string.Empty;
                else return S6xRoutine.Comments;
            }
        }

        public bool OutputComments
        {
            get
            {
                if (S6xRoutine == null) return false;
                else return S6xRoutine.OutputComments;
            }
        }

        public Call(int bankNum, int addressInt, CallType callType, int callerBankNum, int callerAddressInt)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            AddressEndInt = AddressInt;
            CallType = callType;

            Callers.Add(new Caller(callerBankNum, callerAddressInt, callType));
        }

        // AddCall add a Caller, can promote Call Type and returns true if Call should be identified
        public bool AddCaller(int callerBankNum, int callerAddressInt, CallType callType)
        {
            Callers.Add(new Caller(callerBankNum, callerAddressInt, callType));

            // Call Type Promotion
            switch (CallType)
            {
                case CallType.Goto:
                case CallType.Skip:
                case CallType.Unknown:
                    switch (callType)
                    {
                        case CallType.Call:
                        case CallType.Jump:
                        case CallType.ShortCall:
                        case CallType.ShortJump:
                            CallType = callType;
                            if (!isIdentified) return true;
                            break;
                    }
                    break;
                case CallType.ShortCall:
                case CallType.ShortJump:
                    switch (callType)
                    {
                        case CallType.Call:
                        case CallType.Jump:
                            CallType = callType;
                            if (!isIdentified) return true;
                            break;
                    }
                    break;
            }

            return false;
        }
    }

    // Routines
    public enum RoutineType
    {
        Unknown,
        TableByte,
        TableWord,
        FunctionWord,
        FunctionByte,
        Other
    }

    // Routines
    public enum RoutineCode
    {
        Unknown,
        Checksum,
        Init,
        CoreInit,
        TableCore
    }

    public class RoutineIOStructure : RoutineIO
    {
        public string StructureNumberRegister = string.Empty;

        public S6xRoutineInputStructure S6xInputStructure = null;

        public RoutineIOStructure() { }

        protected RoutineIOStructure(RoutineIOStructure copy)
            : base(copy)
        {
            StructureNumberRegister = copy.StructureNumberRegister;

            S6xInputStructure = copy.S6xInputStructure;
        }

        public override object Clone()
        {
            return new RoutineIOStructure(this);
        }
    }

    public class RoutineIOTable : RoutineIO
    {
        public bool TableWord = false;

        public string TableColRegister = string.Empty;
        public string TableRowRegister = string.Empty;
        public string TableColNumberRegister = string.Empty;
        public string TableOutputRegisterByte = string.Empty;

        public S6xRoutineInputTable S6xInputTable = null;

        public RoutineIOTable() { }

        protected RoutineIOTable(RoutineIOTable copy)
            : base(copy)
        {
            TableWord = copy.TableWord;
            
            TableColRegister = copy.TableColRegister;
            TableRowRegister = copy.TableRowRegister;
            TableColNumberRegister = copy.TableColNumberRegister;
            TableOutputRegisterByte = copy.TableOutputRegisterByte;

            S6xInputTable = copy.S6xInputTable;
        }

        public override object Clone()
        {
            return new RoutineIOTable(this);
        }
    }

    public class RoutineIOFunction : RoutineIO
    {
        public bool FunctionByte = false;
        
        public string FunctionInputRegister = string.Empty;
        public bool FunctionSignedInput = false;

        public string FunctionSignInputCondRegister = string.Empty;
        public int FunctionSignInputCondBit = -1;
        public int FunctionSignedInputCondValue = -1;
        public int FunctionUnSignedInputCondValue = -1;

        public bool isFunctionSignedInputDefined = false;

        public S6xRoutineInputFunction S6xInputFunction = null;

        public RoutineIOFunction() { }

        protected RoutineIOFunction(RoutineIOFunction copy)
            : base(copy)
        {
            FunctionByte = copy.FunctionByte;
            
            FunctionInputRegister = copy.FunctionInputRegister;
            FunctionSignedInput = copy.FunctionSignedInput;

            FunctionSignInputCondRegister = copy.FunctionSignInputCondRegister;
            FunctionSignInputCondBit = copy.FunctionSignInputCondBit;
            FunctionSignedInputCondValue = copy.FunctionSignedInputCondValue;
            FunctionUnSignedInputCondValue = copy.FunctionUnSignedInputCondValue;

            isFunctionSignedInputDefined = copy.isFunctionSignedInputDefined;

            S6xInputFunction = copy.S6xInputFunction;
        }

        public override object Clone()
        {
            return new RoutineIOFunction(this);
        }
        
        public void setFunctionSignedInput(byte condValue)
        {
            if (FunctionSignInputCondBit < 0 && FunctionSignedInputCondValue < 0) return;

            if (FunctionSignInputCondBit >= 0)
            // Bit Condition
            {
                if (FunctionSignedInputCondValue >= 0)
                {
                    FunctionSignedInput = Tools.checkBit(FunctionSignInputCondBit, (FunctionSignedInputCondValue == 1), condValue);
                    isFunctionSignedInputDefined = true;
                }
                else if (FunctionUnSignedInputCondValue >= 0)
                {
                    FunctionSignedInput = !Tools.checkBit(FunctionSignInputCondBit, (FunctionSignedInputCondValue == 1), condValue);
                    isFunctionSignedInputDefined = true;
                }
            }
            else
            // Value Condition
            {
                if (FunctionSignedInputCondValue >= 0)
                {
                    FunctionSignedInput = (FunctionSignedInputCondValue == condValue);
                    isFunctionSignedInputDefined = true;
                }
                else if (FunctionUnSignedInputCondValue >= 0)
                {
                    FunctionSignedInput = (FunctionSignedInputCondValue != condValue);
                    isFunctionSignedInputDefined = true;
                }
            }
        }
    }

    public class RoutineIOScalar : RoutineIO
    {
        public bool ScalarByte = false;
        public bool ScalarSigned = false;

        public S6xRoutineInputScalar S6xInputScalar = null;
        
        public RoutineIOScalar() { }

        protected RoutineIOScalar(RoutineIOScalar copy)
            : base(copy)
        {
            ScalarByte = copy.ScalarByte;
            ScalarSigned = copy.ScalarSigned;

            S6xInputScalar = copy.S6xInputScalar;
        }

        public override object Clone()
        {
            return new RoutineIOScalar(this);
        }
    }

    public class RoutineIO
    {
        public string AddressRegister = string.Empty;

        public string OutputRegister = string.Empty;
        public bool SignedOutput = false;

        public int FirstCondRegisterOpeAddress = -1;

        public string SignOutputCondRegister = string.Empty;
        public int SignOutputCondBit = -1;
        public int SignedOutputCondValue = -1;
        public int UnSignedOutputCondValue = -1;

        public bool isSignedOutputDefined = false;

        public RoutineIO() { }

        protected RoutineIO(RoutineIO copy)
        {
            AddressRegister = copy.AddressRegister;

            OutputRegister = copy.OutputRegister;
            SignedOutput = copy.SignedOutput;

            FirstCondRegisterOpeAddress = copy.FirstCondRegisterOpeAddress;

            SignOutputCondRegister = copy.SignOutputCondRegister;
            SignOutputCondBit = copy.SignOutputCondBit;
            SignedOutputCondValue = copy.SignedOutputCondValue;
            UnSignedOutputCondValue = copy.UnSignedOutputCondValue;

            isSignedOutputDefined = copy.isSignedOutputDefined;
        }

        public virtual object Clone()
        {
            return new RoutineIO(this);
        }

        public void setSignedOutput(byte condValue)
        {
            if (SignOutputCondBit < 0 && SignedOutputCondValue < 0) return;

            if (SignOutputCondBit >= 0)
            // Bit Condition
            {
                if (SignedOutputCondValue >= 0)
                {
                    SignedOutput = Tools.checkBit(SignOutputCondBit, (SignedOutputCondValue == 1), condValue);
                    isSignedOutputDefined = true;
                }
                else if (UnSignedOutputCondValue >= 0)
                {
                    SignedOutput = !Tools.checkBit(SignOutputCondBit, (SignedOutputCondValue == 1), condValue);
                    isSignedOutputDefined = true;
                }
            }
            else
            // Value Condition
            {
                if (SignedOutputCondValue >= 0)
                {
                    SignedOutput = (SignedOutputCondValue == condValue);
                    isSignedOutputDefined = true;
                }
                else if (UnSignedOutputCondValue >= 0)
                {
                    SignedOutput = (SignedOutputCondValue != condValue);
                    isSignedOutputDefined = true;
                }
            }
        }
    }
    
    public class Routine
    {
        public int BankNum = -1;
        
        public int AddressInt = -1;

        public RoutineType Type = RoutineType.Unknown;
        public RoutineCode Code = RoutineCode.Unknown;

        public RoutineIO[] IOs = null;

        /*
        public string AddressRegister = string.Empty;

        public string OutputRegister = string.Empty;
        public bool SignedOutput = false;
        
        public string FunctionInputRegister = string.Empty;
        public bool FunctionSignedInput = false;
        
        public string TableColRegister = string.Empty;
        public string TableRowRegister = string.Empty;
        public string TableColNumberRegister = string.Empty;
        public string TableOutputRegisterByte = string.Empty;

        public int FirstCondRegisterOpeAddress = -1;
        
        public string FunctionSignInputCondRegister = string.Empty;
        public int FunctionSignInputCondBit = -1;
        public int FunctionSignedInputCondValue = -1;
        public int FunctionUnSignedInputCondValue = -1;
        
        public string SignOutputCondRegister = string.Empty;
        public int SignOutputCondBit = -1;
        public int SignedOutputCondValue = -1;
        public int UnSignedOutputCondValue = -1;

        public bool isSignedOutputDefined = false;
        public bool isFunctionSignedInputDefined = false;
        */

        public string Label = string.Empty;
        public string ShortLabel = string.Empty;
        public string Comments = string.Empty;

        public S6xRoutine S6xRoutine = null;
        
        public string Address { get { return string.Format("{0:x4}", SADDef.EecBankStartAddress + AddressInt); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public Routine(int bankNum, int addressInt)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
        }

        public void SetTranslationComments()
        {
            RoutineIOTable ioTable = null;
            RoutineIOFunction ioFunction = null;
            bool word = false;
            bool signedInput = false;
            bool signedOutput = false;

            if (Code != RoutineCode.Unknown)
            {
                foreach (object[] defRoutine in SADDef.RoutinesCodes)
                {
                    if (Code == (RoutineCode)defRoutine[0])
                    {
                        ShortLabel = defRoutine[1].ToString();
                        Label = defRoutine[2].ToString();
                        Comments = defRoutine[3].ToString();
                        return;
                    }
                }
            }
            else if (Type != RoutineType.Unknown && Type != RoutineType.Other && IOs != null)
            {
                if (IOs.Length == 1)
                {
                    foreach (object[] defRoutine in SADDef.RoutinesTypes)
                    {
                        word = Convert.ToBoolean(defRoutine[1]);
                        signedInput = Convert.ToBoolean(defRoutine[2]);
                        signedOutput = Convert.ToBoolean(defRoutine[3]);
                        if (defRoutine[0].ToString().ToUpper() == "TABLE" && (Type == RoutineType.TableByte || Type == RoutineType.TableWord))
                        {
                            ioTable = (RoutineIOTable)IOs[0];
                            if (Type == RoutineType.TableByte && !word && ioTable.SignedOutput == signedOutput)
                            {
                                ShortLabel = defRoutine[4].ToString();
                                Label = defRoutine[5].ToString();
                                Comments = defRoutine[6].ToString();
                                Comments = Comments.Replace("%1%", Tools.RegisterInstruction(ioTable.AddressRegister));
                                Comments = Comments.Replace("%2%", Tools.RegisterInstruction(ioTable.TableColRegister));
                                Comments = Comments.Replace("%3%", Tools.RegisterInstruction(ioTable.TableRowRegister));
                                Comments = Comments.Replace("%4%", Tools.RegisterInstruction(ioTable.TableColNumberRegister));
                                Comments = Comments.Replace("%5%", Tools.RegisterInstruction(ioTable.OutputRegister));
                                ioTable = null;
                                return;
                            }
                            else if (Type == RoutineType.TableWord && word && ioTable.SignedOutput == signedOutput)
                            {
                                ShortLabel = defRoutine[4].ToString();
                                Label = defRoutine[5].ToString();
                                Comments = defRoutine[6].ToString();
                                Comments = Comments.Replace("%1%", Tools.RegisterInstruction(ioTable.AddressRegister));
                                Comments = Comments.Replace("%2%", Tools.RegisterInstruction(ioTable.TableColRegister));
                                Comments = Comments.Replace("%3%", Tools.RegisterInstruction(ioTable.TableRowRegister));
                                Comments = Comments.Replace("%4%", Tools.RegisterInstruction(ioTable.TableColNumberRegister));
                                Comments = Comments.Replace("%5%", Tools.RegisterInstruction(ioTable.OutputRegister));
                                ioTable = null;
                                return;
                            }
                        }
                        else if (defRoutine[0].ToString().ToUpper() == "FUNCTION" && (Type == RoutineType.FunctionByte || Type == RoutineType.FunctionWord))
                        {
                            ioFunction = (RoutineIOFunction)IOs[0];
                            if (Type == RoutineType.FunctionByte && !word && ioFunction.SignedOutput == signedOutput && ioFunction.FunctionSignedInput == signedInput)
                            {
                                ShortLabel = defRoutine[4].ToString();
                                Label = defRoutine[5].ToString();
                                Comments = defRoutine[6].ToString();
                                Comments = Comments.Replace("%1%", Tools.RegisterInstruction(ioFunction.AddressRegister));
                                Comments = Comments.Replace("%2%", Tools.RegisterInstruction(ioFunction.FunctionInputRegister));
                                Comments = Comments.Replace("%3%", Tools.RegisterInstruction(ioFunction.OutputRegister));
                                ioFunction = null;
                                return;
                            }
                            else if (Type == RoutineType.FunctionWord && word && ioFunction.SignedOutput == signedOutput && ioFunction.FunctionSignedInput == signedInput)
                            {
                                ShortLabel = defRoutine[4].ToString();
                                Label = defRoutine[5].ToString();
                                Comments = defRoutine[6].ToString();
                                Comments = Comments.Replace("%1%", Tools.RegisterInstruction(ioFunction.AddressRegister));
                                Comments = Comments.Replace("%2%", Tools.RegisterInstruction(ioFunction.FunctionInputRegister));
                                Comments = Comments.Replace("%3%", Tools.RegisterInstruction(ioFunction.OutputRegister));
                                ioFunction = null;
                                return;
                            }
                        }
                    }
                }
            }
        }

        public RoutineIO[] CloneIOs()
        {
            RoutineIO[] resIOs = null;

            if (IOs == null) return resIOs;

            resIOs = new RoutineIO[IOs.Length];

            for (int iIO = 0; iIO < IOs.Length; iIO++)
            {
                if (IOs[iIO].GetType() == typeof(RoutineIOTable)) resIOs[iIO] = (RoutineIOTable)IOs[iIO].Clone();
                else if (IOs[iIO].GetType() == typeof(RoutineIOFunction)) resIOs[iIO] = (RoutineIOFunction)IOs[iIO].Clone();
                else if (IOs[iIO].GetType() == typeof(RoutineIOStructure)) resIOs[iIO] = (RoutineIOStructure)IOs[iIO].Clone();
                else if (IOs[iIO].GetType() == typeof(RoutineIOScalar)) resIOs[iIO] = (RoutineIOScalar)IOs[iIO].Clone();
                else resIOs[iIO] = (RoutineIO)IOs[iIO].Clone();
            }

            return resIOs;
        }
    }

    // Calibration Element
    public class CalibrationElement
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressBinInt = -1;
        
        public string RBase = string.Empty;

        public Scalar ScalarElem = null;
        public Function FunctionElem = null;
        public Table TableElem = null;
        public Structure StructureElem = null;

        public string RBaseCalc = string.Empty;

        public ArrayList RelatedOpsUniqueAddresses = new ArrayList();

        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }
        public int AddressEndInt { get { return AddressInt + Size - 1; } }
        public string AddressEnd { get { return string.Format("{0:x4}", AddressEndInt + SADDef.EecBankStartAddress); } }
        public int Size
        {
            get
            {
                if (isScalar) return ScalarElem.Size;
                else if (isFunction) return FunctionElem.Size;
                else if (isTable) return TableElem.Size;
                else if (isStructure) return StructureElem.Size;
                else return 0;
            }
        }
        public bool isScalar { get { return ScalarElem != null; } }
        public bool isFunction { get {return FunctionElem != null; }}
        public bool isTable { get {return TableElem != null; }}
        public bool isStructure { get { return StructureElem != null; } }

        public bool isTypeIdentified { get { return isScalar || isFunction || isTable || isStructure; } }
        public bool isFullyIdentified
        {
            get
            {
                if (isFunction) return true;
                else if (isTable) return TableElem.ColsNumber > 0;
                else if (isStructure) return (StructureElem.Number > 0 && StructureElem.StructDefString != string.Empty);
                else if (isScalar) return ScalarElem.isIdentified;
                else return false;
            }
        }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public CalibrationElement(int bankNum, string rBaseCode)
        {
            BankNum = bankNum;
            RBase = rBaseCode;
        }
    }

    // TableScaler
    public class TableScaler
    {
        public ArrayList InputRegistersAddresses = null;
        public ArrayList InputFunctionsUniqueAddresses = null;
        public ArrayList ColsScaledTablesUniqueAddresses = null;
        public ArrayList RowsScaledTablesUniqueAddresses = null;

        public TableScaler()
        {
            InputRegistersAddresses = new ArrayList();
            InputFunctionsUniqueAddresses = new ArrayList();
            ColsScaledTablesUniqueAddresses = new ArrayList();
            RowsScaledTablesUniqueAddresses = new ArrayList();
        }

        public void addRegister(string registerAddress)
        {
            if (registerAddress == null || registerAddress == string.Empty) return;
            if (InputRegistersAddresses.Contains(registerAddress)) return;
            InputRegistersAddresses.Add(registerAddress);
        }

        public void addFunction(string functionUniqueAddress)
        {
            if (functionUniqueAddress == null || functionUniqueAddress == string.Empty) return;
            if (InputFunctionsUniqueAddresses.Contains(functionUniqueAddress)) return;
            InputFunctionsUniqueAddresses.Add(functionUniqueAddress);
        }

        public void addColsScaledTable(string tableUniqueAddress)
        {
            if (tableUniqueAddress == null || tableUniqueAddress == string.Empty) return;
            if (ColsScaledTablesUniqueAddresses.Contains(tableUniqueAddress)) return;
            ColsScaledTablesUniqueAddresses.Add(tableUniqueAddress);
        }

        public void addRowsScaledTable(string tableUniqueAddress)
        {
            if (tableUniqueAddress == null || tableUniqueAddress == string.Empty) return;
            if (RowsScaledTablesUniqueAddresses.Contains(tableUniqueAddress)) return;
            RowsScaledTablesUniqueAddresses.Add(tableUniqueAddress);
        }
    }

    // RoutineCallInfoTable
    public class RoutineCallInfoTable
    {
        public string CalibrationElementOpeUniqueAddress = string.Empty;
        public string RoutineUniqueAddress = string.Empty;
        public string RoutineCallOpeUniqueAddress = string.Empty;

        public RoutineIOTable RoutineInputOutput = null;

        public string ColsScalerRegister = string.Empty;
        public string RowsScalerRegister = string.Empty;
        public string ColsScalerFunctionUniqueAddress = string.Empty;
        public string RowsScalerFunctionUniqueAddress = string.Empty;
        public string OutputRegister = string.Empty;
        public string OutputRegisterByte = string.Empty;
        public bool OutputRegisterSigned = false;
    }

    // Table
    public class Table
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressBinInt = -1;

        public bool WordOutput = false;

        public bool SignedOutput = false;
        public int ColsNumber = -1;

        public ScalarLine[] Lines = null;

        public string RBase = string.Empty;
        public string RBaseCalc = string.Empty;

        public ArrayList RoutinesCallsInfos = new ArrayList();
        public string ColsScalerUniqueAddress = string.Empty;
        public string RowsScalerUniqueAddress = string.Empty;
        public string CellsScaleExpression = "X";
        public int CellsScalePrecision = SADDef.DefaultScalePrecision;

        public string ColsUnits = string.Empty;
        public string RowsUnits = string.Empty;
        public string CellsUnits = string.Empty;

        private string label = string.Empty;
        private string shortLabel = string.Empty;

        private string comments = string.Empty;

        public S6xTable S6xTable = null;
        public S6xElementSignature S6xElementSignatureSource = null;

        public ArrayList OtherRelatedOpsUniqueAddresses = new ArrayList();

        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }
        public int AddressEndInt { get { return AddressInt + ((Size == 0) ? 0 : Size - 1); } }
        public string AddressEnd { get { return string.Format("{0:x4}", AddressEndInt + SADDef.EecBankStartAddress); } }
        public int Size { get { if (Lines == null) return 0; else return Lines.Length * SizeLine; } }
        public int SizeLine { get { if (WordOutput) return ColsNumber * 2; else return ColsNumber; } }
        public bool HasValues { get { return (Lines != null); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public bool isCellsScaled { get { return (getCellsScaleExpression.ToLower().Trim() != "x"); } }

        public string getCellsScaleExpression
        {
            get
            {
                if (S6xTable == null) return CellsScaleExpression;
                if (S6xTable.CellsScaleExpression == null || S6xTable.CellsScaleExpression == string.Empty) return CellsScaleExpression;
                return S6xTable.CellsScaleExpression;
            }
        }

        public int getCellsScalePrecision
        {
            get
            {
                if (S6xTable == null) return CellsScalePrecision;
                return S6xTable.CellsScalePrecision;
            }
        }

        public string Label
        {
            get
            {
                if (S6xTable == null && label == string.Empty) return string.Empty;
                else if (S6xTable == null) return label;
                else if (label == string.Empty && (S6xTable.Label == null || S6xTable.Label == string.Empty)) return string.Empty;
                else return S6xTable.Label;
            }
            set { label = value; }
        }

        public string ShortLabel
        {
            get
            {
                if (S6xTable == null && shortLabel == string.Empty) return Address;
                else if (S6xTable == null) return shortLabel;
                else if (shortLabel == string.Empty && (S6xTable.ShortLabel == null || S6xTable.ShortLabel == string.Empty)) return Address;
                else return S6xTable.ShortLabel;
            }
            set { shortLabel = value; }
        }

        public string FullLabel
        {
            get
            {
                if (Label != string.Empty && ShortLabel != string.Empty && Label != ShortLabel) return ShortLabel + " - " + Label;
                else if (Label == string.Empty) return SADDef.LongTablePrefix + UniqueAddressHex;
                else return Label;
            }
        }

        public string Comments
        {
            get
            {
                if (S6xTable == null) return comments;
                else if (S6xTable.Comments == null) return string.Empty;
                else if (!S6xTable.OutputComments) return string.Empty;
                else return S6xTable.Comments; 
            }
            set { comments = value; }
        }

        public bool OutputComments
        {
            get
            {
                if (S6xTable == null) return false;
                else return S6xTable.OutputComments;
            }
        }

        public Table()
        {

        }

        public Table(S6xTable s6xTable)
        {
            BankNum = s6xTable.BankNum;
            AddressInt = s6xTable.AddressInt;
            WordOutput = s6xTable.WordOutput;
            SignedOutput = s6xTable.SignedOutput;
            ColsNumber = s6xTable.ColsNumber;

            CellsScaleExpression = s6xTable.CellsScaleExpression;
            CellsScalePrecision = s6xTable.CellsScalePrecision;

            S6xTable = s6xTable;
        }

        public void Read(string[] arrBytes)
        {
            int rowsNum = 0;
            int iPos = 0;
            string[] arrScalarBytes = null;

            if (S6xTable != null)
            {
                ColsNumber = S6xTable.ColsNumber;
                WordOutput = S6xTable.WordOutput;
                SignedOutput = S6xTable.SignedOutput;
            }

            if (ColsNumber <= 0) return;

            if (WordOutput)
            {
                rowsNum = arrBytes.Length / (ColsNumber * 2);
                if (rowsNum * ColsNumber * 2 != arrBytes.Length) return;
            }
            else
            {
                rowsNum = arrBytes.Length / ColsNumber;
                if (rowsNum * ColsNumber != arrBytes.Length) return;
            }

            Lines = new ScalarLine[rowsNum];

            iPos = 0;
            for (int iRow = 0; iRow < Lines.Length; iRow++)
            {
                Lines[iRow] = new ScalarLine(BankNum, AddressInt + iPos, ColsNumber);

                for (int iCol = 0; iCol < Lines[iRow].Scalars.Length; iCol++)
                {
                    Lines[iRow].Scalars[iCol] = new Scalar(BankNum, AddressInt + iPos, !WordOutput, SignedOutput);
                    arrScalarBytes = new string[Lines[iRow].Scalars[iCol].Size];
                    for (int iByte = 0; iByte < arrScalarBytes.Length; iByte++) arrScalarBytes[iByte] = arrBytes[iByte + iPos];
                    iPos += arrScalarBytes.Length;
                    Lines[iRow].Scalars[iCol].Read(arrScalarBytes);
                    arrScalarBytes = null;
                }
            }
        }
    }

    // RoutineCallInfoFunction
    public class RoutineCallInfoFunction
    {
        public string CalibrationElementOpeUniqueAddress = string.Empty;
        public string RoutineUniqueAddress = string.Empty;
        public string RoutineCallOpeUniqueAddress = string.Empty;

        public RoutineIOFunction RoutineInputOutput = null;

        public string InputRegister = string.Empty;
        public string OutputRegister = string.Empty;
        public string OutputRegisterByte = string.Empty;
        public bool OutputRegisterSigned = false;
    }

    // Function
    public class Function
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressBinInt = -1;

        public bool ByteInput = false;
        public bool ByteOutput = false;
        public bool SignedInput = false;
        public bool SignedOutput = false;

        public ScalarLine[] Lines = null;

        public string RBase = string.Empty;
        public string RBaseCalc = string.Empty;

        public ArrayList RoutinesCallsInfos = new ArrayList();
        public int ScalerItemsNum = 0;
        public string InputScaleExpression = "X";
        public string OutputScaleExpression = "X";
        public int InputScalePrecision = SADDef.DefaultScalePrecision;
        public int OutputScalePrecision = SADDef.DefaultScalePrecision;

        public string InputUnits = string.Empty;
        public string OutputUnits = string.Empty;

        private string label = string.Empty;
        private string shortLabel = string.Empty;

        private string comments = string.Empty;

        public S6xFunction S6xFunction = null;
        public S6xElementSignature S6xElementSignatureSource = null;

        public ArrayList OtherRelatedOpsUniqueAddresses = new ArrayList();

        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }
        public int AddressEndInt { get { return AddressInt + ((Size == 0) ? 0 : Size - 1); } }
        public string AddressEnd { get { return string.Format("{0:x4}", AddressEndInt + SADDef.EecBankStartAddress); } }
        public int Size { get { if (Lines == null) return 0; else return Lines.Length * SizeLine; } }
        public int SizeLine
        {
            get
            {
                if (ByteInput && ByteOutput) return 2;
                else if (!ByteInput && !ByteOutput) return 4;
                else return 3;
            }
        }
        public bool HasValues { get { return Lines != null; } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public bool isInputScaled { get { return (getInputScaleExpression.ToLower().Trim() != "x"); } }
        public bool isOutputScaled { get { return (getOutputScaleExpression.ToLower().Trim() != "x"); } }

        public string getInputScaleExpression
        {
            get
            {
                if (S6xFunction == null) return InputScaleExpression;
                if (S6xFunction.InputScaleExpression == null || S6xFunction.InputScaleExpression == string.Empty) return InputScaleExpression;
                return S6xFunction.InputScaleExpression;
            }
        }

        public string getOutputScaleExpression
        {
            get
            {
                if (S6xFunction == null) return OutputScaleExpression;
                if (S6xFunction.OutputScaleExpression == null || S6xFunction.OutputScaleExpression == string.Empty) return OutputScaleExpression;
                return S6xFunction.OutputScaleExpression;
            }
        }

        public int getInputScalePrecision
        {
            get
            {
                if (S6xFunction == null) return InputScalePrecision;
                return S6xFunction.InputScalePrecision;
            }
        }

        public int getOutputScalePrecision
        {
            get
            {
                if (S6xFunction == null) return OutputScalePrecision;
                return S6xFunction.OutputScalePrecision;
            }
        }

        public string Label
        {
            get
            {
                if (S6xFunction == null && label == string.Empty) return string.Empty;
                else if (S6xFunction == null) return label;
                else if (label == string.Empty && (S6xFunction.Label == null || S6xFunction.Label == string.Empty)) return string.Empty;
                else return S6xFunction.Label;
            }
            set { label = value; }
        }

        public string ShortLabel
        {
            get
            {
                if (S6xFunction == null && shortLabel == string.Empty) return Address;
                else if (S6xFunction == null) return shortLabel;
                else if (shortLabel == string.Empty && (S6xFunction.ShortLabel == null || S6xFunction.ShortLabel == string.Empty)) return Address;
                else return S6xFunction.ShortLabel;
            }
            set { shortLabel = value; }
        }

        public string FullLabel
        {
            get
            {
                if (Label != string.Empty && ShortLabel != string.Empty && Label != ShortLabel) return ShortLabel + " - " + Label;
                else if (Label == string.Empty) return SADDef.LongFunctionPrefix + UniqueAddressHex;
                else return Label;
            }
        }

        public string Comments
        {
            get
            {
                if (S6xFunction == null) return comments;
                else if (S6xFunction.Comments == null) return string.Empty;
                else if (!S6xFunction.OutputComments) return string.Empty;
                else return S6xFunction.Comments;
            }
            set { comments = value; }
        }

        public bool OutputComments
        {
            get
            {
                if (S6xFunction == null) return false;
                else return S6xFunction.OutputComments;
            }
        }

        public Function()
        {

        }

        public Function(S6xFunction s6xFunction)
        {
            BankNum = s6xFunction.BankNum;
            AddressInt = s6xFunction.AddressInt;
            ByteInput = s6xFunction.ByteInput;
            ByteOutput = s6xFunction.ByteOutput;
            SignedInput = s6xFunction.SignedInput;
            SignedOutput = s6xFunction.SignedOutput;

            InputScaleExpression = s6xFunction.InputScaleExpression;
            InputScalePrecision = s6xFunction.InputScalePrecision;
            OutputScaleExpression = s6xFunction.OutputScaleExpression;
            OutputScalePrecision = s6xFunction.OutputScalePrecision;
            
            S6xFunction = s6xFunction;
        }

        public void Read(string[] arrBytes)
        {
            int rowsNum = 0;
            int bytesPerRow = 0;
            int iPos = 0;
            string[] arrScalarBytes = null;

            if (S6xFunction != null)
            {
                ByteInput = S6xFunction.ByteInput;
                ByteOutput = S6xFunction.ByteOutput;
                SignedInput = S6xFunction.SignedInput;
                SignedOutput = S6xFunction.SignedOutput;
            }

            bytesPerRow = 2;
            if (!ByteInput) bytesPerRow++;
            if (!ByteOutput) bytesPerRow++;
            rowsNum = arrBytes.Length / bytesPerRow;

            if (rowsNum * bytesPerRow != arrBytes.Length) return;

            Lines = new ScalarLine[rowsNum];

            iPos = 0;
            for (int iRow = 0; iRow < Lines.Length; iRow++)
            {
                Lines[iRow] = new ScalarLine(BankNum, AddressInt + iPos, 2);

                Lines[iRow].Scalars[0] = new Scalar(BankNum , AddressInt + iPos, ByteInput, SignedInput);
                arrScalarBytes = new string[Lines[iRow].Scalars[0].Size];
                for (int iByte = 0; iByte < arrScalarBytes.Length; iByte++) arrScalarBytes[iByte] = arrBytes[iByte + iPos];
                iPos += arrScalarBytes.Length;
                Lines[iRow].Scalars[0].Read(arrScalarBytes);
                arrScalarBytes = null;

                Lines[iRow].Scalars[1] = new Scalar(BankNum, AddressInt + iPos, ByteOutput, SignedOutput);
                arrScalarBytes = new string[Lines[iRow].Scalars[1].Size];
                for (int iByte = 0; iByte < arrScalarBytes.Length; iByte++) arrScalarBytes[iByte] = arrBytes[iByte + iPos];
                iPos += arrScalarBytes.Length;
                Lines[iRow].Scalars[1].Read(arrScalarBytes);
                arrScalarBytes = null;
            }
        }
    }

    // Scalar Line
    public class ScalarLine
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public Scalar[] Scalars = null;

        public int AddressEndInt
        {
            get
            {
                if (Scalars == null) return AddressInt;
                if (Scalars.Length == 0) return AddressInt;
                return Scalars[Scalars.Length - 1].AddressEndInt;
            }
        }

        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }
        public string AddressEnd { get { return string.Format("{0:x4}", AddressEndInt + SADDef.EecBankStartAddress); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public string InitialValue
        {
            get
            {
                string sResult = string.Empty;
                for (int iCol = 0; iCol < Scalars.Length; iCol++)
                {
                    if (iCol > 0) sResult += SADDef.GlobalSeparator;
                    sResult += Scalars[iCol].InitialValue;
                }
                return sResult;
            }
        }
        
        public ScalarLine(int bankNum, int iAddress, int colsNumber)
        {
            BankNum = bankNum;
            AddressInt = iAddress;
            Scalars = new Scalar[colsNumber];
        }
    }

    // RoutineCallInfoScalar
    public class RoutineCallInfoScalar
    {
        public string CalibrationElementOpeUniqueAddress = string.Empty;
        public string RoutineUniqueAddress = string.Empty;
        public string RoutineCallOpeUniqueAddress = string.Empty;

        public RoutineIOScalar RoutineInputOutput = null;
    }

    // Scalar
    public class Scalar
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressBinInt = -1;

        public bool Byte = false;
        public bool Word = false;
        public bool UnSigned = false;
        public bool Signed = false;

        private bool foundBitFlags = false;
        
        public int ValueInt = Int32.MinValue;
        public string InitialValue = string.Empty;

        public string RBase = string.Empty;
        public string RBaseCalc = string.Empty;

        public ArrayList RoutinesCallsInfos = new ArrayList();
        public string ScaleExpression = "X";
        public int ScalePrecision = SADDef.DefaultScalePrecision;

        public string Units = string.Empty;

        private string label = string.Empty;
        private string shortLabel = string.Empty;

        private string comments = string.Empty;

        public S6xScalar S6xScalar = null;
        public S6xElementSignature S6xElementSignatureSource = null;

        public ArrayList OtherRelatedOpsUniqueAddresses = new ArrayList();

        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }
        public int AddressEndInt { get { return AddressInt + Size - 1; } }
        public string AddressEnd { get { return string.Format("{0:x4}", AddressEndInt + SADDef.EecBankStartAddress); } }
        public int Size { get { if (Byte) return 1; else return 2; } }

        public bool isIdentified { get { return (Byte || Word) && (UnSigned || Signed); } }
        public bool HasValue { get { return ValueInt != Int32.MinValue; } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public bool isScaled { get { return (getScaleExpression.ToLower().Trim() != "x"); } }

        public string getScaleExpression
        {
            get
            {
                if (S6xScalar == null) return ScaleExpression;
                if (S6xScalar.ScaleExpression == null || S6xScalar.ScaleExpression == string.Empty) return ScaleExpression;
                return S6xScalar.ScaleExpression;
            }
        }

        public int getScalePrecision
        {
            get
            {
                if (S6xScalar == null) return ScalePrecision;
                return S6xScalar.ScalePrecision;
            }
        }

        public bool isBitFlags
        {
            get
            {
                if (S6xScalar == null) return foundBitFlags;

                int activeBitFlags = 0;

                if (S6xScalar.isBitFlags)
                {
                    foreach (S6xBitFlag bitFlag in S6xScalar.BitFlags)
                    {
                        if (bitFlag == null) continue;
                        if (!bitFlag.Skip) activeBitFlags++;
                    }
                }

                return activeBitFlags > 0;
            }

            set
            {
                foundBitFlags = true;
            }
        }
        
        public string Label
        {
            get
            {
                if (S6xScalar == null && label == string.Empty) return string.Empty;
                else if (S6xScalar == null) return label;
                else if (label == string.Empty && (S6xScalar.Label == null || S6xScalar.Label == string.Empty)) return string.Empty;
                else return S6xScalar.Label;
            }
            set { label = value; }
        }

        public string ShortLabel
        {
            get
            {
                if (S6xScalar == null && shortLabel == string.Empty) return Address;
                else if (S6xScalar == null) return shortLabel;
                else if (shortLabel == string.Empty && (S6xScalar.ShortLabel == null || S6xScalar.ShortLabel == string.Empty)) return Address;
                else return S6xScalar.ShortLabel;
            }
            set { shortLabel = value; }
        }

        public string FullLabel
        {
            get
            {
                if (Label != string.Empty && ShortLabel != string.Empty && Label != ShortLabel) return ShortLabel + " - " + Label;
                else return Label;
            }
        }

        public string Comments
        {
            get
            {
                if (S6xScalar == null) return comments;
                else if (S6xScalar.Comments == null) return string.Empty;
                else if (!S6xScalar.OutputComments) return string.Empty;
                else return S6xScalar.Comments;
            }
            set
            {
                comments = value;
            }
        }

        public bool OutputComments
        {
            get
            {
                if (S6xScalar == null) return false;
                else return S6xScalar.OutputComments;
            }
        }

        public bool InlineComments
        {
            get
            {
                if (S6xScalar == null) return false;
                else return S6xScalar.InlineComments;
            }
        }

        public Scalar()
        {

        }

        public Scalar(int bankNum, int addressInt, bool isByte, bool isSigned)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            Byte = isByte;
            Word = !isByte;
            Signed = isSigned;
            UnSigned = !isSigned;
        }

        public Scalar(S6xScalar s6xScalar)
        {
            BankNum = s6xScalar.BankNum;
            AddressInt = s6xScalar.AddressInt;
            Byte = s6xScalar.Byte;
            Word = !s6xScalar.Byte;
            Signed = s6xScalar.Signed;
            UnSigned = !s6xScalar.Signed;

            S6xScalar = s6xScalar;
        }

        public string Value(int iBase)
        {
            string sValue = string.Empty;

            sValue = Convert.ToString(ValueInt, iBase);
            if (iBase == 16 && Byte && sValue.Length > 2) return sValue.Substring(sValue.Length - 2, 2);
            else if (iBase == 16 && Word && sValue.Length > 4) return sValue.Substring(sValue.Length - 4, 4);
            else return sValue;
        }

        public string ValueScaled()
        {
            if (isScaled) return ValueScaled(getScaleExpression, getScalePrecision);
            else return ValueScaled("X", 0);
        }

        public string ValueScaled(string scaleExpression, int scalePrecision)
        {
            return Tools.ScaleValue(ValueInt, scaleExpression, scalePrecision, false);
        }

        public string[][] ValueBitFlags
        {
            get
            {
                string[][] arrRes = null;
                ArrayList alShortLabels = null;
                ArrayList alValues = null;
                BitArray arrBit = null;
                bool defaultBitFlags = false;

                if (!isBitFlags) return arrRes;

                alShortLabels = new ArrayList();
                alValues = new ArrayList();
                arrBit = new BitArray(new int[] { ValueInt });

                if (S6xScalar == null) defaultBitFlags = true;
                else if (S6xScalar.BitFlags == null) defaultBitFlags = true;
                else if (S6xScalar.BitFlags.Length == 0) defaultBitFlags = true;

                if (defaultBitFlags)
                {
                    int iBfTop = 15;
                    if (Byte) iBfTop = 7;

                    for (int iBf = iBfTop; iBf >= 0; iBf--)
                    {
                        alShortLabels.Add("B" + iBf.ToString());
                        if (arrBit[iBf]) alValues.Add("1");
                        else alValues.Add("0");
                    }
                }
                else
                {
                    for (int iBf = S6xScalar.BitFlags.Length - 1; iBf >= 0; iBf--)
                    {
                        if (S6xScalar.BitFlags[iBf].Skip) continue;

                        alShortLabels.Add(S6xScalar.BitFlags[iBf].ShortLabel);
                        if (arrBit[S6xScalar.BitFlags[iBf].Position]) alValues.Add(S6xScalar.BitFlags[iBf].SetValue);
                        else alValues.Add(S6xScalar.BitFlags[iBf].NotSetValue);
                    }
                }
                arrBit = null;

                arrRes = new string[2][] { (string[])alShortLabels.ToArray(typeof(string)), (string[])alValues.ToArray(typeof(string)) };
                alShortLabels = null;
                alValues = null;
                return arrRes;
            }
        }

        public void Read(string[] arrBytes)
        {
            if (S6xScalar != null)
            {
                Byte = S6xScalar.Byte;
                Word = !S6xScalar.Byte;
                Signed = S6xScalar.Signed;
                UnSigned = !S6xScalar.Signed;
            }

            if (Word)
            {
                try { ValueInt = Tools.getWordInt(arrBytes, Signed, true); }
                catch { ValueInt = 0; }
                InitialValue = arrBytes[1] + SADDef.GlobalSeparator + arrBytes[0];
            }
            else
            {
                try { ValueInt = Tools.getByteInt(arrBytes[0], Signed); }
                catch { ValueInt = 0; }
                InitialValue = arrBytes[0];
            }
        }
    }

    // Structures

    public enum StructureItemType
    {
        Byte,       // BYTE
        Word,       // WORD
        SignedByte, // SBYTE
        SignedWord, // SWORD
        ByteHex,    // BYTE
        WordHex,    // WORD
        Hex,        // HEX
        HexLsb,     // HEX LSB
        Ascii,      // ASCII
        Skip,       // SKIP
        String,     // STRING
        Vector8,    // VECT8
        Vector1,    // VECT1
        Vector9,    // VECT9
        Vector0,    // VECT0
        Num,        // NUM
        NumHex,     // NUMHEX
        Empty       // EMPTY
    }

    // Structure Item
    public class StructureItem
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressBinInt = -1;

        public StructureItemType Type = StructureItemType.Hex;

        public string[] arrBytes = null;
        public string FixedValue = string.Empty;
        public string ScaleExpression = string.Empty;
        public int ScalePrecision = SADDef.DefaultScalePrecision;

        public string InitialValue { get { if (arrBytes == null) return string.Empty; else return string.Join(SADDef.GlobalSeparator.ToString(), arrBytes); } }

        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }
        public int AddressEndInt { get { return AddressInt + Size - 1; } }
        public string AddressEnd { get { return string.Format("{0:x4}", AddressEndInt + SADDef.EecBankStartAddress); } }
        public int Size { get { if (arrBytes == null) return 0; else return arrBytes.Length; } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public StructureItem(int bankNum, int addressInt, int addressBinInt, StructureItemType type)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            AddressBinInt = addressBinInt;
            Type = type;
        }

        public string Value(int structLineNum)
        {
            int iValue = 0;

            switch (Type)
            {
                case StructureItemType.Skip:
                case StructureItemType.String:
                case StructureItemType.Empty:
                case StructureItemType.Num:
                case StructureItemType.NumHex:
                    break;
                default:
                    if (arrBytes == null) return string.Empty;
                    break;
            }

            try
            {
                switch (Type)
                {
                    case StructureItemType.Byte:
                        if (Size == 1)
                        {
                            iValue = Tools.getByteInt(arrBytes[0], false);

                            if (ScaleExpression == string.Empty) return iValue.ToString();
                            else return string.Format("{0:F2}", Tools.ScaleValue(iValue, ScaleExpression, SADDef.DefaultScalePrecision, false));
                        }
                        break;
                    case StructureItemType.Word:
                        if (Size == 2)
                        {
                            iValue = Tools.getWordInt(arrBytes, false, true);

                            if (ScaleExpression == string.Empty) return iValue.ToString();
                            else return string.Format("{0:F2}", Tools.ScaleValue(iValue, ScaleExpression, SADDef.DefaultScalePrecision, false));
                        }
                        break;
                    case StructureItemType.SignedByte:
                        if (Size == 1)
                        {
                            iValue = Tools.getByteInt(arrBytes[0], true);

                            if (ScaleExpression == string.Empty) return iValue.ToString();
                            else return string.Format("{0:F2}", Tools.ScaleValue(iValue, ScaleExpression, SADDef.DefaultScalePrecision, false));
                        }
                        break;
                    case StructureItemType.SignedWord:
                        if (Size == 2)
                        {
                            iValue = Tools.getWordInt(arrBytes, true, true);

                            if (ScaleExpression == string.Empty) return iValue.ToString();
                            else return string.Format("{0:F2}", Tools.ScaleValue(iValue, ScaleExpression, SADDef.DefaultScalePrecision, false));
                        }
                        break;
                    case StructureItemType.ByteHex:
                        if (Size == 1) return Convert.ToString(Tools.getByteInt(arrBytes[0], false), 16);
                        break;
                    case StructureItemType.WordHex:
                        if (Size == 2) return Convert.ToString(Tools.getWordInt(arrBytes, false, true), 16);
                        break;
                    case StructureItemType.Hex:
                        return string.Join(string.Empty, arrBytes).ToUpper();
                    case StructureItemType.HexLsb:
                        string[] arrRes = new string[arrBytes.Length];
                        for (int iPos = 0; iPos < arrRes.Length; iPos++)
                        {
                            if (iPos % 2 != 0) arrRes[iPos - 1] = arrBytes[iPos];
                            else if (iPos % 2 == 0 && iPos + 1 < arrRes.Length) arrRes[iPos + 1] = arrBytes[iPos];
                            else arrRes[iPos] = arrBytes[iPos];
                        }
                        return string.Join(string.Empty, arrRes).ToUpper();
                    case StructureItemType.Ascii:
                        string sRes = string.Empty;
                        foreach (string sByte in arrBytes) sRes += Convert.ToChar(Convert.ToByte(sByte, 16));
                        return sRes;
                    case StructureItemType.Skip:
                        return string.Empty;
                    case StructureItemType.String:
                        return FixedValue;
                    case StructureItemType.Vector8:
                    case StructureItemType.Vector1:
                    case StructureItemType.Vector9:
                    case StructureItemType.Vector0:
                        if (Size == 2) return Convert.ToString(Tools.getWordInt(arrBytes, false, true), 16);
                        break;
                    case StructureItemType.Num:
                        iValue = structLineNum;
                        if (ScaleExpression == string.Empty) return iValue.ToString();
                        else return string.Format("{0}", Tools.ScaleValue(iValue, ScaleExpression, 0, false));
                    case StructureItemType.NumHex:
                        iValue = structLineNum;
                        if (ScaleExpression != string.Empty) iValue = (int)Tools.ScaleValue(iValue, ScaleExpression, false);
                        return string.Format("{0:x}", iValue);
                    case StructureItemType.Empty:
                        return string.Empty;
                }
            }
            catch
            {
                return string.Join(string.Empty, arrBytes);
            }

            return string.Join(string.Empty, arrBytes);
        }
    }

    // Structure Line
    public class StructureLine
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressBinInt = -1;
        public int NumberInStructure = -1;
        public ArrayList Items = null;

        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }
        public int AddressEndInt { get { return AddressInt + Size - 1; } }
        public string AddressEnd { get { return string.Format("{0:x4}", AddressEndInt + SADDef.EecBankStartAddress); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public int Size
        {
            get
            {
                int iSize = 0;
                foreach (StructureItem item in Items) iSize += item.Size;
                return iSize;
            }
        }

        public string InitialValue
        {
            get
            {
                string sResult = string.Empty;
                for (int iCol = 0; iCol < Items.Count; iCol++)
                {
                    if (iCol > 0)
                    {
                        switch (((StructureItem)Items[iCol]).Type)
                        {
                            case StructureItemType.Empty:
                            case StructureItemType.String:
                            case StructureItemType.Num:
                            case StructureItemType.NumHex:
                                break;
                            default:
                                if (sResult != string.Empty) sResult += SADDef.GlobalSeparator;
                                break;
                        }
                    }
                    sResult += ((StructureItem)Items[iCol]).InitialValue;
                }
                return sResult;
            }
        }

        public StructureLine()
        {
            Items = new ArrayList();
        }

        public StructureLine(int bankNum, int addressInt, int addressBinInt, int numberInStructure)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            AddressBinInt = addressBinInt;
            NumberInStructure = numberInStructure;
            Items = new ArrayList();
        }
    }

    // RoutineCallInfoStructure
    public class RoutineCallInfoStructure
    {
        public string CalibrationElementOpeUniqueAddress = string.Empty;
        public string RoutineUniqueAddress = string.Empty;
        public string RoutineCallOpeUniqueAddress = string.Empty;

        public RoutineIOStructure RoutineInputOutput = null;
    }

    public class StructureItemAnalysis
    {
        public int Position = -1;

        public Operation ReadOperation = null;
        public string ReadRegister = string.Empty;
        public ArrayList ReadCondOperations = null;
        public SortedList ReadCondCmpOperations = null;

        public string PointerRegister = string.Empty;

        public bool isByte = true;
        public bool isSigned = false;

        public bool isWordLowByte = false;
        public bool isTopByte = false;

        public bool isRBase = false;
        public bool isRBaseAdder = false;
        public string calcRBaseRegister = string.Empty;
        public bool givesTable = false;
        public bool givesFunction = false;
        public bool givesStructure = false;

        public bool forReading = false;
        public bool forStoring = false;
        public bool forVectorPush = false;

        public int VectorBank = -1;

        public bool isBaseIdentified = false;
        public bool isUseIdentified = false;
        
        /*
        public bool isRegister = false;
        public bool isRegisterRead = false;
        public bool isRegisterStore = false;
        public bool isRegisterIncrement = false;
        */

        public StructureItemAnalysis(int position)
        {
            Position = position;
            ReadCondOperations = new ArrayList();
            ReadCondCmpOperations = new SortedList();
        }
    }
    
    public class StructureAnalysis
    {
        public int StructBankNum = -1;
        public int StructAddressInt = -1;

        public int StructNewAddressInt = -1;

        public int AnalysisBankNum = -1;
        public int AnalysisSourceOpeAddressInt = -1;
        public int AnalysisStartOpeAddressInt = -1;
        public int AnalysisInitOpeAddressInt = -1;

        public bool isAdderStructure = false;
        
        public int Number = -1;
        public int LineSize = -1;
        
        public string ExitAddress = string.Empty;

        public string MainRegister = string.Empty;
        public string MainRegisterTB = string.Empty;
        
        public string LoopRegister = string.Empty;

        public bool LoopReversed = false;
        public int LoopInitValue = 0;
        public int LoopOpeAddressInt = -1;
        public int LoopExitOpeAddressInt = -1;
        public int LoopExitFirstItemOpeAddressInt = -1;
        public int LoopExitFirstItemValue = int.MinValue;
        
        public SortedList slItems = null;
        
        private string[] arrDefString = null;
        private string[] arrDefConds = null;
        private bool arrDefStringReproc = true;
        private bool arrDefCondsReproc = true;

        public string StructAddress { get { return string.Format("{0:x4}", StructAddressInt + SADDef.EecBankStartAddress); } }
        public string AnalysisStartOpeAddress { get { return string.Format("{0:x4}", AnalysisStartOpeAddressInt + SADDef.EecBankStartAddress); } }
        public string AnalysisInitOpeAddress { get { return string.Format("{0:x4}", AnalysisInitOpeAddressInt + SADDef.EecBankStartAddress); } }

        public string StructUniqueAddress { get { return string.Format("{0,1} {1,5}", StructBankNum, StructAddressInt); } }
        public string StructUniqueAddressHex { get { return string.Format("{0,1} {1,4}", StructBankNum, StructAddress); } }

        public bool hasItems { get { return (slItems == null) ? false : slItems.Count != 0; } }
        public bool hasConditions
        {
            get
            {
                if (DefinitionConds == null) return false;
                foreach (string defCond in DefinitionConds) if (defCond != null && defCond != string.Empty) return true;

                return false;
            }
        }

        public bool isValid 
        {
            get
            {
                // Basic Validity
                if (Number != -1 && (LineSize != -1 || (hasItems && ! hasConditions))) return true;
                // Number Calculation Required Validity
                if (ExitAddress != string.Empty && (LineSize != -1 || (hasItems && ! hasConditions))) return true;
                // Structure Read Required Validity
                if (LoopExitFirstItemValue != int.MinValue) return true;

                return false;
            }
        }

        public bool isNumberCalculationRequired
        {
            get
            {
                // Basic Validity
                if (Number != -1 && (LineSize != -1 || (hasItems && !hasConditions))) return false;
                // Number Calculation Required Validity
                if (ExitAddress != string.Empty && (LineSize != -1 || (hasItems && !hasConditions))) return true;
                // Structure Read Required Validity
                if (LoopExitFirstItemValue != int.MinValue) return false;

                return false;
            }
        }

        public bool isStructureReadRequired
        {
            get
            {
                // Basic Validity
                if (Number != -1 && (LineSize != -1 || (hasItems && !hasConditions))) return false;
                // Number Calculation Required Validity
                if (ExitAddress != string.Empty && (LineSize != -1 || (hasItems && !hasConditions))) return false;
                // Structure Read Required Validity
                if (LoopExitFirstItemValue != int.MinValue) return true;

                return false;
            }
        }

        public string ProposedStructureDefString
        {
            get
            {
                // Structure Definition String proposal
                if (DefinitionStrings == null || DefinitionConds == null) return string.Empty;
                
                string defString = string.Empty;

                if (LoopExitFirstItemValue != int.MinValue)
                {
                    string exitType = "ByteHex";
                    if (slItems.ContainsKey(0)) if (!((StructureItemAnalysis)slItems[0]).isByte) exitType = "WordHex";
                    defString = "IF (" + string.Format("{0:X2}", LoopExitFirstItemValue) + ":1) { " + exitType + ", \" EXIT\" } ELSE {";
                }

                if (DefinitionStrings.Length == 0)
                {
                    if (defString != string.Empty) defString += "\n";
                    defString += "Empty";
                }
                else
                {
                    for (int iVal = 0; iVal < DefinitionStrings.Length; iVal++)
                    {
                        if (DefinitionStrings[iVal] == null) continue;
                        if (defString != string.Empty) defString += "\n";
                        if (DefinitionConds[iVal] == string.Empty) defString += DefinitionStrings[iVal];
                        else defString += "IF (" + DefinitionConds[iVal] + ") { " + DefinitionStrings[iVal] + " } ELSE { Empty }";
                    }
                }

                if (LoopExitFirstItemValue != int.MinValue) defString += "\n}";

                return defString;
            }
        }

        public string[] DefinitionStrings
        {
            set
            {
                arrDefString = value;
                arrDefStringReproc = true;
                arrDefCondsReproc = true;
            }
            get
            {
                if (!arrDefStringReproc) return arrDefString;
                if (slItems.Count == 0)
                {
                    arrDefStringReproc = false;
                    return arrDefString;
                }

                // Structure Definition for Values
                // and Structure Definition deeper use by following affected registers essentially for Vect or RBase search
                string[] tmpArrDefString = null;
                if (arrDefString == null) arrDefString = new string[(int)slItems.GetKey(slItems.Count - 1) + 1];
                else if (arrDefString.Length < (int)slItems.GetKey(slItems.Count - 1) + 1)
                {
                    tmpArrDefString = new string[(int)slItems.GetKey(slItems.Count - 1) + 1];
                    for (int iDS = 0; iDS < arrDefString.Length; iDS++) tmpArrDefString[iDS] = arrDefString[iDS];
                    arrDefString = tmpArrDefString;
                    tmpArrDefString = null;
                }
                foreach (int itemPos in slItems.Keys)
                {
                    if (itemPos > arrDefString.Length - 1) continue;

                    StructureItemAnalysis siaSIA = (StructureItemAnalysis)slItems[itemPos];

                    // Type Upgrade is Forced
                    if (siaSIA.isTopByte)
                    {
                        arrDefString[siaSIA.Position] = null;
                    }
                    else if (siaSIA.forVectorPush)
                    {
                        arrDefString[siaSIA.Position] = "Vect" + siaSIA.VectorBank.ToString();
                    }
                    else if (siaSIA.isRBase)
                    {
                        if (siaSIA.isByte) arrDefString[siaSIA.Position] = "\"RBase R\",ByteHex";
                        else arrDefString[siaSIA.Position] = "\"RBase R\",WordHex";
                    }
                    else if (siaSIA.isRBaseAdder)
                    {
                        arrDefString[siaSIA.Position] = "\"RBase+\",WordHex";
                    }
                    else if (siaSIA.forStoring && siaSIA.isByte && !siaSIA.isWordLowByte)
                    {
                        arrDefString[siaSIA.Position] = "\"Reg \",ByteHex";
                    }
                    else if (siaSIA.forStoring)
                    {
                        arrDefString[siaSIA.Position] = "\"Reg \",WordHex";
                    }
                    else if (arrDefString[siaSIA.Position] == null || arrDefString[siaSIA.Position] == "Hex:2" || arrDefString[siaSIA.Position] == "Hex")
                    // Basic values are not forced or only when already forced as Hex
                    {
                        if (siaSIA.isSigned) arrDefString[siaSIA.Position] = "S";
                        else arrDefString[siaSIA.Position] = string.Empty;
                        if (siaSIA.isByte && !siaSIA.isWordLowByte) arrDefString[siaSIA.Position] += "Byte";
                        else arrDefString[siaSIA.Position] += "Word";
                    }
                }

                // LineSize is defined and overrides detected elements when different
                if (LineSize != -1 && LineSize != arrDefString.Length)
                {
                    tmpArrDefString = new string[LineSize];
                    if (LineSize > arrDefString.Length)
                    {
                        for (int iDS = 0; iDS < arrDefString.Length; iDS++) tmpArrDefString[iDS] = arrDefString[iDS];
                        // Additional Items are defaulted as Hex
                        int iDSStart = arrDefString.Length;
                        // When a Word is the last entry in array, default filling should be moved
                        StructureItemAnalysis siaTmp = (StructureItemAnalysis)slItems.GetByIndex(slItems.Count - 1);
                        if (arrDefString.Length == siaTmp.Position + 1 && (!siaTmp.isByte || siaTmp.isWordLowByte)) iDSStart++;
                        siaTmp = null;
                        for (int iDS = iDSStart; iDS < LineSize; iDS += 2)
                        {
                            if (LineSize - iDS == 1) tmpArrDefString[iDS] = "Hex";
                            else tmpArrDefString[iDS] = "Hex:2";
                        }
                    }
                    else
                    {
                        for (int iDS = 0; iDS < LineSize; iDS++) tmpArrDefString[iDS] = arrDefString[iDS];
                    }
                    arrDefString = tmpArrDefString;
                    tmpArrDefString = null;
                }

                // Low Byte / Top Byte / Empty Values Coherence
                int filledBytes = 0;
                for (int iDS = 0; iDS < arrDefString.Length; iDS ++)
                {
                    if (iDS % 2 == 0 && filledBytes < iDS)
                    {
                        if (iDS - filledBytes == 2) arrDefString[iDS - 2] = "Hex:2";
                        else
                        {
                            if (arrDefString[iDS - 1] == null || arrDefString[iDS - 1] == string.Empty) arrDefString[iDS - 1] = "Hex";
                            else arrDefString[iDS - 2] = "Hex";
                        }
                        filledBytes = iDS;
                    }
                    switch (arrDefString[iDS])
                    {
                        case null:
                        case "":
                            break;
                        case "\"RBase R\",ByteHex":
                        case "\"Reg \",ByteHex":
                        case "Hex":
                        case "Byte":
                        case "SByte":
                            filledBytes++;
                            break;
                        default:
                            filledBytes += 2;
                            if (iDS < arrDefString.Length - 1) arrDefString[iDS + 1] = null;
                            break;
                    }
                }
                if (filledBytes < arrDefString.Length)
                {
                    if (arrDefString.Length - filledBytes == 2) arrDefString[arrDefString.Length - 1] = "Hex:2";
                    else arrDefString[arrDefString.Length - 1] = "Hex";
                }

                arrDefStringReproc = false;
                return arrDefString;
            }
        }
        public string[] DefinitionConds
        {
            set
            {
                arrDefConds = value;
                arrDefCondsReproc = true;
            }
            get
            {
                if (!arrDefCondsReproc) return arrDefConds;
                if (DefinitionStrings == null)
                {
                    arrDefCondsReproc = false;
                    return arrDefConds;
                }

                // Structure Definition for Conditions
                if (arrDefConds == null) arrDefConds = new string[DefinitionStrings.Length];
                else if (arrDefConds.Length < DefinitionStrings.Length)
                {
                    string[] tmpArrDefConds = new string[DefinitionStrings.Length];
                    for (int iDS = 0; iDS < arrDefConds.Length; iDS++) tmpArrDefConds[iDS] = arrDefConds[iDS];
                    arrDefConds = tmpArrDefConds;
                    tmpArrDefConds = null;
                }
                for (int iCond = 0; iCond < arrDefConds.Length; iCond++) if (arrDefConds[iCond] == null) arrDefConds[iCond] = string.Empty;
                foreach (StructureItemAnalysis siaSIA in slItems.Values)
                {
                    if (siaSIA.Position > arrDefConds.Length - 1) continue;

                    foreach (StructureItemAnalysis siaCond in slItems.Values)
                    {
                        if (siaCond.Position >= siaSIA.Position) break;
                        foreach (Operation cOpe in siaCond.ReadCondOperations)
                        {
                            string sCond = string.Empty;

                            if (cOpe.AddressInt == LoopExitFirstItemOpeAddressInt) continue;
                            if (cOpe.AddressInt < siaSIA.ReadOperation.AddressInt && cOpe.AddressJumpInt > siaSIA.ReadOperation.AddressInt)
                            {
                                switch (cOpe.OriginalOPCode.ToLower().Substring(0, 1))
                                {
                                    // JB/JNB operation
                                    case "3":
                                        // Inverted - Set to condition to read structure item
                                        if (cOpe.OriginalInstruction.ToLower() == "jnb")
                                        {
                                            sCond = "B" + (Convert.ToInt32(cOpe.OriginalOPCode.ToLower(), 16) - 0x30).ToString() + ":" + (siaCond.Position + 1).ToString() + "|";
                                        }
                                        else
                                        {
                                            sCond = "!B" + (Convert.ToInt32(cOpe.OriginalOPCode.ToLower(), 16) - 0x38).ToString() + ":" + (siaCond.Position + 1).ToString() + "|";
                                        }
                                        break;
                                    // Other J Ops using GotoOpParams
                                    case "d":
                                        if (cOpe.GotoOpParams == null) continue;
                                        Operation cmpOpe = (Operation)siaCond.ReadCondCmpOperations[cOpe.GotoOpParams.OpeUniqueAddress];
                                        if (cmpOpe == null) continue;
                                        if (cmpOpe.OperationParams.Length == 0) continue;
                                        object[] arrPointersValues = Tools.InstructionPointersValues(cmpOpe.OperationParams[cmpOpe.OperationParams.Length - 1].InstructedParam);
                                        if ((bool)arrPointersValues[0]) continue;
                                        // Inverted - Set to condition to read structure item
                                        switch (cOpe.OriginalInstruction.ToLower())
                                        {
                                            case "je":
                                                sCond = "!" + string.Format("{0:x2}", arrPointersValues[2]) + ":" + (siaCond.Position + 1).ToString() + "|";
                                                break;
                                            case "jne":
                                                sCond = "!" + string.Format("{0:x2}", arrPointersValues[2]) + ":" + (siaCond.Position + 1).ToString() + "|";
                                                break;
                                            default:
                                                // To Be Managed
                                                break;
                                        }
                                        arrPointersValues = null;
                                        break;
                                }
                            }
                            if (sCond != string.Empty && !arrDefConds[siaSIA.Position].Contains(sCond.Replace("|", string.Empty)))
                            {
                                if (arrDefConds[siaSIA.Position] == string.Empty || arrDefConds[siaSIA.Position].EndsWith("|")) arrDefConds[siaSIA.Position] += sCond;
                                else arrDefConds[siaSIA.Position] += "|" + sCond;
                            }
                        }
                    }
                    if (arrDefConds[siaSIA.Position].EndsWith("|")) arrDefConds[siaSIA.Position] = arrDefConds[siaSIA.Position].Substring(0, arrDefConds[siaSIA.Position].Length - 1);
                }

                arrDefCondsReproc = false;
                return arrDefConds;
            }
        }

        public StructureAnalysis()
        {
            slItems = new SortedList();
        }

        public void NumberCalculation()
        {
            if (LineSize == -1 || ExitAddress == string.Empty) return;

            int addressInt = StructAddressInt;
            if (StructNewAddressInt != -1) addressInt = StructNewAddressInt;

            Number = (Convert.ToInt32(ExitAddress, 16) - SADDef.EecBankStartAddress - addressInt) / LineSize;
            if ((Convert.ToInt32(ExitAddress, 16) - SADDef.EecBankStartAddress - addressInt) % LineSize != 0) Number--;

            if (StructNewAddressInt != -1 && StructAddressInt > StructNewAddressInt) Number -= (StructAddressInt - StructNewAddressInt) / LineSize;

            if (Number < -1) Number = -1;
        }
    }
    
    // Structure
    public class Structure
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int AddressBinInt = -1;
        public ArrayList Lines = null;

        // Included Structure / Duplicated Structure
        public int ParentAddressInt = -1;
        public Structure ParentStructure = null;
        // Included Structure / Duplicated Structure

        private string structDefString = string.Empty;
        private object[] structDef = null;
        private int maxSizeSingle = -1;

        public int Number = 0;

        public string RBase = string.Empty;
        public string RBaseCalc = string.Empty;

        public ArrayList RoutinesCallsInfos = new ArrayList();

        private string label = string.Empty;
        private string shortLabel = string.Empty;

        private string comments = string.Empty;

        private bool bIsVectorsList = false;
        private int iVectorsBankNum = -1;
        public SortedList Vectors = new SortedList();

        private bool bContainsOtherVectorAddresses = false;
        private int[][] includedOtherVectorAddresses = null;

        private bool bIsVectorsStructure = false;

        public bool Defaulted = true;

        public S6xStructure S6xStructure = null;
        public S6xElementSignature S6xElementSignatureSource = null;

        public ArrayList OtherRelatedOpsUniqueAddresses = new ArrayList();

        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }
        public int AddressEndInt { get { return AddressInt + ((Size == 0) ? 0 : Size - 1); } }
        public string AddressEnd { get { return string.Format("{0:x4}", AddressEndInt + SADDef.EecBankStartAddress); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public string ParentAddress { get { return string.Format("{0:x4}", ParentAddressInt + SADDef.EecBankStartAddress); } }
        public int ParentAddressEndInt { get { return ParentAddressInt + ((Size == 0) ? 0 : Size - 1); } }
        public string ParentAddressEnd { get { return string.Format("{0:x4}", ParentAddressEndInt + SADDef.EecBankStartAddress); } }

        public string ParentUniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, ParentAddressInt); } }
        public string ParentUniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, ParentAddress); } }

        public int Size
        {
            get
            {
                if (Lines.Count == 0) return 0;

                int iSize = 0;
                foreach (StructureLine line in Lines) iSize += line.Size;
                return iSize;
            }
        }

        public bool HasValues { get { return (Lines == null) ? false : Lines.Count > 0; } }

        public string InitialValue
        {
            get
            {
                string sResult = string.Empty;
                for (int iLine = 0; iLine < Lines.Count; iLine++)
                {
                    if (iLine > 0) sResult += SADDef.GlobalSeparator;
                    sResult += ((StructureLine)Lines[iLine]).InitialValue;
                }
                return sResult;
            }
        }

        public string Label
        {
            get
            {
                if (ParentStructure != null) return ParentStructure.Label + " " + Address;
                else if (S6xStructure == null && label == string.Empty) return string.Empty;
                else if (S6xStructure == null) return label;
                else if (label == string.Empty && (S6xStructure.Label == null || S6xStructure.Label == string.Empty)) return string.Empty;
                else return S6xStructure.Label;
            }
            set { label = value; }
        }

        public string ShortLabel
        {
            get
            {
                if (ParentStructure != null) return ParentStructure.ShortLabel + "_" + Address;
                else if (S6xStructure == null && shortLabel == string.Empty) return Address;
                else if (S6xStructure == null) return shortLabel;
                else if (shortLabel == string.Empty && (S6xStructure.ShortLabel == null || S6xStructure.ShortLabel == string.Empty)) return Address;
                else return S6xStructure.ShortLabel;
            }
            set { shortLabel = value; }
        }

        public string FullLabel
        {
            get
            {
                if (ParentStructure != null) return ShortLabel + " - " + ParentStructure.Label;
                if (Label != string.Empty && ShortLabel != string.Empty && Label != ShortLabel) return ShortLabel + " - " + Label;
                else return Label;
            }
        }

        public string Comments
        {
            get
            {
                if (S6xStructure == null) return comments;
                else if (S6xStructure.Comments == null) return string.Empty;
                else if (!S6xStructure.OutputComments) return string.Empty;
                else return S6xStructure.Comments;
            }
            set { comments = value; }
        }

        public bool OutputComments
        {
            get
            {
                if (S6xStructure == null) return false;
                else return S6xStructure.OutputComments;
            }
        }

        public int MaxLineItemsNum
        {
            get
            {
                int iMaxNum = 0;
                foreach (StructureLine line in Lines) if (line.Items.Count > iMaxNum) iMaxNum = line.Items.Count;
                return iMaxNum;
            }
        }

        public int MaxSizeSingle { get {return maxSizeSingle;} }

        public bool isValid { get { return maxSizeSingle >= 0; } }
        public bool isEmpty { get { return maxSizeSingle == 0; } }

        public bool isVectorsList { get { return bIsVectorsList; } }
        public bool isVectorsStructure { get { return bIsVectorsStructure; } }
        public int VectorsBankNum { get { return iVectorsBankNum; } }
        public bool containsOtherVectorsAddresses { get { return bContainsOtherVectorAddresses; } }
        
        public bool containsConditions
        {
            get
            {
                if (!isValid) return false;
                if (isEmpty) return false;

                foreach (object[] structDefElem in structDef)
                {
                    switch (structDefElem[0].ToString())
                    {
                        case "COND":
                            return true;
                    }
                }

                return false;
            }
        }

        public bool isTableCompatible
        {
            get
            {
                string uniqueType = string.Empty;
                string uniqueTypePrev = string.Empty;

                if (!isValid) return false;
                if (isEmpty) return false;

                foreach (object[] structDefElem in structDef)
                {
                    switch (structDefElem[0].ToString())
                    {
                        case "COND":
                        case "CR":
                            return false;
                        case "VAL":
                            switch ((StructureItemType)structDefElem[2])
                            {
                                case StructureItemType.Byte:
                                case StructureItemType.SignedByte:
                                case StructureItemType.ByteHex:
                                    uniqueType = "byte";
                                    break;
                                case StructureItemType.Word:
                                case StructureItemType.SignedWord:
                                case StructureItemType.WordHex:
                                case StructureItemType.Vector8:
                                case StructureItemType.Vector1:
                                case StructureItemType.Vector9:
                                case StructureItemType.Vector0:
                                    uniqueType = "word";
                                    break;
                                default:
                                    return false;
                            }
                            if (uniqueTypePrev != string.Empty && uniqueType != uniqueTypePrev) return false;
                            uniqueTypePrev = uniqueType;
                            break;
                    }
                }

                return true;
            }
        }

        public bool isFunctionCompatible
        {
            get
            {
                int iColsNum = 0;

                if (!isValid) return false;
                if (isEmpty) return false;

                foreach (object[] structDefElem in structDef)
                {
                    switch (structDefElem[0].ToString())
                    {
                        case "COND":
                        case "CR":
                            return false;
                        case "VAL":
                            switch ((StructureItemType)structDefElem[2])
                            {
                                case StructureItemType.Byte:
                                case StructureItemType.SignedByte:
                                case StructureItemType.ByteHex:
                                case StructureItemType.Word:
                                case StructureItemType.SignedWord:
                                case StructureItemType.WordHex:
                                case StructureItemType.Vector8:
                                case StructureItemType.Vector1:
                                case StructureItemType.Vector9:
                                case StructureItemType.Vector0:
                                    break;
                                default:
                                    return false;
                            }
                            break;
                    }
                    iColsNum += (int)structDefElem[1];
                    if (iColsNum > 2) return false;
                }

                if (iColsNum == 2) return true;
                else return false;
            }
        }

        public bool isScalarCompatible
        {
            get
            {
                int iColsNum = 0;

                if (!isValid) return false;
                if (isEmpty) return false;

                foreach (object[] structDefElem in structDef)
                {
                    switch (structDefElem[0].ToString())
                    {
                        case "COND":
                        case "CR":
                            return false;
                        case "VAL":
                            switch ((StructureItemType)structDefElem[2])
                            {
                                case StructureItemType.Byte:
                                case StructureItemType.SignedByte:
                                case StructureItemType.ByteHex:
                                case StructureItemType.Word:
                                case StructureItemType.SignedWord:
                                case StructureItemType.WordHex:
                                case StructureItemType.Vector8:
                                case StructureItemType.Vector1:
                                case StructureItemType.Vector9:
                                case StructureItemType.Vector0:
                                    break;
                                default:
                                    return false;
                            }
                            break;
                    }
                    iColsNum += (int)structDefElem[1];
                    if (iColsNum > 1) return false;
                }

                if (iColsNum == 1) return true;
                else return false;
            }
        }

        public string StructDefString
        {
            get { return structDefString; }
            set
            {
                structDefString = value;
                parseStructDefString();
            }
        }
        
        public Structure()
        {
            Lines = new ArrayList();
        }
        
        public Structure(S6xStructure s6xStruct)
        {
            BankNum = s6xStruct.BankNum;
            AddressInt = s6xStruct.AddressInt;
            AddressBinInt = s6xStruct.AddressBinInt;
            S6xStructure = s6xStruct;
            StructDefString = S6xStructure.StructDef;
            //20210902 - PYM - Coming from definition
            Number = s6xStruct.Number;
            Defaulted = false;

            Lines = new ArrayList();
        }

        public Structure(int bankNum, int addressInt, int addressBinInt, object[] StructDef)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
            AddressBinInt = addressBinInt;
            structDef = StructDef;
            parseStructDef();
            Lines = new ArrayList();
        }

        public int[] GetOtherVectorAddresses(int bankNum)
        {
            if (includedOtherVectorAddresses == null) return new int[] { };

            ArrayList alAddr = new ArrayList();
            foreach (int[] bankAddr in includedOtherVectorAddresses)
            {
                if (bankAddr[0] != bankNum) continue;
                alAddr.Add(bankAddr[1]);
            }
            return (int[])alAddr.ToArray(typeof(int));
        }

        private void parseStructDef()
        {
            maxSizeSingle = -1;

            structDefString = string.Empty;

            if (structDef == null) return;

            structDefString = parseStructDef(structDef);

            calcMaxSizeSingle();
            checkVectorsList();
            checkVectorsStructure();
            checkOtherVectors();
        }

        private string parseStructDef(object[] arrDef)
        {
            string sResult = string.Empty;
            bool elseCond = false;

            if (arrDef == null) return sResult;
            
            foreach (object[] structDefElem in arrDef)
            {
                switch (structDefElem[0].ToString())
                {
                    case "COND":
                        if (sResult != string.Empty && !sResult.EndsWith("\r\n")) sResult += "\r\n";
                        if (elseCond)
                        {
                            sResult += "\r\nelse\r\n";
                            sResult += "{\r\n" + parseStructDef((object[])structDefElem[3]) + "\r\n}\r\n";
                            elseCond = false;
                        }
                        else
                        {
                            sResult += "if (" + structDefElem[2].ToString().ToUpper() + ":" + structDefElem[1].ToString() + ")\r\n";
                            sResult += "{\r\n" + parseStructDef((object[])structDefElem[3]) + "\r\n}";
                            elseCond = true;
                        }
                        break;
                    case "CR":
                        sResult += "\\n";
                        break;
                    case "VAL":
                        if (sResult != string.Empty && !sResult.EndsWith("\r\n")) sResult += ",";
                        switch ((StructureItemType)structDefElem[2])
                        {
                            case StructureItemType.Ascii:
                                sResult += "Ascii";
                                break;
                            case StructureItemType.Hex:
                                sResult += "Hex";
                                break;
                            case StructureItemType.HexLsb:
                                sResult += "HexLsb";
                                break;
                            case StructureItemType.Byte:
                                sResult += "Byte";
                                // Included formula
                                if (structDefElem[4].ToString().Length > 0) sResult += "(" + structDefElem[4].ToString() + ")";
                                break;
                            case StructureItemType.Word:
                                sResult += "Word";
                                // Included formula
                                if (structDefElem[4].ToString().Length > 0) sResult += "(" + structDefElem[4].ToString() + ")";
                                break;
                            case StructureItemType.SignedByte:
                                sResult += "SByte";
                                // Included formula
                                if (structDefElem[4].ToString().Length > 0) sResult += "(" + structDefElem[4].ToString() + ")";
                                break;
                            case StructureItemType.SignedWord:
                                sResult += "SWord";
                                // Included formula
                                if (structDefElem[4].ToString().Length > 0) sResult += "(" + structDefElem[4].ToString() + ")";
                                break;
                            case StructureItemType.ByteHex:
                                sResult += "ByteHex";
                                break;
                            case StructureItemType.WordHex:
                                sResult += "WordHex";
                                break;
                            case StructureItemType.Skip:
                                sResult += "Skip";
                                break;
                            case StructureItemType.String:
                                sResult += "\"" + structDefElem[4].ToString() + "\"";
                                break;
                            case StructureItemType.Vector8:
                                sResult += "Vect8";
                                break;
                            case StructureItemType.Vector1:
                                sResult += "Vect1";
                                break;
                            case StructureItemType.Vector9:
                                sResult += "Vect9";
                                break;
                            case StructureItemType.Vector0:
                                sResult += "Vect0";
                                break;
                            case StructureItemType.Num:
                                sResult += "Num";
                                break;
                            case StructureItemType.NumHex:
                                sResult += "NumHex";
                                break;
                            case StructureItemType.Empty:
                                sResult += "Empty";
                                break;
                        }
                        sResult += ":" + structDefElem[1].ToString();
                        break;
                }
            }

            return sResult;
        }

        private static int[] getStructDefCondPositions(string sDef)
        {
            int insideBracketsCount = 1;
            bool insideText = false;

            if (sDef.StartsWith("{")) insideBracketsCount--;

            char[] arrDef = sDef.ToCharArray();
            for (int iPos = 0; iPos < arrDef.Length; iPos++)
            {
                if (arrDef[iPos] == '{' && !insideText) insideBracketsCount++;
                else if (arrDef[iPos] == '}' && !insideText) insideBracketsCount--;
                else if (arrDef[iPos] == '"' && !insideText) insideText = true;
                else if (arrDef[iPos] == '"' && insideText) insideText = false;

                if (insideBracketsCount == 0) return new int[] { sDef.StartsWith("{") ? 1 : 0, iPos - 1 };
                else if (insideBracketsCount < 0) break;
            }
            arrDef = null;

            return new int[] { -1, -1 };
        }


        private void parseStructDefString()
        {
            maxSizeSingle = -1;
            structDef = parseStructDefString(structDefString);
            if (structDef == null) return;

            calcMaxSizeSingle();
            checkVectorsList();
            checkVectorsStructure();
            checkOtherVectors();
        }

        private object[] parseStructDefString(string sDef)
        {
            /*
            Byte:2,Word,Hex:4,Ascii:8
            Hex,Byte:8
            Word:2,Byte

            If(B0:2) {
            Byte:2,Word
            Hex,Byte:8 }
            Else
            { Byte:2,Word:2
            Word:2,Byte }
            */

            ArrayList alDef = null;

            alDef = new ArrayList();
            sDef = sDef.Replace("\r", "").Replace("\\n", "<CR>").Replace("\n", "|");

            // Replace Spaces except for String elements " a b c "
            if (!sDef.Contains("\"")) sDef = sDef.Replace(" ", "");
            else
            {
                string[] arrTmp = sDef.Split('"');
                for (int iPos = 0; iPos < arrTmp.Length; iPos++) if (iPos % 2 == 0) arrTmp[iPos] = arrTmp[iPos].Replace(" ", "");
                sDef = string.Join("\"", arrTmp);
                arrTmp = null;
            }

            while (sDef.Contains("||")) sDef = sDef.Replace("||", "|");
            sDef = sDef.Replace("|{", "{").Replace("{|", "{").Replace("|}", "}").Replace("}|", "}");
            sDef = sDef.Replace("|i", "i").Replace("|I", "I").Replace("|e", "e").Replace("|E", "E");
            while (sDef != string.Empty)
            {
                if (sDef.StartsWith("<CR>"))
                {
                    sDef = sDef.Substring("<CR>".Length);
                    alDef.Add(new object[] { "CR" });
                }
                else if (sDef.StartsWith("|"))
                {
                    sDef = sDef.Substring("|".Length);
                }
                else if (sDef.StartsWith(","))
                {
                    sDef = sDef.Substring(",".Length);
                }
                else if (sDef.ToLower().StartsWith("if"))
                {
                    object[] arrIfInst = new object[] { "COND", null, null, null };
                    string sCond = string.Empty;
                    try
                    {
                        sCond = sDef.Substring(0, sDef.IndexOf("{") + 1).ToLower();
                        sDef = sDef.Substring(sCond.Length);
                        sCond = sCond.Replace("if(", "").Replace("){", "");
                        arrIfInst[1] = Convert.ToInt32(sCond.Split(':')[1]);
                        arrIfInst[2] = sCond.Split(':')[0];
                        if (arrIfInst[1].ToString().Length <= 0 && arrIfInst[1].ToString().Length >= 3) return null;
                        if (arrIfInst[2].ToString().Contains("!") && !arrIfInst[2].ToString().StartsWith("!")) return null;
                        if (arrIfInst[2].ToString().Contains("!") && arrIfInst[2].ToString().Length != 3) return null;
                        if (arrIfInst[2].ToString().Replace("!", string.Empty).Length != 2) return null;
                        int tmpCond = Convert.ToInt32(arrIfInst[2].ToString().Replace("!", string.Empty), 16);
                    }
                    catch { return null; }
                    
                    int[] arrCondPositions = getStructDefCondPositions(sDef);
                    if (arrCondPositions[0] == -1 || arrCondPositions[1] == -1) return null;
                    arrIfInst[3] = parseStructDefString(sDef.Substring(arrCondPositions[0], arrCondPositions[1] - arrCondPositions[0] + 1));
                    if (arrIfInst[3] == null) return null;
                    alDef.Add(arrIfInst);

                    sDef = sDef.Substring(arrCondPositions[1] + 1);
                }
                else if (sDef.ToLower().StartsWith("}else{"))
                {
                    sDef = sDef.Substring("}else{".Length);
                    if (alDef.Count == 0) return null;
                    object[] arrIfInst = (object[])alDef[alDef.Count - 1];
                    if (arrIfInst[0].ToString() != "COND") return null;
                    object[] arrElseInst = new object[] { arrIfInst[0], arrIfInst[1], arrIfInst[2], null };
                    arrIfInst = null;
                    if (arrElseInst[2].ToString().StartsWith("!")) arrElseInst[2] = arrElseInst[2].ToString().Substring(1);
                    else arrElseInst[2] = "!" + arrElseInst[2].ToString();

                    int[] arrCondPositions = getStructDefCondPositions(sDef);
                    if (arrCondPositions[0] == -1 || arrCondPositions[1] == -1) return null;
                    arrElseInst[3] = parseStructDefString(sDef.Substring(arrCondPositions[0], arrCondPositions[1] - arrCondPositions[0] + 1));
                    if (arrElseInst[3] == null) return null;
                    alDef.Add(arrElseInst);

                    sDef = sDef.Substring(arrCondPositions[1] + 2);
                }
                else
                {
                    object[] arrInst = new object[] { "VAL", null, null, null, string.Empty };
                    string sVal = string.Empty;
                    try
                    {
                        int iLen = sDef.Length;
                        int iCr = sDef.IndexOf("|");
                        int iCo = sDef.IndexOf(",");
                        int iCe = sDef.IndexOf("}");
                        int iCi = sDef.ToLower().IndexOf("if(");
                        int iCf = sDef.IndexOf("<CR>");
                        if (iCr > 0 && iCr < iLen) iLen = iCr;
                        if (iCo > 0 && iCo < iLen) iLen = iCo;
                        if (iCe > 0 && iCe < iLen) iLen = iCe;
                        if (iCi > 0 && iCi < iLen) iLen = iCi;
                        if (iCf > 0 && iCf < iLen) iLen = iCf;
                        sVal = sDef.Substring(0, iLen);
                        sDef = sDef.Substring(iLen);
                        if (sVal.Contains(":"))
                        {
                            arrInst[1] = Convert.ToInt32(sVal.Split(':')[1]);
                            arrInst[2] = sVal.Split(':')[0];
                        }
                        else
                        {
                            arrInst[1] = 1;
                            arrInst[2] = sVal;
                        }
                        if (arrInst[1].ToString().Length <= 0 && arrInst[1].ToString().Length >= 3) return null;
                        arrInst[3] = arrInst[1];
                        // Included formula, only for byte, word and signed versions and in addition num and numhex
                        if (arrInst[2].ToString().Contains("(") && arrInst[2].ToString().EndsWith(")"))
                        {
                            arrInst[4] = arrInst[2].ToString().Split('(')[1];
                            arrInst[4] = arrInst[4].ToString().Substring(0, arrInst[4].ToString().Length - 1);
                            arrInst[2] = arrInst[2].ToString().Split('(')[0];
                        }
                        if (arrInst[2].ToString().StartsWith("\"") && arrInst[2].ToString().EndsWith("\""))
                        {
                            arrInst[4] = arrInst[2].ToString().Substring(1, arrInst[2].ToString().Length - 2);
                            arrInst[2] = StructureItemType.String;
                            arrInst[3] = 0;
                        }
                        else
                        {
                            switch (arrInst[2].ToString().ToLower())
                            {
                                case "byte":
                                    arrInst[2] = StructureItemType.Byte;
                                    break;
                                case "word":
                                    arrInst[2] = StructureItemType.Word;
                                    arrInst[3] = (int)arrInst[1] * 2;
                                    break;
                                case "sbyte":
                                    arrInst[2] = StructureItemType.SignedByte;
                                    break;
                                case "sword":
                                    arrInst[2] = StructureItemType.SignedWord;
                                    arrInst[3] = (int)arrInst[1] * 2;
                                    break;
                                case "bytehex":
                                    arrInst[2] = StructureItemType.ByteHex;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                case "wordhex":
                                    arrInst[2] = StructureItemType.WordHex;
                                    arrInst[3] = (int)arrInst[1] * 2;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                case "hex":
                                    arrInst[2] = StructureItemType.Hex;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                case "hexlsb":
                                    arrInst[2] = StructureItemType.HexLsb;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                case "ascii":
                                    arrInst[2] = StructureItemType.Ascii;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                case "skip":
                                    arrInst[2] = StructureItemType.Skip;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                case "vect8":
                                    arrInst[2] = StructureItemType.Vector8;
                                    arrInst[3] = (int)arrInst[1] * 2;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                case "vect1":
                                    arrInst[2] = StructureItemType.Vector1;
                                    arrInst[3] = (int)arrInst[1] * 2;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                case "vect9":
                                    arrInst[2] = StructureItemType.Vector9;
                                    arrInst[3] = (int)arrInst[1] * 2;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                case "vect0":
                                    arrInst[2] = StructureItemType.Vector0;
                                    arrInst[3] = (int)arrInst[1] * 2;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                case "num":
                                    arrInst[2] = StructureItemType.Num;
                                    arrInst[3] = 0;
                                    break;
                                case "numhex":
                                    arrInst[2] = StructureItemType.NumHex;
                                    arrInst[3] = 0;
                                    break;
                                case "empty":
                                    arrInst[2] = StructureItemType.Empty;
                                    arrInst[3] = 0;
                                    if (arrInst[4].ToString() != string.Empty) return null;
                                    break;
                                default:
                                    return null;
                            }
                        }
                    }
                    catch { return null; }
                    alDef.Add(arrInst);
                }
            }

            // Security to prevent using If without Else
            //      Tested at current level to propagate it everywhere
            for (int iElem = 0; iElem < alDef.Count; iElem++)
            {
                object[] arrInst = (object[])alDef[iElem];
                if (arrInst[0].ToString() == "COND")
                {
                    if (iElem == alDef.Count - 1) return null;
                    if (((object[])alDef[iElem + 1])[0].ToString() != "COND") return null;
                    iElem++;
                }
            }
            
            object[] arrRes = new object[alDef.Count];
            arrRes = alDef.ToArray();
            return arrRes;
        }

        private void calcMaxSizeSingle()
        {
            if (structDef == null)
            {
                maxSizeSingle = -1;
                return;
            }

            maxSizeSingle = calcMaxSizeSingle(structDef);
        }

        private int calcMaxSizeSingle(object[] arrDef)
        {
            int iMaxSize = 0;
            int iMaxSizeIf = -1;
            int iMaxSizeElse = -1;

            if (arrDef == null) return -1;

            foreach (object[] structDefElem in arrDef)
            {
                switch (structDefElem[0].ToString())
                {
                    case "COND":
                        if (iMaxSizeIf >= 0)
                        {
                            iMaxSizeElse = calcMaxSizeSingle((object[])structDefElem[3]);
                            if (iMaxSizeIf >= iMaxSizeElse) iMaxSize += iMaxSizeIf;
                            else iMaxSize += iMaxSizeElse;
                            iMaxSizeIf = -1;
                            iMaxSizeElse = -1;
                        }
                        else
                        {
                            iMaxSizeIf = calcMaxSizeSingle((object[])structDefElem[3]);
                        }
                        break;
                    case "VAL":
                        iMaxSize += (int)structDefElem[3];
                        break;
                }
            }

            return iMaxSize;
        }

        private void checkVectorsList()
        {
            bIsVectorsList = false;
            iVectorsBankNum = -1;

            if (!isValid) return;
            if (isEmpty) return;
            if (structDef.Length != 1) return;

            switch (((object[])structDef[0])[0].ToString())
            {
                case "VAL":
                    if ((int)((object[])structDef[0])[1] != 1) return;
                    switch ((StructureItemType)((object[])structDef[0])[2])
                    {
                        case StructureItemType.Vector8:
                            iVectorsBankNum = 8;
                            break;
                        case StructureItemType.Vector1:
                            iVectorsBankNum = 1;
                            break;
                        case StructureItemType.Vector9:
                            iVectorsBankNum = 9;
                            break;
                        case StructureItemType.Vector0:
                            iVectorsBankNum = 0;
                            break;
                        default:
                            return;
                    }
                    break;
                default:
                    return;
            }
            bIsVectorsList = true;
        }

        private void checkVectorsStructure()
        {
            bIsVectorsStructure = false;

            if (!isValid) return;
            if (isEmpty) return;
            if (structDef.Length != 2) return;
            if (MaxSizeSingle != 2) return;
            if (((object[])structDef[0])[0].ToString() != "COND") return;
            if (((object[])structDef[1])[0].ToString() != "COND") return;

            /*
            IF (00:1) { WordHex, " EXIT" } ELSE {
            Vect8
            } 
            */

            object[] firstCond = (object[])structDef[0];
            object[] secondCond = (object[])structDef[1];

            switch (((object[])((object[])firstCond[3])[0])[0].ToString())
            {
                case "VAL":
                    if ((int)((object[])((object[])firstCond[3])[0])[1] != 1) return;
                    switch ((StructureItemType)((object[])((object[])firstCond[3])[0])[2])
                    {
                        case StructureItemType.WordHex:
                            break;
                        default:
                            return;
                    }
                    break;
                default:
                    return;
            }

            switch (((object[])((object[])secondCond[3])[0])[0].ToString())
            {
                case "VAL":
                    if ((int)((object[])((object[])secondCond[3])[0])[1] != 1) return;
                    switch ((StructureItemType)((object[])((object[])secondCond[3])[0])[2])
                    {
                        case StructureItemType.Vector8:
                        case StructureItemType.Vector1:
                        case StructureItemType.Vector9:
                        case StructureItemType.Vector0:
                            break;
                        default:
                            return;
                    }
                    break;
                default:
                    return;
            }

            bIsVectorsStructure = true;
        }

        private void checkOtherVectors()
        {
            bContainsOtherVectorAddresses = false;

            if (!isValid) return;
            if (isEmpty) return;

            bContainsOtherVectorAddresses = checkOtherVectors(structDef);
        }

        private bool checkOtherVectors(object[] arrDef)
        {
            if (arrDef == null) return false;

            foreach (object[] structItem in arrDef)
            {
                switch (structItem[0].ToString())
                {
                    case "VAL":
                        switch ((StructureItemType)(structItem[2]))
                        {
                            case StructureItemType.Vector8:
                            case StructureItemType.Vector1:
                            case StructureItemType.Vector9:
                            case StructureItemType.Vector0:
                                return true;
                        }
                        break;
                    case "COND":
                        if (checkOtherVectors((object[])structItem[3])) return true;
                        break;
                }
            }
            return false;
        }

        public void Read(ref string[] arrBytes, int number)
        {
            Lines = new ArrayList();
            includedOtherVectorAddresses = null;

            if (!isValid) return;
            if (isEmpty) return;
            if (number == 0) return;

            int iPos = 0;
            for (int iNum = 0; iNum < number; iNum++)
            {
                if (!ReadLines(ref Lines, ref arrBytes, iPos, iNum))
                {
                    Lines = new ArrayList();
                    break;
                }
                iPos += ((StructureLine)Lines[Lines.Count - 1]).Size;
            }

            // Included Other Vectors Management
            if (bContainsOtherVectorAddresses)
            {
                ArrayList alIncludedOtherVectorAddresses = new ArrayList();
                foreach (StructureLine sLine in Lines)
                {
                    foreach (StructureItem sItem in sLine.Items)
                    {
                        int bNum = -1;
                        switch (sItem.Type)
                        {
                            case StructureItemType.Vector0:
                                bNum = 0;
                                break;
                            case StructureItemType.Vector1:
                                bNum = 1;
                                break;
                            case StructureItemType.Vector8:
                                bNum = 8;
                                break;
                            case StructureItemType.Vector9:
                                bNum = 9;
                                break;
                        }
                        if (bNum != -1) alIncludedOtherVectorAddresses.Add(new int[] { bNum, Convert.ToInt32(sItem.Value(sLine.NumberInStructure), 16) - SADDef.EecBankStartAddress });
                    }
                }
                includedOtherVectorAddresses = (int[][])alIncludedOtherVectorAddresses.ToArray(typeof(int[]));
            }
        }

        private bool ReadLines(ref ArrayList arrLines, ref string[] arrBytes, int iPos, int iNum)
        {
            StructureLine structLine = null;

            object[] subStructDef = null;
            string[] subArrBytes = null;
            Structure subStruct = null;
            BitArray bitArray = null;
            int iNumPos = iPos;

            try
            {
                structLine = new StructureLine(BankNum, AddressInt + iPos, AddressBinInt + iPos, iNum);

                for (int iElem = 0; iElem < structDef.Length; iElem++)
                {
                    object[] structDefElem = (object[])structDef[iElem];
                    switch (structDefElem[0].ToString())
                    {
                        case "COND":
                            int iBF = -1;
                            bool validCond = false;
                            if (structDefElem[2].ToString().Replace("!", "").Length == 2)
                            {
                                if (structDefElem[2].ToString().Replace("!", "").ToLower().ToCharArray()[0] == 'b')
                                {
                                    iBF = Convert.ToInt32(structDefElem[2].ToString().Replace("!", "").Substring(1, 1));
                                    if (iBF < 0 || iBF > 7) iBF = -1;
                                }
                            }
                            if (iBF >= 0)
                            {
                                bitArray = new BitArray(new int[] { Tools.getByteInt(arrBytes[(int)structDefElem[1] + iNumPos - 1], false) });
                                if (structDefElem[2].ToString().ToLower().StartsWith("b")) validCond = bitArray[iBF];
                                else validCond = !bitArray[iBF];
                                bitArray = null;
                            }
                            else
                            {
                                validCond = (structDefElem[2].ToString().Replace("!", string.Empty).ToLower() == arrBytes[(int)structDefElem[1] + iNumPos - 1].ToLower());
                                if (structDefElem[2].ToString().StartsWith("!")) validCond = !validCond;
                            }

                            // Items will be calculated from related Sub Structure
                            if (validCond)
                            {
                                // Items calculation through Sub Structure
                                subStructDef = (object[])structDefElem[3];
                                subArrBytes = new string[arrBytes.Length - iPos];
                                for (int iSubPos = 0; iSubPos < arrBytes.Length - iPos; iSubPos++) subArrBytes[iSubPos] = arrBytes[iPos + iSubPos];
                                subStruct = new Structure(structLine.BankNum, structLine.AddressInt + iPos, structLine.AddressBinInt + iPos, subStructDef);
                                subStruct.Read(ref subArrBytes, 1);
                                for (int iLine = 0; iLine < subStruct.Lines.Count; iLine++)
                                {
                                    if (iLine == 0 || iLine == subStruct.Lines.Count - 1)
                                    {
                                        foreach (StructureItem sItem in ((StructureLine)subStruct.Lines[iLine]).Items) structLine.Items.Add(sItem);
                                        iPos += ((StructureLine)subStruct.Lines[iLine]).Size;
                                        if (iLine == 0 && subStruct.Lines.Count > 1)
                                        {
                                            arrLines.Add(structLine);
                                            structLine = new StructureLine(BankNum, AddressInt + iPos, AddressBinInt + iPos, iNum);
                                        }
                                    }
                                    else
                                    {
                                        arrLines.Add(subStruct.Lines[iLine]);
                                    }
                                }
                                subStructDef = null;
                                subArrBytes = null;
                                subStruct = null;
                            }
                            break;
                        case "CR":
                            if (structLine.Items.Count > 0)
                            {
                                arrLines.Add(structLine);
                                structLine = new StructureLine(BankNum, AddressInt + iPos, AddressBinInt + iPos, iNum);
                            }
                            break;
                        case "VAL":
                            int iItemsNum = -1;
                            int iBytesNum = -1;
                            string sString = string.Empty;
                            string sScaleExp = string.Empty;
                            switch ((StructureItemType)structDefElem[2])
                            {
                                case StructureItemType.Byte:
                                case StructureItemType.SignedByte:
                                    iItemsNum = (int)structDefElem[1];
                                    iBytesNum = 1;
                                    sScaleExp = structDefElem[4].ToString();
                                    break;
                                case StructureItemType.ByteHex:
                                    iItemsNum = (int)structDefElem[1];
                                    iBytesNum = 1;
                                    break;
                                case StructureItemType.Word:
                                case StructureItemType.SignedWord:
                                    iItemsNum = (int)structDefElem[1];
                                    iBytesNum = 2;
                                    sScaleExp = structDefElem[4].ToString();
                                    break;
                                case StructureItemType.WordHex:
                                case StructureItemType.Vector8:
                                case StructureItemType.Vector1:
                                case StructureItemType.Vector9:
                                case StructureItemType.Vector0:
                                    iItemsNum = (int)structDefElem[1];
                                    iBytesNum = 2;
                                    break;
                                case StructureItemType.Skip:
                                    iItemsNum = 0;
                                    iPos += (int)structDefElem[1];
                                    break;
                                case StructureItemType.String:
                                    iItemsNum = (int)structDefElem[1];
                                    iBytesNum = 0;
                                    sString = structDefElem[4].ToString();
                                    break;
                                case StructureItemType.Num:
                                case StructureItemType.NumHex:
                                    iItemsNum = (int)structDefElem[1];
                                    iBytesNum = 0;
                                    sScaleExp = structDefElem[4].ToString();
                                    break;
                                case StructureItemType.Empty:
                                    iItemsNum = (int)structDefElem[1];
                                    iBytesNum = 0;
                                    break;
                                default:
                                    iItemsNum = 1;
                                    iBytesNum = (int)structDefElem[1];
                                    break;
                            }
                            for (int iItemNum = 0; iItemNum < iItemsNum; iItemNum++)
                            {
                                StructureItem structItem = new StructureItem(BankNum, AddressInt + iPos, AddressBinInt + iPos, (StructureItemType)structDefElem[2]);
                                if (sString != string.Empty) structItem.FixedValue = sString;
                                if (sScaleExp != string.Empty) structItem.ScaleExpression = sScaleExp;
                                if (iBytesNum > 0)
                                {
                                    structItem.arrBytes = new string[iBytesNum];
                                    for (int iByte = 0; iByte < structItem.arrBytes.Length; iByte++) structItem.arrBytes[iByte] = arrBytes[iPos + iByte];
                                }
                                structLine.Items.Add(structItem);
                                iPos += structItem.Size;
                                structItem = null;
                            }
                            break;
                    }
                }

                arrLines.Add(structLine);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    // OP Codes
    public enum OPCodeParamsTypes
    {
        ValueByte,                      // VB
        ValueWordPart,                  // WB
        RegisterByte,                   // RB
        RegisterWord,                   // RW
        AddressRelativePosition,        // AR
        AddressPartRelativePosition,    // AB
        AddressPartAbsolutePosition,    // AA
        Bank                            // BN
    }

    public enum OPCodeType
    {
        UndefinedOP,                    // UOP
        ByteOP,                         // BOP
        WordOP,                         // WOP
        ShortJumpOP,                    // SJO
        BitByteGotoOP,                  // BGO
        GotoOP,                         // GOP
        MixedOP,                        // MOP
    }
    
    public struct OPCodeParameter
    {
        public string DefType;
        public OPCodeParamsTypes Type;
        public bool isPointer;          // XXP
    }

    public enum CarryMode
    {
        Default,        // DEFAULT
        Carry,          // CY
        Comparison,     // CMP
        Substract,      // -
        AddWords,       // +W
        AddBytes,       // +B
        MultiplyWords,  // *W
        MultiplyBytes   // *B
    }

    // GotoOpParam
    // GotoOpParam[Ope UniqueAddress, Ope Elem UniqueAddress, P1, P2, CY Ope UniqueAddress, CY Mode, CY P1, CY P2]
    public class GotoOpParams
    {
        // 20210909 - PYM - Reference to Operation for better results
        public Operation Ope = null;
        
        public int OpeBankNum = -1;
        public int OpeAddressInt = -1;

        public object OpeEmbeddedParam1 = null;
        public object OpeEmbeddedParam2 = null;
        
        public string OpeDefaultParamTranslation1 = "0";
        public string OpeDefaultParamTranslation2 = "0";

        // 20210909 - PYM - Reversed meaning mngt
        public bool OpeReversedMeaning = false;
        
        public int ElemBankNum = -1;
        public int ElemAddressInt = -1;

        // 20210909 - PYM - Reference to Carry Operation for better results
        public Operation CarryOpe = null;

        public int CarryOpeBankNum = -1;
        public int CarryOpeAddressInt = -1;

        public CarryMode CarryOpeMode = CarryMode.Default;

        public object CarryOpeEmbeddedParam1 = null;
        public object CarryOpeEmbeddedParam2 = null;

        public string CarryOpeDefaultParamTranslation1 = "0";
        public string CarryOpeDefaultParamTranslation2 = "0";

        public string OpeUniqueAddress { get { if (OpeBankNum == -1 || OpeAddressInt == -1) return string.Empty; else return string.Format("{0,1} {1,5}", OpeBankNum, OpeAddressInt); } }
        public string ElemUniqueAddress { get { if (ElemBankNum == -1 || ElemAddressInt == -1) return string.Empty; else return string.Format("{0,1} {1,5}", ElemBankNum, ElemAddressInt); } }
        public string CarryOpeUniqueAddress { get { if (CarryOpeBankNum == -1 || CarryOpeAddressInt == -1) return string.Empty; else return string.Format("{0,1} {1,5}", CarryOpeBankNum, CarryOpeAddressInt); } }

        public string OpeAddress { get { return string.Format("{0:x4}", OpeAddressInt + SADDef.EecBankStartAddress); } }
        public string ElemAddress { get { return string.Format("{0:x4}", ElemAddressInt + SADDef.EecBankStartAddress); } }
        public string CarryOpeAddress { get { return string.Format("{0:x4}", CarryOpeAddressInt + SADDef.EecBankStartAddress); } }

        public GotoOpParams(Operation oOpe, int opeBankNum, int opeAddressInt)
        {
            Ope = oOpe;
            OpeBankNum = opeBankNum;
            OpeAddressInt = opeAddressInt;
        }

        public GotoOpParams Clone()
        {
            GotoOpParams clone = new GotoOpParams(Ope, OpeBankNum, OpeAddressInt);

            clone.OpeEmbeddedParam1 = OpeEmbeddedParam1;
            clone.OpeEmbeddedParam2 = OpeEmbeddedParam2;

            clone.OpeDefaultParamTranslation1 = OpeDefaultParamTranslation1;
            clone.OpeDefaultParamTranslation2 = OpeDefaultParamTranslation2;

            clone.OpeReversedMeaning = OpeReversedMeaning;
            
            clone.ElemBankNum = ElemBankNum;
            clone.ElemAddressInt = ElemAddressInt;

            clone.CarryOpe = CarryOpe;

            clone.CarryOpeBankNum = CarryOpeBankNum;
            clone.CarryOpeAddressInt = CarryOpeAddressInt;

            clone.CarryOpeMode = CarryOpeMode;

            clone.CarryOpeEmbeddedParam1 = CarryOpeEmbeddedParam1;
            clone.CarryOpeEmbeddedParam2 = CarryOpeEmbeddedParam2;

            clone.CarryOpeDefaultParamTranslation1 = CarryOpeDefaultParamTranslation1;
            clone.CarryOpeDefaultParamTranslation2 = CarryOpeDefaultParamTranslation2;

            return clone;
        }
    }

    // Operation Param
    public class OperationParam
    {
        public string InstructedParam = string.Empty;
        public string CalculatedParam = string.Empty;
        public string DefaultTranslatedParam = string.Empty;

        public object EmbeddedParam = null;

        public string OriginalInstructedParam = string.Empty;

        // Statistics
        private ArrayList possibleStatistics = null;

        public void addPossibleStatistic(object statisticsEnumItem)
        {
            if (possibleStatistics == null) possibleStatistics = new ArrayList();
            if (!possibleStatistics.Contains(statisticsEnumItem)) possibleStatistics.Add(statisticsEnumItem);
        }

        public bool isPossibleStatistic(object statisticsEnumItem)
        {
            if (possibleStatistics == null) return false;
            return possibleStatistics.Contains(statisticsEnumItem);
        }
    }

    public class CallArgsParam
    {
        public string DefaultTranslatedParam = string.Empty;

        public object EmbeddedParam = null;
    }
    
    // Operation
    public class Operation
    {
        // 20200409 - PYM - Creation Parameters to get a backup and to be able to reprocess operation entirely
        public SADOPCode OPCode = null;
        public bool ApplySignedAlt = false;
        public Operation initialOpPrevResult = null;
        public GotoOpParams initialGotoOpParams = null;

        public string OriginalOPCode = string.Empty;
        public string OriginalInstruction = string.Empty;

        public int BankNum = -1;
        public int AddressInt = -1;

        public int InitialCallAddressInt = -1;
        
        public int BytesNumber = -1;
        public string[] OriginalOpArr = new string[] {};
        public int AddressJumpInt;

        public int CallArgsNum = 0;
        public string[] CallArgsArr = new string[] {};
        public CallArgument[] CallArguments = null;
        public CallArgsParam[] CallArgsParams = new CallArgsParam[] { }; 
        public string CallArgsStructRegister = string.Empty;

        public GotoOpParams GotoOpParams = null;

        public int SetBankNum = -1;
        public int ApplyOnBankNum = -1;
        public int ReadDataBankNum = -1;

        public string forcedInstruction = string.Empty;
        public string forcedTranslation1 = string.Empty;
        public string forcedTranslation2 = string.Empty;
        public string forcedTranslation3 = string.Empty;

        public string TranslationReplacementAddress = string.Empty;
        public string TranslationReplacementLabel = string.Empty;

        public OperationParam[] OperationParams = new OperationParam[] { };
        public int IgnoredTranslatedParam = -1;

        public CallType CallType = CallType.Unknown;
        
        public bool isReturn = false;
        public bool isFEConflict = false;

        public ArrayList alCalibrationElems = null;

        public string OtherElemAddress = string.Empty;
        public string KnownElemAddress = string.Empty;

        public string CalElemRBaseStructRBase = string.Empty;
        public string CalElemRBaseStructAddress = string.Empty;
        public bool CalElemRBaseStructAdder = false;

        public S6xOperation S6xOperation = null;

        public bool isKamRelated = false;
        public bool isCcRelated = false;
        public bool isEcRelated = false;

        public int VectorListAddressInt = -1;
        public int VectorListBankNum = -1;

        public int AddressNextInt { get { return AddressInt + BytesNumber + CallArgsNum; } }
        
        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }
        public string AddressNext { get { return string.Format("{0:x4}", AddressNextInt + SADDef.EecBankStartAddress); } }
        public string AddressJump { get { if (AddressJumpInt < 0) return string.Empty; else return string.Format("{0:x4}", AddressJumpInt + SADDef.EecBankStartAddress); } }

        public string InitialCallAddress { get { return string.Format("{0:x4}", InitialCallAddressInt + SADDef.EecBankStartAddress); } }

        public string OriginalOp { get { if (OriginalOpArr == null) return string.Empty; else return string.Join(SADDef.GlobalSeparator.ToString(), OriginalOpArr); } }

        public string CallArgsAddress { get { if (CallArgsNum == 0) return string.Empty; else return string.Format("{0:x4}", AddressInt + BytesNumber + SADDef.EecBankStartAddress); } }
        public string CallArgs { get { if (CallArgsArr == null) return string.Empty; else return string.Join(SADDef.GlobalSeparator.ToString(), CallArgsArr); } }
        public string CallArgsTranslated
        {
            get
            {
                if (CallArgsParams == null) return string.Empty;

                string[] sTranslations = new string[CallArgsParams.Length];
                for (int iArg = 0; iArg < sTranslations.Length; iArg++) sTranslations[iArg] = OPCode.getParamTranslation(CallArgsParams[iArg].EmbeddedParam, CallArgsParams[iArg].DefaultTranslatedParam, false);

                return string.Join(SADDef.GlobalSeparator.ToString(), sTranslations);
            }
        }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }
        public string UniqueCallArgsAddressHex { get { if (CallArgsNum == 0) return string.Empty; else return string.Format("{0,1} {1,4}", BankNum, CallArgsAddress); } }

        public string Instruction
        {
            get
            {
                if (forcedInstruction != string.Empty) return forcedInstruction;
                if (OPCode != null) return OPCode.TranslateInstruction(this);
                return string.Empty;
            }
        }

        public string Translation
        {
            get
            {
                if (OPCode != null) return OPCode.TranslateTranslation(this);
                return string.Empty;
            }
        }

        public string Translation1 { get { if (forcedTranslation1 != string.Empty) return forcedTranslation1; else return Translation; } }
        public string Translation2 { get { if (forcedTranslation2 != string.Empty) return forcedTranslation2; else return Translation; } }
        public string Translation3 { get { if (forcedTranslation3 != string.Empty) return forcedTranslation3; else return Translation; } } 
        
        public string Label
        {
            get
            {
                if (S6xOperation == null) return string.Empty;
                else if (S6xOperation.Label == null) return string.Empty;
                else return S6xOperation.Label;
            }
        }

        public string ShortLabel
        {
            get
            {
                if (S6xOperation == null) return string.Empty;
                else if (S6xOperation.ShortLabel == null) return string.Empty;
                else return S6xOperation.ShortLabel;
            }
        }

        public string FullLabel
        {
            get
            {
                if (Label != string.Empty && ShortLabel != string.Empty && Label != ShortLabel) return ShortLabel + " - " + Label;
                else return Label;
            }
        }

        public string Comments
        {
            get
            {
                if (S6xOperation == null) return string.Empty;
                else if (S6xOperation.Comments == null) return string.Empty;
                else if (!S6xOperation.OutputComments) return string.Empty;
                else return S6xOperation.Comments;
            }
        }

        public bool OutputComments
        {
            get
            {
                if (S6xOperation == null) return false;
                else return S6xOperation.OutputComments;
            }
        }

        public bool InlineComments
        {
            get
            {
                if (S6xOperation == null) return false;
                else return S6xOperation.InlineComments;
            }
        }

        public Operation(int bankNum, int addressInt)
        {
            BankNum = bankNum;
            AddressInt = addressInt;
        }
    }

    public class OperationNode
    {
        public Operation Operation = null;
        public OperationNode[] Branch = null;

        public OperationNode(ref Operation ope)
        {
            Operation = ope;
        }
    }
    
    public class RoutineSkeleton
    {
        public int BankNum = -1;
        public int AddressInt = -1;
        public int LastOperationAddressInt = -1;

        private string opsSkeleton = string.Empty;
        private string opsBytes = string.Empty;
        private int opsNumber = -1;

        public string ShortLabel = string.Empty;
        public string Label = string.Empty;
        public string Comments = string.Empty;

        public ArrayList alMatches = null;

        // Only provided related with a Binary file, not a skeleton file
        public ArrayList alOperations = null;
        public ArrayList alMixedElements = null;

        public SortedList slPossibleMatchingRegisters = null;
        public SortedList slPossibleMatchingMixedElements = null;

        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }

        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public string FullLabel
        {
            get
            {
                if (Label != string.Empty && ShortLabel != string.Empty && Label != ShortLabel) return ShortLabel + " - " + Label;
                else return Label;
            }
        }

        public string Skeleton
        {
            get { return opsSkeleton; }
        }

        public string Bytes
        {
            get { return opsBytes; }
        }

        public int OpsNumber
        {
            get { return opsNumber; }
        }

        // 20200908 - Created as base for everything
        public string getOperationSkeleton(Operation ope)
        {
            if (ope == null) return null;
            if (ope.OriginalOpArr.Length <= 0) return null;
            string opeCode = ope.OriginalOpArr[0];
            bool signedOpe = (ope.OriginalOpArr.Length > 1 && opeCode == "fe");
            if (signedOpe) opeCode = ope.OriginalOpArr[1];
            // Short Call becomes Call and Short Jump becomes Jump
            switch (opeCode)
            {
                case "20":
                case "21":
                case "22":
                case "23":
                case "24":
                case "25":
                case "26":
                case "27":
                    opeCode = "e7";
                    break;
                case "28":
                case "29":
                case "2a":
                case "2b":
                case "2c":
                case "2d":
                case "2e":
                case "2f":
                    opeCode = "ef";
                    break;
            }
            // JB/JNB cases BitFlags can be inverted between registers
            switch (opeCode)
            {
                case "30":
                case "31":
                case "32":
                case "33":
                case "34":
                case "35":
                case "36":
                case "37":
                    opeCode = "30";
                    break;
                case "38":
                case "39":
                case "3a":
                case "3b":
                case "3c":
                case "3d":
                case "3e":
                case "3f":
                    opeCode = "38";
                    break;
            }
            // Other equivalences
            switch (opeCode)
            {
                case "c3":  // Storing based on RConst c3, replaced by equivalent store c0
                    opeCode = "c0";
                    break;
                case "c7":  // Storing based on RConst c3, replaced by equivalent store c0
                    opeCode = "c4";
                    break;
                // Other Hard coded values ops, replaced by equivalent Calibration Element ops
                case "a0":
                case "a1":
                    opeCode = "a3";
                    break;
                case "ac":
                case "ad":
                    opeCode = "af";
                    break;
                case "b0":
                case "b1":
                    opeCode = "b3";
                    break;
                case "bc":
                case "bd":
                    opeCode = "bf";
                    break;
                case "88":
                case "89":
                    opeCode = "8b";
                    break;
                case "98":
                case "99":
                    opeCode = "9b";
                    break;
                case "44": // Calibration Element loaded through 45, replaced by equivalent Calibration Element load through a3
                case "45":
                case "47":
                    opeCode = "a3";
                    break;
                case "48":
                case "49":
                    opeCode = "4b";
                    break;
                case "4c":
                case "4d":
                    opeCode = "4f";
                    break;
                case "54":
                case "55":
                    opeCode = "57";
                    break;
                case "58":
                case "59":
                    opeCode = "5b";
                    break;
                case "5c":
                case "5d":
                    opeCode = "5f";
                    break;
                case "64":
                case "65":
                    opeCode = "67";
                    break;
                case "68":
                case "69":
                    opeCode = "6b";
                    break;
                case "6c":
                case "6d":
                    opeCode = "6f";
                    break;
                case "74":
                case "75":
                    opeCode = "77";
                    break;
                case "78":
                case "79":
                    opeCode = "7b";
                    break;
                case "7c":
                case "7d":
                    opeCode = "7f";
                    break;
            }
            // 0xFE case
            if (signedOpe) opeCode = "fe" + SADDef.GlobalSeparator + opeCode;
            return opeCode;
        }

        // 20200908 - Reviewed by using getOperationSkeleton
        public void setSkeleton()
        {
            opsSkeleton = string.Empty;
            if (alOperations == null)
            {
                opsNumber = (alOperations == null) ? -1 : alOperations.Count;
                opsBytes = (alOperations == null) ? string.Empty : ToolsRoutinesComp.skeletonToBytes(opsSkeleton);
            }
            else
            {
                opsSkeleton = string.Empty;
                foreach (Operation currentOpe in alOperations)
                {
                    string opSkeleton = getOperationSkeleton(currentOpe);
                    if (opSkeleton != null) opsSkeleton += "\t" + opSkeleton + "\n";
                }
                opsNumber = alOperations.Count;
                opsBytes = ToolsRoutinesComp.skeletonToBytes(opsSkeleton);
            }
        }

        public void setSkeleton(string sSkeleton)
        {
            opsSkeleton = sSkeleton;
            alOperations = null;
            opsNumber = opsSkeleton.Split('\n').Length - 1;
            opsBytes = ToolsRoutinesComp.skeletonToBytes(opsSkeleton);
        }

        public RoutineSkeleton Clone()
        {
            RoutineSkeleton clone = new RoutineSkeleton();

            clone.BankNum = BankNum;
            clone.AddressInt = AddressInt;
            clone.LastOperationAddressInt = LastOperationAddressInt;
            
            clone.opsSkeleton = opsSkeleton;
            clone.opsBytes = opsBytes;
            clone.opsNumber = opsNumber;

            clone.ShortLabel = ShortLabel;
            clone.Label = Label;
            clone.Comments = Comments;

            if (alMatches != null)
            {
                clone.alMatches = new ArrayList();
                foreach (object oObj in alMatches) clone.alMatches.Add(oObj);
            }

            if (alOperations != null)
            {
                clone.alOperations = new ArrayList();
                foreach (object oObj in alOperations) clone.alOperations.Add(oObj);
            }

            if (alMixedElements != null)
            {
                clone.alMixedElements = new ArrayList();
                foreach (object oObj in alMixedElements) clone.alMixedElements.Add(oObj);
            }

            return clone;
        }
    }

    public class RoutineSkeletonAnalysisMatchedObject
    {
        public string MatchedKey = string.Empty;
        public string MatchedValue = string.Empty;
        public SortedList slMatchings = null;
        
        public SortedList slSubMatched = null;

        public RoutineSkeletonAnalysisMatchedObject ParentMatchedObject = null;

        public bool UniqueMatching = true;
        public bool UniqueCounterMatching = true;

        public bool SubMatchingCompatibility = true;

        public RoutineSkeletonAnalysisMatchedObject(string matchedKey)
        {
            MatchedKey = matchedKey;
            MatchedValue = matchedKey;

            slMatchings = new SortedList();
        }

        public RoutineSkeletonAnalysisMatchingObject AddMatching(string matchingKey, int addedOccurences)
        {
            RoutineSkeletonAnalysisMatchingObject rsaMGO = (RoutineSkeletonAnalysisMatchingObject)slMatchings[matchingKey];
            if (rsaMGO == null)
            {
                rsaMGO = new RoutineSkeletonAnalysisMatchingObject(MatchedKey, matchingKey);
                slMatchings.Add(rsaMGO.MatchingKey, rsaMGO);
            }
            rsaMGO.AddOccurences(addedOccurences);

            return rsaMGO;
        }

        public RoutineSkeletonAnalysisMatchedObject AddSubMatched(string subMatchedKey)
        {
            if (slSubMatched == null) slSubMatched = new SortedList();

            RoutineSkeletonAnalysisMatchedObject rsaSubMDO = (RoutineSkeletonAnalysisMatchedObject)slSubMatched[subMatchedKey];
            if (rsaSubMDO == null)
            {
                rsaSubMDO = new RoutineSkeletonAnalysisMatchedObject(subMatchedKey);
                rsaSubMDO.ParentMatchedObject = this;
                slSubMatched.Add(rsaSubMDO.MatchedKey, rsaSubMDO);
            }
            return rsaSubMDO;
        }
    }

    public class RoutineSkeletonAnalysisMatchingObject
    {
        public string MatchedKey = string.Empty;
        public string MatchingKey = string.Empty;
        public string MatchingValue = string.Empty;

        public string ParentMatchingKey = string.Empty;

        public int Occurences = 0;

        public int CounterMatchingCount = 1;

        public bool UniqueCounterMatching
        {
            get { return CounterMatchingCount == 1; }
        }

        public RoutineSkeletonAnalysisMatchingObject(string matchedKey, string matchingKey)
        {
            MatchedKey = matchedKey;
            MatchingKey = matchingKey;
            MatchingValue = matchingKey;
        }

        public void AddOccurences(int addedOccurences)
        {
            Occurences += addedOccurences;
        }
    }
}
