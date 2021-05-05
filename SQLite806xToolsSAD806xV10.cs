using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SAD806x;

namespace SQLite806x
{
    public static class SQLite806xToolsSAD806xV10
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

        public static object addDBVersionRow(SQLite806xDB db806x, ref List<R_SAD806x_DB_Information> rList)
        {
            if (db806x == null) return null;
            if (rList == null) return null;

            R_SAD806x_DB_Information rRow = db806x.newRow<R_SAD806x_DB_Information>();
            rRow.Version.Value = SQLite806xToolsSAD806x.getDBVersion();
            rRow.Date.Value = SQLite806xToolsSAD806x.getDBVersionDate();
            rRow.Comments.Value = SQLite806xToolsSAD806x.getDBVersionComments();
            rList.Add(rRow);
            return rRow;
        }

        public static object addPropertiesRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Properties> rList, S6xProperties s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;

            R_SAD806x_Properties rRow = db806x.newRow<R_SAD806x_Properties>();
            updatePropertiesRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updatePropertiesRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Properties rRow, S6xProperties s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Comments.Value = s6xObject.Comments;
            rRow.Header.Value = s6xObject.Header;
            rRow.Label.Value = s6xObject.Label;
            rRow.NoAutoNumbering.Value = s6xObject.NoNumbering;
            rRow.NoAutoNumberingShortFormat.Value = s6xObject.NoNumberingShortFormat;
            rRow.OutputHeader.Value = s6xObject.OutputHeader;
            rRow.RegistersListOutput.Value = s6xObject.RegListOutput;
            rRow.XdfBaseOffset.Value = s6xObject.XdfBaseOffset;
            rRow.XdfBaseOffsetSubtract.Value = s6xObject.XdfBaseOffsetSubtract;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
            rRow.Ignore8065RegShortcut0x100.Value = s6xObject.Ignore8065RegShortcut0x100;
            rRow.Ignore8065RegShortcut0x100SFR.Value = s6xObject.Ignore8065RegShortcut0x100SFR;
        }

        public static object[] setPropertiesS6x(ref SADS6x sadS6x, R_SAD806x_Properties rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xProperties s6xObject = new S6xProperties();

            s6xObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xObject.Header = (string)rRow.Header.ValueConverted;
            s6xObject.Label = (string)rRow.Label.ValueConverted;
            s6xObject.NoNumbering = (bool)rRow.NoAutoNumbering.ValueConverted;
            s6xObject.NoNumberingShortFormat = (bool)rRow.NoAutoNumberingShortFormat.ValueConverted;
            s6xObject.OutputHeader = (bool)rRow.OutputHeader.ValueConverted;
            s6xObject.RegListOutput = (bool)rRow.RegistersListOutput.ValueConverted;
            s6xObject.XdfBaseOffset = (string)rRow.XdfBaseOffset.ValueConverted;
            s6xObject.XdfBaseOffsetSubtract = (bool)rRow.XdfBaseOffsetSubtract.ValueConverted;
            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;
            s6xObject.Ignore8065RegShortcut0x100 = (bool)rRow.Ignore8065RegShortcut0x100.ValueConverted;
            s6xObject.Ignore8065RegShortcut0x100SFR = (bool)rRow.Ignore8065RegShortcut0x100SFR.ValueConverted;

            sadS6x.Properties = s6xObject;

            return new object[] { s6xObject, null, null };
        }

        public static object addSyncStateRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SyncStates> rList, S6xSyncState s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;

            R_SAD806x_SyncStates rRow = db806x.newRow<R_SAD806x_SyncStates>();
            rRow.SyncType.Value = s6xObject.SyncType;
            rRow.SyncId.Value = s6xObject.SyncId;
            rRow.DateFirstSync.Value = s6xObject.DateFirstSync;
            rRow.DateLastSync.Value = s6xObject.DateLastSync;

            rList.Add(rRow);
            return rRow;
        }

        public static object addTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Tables> rList, S6xTable s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;

            R_SAD806x_Def_Tables rRow = db806x.newRow<R_SAD806x_Def_Tables>();
            updateTableRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Tables rRow, S6xTable s6xObject)
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
            rRow.OutputComments.Value = s6xObject.OutputComments;
            rRow.Rows.Value = s6xObject.RowsNumber;
            rRow.RowsUnits.Value = s6xObject.RowsUnits;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.Signed.Value = s6xObject.SignedOutput;
            rRow.Skip.Value = s6xObject.Skip;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category.Value = s6xObject.Category;
            rRow.Category2.Value = s6xObject.Category2;
            rRow.Category3.Value = s6xObject.Category3;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;

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

        public static object[] setTableS6x(ref SADS6x sadS6x, R_SAD806x_Def_Tables rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xTable s6xObject = new S6xTable();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;
            try { s6xObject.DuplicateNum = ((string)rRow.UniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.UniqueAddCode.ValueConverted); }
            catch { s6xObject.DuplicateNum = 0; }

            if (s6xObject.DuplicateNum == 0)
            {
                if (!sadS6x.slTables.ContainsKey(s6xObject.UniqueAddress)) return null;
                s6xObject = (S6xTable)sadS6x.slTables[s6xObject.UniqueAddress];
            }
            else
            {
                if (!sadS6x.slDupTables.ContainsKey(s6xObject.DuplicateAddress)) return null;
                s6xObject = (S6xTable)sadS6x.slDupTables[s6xObject.DuplicateAddress];
            }

            s6xObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xObject.Skip = (bool)rRow.Skip.ValueConverted;

            return new object[] { s6xObject, null, null };
        }

        public static object addFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Functions> rList, S6xFunction s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;

            R_SAD806x_Def_Functions rRow = db806x.newRow<R_SAD806x_Def_Functions>();
            updateFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Functions rRow, S6xFunction s6xObject)
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
            rRow.OutputComments.Value = s6xObject.OutputComments;
            rRow.OutputScaleExpression.Value = s6xObject.OutputScaleExpression;
            rRow.OutputScalePrecision.Value = s6xObject.OutputScalePrecision;
            rRow.OutputSigned.Value = s6xObject.SignedOutput;
            rRow.OutputUnits.Value = s6xObject.OutputUnits;
            rRow.OutputMin.Value = s6xObject.OutputMin;
            rRow.OutputMax.Value = s6xObject.OutputMax;
            rRow.Rows.Value = s6xObject.RowsNumber;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.Skip.Value = s6xObject.Skip;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category.Value = s6xObject.Category;
            rRow.Category2.Value = s6xObject.Category2;
            rRow.Category3.Value = s6xObject.Category3;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setFunctionS6x(ref SADS6x sadS6x, R_SAD806x_Def_Functions rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xFunction s6xObject = new S6xFunction();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;
            try { s6xObject.DuplicateNum = ((string)rRow.UniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.UniqueAddCode.ValueConverted); }
            catch { s6xObject.DuplicateNum = 0; }

            if (s6xObject.DuplicateNum == 0)
            {
                if (!sadS6x.slFunctions.ContainsKey(s6xObject.UniqueAddress)) return null;
                s6xObject = (S6xFunction)sadS6x.slFunctions[s6xObject.UniqueAddress];
            }
            else
            {
                if (!sadS6x.slDupFunctions.ContainsKey(s6xObject.DuplicateAddress)) return null;
                s6xObject = (S6xFunction)sadS6x.slDupFunctions[s6xObject.DuplicateAddress];
            }

            s6xObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xObject.Skip = (bool)rRow.Skip.ValueConverted;

            return new object[] { s6xObject, null, null };
        }

        public static object addScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Scalars> rList, S6xScalar s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;

            R_SAD806x_Def_Scalars rRow = db806x.newRow<R_SAD806x_Def_Scalars>();
            updateScalarRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Scalars rRow, S6xScalar s6xObject)
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
            rRow.InlineComments.Value = s6xObject.InlineComments;
            rRow.Label.Value = s6xObject.Label;
            rRow.OutputComments.Value = s6xObject.OutputComments;
            rRow.ScaleExpression.Value = s6xObject.ScaleExpression;
            rRow.ScalePrecision.Value = s6xObject.ScalePrecision;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.Signed.Value = s6xObject.Signed;
            rRow.Skip.Value = s6xObject.Skip;
            rRow.Units.Value = s6xObject.Units;
            rRow.Min.Value = s6xObject.Min;
            rRow.Max.Value = s6xObject.Max;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category.Value = s6xObject.Category;
            rRow.Category2.Value = s6xObject.Category2;
            rRow.Category3.Value = s6xObject.Category3;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setScalarS6x(ref SADS6x sadS6x, R_SAD806x_Def_Scalars rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xScalar s6xObject = new S6xScalar();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;
            try { s6xObject.DuplicateNum = ((string)rRow.UniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.UniqueAddCode.ValueConverted); }
            catch { s6xObject.DuplicateNum = 0; }

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

            s6xObject.InlineComments = (bool)rRow.InlineComments.ValueConverted;
            s6xObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xObject.Skip = (bool)rRow.Skip.ValueConverted;

            return new object[] { s6xObject, null, null };
        }

        public static object[] addScalarBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_ScalarsBitFlags> rList, S6xScalar s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isBitFlags) return null;
            if (s6xObject.BitFlags == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xBitFlag s6xBF in s6xObject.BitFlags)
            {
                R_SAD806x_Def_ScalarsBitFlags rRow = db806x.newRow<R_SAD806x_Def_ScalarsBitFlags>();
                updateScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xBF);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_ScalarsBitFlags> rList, S6xScalar s6xObject, S6xBitFlag s6xBitFlag)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xBitFlag == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isBitFlags) return null;
            if (s6xObject.BitFlags == null) return null;

            R_SAD806x_Def_ScalarsBitFlags rRow = db806x.newRow<R_SAD806x_Def_ScalarsBitFlags>();
            updateScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xBitFlag);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_ScalarsBitFlags rRow, S6xScalar s6xObject, S6xBitFlag s6xBitFlag)
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
            rRow.Comments.Value = s6xBitFlag.Comments;
            rRow.HideParent.Value = s6xBitFlag.HideParent;
            rRow.Label.Value = s6xBitFlag.Label;
            rRow.NotSetValue.Value = s6xBitFlag.NotSetValue;
            rRow.SetValue.Value = s6xBitFlag.SetValue;
            rRow.ShortLabel.Value = s6xBitFlag.ShortLabel;
            rRow.Skip.Value = s6xBitFlag.Skip;
            rRow.DateCreated.Value = s6xBitFlag.DateCreated;
            rRow.DateUpdated.Value = s6xBitFlag.DateUpdated;
            rRow.Category.Value = s6xObject.Category;
            rRow.Category2.Value = s6xObject.Category2;
            rRow.Category3.Value = s6xObject.Category3;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setScalarBitFlagS6x(ref SADS6x sadS6x, R_SAD806x_Def_ScalarsBitFlags rRow)
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

            if (!s6xObject.isBitFlags) return null;

            foreach (S6xBitFlag s6xBF in s6xObject.BitFlags)
            {
                if (s6xBF.Position != (int)rRow.Position.ValueConverted) continue;
                s6xBF.HideParent = (bool)rRow.HideParent.ValueConverted;
                s6xBF.NotSetValue = (string)rRow.NotSetValue.ValueConverted;
                s6xBF.SetValue = (string)rRow.SetValue.ValueConverted;
                s6xBF.Skip = (bool)rRow.Skip.ValueConverted;
                return new object[] { s6xBF, s6xObject, null };
            }
            return null;
        }

        public static object addStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Structures> rList, S6xStructure s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;

            R_SAD806x_Def_Structures rRow = db806x.newRow<R_SAD806x_Def_Structures>();
            updateStructureRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Structures rRow, S6xStructure s6xObject)
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
            rRow.OutputComments.Value = s6xObject.OutputComments;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.Skip.Value = s6xObject.Skip;
            rRow.StructureDefinition.Value = s6xObject.StructDef;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category.Value = s6xObject.Category;
            rRow.Category2.Value = s6xObject.Category2;
            rRow.Category3.Value = s6xObject.Category3;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setStructureS6x(ref SADS6x sadS6x, R_SAD806x_Def_Structures rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xStructure s6xObject = new S6xStructure();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;
            try { s6xObject.DuplicateNum = ((string)rRow.UniqueAddCode.ValueConverted == string.Empty) ? 0 : Convert.ToInt32((string)rRow.UniqueAddCode.ValueConverted); }
            catch { s6xObject.DuplicateNum = 0; }

            if (s6xObject.DuplicateNum == 0)
            {
                if (!sadS6x.slStructures.ContainsKey(s6xObject.UniqueAddress)) return null;
                s6xObject = (S6xStructure)sadS6x.slStructures[s6xObject.UniqueAddress];
            }
            else
            {
                if (!sadS6x.slDupStructures.ContainsKey(s6xObject.DuplicateAddress)) return null;
                s6xObject = (S6xStructure)sadS6x.slDupStructures[s6xObject.DuplicateAddress];
            }

            s6xObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xObject.Skip = (bool)rRow.Skip.ValueConverted;

            return new object[] { s6xObject, null, null };
        }

        public static object addRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Routines> rList, S6xRoutine s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;

            R_SAD806x_Def_Routines rRow = db806x.newRow<R_SAD806x_Def_Routines>();
            updateRoutineRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Routines rRow, S6xRoutine s6xObject)
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
            rRow.OutputComments.Value = s6xObject.OutputComments;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.Skip.Value = s6xObject.Skip;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category.Value = s6xObject.Category;
            rRow.Category2.Value = s6xObject.Category2;
            rRow.Category3.Value = s6xObject.Category3;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setRoutineS6x(ref SADS6x sadS6x, R_SAD806x_Def_Routines rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRoutine s6xObject = new S6xRoutine();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;

            if (!sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xRoutine)sadS6x.slRoutines[s6xObject.UniqueAddress];

            s6xObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xObject.Skip = (bool)rRow.Skip.ValueConverted;

            return new object[] { s6xObject, null, null };
        }

        public static object[] addRoutineInputArgRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputArgs> rList, S6xRoutine s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputArguments == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputArgument s6xSubObject in s6xObject.InputArguments)
            {
                R_SAD806x_Def_RoutinesInputArgs rRow = db806x.newRow<R_SAD806x_Def_RoutinesInputArgs>();
                updateRoutineInputArgRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputArgs> rList, S6xRoutine s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputArguments == null) return null;

            R_SAD806x_Def_RoutinesInputArgs rRow = db806x.newRow<R_SAD806x_Def_RoutinesInputArgs>();
            updateRoutineInputArgRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputArgs rRow, S6xRoutine s6xObject, S6xRoutineInputArgument s6xSubObject)
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
            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
        }

        public static object[] setRoutineInputArgS6x(ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputArgs rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            // Nothing to be added.
            return null; 

            /*
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

            S6xRoutineInputArgument[] newArray = null;
            if (s6xObject.InputArguments == null) newArray = new S6xRoutineInputArgument[s6xSubObject.Position];
            else if (s6xObject.InputArguments.Length < s6xSubObject.Position - 1) newArray = new S6xRoutineInputArgument[s6xSubObject.Position];
            else newArray = (S6xRoutineInputArgument[])s6xObject.InputArguments.Clone();

            for (int iPos = 0; iPos < newArray.Length; iPos++)
            {
                if (iPos == s6xSubObject.Position) newArray[iPos] = s6xSubObject;
                else newArray[iPos] = s6xObject.InputArguments[iPos];
            }
            s6xObject.InputArguments = newArray;
            */
        }

        public static object[] addRoutineInputScalarRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputScalars> rList, S6xRoutine s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputScalars == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputScalar s6xSubObject in s6xObject.InputScalars)
            {
                R_SAD806x_Def_RoutinesInputScalars rRow = db806x.newRow<R_SAD806x_Def_RoutinesInputScalars>();
                updateRoutineInputScalarRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addRoutineInputScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputScalars> rList, S6xRoutine s6xObject, S6xRoutineInputScalar s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputScalars == null) return null;

            R_SAD806x_Def_RoutinesInputScalars rRow = db806x.newRow<R_SAD806x_Def_RoutinesInputScalars>();
            updateRoutineInputScalarRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRoutineInputScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputScalars rRow, S6xRoutine s6xObject, S6xRoutineInputScalar s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InputScalars == null) return;

            rRow.RoutineBank.Value = s6xObject.BankNum;
            rRow.RoutineAddress.Value = s6xObject.AddressInt;
            rRow.RoutineUniqueAddCode.Value = 0;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;
            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;
            rRow.Byte.Value = s6xSubObject.Byte;
            rRow.Signed.Value = s6xSubObject.Signed;
            rRow.ForcedScaleExpression.Value = s6xSubObject.ForcedScaleExpression;
            rRow.ForcedScalePrecision.Value = s6xSubObject.ForcedScalePrecision;
            rRow.ForcedUnits.Value = s6xSubObject.ForcedUnits;
            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
        }

        public static object[] setRoutineInputScalarS6x(ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputScalars rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRoutine s6xObject = new S6xRoutine();

            s6xObject.BankNum = (int)rRow.RoutineBank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.RoutineAddress.ValueConverted;

            if (!sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xRoutine)sadS6x.slRoutines[s6xObject.UniqueAddress];

            S6xRoutineInputScalar s6xSubObject = new S6xRoutineInputScalar();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;
            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;
            s6xSubObject.Byte = (bool)rRow.Byte.ValueConverted;
            s6xSubObject.Signed = (bool)rRow.Signed.ValueConverted;
            s6xSubObject.ForcedScaleExpression = (string)rRow.ForcedScaleExpression.ValueConverted;
            s6xSubObject.ForcedScalePrecision = (int)rRow.ForcedScalePrecision.ValueConverted;
            s6xSubObject.ForcedUnits = (string)rRow.ForcedUnits.ValueConverted;
            
            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            Dictionary<string, S6xRoutineInputScalar> lstSubObjects = new Dictionary<string, S6xRoutineInputScalar>();
            if (s6xObject.InputScalars != null)
            {
                foreach (S6xRoutineInputScalar s6xSSO in s6xObject.InputScalars)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InputScalars = new S6xRoutineInputScalar[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InputScalars, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addRoutineInputFunctionRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputFunctions> rList, S6xRoutine s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputFunctions == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputFunction s6xSubObject in s6xObject.InputFunctions)
            {
                R_SAD806x_Def_RoutinesInputFunctions rRow = db806x.newRow<R_SAD806x_Def_RoutinesInputFunctions>();
                updateRoutineInputFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addRoutineInputFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputFunctions> rList, S6xRoutine s6xObject, S6xRoutineInputFunction s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputFunctions == null) return null;

            R_SAD806x_Def_RoutinesInputFunctions rRow = db806x.newRow<R_SAD806x_Def_RoutinesInputFunctions>();
            updateRoutineInputFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRoutineInputFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputFunctions rRow, S6xRoutine s6xObject, S6xRoutineInputFunction s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InputFunctions == null) return;

            rRow.RoutineBank.Value = s6xObject.BankNum;
            rRow.RoutineAddress.Value = s6xObject.AddressInt;
            rRow.RoutineUniqueAddCode.Value = 0;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;
            rRow.VariableInput.Value = s6xSubObject.VariableInput;
            rRow.VariableOutput.Value = s6xSubObject.VariableOutput;

            rRow.ByteInput.Value = s6xSubObject.ByteInput;
            rRow.ForcedInputScaleExpression.Value = s6xSubObject.ForcedInputScaleExpression;
            rRow.ForcedInputScalePrecision.Value = s6xSubObject.ForcedInputScalePrecision;
            rRow.ForcedInputUnits.Value = s6xSubObject.ForcedInputUnits;
            rRow.ForcedOutputScaleExpression.Value = s6xSubObject.ForcedOutputScaleExpression;
            rRow.ForcedOutputScalePrecision.Value = s6xSubObject.ForcedOutputScalePrecision;
            rRow.ForcedOutputUnits.Value = s6xSubObject.ForcedOutputUnits;
            rRow.ForcedRowsNumber.Value = s6xSubObject.ForcedRowsNumber;
            rRow.SignedInput.Value = s6xSubObject.SignedInput;
            rRow.SignedOutput.Value = s6xSubObject.SignedOutput;

            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
        }
        
        public static object[] setRoutineInputFunctionS6x(ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputFunctions rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRoutine s6xObject = new S6xRoutine();

            s6xObject.BankNum = (int)rRow.RoutineBank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.RoutineAddress.ValueConverted;

            if (!sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xRoutine)sadS6x.slRoutines[s6xObject.UniqueAddress];

            S6xRoutineInputFunction s6xSubObject = new S6xRoutineInputFunction();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;
            s6xSubObject.VariableInput = (string)rRow.VariableInput.ValueConverted;
            s6xSubObject.VariableOutput = (string)rRow.VariableOutput.ValueConverted;

            s6xSubObject.ByteInput = (bool)rRow.ByteInput.ValueConverted;
            s6xSubObject.ForcedInputScaleExpression = (string)rRow.ForcedInputScaleExpression.ValueConverted;
            s6xSubObject.ForcedInputScalePrecision = (int)rRow.ForcedInputScalePrecision.ValueConverted;
            s6xSubObject.ForcedInputUnits = (string)rRow.ForcedInputUnits.ValueConverted;
            s6xSubObject.ForcedOutputScaleExpression = (string)rRow.ForcedOutputScaleExpression.ValueConverted;
            s6xSubObject.ForcedOutputScalePrecision = (int)rRow.ForcedOutputScalePrecision.ValueConverted;
            s6xSubObject.ForcedOutputUnits = (string)rRow.ForcedOutputUnits.ValueConverted;
            s6xSubObject.ForcedRowsNumber = (string)rRow.ForcedRowsNumber.ValueConverted;
            s6xSubObject.SignedInput = (bool)rRow.SignedInput.ValueConverted;
            s6xSubObject.SignedOutput = (bool)rRow.SignedOutput.ValueConverted;
            
            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            Dictionary<string, S6xRoutineInputFunction> lstSubObjects = new Dictionary<string, S6xRoutineInputFunction>();
            if (s6xObject.InputFunctions != null)
            {
                foreach (S6xRoutineInputFunction s6xSSO in s6xObject.InputFunctions)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InputFunctions = new S6xRoutineInputFunction[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InputFunctions, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addRoutineInputTableRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputTables> rList, S6xRoutine s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputTables == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputTable s6xSubObject in s6xObject.InputTables)
            {
                R_SAD806x_Def_RoutinesInputTables rRow = db806x.newRow<R_SAD806x_Def_RoutinesInputTables>();
                updateRoutineInputTableRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addRoutineInputTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputTables> rList, S6xRoutine s6xObject, S6xRoutineInputTable s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputTables == null) return null;

            R_SAD806x_Def_RoutinesInputTables rRow = db806x.newRow<R_SAD806x_Def_RoutinesInputTables>();
            updateRoutineInputTableRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRoutineInputTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputTables rRow, S6xRoutine s6xObject, S6xRoutineInputTable s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InputTables == null) return;

            rRow.RoutineBank.Value = s6xObject.BankNum;
            rRow.RoutineAddress.Value = s6xObject.AddressInt;
            rRow.RoutineUniqueAddCode.Value = 0;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;
            rRow.VariableColsNumberReg.Value = s6xSubObject.VariableColsNumberReg;
            rRow.VariableColsReg.Value = s6xSubObject.VariableColsReg;
            rRow.VariableRowsReg.Value = s6xSubObject.VariableRowsReg;
            rRow.VariableOutput.Value = s6xSubObject.VariableOutput;

            rRow.ByteOutput.Value = !s6xSubObject.WordOutput;
            rRow.SignedOutput.Value = s6xSubObject.SignedOutput;
            rRow.ForcedCellsScaleExpression.Value = s6xSubObject.ForcedCellsScaleExpression;
            rRow.ForcedCellsScalePrecision.Value = s6xSubObject.ForcedCellsScalePrecision;
            rRow.ForcedCellsUnits.Value = s6xSubObject.ForcedCellsUnits;
            rRow.ForcedColsNumber.Value = s6xSubObject.ForcedColsNumber;
            rRow.ForcedColsUnits.Value = s6xSubObject.ForcedColsUnits;
            rRow.ForcedRowsNumber.Value = s6xSubObject.ForcedRowsNumber;
            rRow.ForcedRowsUnits.Value = s6xSubObject.ForcedRowsUnits;

            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
        }

        public static object[] setRoutineInputTableS6x(ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputTables rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRoutine s6xObject = new S6xRoutine();

            s6xObject.BankNum = (int)rRow.RoutineBank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.RoutineAddress.ValueConverted;

            if (!sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xRoutine)sadS6x.slRoutines[s6xObject.UniqueAddress];

            S6xRoutineInputTable s6xSubObject = new S6xRoutineInputTable();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;
            s6xSubObject.VariableColsNumberReg = (string)rRow.VariableColsNumberReg.ValueConverted;
            s6xSubObject.VariableColsReg = (string)rRow.VariableColsReg.ValueConverted;
            s6xSubObject.VariableRowsReg = (string)rRow.VariableRowsReg.ValueConverted;
            s6xSubObject.VariableOutput = (string)rRow.VariableOutput.ValueConverted;

            s6xSubObject.WordOutput = !(bool)rRow.ByteOutput.ValueConverted;
            s6xSubObject.SignedOutput = (bool)rRow.SignedOutput.ValueConverted;
            s6xSubObject.ForcedCellsScaleExpression = (string)rRow.ForcedCellsScaleExpression.ValueConverted;
            s6xSubObject.ForcedCellsScalePrecision = (int)rRow.ForcedCellsScalePrecision.ValueConverted;
            s6xSubObject.ForcedCellsUnits = (string)rRow.ForcedCellsUnits.ValueConverted;
            s6xSubObject.ForcedColsNumber = (string)rRow.ForcedColsNumber.ValueConverted;
            s6xSubObject.ForcedColsUnits = (string)rRow.ForcedColsUnits.ValueConverted;
            s6xSubObject.ForcedRowsNumber = (string)rRow.ForcedRowsNumber.ValueConverted;
            s6xSubObject.ForcedRowsUnits = (string)rRow.ForcedRowsUnits.ValueConverted;
            
            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            Dictionary<string, S6xRoutineInputTable> lstSubObjects = new Dictionary<string, S6xRoutineInputTable>();
            if (s6xObject.InputTables != null)
            {
                foreach (S6xRoutineInputTable s6xSSO in s6xObject.InputTables)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InputTables = new S6xRoutineInputTable[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InputTables, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addRoutineInputStructureRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputStructures> rList, S6xRoutine s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputStructures == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputStructure s6xSubObject in s6xObject.InputStructures)
            {
                R_SAD806x_Def_RoutinesInputStructures rRow = db806x.newRow<R_SAD806x_Def_RoutinesInputStructures>();
                updateRoutineInputStructureRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addRoutineInputStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputStructures> rList, S6xRoutine s6xObject, S6xRoutineInputStructure s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputStructures == null) return null;

            R_SAD806x_Def_RoutinesInputStructures rRow = db806x.newRow<R_SAD806x_Def_RoutinesInputStructures>();
            updateRoutineInputStructureRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRoutineInputStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputStructures rRow, S6xRoutine s6xObject, S6xRoutineInputStructure s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InputStructures == null) return;

            rRow.RoutineBank.Value = s6xObject.BankNum;
            rRow.RoutineAddress.Value = s6xObject.AddressInt;
            rRow.RoutineUniqueAddCode.Value = 0;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;
            rRow.VariableNumber.Value = s6xSubObject.VariableNumber;

            rRow.ForcedNumber.Value = s6xSubObject.ForcedNumber;
            rRow.StructDef.Value = s6xSubObject.StructDef;

            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
        }

        public static object[] setRoutineInputStructureS6x(ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputStructures rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRoutine s6xObject = new S6xRoutine();

            s6xObject.BankNum = (int)rRow.RoutineBank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.RoutineAddress.ValueConverted;

            if (!sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xRoutine)sadS6x.slRoutines[s6xObject.UniqueAddress];

            S6xRoutineInputStructure s6xSubObject = new S6xRoutineInputStructure();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;
            s6xSubObject.VariableNumber = (string)rRow.VariableNumber.ValueConverted;

            s6xSubObject.ForcedNumber = (string)rRow.ForcedNumber.ValueConverted;
            s6xSubObject.StructDef = (string)rRow.StructDef.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            Dictionary<string, S6xRoutineInputStructure> lstSubObjects = new Dictionary<string, S6xRoutineInputStructure>();
            if (s6xObject.InputStructures != null)
            {
                foreach (S6xRoutineInputStructure s6xSSO in s6xObject.InputStructures)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InputStructures = new S6xRoutineInputStructure[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InputStructures, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object addOperationRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Operations> rList, S6xOperation s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;

            R_SAD806x_Def_Operations rRow = db806x.newRow<R_SAD806x_Def_Operations>();
            updateOperationRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateOperationRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Operations rRow, S6xOperation s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = 0;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.InlineComments.Value = s6xObject.InlineComments;
            rRow.Label.Value = s6xObject.Label;
            rRow.OutputComments.Value = s6xObject.OutputComments;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.Skip.Value = s6xObject.Skip;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category.Value = s6xObject.Category;
            rRow.Category2.Value = s6xObject.Category2;
            rRow.Category3.Value = s6xObject.Category3;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setOperationS6x(ref SADS6x sadS6x, R_SAD806x_Def_Operations rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xOperation s6xObject = new S6xOperation();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;

            if (!sadS6x.slOperations.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xOperation)sadS6x.slOperations[s6xObject.UniqueAddress];

            s6xObject.InlineComments = (bool)rRow.OutputComments.ValueConverted;
            s6xObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xObject.Skip = (bool)rRow.Skip.ValueConverted;
            return new object[] { s6xObject, null, null };
        }

        public static object addOtherRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Others> rList, S6xOtherAddress s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;

            R_SAD806x_Def_Others rRow = db806x.newRow<R_SAD806x_Def_Others>();
            updateOtherRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateOtherRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Others rRow, S6xOtherAddress s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = 0;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.InlineComments.Value = s6xObject.InlineComments;
            rRow.Label.Value = s6xObject.Label;
            rRow.OutputComments.Value = s6xObject.OutputComments;
            rRow.Skip.Value = s6xObject.Skip;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category.Value = s6xObject.Category;
            rRow.Category2.Value = s6xObject.Category2;
            rRow.Category3.Value = s6xObject.Category3;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setOtherS6x(ref SADS6x sadS6x, R_SAD806x_Def_Others rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xOtherAddress s6xObject = new S6xOtherAddress();

            s6xObject.BankNum = (int)rRow.Bank.ValueConverted;
            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;

            if (!sadS6x.slOtherAddresses.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xOtherAddress)sadS6x.slOtherAddresses[s6xObject.UniqueAddress];

            s6xObject.InlineComments = (bool)rRow.OutputComments.ValueConverted;
            s6xObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xObject.Skip = (bool)rRow.Skip.ValueConverted;
            return new object[] { s6xObject, null, null };
        }

        public static object addRegisterRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Registers> rList, S6xRegister s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;

            R_SAD806x_Def_Registers rRow = db806x.newRow<R_SAD806x_Def_Registers>();
            updateRegisterRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRegisterRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Registers rRow, S6xRegister s6xObject)
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
            rRow.ConstValue.Value = s6xObject.ConstValue;
            rRow.isRBase.Value = s6xObject.isRBase;
            rRow.isRConst.Value = s6xObject.isRConst;
            rRow.Label.Value = s6xObject.Label;
            rRow.ScaleExpression.Value = s6xObject.ScaleExpression;
            rRow.ScalePrecision.Value = s6xObject.ScalePrecision;
            rRow.Skip.Value = s6xObject.Skip;
            rRow.Units.Value = s6xObject.Units;
            rRow.WordLabel.Value = s6xObject.WordLabel;
            rRow.SizeStatus.Value = s6xObject.SizeStatus;
            rRow.SignedStatus.Value = s6xObject.SignedStatus;
            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.Category.Value = s6xObject.Category;
            rRow.Category2.Value = s6xObject.Category2;
            rRow.Category3.Value = s6xObject.Category3;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
        }
        
        public static object[] setRegisterS6x(ref SADS6x sadS6x, R_SAD806x_Def_Registers rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRegister s6xObject = new S6xRegister();

            s6xObject.AddressInt = (int)rRow.Address.ValueConverted;
            s6xObject.AdditionalAddress10 = (string)rRow.AddressAdder.ValueConverted;

            if (!sadS6x.slRegisters.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xRegister)sadS6x.slRegisters[s6xObject.UniqueAddress];

            s6xObject.ConstValue = (string)rRow.ConstValue.ValueConverted;
            s6xObject.isRBase = (bool)rRow.isRBase.ValueConverted;
            s6xObject.isRConst = (bool)rRow.isRConst.ValueConverted;
            s6xObject.ScaleExpression = (string)rRow.ScaleExpression.ValueConverted;
            s6xObject.ScalePrecision = (int)rRow.ScalePrecision.ValueConverted;
            s6xObject.Skip = (bool)rRow.Skip.ValueConverted;
            s6xObject.Units = (string)rRow.Units.ValueConverted;
            s6xObject.SizeStatus = (string)rRow.SizeStatus.ValueConverted;
            s6xObject.SignedStatus = (string)rRow.SignedStatus.ValueConverted;
            return new object[] { s6xObject, null, null };
        }

        public static object[] addRegisterBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RegistersBitFlags> rList, S6xRegister s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isBitFlags) return null;
            if (s6xObject.BitFlags == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xBitFlag s6xSubObject in s6xObject.BitFlags)
            {
                R_SAD806x_Def_RegistersBitFlags rRow = db806x.newRow<R_SAD806x_Def_RegistersBitFlags>();
                updateRegisterBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addRegisterBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RegistersBitFlags> rList, S6xRegister s6xObject, S6xBitFlag s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xSubObject == null) return null;
            if (!s6xObject.Store) return null;
            if (!s6xObject.isBitFlags) return null;
            if (s6xObject.BitFlags == null) return null;

            R_SAD806x_Def_RegistersBitFlags rRow = db806x.newRow<R_SAD806x_Def_RegistersBitFlags>();
            updateRegisterBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateRegisterBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RegistersBitFlags rRow, S6xRegister s6xObject, S6xBitFlag s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xSubObject == null) return;

            rRow.RegisterAddress.Value = s6xObject.AddressInt;
            rRow.RegisterAddressAdder.Value = s6xObject.AdditionalAddress10;
            rRow.RegisterUniqueAddCode.Value = 0;
            rRow.Position.Value = s6xSubObject.Position;
            rRow.Comments.Value = s6xSubObject.Comments;
            rRow.HideParent.Value = s6xSubObject.HideParent;
            rRow.Label.Value = s6xSubObject.Label;
            rRow.NotSetValue.Value = s6xSubObject.NotSetValue;
            rRow.SetValue.Value = s6xSubObject.SetValue;
            rRow.ShortLabel.Value = s6xSubObject.ShortLabel;
            rRow.Skip.Value = s6xSubObject.Skip;
            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
            rRow.Category.Value = s6xSubObject.Category;
            rRow.Category2.Value = s6xSubObject.Category2;
            rRow.Category3.Value = s6xSubObject.Category3;
            rRow.IdentificationStatus.Value = s6xSubObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xSubObject.IdentificationDetails;
        }

        public static object[] setRegisterBitFlagS6x(ref SADS6x sadS6x, R_SAD806x_Def_RegistersBitFlags rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xRegister s6xObject = new S6xRegister();

            s6xObject.AddressInt = (int)rRow.RegisterAddress.ValueConverted;
            s6xObject.AdditionalAddress10 = (string)rRow.RegisterAddressAdder.ValueConverted;

            if (!sadS6x.slRegisters.ContainsKey(s6xObject.UniqueAddress)) return null;
            s6xObject = (S6xRegister)sadS6x.slRegisters[s6xObject.UniqueAddress];

            if (!s6xObject.isBitFlags) return null;

            foreach (S6xBitFlag s6xBF in s6xObject.BitFlags)
            {
                if (s6xBF.Position != (int)rRow.Position.ValueConverted) continue;
                s6xBF.HideParent = (bool)rRow.HideParent.ValueConverted;
                s6xBF.NotSetValue = (string)rRow.NotSetValue.ValueConverted;
                s6xBF.SetValue = (string)rRow.SetValue.ValueConverted;
                s6xBF.Skip = (bool)rRow.Skip.ValueConverted;
                return new object[] { s6xBF, s6xObject, null };
            }

            return null;
        }

        public static object addSignatureRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutines> rList, S6xSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;

            R_SAD806x_SignaturesRoutines rRow = db806x.newRow<R_SAD806x_SignaturesRoutines>();
            updateSignatureRoutineRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutines rRow, S6xSignature s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.for806x.Value = s6xObject.for806x;
            rRow.forBankNum.Value = s6xObject.forBankNum;
            rRow.Forced.Value = s6xObject.Forced;
            rRow.RoutineComments.Value = s6xObject.Comments;
            rRow.RoutineLabel.Value = s6xObject.Label;
            rRow.RoutineOutputComments.Value = s6xObject.OutputComments;
            rRow.RoutineShortLabel.Value = s6xObject.ShortLabel;
            rRow.RoutineDateCreated.Value = s6xObject.RoutineDateCreated;
            rRow.RoutineDateUpdated.Value = s6xObject.RoutineDateUpdated;
            rRow.RoutineCategory.Value = s6xObject.RoutineCategory;
            rRow.RoutineCategory2.Value = s6xObject.RoutineCategory2;
            rRow.RoutineCategory3.Value = s6xObject.RoutineCategory3;
            rRow.RoutineIdentificationStatus.Value = s6xObject.RoutineIdentificationStatus;
            rRow.RoutineIdentificationDetails.Value = s6xObject.RoutineIdentificationDetails;
            rRow.Signature.Value = s6xObject.Signature;
            rRow.SignatureCategory.Value = s6xObject.SignatureCategory;
            rRow.SignatureCategory2.Value = s6xObject.SignatureCategory2;
            rRow.SignatureCategory3.Value = s6xObject.SignatureCategory3;
            rRow.SignatureComments.Value = s6xObject.SignatureComments;
            rRow.SignatureLabel.Value = s6xObject.SignatureLabel;
            rRow.Skip.Value = s6xObject.Skip;
            rRow.UniqueKey.Value = s6xObject.UniqueKey;

            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setSignatureRoutineS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutines rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            if (sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            s6xObject.for806x = (string)rRow.for806x.ValueConverted;
            s6xObject.forBankNum = (string)rRow.forBankNum.ValueConverted;
            s6xObject.Forced = (bool)rRow.Forced.ValueConverted;
            s6xObject.Comments = (string)rRow.RoutineComments.ValueConverted;
            s6xObject.Label = (string)rRow.RoutineLabel.ValueConverted;
            s6xObject.OutputComments = (bool)rRow.RoutineOutputComments.ValueConverted;
            s6xObject.ShortLabel = (string)rRow.RoutineShortLabel.ValueConverted;
            s6xObject.RoutineDateCreated = (DateTime)rRow.RoutineDateCreated.ValueConverted;
            s6xObject.RoutineDateUpdated = (DateTime)rRow.RoutineDateUpdated.ValueConverted;
            s6xObject.RoutineCategory = (string)rRow.RoutineCategory.ValueConverted;
            s6xObject.RoutineCategory2 = (string)rRow.RoutineCategory2.ValueConverted;
            s6xObject.RoutineCategory3 = (string)rRow.RoutineCategory3.ValueConverted;
            s6xObject.RoutineIdentificationStatus = (string)rRow.RoutineIdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xObject.RoutineIdentificationDetails = (string)rRow.RoutineIdentificationDetails.ValueConverted;
            
            s6xObject.Signature = (string)rRow.Signature.ValueConverted;
            s6xObject.SignatureCategory = (string)rRow.SignatureCategory.ValueConverted;
            s6xObject.SignatureCategory2 = (string)rRow.SignatureCategory2.ValueConverted;
            s6xObject.SignatureCategory3 = (string)rRow.SignatureCategory3.ValueConverted;
            s6xObject.SignatureComments = (string)rRow.SignatureComments.ValueConverted;
            s6xObject.SignatureLabel = (string)rRow.SignatureLabel.ValueConverted;
            s6xObject.Skip = (bool)rRow.Skip.ValueConverted;

            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) sadS6x.slSignatures.Add(s6xObject.UniqueKey, s6xObject);

            return new object[] { s6xObject, null, null };
        }

        public static object[] addSignatureRoutineInputArgRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputArgs> rList, S6xSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputArguments == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputArgument s6xSubObject in s6xObject.InputArguments)
            {
                R_SAD806x_SignaturesRoutinesInputArgs rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputArgs>();
                updateSignatureRoutineInputArgRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputArgs> rList, S6xSignature s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputArguments == null) return null;

            R_SAD806x_SignaturesRoutinesInputArgs rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputArgs>();
            updateSignatureRoutineInputArgRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputArgs rRow, S6xSignature s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InputArguments == null) return;

            rRow.SignatureRoutineUniqueKey.Value = s6xObject.UniqueKey;
            rRow.Byte.Value = !s6xSubObject.Word;
            rRow.Encryption.Value = s6xSubObject.Encryption;
            rRow.Pointer.Value = s6xSubObject.Pointer;
            rRow.Position.Value = s6xSubObject.Position;
            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
        }

        public static object[] setSignatureRoutineInputArgS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputArgs rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureRoutineUniqueKey.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            S6xRoutineInputArgument s6xSubObject = new S6xRoutineInputArgument();
            s6xSubObject.Word = !(bool)rRow.Byte.ValueConverted;
            s6xSubObject.Encryption = (int)rRow.Encryption.ValueConverted;
            s6xSubObject.Pointer = (bool)rRow.Pointer.ValueConverted;
            s6xSubObject.Position = (int)rRow.Position.ValueConverted;
            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            S6xRoutineInputArgument[] newArray = null;
            if (s6xObject.InputArguments == null) newArray = new S6xRoutineInputArgument[s6xSubObject.Position];
            else if (s6xObject.InputArguments.Length < s6xSubObject.Position - 1) newArray = new S6xRoutineInputArgument[s6xSubObject.Position];
            else newArray = (S6xRoutineInputArgument[])s6xObject.InputArguments.Clone();

            for (int iPos = 0; iPos < newArray.Length; iPos++)
            {
                if (iPos == s6xSubObject.Position) newArray[iPos] = s6xSubObject;
                else newArray[iPos] = s6xObject.InputArguments[iPos];
            }
            s6xObject.InputArguments = newArray;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addSignatureRoutineInputScalarRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputScalars> rList, S6xSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputScalars == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputScalar s6xSubObject in s6xObject.InputScalars)
            {
                R_SAD806x_SignaturesRoutinesInputScalars rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputScalars>();
                updateSignatureRoutineInputScalarRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureRoutineInputScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputScalars> rList, S6xSignature s6xObject, S6xRoutineInputScalar s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputScalars == null) return null;

            R_SAD806x_SignaturesRoutinesInputScalars rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputScalars>();
            updateSignatureRoutineInputScalarRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineInputScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputScalars rRow, S6xSignature s6xObject, S6xRoutineInputScalar s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InputScalars == null) return;

            rRow.SignatureRoutineUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;
            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;
            rRow.Byte.Value = s6xSubObject.Byte;
            rRow.Signed.Value = s6xSubObject.Signed;
            rRow.ForcedScaleExpression.Value = s6xSubObject.ForcedScaleExpression;
            rRow.ForcedScalePrecision.Value = s6xSubObject.ForcedScalePrecision;
            rRow.ForcedUnits.Value = s6xSubObject.ForcedUnits;
            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
        }

        public static object[] setSignatureRoutineInputScalarS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputScalars rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureRoutineUniqueKey.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            S6xRoutineInputScalar s6xSubObject = new S6xRoutineInputScalar();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;
            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;
            s6xSubObject.Byte = (bool)rRow.Byte.ValueConverted;
            s6xSubObject.Signed = (bool)rRow.Signed.ValueConverted;
            s6xSubObject.ForcedScaleExpression = (string)rRow.ForcedScaleExpression.ValueConverted;
            s6xSubObject.ForcedScalePrecision = (int)rRow.ForcedScalePrecision.ValueConverted;
            s6xSubObject.ForcedUnits = (string)rRow.ForcedUnits.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            Dictionary<string, S6xRoutineInputScalar> lstSubObjects = new Dictionary<string, S6xRoutineInputScalar>();
            if (s6xObject.InputScalars != null)
            {
                foreach (S6xRoutineInputScalar s6xSSO in s6xObject.InputScalars)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InputScalars = new S6xRoutineInputScalar[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InputScalars, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addSignatureRoutineInputFunctionRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputFunctions> rList, S6xSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputFunctions == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputFunction s6xSubObject in s6xObject.InputFunctions)
            {
                R_SAD806x_SignaturesRoutinesInputFunctions rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputFunctions>();
                updateSignatureRoutineInputFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureRoutineInputFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputFunctions> rList, S6xSignature s6xObject, S6xRoutineInputFunction s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputFunctions == null) return null;

            R_SAD806x_SignaturesRoutinesInputFunctions rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputFunctions>();
            updateSignatureRoutineInputFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineInputFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputFunctions rRow, S6xSignature s6xObject, S6xRoutineInputFunction s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InputFunctions == null) return;

            rRow.SignatureRoutineUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;
            rRow.VariableInput.Value = s6xSubObject.VariableInput;
            rRow.VariableOutput.Value = s6xSubObject.VariableOutput;

            rRow.ByteInput.Value = s6xSubObject.ByteInput;
            rRow.ForcedInputScaleExpression.Value = s6xSubObject.ForcedInputScaleExpression;
            rRow.ForcedInputScalePrecision.Value = s6xSubObject.ForcedInputScalePrecision;
            rRow.ForcedInputUnits.Value = s6xSubObject.ForcedInputUnits;
            rRow.ForcedOutputScaleExpression.Value = s6xSubObject.ForcedOutputScaleExpression;
            rRow.ForcedOutputScalePrecision.Value = s6xSubObject.ForcedOutputScalePrecision;
            rRow.ForcedOutputUnits.Value = s6xSubObject.ForcedOutputUnits;
            rRow.ForcedRowsNumber.Value = s6xSubObject.ForcedRowsNumber;
            rRow.SignedInput.Value = s6xSubObject.SignedInput;
            rRow.SignedOutput.Value = s6xSubObject.SignedOutput;

            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
        }

        public static object[] setSignatureRoutineInputFunctionS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputFunctions rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureRoutineUniqueKey.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            S6xRoutineInputFunction s6xSubObject = new S6xRoutineInputFunction();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;
            s6xSubObject.VariableInput = (string)rRow.VariableInput.ValueConverted;
            s6xSubObject.VariableOutput = (string)rRow.VariableOutput.ValueConverted;

            s6xSubObject.ByteInput = (bool)rRow.ByteInput.ValueConverted;
            s6xSubObject.ForcedInputScaleExpression = (string)rRow.ForcedInputScaleExpression.ValueConverted;
            s6xSubObject.ForcedInputScalePrecision = (int)rRow.ForcedInputScalePrecision.ValueConverted;
            s6xSubObject.ForcedInputUnits = (string)rRow.ForcedInputUnits.ValueConverted;
            s6xSubObject.ForcedOutputScaleExpression = (string)rRow.ForcedOutputScaleExpression.ValueConverted;
            s6xSubObject.ForcedOutputScalePrecision = (int)rRow.ForcedOutputScalePrecision.ValueConverted;
            s6xSubObject.ForcedOutputUnits = (string)rRow.ForcedOutputUnits.ValueConverted;
            s6xSubObject.ForcedRowsNumber = (string)rRow.ForcedRowsNumber.ValueConverted;
            s6xSubObject.SignedInput = (bool)rRow.SignedInput.ValueConverted;
            s6xSubObject.SignedOutput = (bool)rRow.SignedOutput.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            Dictionary<string, S6xRoutineInputFunction> lstSubObjects = new Dictionary<string, S6xRoutineInputFunction>();
            if (s6xObject.InputFunctions != null)
            {
                foreach (S6xRoutineInputFunction s6xSSO in s6xObject.InputFunctions)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InputFunctions = new S6xRoutineInputFunction[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InputFunctions, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addSignatureRoutineInputTableRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputTables> rList, S6xSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputTables == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputTable s6xSubObject in s6xObject.InputTables)
            {
                R_SAD806x_SignaturesRoutinesInputTables rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputTables>();
                updateSignatureRoutineInputTableRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureRoutineInputTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputTables> rList, S6xSignature s6xObject, S6xRoutineInputTable s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputTables == null) return null;

            R_SAD806x_SignaturesRoutinesInputTables rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputTables>();
            updateSignatureRoutineInputTableRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineInputTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputTables rRow, S6xSignature s6xObject, S6xRoutineInputTable s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InputTables == null) return;

            rRow.SignatureRoutineUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;
            rRow.VariableColsNumberReg.Value = s6xSubObject.VariableColsNumberReg;
            rRow.VariableColsReg.Value = s6xSubObject.VariableColsReg;
            rRow.VariableRowsReg.Value = s6xSubObject.VariableRowsReg;
            rRow.VariableOutput.Value = s6xSubObject.VariableOutput;

            rRow.ByteOutput.Value = !s6xSubObject.WordOutput;
            rRow.SignedOutput.Value = s6xSubObject.SignedOutput;
            rRow.ForcedCellsScaleExpression.Value = s6xSubObject.ForcedCellsScaleExpression;
            rRow.ForcedCellsScalePrecision.Value = s6xSubObject.ForcedCellsScalePrecision;
            rRow.ForcedCellsUnits.Value = s6xSubObject.ForcedCellsUnits;
            rRow.ForcedColsNumber.Value = s6xSubObject.ForcedColsNumber;
            rRow.ForcedColsUnits.Value = s6xSubObject.ForcedColsUnits;
            rRow.ForcedRowsNumber.Value = s6xSubObject.ForcedRowsNumber;
            rRow.ForcedRowsUnits.Value = s6xSubObject.ForcedRowsUnits;

            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
        }

        public static object[] setSignatureRoutineInputTableS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputTables rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureRoutineUniqueKey.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            S6xRoutineInputTable s6xSubObject = new S6xRoutineInputTable();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;
            s6xSubObject.VariableColsNumberReg = (string)rRow.VariableColsNumberReg.ValueConverted;
            s6xSubObject.VariableColsReg = (string)rRow.VariableColsReg.ValueConverted;
            s6xSubObject.VariableRowsReg = (string)rRow.VariableRowsReg.ValueConverted;
            s6xSubObject.VariableOutput = (string)rRow.VariableOutput.ValueConverted;

            s6xSubObject.WordOutput = !(bool)rRow.ByteOutput.ValueConverted;
            s6xSubObject.SignedOutput = (bool)rRow.SignedOutput.ValueConverted;
            s6xSubObject.ForcedCellsScaleExpression = (string)rRow.ForcedCellsScaleExpression.ValueConverted;
            s6xSubObject.ForcedCellsScalePrecision = (int)rRow.ForcedCellsScalePrecision.ValueConverted;
            s6xSubObject.ForcedCellsUnits = (string)rRow.ForcedCellsUnits.ValueConverted;
            s6xSubObject.ForcedColsNumber = (string)rRow.ForcedColsNumber.ValueConverted;
            s6xSubObject.ForcedColsUnits = (string)rRow.ForcedColsUnits.ValueConverted;
            s6xSubObject.ForcedRowsNumber = (string)rRow.ForcedRowsNumber.ValueConverted;
            s6xSubObject.ForcedRowsUnits = (string)rRow.ForcedRowsUnits.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            Dictionary<string, S6xRoutineInputTable> lstSubObjects = new Dictionary<string, S6xRoutineInputTable>();
            if (s6xObject.InputTables != null)
            {
                foreach (S6xRoutineInputTable s6xSSO in s6xObject.InputTables)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InputTables = new S6xRoutineInputTable[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InputTables, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addSignatureRoutineInputStructureRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputStructures> rList, S6xSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputStructures == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInputStructure s6xSubObject in s6xObject.InputStructures)
            {
                R_SAD806x_SignaturesRoutinesInputStructures rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputStructures>();
                updateSignatureRoutineInputStructureRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureRoutineInputStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputStructures> rList, S6xSignature s6xObject, S6xRoutineInputStructure s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InputStructures == null) return null;

            R_SAD806x_SignaturesRoutinesInputStructures rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputStructures>();
            updateSignatureRoutineInputStructureRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineInputStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputStructures rRow, S6xSignature s6xObject, S6xRoutineInputStructure s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InputStructures == null) return;

            rRow.SignatureRoutineUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;
            rRow.VariableNumber.Value = s6xSubObject.VariableNumber;

            rRow.ForcedNumber.Value = s6xSubObject.ForcedNumber;
            rRow.StructDef.Value = s6xSubObject.StructDef;

            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
        }

        public static object[] setSignatureRoutineInputStructureS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputStructures rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureRoutineUniqueKey.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            S6xRoutineInputStructure s6xSubObject = new S6xRoutineInputStructure();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;
            s6xSubObject.VariableNumber = (string)rRow.VariableNumber.ValueConverted;

            s6xSubObject.ForcedNumber = (string)rRow.ForcedNumber.ValueConverted;
            s6xSubObject.StructDef = (string)rRow.StructDef.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            Dictionary<string, S6xRoutineInputStructure> lstSubObjects = new Dictionary<string, S6xRoutineInputStructure>();
            if (s6xObject.InputStructures != null)
            {
                foreach (S6xRoutineInputStructure s6xSSO in s6xObject.InputStructures)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InputStructures = new S6xRoutineInputStructure[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InputStructures, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addSignatureRoutineInternalScalarRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalScalars> rList, S6xSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InternalScalars == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInternalScalar s6xSubObject in s6xObject.InternalScalars)
            {
                R_SAD806x_SignaturesRoutinesInternalScalars rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalScalars>();
                updateSignatureRoutineInternalScalarRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureRoutineInternalScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalScalars> rList, S6xSignature s6xObject, S6xRoutineInternalScalar s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InternalScalars == null) return null;

            R_SAD806x_SignaturesRoutinesInternalScalars rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalScalars>();
            updateSignatureRoutineInternalScalarRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineInternalScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalScalars rRow, S6xSignature s6xObject, S6xRoutineInternalScalar s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InternalScalars == null) return;

            rRow.SignatureRoutineUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.VariableBank.Value = s6xSubObject.VariableBankNum;
            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;

            rRow.Byte.Value = s6xSubObject.Byte;
            rRow.Comments.Value = s6xSubObject.Comments;
            rRow.InlineComments.Value = s6xSubObject.InlineComments;
            rRow.Label.Value = s6xSubObject.Label;
            rRow.OutputComments.Value = s6xSubObject.OutputComments;
            rRow.ScaleExpression.Value = s6xSubObject.ScaleExpression;
            rRow.ScalePrecision.Value = s6xSubObject.ScalePrecision;
            rRow.ShortLabel.Value = s6xSubObject.ShortLabel;
            rRow.Signed.Value = s6xSubObject.Signed;
            rRow.Units.Value = s6xSubObject.Units;
            rRow.Min.Value = s6xSubObject.Min;
            rRow.Max.Value = s6xSubObject.Max;

            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
            rRow.Category.Value = s6xSubObject.Category;
            rRow.Category2.Value = s6xSubObject.Category2;
            rRow.Category3.Value = s6xSubObject.Category3;
            rRow.IdentificationStatus.Value = s6xSubObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xSubObject.IdentificationDetails;
        }

        public static object[] setSignatureRoutineInternalScalarS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalScalars rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureRoutineUniqueKey.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            S6xRoutineInternalScalar s6xSubObject = new S6xRoutineInternalScalar();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableBankNum = (string)rRow.VariableBank.ValueConverted;
            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;

            s6xSubObject.Byte = (bool)rRow.Byte.ValueConverted;
            s6xSubObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xSubObject.InlineComments = (bool)rRow.InlineComments.ValueConverted;
            s6xSubObject.Label = (string)rRow.Label.ValueConverted;
            s6xSubObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xSubObject.ScaleExpression = (string)rRow.ScaleExpression.ValueConverted;
            s6xSubObject.ScalePrecision = (int)rRow.ScalePrecision.ValueConverted;
            s6xSubObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xSubObject.Signed = (bool)rRow.Signed.ValueConverted;
            s6xSubObject.Units = (string)rRow.Units.ValueConverted;

            s6xSubObject.Min = (string)rRow.Min.ValueConverted;
            s6xSubObject.Max = (string)rRow.Max.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xSubObject.Category = (string)rRow.Category.ValueConverted;
            s6xSubObject.Category2 = (string)rRow.Category2.ValueConverted;
            s6xSubObject.Category3 = (string)rRow.Category3.ValueConverted;
            s6xSubObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xSubObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;

            Dictionary<string, S6xRoutineInternalScalar> lstSubObjects = new Dictionary<string, S6xRoutineInternalScalar>();
            if (s6xObject.InternalScalars != null)
            {
                foreach (S6xRoutineInternalScalar s6xSSO in s6xObject.InternalScalars)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InternalScalars = new S6xRoutineInternalScalar[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InternalScalars, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addSignatureRoutineInternalScalarBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags> rList, S6xSignature s6xObject, S6xRoutineInternalScalar s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xSubObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InternalScalars == null) return null;
            if (!s6xSubObject.isBitFlags) return null;
            if (s6xSubObject.BitFlags == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xBitFlag s6xBF in s6xSubObject.BitFlags)
            {
                R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();
                updateSignatureRoutineInternalScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject, s6xBF);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureRoutineInternalScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags> rList, S6xSignature s6xObject, S6xRoutineInternalScalar s6xSubObject, S6xBitFlag s6xBF)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xSubObject == null) return null;
            if (s6xBF == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InternalScalars == null) return null;
            if (!s6xSubObject.isBitFlags) return null;
            if (s6xSubObject.BitFlags == null) return null;

            R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();
            updateSignatureRoutineInternalScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject, s6xBF);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineInternalScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rRow, S6xSignature s6xObject, S6xRoutineInternalScalar s6xSubObject, S6xBitFlag s6xBF)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xSubObject == null) return;
            if (s6xBF == null) return;

            rRow.SignatureRoutineUniqueKey.Value = s6xObject.UniqueKey;
            rRow.InternalScalarUniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.Comments.Value = s6xBF.Comments;
            rRow.HideParent.Value = s6xBF.HideParent;
            rRow.Label.Value = s6xBF.Label;
            rRow.NotSetValue.Value = s6xBF.NotSetValue;
            rRow.Position.Value = s6xBF.Position;
            rRow.SetValue.Value = s6xBF.SetValue;
            rRow.ShortLabel.Value = s6xBF.ShortLabel;
            rRow.Skip.Value = s6xBF.Skip;

            rRow.DateCreated.Value = s6xBF.DateCreated;
            rRow.DateUpdated.Value = s6xBF.DateUpdated;
            rRow.Category.Value = s6xBF.Category;
            rRow.Category2.Value = s6xBF.Category2;
            rRow.Category3.Value = s6xBF.Category3;
            rRow.IdentificationStatus.Value = s6xBF.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xBF.IdentificationDetails;
        }

        public static object[] setSignatureRoutineInternalScalarBitFlagS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureRoutineUniqueKey.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            if (s6xObject.InputScalars == null) return null;

            S6xRoutineInternalScalar s6xSubObject = null;

            foreach (S6xRoutineInternalScalar s6xSSO in s6xObject.InternalScalars)
            {
                if (s6xSSO.UniqueKey == (string)rRow.InternalScalarUniqueKey.ValueConverted)
                {
                    s6xSubObject = s6xSSO;
                    break;
                }
            }

            if (s6xSubObject == null) return null;

            if (!s6xSubObject.isBitFlags) return null;

            S6xBitFlag s6xBF = new S6xBitFlag();
            s6xBF.Position = (int)rRow.Position.ValueConverted;
            s6xBF.Comments = (string)rRow.Comments.ValueConverted;
            s6xBF.Label = (string)rRow.Label.ValueConverted;
            s6xBF.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xBF.HideParent = (bool)rRow.HideParent.ValueConverted;
            s6xBF.NotSetValue = (string)rRow.NotSetValue.ValueConverted;
            s6xBF.SetValue = (string)rRow.SetValue.ValueConverted;
            s6xBF.Skip = (bool)rRow.Skip.ValueConverted;
            s6xBF.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xBF.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xBF.Category = (string)rRow.Category.ValueConverted;
            s6xBF.Category2 = (string)rRow.Category2.ValueConverted;
            s6xBF.Category3 = (string)rRow.Category3.ValueConverted;
            s6xBF.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xBF.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;
            s6xSubObject.AddBitFlag(s6xBF);
            return new object[] { s6xBF, s6xSubObject, s6xObject };
        }

        public static object[] addSignatureRoutineInternalFunctionRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalFunctions> rList, S6xSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InternalFunctions == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInternalFunction s6xSubObject in s6xObject.InternalFunctions)
            {
                R_SAD806x_SignaturesRoutinesInternalFunctions rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalFunctions>();
                updateSignatureRoutineInternalFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureRoutineInternalFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalFunctions> rList, S6xSignature s6xObject, S6xRoutineInternalFunction s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InternalFunctions == null) return null;

            R_SAD806x_SignaturesRoutinesInternalFunctions rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalFunctions>();
            updateSignatureRoutineInternalFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineInternalFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalFunctions rRow, S6xSignature s6xObject, S6xRoutineInternalFunction s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InternalFunctions == null) return;

            rRow.SignatureRoutineUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.VariableBank.Value = s6xSubObject.VariableBankNum;
            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;

            rRow.Byte.Value = s6xSubObject.ByteInput;
            rRow.Comments.Value = s6xSubObject.Comments;
            rRow.InputScaleExpression.Value = s6xSubObject.InputScaleExpression;
            rRow.InputScalePrecision.Value = s6xSubObject.InputScalePrecision;
            rRow.InputSigned.Value = s6xSubObject.SignedInput;
            rRow.InputUnits.Value = s6xSubObject.InputUnits;
            rRow.InputMin.Value = s6xSubObject.InputMin;
            rRow.InputMax.Value = s6xSubObject.InputMax;
            rRow.Label.Value = s6xSubObject.Label;
            rRow.OutputComments.Value = s6xSubObject.OutputComments;
            rRow.OutputScaleExpression.Value = s6xSubObject.OutputScaleExpression;
            rRow.OutputScalePrecision.Value = s6xSubObject.OutputScalePrecision;
            rRow.OutputSigned.Value = s6xSubObject.SignedOutput;
            rRow.OutputUnits.Value = s6xSubObject.OutputUnits;
            rRow.OutputMin.Value = s6xSubObject.OutputMin;
            rRow.OutputMax.Value = s6xSubObject.OutputMax;
            rRow.Rows.Value = s6xSubObject.RowsNumber;
            rRow.ShortLabel.Value = s6xSubObject.ShortLabel;

            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
            rRow.Category.Value = s6xSubObject.Category;
            rRow.Category2.Value = s6xSubObject.Category2;
            rRow.Category3.Value = s6xSubObject.Category3;
            rRow.IdentificationStatus.Value = s6xSubObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xSubObject.IdentificationDetails;
        }

        public static object[] setSignatureRoutineInternalFunctionS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalFunctions rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureRoutineUniqueKey.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            S6xRoutineInternalFunction s6xSubObject = new S6xRoutineInternalFunction();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableBankNum = (string)rRow.VariableBank.ValueConverted;
            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;

            s6xSubObject.ByteInput = (bool)rRow.Byte.ValueConverted;
            s6xSubObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xSubObject.InputScaleExpression = (string)rRow.InputScaleExpression.ValueConverted;
            s6xSubObject.InputScalePrecision = (int)rRow.InputScalePrecision.ValueConverted;
            s6xSubObject.SignedInput = (bool)rRow.InputSigned.ValueConverted;
            s6xSubObject.InputUnits = (string)rRow.InputUnits.ValueConverted;
            s6xSubObject.Label = (string)rRow.Label.ValueConverted;
            s6xSubObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xSubObject.OutputScaleExpression = (string)rRow.OutputScaleExpression.ValueConverted;
            s6xSubObject.OutputScalePrecision = (int)rRow.OutputScalePrecision.ValueConverted;
            s6xSubObject.SignedOutput = (bool)rRow.OutputSigned.ValueConverted;
            s6xSubObject.OutputUnits = (string)rRow.OutputUnits.ValueConverted;
            s6xSubObject.RowsNumber = (int)rRow.Rows.ValueConverted;
            s6xSubObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;

            s6xSubObject.InputMin = (string)rRow.InputMin.ValueConverted;
            s6xSubObject.InputMax = (string)rRow.InputMax.ValueConverted;
            s6xSubObject.OutputMin = (string)rRow.OutputMin.ValueConverted;
            s6xSubObject.OutputMax = (string)rRow.OutputMax.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xSubObject.Category = (string)rRow.Category.ValueConverted;
            s6xSubObject.Category2 = (string)rRow.Category2.ValueConverted;
            s6xSubObject.Category3 = (string)rRow.Category3.ValueConverted;
            s6xSubObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xSubObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;

            Dictionary<string, S6xRoutineInternalFunction> lstSubObjects = new Dictionary<string, S6xRoutineInternalFunction>();
            if (s6xObject.InternalFunctions != null)
            {
                foreach (S6xRoutineInternalFunction s6xSSO in s6xObject.InternalFunctions)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InternalFunctions = new S6xRoutineInternalFunction[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InternalFunctions, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }
        
        public static object[] addSignatureRoutineInternalTableRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalTables> rList, S6xSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InternalTables == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInternalTable s6xSubObject in s6xObject.InternalTables)
            {
                R_SAD806x_SignaturesRoutinesInternalTables rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalTables>();
                updateSignatureRoutineInternalTableRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureRoutineInternalTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalTables> rList, S6xSignature s6xObject, S6xRoutineInternalTable s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InternalTables == null) return null;

            R_SAD806x_SignaturesRoutinesInternalTables rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalTables>();
            updateSignatureRoutineInternalTableRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineInternalTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalTables rRow, S6xSignature s6xObject, S6xRoutineInternalTable s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InternalTables == null) return;

            rRow.SignatureRoutineUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.VariableBank.Value = s6xSubObject.VariableBankNum;
            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;
            rRow.VariableColsNumber.Value = s6xSubObject.VariableColsNumber;

            rRow.Byte.Value = !s6xSubObject.WordOutput;
            rRow.CellsScaleExpression.Value = s6xSubObject.CellsScaleExpression;
            rRow.CellsScalePrecision.Value = s6xSubObject.CellsScalePrecision;
            rRow.CellsUnits.Value = s6xSubObject.CellsUnits;
            rRow.CellsMin.Value = s6xSubObject.CellsMin;
            rRow.CellsMax.Value = s6xSubObject.CellsMax;
            rRow.ColumnsUnits.Value = s6xSubObject.ColsUnits;
            rRow.Comments.Value = s6xSubObject.Comments;
            rRow.Label.Value = s6xSubObject.Label;
            rRow.OutputComments.Value = s6xSubObject.OutputComments;
            rRow.Rows.Value = s6xSubObject.RowsNumber;
            rRow.RowsUnits.Value = s6xSubObject.RowsUnits;
            rRow.ShortLabel.Value = s6xSubObject.ShortLabel;
            rRow.Signed.Value = s6xSubObject.SignedOutput;

            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
            rRow.Category.Value = s6xSubObject.Category;
            rRow.Category2.Value = s6xSubObject.Category2;
            rRow.Category3.Value = s6xSubObject.Category3;
            rRow.IdentificationStatus.Value = s6xSubObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xSubObject.IdentificationDetails;
        }

        public static object[] setSignatureRoutineInternalTableS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalTables rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureRoutineUniqueKey.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            S6xRoutineInternalTable s6xSubObject = new S6xRoutineInternalTable();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableBankNum = (string)rRow.VariableBank.ValueConverted;
            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;
            s6xSubObject.VariableColsNumber = (string)rRow.VariableColsNumber.ValueConverted;

            s6xSubObject.WordOutput = !(bool)rRow.Byte.ValueConverted;
            s6xSubObject.CellsScaleExpression = (string)rRow.CellsScaleExpression.ValueConverted;
            s6xSubObject.CellsScalePrecision = (int)rRow.CellsScalePrecision.ValueConverted;
            s6xSubObject.CellsUnits = (string)rRow.CellsUnits.ValueConverted;
            s6xSubObject.ColsUnits = (string)rRow.ColumnsUnits.ValueConverted;
            s6xSubObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xSubObject.Label = (string)rRow.Label.ValueConverted;
            s6xSubObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xSubObject.RowsNumber = (int)rRow.Rows.ValueConverted;
            s6xSubObject.RowsUnits = (string)rRow.RowsUnits.ValueConverted;
            s6xSubObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xSubObject.SignedOutput = (bool)rRow.Signed.ValueConverted;

            s6xSubObject.CellsMin = (string)rRow.CellsMin.ValueConverted;
            s6xSubObject.CellsMax = (string)rRow.CellsMax.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xSubObject.Category = (string)rRow.Category.ValueConverted;
            s6xSubObject.Category2 = (string)rRow.Category2.ValueConverted;
            s6xSubObject.Category3 = (string)rRow.Category3.ValueConverted;
            s6xSubObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xSubObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;

            Dictionary<string, S6xRoutineInternalTable> lstSubObjects = new Dictionary<string, S6xRoutineInternalTable>();
            if (s6xObject.InternalTables != null)
            {
                foreach (S6xRoutineInternalTable s6xSSO in s6xObject.InternalTables)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InternalTables = new S6xRoutineInternalTable[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InternalTables, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addSignatureRoutineInternalStructureRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalStructures> rList, S6xSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InternalStructures == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xRoutineInternalStructure s6xSubObject in s6xObject.InternalStructures)
            {
                R_SAD806x_SignaturesRoutinesInternalStructures rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalStructures>();
                updateSignatureRoutineInternalStructureRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureRoutineInternalStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalStructures> rList, S6xSignature s6xObject, S6xRoutineInternalStructure s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (!s6xObject.isAdvanced) return null;
            if (s6xObject.InternalStructures == null) return null;

            R_SAD806x_SignaturesRoutinesInternalStructures rRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalStructures>();
            updateSignatureRoutineInternalStructureRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureRoutineInternalStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalStructures rRow, S6xSignature s6xObject, S6xRoutineInternalStructure s6xSubObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.InternalStructures == null) return;

            rRow.SignatureRoutineUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.VariableBank.Value = s6xSubObject.VariableBankNum;
            rRow.VariableAddress.Value = s6xSubObject.VariableAddress;

            rRow.Comments.Value = s6xSubObject.Comments;
            rRow.Label.Value = s6xSubObject.Label;
            rRow.Number.Value = s6xSubObject.Number;
            rRow.OutputComments.Value = s6xSubObject.OutputComments;
            rRow.ShortLabel.Value = s6xSubObject.ShortLabel;
            rRow.StructureDefinition.Value = s6xSubObject.StructDef;

            rRow.DateCreated.Value = s6xSubObject.DateCreated;
            rRow.DateUpdated.Value = s6xSubObject.DateUpdated;
            rRow.Category.Value = s6xSubObject.Category;
            rRow.Category2.Value = s6xSubObject.Category2;
            rRow.Category3.Value = s6xSubObject.Category3;
            rRow.IdentificationStatus.Value = s6xSubObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xSubObject.IdentificationDetails;
        }

        public static object[] setSignatureRoutineInternalStructureS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalStructures rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xSignature s6xObject = new S6xSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureRoutineUniqueKey.ValueConverted;

            if (!sadS6x.slSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xSignature)sadS6x.slSignatures[s6xObject.UniqueKey];

            S6xRoutineInternalStructure s6xSubObject = new S6xRoutineInternalStructure();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableBankNum = (string)rRow.VariableBank.ValueConverted;
            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;

            s6xSubObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xSubObject.Label = (string)rRow.Label.ValueConverted;
            s6xSubObject.Number = (int)rRow.Number.ValueConverted;
            s6xSubObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xSubObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xSubObject.StructDef = (string)rRow.StructureDefinition.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xSubObject.Category = (string)rRow.Category.ValueConverted;
            s6xSubObject.Category2 = (string)rRow.Category2.ValueConverted;
            s6xSubObject.Category3 = (string)rRow.Category3.ValueConverted;
            s6xSubObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xSubObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;

            Dictionary<string, S6xRoutineInternalStructure> lstSubObjects = new Dictionary<string, S6xRoutineInternalStructure>();
            if (s6xObject.InternalStructures != null)
            {
                foreach (S6xRoutineInternalStructure s6xSSO in s6xObject.InternalStructures)
                {
                    if (!lstSubObjects.ContainsKey(s6xSSO.UniqueKey)) lstSubObjects.Add(s6xSSO.UniqueKey, s6xSSO);
                }
            }
            if (!lstSubObjects.ContainsKey(s6xSubObject.UniqueKey)) lstSubObjects.Add(s6xSubObject.UniqueKey, s6xSubObject);
            else lstSubObjects[s6xSubObject.UniqueKey] = s6xSubObject;

            s6xObject.InternalStructures = new S6xRoutineInternalStructure[lstSubObjects.Count];
            lstSubObjects.Values.CopyTo(s6xObject.InternalStructures, 0);
            lstSubObjects = null;
            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object addSignatureElementRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElements> rList, S6xElementSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;

            R_SAD806x_SignaturesElements rRow = db806x.newRow<R_SAD806x_SignaturesElements>();
            updateSignatureElementRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureElementRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElements rRow, S6xElementSignature s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;

            rRow.for806x.Value = s6xObject.for8061 ? 1 : 2;
            rRow.forBankNum.Value = s6xObject.forBankNum;
            rRow.Forced.Value = s6xObject.Forced;
            rRow.Signature.Value = s6xObject.Signature;
            rRow.SignatureCategory.Value = s6xObject.SignatureCategory;
            rRow.SignatureCategory2.Value = s6xObject.SignatureCategory2;
            rRow.SignatureCategory3.Value = s6xObject.SignatureCategory3;
            rRow.SignatureComments.Value = s6xObject.SignatureComments;
            rRow.SignatureLabel.Value = s6xObject.SignatureLabel;
            rRow.SignatureOpeIncludingElemAddress.Value = s6xObject.SignatureOpeIncludingElemAddress;
            rRow.Skip.Value = s6xObject.Skip;
            rRow.UniqueKey.Value = s6xObject.UniqueKey;

            rRow.DateCreated.Value = s6xObject.DateCreated;
            rRow.DateUpdated.Value = s6xObject.DateUpdated;
            rRow.IdentificationStatus.Value = s6xObject.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.IdentificationDetails;
        }

        public static object[] setSignatureElementS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElements rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xElementSignature s6xObject = new S6xElementSignature();

            s6xObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;

            if (sadS6x.slElementsSignatures.ContainsKey(s6xObject.UniqueKey)) s6xObject = (S6xElementSignature)sadS6x.slElementsSignatures[s6xObject.UniqueKey];

            s6xObject.for8061 = (int)rRow.for806x.ValueConverted == 1;
            s6xObject.forBankNum = (string)rRow.forBankNum.ValueConverted;
            s6xObject.Forced = (bool)rRow.Forced.ValueConverted;
            s6xObject.Signature = (string)rRow.Signature.ValueConverted;
            s6xObject.SignatureCategory = (string)rRow.SignatureCategory.ValueConverted;
            s6xObject.SignatureCategory2 = (string)rRow.SignatureCategory2.ValueConverted;
            s6xObject.SignatureCategory3 = (string)rRow.SignatureCategory3.ValueConverted;
            s6xObject.SignatureComments = (string)rRow.SignatureComments.ValueConverted;
            s6xObject.SignatureLabel = (string)rRow.SignatureLabel.ValueConverted;
            s6xObject.SignatureOpeIncludingElemAddress = (string)rRow.SignatureOpeIncludingElemAddress.ValueConverted;
            s6xObject.Skip = (bool)rRow.Skip.ValueConverted;

            s6xObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;

            if (!sadS6x.slElementsSignatures.ContainsKey(s6xObject.UniqueKey)) sadS6x.slElementsSignatures.Add(s6xObject.UniqueKey, s6xObject);
            return new object[] { s6xObject, null, null };
        }

        public static object addSignatureElementInternalScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalScalars> rList, S6xElementSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Scalar == null) return null;

            R_SAD806x_SignaturesElementsInternalScalars rRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalScalars>();
            updateSignatureElementInternalScalarRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureElementInternalScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalScalars rRow, S6xElementSignature s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Scalar == null) return;

            rRow.SignatureElementUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xObject.Scalar.UniqueKey;

            rRow.VariableBank.Value = s6xObject.Scalar.VariableBankNum;
            rRow.VariableAddress.Value = s6xObject.Scalar.VariableAddress;

            rRow.Byte.Value = s6xObject.Scalar.Byte;
            rRow.Comments.Value = s6xObject.Scalar.Comments;
            rRow.InlineComments.Value = s6xObject.Scalar.InlineComments;
            rRow.Label.Value = s6xObject.Scalar.Label;
            rRow.OutputComments.Value = s6xObject.Scalar.OutputComments;
            rRow.ScaleExpression.Value = s6xObject.Scalar.ScaleExpression;
            rRow.ScalePrecision.Value = s6xObject.Scalar.ScalePrecision;
            rRow.ShortLabel.Value = s6xObject.Scalar.ShortLabel;
            rRow.Signed.Value = s6xObject.Scalar.Signed;
            rRow.Units.Value = s6xObject.Scalar.Units;
            rRow.Min.Value = s6xObject.Scalar.Min;
            rRow.Max.Value = s6xObject.Scalar.Max;

            rRow.DateCreated.Value = s6xObject.Scalar.DateCreated;
            rRow.DateUpdated.Value = s6xObject.Scalar.DateUpdated;
            rRow.Category.Value = s6xObject.Scalar.Category;
            rRow.Category2.Value = s6xObject.Scalar.Category2;
            rRow.Category3.Value = s6xObject.Scalar.Category3;
            rRow.IdentificationStatus.Value = s6xObject.Scalar.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.Scalar.IdentificationDetails;
        }

        public static object[] setSignatureElementInternalScalarS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalScalars rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xElementSignature s6xObject = new S6xElementSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureElementUniqueKey.ValueConverted;

            if (!sadS6x.slElementsSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xElementSignature)sadS6x.slElementsSignatures[s6xObject.UniqueKey];

            S6xRoutineInternalScalar s6xSubObject = new S6xRoutineInternalScalar();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableBankNum = (string)rRow.VariableBank.ValueConverted;
            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;

            s6xSubObject.Byte = (bool)rRow.Byte.ValueConverted;
            s6xSubObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xSubObject.InlineComments = (bool)rRow.InlineComments.ValueConverted;
            s6xSubObject.Label = (string)rRow.Label.ValueConverted;
            s6xSubObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xSubObject.ScaleExpression = (string)rRow.ScaleExpression.ValueConverted;
            s6xSubObject.ScalePrecision = (int)rRow.ScalePrecision.ValueConverted;
            s6xSubObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xSubObject.Signed = (bool)rRow.Signed.ValueConverted;
            s6xSubObject.Units = (string)rRow.Units.ValueConverted;

            s6xSubObject.Min = (string)rRow.Min.ValueConverted;
            s6xSubObject.Max = (string)rRow.Max.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xSubObject.Category = (string)rRow.Category.ValueConverted;
            s6xSubObject.Category2 = (string)rRow.Category2.ValueConverted;
            s6xSubObject.Category3 = (string)rRow.Category3.ValueConverted;
            s6xSubObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xSubObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;

            s6xObject.Scalar = s6xSubObject;
            s6xObject.Function = null;
            s6xObject.Table = null;
            s6xObject.Structure = null;

            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object[] addSignatureElementInternalScalarBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags> rList, S6xElementSignature s6xObject, S6xRoutineInternalScalar s6xSubObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xSubObject == null) return null;
            if (!s6xSubObject.isBitFlags) return null;
            if (s6xSubObject.BitFlags == null) return null;

            List<object> resList = new List<object>();
            foreach (S6xBitFlag s6xBF in s6xSubObject.BitFlags)
            {
                R_SAD806x_SignaturesElementsInternalScalarsBitFlags rRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>();
                updateSignatureElementInternalScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject, s6xBF);
                rList.Add(rRow);
                resList.Add(rRow);
            }
            return resList.ToArray();
        }

        public static object addSignatureElementInternalScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags> rList, S6xElementSignature s6xObject, S6xRoutineInternalScalar s6xSubObject, S6xBitFlag s6xBF)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xSubObject == null) return null;
            if (s6xBF == null) return null;
            if (!s6xSubObject.isBitFlags) return null;
            if (s6xSubObject.BitFlags == null) return null;

            R_SAD806x_SignaturesElementsInternalScalarsBitFlags rRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>();
            updateSignatureElementInternalScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject, s6xBF);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureElementInternalScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalScalarsBitFlags rRow, S6xElementSignature s6xObject, S6xRoutineInternalScalar s6xSubObject, S6xBitFlag s6xBF)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xSubObject == null) return;
            if (s6xBF == null) return;

            rRow.SignatureElementUniqueKey.Value = s6xObject.UniqueKey;
            rRow.InternalScalarUniqueKey.Value = s6xSubObject.UniqueKey;

            rRow.Comments.Value = s6xBF.Comments;
            rRow.HideParent.Value = s6xBF.HideParent;
            rRow.Label.Value = s6xBF.Label;
            rRow.NotSetValue.Value = s6xBF.NotSetValue;
            rRow.Position.Value = s6xBF.Position;
            rRow.SetValue.Value = s6xBF.SetValue;
            rRow.ShortLabel.Value = s6xBF.ShortLabel;
            rRow.Skip.Value = s6xBF.Skip;

            rRow.DateCreated.Value = s6xBF.DateCreated;
            rRow.DateUpdated.Value = s6xBF.DateUpdated;
            rRow.Category.Value = s6xBF.Category;
            rRow.Category2.Value = s6xBF.Category2;
            rRow.Category3.Value = s6xBF.Category3;
            rRow.IdentificationStatus.Value = s6xBF.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xBF.IdentificationDetails;
        }

        public static object[] setSignatureElementInternalScalarBitFlagS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalScalarsBitFlags rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xElementSignature s6xObject = new S6xElementSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureElementUniqueKey.ValueConverted;

            if (!sadS6x.slElementsSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xElementSignature)sadS6x.slElementsSignatures[s6xObject.UniqueKey];

            if (s6xObject.Scalar == null) return null;

            if (!s6xObject.Scalar.isBitFlags) return null;

            S6xBitFlag s6xBF = new S6xBitFlag();
            s6xBF.Position = (int)rRow.Position.ValueConverted;
            s6xBF.Comments = (string)rRow.Comments.ValueConverted;
            s6xBF.Label = (string)rRow.Label.ValueConverted;
            s6xBF.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xBF.HideParent = (bool)rRow.HideParent.ValueConverted;
            s6xBF.NotSetValue = (string)rRow.NotSetValue.ValueConverted;
            s6xBF.SetValue = (string)rRow.SetValue.ValueConverted;
            s6xBF.Skip = (bool)rRow.Skip.ValueConverted;
            s6xBF.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xBF.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xBF.Category = (string)rRow.Category.ValueConverted;
            s6xBF.Category2 = (string)rRow.Category2.ValueConverted;
            s6xBF.Category3 = (string)rRow.Category3.ValueConverted;
            s6xBF.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xBF.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;
            s6xObject.Scalar.AddBitFlag(s6xBF);

            return new object[] { s6xBF, s6xObject.Scalar, s6xObject };
        }

        public static object addSignatureElementInternalFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalFunctions> rList, S6xElementSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Function == null) return null;

            R_SAD806x_SignaturesElementsInternalFunctions rRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalFunctions>();
            updateSignatureElementInternalFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureElementInternalFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalFunctions rRow, S6xElementSignature s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Function == null) return;

            rRow.SignatureElementUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xObject.Function.UniqueKey;

            rRow.VariableBank.Value = s6xObject.Function.VariableBankNum;
            rRow.VariableAddress.Value = s6xObject.Function.VariableAddress;

            rRow.Byte.Value = s6xObject.Function.ByteInput;
            rRow.Comments.Value = s6xObject.Function.Comments;
            rRow.InputScaleExpression.Value = s6xObject.Function.InputScaleExpression;
            rRow.InputScalePrecision.Value = s6xObject.Function.InputScalePrecision;
            rRow.InputSigned.Value = s6xObject.Function.SignedInput;
            rRow.InputUnits.Value = s6xObject.Function.InputUnits;
            rRow.InputMin.Value = s6xObject.Function.InputMin;
            rRow.InputMax.Value = s6xObject.Function.InputMax;
            rRow.Label.Value = s6xObject.Function.Label;
            rRow.OutputComments.Value = s6xObject.Function.OutputComments;
            rRow.OutputScaleExpression.Value = s6xObject.Function.OutputScaleExpression;
            rRow.OutputScalePrecision.Value = s6xObject.Function.OutputScalePrecision;
            rRow.OutputSigned.Value = s6xObject.Function.SignedOutput;
            rRow.OutputUnits.Value = s6xObject.Function.OutputUnits;
            rRow.OutputMin.Value = s6xObject.Function.OutputMin;
            rRow.OutputMax.Value = s6xObject.Function.OutputMax;
            rRow.Rows.Value = s6xObject.Function.RowsNumber;
            rRow.ShortLabel.Value = s6xObject.Function.ShortLabel;

            rRow.DateCreated.Value = s6xObject.Function.DateCreated;
            rRow.DateUpdated.Value = s6xObject.Function.DateUpdated;
            rRow.Category.Value = s6xObject.Function.Category;
            rRow.Category2.Value = s6xObject.Function.Category2;
            rRow.Category3.Value = s6xObject.Function.Category3;
            rRow.IdentificationStatus.Value = s6xObject.Function.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.Function.IdentificationDetails;
        }

        public static object[] setSignatureElementInternalFunctionS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalFunctions rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xElementSignature s6xObject = new S6xElementSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureElementUniqueKey.ValueConverted;

            if (!sadS6x.slElementsSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xElementSignature)sadS6x.slElementsSignatures[s6xObject.UniqueKey];

            S6xRoutineInternalFunction s6xSubObject = new S6xRoutineInternalFunction();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableBankNum = (string)rRow.VariableBank.ValueConverted;
            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;

            s6xSubObject.ByteInput = (bool)rRow.Byte.ValueConverted;
            s6xSubObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xSubObject.InputScaleExpression = (string)rRow.InputScaleExpression.ValueConverted;
            s6xSubObject.InputScalePrecision = (int)rRow.InputScalePrecision.ValueConverted;
            s6xSubObject.SignedInput = (bool)rRow.InputSigned.ValueConverted;
            s6xSubObject.InputUnits = (string)rRow.InputUnits.ValueConverted;
            s6xSubObject.Label = (string)rRow.Label.ValueConverted;
            s6xSubObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xSubObject.OutputScaleExpression = (string)rRow.OutputScaleExpression.ValueConverted;
            s6xSubObject.OutputScalePrecision = (int)rRow.OutputScalePrecision.ValueConverted;
            s6xSubObject.SignedOutput = (bool)rRow.OutputSigned.ValueConverted;
            s6xSubObject.OutputUnits = (string)rRow.OutputUnits.ValueConverted;
            s6xSubObject.RowsNumber = (int)rRow.Rows.ValueConverted;
            s6xSubObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;

            s6xSubObject.InputMin = (string)rRow.InputMin.ValueConverted;
            s6xSubObject.InputMax = (string)rRow.InputMax.ValueConverted;
            s6xSubObject.OutputMin = (string)rRow.OutputMin.ValueConverted;
            s6xSubObject.OutputMax = (string)rRow.OutputMax.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xSubObject.Category = (string)rRow.Category.ValueConverted;
            s6xSubObject.Category2 = (string)rRow.Category2.ValueConverted;
            s6xSubObject.Category3 = (string)rRow.Category3.ValueConverted;
            s6xSubObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xSubObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;

            s6xObject.Scalar = null;
            s6xObject.Function = s6xSubObject;
            s6xObject.Table = null;
            s6xObject.Structure = null;

            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object addSignatureElementInternalTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalTables> rList, S6xElementSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Table == null) return null;

            R_SAD806x_SignaturesElementsInternalTables rRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalTables>();
            updateSignatureElementInternalTableRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureElementInternalTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalTables rRow, S6xElementSignature s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Table == null) return;

            rRow.SignatureElementUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xObject.Table.UniqueKey;

            rRow.VariableBank.Value = s6xObject.Table.VariableBankNum;
            rRow.VariableAddress.Value = s6xObject.Table.VariableAddress;
            rRow.VariableColsNumber.Value = s6xObject.Table.VariableColsNumber;

            rRow.Byte.Value = !s6xObject.Table.WordOutput;
            rRow.CellsScaleExpression.Value = s6xObject.Table.CellsScaleExpression;
            rRow.CellsScalePrecision.Value = s6xObject.Table.CellsScalePrecision;
            rRow.CellsUnits.Value = s6xObject.Table.CellsUnits;
            rRow.CellsMin.Value = s6xObject.Table.CellsMin;
            rRow.CellsMax.Value = s6xObject.Table.CellsMax;
            rRow.ColumnsUnits.Value = s6xObject.Table.ColsUnits;
            rRow.Comments.Value = s6xObject.Table.Comments;
            rRow.Label.Value = s6xObject.Table.Label;
            rRow.OutputComments.Value = s6xObject.Table.OutputComments;
            rRow.Rows.Value = s6xObject.Table.RowsNumber;
            rRow.RowsUnits.Value = s6xObject.Table.RowsUnits;
            rRow.ShortLabel.Value = s6xObject.Table.ShortLabel;
            rRow.Signed.Value = s6xObject.Table.SignedOutput;

            rRow.DateCreated.Value = s6xObject.Table.DateCreated;
            rRow.DateUpdated.Value = s6xObject.Table.DateUpdated;
            rRow.Category.Value = s6xObject.Table.Category;
            rRow.Category2.Value = s6xObject.Table.Category2;
            rRow.Category3.Value = s6xObject.Table.Category3;
            rRow.IdentificationStatus.Value = s6xObject.Table.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.Table.IdentificationDetails;
        }

        public static object[] setSignatureElementInternalTableS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalTables rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xElementSignature s6xObject = new S6xElementSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureElementUniqueKey.ValueConverted;

            if (!sadS6x.slElementsSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xElementSignature)sadS6x.slElementsSignatures[s6xObject.UniqueKey];

            S6xRoutineInternalTable s6xSubObject = new S6xRoutineInternalTable();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableBankNum = (string)rRow.VariableBank.ValueConverted;
            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;
            s6xSubObject.VariableColsNumber = (string)rRow.VariableColsNumber.ValueConverted;

            s6xSubObject.WordOutput = !(bool)rRow.Byte.ValueConverted;
            s6xSubObject.CellsScaleExpression = (string)rRow.CellsScaleExpression.ValueConverted;
            s6xSubObject.CellsScalePrecision = (int)rRow.CellsScalePrecision.ValueConverted;
            s6xSubObject.CellsUnits = (string)rRow.CellsUnits.ValueConverted;
            s6xSubObject.ColsUnits = (string)rRow.ColumnsUnits.ValueConverted;
            s6xSubObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xSubObject.Label = (string)rRow.Label.ValueConverted;
            s6xSubObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xSubObject.RowsNumber = (int)rRow.Rows.ValueConverted;
            s6xSubObject.RowsUnits = (string)rRow.RowsUnits.ValueConverted;
            s6xSubObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xSubObject.SignedOutput = (bool)rRow.Signed.ValueConverted;

            s6xSubObject.CellsMin = (string)rRow.CellsMin.ValueConverted;
            s6xSubObject.CellsMax = (string)rRow.CellsMax.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xSubObject.Category = (string)rRow.Category.ValueConverted;
            s6xSubObject.Category2 = (string)rRow.Category2.ValueConverted;
            s6xSubObject.Category3 = (string)rRow.Category3.ValueConverted;
            s6xSubObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xSubObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;

            s6xObject.Scalar = null;
            s6xObject.Function = null;
            s6xObject.Table = s6xSubObject;
            s6xObject.Structure = null;

            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static object addSignatureElementInternalStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalStructures> rList, S6xElementSignature s6xObject)
        {
            if (db806x == null) return null;
            if (sadS6x == null) return null;
            if (rList == null) return null;
            if (s6xObject == null) return null;
            if (s6xObject.Structure == null) return null;

            R_SAD806x_SignaturesElementsInternalStructures rRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalStructures>();
            updateSignatureElementInternalStructureRow(ref db806x, ref sadS6x, rRow, s6xObject);
            rList.Add(rRow);
            return rRow;
        }

        public static void updateSignatureElementInternalStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalStructures rRow, S6xElementSignature s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rRow == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Structure == null) return;

            rRow.SignatureElementUniqueKey.Value = s6xObject.UniqueKey;
            rRow.UniqueKey.Value = s6xObject.Structure.UniqueKey;

            rRow.VariableBank.Value = s6xObject.Structure.VariableBankNum;
            rRow.VariableAddress.Value = s6xObject.Structure.VariableAddress;

            rRow.Comments.Value = s6xObject.Structure.Comments;
            rRow.Label.Value = s6xObject.Structure.Label;
            rRow.Number.Value = s6xObject.Structure.Number;
            rRow.OutputComments.Value = s6xObject.Structure.OutputComments;
            rRow.ShortLabel.Value = s6xObject.Structure.ShortLabel;
            rRow.StructureDefinition.Value = s6xObject.Structure.StructDef;

            rRow.DateCreated.Value = s6xObject.Structure.DateCreated;
            rRow.DateUpdated.Value = s6xObject.Structure.DateUpdated;
            rRow.Category.Value = s6xObject.Structure.Category;
            rRow.Category2.Value = s6xObject.Structure.Category2;
            rRow.Category3.Value = s6xObject.Structure.Category3;
            rRow.IdentificationStatus.Value = s6xObject.Structure.IdentificationStatus;
            rRow.IdentificationDetails.Value = s6xObject.Structure.IdentificationDetails;
        }

        public static object[] setSignatureElementInternalStructureS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalStructures rRow)
        {
            if (sadS6x == null) return null;
            if (rRow == null) return null;

            S6xElementSignature s6xObject = new S6xElementSignature();

            s6xObject.UniqueKey = (string)rRow.SignatureElementUniqueKey.ValueConverted;

            if (!sadS6x.slElementsSignatures.ContainsKey(s6xObject.UniqueKey)) return null;
            s6xObject = (S6xElementSignature)sadS6x.slElementsSignatures[s6xObject.UniqueKey];

            S6xRoutineInternalStructure s6xSubObject = new S6xRoutineInternalStructure();
            s6xSubObject.UniqueKey = (string)rRow.UniqueKey.ValueConverted;

            s6xSubObject.VariableBankNum = (string)rRow.VariableBank.ValueConverted;
            s6xSubObject.VariableAddress = (string)rRow.VariableAddress.ValueConverted;

            s6xSubObject.Comments = (string)rRow.Comments.ValueConverted;
            s6xSubObject.Label = (string)rRow.Label.ValueConverted;
            s6xSubObject.Number = (int)rRow.Number.ValueConverted;
            s6xSubObject.OutputComments = (bool)rRow.OutputComments.ValueConverted;
            s6xSubObject.ShortLabel = (string)rRow.ShortLabel.ValueConverted;
            s6xSubObject.StructDef = (string)rRow.StructureDefinition.ValueConverted;

            s6xSubObject.DateCreated = (DateTime)rRow.DateCreated.ValueConverted;
            s6xSubObject.DateUpdated = (DateTime)rRow.DateUpdated.ValueConverted;
            s6xSubObject.Category = (string)rRow.Category.ValueConverted;
            s6xSubObject.Category2 = (string)rRow.Category2.ValueConverted;
            s6xSubObject.Category3 = (string)rRow.Category3.ValueConverted;
            s6xSubObject.IdentificationStatus = (string)rRow.IdentificationStatus.ValueConverted == string.Empty ? 0 : Convert.ToInt32(rRow.IdentificationStatus.ValueConverted);
            s6xSubObject.IdentificationDetails = (string)rRow.IdentificationDetails.ValueConverted;

            s6xObject.Scalar = null;
            s6xObject.Function = null;
            s6xObject.Table = null;
            s6xObject.Structure = s6xSubObject;

            return new object[] { s6xSubObject, s6xObject, null };
        }

        public static List<object> getScalarSubRows(ref SQLite806xDB db806x, R_SAD806x_Def_Scalars rRow)
        {
            if (db806x == null) return null;
            if (rRow == null) return null;

            R_SAD806x_Def_ScalarsBitFlags sampleRow = db806x.newRow<R_SAD806x_Def_ScalarsBitFlags>();
            string subWhereClause = string.Empty;
            subWhereClause = string.Format("{0} = {1} AND {2} = {3} AND ({4} = '{5}' OR ({4} IS NULL AND '{5}' = ''))", sampleRow.ScalarBank.Name, rRow.Bank.ValueConverted, sampleRow.ScalarAddress.Name, rRow.Address.ValueConverted, sampleRow.ScalarUniqueAddCode.Name, rRow.UniqueAddCode.ValueConverted);

            List<object> rList = new List<object>();
            try { foreach (object oRow in db806x.Read<R_SAD806x_Def_ScalarsBitFlags>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { rList = null; }

            return rList;
        }

        public static List<object> getRoutineSubRows(ref SQLite806xDB db806x, R_SAD806x_Def_Routines rRow)
        {
            if (db806x == null) return null;
            if (rRow == null) return null;

            string subWhereClause = string.Empty;
            List<object> rList = new List<object>();

            R_SAD806x_Def_RoutinesInputArgs argRow = db806x.newRow<R_SAD806x_Def_RoutinesInputArgs>();
            subWhereClause = string.Format("{0} = {1} AND {2} = {3} AND ({4} = '{5}' OR ({4} IS NULL AND '{5}' = ''))", argRow.RoutineBank.Name, rRow.Bank.ValueConverted, argRow.RoutineAddress.Name, rRow.Address.ValueConverted, argRow.RoutineUniqueAddCode.Name, rRow.UniqueAddCode.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_Def_RoutinesInputArgs>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_Def_RoutinesInputScalars scalarRow = db806x.newRow<R_SAD806x_Def_RoutinesInputScalars>();
            subWhereClause = string.Format("{0} = {1} AND {2} = {3} AND ({4} = '{5}' OR ({4} IS NULL AND '{5}' = ''))", scalarRow.RoutineBank.Name, rRow.Bank.ValueConverted, scalarRow.RoutineAddress.Name, rRow.Address.ValueConverted, scalarRow.RoutineUniqueAddCode.Name, rRow.UniqueAddCode.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_Def_RoutinesInputScalars>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_Def_RoutinesInputFunctions functionRow = db806x.newRow<R_SAD806x_Def_RoutinesInputFunctions>();
            subWhereClause = string.Format("{0} = {1} AND {2} = {3} AND ({4} = '{5}' OR ({4} IS NULL AND '{5}' = ''))", functionRow.RoutineBank.Name, rRow.Bank.ValueConverted, functionRow.RoutineAddress.Name, rRow.Address.ValueConverted, functionRow.RoutineUniqueAddCode.Name, rRow.UniqueAddCode.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_Def_RoutinesInputFunctions>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_Def_RoutinesInputTables tableRow = db806x.newRow<R_SAD806x_Def_RoutinesInputTables>();
            subWhereClause = string.Format("{0} = {1} AND {2} = {3} AND ({4} = '{5}' OR ({4} IS NULL AND '{5}' = ''))", tableRow.RoutineBank.Name, rRow.Bank.ValueConverted, tableRow.RoutineAddress.Name, rRow.Address.ValueConverted, tableRow.RoutineUniqueAddCode.Name, rRow.UniqueAddCode.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_Def_RoutinesInputTables>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_Def_RoutinesInputStructures structureRow = db806x.newRow<R_SAD806x_Def_RoutinesInputStructures>();
            subWhereClause = string.Format("{0} = {1} AND {2} = {3} AND ({4} = '{5}' OR ({4} IS NULL AND '{5}' = ''))", structureRow.RoutineBank.Name, rRow.Bank.ValueConverted, structureRow.RoutineAddress.Name, rRow.Address.ValueConverted, structureRow.RoutineUniqueAddCode.Name, rRow.UniqueAddCode.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_Def_RoutinesInputStructures>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            return rList;
        }

        public static List<object> getRegisterSubRows(ref SQLite806xDB db806x, R_SAD806x_Def_Registers rRow)
        {
            if (db806x == null) return null;
            if (rRow == null) return null;

            R_SAD806x_Def_RegistersBitFlags sampleRow = db806x.newRow<R_SAD806x_Def_RegistersBitFlags>();
            string subWhereClause = string.Empty;
            subWhereClause = string.Format("{0} = {1} AND ({2} = '{3}' OR ({2} IS NULL AND '{3}' = '')) AND ({4} = '{5}' OR ({4} IS NULL AND '{5}' = ''))", sampleRow.RegisterAddress.Name, rRow.Address.ValueConverted, sampleRow.RegisterAddressAdder.Name, rRow.AddressAdder.ValueConverted, sampleRow.RegisterUniqueAddCode.Name, rRow.UniqueAddCode.ValueConverted);

            List<object> rList = new List<object>();
            try { foreach (object oRow in db806x.Read<R_SAD806x_Def_RegistersBitFlags>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { rList = null; }

            return rList;
        }

        public static List<object> getSignatureRoutineSubRows(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutines rRow)
        {
            if (db806x == null) return null;
            if (rRow == null) return null;

            string subWhereClause = string.Empty;
            List<object> rList = new List<object>();

            R_SAD806x_SignaturesRoutinesInputArgs argRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputArgs>();
            subWhereClause = string.Format("{0} = '{1}'", argRow.SignatureRoutineUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInputArgs>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesRoutinesInputScalars scalarRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputScalars>();
            subWhereClause = string.Format("{0} = '{1}'", scalarRow.SignatureRoutineUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInputScalars>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesRoutinesInputFunctions functionRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputFunctions>();
            subWhereClause = string.Format("{0} = '{1}'", functionRow.SignatureRoutineUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInputFunctions>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesRoutinesInputTables tableRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputTables>();
            subWhereClause = string.Format("{0} = '{1}'", tableRow.SignatureRoutineUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInputTables>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesRoutinesInputStructures structureRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInputStructures>();
            subWhereClause = string.Format("{0} = '{1}'", structureRow.SignatureRoutineUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInputStructures>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesRoutinesInternalScalars intScalarRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalScalars>();
            subWhereClause = string.Format("{0} = '{1}'", intScalarRow.SignatureRoutineUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInternalScalars>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags intScalarBitFlagRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();
            subWhereClause = string.Format("{0} = '{1}'", intScalarBitFlagRow.SignatureRoutineUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesRoutinesInternalFunctions intFunctionRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalFunctions>();
            subWhereClause = string.Format("{0} = '{1}'", intFunctionRow.SignatureRoutineUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInternalFunctions>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesRoutinesInternalTables intTableRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalTables>();
            subWhereClause = string.Format("{0} = '{1}'", intTableRow.SignatureRoutineUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInternalTables>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesRoutinesInternalStructures intStructureRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalStructures>();
            subWhereClause = string.Format("{0} = '{1}'", intStructureRow.SignatureRoutineUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInternalStructures>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            return rList;
        }

        public static List<object> getSignatureRoutineInternalScalarSubRows(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalScalars rRow)
        {
            if (db806x == null) return null;
            if (rRow == null) return null;

            string subWhereClause = string.Empty;
            List<object> rList = new List<object>();

            R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags sampleRow = db806x.newRow<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();
            subWhereClause = string.Format("{0} = '{1}' AND {2} = '{3}'", sampleRow.SignatureRoutineUniqueKey.Name, rRow.SignatureRoutineUniqueKey, sampleRow.InternalScalarUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            return rList;
        }

        public static List<object> getSignatureElementSubRows(ref SQLite806xDB db806x, R_SAD806x_SignaturesElements rRow)
        {
            if (db806x == null) return null;
            if (rRow == null) return null;

            string subWhereClause = string.Empty;
            List<object> rList = new List<object>();

            R_SAD806x_SignaturesElementsInternalScalars intScalarRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalScalars>();
            subWhereClause = string.Format("{0} = '{1}'", intScalarRow.SignatureElementUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesElementsInternalScalars>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesElementsInternalScalarsBitFlags intScalarBitFlagRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>();
            subWhereClause = string.Format("{0} = '{1}'", intScalarBitFlagRow.SignatureElementUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesElementsInternalFunctions intFunctionRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalFunctions>();
            subWhereClause = string.Format("{0} = '{1}'", intFunctionRow.SignatureElementUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesElementsInternalFunctions>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesElementsInternalTables intTableRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalTables>();
            subWhereClause = string.Format("{0} = '{1}'", intTableRow.SignatureElementUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesElementsInternalTables>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            R_SAD806x_SignaturesElementsInternalStructures intStructureRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalStructures>();
            subWhereClause = string.Format("{0} = '{1}'", intStructureRow.SignatureElementUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesElementsInternalStructures>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            return rList;
        }

        public static List<object> getSignatureElementInternalScalarSubRows(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalScalars rRow)
        {
            if (db806x == null) return null;
            if (rRow == null) return null;

            string subWhereClause = string.Empty;
            List<object> rList = new List<object>();

            R_SAD806x_SignaturesElementsInternalScalarsBitFlags sampleRow = db806x.newRow<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>();
            subWhereClause = string.Format("{0} = '{1}' AND {2} = '{3}'", sampleRow.SignatureElementUniqueKey.Name, rRow.SignatureElementUniqueKey, sampleRow.InternalScalarUniqueKey.Name, rRow.UniqueKey.ValueConverted);
            try { foreach (object oRow in db806x.Read<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>(-1, null, subWhereClause, string.Empty)) rList.Add(oRow); }
            catch { return null; }

            return rList;
        }

        public static bool deleteTableRow(ref SQLite806xDB db806x, R_SAD806x_Def_Tables rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_Tables> rList = new List<R_SAD806x_Def_Tables>() { rRow };
            return db806x.Delete<R_SAD806x_Def_Tables>(rList);
        }

        public static bool deleteFunctionRow(ref SQLite806xDB db806x, R_SAD806x_Def_Functions rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_Functions> rList = new List<R_SAD806x_Def_Functions>() { rRow };
            return db806x.Delete<R_SAD806x_Def_Functions>(rList);
        }

        public static bool deleteScalarRow(ref SQLite806xDB db806x, R_SAD806x_Def_Scalars rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                List<object> rSubListObjects = getScalarSubRows(ref db806x, rRow);
                if (rSubListObjects == null) return false;
                List<R_SAD806x_Def_ScalarsBitFlags> rSubList = new List<R_SAD806x_Def_ScalarsBitFlags>();
                foreach (R_SAD806x_Def_ScalarsBitFlags rSubRow in rSubListObjects) rSubList.Add(rSubRow);
                if (rSubList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_Def_ScalarsBitFlags>(rSubList)) return false;
                }
            }

            List<R_SAD806x_Def_Scalars> rList = new List<R_SAD806x_Def_Scalars>() { rRow };
            return db806x.Delete<R_SAD806x_Def_Scalars>(rList);
        }

        public static bool deleteScalarBitFlagRow(ref SQLite806xDB db806x, R_SAD806x_Def_ScalarsBitFlags rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_ScalarsBitFlags> rList = new List<R_SAD806x_Def_ScalarsBitFlags>() { rRow };
            return db806x.Delete<R_SAD806x_Def_ScalarsBitFlags>(rList);
        }

        public static bool deleteStructureRow(ref SQLite806xDB db806x, R_SAD806x_Def_Structures rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_Structures> rList = new List<R_SAD806x_Def_Structures>() { rRow };
            return db806x.Delete<R_SAD806x_Def_Structures>(rList);
        }

        public static bool deleteRoutineRow(ref SQLite806xDB db806x, R_SAD806x_Def_Routines rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                List<object> rSubListObjects = getRoutineSubRows(ref db806x, rRow);
                if (rSubListObjects == null) return false;

                List<R_SAD806x_Def_RoutinesInputArgs> rArgsList = new List<R_SAD806x_Def_RoutinesInputArgs>();
                List<R_SAD806x_Def_RoutinesInputScalars> rScalarsList = new List<R_SAD806x_Def_RoutinesInputScalars>();
                List<R_SAD806x_Def_RoutinesInputFunctions> rFunctionsList = new List<R_SAD806x_Def_RoutinesInputFunctions>();
                List<R_SAD806x_Def_RoutinesInputTables> rTablesList = new List<R_SAD806x_Def_RoutinesInputTables>();
                List<R_SAD806x_Def_RoutinesInputStructures> rStructuresList = new List<R_SAD806x_Def_RoutinesInputStructures>();

                foreach (object rSubRow in rSubListObjects)
                {
                    if (rSubRow == null) continue;
                    if (rSubRow.GetType() == typeof(R_SAD806x_Def_RoutinesInputArgs)) rArgsList.Add((R_SAD806x_Def_RoutinesInputArgs)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_Def_RoutinesInputScalars)) rScalarsList.Add((R_SAD806x_Def_RoutinesInputScalars)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_Def_RoutinesInputFunctions)) rFunctionsList.Add((R_SAD806x_Def_RoutinesInputFunctions)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_Def_RoutinesInputTables)) rTablesList.Add((R_SAD806x_Def_RoutinesInputTables)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_Def_RoutinesInputStructures)) rStructuresList.Add((R_SAD806x_Def_RoutinesInputStructures)rSubRow);
                }

                if (rArgsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_Def_RoutinesInputArgs>(rArgsList)) return false;
                }
                if (rScalarsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_Def_RoutinesInputScalars>(rScalarsList)) return false;
                }
                if (rFunctionsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_Def_RoutinesInputFunctions>(rFunctionsList)) return false;
                }
                if (rTablesList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_Def_RoutinesInputTables>(rTablesList)) return false;
                }
                if (rStructuresList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_Def_RoutinesInputStructures>(rStructuresList)) return false;
                }
            }

            List<R_SAD806x_Def_Routines> rList = new List<R_SAD806x_Def_Routines>() { rRow };
            return db806x.Delete<R_SAD806x_Def_Routines>(rList);
        }

        public static bool deleteRoutineInputArgRow(ref SQLite806xDB db806x, R_SAD806x_Def_RoutinesInputArgs rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_RoutinesInputArgs> rList = new List<R_SAD806x_Def_RoutinesInputArgs>() { rRow };
            return db806x.Delete<R_SAD806x_Def_RoutinesInputArgs>(rList);
        }

        public static bool deleteRoutineInputScalarRow(ref SQLite806xDB db806x, R_SAD806x_Def_RoutinesInputScalars rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_RoutinesInputScalars> rList = new List<R_SAD806x_Def_RoutinesInputScalars>() { rRow };
            return db806x.Delete<R_SAD806x_Def_RoutinesInputScalars>(rList);
        }

        public static bool deleteRoutineInputFunctionRow(ref SQLite806xDB db806x, R_SAD806x_Def_RoutinesInputFunctions rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_RoutinesInputFunctions> rList = new List<R_SAD806x_Def_RoutinesInputFunctions>() { rRow };
            return db806x.Delete<R_SAD806x_Def_RoutinesInputFunctions>(rList);
        }

        public static bool deleteRoutineInputTableRow(ref SQLite806xDB db806x, R_SAD806x_Def_RoutinesInputTables rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_RoutinesInputTables> rList = new List<R_SAD806x_Def_RoutinesInputTables>() { rRow };
            return db806x.Delete<R_SAD806x_Def_RoutinesInputTables>(rList);
        }

        public static bool deleteRoutineInputStructureRow(ref SQLite806xDB db806x, R_SAD806x_Def_RoutinesInputStructures rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_RoutinesInputStructures> rList = new List<R_SAD806x_Def_RoutinesInputStructures>() { rRow };
            return db806x.Delete<R_SAD806x_Def_RoutinesInputStructures>(rList);
        }

        public static bool deleteOperationRow(ref SQLite806xDB db806x, R_SAD806x_Def_Operations rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_Operations> rList = new List<R_SAD806x_Def_Operations>() { rRow };
            return db806x.Delete<R_SAD806x_Def_Operations>(rList);
        }

        public static bool deleteOtherRow(ref SQLite806xDB db806x, R_SAD806x_Def_Others rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_Others> rList = new List<R_SAD806x_Def_Others>() { rRow };
            return db806x.Delete<R_SAD806x_Def_Others>(rList);
        }

        public static bool deleteRegisterRow(ref SQLite806xDB db806x, R_SAD806x_Def_Registers rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                List<object> rSubListObjects = getRegisterSubRows(ref db806x, rRow);
                if (rSubListObjects == null) return false;
                List<R_SAD806x_Def_RegistersBitFlags> rSubList = new List<R_SAD806x_Def_RegistersBitFlags>();
                foreach (R_SAD806x_Def_RegistersBitFlags rSubRow in rSubListObjects) rSubList.Add(rSubRow);
                if (rSubList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_Def_RegistersBitFlags>(rSubList)) return false;
                }
            }

            List<R_SAD806x_Def_Registers> rList = new List<R_SAD806x_Def_Registers>() { rRow };
            return db806x.Delete<R_SAD806x_Def_Registers>(rList);
        }

        public static bool deleteRegisterBitFlagRow(ref SQLite806xDB db806x, R_SAD806x_Def_RegistersBitFlags rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_Def_RegistersBitFlags> rList = new List<R_SAD806x_Def_RegistersBitFlags>() { rRow };
            return db806x.Delete<R_SAD806x_Def_RegistersBitFlags>(rList);
        }

        public static bool deleteSignatureRoutineRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutines rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                List<object> rSubListObjects = getSignatureRoutineSubRows(ref db806x, rRow);
                if (rSubListObjects == null) return false;

                List<R_SAD806x_SignaturesRoutinesInputArgs> rArgsList = new List<R_SAD806x_SignaturesRoutinesInputArgs>();
                List<R_SAD806x_SignaturesRoutinesInputScalars> rScalarsList = new List<R_SAD806x_SignaturesRoutinesInputScalars>();
                List<R_SAD806x_SignaturesRoutinesInputFunctions> rFunctionsList = new List<R_SAD806x_SignaturesRoutinesInputFunctions>();
                List<R_SAD806x_SignaturesRoutinesInputTables> rTablesList = new List<R_SAD806x_SignaturesRoutinesInputTables>();
                List<R_SAD806x_SignaturesRoutinesInputStructures> rStructuresList = new List<R_SAD806x_SignaturesRoutinesInputStructures>();
                List<R_SAD806x_SignaturesRoutinesInternalScalars> rInternalScalarsList = new List<R_SAD806x_SignaturesRoutinesInternalScalars>();
                List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags> rInternalScalarsBitFlagsList = new List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();
                List<R_SAD806x_SignaturesRoutinesInternalFunctions> rInternalFunctionsList = new List<R_SAD806x_SignaturesRoutinesInternalFunctions>();
                List<R_SAD806x_SignaturesRoutinesInternalTables> rInternalTablesList = new List<R_SAD806x_SignaturesRoutinesInternalTables>();
                List<R_SAD806x_SignaturesRoutinesInternalStructures> rInternalStructuresList = new List<R_SAD806x_SignaturesRoutinesInternalStructures>();

                foreach (object rSubRow in rSubListObjects)
                {
                    if (rSubRow == null) continue;
                    if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesRoutinesInputArgs)) rArgsList.Add((R_SAD806x_SignaturesRoutinesInputArgs)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesRoutinesInputScalars)) rScalarsList.Add((R_SAD806x_SignaturesRoutinesInputScalars)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesRoutinesInputFunctions)) rFunctionsList.Add((R_SAD806x_SignaturesRoutinesInputFunctions)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesRoutinesInputTables)) rTablesList.Add((R_SAD806x_SignaturesRoutinesInputTables)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesRoutinesInputStructures)) rStructuresList.Add((R_SAD806x_SignaturesRoutinesInputStructures)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalScalars)) rInternalScalarsList.Add((R_SAD806x_SignaturesRoutinesInternalScalars)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags)) rInternalScalarsBitFlagsList.Add((R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalFunctions)) rInternalFunctionsList.Add((R_SAD806x_SignaturesRoutinesInternalFunctions)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalTables)) rInternalTablesList.Add((R_SAD806x_SignaturesRoutinesInternalTables)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalStructures)) rInternalStructuresList.Add((R_SAD806x_SignaturesRoutinesInternalStructures)rSubRow);
                }

                if (rArgsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInputArgs>(rArgsList)) return false;
                }
                if (rScalarsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInputScalars>(rScalarsList)) return false;
                }
                if (rFunctionsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInputFunctions>(rFunctionsList)) return false;
                }
                if (rTablesList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInputTables>(rTablesList)) return false;
                }
                if (rStructuresList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInputStructures>(rStructuresList)) return false;
                }
                if (rInternalScalarsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInternalScalars>(rInternalScalarsList)) return false;
                }
                if (rInternalScalarsBitFlagsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>(rInternalScalarsBitFlagsList)) return false;
                }
                if (rInternalFunctionsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInternalFunctions>(rInternalFunctionsList)) return false;
                }
                if (rInternalTablesList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInternalTables>(rInternalTablesList)) return false;
                }
                if (rInternalStructuresList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInternalStructures>(rInternalStructuresList)) return false;
                }
            }

            List<R_SAD806x_SignaturesRoutines> rList = new List<R_SAD806x_SignaturesRoutines>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutines>(rList);
        }

        public static bool deleteSignatureRoutineInputArgRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInputArgs rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesRoutinesInputArgs> rList = new List<R_SAD806x_SignaturesRoutinesInputArgs>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutinesInputArgs>(rList);
        }

        public static bool deleteSignatureRoutineInputScalarRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInputScalars rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesRoutinesInputScalars> rList = new List<R_SAD806x_SignaturesRoutinesInputScalars>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutinesInputScalars>(rList);
        }

        public static bool deleteSignatureRoutineInputFunctionRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInputFunctions rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesRoutinesInputFunctions> rList = new List<R_SAD806x_SignaturesRoutinesInputFunctions>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutinesInputFunctions>(rList);
        }

        public static bool deleteSignatureRoutineInputTableRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInputTables rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesRoutinesInputTables> rList = new List<R_SAD806x_SignaturesRoutinesInputTables>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutinesInputTables>(rList);
        }

        public static bool deleteSignatureRoutineInputStructureRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInputStructures rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesRoutinesInputStructures> rList = new List<R_SAD806x_SignaturesRoutinesInputStructures>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutinesInputStructures>(rList);
        }

        public static bool deleteSignatureRoutineInternalScalarRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalScalars rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                List<object> rSubListObjects = getSignatureRoutineInternalScalarSubRows(ref db806x, rRow);
                if (rSubListObjects == null) return false;
                List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags> rSubList = new List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>();
                foreach (R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rSubRow in rSubListObjects) rSubList.Add(rSubRow);
                if (rSubList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>(rSubList)) return false;
                }
            }

            List<R_SAD806x_SignaturesRoutinesInternalScalars> rList = new List<R_SAD806x_SignaturesRoutinesInternalScalars>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutinesInternalScalars>(rList);
        }

        public static bool deleteSignatureRoutineInternalScalarBitFlagRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags> rList = new List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>(rList);
        }

        public static bool deleteSignatureRoutineInternalFunctionRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalFunctions rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesRoutinesInternalFunctions> rList = new List<R_SAD806x_SignaturesRoutinesInternalFunctions>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutinesInternalFunctions>(rList);
        }

        public static bool deleteSignatureRoutineInternalTableRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalTables rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesRoutinesInternalTables> rList = new List<R_SAD806x_SignaturesRoutinesInternalTables>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutinesInternalTables>(rList);
        }

        public static bool deleteSignatureRoutineInternalStructureRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalStructures rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesRoutinesInternalStructures> rList = new List<R_SAD806x_SignaturesRoutinesInternalStructures>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesRoutinesInternalStructures>(rList);
        }

        public static bool deleteSignatureElementRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElements rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                List<object> rSubListObjects = getSignatureElementSubRows(ref db806x, rRow);
                if (rSubListObjects == null) return false;

                List<R_SAD806x_SignaturesElementsInternalScalars> rInternalScalarsList = new List<R_SAD806x_SignaturesElementsInternalScalars>();
                List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags> rInternalScalarsBitFlagsList = new List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>();
                List<R_SAD806x_SignaturesElementsInternalFunctions> rInternalFunctionsList = new List<R_SAD806x_SignaturesElementsInternalFunctions>();
                List<R_SAD806x_SignaturesElementsInternalTables> rInternalTablesList = new List<R_SAD806x_SignaturesElementsInternalTables>();
                List<R_SAD806x_SignaturesElementsInternalStructures> rInternalStructuresList = new List<R_SAD806x_SignaturesElementsInternalStructures>();

                foreach (object rSubRow in rSubListObjects)
                {
                    if (rSubRow == null) continue;
                    if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesElementsInternalScalars)) rInternalScalarsList.Add((R_SAD806x_SignaturesElementsInternalScalars)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesElementsInternalScalarsBitFlags)) rInternalScalarsBitFlagsList.Add((R_SAD806x_SignaturesElementsInternalScalarsBitFlags)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesElementsInternalFunctions)) rInternalFunctionsList.Add((R_SAD806x_SignaturesElementsInternalFunctions)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesElementsInternalTables)) rInternalTablesList.Add((R_SAD806x_SignaturesElementsInternalTables)rSubRow);
                    else if (rSubRow.GetType() == typeof(R_SAD806x_SignaturesElementsInternalStructures)) rInternalStructuresList.Add((R_SAD806x_SignaturesElementsInternalStructures)rSubRow);
                }

                if (rInternalScalarsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesElementsInternalScalars>(rInternalScalarsList)) return false;
                }
                if (rInternalScalarsBitFlagsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>(rInternalScalarsBitFlagsList)) return false;
                }
                if (rInternalFunctionsList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesElementsInternalFunctions>(rInternalFunctionsList)) return false;
                }
                if (rInternalTablesList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesElementsInternalTables>(rInternalTablesList)) return false;
                }
                if (rInternalStructuresList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesElementsInternalStructures>(rInternalStructuresList)) return false;
                }
            }

            List<R_SAD806x_SignaturesElements> rList = new List<R_SAD806x_SignaturesElements>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesElements>(rList);
        }

        public static bool deleteSignatureElementInternalScalarRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalScalars rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                List<object> rSubListObjects = getSignatureElementInternalScalarSubRows(ref db806x, rRow);
                if (rSubListObjects == null) return false;
                List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags> rSubList = new List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>();
                foreach (R_SAD806x_SignaturesElementsInternalScalarsBitFlags rSubRow in rSubListObjects) rSubList.Add(rSubRow);
                if (rSubList.Count > 0)
                {
                    if (!db806x.Delete<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>(rSubList)) return false;
                }
            }

            List<R_SAD806x_SignaturesElementsInternalScalars> rList = new List<R_SAD806x_SignaturesElementsInternalScalars>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesElementsInternalScalars>(rList);
        }

        public static bool deleteSignatureElementInternalScalarBitFlagRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalScalarsBitFlags rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags> rList = new List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>(rList);
        }

        public static bool deleteSignatureElementInternalFunctionRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalFunctions rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesElementsInternalFunctions> rList = new List<R_SAD806x_SignaturesElementsInternalFunctions>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesElementsInternalFunctions>(rList);
        }

        public static bool deleteSignatureElementInternalTableRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalTables rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesElementsInternalTables> rList = new List<R_SAD806x_SignaturesElementsInternalTables>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesElementsInternalTables>(rList);
        }

        public static bool deleteSignatureElementInternalStructureRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalStructures rRow, bool recursively)
        {
            if (db806x == null) return false;
            if (rRow == null) return false;

            if (recursively)
            {
                // No Linked Records
            }

            List<R_SAD806x_SignaturesElementsInternalStructures> rList = new List<R_SAD806x_SignaturesElementsInternalStructures>() { rRow };
            return db806x.Delete<R_SAD806x_SignaturesElementsInternalStructures>(rList);
        }

        public static void initSchema(ref List<T_SQLiteTable> sqlTables)
        {
            if (sqlTables == null) sqlTables = new List<T_SQLiteTable>();

            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_DB_Information>("Information", "Database Information"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Properties>("Properties", "Properties"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_Functions>("Functions Definition", "Definition for functions"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_Operations>("Operations Definition", "Definition for operations"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_Others>("Others Definition", "Definition for other addresses"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_Registers>("Registers Definition", "Definition for registers"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_RegistersBitFlags>("Registers Bit Flags", "Bit flags for registers"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_Routines>("Routines Definition", "Definition for routines"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_RoutinesInputArgs>("Routines Input Arguments", "Input arguments for routines"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_RoutinesInputScalars>("Routines Input Scalars", "Input scalars for routines"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_RoutinesInputFunctions>("Routines Input Functions", "Input functions for routines"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_RoutinesInputTables>("Routines Input Tables", "Input tables for routines"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_RoutinesInputStructures>("Routines Input Structures", "Input structures for routines"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_Scalars>("Scalars Definition", "Definition for scalars"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_ScalarsBitFlags>("Scalars Bit Flags", "Bit flags for scalars"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_Structures>("Structures Definition", "Definition for structures"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_Def_Tables>("Tables Definition", "Definition for tables"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesElements>("Elements Signatures Definition", "Definition for elements signatures"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesElementsInternalScalars >("Elements Signatures Internal Scalars Definition", "Definition for elements signatures internal scalars"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesElementsInternalScalarsBitFlags>("Elements Signatures Internal Scalars Bit Flags Definition", "Definition for elements signatures internal scalars bit flags"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesElementsInternalFunctions>("Elements Signatures Internal Functions Definition", "Definition for elements functions internal scalars"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesElementsInternalTables>("Elements Signatures Internal Tables Definition", "Definition for elements signatures internal tables"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesElementsInternalStructures>("Elements Signatures Internal Structures Definition", "Definition for elements signatures internal structures"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutines>("Routines Signatures Definition", "Definition for routines signatures"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutinesInputArgs>("Routines Signatures Input Arguments Definition", "Definition for routines signatures input arguments"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutinesInputScalars>("Routines Signatures Input Scalars Definition", "Definition for routines signatures input scalars"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutinesInputFunctions>("Routines Signatures Input Functions Definition", "Definition for routines functions input scalars"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutinesInputTables>("Routines Signatures Input Tables Definition", "Definition for routines signatures input tables"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutinesInputStructures>("Routines Signatures Input Structures Definition", "Definition for routines signatures input structures"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutinesInternalScalars>("Routines Signatures Internal Scalars Definition", "Definition for routines signatures internal scalars"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags>("Routines Signatures Internal Scalars Bit Flags Definition", "Definition for routines signatures internal scalars bit flags"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutinesInternalFunctions>("Routines Signatures Internal Functions Definition", "Definition for routines functions internal scalars"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutinesInternalTables>("Routines Signatures Internal Tables Definition", "Definition for routines signatures internal tables"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SignaturesRoutinesInternalStructures>("Routines Signatures Internal Structures Definition", "Definition for routines signatures internal structures"));
            sqlTables.Add(SQLite806xTools.initTable<R_SAD806x_SyncStates>("Synchronization States", "Synchronization States"));
        }
    }

    public class R_SAD806x_DB_Information : R_SQLite_DB_Information {}

    public class R_SAD806x_Properties : R_SQLite_Core
    {
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField Header { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField NoAutoNumbering { get; set; }
        public F_SQLiteField NoAutoNumberingShortFormat { get; set; }
        public F_SQLiteField OutputHeader { get; set; }
        public F_SQLiteField RegistersListOutput { get; set; }
        public F_SQLiteField XdfBaseOffset { get; set; }
        public F_SQLiteField XdfBaseOffsetSubtract { get; set; }
        public F_SQLiteField IdentificationStatus { get; set; }
        public F_SQLiteField IdentificationDetails { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public F_SQLiteField Ignore8065RegShortcut0x100 { get; set; }
        public F_SQLiteField Ignore8065RegShortcut0x100SFR { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("Header", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("Label", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("NoAutoNumbering", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("NoAutoNumberingShortFormat", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("OutputHeader", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("RegistersListOutput", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("XdfBaseOffset", new F_SQLiteFieldProperties("NVARCHAR (10)"));
            dResult.Add("XdfBaseOffsetSubtract", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("IdentificationStatus", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("Ignore8065RegShortcut0x100", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Ignore8065RegShortcut0x100SFR", new F_SQLiteFieldProperties("BOOLEAN"));
            return dResult;
        }
    }

    public class R_SAD806x_SyncStates : R_SQLite_Core
    {
        public F_SQLiteField SyncType { get; set; }
        public F_SQLiteField SyncId { get; set; }
        public F_SQLiteField DateFirstSync { get; set; }
        public F_SQLiteField DateLastSync { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SyncType", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SyncId", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("DateFirstSync", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateLastSync", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_Elements_Core : R_SQLite_Core
    {
        public F_SQLiteField Bank { get; set; }
        public F_SQLiteField Address { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField ShortLabel { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField OutputComments { get; set; }
        public F_SQLiteField Skip { get; set; }
        public F_SQLiteField Category { get; set; }
        public F_SQLiteField Category2 { get; set; }
        public F_SQLiteField Category3 { get; set; }
        public F_SQLiteField IdentificationStatus { get; set; }
        public F_SQLiteField IdentificationDetails { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Bank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Address", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("UniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            dResult.Add("ShortLabel", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Label", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("OutputComments", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Skip", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Category", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category2", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category3", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationStatus", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_Internal_Core : R_SQLite_Core
    {
        public F_SQLiteField UniqueKey { get; set; }
        public F_SQLiteField VariableBank { get; set; }
        public F_SQLiteField VariableAddress { get; set; }
        public F_SQLiteField ShortLabel { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField OutputComments { get; set; }
        public F_SQLiteField Category { get; set; }
        public F_SQLiteField Category2 { get; set; }
        public F_SQLiteField Category3 { get; set; }
        public F_SQLiteField IdentificationStatus { get; set; }
        public F_SQLiteField IdentificationDetails { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("UniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("VariableBank", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("VariableAddress", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("ShortLabel", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Label", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("OutputComments", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Category", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category2", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category3", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationStatus", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_InputArgs : R_SQLite_Core
    {
        public F_SQLiteField Position { get; set; }
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Encryption { get; set; }
        public F_SQLiteField Pointer { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Position", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Byte", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Encryption", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Pointer", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_BitFlags : R_SQLite_Core
    {
        public F_SQLiteField Position { get; set; }
        public F_SQLiteField Skip { get; set; }
        public F_SQLiteField ShortLabel { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField SetValue { get; set; }
        public F_SQLiteField NotSetValue { get; set; }
        public F_SQLiteField HideParent { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField Category { get; set; }
        public F_SQLiteField Category2 { get; set; }
        public F_SQLiteField Category3 { get; set; }
        public F_SQLiteField IdentificationStatus { get; set; }
        public F_SQLiteField IdentificationDetails { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Position", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Skip", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("ShortLabel", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Label", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SetValue", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("NotSetValue", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("HideParent", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("Category", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category2", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category3", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationStatus", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_InputScalars : R_SQLite_Core
    {
        public F_SQLiteField UniqueKey { get; set; }
        public F_SQLiteField VariableAddress { get; set; }
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Signed { get; set; }
        public F_SQLiteField ForcedScaleExpression { get; set; }
        public F_SQLiteField ForcedScalePrecision { get; set; }
        public F_SQLiteField ForcedUnits { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("UniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("VariableAddress", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Byte", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Signed", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("ForcedUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ForcedScaleExpression", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ForcedScalePrecision", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_InputFunctions : R_SQLite_Core
    {
        public F_SQLiteField UniqueKey { get; set; }
        public F_SQLiteField VariableAddress { get; set; }
        public F_SQLiteField VariableInput { get; set; }
        public F_SQLiteField VariableOutput { get; set; }

        public F_SQLiteField ByteInput { get; set; }
        public F_SQLiteField SignedInput { get; set; }
        public F_SQLiteField SignedOutput { get; set; }

        public F_SQLiteField ForcedRowsNumber { get; set; }

        public F_SQLiteField ForcedInputScaleExpression { get; set; }
        public F_SQLiteField ForcedInputScalePrecision { get; set; }
        public F_SQLiteField ForcedInputUnits { get; set; }
        public F_SQLiteField ForcedOutputScaleExpression { get; set; }
        public F_SQLiteField ForcedOutputScalePrecision { get; set; }
        public F_SQLiteField ForcedOutputUnits { get; set; }

        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("UniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("VariableAddress", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("VariableInput", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("VariableOutput", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("ByteInput", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("SignedInput", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("SignedOutput", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("ForcedRowsNumber", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("ForcedInputScaleExpression", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ForcedInputScalePrecision", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("ForcedInputUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ForcedOutputScaleExpression", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ForcedOutputScalePrecision", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("ForcedOutputUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_InputTables : R_SQLite_Core
    {
        public F_SQLiteField UniqueKey { get; set; }
        public F_SQLiteField VariableAddress { get; set; }
        public F_SQLiteField VariableColsNumberReg { get; set; }
        public F_SQLiteField VariableColsReg { get; set; }
        public F_SQLiteField VariableOutput { get; set; }
        public F_SQLiteField VariableRowsReg { get; set; }

        public F_SQLiteField ByteOutput { get; set; }
        public F_SQLiteField SignedOutput { get; set; }

        public F_SQLiteField ForcedColsNumber { get; set; }
        public F_SQLiteField ForcedRowsNumber { get; set; }

        public F_SQLiteField ForcedColsUnits { get; set; }
        public F_SQLiteField ForcedRowsUnits { get; set; }
        public F_SQLiteField ForcedCellsUnits { get; set; }
        public F_SQLiteField ForcedCellsScaleExpression { get; set; }
        public F_SQLiteField ForcedCellsScalePrecision { get; set; }

        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("UniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("VariableAddress", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("VariableColsNumberReg", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("VariableColsReg", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("VariableOutput", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("VariableRowsReg", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("ByteInput", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("SignedOutput", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("ForcedColsNumber", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("ForcedRowsNumber", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("ForcedColsUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ForcedRowsUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ForcedCellsUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ForcedCellsScaleExpression", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ForcedCellsScalePrecision", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_InputStructures : R_SQLite_Core
    {
        public F_SQLiteField UniqueKey { get; set; }
        public F_SQLiteField VariableAddress { get; set; }
        public F_SQLiteField VariableNumber { get; set; }
        public F_SQLiteField ForcedNumber { get; set; }
        public F_SQLiteField StructDef { get; set; }

        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("UniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("VariableAddress", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("VariableNumber", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("ForcedNumber", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("StructDef", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_InternalScalars : R_SAD806x_Internal_Core
    {
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Signed { get; set; }
        public F_SQLiteField Units { get; set; }
        public F_SQLiteField ScaleExpression { get; set; }
        public F_SQLiteField ScalePrecision { get; set; }
        public F_SQLiteField InlineComments { get; set; }
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
            dResult.Add("InlineComments", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Min", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Max", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_InternalScalarsBitFlags : R_SAD806x_Def_BitFlags
    {
        public F_SQLiteField InternalScalarUniqueKey { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("InternalScalarUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_InternalFunctions : R_SAD806x_Internal_Core
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

    public class R_SAD806x_Def_InternalTables : R_SAD806x_Internal_Core
    {
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Signed { get; set; }
        public F_SQLiteField VariableColsNumber { get; set; }
        public F_SQLiteField Rows { get; set; }
        public F_SQLiteField ColumnsUnits { get; set; }
        public F_SQLiteField RowsUnits { get; set; }
        public F_SQLiteField CellsUnits { get; set; }
        public F_SQLiteField CellsScaleExpression { get; set; }
        public F_SQLiteField CellsScalePrecision { get; set; }
        public F_SQLiteField CellsMin { get; set; }
        public F_SQLiteField CellsMax { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Byte", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Signed", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("VariableColsNumber", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Rows", new F_SQLiteFieldProperties("INT (3)"));
            dResult.Add("ColumnsUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("RowsUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("CellsUnits", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("CellsScaleExpression", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("CellsScalePrecision", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("CellsMin", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("CellsMax", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_InternalStructures : R_SAD806x_Internal_Core
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

    public class R_SAD806x_Def_Functions : R_SAD806x_Elements_Core
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

    public class R_SAD806x_Def_Operations : R_SAD806x_Elements_Core
    {
        public F_SQLiteField InlineComments { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("InlineComments", new F_SQLiteFieldProperties("BOOLEAN"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_Others : R_SAD806x_Elements_Core
    {
        public F_SQLiteField OutputLabel { get; set; }
        public F_SQLiteField InlineComments { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("OutputLabel", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("InlineComments", new F_SQLiteFieldProperties("BOOLEAN"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_Registers : R_SQLite_Core
    {
        public F_SQLiteField Address { get; set; }
        public F_SQLiteField AddressAdder { get; set; }
        public F_SQLiteField UniqueAddCode { get; set; }
        public F_SQLiteField Skip { get; set; }

        public F_SQLiteField isRBase { get; set; }
        public F_SQLiteField isRConst { get; set; }
        public F_SQLiteField ConstValue { get; set; }
        public F_SQLiteField Label { get; set; }
        public F_SQLiteField ByteLabel { get; set; }
        public F_SQLiteField WordLabel { get; set; }
        public F_SQLiteField Comments { get; set; }
        public F_SQLiteField ScaleExpression { get; set; }
        public F_SQLiteField ScalePrecision { get; set; }
        public F_SQLiteField Units { get; set; }
        public F_SQLiteField SizeStatus { get; set; }
        public F_SQLiteField SignedStatus { get; set; }

        public F_SQLiteField Category { get; set; }
        public F_SQLiteField Category2 { get; set; }
        public F_SQLiteField Category3 { get; set; }
        public F_SQLiteField IdentificationStatus { get; set; }
        public F_SQLiteField IdentificationDetails { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Address", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("AddressAdder", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            dResult.Add("UniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            dResult.Add("Skip", new F_SQLiteFieldProperties("BOOLEAN"));

            dResult.Add("isRBase", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("isRConst", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("ConstValue", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Label", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ByteLabel", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("WordLabel", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Comments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("ScaleExpression", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("ScalePrecision", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("Units", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SizeStatus", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("SignedStatus", new F_SQLiteFieldProperties("NVARCHAR (100)"));

            dResult.Add("Category", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category2", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("Category3", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationStatus", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_RegistersBitFlags : R_SAD806x_Def_BitFlags
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

    public class R_SAD806x_Def_Routines : R_SAD806x_Elements_Core {}

    public class R_SAD806x_Def_RoutinesInputArgs : R_SAD806x_Def_InputArgs
    {
        public F_SQLiteField RoutineBank { get; set; }
        public F_SQLiteField RoutineAddress { get; set; }
        public F_SQLiteField RoutineUniqueAddCode { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("RoutineBank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("RoutineAddress", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("RoutineUniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_RoutinesInputScalars : R_SAD806x_Def_InputScalars
    {
        public F_SQLiteField RoutineBank { get; set; }
        public F_SQLiteField RoutineAddress { get; set; }
        public F_SQLiteField RoutineUniqueAddCode { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("RoutineBank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("RoutineAddress", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("RoutineUniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_RoutinesInputFunctions : R_SAD806x_Def_InputFunctions
    {
        public F_SQLiteField RoutineBank { get; set; }
        public F_SQLiteField RoutineAddress { get; set; }
        public F_SQLiteField RoutineUniqueAddCode { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("RoutineBank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("RoutineAddress", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("RoutineUniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_RoutinesInputTables : R_SAD806x_Def_InputTables
    {
        public F_SQLiteField RoutineBank { get; set; }
        public F_SQLiteField RoutineAddress { get; set; }
        public F_SQLiteField RoutineUniqueAddCode { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("RoutineBank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("RoutineAddress", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("RoutineUniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_RoutinesInputStructures : R_SAD806x_Def_InputStructures
    {
        public F_SQLiteField RoutineBank { get; set; }
        public F_SQLiteField RoutineAddress { get; set; }
        public F_SQLiteField RoutineUniqueAddCode { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("RoutineBank", new F_SQLiteFieldProperties("INT (2)"));
            dResult.Add("RoutineAddress", new F_SQLiteFieldProperties("INT (5)"));
            dResult.Add("RoutineUniqueAddCode", new F_SQLiteFieldProperties("NVARCHAR (4)"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_Scalars : R_SAD806x_Elements_Core
    {
        public F_SQLiteField Byte { get; set; }
        public F_SQLiteField Signed { get; set; }
        public F_SQLiteField Units { get; set; }
        public F_SQLiteField ScaleExpression { get; set; }
        public F_SQLiteField ScalePrecision { get; set; }
        public F_SQLiteField InlineComments { get; set; }
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
            dResult.Add("InlineComments", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Min", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("Max", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            return dResult;
        }
    }

    public class R_SAD806x_Def_ScalarsBitFlags : R_SAD806x_Def_BitFlags
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

    public class R_SAD806x_Def_Structures : R_SAD806x_Elements_Core
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

    public class R_SAD806x_Def_Tables : R_SAD806x_Elements_Core
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

    public class R_SAD806x_SignaturesElements : R_SQLite_Core
    {
        public F_SQLiteField UniqueKey { get; set; }
        public F_SQLiteField Skip { get; set; }
        public F_SQLiteField Forced { get; set; }
        public F_SQLiteField Signature { get; set; }
        
        public F_SQLiteField SignatureLabel { get; set; }
        public F_SQLiteField SignatureCategory { get; set; }
        public F_SQLiteField SignatureCategory2 { get; set; }
        public F_SQLiteField SignatureCategory3 { get; set; }
        public F_SQLiteField SignatureComments { get; set; }

        public F_SQLiteField for806x { get; set; }
        public F_SQLiteField forBankNum { get; set; }

        public F_SQLiteField SignatureOpeIncludingElemAddress { get; set; }

        public F_SQLiteField IdentificationStatus { get; set; }
        public F_SQLiteField IdentificationDetails { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("UniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("Skip", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Forced", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Signature", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("SignatureLabel", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SignatureCategory", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SignatureCategory2", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SignatureCategory3", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SignatureComments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("for806x", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("forBankNum", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("SignatureOpeIncludingElemAddress", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("IdentificationStatus", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesElementsInternalScalars : R_SAD806x_Def_InternalScalars
    {
        public F_SQLiteField SignatureElementUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureElementUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesElementsInternalScalarsBitFlags : R_SAD806x_Def_InternalScalarsBitFlags
    {
        public F_SQLiteField SignatureElementUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureElementUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesElementsInternalFunctions : R_SAD806x_Def_InternalFunctions
    {
        public F_SQLiteField SignatureElementUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureElementUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesElementsInternalTables : R_SAD806x_Def_InternalTables
    {
        public F_SQLiteField SignatureElementUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureElementUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesElementsInternalStructures : R_SAD806x_Def_InternalStructures
    {
        public F_SQLiteField SignatureElementUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureElementUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutines : R_SQLite_Core
    {
        public F_SQLiteField UniqueKey { get; set; }
        public F_SQLiteField Skip { get; set; }
        public F_SQLiteField Forced { get; set; }
        public F_SQLiteField Signature { get; set; }
        
        public F_SQLiteField RoutineShortLabel { get; set; }
        public F_SQLiteField RoutineOutputComments { get; set; }
        public F_SQLiteField RoutineLabel { get; set; }
        public F_SQLiteField RoutineComments { get; set; }
        public F_SQLiteField RoutineCategory { get; set; }
        public F_SQLiteField RoutineCategory2 { get; set; }
        public F_SQLiteField RoutineCategory3 { get; set; }
        public F_SQLiteField RoutineIdentificationStatus { get; set; }
        public F_SQLiteField RoutineIdentificationDetails { get; set; }
        public F_SQLiteField RoutineDateCreated { get; set; }
        public F_SQLiteField RoutineDateUpdated { get; set; }

        public F_SQLiteField SignatureLabel { get; set; }
        public F_SQLiteField SignatureCategory { get; set; }
        public F_SQLiteField SignatureCategory2 { get; set; }
        public F_SQLiteField SignatureCategory3 { get; set; }
        public F_SQLiteField SignatureComments { get; set; }

        public F_SQLiteField for806x { get; set; }
        public F_SQLiteField forBankNum { get; set; }

        public F_SQLiteField IdentificationStatus { get; set; }
        public F_SQLiteField IdentificationDetails { get; set; }
        public F_SQLiteField DateCreated { get; set; }
        public F_SQLiteField DateUpdated { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("UniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("Skip", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Forced", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("Signature", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("RoutineShortLabel", new F_SQLiteFieldProperties("NVARCHAR (100)"));
            dResult.Add("RoutineOutputComments", new F_SQLiteFieldProperties("BOOLEAN"));
            dResult.Add("RoutineLabel", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("RoutineComments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("RoutineCategory", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("RoutineCategory2", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("RoutineCategory3", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("RoutineIdentificationStatus", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("RoutineIdentificationDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("RoutineDateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("RoutineDateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("SignatureLabel", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SignatureCategory", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SignatureCategory2", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SignatureCategory3", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("SignatureComments", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("for806x", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("forBankNum", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            dResult.Add("IdentificationStatus", new F_SQLiteFieldProperties("NVARCHAR (500)"));
            dResult.Add("IdentificationDetails", new F_SQLiteFieldProperties("TEXT"));
            dResult.Add("DateCreated", new F_SQLiteFieldProperties("DATETIME"));
            dResult.Add("DateUpdated", new F_SQLiteFieldProperties("DATETIME"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutinesInputArgs : R_SAD806x_Def_InputArgs
    {
        public F_SQLiteField SignatureRoutineUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureRoutineUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutinesInputScalars : R_SAD806x_Def_InputScalars
    {
        public F_SQLiteField SignatureRoutineUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureRoutineUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutinesInputFunctions : R_SAD806x_Def_InputFunctions
    {
        public F_SQLiteField SignatureRoutineUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureRoutineUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutinesInputTables : R_SAD806x_Def_InputTables
    {
        public F_SQLiteField SignatureRoutineUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureRoutineUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutinesInputStructures : R_SAD806x_Def_InputStructures
    {
        public F_SQLiteField SignatureRoutineUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureRoutineUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutinesInternalScalars : R_SAD806x_Def_InternalScalars
    {
        public F_SQLiteField SignatureRoutineUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureRoutineUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags : R_SAD806x_Def_InternalScalarsBitFlags
    {
        public F_SQLiteField SignatureRoutineUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureRoutineUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutinesInternalFunctions : R_SAD806x_Def_InternalFunctions
    {
        public F_SQLiteField SignatureRoutineUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureRoutineUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutinesInternalTables : R_SAD806x_Def_InternalTables
    {
        public F_SQLiteField SignatureRoutineUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureRoutineUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }

    public class R_SAD806x_SignaturesRoutinesInternalStructures : R_SAD806x_Def_InternalStructures
    {
        public F_SQLiteField SignatureRoutineUniqueKey { get; set; }
        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("SignatureRoutineUniqueKey", new F_SQLiteFieldProperties("NVARCHAR (20)"));
            return dResult;
        }
    }
}
