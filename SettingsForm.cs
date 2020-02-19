using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SAD806x
{
    public partial class SettingsForm : Form
    {
        private string settingsFilePath = string.Empty;
        private string specificType = string.Empty;
        private SettingsLst setSettings = null;
        private string[] setCategories = null;
        private SettingItem selectedSetting = null;
        private BindingSource bindingSource = null;

        public SettingsForm(string title, string filePath, string specType)
        {
            InitializeComponent();

            this.Text = title;
            settingsFilePath = filePath;
            specificType = specType;

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            setSettings = (SettingsLst)ToolsXml.DeserializeFile(settingsFilePath, typeof(SettingsLst));
            if (setSettings == null) setSettings = new SettingsLst();
            setSettings.isSaved = true;

            ToolsSettings.Update(ref setSettings, specType);

            bindingSource = new BindingSource();
            bindingSource.DataSource = setSettings.GetOrderedItems();

            settingsListBox.DisplayMember = "Label";
            settingsListBox.DataSource = bindingSource;

            setCategories = setSettings.GetCategs();
            categComboBox.DataSource = setCategories;
            categComboBox.SelectedIndex = 0;
            
            this.FormClosing += new FormClosingEventHandler(SettingsForm_FormClosing);

            searchTextBox.KeyPress += new KeyPressEventHandler(searchTextBox_KeyPress);
            categComboBox.SelectedIndexChanged += new EventHandler(categComboBox_SelectedIndexChanged);
            settingsListBox.MouseMove += new MouseEventHandler(settingsListBox_MouseMove);
            settingsListBox.MouseDown += new MouseEventHandler(settingsListBox_MouseDown);
            settingsListBox.MouseClick += new MouseEventHandler(settingsListBox_MouseClick);
            settingsListBox.SelectedIndexChanged += new EventHandler(settingsListBox_SelectedIndexChanged);

            settingsListBox.SelectedItem = null;
            settingsListBox_SelectedIndexChanged(settingsListBox, new EventArgs());
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!setSettings.isSaved)
            {
                if (MessageBox.Show("Settings file is not saved, continue ?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }

            if (!e.Cancel)
            {
                Dispose();

                GC.Collect();
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!setSettings.Save(settingsFilePath)) throw new Exception();

                informationTextBox.Modified = false;
            }
            catch
            {
                MessageBox.Show("Saving settings has failed.\r\n\r\nPlease check related file access.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)13) return;
            if (bindingSource.DataSource == null) return;

            string search = searchTextBox.Text.ToUpper();

            List<SettingItem> lItems = new List<SettingItem>();

            switch (categComboBox.SelectedIndex)
            {
                case 0:
                    foreach (SettingItem sItem in setSettings.GetOrderedItems())
                    {
                        if (sItem.Label.ToUpper().Contains(search)) lItems.Add(sItem);
                    }
                    break;
                default:
                    foreach (SettingItem sItem in setSettings.GetCategOrderedItems(categComboBox.SelectedItem.ToString()))
                    {
                        if (sItem.Label.ToUpper().Contains(search)) lItems.Add(sItem);
                    }
                    break;
            }
            settingsListBox.SelectedItem = null;
            bindingSource.DataSource = lItems;
        }

        private void categComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (categComboBox.SelectedIndex)
            {
                case 0:
                    bindingSource.DataSource = setSettings.Items;
                    break;
                default:
                    bindingSource.DataSource = setSettings.GetCategOrderedItems(categComboBox.SelectedItem.ToString());
                    break;
            }
        }

        private void settingsListBox_MouseMove(object sender, MouseEventArgs e)
        {
            int itemIndex = settingsListBox.IndexFromPoint(e.Location);
            if (itemIndex < 0) return;

            string newTip = ((SettingItem)settingsListBox.Items[itemIndex]).Label;
            if (settingsToolTip.GetToolTip(settingsListBox) != newTip) settingsToolTip.SetToolTip(settingsListBox, newTip);
        }

        private void settingsListBox_MouseDown(object sender, MouseEventArgs e)
        {
            int itemIndex = settingsListBox.IndexFromPoint(e.Location);
            if (itemIndex < 0)
            {
                selectedSetting = null;
                return;
            }

            settingsListBox.SelectedIndex = itemIndex;
        }

        private void settingsListBox_MouseClick(object sender, MouseEventArgs e)
        {
            int itemIndex = settingsListBox.IndexFromPoint(e.Location);
            if (itemIndex < 0)
            {
                selectedSetting = null;
                return;
            }

            settingsListBox.SelectedIndex = itemIndex;
        }

        private void CheckBoxBinding_Format(object sender, ConvertEventArgs e)
        {
            switch (e.Value.ToString())
            {
                case "true":
                    e.Value = true;
                    break;
                default:
                    e.Value = false;
                    break;
            }
        }

        private void CheckBoxBinding_Parse(object sender, ConvertEventArgs e)
        {
            if ((bool)e.Value) e.Value = "true";
            else e.Value = "false";
        }

        private void addDetail(string description, string selectedSettingProperty, ref SettingItem sSetting)
        {
            Panel pPanel = null;
            Label lLabel = null;
            int panelHeight = -1;

            pPanel = new Panel();
            pPanel.Dock = DockStyle.Top;

            if (selectedSettingProperty != string.Empty)
            {
                switch (sSetting.Type)
                {
                    case SettingType.Boolean:
                        Binding cbBinding = new Binding("Checked", sSetting, selectedSettingProperty);
                        cbBinding.DataSourceUpdateMode = DataSourceUpdateMode.Never;
                        cbBinding.Format += new ConvertEventHandler(CheckBoxBinding_Format);
                        cbBinding.Parse += new ConvertEventHandler(CheckBoxBinding_Parse);
                        CheckBox cCheckBox = new CheckBox();
                        cCheckBox.Enabled = false;
                        cCheckBox.CheckAlign = ContentAlignment.TopLeft;
                        cCheckBox.AutoSize = true;
                        cCheckBox.DataBindings.Add(cbBinding);
                        cCheckBox.Dock = DockStyle.Left;
                        panelHeight = cCheckBox.Height;
                        pPanel.Controls.Add(cCheckBox);
                        break;
                    default:
                        lLabel = new Label();
                        lLabel.AutoSize = true;
                        lLabel.DataBindings.Add("Text", sSetting, selectedSettingProperty, false, DataSourceUpdateMode.Never);
                        lLabel.Dock = DockStyle.Left;
                        panelHeight = lLabel.Height;
                        pPanel.Controls.Add(lLabel);
                        break;
                }
            }

            if (description != string.Empty)
            {
                lLabel = new Label();
                lLabel.AutoSize = true;
                lLabel.Text = description;
                lLabel.Dock = DockStyle.Left;
                pPanel.Controls.Add(lLabel);
                if (lLabel.Height > panelHeight) panelHeight = lLabel.Height;
            }

            pPanel.Height = panelHeight;

            settingValuePanel.Controls.Add(pPanel);
        }

        private void addSample(ref SettingItem sSetting)
        {
            RichTextBox rTextBox = new RichTextBox();
            rTextBox.Name = "SettingSampleRichTextBox";
            rTextBox.ReadOnly = true;
            rTextBox.WordWrap = false;
            rTextBox.Height = 150;
            rTextBox.Font = new Font(FontFamily.GenericMonospace, rTextBox.Font.Size);
            rTextBox.DataBindings.Add("Text", sSetting, "SampleOutput", false, DataSourceUpdateMode.Never);
            rTextBox.Dock = DockStyle.Top;
            settingValuePanel.Controls.Add(rTextBox);
        }

        private void addValue(ref SettingItem sSetting)
        {
            switch (sSetting.Type)
            {
                case SettingType.Boolean:
                    Binding cbBinding = new Binding("Checked", sSetting, "Value");
                    cbBinding.DataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged;
                    cbBinding.Format += new ConvertEventHandler(CheckBoxBinding_Format);
                    cbBinding.Parse += new ConvertEventHandler(CheckBoxBinding_Parse);

                    CheckBox cCheckBox = new CheckBox();
                    cCheckBox.DataBindings.Add(cbBinding);
                    cCheckBox.CheckAlign = ContentAlignment.TopCenter;
                    cCheckBox.Dock = DockStyle.Top;
                    settingValuePanel.Controls.Add(cCheckBox);

                    cCheckBox.CheckedChanged += new EventHandler(SettingValueCheckBox_CheckedChanged);
                    break;
                case SettingType.Number:
                    NumericUpDown sNumUD = new NumericUpDown();
                    sNumUD.DataBindings.Add("Minimum", sSetting, "MinValue", false, DataSourceUpdateMode.Never);
                    sNumUD.DataBindings.Add("Maximum", sSetting, "MaxValue", false, DataSourceUpdateMode.Never);
                    sNumUD.DataBindings.Add("Value", sSetting, "Value", false, DataSourceUpdateMode.OnPropertyChanged);
                    sNumUD.Dock = DockStyle.Top;
                    settingValuePanel.Controls.Add(sNumUD);

                    sNumUD.ValueChanged += new EventHandler(SettingValueNumericUpDown_ValueChanged);
                    break;
                default:
                    TextBox sTextBox = new TextBox();
                    if (sSetting.Type == SettingType.LargeText)
                    {
                        sTextBox.Multiline = true;
                        sTextBox.Height = sTextBox.Height * 5;
                    }
                    sTextBox.DataBindings.Add("Text", sSetting, "Value", false, DataSourceUpdateMode.OnValidation);
                    sTextBox.Dock = DockStyle.Top;
                    settingValuePanel.Controls.Add(sTextBox);

                    sTextBox.TextChanged += new EventHandler(SettingValueTextBox_TextChanged);
                    sTextBox.ModifiedChanged += new EventHandler(SettingValueTextBox_TextChanged);
                    sTextBox.KeyDown += new KeyEventHandler(SettingValueTextBox_KeyDown);
                    sTextBox.Leave += new EventHandler(SettingValueTextBox_Leave);
                    break;
            }
        }

        private void settingsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedSetting = (SettingItem)settingsListBox.SelectedItem;

            informationTextBox.DataBindings.Clear();

            detailsPanel.Visible = (selectedSetting != null);

            if (selectedSetting == null) return;

            try
            {
                informationTextBox.DataBindings.Add("Text", selectedSetting, "Information", false, DataSourceUpdateMode.Never);

                settingValuePanel.Controls.Clear();

                if (selectedSetting.MaxValue != null && selectedSetting.MaxValue != string.Empty)
                {
                    addDetail("Maximum value: ", "MaxValue", ref selectedSetting);
                }

                if (selectedSetting.MinValue != null && selectedSetting.MinValue != string.Empty)
                {
                    addDetail("Minimum value: ", "MinValue", ref selectedSetting);
                }

                if (selectedSetting.DefaultValue != null && selectedSetting.DefaultValue != string.Empty)
                {
                    addDetail("Default value: ", "DefaultValue", ref selectedSetting);
                }

                addDetail(" ", string.Empty, ref selectedSetting);

                addValue(ref selectedSetting);

                addDetail(" ", string.Empty, ref selectedSetting);

                if (selectedSetting.Sample != null && selectedSetting.Sample != string.Empty)
                {
                    addSample(ref selectedSetting);
                    addDetail(" ", string.Empty, ref selectedSetting);
                }
            }
            catch
            {
            }
        }

        private void SettingValueNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            setSettings.isSaved = false;

            if (selectedSetting != null) selectedSetting.updated = DateTime.UtcNow;
        }

        private void SettingValueCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            setSettings.isSaved = false;

            if (selectedSetting != null) selectedSetting.updated = DateTime.UtcNow;
        }

        private void SettingValueTextBox_TextChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).Modified)
            {
                setSettings.isSaved = false;

                if (selectedSetting != null) selectedSetting.updated = DateTime.UtcNow;

                bindingSource.ResetBindings(false);
            }
        }

        private void SettingValueTextBox_Leave(object sender, EventArgs e)
        {
            SettingValueTextBox_TextChanged(sender, e);
        }

        private void SettingValueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    try { ((ContainerControl)((TextBox)sender).TopLevelControl).Validate(); }
                    catch { }
                    break;
                // UpperCase / LowerCase
                case Keys.U:
                    if (e.Control && e.Shift)
                    {
                        if (((TextBox)sender).SelectedText != string.Empty) ((TextBox)sender).SelectedText = ((TextBox)sender).SelectedText.ToUpper();
                    }
                    else if (e.Control)
                    {
                        if (((TextBox)sender).SelectedText != string.Empty) ((TextBox)sender).SelectedText = ((TextBox)sender).SelectedText.ToLower();
                    }
                    break;
            }
        }
    }
}
