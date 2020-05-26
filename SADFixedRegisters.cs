using System;
using System.Collections.Generic;
using System.Text;

namespace SAD806x
{
    public static class SADFixedRegisters
    {
        // Fixed Elements Enums
        public enum FixedRegisters
        {
            UNDEFINED,
            SCALER_SAMPLE,
            N_RPM,
            ECT,
            ECT_EU,
            ECT_w,
            ECT_w_EU,
            ACT,
            ACT_EU,
            ACT_WORD,
            IECT,
            IACT,
            VBAT,
            ITP,
            IMAF,
            IMAP,
            IEVP,
            FPUMP_DC,
            CHT_ER_TMR,
            CHT,
            CHT_RES,
            CHT_LONG,
            CHT_ENG
        }

        public static S6xRegister GetFixedRegisterTemplate(FixedRegisters fixedRegister)
        {
            S6xRegister oRes = new S6xRegister();

            switch (fixedRegister)
            {
                case FixedRegisters.SCALER_SAMPLE:
                    oRes.Units = "SC";
                    break;
                case FixedRegisters.N_RPM:
                    oRes.Label = "n_RPM";
                    oRes.Comments = oRes.Label;
                    oRes.ScaleExpression = "X/4";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "Rpm";
                    break;
                case FixedRegisters.ECT:
                    oRes.Label = "eCT";
                    oRes.Comments = oRes.Label;
                    oRes.ScaleExpression = "X*2";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°F";
                    break;
                case FixedRegisters.ECT_EU:
                    oRes.Label = "eCT";
                    oRes.Comments = oRes.Label;
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°C";
                    break;
                case FixedRegisters.ECT_w:
                    oRes.Label = "eCTw";
                    oRes.Comments = oRes.Label;
                    oRes.ScaleExpression = "X/128";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°F";
                    break;
                case FixedRegisters.ECT_w_EU:
                    oRes.Label = "eCTw";
                    oRes.Comments = oRes.Label;
                    oRes.ScaleExpression = "X/256";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°C";
                    break;
                case FixedRegisters.ACT:
                    oRes.Label = "aCT";
                    oRes.Comments = oRes.Label;
                    oRes.ScaleExpression = "X*2";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°F";
                    break;
                case FixedRegisters.ACT_EU:
                    oRes.Label = "aCT";
                    oRes.Comments = oRes.Label;
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°C";
                    break;
                case FixedRegisters.ACT_WORD:
                    oRes.Label = "aCT_WORD";
                    oRes.Comments = oRes.Label;
                    oRes.ScaleExpression = "X*2";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°F";
                    break;
                case FixedRegisters.IECT:
                    oRes.Label = "iECT";
                    oRes.Comments = "Engine coolant temperature sensor voltage.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts";
                    break;
                case FixedRegisters.IACT:
                    oRes.Label = "iACT";
                    oRes.Comments = "Air charge temperature sensor voltage.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts";
                    break;
                case FixedRegisters.VBAT:
                    oRes.Label = "vBAT";
                    oRes.Comments = "Battery voltage reference.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts";
                    break;
                case FixedRegisters.ITP:
                    oRes.Label = "iTP";
                    oRes.Comments = "Throttle position sensor voltage.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts";
                    break;
                case FixedRegisters.IMAF:
                    oRes.Label = "iMAF";
                    oRes.Comments = "Mass air flow sensor voltage/counts.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts/counts";
                    break;
                case FixedRegisters.IMAP:
                    oRes.Label = "iMAP";
                    oRes.Comments = "Manifold absolute pressure sensor voltage/counts.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts/counts";
                    break;
                case FixedRegisters.IEVP:
                    oRes.Label = "iEVP";
                    oRes.Comments = "Exhaust Gas Recirculation (EGR) Valve Position Sensor.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts/counts";
                    break;
                case FixedRegisters.FPUMP_DC:
                    oRes.Label = "fPUMP_DC";
                    oRes.Comments = "Fuel Pump voltage.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts";
                    break;
                case FixedRegisters.CHT_ER_TMR:
                    oRes.Label = "cHT_ER_TMR";
                    oRes.Comments = "Cylinder Head Temperature Error TMR.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "Counts";
                    break;
                case FixedRegisters.CHT:
                    oRes.Label = "cHT";
                    oRes.Comments = "Cylinder Head Temperature.";
                    oRes.ScaleExpression = "X*2";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°F";
                    break;
                case FixedRegisters.CHT_RES:
                    oRes.Label = "cHT_RES";
                    oRes.Comments = "Cylinder Head Temperature Res.";
                    oRes.ScaleExpression = "X*2";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°F";
                    break;
                case FixedRegisters.CHT_LONG:
                    oRes.Label = "cHT_LONG";
                    oRes.Comments = "Cylinder Head Temperature Long.";
                    oRes.ScaleExpression = "X*2";
                    oRes.ScalePrecision = 0;
                    oRes.Units = "°F";
                    break;
                case FixedRegisters.CHT_ENG:
                    oRes.Label = "cHT_ENG";
                    oRes.Comments = "Cylinder Head Temperature sensor voltage.";
                    oRes.ScaleExpression = "X";
                    oRes.ScalePrecision = 2;
                    oRes.Units = "Volts";
                    break;
                default:
                    oRes.Label = fixedRegister.ToString();
                    break;
            }

            return oRes;
        }
    }
}
