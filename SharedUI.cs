using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SAD806x
{
    // Main Form Processes
    public enum ProcessType
    {
        None,
        Load,
        Disassemble,
        Output,
        ProcessManager
    }

    public enum S6xNavHeaderCategory
    {
        UNDEFINED,
        PROPERTIES,
        RESERVED,
        TABLES,
        FUNCTIONS,
        SCALARS,
        STRUCTURES,
        ROUTINES,
        OPERATIONS,
        REGISTERS,
        OTHER,
        SIGNATURES,
        ELEMSSIGNATURES
    }

    public enum S6xNavCategoryLevel
    {
        ONE,
        TWO,
        THREE
    }

    // Main Form Navigation Information based on TreeNodes
    public class S6xNavCategory
    {
        private string name = string.Empty;

        public string Name { get { return name; } }
        public string Key { get { return name.ToUpper() + "_S6xNC"; } }

        public S6xNavCategory(string sName)
        {
            name = sName;
        }

        public override string ToString()
        {
            return name;
        }

    }

    public class S6xNavCategories
    {
        private SortedList<S6xNavHeaderCategory, SortedList<string, S6xNavCategory>> slLists1 = null;
        private SortedList<S6xNavHeaderCategory, SortedList<string, S6xNavCategory>> slLists2 = null;
        private SortedList<S6xNavHeaderCategory, SortedList<string, S6xNavCategory>> slLists3 = null;

        public S6xNavCategories()
        {
            slLists1 = new SortedList<S6xNavHeaderCategory, SortedList<string, S6xNavCategory>>();
            slLists2 = new SortedList<S6xNavHeaderCategory, SortedList<string, S6xNavCategory>>();
            slLists3 = new SortedList<S6xNavHeaderCategory, SortedList<string, S6xNavCategory>>();
            foreach (S6xNavHeaderCategory headerCategory in Enum.GetValues(typeof(S6xNavHeaderCategory)))
            {
                slLists1.Add(headerCategory, new SortedList<string, S6xNavCategory>());
                slLists2.Add(headerCategory, new SortedList<string, S6xNavCategory>());
                slLists3.Add(headerCategory, new SortedList<string, S6xNavCategory>());
            }
        }

        public void resetCategory(S6xNavHeaderCategory headerCategory, S6xNavCategoryLevel categoryLevel)
        {
            if (categoryLevel == S6xNavCategoryLevel.ONE) slLists1[headerCategory] = new SortedList<string, S6xNavCategory>();
            else if (categoryLevel == S6xNavCategoryLevel.TWO) slLists2[headerCategory] = new SortedList<string, S6xNavCategory>();
            else if (categoryLevel == S6xNavCategoryLevel.THREE) slLists3[headerCategory] = new SortedList<string, S6xNavCategory>();
        }

        public void addCategory(S6xNavHeaderCategory headerCategory, S6xNavCategoryLevel categoryLevel, string categoryName)
        {
            S6xNavCategory newNICateg = new S6xNavCategory(categoryName);
            if (categoryLevel == S6xNavCategoryLevel.ONE)
            {
                if (!slLists1[headerCategory].ContainsKey(newNICateg.Key)) slLists1[headerCategory].Add(newNICateg.Key, newNICateg);
            }
            else if (categoryLevel == S6xNavCategoryLevel.TWO)
            {
                if (!slLists2[headerCategory].ContainsKey(newNICateg.Key)) slLists2[headerCategory].Add(newNICateg.Key, newNICateg);
            }
            else if (categoryLevel == S6xNavCategoryLevel.THREE)
            {
                if (!slLists3[headerCategory].ContainsKey(newNICateg.Key)) slLists3[headerCategory].Add(newNICateg.Key, newNICateg);
            }
        }

        public S6xNavCategory getCategory(S6xNavHeaderCategory headerCategory, S6xNavCategoryLevel categoryLevel, bool globalIncluded, string categoryName)
        {
            if (categoryName == null || categoryName == string.Empty) return null;

            try
            {
                return getCategories(headerCategory, categoryLevel, globalIncluded)[(new S6xNavCategory(categoryName)).Key];
            }
            catch
            {
                return null;
            }
        }
        
        public SortedList<string, S6xNavCategory> getCategories(S6xNavHeaderCategory headerCategory, S6xNavCategoryLevel categoryLevel, bool globalIncluded)
        {
            if (!globalIncluded)
            {
                if (categoryLevel == S6xNavCategoryLevel.ONE) return slLists1[headerCategory];
                else if (categoryLevel == S6xNavCategoryLevel.TWO) return slLists2[headerCategory];
                else if (categoryLevel == S6xNavCategoryLevel.THREE) return slLists3[headerCategory];
            }

            SortedList<string, S6xNavCategory> slResultList = null;
            if (categoryLevel == S6xNavCategoryLevel.ONE)
            {
                slResultList = new SortedList<string, S6xNavCategory>(slLists1[headerCategory]);
                foreach (S6xNavCategory gbNICateg in slLists1[S6xNavHeaderCategory.UNDEFINED].Values)
                {
                    if (!slResultList.ContainsKey(gbNICateg.Key)) slResultList.Add(gbNICateg.Key, gbNICateg);
                }
            }
            else if (categoryLevel == S6xNavCategoryLevel.TWO)
            {
                slResultList = new SortedList<string, S6xNavCategory>(slLists2[headerCategory]);
                foreach (S6xNavCategory gbNICateg in slLists2[S6xNavHeaderCategory.UNDEFINED].Values)
                {
                    if (!slResultList.ContainsKey(gbNICateg.Key)) slResultList.Add(gbNICateg.Key, gbNICateg);
                }
            }
            else if (categoryLevel == S6xNavCategoryLevel.THREE)
            {
                slResultList = new SortedList<string, S6xNavCategory>(slLists3[headerCategory]);
                foreach (S6xNavCategory gbNICateg in slLists3[S6xNavHeaderCategory.UNDEFINED].Values)
                {
                    if (!slResultList.ContainsKey(gbNICateg.Key)) slResultList.Add(gbNICateg.Key, gbNICateg);
                }
            }
            return slResultList;
        }

        public int getCategoriesCount(S6xNavHeaderCategory headerCategory, S6xNavCategoryLevel categoryLevel, bool globalIncluded)
        {
            int iCount = 0;

            if (categoryLevel == S6xNavCategoryLevel.ONE) iCount = slLists1[headerCategory].Count + (globalIncluded ? slLists1[S6xNavHeaderCategory.UNDEFINED].Count : 0);
            else if (categoryLevel == S6xNavCategoryLevel.TWO) iCount = slLists2[headerCategory].Count + (globalIncluded ? slLists2[S6xNavHeaderCategory.UNDEFINED].Count : 0);
            else if (categoryLevel == S6xNavCategoryLevel.THREE) iCount = slLists3[headerCategory].Count + (globalIncluded ? slLists3[S6xNavHeaderCategory.UNDEFINED].Count : 0);

            return iCount;
        }
    }
    
    public class S6xNavInfo
    {
        private TreeNode tnNode = null;                     // Related Node, has always its copy on other declared nodes

        private bool bIsValid = false;
        private bool bIsHeaderCategory = false;
        private bool bIsCategory = false;
        private bool bIsCaterory2 = false;
        private bool bIsCaterory3 = false;
        private bool bIsElement = false;
        private bool bIsDuplicate = false;

        private TreeNode tnHeaderCategory = null;           // HeaderCategory Node, even if isHeaderCategory
        private TreeNode tnCategory = null;                 // Category Node, even if isCategory
        private TreeNode tnCategory2 = null;                // Category2 Node, even if isCategory2
        private TreeNode tnCategory3 = null;                // Category3 Node, even if isCategory3
        private TreeNode tnMain = null;                     // Main Element Node, even if isElement
        private TreeNode tnDuplicate = null;                // Duplicate Element Node, only if isDuplicate
        
        public TreeNode Node { get { return tnNode; } }

        public TreeNode HeaderCategoryNode { get { return tnHeaderCategory; } }
        public TreeNode CategoryNode { get { return tnCategory; } }
        public TreeNode Category2Node { get { return tnCategory2; } }
        public TreeNode Category3Node { get { return tnCategory3; } }
        public TreeNode MainNode { get { return tnMain; } }
        public TreeNode DuplicateNode { get { return tnDuplicate; } }

        public TreeNodeCollection Nodes { get { return tnHeaderCategory.Nodes; } }
        public TreeNodeCollection DirectNodes
        {
            get
            {
                if (DuplicateNode != null) return DuplicateNode.Nodes; 
                else if (MainNode != null) return MainNode.Nodes;
                else if (Category3Node != null) return Category3Node.Nodes;
                else if (Category2Node != null) return Category2Node.Nodes;
                else if (CategoryNode != null) return CategoryNode.Nodes;
                else return HeaderCategoryNode.Nodes;
            } 
        }
        public TreeNodeCollection DuplicateNodes
        {
            get
            {
                if (DuplicateNode != null) return DuplicateNode.Nodes;
                else if (MainNode != null) return MainNode.Nodes;
                else return (new TreeNode()).Nodes;
            }
        }

        public List<TreeNode> DirectCategoryNodes
        {
            get
            {
                List<TreeNode> lList = new List<TreeNode>();

                if (DuplicateNode != null) return lList;
                else if (MainNode != null) return lList;
                else if (Category3Node != null) return lList;
                
                foreach (TreeNode tnNode in DirectNodes)
                {
                    S6xNavInfo navInfo = new S6xNavInfo(tnNode);
                    if (navInfo.isCategory || navInfo.isCaterory2 || navInfo.isCaterory3) lList.Add(tnNode);
                }

                return lList;
            }
        }

        public bool isValid { get { return bIsValid; } }
        public bool isHeaderCategory { get { return bIsHeaderCategory; } }
        public bool isCategory { get { return bIsCategory; } }
        public bool isCaterory2 { get { return bIsCaterory2; } }
        public bool isCaterory3 { get { return bIsCaterory3; } }
        public bool isElement { get { return bIsElement; } }
        public bool isDuplicate { get { return bIsDuplicate; } }

        public S6xNavHeaderCategory HeaderCategory { get { return (tnHeaderCategory == null) ? S6xNavHeaderCategory.UNDEFINED : S6xNav.getHeaderCateg(tnHeaderCategory.Name); } }
        public string HeaderCategoryName { get { return (tnHeaderCategory == null) ? Enum.GetName(typeof(S6xNavHeaderCategory), S6xNavHeaderCategory.UNDEFINED) : tnHeaderCategory.Name; } }

        public S6xNavCategory Category { get { return (tnCategory == null ? null : new S6xNavCategory(tnCategory.Name)); } }
        public S6xNavCategory Category2 { get { return (tnCategory2 == null ? null : new S6xNavCategory(tnCategory2.Name)); } }
        public S6xNavCategory Category3 { get { return (tnCategory3 == null ? null : new S6xNavCategory(tnCategory3.Name)); } }

        public int ElementsCount
        {
            get
            {
                int iCount = DirectNodes.Count - DirectCategoryNodes.Count;
                foreach (TreeNode tnCateg in DirectCategoryNodes) iCount += (new S6xNavInfo(tnCateg)).ElementsCount;
                return iCount;
            }
        }
        
        public TreeNode FindElement(string nodeName)
        {
            if (DirectNodes.ContainsKey(nodeName)) return DirectNodes[nodeName];

            foreach (TreeNode tnCategoryNode in DirectCategoryNodes)
            {
                S6xNavInfo niCateg = new S6xNavInfo(tnCategoryNode);
                TreeNode tnResult = niCateg.FindElement(nodeName);
                if (tnResult != null) return tnResult;
            }

            return null;
        }

        public TreeNode FindElementDuplicate(string nodeName, string nodeDuplicateName)
        {
            TreeNode tnElement = FindElement(nodeName);
            if (tnElement != null) return tnElement.Nodes[nodeDuplicateName];
            return null;
        }

        public void AddNode(TreeNode tnNode, S6xNavCategory navCateg1, S6xNavCategory navCateg2, S6xNavCategory navCateg3, bool isNodeDuplicate)
        {
            if (isNodeDuplicate)
            {
                if (MainNode != null)
                {
                    if (!MainNode.Nodes.ContainsKey(tnNode.Name)) MainNode.Nodes.Add(tnNode);
                }
                return;
            }

            if (MainNode != null) return;

            TreeNode tnRightCateg = HeaderCategoryNode;
            TreeNode tnCateg = null;

            if (navCateg1 != null)
            {
                tnCateg = tnRightCateg.Nodes[navCateg1.Key];
                if (tnCateg == null)
                {
                    tnCateg = new TreeNode();
                    tnCateg.Name = navCateg1.Key;
                    tnCateg.Text = navCateg1.Name;
                    tnCateg.Tag = navCateg1;
                    tnRightCateg.Nodes.Add(tnCateg);
                }
                tnRightCateg = tnCateg;

                if (navCateg2 != null)
                {
                    tnCateg = tnRightCateg.Nodes[navCateg2.Key];
                    if (tnCateg == null)
                    {
                        tnCateg = new TreeNode();
                        tnCateg.Name = navCateg2.Key;
                        tnCateg.Text = navCateg2.Name;
                        tnCateg.Tag = navCateg2;
                        tnRightCateg.Nodes.Add(tnCateg);
                    }
                    tnRightCateg = tnCateg;

                    if (navCateg3 != null)
                    {
                        tnCateg = tnRightCateg.Nodes[navCateg3.Key];
                        if (tnCateg == null)
                        {
                            tnCateg = new TreeNode();
                            tnCateg.Name = navCateg3.Key;
                            tnCateg.Text = navCateg3.Name;
                            tnCateg.Tag = navCateg3;
                            tnRightCateg.Nodes.Add(tnCateg);
                        }
                        tnRightCateg = tnCateg;
                    }
                }
            }

            tnRightCateg.Nodes.Add(tnNode);
        }

        public S6xNavInfo(TreeNode nNode)
        {
            bool bIdentified = false;

            tnNode = nNode;

            if (tnNode == null) return;
            if (tnNode.TreeView == null) return;
            bIsValid = true;

            if (tnNode.Level == 0)
            {
                tnHeaderCategory = tnNode;
                bIsHeaderCategory = true;
            }
            else if (tnNode.Level == 1)
            {
                tnHeaderCategory = tnNode.Parent;
                if (tnNode.Tag != null)
                {
                    if (tnNode.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory = tnNode;
                        bIsCategory = true;
                        bIdentified = true;
                    }
                }
                if (!bIdentified)
                {
                    tnMain = tnNode;
                    bIsElement = true;
                }
            }
            else if (tnNode.Level == 2)
            {
                tnHeaderCategory = tnNode.Parent.Parent;
                if (tnNode.Parent.Tag != null)
                {
                    if (tnNode.Parent.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory = tnNode.Parent;
                    }
                }
                if (tnNode.Tag != null)
                {
                    if (tnNode.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory2 = tnNode;
                        bIsCategory = true;
                        bIdentified = true;
                    }
                }
                if (!bIdentified)
                {
                    if (tnCategory == null)
                    {
                        tnMain = tnNode.Parent;
                        tnDuplicate = tnNode;
                        bIsDuplicate = true;
                    }
                    else
                    {
                        tnMain = tnNode;
                        bIsElement = true;
                    }
                }
            }
            else if (tnNode.Level == 3)
            {
                tnHeaderCategory = tnNode.Parent.Parent.Parent;
                if (tnNode.Parent.Parent.Tag != null)
                {
                    if (tnNode.Parent.Parent.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory = tnNode.Parent.Parent;
                    }
                }
                if (tnNode.Parent.Tag != null)
                {
                    if (tnNode.Parent.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory2 = tnNode.Parent;
                    }
                }
                if (tnNode.Tag != null)
                {
                    if (tnNode.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory3 = tnNode;
                        bIsCategory = true;
                        bIdentified = true;
                    }
                }
                if (!bIdentified)
                {
                    if (tnCategory2 == null)
                    {
                        tnMain = tnNode.Parent;
                        tnDuplicate = tnNode;
                        bIsDuplicate = true;
                    }
                    else
                    {
                        tnMain = tnNode;
                        bIsElement = true;
                    }
                }
            }
            else if (tnNode.Level == 4)
            {
                tnHeaderCategory = tnNode.Parent.Parent.Parent.Parent;
                if (tnNode.Parent.Parent.Parent.Tag != null)
                {
                    if (tnNode.Parent.Parent.Parent.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory = tnNode.Parent.Parent.Parent;
                    }
                }
                if (tnNode.Parent.Parent.Tag != null)
                {
                    if (tnNode.Parent.Parent.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory2 = tnNode.Parent.Parent;
                    }
                }
                if (tnNode.Parent.Tag != null)
                {
                    if (tnNode.Parent.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory3 = tnNode.Parent;
                    }
                }
                if (tnCategory3 == null)
                {
                    tnMain = tnNode.Parent;
                    tnDuplicate = tnNode;
                    bIsDuplicate = true;
                }
                else
                {
                    tnMain = tnNode;
                    bIsElement = true;
                }
            }
            else if (tnNode.Level == 4)
            {
                tnHeaderCategory = tnNode.Parent.Parent.Parent.Parent.Parent;
                if (tnNode.Parent.Parent.Parent.Parent.Tag != null)
                {
                    if (tnNode.Parent.Parent.Parent.Parent.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory = tnNode.Parent.Parent.Parent.Parent;
                    }
                }
                if (tnNode.Parent.Parent.Parent.Tag != null)
                {
                    if (tnNode.Parent.Parent.Parent.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory2 = tnNode.Parent.Parent.Parent;
                    }
                }
                if (tnNode.Parent.Parent.Tag != null)
                {
                    if (tnNode.Parent.Parent.Tag.GetType() == typeof(S6xNavCategory))
                    {
                        tnCategory3 = tnNode.Parent.Parent;
                    }
                }
                tnMain = tnNode.Parent;
                tnDuplicate = tnNode;
                bIsDuplicate = true;
            }
        }
    }

    public static class S6xNav
    {
        public static S6xNavHeaderCategory getHeaderCateg(string sName)
        {
            try { return (S6xNavHeaderCategory)Enum.Parse(typeof(S6xNavHeaderCategory), sName); }
            catch { return S6xNavHeaderCategory.UNDEFINED; }
        }

        public static string getHeaderCategName(S6xNavHeaderCategory hCateg)
        {
            return Enum.GetName(typeof(S6xNavHeaderCategory), hCateg);
        }

        public static string getHeaderCategLabel(string sName)
        {
            return getHeaderCategLabel(getHeaderCateg(sName));
        }

        public static string getHeaderCategLabel(S6xNavHeaderCategory hCateg)
        {
            switch (hCateg)
            {
                case S6xNavHeaderCategory.PROPERTIES:
                    return "Properties";
                case S6xNavHeaderCategory.RESERVED:
                    return "Reserved";
                case S6xNavHeaderCategory.TABLES:
                    return "Tables";
                case S6xNavHeaderCategory.FUNCTIONS:
                    return "Functions";
                case S6xNavHeaderCategory.SCALARS:
                    return "Scalars";
                case S6xNavHeaderCategory.STRUCTURES:
                    return "Structures";
                case S6xNavHeaderCategory.ROUTINES:
                    return "Routines";
                case S6xNavHeaderCategory.OPERATIONS:
                    return "Operations";
                case S6xNavHeaderCategory.REGISTERS:
                    return "Registers";
                case S6xNavHeaderCategory.OTHER:
                    return "Other Addresses";
                case S6xNavHeaderCategory.SIGNATURES:
                    return "Routines Signatures";
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    return "Elements Signatures";
                default:
                    return string.Empty;
            }
        }

        public static string getHeaderCategToolTip(S6xNavHeaderCategory hCateg)
        {
            switch (hCateg)
            {
                case S6xNavHeaderCategory.PROPERTIES:
                    return "Properties";
                case S6xNavHeaderCategory.RESERVED:
                    return "Reserved Elements";
                case S6xNavHeaderCategory.TABLES:
                    return "Tables";
                case S6xNavHeaderCategory.FUNCTIONS:
                    return "Functions";
                case S6xNavHeaderCategory.SCALARS:
                    return "Scalars";
                case S6xNavHeaderCategory.STRUCTURES:
                    return "Structures";
                case S6xNavHeaderCategory.ROUTINES:
                    return "Routines";
                case S6xNavHeaderCategory.OPERATIONS:
                    return "Operations";
                case S6xNavHeaderCategory.REGISTERS:
                    return "Registers";
                case S6xNavHeaderCategory.OTHER:
                    return "Other Addresses";
                case S6xNavHeaderCategory.SIGNATURES:
                    return "Routines Signatures";
                case S6xNavHeaderCategory.ELEMSSIGNATURES:
                    return "Elements Signatures";
                default:
                    return string.Empty;
            }
        }


    }

    public static class SharedUI
    {
        public static string About()
        {
            string sAbout = Application.ProductName + " " + Application.ProductVersion + "\t\t\tby Pym\n\r\n\r";
            sAbout += "Purpose:\n\r";
            sAbout += "\t- To disassemble 8061/8065 roms\n\r";
            sAbout += "\t- To do it automatically or semi-automatically\n\r";
            sAbout += "\t- To generate disassembly outputs in multiple formats\n\r";
            sAbout += "\n\rInformation:\n\r";
            sAbout += "\t- Disassembly can generate conflicts on Operations and Elements.";
            sAbout += " They will appear in output and error messages, to be managed manually.";
            sAbout += " For Operations they are essentially related with embedded arguments in calls.";
            sAbout += " For Elements they are essentially related to multiple type usage.\n\r";
            sAbout += "\n\rKnown issues:\n\r";
            sAbout += "\t- To be discovered\n\r";
            sAbout += "\n\rThanks to Andy (tvrfan) for SAD, software used as template for initial output, to Mark Mansur for TunerPro, which permits to continue working generated data.\n\r";
            sAbout += "\t\t\t\t\tPym\n\r";
            sAbout += "\t\t\t\t\t" + Properties.Settings.Default.VersionDate;

            return sAbout;
        }

        public static string AboutRoutinesComparisonSkeletons()
        {
            string sAbout = "Routines comparison principle:\n\r\n\r";
            sAbout += "Purpose:\n\r";
            sAbout += "- To match routines between two disassembled binaries\n\r";
            sAbout += "\n\rSkeletons generation Method:\n\r";
            sAbout += "- Skeletons file (.skt) has to be generated after disassembly.\n\r";
            sAbout += "- Routine start and end are here based only on jumps and returns operations.\n\r";
            sAbout += "- Skeleton of routine is generated base on Operations codes.\n\r";
            sAbout += "\n\rSkeletons files comparison Method:\n\r";
            sAbout += "- Two different Skeletons files (.skt) have to be selected.\n\r";
            sAbout += "- Based on routine size (Minimum Operations Count), routine can be ignored.\n\r";
            sAbout += "- Based on routines size difference (Operations Count Gap Maximum Tolerance %), matching can be ignored.\n\r";
            sAbout += "- Based on routines skeletons proximity (Damerau Levenshtein Distance Minimum Tolerance %), matching can be ignored.\n\r";
            sAbout += "- Reports will be outputted, including possible matches with % chance.\n\r";
            sAbout += "- One report for each way.";

            return sAbout;
        }
    
        public static string StructureTip()
        {
            string sMessage = string.Empty;
            sMessage = "Structure Options\n\n";
            sMessage += "Data Types:\n";
            sMessage += "- Decimal: Byte, Word, SByte (Signed Byte), SWord (Signed Word)\n";
            sMessage += "- Hexadecimal: ByteHex (Lowered), WordHex(Lowered), Hex, HexLsb (Lsb First 2 by 2)\n";
            sMessage += "- Other: Skip, Ascii, \"String\", Vect8, Vect1, Vect9, Vect0, Empty, Num, NumHex\n";
            sMessage += "Special Keys:\n";
            sMessage += "- Carrier Returns: \"\\n\"\n";
            sMessage += "Basic tests(on byte position in structure line):\n";
            sMessage += "- B0 (B0=1) to B7 (B7=1), !B0 (B0=0) to !B7 (B7=0), 00, !00, FF, !FF\n";

            sMessage += "\nSample:\n";
            sMessage += "Byte:2,Word,Hex:4,Ascii:8\n";
            sMessage += "Hex,Byte:8\n";
            sMessage += "Word:2,Byte\n";
            sMessage += "If(B0:2) {\n";
            sMessage += "Byte:2,Word\n";
            sMessage += "} Else {\n";
            sMessage += "Byte:2,Word:2 }\n";

            return sMessage;
        }

        public static string SignatureTip()
        {
            string sMessage = string.Empty;
            sMessage = "Routine Signature Options\n\n";
            sMessage += "Format:\n";
            sMessage += "- Bytes (00 - FF), Spaces, Comma ',' and carrier returns can be used\n";
            sMessage += "- Dot '.' means one unknown half byte, '*' means 0 to 100 unknown half bytes\n";
            sMessage += "- Parameters start and end with '#', one Parameter per Byte\n";
            sMessage += "- For proper address matching, signature should always provide complete bytes\n";
            sMessage += "Principle:\n";
            sMessage += "- Signature matching is based on string regular expression comparison\n";
            sMessage += "- ab,cd,ef will be searched and found in 00FFCEABCDEF00FF\n";
            sMessage += "Using Parameters:\n";
            sMessage += "- Parameters will be reused for routine or internal elements generation\n";
            sMessage += "- Predefined fields can reuse them like this : #01# or #01##02# or #01#+#02#\n";
            sMessage += "Purpose:\n";
            sMessage += "- Signature has to properly generate the related routine\n";

            return sMessage;
        }

        public static string ElementSignatureTip()
        {
            string sMessage = string.Empty;
            sMessage = " Element Signature Options \n\n";
            sMessage += "Format:\n";
            sMessage += "- Bytes (00 - FF), Spaces, Comma ',' and carrier returns can be used\n";
            sMessage += "- Dot '.' means one unknown half byte, '*' is not authorized for this type of signatures\n";
            sMessage += "- '#EAOP#' means operation using(including) element address and should be used.\n";
            sMessage += "   It will be replaced by identified operation in signature.\n";
            sMessage += "- For proper address matching, signature should always provide complete bytes\n";
            sMessage += "Principle:\n";
            sMessage += "- Signature matching is based on string regular expression comparison\n";
            sMessage += "- ab,cd,ef will be searched and found in 00FFCEABCDEF00FF\n";
            sMessage += "Purpose:\n";
            sMessage += "- Signature has to properly match the operations near element use.\n";

            return sMessage;
        }
    }
}
