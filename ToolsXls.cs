using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Reflection;

namespace SAD806x
{
    public static class ToolsXls
    {
        private struct XlsDataTypes
        {
            public const string NUMBER = "NUMBER";
            public const string DATETIME = "DATETIME";
            public const string TEXT = "TEXT";
        }

        private struct NetDataTypes
        {
            public const string SHORT = "int16";
            public const string INT = "int32";
            public const string LONG = "int64";
            public const string STRING = "string";
            public const string DATE = "DateTime";
            public const string BOOL = "Boolean";
            public const string DECIMAL = "decimal";
            public const string DOUBLE = "double";
            public const string FLOAT = "float";
        }
        
        private static string xlsConnectionString(string xlsPath)
        {
            return "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + xlsPath + ";Extended Properties=\"Excel 8.0;HDR=No;IMEX=1\"";
        }

        private static string xlsAltConnectionString(string xlsPath)
        {
            return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + xlsPath + ";Extended Properties=\"Excel 8.0;HDR=No;IMEX=1\"";
        }

        private static string xlsxConnectionString(string xlsPath)
        {
            return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + xlsPath + ";Extended Properties=\"Excel 12.0;HDR=No;IMEX=1\"";
        }

        private static string xlsxWriteConnectionString(string xlsPath)
        {
            return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + xlsPath + ";Mode=ReadWrite;Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=0\"";
        }

        public static string CheckXls(string xlsPath, bool forceXlsAlt)
        {
            string connectionString = string.Empty;

            switch (Path.GetExtension(xlsPath).ToUpper())
            {
                case ".XLSX":
                    connectionString = xlsxConnectionString(xlsPath);
                    break;
                default:
                    if (forceXlsAlt) connectionString = xlsAltConnectionString(xlsPath);
                    else connectionString = xlsConnectionString(xlsPath);
                    break;
            }

            try
            {
                using (OleDbConnection conConnection = new OleDbConnection(connectionString))
                {
                    conConnection.Open();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "OK";
        }

        public static SortedList<string, DataTable> ReadXls(string xlsPath, bool forceXlsAlt)
        {
            string connectionString = string.Empty;
            SortedList<string, DataTable> slResult = null;

            switch (Path.GetExtension(xlsPath).ToUpper())
            {
                case ".XLSX":
                    connectionString = xlsxConnectionString(xlsPath);
                    break;
                default:
                    if (forceXlsAlt) connectionString = xlsAltConnectionString(xlsPath);
                    else connectionString = xlsConnectionString(xlsPath);
                    break;
            }

            using (OleDbConnection conConnection = new OleDbConnection(connectionString))
            {
                conConnection.Open();

                List<string> lstWorkSheets = new List<string>();
                foreach (DataRow drDataRow in conConnection.GetSchema("Tables").Rows)
                {
                    if (drDataRow["TABLE_NAME"].ToString().EndsWith("$'")) lstWorkSheets.Add(drDataRow["TABLE_NAME"].ToString().Substring(1, drDataRow["TABLE_NAME"].ToString().Length - 3));
                    if (drDataRow["TABLE_NAME"].ToString().EndsWith("$")) lstWorkSheets.Add(drDataRow["TABLE_NAME"].ToString().Substring(0, drDataRow["TABLE_NAME"].ToString().Length - 1));
                }

                slResult = new SortedList<string, DataTable>();

                foreach (string workSheetName in lstWorkSheets)
                {
                    if (slResult.ContainsKey(workSheetName)) continue;

                    using (OleDbCommand comCommand = new OleDbCommand("SELECT * FROM [" + workSheetName + "$]", conConnection))
                    {
                        using (OleDbDataAdapter daDataAdapter = new OleDbDataAdapter(comCommand))
                        {
                            using (DataTable dtDataTable = new DataTable())
                            {
                                daDataAdapter.Fill(dtDataTable);

                                slResult.Add(workSheetName, dtDataTable);
                            }
                        }
                    }
                }
            }

            return slResult;
        }

        private static string getWorkSheetName(ref DataTable dataTable)
        {
            return dataTable.TableName;
        }
        
        private static Dictionary<string, string> getXlsDataTypes()
        {
            Dictionary<string, string> dataTypeLookUp = new Dictionary<string, string>();

            // I cannot get the Excel formatting correct here!?
            dataTypeLookUp.Add(NetDataTypes.SHORT, XlsDataTypes.NUMBER);
            dataTypeLookUp.Add(NetDataTypes.INT, XlsDataTypes.NUMBER);
            dataTypeLookUp.Add(NetDataTypes.LONG, XlsDataTypes.NUMBER);
            dataTypeLookUp.Add(NetDataTypes.STRING, XlsDataTypes.TEXT);
            dataTypeLookUp.Add(NetDataTypes.DATE, XlsDataTypes.DATETIME);
            dataTypeLookUp.Add(NetDataTypes.BOOL, XlsDataTypes.TEXT);
            dataTypeLookUp.Add(NetDataTypes.DECIMAL, XlsDataTypes.NUMBER);
            dataTypeLookUp.Add(NetDataTypes.DOUBLE, XlsDataTypes.NUMBER);
            dataTypeLookUp.Add(NetDataTypes.FLOAT, XlsDataTypes.NUMBER);
            return dataTypeLookUp;
        }
        
        private static string getCreateTableCommand(ref DataTable dataTable)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> dataTypeList = getXlsDataTypes();

            if (dataTable.Columns.Count <= 0) return null;

            sb.AppendFormat("CREATE TABLE [{0}] (", getWorkSheetName(ref dataTable));

            foreach (DataColumn col in dataTable.Columns)
            {
                string type = XlsDataTypes.TEXT;
                if (dataTypeList.ContainsKey(col.DataType.Name.ToString().ToLower()))
                {
                    type = dataTypeList[col.DataType.Name.ToString().ToLower()];
                }
                sb.AppendFormat("[{0}] {1},", col.Caption, type);
            }
            sb = sb.Replace(',', ')', sb.ToString().LastIndexOf(','), 1);
            return sb.ToString();
        }

        private static string getInsertCommand(ref DataTable dataTable, int rowIndex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("INSERT INTO [{0}$](", getWorkSheetName(ref dataTable));
            foreach (DataColumn dcDC in dataTable.Columns)
            {
                if (dcDC.Caption.Contains(" ")) continue;
                sb.AppendFormat("[{0}],", dcDC.Caption);
            }
            sb = sb.Replace(',', ')', sb.ToString().LastIndexOf(','), 1);

            sb.Append("VALUES (");
            foreach (DataColumn dcDC in dataTable.Columns)
            {
                if (dcDC.Caption.Contains(" ")) continue;
                string type = dcDC.DataType.ToString();
                string strToInsert = String.Empty;
                strToInsert = dataTable.Rows[rowIndex][dcDC].ToString().Replace("'", "''");
                sb.AppendFormat("'{0}',", strToInsert);
            }
            sb = sb.Replace(',', ')', sb.ToString().LastIndexOf(','), 1);
            return sb.ToString();
        }

        private static string getInsertRowIdOnlyCommand(ref DataTable dataTable, string rowIdColumnName, int rowIndex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("INSERT INTO [{0}$] ([{1}]) VALUES ('{2}');", getWorkSheetName(ref dataTable), rowIdColumnName, string.Format("S6XUID_{0:6}", rowIndex));
            return sb.ToString();
        }

        private static string getUpdateByRowIdCommand(ref DataTable dataTable, string rowIdColumnName, int rowIndex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("UPDATE [{0}$] SET ", getWorkSheetName(ref dataTable));
            foreach (DataColumn dcDC in dataTable.Columns)
            {
                string type = dcDC.DataType.ToString();
                string strToInsert = String.Empty;
                strToInsert = dataTable.Rows[rowIndex][dcDC].ToString().Replace("'", "''");
                sb.AppendFormat("[{0}]='{1}',", dcDC.Caption, strToInsert);
            }
            sb = sb.Replace(",", "", sb.ToString().LastIndexOf(','), 1);
            sb.AppendFormat(" WHERE [{0}] = '{1}';", rowIdColumnName, string.Format("S6XUID_{0:6}", rowIndex));
            return sb.ToString();
        }

        private static string getTemplateSheetClearCommand(ref DataTable dataTable)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("UPDATE [{0}$] SET ", getWorkSheetName(ref dataTable));
            string firstColumnCaption = string.Empty;
            foreach (DataColumn dcDC in dataTable.Columns)
            {
                if (firstColumnCaption == string.Empty) firstColumnCaption = dcDC.Caption;
                sb.AppendFormat("[{0}]='',", dcDC.Caption);
            }
            sb = sb.Replace(",", "", sb.ToString().LastIndexOf(','), 1);
            sb.AppendFormat(" WHERE [{0}] <> '';", firstColumnCaption);
            return sb.ToString();
        }

        private static string getTemplateSheetUpdateCommand(ref DataTable dataTable, int rowIndex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("UPDATE [{0}$] SET ", getWorkSheetName(ref dataTable));
            string firstColumnCaption = string.Empty;
            foreach (DataColumn dcDC in dataTable.Columns)
            {
                if (firstColumnCaption == string.Empty) firstColumnCaption = dcDC.Caption;
                string type = dcDC.DataType.ToString();
                string strToInsert = String.Empty;
                strToInsert = dataTable.Rows[rowIndex][dcDC].ToString().Replace("'", "''");
                sb.AppendFormat("[{0}]='{1}',", dcDC.Caption, strToInsert);
            }
            sb = sb.Replace(",", "", sb.ToString().LastIndexOf(','), 1);
            sb.AppendFormat(" WHERE [{0}] <> '';", firstColumnCaption);
            return sb.ToString();
        }

        public static bool WriteXls(string xlsPath, ref SortedList<string, DataTable> slWorkSheets, ref List<string> lstErrors)
        {
            if (lstErrors == null) lstErrors = new List<string>(); 

            // SAD806xWorkBookXlsx.xslx is the default template to overwrite existing file (which has now a backup)
            // First Row is the column name, second row is set with 'abc' everywhere to set values as text, except for long text, which is at more the 256 chars
            // First update should also overwrite this row
            try { File.WriteAllBytes(xlsPath, Properties.Resources.SAD806xWorkBookXlsx); }
            catch
            {
                lstErrors.Add("Writing template file has failed. File is opened, protected or its directory is protected.");
                return false;
            }

            string connectionString = xlsxWriteConnectionString(xlsPath);

            using (OleDbConnection conConnection = new OleDbConnection(connectionString))
            {
                try { conConnection.Open(); }
                catch
                {
                    lstErrors.Add("Accessing the template file has failed. Access method (ACE OleDb) seems not managed.");
                    return false;
                }

                List<string> lsExistingWorkSheets = new List<string>();
                foreach (DataRow drDataRow in conConnection.GetSchema("Tables").Rows)
                {
                    if (drDataRow["TABLE_NAME"].ToString().EndsWith("$'")) lsExistingWorkSheets.Add(drDataRow["TABLE_NAME"].ToString().Substring(1, drDataRow["TABLE_NAME"].ToString().Length - 3));
                    if (drDataRow["TABLE_NAME"].ToString().EndsWith("$")) lsExistingWorkSheets.Add(drDataRow["TABLE_NAME"].ToString().Substring(0, drDataRow["TABLE_NAME"].ToString().Length - 1));
                }
                
                if (slWorkSheets != null)
                {
                    foreach (string workSheetKey in slWorkSheets.Keys)
                    {
                        DataTable dtDT = slWorkSheets[workSheetKey];
                        bool templateSheet = true;
                        bool hasColumnsWithoutSpaces = false;
                        bool hasColumnsWithSpaces = false;
                        string firstColumnWithoutSpaces = string.Empty;

                        foreach (DataColumn dcDC in dtDT.Columns)
                        {
                            if (dcDC.Caption.Contains(" ")) hasColumnsWithSpaces = true;
                            else
                            {
                                hasColumnsWithoutSpaces = true;
                                if (firstColumnWithoutSpaces == string.Empty) firstColumnWithoutSpaces = dcDC.Caption;
                            }
                            if (hasColumnsWithoutSpaces && hasColumnsWithSpaces) break;
                        }

                        if (!hasColumnsWithoutSpaces)
                        {
                            lstErrors.Add("Columns on worksheet " + dtDT.TableName + " can't be managed. Worksheet will be ignored.");
                            continue;
                        }

                        templateSheet = lsExistingWorkSheets.Contains(dtDT.TableName);
                        if (templateSheet)
                        {
                            if (dtDT.Rows.Count == 0)
                            {
                                using (OleDbCommand comCommand = new OleDbCommand(getTemplateSheetClearCommand(ref dtDT), conConnection))
                                {
                                    try { comCommand.ExecuteNonQuery(); }
                                    catch
                                    {
                                        lstErrors.Add("Cleaning template worksheet " + dtDT.TableName + " has failed. Some rows from the template will stay in place.");
                                    }
                                }
                            }
                        }
                        else
                        {
                            string createTableCommand = getCreateTableCommand(ref dtDT);
                            if (createTableCommand == null || createTableCommand == string.Empty)
                            {
                                lstErrors.Add("Creating worksheet " + dtDT.TableName + " has failed. Worksheet will be ignored.");
                                continue;
                            }

                            using (OleDbCommand comCommand = new OleDbCommand(createTableCommand, conConnection))
                            {
                                try { comCommand.ExecuteNonQuery(); }
                                catch
                                {
                                    lstErrors.Add("Creating worksheet " + dtDT.TableName + " has failed. Worksheet will be ignored.");
                                    continue;
                                }
                            }
                        }

                        bool workSheetInsertUpdateError = false;
                        for (int iRow = 0; iRow < dtDT.Rows.Count; iRow++)
                        {
                            if (templateSheet && iRow == 0)
                            {
                                using (OleDbCommand comCommand = new OleDbCommand(getTemplateSheetUpdateCommand(ref dtDT, iRow), conConnection))
                                {
                                    try { comCommand.ExecuteNonQuery(); }
                                    catch
                                    {
                                        lstErrors.Add("Updating at least a row on worksheet " + dtDT.TableName + " has failed.");
                                        workSheetInsertUpdateError = true;
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                if (hasColumnsWithSpaces)
                                {
                                    // Insert with RowId on first column withtout spaces
                                    using (OleDbCommand comCommand = new OleDbCommand(getInsertRowIdOnlyCommand(ref dtDT, firstColumnWithoutSpaces, iRow), conConnection))
                                    {
                                        try { comCommand.ExecuteNonQuery(); }
                                        catch
                                        {
                                            if (!workSheetInsertUpdateError)
                                            {
                                                lstErrors.Add("Adding at least a row on worksheet " + dtDT.TableName + " has failed.");
                                                workSheetInsertUpdateError = true;
                                            }
                                            continue;
                                        }
                                    }

                                    // Update for the whole row
                                    using (OleDbCommand comCommand = new OleDbCommand(getUpdateByRowIdCommand(ref dtDT, firstColumnWithoutSpaces, iRow), conConnection))
                                    {
                                        try { comCommand.ExecuteNonQuery(); }
                                        catch
                                        {
                                            if (!workSheetInsertUpdateError)
                                            {
                                                lstErrors.Add("Updating at least a row on worksheet " + dtDT.TableName + " has failed.");
                                                workSheetInsertUpdateError = true;
                                            }
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    // Classical Insert
                                    using (OleDbCommand comCommand = new OleDbCommand(getInsertCommand(ref dtDT, iRow), conConnection))
                                    {
                                        try { comCommand.ExecuteNonQuery(); }
                                        catch
                                        {
                                            if (!workSheetInsertUpdateError)
                                            {
                                                lstErrors.Add("Adding at least a row on worksheet " + dtDT.TableName + " has failed.");
                                                workSheetInsertUpdateError = true;
                                            }
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public static bool ReadXlsFile(ref XlsFile xlsFile, string xlsFilePath, bool forceXlsAlt, bool requireGlobalValidity)
        {
            SortedList<string, DataTable> slWorkSheetsData = new SortedList<string,DataTable>();
            bool globalValidity = true;

            try { slWorkSheetsData = ToolsXls.ReadXls(xlsFilePath, forceXlsAlt); }
            catch { return false; }

            int requiredWorkSheets = 0;
            foreach (string workSheetName in slWorkSheetsData.Keys)
            {
                switch (workSheetName.ToUpper())
                {
                    case "LEVELS":
                    case "PAYLOAD":
                    case "FUNCTIONS":
                    case "TABLES":
                    case "SCALARS":
                        requiredWorkSheets++;
                        break;
                }
            }
            if (requiredWorkSheets < 5) return false;

            xlsFile.Revisions = new List<XlsRevision>();
            xlsFile.ConfigParameters = new List<XlsConfigParameter>();
            xlsFile.Levels = new SortedList<string, XlsLevel>();
            xlsFile.QHorses = new List<XlsQHorse>();
            xlsFile.TwEECers = new List<XlsTwEECer>();
            xlsFile.PayLoads = new SortedList<int, XlsPayLoad>();
            xlsFile.Functions = new SortedList<string, XlsFunction>();
            xlsFile.Tables = new SortedList<string, XlsTable>();
            xlsFile.Scalars = new SortedList<string, XlsScalar>();
            
            SortedList<string, string> slOrderedWorkSheetNames = new SortedList<string, string>();
            foreach (string workSheetName in slWorkSheetsData.Keys)
            {
                switch (workSheetName.ToUpper())
                {
                    case "LEVELS":
                        slOrderedWorkSheetNames.Add("0." + workSheetName, workSheetName);
                        break;
                    case "PAYLOAD":
                        slOrderedWorkSheetNames.Add("1." + workSheetName, workSheetName);
                        break;
                    case "FUNCTIONS":
                        slOrderedWorkSheetNames.Add("2." + workSheetName, workSheetName);
                        break;
                    default:
                        slOrderedWorkSheetNames.Add("9." + workSheetName, workSheetName);
                        break;
                }
            }
            foreach (string workSheetName in slOrderedWorkSheetNames.Values)
            {
                DataTable dtDT = slWorkSheetsData[workSheetName];
                if (dtDT.Rows.Count == 0) continue;

                Type objectType = null;
                
                // Column Object[] is Ordinal, Column Name (Upper Case), SADXls object related property name
                SortedList<string, object[]> slColumns = new SortedList<string, object[]>();
                foreach (DataColumn dcDC in dtDT.Columns)
                {
                    if (dtDT.Rows[0][dcDC.Ordinal].ToString().Trim() == string.Empty) continue;
                    if (slColumns.ContainsKey(dtDT.Rows[0][dcDC.Ordinal].ToString().Trim().ToUpper())) continue;
                    slColumns.Add(dtDT.Rows[0][dcDC.Ordinal].ToString().Trim().ToUpper(), new object[] { dcDC.Ordinal, dtDT.Rows[0][dcDC.Ordinal].ToString().Trim().ToUpper(), string.Empty});
                }

                // Required Column Object[] is SADXls object related property name, Required flag
                SortedList<string, object[]> slRequiredColumnsProperties = new SortedList<string, object[]>();
                switch (workSheetName.ToUpper())
                {
                    case "REVISION":
                        objectType = typeof(XlsRevision);
                        slRequiredColumnsProperties.Add("REVISION #", new object[] { "Revision", true });
                        slRequiredColumnsProperties.Add("COMMENTS", new object[] { "Comments", true });
                        break;
                    case "CONFIG":
                        objectType = typeof(XlsConfigParameter);
                        slRequiredColumnsProperties.Add("PARAMETER", new object[] { "Parameter", true });
                        slRequiredColumnsProperties.Add("VALUE", new object[] { "Value", true });
                        break;
                    case "LEVELS":
                        objectType = typeof(XlsLevel);
                        slRequiredColumnsProperties.Add("LEVEL", new object[] { "Label", true });
                        slRequiredColumnsProperties.Add("VALUE", new object[] { "Value", true });
                        break;
                    case "QHORSE":
                        objectType = typeof(XlsQHorse);
                        slRequiredColumnsProperties.Add("ADDRESS", new object[] { "Address", false });
                        slRequiredColumnsProperties.Add("DATA", new object[] { "Data", false });
                        slRequiredColumnsProperties.Add("PARAMETER", new object[] { "Parameter", false });
                        slRequiredColumnsProperties.Add("VALUE", new object[] { "Value", false });
                        break;
                    case "TWEECER":
                        objectType = typeof(XlsTwEECer);
                        slRequiredColumnsProperties.Add("ADDRESS", new object[] { "Address", false });
                        slRequiredColumnsProperties.Add("DATA", new object[] { "Data", false });
                        slRequiredColumnsProperties.Add("PARAMETER", new object[] { "Parameter", false });
                        slRequiredColumnsProperties.Add("VALUE", new object[] { "Value", false });
                        break;
                    case "PAYLOAD":
                        objectType = typeof(XlsPayLoad);
                        slRequiredColumnsProperties.Add("TAG", new object[] { "Label", true });
                        slRequiredColumnsProperties.Add("DESCRIPTION", new object[] { "Description", true });
                        slRequiredColumnsProperties.Add("COMMENTS", new object[] { "Comments", false });
                        slRequiredColumnsProperties.Add("ADDRESS", new object[] { "AddressInt", true });
                        slRequiredColumnsProperties.Add("BYTES", new object[] { "Bytes", true });
                        slRequiredColumnsProperties.Add("SIGNED", new object[] { "Signed", true });
                        slRequiredColumnsProperties.Add("EQUATION", new object[] { "Equation", true });
                        slRequiredColumnsProperties.Add("DIGITS", new object[] { "Digits", true });
                        slRequiredColumnsProperties.Add("UNITS", new object[] { "Units", false });
                        slRequiredColumnsProperties.Add("BIT0", new object[] { "BitsArray", false });
                        slRequiredColumnsProperties.Add("BIT1", new object[] { "BitsArray", false });
                        slRequiredColumnsProperties.Add("BIT2", new object[] { "BitsArray", false });
                        slRequiredColumnsProperties.Add("BIT3", new object[] { "BitsArray", false });
                        slRequiredColumnsProperties.Add("BIT4", new object[] { "BitsArray", false });
                        slRequiredColumnsProperties.Add("BIT5", new object[] { "BitsArray", false });
                        slRequiredColumnsProperties.Add("BIT6", new object[] { "BitsArray", false });
                        slRequiredColumnsProperties.Add("BIT7", new object[] { "BitsArray", false });
                        break;
                    case "FUNCTIONS":
                        objectType = typeof(XlsFunction);
                        slRequiredColumnsProperties.Add("LEVEL", new object[] { "Level", false });
                        slRequiredColumnsProperties.Add("LEVEL2", new object[] { "Level2", false });
                        slRequiredColumnsProperties.Add("KEY", new object[] { "Key", false });
                        slRequiredColumnsProperties.Add("PID", new object[] { "PID", true });
                        slRequiredColumnsProperties.Add("TITLE", new object[] { "Title", true });
                        slRequiredColumnsProperties.Add("COMMENTS", new object[] { "Comments", true });
                        slRequiredColumnsProperties.Add("ROWS", new object[] { "Rows", true });
                        slRequiredColumnsProperties.Add("BYTES", new object[] { "Bytes", true });
                        slRequiredColumnsProperties.Add("YSIGNED", new object[] { "YSigned", true });
                        slRequiredColumnsProperties.Add("YDIGITS", new object[] { "YDigits", true });
                        slRequiredColumnsProperties.Add("YEQUATION", new object[] { "YEquation", true });
                        slRequiredColumnsProperties.Add("YUNITS", new object[] { "YUnits", false });
                        slRequiredColumnsProperties.Add("YMIN", new object[] { "YMin", false });
                        slRequiredColumnsProperties.Add("YMAX", new object[] { "YMax", false });
                        slRequiredColumnsProperties.Add("YPAYLOADTAG", new object[] { "YPayLoad", false });
                        slRequiredColumnsProperties.Add("XADDRESS", new object[] { "AddressInt", true });
                        slRequiredColumnsProperties.Add("XSIGNED", new object[] { "XSigned", true });
                        slRequiredColumnsProperties.Add("XDIGITS", new object[] { "XDigits", true });
                        slRequiredColumnsProperties.Add("XEQUATION", new object[] { "XEquation", true });
                        slRequiredColumnsProperties.Add("XUNITS", new object[] { "XUnits", false });
                        slRequiredColumnsProperties.Add("XPAYLOADTAG", new object[] { "XPayLoad", false });
                        slRequiredColumnsProperties.Add("XMIN", new object[] { "XMin", false });
                        slRequiredColumnsProperties.Add("XMAX", new object[] { "XMax", false });
                        slRequiredColumnsProperties.Add("PARENTPID", new object[] { "ParentPID", false });
                        slRequiredColumnsProperties.Add("PARENTVAL", new object[] { "ParentVal", false });
                        break;
                    case "TABLES":
                        objectType = typeof(XlsTable);
                        slRequiredColumnsProperties.Add("LEVEL", new object[] { "Level", false });
                        slRequiredColumnsProperties.Add("LEVEL2", new object[] { "Level2", false });
                        slRequiredColumnsProperties.Add("KEY", new object[] { "Key", false });
                        slRequiredColumnsProperties.Add("PID", new object[] { "PID", true });
                        slRequiredColumnsProperties.Add("TITLE", new object[] { "Title", true });
                        slRequiredColumnsProperties.Add("COMMENTS", new object[] { "Comments", true });
                        slRequiredColumnsProperties.Add("ADDRESS", new object[] { "AddressInt", true });
                        slRequiredColumnsProperties.Add("BYTES", new object[] { "Bytes", true });
                        slRequiredColumnsProperties.Add("SIGNED", new object[] { "Signed", true });
                        slRequiredColumnsProperties.Add("DECIMALPL", new object[] { "Digits", true });
                        slRequiredColumnsProperties.Add("ROWS", new object[] { "Rows", true });
                        slRequiredColumnsProperties.Add("COLS", new object[] { "Cols", true });
                        slRequiredColumnsProperties.Add("YLABELLINK", new object[] { "YFunction", true });
                        slRequiredColumnsProperties.Add("YUNITS", new object[] { "YUnits", false });
                        slRequiredColumnsProperties.Add("YLABELS", new object[] { "YLabels", false });
                        slRequiredColumnsProperties.Add("YPAYLOADTAG", new object[] { "YPayLoad", false });
                        slRequiredColumnsProperties.Add("XLABELLINK", new object[] { "XFunction", true });
                        slRequiredColumnsProperties.Add("XUNITS", new object[] { "XUnits", false });
                        slRequiredColumnsProperties.Add("XLABELS", new object[] { "XLabels", false });
                        slRequiredColumnsProperties.Add("XPAYLOADTAG", new object[] { "XPayLoad", false });
                        slRequiredColumnsProperties.Add("ZUNITS", new object[] { "Units", false });
                        slRequiredColumnsProperties.Add("ZEQUATION", new object[] { "Equation", true });
                        slRequiredColumnsProperties.Add("MIN", new object[] { "Min", false });
                        slRequiredColumnsProperties.Add("MAX", new object[] { "Max", false });
                        slRequiredColumnsProperties.Add("PARENTPID", new object[] { "ParentPID", false });
                        slRequiredColumnsProperties.Add("PARENTVAL", new object[] { "ParentVal", false });
                        break;
                    case "SCALARS":
                        objectType = typeof(XlsScalar);
                        slRequiredColumnsProperties.Add("LEVEL", new object[] { "Level", false });
                        slRequiredColumnsProperties.Add("LEVEL2", new object[] { "Level2", false });
                        slRequiredColumnsProperties.Add("KEY", new object[] { "Key", false });
                        slRequiredColumnsProperties.Add("PID", new object[] { "PID", true });
                        slRequiredColumnsProperties.Add("PARAMETER", new object[] { "Parameter", true });
                        slRequiredColumnsProperties.Add("UNITS", new object[] { "Units", false });
                        slRequiredColumnsProperties.Add("COMMENTS", new object[] { "Comments", true });
                        slRequiredColumnsProperties.Add("ADDRESS", new object[] { "AddressInt", true });
                        slRequiredColumnsProperties.Add("BYTES", new object[] { "Bytes", true });
                        slRequiredColumnsProperties.Add("SIGNED", new object[] { "Signed", true });
                        slRequiredColumnsProperties.Add("DECIMALPL", new object[] { "Digits", true });
                        slRequiredColumnsProperties.Add("EQUATION", new object[] { "Equation", true });
                        slRequiredColumnsProperties.Add("ENUMNAME", new object[] { "EnumName", false });
                        slRequiredColumnsProperties.Add("ENUMVAL", new object[] { "EnumVal", false });
                        slRequiredColumnsProperties.Add("MIN", new object[] { "Min", false });
                        slRequiredColumnsProperties.Add("MAX", new object[] { "Max", false });
                        slRequiredColumnsProperties.Add("PARENTPID", new object[] { "ParentPID", false });
                        slRequiredColumnsProperties.Add("PARENTVAL", new object[] { "ParentVal", false });
                        break;
                    default:
                        continue;
                }

                bool requiredColumnsPresent = true;
                foreach (string colName in slRequiredColumnsProperties.Keys)
                {
                    string propertyName = (string)slRequiredColumnsProperties[colName][0];
                    bool isRequired = (bool)slRequiredColumnsProperties[colName][1];
                    if (!slColumns.ContainsKey(colName))
                    {
                        if (isRequired)
                        {
                            requiredColumnsPresent = false;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    ((object[])slColumns[colName])[2] = propertyName;
                }
                slRequiredColumnsProperties = null;

                if (!requiredColumnsPresent)
                {
                    globalValidity = false;
                    continue;
                }

                for (int iRow = 1; iRow < dtDT.Rows.Count; iRow++)
                {
                    object xlsObject = Activator.CreateInstance(objectType);

                    foreach (object[] arrColDef in slColumns.Values)
                    {
                        int iCol = (int)arrColDef[0];

                        string nameCol = (string)arrColDef[1];
                        string propCol = (string)arrColDef[2];

                        // No Matching for tis column
                        if (propCol == string.Empty) continue;

                        string sValue = string.Empty;
                        if (dtDT.Rows[iRow][iCol] != null) sValue = dtDT.Rows[iRow][iCol].ToString();

                        switch (workSheetName.ToUpper())
                        {
                            case "REVISION":
                            case "CONFIG":
                            case "LEVELS":
                            case "QHORSE":
                            case "TWEECER":
                                switch (propCol)
                                {
                                    default:
                                        objectType.GetProperty(propCol).SetValue(xlsObject, sValue, null);
                                        break;
                                }
                                break;
                            case "PAYLOAD":
                                switch (propCol)
                                {
                                    case "AddressInt":
                                        if (sValue != string.Empty)
                                        {
                                            try { objectType.GetProperty(propCol).SetValue(xlsObject, Convert.ToInt32(sValue, 16), null); }
                                            catch { }
                                        }
                                        break;
                                    case "Bytes":
                                    case "Digits":
                                        if (sValue != string.Empty)
                                        {
                                            try { objectType.GetProperty(propCol).SetValue(xlsObject, Convert.ToInt32(sValue), null); }
                                            catch { }
                                        }
                                        break;
                                    case "Signed":
                                        if (sValue != string.Empty) objectType.GetProperty(propCol).SetValue(xlsObject, sValue == "1" || sValue.ToLower() == "true", null);
                                        break;
                                    case "BitsArray":
                                        if (sValue != string.Empty)
                                        {
                                            List<string> bitsArray = (List<string>)objectType.GetProperty(propCol).GetValue(xlsObject, null);
                                            if (bitsArray == null) bitsArray = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
                                            try
                                            {
                                                int iBit = Convert.ToInt32(nameCol.Substring(nameCol.Length - 1, 1));
                                                bitsArray[iBit] = sValue;
                                            }
                                            catch { }
                                            objectType.GetProperty(propCol).SetValue(xlsObject, bitsArray, null);
                                        }
                                        break;
                                    default:
                                        objectType.GetProperty(propCol).SetValue(xlsObject, sValue, null);
                                        break;
                                }
                                break;
                            case "FUNCTIONS":
                                switch (propCol)
                                {
                                    case "AddressInt":
                                        if (sValue != string.Empty)
                                        {
                                            try { objectType.GetProperty(propCol).SetValue(xlsObject, Convert.ToInt32(sValue, 16), null); }
                                            catch { }
                                        }
                                        break;
                                    case "Bytes":
                                    case "XDigits":
                                    case "YDigits":
                                    case "Rows":
                                        if (sValue != string.Empty)
                                        {
                                            try { objectType.GetProperty(propCol).SetValue(xlsObject, Convert.ToInt32(sValue), null); }
                                            catch { }
                                        }
                                        break;
                                    case "XSigned":
                                    case "YSigned":
                                        if (sValue != string.Empty) objectType.GetProperty(propCol).SetValue(xlsObject, sValue == "1" || sValue.ToLower() == "true", null);
                                        break;
                                    case "Level":
                                    case "Level2":
                                        if (sValue != string.Empty)
                                        {
                                            if (xlsFile.Levels.ContainsKey(sValue)) objectType.GetProperty(propCol).SetValue(xlsObject, xlsFile.Levels[sValue], null);
                                        }
                                        break;
                                    case "XPayLoad":
                                    case "YPayLoad":
                                        if (sValue != string.Empty)
                                        {
                                            foreach (XlsPayLoad xlsPayLoad in xlsFile.PayLoads.Values)
                                            {
                                                if (xlsPayLoad.Label.ToUpper() == sValue.ToUpper())
                                                {
                                                    objectType.GetProperty(propCol).SetValue(xlsObject, xlsPayLoad, null);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        objectType.GetProperty(propCol).SetValue(xlsObject, sValue, null);
                                        break;
                                }
                                break;
                            case "TABLES":
                                switch (propCol)
                                {
                                    case "AddressInt":
                                        if (sValue != string.Empty)
                                        {
                                            try { objectType.GetProperty(propCol).SetValue(xlsObject, Convert.ToInt32(sValue, 16), null); }
                                            catch { }
                                        }
                                        break;
                                    case "Bytes":
                                    case "Digits":
                                    case "Cols":
                                    case "Rows":
                                        if (sValue != string.Empty)
                                        {
                                            try { objectType.GetProperty(propCol).SetValue(xlsObject, Convert.ToInt32(sValue), null); }
                                            catch { }
                                        }
                                        break;
                                    case "Signed":
                                        if (sValue != string.Empty) objectType.GetProperty(propCol).SetValue(xlsObject, sValue == "1" || sValue.ToLower() == "true", null);
                                        break;
                                    case "Level":
                                    case "Level2":
                                        if (sValue != string.Empty)
                                        {
                                            if (xlsFile.Levels.ContainsKey(sValue)) objectType.GetProperty(propCol).SetValue(xlsObject, xlsFile.Levels[sValue], null);
                                        }
                                        break;
                                    case "XFunction":
                                    case "YFunction":
                                        if (sValue != string.Empty)
                                        {
                                            if (xlsFile.Functions.ContainsKey(sValue)) objectType.GetProperty(propCol).SetValue(xlsObject, xlsFile.Functions[sValue], null);
                                        }
                                        break;
                                    case "XPayLoad":
                                    case "YPayLoad":
                                        if (sValue != string.Empty)
                                        {
                                            foreach (XlsPayLoad xlsPayLoad in xlsFile.PayLoads.Values)
                                            {
                                                if (xlsPayLoad.Label.ToUpper() == sValue.ToUpper())
                                                {
                                                    objectType.GetProperty(propCol).SetValue(xlsObject, xlsPayLoad, null);
                                                    break;
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        objectType.GetProperty(propCol).SetValue(xlsObject, sValue, null);
                                        break;
                                }
                                break;
                            case "SCALARS":
                                switch (propCol)
                                {
                                    case "AddressInt":
                                        if (sValue != string.Empty)
                                        {
                                            try { objectType.GetProperty(propCol).SetValue(xlsObject, Convert.ToInt32(sValue, 16), null); }
                                            catch { }
                                        }
                                        break;
                                    case "Bytes":
                                    case "Digits":
                                        if (sValue != string.Empty)
                                        {
                                            try { objectType.GetProperty(propCol).SetValue(xlsObject, Convert.ToInt32(sValue), null); }
                                            catch { }
                                        }
                                        break;
                                    case "Signed":
                                        if (sValue != string.Empty) objectType.GetProperty(propCol).SetValue(xlsObject, sValue == "1" || sValue.ToLower() == "true", null);
                                        break;
                                    case "Level":
                                    case "Level2":
                                        if (sValue != string.Empty)
                                        {
                                            if (xlsFile.Levels.ContainsKey(sValue)) objectType.GetProperty(propCol).SetValue(xlsObject, xlsFile.Levels[sValue], null);
                                        }
                                        break;
                                    default:
                                        objectType.GetProperty(propCol).SetValue(xlsObject, sValue, null);
                                        break;
                                }
                                break;
                        
                        }
                    }

                    switch (workSheetName.ToUpper())
                    {
                        case "REVISION":
                            xlsFile.Revisions.Add((XlsRevision)xlsObject);
                            break;
                        case "CONFIG":
                            xlsFile.ConfigParameters.Add((XlsConfigParameter)xlsObject);
                            break;
                        case "LEVELS":
                            if (!xlsFile.Levels.ContainsKey(((XlsLevel)xlsObject).Value)) xlsFile.Levels.Add(((XlsLevel)xlsObject).Value, (XlsLevel)xlsObject);
                            break;
                        case "QHORSE":
                            xlsFile.QHorses.Add((XlsQHorse)xlsObject);
                            break;
                        case "TWEECER":
                            xlsFile.TwEECers.Add((XlsTwEECer)xlsObject);
                            break;
                        case "PAYLOAD":
                            if (!xlsFile.PayLoads.ContainsKey(((XlsPayLoad)xlsObject).AddressInt)) xlsFile.PayLoads.Add(((XlsPayLoad)xlsObject).AddressInt, (XlsPayLoad)xlsObject);
                            break;
                        case "FUNCTIONS":
                            if (((XlsFunction)xlsObject).Key == null || ((XlsFunction)xlsObject).Key == string.Empty) ((XlsFunction)xlsObject).Key = ((XlsFunction)xlsObject).PID;
                            if (!xlsFile.Functions.ContainsKey(((XlsFunction)xlsObject).Key)) xlsFile.Functions.Add(((XlsFunction)xlsObject).Key, (XlsFunction)xlsObject);
                            break;
                        case "TABLES":
                            if (((XlsTable)xlsObject).Key == null || ((XlsTable)xlsObject).Key == string.Empty) ((XlsTable)xlsObject).Key = ((XlsTable)xlsObject).PID;
                            if (!xlsFile.Tables.ContainsKey(((XlsTable)xlsObject).Key)) xlsFile.Tables.Add(((XlsTable)xlsObject).Key, (XlsTable)xlsObject);
                            break;
                        case "SCALARS":
                            if (((XlsScalar)xlsObject).Key == null || ((XlsScalar)xlsObject).Key == string.Empty) ((XlsScalar)xlsObject).Key = ((XlsScalar)xlsObject).PID;
                            if (!xlsFile.Scalars.ContainsKey(((XlsScalar)xlsObject).Key)) xlsFile.Scalars.Add(((XlsScalar)xlsObject).Key, (XlsScalar)xlsObject);
                            break;

                    }
                }
            }

            slWorkSheetsData = null;

            return requireGlobalValidity ? globalValidity : true;
        }

        public static bool WriteXlsFile(ref XlsFile xlsFile, string xlsFilePath, ref List<string> lstErrors)
        {
            string[] arrWorkSheets = new string[] { "Scalars", "Tables", "Functions", "PayLoad", "TwEECer", "QHorse", "Levels", "Config", "Revision" };
            SortedList<string, DataTable> slWorkSheetsData = new SortedList<string, DataTable>();
            List<string> lstLongTextColumns = new List<string>();

            for (int iWorkSheet = 0; iWorkSheet < arrWorkSheets.Length; iWorkSheet++)
            {
                string workSheetName = arrWorkSheets[iWorkSheet];
                DataTable dtDT = new DataTable(workSheetName);
                SortedList<int, object[]> slColumns = new SortedList<int, object[]>();

                switch (workSheetName.ToUpper())
                {
                    case "REVISION":
                        slColumns.Add(0, new object[] {0, "REVISION #", "Revision #", "Revision"});
                        slColumns.Add(1, new object[] {1, "COMMENTS", "Comments", "Comments"});
                        lstLongTextColumns.Add(workSheetName + "." + "Comments");
                        break;
                    case "CONFIG":
                        slColumns.Add(0, new object[] {0, "PARAMETER", "Parameter", "Parameter"});
                        slColumns.Add(1, new object[] {1, "VALUE", "Value", "Value"});
                        break;
                    case "LEVELS":
                        slColumns.Add(0, new object[] {0, "LEVEL", "Level", "Label"});
                        slColumns.Add(1, new object[] { 1, "VALUE", "Value", "Value" });
                        break;
                    case "QHORSE":
                        slColumns.Add(0, new object[] { 0, "ADDRESS", "Address", "Address" });
                        slColumns.Add(1, new object[] { 1, "DATA", "Data", "Data" });
                        slColumns.Add(2, new object[] { 2, "PARAMETER", "Parameter", "Parameter" });
                        slColumns.Add(3, new object[] { 3, "VALUE", "Value", "Value" });
                        break;
                    case "TWEECER":
                        slColumns.Add(0, new object[] { 0, "ADDRESS", "Address", "Address" });
                        slColumns.Add(1, new object[] { 1, "DATA", "Data", "Data" });
                        slColumns.Add(2, new object[] { 2, "PARAMETER", "Parameter", "Parameter" });
                        slColumns.Add(3, new object[] { 3, "VALUE", "Value", "Value" });
                        break;
                    case "PAYLOAD":
                        slColumns.Add(0, new object[] { 0, "TAG", "TAG", "Label"});
                        slColumns.Add(1, new object[] { 1, "DESCRIPTION", "Description", "Description"});
                        slColumns.Add(2, new object[] { 2, "ADDRESS", "Address", "AddressInt"});
                        slColumns.Add(3, new object[] { 3, "BYTES", "Bytes", "Bytes"});
                        slColumns.Add(4, new object[] { 4, "SIGNED", "Signed", "Signed"});
                        slColumns.Add(5, new object[] { 5, "EQUATION", "Equation", "Equation"});
                        slColumns.Add(6, new object[] { 6, "DIGITS", "Digits", "Digits"});
                        slColumns.Add(7, new object[] { 7, "UNITS", "Units", "Units"});
                        slColumns.Add(8, new object[] { 8, "BIT0", "Bit0", "BitsArray"});
                        slColumns.Add(9, new object[] { 9, "BIT1", "Bit1", "BitsArray"});
                        slColumns.Add(10, new object[] { 10, "BIT2", "Bit2", "BitsArray"});
                        slColumns.Add(11, new object[] { 11, "BIT3", "Bit3", "BitsArray"});
                        slColumns.Add(12, new object[] { 12, "BIT4", "Bit4", "BitsArray"});
                        slColumns.Add(13, new object[] { 13, "BIT5", "Bit5", "BitsArray"});
                        slColumns.Add(14, new object[] { 14, "BIT6", "Bit6", "BitsArray"});
                        slColumns.Add(15, new object[] { 15, "BIT7", "Bit7", "BitsArray"});
                        lstLongTextColumns.Add(workSheetName + "." + "Description");
                        break;
                    case "FUNCTIONS":
                        slColumns.Add(0, new object[] { 0, "LEVEL", "Level", "Level"});
                        slColumns.Add(1, new object[] { 1, "LEVEL2", "Level2", "Level2"});
                        slColumns.Add(2, new object[] { 2, "KEY", "Key", "Key"});
                        slColumns.Add(3, new object[] { 3, "PID", "PID", "PID"});
                        slColumns.Add(4, new object[] { 4, "TITLE", "Title", "Title"});
                        slColumns.Add(5, new object[] { 5, "COMMENTS", "Comments", "Comments"});
                        slColumns.Add(6, new object[] { 6, "ROWS", "Rows", "Rows"});
                        slColumns.Add(7, new object[] { 7, "BYTES", "Bytes", "Bytes"});
                        slColumns.Add(8, new object[] { 8, "YSIGNED", "YSigned", "YSigned"});
                        slColumns.Add(9, new object[] { 9, "YDIGITS", "YDigits", "YDigits"});
                        slColumns.Add(10, new object[] { 10, "YEQUATION", "YEquation", "YEquation"});
                        slColumns.Add(11, new object[] { 11, "YUNITS", "YUnits", "YUnits"});
                        slColumns.Add(12, new object[] { 12, "YMIN", "YMin", "YMin"});
                        slColumns.Add(13, new object[] { 13, "YMAX", "YMax", "YMax"});
                        slColumns.Add(14, new object[] { 14, "YPAYLOADTAG", "YPayloadTag", "YPayLoad"});
                        slColumns.Add(15, new object[] { 15, "XADDRESS", "XAddress", "AddressInt"});
                        slColumns.Add(16, new object[] { 16, "XSIGNED", "XSigned", "XSigned"});
                        slColumns.Add(17, new object[] { 17, "XDIGITS", "XDigits", "XDigits"});
                        slColumns.Add(18, new object[] { 18, "XEQUATION", "XEquation", "XEquation"});
                        slColumns.Add(19, new object[] { 19, "XUNITS", "XUnits", "XUnits"});
                        slColumns.Add(20, new object[] { 20, "XPAYLOADTAG", "XPayloadTag", "XPayLoad"});
                        slColumns.Add(21, new object[] { 21, "XMIN", "XMin", "XMin"});
                        slColumns.Add(22, new object[] { 22, "XMAX", "XMax", "XMax"});
                        slColumns.Add(23, new object[] { 23, "PARENTPID", "ParentPID", "ParentPID"});
                        slColumns.Add(24, new object[] { 24, "PARENTVAL", "ParentVal", "ParentVal"});
                        lstLongTextColumns.Add(workSheetName + "." + "Comments");
                        break;
                    case "TABLES":
                        slColumns.Add(0, new object[] { 0, "LEVEL", "Level", "Level"});
                        slColumns.Add(1, new object[] { 1, "LEVEL2", "Level2", "Level2"});
                        slColumns.Add(2, new object[] { 2, "KEY", "Key", "Key"});
                        slColumns.Add(3, new object[] { 3, "PID", "PID", "PID"});
                        slColumns.Add(4, new object[] { 4, "TITLE", "Title", "Title"});
                        slColumns.Add(5, new object[] { 5, "COMMENTS", "Comments", "Comments"});
                        slColumns.Add(6, new object[] { 6, "ADDRESS", "Address", "AddressInt"});
                        slColumns.Add(7, new object[] { 7, "BYTES", "Bytes", "Bytes"});
                        slColumns.Add(8, new object[] { 8, "SIGNED", "Signed", "Signed"});
                        slColumns.Add(9, new object[] { 9, "DECIMALPL", "DecimalPl", "Digits"});
                        slColumns.Add(10, new object[] { 10, "ROWS", "Rows", "Rows"});
                        slColumns.Add(11, new object[] { 11, "COLS", "Cols", "Cols"});
                        slColumns.Add(12, new object[] { 12, "YLABELLINK", "YLabelLink", "YFunction"});
                        slColumns.Add(13, new object[] { 13, "YUNITS", "YUnits", "YUnits"});
                        slColumns.Add(14, new object[] { 14, "YLABELS", "YLabels", "YLabels"});
                        slColumns.Add(15, new object[] { 15, "YPAYLOADTAG", "YPayloadTag", "YPayLoad"});
                        slColumns.Add(16, new object[] { 16, "XLABELLINK", "XLabelLink", "XFunction"});
                        slColumns.Add(17, new object[] { 17, "XUNITS", "XUnits", "XUnits"});
                        slColumns.Add(18, new object[] { 18, "XLABELS", "XLabels", "XLabels"});
                        slColumns.Add(19, new object[] { 19, "XPAYLOADTAG", "XPayloadTag", "XPayLoad"});
                        slColumns.Add(20, new object[] { 20, "ZUNITS", "ZUnits", "Units"});
                        slColumns.Add(21, new object[] { 21, "ZEQUATION", "ZEquation", "Equation"});
                        slColumns.Add(22, new object[] { 22, "MIN", "Min", "Min"});
                        slColumns.Add(23, new object[] { 23, "MAX", "Max", "Max"});
                        slColumns.Add(24, new object[] { 24, "PARENTPID", "ParentPID", "ParentPID"});
                        slColumns.Add(25, new object[] { 25, "PARENTVAL", "ParentVal", "ParentVal"});
                        lstLongTextColumns.Add(workSheetName + "." + "Comments");
                        break;
                    case "SCALARS":
                        slColumns.Add(0, new object[] { 0, "LEVEL", "Level", "Level"});
                        slColumns.Add(1, new object[] { 1, "LEVEL2", "Level2", "Level2"});
                        slColumns.Add(2, new object[] { 2, "KEY", "Key", "Key"});
                        slColumns.Add(3, new object[] { 3, "PID", "PID", "PID"});
                        slColumns.Add(4, new object[] { 4, "PARAMETER", "Parameter", "Parameter" });
                        slColumns.Add(5, new object[] { 5, "UNITS", "Units", "Units" });
                        slColumns.Add(6, new object[] { 6, "COMMENTS", "Comments", "Comments" });
                        slColumns.Add(7, new object[] { 7, "ADDRESS", "Address", "AddressInt" });
                        slColumns.Add(8, new object[] { 8, "BYTES", "Bytes", "Bytes" });
                        slColumns.Add(9, new object[] { 9, "SIGNED", "Signed", "Signed" });
                        slColumns.Add(10, new object[] { 10, "DECIMALPL", "DecimalPl", "Digits" });
                        slColumns.Add(11, new object[] { 11, "EQUATION", "Equation", "Equation" });
                        slColumns.Add(12, new object[] { 12, "ENUMNAME", "EnumName", "EnumName" });
                        slColumns.Add(13, new object[] { 13, "ENUMVAL", "EnumVal", "EnumVal" });
                        slColumns.Add(14, new object[] { 14, "MIN", "Min", "Min" });
                        slColumns.Add(15, new object[] { 15, "MAX", "Max", "Max" });
                        slColumns.Add(16, new object[] { 16, "PARENTPID", "ParentPID", "ParentPID" });
                        slColumns.Add(17, new object[] { 17, "PARENTVAL", "ParentVal", "ParentVal" });
                        lstLongTextColumns.Add(workSheetName + "." + "Comments");
                        break;
                }

                foreach (object[] arrColInfo in slColumns.Values) dtDT.Columns.Add(arrColInfo[2].ToString());

                slWorkSheetsData.Add(string.Format("{0:d2}.{1}", iWorkSheet, workSheetName), dtDT);

                List<object> lstXlsObject = null;
                switch (workSheetName.ToUpper())
                {
                    case "REVISION":
                        foreach (XlsRevision xlsObject in xlsFile.Revisions)
                        {
                            DataRow drDR = dtDT.NewRow();
                            foreach (object[] arrColInfo in slColumns.Values) drDR[arrColInfo[2].ToString()] = xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                            dtDT.Rows.Add(drDR);
                        }
                        break;
                    case "CONFIG":
                        foreach (XlsConfigParameter xlsObject in xlsFile.ConfigParameters)
                        {
                            DataRow drDR = dtDT.NewRow();
                            foreach (object[] arrColInfo in slColumns.Values) drDR[arrColInfo[2].ToString()] = xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                            dtDT.Rows.Add(drDR);
                        }
                        break;
                    case "LEVELS":
                        foreach (XlsLevel xlsObject in xlsFile.Levels.Values)
                        {
                            DataRow drDR = dtDT.NewRow();
                            foreach (object[] arrColInfo in slColumns.Values) drDR[arrColInfo[2].ToString()] = xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                            dtDT.Rows.Add(drDR);
                        }
                        break;
                    case "QHORSE":
                        foreach (XlsQHorse xlsObject in xlsFile.QHorses)
                        {
                            DataRow drDR = dtDT.NewRow();
                            foreach (object[] arrColInfo in slColumns.Values) drDR[arrColInfo[2].ToString()] = xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                            dtDT.Rows.Add(drDR);
                        }
                        break;
                    case "TWEECER":
                        foreach (XlsTwEECer xlsObject in xlsFile.TwEECers)
                        {
                            DataRow drDR = dtDT.NewRow();
                            foreach (object[] arrColInfo in slColumns.Values) drDR[arrColInfo[2].ToString()] = xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                            dtDT.Rows.Add(drDR);
                        }
                        break;
                    case "PAYLOAD":
                        lstXlsObject = new List<object>();
                        // Starting with Long Text Fields to be Xls Compliant
                        foreach (XlsPayLoad xlsObject in xlsFile.PayLoads.Values)
                        {
                            if (xlsObject.Description == null) continue;
                            if (xlsObject.Description.Length < 256) continue;
                            lstXlsObject.Add(xlsObject);
                        }
                        foreach (XlsPayLoad xlsObject in xlsFile.PayLoads.Values)
                        {
                            if (xlsObject.Description == null) lstXlsObject.Add(xlsObject);
                            else if (xlsObject.Description.Length < 256) lstXlsObject.Add(xlsObject);
                        }
                        foreach (XlsPayLoad xlsObject in lstXlsObject)
                        {
                            DataRow drDR = dtDT.NewRow();
                            foreach (object[] arrColInfo in slColumns.Values)
                            {
                                switch (((string)arrColInfo[3]).ToUpper())
                                {
                                    case "ADDRESSINT":
                                        drDR[arrColInfo[2].ToString()] = string.Format("0x{0:X4}", xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null));
                                        break;
                                    case "BITSARRAY":
                                        List<string> arrBits = (List<string>)xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        if (arrBits != null)
                                        {
                                            for (int iBit = 0; iBit < arrBits.Count; iBit++) drDR[arrColInfo[2].ToString()] = arrBits[iBit];
                                        }
                                        break;
                                    default:
                                        drDR[arrColInfo[2].ToString()] = xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        break;
                                }
                            }
                            dtDT.Rows.Add(drDR);
                        }
                        lstXlsObject = null;
                        break;
                    case "FUNCTIONS":
                        lstXlsObject = new List<object>();
                        // Starting with Long Text Fields to be Xls Compliant
                        foreach (XlsFunction xlsObject in xlsFile.Functions.Values)
                        {
                            if (xlsObject.Comments == null) continue;
                            if (xlsObject.Comments.Length < 256) continue;
                            lstXlsObject.Add(xlsObject);
                        }
                        foreach (XlsFunction xlsObject in xlsFile.Functions.Values)
                        {
                            if (xlsObject.Comments == null) lstXlsObject.Add(xlsObject);
                            else if (xlsObject.Comments.Length < 256) lstXlsObject.Add(xlsObject);
                        }
                        foreach (XlsFunction xlsObject in lstXlsObject)
                        {
                            DataRow drDR = dtDT.NewRow();
                            foreach (object[] arrColInfo in slColumns.Values)
                            {
                                switch (((string)arrColInfo[3]).ToUpper())
                                {
                                    case "ADDRESSINT":
                                        drDR[arrColInfo[2].ToString()] = string.Format("0x{0:X6}", xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null));
                                        break;
                                    case "LEVEL":
                                    case "LEVEL2":
                                        XlsLevel xlsLevel = (XlsLevel)xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        if (xlsLevel != null) drDR[arrColInfo[2].ToString()] = xlsLevel.Value;
                                        break;
                                    case "XPAYLOAD":
                                    case "YPAYLOAD":
                                        XlsPayLoad xlsPayLoad = (XlsPayLoad)xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        if (xlsPayLoad != null) drDR[arrColInfo[2].ToString()] = xlsPayLoad.Label;
                                        break;
                                    default:
                                        drDR[arrColInfo[2].ToString()] = xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        break;
                                }
                            }
                            dtDT.Rows.Add(drDR);
                        }
                        lstXlsObject = null;
                        break;
                    case "TABLES":
                        foreach (XlsTable xlsObject in xlsFile.Tables.Values)
                        {
                            DataRow drDR = dtDT.NewRow();
                            foreach (object[] arrColInfo in slColumns.Values)
                            {
                                switch (((string)arrColInfo[3]).ToUpper())
                                {
                                    case "ADDRESSINT":
                                        drDR[arrColInfo[2].ToString()] = string.Format("0x{0:X6}", xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null));
                                        break;
                                    case "LEVEL":
                                    case "LEVEL2":
                                        XlsLevel xlsLevel = (XlsLevel)xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        if (xlsLevel != null) drDR[arrColInfo[2].ToString()] = xlsLevel.Value;
                                        break;
                                    case "XPAYLOAD":
                                    case "YPAYLOAD":
                                        XlsPayLoad xlsPayLoad = (XlsPayLoad)xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        if (xlsPayLoad != null) drDR[arrColInfo[2].ToString()] = xlsPayLoad.Label;
                                        break;
                                    case "XFUNCTION":
                                    case "YFUNCTION":
                                        XlsFunction xlsFunction = (XlsFunction)xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        if (xlsFunction != null) drDR[arrColInfo[2].ToString()] = xlsFunction.Key;
                                        break;
                                    default:
                                        drDR[arrColInfo[2].ToString()] = xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        break;
                                }
                            }
                            dtDT.Rows.Add(drDR);
                        }
                        break;
                    case "SCALARS":
                        foreach (XlsScalar xlsObject in xlsFile.Scalars.Values)
                        {
                            DataRow drDR = dtDT.NewRow();
                            foreach (object[] arrColInfo in slColumns.Values)
                            {
                                switch (((string)arrColInfo[3]).ToUpper())
                                {
                                    case "ADDRESSINT":
                                        drDR[arrColInfo[2].ToString()] = string.Format("0x{0:X6}", xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null));
                                        break;
                                    case "LEVEL":
                                    case "LEVEL2":
                                        XlsLevel xlsLevel = (XlsLevel)xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        if (xlsLevel != null) drDR[arrColInfo[2].ToString()] = xlsLevel.Value;
                                        break;
                                    default:
                                        drDR[arrColInfo[2].ToString()] = xlsObject.GetType().GetProperty((string)arrColInfo[3]).GetValue(xlsObject, null);
                                        break;
                                }
                            }
                            dtDT.Rows.Add(drDR);
                        }
                        break;
                }
            }

            return WriteXls(xlsFilePath, ref slWorkSheetsData, ref lstErrors);
        }
    }
}
