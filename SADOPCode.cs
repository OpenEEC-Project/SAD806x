using System;
using System.Collections;
using System.Text;

namespace SAD806x
{
    public class SADOPCode
    {
        public string OPCode = string.Empty;
        public int OPCodeInt = -1;

        public OPCodeType Type = OPCodeType.UndefinedOP;

        public bool is8061 = false;

        public bool isActive = false;

        public string Instruction = string.Empty;
        public string InstructionTrans = string.Empty;
        public string Translation1 = string.Empty;
        public string Translation2 = string.Empty;
        public string Translation3 = string.Empty;
        public string Comments = string.Empty;

        public int paramsNumber = -1;
        public int paramsNumberBis = -1;
        public OPCodeParameter[] parameters = null;

        public bool isSigndAlt = false;
        public bool hasVariableParams = false;

        private object[] OPCHeaderDef = null;
        private object[] OPCDef = null;

        private bool hasJump = false;
        private bool hasParts = false;

        private int shortJumpAdder = 0;

        private enum CarryModes
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

        private SortedList CarryTranslations = null;

        public SADOPCode(int iOPCode, bool bIs8061)
        {
            int tmpNumber = 0;

            OPCodeInt = iOPCode;
            OPCode = string.Format("{0:x2}", OPCodeInt);
            foreach (object[] OpcPart1 in SADDef.OPCodes)
            {
                if (Convert.ToInt32(OpcPart1[0]) == (OPCodeInt - OPCodeInt % 16) / 16)
                {
                    OPCHeaderDef = (object[])OpcPart1[1];
                    foreach (object[] OpcPart2 in (object[])OpcPart1[2])
                    {
                        if (Convert.ToInt32(OpcPart2[0]) == OPCodeInt % 16)
                        {
                            OPCDef = (object[])OpcPart2[1];
                            isActive = (OPCDef.Length != 0);
                            break;
                        }
                    }
                    break;
                }
            }

            is8061 = bIs8061;
            
            if (!isActive) return;

            Instruction = OPCDef[0].ToString();

            switch (OPCHeaderDef[0].ToString().Substring(0, 3))
            {
                case "UOP":
                    Type = OPCodeType.UndefinedOP;
                    break;
                case "BOP":
                    Type = OPCodeType.ByteOP;
                    break;
                case "WOP":
                    Type = OPCodeType.WordOP;
                    break;
                case "SJO":
                    Type = OPCodeType.ShortJumpOP;
                    break;
                case "BGO":
                    Type = OPCodeType.BitByteGotoOP;
                    break;
                case "GOP":
                    Type = OPCodeType.GotoOP;
                    break;
                case "MOP":
                    Type = OPCodeType.MixedOP;
                    break;
            }
            if (OPCHeaderDef[0].ToString().Length > 3)
            {
                if (OPCHeaderDef[0].ToString().Substring(3, 1) == "+")
                {
                    for (int iOPPart2 = 0; iOPPart2 < Convert.ToInt32(OPCHeaderDef[0].ToString().Substring(4)); iOPPart2++)
                    {
                        if (OPCode.EndsWith(string.Format("{0:x1}", iOPPart2))) Type = OPCodeType.MixedOP;
                    }
                }
                else if (OPCHeaderDef[0].ToString().Substring(3, 1) == "-")
                {
                    for (int iOPPart2 = 16 - Convert.ToInt32(OPCHeaderDef[0].ToString().Substring(4)); iOPPart2 < 16; iOPPart2++)
                    {
                        if (OPCode.EndsWith(string.Format("{0:x1}", iOPPart2))) Type = OPCodeType.MixedOP;
                    }
                }
            }

            if (Convert.ToInt32(OPCDef[1]) < 0)
            {
                isSigndAlt = true;
                return;
            }
            else if (Convert.ToInt32(OPCDef[1]) <= 10)
            {
                paramsNumber = Convert.ToInt32(OPCDef[1]);
                paramsNumberBis = -1;
                hasVariableParams = false;
            }
            else
            {
                paramsNumber = Convert.ToInt32(OPCDef[1].ToString().Substring(0, 1));
                paramsNumberBis = Convert.ToInt32(OPCDef[1].ToString().Substring(1, 1));
                hasVariableParams = true;
                if (paramsNumber > paramsNumberBis)
                {
                    tmpNumber = paramsNumber;
                    paramsNumber = paramsNumberBis;
                    paramsNumberBis = tmpNumber;
                }
            }

            parameters = new OPCodeParameter[paramsNumber];
            // Specific case for Skip which has no parameter
            if (OPCode == "00") parameters = new OPCodeParameter[1];

            for (int iParameter = 0; iParameter < parameters.Length; iParameter++)
            {
                parameters[iParameter].DefType = OPCDef[iParameter + 2].ToString().ToUpper();
                switch (parameters[iParameter].DefType.Substring(0, 2))
                {
                    case "VB":
                        parameters[iParameter].Type = OPCodeParamsTypes.ValueByte;
                        break;
                    case "WB":
                        parameters[iParameter].Type = OPCodeParamsTypes.ValueWordPart;
                        hasParts = true;
                        break;
                    case "RB":
                        parameters[iParameter].Type = OPCodeParamsTypes.RegisterByte;
                        break;
                    case "RW":
                        parameters[iParameter].Type = OPCodeParamsTypes.RegisterWord;
                        break;
                    case "AR":
                        parameters[iParameter].Type = OPCodeParamsTypes.AddressRelativePosition;
                        hasJump = true;
                        break;
                    case "AB":
                        parameters[iParameter].Type = OPCodeParamsTypes.AddressPartRelativePosition;
                        hasJump = true;
                        hasParts = true;
                        break;
                    case "AA":
                        parameters[iParameter].Type = OPCodeParamsTypes.AddressPartAbsolutePosition;
                        hasJump = true;
                        hasParts = true;
                        break;
                    case "BN":
                        parameters[iParameter].Type = OPCodeParamsTypes.Bank;
                        break;
                }
                parameters[iParameter].isPointer = parameters[iParameter].DefType.Contains("P");
            }

            if (OPCDef.Length > parameters.Length + 3)
            {
                InstructionTrans = OPCDef[2 + parameters.Length].ToString();
            }
            else
            {
                InstructionTrans = string.Empty;
                for (int iParam = parameters.Length - 1; iParam >= 0; iParam--)
                {
                    if (hasParts) InstructionTrans += "%" + iParam.ToString() + "%";
                    else InstructionTrans += "%" + (iParam + 1).ToString() + "%";
                    if ((iParam > 0 && !hasParts) || iParam > 1) InstructionTrans += SADDef.GlobalSeparator;
                    if (hasParts && iParam == 1) break;
                }
            }
            if (OPCDef.Length > parameters.Length + 4) Translation1 = OPCDef[3 + parameters.Length].ToString();
            else Translation1 = InstructionTrans;
            if (OPCDef.Length > parameters.Length + 5) Translation2 = OPCDef[4 + parameters.Length].ToString();
            else Translation2 = InstructionTrans;
            if (OPCDef.Length > parameters.Length + 6) Translation3 = OPCDef[5 + parameters.Length].ToString();
            else Translation3 = InstructionTrans;
            Comments = OPCDef[OPCDef.Length - 1].ToString();

            // Special Jumps Adder Management
            switch (OPCode.ToLower())
            {
                // sjmp
                case "20":
                // scall
                case "28":
                    shortJumpAdder = 0x0;
                    break;
                case "21":
                case "29":
                    shortJumpAdder = 0x100;
                    break;
                case "22":
                case "2a":
                    shortJumpAdder = 0x200;
                    break;
                case "23":
                case "2b":
                    shortJumpAdder = 0x300;
                    break;
                case "24":
                case "2c":
                    shortJumpAdder = -0x400;
                    break;
                case "25":
                case "2d":
                    shortJumpAdder = -0x300;
                    break;
                case "26":
                case "2e":
                    shortJumpAdder = -0x200;
                    break;
                case "27":
                case "2f":
                    shortJumpAdder = -0x100;
                    break;
            }

            // Carry Goto (jc, jnc) translation specificity
            if (OPCode.ToLower() == "db" || OPCode.ToLower() == "d3")
            {
                object[] defTranslations = null;
                if (OPCode.ToLower() == "db") defTranslations = SADDef.OPCJCTranslations;
                else defTranslations = SADDef.OPCJNCTranslations;

                CarryTranslations = new SortedList();
                foreach (string[] sTrans in defTranslations)
                {
                    CarryModes carryMode = CarryModes.Default;
                    switch (sTrans[0].ToLower())
                    {
                        case "default":
                            carryMode = CarryModes.Default; 
                            break;
                        case "cy":
                            carryMode = CarryModes.Carry;
                            break;
                        case "cmp":
                            carryMode = CarryModes.Comparison;
                            break;
                        case "-":
                            carryMode = CarryModes.Substract;
                            break;
                        case "+w":
                            carryMode = CarryModes.AddWords;
                            break;
                        case "+b":
                            carryMode = CarryModes.AddBytes;
                            break;
                        case "*w":
                            carryMode = CarryModes.MultiplyWords;
                            break;
                        case "*b":
                            carryMode = CarryModes.MultiplyBytes;
                            break;
                    }
                    CarryTranslations.Add(carryMode, sTrans[1]);
                }
                defTranslations = null;
            }
        }

        public Operation processOP(int opAddress, int callBankNum, int callAddress, string[] arrOP, ref SADCalib Calibration, bool applySignedAlt, ref Operation opPrevResult, ref string[] gopParams, ref SADS6x S6x)
        {
            Operation ope = null;
            int opParams = 0;
            bool bVariableParamsEnabled = false;
            bool bParamPartsProcessed = false;
            string sReg = string.Empty;
            int iValue = -1;
            int iParamForParameters = 0;
            ArrayList arrCleanedParams = null;

            ope = new Operation(callBankNum, opAddress);
            ope.OriginalOPCode = OPCode;
            ope.OriginalInstruction = Instruction;
            
            ope.ApplyOnBankNum = callBankNum;
            ope.SetBankNum = callBankNum;
            ope.ReadDataBankNum = Calibration.BankNum;
            ope.InitialCallAddressInt = callAddress;

            // External Bank Call or Jump Management
            // 20171110 Move from applyJumps to be applied on all Ops especially the one using pointers and the jump ones
            if (opPrevResult != null)
            {
                if (opPrevResult.ApplyOnBankNum != opPrevResult.SetBankNum)
                {
                    ope.ApplyOnBankNum = opPrevResult.SetBankNum;
                    ope.ReadDataBankNum = opPrevResult.SetBankNum;
                }
            }

            ope.BytesNumber = paramsNumber + 1;
            if (hasVariableParams)
            {
                //53,5b,96,47,c9,00 an3b  0,Rc9,[R5a+4796] 0 = Rc9 & [R5a+4796];
                //47,39,3a,72,00,38 ad3w  R38,0,[R38+723a] R38 = [R38+723a];
                //47,4d,64,03,34,32 ad3w  R32,R34,[R4c+364] R32 = R34 + [R4c+364];
                //47,32,0a,00,34    ad3w  R34,0,[R32+a]  R34 = [R32+a];
                //47,84,6e,a2,20    ad3w  R20,Ra2,[R84+6e] R20 = Ra2 + [2fa2];
                //a3,01,24,01,30    ldw   R30,[124]      R30 = [124];
                //a3,7e,1e,38       ldw   R38,[R7e+1e]   R38 = [264e];
                //a3,00,68,32       ldw   R32,[68]       R32 = [68];

                //If first parameter is multiple of 2 (B0 = 0), this is a direct register, lowest number of parameters has to be used.
                //                             if not (B0 = 1), 1 has to be removed from this register and highest number of parameters has to be used.
                if (Convert.ToByte(arrOP[1], 16) % 2 != 0)
                {
                    ope.BytesNumber = paramsNumberBis + 1;
                    bVariableParamsEnabled = true;
                }
            }
            opParams = ope.BytesNumber - 1;

            if (ope.BytesNumber > arrOP.Length) return ope;

            ope.OriginalOpArr = new string[ope.BytesNumber];
            for (int iOp = 0; iOp < ope.OriginalOpArr.Length; iOp++) ope.OriginalOpArr[iOp] = arrOP[iOp];

            ope.InstructedParams = new string[opParams];
            for (int iParam = 0; iParam < ope.InstructedParams.Length; iParam++) ope.InstructedParams[iParam] = arrOP[iParam + ope.BytesNumber - opParams];

            // Specific cases when no provided parameters
            switch (OPCode.ToLower())
            {
                // Skip
                case "00":
                    ope.InstructedParams = new string[] {"01"};
                    break;
            }

            bParamPartsProcessed = false;
            for (int iParam = 0; iParam < ope.InstructedParams.Length; iParam++)
            {
                // Predefined Parameters for OP Code are not in line for variable parameters (should be 1 less), the last value is used in this case.
                iParamForParameters = iParam;
                if (iParam >= parameters.Length) iParamForParameters = parameters.Length - 1;
                switch (parameters[iParamForParameters].Type)
                {
                    case OPCodeParamsTypes.Bank:
                        ope.SetBankNum = Convert.ToInt32(ope.InstructedParams[iParam], 16);
                        ope.InstructedParams[iParam] = ope.SetBankNum.ToString();
                        break;
                    case OPCodeParamsTypes.AddressRelativePosition:
                        if (parameters[iParamForParameters].isPointer) ope.InstructedParams[iParam] = Tools.PointerTranslation(ope.InstructedParams[iParam]);
                        break;
                    case OPCodeParamsTypes.RegisterByte:
                    case OPCodeParamsTypes.RegisterWord:
                        if (ope.InstructedParams[iParam] == "00")
                        {
                            ope.InstructedParams[iParam] = "0";
                        }
                        else
                        {
                            // Register auto Increment for Pointers
                            //  Multiple of 2 (B0 = 0), this is a direct register, no increment.
                            //                (B0 = 1), 1 has to be removed from this register, increment is applied.
                            if (parameters[iParamForParameters].isPointer)
                            {
                                if (Convert.ToByte(ope.InstructedParams[iParam], 16) % 2 != 0)
                                {
                                    ope.InstructedParams[iParam] = string.Format("{0:x2}" + SADDef.IncrementSuffix, Convert.ToInt32(ope.InstructedParams[iParam], 16) - 1);
                                }
                            }
                            ope.InstructedParams[iParam] = SADDef.ShortRegisterPrefix + ope.InstructedParams[iParam];
                            if (parameters[iParamForParameters].isPointer) ope.InstructedParams[iParam] = Tools.PointerTranslation(ope.InstructedParams[iParam]);
                        }
                        break;
                    case OPCodeParamsTypes.ValueByte:
                        // 20171118 - Remove Leading 0
                        ope.InstructedParams[iParam] = Convert.ToString(Convert.ToInt32(ope.InstructedParams[iParam], 16), 16);
                        if (parameters[iParamForParameters].isPointer) ope.InstructedParams[iParam] = Tools.PointerTranslation(ope.InstructedParams[iParam]);
                        break;
                    case OPCodeParamsTypes.ValueWordPart:
                    case OPCodeParamsTypes.AddressPartRelativePosition:
                    case OPCodeParamsTypes.AddressPartAbsolutePosition:
                        if (!bParamPartsProcessed)
                        {
                            if (hasVariableParams && bVariableParamsEnabled)
                            {
                                sReg = string.Format("{0:x2}", Convert.ToInt32(ope.InstructedParams[iParam], 16) - 1);
                                if (sReg == "00") sReg = string.Empty;
                                else sReg = Tools.RegisterInstruction(sReg);
                                iValue = Convert.ToInt32(ope.InstructedParams[iParam + 2] + ope.InstructedParams[iParam + 1], 16);
                                if (sReg == string.Empty && iValue == 0) ope.InstructedParams[iParam] = "0";
                                else if (sReg == string.Empty) ope.InstructedParams[iParam] = Convert.ToString(iValue, 16);
                                else if (iValue == 0) ope.InstructedParams[iParam] = sReg;
                                else ope.InstructedParams[iParam] = sReg + SADDef.AdditionSeparator + Convert.ToString(iValue, 16);
                                if (parameters[iParamForParameters].isPointer) ope.InstructedParams[iParam] = Tools.PointerTranslation(ope.InstructedParams[iParam]);
                                ope.InstructedParams[iParam + 1] = string.Empty;
                                ope.InstructedParams[iParam + 2] = string.Empty;
                                iParam += 2;
                            }
                            else if (hasVariableParams)
                            // First Instructed Param is a register, the other one a value to be added
                            {
                                sReg = ope.InstructedParams[iParam];
                                if (sReg == "00") sReg = string.Empty;
                                else sReg = Tools.RegisterInstruction(sReg);
                                iValue = Convert.ToInt32(ope.InstructedParams[iParam + 1], 16);
                                if (sReg == string.Empty && iValue == 0) ope.InstructedParams[iParam] = "0";
                                else if (sReg == string.Empty) ope.InstructedParams[iParam] = Convert.ToString(iValue, 16);
                                else if (iValue == 0) ope.InstructedParams[iParam] = sReg;
                                else ope.InstructedParams[iParam] = sReg + SADDef.AdditionSeparator + Convert.ToString(iValue, 16);
                                if (parameters[iParamForParameters].isPointer) ope.InstructedParams[iParam] = Tools.PointerTranslation(ope.InstructedParams[iParam]);
                                ope.InstructedParams[iParam + 1] = string.Empty;
                                iParam++;
                            }
                            else
                            {
                                ope.InstructedParams[iParam] = Convert.ToString(Convert.ToInt32(ope.InstructedParams[iParam + 1] + ope.InstructedParams[iParam], 16), 16);
                                if (parameters[iParamForParameters].isPointer) ope.InstructedParams[iParam] = Tools.PointerTranslation(ope.InstructedParams[iParam]);
                                ope.InstructedParams[iParam + 1] = string.Empty;
                                iParam++;
                            }
                        }
                        bParamPartsProcessed = true; ;
                        break;
                }
            }

            arrCleanedParams = new ArrayList();
            foreach (string sParam in ope.InstructedParams)
            {
                if (sParam != string.Empty) arrCleanedParams.Add(sParam);
            }
            ope.InstructedParams = (string[])arrCleanedParams.ToArray(typeof(string));
            arrCleanedParams = null;

            applyJumps(ref ope, ref gopParams, ref Calibration);

            ope.CalculatedParams = (string[])ope.InstructedParams.Clone();
            ope.TranslatedParams = (string[])ope.InstructedParams.Clone();
            ope.IgnoredTranslatedParam = -1;

            applyRbaseRconst(ref ope, ref Calibration, ref S6x, applySignedAlt);
            applyRegisters(ref ope, ref Calibration.slEecRegisters, ref S6x);
            applyShifting(ref ope);
            applyGotoOpParams(ref ope, ref gopParams);

            ope.Instruction = getInstructionTranslation(InstructionTrans, ref ope);
            ope.Translation1 = getTranslation(Translation1, ref ope, ref opPrevResult, ref S6x);
            ope.Translation2 = getTranslation(Translation2, ref ope, ref opPrevResult, ref S6x);
            ope.Translation3 = getTranslation(Translation3, ref ope, ref opPrevResult, ref S6x);

            //applyKnownTranslation(ref ope, ref S6x, true);

            if (applySignedAlt)
            {
                ope.AddressInt--;
                ope.BytesNumber++;
                string[] tmpOpArr = new string[ope.OriginalOpArr.Length + 1];
                tmpOpArr[0] = SADDef.OPCSigndAltOpCode;
                for (int iOp = 0; iOp < ope.OriginalOpArr.Length; iOp++) tmpOpArr[iOp + 1] = ope.OriginalOpArr[iOp];
                ope.OriginalOpArr = new string[tmpOpArr.Length];
                for (int iOp = 0; iOp < ope.OriginalOpArr.Length; iOp++) ope.OriginalOpArr[iOp] = tmpOpArr[iOp];
                if (ope.Instruction != string.Empty) ope.Instruction = SADDef.OPCSigndAltInstructionPrefix + ope.Instruction.Substring(0, ope.Instruction.Length - SADDef.OPCSigndAltInstructionPrefix.Length);
                ope.Translation1 = ope.Translation1.Replace("= ", "= " + SADDef.OPCSigndAltTranslationAdder);
                ope.Translation2 = ope.Translation2.Replace("= ", "= " + SADDef.OPCSigndAltTranslationAdder);
                ope.Translation3 = ope.Translation3.Replace("= ", "= " + SADDef.OPCSigndAltTranslationAdder);
            }

            ope.isReturn = false;
            switch (Instruction.ToLower())
            {
                case "ret":
                case "reti":
                    ope.isReturn = true;
                    // Goto Op Params coming from last compatible Op
                    // Trace stored on Return instruction to provide params to multiple call/scall
                    if (gopParams == null) ope.GotoOpParams = new string[] { ope.UniqueAddress, string.Empty, "0", "0", string.Empty, CarryModes.Default.ToString(), "0", "0" };
                    else ope.GotoOpParams = (string[])gopParams.Clone();
                    break;
            }
            
            return ope;
        }

        // Post Process Op RConst when RConst main call is validated, for inside main Call operations
        public void postProcessOpRConst(ref Operation ope, ref SADCalib Calibration, ref SADS6x S6x)
        {
            // No RConst calculation on Operations with less than 4 bytes
            if (ope.OriginalOpArr.Length < 4) return;

            foreach (RConst rConst in Calibration.slRconst.Values)
            {
                for (int iPos = 1; iPos < ope.OriginalOpArr.Length; iPos++)
                {
                    if (ope.OriginalOpArr[iPos] == rConst.Code)
                    {
                        applyRbaseRconst(ref ope, ref Calibration, ref S6x, false);
                        applyRegisters(ref ope, ref Calibration.slEecRegisters, ref S6x);
                        applyShifting(ref ope);

                        Operation opPrevResult = null;

                        ope.Instruction = getInstructionTranslation(InstructionTrans, ref ope);
                        ope.Translation1 = getTranslation(Translation1, ref ope, ref opPrevResult, ref S6x);
                        ope.Translation2 = getTranslation(Translation2, ref ope, ref opPrevResult, ref S6x);
                        ope.Translation3 = getTranslation(Translation3, ref ope, ref opPrevResult, ref S6x);

                        return;
                    }
                }
            }
        }

        // Post Process Op Call Args with translation
        public void postProcessOpCallArgs(ref Operation ope, ref SADCalib Calibration, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            ArrayList alCallArgsTranslated = null;
            Operation nullOpe = null;
            RBase rBase = null;
            SADBank readDataBank = null;
            CalibrationElement opeCalElem = null;
            int usedArgs = -1;
            int rBaseNum = -1;
            int rBaseAdder = -1;
            int structArgAddress = -1;
            int structRegAddress = -1;
            string translatedArg = string.Empty;

            // Calls and Short Calls only
            switch (ope.CallType)
            {
                case CallType.Call:
                case CallType.ShortCall:
                    break;
                default:
                    return;
            }
            if (ope.CallArgsNum == 0) return;
            if (ope.CallArguments == null) return;

            alCallArgsTranslated = new ArrayList();

            // Args Mode decoding to product Elements & Structures
            usedArgs = 0;
            foreach (CallArgument cArg in ope.CallArguments)
            {
                if (cArg.ByteSize + usedArgs > ope.CallArgsArr.Length) break;
                
                if (cArg.Word)
                // Word Argument - Elements Addresses are Word Arguments Mode0 to Mode4
                {
                    switch (cArg.Mode)
                    {
                        case CallArgsMode.Mode0:
                            // Direct Element without RBase
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16) - SADDef.EecBankStartAddress;
                            cArg.DecryptedValueInt = cArg.InputValueInt + SADDef.EecBankStartAddress;

                            if (Calibration.BankNum == ope.ReadDataBankNum && Calibration.isCalibrationAddress(cArg.InputValueInt) && !(Calibration.Info.is8061 && Calibration.Info.isEarly))
                            {
                                if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                opeCalElem = new CalibrationElement(ope.ReadDataBankNum, string.Empty);
                                ope.alCalibrationElems.Add(opeCalElem);
                                opeCalElem.AddressInt = cArg.InputValueInt;
                                opeCalElem.RBaseCalc = opeCalElem.Address;
                                switch (opeCalElem.BankNum)
                                {
                                    case 8:
                                        opeCalElem.AddressBinInt = opeCalElem.AddressInt + Bank8.AddressBinInt;
                                        break;
                                    case 1:
                                        opeCalElem.AddressBinInt = opeCalElem.AddressInt + Bank1.AddressBinInt;
                                        break;
                                    case 9:
                                        opeCalElem.AddressBinInt = opeCalElem.AddressInt + Bank9.AddressBinInt;
                                        break;
                                    case 0:
                                        opeCalElem.AddressBinInt = opeCalElem.AddressInt + Bank0.AddressBinInt;
                                        break;
                                }
                                if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                {
                                    opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                }
                                alCallArgsTranslated.Add(opeCalElem.Address);
                                opeCalElem = null;
                            }
                            else
                            {
                                ope.OtherElemAddress = cArg.DecryptedValue;
                                ope.KnownElemAddress = cArg.DecryptedValue;
                                alCallArgsTranslated.Add(ope.OtherElemAddress);
                            }
                            break;
                        case CallArgsMode.Mode1:
                            // To be managed - Temporary managed as Standard
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16);
                            cArg.DecryptedValueInt = cArg.InputValueInt;

                            alCallArgsTranslated.Add(Tools.PointerTranslation(cArg.DecryptedValue));
                            break;
                        case CallArgsMode.Mode2:
                            // To be managed - Temporary managed as Standard
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16);
                            cArg.DecryptedValueInt = cArg.InputValueInt;

                            alCallArgsTranslated.Add(Tools.PointerTranslation(cArg.DecryptedValue));
                            break;
                        case CallArgsMode.Mode3:
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16);
                            cArg.DecryptedValueInt = cArg.InputValueInt;

                            // f03a (3a,f0) => F[0+((f - 8) * 10) / 8] => F[0+70/8] => F[0+E] => FE + 03a
                            // 010c (0c,01) => Not compatible => [10c]
                            rBaseNum = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1].Substring(0, 1), 16);
                            if (rBaseNum < 8)
                            // Not Compatible Expression, this is a register
                            {
                                alCallArgsTranslated.Add(Tools.RegisterInstruction(cArg.DecryptedValue));
                            }
                            else
                            // Compatible Expression, this is a Calibration Element
                            {
                                rBaseNum = (((rBaseNum - 8) * 0x10) / 8) / 2;
                                rBaseAdder = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1].Substring(1, 1) + ope.CallArgsArr[usedArgs], 16);
                                if (rBaseNum < Calibration.slRbases.Count)
                                {
                                    rBase = ((RBase)Calibration.slRbases.GetByIndex(rBaseNum));
                                    if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                    opeCalElem = new CalibrationElement(rBase.BankNum, rBase.Code);
                                    ope.alCalibrationElems.Add(opeCalElem);
                                    opeCalElem.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code + SADDef.AdditionSeparator + Convert.ToString(rBaseAdder, 16);
                                    opeCalElem.AddressInt = rBase.AddressBankInt + rBaseAdder;
                                    opeCalElem.AddressBinInt = opeCalElem.AddressInt + Calibration.BankAddressBinInt;
                                    if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                    {
                                        opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                    }
                                    rBase = null;

                                    cArg.DecryptedValueInt = opeCalElem.AddressInt + SADDef.EecBankStartAddress;

                                    alCallArgsTranslated.Add(opeCalElem.Address);
                                    opeCalElem = null;
                                }
                                else
                                {
                                    alCallArgsTranslated.Add(Tools.PointerTranslation(cArg.DecryptedValue));
                                }
                            }
                            break;
                        case CallArgsMode.Mode4:
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16);
                            cArg.DecryptedValueInt = cArg.InputValueInt;

                            // 44,22 => 2244 => 2 + 244 => f2(f(0+2)) + 244 => RBase f2 + 244
                            // 44,32 => 3244 => 3 + 244 => f2(f(0+3-1)) + 1244 => RBase f2 + 1244
                            rBaseNum = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1].Substring(0, 1), 16);
                            rBaseAdder = (rBaseNum % 2) * 0x1000 + Convert.ToInt32(ope.CallArgsArr[usedArgs + 1].Substring(1, 1) + ope.CallArgsArr[usedArgs], 16);
                            if (rBaseNum % 2 == 1) rBaseNum--;
                            rBaseNum = rBaseNum / 2;
                            if (rBaseNum < Calibration.slRbases.Count)
                            {
                                rBase = ((RBase)Calibration.slRbases.GetByIndex(rBaseNum));
                                if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                opeCalElem = new CalibrationElement(rBase.BankNum, rBase.Code);
                                ope.alCalibrationElems.Add(opeCalElem);
                                opeCalElem.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code + SADDef.AdditionSeparator + Convert.ToString(rBaseAdder, 16);
                                opeCalElem.AddressInt = rBase.AddressBankInt + rBaseAdder;
                                opeCalElem.AddressBinInt = opeCalElem.AddressInt + Calibration.BankAddressBinInt;
                                if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                {
                                    opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                }
                                rBase = null;

                                cArg.DecryptedValueInt = opeCalElem.AddressInt + SADDef.EecBankStartAddress;

                                alCallArgsTranslated.Add(opeCalElem.Address);
                                opeCalElem = null;
                            }
                            else
                            {
                                alCallArgsTranslated.Add(Tools.PointerTranslation(cArg.DecryptedValue));
                            }
                            break;
                        case CallArgsMode.Mode4Struct:
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16);
                            cArg.DecryptedValueInt = cArg.InputValueInt;

                            // 44,22 => 2244 => [2244],[2246]   => Values to read in a structure
                            //      [2244] => 47,26 => 2647 => 2 + 647 => f2(f(0+2)) + 647 => RBase f2 + 647
                            //      [2246] => 12,01 => [112] => Input Register
                            //  Same principe than Mode 4 for other things
                            structArgAddress = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16) - SADDef.EecBankStartAddress;
                            structRegAddress = structArgAddress + 2;
                            switch (ope.ReadDataBankNum)
                            {
                                case 8:
                                    readDataBank = Bank8;
                                    break;
                                case 1:
                                    readDataBank = Bank1;
                                    break;
                                case 9:
                                    readDataBank = Bank9;
                                    break;
                                case 0:
                                    readDataBank = Bank0;
                                    break;
                            }
                            if (structArgAddress >= readDataBank.AddressInternalInt && structRegAddress < readDataBank.AddressInternalEndInt)
                            {
                                translatedArg = readDataBank.getWord(structArgAddress, true);
                                rBaseNum = Convert.ToInt32(translatedArg.Substring(0, 1), 16);
                                rBaseAdder = (rBaseNum % 2) * 0x1000 + Convert.ToInt32(translatedArg.Substring(1, 1) + translatedArg.Substring(2, 2), 16);
                                if (rBaseNum % 2 == 1) rBaseNum--;
                                rBaseNum = rBaseNum / 2;
                                translatedArg = Convert.ToString(readDataBank.getWordInt(structRegAddress, false, true), 16);
                                if (rBaseNum < Calibration.slRbases.Count)
                                {
                                    rBase = ((RBase)Calibration.slRbases.GetByIndex(rBaseNum));
                                    if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                    opeCalElem = new CalibrationElement(rBase.BankNum, rBase.Code);
                                    ope.alCalibrationElems.Add(opeCalElem);
                                    opeCalElem.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code + SADDef.AdditionSeparator + Convert.ToString(rBaseAdder, 16);
                                    opeCalElem.AddressInt = rBase.AddressBankInt + rBaseAdder;
                                    opeCalElem.AddressBinInt = opeCalElem.AddressInt + Calibration.BankAddressBinInt;
                                    if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                    {
                                        opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                    }
                                    rBase = null;
                                    ope.CallArgsStructRegister = Tools.RegisterInstruction(translatedArg);

                                    alCallArgsTranslated.Add(opeCalElem.Address);
                                    alCallArgsTranslated.Add(ope.CallArgsStructRegister);

                                    cArg.DecryptedValueInt = opeCalElem.AddressInt + SADDef.EecBankStartAddress;

                                    opeCalElem = null;
                                }
                                else
                                {
                                    alCallArgsTranslated.Add(Tools.PointerTranslation(cArg.DecryptedValue));
                                }
                            }
                            else
                            {
                                alCallArgsTranslated.Add(Tools.PointerTranslation(cArg.DecryptedValue));
                            }
                            readDataBank = null;
                            break;
                        default:
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16);
                            cArg.DecryptedValueInt = cArg.InputValueInt;

                            alCallArgsTranslated.Add(getRegisterTranslation(Tools.RegisterInstruction(cArg.DecryptedValue), false, ref Calibration.slEecRegisters, ref S6x));
                            break;
                    }
                }
                else
                // Byte Argument
                {
                    cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs], 16);
                    cArg.DecryptedValueInt = cArg.InputValueInt;
                    alCallArgsTranslated.Add(cArg.DecryptedValue);
                }
                usedArgs += cArg.ByteSize;
            }

            ope.CallArgsTranslatedArr = (string[])alCallArgsTranslated.ToArray(typeof(string));
            alCallArgsTranslated = null;

            ope.Translation1 = getTranslation(Translation1, ref ope, ref nullOpe, ref S6x);
            ope.Translation2 = getTranslation(Translation2, ref ope, ref nullOpe, ref S6x);
            ope.Translation3 = getTranslation(Translation3, ref ope, ref nullOpe, ref S6x);
        }

        public void postProcessOpSkipTranslation(ref Operation ope)
        {
            string skipGoto = string.Empty;
            
            if (ope.CallType != CallType.Skip) return;

            ope.Instruction = ope.Instruction.Replace("%JA%", ope.AddressJump);
            ope.Translation1 = ope.Translation1.Replace("%JA%", ope.AddressJump);
            ope.Translation2 = ope.Translation2.Replace("%JA%", ope.AddressJump);
            ope.Translation3 = ope.Translation3.Replace("%JA%", ope.AddressJump);
        }

        // Post Process Op for Element Translation
        public void postProcessOpElemTranslation(ref Operation ope, string elemAddress, string shortLabel)
        {
            if (ope == null) return;
            if (shortLabel == string.Empty) return;

            ope.Translation1 = ope.Translation1.Replace(elemAddress, shortLabel);
            ope.Translation2 = ope.Translation2.Replace(elemAddress, shortLabel);
            ope.Translation3 = ope.Translation3.Replace(elemAddress, shortLabel);
        }

        public void postProcessOpCallTranslation(ref Operation ope, ref Call cCall)
        {
            switch (ope.CallType)
            {
                case CallType.Unknown:
                case CallType.Skip:
                    return;
            }

            if (cCall.ShortLabel != string.Empty)
            {
                ope.Translation1 = ope.Translation1.Replace(cCall.Address, cCall.ShortLabel);
                ope.Translation2 = ope.Translation2.Replace(cCall.Address, cCall.ShortLabel);
                ope.Translation3 = ope.Translation3.Replace(cCall.Address, cCall.ShortLabel);
            }
        }

        private void applyJumps(ref Operation ope, ref string[] gopParams, ref SADCalib Calibration)
        {
            ope.AddressJumpInt = -1;

            if (!hasJump) return;

            // Jump type identification based on Instruction
            switch (Instruction.ToLower())
            {
                case "call":
                case "push":
                    ope.CallType = CallType.Call;
                    break;
                case "scall":
                    ope.CallType = CallType.ShortCall;
                    break;
                case "sjmp":
                    ope.CallType = CallType.ShortJump;
                    break;
                case "jump":
                    ope.CallType = CallType.Jump;
                    break;
                case "skip":
                    ope.CallType = CallType.Skip;
                    break;
                default:
                    ope.CallType = CallType.Goto;
                    break;
            }

            // Goto Op Params coming from last compatible Op, will be used for translations
            if (ope.CallType == CallType.Goto && Type == OPCodeType.GotoOP)
            {
                if (gopParams == null) ope.GotoOpParams = new string[] { ope.UniqueAddress, string.Empty, "0", "0", string.Empty, CarryModes.Default.ToString(), "0", "0" };
                else ope.GotoOpParams = (string[])gopParams.Clone();
            }

            for (int iParam = 0; iParam < ope.InstructedParams.Length; iParam++)
            {
                if (parameters[iParam].Type == OPCodeParamsTypes.AddressRelativePosition)
                {
                    if (ope.CallType == CallType.ShortCall || ope.CallType == CallType.ShortJump)
                    {
                        ope.AddressJumpInt = ope.AddressNextInt + Convert.ToInt32(ope.InstructedParams[iParam], 16) + shortJumpAdder;
                    }
                    else
                    {
                        // Signed Jump
                        ope.AddressJumpInt = ope.AddressNextInt + Convert.ToSByte(ope.InstructedParams[iParam], 16) + shortJumpAdder;
                    }
                    ope.InstructedParams[iParam] = ope.AddressJump;
                    // Long Word Result happens
                    if (ope.InstructedParams[iParam].Length > 4)
                    {
                        ope.InstructedParams[iParam] = ope.InstructedParams[iParam].Substring(ope.InstructedParams[iParam].Length - 4, 4);
                        ope.AddressJumpInt = Convert.ToInt32(ope.InstructedParams[iParam], 16) - SADDef.EecBankStartAddress;
                    }
                    break;
                }
                else if (parameters[iParam].Type == OPCodeParamsTypes.AddressPartRelativePosition)
                {
                    ope.AddressJumpInt = ope.AddressNextInt + Convert.ToInt32(ope.InstructedParams[iParam], 16);
                    ope.InstructedParams[iParam] = ope.AddressJump;
                    // Long Word Result happens
                    if (ope.InstructedParams[iParam].Length > 4)
                    {
                        ope.InstructedParams[iParam] = ope.InstructedParams[iParam].Substring(ope.InstructedParams[iParam].Length - 4, 4);
                        ope.AddressJumpInt = Convert.ToInt32(ope.InstructedParams[iParam], 16) - SADDef.EecBankStartAddress;
                    }
                    break;
                }
                else if (parameters[iParam].Type == OPCodeParamsTypes.AddressPartAbsolutePosition)
                {
                    ope.AddressJumpInt = Convert.ToInt32(ope.InstructedParams[iParam], 16) - SADDef.EecBankStartAddress;
                    ope.InstructedParams[iParam] = ope.AddressJump;
                    // Long Word Result happens
                    if (ope.InstructedParams[iParam].Length > 4)
                    {
                        ope.InstructedParams[iParam] = ope.InstructedParams[iParam].Substring(ope.InstructedParams[iParam].Length - 4, 4);
                        ope.AddressJumpInt = Convert.ToInt32(ope.InstructedParams[iParam], 16) - SADDef.EecBankStartAddress;
                    }
                    break;
                }
            }
        }

        // RBase, RConst Included Param Calculation
        private void applyRbaseRconst(ref Operation ope, ref SADCalib Calibration, ref SADS6x S6x, bool applySignedAlt)
        {
            string sParamCalc = string.Empty;
            object[] arrPointersValues = null;
            RBase rBase = null;
            RConst rConst = null;
            CalibrationElement opeCalElem = null;

            //  Returns 0,1 - 0 Not Pointer, 1 Pointer - Int
            //          Pointer / Value 1   - String format
            //          Pointer / Value 1   - Integer format
            //          Pointer / Value 2   - String if exists
            //          Pointer / Value 2   - Integer format if exists

            // Pointer / Value 2 is not managed for now

            switch (OPCode.ToLower())
            {
                case "45":
                    // Specific case RBase an3w R32 = R7c + 1a => R32 = 241a
                    //      for main part of Functions & Tables
                    //20140428
                    //arrPointersValues = Tools.InstructionPointersValues(ope.TranslatedParams[1]);
                    arrPointersValues = Tools.InstructionPointersValues(ope.CalculatedParams[1]);

                    // No Register to manage
                    if (!(bool)arrPointersValues[0]) break;

                    rBase = (RBase)Calibration.slRbases[arrPointersValues[1].ToString()];
                    if (rBase != null)
                    {
                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                        opeCalElem = new CalibrationElement(Calibration.BankNum, rBase.Code);
                        ope.alCalibrationElems.Add(opeCalElem);
                        opeCalElem.RBaseCalc = ope.TranslatedParams[1] + SADDef.AdditionSeparator + ope.TranslatedParams[0];
                        //20140428
                        //opeCalElem.AddressInt = rBase.AddressBankInt + Convert.ToInt32(ope.TranslatedParams[0], 16);
                        opeCalElem.AddressInt = rBase.AddressBankInt + Convert.ToInt32(ope.CalculatedParams[0], 16);
                        opeCalElem.AddressBinInt = opeCalElem.AddressInt + Calibration.BankAddressBinInt;
                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                        {
                            opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                        }

                        ope.CalculatedParams[1] = opeCalElem.Address;
                        ope.TranslatedParams[1] = opeCalElem.Address;
                        ope.IgnoredTranslatedParam = 0;
    
                        rBase = null;
                        opeCalElem = null;
                        break;
                    }

                    rConst = (RConst)Calibration.slRconst[arrPointersValues[1].ToString()];
                    if (rConst != null)
                    {
                        if (rConst.isValue)
                        {
                            ope.CalculatedParams[1] = rConst.Value;
                            ope.TranslatedParams[1] = rConst.Value;
                            ope.IgnoredTranslatedParam = 0;
                        }
                        rConst = null;
                        break;
                    }
                    break;
                case "64":
                case "a0":
                    // Specific case RBase Calibration Structure
                    // 65,ad,07,34          ad2w  R34,7ad            R34 += 7ad;          
                    // 64,84,34             ad2w  R34,R84            R34 += R84;          
                    // b2,34,30             ldb   R30,[R34]          R30 = [R34];         // R34 is a structure address 
                    //
                    // a0,R84,R34           ldw   R34,R84            R34 = R84;
                    // 65,ad,07,34          ad2w  R34,7ad            R34 += 7ad;
                    //                                               R34 += ...;
                    // b2,34,30             ldb   R30,[R34]          R30 = [R34];         // R34 is a structure address 
                    if (Calibration.slRbases.ContainsKey(ope.OriginalOpArr[1]))
                    {
                        ope.CalElemRBaseStructRBase = ope.OriginalOpArr[1];
                    }
                    break;
                case "65":
                case "a1":
                    // Specific case Possible Non Calibration Element
                    // 65,72,40,32       ad2w  R32,4072       R32 += 4072;
                    // a2,32,34          ldw   R34,[R32]      R34 = [R32];
                    //
                    // a1,2c,41,30       ldw   R30,412c       R30 = 412c;
                    // 57,30,00,00,36    ad3b  R36,0,[R30+0]  R36 = [R30];
                    //      for main part of Structures
                    ope.OtherElemAddress = ope.InstructedParams[0];
                    ope.KnownElemAddress = ope.InstructedParams[0];
                    // Now manages Register Addresses between 0xf000 & 0xffff
                    if (Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress < 0 || Convert.ToInt32(ope.OtherElemAddress, 16) >= 0xf000 && Convert.ToInt32(ope.OtherElemAddress, 16) <= 0xffff) ope.OtherElemAddress = string.Empty;
                    if (ope.KnownElemAddress.Length < 4) ope.KnownElemAddress = string.Empty;
                    break;
                default:
                    //20180428
                    //for (int iParam = 0; iParam < ope.TranslatedParams.Length; iParam++)
                    for (int iParam = 0; iParam < ope.CalculatedParams.Length; iParam++)
                    {
                        //20180428
                        //sParamCalc = ope.TranslatedParams[iParam];
                        sParamCalc = ope.CalculatedParams[iParam];
                        if (sParamCalc.StartsWith(SADDef.LongRegisterPointerPrefix) && !sParamCalc.Contains(SADDef.AdditionSeparator + SADDef.ShortRegisterPrefix) && !sParamCalc.Contains(SADDef.IncrementSuffix))
                        {
                            arrPointersValues = Tools.InstructionPointersValues(sParamCalc);

                            // No Register to manage
                            if (!(bool)arrPointersValues[0]) break;

                            if (arrPointersValues.Length == 5)
                            {
                                rBase = (RBase)Calibration.slRbases[arrPointersValues[1].ToString()];
                                if (rBase != null)
                                {
                                    // Pointer Calculation
                                    if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                    opeCalElem = new CalibrationElement(Calibration.BankNum, rBase.Code);
                                    ope.alCalibrationElems.Add(opeCalElem);
                                    opeCalElem.RBaseCalc = Tools.RegisterInstruction(rBase.Code) + SADDef.AdditionSeparator + arrPointersValues[3].ToString();
                                    opeCalElem.AddressInt = rBase.AddressBankInt + (int)arrPointersValues[4];
                                    opeCalElem.AddressBinInt = opeCalElem.AddressInt + Calibration.BankAddressBinInt;

                                    // Checking if Object is defined on this Type on S6x, if not its type will stay unidentifed until S6x Processing
                                    if (!S6x.isS6xProcessTypeConflict(opeCalElem.UniqueAddress, typeof(Scalar)))
                                    {
                                        opeCalElem.ScalarElem = new Scalar();
                                        opeCalElem.ScalarElem.BankNum = opeCalElem.BankNum;
                                        opeCalElem.ScalarElem.AddressInt = opeCalElem.AddressInt;
                                        opeCalElem.ScalarElem.AddressBinInt = opeCalElem.AddressBinInt;
                                        opeCalElem.ScalarElem.RBase = opeCalElem.RBase;
                                        opeCalElem.ScalarElem.RBaseCalc = opeCalElem.RBaseCalc;
                                        // Mixed OP is for ldzbw or ldsbw is this case, so Byte
                                        opeCalElem.ScalarElem.Byte = (Type == OPCodeType.ByteOP || Type == OPCodeType.MixedOP);
                                        opeCalElem.ScalarElem.Word = (Type == OPCodeType.WordOP);
                                        // Sign basic identification
                                        if (applySignedAlt) opeCalElem.ScalarElem.Signed = true;
                                        else if (Instruction.ToLower() == "ldsbw") opeCalElem.ScalarElem.Signed = true;
                                        else if (Instruction.ToLower() == "ldzbw") opeCalElem.ScalarElem.UnSigned = true;
                                        else if (Instruction.ToLower().StartsWith("ml")) opeCalElem.ScalarElem.UnSigned = true;
                                        else if (Instruction.ToLower().StartsWith("div")) opeCalElem.ScalarElem.UnSigned = true;
                                    }

                                    ope.CalculatedParams[iParam] = Tools.PointerTranslation(opeCalElem.Address);
                                    ope.TranslatedParams[iParam] = Tools.PointerTranslation(opeCalElem.Address);

                                    if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                    {
                                        opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                    }

                                    rBase = null;
                                    opeCalElem = null;
                                    break;
                                }

                                rConst = (RConst)Calibration.slRconst[arrPointersValues[1].ToString()];
                                if (rConst != null)
                                {
                                    if (rConst.isValue)
                                    {
                                        // Pointer Calculation
                                        if (arrPointersValues[3].ToString().Length > 2) sParamCalc = Convert.ToString(rConst.ValueInt + (int)arrPointersValues[4], 16);
                                        else sParamCalc = Convert.ToString(rConst.ValueInt + Convert.ToSByte(arrPointersValues[3].ToString(), 16), 16);

                                        ope.CalculatedParams[iParam] = Tools.PointerTranslation(sParamCalc);
                                        ope.TranslatedParams[iParam] = Tools.PointerTranslation(sParamCalc);

                                        // Possible Non Calibration Element
                                        switch (Instruction.ToLower())
                                        {
                                            case "stb":
                                            case "stw":
                                                // Not related with Non Calibration Element 

                                                // But can be with External Elements
                                                ope.KnownElemAddress = sParamCalc;
                                                break;
                                            default:
                                                ope.OtherElemAddress = sParamCalc;
                                                ope.KnownElemAddress = sParamCalc;
                                                // Now manages Register Addresses between 0xf000 & 0xffff
                                                if (Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress < 0 || Convert.ToInt32(ope.OtherElemAddress, 16) >= 0xf000 && Convert.ToInt32(ope.OtherElemAddress, 16) <= 0xffff) ope.OtherElemAddress = string.Empty;
                                                if (ope.KnownElemAddress.Length < 4) ope.KnownElemAddress = string.Empty;
                                                break;
                                        }
                                    }
                                    rConst = null;
                                    break;
                                }
                            }

                            // Possible Non Calibration Element
                            switch (Instruction.ToLower())
                            {
                                case "stb":
                                case "stw":
                                    // Not related with Non Calibration Element 

                                    // But can be with External Elements
                                    if (arrPointersValues.Length == 5) ope.KnownElemAddress = arrPointersValues[3].ToString();
                                    else ope.KnownElemAddress = arrPointersValues[1].ToString();
                                    if (ope.KnownElemAddress.Length < 4) ope.KnownElemAddress = string.Empty;
                                    break;
                                default:
                                    if (arrPointersValues.Length == 5)
                                    {
                                        ope.OtherElemAddress = arrPointersValues[3].ToString();
                                        ope.KnownElemAddress = arrPointersValues[3].ToString();
                                        // Now manages Register Addresses between 0xf000 & 0xffff
                                        if ((int)arrPointersValues[4] - SADDef.EecBankStartAddress < 0 || (int)arrPointersValues[4] >= 0xf000 && (int)arrPointersValues[4] <= 0xffff) ope.OtherElemAddress = string.Empty;
                                        if (ope.KnownElemAddress.Length < 4) ope.KnownElemAddress = string.Empty;
                                    }
                                    else
                                    {
                                        ope.OtherElemAddress = arrPointersValues[1].ToString();
                                        ope.KnownElemAddress = arrPointersValues[1].ToString();
                                        // Now manages Register Addresses between 0xf000 & 0xffff
                                        if ((int)arrPointersValues[2] - SADDef.EecBankStartAddress < 0 || (int)arrPointersValues[2] >= 0xf000 && (int)arrPointersValues[2] <= 0xffff) ope.OtherElemAddress = string.Empty;
                                        if (ope.KnownElemAddress.Length < 4) ope.KnownElemAddress = string.Empty;
                                    }
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        // Registers Translation applied on Operation Translated Params
        private void applyRegisters(ref Operation ope, ref SortedList slEecRegisters, ref SADS6x S6x)
        {
            for (int iParam = 0; iParam < ope.CalculatedParams.Length; iParam++)
            {
                // 8061 KAM / CC / EC Additional Detection and Flags
                if (is8061) set8061KamCcEc(ref ope, ope.CalculatedParams[iParam]);

                ope.TranslatedParams[iParam] = getRegisterTranslation(ope.CalculatedParams[iParam], (iParam == ope.CalculatedParams.Length - 1), ref slEecRegisters, ref S6x);
            }
        }

        // Byte shifting
        private void applyShifting(ref Operation ope)
        {
            switch (Instruction.ToLower())
            {
                case "shrw":
                case "shlw":
                case "asrw":
                case "shrdw":
                case "shldw":
                case "asrdw":
                case "shrb":
                case "shlb":
                case "asrb":
                    break;
                default:
                    return;
            }
            
            // Value to use is always the first parameter
            //20180428
            //int byteValue = Convert.ToInt32(ope.TranslatedParams[0], 16);
            int byteValue = Convert.ToInt32(ope.CalculatedParams[0], 16);
            if (byteValue > 16)
            {
                ope.TranslatedParams[0] = Tools.RegisterInstruction(Convert.ToString(byteValue, 16));
            }
            else
            {
                ope.TranslatedParams[0] = Convert.ToString((int)Math.Pow(2, byteValue), 16);
            }
        }

        // Registers Translation applied on Operation Translated Params
        private void applyGotoOpParams(ref Operation ope, ref string[] gopParams)
        {
            string elemAddress = string.Empty;
            string cyOpeUniqueAddress = string.Empty;
            CarryModes cyMode = CarryModes.Default;
            string cyP1 = string.Empty;
            string cyP2 = string.Empty;

            if (ope.alCalibrationElems != null)
            {
                foreach (CalibrationElement opeCalElem in ope.alCalibrationElems)
                {
                    elemAddress = opeCalElem.Address;
                    // First Calibration Element is enought for this process
                    break;
                }
            }
            if (elemAddress == string.Empty && ope.OtherElemAddress != string.Empty) elemAddress = ope.OtherElemAddress;

            // Goto Ops Params Mngt
            // GotoOpParams[Params Ope UniqueAddress, Params Ope Elem UniqueAddress, P1, P2, CY Ope UniqueAddress, CY Mode, CY P1, CY P2]
            switch (ope.CallType)
            {
                case CallType.Unknown:
                    if (gopParams == null) gopParams = new string[] { ope.UniqueAddress, elemAddress, "0", "0", cyOpeUniqueAddress, cyMode.ToString(), cyP1, cyP2 };

                    gopParams[0] = ope.UniqueAddress;
                    gopParams[1] = elemAddress;
                    if (ope.TranslatedParams.Length >= 1) gopParams[2] = ope.TranslatedParams[0];
                    if (ope.TranslatedParams.Length >= 2) gopParams[3] = ope.TranslatedParams[1];

                    cyOpeUniqueAddress = gopParams[4];
                    cyMode = (CarryModes)Enum.Parse(typeof(CarryModes), gopParams[5]);
                    cyP1 = gopParams[6];
                    cyP2 = gopParams[7];
                    switch (Instruction.ToLower())
                    {
                        // Carry Flag operations
                        case "clc":
                        case "stc":
                            cyOpeUniqueAddress = ope.UniqueAddress;
                            cyMode = CarryModes.Carry;
                            break;
                        // Comparison operations
                        case "cmpb":
                        case "cmpw":
                            cyOpeUniqueAddress = ope.UniqueAddress;
                            cyMode = CarryModes.Comparison;
                            if (ope.TranslatedParams.Length >= 2)
                            {
                                cyP1 = ope.TranslatedParams[0];
                                cyP2 = ope.TranslatedParams[1];
                            }
                            break;
                        // Arithmetic operations
                        case "sbbb":
                        case "sb2b":
                        case "sb3b":
                        case "sbbw":
                        case "sb2w":
                        case "sb3w":
                            cyOpeUniqueAddress = ope.UniqueAddress;
                            cyMode = CarryModes.Substract;
                            if (ope.TranslatedParams.Length >= 2)
                            {
                                cyP1 = ope.TranslatedParams[0];
                                cyP2 = ope.TranslatedParams[1];
                            }
                            break;
                        case "adcb":
                        case "ad2b":
                        case "ad3b":
                            cyOpeUniqueAddress = ope.UniqueAddress;
                            cyMode = CarryModes.AddBytes;
                            if (ope.TranslatedParams.Length >= 2)
                            {
                                cyP1 = ope.TranslatedParams[0];
                                cyP2 = ope.TranslatedParams[1];
                            }
                            break;
                        case "adcw":
                        case "ad2w":
                        case "ad3w":
                            cyOpeUniqueAddress = ope.UniqueAddress;
                            cyMode = CarryModes.AddWords;
                            if (ope.TranslatedParams.Length >= 2)
                            {
                                cyP1 = ope.TranslatedParams[0];
                                cyP2 = ope.TranslatedParams[1];
                            }
                            break;
                        case "ml2b":
                        case "ml3b":
                        case "shlb":
                            cyOpeUniqueAddress = ope.UniqueAddress;
                            cyMode = CarryModes.MultiplyBytes;
                            if (ope.TranslatedParams.Length >= 2)
                            {
                                cyP1 = ope.TranslatedParams[0];
                                cyP2 = ope.TranslatedParams[1];
                            }
                            break;
                        case "ml2w":
                        case "ml3w":
                        case "shlw":
                        case "shldw":
                            cyOpeUniqueAddress = ope.UniqueAddress;
                            cyMode = CarryModes.MultiplyWords;
                            if (ope.TranslatedParams.Length >= 2)
                            {
                                cyP1 = ope.TranslatedParams[0];
                                cyP2 = ope.TranslatedParams[1];
                            }
                            break;
                        case "decb":
                        case "decw":
                            cyOpeUniqueAddress = ope.UniqueAddress;
                            cyMode = CarryModes.Substract;
                            if (ope.TranslatedParams.Length >= 1)
                            {
                                cyP1 = ope.TranslatedParams[0];
                                cyP2 = "1";
                            }
                            break;
                        case "incb":
                            cyOpeUniqueAddress = ope.UniqueAddress;
                            cyMode = CarryModes.AddBytes;
                            if (ope.TranslatedParams.Length >= 1)
                            {
                                cyP1 = ope.TranslatedParams[0];
                                cyP2 = "1";
                            }
                            break;
                        case "incw":
                            cyOpeUniqueAddress = ope.UniqueAddress;
                            cyMode = CarryModes.AddWords;
                            if (ope.TranslatedParams.Length >= 1)
                            {
                                cyP1 = ope.TranslatedParams[0];
                                cyP2 = "1";
                            }
                            break;
                    }
                    gopParams[4] = cyOpeUniqueAddress;
                    gopParams[5] = cyMode.ToString();
                    gopParams[6] = cyP1;
                    gopParams[7] = cyP2;
                    break;
            }
        }

        // Set KAM, CC and EC Flags on Operation for 8061
        //  Should be executed in applyRegisters Function before Register Translation
        private void set8061KamCcEc(ref Operation ope, string initialTranslatedParam)
        {
            object[] arrPointersValues = null;
            string sTranslation = string.Empty;

            // 8061 dedicated
            if (!is8061) return;

            arrPointersValues = Tools.InstructionPointersValues(initialTranslatedParam);
            //  Returns 0,1 - 0 Not Pointer, 1 Pointer - Int
            //          Pointer / Value 1   - String format
            //          Pointer / Value 1   - Integer format
            //          Pointer / Value 2   - String if exists
            //          Pointer / Value 2   - Integer format if exists

            // Pointer / Value 2 is not managed for now

            // No Register to manage
            if (!(bool)arrPointersValues[0]) return;

            // KAM / CC / EC Register ranges - 8061 only
            //  Flag will be set
            if ((int)arrPointersValues[2] >= SADDef.KAMRegisters8061MinAdress && (int)arrPointersValues[2] <= SADDef.KAMRegisters8061MaxAdress)
            {
                ope.isKamRelated = true;
            }
            else if ((int)arrPointersValues[2] >= SADDef.CCRegisters8061MinAdress && (int)arrPointersValues[2] <= SADDef.CCRegisters8061MaxAdress)
            {
                ope.isCcRelated = true;
            }
            else if ((int)arrPointersValues[2] >= SADDef.ECRegisters8061MinAdress && (int)arrPointersValues[2] <= SADDef.ECRegisters8061MaxAdress)
            {
                ope.isEcRelated = true;
            }
        }
        
        private string getInstructionTranslation(string sTemplate, ref Operation ope)
        {
            string sTranslation = sTemplate;

            // Instruction Translation
            if (sTranslation != string.Empty)
            {
                for (int iParam = 0; iParam < ope.InstructedParams.Length; iParam++)
                {
                    sTranslation = sTranslation.Replace("%" + (iParam + 1).ToString() + "%", ope.InstructedParams[iParam]);
                }
            }
            return string.Format("{0,-6}{1,-15}", Instruction.ToLower(), sTranslation);
        }

        private string getTranslation(string sTemplate, ref Operation ope, ref Operation opPrevResult, ref SADS6x S6x)
        {
            string sTranslation = sTemplate;

            if (sTranslation == string.Empty) return sTranslation;

            // BitByteGotoOP Specific BitFlags Case - Started First to override normal template on register
            //      Template is overriden when  BitFlags are provided on related S6x Register
            if (Type == OPCodeType.BitByteGotoOP && ope.OriginalOpArr.Length >= 2)
            {
                S6xRegister s6xReg = (S6xRegister)S6x.slProcessRegisters[Tools.RegisterUniqueAddress(ope.OriginalOpArr[1])];
                if (s6xReg != null)
                {
                    if (s6xReg.isBitFlags && s6xReg.BitFlags != null)
                    {
                        foreach (S6xBitFlag bitFlag in s6xReg.BitFlags)
                        {
                            if (bitFlag == null) break;
                            if (bitFlag.Position > 7) continue;
                            if (OPCodeInt == 0x30 + bitFlag.Position || OPCodeInt == 0x38 + bitFlag.Position)
                            // jnb / jb
                            {
                                string regTranslation = string.Empty;
                                string sValue = string.Empty;

                                if (bitFlag.ShortLabel == null) break;
                                if (bitFlag.ShortLabel.ToLower() == "b" + bitFlag.Position.ToString()) break;
                                // jnb
                                if (OPCodeInt == 0x30 + bitFlag.Position && bitFlag.NotSetValue != null) sValue = bitFlag.NotSetValue;
                                // jb
                                else if (bitFlag.SetValue != null) sValue = bitFlag.SetValue;

                                if (s6xReg.MultipleMeanings && s6xReg.ByteLabel != null) regTranslation = Tools.PointerTranslation(s6xReg.Labels(true));
                                else if (s6xReg.Label == null) regTranslation = Tools.PointerTranslation(s6xReg.Address);
                                else regTranslation = Tools.PointerTranslation(s6xReg.Label);
                                
                                // Based on existing templated, if updated this code should be updated too
                                if (sValue == string.Empty)
                                {
                                    sTranslation = sTranslation.Replace("%1%", "").Replace("B" + bitFlag.Position.ToString() + "_", regTranslation + SADDef.BitByteGotoOPAltSeparator + bitFlag.ShortLabel);
                                }
                                else
                                {
                                    sTranslation = sTranslation.Replace("!", "").Replace("B" + bitFlag.Position.ToString() + "_", regTranslation + SADDef.BitByteGotoOPAltSeparator + bitFlag.ShortLabel + SADDef.BitByteGotoOPAltComparison).Replace("%1%", sValue);
                                }
                                break;
                            }
                        }
                    }
                    s6xReg = null;
                }
            }

            for (int iParam = 0; iParam < ope.TranslatedParams.Length; iParam++)
            {
                // Specific case related to RBase calculation
                if (iParam == ope.IgnoredTranslatedParam)
                {
                    switch (iParam)
                    {
                        case 0:
                            if (iParam < ope.TranslatedParams.Length - 1)
                            {
                                int iEndPosNext = sTranslation.LastIndexOf("%" + (iParam + 2).ToString() + "%") + ("%" + (iParam + 2).ToString() + "%").Length - 1;
                                int iEndPosCurr = sTranslation.LastIndexOf("%" + (iParam + 1).ToString() + "%") + ("%" + (iParam + 1).ToString() + "%").Length - 1;
                                if (iEndPosCurr > iEndPosNext && iEndPosCurr <= sTranslation.Length)
                                {
                                    sTranslation = sTranslation.Replace(sTranslation.Substring(iEndPosNext, iEndPosCurr - iEndPosNext), "");
                                }
                                else
                                {
                                    sTranslation = sTranslation.Replace("%" + (iParam + 1).ToString() + "%", ope.TranslatedParams[iParam]);
                                }
                            }
                            break;
                        case 1:
                            // Not Managed - Not Detected
                            break;
                        case 2:
                            // Not Managed - Not Detected
                            break;
                    }
                }
                else
                {
                    sTranslation = sTranslation.Replace("%" + (iParam + 1).ToString() + "%", ope.TranslatedParams[iParam]);
                }
            }

            // Goto Op Params Translation
            if (ope.GotoOpParams != null && (sTemplate.Contains("%P1%") || sTemplate.Contains("%P2%") || sTemplate.Contains("%CY%")))
            {
                // GotoOpParams[Params Ope UniqueAddress, Params Ope Elem UniqueAddress, P1, P2, CY Ope UniqueAddress, CY Mode, CY P1, CY P2]
                if (CarryTranslations == null)
                {
                    sTranslation = sTranslation.Replace("%P1%", ope.GotoOpParams[2]).Replace("%P2%", ope.GotoOpParams[3]);
                }
                else
                {
                    sTranslation = sTranslation.Replace("%CY%", CarryTranslations[(CarryModes)Enum.Parse(typeof(CarryModes), ope.GotoOpParams[5])].ToString()).Replace("%P1%", ope.GotoOpParams[6]).Replace("%P2%", ope.GotoOpParams[7]);
                }
            }

            // Arguments
            if (ope.CallType == CallType.Call || ope.CallType == CallType.ShortCall)
            {
                sTranslation = sTranslation.Replace("%ARGS%", ope.CallArgsTranslated);
            }

            // Byte shifting / Value to use is always the first parameter
            if (sTemplate.Contains("%2^%"))
            {
                if (ope.TranslatedParams[0] == Tools.RegisterInstruction(ope.InstructedParams[0]))
                {
                    sTranslation = sTranslation.Replace("/", ">>").Replace("*", "<<").Replace("%2^%", ope.TranslatedParams[0]);
                }
                else
                {
                    sTranslation = sTranslation.Replace("%2^%", ope.TranslatedParams[0]);
                }
            }

            return sTranslation;
        }

        // Get Translation for Instructed Param
        private string getRegisterTranslation(string instructedParam, bool writeMode, ref SortedList slEecRegisters, ref SADS6x S6x)
        {
            // Possible values samples : 12, abcd, R12, [abcd], R12++, [R12], [R12+abcd], ...

            object[] arrPointersValues = null;
            string sTranslation = string.Empty;
            string sInstructedRegister = string.Empty;
            string sInstructedAddedRegister = string.Empty;
            bool bDualMode = false;
            bool bAdderMode = false;

            arrPointersValues = Tools.InstructionPointersValues(instructedParam);
            //  Returns 0,1 - 0 Not Pointer, 1 Pointer - Int
            //          Pointer / Value 1   - String format
            //          Pointer / Value 1   - Integer format
            //          Pointer / Value 2   - String if exists
            //          Pointer / Value 2   - Integer format if exists

            // Pointer / Value 2 is not managed for now

            // No Register Translation to do
            if (!(bool)arrPointersValues[0]) return instructedParam;

            // S6x Translation First to override other translations
            if (arrPointersValues.Length == 3)
            {
                sInstructedRegister = Tools.RegisterUniqueAddress((int)arrPointersValues[2]);
            }
            else
            {
                sInstructedRegister = Tools.RegisterUniqueAddress((int)arrPointersValues[2]) + SADDef.AdditionSeparator + arrPointersValues[4].ToString();
                sInstructedAddedRegister = Tools.RegisterUniqueAddress((int)arrPointersValues[4]);
                bDualMode = true;
            }

            S6xRegister s6xReg = (S6xRegister)S6x.slProcessRegisters[sInstructedRegister];
            if (s6xReg == null && bDualMode)
            {
                // 0x24 to 0x58 to keep only real adders
                // 0x100 to prevent basic registers to be calculated or shown for second part
                if ((int)arrPointersValues[2] >= 0x24 && (int)arrPointersValues[2] <= 0x58 && (int)arrPointersValues[4] >= 0x100)
                {
                    s6xReg = (S6xRegister)S6x.slProcessRegisters[sInstructedAddedRegister];
                    bAdderMode = true;
                }
            }

            if (s6xReg != null)
            {
                sTranslation = instructedParam;
                string regLabel = s6xReg.Label;
                if (s6xReg.MultipleMeanings)
                {
                    bool byteOpe = true;
                    switch (Type)
                    {
                        case OPCodeType.WordOP:
                            byteOpe = false;
                            break;
                        case OPCodeType.MixedOP:
                            byteOpe = (Instruction.ToLower() != "stw");
                            break;
                    }
                    regLabel = s6xReg.Labels(byteOpe);
                }
                if (regLabel != null && regLabel != string.Empty && regLabel != s6xReg.Address)
                {
                    if (bDualMode)
                    {
                        if (bAdderMode)
                        {
                            if (Tools.RegisterInstruction(arrPointersValues[3].ToString()).ToUpper() != regLabel.ToUpper())
                            {
                                sTranslation = instructedParam.Replace(SADDef.AdditionSeparator + arrPointersValues[3].ToString(), SADDef.AdditionSeparator + regLabel);
                            }
                        }
                        else
                        {
                            if (Tools.RegisterInstruction(arrPointersValues[1].ToString() + SADDef.AdditionSeparator + arrPointersValues[3].ToString()).ToUpper() != regLabel.ToUpper())
                            {
                                sTranslation = instructedParam.Replace(Tools.RegisterInstruction(arrPointersValues[1].ToString() + SADDef.AdditionSeparator + arrPointersValues[3].ToString()), SADDef.LongRegisterTemplate.Replace("%LREG%", regLabel));
                            }
                        }
                    }
                    else
                    {
                        if (Tools.RegisterInstruction(arrPointersValues[1].ToString()).ToUpper() != regLabel.ToUpper())
                        {
                            sTranslation = instructedParam.Replace(Tools.RegisterInstruction(arrPointersValues[1].ToString()), SADDef.LongRegisterTemplate.Replace("%LREG%", regLabel));
                        }
                    }
                }
                s6xReg = null;
                return sTranslation;
            }

            // Eec Register Param Calculation
            EecRegister eecRegister = (EecRegister)slEecRegisters[arrPointersValues[1].ToString()];
            if (eecRegister != null)
            {
                switch (eecRegister.Check)
                {
                    case EecRegisterCheck.ReadWrite:
                        if (writeMode) sTranslation = eecRegister.TranslationWriteByte;
                        else sTranslation = eecRegister.TranslationReadByte;
                        break;
                    case EecRegisterCheck.DataType:
                        switch (Type)
                        {
                            case OPCodeType.BitByteGotoOP:
                            case OPCodeType.ByteOP:
                                sTranslation = eecRegister.TranslationReadByte;
                                break;
                            case OPCodeType.WordOP:
                                sTranslation = eecRegister.TranslationReadWord;
                                break;
                            case OPCodeType.MixedOP:
                                if (writeMode) sTranslation = eecRegister.TranslationReadWord;
                                else sTranslation = eecRegister.TranslationReadByte;
                                break;
                            default: // Should never happen
                                sTranslation = eecRegister.TranslationReadByte;
                                break;
                        }
                        break;
                    case EecRegisterCheck.Both:
                        switch (Type)
                        {
                            case OPCodeType.BitByteGotoOP:
                            case OPCodeType.ByteOP:
                                if (writeMode) sTranslation = eecRegister.TranslationWriteByte;
                                else sTranslation = eecRegister.TranslationReadByte;
                                break;
                            case OPCodeType.WordOP:
                                if (writeMode) sTranslation = eecRegister.TranslationWriteWord;
                                else sTranslation = eecRegister.TranslationReadWord;
                                break;
                            case OPCodeType.MixedOP:
                                if (writeMode) sTranslation = eecRegister.TranslationWriteWord;
                                else sTranslation = eecRegister.TranslationReadByte;
                                break;
                            default: // Should never happen
                                sTranslation = eecRegister.TranslationReadByte;
                                break;
                        }
                        break;
                    default:
                        sTranslation = eecRegister.TranslationReadByte;
                        break;
                }
                sTranslation = instructedParam.Replace(eecRegister.InstructionTrans, sTranslation);
                eecRegister = null;
                return sTranslation;
            }

            // KAM / CC / EC Register ranges - 8061 only
            //  Default Translation for Special Registers
            if (is8061)
            {
                if ((int)arrPointersValues[2] >= SADDef.KAMRegisters8061MinAdress && (int)arrPointersValues[2] <= SADDef.KAMRegisters8061MaxAdress)
                {
                    return instructedParam.Replace(Tools.RegisterInstruction(arrPointersValues[1].ToString()), Tools.PointerTranslation(SADDef.KAMRegister8061Template.Replace("%LREG%", arrPointersValues[1].ToString())));
                }
                if ((int)arrPointersValues[2] >= SADDef.CCRegisters8061MinAdress && (int)arrPointersValues[2] <= SADDef.CCRegisters8061MaxAdress)
                {
                    return instructedParam.Replace(Tools.RegisterInstruction(arrPointersValues[1].ToString()), Tools.PointerTranslation(SADDef.CCRegister8061Template.Replace("%LREG%", arrPointersValues[1].ToString())));
                }
                if ((int)arrPointersValues[2] >= SADDef.ECRegisters8061MinAdress && (int)arrPointersValues[2] <= SADDef.ECRegisters8061MaxAdress)
                {
                    return instructedParam.Replace(Tools.RegisterInstruction(arrPointersValues[1].ToString()), Tools.PointerTranslation(SADDef.ECRegister8061Template.Replace("%LREG%", arrPointersValues[1].ToString())));
                }
            }
            
            // No Translation
            return instructedParam;
        }
    }
}
