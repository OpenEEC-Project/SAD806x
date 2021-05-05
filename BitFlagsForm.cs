using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SAD806x
{
    public partial class BitFlagsForm : Form
    {
        private const string TreeRootNodeName = "BITFLAGS";

        private SADS6x S6x = null;
        private S6xScalar s6xScalar = null;
        private S6xRegister s6xReg = null;

        private S6xNavCategories s6xNavCategories = null; 
        
        private SortedList slBitFlags = null;

        private TreeNode currentTreeNode = null;

        private DialogResult closingDialogResult = DialogResult.Cancel;

        public BitFlagsForm(ref SADS6x s6x, ref S6xScalar scalar, ref ImageList stateImageList, ref S6xNavCategories navCategories)
        {
            S6x = s6x;
            s6xScalar = scalar;
            s6xNavCategories = navCategories;

            slBitFlags = new SortedList();

            if (s6xScalar.BitFlags != null) foreach (S6xBitFlag s6xObject in s6xScalar.BitFlags) if (s6xObject != null) slBitFlags.Add(s6xObject.UniqueKey, s6xObject);

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);

            InitializeComponent();

            advElemsTreeView.StateImageList = stateImageList;
        }

        public BitFlagsForm(ref SADS6x s6x, ref S6xRegister register, ref ImageList stateImageList, ref S6xNavCategories navCategories)
        {
            S6x = s6x;
            s6xReg = register;
            s6xNavCategories = navCategories;

            slBitFlags = new SortedList();

            if (s6xReg.BitFlags != null) foreach (S6xBitFlag s6xObject in s6xReg.BitFlags) if (s6xObject != null) slBitFlags.Add(s6xObject.UniqueKey, s6xObject);

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);

            InitializeComponent();

            advElemsTreeView.StateImageList = stateImageList;
        }

        private void BitFlagsForm_Load(object sender, EventArgs e)
        {
            advElemsTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(advElemsTreeView_NodeMouseClick);
            advElemsTreeView.AfterSelect += new TreeViewEventHandler(advElemsTreeView_AfterSelect);

            advElemsContextMenuStrip.Opening += new CancelEventHandler(advElemsContextMenuStrip_Opening);

            bitFlagPositionComboBox.SelectedIndexChanged += new EventHandler(bitFlagPositionComboBox_SelectedIndexChanged);

            sharedIdentificationStatusTrackBar.ValueChanged += new EventHandler(sharedIdentificationStatusTrackBar_ValueChanged);

            S6xNavHeaderCategory headerCateg = S6xNavHeaderCategory.UNDEFINED;
            if (s6xScalar != null)
            {
                advLabelTextBox.Text = s6xScalar.Label;
                advSLabelTextBox.Text = s6xScalar.UniqueAddressHex;
                headerCateg = S6xNavHeaderCategory.SCALARS;
            }
            else if (s6xReg != null)
            {
                advLabelTextBox.Text = s6xReg.Label;
                advSLabelTextBox.Text = s6xReg.Address;
                headerCateg = S6xNavHeaderCategory.REGISTERS;
            }

            sharedCategComboBox.Items.Clear();
            sharedCategComboBox.Items.Add(new S6xNavCategory(string.Empty));
            foreach (S6xNavCategory navCateg in s6xNavCategories.getCategories(headerCateg, S6xNavCategoryLevel.ONE, true).Values) sharedCategComboBox.Items.Add(navCateg);
            sharedCateg2ComboBox.Items.Clear();
            sharedCateg2ComboBox.Items.Add(new S6xNavCategory(string.Empty));
            foreach (S6xNavCategory navCateg in s6xNavCategories.getCategories(headerCateg, S6xNavCategoryLevel.TWO, true).Values) sharedCateg2ComboBox.Items.Add(navCateg);
            sharedCateg3ComboBox.Items.Clear();
            sharedCateg3ComboBox.Items.Add(new S6xNavCategory(string.Empty));
            foreach (S6xNavCategory navCateg in s6xNavCategories.getCategories(headerCateg, S6xNavCategoryLevel.THREE, true).Values) sharedCateg3ComboBox.Items.Add(navCateg);

            Control.ControlCollection controls = null;
            controls = (Control.ControlCollection)bitFlagTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = null;

            loadElemsTreeView();
            if (advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Count == 0) clearElem();
            else advElemsTreeView.SelectedNode = advElemsTreeView.Nodes[TreeRootNodeName].Nodes[0];
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = closingDialogResult;
        }

        private void attachPropertiesEventsControls(ref Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control.GetType().Name)
                {
                    case "TextBox":
                        ((TextBox)control).KeyDown += new KeyEventHandler(textBox_KeyDown);
                        break;
                }
            }
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
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

        private void advElemsTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ((TreeView)sender).SelectedNode = e.Node;
        }

        private void advElemsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            clearElem();

            currentTreeNode = e.Node;

            if (currentTreeNode.Name == TreeRootNodeName) return;

            showElem();
        }

        private void advElemsContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (currentTreeNode == null)
            {
                e.Cancel = true;
                return;
            }

            if (currentTreeNode.Name == TreeRootNodeName)
            {
                createAllToolStripMenuItem.Visible = true;
                newElementToolStripMenuItem.Visible = true;
                delElementToolStripMenuItem.Visible = false;
                removeAllToolStripMenuItem.Visible = true;
                copyXdfToolStripMenuItem.Visible = false;
            }
            else
            {
                createAllToolStripMenuItem.Visible = false;
                newElementToolStripMenuItem.Visible = true;
                delElementToolStripMenuItem.Visible = true;
                removeAllToolStripMenuItem.Visible = false;
                copyXdfToolStripMenuItem.Visible = (s6xScalar != null);
            }
        }

        private void sharedIdentificationStatusTrackBar_ValueChanged(object sender, EventArgs e)
        {
            int iStatus = sharedIdentificationStatusTrackBar.Value;

            sharedIdentificationLabel.Text = string.Format("{0} ({1:d2}%)", sharedIdentificationLabel.Tag, iStatus);
        }

        private void newElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newElem();
        }

        private void delElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            delElem();
        }

        private void createAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            createAll();
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeAll();
        }

        private void copyXdfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyXdf();
        }

        private void elemUpdateButton_Click(object sender, EventArgs e)
        {
            updateElem();
        }

        private void bitFlagPositionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string defaultBitFlagLabel = "B" + bitFlagPositionComboBox.SelectedIndex.ToString();
            if (bitFlagLabelTextBox.Text == string.Empty && bitFlagSLabelTextBox.Text == string.Empty)
            {
                bitFlagLabelTextBox.Text = defaultBitFlagLabel;
                bitFlagSLabelTextBox.Text = defaultBitFlagLabel;
            }
            else if (bitFlagLabelTextBox.Text.StartsWith("B") && bitFlagLabelTextBox.Text == bitFlagSLabelTextBox.Text)
            {
                try
                {
                    int possiblePreviousPosition = Convert.ToInt32(bitFlagLabelTextBox.Text.Substring(1));
                    if (possiblePreviousPosition >= 0 && possiblePreviousPosition < bitFlagPositionComboBox.Items.Count)
                    {
                        bitFlagLabelTextBox.Text = defaultBitFlagLabel;
                        bitFlagSLabelTextBox.Text = defaultBitFlagLabel;
                    }
                }
                catch
                { }
            }
        }

        private void loadElemsTreeView()
        {
            TreeNode tnNode = null;

            foreach (S6xBitFlag s6xObject in slBitFlags.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                if (s6xObject.Label == "B" + s6xObject.Position.ToString()) tnNode.Text = s6xObject.Label;
                else tnNode.Text = "B" + s6xObject.Position.ToString() + " - " + s6xObject.Label;
                tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(s6xObject.IdentificationStatus);
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Add(tnNode);
                tnNode = null;
            }

            advElemsTreeView.ExpandAll();
        }

        private void updateArray(string categ)
        {
            if (categ != TreeRootNodeName) return;

            if (slBitFlags.Count == 0)
            {
                if (s6xScalar != null) s6xScalar.BitFlags = null;
                else if (s6xReg != null) s6xReg.BitFlags = null;
            }
            else
            {
                if (s6xScalar != null)
                {
                    s6xScalar.BitFlags = new S6xBitFlag[slBitFlags.Count];
                    slBitFlags.Values.CopyTo(s6xScalar.BitFlags, 0);
                }
                else if (s6xReg != null)
                {
                    s6xReg.BitFlags = new S6xBitFlag[slBitFlags.Count];
                    slBitFlags.Values.CopyTo(s6xReg.BitFlags, 0);
                }
            }
        }

        private void clearElem()
        {
            foreach (TabPage tabPage in elemTabControl.TabPages)
            {
                Control.ControlCollection controls = (Control.ControlCollection)tabPage.Controls;
                clearControls(ref controls);
            }
        }

        private void clearControls(ref Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                switch (control.GetType().Name)
                {
                    case "TextBox":
                        switch (control.Name)
                        {
                            case "bitFlagSetValueTextBox":
                                ((TextBox)control).Text = "1";
                                break;
                            case "bitFlagNSetValueTextBox":
                                ((TextBox)control).Text = "0";
                                break;
                            case "bitFlagLabelTextBox":
                            case "bitFlagSLabelTextBox":
                                ((TextBox)control).Text = "B0";
                                break;
                            default:
                                ((TextBox)control).Text = string.Empty;
                                break;
                        }
                        break;
                    case "CheckBox":
                        if (((CheckBox)control).ThreeState) ((CheckBox)control).CheckState = CheckState.Indeterminate;
                        else ((CheckBox)control).Checked = false;
                        break;
                    case "ComboBox":
                        ((ComboBox)control).SelectedIndex = 0;
                        // To reset start position.
                        ((ComboBox)control).Tag = null;
                        break;
                    case "DateTimePicker":
                        ((DateTimePicker)control).Value = DateTime.Now;
                        break;
                    case "TrackBar":
                        ((TrackBar)control).Value = 0;
                        break;
                }
            }
        }

        private void showCateg(string categ)
        {
            if (categ != TreeRootNodeName) return;
        }

        private void showElem()
        {
            if (currentTreeNode == null) return;
            if (currentTreeNode.Parent == null) return;

            switch (currentTreeNode.Parent.Name)
            {
                case TreeRootNodeName:
                    S6xBitFlag s6xBitFlag = (S6xBitFlag)slBitFlags[currentTreeNode.Name];
                    bitFlagLabelTextBox.Text = s6xBitFlag.Label;
                    bitFlagSLabelTextBox.Text = s6xBitFlag.ShortLabel;
                    bitFlagSkipCheckBox.Checked = s6xBitFlag.Skip;
                    bitFlagSetValueTextBox.Text = s6xBitFlag.SetValue;
                    bitFlagNSetValueTextBox.Text = s6xBitFlag.NotSetValue;
                    bitFlagHParentCheckBox.Checked = s6xBitFlag.HideParent;

                    // Windows 10 1809 (10.0.17763) Issue
                    bitFlagCommentsTextBox.Clear();
                    bitFlagCommentsTextBox.Multiline = false;
                    bitFlagCommentsTextBox.Multiline = true;
                    
                    bitFlagCommentsTextBox.Text = s6xBitFlag.Comments;
                    bitFlagCommentsTextBox.Text = bitFlagCommentsTextBox.Text.Replace("\n", "\r\n");
                    bitFlagPositionComboBox.SelectedIndex = s6xBitFlag.Position;

                    sharedDateCreatedDateTimePicker.Value = Tools.getValidDateTime(s6xBitFlag.DateCreated, S6x.Properties.DateCreated).ToLocalTime();
                    sharedDateUpdatedDateTimePicker.Value = Tools.getValidDateTime(s6xBitFlag.DateUpdated, S6x.Properties.DateUpdated).ToLocalTime();

                    if (s6xBitFlag.Category == null) sharedCategComboBox.Text = string.Empty;
                    else sharedCategComboBox.Text = s6xBitFlag.Category;
                    if (s6xBitFlag.Category2 == null) sharedCateg2ComboBox.Text = string.Empty;
                    else sharedCateg2ComboBox.Text = s6xBitFlag.Category2;
                    if (s6xBitFlag.Category3 == null) sharedCateg3ComboBox.Text = string.Empty;
                    else sharedCateg3ComboBox.Text = s6xBitFlag.Category3;

                    if (s6xBitFlag.IdentificationStatus < 0) sharedIdentificationStatusTrackBar.Value = 0;
                    else if (s6xBitFlag.IdentificationStatus > 100) sharedIdentificationStatusTrackBar.Value = 100;
                    else sharedIdentificationStatusTrackBar.Value = s6xBitFlag.IdentificationStatus;

                    // Windows 10 1809 (10.0.17763) Issue
                    sharedIdentificationDetailsTextBox.Clear();
                    sharedIdentificationDetailsTextBox.Multiline = false;
                    sharedIdentificationDetailsTextBox.Multiline = true;

                    if (s6xBitFlag.IdentificationDetails == null) sharedIdentificationDetailsTextBox.Text = string.Empty;
                    else sharedIdentificationDetailsTextBox.Text = s6xBitFlag.IdentificationDetails;
                    sharedIdentificationDetailsTextBox.Text = sharedIdentificationDetailsTextBox.Text.Replace("\r\n", "\n").Replace("\n\r", "\n").Replace("\n", "\r\n");

                    // To Keep trace of start position.
                    bitFlagPositionComboBox.Tag = s6xBitFlag.Position;
                    s6xBitFlag = null;

                    elemTabControl.SelectedTab = bitFlagTabPage;
                    break;
                default:
                    return;
            }

            showCateg(currentTreeNode.Parent.Name);
        }

        private void newElem()
        {
            clearElem();

            if (currentTreeNode == null) return;

            switch (currentTreeNode.Name)
            {
                case TreeRootNodeName:
                    showCateg(currentTreeNode.Name);
                    break;
                default:
                    break;
            }
        }

        private void delElem()
        {
            clearElem();

            if (currentTreeNode == null) return;
            if (currentTreeNode.Parent == null) return;

            switch (currentTreeNode.Parent.Name)
            {
                case TreeRootNodeName:
                    slBitFlags.Remove(currentTreeNode.Name);
                    break;
                default:
                    return;
            }

            updateArray(currentTreeNode.Parent.Name);

            closingDialogResult = DialogResult.OK;

            int tnIndex = currentTreeNode.Parent.Nodes.IndexOf(currentTreeNode);
            currentTreeNode.Parent.Nodes.Remove(currentTreeNode);
            if (tnIndex < currentTreeNode.Parent.Nodes.Count) advElemsTreeView.SelectedNode = advElemsTreeView.Nodes[TreeRootNodeName].Nodes[tnIndex];
        }

        private void createAll()
        {
            int topBitFlag = 15;

            if (s6xScalar != null) if (s6xScalar.Byte) topBitFlag = 7;

            for (int iBit = 0; iBit <= topBitFlag; iBit++)
            {
                S6xBitFlag bitFlag = new S6xBitFlag();
                bitFlag.Position = iBit;

                string uniqueKey = bitFlag.UniqueKey;

                if (!slBitFlags.ContainsKey(uniqueKey))
                {
                    slBitFlags.Add(uniqueKey, bitFlag);
                    bitFlag.Label = "B" + iBit.ToString();
                    bitFlag.ShortLabel = bitFlag.Label;
                    bitFlag.SetValue = "1";
                    bitFlag.NotSetValue = "0";

                    bitFlag.DateCreated = DateTime.UtcNow;
                    bitFlag.DateUpdated = DateTime.UtcNow;
                    bitFlag.Category = string.Empty;
                    bitFlag.Category2 = string.Empty;
                    bitFlag.Category3 = string.Empty;
                    bitFlag.IdentificationStatus = 0;
                    bitFlag.IdentificationDetails = string.Empty;

                    updateArray(TreeRootNodeName);

                    TreeNode tnNode = new TreeNode();
                    tnNode.Name = uniqueKey;
                    tnNode.Text = bitFlag.Label;
                    tnNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(bitFlag.IdentificationStatus);
                    advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Add(tnNode);

                    tnNode = null;
                }

                bitFlag = null;
            }

            closingDialogResult = DialogResult.OK;

            advElemsTreeView.ExpandAll();

            if (advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Count > 0) advElemsTreeView.SelectedNode = advElemsTreeView.Nodes[TreeRootNodeName].Nodes[0];
        }

        private void removeAll()
        {
            currentTreeNode = null;

            slBitFlags.Clear();
            updateArray(TreeRootNodeName);

            closingDialogResult = DialogResult.OK;

            advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Clear();

            clearElem();
        }

        private void updateElem()
        {
            string categ = string.Empty;
            string uniqueKey = string.Empty;
            string startUniqueKey = string.Empty;
            string label = string.Empty;
            string comments = string.Empty;

            categ = TreeRootNodeName;

            if (!checkElem(categ))
            {
                MessageBox.Show("Invalid values are present, please correct them to continue.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (currentTreeNode != null)
            {
                if (currentTreeNode.Name == categ) currentTreeNode = null;
            }

            switch (categ)
            {
                case TreeRootNodeName:
                    S6xBitFlag bitFlag = new S6xBitFlag();
                    bitFlag.Position = bitFlagPositionComboBox.SelectedIndex;
                    uniqueKey = bitFlag.UniqueKey;

                    // Start position change mngt
                    if (bitFlagPositionComboBox.Tag != null)
                    {
                        S6xBitFlag startBitFlag = new S6xBitFlag();
                        startBitFlag.Position = (int)bitFlagPositionComboBox.Tag;
                        startUniqueKey = startBitFlag.UniqueKey;
                        startBitFlag = null;
                    }

                    if (slBitFlags.ContainsKey(uniqueKey)) bitFlag = (S6xBitFlag)slBitFlags[uniqueKey];
                    else slBitFlags.Add(uniqueKey, bitFlag);

                    bitFlag.Label = bitFlagLabelTextBox.Text;
                    bitFlag.ShortLabel = bitFlagSLabelTextBox.Text;
                    bitFlag.Skip = bitFlagSkipCheckBox.Checked;
                    bitFlag.SetValue = bitFlagSetValueTextBox.Text;
                    bitFlag.NotSetValue = bitFlagNSetValueTextBox.Text;
                    bitFlag.Comments = bitFlagCommentsTextBox.Text;
                    bitFlag.Position = bitFlagPositionComboBox.SelectedIndex;
                    bitFlag.HideParent = bitFlagHParentCheckBox.Checked;

                    bitFlag.IdentificationStatus = sharedIdentificationStatusTrackBar.Value;
                    bitFlag.IdentificationDetails = sharedIdentificationDetailsTextBox.Text;

                    bitFlag.DateCreated = sharedDateCreatedDateTimePicker.Value.ToUniversalTime();
                    sharedDateUpdatedDateTimePicker.Value = DateTime.Now;
                    bitFlag.DateUpdated = sharedDateUpdatedDateTimePicker.Value.ToUniversalTime();

                    bitFlag.Category = sharedCategComboBox.Text;
                    bitFlag.Category2 = sharedCateg2ComboBox.Text;
                    bitFlag.Category3 = sharedCateg3ComboBox.Text;

                    // Start position change mngt
                    if (startUniqueKey != string.Empty && uniqueKey != startUniqueKey)
                    {
                        if (slBitFlags.ContainsKey(startUniqueKey)) slBitFlags.Remove(startUniqueKey);
                    }

                    label = bitFlag.Label;
                    if (label != "B" + bitFlag.Position.ToString()) label = "B" + bitFlag.Position.ToString() + " - " + label;
                    comments = bitFlag.Comments;

                    bitFlag = null;
                    break;
                default:
                    return;
            }

            if (startUniqueKey != string.Empty && uniqueKey != startUniqueKey)
            {
                if (advElemsTreeView.Nodes[TreeRootNodeName].Nodes.ContainsKey(startUniqueKey)) advElemsTreeView.Nodes[TreeRootNodeName].Nodes.RemoveByKey(startUniqueKey);
            }

            if (uniqueKey != string.Empty && uniqueKey != startUniqueKey)
            {
                updateArray(categ);

                currentTreeNode = new TreeNode();
                currentTreeNode.Name = uniqueKey;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Add(currentTreeNode);
            }

            closingDialogResult = DialogResult.OK;
            
            currentTreeNode.Text = label;
            currentTreeNode.ToolTipText = comments;
            currentTreeNode.StateImageKey = S6xNav.getIdentificationStatusStateImageKey(sharedIdentificationStatusTrackBar.Value);

            clearElem();

            advElemsTreeView.SelectedNode = advElemsTreeView.Nodes[TreeRootNodeName];
        }

        private bool checkElem(string categ)
        {
            bool checkPassed = true;
            string newUniqueKey = string.Empty;

            switch (categ)
            {
                case TreeRootNodeName:
                    checkPassed &= (bitFlagLabelTextBox.Text != string.Empty);
                    checkPassed &= (bitFlagSLabelTextBox.Text != string.Empty);
                    if (checkPassed)
                    {
                        newUniqueKey = string.Format("{0:d2}", bitFlagPositionComboBox.SelectedIndex);
                        if (currentTreeNode == null)
                        {
                            checkPassed &= !advElemsTreeView.Nodes[TreeRootNodeName].Nodes.ContainsKey(newUniqueKey);
                        }
                        else if (currentTreeNode.Name != newUniqueKey)
                        {
                            checkPassed &= !advElemsTreeView.Nodes[TreeRootNodeName].Nodes.ContainsKey(newUniqueKey);
                        }
                        if (!checkPassed) MessageBox.Show("This Bit Flag is already defined.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (checkPassed && s6xScalar != null)
                    {
                        if (s6xScalar.Byte)
                        {
                            checkPassed &= bitFlagPositionComboBox.SelectedIndex <= 7;
                            if (!checkPassed) MessageBox.Show("Bit Flag is not available for this Scalar.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    return checkPassed;
                default:
                    return false;
            }
        }

        private void copyXdf()
        {
            if (currentTreeNode == null) return;
            if (currentTreeNode.Parent == null) return;

            switch (currentTreeNode.Parent.Name)
            {
                case TreeRootNodeName:
                    break;
                default:
                    return;
            }

            object xdfObject = null;

            if (s6xScalar.AddressBinInt >= S6x.Properties.XdfBaseOffsetInt)
            {
                XdfHeaderCategory[] defaultElementHeaderCategories = Tools.XDFDefaultElementHeaderCategories(new string[] { ((S6xBitFlag)slBitFlags[currentTreeNode.Name]).Category, ((S6xBitFlag)slBitFlags[currentTreeNode.Name]).Category2, ((S6xBitFlag)slBitFlags[currentTreeNode.Name]).Category3 });
                xdfObject = new XdfFlag(s6xScalar, (S6xBitFlag)slBitFlags[currentTreeNode.Name], S6x.Properties.XdfBaseOffsetInt, defaultElementHeaderCategories);
            }

            MemoryStream mStr = new MemoryStream();
            ToolsXml.SerializeXdf(ref mStr, xdfObject);
            Clipboard.SetData(SADDef.XdfClipboardFormat, mStr);
            mStr.Close();
            mStr = null;
            xdfObject = null;
        }
    }
}
