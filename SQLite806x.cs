using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Reflection;

namespace SQLite806x
{
    public class SQLite806xDB
    {
        private string dbFilePath = string.Empty;
        private string dbDataSource = string.Empty;

        private bool validDB = false;
        public bool ValidDB { get { return validDB; } }

        private List<T_SQLiteTable> sqlTables = null;

        public SQLite806xDB(string filePath)
        {
            dbFilePath = filePath;
            dbDataSource = "Data Source=" + filePath + ";";

            readSchema();
        }

        public void readSchema()
        {
            validDB = false;

            if (!File.Exists(dbFilePath)) return;

            sqlTables = new List<T_SQLiteTable>();

            using (SQLiteConnection sCon = new SQLiteConnection(dbDataSource))
            {
                sCon.Open();

                using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                {
                    sCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
                    using (SQLiteDataReader sReader = sCmd.ExecuteReader())
                    {
                        if (!sReader.HasRows) return;
                        while (sReader.Read())
                        {
                            string tableName = sReader.GetString(0);
                            string @namespace = "SQLite806x";
                            string @rowClass = "R_" + tableName;
                            Type tableClassType = Type.GetType(string.Format("{0}.{1}", @namespace, @rowClass));
                            if (tableClassType == null) continue;

                            sqlTables.Add(new T_SQLiteTable(tableName));
                        }
                    }
                }

                foreach (T_SQLiteTable sqlTable in sqlTables)
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

                sCon.Close();

                // TO BE REVIEWED
                validDB = true;
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
                typeof(T).GetProperty(fField.Name).SetValue(tRow, fField, null);
            }

            return tRow;
        }
        
        public List<T> Read<T>() where T : new()
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
                    sCmd.CommandText = "SELECT RowId, * FROM [" + sqlTable.Name + "];";
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

                using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                {
                    sCmd.CommandText = "DELETE FROM [" + sqlTable.Name + "];";
                    sCmd.ExecuteNonQuery();
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

                foreach (F_SQLiteField tField in sqlTable.Fields)
                {
                    if (tField.Name.ToLower() == "rowid") continue;
                    updateCmd += tField.Name + " = :p" + tField.Name.ToUpper() + ", ";
                    insertCmd += tField.Name + ", ";
                }
                updateCmd = updateCmd.Substring(0, updateCmd.Length - 2);
                updateCmd += " WHERE RowId = :pROWID;";
                insertCmd = insertCmd.Substring(0, insertCmd.Length - 2);
                insertCmd += ") VALUES (";

                foreach (F_SQLiteField tField in sqlTable.Fields)
                {
                    if (tField.Name.ToLower() == "rowid") continue;
                    insertCmd += ":p" + tField.Name.ToUpper() + ", ";
                }
                insertCmd = insertCmd.Substring(0, insertCmd.Length - 2);
                insertCmd += ");";

                foreach (T rRow in rList)
                {
                    bool insertMode = ((F_SQLiteField)rType.GetProperty("RowId").GetValue(rRow, null)).Value == null;

                    using (SQLiteCommand sCmd = new SQLiteCommand(sCon))
                    {
                        if (insertMode) sCmd.CommandText = insertCmd;
                        else sCmd.CommandText = updateCmd;

                        foreach (F_SQLiteField tField in sqlTable.Fields)
                        {
                            if (insertMode && tField.Name.ToLower() == "rowid") continue;
                            F_SQLiteField fField = (F_SQLiteField)rType.GetProperty(tField.Name).GetValue(rRow, null);
                            sCmd.Parameters.Add("p" + tField.Name.ToUpper(), tField.EDbType).Value = fField.Value; 
                        }

                        sCmd.ExecuteNonQuery();
                    }
                }

                sCon.Close();
            }

            return true;
        }
    }

    public class T_SQLiteTable
    {
        private string name = string.Empty;
        public string Name { get { return name; } }

        private List<F_SQLiteField> fields = null;
        public List<F_SQLiteField> Fields { get { return fields; } }

        public T_SQLiteTable(string tName)
        {
            name = tName;
            fields = new List<F_SQLiteField>();
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
        public string TableName { get { return tableName; } }

        private string name = string.Empty;
        public string Name { get { return name; } }

        private string schemaType = string.Empty;
        public string SchemaType { get { return schemaType; } }

        private string schemaShortType = string.Empty;
        public string SchemaShortType { get { return schemaShortType; } }

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

        public F_SQLiteField(string fTableName, string fName, string fSchemaType)
        {
            this.tableName = fTableName;
            this.name = fName;
            this.schemaType = fSchemaType;

            this.schemaShortType = this.schemaType;
            this.maxLength = 0;

            if (this.schemaType.Contains("(") && this.schemaType.EndsWith(")"))
            {
                try
                {
                    this.schemaShortType = this.schemaType.Split(' ')[0].Trim();
                    this.maxLength = Convert.ToInt32(this.schemaType.Split(' ')[1].Trim().Replace("(", "").Replace(")", "").Trim());
                }
                catch { }
            }

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

        public F_SQLiteField clone()
        {
            F_SQLiteField clone = new F_SQLiteField(this.tableName, this.name, this.schemaType);

            return clone;
        }

        public string toString()
        {
            return Name;
        }
    }

    public class R_SQLite_Core
    {
        public F_SQLiteField RowId { get; set; }
    }

    public class R_806x_DB_Information : R_SQLite_Core
    {
        public F_SQLiteField Version { get; set; }
        public F_SQLiteField Date { get; set; }
        public F_SQLiteField Comments { get; set; }
    }

    public class R_806x_Elements_Core : R_SQLite_Core
    {
        public F_SQLiteField Bank { get; set; }
        public F_SQLiteField Address { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField ShortLabel { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField Category_A { get; set; }
        public F_SQLiteField Category_B { get; set; }
        public F_SQLiteField Category_C { get; set; }
        public F_SQLiteField Status { get; set; }
    }

    public class R_806x_Def_Functions : R_806x_Elements_Core
    {
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Rows { get; set; }
        public F_SQLiteField InputSigned { get; set; }
        public F_SQLiteField InputUnits { get; set; }
        public F_SQLiteField InputScaleExpression { get; set; }
        public F_SQLiteField InputScalePrecision { get; set; }
        public F_SQLiteField OutputSigned { get; set; }
        public F_SQLiteField OutputUnits { get; set; }
        public F_SQLiteField OutputScaleExpression { get; set; }
        public F_SQLiteField OutputScalePrecision { get; set; }
    }

    public class R_806x_Def_Operations : R_806x_Elements_Core
    {
    }

    public class R_806x_Def_Others : R_SQLite_Core
    {
        public F_SQLiteField Bank { get; set; }
        public F_SQLiteField Address { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField Comments { get; set; }
    }

    public class R_806x_Def_Registers : R_SQLite_Core
    {
        public F_SQLiteField Address { get; set; }
        public F_SQLiteField AddressAdder { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField ShortLabel { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField ByteLabel { get; set; }
        public F_SQLiteField WordLabel { get; set; }
        public F_SQLiteField Category_A { get; set; }
        public F_SQLiteField Category_B { get; set; }
        public F_SQLiteField Category_C { get; set; }
        public F_SQLiteField Status { get; set; }
    }

    public class R_806x_Def_RegistersBitFlags : R_SQLite_Core
    {
        public F_SQLiteField RegisterAddress { get; set; }
        public F_SQLiteField RegisterAddressAdder { get; set; }
        public F_SQLiteField RegisterUniqueAddCode { get; set; }
        public F_SQLiteField Position { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField ShortLabel { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField Comments { get; set; }
    }

    public class R_806x_Def_Routines : R_806x_Elements_Core
    {
    }

    public class R_806x_Def_RoutinesArgs : R_SQLite_Core
    {
        public F_SQLiteField RoutineBank { get; set; }
        public F_SQLiteField RoutineAddress { get; set; }
        public F_SQLiteField RoutineUniqueAddCode { get; set; }
        public F_SQLiteField Position { get; set; }
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Encryption { get; set; }
        public F_SQLiteField Pointer { get; set; }
    }

    public class R_806x_Def_Scalars : R_806x_Elements_Core
    {
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Signed { get; set; }
        public F_SQLiteField Units { get; set; }
        public F_SQLiteField ScaleExpression { get; set; }
        public F_SQLiteField ScalePrecision { get; set; }
    }

    public class R_806x_Def_ScalarsBitFlags : R_SQLite_Core
    {
        public F_SQLiteField ScalarBank { get; set; }
        public F_SQLiteField ScalarAddress { get; set; }
        public F_SQLiteField ScalarUniqueAddCode { get; set; }
        public F_SQLiteField Position { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField ShortLabel { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField Comments { get; set; }
    }

    public class R_806x_Def_Structures : R_806x_Elements_Core
    {
        public F_SQLiteField Number { get; set; }
        public F_SQLiteField StructureDefinition { get; set; }
    }

    public class R_806x_Def_Tables : R_806x_Elements_Core
    {
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Signed { get; set; }
        public F_SQLiteField Columns { get; set; }
        public F_SQLiteField Rows { get; set; }
        public F_SQLiteField ColumnsUnits { get; set; }
        public F_SQLiteField RowsUnits { get; set; }
        public F_SQLiteField CellsUnits { get; set; }
        public F_SQLiteField CellsScaleExpression { get; set; }
        public F_SQLiteField CellsScalePrecision { get; set; }
        public F_SQLiteField ColumnsScalerBank { get; set; }
        public F_SQLiteField ColumnsScalerAddress { get; set; }
        public F_SQLiteField ColumnsScalerUniqueAddCode { get; set; }
        public F_SQLiteField RowsScalerBank { get; set; }
        public F_SQLiteField RowsScalerAddress { get; set; }
        public F_SQLiteField RowsScalerUniqueAddCode { get; set; }
    }

    public class R_806x_Hardware_Information : R_SQLite_Core
    {
        public F_SQLiteField PartNumber { get; set; }
        public F_SQLiteField FinisCode { get; set; }
        public F_SQLiteField HardwareCode { get; set; }
        public F_SQLiteField CatchCode { get; set; }
        public F_SQLiteField KnownLabel { get; set; }
        public F_SQLiteField ProcessingUnit { get; set; }
        public F_SQLiteField ConnectorPins { get; set; }
        public F_SQLiteField Comments { get; set; }
    }

    public class R_806x_Hardware_Pinout : R_SQLite_Core
    {
        public F_SQLiteField PartNumber { get; set; }
        public F_SQLiteField Pin { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Color { get; set; }
        public F_SQLiteField Comments { get; set; }
    }

    public class R_806x_Repository_Core : R_SQLite_Core
    {
        public F_SQLiteField Code { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField Comments { get; set; }
    }

    public class R_806x_Repository_Categories : R_806x_Repository_Core { }
    public class R_806x_Repository_Status : R_806x_Repository_Core { }
    public class R_806x_Repository_Units : R_806x_Repository_Core { }

    public class R_806x_Strategy_Banks : R_SQLite_Core
    {
        public F_SQLiteField Number { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Comments { get; set; }
    }

    public class R_806x_Strategy_Binaries : R_SQLite_Core
    {
        public F_SQLiteField Number { get; set; }
        public F_SQLiteField FileName { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField EntryCreation { get; set; }
        public F_SQLiteField EntryModification { get; set; }
        public F_SQLiteField FileSize { get; set; }
        public F_SQLiteField FileCreation { get; set; }
        public F_SQLiteField FileModification { get; set; }
        public F_SQLiteField FileExtension { get; set; }
        public F_SQLiteField FileBinary { get; set; }
    }

    public class R_806x_Strategy_History : R_SQLite_Core
    {
        public F_SQLiteField EntryCreation { get; set; }
        public F_SQLiteField EntryModification { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Version { get; set; }
        public F_SQLiteField Author { get; set; }
        public F_SQLiteField Details { get; set; }
    }

    public class R_806x_Strategy_Information : R_SQLite_Core
    {
        public F_SQLiteField Name { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Author { get; set; }
        public F_SQLiteField Version { get; set; }
        public F_SQLiteField Status { get; set; }
    }

    public class R_SAD806x_DB_Information : R_806x_DB_Information { }

    public class R_SAD806x_Elements_Core : R_SQLite_Core
    {
        public F_SQLiteField Bank { get; set; }
        public F_SQLiteField Address { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField Skip { get; set; }
    }

    public class R_SAD806x_Def_Functions : R_SAD806x_Elements_Core
    {
        public F_SQLiteField InputScaleExpression { get; set; }
        public F_SQLiteField OutputScaleExpression { get; set; }
    }

    public class R_SAD806x_Def_Operations : R_SAD806x_Elements_Core { }

    public class R_SAD806x_Def_Others : R_SAD806x_Elements_Core { }

    public class R_SAD806x_Def_Registers : R_SQLite_Core
    {
        public F_SQLiteField Address { get; set; }
        public F_SQLiteField AddressAdder { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField Skip { get; set; }
    }

    public class R_SAD806x_Def_RegistersBitFlags : R_SQLite_Core
    {
        public F_SQLiteField RegisterAddress { get; set; }
        public F_SQLiteField RegisterAddressAdder { get; set; }
        public F_SQLiteField RegisterUniqueAddCode { get; set; }
        public F_SQLiteField Position { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField Skip { get; set; }
    }

    public class R_SAD806x_Def_Routines : R_SAD806x_Elements_Core { }

    public class R_SAD806x_Def_RoutinesArgs : R_SQLite_Core
    {
        public F_SQLiteField RoutineBank { get; set; }
        public F_SQLiteField RoutineAddress { get; set; }
        public F_SQLiteField RoutineUniqueAddCode { get; set; }
        public F_SQLiteField Position { get; set; }
    }

    public class R_SAD806x_Def_Scalars : R_SAD806x_Elements_Core
    {
        public F_SQLiteField ScaleExpression { get; set; }
    }

    public class R_SAD806x_Def_ScalarsBitFlags : R_SQLite_Core
    {
        public F_SQLiteField ScalarBank { get; set; }
        public F_SQLiteField ScalarAddress { get; set; }
        public F_SQLiteField ScalarUniqueAddCode { get; set; }
        public F_SQLiteField Position { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField Skip { get; set; }
    }

    public class R_SAD806x_Def_Structures : R_SAD806x_Elements_Core
    {
        public F_SQLiteField StructureDefinition { get; set; }
    }

    public class R_SAD806x_Def_Tables : R_SAD806x_Elements_Core
    {
        public F_SQLiteField CellsScaleExpression { get; set; }
    }

    public class R_SAD806x_Properties : R_SQLite_Core
    {
        public F_SQLiteField NoAutoNumbering { get; set; }
        public F_SQLiteField RegistersListOutput { get; set; }
    }

    public class R_SAD806x_Signatures_Elements : R_SQLite_Core
    {
        public F_SQLiteField Something { get; set; }
    }

    public class R_SAD806x_Signatures_Routines : R_SQLite_Core
    {
        public F_SQLiteField Something { get; set; }
    }

    public class R_SAD_DB_Information : R_806x_DB_Information { }

    public class R_SAD_Properties : R_SQLite_Core
    {
        public F_SQLiteField Option1 { get; set; }
    }

    public class R_TunerPro_DB_Information : R_806x_DB_Information { }

    public class R_TunerPro_Properties : R_SQLite_Core
    {
        public F_SQLiteField BinaryNumber { get; set; }
        public F_SQLiteField BinaryBaseOffset { get; set; }
        public F_SQLiteField BinaryBaseOffsetSubtract { get; set; }
    }
}
