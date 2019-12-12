using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;

namespace SAD806x
{
    public class SADS6x
    {
        private string s6xFilePath = string.Empty;
        private string s6xFileName = string.Empty;

        private bool valid = false;

        public S6xProperties Properties = null;
        
        public SortedList slTables = null;
        public SortedList slFunctions = null;
        public SortedList slScalars = null;
        public SortedList slStructures = null;
        public SortedList slRoutines = null;
        public SortedList slOperations = null;
        public SortedList slRegisters = null;
        public SortedList slOtherAddresses = null;
        public SortedList slSignatures = null;
        public SortedList slElementsSignatures = null;

        public SortedList slDupTables = null;
        public SortedList slDupFunctions = null;
        public SortedList slDupScalars = null;
        public SortedList slDupStructures = null;

        public SortedList slProcessTables = null;
        public SortedList slProcessFunctions = null;
        public SortedList slProcessScalars = null;
        public SortedList slProcessStructures = null;
        public SortedList slProcessRoutines = null;
        public SortedList slProcessOperations = null;
        public SortedList slProcessRegisters = null;
        public SortedList slProcessOtherAddresses = null;
        public SortedList slProcessSignatures = null;
        public SortedList slProcessElementsSignatures = null;

        public string FilePath { get { return s6xFilePath; } }
        public string FileName { get { return s6xFileName; } }

        public bool isValid { get { return valid; } }

        public bool isSaved = false;

        public SADS6x(string filePath)
        {
            Properties = new S6xProperties();
            
            slTables = new SortedList();
            slFunctions = new SortedList();
            slScalars = new SortedList();
            slStructures = new SortedList();
            slRoutines = new SortedList();
            slOperations = new SortedList();
            slRegisters = new SortedList();
            slOtherAddresses = new SortedList();
            slSignatures = new SortedList();
            slElementsSignatures = new SortedList();

            slDupTables = new SortedList();
            slDupFunctions = new SortedList();
            slDupScalars = new SortedList();
            slDupStructures = new SortedList();

            s6xFilePath = filePath;
            if (File.Exists(s6xFilePath)) s6xFileName = new FileInfo(s6xFilePath).Name;
            else s6xFileName = s6xFilePath.Substring(s6xFilePath.LastIndexOf('\\') + 1);

            Load();
        }

        #region SortedList Accessors

        public string getNewSignatureUniqueKey()
        {
            int cnt = slSignatures.Count;
            string uniqueKey = string.Empty;
            while (uniqueKey == string.Empty || slSignatures.ContainsKey(uniqueKey))
            {
                uniqueKey = string.Format("S{0:d4}", cnt);
                cnt++;
            }
            return uniqueKey;
        }

        public string getNewElementSignatureUniqueKey()
        {
            int cnt = slElementsSignatures.Count;
            string uniqueKey = string.Empty;
            while (uniqueKey == string.Empty || slElementsSignatures.ContainsKey(uniqueKey))
            {
                uniqueKey = string.Format("S{0:d4}", cnt);
                cnt++;
            }
            return uniqueKey;
        }

        #endregion

        private void Load()
        {
            S6xXml s6xXml = null;

            try
            {
                valid = false;

                if (!File.Exists(FilePath))
                {
                    valid = true;
                    if (SADFixedSigs.Fixed_Elements_Signatures != null)
                    {
                        foreach (object[] elementSignature in SADFixedSigs.Fixed_Elements_Signatures)
                        {
                            S6xElementSignature s6xES = new S6xElementSignature(elementSignature);
                            slElementsSignatures.Add(s6xES.UniqueKey, s6xES);
                        }
                    }
                    return;
                }

                s6xXml = (S6xXml)ToolsXml.DeserializeFile(FilePath, typeof(S6xXml));

                Properties = s6xXml.Properties;

                if (s6xXml.S6xTables != null)
                {
                    foreach (S6xTable s6xTable in s6xXml.S6xTables)
                    {
                        s6xTable.Store = true;
                        slTables.Add(s6xTable.UniqueAddress, s6xTable);
                    }
                }
                if (s6xXml.S6xFunctions != null)
                {
                    foreach (S6xFunction s6xFunction in s6xXml.S6xFunctions)
                    {
                        s6xFunction.Store = true;
                        slFunctions.Add(s6xFunction.UniqueAddress, s6xFunction);
                    }
                }
                if (s6xXml.S6xScalars != null)
                {
                    foreach (S6xScalar s6xScalar in s6xXml.S6xScalars)
                    {
                        s6xScalar.Store = true;
                        slScalars.Add(s6xScalar.UniqueAddress, s6xScalar);
                    }
                }
                if (s6xXml.S6xStructures != null)
                {
                    foreach (S6xStructure s6xStructure in s6xXml.S6xStructures)
                    {
                        s6xStructure.Store = true;
                        slStructures.Add(s6xStructure.UniqueAddress, s6xStructure);
                    }
                }
                if (s6xXml.S6xRoutines != null)
                {
                    foreach (S6xRoutine s6xRoutine in s6xXml.S6xRoutines)
                    {
                        s6xRoutine.Store = true;
                        slRoutines.Add(s6xRoutine.UniqueAddress, s6xRoutine);
                    }
                }
                if (s6xXml.S6xOperations != null)
                {
                    foreach (S6xOperation s6xOpe in s6xXml.S6xOperations) slOperations.Add(s6xOpe.UniqueAddress, s6xOpe);
                }
                if (s6xXml.S6xRegisters != null)
                {
                    foreach (S6xRegister s6xReg in s6xXml.S6xRegisters)
                    {
                        s6xReg.Store = true;
                        slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                    }
                }
                if (s6xXml.S6xOtherAddresses != null)
                {
                    foreach (S6xOtherAddress s6xOther in s6xXml.S6xOtherAddresses) slOtherAddresses.Add(s6xOther.UniqueAddress, s6xOther);
                }
                if (s6xXml.S6xSignatures != null)
                {
                    foreach (S6xSignature s6xSig in s6xXml.S6xSignatures) slSignatures.Add(s6xSig.UniqueKey, s6xSig);
                }

                if (s6xXml.S6xElementsSignatures != null)
                {
                    foreach (S6xElementSignature s6xESig in s6xXml.S6xElementsSignatures) slElementsSignatures.Add(s6xESig.UniqueKey, s6xESig);
                }
                // Fixed Signature Mngt
                // Unicity Check to be Added !!!!
                if (SADFixedSigs.Fixed_Elements_Signatures != null)
                {
                    foreach (object[] elementSignature in SADFixedSigs.Fixed_Elements_Signatures)
                    {
                        S6xElementSignature s6xES = new S6xElementSignature(elementSignature);
                        if (!slElementsSignatures.ContainsKey(s6xES.UniqueKey)) slElementsSignatures.Add(s6xES.UniqueKey, s6xES);
                    }
                }

                // Duplicates
                if (s6xXml.S6xDupTables != null)
                {
                    foreach (S6xTable s6xTable in s6xXml.S6xDupTables)
                    {
                        s6xTable.Store = true;
                        slDupTables.Add(s6xTable.DuplicateAddress, s6xTable);
                    }
                }
                if (s6xXml.S6xDupFunctions != null)
                {
                    foreach (S6xFunction s6xFunction in s6xXml.S6xDupFunctions)
                    {
                        s6xFunction.Store = true;
                        slDupFunctions.Add(s6xFunction.DuplicateAddress, s6xFunction);
                    }
                }
                if (s6xXml.S6xDupScalars != null)
                {
                    foreach (S6xScalar s6xScalar in s6xXml.S6xDupScalars)
                    {
                        s6xScalar.Store = true;
                        slDupScalars.Add(s6xScalar.DuplicateAddress, s6xScalar);
                    }
                }
                if (s6xXml.S6xDupStructures != null)
                {
                    foreach (S6xStructure s6xStructure in s6xXml.S6xDupStructures)
                    {
                        s6xStructure.Store = true;
                        slDupStructures.Add(s6xStructure.DuplicateAddress, s6xStructure);
                    }
                }

                valid = true;
                isSaved = true;
            }
            catch
            {
                valid = false;
            }
            finally
            {
                s6xXml = null;
                GC.Collect();
            }
        }

        public bool Save()
        {
            XmlDocument xDoc = null;

            try
            {
                if (isSaved) return true;

                xDoc = new XmlDocument();
                generateXDoc(ref xDoc);
                xDoc.Save(FilePath);

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

        public void processS6xInit()
        {
            slProcessTables = new SortedList();
            slProcessFunctions = new SortedList();
            slProcessScalars = new SortedList();
            slProcessStructures = new SortedList();
            slProcessRoutines = new SortedList();
            slProcessOperations = new SortedList();
            slProcessRegisters = new SortedList();
            slProcessOtherAddresses = new SortedList();
            slProcessSignatures = new SortedList();
            slProcessElementsSignatures = new SortedList();

            foreach (S6xTable s6xObject in slTables.Values) if (s6xObject.Store && !s6xObject.Skip) slProcessTables.Add(s6xObject.UniqueAddress, s6xObject);
            foreach (S6xFunction s6xObject in slFunctions.Values) if (s6xObject.Store && !s6xObject.Skip) slProcessFunctions.Add(s6xObject.UniqueAddress, s6xObject);
            foreach (S6xScalar s6xObject in slScalars.Values) if (s6xObject.Store && !s6xObject.Skip) slProcessScalars.Add(s6xObject.UniqueAddress, s6xObject);
            foreach (S6xStructure s6xObject in slStructures.Values) if (s6xObject.Store && !s6xObject.Skip) slProcessStructures.Add(s6xObject.UniqueAddress, s6xObject);
            foreach (S6xRoutine s6xObject in slRoutines.Values) if (s6xObject.Store && !s6xObject.Skip) slProcessRoutines.Add(s6xObject.UniqueAddress, s6xObject);
            foreach (S6xOperation s6xObject in slOperations.Values) if (!s6xObject.Skip) slProcessOperations.Add(s6xObject.UniqueAddress, s6xObject);
            foreach (S6xRegister s6xObject in slRegisters.Values) if (s6xObject.Store && !s6xObject.Skip) slProcessRegisters.Add(s6xObject.UniqueAddress, s6xObject);
            foreach (S6xOtherAddress s6xObject in slOtherAddresses.Values) if (!s6xObject.Skip) slProcessOtherAddresses.Add(s6xObject.UniqueAddress, s6xObject);
            foreach (S6xSignature s6xObject in slSignatures.Values) if (!s6xObject.Skip) slProcessSignatures.Add(s6xObject.UniqueKey, s6xObject);

            foreach (S6xElementSignature s6xObject in slElementsSignatures.Values)
            {
                if (!s6xObject.Skip)
                {
                    s6xObject.Found = false;
                    slProcessElementsSignatures.Add(s6xObject.UniqueKey, s6xObject);
                }
            }
        }

        // Conflict check at Process level, when creating Element
        //  S6x object presence permits to override Element Type
        public bool isS6xProcessTypeConflict(string uniqueAddress, Type type)
        {
            switch (type.Name)
            {
                case "Table":
                    if (slProcessTables.ContainsKey(uniqueAddress)) return false;
                    else if (slProcessStructures.ContainsKey(uniqueAddress)) return true;
                    else if (slProcessFunctions.ContainsKey(uniqueAddress)) return true;
                    else if (slProcessScalars.ContainsKey(uniqueAddress)) return true;
                    break;
                case "Function":
                    if (slProcessFunctions.ContainsKey(uniqueAddress)) return false;
                    else if (slProcessStructures.ContainsKey(uniqueAddress)) return true;
                    else if (slProcessTables.ContainsKey(uniqueAddress)) return true;
                    else if (slProcessScalars.ContainsKey(uniqueAddress)) return true;
                    break;
                case "Scalar":
                    if (slProcessScalars.ContainsKey(uniqueAddress)) return false;
                    else if (slProcessStructures.ContainsKey(uniqueAddress)) return true;
                    else if (slProcessTables.ContainsKey(uniqueAddress)) return true;
                    else if (slProcessFunctions.ContainsKey(uniqueAddress)) return true;
                    break;
                case "Structure":
                    if (slProcessStructures.ContainsKey(uniqueAddress)) return false;
                    else if (slProcessTables.ContainsKey(uniqueAddress)) return true;
                    else if (slProcessFunctions.ContainsKey(uniqueAddress)) return true;
                    else if (slProcessScalars.ContainsKey(uniqueAddress)) return true;
                    break;
            }
            return false;
        }
        
        private void generateXDoc(ref XmlDocument xDoc)
        {
            S6xXml s6xXml = null;
            XPathNavigator xNav = null;
            ArrayList alTables = null;
            ArrayList alFunctions = null;
            ArrayList alScalars = null;
            ArrayList alStructures = null;
            ArrayList alRoutines = null;
            ArrayList alRegisters = null;
            ArrayList alOtherAddresses = null;

            s6xXml = new S6xXml();
            s6xXml.version = "1.00";
            s6xXml.generated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            s6xXml.Properties = Properties;

            alTables = new ArrayList();
            foreach (S6xTable s6xObject in slTables.Values) if (s6xObject.Store) alTables.Add(s6xObject);
            s6xXml.S6xTables = new S6xTable[alTables.Count];
            alTables.CopyTo(s6xXml.S6xTables);
            alTables = null;

            alFunctions = new ArrayList();
            foreach (S6xFunction s6xObject in slFunctions.Values) if (s6xObject.Store) alFunctions.Add(s6xObject);
            s6xXml.S6xFunctions = new S6xFunction[alFunctions.Count];
            alFunctions.CopyTo(s6xXml.S6xFunctions);
            alFunctions = null;

            alScalars = new ArrayList();
            foreach (S6xScalar s6xObject in slScalars.Values) if (s6xObject.Store) alScalars.Add(s6xObject);
            s6xXml.S6xScalars = new S6xScalar[alScalars.Count];
            alScalars.CopyTo(s6xXml.S6xScalars);
            alScalars = null;

            alStructures = new ArrayList();
            foreach (S6xStructure s6xObject in slStructures.Values) if (s6xObject.Store) alStructures.Add(s6xObject);
            s6xXml.S6xStructures = new S6xStructure[alStructures.Count];
            alStructures.CopyTo(s6xXml.S6xStructures);
            alStructures = null;

            alRoutines = new ArrayList();
            foreach (S6xRoutine s6xObject in slRoutines.Values) if (s6xObject.Store) alRoutines.Add(s6xObject);
            s6xXml.S6xRoutines = new S6xRoutine[alRoutines.Count];
            alRoutines.CopyTo(s6xXml.S6xRoutines);
            alRoutines = null;

            s6xXml.S6xOperations = new S6xOperation[slOperations.Count];
            slOperations.Values.CopyTo(s6xXml.S6xOperations, 0);

            alRegisters = new ArrayList();
            foreach (S6xRegister s6xObject in slRegisters.Values) if (s6xObject.Store) alRegisters.Add(s6xObject);
            s6xXml.S6xRegisters = new S6xRegister[alRegisters.Count];
            alRegisters.CopyTo(s6xXml.S6xRegisters);
            alRegisters = null;

            alOtherAddresses = new ArrayList();
            foreach (S6xOtherAddress s6xObject in slOtherAddresses.Values) alOtherAddresses.Add(s6xObject);
            s6xXml.S6xOtherAddresses = new S6xOtherAddress[alOtherAddresses.Count];
            alOtherAddresses.CopyTo(s6xXml.S6xOtherAddresses);
            alOtherAddresses = null;

            s6xXml.S6xSignatures = new S6xSignature[slSignatures.Count];
            slSignatures.Values.CopyTo(s6xXml.S6xSignatures, 0);

            s6xXml.S6xElementsSignatures = new S6xElementSignature[slElementsSignatures.Count];
            slElementsSignatures.Values.CopyTo(s6xXml.S6xElementsSignatures, 0);

            // Duplicates
            alTables = new ArrayList();
            foreach (S6xTable s6xObject in slDupTables.Values) if (s6xObject.Store) alTables.Add(s6xObject);
            s6xXml.S6xDupTables = new S6xTable[alTables.Count];
            alTables.CopyTo(s6xXml.S6xDupTables);
            alTables = null;

            alFunctions = new ArrayList();
            foreach (S6xFunction s6xObject in slDupFunctions.Values) if (s6xObject.Store) alFunctions.Add(s6xObject);
            s6xXml.S6xDupFunctions = new S6xFunction[alFunctions.Count];
            alFunctions.CopyTo(s6xXml.S6xDupFunctions);
            alFunctions = null;

            alScalars = new ArrayList();
            foreach (S6xScalar s6xObject in slDupScalars.Values) if (s6xObject.Store) alScalars.Add(s6xObject);
            s6xXml.S6xDupScalars = new S6xScalar[alScalars.Count];
            alScalars.CopyTo(s6xXml.S6xDupScalars);
            alScalars = null;

            alStructures = new ArrayList();
            foreach (S6xStructure s6xObject in slDupStructures.Values) if (s6xObject.Store) alStructures.Add(s6xObject);
            s6xXml.S6xDupStructures = new S6xStructure[alStructures.Count];
            alStructures.CopyTo(s6xXml.S6xDupStructures);
            alStructures = null;

            xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
            xNav = xDoc.CreateNavigator();
            ToolsXml.Serialize(ref xNav, s6xXml);
            
            s6xXml = null;
            xNav = null;

            GC.Collect();
        }

        public object[] readFromFileObject(ref XdfFile xdfFile, ref SADBin sadBin, ref ArrayList alReservedAddresses)
        {
            ArrayList alS6xUpdates = new ArrayList();
            ArrayList alS6xCreations = new ArrayList();
            ArrayList alDuplicates = new ArrayList();
            ArrayList alTablesConflicts = new ArrayList();
            ArrayList alFunctionsConflicts = new ArrayList();
            ArrayList alScalarsConflicts = new ArrayList();
            ArrayList alIgnored = new ArrayList();
            ArrayList alS6xDupUpdates = new ArrayList();
            ArrayList alS6xDupCreations = new ArrayList();
            SortedList slProcessed = new SortedList();

            ArrayList alFunctions = new ArrayList();
            ArrayList alTables = new ArrayList();
            ArrayList alScalars = new ArrayList();

            int xdfBaseOffset = 0;
            // 0 S6x Updates
            // 1 S6x Creations
            // 2 Duplicates
            // 3 Conflicts
            // 4 Ignored
            // 5 S6x Duplicates Updates
            // 6 S6x Duplicates Creations
            object[] arrResult = new object[7];

            if (xdfFile.xdfHeader != null)
            {
                Properties.Label = xdfFile.xdfHeader.deftitle;
                Properties.Comments = xdfFile.xdfHeader.description;
                try
                {
                    switch (xdfFile.version)
                    {
                        case "1.50":
                            Properties.XdfBaseOffset = string.Format("{0:x4}", Convert.ToInt32(xdfFile.xdfHeader.baseoffset1_50.Trim().Replace("-", "").Replace("+", "")));
                            Properties.XdfBaseOffsetSubtract = xdfFile.xdfHeader.baseoffset1_50.Contains("-");
                            break;
                        case "1.60":
                            Properties.XdfBaseOffset = string.Format("{0:x4}", Convert.ToInt32(xdfFile.xdfHeader.xdfBaseOffset.offset));
                            Properties.XdfBaseOffsetSubtract = (xdfFile.xdfHeader.xdfBaseOffset.subtract == "1");
                            break;
                    }
                }
                catch { }
            }
            xdfBaseOffset = Convert.ToInt32(Properties.XdfBaseOffset, 16);
            if (Properties.XdfBaseOffsetSubtract) xdfBaseOffset *= -1;

            // Starting with Functions for being able to manage Table Scaling after
            if (xdfFile.xdfFunctions != null)
            {
                foreach (XdfFunction xdfObject in xdfFile.xdfFunctions)
                {
                    S6xFunction s6xObject = null;
                    int bankNum = -1;
                    int address = -1;
                    int addressBin = Tools.binAddressFromXdfAddress(xdfObject.mmeMainAddress, Properties.XdfBaseOffsetInt);
                    if (sadBin.isCalibrationAddress(addressBin))
                    {
                        bankNum = sadBin.Calibration.BankNum;
                        address = addressBin - sadBin.Calibration.BankAddressBinInt;
                        s6xObject = new S6xFunction(xdfObject, bankNum, address, addressBin, true);

                        // Ignored
                        if (alReservedAddresses.Contains(s6xObject.UniqueAddress) || sadBin.Calibration.isLoadCreated(s6xObject.UniqueAddress))
                        {
                            if (!alIgnored.Contains(s6xObject.UniqueAddress)) alIgnored.Add(s6xObject.UniqueAddress);
                            continue;
                        }
                        // Conflicts
                        if (slTables.ContainsKey(s6xObject.UniqueAddress) || slScalars.ContainsKey(s6xObject.UniqueAddress) || slStructures.ContainsKey(s6xObject.UniqueAddress))
                        {
                            if (!alFunctionsConflicts.Contains(s6xObject.UniqueAddress)) alFunctionsConflicts.Add(s6xObject.UniqueAddress);
                            continue;
                        }
                        // Duplicates
                        if (slProcessed.ContainsKey(s6xObject.UniqueAddress))
                        {
                            slProcessed[s6xObject.UniqueAddress] = (int)slProcessed[s6xObject.UniqueAddress] + 1;
                            if (!alDuplicates.Contains(s6xObject.UniqueAddress)) alDuplicates.Add(s6xObject.UniqueAddress);
                        }
                        else
                        {
                            slProcessed.Add(s6xObject.UniqueAddress, 1);
                        }

                        if (slFunctions.ContainsKey(s6xObject.UniqueAddress))
                        {
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            string sLabel = ((S6xFunction)slFunctions[s6xObject.UniqueAddress]).ShortLabel;
                            if (s6xObject.ShortLabel.StartsWith("TpFc") && sLabel != null && sLabel != string.Empty) s6xObject.ShortLabel = sLabel;
                        }
                        s6xObject.DuplicateNum = -1; // For Duplicates Mngt
                        alFunctions.Add(s6xObject);
                        s6xObject = null;
                    }
                    else
                    {
                        bankNum = sadBin.getBankNum(addressBin);
                        if (bankNum >= 0)
                        {
                            switch (bankNum)
                            {
                                case 8:
                                    address = addressBin - sadBin.Bank8.AddressBinInt;
                                    break;
                                case 1:
                                    address = addressBin - sadBin.Bank1.AddressBinInt;
                                    break;
                                case 9:
                                    address = addressBin - sadBin.Bank9.AddressBinInt;
                                    break;
                                case 0:
                                    address = addressBin - sadBin.Bank0.AddressBinInt;
                                    break;
                            }
                            s6xObject = new S6xFunction(xdfObject, bankNum, address, addressBin, false);
                            // Ignored
                            if (alReservedAddresses.Contains(s6xObject.UniqueAddress) || sadBin.Calibration.isLoadCreated(s6xObject.UniqueAddress))
                            {
                                if (!alIgnored.Contains(s6xObject.UniqueAddress)) alIgnored.Add(s6xObject.UniqueAddress);
                                continue;
                            }
                            // Conflicts
                            if (slTables.ContainsKey(s6xObject.UniqueAddress) || slScalars.ContainsKey(s6xObject.UniqueAddress) || slStructures.ContainsKey(s6xObject.UniqueAddress))
                            {
                                if (!alFunctionsConflicts.Contains(s6xObject.UniqueAddress)) alFunctionsConflicts.Add(s6xObject.UniqueAddress);
                                continue;
                            }
                            // Duplicates
                            if (slProcessed.ContainsKey(s6xObject.UniqueAddress))
                            {
                                slProcessed[s6xObject.UniqueAddress] = (int)slProcessed[s6xObject.UniqueAddress] + 1;
                                if (!alDuplicates.Contains(s6xObject.UniqueAddress)) alDuplicates.Add(s6xObject.UniqueAddress);
                            }
                            else
                            {
                                slProcessed.Add(s6xObject.UniqueAddress, 1);
                            }

                            if (slFunctions.ContainsKey(s6xObject.UniqueAddress))
                            {
                                // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                                string sLabel = ((S6xFunction)slFunctions[s6xObject.UniqueAddress]).ShortLabel;
                                if (s6xObject.ShortLabel.StartsWith("TpFc") && sLabel != null && sLabel != string.Empty) s6xObject.ShortLabel = sLabel;
                            }
                            s6xObject.DuplicateNum = -1; // For Duplicates Mngt
                            alFunctions.Add(s6xObject);
                            s6xObject = null;
                        }
                    }
                }
            }

            // Duplicates Mngt through DuplicateNum calculation
            foreach (S6xFunction s6xFunction in alFunctions)
            {
                if (!slProcessed.ContainsKey(s6xFunction.UniqueAddress)) continue;
                if ((int)slProcessed[s6xFunction.UniqueAddress] == 1)
                {
                    // Unique Item
                    s6xFunction.DuplicateNum = 0;
                }
                else
                {
                    // Duplicates
                    if (s6xFunction.DuplicateNum >= 0) continue; // ALready Processed

                    ArrayList alS6x = new ArrayList();
                    if (slFunctions.ContainsKey(s6xFunction.UniqueAddress))
                    {
                        alS6x.Add(slFunctions[s6xFunction.UniqueAddress]);
                        foreach (S6xFunction s6xOtherDup in slDupFunctions.Values)
                        {
                            if (s6xOtherDup.UniqueAddress == s6xFunction.UniqueAddress) alS6x.Add(s6xOtherDup);
                        }
                    }

                    ArrayList alDup = new ArrayList();
                    foreach (S6xFunction s6xDup in alFunctions) if (s6xDup.UniqueAddress == s6xFunction.UniqueAddress) alDup.Add(s6xDup);

                    if (alS6x.Count == 0)
                    {
                        // Nothing in S6x
                        // First Xdf Item becomes main Item
                        for (int iDup = 0; iDup < alDup.Count; iDup++) ((S6xFunction)alDup[iDup]).DuplicateNum = iDup;

                        continue;
                    }

                    ArrayList alMatchedNums = new ArrayList();
                    
                    // Matching XdfUniqueId
                    foreach (S6xFunction s6xExisting in alS6x)
                    {
                        foreach (S6xFunction s6xDup in alDup)
                        {
                            if (s6xDup.DuplicateNum >= 0) continue;
                            if (s6xExisting.XdfUniqueId == s6xDup.XdfUniqueId)
                            {
                                s6xDup.DuplicateNum = s6xExisting.DuplicateNum;
                                alMatchedNums.Add(s6xDup.DuplicateNum);
                                break;
                            }
                        }
                    }

                    // Matching Label
                    foreach (S6xFunction s6xExisting in alS6x)
                    {
                        if (alMatchedNums.Contains(s6xExisting.DuplicateNum)) continue;
                        foreach (S6xFunction s6xDup in alDup)
                        {
                            if (s6xDup.DuplicateNum >= 0) continue;
                            if (s6xExisting.Label == s6xDup.Label)
                            {
                                s6xDup.DuplicateNum = s6xExisting.DuplicateNum;
                                alMatchedNums.Add(s6xDup.DuplicateNum);
                                break;
                            }
                        }
                    }

                    // Main Item is required
                    if (!alMatchedNums.Contains(0))
                    {
                        if (alMatchedNums.Count > alDup.Count)
                        {
                            // First found is used
                            foreach (S6xFunction s6xDup in alDup)
                            {
                                if (s6xDup.DuplicateNum >= 0) continue;
                                s6xDup.DuplicateNum = 0;
                                alMatchedNums.Add(s6xDup.DuplicateNum);
                                break;
                            }
                        }
                        else
                        {
                            // Defaulted
                            for (int iDup = 0; iDup < alDup.Count; iDup++)
                            {
                                alMatchedNums.Clear();
                                ((S6xFunction)alDup[iDup]).DuplicateNum = iDup;
                                alMatchedNums.Add(iDup);
                            }
                        }
                    }

                    // Remaining Duplicates
                    foreach (S6xFunction s6xDup in alDup)
                    {
                        if (s6xDup.DuplicateNum >= 0) continue;
                        int iDup = 0;
                        while (alMatchedNums.Contains(iDup)) iDup++;
                        s6xDup.DuplicateNum = iDup;
                        alMatchedNums.Add(iDup);
                    }
                }
            }
            // Real Import
            foreach (S6xFunction s6xFunction in alFunctions)
            {
                // Main Item
                if (s6xFunction.DuplicateNum == 0)
                {
                    if (slFunctions.ContainsKey(s6xFunction.UniqueAddress))
                    {
                        slFunctions[s6xFunction.UniqueAddress] = s6xFunction;
                        alS6xUpdates.Add(s6xFunction.UniqueAddress);
                    }
                    else
                    {
                        slFunctions.Add(s6xFunction.UniqueAddress, s6xFunction);
                        alS6xCreations.Add(s6xFunction.UniqueAddress);
                    }
                }
                else if (s6xFunction.DuplicateNum >= 1)
                {
                    if (slDupFunctions.ContainsKey(s6xFunction.DuplicateAddress))
                    {
                        slDupFunctions[s6xFunction.DuplicateAddress] = s6xFunction;
                        alS6xDupUpdates.Add(s6xFunction.DuplicateAddress);
                    }
                    else
                    {
                        slDupFunctions.Add(s6xFunction.DuplicateAddress, s6xFunction);
                        alS6xDupCreations.Add(s6xFunction.DuplicateAddress);
                    }
                }
                else
                {
                    // Should not Exist
                }
            }
            alFunctions = null;

            if (xdfFile.xdfTables != null)
            {
                foreach (XdfTable xdfObject in xdfFile.xdfTables)
                {
                    S6xTable s6xObject = null;
                    int bankNum = -1;
                    int address = -1;
                    int addressBin = Tools.binAddressFromXdfAddress(xdfObject.mmeMainAddress, Properties.XdfBaseOffsetInt);
                    if (sadBin.isCalibrationAddress(addressBin))
                    {
                        bankNum = sadBin.Calibration.BankNum;
                        address = addressBin - sadBin.Calibration.BankAddressBinInt;
                        s6xObject = new S6xTable(xdfObject, bankNum, address, addressBin, true, ref slFunctions);

                        // Ignored
                        if (alReservedAddresses.Contains(s6xObject.UniqueAddress) || sadBin.Calibration.isLoadCreated(s6xObject.UniqueAddress))
                        {
                            if (!alIgnored.Contains(s6xObject.UniqueAddress)) alIgnored.Add(s6xObject.UniqueAddress);
                            continue;
                        }
                        // Conflicts
                        if (slFunctions.ContainsKey(s6xObject.UniqueAddress) || slScalars.ContainsKey(s6xObject.UniqueAddress) || slStructures.ContainsKey(s6xObject.UniqueAddress))
                        {
                            if (!alTablesConflicts.Contains(s6xObject.UniqueAddress)) alTablesConflicts.Add(s6xObject.UniqueAddress);
                            continue;
                        }
                        // Duplicates
                        if (slProcessed.ContainsKey(s6xObject.UniqueAddress))
                        {
                            slProcessed[s6xObject.UniqueAddress] = (int)slProcessed[s6xObject.UniqueAddress] + 1;
                            if (!alDuplicates.Contains(s6xObject.UniqueAddress)) alDuplicates.Add(s6xObject.UniqueAddress);
                        }
                        else
                        {
                            slProcessed.Add(s6xObject.UniqueAddress, 1);
                        }

                        if (slTables.ContainsKey(s6xObject.UniqueAddress))
                        {
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            string sLabel = ((S6xTable)slTables[s6xObject.UniqueAddress]).ShortLabel;
                            if (s6xObject.ShortLabel.StartsWith("TpTb") && sLabel != null && sLabel != string.Empty) s6xObject.ShortLabel = sLabel;
                        }
                        s6xObject.DuplicateNum = -1; // For Duplicates Mngt
                        alTables.Add(s6xObject);
                        s6xObject = null;
                    }
                    else
                    {
                        bankNum = sadBin.getBankNum(addressBin);
                        if (bankNum >= 0)
                        {
                            switch (bankNum)
                            {
                                case 8:
                                    address = addressBin - sadBin.Bank8.AddressBinInt;
                                    break;
                                case 1:
                                    address = addressBin - sadBin.Bank1.AddressBinInt;
                                    break;
                                case 9:
                                    address = addressBin - sadBin.Bank9.AddressBinInt;
                                    break;
                                case 0:
                                    address = addressBin - sadBin.Bank0.AddressBinInt;
                                    break;
                            }
                            s6xObject = new S6xTable(xdfObject, bankNum, address, addressBin, false, ref slFunctions);
                            // Ignored
                            if (alReservedAddresses.Contains(s6xObject.UniqueAddress) || sadBin.Calibration.isLoadCreated(s6xObject.UniqueAddress))
                            {
                                if (!alIgnored.Contains(s6xObject.UniqueAddress)) alIgnored.Add(s6xObject.UniqueAddress);
                                continue;
                            }
                            // Conflicts
                            if (slFunctions.ContainsKey(s6xObject.UniqueAddress) || slScalars.ContainsKey(s6xObject.UniqueAddress) || slStructures.ContainsKey(s6xObject.UniqueAddress))
                            {
                                if (!alTablesConflicts.Contains(s6xObject.UniqueAddress)) alTablesConflicts.Add(s6xObject.UniqueAddress);
                                continue;
                            }
                            // Duplicates
                            if (slProcessed.ContainsKey(s6xObject.UniqueAddress))
                            {
                                slProcessed[s6xObject.UniqueAddress] = (int)slProcessed[s6xObject.UniqueAddress] + 1;
                                if (!alDuplicates.Contains(s6xObject.UniqueAddress)) alDuplicates.Add(s6xObject.UniqueAddress);
                            }
                            else
                            {
                                slProcessed.Add(s6xObject.UniqueAddress, 1);
                            }

                            if (slTables.ContainsKey(s6xObject.UniqueAddress))
                            {
                                // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                                string sLabel = ((S6xTable)slTables[s6xObject.UniqueAddress]).ShortLabel;
                                if (s6xObject.ShortLabel.StartsWith("TpTb") && sLabel != null && sLabel != string.Empty) s6xObject.ShortLabel = sLabel;
                            }
                            s6xObject.DuplicateNum = -1; // For Duplicates Mngt
                            alTables.Add(s6xObject);
                            s6xObject = null;
                        }
                    }
                }
            }
            // Duplicates Mngt through DuplicateNum calculation
            foreach (S6xTable s6xTable in alTables)
            {
                if (!slProcessed.ContainsKey(s6xTable.UniqueAddress)) continue;
                if ((int)slProcessed[s6xTable.UniqueAddress] == 1)
                {
                    // Unique Item
                    s6xTable.DuplicateNum = 0;
                }
                else
                {
                    // Duplicates
                    if (s6xTable.DuplicateNum >= 0) continue; // ALready Processed

                    ArrayList alS6x = new ArrayList();
                    if (slTables.ContainsKey(s6xTable.UniqueAddress))
                    {
                        alS6x.Add(slTables[s6xTable.UniqueAddress]);
                        foreach (S6xTable s6xOtherDup in slDupTables.Values)
                        {
                            if (s6xOtherDup.UniqueAddress == s6xTable.UniqueAddress) alS6x.Add(s6xOtherDup);
                        }
                    }

                    ArrayList alDup = new ArrayList();
                    foreach (S6xTable s6xDup in alTables) if (s6xDup.UniqueAddress == s6xTable.UniqueAddress) alDup.Add(s6xDup);

                    if (alS6x.Count == 0)
                    {
                        // Nothing in S6x
                        // First Xdf Item becomes main Item
                        for (int iDup = 0; iDup < alDup.Count; iDup++) ((S6xTable)alDup[iDup]).DuplicateNum = iDup;

                        continue;
                    }

                    ArrayList alMatchedNums = new ArrayList();

                    // Matching XdfUniqueId
                    foreach (S6xTable s6xExisting in alS6x)
                    {
                        foreach (S6xTable s6xDup in alDup)
                        {
                            if (s6xDup.DuplicateNum >= 0) continue;
                            if (s6xExisting.XdfUniqueId == s6xDup.XdfUniqueId)
                            {
                                s6xDup.DuplicateNum = s6xExisting.DuplicateNum;
                                alMatchedNums.Add(s6xDup.DuplicateNum);
                                break;
                            }
                        }
                    }

                    // Matching Label
                    foreach (S6xTable s6xExisting in alS6x)
                    {
                        if (alMatchedNums.Contains(s6xExisting.DuplicateNum)) continue;
                        foreach (S6xTable s6xDup in alDup)
                        {
                            if (s6xDup.DuplicateNum >= 0) continue;
                            if (s6xExisting.Label == s6xDup.Label)
                            {
                                s6xDup.DuplicateNum = s6xExisting.DuplicateNum;
                                alMatchedNums.Add(s6xDup.DuplicateNum);
                                break;
                            }
                        }
                    }

                    // Main Item is required
                    if (!alMatchedNums.Contains(0))
                    {
                        if (alMatchedNums.Count > alDup.Count)
                        {
                            // First found is used
                            foreach (S6xTable s6xDup in alDup)
                            {
                                if (s6xDup.DuplicateNum >= 0) continue;
                                s6xDup.DuplicateNum = 0;
                                alMatchedNums.Add(s6xDup.DuplicateNum);
                                break;
                            }
                        }
                        else
                        {
                            // Defaulted
                            for (int iDup = 0; iDup < alDup.Count; iDup++)
                            {
                                alMatchedNums.Clear();
                                ((S6xTable)alDup[iDup]).DuplicateNum = iDup;
                                alMatchedNums.Add(iDup);
                            }
                        }
                    }

                    // Remaining Duplicates
                    foreach (S6xTable s6xDup in alDup)
                    {
                        if (s6xDup.DuplicateNum >= 0) continue;
                        int iDup = 0;
                        while (alMatchedNums.Contains(iDup)) iDup++;
                        s6xDup.DuplicateNum = iDup;
                        alMatchedNums.Add(iDup);
                    }
                }
            }
            // Real Import
            foreach (S6xTable s6xTable in alTables)
            {
                // Main Item
                if (s6xTable.DuplicateNum == 0)
                {
                    if (slTables.ContainsKey(s6xTable.UniqueAddress))
                    {
                        slTables[s6xTable.UniqueAddress] = s6xTable;
                        alS6xUpdates.Add(s6xTable.UniqueAddress);
                    }
                    else
                    {
                        slTables.Add(s6xTable.UniqueAddress, s6xTable);
                        alS6xCreations.Add(s6xTable.UniqueAddress);
                    }
                }
                else if (s6xTable.DuplicateNum >= 1)
                {
                    if (slDupTables.ContainsKey(s6xTable.DuplicateAddress))
                    {
                        slDupTables[s6xTable.DuplicateAddress] = s6xTable;
                        alS6xDupUpdates.Add(s6xTable.DuplicateAddress);
                    }
                    else
                    {
                        slDupTables.Add(s6xTable.DuplicateAddress, s6xTable);
                        alS6xDupCreations.Add(s6xTable.DuplicateAddress);
                    }

                }
                else
                {
                    // Should not Exist
                }
            }
            alTables = null;

            if (xdfFile.xdfScalars != null)
            {
                foreach (XdfScalar xdfObject in xdfFile.xdfScalars)
                {
                    S6xScalar s6xObject = null;
                    int bankNum = -1;
                    int address = -1;
                    int addressBin = Tools.binAddressFromXdfAddress(xdfObject.mmeMainAddress, Properties.XdfBaseOffsetInt);
                    if (sadBin.isCalibrationAddress(addressBin))
                    {
                        bankNum = sadBin.Calibration.BankNum;
                        address = addressBin - sadBin.Calibration.BankAddressBinInt;
                        s6xObject = new S6xScalar(xdfObject, bankNum, address, addressBin, true);
                        // Ignored
                        if (alReservedAddresses.Contains(s6xObject.UniqueAddress) || sadBin.Calibration.isLoadCreated(s6xObject.UniqueAddress))
                        {
                            if (!alIgnored.Contains(s6xObject.UniqueAddress)) alIgnored.Add(s6xObject.UniqueAddress);
                            continue;
                        }
                        // Conflicts
                        if (slTables.ContainsKey(s6xObject.UniqueAddress) || slFunctions.ContainsKey(s6xObject.UniqueAddress) || slStructures.ContainsKey(s6xObject.UniqueAddress))
                        {
                            if (!alScalarsConflicts.Contains(s6xObject.UniqueAddress)) alScalarsConflicts.Add(s6xObject.UniqueAddress);
                            continue;
                        }
                        // Duplicates
                        if (slProcessed.ContainsKey(s6xObject.UniqueAddress))
                        {
                            slProcessed[s6xObject.UniqueAddress] = (int)slProcessed[s6xObject.UniqueAddress] + 1;
                            if (!alDuplicates.Contains(s6xObject.UniqueAddress)) alDuplicates.Add(s6xObject.UniqueAddress);
                        }
                        else
                        {
                            slProcessed.Add(s6xObject.UniqueAddress, 1);
                        }

                        if (slScalars.ContainsKey(s6xObject.UniqueAddress))
                        {
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            string sLabel = ((S6xScalar)slScalars[s6xObject.UniqueAddress]).ShortLabel;
                            if (s6xObject.ShortLabel.StartsWith("TpSc") && sLabel != null && sLabel != string.Empty) s6xObject.ShortLabel = sLabel;
                        }
                        s6xObject.DuplicateNum = -1; // For Duplicates Mngt
                        alScalars.Add(s6xObject);
                        s6xObject = null;
                    }
                    else
                    {
                        bankNum = sadBin.getBankNum(addressBin);
                        if (bankNum >= 0)
                        {
                            switch (bankNum)
                            {
                                case 8:
                                    address = addressBin - sadBin.Bank8.AddressBinInt;
                                    break;
                                case 1:
                                    address = addressBin - sadBin.Bank1.AddressBinInt;
                                    break;
                                case 9:
                                    address = addressBin - sadBin.Bank9.AddressBinInt;
                                    break;
                                case 0:
                                    address = addressBin - sadBin.Bank0.AddressBinInt;
                                    break;
                            }
                            s6xObject = new S6xScalar(xdfObject, bankNum, address, addressBin, false);
                            // Ignored
                            if (alReservedAddresses.Contains(s6xObject.UniqueAddress) || sadBin.Calibration.isLoadCreated(s6xObject.UniqueAddress))
                            {
                                if (!alIgnored.Contains(s6xObject.UniqueAddress)) alIgnored.Add(s6xObject.UniqueAddress);
                                continue;
                            }
                            // Conflicts
                            if (slTables.ContainsKey(s6xObject.UniqueAddress) || slFunctions.ContainsKey(s6xObject.UniqueAddress) || slStructures.ContainsKey(s6xObject.UniqueAddress))
                            {
                                if (!alScalarsConflicts.Contains(s6xObject.UniqueAddress)) alScalarsConflicts.Add(s6xObject.UniqueAddress);
                                continue;
                            }
                            // Duplicates
                            if (slProcessed.ContainsKey(s6xObject.UniqueAddress))
                            {
                                slProcessed[s6xObject.UniqueAddress] = (int)slProcessed[s6xObject.UniqueAddress] + 1;
                                if (!alDuplicates.Contains(s6xObject.UniqueAddress)) alDuplicates.Add(s6xObject.UniqueAddress);
                            }
                            else
                            {
                                slProcessed.Add(s6xObject.UniqueAddress, 1);
                            }

                            if (slScalars.ContainsKey(s6xObject.UniqueAddress))
                            {
                                // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                                string sLabel = ((S6xScalar)slScalars[s6xObject.UniqueAddress]).ShortLabel;
                                if (s6xObject.ShortLabel.StartsWith("TpSc") && sLabel != null && sLabel != string.Empty) s6xObject.ShortLabel = sLabel;
                            }
                            s6xObject.DuplicateNum = -1; // For Duplicates Mngt
                            alScalars.Add(s6xObject);
                            s6xObject = null;
                        }
                    }
                }
            }

            if (xdfFile.xdfFlags != null)
            {
                foreach (XdfFlag xdfObject in xdfFile.xdfFlags)
                {
                    S6xScalar s6xObject = null;
                    int bankNum = -1;
                    int address = -1;
                    int addressBin = Tools.binAddressFromXdfAddress(xdfObject.mmeMainAddress, Properties.XdfBaseOffsetInt);
                    if (sadBin.isCalibrationAddress(addressBin))
                    {
                        bankNum = sadBin.Calibration.BankNum;
                        address = addressBin - sadBin.Calibration.BankAddressBinInt;
                        // Ignored
                        if (alReservedAddresses.Contains(Tools.UniqueAddress(bankNum, address)) || sadBin.Calibration.isLoadCreated(Tools.UniqueAddress(bankNum, address)))
                        {
                            if (!alIgnored.Contains(Tools.UniqueAddress(bankNum, address))) alIgnored.Add(Tools.UniqueAddress(bankNum, address));
                            continue;
                        }
                        // Conflicts
                        if (slTables.ContainsKey(Tools.UniqueAddress(bankNum, address)) || slFunctions.ContainsKey(Tools.UniqueAddress(bankNum, address)) || slStructures.ContainsKey(Tools.UniqueAddress(bankNum, address)))
                        {
                            if (!alScalarsConflicts.Contains(Tools.UniqueAddress(bankNum, address))) alScalarsConflicts.Add(Tools.UniqueAddress(bankNum, address));
                            continue;
                        }
                        // No Duplicates for Bit Flags
                        if (!slProcessed.ContainsKey(Tools.UniqueAddress(bankNum, address))) slProcessed.Add(Tools.UniqueAddress(bankNum, address), 1);

                        if (slScalars.ContainsKey(Tools.UniqueAddress(bankNum, address)))
                        {
                            s6xObject = (S6xScalar)slScalars[Tools.UniqueAddress(bankNum, address)];
                            s6xObject.AddBitFlag(xdfObject);
                        }
                        else
                        {
                            s6xObject = new S6xScalar(xdfObject, bankNum, address, addressBin, true);
                        }
                        s6xObject.DuplicateNum = -1; // For Duplicates Mngt
                        alScalars.Add(s6xObject);
                        s6xObject = null;
                    }
                    else
                    {
                        bankNum = sadBin.getBankNum(addressBin);
                        if (bankNum >= 0)
                        {
                            switch (bankNum)
                            {
                                case 8:
                                    address = addressBin - sadBin.Bank8.AddressBinInt;
                                    break;
                                case 1:
                                    address = addressBin - sadBin.Bank1.AddressBinInt;
                                    break;
                                case 9:
                                    address = addressBin - sadBin.Bank9.AddressBinInt;
                                    break;
                                case 0:
                                    address = addressBin - sadBin.Bank0.AddressBinInt;
                                    break;
                            }
                            // Ignored
                            if (alReservedAddresses.Contains(Tools.UniqueAddress(bankNum, address)) || sadBin.Calibration.isLoadCreated(Tools.UniqueAddress(bankNum, address)))
                            {
                                if (!alIgnored.Contains(Tools.UniqueAddress(bankNum, address))) alIgnored.Add(Tools.UniqueAddress(bankNum, address));
                                continue;
                            }
                            // Conflicts
                            if (slTables.ContainsKey(Tools.UniqueAddress(bankNum, address)) || slFunctions.ContainsKey(Tools.UniqueAddress(bankNum, address)) || slStructures.ContainsKey(Tools.UniqueAddress(bankNum, address)))
                            {
                                if (!alScalarsConflicts.Contains(Tools.UniqueAddress(bankNum, address))) alScalarsConflicts.Add(Tools.UniqueAddress(bankNum, address));
                                continue;
                            }
                            // No Duplicates for Bit Flags
                            if (!slProcessed.ContainsKey(Tools.UniqueAddress(bankNum, address))) slProcessed.Add(Tools.UniqueAddress(bankNum, address), 1);

                            if (slScalars.ContainsKey(Tools.UniqueAddress(bankNum, address)))
                            {
                                s6xObject = (S6xScalar)slScalars[Tools.UniqueAddress(bankNum, address)];
                                s6xObject.AddBitFlag(xdfObject);
                            }
                            else
                            {
                                s6xObject = new S6xScalar(xdfObject, bankNum, address, addressBin, true);
                            }
                            s6xObject.DuplicateNum = -1; // For Duplicates Mngt
                            alScalars.Add(s6xObject);
                            s6xObject = null;
                        }
                    }
                }
            }

            // Duplicates Mngt through DuplicateNum calculation
            foreach (S6xScalar s6xScalar in alScalars)
            {
                if (!slProcessed.ContainsKey(s6xScalar.UniqueAddress)) continue;
                if ((int)slProcessed[s6xScalar.UniqueAddress] == 1)
                {
                    // Unique Item
                    s6xScalar.DuplicateNum = 0;
                }
                else
                {
                    // Duplicates
                    if (s6xScalar.DuplicateNum >= 0) continue; // ALready Processed

                    ArrayList alS6x = new ArrayList();
                    if (slScalars.ContainsKey(s6xScalar.UniqueAddress))
                    {
                        alS6x.Add(slScalars[s6xScalar.UniqueAddress]);
                        foreach (S6xScalar s6xOtherDup in slDupScalars.Values)
                        {
                            if (s6xOtherDup.UniqueAddress == s6xScalar.UniqueAddress) alS6x.Add(s6xOtherDup);
                        }
                    }

                    ArrayList alDup = new ArrayList();
                    foreach (S6xScalar s6xDup in alScalars) if (s6xDup.UniqueAddress == s6xScalar.UniqueAddress) alDup.Add(s6xDup);

                    if (alS6x.Count == 0)
                    {
                        // Nothing in S6x
                        // First Xdf Item becomes main Item
                        for (int iDup = 0; iDup < alDup.Count; iDup++) ((S6xScalar)alDup[iDup]).DuplicateNum = iDup;

                        continue;
                    }

                    ArrayList alMatchedNums = new ArrayList();

                    // Matching XdfUniqueId
                    foreach (S6xScalar s6xExisting in alS6x)
                    {
                        foreach (S6xScalar s6xDup in alDup)
                        {
                            if (s6xDup.DuplicateNum >= 0) continue;
                            if (s6xExisting.XdfUniqueId == s6xDup.XdfUniqueId)
                            {
                                s6xDup.DuplicateNum = s6xExisting.DuplicateNum;
                                alMatchedNums.Add(s6xDup.DuplicateNum);
                                break;
                            }
                        }
                    }

                    // Matching Label
                    foreach (S6xScalar s6xExisting in alS6x)
                    {
                        if (alMatchedNums.Contains(s6xExisting.DuplicateNum)) continue;
                        foreach (S6xScalar s6xDup in alDup)
                        {
                            if (s6xDup.DuplicateNum >= 0) continue;
                            if (s6xExisting.Label == s6xDup.Label)
                            {
                                s6xDup.DuplicateNum = s6xExisting.DuplicateNum;
                                alMatchedNums.Add(s6xDup.DuplicateNum);
                                break;
                            }
                        }
                    }

                    // Main Item is required
                    if (!alMatchedNums.Contains(0))
                    {
                        if (alMatchedNums.Count > alDup.Count)
                        {
                            // First found is used
                            foreach (S6xScalar s6xDup in alDup)
                            {
                                if (s6xDup.DuplicateNum >= 0) continue;
                                s6xDup.DuplicateNum = 0;
                                alMatchedNums.Add(s6xDup.DuplicateNum);
                                break;
                            }
                        }
                        else
                        {
                            // Defaulted
                            for (int iDup = 0; iDup < alDup.Count; iDup++)
                            {
                                alMatchedNums.Clear();
                                ((S6xScalar)alDup[iDup]).DuplicateNum = iDup;
                                alMatchedNums.Add(iDup);
                            }
                        }
                    }

                    // Remaining Duplicates
                    foreach (S6xScalar s6xDup in alDup)
                    {
                        if (s6xDup.DuplicateNum >= 0) continue;
                        int iDup = 0;
                        while (alMatchedNums.Contains(iDup)) iDup++;
                        s6xDup.DuplicateNum = iDup;
                        alMatchedNums.Add(iDup);
                    }
                }
            }
            // Real Import
            foreach (S6xScalar s6xScalar in alScalars)
            {
                // BitFlags that are Switches
                if (s6xScalar.isBitFlags)
                {
                    if (s6xScalar.BitFlagsNum == 1) // Only One BitFlag
                    {
                        if (s6xScalar.BitFlags[0].Position == 0) // 0 for Position for a value at 0 or 1
                        {
                            s6xScalar.ShortLabel = s6xScalar.BitFlags[0].ShortLabel;
                            s6xScalar.Label = s6xScalar.BitFlags[0].Label;
                            s6xScalar.Comments = s6xScalar.BitFlags[0].Comments;
                            s6xScalar.BitFlags = null;
                        }
                    }
                }
                
                // Main Item
                if (s6xScalar.DuplicateNum == 0)
                {
                    if (slScalars.ContainsKey(s6xScalar.UniqueAddress))
                    {
                        slScalars[s6xScalar.UniqueAddress] = s6xScalar;
                        alS6xUpdates.Add(s6xScalar.UniqueAddress);
                    }
                    else
                    {
                        slScalars.Add(s6xScalar.UniqueAddress, s6xScalar);
                        alS6xCreations.Add(s6xScalar.UniqueAddress);
                    }
                }
                else if (s6xScalar.DuplicateNum >= 1)
                {
                    if (slDupScalars.ContainsKey(s6xScalar.DuplicateAddress))
                    {
                        slDupScalars[s6xScalar.DuplicateAddress] = s6xScalar;
                        alS6xDupUpdates.Add(s6xScalar.DuplicateAddress);
                    }
                    else
                    {
                        slDupScalars.Add(s6xScalar.DuplicateAddress, s6xScalar);
                        alS6xDupCreations.Add(s6xScalar.DuplicateAddress);
                    }

                }
                else
                {
                    // Should not Exist
                }
            }
            alScalars = null;

            ArrayList alConflicts = new ArrayList();
            alConflicts.AddRange(alTablesConflicts);
            alConflicts.AddRange(alFunctionsConflicts);
            alConflicts.AddRange(alScalarsConflicts);
            
            arrResult[0] = alS6xUpdates.ToArray(typeof(string));
            arrResult[1] = alS6xCreations.ToArray(typeof(string));
            arrResult[2] = alDuplicates.ToArray(typeof(string));
            arrResult[3] = alConflicts.ToArray(typeof(string));
            arrResult[4] = alIgnored.ToArray(typeof(string));
            arrResult[5] = alS6xDupUpdates.ToArray(typeof(string));
            arrResult[6] = alS6xDupCreations.ToArray(typeof(string));

            alS6xUpdates = null;
            alS6xCreations = null;
            alDuplicates = null;
            alConflicts = null;
            alTablesConflicts = null;
            alFunctionsConflicts = null;
            alScalarsConflicts = null;
            slProcessed = null;
            alIgnored = null;
            alS6xDupUpdates = null;
            alS6xDupCreations = null;

            isSaved = false;

            GC.Collect();

            return arrResult;
        }

        public object[] writeToFileObject(ref XdfFile xdfFile, ref SADBin sadBin)
        {
            ArrayList alS6xUpdates = new ArrayList();
            ArrayList alS6xCreations = new ArrayList();
            ArrayList alDuplicates = new ArrayList();
            ArrayList alS6xDupUpdates = new ArrayList();
            SortedList slProcessed = new SortedList();
            ArrayList alXdfObjects = null;
            ArrayList alS6xDupUniqueAddresses = null;
            string xdfUniqueId = string.Empty;
            string uniqueAddress = string.Empty;
            int xdfBaseOffset = 0;
            // 0 S6x Updates
            // 1 S6x Creations
            // 2 Duplicates
            // 3 Conflicts  - No Mngt needed for writeToFileObject
            // 4 Ignored  - No Mngt needed for writeToFileObject
            // 5 S6x Duplicates Updates
            // 6 S6x Duplicates Creations  - No Mngt needed for writeToFileObject
            object[] arrResult = new object[7];

            int newXdfUniqueId = Convert.ToInt32(xdfFile.getLastXdfUniqueId().ToLower().Replace("0x", ""), 16) + 1;

            if (xdfFile.xdfHeader != null)
            {
                try
                {
                    switch (xdfFile.version)
                    {
                        case "1.50":
                            xdfBaseOffset = Convert.ToInt32(xdfFile.xdfHeader.baseoffset1_50.Trim().Replace("-", "").Replace("+", ""));
                            if (xdfFile.xdfHeader.baseoffset1_50.Contains("-")) xdfBaseOffset *= -1;
                            break;
                        case "1.60":
                            xdfBaseOffset = Convert.ToInt32(xdfFile.xdfHeader.xdfBaseOffset.offset);
                            if (xdfFile.xdfHeader.xdfBaseOffset.subtract == "1") xdfBaseOffset *= -1;
                            break;
                    }
                }
                catch
                {
                    xdfBaseOffset = Properties.XdfBaseOffsetInt;
                }
            }


            // XdfUniqueIds Update on existing S6x Functions to Prepare Scalers
            if (xdfFile.xdfFunctions != null)
            {
                foreach (XdfFunction xdfObject in xdfFile.xdfFunctions)
                {
                    xdfUniqueId = xdfObject.uniqueid;
                    uniqueAddress = Tools.UniqueAddress(sadBin.Calibration.BankNum, Tools.binAddressFromXdfAddress(xdfObject.getMmedAddress(), xdfBaseOffset), sadBin.Calibration.BankAddressBinInt);
                    if (!slFunctions.ContainsKey(uniqueAddress)) continue;

                    if (((S6xFunction)slFunctions[uniqueAddress]).XdfUniqueId == xdfUniqueId)
                    {
                        // Already Matched on xdfUniqueId
                        if (slProcessed.ContainsKey(uniqueAddress))
                        {
                            if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                            slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            continue;
                        }
                        slProcessed.Add(uniqueAddress, 1);
                    }
                    else if (((S6xFunction)slFunctions[uniqueAddress]).Label == xdfObject.title)
                    {
                        // Matching on Title
                        if (slProcessed.ContainsKey(uniqueAddress))
                        {
                            if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                            slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            continue;
                        }
                        slProcessed.Add(uniqueAddress, 1);
                    }
                    if (slProcessed.ContainsKey(uniqueAddress))
                    {
                        // Matched on main Item
                        ((S6xFunction)slFunctions[uniqueAddress]).XdfUniqueId = xdfUniqueId;
                        ((S6xFunction)slFunctions[uniqueAddress]).Store = true;
                        ((S6xFunction)slFunctions[uniqueAddress]).Skip = false;
                        alS6xUpdates.Add(uniqueAddress);

                        xdfObject.Import((S6xFunction)slFunctions[uniqueAddress], xdfBaseOffset);
                        continue;
                    }

                    // Real Duplicates
                    foreach (S6xFunction s6xDup in slDupFunctions)
                    {
                        if (uniqueAddress != s6xDup.UniqueAddress) continue;

                        if (((S6xFunction)slDupFunctions[s6xDup.DuplicateAddress]).XdfUniqueId == xdfUniqueId)
                        {
                            // Already Matched on xdfUniqueId
                            if (slProcessed.ContainsKey(uniqueAddress))
                            {
                                if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                                slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            }
                            else
                            {
                                slProcessed.Add(uniqueAddress, 1);
                            }
                        }
                        else if (((S6xFunction)slDupFunctions[s6xDup.DuplicateAddress]).Label == xdfObject.title)
                        {
                            // Matching on Title
                            if (slProcessed.ContainsKey(uniqueAddress))
                            {
                                if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                                slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            }
                            else
                            {
                                slProcessed.Add(uniqueAddress, 1);
                            }
                        }
                        else
                        {
                            continue;
                        }

                        ((S6xFunction)slDupFunctions[s6xDup.DuplicateAddress]).XdfUniqueId = xdfUniqueId;
                        ((S6xFunction)slDupFunctions[s6xDup.DuplicateAddress]).Store = true;
                        ((S6xFunction)slDupFunctions[s6xDup.DuplicateAddress]).Skip = false;
                        alS6xDupUpdates.Add(s6xDup.DuplicateAddress);

                        xdfObject.Import((S6xFunction)slDupFunctions[s6xDup.DuplicateAddress], xdfBaseOffset);
                        
                        break;
                    }
                }
            }
            // Missing Xdf Functions Creation to Prepare Scalers
            if (xdfFile.xdfFunctions == null) alXdfObjects = new ArrayList();
            else alXdfObjects = new ArrayList(xdfFile.xdfFunctions);
            
            alS6xDupUniqueAddresses = new ArrayList();
            foreach (S6xFunction s6xObject in slDupFunctions.Values) if (!alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress)) alS6xDupUniqueAddresses.Add(s6xObject.UniqueAddress);

            foreach (S6xFunction s6xObject in slFunctions.Values)
            {
                if (!s6xObject.Skip && (s6xObject.Store || s6xObject.OutputScaleExpression.ToUpper() != "X")  && !alS6xUpdates.Contains(s6xObject.UniqueAddress) && !alS6xCreations.Contains(s6xObject.UniqueAddress) && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    XdfFunction xdfObject = new XdfFunction(s6xObject, xdfBaseOffset);
                    xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                    newXdfUniqueId++;
                    alXdfObjects.Add(xdfObject);
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xFunction s6xDupObject in slDupFunctions.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && (s6xDupObject.Store || s6xDupObject.OutputScaleExpression.ToUpper() != "X") && !alS6xDupUpdates.Contains(s6xDupObject.DuplicateAddress) && s6xDupObject.AddressBinInt >= xdfBaseOffset)
                        {
                            XdfFunction xdfObject = new XdfFunction(s6xDupObject, xdfBaseOffset);
                            xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                            newXdfUniqueId++;
                            alXdfObjects.Add(xdfObject);
                        }
                    }
                }
            }
            xdfFile.xdfFunctions = (XdfFunction[])alXdfObjects.ToArray(typeof(XdfFunction));
            alXdfObjects = null;
            alS6xDupUniqueAddresses = null;

            // Xdf other Objects Update from S6x other Objects
            if (xdfFile.xdfTables != null)
            {
                foreach (XdfTable xdfObject in xdfFile.xdfTables)
                {
                    xdfUniqueId = xdfObject.uniqueid;
                    uniqueAddress = Tools.UniqueAddress(sadBin.Calibration.BankNum, Tools.binAddressFromXdfAddress(xdfObject.getMmedAddress(), xdfBaseOffset), sadBin.Calibration.BankAddressBinInt);
                    if (!slTables.ContainsKey(uniqueAddress)) continue;

                    if (((S6xTable)slTables[uniqueAddress]).XdfUniqueId == xdfUniqueId)
                    {
                        // Already Matched on xdfUniqueId
                        if (slProcessed.ContainsKey(uniqueAddress))
                        {
                            if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                            slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            continue;
                        }
                        slProcessed.Add(uniqueAddress, 1);
                    }
                    else if (((S6xTable)slTables[uniqueAddress]).Label == xdfObject.title)
                    {
                        // Matching on Title
                        if (slProcessed.ContainsKey(uniqueAddress))
                        {
                            if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                            slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            continue;
                        }
                        slProcessed.Add(uniqueAddress, 1);
                    }
                    if (slProcessed.ContainsKey(uniqueAddress))
                    {
                        // Matched on main Item
                        // Trying to find Scalers Unique Xdf Ids
                        S6xTable s6xTable = (S6xTable)slTables[uniqueAddress];
                        s6xTable.XdfUniqueId = xdfUniqueId;
                        s6xTable.Store = true;
                        s6xTable.Skip = false;
                        if (s6xTable.ColsScalerAddress != null && s6xTable.ColsScalerAddress != string.Empty)
                        {
                            if (slFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)slFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                            else if (slDupFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)slDupFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                            else s6xTable.ColsScalerXdfUniqueId = string.Empty;
                        }
                        if (s6xTable.RowsScalerAddress != null && s6xTable.RowsScalerAddress != string.Empty)
                        {
                            if (slFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)slFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                            else if (slDupFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)slDupFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                            else s6xTable.RowsScalerXdfUniqueId = string.Empty;
                        }
                        slTables[s6xTable.UniqueAddress] = s6xTable;
                        alS6xUpdates.Add(uniqueAddress);

                        xdfObject.Import(s6xTable, xdfBaseOffset);
                        s6xTable = null;
                        continue;
                    }

                    // Real Duplicates
                    foreach (S6xTable s6xDup in slDupTables)
                    {
                        if (uniqueAddress != s6xDup.UniqueAddress) continue;

                        if (((S6xTable)slDupTables[s6xDup.DuplicateAddress]).XdfUniqueId == xdfUniqueId)
                        {
                            // Already Matched on xdfUniqueId
                            if (slProcessed.ContainsKey(uniqueAddress))
                            {
                                if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                                slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            }
                            else
                            {
                                slProcessed.Add(uniqueAddress, 1);
                            }
                        }
                        else if (((S6xTable)slDupTables[s6xDup.DuplicateAddress]).Label == xdfObject.title)
                        {
                            // Matching on Title
                            if (slProcessed.ContainsKey(uniqueAddress))
                            {
                                if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                                slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            }
                            else
                            {
                                slProcessed.Add(uniqueAddress, 1);
                            }
                        }
                        else
                        {
                            continue;
                        }

                        // Trying to find Scalers Unique Xdf Ids
                        S6xTable s6xTable = (S6xTable)slDupTables[s6xDup.DuplicateAddress];
                        s6xTable.XdfUniqueId = xdfUniqueId;
                        s6xTable.Store = true;
                        s6xTable.Skip = false;
                        if (s6xTable.ColsScalerAddress != null && s6xTable.ColsScalerAddress != string.Empty)
                        {
                            if (slFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)slFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                            else if (slDupFunctions.ContainsKey(s6xTable.ColsScalerAddress)) s6xTable.ColsScalerXdfUniqueId = ((S6xFunction)slDupFunctions[s6xTable.ColsScalerAddress]).XdfUniqueId;
                            else s6xTable.ColsScalerXdfUniqueId = string.Empty;
                        }
                        if (s6xTable.RowsScalerAddress != null && s6xTable.RowsScalerAddress != string.Empty)
                        {
                            if (slFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)slFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                            else if (slDupFunctions.ContainsKey(s6xTable.RowsScalerAddress)) s6xTable.RowsScalerXdfUniqueId = ((S6xFunction)slDupFunctions[s6xTable.RowsScalerAddress]).XdfUniqueId;
                            else s6xTable.RowsScalerXdfUniqueId = string.Empty;
                        }
                        slDupTables[s6xTable.DuplicateAddress] = s6xTable;
                        alS6xDupUpdates.Add(s6xTable.DuplicateAddress);

                        xdfObject.Import(s6xTable, xdfBaseOffset);
                        s6xTable = null;

                        break;
                    }
                }
            }

            if (xdfFile.xdfScalars != null)
            {
                foreach (XdfScalar xdfObject in xdfFile.xdfScalars)
                {
                    xdfUniqueId = xdfObject.uniqueid;
                    uniqueAddress = Tools.UniqueAddress(sadBin.Calibration.BankNum, Tools.binAddressFromXdfAddress(xdfObject.getMmedAddress(), xdfBaseOffset), sadBin.Calibration.BankAddressBinInt);
                    if (!slScalars.ContainsKey(uniqueAddress)) continue;

                    if (((S6xScalar)slScalars[uniqueAddress]).XdfUniqueId == xdfUniqueId)
                    {
                        // Already Matched on xdfUniqueId
                        if (slProcessed.ContainsKey(uniqueAddress))
                        {
                            if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                            slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            continue;
                        }
                        slProcessed.Add(uniqueAddress, 1);
                    }
                    else if (((S6xScalar)slScalars[uniqueAddress]).Label == xdfObject.title)
                    {
                        // Matching on Title
                        if (slProcessed.ContainsKey(uniqueAddress))
                        {
                            if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                            slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            continue;
                        }
                        slProcessed.Add(uniqueAddress, 1);
                    }
                    if (slProcessed.ContainsKey(uniqueAddress))
                    {
                        // Matched on main Item
                        ((S6xScalar)slScalars[uniqueAddress]).XdfUniqueId = xdfUniqueId;
                        ((S6xScalar)slScalars[uniqueAddress]).Store = true;
                        ((S6xScalar)slScalars[uniqueAddress]).Skip = false;
                        alS6xUpdates.Add(uniqueAddress);

                        xdfObject.Import((S6xScalar)slScalars[uniqueAddress], xdfBaseOffset);
                        continue;
                    }

                    // Real Duplicates
                    foreach (S6xScalar s6xDup in slDupScalars)
                    {
                        if (uniqueAddress != s6xDup.UniqueAddress) continue;

                        if (((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).XdfUniqueId == xdfUniqueId)
                        {
                            // Already Matched on xdfUniqueId
                            if (slProcessed.ContainsKey(uniqueAddress))
                            {
                                if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                                slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            }
                            else
                            {
                                slProcessed.Add(uniqueAddress, 1);
                            }
                        }
                        else if (((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).Label == xdfObject.title)
                        {
                            // Matching on Title
                            if (slProcessed.ContainsKey(uniqueAddress))
                            {
                                if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                                slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            }
                            else
                            {
                                slProcessed.Add(uniqueAddress, 1);
                            }
                        }
                        else
                        {
                            continue;
                        }

                        ((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).XdfUniqueId = xdfUniqueId;
                        ((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).Store = true;
                        ((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).Skip = false;
                        alS6xDupUpdates.Add(s6xDup.DuplicateAddress);

                        xdfObject.Import((S6xScalar)slDupScalars[s6xDup.DuplicateAddress], xdfBaseOffset);

                        break;
                    }
                }
            }

            if (xdfFile.xdfFlags != null)
            {
                foreach (XdfFlag xdfObject in xdfFile.xdfFlags)
                {
                    xdfUniqueId = xdfObject.uniqueid;
                    uniqueAddress = Tools.UniqueAddress(sadBin.Calibration.BankNum, Tools.binAddressFromXdfAddress(xdfObject.getMmedAddress(), xdfBaseOffset), sadBin.Calibration.BankAddressBinInt);

                    if (!slScalars.ContainsKey(uniqueAddress)) continue;

                    if (((S6xScalar)slScalars[uniqueAddress]).XdfUniqueId == xdfUniqueId)
                    {
                        // Already Matched on xdfUniqueId
                        if (slProcessed.ContainsKey(uniqueAddress))
                        {
                            if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                            slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            continue;
                        }
                        slProcessed.Add(uniqueAddress, 1);
                    }
                    else if (((S6xScalar)slScalars[uniqueAddress]).Label == xdfObject.title)
                    {
                        // Matching on Title
                        if (slProcessed.ContainsKey(uniqueAddress))
                        {
                            if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                            slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            continue;
                        }
                        slProcessed.Add(uniqueAddress, 1);
                    }
                    if (slProcessed.ContainsKey(uniqueAddress))
                    {
                        // Matched on main Item
                        ((S6xScalar)slScalars[uniqueAddress]).Store = true;
                        ((S6xScalar)slScalars[uniqueAddress]).Skip = false;

                        foreach (S6xBitFlag bitFlag in ((S6xScalar)slScalars[uniqueAddress]).BitFlags)
                        {
                            if (bitFlag.Skip) continue;

                            try
                            {
                                int iMask = Convert.ToInt32(xdfObject.mask.Replace("0x", ""), 16);
                                if (iMask == (int)Math.Pow(2, bitFlag.Position))
                                {
                                    xdfObject.Import((S6xScalar)slScalars[uniqueAddress], bitFlag, xdfBaseOffset);
                                    alS6xUpdates.Add(uniqueAddress + "." + bitFlag.Position.ToString());
                                }
                            }
                            catch { }
                        }
                        continue;
                    }

                    // Real Duplicates
                    foreach (S6xScalar s6xDup in slDupScalars)
                    {
                        if (uniqueAddress != s6xDup.UniqueAddress) continue;

                        if (((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).XdfUniqueId == xdfUniqueId)
                        {
                            // Already Matched on xdfUniqueId
                            if (slProcessed.ContainsKey(uniqueAddress))
                            {
                                if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                                slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            }
                            else
                            {
                                slProcessed.Add(uniqueAddress, 1);
                            }
                        }
                        else if (((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).Label == xdfObject.title)
                        {
                            // Matching on Title
                            if (slProcessed.ContainsKey(uniqueAddress))
                            {
                                if (!alDuplicates.Contains(uniqueAddress)) alDuplicates.Add(uniqueAddress);
                                slProcessed[uniqueAddress] = (int)slProcessed[uniqueAddress] + 1;
                            }
                            else
                            {
                                slProcessed.Add(uniqueAddress, 1);
                            }
                        }
                        else
                        {
                            continue;
                        }

                        ((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).Store = true;
                        ((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).Skip = false;

                        foreach (S6xBitFlag bitFlag in ((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).BitFlags)
                        {
                            if (bitFlag.Skip) continue;

                            try
                            {
                                int iMask = Convert.ToInt32(xdfObject.mask.Replace("0x", ""), 16);
                                if (iMask == (int)Math.Pow(2, bitFlag.Position))
                                {
                                    xdfObject.Import((S6xScalar)slDupScalars[s6xDup.DuplicateAddress], bitFlag, xdfBaseOffset);
                                    alS6xDupUpdates.Add(s6xDup.DuplicateAddress + "." + bitFlag.Position.ToString());
                                }
                            }
                            catch { }
                        }

                        break;
                    }
                }
            }
            
            // Missing Xdf other Objects Creation
            if (xdfFile.xdfTables == null) alXdfObjects = new ArrayList();
            else alXdfObjects = new ArrayList(xdfFile.xdfTables);

            alS6xDupUniqueAddresses = new ArrayList();
            foreach (S6xTable s6xObject in slDupTables.Values) if (!alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress)) alS6xDupUniqueAddresses.Add(s6xObject.UniqueAddress);
            
            foreach (S6xTable s6xObject in slTables.Values)
            {
                if (!s6xObject.Skip && s6xObject.Store && !alS6xUpdates.Contains(s6xObject.UniqueAddress) && !alS6xCreations.Contains(s6xObject.UniqueAddress) && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    // Trying to find Scalers Unique Xdf Ids
                    if (s6xObject.ColsScalerAddress != null && s6xObject.ColsScalerAddress != string.Empty)
                    {
                        if (slFunctions.ContainsKey(s6xObject.ColsScalerAddress)) s6xObject.ColsScalerXdfUniqueId = ((S6xFunction)slFunctions[s6xObject.ColsScalerAddress]).XdfUniqueId;
                        else if (slDupFunctions.ContainsKey(s6xObject.ColsScalerAddress)) s6xObject.ColsScalerXdfUniqueId = ((S6xFunction)slDupFunctions[s6xObject.ColsScalerAddress]).XdfUniqueId;
                        else s6xObject.ColsScalerXdfUniqueId = string.Empty;
                    }
                    if (s6xObject.RowsScalerAddress != null && s6xObject.RowsScalerAddress != string.Empty)
                    {
                        if (slFunctions.ContainsKey(s6xObject.RowsScalerAddress)) s6xObject.RowsScalerXdfUniqueId = ((S6xFunction)slFunctions[s6xObject.RowsScalerAddress]).XdfUniqueId;
                        else if (slDupFunctions.ContainsKey(s6xObject.RowsScalerAddress)) s6xObject.RowsScalerXdfUniqueId = ((S6xFunction)slDupFunctions[s6xObject.RowsScalerAddress]).XdfUniqueId;
                        else s6xObject.RowsScalerXdfUniqueId = string.Empty;
                    }
                    XdfTable xdfObject = new XdfTable(s6xObject, xdfBaseOffset);
                    xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                    newXdfUniqueId++;
                    alXdfObjects.Add(xdfObject);
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xTable s6xDupObject in slDupTables.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && s6xDupObject.Store && !alS6xDupUpdates.Contains(s6xDupObject.DuplicateAddress) && s6xDupObject.AddressBinInt >= xdfBaseOffset)
                        {
                            // Trying to find Scalers Unique Xdf Ids
                            if (s6xDupObject.ColsScalerAddress != null && s6xDupObject.ColsScalerAddress != string.Empty)
                            {
                                if (slFunctions.ContainsKey(s6xDupObject.ColsScalerAddress)) s6xDupObject.ColsScalerXdfUniqueId = ((S6xFunction)slFunctions[s6xDupObject.ColsScalerAddress]).XdfUniqueId;
                                else if (slDupFunctions.ContainsKey(s6xDupObject.ColsScalerAddress)) s6xDupObject.ColsScalerXdfUniqueId = ((S6xFunction)slDupFunctions[s6xDupObject.ColsScalerAddress]).XdfUniqueId;
                                else s6xDupObject.ColsScalerXdfUniqueId = string.Empty;
                            }
                            if (s6xDupObject.RowsScalerAddress != null && s6xDupObject.RowsScalerAddress != string.Empty)
                            {
                                if (slFunctions.ContainsKey(s6xDupObject.RowsScalerAddress)) s6xDupObject.RowsScalerXdfUniqueId = ((S6xFunction)slFunctions[s6xDupObject.RowsScalerAddress]).XdfUniqueId;
                                else if (slDupFunctions.ContainsKey(s6xDupObject.RowsScalerAddress)) s6xDupObject.RowsScalerXdfUniqueId = ((S6xFunction)slDupFunctions[s6xDupObject.RowsScalerAddress]).XdfUniqueId;
                                else s6xDupObject.RowsScalerXdfUniqueId = string.Empty;
                            }
                            XdfTable xdfObject = new XdfTable(s6xDupObject, xdfBaseOffset);
                            xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                            newXdfUniqueId++;
                            alXdfObjects.Add(xdfObject);
                        }
                    }
                }
            }
            xdfFile.xdfTables = (XdfTable[])alXdfObjects.ToArray(typeof(XdfTable));
            alXdfObjects = null;
            alS6xDupUniqueAddresses = null;

            if (xdfFile.xdfScalars == null) alXdfObjects = new ArrayList();
            else alXdfObjects = new ArrayList(xdfFile.xdfScalars);

            alS6xDupUniqueAddresses = new ArrayList();
            foreach (S6xScalar s6xObject in slDupScalars.Values) if (!alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress)) alS6xDupUniqueAddresses.Add(s6xObject.UniqueAddress);

            foreach (S6xScalar s6xObject in slScalars.Values)
            {
                if (!s6xObject.Skip && s6xObject.Store && !alS6xUpdates.Contains(s6xObject.UniqueAddress) && !alS6xCreations.Contains(s6xObject.UniqueAddress) && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    XdfScalar xdfObject = new XdfScalar(s6xObject, xdfBaseOffset);
                    xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                    newXdfUniqueId++;
                    alXdfObjects.Add(xdfObject);
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xScalar s6xDupObject in slDupScalars.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && s6xDupObject.Store && !alS6xDupUpdates.Contains(s6xDupObject.DuplicateAddress) && s6xDupObject.AddressBinInt >= xdfBaseOffset)
                        {
                            XdfScalar xdfObject = new XdfScalar(s6xDupObject, xdfBaseOffset);
                            xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                            newXdfUniqueId++;
                            alXdfObjects.Add(xdfObject);
                        }
                    }
                }
            }
            xdfFile.xdfScalars = (XdfScalar[])alXdfObjects.ToArray(typeof(XdfScalar));
            alXdfObjects = null;
            alS6xDupUniqueAddresses = null;

            if (xdfFile.xdfFlags == null) alXdfObjects = new ArrayList();
            else alXdfObjects = new ArrayList(xdfFile.xdfFlags);
            foreach (S6xScalar s6xObject in slScalars.Values)
            {
                if (!s6xObject.Skip && s6xObject.Store && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    if (s6xObject.isBitFlags)
                    {
                        if (s6xObject.BitFlags != null)
                        {
                            foreach (S6xBitFlag bitFlag in s6xObject.BitFlags)
                            {
                                if (bitFlag.Skip) continue;
                                uniqueAddress = s6xObject.UniqueAddress + "." + bitFlag.Position.ToString();
                                if (!alS6xUpdates.Contains(uniqueAddress))
                                {
                                    XdfFlag xdfFlag = new XdfFlag(s6xObject, bitFlag, xdfBaseOffset);
                                    xdfFlag.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                                    newXdfUniqueId++;
                                    alXdfObjects.Add(xdfFlag);
                                }
                            }
                        }
                    }
                }
            }
            foreach (S6xScalar s6xObject in slDupScalars.Values)
            {
                if (!s6xObject.Skip && s6xObject.Store && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    if (s6xObject.isBitFlags)
                    {
                        if (s6xObject.BitFlags != null)
                        {
                            foreach (S6xBitFlag bitFlag in s6xObject.BitFlags)
                            {
                                if (bitFlag.Skip) continue;
                                if (!alS6xDupUpdates.Contains(s6xObject.DuplicateAddress + "." + bitFlag.Position.ToString()))
                                {
                                    XdfFlag xdfFlag = new XdfFlag(s6xObject, bitFlag, xdfBaseOffset);
                                    xdfFlag.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                                    newXdfUniqueId++;
                                    alXdfObjects.Add(xdfFlag);
                                }
                            }
                        }
                    }
                }
            }
            xdfFile.xdfFlags = (XdfFlag[])alXdfObjects.ToArray(typeof(XdfFlag));
            alXdfObjects = null;

            arrResult[0] = alS6xUpdates.ToArray(typeof(string));
            arrResult[1] = alS6xCreations.ToArray(typeof(string));
            arrResult[2] = alDuplicates.ToArray(typeof(string));
            arrResult[3] = new string[] { };
            arrResult[4] = new string[] { };
            arrResult[5] = alS6xDupUpdates.ToArray(typeof(string));
            arrResult[6] = new string[] { };

            alS6xUpdates = null;
            alS6xCreations = null;
            alDuplicates = null;
            slProcessed = null;
            alS6xDupUpdates = null;

            isSaved = false;

            GC.Collect();

            return arrResult;
        }

        public void MatchBanksProperties(ref SortedList slBanksInfos)
        {
            if (slBanksInfos == null) return;
            if (Properties.BanksProperties == null) return;

            SortedList slBanksCorrections = new SortedList();
            
            foreach (S6xBankProperties bankProperties in Properties.BanksProperties)
            {
                foreach (string[] bankInfos in slBanksInfos.Values)
                {
                    if (bankProperties.BankNum != bankInfos[0]) continue;

                    if (bankProperties.AddressBin != bankInfos[1])
                    {
                        try { slBanksCorrections.Add(Convert.ToInt32(bankProperties.BankNum), Convert.ToInt32(bankInfos[1], 16) - Convert.ToInt32(bankProperties.AddressBin, 16)); }
                        catch { }
                    }
                    break;
                }
            }

            if (slBanksCorrections.Count > 0)
            {
        
                foreach (S6xTable s6xObject in slTables.Values) if (slBanksCorrections.ContainsKey(s6xObject.BankNum)) s6xObject.AddressBinInt += (int)slBanksCorrections[s6xObject.BankNum];
                foreach (S6xFunction s6xObject in slFunctions.Values) if (slBanksCorrections.ContainsKey(s6xObject.BankNum)) s6xObject.AddressBinInt += (int)slBanksCorrections[s6xObject.BankNum];
                foreach (S6xScalar s6xObject in slScalars.Values) if (slBanksCorrections.ContainsKey(s6xObject.BankNum)) s6xObject.AddressBinInt += (int)slBanksCorrections[s6xObject.BankNum];
                foreach (S6xStructure s6xObject in slStructures.Values) if (slBanksCorrections.ContainsKey(s6xObject.BankNum)) s6xObject.AddressBinInt += (int)slBanksCorrections[s6xObject.BankNum];

                foreach (S6xTable s6xObject in slDupTables.Values) if (slBanksCorrections.ContainsKey(s6xObject.BankNum)) s6xObject.AddressBinInt += (int)slBanksCorrections[s6xObject.BankNum];
                foreach (S6xFunction s6xObject in slDupFunctions.Values) if (slBanksCorrections.ContainsKey(s6xObject.BankNum)) s6xObject.AddressBinInt += (int)slBanksCorrections[s6xObject.BankNum];
                foreach (S6xScalar s6xObject in slDupScalars.Values) if (slBanksCorrections.ContainsKey(s6xObject.BankNum)) s6xObject.AddressBinInt += (int)slBanksCorrections[s6xObject.BankNum];
                foreach (S6xStructure s6xObject in slDupStructures.Values) if (slBanksCorrections.ContainsKey(s6xObject.BankNum)) s6xObject.AddressBinInt += (int)slBanksCorrections[s6xObject.BankNum];

                isSaved = false;
            }

            slBanksCorrections = null;
        }

        public void ResetBanksProperties(ref SortedList slBanksInfos)
        {
            Properties.BanksProperties = null;

            if (slBanksInfos == null) return;
            if (slBanksInfos.Count == 0) return;

            Properties.BanksProperties = new S6xBankProperties[slBanksInfos.Count];
            for (int iBank = 0; iBank < slBanksInfos.Count; iBank++)
            {
                string[] bankInfos = (string[])slBanksInfos.GetByIndex(iBank);
                Properties.BanksProperties[iBank] = new S6xBankProperties();
                Properties.BanksProperties[iBank].BankNum = bankInfos[0];
                Properties.BanksProperties[iBank].AddressBin = bankInfos[1];
            }
        }
    }

    [Serializable]
    [XmlRoot("Table")]
    public class S6xTable
    {
        [XmlAttribute]
        public int BankNum { get; set; }
        [XmlAttribute]
        public int AddressInt { get; set; }
        [XmlAttribute]
        public int DuplicateNum { get; set; }
        [XmlAttribute]
        public int AddressBinInt { get; set; }
        [XmlAttribute]
        public string XdfUniqueId { get; set; }

        [XmlAttribute]
        public bool isCalibrationElement { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }

        [XmlAttribute]
        public bool WordOutput { get; set; }
        [XmlAttribute]
        public bool SignedOutput { get; set; }

        [XmlAttribute]
        public int ColsNumber { get; set; }
        [XmlAttribute]
        public int RowsNumber { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        public string CellsScaleExpression { get; set; }
        
        public string ColsScalerAddress { get; set; }
        public string RowsScalerAddress { get; set; }

        public string ColsScalerXdfUniqueId { get; set; }
        public string RowsScalerXdfUniqueId { get; set; }

        public string ColsUnits { get; set; }
        public string RowsUnits { get; set; }
        public string CellsUnits { get; set; }

        [XmlIgnore]
        public bool Store { get; set; }

        [XmlIgnore]
        public string Information { get; set; }

        [XmlIgnore]
        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }

        [XmlIgnore]
        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        [XmlIgnore]
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }
        [XmlIgnore]
        public string DuplicateAddress { get { return string.Format("{0,1} {1,5} {2,2}", BankNum, AddressInt, DuplicateNum); } }

        public S6xTable()
        {
        }

        public S6xTable(CalibrationElement tableCalElem)
        {
            BankNum = tableCalElem.BankNum;
            AddressInt = tableCalElem.AddressInt;
            AddressBinInt = tableCalElem.AddressBinInt;
            isCalibrationElement = true;
            Skip = false;
            WordOutput = tableCalElem.TableElem.WordOutput;
            SignedOutput = tableCalElem.TableElem.SignedOutput;
            ColsNumber = tableCalElem.TableElem.ColsNumber;
            RowsNumber = 0;
            if (tableCalElem.TableElem.Lines != null) RowsNumber = tableCalElem.TableElem.Lines.Length;
            Label = tableCalElem.TableElem.Label;
            if (Label == string.Empty) Label = tableCalElem.TableElem.RBaseCalc;
            ShortLabel = tableCalElem.TableElem.ShortLabel;
            if (ShortLabel == string.Empty) ShortLabel = tableCalElem.TableElem.Address;
            Comments = tableCalElem.TableElem.FullLabel;
            ColsScalerAddress = tableCalElem.TableElem.ColsScalerUniqueAddress;  // From Forced Values at lower level
            RowsScalerAddress = tableCalElem.TableElem.RowsScalerUniqueAddress;  // From Forced Values at lower level
            CellsScaleExpression = tableCalElem.TableElem.CellsScaleExpression;  // From Forced Values at lower level
            ColsUnits = string.Empty;
            RowsUnits = string.Empty;
            CellsUnits = string.Empty;

            // Forced Values (Initially from S6x Inputs)
            foreach (RoutineCallInfoTable ciCI in tableCalElem.TableElem.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputTable != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputTable.ForcedColsUnits != null && ciCI.RoutineInputOutput.S6xInputTable.ForcedColsUnits != string.Empty)
                        {
                            ColsUnits = ciCI.RoutineInputOutput.S6xInputTable.ForcedColsUnits;
                        }
                        if (ciCI.RoutineInputOutput.S6xInputTable.ForcedRowsUnits != null && ciCI.RoutineInputOutput.S6xInputTable.ForcedRowsUnits != string.Empty)
                        {
                            RowsUnits = ciCI.RoutineInputOutput.S6xInputTable.ForcedRowsUnits;
                        }
                        if (ciCI.RoutineInputOutput.S6xInputTable.ForcedCellsUnits != null && ciCI.RoutineInputOutput.S6xInputTable.ForcedCellsUnits != string.Empty)
                        {
                            CellsUnits = ciCI.RoutineInputOutput.S6xInputTable.ForcedCellsUnits;
                        }
                    }
                }
                if (ColsUnits != string.Empty && RowsUnits != string.Empty && CellsUnits != string.Empty) break;
            }

            // Forced Values (Initially from S6x Internals)
            if (tableCalElem.TableElem.S6xElementSignatureSource != null)
            {
                if (tableCalElem.TableElem.S6xElementSignatureSource.Table != null)
                {
                    if (tableCalElem.TableElem.S6xElementSignatureSource.Table.Comments != null && tableCalElem.TableElem.S6xElementSignatureSource.Table.Comments != string.Empty)
                    {
                        Comments = tableCalElem.TableElem.S6xElementSignatureSource.Table.Comments;
                    }
                    if (tableCalElem.TableElem.S6xElementSignatureSource.Table.ColsUnits != null && tableCalElem.TableElem.S6xElementSignatureSource.Table.ColsUnits != string.Empty)
                    {
                        ColsUnits = tableCalElem.TableElem.S6xElementSignatureSource.Table.ColsUnits;
                    }
                    if (tableCalElem.TableElem.S6xElementSignatureSource.Table.RowsUnits != null && tableCalElem.TableElem.S6xElementSignatureSource.Table.RowsUnits != string.Empty)
                    {
                        RowsUnits = tableCalElem.TableElem.S6xElementSignatureSource.Table.RowsUnits;
                    }
                    if (tableCalElem.TableElem.S6xElementSignatureSource.Table.CellsUnits != null && tableCalElem.TableElem.S6xElementSignatureSource.Table.CellsUnits != string.Empty)
                    {
                        CellsUnits = tableCalElem.TableElem.S6xElementSignatureSource.Table.CellsUnits;
                    }
                }
            }
        }

        public S6xTable(Table table, int bankBinAddress)
        {
            BankNum = table.BankNum;
            AddressInt = table.AddressInt;
            AddressBinInt = table.AddressInt + bankBinAddress;
            isCalibrationElement = false;
            Skip = false;
            WordOutput = table.WordOutput;
            SignedOutput = table.SignedOutput;
            ColsNumber = table.ColsNumber;
            RowsNumber = 0;
            if (table.Lines != null) RowsNumber = table.Lines.Length;
            Label = table.Label;
            ShortLabel = table.ShortLabel;
            Comments = string.Empty;
            ColsScalerAddress = table.ColsScalerUniqueAddress;  // From Forced Values at lower level
            RowsScalerAddress = table.RowsScalerUniqueAddress;  // From Forced Values at lower level
            CellsScaleExpression = table.CellsScaleExpression;  // From Forced Values at lower level
            ColsUnits = string.Empty;
            RowsUnits = string.Empty;
            CellsUnits = string.Empty;

            // Forced Values (Initially from S6x Inputs)
            foreach (RoutineCallInfoTable ciCI in table.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputTable != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputTable.ForcedColsUnits != null && ciCI.RoutineInputOutput.S6xInputTable.ForcedColsUnits != string.Empty)
                        {
                            ColsUnits = ciCI.RoutineInputOutput.S6xInputTable.ForcedColsUnits;
                        }
                        if (ciCI.RoutineInputOutput.S6xInputTable.ForcedRowsUnits != null && ciCI.RoutineInputOutput.S6xInputTable.ForcedRowsUnits != string.Empty)
                        {
                            RowsUnits = ciCI.RoutineInputOutput.S6xInputTable.ForcedRowsUnits;
                        }
                        if (ciCI.RoutineInputOutput.S6xInputTable.ForcedCellsUnits != null && ciCI.RoutineInputOutput.S6xInputTable.ForcedCellsUnits != string.Empty)
                        {
                            CellsUnits = ciCI.RoutineInputOutput.S6xInputTable.ForcedCellsUnits;
                        }
                    }
                }
                if (ColsUnits != string.Empty && RowsUnits != string.Empty && CellsUnits != string.Empty) break;
            }

            // Forced Values (Initially from S6x Internals)
            if (table.S6xElementSignatureSource != null)
            {
                if (table.S6xElementSignatureSource.Table != null)
                {
                    if (table.S6xElementSignatureSource.Table.Comments != null && table.S6xElementSignatureSource.Table.Comments != string.Empty)
                    {
                        Comments = table.S6xElementSignatureSource.Table.Comments;
                    }
                    if (table.S6xElementSignatureSource.Table.ColsUnits != null && table.S6xElementSignatureSource.Table.ColsUnits != string.Empty)
                    {
                        ColsUnits = table.S6xElementSignatureSource.Table.ColsUnits;
                    }
                    if (table.S6xElementSignatureSource.Table.RowsUnits != null && table.S6xElementSignatureSource.Table.RowsUnits != string.Empty)
                    {
                        RowsUnits = table.S6xElementSignatureSource.Table.RowsUnits;
                    }
                    if (table.S6xElementSignatureSource.Table.CellsUnits != null && table.S6xElementSignatureSource.Table.CellsUnits != string.Empty)
                    {
                        CellsUnits = table.S6xElementSignatureSource.Table.CellsUnits;
                    }
                }
            }
        }

        public S6xTable(XdfTable xdfTable, int bankNum, int address, int addressBin, bool isCalElem, ref SortedList slFunctions)
        {
            XdfUniqueId = xdfTable.uniqueid;
            BankNum = bankNum;
            AddressInt = address;
            AddressBinInt = addressBin;
            isCalibrationElement = isCalElem;
            Store = true;
            Skip = false;
            Label = xdfTable.titleXmlValid;
            Comments = xdfTable.descriptionXmlValid;
            ShortLabel = Tools.XDFLabelComToShortLabel(Label, Comments, "TpTb" + UniqueAddressHex.Replace(" ", "_"));
            Label = Tools.XDFLabelReview(Label, ShortLabel);

            ColsScalerAddress = string.Empty;
            RowsScalerAddress = string.Empty;

            ColsNumber = 0;
            RowsNumber = 0;

            WordOutput = false;
            
            foreach (XdfAxis xdfAxis in xdfTable.xdfAxis)
            {
                switch (xdfAxis.id.ToLower())
                {
                    case "z":
                        if (xdfAxis.xdfData != null)
                        {
                            if (ColsNumber == 0)
                            {
                                try { ColsNumber = Convert.ToInt32(xdfAxis.xdfData.mmedcolcount); }
                                catch { }
                            }
                            if (RowsNumber == 0)
                            {
                                try { RowsNumber = Convert.ToInt32(xdfAxis.xdfData.mmedrowcount); }
                                catch { }
                            }

                            if (xdfAxis.xdfData.mmedelementsizebits == "8") WordOutput = false;
                            else WordOutput = true;

                            switch (xdfAxis.xdfData.mmedtypeflags)
                            {
                                case "0x01":
                                case "0x03":
                                case "0x05":
                                case "0x07":
                                    SignedOutput = true;
                                    break;
                                default:
                                    SignedOutput = false;
                                    break;
                            }
                        }

                        if (xdfAxis.xdfMath != null) CellsScaleExpression = xdfAxis.xdfMath.equation;
                        CellsUnits = xdfAxis.units;
                        break;
                    case "x":
                        if (ColsNumber == 0)
                        {
                            try { ColsNumber = Convert.ToInt32(xdfAxis.indexcount); }
                            catch { }
                        }
                        ColsUnits = xdfAxis.units;
                        if (xdfAxis.xdfInfo != null)
                        {
                            ColsScalerXdfUniqueId = xdfAxis.xdfInfo.linkobjid;
                            foreach (S6xFunction s6xFunction in slFunctions.Values)
                            {
                                if (s6xFunction.XdfUniqueId == ColsScalerXdfUniqueId)
                                {
                                    ColsScalerAddress = s6xFunction.UniqueAddress;
                                    break;
                                }
                            }
                        }
                        break;
                    case "y":
                        if (RowsNumber == 0)
                        {
                            try { RowsNumber = Convert.ToInt32(xdfAxis.indexcount); }
                            catch { }
                        }
                        RowsUnits = xdfAxis.units;
                        if (xdfAxis.xdfInfo != null)
                        {
                            RowsScalerXdfUniqueId = xdfAxis.xdfInfo.linkobjid;
                            foreach (S6xFunction s6xFunction in slFunctions.Values)
                            {
                                if (s6xFunction.XdfUniqueId == RowsScalerXdfUniqueId)
                                {
                                    RowsScalerAddress = s6xFunction.UniqueAddress;
                                    break;
                                }
                            }
                        }
                        break;
                }
            }
        }

        public S6xTable Clone()
        {
            S6xTable clone = new S6xTable();
            clone.BankNum = BankNum;
            clone.AddressInt = AddressInt;
            clone.AddressBinInt = AddressBinInt;
            clone.Skip = Skip;
            clone.Store = Store;

            clone.isCalibrationElement = isCalibrationElement;

            clone.XdfUniqueId = XdfUniqueId;

            clone.WordOutput = WordOutput;
            clone.SignedOutput = SignedOutput;

            clone.ColsNumber = ColsNumber;
            clone.RowsNumber = RowsNumber;

            clone.CellsScaleExpression = CellsScaleExpression;

            clone.ColsScalerAddress = ColsScalerAddress;
            clone.RowsScalerAddress = RowsScalerAddress;

            clone.ColsScalerXdfUniqueId = ColsScalerXdfUniqueId;
            clone.RowsScalerXdfUniqueId = RowsScalerXdfUniqueId;

            clone.ColsUnits = ColsUnits;
            clone.RowsUnits = RowsUnits;
            clone.CellsUnits = CellsUnits;

            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;

            return clone;
        }

        public bool isUserDefined
        {
            get
            {
                if (ColsUnits != null && ColsUnits != string.Empty) return true;
                if (RowsUnits != null && RowsUnits != string.Empty) return true;
                if (CellsUnits != null && CellsUnits != string.Empty) return true;
                if (OutputComments) return true;
                if (CellsScaleExpression != null) if (CellsScaleExpression.ToUpper() != "X") return true;

                if (ShortLabel == null) return true;
                if (Label == null) return true;

                if (isCalibrationElement)
                {
                    if (Comments == null) return true;
                    if (ShortLabel != SADDef.ShortTablePrefix + SADDef.NamingShortBankSeparator + Address && !ShortLabel.StartsWith(SADDef.ShortTablePrefix)) return true;
                    if (Label != SADDef.LongTablePrefix + Address && !Label.StartsWith(SADDef.LongTablePrefix)) return true;
                    if (Comments != ShortLabel + " - " + Label) return true;
                }
                else
                {
                    if (Comments != null && Comments != string.Empty) return true;
                    if (ShortLabel != SADDef.ShortExtTablePrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator) && !ShortLabel.StartsWith(SADDef.ShortExtTablePrefix + BankNum.ToString() + SADDef.NamingShortBankSeparator)) return true;
                    if (Label != SADDef.LongExtTablePrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator) && !Label.StartsWith(SADDef.LongExtTablePrefix + BankNum.ToString() + SADDef.NamingShortBankSeparator)) return true;
                }

                return false;
            }
        }
    }

    [Serializable]
    [XmlRoot("Function")]
    public class S6xFunction
    {
        [XmlAttribute]
        public int BankNum { get; set; }
        [XmlAttribute]
        public int AddressInt { get; set; }
        [XmlAttribute]
        public int DuplicateNum { get; set; }
        [XmlAttribute]
        public int AddressBinInt { get; set; }
        [XmlAttribute]
        public string XdfUniqueId { get; set; }

        [XmlAttribute]
        public bool isCalibrationElement { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }

        [XmlAttribute]
        public bool ByteInput { get; set; }
        [XmlAttribute]
        public bool ByteOutput { get; set; }

        [XmlAttribute]
        public bool SignedInput { get; set; }
        [XmlAttribute]
        public bool SignedOutput { get; set; }

        [XmlAttribute]
        public int RowsNumber { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        public string InputScaleExpression { get; set; }
        public string OutputScaleExpression { get; set; }

        public string InputUnits { get; set; }
        public string OutputUnits { get; set; }

        [XmlIgnore]
        public bool Store { get; set; }

        [XmlIgnore]
        public string Information { get; set; }

        [XmlIgnore]
        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }

        [XmlIgnore]
        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        [XmlIgnore]
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }
        [XmlIgnore]
        public string DuplicateAddress { get { return string.Format("{0,1} {1,5} {2,2}", BankNum, AddressInt, DuplicateNum); } }

        public S6xFunction()
        {

        }

        public S6xFunction(CalibrationElement functionCalElem)
        {
            BankNum = functionCalElem.BankNum;
            AddressInt = functionCalElem.AddressInt;
            AddressBinInt = functionCalElem.AddressBinInt;
            isCalibrationElement = true;
            Skip = false;
            ByteInput = functionCalElem.FunctionElem.ByteInput;
            ByteOutput = functionCalElem.FunctionElem.ByteOutput;
            SignedInput = functionCalElem.FunctionElem.SignedInput;
            SignedOutput = functionCalElem.FunctionElem.SignedOutput;
            RowsNumber = 0;
            if (functionCalElem.FunctionElem.Lines != null) RowsNumber = functionCalElem.FunctionElem.Lines.Length;
            Label = functionCalElem.FunctionElem.Label;
            if (Label == string.Empty) Label = functionCalElem.FunctionElem.RBaseCalc;
            ShortLabel = functionCalElem.FunctionElem.ShortLabel;
            if (ShortLabel == string.Empty) ShortLabel = functionCalElem.FunctionElem.Address;
            Comments = functionCalElem.FunctionElem.FullLabel;
            InputScaleExpression = functionCalElem.FunctionElem.InputScaleExpression;       // From Forced Values at lower level
            OutputScaleExpression = functionCalElem.FunctionElem.OutputScaleExpression;     // From Forced Values at lower level
            InputUnits = string.Empty;
            OutputUnits = string.Empty;

            // Forced Values (Initially from S6x Inputs)
            foreach (RoutineCallInfoFunction ciCI in functionCalElem.FunctionElem.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputFunction != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputFunction.ForcedInputUnits != null && ciCI.RoutineInputOutput.S6xInputFunction.ForcedInputUnits != string.Empty)
                        {
                            InputUnits = ciCI.RoutineInputOutput.S6xInputFunction.ForcedInputUnits;
                        }
                        if (ciCI.RoutineInputOutput.S6xInputFunction.ForcedOutputUnits != null && ciCI.RoutineInputOutput.S6xInputFunction.ForcedOutputUnits != string.Empty)
                        {
                            OutputUnits = ciCI.RoutineInputOutput.S6xInputFunction.ForcedOutputUnits;
                        }
                    }
                }
                if (InputUnits != string.Empty && OutputUnits != string.Empty) break;
            }

            // Forced Values (Initially from S6x Internals)
            if (functionCalElem.FunctionElem.S6xElementSignatureSource != null)
            {
                if (functionCalElem.FunctionElem.S6xElementSignatureSource.Function != null)
                {
                    if (functionCalElem.FunctionElem.S6xElementSignatureSource.Function.Comments != null && functionCalElem.FunctionElem.S6xElementSignatureSource.Function.Comments != string.Empty)
                    {
                        Comments = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.Comments;
                    }
                    if (functionCalElem.FunctionElem.S6xElementSignatureSource.Function.InputUnits != null && functionCalElem.FunctionElem.S6xElementSignatureSource.Function.InputUnits != string.Empty)
                    {
                        InputUnits = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.InputUnits;
                    }
                    if (functionCalElem.FunctionElem.S6xElementSignatureSource.Function.OutputUnits != null && functionCalElem.FunctionElem.S6xElementSignatureSource.Function.OutputUnits != string.Empty)
                    {
                        OutputUnits = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.OutputUnits;
                    }
                }
            }
        }

        public S6xFunction(Function function, int bankBinAddress)
        {
            BankNum = function.BankNum;
            AddressInt = function.AddressInt;
            AddressBinInt = function.AddressInt + bankBinAddress;
            isCalibrationElement = false;
            Skip = false;
            ByteInput = function.ByteInput;
            ByteOutput = function.ByteOutput;
            SignedInput = function.SignedInput;
            SignedOutput = function.SignedOutput;
            RowsNumber = 0;
            if (function.Lines != null) RowsNumber = function.Lines.Length;
            Label = function.Label;
            ShortLabel = function.ShortLabel;
            Comments = string.Empty;
            InputScaleExpression = function.InputScaleExpression;         // From Forced Values at lower level
            OutputScaleExpression = function.OutputScaleExpression;       // From Forced Values at lower level
            InputUnits = string.Empty;
            OutputUnits = string.Empty;

            // Forced Values (Initially from S6x Inputs)
            foreach (RoutineCallInfoFunction ciCI in function.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputFunction != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputFunction.ForcedInputUnits != null && ciCI.RoutineInputOutput.S6xInputFunction.ForcedInputUnits != string.Empty)
                        {
                            InputUnits = ciCI.RoutineInputOutput.S6xInputFunction.ForcedInputUnits;
                        }
                        if (ciCI.RoutineInputOutput.S6xInputFunction.ForcedOutputUnits != null && ciCI.RoutineInputOutput.S6xInputFunction.ForcedOutputUnits != string.Empty)
                        {
                            OutputUnits = ciCI.RoutineInputOutput.S6xInputFunction.ForcedOutputUnits;
                        }
                    }
                }
                if (InputUnits != string.Empty && OutputUnits != string.Empty) break;
            }

            // Forced Values (Initially from S6x Internals)
            if (function.S6xElementSignatureSource != null)
            {
                if (function.S6xElementSignatureSource.Function != null)
                {
                    if (function.S6xElementSignatureSource.Function.Comments != null && function.S6xElementSignatureSource.Function.Comments != string.Empty)
                    {
                        Comments = function.S6xElementSignatureSource.Function.Comments;
                    }
                    if (function.S6xElementSignatureSource.Function.InputUnits != null && function.S6xElementSignatureSource.Function.InputUnits != string.Empty)
                    {
                        InputUnits = function.S6xElementSignatureSource.Function.InputUnits;
                    }
                    if (function.S6xElementSignatureSource.Function.OutputUnits != null && function.S6xElementSignatureSource.Function.OutputUnits != string.Empty)
                    {
                        OutputUnits = function.S6xElementSignatureSource.Function.OutputUnits;
                    }
                }
            }
        }

        public S6xFunction(XdfFunction xdfFunction, int bankNum, int address, int addressBin, bool isCalElem)
        {
            XdfUniqueId = xdfFunction.uniqueid;
            BankNum = bankNum;
            AddressInt = address;
            AddressBinInt = addressBin;
            isCalibrationElement = isCalElem;
            Store = true;
            Skip = false;
            Label = xdfFunction.titleXmlValid;
            Comments = xdfFunction.descriptionXmlValid;
            ShortLabel = Tools.XDFLabelComToShortLabel(Label, Comments, "TpFc" + UniqueAddressHex.Replace(" ", "_"));
            Label = Tools.XDFLabelReview(Label, ShortLabel);
            foreach (XdfAxis xdfAxis in xdfFunction.xdfAxis)
            {
                if (xdfAxis.id.ToLower() == "x")
                {
                    try { RowsNumber = Convert.ToInt32(xdfAxis.indexcount); }
                    catch
                    {
                        try { RowsNumber = Convert.ToInt32(xdfAxis.xdfData.mmedcolcount); }
                        catch { RowsNumber = 0; }
                    }

                    ByteInput = (xdfAxis.xdfData.mmedelementsizebits == "8");
                    switch (xdfAxis.xdfData.mmedtypeflags)
                    {
                        case "0x01":
                        case "0x03":
                        case "0x05":
                        case "0x07":
                            SignedInput = true;
                            break;
                        default:
                            SignedInput = false;
                            break;
                    }
                    InputScaleExpression = xdfAxis.xdfMath.equation;
                    InputUnits = xdfAxis.units;
                }
                else
                {
                    ByteOutput = (xdfAxis.xdfData.mmedelementsizebits == "8");
                    switch (xdfAxis.xdfData.mmedtypeflags)
                    {
                        case "0x01":
                        case "0x03":
                        case "0x05":
                        case "0x07":
                            SignedOutput = true;
                            break;
                        default:
                            SignedOutput = false;
                            break;
                    }
                    OutputScaleExpression = xdfAxis.xdfMath.equation;
                    OutputUnits = xdfAxis.units;
                }
            }
        }

        public S6xFunction Clone()
        {
            S6xFunction clone = new S6xFunction();
            clone.BankNum = BankNum;
            clone.AddressInt = AddressInt;
            clone.AddressBinInt = AddressBinInt;
            clone.Skip = Skip;
            clone.Store = Store;

            clone.isCalibrationElement = isCalibrationElement;

            clone.XdfUniqueId = XdfUniqueId;

            clone.ByteInput = ByteInput;
            clone.ByteOutput = ByteOutput;

            clone.SignedInput = SignedInput;
            clone.SignedOutput = SignedOutput;

            clone.RowsNumber = RowsNumber;

            clone.InputScaleExpression = InputScaleExpression;
            clone.OutputScaleExpression = OutputScaleExpression;

            clone.InputUnits = InputUnits;
            clone.OutputUnits = OutputUnits;

            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;

            return clone;
        }

        public bool isUserDefined
        {
            get
            {
                if (InputUnits != null && InputUnits != string.Empty) return true;
                if (OutputUnits != null && OutputUnits != string.Empty) return true;
                if (OutputComments) return true;
                if (InputScaleExpression != null) if (InputScaleExpression.ToUpper() != "X") return true;
                if (OutputScaleExpression != null) if (OutputScaleExpression.ToUpper() != "X") return true;

                if (ShortLabel == null) return true;
                if (Label == null) return true;

                if (isCalibrationElement)
                {
                    if (Comments == null) return true;
                    if (ShortLabel != SADDef.ShortFunctionPrefix + SADDef.NamingShortBankSeparator + Address && !ShortLabel.StartsWith(SADDef.ShortFunctionPrefix)) return true;
                    if (Label != SADDef.LongFunctionPrefix + Address && !Label.StartsWith(SADDef.LongFunctionPrefix)) return true;
                    if (Comments != ShortLabel + " - " + Label) return true;
                }
                else
                {
                    if (Comments != null && Comments != string.Empty) return true;
                    if (ShortLabel != SADDef.ShortExtFunctionPrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator) && !ShortLabel.StartsWith(SADDef.ShortExtFunctionPrefix + BankNum.ToString() + SADDef.NamingShortBankSeparator)) return true;
                    if (Label != SADDef.LongExtFunctionPrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator) && !Label.StartsWith(SADDef.LongExtFunctionPrefix + BankNum.ToString() + SADDef.NamingShortBankSeparator)) return true;
                }

                return false;
            }
        }
    }

    [Serializable]
    [XmlRoot("BitFlag")]
    public class S6xBitFlag
    {
        [XmlAttribute]
        public int Position { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }
        
        [XmlAttribute]
        public string SetValue { get; set; }
        [XmlAttribute]
        public string NotSetValue { get; set; }

        public string Label { get; set; }

        public string Comments { get; set; }

        [XmlIgnore]
        public string UniqueKey { get { return string.Format("{0:d2}", Position); } }
    }

    [Serializable]
    [XmlRoot("Scalar")]
    public class S6xScalar
    {
        [XmlAttribute]
        public int BankNum { get; set; }
        [XmlAttribute]
        public int AddressInt { get; set; }
        [XmlAttribute]
        public int DuplicateNum { get; set; }
        [XmlAttribute]
        public int AddressBinInt { get; set; }
        [XmlAttribute]
        public string XdfUniqueId { get; set; }

        [XmlAttribute]
        public bool isCalibrationElement { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }

        [XmlAttribute]
        public bool Byte { get; set; }
        [XmlAttribute]
        public bool Signed { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        public string ScaleExpression { get; set; }

        public string Units { get; set; }

        [XmlIgnore]
        public bool Store { get; set; }

        [XmlArray(ElementName = "BitFlags")]
        [XmlArrayItem(ElementName = "BitFlag")]
        public S6xBitFlag[] BitFlags { get; set; }
        
        [XmlIgnore]
        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }

        [XmlIgnore]
        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        [XmlIgnore]
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }
        [XmlIgnore]
        public string DuplicateAddress { get { return string.Format("{0,1} {1,5} {2,2}", BankNum, AddressInt, DuplicateNum); } }

        [XmlIgnore]
        public bool isBitFlags { get { return (BitFlags != null); } }

        [XmlIgnore]
        public int BitFlagsNum { get { if (BitFlags == null) return 0; else return BitFlags.Length; } }

        public S6xScalar()
        {
        }

        public S6xScalar(CalibrationElement scalarCalElem)
        {
            BankNum = scalarCalElem.BankNum;
            AddressInt = scalarCalElem.AddressInt;
            AddressBinInt = scalarCalElem.AddressBinInt;
            isCalibrationElement = true;
            Skip = false;
            Byte = scalarCalElem.ScalarElem.Byte;
            Signed = scalarCalElem.ScalarElem.Signed;
            Label = scalarCalElem.ScalarElem.Label;
            if (Label == string.Empty) Label = scalarCalElem.ScalarElem.RBaseCalc;
            ShortLabel = scalarCalElem.ScalarElem.ShortLabel;
            if (ShortLabel == string.Empty) ShortLabel = scalarCalElem.ScalarElem.Address;
            Comments = scalarCalElem.ScalarElem.Address;
            ScaleExpression = scalarCalElem.ScalarElem.ScaleExpression;  // From Forced Values at lower level
            Units = string.Empty;

            if (scalarCalElem.ScalarElem.isBitFlags)
            {
                int topBitFlag = 15;
                if (Byte) topBitFlag = 7;

                for (int iBit = 0; iBit <= topBitFlag; iBit++)
                {
                    S6xBitFlag bitFlag = new S6xBitFlag();
                    bitFlag.Position = iBit;
                    bitFlag.Label = "B" + iBit.ToString();
                    bitFlag.ShortLabel = bitFlag.Label;
                    bitFlag.SetValue = "1";
                    bitFlag.NotSetValue = "0";
                    AddBitFlag(bitFlag);
                    bitFlag = null;
                }
            }

            // Forced Values (Initially from S6x Inputs)
            foreach (RoutineCallInfoScalar ciCI in scalarCalElem.ScalarElem.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputScalar != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputScalar.ForcedUnits != null && ciCI.RoutineInputOutput.S6xInputScalar.ForcedUnits != string.Empty)
                        {
                            Units = ciCI.RoutineInputOutput.S6xInputScalar.ForcedUnits;
                        }
                        if (ciCI.RoutineInputOutput.S6xInputScalar.BitFlags != null)
                        {
                            BitFlags = (S6xBitFlag[])ciCI.RoutineInputOutput.S6xInputScalar.BitFlags.Clone();
                        }
                    }
                }
                if (Units != string.Empty) break;
            }

            // Forced Values (Initially from S6x Internals)
            if (scalarCalElem.ScalarElem.S6xElementSignatureSource != null)
            {
                if (scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar != null)
                {
                    if (scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Comments != null && scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Comments != string.Empty)
                    {
                        Comments = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Comments;
                    }
                    if (scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Units != null && scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Units != string.Empty)
                    {
                        Units = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Units;
                    }
                    if (scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.BitFlags != null)
                    {
                        BitFlags = (S6xBitFlag[])scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.BitFlags.Clone();
                    }
                }
            }
        }

        public S6xScalar(Scalar scalar, int bankBinAddress)
        {
            BankNum = scalar.BankNum;
            AddressInt = scalar.AddressInt;
            AddressBinInt = scalar.AddressInt + bankBinAddress;
            isCalibrationElement = false;
            Skip = false;
            Byte = scalar.Byte;
            Signed = scalar.Signed;
            Label = scalar.Label;
            ShortLabel = scalar.ShortLabel;
            if (Label == string.Empty) Label = ShortLabel;
            Comments = string.Empty;
            ScaleExpression = scalar.ScaleExpression;  // From Forced Values at lower level
            Units = string.Empty;

            if (scalar.isBitFlags)
            {
                int topBitFlag = 15;
                if (Byte) topBitFlag = 7;

                for (int iBit = 0; iBit <= topBitFlag; iBit++)
                {
                    S6xBitFlag bitFlag = new S6xBitFlag();
                    bitFlag.Position = iBit;
                    bitFlag.Label = "B" + iBit.ToString();
                    bitFlag.ShortLabel = bitFlag.Label;
                    bitFlag.SetValue = "1";
                    bitFlag.NotSetValue = "0";
                    AddBitFlag(bitFlag);
                    bitFlag = null;
                }
            }

            // Forced Values (Initially from S6x Inputs)
            foreach (RoutineCallInfoScalar ciCI in scalar.RoutinesCallsInfos)
            {
                if (ciCI.RoutineInputOutput != null)
                {
                    if (ciCI.RoutineInputOutput.S6xInputScalar != null)
                    {
                        if (ciCI.RoutineInputOutput.S6xInputScalar.ForcedUnits != null && ciCI.RoutineInputOutput.S6xInputScalar.ForcedUnits != string.Empty)
                        {
                            Units = ciCI.RoutineInputOutput.S6xInputScalar.ForcedUnits;
                        }
                        if (ciCI.RoutineInputOutput.S6xInputScalar.BitFlags != null)
                        {
                            BitFlags = (S6xBitFlag[])ciCI.RoutineInputOutput.S6xInputScalar.BitFlags.Clone();
                        }
                    }
                }
                if (Units != string.Empty) break;
            }

            // Forced Values (Initially from S6x Internals)
            if (scalar.S6xElementSignatureSource != null)
            {
                if (scalar.S6xElementSignatureSource.Scalar != null)
                {
                    if (scalar.S6xElementSignatureSource.Scalar.Comments != null && scalar.S6xElementSignatureSource.Scalar.Comments != string.Empty)
                    {
                        Comments = scalar.S6xElementSignatureSource.Scalar.Comments;
                    }
                    if (scalar.S6xElementSignatureSource.Scalar.Units != null && scalar.S6xElementSignatureSource.Scalar.Units != string.Empty)
                    {
                        Units = scalar.S6xElementSignatureSource.Scalar.Units;
                    }
                    if (scalar.S6xElementSignatureSource.Scalar.BitFlags != null)
                    {
                        BitFlags = (S6xBitFlag[])scalar.S6xElementSignatureSource.Scalar.BitFlags.Clone();
                    }
                }
            }
        }

        public S6xScalar(XdfScalar xdfScalar, int bankNum, int address, int addressBin, bool isCalElem)
        {
            XdfUniqueId = xdfScalar.uniqueid;
            BankNum = bankNum;
            AddressInt = address;
            AddressBinInt = addressBin;
            isCalibrationElement = isCalElem;
            Store = true;
            Skip = false;
            Byte = (xdfScalar.xdfData.mmedelementsizebits == "8");
            switch (xdfScalar.xdfData.mmedtypeflags)
            {
                case "0x01":
                case "0x03":
                case "0x05":
                case "0x07":
                    Signed = true;
                    break;
                default:
                    Signed = false;
                    break;
            }
            Label = xdfScalar.titleXmlValid;
            Comments = xdfScalar.descriptionXmlValid;
            ShortLabel = Tools.XDFLabelComToShortLabel(Label, Comments, "TpSc" + UniqueAddressHex.Replace(" ", "_"));
            Label = Tools.XDFLabelReview(Label, ShortLabel);
            ScaleExpression = xdfScalar.xdfMath.equation;
            Units = xdfScalar.units;
        }

        public S6xScalar(XdfFlag xdfFlag, int bankNum, int address, int addressBin, bool isCalElem)
        {
            XdfUniqueId = xdfFlag.uniqueid;
            BankNum = bankNum;
            AddressInt = address;
            AddressBinInt = addressBin;
            isCalibrationElement = isCalElem;
            Store = true;
            Skip = false;
            Byte = (xdfFlag.xdfData.mmedelementsizebits == "8");
            Signed = false;

            ScaleExpression = "X";
            Units = string.Empty;

            AddBitFlag(xdfFlag);

            Label = "Bit Flags " + UniqueAddressHex;
            ShortLabel = "TpBf" + UniqueAddressHex.Replace(" ", "_");
        }

        public void AddBitFlag(XdfFlag xdfFlag)
        {
            S6xBitFlag bitFlag = null;
            SortedList slBitFlags = null;
            int iPosition = 0;
            int iMask = 0;
            bool existingBitFlag = false;

            try { iMask = Convert.ToInt32(xdfFlag.mask.Replace("0x", ""), 16); }
            catch { return; }

            while (true)
            {
                if ((int)Math.Pow(2, iPosition) == iMask) break;
                iPosition++;
                if (Byte && iPosition > 7) return;
                else if (iPosition > 15) return;
            }

            slBitFlags = new SortedList();
            if (BitFlags != null) foreach (S6xBitFlag bF in BitFlags) slBitFlags.Add(bF.UniqueKey, bF);

            bitFlag = new S6xBitFlag();
            bitFlag.Position = iPosition;

            existingBitFlag = slBitFlags.ContainsKey(bitFlag.UniqueKey);
            if (existingBitFlag) bitFlag = (S6xBitFlag)slBitFlags[bitFlag.UniqueKey];
            else slBitFlags.Add(bitFlag.UniqueKey, bitFlag);

            bitFlag.Label = xdfFlag.titleXmlValid;
            bitFlag.Comments = xdfFlag.descriptionXmlValid;

            if (!existingBitFlag)
            {
                bitFlag.ShortLabel = Tools.XDFLabelComToShortLabel(Label, Comments, "B" + bitFlag.Position.ToString());
                bitFlag.Label = Tools.XDFLabelReview(Label, ShortLabel);
                bitFlag.SetValue = "1";
                bitFlag.NotSetValue = "0";

            }

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

            bitFlag = null;
            slBitFlags = null;
        }

        public void AddBitFlag(S6xBitFlag s6xBitFlag)
        {
            S6xBitFlag bitFlag = null;
            SortedList slBitFlags = null;
            bool existingBitFlag = false;

            slBitFlags = new SortedList();
            if (BitFlags != null) foreach (S6xBitFlag bF in BitFlags) slBitFlags.Add(bF.UniqueKey, bF);

            bitFlag = new S6xBitFlag();
            bitFlag.Position = s6xBitFlag.Position;

            existingBitFlag = slBitFlags.ContainsKey(bitFlag.UniqueKey);
            if (existingBitFlag) bitFlag = (S6xBitFlag)slBitFlags[bitFlag.UniqueKey];
            else slBitFlags.Add(bitFlag.UniqueKey, bitFlag);

            bitFlag.Label = s6xBitFlag.Label;
            bitFlag.Comments = s6xBitFlag.Comments;
            bitFlag.ShortLabel = s6xBitFlag.ShortLabel;
            bitFlag.SetValue = s6xBitFlag.SetValue;
            bitFlag.NotSetValue = s6xBitFlag.NotSetValue;

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

            bitFlag = null;
            slBitFlags = null;
        }

        public S6xScalar Clone()
        {
            S6xScalar clone = new S6xScalar();
            clone.BankNum = BankNum;
            clone.AddressInt = AddressInt;
            clone.AddressBinInt = AddressBinInt;
            clone.Skip = Skip;
            clone.Store = Store;

            clone.Byte = Byte;
            clone.Signed = Signed;
            clone.isCalibrationElement = isCalibrationElement;
            clone.ScaleExpression = ScaleExpression;
            clone.Units = Units;

            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;

            if (BitFlags != null) clone.BitFlags = (S6xBitFlag[])BitFlags.Clone();
            return clone;
        }

        public bool isUserDefined
        {
            get
            {
                if (Units != null && Units != string.Empty) return true;
                if (OutputComments) return true;
                if (ScaleExpression != null) if (ScaleExpression.ToUpper() != "X") return true;

                if (ShortLabel == null) return true;
                if (Label == null) return true;

                if (isCalibrationElement)
                {
                    if (Comments == null) return true;
                    if (ShortLabel != SADDef.ShortScalarPrefix + SADDef.NamingShortBankSeparator + Address && !ShortLabel.StartsWith(SADDef.ShortScalarPrefix)) return true;
                    if (!Label.StartsWith(SADDef.ShortRegisterPrefix) || !Label.Contains(SADDef.AdditionSeparator)) return true;
                    if (Comments != Address) return true;
                }
                else
                {
                    if (Comments != null && Comments != string.Empty) return true;
                    if (ShortLabel != SADDef.ShortExtScalarPrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator) && !ShortLabel.StartsWith(SADDef.ShortExtScalarPrefix + BankNum.ToString() + SADDef.NamingShortBankSeparator)) return true;
                    if (Label != ShortLabel) return true;
                }

                return false;
            }
        }
    }

    [Serializable]
    [XmlRoot("Structure")]
    public class S6xStructure
    {
        [XmlAttribute]
        public int BankNum { get; set; }
        [XmlAttribute]
        public int AddressInt { get; set; }
        [XmlAttribute]
        public int DuplicateNum { get; set; }
        [XmlAttribute]
        public int AddressBinInt { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }

        public string StructDef { get; set; }
        [XmlAttribute]
        public int Number { get; set; }

        [XmlAttribute]
        public bool isCalibrationElement { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        [XmlIgnore]
        public Structure Structure { get; set; }
        
        [XmlIgnore]
        public bool Store { get; set; }

        [XmlIgnore]
        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }

        [XmlIgnore]
        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        [XmlIgnore]
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }
        [XmlIgnore]
        public string DuplicateAddress { get { return string.Format("{0,1} {1,5} {2,2}", BankNum, AddressInt, DuplicateNum); } }

        [XmlIgnore]
        public bool isVectorsList
        {
            get
            {
                if (Structure == null) return new Structure(this).isVectorsList;
                else return Structure.isVectorsList;
            }
        }

        [XmlIgnore]
        public int VectorsBankNum
        {
            get
            {
                if (Structure == null) return new Structure(this).VectorsBankNum;
                else return Structure.VectorsBankNum;
            }
        }

        public S6xStructure()
        {
        }

        public S6xStructure(Structure structure)
        {
            BankNum = structure.BankNum;
            AddressInt = structure.AddressInt;
            AddressBinInt = structure.AddressBinInt;
            Structure = structure;
            StructDef = Structure.StructDefString;
            Number = structure.Number;
            Skip = false;
            Label = structure.Label;
            ShortLabel = structure.ShortLabel;
            Comments = (structure.Defaulted) ? "Structure definition was defaulted" : string.Empty;

            // Forced Values (Initially from S6x Internals)
            if (structure.S6xElementSignatureSource != null)
            {
                if (structure.S6xElementSignatureSource.Structure != null)
                {
                    if (structure.S6xElementSignatureSource.Structure.Comments != null && structure.S6xElementSignatureSource.Structure.Comments != string.Empty)
                    {
                        Comments = structure.S6xElementSignatureSource.Structure.Comments;
                    }
                }
            }
        }

        public S6xStructure(CalibrationElement structureCalElem)
        {

            BankNum = structureCalElem.BankNum;
            AddressInt = structureCalElem.AddressInt;
            AddressBinInt = structureCalElem.AddressBinInt;
            Structure = structureCalElem.StructureElem;
            Number = structureCalElem.StructureElem.Number;
            StructDef = structureCalElem.StructureElem.StructDefString;
            isCalibrationElement = true;
            Skip = false;
            Label = structureCalElem.StructureElem.Label;
            if (Label == string.Empty) Label = structureCalElem.StructureElem.RBaseCalc;
            ShortLabel = structureCalElem.StructureElem.ShortLabel;
            if (ShortLabel == string.Empty) ShortLabel = structureCalElem.StructureElem.Address;
            Comments = (structureCalElem.StructureElem.Defaulted) ? "Structure definition was defaulted" : string.Empty;

            // Forced Values (Initially from S6x Internals)
            if (structureCalElem.StructureElem.S6xElementSignatureSource != null)
            {
                if (structureCalElem.StructureElem.S6xElementSignatureSource.Structure != null)
                {
                    if (structureCalElem.StructureElem.S6xElementSignatureSource.Structure.Comments != null && structureCalElem.StructureElem.S6xElementSignatureSource.Structure.Comments != string.Empty)
                    {
                        Comments = structureCalElem.StructureElem.S6xElementSignatureSource.Structure.Comments;
                    }
                }
            }
        }

        public S6xStructure(S6xStructureClipBoard clipBoardClone)
        {
            BankNum = clipBoardClone.BankNum;
            AddressInt = clipBoardClone.AddressInt;
            AddressBinInt = clipBoardClone.AddressBinInt;
            Skip = clipBoardClone.Skip;
            Store = clipBoardClone.Store;

            StructDef = clipBoardClone.StructDef;
            Number = clipBoardClone.Number;
            isCalibrationElement = clipBoardClone.isCalibrationElement;

            ShortLabel = clipBoardClone.ShortLabel;
            OutputComments = clipBoardClone.OutputComments;
            Label = clipBoardClone.Label;
            Comments = clipBoardClone.Comments;
        }

        public S6xStructureClipBoard ClipBoardClone()
        {
            S6xStructureClipBoard clone = new S6xStructureClipBoard();
            clone.BankNum = BankNum;
            clone.AddressInt = AddressInt;
            clone.AddressBinInt = AddressBinInt;
            clone.Skip = Skip;
            clone.Store = Store;

            clone.StructDef = StructDef;
            clone.Number = Number;
            clone.isCalibrationElement = isCalibrationElement;

            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;

            return clone;
        }

        public S6xStructure Clone()
        {
            S6xStructure clone = new S6xStructure();
            clone.BankNum = BankNum;
            clone.AddressInt = AddressInt;
            clone.AddressBinInt = AddressBinInt;
            clone.Skip = Skip;
            clone.Store = Store;

            clone.StructDef = StructDef;
            clone.Number = Number;
            clone.isCalibrationElement = isCalibrationElement;

            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;

            return clone;
        }

        public bool isUserDefined
        {
            get
            {
                if (OutputComments) return true;

                if (ShortLabel == null) return true;
                if (Label == null) return true;

                if (isCalibrationElement)
                {
                    if (Comments == null) return true;
                    if (ShortLabel != SADDef.ShortStructurePrefix + SADDef.NamingShortBankSeparator + Address && !ShortLabel.StartsWith(SADDef.ShortStructurePrefix)) return true;
                    if (Label != SADDef.LongStructurePrefix + Address && !Label.StartsWith(SADDef.LongStructurePrefix)) return true;
                }
                else
                {
                    if (Comments != null && Comments != string.Empty) return true;
                    if (ShortLabel != SADDef.ShortExtStructurePrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator) && !ShortLabel.StartsWith(SADDef.ShortExtStructurePrefix + BankNum.ToString() + SADDef.NamingShortBankSeparator)) return true;
                    if (Label != SADDef.LongExtStructurePrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator) && !Label.StartsWith(SADDef.LongExtStructurePrefix + BankNum.ToString() + SADDef.NamingShortBankSeparator)) return true;
                }

                return false;
            }
        }
    }

    [Serializable]
    [XmlRoot("StructureClipBoard")]
    public class S6xStructureClipBoard
    {
        [XmlAttribute]
        public int BankNum { get; set; }
        [XmlAttribute]
        public int AddressInt { get; set; }
        [XmlAttribute]
        public int AddressBinInt { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }

        public string StructDef { get; set; }
        [XmlAttribute]
        public int Number { get; set; }

        [XmlAttribute]
        public bool isCalibrationElement { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        [XmlIgnore]
        public bool Store { get; set; }

        [XmlIgnore]
        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }

        [XmlIgnore]
        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        [XmlIgnore]
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public S6xStructureClipBoard()
        {
        }

    }

    [Serializable]
    [XmlRoot("Routine")]
    public class S6xRoutine
    {
        [XmlAttribute]
        public int BankNum { get; set; }
        [XmlAttribute]
        public int AddressInt { get; set; }
        [XmlAttribute]
        public int ByteArgumentsNum { get; set; }
        [XmlAttribute]
        public bool ByteArgumentsNumOverride { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }
        [XmlIgnore]
        public bool Store { get; set; }

        [XmlArray(ElementName = "InputArguments")]
        [XmlArrayItem(ElementName = "InputArgument")]
        public S6xRoutineInputArgument[] InputArguments { get; set; }

        [XmlArray(ElementName = "InputStructures")]
        [XmlArrayItem(ElementName = "InputStructure")]
        public S6xRoutineInputStructure[] InputStructures { get; set; }

        [XmlArray(ElementName = "InputTables")]
        [XmlArrayItem(ElementName = "InputTable")]
        public S6xRoutineInputTable[] InputTables { get; set; }

        [XmlArray(ElementName = "InputFunctions")]
        [XmlArrayItem(ElementName = "InputFunction")]
        public S6xRoutineInputFunction[] InputFunctions { get; set; }

        [XmlArray(ElementName = "InputScalars")]
        [XmlArrayItem(ElementName = "InputScalar")]
        public S6xRoutineInputScalar[] InputScalars { get; set; }

        [XmlIgnore]
        public bool isAdvanced
        {
            get
            {
                if (InputArguments != null) return true;
                if (InputStructures != null) return true;
                if (InputTables != null) return true;
                if (InputFunctions != null) return true;
                if (InputScalars != null) return true;

                return false;
            }
        }

        [XmlIgnore]
        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }

        [XmlIgnore]
        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        [XmlIgnore]
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        [XmlIgnore]
        public int InputArgumentsNum { get { if (InputArguments == null) return 0; else return InputArguments.Length; } }
        [XmlIgnore]
        public int InputStructuresNum { get { if (InputStructures == null) return 0; else return InputStructures.Length; } }
        [XmlIgnore]
        public int InputTablesNum { get { if (InputTables == null) return 0; else return InputTables.Length; } }
        [XmlIgnore]
        public int InputFunctionsNum { get { if (InputFunctions == null) return 0; else return InputFunctions.Length; } }
        [XmlIgnore]
        public int InputScalarsNum { get { if (InputScalars == null) return 0; else return InputScalars.Length; } }

        public S6xRoutine()
        {
        }

        public S6xRoutine(Call cCall, Routine rRoutine)
        {
            BankNum = cCall.BankNum;
            AddressInt = cCall.AddressInt;
            ByteArgumentsNum = cCall.ArgsNum;
            if (cCall.ArgsCondValidated) ByteArgumentsNum += cCall.ArgsNumCondAdder;
            if (ByteArgumentsNum < 0) ByteArgumentsNum = 0;
            if (cCall.ArgsStackDepth != 1) ByteArgumentsNum = 0;
            ByteArgumentsNumOverride = false;
            Label = cCall.Label;
            ShortLabel = cCall.ShortLabel;
            if (Label == string.Empty) Label = cCall.UniqueAddressHex;
            if (rRoutine == null) Comments = cCall.UniqueAddressHex;
            else Comments = rRoutine.Comments;
            if (cCall.Callers.Count > 4) Comments += "\r\nCalled " + cCall.Callers.Count.ToString() + " times.";

            if (cCall.Arguments != null && ByteArgumentsNum > 0)
            {
                InputArguments = new S6xRoutineInputArgument[cCall.Arguments.Length];
                for (int iArg = 0; iArg < InputArguments.Length; iArg++)
                {
                    InputArguments[iArg] = new S6xRoutineInputArgument();
                    InputArguments[iArg].UniqueKey = string.Format("Ra{0:d3}", iArg + 1);
                    InputArguments[iArg].Position = iArg + 1;
                    InputArguments[iArg].Encryption = (int)cCall.Arguments[iArg].Mode;
                    InputArguments[iArg].Word = cCall.Arguments[iArg].Word;
                    InputArguments[iArg].Pointer = InputArguments[iArg].Word;
                }
            }

            if (rRoutine != null)
            {
                if (rRoutine.IOs != null)
                {
                    ArrayList alIOTables = new ArrayList();
                    ArrayList alIOFunctions = new ArrayList();
                    ArrayList alIOStructures = new ArrayList();
                    ArrayList alIOScalars = new ArrayList();
                    int iKeyCount = 0;
                    if (InputArguments != null) iKeyCount = InputArguments.Length;

                    foreach (RoutineIO ioIO in rRoutine.IOs)
                    {
                        if (ioIO.GetType() == typeof(RoutineIOTable)) alIOTables.Add(ioIO);
                        else if (ioIO.GetType() == typeof(RoutineIOFunction)) alIOFunctions.Add(ioIO);
                        else if (ioIO.GetType() == typeof(RoutineIOStructure)) alIOStructures.Add(ioIO);
                        else if (ioIO.GetType() == typeof(RoutineIOScalar)) alIOScalars.Add(ioIO);
                    }

                    if (alIOTables.Count > 0)
                    {
                        InputTables = new S6xRoutineInputTable[alIOTables.Count];
                        for (int iIO = 0; iIO < InputTables.Length; iIO++)
                        {
                            iKeyCount++;
                            InputTables[iIO] = new S6xRoutineInputTable();
                            InputTables[iIO].UniqueKey = string.Format("Ra{0:d3}", iKeyCount);
                            InputTables[iIO].WordOutput = ((RoutineIOTable)alIOTables[iIO]).TableWord;
                            InputTables[iIO].SignedOutput = ((RoutineIOTable)alIOTables[iIO]).SignedOutput;
                            InputTables[iIO].VariableAddress = Tools.PointerTranslation(((RoutineIOTable)alIOTables[iIO]).AddressRegister);
                            InputTables[iIO].VariableColsNumberReg = Tools.PointerTranslation(((RoutineIOTable)alIOTables[iIO]).TableColNumberRegister);
                            InputTables[iIO].VariableColsReg = Tools.PointerTranslation(((RoutineIOTable)alIOTables[iIO]).TableColRegister);
                            InputTables[iIO].VariableRowsReg = Tools.PointerTranslation(((RoutineIOTable)alIOTables[iIO]).TableRowRegister);
                            InputTables[iIO].VariableOutput = Tools.PointerTranslation(((RoutineIOTable)alIOTables[iIO]).OutputRegister);

                            // Args Mngt
                            if (InputArguments != null && cCall.Arguments != null)
                            {
                                if (InputArguments.Length == cCall.Arguments.Length)
                                {
                                    for (int iArg = 0; iArg < cCall.Arguments.Length; iArg++)
                                    {
                                        if (cCall.Arguments[iArg].OutputRegisterAddress == ((RoutineIOTable)alIOTables[iIO]).AddressRegister)
                                        {
                                            InputTables[iIO].VariableAddress = InputArguments[iArg].Code + SADDef.VariableValuesSeparator + InputTables[iIO].VariableAddress;
                                        }
                                        else if (cCall.Arguments[iArg].OutputRegisterAddress == ((RoutineIOTable)alIOTables[iIO]).TableColNumberRegister)
                                        {
                                            InputTables[iIO].VariableColsNumberReg = InputArguments[iArg].Code + SADDef.VariableValuesSeparator + InputTables[iIO].VariableColsNumberReg;
                                        }
                                        else if (cCall.Arguments[iArg].OutputRegisterAddress == ((RoutineIOTable)alIOTables[iIO]).TableColRegister)
                                        {
                                            InputTables[iIO].VariableColsReg = InputArguments[iArg].Code + SADDef.VariableValuesSeparator + InputTables[iIO].VariableColsReg;
                                        }
                                        else if (cCall.Arguments[iArg].OutputRegisterAddress == ((RoutineIOTable)alIOTables[iIO]).TableRowRegister)
                                        {
                                            InputTables[iIO].VariableRowsReg = InputArguments[iArg].Code + SADDef.VariableValuesSeparator + InputTables[iIO].VariableRowsReg;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    alIOTables = null;

                    if (alIOFunctions.Count > 0)
                    {
                        InputFunctions = new S6xRoutineInputFunction[alIOFunctions.Count];
                        for (int iIO = 0; iIO < InputFunctions.Length; iIO++)
                        {
                            iKeyCount++;
                            InputFunctions[iIO] = new S6xRoutineInputFunction();
                            InputFunctions[iIO].UniqueKey = string.Format("Ra{0:d3}", iKeyCount);
                            InputFunctions[iIO].ByteInput = ((RoutineIOFunction)alIOFunctions[iIO]).FunctionByte;
                            InputFunctions[iIO].SignedInput = ((RoutineIOFunction)alIOFunctions[iIO]).FunctionSignedInput;
                            InputFunctions[iIO].SignedOutput = ((RoutineIOFunction)alIOFunctions[iIO]).SignedOutput;
                            InputFunctions[iIO].VariableAddress = Tools.PointerTranslation(((RoutineIOFunction)alIOFunctions[iIO]).AddressRegister);
                            InputFunctions[iIO].VariableInput = Tools.PointerTranslation(((RoutineIOFunction)alIOFunctions[iIO]).FunctionInputRegister);
                            InputFunctions[iIO].VariableOutput = Tools.PointerTranslation(((RoutineIOFunction)alIOFunctions[iIO]).OutputRegister);

                            // Args Mngt
                            if (InputArguments != null && cCall.Arguments != null)
                            {
                                if (InputArguments.Length == cCall.Arguments.Length)
                                {
                                    for (int iArg = 0; iArg < cCall.Arguments.Length; iArg++)
                                    {
                                        if (cCall.Arguments[iArg].OutputRegisterAddress == ((RoutineIOFunction)alIOFunctions[iIO]).AddressRegister)
                                        {
                                            InputFunctions[iIO].VariableAddress = InputArguments[iArg].Code + SADDef.VariableValuesSeparator + InputFunctions[iIO].VariableAddress;
                                        }
                                        else if (cCall.Arguments[iArg].OutputRegisterAddress == ((RoutineIOFunction)alIOFunctions[iIO]).FunctionInputRegister)
                                        {
                                            InputFunctions[iIO].VariableInput = InputArguments[iArg].Code + SADDef.VariableValuesSeparator + InputFunctions[iIO].VariableInput;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    alIOFunctions = null;

                    if (alIOStructures.Count > 0)
                    {
                        InputStructures = new S6xRoutineInputStructure[alIOStructures.Count];
                        for (int iIO = 0; iIO < InputStructures.Length; iIO++)
                        {
                            iKeyCount++;
                            InputStructures[iIO] = new S6xRoutineInputStructure();
                            InputStructures[iIO].UniqueKey = string.Format("Ra{0:d3}", iKeyCount);
                            InputStructures[iIO].VariableAddress = Tools.PointerTranslation(((RoutineIOStructure)alIOStructures[iIO]).AddressRegister);
                            InputStructures[iIO].VariableNumber = Tools.PointerTranslation(((RoutineIOStructure)alIOStructures[iIO]).StructureNumberRegister);

                            // Args Mngt
                            if (InputArguments != null && cCall.Arguments != null)
                            {
                                if (InputArguments.Length == cCall.Arguments.Length)
                                {
                                    for (int iArg = 0; iArg < cCall.Arguments.Length; iArg++)
                                    {
                                        if (cCall.Arguments[iArg].OutputRegisterAddress == ((RoutineIOStructure)alIOStructures[iIO]).AddressRegister)
                                        {
                                            InputStructures[iIO].VariableAddress = InputArguments[iArg].Code + SADDef.VariableValuesSeparator + InputStructures[iIO].VariableAddress;
                                        }
                                        else if (cCall.Arguments[iArg].OutputRegisterAddress == ((RoutineIOStructure)alIOStructures[iIO]).StructureNumberRegister)
                                        {
                                            InputStructures[iIO].VariableNumber = InputArguments[iArg].Code + SADDef.VariableValuesSeparator + InputStructures[iIO].VariableNumber;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    alIOStructures = null;

                    if (alIOScalars.Count > 0)
                    {
                        InputScalars = new S6xRoutineInputScalar[alIOScalars.Count];

                        for (int iIO = 0; iIO < InputScalars.Length; iIO++)
                        {
                            iKeyCount++;
                            InputScalars[iIO] = new S6xRoutineInputScalar();
                            InputScalars[iIO].UniqueKey = string.Format("Ra{0:d3}", iKeyCount);
                            InputScalars[iIO].Byte = ((RoutineIOScalar)alIOScalars[iIO]).ScalarByte;
                            InputScalars[iIO].Signed = ((RoutineIOScalar)alIOScalars[iIO]).ScalarSigned;
                            InputScalars[iIO].VariableAddress = Tools.PointerTranslation(((RoutineIOScalar)alIOScalars[iIO]).AddressRegister);

                            // Args Mngt
                            if (InputArguments != null && cCall.Arguments != null)
                            {
                                if (InputArguments.Length == cCall.Arguments.Length)
                                {
                                    for (int iArg = 0; iArg < cCall.Arguments.Length; iArg++)
                                    {
                                        if (cCall.Arguments[iArg].OutputRegisterAddress == ((RoutineIOScalar)alIOScalars[iIO]).AddressRegister)
                                        {
                                            InputScalars[iIO].VariableAddress = InputArguments[iArg].Code + SADDef.VariableValuesSeparator + InputScalars[iIO].VariableAddress;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    alIOScalars = null;
                }
            }
        }

        public S6xRoutine Clone()
        {
            S6xRoutine clone = new S6xRoutine();
            clone.BankNum = BankNum;
            clone.AddressInt = AddressInt;
            clone.ByteArgumentsNum = ByteArgumentsNum;
            clone.ByteArgumentsNumOverride = ByteArgumentsNumOverride;
            clone.Skip = Skip;
            clone.Store = Store;
            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;

            if (InputArguments != null)
            {
                clone.InputArguments = new S6xRoutineInputArgument[InputArguments.Length];
                for (int i = 0; i < InputArguments.Length; i++) clone.InputArguments[i] = InputArguments[i].Clone();
            }
            if (InputStructures != null)
            {
                clone.InputStructures = new S6xRoutineInputStructure[InputStructures.Length];
                for (int i = 0; i < InputStructures.Length; i++) clone.InputStructures[i] = InputStructures[i].Clone();
            }
            if (InputTables != null)
            {
                clone.InputTables = new S6xRoutineInputTable[InputTables.Length];
                for (int i = 0; i < InputTables.Length; i++) clone.InputTables[i] = InputTables[i].Clone();
            }
            if (InputFunctions != null)
            {
                clone.InputFunctions = new S6xRoutineInputFunction[InputFunctions.Length];
                for (int i = 0; i < InputFunctions.Length; i++) clone.InputFunctions[i] = InputFunctions[i].Clone();
            }
            if (InputScalars != null)
            {
                clone.InputScalars = new S6xRoutineInputScalar[InputScalars.Length];
                for (int i = 0; i < InputScalars.Length; i++) clone.InputScalars[i] = InputScalars[i].Clone();
            }
            return clone;
        }

        public int VariableRegisterAddressByEquivalent(string argCode)
        {
            int iRes = -1;

            if (InputStructures != null)
            {
                foreach (S6xRoutineInputStructure inpElem in InputStructures)
                {
                    iRes = inpElem.VariableRegisterAddressByEquivalent(argCode);
                    if (iRes != -1) return iRes;
                }
            }

            if (InputTables != null)
            {
                foreach (S6xRoutineInputTable inpElem in InputTables)
                {
                    iRes = inpElem.VariableRegisterAddressByEquivalent(argCode);
                    if (iRes != -1) return iRes;
                }
            }

            if (InputFunctions != null)
            {
                foreach (S6xRoutineInputFunction inpElem in InputFunctions)
                {
                    iRes = inpElem.VariableRegisterAddressByEquivalent(argCode);
                    if (iRes != -1) return iRes;
                }
            }

            if (InputScalars != null)
            {
                foreach (S6xRoutineInputScalar inpElem in InputScalars)
                {
                    iRes = inpElem.VariableRegisterAddressByEquivalent(argCode);
                    if (iRes != -1) return iRes;
                }
            }

            return iRes;
        }
    }

    [Serializable]
    [XmlRoot("InternalStructure")]
    public class S6xRoutineInternalStructure
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public string VariableBankNum { get; set; }
        [XmlAttribute]
        public string VariableAddress { get; set; }

        public string StructDef { get; set; }
        [XmlAttribute]
        public int Number { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        public S6xRoutineInternalStructure Clone()
        {
            S6xRoutineInternalStructure clone = new S6xRoutineInternalStructure();
            clone.UniqueKey = UniqueKey;

            clone.VariableBankNum = VariableBankNum;
            clone.VariableAddress = VariableAddress;

            clone.StructDef = StructDef;
            clone.Number = Number;
            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;

            return clone;
        }
    }

    [Serializable]
    [XmlRoot("InternalTable")]
    public class S6xRoutineInternalTable
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public string VariableBankNum { get; set; }
        [XmlAttribute]
        public string VariableAddress { get; set; }

        [XmlAttribute]
        public bool WordOutput { get; set; }
        [XmlAttribute]
        public bool SignedOutput { get; set; }

        [XmlAttribute]
        public string VariableColsNumber { get; set; }
        [XmlAttribute]
        public int RowsNumber { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        public string CellsScaleExpression { get; set; }

        public string ColsUnits { get; set; }
        public string RowsUnits { get; set; }
        public string CellsUnits { get; set; }

        public S6xRoutineInternalTable Clone()
        {
            S6xRoutineInternalTable clone = new S6xRoutineInternalTable();
            clone.UniqueKey = UniqueKey;

            clone.VariableBankNum = VariableBankNum;
            clone.VariableAddress = VariableAddress;
            clone.VariableColsNumber = VariableColsNumber;

            clone.WordOutput = WordOutput;
            clone.SignedOutput = SignedOutput;
            clone.RowsNumber = RowsNumber;
            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;
            clone.CellsScaleExpression = CellsScaleExpression;
            clone.ColsUnits = ColsUnits;
            clone.RowsUnits = RowsUnits;
            clone.CellsUnits = CellsUnits;

            return clone;
        }
    }

    [Serializable]
    [XmlRoot("InternalFunction")]
    public class S6xRoutineInternalFunction
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public string VariableBankNum { get; set; }
        [XmlAttribute]
        public string VariableAddress { get; set; }

        [XmlAttribute]
        public bool ByteInput { get; set; }
        [XmlAttribute]
        public bool ByteOutput { get; set; }

        [XmlAttribute]
        public bool SignedInput { get; set; }
        [XmlAttribute]
        public bool SignedOutput { get; set; }

        [XmlAttribute]
        public int RowsNumber { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        public string InputScaleExpression { get; set; }
        public string OutputScaleExpression { get; set; }

        public string InputUnits { get; set; }
        public string OutputUnits { get; set; }

        public S6xRoutineInternalFunction Clone()
        {
            S6xRoutineInternalFunction clone = new S6xRoutineInternalFunction();
            clone.UniqueKey = UniqueKey;

            clone.VariableBankNum = VariableBankNum;
            clone.VariableAddress = VariableAddress;

            clone.ByteInput = ByteInput;
            clone.ByteOutput = ByteOutput;
            clone.SignedInput = SignedInput;
            clone.SignedOutput = SignedOutput;
            clone.RowsNumber = RowsNumber;
            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;
            clone.InputScaleExpression = InputScaleExpression;
            clone.OutputScaleExpression = OutputScaleExpression;
            clone.InputUnits = InputUnits;
            clone.OutputUnits = OutputUnits;

            return clone;
        }
    }

    [Serializable]
    [XmlRoot("InternalScalar")]
    public class S6xRoutineInternalScalar
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public string VariableBankNum { get; set; }
        [XmlAttribute]
        public string VariableAddress { get; set; }

        [XmlAttribute]
        public bool Byte { get; set; }
        [XmlAttribute]
        public bool Signed { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        public string ScaleExpression { get; set; }

        public string Units { get; set; }

        [XmlArray(ElementName = "BitFlags")]
        [XmlArrayItem(ElementName = "BitFlag")]
        public S6xBitFlag[] BitFlags { get; set; }

        [XmlIgnore]
        public bool isBitFlags { get { return (BitFlags != null); } }

        [XmlIgnore]
        public int BitFlagsNum { get { if (BitFlags == null) return 0; else return BitFlags.Length; } }

        public S6xRoutineInternalScalar Clone()
        {
            S6xRoutineInternalScalar clone = new S6xRoutineInternalScalar();
            clone.UniqueKey = UniqueKey;

            clone.VariableBankNum = VariableBankNum;
            clone.VariableAddress = VariableAddress;

            clone.Byte = Byte;
            clone.Signed = Signed;
            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;
            clone.ScaleExpression = ScaleExpression;
            clone.Units = Units;

            if (BitFlags != null) clone.BitFlags = (S6xBitFlag[])BitFlags.Clone();
            return clone;
        }

        public void AddBitFlag(S6xBitFlag s6xBitFlag)
        {
            S6xBitFlag bitFlag = null;
            SortedList slBitFlags = null;
            bool existingBitFlag = false;

            slBitFlags = new SortedList();
            if (BitFlags != null) foreach (S6xBitFlag bF in BitFlags) slBitFlags.Add(bF.UniqueKey, bF);

            bitFlag = new S6xBitFlag();
            bitFlag.Position = s6xBitFlag.Position;

            existingBitFlag = slBitFlags.ContainsKey(bitFlag.UniqueKey);
            if (existingBitFlag) bitFlag = (S6xBitFlag)slBitFlags[bitFlag.UniqueKey];
            else slBitFlags.Add(bitFlag.UniqueKey, bitFlag);

            bitFlag.Label = s6xBitFlag.Label;
            bitFlag.Comments = s6xBitFlag.Comments;
            bitFlag.ShortLabel = s6xBitFlag.ShortLabel;
            bitFlag.SetValue = s6xBitFlag.SetValue;
            bitFlag.NotSetValue = s6xBitFlag.NotSetValue;

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

            bitFlag = null;
            slBitFlags = null;
        }
    }

    [Serializable]
    [XmlRoot("InputArgument")]
    public class S6xRoutineInputArgument
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public int Position { get; set; }
        [XmlAttribute]
        public int Encryption { get; set; }

        [XmlAttribute]
        public bool Word { get; set; }
        [XmlAttribute]
        public bool Pointer { get; set; }

        [XmlIgnore]
        public string Code { get { return Tools.ArgumentCode(Position); } }

        public S6xRoutineInputArgument Clone()
        {
            S6xRoutineInputArgument clone = new S6xRoutineInputArgument();
            clone.UniqueKey = UniqueKey;
            clone.Position = Position;
            clone.Encryption = Encryption;
            clone.Word = Word;
            clone.Pointer = Pointer;

            return clone;
        }
    }

    [Serializable]
    [XmlRoot("InputStructure")]
    public class S6xRoutineInputStructure
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public string VariableAddress { get; set; }
        [XmlAttribute]
        public string VariableNumber { get; set; }

        [XmlAttribute]
        public string ForcedNumber { get; set; }

        public string StructDef { get; set; }

        public int VariableRegisterAddressByEquivalent(string eqCode)
        {
            int iRes = -1;

            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableAddress);
            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableNumber);

            return iRes;
        }

        public S6xRoutineInputStructure Clone()
        {
            S6xRoutineInputStructure clone = new S6xRoutineInputStructure();
            clone.UniqueKey = UniqueKey;

            clone.VariableAddress = VariableAddress;
            clone.VariableNumber = VariableNumber;

            clone.ForcedNumber = ForcedNumber;

            clone.StructDef = StructDef;

            return clone;
        }
    }

    [Serializable]
    [XmlRoot("InputTable")]
    public class S6xRoutineInputTable
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public string VariableAddress { get; set; }

        [XmlAttribute]
        public string VariableColsNumberReg { get; set; }
        [XmlAttribute]
        public string VariableColsReg { get; set; }
        [XmlAttribute]
        public string VariableOutput { get; set; }
        [XmlAttribute]
        public string VariableRowsReg { get; set; }

        [XmlAttribute]
        public bool SignedOutput { get; set; }
        [XmlAttribute]
        public bool WordOutput { get; set; }

        [XmlAttribute]
        public string ForcedColsNumber { get; set; }
        [XmlAttribute]
        public string ForcedRowsNumber { get; set; }

        public string ForcedColsUnits { get; set; }
        public string ForcedRowsUnits { get; set; }
        public string ForcedCellsUnits { get; set; }

        public string ForcedCellsScaleExpression { get; set; }
        
        public int VariableRegisterAddressByEquivalent(string eqCode)
        {
            int iRes = -1;

            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableAddress);
            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableColsNumberReg);
            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableColsReg);
            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableRowsReg);
            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableOutput);

            return iRes;
        }

        public S6xRoutineInputTable Clone()
        {
            S6xRoutineInputTable clone = new S6xRoutineInputTable();
            clone.UniqueKey = UniqueKey;
            
            clone.VariableAddress = VariableAddress;
            clone.VariableColsNumberReg = VariableColsNumberReg;
            clone.VariableColsReg = VariableColsReg;
            clone.VariableOutput = VariableOutput;
            clone.VariableRowsReg = VariableRowsReg;
            
            clone.SignedOutput = SignedOutput;
            clone.WordOutput = WordOutput;
            
            clone.ForcedColsNumber = ForcedColsNumber;
            clone.ForcedRowsNumber = ForcedRowsNumber;
            
            clone.ForcedColsUnits = ForcedColsUnits;
            clone.ForcedRowsUnits = ForcedRowsUnits;
            clone.ForcedCellsUnits = ForcedCellsUnits;
            clone.ForcedCellsScaleExpression = ForcedCellsScaleExpression;

            return clone;
        }
    }

    [Serializable]
    [XmlRoot("InputFunction")]
    public class S6xRoutineInputFunction
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public string VariableAddress { get; set; }
        [XmlAttribute]
        public string VariableInput { get; set; }
        [XmlAttribute]
        public string VariableOutput { get; set; }

        [XmlAttribute]
        public bool ByteInput { get; set; }
        [XmlAttribute]
        public bool SignedInput { get; set; }
        [XmlAttribute]
        public bool SignedOutput { get; set; }

        [XmlAttribute]
        public string ForcedRowsNumber { get; set; }

        public string ForcedInputScaleExpression { get; set; }
        public string ForcedOutputScaleExpression { get; set; }

        public string ForcedInputUnits { get; set; }
        public string ForcedOutputUnits { get; set; }
        
        public int VariableRegisterAddressByEquivalent(string eqCode)
        {
            int iRes = -1;

            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableAddress);
            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableInput);
            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableOutput);

            return iRes;
        }

        public S6xRoutineInputFunction Clone()
        {
            S6xRoutineInputFunction clone = new S6xRoutineInputFunction();
            clone.UniqueKey = UniqueKey;

            clone.VariableAddress = VariableAddress;
            clone.VariableInput = VariableInput;
            clone.VariableOutput = VariableOutput;

            clone.ByteInput = ByteInput;
            clone.SignedInput = SignedInput;
            clone.SignedOutput = SignedOutput;

            clone.ForcedRowsNumber = ForcedRowsNumber;
            
            clone.ForcedInputScaleExpression = ForcedInputScaleExpression;
            clone.ForcedOutputScaleExpression = ForcedOutputScaleExpression;

            clone.ForcedInputUnits = ForcedInputUnits;
            clone.ForcedOutputUnits = ForcedOutputUnits;

            return clone;
        }
    }

    [Serializable]
    [XmlRoot("InputScalar")]
    public class S6xRoutineInputScalar
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public string VariableAddress { get; set; }

        [XmlAttribute]
        public bool Byte { get; set; }
        [XmlAttribute]
        public bool Signed { get; set; }

        public string ForcedScaleExpression { get; set; }

        public string ForcedUnits { get; set; }

        [XmlArray(ElementName = "BitFlags")]
        [XmlArrayItem(ElementName = "BitFlag")]
        public S6xBitFlag[] BitFlags { get; set; }

        [XmlIgnore]
        public bool isBitFlags { get { return (BitFlags != null); } }

        [XmlIgnore]
        public int BitFlagsNum { get { if (BitFlags == null) return 0; else return BitFlags.Length; } }

        public int VariableRegisterAddressByEquivalent(string eqCode)
        {
            int iRes = -1;

            if (iRes == -1) iRes = Tools.VariableRegisterAddressByEquivalent(eqCode, VariableAddress);

            return iRes;
        }

        public S6xRoutineInputScalar Clone()
        {
            S6xRoutineInputScalar clone = new S6xRoutineInputScalar();
            clone.UniqueKey = UniqueKey;

            clone.VariableAddress = VariableAddress;

            clone.Byte = Byte;
            clone.Signed = Signed;

            clone.ForcedScaleExpression = ForcedScaleExpression;

            clone.ForcedUnits = ForcedUnits;

            if (BitFlags != null) clone.BitFlags = (S6xBitFlag[])BitFlags.Clone();

            return clone;
        }

        public void AddBitFlag(S6xBitFlag s6xBitFlag)
        {
            S6xBitFlag bitFlag = null;
            SortedList slBitFlags = null;
            bool existingBitFlag = false;

            slBitFlags = new SortedList();
            if (BitFlags != null) foreach (S6xBitFlag bF in BitFlags) slBitFlags.Add(bF.UniqueKey, bF);

            bitFlag = new S6xBitFlag();
            bitFlag.Position = s6xBitFlag.Position;

            existingBitFlag = slBitFlags.ContainsKey(bitFlag.UniqueKey);
            if (existingBitFlag) bitFlag = (S6xBitFlag)slBitFlags[bitFlag.UniqueKey];
            else slBitFlags.Add(bitFlag.UniqueKey, bitFlag);

            bitFlag.Label = s6xBitFlag.Label;
            bitFlag.Comments = s6xBitFlag.Comments;
            bitFlag.ShortLabel = s6xBitFlag.ShortLabel;
            bitFlag.SetValue = s6xBitFlag.SetValue;
            bitFlag.NotSetValue = s6xBitFlag.NotSetValue;

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

            bitFlag = null;
            slBitFlags = null;
        }
    }

    [Serializable]
    [XmlRoot("Operation")]
    public class S6xOperation
    {
        [XmlAttribute]
        public int BankNum { get; set; }
        [XmlAttribute]
        public int AddressInt { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        [XmlIgnore]
        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }

        [XmlIgnore]
        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        [XmlIgnore]
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public S6xOperation()
        {
        }
    }

    [Serializable]
    [XmlRoot("Register")]
    public class S6xRegister
    {
        [XmlAttribute]
        public int AddressInt { get; set; }
        [XmlAttribute]
        public string AdditionalAddress10 { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }
        [XmlIgnore]
        public bool Store { get; set; }

        public string Label { get; set; }
        public string ByteLabel { get; set; }
        public string WordLabel { get; set; }
        public string Comments { get; set; }

        [XmlArray(ElementName = "BitFlags")]
        [XmlArrayItem(ElementName = "BitFlag")]
        public S6xBitFlag[] BitFlags { get; set; }

        [XmlIgnore]
        public string Information { get; set; }

        [XmlIgnore]
        public string Address
        {
            get
            {
                if (isDual) return Convert.ToString(AddressInt, 16) + SADDef.AdditionSeparator + Convert.ToString(Convert.ToInt32(AdditionalAddress10), 16);
                else return Convert.ToString(AddressInt, 16);
            }
        }

        [XmlIgnore]
        public string UniqueAddress
        {
            get
            {
                if (isDual) return Tools.RegisterUniqueAddress(AddressInt) + SADDef.AdditionSeparator + AdditionalAddress10;
                else return Tools.RegisterUniqueAddress(AddressInt);
            }
        }

        [XmlIgnore]
        public int UniqueAddressInt
        {
            get
            {
                if (isDual) return AddressInt + Convert.ToInt32(AdditionalAddress10);
                else return AddressInt;
            }
        }

        [XmlIgnore]
        public string FullLabel { get { return (Label == Tools.RegisterInstruction(Address)) ? Label : string.Format("{0,-6}{1,4}{2}", Tools.RegisterInstruction(Address), string.Empty, Label); } }

        [XmlIgnore]
        public string FullComments { get { return string.Format("{0}\r\n\r\n{1}", FullLabel, Comments); } }

        [XmlIgnore]
        public bool isDual { get { return !(AdditionalAddress10 == null || AdditionalAddress10 == string.Empty); } }

        [XmlIgnore]
        public bool isBitFlags { get { return (BitFlags != null); } }

        [XmlIgnore]
        public int BitFlagsNum { get { if (BitFlags == null) return 0; else return BitFlags.Length; } }

        [XmlIgnore]
        public bool MultipleMeanings { get { return ((ByteLabel != null && ByteLabel != string.Empty) || (WordLabel != null && WordLabel != string.Empty)); } }

        public string Labels(bool forByte)
        {
            if (!MultipleMeanings) return Label;

            string label = Label;
            if (forByte) label = (ByteLabel != null && ByteLabel != string.Empty) ? ByteLabel : Address;
            else label = (WordLabel != null && WordLabel != string.Empty) ? WordLabel : Address;

            return label;
        }

        public string FullLabels(bool forByte, bool includeSizeDetail)
        {
            string label = Labels(forByte);
            if (includeSizeDetail)
            {
                return (label == Tools.RegisterInstruction(Address)) ? label : string.Format("{0,-10}{1}", Tools.RegisterInstruction(Address) + " " + ((forByte) ? "B" : "W"), label);
            }
            else
            {
                return (label == Tools.RegisterInstruction(Address)) ? label : string.Format("{0,-10}{1}", Tools.RegisterInstruction(Address), label);
            }
        }

        public S6xRegister()
        {
        }

        public S6xRegister(int addressInt)
        {
            AddressInt = addressInt;
        }

        public S6xRegister(string address)
        {
            if (address.Contains(SADDef.AdditionSeparator))
            {
                string regPart1 = address.Substring(0, address.IndexOf(SADDef.AdditionSeparator));
                string regPart2 = address.Replace(regPart1 + SADDef.AdditionSeparator, string.Empty);

                AddressInt = Convert.ToInt32(regPart1, 16);
                AdditionalAddress10 = Convert.ToString(Convert.ToInt32(regPart2, 16), 10);
            }
            else
            {
                AddressInt = Convert.ToInt32(address, 16);
            }
        }

        public void AddBitFlag(S6xBitFlag s6xBitFlag)
        {
            S6xBitFlag bitFlag = null;
            SortedList slBitFlags = null;
            bool existingBitFlag = false;

            slBitFlags = new SortedList();
            if (BitFlags != null) foreach (S6xBitFlag bF in BitFlags) slBitFlags.Add(bF.UniqueKey, bF);

            bitFlag = new S6xBitFlag();
            bitFlag.Position = s6xBitFlag.Position;

            existingBitFlag = slBitFlags.ContainsKey(bitFlag.UniqueKey);
            if (existingBitFlag) bitFlag = (S6xBitFlag)slBitFlags[bitFlag.UniqueKey];
            else slBitFlags.Add(bitFlag.UniqueKey, bitFlag);

            bitFlag.Label = s6xBitFlag.Label;
            bitFlag.Comments = s6xBitFlag.Comments;
            bitFlag.ShortLabel = s6xBitFlag.ShortLabel;
            bitFlag.SetValue = s6xBitFlag.SetValue;
            bitFlag.NotSetValue = s6xBitFlag.NotSetValue;

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

            bitFlag = null;
            slBitFlags = null;
        }

        public S6xRegister Clone()
        {
            S6xRegister clone = new S6xRegister();
            clone.AddressInt = AddressInt;
            clone.AdditionalAddress10 = AdditionalAddress10;
            clone.Skip = Skip;
            clone.Store = Store;

            clone.Label = Label;
            clone.ByteLabel = ByteLabel;
            clone.WordLabel = WordLabel;
            clone.Comments = Comments;

            clone.Information = Information;

            if (BitFlags != null) clone.BitFlags = (S6xBitFlag[])BitFlags.Clone();
            return clone;
        }
    }

    [Serializable]
    [XmlRoot("OtherAddress")]
    public class S6xOtherAddress
    {
        [XmlAttribute]
        public int BankNum { get; set; }
        [XmlAttribute]
        public int AddressInt { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        [XmlIgnore]
        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }

        [XmlIgnore]
        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        [XmlIgnore]
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        public S6xOtherAddress()
        {
        }
    }

    [Serializable]
    [XmlRoot("Signature")]
    public class S6xSignature
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }

        public string Signature { get; set; }

        [XmlAttribute]
        public string ShortLabel { get; set; }

        [XmlAttribute]
        public bool OutputComments { get; set; }
        
        public string Label { get; set; }
        public string Comments { get; set; }

        [XmlIgnore]
        public bool Ignore { get; set; }
        [XmlIgnore]
        public string Information { get; set; }

        [XmlArray(ElementName = "InternalStructures")]
        [XmlArrayItem(ElementName = "InternalStructure")]
        public S6xRoutineInternalStructure[] InternalStructures { get; set; }

        [XmlArray(ElementName = "InternalTables")]
        [XmlArrayItem(ElementName = "InternalTable")]
        public S6xRoutineInternalTable[] InternalTables { get; set; }

        [XmlArray(ElementName = "InternalFunctions")]
        [XmlArrayItem(ElementName = "InternalFunction")]
        public S6xRoutineInternalFunction[] InternalFunctions { get; set; }

        [XmlArray(ElementName = "InternalScalars")]
        [XmlArrayItem(ElementName = "InternalScalar")]
        public S6xRoutineInternalScalar[] InternalScalars { get; set; }

        [XmlArray(ElementName = "InputArguments")]
        [XmlArrayItem(ElementName = "InputArgument")]
        public S6xRoutineInputArgument[] InputArguments { get; set; }

        [XmlArray(ElementName = "InputStructures")]
        [XmlArrayItem(ElementName = "InputStructure")]
        public S6xRoutineInputStructure[] InputStructures { get; set; }

        [XmlArray(ElementName = "InputTables")]
        [XmlArrayItem(ElementName = "InputTable")]
        public S6xRoutineInputTable[] InputTables { get; set; }

        [XmlArray(ElementName = "InputFunctions")]
        [XmlArrayItem(ElementName = "InputFunction")]
        public S6xRoutineInputFunction[] InputFunctions { get; set; }

        [XmlArray(ElementName = "InputScalars")]
        [XmlArrayItem(ElementName = "InputScalar")]
        public S6xRoutineInputScalar[] InputScalars { get; set; }

        [XmlIgnore]
        public bool isAdvanced
        {
            get
            {
                if (InputArguments != null) return true;
                if (InputStructures != null) return true;
                if (InputTables != null) return true;
                if (InputFunctions != null) return true;
                if (InputScalars != null) return true;
                if (InternalStructures != null) return true;
                if (InternalTables != null) return true;
                if (InternalFunctions != null) return true;
                if (InternalScalars != null) return true;

                return false;
            }
        }

        [XmlIgnore]
        public int InputArgumentsNum { get { if (InputArguments == null) return 0; else return InputArguments.Length; } }
        [XmlIgnore]
        public int InputStructuresNum { get { if (InputStructures == null) return 0; else return InputStructures.Length; } }
        [XmlIgnore]
        public int InputTablesNum { get { if (InputTables == null) return 0; else return InputTables.Length; } }
        [XmlIgnore]
        public int InputFunctionsNum { get { if (InputFunctions == null) return 0; else return InputFunctions.Length; } }
        [XmlIgnore]
        public int InputScalarsNum { get { if (InputScalars == null) return 0; else return InputScalars.Length; } }
        [XmlIgnore]
        public int InternalStructuresNum { get { if (InternalStructures == null) return 0; else return InternalStructures.Length; } }
        [XmlIgnore]
        public int InternalTablesNum { get { if (InternalTables == null) return 0; else return InternalTables.Length; } }
        [XmlIgnore]
        public int InternalFunctionsNum { get { if (InternalFunctions == null) return 0; else return InternalFunctions.Length; } }
        [XmlIgnore]
        public int InternalScalarsNum { get { if (InternalScalars == null) return 0; else return InternalScalars.Length; } }

        public S6xSignature()
        {
        }

        public S6xSignature Clone()
        {
            S6xSignature clone = new S6xSignature();
            clone.UniqueKey = UniqueKey;
            clone.Skip = Skip;
            clone.Signature = Signature;
            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;

            if (InputArguments != null)
            {
                clone.InputArguments = new S6xRoutineInputArgument[InputArguments.Length];
                for (int i = 0; i < InputArguments.Length; i++) clone.InputArguments[i] = InputArguments[i].Clone();
            }
            if (InputStructures != null)
            {
                clone.InputStructures = new S6xRoutineInputStructure[InputStructures.Length];
                for (int i = 0; i < InputStructures.Length; i++) clone.InputStructures[i] = InputStructures[i].Clone();
            }
            if (InputTables != null)
            {
                clone.InputTables = new S6xRoutineInputTable[InputTables.Length];
                for (int i = 0; i < InputTables.Length; i++) clone.InputTables[i] = InputTables[i].Clone();
            }
            if (InputFunctions != null)
            {
                clone.InputFunctions = new S6xRoutineInputFunction[InputFunctions.Length];
                for (int i = 0; i < InputFunctions.Length; i++) clone.InputFunctions[i] = InputFunctions[i].Clone();
            }
            if (InputScalars != null)
            {
                clone.InputScalars = new S6xRoutineInputScalar[InputScalars.Length];
                for (int i = 0; i < InputScalars.Length; i++) clone.InputScalars[i] = InputScalars[i].Clone();
            }

            if (InternalStructures != null)
            {
                clone.InternalStructures = new S6xRoutineInternalStructure[InternalStructures.Length];
                for (int i = 0; i < InternalStructures.Length; i++) clone.InternalStructures[i] = InternalStructures[i].Clone();
            }
            if (InternalTables != null)
            {
                clone.InternalTables = new S6xRoutineInternalTable[InternalTables.Length];
                for (int i = 0; i < InternalTables.Length; i++) clone.InternalTables[i] = InternalTables[i].Clone();
            }
            if (InternalFunctions != null)
            {
                clone.InternalFunctions = new S6xRoutineInternalFunction[InternalFunctions.Length];
                for (int i = 0; i < InternalFunctions.Length; i++) clone.InternalFunctions[i] = InternalFunctions[i].Clone();
            }
            if (InternalScalars != null)
            {
                clone.InternalScalars = new S6xRoutineInternalScalar[InternalScalars.Length];
                for (int i = 0; i < InternalScalars.Length; i++) clone.InternalScalars[i] = InternalScalars[i].Clone();
            }

            return clone;
        }
    }

    [Serializable]
    [XmlRoot("ElementSignature")]
    public class S6xElementSignature
    {
        [XmlAttribute]
        public string UniqueKey { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }

        [XmlAttribute]
        public bool Forced { get; set; }

        public string Signature { get; set; }

        public string SignatureLabel { get; set; }
        public string SignatureComments { get; set; }

        public string SignatureOpeIncludingElemAddress { get; set; }

        [XmlIgnore]
        public string SignatureKey
        {
            get
            {
                if (Structure != null) return Structure.ShortLabel;
                else if (Table != null) return Table.ShortLabel;
                else if (Function != null) return Function.ShortLabel;
                else if (Scalar != null) return Scalar.ShortLabel;

                return string.Empty;
            }
        }

        [XmlIgnore]
        public string Information { get; set; }

        [XmlIgnore]
        public bool for8061 { get; set; }
        [XmlIgnore]
        public string forBankNum { get; set; }
        [XmlIgnore]
        public bool Found { get; set; }

        public S6xRoutineInternalStructure Structure { get; set; }
        public S6xRoutineInternalTable Table { get; set; }
        public S6xRoutineInternalFunction Function { get; set; }
        public S6xRoutineInternalScalar Scalar { get; set; }

        public S6xElementSignature()
        {
            Forced = false;

            if (SignatureOpeIncludingElemAddress == null) SignatureOpeIncludingElemAddress = SADFixedSigs.Fixed_String_OpeIncludingElemAddress;
        }

        public S6xElementSignature(object[] elementSignature)
        {
            Forced = true;

            for8061 = true;
            forBankNum = string.Empty;

            try
            {
                UniqueKey = (string)elementSignature[0];
                SignatureLabel = (string)elementSignature[1];
                SignatureComments = (string)elementSignature[2];

                for8061 = (bool)elementSignature[3];
                forBankNum = (string)elementSignature[4];

                object oElement = SADFixedSigs.GetFixedElementS6xRoutineInternalTemplate((SADFixedSigs.Fixed_Elements)elementSignature[5]);
                if (oElement.GetType() == typeof(S6xRoutineInternalStructure)) Structure = (S6xRoutineInternalStructure)oElement;
                else if (oElement.GetType() == typeof(S6xRoutineInternalTable)) Table = (S6xRoutineInternalTable)oElement;
                else if (oElement.GetType() == typeof(S6xRoutineInternalFunction)) Function = (S6xRoutineInternalFunction)oElement;
                else if (oElement.GetType() == typeof(S6xRoutineInternalScalar)) Scalar = (S6xRoutineInternalScalar)oElement;
                oElement = null;

                Signature = (string)elementSignature[6];

                Found = false;

                SignatureOpeIncludingElemAddress = SADFixedSigs.Fixed_String_OpeIncludingElemAddress;
                
                if (Signature.Replace(SADFixedSigs.Fixed_String_OpeIncludingElemAddress, string.Empty).Length <= 0)
                {
                    Skip = true;
                    return;
                }

                Skip = false;
            }
            catch
            {
                Skip = true;
            }
        }

        public S6xElementSignature Clone()
        {
            S6xElementSignature clone = new S6xElementSignature();
            clone.UniqueKey = UniqueKey;
            clone.Skip = Skip;
            clone.Signature = Signature;
            clone.SignatureLabel = SignatureLabel;
            clone.SignatureComments = SignatureComments;

            clone.for8061 = for8061;
            clone.forBankNum = forBankNum;
            clone.Found = Found;

            if (Structure != null) clone.Structure = Structure.Clone();
            if (Table != null) clone.Table = Table.Clone();
            if (Function != null) clone.Function = Function.Clone();
            if (Scalar != null) clone.Scalar = Scalar.Clone();

            return clone;
        }

        public bool matchSignature(ref Operation elementUseOpe, SADBank elementUseOpeBank, bool is8061)
        {
            if (Skip || Found) return false;
            if (Signature.Contains("*") || Signature.Contains("{")) return false; // Not authorized for elements, based on operation location
            if (for8061 && !is8061) return false;
            switch (forBankNum)
            {
                case "0":
                case "1":
                case "8":
                case "9":
                    if (Convert.ToInt32(forBankNum) != elementUseOpeBank.Num) return false;
                    break;
            }

            string preparedSignature = Tools.getPreparedSignature(Signature);

            int startPos = elementUseOpe.AddressInt;
            if (preparedSignature.Contains(SignatureOpeIncludingElemAddress)) startPos -= preparedSignature.IndexOf(SignatureOpeIncludingElemAddress) / 2;

            preparedSignature = preparedSignature.Replace(SignatureOpeIncludingElemAddress, elementUseOpe.OriginalOp.Replace(SADDef.GlobalSeparator, string.Empty).ToUpper());

            string sBytes = elementUseOpeBank.getBytes(startPos, preparedSignature.Length / 2);
            object[] oBytes = Tools.getBytesFromSignature(preparedSignature, ref sBytes);
            if (oBytes.Length == 0) return false; // Not Macthing Signature

            // Matching Signature - Element is identified
            return true;
}
    }

    [Serializable]
    [XmlRoot("BankProperties")]
    public class S6xBankProperties
    {
        [XmlAttribute]
        public string BankNum { get; set; }
        [XmlAttribute]
        public string AddressBin { get; set; }

        public S6xBankProperties()
        {
        }
    }

    [Serializable]
    [XmlRoot("Properties")]
    public class S6xProperties
    {
        public string Label { get; set; }
        public string Comments { get; set; }

        [XmlAttribute]
        public bool NoNumbering { get; set; }

        [XmlAttribute]
        public bool RegListOutput { get; set; }

        [XmlAttribute]
        public string XdfBaseOffset { get; set; }
        [XmlAttribute]
        public bool XdfBaseOffsetSubtract { get; set; }

        [XmlArray(ElementName = "BanksProperties")]
        [XmlArrayItem(ElementName = "BankProperties")]
        public S6xBankProperties[] BanksProperties { get; set; }

        [XmlIgnore]
        public int XdfBaseOffsetInt
        {
            get
            {
                int baseOffset = 0;
                try { baseOffset = Convert.ToInt32(XdfBaseOffset, 16); }
                catch { baseOffset = 0; }

                if (XdfBaseOffsetSubtract) return 0 - baseOffset;
                else return baseOffset;
            }
        }
        
        public S6xProperties()
        {
        }
    }

    [Serializable]
    [XmlRoot("S6x")]
    public class S6xXml
    {
        [XmlAttribute]
        public string version { get; set; }
        [XmlAttribute]
        public string generated { get; set; }

        public S6xProperties Properties = null;

        [XmlArray(ElementName = "Tables")]
        [XmlArrayItem(ElementName = "Table")]
        public S6xTable[] S6xTables { get; set; }
        [XmlArray(ElementName = "Functions")]
        [XmlArrayItem(ElementName = "Function")]
        public S6xFunction[] S6xFunctions { get; set; }
        [XmlArray(ElementName = "Scalars")]
        [XmlArrayItem(ElementName = "Scalar")]
        public S6xScalar[] S6xScalars { get; set; }
        [XmlArray(ElementName = "Structures")]
        [XmlArrayItem(ElementName = "Structure")]
        public S6xStructure[] S6xStructures { get; set; }
        [XmlArray(ElementName = "Routines")]
        [XmlArrayItem(ElementName = "Routine")]
        public S6xRoutine[] S6xRoutines { get; set; }
        [XmlArray(ElementName = "Operations")]
        [XmlArrayItem(ElementName = "Operation")]
        public S6xOperation[] S6xOperations { get; set; }
        [XmlArray(ElementName = "Registers")]
        [XmlArrayItem(ElementName = "Register")]
        public S6xRegister[] S6xRegisters { get; set; }
        [XmlArray(ElementName = "OtherAddresses")]
        [XmlArrayItem(ElementName = "OtherAddress")]
        public S6xOtherAddress[] S6xOtherAddresses { get; set; }
        [XmlArray(ElementName = "Signatures")]
        [XmlArrayItem(ElementName = "Signature")]
        public S6xSignature[] S6xSignatures { get; set; }
        [XmlArray(ElementName = "ElementsSignatures")]
        [XmlArrayItem(ElementName = "ElementSignature")]
        public S6xElementSignature[] S6xElementsSignatures { get; set; }

        //Duplicates
        [XmlArray(ElementName = "DupTables")]
        [XmlArrayItem(ElementName = "Table")]
        public S6xTable[] S6xDupTables { get; set; }
        [XmlArray(ElementName = "DupFunctions")]
        [XmlArrayItem(ElementName = "Function")]
        public S6xFunction[] S6xDupFunctions { get; set; }
        [XmlArray(ElementName = "DupScalars")]
        [XmlArrayItem(ElementName = "Scalar")]
        public S6xScalar[] S6xDupScalars { get; set; }
        [XmlArray(ElementName = "DupStructures")]
        [XmlArrayItem(ElementName = "Structure")]
        public S6xStructure[] S6xDupStructures { get; set; }

        public S6xXml()
        {
            Properties = new S6xProperties();
        }
    }
}
