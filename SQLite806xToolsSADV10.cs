using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using SAD806x;

namespace SQLite806x
{
    public static class SQLite806xToolsSADV10
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

        public static void addDBVersionRow(SQLite806xDB db806x, ref List<R_SAD_DB_Information> rList)
        {
            if (db806x == null) return;
            if (rList == null) return;

            R_SAD_DB_Information rRow = db806x.newRow<R_SAD_DB_Information>();
            rRow.Version.Value = SQLite806xToolsSAD.getDBVersion();
            rRow.Date.Value = SQLite806xToolsSAD.getDBVersionDate();
            rRow.Comments.Value = SQLite806xToolsSAD.getDBVersionComments();

            rList.Add(rRow);
        }

        public static void initSchema(ref List<T_SQLiteTable> sqlTables)
        {
           if (sqlTables == null) sqlTables = new List<T_SQLiteTable>();

           sqlTables.Add(SQLite806xTools.initTable<R_SAD_DB_Information>("Information", "Database Information"));
           sqlTables.Add(SQLite806xTools.initTable<R_SAD_Properties>("Properties", "Properties"));
        }
    }

    public class R_SAD_DB_Information : R_SQLite_DB_Information {}

    public class R_SAD_Properties : R_SQLite_Core
    {
        public F_SQLiteField Option1 { get; set; }

        public new Dictionary<string, F_SQLiteFieldProperties> getFieldsSchemaTypes()
        {
            Dictionary<string, F_SQLiteFieldProperties> dResult = base.getFieldsSchemaTypes();
            dResult.Add("Option1", new F_SQLiteFieldProperties("BOOLEAN"));
            return dResult;
        }
    }
}
