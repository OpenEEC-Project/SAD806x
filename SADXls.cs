using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SAD806x
{
    // EEC Analyser / Binary Editor XLS/XLSX Format

    public class XlsConfigParameter
    {
        public string Parameter { get; set; }
        public string Value { get; set; }
    }

    public class XlsRevision
    {
        public string Revision { get; set; }
        public string Comments { get; set; }
    }

    public class XlsLevel
    {
        public string Value { get; set; }           // Is Key
        public string Label { get; set; }           // Level Label
    }

    public class XlsQHorse
    {
        public string Address { get; set; }
        public string Data { get; set; }
        public string Parameter { get; set; }
        public string Value { get; set; }
    }

    public class XlsTwEECer
    {
        public string Address { get; set; }
        public string Data { get; set; }
        public string Parameter { get; set; }
        public string Value { get; set; }
    }
    
    public class XlsPayLoad
    {
        public int AddressInt { get; set; }         // Is Key and Address in Header
        
        public string Label { get; set; }           // TAG in header
        public string Description { get; set; }
        public string Comments { get; set; }
        public int Bytes { get; set; }
        public bool Signed { get; set; }
        public string Equation { get; set; }
        public int Digits { get; set; }
        public string Units { get; set; }

        public List<string> BitsArray { get; set; }     // BITX in header, X is 0 to 7, Value is the label

        public string Address { get { return string.Format("0x{0:x4}", AddressInt); } }

        public void Import(S6xRegister s6xRegister)
        {
            AddressInt = s6xRegister.AddressInt;
            Label = s6xRegister.Label;
            Description = s6xRegister.Comments;
            Bytes = s6xRegister.SizeStatus == "Word" ? 2 : 1;
            Signed = s6xRegister.SignedStatus == "Signed" ? true : false;
            Equation = s6xRegister.ScaleExpression;
            Digits = s6xRegister.ScalePrecision;
            Units = s6xRegister.Units;

            if (s6xRegister.isBitFlags)
            {
                if (BitsArray == null) BitsArray = new List<string>() { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
                foreach (S6xBitFlag s6xBF in s6xRegister.BitFlags)
                {
                    if (s6xBF.Position > 7) continue;
                    BitsArray[s6xBF.Position] = s6xBF.Label;
                }
            }
        }
    }

    public class XlsScalar
    {
        public int AddressInt { get; set; }         // Is Key and Address in Header

        public string Key { get; set; }             // Unique
        public string PID { get; set; }             // Short Label

        public string Parameter { get; set; }       // Label
        public string Units { get; set; }
        public string Comments { get; set; }
        public int Bytes { get; set; }
        public bool Signed { get; set; }
        public int Digits { get; set; }             // DecimalPl in Header
        public string Equation { get; set; }

        public string EnumName { get; set; }
        public string EnumVal { get; set; }

        public string Min { get; set; }
        public string Max { get; set; }

        public string ParentPID { get; set; }
        public string ParentVal { get; set; }

        public XlsLevel Level { get; set; }
        public XlsLevel Level2 { get; set; }

        public string Address { get { return string.Format("0x{0:x6}", AddressInt); } }

        public void Import(S6xScalar s6xScalar, int xlsBaseOffset, SortedList<string, XlsLevel> xlsLevels)
        {
            PID = s6xScalar.ShortLabel;
            Key = s6xScalar.DuplicateNum == 0 ? Key : Key + "_" + s6xScalar.DuplicateNum.ToString();

            Parameter = s6xScalar.Label;
            Comments = s6xScalar.Comments;

            if (s6xScalar.Category != null && s6xScalar.Category != string.Empty)
            {
                foreach (XlsLevel xlsLevel in xlsLevels.Values)
                {
                    if (xlsLevel.Label.ToUpper() == s6xScalar.Category.ToUpper())
                    {
                        Level = xlsLevel;
                        break;
                    }
                }
            }

            if (s6xScalar.Category2 != null && s6xScalar.Category2 != string.Empty)
            {
                foreach (XlsLevel xlsLevel in xlsLevels.Values)
                {
                    if (xlsLevel.Label.ToUpper() == s6xScalar.Category2.ToUpper())
                    {
                        Level2 = xlsLevel;
                        break;
                    }
                }
            }

            AddressInt = Tools.xlsAddressFromBinAddress(s6xScalar.AddressBinInt, xlsBaseOffset);

            Units = s6xScalar.Units;
            Bytes = s6xScalar.Byte ? 1 : 2;
            Signed = s6xScalar.Signed;

            Equation = s6xScalar.ScaleExpression;
            Digits = s6xScalar.ScalePrecision;

            Min = Tools.getValidMinMax(s6xScalar.Min);
            Max = Tools.getValidMinMax(s6xScalar.Max);
        }
    }

    public class XlsFunction
    {
        public int AddressInt { get; set; }         // Is Key and Address in Header

        public string Key { get; set; }             // Unique
        public string PID { get; set; }             // Short Label

        public string Title { get; set; }           // Label
        public string Comments { get; set; }
        
        public int Rows { get; set; }
        public int Bytes { get; set; }

        public bool XSigned { get; set; }
        public int XDigits { get; set; }            // DecimalPl in Header
        public string XEquation { get; set; }
        public string XUnits { get; set; }
        public string XMin { get; set; }
        public string XMax { get; set; }
        public XlsPayLoad XPayLoad { get; set; }    // Based on XPayloadTag in Header
        
        public bool YSigned { get; set; }
        public int YDigits { get; set; }            // DecimalPl in Header
        public string YEquation { get; set; }
        public string YUnits { get; set; }
        public string YMin { get; set; }
        public string YMax { get; set; }
        public XlsPayLoad YPayLoad { get; set; }    // Based on YPayloadTag in Header
        
        public string ParentPID { get; set; }
        public string ParentVal { get; set; }

        public XlsLevel Level { get; set; }
        public XlsLevel Level2 { get; set; }

        public string Address { get { return string.Format("0x{0:x6}", AddressInt); } }

        public void Import(S6xFunction s6xFunction, int xlsBaseOffset, SortedList<string, XlsLevel> xlsLevels)
        {
            PID = s6xFunction.ShortLabel;
            Key = s6xFunction.DuplicateNum == 0 ? Key : Key + "_" + s6xFunction.DuplicateNum.ToString();

            Title = s6xFunction.Label;
            Comments = s6xFunction.Comments;

            if (s6xFunction.Category != null && s6xFunction.Category != string.Empty)
            {
                foreach (XlsLevel xlsLevel in xlsLevels.Values)
                {
                    if (xlsLevel.Label.ToUpper() == s6xFunction.Category.ToUpper())
                    {
                        Level = xlsLevel;
                        break;
                    }
                }
            }

            if (s6xFunction.Category2 != null && s6xFunction.Category2 != string.Empty)
            {
                foreach (XlsLevel xlsLevel in xlsLevels.Values)
                {
                    if (xlsLevel.Label.ToUpper() == s6xFunction.Category2.ToUpper())
                    {
                        Level2 = xlsLevel;
                        break;
                    }
                }
            }
            
            AddressInt = Tools.xlsAddressFromBinAddress(s6xFunction.AddressBinInt, xlsBaseOffset);
            Bytes = s6xFunction.ByteInput ? 1 : 2;
            Rows = s6xFunction.RowsNumber;

            XSigned = s6xFunction.SignedInput;
            XUnits = s6xFunction.InputUnits;
            XEquation = s6xFunction.InputScaleExpression.Trim();
            XDigits = s6xFunction.InputScalePrecision;
            XMin = Tools.getValidMinMax(s6xFunction.InputMin);
            XMax = Tools.getValidMinMax(s6xFunction.InputMax);

            YSigned = s6xFunction.SignedOutput;
            YUnits = s6xFunction.OutputUnits;
            YEquation = s6xFunction.OutputScaleExpression.Trim();
            YDigits = s6xFunction.OutputScalePrecision;
            YMin = Tools.getValidMinMax(s6xFunction.OutputMin);
            YMax = Tools.getValidMinMax(s6xFunction.OutputMax);
        }
    }

    public class XlsTable
    {
        public int AddressInt { get; set; }         // Key and Address in Header

        public string Key { get; set; }             // Unique
        public string PID { get; set; }             // Short Label

        public string Title { get; set; }           // Label
        public string Comments { get; set; }
        
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int Bytes { get; set; }
        public bool Signed { get; set; }

        public int Digits { get; set; }             // DecimalPl in Header
        public string Equation { get; set; }        // ZEquation in Header
        public string Units { get; set; }           // ZUnits in Header
        public string Min { get; set; }
        public string Max { get; set; }

        public XlsFunction XFunction { get; set; }  // Based on XLabelLink in Header
        public string XLabels { get; set; }   // Default Labels when no Function is known
        public string XUnits { get; set; }
        public XlsPayLoad XPayLoad { get; set; }    // Based on XPayloadTag in Header

        public XlsFunction YFunction { get; set; }  // Based on YLabelLink in Header
        public string YLabels { get; set; }   // Default Labels when no Function is known
        public string YUnits { get; set; }
        public XlsPayLoad YPayLoad { get; set; }    // Based on YPayloadTag in Header

        public string ParentPID { get; set; }
        public string ParentVal { get; set; }

        public string LevelXlsKey { get; set; }
        public string Level2XlsKey { get; set; }

        public XlsLevel Level { get; set; }
        public XlsLevel Level2 { get; set; }

        public string Address { get { return string.Format("0x{0:x6}", AddressInt); } }

        public void Import(S6xTable s6xTable, int xlsBaseOffset, SortedList<string, XlsLevel> xlsLevels)
        {
            PID = s6xTable.ShortLabel;
            Key = s6xTable.DuplicateNum == 0 ? Key : Key + "_" + s6xTable.DuplicateNum.ToString();

            Title = s6xTable.Label;
            Comments = s6xTable.Comments;

            if (s6xTable.Category != null && s6xTable.Category != string.Empty)
            {
                foreach (XlsLevel xlsLevel in xlsLevels.Values)
                {
                    if (xlsLevel.Label.ToUpper() == s6xTable.Category.ToUpper())
                    {
                        Level = xlsLevel;
                        break;
                    }
                }
            }

            if (s6xTable.Category2 != null && s6xTable.Category2 != string.Empty)
            {
                foreach (XlsLevel xlsLevel in xlsLevels.Values)
                {
                    if (xlsLevel.Label.ToUpper() == s6xTable.Category2.ToUpper())
                    {
                        Level2 = xlsLevel;
                        break;
                    }
                }
            }

            AddressInt = Tools.xlsAddressFromBinAddress(s6xTable.AddressBinInt, xlsBaseOffset);

            Cols = s6xTable.ColsNumber;
            Rows = s6xTable.RowsNumber;
            Bytes = s6xTable.WordOutput ? 2 : 1;
            Signed = s6xTable.SignedOutput;

            Equation = s6xTable.CellsScaleExpression;
            Digits = s6xTable.CellsScalePrecision;
            Units = s6xTable.CellsUnits;

            Min = Tools.getValidMinMax(s6xTable.CellsMin);
            Max = Tools.getValidMinMax(s6xTable.CellsMax);

            XUnits = s6xTable.ColsUnits;
            if ((XUnits == null || XUnits == string.Empty) && XFunction != null) XUnits = XFunction.XUnits;
            YUnits = s6xTable.RowsUnits;
            if ((YUnits == null || YUnits == string.Empty) && YFunction != null) YUnits = YFunction.XUnits;
        }
    }

    public class XlsFile
    {
        public List<XlsConfigParameter> ConfigParameters = null;
        public List<XlsRevision> Revisions = null;
        public SortedList<string, XlsLevel> Levels = null;

        public List<XlsQHorse> QHorses = null;
        public List<XlsTwEECer> TwEECers = null;
        
        public SortedList<int, XlsPayLoad> PayLoads = null;
        public SortedList<string, XlsScalar> Scalars = null;
        public SortedList<string, XlsFunction> Functions = null;
        public SortedList<string, XlsTable> Tables = null;

        private bool valid = false;
        public bool Valid { get { return valid; } }

        private string filePath = string.Empty;
        public string FilePath { get { return filePath; } }

        private string fileName = string.Empty;
        public string FileName { get { return fileName; } }

        public void Create(string xlsFilePath, ref SADBin sadBin)
        {
            filePath = xlsFilePath;
            fileName = Path.GetFileName(xlsFilePath);
            valid = true;

            ConfigParameters = new List<XlsConfigParameter>();
            Revisions = new List<XlsRevision>();
            Levels = new SortedList<string, XlsLevel>();
            QHorses = new List<XlsQHorse>();
            TwEECers = new List<XlsTwEECer>();
            PayLoads = new SortedList<int, XlsPayLoad>();
            Scalars = new SortedList<string, XlsScalar>();
            Functions = new SortedList<string, XlsFunction>();
            Tables = new SortedList<string, XlsTable>();

            // To be checked
            // Some Minimum records on Config or Revisions could be required
        }

        public void Load(string xlsFilePath, bool forceXlsAlt)
        {
            XlsFile refXlsFile = this;
            valid = ToolsXls.ReadXlsFile(ref refXlsFile, xlsFilePath, forceXlsAlt, true);
            refXlsFile = null;

            if (valid)
            {
                filePath = xlsFilePath;
                fileName = Path.GetFileName(xlsFilePath);
            }
        }

        public bool Save(string saveFilePath, ref List<string> lstErrors)
        {
            XlsFile refXlsFile = this;
            bool bResult = ToolsXls.WriteXlsFile(ref refXlsFile, saveFilePath, ref lstErrors);
            refXlsFile = null;

            return bResult;
        }
    }
}
