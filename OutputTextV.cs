using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace SAD806x
{
    // OuputTextV - Variable Settings Based Version
    public class OutputTextV
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
        private bool is8065SingleBank = false;

        private string binaryFileName = string.Empty;

        private List<string> lReplacementCoreBaseStrings = null;
        private SortedList<string, string> slReplacementsCore = null;
        private string sReplacementCoreStartString = SADDef.ReplacementCoreStartString;
        private string sReplacementCoreStartStringBis = SADDef.ReplacementCoreStartStringBis;
        private string sReplacementCoreBitFlagString = SADDef.ReplacementCoreBitFlagString;
        
        // Generic Settings
        private int lineAddressMinWidth = 6;

        private int lineCenterOpsMinWidth = 46;
        private int lineRightOpsMinWidth = 100;

        private int lineCenterOpsPart1MinWidth = 21;
        private int lineCenterOpsPart2MinWidth = 6;
        private int lineCenterOpsPart3MinWidth { get { return lineCenterOpsMinWidth - lineCenterOpsPart1MinWidth - lineCenterOpsPart2MinWidth; } }

        private int lineRightOpsPart1MinWidth = 21;
        private int lineRightOpsPart2MinWidth { get { return lineRightOpsMinWidth - lineRightOpsPart1MinWidth; } }

        private int lineCenterCalMinWidth = 52;
        private int lineRightCalMinWidth = 83;

        private int lineCenterCalPart1MinWidth = 18;
        private int lineCenterCalPart2MinWidth = 20;
        private int lineCenterCalPart3MinWidth = 8;
        private int lineCenterCalPart4MinWidth { get { return lineCenterCalMinWidth - lineCenterCalPart1MinWidth - lineCenterCalPart2MinWidth - lineCenterCalPart3MinWidth; } }

        private int lineRightCalPart1MinWidth = 20;
        private int lineRightCalPart2MinWidth = 10;
        private int lineRightCalPart3MinWidth { get { return lineRightCalMinWidth - lineRightCalPart1MinWidth - lineRightCalPart2MinWidth; } }

        private int tableSeparator1MinWidth = 7;
        private int tableSeparator2MinWidth = 7;
        private int tableSeparator3MinWidth = 8;
        private int tableByteHexMinWidth = 3;
        private int tableWordHexMinWidth = 5;
        private int tableByteDecUnscaledMinWidth = 4;
        private int tableWordDecUnscaledMinWidth = 6;
        private int tableByteDecScaledMinWidth = 8;
        private int tableWordDecScaledMinWidth = 10;

        private string lineAddressBaseFollower = ": ";
        private string lineAddressFillFollower = " -> ";

        private string typeUnknownOperationLine = "Unknown Operation/Structure";
        private string typeUnknownOperationFillLine = "fill";
        private string typeUnknownCalibrationLine = "Unknown Calibration";
        private string typeUnknownCalibrationFillLine = "fill";

        private string typeCalElemScalarByte = "byte";
        private string typeCalElemScalarWord = "word";
        private string typeExtScalarByte = "obyte";
        private string typeExtScalarWord = "oword";
        private string typeCalElemFunction = "func";
        private string typeExtFunction = "ofunc";
        private string typeCalElemTable = "table";
        private string typeExtTable = "otable";
        private string typeCalElemStruct = "struct";
        private string typeExtStruct = "ostruct";
        private string typeVector = "Vector";

        private string commentsPrefix = "//";
        private string commentsSuffixHeader = "\\\\";
        private int commentsPrefixMargin = 2;
        private int commentsSuffixMarginHeader = 2;
        private int commentsHeaderMinWidth = 100;

        public int GLOBAL_LINE_ADDRESS_WIDTH { get { return lineAddressMinWidth; } set { lineAddressMinWidth = value; } }
        public int GLOBAL_LINE_OPS_CENTER_WIDTH
        {
            get { return lineCenterOpsMinWidth; }
            set
            {
                lineCenterOpsMinWidth = value;
                if (lineCenterOpsPart3MinWidth < 1) lineCenterOpsMinWidth = lineCenterOpsPart1MinWidth + lineCenterOpsPart2MinWidth + 1;
            }
        }
        public int GLOBAL_LINE_OPS_CENTER_PART1_WIDTH { get { return lineCenterOpsPart1MinWidth; } set { lineCenterOpsPart1MinWidth = value; } }
        public int GLOBAL_LINE_OPS_CENTER_PART2_WIDTH { get { return lineCenterOpsPart2MinWidth; } set { lineCenterOpsPart2MinWidth = value; } }
        public int GLOBAL_LINE_OPS_RIGHT_WIDTH
        {
            get { return lineRightOpsMinWidth; }
            set
            {
                lineRightOpsMinWidth = value;
                if (lineRightOpsPart2MinWidth < 1) lineRightOpsMinWidth = lineRightOpsPart1MinWidth + 1;
            }
        }
        public int GLOBAL_LINE_OPS_RIGHT_PART1_WIDTH { get { return lineRightOpsPart1MinWidth; } set { lineRightOpsPart1MinWidth = value; } }

        public int GLOBAL_LINE_CAL_CENTER_WIDTH
        {
            get { return lineCenterCalMinWidth; }
            set
            {
                lineCenterCalMinWidth = value;
                if (lineCenterCalPart4MinWidth < 1) lineCenterCalMinWidth = lineCenterCalPart1MinWidth + lineCenterCalPart2MinWidth + lineCenterCalPart3MinWidth + 1;
            }
        }
        public int GLOBAL_LINE_CAL_CENTER_PART1_WIDTH { get { return lineCenterCalPart1MinWidth; } set { lineCenterCalPart1MinWidth = value; } }
        public int GLOBAL_LINE_CAL_CENTER_PART2_WIDTH { get { return lineCenterCalPart2MinWidth; } set { lineCenterCalPart2MinWidth = value; } }
        public int GLOBAL_LINE_CAL_CENTER_PART3_WIDTH { get { return lineCenterCalPart3MinWidth; } set { lineCenterCalPart3MinWidth = value; } }
        public int GLOBAL_LINE_CAL_RIGHT_WIDTH
        {
            get { return lineRightCalMinWidth; }
            set
            {
                lineRightCalMinWidth = value;
                if (lineRightCalPart3MinWidth < 1) lineRightCalMinWidth = lineRightCalPart1MinWidth + lineRightCalPart2MinWidth + 1;
            }
        }
        public int GLOBAL_LINE_CAL_RIGHT_PART1_WIDTH { get { return lineRightCalPart1MinWidth; } set { lineRightCalPart1MinWidth = value; } }
        public int GLOBAL_LINE_CAL_RIGHT_PART2_WIDTH { get { return lineRightCalPart2MinWidth; } set { lineRightCalPart2MinWidth = value; } }

        public int TABLE_SEPARATOR1_WIDTH { get { return tableSeparator1MinWidth; } set { tableSeparator1MinWidth = value; } }
        public int TABLE_SEPARATOR2_WIDTH { get { return tableSeparator2MinWidth; } set { tableSeparator2MinWidth = value; } }
        public int TABLE_SEPARATOR3_WIDTH { get { return tableSeparator3MinWidth; } set { tableSeparator3MinWidth = value; } }
        public int TABLE_BYTE_HEX_WIDTH { get { return tableByteHexMinWidth; } set { tableByteHexMinWidth = value; } }
        public int TABLE_WORD_HEX_WIDTH { get { return tableWordHexMinWidth; } set { tableWordHexMinWidth = value; } }
        public int TABLE_BYTE_DEC_UNSCALED_WIDTH { get { return tableByteDecUnscaledMinWidth; } set { tableByteDecUnscaledMinWidth = value; } }
        public int TABLE_WORD_DEC_UNSCALED_WIDTH { get { return tableWordDecUnscaledMinWidth; } set { tableWordDecUnscaledMinWidth = value; } }
        public int TABLE_BYTE_DEC_SCALED_WIDTH { get { return tableByteDecScaledMinWidth; } set { tableByteDecScaledMinWidth = value; } }
        public int TABLE_WORD_DEC_SCALED_WIDTH { get { return tableWordDecScaledMinWidth; } set { tableWordDecScaledMinWidth = value; } }

        public string GLOBAL_LINE_BASE_FOLLOWER { get { return lineAddressBaseFollower; } set { lineAddressBaseFollower = value; } }
        public string GLOBAL_LINE_FILL_FOLLOWER { get { return lineAddressFillFollower; } set { lineAddressFillFollower = value; } }

        public string TYPE_UNKNOWNOPERATION { get { return typeUnknownOperationLine; } set { typeUnknownOperationLine = value; } }
        public string TYPE_UNKNOWNOPERATIONFILL { get { return typeUnknownOperationFillLine; } set { typeUnknownOperationFillLine = value; } }
        public string TYPE_UNKNOWNCALIBRATION { get { return typeUnknownCalibrationLine; } set { typeUnknownCalibrationLine = value; } }
        public string TYPE_UNKNOWNCALIBRATIONFILL { get { return typeUnknownCalibrationFillLine; } set { typeUnknownCalibrationFillLine = value; } }

        public string TYPE_CALELEM_SCALAR_BYTE { get { return typeCalElemScalarByte; } set { typeCalElemScalarByte = value; } }
        public string TYPE_CALELEM_SCALAR_WORD { get { return typeCalElemScalarWord; } set { typeCalElemScalarWord = value; } }
        public string TYPE_EXT_SCALAR_BYTE { get { return typeExtScalarByte; } set { typeExtScalarByte = value; } }
        public string TYPE_EXT_SCALAR_WORD { get { return typeExtScalarWord; } set { typeExtScalarWord = value; } }
        public string TYPE_CALELEM_FUNCTION { get { return typeCalElemFunction; } set { typeCalElemFunction = value; } }
        public string TYPE_EXT_FUNCTION { get { return typeExtFunction; } set { typeExtFunction = value; } }
        public string TYPE_CALELEM_TABLE { get { return typeCalElemTable; } set { typeCalElemTable = value; } }
        public string TYPE_EXT_TABLE { get { return typeExtTable; } set { typeExtTable = value; } }
        public string TYPE_CALELEM_STRUCT { get { return typeCalElemStruct; } set { typeCalElemStruct = value; } }
        public string TYPE_EXT_STRUCT { get { return typeExtStruct; } set { typeExtStruct = value; } }
        public string TYPE_VECTOR { get { return typeVector; } set { typeVector = value; } }

        public string COMMENTS_PREFIX { get { return commentsPrefix; } set { commentsPrefix = value; } }
        public string COMMENTS_SUFFIX_HEADER { get { return commentsSuffixHeader; } set { commentsSuffixHeader = value; } }
        public int COMMENTS_PREFIX_MARGIN { get { return commentsPrefixMargin; } set { commentsPrefixMargin = value; } }
        public int COMMENTS_SUFFIX_MARGIN_HEADER { get { return commentsSuffixMarginHeader; } set { commentsSuffixMarginHeader = value; } }
        public int COMMENTS_HEADER_MIN_WIDTH { get { return commentsHeaderMinWidth; } set { commentsHeaderMinWidth = value; } }

        public OutputTextV(ref SADBin sbSadBin, string textOutputFilePath, ref SettingsLst slToSettings)
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
            is8065SingleBank = sadBin.is8065SingleBank;

            // Generic Settings
            OutputSetGenericSetting();
        }

        private void OutputSetGenericSetting()
        {
            foreach (PropertyInfo pProp in this.GetType().GetProperties())
            {
                if (!pProp.CanWrite) continue;
                SettingItem sItem = toSettings.Get(pProp.Name);
                if (sItem == null) continue;
                try
                {
                    switch (sItem.Type)
                    {
                        case SettingType.Number:
                            pProp.SetValue(this, toSettings.Get<int>(pProp.Name, pProp.GetValue(this, null)), null);
                            break;
                        case SettingType.DecimalNumber:
                            pProp.SetValue(this, toSettings.Get<double>(pProp.Name, pProp.GetValue(this, null)), null);
                            break;
                        case SettingType.Boolean:
                            pProp.SetValue(this, toSettings.Get<bool>(pProp.Name, pProp.GetValue(this, null)), null);
                            break;
                        default:
                            pProp.SetValue(this, toSettings.Get<string>(pProp.Name, pProp.GetValue(this, null)), null);
                            break;
                    }
                }
                catch { }
            }
        }

        private void OutputReplacementsPrepare(ref SortedList slElements)
        {
            lReplacementCoreBaseStrings = new List<string>();
            lReplacementCoreBaseStrings.Add(sReplacementCoreStartString);
            lReplacementCoreBaseStrings.Add(sReplacementCoreStartStringBis);

            slReplacementsCore = new SortedList<string, string>();
            
            // Registers
            foreach (S6xRegister s6xReg in S6x.slRegisters.Values)
            {
                if (s6xReg.Skip || !s6xReg.Store) continue;

                if (s6xReg.Address == null || s6xReg.Address == string.Empty) continue;

                // BitFlags before for replacement reasons
                if (s6xReg.isBitFlags)
                {
                    if (s6xReg.BitFlags != null)
                    {
                        foreach (S6xBitFlag s6xBF in s6xReg.BitFlags)
                        {
                            if (s6xBF.Skip) continue;
                            if (s6xBF.ShortLabel == null || s6xBF.ShortLabel == string.Empty) continue;
                            if (!slReplacementsCore.ContainsKey(s6xReg.Address + sReplacementCoreBitFlagString + s6xBF.Position.ToString()))
                            {
                                slReplacementsCore.Add(s6xReg.Address + sReplacementCoreBitFlagString + s6xBF.Position.ToString(), s6xBF.ShortLabel);
                            }
                        }
                    }
                }
                
                if (s6xReg.Label == null || s6xReg.Label == string.Empty) continue;

                if (!slReplacementsCore.ContainsKey(s6xReg.Address))
                {
                    slReplacementsCore.Add(s6xReg.Address, s6xReg.Label);
                }
            }
            
            // Interesting Elements
            foreach (Element elem in slElements.Values)
            {
                string uniqueAddressHex = string.Empty;
                string calElemAddressHex = string.Empty;
                string shortLabel = string.Empty;
                S6xBitFlag[] s6xBitFlags = null;

                switch (elem.Type)
                {
                    case MergedType.ReservedAddress:
                        uniqueAddressHex = elem.ReservedAddress.UniqueAddressHex;
                        shortLabel = elem.ReservedAddress.ShortLabel;
                        break;
                    case MergedType.Operation:
                        if (Calibration.alMainCallsUniqueAddresses.Contains(elem.Operation.UniqueAddress))
                        {
                            uniqueAddressHex = elem.Operation.UniqueAddressHex;
                            shortLabel = ((Call)Calibration.slCalls[elem.Operation.UniqueAddress]).ShortLabel;
                        }
                        else if (S6x.slProcessRoutines.ContainsKey(elem.Operation.UniqueAddress))
                        {
                            uniqueAddressHex = elem.Operation.UniqueAddressHex;
                            shortLabel = ((S6xRoutine)S6x.slProcessRoutines[elem.Operation.UniqueAddress]).ShortLabel;
                        }
                        break;
                    case MergedType.CalibrationElement:
                        uniqueAddressHex = elem.CalElement.UniqueAddressHex;
                        calElemAddressHex = elem.CalElement.Address;
                        if (elem.CalElement.isScalar)
                        {
                            shortLabel = elem.CalElement.ScalarElem.ShortLabel;
                            if (elem.CalElement.ScalarElem.isBitFlags && elem.CalElement.ScalarElem.S6xScalar != null) s6xBitFlags = elem.CalElement.ScalarElem.S6xScalar.BitFlags;
                        }
                        else if (elem.CalElement.isFunction) shortLabel = elem.CalElement.FunctionElem.ShortLabel;
                        else if (elem.CalElement.isTable) shortLabel = elem.CalElement.TableElem.ShortLabel;
                        else if (elem.CalElement.isStructure) shortLabel = elem.CalElement.StructureElem.ShortLabel;
                        break;
                    case MergedType.ExtScalar:
                        uniqueAddressHex = elem.ExtScalar.UniqueAddressHex;
                        shortLabel = elem.ExtScalar.ShortLabel;
                        if (elem.ExtScalar.isBitFlags && elem.ExtScalar.S6xScalar != null) s6xBitFlags = elem.ExtScalar.S6xScalar.BitFlags;
                        break;
                    case MergedType.ExtFunction:
                        uniqueAddressHex = elem.ExtFunction.UniqueAddressHex;
                        shortLabel = elem.ExtFunction.ShortLabel;
                        break;
                    case MergedType.ExtTable:
                        uniqueAddressHex = elem.ExtTable.UniqueAddressHex;
                        shortLabel = elem.ExtTable.ShortLabel;
                        break;
                    case MergedType.ExtStructure:
                        uniqueAddressHex = elem.ExtStructure.UniqueAddressHex;
                        shortLabel = elem.ExtStructure.ShortLabel;
                        break;
                }

                if (uniqueAddressHex == null || uniqueAddressHex == string.Empty) continue;

                // BitFlags before for replacement reasons
                if (s6xBitFlags != null)
                {
                    foreach (S6xBitFlag s6xBF in s6xBitFlags)
                    {
                        if (s6xBF.Skip) continue;
                        if (s6xBF.ShortLabel == null || s6xBF.ShortLabel == string.Empty) continue;
                        if (!slReplacementsCore.ContainsKey(uniqueAddressHex.Replace(" ", "_") + sReplacementCoreBitFlagString + s6xBF.Position.ToString()))
                        {
                            slReplacementsCore.Add(uniqueAddressHex.Replace(" ", "_") + sReplacementCoreBitFlagString + s6xBF.Position.ToString(), s6xBF.ShortLabel);
                        }
                        if (calElemAddressHex == null || calElemAddressHex == string.Empty) continue;
                        if (!slReplacementsCore.ContainsKey(calElemAddressHex + sReplacementCoreBitFlagString + s6xBF.Position.ToString()))
                        {
                            slReplacementsCore.Add(calElemAddressHex + sReplacementCoreBitFlagString + s6xBF.Position.ToString(), s6xBF.ShortLabel);
                        }
                    }
                }
                
                if (shortLabel == null || shortLabel == string.Empty) continue;

                if (!slReplacementsCore.ContainsKey(uniqueAddressHex.Replace(" ", "_")))
                {
                    slReplacementsCore.Add(uniqueAddressHex.Replace(" ", "_"), shortLabel);
                }
                
                if (calElemAddressHex == null || calElemAddressHex == string.Empty) continue;

                if (!slReplacementsCore.ContainsKey(calElemAddressHex))
                {
                    slReplacementsCore.Add(calElemAddressHex, shortLabel);
                }
            }
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

            // Replacements preparation
            OutputReplacementsPrepare(ref slElements);
            
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
            if (is8065SingleBank) col1 += "SB";
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

            // Written Header
            if (S6x.Properties.OutputHeader && S6x.Properties.Header != null && S6x.Properties.Header != string.Empty)
            {
                string[] headerLines = Tools.CommentsReplaced(S6x.Properties.Header, ref lReplacementCoreBaseStrings, ref slReplacementsCore).Replace("\r", "\n").Replace("\n\n", "\n").Replace("\n\n", "\n").Split('\n');
                int iMaxLineLength = 0;
                foreach (string hLine in headerLines) if (hLine.Length > iMaxLineLength) iMaxLineLength = hLine.Length;

                txWriter.WriteLine(OutputTools.BorderedHeader(iMaxLineLength, true));
                txWriter.WriteLine(OutputTools.BorderedEmpty(iMaxLineLength, true));
                foreach (string hLine in headerLines) txWriter.WriteLine(OutputTools.BorderedText(hLine, iMaxLineLength, true));
                txWriter.WriteLine(OutputTools.BorderedEmpty(iMaxLineLength, true));
                txWriter.WriteLine(OutputTools.BorderedFooter(iMaxLineLength, true));
            }
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

        private string[] getOutputTextElementLabel(string label)
        {
            if (label == null || label == string.Empty) return new string[] {};

            return new string[] { label + ":" };
        }

        private string[] getOutputTextElementHeaderComments(string comments)
        {
            if (comments == null || comments == string.Empty) return new string[] {};

            string[] cLines = comments.Replace("\r\n", "\n").Split('\n');
            int cLineMaxLength = commentsHeaderMinWidth;
            foreach (string cLine in cLines) if (cLine.Length > cLineMaxLength) cLineMaxLength = cLine.Length;
            for (int iLine = 0; iLine < cLines.Length; iLine++) 
            {
                cLines[iLine] = string.Format("{0,-" + (commentsPrefix.Length + commentsPrefixMargin).ToString() + "}{1,-" + cLineMaxLength.ToString() + "}{2," + (commentsSuffixHeader.Length + commentsSuffixMarginHeader).ToString() + "}", commentsPrefix, cLines[iLine], commentsSuffixHeader);
            }
            return cLines;
        }

        private string[] getOutputTextElementInlineComments(string comments)
        {
            if (comments == null || comments == string.Empty) return new string[] { };

            string[] cLines = comments.Replace("\r\n", "\n").Split('\n');
            for (int iLine = 0; iLine < cLines.Length; iLine++)
            {
                cLines[iLine] = string.Format("{0,-" + (commentsPrefix.Length + commentsPrefixMargin).ToString() + "}{1,1}", commentsPrefix, cLines[iLine]);
            }
            return cLines;
        }

        private string[] getOutputTextElementWithComments(string firstLine, string secondLine, string[] commentsLines, bool operationType)
        {
            ArrayList alLines = new ArrayList();
            ArrayList alCommentsLines = new ArrayList();

            if (commentsLines != null) if (commentsLines.Length == 0) commentsLines = null;
            if (commentsLines == null)
            {
                alLines.Add(firstLine);
                if (secondLine != string.Empty) alLines.Add(secondLine);
                return (string[])alLines.ToArray(typeof(string));
            }

            int elemLines = 1;
            if (secondLine != string.Empty) elemLines++;

            int elemLinesMaxLength = lineAddressMinWidth + lineAddressBaseFollower.Length;

            if (operationType) elemLinesMaxLength += lineCenterOpsMinWidth + lineRightOpsMinWidth;
            else elemLinesMaxLength += lineCenterCalMinWidth + lineRightCalMinWidth;

            if (firstLine.Length > elemLinesMaxLength) elemLinesMaxLength = firstLine.Length;
            if (secondLine.Length > elemLinesMaxLength) elemLinesMaxLength = secondLine.Length;

            // Lines ReMap / Sometimes Comment can be provided directly
            foreach (string cLine in commentsLines) alCommentsLines.Add(cLine.Replace("\r\n", "\n").Split('\n'));
            for (int iLine = 0; iLine < commentsLines.Length; iLine++)
            {
                string elemText = string.Empty;
                if (iLine == 0) elemText = firstLine;
                else if (iLine == 1) elemText = secondLine;

                alLines.Add(string.Format("{0,-" + elemLinesMaxLength.ToString() + "}{1,1}", elemText, commentsLines[iLine]));
            }
            if (commentsLines.Length < elemLines) alLines.Add(secondLine);

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputValueBitFlagsWithComments(string[][] valueBitFlags)
        {
            string[] arrShortLabels = null;
            string[] arrValues = null;
            int[] arrSizes = null;
            string shortLabels = string.Empty;
            string values = string.Empty;
            string finalFormat = string.Empty;
            int lineSize = 0;
            int leftMargin = 0;

            if (valueBitFlags == null) return new string[] {};
            if (valueBitFlags.Length != 2) return new string[] { };
            if (valueBitFlags[0] == null) return new string[] { };
            if (valueBitFlags[1] == null) return new string[] { };
            if (valueBitFlags[0].Length == 0) return new string[] { };
            if (valueBitFlags[1].Length == 0) return new string[] { };
            if (valueBitFlags[0].Length != valueBitFlags[1].Length) return new string[] { };

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

            leftMargin = lineAddressMinWidth + lineAddressBaseFollower.Length;
            lineSize = leftMargin + lineCenterCalMinWidth + lineRightCalPart1MinWidth;
            while (shortLabels.Length > lineSize - leftMargin) lineSize += 20;
            finalFormat = "{0," + lineSize.ToString() + "}";

            arrShortLabels = null;
            arrValues = null;
            arrSizes = null;

            return new string[] { string.Format(finalFormat, shortLabels), string.Format(finalFormat, values) };
        }

        private string[] getOutputTextElementOtherAddress(string uniqueAddress, bool headerMode)
        {
            if (uniqueAddress == string.Empty) return new string[] { };
            if (!S6x.slProcessOtherAddresses.ContainsKey(uniqueAddress)) return new string[] { };
            S6xOtherAddress other = (S6xOtherAddress)S6x.slOtherAddresses[uniqueAddress];
            if (other == null) return new string[] { };

            ArrayList alLines = new ArrayList();

            // 20200308 - PYM - other.UniqueAddressHex added because now initialized with this value by default, not interest on output.
            if (headerMode)
            {
                if (other.OutputLabel && other.Label != null && other.Label != string.Empty && !other.hasDefaultLabel) alLines.AddRange(getOutputTextElementLabel(other.Label));
                if (!other.InlineComments && other.OutputComments)
                {
                    if (other.Comments != null && other.Comments != string.Empty) alLines.AddRange(getOutputTextElementHeaderComments(Tools.CommentsReviewed(other.Comments, string.Empty, other.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true)));
                }
            }
            else
            {
                if (other.InlineComments && other.OutputComments)
                {
                    if (other.Comments != null && other.Comments != string.Empty) alLines.AddRange(getOutputTextElementInlineComments(Tools.CommentsReviewed(other.Comments, string.Empty, other.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true)));
                }
            }

            other = null;

            return (string[])alLines.ToArray(typeof(string));
        }

        private string getOtherAddressCommentsForOutput(string uniqueAddress, bool removeFirstLineShortLabelLabel)
        {
            if (uniqueAddress == string.Empty) return string.Empty;

            if (!S6x.slProcessOtherAddresses.ContainsKey(uniqueAddress)) return string.Empty;

            S6xOtherAddress other = (S6xOtherAddress)S6x.slOtherAddresses[uniqueAddress];

            if (other == null) return string.Empty;
            
            if (!other.OutputComments) return string.Empty;
                
            if (other.Comments == null || other.Comments == string.Empty) return string.Empty;

            if (other.Comments.Trim() == string.Empty) return string.Empty;

            if (removeFirstLineShortLabelLabel) return Tools.CommentsReviewed(other.Comments.Trim(), string.Empty, other.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true);

            return other.Comments.Trim();
        }

        private string[] getOutputTextIncludedElements(ref Element elem, int startAddress, int endAddress)
        {
            if (elem.IncludedElements == null) return new string[] { };

            ArrayList alLines = new ArrayList();
            for (int iElem = 0; iElem < elem.IncludedElements.Count; iElem++)
            {
                Element incElem = (Element)elem.IncludedElements.GetByIndex(iElem);
                if (incElem == null) continue;
                if (incElem.AddressInt <= endAddress && (incElem.AddressInt >= startAddress || incElem.AddressEndInt >= startAddress))
                {
                    foreach (string sLine in getOutputTextIncludedElement(elem.Type, ref incElem)) alLines.Add(sLine);
                }
            }

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextIncludedElement(MergedType parentElemType, ref Element incElem)
        {
            if (incElem == null) return new string[] {};

            ArrayList alLines = new ArrayList();
            string fullLabel = string.Empty;
            string sValues = string.Empty;
            string valueLine = string.Empty;
            //string sFormat = string.Empty;
            string subType = string.Empty;

            string[] cellsValues = null;
            int[] cellsMinSizes = null;
            OutputCellAlignment[] cellsAlignments = null;

            switch (incElem.Type)
            {
                case MergedType.CalibrationElement:
                    if (incElem.CalElement.isScalar)
                    {
                        fullLabel = incElem.CalElement.ScalarElem.FullLabel + ":";
                        if (incElem.CalElement.ScalarElem.isScaled) sValues = incElem.CalElement.ScalarElem.ValueScaled();
                        else sValues = incElem.CalElement.ScalarElem.Value(10);
                        subType = typeCalElemScalarWord;
                        if (incElem.CalElement.ScalarElem.Byte) subType = typeCalElemScalarByte;
                        //sFormat = "{0,6}: {1,-8}{2,-" + (20 - subType.Length).ToString() + "}{3,8}{4,10}{5,20}";
                        //valueLine = string.Format(sFormat, incElem.CalElement.UniqueAddressHex, incElem.CalElement.ScalarElem.InitialValue, incElem.CalElement.RBaseCalc + " " + incElem.CalElement.ScalarElem.ShortLabel, subType, incElem.CalElement.ScalarElem.Value(16), sValues);
                        cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth, lineCenterCalPart4MinWidth, lineRightCalPart1MinWidth };
                        cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Right, OutputCellAlignment.Right };
                        cellsValues = new string[] { incElem.CalElement.UniqueAddressHex, lineAddressBaseFollower, incElem.CalElement.ScalarElem.InitialValue, incElem.CalElement.RBaseCalc + " " + incElem.CalElement.ScalarElem.ShortLabel, subType, incElem.CalElement.ScalarElem.Value(16), sValues };
                        valueLine = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
                    }
                    else if (incElem.CalElement.isFunction)
                    {
                        subType = typeCalElemFunction;
                        //sFormat = "{0,6}: {1,-8}{2,-10}{3,1}";
                        //fullLabel = string.Format(sFormat, incElem.CalElement.UniqueAddressHex, incElem.CalElement.RBaseCalc, subType, incElem.CalElement.FunctionElem.FullLabel);
                        cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth };
                        cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };
                        cellsValues = new string[] { incElem.CalElement.UniqueAddressHex, lineAddressBaseFollower, incElem.CalElement.RBaseCalc, subType, incElem.CalElement.FunctionElem.FullLabel };
                        fullLabel = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
                    }
                    else if (incElem.CalElement.isTable)
                    {
                        subType = typeCalElemTable;
                        //sFormat = "{0,6}: {1,-8}{2,-10}{3,1}";
                        //fullLabel = string.Format(sFormat, incElem.CalElement.UniqueAddressHex, incElem.CalElement.RBaseCalc, subType, incElem.CalElement.TableElem.FullLabel);
                        cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth };
                        cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };
                        cellsValues = new string[] { incElem.CalElement.UniqueAddressHex, lineAddressBaseFollower, incElem.CalElement.RBaseCalc, subType, incElem.CalElement.TableElem.FullLabel };
                        fullLabel = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
                    }
                    else if (incElem.CalElement.isStructure)
                    {
                        subType = typeCalElemStruct;
                        //sFormat = "{0,6}: {1,-8}{2,-10}{3,1}";
                        //fullLabel = string.Format(sFormat, incElem.CalElement.UniqueAddressHex, incElem.CalElement.RBaseCalc, subType, incElem.CalElement.StructureElem.FullLabel);
                        cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth };
                        cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };
                        cellsValues = new string[] { incElem.CalElement.UniqueAddressHex, lineAddressBaseFollower, incElem.CalElement.RBaseCalc, subType, incElem.CalElement.StructureElem.FullLabel };
                        fullLabel = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
                    }
                    break;
                case MergedType.ExtScalar:
                    fullLabel = incElem.ExtScalar.FullLabel;
                    if (incElem.ExtScalar.isScaled) sValues = incElem.ExtScalar.ValueScaled();
                    else sValues = incElem.ExtScalar.Value(10);
                    subType = typeExtScalarWord;
                    if (incElem.ExtScalar.Byte) subType = typeExtScalarByte;
                    //sFormat = "{0,6}: {1,-8}{2,-" + (20 - subType.Length).ToString() + "}{3,8}{4,10}{5,20}";
                    //valueLine = string.Format(sFormat, incElem.ExtScalar.UniqueAddressHex, incElem.ExtScalar.InitialValue, incElem.ExtScalar.ShortLabel, subType, incElem.ExtScalar.Value(16), sValues);
                    cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth, lineCenterCalPart4MinWidth, lineRightCalPart1MinWidth };
                    cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Right, OutputCellAlignment.Right };
                    cellsValues = new string[] { incElem.ExtScalar.UniqueAddressHex, lineAddressBaseFollower, incElem.ExtScalar.InitialValue, incElem.ExtScalar.ShortLabel, subType, incElem.ExtScalar.Value(16), sValues };
                    valueLine = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
                    break;
                case MergedType.ExtFunction:
                    subType = typeExtFunction;
                    //sFormat = "{0,6}: {1,-10}{2,1}";
                    //fullLabel = string.Format(sFormat, incElem.ExtFunction.UniqueAddressHex, subType, incElem.ExtFunction.FullLabel);
                    cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth };
                    cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };
                    cellsValues = new string[] { incElem.ExtFunction.UniqueAddressHex, lineAddressBaseFollower, string.Empty, subType, incElem.ExtFunction.FullLabel };
                    fullLabel = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
                    break;
                case MergedType.ExtTable:
                    subType = typeExtTable;
                    //sFormat = "{0,6}: {1,-10}{2,1}";
                    //fullLabel = string.Format(sFormat, incElem.ExtTable.UniqueAddressHex, subType, incElem.ExtTable.FullLabel);
                    cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth };
                    cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };
                    cellsValues = new string[] { incElem.ExtTable.UniqueAddressHex, lineAddressBaseFollower, string.Empty, subType, incElem.ExtTable.FullLabel };
                    fullLabel = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
                    break;
                case MergedType.ExtStructure:
                    subType = typeExtStruct;
                    //sFormat = "{0,6}: {1,-10}{2,1}";
                    //fullLabel = string.Format(sFormat, incElem.ExtStructure.UniqueAddressHex, subType, incElem.ExtStructure.FullLabel);
                    cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth };
                    cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };
                    cellsValues = new string[] { incElem.ExtStructure.UniqueAddressHex, lineAddressBaseFollower, string.Empty, subType, incElem.ExtStructure.FullLabel };
                    fullLabel = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
                    break;
            }

            if (fullLabel != string.Empty) alLines.Add(string.Format("{0," + (lineAddressMinWidth + lineAddressBaseFollower.Length).ToString() + "}{1,-6}{2,1}", string.Empty, "Inc", fullLabel));
            if (valueLine != string.Empty) alLines.Add(string.Format("{0," + (lineAddressMinWidth + lineAddressBaseFollower.Length).ToString() + "}{1,-6}{2,1}", string.Empty, "Inc", valueLine));

            return (string[])alLines.ToArray(typeof(string));
    }

        private string[] getOutputTextElementUnknownOperationLine(ref Element elem, ref string subType, ref bool beginWithSpacerLine)
        {
            if (elem.Type != MergedType.UnknownOperationLine) return new string[] { };
            
            string[] cellsValues = null;
            int[] cellsMinSizes = null;
            OutputCellAlignment[] cellsAlignments = null;

            if (elem.UnknownOpPartLine.DuplicatedValues == string.Empty)
            {
                subType = typeUnknownOperationLine;
                cellsValues = new string[] { elem.UnknownOpPartLine.UniqueAddressHex, lineAddressBaseFollower, elem.UnknownOpPartLine.Values, subType };
                cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsMinWidth, 1};
                //sFormat = "{0,6}: {1,-" + (SADDef.UnknownOpPartsAggregationSize * 3 + 22).ToString() + "}{2,1}";
            }
            else
            {
                subType = typeUnknownOperationFillLine;
                cellsValues = new string[] { elem.UnknownOpPartLine.UniqueAddressHex, lineAddressFillFollower, elem.UnknownOpPartLine.AddressEnd, subType, elem.UnknownOpPartLine.DuplicatedValues };
                cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsPart1MinWidth + lineCenterOpsPart2MinWidth, lineCenterOpsPart3MinWidth, 1 };
                //sFormat = "{0,6} -> {1,-25}{2,-19}{3,-2}";
            }

            ArrayList alLines = new ArrayList();

            // OtherAddress Header if available in header mode
            alLines.AddRange(getOutputTextElementOtherAddress(elem.UnknownOpPartLine.UniqueAddress, true));

            // Header is present, a line is inserted before
            beginWithSpacerLine |= (alLines.Count > 0);

            // Included Elements
            alLines.AddRange(getOutputTextIncludedElements(ref elem, elem.AddressInt, elem.AddressEndInt));
            // Current Element and its inline OtherAddress comments if available
            alLines.AddRange(getOutputTextElementWithComments(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments), string.Empty, getOutputTextElementOtherAddress(elem.UnknownOpPartLine.UniqueAddress, false), true));

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementUnknownCalibrationLine(ref Element elem, ref string subType, ref bool beginWithSpacerLine)
        {
            if (elem.Type != MergedType.UnknownCalibrationLine) return new string[] { };

            string[] cellsValues = null;
            int[] cellsMinSizes = null;
            OutputCellAlignment[] cellsAlignments = null;
            int iExpectedSize = 0;

            if (elem.UnknownCalibPartLine.DuplicatedValues == string.Empty)
            {
                subType = typeUnknownCalibrationLine;

                string sValues = string.Empty;
                foreach (string sValueInt in elem.UnknownCalibPartLine.ValuesInt(16)) sValues += string.Format(SADDef.GlobalSeparator + "{0,3}", sValueInt);
                if (sValues.Length < 2) sValues = string.Empty;
                else sValues = sValues.Substring(2);
                iExpectedSize = (SADDef.UnknownCalibPartsAggregationSize + 1) * (3 + SADDef.GlobalSeparator.Length);
                sValues = string.Format("{0,-" + iExpectedSize + "}", sValues);

                string sValues2 = string.Empty;
                foreach (string sValueInt in elem.UnknownCalibPartLine.ValuesInt(10)) sValues2 += string.Format(SADDef.GlobalSeparator + "{0,4}", sValueInt);
                if (sValues2.Length < 2) sValues2 = string.Empty;
                else sValues2 = sValues2.Substring(2);
                iExpectedSize = (SADDef.UnknownCalibPartsAggregationSize + 1) * (4 + SADDef.GlobalSeparator.Length);
                sValues2 = string.Format("{0,-" + iExpectedSize + "}", sValues2);

                cellsValues = new string[] { elem.UnknownCalibPartLine.UniqueAddressHex, lineAddressBaseFollower, elem.UnknownCalibPartLine.Values, subType, sValues, sValues2 };
                cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth + lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth + lineCenterCalPart4MinWidth + lineRightCalPart1MinWidth, lineRightCalPart2MinWidth, lineRightCalPart3MinWidth };
                cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };
                //sFormat = "{0,6}: {1,-" + (SADDef.UnknownOpPartsAggregationSize * 3 + 22).ToString() + "}{2,1}";
            }
            else
            {
                subType = typeUnknownCalibrationFillLine;

                cellsValues = new string[] { elem.UnknownCalibPartLine.UniqueAddressHex, lineAddressFillFollower, elem.UnknownCalibPartLine.AddressEnd, subType, elem.UnknownCalibPartLine.DuplicatedValues };
                cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth + lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth + lineCenterCalPart4MinWidth + lineRightCalPart1MinWidth, lineRightCalPart2MinWidth };
                cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };
                //sFormat = "{0,6} -> {1,-25}{2,-19}{3,-2}";
            }

            ArrayList alLines = new ArrayList();

            // OtherAddress Header if available in header mode
            alLines.AddRange(getOutputTextElementOtherAddress(elem.UnknownCalibPartLine.UniqueAddress, true));

            // Header is present, a line is inserted before
            beginWithSpacerLine |= (alLines.Count > 0);

            // Included Elements
            alLines.AddRange(getOutputTextIncludedElements(ref elem, elem.AddressInt, elem.AddressEndInt));
            // Current Element and its inline OtherAddress comments if available
            alLines.AddRange(getOutputTextElementWithComments(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments), string.Empty, getOutputTextElementOtherAddress(elem.UnknownCalibPartLine.UniqueAddress, false), false));

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementReservedAddress(ref Element elem, ref string subType, ref bool beginWithSpacerLine)
        {
            if (elem.Type != MergedType.ReservedAddress) return new string[] { };

            string[] cellsValues = null;
            int[] cellsMinSizes = null;
            OutputCellAlignment[] cellsAlignments = null;
            string sValues = string.Empty;
            string sValues2 = string.Empty;
            Call cCall = null;
            Vector vVect = null;
            string firstLine = string.Empty;
            string secondLine = string.Empty;

            ArrayList alLines = new ArrayList();

            // OtherAddress Header if available in header mode
            // In this place for managing properly Hex/Ascii return
            alLines.AddRange(getOutputTextElementOtherAddress(elem.ReservedAddress.UniqueAddress, true));

            // Header is present, a line is inserted before
            beginWithSpacerLine |= (alLines.Count > 0);
            
            subType = elem.ReservedAddress.Type.ToString();

            switch (elem.ReservedAddress.Type)
            {
                case ReservedAddressType.Fill:
                    cellsValues = new string[] { elem.ReservedAddress.UniqueAddressHex, lineAddressBaseFollower, elem.ReservedAddress.InitialValue, elem.ReservedAddress.Label };
                    cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsMinWidth, 1 };
                    //sFormat = "{0,6}: {1,-46}{2,1}";
                    break;
                case ReservedAddressType.IntVector:
                    cCall = (Call)Calibration.slCalls[Tools.UniqueAddress(elem.BankNum, elem.ReservedAddress.ValueInt - SADDef.EecBankStartAddress)];
                    if (cCall == null)
                    {
                        switch (elem.BankNum)
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
                        cellsValues = new string[] { elem.ReservedAddress.UniqueAddressHex, lineAddressBaseFollower, elem.ReservedAddress.InitialValue, string.Format("{0,4}", elem.ReservedAddress.Value(16)), sValues, sValues2 };
                        cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsPart1MinWidth, lineCenterOpsPart2MinWidth, lineCenterOpsPart3MinWidth, 1 };
                        //sFormat = "{0,6}: {1,-21}{2,-6}{3,-19}{4,1}";
                    }
                    break;
                case ReservedAddressType.RomSize:
                    cellsValues = new string[] { elem.ReservedAddress.UniqueAddressHex, lineAddressBaseFollower, elem.ReservedAddress.InitialValue, string.Format("{0,4}", elem.ReservedAddress.Value(16)), elem.ReservedAddress.ShortLabel, elem.ReservedAddress.Label + " (ffff is not defined)" };
                    cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsPart1MinWidth, lineCenterOpsPart2MinWidth, lineCenterOpsPart3MinWidth, 1 };
                    //sFormat = "{0,6}: {1,-21}{2,-6}{3,-19}{4,1}";
                    break;
                case ReservedAddressType.CalPointer:
                    sValues = Tools.RegisterInstruction(Calibration.getRbaseByAddress(elem.ReservedAddress.Value(16)).Code);
                    cellsValues = new string[] { elem.ReservedAddress.UniqueAddressHex, lineAddressBaseFollower, elem.ReservedAddress.InitialValue, string.Format("{0,4}", elem.ReservedAddress.Value(16)), elem.ReservedAddress.ShortLabel, elem.ReservedAddress.Label + " " + sValues };
                    cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsPart1MinWidth, lineCenterOpsPart2MinWidth, lineCenterOpsPart3MinWidth, 1 };
                    //sFormat = "{0,6}: {1,-21}{2,-6}{3,-19}{4,1}";
                    break;
                case ReservedAddressType.Ascii:
                case ReservedAddressType.Hex:
                    cellsValues = new string[] { elem.ReservedAddress.UniqueAddressHex, lineAddressBaseFollower, elem.ReservedAddress.InitialValue };
                    cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsMinWidth };
                    firstLine = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
                    secondLine = elem.ReservedAddress.ValueString;
                    alLines.Add(elem.ReservedAddress.FullLabel + ":");
                    alLines.AddRange(getOutputTextElementWithComments(firstLine, secondLine, getOutputTextElementOtherAddress(elem.ReservedAddress.UniqueAddress, false), true));
                    alLines.Add(string.Empty);
                    return (string[])alLines.ToArray(typeof(string));
                default:
                    cellsValues = new string[] { elem.ReservedAddress.UniqueAddressHex, lineAddressBaseFollower, elem.ReservedAddress.InitialValue, string.Format("{0,4}", elem.ReservedAddress.Value(16)), elem.ReservedAddress.ShortLabel, elem.ReservedAddress.Label };
                    cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsPart1MinWidth, lineCenterOpsPart2MinWidth, lineCenterOpsPart3MinWidth, 1 };
                    //sFormat = "{0,6}: {1,-21}{2,-6}{3,-19}{4,1}";
                    break;
            }

            // Current Element and its inline OtherAddress comments if available
            alLines.AddRange(getOutputTextElementWithComments(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments), string.Empty, getOutputTextElementOtherAddress(elem.ReservedAddress.UniqueAddress, false), true));

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementOperation(ref Element elem, ref string subType, ref bool beginWithSpacerLine)
        {
            if (elem.Type != MergedType.Operation) return new string[] { };

            string[] cellsValues = null;
            int[] cellsMinSizes = null;
            OutputCellAlignment[] cellsAlignments = null;

            string firstLine = string.Empty;
            string secondLine = string.Empty;

            if (elem.Operation.isFEConflict)
            {
                cellsValues = new string[] { "//", elem.Operation.Address, elem.Operation.Instruction, elem.Operation.Translation1 };
                cellsMinSizes = new int[] { lineAddressMinWidth + lineAddressBaseFollower.Length + 3, lineCenterOpsPart1MinWidth - 3, lineCenterOpsPart2MinWidth + lineCenterOpsPart3MinWidth, lineRightOpsPart1MinWidth };
                //firstLine = string.Format("{0,-11}{1,-18}{2,-25}{3,-21}", "//", elem.Operation.Address, elem.Operation.Instruction, elem.Operation.Translation1);
            }
            else
            {
                cellsValues = new string[] { elem.Operation.UniqueAddressHex, lineAddressBaseFollower, elem.Operation.OriginalOp, elem.Operation.Instruction, elem.Operation.Translation1 };
                cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsPart1MinWidth, lineCenterOpsPart2MinWidth + lineCenterOpsPart3MinWidth, lineRightOpsPart1MinWidth };
                //firstLine = string.Format("{0,6}: {1,-21}{2,-25}{3,-21}", elem.Operation.UniqueAddressHex, elem.Operation.OriginalOp, elem.Operation.Instruction, elem.Operation.Translation1);
            }
            firstLine = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
            secondLine = string.Empty;
            if (elem.Operation.CallArgsNum > 0)
            {
                cellsValues = new string[] { elem.Operation.UniqueAddressHex, lineAddressBaseFollower, elem.Operation.CallArgs, "#args" };
                cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsMinWidth, lineRightOpsMinWidth };
                //secondLine = string.Format("{0,6}: {1,-46}{2,-21}", elem.Operation.UniqueCallArgsAddressHex, elem.Operation.CallArgs, "#args");
                secondLine = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
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

                            cellsValues = new string[] { string.Empty, string.Format("{0,1} - {1,1} - {2,1}", rBaseAddCalElem.RBaseCalc, rBaseAddCalElem.Address, rBaseAddCalElemShortLabel) };
                            cellsMinSizes = new int[] { lineAddressMinWidth + lineAddressBaseFollower.Length + lineCenterOpsMinWidth, 1 };
                            //secondLine = string.Format("{0,54}{1,1} - {2,1} - {3,1}", string.Empty, rBaseAddCalElem.RBaseCalc, rBaseAddCalElem.Address, rBaseAddCalElemShortLabel);
                            rBaseAddCalElem = null;
                            secondLine = OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments);
                        }
                    }
                }
            }

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

            ArrayList alLines = new ArrayList();
            // OtherAddress Header if available in header mode
            alLines.AddRange(getOutputTextElementOtherAddress(elem.Operation.UniqueAddress, true));
            // Element Comments if available in header mode
            if (elem.Operation.OutputComments && !elem.Operation.InlineComments)
            {
                alLines.AddRange(getOutputTextElementHeaderComments(Tools.CommentsReviewed(elem.Operation.Comments, elem.Operation.ShortLabel, elem.Operation.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true)));
            }
            // Element Label in header mode
            alLines.AddRange(getOutputTextElementLabel(elem.Operation.FullLabel));

            // Header is present, a line is inserted before
            beginWithSpacerLine |= (alLines.Count > 0);

            string[] arrCommentsInline = new string[] {};
            if (elem.Operation.OutputComments && elem.Operation.InlineComments) arrCommentsInline = getOutputTextElementInlineComments(Tools.CommentsReviewed(elem.Operation.Comments, elem.Operation.ShortLabel, elem.Operation.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true));
            if (arrCommentsInline.Length == 0) arrCommentsInline = getOutputTextElementOtherAddress(elem.Operation.UniqueAddress, false);
            // Current Element and its inline comment if available (Current element one or by default OtherAddress one)
            alLines.AddRange(getOutputTextElementWithComments(firstLine, secondLine, arrCommentsInline, true));
            arrCommentsInline = null;

            // Element Included Elements
            alLines.AddRange(getOutputTextIncludedElements(ref elem, elem.AddressInt, elem.AddressEndInt));

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementScalar(ref Element elem, ref string subType, ref bool beginWithSpacerLine)
        {
            if (elem.Type != MergedType.CalibrationElement && elem.Type != MergedType.ExtScalar) return new string[] { };

            Scalar sScalar = null;
            if (elem.Type == MergedType.CalibrationElement) sScalar = elem.CalElement.ScalarElem;
            else sScalar = elem.ExtScalar;

            if (sScalar == null) return new string[] { };

            string[] cellsValues = null;
            int[] cellsMinSizes = null;
            OutputCellAlignment[] cellsAlignments = null;

            string sVariableValue = string.Empty;
            string sValues = string.Empty;

            if (elem.Type == MergedType.CalibrationElement)
            {
                subType = typeCalElemScalarWord;
                if (sScalar.Byte) subType = typeCalElemScalarByte;
            }
            else
            {
                subType = typeExtScalarWord;
                if (sScalar.Byte) subType = typeExtScalarByte;
            }

            if (sScalar.isScaled) sValues = sScalar.ValueScaled();
            else sValues = sScalar.Value(10);

            if (elem.Type == MergedType.CalibrationElement) sVariableValue = elem.CalElement.RBaseCalc + " " + sScalar.ShortLabel;
            else sVariableValue = sScalar.ShortLabel;
                
            ArrayList alLines = new ArrayList();
            // OtherAddress Header if available in header mode
            alLines.AddRange(getOutputTextElementOtherAddress(sScalar.UniqueAddress, true));
            // Element Comments in header mode if available
            if (sScalar.OutputComments && !sScalar.InlineComments)
            {
                alLines.AddRange(getOutputTextElementHeaderComments(Tools.CommentsReviewed(sScalar.Comments, sScalar.ShortLabel, sScalar.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true)));
            }
            // Element Label in header mode
            alLines.AddRange(getOutputTextElementLabel(sScalar.FullLabel));
            
            // Header is present, a line is inserted before
            beginWithSpacerLine |= (alLines.Count > 0);
            
            // Element Bit Flags
            if (sScalar.isBitFlags) alLines.AddRange(getOutputValueBitFlagsWithComments(sScalar.ValueBitFlags));

            string[] arrCommentsInline = new string[] { };
            if (sScalar.OutputComments && sScalar.InlineComments) arrCommentsInline = getOutputTextElementInlineComments(Tools.CommentsReviewed(sScalar.Comments, sScalar.ShortLabel, sScalar.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true));
            if (arrCommentsInline.Length == 0) arrCommentsInline = getOutputTextElementOtherAddress(sScalar.UniqueAddress, false);
            cellsValues = new string[] { sScalar.UniqueAddressHex, lineAddressBaseFollower, sScalar.InitialValue, sVariableValue, subType, sScalar.Value(16), sValues };
            cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth, lineCenterCalPart4MinWidth, lineRightCalPart1MinWidth };
            cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Right, OutputCellAlignment.Right };
            // Current Element and its inline comment if available (Current element one or by default OtherAddress one)
            alLines.AddRange(getOutputTextElementWithComments(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments), string.Empty, arrCommentsInline, elem.Type == MergedType.ExtScalar));
            arrCommentsInline = null;

            // Element Included Elements
            alLines.AddRange(getOutputTextIncludedElements(ref elem, elem.AddressInt, elem.AddressEndInt));

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementFunction(ref Element elem, ref string subType, ref bool beginWithSpacerLine)
        {
            if (elem.Type != MergedType.CalibrationElement && elem.Type != MergedType.ExtFunction) return new string[] { };

            Function fFunction = null;
            if (elem.Type == MergedType.CalibrationElement) fFunction = elem.CalElement.FunctionElem;
            else fFunction = elem.ExtFunction;

            if (fFunction == null) return new string[] { };

            if (elem.Type == MergedType.CalibrationElement) subType = typeCalElemFunction;
            else subType = typeExtFunction;

            ArrayList alLines = new ArrayList();

            // OtherAddress Header if available in header mode
            alLines.AddRange(getOutputTextElementOtherAddress(fFunction.UniqueAddress, true));
            // Element Comments in header mode if available
            if (fFunction.OutputComments) alLines.AddRange(getOutputTextElementHeaderComments(Tools.CommentsReviewed(fFunction.Comments, fFunction.ShortLabel, fFunction.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true)));
            // Element Label in header mode
            alLines.AddRange(getOutputTextElementLabel(fFunction.FullLabel));

            // Header is present, a line is inserted before
            beginWithSpacerLine |= (alLines.Count > 0);

            //sFormat = "{0,6}: {1,-18}{2,-" + (20 - subType.Length).ToString() + "}{3,8},{4,9}{5,20},{6,9}";

            int[] cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth - 4, SADDef.GlobalSeparator.Length, lineCenterCalPart4MinWidth + 4 - SADDef.GlobalSeparator.Length, lineRightCalPart1MinWidth, SADDef.GlobalSeparator.Length, lineRightCalPart2MinWidth - SADDef.GlobalSeparator.Length };
            OutputCellAlignment[] cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Right, OutputCellAlignment.Right, OutputCellAlignment.Right, OutputCellAlignment.Right, OutputCellAlignment.Right, OutputCellAlignment.Right };

            foreach (ScalarLine scalarLine in fFunction.Lines)
            {
                string sValues = string.Empty;
                string sValues2 = string.Empty;
                if (fFunction.isInputScaled) sValues = scalarLine.Scalars[0].ValueScaled(fFunction.getInputScaleExpression, fFunction.getInputScalePrecision);
                else sValues = scalarLine.Scalars[0].Value(10);
                if (fFunction.isOutputScaled) sValues2 = scalarLine.Scalars[1].ValueScaled(fFunction.getOutputScaleExpression, fFunction.getOutputScalePrecision);
                else sValues2 = scalarLine.Scalars[1].Value(10);

                // OtherAddress Header if available in header mode
                alLines.AddRange(getOutputTextElementOtherAddress(scalarLine.UniqueAddress, true));
                // Element Included Elements
                alLines.AddRange(getOutputTextIncludedElements(ref elem, scalarLine.AddressInt, scalarLine.AddressEndInt));

                string[] cellsValues = new string[] { scalarLine.UniqueAddressHex, lineAddressBaseFollower, scalarLine.InitialValue, subType, scalarLine.Scalars[0].Value(16), SADDef.GlobalSeparator, scalarLine.Scalars[1].Value(16), sValues, SADDef.GlobalSeparator, sValues2 };
                // Current Element and its inline OtherAddress comment if available
                alLines.AddRange(getOutputTextElementWithComments(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments), string.Empty, getOutputTextElementOtherAddress(scalarLine.UniqueAddress, false), elem.Type == MergedType.ExtFunction));
            }

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementTable(ref Element elem, ref string subType, ref bool beginWithSpacerLine)
        {
            if (elem.Type != MergedType.CalibrationElement && elem.Type != MergedType.ExtTable) return new string[] { };

            Table tTable = null;
            if (elem.Type == MergedType.CalibrationElement) tTable = elem.CalElement.TableElem;
            else tTable = elem.ExtTable;

            if (tTable == null) return new string[] { };

            if (elem.Type == MergedType.CalibrationElement) subType = typeCalElemTable;
            else subType = typeExtTable;

            ArrayList alLines = new ArrayList();

            // OtherAddress Header if available in header mode
            alLines.AddRange(getOutputTextElementOtherAddress(tTable.UniqueAddress, true));
            // Element Comments in header mode if available
            if (tTable.OutputComments) alLines.AddRange(getOutputTextElementHeaderComments(Tools.CommentsReviewed(tTable.Comments, tTable.ShortLabel, tTable.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true)));
            // Element Label in header mode
            alLines.AddRange(getOutputTextElementLabel(tTable.FullLabel));

            // Header is present, a line is inserted before
            beginWithSpacerLine |= (alLines.Count > 0);

            int[] cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, -1, subType.Length + tableSeparator2MinWidth, -1, -1 };
            OutputCellAlignment[] cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };

            foreach (ScalarLine scalarLine in tTable.Lines)
            {
                string sValues = string.Empty;
                string sValues2 = string.Empty;
                foreach (Scalar scalar in scalarLine.Scalars)
                {
                    if (tTable.WordOutput)
                    {
                        sValues += string.Format(SADDef.GlobalSeparator + "{0," + tableWordHexMinWidth.ToString() + "}", scalar.Value(16));
                        if (tTable.isCellsScaled) sValues2 += string.Format(SADDef.GlobalSeparator + "{0," + tableWordDecScaledMinWidth.ToString() + "}", scalar.ValueScaled(tTable.getCellsScaleExpression, tTable.getCellsScalePrecision));
                        else sValues2 += string.Format(SADDef.GlobalSeparator + "{0," + tableWordDecUnscaledMinWidth.ToString() + "}", scalar.Value(10));
                    }
                    else
                    {
                        sValues += string.Format(SADDef.GlobalSeparator + "{0," + tableByteHexMinWidth.ToString() + "}", scalar.Value(16));
                        if (tTable.isCellsScaled) sValues2 += string.Format(SADDef.GlobalSeparator + "{0," + tableByteDecScaledMinWidth.ToString() + "}", scalar.ValueScaled(tTable.getCellsScaleExpression, tTable.getCellsScalePrecision));
                        else sValues2 += string.Format(SADDef.GlobalSeparator + "{0," + tableByteDecUnscaledMinWidth.ToString() + "}", scalar.Value(10));
                    }
                }
                sValues = sValues.Substring(2);
                sValues2 = sValues2.Substring(2);
                if (cellsMinSizes[2] < 0)
                {
                    cellsMinSizes[2] = scalarLine.InitialValue.Length + tableSeparator1MinWidth;
                    if (cellsMinSizes[2] < lineCenterCalPart1MinWidth)
                    {
                        cellsMinSizes[2] = lineCenterCalPart1MinWidth;
                        if (cellsMinSizes[3] < lineCenterCalPart2MinWidth) cellsMinSizes[3] = lineCenterCalPart2MinWidth;
                    }
                }
                if (cellsMinSizes[4] < 0) cellsMinSizes[4] = sValues.Length + tableSeparator3MinWidth;
                if (cellsMinSizes[5] < 0) cellsMinSizes[5] = sValues2.Length;

                // OtherAddress Header if available in header mode
                alLines.AddRange(getOutputTextElementOtherAddress(scalarLine.UniqueAddress, true));
                // Element Included Elements
                alLines.AddRange(getOutputTextIncludedElements(ref elem, scalarLine.AddressInt, scalarLine.AddressEndInt));
                
                string[] cellsValues = new string[] { scalarLine.UniqueAddressHex, lineAddressBaseFollower, scalarLine.InitialValue, subType, sValues, sValues2 };
                // Current Element without its inline OtherAddress comment for output reasons
                alLines.Add(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments));
            }

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementStructure(ref Element elem, ref string subType, ref bool beginWithSpacerLine)
        {
            if (elem.Type != MergedType.CalibrationElement && elem.Type != MergedType.ExtStructure) return new string[] { };

            Structure sStruct = null;
            if (elem.Type == MergedType.CalibrationElement) sStruct = elem.CalElement.StructureElem;
            else sStruct = elem.ExtStructure;

            if (sStruct == null) return new string[] { };
            if (sStruct.ParentStructure != null) return new string[] { };  // Included / Duplicated elements are not generated
            
            if (elem.Type == MergedType.CalibrationElement) subType = typeCalElemStruct;
            else subType = typeExtStruct;

            ArrayList alLines = new ArrayList();

            // OtherAddress Header if available in header mode
            alLines.AddRange(getOutputTextElementOtherAddress(sStruct.UniqueAddress, true));
            // Element Comments in header mode if available
            if (sStruct.OutputComments) alLines.AddRange(getOutputTextElementHeaderComments(Tools.CommentsReviewed(sStruct.Comments, sStruct.ShortLabel, sStruct.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true)));
            // Element Label in header mode
            alLines.AddRange(getOutputTextElementLabel(sStruct.FullLabel));

            // Header is present, a line is inserted before
            beginWithSpacerLine |= (alLines.Count > 0);

            int maxInitialValuesLength = 0;
            int[] maxItemValuesLengths = new int[] { };
            foreach (StructureLine sLine in sStruct.Lines)
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
                    string sValue = ((StructureItem)sLine.Items[iCol]).Value(sLine.NumberInStructure);
                    if (sValue.Length > maxItemValuesLengths[iCol]) maxItemValuesLengths[iCol] = sValue.Length;
                }
            }

            int[] cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, maxInitialValuesLength + tableSeparator1MinWidth, subType.Length + tableSeparator2MinWidth, 1 };
            OutputCellAlignment[] cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };

            if (cellsMinSizes[2] < lineCenterCalPart1MinWidth)
            {
                cellsMinSizes[2] = lineCenterCalPart1MinWidth;
                if (cellsMinSizes[3] < lineCenterCalPart2MinWidth) cellsMinSizes[3] = lineCenterCalPart2MinWidth;
            }

            foreach (StructureLine sLine in sStruct.Lines)
            {
                string sValues = string.Empty;
                for (int iCol = 0; iCol < sLine.Items.Count; iCol++)
                {
                    string sValue = ((StructureItem)sLine.Items[iCol]).Value(sLine.NumberInStructure);
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
                // OtherAddress Header if available in header mode
                alLines.AddRange(getOutputTextElementOtherAddress(sLine.UniqueAddress, true));
                // Element Included Elements
                alLines.AddRange(getOutputTextIncludedElements(ref elem, sLine.AddressInt, sLine.AddressEndInt));

                string[] cellsValues = new string[] { sLine.UniqueAddressHex, lineAddressBaseFollower, sLine.InitialValue, subType, sValues };
                // Current Element and its inline OtherAddress comment if available
                alLines.AddRange(getOutputTextElementWithComments(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments), string.Empty, getOutputTextElementOtherAddress(sLine.UniqueAddress, false), elem.Type == MergedType.ExtStructure));
            }

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementVector(ref Element elem, ref string subType, ref bool beginWithSpacerLine)
        {
            if (elem.Type != MergedType.Vector) return new string[] { };

            subType = typeVector;

            ArrayList alLines = new ArrayList();

            // OtherAddress Header if available in header mode
            alLines.AddRange(getOutputTextElementOtherAddress(elem.Vector.UniqueSourceAddress, true));

            // Header is present, a line is inserted before
            beginWithSpacerLine |= (alLines.Count > 0);

            if (elem.Vector.VectList != null)
            {
                if (elem.Vector.VectList.UniqueAddress == elem.Vector.UniqueSourceAddress)
                {
                    // Element Comments in header mode if available
                    alLines.AddRange(getOutputTextElementHeaderComments(Tools.CommentsReviewed(elem.Vector.VectList.Comments, elem.Vector.VectList.ShortLabel, elem.Vector.VectList.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true)));
                    // Element Label in header mode
                    alLines.AddRange(getOutputTextElementLabel(elem.Vector.VectList.FullLabel));
                }
            }

            int[] cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsPart1MinWidth, lineCenterOpsPart2MinWidth, lineCenterOpsPart3MinWidth, 1 };
            string[] cellsValues = new string[] { elem.Vector.UniqueSourceAddressHex, lineAddressBaseFollower, elem.Vector.InitialValue, elem.Vector.Address, "Bank " + elem.Vector.ApplyOnBankNum + " " + subType, elem.Vector.FullLabel };
            OutputCellAlignment[] cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };
            // Current Element and its inline OtherAddress comment if available
            alLines.AddRange(getOutputTextElementWithComments(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments), string.Empty, getOutputTextElementOtherAddress(elem.Vector.UniqueSourceAddress, false), true));
            
            // Element Included Elements
            alLines.AddRange(getOutputTextIncludedElements(ref elem, elem.Vector.SourceAddressInt, elem.Vector.SourceAddressInt + 1));

            return (string[])alLines.ToArray(typeof(string));
        }

        private void processOutputTextElement(ref TextWriter txWriter, ref Element elem, ref object[] previousResult)
        {
            if (elem.isIncluded) return;

            MergedType prevType = MergedType.Unknown;
            string subType = string.Empty;
            string prevSubType = string.Empty;
            ArrayList alLines = new ArrayList();
            bool beginWithSpacerLine = false;
            int lastLinesCount = 0;

            if (previousResult != null)
            {
                prevType = (MergedType)previousResult[0];
                prevSubType = (string)previousResult[1];

                beginWithSpacerLine |= (prevType != elem.Type);
            }

            switch (elem.Type)
            {
                case MergedType.UnknownOperationLine:
                    alLines.AddRange(getOutputTextElementUnknownOperationLine(ref elem, ref subType, ref beginWithSpacerLine));
                    break;
                case MergedType.UnknownCalibrationLine:
                    alLines.AddRange(getOutputTextElementUnknownCalibrationLine(ref elem, ref subType, ref beginWithSpacerLine));
                    break;
                case MergedType.ReservedAddress:
                    alLines.AddRange(getOutputTextElementReservedAddress(ref elem, ref subType, ref beginWithSpacerLine));
                    beginWithSpacerLine |= (subType != prevSubType);
                    break;
                case MergedType.Operation:
                    lastLinesCount = alLines.Count;
                    if (Calibration.alMainCallsUniqueAddresses.Contains(elem.Operation.UniqueAddress))
                    {
                        Call cCall = (Call)Calibration.slCalls[elem.Operation.UniqueAddress];
                        if (cCall.S6xRoutine != null)
                        {
                            if (cCall.S6xRoutine.OutputComments)
                            {
                                alLines.AddRange(getOutputTextElementHeaderComments(Tools.CommentsReviewed(cCall.Comments, cCall.ShortLabel, cCall.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true)));
                            }
                        }
                        alLines.AddRange(getOutputTextElementLabel(cCall.FullLabel));
                        cCall = null;
                        // Added header or label requires a new line at beginning (0)
                        // Only required when lastLinesCount was at 0, otherwise a new line was already added
                        beginWithSpacerLine |= (lastLinesCount == 0 && alLines.Count > 0);
                    }
                    else if (S6x.slProcessRoutines.ContainsKey(elem.Operation.UniqueAddress))
                    {
                        lastLinesCount = alLines.Count;
                        S6xRoutine s6xRoutine = (S6xRoutine)S6x.slProcessRoutines[elem.Operation.UniqueAddress];
                        if (s6xRoutine.OutputComments)
                        {
                            alLines.AddRange(getOutputTextElementHeaderComments(Tools.CommentsReviewed(s6xRoutine.Comments, s6xRoutine.ShortLabel, s6xRoutine.Label, ref lReplacementCoreBaseStrings, ref slReplacementsCore, true, true)));
                        }
                        alLines.AddRange(getOutputTextElementLabel((s6xRoutine.Label != string.Empty && s6xRoutine.ShortLabel != string.Empty && s6xRoutine.Label != s6xRoutine.ShortLabel) ? s6xRoutine.ShortLabel + " - " + s6xRoutine.Label : s6xRoutine.Label));
                        s6xRoutine = null;
                        // Added header or label requires a new line at beginning (0)
                        // Only required when lastLinesCount was at 0, otherwise a new line was already added
                        beginWithSpacerLine |= (lastLinesCount == 0 && alLines.Count > 0);
                    }
                    else
                    {
                        beginWithSpacerLine |= (lastLinesCount == 0 && prevSubType == "newLine");
                    }

                    alLines.AddRange(getOutputTextElementOperation(ref elem, ref subType, ref beginWithSpacerLine));
                    break;
                case MergedType.CalibrationElement:
                    if (elem.CalElement.isScalar)
                    {
                        alLines.AddRange(getOutputTextElementScalar(ref elem, ref subType, ref beginWithSpacerLine));
                        beginWithSpacerLine |= (subType != prevSubType);
                    }
                    else if (elem.CalElement.isFunction)
                    {
                        alLines.AddRange(getOutputTextElementFunction(ref elem, ref subType, ref beginWithSpacerLine));
                    }
                    else if (elem.CalElement.isTable)
                    {
                        alLines.AddRange(getOutputTextElementTable(ref elem, ref subType, ref beginWithSpacerLine));
                    }
                    else if (elem.CalElement.isStructure)
                    {
                        alLines.AddRange(getOutputTextElementStructure(ref elem, ref subType, ref beginWithSpacerLine));
                    }
                    break;
                case MergedType.ExtScalar:
                    alLines.AddRange(getOutputTextElementScalar(ref elem, ref subType, ref beginWithSpacerLine));
                    beginWithSpacerLine |= (subType != prevSubType);
                    break;
                case MergedType.ExtFunction:
                    alLines.AddRange(getOutputTextElementFunction(ref elem, ref subType, ref beginWithSpacerLine));
                    break;
                case MergedType.ExtTable:
                    alLines.AddRange(getOutputTextElementTable(ref elem, ref subType, ref beginWithSpacerLine));
                    break;
                case MergedType.ExtStructure:
                    alLines.AddRange(getOutputTextElementStructure(ref elem, ref subType, ref beginWithSpacerLine));
                    break;
                case MergedType.Vector:
                    alLines.AddRange(getOutputTextElementVector(ref elem, ref subType, ref beginWithSpacerLine));
                    break;
            }

            if (beginWithSpacerLine && alLines.Count > 0) txWriter.WriteLine(string.Empty);
            foreach (string sLine in alLines) txWriter.WriteLine(sLine);

            previousResult = new object[] { elem.Type, subType };
        }
    }
}
