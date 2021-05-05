using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SAD806x;

namespace SQLite806x
{
    public static class SQLite806xToolsTunerPro
    {
        public static string getDBVersion()
        {
            return SQLite806xToolsTunerProV10.getDBVersion();
        }

        public static DateTime getDBVersionDate()
        {
            return SQLite806xToolsTunerProV10.getDBVersionDate();
        }

        public static string getDBVersionComments()
        {
            return SQLite806xToolsTunerProV10.getDBVersionComments();
        }

        public static void addDBVersionRow(SQLite806xDB db806x, ref List<R_TunerPro_DB_Information> rList)
        {
            SQLite806xToolsTunerProV10.addDBVersionRow(db806x, ref rList);
        }

        public static void initSchema(ref List<T_SQLiteTable> sqlTables)
        {
            SQLite806xToolsTunerProV10.initSchema(ref sqlTables);
        }
    }
}
