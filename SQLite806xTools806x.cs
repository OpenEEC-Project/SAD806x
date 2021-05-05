using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SAD806x;

namespace SQLite806x
{
    public static class SQLite806xTools806x
    {
        public static string getDBVersion()
        {
            return SQLite806xTools806xV10.getDBVersion();
        }

        public static DateTime getDBVersionDate()
        {
            return SQLite806xTools806xV10.getDBVersionDate();
        }

        public static string getDBVersionComments()
        {
            return SQLite806xTools806xV10.getDBVersionComments();
        }

        public static void initSchema(ref List<T_SQLiteTable> sqlTables)
        {
            SQLite806xTools806xV10.initSchema(ref sqlTables);
        }

        public static object addDBVersionRow(SQLite806xDB db806x, ref List<R_806x_DB_Information> rList)
        {
            return SQLite806xTools806xV10.addDBVersionRow(db806x, ref rList);
        }
        
        public static object addTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Tables> rList, S6xTable s6xObject)
        {
            return SQLite806xTools806xV10.addTableRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateTableRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Tables rRow, S6xTable s6xObject)
        {
            SQLite806xTools806xV10.updateTableRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Functions> rList, S6xFunction s6xObject)
        {
            return SQLite806xTools806xV10.addFunctionRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateFunctionRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Functions rRow, S6xFunction s6xObject)
        {
            SQLite806xTools806xV10.updateFunctionRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Scalars> rList, S6xScalar s6xObject)
        {
            return SQLite806xTools806xV10.addScalarRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateScalarRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Scalars rRow, S6xScalar s6xObject)
        {
            SQLite806xTools806xV10.updateScalarRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addScalarBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_ScalarsBitFlags> rList, S6xScalar s6xObject)
        {
            return SQLite806xTools806xV10.addScalarBitFlagRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_ScalarsBitFlags> rList, S6xScalar s6xObject, S6xBitFlag s6xBitFlag)
        {
            return SQLite806xTools806xV10.addScalarBitFlagRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xBitFlag);
        }

        public static void updateScalarBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_ScalarsBitFlags rRow, S6xScalar s6xObject, S6xBitFlag s6xBitFlag)
        {
            SQLite806xTools806xV10.updateScalarBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xBitFlag);
        }

        public static object addStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Structures> rList, S6xStructure s6xObject)
        {
            return SQLite806xTools806xV10.addStructureRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateStructureRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Structures rRow, S6xStructure s6xObject)
        {
            SQLite806xTools806xV10.updateStructureRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Routines> rList, S6xRoutine s6xObject)
        {
            return SQLite806xTools806xV10.addRoutineRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateRoutineRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Routines rRow, S6xRoutine s6xObject)
        {
            SQLite806xTools806xV10.updateRoutineRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addRoutineInputArgRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_RoutinesArgs> rList, S6xRoutine s6xObject)
        {
            return SQLite806xTools806xV10.addRoutineInputArgRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_RoutinesArgs> rList, S6xRoutine s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            return SQLite806xTools806xV10.addRoutineInputArgRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xSubObject);
        }

        public static void updateRoutineInputArgRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_RoutinesArgs rRow, S6xRoutine s6xObject, S6xRoutineInputArgument s6xSubObject)
        {
            SQLite806xTools806xV10.updateRoutineInputArgRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xSubObject);
        }

        public static object addOperationRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Operations> rList, S6xOperation s6xObject)
        {
            return SQLite806xTools806xV10.addOperationRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateOperationRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Operations rRow, S6xOperation s6xObject)
        {
            SQLite806xTools806xV10.updateOperationRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addOtherRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Others> rList, S6xOtherAddress s6xObject)
        {
            return SQLite806xTools806xV10.addOtherRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateOtherRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Others rRow, S6xOtherAddress s6xObject)
        {
            SQLite806xTools806xV10.updateOtherRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addRegisterRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_Registers> rList, S6xRegister s6xObject)
        {
            return SQLite806xTools806xV10.addRegisterRow(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static void updateRegisterRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_Registers rRow, S6xRegister s6xObject)
        {
            SQLite806xTools806xV10.updateRegisterRow(ref db806x, ref sadS6x, rRow, s6xObject);
        }

        public static object addRegisterBitFlagRows(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_RegistersBitFlags> rList, S6xRegister s6xObject)
        {
            return SQLite806xTools806xV10.addRegisterBitFlagRows(ref db806x, ref sadS6x, ref rList, s6xObject);
        }

        public static object addRegisterBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, ref List<R_806x_Def_RegistersBitFlags> rList, S6xRegister s6xObject, S6xBitFlag s6xBitFlag)
        {
            return SQLite806xTools806xV10.addRegisterBitFlagRow(ref db806x, ref sadS6x, ref rList, s6xObject, s6xBitFlag);
        }

        public static void updateRegisterBitFlagRow(ref SQLite806xDB db806x, ref SADS6x sadS6x, R_806x_Def_RegistersBitFlags rRow, S6xRegister s6xObject, S6xBitFlag s6xBitFlag)
        {
            SQLite806xTools806xV10.updateRegisterBitFlagRow(ref db806x, ref sadS6x, rRow, s6xObject, s6xBitFlag);
        }

        public static bool deleteTableRow(ref SQLite806xDB db806x, R_806x_Def_Tables rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteTableRow(ref db806x, rRow, recursively);
        }

        public static bool deleteFunctionRow(ref SQLite806xDB db806x, R_806x_Def_Functions rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteFunctionRow(ref db806x, rRow, recursively);
        }

        public static bool deleteScalarRow(ref SQLite806xDB db806x, R_806x_Def_Scalars rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteScalarRow(ref db806x, rRow, recursively);
        }

        public static bool deleteScalarBitFlagRow(ref SQLite806xDB db806x, R_806x_Def_ScalarsBitFlags rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteScalarBitFlagRow(ref db806x, rRow, recursively);
        }

        public static bool deleteStructureRow(ref SQLite806xDB db806x, R_806x_Def_Structures rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteStructureRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRoutineRow(ref SQLite806xDB db806x, R_806x_Def_Routines rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteRoutineRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRoutineInputArgRow(ref SQLite806xDB db806x, R_806x_Def_RoutinesArgs rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteRoutineInputArgRow(ref db806x, rRow, recursively);
        }

        public static bool deleteOperationRow(ref SQLite806xDB db806x, R_806x_Def_Operations rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteOperationRow(ref db806x, rRow, recursively);
        }

        public static bool deleteOtherRow(ref SQLite806xDB db806x, R_806x_Def_Others rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteOtherRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRegisterRow(ref SQLite806xDB db806x, R_806x_Def_Registers rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteRegisterRow(ref db806x, rRow, recursively);
        }

        public static bool deleteRegisterBitFlagRow(ref SQLite806xDB db806x, R_806x_Def_RegistersBitFlags rRow, bool recursively)
        {
            return SQLite806xTools806xV10.deleteRegisterBitFlagRow(ref db806x, rRow, recursively);
        }

        public static object[] setTableS6x(ref SADS6x sadS6x, R_806x_Def_Tables rRow)
        {
            return SQLite806xTools806xV10.setTableS6x(ref sadS6x, rRow);
        }

        public static object[] setFunctionS6x(ref SADS6x sadS6x, R_806x_Def_Functions rRow)
        {
            return SQLite806xTools806xV10.setFunctionS6x(ref sadS6x, rRow);
        }

        public static object[] setScalarS6x(ref SADS6x sadS6x, R_806x_Def_Scalars rRow)
        {
            return SQLite806xTools806xV10.setScalarS6x(ref sadS6x, rRow);
        }

        public static object[] setScalarBitFlagS6x(ref SADS6x sadS6x, R_806x_Def_ScalarsBitFlags rRow)
        {
            return SQLite806xTools806xV10.setScalarBitFlagS6x(ref sadS6x, rRow);
        }

        public static object[] setStructureS6x(ref SADS6x sadS6x, R_806x_Def_Structures rRow)
        {
            return SQLite806xTools806xV10.setStructureS6x(ref sadS6x, rRow);
        }

        public static object[] setRoutineS6x(ref SADS6x sadS6x, R_806x_Def_Routines rRow)
        {
            return SQLite806xTools806xV10.setRoutineS6x(ref sadS6x, rRow);
        }

        public static object[] setRoutineInputArgS6x(ref SADS6x sadS6x, R_806x_Def_RoutinesArgs rRow)
        {
            return SQLite806xTools806xV10.setRoutineInputArgS6x(ref sadS6x, rRow);
        }

        public static object[] setOperationS6x(ref SADS6x sadS6x, R_806x_Def_Operations rRow)
        {
            return SQLite806xTools806xV10.setOperationS6x(ref sadS6x, rRow);
        }

        public static object[] setOtherS6x(ref SADS6x sadS6x, R_806x_Def_Others rRow)
        {
            return SQLite806xTools806xV10.setOtherS6x(ref sadS6x, rRow);
        }

        public static object[] setRegisterS6x(ref SADS6x sadS6x, R_806x_Def_Registers rRow)
        {
            return SQLite806xTools806xV10.setRegisterS6x(ref sadS6x, rRow);
        }

        public static object[] setRegisterBitFlagS6x(ref SADS6x sadS6x, R_806x_Def_RegistersBitFlags rRow)
        {
            return SQLite806xTools806xV10.setRegisterBitFlagS6x(ref sadS6x, rRow);
        }

        public static List<object> getScalarSubRows(ref SQLite806xDB db806x, R_806x_Def_Scalars rRow)
        {
            return SQLite806xTools806xV10.getScalarSubRows(ref db806x, rRow);
        }

        public static List<object> getRoutineSubRows(ref SQLite806xDB db806x, R_806x_Def_Routines rRow)
        {
            return SQLite806xTools806xV10.getRoutineSubRows(ref db806x, rRow);
        }

        public static List<object> getRegisterSubRows(ref SQLite806xDB db806x, R_806x_Def_Registers rRow)
        {
            return SQLite806xTools806xV10.getRegisterSubRows(ref db806x, rRow);
        }
    }
}
