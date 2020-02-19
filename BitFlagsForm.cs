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

        private SortedList slBitFlags = null;

        private TreeNode currentTreeNode = null;

        public BitFlagsForm(ref SADS6x s6x, ref S6xScalar scalar)
        {
            S6x = s6x;
            s6xScalar = scalar;

            slBitFlags = new SortedList();

            if (s6xScalar.BitFlags != null) foreach (S6xBitFlag s6xObject in s6xScalar.BitFlags) if (s6xObject != null) slBitFlags.Add(s6xObject.UniqueKey, s6xObject);

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            InitializeComponent();
        }

        public BitFlagsForm(ref SADS6x s6x, ref S6xRegister register)
        {
            S6x = s6x;
            s6xReg = register;

            slBitFlags = new SortedList();

            if (s6xReg.BitFlags != null) foreach (S6xBitFlag s6xObject in s6xReg.BitFlags) if (s6xObject != null) slBitFlags.Add(s6xObject.UniqueKey, s6xObject);

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            InitializeComponent();
        }

        private void BitFlagsForm_Load(object sender, EventArgs e)
        {
            advElemsTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(advElemsTreeView_NodeMouseClick);
            advElemsTreeView.AfterSelect += new TreeViewEventHandler(advElemsTreeView_AfterSelect);

            advElemsContextMenuStrip.Opening += new CancelEventHandler(advElemsContextMenuStrip_Opening);

            if (s6xScalar != null)
            {
                advLabelTextBox.Text = s6xScalar.Label;
                advSLabelTextBox.Text = s6xScalar.UniqueAddressHex;
            }
            else if (s6xReg != null)
            {
                advLabelTextBox.Text = s6xReg.Label;
                advSLabelTextBox.Text = s6xReg.Address;
            }

            Control.ControlCollection controls = null;
            controls = (Control.ControlCollection)bitFlagTabPage.Controls;
            attachPropertiesEventsControls(ref controls);
            controls = null;

            loadElemsTreeView();
            if (advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Count == 0) clearElem();
            else advElemsTreeView.SelectedNode = advElemsTreeView.Nodes[TreeRootNodeName].Nodes[0];
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
                copyXdfToolStripMenuItem.Visible = false;
            }
            else
            {
                createAllToolStripMenuItem.Visible = false;
                newElementToolStripMenuItem.Visible = false;
                delElementToolStripMenuItem.Visible = true;
                copyXdfToolStripMenuItem.Visible = (s6xScalar != null);
            }
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

        private void loadElemsTreeView()
        {
            TreeNode tnNode = null;

            foreach (S6xBitFlag s6xObject in slBitFlags.Values)
            {
                tnNode = new TreeNode();
                tnNode.Name = s6xObject.UniqueKey;
                if (s6xObject.Label == "B" + s6xObject.Position.ToString()) tnNode.Text = s6xObject.Label;
                else tnNode.Text = "B" + s6xObject.Position.ToString() + " - " + s6xObject.Label;
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
                    s6xBitFlag = null;
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

                    updateArray(TreeRootNodeName);

                    TreeNode tnNode = new TreeNode();
                    tnNode.Name = uniqueKey;
                    tnNode.Text = bitFlag.Label;
                    advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Add(tnNode);

                    tnNode = null;
                }

                bitFlag = null;
            }

            advElemsTreeView.ExpandAll();

            if (advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Count > 0) advElemsTreeView.SelectedNode = advElemsTreeView.Nodes[TreeRootNodeName].Nodes[0];
        }

        private void removeAll()
        {
            currentTreeNode = null;

            slBitFlags.Clear();
            updateArray(TreeRootNodeName);

            advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Clear();

            clearElem();
        }

        private void updateElem()
        {
            string categ = string.Empty;
            string uniqueKey = string.Empty;
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
                    S6xBitFlag bitFlag = null;
                    if (currentTreeNode == null)
                    {
                        bitFlag = new S6xBitFlag();
                        bitFlag.Position = bitFlagPositionComboBox.SelectedIndex;
                        if (slBitFlags.ContainsKey(bitFlag.UniqueKey)) bitFlag = (S6xBitFlag)slBitFlags[bitFlag.UniqueKey];
                        else slBitFlags.Add(bitFlag.UniqueKey, bitFlag);
                    }
                    else
                    {
                        bitFlag = (S6xBitFlag)slBitFlags[currentTreeNode.Name];
                        bitFlag.Position = bitFlagPositionComboBox.SelectedIndex;

                        if (currentTreeNode.Name != bitFlag.UniqueKey)
                        {
                            slBitFlags.Remove(currentTreeNode.Name);
                            if (slBitFlags.ContainsKey(bitFlag.UniqueKey)) slBitFlags[bitFlag.UniqueKey] = bitFlag;
                            else slBitFlags.Add(bitFlag.UniqueKey, bitFlag);
                        }
                    }
                    bitFlag.Label = bitFlagLabelTextBox.Text;
                    bitFlag.ShortLabel = bitFlagSLabelTextBox.Text;
                    bitFlag.Skip = bitFlagSkipCheckBox.Checked;
                    bitFlag.SetValue = bitFlagSetValueTextBox.Text;
                    bitFlag.NotSetValue = bitFlagNSetValueTextBox.Text;
                    bitFlag.Comments = bitFlagCommentsTextBox.Text;
                    bitFlag.Position = bitFlagPositionComboBox.SelectedIndex;
                    bitFlag.HideParent = bitFlagHParentCheckBox.Checked;


                    uniqueKey = bitFlag.UniqueKey;
                    label = bitFlag.Label;
                    if (label != "B" + bitFlag.Position.ToString()) label = "B" + bitFlag.Position.ToString() + " - " + label;
                    comments = bitFlag.Comments;

                    bitFlag = null;
                    break;
                default:
                    return;
            }

            if (currentTreeNode != null)
            {
                if (currentTreeNode.Name != uniqueKey)
                {
                    advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Remove(currentTreeNode);
                    currentTreeNode = null;
                }
            }

            if (currentTreeNode == null)
            {
                updateArray(categ);

                currentTreeNode = new TreeNode();
                currentTreeNode.Name = uniqueKey;
                advElemsTreeView.Nodes[TreeRootNodeName].Nodes.Add(currentTreeNode);
            }
            currentTreeNode.Text = label;
            currentTreeNode.ToolTipText = comments;

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
                xdfObject = new XdfFlag(s6xScalar, (S6xBitFlag)slBitFlags[currentTreeNode.Name], S6x.Properties.XdfBaseOffsetInt);
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
