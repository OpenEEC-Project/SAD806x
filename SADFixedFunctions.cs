using System;
using System.Collections.Generic;
using System.Text;

namespace SAD806x
{
    public static class SADFixedFunctions
    {
        // Fixed Elements Enums
        public enum FixedFunctions
        {
            UNDEFINED,
            FN703
        }

        public static S6xFunction GetFixedFunctionTemplate(FixedFunctions fixedFunction)
        {
            S6xFunction oRes = new S6xFunction();

            switch (fixedFunction)
            {
                case FixedFunctions.FN703:
                    oRes.ShortLabel = "FN703";
                    oRes.Label = "ECT/ACT Transfer";
                    oRes.Comments = oRes.ShortLabel + " - " + oRes.Label;
                    oRes.Comments += "\r\nInput is a rough high byte only conversion.";
                    oRes.InputScaleExpression = "X/50";
                    oRes.InputScalePrecision = 2;
                    oRes.InputUnits = "Volts";
                    oRes.OutputScaleExpression = "X*2";
                    oRes.OutputScalePrecision = 0;
                    oRes.OutputUnits = "°F";
                    break;
                default:
                    oRes.ShortLabel = fixedFunction.ToString();
                    oRes.Label = oRes.ShortLabel;
                    break;
            }

            return oRes;
        }
    }
}
