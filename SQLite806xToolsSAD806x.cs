using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SAD806x;

namespace SQLite806x
{
    public static class SQLite806xToolsSAD806x
    {
        public static void initSchema(ref List<T_SQLiteTable> sqlTables)
        {
            SQLite806xToolsSAD806xV10.initSchema(ref sqlTables);
        }

        public static string getDBVersion()
        {
            return SQLite806xToolsSAD806xV10.getDBVersion();
        }

        public static DateTime getDBVersionDate()
        {
            return SQLite806xToolsSAD806xV10.getDBVersionDate();
        }

        public static string getDBVersionComments()
        {
            return SQLite806xToolsSAD806xV10.getDBVersionComments();
        }

        public static object addDBVersionRow(SQLite806xDB db806x, ref List<R_SAD806x_DB_Information> rList)
        {
            return SQLite806xToolsSAD806xV10.addDBVersionRow(db806x, ref rList);
        }

        public static object addPropertiesRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Properties> rList, S6xProperties s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addPropertiesRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updatePropertiesRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Properties rRow, S6xProperties s6xObject)
        {
            SQLite806xToolsSAD806xV10.updatePropertiesRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static void setPropertiesS6x(ref SADS6x sadS6x, R_SAD806x_Properties rRow)
        {
            SQLite806xToolsSAD806xV10.setPropertiesS6x(ref sadS6x, rRow);
        }

        public static object addSyncStateRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SyncStates> rList, S6xSyncState s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSyncStateRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Tables> rList, S6xTable s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addTableRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Tables rRow, S6xTable s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateTableRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Functions> rList, S6xFunction s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addFunctionRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Functions rRow, S6xFunction s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Scalars> rList, S6xScalar s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addScalarRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Scalars rRow, S6xScalar s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateScalarRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addScalarBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_ScalarsBitFlags> rList, S6xScalar s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addScalarBitFlagRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_ScalarsBitFlags> rList, S6xScalar s6xObject, S6xBitFlag s6xBitFlag)
        {
            return SQLite806xToolsSAD806xV10.addScalarBitFlagRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xBitFlag);
        }

        public static void updateScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_ScalarsBitFlags rRow, S6xScalar s6xObject, S6xBitFlag s6xBitFlag)
        {
            SQLite806xToolsSAD806xV10.updateScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xBitFlag);
        }

        public static object addStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Structures> rList, S6xStructure s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addStructureRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Structures rRow, S6xStructure s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateStructureRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Routines> rList, S6xRoutine s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Routines rRow, S6xRoutine s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateRoutineRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addRoutineInputArgRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputArgs> rList, S6xRoutine s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineInputArgRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputArgs> rList, S6xRoutine s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineInputArgRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputArgs rRow, S6xRoutine s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateRoutineInputArgRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }
        
        public static object addRoutineInputScalarRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputScalars> rList, S6xRoutine s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineInputScalarRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addRoutineInputScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputScalars> rList, S6xRoutine s6xObject, S6xRoutineInputScalar s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineInputScalarRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateRoutineInputScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputScalars rRow, S6xRoutine s6xObject, S6xRoutineInputScalar s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateRoutineInputScalarRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addRoutineInputFunctionRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputFunctions> rList, S6xRoutine s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineInputFunctionRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addRoutineInputFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputFunctions> rList, S6xRoutine s6xObject, S6xRoutineInputFunction s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineInputFunctionRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateRoutineInputFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputFunctions rRow, S6xRoutine s6xObject, S6xRoutineInputFunction s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateRoutineInputFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addRoutineInputTableRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputTables> rList, S6xRoutine s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineInputTableRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addRoutineInputTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputTables> rList, S6xRoutine s6xObject, S6xRoutineInputTable s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineInputTableRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateRoutineInputTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputTables rRow, S6xRoutine s6xObject, S6xRoutineInputTable s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateRoutineInputTableRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addRoutineInputStructureRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputStructures> rList, S6xRoutine s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineInputStructureRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addRoutineInputStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RoutinesInputStructures> rList, S6xRoutine s6xObject, S6xRoutineInputStructure s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addRoutineInputStructureRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateRoutineInputStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputStructures rRow, S6xRoutine s6xObject, S6xRoutineInputStructure s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateRoutineInputStructureRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addOperationRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Operations> rList, S6xOperation s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addOperationRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateOperationRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Operations rRow, S6xOperation s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateOperationRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addOtherRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Others> rList, S6xOtherAddress s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addOtherRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateOtherRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Others rRow, S6xOtherAddress s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateOtherRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addRegisterRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_Registers> rList, S6xRegister s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addRegisterRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateRegisterRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_Registers rRow, S6xRegister s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateRegisterRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addRegisterBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RegistersBitFlags> rList, S6xRegister s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addRegisterBitFlagRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addRegisterBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_Def_RegistersBitFlags> rList, S6xRegister s6xObject, S6xBitFlag s6xBitFlag)
        {
            return SQLite806xToolsSAD806xV10.addRegisterBitFlagRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xBitFlag);
        }

        public static void updateRegisterBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_Def_RegistersBitFlags rRow, S6xRegister s6xObject, S6xBitFlag s6xBitFlag)
        {
            SQLite806xToolsSAD806xV10.updateRegisterBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xBitFlag);
        }

        public static object addSignatureRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutines> rList, S6xSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateSignatureRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutines rRow, S6xSignature s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addSignatureRoutineInputArgRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputArgs> rList, S6xSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInputArgRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addSignatureRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputArgs> rList, S6xSignature s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInputArgRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateSignatureRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputArgs rRow, S6xSignature s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineInputArgRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addSignatureRoutineInputScalarRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputScalars> rList, S6xSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInputScalarRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addSignatureRoutineInputScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputScalars> rList, S6xSignature s6xObject, S6xRoutineInputScalar s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInputScalarRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateSignatureRoutineInputScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputScalars rRow, S6xSignature s6xObject, S6xRoutineInputScalar s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineInputScalarRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addSignatureRoutineInputFunctionRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputFunctions> rList, S6xSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInputFunctionRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addSignatureRoutineInputFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputFunctions> rList, S6xSignature s6xObject, S6xRoutineInputFunction s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInputFunctionRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateSignatureRoutineInputFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputFunctions rRow, S6xSignature s6xObject, S6xRoutineInputFunction s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineInputFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addSignatureRoutineInputTableRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputTables> rList, S6xSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInputTableRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addSignatureRoutineInputTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputTables> rList, S6xSignature s6xObject, S6xRoutineInputTable s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInputTableRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateSignatureRoutineInputTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputTables rRow, S6xSignature s6xObject, S6xRoutineInputTable s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineInputTableRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addSignatureRoutineInputStructureRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputStructures> rList, S6xSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInputStructureRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addSignatureRoutineInputStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInputStructures> rList, S6xSignature s6xObject, S6xRoutineInputStructure s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInputStructureRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateSignatureRoutineInputStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputStructures rRow, S6xSignature s6xObject, S6xRoutineInputStructure s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineInputStructureRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addSignatureRoutineInternalScalarRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalScalars> rList, S6xSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInternalScalarRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addSignatureRoutineInternalScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalScalars> rList, S6xSignature s6xObject, S6xRoutineInternalScalar s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInternalScalarRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateSignatureRoutineInternalScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalScalars rRow, S6xSignature s6xObject, S6xRoutineInternalScalar s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineInternalScalarRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addSignatureRoutineInternalScalarBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags> rList, S6xSignature s6xObject, S6xRoutineInternalScalar s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInternalScalarBitFlagRows(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static object addSignatureRoutineInternalScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags> rList, S6xSignature s6xObject, S6xRoutineInternalScalar s6xSubObject, S6xBitFlag s6xBitFlag)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInternalScalarBitFlagRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject, s6xBitFlag);
        }

        public static void updateSignatureRoutineInternalScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rRow, S6xSignature s6xObject, S6xRoutineInternalScalar s6xSubObject, S6xBitFlag s6xBitFlag)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineInternalScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject, s6xBitFlag);
        }

        public static object addSignatureRoutineInternalFunctionRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalFunctions> rList, S6xSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInternalFunctionRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addSignatureRoutineInternalFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalFunctions> rList, S6xSignature s6xObject, S6xRoutineInternalFunction s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInternalFunctionRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateSignatureRoutineInternalFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalFunctions rRow, S6xSignature s6xObject, S6xRoutineInternalFunction s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineInternalFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addSignatureRoutineInternalTableRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalTables> rList, S6xSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInternalTableRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addSignatureRoutineInternalTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalTables> rList, S6xSignature s6xObject, S6xRoutineInternalTable s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInternalTableRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateSignatureRoutineInternalTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalTables rRow, S6xSignature s6xObject, S6xRoutineInternalTable s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineInternalTableRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addSignatureRoutineInternalStructureRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalStructures> rList, S6xSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInternalStructureRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addSignatureRoutineInternalStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesRoutinesInternalStructures> rList, S6xSignature s6xObject, S6xRoutineInternalStructure s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureRoutineInternalStructureRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateSignatureRoutineInternalStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalStructures rRow, S6xSignature s6xObject, S6xRoutineInternalStructure s6xSubObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureRoutineInternalStructureRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addSignatureElementRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElements> rList, S6xElementSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureElementRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateSignatureElementRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElements rRow, S6xElementSignature s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureElementRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addSignatureElementInternalScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalScalars> rList, S6xElementSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureElementInternalScalarRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateSignatureElementInternalScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalScalars rRow, S6xElementSignature s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureElementInternalScalarRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addSignatureElementInternalScalarBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags> rList, S6xElementSignature s6xObject, S6xRoutineInternalScalar s6xSubObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureElementInternalScalarBitFlagRows(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static object addSignatureElementInternalScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalScalarsBitFlags> rList, S6xElementSignature s6xObject, S6xRoutineInternalScalar s6xSubObject, S6xBitFlag s6xBF)
        {
            return SQLite806xToolsSAD806xV10.addSignatureElementInternalScalarBitFlagRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject, s6xBF);
        }

        public static void updateSignatureElementInternalScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalScalarsBitFlags rRow, S6xElementSignature s6xObject, S6xRoutineInternalScalar s6xSubObject, S6xBitFlag s6xBF)
        {
            SQLite806xToolsSAD806xV10.updateSignatureElementInternalScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject, s6xBF);
        }

        public static object addSignatureElementInternalFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalFunctions> rList, S6xElementSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureElementInternalFunctionRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateSignatureElementInternalFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalFunctions rRow, S6xElementSignature s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureElementInternalFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addSignatureElementInternalTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalTables> rList, S6xElementSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureElementInternalTableRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateSignatureElementInternalTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalTables rRow, S6xElementSignature s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureElementInternalTableRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addSignatureElementInternalStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_SAD806x_SignaturesElementsInternalStructures> rList, S6xElementSignature s6xObject)
        {
            return SQLite806xToolsSAD806xV10.addSignatureElementInternalStructureRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateSignatureElementInternalStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalStructures rRow, S6xElementSignature s6xObject)
        {
            SQLite806xToolsSAD806xV10.updateSignatureElementInternalStructureRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static bool deleteTableRow(ref SQLite806xDB db806x, R_SAD806x_Def_Tables rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteTableRow(ref db806x, rRow, recursively);
        }

        public static bool deleteFunctionRow(ref SQLite806xDB db806x, R_SAD806x_Def_Functions rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteFunctionRow(ref db806x, rRow, recursively);
        }

        public static bool deleteScalarRow(ref SQLite806xDB db806x, R_SAD806x_Def_Scalars rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteScalarRow(ref db806x, rRow, recursively);
        }

        public static bool deleteScalarBitFlagRow(ref SQLite806xDB db806x, R_SAD806x_Def_ScalarsBitFlags rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteScalarBitFlagRow(ref db806x, rRow, recursively);
        }

        public static bool deleteStructureRow(ref SQLite806xDB db806x, R_SAD806x_Def_Structures rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteStructureRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRoutineRow(ref SQLite806xDB db806x, R_SAD806x_Def_Routines rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteRoutineRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRoutineInputArgRow(ref SQLite806xDB db806x, R_SAD806x_Def_RoutinesInputArgs rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteRoutineInputArgRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRoutineInputScalarRow(ref SQLite806xDB db806x, R_SAD806x_Def_RoutinesInputScalars rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteRoutineInputScalarRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRoutineInputFunctionRow(ref SQLite806xDB db806x, R_SAD806x_Def_RoutinesInputFunctions rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteRoutineInputFunctionRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRoutineInputTableRow(ref SQLite806xDB db806x, R_SAD806x_Def_RoutinesInputTables rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteRoutineInputTableRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRoutineInputStructureRow(ref SQLite806xDB db806x, R_SAD806x_Def_RoutinesInputStructures rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteRoutineInputStructureRow(ref db806x, rRow, recursively);
        }

        public static bool deleteOperationRow(ref SQLite806xDB db806x, R_SAD806x_Def_Operations rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteOperationRow(ref db806x, rRow, recursively);
        }

        public static bool deleteOtherRow(ref SQLite806xDB db806x, R_SAD806x_Def_Others rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteOtherRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRegisterRow(ref SQLite806xDB db806x, R_SAD806x_Def_Registers rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteRegisterRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRegisterBitFlagRow(ref SQLite806xDB db806x, R_SAD806x_Def_RegistersBitFlags rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteRegisterBitFlagRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutines rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineInputArgRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInputArgs rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineInputArgRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineInputScalarRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInputScalars rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineInputScalarRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineInputFunctionRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInputFunctions rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineInputFunctionRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineInputTableRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInputTables rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineInputTableRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineInputStructureRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInputStructures rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineInputStructureRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineInternalScalarRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalScalars rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineInternalScalarRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineInternalScalarBitFlagRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineInternalScalarBitFlagRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineInternalFunctionRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalFunctions rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineInternalFunctionRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineInternalTableRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalTables rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineInternalTableRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureRoutineInternalStructureRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalStructures rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureRoutineInternalStructureRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureElementRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElements rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureElementRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureElementInternalScalarRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalScalars rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureElementInternalScalarRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureElementInternalScalarBitFlagRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalScalarsBitFlags rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureElementInternalScalarBitFlagRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureElementInternalFunctionRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalFunctions rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureElementInternalFunctionRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureElementInternalTableRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalTables rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureElementInternalTableRow(ref db806x, rRow, recursively);
        }

        public static bool deleteSignatureElementInternalStructureRow(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalStructures rRow, bool recursively)
        {
            return SQLite806xToolsSAD806xV10.deleteSignatureElementInternalStructureRow(ref db806x, rRow, recursively);
        }

        public static object[] setTableS6x(ref SADS6x sadS6x, R_SAD806x_Def_Tables rRow)
        {
            return SQLite806xToolsSAD806xV10.setTableS6x(ref sadS6x, rRow);
        }

        public static object[] setFunctionS6x(ref SADS6x sadS6x, R_SAD806x_Def_Functions rRow)
        {
            return SQLite806xToolsSAD806xV10.setFunctionS6x(ref sadS6x, rRow);
        }

        public static object[] setScalarS6x(ref SADS6x sadS6x, R_SAD806x_Def_Scalars rRow)
        {
            return SQLite806xToolsSAD806xV10.setScalarS6x(ref sadS6x, rRow);
        }

        public static object[] setScalarBitFlagS6x(ref SADS6x sadS6x, R_SAD806x_Def_ScalarsBitFlags rRow)
        {
            return SQLite806xToolsSAD806xV10.setScalarBitFlagS6x(ref sadS6x, rRow);
        }

        public static object[] setStructureS6x(ref SADS6x sadS6x, R_SAD806x_Def_Structures rRow)
        {
            return SQLite806xToolsSAD806xV10.setStructureS6x(ref sadS6x, rRow);
        }

        public static object[] setRoutineS6x(ref SADS6x sadS6x, R_SAD806x_Def_Routines rRow)
        {
            return SQLite806xToolsSAD806xV10.setRoutineS6x(ref sadS6x, rRow);
        }

        public static object[] setRoutineInputArgS6x(ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputArgs rRow)
        {
            return SQLite806xToolsSAD806xV10.setRoutineInputArgS6x(ref sadS6x, rRow);
        }

        public static object[] setRoutineInputScalarS6x(ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputScalars rRow)
        {
            return SQLite806xToolsSAD806xV10.setRoutineInputScalarS6x(ref sadS6x, rRow);
        }

        public static object[] setRoutineInputFunctionS6x(ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputFunctions rRow)
        {
            return SQLite806xToolsSAD806xV10.setRoutineInputFunctionS6x(ref sadS6x, rRow);
        }

        public static object[] setRoutineInputTableS6x(ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputTables rRow)
        {
            return SQLite806xToolsSAD806xV10.setRoutineInputTableS6x(ref sadS6x, rRow);
        }

        public static object[] setRoutineInputStructureS6x(ref SADS6x sadS6x, R_SAD806x_Def_RoutinesInputStructures rRow)
        {
            return SQLite806xToolsSAD806xV10.setRoutineInputStructureS6x(ref sadS6x, rRow);
        }

        public static object[] setOperationS6x(ref SADS6x sadS6x, R_SAD806x_Def_Operations rRow)
        {
            return SQLite806xToolsSAD806xV10.setOperationS6x(ref sadS6x, rRow);
        }

        public static object[] setOtherS6x(ref SADS6x sadS6x, R_SAD806x_Def_Others rRow)
        {
            return SQLite806xToolsSAD806xV10.setOtherS6x(ref sadS6x, rRow);
        }

        public static object[] setRegisterS6x(ref SADS6x sadS6x, R_SAD806x_Def_Registers rRow)
        {
            return SQLite806xToolsSAD806xV10.setRegisterS6x(ref sadS6x, rRow);
        }

        public static object[] setRegisterBitFlagS6x(ref SADS6x sadS6x, R_SAD806x_Def_RegistersBitFlags rRow)
        {
            return SQLite806xToolsSAD806xV10.setRegisterBitFlagS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutines rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineInputArgS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputArgs rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineInputArgS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineInputScalarS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputScalars rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineInputScalarS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineInputFunctionS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputFunctions rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineInputFunctionS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineInputTableS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputTables rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineInputTableS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineInputStructureS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInputStructures rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineInputStructureS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineInternalScalarS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalScalars rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineInternalScalarS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineInternalScalarBitFlagS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineInternalScalarBitFlagS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineInternalFunctionS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalFunctions rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineInternalFunctionS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineInternalTableS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalTables rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineInternalTableS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureRoutineInternalStructureS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesRoutinesInternalStructures rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureRoutineInternalStructureS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureElementS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElements rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureElementS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureElementInternalScalarS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalScalars rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureElementInternalScalarS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureElementInternalScalarBitFlagS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalScalarsBitFlags rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureElementInternalScalarBitFlagS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureElementInternalFunctionS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalFunctions rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureElementInternalFunctionS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureElementInternalTableS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalTables rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureElementInternalTableS6x(ref sadS6x, rRow);
        }

        public static object[] setSignatureElementInternalStructureS6x(ref SADS6x sadS6x, R_SAD806x_SignaturesElementsInternalStructures rRow)
        {
            return SQLite806xToolsSAD806xV10.setSignatureElementInternalStructureS6x(ref sadS6x, rRow);
        }
        
        public static List<object> getScalarSubRows(ref SQLite806xDB db806x, R_SAD806x_Def_Scalars rRow)
        {
            return SQLite806xToolsSAD806xV10.getScalarSubRows(ref db806x, rRow);
        }

        public static List<object> getRoutineSubRows(ref SQLite806xDB db806x, R_SAD806x_Def_Routines rRow)
        {
            return SQLite806xToolsSAD806xV10.getRoutineSubRows(ref db806x, rRow);
        }

        public static List<object> getRegisterSubRows(ref SQLite806xDB db806x, R_SAD806x_Def_Registers rRow)
        {
            return SQLite806xToolsSAD806xV10.getRegisterSubRows(ref db806x, rRow);
        }

        public static List<object> getSignatureRoutineSubRows(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutines rRow)
        {
            return SQLite806xToolsSAD806xV10.getSignatureRoutineSubRows(ref db806x, rRow);
        }

        public static List<object> getSignatureRoutineInternalScalarSubRows(ref SQLite806xDB db806x, R_SAD806x_SignaturesRoutinesInternalScalars rRow)
        {
            return SQLite806xToolsSAD806xV10.getSignatureRoutineInternalScalarSubRows(ref db806x, rRow);
        }

        public static List<object> getSignatureElementSubRows(ref SQLite806xDB db806x, R_SAD806x_SignaturesElements rRow)
        {
            return SQLite806xToolsSAD806xV10.getSignatureElementSubRows(ref db806x, rRow);
        }

        public static List<object> getSignatureElementInternalScalarSubRows(ref SQLite806xDB db806x, R_SAD806x_SignaturesElementsInternalScalars rRow)
        {
            return SQLite806xToolsSAD806xV10.getSignatureElementInternalScalarSubRows(ref db806x, rRow);
        }
    }
}
