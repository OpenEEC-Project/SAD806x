using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;

namespace SAD806x
{
    [Serializable]
    [XmlRoot("Repository")]
    public class Repository
    {
        [XmlAttribute]
        public DateTime generated { get; set; }

        [XmlArray(ElementName = "Items")]
        [XmlArrayItem(ElementName = "Item")]
        public List<RepositoryItem> Items { get; set; }

        [XmlIgnore]
        public bool isSaved { get; set; }

        public Repository()
        {
            Items = new List<RepositoryItem>();
        }

        public bool Save(string filePath)
        {
            XmlDocument xDoc = null;

            try
            {
                if (isSaved) return true;

                xDoc = new XmlDocument();
                generated = DateTime.UtcNow;
                generateXDoc(ref xDoc);
                xDoc.Save(filePath);

                isSaved = true;
                return true;
            }
            catch
            {
                isSaved = false;
                return false;
            }
            finally
            {
                xDoc = null;
                GC.Collect();
            }
        }

        private void generateXDoc(ref XmlDocument xDoc)
        {
            XPathNavigator xNav = null;

            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
            xNav = xDoc.CreateNavigator();
            ToolsXml.Serialize(ref xNav, this);
            xNav = null;

            GC.Collect();
        }
    }

    [Serializable]
    [XmlRoot("Item")]
    public class RepositoryItem
    {
        [XmlAttribute]
        public string ShortLabel { get; set; }
        [XmlAttribute]
        public DateTime updated { get; set; }
        
        public string Label { get; set; }
        public string Comments { get; set; }
        public string Information { get; set; }

        [XmlIgnore]
        public string FullLabel
        {
            get
            {
                if (ShortLabel == null) return Label;
                else if (Label == null) return ShortLabel;
                else if (ShortLabel == Label) return ShortLabel;
                else return ShortLabel + " - " + Label;
            }
        }

        public RepositoryItem()
        {

        }

        public RepositoryItem(string shortLabel, string label)
        {
            ShortLabel = shortLabel;
            Label = label;
            updated = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return FullLabel;
        }
    }

    [Serializable]
    [XmlRoot("RepositoryConversion")]
    public class RepositoryConversion
    {
        [XmlAttribute]
        public DateTime generated { get; set; }

        [XmlArray(ElementName = "Items")]
        [XmlArrayItem(ElementName = "Item")]
        public List<RepositoryConversionItem> Items { get; set; }

        [XmlIgnore]
        public bool isSaved { get; set; }

        public RepositoryConversion()
        {
            Items = new List<RepositoryConversionItem>();
        }

        public bool Save(string filePath)
        {
            XmlDocument xDoc = null;

            try
            {
                if (isSaved) return true;

                xDoc = new XmlDocument();
                generated = DateTime.UtcNow;
                generateXDoc(ref xDoc);
                xDoc.Save(filePath);

                isSaved = true;
                return true;
            }
            catch
            {
                isSaved = false;
                return false;
            }
            finally
            {
                xDoc = null;
                GC.Collect();
            }
        }

        private void generateXDoc(ref XmlDocument xDoc)
        {
            XPathNavigator xNav = null;

            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
            xNav = xDoc.CreateNavigator();
            ToolsXml.Serialize(ref xNav, this);
            xNav = null;

            GC.Collect();
        }
    }

    [Serializable]
    [XmlRoot("Item")]
    public class RepositoryConversionItem
    {
        [XmlAttribute]
        public string Title { get; set; }
        [XmlAttribute]
        public DateTime updated { get; set; }

        public string InternalFormula { get; set; }
        public string Comments { get; set; }
        public string Information { get; set; }

        public RepositoryConversionItem()
        {

        }

        public RepositoryConversionItem(string title)
        {
            Title = title;
            InternalFormula = "X*1";
            updated = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
