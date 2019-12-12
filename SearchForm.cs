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
        
        public SearchForm(ref SADS6x mainSadS6x, ref TreeView mainElemsTreeView)
        {
            sadS6x = mainSadS6x;
            elemsTreeView = mainElemsTreeView;
            
            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            searchTextBox.KeyPress += new KeyPressEventHandler(searchTextBox_KeyPress);
            searchTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(searchTreeView_NodeMouseClick);
            searchTreeView.AfterSelect += new TreeViewEventHandler(searchTreeView_AfterSelect);

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
            try { elemsTreeView.SelectedNode = elemsTreeView.Nodes[e.Node.Parent.Name].Nodes[e.Node.Name]; }
            catch { }
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

            searchTreeViewInit();

            foreach (string[] result in results)
            {
                TreeNode parentNode = searchTreeView.Nodes[elemsTreeView.Nodes[result[0]].Name];
                if (parentNode == null) continue;

                if (!elemsTreeView.Nodes[result[0]].Nodes.ContainsKey(result[1])) continue;
                if (parentNode.Nodes.ContainsKey(result[1])) continue;

                TreeNode tnNode = new TreeNode();
                tnNode.Name = elemsTreeView.Nodes[result[0]].Nodes[result[1]].Name;
                tnNode.Text = elemsTreeView.Nodes[result[0]].Nodes[result[1]].Text;
                tnNode.ToolTipText = elemsTreeView.Nodes[result[0]].Nodes[result[1]].ToolTipText;
                parentNode.Nodes.Add(tnNode);
            }

            searchTreeViewCount();
        }

        private List<string[]> SearchText(string searchText)
        {
            List<string[]> results = new List<string[]>();

            foreach (S6xFunction s6xObject in sadS6x.slFunctions.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { "FUNCTIONS", s6xObject.UniqueAddress });
            }
            foreach (S6xOperation s6xObject in sadS6x.slOperations.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { "OPERATIONS", s6xObject.UniqueAddress });
            }
            foreach (S6xOtherAddress s6xObject in sadS6x.slOtherAddresses.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.Comments };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { "OTHER", s6xObject.UniqueAddress });
            }
            foreach (S6xRegister s6xObject in sadS6x.slRegisters.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ByteLabel, s6xObject.WordLabel, s6xObject.Comments };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { "REGISTERS", s6xObject.UniqueAddress });
            }
            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { "ROUTINES", s6xObject.UniqueAddress });
            }
            foreach (S6xScalar s6xObject in sadS6x.slScalars.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { "SCALARS", s6xObject.UniqueAddress });
            }
            foreach (S6xSignature s6xObject in sadS6x.slSignatures.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { "SIGNATURES", s6xObject.UniqueKey });
            }
            foreach (S6xElementSignature s6xObject in sadS6x.slElementsSignatures.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.SignatureLabel, s6xObject.SignatureKey, s6xObject.SignatureComments, string.Empty, string.Empty, string.Empty };
                if (s6xObject.Scalar != null)
                {
                    searchValues[3] = s6xObject.Scalar.Label;
                    searchValues[4] = s6xObject.Scalar.ShortLabel;
                    searchValues[5] = s6xObject.Scalar.Comments;
                }
                else if (s6xObject.Function != null)
                {
                    searchValues[3] = s6xObject.Function.Label;
                    searchValues[4] = s6xObject.Function.ShortLabel;
                    searchValues[5] = s6xObject.Function.Comments;
                }
                else if (s6xObject.Table != null)
                {
                    searchValues[3] = s6xObject.Table.Label;
                    searchValues[4] = s6xObject.Table.ShortLabel;
                    searchValues[5] = s6xObject.Table.Comments;
                }
                else if (s6xObject.Structure != null)
                {
                    searchValues[3] = s6xObject.Structure.Label;
                    searchValues[4] = s6xObject.Structure.ShortLabel;
                    searchValues[5] = s6xObject.Structure.Comments;
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
                if (isResult) results.Add(new string[] { "ELEMSSIGNATURES", s6xObject.UniqueKey });
            }
            foreach (S6xStructure s6xObject in sadS6x.slStructures.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { "STRUCTURES", s6xObject.UniqueAddress });
            }
            foreach (S6xTable s6xObject in sadS6x.slTables.Values)
            {
                bool isResult = false;
                string[] searchValues = new string[] { s6xObject.Address, s6xObject.Label, s6xObject.ShortLabel, s6xObject.Comments };
                foreach (string searchValue in searchValues)
                {
                    string cSearchValue = (searchValue == null) ? string.Empty : searchValue.ToLower();
                    if (cSearchValue.Contains(searchText))
                    {
                        isResult = true;
                        break;
                    }
                }
                if (isResult) results.Add(new string[] { "TABLES", s6xObject.UniqueAddress });
            }

            return results;
        }

        private List<string[]> SearchArgsRoutines()
        {
            List<string[]> results = new List<string[]>();

            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                if (s6xObject.ByteArgumentsNum > 0) results.Add(new string[] { "ROUTINES", s6xObject.UniqueAddress });
            }

            return results;
        }

        private List<string[]> SearchAdvancedRoutines()
        {
            List<string[]> results = new List<string[]>();

            foreach (S6xRoutine s6xObject in sadS6x.slRoutines.Values)
            {
                if (s6xObject.isAdvanced) results.Add(new string[] { "ROUTINES", s6xObject.UniqueAddress });
            }

            return results;
        }

        private void searchTreeViewInit()
        {
            searchTreeView.Nodes.Clear();

            foreach (TreeNode tnMainParent in elemsTreeView.Nodes)
            {
                switch (tnMainParent.Name)
                {
                    case "PROPERTIES":
                    case "RESERVED":
                        break;
                    default:
                        string categLabel = string.Empty;
                        switch (tnMainParent.Name)
                        {
                            case "TABLES":
                                categLabel = "Tables";
                                break;
                            case "FUNCTIONS":
                                categLabel = "Functions";
                                break;
                            case "SCALARS":
                                categLabel = "Scalars";
                                break;
                            case "STRUCTURES":
                                categLabel = "Structures";
                                break;
                            case "ROUTINES":
                                categLabel = "Routines";
                                break;
                            case "OPERATIONS":
                                categLabel = "Operations";
                                break;
                            case "REGISTERS":
                                categLabel = "Registers";
                                break;
                            case "OTHER":
                                categLabel = "Other Addresses";
                                break;
                            case "SIGNATURES":
                                categLabel = "Routines Signatures";
                                break;
                            case "ELEMSSIGNATURES":
                                categLabel = "Elements Signatures";
                                break;
                            default:
                                return;
                        }
                        TreeNode tnParent = new TreeNode();
                        tnParent.Name = tnMainParent.Name;
                        tnParent.Text = categLabel;
                        tnParent.ToolTipText = tnMainParent.ToolTipText;
                        searchTreeView.Nodes.Add(tnParent);
                        break;
                }
            }
        }

        private void searchTreeViewCount()
        {
            foreach (TreeNode tnParent in searchTreeView.Nodes)
            {
                string categLabel = string.Empty;
                switch (tnParent.Name)
                {
                    case "TABLES":
                        categLabel = "Tables";
                        break;
                    case "FUNCTIONS":
                        categLabel = "Functions";
                        break;
                    case "SCALARS":
                        categLabel = "Scalars";
                        break;
                    case "STRUCTURES":
                        categLabel = "Structures";
                        break;
                    case "ROUTINES":
                        categLabel = "Routines";
                        break;
                    case "OPERATIONS":
                        categLabel = "Operations";
                        break;
                    case "REGISTERS":
                        categLabel = "Registers";
                        break;
                    case "OTHER":
                        categLabel = "Other Addresses";
                        break;
                    case "SIGNATURES":
                        categLabel = "Routines Signatures";
                        break;
                    case "ELEMSSIGNATURES":
                        categLabel = "Elements Signatures";
                        break;
                    default:
                        return;
                }
                tnParent.Text = categLabel + " (" + tnParent.Nodes.Count.ToString() + ")";
            }
        }
    }
}
