using System;
using System.Collections;
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

        private string binaryFileName = string.Empty;

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

        public int GLOBAL_LINE_ADDRESS_WIDTH { get { return lineAddressMinWidth; } set { lineAddressMinWidth = value; } }
        public int GLOBAL_LINE_OPS_CENTER_WIDTH { get { return lineCenterOpsMinWidth; } set { lineCenterOpsMinWidth = value; } }
        public int GLOBAL_LINE_OPS_CENTER_PART1_WIDTH { get { return lineCenterOpsPart1MinWidth; } set { lineCenterOpsPart1MinWidth = value; } }
        public int GLOBAL_LINE_OPS_CENTER_PART2_WIDTH { get { return lineCenterOpsPart2MinWidth; } set { lineCenterOpsPart2MinWidth = value; } }
        public int GLOBAL_LINE_OPS_RIGHT_WIDTH { get { return lineRightOpsMinWidth; } set { lineRightOpsMinWidth = value; } }
        public int GLOBAL_LINE_OPS_RIGHT_PART1_WIDTH { get { return lineRightOpsPart1MinWidth; } set { lineRightOpsPart1MinWidth = value; } }

        public int GLOBAL_LINE_CAL_CENTER_WIDTH { get { return lineCenterCalMinWidth; } set { lineCenterCalMinWidth = value; } }
        public int GLOBAL_LINE_CAL_CENTER_PART1_WIDTH { get { return lineCenterCalPart1MinWidth; } set { lineCenterCalPart1MinWidth = value; } }
        public int GLOBAL_LINE_CAL_CENTER_PART2_WIDTH { get { return lineCenterCalPart2MinWidth; } set { lineCenterCalPart2MinWidth = value; } }
        public int GLOBAL_LINE_CAL_CENTER_PART3_WIDTH { get { return lineCenterCalPart3MinWidth; } set { lineCenterCalPart3MinWidth = value; } }
        public int GLOBAL_LINE_CAL_RIGHT_WIDTH { get { return lineRightCalMinWidth; } set { lineRightCalMinWidth = value; } }
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

        private string[] getOutputTextElementLabel(string label)
        {
            if (label == null || label == string.Empty) return new string[] {};

            return new string[] { string.Empty, label + ":" };
        }

        private string[] getOutputTextElementHeaderComments(string comments)
        {
            if (comments == null || comments == string.Empty) return new string[] {};

            string[] cLines = comments.Replace("\r\n", "\n").Split('\n');
            int cLineMaxLength = 80;
            foreach (string cLine in cLines) if (cLine.Length > cLineMaxLength) cLineMaxLength = cLine.Length;
            for (int iLine = 0; iLine < cLines.Length; iLine++) 
            {
                cLines[iLine] = string.Format("{0,-4}{1,-" + cLineMaxLength.ToString() + "}{2,4}", "//", cLines[iLine], "//");
            }
            return cLines;
        }

        private string[] getOutputTextElementWithComments(string firstLine, string secondLine, string comments)
        {
            ArrayList alLines = new ArrayList();

            if (comments == null || comments == string.Empty)
            {
                alLines.Add(firstLine);
                if (secondLine != string.Empty) alLines.Add(secondLine);
                return (string[])alLines.ToArray(typeof(string));
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

                alLines.Add(string.Format("{0,-" + elemLinesMaxLength.ToString() + "}{1,10}{2,-" + cLineMaxLength.ToString() + "}{3,4}", elemText, "//  ", cLines[iLine], "//"));
            }
            if (cLines.Length < elemLines) alLines.Add(secondLine);
            cLines = null;

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

        private string[] getOutputTextElementOtherAddress(string uniqueAddress)
        {
            if (uniqueAddress == string.Empty) return new string[] { };

            ArrayList alLines = new ArrayList();
            S6xOtherAddress other = null;

            if (!S6x.slProcessOtherAddresses.ContainsKey(uniqueAddress)) return new string[] {};

            other = (S6xOtherAddress)S6x.slOtherAddresses[uniqueAddress];

            if (other.Label != null && other.Label != string.Empty) alLines.Add(getOutputTextElementLabel(other.Label));
            if (other.OutputComments)
            {
                if (other.Comments != null && other.Comments != string.Empty) alLines.Add(getOutputTextElementHeaderComments(other.Comments));
            }

            other = null;

            return (string[])alLines.ToArray(typeof(string));
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

        private string[] getOutputTextElementUnknownOperationLine(ref Element elem, ref string subType)
        {
            if (elem.Type != MergedType.UnknownOperationLine) return new string[] { };
            
            string[] arrOtherAddressLines = getOutputTextElementOtherAddress(elem.UnknownOpPartLine.UniqueAddress);
            string[] arrIncludedElementsLines = getOutputTextIncludedElements(ref elem, elem.AddressInt, elem.AddressEndInt);
            string[] arrElemLines = null;

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

            arrElemLines = new string[] { OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments) };

            string[] arrLines = new string[arrOtherAddressLines.Length + arrIncludedElementsLines.Length + arrElemLines.Length];
            int iLine = 0;
            foreach (string sLine in arrOtherAddressLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrIncludedElementsLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrElemLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }

            return arrLines;
        }

        private string[] getOutputTextElementUnknownCalibrationLine(ref Element elem, ref string subType)
        {
            if (elem.Type != MergedType.UnknownCalibrationLine) return new string[] { };

            string[] arrOtherAddressLines = getOutputTextElementOtherAddress(elem.UnknownCalibPartLine.UniqueAddress);
            string[] arrIncludedElementsLines = getOutputTextIncludedElements(ref elem, elem.AddressInt, elem.AddressEndInt);
            string[] arrElemLines = null;

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

            arrElemLines = new string[] { OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments) };

            string[] arrLines = new string[arrOtherAddressLines.Length + arrIncludedElementsLines.Length + arrElemLines.Length];
            int iLine = 0;
            foreach (string sLine in arrOtherAddressLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrIncludedElementsLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrElemLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }

            return arrLines;
        }

        private string[] getOutputTextElementReservedAddress(ref Element elem, ref string subType)
        {
            if (elem.Type != MergedType.ReservedAddress) return new string[] { };

            string[] arrOtherAddressLines = getOutputTextElementOtherAddress(elem.ReservedAddress.UniqueAddress);
            string[] arrElemLines = null;

            string[] cellsValues = null;
            int[] cellsMinSizes = null;
            OutputCellAlignment[] cellsAlignments = null;
            string sValues = string.Empty;
            string sValues2 = string.Empty;
            Call cCall = null;
            Vector vVect = null;

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
                    arrElemLines = new string[] { elem.ReservedAddress.FullLabel + ":", OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments), elem.ReservedAddress.ValueString, string.Empty };
                    break;
                default:
                    cellsValues = new string[] { elem.ReservedAddress.UniqueAddressHex, lineAddressBaseFollower, elem.ReservedAddress.InitialValue, string.Format("{0,4}", elem.ReservedAddress.Value(16)), elem.ReservedAddress.ShortLabel, elem.ReservedAddress.Label };
                    cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsPart1MinWidth, lineCenterOpsPart2MinWidth, lineCenterOpsPart3MinWidth, 1 };
                    //sFormat = "{0,6}: {1,-21}{2,-6}{3,-19}{4,1}";
                    break;
            }
            
            // Null except for Ascii & Hex
            if (arrElemLines == null) arrElemLines = new string[] { OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments) };

            string[] arrLines = new string[arrOtherAddressLines.Length + arrElemLines.Length];
            int iLine = 0;
            foreach (string sLine in arrOtherAddressLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrElemLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }

            return arrLines;
        }

        private string[] getOutputTextElementOperation(ref Element elem, ref string subType)
        {
            if (elem.Type != MergedType.Operation) return new string[] { };

            string[] arrOtherAddressLines = getOutputTextElementOtherAddress(elem.Operation.UniqueAddress);
            string[] arrLabelLines = getOutputTextElementLabel(elem.Operation.FullLabel);
            string[] arrElemLines = null;

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

            arrElemLines = getOutputTextElementWithComments(firstLine, secondLine, elem.Operation.Comments);

            string[] arrLines = new string[arrOtherAddressLines.Length + arrLabelLines.Length + arrElemLines.Length];
            int iLine = 0;
            foreach (string sLine in arrOtherAddressLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrLabelLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrElemLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }

            return arrLines;
        }

        private string[] getOutputTextElementScalar(ref Element elem, ref string subType)
        {
            if (elem.Type != MergedType.CalibrationElement && elem.Type != MergedType.ExtScalar) return new string[] { };

            Scalar sScalar = null;
            if (elem.Type == MergedType.CalibrationElement) sScalar = elem.CalElement.ScalarElem;
            else sScalar = elem.ExtScalar;

            if (sScalar == null) return new string[] { };

            string[] arrOtherAddressLines = getOutputTextElementOtherAddress(sScalar.UniqueAddress);
            string[] arrIncludedElementsLines = getOutputTextIncludedElements(ref elem, elem.AddressInt, elem.AddressEndInt);
            string[] arrLabelLines = null;
            string[] arrBitFlagsLines = null;
            string[] arrElemLines = null;
            string[] arrLines = null;

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

            arrLabelLines = getOutputTextElementLabel(sScalar.FullLabel);

            arrBitFlagsLines = new string[] { };
            if (sScalar.isBitFlags) arrBitFlagsLines = getOutputValueBitFlagsWithComments(sScalar.ValueBitFlags);

            if (sScalar.isScaled) sValues = sScalar.ValueScaled();
            else sValues = sScalar.Value(10);

            if (elem.Type == MergedType.CalibrationElement) sVariableValue = elem.CalElement.RBaseCalc + " " + sScalar.ShortLabel;
            else sVariableValue = sScalar.ShortLabel;
                
            cellsValues = new string[] { sScalar.UniqueAddressHex, lineAddressBaseFollower, sScalar.InitialValue, sVariableValue, subType, sScalar.Value(16), sValues };
            cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterCalPart1MinWidth, lineCenterCalPart2MinWidth, lineCenterCalPart3MinWidth, lineCenterCalPart4MinWidth, lineRightCalPart1MinWidth };
            cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Right, OutputCellAlignment.Right };

            arrElemLines = getOutputTextElementWithComments(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments), string.Empty, sScalar.Comments);

            arrLines = new string[arrOtherAddressLines.Length + arrLabelLines.Length + arrBitFlagsLines.Length + arrElemLines.Length + arrIncludedElementsLines.Length];
            int iLine = 0;
            foreach (string sLine in arrOtherAddressLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrLabelLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrBitFlagsLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrElemLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }
            foreach (string sLine in arrIncludedElementsLines)
            {
                arrLines[iLine] = sLine;
                iLine++;
            }

            return arrLines;
        }

        private string[] getOutputTextElementFunction(ref Element elem, ref string subType)
        {
            if (elem.Type != MergedType.CalibrationElement && elem.Type != MergedType.ExtFunction) return new string[] { };

            Function fFunction = null;
            if (elem.Type == MergedType.CalibrationElement) fFunction = elem.CalElement.FunctionElem;
            else fFunction = elem.ExtFunction;

            if (fFunction == null) return new string[] { };

            ArrayList alLines = new ArrayList();

            if (elem.Type == MergedType.CalibrationElement) subType = typeCalElemFunction;
            else subType = typeExtFunction;

            alLines.AddRange(getOutputTextElementLabel(fFunction.FullLabel));
            alLines.AddRange(getOutputTextElementHeaderComments(fFunction.Comments));

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
                alLines.AddRange(getOutputTextElementOtherAddress(scalarLine.UniqueAddress));
                alLines.AddRange(getOutputTextIncludedElements(ref elem, scalarLine.AddressInt, scalarLine.AddressEndInt));

                string[] cellsValues = new string[] { scalarLine.UniqueAddressHex, lineAddressBaseFollower, scalarLine.InitialValue, subType, scalarLine.Scalars[0].Value(16), SADDef.GlobalSeparator, scalarLine.Scalars[1].Value(16), sValues, SADDef.GlobalSeparator, sValues2 };
                alLines.Add(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments));
            }

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementTable(ref Element elem, ref string subType)
        {
            if (elem.Type != MergedType.CalibrationElement && elem.Type != MergedType.ExtTable) return new string[] { };

            Table tTable = null;
            if (elem.Type == MergedType.CalibrationElement) tTable = elem.CalElement.TableElem;
            else tTable = elem.ExtTable;

            if (tTable == null) return new string[] { };

            ArrayList alLines = new ArrayList();

            if (elem.Type == MergedType.CalibrationElement) subType = typeCalElemTable;
            else subType = typeExtTable;

            alLines.AddRange(getOutputTextElementLabel(tTable.FullLabel));
            alLines.AddRange(getOutputTextElementHeaderComments(tTable.Comments));

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
                alLines.AddRange(getOutputTextElementOtherAddress(scalarLine.UniqueAddress));
                alLines.AddRange(getOutputTextIncludedElements(ref elem, scalarLine.AddressInt, scalarLine.AddressEndInt));
                string[] cellsValues = new string[] { scalarLine.UniqueAddressHex, lineAddressBaseFollower, scalarLine.InitialValue, subType, sValues, sValues2 };
                alLines.Add(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments));
            }

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementStructure(ref Element elem, ref string subType)
        {
            if (elem.Type != MergedType.CalibrationElement && elem.Type != MergedType.ExtStructure) return new string[] { };

            Structure sStruct = null;
            if (elem.Type == MergedType.CalibrationElement) sStruct = elem.CalElement.StructureElem;
            else sStruct = elem.ExtStructure;

            if (sStruct == null) return new string[] { };
            if (sStruct.ParentStructure != null) return new string[] { };  // Included / Duplicated elements are not generated
            
            ArrayList alLines = new ArrayList();

            if (elem.Type == MergedType.CalibrationElement) subType = typeCalElemStruct;
            else subType = typeExtStruct;

            alLines.AddRange(getOutputTextElementLabel(sStruct.FullLabel));
            alLines.AddRange(getOutputTextElementHeaderComments(sStruct.Comments));
            
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
                    string sValue = ((StructureItem)sLine.Items[iCol]).Value();
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
                alLines.AddRange(getOutputTextElementOtherAddress(sLine.UniqueAddress));
                alLines.AddRange(getOutputTextIncludedElements(ref elem, sLine.AddressInt, sLine.AddressEndInt));
                string[] cellsValues = new string[] { sLine.UniqueAddressHex, lineAddressBaseFollower, sLine.InitialValue, subType, sValues };
                alLines.Add(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments));
            }

            return (string[])alLines.ToArray(typeof(string));
        }

        private string[] getOutputTextElementVector(ref Element elem, ref string subType)
        {
            if (elem.Type != MergedType.Vector) return new string[] { };

            ArrayList alLines = new ArrayList();

            subType = typeVector;

            if (elem.Vector.VectList == null)
            {
                alLines.AddRange(getOutputTextElementOtherAddress(elem.Vector.UniqueSourceAddress));
            }
            else if (elem.Vector.VectList.UniqueAddress == elem.Vector.UniqueSourceAddress)
            {
                alLines.AddRange(getOutputTextElementLabel(elem.Vector.VectList.FullLabel));
                alLines.AddRange(getOutputTextElementHeaderComments(elem.Vector.VectList.Comments));
            }
            else
            {
                alLines.AddRange(getOutputTextElementOtherAddress(elem.Vector.UniqueSourceAddress));
            }

            int[] cellsMinSizes = new int[] { lineAddressMinWidth, lineAddressBaseFollower.Length, lineCenterOpsPart1MinWidth, lineCenterOpsPart2MinWidth, lineCenterOpsPart3MinWidth, 1 };
            string[] cellsValues = new string[] { elem.Vector.UniqueSourceAddressHex, lineAddressBaseFollower, elem.Vector.InitialValue, elem.Vector.Address, "Bank " + elem.Vector.ApplyOnBankNum + " " + subType, elem.Vector.FullLabel };
            OutputCellAlignment[] cellsAlignments = new OutputCellAlignment[] { OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left, OutputCellAlignment.Left };
            alLines.Add(OutputTools.GetCellsOutput(cellsValues, cellsMinSizes, cellsAlignments));

            alLines.AddRange(getOutputTextIncludedElements(ref elem, elem.Vector.SourceAddressInt, elem.Vector.SourceAddressInt + 1));

            return (string[])alLines.ToArray(typeof(string));
        }

        private void processOutputTextElement(ref TextWriter txWriter, ref Element elem, ref object[] previousResult)
        {
            MergedType prevType = MergedType.Unknown;
            string subType = string.Empty;
            string prevSubType = string.Empty;
            string[] arrLines = null;

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
                    arrLines = getOutputTextElementUnknownOperationLine(ref elem, ref subType);
                    if (subType != prevSubType) txWriter.WriteLine();
                    foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    break;
                case MergedType.UnknownCalibrationLine:
                    arrLines = getOutputTextElementUnknownCalibrationLine(ref elem, ref subType);
                    if (subType != prevSubType) txWriter.WriteLine();
                    foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    break;
                case MergedType.ReservedAddress:
                    arrLines = getOutputTextElementReservedAddress(ref elem, ref subType);
                    if (prevSubType != subType) txWriter.WriteLine();
                    foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    break;
                case MergedType.Operation:
                    if (Calibration.alMainCallsUniqueAddresses.Contains(elem.Operation.UniqueAddress))
                    {
                        Call cCall = (Call)Calibration.slCalls[elem.Operation.UniqueAddress];
                        arrLines = getOutputTextElementLabel(cCall.FullLabel);
                        foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                        if (cCall.S6xRoutine != null)
                        {
                            if (cCall.S6xRoutine.OutputComments)
                            {
                                arrLines = getOutputTextElementHeaderComments(cCall.Comments);
                                foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                            }
                        }
                        cCall = null;
                    }
                    else if (S6x.slProcessRoutines.ContainsKey(elem.Operation.UniqueAddress))
                    {
                        S6xRoutine s6xRoutine = (S6xRoutine)S6x.slProcessRoutines[elem.Operation.UniqueAddress];
                        arrLines = getOutputTextElementLabel((s6xRoutine.Label != string.Empty && s6xRoutine.ShortLabel != string.Empty && s6xRoutine.Label != s6xRoutine.ShortLabel) ? s6xRoutine.ShortLabel + " - " + s6xRoutine.Label : s6xRoutine.Label);
                        foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                        if (s6xRoutine.OutputComments)
                        {
                            arrLines = getOutputTextElementHeaderComments(s6xRoutine.Comments);
                            foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                        }
                        s6xRoutine = null;
                    }
                    else if (prevSubType == "newLine") txWriter.WriteLine();

                    arrLines = getOutputTextElementOperation(ref elem, ref subType);
                    foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    break;
                case MergedType.CalibrationElement:
                    if (elem.CalElement.isScalar)
                    {
                        arrLines = getOutputTextElementScalar(ref elem, ref subType);
                        if (subType != prevSubType) txWriter.WriteLine();
                        foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    }
                    else if (elem.CalElement.isFunction)
                    {
                        arrLines = getOutputTextElementFunction(ref elem, ref subType);
                        foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    }
                    else if (elem.CalElement.isTable)
                    {
                        arrLines = getOutputTextElementTable(ref elem, ref subType);
                        foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    }
                    else if (elem.CalElement.isStructure)
                    {
                        arrLines = getOutputTextElementStructure(ref elem, ref subType);
                        foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    }
                    break;
                case MergedType.ExtScalar:
                    arrLines = getOutputTextElementScalar(ref elem, ref subType);
                    if (subType != prevSubType) txWriter.WriteLine();
                    foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    break;
                case MergedType.ExtFunction:
                    arrLines = getOutputTextElementFunction(ref elem, ref subType);
                    foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    break;
                case MergedType.ExtTable:
                    arrLines = getOutputTextElementTable(ref elem, ref subType);
                    foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    break;
                case MergedType.ExtStructure:
                    arrLines = getOutputTextElementStructure(ref elem, ref subType);
                    foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    break;
                case MergedType.Vector:
                    arrLines = getOutputTextElementVector(ref elem, ref subType);
                    if (subType != prevSubType) txWriter.WriteLine();
                    foreach (string sLine in arrLines) txWriter.WriteLine(sLine);
                    break;

            }

            previousResult = new object[] { elem.Type, subType };
        }
    }
}
