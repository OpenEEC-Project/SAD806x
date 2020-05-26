using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAD806x
{
    public partial class SearchSignatureForm : Form
    {
        private SADBin sadBin = null;

        public SearchSignatureForm(ref SADBin mainSadBin)
        {
            sadBin = mainSadBin;

            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            //searchTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(searchTreeView_NodeMouseClick);
            //searchTreeView.AfterSelect += new TreeViewEventHandler(searchTreeView_AfterSelect);

            mainTipPictureBox.Tag = SharedUI.ElementSignatureTip();
            mainTipPictureBox.MouseHover += new EventHandler(TipPictureBox_MouseHover);
            mainTipPictureBox.Click += new EventHandler(TipPictureBox_Click);
        }

        private void TipPictureBox_MouseHover(object sender, EventArgs e)
        {
            if (((Control)sender).Tag == null) return;

            mainToolTip.SetToolTip((Control)sender, ((Control)sender).Tag.ToString());
        }

        private void TipPictureBox_Click(object sender, EventArgs e)
        {
            if (((Control)sender).Tag == null) return;

            MessageBox.Show(((PictureBox)sender).Tag.ToString(), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /*
        private void searchTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ((TreeView)sender).SelectedNode = e.Node;
        }

        private void searchTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null) return;
            try { elemsTreeView.SelectedNode = elemsTreeView.Nodes[e.Node.Parent.Name].Nodes[e.Node.Name]; }
            catch { }
        }
        */

        private void searchButton_Click(object sender, EventArgs e)
        {
            Search();
        }

        private void Search()
        {
            if (sadBin == null) return;

            bool invalidSignature = false;
            SortedList slResult = new SortedList();
            string cleanedSignature = searchTextBox.Text;

            searchTreeViewInit();

            if (cleanedSignature != null && cleanedSignature != string.Empty)
            {
                //Signature Parameters management
                while (cleanedSignature.Contains(SADDef.SignatureParamBytePrefixSuffix))
                {
                    // Parameter is always #XX, a group of 1 byte (..) is created in regular expression to get it later on
                    int startPos = -1;
                    int length = -1;
                    startPos = cleanedSignature.IndexOf(SADDef.SignatureParamBytePrefixSuffix);
                    if (startPos <= cleanedSignature.Length - 2)
                    {
                        if (cleanedSignature.Substring(startPos + 1).Contains(SADDef.SignatureParamBytePrefixSuffix))
                        {
                            length = cleanedSignature.Substring(startPos + 1).IndexOf(SADDef.SignatureParamBytePrefixSuffix) + 2;
                        }
                    }
                    if (startPos >= 0 && length > 0)
                    {
                        string sParam = cleanedSignature.Substring(startPos, length);
                        cleanedSignature = cleanedSignature.Replace(sParam, "(..)");
                    }
                    else if (startPos > 0)
                    {
                        cleanedSignature = cleanedSignature.Replace(SADDef.SignatureParamBytePrefixSuffix, "Z");
                    }
                }

                try
                {
                    SADBank sadBank = sadBin.Bank8;
                    while (sadBank != null)
                    {
                        object[] oMatches = sadBank.getBytesFromSignature(cleanedSignature);
                        foreach (object[] oMatch in oMatches)
                        {
                            int matchingStartAddress = (int)oMatch[0];
                            string matchingBytes = oMatch[1].ToString();
                            string[] matchingParams = (string[])oMatch[2];

                            if (!slResult.ContainsKey(Tools.UniqueAddress(sadBank.Num, matchingStartAddress))) slResult.Add(Tools.UniqueAddress(sadBank.Num, matchingStartAddress), new int[] { sadBank.Num, matchingStartAddress });
                        }
                        oMatches = null;

                        if (sadBank.Num == 0) sadBank = null;
                        else if (sadBank.Num == 9) sadBank = sadBin.Bank0;
                        else if (sadBank.Num == 1) sadBank = sadBin.Bank9;
                        else if (sadBank.Num == 8) sadBank = sadBin.Bank1;
                    }
                }
                catch
                {
                    invalidSignature = true;
                }
            }
            
            foreach (int[] arrMatches in slResult.Values)
            {
                TreeNode parentNode = searchTreeView.Nodes[arrMatches[0].ToString()];
                if (parentNode == null)
                {
                    parentNode = new TreeNode();
                    parentNode.Name = arrMatches[0].ToString();
                    parentNode.Text = "Bank " + parentNode.Name;
                    parentNode.ToolTipText = parentNode.Text;
                    searchTreeView.Nodes.Add(parentNode);
                }

                string uniqueAddress = Tools.UniqueAddress(arrMatches[0], arrMatches[1]);
                if (parentNode.Nodes.ContainsKey(uniqueAddress)) continue;

                string address = string.Format("{0:x4}", SADDef.EecBankStartAddress + arrMatches[1]);

                TreeNode tnNode = new TreeNode();
                tnNode.Name = uniqueAddress;
                tnNode.Text = address;
                tnNode.ToolTipText = address;
                parentNode.Nodes.Add(tnNode);
            }

            searchTreeViewCount();

            if (invalidSignature) MessageBox.Show("Signature is not valid.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void searchTreeViewInit()
        {
            searchTreeView.Nodes.Clear();
        }

        private void searchTreeViewCount()
        {
            foreach (TreeNode tnParent in searchTreeView.Nodes)
            {
                tnParent.Text = "Bank " + tnParent.Name + " (" + tnParent.Nodes.Count.ToString() + ")";
            }
        }
    }
}
