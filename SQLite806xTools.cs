using System;
using System.Collections.Generic;
using System.Text;
using SQLite806x;

namespace SAD806x
{
    public static class SQLite806xTools
    {
        public static void addTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Tables> rList, S6xTable s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rList == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Skip || !s6xObject.Store) return;

            R_806x_Def_Tables rRow = db806x.newRow<R_806x_Def_Tables>();
            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = s6xObject.DuplicateNum;
            rRow.Byte.Value = !s6xObject.WordOutput;
            rRow.CellsScaleExpression.Value = s6xObject.CellsScaleExpression;
            rRow.CellsScalePrecision.Value = s6xObject.CellsScalePrecision;
            rRow.CellsUnits.Value = s6xObject.CellsUnits;
            rRow.Columns.Value = s6xObject.ColsNumber;
            rRow.ColumnsUnits.Value = s6xObject.ColsUnits;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.Label.Value = s6xObject.Label;
            rRow.Rows.Value = s6xObject.RowsNumber;
            rRow.RowsUnits.Value = s6xObject.RowsUnits;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.Signed.Value = s6xObject.SignedOutput;

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

            rList.Add(rRow);
        }

        public static void addFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Functions> rList, S6xFunction s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rList == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Skip || !s6xObject.Store) return;

            R_806x_Def_Functions rRow = db806x.newRow<R_806x_Def_Functions>();
            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = s6xObject.DuplicateNum;
            rRow.Byte.Value = s6xObject.ByteInput;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.InputScaleExpression.Value = s6xObject.InputScaleExpression;
            rRow.InputScalePrecision.Value = s6xObject.InputScalePrecision;
            rRow.InputSigned.Value = s6xObject.SignedInput;
            rRow.InputUnits.Value = s6xObject.InputUnits;
            rRow.Label.Value = s6xObject.Label;
            rRow.OutputScaleExpression.Value = s6xObject.OutputScaleExpression;
            rRow.OutputScalePrecision.Value = s6xObject.OutputScalePrecision;
            rRow.OutputSigned.Value = s6xObject.SignedOutput;
            rRow.OutputUnits.Value = s6xObject.OutputUnits;
            rRow.Rows.Value = s6xObject.RowsNumber;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;

            rList.Add(rRow);
        }

        public static void addScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Scalars> rList, S6xScalar s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rList == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Skip || !s6xObject.Store) return;

            R_806x_Def_Scalars rRow = db806x.newRow<R_806x_Def_Scalars>();
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

            rList.Add(rRow);
        }

        public static void addScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_ScalarsBitFlags> rList, S6xScalar s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rList == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Skip || !s6xObject.Store) return;
            if (!s6xObject.isBitFlags) return;
            if (s6xObject.BitFlags == null) return;

            foreach (S6xBitFlag s6xBF in s6xObject.BitFlags)
            {
                if (s6xBF.Skip) continue;

                R_806x_Def_ScalarsBitFlags rRow = db806x.newRow<R_806x_Def_ScalarsBitFlags>();
                rRow.ScalarBank.Value = s6xObject.BankNum;
                rRow.ScalarAddress.Value = s6xObject.AddressInt;
                rRow.ScalarUniqueAddCode.Value = s6xObject.DuplicateNum;
                rRow.Position.Value = s6xBF.Position;
                rRow.UniqueAddCode.Value = 0;
                rRow.Comments.Value = s6xBF.Comments;
                rRow.Label.Value = s6xBF.Comments;
                rRow.ShortLabel.Value = s6xBF.Comments;

                rList.Add(rRow);
            }
        }

        public static void addStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Structures> rList, S6xStructure s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rList == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Skip || !s6xObject.Store) return;

            R_806x_Def_Structures rRow = db806x.newRow<R_806x_Def_Structures>();
            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = s6xObject.DuplicateNum;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.Label.Value = s6xObject.Label;
            rRow.Number.Value = s6xObject.Number;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;
            rRow.StructureDefinition.Value = s6xObject.StructDef;

            rList.Add(rRow);
        }

        public static void addRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Routines> rList, S6xRoutine s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rList == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Skip || !s6xObject.Store) return;

            R_806x_Def_Routines rRow = db806x.newRow<R_806x_Def_Routines>();
            rRow.Bank.Value = s6xObject.BankNum;
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.UniqueAddCode.Value = 0;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.Label.Value = s6xObject.Label;
            rRow.ShortLabel.Value = s6xObject.ShortLabel;

            rList.Add(rRow);
        }

        public static void addRoutineArgsRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_RoutinesArgs> rList, S6xRoutine s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rList == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Skip || !s6xObject.Store) return;
            if (!s6xObject.isAdvanced) return;
            if (s6xObject.InputArguments == null) return;

            foreach (S6xRoutineInputArgument s6xArg in s6xObject.InputArguments)
            {
                R_806x_Def_RoutinesArgs rRow = db806x.newRow<R_806x_Def_RoutinesArgs>();
                rRow.RoutineBank.Value = s6xObject.BankNum;
                rRow.RoutineAddress.Value = s6xObject.AddressInt;
                rRow.RoutineUniqueAddCode.Value = 0;
                rRow.Byte.Value = !s6xArg.Word;
                rRow.Encryption.Value = s6xArg.Encryption;
                rRow.Pointer.Value = s6xArg.Pointer;
                rRow.Position.Value = s6xArg.Position;

                rList.Add(rRow);
            }
        }

        public static void addRegisterRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Registers> rList, S6xRegister s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rList == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Skip || !s6xObject.Store) return;

            R_806x_Def_Registers rRow = db806x.newRow<R_806x_Def_Registers>();
            rRow.Address.Value = s6xObject.AddressInt;
            rRow.AddressAdder.Value = s6xObject.AdditionalAddress10;
            rRow.UniqueAddCode.Value = 0;
            rRow.ByteLabel.Value = s6xObject.ByteLabel;
            rRow.Comments.Value = s6xObject.Comments;
            rRow.ShortLabel.Value = s6xObject.Label;
            rRow.WordLabel.Value = s6xObject.WordLabel;

            rList.Add(rRow);
        }

        public static void addRegisterBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_RegistersBitFlags> rList, S6xRegister s6xObject)
        {
            if (db806x == null) return;
            if (sadS6x == null) return;
            if (rList == null) return;
            if (s6xObject == null) return;
            if (s6xObject.Skip || !s6xObject.Store) return;
            if (!s6xObject.isBitFlags) return;
            if (s6xObject.BitFlags == null) return;
            
            foreach (S6xBitFlag s6xBF in s6xObject.BitFlags)
            {
                if (s6xBF.Skip) continue;

                R_806x_Def_RegistersBitFlags rRow = db806x.newRow<R_806x_Def_RegistersBitFlags>();
                rRow.RegisterAddress.Value = s6xObject.AddressInt;
                rRow.RegisterAddressAdder.Value = s6xObject.AdditionalAddress10;
                rRow.RegisterUniqueAddCode.Value = 0;
                rRow.Position.Value = s6xBF.Position;
                rRow.UniqueAddCode.Value = 0;
                rRow.Comments.Value = s6xBF.Comments;
                rRow.Label.Value = s6xBF.Comments;
                rRow.ShortLabel.Value = s6xBF.Comments;

                rList.Add(rRow);
            }
        }
    }
}
