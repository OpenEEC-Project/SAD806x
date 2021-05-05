using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using SQLite806x;

namespace SAD806x
{
    public partial class SQLite806xSyncForm : Form
    {
        SQLite806xDB sqlDB806x = null;
        List<S_SQLiteSyncS6x> unSyncObjects = null;
        List<S_SQLiteSyncS6x> objectsToRemove = null;
        TreeView elemsTreeView = null;

        private const string SyncSideS6xName = "S6X";
        private const string SyncSideS6xText = "SAD 806x Definition";
        private const string SyncSideS6xToolTipText = "SAD 806x Definition";

        private const string SyncSideDb806xName = "DB806X";
        private const string SyncSideDb806xText = "806x Universal Database";
        private const string SyncSideDb806xToolTipText = "806x Universal Database";
        
        public SQLite806xSyncForm(ref SQLite806xDB db806x, ref List<S_SQLiteSyncS6x> lstUnSyncObjects, ref List<S_SQLiteSyncS6x> lstObjectsToRemove, ref TreeView mainElemsTreeView)
        {
            sqlDB806x = db806x;
            unSyncObjects = lstUnSyncObjects;
            objectsToRemove = lstObjectsToRemove;
            elemsTreeView = mainElemsTreeView;

            InitializeComponent();

            try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { }

            this.Load += new EventHandler(Form_Load);
            this.FormClosing += new FormClosingEventHandler(Form_FormClosing);

            //syncTreeView.StateImageList = elemsTreeView.StateImageList;

            syncTreeView.AfterCheck += new TreeViewEventHandler(syncTreeView_AfterCheck);
        }

        private void Form_Load(object sender, EventArgs e)
        {
            if (sqlDB806x == null)
            {
                this.Close();
                return;
            }

            if (!sqlDB806x.ValidDB)
            {
                this.Close();
                return;
            }

            if (unSyncObjects == null) return;
            if (unSyncObjects.Count == 0) return;

            List<object[]> s6xResults = new List<object[]>();
            List<object[]> dbResults = new List<object[]>();

            foreach (S_SQLiteSyncS6x unSyncObject in unSyncObjects)
            {
                S6xNavHeaderCategory headerCateg = S6xNavHeaderCategory.UNDEFINED;
                string nodeName = string.Empty;
                string nodeText = string.Empty;
                string nodeToolTipText = string.Empty;
                string subNodeName = string.Empty;
                string subNodeText = string.Empty;
                string subNodeToolTipText = string.Empty;
                string subSubNodeName = string.Empty;
                string subSubNodeText = string.Empty;
                string subSubNodeToolTipText = string.Empty;

                if (unSyncObject.S6xObject != null)
                {
                    if (unSyncObject.S6xObject.GetType() == typeof(S6xTable))
                    {
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = ((S6xTable)unSyncObject.S6xObject).Label;
                        nodeToolTipText = ((S6xTable)unSyncObject.S6xObject).Comments;
                        headerCateg = S6xNavHeaderCategory.TABLES;
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xFunction))
                    {
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = ((S6xFunction)unSyncObject.S6xObject).Label;
                        nodeToolTipText = ((S6xFunction)unSyncObject.S6xObject).Comments;
                        headerCateg = S6xNavHeaderCategory.FUNCTIONS;
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xScalar))
                    {
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = ((S6xScalar)unSyncObject.S6xObject).Label;
                        nodeToolTipText = ((S6xScalar)unSyncObject.S6xObject).Comments;
                        headerCateg = S6xNavHeaderCategory.SCALARS;
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xStructure))
                    {
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = ((S6xStructure)unSyncObject.S6xObject).Label;
                        nodeToolTipText = ((S6xStructure)unSyncObject.S6xObject).Comments;
                        headerCateg = S6xNavHeaderCategory.STRUCTURES;
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRoutine))
                    {
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = ((S6xRoutine)unSyncObject.S6xObject).Label;
                        nodeToolTipText = ((S6xRoutine)unSyncObject.S6xObject).Comments;
                        headerCateg = S6xNavHeaderCategory.ROUTINES;
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xOperation))
                    {
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = ((S6xOperation)unSyncObject.S6xObject).Label;
                        nodeToolTipText = ((S6xOperation)unSyncObject.S6xObject).Comments;
                        headerCateg = S6xNavHeaderCategory.OPERATIONS;
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRegister))
                    {
                        nodeName = unSyncObject.RegisterUniqueAddress;
                        nodeText = ((S6xRegister)unSyncObject.S6xObject).Label;
                        nodeToolTipText = ((S6xRegister)unSyncObject.S6xObject).Comments;
                        headerCateg = S6xNavHeaderCategory.REGISTERS;
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xOtherAddress))
                    {
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = ((S6xOtherAddress)unSyncObject.S6xObject).Label;
                        nodeToolTipText = ((S6xOtherAddress)unSyncObject.S6xObject).Comments;
                        headerCateg = S6xNavHeaderCategory.OTHER;
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xSignature))
                    {
                        nodeName = unSyncObject.UniqueKey;
                        nodeText = ((S6xSignature)unSyncObject.S6xObject).SignatureLabel;
                        nodeToolTipText = ((S6xSignature)unSyncObject.S6xObject).SignatureComments;
                        headerCateg = S6xNavHeaderCategory.SIGNATURES;
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xElementSignature))
                    {
                        nodeName = unSyncObject.UniqueKey;
                        nodeText = ((S6xElementSignature)unSyncObject.S6xObject).SignatureLabel;
                        nodeToolTipText = ((S6xElementSignature)unSyncObject.S6xObject).SignatureComments;
                        headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xBitFlag))
                    {
                        if (unSyncObject.S6xParentObject != null)
                        {
                            if (unSyncObject.S6xParentObject.GetType() == typeof(S6xScalar))
                            {
                                nodeName = unSyncObject.isDuplicate ? ((S6xScalar)unSyncObject.S6xParentObject).DuplicateAddress : ((S6xScalar)unSyncObject.S6xParentObject).UniqueAddress;
                                nodeText = ((S6xScalar)unSyncObject.S6xParentObject).Label;
                                nodeToolTipText = ((S6xScalar)unSyncObject.S6xParentObject).Comments;
                                subNodeName = unSyncObject.SyncUniqueKey;
                                subNodeText = ((S6xBitFlag)unSyncObject.S6xObject).Position.ToString() + " - " + ((S6xBitFlag)unSyncObject.S6xObject).ShortLabel;
                                subNodeToolTipText = subNodeText + "\r\n\r\n" + ((S6xBitFlag)unSyncObject.S6xObject).Comments;
                                headerCateg = S6xNavHeaderCategory.SCALARS;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xRegister))
                            {
                                nodeName = ((S6xRegister)unSyncObject.S6xParentObject).UniqueAddress;
                                nodeText = ((S6xRegister)unSyncObject.S6xParentObject).Label;
                                nodeToolTipText = ((S6xRegister)unSyncObject.S6xParentObject).Comments;
                                subNodeName = unSyncObject.SyncUniqueKey;
                                subNodeText = ((S6xBitFlag)unSyncObject.S6xObject).Position.ToString() + " - " + ((S6xBitFlag)unSyncObject.S6xObject).ShortLabel;
                                subNodeToolTipText = subNodeText + "\r\n\r\n" + ((S6xBitFlag)unSyncObject.S6xObject).Comments;
                                headerCateg = S6xNavHeaderCategory.REGISTERS;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xRoutineInternalScalar))
                            {
                                if (unSyncObject.S6xParentParentObject != null)
                                {
                                    if (unSyncObject.S6xParentParentObject.GetType() == typeof(S6xSignature))
                                    {
                                        nodeName = ((S6xSignature)unSyncObject.S6xParentParentObject).UniqueKey;
                                        nodeText = ((S6xSignature)unSyncObject.S6xParentParentObject).SignatureLabel;
                                        nodeToolTipText = ((S6xSignature)unSyncObject.S6xParentParentObject).SignatureComments;
                                        headerCateg = S6xNavHeaderCategory.SIGNATURES;
                                    }
                                    else if (unSyncObject.S6xParentParentObject.GetType() == typeof(S6xElementSignature))
                                    {
                                        nodeName = ((S6xElementSignature)unSyncObject.S6xParentParentObject).UniqueKey;
                                        nodeText = ((S6xElementSignature)unSyncObject.S6xParentParentObject).SignatureLabel;
                                        nodeToolTipText = ((S6xElementSignature)unSyncObject.S6xParentParentObject).SignatureComments;
                                        headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                                    }
                                    subNodeName = nodeName + "." + ((S6xRoutineInternalScalar)unSyncObject.S6xParentObject).UniqueKey;
                                    subNodeText = ((S6xRoutineInternalScalar)unSyncObject.S6xParentObject).ShortLabel + " - " + ((S6xRoutineInternalScalar)unSyncObject.S6xParentObject).Label;
                                    subNodeToolTipText = subNodeText + "\r\n\r\n" + ((S6xRoutineInternalScalar)unSyncObject.S6xParentObject).Comments;
                                    subSubNodeName = unSyncObject.SyncUniqueKey;
                                    subSubNodeText = ((S6xBitFlag)unSyncObject.S6xObject).Position.ToString() + " - " + ((S6xBitFlag)unSyncObject.S6xObject).ShortLabel;
                                    subSubNodeToolTipText = subSubNodeToolTipText + "\r\n\r\n" + ((S6xBitFlag)unSyncObject.S6xObject).Comments;
                                }
                            }
                        }
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRoutineInputArgument))
                    {
                        if (unSyncObject.S6xParentObject != null)
                        {
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Argument " + ((S6xRoutineInputArgument)unSyncObject.S6xObject).Code;
                            if (unSyncObject.S6xParentObject.GetType() == typeof(S6xRoutine))
                            {
                                nodeName = ((S6xRoutine)unSyncObject.S6xParentObject).UniqueAddress;
                                nodeText = ((S6xRoutine)unSyncObject.S6xParentObject).Label;
                                nodeToolTipText = ((S6xRoutine)unSyncObject.S6xParentObject).Comments;
                                headerCateg = S6xNavHeaderCategory.ROUTINES;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xSignature))
                            {
                                nodeName = ((S6xSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.SIGNATURES;
                            }
                        }
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRoutineInputScalar))
                    {
                        if (unSyncObject.S6xParentObject != null)
                        {
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Scalar " + ((S6xRoutineInputScalar)unSyncObject.S6xObject).UniqueKey;
                            if (unSyncObject.S6xParentObject.GetType() == typeof(S6xRoutine))
                            {
                                nodeName = ((S6xRoutine)unSyncObject.S6xParentObject).UniqueAddress;
                                nodeText = ((S6xRoutine)unSyncObject.S6xParentObject).Label;
                                nodeToolTipText = ((S6xRoutine)unSyncObject.S6xParentObject).Comments;
                                headerCateg = S6xNavHeaderCategory.ROUTINES;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xSignature))
                            {
                                nodeName = ((S6xSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.SIGNATURES;
                            }
                        }
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRoutineInputFunction))
                    {
                        if (unSyncObject.S6xParentObject != null)
                        {
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Function " + ((S6xRoutineInputFunction)unSyncObject.S6xObject).UniqueKey;
                            if (unSyncObject.S6xParentObject.GetType() == typeof(S6xRoutine))
                            {
                                nodeName = ((S6xRoutine)unSyncObject.S6xParentObject).UniqueAddress;
                                nodeText = ((S6xRoutine)unSyncObject.S6xParentObject).Label;
                                nodeToolTipText = ((S6xRoutine)unSyncObject.S6xParentObject).Comments;
                                headerCateg = S6xNavHeaderCategory.ROUTINES;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xSignature))
                            {
                                nodeName = ((S6xSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.SIGNATURES;
                            }
                        }
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRoutineInputTable))
                    {
                        if (unSyncObject.S6xParentObject != null)
                        {
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Table " + ((S6xRoutineInputTable)unSyncObject.S6xObject).UniqueKey;
                            if (unSyncObject.S6xParentObject.GetType() == typeof(S6xRoutine))
                            {
                                nodeName = ((S6xRoutine)unSyncObject.S6xParentObject).UniqueAddress;
                                nodeText = ((S6xRoutine)unSyncObject.S6xParentObject).Label;
                                nodeToolTipText = ((S6xRoutine)unSyncObject.S6xParentObject).Comments;
                                headerCateg = S6xNavHeaderCategory.ROUTINES;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xSignature))
                            {
                                nodeName = ((S6xSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.SIGNATURES;
                            }
                        }
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRoutineInputStructure))
                    {
                        if (unSyncObject.S6xParentObject != null)
                        {
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Structure " + ((S6xRoutineInputStructure)unSyncObject.S6xObject).UniqueKey;
                            if (unSyncObject.S6xParentObject.GetType() == typeof(S6xRoutine))
                            {
                                nodeName = ((S6xRoutine)unSyncObject.S6xParentObject).UniqueAddress;
                                nodeText = ((S6xRoutine)unSyncObject.S6xParentObject).Label;
                                nodeToolTipText = ((S6xRoutine)unSyncObject.S6xParentObject).Comments;
                                headerCateg = S6xNavHeaderCategory.ROUTINES;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xSignature))
                            {
                                nodeName = ((S6xSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.SIGNATURES;
                            }
                        }
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRoutineInternalScalar))
                    {
                        if (unSyncObject.S6xParentObject != null)
                        {
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Scalar " + ((S6xRoutineInternalScalar)unSyncObject.S6xObject).ShortLabel + " - " + ((S6xRoutineInternalScalar)unSyncObject.S6xObject).Label;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((S6xRoutineInternalScalar)unSyncObject.S6xObject).Comments;
                            if (unSyncObject.S6xParentObject.GetType() == typeof(S6xElementSignature))
                            {
                                nodeName = ((S6xElementSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xElementSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xElementSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xSignature))
                            {
                                nodeName = ((S6xSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.SIGNATURES;
                            }
                        }
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRoutineInternalFunction))
                    {
                        if (unSyncObject.S6xParentObject != null)
                        {
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Function " + ((S6xRoutineInternalFunction)unSyncObject.S6xObject).ShortLabel + " - " + ((S6xRoutineInternalFunction)unSyncObject.S6xObject).Label;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((S6xRoutineInternalFunction)unSyncObject.S6xObject).Comments;
                            if (unSyncObject.S6xParentObject.GetType() == typeof(S6xElementSignature))
                            {
                                nodeName = ((S6xElementSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xElementSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xElementSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xSignature))
                            {
                                nodeName = ((S6xSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.SIGNATURES;
                            }
                        }
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRoutineInternalTable))
                    {
                        if (unSyncObject.S6xParentObject != null)
                        {
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Table " + ((S6xRoutineInternalTable)unSyncObject.S6xObject).ShortLabel + " - " + ((S6xRoutineInternalTable)unSyncObject.S6xObject).Label;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((S6xRoutineInternalTable)unSyncObject.S6xObject).Comments;
                            if (unSyncObject.S6xParentObject.GetType() == typeof(S6xElementSignature))
                            {
                                nodeName = ((S6xElementSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xElementSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xElementSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xSignature))
                            {
                                nodeName = ((S6xSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.SIGNATURES;
                            }
                        }
                    }
                    else if (unSyncObject.S6xObject.GetType() == typeof(S6xRoutineInternalStructure))
                    {
                        if (unSyncObject.S6xParentObject != null)
                        {
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Structure " + ((S6xRoutineInternalStructure)unSyncObject.S6xObject).ShortLabel + " - " + ((S6xRoutineInternalStructure)unSyncObject.S6xObject).Label;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((S6xRoutineInternalStructure)unSyncObject.S6xObject).Comments;
                            if (unSyncObject.S6xParentObject.GetType() == typeof(S6xElementSignature))
                            {
                                nodeName = ((S6xElementSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xElementSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xElementSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                            }
                            else if (unSyncObject.S6xParentObject.GetType() == typeof(S6xSignature))
                            {
                                nodeName = ((S6xSignature)unSyncObject.S6xParentObject).UniqueKey;
                                nodeText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureLabel;
                                nodeToolTipText = ((S6xSignature)unSyncObject.S6xParentObject).SignatureComments;
                                headerCateg = S6xNavHeaderCategory.SIGNATURES;
                            }
                        }
                    }

                    if (headerCateg == S6xNavHeaderCategory.UNDEFINED) continue;

                    s6xResults.Add(new object[] { unSyncObject, S6xNav.getHeaderCategName(headerCateg), nodeName, nodeText, nodeToolTipText, subNodeName, subNodeText, subNodeToolTipText, subSubNodeName, subSubNodeText, subSubNodeToolTipText });

                    continue;
                }

                if (unSyncObject.SqLiteSAD806xObject != null)
                {
                    if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Tables))
                    {
                        headerCateg = S6xNavHeaderCategory.TABLES;
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = (string)((R_SAD806x_Def_Tables)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                        nodeToolTipText = (string)((R_SAD806x_Def_Tables)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Functions))
                    {
                        headerCateg = S6xNavHeaderCategory.FUNCTIONS;
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = (string)((R_SAD806x_Def_Functions)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                        nodeToolTipText = (string)((R_SAD806x_Def_Functions)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Scalars))
                    {
                        headerCateg = S6xNavHeaderCategory.SCALARS;
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = (string)((R_SAD806x_Def_Scalars)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                        nodeToolTipText = (string)((R_SAD806x_Def_Scalars)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Structures))
                    {
                        headerCateg = S6xNavHeaderCategory.STRUCTURES;
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = (string)((R_SAD806x_Def_Structures)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                        nodeToolTipText = (string)((R_SAD806x_Def_Structures)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Routines))
                    {
                        headerCateg = S6xNavHeaderCategory.ROUTINES;
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = (string)((R_SAD806x_Def_Routines)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                        nodeToolTipText = (string)((R_SAD806x_Def_Routines)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Operations))
                    {
                        headerCateg = S6xNavHeaderCategory.OPERATIONS;
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = (string)((R_SAD806x_Def_Operations)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                        nodeToolTipText = (string)((R_SAD806x_Def_Operations)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Registers))
                    {
                        headerCateg = S6xNavHeaderCategory.REGISTERS;
                        nodeName = unSyncObject.RegisterUniqueAddress;
                        nodeText = (string)((R_SAD806x_Def_Registers)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                        nodeToolTipText = (string)((R_SAD806x_Def_Registers)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_Registers))
                    {
                        headerCateg = S6xNavHeaderCategory.OTHER;
                        nodeName = unSyncObject.isDuplicate ? unSyncObject.DuplicateAddress : unSyncObject.UniqueAddress;
                        nodeText = (string)((R_SAD806x_Def_Registers)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                        nodeToolTipText = (string)((R_SAD806x_Def_Registers)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutines))
                    {
                        headerCateg = S6xNavHeaderCategory.SIGNATURES;
                        nodeName = unSyncObject.UniqueKey;
                        nodeText = (string)((R_SAD806x_SignaturesRoutines)unSyncObject.SqLiteSAD806xObject).SignatureLabel.ValueConverted;
                        nodeToolTipText = (string)((R_SAD806x_SignaturesRoutines)unSyncObject.SqLiteSAD806xObject).SignatureComments.ValueConverted;
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesElements))
                    {
                        headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                        nodeName = unSyncObject.UniqueKey;
                        nodeText = (string)((R_SAD806x_SignaturesElements)unSyncObject.SqLiteSAD806xObject).SignatureLabel.ValueConverted;
                        nodeToolTipText = (string)((R_SAD806x_SignaturesElements)unSyncObject.SqLiteSAD806xObject).SignatureComments.ValueConverted;
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_ScalarsBitFlags))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = ((R_SAD806x_Def_ScalarsBitFlags)unSyncObject.SqLiteSAD806xObject).Position.ValueConverted.ToString() + " - " + ((R_SAD806x_Def_ScalarsBitFlags)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_Def_ScalarsBitFlags)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.SCALARS;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_RegistersBitFlags))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = ((R_SAD806x_Def_RegistersBitFlags)unSyncObject.SqLiteSAD806xObject).Position.ValueConverted.ToString() + " - " + ((R_SAD806x_Def_RegistersBitFlags)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_Def_RegistersBitFlags)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.REGISTERS;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            if (unSyncObject.SyncUniqueKey.Split('.').Length > 2)
                            {
                                subNodeName = nodeName + "." + unSyncObject.SyncUniqueKey.Split('.')[1];
                                subSubNodeName = unSyncObject.SyncUniqueKey;
                                subSubNodeText = ((R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags)unSyncObject.SqLiteSAD806xObject).Position.ValueConverted.ToString() + " - " + ((R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted;
                                subSubNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_SignaturesRoutinesInternalScalarsBitFlags)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                                headerCateg = S6xNavHeaderCategory.SIGNATURES;
                            }
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesElementsInternalScalarsBitFlags))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            if (unSyncObject.SyncUniqueKey.Split('.').Length > 2)
                            {
                                subNodeName = nodeName + "." + unSyncObject.SyncUniqueKey.Split('.')[1];
                                subSubNodeName = unSyncObject.SyncUniqueKey;
                                subSubNodeText = ((R_SAD806x_SignaturesElementsInternalScalarsBitFlags)unSyncObject.SqLiteSAD806xObject).Position.ValueConverted.ToString() + " - " + ((R_SAD806x_SignaturesElementsInternalScalarsBitFlags)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted;
                                subSubNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_SignaturesElementsInternalScalarsBitFlags)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                                headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                            }
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_RoutinesInputArgs))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Argument " + ((R_SAD806x_Def_RoutinesInputArgs)unSyncObject.SqLiteSAD806xObject).Position.ValueConverted.ToString();
                            headerCateg = S6xNavHeaderCategory.ROUTINES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_RoutinesInputScalars))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Scalar " + ((R_SAD806x_Def_RoutinesInputScalars)unSyncObject.SqLiteSAD806xObject).UniqueKey.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.ROUTINES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_RoutinesInputFunctions))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Function " + ((R_SAD806x_Def_RoutinesInputFunctions)unSyncObject.SqLiteSAD806xObject).UniqueKey.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.ROUTINES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_RoutinesInputTables))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Table " + ((R_SAD806x_Def_RoutinesInputTables)unSyncObject.SqLiteSAD806xObject).UniqueKey.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.ROUTINES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_Def_RoutinesInputStructures))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Structure " + ((R_SAD806x_Def_RoutinesInputStructures)unSyncObject.SqLiteSAD806xObject).UniqueKey.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.ROUTINES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInputArgs))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Argument " + ((R_SAD806x_SignaturesRoutinesInputArgs)unSyncObject.SqLiteSAD806xObject).Position.ValueConverted.ToString();
                            headerCateg = S6xNavHeaderCategory.SIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInputScalars))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Scalar " + ((R_SAD806x_SignaturesRoutinesInputScalars)unSyncObject.SqLiteSAD806xObject).UniqueKey.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.SIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInputFunctions))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Function " + ((R_SAD806x_SignaturesRoutinesInputFunctions)unSyncObject.SqLiteSAD806xObject).UniqueKey.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.SIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInputTables))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Table " + ((R_SAD806x_SignaturesRoutinesInputTables)unSyncObject.SqLiteSAD806xObject).UniqueKey.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.SIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInputStructures))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Input Structure " + ((R_SAD806x_SignaturesRoutinesInputStructures)unSyncObject.SqLiteSAD806xObject).UniqueKey.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.SIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalScalars))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Scalar " + ((R_SAD806x_SignaturesRoutinesInternalScalars)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted + " - " + ((R_SAD806x_SignaturesRoutinesInternalScalars)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_SignaturesRoutinesInternalScalars)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.SIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalFunctions))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Function " + ((R_SAD806x_SignaturesRoutinesInternalFunctions)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted + " - " + ((R_SAD806x_SignaturesRoutinesInternalFunctions)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_SignaturesRoutinesInternalFunctions)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.SIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalTables))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Table " + ((R_SAD806x_SignaturesRoutinesInternalTables)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted + " - " + ((R_SAD806x_SignaturesRoutinesInternalTables)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_SignaturesRoutinesInternalTables)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.SIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesRoutinesInternalStructures))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Structure " + ((R_SAD806x_SignaturesRoutinesInternalStructures)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted + " - " + ((R_SAD806x_SignaturesRoutinesInternalStructures)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_SignaturesRoutinesInternalStructures)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.SIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesElementsInternalScalars))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Scalar " + ((R_SAD806x_SignaturesElementsInternalScalars)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted + " - " + ((R_SAD806x_SignaturesElementsInternalScalars)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_SignaturesElementsInternalScalars)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesElementsInternalFunctions))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Function " + ((R_SAD806x_SignaturesElementsInternalFunctions)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted + " - " + ((R_SAD806x_SignaturesElementsInternalFunctions)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_SignaturesElementsInternalFunctions)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesElementsInternalTables))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Table " + ((R_SAD806x_SignaturesElementsInternalTables)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted + " - " + ((R_SAD806x_SignaturesElementsInternalTables)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_SignaturesElementsInternalTables)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                        }
                    }
                    else if (unSyncObject.SqLiteSAD806xObject.GetType() == typeof(R_SAD806x_SignaturesElementsInternalStructures))
                    {
                        if (unSyncObject.SyncUniqueKey.Contains("."))
                        {
                            nodeName = unSyncObject.SyncUniqueKey.Split('.')[0];
                            subNodeName = unSyncObject.SyncUniqueKey;
                            subNodeText = "Internal Structure " + ((R_SAD806x_SignaturesElementsInternalStructures)unSyncObject.SqLiteSAD806xObject).ShortLabel.ValueConverted + " - " + ((R_SAD806x_SignaturesElementsInternalStructures)unSyncObject.SqLiteSAD806xObject).Label.ValueConverted;
                            subNodeToolTipText = subNodeText + "\r\n\r\n" + ((R_SAD806x_SignaturesElementsInternalStructures)unSyncObject.SqLiteSAD806xObject).Comments.ValueConverted;
                            headerCateg = S6xNavHeaderCategory.ELEMSSIGNATURES;
                        }
                    }

                    if (headerCateg == S6xNavHeaderCategory.UNDEFINED) continue;

                    dbResults.Add(new object[] { unSyncObject, S6xNav.getHeaderCategName(headerCateg), nodeName, nodeText, nodeToolTipText, subNodeName, subNodeText, subNodeToolTipText, subSubNodeName, subSubNodeText, subSubNodeToolTipText });

                    continue;
                }
            }

            syncTreeView.BeginUpdate();

            syncTreeViewInit();

            TreeNode tnSyncSide = null;

            if (syncTreeView.Nodes.ContainsKey(SyncSideS6xName))
            {
                tnSyncSide = syncTreeView.Nodes[SyncSideS6xName];
                foreach (object[] result in s6xResults)
                {
                    S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[(string)result[1]]);
                    if (!niMFHeaderCateg.isValid) continue;

                    TreeNode parentNode = tnSyncSide.Nodes[tnSyncSide.Name + "." + niMFHeaderCateg.HeaderCategoryName];
                    if (parentNode == null) continue;

                    TreeNode tnMFNode = niMFHeaderCateg.FindElement((string)result[2]);
                    
                    TreeNode tnNode = null;
                    if (parentNode.Nodes.ContainsKey((string)result[2])) tnNode = parentNode.Nodes[(string)result[2]];
                    else
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = tnMFNode == null ? (string)result[2] : tnMFNode.Name;
                        tnNode.Text = tnMFNode == null ? (string)result[3] : tnMFNode.Text;
                        tnNode.ToolTipText = tnMFNode == null ? (string)result[4] : tnMFNode.ToolTipText;
                        if (tnMFNode != null) tnNode.StateImageKey = tnMFNode.StateImageKey;
                        tnNode.Checked = true;
                        parentNode.Nodes.Add(tnNode);
                    }
                    if ((string)result[5] == string.Empty) tnNode.Tag = result[0];

                    if ((string)result[5] != string.Empty)
                    {
                        TreeNode tnSubNode = null;
                        if (tnNode.Nodes.ContainsKey((string)result[5])) tnSubNode = tnNode.Nodes[(string)result[5]];
                        else
                        {
                            tnSubNode = new TreeNode();
                            tnSubNode.Name = (string)result[5];
                            tnSubNode.Text = (string)result[6];
                            tnSubNode.ToolTipText = (string)result[7];
                            tnSubNode.Checked = true;
                            tnNode.Nodes.Add(tnSubNode);
                        }
                        if ((string)result[8] == string.Empty) tnSubNode.Tag = result[0];

                        if ((string)result[8] != string.Empty)
                        {
                            TreeNode tnSubSubNode = new TreeNode();
                            tnSubSubNode.Tag = result[0];
                            tnSubSubNode.Name = (string)result[5];
                            tnSubSubNode.Text = (string)result[6];
                            tnSubSubNode.ToolTipText = (string)result[7];
                            tnSubSubNode.Checked = true;
                            if (!tnSubNode.Nodes.ContainsKey(tnSubSubNode.Name)) tnSubNode.Nodes.Add(tnSubSubNode);
                        }
                    }
                }
            }

            if (syncTreeView.Nodes.ContainsKey(SyncSideDb806xName))
            {
                tnSyncSide = syncTreeView.Nodes[SyncSideDb806xName];
                foreach (object[] result in dbResults)
                {
                    S6xNavInfo niMFHeaderCateg = new S6xNavInfo(elemsTreeView.Nodes[(string)result[1]]);
                    if (!niMFHeaderCateg.isValid) continue;

                    TreeNode parentNode = tnSyncSide.Nodes[tnSyncSide.Name + "." + niMFHeaderCateg.HeaderCategoryName];
                    if (parentNode == null) continue;

                    TreeNode tnNode = null;
                    if (parentNode.Nodes.ContainsKey((string)result[2])) tnNode = parentNode.Nodes[(string)result[2]];
                    else
                    {
                        tnNode = new TreeNode();
                        tnNode.Name = (string)result[2];
                        tnNode.Text = (string)result[3];
                        tnNode.ToolTipText = (string)result[4];
                        tnNode.Checked = true;
                        parentNode.Nodes.Add(tnNode);
                    }
                    if ((string)result[5] == string.Empty) tnNode.Tag = result[0];

                    if ((string)result[5] != string.Empty)
                    {
                        TreeNode tnSubNode = null;
                        if (tnNode.Nodes.ContainsKey((string)result[5])) tnSubNode = tnNode.Nodes[(string)result[5]];
                        else
                        {
                            tnSubNode.Name = (string)result[5];
                            tnSubNode.Text = (string)result[6];
                            tnSubNode.ToolTipText = (string)result[7];
                            tnSubNode.Checked = true;
                            tnNode.Nodes.Add(tnSubNode);
                        }
                        if ((string)result[8] == string.Empty) tnSubNode.Tag = result[0];

                        if ((string)result[8] != string.Empty)
                        {
                            TreeNode tnSubSubNode = new TreeNode();
                            tnSubSubNode.Tag = result[0];
                            tnSubSubNode.Name = (string)result[5];
                            tnSubSubNode.Text = (string)result[6];
                            tnSubSubNode.ToolTipText = (string)result[7];
                            tnSubSubNode.Checked = true;
                            if (!tnSubNode.Nodes.ContainsKey(tnSubSubNode.Name)) tnSubNode.Nodes.Add(tnSubSubNode);
                        }
                    }
                }
            }

            syncTreeViewCount();

            syncTreeView.EndUpdate();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!e.Cancel) Exit();
        }

        private void Exit()
        {
            sqlDB806x = null;

            Dispose();

            GC.Collect();
        }

        private void syncTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action != TreeViewAction.ByMouse && e.Action != TreeViewAction.ByKeyboard) return;

            foreach (TreeNode tnSubNode in e.Node.Nodes)
            {
                tnSubNode.Checked = e.Node.Checked;
                foreach (TreeNode tnSubSubNode in tnSubNode.Nodes) tnSubSubNode.Checked = e.Node.Checked;
            }

            if (e.Node.Parent == null) return;

            bool gChecked = true;
            bool gUnchecked = true;
            foreach (TreeNode tnSubNode in e.Node.Parent.Nodes)
            {
                gChecked &= tnSubNode.Checked;
                gUnchecked &= !tnSubNode.Checked;
            }
            e.Node.Parent.Checked = gChecked;

            if (e.Node.Parent.Parent == null) return;
            gChecked = true;
            gUnchecked = true;
            foreach (TreeNode tnSubNode in e.Node.Parent.Parent.Nodes)
            {
                gChecked &= tnSubNode.Checked;
                gUnchecked &= !tnSubNode.Checked;
            }
            e.Node.Parent.Parent.Checked = gChecked;

            if (e.Node.Parent.Parent.Parent == null) return;
            gChecked = true;
            gUnchecked = true;
            foreach (TreeNode tnSubNode in e.Node.Parent.Parent.Parent.Nodes)
            {
                gChecked &= tnSubNode.Checked;
                gUnchecked &= !tnSubNode.Checked;
            }
            e.Node.Parent.Parent.Parent.Checked = gChecked;

            if (e.Node.Parent.Parent.Parent.Parent == null) return;
            gChecked = true;
            gUnchecked = true;
            foreach (TreeNode tnSubNode in e.Node.Parent.Parent.Parent.Parent.Nodes)
            {
                gChecked &= tnSubNode.Checked;
                gUnchecked &= !tnSubNode.Checked;
            }
            e.Node.Parent.Parent.Parent.Parent.Checked = gChecked;
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            if (objectsToRemove == null) objectsToRemove = new List<S_SQLiteSyncS6x>();
            else objectsToRemove.Clear();

            TreeNode tnSyncSide = null;
            if (syncTreeView.Nodes.ContainsKey(SyncSideS6xName))
            {
                tnSyncSide = syncTreeView.Nodes[SyncSideS6xName];
                foreach (TreeNode tnCateg in tnSyncSide.Nodes)
                {
                    foreach (TreeNode tnNode in tnCateg.Nodes)
                    {
                        if (!tnNode.Checked && tnNode.Tag != null) objectsToRemove.Add((S_SQLiteSyncS6x)tnNode.Tag);
                        foreach (TreeNode tnSubNode in tnNode.Nodes)
                        {
                            if (!tnSubNode.Checked && tnSubNode.Tag != null) objectsToRemove.Add((S_SQLiteSyncS6x)tnSubNode.Tag);
                            foreach (TreeNode tnSubSubNode in tnSubNode.Nodes)
                            {
                                if (!tnSubSubNode.Checked && tnSubSubNode.Tag != null) objectsToRemove.Add((S_SQLiteSyncS6x)tnSubSubNode.Tag);
                            }
                        }
                    }
                }
            }

            if (syncTreeView.Nodes.ContainsKey(SyncSideDb806xName))
            {
                tnSyncSide = syncTreeView.Nodes[SyncSideDb806xName];
                foreach (TreeNode tnCateg in tnSyncSide.Nodes)
                {
                    foreach (TreeNode tnNode in tnCateg.Nodes)
                    {
                        if (!tnNode.Checked && tnNode.Tag != null) objectsToRemove.Add((S_SQLiteSyncS6x)tnNode.Tag);
                        foreach (TreeNode tnSubNode in tnNode.Nodes)
                        {
                            if (!tnSubNode.Checked && tnSubNode.Tag != null) objectsToRemove.Add((S_SQLiteSyncS6x)tnSubNode.Tag);
                            foreach (TreeNode tnSubSubNode in tnSubNode.Nodes)
                            {
                                if (!tnSubSubNode.Checked && tnSubSubNode.Tag != null) objectsToRemove.Add((S_SQLiteSyncS6x)tnSubSubNode.Tag);
                            }
                        }
                    }
                }
            }
            tnSyncSide = null;

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void syncTreeViewInit()
        {
            syncTreeView.Nodes.Clear();

            TreeNode tnNewSyncSide = null;

            tnNewSyncSide = new TreeNode();
            tnNewSyncSide.Name = SyncSideS6xName;
            tnNewSyncSide.Text = SyncSideS6xText;
            tnNewSyncSide.ToolTipText = SyncSideS6xToolTipText;
            tnNewSyncSide.Checked = true;
            syncTreeView.Nodes.Add(tnNewSyncSide);

            tnNewSyncSide = new TreeNode();
            tnNewSyncSide.Name = SyncSideDb806xName;
            tnNewSyncSide.Text = SyncSideDb806xText;
            tnNewSyncSide.ToolTipText = SyncSideDb806xToolTipText;
            tnNewSyncSide.Checked = true;
            syncTreeView.Nodes.Add(tnNewSyncSide);

            tnNewSyncSide = null;

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
                        foreach (TreeNode tnSyncSide in syncTreeView.Nodes)
                        {
                            TreeNode tnParent = new TreeNode();
                            tnParent.Name = tnSyncSide.Name + "." + tnMainParent.Name;
                            tnParent.Text = S6xNav.getHeaderCategLabel(S6xNav.getHeaderCateg(tnMainParent.Name));
                            tnParent.ToolTipText = tnMainParent.ToolTipText;
                            tnParent.StateImageKey = tnMainParent.StateImageKey;
                            tnParent.Checked = true;
                            tnSyncSide.Nodes.Add(tnParent);
                        }
                        break;
                    default:
                        continue;
                }
            }
        }

        private void syncTreeViewCount()
        {
            List<TreeNode> sidesToRemove = new List<TreeNode>();
            foreach (TreeNode tnSyncSide in syncTreeView.Nodes)
            {
                List<TreeNode> nodesToRemove = new List<TreeNode>();
                foreach (TreeNode tnParent in tnSyncSide.Nodes)
                {
                    if (tnParent.Nodes.Count == 0) nodesToRemove.Add(tnParent);
                    else tnParent.Text = S6xNav.getHeaderCategLabel(tnParent.Name.Remove(0, tnSyncSide.Name.Length + 1)) + " (" + tnParent.Nodes.Count.ToString() + ")";
                }
                foreach (TreeNode tnParent in nodesToRemove) tnSyncSide.Nodes.Remove(tnParent);

                if (tnSyncSide.Nodes.Count == 0) sidesToRemove.Add(tnSyncSide);
            }

            foreach (TreeNode tnSyncSide in sidesToRemove) syncTreeView.Nodes.Remove(tnSyncSide);
            sidesToRemove = null;

            foreach (TreeNode tnSyncSide in syncTreeView.Nodes) tnSyncSide.Expand();
        }
    }
}
