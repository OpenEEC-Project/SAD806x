using System;
using System.Collections.Generic;
using System.Text;

namespace SAD806x
{
    public static class SADFixedTables
    {
        // Fixed Elements Enums
        public enum FixedTables
        {
            UNDEFINED,
        }

        public static S6xTable GetFixedTableTemplate(FixedTables fixedTable)
        {
            S6xTable oRes = new S6xTable();

            switch (fixedTable)
            {
                default:
                    oRes.ShortLabel = fixedTable.ToString();
                    oRes.Label = oRes.ShortLabel;
                    break;
            }

            return oRes;
        }
    }
}
