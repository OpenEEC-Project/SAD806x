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
    [XmlRoot("RecentFiles")]
    public class RecentFiles
    {
        [XmlIgnore]
        public string rfsFilePath { get; set; }
        
        [XmlAttribute]
        public DateTime updated { get; set; }

        [XmlArray(ElementName = "Items")]
        [XmlArrayItem(ElementName = "Item")]
        public List<RecentFile> Items { get; set; }

        public RecentFiles()
        {
            Items = new List<RecentFile>();
        }

        public void Update(RecentFile rfNew)
        {
            RecentFile rfExisting = null;
            foreach (RecentFile rfItem in Items)
            {
                if (rfItem.binPath == rfNew.binPath && rfItem.binFileName == rfNew.binFileName)
                {
                    rfExisting = rfItem;
                    break;
                }
            }

            if (rfExisting != null)
            {
                rfExisting.Update(ref rfNew);
                Items.Remove(rfExisting);
                Items.Insert(0, rfExisting);
            }
            else
            {
                SortedList slItems = new SortedList();
                foreach (RecentFile rfItem in Items)
                {
                    try { slItems.Add(rfItem.orderKey, rfItem); }
                    catch { }
                }
                slItems.Add(rfNew.orderKey, rfNew);
                Items.Clear();
                for (int iItem = slItems.Count - 1; iItem >= 0; iItem --)
                {
                    Items.Add((RecentFile)slItems.GetByIndex(iItem));
                    if (Items.Count >= 20) break;
                }
                slItems = null;
            }

            updated = DateTime.UtcNow;
        }
        
        public bool Save()
        {
            XmlDocument xDoc = null;

            if (rfsFilePath == null) return false;
            
            try
            {
                xDoc = new XmlDocument();
                generateXDoc(ref xDoc);
                xDoc.Save(rfsFilePath);

                return true;
            }
            catch
            {
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
    public class RecentFile
    {
        public string binPath { get; set; }
        public string s6xPath { get; set; }

        [XmlAttribute]
        public string binFileName { get; set; }
        [XmlAttribute]
        public string s6xFileName { get; set; }
        [XmlAttribute]
        public DateTime created { get; set; }
        [XmlAttribute]
        public DateTime updated { get; set; }

        [XmlIgnore]
        public string orderKey
        {
            get { return updated.ToString("yyyyMMddHHmmssFFF"); }
        }

        [XmlIgnore]
        public string Label
        {
            get
            {
                if (s6xFileName == null) return binFileName + " / No SAD806x";
                else return binFileName + " / " + s6xFileName;
            }
        }

        [XmlIgnore]
        public string Details
        {
            get
            {
                string details = binPath + "\\" + binFileName;
                if (s6xFileName == null) details += "\r\n" + "No SAD806x";
                else details += "\r\n" + s6xPath + "\\" + s6xFileName;
                try
                {
                    details += "\r\nFirst use: " + created.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    details += "\r\nLast use: " + updated.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                catch { }
                return details;
            }
        }

        public RecentFile()
        {
            created = DateTime.UtcNow;
            updated = DateTime.UtcNow;
        }

        public RecentFile(string binFilePath, string s6xFilePath)
        {
            FileInfo fiFI = null;
            if (File.Exists(binFilePath))
            {
                fiFI = new FileInfo(binFilePath);
                binFileName = fiFI.Name;
                binPath = fiFI.Directory.FullName;
                while (binPath.EndsWith("\\")) binPath = binPath.Substring(0, binPath.Length - 1);
                fiFI = null;
            }
            if (File.Exists(s6xFilePath))
            {
                fiFI = new FileInfo(s6xFilePath);
                s6xFileName = fiFI.Name;
                s6xPath = fiFI.Directory.FullName;
                while (s6xPath.EndsWith("\\")) s6xPath = s6xPath.Substring(0, s6xPath.Length - 1);
                fiFI = null;
            }
            
            created = DateTime.UtcNow;
            updated = DateTime.UtcNow;
        }

        public void Update(ref RecentFile rfNew)
        {
            updated = DateTime.UtcNow;

            if (rfNew.s6xFileName != null)
            {
                s6xFileName = rfNew.s6xFileName;
                s6xPath = rfNew.s6xPath;
            }
        }
        
        public override string ToString()
        {
            return Label;
        }
    }
}
