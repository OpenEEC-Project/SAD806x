using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SAD806x;

namespace SQLite806x
{
    public static class SQLite806xToolsSAD
    {
        public static string getDBVersion()
        {
            return SQLite806xToolsSADV10.getDBVersion();
        }

        public static DateTime getDBVersionDate()
        {
            return SQLite806xToolsSADV10.getDBVersionDate();
        }

        public static string getDBVersionComments()
        {
            return SQLite806xToolsSADV10.getDBVersionComments();
        }

        public static void addDBVersionRow(SQLite806xDB db806x, ref List<R_SAD_DB_Information> rList)
        {
            SQLite806xToolsSADV10.addDBVersionRow(db806x, ref rList);
        }

        public static void initSchema(ref List<T_SQLiteTable> sqlTables)
        {
            SQLite806xToolsSADV10.initSchema(ref sqlTables);
        }
    }
}
