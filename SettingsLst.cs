using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.ComponentModel;

namespace SAD806x
{
    [Serializable]
    public enum SettingType
    {
        Text = 0,
        LargeText = 1,
        Number = 2,
        DecimalNumber = 3,
        Boolean = 4
    }

    [Serializable]
    public enum SettingSampleMode
    {
        Inactive = 0,
        StartAndLength = 1,
        StartAndValue = 2,
        ValueAndLength = 3
    }
    
    [Serializable]
    [XmlRoot("Settings")]
    public class SettingsLst
    {
        [XmlAttribute]
        public string version { get; set; }

        [XmlAttribute]
        public DateTime generated { get; set; }

        [XmlArray(ElementName = "Items")]
        [XmlArrayItem(ElementName = "Item")]
        public List<SettingItem> Items { get; set; }

        [XmlIgnore]
        public bool isSaved { get; set; }

        [XmlIgnore]
        private SortedList slItems { get; set; }

        [XmlIgnore]
        private SettingItem[] orderedItems { get; set; }

        [XmlIgnore]
        private SortedList slCategs { get; set; }
        
        public SettingsLst()
        {
            Items = new List<SettingItem>();
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

        private void initSortedItems()
        {
            Items.Sort();

            slItems = new SortedList();
            
            foreach (SettingItem sItem in Items)
            {
                try { slItems.Add(sItem.UniqueId, sItem); }
                catch { }
            }

            orderedItems = (SettingItem[])Items.ToArray();

            slCategs = new SortedList();
            slCategs.Add(" All Settings ", new SettingItem[] {});
            foreach (SettingItem sItem in Items)
            {
                if (sItem.Category == null) continue;
                if (sItem.Category == string.Empty) continue;

                if (!slCategs.ContainsKey(sItem.Category)) slCategs.Add(sItem.Category, new SettingItem[] { });
                SettingItem[] newItems = new SettingItem[((SettingItem[])slCategs[sItem.Category]).Length + 1];
                ((SettingItem[])slCategs[sItem.Category]).CopyTo(newItems, 0);
                newItems[newItems.Length - 1] = sItem;
                slCategs[sItem.Category] = newItems;
            }
        }

        public SettingItem[] GetOrderedItems()
        {
            if (slCategs == null) initSortedItems();

            return orderedItems;
        }

        public SettingItem[] GetCategOrderedItems(string categ)
        {
            if (slCategs == null) initSortedItems();

            if (slCategs.ContainsKey(categ)) return (SettingItem[])slCategs[categ];
            else return new SettingItem[] { };
        }

        public string[] GetCategs()
        {
            if (slCategs == null) initSortedItems();
            
            string[] arrCategs = new string[slCategs.Count];
            slCategs.Keys.CopyTo(arrCategs, 0);

            return arrCategs;
        }

        public SettingItem Get(string itemId)
        {
            if (slItems == null) initSortedItems();

            return (SettingItem)slItems[itemId];
        }

        public T Get<T>(string itemId, object defValue)
        {
            TypeConverter cConv = TypeDescriptor.GetConverter(typeof(T));
            if (cConv == null) return (T)defValue;

            SettingItem sItem = Get(itemId);
            if (sItem == null) return (T)defValue;

            try
            {
                return (T)cConv.ConvertFrom(sItem.Value);
            }
            catch
            {
                return (T)defValue;
            }
        }

        public T Get<T>(string itemId)
        {
            return Get<T>(itemId, default(T));
        }
    }

    [Serializable]
    [XmlRoot("Item")]
    public class SettingItem : IComparable<SettingItem>
    {
        [XmlAttribute]
        public string UniqueId { get; set; }
        [XmlAttribute]
        public string UniqueOrder { get; set; }
        [XmlAttribute]
        public DateTime updated { get; set; }
        [XmlAttribute]
        public string Category { get; set; }

        public string Label { get; set; }
        public string Information { get; set; }
        public string Sample { get; set; }
        public SettingSampleMode SampleMode { get; set; }
        public int SampleSelectionStart { get; set; }
        public int SampleSelectionLength { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }
        [XmlAttribute]
        public SettingType Type { get; set; }
        [XmlAttribute]
        public string MinValue { get; set; }
        [XmlAttribute]
        public string MaxValue { get; set; }

        public string DefaultValue { get; set; }

        public string Value { get; set; }
        
        [XmlIgnore]
        public string SampleOutput
        {
            get
            {
                if (Sample == null) return string.Empty;
                if (Sample == string.Empty) return string.Empty;

                string firstPart = string.Empty;
                string secondPart = string.Empty;
                string[] uSamples = Sample.Split('\n');
                int iLastLineLength = uSamples[uSamples.Length - 1].Length;
                int iValue = 0;
                int iDefault = 0;
                int iDiff = 0;

                switch (SampleMode)
                {
                    case SettingSampleMode.StartAndLength:
                        if (SampleSelectionStart < iLastLineLength && SampleSelectionStart + SampleSelectionLength <= iLastLineLength)
                        {
                            firstPart = new string(' ', SampleSelectionStart);
                            secondPart = new string('¯', SampleSelectionLength);
                        }
                        break;
                    case SettingSampleMode.StartAndValue:
                        try
                        {
                            switch (Type)
                            {
                                case SettingType.Text:
                                    iValue = Value.Length;
                                    if (SampleSelectionStart < iLastLineLength && SampleSelectionStart + iValue <= iLastLineLength)
                                    {
                                        firstPart = new string(' ', SampleSelectionStart);
                                        secondPart = new string('¯', iValue);
                                        if (Value != DefaultValue)
                                        {
                                            try
                                            {
                                                for (int iLine = uSamples.Length - 1; iLine >= 0; iLine--)
                                                {
                                                    if (uSamples[iLine].Length != iLastLineLength + 1 && iLine != uSamples.Length - 1) continue;
                                                    uSamples[iLine] = uSamples[iLine].Remove(SampleSelectionStart, DefaultValue.Length).Insert(SampleSelectionStart, Value);
                                                }
                                            }
                                            catch { }
                                        }
                                    }
                                    break;
                                default: // Number in fact
                                    iValue = Convert.ToInt32(Value);
                                    if (SampleSelectionStart < iLastLineLength)
                                    {
                                        if (SampleSelectionStart + iValue > iLastLineLength) iValue = iLastLineLength - SampleSelectionStart;
                                        firstPart = new string(' ', SampleSelectionStart);
                                        secondPart = new string('¯', iValue);
                                        try
                                        {
                                            iDefault = Convert.ToInt32(DefaultValue);
                                            iDiff = iValue - iDefault;
                                            if (iDiff > 0)
                                            {
                                                for (int iLine = uSamples.Length - 1; iLine >= 0; iLine--)
                                                {
                                                    if (uSamples[iLine].Length != iLastLineLength + 1 && iLine != uSamples.Length - 1) break;
                                                    uSamples[iLine] = uSamples[iLine].Insert(SampleSelectionStart + iDefault, new string(' ', iDiff));
                                                }
                                            }
                                            else if (iDiff < 0)
                                            {
                                                for (int iLine = uSamples.Length - 1; iLine >= 0; iLine--)
                                                {
                                                    if (uSamples[iLine].Length != iLastLineLength + 1 && iLine != uSamples.Length - 1) break;
                                                    uSamples[iLine] = uSamples[iLine].Remove(SampleSelectionStart + iDefault + iDiff, -iDiff);
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                    break;
                            }
                        }
                        catch { }
                        break;
                    case SettingSampleMode.ValueAndLength:
                        try
                        {
                            iValue = Convert.ToInt32(Value);
                            if (iValue < iLastLineLength && iValue + SampleSelectionLength <= iLastLineLength)
                            {
                                firstPart = new string(' ', iValue);
                                secondPart = new string('¯', SampleSelectionLength);
                                try
                                {
                                    iDefault = Convert.ToInt32(DefaultValue);
                                    iDiff = iValue - iDefault;
                                    if (iDiff > 0)
                                    {
                                        for (int iLine = uSamples.Length - 1; iLine >= 0; iLine--)
                                        {
                                            if (uSamples[iLine].Length != iLastLineLength + 1 && iLine != uSamples.Length - 1) break;
                                            uSamples[iLine] = uSamples[iLine].Insert(iDefault, new string(' ', iDiff));
                                        }
                                    }
                                    else if (iDiff < 0)
                                    {
                                        for (int iLine = uSamples.Length - 1; iLine >= 0; iLine--)
                                        {
                                            if (uSamples[iLine].Length != iLastLineLength + 1 && iLine != uSamples.Length - 1) break;
                                            uSamples[iLine] = uSamples[iLine].Remove(iValue, -iDiff);
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                        break;
                    default:
                        return string.Empty;
                }

                return string.Join("\n", uSamples) + "\r\n" + firstPart + secondPart + new string(' ', iLastLineLength - firstPart.Length - secondPart.Length);
            }
        }
        
        public SettingItem()
        {

        }

        public SettingItem(string uniqueId, string uniqueOrder, string label, SettingType type)
        {
            UniqueId = uniqueId;
            UniqueOrder = uniqueOrder;
            Label = label;
            Information = Label;
            Type = type;

            updated = DateTime.UtcNow;
        }

        public override string ToString()
        {
            return Label;
        }

        public int CompareTo(SettingItem oItem)
        {
            if (oItem == null) return 1;
            else return this.UniqueOrder.CompareTo(oItem.UniqueOrder);
        }

        public bool Refresh(SettingItem sItem)
        {
            bool refreshed = false;
            bool differentType = false;

            refreshed |= this.UniqueOrder != sItem.UniqueOrder;
            this.UniqueOrder = sItem.UniqueOrder;

            refreshed |= this.Label != sItem.Label;
            this.Label = sItem.Label;

            refreshed |= this.Information != sItem.Information;
            this.Information = sItem.Information;

            refreshed |= this.Sample != sItem.Sample;
            this.Sample = sItem.Sample;

            refreshed |= this.SampleMode != sItem.SampleMode;
            this.SampleMode = sItem.SampleMode;

            refreshed |= this.SampleSelectionStart != sItem.SampleSelectionStart;
            this.SampleSelectionStart = sItem.SampleSelectionStart;

            refreshed |= this.SampleSelectionLength != sItem.SampleSelectionLength;
            this.SampleSelectionLength = sItem.SampleSelectionLength;

            differentType |= this.Type != sItem.Type;
            refreshed |= differentType;
            this.Type = sItem.Type;

            refreshed |= this.Category != sItem.Category;
            this.Category = sItem.Category;

            refreshed |= this.DefaultValue != sItem.DefaultValue;
            this.DefaultValue = sItem.DefaultValue;

            refreshed |= this.MinValue != sItem.MinValue;
            this.MinValue = sItem.MinValue;

            refreshed |= this.MaxValue != sItem.MaxValue;
            this.MaxValue = sItem.MaxValue;

            if (!differentType)
            {
                if (this.MinValue != null && this.MinValue != string.Empty && this.MaxValue != null && this.MaxValue != string.Empty && this.Value != null && this.Value != string.Empty)
                {
                    try
                    {
                        if (Convert.ToDouble(this.Value) < Convert.ToDouble(this.MinValue) || Convert.ToDouble(this.Value) > Convert.ToDouble(this.MaxValue))
                        {
                            this.Value = DefaultValue;
                            refreshed = true;
                        }
                    }
                    catch
                    {
                        this.Value = DefaultValue;
                        refreshed = true;
                    }
                }
            }

            if (refreshed) updated = DateTime.UtcNow;

            return refreshed;
        }
    }
}
