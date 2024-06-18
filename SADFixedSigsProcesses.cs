using System;
using System.Collections;
using System.Text;
using System.Windows.Forms;

namespace SAD806x
{
    public static class SADFixedSigsProcesses
    {
        // Forced Signatures analysis
        //      Forced Signatures analysis to generate specific registers, structures and so on...
        //  20200402 - PYM - New process added
        public static void processForcedFoundSignatures(ref SADCalib Calibration, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x, ref ArrayList alErrors)
        {
            // OBDIIRegistersRoutines Process ant other
            ArrayList alBaseRegistersRoutines = new ArrayList();
            ArrayList alOBDIIRegistersRoutines = new ArrayList();
            ArrayList alOBDI3DRegistersRoutines = new ArrayList();
            ArrayList alOBDI2DRegistersRoutines = new ArrayList();

            // Added Other Elements for reading, because not done anywhere else
            ArrayList alAddedOtherElementsUniqueAddressesToRead = new ArrayList();

            // Found Signatures generate Routines in ProcessRoutines list
            //  Interesting Found Signatures are Signature Forced ones
            foreach (S6xRoutine s6xRoutine in S6x.slProcessRoutines.Values)
            {
                if (!s6xRoutine.SignatureForced) continue;
                if (s6xRoutine.SignatureKey == null || s6xRoutine.SignatureKey == string.Empty) continue;
                switch (s6xRoutine.BankNum)
                {
                    case 8:
                        if (Bank8 == null) continue;
                        break;
                    case 1:
                        if (Bank1 == null) continue;
                        break;
                    case 9:
                        if (Bank9 == null) continue;
                        break;
                    case 0:
                        if (Bank0 == null) continue;
                        break;
                }

                foreach (object[] routineSignature in SADFixedSigs.Fixed_Routines_Signatures)
                {
                    if (s6xRoutine.SignatureKey == routineSignature[0].ToString())
                    {
                        SADFixedSigs.Fixed_Routines fixedRoutineType = (SADFixedSigs.Fixed_Routines)routineSignature[1];
                        switch (fixedRoutineType)
                        {
                            case SADFixedSigs.Fixed_Routines.OBDII_REG_INIT:
                            case SADFixedSigs.Fixed_Routines.OBDII_REG_RESET:
                            case SADFixedSigs.Fixed_Routines.OBDII_REG_FLAGS:
                            case SADFixedSigs.Fixed_Routines.OBDII_CLEAR_MALF:
                            case SADFixedSigs.Fixed_Routines.OBDII_MALFUNCTION:
                                alOBDIIRegistersRoutines.Add(new object[] { fixedRoutineType, s6xRoutine });
                                break;
                            case SADFixedSigs.Fixed_Routines.OBDI_COD_3D:
                            case SADFixedSigs.Fixed_Routines.OBDI_CNT_3D:
                            case SADFixedSigs.Fixed_Routines.OBDI_MALFUNC:
                            case SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_BT:
                            case SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_EX:
                            case SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_LW:
                            case SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_SB:
                                alOBDI3DRegistersRoutines.Add(new object[] { fixedRoutineType, s6xRoutine });
                                break;
                            case SADFixedSigs.Fixed_Routines.OBDI_COD_2D:
                            case SADFixedSigs.Fixed_Routines.OBDI_TIM_2D:
                                alOBDI2DRegistersRoutines.Add(new object[] { fixedRoutineType, s6xRoutine });
                                break;
                            case SADFixedSigs.Fixed_Routines.RPM_MNGT_8061_01:
                            case SADFixedSigs.Fixed_Routines.RPM_MNGT_8061_02:
                            case SADFixedSigs.Fixed_Routines.RPM_MNGT_8065_01:
                            case SADFixedSigs.Fixed_Routines.RPM_MNGT_8065_02:
                            case SADFixedSigs.Fixed_Routines.RPM_MNGT_8065_03:
                            case SADFixedSigs.Fixed_Routines.ECT_INIT_8061_01:
                            case SADFixedSigs.Fixed_Routines.ECT_INIT_8061_02:
                                alBaseRegistersRoutines.Add(new object[] { fixedRoutineType, s6xRoutine });
                                break;
                        }
                        break;
                    }
                }
            }

            // RegistersRoutines Process
            try
            {
                processForcedFoundSignaturesBaseRegistersRoutines(ref alBaseRegistersRoutines, ref Calibration, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x, ref alAddedOtherElementsUniqueAddressesToRead);
            }
            catch
            {
                alErrors.Add("Processing base registers routines signatures and elements has failed.");
            }

            // OBDI3DRegistersRoutines Process
            try
            {
                processForcedFoundSignaturesOBDI2DRegistersRoutines(ref alOBDI2DRegistersRoutines, ref Calibration, ref Bank8, ref S6x, ref alAddedOtherElementsUniqueAddressesToRead);
            }
            catch
            {
                alErrors.Add("Processing OBDI 3 digits signatures and elements has failed.");
            }

            // OBDI3DRegistersRoutines Process
            try
            {
                processForcedFoundSignaturesOBDI3DRegistersRoutines(ref alOBDI3DRegistersRoutines, ref Calibration, ref Bank8, ref S6x, ref alAddedOtherElementsUniqueAddressesToRead);
            }
            catch
            {
                alErrors.Add("Processing OBDI 2 digits signatures and elements has failed.");
            }

            // OBDIIRegistersRoutines Process
            try
            {
                processForcedFoundSignaturesOBDIIRegistersRoutines(ref alOBDIIRegistersRoutines, ref Calibration, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref S6x, ref alAddedOtherElementsUniqueAddressesToRead);
            }
            catch
            {
                alErrors.Add("Processing OBDII signatures and elements has failed.");
            }

            // Reading generated elements
            readAddedOtherElements(ref alAddedOtherElementsUniqueAddressesToRead, ref Calibration, ref Bank0, ref Bank1, ref Bank8, ref Bank9);

            alAddedOtherElementsUniqueAddressesToRead = null;
            
            alOBDI2DRegistersRoutines = null;
            alOBDI3DRegistersRoutines = null;
            alOBDIIRegistersRoutines = null;
            alBaseRegistersRoutines = null;
        }

        // Forced Signatures BaseRegistersRoutines part
        //  20200502 - PYM - New process added
        private static void processForcedFoundSignaturesBaseRegistersRoutines(ref ArrayList alRoutines, ref SADCalib Calibration, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x, ref ArrayList alAddedOtherElementsUniqueAddressesToRead)
        {
            S6xRoutine rtRpmMngt = null;
            SADFixedSigs.Fixed_Routines rtRpmMngtSource = SADFixedSigs.Fixed_Routines.UNKNOWN;

            S6xRoutine rtEctInit = null;
            SADFixedSigs.Fixed_Routines rtEctInitSource = SADFixedSigs.Fixed_Routines.UNKNOWN;

            foreach (object[] rtObject in alRoutines)
            {
                switch ((SADFixedSigs.Fixed_Routines)rtObject[0])
                {
                    case SADFixedSigs.Fixed_Routines.RPM_MNGT_8061_01:
                    case SADFixedSigs.Fixed_Routines.RPM_MNGT_8061_02:
                    case SADFixedSigs.Fixed_Routines.RPM_MNGT_8065_01:
                    case SADFixedSigs.Fixed_Routines.RPM_MNGT_8065_02:
                    case SADFixedSigs.Fixed_Routines.RPM_MNGT_8065_03:
                        rtRpmMngt = (S6xRoutine)rtObject[1];
                        rtRpmMngtSource = (SADFixedSigs.Fixed_Routines)rtObject[0];
                        break;
                    case SADFixedSigs.Fixed_Routines.ECT_INIT_8061_01:
                    case SADFixedSigs.Fixed_Routines.ECT_INIT_8061_02:
                        rtEctInit = (S6xRoutine)rtObject[1];
                        rtEctInitSource = (SADFixedSigs.Fixed_Routines)rtObject[0];
                        break;
                }

                if (rtRpmMngt != null) break;
            }

            ArrayList alS6xRegisters = new ArrayList();
            ArrayList alS6xTables = new ArrayList();
            ArrayList alS6xFunctions = new ArrayList();
            ArrayList alS6xScalars = new ArrayList();
            ArrayList alS6xStructures = new ArrayList();

            if (rtRpmMngt != null)
            {
                getBaseRegistersRoutinesRPM(ref rtRpmMngt, rtRpmMngtSource, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref alS6xRegisters, ref alS6xTables, ref alS6xFunctions, ref alS6xScalars, ref alS6xStructures);
            }

            if (rtEctInit != null)
            {
                getBaseRegistersRoutinesECTACT(ref rtEctInit, rtEctInitSource, ref Bank0, ref Bank1, ref Bank8, ref Bank9, ref alS6xRegisters, ref alS6xTables, ref alS6xFunctions, ref alS6xScalars, ref alS6xStructures);
            }

            foreach (S6xRegister s6xReg in alS6xRegisters) addRegister(s6xReg, ref Calibration, ref S6x);
            foreach (S6xScalar s6xScal in alS6xScalars)
            {
                addScalar(s6xScal, ref Calibration);
                alAddedOtherElementsUniqueAddressesToRead.Add(s6xScal.UniqueAddress);
            }
            foreach (S6xFunction s6xFunc in alS6xFunctions)
            {
                addFunction(s6xFunc, ref Calibration);
                alAddedOtherElementsUniqueAddressesToRead.Add(s6xFunc.UniqueAddress);
            }
            /*
            foreach (S6xTable s6xTable in alS6xTables)
            {
                addTable(s6xTable, ref Calibration);
                alAddedOtherElementsUniqueAddressesToRead.Add(s6xTable.UniqueAddress);
            }
            foreach (S6xStructure s6xStruct in alS6xStructures)
            {
                addStructure(s6xStruct, ref Calibration, XXXX);
                alAddedOtherElementsUniqueAddressesToRead.Add(s6xStruct.UniqueAddress);
            }
            */
        }

        // Forced Signatures BaseRegistersRoutines part
        //  20200502 - PYM - New process added
        private static void getBaseRegistersRoutinesRPM(ref S6xRoutine rtRpmMngt, SADFixedSigs.Fixed_Routines rtRpmMngtSource, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref ArrayList alS6xRegisters, ref ArrayList alS6xTables, ref ArrayList alS6xFunctions, ref ArrayList alS6xScalars, ref ArrayList alS6xStructures)
        {
            if (rtRpmMngt == null) return;
            switch (rtRpmMngtSource)
            {
                case SADFixedSigs.Fixed_Routines.RPM_MNGT_8061_01:
                case SADFixedSigs.Fixed_Routines.RPM_MNGT_8061_02:
                case SADFixedSigs.Fixed_Routines.RPM_MNGT_8065_01:
                case SADFixedSigs.Fixed_Routines.RPM_MNGT_8065_02:
                case SADFixedSigs.Fixed_Routines.RPM_MNGT_8065_03:
                    break;
                default:
                    // Not Managed
                    return;
            }

            if (alS6xRegisters == null) alS6xRegisters = new ArrayList();
            if (alS6xTables == null) alS6xTables = new ArrayList();
            if (alS6xFunctions == null) alS6xFunctions = new ArrayList();
            if (alS6xScalars == null) alS6xScalars = new ArrayList();
            if (alS6xStructures == null) alS6xStructures = new ArrayList();

            S6xRegister s6xReg = null;
            S6xRegister s6xRegTmp = null;

            // Register mapping
            SADBank Bank = null;
            switch (rtRpmMngt.BankNum)
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
            Operation[] arrOps = Bank.getFollowingOPs(rtRpmMngt.AddressInt, 32, 0, false, false, false, false, false, false, false, true);
            Bank = null;

            int foundRegisters = 0;
            //int foundTables = 0;
            //int foundFunctions = 0;
            //int foundScalars = 0;
            //int foundStructures = 0;

            if (rtRpmMngtSource == SADFixedSigs.Fixed_Routines.RPM_MNGT_8061_01)
            {
                foundRegisters = 0;
                Operation lastOpeA0 = null;
                bool bA1Previous = false;
                foreach (Operation ope in arrOps)
                {
                    if (ope == null) break;
                    if (ope.isReturn) break;

                    switch (ope.OriginalOPCode)
                    {
                        case "a0":
                            lastOpeA0 = ope;
                            break;
                        case "a1":
                            bA1Previous = true;
                            break;
                        case "a3":
                            if (bA1Previous && lastOpeA0 != null)
                            {
                                if (ope.OperationParams.Length > 0)
                                {
                                    object[] arrPointersValues = Tools.InstructionPointersValues(lastOpeA0.OperationParams[0].InstructedParam);
                                    if ((bool)arrPointersValues[0])
                                    {
                                        s6xReg = new S6xRegister((string)arrPointersValues[1]);
                                        s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.N_RPM);
                                        s6xReg.Label = s6xRegTmp.Label;
                                        s6xReg.Comments = s6xRegTmp.Comments;
                                        s6xReg.ScaleExpression = s6xRegTmp.ScaleExpression;
                                        s6xReg.ScalePrecision = s6xRegTmp.ScalePrecision;
                                        s6xReg.Units = s6xRegTmp.Units;
                                        s6xReg.Store = true;
                                        s6xRegTmp = null;
                                        alS6xRegisters.Add(s6xReg);
                                        s6xReg = null;

                                        foundRegisters++;
                                    }
                                }
                            }
                            break;
                        default:
                            bA1Previous = false;
                            break;
                    }

                    if (foundRegisters >= 1) break;
                }
            }
            else
            {
                foundRegisters = 0;
                bool intsDisabled = false;
                foreach (Operation ope in arrOps)
                {
                    if (ope == null) break;
                    if (ope.isReturn) break;

                    switch (ope.OriginalOPCode)
                    {
                        case "fa":
                            intsDisabled = true;
                            break;
                        case "fb":
                            intsDisabled = false;
                            break;
                        case "01":
                            if (intsDisabled && ope.OperationParams.Length > 0)
                            {
                                object[] arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                                if ((bool)arrPointersValues[0])
                                {
                                    s6xReg = new S6xRegister((string)arrPointersValues[1]);
                                    s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.N_RPM);
                                    s6xReg.Label = s6xRegTmp.Label;
                                    s6xReg.Comments = s6xRegTmp.Comments;
                                    s6xReg.ScaleExpression = s6xRegTmp.ScaleExpression;
                                    s6xReg.ScalePrecision = s6xRegTmp.ScalePrecision;
                                    s6xReg.Units = s6xRegTmp.Units;
                                    s6xReg.Store = true;
                                    s6xRegTmp = null;
                                    alS6xRegisters.Add(s6xReg);
                                    s6xReg = null;

                                    foundRegisters++;
                                }
                            }
                            break;
                    }

                    if (foundRegisters >= 1) break;
                }
            }

            arrOps = null;
        }

        // Forced Signatures BaseRegistersRoutines part
        //  20200502 - PYM - New process added
        private static void getBaseRegistersRoutinesECTACT(ref S6xRoutine rtEctInit, SADFixedSigs.Fixed_Routines rtEctInitSource, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref ArrayList alS6xRegisters, ref ArrayList alS6xTables, ref ArrayList alS6xFunctions, ref ArrayList alS6xScalars, ref ArrayList alS6xStructures)
        {
            if (rtEctInit == null) return;
            switch (rtEctInitSource)
            {
                case SADFixedSigs.Fixed_Routines.ECT_INIT_8061_01:
                case SADFixedSigs.Fixed_Routines.ECT_INIT_8061_02:
                    break;
                default:
                    // Not Managed
                    return;
            }

            if (alS6xRegisters == null) alS6xRegisters = new ArrayList();
            if (alS6xTables == null) alS6xTables = new ArrayList();
            if (alS6xFunctions == null) alS6xFunctions = new ArrayList();
            if (alS6xScalars == null) alS6xScalars = new ArrayList();
            if (alS6xStructures == null) alS6xStructures = new ArrayList();

            object[] arrPointersValues = null;
            S6xRegister s6xReg = null;
            S6xRegister s6xRegTmp = null;
            S6xFunction s6xFunc = null;
            S6xFunction s6xFuncTmp = null;
            S6xScalar s6xScal = null;
            S6xScalar s6xScalTmp = null;

            // Register mapping
            SADBank Bank = null;
            switch (rtEctInit.BankNum)
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
            Operation[] arrOps = Bank.getFollowingOPs(rtEctInit.AddressInt, 32, 0, false, false, false, false, false, false, false, true);
            Bank = null;

            int foundRegisters = 0;
            //int foundTables = 0;
            int foundFunctions = 0;
            int foundScalars = 0;
            //int foundStructures = 0;

            if (rtEctInitSource == SADFixedSigs.Fixed_Routines.ECT_INIT_8061_01)
            {
                foundRegisters = 0;
                foreach (Operation ope in arrOps)
                {
                    if (ope == null) break;
                    if (ope.isReturn) break;

                    switch (ope.OriginalOPCode)
                    {
                        case "c7":
                            if (ope.OperationParams.Length > 0)
                            {
                                arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                                if ((bool)arrPointersValues[0])
                                {
                                    s6xReg = new S6xRegister((string)arrPointersValues[1]);
                                    if (rtEctInitSource == SADFixedSigs.Fixed_Routines.ECT_INIT_8061_01) s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.ECT_w_EU);
                                    else s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.ECT_w);
                                    s6xReg.Label = s6xRegTmp.Label;
                                    s6xReg.Comments = s6xRegTmp.Comments;
                                    s6xReg.ScaleExpression = s6xRegTmp.ScaleExpression;
                                    s6xReg.ScalePrecision = s6xRegTmp.ScalePrecision;
                                    s6xReg.Units = s6xRegTmp.Units;
                                    s6xReg.Store = true;
                                    s6xRegTmp = null;
                                    alS6xRegisters.Add(s6xReg);
                                    s6xReg = null;

                                    foundRegisters++;
                                }
                            }
                            break;
                        case "b0":
                            if (ope.OperationParams.Length > 1)
                            {
                                arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[1].InstructedParam);
                                if ((bool)arrPointersValues[0])
                                {
                                    s6xReg = new S6xRegister((string)arrPointersValues[1]);
                                    if (rtEctInitSource == SADFixedSigs.Fixed_Routines.ECT_INIT_8061_01) s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.ECT_EU);
                                    else s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.ECT);
                                    s6xReg.Label = s6xRegTmp.Label;
                                    s6xReg.Comments = s6xRegTmp.Comments;
                                    s6xReg.ScaleExpression = s6xRegTmp.ScaleExpression;
                                    s6xReg.ScalePrecision = s6xRegTmp.ScalePrecision;
                                    s6xReg.Units = s6xRegTmp.Units;
                                    s6xReg.Store = true;
                                    s6xRegTmp = null;
                                    alS6xRegisters.Add(s6xReg);
                                    s6xReg = null;

                                    foundRegisters++;
                                }
                            }
                            break;
                        case "5c":
                            if (ope.ApplySignedAlt && ope.OperationParams.Length > 0)
                            {
                                arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                                if ((bool)arrPointersValues[0])
                                {
                                    s6xReg = new S6xRegister((string)arrPointersValues[1]);
                                    if (rtEctInitSource == SADFixedSigs.Fixed_Routines.ECT_INIT_8061_01) s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.ACT_EU);
                                    else s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.ACT);
                                    s6xReg.Label = s6xRegTmp.Label;
                                    s6xReg.Comments = s6xRegTmp.Comments;
                                    s6xReg.ScaleExpression = s6xRegTmp.ScaleExpression;
                                    s6xReg.ScalePrecision = s6xRegTmp.ScalePrecision;
                                    s6xReg.Units = s6xRegTmp.Units;
                                    s6xReg.Store = true;
                                    s6xRegTmp = null;
                                    alS6xRegisters.Add(s6xReg);
                                    s6xReg = null;

                                    foundRegisters++;
                                }
                            }
                            break;
                    }

                    if (foundRegisters >= 3) break;
                }
            }

            if (rtEctInitSource == SADFixedSigs.Fixed_Routines.ECT_INIT_8061_02)
            {
                foundRegisters = 0;
                foundFunctions = 0;
                foundScalars = 0;
                foreach (Operation ope in arrOps)
                {
                    if (ope == null) break;
                    if (ope.isReturn) break;

                    switch (ope.OriginalOPCode)
                    {
                        case "a3":
                        case "b0":
                            if (ope.OperationParams.Length > 0)
                            {
                                if (foundRegisters == 2 && ope.OperationParams.Length > 1) arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[1].InstructedParam);
                                else arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                                if ((bool)arrPointersValues[0])
                                {
                                    if (foundRegisters == 0) s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.IACT);
                                    else if (foundRegisters == 1) s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.ECT);
                                    else if (foundRegisters == 2) s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.ACT);
                                    else if (foundRegisters == 3) s6xRegTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.IECT);
                                    if (s6xRegTmp != null)
                                    {
                                        s6xReg = new S6xRegister((string)arrPointersValues[1]);
                                        s6xReg.Label = s6xRegTmp.Label;
                                        s6xReg.Comments = s6xRegTmp.Comments;
                                        s6xReg.ScaleExpression = s6xRegTmp.ScaleExpression;
                                        s6xReg.ScalePrecision = s6xRegTmp.ScalePrecision;
                                        s6xReg.Units = s6xRegTmp.Units;
                                        s6xReg.Store = true;
                                        s6xRegTmp = null;
                                        alS6xRegisters.Add(s6xReg);
                                        s6xReg = null;

                                        foundRegisters++;
                                    }
                                }
                            }
                            break;
                        case "8b":
                        case "9b":
                        case "b3":
                            if (ope.alCalibrationElems != null)
                            {
                                if (ope.alCalibrationElems.Count == 1)
                                {
                                    s6xScalTmp = null;
                                    if (foundScalars == 0) s6xScalTmp = SADFixedScalars.GetFixedScalarTemplate(SADFixedScalars.FixedScalars.ACTMAX);
                                    else if (foundScalars == 1) s6xScalTmp = SADFixedScalars.GetFixedScalarTemplate(SADFixedScalars.FixedScalars.ACTMIN);
                                    else if (foundScalars == 2) s6xScalTmp = SADFixedScalars.GetFixedScalarTemplate(SADFixedScalars.FixedScalars.NUMEGO);
                                    else if (foundScalars == 3) s6xScalTmp = SADFixedScalars.GetFixedScalarTemplate(SADFixedScalars.FixedScalars.ACTFMM);
                                    else if (foundScalars == 4) s6xScalTmp = SADFixedScalars.GetFixedScalarTemplate(SADFixedScalars.FixedScalars.ECTMAX);
                                    else if (foundScalars == 5) s6xScalTmp = SADFixedScalars.GetFixedScalarTemplate(SADFixedScalars.FixedScalars.ECTMIN);
                                    if (s6xScalTmp != null)
                                    {
                                        s6xScal = new S6xScalar((CalibrationElement)ope.alCalibrationElems[0]);
                                        s6xScal.ShortLabel = s6xScalTmp.ShortLabel;
                                        s6xScal.Label = s6xScalTmp.Label;
                                        s6xScal.Comments = s6xScalTmp.Comments;
                                        s6xScal.ScaleExpression = s6xScalTmp.ScaleExpression;
                                        s6xScal.ScalePrecision = s6xScalTmp.ScalePrecision;
                                        s6xScal.Units = s6xScalTmp.Units;
                                        s6xScal.Store = true;
                                        s6xScalTmp = null;
                                        alS6xScalars.Add(s6xScal);
                                        s6xScal = null;

                                        foundScalars++;
                                    }
                                }
                            }
                            break;
                        case "ef":
                            if (ope.alCalibrationElems != null)
                            {
                                if (ope.alCalibrationElems.Count == 1)
                                {
                                    s6xFuncTmp = null;
                                    if (foundFunctions == 0) s6xFuncTmp = SADFixedFunctions.GetFixedFunctionTemplate(SADFixedFunctions.FixedFunctions.FN703);
                                    if (s6xFuncTmp != null)
                                    {
                                        s6xFunc = new S6xFunction((CalibrationElement)ope.alCalibrationElems[0]);
                                        s6xFunc.ShortLabel = s6xFuncTmp.ShortLabel;
                                        s6xFunc.Label = s6xFuncTmp.Label;
                                        s6xFunc.Comments = s6xFuncTmp.Comments;
                                        s6xFunc.InputScaleExpression = s6xFuncTmp.InputScaleExpression;
                                        s6xFunc.InputScalePrecision = s6xFuncTmp.InputScalePrecision;
                                        s6xFunc.InputUnits = s6xFuncTmp.InputUnits;
                                        s6xFunc.OutputScaleExpression = s6xFuncTmp.OutputScaleExpression;
                                        s6xFunc.OutputScalePrecision = s6xFuncTmp.OutputScalePrecision;
                                        s6xFunc.OutputUnits = s6xFuncTmp.OutputUnits;
                                        s6xFunc.Store = true;
                                        s6xFuncTmp = null;
                                        alS6xFunctions.Add(s6xFunc);
                                        s6xFunc = null;

                                        foundFunctions++;
                                    }
                                }
                            }
                            break;
                    }

                    if (foundRegisters >= 4 && foundScalars >= 6 && foundFunctions >= 1) break;
                }
            }


            arrOps = null;
        }

        // Forced Signatures OBDI2DRegistersRoutines part
        //  20200406 - PYM - New process added
        private static void processForcedFoundSignaturesOBDI2DRegistersRoutines(ref ArrayList alRoutines, ref SADCalib Calibration, ref SADBank Bank8, ref SADS6x S6x, ref ArrayList alAddedOtherElementsUniqueAddressesToRead)
        {
            S6xRoutine rtCodes = null;
            S6xRoutine rtTimings = null;

            foreach (object[] rtObject in alRoutines)
            {
                switch ((SADFixedSigs.Fixed_Routines)rtObject[0])
                {
                    case SADFixedSigs.Fixed_Routines.OBDI_COD_2D:
                        rtCodes = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDI_TIM_2D:
                        rtTimings = (S6xRoutine)rtObject[1];
                        break;
                }
            }

            if (rtCodes == null) return;

            Operation[] arrOps = null;

            S6xStructure s6xCodes = null;
            S6xStructure s6xRegs = null;
            S6xStructure s6xRegsSvc = null;

            string[] arrCodes = null;
            string[] arrRegs = null;
            string[] arrRegsSvc = null;

            string[] arrBytes = null;

            int codesNumber = 0;
            int regsNumber = 0;
            int regsSvcNumber = 0;
            int partialBitsNumber = 0;

            S6xScalar s6xNoFault = null;

            // Structures mapping through Codes
            arrOps = Bank8.getFollowingOPs(rtCodes.AddressInt, 32, 0, false, false, false, false, false, false, false, true);
            foreach (Operation ope in arrOps)
            {
                if (ope == null) break;
                if (ope.isReturn) break;
                if (ope.OperationParams.Length == 0) continue;

                switch (ope.OriginalOPCode)
                {
                    case "65":
                        if (s6xCodes != null) continue;
                        if (ope.OtherElemAddress == string.Empty) continue;
                        try
                        {
                            s6xCodes = new S6xStructure();
                            s6xCodes.BankNum = Calibration.BankNum;
                            s6xCodes.AddressInt = Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress;
                        }
                        catch
                        {
                            s6xCodes = null;
                        }
                        break;
                    case "af":
                        if (ope.OtherElemAddress == string.Empty) continue;
                        if (s6xRegsSvc == null)
                        {
                            try
                            {
                                s6xRegsSvc = new S6xStructure();
                                s6xRegsSvc.BankNum = Calibration.BankNum;
                                s6xRegsSvc.AddressInt = Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress;
                                s6xRegsSvc.Number = 0;
                            }
                            catch
                            {
                                s6xRegsSvc = null;
                            }
                        }
                        else if (s6xRegs == null)
                        {
                            try
                            {
                                s6xRegs = new S6xStructure();
                                s6xRegs.BankNum = Calibration.BankNum;
                                s6xRegs.AddressInt = Convert.ToInt32(ope.OtherElemAddress, 16) - SADDef.EecBankStartAddress;
                                s6xRegs.Number = 0;
                            }
                            catch
                            {
                                s6xRegs = null;
                            }
                        }
                        break;
                }

                if (s6xCodes != null && s6xRegs != null && s6xRegsSvc != null) break;
            }
            if (s6xCodes == null || s6xRegs == null || s6xRegsSvc == null) return;

            // Reading codes (outside of structure, much more simple)
            //      Starts at address - 1 for no fault code which should be 11
            //      Ends when code is not compatible anymore (00 is OK, KO for other values ending with .0)
            ArrayList alCodes = new ArrayList();
            int iAddress = s6xCodes.AddressInt - 1;
            while (true)
            {
                string cCode = Bank8.getByte(iAddress);
                if (s6xNoFault == null && iAddress == s6xCodes.AddressInt - 1)
                {
                    if (cCode == "11")
                    {
                        s6xNoFault = new S6xScalar();
                        s6xNoFault.BankNum = Bank8.Num;
                        s6xNoFault.AddressInt = iAddress;
                    }
                    iAddress++;
                    continue;
                }
                if (Convert.ToInt32(cCode.Substring(0, 1), 16) != Convert.ToInt32(cCode.Substring(0, 1))) break;
                if (Convert.ToInt32(cCode.Substring(1, 1), 16) != Convert.ToInt32(cCode.Substring(1, 1))) break;
                // 20240523 - PYM - It often ends with 70
                if (cCode != "00" && cCode != "70" && cCode.EndsWith("0")) break;
                if (alCodes.Count > 80) break;
                alCodes.Add(cCode);
                iAddress++;
            }
            arrCodes = (string[])alCodes.ToArray(typeof(string));
            alCodes = null;

            codesNumber = arrCodes.Length;

            if (codesNumber == 0) return;
            
            partialBitsNumber = codesNumber % 8;
            regsNumber = (codesNumber - partialBitsNumber) / 8;
            if (partialBitsNumber > 0) regsNumber++;

            regsSvcNumber = (int)((double)regsNumber / 2);
            if (regsNumber % 2 > 0) regsSvcNumber++;

            // Reading Repository
            Repository repoOBDIErrors = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameOBDIErrors, typeof(Repository));
            if (repoOBDIErrors == null) repoOBDIErrors = new Repository();
            SortedList slRepoOBDIErrors = new SortedList();
            foreach (RepositoryItem repoItem in repoOBDIErrors.Items) if (!slRepoOBDIErrors.ContainsKey(repoItem.ShortLabel.ToUpper())) slRepoOBDIErrors.Add(repoItem.ShortLabel.ToUpper(), repoItem);
            repoOBDIErrors = null;

            // Reading registers (outside of structure, much more simple)
            arrRegs = Bank8.getBytesArray(s6xRegs.AddressInt, regsNumber);
            arrRegsSvc = Bank8.getBytesArray(s6xRegsSvc.AddressInt, regsSvcNumber);

            SortedList slRegs = new SortedList();
            // Registers mngt and creation
            for (int iReg = 0; iReg < arrRegs.Length; iReg++)
            {
                S6xRegister s6xReg = new S6xRegister();
                s6xReg.AddressInt = Convert.ToInt32(arrRegs[iReg], 16);
                s6xReg.Label = "oBDeR" + (iReg + 1).ToString();
                s6xReg.Comments = s6xReg.Label + " - ODBI Error Register " + (iReg + 1).ToString();
                for (int iBf = 0; iBf < 8; iBf++)
                {
                    if (arrCodes.Length <= iReg * 8 + iBf) break;
                    RepositoryItem repoItem = (RepositoryItem)slRepoOBDIErrors[arrCodes[iReg * 8 + iBf]];
                    if (repoItem == null) continue;
                    S6xBitFlag s6xBF = new S6xBitFlag();
                    s6xBF.Position = iBf;
                    s6xBF.SetValue = "1";
                    s6xBF.NotSetValue = "0";
                    s6xBF.ShortLabel = "oDBe" + repoItem.ShortLabel;
                    s6xBF.Label = repoItem.Label;
                    s6xBF.Comments = s6xBF.ShortLabel + " - " + s6xBF.Label;
                    s6xBF.Comments += "\r\nWhen bit is set.";
                    s6xBF.Comments += repoItem.Comments;
                    s6xBF.HideParent = true;
                    s6xReg.AddBitFlag(s6xBF);
                    s6xReg.Comments += "\r\nB" + iBf.ToString() + " is for " + s6xBF.ShortLabel + " - " + s6xBF.Label;
                }
                s6xReg.Store = true;
                addRegister(s6xReg, ref Calibration, ref S6x);
                if (!slRegs.ContainsKey(s6xReg.Address)) slRegs.Add(s6xReg.Address, s6xReg);
            }

            // Serivce Registers mngt and creation
            for (int iReg = 0; iReg < arrRegsSvc.Length; iReg++)
            {
                S6xRegister s6xReg = new S6xRegister();
                s6xReg.AddressInt = Convert.ToInt32(arrRegsSvc[iReg], 16);
                s6xReg.Label = "oBDeSR" + (iReg + 1).ToString();
                s6xReg.Comments = s6xReg.Label + " - ODBI Error Register " + (iReg + 1).ToString() + " for Service";
                for (int iBf = 0; iBf < 8; iBf++)
                {
                    if (arrCodes.Length <= iReg * 8 + iBf) break;
                    RepositoryItem repoItem = (RepositoryItem)slRepoOBDIErrors[arrCodes[iReg * 8 + iBf]];
                    if (repoItem == null) continue;
                    S6xBitFlag s6xBF = new S6xBitFlag();
                    s6xBF.Position = iBf;
                    s6xBF.SetValue = "1";
                    s6xBF.NotSetValue = "0";
                    s6xBF.ShortLabel = "oDBeS" + repoItem.ShortLabel;
                    s6xBF.Label = repoItem.Label;
                    s6xBF.Comments = s6xBF.ShortLabel + " - " + s6xBF.Label;
                    s6xBF.Comments += "\r\nWhen bit is set.";
                    s6xBF.Comments += repoItem.Comments;
                    s6xBF.HideParent = true;
                    s6xReg.AddBitFlag(s6xBF);
                    s6xReg.Comments += "\r\nB" + iBf.ToString() + " is for " + s6xBF.ShortLabel + " - " + s6xBF.Label;
                }
                s6xReg.Store = true;
                addRegister(s6xReg, ref Calibration, ref S6x);
                if (!slRegs.ContainsKey(s6xReg.Address)) slRegs.Add(s6xReg.Address, s6xReg);
            }
            
            // ODB1 No Fault Code scalar mngt
            if (s6xNoFault != null)
            {
                s6xNoFault.ShortLabel = "CNOFAULT";
                s6xNoFault.Label = "OBDI No Fault Code";
                s6xNoFault.Byte = true;
                s6xNoFault.Comments = s6xNoFault.ShortLabel + " - " + s6xNoFault.Label;
                s6xNoFault.Store = true;
                addScalar(s6xNoFault, ref Calibration);
                alAddedOtherElementsUniqueAddressesToRead.Add(s6xNoFault.UniqueAddress); 
                s6xNoFault = null;
            }

            // OBDI Codes 2 Digits structure mngt
            s6xCodes.ShortLabel = "OBDIECOD2D";
            s6xCodes.Label = "OBDI Errors Codes 2 Digits";
            string codesDefString = string.Empty;
            for (int iLine = 0; iLine < regsNumber; iLine++)
            {
                if (iLine < regsNumber - 1) codesDefString += "\"" + Tools.RegisterInstruction(arrRegs[iLine]) + " \", \"" + ((S6xRegister)slRegs[arrRegs[iLine]]).Label + " \", \"B0> \", ByteHex:8, \"<B7\"" + (arrRegsSvc.Length > iLine ? ", \" " + Tools.RegisterInstruction(arrRegsSvc[iLine]) + "\", \" " + ((S6xRegister)slRegs[arrRegsSvc[iLine]]).Label + "\"" : string.Empty) + "\\n\r\n";
                else if (partialBitsNumber == 0) codesDefString += "\"" + Tools.RegisterInstruction(arrRegs[iLine]) + "\", \"" + ((S6xRegister)slRegs[arrRegs[iLine]]).Label + "\", \"B0> \", ByteHex:8, \"<B7\"" + (arrRegsSvc.Length > iLine ? ", \" " + Tools.RegisterInstruction(arrRegsSvc[iLine]) + "\", \" " + ((S6xRegister)slRegs[arrRegsSvc[iLine]]).Label + "\"" : string.Empty);
                else codesDefString += "\"" + Tools.RegisterInstruction(arrRegs[iLine]) + " \", \"" + ((S6xRegister)slRegs[arrRegs[iLine]]).Label + " \", \"B0> \", ByteHex:" + partialBitsNumber.ToString();
            }
            s6xCodes.StructDef = codesDefString;
            s6xCodes.Number = 1;
            s6xCodes.Comments = s6xCodes.ShortLabel + " - " + s6xCodes.Label;
            s6xCodes.Comments += "\r\nWhen different from 0, will appear as 2 digits like hexadecimal value.";
            s6xCodes.Comments += "\r\n0x24 will be 2 long and 4 short.";
            s6xCodes.Comments += "\r\n";
            s6xCodes.Comments += "\r\nFirst code is for B0 of first register in structure OBDIEREG2D - OBDI Errors Registers 2 Digits or first register in structure OBDIESREG2D - OBDI Errors Service Registers 2 Digits.";
            s6xCodes.Comments += "\r\nSecond code is for B1 for the same register.";
            s6xCodes.Comments += "\r\n...";
            s6xCodes.Comments += "\r\n8th code is for B7 for the same register.";
            s6xCodes.Comments += "\r\n9th code is for B0 for the second register in structure OBDIEREG2D or OBDIESREG2D";
            s6xCodes.Comments += "\r\nand so on...";
            s6xCodes.Store = true;
            arrBytes = Bank8.getBytesArray(s6xCodes.AddressInt, codesNumber);
            addStructure(s6xCodes, ref Calibration, ref arrBytes);
            alAddedOtherElementsUniqueAddressesToRead.Add(s6xCodes.UniqueAddress);
            s6xCodes = null;
            arrBytes = null;

            // OBDI Codes 2 Digits structure mngt
            s6xRegs.ShortLabel = "OBDIEREG2D";
            s6xRegs.Label = "OBDI Errors Registers 2 Digits";
            string RegsDefString = string.Empty;
            for (int iLine = 0; iLine < arrRegs.Length; iLine++)
            {
                RegsDefString += "\"R\", ByteHex, \"" + ((S6xRegister)slRegs[arrRegs[iLine]]).Label + "\"";
                if (iLine < arrRegs.Length - 1) RegsDefString += "\\n\r\n";
            }
            s6xRegs.StructDef = RegsDefString;
            s6xRegs.Number = 1;
            s6xRegs.Comments = s6xRegs.ShortLabel + " - " + s6xRegs.Label;
            s6xRegs.Store = true;
            arrBytes = Bank8.getBytesArray(s6xRegs.AddressInt, arrRegs.Length);
            addStructure(s6xRegs, ref Calibration, ref arrBytes);
            alAddedOtherElementsUniqueAddressesToRead.Add(s6xRegs.UniqueAddress);
            s6xRegs = null;
            arrBytes = null;

            // OBDI Codes 2 Digits structure mngt
            s6xRegsSvc.ShortLabel = "OBDIESREG2D";
            s6xRegsSvc.Label = "OBDI Errors Service Registers 2 Digits";
            string RegsSvcDefString = string.Empty;
            for (int iLine = 0; iLine < arrRegsSvc.Length; iLine++)
            {
                RegsSvcDefString += "\"R\", ByteHex, \"" + ((S6xRegister)slRegs[arrRegsSvc[iLine]]).Label + "\"";
                if (iLine < arrRegsSvc.Length - 1) RegsSvcDefString += "\\n\r\n";
            }
            s6xRegsSvc.StructDef = RegsSvcDefString;
            s6xRegsSvc.Number = 1;
            s6xRegsSvc.Comments = s6xRegsSvc.ShortLabel + " - " + s6xRegsSvc.Label;
            s6xRegsSvc.Store = true;
            arrBytes = Bank8.getBytesArray(s6xRegsSvc.AddressInt, arrRegsSvc.Length);
            addStructure(s6xRegsSvc, ref Calibration, ref arrBytes);
            alAddedOtherElementsUniqueAddressesToRead.Add(s6xRegsSvc.UniqueAddress);
            s6xRegsSvc = null;
            arrBytes = null;

            // Structures mapping through Timings
            if (rtTimings != null)
            {
                SortedList slAddresses = new SortedList();
                arrOps = Bank8.getFollowingOPs(rtTimings.AddressInt, 48, 0, false, false, false, false, false, false, false, true);
                foreach (Operation ope in arrOps)
                {
                    if (ope == null) break;
                    //if (ope.isReturn) break;  // Some returns on the path
                    if (ope.OperationParams.Length == 0) continue;

                    switch (ope.OriginalOPCode)
                    {
                        case "4f":
                            if (ope.OtherElemAddress == string.Empty) continue;
                            if (!slAddresses.ContainsKey(ope.OtherElemAddress)) slAddresses.Add(ope.OtherElemAddress, ope.OtherElemAddress);
                            break;
                    }

                    if (slAddresses.Count >= 3) break;
                }
                if (slAddresses.Count != 3) return;     // 3 structures with different addresses to be found
                for (int iPos = 0; iPos < slAddresses.Count; iPos++)
                {
                    int iStep = 0;
                    switch (iPos)
                    {
                        case 0:
                            iStep = 2;
                            break;
                        case 1:
                            iStep = 3;
                            break;
                        case 2:
                            iStep = 1;
                            break;
                    }
                    // OBDI Codes 2 Digits structure mngt
                    S6xStructure s6xTimingStep = new S6xStructure();
                    s6xTimingStep.BankNum = Bank8.Num;
                    s6xTimingStep.AddressInt = Convert.ToInt32(slAddresses.GetByIndex(iPos).ToString(), 16) - SADDef.EecBankStartAddress + 2;  // Structures are beginning 2 bytes later
                    s6xTimingStep.ShortLabel = "OBDIEOUTTM" + iStep.ToString();
                    s6xTimingStep.Label = "OBDI Errors Output Led Timing Step " + iStep.ToString();
                    s6xTimingStep.StructDef = "Word(X/32)"; // To give mSecs
                    s6xTimingStep.Number = 5;               // Size is fixed
                    s6xTimingStep.Comments = s6xTimingStep.ShortLabel + " - " + s6xTimingStep.Label;
                    s6xTimingStep.Comments += "\r\nIn mSecs.";
                    s6xTimingStep.Store = true;
                    arrBytes = Bank8.getBytesArray(s6xTimingStep.AddressInt, 5 * 2);
                    addStructure(s6xTimingStep, ref Calibration, ref arrBytes);
                    alAddedOtherElementsUniqueAddressesToRead.Add(s6xTimingStep.UniqueAddress);
                    arrBytes = null;
                }
            }
        }

        // Forced Signatures OBDI3DRegistersRoutines part
        //  20200405 - PYM - New process added
        private static void processForcedFoundSignaturesOBDI3DRegistersRoutines(ref ArrayList alRoutines, ref SADCalib Calibration, ref SADBank Bank8, ref SADS6x S6x, ref ArrayList alAddedOtherElementsUniqueAddressesToRead)
        {
            S6xRoutine rtCodes = null;
            S6xRoutine rtCount = null;
            S6xRoutine rtMalfunc = null;
            S6xRoutine rtMalfuncBT = null;
            S6xRoutine rtMalfuncEX = null;
            S6xRoutine rtMalfuncLW = null;
            S6xRoutine rtMalfuncSB = null;

            foreach (object[] rtObject in alRoutines)
            {
                switch ((SADFixedSigs.Fixed_Routines)rtObject[0])
                {
                    case SADFixedSigs.Fixed_Routines.OBDI_COD_3D:
                        rtCodes = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDI_CNT_3D:
                        rtCount = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDI_MALFUNC:
                        rtMalfunc = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_BT:
                        rtMalfuncBT = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_EX:
                        rtMalfuncEX = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_LW:
                        rtMalfuncLW = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_SB:
                        rtMalfuncSB = (S6xRoutine)rtObject[1];
                        break;
                }
            }

            if (rtCodes == null) return;

            Operation[] arrOps = null;
            string[] arrBytes = null;

            S6xStructure s6xCodes = null;

            string[] arrCodes = null;

            int codesNumber = 0;
            int regsNumber = 0;
            int partialBitsNumber = 0;

            // Structures mapping through Codes
            arrOps = Bank8.getFollowingOPs(rtCodes.AddressInt, 32, 0, false, false, false, false, false, false, false, true);
            foreach (Operation ope in arrOps)
            {
                if (ope == null) break;
                if (ope.isReturn) break;
                if (ope.OperationParams.Length == 0) continue;

                object[] arrPointersValues = null;

                switch (ope.OriginalOPCode)
                {
                    case "89":
                        if (codesNumber > 0) continue;
                        arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                        if ((bool)arrPointersValues[0]) continue;
                        codesNumber = (int)arrPointersValues[2];
                        break;
                    case "a3":
                        if (s6xCodes != null) continue;
                        arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                        if (!(bool)arrPointersValues[0] && arrPointersValues.Length <= 3) continue;
                        {
                            s6xCodes = new S6xStructure();
                            s6xCodes.BankNum = Calibration.BankNum;
                            s6xCodes.AddressInt = (int)arrPointersValues[4] - SADDef.EecBankStartAddress;
                        }
                        break;
                }

                if (codesNumber > 0 && s6xCodes != null) break;
            }
            if (codesNumber <= 0 || s6xCodes == null) return;

            partialBitsNumber = codesNumber % 8;
            regsNumber = (codesNumber - partialBitsNumber) / 8;
            if (partialBitsNumber > 0) regsNumber++;

            // Reading codes (outside of structure, much more simple)
            arrBytes = Bank8.getBytesArray(s6xCodes.AddressInt, 2 * codesNumber);
            arrCodes = new string[codesNumber];
            for (int iNum = 0; iNum < arrCodes.Length; iNum++) arrCodes[iNum] = Convert.ToInt32(Tools.getWord(iNum * 2, true, ref arrBytes)).ToString();

            // Reading Repository
            Repository repoOBDIErrors = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameOBDIErrors, typeof(Repository));
            if (repoOBDIErrors == null) repoOBDIErrors = new Repository();
            SortedList slRepoOBDIErrors = new SortedList();
            foreach (RepositoryItem repoItem in repoOBDIErrors.Items) if (!slRepoOBDIErrors.ContainsKey(repoItem.ShortLabel.ToUpper())) slRepoOBDIErrors.Add(repoItem.ShortLabel.ToUpper(), repoItem);
            repoOBDIErrors = null;

            // OBDI Codes 3 Digits structure mngt
            s6xCodes.ShortLabel = "OBDIECOD3D";
            s6xCodes.Label = "OBDI Errors Codes 3 Digits";
            int codesLastLineCount = codesNumber % 8;
            int codesFullLines = (codesNumber - codesLastLineCount) / 8;
            string codesDefString = string.Empty;
            for (int iLine = 0; iLine < regsNumber; iLine++)
            {
                if (iLine < regsNumber - 1) codesDefString += "WordHex:8\\n\r\n";
                else if (partialBitsNumber == 0) codesDefString += "WordHex:8";
                else codesDefString += "WordHex:" + partialBitsNumber.ToString();
            }
            s6xCodes.StructDef = codesDefString;
            s6xCodes.Number = 1;
            s6xCodes.Comments = s6xCodes.ShortLabel + " - " + s6xCodes.Label;
            s6xCodes.Comments += "\r\nWhen different from 0, will appear as 3 digits like hexadecimal value.";
            s6xCodes.Comments += "\r\n0x244 will be 2 long long, 4 long and 4 short.";
            s6xCodes.Store = true;
            addStructure(s6xCodes, ref Calibration, ref arrBytes);
            alAddedOtherElementsUniqueAddressesToRead.Add(s6xCodes.UniqueAddress);
            s6xCodes = null;
            arrBytes = null;

            // Count Routine to manage OBDI Count related scalars
            if (rtCount != null)
            {
                S6xScalar rCountFirst = null;
                S6xScalar rThresFirst = null;
                RBase rBase = null; 

                // First scalar address to be found in routine
                arrOps = Bank8.getFollowingOPs(rtCount.AddressInt, 32, 0, false, false, false, false, false, false, false, true);
                foreach (Operation ope in arrOps)
                {
                    if (ope == null) break;
                    if (ope.isReturn) break;
                    if (ope.OperationParams.Length == 0) continue;

                    switch (ope.OriginalOPCode)
                    {
                        case "45":
                            if (rCountFirst != null) continue;
                            if (ope.alCalibrationElems == null) continue;
                            if (ope.alCalibrationElems.Count != 1) continue;
                            rCountFirst = new S6xScalar();
                            rCountFirst.BankNum = ((CalibrationElement)ope.alCalibrationElems[0]).BankNum;
                            rCountFirst.AddressInt = ((CalibrationElement)ope.alCalibrationElems[0]).AddressInt;
                            break;
                        case "44":
                            if (rThresFirst != null) continue;
                            if (rBase != null) continue;
                            if (ope.OperationParams[0].EmbeddedParam == null) continue;
                            if (ope.OperationParams[0].EmbeddedParam.GetType() == typeof(RBase)) rBase = (RBase)ope.OperationParams[0].EmbeddedParam;
                            else if (ope.OperationParams[0].EmbeddedParam.GetType() == typeof(Register)) rBase = ((Register)ope.OperationParams[0].EmbeddedParam).RBase;
                            break;
                        case "b3":
                            if (rThresFirst != null) continue;
                            if (rBase == null) continue;
                            if (ope.OperationParams[0].EmbeddedParam == null) continue;
                            if (ope.OperationParams[0].EmbeddedParam.GetType() != typeof(object[])) continue;
                            if (((object[])ope.OperationParams[0].EmbeddedParam).Length != 2) continue;
                            if (((object[])ope.OperationParams[0].EmbeddedParam)[1].GetType() != typeof(string)) continue;
                            try
                            {
                                rThresFirst = new S6xScalar();
                                rThresFirst.BankNum = rBase.BankNum;
                                rThresFirst.AddressInt = rBase.AddressBankInt + Convert.ToInt32(((object[])ope.OperationParams[0].EmbeddedParam)[1].ToString(), 16);
                            }
                            catch
                            {
                                rThresFirst = null;
                            }
                            break;
                    }

                    if (rCountFirst != null && rThresFirst != null) break;
                }

                // OBDI Codes applied to Count Scalars
                if (rCountFirst != null)
                {
                    for (int iNum = 0; iNum < arrCodes.Length; iNum++)
                    {
                        int iAddress = rCountFirst.AddressInt + iNum;
                        S6xScalar s6xCount = new S6xScalar();
                        s6xCount.BankNum = rCountFirst.BankNum;
                        s6xCount.AddressInt = iAddress;
                        s6xCount.ShortLabel = "C" + arrCodes[iNum] + "UP";
                        s6xCount.Label = "Error " + arrCodes[iNum] + " Up Count";
                        RepositoryItem repoItem = (RepositoryItem)slRepoOBDIErrors[(arrCodes[iNum]).ToUpper()];
                        if (repoItem == null) s6xCount.Comments = s6xCount.ShortLabel + " - " + s6xCount.Label;
                        else
                        {
                            s6xCount.Label += " (" + repoItem.Label + ")";
                            s6xCount.Comments = s6xCount.ShortLabel + " - " + s6xCount.Label;
                            s6xCount.Comments += "\r\n" + SADDef.repoCommentsHeaderOBDIErrors.Replace("#OBDCODE#", repoItem.ShortLabel);
                            s6xCount.Comments += "\r\n" + repoItem.Label;
                            if (repoItem.Comments != repoItem.FullLabel) s6xCount.Comments += "\r\n" + repoItem.Comments;
                            repoItem = null;
                        }
                        s6xCount.Store = true;
                        addScalar(s6xCount, ref Calibration);
                        alAddedOtherElementsUniqueAddressesToRead.Add(s6xCount.UniqueAddress);
                        s6xCount = null;
                    }
                    rCountFirst = null;
                }

                if (rThresFirst != null)
                {
                    for (int iNum = 0; iNum < arrCodes.Length; iNum++)
                    {
                        int iAddress = rThresFirst.AddressInt + iNum;
                        S6xScalar s6xCount = new S6xScalar();
                        s6xCount.BankNum = rThresFirst.BankNum;
                        s6xCount.AddressInt = iAddress;
                        s6xCount.ShortLabel = "C" + arrCodes[iNum] + "LVL";
                        s6xCount.Label = "Error " + arrCodes[iNum] + " Threshold";
                        RepositoryItem repoItem = (RepositoryItem)slRepoOBDIErrors[(arrCodes[iNum]).ToUpper()];
                        if (repoItem == null) s6xCount.Comments = s6xCount.ShortLabel + " - " + s6xCount.Label;
                        else
                        {
                            s6xCount.Label += " (" + repoItem.Label + ")";
                            s6xCount.Comments = s6xCount.ShortLabel + " - " + s6xCount.Label;
                            s6xCount.Comments += "\r\n" + SADDef.repoCommentsHeaderOBDIErrors.Replace("#OBDCODE#", repoItem.ShortLabel);
                            s6xCount.Comments += "\r\n" + repoItem.Label;
                            if (repoItem.Comments != repoItem.FullLabel) s6xCount.Comments += "\r\n" + repoItem.Comments;
                            repoItem = null;
                        }
                        s6xCount.Store = true;
                        addScalar(s6xCount, ref Calibration);
                        alAddedOtherElementsUniqueAddressesToRead.Add(s6xCount.UniqueAddress);
                        s6xCount = null;
                    }
                    rCountFirst = null;
                }
            }

            ArrayList alRegistersVsCodes = new ArrayList();
            Call cCall = null;

            // Registers Identification through OBD
            if (rtMalfunc != null)
            {
                // Not Managed for now
            }

            if (rtMalfuncBT != null)
            {
                cCall = (Call)Calibration.slCalls[rtMalfuncBT.UniqueAddress];
                if (cCall != null)
                {
                    foreach (Caller cCaller in cCall.Callers)
                    {
                        if (cCaller == null) continue;
                        if (cCaller.CallType != CallType.Call && cCaller.CallType != CallType.ShortCall) continue;
                        Operation ope = (Operation)Bank8.slOPs[cCaller.UniqueAddress];
                        if (ope == null) continue;
                        if (ope.CallArguments == null) continue;
                        if (ope.CallArguments.Length != 4) continue;
                        int regAddr = ope.CallArguments[0].DecryptedValueInt;
                        string addrVMinUAddr = Tools.UniqueAddress(Calibration.BankNum, ope.CallArguments[1].DecryptedValueInt);
                        string addrVMaxUAddr = Tools.UniqueAddress(Calibration.BankNum, ope.CallArguments[2].DecryptedValueInt);
                        string obdCode = ope.CallArguments[3].DecryptedValue;
                        alRegistersVsCodes.Add(new object[] { obdCode, regAddr, SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_BT, new string[] { addrVMinUAddr, addrVMaxUAddr } });
                    }
                }
            }

            if (rtMalfuncEX != null)
            {
                cCall = (Call)Calibration.slCalls[rtMalfuncEX.UniqueAddress];
                if (cCall != null)
                {
                    foreach (Caller cCaller in cCall.Callers)
                    {
                        if (cCaller == null) continue;
                        if (cCaller.CallType != CallType.Call && cCaller.CallType != CallType.ShortCall) continue;
                        Operation ope = (Operation)Bank8.slOPs[cCaller.UniqueAddress];
                        if (ope == null) continue;
                        if (ope.CallArguments == null) continue;
                        if (ope.CallArguments.Length != 5) continue;
                        int regAddr = ope.CallArguments[0].DecryptedValueInt;
                        string addrVal1UAddr = Tools.UniqueAddress(Calibration.BankNum, ope.CallArguments[1].DecryptedValueInt);
                        string addrVal2UAddr = Tools.UniqueAddress(Calibration.BankNum, ope.CallArguments[2].DecryptedValueInt);
                        string obdCode1 = ope.CallArguments[3].DecryptedValue;
                        string obdCode2 = ope.CallArguments[4].DecryptedValue;
                        alRegistersVsCodes.Add(new object[] { obdCode1, regAddr, SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_EX, new string[] { addrVal1UAddr, addrVal2UAddr } });
                        alRegistersVsCodes.Add(new object[] { obdCode2, regAddr, SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_EX, new string[] { addrVal1UAddr, addrVal2UAddr } });
                    }
                }
            }

            if (rtMalfuncLW != null)
            {
                cCall = (Call)Calibration.slCalls[rtMalfuncLW.UniqueAddress];
                if (cCall != null)
                {
                    foreach (Caller cCaller in cCall.Callers)
                    {
                        if (cCaller == null) continue;
                        if (cCaller.CallType != CallType.Call && cCaller.CallType != CallType.ShortCall) continue;
                        Operation ope = (Operation)Bank8.slOPs[cCaller.UniqueAddress];
                        if (ope == null) continue;
                        if (ope.CallArguments == null) continue;
                        if (ope.CallArguments.Length != 3) continue;
                        int addr1 = ope.CallArguments[0].DecryptedValueInt;
                        int addr2 = ope.CallArguments[1].DecryptedValueInt;
                        int regAddr = 0;
                        string addrVal1UAddr = string.Empty;
                        string addrVal2UAddr = string.Empty;
                        if (addr1 < addr2)
                        {
                            regAddr = addr1;
                            addrVal2UAddr = Tools.UniqueAddress(Calibration.BankNum, addr2);
                        }
                        else
                        {
                            regAddr = addr2;
                            addrVal1UAddr = Tools.UniqueAddress(Calibration.BankNum, addr1);
                        }
                        string obdCode = ope.CallArguments[2].DecryptedValue;
                        alRegistersVsCodes.Add(new object[] { obdCode, regAddr, SADFixedSigs.Fixed_Routines.OBDI_MALFUNC_LW, new string[] { addrVal1UAddr, addrVal2UAddr } });
                    }
                }
            }

            if (rtMalfuncSB != null)
            {
                // Not Managed for now
            }

            foreach (object[] arrRegisterVsCode in alRegistersVsCodes)
            {
                string obdCode = (string)arrRegisterVsCode[0];
                int regAddr = (int)arrRegisterVsCode[1];

                string regUAddr = Tools.RegisterUniqueAddress(regAddr);

                S6xRegister s6xTmp = null;
                S6xRegister s6xReg = (S6xRegister)S6x.slProcessRegisters[regUAddr];
                // Register Identification
                if (s6xReg == null)
                {
                    switch (obdCode)
                    {
                        case "328":
                        case "334":
                        case "335":
                        case "344":
                        case "345":
                            s6xTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.IEVP);
                            break;
                        case "112":
                        case "113":
                        case "114":
                            s6xTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.IACT);
                            break;
                        case "116":
                        case "117":
                        case "118":
                            s6xTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.IECT);
                            break;
                        case "121":
                        case "122":
                        case "123":
                            s6xTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.ITP);
                            break;
                        case "126":
                            s6xTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.IMAP);
                            break;
                        case "159":
                            s6xTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.IMAF);
                            break;
                        case "513":
                            s6xTmp = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.VBAT);
                            break;
                    }
                    if (s6xTmp != null)
                    {
                        s6xReg = new S6xRegister(regAddr);
                        s6xReg.Label = s6xTmp.Label;
                        s6xReg.Comments = s6xTmp.Comments;
                        s6xReg.ScaleExpression = s6xTmp.ScaleExpression;
                        s6xReg.ScalePrecision = s6xTmp.ScalePrecision;
                        s6xReg.Units = s6xTmp.Units;
                        s6xReg.Store = true;
                        addRegister(s6xReg, ref Calibration, ref S6x);
                        s6xReg = (S6xRegister)S6x.slRegisters[regUAddr];
                    }
                }
            }
            alRegistersVsCodes = null;
        }

        // Forced Signatures OBDIIRegistersRoutines part
        //  20200402 - PYM - New process added
        private static void processForcedFoundSignaturesOBDIIRegistersRoutines(ref ArrayList alRoutines, ref SADCalib Calibration, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9, ref SADS6x S6x, ref ArrayList alAddedOtherElementsUniqueAddressesToRead)
        {
            SADBank Bank = null;
            
            S6xRoutine rtInit = null;
            S6xRoutine rtReset = null;
            S6xRoutine rtFlags = null;
            S6xRoutine rtClearMalf = null;
            S6xRoutine rtMalfunction = null;

            SortedList slRegistersUAddrOBDCodes = new SortedList();

            Operation[] arrOps = null;

            foreach (object[] rtObject in alRoutines)
            {
                switch ((SADFixedSigs.Fixed_Routines)rtObject[0])
                {
                    case SADFixedSigs.Fixed_Routines.OBDII_REG_INIT:
                        rtInit = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDII_REG_RESET:
                        rtReset = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDII_REG_FLAGS:
                        rtFlags = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDII_CLEAR_MALF:
                        rtClearMalf = (S6xRoutine)rtObject[1];
                        break;
                    case SADFixedSigs.Fixed_Routines.OBDII_MALFUNCTION:
                        rtMalfunction = (S6xRoutine)rtObject[1];
                        break;
                }
            }

            if (rtInit == null) return;
            if (rtReset == null) return;

            int iMode = -1;
            switch (rtReset.SignatureKey)
            {
                case "OBDII_REG_RESET_01":
                case "OBDII_REG_RESET_01_01":
                    iMode = 1;  // ECU Generation 1
                    break;
                case "OBDII_REG_RESET_02":
                    iMode = 2;  // ECU Generation 2
                    break;
            }

            if (rtFlags == null)
            {
                if (iMode == 2)
                {
                    return;
                }
            }

            S6xRegister rFirst = null;
            S6xRegister rLast = null;
            S6xRegister rMisFfCode = null;
            S6xRegister rUnkMisCodeReg = null;
            S6xRegister rMilOnFlagsReg = null;

            // Registers mapping through Init
            switch (rtInit.BankNum)
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
            arrOps = Bank.getFollowingOPs(rtInit.AddressInt, 32, 0, false, false, false, false, false, false, false, true);
            Bank = null;
            foreach (Operation ope in arrOps)
            {
                if (ope == null) break;
                if (ope.isReturn) break;
                if (ope.OperationParams.Length == 0) continue;

                object[] arrPointersValues = null;

                switch (ope.OriginalOPCode)
                {
                    case "c3":
                        if (ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] == "00")
                        {
                            arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                            if (!(bool)arrPointersValues[0]) continue;
                            if (rMisFfCode == null) rMisFfCode = new S6xRegister((int)arrPointersValues[2]);
                            else if (rUnkMisCodeReg == null) rUnkMisCodeReg = new S6xRegister((int)arrPointersValues[2]);
                        }
                        break;
                    case "c7":
                        if (ope.OriginalOpArr[ope.OriginalOpArr.Length - 1] == "00")
                        {
                            arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                            if (!(bool)arrPointersValues[0]) continue;
                            if (rMilOnFlagsReg == null) rMilOnFlagsReg = new S6xRegister((int)arrPointersValues[2]);
                        }
                        break;
                    case "a1":
                        arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].CalculatedParam);
                        if ((bool)arrPointersValues[0]) continue;
                        if (rFirst == null) rFirst = new S6xRegister((int)arrPointersValues[2]);
                        break;
                    case "89":
                        arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].CalculatedParam);
                        if ((bool)arrPointersValues[0]) continue;
                        if (rLast == null) rLast = new S6xRegister((int)arrPointersValues[2] - 2);
                        break;
                }

                if (rFirst != null && rLast != null && rMisFfCode != null && rMilOnFlagsReg != null)
                {
                    if (iMode == 1)
                    {
                        break;
                    }
                    else if (iMode == 2)
                    {
                        if (rUnkMisCodeReg != null) break;
                    }
                }
            }

            if (rFirst == null && rLast == null && rMisFfCode == null && rMilOnFlagsReg == null) return;
            if (rUnkMisCodeReg == null)
            {
                if (iMode == 2) return;
            }

            S6xStructure s6xCodes = null;
            S6xStructure s6xSwitches = null;
            S6xStructure s6xFlags = null;

            // Structures mapping through Reset
            // Registers mapping through Init
            switch (rtReset.BankNum)
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
            arrOps = Bank.getFollowingOPs(rtReset.AddressInt, 32, 0, false, false, false, false, false, false, false, true);
            Bank = null;
            foreach (Operation ope in arrOps)
            {
                if (ope == null) break;
                if (ope.isReturn) break;
                if (ope.OperationParams.Length == 0) continue;

                object[] arrPointersValues = null;

                switch (ope.OriginalOPCode)
                {
                    case "65":
                        arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                        if ((bool)arrPointersValues[0]) continue;
                        if (iMode == 1)
                        {
                            if (s6xCodes == null)
                            {
                                s6xCodes = new S6xStructure();
                                s6xCodes.BankNum = Calibration.BankNum;
                                s6xCodes.AddressInt = (int)arrPointersValues[2] - SADDef.EecBankStartAddress;
                            }
                        }
                        else if (iMode == 2)
                        {
                            if (s6xSwitches == null)
                            {
                                s6xSwitches = new S6xStructure();
                                s6xSwitches.BankNum = Calibration.BankNum;
                                s6xSwitches.AddressInt = (int)arrPointersValues[2] - SADDef.EecBankStartAddress;
                            }
                            else if (s6xCodes == null)
                            {
                                s6xCodes = new S6xStructure();
                                s6xCodes.BankNum = Calibration.BankNum;
                                s6xCodes.AddressInt = (int)arrPointersValues[2] - SADDef.EecBankStartAddress;
                            }
                        }
                        break;
                }

                if (s6xCodes != null)
                {
                    if (iMode == 1)
                    {
                        break;
                    }
                    else if (iMode == 2)
                    {
                        if (s6xSwitches != null) break;
                    }
                }
            }
            arrOps = null;

            if (rtFlags != null)
            {
                // Structures mapping through Flags
                switch (rtFlags.BankNum)
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
                arrOps = Bank.getFollowingOPs(rtFlags.AddressInt, 32, 0, false, false, false, false, false, false, false, true);
                Bank = null;
                foreach (Operation ope in arrOps)
                {
                    if (ope == null) break;
                    if (ope.isReturn) break;
                    if (ope.OperationParams.Length == 0) continue;

                    object[] arrPointersValues = null;

                    switch (ope.OriginalOPCode)
                    {
                        case "65":
                            arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].InstructedParam);
                            if ((bool)arrPointersValues[0]) continue;
                            s6xFlags = new S6xStructure();
                            s6xFlags.BankNum = Calibration.BankNum;
                            s6xFlags.AddressInt = (int)arrPointersValues[2] - SADDef.EecBankStartAddress;
                            break;
                    }

                    if (s6xFlags != null) break;
                }
                arrOps = null;
            }

            if (s6xCodes == null) return;
            if (s6xSwitches == null || s6xFlags == null)
            {
                if (iMode == 2) return;
            }

            SADBank calibrationBank = Bank1;
            string[] arrBytes = null;
            string[] arrCodes = null;
            int[] arrSwitchesAddresses = null;

            if (calibrationBank == null) return;

            s6xCodes.Number = (rLast.AddressInt - rFirst.AddressInt) / 2 + 1;
            if (s6xCodes.Number <= 0) return;

            if (s6xSwitches != null) s6xSwitches.Number = s6xCodes.Number;
            if (s6xFlags != null) s6xFlags.Number = s6xCodes.Number;

            // Reading codes (outside of structure, much more simple)
            arrBytes = calibrationBank.getBytesArray(s6xCodes.AddressInt, 2 * s6xCodes.Number);
            arrCodes = new string[s6xCodes.Number];
            for (int iNum = 0; iNum < arrCodes.Length; iNum++) arrCodes[iNum] = Tools.getWord(iNum * 2, true, ref arrBytes);

            // Reading Repository
            Repository repoOBDIIErrors = (Repository)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.repoFileNameOBDIIErrors, typeof(Repository));
            if (repoOBDIIErrors == null) repoOBDIIErrors = new Repository();
            SortedList slRepoOBDIIErrors = new SortedList();
            foreach (RepositoryItem repoItem in repoOBDIIErrors.Items) if (!slRepoOBDIIErrors.ContainsKey(repoItem.ShortLabel.ToUpper())) slRepoOBDIIErrors.Add(repoItem.ShortLabel.ToUpper(), repoItem);
            repoOBDIIErrors = null;

            // OBDII Codes structure mngt
            s6xCodes.ShortLabel = "OBDIIECOD";
            s6xCodes.Label = "OBDII Errors Codes";
            s6xCodes.StructDef = "\"P\",WordHex,\"REG \",NumHex(X*2+" + rFirst.AddressInt + ")";
            s6xCodes.Comments = s6xCodes.ShortLabel + " - " + s6xCodes.Label;
            s6xCodes.Comments += "\r\nValue is P0 + X (P0420 for example).";
            s6xCodes.Comments += "\r\nValue is related to register [" + rFirst.Address + " + RowNumber]. RowNumber starts at 0.";
            s6xCodes.Store = true;
            addStructure(s6xCodes, ref Calibration, ref arrBytes);
            alAddedOtherElementsUniqueAddressesToRead.Add(s6xCodes.UniqueAddress);
            s6xCodes = null;
            arrBytes = null;

            // OBDII Codes applied to related registers
            for (int iNum = 0; iNum < arrCodes.Length; iNum++)
            {
                S6xRegister s6xReg = new S6xRegister(rFirst.AddressInt + iNum * 2);
                // 20200908 - CNT register added
                S6xRegister s6xRegCnt = new S6xRegister(rFirst.AddressInt + iNum * 2 + 1);
                s6xReg.Label = "p" + arrCodes[iNum] + "_RECORD";
                s6xRegCnt.Label = "p" + arrCodes[iNum] + "CNT";

                // 20200908 - BitFlags forced
                
                // RECORD
                S6xBitFlag s6xBF = null;
                s6xBF = new S6xBitFlag();
                s6xBF.Position = 4;
                s6xBF.ShortLabel = "p" + arrCodes[iNum] + "MIL_ON";
                s6xBF.Label = s6xBF.ShortLabel;
                s6xBF.SetValue = "1";
                s6xBF.NotSetValue = "0";
                s6xReg.AddBitFlag(s6xBF);
                s6xBF = null;

                s6xBF = new S6xBitFlag();
                s6xBF.Position = 5;
                s6xBF.ShortLabel = "p" + arrCodes[iNum] + "FAULT";
                s6xBF.Label = s6xBF.ShortLabel;
                s6xBF.SetValue = "1";
                s6xBF.NotSetValue = "0";
                s6xReg.AddBitFlag(s6xBF);
                s6xBF = null;

                s6xBF = new S6xBitFlag();
                s6xBF.Position = 6;
                s6xBF.ShortLabel = "p" + arrCodes[iNum] + "UPDATED";
                s6xBF.Label = s6xBF.ShortLabel;
                s6xBF.SetValue = "1";
                s6xBF.NotSetValue = "0";
                s6xReg.AddBitFlag(s6xBF);
                s6xBF = null;

                s6xBF = new S6xBitFlag();
                s6xBF.Position = 7;
                s6xBF.ShortLabel = "p" + arrCodes[iNum] + "MALF";
                s6xBF.Label = s6xBF.ShortLabel;
                s6xBF.SetValue = "1";
                s6xBF.NotSetValue = "0";
                s6xReg.AddBitFlag(s6xBF);
                s6xBF = null;

                //CNT
                s6xBF = new S6xBitFlag();
                s6xBF.Position = 7;
                s6xBF.ShortLabel = "p" + arrCodes[iNum] + "FAULT_A";
                s6xBF.Label = s6xBF.ShortLabel;
                s6xBF.SetValue = "1";
                s6xBF.NotSetValue = "0";
                s6xRegCnt.AddBitFlag(s6xBF);
                s6xBF = null;

                RepositoryItem repoItem = (RepositoryItem)slRepoOBDIIErrors[("P" + arrCodes[iNum]).ToUpper()];
                if (repoItem != null)
                {
                    s6xReg.Comments = SADDef.repoCommentsHeaderOBDIIErrors.Replace("#OBDCODE#", repoItem.ShortLabel);
                    s6xReg.Comments += "\r\n" + repoItem.Label;
                    if (repoItem.Comments != repoItem.FullLabel) s6xReg.Comments += "\r\n" + repoItem.Comments;
                    s6xRegCnt.Comments = s6xReg.Comments;
                    repoItem = null;
                }
                s6xReg.Store = true;
                s6xRegCnt.Store = true;
                addRegister(s6xReg, ref Calibration, ref S6x);
                addRegister(s6xRegCnt, ref Calibration, ref S6x);
                if (!slRegistersUAddrOBDCodes.ContainsKey(s6xReg.UniqueAddress)) slRegistersUAddrOBDCodes.Add(s6xReg.UniqueAddress, arrCodes[iNum]);
            }

            if (s6xSwitches != null)
            {
                // Reading Switches Addresses (outside of structure, much more simple)
                arrBytes = calibrationBank.getBytesArray(s6xSwitches.AddressInt, 2 * s6xSwitches.Number);
                arrSwitchesAddresses = new int[s6xSwitches.Number];
                for (int iNum = 0; iNum < arrSwitchesAddresses.Length; iNum++) arrSwitchesAddresses[iNum] = Tools.getWordInt(iNum * 2, false, true, ref arrBytes) - SADDef.EecBankStartAddress;

                // Switches Structure Mngt
                s6xSwitches.ShortLabel = "OBDIIESWAD";
                s6xSwitches.Label = "OBDII Errors Switches Addresses";
                s6xSwitches.StructDef = "\"SW AD \",WordHex,\"REG \",NumHex(X*2+" + rFirst.AddressInt + ")";
                s6xSwitches.Comments = s6xSwitches.ShortLabel + " - " + s6xSwitches.Label;
                s6xSwitches.Comments += "\r\nValue is the address where mode is set.";
                s6xSwitches.Comments += "\r\nValue is related to register [" + rFirst.Address + " + RowNumber]. RowNumber starts at 0.";
                s6xSwitches.Store = true;
                addStructure(s6xSwitches, ref Calibration, ref arrBytes);
                alAddedOtherElementsUniqueAddressesToRead.Add(s6xSwitches.UniqueAddress);
                s6xSwitches = null;
                arrBytes = null;

                // OBDII Switches applied to related scalars
                for (int iNum = 0; iNum < arrSwitchesAddresses.Length; iNum++)
                {
                    S6xScalar s6xSwitch = new S6xScalar();
                    s6xSwitch.BankNum = calibrationBank.Num;
                    s6xSwitch.AddressInt = arrSwitchesAddresses[iNum];
                    s6xSwitch.ShortLabel = "P" + arrCodes[iNum] + "SW";
                    s6xSwitch.Label = "P" + arrCodes[iNum] + " Switch";
                    s6xSwitch.Comments = s6xSwitch.ShortLabel + " - " + s6xSwitch.Label;
                    RepositoryItem repoItem = (RepositoryItem)slRepoOBDIIErrors[("P" + arrCodes[iNum]).ToUpper()];
                    if (repoItem != null)
                    {
                        s6xSwitch.Comments += "\r\n" + SADDef.repoCommentsHeaderOBDIIErrors.Replace("#OBDCODE#", repoItem.ShortLabel);
                        s6xSwitch.Comments += "\r\n" + repoItem.Label;
                        if (repoItem.Comments != repoItem.FullLabel) s6xSwitch.Comments += "\r\n" + repoItem.Comments;
                        repoItem = null;
                    }
                    s6xSwitch.Store = true;
                    addScalar(s6xSwitch, ref Calibration);
                    alAddedOtherElementsUniqueAddressesToRead.Add(s6xSwitch.UniqueAddress);
                }
            }

            if (s6xFlags != null)
            {
                s6xFlags.ShortLabel = "OBDIIEIDS";
                s6xFlags.Label = "OBDII Errors IDs";
                s6xFlags.StructDef = "\"ID \",ByteHex,\"REG \",NumHex(X*2+" + rFirst.AddressInt + ")";
                s6xFlags.Comments = s6xFlags.ShortLabel + " - " + s6xFlags.Label;
                s6xFlags.Comments += "\r\nValue is related to register [" + rFirst.Address + " + RowNumber]. RowNumber starts at 0.";
                s6xFlags.Store = true;
                arrBytes = calibrationBank.getBytesArray(s6xFlags.AddressInt, s6xFlags.Number);
                addStructure(s6xFlags, ref Calibration, ref arrBytes);
                alAddedOtherElementsUniqueAddressesToRead.Add(s6xFlags.UniqueAddress);
                s6xFlags = null;
                arrBytes = null;
            }

            // Now Linking Codes with Registers
            if (rtClearMalf != null && rtMalfunction != null)
            {
                SortedList slClearMalfCallsOperations = new SortedList();
                if (rtClearMalf != null)
                {
                    Call cCMCall = (Call)Calibration.slCalls[rtClearMalf.UniqueAddress];
                    if (cCMCall != null)
                    {
                        foreach (Caller cCaller in cCMCall.Callers)
                        {
                            if (cCaller == null) continue;
                            if (cCaller.CallType != CallType.Call && cCaller.CallType != CallType.ShortCall) continue;
                            SADBank callerBank = null;
                            switch (cCaller.BankNum)
                            {
                                case 8:
                                    callerBank = Bank8;
                                    break;
                                case 1:
                                    callerBank = Bank1;
                                    break;
                                case 9:
                                    callerBank = Bank9;
                                    break;
                                case 0:
                                    callerBank = Bank0;
                                    break;
                            }
                            Operation callerOpe = (Operation)callerBank.slOPs[cCaller.UniqueAddress];
                            if (callerOpe == null) continue;
                            if (callerOpe.CallArguments == null) continue;
                            if (callerOpe.CallArguments.Length != 1) continue;
                            Register recReg = (Register)Calibration.slRegisters[Tools.RegisterUniqueAddress(callerOpe.CallArguments[0].DecryptedValueInt)];
                            if (recReg == null) continue;
                            if (recReg.S6xRegister == null) continue;
                            if (!slRegistersUAddrOBDCodes.ContainsKey(recReg.UniqueAddress)) continue;
                            string obdCode = (string)slRegistersUAddrOBDCodes[recReg.UniqueAddress];
                            if (slClearMalfCallsOperations.ContainsKey(obdCode))
                            {
                                //((ArrayList)slClearMalfCallsOperations[obdCode]).Add(callerBank.getPrecedingOPs(callerOpe.AddressInt, 16, 0, true, false, false, false, false, false, false, false));
                                ((ArrayList)slClearMalfCallsOperations[obdCode]).Add(callerBank.getPrecedingOPs(callerOpe.AddressInt, 16, 0));
                            }
                            else
                            {
                                //slClearMalfCallsOperations.Add(obdCode, new ArrayList() { callerBank.getPrecedingOPs(callerOpe.AddressInt, 16, 0, true, false, false, false, false, false, false, false) });
                                slClearMalfCallsOperations.Add(obdCode, new ArrayList() { callerBank.getPrecedingOPs(callerOpe.AddressInt, 16, 0) });
                            }
                        }
                        cCMCall = null;
                    }
                }

                // slClearMalfCallsOperations contains ArrayList with Precdeding Ops arrays, key is obdCode
                foreach (string obdCode in slClearMalfCallsOperations.Keys)
                {
                    ArrayList alCOPSArrOperations = (ArrayList)slClearMalfCallsOperations[obdCode];
                    if (alCOPSArrOperations == null) continue;
                    if (alCOPSArrOperations.Count == 0) continue;
                    Operation[] arrCOPSPrecOps = null;
                    S6xRegister s6xCMReg = null;
                    switch (obdCode)
                    {
                        // P0112, P0113 - ACT
                        case "0112":
                            if (alCOPSArrOperations.Count > 1) continue;   // No idea how to manage 2 iterations
                            arrCOPSPrecOps = (Operation[])alCOPSArrOperations[0];
                            foreach (Operation ope in arrCOPSPrecOps)
                            {
                                if (ope == null) break;
                                if (ope.isReturn) break;
                                if (ope.OperationParams.Length == 0) continue;

                                object[] arrPointersValues = null;

                                switch (ope.OriginalInstruction.ToLower())
                                {
                                    case "stb":
                                    case "stw":
                                        arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].CalculatedParam);
                                        if (!(bool)arrPointersValues[0]) continue;
                                        s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate((ope.OriginalInstruction.ToLower() == "stb") ? SADFixedRegisters.FixedRegisters.ACT : SADFixedRegisters.FixedRegisters.ACT_WORD);
                                        s6xCMReg.AddressInt = (int)arrPointersValues[2];
                                        if (arrPointersValues.Length > 3) s6xCMReg.AdditionalAddress10 = ((int)arrPointersValues[4]).ToString();
                                        s6xCMReg.Store = true;
                                        addRegister(s6xCMReg, ref Calibration, ref S6x);
                                        break;
                                }

                                if (s6xCMReg != null) break;
                            }
                            break;
                        // P0231, P0232 - FPUMP_DC and VBAT, because it is linked
                        case "0232":
                            if (alCOPSArrOperations.Count != 2) continue;
                            if (alCOPSArrOperations.Count > 2) continue;   // No idea how to manage 3 iterations
                            if (((Operation[])alCOPSArrOperations[0])[0].AddressInt > ((Operation[])alCOPSArrOperations[1])[0].AddressInt) arrCOPSPrecOps = (Operation[])alCOPSArrOperations[1];
                            else arrCOPSPrecOps = (Operation[])alCOPSArrOperations[0];
                            arrCOPSPrecOps = (Operation[])alCOPSArrOperations[1];
                            bool found232sb3w = false;
                            foreach (Operation ope in arrCOPSPrecOps)
                            {
                                if (ope == null) break;
                                if (ope.isReturn) break;
                                if (ope.OperationParams.Length == 0) continue;

                                object[] arrPointersValues = null;

                                switch (ope.OriginalInstruction.ToLower())
                                {
                                    case "sb3w":
                                        found232sb3w = true;
                                        break;
                                    case "ldw":
                                        if (found232sb3w)
                                        {
                                            arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].CalculatedParam);
                                            if (!(bool)arrPointersValues[0]) continue;
                                            s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.FPUMP_DC);
                                            s6xCMReg.AddressInt = (int)arrPointersValues[2];
                                            if (arrPointersValues.Length > 3) s6xCMReg.AdditionalAddress10  = ((int)arrPointersValues[4]).ToString();
                                            s6xCMReg.Store = true;
                                            addRegister(s6xCMReg, ref Calibration, ref S6x);
                                            s6xCMReg = null;
                                        }
                                        break;
                                    case "ldb":
                                        if (found232sb3w)
                                        {
                                            arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].CalculatedParam);
                                            if (!(bool)arrPointersValues[0]) continue;
                                            s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.VBAT);
                                            s6xCMReg.AddressInt = (int)arrPointersValues[2];
                                            if (arrPointersValues.Length > 3) s6xCMReg.AdditionalAddress10  = ((int)arrPointersValues[4]).ToString();
                                            s6xCMReg.Store = true;
                                            addRegister(s6xCMReg, ref Calibration, ref S6x);
                                        }
                                        break;
                                }

                                if (s6xCMReg != null) break;
                            }
                            break;
                        // P0552, P0553 - Power steering => Can Give pstmr, pspres, pspt_eng
                        case "0553":
                            // TO BE DONE
                            break;
                        // P1290 - CHT and related registers
                        case "1290":
                            if (alCOPSArrOperations.Count > 1) continue;   // No idea how to manage 2 iterations
                            arrCOPSPrecOps = (Operation[])alCOPSArrOperations[0];
                            bool foundCall = false;
                            bool foundClip = false;
                            bool foundCHT_ER_TMR = false;
                            bool bBreak = false;
                            ArrayList alCHTsArrPointersValues = new ArrayList();
                            foreach (Operation ope in arrCOPSPrecOps)
                            {
                                if (ope == null) break;
                                if (ope.isReturn) break;
                                if (ope.OperationParams.Length == 0) continue;

                                object[] arrPointersValues = null;

                                if (ope.CallType == CallType.Call || ope.CallType == CallType.ShortCall)
                                {
                                    if (ope.CallArguments != null)
                                    {
                                        // Initial call, not the right one
                                        if (ope.CallArguments.Length > 0) continue;
                                    }

                                    if (!foundCall)
                                    {
                                        // This is the interesting one.
                                        foundCall = true;

                                        int iCHTsArrPointersValues = 0;
                                        foreach (object[] arrPV in alCHTsArrPointersValues)
                                        {
                                            if (alCHTsArrPointersValues.Count == 3)
                                            {
                                                if (iCHTsArrPointersValues == 2) s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.CHT_LONG);
                                                else if (iCHTsArrPointersValues == 1) s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.CHT);
                                                else if (iCHTsArrPointersValues == 0) s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.CHT_RES);
                                            }
                                            else if (alCHTsArrPointersValues.Count == 2)
                                            {
                                                if (iCHTsArrPointersValues == 1) s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.CHT_LONG);
                                                else if (iCHTsArrPointersValues == 0) s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.CHT);
                                            }
                                            else if (alCHTsArrPointersValues.Count == 1)
                                            {
                                                s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.CHT);
                                            }
                                            s6xCMReg.AddressInt = (int)arrPV[2];
                                            if (arrPV.Length > 3) s6xCMReg.AdditionalAddress10 = ((int)arrPV[4]).ToString();
                                            s6xCMReg.Store = true;
                                            addRegister(s6xCMReg, ref Calibration, ref S6x);
                                            s6xCMReg = null;
                                            iCHTsArrPointersValues++;
                                        }

                                        if (alCHTsArrPointersValues.Count < 2) break;
                                    }
                                    else
                                    {
                                        // This is CHT_CLIPW
                                        foundClip = true;
                                        Call cChtClipW = (Call)Calibration.slCalls[Tools.UniqueAddress(ope.ApplyOnBankNum, ope.AddressJumpInt)];
                                        cChtClipW.ShortLabel = "CHT_CLIPW";
                                        cChtClipW.Label = "CHT Clip W";
                                        cChtClipW = null;
                                    }
                                }

                                switch (ope.OriginalInstruction.ToLower())
                                {
                                    case "stb":
                                        if (!foundCall && !foundCHT_ER_TMR)
                                        {
                                            arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].CalculatedParam);
                                            if (!(bool)arrPointersValues[0]) continue;
                                            s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.CHT_ER_TMR);
                                            s6xCMReg.AddressInt = (int)arrPointersValues[2];
                                            if (arrPointersValues.Length > 3) s6xCMReg.AdditionalAddress10 = ((int)arrPointersValues[4]).ToString();
                                            s6xCMReg.Store = true;
                                            addRegister(s6xCMReg, ref Calibration, ref S6x);
                                            s6xCMReg = null;
                                            foundCHT_ER_TMR = true;
                                        }
                                        break;
                                    case "stw":
                                        if (!foundCall)
                                        {
                                            arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].CalculatedParam);
                                            if (!(bool)arrPointersValues[0]) continue;
                                            alCHTsArrPointersValues.Add(arrPointersValues);
                                        }
                                        break;
                                    case "ldw":
                                        if (foundClip)
                                        {
                                            arrPointersValues = Tools.InstructionPointersValues(ope.OperationParams[0].CalculatedParam);
                                            if (!(bool)arrPointersValues[0]) continue;
                                            s6xCMReg = SADFixedRegisters.GetFixedRegisterTemplate(SADFixedRegisters.FixedRegisters.CHT_ENG);
                                            s6xCMReg.AddressInt = (int)arrPointersValues[2];
                                            if (arrPointersValues.Length > 3) s6xCMReg.AdditionalAddress10 = ((int)arrPointersValues[4]).ToString();
                                            s6xCMReg.Store = true;
                                            addRegister(s6xCMReg, ref Calibration, ref S6x);
                                            s6xCMReg = null;
                                            bBreak = true;
                                        }
                                        break;
                                }

                                if (bBreak) break;
                            }
                            break;
                    }
                }

                string[] managedCodes = new string[slClearMalfCallsOperations.Count];
                int iIndex = 0;
                foreach (string obdCode in slClearMalfCallsOperations.Keys)
                {
                    managedCodes[iIndex] = obdCode;
                    iIndex++;
                }
                string sManagedCodes = string.Join(", ", managedCodes);
                slClearMalfCallsOperations = null;
            }

            slRegistersUAddrOBDCodes = null;
        }

        // addElement to automatize element creation / update or mngt
        private static bool addFunction(S6xFunction s6xFunctionTemplate, ref SADCalib Calibration)
        {
            Function fFunction = null;
            if (Calibration.BankNum == s6xFunctionTemplate.BankNum && Calibration.isCalibrationAddress(s6xFunctionTemplate.AddressInt))
            {
                CalibrationElement calFunction = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(s6xFunctionTemplate.BankNum, s6xFunctionTemplate.AddressInt)];
                if (calFunction != null)
                {
                    if (calFunction.isFunction)
                    {
                        if (calFunction.FunctionElem.S6xFunction != null) return false;
                    }
                    else
                    {
                        if (calFunction.isStructure)
                        {
                            if (calFunction.StructureElem.S6xStructure != null) return false;
                        }
                        else if (calFunction.isTable)
                        {
                            if (calFunction.TableElem.S6xTable != null) return false;
                        }
                        else if (calFunction.isScalar)
                        {
                            if (calFunction.ScalarElem.S6xScalar != null) return false;
                        }

                        // Conflict Mngt
                        calFunction.ScalarElem = null;
                        calFunction.FunctionElem = null;
                        calFunction.StructureElem = null;
                        calFunction.TableElem = null;

                        calFunction.FunctionElem = new Function();
                        calFunction.FunctionElem.BankNum = calFunction.BankNum;
                        calFunction.FunctionElem.AddressInt = calFunction.AddressInt;
                        calFunction.FunctionElem.RBase = calFunction.RBase;
                        calFunction.FunctionElem.RBaseCalc = calFunction.RBaseCalc;
                    }
                }
                if (calFunction == null)
                {
                    calFunction = new CalibrationElement(s6xFunctionTemplate.BankNum, s6xFunctionTemplate.Address);
                    calFunction.AddressInt = s6xFunctionTemplate.AddressInt;
                    calFunction.RBase = Calibration.getRbaseByAddress(calFunction.AddressInt).Code;
                    calFunction.FunctionElem = new Function();
                    calFunction.FunctionElem.BankNum = calFunction.BankNum;
                    calFunction.FunctionElem.AddressInt = calFunction.AddressInt;
                    calFunction.FunctionElem.RBase = calFunction.RBase;
                    calFunction.FunctionElem.RBaseCalc = calFunction.RBaseCalc;
                    Calibration.slCalibrationElements.Add(calFunction.UniqueAddress, calFunction);
                }
                fFunction = calFunction.FunctionElem;
                calFunction = null;
            }
            else
            {
                fFunction = (Function)Calibration.slExtFunctions[Tools.UniqueAddress(s6xFunctionTemplate.BankNum, s6xFunctionTemplate.AddressInt)];
                if (fFunction != null)
                {
                    if (fFunction.S6xFunction != null) return false;
                }
                if (fFunction == null)
                {
                    if (Calibration.slExtStructures.ContainsKey(Tools.UniqueAddress(s6xFunctionTemplate.BankNum, s6xFunctionTemplate.AddressInt))) return false;
                    if (Calibration.slExtTables.ContainsKey(Tools.UniqueAddress(s6xFunctionTemplate.BankNum, s6xFunctionTemplate.AddressInt))) return false;
                    if (Calibration.slExtScalars.ContainsKey(Tools.UniqueAddress(s6xFunctionTemplate.BankNum, s6xFunctionTemplate.AddressInt))) return false;

                    fFunction = new Function();
                    fFunction.BankNum = s6xFunctionTemplate.BankNum;
                    fFunction.AddressInt = s6xFunctionTemplate.AddressInt;
                    fFunction.RBaseCalc = fFunction.Address;
                    Calibration.slExtFunctions.Add(fFunction.UniqueAddress, fFunction);
                }
            }
            if (fFunction.S6xFunction == null)
            {
                fFunction.ShortLabel = s6xFunctionTemplate.ShortLabel;
                fFunction.Label = s6xFunctionTemplate.Label;
                fFunction.InputScaleExpression = s6xFunctionTemplate.InputScaleExpression;
                fFunction.InputScalePrecision = s6xFunctionTemplate.InputScalePrecision;
                fFunction.InputUnits = s6xFunctionTemplate.InputUnits;
                fFunction.OutputScaleExpression = s6xFunctionTemplate.OutputScaleExpression;
                fFunction.OutputScalePrecision = s6xFunctionTemplate.OutputScalePrecision;
                fFunction.OutputUnits = s6xFunctionTemplate.OutputUnits;
                fFunction.Comments = s6xFunctionTemplate.Comments;
                // No S6x Creation to permit automatic process to manage non identified parts
            }
            fFunction = null;

            return true;
        }

        // addElement to automatize element creation / update or mngt
        private static bool addStructure(S6xStructure s6xStructTemplate, ref SADCalib Calibration, ref string[] readBytes)
        {
            Structure sStruct = null;
            if (Calibration.BankNum == s6xStructTemplate.BankNum && Calibration.isCalibrationAddress(s6xStructTemplate.AddressInt))
            {
                CalibrationElement calStruct = (CalibrationElement)Calibration.slCalibrationElements[s6xStructTemplate.UniqueAddress];
                if (calStruct != null)
                {
                    if (calStruct.isStructure)
                    {
                        if (calStruct.StructureElem.S6xStructure != null)
                        {
                            if (calStruct.StructureElem.S6xStructure.Number != s6xStructTemplate.Number)
                            {
                                calStruct = null;
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if (calStruct.isScalar)
                        {
                            if (calStruct.ScalarElem.S6xScalar != null) return false;
                        }
                        else if (calStruct.isTable)
                        {
                            if (calStruct.TableElem.S6xTable != null) return false;
                        }
                        else if (calStruct.isFunction)
                        {
                            if (calStruct.FunctionElem.S6xFunction != null) return false;
                        }

                        // Conflict Mngt
                        calStruct.ScalarElem = null;
                        calStruct.FunctionElem = null;
                        calStruct.StructureElem = null;
                        calStruct.TableElem = null;

                        calStruct.StructureElem = new Structure();
                    }
                }
                if (calStruct == null)
                {
                    calStruct = new CalibrationElement(s6xStructTemplate.BankNum, s6xStructTemplate.Address);
                    calStruct.AddressInt = s6xStructTemplate.AddressInt;
                    calStruct.RBase = Calibration.getRbaseByAddress(calStruct.AddressInt).Code;
                    calStruct.StructureElem = new Structure();
                    Calibration.slCalibrationElements.Add(calStruct.UniqueAddress, calStruct);
                }
                sStruct = calStruct.StructureElem;
                sStruct.BankNum = calStruct.BankNum;
                sStruct.AddressInt = calStruct.AddressInt;
                sStruct.RBase = calStruct.RBase;
                sStruct.RBaseCalc = calStruct.RBaseCalc;
                sStruct.Defaulted = true;
                calStruct = null;
            }
            else
            {
                sStruct = (Structure)Calibration.slExtStructures[s6xStructTemplate.UniqueAddress];
                if (sStruct == null)
                {
                    sStruct = new Structure();
                    sStruct.BankNum = s6xStructTemplate.BankNum;
                    sStruct.AddressInt = s6xStructTemplate.AddressInt;
                    sStruct.RBaseCalc = s6xStructTemplate.Address;
                    sStruct.Defaulted = true;
                    Calibration.slExtStructures.Add(sStruct.UniqueAddress, sStruct);
                }
            }
            if (sStruct.S6xStructure == null)
            {
                sStruct.ShortLabel = s6xStructTemplate.ShortLabel;
                sStruct.Label = s6xStructTemplate.Label;
                sStruct.StructDefString = s6xStructTemplate.StructDef;
                sStruct.Number = s6xStructTemplate.Number;
                sStruct.S6xStructure = new S6xStructure(sStruct);
                sStruct.S6xStructure.Comments = s6xStructTemplate.Comments;
            }

            // 20240523 - PYM - Overwritting is not necessary and can generate issues
            //if (readBytes != null) sStruct.Read(ref readBytes, sStruct.Number);
            if (sStruct.Defaulted && readBytes != null) sStruct.Read(ref readBytes, sStruct.Number);

            sStruct = null;

            return true;
        }

        // addElement to automatize element creation / update or mngt
        private static bool addScalar(S6xScalar s6xScalarTemplate, ref SADCalib Calibration)
        {
            Scalar sScalar = null;
            if (Calibration.BankNum == s6xScalarTemplate.BankNum && Calibration.isCalibrationAddress(s6xScalarTemplate.AddressInt))
            {
                CalibrationElement calScalar = (CalibrationElement)Calibration.slCalibrationElements[Tools.UniqueAddress(s6xScalarTemplate.BankNum, s6xScalarTemplate.AddressInt)];
                if (calScalar != null)
                {
                    if (calScalar.isScalar)
                    {
                        if (calScalar.ScalarElem.S6xScalar != null) return false;
                    }
                    else
                    {
                        if (calScalar.isStructure)
                        {
                            if (calScalar.StructureElem.S6xStructure != null) return false;
                        }
                        else if (calScalar.isTable)
                        {
                            if (calScalar.TableElem.S6xTable != null) return false;
                        }
                        else if (calScalar.isFunction)
                        {
                            if (calScalar.FunctionElem.S6xFunction != null) return false;
                        }

                        // Conflict Mngt
                        calScalar.ScalarElem = null;
                        calScalar.FunctionElem = null;
                        calScalar.StructureElem = null;
                        calScalar.TableElem = null;

                        calScalar.ScalarElem = new Scalar(calScalar.BankNum, calScalar.AddressInt, true, false);
                        calScalar.ScalarElem.RBase = calScalar.RBase;
                        calScalar.ScalarElem.RBaseCalc = calScalar.RBaseCalc;
                    }
                }
                if (calScalar == null)
                {
                    calScalar = new CalibrationElement(s6xScalarTemplate.BankNum, s6xScalarTemplate.Address);
                    calScalar.AddressInt = s6xScalarTemplate.AddressInt;
                    calScalar.RBase = Calibration.getRbaseByAddress(calScalar.AddressInt).Code;
                    calScalar.ScalarElem = new Scalar(calScalar.BankNum, calScalar.AddressInt, true, false);
                    calScalar.ScalarElem.RBase = calScalar.RBase;
                    calScalar.ScalarElem.RBaseCalc = calScalar.RBaseCalc;
                    Calibration.slCalibrationElements.Add(calScalar.UniqueAddress, calScalar);
                }
                sScalar = calScalar.ScalarElem;
                calScalar = null;
            }
            else
            {
                sScalar = (Scalar)Calibration.slExtScalars[Tools.UniqueAddress(s6xScalarTemplate.BankNum, s6xScalarTemplate.AddressInt)];
                if (sScalar != null)
                {
                    if (sScalar.S6xScalar != null) return false;
                }
                if (sScalar == null)
                {
                    if (Calibration.slExtStructures.ContainsKey(Tools.UniqueAddress(s6xScalarTemplate.BankNum, s6xScalarTemplate.AddressInt))) return false;
                    if (Calibration.slExtTables.ContainsKey(Tools.UniqueAddress(s6xScalarTemplate.BankNum, s6xScalarTemplate.AddressInt))) return false;
                    if (Calibration.slExtFunctions.ContainsKey(Tools.UniqueAddress(s6xScalarTemplate.BankNum, s6xScalarTemplate.AddressInt))) return false;

                    sScalar = new Scalar(s6xScalarTemplate.BankNum, s6xScalarTemplate.AddressInt, true, false);
                    sScalar.RBaseCalc = sScalar.Address;
                    Calibration.slExtScalars.Add(sScalar.UniqueAddress, sScalar);
                }
            }
            if (sScalar.S6xScalar == null)
            {
                sScalar.ShortLabel = s6xScalarTemplate.ShortLabel;
                sScalar.Label = s6xScalarTemplate.Label;
                sScalar.Comments = s6xScalarTemplate.Comments;
                // No S6x Creation to permit automatic process to manage non identified parts
            }
            sScalar = null;

            return true;
        }

        // addElement to automatize element creation / update or mngt
        //      S6xRegister is added when not existing (overriden when existing but not in processes, when it is skipped)
        //      20200908 - BitFlag is added when its position is not defined
        private static bool addRegister(S6xRegister s6xRegTemplate, ref SADCalib Calibration, ref SADS6x S6x)
        {
            S6xRegister s6xReg = (S6xRegister)S6x.slProcessRegisters[s6xRegTemplate.UniqueAddress];
            if (s6xReg == null)
            {
                s6xReg = s6xRegTemplate.Clone();
                S6x.slProcessRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                if (S6x.slRegisters.ContainsKey(s6xReg.UniqueAddress)) S6x.slRegisters[s6xReg.UniqueAddress] = s6xReg;
                else S6x.slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
            }
            else
            {
                // 20200908 - BitFlags Mngt
                if (s6xRegTemplate.isBitFlags)
                {
                    foreach (S6xBitFlag s6xBFT in s6xRegTemplate.BitFlags)
                    {
                        if (!s6xReg.isBitFlags)
                        {
                            s6xReg.AddBitFlag(s6xBFT);
                            continue;
                        }
                        bool existingBF = false;
                        foreach (S6xBitFlag s6xBF in s6xReg.BitFlags)
                        {
                            if (s6xBF.Position == s6xBFT.Position)
                            {
                                existingBF = true;
                                break;
                            }
                        }
                        if (!existingBF) s6xReg.AddBitFlag(s6xBFT);
                    }
                }
                else
                {
                    s6xReg = null;
                    return false;
                }
            }

            if (s6xReg != null)
            {
                Register rReg = (Register)Calibration.slRegisters[s6xReg.UniqueAddress];
                if (rReg != null) if (rReg.S6xRegister == null) rReg.S6xRegister = s6xReg;
            }

            return true;
        }

        private static void readAddedOtherElements(ref ArrayList alAddedOtherElementsToRead, ref SADCalib Calibration, ref SADBank Bank0, ref SADBank Bank1, ref SADBank Bank8, ref SADBank Bank9)
        {
            foreach (string uniqueAddress in alAddedOtherElementsToRead)
            {
                SADBank bBank = null;
                string[] arrElemBytes = null;

                if (Calibration.slExtTables.ContainsKey(uniqueAddress))
                {
                    Table extTable = (Table)Calibration.slExtTables[uniqueAddress];
                    if (extTable.S6xTable == null) continue;
                    if (extTable.S6xTable.ColsNumber < 1) continue;
                    if (extTable.S6xTable.RowsNumber < 1) continue;
                    
                    switch (extTable.BankNum)
                    {
                        case 8:
                            bBank = Bank8;
                            break;
                        case 1:
                            bBank = Bank1;
                            break;
                        case 9:
                            bBank = Bank9;
                            break;
                        case 0:
                            bBank = Bank0;
                            break;
                    }

                    // Manages non calculated AddressBinInt
                    if (extTable.AddressBinInt == -1) extTable.AddressBinInt = extTable.AddressInt + bBank.AddressBinInt;

                    arrElemBytes = bBank.getBytesArray(extTable.AddressInt, extTable.S6xTable.ColsNumber * extTable.S6xTable.RowsNumber);
                    extTable.Read(arrElemBytes);

                    continue;
                }
                if (Calibration.slExtFunctions.ContainsKey(uniqueAddress))
                {
                    Function extFunction = (Function)Calibration.slExtFunctions[uniqueAddress];
                    if (extFunction.S6xFunction == null) continue;
                    if (extFunction.S6xFunction.RowsNumber < 1) continue;

                    switch (extFunction.BankNum)
                    {
                        case 8:
                            bBank = Bank8;
                            break;
                        case 1:
                            bBank = Bank1;
                            break;
                        case 9:
                            bBank = Bank9;
                            break;
                        case 0:
                            bBank = Bank0;
                            break;
                    }

                    // Manages non calculated AddressBinInt
                    if (extFunction.AddressBinInt == -1) extFunction.AddressBinInt = extFunction.AddressInt + bBank.AddressBinInt;

                    arrElemBytes = bBank.getBytesArray(extFunction.AddressInt, extFunction.SizeLine * extFunction.S6xFunction.RowsNumber);
                    extFunction.Read(arrElemBytes);

                    continue;
                }
                if (Calibration.slExtScalars.ContainsKey(uniqueAddress))
                {
                    Scalar extScalar = (Scalar)Calibration.slExtScalars[uniqueAddress];

                    switch (extScalar.BankNum)
                    {
                        case 8:
                            bBank = Bank8;
                            break;
                        case 1:
                            bBank = Bank1;
                            break;
                        case 9:
                            bBank = Bank9;
                            break;
                        case 0:
                            bBank = Bank0;
                            break;
                    }

                    // Manages non calculated AddressBinInt
                    if (extScalar.AddressBinInt == -1) extScalar.AddressBinInt = extScalar.AddressInt + bBank.AddressBinInt;

                    arrElemBytes = bBank.getBytesArray(extScalar.AddressInt, extScalar.Size);
                    extScalar.Read(arrElemBytes);

                    continue;
                }
                if (Calibration.slExtStructures.ContainsKey(uniqueAddress))
                {
                    Structure extStructure = (Structure)Calibration.slExtStructures[uniqueAddress];
                    if (extStructure.S6xStructure == null) continue;
                    if (extStructure.S6xStructure.Number < 1) continue;

                    switch (extStructure.BankNum)
                    {
                        case 8:
                            bBank = Bank8;
                            break;
                        case 1:
                            bBank = Bank1;
                            break;
                        case 9:
                            bBank = Bank9;
                            break;
                        case 0:
                            bBank = Bank0;
                            break;
                    }

                    // Manages non calculated AddressBinInt
                    if (extStructure.AddressBinInt == -1) extStructure.AddressBinInt = extStructure.AddressInt + bBank.AddressBinInt;

                    // No Read process here

                    continue;
                }
            }
        }
    }
}
