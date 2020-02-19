using System;
using System.Collections;
using System.Text;

namespace SAD806x
{
    // Hard Coded Signature, Will be reused at S6x Definition Creation / Update
    public static class SADFixedSigs
    {
        // Fixed Elements Signatures Array
        // Is 8061 (True, False), Fixed Bank (-1, 0, 1, 8, 9), Calibration Element Enum (Enum), Bytes Signature (String)
        public static object[] Fixed_Elements_Signatures = new object[] {
            new object[] { "FN036_61", "MAF Transfer 8061", "MAF Transfer Signature for 8061", true, string.Empty,  Fixed_Elements.MAF_TRANSFER, Fixed_8061_MAF_Transfer_Use_Following_Ops },
            new object[] { "FN036_65_01", "MAF Transfer 8065 01", "MAF Transfer Signature for 8065 01", false, string.Empty,  Fixed_Elements.MAF_TRANSFER, Fixed_8065_MAF_Transfer_Use_Following_Ops_1 },
            new object[] { "FN036_65_02", "MAF Transfer 8065 02", "MAF Transfer Signature for 8065 02", false, string.Empty,  Fixed_Elements.MAF_TRANSFER, Fixed_8065_MAF_Transfer_Use_Following_Ops_2 },
            new object[] { "FN036_65_03", "MAF Transfer 8065 03", "MAF Transfer Signature for 8065 03", false, string.Empty,  Fixed_Elements.MAF_TRANSFER, Fixed_8065_MAF_Transfer_Use_Following_Ops_3 }
        };

        // Fixed Elements Enums
        public enum Fixed_Elements
        {
            UNKNOWN = 0,
            MAF_TRANSFER = 1
        }

        // Fixed Elements Bytes Signatures
        public const string Fixed_String_OpeIncludingElemAddress = "#EAOP#";
        
        // Only .. have to be used
        public const string Fixed_8061_MAF_Transfer_Use_Following_Ops = "#EAOP#\nA3,..,..,..\n88,..,..\nD9,12\n45,78,00,..,..\n88,..,..\nD3,08\n94,..,..\n71,03,..\nDF,04\nC3,..,..,..\n71,..,..\nF0";
        public const string Fixed_8065_MAF_Transfer_Use_Following_Ops_1 = "#EAOP#\nA3,..,..,..\n88,..,..\nD9,12\n45,78,00,..,..\n88,..,..\nD3,08\n94,..,..\n71,03,..\nDF,04\nC3,..,..,..\n71,..,..\nF0";
        public const string Fixed_8065_MAF_Transfer_Use_Following_Ops_2 = "#EAOP#\nA3,..,..,..,..\n88,..,..\nD9,14\n45,78,00,..,..\n8B,..,..,..,..\nD3,08\n94,..,..\n71,03,..\nDF,05\nC3,..,..,..,..\n71,..,..\nF0";
        public const string Fixed_8065_MAF_Transfer_Use_Following_Ops_3 = "#EAOP#\nA3,..,..,..\n88,..,..\nD9,13\n45,78,00,..,..\n8B,..,..,..\nD3,08\n94,..,..\n71,03,..\nDF,04\nC3,..,..,..\nF0";

        // Get Fixed Elements Definitions
        public static object GetFixedElementS6xRoutineInternalTemplate(Fixed_Elements fixedElement)
        {
            object oRes = null;

            switch (fixedElement)
            {
                case Fixed_Elements.MAF_TRANSFER:
                    oRes = new S6xRoutineInternalFunction();
                    ((S6xRoutineInternalFunction)oRes).ShortLabel = "FN036";
                    ((S6xRoutineInternalFunction)oRes).Label = "MAF Transfer";
                    ((S6xRoutineInternalFunction)oRes).Comments = "";
                    ((S6xRoutineInternalFunction)oRes).ByteInput = false;
                    ((S6xRoutineInternalFunction)oRes).ByteOutput = false;
                    ((S6xRoutineInternalFunction)oRes).SignedInput = false;
                    ((S6xRoutineInternalFunction)oRes).SignedOutput = false;
                    ((S6xRoutineInternalFunction)oRes).RowsNumber = 0;
                    ((S6xRoutineInternalFunction)oRes).InputScaleExpression = "X";
                    ((S6xRoutineInternalFunction)oRes).OutputScaleExpression = "X";
                    ((S6xRoutineInternalFunction)oRes).InputScalePrecision = SADDef.DefaultScalePrecision;
                    ((S6xRoutineInternalFunction)oRes).OutputScalePrecision = SADDef.DefaultScalePrecision;
                    break;
                default:
                    break;
            }

            return oRes;
        }

    }
}
