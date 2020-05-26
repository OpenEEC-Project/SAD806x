using System;
using System.Collections.Generic;
using System.Text;

namespace SAD806x
{
    public static class SADFixedScalars
    {
        // Fixed Elements Enums
        public enum FixedScalars
        {
            UNDEFINED,
            ACTMAX,
            ACTMIN,
            ACTFMM,
            ECTMAX,
            ECTMIN,
            ECTFMM,
            NUMEGO
        }

        public static S6xScalar GetFixedScalarTemplate(FixedScalars fixedScalar)
        {
            S6xScalar oRes = new S6xScalar();

            switch (fixedScalar)
            {
                case FixedScalars.ACTMAX:
                    oRes.ShortLabel = "ACTMAX";
                    oRes.Label = "ACT Max";
                    oRes.Comments = oRes.ShortLabel + " - " + oRes.Label;
                    oRes.Comments += "\r\nValue at which the eec considers that the sensor has failed open circuit.";
                    oRes.ScaleExpression = "X/12800";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts";
                    break;
                case FixedScalars.ACTMIN:
                    oRes.ShortLabel = "ACTMIN";
                    oRes.Label = "ACT Min";
                    oRes.Comments = oRes.ShortLabel + " - " + oRes.Label;
                    oRes.Comments += "\r\nValue at which the eec considers that the sensor has failed short circuit.";
                    oRes.ScaleExpression = "X/12800";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts";
                    break;
                case FixedScalars.ACTFMM:
                    oRes.ShortLabel = "ACTFMM";
                    oRes.Label = "ACT FMM Default";
                    oRes.Comments = oRes.ShortLabel + " - " + oRes.Label;
                    oRes.Comments += "\r\nFailed ACT value, should be set to the hottest the engine will run at IAT wise.";
                    oRes.ScaleExpression = "X*2";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°F";
                    break;
                case FixedScalars.ECTMAX:
                    oRes.ShortLabel = "ECTMAX";
                    oRes.Label = "ECT Max";
                    oRes.Comments = oRes.ShortLabel + " - " + oRes.Label;
                    oRes.Comments += "\r\nValue at which the eec considers that the sensor has failed open circuit.";
                    oRes.ScaleExpression = "X/12800";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts";
                    break;
                case FixedScalars.ECTMIN:
                    oRes.ShortLabel = "ECTMIN";
                    oRes.Label = "ECT Min";
                    oRes.Comments = oRes.ShortLabel + " - " + oRes.Label;
                    oRes.Comments += "\r\nValue at which the eec considers that the sensor has failed short circuit.";
                    oRes.ScaleExpression = "X/12800";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts";
                    break;
                case FixedScalars.ECTFMM:
                    oRes.ShortLabel = "ECTFMM";
                    oRes.Label = "ECT FMM Default";
                    oRes.Comments = oRes.ShortLabel + " - " + oRes.Label;
                    oRes.Comments += "\r\nFailed ECT value, should be set to the hottest the engine will run at ECT wise.";
                    oRes.ScaleExpression = "X*2";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°F";
                    break;
                case FixedScalars.NUMEGO:
                    oRes.ShortLabel = "NUMEGO";
                    oRes.Label = "Number of HEGOs";
                    oRes.Comments = oRes.ShortLabel + " - " + oRes.Label;
                    oRes.Comments += "\r\nWhen changing from 2 to 1 the injector output port must be reassigned, all bank2 injectors must be assigned to bank1 otherwise all bank2 injectors will be disabled.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "SW";
                    break;
                default:
                    oRes.ShortLabel = fixedScalar.ToString();
                    oRes.Label = oRes.ShortLabel;
                    break;
            }

            return oRes;
        }
    }
}
