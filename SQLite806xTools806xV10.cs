using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SAD806x;

namespace SQLite806x
{
    public static class SQLite806xTools806xV10
    {
        public static string getDBVersion()
        {
            return "1.0";
        }

        public static DateTime getDBVersionDate()
        {
            return Convert.ToDateTime("2020-10-01");
        }

        public static string getDBVersionComments()
        {
            return "Initial version.";
        }

        public static void initSchema(ref List<T_SQLiteTable> sqlTables)
        {
            if (sqlTables == null) sqlTables = new List<T_SQLiteTable>();

            sqlTables.Add(SQLite806xTools.initTable<R_806x_DB_Information>("Information", "Database Information", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_Functions>("Functions Definition", "Definition for functions", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_Operations>("Operations Definition", "Definition for operations", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_Others>("Others Definition", "Definition for other addresses", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_Registers>("Registers Definition", "Definition for registers", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_RegistersBitFlags>("Registers Bit Flags", "Bit flags for registers", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_Routines>("Routines Definition", "Definition for routines", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_RoutinesArgs>("Routines Arguments", "Arguments for routines", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_Scalars>("Scalars Definition", "Definition for scalars", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_ScalarsBitFlags>("Scalars Bit Flags", "Bit flags for scalars", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_Structures>("Structures Definition", "Definition for structures", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Def_Tables>("Tables Definition", "Definition for tables", true, true));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Hardware_Information>("Hardware Information", "EEC Hardware Information"));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Hardware_Pinout>("Hardware Pinout", "EEC Hardware Pinout"));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Repository_Categories>("Repository Categories", "Repository Categories"));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Repository_Status>("Repository Status", "Repository Status"));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Repository_Units>("Repository Units", "Repository Units"));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Strategy_Banks>("Strategy Banks", "Banks"));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Strategy_Binaries>("Strategy Binaries", "Strategy related binaries"));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Strategy_Files>("Strategy Files", "Other strategy related files"));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Strategy_History>("Strategy History", "History"));
            sqlTables.Add(SQLite806xTools.initTable<R_806x_Strategy_Information>("Strategy Information", "Information"));
        }

        public static object addDBVersionRow(SQLite806xDB db806x, ref List<R_806x_DB_Information> rList)
        {
            if (db806x == null) return null;
            if (rList == null) return null;

            R_806x_DB_Information rRow = db806x.newRow<R_806x_DB_Information>();
            rRow.Version.Value = SQLite806xTools806x.getDBVersion();
            rRow.Date.Value = SQLite806xTools806x.getDBVersionDate();
            rRow.Comments.Value = SQLite806xTools806x.getDBVersionComments();

            rList.Add(rRow);
            return rRow;
        }

        public static object addTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Tables> rList, S6xTable s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;

            R_806x_Def_Tables rRow = db806x.newRow<R_806x_Def_Tables>();
            updateTableRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Tables rRow, S6xTable s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = s6xObject.DuplicateNum;
            rRow.Byte.Value = !s6xObject.WordOutput;
            rRow.CellsScaleExpression.Value = s6xObject.CellsScaleExpression;
            rRow.CellsScalePrecision.Value = s6xObject.CellsScalePrecision;
            rRow.CellsUnits.Value = s6xObject.CellsUnits;
            rRow.CellsMin.Value = s6xObject.CellsMin;
            rRow.CellsMax.Value = s6xObject.CellsMax;
            rRow.Columns.Value = s6xObject.ColsNumber;
            rRow.ColumnsUnits.Value = s6xObject.ColsUnits;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.Label.Value = s6xObject.Label;
            rRow.Rows.Value = s6xObject.RowsNumber;
            rRow.RowsUnits.Value = s6xObject.RowsUnits;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.Signed.Value = s6xObject.SignedOutput;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category_A.Value = s6xObject.Category;
            rRow.Category_B.Value = s6xObject.Category2;
            rRow.Category_C.Value = s6xObject.Category3;
            rRow.Status.Value = s6xObject.IdentificationStatus;
            rRow.StatusDetails.Value = s6xObject.IdentificationDetails;

            S6xFunction s6xScaler = null;
            if (s6xObject.ColsScalerAddress != null && s6xObject.ColsScalerAddress != string.Empty)
            {
                s6xScaler = (S6xFunction)sadS6x.slFunctions[s6xObject.ColsScalerAddress];
                if (s6xScaler == null) s6xScaler = (S6xFunction)sadS6x.slDupFunctions[s6xObject.ColsScalerAddress];

                if (s6xScaler != null)
                {
                    rRow.ColumnsScalerBank.Value = s6xScaler.BankNum;
                    rRow.ColumnsScalerAddress.Value = s6xScaler.AddressInt;
                    rRow.ColumnsScalerUniqueAddCode.Value = (s6xScaler.DuplicateAddress == null || s6xScaler.DuplicateAddress == string.Empty) ? 0.ToString() : s6xScaler.DuplicateNum.ToString();
                }
            }
            if (s6xObject.RowsScalerAddress != null && s6xObject.RowsScalerAddress != string.Empty)
            {
                s6xScaler = (S6xFunction)sadS6x.slFunctions[s6xObject.RowsScalerAddress];
                if (s6xScaler == null) s6xScaler = (S6xFunction)sadS6x.slDupFunctions[s6xObject.RowsScalerAddress];

                if (s6xScaler != null)
                {
                    rRow.RowsScalerBank.Value = s6xScaler.BankNum;
                    rRow.RowsScalerAddress.Value = s6xScaler.AddressInt;
                    rRow.RowsScalerUniqueAddCode.Value = (s6xScaler.DuplicateAddress == null || s6xScaler.DuplicateAddress == string.Empty) ? 0.ToString() : s6xScaler.DuplicateNum.ToString();
                }
            }
        }

        // Return 99 for Error, 0 when Added or Updated, 1 when Ignored on DateUpdated, 2 when Ignored on creation based on CreationMinDate
        public static object[] setTableS6x(ref SADS6x sadS6x, R_806x_Def_Tables rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xTable s6xObject = new S6xTable();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;
            try { s6xObject.DuplicateNum = ((string)rRow.UniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.UniqueAddCode.ValueConverted); }
            catch { s6xObject.DuplicateNum = 0; }

            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            if (s6xObject.DuplicateNum == 0)
            {
                if (sadS6x.slTables.ContainsKey(s6xObject.UniqueAddress)) s6xObject = (S6xTable)sadS6x.slTables[s6xObject.UniqueAddress];
            }
            else
            {
                if (sadS6x.slDupTables.ContainsKey(s6xObject.DuplicateAddress)) s6xObject = (S6xTable)sadS6x.slDupTables[s6xObject.DuplicateAddress];
            }

            s6xObject.Store = true;

            s6xObject.WordOutput = !(bool)rRow.Byte.ValueConverted;
            s6xObject.CellsScaleExpression = (string)rRow.CellsScaleExpression.ValueConverted;
            s6xObject.CellsScalePrecision = (int)rRow.CellsScalePrecision.ValueConverted;
            s6xObject.CellsUnits = (string)rRow.CellsUnits.ValueConverted;
            s6xObject.ColsNumber = (int)rRow.Columns.ValueConverted;
            s6xObject.ColsUnits = (string)rRow.ColumnsUnits.ValueConverted;
            s6xObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xObject.Label = (string)rRow.Label.ValueConverted;
            s6xObject.RowsNumber = (int)rRow.Rows.ValueConverted;
            s6xObject.RowsUnits = (string)rRow.RowsUnits.ValueConverted;
            s6xObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xObject.SignedOutput = (bool)rRow.Signed.ValueConverted;

            s6xObject.CellsMin = (string)rRow.CellsMin.ValueConverted;
            s6xObject.CellsMax = (string)rRow.CellsMax.ValueConverted;

            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.Category = (string)rRow.Category_A.ValueConverted;
            s6xObject.Category2 = (string)rRow.Category_B.ValueConverted;
            s6xObject.Category3 = (string)rRow.Category_C.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.Status.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.Status.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.StatusDetails.ValueConverted;

            S6xFunction s6xScaler = null;
            s6xScaler = new S6xFunction();
            s6xScaler.BankNum = (int)rRow.ColumnsScalerBank.ValueConverted;
            s6xScaler.AddressInt = (int)rRow.ColumnsScalerAddress.ValueConverted;
            s6xScaler.DuplicateNum = ((string)rRow.ColumnsScalerUniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.ColumnsScalerUniqueAddCode.ValueConverted);
            if (s6xScaler.DuplicateNum == 0) s6xObject.ColsScalerAddress = s6xScaler.UniqueAddress;
            else s6xObject.ColsScalerAddress = s6xScaler.DuplicateAddress;
            s6xScaler = new S6xFunction();
            s6xScaler.BankNum = (int)rRow.RowsScalerBank.ValueConverted;
            s6xScaler.AddressInt = (int)rRow.RowsScalerAddress.ValueConverted;
            s6xScaler.DuplicateNum = ((string)rRow.RowsScalerUniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.RowsScalerUniqueAddCode.ValueConverted);
            if (s6xScaler.DuplicateNum == 0) s6xObject.RowsScalerAddress = s6xScaler.UniqueAddress;
            else s6xObject.RowsScalerAddress = s6xScaler.DuplicateAddress;
            s6xScaler = null;

            if (s6xObject.DuplicateNum == 0)
            {
                if (!sadS6x.slTables.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slTables.Add(s6xObject.UniqueAddress, s6xObject);
            }
            else
            {
                if (!sadS6x.slDupTables.ContainsKey(s6xObject.DuplicateAddress)) sadS6x.slDupTables.Add(s6xObject.DuplicateAddress, s6xObject);
            }

            return new object[] { s6xObject, null, null };
        }

        public static object addFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Functions> rList, S6xFunction s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;

            R_806x_Def_Functions rRow = db806x.newRow<R_806x_Def_Functions>();
            updateFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Functions rRow, S6xFunction s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = s6xObject.DuplicateNum;
            rRow.Byte.Value = s6xObject.ByteInput;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.InputScaleExpression.Value = s6xObject.InputScaleExpression;
            rRow.InputScalePrecision.Value = s6xObject.InputScalePrecision;
            rRow.InputSigned.Value = s6xObject.SignedInput;
            rRow.InputUnits.Value = s6xObject.InputUnits;
            rRow.InputMin.Value = s6xObject.InputMin;
            rRow.InputMax.Value = s6xObject.InputMax;
            rRow.Label.Value = s6xObject.Label;
            rRow.OutputScaleExpression.Value = s6xObject.OutputScaleExpression;
            rRow.OutputScalePrecision.Value = s6xObject.OutputScalePrecision;
            rRow.OutputSigned.Value = s6xObject.SignedOutput;
            rRow.OutputUnits.Value = s6xObject.OutputUnits;
            rRow.OutputMin.Value = s6xObject.OutputMin;
            rRow.OutputMax.Value = s6xObject.OutputMax;
            rRow.Rows.Value = s6xObject.RowsNumber;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category_A.Value = s6xObject.Category;
            rRow.Category_B.Value = s6xObject.Category2;
            rRow.Category_C.Value = s6xObject.Category3;
            rRow.Status.Value = s6xObject.IdentificationStatus;
            rRow.StatusDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setFunctionS6x(ref SADS6x sadS6x, R_806x_Def_Functions rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xFunction s6xObject = new S6xFunction();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;
            try { s6xObject.DuplicateNum = ((string)rRow.UniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.UniqueAddCode.ValueConverted); }
            catch { s6xObject.DuplicateNum = 0; }

            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            if (s6xObject.DuplicateNum == 0)
            {
                if (sadS6x.slFunctions.ContainsKey(s6xObject.UniqueAddress)) s6xObject = (S6xFunction)sadS6x.slFunctions[s6xObject.UniqueAddress];
            }
            else
            {
                if (sadS6x.slDupFunctions.ContainsKey(s6xObject.DuplicateAddress)) s6xObject = (S6xFunction)sadS6x.slDupFunctions[s6xObject.DuplicateAddress];
            }

            s6xObject.Store = true;

            s6xObject.ByteInput = (bool)rRow.Byte.ValueConverted;
            s6xObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xObject.InputScaleExpression = (string)rRow.InputScaleExpression.ValueConverted;
            s6xObject.InputScalePrecision = (int)rRow.InputScalePrecision.ValueConverted;
            s6xObject.SignedInput = (bool)rRow.InputSigned.ValueConverted;
            s6xObject.InputUnits = (string)rRow.InputUnits.ValueConverted;
            s6xObject.Label = (string)rRow.Label.ValueConverted;
            s6xObject.OutputScaleExpression = (string)rRow.OutputScaleExpression.ValueConverted;
            s6xObject.OutputScalePrecision = (int)rRow.OutputScalePrecision.ValueConverted;
            s6xObject.SignedOutput = (bool)rRow.OutputSigned.ValueConverted;
            s6xObject.OutputUnits = (string)rRow.OutputUnits.ValueConverted;
            s6xObject.RowsNumber = (int)rRow.Rows.ValueConverted;
            s6xObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;

            s6xObject.InputMin = (string)rRow.InputMin.ValueConverted;
            s6xObject.InputMax = (string)rRow.InputMax.ValueConverted;
            s6xObject.OutputMin = (string)rRow.OutputMin.ValueConverted;
            s6xObject.OutputMax = (string)rRow.OutputMax.ValueConverted;

            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.Category = (string)rRow.Category_A.ValueConverted;
            s6xObject.Category2 = (string)rRow.Category_B.ValueConverted;
            s6xObject.Category3 = (string)rRow.Category_C.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.Status.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.Status.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.StatusDetails.ValueConverted;

            if (s6xObject.DuplicateNum == 0)
            {
                if (!sadS6x.slFunctions.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slFunctions.Add(s6xObject.UniqueAddress, s6xObject);
            }
            else
            {
                if (!sadS6x.slDupFunctions.ContainsKey(s6xObject.DuplicateAddress)) sadS6x.slDupFunctions.Add(s6xObject.DuplicateAddress, s6xObject);
            }

            return new object[] { s6xObject, null, null };
        }

        public static object addScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Scalars> rList, S6xScalar s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;

            R_806x_Def_Scalars rRow = db806x.newRow<R_806x_Def_Scalars>();
            updateScalarRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Scalars rRow, S6xScalar s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = s6xObject.DuplicateNum;
            rRow.Byte.Value = s6xObject.Byte;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.Label.Value = s6xObject.Label;
            rRow.ScaleExpression.Value = s6xObject.ScaleExpression;
            rRow.ScalePrecision.Value = s6xObject.ScalePrecision;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.Signed.Value = s6xObject.Signed;
            rRow.Units.Value = s6xObject.Units;
            rRow.Min.Value = s6xObject.Min;
            rRow.Max.Value = s6xObject.Max;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category_A.Value = s6xObject.Category;
            rRow.Category_B.Value = s6xObject.Category2;
            rRow.Category_C.Value = s6xObject.Category3;
            rRow.Status.Value = s6xObject.IdentificationStatus;
            rRow.StatusDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setScalarS6x(ref SADS6x sadS6x, R_806x_Def_Scalars rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xScalar s6xObject = new S6xScalar();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;
            try { s6xObject.DuplicateNum = ((string)rRow.UniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.UniqueAddCode.ValueConverted); }
            catch { s6xObject.DuplicateNum = 0; }

            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            if (s6xObject.DuplicateNum == 0)
            {
                if (sadS6x.slScalars.ContainsKey(s6xObject.UniqueAddress)) s6xObject = (S6xScalar)sadS6x.slScalars[s6xObject.UniqueAddress];
            }
            else
            {
                if (sadS6x.slDupScalars.ContainsKey(s6xObject.DuplicateAddress)) s6xObject = (S6xScalar)sadS6x.slDupScalars[s6xObject.DuplicateAddress];
            }

            s6xObject.Store = true;

            s6xObject.Byte = (bool)rRow.Byte.ValueConverted;
            s6xObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xObject.Label = (string)rRow.Label.ValueConverted;
            s6xObject.ScaleExpression = (string)rRow.ScaleExpression.ValueConverted;
            s6xObject.ScalePrecision = (int)rRow.ScalePrecision.ValueConverted;
            s6xObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xObject.Signed = (bool)rRow.Signed.ValueConverted;
            s6xObject.Units = (string)rRow.Units.ValueConverted;

            s6xObject.Min = (string)rRow.Min.ValueConverted;
            s6xObject.Max = (string)rRow.Max.ValueConverted;

            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.Category = (string)rRow.Category_A.ValueConverted;
            s6xObject.Category2 = (string)rRow.Category_B.ValueConverted;
            s6xObject.Category3 = (string)rRow.Category_C.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.Status.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.Status.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.StatusDetails.ValueConverted;

            if (s6xObject.DuplicateNum == 0)
            {
                if (!sadS6x.slScalars.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slScalars.Add(s6xObject.UniqueAddress, s6xObject);
            }
            else
            {
                if (!sadS6x.slDupScalars.ContainsKey(s6xObject.DuplicateAddress)) sadS6x.slDupScalars.Add(s6xObject.DuplicateAddress, s6xObject);
            }

            return new object[] { s6xObject, null, null };
        }

        public static object[] addScalarBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_ScalarsBitFlags> rList, S6xScalar s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;
            if (!s6xObject.isBitFlags) return null;
            if (s6xObject.BitFlags == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xBitFlag s6xBF in s6xObject.BitFlags)
            {
                if (s6xBF.Skip) continue;

                R_806x_Def_ScalarsBitFlags rRow = db806x.newRow<R_806x_Def_ScalarsBitFlags>();
                updateScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xBF);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_ScalarsBitFlags> rList, S6xScalar s6xObject, S6xBitFlag s6xBitFlag)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;
            if (!s6xObject.isBitFlags) return null;
            if (s6xObject.BitFlags == null) return null;

            R_806x_Def_ScalarsBitFlags rRow = db806x.newRow<R_806x_Def_ScalarsBitFlags>();
            updateScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xBitFlag);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_ScalarsBitFlags rRow, S6xScalar s6xObject, S6xBitFlag s6xBitFlag)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xBitFlag == null) return;

            rRow.ScalarBank.Value = s6xObject.BankNum;
            rRow.ScalarAddress.Value = s6xObject.AddressInt;
            rRow.ScalarUniqueAddCode.Value = s6xObject.DuplicateNum;
            rRow.Position.Value = s6xBitFlag.Position;
            rRow.UniqueAddCode.Value = 0;
            rRow.Comments.Value = s6xBitFlag.Comments;
            rRow.Label.Value = s6xBitFlag.Label;
            rRow.ShortLabel.Value = s6xBitFlag.ShortLabel;
            rRow.DateCreated.Value = s6xBitFlag.DateCreated;
            rRow.DateUpdated.Value = s6xBitFlag.DateUpdated;
            rRow.Category_A.Value = s6xObject.Category;
            rRow.Category_B.Value = s6xObject.Category2;
            rRow.Category_C.Value = s6xObject.Category3;
            rRow.Status.Value = s6xObject.IdentificationStatus;
            rRow.StatusDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setScalarBitFlagS6x(ref SADS6x sadS6x, R_806x_Def_ScalarsBitFlags rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xScalar s6xObject = new S6xScalar();

            s6xObject.BankNum = (int)rRow.ScalarBank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.ScalarAddress.ValueConverted;
            s6xObject.DuplicateNum = ((string)rRow.ScalarUniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.ScalarUniqueAddCode.ValueConverted);

            if (s6xObject.DuplicateNum == 0)
            {
                if (!sadS6x.slScalars.ContainsKey(s6xObject.UniqueAddress)) return null;
                s6xObject = (S6xScalar)sadS6x.slScalars[s6xObject.UniqueAddress];
            }
            else
            {
                if (!sadS6x.slDupScalars.ContainsKey(s6xObject.DuplicateAddress)) return null;
                s6xObject = (S6xScalar)sadS6x.slDupScalars[s6xObject.DuplicateAddress];
            }

            S6xBitFlag s6xBF = new S6xBitFlag();
            s6xBF.Position = (int)rRow.Position.ValueConverted;
            s6xBF.Comments = (string)rRow.Comments.ValueConverted;
            s6xBF.Label = (string)rRow.Label.ValueConverted;
            s6xBF.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xBF.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xBF.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xBF.Category = (string)rRow.Category_A.ValueConverted;
            s6xBF.Category2 = (string)rRow.Category_B.ValueConverted;
            s6xBF.Category3 = (string)rRow.Category_C.ValueConverted;
            s6xBF.IdentificationStatus = (string)rRow.Status.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.Status.ValueConverted);
            s6xBF.IdentificationDetails = (string)rRow.StatusDetails.ValueConverted;
            s6xObject.AddBitFlag(s6xBF);

            return new object[] { s6xBF, s6xObject, null };
        }

        public static object addStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Structures> rList, S6xStructure s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;

            R_806x_Def_Structures rRow = db806x.newRow<R_806x_Def_Structures>();
            updateStructureRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Structures rRow, S6xStructure s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = s6xObject.DuplicateNum;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.Label.Value = s6xObject.Label;
            rRow.Number.Value = s6xObject.Number;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.StructureDefinition.Value = s6xObject.StructDef;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category_A.Value = s6xObject.Category;
            rRow.Category_B.Value = s6xObject.Category2;
            rRow.Category_C.Value = s6xObject.Category3;
            rRow.Status.Value = s6xObject.IdentificationStatus;
            rRow.StatusDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setStructureS6x(ref SADS6x sadS6x, R_806x_Def_Structures rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xStructure s6xObject = new S6xStructure();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;
            try { s6xObject.DuplicateNum = ((string)rRow.UniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.UniqueAddCode.ValueConverted); }
            catch { s6xObject.DuplicateNum = 0; }

            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            if (s6xObject.DuplicateNum == 0)
            {
                if (sadS6x.slStructures.ContainsKey(s6xObject.UniqueAddress)) s6xObject = (S6xStructure)sadS6x.slStructures[s6xObject.UniqueAddress];
            }
            else
            {
                if (sadS6x.slDupStructures.ContainsKey(s6xObject.DuplicateAddress)) s6xObject = (S6xStructure)sadS6x.slDupStructures[s6xObject.DuplicateAddress];
            }

            s6xObject.Store = true;

            s6xObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xObject.Label = (string)rRow.Label.ValueConverted;
            s6xObject.Number = (int)rRow.Number.ValueConverted;
            s6xObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xObject.StructDef = (string)rRow.StructureDefinition.ValueConverted;

            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.Category = (string)rRow.Category_A.ValueConverted;
            s6xObject.Category2 = (string)rRow.Category_B.ValueConverted;
            s6xObject.Category3 = (string)rRow.Category_C.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.Status.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.Status.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.StatusDetails.ValueConverted;

            if (s6xObject.DuplicateNum == 0)
            {
                if (!sadS6x.slStructures.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slStructures.Add(s6xObject.UniqueAddress, s6xObject);
            }
            else
            {
                if (!sadS6x.slDupStructures.ContainsKey(s6xObject.DuplicateAddress)) sadS6x.slDupStructures.Add(s6xObject.DuplicateAddress, s6xObject);
            }

            return new object[] { s6xObject, null, null };
        }

        public static object addRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Routines> rList, S6xRoutine s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;

            R_806x_Def_Routines rRow = db806x.newRow<R_806x_Def_Routines>();
            updateRoutineRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Routines rRow, S6xRoutine s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = 0;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.Label.Value = s6xObject.Label;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category_A.Value = s6xObject.Category;
            rRow.Category_B.Value = s6xObject.Category2;
            rRow.Category_C.Value = s6xObject.Category3;
            rRow.Status.Value = s6xObject.IdentificationStatus;
            rRow.StatusDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setRoutineS6x(ref SADS6x sadS6x, R_806x_Def_Routines rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRoutine s6xObject = new S6xRoutine();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;

            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            if (sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress)) s6xObject = (S6xRoutine)sadS6x.slRoutines[s6xObject.UniqueAddress];

            s6xObject.Store = true;

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;

            s6xObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xObject.Label = (string)rRow.Label.ValueConverted;
            s6xObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;

            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.Category = (string)rRow.Category_A.ValueConverted;
            s6xObject.Category2 = (string)rRow.Category_B.ValueConverted;
            s6xObject.Category3 = (string)rRow.Category_C.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.Status.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.Status.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.StatusDetails.ValueConverted;

            if (!sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slRoutines.Add(s6xObject.UniqueAddress, s6xObject);

            return new object[] { s6xObject, null, null };
        }

        public static object[] addRoutineInputArgRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_RoutinesArgs> rList, S6xRoutine s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputArguments == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputArgument s6xSubObject in s6xObject.InputArguments)
            {
                R_806x_Def_RoutinesArgs rRow = db806x.newRow<R_806x_Def_RoutinesArgs>();
                updateRoutineInputArgRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_RoutinesArgs> rList, S6xRoutine s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputArguments == null) return null;

            R_806x_Def_RoutinesArgs rRow = db806x.newRow<R_806x_Def_RoutinesArgs>();
            updateRoutineInputArgRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_RoutinesArgs rRow, S6xRoutine s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InputArguments == null) return;

            rRow.RoutineBank.Value = s6xObject.BankNum;
            rRow.RoutineAddress.Value = s6xObject.AddressInt;
            rRow.RoutineUniqueAddCode.Value = 0;
            rRow.Byte.Value = !s6xSubObject.Word;
            rRow.Encryption.Value = s6xSubObject.Encryption;
            rRow.Pointer.Value = s6xSubObject.Pointer;
            rRow.Position.Value = s6xSubObject.Position;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
        }

        public static object[] setRoutineInputArgS6x(ref SADS6x sadS6x, R_806x_Def_RoutinesArgs rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRoutine s6xObject = new S6xRoutine();

            s6xObject.BankNum = (int)rRow.RoutineBank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.RoutineAddress.ValueConverted;

            if (!sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xRoutine)sadS6x.slRoutines[s6xObject.UniqueAddress];

            S6xRoutineInputArgument s6xSubObject = new S6xRoutineInputArgument();
            s6xSubObject.Word = !(bool)rRow.Byte.ValueConverted;
            s6xSubObject.Encryption = (int)rRow.Encryption.ValueConverted;
            s6xSubObject.Pointer = (bool)rRow.Pointer.ValueConverted;
            s6xSubObject.Position = (int)rRow.Position.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            S6xRoutineInputArgument[] newArray = null;
            if (s6xObject.InputArguments == null) newArray =  new S6xRoutineInputArgument[s6xSubObject.Position];
            else if (s6xObject.InputArguments.Length < s6xSubObject.Position - 1) newArray =  new S6xRoutineInputArgument[s6xSubObject.Position];
            else newArray = (S6xRoutineInputArgument[])s6xObject.InputArguments.Clone();

            for (int iPos = 0; iPos < newArray.Length; iPos++)
            {
                if (iPos == s6xSubObject.Position) newArray[iPos] = s6xSubObject;
                else newArray[iPos] = s6xObject.InputArguments[iPos];
            }
            s6xObject.InputArguments = newArray;

            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object addOperationRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Operations> rList, S6xOperation s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip) return null;

            R_806x_Def_Operations rRow = db806x.newRow<R_806x_Def_Operations>();
            updateOperationRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateOperationRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Operations rRow, S6xOperation s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = 0;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.Label.Value = s6xObject.Label;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category_A.Value = s6xObject.Category;
            rRow.Category_B.Value = s6xObject.Category2;
            rRow.Category_C.Value = s6xObject.Category3;
            rRow.Status.Value = s6xObject.IdentificationStatus;
            rRow.StatusDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setOperationS6x(ref SADS6x sadS6x, R_806x_Def_Operations rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xOperation s6xObject = new S6xOperation();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;

            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            if (sadS6x.slOperations.ContainsKey(s6xObject.UniqueAddress)) s6xObject = (S6xOperation)sadS6x.slOperations[s6xObject.UniqueAddress];

            s6xObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xObject.Label = (string)rRow.Label.ValueConverted;
            s6xObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;

            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.Category = (string)rRow.Category_A.ValueConverted;
            s6xObject.Category2 = (string)rRow.Category_B.ValueConverted;
            s6xObject.Category3 = (string)rRow.Category_C.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.Status.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.Status.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.StatusDetails.ValueConverted;

            if (!sadS6x.slOperations.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slOperations.Add(s6xObject.UniqueAddress, s6xObject);

            return new object[] { s6xObject, null, null };
        }

        public static object addOtherRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Others> rList, S6xOtherAddress s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip) return null;

            R_806x_Def_Others rRow = db806x.newRow<R_806x_Def_Others>();
            updateOtherRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateOtherRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Others rRow, S6xOtherAddress s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = 0;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.Label.Value = s6xObject.Label;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category_A.Value = s6xObject.Category;
            rRow.Category_B.Value = s6xObject.Category2;
            rRow.Category_C.Value = s6xObject.Category3;
            rRow.Status.Value = s6xObject.IdentificationStatus;
            rRow.StatusDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setOtherS6x(ref SADS6x sadS6x, R_806x_Def_Others rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xOtherAddress s6xObject = new S6xOtherAddress();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;

            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            if (sadS6x.slOtherAddresses.ContainsKey(s6xObject.UniqueAddress)) s6xObject = (S6xOtherAddress)sadS6x.slOtherAddresses[s6xObject.UniqueAddress];

            s6xObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xObject.Label = (string)rRow.Label.ValueConverted;

            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.Category = (string)rRow.Category_A.ValueConverted;
            s6xObject.Category2 = (string)rRow.Category_B.ValueConverted;
            s6xObject.Category3 = (string)rRow.Category_C.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.Status.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.Status.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.StatusDetails.ValueConverted;

            if (!sadS6x.slOtherAddresses.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slOtherAddresses.Add(s6xObject.UniqueAddress, s6xObject);

            return new object[] { s6xObject, null, null };
        }

        public static object addRegisterRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Registers> rList, S6xRegister s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;

            R_806x_Def_Registers rRow = db806x.newRow<R_806x_Def_Registers>();
            updateRegisterRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRegisterRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Registers rRow, S6xRegister s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Address.Value = s6xObject.AddressInt;
            rRow.AddressAdder.Value = s6xObject.AdditionalAddress10;
            rRow.UniqueAddCode.Value = 0;
            rRow.ByteLabel.Value = s6xObject.ByteLabel;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.ShortLabel.Value = s6xObject.Label;
            rRow.WordLabel.Value = s6xObject.WordLabel;
            rRow.SizeStatus.Value = s6xObject.SizeStatus;
            rRow.SignedStatus.Value = s6xObject.SignedStatus;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category_A.Value = s6xObject.Category;
            rRow.Category_B.Value = s6xObject.Category2;
            rRow.Category_C.Value = s6xObject.Category3;
            rRow.Status.Value = s6xObject.IdentificationStatus;
            rRow.StatusDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setRegisterS6x(ref SADS6x sadS6x, R_806x_Def_Registers rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRegister s6xObject = new S6xRegister();

            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;
            s6xObject.AdditionalAddress10 = (string)rRow.AddressAdder.ValueConverted;

            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            if (sadS6x.slRegisters.ContainsKey(s6xObject.UniqueAddress)) s6xObject = (S6xRegister)sadS6x.slRegisters[s6xObject.UniqueAddress];

            s6xObject.Store = true;

            s6xObject.ByteLabel = (string)rRow.ByteLabel.ValueConverted;
            s6xObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xObject.Label = (string)rRow.ShortLabel.ValueConverted;
            s6xObject.WordLabel = (string)rRow.WordLabel.ValueConverted;

            s6xObject.SizeStatus = (string)rRow.SizeStatus.ValueConverted;
            s6xObject.SignedStatus = (string)rRow.SignedStatus.ValueConverted;

            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.Category = (string)rRow.Category_A.ValueConverted;
            s6xObject.Category2 = (string)rRow.Category_B.ValueConverted;
            s6xObject.Category3 = (string)rRow.Category_C.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.Status.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.Status.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.StatusDetails.ValueConverted;

            if (!sadS6x.slRegisters.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slRegisters.Add(s6xObject.UniqueAddress, s6xObject);

            return new object[] { s6xObject, null, null };
        }

        public static object addRegisterBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_RegistersBitFlags> rList, S6xRegister s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;
            if (!s6xObject.isBitFlags) return null;
            if (s6xObject.BitFlags == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xBitFlag s6xSubObject in s6xObject.BitFlags)
            {
                if (s6xSubObject.Skip) continue;

                R_806x_Def_RegistersBitFlags rRow = db806x.newRow<R_806x_Def_RegistersBitFlags>();
                updateRegisterBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addRegisterBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_RegistersBitFlags> rList, S6xRegister s6xObject, S6xBitFlag s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Skip || !s6xObject.Store) return null;
            if (!s6xObject.isBitFlags) return null;
            if (s6xObject.BitFlags == null) return null;

            R_806x_Def_RegistersBitFlags rRow = db806x.newRow<R_806x_Def_RegistersBitFlags>();
            updateRegisterBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRegisterBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_RegistersBitFlags rRow, S6xRegister s6xObject, S6xBitFlag s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.RegisterAddress.Value = s6xObject.AddressInt;
            rRow.RegisterAddressAdder.Value = s6xObject.AdditionalAddress10;
            rRow.RegisterUniqueAddCode.Value = 0;
            rRow.Position.Value = s6xSubObject.Position;
            rRow.UniqueAddCode.Value = 0;
            rRow.Comments.Value = s6xSubObject.Comments;
            rRow.Label.Value = s6xSubObject.Label;
            rRow.ShortLabel.Value = s6xSubObject.ShortLabel;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category_A.Value = s6xObject.Category;
            rRow.Category_B.Value = s6xObject.Category2;
            rRow.Category_C.Value = s6xObject.Category3;
            rRow.Status.Value = s6xObject.IdentificationStatus;
            rRow.StatusDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setRegisterBitFlagS6x(ref SADS6x sadS6x, R_806x_Def_RegistersBitFlags rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRegister s6xObject = new S6xRegister();

            s6xObject.AddressInt = (int)rRow.RegisterAddress.ValueConverted;
            s6xObject.AdditionalAddress10 = (string)rRow.RegisterAddressAdder.ValueConverted;

            if (!sadS6x.slRegisters.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xRegister)sadS6x.slRegisters[s6xObject.UniqueAddress];

            S6xBitFlag s6xBF = new S6xBitFlag();
            s6xBF.Position = (int)rRow.Position.ValueConverted;
            s6xBF.Comments = (string)rRow.Comments.ValueConverted;
            s6xBF.Label = (string)rRow.Label.ValueConverted;
            s6xBF.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xBF.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xBF.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xBF.Category = (string)rRow.Category_A.ValueConverted;
            s6xBF.Category2 = (string)rRow.Category_B.ValueConverted;
            s6xBF.Category3 = (string)rRow.Category_C.ValueConverted;
            s6xBF.IdentificationStatus = (string)rRow.Status.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.Status.ValueConverted);
            s6xBF.IdentificationDetails = (string)rRow.StatusDetails.ValueConverted;
            s6xObject.AddBitFlag(s6xBF);

            return new object[] { s6xBF, s6xObject, null };
        }

        public static List<object> getScalarSubRows(ref SQLite806xDB db806x, R_806x_Def_Scalars rRow)
        {
            if (db806x == null) return null;
            if (rRow == null) return null;
            
            R_806x_Def_ScalarsBitFlags sampleRow = db806x.newRow<R_806x_Def_ScalarsBitFlags>();
            string subWhereClause = string.Empty;
            subWhereClause = string.Format("{0} = {1} AND {2} = {3} AND ({4} = '{5}' OR ({4} IS NULL AND '{5}' = ''))", sampleRow.ScalarBank.Name, rRow.Bank.ValueConverted, sampleRow.ScalarAddress.Name, rRow.Address.ValueConverted, sampleRow.ScalarUniqueAddCode.Name, rRow.UniqueAddCode.ValueConverted);

            List<object> rList = new List<object>();
            try { foreach (object oRow in db806x.Read<R_806x_Def_ScalarsBitFlags>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { rList = null; }

            return rList;
        }

        public static List<object> getRoutineSubRows(ref SQLite806xDB db806x, R_806x_Def_Routines rRow)
        {
            if (db806x == null) return null;
            if (rRow == null) return null;

            R_806x_Def_RoutinesArgs sampleRow = db806x.newRow<R_806x_Def_RoutinesArgs>();
            string subWhereClause = string.Empty;
            subWhereClause = string.Format("{0} = {1} AND {2} = {3} AND ({4} = '{5}' OR ({4} IS NULL AND '{5}' = ''))", sampleRow.RoutineBank.Name, rRow.Bank.ValueConverted, sampleRow.RoutineAddress.Name, rRow.Address.ValueConverted, sampleRow.RoutineUniqueAddCode.Name, rRow.UniqueAddCode.ValueConverted);

            List<object> rList = new List<object>();
            try { foreach (object oRow in db806x.Read<R_806x_Def_RoutinesArgs>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { rList = null; }

            return rList;
        }

        public static List<object> getRegisterSubRows(ref SQLite806xDB db806x, R_806x_Def_Registers rRow)
        {
            if (db806x == null) return null;
            if (rRow == null) return null;

            R_806x_Def_RegistersBitFlags sampleRow = db806x.newRow<R_806x_Def_RegistersBitFlags>();
            string subWhereClause = string.Empty;
            subWhereClause = string.Format("{0} = {1} AND ({2} = '{3}' OR ({2} IS NULL AND '{3}' = '')) AND ({4} = '{5}' OR ({4} IS NULL AND '{5}' = ''))", sampleRow.RegisterAddress.Name, rRow.Address.ValueConverted, sampleRow.RegisterAddressAdder.Name, rRow.AddressAdder.ValueConverted, sampleRow.RegisterUniqueAddCode.Name, rRow.UniqueAddCode.ValueConverted);

            List<object> rList = new List<object>();
            try { foreach (object oRow in db806x.Read<R_806x_Def_RegistersBitFlags>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { rList = null; }

            return rList;
        }

        public static bool deleteTableRow(ref SQLite806xDB db806x, R_806x_Def_Tables rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_806x_Def_Tables> rList = new List<R_806x_Def_Tables>() { rRow };
            return db806x.Delete<R_806x_Def_Tables>(rList);
        }

        public static bool deleteFunctionRow(ref SQLite806xDB db806x, R_806x_Def_Functions rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_806x_Def_Functions> rList = new List<R_806x_Def_Functions>() { rRow };
            return db806x.Delete<R_806x_Def_Functions>(rList);
        }

        public static bool deleteScalarRow(ref SQLite806xDB db806x, R_806x_Def_Scalars rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                List<object> rSubListObjects = getScalarSubRows(ref db806x, rRow);
                if (rSubListObjects == null) return false;
                List<R_806x_Def_ScalarsBitFlags> rSubList = new List<R_806x_Def_ScalarsBitFlags>();
                foreach (R_806x_Def_ScalarsBitFlags rSubRow in rSubListObjects) rSubList.Add(rSubRow);
                if (rSubList.Count > 0)
                {
                    if (!db806x.Delete<R_806x_Def_ScalarsBitFlags>(rSubList)) return false;
                }
            }

            List<R_806x_Def_Scalars> rList = new List<R_806x_Def_Scalars>() { rRow };
            return db806x.Delete<R_806x_Def_Scalars>(rList);
        }

        public static bool deleteScalarBitFlagRow(ref SQLite806xDB db806x, R_806x_Def_ScalarsBitFlags rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_806x_Def_ScalarsBitFlags> rList = new List<R_806x_Def_ScalarsBitFlags>() { rRow };
            return db806x.Delete<R_806x_Def_ScalarsBitFlags>(rList);
        }

        public static bool deleteStructureRow(ref SQLite806xDB db806x, R_806x_Def_Structures rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_806x_Def_Structures> rList = new List<R_806x_Def_Structures>() { rRow };
            return db806x.Delete<R_806x_Def_Structures>(rList);
        }

        public static bool deleteRoutineRow(ref SQLite806xDB db806x, R_806x_Def_Routines rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                List<object> rSubListObjects = getRoutineSubRows(ref db806x, rRow);
                if (rSubListObjects == null) return false;
                List<R_806x_Def_RoutinesArgs> rSubList = new List<R_806x_Def_RoutinesArgs>();
                foreach (R_806x_Def_RoutinesArgs rSubRow in rSubListObjects) rSubList.Add(rSubRow);
                if (rSubList.Count > 0)
                {
                    if (!db806x.Delete<R_806x_Def_RoutinesArgs>(rSubList)) return false;
                }
            }

            List<R_806x_Def_Routines> rList = new List<R_806x_Def_Routines>() { rRow };
            return db806x.Delete<R_806x_Def_Routines>(rList);
        }

        public static bool deleteRoutineInputArgRow(ref SQLite806xDB db806x, R_806x_Def_RoutinesArgs rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_806x_Def_RoutinesArgs> rList = new List<R_806x_Def_RoutinesArgs>() { rRow };
            return db806x.Delete<R_806x_Def_RoutinesArgs>(rList);
        }

        public static bool deleteOperationRow(ref SQLite806xDB db806x, R_806x_Def_Operations rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_806x_Def_Operations> rList = new List<R_806x_Def_Operations>() { rRow };
            return db806x.Delete<R_806x_Def_Operations>(rList);
        }

        public static bool deleteOtherRow(ref SQLite806xDB db806x, R_806x_Def_Others rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_806x_Def_Others> rList = new List<R_806x_Def_Others>() { rRow };
            return db806x.Delete<R_806x_Def_Others>(rList);
        }

        public static bool deleteRegisterRow(ref SQLite806xDB db806x, R_806x_Def_Registers rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                List<object> rSubListObjects = getRegisterSubRows(ref db806x, rRow);
                if (rSubListObjects == null) return false;
                List<R_806x_Def_RegistersBitFlags> rSubList = new List<R_806x_Def_RegistersBitFlags>();
                foreach (R_806x_Def_RegistersBitFlags rSubRow in rSubListObjects) rSubList.Add(rSubRow);
                if (rSubList.Count > 0)
                {
                    if (!db806x.Delete<R_806x_Def_RegistersBitFlags>(rSubList)) return false;
                }
            }

            List<R_806x_Def_Registers> rList = new List<R_806x_Def_Registers>() { rRow };
            return db806x.Delete<R_806x_Def_Registers>(rList);
        }

        public static bool deleteRegisterBitFlagRow(ref SQLite806xDB db806x, R_806x_Def_RegistersBitFlags rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_806x_Def_RegistersBitFlags> rList = new List<R_806x_Def_RegistersBitFlags>() { rRow };
            return db806x.Delete<R_806x_Def_RegistersBitFlags>(rList);
        }

    }

    public class R_806x_DB_Information : R_SQLite_DB_Information {}

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
        public F_SQLiteField StatusDetails { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (Label != null) if (Label.Value != null && Label.Value.ToString() != string.Empty) sParts.Add(Label.Value.ToString());

            return string.Join(" - ", sParts.ToArray());
        }

        public new string getRecordDescription()
        {
            List<string> sParts = new List<string>();
            if (ShortLabel != null) if (ShortLabel.Value != null) if (ShortLabel.Value.ToString() != string.Empty) sParts.Add(ShortLabel.Value.ToString());
            if (Label != null) if (Label.Value != null) if (Label.Value.ToString() != string.Empty) sParts.Add(Label.Value.ToString());
            if (Comments != null) if (Comments.Value != null) if (Comments.Value.ToString() != string.Empty) sParts.Add(Comments.Value.ToString());

            return string.Join("\r\n", sParts.ToArray());
        }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Bank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Address", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("UniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            dResult.Add("ShortLabel", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Label", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("Category_A", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category_B", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category_C", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Status", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("StatusDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATE"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATE"));
            return dResult;
        }
    }

    public class R_806x_Def_BitFlags : R_SQLite_Core
    {
        public F_SQLiteField Position { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField ShortLabel { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField Category_A { get; set; }
        public F_SQLiteField Category_B { get; set; }
        public F_SQLiteField Category_C { get; set; }
        public F_SQLiteField Status { get; set; }
        public F_SQLiteField StatusDetails { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (Position != null) if (Position.Value != null) sParts.Add(Convert.ToInt32(Position.Value).ToString());
            if (Label != null) if (Label.Value != null) if (Label.Value.ToString() != string.Empty) sParts.Add(Label.Value.ToString());

            return string.Join(" - ", sParts.ToArray());
        }

        public new string getRecordDescription()
        {
            List<string> sParts = new List<string>();
            if (ShortLabel != null) if (ShortLabel.Value != null) if (ShortLabel.Value.ToString() != string.Empty) sParts.Add(ShortLabel.Value.ToString());
            if (Label != null) if (Label.Value != null) if (Label.Value.ToString() != string.Empty) sParts.Add(Label.Value.ToString());
            if (Comments != null) if (Comments.Value != null) if (Comments.Value.ToString() != string.Empty) sParts.Add(Comments.Value.ToString());

            return string.Join("\r\n", sParts.ToArray());
        }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Position", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("UniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            dResult.Add("ShortLabel", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Label", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("Category_A", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category_B", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category_C", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Status", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("StatusDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATE"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATE"));
            return dResult;
        }
    }
    
    public class R_806x_Def_Functions : R_806x_Elements_Core
    {
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Rows { get; set; }
        public F_SQLiteField InputSigned { get; set; }
        public F_SQLiteField InputUnits { get; set; }
        public F_SQLiteField InputScaleExpression { get; set; }
        public F_SQLiteField InputScalePrecision { get; set; }
        public F_SQLiteField InputMin { get; set; }
        public F_SQLiteField InputMax { get; set; }
        public F_SQLiteField OutputSigned { get; set; }
        public F_SQLiteField OutputUnits { get; set; }
        public F_SQLiteField OutputScaleExpression { get; set; }
        public F_SQLiteField OutputScalePrecision { get; set; }
        public F_SQLiteField OutputMin { get; set; }
        public F_SQLiteField OutputMax { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Byte", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Rows", new F_SQLiteFieldProperties("INT (3)"));
            dResult.Add("InputSigned", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("InputUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("InputScaleExpression", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("InputScalePrecision", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("InputMin", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("InputMax", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("OutputSigned", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("OutputUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("OutputScaleExpression", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("OutputScalePrecision", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("OutputMin", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("OutputMax", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            return dResult;
        }
    }

    public class R_806x_Def_Operations : R_806x_Elements_Core {}

    public class R_806x_Def_Others : R_806x_Elements_Core {}

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
        public F_SQLiteField SizeStatus { get; set; }
        public F_SQLiteField SignedStatus { get; set; }
        public F_SQLiteField Category_A { get; set; }
        public F_SQLiteField Category_B { get; set; }
        public F_SQLiteField Category_C { get; set; }
        public F_SQLiteField Status { get; set; }
        public F_SQLiteField StatusDetails { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (ShortLabel != null) if (ShortLabel.Value != null) if (ShortLabel.Value.ToString() != string.Empty) sParts.Add(ShortLabel.Value.ToString());

            return string.Join(" - ", sParts.ToArray());
        }

        public new string getRecordDescription()
        {
            List<string> sParts = new List<string>();
            if (Description != null) if (Description.Value != null) if (Description.Value.ToString() != string.Empty) sParts.Add(Description.Value.ToString());
            if (Comments != null) if (Comments.Value != null) if (Comments.Value.ToString() != string.Empty) sParts.Add(Comments.Value.ToString());

            return string.Join("\r\n", sParts.ToArray());
        }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Address", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("AddressAdder", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            dResult.Add("UniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            dResult.Add("ShortLabel", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Description", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("ByteLabel", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("WordLabel", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("SizeStatus", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("SignedStatus", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Category_A", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category_B", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category_C", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Status", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("StatusDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATE"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATE"));
            return dResult;
        }
    }

    public class R_806x_Def_RegistersBitFlags : R_806x_Def_BitFlags
    {
        public F_SQLiteField RegisterAddress { get; set; }
        public F_SQLiteField RegisterAddressAdder { get; set; }
        public F_SQLiteField RegisterUniqueAddCode { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("RegisterAddress", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("RegisterAddressAdder", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            dResult.Add("RegisterUniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            return dResult;
        }
    }

    public class R_806x_Def_Routines : R_806x_Elements_Core { }

    public class R_806x_Def_RoutinesArgs : R_SQLite_Core
    {
        public F_SQLiteField RoutineBank { get; set; }
        public F_SQLiteField RoutineAddress { get; set; }
        public F_SQLiteField RoutineUniqueAddCode { get; set; }
        public F_SQLiteField Position { get; set; }
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Encryption { get; set; }
        public F_SQLiteField Pointer { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (Position != null) if (Position.Value != null) sParts.Add(Convert.ToInt32(Position.Value).ToString());

            return string.Join(" - ", sParts.ToArray());
        }

        public new string getRecordDescription()
        {
            List<string> sParts = new List<string>();

            return string.Join("\r\n", sParts.ToArray());
        }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("RoutineBank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("RoutineAddress", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("RoutineUniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            dResult.Add("Position", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Byte", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Encryption", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Pointer", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_806x_Def_Scalars : R_806x_Elements_Core
    {
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Signed { get; set; }
        public F_SQLiteField Units { get; set; }
        public F_SQLiteField ScaleExpression { get; set; }
        public F_SQLiteField ScalePrecision { get; set; }
        public F_SQLiteField Min { get; set; }
        public F_SQLiteField Max { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Byte", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Signed", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Units", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ScaleExpression", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ScalePrecision", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Min", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Max", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            return dResult;
        }
    }

    public class R_806x_Def_ScalarsBitFlags : R_806x_Def_BitFlags
    {
        public F_SQLiteField ScalarBank { get; set; }
        public F_SQLiteField ScalarAddress { get; set; }
        public F_SQLiteField ScalarUniqueAddCode { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("ScalarBank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("ScalarAddress", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("ScalarUniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            return dResult;
        }
    }

    public class R_806x_Def_Structures : R_806x_Elements_Core
    {
        public F_SQLiteField Number { get; set; }
        public F_SQLiteField StructureDefinition { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Number", new F_SQLiteFieldProperties("INT (3)"));
            dResult.Add("StructureDefinition", new F_SQLiteFieldProperties("TEXT"));
            return dResult;
        }
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
        public F_SQLiteField CellsMin { get; set; }
        public F_SQLiteField CellsMax { get; set; }
        public F_SQLiteField ColumnsScalerBank { get; set; }
        public F_SQLiteField ColumnsScalerAddress { get; set; }
        public F_SQLiteField ColumnsScalerUniqueAddCode { get; set; }
        public F_SQLiteField RowsScalerBank { get; set; }
        public F_SQLiteField RowsScalerAddress { get; set; }
        public F_SQLiteField RowsScalerUniqueAddCode { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Byte", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Signed", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Columns", new F_SQLiteFieldProperties("INT (3)"));
            dResult.Add("Rows", new F_SQLiteFieldProperties("INT (3)"));
            dResult.Add("ColumnsUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("RowsUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("CellsUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("CellsScaleExpression", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("CellsScalePrecision", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("CellsMin", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("CellsMax", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("ColumnsScalerBank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("ColumnsScalerAddress", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("ColumnsScalerUniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            dResult.Add("RowsScalerBank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("RowsScalerAddress", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("RowsScalerUniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            return dResult;
        }
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
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (CatchCode != null) if (CatchCode.Value != null) if (CatchCode.Value.ToString() != string.Empty) sParts.Add(CatchCode.Value.ToString());
            if (PartNumber != null) if (PartNumber.Value != null) if (PartNumber.Value.ToString() != string.Empty) sParts.Add(PartNumber.Value.ToString());
            if (FinisCode != null) if (FinisCode.Value != null) if (FinisCode.Value.ToString() != string.Empty) sParts.Add(FinisCode.Value.ToString());
            
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
            dResult.Add("PartNumber", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("FinisCode", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("HardwareCode", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("CatchCode", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("KnownLabel", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("ProcessingUnit", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("ConnectorPins", new F_SQLiteFieldProperties("INT (3)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_806x_Hardware_Pinout : R_SQLite_Core
    {
        public F_SQLiteField PartNumber { get; set; }
        public F_SQLiteField Pin { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Color { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (Pin != null) if (Pin.Value != null) if (Pin.Value.ToString() != string.Empty) sParts.Add(Pin.Value.ToString());
            if (Description != null) if (Description.Value != null) if (Description.Value.ToString() != string.Empty) sParts.Add(Description.Value.ToString());

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
            dResult.Add("PartNumber", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Pin", new F_SQLiteFieldProperties("INT (3)"));
            dResult.Add("Description", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Color", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_806x_Repository_Core : R_SQLite_Core
    {
        public F_SQLiteField Code { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (Code != null) if (Code.Value != null) if (Code.Value.ToString() != string.Empty) sParts.Add(Code.Value.ToString());
            if (Label != null) if (Label.Value != null) if (Label.Value.ToString() != string.Empty) sParts.Add(Label.Value.ToString());

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
            dResult.Add("Code", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Label", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_806x_Repository_Categories : R_806x_Repository_Core {}

    public class R_806x_Repository_Status : R_806x_Repository_Core {}

    public class R_806x_Repository_Units : R_806x_Repository_Core {}

    public class R_806x_Strategy_Banks : R_SQLite_Core
    {
        public F_SQLiteField Number { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (Number != null) if (Number.Value != null) if (Number.Value.ToString() != string.Empty) sParts.Add(Number.Value.ToString());
            if (Description != null) if (Description.Value != null) if (Description.Value.ToString() != string.Empty) sParts.Add(Description.Value.ToString());

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
            dResult.Add("Number", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Description", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_806x_Strategy_Files : R_SQLite_Files {}

    public class R_806x_Strategy_Binaries : R_SQLite_Files { }

    public class R_806x_Strategy_History : R_SQLite_Core
    {
        public F_SQLiteField EntryCreation { get; set; }
        public F_SQLiteField EntryModification { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Version { get; set; }
        public F_SQLiteField Author { get; set; }
        public F_SQLiteField Details { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (Version != null) if (Version.Value != null) if (Version.Value.ToString() != string.Empty) sParts.Add(Version.Value.ToString());
            if (EntryModification != null) if (EntryModification.Value != null) sParts.Add(Convert.ToDateTime(EntryModification.Value).ToString("yyyy-MM-dd"));

            return string.Join(" - ", sParts.ToArray());
        }

        public new string getRecordDescription()
        {
            List<string> sParts = new List<string>();
            if (Description != null) if (Description.Value != null) if (Description.Value.ToString() != string.Empty) sParts.Add(Description.Value.ToString());
            if (Author != null) if (Author.Value != null) if (Author.Value.ToString() != string.Empty) sParts.Add(Author.Value.ToString());
            if (Details != null) if (Details.Value != null) if (Details.Value.ToString() != string.Empty) sParts.Add(Details.Value.ToString());

            return string.Join("\r\n", sParts.ToArray());
        }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("EntryCreation", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("EntryModification", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("Description", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Version", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("Author", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Details", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_806x_Strategy_Information : R_SQLite_Core
    {
        public F_SQLiteField Name { get; set; }
        public F_SQLiteField Description { get; set; }
        public F_SQLiteField Author { get; set; }
        public F_SQLiteField Version { get; set; }
        public F_SQLiteField Status { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new string getRecordLabel()
        {
            List<string> sParts = new List<string>();
            if (Name != null) if (Name.Value != null) if (Name.Value.ToString() != string.Empty) sParts.Add(Name.Value.ToString());
            if (Version != null) if (Version.Value != null) if (Version.Value.ToString() != string.Empty) sParts.Add(Version.Value.ToString());

            return string.Join(" - ", sParts.ToArray());
        }

        public new string getRecordDescription()
        {
            List<string> sParts = new List<string>();
            if (Description != null) if (Description.Value != null) if (Description.Value.ToString() != string.Empty) sParts.Add(Description.Value.ToString());
            if (Author != null) if (Author.Value != null) if (Author.Value.ToString() != string.Empty) sParts.Add(Author.Value.ToString());

            return string.Join("\r\n", sParts.ToArray());
        }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Name", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Description", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Author", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Version", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("Status", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }
}
