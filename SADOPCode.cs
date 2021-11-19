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
        // 20210406 - PYM - New information now at OPCode level too
        public bool isEarly = false;
        public bool isPilot = false;
        public bool is8065SingleBank = false;

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

        public bool Apply0x100RegisterShortcut = false;
        public bool Apply0x100RegisterSFRShortcut = false;

        private object[] OPCHeaderDef = null;
        private object[] OPCDef = null;

        private bool hasJump = false;
        private bool hasParts = false;

        private int shortJumpAdder = 0;

        private SortedList CarryTranslations = null;

        // 20210406 - PYM - New information now at OPCode level too
        //public SADOPCode(int iOPCode, bool bIs8061)
        public SADOPCode(int iOPCode, bool bIs8061, bool bIsEarly, bool bIsPilot, bool bIs8065SingleBank)
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
            // 20210406 - PYM - New information now at OPCode level too
            isEarly = bIsEarly;
            isPilot = bIsPilot;
            is8065SingleBank = bIs8065SingleBank;

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
                    CarryMode carryMode = CarryMode.Default;
                    switch (sTrans[0].ToLower())
                    {
                        case "default":
                            carryMode = CarryMode.Default; 
                            break;
                        case "cy":
                            carryMode = CarryMode.Carry;
                            break;
                        case "cmp":
                            carryMode = CarryMode.Comparison;
                            break;
                        case "-":
                            carryMode = CarryMode.Substract;
                            break;
                        case "+w":
                            carryMode = CarryMode.AddWords;
                            break;
                        case "+b":
                            carryMode = CarryMode.AddBytes;
                            break;
                        case "*w":
                            carryMode = CarryMode.MultiplyWords;
                            break;
                        case "*b":
                            carryMode = CarryMode.MultiplyBytes;
                            break;
                    }
                    CarryTranslations.Add(carryMode, sTrans[1]);
                }
                defTranslations = null;
            }
        }

        public Operation processOP(int opAddress, int callBankNum, int callAddress, string[] arrOP, ref SADCalib Calibration, bool applySignedAlt, ref Operation opPrevResult, ref GotoOpParams gopParams, ref SADS6x S6x)
        {
            Operation ope = null;
            int opParams = 0;
            bool bVariableParamsEnabled = false;
            bool bParamPartsProcessed = false;
            string sReg = string.Empty;
            int iValue = -1;
            ArrayList arrCleanedParams = null;

            ope = new Operation(callBankNum, opAddress);
            ope.OriginalOPCode = OPCode;
            ope.OriginalInstruction = Instruction;
            
            ope.ApplyOnBankNum = callBankNum;
            ope.SetBankNum = callBankNum;
            ope.ReadDataBankNum = Calibration.BankNum;
            ope.InitialCallAddressInt = callAddress;

            // 20200409 - PYM - Creation Parameters to get a backup and to be able to reprocess operation entirely
            ope.OPCode = this;
            ope.ApplySignedAlt = applySignedAlt;
            ope.initialOpPrevResult = opPrevResult;
            ope.initialGotoOpParams = gopParams == null ? null : gopParams.Clone();

            // External Bank Call or Jump Management
            // 20171110 Move from applyJumps to be applied on all Ops especially the one using pointers and the jump ones
            if (opPrevResult != null)
            {
                if (opPrevResult.OriginalOPCode == "10")
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

            ope.OperationParams = new OperationParam[opParams];
            for (int iParam = 0; iParam < ope.OperationParams.Length; iParam++)
            {
                ope.OperationParams[iParam] = new OperationParam();
                ope.OperationParams[iParam].InstructedParam = arrOP[iParam + ope.BytesNumber - opParams];
                // 20210407 - PYM - New OriginalInstructedParam Property
                ope.OperationParams[iParam].OriginalInstructedParam = ope.OperationParams[iParam].InstructedParam;
            }

            // Specific cases when no provided parameters
            switch (OPCode.ToLower())
            {
                // Skip
                case "00":
                    ope.OperationParams = new OperationParam[1];
                    ope.OperationParams[0] = new OperationParam();
                    ope.OperationParams[0].InstructedParam = "01";
                    // 20210407 - PYM - New OriginalInstructedParam Property
                    ope.OperationParams[0].OriginalInstructedParam = ope.OperationParams[0].InstructedParam;
                    break;
            }

            bParamPartsProcessed = false;
            for (int iParam = 0; iParam < ope.OperationParams.Length; iParam++)
            {
                // Predefined Parameters for OP Code are not in line for variable parameters (should be 1 less), the last value is used in this case.
                int iParamValue = 0;
                int iParamForParameters = iParam;
                if (iParamForParameters >= parameters.Length) iParamForParameters = parameters.Length - 1;
                switch (parameters[iParamForParameters].Type)
                {
                    case OPCodeParamsTypes.Bank:
                        ope.SetBankNum = Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16);
                        ope.OperationParams[iParam].InstructedParam = ope.SetBankNum.ToString();
                        break;
                    case OPCodeParamsTypes.AddressRelativePosition:
                        if (parameters[iParamForParameters].isPointer) ope.OperationParams[iParam].InstructedParam = Tools.PointerTranslation(ope.OperationParams[iParam].InstructedParam);
                        break;
                    case OPCodeParamsTypes.RegisterByte:
                    case OPCodeParamsTypes.RegisterWord:
                        if (ope.OperationParams[iParam].InstructedParam == "00")
                        {
                            ope.OperationParams[iParam].InstructedParam = "0";
                            break;
                        }
                        // Register auto Increment for Pointers
                        //  Multiple of 2 (B0 = 0), this is a direct register, no increment.
                        //                (B0 = 1), 1 has to be removed from this register, increment is applied.
                        if (parameters[iParamForParameters].isPointer)
                        {
                            iParamValue = Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16);
                            if (iParamValue % 2 != 0) ope.OperationParams[iParam].InstructedParam = string.Format("{0:x2}" + SADDef.IncrementSuffix, iParamValue - 1);
                            ope.OperationParams[iParam].InstructedParam = SADDef.ShortRegisterPrefix + ope.OperationParams[iParam].InstructedParam;
                            if (parameters[iParamForParameters].isPointer) ope.OperationParams[iParam].InstructedParam = Tools.PointerTranslation(ope.OperationParams[iParam].InstructedParam);
                            break;
                        }
                        // 20200612 - PYM    
                        // Odd Word / Shortcut +0x100 for Word Instructions on first parameter
                        // 20200711 - PYM
                        // Tested on 8061, it only applies to 8065 starting on step D
                        // 20200813 - PYM
                        // Extended to all parameters
                        // 20210406 - PYM
                        // Now managed by booleans
                        if (parameters[iParamForParameters].Type == OPCodeParamsTypes.RegisterWord)
                        {
                            iParamValue = Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16);
                            if (iParamValue % 2 != 0)
                            {
                                ope.OperationParams[iParam].addPossibleStatistic(StatisticsRegisterItems.RegShortcut0x100);
                                if (Apply0x100RegisterShortcut || Apply0x100RegisterSFRShortcut)
                                {
                                    if ((iParamValue > 0x23 && Apply0x100RegisterShortcut) || (iParamValue <= 0x23 && Apply0x100RegisterSFRShortcut))
                                    {
                                        ope.OperationParams[iParam].InstructedParam = string.Format("{0:x3}", 0x100 + iParamValue - 1);
                                    }
                                }
                            }
                            ope.OperationParams[iParam].InstructedParam = Tools.RegisterInstruction(ope.OperationParams[iParam].InstructedParam);
                            break;
                        }
                        // Default
                        ope.OperationParams[iParam].InstructedParam = SADDef.ShortRegisterPrefix + ope.OperationParams[iParam].InstructedParam;
                        break;
                    case OPCodeParamsTypes.ValueByte:
                        // 20171118 - Remove Leading 0
                        ope.OperationParams[iParam].InstructedParam = Convert.ToString(Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16), 16);
                        if (parameters[iParamForParameters].isPointer) ope.OperationParams[iParam].InstructedParam = Tools.PointerTranslation(ope.OperationParams[iParam].InstructedParam);
                        break;
                    case OPCodeParamsTypes.ValueWordPart:
                    case OPCodeParamsTypes.AddressPartRelativePosition:
                    case OPCodeParamsTypes.AddressPartAbsolutePosition:
                        if (!bParamPartsProcessed)
                        {
                            if (hasVariableParams && bVariableParamsEnabled)
                            {
                                sReg = string.Format("{0:x2}", Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16) - 1);
                                if (sReg == "00") sReg = string.Empty;
                                else sReg = Tools.RegisterInstruction(sReg);
                                iValue = Convert.ToInt32(ope.OperationParams[iParam + 2].InstructedParam + ope.OperationParams[iParam + 1].InstructedParam, 16);
                                if (sReg == string.Empty && iValue == 0) ope.OperationParams[iParam].InstructedParam = "0";
                                else if (sReg == string.Empty) ope.OperationParams[iParam].InstructedParam = Convert.ToString(iValue, 16);
                                else if (iValue == 0) ope.OperationParams[iParam].InstructedParam = sReg;
                                else ope.OperationParams[iParam].InstructedParam = sReg + SADDef.AdditionSeparator + Convert.ToString(iValue, 16);
                                if (parameters[iParamForParameters].isPointer) ope.OperationParams[iParam].InstructedParam = Tools.PointerTranslation(ope.OperationParams[iParam].InstructedParam);
                                ope.OperationParams[iParam + 1].InstructedParam = string.Empty;
                                ope.OperationParams[iParam + 2].InstructedParam = string.Empty;

                                // 20210407 - PYM - New OriginalInstructedParam Property
                                ope.OperationParams[iParam].OriginalInstructedParam = ope.OperationParams[iParam].InstructedParam;

                                iParam += 2;
                            }
                            else if (hasVariableParams)
                            // First Instructed Param is a register, the other one a value to be added
                            {
                                sReg = ope.OperationParams[iParam].InstructedParam;
                                if (sReg == "00") sReg = string.Empty;
                                else sReg = Tools.RegisterInstruction(sReg);
                                iValue = Convert.ToInt32(ope.OperationParams[iParam + 1].InstructedParam, 16);
                                if (sReg == string.Empty && iValue == 0) ope.OperationParams[iParam].InstructedParam = "0";
                                else if (sReg == string.Empty) ope.OperationParams[iParam].InstructedParam = Convert.ToString(iValue, 16);
                                else if (iValue == 0) ope.OperationParams[iParam].InstructedParam = sReg;
                                else ope.OperationParams[iParam].InstructedParam = sReg + SADDef.AdditionSeparator + Convert.ToString(iValue, 16);
                                if (parameters[iParamForParameters].isPointer) ope.OperationParams[iParam].InstructedParam = Tools.PointerTranslation(ope.OperationParams[iParam].InstructedParam);
                                ope.OperationParams[iParam + 1].InstructedParam = string.Empty;

                                // 20210407 - PYM - New OriginalInstructedParam Property
                                ope.OperationParams[iParam].OriginalInstructedParam = ope.OperationParams[iParam].InstructedParam;

                                iParam++;
                            }
                            else
                            {
                                ope.OperationParams[iParam].InstructedParam = Convert.ToString(Convert.ToInt32(ope.OperationParams[iParam + 1].InstructedParam + ope.OperationParams[iParam].InstructedParam, 16), 16);
                                if (parameters[iParamForParameters].isPointer) ope.OperationParams[iParam].InstructedParam = Tools.PointerTranslation(ope.OperationParams[iParam].InstructedParam);
                                ope.OperationParams[iParam + 1].InstructedParam = string.Empty;

                                // 20210407 - PYM - New OriginalInstructedParam Property
                                ope.OperationParams[iParam].OriginalInstructedParam = ope.OperationParams[iParam].InstructedParam;
                                
                                iParam++;
                            }
                        }
                        bParamPartsProcessed = true; ;
                        break;
                }
            }

            arrCleanedParams = new ArrayList();
            foreach (OperationParam opeParam in ope.OperationParams)
            {
                if (opeParam.InstructedParam != string.Empty) arrCleanedParams.Add(opeParam);
            }
            ope.OperationParams = (OperationParam[])arrCleanedParams.ToArray(typeof(OperationParam));
            arrCleanedParams = null;

            applyJumps(ref ope, ref gopParams, ref Calibration);

            foreach (OperationParam opeParam in ope.OperationParams)
            {
                opeParam.CalculatedParam = opeParam.InstructedParam;
                opeParam.DefaultTranslatedParam = opeParam.InstructedParam;
            }
            
            ope.IgnoredTranslatedParam = -1;

            applyRbaseRconst(ref ope, ref Calibration, ref S6x, applySignedAlt);
            applyRegisters(ref ope, ref Calibration, ref S6x);
            applyShifting(ref ope, ref Calibration);
            applyGotoOpParams(ref ope, ref gopParams);

            if (applySignedAlt)
            {
                ope.AddressInt--;
                ope.BytesNumber++;
                string[] tmpOpArr = new string[ope.OriginalOpArr.Length + 1];
                tmpOpArr[0] = SADDef.OPCSigndAltOpCode;
                for (int iOp = 0; iOp < ope.OriginalOpArr.Length; iOp++) tmpOpArr[iOp + 1] = ope.OriginalOpArr[iOp];
                ope.OriginalOpArr = new string[tmpOpArr.Length];
                for (int iOp = 0; iOp < ope.OriginalOpArr.Length; iOp++) ope.OriginalOpArr[iOp] = tmpOpArr[iOp];
            }

            ope.isReturn = false;
            switch (Instruction.ToLower())
            {
                case "ret":
                case "reti":
                    ope.isReturn = true;
                    // Goto Op Params coming from last compatible Op
                    // Trace stored on Return instruction to provide params to multiple call/scall
                    if (gopParams == null) ope.GotoOpParams = new GotoOpParams(ope, ope.BankNum, ope.AddressInt);
                    else ope.GotoOpParams = gopParams.Clone();
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
                        applyRegisters(ref ope, ref Calibration, ref S6x);
                        applyShifting(ref ope, ref Calibration);
                        return;
                    }
                }
            }
        }

        // Post Process Op Call Args with translation
        public void postProcessOpCallArgs(ref Operation ope, ref SADCalib Calibration, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x)
        {
            ArrayList alCallArgsParams = null;
            CallArgsParam cArgsParam = null;
            RBase rBase = null;
            SADBank readDataBank = null;
            CalibrationElement opeCalElem = null;
            int usedArgs = -1;
            int rBaseNum = -1;
            int rBaseAdder = -1;
            int structArgAddress = -1;
            int structRegAddress = -1;
            string translatedArg = string.Empty;
            string defaultTranslation = string.Empty;

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

            alCallArgsParams = new ArrayList();

            // Args Mode decoding to product Elements & Structures
            usedArgs = 0;
            foreach (CallArgument cArg in ope.CallArguments)
            {
                if (cArg.ByteSize + usedArgs > ope.CallArgsArr.Length) break;
                
                if (cArg.Word)
                // Word Argument - Elements Addresses are Word Arguments Mode0 to Mode4
                {
                    bool outputAsPointer = (cArg.S6xRoutineInputArgument == null) ? true : cArg.S6xRoutineInputArgument.Pointer;

                    switch (cArg.Mode)
                    {
                        case CallArgsMode.Mode0:
                            // Direct Element without RBase
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16) - SADDef.EecBankStartAddress;
                            cArg.DecryptedValueInt = cArg.InputValueInt + SADDef.EecBankStartAddress;

                            if (Calibration.BankNum == ope.ReadDataBankNum && Calibration.isCalibrationAddress(cArg.InputValueInt) && !(Calibration.Info.is8061 && Calibration.Info.isEarly))
                            {
                                opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(ope.ReadDataBankNum, cArg.InputValueInt)];
                                if (opeCalElem == null)
                                {
                                    opeCalElem = new CalibrationElement(ope.ReadDataBankNum, string.Empty);
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
                                    Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                                }

                                if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                ope.alCalibrationElems.Add(opeCalElem);
                                if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                cArgsParam = new CallArgsParam();
                                cArgsParam.EmbeddedParam = opeCalElem;
                                cArgsParam.DefaultTranslatedParam = opeCalElem.Address;
                                alCallArgsParams.Add(cArgsParam);
                                cArgsParam = null;
                                opeCalElem = null;
                            }
                            else
                            {
                                ope.OtherElemAddress = cArg.DecryptedValue;
                                ope.KnownElemAddress = cArg.DecryptedValue;
                                cArgsParam = new CallArgsParam();
                                cArgsParam.EmbeddedParam = ope.OtherElemAddress;
                                cArgsParam.DefaultTranslatedParam = ope.OtherElemAddress;
                                alCallArgsParams.Add(cArgsParam);
                                cArgsParam = null;
                            }
                            break;
                        case CallArgsMode.Mode1:
                            // To be managed - Temporary managed as Standard
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16);
                            cArg.DecryptedValueInt = cArg.InputValueInt;

                            cArgsParam = new CallArgsParam();
                            if (outputAsPointer) cArgsParam.EmbeddedParam= Calibration.slRegisters[Tools.RegisterUniqueAddress(cArg.DecryptedValueInt)];
                            else cArgsParam.EmbeddedParam = cArg.DecryptedValueInt;
                            cArgsParam.DefaultTranslatedParam = outputAsPointer ? Tools.PointerTranslation(cArg.DecryptedValue) : cArg.DecryptedValue;
                            alCallArgsParams.Add(cArgsParam);
                            cArgsParam = null;
                            break;
                        case CallArgsMode.Mode2:
                            // To be managed - Temporary managed as Standard
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16);
                            cArg.DecryptedValueInt = cArg.InputValueInt;

                            cArgsParam = new CallArgsParam();
                            if (outputAsPointer) cArgsParam.EmbeddedParam = Calibration.slRegisters[Tools.RegisterUniqueAddress(cArg.DecryptedValueInt)];
                            else cArgsParam.EmbeddedParam = cArg.DecryptedValueInt;
                            cArgsParam.DefaultTranslatedParam = outputAsPointer ? Tools.PointerTranslation(cArg.DecryptedValue) : cArg.DecryptedValue;
                            alCallArgsParams.Add(cArgsParam);
                            cArgsParam = null;
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
                                cArgsParam = new CallArgsParam();
                                if (outputAsPointer)
                                {
                                    Register rReg = (Register)Calibration.slRegisters[Tools.RegisterUniqueAddress(cArg.DecryptedValueInt)];
                                    cArgsParam.EmbeddedParam = rReg;
                                    cArgsParam.DefaultTranslatedParam = getParamTranslation(rReg, Tools.RegisterInstruction(cArg.DecryptedValue), false);
                                }
                                else
                                {
                                    cArgsParam.EmbeddedParam = cArg.DecryptedValueInt;
                                    cArgsParam.DefaultTranslatedParam = cArg.DecryptedValue;
                                }
                                alCallArgsParams.Add(cArgsParam);
                                cArgsParam = null;
                            }
                            else
                            // Compatible Expression, this is a Calibration Element
                            {
                                rBaseNum = (((rBaseNum - 8) * 0x10) / 8) / 2;
                                rBaseAdder = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1].Substring(1, 1) + ope.CallArgsArr[usedArgs], 16);
                                if (rBaseNum < Calibration.slRbases.Count)
                                {
                                    rBase = ((RBase)Calibration.slRbases.GetByIndex(rBaseNum));
                                    opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(rBase.BankNum, rBase.AddressBankInt + rBaseAdder)];
                                    if (opeCalElem == null)
                                    {
                                        opeCalElem = new CalibrationElement(rBase.BankNum, rBase.Code);
                                        opeCalElem.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code + SADDef.AdditionSeparator + Convert.ToString(rBaseAdder, 16);
                                        opeCalElem.AddressInt = rBase.AddressBankInt + rBaseAdder;
                                        opeCalElem.AddressBinInt = opeCalElem.AddressInt + Calibration.BankAddressBinInt;
                                        Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                                    }
                                    
                                    if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                    ope.alCalibrationElems.Add(opeCalElem);
                                    if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                    rBase = null;

                                    cArg.DecryptedValueInt = opeCalElem.AddressInt + SADDef.EecBankStartAddress;

                                    cArgsParam = new CallArgsParam();
                                    cArgsParam.EmbeddedParam = opeCalElem;
                                    cArgsParam.DefaultTranslatedParam = opeCalElem.Address;
                                    alCallArgsParams.Add(cArgsParam);
                                    cArgsParam = null;
                                    opeCalElem = null;
                                }
                                else
                                {
                                    cArgsParam = new CallArgsParam();
                                    cArgsParam.EmbeddedParam = Calibration.slRegisters[Tools.RegisterUniqueAddress(cArg.DecryptedValueInt)];
                                    cArgsParam.DefaultTranslatedParam = outputAsPointer ? Tools.PointerTranslation(cArg.DecryptedValue) : cArg.DecryptedValue;
                                    alCallArgsParams.Add(cArgsParam);
                                    cArgsParam = null;
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
                                opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(rBase.BankNum, rBase.AddressBankInt + rBaseAdder)];
                                if (opeCalElem == null)
                                {
                                    opeCalElem = new CalibrationElement(rBase.BankNum, rBase.Code);
                                    opeCalElem.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code + SADDef.AdditionSeparator + Convert.ToString(rBaseAdder, 16);
                                    opeCalElem.AddressInt = rBase.AddressBankInt + rBaseAdder;
                                    opeCalElem.AddressBinInt = opeCalElem.AddressInt + Calibration.BankAddressBinInt;
                                    Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                                }

                                if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                ope.alCalibrationElems.Add(opeCalElem);
                                if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                rBase = null;

                                cArg.DecryptedValueInt = opeCalElem.AddressInt + SADDef.EecBankStartAddress;

                                cArgsParam = new CallArgsParam();
                                cArgsParam.EmbeddedParam = opeCalElem;
                                cArgsParam.DefaultTranslatedParam = opeCalElem.Address;
                                alCallArgsParams.Add(cArgsParam);
                                cArgsParam = null;
                                opeCalElem = null;
                            }
                            else
                            {
                                cArgsParam = new CallArgsParam();
                                cArgsParam.EmbeddedParam = Calibration.slRegisters[Tools.RegisterUniqueAddress(cArg.DecryptedValueInt)];
                                cArgsParam.DefaultTranslatedParam = outputAsPointer ? Tools.PointerTranslation(cArg.DecryptedValue) : cArg.DecryptedValue;
                                alCallArgsParams.Add(cArgsParam);
                                cArgsParam = null;
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
                                    opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(rBase.BankNum, rBase.AddressBankInt + rBaseAdder)];
                                    if (opeCalElem == null)
                                    {
                                        opeCalElem = new CalibrationElement(rBase.BankNum, rBase.Code);
                                        opeCalElem.RBaseCalc = SADDef.ShortRegisterPrefix + rBase.Code + SADDef.AdditionSeparator + Convert.ToString(rBaseAdder, 16);
                                        opeCalElem.AddressInt = rBase.AddressBankInt + rBaseAdder;
                                        opeCalElem.AddressBinInt = opeCalElem.AddressInt + Calibration.BankAddressBinInt;
                                        Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                                    }
                                    
                                    if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                    ope.alCalibrationElems.Add(opeCalElem);
                                    if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                    rBase = null;

                                    ope.CallArgsStructRegister = Tools.RegisterInstruction(translatedArg);

                                    cArgsParam = new CallArgsParam();
                                    cArgsParam.EmbeddedParam = opeCalElem;
                                    cArgsParam.DefaultTranslatedParam = opeCalElem.Address;
                                    alCallArgsParams.Add(cArgsParam);
                                    cArgsParam = null;

                                    cArgsParam = new CallArgsParam();
                                    cArgsParam.EmbeddedParam = Calibration.slRegisters[Tools.RegisterUniqueAddress(translatedArg)];
                                    cArgsParam.DefaultTranslatedParam = ope.CallArgsStructRegister;
                                    alCallArgsParams.Add(cArgsParam);
                                    cArgsParam = null;

                                    cArg.DecryptedValueInt = opeCalElem.AddressInt + SADDef.EecBankStartAddress;

                                    opeCalElem = null;
                                }
                                else
                                {
                                    cArgsParam = new CallArgsParam();
                                    cArgsParam.EmbeddedParam = cArg.DecryptedValue;
                                    cArgsParam.DefaultTranslatedParam = outputAsPointer ? Tools.PointerTranslation(cArg.DecryptedValue) : cArg.DecryptedValue;
                                    alCallArgsParams.Add(cArgsParam);
                                    cArgsParam = null;
                                }
                            }
                            else
                            {
                                cArgsParam = new CallArgsParam();
                                cArgsParam.EmbeddedParam = cArg.DecryptedValue;
                                cArgsParam.DefaultTranslatedParam = outputAsPointer ? Tools.PointerTranslation(cArg.DecryptedValue) : cArg.DecryptedValue;
                                alCallArgsParams.Add(cArgsParam);
                                cArgsParam = null;
                            }
                            readDataBank = null;
                            break;
                        default:
                            cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs + 1] + ope.CallArgsArr[usedArgs], 16);
                            cArg.DecryptedValueInt = cArg.InputValueInt;

                            if (outputAsPointer)
                            {
                                cArgsParam = new CallArgsParam();
                                cArgsParam.EmbeddedParam = Calibration.slRegisters[Tools.RegisterUniqueAddress(cArg.DecryptedValueInt)];
                                cArgsParam.DefaultTranslatedParam = getParamTranslation(cArgsParam.EmbeddedParam, Tools.RegisterInstruction(cArg.DecryptedValue), false);
                                alCallArgsParams.Add(cArgsParam);
                                cArgsParam = null;
                            }
                            else
                            {
                                cArgsParam = new CallArgsParam();
                                cArgsParam.EmbeddedParam = cArg.DecryptedValueInt;
                                cArgsParam.DefaultTranslatedParam = cArg.DecryptedValue;
                                alCallArgsParams.Add(cArgsParam);
                                cArgsParam = null;
                            }
                            break;
                    }
                }
                else
                // Byte Argument
                {
                    cArg.InputValueInt = Convert.ToInt32(ope.CallArgsArr[usedArgs], 16);
                    cArg.DecryptedValueInt = cArg.InputValueInt;
                    cArgsParam = new CallArgsParam();
                    cArgsParam.EmbeddedParam = cArg.DecryptedValueInt;
                    cArgsParam.DefaultTranslatedParam = cArg.DecryptedValue;
                    alCallArgsParams.Add(cArgsParam);
                    cArgsParam = null;
                }
                usedArgs += cArg.ByteSize;
            }

            ope.CallArgsParams = (CallArgsParam[])alCallArgsParams.ToArray(typeof(CallArgsParam));
            alCallArgsParams = null;
        }

        private void applyJumps(ref Operation ope, ref GotoOpParams gopParams, ref SADCalib Calibration)
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
                if (gopParams == null) ope.GotoOpParams = new GotoOpParams(ope, ope.BankNum, ope.AddressInt);
                else ope.GotoOpParams = gopParams.Clone();
            }

            for (int iParam = 0; iParam < ope.OperationParams.Length; iParam++)
            {
                if (parameters[iParam].Type == OPCodeParamsTypes.AddressRelativePosition)
                {
                    if (ope.CallType == CallType.ShortCall || ope.CallType == CallType.ShortJump)
                    {
                        ope.AddressJumpInt = ope.AddressNextInt + Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16) + shortJumpAdder;
                    }
                    else
                    {
                        // Signed Jump
                        ope.AddressJumpInt = ope.AddressNextInt + Convert.ToSByte(ope.OperationParams[iParam].InstructedParam, 16) + shortJumpAdder;
                    }
                    ope.OperationParams[iParam].InstructedParam = ope.AddressJump;
                    // Long Word Result happens
                    if (ope.OperationParams[iParam].InstructedParam.Length > 4)
                    {
                        ope.OperationParams[iParam].InstructedParam = ope.OperationParams[iParam].InstructedParam.Substring(ope.OperationParams[iParam].InstructedParam.Length - 4, 4);
                        ope.AddressJumpInt = Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16) - SADDef.EecBankStartAddress;
                    }
                    break;
                }
                else if (parameters[iParam].Type == OPCodeParamsTypes.AddressPartRelativePosition)
                {
                    ope.AddressJumpInt = ope.AddressNextInt + Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16);
                    ope.OperationParams[iParam].InstructedParam = ope.AddressJump;
                    // Long Word Result happens
                    if (ope.OperationParams[iParam].InstructedParam.Length > 4)
                    {
                        ope.OperationParams[iParam].InstructedParam = ope.OperationParams[iParam].InstructedParam.Substring(ope.OperationParams[iParam].InstructedParam.Length - 4, 4);
                        ope.AddressJumpInt = Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16) - SADDef.EecBankStartAddress;
                    }
                    break;
                }
                else if (parameters[iParam].Type == OPCodeParamsTypes.AddressPartAbsolutePosition)
                {
                    ope.AddressJumpInt = Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16) - SADDef.EecBankStartAddress;
                    ope.OperationParams[iParam].InstructedParam = ope.AddressJump;
                    // Long Word Result happens
                    if (ope.OperationParams[iParam].InstructedParam.Length > 4)
                    {
                        ope.OperationParams[iParam].InstructedParam = ope.OperationParams[iParam].InstructedParam.Substring(ope.OperationParams[iParam].InstructedParam.Length - 4, 4);
                        ope.AddressJumpInt = Convert.ToInt32(ope.OperationParams[iParam].InstructedParam, 16) - SADDef.EecBankStartAddress;
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
            int opeCalElemAddressInt = -1;

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
                    arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[1].CalculatedParam);

                    // No Register to manage
                    if (!(bool)arrPointersValues[0]) break;

                    rBase = (RBase)Calibration.slRbases[arrPointersValues[1].ToString()];
                    if (rBase != null)
                    {
                        opeCalElemAddressInt = rBase.AddressBankInt + Convert.ToInt32(ope.OperationParams[0].CalculatedParam, 16);
                        opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(Calibration.BankNum, opeCalElemAddressInt)];
                        if (opeCalElem == null)
                        {
                            opeCalElem = new CalibrationElement(Calibration.BankNum, rBase.Code);
                            opeCalElem.RBaseCalc = ope.OperationParams[1].DefaultTranslatedParam + SADDef.AdditionSeparator + ope.OperationParams[0].DefaultTranslatedParam;
                            opeCalElem.AddressInt = opeCalElemAddressInt;
                            opeCalElem.AddressBinInt = opeCalElem.AddressInt + Calibration.BankAddressBinInt;
                            Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                        }

                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                        ope.alCalibrationElems.Add(opeCalElem);
                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress)) opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);

                        ope.OperationParams[1].EmbeddedParam = opeCalElem;
                        ope.OperationParams[1].CalculatedParam = opeCalElem.Address;
                        ope.OperationParams[1].DefaultTranslatedParam = opeCalElem.Address;
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
                            ope.OperationParams[1].EmbeddedParam = rConst;
                            ope.OperationParams[1].CalculatedParam = rConst.Value;
                            ope.OperationParams[1].DefaultTranslatedParam = rConst.Value;
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
                    // 45,fb,00,30,48       ad3w  R48,R30,fb         R48 = R30 + fb;      
                    // 64,82,48             ad2w  R48,R82            R48 += R82;          
                    // b2,48,34             ldb   R34,[R48]          R34 = [R48];         // R48 is a structure address 
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
                    ope.OtherElemAddress = ope.OperationParams[0].InstructedParam;
                    ope.KnownElemAddress = ope.OperationParams[0].InstructedParam;
                    // Now manages Register Addresses between 0xf000 & 0xffff
                    if (Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress < 0 || Convert.ToInt32(ope.OtherElemAddress, 16) >= 0xf000 && Convert.ToInt32(ope.OtherElemAddress, 16) <= 0xffff) ope.OtherElemAddress = string.Empty;
                    if (ope.KnownElemAddress.Length < 4) ope.KnownElemAddress = string.Empty;
                    if (ope.OtherElemAddress != string.Empty) ope.OperationParams[0].EmbeddedParam = ope.OtherElemAddress;
                    break;
                default:
                    //20180428
                    //for (int iParam = 0; iParam < ope.TranslatedParams.Length; iParam++)
                    for (int iParam = 0; iParam < ope.OperationParams.Length; iParam++)
                    {
                        //20180428
                        //sParamCalc = ope.TranslatedParams[iParam];
                        sParamCalc = ope.OperationParams[iParam].CalculatedParam;
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
                                    opeCalElemAddressInt = rBase.AddressBankInt + (int)arrPointersValues[4];
                                    if (opeCalElemAddressInt >= rBase.AddressBankInt && opeCalElemAddressInt <= rBase.AddressBankEndInt)
                                    {
                                        opeCalElem = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(Calibration.BankNum, opeCalElemAddressInt)];
                                        if (opeCalElem == null)
                                        {
                                            opeCalElem = new CalibrationElement(Calibration.BankNum, rBase.Code);
                                            opeCalElem.RBaseCalc = Tools.RegisterInstruction(rBase.Code) + SADDef.AdditionSeparator + arrPointersValues[3].ToString();
                                            opeCalElem.AddressInt = opeCalElemAddressInt;
                                            opeCalElem.AddressBinInt = opeCalElem.AddressInt + Calibration.BankAddressBinInt;
                                            Calibration.slCalibrationElements.Add(opeCalElem.UniqueAddress, opeCalElem);
                                        }

                                        if (ope.alCalibrationElems == null) ope.alCalibrationElems = new ArrayList();
                                        ope.alCalibrationElems.Add(opeCalElem);

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

                                        ope.OperationParams[iParam].EmbeddedParam = opeCalElem;
                                        ope.OperationParams[iParam].CalculatedParam = Tools.PointerTranslation(opeCalElem.Address);
                                        ope.OperationParams[iParam].DefaultTranslatedParam = Tools.PointerTranslation(opeCalElem.Address);

                                        if (!opeCalElem.RelatedOpsUniqueAddresses.Contains(ope.UniqueAddress))
                                        {
                                            opeCalElem.RelatedOpsUniqueAddresses.Add(ope.UniqueAddress);
                                        }

                                        rBase = null;
                                        opeCalElem = null;
                                    }
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

                                        ope.OperationParams[iParam].CalculatedParam = Tools.PointerTranslation(sParamCalc);
                                        ope.OperationParams[iParam].DefaultTranslatedParam = Tools.PointerTranslation(sParamCalc);

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
                                        if (ope.OtherElemAddress != string.Empty) ope.OperationParams[iParam].EmbeddedParam = ope.OtherElemAddress;
                                        else ope.OperationParams[iParam].EmbeddedParam = Convert.ToInt32(sParamCalc, 16);
                                    }
                                    rConst = null;
                                    break;
                                }
                            }
                            else
                            {
                                // 20200827 - RConst can be directly used without addition separator - R32 = [Rd6]; Rd6 is a RConst => R32 = [380];
                                //              Instructed Param starts with [R and RConst is always 2 chars so, 5 chars length.
                                if (ope.OperationParams[iParam].InstructedParam.StartsWith(SADDef.LongRegisterPointerPrefix + SADDef.ShortRegisterPrefix) && ope.OperationParams[iParam].InstructedParam.Length == 5)
                                {
                                    rConst = (RConst)Calibration.slRconst[arrPointersValues[1].ToString()];
                                    if (rConst != null)
                                    {
                                        if (rConst.isValue)
                                        {
                                            ope.OperationParams[iParam].CalculatedParam = Tools.PointerTranslation(rConst.Value);
                                            ope.OperationParams[iParam].DefaultTranslatedParam = Tools.PointerTranslation(rConst.Value);
                                            ope.OperationParams[iParam].EmbeddedParam = rConst.ValueInt;
                                        }
                                        rConst = null;
                                        break;
                                    }
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
                            if (ope.OtherElemAddress != string.Empty) ope.OperationParams[iParam].EmbeddedParam = ope.OtherElemAddress;
                        }
                    }
                    break;
            }
        }

        // Registers Translation applied on Operation Translated Params
        private void applyRegisters(ref Operation ope, ref SADCalib Calibration, ref SADS6x S6x)
        {
            for (int iParam = 0; iParam < ope.OperationParams.Length; iParam++)
            {
                object[] arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[iParam].CalculatedParam);
                //  Returns 0,1 - 0 Not Pointer, 1 Pointer - Int
                //          Pointer / Value 1   - String format
                //          Pointer / Value 1   - Integer format
                //          Pointer / Value 2   - String if exists
                //          Pointer / Value 2   - Integer format if exists

                // Pointer / Value 2 is not managed for registers

                // No Register to manage
                if (!(bool)arrPointersValues[0])
                {
                    ope.OperationParams[iParam].DefaultTranslatedParam = ope.OperationParams[iParam].CalculatedParam;
                    continue;
                }

                // 20200816
                // No Register to manage - Probably Calibartion Element or Other Element
                if (ope.OperationParams[iParam].EmbeddedParam != null)
                {
                    // For non null EmbeddedParam, only type int can be register
                    //      String is for Other Element
                    if (ope.OperationParams[iParam].EmbeddedParam.GetType() != typeof(int))
                    {
                        continue;
                    }
                }

                object regParam = null;
                object regParam2 = null;
                // Embedded Params Register linking
                if (arrPointersValues.Length == 3)
                {
                    regParam = Calibration.slRegisters[Tools.RegisterUniqueAddress((int)arrPointersValues[2])];
                    if (regParam != null) ope.OperationParams[iParam].EmbeddedParam = regParam;
                }
                else
                {
                    regParam = Calibration.slRegisters[Tools.RegisterUniqueAddress((int)arrPointersValues[2]) + SADDef.AdditionSeparator + arrPointersValues[4].ToString()];
                    if (regParam != null)
                    {
                        ope.OperationParams[iParam].EmbeddedParam = regParam;
                    }
                    else
                    {
                        // 20210909 - PYM - Second Part Register mngt
                        //ope.OperationParams[iParam].EmbeddedParam = new object[] { Calibration.slRegisters[Tools.RegisterUniqueAddress((int)arrPointersValues[2])], arrPointersValues[3] };

                        regParam = Calibration.slRegisters[Tools.RegisterUniqueAddress((int)arrPointersValues[2])];
                        bool isRegParamRBase = false;
                        if (regParam != null) if (((Register)regParam).RBase != null) isRegParamRBase = true;

                        // Added Second part is a register
                        // If first part is not a RBase register and if addresses match
                        if (!isRegParamRBase && ((int)arrPointersValues[4] >= 0x0 || (int)arrPointersValues[4] <= 0x2000 || (!is8061 && (int)arrPointersValues[4] >= 0xf000 && (int)arrPointersValues[4] <= 0xffff)))
                        {
                            regParam2 = Calibration.slRegisters[Tools.RegisterUniqueAddress((int)arrPointersValues[4])];
                            if (regParam2 != null)
                            {
                                // 20210921 - PYM - Not for EecRegisters
                                if (((Register)regParam2).EecRegister != null) regParam2 = arrPointersValues[3];
                            }
                        }
                        else
                        {
                            regParam2 = arrPointersValues[3];
                        }
                        ope.OperationParams[iParam].EmbeddedParam = new object[] { regParam, regParam2 };
                    }
                }
                regParam2 = null;
                regParam = null;
                
                // 8061 KAM / CC / EC Additional Detection and Flags
                //if (is8061) set8061KamCcEc(ref ope, ope.OperationParams[iParam].CalculatedParam);
                if (is8061)
                {
                    if (ope.OperationParams[iParam].EmbeddedParam != null)
                    {
                        if (ope.OperationParams[iParam].EmbeddedParam.GetType() == typeof(Register))
                        {
                            if (((Register)ope.OperationParams[iParam].EmbeddedParam).is8061KAMRegister) ope.isKamRelated = true;
                            if (((Register)ope.OperationParams[iParam].EmbeddedParam).is8061CCRegister) ope.isCcRelated = true;
                            if (((Register)ope.OperationParams[iParam].EmbeddedParam).is8061ECRegister) ope.isEcRelated = true;
                        }
                    }
                }

                // For initialization
                ope.OperationParams[iParam].DefaultTranslatedParam = getParamTranslation(ope.OperationParams[iParam].EmbeddedParam, ope.OperationParams[iParam].CalculatedParam, (iParam == ope.OperationParams.Length - 1));
            }
        }

        // Byte shifting
        private void applyShifting(ref Operation ope, ref SADCalib Calibration)
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
            int byteValue = Convert.ToInt32(ope.OperationParams[0].CalculatedParam, 16);
            if (byteValue > 16)
            {
                ope.OperationParams[0].EmbeddedParam = Calibration.slRegisters[Tools.RegisterUniqueAddress(byteValue)];
                ope.OperationParams[0].DefaultTranslatedParam = Tools.RegisterInstruction(Convert.ToString(byteValue, 16));
            }
            else
            {
                ope.OperationParams[0].EmbeddedParam = (int)Math.Pow(2, byteValue);
                ope.OperationParams[0].DefaultTranslatedParam = Convert.ToString((int)ope.OperationParams[0].EmbeddedParam, 16);
            }
        }

        // Registers Translation applied on Operation Translated Params
        //  20200618 - PYM - Reviewed
        private void applyGotoOpParams(ref Operation ope, ref GotoOpParams gopParams)
        {
            // GotoOpParams are updated only by non Goto operations
            if (ope.CallType != CallType.Unknown) return;

            // 20210630 - PYM
            // GotoOpParams are updated only for some deterministics OpCodes
            switch (Instruction.ToLower())
            {
                case "rbnk":
                case "bank0":
                case "bank1":
                case "bank2":
                case "bank3":
                case "di":
                case "ei":
                case "ret":
                case "reti":
                case "push":
                case "pop":
                case "pushp":
                case "popp":
                case "ldb":
                case "ldw":
                case "ldzbw":
                case "ldsbw":
                case "stb":
                case "stw":
                    return;
            }

            if (gopParams == null) gopParams = new GotoOpParams(ope, ope.BankNum, ope.AddressInt);

            gopParams.Ope = ope;
            gopParams.OpeBankNum = ope.BankNum;
            gopParams.OpeAddressInt = ope.AddressInt;
            gopParams.OpeReversedMeaning = false;

            bool foundElement = false;            
            if (ope.alCalibrationElems != null)
            {
                foreach (CalibrationElement opeCalElem in ope.alCalibrationElems)
                {
                    gopParams.ElemBankNum = opeCalElem.BankNum;
                    gopParams.ElemAddressInt = opeCalElem.AddressInt;
                    foundElement = true;
                    // First Calibration Element is enough for this process
                    break;
                }
            }
            if (!foundElement && ope.OtherElemAddress != string.Empty)
            {
                gopParams.ElemBankNum = ope.ApplyOnBankNum;
                gopParams.ElemAddressInt = Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress;
            }

            // 20210630 - PYM
            // GotoOpParams are now managing specific cases on instructions
            switch (Instruction.ToLower())
            {
                case "sb2b":
                case "sb2w":
                case "sb3b":
                case "sb3w":
                case "sbbb":
                case "sbbw":
                case "ad2b":
                case "ad2w":
                case "ad3b":
                case "ad3w":
                case "adcw":
                case "adcb":
                    // After substraction or addition we can compare result to 0
                    gopParams.OpeDefaultParamTranslation1 = "0";
                    gopParams.OpeEmbeddedParam1 = null;
                    if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == "0")
                    {
                        gopParams.OpeDefaultParamTranslation2 = ope.OperationParams[ope.OperationParams.Length - 2].DefaultTranslatedParam;
                        gopParams.OpeEmbeddedParam2 = ope.OperationParams[ope.OperationParams.Length - 2].EmbeddedParam;
                    }
                    else
                    {
                        gopParams.OpeDefaultParamTranslation2 = ope.OperationParams[ope.OperationParams.Length - 1].DefaultTranslatedParam;
                        gopParams.OpeEmbeddedParam2 = ope.OperationParams[ope.OperationParams.Length - 1].EmbeddedParam;
                    }
                    break;
                case "an3b":
                case "an3w":
                    // Specificity - Inverted Meaning
                    if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == "0")
                    {
                        gopParams.OpeReversedMeaning = true;
                        gopParams.OpeDefaultParamTranslation1 = ope.OperationParams[0].DefaultTranslatedParam;
                        gopParams.OpeEmbeddedParam1 = ope.OperationParams[0].EmbeddedParam;
                        gopParams.OpeDefaultParamTranslation2 = ope.OperationParams[1].DefaultTranslatedParam;
                        gopParams.OpeEmbeddedParam2 = ope.OperationParams[1].EmbeddedParam;
                    }
                    else
                    {
                        gopParams.OpeDefaultParamTranslation1 = "0";
                        gopParams.OpeEmbeddedParam1 = null;
                        gopParams.OpeDefaultParamTranslation2 = ope.OperationParams[ope.OperationParams.Length - 1].DefaultTranslatedParam;
                        gopParams.OpeEmbeddedParam2 = ope.OperationParams[ope.OperationParams.Length - 1].EmbeddedParam;
                    }
                    break;
                case "cmpb":
                case "cmpw":
                default:
                    // Classical way
                    if (ope.OperationParams.Length >= 1)
                    {
                        gopParams.OpeDefaultParamTranslation1 = ope.OperationParams[0].DefaultTranslatedParam;
                        gopParams.OpeEmbeddedParam1 = ope.OperationParams[0].EmbeddedParam;
                    }
                    if (ope.OperationParams.Length >= 2)
                    {
                        gopParams.OpeDefaultParamTranslation2 = ope.OperationParams[1].DefaultTranslatedParam;
                        gopParams.OpeEmbeddedParam2 = ope.OperationParams[1].EmbeddedParam;
                    }
                    break;
            }

            Operation cyOpe = gopParams.CarryOpe;
            int cyOpeBankNum = gopParams.CarryOpeBankNum;
            int cyOpeAddressInt = gopParams.CarryOpeAddressInt;
            CarryMode cyMode = gopParams.CarryOpeMode;
            object cyEP1 = gopParams.CarryOpeEmbeddedParam1;
            object cyEP2 = gopParams.CarryOpeEmbeddedParam2;
            string cyP1 = gopParams.CarryOpeDefaultParamTranslation1;
            string cyP2 = gopParams.CarryOpeDefaultParamTranslation2;

            switch (Instruction.ToLower())
            {
                // Carry Flag operations
                case "clc":
                case "stc":
                    cyOpe = ope;
                    cyOpeBankNum = ope.BankNum;
                    cyOpeAddressInt = ope.AddressInt;
                    cyMode = CarryMode.Carry;
                    break;
                // Comparison operations
                case "cmpb":
                case "cmpw":
                    cyOpe = ope;
                    cyOpeBankNum = ope.BankNum;
                    cyOpeAddressInt = ope.AddressInt;
                    cyMode = CarryMode.Comparison;
                    if (ope.OperationParams.Length >= 2)
                    {
                        cyEP1 = ope.OperationParams[0].EmbeddedParam;
                        cyEP2 = ope.OperationParams[1].EmbeddedParam;
                        cyP1 = ope.OperationParams[0].DefaultTranslatedParam;
                        cyP2 = ope.OperationParams[1].DefaultTranslatedParam;
                    }
                    break;
                // Arithmetic operations
                case "sbbb":
                case "sb2b":
                case "sb3b":
                case "sbbw":
                case "sb2w":
                case "sb3w":
                    cyOpe = ope;
                    cyOpeBankNum = ope.BankNum;
                    cyOpeAddressInt = ope.AddressInt;
                    cyMode = CarryMode.Substract;
                    // 20210720 - PYM - To match other GotoOpeParams
                    /*
                    if (ope.OperationParams.Length >= 2)
                    {
                        cyEP1 = ope.OperationParams[0].EmbeddedParam;
                        cyEP2 = ope.OperationParams[1].EmbeddedParam;
                        cyP1 = ope.OperationParams[0].DefaultTranslatedParam;
                        cyP2 = ope.OperationParams[1].DefaultTranslatedParam;
                    }
                    */
                    // After substraction we can compare result to 0
                    cyP1 = "0";
                    cyEP1 = null;
                    if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == "0")
                    {
                        cyP2 = ope.OperationParams[ope.OperationParams.Length - 2].DefaultTranslatedParam;
                        cyEP2 = ope.OperationParams[ope.OperationParams.Length - 2].EmbeddedParam;
                    }
                    else
                    {
                        cyP2 = ope.OperationParams[ope.OperationParams.Length - 1].DefaultTranslatedParam;
                        cyEP2 = ope.OperationParams[ope.OperationParams.Length - 1].EmbeddedParam;
                    }
                    break;
                case "adcb":
                case "ad2b":
                case "ad3b":
                    cyOpe = ope;
                    cyOpeBankNum = ope.BankNum;
                    cyOpeAddressInt = ope.AddressInt;
                    cyMode = CarryMode.AddBytes;
                    // 20210720 - PYM - To match other GotoOpeParams
                    /*
                    if (ope.OperationParams.Length >= 2)
                    {
                        cyEP1 = ope.OperationParams[0].EmbeddedParam;
                        cyEP2 = ope.OperationParams[1].EmbeddedParam;
                        cyP1 = ope.OperationParams[0].DefaultTranslatedParam;
                        cyP2 = ope.OperationParams[1].DefaultTranslatedParam;
                    }
                    */
                    // After addition we can compare result to ff/ffff
                    cyP1 = "ff";
                    cyEP1 = null;
                    if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == "0")
                    {
                        cyP2 = ope.OperationParams[ope.OperationParams.Length - 2].DefaultTranslatedParam;
                        cyEP2 = ope.OperationParams[ope.OperationParams.Length - 2].EmbeddedParam;
                    }
                    else
                    {
                        cyP2 = ope.OperationParams[ope.OperationParams.Length - 1].DefaultTranslatedParam;
                        cyEP2 = ope.OperationParams[ope.OperationParams.Length - 1].EmbeddedParam;
                    }
                    break;
                case "adcw":
                case "ad2w":
                case "ad3w":
                    cyOpe = ope;
                    cyOpeBankNum = ope.BankNum;
                    cyOpeAddressInt = ope.AddressInt;
                    cyMode = CarryMode.AddWords;
                    // 20210720 - PYM - To match other GotoOpeParams
                    /*
                    if (ope.OperationParams.Length >= 2)
                    {
                        cyEP1 = ope.OperationParams[0].EmbeddedParam;
                        cyEP2 = ope.OperationParams[1].EmbeddedParam;
                        cyP1 = ope.OperationParams[0].DefaultTranslatedParam;
                        cyP2 = ope.OperationParams[1].DefaultTranslatedParam;
                    }
                    */
                    // After addition we can compare result to ff/ffff
                    cyP1 = "ffff";
                    cyEP1 = null;
                    if (ope.OperationParams[ope.OperationParams.Length - 1].InstructedParam == "0")
                    {
                        cyP2 = ope.OperationParams[ope.OperationParams.Length - 2].DefaultTranslatedParam;
                        cyEP2 = ope.OperationParams[ope.OperationParams.Length - 2].EmbeddedParam;
                    }
                    else
                    {
                        cyP2 = ope.OperationParams[ope.OperationParams.Length - 1].DefaultTranslatedParam;
                        cyEP2 = ope.OperationParams[ope.OperationParams.Length - 1].EmbeddedParam;
                    }
                    break;
                case "decb":
                case "decw":
                    cyOpe = ope;
                    cyOpeBankNum = ope.BankNum;
                    cyOpeAddressInt = ope.AddressInt;
                    cyMode = CarryMode.Substract;
                    // 20210720 - PYM - To match other GotoOpeParams
                    /*
                    if (ope.OperationParams.Length >= 1)
                    {
                        cyEP1 = ope.OperationParams[0].EmbeddedParam;
                        cyEP2 = "1";
                        cyP1 = ope.OperationParams[0].DefaultTranslatedParam;
                        cyP2 = "1";
                    }
                    */
                    cyP1 = "0";
                    cyEP1 = null;
                    cyP2 = ope.OperationParams[ope.OperationParams.Length - 1].DefaultTranslatedParam;
                    cyEP2 = ope.OperationParams[ope.OperationParams.Length - 1].EmbeddedParam;
                    break;
                case "incb":
                    cyOpe = ope;
                    cyOpeBankNum = ope.BankNum;
                    cyOpeAddressInt = ope.AddressInt;
                    cyMode = CarryMode.AddBytes;
                    // 20210720 - PYM - To match other GotoOpeParams
                    /*
                    if (ope.OperationParams.Length >= 1)
                    {
                        cyEP1 = ope.OperationParams[0].EmbeddedParam;
                        cyEP2 = "1";
                        cyP1 = ope.OperationParams[0].DefaultTranslatedParam;
                        cyP2 = "1";
                    }
                    */
                    cyP1 = "ff";
                    cyEP1 = null;
                    cyP2 = ope.OperationParams[ope.OperationParams.Length - 1].DefaultTranslatedParam;
                    cyEP2 = ope.OperationParams[ope.OperationParams.Length - 1].EmbeddedParam;
                    break;
                case "incw":
                    cyOpe = ope;
                    cyOpeBankNum = ope.BankNum;
                    cyOpeAddressInt = ope.AddressInt;
                    cyMode = CarryMode.AddWords;
                    // 20210720 - PYM - To match other GotoOpeParams
                    /*
                    if (ope.OperationParams.Length >= 1)
                    {
                        cyEP1 = ope.OperationParams[0].EmbeddedParam;
                        cyEP2 = "1";
                        cyP1 = ope.OperationParams[0].DefaultTranslatedParam;
                        cyP2 = "1";
                    }
                    */
                    cyP1 = "ffff";
                    cyEP1 = null;
                    cyP2 = ope.OperationParams[ope.OperationParams.Length - 1].DefaultTranslatedParam;
                    cyEP2 = ope.OperationParams[ope.OperationParams.Length - 1].EmbeddedParam;
                    break;
                case "ml2b":
                case "ml3b":
                case "shlb":
                    cyOpe = ope;
                    cyOpeBankNum = ope.BankNum;
                    cyOpeAddressInt = ope.AddressInt;
                    cyMode = CarryMode.MultiplyBytes;
                    if (ope.OperationParams.Length >= 2)
                    {
                        cyEP1 = ope.OperationParams[0].EmbeddedParam;
                        cyEP2 = ope.OperationParams[1].EmbeddedParam;
                        cyP1 = ope.OperationParams[0].DefaultTranslatedParam;
                        cyP2 = ope.OperationParams[1].DefaultTranslatedParam;
                    }
                    break;
                case "ml2w":
                case "ml3w":
                case "shlw":
                case "shldw":
                    cyOpe = ope;
                    cyOpeBankNum = ope.BankNum;
                    cyOpeAddressInt = ope.AddressInt;
                    cyMode = CarryMode.MultiplyWords;
                    if (ope.OperationParams.Length >= 2)
                    {
                        cyEP1 = ope.OperationParams[0].EmbeddedParam;
                        cyEP2 = ope.OperationParams[1].EmbeddedParam;
                        cyP1 = ope.OperationParams[0].DefaultTranslatedParam;
                        cyP2 = ope.OperationParams[1].DefaultTranslatedParam;
                    }
                    break;
            }
            gopParams.CarryOpe = cyOpe;
            gopParams.CarryOpeBankNum = cyOpeBankNum;
            gopParams.CarryOpeAddressInt = cyOpeAddressInt;
            gopParams.CarryOpeMode = cyMode;
            gopParams.CarryOpeEmbeddedParam1 = cyEP1;
            gopParams.CarryOpeEmbeddedParam2 = cyEP2;
            gopParams.CarryOpeDefaultParamTranslation1 = cyP1;
            gopParams.CarryOpeDefaultParamTranslation2 = cyP2;
        }

        public string TranslateInstruction(Operation ope)
        {
            return getInstructionTranslation(InstructionTrans, ope);
        }
        
        private string getInstructionTranslation(string sTemplate, Operation ope)
        {
            string sTranslation = sTemplate;

            // Instruction Translation
            if (sTranslation != string.Empty)
            {
                for (int iParam = 0; iParam < ope.OperationParams.Length; iParam++)
                {
                    sTranslation = sTranslation.Replace("%" + (iParam + 1).ToString() + "%", ope.OperationParams[iParam].InstructedParam);
                }
            }

            sTranslation = string.Format("{0,-6}{1,-15}", Instruction.ToLower(), sTranslation);

            // ApplySignedAlt Operation
            if (ope.ApplySignedAlt)
            {
                if (sTranslation != string.Empty) sTranslation = SADDef.OPCSigndAltInstructionPrefix + sTranslation.Substring(0, sTranslation.Length - SADDef.OPCSigndAltInstructionPrefix.Length);
            }

            // Skip Operation
            if (ope.CallType == CallType.Skip)
            {
                sTranslation = sTranslation.Replace("%JA%", ope.AddressJump);
            }

            return sTranslation;
        }

        public string TranslateTranslation(Operation ope)
        {
            return getTranslation(Translation1, ope);
            
            // For Performance reasons Translation2 & Translation3 are not managed anymore
        }

        private string getTranslation(string sTemplate, Operation ope)
        {
            string sTranslation = sTemplate;
            Register rReg = null;
            Register rRegCpy = null;
            S6xRegister s6xReg = null;

            if (sTranslation == string.Empty) return sTranslation;

            // BitByteGotoOP Specific BitFlags Case - Started First to override normal template on register
            //      Template is overriden when  BitFlags are provided on related S6x Register
            if (Type == OPCodeType.BitByteGotoOP && ope.OriginalOpArr.Length >= 2)
            {
                rReg = null;
                s6xReg = null;
                if (ope.OperationParams[0].EmbeddedParam != null)
                {
                    if (ope.OperationParams[0].EmbeddedParam.GetType() == typeof(Register))
                    {
                        rReg = (Register)ope.OperationParams[0].EmbeddedParam;
                        s6xReg = rReg.S6xRegister;
                    }
                }

                // 20200903 - Mangement for register copy to use temporary bit flag register
                //            rReg should be the same than ope.initialGotoOpParams.OpeEmbeddedParam2
                //            s6xReg becomes also ope.initialGotoOpParams.OpeEmbeddedParam1.S6xRegister
                //            rRegCpy becomes rReg
                if (s6xReg != null)
                {
                    if (!s6xReg.isBitFlags || s6xReg.BitFlags == null) s6xReg = null;
                }
                if (rReg != null && s6xReg == null)
                {
                    if (ope.initialGotoOpParams != null)
                    {
                        if (ope.initialGotoOpParams.OpeEmbeddedParam1 != null && ope.initialGotoOpParams.OpeEmbeddedParam2 != null)
                        {
                            if (ope.initialGotoOpParams.OpeEmbeddedParam1.GetType() == typeof(Register) && ope.initialGotoOpParams.OpeEmbeddedParam2.GetType() == typeof(Register))
                            {
                                if (((Register)ope.initialGotoOpParams.OpeEmbeddedParam2).UniqueAddress == rReg.UniqueAddress)
                                {
                                    s6xReg = ((Register)ope.initialGotoOpParams.OpeEmbeddedParam1).S6xRegister;
                                    if (s6xReg != null)
                                    {
                                        if (s6xReg.isBitFlags && s6xReg.BitFlags != null) rRegCpy = rReg;
                                        else s6xReg = null;
                                    }
                                }
                            }
                        }
                    }
                }
                rReg = null;

                if (s6xReg != null)
                {
                    if (s6xReg.isBitFlags && s6xReg.BitFlags != null)
                    {
                        foreach (S6xBitFlag bitFlag in s6xReg.BitFlags)
                        {
                            if (bitFlag == null) break;
                            if (bitFlag.Skip) continue;
                            if (bitFlag.Position > 7) continue;
                            if (OPCodeInt == 0x30 + bitFlag.Position || OPCodeInt == 0x38 + bitFlag.Position)
                            // jnb / jb
                            {
                                string regTranslation = string.Empty;
                                string regCpyTranslation = string.Empty;
                                string sValue = string.Empty;
                                string bfTranslation = string.Empty;

                                if (bitFlag.ShortLabel == null) break;
                                if (bitFlag.ShortLabel.ToLower() == "b" + bitFlag.Position.ToString()) break;
                                // jnb
                                if (OPCodeInt == 0x30 + bitFlag.Position && bitFlag.NotSetValue != null && bitFlag.NotSetValue != string.Empty) sValue = bitFlag.NotSetValue;
                                // jb
                                else if (bitFlag.SetValue != null && bitFlag.SetValue != string.Empty) sValue = bitFlag.SetValue;

                                if (bitFlag.HideParent)
                                {
                                    bfTranslation = SADDef.BitByteGotoOPAltHRegPrefix + bitFlag.ShortLabel + SADDef.BitByteGotoOPAltHRegSuffix;
                                }
                                else
                                {
                                    if (s6xReg.MultipleMeanings && s6xReg.ByteLabel != null) regTranslation = Tools.PointerTranslation(s6xReg.Labels(true));
                                    else if (s6xReg.Label == null) regTranslation = Tools.PointerTranslation(s6xReg.Address);
                                    else regTranslation = Tools.PointerTranslation(s6xReg.Label);

                                    bfTranslation = regTranslation + SADDef.BitByteGotoOPAltSeparator + bitFlag.ShortLabel;

                                    // 20200903 - Mangement for register copy to use temporary bit flag register
                                    if (rRegCpy != null)
                                    {
                                        if (rRegCpy.S6xRegister == null)
                                        {
                                            regCpyTranslation = Tools.PointerTranslation(rRegCpy.Address);
                                        }
                                        else
                                        {
                                            if (rRegCpy.S6xRegister.MultipleMeanings && rRegCpy.S6xRegister.ByteLabel != null) regCpyTranslation = Tools.PointerTranslation(rRegCpy.S6xRegister.Labels(true));
                                            else if (rRegCpy.S6xRegister.Label == null) regCpyTranslation = Tools.PointerTranslation(rRegCpy.S6xRegister.Address);
                                            else regCpyTranslation = Tools.PointerTranslation(rRegCpy.S6xRegister.Label);
                                        }

                                        bfTranslation = regCpyTranslation + SADDef.BitByteGotoOPAltCopySeparator + bfTranslation;
                                    }
                                }
                                // Based on existing template, if updated this code should be updated too
                                if (sValue == string.Empty)
                                {
                                    sTranslation = sTranslation.Replace("%1%", "").Replace("B" + bitFlag.Position.ToString() + "_", bfTranslation);
                                }
                                else
                                {
                                    sTranslation = sTranslation.Replace("!", "").Replace("B" + bitFlag.Position.ToString() + "_", bfTranslation + SADDef.BitByteGotoOPAltComparison).Replace("%1%", sValue);
                                }
                                break;
                            }
                        }
                    }
                    s6xReg = null;
                }
            }

            // Specific BitFlags Case for registers on dedicated flag operations (orrb 0x91, an2b 0x71 only)
            bool fOperation = false;
            if (ope.OriginalOpArr.Length == 3 && (OPCodeInt == 0x71 || OPCodeInt == 0x91))
            {
                if (OPCodeInt == 0x91)
                {
                    switch (ope.OriginalOpArr[1].ToLower())
                    {
                        case "01":
                        case "02":
                        case "04":
                        case "08":
                        case "10":
                        case "20":
                        case "40":
                        case "80":
                            fOperation = true;
                            break;
                    }
                }
                else if (OPCodeInt == 0x71)
                {
                    switch (ope.OriginalOpArr[1].ToLower())
                    {
                        case "7f":
                        case "bf":
                        case "df":
                        case "ef":
                        case "f7":
                        case "fb":
                        case "fd":
                        case "fe":
                            fOperation = true;
                            break;
                    }
                }
                if (fOperation)
                {
                    rReg = null;
                    s6xReg = null;
                    if (ope.OperationParams[1].EmbeddedParam != null)
                    {
                        if (ope.OperationParams[1].EmbeddedParam.GetType() == typeof(Register))
                        {
                            rReg = (Register)ope.OperationParams[1].EmbeddedParam;
                            s6xReg = rReg.S6xRegister;
                        }
                    }

                    // 20200903 - Mangement for register copy to use temporary bit flag register
                    //            rReg should be the same than ope.initialGotoOpParams.OpeEmbeddedParam1
                    //            s6xReg becomes also ope.initialGotoOpParams.OpeEmbeddedParam2.S6xRegister
                    //            rRegCpy becomes rReg
                    if (s6xReg != null)
                    {
                        if (!s6xReg.isBitFlags || s6xReg.BitFlags == null) s6xReg = null;
                    }
                    if (rReg != null && s6xReg == null)
                    {
                        if (ope.initialGotoOpParams != null)
                        {
                            if (ope.initialGotoOpParams.OpeEmbeddedParam1 != null && ope.initialGotoOpParams.OpeEmbeddedParam2 != null)
                            {
                                if (ope.initialGotoOpParams.OpeEmbeddedParam1.GetType() == typeof(Register) && ope.initialGotoOpParams.OpeEmbeddedParam2.GetType() == typeof(Register))
                                {
                                    if (((Register)ope.initialGotoOpParams.OpeEmbeddedParam2).UniqueAddress == rReg.UniqueAddress)
                                    {
                                        s6xReg = ((Register)ope.initialGotoOpParams.OpeEmbeddedParam1).S6xRegister;
                                        if (s6xReg != null)
                                        {
                                            if (s6xReg.isBitFlags && s6xReg.BitFlags != null) rRegCpy = rReg;
                                            else s6xReg = null;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    rReg = null;
                    
                    if (s6xReg != null)
                    {
                        if (s6xReg.isBitFlags && s6xReg.BitFlags != null)
                        {
                            foreach (S6xBitFlag bitFlag in s6xReg.BitFlags)
                            {
                                if (bitFlag == null) break;
                                if (bitFlag.Skip) continue;
                                if (bitFlag.Position > 7) continue;
                                
                                BitArray bitArray = new BitArray(new int[] { Convert.ToInt32(ope.OriginalOpArr[1], 16) });

                                if ((OPCodeInt == 0x91 && bitArray.Get(bitFlag.Position)) || (OPCodeInt == 0x71 && !bitArray.Get(bitFlag.Position)))
                                {
                                    string regTranslation = string.Empty;
                                    string regCpyTranslation = string.Empty;
                                    string sValue = string.Empty;
                                    string bfTranslation = string.Empty;

                                    if (bitFlag.ShortLabel == null) break;
                                    if (bitFlag.ShortLabel.ToLower() == "b" + bitFlag.Position.ToString()) break;

                                    if (OPCodeInt == 0x91) sValue = (bitFlag.SetValue == null || bitFlag.SetValue == string.Empty) ? SADDef.BitByteSetOPAltDefVal : bitFlag.SetValue;
                                    else if (OPCodeInt == 0x71) sValue = (bitFlag.NotSetValue == null || bitFlag.NotSetValue == string.Empty) ? SADDef.BitByteUnSetOPAltDefVal : bitFlag.NotSetValue;

                                    if (bitFlag.HideParent)
                                    {
                                        bfTranslation = SADDef.BitByteGotoOPAltHRegPrefix + bitFlag.ShortLabel + SADDef.BitByteGotoOPAltHRegSuffix;
                                    }
                                    else
                                    {
                                        if (s6xReg.MultipleMeanings && s6xReg.ByteLabel != null) regTranslation = Tools.PointerTranslation(s6xReg.Labels(true));
                                        else if (s6xReg.Label == null) regTranslation = Tools.PointerTranslation(s6xReg.Address);
                                        else regTranslation = Tools.PointerTranslation(s6xReg.Label);

                                        bfTranslation = regTranslation + SADDef.BitByteGotoOPAltSeparator + bitFlag.ShortLabel;

                                        // 20200903 - Mangement for register copy to use temporary bit flag register
                                        if (rRegCpy != null)
                                        {
                                            if (rRegCpy.S6xRegister == null)
                                            {
                                                regCpyTranslation = Tools.PointerTranslation(rRegCpy.Address);
                                            }
                                            else
                                            {
                                                if (rRegCpy.S6xRegister.MultipleMeanings && rRegCpy.S6xRegister.ByteLabel != null) regCpyTranslation = Tools.PointerTranslation(rRegCpy.S6xRegister.Labels(true));
                                                else if (rRegCpy.S6xRegister.Label == null) regCpyTranslation = Tools.PointerTranslation(rRegCpy.S6xRegister.Address);
                                                else regCpyTranslation = Tools.PointerTranslation(rRegCpy.S6xRegister.Label);
                                            }

                                            bfTranslation = regCpyTranslation + SADDef.BitByteGotoOPAltCopySeparator + bfTranslation;
                                        }
                                    }

                                    sTranslation = SADDef.BitByteSetOPAltTemplate.Replace("%1%", bfTranslation).Replace("%2%", sValue);
                                    break;
                                }
                            }
                        }
                        s6xReg = null;
                    }
                }
            }

            for (int iParam = 0; iParam < ope.OperationParams.Length; iParam++)
            {
                // Specific case related to RBase calculation
                if (iParam == ope.IgnoredTranslatedParam)
                {
                    switch (iParam)
                    {
                        case 0:
                            if (iParam < ope.OperationParams.Length - 1)
                            {
                                int iEndPosNext = sTranslation.LastIndexOf("%" + (iParam + 2).ToString() + "%") + ("%" + (iParam + 2).ToString() + "%").Length - 1;
                                int iEndPosCurr = sTranslation.LastIndexOf("%" + (iParam + 1).ToString() + "%") + ("%" + (iParam + 1).ToString() + "%").Length - 1;
                                if (iEndPosCurr > iEndPosNext && iEndPosCurr <= sTranslation.Length)
                                {
                                    sTranslation = sTranslation.Replace(sTranslation.Substring(iEndPosNext, iEndPosCurr - iEndPosNext), "");
                                }
                                else
                                {
                                    sTranslation = sTranslation.Replace("%" + (iParam + 1).ToString() + "%", getParamTranslation(ope.OperationParams[iParam].EmbeddedParam, ope.OperationParams[iParam].DefaultTranslatedParam, (iParam == ope.OperationParams.Length - 1)));
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
                    sTranslation = sTranslation.Replace("%" + (iParam + 1).ToString() + "%", getParamTranslation(ope.OperationParams[iParam].EmbeddedParam, ope.OperationParams[iParam].DefaultTranslatedParam, (iParam == ope.OperationParams.Length - 1)));
                }
            }

            // Goto Op Params Translation
            if (ope.GotoOpParams != null && (sTemplate.Contains("%P1%") || sTemplate.Contains("%P2%") || sTemplate.Contains("%CY%")))
            {
                if (CarryTranslations == null)
                {
                    // 20210909 - PYM - Reverse meaning mngt
                    if (ope.GotoOpParams.OpeReversedMeaning)
                    {
                        if (sTranslation.Contains("==")) sTranslation = sTranslation.Replace("==", "!=");
                        else if (sTranslation.Contains("!=")) sTranslation = sTranslation.Replace("!=", "==");
                        else if (sTranslation.Contains(" (")) sTranslation = sTranslation.Replace(" (", " !(");
                    }
                    sTranslation = sTranslation.Replace("%P1%", ope.OPCode.getParamTranslation(ope.GotoOpParams.OpeEmbeddedParam1, ope.GotoOpParams.OpeDefaultParamTranslation1, false)).Replace("%P2%", ope.OPCode.getParamTranslation(ope.GotoOpParams.OpeEmbeddedParam2, ope.GotoOpParams.OpeDefaultParamTranslation2, false));
                    // 20210909 - PYM - Address Replacement
                    // GotoOpe Address Replacement
                    if (ope.GotoOpParams.Ope != null)
                    {
                        if (ope.GotoOpParams.Ope.TranslationReplacementAddress != string.Empty && ope.GotoOpParams.Ope.TranslationReplacementLabel != string.Empty)
                        {
                            sTranslation = sTranslation.Replace(ope.GotoOpParams.Ope.TranslationReplacementAddress, ope.GotoOpParams.Ope.TranslationReplacementLabel);
                        }
                    }
                }
                else
                {
                    sTranslation = sTranslation.Replace("%CY%", CarryTranslations[ope.GotoOpParams.CarryOpeMode].ToString()).Replace("%P1%", ope.OPCode.getParamTranslation(ope.GotoOpParams.CarryOpeEmbeddedParam1, ope.GotoOpParams.CarryOpeDefaultParamTranslation1, false)).Replace("%P2%", ope.OPCode.getParamTranslation(ope.GotoOpParams.CarryOpeEmbeddedParam2, ope.GotoOpParams.CarryOpeDefaultParamTranslation2, false));
                    // 20210909 - PYM - Address Replacement
                    // Carry Address Replacement
                    if (ope.GotoOpParams.CarryOpe != null)
                    {
                        if (ope.GotoOpParams.CarryOpe.TranslationReplacementAddress != string.Empty && ope.GotoOpParams.CarryOpe.TranslationReplacementLabel != string.Empty)
                        {
                            sTranslation = sTranslation.Replace(ope.GotoOpParams.CarryOpe.TranslationReplacementAddress, ope.GotoOpParams.CarryOpe.TranslationReplacementLabel);
                        }
                    }
                }
            }

            // Skip Operation
            if (ope.CallType == CallType.Skip)
            {
                sTranslation = sTranslation.Replace("%JA%", ope.AddressJump);
            }

            // Arguments
            if (ope.CallType == CallType.Call || ope.CallType == CallType.ShortCall)
            {
                sTranslation = sTranslation.Replace("%ARGS%", ope.CallArgsTranslated);
            }

            // Byte shifting / Value to use is always the first parameter
            if (sTemplate.Contains("%2^%"))
            {
                if (ope.OperationParams[0].DefaultTranslatedParam == Tools.RegisterInstruction(ope.OperationParams[0].InstructedParam))
                {
                    sTranslation = sTranslation.Replace("/", ">>").Replace("*", "<<").Replace("%2^%", getParamTranslation(ope.OperationParams[0].EmbeddedParam, ope.OperationParams[0].DefaultTranslatedParam, (0 == ope.OperationParams.Length - 1)));
                }
                else
                {
                    sTranslation = sTranslation.Replace("%2^%", getParamTranslation(ope.OperationParams[0].EmbeddedParam, ope.OperationParams[0].DefaultTranslatedParam, (0 == ope.OperationParams.Length - 1)));
                }
            }

            // ApplyAltSigned
            if (ope.ApplySignedAlt) sTranslation = sTranslation.Replace("= ", "= " + SADDef.OPCSigndAltTranslationAdder);
            
            // Address Replacement
            if (ope.TranslationReplacementAddress != string.Empty && ope.TranslationReplacementLabel != string.Empty)
            {
                sTranslation = sTranslation.Replace(ope.TranslationReplacementAddress, ope.TranslationReplacementLabel);
            }

            return sTranslation;
        }

        public string getParamTranslation(object oParam, string defaultTranslation, bool writeModeForParam)
        {
            if (oParam == null) return defaultTranslation;

            if (oParam.GetType() == typeof(string)) return defaultTranslation;

            if (oParam.GetType() == typeof(int)) return defaultTranslation;

            if (oParam.GetType() == typeof(RBase)) return Tools.RegisterInstruction(((RBase)oParam).Code);

            if (oParam.GetType() == typeof(RConst)) return ((RConst)oParam).Value;

            if (oParam.GetType() == typeof(Call)) return ((Call)oParam).ShortLabel;

            // 20200818 - PYM - To manage missing brackets around Calibration Elements / Elements
            //  When defaultTranslation or CalculatedParam are including brackets, param translation should too.
            string elementParamTranslation = string.Empty;
            bool elementPointerUse = false;
            if (oParam.GetType() == typeof(CalibrationElement))
            {
                elementPointerUse = (defaultTranslation != ((CalibrationElement)oParam).Address);
                if (((CalibrationElement)oParam).isScalar) elementParamTranslation = ((CalibrationElement)oParam).ScalarElem.ShortLabel;
                else if (((CalibrationElement)oParam).isFunction) elementParamTranslation = ((CalibrationElement)oParam).FunctionElem.ShortLabel;
                else if (((CalibrationElement)oParam).isTable) elementParamTranslation = ((CalibrationElement)oParam).TableElem.ShortLabel;
                else if (((CalibrationElement)oParam).isStructure) elementParamTranslation= ((CalibrationElement)oParam).StructureElem.ShortLabel;
            }

            if (oParam.GetType() == typeof(Scalar))
            {
                elementPointerUse = (defaultTranslation != ((Scalar)oParam).Address);
                elementParamTranslation = ((Scalar)oParam).ShortLabel;
            }
            if (oParam.GetType() == typeof(Function))
            {
                elementPointerUse = (defaultTranslation != ((Function)oParam).Address);
                elementParamTranslation = ((Function)oParam).ShortLabel;
            }
            if (oParam.GetType() == typeof(Table))
            {
                elementPointerUse = (defaultTranslation != ((Table)oParam).Address);
                elementParamTranslation = ((Table)oParam).ShortLabel;
            }
            if (oParam.GetType() == typeof(Structure))
            {
                elementPointerUse = (defaultTranslation != ((Structure)oParam).Address);
                elementParamTranslation = ((Structure)oParam).ShortLabel;
            }

            if (elementParamTranslation != string.Empty)
            {
                if (elementPointerUse) return Tools.PointerTranslation(elementParamTranslation);
                else return elementParamTranslation;
            }

            if (oParam.GetType() == typeof(Register)) return getRegisterTranslation((Register)oParam, defaultTranslation, writeModeForParam);

            if (oParam.GetType() == typeof(object[]))
            {
                if (((object[])oParam).Length == 2 && defaultTranslation.Contains(SADDef.AdditionSeparator))
                {
                    bool isPointer = defaultTranslation.StartsWith(SADDef.LongRegisterPointerPrefix) && defaultTranslation.EndsWith(SADDef.LongRegisterPointerSuffix);
                    string defaultTranslation0 = string.Empty;
                    string defaultTranslation1 = string.Empty;
                    string sTranslation = string.Empty;

                    if (!defaultTranslation.StartsWith(SADDef.AdditionSeparator)) defaultTranslation0 = defaultTranslation.Substring(0, defaultTranslation.IndexOf(SADDef.AdditionSeparator));
                    if (!defaultTranslation.EndsWith(SADDef.AdditionSeparator)) defaultTranslation1 = defaultTranslation.Substring(defaultTranslation.IndexOf(SADDef.AdditionSeparator) + 1);

                    if (isPointer)
                    {
                        defaultTranslation0 = defaultTranslation0.Substring(1);
                        defaultTranslation1 = defaultTranslation1.Substring(0, defaultTranslation1.Length - 1);
                    }

                    sTranslation = (isPointer ? SADDef.LongRegisterPointerPrefix : string.Empty);
                    sTranslation += getParamTranslation(((object[])oParam)[0], defaultTranslation0, writeModeForParam) + SADDef.AdditionSeparator;
                    sTranslation += getParamTranslation(((object[])oParam)[1], defaultTranslation1, writeModeForParam) + (isPointer ? SADDef.LongRegisterPointerSuffix : string.Empty);

                    return sTranslation;
                }
            }
            
            return defaultTranslation;
        }

        // Get Translation for Instructed Param
        private string getRegisterTranslation(Register rReg, string defaultTranslation, bool writeMode)
        {
            // Possible values samples : 12, abcd, R12, [abcd], R12++, [R12], [R12+abcd], ...

            string sTranslation = string.Empty;

            if (rReg == null) return defaultTranslation;

            // S6x Translation First to override other translations
            if (rReg.S6xRegister != null)
            {
                string regLabel = rReg.S6xRegister.Label;
                if (rReg.S6xRegister.MultipleMeanings)
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
                    regLabel = rReg.S6xRegister.Labels(byteOpe);
                }
                if (regLabel != null && regLabel != string.Empty && regLabel != rReg.Address && regLabel != Tools.RegisterInstruction(rReg.Address))
                {
                    if (defaultTranslation.StartsWith(SADDef.LongRegisterPointerPrefix) && defaultTranslation.EndsWith(SADDef.LongRegisterPointerSuffix) && defaultTranslation.Contains(SADDef.AdditionSeparator))
                    {
                        if (defaultTranslation.StartsWith(SADDef.LongRegisterPointerPrefix + rReg.Instruction))
                        {
                            return defaultTranslation.Replace(SADDef.AdditionSeparator + rReg.Address, SADDef.AdditionSeparator + regLabel);
                        }
                        return defaultTranslation.Replace(rReg.Address + SADDef.AdditionSeparator, regLabel + SADDef.AdditionSeparator);
                    }
                    if (defaultTranslation.StartsWith(SADDef.ShortRegisterPrefix) || (defaultTranslation.StartsWith(SADDef.LongRegisterPointerPrefix) && defaultTranslation.EndsWith(SADDef.LongRegisterPointerSuffix)))
                    {
                        return defaultTranslation.Replace(rReg.Instruction, Tools.PointerTranslation(regLabel));
                    }
                    else if (defaultTranslation.StartsWith(SADDef.LongRegisterPointerPrefix))
                    {
                        return SADDef.LongRegisterPointerPrefix + regLabel;
                    }
                    else if (defaultTranslation.EndsWith(SADDef.LongRegisterPointerSuffix))
                    {
                        return regLabel + SADDef.LongRegisterPointerPrefix;
                    }
                    else
                    {
                        return regLabel;
                    }
                }
                return defaultTranslation;
            }

            // Eec Register Param Calculation
            if (rReg.EecRegister != null)
            {
                switch (rReg.EecRegister.Check)
                {
                    case EecRegisterCheck.ReadWrite:
                        if (writeMode) sTranslation = rReg.EecRegister.TranslationWriteByte;
                        else sTranslation = rReg.EecRegister.TranslationReadByte;
                        break;
                    case EecRegisterCheck.DataType:
                        switch (Type)
                        {
                            case OPCodeType.BitByteGotoOP:
                            case OPCodeType.ByteOP:
                                sTranslation = rReg.EecRegister.TranslationReadByte;
                                break;
                            case OPCodeType.WordOP:
                                sTranslation = rReg.EecRegister.TranslationReadWord;
                                break;
                            case OPCodeType.MixedOP:
                                if (writeMode) sTranslation = rReg.EecRegister.TranslationReadWord;
                                else sTranslation = rReg.EecRegister.TranslationReadByte;
                                break;
                            default: // Should never happen
                                sTranslation = rReg.EecRegister.TranslationReadByte;
                                break;
                        }
                        break;
                    case EecRegisterCheck.Both:
                        switch (Type)
                        {
                            case OPCodeType.BitByteGotoOP:
                            case OPCodeType.ByteOP:
                                if (writeMode) sTranslation = rReg.EecRegister.TranslationWriteByte;
                                else sTranslation = rReg.EecRegister.TranslationReadByte;
                                break;
                            case OPCodeType.WordOP:
                                if (writeMode) sTranslation = rReg.EecRegister.TranslationWriteWord;
                                else sTranslation = rReg.EecRegister.TranslationReadWord;
                                break;
                            case OPCodeType.MixedOP:
                                if (writeMode) sTranslation = rReg.EecRegister.TranslationWriteWord;
                                else sTranslation = rReg.EecRegister.TranslationReadByte;
                                break;
                            default: // Should never happen
                                sTranslation = rReg.EecRegister.TranslationReadByte;
                                break;
                        }
                        break;
                    default:
                        sTranslation = rReg.EecRegister.TranslationReadByte;
                        break;
                }

                if (defaultTranslation.StartsWith(SADDef.ShortRegisterPrefix) || (defaultTranslation.StartsWith(SADDef.LongRegisterPointerPrefix) && defaultTranslation.EndsWith(SADDef.LongRegisterPointerSuffix)))
                {
                    sTranslation = defaultTranslation.Replace(rReg.EecRegister.InstructionTrans, sTranslation);
                }
                else if (defaultTranslation.StartsWith(SADDef.LongRegisterPointerPrefix))
                {
                    sTranslation = SADDef.LongRegisterPointerPrefix + sTranslation;
                }
                else if (defaultTranslation.EndsWith(SADDef.LongRegisterPointerSuffix))
                {
                    sTranslation = sTranslation + SADDef.LongRegisterPointerPrefix;
                }

                return sTranslation;
            }

            // KAM / CC / EC Register ranges - 8061 only
            //  Default Translation for Special Registers
            if (is8061)
            {
                if (rReg.is8061KAMRegister) sTranslation = Tools.PointerTranslation(SADDef.KAMRegister8061Template.Replace("%LREG%", rReg.Address));
                if (rReg.is8061CCRegister) sTranslation =  Tools.PointerTranslation(SADDef.CCRegister8061Template.Replace("%LREG%", rReg.Address));
                if (rReg.is8061ECRegister) sTranslation = Tools.PointerTranslation(SADDef.ECRegister8061Template.Replace("%LREG%", rReg.Address));

                if (sTranslation != string.Empty)
                {
                    if (defaultTranslation.StartsWith(SADDef.ShortRegisterPrefix) || (defaultTranslation.StartsWith(SADDef.LongRegisterPointerPrefix) && defaultTranslation.EndsWith(SADDef.LongRegisterPointerSuffix)))
                    {
                        sTranslation = defaultTranslation.Replace(rReg.Instruction, sTranslation);
                    }
                    else if (defaultTranslation.StartsWith(SADDef.LongRegisterPointerPrefix))
                    {
                        sTranslation = SADDef.LongRegisterPointerPrefix + sTranslation;
                    }
                    else if (defaultTranslation.EndsWith(SADDef.LongRegisterPointerSuffix))
                    {
                        sTranslation = sTranslation + SADDef.LongRegisterPointerPrefix;
                    }
                    return sTranslation;
                }
            }
            
            // No Translation
            return defaultTranslation;
        }
    }
}
