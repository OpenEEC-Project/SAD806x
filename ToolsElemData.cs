using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;

namespace SAD806x
{
    public static class ToolsElemData
    {
        public static bool showElemData(ref DataGridView elemDataGridView, ref SADBin sadBin, object s6xObject, bool dataDecimal, bool ignoreDefinedConversion, bool dataReversed, RepositoryConversionItem rcOutput, RepositoryConversionItem rcInput)
        {
            S6xScalar s6xScalar = null;
            S6xFunction s6xFunction = null;
            S6xTable s6xTable = null;
            S6xStructure s6xStruct = null;
            DataTable dtTable = null;
            Type dataType = null;
            string sValue = string.Empty;
            int iValue = 0;
            int iAddress = 0;
            string[] arrBytes = null;
            string[] arrCols = null;
            object[] arrRows = null;
            object[] arrRowsHeaders = null;
            object[] arrRow = null;
            bool failedScale = false;
            int iBfTop = -1;

            if (elemDataGridView == null) return false;
            if (sadBin == null) return false;
            if (s6xObject == null) return false;

            if (dataDecimal) dataType = typeof(double);
            else dataType = typeof(string);

            elemDataGridView.DataSource = null;

            S6xNavHeaderCategory headerCategory = S6xNavHeaderCategory.UNDEFINED;
            if (s6xObject.GetType() == typeof(S6xScalar)) headerCategory = S6xNavHeaderCategory.SCALARS;
            else if (s6xObject.GetType() == typeof(S6xFunction)) headerCategory = S6xNavHeaderCategory.FUNCTIONS;
            else if (s6xObject.GetType() == typeof(S6xTable)) headerCategory = S6xNavHeaderCategory.TABLES;
            else if (s6xObject.GetType() == typeof(S6xStructure)) headerCategory = S6xNavHeaderCategory.STRUCTURES;

            switch (headerCategory)
            {
                case S6xNavHeaderCategory.SCALARS:
                    s6xScalar = (S6xScalar)s6xObject;

                    elemDataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    s6xScalar.AddressBinInt = Tools.binAddressCorrected(s6xScalar.BankNum, s6xScalar.AddressInt, ref sadBin, s6xScalar.AddressBinInt);

                    if (s6xScalar.isBitFlags)
                    {
                        dataType = typeof(string);
                        arrCols = null;
                        if (s6xScalar.BitFlags != null)
                        {
                            if (s6xScalar.BitFlags.Length > 0)
                            {
                                arrCols = new string[s6xScalar.BitFlags.Length + 1];
                                for (int iCol = 1; iCol < arrCols.Length; iCol++) arrCols[iCol] = s6xScalar.BitFlags[arrCols.Length - iCol - 1].ShortLabel;
                            }
                        }
                        // Default BitFlags
                        if (arrCols == null)
                        {
                            iBfTop = 15;
                            if (s6xScalar.Byte) iBfTop = 7;

                            for (int iBf = iBfTop; iBf >= 0; iBf--)
                            {
                                arrCols = new string[iBfTop + 2];
                                for (int iCol = 1; iCol < arrCols.Length; iCol++) arrCols[iCol] = "B" + iBf.ToString();
                            }
                        }
                        // For better output
                        if (arrCols != null)
                        {
                            for (int iCol = 1; iCol < arrCols.Length; iCol++) arrCols[iCol] = OutputTools.GetSpacesCenteredString(arrCols[iCol], 20);
                        }
                    }
                    else
                    {
                        arrCols = new string[1];
                    }
                    if (s6xScalar.Byte)
                    {
                        arrCols[0] = "Byte";
                        arrBytes = sadBin.getBytesArray(s6xScalar.AddressBinInt, 1);
                        try
                        {
                            sValue = arrBytes[0];
                            iValue = Tools.getByteInt(sValue, s6xScalar.Signed);
                        }
                        catch
                        {
                            sValue = string.Empty;
                            iValue = 0;
                        }
                    }
                    else
                    {
                        arrCols[0] = "Word";
                        arrBytes = sadBin.getBytesArray(s6xScalar.AddressBinInt, 2);
                        try
                        {
                            sValue = Tools.LsbFirst(arrBytes);
                            iValue = Tools.getWordInt(sValue, s6xScalar.Signed);
                        }
                        catch
                        {
                            sValue = string.Empty;
                            iValue = 0;
                        }
                    }
                    // For better output
                    if (arrCols.Length == 1) arrCols[0] = OutputTools.GetSpacesCenteredString(arrCols[0], 100);
                    else arrCols[0] = OutputTools.GetSpacesCenteredString(arrCols[0], 50);

                    arrRow = new object[arrCols.Length];
                    if (s6xScalar.isBitFlags)
                    {
                        BitArray arrBit = new BitArray(new int[] { iValue });
                        for (int iCol = 1; iCol < arrRow.Length; iCol++)
                        {
                            if (arrBit[s6xScalar.BitFlags[arrRow.Length - iCol - 1].Position]) arrRow[iCol] = s6xScalar.BitFlags[arrRow.Length - iCol - 1].SetValue;
                            else arrRow[iCol] = s6xScalar.BitFlags[arrRow.Length - iCol - 1].NotSetValue;
                        }
                    }
                    if (dataDecimal)
                    {
                        if (failedScale) arrRow[0] = iValue;
                        else
                        {
                            try
                            {
                                if (ignoreDefinedConversion)
                                {
                                    if (rcOutput == null) arrRow[0] = iValue;
                                    else arrRow[0] = Tools.ScaleValue(iValue, rcOutput.InternalFormula, 0, true);
                                }
                                else
                                {
                                    if (rcOutput == null) arrRow[0] = Tools.ScaleValue(iValue, s6xScalar.ScaleExpression, s6xScalar.ScalePrecision, true);
                                    else arrRow[0] = Tools.ScaleValue(iValue, rcOutput.InternalFormula.ToUpper().Replace("X", "(" + s6xScalar.ScaleExpression + ")"), s6xScalar.ScalePrecision, true);
                                }
                            }
                            catch
                            {
                                failedScale = true;
                                arrRow[0] = iValue;
                            }
                        }
                    }
                    else
                    {
                        arrRow[0] = sValue.ToUpper();
                    }

                    arrRows = new object[] { arrRow };
                    
                    s6xScalar = null;
                    break;
                case S6xNavHeaderCategory.FUNCTIONS:
                    s6xFunction = (S6xFunction)s6xObject;

                    elemDataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    s6xFunction.AddressBinInt = Tools.binAddressCorrected(s6xFunction.BankNum, s6xFunction.AddressInt, ref sadBin, s6xFunction.AddressBinInt);

                    arrCols = new string[2];
                    arrCols[0] = "Word Input";
                    iValue = 2;
                    if (s6xFunction.ByteInput)
                    {
                        arrCols[0] = "Byte Input";
                        iValue--;
                    }
                    arrCols[1] = "Word Output";
                    iValue += 2;
                    if (s6xFunction.ByteOutput)
                    {
                        arrCols[1] = "Byte Output";
                        iValue--;
                    }
                    arrCols[0] = OutputTools.GetSpacesCenteredString(arrCols[0], 50);   // For better output
                    arrCols[1] = OutputTools.GetSpacesCenteredString(arrCols[1], 50);   // For better output

                    if (s6xFunction.RowsNumber <= 0)
                    {
                        arrRows = new object[] { };
                    }
                    else
                    {
                        arrRows = new object[s6xFunction.RowsNumber];
                        arrBytes = sadBin.getBytesArray(s6xFunction.AddressBinInt, iValue * arrRows.Length);
                        iAddress = 0;
                        for (int iRow = 0; iRow < arrRows.Length; iRow++)
                        {
                            arrRow = new object[arrCols.Length];
                            for (int iCol = 0; iCol < arrRow.Length; iCol++)
                            {
                                if (iCol % 2 == 0)
                                {
                                    if (s6xFunction.ByteInput)
                                    {
                                        try
                                        {
                                            sValue = arrBytes[iAddress];
                                            iValue = Tools.getByteInt(sValue, s6xFunction.SignedInput);
                                        }
                                        catch
                                        {
                                            sValue = string.Empty;
                                            iValue = 0;
                                        }
                                        iAddress++;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            sValue = arrBytes[iAddress + 1] + arrBytes[iAddress];
                                            iValue = Tools.getWordInt(sValue, s6xFunction.SignedInput);
                                        }
                                        catch
                                        {
                                            sValue = string.Empty;
                                            iValue = 0;
                                        }
                                        iAddress += 2;
                                    }
                                    if (dataDecimal)
                                    {
                                        if (failedScale) arrRow[iCol] = iValue;
                                        else
                                        {
                                            try
                                            {
                                                if (ignoreDefinedConversion)
                                                {
                                                    if (rcInput == null) arrRow[iCol] = iValue;
                                                    else arrRow[iCol] = Tools.ScaleValue(iValue, rcInput.InternalFormula, 0, true);
                                                }
                                                else
                                                {
                                                    if (rcInput == null) arrRow[iCol] = Tools.ScaleValue(iValue, s6xFunction.InputScaleExpression, s6xFunction.InputScalePrecision, true);
                                                    else arrRow[iCol] = Tools.ScaleValue(iValue, rcInput.InternalFormula.ToUpper().Replace("X", "(" + s6xFunction.InputScaleExpression + ")"), s6xFunction.InputScalePrecision, true);
                                                }
                                            }
                                            catch
                                            {
                                                failedScale = true;
                                                arrRow[iCol] = iValue;
                                            }
                                        }
                                    }
                                    else arrRow[iCol] = sValue.ToUpper();
                                }
                                else
                                {
                                    if (s6xFunction.ByteOutput)
                                    {
                                        try
                                        {
                                            sValue = arrBytes[iAddress];
                                            iValue = Tools.getByteInt(sValue, s6xFunction.SignedOutput);
                                        }
                                        catch
                                        {
                                            sValue = string.Empty;
                                            iValue = 0;
                                        }
                                        iAddress++;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            sValue = arrBytes[iAddress + 1] + arrBytes[iAddress];
                                            iValue = Tools.getWordInt(sValue, s6xFunction.SignedOutput);
                                        }
                                        catch
                                        {
                                            sValue = string.Empty;
                                            iValue = 0;
                                        }
                                        iAddress += 2;
                                    }
                                    if (dataDecimal)
                                    {
                                        if (failedScale) arrRow[iCol] = iValue;
                                        else
                                        {
                                            try
                                            {
                                                if (ignoreDefinedConversion)
                                                {
                                                    if (rcOutput == null) arrRow[iCol] = iValue;
                                                    else arrRow[iCol] = Tools.ScaleValue(iValue, rcOutput.InternalFormula, 0, true);
                                                }
                                                else
                                                {
                                                    if (rcOutput == null) arrRow[iCol] = Tools.ScaleValue(iValue, s6xFunction.OutputScaleExpression, s6xFunction.OutputScalePrecision, true);
                                                    else arrRow[iCol] = Tools.ScaleValue(iValue, rcOutput.InternalFormula.ToUpper().Replace("X", "(" + s6xFunction.OutputScaleExpression + ")"), s6xFunction.OutputScalePrecision, true);
                                                }
                                            }
                                            catch
                                            {
                                                failedScale = true;
                                                arrRow[iCol] = iValue;
                                            }
                                        }
                                    }
                                    else arrRow[iCol] = sValue.ToUpper();
                                }
                            }
                            arrRows[iRow] = arrRow;
                        }
                    }
                    s6xFunction = null;
                    break;
                case S6xNavHeaderCategory.TABLES:
                    s6xTable = (S6xTable)s6xObject;
                    if (s6xTable.ColsNumber <= 0) return false;

                    elemDataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    s6xTable.AddressBinInt = Tools.binAddressCorrected(s6xTable.BankNum, s6xTable.AddressInt, ref sadBin, s6xTable.AddressBinInt);

                    //arrCols = new string[s6xTable.ColsNumber];
                    //for (int iCol = 0; iCol < arrCols.Length; iCol++) arrCols[iCol] = (iCol + 1).ToString();
                    arrCols = getTableElemDataScale(ref sadBin, s6xTable.ColsScalerAddress, s6xTable.ColsNumber);

                    // For better output
                    if (arrCols != null)
                    {
                        for (int iCol = 0; iCol < arrCols.Length; iCol++) arrCols[iCol] = OutputTools.GetSpacesCenteredString(arrCols[iCol], 10);
                    }

                    if (s6xTable.RowsNumber <= 0)
                    {
                        arrRows = new object[] { };
                    }
                    else
                    {
                        arrRows = new object[s6xTable.RowsNumber];
                        arrRowsHeaders = getTableElemDataScale(ref sadBin, s6xTable.RowsScalerAddress, s6xTable.RowsNumber);
                        if (s6xTable.WordOutput) arrBytes = sadBin.getBytesArray(s6xTable.AddressBinInt, arrCols.Length * arrRows.Length * 2);
                        else arrBytes = sadBin.getBytesArray(s6xTable.AddressBinInt, arrCols.Length * arrRows.Length);
                        iAddress = 0;
                        for (int iRow = 0; iRow < arrRows.Length; iRow++)
                        {
                            arrRow = new object[arrCols.Length];
                            for (int iCol = 0; iCol < arrRow.Length; iCol++)
                            {
                                try
                                {
                                    if (s6xTable.WordOutput)
                                    {
                                        sValue = arrBytes[iAddress + 1] + arrBytes[iAddress];
                                        iValue = Tools.getWordInt(sValue, s6xTable.SignedOutput);
                                    }
                                    else
                                    {
                                        sValue = arrBytes[iAddress];
                                        iValue = Tools.getByteInt(sValue, s6xTable.SignedOutput);
                                    }
                                }
                                catch
                                {
                                    sValue = string.Empty;
                                    iValue = 0;
                                }
                                iAddress++;
                                if (s6xTable.WordOutput) iAddress++;

                                if (dataDecimal)
                                {
                                    if (failedScale) arrRow[iCol] = iValue;
                                    else
                                    {
                                        try
                                        {
                                            if (ignoreDefinedConversion)
                                            {
                                                if (rcOutput == null) arrRow[iCol] = iValue;
                                                else arrRow[iCol] = Tools.ScaleValue(iValue, rcOutput.InternalFormula, 0, true);
                                            }
                                            else
                                            {
                                                if (rcOutput == null) arrRow[iCol] = Tools.ScaleValue(iValue, s6xTable.CellsScaleExpression, s6xTable.CellsScalePrecision, true);
                                                else arrRow[iCol] = Tools.ScaleValue(iValue, rcOutput.InternalFormula.ToUpper().Replace("X", "(" + s6xTable.CellsScaleExpression + ")"), s6xTable.CellsScalePrecision, true);
                                            }
                                        }
                                        catch
                                        {
                                            failedScale = true;
                                            arrRow[iCol] = iValue;
                                        }
                                    }
                                }
                                else arrRow[iCol] = sValue.ToUpper();
                            }
                            arrRows[iRow] = arrRow;
                        }
                    }
                    s6xTable = null;
                    break;
                case S6xNavHeaderCategory.STRUCTURES:
                    s6xStruct = (S6xStructure)s6xObject;
                    if (s6xStruct.Number <= 0) return false;

                    s6xStruct.Structure = new Structure(s6xStruct);
                    if (!s6xStruct.Structure.isValid) return false;
                    if (s6xStruct.Structure.isEmpty) return false;
                    
                    elemDataGridView.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    s6xStruct.Structure.AddressBinInt = Tools.binAddressCorrected(s6xStruct.Structure.BankNum, s6xStruct.Structure.AddressInt, ref sadBin, s6xStruct.Structure.AddressBinInt);

                    arrBytes = sadBin.getBytesArray(s6xStruct.Structure.AddressBinInt, s6xStruct.Structure.MaxSizeSingle * s6xStruct.Number);
                    s6xStruct.Structure.Read(ref arrBytes, s6xStruct.Number);
                    arrBytes = null;

                    arrCols = new string[s6xStruct.Structure.MaxLineItemsNum];
                    for (int iCol = 0; iCol < arrCols.Length; iCol++) arrCols[iCol] = (iCol + 1).ToString();

                    // For better output
                    for (int iCol = 0; iCol < arrCols.Length; iCol++) arrCols[iCol] = OutputTools.GetSpacesCenteredString(arrCols[iCol], 10);

                    dataType = typeof(string);

                    if (s6xStruct.Structure.Lines.Count <= 0)
                    {
                        arrRows = new object[] { };
                    }
                    else
                    {
                        arrRows = new object[s6xStruct.Structure.Lines.Count];
                        arrRowsHeaders = new object[s6xStruct.Structure.Lines.Count];
                        int iRow = 0;
                        foreach (StructureLine structLine in s6xStruct.Structure.Lines)
                        {
                            arrRowsHeaders[iRow] = structLine.NumberInStructure.ToString();
                            arrRow = new object[arrCols.Length];
                            for (int iCol = 0; iCol < structLine.Items.Count; iCol++) arrRow[iCol] = ((StructureItem)structLine.Items[iCol]).Value(structLine.NumberInStructure);
                            arrRows[iRow] = arrRow;
                            iRow++;
                        }
                    }
                    s6xStruct = null;
                    break;
                default:
                    return false;
            }

            if (arrCols == null) return false;

            if (dataReversed)
            {
                object[] arrReversedRows = new object[arrRows.Length];
                for (int iRow = 0; iRow < arrReversedRows.Length; iRow++) arrReversedRows[arrReversedRows.Length - 1 - iRow] = arrRows[iRow];
                arrRows = arrReversedRows;
                arrReversedRows = null;

                if (arrRowsHeaders != null)
                {
                    arrReversedRows = new object[arrRowsHeaders.Length];
                    for (int iRow = 0; iRow < arrReversedRows.Length; iRow++) arrReversedRows[arrReversedRows.Length - 1 - iRow] = arrRowsHeaders[iRow];
                    arrRowsHeaders = arrReversedRows;
                    arrReversedRows = null;
                }
            }

            dtTable = new DataTable();
            //foreach (string colLabel in arrCols) dtTable.Columns.Add(new DataColumn(colLabel, dataType));
            for (int iCol = 0; iCol < arrCols.Length; iCol++)
            {
                DataColumn dcDC = new DataColumn(iCol.ToString(), dataType);
                dcDC.Caption = arrCols[iCol];
                dtTable.Columns.Add(dcDC);
            }

            foreach (object[] oRow in arrRows) dtTable.Rows.Add(oRow);
            arrRows = null;

            elemDataGridView.Tag = new object[] { arrCols, arrRowsHeaders };

            // For Speed purpose
            elemDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            elemDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            elemDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            try { elemDataGridView.DataSource = dtTable; }
            catch { return false; }

            // For Speed purpose
            switch (headerCategory)
            {
                case S6xNavHeaderCategory.SCALARS:
                case S6xNavHeaderCategory.FUNCTIONS:
                    elemDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    break;
                case S6xNavHeaderCategory.TABLES:
                case S6xNavHeaderCategory.STRUCTURES:
                    //elemDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    elemDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    break;
            }

            elemDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            elemDataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;

            dtTable = null;
            arrCols = null;
            arrRowsHeaders = null;

            elemDataGridView.Visible = true;

            return true;
        }

        // To be able to show scaled data for tables
        public static string[] getTableElemDataScale(ref SADBin sadBin, string scalerUniqueAddress, int stepNumber)
        {
            string[] scaleResult = null;
            S6xFunction s6xScaler = null;

            if (sadBin == null) return new string[] { };
            if (stepNumber <= 0) return new string[] { };
            scaleResult = new string[stepNumber];

            for (int iStep = 0; iStep < stepNumber; iStep++) scaleResult[iStep] = (iStep + 1).ToString();

            if (scalerUniqueAddress != null && scalerUniqueAddress != string.Empty) s6xScaler = (S6xFunction)sadBin.S6x.slFunctions[scalerUniqueAddress];
            if (s6xScaler == null && scalerUniqueAddress != null && scalerUniqueAddress != string.Empty) s6xScaler = (S6xFunction)sadBin.S6x.slDupFunctions[scalerUniqueAddress];
            if (s6xScaler != null) if (s6xScaler.RowsNumber <= 0) s6xScaler = null;
            if (s6xScaler == null) return scaleResult;

            s6xScaler.AddressBinInt = Tools.binAddressCorrected(s6xScaler.BankNum, s6xScaler.AddressInt, ref sadBin, s6xScaler.AddressBinInt);

            int iRowSize = 4;
            if (s6xScaler.ByteInput) iRowSize--;
            if (s6xScaler.ByteOutput) iRowSize--;

            object[] arrRows = new object[s6xScaler.RowsNumber];
            string[] arrBytes = sadBin.getBytesArray(s6xScaler.AddressBinInt, iRowSize * s6xScaler.RowsNumber);
            bool failedInputScale = false;
            bool failedOutputScale = false;
            int iAddress = 0;
            for (int iRow = 0; iRow < arrRows.Length; iRow++)
            {
                object[] arrRow = new object[2];
                int iValue = 0;

                if (s6xScaler.ByteInput)
                {
                    try { iValue = Tools.getByteInt(arrBytes[iAddress], s6xScaler.SignedInput); }
                    catch { iValue = 0; }
                    iAddress++;
                }
                else
                {
                    try { iValue = Tools.getWordInt(arrBytes[iAddress + 1] + arrBytes[iAddress], s6xScaler.SignedInput); }
                    catch { iValue = 0; }
                    iAddress += 2;
                }
                if (failedInputScale) arrRow[0] = iValue;
                else
                {
                    try { arrRow[0] = Tools.ScaleValue(iValue, s6xScaler.InputScaleExpression, true); }
                    catch
                    {
                        failedInputScale = true;
                        arrRow[0] = iValue;
                    }
                }

                if (s6xScaler.ByteOutput)
                {
                    try { iValue = Tools.getByteInt(arrBytes[iAddress], s6xScaler.SignedOutput); }
                    catch { iValue = 0; }
                    iAddress++;
                }
                else
                {
                    try { iValue = Tools.getWordInt(arrBytes[iAddress + 1] + arrBytes[iAddress], s6xScaler.SignedOutput); }
                    catch { iValue = 0; }
                    iAddress += 2;
                }
                if (failedOutputScale) arrRow[1] = iValue;
                else
                {
                    try { arrRow[1] = Tools.ScaleValue(iValue, s6xScaler.OutputScaleExpression, true); }
                    catch
                    {
                        failedOutputScale = true;
                        arrRow[1] = iValue;
                    }
                }
                arrRows[iRow] = arrRow;
            }

            if (failedOutputScale) return scaleResult;
            double[] scaleValues = new double[stepNumber];

            for (int iStep = 0; iStep < stepNumber; iStep++)
            {
                scaleResult[iStep] = string.Empty;
                scaleValues[iStep] = 0.0;
            }
            double dMaxIndex = 0.0;
            double dMinIndex = double.MaxValue;
            double dMaxValue = 0.0;
            double dMinValue = 0.0;
            for (int iRow = 0; iRow < arrRows.Length; iRow++)
            {
                double dSc = (double)((object[])arrRows[iRow])[1];
                if (dSc >= dMaxIndex)
                {
                    dMaxIndex = dSc;
                    dMaxValue = (double)((object[])arrRows[iRow])[0];
                }
                if (dSc <= dMinIndex)
                {
                    dMinIndex = dSc;
                    dMinValue = (double)((object[])arrRows[iRow])[0];
                }
                if (dSc == (int)dSc && dSc >= 0 && dSc < stepNumber)
                {
                    scaleValues[(int)dSc] = (double)((object[])arrRows[iRow])[0];
                    scaleResult[(int)dSc] = string.Format("{0:G}", scaleValues[(int)dSc]);
                }
                if (dSc <= 0.0) break;
            }
            if (scaleResult[0] == string.Empty)
            {
                if (dMinIndex > (double)(0) && dMinIndex < (double)(0 + 1))
                {
                    scaleValues[0] = dMinValue;
                    scaleResult[0] = string.Format("{0:G}", scaleValues[0]);
                }
            }
            if (scaleResult[scaleResult.Length - 1] == string.Empty)
            {
                if (dMaxIndex > (double)(scaleResult.Length - 1 - 1) && dMinIndex < (double)(scaleResult.Length - 1 + 1))
                {
                    scaleValues[scaleResult.Length - 1] = dMaxValue;
                    scaleResult[scaleResult.Length - 1] = string.Format("{0:G}", scaleValues[scaleResult.Length - 1]);
                }
                else if (dMaxIndex > 0.0 && dMaxIndex > dMinIndex)
                {
                    for (int iStep = (int)dMaxIndex; iStep < stepNumber; iStep++)
                    {
                        scaleValues[scaleResult.Length - 1] = dMaxValue;
                        scaleResult[iStep] = string.Format("{0:G}", scaleValues[scaleResult.Length - 1]);
                    }
                }
            }
            if (scaleResult[0] == string.Empty || scaleResult[scaleResult.Length - 1] == string.Empty)
            {
                for (int iStep = 0; iStep < stepNumber; iStep++) scaleResult[iStep] = (iStep + 1).ToString();
                return scaleResult;
            }

            int lastStepWithValue = -1;
            for (int iStep = 0; iStep < stepNumber; iStep++)
            {
                if (scaleResult[iStep] == string.Empty) continue;
                if (lastStepWithValue >= 0 && iStep - lastStepWithValue > 1)
                {
                    double stepGap = (scaleValues[iStep] - scaleValues[lastStepWithValue]) / (iStep - lastStepWithValue);
                    for (int iStepUpdate = lastStepWithValue + 1; iStepUpdate < iStep; iStepUpdate++) scaleValues[iStepUpdate] = scaleValues[iStepUpdate - 1] + stepGap;
                }
                lastStepWithValue = iStep;
            }

            // Final Format
            bool pureIntFormat = true;
            bool lowNumberFormat = true;
            for (int iStep = 0; iStep < stepNumber; iStep++)
            {
                if (scaleValues[iStep] != (int)iStep) pureIntFormat = false;
                // Thinking about Transfer function on 5v12
                if (scaleValues[iStep] > 6.0) lowNumberFormat = false;
            }
            string sFormat = "{0:0}";
            if (pureIntFormat || !lowNumberFormat) sFormat = "{0:0}";
            // New Scale Precision field
            if (s6xScaler.InputScalePrecision >= 0 && s6xScaler.InputScalePrecision <= 8) sFormat = "{0:0." + new string('0', s6xScaler.InputScalePrecision) + "}";
            for (int iStep = 0; iStep < stepNumber; iStep++) scaleResult[iStep] = string.Format(sFormat, scaleValues[iStep]);

            return scaleResult;
        }
    }
}
