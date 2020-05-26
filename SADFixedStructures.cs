using System;
using System.Collections.Generic;
using System.Text;

namespace SAD806x
{
    public static class SADFixedStructures
    {
        // Fixed Elements Enums
        public enum FixedStructures
        {
            UNDEFINED,
            CORE_REG_INIT_ST1,
            CORE_REG_INIT_ST2
        }

        public static S6xStructure GetFixedStructureTemplate(FixedStructures fixedStructure)
        {
            S6xStructure oRes = new S6xStructure();

            switch (fixedStructure)
            {
                // For Routine Signature Internal Structure
                case FixedStructures.CORE_REG_INIT_ST1:
                    oRes.ShortLabel = "CORE_REG_INIT_ST1";
                    oRes.Label = "Core Registers Initialization Structure 1";
                    oRes.Comments = oRes.ShortLabel + " - " + oRes.Label;
                    oRes.Number = 0;                                            // To be updated by processing
                    oRes.StructDef = "\"Reg \", WordHex, \"Reg \", WordHex";    // To be updated by processing
                    break;
                // For Routine Signature Internal Structure
                case FixedStructures.CORE_REG_INIT_ST2:
                    oRes.ShortLabel = "CORE_REG_INIT_ST2";
                    oRes.Label = "Core Registers Initialization Structure 2";
                    oRes.Comments = oRes.ShortLabel + " - " + oRes.Label;
                    oRes.Number = 0;                                            // To be updated by processing
                    oRes.StructDef = "\"Reg \", WordHex, \"Reg \", WordHex";    // To be updated by processing
                    break;
                default:
                    oRes.ShortLabel = fixedStructure.ToString();
                    oRes.Label = oRes.ShortLabel;
                    break;
            }

            return oRes;
        }
    }
}
