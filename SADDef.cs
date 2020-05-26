using System;
using System.Collections;
using System.Text;

namespace SAD806x
{
    public static class SADDef
    {
        // SAD 806x Clipboard Format
        public const string S6xClipboardFormat = "S6XCLIPBOARDFMT_1.0";

        // TunerPro Clipboard Format
        public const string XdfClipboardFormat = "TPXDFCLIPBOARDFMT_1.0";
        
        // Settings file names
        public const string recentFilesFileName = "recent.xml";
        public const string settingsTextOuputFileName = "settingstextoutput.xml";
        public const string settingsSAD806xImpExpFileName = "settingss6ximpexp.xml";
        public const string settingsSADImpExpFileName = "settingssadimpexp.xml";
        public const string settingsTunerProImpExpFileName = "settingstpimpexp.xml";
        public const string settings806xUniDbImpExpFileName = "settingsudbimpexp.xml";
        
        // Repository file names
        public const string repoFileNameRegisters = "registers.xml";
        public const string repoFileNameStructures = "structures.xml";
        public const string repoFileNameTables = "tables.xml";
        public const string repoFileNameFunctions = "functions.xml";
        public const string repoFileNameScalars = "scalars.xml";
        public const string repoFileNameUnits = "units.xml";
        public const string repoFileNameConversion = "conversion.xml";
        public const string repoFileNameOBDIErrors = "obdierrors.xml";
        public const string repoFileNameOBDIIErrors = "obdiierrors.xml";

        // Repository other things
        public const string repoLabelOBDIErrorsShortLabel = "Code";
        public const string repoLabelOBDIIErrorsShortLabel = "Code";
        public const string repoToolTipOBDIErrorsShortLabel = "XX or XXX\r\n_ER for Engine Running\r\n_EO for Engine Off)";
        public const string repoToolTipOBDIIErrorsShortLabel = "PXXXX";
        public const string repoCommentsHeaderOBDIErrors = "Related with OBDI Code #OBDCODE#.";
        public const string repoCommentsHeaderOBDIIErrors = "Related with OBDII Code #OBDCODE#.";

        // Fixed EEC Bank Start Address 2000
        public const int EecBankStartAddress = 0x2000;

        // Default Values
        public const int DefaultScalePrecision = 2;
        public const int DefaultScaleMinPrecision = 0;
        public const int DefaultScaleMaxPrecision = 8;
        
        // Banks identification

        public const int Bank_Prev_Pattern_String_Length = 8;
        public static string[] Bank_Prev_Pattern_Possible_Bytes = new string[] { "FF", "00", "0F", "1F", "2F", "3F", "4F", "5F", "6F", "7F", "8F", "9F", "AF", "BF", "CF", "DF", "EF" };
        public static string[] Bank_Prev_Pattern_Possible_Fixed_Last_Bytes = new string[] {"91"};
        // To obtain "FFFFFFFF" or "FFFFFF91"

        public const string Bank_8_Early_SigStart_S = "E71D00FFFFFFFFFFFFFF";
        public const string Bank_8_Early_SigStart_1 = "FFFA201C";
        public const string Bank_8_Early_SigStart_2 = "FFFA2021";
        public const string Bank_8_Early_SigStart_3 = "FFFA2023";
        public const string Bank_8_9_0_SigStart = "FFFA";
        public const string Bank_9_0_SigStart = "FFFA27";
        public const string Bank_1_SigStart = "27FE";
        public const string Bank_9_SigEnd_1 = "60206520";
        public const string Bank_9_SigEnd_2 = "44FB49FB";
        public const string Bank_0_SigEnd_1 = "60206320";
        public const string Bank_0_SigEnd_2 = "A1A5A4A5";

        public const int Bank_Min_Size = 0x8000;
        public const int Bank_Max_Size = 0xe000;

        // 8065 Copyright Ascii for Early /Pilot 8065 Bank 1 short Identification
        public const string Bank_1_9_Copyright_Ascii = "436F70797269676874";

        // Start Information
        // 8061
        // RBase Signature
        public const string Info_8061_FirstRBase_Signature_1 = "A1" + "%CPOI%" + "..A1%FRBASE%00..B301" + "%LNUM%" + "..A2....C2....E0";
        public const string Info_8061_FirstRBase_Signature_2 = "A1" + "%CPOI%" + "..........A1%FRBASE%00..B301" + "%LNUM%" + "..A2....C2....E0";
        public const string Info_8061_FirstRBase_Signature_3 = "0901..A1%FRBASE%00..B301" + "%LNUM%" + "..A2....C2....E0";
        // Checksum Calculation Signature
        public const string Info_8061_CheckSumCalc_Signature_1 = "A301" + "%CSCSTARTP%" + "..A301" + "%CSCENDP%" + "..01..110566....88....D...8800..";
        public const string Info_8061_CheckSumCalc_Signature_2 = "A1" + "%CSCSTARTP%" + "......00..DF..A2..........66....88....";
        public const string Info_8061_CheckSumCalc_Signature_3 = "A1" + "%CSCSTARTP%" + "......00..DF..A2....66....88....";
        public const string Info_8061_CheckSumCalc_Signature_4 = "C3....00A1" + "%CSCSTARTP%" + "..17..F0A3......A2....8800..";
        public const string Info_8061_CheckSumCalc_Signature_5 = "A301" + "%CSCSTARTP%" + "..A301" + "%CSCENDP%" + "..01..95........110595....66....88....D...8900..";
        
        // VID Block
        public static object[] Info_8061_VID_Block_Addresses = new object[] {
            new object[] { 0x7fe0, 6, "ASCII", "STRATEGY", "FF", "Strategy", "Strategy"},
            new object[] { 0x7fe6, 26, "ASCII", "COPYRIGHT", "FF", "Copyright", "Copyright"}
        };

        // 8065
        // RBase Signature
        public const string Info_8065_FirstRBase_Signature_1 = "A1" + "%CPOI%" + "..A1%FRBASE%00..1008B301" + "%LNUM%" + "..1008A2....C2....E0";
        public const string Info_8065_FirstRBase_Signature_2 = "A1" + "%CPOI%" + "..........A1%FRBASE%00..1008B301" + "%LNUM%" + "..1008A2....C2....E0";
        public const string Info_8065_FirstRBase_Signature_3 = "A1" + "%CPOI%" + "..A1%FRBASE%00..B301" + "%LNUM%" + "..A2....C2....E0";
        // Checksum Calculation Signature
        public const string Info_8065_CheckSumCalc_Signature_Banks_8_0_9 = "A1" + "%CSCSTARTP%" + "..B1....10" + "%CSCBANK%" + "66....89" + "%CSCENDP%" + "..D3F210" + "%CSCBANK%";
        public const string Info_8065_CheckSumCalc_Signature_Bank_1 = "A1" + "%CSCSTARTP%" + "..B1....66....89" + "%CSCENDP%" + "..D1F4";
        // Checksum Calculation Signature Early 8065
        public const string Info_8065_Early_CheckSumCalc_Signature_Banks_8_1 = "A1" + "%CSCSTARTP%" + "..A1" + "%CSCENDP1%" + "..C3......2010A3......A3......A1" + "%CSCENDP8%" + "..";
        public const string Info_8065_Early_CheckSumCalc_Signature_Bank_8 = "100866....89" + "%CSCENDP%" + "..D3..100866";
        public const string Info_8065_Early_CheckSumCalc_Signature_Bank_1 = "66....89" + "%CSCENDP%" + "..D1..88..00DF";
        // Checksum Calculation Signature Pilot 8065
        public const string Info_8065_Pilot_CheckSumCalc_Signature_Banks_8 = "01..01..A1" + "%CSCSTARTP%" + "..46..00..DF..A2....66";

        // VID Block - On Early 8065, addresses will be lowered with 0x6000
        public static object[] Info_8065_VID_Block_Addresses = new object[] {
            new object[] { 0xdf06, 7, "ASCII", "STRATEGY", "FF", "Strategy", "Strategy"},
            new object[] { 0xdf14, 7, "ASCII", "SERIAL", "FF", "Part Number", "Part Number"},
            new object[] { 0xdf1d, 26, "HEX", "PATSCODE", "PATS Code", "PATS Code"},
            new object[] { 0xdf63, 29, "ASCII", "COPYRIGHT", "FF", "Copyright", "Copyright"},
            new object[] { 0xdf80, 17, "ASCII", "VIN", "FF", "VIN Code", "VIN Code"},
            new object[] { 0xdf95, 1, "FLAG", "VIDENABLED", "FF", "VID Block Enabled", "VID Block Enabled"},
            new object[] { 0xdf9a, 2, "WORD", "REVMILE", 1, "Tyre Revolutions per Mile", "Tyre Revolutions per Mile"},
            new object[] { 0xdf9c, 2, "WORD", "RTAXLE", (double)1/1024, "Rear End Gear Ratio", "Rear End Gear Ratio"}
        };

        // Output
        public const string GlobalSeparator = ",";
        public const string AdditionSeparator = "+";
        public const string NamingLongBankSeparator = " ";
        public const string NamingShortBankSeparator = "_";
        public const string VariableValuesSeparator = ".";
        public const string ShortRegisterPrefix = "R";
        public const string ShortPointerPrefix = "R";
        public const string LongRegisterPointerPrefix = "[";
        public const string LongRegisterPointerSuffix = "]";
        public static string LongRegisterTemplate = LongRegisterPointerPrefix + "%LREG%" + LongRegisterPointerSuffix;
        public static string LongPointerTemplate = LongRegisterPointerPrefix + "%POINTER%" + LongRegisterPointerSuffix;
        public const string IncrementSuffix = "++";

        public const string BitByteGotoOPAltSeparator = ".";
        public const string BitByteGotoOPAltComparison = " == ";
        public const string BitByteGotoOPAltHRegPrefix = "<";
        public const string BitByteGotoOPAltHRegSuffix = ">";
        public const string BitByteSetOPAltTemplate = "%1% = %2%;";
        public const string BitByteSetOPAltDefVal = "true";
        public const string BitByteUnSetOPAltDefVal = "false";

        public const string ReplacementCoreStartString = "$";
        public const string ReplacementCoreStartStringBis = "@";
        public const string ReplacementCoreBitFlagString = ":";
        
        // Signatures Keywords
        public const string SignatureParamBytePrefixSuffix = "#";

        // Special Templates
        public static string KAMRegister8061Template = "KAM_" + "%LREG%";
        public static string CCRegister8061Template = "CC_" + "%LREG%";
        public static string ECRegister8061Template = "EC_" + "%LREG%";
        public static string CCMemory8061Template = "CC_" + "%LREG%";
        public static string ECMemory8061Template = "EC_" + "%LREG%";

        public const string LongLabelCC8061Vector = "Calibration Console Vector";
        public const string LongLabelEC8061Vector = "Engineering Console Vector";

        // Recurring Labels
        public const string ShortOpePrefix = "Ope";
        
        public const string ShortCallPrefix = "Sub";
        public const string LongCallPrefix = "Sub ";
        public const string ShortCallFakePrefix = "FkSub";
        public const string LongCallFakePrefix = "Fake Sub ";

        public const string ShortTablePrefix = "Tb";
        public const string LongTablePrefix = "Table ";
        public const string ShortFunctionPrefix = "Fn";
        public const string LongFunctionPrefix = "Function ";
        public const string ShortScalarPrefix = "Sc";
        public const string LongScalarPrefix = "Scalar ";
        public const string ShortStructurePrefix = "St";
        public const string LongStructurePrefix = "Structure ";

        public const string ShortExtTablePrefix = "OTb";
        public const string LongExtTablePrefix = "Other Table ";
        public const string ShortExtFunctionPrefix = "OFn";
        public const string LongExtFunctionPrefix = "Other Function ";
        public const string ShortExtScalarPrefix = "OCn";
        public const string LongExtScalarPrefix = "Other Scalar ";
        public const string ShortExtStructurePrefix = "OSt";
        public const string LongExtStructurePrefix = "Other Structure ";

        public const string ShortVectListPrefix = "VecLst";
        public const string LongVectListPrefix = "Vectors List ";
        public const string ShortVectStructPrefix = "VecSt";
        public const string LongVectStructPrefix = "Vectors Structure ";

        public const string ShortOtherAddressPrefix = "OAdr";
        public const string LongOtherAddressPrefix = "Other Address ";

        public const string ArgumentCodePrefix = "Ar";

        // Other Labels
        public const string ShortLabelCallBankStartTemplate = "Bnk%1%Start";
        public const string LongLabelCallBankStartTemplate = "Bank %1% Start";

        public const string ShortLabelScalarRBaseEndNextAdr = "%1%_EndAddr";
        public const string LongLabelScalarRBaseEndNextAdr = "Rbase %1% end next address";
        public const string ShortLabelScalarCheckSumStartAdr = "ChkStartAdr";
        public const string LongLabelScalarCheckSumStartAdr = "Checksum Start Address";
        public const string ShortLabelScalarCheckSumEndAdr = "ChkEndAdr";
        public const string LongLabelScalarCheckSumEndAdr = "Checksum End Address";

        public const string ShortLabelStructCheckSumAdr = "ChkAdrStruct";
        public const string LongLabelStructCheckSumAdr = "Checksum Address Structure";

        public const string LongSignaturePrefix = "Signature for ";

        // Fake Call Duplicates Detection Size
        public const int FakeCallDuplicatesDetectionSize = 5;
        
        // Unknown Elements Aggregation Minimum Size
        public const int UnknownOpPartsAggregationSize = 8;
        public const int UnknownCalibPartsAggregationSize = 8;

        //8061 EC (Engineering Console) Ranges / CC (Calibration Console) Ranges / KAM (Keep Alive Memory) Ranges
        public const int KAMRegisters8061MinAdress = 0xa00;
        public const int KAMRegisters8061MaxAdress = 0xbff;
        public const int CCRegisters8061MinAdress = 0xc00;
        public const int CCRegisters8061MaxAdress = 0xfff;
        public const int ECRegisters8061MinAdress = 0x1000;
        public const int ECRegisters8061MaxAdress = 0x1fff;

        public const int CCMemory8061MinAdress = 0xc000;
        public const int CCMemory8061MaxAdress = 0xdfff;
        public const int ECMemory8061MinAdress = 0xe000;
        public const int ECMemory8061MaxAdress = 0xffff;

        //    Number, Relative Address, Size, Type, Translation, Comments

        // Bank 8 Reserved Address
        // 8061 Early
        // 2000-2005 => OP (2003-2005 => First Jump)
        // 2006-2009 => Fill
        // 200a => Checksum (Word)
        // 200c => SMP Base Address (Word)
        // 200e => Calibration Execution Time (Word) - CC_EXE_TIME
        // 2010-201F => 8 Interrupt Vectors (Word)
        public static object[] ReservedAddressesEarly8061Bank8 = new object[] {
            new object[] {1, 0x4, 2, "FILL", "fill", "fill" },
            new object[] {1, 0x6, 2, "ROMSIZE", "Bank Rom Size", "Hardware Bank Rom Size" },
            new object[] {1, 0x8, 2, "FILL", "fill", "fill" },
            new object[] {1, 0xa, 2, "CHECKSUM", "Checksum", "Checksum" },
            new object[] {1, 0xc, 2, "SMPBASEADR", "Smp Base Address", "Smp Base Address" },
            new object[] {1, 0xe, 2, "CCEXETIME", "Cc Exe Time", "Calibration Console Execution Time" },
            new object[] {8, 0x10, 2, "INTVECTORADR", "Int. Vector", "Interrupt Vector Address" }
        };
        // 8061
            // 2000-2005 => OP (2003-2005 => First Jump)
            // 2006-2009 => Fill
            // 200a => Checksum (Word)
            // 200c => SMP Base Address (Word)
            // 200e => Calibration Execution Time (Word) - CC_EXE_TIME
            // 2010-201F => 8 Interrupt Vectors (Word)
            // 2020 => Number of Levels (Byte)
            // 2021 => Number of Calibrations (Byte)
            // 2022-XXXX => Calibration Pointers (Word) - As many as Number of Levels
        public static object[] ReservedAddresses8061Bank8 = new object[] {
            new object[] {1, 0x6, 2, "ROMSIZE", "Bank Rom Size", "Bank Hardware Rom Size" },
            new object[] {1, 0x8, 2, "FILL", "fill", "fill" },
            new object[] {1, 0xa, 2, "CHECKSUM", "Checksum", "Checksum" },
            new object[] {1, 0xc, 2, "SMPBASEADR", "Smp Base Address", "Smp Base Address" },
            new object[] {1, 0xe, 2, "CCEXETIME", "Cc Exe Time", "Calibration Console Execution Time" },
            new object[] {8, 0x10, 2, "INTVECTORADR", "Int. Vector", "Interrupt Vector Address" },
            new object[] {1, 0x20, 1, "LEVNUM", "Levels Number", "Number of Levels" },
            new object[] {1, 0x21, 1, "CALNUM", "Calibs Number", "Number of Calibrations" },
            new object[] {-1, 0x22, 2, "RBASEADR", "Rbase", "Calibration / Rbase Pointer - As many as Number of Levels" }
        };
        // 8065
            // 2000-2004 => OP (2002-2004 => First Jump)
            // 2005-2009 => Fill
            // 200a => Checksum (Word)
            // 200c => SMP Base Address (Word)
            // 200e => Calibration Execution Time (Word) - CC_EXE_TIME
            // 2010-205F => 40 Interrupt Vectors (Word)
            // 2060 => Number of Levels (Byte)
            // 2061 => Number of Calibrations (Byte)
            // 2062-XXXX => Calibration Pointers (Word) - As many as Number of Levels
        public static object[] ReservedAddresses8065Bank8 = new object[] {
            new object[] {1, 0x5, 1, "FILL", "fill", "fill" },
            new object[] {1, 0x6, 2, "ROMSIZE", "Bank Rom Size", "Bank Hardware Rom Size" },
            new object[] {1, 0x8, 2, "FILL", "fill", "fill" },
            new object[] {1, 0xa, 2, "CHECKSUM", "Checksum", "Checksum" },
            new object[] {1, 0xc, 2, "SMPBASEADR", "Smp Base Address", "Smp Base Address" },
            new object[] {1, 0xe, 2, "CCEXETIME", "Cc Exe Time", "Calibration Console Execution Time" },
            new object[] {40, 0x10, 2, "INTVECTORADR", "Int. Vector", "Interrupt Vector Address" },
            new object[] {1, 0x60, 1, "LEVNUM", "Levels Number", "Number of Levels" },
            new object[] {1, 0x61, 1, "CALNUM", "Calibs Number", "Number of Calibrations" },
            new object[] {-1, 0x62, 2, "RBASEADR", "Rbase", "Calibration / Rbase Pointer - As many as Number of Levels x Number of Calibrations" }
        };

        // Bank 1
        public static object[] ReservedAddresses8065Bank1 = new object[] {
            new object[] {1, 0x2, 2, "WORD", "Word", "Word" },
            new object[] {1, 0x4, 2, "WORD", "Word", "Word" },
            new object[] {1, 0x6, 4, "FILL", "fill", "fill" },
            new object[] {1, 0xa, 2, "WORD", "Word", "Word" },
            new object[] {1, 0xc, 4, "FILL", "fill", "fill" },
            new object[] {40, 0x10, 2, "INTVECTORADR", "Int. Vector", "Interrupt Vector Address" },
        };

        // Bank 9
        public static object[] ReservedAddresses8065Bank9 = new object[] {
            new object[] {1, 0x4, 12, "FILL", "fill", "fill" },
            new object[] {40, 0x10, 2, "INTVECTORADR", "Int. Vector", "Interrupt Vector Address" },
        };

        // Bank 0
        public static object[] ReservedAddresses8065Bank0 = new object[] {
            new object[] {1, 0x4, 12, "FILL", "fill", "fill" },
            new object[] {40, 0x10, 2, "INTVECTORADR", "Int. Vector", "Interrupt Vector Address" },
        };

        // EEC Registers / EC (Engineering Console) Registers / CC (Calibration Console) Registers / KAM (Keep Alive Memory) Registers
        // Register, Variants, Translation, Comments
        //  Variants : RW Read Write
        //             RO Read Only
        //             WO Write Only
        //             + _B Byte Only
        //             + _W Byte Only
        
        // 8061
        public static object[] EecRegisters8061 = {
            new string[] { "00", "RW", "0", "Read as 0"},
            new string[] { "01", "RW", "0", "Read as 0"},
            new string[] { "02", "RW", "CPU_OK", "LSO Port (Non-Bidirectional)"},
            new string[] { "03", "RW", "LIO_PORT", "Bidirectional I/O Port"},
            new string[] { "04", "RO", "AD_LO", "A/D Result Lo"},
            new string[] { "04", "WO", "AD_CMD", "A/D Command (Channel #)"},
            new string[] { "05", "RO", "AD_HI", "A/D Result Hi"},
            new string[] { "05", "WO", "WD_TIMER", "Watchdog Timer"},
            new string[] { "06", "RW", "IO_TIMER", "Master I/O Timer Lo"},
            new string[] { "07", "RW", "IO_TIMER", "Master I/O Timer Hi"},
            new string[] { "08", "RW", "INT_MASK", "Interrup Mask"},
            new string[] { "09", "RW", "INT_PEND", "Interrup Pend"},
            new string[] { "0A", "RW", "HSO_OVF", "I/O Status Register"},
            new string[] { "0B", "RW", "HSI_SAMP", "HSI Sample Register"},
            new string[] { "0C", "RW", "HSI_MASK", "HSI Data Mask"},
            new string[] { "0D", "RO", "HSI_DATA", "HSI Data Hold"},
            new string[] { "0D", "WO", "HSO_CMD", "HSO Command Hold"},
            new string[] { "0E", "RO", "HSI_TIME", "HSI Time Hold Lo"},
            new string[] { "0E", "WO", "HSO_TIME", "HSO Time Hold Lo"},
            new string[] { "0F", "RO", "HSI_TIME", "HSI Time Hold Hi"},
            new string[] { "0F", "WO", "HSO_TIME", "HSO Time Hold Hi"},
            new string[] { "10", "RW", "STACK", "Stack Pointer Lo Byte"},
            new string[] { "11", "RW", "STACK", "Stack Pointer Hi Byte"},
            new string[] { "D00", "RW", "CC_PRESENT", "Calibration Console Present"},
            new string[] { "C80", "RW", "CC_FLAGS", "Calibration Console Flags"}
        };

        // EEC Registers
        // 8065
        public static object[] EecRegisters8065 = {
            new string[] { "00", "RW", "0", "Read as 0"},
            new string[] { "01", "RW", "0", "Read as 0"},
            new string[] { "02", "RW", "CPU_OK", "LSO Port (Non-Bidirectional)"},
            new string[] { "03", "RW", "LIO_PORT", "Bidirectional I/O Port"},
            new string[] { "04", "RO", "AD_LO", "A/D Immediate Result Lo"},
            new string[] { "04", "WO", "AD_CMD", "A/D Immediate Cmd"},
            new string[] { "05", "RO", "AD_HI", "A/D Immediate Result Hi"},
            new string[] { "05", "WO", "WD_TIMER", "Watchdog Timer"},
            new string[] { "06", "RW", "IO_TIMER", "Master I/O Timer Lo"},
            new string[] { "07", "RW_W", "IO_TIMER", "Master I/O Timer Mid"},
            new string[] { "07", "RO_B", "AD_TIME_R", "A/D-Timed Result Lo"},
            new string[] { "07", "RW_B", "AD_TIME_C", "A/D-Timed Cmd"},
            new string[] { "08", "RW", "INT_MASK", "Interrup Mask"},
            new string[] { "09", "RW", "INT_PEND", "Interrup Pend"},
            new string[] { "0A", "RW", "HSI_RDY", "I/O Status/Mem"},
            new string[] { "0B", "RO", "HSI_SAMPLE", "HSI Sample"},
            new string[] { "0B", "WO", "HSI_SAMPLE", "IDDQ Test Mode"},
            new string[] { "0C", "RW", "HSI_MASK", "HSI Transition Mask"},
            new string[] { "0D", "RO", "HSI_DATA", "HSI Data Hold"},
            new string[] { "0D", "WO", "HSO_CMD", "HSO Command Hold"},
            new string[] { "0E", "RO", "HSI_TIME", "HSI Time Hold Lo"},
            new string[] { "0E", "WO", "HSO_TIME", "HSO Time Hold Lo"},
            new string[] { "0F", "RO_W", "HSI_TIME", "HSI Time Hold Hi"},
            new string[] { "0F", "WO_W", "HSO_TIME", "HSO Time Hold Hi"},
            new string[] { "0F", "RW_B", "HSO_TIME", "A/D-Timed Result Hi"},
            new string[] { "10", "RW", "HSO_PEND1", "HSO Int Pend #1 Lo"},
            new string[] { "11", "RW_W", "HSO_PEND1", "HSO Int Pend #1 Hi"},
            new string[] { "11", "RW_B", "BANK_SEL", "Memory Bank Select"},
            new string[] { "12", "RW", "HSO_MASK1", "HSO Int Mask #1 Lo"},
            new string[] { "13", "RW_W", "HSO_MASK1", "HSO Int Mask #1 Hi"},
            new string[] { "13", "RW_B", "IO_TIMER", "Master I/O Timer Hi"},
            new string[] { "14", "RW", "HSO_PEND2", "HSO Int Pend #2 Lo"},
            new string[] { "15", "RW_W", "HSO_PEND2", "HSO Int Pend #2 Hi"},
            new string[] { "15", "RO_B", "LSSI_A", "LSSI #A"},
            new string[] { "15", "WO_B", "LSSO_A", "LSSO #A"},
            new string[] { "16", "RW", "HSO_MASK2", "HSO Int Mask #2 Lo"},
            new string[] { "17", "RW_W", "HSO_MASK2", "HSO Int Mask #2 Hi"},
            new string[] { "17", "RO_B", "LSSI_B", "LSSI #B"},
            new string[] { "17", "WO_B", "LSSO_B", "LSSO #B"},
            new string[] { "18", "RW", "HSO_RDY", "HSO State Lo"},
            new string[] { "19", "RW_W", "HSO_RDY", "HSO State Hi"},
            new string[] { "19", "RO_B", "LSSI_C", "LSSI #C"},
            new string[] { "19", "WO_B", "LSSO_C", "LSSO #C"},
            new string[] { "1A", "RW", "HSI_EDGE", "HSI Edge/Mode"},
            new string[] { "1B", "RW", "HSO_SCNT", "HSO Used Slot Cnt & Rst Status"},
            new string[] { "1C", "RW", "HSO_TCNT", "HSO Time/Cnt Read Lo"},
            new string[] { "1D", "RW_W", "HSO_TCNT", "HSO Time/Cnt Read Hi"},
            new string[] { "1D", "RO_B", "LSSI_D", "LSSI #D"},
            new string[] { "1D", "WO_B", "LSSO_D", "LSSO #D"},
            new string[] { "1E", "RW", "HSO_LSLOT", "HSO Last Slot Used"},
            new string[] { "1F", "RW", "HSO_SSLOT", "HSO Slot Select"},
            new string[] { "20", "RW", "STACK", "Stack Pointer Lo"},
            new string[] { "21", "RW", "STACK", "Stack Pointer Hi"},
            new string[] { "22", "RW", "ALTSTACK", "Alt Stack Pointer Lo"},
            new string[] { "23", "RW", "ALTSTACK", "Alt Stack Pointer Hi"},
            new string[] { "100", "RW", "0", "Read as 0"},
            new string[] { "101", "RW", "0", "Read as 0"},
            new string[] { "200", "RW", "0", "Read as 0"},
            new string[] { "201", "RW", "0", "Read as 0"},
            new string[] { "300", "RW", "0", "Read as 0"},
            new string[] { "301", "RW", "0", "Read as 0"}
        };

        // Interruption Vectors
        public static object[] IntVectors_8061 = {
            new object[] { 1, "IPT_HSO_2", "Interrupt High Speed Output 2", "Interrupt High Speed Output"},
            new object[] { 2, "IPT_Timer_OVF", "Interrupt Timer OVF", "Interrupt Timer OVF"},
            new object[] { 3, "IPT_HSI_0", "Interrupt High Speed Input 0", "Interrupt High Speed Input"},
            new object[] { 4, "IPT_HSI_Data", "Interrupt High Speed Input Data", "Interrupt High Speed Input Data"},
            new object[] { 5, "IPT_HSI_0", "Interrupt High Speed Input 0", "Interrupt High Speed Input"},
            new object[] { 6, "IPT_HSO_1", "Interrupt High Speed Output 1", "Interrupt High Speed Output"},
            new object[] { 7, "IPT_HSI_0", "Interrupt High Speed Input 0", "Interrupt High Speed Input"},
            new object[] { 8, "IPT_HSI_0", "Interrupt High Speed Input 0", "Interrupt High Speed Input"},
        };

        public static object[] IntVectors_8065 = {
            new object[] { 1, "IPT_HSO_0", "Interrupt High Speed Output 0", "Interrupt High Speed Output"},
            new object[] { 2, "IPT_HSO_1", "Interrupt High Speed Output 1", "Interrupt High Speed Output"},
            new object[] { 3, "IPT_HSO_2", "Interrupt High Speed Output 2", "Interrupt High Speed Output"},
            new object[] { 4, "IPT_HSO_3", "Interrupt High Speed Output 3", "Interrupt High Speed Output"},
            new object[] { 5, "IPT_HSO_4", "Interrupt High Speed Output 4", "Interrupt High Speed Output"},
            new object[] { 6, "IPT_HSO_5", "Interrupt High Speed Output 5", "Interrupt High Speed Output"},
            new object[] { 7, "IPT_HSO_6", "Interrupt High Speed Output 6", "Interrupt High Speed Output"},
            new object[] { 8, "IPT_HSO_7", "Interrupt High Speed Output 7", "Interrupt High Speed Output"},
            new object[] { 9, "IPT_HSO_8", "Interrupt High Speed Output 8", "Interrupt High Speed Output"},
            new object[] { 10, "IPT_HSO_9", "Interrupt High Speed Output 9", "Interrupt High Speed Output"},
            new object[] { 11, "IPT_HSO_10", "Interrupt High Speed Output 10", "Interrupt High Speed Output"},
            new object[] { 12, "IPT_HSO_11", "Interrupt High Speed Output 11", "Interrupt High Speed Output"},
            new object[] { 13, "IPT_HSO_12", "Interrupt High Speed Output 12", "Interrupt High Speed Output"},
            new object[] { 14, "IPT_HSO_13", "Interrupt High Speed Output 13", "Interrupt High Speed Output"},
            new object[] { 15, "IPT_HSO_14", "Interrupt High Speed Output 14", "Interrupt High Speed Output"},
            new object[] { 16, "IPT_HSO_15", "Interrupt High Speed Output 15", "Interrupt High Speed Output"},
            new object[] { 17, "IPT_HSI_FIFO", "Interrupt High Speed Input FIFO", "Interrupt High Speed Input FIFO"},
            new object[] { 18, "IPT_External", "Interrupt External", "Interrupt External"},
            new object[] { 19, "IPT_HSI_0", "Interrupt High Speed Input 0", "Interrupt High Speed Input"},
            new object[] { 20, "IPT_HSI_Data", "Interrupt High Speed Input Data", "Interrupt High Speed Input Data"},
            new object[] { 21, "IPT_HSI_1", "Interrupt High Speed Input 1", "Interrupt High Speed Input"},
            new object[] { 22, "IPT_AD_Imm_Rdy", "Interrupt Analogic/Digital Imm Ready", "Interrupt Analogic/Digital Imm Ready"},
            new object[] { 23, "IPT_AD_Timed_Rdy", "Interrupt Analogic/Digital Timed Ready", "Interrupt Analogic/Digital Timed Ready"},
            new object[] { 24, "IPT_ATimer_OVF", "Interrupt Analogic Timer OVF", "Interrupt Analogic Timer OVF"},
            new object[] { 25, "IPT_AD_Timed_Start", "Interrupt Analogic/Digital Timed Start", "Interrupt Analogic/Digital Timed Start"},
            new object[] { 26, "IPT_ATimer_reset", "Interrupt Analogic Timer Reset", "Interrupt Analogic Timer Reset"},
            new object[] { 27, "IPT_Counter_0", "Interrupt Counter 0", "Interrupt Counter"},
            new object[] { 28, "IPT_Counter_1", "Interrupt Counter 1", "Interrupt Counter"},
            new object[] { 29, "IPT_Counter_2", "Interrupt Counter 2", "Interrupt Counter"},
            new object[] { 30, "IPT_Counter_3", "Interrupt Counter 3", "Interrupt Counter"},
            new object[] { 31, "IPT_Software_0", "Interrupt Software 0", "Interrupt Software"},
            new object[] { 32, "IPT_Software_1", "Interrupt Software 1", "Interrupt Software"},
            new object[] { 33, "IPT_Software_2", "Interrupt Software 2", "Interrupt Software"},
            new object[] { 34, "IPT_Software_3", "Interrupt Software 3", "Interrupt Software"},
            new object[] { 35, "IPT_Software_4", "Interrupt Software 4", "Interrupt Software"},
            new object[] { 36, "IPT_Software_5", "Interrupt Software 5", "Interrupt Software"},
            new object[] { 37, "IPT_Software_6", "Interrupt Software 6", "Interrupt Software"},
            new object[] { 38, "IPT_Software_7", "Interrupt Software 7", "Interrupt Software"},
            new object[] { 39, "IPT_Software_8", "Interrupt Software 8", "Interrupt Software"},
            new object[] { 40, "IPT_Software_9", "Interrupt Software 9", "Interrupt Software"}
        };

        // Routines Types Translations
        //      Type, Word, Signed Input, Signed Output
        public static object[] RoutinesTypes = {
            new object[] { "TABLE", false, false, false, "UTabLU", "Table Reader", "Address Reg. %1%, Colums Reg. %2%, Rows Reg. %3%, Columns Num. Reg. %4%, Output Reg. %5%\r\nUnsigned Input, Unsigned Output Byte Table Routine"},
            new object[] { "TABLE", true, false, false, "UTabWLU", "Word Table Reader", "Address Reg. %1%, Colums Reg. %2%, Rows Reg. %3%, Columns Num. Reg. %4%, Output Reg. %5%\r\nUnsigned Input, Unsigned Output Word Table Routine"},
            new object[] { "TABLE", false, true, false, "STabLU", "Table Reader Signed", "Address Reg. %1%, Colums Reg. %2%, Rows Reg. %3%, Columns Num. Reg. %4%, Output Reg. %5%\r\nSigned Input, Unsigned Output Byte Table Routine"},
            new object[] { "TABLE", true, true, false, "STabWLU", "Word Table Reader Signed", "Address Reg. %1%, Colums Reg. %2%, Rows Reg. %3%, Columns Num. Reg. %4%, Output Reg. %5%\r\nSigned Input, Unsigned Output Word Table Routine"},
            new object[] { "TABLE", false, false, true, "STabLU", "Table Reader Signed", "Address Reg. %1%, Colums Reg. %2%, Rows Reg. %3%, Columns Num. Reg. %4%, Output Reg. %5%\r\nUnsigned Input, Signed Output Byte Table Routine"},
            new object[] { "TABLE", true, false, true, "STabWLU", "Word Table Reader Signed", "Address Reg. %1%, Colums Reg. %2%, Rows Reg. %3%, Columns Num. Reg. %4%, Output Reg. %5%\r\nUnsigned Input, Signed Output Word Table Routine"},
            new object[] { "TABLE", false, true, true, "STabLU", "Table Reader Signed", "Address Reg. %1%, Colums Reg. %2%, Rows Reg. %3%, Columns Num. Reg. %4%, Output Reg. %5%\r\nSigned Input, Signed Output Byte Table Routine"},
            new object[] { "TABLE", true, true, true, "STabWLU", "Word Table Reader Signed", "Address Reg. %1%, Colums Reg. %2%, Rows Reg. %3%, Columns Num. Reg. %4%, Output Reg. %5%\r\nSigned Input, Signed Output Word Table Routine"},
            new object[] { "FUNCTION", false, false, false, "UUByteLU", "Byte Function Reader", "Address Reg. %1%, Input Reg. %2%, Output Reg. %3%\r\nUnsigned Input, Unsigned Output Byte Function Routine"},
            new object[] { "FUNCTION", true, false, false, "UUWordLU", "Word Function Reader", "Address Reg. %1%, Input Reg. %2%, Output Reg. %3%\r\nUnsigned Input, Unsigned Output Word Function Routine"},
            new object[] { "FUNCTION", false, true, false, "SUByteLU", "Byte Function Reader SI", "Address Reg. %1%, Input Reg. %2%, Output Reg. %3%\r\nSigned Input, Unsigned Output Byte Function Routine"},
            new object[] { "FUNCTION", true, true, false, "SUWordLU", "Word Function Reader SI", "Address Reg. %1%, Input Reg. %2%, Output Reg. %3%\r\nSigned Input, Unsigned Output Word Function Routine"},
            new object[] { "FUNCTION", false, false, true, "USByteLU", "Byte Function Reader SO", "Address Reg. %1%, Input Reg. %2%, Output Reg. %3%\r\nUnsigned Input, Signed Output Byte Function Routine"},
            new object[] { "FUNCTION", true, false, true, "USWordLU", "Word Function Reader SO", "Address Reg. %1%, Input Reg. %2%, Output Reg. %3%\r\nUnsigned Input, Signed Output Word Function Routine"},
            new object[] { "FUNCTION", false, true, true, "SSByteLU", "Byte Function Reader SI SO", "Address Reg. %1%, Input Reg. %2%, Output Reg. %3%\r\nSigned Input, Signed Output Byte Function Routine"},
            new object[] { "FUNCTION", true, true, true, "SSWordLU", "Word Function Reader SI SO", "Address Reg. %1%, Input Reg. %2%, Output Reg. %3%\r\nSigned Input, Signed Output Word Function Routine"}
        };

        // Routines Codes Translations
        public static object[] RoutinesCodes = {
            new object[] { RoutineCode.Checksum, "CalCheck", "Calibration Starting Checks", "Calibration Starting Checks routine, used to check Calibration is Valid, Checksum calculation at least."},
            new object[] { RoutineCode.Init, "CalInit", "Calibration Init", "Calibration Init routine, used to define main information on the Calibration."},
            new object[] { RoutineCode.CoreInit, "CoreCalInit", "Core Calibration Init", "Core Calibration Init routine, used to define main information on the Calibration."},
            new object[] { RoutineCode.TableCore, "TabCore", "Table Reader Core", "Core for all table reader variants."}
        };
        
        // OP Codes
        public const string OPCSigndAltOpCode = "fe";
        public const string OPCSigndAltInstructionPrefix = "s";
        public const string OPCSigndAltTranslationAdder = "(sig)";

        public static object[] OPCJCTranslations = {
            new string[] {"DEFAULT", "(uns) %P2% >= %P1%"},
            new string[] {"CY", "CY == 1"},
            new string[] {"CMP", "(uns) %P2% >= %P1%"},
            new string[] {"+W", "%P2% + %P1% > ffff"},
            new string[] {"+B", "%P2% + %P1% > ff"},
            new string[] {"-", "(uns) %P2% < %P1%"},
            new string[] {"*W", "%P2% * %P1% > ffff"},
            new string[] {"*B", "%P2% * %P1% > ff"}
        };

        public static object[] OPCJNCTranslations = {
            new string[] {"DEFAULT", "(uns) %P2% < %P1%"},
            new string[] {"CY", "CY == 0"},
            new string[] {"CMP", "(uns) %P2% < %P1%"},
            new string[] {"+W", "%P2% + %P1% <= ffff"},
            new string[] {"+B", "%P2% + %P1% <= ff"},
            new string[] {"-", "(uns) %P2% >= %P1%"},
            new string[] {"*W", "%P2% * %P1% <= ffff"},
            new string[] {"*B", "%P2% * %P1% <= ff"}
        };

        public static object[] OPCodes = {
            new object[] { 0x0, new object[] {"WOP"},
                new object[] {
                    new object[] {0x0, new object[] {"SKIP", 0, "AR", "%JA%", "goto %JA%;", "skip - 2 byte no operation"}},
                    new object[] {0x1, new object[] {"CLRW", 1, "RW", "%1%", "%1% = 0;", "clear word"}},
                    new object[] {0x2, new object[] {"CPLW", 1, "RW", "%1%", "%1% = ~%1%;", "complement word"}},
                    new object[] {0x3, new object[] {"NEGW", 1, "RW", "%1%", "%1% = -%1%;", "negate integer"}},
                    new object[] {0x4, new object[] {}},
                    new object[] {0x5, new object[] {"DECW", 1, "RW", "%1%", "%1%--;", "decrement word"}},
                    new object[] {0x6, new object[] {"SEXW", 1, "RW", "%1%", "%1%L = (long)%1%;", "sign extend int to long"}},
                    new object[] {0x7, new object[] {"INCW", 1, "RW", "%1%", "%1%++;", "increment word"}},
                    new object[] {0x8, new object[] {"SHRW", 2, "VB", "RW", "%2%,%1%", "%2% = %2% / %2^%;", "logical right shift word"}},
                    new object[] {0x9, new object[] {"SHLW", 2, "VB", "RW", "%2%,%1%", "%2% = %2% * %2^%;", "shift word left"}},
                    new object[] {0xA, new object[] {"ASRW", 2, "VB", "RW", "%2%,%1%", "%2% = %2% / %2^%;", "arithmetic right shift word"}},
                    new object[] {0xB, new object[] {}},
                    new object[] {0xC, new object[] {"SHRDW", 2, "VB", "RW", "%2%,%1%", "%2%L = %2%L / %2^%;", "logical right shift double word"}},
                    new object[] {0xD, new object[] {"SHLDW", 2, "VB", "RW", "%2%,%1%", "%2%L = %2%L * %2^%;", "shift double word left"}},
                    new object[] {0xE, new object[] {"ASRDW", 2, "VB", "RW", "%2%,%1%", "%2%L = %2%L / %2^%;", "arithmetic right shift double word"}},
                    new object[] {0xF, new object[] {"NORM", 2, "RW", "RW", "%2%,%1%", "%2%L = %2%L Norm %1%L;", "normalize long integer, shift left bits of double word"}}
                }
            },
            new object[] { 0x1, new object[] {"BOP"},
                new object[] {
                    new object[] {0x0, new object[] {"RBNK", 1, "BN", "%1%", "Set Bank %1%;", "Rombank"}},
                    new object[] {0x1, new object[] {"CLRB", 1, "RB", "%1%", "%1% = 0;", "clear byte"}},
                    new object[] {0x2, new object[] {"CPLB", 1, "RB", "%1%", "%1% = ~%1%;", "complement byte"}},
                    new object[] {0x3, new object[] {"NEGB", 1, "RB", "%1%", "%1% = -%1%;", "negate byte"}},
                    new object[] {0x4, new object[] {}},
                    new object[] {0x5, new object[] {"DECB", 1, "RB", "%1%", "%1%--;", "decrement byte"}},
                    new object[] {0x6, new object[] {"SEXB", 1, "RB", "%1%", "%1%W = (int)%1%;", "sign extend 8-bit in to 16-bit int"}},
                    new object[] {0x7, new object[] {"INCB", 1, "RB", "%1%", "%1%++;", "increment byte"}},
                    new object[] {0x8, new object[] {"SHRB", 2, "VB", "RB", "%2%,%1%", "%2% = %2% / %2^%;", "logical right shift byte"}},
                    new object[] {0x9, new object[] {"SHLB", 2, "VB", "RB", "%2%,%1%", "%2% = %2% * %2^%;", "shift byte left"}},
                    new object[] {0xA, new object[] {"ASRB", 2, "VB", "RB", "%2%,%1%", "%2% = %2% / %2^%;", "arithmetic right shift byte"}},
                    new object[] {0xB, new object[] {}},
                    new object[] {0xC, new object[] {}},
                    new object[] {0xD, new object[] {}},
                    new object[] {0xE, new object[] {}},
                    new object[] {0xF, new object[] {}}
                }
            },
            new object[] { 0x2, new object[] {"SJO"},
                new object[] {
                    new object[] {0x0, new object[] {"SJMP", 1, "AR", "%1%", "goto %1%;", "Short jump + (uns)AR"}},
                    new object[] {0x1, new object[] {"SJMP", 1, "AR", "%1%", "goto %1%;", "Short jump + (uns)AR + 100"}},
                    new object[] {0x2, new object[] {"SJMP", 1, "AR", "%1%", "goto %1%;", "Short jump + (uns)AR + 200"}},
                    new object[] {0x3, new object[] {"SJMP", 1, "AR", "%1%", "goto %1%;", "Short jump + (uns)AR + 300"}},
                    new object[] {0x4, new object[] {"SJMP", 1, "AR", "%1%", "goto %1%;", "Short jump + (uns)AR - 400"}},
                    new object[] {0x5, new object[] {"SJMP", 1, "AR", "%1%", "goto %1%;", "Short jump + (uns)AR - 300"}},
                    new object[] {0x6, new object[] {"SJMP", 1, "AR", "%1%", "goto %1%;", "Short jump + (uns)AR - 200"}},
                    new object[] {0x7, new object[] {"SJMP", 1, "AR", "%1%", "goto %1%;", "Short jump + (uns)AR - 100"}},
                    new object[] {0x8, new object[] {"SCALL", 1, "AR", "%1%", "%1%(%ARGS%);", "Short call + (uns)AR"}},
                    new object[] {0x9, new object[] {"SCALL", 1, "AR", "%1%", "%1%(%ARGS%);", "Short call + (uns)AR + 100"}},
                    new object[] {0xA, new object[] {"SCALL", 1, "AR", "%1%", "%1%(%ARGS%);", "Short call + (uns)AR + 200"}},
                    new object[] {0xB, new object[] {"SCALL", 1, "AR", "%1%", "%1%(%ARGS%);", "Short call + (uns)AR + 300"}},
                    new object[] {0xC, new object[] {"SCALL", 1, "AR", "%1%", "%1%(%ARGS%);", "Short call + (uns)AR - 400"}},
                    new object[] {0xD, new object[] {"SCALL", 1, "AR", "%1%", "%1%(%ARGS%);", "Short call + (uns)AR - 300"}},
                    new object[] {0xE, new object[] {"SCALL", 1, "AR", "%1%", "%1%(%ARGS%);", "Short call + (uns)AR - 200"}},
                    new object[] {0xF, new object[] {"SCALL", 1, "AR", "%1%", "%1%(%ARGS%);", "Short call + (uns)AR - 100"}},
                }
            },
            new object[] { 0x3, new object[] {"BGO"},
                new object[] {
                    new object[] {0x0, new object[] {"JNB", 2, "RB", "AR", "B0,%1%,%2%", "if (!B0_%1%) goto %2%;", "Signed jump if bit clear"}},
                    new object[] {0x1, new object[] {"JNB", 2, "RB", "AR", "B1,%1%,%2%", "if (!B1_%1%) goto %2%;", "Signed jump if bit clear"}},
                    new object[] {0x2, new object[] {"JNB", 2, "RB", "AR", "B2,%1%,%2%", "if (!B2_%1%) goto %2%;", "Signed jump if bit clear"}},
                    new object[] {0x3, new object[] {"JNB", 2, "RB", "AR", "B3,%1%,%2%", "if (!B3_%1%) goto %2%;", "Signed jump if bit clear"}},
                    new object[] {0x4, new object[] {"JNB", 2, "RB", "AR", "B4,%1%,%2%", "if (!B4_%1%) goto %2%;", "Signed jump if bit clear"}},
                    new object[] {0x5, new object[] {"JNB", 2, "RB", "AR", "B5,%1%,%2%", "if (!B5_%1%) goto %2%;", "Signed jump if bit clear"}},
                    new object[] {0x6, new object[] {"JNB", 2, "RB", "AR", "B6,%1%,%2%", "if (!B6_%1%) goto %2%;", "Signed jump if bit clear"}},
                    new object[] {0x7, new object[] {"JNB", 2, "RB", "AR", "B7,%1%,%2%", "if (!B7_%1%) goto %2%;", "Signed jump if bit clear"}},
                    new object[] {0x8, new object[] {"JB", 2, "RB", "AR", "B0,%1%,%2%", "if (B0_%1%) goto %2%;", "Signed jump if bit set"}},
                    new object[] {0x9, new object[] {"JB", 2, "RB", "AR", "B1,%1%,%2%", "if (B1_%1%) goto %2%;", "Signed jump if bit set"}},
                    new object[] {0xA, new object[] {"JB", 2, "RB", "AR", "B2,%1%,%2%", "if (B2_%1%) goto %2%;", "Signed jump if bit set"}},
                    new object[] {0xB, new object[] {"JB", 2, "RB", "AR", "B3,%1%,%2%", "if (B3_%1%) goto %2%;", "Signed jump if bit set"}},
                    new object[] {0xC, new object[] {"JB", 2, "RB", "AR", "B4,%1%,%2%", "if (B4_%1%) goto %2%;", "Signed jump if bit set"}},
                    new object[] {0xD, new object[] {"JB", 2, "RB", "AR", "B5,%1%,%2%", "if (B5_%1%) goto %2%;", "Signed jump if bit set"}},
                    new object[] {0xE, new object[] {"JB", 2, "RB", "AR", "B6,%1%,%2%", "if (B6_%1%) goto %2%;", "Signed jump if bit set"}},
                    new object[] {0xF, new object[] {"JB", 2, "RB", "AR", "B7,%1%,%2%", "if (B7_%1%) goto %2%;", "Signed jump if bit set"}}
                }
            },
            new object[] { 0x4, new object[] {"WOP"},
                new object[] {
                    new object[] {0x0, new object[] {"AN3W", 3, "RW", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% & %1%;", "logical and words (3 operands)"}}, // 40,34,16,00       an3w  0,R16,R34      0 = R16 & R34;
                    new object[] {0x1, new object[] {"AN3W", 4, "WB", "WB", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% & %1%;", "logical and words (3 operands)"}}, // 41,03,00,34,30    an3w  R30,R34,3      R30 = R34 & 3;
                    new object[] {0x2, new object[] {"AN3W", 3, "RWP", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% & %1%;", "logical and words (3 operands)"}}, // ???????????????????????????
                    new object[] {0x3, new object[] {"AN3W", 45, "WBP", "WBP", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% & %1%;", "logical and words (3 operands)"}}, // 43,7c,6a,32,30    an3w  R30,R32,[R7c+6a] R30 = R32 & [246a];
                    new object[] {0x4, new object[] {"AD3W", 3, "RW", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% + %1%;", "add words (3 operands)"}}, // 44,38,a2,32       ad3w  R32,Ra2,R38    R32 = Ra2 + R38;
                    new object[] {0x5, new object[] {"AD3W", 4, "WB", "WB", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% + %1%;", "add words (3 operands)"}}, // 45,38,02,5a,5e    ad3w  R5e,R5a,238    R5e = R5a + 238;
                    new object[] {0x6, new object[] {"AD3W", 3, "RWP", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% + %1%;", "add words (3 operands)"}}, // 46,5e,58,54       ad3w  R54,R58,[R5e]  R54 = R58 + [R5e];
                    new object[] {0x7, new object[] {"AD3W", 45, "WBP", "WBP", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% + %1%;", "add words (3 operands)"}}, // 47,01,4e,01,3e,3a ad3w  R3a,R3e,[14e]  R3a = R3e + [14e]; !!!! 47,88,14,00,30    ad3w  R30,0,[R88+14] R30 = [3a6a];
                    new object[] {0x8, new object[] {"SB3W", 3, "RW", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% - %1%;", "subtract words (3 operands)"}}, // 48,36,3a,36       sb3w  R36,R3a,R36    R36 = R3a - R36;
                    new object[] {0x9, new object[] {"SB3W", 4, "WB", "WB", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% - %1%;", "subtract words (3 operands)"}}, // 49,be,8a,3a,38    sb3w  R38,R3a,8abe   R38 = R3a - 8abe;
                    new object[] {0xA, new object[] {"SB3W", 3, "RWP", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% - %1%;", "subtract words (3 operands)"}}, // 4a,30,00,38       sb3w  R38,0,[R30]    R38 = 0 - [R30];
                    new object[] {0xB, new object[] {"SB3W", 45, "WBP", "WBP", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% - %1%;", "subtract words (3 operands)"}}, // 4b,01,f4,02,3c,30 sb3w  R30,R3c,[2f4]  R30 = R3c - [2f4];
                    new object[] {0xC, new object[] {"ML3W", 3, "RW", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% * %1%;", "multiply unsigned words (3 operands)"}}, //4c,4a,38,40       ml3w  R40,R38,R4a    R40L = R38 * R4a;
                    new object[] {0xD, new object[] {"ML3W", 4, "WB", "WB", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% * %1%;", "multiply unsigned words (3 operands)"}}, //4d,41,00,3c,30    ml3w  R30,R3c,41     R30L = R3c * 41;
                    new object[] {0xE, new object[] {"ML3W", 3, "RWP", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% * %1%;", "multiply unsigned words (3 operands)"}}, //4e,33,34,38       ml3w  R38,R34,[R32++] R38L = R34 * [R32++];
                    new object[] {0xF, new object[] {"ML3W", 45, "WBP", "WBP", "RW", "RW", "%3%,%2%,%1%", "%3% = %2% * %1%;", "multiply unsigned words (3 operands)"}} //4f,7c,40,38,30    ml3w  R30,R38,[R7c+40] R30L = R38 * [2440];
                }
            },
            new object[] { 0x5, new object[] {"BOP"},
                new object[] {
                    new object[] {0x0, new object[] {"AN3B", 3, "RB", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% & %1%;", "logical and bytes (3 operands)"}}, //50,34,d6,00       an3b  0,Rd6,R34      0 = Rd6 & R34;
                    new object[] {0x1, new object[] {"AN3B", 3, "VB", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% & %1%;", "logical and bytes (3 operands)"}}, //51,1f,1a,df       an3b  Rdf,R1a,1f     Rdf = R1a & 1f;
                    new object[] {0x2, new object[] {"AN3B", 3, "RBP", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% & %1%;", "logical and bytes (3 operands)"}}, //52,4c,32,48       an3b  R48,R32,[R4c]  R48 = R32 & [R4c];
                    new object[] {0x3, new object[] {"AN3B", 45, "WBP", "WBP", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% & %1%;", "logical and bytes (3 operands)"}}, //53,5b,96,47,c9,00 an3b  0,Rc9,[R5a+4796] 0 = Rc9 & [R5a+4796];
                    new object[] {0x4, new object[] {"AD3B", 3, "RB", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% + %1%;", "add bytes (3 operands)"}}, // 54,5e,5e,58       ad3b  R58,R5e,R5e    R58 = R5e + R5e;
                    new object[] {0x5, new object[] {"AD3B", 3, "VB", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% + %1%;", "add bytes (3 operands)"}}, // 55,02,45,40       ad3b  R40,R45,2      R40 = R45 + 2;
                    new object[] {0x6, new object[] {"AD3B", 3, "RBP", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% + %1%;", "add bytes (3 operands)"}}, // 56,35,32,33       ad3b  R33,R32,[R34++] R33 = R32 + [R34++];
                    new object[] {0x7, new object[] {"AD3B", 45, "WBP", "WBP", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% + %1%;", "add bytes (3 operands)"}}, // 57,33,98,ee,30,34 ad3b  R34,R30,[R32+ee98] R34 = R30 + [R32+ee98];
                    new object[] {0x8, new object[] {"SB3B", 3, "RB", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% - %1%;", "subtract bytes (3 operands)"}}, // 58,cf,54,cf       sb3b  Rcf,R54,Rcf    Rcf = R54 - Rcf;
                    new object[] {0x9, new object[] {"SB3B", 3, "VB", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% - %1%;", "subtract bytes (3 operands)"}}, // 59,01,cf,54       sb3b  R54,Rcf,1      R54 = Rcf - 1;
                    new object[] {0xA, new object[] {"SB3B", 3, "RBP", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% - %1%;", "subtract bytes (3 operands)"}}, // ??????????????????????????
                    new object[] {0xB, new object[] {"SB3B", 45, "WBP", "WBP", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% - %1%;", "subtract bytes (3 operands)"}}, // 5b,01,f0,0d,44,45 sb3b  R45,R44,[df0]  R45 = R44 - [df0]; !!!!! 5b,de,d7,2a,34    sb3b  R34,R2a,[Rde+d7] R34 = R2a - [Rde+d7];
                    new object[] {0xC, new object[] {"ML3B", 3, "RB", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% * %1%;", "multiply unsigned bytes (3 operands)"}},
                    new object[] {0xD, new object[] {"ML3B", 3, "VB", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% * %1%;", "multiply unsigned bytes (3 operands)"}},
                    new object[] {0xE, new object[] {"ML3B", 3, "RBP", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% * %1%;", "multiply unsigned bytes (3 operands)"}},
                    new object[] {0xF, new object[] {"ML3B", 45, "WBP", "WBP", "RB", "RB", "%3%,%2%,%1%", "%3% = %2% * %1%;", "multiply unsigned bytes (3 operands)"}}
                }
            },
            new object[] { 0x6, new object[] {"WOP"},
                new object[] {
                    new object[] {0x0, new object[] {"AN2W", 2, "RW", "RW", "%2%,%1%", "%2% &= %1%;", "logical and words (2 operands)"}},
                    new object[] {0x1, new object[] {"AN2W", 3, "WB", "WB", "RW", "%2%,%1%", "%2% &= %1%;", "logical and words (2 operands)"}}, // 61,ff,7f,30       an2w  R30,7fff       R30 &= 7fff;
                    new object[] {0x2, new object[] {"AN2W", 2, "RWP", "RW", "%2%,%1%", "%2% &= %1%;", "logical and words (2 operands)"}}, // ?????????????????????????????
                    new object[] {0x3, new object[] {"AN2W", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% &= %1%;", "logical and words (2 operands)"}}, // ?????????????????????????????
                    new object[] {0x4, new object[] {"AD2W", 2, "RW", "RW", "%2%,%1%", "%2% += %1%;", "add words (2 operands)"}},
                    new object[] {0x5, new object[] {"AD2W", 3, "WB", "WB", "RW", "%2%,%1%", "%2% += %1%;", "add words (2 operands)"}},
                    new object[] {0x6, new object[] {"AD2W", 2, "RWP", "RW", "%2%,%1%", "%2% += %1%;", "add words (2 operands)"}}, // 66,4c,34          ad2w  R34,[R4c]      R34 += [R4c];
                    new object[] {0x7, new object[] {"AD2W", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% += %1%;", "add words (2 operands)"}}, // 67,33,f0,00,34    ad2w  R34,[R32+f0]   R34 += [R32+f0];
                    new object[] {0x8, new object[] {"SB2W", 2, "RW", "RW", "%2%,%1%", "%2% -= %1%;", "subtract words (2 operands)"}},
                    new object[] {0x9, new object[] {"SB2W", 3, "WB", "WB", "RW", "%2%,%1%", "%2% -= %1%;", "subtract words (2 operands)"}},
                    new object[] {0xA, new object[] {"SB2W", 2, "RWP", "RW", "%2%,%1%", "%2% -= %1%;", "subtract words (2 operands)"}},
                    new object[] {0xB, new object[] {"SB2W", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% -= %1%;", "subtract words (2 operands)"}},
                    new object[] {0xC, new object[] {"ML2W", 2, "RW", "RW", "%2%,%1%", "%2% *= %1%;", "multiply unsigned words (2 operands)"}},
                    new object[] {0xD, new object[] {"ML2W", 3, "WB", "WB", "RW", "%2%,%1%", "%2% *= %1%;", "multiply unsigned words (2 operands)"}},
                    new object[] {0xE, new object[] {"ML2W", 2, "RWP", "RW", "%2%,%1%", "%2% *= %1%;", "multiply unsigned words (2 operands)"}},
                    new object[] {0xF, new object[] {"ML2W", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% *= %1%;", "multiply unsigned words (2 operands)"}}
                }
            },
            new object[] { 0x7, new object[] {"BOP"},
                new object[] {
                    new object[] {0x0, new object[] {"AN2B", 2, "RB", "RB", "%2%,%1%", "%2% &= %1%;", "logical and bytes (2 operands)"}},
                    new object[] {0x1, new object[] {"AN2B", 2, "VB", "RB", "%2%,%1%", "%2% &= %1%;", "logical and bytes (2 operands)"}},
                    new object[] {0x2, new object[] {"AN2B", 2, "RBP", "RB", "%2%,%1%", "%2% &= %1%;", "logical and bytes (2 operands)"}},
                    new object[] {0x3, new object[] {"AN2B", 34, "WBP", "WBP", "RB", "%2%,%1%", "%2% &= %1%;", "logical and bytes (2 operands)"}}, // 73,55,27,63,cb    an2b  Rcb,[R54+6327] Rcb &= [R54+6327];
                    new object[] {0x4, new object[] {"AD2B", 2, "RB", "RB", "%2%,%1%", "%2% += %1%;", "add bytes (2 operands)"}},
                    new object[] {0x5, new object[] {"AD2B", 2, "VB", "RB", "%2%,%1%", "%2% += %1%;", "add bytes (2 operands)"}},
                    new object[] {0x6, new object[] {"AD2B", 2, "RBP", "RB", "%2%,%1%", "%2% += %1%;", "add bytes (2 operands)"}},
                    new object[] {0x7, new object[] {"AD2B", 34, "WBP", "WBP", "RB", "%2%,%1%", "%2% += %1%;", "add bytes (2 operands)"}},
                    new object[] {0x8, new object[] {"SB2B", 2, "RB", "RB", "%2%,%1%", "%2% -= %1%;", "subtract bytes (2 operands)"}},
                    new object[] {0x9, new object[] {"SB2B", 2, "VB", "RB", "%2%,%1%", "%2% -= %1%;", "subtract bytes (2 operands)"}},
                    new object[] {0xA, new object[] {"SB2B", 2, "RBP", "RB", "%2%,%1%", "%2% -= %1%;", "subtract bytes (2 operands)"}},
                    new object[] {0xB, new object[] {"SB2B", 34, "WBP", "WBP", "RB", "%2%,%1%", "%2% -= %1%;", "subtract bytes (2 operands)"}},
                    new object[] {0xC, new object[] {"ML2B", 2, "RB", "RB", "%2%,%1%", "%2% *= %1%;", "multiply unsigned bytes (2 operands)"}},
                    new object[] {0xD, new object[] {"ML2B", 2, "VB", "RB", "%2%,%1%", "%2% *= %1%;", "multiply unsigned bytes (2 operands)"}},
                    new object[] {0xE, new object[] {"ML2B", 2, "RBP", "RB", "%2%,%1%", "%2% *= %1%;", "multiply unsigned bytes (2 operands)"}},
                    new object[] {0xF, new object[] {"ML2B", 34, "WBP", "WBP", "RB", "%2%,%1%", "%2% *= %1%;", "multiply unsigned bytes (2 operands)"}}
                }
            },
            new object[] { 0x8, new object[] {"WOP"},
                new object[] {
                    new object[] {0x0, new object[] {"ORRW", 2, "RW", "RW", "%2%,%1%", "%2% |= %1%;", "logical or words"}},
                    new object[] {0x1, new object[] {"ORRW", 3, "WB", "WB", "RW", "%2%,%1%", "%2% |= %1%;", "logical or words"}},
                    new object[] {0x2, new object[] {"ORRW", 2, "RWP", "RW", "%2%,%1%", "%2% |= %1%;", "logical or words"}},
                    new object[] {0x3, new object[] {"ORRW", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% |= %1%;", "logical or words"}},
                    new object[] {0x4, new object[] {"XRW", 2, "RW", "RW", "%2%,%1%", "%2% ^= %1%;", "logical exclusive or words"}},
                    new object[] {0x5, new object[] {"XRW", 3, "WB", "WB", "RW", "%2%,%1%", "%2% ^= %1%;", "logical exclusive or words"}},
                    new object[] {0x6, new object[] {"XRW", 2, "RWP", "RW", "%2%,%1%", "%2% ^= %1%;", "logical exclusive or words"}},
                    new object[] {0x7, new object[] {"XRW", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% ^= %1%;", "logical exclusive or words"}},
                    new object[] {0x8, new object[] {"CMPW", 2, "RW", "RW", "%2%,%1%", "", "compare words"}},
                    new object[] {0x9, new object[] {"CMPW", 3, "WB", "WB", "RW", "%2%,%1%", "", "compare words"}},
                    new object[] {0xA, new object[] {"CMPW", 2, "RWP", "RW", "%2%,%1%", "", "compare words"}},
                    new object[] {0xB, new object[] {"CMPW", 34, "WBP", "WBP", "RW", "%2%,%1%", "", "compare words"}},
                    new object[] {0xC, new object[] {"DIVW", 2, "RW", "RW", "%2%,%1%", "%2% = %2%L / %1%;", "divide unsigned words"}},
                    new object[] {0xD, new object[] {"DIVW", 3, "WB", "WB", "RW", "%2%,%1%", "%2% = %2%L / %1%;", "divide unsigned words"}},
                    new object[] {0xE, new object[] {"DIVW", 2, "RWP", "RW", "%2%,%1%", "%2% = %2%L / %1%;", "divide unsigned words"}},
                    new object[] {0xF, new object[] {"DIVW", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% = %2%L / %1%;", "divide unsigned words"}}
                }
            },
            new object[] { 0x9, new object[] {"BOP"},
                new object[] {
                    new object[] {0x0, new object[] {"ORRB", 2, "RB", "RB", "%2%,%1%", "%2% |= %1%;", "logical or bytes"}},
                    new object[] {0x1, new object[] {"ORRB", 2, "VB", "RB", "%2%,%1%", "%2% |= %1%;", "logical or bytes"}},
                    new object[] {0x2, new object[] {"ORRB", 2, "RBP", "RB", "%2%,%1%", "%2% |= %1%;", "logical or bytes"}},
                    new object[] {0x3, new object[] {"ORRB", 34, "WBP", "WBP", "RB", "%2%,%1%", "%2% |= %1%;", "logical or bytes"}},
                    new object[] {0x4, new object[] {"XRB", 2, "RB", "RB", "%2%,%1%", "%2% ^= %1%;", "logical exclusive or bytes"}},
                    new object[] {0x5, new object[] {"XRB", 2, "VB", "RB", "%2%,%1%", "%2% ^= %1%;", "logical exclusive or bytes"}},
                    new object[] {0x6, new object[] {"XRB", 2, "RBP", "RB", "%2%,%1%", "%2% ^= %1%;", "logical exclusive or bytes"}},
                    new object[] {0x7, new object[] {"XRB", 34, "WBP", "WBP", "RB", "%2%,%1%", "%2% ^= %1%;", "logical exclusive or bytes"}},
                    new object[] {0x8, new object[] {"CMPB", 2, "RB", "RB", "%2%,%1%", "", "compare bytes"}},
                    new object[] {0x9, new object[] {"CMPB", 2, "VB", "RB", "%2%,%1%", "", "compare bytes"}},
                    new object[] {0xA, new object[] {"CMPB", 2, "RBP", "RB", "%2%,%1%", "", "compare bytes"}},
                    new object[] {0xB, new object[] {"CMPB", 34, "WBP", "WBP", "RB", "%2%,%1%", "", "compare bytes"}},
                    new object[] {0xC, new object[] {"DIVB", 2, "RB", "RB", "%2%,%1%", "%2% = %2%W / %1%;", "divide unsigned bytes"}},
                    new object[] {0xD, new object[] {"DIVB", 2, "VB", "RB", "%2%,%1%", "%2% = %2%W / %1%;", "divide unsigned bytes"}},
                    new object[] {0xE, new object[] {"DIVB", 2, "RBP", "RB", "%2%,%1%", "%2% = %2%W / %1%;", "divide unsigned bytes"}},
                    new object[] {0xF, new object[] {"DIVB", 34, "WBP", "WBP", "RB", "%2%,%1%", "%2% = %2%W / %1%;", "divide unsigned bytes"}}
                }
            },
            new object[] { 0xA, new object[] {"WOP-4"},
                new object[] {
                    new object[] {0x0, new object[] {"LDW", 2, "RW", "RW", "%2%,%1%", "%2% = %1%;", "load word"}},
                    new object[] {0x1, new object[] {"LDW", 3, "WB", "WB", "RW", "%2%,%1%", "%2% = %1%;", "load word"}},
                    new object[] {0x2, new object[] {"LDW", 2, "RWP", "RW", "%2%,%1%", "%2% = %1%;", "load word"}},
                    new object[] {0x3, new object[] {"LDW", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% = %1%;", "load word"}},
                    new object[] {0x4, new object[] {"ADCW", 2, "RW", "RW", "%2%,%1%", "%2% += %1% + CY;", "add words with carry"}},
                    new object[] {0x5, new object[] {"ADCW", 3, "WB", "WB", "RW", "%2%,%1%", "%2% += %1% + CY;", "add words with carry"}},
                    new object[] {0x6, new object[] {"ADCW", 2, "RWP", "RW", "%2%,%1%", "%2% += %1% + CY;", "add words with carry"}},
                    new object[] {0x7, new object[] {"ADCW", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% +=%1% + CY;", "add words with carry"}},
                    new object[] {0x8, new object[] {"SBBW", 2, "RW", "RW", "%2%,%1%", "%2% -= %1% - CY;", "subtract words with borrow"}},
                    new object[] {0x9, new object[] {"SBBW", 3, "WB", "WB", "RW", "%2%,%1%", "%2% -= %1% - CY;", "subtract words with borrow"}},
                    new object[] {0xA, new object[] {"SBBW", 2, "RWP", "RW", "%2%,%1%", "%2% -= %1% - CY;", "subtract words with borrow"}},
                    new object[] {0xB, new object[] {"SBBW", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% -= %1% - CY;", "subtract words with borrow"}},
                    new object[] {0xC, new object[] {"LDZBW", 2, "RB", "RW", "%2%,%1%", "%2% = (uns)%1%;", "load word with byte, zero extended"}},
                    new object[] {0xD, new object[] {"LDZBW", 2, "VB", "RW", "%2%,%1%", "%2% = (uns)%1%;", "load word with byte, zero extended"}},
                    new object[] {0xE, new object[] {"LDZBW", 2, "RWP", "RW", "%2%,%1%", "%2% = (uns)%1%;", "load word with byte, zero extended"}},
                    new object[] {0xF, new object[] {"LDZBW", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% = (uns)%1%;", "load word with byte, zero extended"}}
                }
            },
            new object[] { 0xB, new object[] {"BOP-4"},
                new object[] {
                    new object[] {0x0, new object[] {"LDB", 2, "RB", "RB", "%2%,%1%", "%2% = %1%;", "load byte"}},
                    new object[] {0x1, new object[] {"LDB", 2, "VB", "RB", "%2%,%1%", "%2% = %1%;", "load byte"}},
                    new object[] {0x2, new object[] {"LDB", 2, "RBP", "RB", "%2%,%1%", "%2% = %1%;", "load byte"}},
                    new object[] {0x3, new object[] {"LDB", 34, "WBP", "WBP", "RB", "%2%,%1%", "%2% = %1%;", "load byte"}},
                    new object[] {0x4, new object[] {"ADCB", 2, "RB", "RB", "%2%,%1%", "%2% += %1% + CY;", "add bytes with carry"}},
                    new object[] {0x5, new object[] {"ADCB", 2, "VB", "RB", "%2%,%1%", "%2% += %1% + CY;", "add bytes with carry"}},
                    new object[] {0x6, new object[] {"ADCB", 2, "RBP", "RB", "%2%,%1%", "%2% += %1% + CY;", "add bytes with carry"}},
                    new object[] {0x7, new object[] {"ADCB", 34, "WBP", "WBP", "RB", "%2%,%1%", "%2% += %1% + CY;", "add bytes with carry"}},
                    new object[] {0x8, new object[] {"SBBB", 2, "RB", "RB", "%2%,%1%", "%2% -= %1% - CY;", "subtract bytes with borrow"}},
                    new object[] {0x9, new object[] {"SBBB", 2, "VB", "RB", "%2%,%1%", "%2% -= %1% - CY;", "subtract bytes with borrow"}},
                    new object[] {0xA, new object[] {"SBBB", 2, "RBP", "RB", "%2%,%1%", "%2% -= %1% - CY;", "subtract bytes with borrow"}},
                    new object[] {0xB, new object[] {"SBBB", 34, "WBP", "WBP", "RB", "%2%,%1%", "%2% -= %1% - CY;", "subtract bytes with borrow"}},
                    new object[] {0xC, new object[] {"LDSBW", 2, "RB", "RW", "%2%,%1%", "%2% = (sig)%1%;", "load integer with byte, sign extended"}},
                    new object[] {0xD, new object[] {"LDSBW", 2, "VB", "RW", "%2%,%1%", "%2% = (sig)%1%;", "load integer with byte, sign extended"}},
                    new object[] {0xE, new object[] {"LDSBW", 2, "RWP", "RW", "%2%,%1%", "%2% = (sig)%1%;", "load integer with byte, sign extended"}},
                    new object[] {0xF, new object[] {"LDSBW", 34, "WBP", "WBP", "RW", "%2%,%1%", "%2% = (sig)%1%;", "load integer with byte, sign extended"}}
                }
            },
            new object[] { 0xC, new object[] {"MOP"},
                new object[] {
                    new object[] {0x0, new object[] {"STW", 2, "RW", "RW", "%1%,%2%", "%1% = %2%;", "store word"}},
                    new object[] {0x1, new object[] {}},
                    new object[] {0x2, new object[] {"STW", 2, "RWP", "RW", "%1%,%2%", "%1% = %2%;", "store word"}},
                    new object[] {0x3, new object[] {"STW", 34, "WBP", "WBP", "RW", "%1%,%2%", "%1% = %2%;", "store word"}},
                    new object[] {0x4, new object[] {"STB", 2, "RB", "RB", "%1%,%2%", "%1% = %2%;", "store byte"}},
                    new object[] {0x5, new object[] {}},
                    new object[] {0x6, new object[] {"STB", 2, "RBP", "RB", "%1%,%2%", "%1% = %2%;", "store byte"}},
                    new object[] {0x7, new object[] {"STB", 34, "WBP", "WBP", "RB", "%1%,%2%", "%1% = %2%;", "store byte"}},
                    new object[] {0x8, new object[] {"PUSH", 1, "RW", "%1%", "push(%1%);", "push word"}},
                    new object[] {0x9, new object[] {"PUSH", 2, "AA", "AA", "%1%", "push(%1%);", "push word"}},
                    new object[] {0xA, new object[] {"PUSH", 1, "RWP", "%1%", "push(%1%);", "push word"}},
                    new object[] {0xB, new object[] {"PUSH", 23, "WBP", "WBP", "%1%", "push(%1%);", "push word"}},
                    new object[] {0xC, new object[] {"POP", 1, "RW", "%1%", "%1% = pop();", "pop word"}},
                    new object[] {0xD, new object[] {}},
                    new object[] {0xE, new object[] {"POP", 1, "RWP", "%1%", "%1% = pop();", "pop word"}}, // ???????????????????????????????
                    new object[] {0xF, new object[] {"POP", 23, "WBP", "WBP", "%1%", "%1% = pop();", "pop word"}}
                }
            },
            new object[] { 0xD, new object[] {"GOP"},
                new object[] {
                    new object[] {0x0, new object[] {"JNST", 1, "AR", "%1%", "if (!STICKY) goto %1%;", "jump if sticky bit is clear"}},
                    new object[] {0x1, new object[] {"JLEU", 1, "AR", "%1%", "if ((uns) %P2% <= %P1%) goto %1%;", "jump if unsigned not higher"}},
                    new object[] {0x2, new object[] {"JGT", 1, "AR", "%1%", "if ((sig) %P2% > %P1%) goto %1%;", "jump if signed greater than"}},
                    //new object[] {0x3, new object[] {"JNC", 1, "AR", "%1%", "if (CY == 0) goto %1%;", "jump if carry flag is clear"}},
                    //new object[] {0x3, new object[] {"JNC", 1, "AR", "%1%", "if ((uns) %P2% < %P1%) goto %1%;", "jump if carry flag is clear"}},
                    new object[] {0x3, new object[] {"JNC", 1, "AR", "%1%", "if (%CY%) goto %1%;", "jump if carry flag is clear"}},
                    new object[] {0x4, new object[] {"JNVT", 1, "AR", "%1%", "if (!OV_TRAP) goto %1%;", "jump if overflow trap is clear"}},
                    new object[] {0x5, new object[] {"JNV", 1, "AR", "%1%", "if (!OV) goto %1%;", "jump if overflow flag is clear"}},
                    new object[] {0x6, new object[] {"JGE", 1, "AR", "%1%", "if ((sig) %P2% >= %P1%) goto %1%;", "jump if signed greater than or equal"}},
                    new object[] {0x7, new object[] {"JNE", 1, "AR", "%1%", "if (%P2% != %P1%) goto %1%;", "jump if not equal"}},
                    new object[] {0x8, new object[] {"JST", 1, "AR", "%1%", "if (STICKY) goto %1%;", "jump sticky bit is set"}},
                    new object[] {0x9, new object[] {"JGTU", 1, "AR", "%1%", "if ((uns) %P2% > %P1%) goto %1%;", "jump if unsigned higher"}},
                    new object[] {0xA, new object[] {"JLE", 1, "AR", "%1%", "if ((sig) %P2% <= %P1%) goto %1%;", "jump if signed less than or equal"}},
                    //new object[] {0xB, new object[] {"JC", 1, "AR", "%1%", "if (CY == 1) goto %1%;", "jump if carry flag is set"}},
                    //new object[] {0xB, new object[] {"JC", 1, "AR", "%1%", "if ((uns) %P2% >= %P1%) goto %1%;", "jump if carry flag is set"}},
                    new object[] {0xB, new object[] {"JC", 1, "AR", "%1%", "if (%CY%) goto %1%;", "jump if carry flag is set"}},
                    new object[] {0xC, new object[] {"JVT", 1, "AR", "%1%", "if (OV_TRAP) goto %1%;", "jump if overflow trap is set"}},
                    new object[] {0xD, new object[] {"JV", 1, "AR", "%1%", "if (OV) goto %1%;", "jump if overflow flag is set"}},
                    new object[] {0xE, new object[] {"JLT", 1, "AR", "%1%", "if ((sig) %P2% < %P1%) goto %1%;", "jump if signed less than"}},
                    new object[] {0xF, new object[] {"JE", 1, "AR", "%1%", "if (%P2% == %P1%) goto %1%;", "jump if equal"}}
                }
            },
            new object[] { 0xE, new object[] {"MOP"},
                new object[] {
                    new object[] {0x0, new object[] {"DJNZ", 2, "RB", "AR", "%1%,%2%", "%1%--; if (%1% !=  0) goto %2%;", "decrement and jump if not zero"}},
                    new object[] {0x1, new object[] {}},
                    new object[] {0x2, new object[] {}},
                    new object[] {0x3, new object[] {}},
                    new object[] {0x4, new object[] {}},
                    new object[] {0x5, new object[] {}},
                    new object[] {0x6, new object[] {}},
                    new object[] {0x7, new object[] {"JUMP", 2, "AB", "AB", "%1%", "goto %1%;", "long jump"}},
                    new object[] {0x8, new object[] {}},
                    new object[] {0x9, new object[] {}},
                    new object[] {0xA, new object[] {}},
                    new object[] {0xB, new object[] {}},
                    new object[] {0xC, new object[] {}},
                    new object[] {0xD, new object[] {}},
                    new object[] {0xE, new object[] {}},
                    new object[] {0xF, new object[] {"CALL", 2, "AB", "AB", "%1%", "%1%(%ARGS%);", "long call"}}
                }
            },
            new object[] { 0xF, new object[] {"UOP"},
                new object[] {
                    new object[] {0x0, new object[] {"RET", 0, "", "return;", "return from subroutine"}},
                    new object[] {0x1, new object[] {"RETI", 0, "", "return;", "retei-8065 reti-8061"}},
                    new object[] {0x2, new object[] {"PUSHP", 0, "", "push(PSW);", "push flags"}},
                    new object[] {0x3, new object[] {"POPP", 0, "", "pop(PSW);", "pop flags"}},
                    new object[] {0x4, new object[] {"BANK0", 0, "", "", "8065 only"}},
                    new object[] {0x5, new object[] {"BANK1", 0, "", "", "8065 only"}},
                    new object[] {0x6, new object[] {"BANK2", 0, "", "", "8065 only"}},
                    new object[] {0x7, new object[] {}},
                    new object[] {0x8, new object[] {"CLC", 0, "", "CY = 0;", "clear carry flag"}},
                    new object[] {0x9, new object[] {"STC", 0, "", "CY = 1;", "set carry flag"}},
                    new object[] {0xA, new object[] {"DI", 0, "", "disable ints;", "disable interrupt"}},
                    new object[] {0xB, new object[] {"EI", 0, "", "enable ints;", "enable interrupt"}},
                    new object[] {0xC, new object[] {"CLRVT", 0, "", "", "clear overflow trap"}},
                    new object[] {0xD, new object[] {"BANK3", 0, "", "", "8065 only"}},
                    new object[] {0xE, new object[] {"SIGND/ALT", -1, "changes multiply/divide to signed"}},
                    new object[] {0xF, new object[] {}}
                }
            }
        };
    }
}
