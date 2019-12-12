using System;
using System.Collections;
using System.Text;
using System.Xml.Serialization;

namespace SAD806x
{
    [Serializable]
    [XmlRoot("EMBEDDEDDATA")]
    public class XdfData
    {
        [XmlAttribute]
        public string mmedtypeflags { get; set; }           // Signed / Lsb First / Floating Point  0x01 + 0x02 + 0x04
        [XmlAttribute]
        public string mmedaddress { get; set; }             // Bin Address 0x2418
        [XmlAttribute]
        public string mmedelementsizebits { get; set; }     // Byte/Word/DWord    8 / 16 / 32
        [XmlAttribute]
        public string mmedrowcount { get; set; }            // Rows Count    9
        [XmlAttribute]
        public string mmedcolcount { get; set; }            // Cols Count    9
        [XmlAttribute]
        public string mmedmajorstridebits { get; set; }     // Address Step -4 Word => 32    -2 Byte => 16 on Functions Only, 0 else
        [XmlAttribute]
        public string mmedminorstridebits { get; set; }     // 0
    }

    [Serializable]
    [XmlRoot("DALINK")]
    public class XdfLink
    {
        [XmlAttribute]
        public string index { get; set; }                   // 0
        [XmlAttribute]
        public string objidhash { get; set; }               // null

        public XdfLink()
        {
            index = "0";
        }
    }

    [Serializable]
    [XmlRoot("MATH")]
    public class XdfMath
    {
        [XmlAttribute]
        public string equation { get; set; }                // X/256 or X/1 or X*1

        [XmlElement("VAR")]
        public XdfMathVar xdfMathVar { get; set; }

        public XdfMath()
        {
            xdfMathVar = new XdfMathVar();
        }
    }

    [Serializable]
    [XmlRoot("VAR")]
    public class XdfMathVar
    {
        [XmlAttribute]
        public string id { get; set; }                      // X

        public XdfMathVar()
        {
            id = "X";
        }
    }

    [Serializable]
    [XmlRoot("embedinfo")]
    public class XdfInfo
    {
        [XmlAttribute]
        public string type { get; set; }            // 1 for function, 2 for table or null
        [XmlAttribute]
        public string linkobjid { get; set; }       // Scaler Unique Xdf Id 0x2B1B
    }

    [Serializable]
    [XmlRoot("LABEL")]
    public class XdfLabel
    {
        [XmlAttribute]
        public string index { get; set; }
        [XmlAttribute]
        public string value { get; set; }
    }

    [Serializable]
    [XmlRoot("XDFAXIS")]
    public class XdfAxis
    {
        [XmlAttribute]
        public string id { get; set; }              // x/y/z
        [XmlAttribute]
        public string uniqueid { get; set; }        // 0x0, null for z

        [XmlElement("EMBEDDEDDATA")]
        public XdfData xdfData { get; set; }

        public string units { get; set; }           // Rpm
        public string indexcount { get; set; }      // Cols/Rows number Iso for function, null for table z
        public string decimalpl { get; set; }       // 2 for 1.00, 3 for 1.000
        public string min { get; set; }             // Min Value    0.000000
        public string max { get; set; }             // Max Value    25.000000
        public string outputtype { get; set; }      // 1 for Floating Point / 2 for Integer / 3 ... / 4 ...

        [XmlElement("embedinfo")]
        public XdfInfo xdfInfo { get; set; }
        
        public string datatype { get; set; }        // 0
        public string unittype { get; set; }        // 0

        [XmlElement("DALINK")]
        public XdfLink xdfLink { get; set; }

        [XmlElement("LABEL")]
        public XdfLabel[] xdfLabels { get; set; }

        [XmlElement("MATH")]
        public XdfMath xdfMath { get; set; }

        public XdfAxis()
        {
            xdfData = new XdfData();
            xdfInfo = new XdfInfo();
            xdfLink = new XdfLink();
            xdfMath = new XdfMath();

            decimalpl = "2";
            outputtype = "1";
            datatype = "0";
            unittype = "0";
        }
    }

    [Serializable]
    [XmlRoot("XDFPATCHENTRY")]
    public class XdfPatchEntry
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string address { get; set; }
        [XmlAttribute]
        public string datasize { get; set; }
        [XmlAttribute]
        public string patchdata { get; set; }
    }

    [Serializable]
    [XmlRoot("XDFPATCH")]
    public class XdfPatch
    {
        [XmlAttribute]
        public string uniqueid { get; set; }        // Unique Xdf Id 0x2B1B
        [XmlAttribute]
        public string flags { get; set; }           // null

        public string title { get; set; }
        public string description { get; set; }

        [XmlElement("XDFPATCHENTRY")]
        public XdfPatchEntry xdfPatchEntry { get; set; }

        [XmlIgnore]
        public string titleXmlValid { get { return ToolsXml.CleanXmlString(title); } }
        [XmlIgnore]
        public string descriptionXmlValid { get { return ToolsXml.CleanXmlString(description); } }
    }

    [Serializable]
    [XmlRoot("XDFFLAG")]
    public class XdfFlag
    {
        [XmlAttribute]
        public string uniqueid { get; set; }        // Unique Xdf Id 0x2B1B
        [XmlAttribute]
        public string flags { get; set; }           // null

        public string title { get; set; }
        public string description { get; set; }

        [XmlElement("EMBEDDEDDATA")]
        public XdfData xdfData { get; set; }

        public string mask { get; set; }

        [XmlElement("DALINK")]
        public XdfLink xdfLink { get; set; }

        [XmlElement("MATH")]
        public XdfMath xdfMath { get; set; }

        [XmlIgnore]
        public string mmeMainAddress
        {
            get
            {
                try { return xdfData.mmedaddress; }
                catch { }
                return string.Empty;
            }
        }

        [XmlIgnore]
        public string titleXmlValid { get { return ToolsXml.CleanXmlString(title); } }
        [XmlIgnore]
        public string descriptionXmlValid { get { return ToolsXml.CleanXmlString(description); } }

        public XdfFlag()
        {
            xdfData = new XdfData();
        }

        public XdfFlag(S6xScalar s6xScalar, S6xBitFlag s6xBitFlag, int xdfBaseOffset)
        {
            Init();
            Import(s6xScalar, s6xBitFlag, xdfBaseOffset);
        }

        public void Init()
        {
            xdfData = new XdfData();

            uniqueid = "0xFFFF";
            flags = null;

            xdfData.mmedmajorstridebits = "0";
            xdfData.mmedminorstridebits = "0";
            xdfData.mmedrowcount = null;
            xdfData.mmedcolcount = null;

            mask = "0x0001";
        }

        public void Import(S6xScalar s6xScalar, S6xBitFlag s6xBitFlag, int xdfBaseOffset)
        {
            title = s6xBitFlag.Label;
            description = Tools.XDFLabelSLabelComXdfComment(s6xBitFlag.Label, s6xBitFlag.ShortLabel, s6xBitFlag.Comments);

            xdfData.mmedaddress = Tools.xdfAddressFromBinAddress(s6xScalar.AddressBinInt, xdfBaseOffset);
            xdfData.mmedtypeflags = "0x02";
            if (s6xScalar.Byte) xdfData.mmedelementsizebits = "8";
            else xdfData.mmedelementsizebits = "16";

            mask = "0x" + string.Format("{0:x4}", (int)Math.Pow(2, s6xBitFlag.Position));
        }

        public string getMmedAddress()
        {
            if (xdfData != null) return xdfData.mmedaddress;
            return "0x0";
        }
    }

    [Serializable]
    [XmlRoot("XDFCONSTANT")]
    public class XdfScalar
    {
        [XmlAttribute]
        public string uniqueid { get; set; }        // Unique Xdf Id 0x2B1B
        [XmlAttribute]
        public string flags { get; set; }           // null

        public string title { get; set; }
        public string description { get; set; }

        [XmlElement("EMBEDDEDDATA")]
        public XdfData xdfData { get; set; }

        public string units { get; set; }           // Rpm
        public string decimalpl { get; set; }       // 2 for 1.00, 3 for 1.000
        public string outputtype { get; set; }      // 1 for Floating Point / 2 for Integer / 3 Hex / 4 Ascii

        public string min { get; set; }             // Min Value    0.000000
        public string max { get; set; }             // Max Value    25.000000

        public string datatype { get; set; }        // 0

        public string unittype { get; set; }        // 0

        [XmlElement("DALINK")]
        public XdfLink xdfLink { get; set; }

        [XmlElement("MATH")]
        public XdfMath xdfMath { get; set; }

        [XmlIgnore]
        public string mmeMainAddress
        {
            get
            {
                try { return xdfData.mmedaddress; }
                catch { }
                return string.Empty;
            }
        }

        [XmlIgnore]
        public string titleXmlValid { get { return ToolsXml.CleanXmlString(title); } }
        [XmlIgnore]
        public string descriptionXmlValid { get { return ToolsXml.CleanXmlString(description); } }

        public XdfScalar()
        {
            xdfData = new XdfData();
            xdfLink = new XdfLink();
            xdfMath = new XdfMath();
        }

        public XdfScalar(S6xScalar s6xScalar, int xdfBaseOffset)
        {
            Init();
            Import(s6xScalar, xdfBaseOffset);
        }

        public XdfScalar(ReservedAddress resAdr, int xdfBaseOffset)
        {
            Init();
            Import(resAdr, xdfBaseOffset);
        }

        public void Init()
        {
            xdfData = new XdfData();
            xdfLink = new XdfLink();
            xdfMath = new XdfMath();

            uniqueid = "0xFFFF";
            flags = null;

            xdfData.mmedmajorstridebits = "0";
            xdfData.mmedminorstridebits = "0";
            xdfData.mmedrowcount = null;
            xdfData.mmedcolcount = null;

            decimalpl = "2";
            min = null;
            max = null;
            datatype = "0";
            unittype = "0";
        }

        public void Import(S6xScalar s6xScalar, int xdfBaseOffset)
        {
            title = s6xScalar.Label;
            description = Tools.XDFLabelSLabelComXdfComment(s6xScalar.Label, s6xScalar.ShortLabel, s6xScalar.Comments);

            xdfData.mmedaddress = Tools.xdfAddressFromBinAddress(s6xScalar.AddressBinInt, xdfBaseOffset);
            if (s6xScalar.Signed) xdfData.mmedtypeflags = "0x03";
            else xdfData.mmedtypeflags = "0x02";
            if (s6xScalar.Byte) xdfData.mmedelementsizebits = "8";
            else xdfData.mmedelementsizebits = "16";

            units = s6xScalar.Units;

            xdfMath.equation = s6xScalar.ScaleExpression.Trim();
        }

        public void Import(ReservedAddress resAdr, int xdfBaseOffset)
        {
            title = resAdr.Label;
            description = Tools.XDFLabelSLabelComXdfComment(resAdr.Label, resAdr.ShortLabel, resAdr.Comments);

            xdfData.mmedaddress = Tools.xdfAddressFromBinAddress(resAdr.AddressBinInt, xdfBaseOffset);
            xdfData.mmedtypeflags = "0x02";
            if (resAdr.Size == 1) xdfData.mmedelementsizebits = "8";
            else xdfData.mmedelementsizebits = "16";

            switch (resAdr.Type)
            {
                case ReservedAddressType.Ascii:
                    // Ascii not managed in Xdf for Scalars
                case ReservedAddressType.Hex:
                case ReservedAddressType.CheckSum:
                    outputtype = "3";
                    break;
                default:
                    outputtype = "2";   // Integer
                    break;
            }

            // Specific case
            if (resAdr.AddressInt == 0xdf9c)
            // RTAXLE
            {
                outputtype = "1";   // Float
                xdfMath.equation = "X/1024";
            }
        }

        public string getMmedAddress()
        {
            if (xdfData != null) return xdfData.mmedaddress;
            return "0x0";
        }
    }

    [Serializable]
    [XmlRoot("XDFFUNCTION")]
    public class XdfFunction
    {
        [XmlAttribute]
        public string uniqueid { get; set; }        // Unique Xdf Id 0x2B1B
        [XmlAttribute]
        public string flags { get; set; }           // 0x0

        public string title { get; set; }
        public string description { get; set; }

        [XmlElement("XDFAXIS")]
        public XdfAxis[] xdfAxis { get; set; }

        [XmlIgnore]
        public string mmeMainAddress
        {
            get
            {
                try
                {
                    foreach (XdfAxis axis in xdfAxis) if (axis.id.Trim().ToLower() == "x") return axis.xdfData.mmedaddress;
                }
                catch { }
                return string.Empty;
            }
        }

        [XmlIgnore]
        public string titleXmlValid { get { return ToolsXml.CleanXmlString(title); } }
        [XmlIgnore]
        public string descriptionXmlValid { get { return ToolsXml.CleanXmlString(description); } }

        public XdfFunction()
        {
            xdfAxis = new XdfAxis[2];
            xdfAxis[0] = new XdfAxis();
            xdfAxis[1] = new XdfAxis();
        }

        public XdfFunction(S6xFunction s6xFunction, int xdfBaseOffset)
        {
            Init();
            Import(s6xFunction, xdfBaseOffset);
        }

        private void Init()
        {
            xdfAxis = new XdfAxis[2];
            xdfAxis[0] = new XdfAxis();
            xdfAxis[1] = new XdfAxis();

            uniqueid = "0xFFFF";
            flags = "0x0";

            xdfAxis[0].id = "x";
            xdfAxis[0].uniqueid = "0x0";

            xdfAxis[0].xdfData.mmedminorstridebits = "0";
            xdfAxis[0].xdfData.mmedrowcount = null;

            xdfAxis[0].decimalpl = "2";
            xdfAxis[0].outputtype = "1";
            xdfAxis[0].xdfInfo.type = "1";
            xdfAxis[0].datatype = "0";
            xdfAxis[0].unittype = "0";
            xdfAxis[0].xdfLink.index = "0";

            xdfAxis[1].id = "y";
            xdfAxis[1].uniqueid = "0x0";

            xdfAxis[1].xdfData.mmedminorstridebits = "0";
            xdfAxis[1].xdfData.mmedrowcount = null;

            xdfAxis[1].decimalpl = "2";
            xdfAxis[1].outputtype = "1";
            xdfAxis[1].xdfInfo.type = "1";
            xdfAxis[1].datatype = "0";
            xdfAxis[1].unittype = "0";
            xdfAxis[1].xdfLink.index = "0";
        }

        public void Import(S6xFunction s6xFunction, int xdfBaseOffset)
        {
            title = s6xFunction.Label;
            description = s6xFunction.Comments;

            xdfAxis[0].xdfData.mmedaddress = Tools.xdfAddressFromBinAddress(s6xFunction.AddressBinInt, xdfBaseOffset);
            if (s6xFunction.SignedInput) xdfAxis[0].xdfData.mmedtypeflags = "0x03";
            else xdfAxis[0].xdfData.mmedtypeflags = "0x02";
            if (s6xFunction.ByteInput) xdfAxis[0].xdfData.mmedelementsizebits = "8";
            else xdfAxis[0].xdfData.mmedelementsizebits = "16";
            if (s6xFunction.ByteInput) xdfAxis[0].xdfData.mmedmajorstridebits = "-16";
            else xdfAxis[0].xdfData.mmedmajorstridebits = "-32";
            xdfAxis[0].xdfData.mmedcolcount = s6xFunction.RowsNumber.ToString();

            xdfAxis[0].units = s6xFunction.InputUnits;
            xdfAxis[0].indexcount = s6xFunction.RowsNumber.ToString();
            xdfAxis[0].xdfMath.equation = s6xFunction.InputScaleExpression.Trim();

            if (s6xFunction.ByteInput) xdfAxis[1].xdfData.mmedaddress = Tools.xdfAddressFromBinAddress(s6xFunction.AddressBinInt + 1, xdfBaseOffset);  // Based on Input Type
            else xdfAxis[1].xdfData.mmedaddress = Tools.xdfAddressFromBinAddress(s6xFunction.AddressBinInt + 2, xdfBaseOffset);
            if (s6xFunction.SignedOutput) xdfAxis[1].xdfData.mmedtypeflags = "0x03";
            else xdfAxis[1].xdfData.mmedtypeflags = "0x02";
            if (s6xFunction.ByteOutput) xdfAxis[1].xdfData.mmedelementsizebits = "8";
            else xdfAxis[1].xdfData.mmedelementsizebits = "16";
            if (s6xFunction.ByteOutput) xdfAxis[1].xdfData.mmedmajorstridebits = "-16";
            else xdfAxis[1].xdfData.mmedmajorstridebits = "-32";
            xdfAxis[1].xdfData.mmedcolcount = s6xFunction.RowsNumber.ToString();

            xdfAxis[1].units = s6xFunction.OutputUnits;
            xdfAxis[1].indexcount = s6xFunction.RowsNumber.ToString();
            xdfAxis[1].xdfMath.equation = s6xFunction.OutputScaleExpression.Trim();
        }

        public string getMmedAddress()
        {
            if (xdfAxis != null)
            {
                foreach (XdfAxis axis in xdfAxis)
                {
                    if (axis.id.ToLower() == "x" && axis.xdfData != null) return axis.xdfData.mmedaddress;
                }
            }
            return "0x0";
        }
    }

    [Serializable]
    [XmlRoot("XDFTABLE")]
    public class XdfTable
    {
        [XmlAttribute]
        public string uniqueid { get; set; }            // Unique Xdf Id 0x2B1B
        [XmlAttribute]
        public string flags { get; set; }               // 0x30

        public string title { get; set; }
        public string description { get; set; }

        [XmlElement("XDFAXIS")]
        public XdfAxis[] xdfAxis { get; set; }

        [XmlIgnore]
        public string mmeMainAddress
        {
            get
            {
                try
                {
                    foreach (XdfAxis axis in xdfAxis) if (axis.id.Trim().ToLower() == "z") return axis.xdfData.mmedaddress;
                }
                catch { }
                return string.Empty;
            }
        }

        [XmlIgnore]
        public string titleXmlValid { get { return ToolsXml.CleanXmlString(title); } }
        [XmlIgnore]
        public string descriptionXmlValid { get { return ToolsXml.CleanXmlString(description); } }

        public XdfTable()
        {
            xdfAxis = new XdfAxis[3];
            xdfAxis[0] = new XdfAxis();
            xdfAxis[1] = new XdfAxis();
            xdfAxis[2] = new XdfAxis();
        }

        public XdfTable(S6xTable s6xTable, int xdfBaseOffset)
        {
            Init();
            Import(s6xTable, xdfBaseOffset);
        }

        public XdfTable(ReservedAddress resAdr, int xdfBaseOffset)
        {
            Init();
            Import(resAdr, xdfBaseOffset);
        }

        private void Init()
        {
            xdfAxis = new XdfAxis[3];
            xdfAxis[0] = new XdfAxis();
            xdfAxis[1] = new XdfAxis();
            xdfAxis[2] = new XdfAxis();

            uniqueid = "0xFFFF";
            flags = "0x0";

            xdfAxis[0].id = "x";
            xdfAxis[0].uniqueid = "0x0";

            xdfAxis[0].xdfData.mmedaddress = null;
            xdfAxis[0].xdfData.mmedtypeflags = null;
            xdfAxis[0].xdfData.mmedelementsizebits = "8";
            xdfAxis[0].xdfData.mmedmajorstridebits = "-32";
            xdfAxis[0].xdfData.mmedminorstridebits = "0";

            xdfAxis[0].decimalpl = null;
            xdfAxis[0].outputtype = null;
            xdfAxis[0].xdfInfo.type = "2";
            xdfAxis[0].datatype = "0";
            xdfAxis[0].unittype = "0";
            xdfAxis[0].xdfLink.index = "0";
            xdfAxis[0].xdfMath.equation = "X";

            xdfAxis[1].id = "y";
            xdfAxis[1].uniqueid = "0x0";

            xdfAxis[1].xdfData.mmedaddress = null;
            xdfAxis[1].xdfData.mmedtypeflags = null;
            xdfAxis[1].xdfData.mmedelementsizebits = "8";
            xdfAxis[1].xdfData.mmedmajorstridebits = "-32";
            xdfAxis[1].xdfData.mmedminorstridebits = "0";

            xdfAxis[1].decimalpl = null;
            xdfAxis[1].outputtype = null;
            xdfAxis[1].xdfInfo.type = "2";
            xdfAxis[1].datatype = "0";
            xdfAxis[1].unittype = "0";
            xdfAxis[1].xdfLink.index = "0";
            xdfAxis[1].xdfMath.equation = "X";

            xdfAxis[2].id = "z";
            xdfAxis[2].uniqueid = "0x0";

            xdfAxis[2].xdfData.mmedelementsizebits = "8";
            xdfAxis[2].xdfData.mmedmajorstridebits = "0";
            xdfAxis[2].xdfData.mmedminorstridebits = "0";

            xdfAxis[2].indexcount = null;
            xdfAxis[2].decimalpl = "2";
            xdfAxis[2].outputtype = "1";
            xdfAxis[2].xdfInfo = null;
            xdfAxis[2].datatype = "0";
            xdfAxis[2].unittype = "0";
            xdfAxis[2].xdfLink = null;
        }
        
        public void Import(S6xTable s6xTable, int xdfBaseOffset)
        {
            title = s6xTable.Label;
            description = Tools.XDFLabelSLabelComXdfComment(s6xTable.Label, s6xTable.ShortLabel, s6xTable.Comments);

            xdfAxis[0].units = s6xTable.ColsUnits;
            xdfAxis[0].indexcount = s6xTable.ColsNumber.ToString();

            if (s6xTable.ColsScalerXdfUniqueId == null && s6xTable.ColsScalerXdfUniqueId == string.Empty)
            {
                xdfAxis[0].xdfInfo = null;
            }
            else
            {
                if (xdfAxis[0].xdfInfo == null)
                {
                    xdfAxis[0].xdfInfo = new XdfInfo();
                    xdfAxis[0].xdfInfo.type = "2";
                    xdfAxis[0].xdfInfo.linkobjid = s6xTable.ColsScalerXdfUniqueId;
                }
                else
                {
                    xdfAxis[0].xdfInfo.linkobjid = s6xTable.ColsScalerXdfUniqueId;
                }
            }

            xdfAxis[1].units = s6xTable.RowsUnits;
            xdfAxis[1].indexcount = s6xTable.RowsNumber.ToString();
            if (s6xTable.RowsScalerXdfUniqueId == null && s6xTable.RowsScalerXdfUniqueId == string.Empty)
            {
                xdfAxis[1].xdfInfo = null;
            }
            else
            {
                if (xdfAxis[1].xdfInfo == null)
                {
                    xdfAxis[1].xdfInfo = new XdfInfo();
                    xdfAxis[1].xdfInfo.type = "2";
                    xdfAxis[1].xdfInfo.linkobjid = s6xTable.RowsScalerXdfUniqueId;
                }
                else
                {
                    xdfAxis[1].xdfInfo.linkobjid = s6xTable.RowsScalerXdfUniqueId;
                }
            }

            xdfAxis[2].xdfData.mmedaddress = Tools.xdfAddressFromBinAddress(s6xTable.AddressBinInt, xdfBaseOffset);
            if (s6xTable.WordOutput) xdfAxis[2].xdfData.mmedelementsizebits = "16";
            else xdfAxis[2].xdfData.mmedelementsizebits = "8";
            if (s6xTable.SignedOutput) xdfAxis[2].xdfData.mmedtypeflags = "0x03";
            else xdfAxis[2].xdfData.mmedtypeflags = "0x02";
            xdfAxis[2].xdfData.mmedrowcount = s6xTable.RowsNumber.ToString();
            xdfAxis[2].xdfData.mmedcolcount = s6xTable.ColsNumber.ToString();

            xdfAxis[2].units = s6xTable.CellsUnits;
            xdfAxis[2].xdfMath.equation = s6xTable.CellsScaleExpression.Trim();
        }

        public void Import(ReservedAddress resAdr, int xdfBaseOffset)
        {
            int iCols = -1;
            string sOutputType = "3";
            string sSize = "8";

            title = resAdr.Label;
            description = Tools.XDFLabelSLabelComXdfComment(resAdr.Label, resAdr.ShortLabel, resAdr.Comments);

            sSize = "8";
            iCols = resAdr.Size;

            switch (resAdr.Type)
            {
                case ReservedAddressType.Ascii:
                    sOutputType = "4";
                    // For Ascii Xdf requires 1 Byte one per Column
                    break;
                case ReservedAddressType.Hex:
                    sOutputType = "3";
                    if (resAdr.Size % 4 == 0)
                    {
                        sSize = "32";
                        iCols = resAdr.Size / 4;
                    }
                    else if (resAdr.Size % 2 == 0)
                    {
                        sSize = "16";
                        iCols = resAdr.Size / 2;
                    }
                    break;
            }

            xdfAxis[0].indexcount = iCols.ToString();

            xdfAxis[1].indexcount = "1";

            xdfAxis[2].outputtype = sOutputType;
            xdfAxis[2].xdfData.mmedaddress = Tools.xdfAddressFromBinAddress(resAdr.AddressBinInt, xdfBaseOffset);
            xdfAxis[2].xdfData.mmedelementsizebits = sSize;
            xdfAxis[2].xdfData.mmedtypeflags = "0x00";
            xdfAxis[2].xdfData.mmedrowcount = "1";
            xdfAxis[2].xdfData.mmedcolcount = iCols.ToString();
        }

        public string getMmedAddress()
        {
            if (xdfAxis != null)
            {
                foreach (XdfAxis axis in xdfAxis)
                {
                    if (axis.id.ToLower() == "z" && axis.xdfData != null) return axis.xdfData.mmedaddress;
                }
            }
            return "0x0";
        }
    }

    [Serializable]
    [XmlRoot("BASEOFFSET")]
    public class XdfHeaderBaseOffset
    {
        [XmlAttribute]
        public string offset { get; set; }      // 0x2000 => offset = 8192
        [XmlAttribute]
        public string subtract { get; set; }    // Checked => 1
    }

    [Serializable]
    [XmlRoot("DEFAULTS")]
    public class XdfHeaderDefaults
    {
        [XmlAttribute]
        public string datasizeinbits { get; set; }      // 8 or 16 or 32
        [XmlAttribute]
        public string sigdigits { get; set; }
        [XmlAttribute]
        public string outputtype { get; set; }
        [XmlAttribute]
        public string signed { get; set; }
        [XmlAttribute]
        public string lsbfirst { get; set; }
        [XmlAttribute("float")]
        public string ffloat { get; set; }
    }

    [Serializable]
    [XmlRoot("REGION")]
    public class XdfHeaderRegion
    {
        [XmlAttribute]
        public string type { get; set; }
        [XmlAttribute]
        public string startaddress { get; set; }
        [XmlAttribute]
        public string size { get; set; }
        [XmlAttribute]
        public string regionflags { get; set; }
        [XmlAttribute]
        public string name { get; set; }
        [XmlAttribute]
        public string desc { get; set; }
    }

    [Serializable]
    [XmlRoot("CATEGORY")]
    public class XdfHeaderCategory
    {
        [XmlAttribute]
        public string index { get; set; }
        [XmlAttribute]
        public string name { get; set; }
    }

    [Serializable]
    [XmlRoot("XDFHEADER")]
    public class XdfHeader
    {
        public string flags { get; set; }
        public string fileversion { get; set; }
        public string deftitle { get; set; }
        public string description { get; set; }
        public string author { get; set; }

        [XmlElement("baseoffset")]
        public string baseoffset1_50 { get; set; }

        [XmlElement("BASEOFFSET")]
        public XdfHeaderBaseOffset xdfBaseOffset { get; set; }
        [XmlElement("DEFAULTS")]
        public XdfHeaderDefaults xdfDefaults { get; set; }
        [XmlElement("REGION")]
        public XdfHeaderRegion xdfRegion { get; set; }
        [XmlElement("CATEGORY")]
        public XdfHeaderCategory xdfCategories { get; set; }

        [XmlIgnore]
        public string deftitleXmlValid { get { return ToolsXml.CleanXmlString(deftitle); } }
        [XmlIgnore]
        public string descriptionXmlValid { get { return ToolsXml.CleanXmlString(description); } }
        [XmlIgnore]
        public string authorXmlValid { get { return ToolsXml.CleanXmlString(author); } }
    }

    [Serializable]
    [XmlRoot("REGION")]
    public class XdfChecksumRegion
    {
        public string datastart { get; set; }
        public string dataend { get; set; }
        public string datasizebits { get; set; }
        public string storeaddress { get; set; }
        public string calculationmethod { get; set; }
    }

    [Serializable]
    [XmlRoot("XDFCHECKSUM")]
    public class XdfChecksum
    {
        [XmlAttribute]
        public string uniqueid { get; set; }
        [XmlAttribute]
        public string flags { get; set; }

        public string title { get; set; }

        [XmlElement("REGION")]
        public XdfChecksumRegion xdfRegion { get; set; }

        [XmlIgnore]
        public string titleXmlValid { get { return ToolsXml.CleanXmlString(title); } }
    }

    [Serializable]
    [XmlRoot("XDFFORMAT")]
    public class XdfFile
    {
        [XmlAttribute]
        public string version { get; set; }

        [XmlElement("XDFHEADER")]
        public XdfHeader xdfHeader { get; set; }

        [XmlElement("XDFCHECKSUM")]
        public XdfChecksum[] xdfChecksums { get; set; }

        [XmlElement("XDFTABLE")]
        public XdfTable[] xdfTables { get; set; }

        [XmlElement("XDFFUNCTION")]
        public XdfFunction[] xdfFunctions { get; set; }

        [XmlElement("XDFCONSTANT")]
        public XdfScalar[] xdfScalars { get; set; }

        [XmlElement("XDFFLAG")]
        public XdfFlag[] xdfFlags { get; set; }

        [XmlElement("XDFPATCH")]
        public XdfPatch[] xdfPatches { get; set; }

        public XdfFile()
        {
        }
    

        public XdfFile(ref SADBin sadBin)
        {
            int lastXdfUniqueId = 0;
            ArrayList alXdfTables = null;
            ArrayList alXdfFunctions = null;
            ArrayList alXdfScalars = null;
            int xdfBaseOffset = 0;

            version = "1.60";

            xdfHeader = new XdfHeader();
            xdfHeader.xdfBaseOffset = new XdfHeaderBaseOffset();
            
            xdfHeader.deftitle = sadBin.S6x.Properties.Label;
            xdfHeader.description = sadBin.S6x.Properties.Comments;

            xdfHeader.xdfBaseOffset.offset = "0";
            xdfHeader.xdfBaseOffset.subtract = "0";
            try
            {
                xdfHeader.xdfBaseOffset.offset = Convert.ToInt32(sadBin.S6x.Properties.XdfBaseOffset, 16).ToString();
                if (sadBin.S6x.Properties.XdfBaseOffsetSubtract) xdfHeader.xdfBaseOffset.subtract = "1";
            }
            catch { }

            xdfBaseOffset = Convert.ToInt32(xdfHeader.xdfBaseOffset.offset);
            if (xdfHeader.xdfBaseOffset.subtract == "1") xdfBaseOffset *= -1;

            xdfHeader.xdfRegion = new XdfHeaderRegion();
            xdfHeader.xdfRegion.type = "0xFFFFFFFF";
            xdfHeader.xdfRegion.startaddress = "0x0";
            xdfHeader.xdfRegion.size = string.Format("0x{0:X4}", sadBin.BinaryFileSize);
            xdfHeader.xdfRegion.regionflags = "0x0";
            xdfHeader.xdfRegion.name = "Binary File";
            xdfHeader.xdfRegion.desc = "This region describes the bin file edited by this XDF";

            alXdfTables = new ArrayList();
            alXdfFunctions = new ArrayList();
            alXdfScalars = new ArrayList();

            foreach (S6xTable s6xObject in sadBin.S6x.slTables.Values)
            {
                if (!s6xObject.Skip && s6xObject.Store && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    XdfTable xdfObject = new XdfTable(s6xObject, xdfBaseOffset);
                    xdfObject.uniqueid = "0x" + string.Format("{0:x4}", lastXdfUniqueId);
                    lastXdfUniqueId++;
                    alXdfTables.Add(xdfObject);
                    xdfObject = null;
                }
            }
            foreach (S6xFunction s6xObject in sadBin.S6x.slFunctions.Values)
            {
                if (!s6xObject.Skip && s6xObject.Store && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    XdfFunction xdfObject = new XdfFunction(s6xObject, xdfBaseOffset);
                    xdfObject.uniqueid = "0x" + string.Format("{0:x4}", lastXdfUniqueId);
                    lastXdfUniqueId++;
                    alXdfFunctions.Add(xdfObject);
                    xdfObject = null;
                }
            }
            foreach (S6xScalar s6xObject in sadBin.S6x.slScalars.Values)
            {
                if (!s6xObject.Skip && s6xObject.Store && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    XdfScalar xdfObject = new XdfScalar(s6xObject, xdfBaseOffset);
                    xdfObject.uniqueid = "0x" + string.Format("{0:x4}", lastXdfUniqueId);
                    lastXdfUniqueId++;
                    alXdfScalars.Add(xdfObject);
                    xdfObject = null;
                }
            }

            xdfTables = (XdfTable[])alXdfTables.ToArray(typeof(XdfTable));
            xdfFunctions = (XdfFunction[])alXdfFunctions.ToArray(typeof(XdfFunction));
            xdfScalars = (XdfScalar[])alXdfScalars.ToArray(typeof(XdfScalar));

            alXdfTables = null;
            alXdfFunctions = null;
            alXdfScalars = null;

            GC.Collect();
        }

        public string getLastXdfUniqueId()
        {
            int lastXdfUniqueId = 0;
            int currentXdfUniqueId = 0;

            if (xdfChecksums != null)
            {
                foreach (XdfChecksum xdfObject in xdfChecksums)
                {
                    currentXdfUniqueId = Convert.ToInt32(xdfObject.uniqueid.ToLower().Replace("0x", ""), 16);
                    if (currentXdfUniqueId > lastXdfUniqueId) lastXdfUniqueId = currentXdfUniqueId;
                }
            }
            if (xdfTables != null)
            {
                foreach (XdfTable xdfObject in xdfTables)
                {
                    currentXdfUniqueId = Convert.ToInt32(xdfObject.uniqueid.ToLower().Replace("0x", ""), 16);
                    if (currentXdfUniqueId > lastXdfUniqueId) lastXdfUniqueId = currentXdfUniqueId;
                }
            }
            if (xdfFunctions != null)
            {
                foreach (XdfFunction xdfObject in xdfFunctions)
                {
                    currentXdfUniqueId = Convert.ToInt32(xdfObject.uniqueid.ToLower().Replace("0x", ""), 16);
                    if (currentXdfUniqueId > lastXdfUniqueId) lastXdfUniqueId = currentXdfUniqueId;
                }
            }
            if (xdfScalars != null)
            {
                foreach (XdfScalar xdfObject in xdfScalars)
                {
                    currentXdfUniqueId = Convert.ToInt32(xdfObject.uniqueid.ToLower().Replace("0x", ""), 16);
                    if (currentXdfUniqueId > lastXdfUniqueId) lastXdfUniqueId = currentXdfUniqueId;
                }
            }
            if (xdfFlags != null)
            {
                foreach (XdfFlag xdfObject in xdfFlags)
                {
                    currentXdfUniqueId = Convert.ToInt32(xdfObject.uniqueid.ToLower().Replace("0x", ""), 16);
                    if (currentXdfUniqueId > lastXdfUniqueId) lastXdfUniqueId = currentXdfUniqueId;
                }
            }
            if (xdfPatches != null)
            {
                foreach (XdfPatch xdfObject in xdfPatches)
                {
                    currentXdfUniqueId = Convert.ToInt32(xdfObject.uniqueid.ToLower().Replace("0x", ""), 16);
                    if (currentXdfUniqueId > lastXdfUniqueId) lastXdfUniqueId = currentXdfUniqueId;
                }
            }

            return "0x" + string.Format("{0:x4}", lastXdfUniqueId);
        }
    }
}
