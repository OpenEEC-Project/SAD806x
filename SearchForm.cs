using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SAD806x
{
    public partial class SearchForm : Form
    {
        private SADS6x sadS6x = null;
        private TreeView elemsTreeView = null;

        private System.Windows.Forms.Timer mainUpdateTimer = null;
        
        public SearchForm(ref SADS6x mainSadS6x, ref TreeView mainElemsTreeView)
        {
            sadS6x = mainSadS6x;
            elemsTreeView = mainElemsTreeView;
            
            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.FormClosing += new FormClosingEventHandler(SearchForm_FormClosing);

            searchTextBox.KeyPress += new KeyPressEventHandler(searchTextBox_KeyPress);
            searchTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(searchTreeView_NodeMouseClick);
            searchTreeView.AfterSelect += new TreeViewEventHandler(searchTreeView_AfterSelect);

            mainUpdateTimer = new System.Windows.Forms.Timer();
            mainUpdateTimer.Enabled = false;
            mainUpdateTimer.Interval = 100;
            mainUpdateTimer.Tick += new EventHandler(mainUpdateTimer_Tick);

        }

        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainUpdateTimer.Enabled = false;
        }

        private void mainUpdateTimer_Tick(object sender, EventArgs e)
        {
            mainUpdateTimer.Enabled = false;

            try
            {
                bool refreshCount = false;

                foreach (TreeNode tnCateg in searchTreeView.Nodes)
                {
                    S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[tnCateg.Name]);
                    if (!niMFHeaderCateg.isValid)
                    {
                        tnCateg.Nodes.Clear();
                        continue;
                    }

                    List<TreeNode> lsRemoval = new List<TreeNode>();

                    foreach (TreeNode tnNode in tnCateg.Nodes)
                    {
                        TreeNode tnMainNode = niMFHeaderCateg.FindElement(tnNode.Name);
                        if (tnMainNode == null)
                        {
                            lsRemoval.Add(tnNode);
                            continue;
                        }
                        if (tnMainNode.Text != tnNode.Text) tnNode.Text = tnMainNode.Text;
                        if (tnMainNode.ToolTipText != tnNode.ToolTipText) tnNode.ToolTipText = tnMainNode.ToolTipText;
                    }

                    foreach (TreeNode tnNode in lsRemoval) tnCateg.Nodes.Remove(tnNode);
                    refreshCount |= lsRemoval.Count > 0;
                }

                if (refreshCount) searchTreeViewCount();
                mainUpdateTimer.Enabled = true;
            }
            catch { }
        }

        private void searchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)13:
                    searchButton_Click(sender, e);
                    break;
            }
        }

        private void searchTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            ((TreeView)sender).SelectedNode = e.Node;
        }

        private void searchTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent == null) return;
            S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[e.Node.Parent.Name]);
            if (!niMFHeaderCateg.isValid) return;
            TreeNode tnMFNode = niMFHeaderCateg.FindElement(e.Node.Name);
            if (tnMFNode == null) return;
            try { elemsTreeView.SelectedNode = tnMFNode; }
            catch { }
            tnMFNode = null;
            niMFHeaderCateg = null;
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            Search(null);
        }

        private void routinesWithArgumentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search("ARGSROUTINES");
        }

        private void routinesAdvancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Search("ADVROUTINES");
        }

        private void Search(string Mode)
        {
            if (sadS6x == null) return;
            if (elemsTreeView == null) return;

            List<string[]> results = null;
            
            switch (Mode)
            {
                case "ARGSROUTINES":
                    results = SearchArgsRoutines();
                    break;
                case "ADVROUTINES":
                    results = SearchAdvancedRoutines();
                    break;
                default:
                    if (searchTextBox.Text == string.Empty) return;
                    results = SearchText(searchTextBox.Text.ToLower());
                    break;
            }

            mainUpdateTimer.Enabled = false; 
            searchTreeViewInit();

            foreach (string[] result in results)
            {
                S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[result[0]]);
                if (!niMFHeaderCateg.isValid) continue;

                TreeNode parentNode = searchTreeView.Nodes[niMFHeaderCateg.HeaderCategoryName];
                if (parentNode == null) continue;

                TreeNode tnMFNode = niMFHeaderCateg.FindElement(result[1]);
                if (tnMFNode == null) continue;
                if (parentNode.Nodes.ContainsKey(result[1])) continue;

                TreeNode tnNode = new TreeNode();
                tnNode.Name = tnMFNode.Name;
                tnNode.Text = tnMFNode.Text;
                tnNode.ToolTipText = tnMFNode.ToolTipText;
                parentNode.Nodes.Add(tnNode);
            }

            searchTreeViewCount();
            mainUpdateTimer.Enabled = true;
        }

        private List<string[]> SearchText(string searchText)
        {
            List<string[]> results = new List<string[]>();

            foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), s6xObject.UniqueAddress });
            }
            foreach (S6xOperation s6xObject in sadS6x.slOperations.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OPERATIONS), s6xObject.UniqueAddress });
            }
            foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.Comments, s6xObject.Information };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OTHER), s6xObject.UniqueAddress });
            }
            foreach (S6xRegister s6xObject in sadS6x.slRegisters.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ByteLabel, s6xObject.WordLabel, s6xObject.Comments, s6xObject.Information };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                // BitFlags AddOn
                if (!isResult && s6xObject.BitFlagsNum > 0)
                {
                    foreach (S6xBitFlag s6xBF in s6xObject.BitFlags)
                    {
                        searchValues = new string[] { s6xBF.ShortLabel, s6xBF.Label, s6xBF.SetValue, s6xBF.NotSetValue, s6xBF.Comments };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS), s6xObject.UniqueAddress });
            }
            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), s6xObject.UniqueAddress });
            }
            foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                // BitFlags AddOn
                if (!isResult && s6xObject.BitFlagsNum > 0)
                {
                    foreach (S6xBitFlag s6xBF in s6xObject.BitFlags)
                    {
                        searchValues = new string[] { s6xBF.ShortLabel, s6xBF.Label, s6xBF.SetValue, s6xBF.NotSetValue, s6xBF.Comments };
                        foreach (string searchValue in searchValues)
                        {
                            string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                            if (cSearchValue.Contains(searchText))
                            {
                                isResult = true;
                                break;
                            }
                        }
                        if (isResult) break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), s6xObject.UniqueAddress });
            }
            foreach (S6xSignature s6xObject in sadS6x.slSignatures.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.SignatureLabel, s6xObject.SignatureCategory, s6xObject.SignatureComments, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                // TO BE EXTENDED
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SIGNATURES), s6xObject.UniqueKey });
            }
            foreach (S6xElementSignature s6xObject in sadS6x.slElementsSignatures.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.SignatureLabel, s6xObject.SignatureKey, s6xObject.SignatureCategory, s6xObject.SignatureComments, s6xObject.Information, string.Empty, string.Empty, string.Empty };
                if (s6xObject.Scalar != null)
                {
                    searchValues[5] = s6xObject.Scalar.Label;
                    searchValues[6] = s6xObject.Scalar.ShortLabel;
                    searchValues[7] = s6xObject.Scalar.Comments;
                }
                else if (s6xObject.Function != null)
                {
                    searchValues[5] = s6xObject.Function.Label;
                    searchValues[6] = s6xObject.Function.ShortLabel;
                    searchValues[7] = s6xObject.Function.Comments;
                }
                else if (s6xObject.Table != null)
                {
                    searchValues[5] = s6xObject.Table.Label;
                    searchValues[6] = s6xObject.Table.ShortLabel;
                    searchValues[7] = s6xObject.Table.Comments;
                }
                else if (s6xObject.Structure != null)
                {
                    searchValues[5] = s6xObject.Structure.Label;
                    searchValues[6] = s6xObject.Structure.ShortLabel;
                    searchValues[7] = s6xObject.Structure.Comments;
                }
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                // BitFlags AddOn
                if (!isResult && s6xObject.Scalar != null)
                {
                    if (s6xObject.Scalar.BitFlagsNum > 0)
                    {
                        foreach (S6xBitFlag s6xBF in s6xObject.Scalar.BitFlags)
                        {
                            searchValues = new string[] { s6xBF.ShortLabel, s6xBF.Label, s6xBF.SetValue, s6xBF.NotSetValue, s6xBF.Comments };
                            foreach (string searchValue in searchValues)
                            {
                                string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                                if (cSearchValue.Contains(searchText))
                                {
                                    isResult = true;
                                    break;
                                }
                            }
                            if (isResult) break;
                        }
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ELEMSSIGNATURES), s6xObject.UniqueKey });
            }
            foreach (S6xStructure s6xObject in sadS6x.slStructures.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), s6xObject.UniqueAddress });
            }
            foreach (S6xTable s6xObject in sadS6x.slTables.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments, s6xObject.Information };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), s6xObject.UniqueAddress });
            }

            return results;
        }

        private List<string[]> SearchArgsRoutines()
        {
            List<string[]> results = new List<string[]>();

            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                if (s6xObject.ByteArgumentsNum > 0) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), s6xObject.UniqueAddress });
            }

            return results;
        }

        private List<string[]> SearchAdvancedRoutines()
        {
            List<string[]> results = new List<string[]>();

            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                if (s6xObject.isAdvanced) results.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), s6xObject.UniqueAddress });
            }

            return results;
        }

        private void searchTreeViewInit()
        {
            searchTreeView.Nodes.Clear();

            foreach (TreeNode tnMainParent in elemsTreeView.Nodes)
            {
                switch (S6xNav.getHeaderCateg(tnMainParent.Name))
                {
                    case S6xNavHeaderCategory.PROPERTIES:
                    case S6xNavHeaderCategory.RESERVED:
                        continue;
                    case S6xNavHeaderCategory.TABLES:
                    case S6xNavHeaderCategory.FUNCTIONS:
                    case S6xNavHeaderCategory.SCALARS:
                    case S6xNavHeaderCategory.STRUCTURES:
                    case S6xNavHeaderCategory.ROUTINES:
                    case S6xNavHeaderCategory.OPERATIONS:
                    case S6xNavHeaderCategory.REGISTERS:
                    case S6xNavHeaderCategory.OTHER:
                    case S6xNavHeaderCategory.SIGNATURES:
                    case S6xNavHeaderCategory.ELEMSSIGNATURES:
                        TreeNode tnParent = new TreeNode();
                        tnParent.Name = tnMainParent.Name;
                        tnParent.Text = S6xNav.getHeaderCategLabel(S6xNav.getHeaderCateg(tnMainParent.Name));
                        tnParent.ToolTipText = tnMainParent.ToolTipText;
                        searchTreeView.Nodes.Add(tnParent);
                        break;
                    default:
                        continue;
                }
            }
        }

        private void searchTreeViewCount()
        {
            foreach (TreeNode tnParent in searchTreeView.Nodes)
            {
                switch (S6xNav.getHeaderCateg(tnParent.Name))
                {
                    case S6xNavHeaderCategory.TABLES:
                    case S6xNavHeaderCategory.FUNCTIONS:
                    case S6xNavHeaderCategory.SCALARS:
                    case S6xNavHeaderCategory.STRUCTURES:
                    case S6xNavHeaderCategory.ROUTINES:
                    case S6xNavHeaderCategory.OPERATIONS:
                    case S6xNavHeaderCategory.REGISTERS:
                    case S6xNavHeaderCategory.OTHER:
                    case S6xNavHeaderCategory.SIGNATURES:
                    case S6xNavHeaderCategory.ELEMSSIGNATURES:
                        break;
                    default:
                        continue;
                }
                tnParent.Text = S6xNav.getHeaderCategLabel(tnParent.Name) + " (" + tnParent.Nodes.Count.ToString() + ")";
            }
        }
    }
}
