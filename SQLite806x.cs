using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Reflection;
using System.Windows.Forms;

namespace SQLite806x
{
    public class SQLite806xDB
    {
        private string dbFilePath = string.Empty;
        private string dbDataSource = string.Empty;

        private string dbCurrentVersion806x = string.Empty;
        private string dbCurrentVersionSAD806x = string.Empty;
        private string dbCurrentVersionSAD = string.Empty;
        private string dbCurrentVersionTunerPro = string.Empty;

        private string dbVersion806x = string.Empty;
        private string dbVersionSAD806x = string.Empty;
        private string dbVersionSAD = string.Empty;
        private string dbVersionTunerPro = string.Empty;

        private bool validDB = false;
        public bool ValidDB { get { return validDB; } }

        private List<T_SQLiteTable> sqlTables = null;

        private List<T_SQLiteTable> sqlMissingTables = null;
        public List<T_SQLiteTable> MissingTables { get { return sqlMissingTables; } }
        private List<F_SQLiteField> sqlMissingFields = null;
        public List<F_SQLiteField> MissingFields { get { return sqlMissingFields; } }
        private List<F_SQLiteField> sqlDifferentFields = null;
        public List<F_SQLiteField> DifferentFields { get { return sqlDifferentFields; } }

        public string SyncType
        {
            get { return this.GetType().Name; }
        }

        public string SyncId
        {
            get
            {
                string sId = string.Empty;
                try
                {
                    if (validDB)
                    {
                        FileInfo fiFI = new FileInfo(dbFilePath);
                        if (fiFI.Exists) sId = fiFI.CreationTimeUtc.ToString("yyyyMMdd.HHmmss.") + fiFI.Name;
                        fiFI = null;
                    }
                }
                catch { }
                return sId;
            }
        }

        public SQLite806xDB(string filePath)
        {
            dbFilePath = filePath;
            dbDataSource = "Data Source=" + filePath + ";";

            initCurrentVersionSchema();

            if (!File.Exists(dbFilePath))
            {
                createSchema();
                updateDBVersionRows();
            }

            sqlTables = validatedSchema();
        }

        public void initCurrentVersionSchema()
        {
            sqlTables = new List<T_SQLiteTable>();

            dbCurrentVersion806x = SQLite806xTools806x.getDBVersion();
            SQLite806xTools806x.initSchema(ref sqlTables);

            dbCurrentVersionSAD806x = SQLite806xToolsSAD806x.getDBVersion();
            SQLite806xToolsSAD806x.initSchema(ref sqlTables);

            dbCurrentVersionSAD = SQLite806xToolsSAD.getDBVersion();
            SQLite806xToolsSAD.initSchema(ref sqlTables);

            dbCurrentVersionTunerPro = SQLite806xToolsTunerPro.getDBVersion();
            SQLite806xToolsTunerPro.initSchema(ref sqlTables);
        }

        public void createSchema()
        {
            validDB = false;

            try { SQLiteConnection.CreateFile(dbFilePath); }
            catch { return; }

            List<T_SQLiteTable> nonCreatedTables = createSchemaTables(ref sqlTables);

            if (nonCreatedTables != null)
            {
                if (nonCreatedTables.Count > 0)
                {
                    try { File.Delete(dbFilePath); }
                    catch { }

                    return;
                }
            }

            validDB = true;
        }

        public List<T_SQLiteTable> createSchemaTables(ref List<T_SQLiteTable> lstTables)
        {
            List<T_SQLiteTable> nonCreatedTables = new List<T_SQLiteTable>();

            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource + "Version=3;"))
            {
                sCon.Open();

                // Schema creation
                foreach (T_SQLiteTable tTable in lstTables)
                {
                    using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                    {
                        List<string> fFields = new List<string>();
                        foreach (F_SQLiteField fField in tTable.Fields)
                        {
                            if (fField.isRowId) continue;
                            fFields.Add(string.Format("{0} {1}", fField.Name, fField.SchemaType));
                        }

                        string sqlQuery = string.Format("CREATE TABLE \"{0}\" ({1});", tTable.Name, string.Join(", ", fFields.ToArray()));

                        sCmd.CommandText = sqlQuery;

                        try
                        {
                            sCmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            nonCreatedTables.Add(tTable);
                        }
                    }
                }

                sCon.Close();
            }

            return nonCreatedTables;
        }

        public List<F_SQLiteField> createSchemaFields(ref List<F_SQLiteField> lstFields)
        {
            List<F_SQLiteField> nonCreatedFields = new List<F_SQLiteField>();

            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource + "Version=3;"))
            {
                sCon.Open();

                // Schema creation
                foreach (F_SQLiteField fField in lstFields)
                {
                    if (fField.isRowId) continue;
                    using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                    {
                        string sFieldDef = string.Format("{0} {1}", fField.Name, fField.SchemaType);

                        string sqlQuery = string.Format("ALTER TABLE [{0}] ADD COLUMN {1};", fField.TableName, sFieldDef);

                        sCmd.CommandText = sqlQuery;

                        try
                        {
                            sCmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            nonCreatedFields.Add(fField);
                        }
                    }
                }

                sCon.Close();
            }

            return nonCreatedFields;
        }

        public List<F_SQLiteField> updateSchemaFields(ref List<F_SQLiteField> lstFields)
        {
            List<F_SQLiteField> nonUpdatedFields = new List<F_SQLiteField>();

            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource + "Version=3;"))
            {
                sCon.Open();

                // Schema creation
                foreach (F_SQLiteField fField in lstFields)
                {
                    if (fField.isRowId) continue;
                    using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                    {
                        string sqlQuery = string.Format("ALTER TABLE [{0}] RENAME COLUMN {1} TO {2};", fField.TableName, fField.Name, fField.Name + DateTime.Now.ToString("_yyyyMMdd_HHmmss"));

                        sCmd.CommandText = sqlQuery;

                        try
                        {
                            sCmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            nonUpdatedFields.Add(fField);
                            continue;
                        }
                    }
                    using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                    {
                        string sFieldDef = string.Format("{0} {1}", fField.Name, fField.SchemaType);

                        string sqlQuery = string.Format("ALTER TABLE [{0}] ADD COLUMN {1};", fField.TableName, sFieldDef);

                        sCmd.CommandText = sqlQuery;

                        try
                        {
                            sCmd.ExecuteNonQuery();
                        }
                        catch
                        {
                            nonUpdatedFields.Add(fField);
                        }
                    }
                }

                sCon.Close();
            }

            return nonUpdatedFields;
        }

        public void updateDBVersionRows()
        {
            // DB Information mngt
            List<R_806x_DB_Information> lst806xDBInfo = new List<R_806x_DB_Information>();
            SQLite806xTools806x.addDBVersionRow(this, ref lst806xDBInfo);
            Truncate<R_806x_DB_Information>();
            Write<R_806x_DB_Information>(ref lst806xDBInfo);
            lst806xDBInfo = null;

            List<R_SAD806x_DB_Information> lstSAD806xDBInfo = new List<R_SAD806x_DB_Information>();
            SQLite806xToolsSAD806x.addDBVersionRow(this, ref lstSAD806xDBInfo);
            Truncate<R_SAD806x_DB_Information>();
            Write<R_SAD806x_DB_Information>(ref lstSAD806xDBInfo);
            lstSAD806xDBInfo = null;

            List<R_SAD_DB_Information> lstSADDBInfo = new List<R_SAD_DB_Information>();
            SQLite806xToolsSAD.addDBVersionRow(this, ref lstSADDBInfo);
            Truncate<R_SAD_DB_Information>();
            Write<R_SAD_DB_Information>(ref lstSADDBInfo);
            lstSADDBInfo = null;

            List<R_TunerPro_DB_Information> lstTPDBInfo = new List<R_TunerPro_DB_Information>();
            SQLite806xToolsTunerPro.addDBVersionRow(this, ref lstTPDBInfo);
            Truncate<R_TunerPro_DB_Information>();
            Write<R_TunerPro_DB_Information>(ref lstTPDBInfo);
            lstTPDBInfo = null;
        }

        public List<T_SQLiteTable> validatedSchema()
        {
            if (!File.Exists(dbFilePath))
            {
                validDB = false;
                return null;
            }

            List<T_SQLiteTable> sqlExistingTables = new List<T_SQLiteTable>();

            validDB = true;

            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource))
            {
                try
                {
                    sCon.Open();
                }
                catch
                {
                    validDB = false;
                    return null;
                }

                try
                {
                    using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                    {
                        sCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
                        using (SQLiteDataReader sReader = sCmd.ExecuteReader())
                        {
                            if (sReader.HasRows)
                            {
                                while (sReader.Read())
                                {
                                    string tableName = sReader.GetString(0);
                                    string @namespace = "SQLite806x";
                                    string @rowClass = "R_" + tableName;
                                    Type tableClassType = Type.GetType(string.Format("{0}.{1}", @namespace, @rowClass));
                                    if (tableClassType == null) continue;

                                    sqlExistingTables.Add(new T_SQLiteTable(tableName, tableClassType, string.Empty, string.Empty, false, false, false));
                                }
                            }
                            else
                            {
                                validDB = false;
                            }
                        }
                    }
                }
                catch
                {
                    validDB = false;
                }

                try
                {
                    foreach (T_SQLiteTable sqlTable in sqlExistingTables)
                    {
                        using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                        {
                            sCmd.CommandText = "PRAGMA table_info([" + sqlTable.Name + "]);";
                            using (SQLiteDataReader sReader = sCmd.ExecuteReader())
                            {
                                string @namespace = this.GetType().Namespace;
                                string @rowClass = "R_" + sqlTable.Name;
                                Type tableClassType = Type.GetType(string.Format("{0}.{1}", @namespace, @rowClass));
                                if (tableClassType == null) continue;

                                if (!sReader.HasRows) continue;

                                //RowId forced
                                F_SQLiteField rIdField = new F_SQLiteField(sqlTable.Name, "RowId", "INT");
                                rIdField.isRowId = true;
                                rIdField.ReadOnly = true;
                                if (tableClassType.GetProperty(rIdField.Name) != null) sqlTable.Fields.Add(rIdField);

                                while (sReader.Read())
                                {
                                    F_SQLiteField fField = new F_SQLiteField(sqlTable.Name, sReader.GetString(1), sReader.GetString(2));
                                    if (tableClassType.GetProperty(fField.Name) == null) continue;
                                    sqlTable.Fields.Add(fField);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    validDB = false;
                }

                try { sCon.Close(); }
                catch { }
            }

            if (!validDB) return null;

            // Version checking
            // 806x_DB_Information
            validateSubSchema<R_806x_DB_Information>(ref dbVersion806x, ref dbCurrentVersion806x, ref sqlExistingTables);

            // SAD806x_DB_Information
            validateSubSchema<R_SAD806x_DB_Information>(ref dbVersionSAD806x, ref dbCurrentVersionSAD806x, ref sqlExistingTables);

            // SAD_DB_Information
            validateSubSchema<R_SAD_DB_Information>(ref dbVersionSAD, ref dbCurrentVersionSAD, ref sqlExistingTables);

            // TunerPro_DB_Information
            validateSubSchema<R_TunerPro_DB_Information>(ref dbVersionTunerPro, ref dbCurrentVersionTunerPro, ref sqlExistingTables);

            if (!validDB) return null;

            return sqlExistingTables;
        }

        private void validateSubSchema<T>(ref string dbVersion, ref string dbCurrentVersion, ref List<T_SQLiteTable> sqlExistingTables)
        {
            dbVersion = string.Empty;
            
            if (!validDB) return;

            if (!File.Exists(dbFilePath))
            {
                validDB = false;
                return;
            }

            string dbiTableName = SQLite806xTools.TableNameFromType(typeof(T));
            if (dbiTableName == string.Empty)
            {
                validDB = false;
                return;
            }

            if (!dbiTableName.EndsWith("_DB_Information"))
            {
                validDB = false;
                return;
            }

            string tableNamePrefix = dbiTableName.Substring(0, dbiTableName.Length - "DB_Information".Length);
            
            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource))
            {
                try { sCon.Open(); }
                catch
                {
                    validDB = false;
                    return;
                }

                if (validDB)
                {
                    int dbiCount = 0;

                    // DB_Information
                    using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                    {
                        sCmd.CommandText = "SELECT Version FROM [" + dbiTableName + "];";
                        try
                        {
                            using (SQLiteDataReader sReader = sCmd.ExecuteReader())
                            {
                                dbiCount = 0;
                                if (sReader.HasRows)
                                {
                                    while (sReader.Read())
                                    {
                                        dbiCount++;
                                        if (dbiCount > 1) break;
                                        dbVersion = sReader.GetString(0);
                                    }
                                }
                                if (dbiCount != 1) dbVersion = string.Empty;
                            }
                        }
                        catch { }
                    }
                }

                try { sCon.Close(); }
                catch { validDB = false; }
            }

            // No readable version, it is not a valid DB
            if (dbVersion == string.Empty)
            {
                validDB = false;
                return;
            }

            // Version is the current one or an old one, it has to be checked
            List<T_SQLiteTable> missingTables = new List<T_SQLiteTable>();
            List<F_SQLiteField> missingFields = new List<F_SQLiteField>();
            List<F_SQLiteField> differentFields = new List<F_SQLiteField>();
            foreach (T_SQLiteTable expectedTable in sqlTables)
            {
                // Working only on specific shema part
                if (!expectedTable.Name.StartsWith(tableNamePrefix)) continue;

                missingTables.Add(expectedTable);
                foreach (T_SQLiteTable existingTable in sqlExistingTables)
                {
                    if (existingTable.Name != expectedTable.Name) continue;

                    missingTables.Remove(expectedTable);

                    existingTable.Label = expectedTable.Label;
                    existingTable.Description = expectedTable.Description;
                    existingTable.ReadOnly = expectedTable.ReadOnly;
                    existingTable.Visible = expectedTable.Visible;

                    foreach (F_SQLiteField expectedField in expectedTable.Fields)
                    {
                        missingFields.Add(expectedField);

                        foreach (F_SQLiteField existingField in existingTable.Fields)
                        {
                            if (existingField.Name != expectedField.Name) continue;

                            missingFields.Remove(expectedField);

                            existingField.isRowId = expectedField.isRowId;
                            existingField.isAttachment = expectedField.isAttachment;
                            existingField.ReadOnly = expectedField.ReadOnly;

                            if (existingField.EDbType != expectedField.EDbType || existingField.MaxLength != expectedField.MaxLength)
                            {
                                differentFields.Add(expectedField);
                            }
                        }
                    }
                }
            }

            if (dbVersion == dbCurrentVersion806x)
            // Version is the current one, no difference should exist
            {
                // Trying to correct
                if (missingTables.Count != 0) missingTables = createSchemaTables(ref missingTables);
                if (missingTables.Count != 0)                 
                {
                    if (sqlMissingTables == null) sqlMissingTables = new List<T_SQLiteTable>();
                    sqlMissingTables.AddRange(missingTables);
                    validDB = false;
                }
                if (missingFields.Count != 0) missingFields = createSchemaFields(ref missingFields);
                if (missingFields.Count != 0) 
                {
                    if (sqlMissingFields == null) sqlMissingFields = new List<F_SQLiteField>();
                    sqlMissingFields.AddRange(missingFields);
                    validDB = false;
                }
                if (differentFields.Count != 0) differentFields = updateSchemaFields(ref differentFields);
                if (differentFields.Count != 0)
                {
                    if (sqlDifferentFields == null) sqlDifferentFields = new List<F_SQLiteField>();
                    sqlDifferentFields.AddRange(differentFields);
                    validDB = false;        // Could be corrected, but more complex
                }
                if (!validDB) return;
            }
            else
            // Version is different, it has to be checked and upgraded
            {
                // To be managed
            }
        }

        private T_SQLiteTable findTable<T>()
        {
            if (typeof(T).Namespace != this.GetType().Namespace) return null;
            if (!typeof(T).Name.StartsWith("R_")) return null;
            if (!validDB) return null;

            string tableName = typeof(T).Name.Substring(2);

            foreach (T_SQLiteTable sTable in sqlTables) if (sTable.Name == tableName) return sTable;

            return null;
        }

        public T newRow<T>() where T : new()
        {
            T tRow = default(T);

            T_SQLiteTable sqlTable = findTable<T>();
            if (sqlTable == null) return tRow;

            tRow = new T();

            foreach (F_SQLiteField tField in sqlTable.Fields)
            {
                F_SQLiteField fField = tField.clone();
                if (fField.isRowId) fField.Value = -1;
                typeof(T).GetProperty(fField.Name).SetValue(tRow, fField, null);
            }

            return tRow;
        }

        public long Count<T>() where T : new()
        {
            return Count<T>(string.Empty);
        }

        public long Count<T>(string whereClause) where T : new()
        {
            long lResult = -1;

            T_SQLiteTable sqlTable = findTable<T>();
            if (sqlTable == null) return lResult;

            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource))
            {
                sCon.Open();

                using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                {
                    string fullWhereClause = string.Empty;
                    if (whereClause != string.Empty && whereClause != null)
                    {
                        if (fullWhereClause == string.Empty) fullWhereClause = " WHERE " + whereClause;
                        else fullWhereClause += " AND (" + whereClause + ")";
                    }

                    sCmd.CommandText = string.Format("SELECT COUNT(1) FROM [{0}]{1};", sqlTable.Name, fullWhereClause);
                    try { lResult = (long)sCmd.ExecuteScalar(); }
                    catch { }
                }

                sCon.Close();
            }

            return lResult;
        }

        public Dictionary<int, string[]> ReadLabelDescription<T>() where T : new()
        {
            MethodInfo mGetLabel = typeof(T).GetMethod("getRecordLabel");
            if (mGetLabel == null) return null;
            MethodInfo mGetDescription = typeof(T).GetMethod("getRecordDescription");
            if (mGetDescription == null) return null;

            List<T> rResult = Read<T>(-1, null, string.Empty, string.Empty);
            if (rResult == null) return null;

            Dictionary<int, string[]> lResult = new Dictionary<int, string[]>();
            int rowId = 0;
            PropertyInfo pRowId = typeof(T).GetProperty("RowId");
            foreach (T tObject in rResult)
            {
                if (pRowId == null) rowId++;
                else
                {
                    F_SQLiteField fRowId = (F_SQLiteField)pRowId.GetValue(tObject, null);
                    rowId = fRowId.Value == null ? rowId + 1 : Convert.ToInt32(fRowId.Value);
                }
                lResult.Add(rowId, new string[] { (string)mGetLabel.Invoke(tObject, null), (string)mGetDescription.Invoke(tObject, null) });
            }

            return lResult;
        }

        public List<T> Read<T>() where T : new()
        {
            return Read<T>(-1, null, string.Empty, string.Empty);
        }

        public List<T> Read<T>(int rowId, List<string> ignoredFields, string whereClause, string orderByClause) where T : new()
        {
            List<T> lResult = new List<T>();

            T_SQLiteTable sqlTable = findTable<T>();
            if (sqlTable == null) return lResult;
            
            Type rType = typeof(T);

            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource))
            {
                sCon.Open();

                using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                {
                    List<string> lstFields = new List<string>() {"RowId", "*"};
                    if (ignoredFields != null)
                    {
                        if (ignoredFields.Count > 0)
                        {
                            lstFields = new List<string>() {"RowId"};
                            foreach (F_SQLiteField fField in sqlTable.Fields)
                            {
                                if (fField.isRowId) continue;
                                if (ignoredFields.Contains(fField.Name)) continue;
                                lstFields.Add(fField.Name);
                            }
                        }
                    }
                    string fullWhereClause = string.Empty;
                    if (rowId >= 0) fullWhereClause = " WHERE RowId = " + rowId.ToString();
                    if (whereClause != string.Empty && whereClause != null)
                    {
                        if (fullWhereClause == string.Empty) fullWhereClause = " WHERE " + whereClause;
                        else fullWhereClause += " AND (" + whereClause + ")";
                        
                    }
                    if (orderByClause != string.Empty && orderByClause != null) orderByClause = " ORDER BY " + orderByClause;

                    sCmd.CommandText = string.Format("SELECT {0} FROM [{1}]{2}{3};", string.Join(", ", lstFields.ToArray()), sqlTable.Name, fullWhereClause, orderByClause);
                    lstFields = null;

                    using (SQLiteDataReader sReader = sCmd.ExecuteReader())
                    {
                        if (!sReader.HasRows) return lResult;
                        List<F_SQLiteField> queryFields = null;
                        while (sReader.Read())
                        {
                            if (queryFields == null)
                            {
                                queryFields = new List<F_SQLiteField>();
                                for (int iField = 0; iField < sReader.FieldCount; iField++)
                                {
                                    F_SQLiteField tField = sqlTable.getField(sReader.GetName(iField));
                                    if (tField == null)
                                    {
                                        queryFields.Add(null);
                                        continue;
                                    }
                                    PropertyInfo pInfo = rType.GetProperty(tField.Name);
                                    if (rType == null)
                                    {
                                        queryFields.Add(null);
                                        continue;
                                    }

                                    queryFields.Add(tField);
                                }
                            }

                            T tRow = new T();

                            for (int iField = 0; iField < sReader.FieldCount; iField++)
                            {
                                F_SQLiteField tField = queryFields[iField];
                                if (tField == null) continue;

                                F_SQLiteField fField = tField.clone();
                                fField.Value = sReader.GetValue(iField);
                                rType.GetProperty(fField.Name).SetValue(tRow, fField, null);
                            }
                            lResult.Add(tRow);
                        }
                    }
                }

                sCon.Close();
            }

            return lResult;
        }

        public bool Truncate<T>()
        {
            T_SQLiteTable sqlTable = findTable<T>();
            if (sqlTable == null) return false;

            Type rType = typeof(T);

            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource))
            {
                sCon.Open();

                using (SQLiteTransaction tTran = sCon.BeginTransaction())
                {

                    using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                    {
                        sCmd.CommandText = "DELETE FROM [" + sqlTable.Name + "];";
                        sCmd.ExecuteNonQuery();
                    }

                    tTran.Commit();
                }

                using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                {
                    sCmd.CommandText = "VACUUM;";
                    sCmd.ExecuteNonQuery();
                }

                sCon.Close();
            }

            return true;
        }

        public bool Write<T>(ref List<T> rList)
        {
            T_SQLiteTable sqlTable = findTable<T>();
            if (sqlTable == null) return false;

            Type rType = typeof(T);

            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource))
            {
                sCon.Open();
                
                string updateCmd = "UPDATE [" + sqlTable.Name + "] SET ";
                string insertCmd = "INSERT INTO [" + sqlTable.Name + "] (";
                string lastInsertRowIdCmd = "SELECT LAST_INSERT_ROWID();";

                foreach (F_SQLiteField tField in sqlTable.Fields)
                {
                    if (tField.isRowId) continue;
                    updateCmd += tField.Name + " = :p" + tField.Name.ToUpper() + ", ";
                    insertCmd += tField.Name + ", ";
                }
                updateCmd = updateCmd.Substring(0, updateCmd.Length - 2);
                updateCmd += " WHERE RowId = :pROWID;";
                insertCmd = insertCmd.Substring(0, insertCmd.Length - 2);
                insertCmd += ") VALUES (";

                foreach (F_SQLiteField tField in sqlTable.Fields)
                {
                    if (tField.isRowId) continue;
                    insertCmd += ":p" + tField.Name.ToUpper() + ", ";
                }
                insertCmd = insertCmd.Substring(0, insertCmd.Length - 2);
                insertCmd += ");";

                using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                {
                    using (SQLiteTransaction tTran = sCon.BeginTransaction())
                    {
                        foreach (T rRow in rList)
                        {
                            if (rRow == null) continue;

                            F_SQLiteField rowRowIdField = (F_SQLiteField)rType.GetProperty("RowId").GetValue(rRow, null);
                            bool insertMode = false;
                            if (rowRowIdField.Value == null) insertMode = true;
                            else if (Convert.ToInt64(rowRowIdField.Value) <= 0) insertMode = true;

                            if (insertMode) sCmd.CommandText = insertCmd;
                            else sCmd.CommandText = updateCmd;

                            foreach (F_SQLiteField tField in sqlTable.Fields)
                            {
                                if (insertMode && tField.Name.ToLower() == rowRowIdField.Name.ToLower()) continue;
                                F_SQLiteField fField = (F_SQLiteField)rType.GetProperty(tField.Name).GetValue(rRow, null);
                                sCmd.Parameters.Add("p" + tField.Name.ToUpper(), tField.EDbType).Value = fField.Value;
                            }

                            sCmd.ExecuteNonQuery();

                            if (insertMode)
                            {
                                sCmd.CommandText = lastInsertRowIdCmd;
                                rowRowIdField.Value = sCmd.ExecuteScalar();
                            }
                        }
                        tTran.Commit();
                    }
                }

                sCon.Close();
            }

            return true;
        }

        public bool Delete<T>(List<T> rList)
        {
            T_SQLiteTable sqlTable = findTable<T>();
            if (sqlTable == null) return false;

            Type rType = typeof(T);

            bool bResult = true;

            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource))
            {
                try { sCon.Open(); }
                catch { return false; }

                using (SQLiteTransaction tTran = sCon.BeginTransaction())
                {
                    foreach (T rRow in rList)
                    {
                        if (((F_SQLiteField)rType.GetProperty("RowId").GetValue(rRow, null)).Value == null) continue;

                        using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                        {
                            sCmd.CommandText = "DELETE FROM [" + sqlTable.Name + "] WHERE RowId = " + Convert.ToInt32(((F_SQLiteField)rType.GetProperty("RowId").GetValue(rRow, null)).Value).ToString();
                            try { sCmd.ExecuteNonQuery(); }
                            catch { bResult = false; }
                        }
                    }
                    try { tTran.Commit(); }
                    catch { bResult = false; }
                }

                if (bResult && rList.Count > 1)
                {
                    using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                    {
                        sCmd.CommandText = "VACUUM;";
                        try { sCmd.ExecuteNonQuery(); }
                        catch { bResult = false; }
                    }
                }

                try { sCon.Close(); }
                catch { }
            }

            return bResult;
        }
    }

    public class T_SQLiteTable
    {
        private string name = string.Empty;
        public string Name { get { return name; } }

        private Type type = null;
        public Type Type { get { return type; } }

        private bool readOnly = false;
        public bool ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        private bool visible = false;
        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

        private string label = string.Empty;
        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        private string description = string.Empty;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private List<F_SQLiteField> fields = null;
        public List<F_SQLiteField> Fields { get { return fields; } }

        public T_SQLiteTable(string tName, Type tType, string sLabel, string sDescription, bool bReadOnly, bool bVisible, bool bInitFields)
        {
            name = tName;
            type = tType;
            label = sLabel;
            description = sDescription;
            readOnly = bReadOnly;
            visible = bVisible;

            fields = new List<F_SQLiteField>();

            if (!bInitFields) return;

            MethodInfo mMethod = type.GetMethod("initFields");
            if (mMethod == null) return;
            object rObject = Activator.CreateInstance(type);
            mMethod.Invoke(rObject, null);

            foreach (PropertyInfo pInfo in type.GetProperties())
            {
                F_SQLiteField fField = (F_SQLiteField)pInfo.GetValue(rObject, null);
                if (fField == null) continue;
                fields.Add(fField);
            }

            rObject = null;
        }

        public F_SQLiteField getField(string fieldName)
        {
            foreach (F_SQLiteField fField in fields) if (fField.Name.ToLower() == fieldName.ToLower()) return fField;

            return null;
        }

        public string toString()
        {
            return Name;
        }
    }

    public class F_SQLiteField
    {
        private string tableName = string.Empty;
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        private string name = string.Empty;
        public string Name { get { return name; } }

        public bool isRowId { get; set; }

        public bool isAttachment { get; set; }

        public bool ReadOnly { get; set; }

        private string schemaType = string.Empty;
        public string SchemaType
        {
            get { return schemaType; }
            set
            {
                schemaType = value;
                if (this.schemaType.Contains("(") && this.schemaType.EndsWith(")"))
                {
                    try
                    {
                        this.SchemaShortType = this.schemaType.Split(' ')[0].Trim();
                        this.maxLength = Convert.ToInt32(this.schemaType.Split(' ')[1].Trim().Replace("(", "").Replace(")", "").Trim());
                    }
                    catch { }
                }
                else
                {
                    this.SchemaShortType = this.schemaType;
                }
            }
        }

        private string schemaShortType = string.Empty;
        public string SchemaShortType
        {
            get { return schemaShortType; }
            set
            {
                this.schemaShortType = value;
                switch (this.schemaShortType)
                {
                    case "INT":
                    case "INTEGER":
                    case "NUMERIC":
                        this.type = typeof(int);
                        this.edbType = DbType.Int32;
                        break;
                    case "BIGINT":
                        this.type = typeof(Int64);
                        this.edbType = DbType.Int64;
                        break;
                    case "DECIMAL":
                    case "DOUBLE":
                    case "REAL":
                        this.type = typeof(double);
                        this.edbType = DbType.Double;
                        break;
                    case "CHAR":
                    case "NVARCHAR":
                    case "VARCHAR":
                    case "TEXT":
                        this.type = typeof(string);
                        this.edbType = DbType.String;
                        break;
                    case "DATE":
                    case "TIME":
                    case "DATETIME":
                        this.type = typeof(DateTime);
                        this.edbType = DbType.DateTime;
                        break;
                    case "BOOLEAN":
                        this.type = typeof(bool);
                        this.edbType = DbType.Boolean;
                        break;
                    case "BLOB":
                        this.type = typeof(object);
                        this.edbType = DbType.Binary;
                        break;
                    default:
                        this.type = typeof(object);
                        this.edbType = DbType.Object;
                        break;
                }
            }
        }

        private Type type = null;
        public Type Type { get { return type; } }

        private DbType edbType = DbType.Object;
        public DbType EDbType { get { return edbType; } }

        private int maxLength = 0;
        public int MaxLength { get { return maxLength; } }

        private object value = null;
        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public object ValueConverted
        {
            get
            {
                switch (edbType)
                {
                    case DbType.Int32:
                        try { return value == null || value == DBNull.Value ? (int)0 : Convert.ToInt32(value); }
                        catch { return (int)0; }
                    case DbType.Int64:
                        try { return value == null || value == DBNull.Value ? (long)0 : Convert.ToInt64(value); }
                        catch { return (long)0; }
                    case DbType.Double:
                        try { return value == null || value == DBNull.Value ? (double)0.0 : Convert.ToDouble(value); }
                        catch { return (double)0.0; }
                    case DbType.String:
                        try { return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value); }
                        catch { return string.Empty; }
                    case DbType.DateTime:
                        try { return value == null || value == DBNull.Value ? new DateTime(2000, 1, 1, 0, 0, 0) : Convert.ToDateTime(value); }
                        catch { return new DateTime(2000, 1, 1, 0, 0, 0); }
                    case DbType.Boolean:
                        try { return value == null || value == DBNull.Value ? false : Convert.ToBoolean(value); }
                        catch { return false; }
                    default:
                        return value;
                }
            }
        }

        public F_SQLiteField(string fTableName, string fName, string fSchemaType)
        {
            this.tableName = fTableName;
            this.name = fName;
            this.maxLength = 0;

            this.SchemaType = fSchemaType;
        }

        public F_SQLiteField clone()
        {
            F_SQLiteField clone = new F_SQLiteField(this.tableName, this.name, this.schemaType);

            return clone;
        }

        public new string ToString()
        {
            return Name;
        }
    }

    public class F_SQLiteFieldProperties
    {
        public string SchemaType { get; set; }

        public bool isRowId { get; set; }

        public bool isAttachment { get; set; }

        public bool ReadOnly { get; set; }

        public F_SQLiteFieldProperties(string fSchemaType)
        {
            this.SchemaType = fSchemaType;
            this.isRowId = false;
            this.isAttachment = false;
            this.ReadOnly = false;
        }

        public F_SQLiteFieldProperties(string fSchemaType, bool bReadOnly)
        {
            this.SchemaType = fSchemaType;
            this.isRowId = false;
            this.isAttachment = false;
            this.ReadOnly = true;
        }

        public F_SQLiteFieldProperties(string fSchemaType, bool bIsAttachment, bool bReadOnly)
        {
            this.SchemaType = fSchemaType;
            this.isRowId = false;
            this.isAttachment = bIsAttachment;
            this.ReadOnly = bReadOnly;
        }

        public F_SQLiteFieldProperties(string fSchemaType, bool bIsRowId, bool bIsAttachment, bool bReadOnly)
        {
            this.SchemaType = fSchemaType;
            this.isRowId = bIsRowId;
            this.isAttachment = bIsAttachment;
            this.ReadOnly = bReadOnly || this.isRowId;
        }

        public F_SQLiteFieldProperties clone()
        {
            F_SQLiteFieldProperties clone = new F_SQLiteFieldProperties(this.SchemaType, this.isRowId, this.isAttachment, this.ReadOnly);

            return clone;
        }
    }

    public class S_SQLiteSyncS6x
    {
        public object SqLite806xObject { get; set; }
        public object SqLiteSAD806xObject { get; set; }
        public object S6xObject { get; set; }

        public object S6xParentObject { get; set; }
        public object S6xParentParentObject { get; set; }

        public int BankNum { get; set; }
        public int Address { get; set; }
        public int DuplicateNum { get; set; }

        public int RegisterAddress { get; set; }
        public string RegisterAdditionalAddress10 { get; set; }
        public string RegisterUniqueAddress
        {
            get
            {
                if (RegisterAddress < 0) return string.Empty;
                if (RegisterAdditionalAddress10 == null || RegisterAdditionalAddress10 == string.Empty) return SAD806x.Tools.RegisterUniqueAddress(RegisterAddress);
                return SAD806x.Tools.RegisterUniqueAddress(RegisterAddress) + SAD806x.SADDef.AdditionSeparator + RegisterAdditionalAddress10;
            }
        }

        public string UniqueKey { get; set; }
        public string SubUniqueKey { get; set; }

        public int SubPosition { get; set; }

        public bool SyncValid { get; set; }

        public bool isDuplicate { get { return DuplicateNum > 0; } }
        public string UniqueAddress { get { return SAD806x.Tools.UniqueAddress(BankNum, Address); } }
        public string DuplicateAddress { get { return SAD806x.Tools.DuplicateUniqueAddress(BankNum, Address, DuplicateNum); } }

        public DateTime SyncDateTimeMinValue { get { return new DateTime(2000, 1, 1, 0, 0, 0); } }
        
        public string SyncUniqueKey
        {
            get
            {
                string syncUniqueKey = string.Empty;

                if (UniqueKey != null && UniqueKey != string.Empty) syncUniqueKey = UniqueKey;
                else if (RegisterUniqueAddress != null && RegisterUniqueAddress != string.Empty) syncUniqueKey = RegisterUniqueAddress;
                else if (isDuplicate) syncUniqueKey = DuplicateAddress;
                else syncUniqueKey = UniqueAddress;

                if (SubUniqueKey != null && SubUniqueKey != string.Empty) syncUniqueKey += "." + SubUniqueKey;
                if (SubPosition != int.MinValue) syncUniqueKey += "." + SubPosition.ToString();

                return syncUniqueKey;
            }
        }

        public DateTime SqLite806xDateCreated
        {
            get
            {
                if (SqLite806xObject == null) return SyncDateTimeMinValue;
                PropertyInfo piPI = SqLite806xObject.GetType().GetProperty("DateCreated");
                if (piPI == null) return SyncDateTimeMinValue;
                F_SQLiteField fField = (F_SQLiteField)piPI.GetValue(SqLite806xObject, null);
                if (fField == null) return SyncDateTimeMinValue;
                DateTime dtDateTime = (DateTime)fField.ValueConverted;
                
                if (dtDateTime < SyncDateTimeMinValue) return SyncDateTimeMinValue;
                else return dtDateTime;
            }
        }
        public DateTime SqLite806xDateUpdated
        {
            get
            {
                if (SqLite806xObject == null) return SyncDateTimeMinValue;
                PropertyInfo piPI = SqLite806xObject.GetType().GetProperty("DateUpdated");
                if (piPI == null) return SyncDateTimeMinValue;
                F_SQLiteField fField = (F_SQLiteField)piPI.GetValue(SqLite806xObject, null);
                if (fField == null) return SyncDateTimeMinValue;
                DateTime dtDateTime = (DateTime)fField.ValueConverted;

                if (dtDateTime < SyncDateTimeMinValue) return SyncDateTimeMinValue;
                else return dtDateTime;
            }
        }
        public DateTime SqLiteSAD806xDateCreated
        {
            get
            {
                if (SqLiteSAD806xObject == null) return SyncDateTimeMinValue;
                PropertyInfo piPI = SqLiteSAD806xObject.GetType().GetProperty("DateCreated");
                if (piPI == null) return SyncDateTimeMinValue;
                F_SQLiteField fField = (F_SQLiteField)piPI.GetValue(SqLiteSAD806xObject, null);
                if (fField == null) return SyncDateTimeMinValue;
                DateTime dtDateTime = (DateTime)fField.ValueConverted;

                if (dtDateTime < SyncDateTimeMinValue) return SyncDateTimeMinValue;
                else return dtDateTime;
            }
        }
        public DateTime SqLiteSAD806xDateUpdated
        {
            get
            {
                if (SqLiteSAD806xObject == null) return SyncDateTimeMinValue;
                PropertyInfo piPI = SqLiteSAD806xObject.GetType().GetProperty("DateUpdated");
                if (piPI == null) return SyncDateTimeMinValue;
                F_SQLiteField fField = (F_SQLiteField)piPI.GetValue(SqLiteSAD806xObject, null);
                if (fField == null) return SyncDateTimeMinValue;
                DateTime dtDateTime = (DateTime)fField.ValueConverted;

                if (dtDateTime < SyncDateTimeMinValue) return SyncDateTimeMinValue;
                else return dtDateTime;
            }
        }
        public DateTime S6xDateCreated
        {
            get
            {
                if (S6xObject == null) return SyncDateTimeMinValue;
                PropertyInfo piPI = S6xObject.GetType().GetProperty("DateCreated");
                if (piPI == null) return SyncDateTimeMinValue;
                DateTime dtDateTime = (DateTime)piPI.GetValue(S6xObject, null);

                if (dtDateTime < SyncDateTimeMinValue) return SyncDateTimeMinValue;
                else return dtDateTime;
            }
        }
        public DateTime S6xDateUpdated
        {
            get
            {
                if (S6xObject == null) return SyncDateTimeMinValue;
                PropertyInfo piPI = S6xObject.GetType().GetProperty("DateUpdated");
                if (piPI == null) return SyncDateTimeMinValue;
                DateTime dtDateTime = (DateTime)piPI.GetValue(S6xObject, null);

                if (dtDateTime < SyncDateTimeMinValue) return SyncDateTimeMinValue;
                else return dtDateTime;
            }
        }

        public bool SyncIgnore
        {
            get
            {
                if (S6xDateUpdated == SqLite806xDateUpdated) return true;
                if (S6xDateUpdated == SqLiteSAD806xDateUpdated) return true;
                return false;
            }
        }

        public bool SyncCompare
        {
            get
            {
                return S6xObject != null && (SqLite806xObject != null || SqLiteSAD806xObject != null);
            }
        }

        public bool SyncS6xMaster
        {
            get
            {
                return SyncIgnore || !SyncCompare ? false : S6xDateUpdated > SqLite806xDateUpdated;
            }
        }

        public bool SyncSqLite806xMaster
        {
            get
            {
                return SyncIgnore || !SyncCompare ? false : SqLite806xDateUpdated > S6xDateUpdated;
            }
        }

        public bool SyncS6xCreation
        {
            get
            {
                return !SyncCompare && S6xObject == null;
            }
        }

        public bool SyncS6xCreationCancelled(DateTime dtLastSync)
        {
            if (!SyncS6xCreation) return true;
            if (dtLastSync == new DateTime(2000, 1, 1, 0, 0, 0)) return false;
            if (dtLastSync >= SqLite806xDateCreated && dtLastSync >= SqLite806xDateUpdated && dtLastSync >= SqLiteSAD806xDateCreated && dtLastSync >= SqLiteSAD806xDateUpdated) return true;
            return false;
        }

        public bool SyncS6xCreationDoubt(DateTime dtLastSync)
        {
            return (dtLastSync == new DateTime(2000, 1, 1, 0, 0, 0));
        }

        public bool SyncSqLite806xCreation
        {
            get
            {
                return !SyncCompare && (SqLite806xObject == null || SqLiteSAD806xObject == null);
            }
        }

        public bool SyncSqLite806xCreationCancelled(DateTime dtLastSync)
        {
            if (!SyncSqLite806xCreation) return true;
            if (dtLastSync == new DateTime(2000, 1, 1, 0, 0, 0)) return false;
            if (dtLastSync >= S6xDateCreated && dtLastSync >= S6xDateUpdated) return true;
            return false;
        }

        public bool SyncSqLite806xCreationDoubt(DateTime dtLastSync)
        {
            return (dtLastSync == new DateTime(2000, 1, 1, 0, 0, 0));
        }

        public bool SyncSqLiteSAD806xCreation
        {
            get
            {
                return !SyncCompare && SqLiteSAD806xObject == null;
            }
        }

        public bool SyncSqLiteSAD806xCreationCancelled(DateTime dtLastSync)
        {
            if (!SyncSqLiteSAD806xCreation) return true;
            if (dtLastSync == new DateTime(2000, 1, 1, 0, 0, 0)) return false;
            if (dtLastSync >= S6xDateCreated && dtLastSync >= S6xDateUpdated) return true;
            return false;
        }

        public bool SyncSqLiteSAD806xCreationDoubt(DateTime dtLastSync)
        {
            return (dtLastSync == new DateTime(2000, 1, 1, 0, 0, 0));
        }

        public S_SQLiteSyncS6x()
        {
        }

        public S_SQLiteSyncS6x(object sqLiteRow, bool isSqLite806xObject)
        {
            if (sqLiteRow == null) return;

            Type s6xType = sqLiteRow.GetType();

            PropertyInfo piPI = null;
            object oField = null;
            string sValue = string.Empty;

            BankNum = int.MinValue;
            piPI = s6xType.GetProperty("Bank");
            if (piPI == null) piPI = s6xType.GetProperty("ScalarBank");
            if (piPI == null) piPI = s6xType.GetProperty("RoutineBank");
            if (piPI != null)
            {
                oField = piPI.GetValue(sqLiteRow, null);
                if (oField != null)
                {
                    if (oField.GetType() == typeof(F_SQLiteField)) BankNum = (int)((F_SQLiteField)oField).ValueConverted;
                }
            }

            Address = int.MinValue;
            piPI = s6xType.GetProperty("Address");
            if (piPI == null) piPI = s6xType.GetProperty("ScalarAddress");
            if (piPI == null) piPI = s6xType.GetProperty("RoutineAddress");
            if (piPI == null) piPI = s6xType.GetProperty("RegisterAddress");
            if (piPI != null)
            {
                oField = piPI.GetValue(sqLiteRow, null);
                if (oField != null)
                {
                    if (oField.GetType() == typeof(F_SQLiteField)) Address = (int)((F_SQLiteField)oField).ValueConverted;
                }
            }

            RegisterAddress = -1;
            RegisterAdditionalAddress10 = string.Empty;
            piPI = s6xType.GetProperty("AddressAdder"); // Registers only
            if (piPI == null) piPI = s6xType.GetProperty("RegisterAddressAdder");
            if (piPI != null)
            {
                RegisterAddress = Address;
                Address = int.MinValue;

                oField = piPI.GetValue(sqLiteRow, null);
                if (oField != null)
                {
                    if (oField.GetType() == typeof(F_SQLiteField)) RegisterAdditionalAddress10 = (string)((F_SQLiteField)oField).ValueConverted;
                }
            }

            DuplicateNum = 0;
            piPI = s6xType.GetProperty("UniqueAddCode");
            if (piPI == null) piPI = s6xType.GetProperty("ScalarUniqueAddCode");
            if (piPI == null) piPI = s6xType.GetProperty("RoutineUniqueAddCode");
            if (piPI != null)
            {
                oField = piPI.GetValue(sqLiteRow, null);
                if (oField != null)
                {
                    if (oField.GetType() == typeof(F_SQLiteField))
                    {
                        sValue = (string)((F_SQLiteField)oField).ValueConverted;
                        if (sValue != string.Empty) DuplicateNum = Convert.ToInt32(sValue);
                    }
                }
            }

            UniqueKey = string.Empty;
            SubUniqueKey = string.Empty;
            piPI = s6xType.GetProperty("SignatureRoutineUniqueKey");
            if (piPI == null) piPI = s6xType.GetProperty("SignatureElementUniqueKey");
            if (piPI == null) piPI = s6xType.GetProperty("InternalScalarUniqueKey");
            if (piPI != null)
            {
                oField = piPI.GetValue(sqLiteRow, null);
                if (oField != null)
                {
                    if (oField.GetType() == typeof(F_SQLiteField)) UniqueKey = (string)((F_SQLiteField)oField).ValueConverted;
                }
            }

            piPI = s6xType.GetProperty("UniqueKey");
            if (piPI != null)
            {
                oField = piPI.GetValue(sqLiteRow, null);
                if (oField != null)
                {
                    if (oField.GetType() == typeof(F_SQLiteField))
                    {
                        sValue = (string)((F_SQLiteField)oField).ValueConverted;
                        if (UniqueKey == string.Empty && Address == int.MinValue && (RegisterUniqueAddress == string.Empty || RegisterUniqueAddress == null))
                        {
                            UniqueKey = sValue;
                        }
                        else
                        {
                            SubUniqueKey = sValue;
                        }
                    }
                }
            }

            SubPosition = int.MinValue;
            piPI = s6xType.GetProperty("Position");
            if (piPI != null)
            {
                oField = piPI.GetValue(sqLiteRow, null);
                if (oField != null)
                {
                    if (oField.GetType() == typeof(F_SQLiteField)) SubPosition = (int)((F_SQLiteField)oField).ValueConverted;
                }
            }

            piPI = null;

            if (isSqLite806xObject) SqLite806xObject = sqLiteRow;
            else SqLiteSAD806xObject = sqLiteRow;

            SyncValid = true;
        }

        public S_SQLiteSyncS6x(object s6xObject)
        {
            if (s6xObject == null) return;

            Type s6xType = s6xObject.GetType();

            PropertyInfo piPI = null;

            SubPosition = int.MinValue;

            BankNum = -1;
            piPI = s6xType.GetProperty("BankNum");
            if (piPI != null) BankNum = (int)piPI.GetValue(s6xObject, null);

            Address = -1;
            piPI = s6xType.GetProperty("AddressInt");
            if (piPI != null) Address = (int)piPI.GetValue(s6xObject, null);

            piPI = s6xType.GetProperty("DuplicateNum");
            DuplicateNum = 0;
            if (piPI != null) DuplicateNum = (int)piPI.GetValue(s6xObject, null);

            piPI = s6xType.GetProperty("UniqueKey");
            if (piPI != null) UniqueKey = (string)piPI.GetValue(s6xObject, null);

            RegisterAddress = -1;
            if (s6xType == typeof(SAD806x.S6xRegister))
            {
                piPI = s6xType.GetProperty("AddressInt");
                if (piPI != null) RegisterAddress = (int)piPI.GetValue(s6xObject, null);

                piPI = s6xType.GetProperty("AdditionalAddress10");
                if (piPI != null) RegisterAdditionalAddress10 = (string)piPI.GetValue(s6xObject, null);
            }

            piPI = null;

            S6xObject = s6xObject;

            SyncValid = true;
        }

        public S_SQLiteSyncS6x(object s6xParentObject, object s6xObject)
        {
            if (s6xObject == null) return;

            S_SQLiteSyncS6x syncParentObject = new S_SQLiteSyncS6x(s6xParentObject);
            if (!syncParentObject.SyncValid) return;

            BankNum = syncParentObject.BankNum;
            Address = syncParentObject.Address;
            DuplicateNum = syncParentObject.DuplicateNum;
            UniqueKey = syncParentObject.UniqueKey;
            RegisterAddress = syncParentObject.RegisterAddress;
            RegisterAdditionalAddress10 = syncParentObject.RegisterAdditionalAddress10;

            S6xParentObject = s6xParentObject;

            Type s6xType = s6xObject.GetType();

            PropertyInfo piPI = null;

            SubPosition = int.MinValue;
            piPI = s6xType.GetProperty("Position");
            if (piPI != null) SubPosition = (int)piPI.GetValue(s6xObject, null);

            SubUniqueKey = string.Empty;
            if (SubPosition == int.MinValue)    // BitFlag have Position and UniqueKey, only Position is good enough
            {
                piPI = s6xType.GetProperty("UniqueKey");
                if (piPI != null) SubUniqueKey = (string)piPI.GetValue(s6xObject, null);
            }

            piPI = null;

            S6xObject = s6xObject;

            SyncValid = true;
        }

        public S_SQLiteSyncS6x(object s6xParentParentObject, object s6xParentObject, object s6xObject)
        {
            if (s6xObject == null) return;

            S_SQLiteSyncS6x syncParentObject = new S_SQLiteSyncS6x(s6xParentParentObject, s6xParentObject);
            if (!syncParentObject.SyncValid) return;

            BankNum = syncParentObject.BankNum;
            Address = syncParentObject.Address;
            DuplicateNum = syncParentObject.DuplicateNum;
            UniqueKey = syncParentObject.UniqueKey;
            RegisterAddress = syncParentObject.RegisterAddress;
            RegisterAdditionalAddress10 = syncParentObject.RegisterAdditionalAddress10;
            SubUniqueKey = syncParentObject.SubUniqueKey;

            S6xParentParentObject = s6xParentParentObject;
            S6xParentObject = s6xParentObject;

            Type s6xType = s6xObject.GetType();

            PropertyInfo piPI = null;

            SubPosition = int.MinValue;
            piPI = s6xType.GetProperty("Position");
            if (piPI != null) SubPosition = (int)piPI.GetValue(s6xObject, null);
            piPI = null;

            S6xObject = s6xObject;

            SyncValid = true;
        }
    }

    // Base for table objects definitions
    public class R_SQLite_Core
    {
        public F_SQLiteField RowId { get; set; }

        public string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (RowId != null) if (RowId.Value != null) sParts.Add(RowId.Value.ToString());

            return string.Join(" - ", sParts.ToArray());
        }

        public string getRecordDescription()
        {
            List<string> sParts = new List<string>();

            return string.Join("\r\n", sParts.ToArray());
        }

        public Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = new Dictionary<string, F_SQLiteFieldProperties>();
            dResult.Add("RowId", new F_SQLiteFieldProperties("INT", true, true, true));
            return dResult;
        }

        public void initFields()
        {
            string tableName = SQLite806xTools.TableNameFromType(this.GetType());
            if (tableName == string.Empty) return;

            MethodInfo mMethod = this.GetType().GetMethod("getFieldsSchemaTypes");
            Dictionary<string, F_SQLiteFieldProperties> dFieldsSchemaTypes = (Dictionary<string, F_SQLiteFieldProperties>)mMethod.Invoke(this, null);

            foreach (PropertyInfo pInfo in this.GetType().GetProperties())
            {
                string fieldName = pInfo.Name;
                if (!dFieldsSchemaTypes.ContainsKey(fieldName)) continue;
                F_SQLiteFieldProperties fProperies = dFieldsSchemaTypes[fieldName];
                if (fProperies == null) continue;
                
                F_SQLiteField fField = (F_SQLiteField)pInfo.GetValue(this, null);
                if (fField == null) fField = new F_SQLiteField(tableName, fieldName, fProperies.SchemaType);
                else
                {
                    fField.TableName = tableName;
                    fField.SchemaType = fProperies.SchemaType;
                }
                fField.isRowId = fProperies.isRowId;
                fField.isAttachment = fProperies.isAttachment;
                fField.ReadOnly = fProperies.ReadOnly;
                pInfo.SetValue(this, fField, null);
            }
        }
    }

    public class R_SQLite_DB_Information : R_SQLite_Core
    {
        public F_SQLiteField Version { get; set; }
        public F_SQLiteField Date { get; set; }
        public F_SQLiteField Comments { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (Version != null) if (Version.Value != null) if (Version.Value.ToString() != string.Empty) sParts.Add(Version.Value.ToString());
            if (Date != null) if (Date.Value != null) sParts.Add(Convert.ToDateTime(Date.Value).ToString("yyyy-MM-dd"));

            return string.Join(" - ", sParts.ToArray());
        }

        public new string getRecordDescription()
        {
            List<string> sParts = new List<string>();
            if (Comments != null) if (Comments.Value != null) if (Comments.Value.ToString() != string.Empty) sParts.Add(Comments.Value.ToString());

            return string.Join("\r\n", sParts.ToArray());
        }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Version", new F_SQLiteFieldProperties("NVARCHAR (10)", true));
            dResult.Add("Date", new F_SQLiteFieldProperties("DATETIME", true));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT", true));
            return dResult;
        }
    }

    public class R_SQLite_Files : R_SQLite_Core
    {
        public F_SQLiteField SortNumber { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField EntryCreation { get; set; }
        public F_SQLiteField EntryModification { get; set; }
        public F_SQLiteField File { get; set; }
        public F_SQLiteField FileName { get; set; }
        public F_SQLiteField FileSize { get; set; }
        public F_SQLiteField FileCreation { get; set; }
        public F_SQLiteField FileModification { get; set; }
        public F_SQLiteField FileExtension { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (FileName != null) if (FileName.Value != null) if (FileName.Value.ToString() != string.Empty) sParts.Add(FileName.Value.ToString());

            return string.Join(" - ", sParts.ToArray());
        }

        public new string getRecordDescription()
        {
            List<string> sParts = new List<string>();
            if (Comments != null) if (Comments.Value != null) if (Comments.Value.ToString() != string.Empty) sParts.Add(Comments.Value.ToString());

            return string.Join("\r\n", sParts.ToArray());
        }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SortNumber", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Description", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("EntryCreation", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("EntryModification", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("File", new F_SQLiteFieldProperties("BLOB", true, false));
            dResult.Add("FileName", new F_SQLiteFieldProperties("NVARCHAR (500)", true, false));
            dResult.Add("FileSize", new F_SQLiteFieldProperties("INT (10)", true, false));
            dResult.Add("FileCreation", new F_SQLiteFieldProperties("DATETIME", true, false));
            dResult.Add("FileModification", new F_SQLiteFieldProperties("DATETIME", true, false));
            dResult.Add("FileExtension", new F_SQLiteFieldProperties("NVARCHAR (100)", true, false));
            return dResult;
        }
    }
}
