using System;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.IO;

namespace SAD806x
{
    public static class ToolsXml
    {
        public static string CleanXmlString(string sXml)
        {
            StringBuilder sB = new StringBuilder();

            if (sXml == null || sXml == string.Empty) return string.Empty; // vacancy test.
            for (int i = 0; i < sXml.Length; i++)
            {
                char cChar = sXml[i];

                if ((cChar == 0x9 || cChar == 0xA || cChar == 0xD) ||
                    ((cChar >= 0x20) && (cChar <= 0xD7FF)) ||
                    ((cChar >= 0xE000) && (cChar <= 0xFFFD))) // ||
                    //((cChar >= 0x10000) && (cChar <= 0x10FFFF)))
                {
                    sB.Append(cChar);
                }
            }
            return sB.ToString();
        }        
        
        public static void Serialize(ref MemoryStream sStr, object oObject)
        {
            XmlTextWriter xtWri = null;
            XmlSerializer xSer = null;
            XmlSerializerNamespaces xNsp = null;

            xtWri = new XmlTextWriter(sStr, Encoding.UTF8);
            xSer = new XmlSerializer(oObject.GetType());

            xNsp = new XmlSerializerNamespaces();
            xNsp.Add(string.Empty, string.Empty);

            xSer.Serialize(xtWri, oObject, xNsp);
            sStr.WriteByte(0);

            xNsp = null;
            xSer = null;
            xtWri = null;
        }

        public static void SerializeXdf(ref MemoryStream sStr, object oObject)
        {
            XmlWriterSettings xWriSettings = null;
            XmlWriter xWri = null;
            XmlSerializer xSer = null;
            XmlSerializerNamespaces xNsp = null;

            xWriSettings = new XmlWriterSettings();
            xWriSettings.OmitXmlDeclaration = true;
            //xWriSettings.Encoding = new UnicodeEncoding(false, true);
            xWriSettings.Encoding = Encoding.ASCII; //  Only Ascii is working
            xWri = XmlWriter.Create(sStr, xWriSettings);
            xSer = new XmlSerializer(oObject.GetType());

            xNsp = new XmlSerializerNamespaces();
            xNsp.Add(string.Empty, string.Empty);

            xSer.Serialize(xWri, oObject, xNsp);
            sStr.WriteByte(0);

            xNsp = null;
            xSer = null;
            xWri = null;
        }

        public static void Serialize(ref StreamWriter sWri, object oObject)
        {
            XmlTextWriter xtWri = null;
            XmlSerializer xSer = null;
            XmlSerializerNamespaces xNsp = null;

            xtWri = new XmlTextWriter(sWri);
            xSer = new XmlSerializer(oObject.GetType());

            xNsp = new XmlSerializerNamespaces();
            xNsp.Add(string.Empty, string.Empty);

            xSer.Serialize(sWri, oObject, xNsp);

            xNsp = null;
            xSer = null;
            xtWri = null;
        }

        public static void Serialize(ref XPathNavigator xNav, object oObject)
        {
            XmlSerializer xSer = null;
            XmlSerializerNamespaces xNsp = null;
            XmlWriter xWri = null;

            xSer = new XmlSerializer(oObject.GetType());

            xNsp = new XmlSerializerNamespaces();
            xNsp.Add(string.Empty, string.Empty);

            xWri = xNav.AppendChild();
            
            xWri.WriteWhitespace(string.Empty);

            xSer.Serialize(xWri, oObject, xNsp);
            xWri.Close();

            xWri = null;
            xNsp = null;
            xSer = null;
        }

        public static object Deserialize(ref MemoryStream mStr, Type type)
        {
            XmlSerializer xSer = null;
            XmlTextReader xtRea = null;

            try
            {
                xtRea = new XmlTextReader(mStr);
                xSer = new XmlSerializer(type);
                return xSer.Deserialize(xtRea);
            }
            catch
            {
                return null;
            }
            finally
            {
                xtRea = null;
                xSer = null;
            }
        }

        
        public static object Deserialize(ref StreamReader xRea, Type type)
        {
            XmlSerializer xSer = null;
            XmlTextReader xtRea = null;

            try
            {
                xtRea = new XmlTextReader(xRea);
                xSer = new XmlSerializer(type);
                return xSer.Deserialize(xtRea);
            }
            catch
            {
                return null;
            }
            finally
            {
                xtRea = null;
                xSer = null;
            }
        }

        public static object DeserializeFile(string filePath, Type type)
        {
            StreamReader xRea = null;
            object oResult = null;

            try
            {
                if (!File.Exists(filePath)) return null;

                xRea = new StreamReader(filePath);
                oResult = Deserialize(ref xRea, type);
                xRea.Close();
                xRea = null;

                return oResult;
            }
            catch
            {
                return null;
            }
            finally
            {
                try { xRea.Close(); }
                catch { }
                xRea = null;
            }
        }
    }
}
