using System;
using System.Collections;
using System.Collections.Generic;
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
        private DateTime s6xFileDateCreated = DateTime.UtcNow;
        private DateTime s6xFileDateUpdated = DateTime.UtcNow;

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
        public DateTime FileDateCreated { get { return s6xFileDateCreated; } }
        public DateTime FileDateUpdated { get { return s6xFileDateUpdated; } }

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

            /*
            if (File.Exists(s6xFilePath)) new FileInfo(s6xFilePath).Name;
            else s6xFileName = s6xFilePath.Substring(s6xFilePath.LastIndexOf('\\') + 1);
            */

            try
            {
                FileInfo fiFI = new FileInfo(s6xFilePath);
                s6xFileName = fiFI.Name;
                if (fiFI.Exists)
                {
                    s6xFileDateCreated = fiFI.CreationTimeUtc;
                    s6xFileDateUpdated = fiFI.LastWriteTimeUtc;
                }
                fiFI = null;
            }
            catch { }

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

                    Properties.DateCreated = FileDateCreated;
                    Properties.DateUpdated = FileDateUpdated;
                    Properties.IdentificationStatus = 0;
                    Properties.IdentificationDetails = string.Empty;

                    if (SADFixedSigs.Fixed_Routines_Signatures != null)
                    {
                        foreach (object[] routineSignature in SADFixedSigs.Fixed_Routines_Signatures)
                        {
                            S6xSignature s6xRS = new S6xSignature(routineSignature);
                            s6xRS.DateCreated = Properties.DateCreated;
                            s6xRS.DateUpdated = Properties.DateUpdated;
                            slSignatures.Add(s6xRS.UniqueKey, s6xRS);
                        }
                    }
                    if (SADFixedSigs.Fixed_Elements_Signatures != null)
                    {
                        foreach (object[] elementSignature in SADFixedSigs.Fixed_Elements_Signatures)
                        {
                            S6xElementSignature s6xES = new S6xElementSignature(elementSignature);
                            s6xES.DateCreated = Properties.DateCreated;
                            s6xES.DateUpdated = Properties.DateUpdated;
                            slElementsSignatures.Add(s6xES.UniqueKey, s6xES);
                        }
                    }
                    return;
                }

                s6xXml = (S6xXml)ToolsXml.DeserializeFile(FilePath, typeof(S6xXml));

                Properties = s6xXml.Properties;
                // Managing new version
                Properties.DateCreated = Tools.getValidDateTime(Properties.DateCreated, FileDateCreated);
                Properties.DateUpdated = Tools.getValidDateTime(Properties.DateUpdated, FileDateUpdated);
                if (Properties.IdentificationStatus < 0) Properties.IdentificationStatus = 0;
                if (Properties.IdentificationStatus > 100) Properties.IdentificationStatus = 100;
                if (Properties.IdentificationDetails == null) Properties.IdentificationDetails = string.Empty;

                if (s6xXml.S6xTables != null)
                {
                    foreach (S6xTable s6xTable in s6xXml.S6xTables)
                    {
                        s6xTable.Store = true;
                        s6xTable.DateCreated = Tools.getValidDateTime(s6xTable.DateCreated, Properties.DateCreated);
                        s6xTable.DateUpdated = Tools.getValidDateTime(s6xTable.DateUpdated, Properties.DateCreated);
                        slTables.Add(s6xTable.UniqueAddress, s6xTable);
                    }
                }
                if (s6xXml.S6xFunctions != null)
                {
                    foreach (S6xFunction s6xFunction in s6xXml.S6xFunctions)
                    {
                        s6xFunction.Store = true;
                        s6xFunction.DateCreated = Tools.getValidDateTime(s6xFunction.DateCreated, Properties.DateCreated);
                        s6xFunction.DateUpdated = Tools.getValidDateTime(s6xFunction.DateUpdated, Properties.DateCreated);
                        slFunctions.Add(s6xFunction.UniqueAddress, s6xFunction);
                    }
                }
                if (s6xXml.S6xScalars != null)
                {
                    foreach (S6xScalar s6xScalar in s6xXml.S6xScalars)
                    {
                        s6xScalar.Store = true;
                        s6xScalar.DateCreated = Tools.getValidDateTime(s6xScalar.DateCreated, Properties.DateCreated);
                        s6xScalar.DateUpdated = Tools.getValidDateTime(s6xScalar.DateUpdated, Properties.DateCreated);
                        if (s6xScalar.BitFlags != null)
                        {
                            foreach (S6xBitFlag s6xBF in s6xScalar.BitFlags)
                            {
                                if (s6xBF == null) continue;
                                s6xBF.DateCreated = Tools.getValidDateTime(s6xBF.DateCreated, s6xScalar.DateCreated);
                                s6xBF.DateUpdated = Tools.getValidDateTime(s6xBF.DateUpdated, s6xScalar.DateCreated);
                            }
                        }
                        slScalars.Add(s6xScalar.UniqueAddress, s6xScalar);
                    }
                }
                if (s6xXml.S6xStructures != null)
                {
                    foreach (S6xStructure s6xStructure in s6xXml.S6xStructures)
                    {
                        s6xStructure.Store = true;
                        s6xStructure.DateCreated = Tools.getValidDateTime(s6xStructure.DateCreated, Properties.DateCreated);
                        s6xStructure.DateUpdated = Tools.getValidDateTime(s6xStructure.DateUpdated, Properties.DateCreated);
                        slStructures.Add(s6xStructure.UniqueAddress, s6xStructure);
                    }
                }
                if (s6xXml.S6xRoutines != null)
                {
                    foreach (S6xRoutine s6xRoutine in s6xXml.S6xRoutines)
                    {
                        s6xRoutine.Store = true;
                        s6xRoutine.DateCreated = Tools.getValidDateTime(s6xRoutine.DateCreated, Properties.DateCreated);
                        s6xRoutine.DateUpdated = Tools.getValidDateTime(s6xRoutine.DateUpdated, Properties.DateCreated);
                        if (s6xRoutine.InputArguments != null)
                        {
                            foreach (S6xRoutineInputArgument s6xSubObject in s6xRoutine.InputArguments)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xRoutine.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xRoutine.DateCreated);
                            }
                        }
                        if (s6xRoutine.InputStructures != null)
                        {
                            foreach (S6xRoutineInputStructure s6xSubObject in s6xRoutine.InputStructures)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xRoutine.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xRoutine.DateCreated);
                            }
                        }
                        if (s6xRoutine.InputTables != null)
                        {
                            foreach (S6xRoutineInputTable s6xSubObject in s6xRoutine.InputTables)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xRoutine.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xRoutine.DateCreated);
                            }
                        }
                        if (s6xRoutine.InputFunctions != null)
                        {
                            foreach (S6xRoutineInputFunction s6xSubObject in s6xRoutine.InputFunctions)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xRoutine.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xRoutine.DateCreated);
                            }
                        }
                        if (s6xRoutine.InputScalars != null)
                        {
                            foreach (S6xRoutineInputScalar s6xSubObject in s6xRoutine.InputScalars)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xRoutine.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xRoutine.DateCreated);
                            }
                        }
                        slRoutines.Add(s6xRoutine.UniqueAddress, s6xRoutine);
                    }
                }
                if (s6xXml.S6xOperations != null)
                {
                    foreach (S6xOperation s6xOpe in s6xXml.S6xOperations)
                    {
                        s6xOpe.DateCreated = Tools.getValidDateTime(s6xOpe.DateCreated, Properties.DateCreated);
                        s6xOpe.DateUpdated = Tools.getValidDateTime(s6xOpe.DateUpdated, Properties.DateCreated);
                        slOperations.Add(s6xOpe.UniqueAddress, s6xOpe);
                    }
                }
                if (s6xXml.S6xRegisters != null)
                {
                    foreach (S6xRegister s6xReg in s6xXml.S6xRegisters)
                    {
                        s6xReg.Store = true;
                        s6xReg.DateCreated = Tools.getValidDateTime(s6xReg.DateCreated, Properties.DateCreated);
                        s6xReg.DateUpdated = Tools.getValidDateTime(s6xReg.DateUpdated, Properties.DateCreated);
                        if (s6xReg.BitFlags != null)
                        {
                            foreach (S6xBitFlag s6xBF in s6xReg.BitFlags)
                            {
                                s6xBF.DateCreated = Tools.getValidDateTime(s6xBF.DateCreated, s6xReg.DateCreated);
                                s6xBF.DateUpdated = Tools.getValidDateTime(s6xBF.DateUpdated, s6xReg.DateCreated);
                            }
                        }
                        slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                    }
                }
                if (s6xXml.S6xOtherAddresses != null)
                {
                    foreach (S6xOtherAddress s6xOther in s6xXml.S6xOtherAddresses)
                    {
                        s6xOther.DateCreated = Tools.getValidDateTime(s6xOther.DateCreated, Properties.DateCreated);
                        s6xOther.DateUpdated = Tools.getValidDateTime(s6xOther.DateUpdated, Properties.DateCreated);
                        slOtherAddresses.Add(s6xOther.UniqueAddress, s6xOther);
                    }
                }
                if (s6xXml.S6xSignatures != null)
                {
                    foreach (S6xSignature s6xSig in s6xXml.S6xSignatures)
                    {
                        s6xSig.DateCreated = Tools.getValidDateTime(s6xSig.DateCreated, Properties.DateCreated);
                        s6xSig.DateUpdated = Tools.getValidDateTime(s6xSig.DateUpdated, Properties.DateCreated);
                        s6xSig.RoutineDateCreated = Tools.getValidDateTime(s6xSig.RoutineDateCreated, s6xSig.DateCreated);
                        s6xSig.RoutineDateUpdated = Tools.getValidDateTime(s6xSig.RoutineDateUpdated, s6xSig.DateCreated);
                        if (s6xSig.InputStructures != null)
                        {
                            foreach (S6xRoutineInputStructure s6xSubObject in s6xSig.InputStructures)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xSig.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xSig.DateCreated);
                            }
                        }
                        if (s6xSig.InputTables != null)
                        {
                            foreach (S6xRoutineInputTable s6xSubObject in s6xSig.InputTables)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xSig.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xSig.DateCreated);
                            }
                        }
                        if (s6xSig.InputFunctions != null)
                        {
                            foreach (S6xRoutineInputFunction s6xSubObject in s6xSig.InputFunctions)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xSig.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xSig.DateCreated);
                            }
                        }
                        if (s6xSig.InputScalars != null)
                        {
                            foreach (S6xRoutineInputScalar s6xSubObject in s6xSig.InputScalars)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xSig.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xSig.DateCreated);
                            }
                        }
                        if (s6xSig.InternalStructures != null)
                        {
                            foreach (S6xRoutineInternalStructure s6xSubObject in s6xSig.InternalStructures)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xSig.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xSig.DateCreated);
                            }
                        }
                        if (s6xSig.InternalTables != null)
                        {
                            foreach (S6xRoutineInternalTable s6xSubObject in s6xSig.InternalTables)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xSig.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xSig.DateCreated);
                            }
                        }
                        if (s6xSig.InternalFunctions != null)
                        {
                            foreach (S6xRoutineInternalFunction s6xSubObject in s6xSig.InternalFunctions)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xSig.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xSig.DateCreated);
                            }
                        }
                        if (s6xSig.InternalScalars != null)
                        {
                            foreach (S6xRoutineInternalScalar s6xSubObject in s6xSig.InternalScalars)
                            {
                                if (s6xSubObject == null) continue;
                                s6xSubObject.DateCreated = Tools.getValidDateTime(s6xSubObject.DateCreated, s6xSig.DateCreated);
                                s6xSubObject.DateUpdated = Tools.getValidDateTime(s6xSubObject.DateUpdated, s6xSig.DateCreated);
                            }
                        }
                        slSignatures.Add(s6xSig.UniqueKey, s6xSig);
                    }
                }

                if (s6xXml.S6xElementsSignatures != null)
                {
                    foreach (S6xElementSignature s6xESig in s6xXml.S6xElementsSignatures)
                    {
                        s6xESig.DateCreated = Tools.getValidDateTime(s6xESig.DateCreated, Properties.DateCreated);
                        s6xESig.DateUpdated = Tools.getValidDateTime(s6xESig.DateUpdated, Properties.DateCreated);
                        if (s6xESig.Structure != null)
                        {
                            s6xESig.Structure.DateCreated = Tools.getValidDateTime(s6xESig.Structure.DateCreated, s6xESig.DateCreated);
                            s6xESig.Structure.DateUpdated = Tools.getValidDateTime(s6xESig.Structure.DateUpdated, s6xESig.DateCreated);
                        }
                        if (s6xESig.Table != null)
                        {
                            s6xESig.Table.DateCreated = Tools.getValidDateTime(s6xESig.Table.DateCreated, s6xESig.DateCreated);
                            s6xESig.Table.DateUpdated = Tools.getValidDateTime(s6xESig.Table.DateUpdated, s6xESig.DateCreated);
                        }
                        if (s6xESig.Function != null)
                        {
                            s6xESig.Function.DateCreated = Tools.getValidDateTime(s6xESig.Function.DateCreated, s6xESig.DateCreated);
                            s6xESig.Function.DateUpdated = Tools.getValidDateTime(s6xESig.Function.DateUpdated, s6xESig.DateCreated);
                        }
                        if (s6xESig.Scalar != null)
                        {
                            s6xESig.Scalar.DateCreated = Tools.getValidDateTime(s6xESig.Scalar.DateCreated, s6xESig.DateCreated);
                            s6xESig.Scalar.DateUpdated = Tools.getValidDateTime(s6xESig.Scalar.DateUpdated, s6xESig.DateCreated);
                        }
                        slElementsSignatures.Add(s6xESig.UniqueKey, s6xESig);
                    }
                }
                // Fixed Signature Mngt
                if (SADFixedSigs.Fixed_Routines_Signatures != null)
                {
                    foreach (object[] routineSignature in SADFixedSigs.Fixed_Routines_Signatures)
                    {
                        S6xSignature s6xRS = new S6xSignature(routineSignature);
                        s6xRS.DateCreated = Properties.DateCreated;
                        s6xRS.DateUpdated = Properties.DateCreated;
                        if (slSignatures.ContainsKey(s6xRS.UniqueKey)) slSignatures[s6xRS.UniqueKey] = s6xRS;
                        else slSignatures.Add(s6xRS.UniqueKey, s6xRS);
                    }
                }
                if (SADFixedSigs.Fixed_Elements_Signatures != null)
                {
                    foreach (object[] elementSignature in SADFixedSigs.Fixed_Elements_Signatures)
                    {
                        S6xElementSignature s6xES = new S6xElementSignature(elementSignature);
                        s6xES.DateCreated = Properties.DateCreated;
                        s6xES.DateUpdated = Properties.DateCreated;
                        if (slElementsSignatures.ContainsKey(s6xES.UniqueKey)) slElementsSignatures[s6xES.UniqueKey] = s6xES;
                        else slElementsSignatures.Add(s6xES.UniqueKey, s6xES);
                    }
                }

                // Duplicates
                if (s6xXml.S6xDupTables != null)
                {
                    foreach (S6xTable s6xTable in s6xXml.S6xDupTables)
                    {
                        s6xTable.Store = true;
                        s6xTable.DateCreated = Tools.getValidDateTime(s6xTable.DateCreated, Properties.DateCreated);
                        s6xTable.DateUpdated = Tools.getValidDateTime(s6xTable.DateUpdated, Properties.DateCreated);
                        slDupTables.Add(s6xTable.DuplicateAddress, s6xTable);
                    }
                }
                if (s6xXml.S6xDupFunctions != null)
                {
                    foreach (S6xFunction s6xFunction in s6xXml.S6xDupFunctions)
                    {
                        s6xFunction.Store = true;
                        s6xFunction.DateCreated = Tools.getValidDateTime(s6xFunction.DateCreated, Properties.DateCreated);
                        s6xFunction.DateUpdated = Tools.getValidDateTime(s6xFunction.DateUpdated, Properties.DateCreated);
                        slDupFunctions.Add(s6xFunction.DuplicateAddress, s6xFunction);
                    }
                }
                if (s6xXml.S6xDupScalars != null)
                {
                    foreach (S6xScalar s6xScalar in s6xXml.S6xDupScalars)
                    {
                        s6xScalar.Store = true;
                        s6xScalar.DateCreated = Tools.getValidDateTime(s6xScalar.DateCreated, Properties.DateCreated);
                        s6xScalar.DateUpdated = Tools.getValidDateTime(s6xScalar.DateUpdated, Properties.DateCreated);
                        if (s6xScalar.BitFlags != null)
                        {
                            foreach (S6xBitFlag s6xBF in s6xScalar.BitFlags)
                            {
                                if (s6xBF == null) continue;
                                s6xBF.DateCreated = Tools.getValidDateTime(s6xBF.DateCreated, s6xScalar.DateCreated);
                                s6xBF.DateUpdated = Tools.getValidDateTime(s6xBF.DateUpdated, s6xScalar.DateCreated);
                            }
                        }
                        slDupScalars.Add(s6xScalar.DuplicateAddress, s6xScalar);
                    }
                }
                if (s6xXml.S6xDupStructures != null)
                {
                    foreach (S6xStructure s6xStructure in s6xXml.S6xDupStructures)
                    {
                        s6xStructure.Store = true;
                        s6xStructure.DateCreated = Tools.getValidDateTime(s6xStructure.DateCreated, Properties.DateCreated);
                        s6xStructure.DateUpdated = Tools.getValidDateTime(s6xStructure.DateUpdated, Properties.DateCreated);
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

        public void processS6xInit(SADBin sadBin)
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
            foreach (S6xRegister s6xObject in slRegisters.Values) if ((s6xObject.Store && !s6xObject.Skip) || s6xObject.AutoConstValue) slProcessRegisters.Add(s6xObject.UniqueAddress, s6xObject);
            foreach (S6xOtherAddress s6xObject in slOtherAddresses.Values) if (!s6xObject.Skip) slProcessOtherAddresses.Add(s6xObject.UniqueAddress, s6xObject);

            foreach (S6xSignature s6xObject in slSignatures.Values)
            {
                if (!s6xObject.Skip)
                {
                    s6xObject.Found = false;
                    s6xObject.Ignore = false;
                    slProcessSignatures.Add(s6xObject.UniqueKey, s6xObject);
                }
            }

            foreach (S6xElementSignature s6xObject in slElementsSignatures.Values)
            {
                if (!s6xObject.Skip)
                {
                    s6xObject.Found = false;
                    slProcessElementsSignatures.Add(s6xObject.UniqueKey, s6xObject);
                }
            }

            // 20200929 - Elements are checked around their isCalibration element parameter
            //            AddressBinInt is also checked 
            foreach (S6xScalar s6xObject in slProcessScalars.Values)
            {
                s6xObject.isCalibrationElement = false;
                if (s6xObject.BankNum == sadBin.Calibration.BankNum)
                {
                    if (sadBin.Calibration.isCalibrationAddress(s6xObject.AddressInt)) s6xObject.isCalibrationElement = true;
                }
                s6xObject.AddressBinInt = sadBin.getBankBinAddress(s6xObject.BankNum) + s6xObject.AddressInt;
            }
            foreach (S6xFunction s6xObject in slProcessFunctions.Values)
            {
                s6xObject.isCalibrationElement = false;
                if (s6xObject.BankNum == sadBin.Calibration.BankNum)
                {
                    if (sadBin.Calibration.isCalibrationAddress(s6xObject.AddressInt)) s6xObject.isCalibrationElement = true;
                }
                s6xObject.AddressBinInt = sadBin.getBankBinAddress(s6xObject.BankNum) + s6xObject.AddressInt;
            }
            foreach (S6xTable s6xObject in slProcessTables.Values)
            {
                s6xObject.isCalibrationElement = false;
                if (s6xObject.BankNum == sadBin.Calibration.BankNum)
                {
                    if (sadBin.Calibration.isCalibrationAddress(s6xObject.AddressInt)) s6xObject.isCalibrationElement = true;
                }
                s6xObject.AddressBinInt = sadBin.getBankBinAddress(s6xObject.BankNum) + s6xObject.AddressInt;
            }
            foreach (S6xStructure s6xObject in slProcessStructures.Values)
            {
                s6xObject.isCalibrationElement = false;
                if (s6xObject.BankNum == sadBin.Calibration.BankNum)
                {
                    if (sadBin.Calibration.isCalibrationAddress(s6xObject.AddressInt)) s6xObject.isCalibrationElement = true;
                }
                s6xObject.AddressBinInt = sadBin.getBankBinAddress(s6xObject.BankNum) + s6xObject.AddressInt;
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

        private void addElementCategories(ref SortedList<string, string[]> slS6xCategoriesByName, string[] arrCategories)
        {
            if (slS6xCategoriesByName == null) return;
            if (arrCategories == null) return;
            foreach (string category in arrCategories)
            {
                if (category == null) continue;
                if (category == string.Empty) continue;
                if (slS6xCategoriesByName.ContainsKey(category.ToUpper())) continue;
                slS6xCategoriesByName.Add(category.ToUpper(), new string[] { category, string.Empty });
            }
        }
        
        public SortedList<string, string[]> getAllElementsCategories()
        {
            SortedList<string, string[]> slS6xCategoriesByName = new SortedList<string, string[]>();

            foreach (S6xStructure s6xObject in slStructures.Values) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
            foreach (S6xStructure s6xObject in slDupStructures.Values) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
            foreach (S6xTable s6xObject in slTables.Values) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
            foreach (S6xTable s6xObject in slDupTables.Values) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
            foreach (S6xFunction s6xObject in slFunctions.Values) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
            foreach (S6xFunction s6xObject in slDupFunctions.Values) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
            foreach (S6xScalar s6xObject in slScalars.Values)
            {
                addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
                if (s6xObject.BitFlags != null)
                {
                    foreach (S6xBitFlag s6xBF in s6xObject.BitFlags) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xBF.Category, s6xBF.Category2, s6xBF.Category3 });
                }
            }
            foreach (S6xScalar s6xObject in slDupScalars.Values)
            {
                addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
                if (s6xObject.BitFlags != null)
                {
                    foreach (S6xBitFlag s6xBF in s6xObject.BitFlags) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xBF.Category, s6xBF.Category2, s6xBF.Category3 });
                }
            }

            foreach (S6xOperation s6xObject in slOperations.Values) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
            foreach (S6xOtherAddress s6xObject in slOtherAddresses.Values) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
            foreach (S6xRegister s6xObject in slRegisters.Values)
            {
                addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });
                if (s6xObject.BitFlags != null)
                {
                    foreach (S6xBitFlag s6xBF in s6xObject.BitFlags) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xBF.Category, s6xBF.Category2, s6xBF.Category3 });
                }
            }
            foreach (S6xRoutine s6xObject in slRoutines.Values) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Category, s6xObject.Category2, s6xObject.Category3 });

            foreach (S6xSignature s6xObject in slSignatures.Values)
            {
                addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.SignatureCategory, s6xObject.SignatureCategory2, s6xObject.SignatureCategory3 });
                addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.RoutineCategory, s6xObject.RoutineCategory2, s6xObject.RoutineCategory3 });
                if (s6xObject.InternalStructures != null)
                {
                    foreach (S6xRoutineInternalStructure s6xSubObject in s6xObject.InternalStructures) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xSubObject.Category, s6xSubObject.Category2, s6xSubObject.Category3 });
                }
                if (s6xObject.InternalTables != null)
                {
                    foreach (S6xRoutineInternalTable s6xSubObject in s6xObject.InternalTables) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xSubObject.Category, s6xSubObject.Category2, s6xSubObject.Category3 });
                }
                if (s6xObject.InternalFunctions != null)
                {
                    foreach (S6xRoutineInternalFunction s6xSubObject in s6xObject.InternalFunctions) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xSubObject.Category, s6xSubObject.Category2, s6xSubObject.Category3 });
                }
                if (s6xObject.InternalScalars != null)
                {
                    foreach (S6xRoutineInternalScalar s6xSubObject in s6xObject.InternalScalars)
                    {
                        addElementCategories(ref slS6xCategoriesByName, new string[] { s6xSubObject.Category, s6xSubObject.Category2, s6xSubObject.Category3 });
                        if (s6xSubObject.BitFlags != null)
                        {
                            foreach (S6xBitFlag s6xBF in s6xSubObject.BitFlags) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xBF.Category, s6xBF.Category2, s6xBF.Category3 });
                        }
                    }
                }
            }

            foreach (S6xElementSignature s6xObject in slElementsSignatures.Values)
            {
                addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.SignatureCategory, s6xObject.SignatureCategory2, s6xObject.SignatureCategory3 });
                if (s6xObject.Structure != null) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Structure.Category, s6xObject.Structure.Category2, s6xObject.Structure.Category3 });
                if (s6xObject.Table != null) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Table.Category, s6xObject.Table.Category2, s6xObject.Table.Category3 });
                if (s6xObject.Function != null) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Function.Category, s6xObject.Function.Category2, s6xObject.Function.Category3 });
                if (s6xObject.Scalar != null)
                {
                    addElementCategories(ref slS6xCategoriesByName, new string[] { s6xObject.Scalar.Category, s6xObject.Scalar.Category2, s6xObject.Scalar.Category3 });
                    if (s6xObject.Scalar.BitFlags != null)
                    {
                        foreach (S6xBitFlag s6xBF in s6xObject.Scalar.BitFlags) addElementCategories(ref slS6xCategoriesByName, new string[] { s6xBF.Category, s6xBF.Category2, s6xBF.Category3 });
                    }
                }
            }

            return slS6xCategoriesByName;
        }

        public object[] readFromFileObject(ref XdfFile xdfFile, ref SADBin sadBin, ref ArrayList alReservedAddresses, bool categsOnly)
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

            // 0 S6x Updates
            // 1 S6x Creations
            // 2 Duplicates
            // 3 Conflicts
            // 4 Ignored
            // 5 S6x Duplicates Updates
            // 6 S6x Duplicates Creations
            object[] arrResult = new object[7];

            if (xdfFile.xdfHeader == null) xdfFile.xdfHeader = new XdfHeader();
            else
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

            // Updating xdfFile.xdfHeader.xdfCategories based on S6x elements
            SortedList<string, string[]> slS6xCategoriesByName = getAllElementsCategories();
            int lastIndex = 0;
            if (xdfFile.xdfHeader.xdfCategories != null)
            {
                foreach (XdfHeaderCategory xdfHeaderCategory in xdfFile.xdfHeader.xdfCategories)
                {
                    int index = -1;
                    try { index = Convert.ToInt32(xdfHeaderCategory.index, 16); }
                    catch { continue; }

                    if (slS6xCategoriesByName.ContainsKey(xdfHeaderCategory.name.ToUpper())) slS6xCategoriesByName[xdfHeaderCategory.name.ToUpper()][1] = index.ToString();
                    else slS6xCategoriesByName.Add(xdfHeaderCategory.name.ToUpper(), new string[] { xdfHeaderCategory.name, index.ToString() });
                    if (index > lastIndex) lastIndex = index;
                }
            }

            SortedList<string, string> slS6xCategoriesByIndex = new SortedList<string, string>();
            foreach (string key in slS6xCategoriesByName.Keys)
            {
                string[] s6xCategory = slS6xCategoriesByName[key];
                if (s6xCategory[1] == string.Empty)
                {
                    lastIndex++;
                    s6xCategory[1] = lastIndex.ToString();
                }
                if (!slS6xCategoriesByIndex.ContainsKey(s6xCategory[1])) slS6xCategoriesByIndex.Add(s6xCategory[1], s6xCategory[0]);
            }
            slS6xCategoriesByName = null;

            xdfFile.xdfHeader.xdfCategories = new XdfHeaderCategory[slS6xCategoriesByIndex.Count];
            for (int iIndex = 0; iIndex < slS6xCategoriesByIndex.Count; iIndex++) xdfFile.xdfHeader.xdfCategories[iIndex] = new XdfHeaderCategory() { index = "0x" + Convert.ToInt32(slS6xCategoriesByIndex.Keys[iIndex]).ToString("X"), name = slS6xCategoriesByIndex.Values[iIndex] };
            slS6xCategoriesByIndex = null;

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
                        s6xObject = new S6xFunction(xdfObject, bankNum, address, addressBin, true, xdfFile.xdfHeader.xdfCategories);

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
                            s6xObject = new S6xFunction(xdfObject, bankNum, address, addressBin, false, xdfFile.xdfHeader.xdfCategories);
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
                S6xFunction existingObject = null;
                
                // Main Item
                if (s6xFunction.DuplicateNum == 0)
                {
                    if (categsOnly)
                    {
                        if (!slFunctions.ContainsKey(s6xFunction.UniqueAddress)) continue;
                        existingObject = (S6xFunction)slFunctions[s6xFunction.UniqueAddress];
                        existingObject.DateUpdated = DateTime.UtcNow;
                        existingObject.Category = s6xFunction.Category;
                        existingObject.Category2 = s6xFunction.Category2;
                        existingObject.Category3 = s6xFunction.Category3;
                        alS6xUpdates.Add(s6xFunction.UniqueAddress);
                    }
                    else
                    {
                        if (slFunctions.ContainsKey(s6xFunction.UniqueAddress))
                        {
                            existingObject = (S6xFunction)slFunctions[s6xFunction.UniqueAddress];
                            s6xFunction.DateUpdated = DateTime.UtcNow;
                            s6xFunction.DateCreated = existingObject.DateCreated;
                            s6xFunction.IdentificationDetails = existingObject.IdentificationDetails;
                            s6xFunction.IdentificationStatus = existingObject.IdentificationStatus;
                            s6xFunction.OutputComments = existingObject.OutputComments;
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            if (s6xFunction.ShortLabel.StartsWith("TpFc") && existingObject.ShortLabel != null && existingObject.ShortLabel != string.Empty) s6xFunction.ShortLabel = existingObject.ShortLabel;

                            slFunctions[s6xFunction.UniqueAddress] = s6xFunction;
                            alS6xUpdates.Add(s6xFunction.UniqueAddress);
                        }
                        else
                        {
                            slFunctions.Add(s6xFunction.UniqueAddress, s6xFunction);
                            alS6xCreations.Add(s6xFunction.UniqueAddress);
                        }
                    }
                }
                else if (s6xFunction.DuplicateNum >= 1)
                {
                    if (categsOnly)
                    {
                        if (!slDupFunctions.ContainsKey(s6xFunction.DuplicateAddress)) continue;
                        existingObject = (S6xFunction)slDupFunctions[s6xFunction.DuplicateAddress];
                        existingObject.DateUpdated = DateTime.UtcNow;
                        existingObject.Category = s6xFunction.Category;
                        existingObject.Category2 = s6xFunction.Category2;
                        existingObject.Category3 = s6xFunction.Category3;
                        alS6xDupUpdates.Add(s6xFunction.DuplicateAddress);
                    }
                    else
                    {
                        if (slDupFunctions.ContainsKey(s6xFunction.DuplicateAddress))
                        {
                            existingObject = (S6xFunction)slDupFunctions[s6xFunction.DuplicateAddress];
                            s6xFunction.DateUpdated = DateTime.UtcNow;
                            s6xFunction.DateCreated = existingObject.DateCreated;
                            s6xFunction.IdentificationDetails = existingObject.IdentificationDetails;
                            s6xFunction.IdentificationStatus = existingObject.IdentificationStatus;
                            s6xFunction.OutputComments = existingObject.OutputComments;
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            if (s6xFunction.ShortLabel.StartsWith("TpFc") && existingObject.ShortLabel != null && existingObject.ShortLabel != string.Empty) s6xFunction.ShortLabel = existingObject.ShortLabel;

                            slDupFunctions[s6xFunction.DuplicateAddress] = s6xFunction;
                            alS6xDupUpdates.Add(s6xFunction.DuplicateAddress);
                        }
                        else
                        {
                            slDupFunctions.Add(s6xFunction.DuplicateAddress, s6xFunction);
                            alS6xDupCreations.Add(s6xFunction.DuplicateAddress);
                        }
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
                        s6xObject = new S6xTable(xdfObject, bankNum, address, addressBin, true, ref slFunctions, xdfFile.xdfHeader.xdfCategories);

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
                            s6xObject = new S6xTable(xdfObject, bankNum, address, addressBin, false, ref slFunctions, xdfFile.xdfHeader.xdfCategories);
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
                S6xTable existingObject = null;

                // Main Item
                if (s6xTable.DuplicateNum == 0)
                {
                    if (categsOnly)
                    {
                        if (!slTables.ContainsKey(s6xTable.UniqueAddress)) continue;
                        existingObject = ((S6xTable)slTables[s6xTable.UniqueAddress]);
                        existingObject.DateUpdated = DateTime.UtcNow;
                        existingObject.Category = s6xTable.Category;
                        existingObject.Category2 = s6xTable.Category2;
                        existingObject.Category3 = s6xTable.Category3;
                        alS6xUpdates.Add(s6xTable.UniqueAddress);
                    }
                    else
                    {
                        if (slTables.ContainsKey(s6xTable.UniqueAddress))
                        {
                            existingObject = (S6xTable)slTables[s6xTable.UniqueAddress];
                            s6xTable.DateUpdated = DateTime.UtcNow;
                            s6xTable.DateCreated = existingObject.DateCreated;
                            s6xTable.IdentificationDetails = existingObject.IdentificationDetails;
                            s6xTable.IdentificationStatus = existingObject.IdentificationStatus;
                            s6xTable.OutputComments = existingObject.OutputComments;
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            if (s6xTable.ShortLabel.StartsWith("TpTb") && existingObject.ShortLabel != null && existingObject.ShortLabel != string.Empty) s6xTable.ShortLabel = existingObject.ShortLabel;

                            slTables[s6xTable.UniqueAddress] = s6xTable;
                            alS6xUpdates.Add(s6xTable.UniqueAddress);
                        }
                        else
                        {
                            slTables.Add(s6xTable.UniqueAddress, s6xTable);
                            alS6xCreations.Add(s6xTable.UniqueAddress);
                        }
                    }
                }
                else if (s6xTable.DuplicateNum >= 1)
                {
                    if (categsOnly)
                    {
                        if (!slDupTables.ContainsKey(s6xTable.DuplicateAddress)) continue;
                        existingObject = (S6xTable)slDupTables[s6xTable.DuplicateAddress];
                        existingObject.DateUpdated = DateTime.UtcNow;
                        existingObject.Category = s6xTable.Category;
                        existingObject.Category2 = s6xTable.Category2;
                        existingObject.Category3 = s6xTable.Category3;
                        alS6xDupUpdates.Add(s6xTable.DuplicateAddress);
                    }
                    else
                    {
                        if (slDupTables.ContainsKey(s6xTable.DuplicateAddress))
                        {
                            existingObject = (S6xTable)slDupTables[s6xTable.DuplicateAddress];
                            s6xTable.DateUpdated = DateTime.UtcNow;
                            s6xTable.DateCreated = existingObject.DateCreated;
                            s6xTable.IdentificationDetails = existingObject.IdentificationDetails;
                            s6xTable.IdentificationStatus = existingObject.IdentificationStatus;
                            s6xTable.OutputComments = existingObject.OutputComments;
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            if (s6xTable.ShortLabel.StartsWith("TpTb") && existingObject.ShortLabel != null && existingObject.ShortLabel != string.Empty) s6xTable.ShortLabel = existingObject.ShortLabel;

                            slDupTables[s6xTable.DuplicateAddress] = s6xTable;
                            alS6xDupUpdates.Add(s6xTable.DuplicateAddress);
                        }
                        else
                        {
                            slDupTables.Add(s6xTable.DuplicateAddress, s6xTable);
                            alS6xDupCreations.Add(s6xTable.DuplicateAddress);
                        }
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
                        s6xObject = new S6xScalar(xdfObject, bankNum, address, addressBin, true, xdfFile.xdfHeader.xdfCategories);
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
                            s6xObject = new S6xScalar(xdfObject, bankNum, address, addressBin, false, xdfFile.xdfHeader.xdfCategories);
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
                            s6xObject.AddBitFlag(xdfObject, xdfFile.xdfHeader.xdfCategories);
                            s6xObject.DateUpdated = DateTime.UtcNow;
                        }
                        else
                        {
                            s6xObject = new S6xScalar(xdfObject, bankNum, address, addressBin, true, xdfFile.xdfHeader.xdfCategories);
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
                                s6xObject.AddBitFlag(xdfObject, xdfFile.xdfHeader.xdfCategories);
                                s6xObject.DateUpdated = DateTime.UtcNow;
                            }
                            else
                            {
                                s6xObject = new S6xScalar(xdfObject, bankNum, address, addressBin, true, xdfFile.xdfHeader.xdfCategories);
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
                S6xScalar existingObject = null;

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
                            s6xScalar.Category = s6xScalar.BitFlags[0].Category;
                            s6xScalar.Category2 = s6xScalar.BitFlags[0].Category2;
                            s6xScalar.Category3 = s6xScalar.BitFlags[0].Category3;
                            s6xScalar.BitFlags = null;
                        }
                    }
                }
                
                // Main Item
                if (s6xScalar.DuplicateNum == 0)
                {
                    if (categsOnly)
                    {
                        if (!slScalars.ContainsKey(s6xScalar.UniqueAddress)) continue;
                        existingObject = (S6xScalar)slScalars[s6xScalar.UniqueAddress];
                        existingObject.DateUpdated = DateTime.UtcNow;
                        existingObject.Category = s6xScalar.Category;
                        existingObject.Category2 = s6xScalar.Category2;
                        existingObject.Category3 = s6xScalar.Category3;
                        if (s6xScalar.isBitFlags && existingObject.isBitFlags)
                        {
                            foreach (S6xBitFlag existingBitFlag in existingObject.BitFlags)
                            {
                                foreach (S6xBitFlag xdfBitFlag in s6xScalar.BitFlags)
                                {
                                    if (existingBitFlag.Position == xdfBitFlag.Position)
                                    {
                                        existingBitFlag.Category = xdfBitFlag.Category;
                                        existingBitFlag.Category2 = xdfBitFlag.Category2;
                                        existingBitFlag.Category3 = xdfBitFlag.Category3;
                                        break;
                                    }
                                }
                            }
                        }
                        alS6xUpdates.Add(s6xScalar.UniqueAddress);
                    }
                    else
                    {
                        if (slScalars.ContainsKey(s6xScalar.UniqueAddress))
                        {
                            existingObject = (S6xScalar)slScalars[s6xScalar.UniqueAddress];
                            s6xScalar.DateUpdated = DateTime.UtcNow;
                            s6xScalar.DateCreated = existingObject.DateCreated;
                            s6xScalar.IdentificationDetails = existingObject.IdentificationDetails;
                            s6xScalar.IdentificationStatus = existingObject.IdentificationStatus;
                            s6xScalar.OutputComments = existingObject.OutputComments;
                            s6xScalar.InlineComments = existingObject.InlineComments;
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            if (s6xScalar.ShortLabel.StartsWith("TpSc") && existingObject.ShortLabel != null && existingObject.ShortLabel != string.Empty) s6xScalar.ShortLabel = existingObject.ShortLabel;

                            slScalars[s6xScalar.UniqueAddress] = s6xScalar;
                            alS6xUpdates.Add(s6xScalar.UniqueAddress);
                        }
                        else
                        {
                            slScalars.Add(s6xScalar.UniqueAddress, s6xScalar);
                            alS6xCreations.Add(s6xScalar.UniqueAddress);
                        }
                    }
                }
                else if (s6xScalar.DuplicateNum >= 1)
                {
                    if (categsOnly)
                    {
                        if (!slDupScalars.ContainsKey(s6xScalar.DuplicateAddress)) continue;
                        existingObject = (S6xScalar)slDupScalars[s6xScalar.DuplicateAddress];
                        existingObject.DateUpdated = DateTime.UtcNow;
                        existingObject.Category = s6xScalar.Category;
                        existingObject.Category2 = s6xScalar.Category2;
                        existingObject.Category3 = s6xScalar.Category3;
                        if (s6xScalar.isBitFlags && existingObject.isBitFlags)
                        {
                            foreach (S6xBitFlag existingBitFlag in existingObject.BitFlags)
                            {
                                foreach (S6xBitFlag xdfBitFlag in s6xScalar.BitFlags)
                                {
                                    if (existingBitFlag.Position == xdfBitFlag.Position)
                                    {
                                        existingBitFlag.Category = xdfBitFlag.Category;
                                        existingBitFlag.Category2 = xdfBitFlag.Category2;
                                        existingBitFlag.Category3 = xdfBitFlag.Category3;
                                        break;
                                    }
                                }
                            }
                        }
                        alS6xDupUpdates.Add(s6xScalar.DuplicateAddress);
                    }
                    else
                    {
                        if (slDupScalars.ContainsKey(s6xScalar.DuplicateAddress))
                        {
                            existingObject = (S6xScalar)slDupScalars[s6xScalar.DuplicateAddress];
                            s6xScalar.DateUpdated = DateTime.UtcNow;
                            s6xScalar.DateCreated = existingObject.DateCreated;
                            s6xScalar.IdentificationDetails = existingObject.IdentificationDetails;
                            s6xScalar.IdentificationStatus = existingObject.IdentificationStatus;
                            s6xScalar.OutputComments = existingObject.OutputComments;
                            s6xScalar.InlineComments = existingObject.InlineComments;
                            // ShortLabel does not exist in Xdf, it is automatically generated, but could stay from existing S6x
                            if (s6xScalar.ShortLabel.StartsWith("TpSc") && existingObject.ShortLabel != null && existingObject.ShortLabel != string.Empty) s6xScalar.ShortLabel = existingObject.ShortLabel;

                            slDupScalars[s6xScalar.DuplicateAddress] = s6xScalar;
                            alS6xDupUpdates.Add(s6xScalar.DuplicateAddress);
                        }
                        else
                        {
                            slDupScalars.Add(s6xScalar.DuplicateAddress, s6xScalar);
                            alS6xDupCreations.Add(s6xScalar.DuplicateAddress);
                        }
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

        public object[] writeToFileObject(ref XdfFile xdfFile, ref SADBin sadBin, ref SettingsLst lstSettings)
        {
            ArrayList alS6xUpdates = new ArrayList();
            ArrayList alS6xCreations = new ArrayList();
            ArrayList alDuplicates = new ArrayList();
            ArrayList alS6xDupUpdates = new ArrayList();
            SortedList slProcessed = new SortedList();
            ArrayList alXdfObjects = null;
            ArrayList alS6xDupUniqueAddresses = null;
            ArrayList alS6xScalersAddresses = null;
            string xdfUniqueId = string.Empty;
            string uniqueAddress = string.Empty;
            int xdfBaseOffset = 0;

            bool mngtAutoDetectedTables = false;
            bool mngtAutoDetectedFunctions = false;
            bool mngtAutoDetectedScalars = false;
            bool mngtAutoDetectedBitFlags = false;

            mngtAutoDetectedTables = lstSettings.Get<bool>("AUTODETECT_MNGT_TABLES", mngtAutoDetectedTables);
            mngtAutoDetectedFunctions = lstSettings.Get<bool>("AUTODETECT_MNGT_FUNCTIONS", mngtAutoDetectedFunctions);
            mngtAutoDetectedScalars = lstSettings.Get<bool>("AUTODETECT_MNGT_SCALARS", mngtAutoDetectedScalars);
            mngtAutoDetectedBitFlags = lstSettings.Get<bool>("AUTODETECT_MNGT_BITFLAGS", mngtAutoDetectedBitFlags);
            
            // 0 S6x Updates
            // 1 S6x Creations
            // 2 Duplicates
            // 3 Conflicts  - No Mngt needed for writeToFileObject
            // 4 Ignored  - No Mngt needed for writeToFileObject
            // 5 S6x Duplicates Updates
            // 6 S6x Duplicates Creations  - No Mngt needed for writeToFileObject
            object[] arrResult = new object[7];

            int newXdfUniqueId = Convert.ToInt32(xdfFile.getLastXdfUniqueId().ToLower().Replace("0x", ""), 16) + 1;

            if (xdfFile.xdfHeader == null) xdfFile.xdfHeader = new XdfHeader();
            else
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

            // Updating xdfFile.xdfHeader.xdfCategories based on S6x elements
            SortedList<string, string[]> slS6xCategoriesByName = getAllElementsCategories();
            int lastIndex = 0;
            if (xdfFile.xdfHeader.xdfCategories != null)
            {
                foreach (XdfHeaderCategory xdfHeaderCategory in xdfFile.xdfHeader.xdfCategories)
                {
                    int index = -1;
                    try { index = Convert.ToInt32(xdfHeaderCategory.index, 16); }
                    catch { continue; }

                    if (slS6xCategoriesByName.ContainsKey(xdfHeaderCategory.name.ToUpper())) slS6xCategoriesByName[xdfHeaderCategory.name.ToUpper()][1] = index.ToString();
                    else slS6xCategoriesByName.Add(xdfHeaderCategory.name.ToUpper(), new string[] { xdfHeaderCategory.name, index.ToString() });
                    if (index > lastIndex) lastIndex = index;
                }
            }
            
            SortedList<string, string> slS6xCategoriesByIndex = new SortedList<string, string>();
            foreach (string key in slS6xCategoriesByName.Keys)
            {
                string[] s6xCategory = slS6xCategoriesByName[key];
                if (s6xCategory[1] == string.Empty)
                {
                    lastIndex++;
                    s6xCategory[1] = lastIndex.ToString();
                }
                if (!slS6xCategoriesByIndex.ContainsKey(s6xCategory[1])) slS6xCategoriesByIndex.Add(s6xCategory[1], s6xCategory[0]);
            }
            slS6xCategoriesByName = null;

            xdfFile.xdfHeader.xdfCategories = new XdfHeaderCategory[slS6xCategoriesByIndex.Count];
            for (int iIndex = 0; iIndex < slS6xCategoriesByIndex.Count; iIndex++) xdfFile.xdfHeader.xdfCategories[iIndex] = new XdfHeaderCategory() { index = "0x" + Convert.ToInt32(slS6xCategoriesByIndex.Keys[iIndex]).ToString("X"), name = slS6xCategoriesByIndex.Values[iIndex] };
            slS6xCategoriesByIndex = null;

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
                        ((S6xFunction)slFunctions[uniqueAddress]).DateUpdated = DateTime.UtcNow;
                        alS6xUpdates.Add(uniqueAddress);

                        xdfObject.Import((S6xFunction)slFunctions[uniqueAddress], xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
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
                        ((S6xFunction)slDupFunctions[s6xDup.DuplicateAddress]).DateUpdated = DateTime.UtcNow;
                        alS6xDupUpdates.Add(s6xDup.DuplicateAddress);

                        xdfObject.Import((S6xFunction)slDupFunctions[s6xDup.DuplicateAddress], xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);

                        break;
                    }
                }
            }
            // Missing Xdf Functions Creation to Prepare Scalers
            if (xdfFile.xdfFunctions == null) alXdfObjects = new ArrayList();
            else alXdfObjects = new ArrayList(xdfFile.xdfFunctions);

            // To be sure to export Scalers
            alS6xScalersAddresses = new ArrayList();
            alS6xDupUniqueAddresses = new ArrayList();
            foreach (S6xTable s6xObject in slDupTables.Values) if (!alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress)) alS6xDupUniqueAddresses.Add(s6xObject.UniqueAddress);

            foreach (S6xTable s6xObject in slTables.Values)
            {
                if (!s6xObject.Skip && (s6xObject.Store || mngtAutoDetectedTables) && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    if (s6xObject.ColsScalerAddress != null && s6xObject.ColsScalerAddress != string.Empty)
                    {
                        if (!alS6xScalersAddresses.Contains(s6xObject.ColsScalerAddress)) alS6xScalersAddresses.Add(s6xObject.ColsScalerAddress);
                    }
                    if (s6xObject.RowsScalerAddress != null && s6xObject.RowsScalerAddress != string.Empty)
                    {
                        if (!alS6xScalersAddresses.Contains(s6xObject.RowsScalerAddress)) alS6xScalersAddresses.Add(s6xObject.RowsScalerAddress);
                    }
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xTable s6xDupObject in slDupTables.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && (s6xDupObject.Store || mngtAutoDetectedTables) && s6xDupObject.AddressBinInt >= xdfBaseOffset)
                        {
                            if (s6xDupObject.ColsScalerAddress != null && s6xDupObject.ColsScalerAddress != string.Empty)
                            {
                                if (!alS6xScalersAddresses.Contains(s6xDupObject.ColsScalerAddress)) alS6xScalersAddresses.Add(s6xDupObject.ColsScalerAddress);
                            }
                            if (s6xDupObject.RowsScalerAddress != null && s6xDupObject.RowsScalerAddress != string.Empty)
                            {
                                if (!alS6xScalersAddresses.Contains(s6xDupObject.RowsScalerAddress)) alS6xScalersAddresses.Add(s6xDupObject.RowsScalerAddress);
                            }
                        }
                    }
                }
            }
            
            alS6xDupUniqueAddresses = new ArrayList();
            foreach (S6xFunction s6xObject in slDupFunctions.Values) if (!alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress)) alS6xDupUniqueAddresses.Add(s6xObject.UniqueAddress);

            foreach (S6xFunction s6xObject in slFunctions.Values)
            {
                if (!s6xObject.Skip && (s6xObject.Store || mngtAutoDetectedFunctions || alS6xScalersAddresses.Contains(s6xObject.UniqueAddress))  && !alS6xUpdates.Contains(s6xObject.UniqueAddress) && !alS6xCreations.Contains(s6xObject.UniqueAddress) && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    XdfFunction xdfObject = new XdfFunction(s6xObject, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
                    xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                    s6xObject.XdfUniqueId = xdfObject.uniqueid;
                    s6xObject.DateUpdated = DateTime.UtcNow;
                    newXdfUniqueId++;
                    alXdfObjects.Add(xdfObject);
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xFunction s6xDupObject in slDupFunctions.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && (s6xDupObject.Store || mngtAutoDetectedFunctions || alS6xScalersAddresses.Contains(s6xObject.DuplicateAddress)) && !alS6xDupUpdates.Contains(s6xDupObject.DuplicateAddress) && s6xDupObject.AddressBinInt >= xdfBaseOffset)
                        {
                            XdfFunction xdfObject = new XdfFunction(s6xDupObject, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
                            xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                            s6xDupObject.XdfUniqueId = xdfObject.uniqueid;
                            s6xDupObject.DateUpdated = DateTime.UtcNow;
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
                        s6xTable.DateUpdated = DateTime.UtcNow;
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

                        xdfObject.Import(s6xTable, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
                        s6xTable = null;
                        continue;
                    }

                    // Real Duplicates
                    foreach (S6xTable s6xDup in slDupTables.Values)
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
                        s6xTable.DateUpdated = DateTime.UtcNow;
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

                        xdfObject.Import(s6xTable, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
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
                        ((S6xScalar)slScalars[uniqueAddress]).DateUpdated = DateTime.UtcNow;
                        alS6xUpdates.Add(uniqueAddress);

                        xdfObject.Import((S6xScalar)slScalars[uniqueAddress], xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
                        continue;
                    }

                    // Real Duplicates
                    foreach (S6xScalar s6xDup in slDupScalars.Values)
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
                        ((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).DateUpdated = DateTime.UtcNow;
                        alS6xDupUpdates.Add(s6xDup.DuplicateAddress);

                        xdfObject.Import((S6xScalar)slDupScalars[s6xDup.DuplicateAddress], xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);

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
                        ((S6xScalar)slScalars[uniqueAddress]).DateUpdated = DateTime.UtcNow;

                        foreach (S6xBitFlag bitFlag in ((S6xScalar)slScalars[uniqueAddress]).BitFlags)
                        {
                            if (bitFlag.Skip) continue;

                            try
                            {
                                int iMask = Convert.ToInt32(xdfObject.mask.Replace("0x", ""), 16);
                                if (iMask == (int)Math.Pow(2, bitFlag.Position))
                                {
                                    xdfObject.Import((S6xScalar)slScalars[uniqueAddress], bitFlag, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
                                    alS6xUpdates.Add(uniqueAddress + "." + bitFlag.Position.ToString());
                                }
                            }
                            catch { }
                        }
                        continue;
                    }

                    // Real Duplicates
                    foreach (S6xScalar s6xDup in slDupScalars.Values)
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
                        ((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).DateUpdated = DateTime.UtcNow;

                        foreach (S6xBitFlag bitFlag in ((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).BitFlags)
                        {
                            if (bitFlag.Skip) continue;

                            try
                            {
                                int iMask = Convert.ToInt32(xdfObject.mask.Replace("0x", ""), 16);
                                if (iMask == (int)Math.Pow(2, bitFlag.Position))
                                {
                                    xdfObject.Import((S6xScalar)slDupScalars[s6xDup.DuplicateAddress], bitFlag, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
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
                if (!s6xObject.Skip && (s6xObject.Store || mngtAutoDetectedTables) && !alS6xUpdates.Contains(s6xObject.UniqueAddress) && !alS6xCreations.Contains(s6xObject.UniqueAddress) && s6xObject.AddressBinInt >= xdfBaseOffset)
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

                    XdfTable xdfObject = new XdfTable(s6xObject, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
                    xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                    s6xObject.XdfUniqueId = xdfObject.uniqueid;
                    s6xObject.DateUpdated = DateTime.UtcNow;
                    newXdfUniqueId++;
                    alXdfObjects.Add(xdfObject);
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xTable s6xDupObject in slDupTables.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && (s6xDupObject.Store || mngtAutoDetectedTables) && !alS6xDupUpdates.Contains(s6xDupObject.DuplicateAddress) && s6xDupObject.AddressBinInt >= xdfBaseOffset)
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
                            XdfTable xdfObject = new XdfTable(s6xDupObject, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
                            xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                            s6xDupObject.XdfUniqueId = xdfObject.uniqueid;
                            s6xDupObject.DateUpdated = DateTime.UtcNow;
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
                if (!s6xObject.Skip && (s6xObject.Store || mngtAutoDetectedScalars) && !alS6xUpdates.Contains(s6xObject.UniqueAddress) && !alS6xCreations.Contains(s6xObject.UniqueAddress) && s6xObject.AddressBinInt >= xdfBaseOffset)
                {
                    XdfScalar xdfObject = new XdfScalar(s6xObject, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
                    xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                    s6xObject.XdfUniqueId = xdfObject.uniqueid;
                    s6xObject.DateUpdated = DateTime.UtcNow;
                    newXdfUniqueId++;
                    alXdfObjects.Add(xdfObject);
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xScalar s6xDupObject in slDupScalars.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && (s6xDupObject.Store || mngtAutoDetectedScalars) && !alS6xDupUpdates.Contains(s6xDupObject.DuplicateAddress) && s6xDupObject.AddressBinInt >= xdfBaseOffset)
                        {
                            XdfScalar xdfObject = new XdfScalar(s6xDupObject, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
                            xdfObject.uniqueid = "0x" + string.Format("{0:x4}", newXdfUniqueId);
                            s6xDupObject.XdfUniqueId = xdfObject.uniqueid;
                            s6xDupObject.DateUpdated = DateTime.UtcNow;
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
                if (!s6xObject.Skip && (s6xObject.Store || mngtAutoDetectedBitFlags) && s6xObject.AddressBinInt >= xdfBaseOffset)
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
                                    XdfFlag xdfFlag = new XdfFlag(s6xObject, bitFlag, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
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
                if (!s6xObject.Skip && (s6xObject.Store || mngtAutoDetectedBitFlags) && s6xObject.AddressBinInt >= xdfBaseOffset)
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
                                    XdfFlag xdfFlag = new XdfFlag(s6xObject, bitFlag, xdfBaseOffset, xdfFile.xdfHeader.xdfCategories);
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

        public object[] readFromFileObject(ref XlsFile xlsFile, ref SADBin sadBin, ref ArrayList alReservedAddresses)
        {
            ArrayList alS6xUpdates = new ArrayList();
            ArrayList alS6xCreations = new ArrayList();
            ArrayList alDuplicates = new ArrayList();
            ArrayList alTablesConflicts = new ArrayList();
            ArrayList alFunctionsConflicts = new ArrayList();
            ArrayList alScalarsConflicts = new ArrayList();
            ArrayList alRegistersConflicts = new ArrayList();
            ArrayList alIgnored = new ArrayList();
            ArrayList alS6xDupUpdates = new ArrayList();
            ArrayList alS6xDupCreations = new ArrayList();
            SortedList slProcessed = new SortedList();

            ArrayList alFunctions = new ArrayList();
            ArrayList alTables = new ArrayList();
            ArrayList alScalars = new ArrayList();
            ArrayList alRegisters = new ArrayList();

            // 0 S6x Updates
            // 1 S6x Creations
            // 2 Duplicates
            // 3 Conflicts
            // 4 Ignored
            // 5 S6x Duplicates Updates
            // 6 S6x Duplicates Creations
            object[] arrResult = new object[7];

            // Updating xlsFile.Levels based on S6x elements
            SortedList<string, string[]> slS6xCategoriesByName = getAllElementsCategories();
            int lastIndex = 0;
            foreach (XlsLevel xlsLevel in xlsFile.Levels.Values)
            {
                int index = -1;
                if (xlsLevel.Value == null) continue;
                if (xlsLevel.Value.Contains("."))
                {
                    try { index = Convert.ToInt32(xlsLevel.Value.Split('.')[0], 16); }
                    catch { continue; }
                }
                else
                {
                    try { index = Convert.ToInt32(xlsLevel.Value, 16); }
                    catch { continue; }
                }

                if (slS6xCategoriesByName.ContainsKey(xlsLevel.Label.ToUpper())) slS6xCategoriesByName[xlsLevel.Label.ToUpper()][1] = xlsLevel.Value;
                else slS6xCategoriesByName.Add(xlsLevel.Label.ToUpper(), new string[] { xlsLevel.Label, xlsLevel.Value });
                if (index > lastIndex) lastIndex = index;
            }

            SortedList<string, string> slS6xCategoriesByIndex = new SortedList<string, string>();
            foreach (string key in slS6xCategoriesByName.Keys)
            {
                string[] s6xCategory = slS6xCategoriesByName[key];
                if (s6xCategory[1] == string.Empty)
                {
                    lastIndex++;
                    s6xCategory[1] = lastIndex.ToString();
                }
                if (!slS6xCategoriesByIndex.ContainsKey(s6xCategory[1])) slS6xCategoriesByIndex.Add(s6xCategory[1], s6xCategory[0]);
            }
            slS6xCategoriesByName = null;

            xlsFile.Levels = new SortedList<string, XlsLevel>();
            for (int iIndex = 0; iIndex < slS6xCategoriesByIndex.Count; iIndex++) xlsFile.Levels.Add(slS6xCategoriesByIndex.Keys[iIndex], new XlsLevel() { Value = slS6xCategoriesByIndex.Keys[iIndex], Label = slS6xCategoriesByIndex.Values[iIndex] });
            slS6xCategoriesByIndex = null;

            // Starting with Functions for being able to manage Table Scaling after
            if (xlsFile.Functions != null)
            {
                foreach (XlsFunction xlsObject in xlsFile.Functions.Values)
                {
                    S6xFunction s6xObject = null;
                    int bankNum = -1;
                    int address = -1;
                    int addressBin = Tools.binAddressFromXlsAddress(xlsObject.AddressInt, Properties.XdfBaseOffsetInt);
                    if (sadBin.isCalibrationAddress(addressBin))
                    {
                        bankNum = sadBin.Calibration.BankNum;
                        address = addressBin - sadBin.Calibration.BankAddressBinInt;
                        s6xObject = new S6xFunction(xlsObject, bankNum, address, addressBin, true);

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
                            s6xObject = new S6xFunction(xlsObject, bankNum, address, addressBin, false);
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
                S6xFunction existingObject = null;

                // Main Item
                if (s6xFunction.DuplicateNum == 0)
                {
                    if (slFunctions.ContainsKey(s6xFunction.UniqueAddress))
                    {
                        existingObject = (S6xFunction)slFunctions[s6xFunction.UniqueAddress];
                        s6xFunction.DateUpdated = DateTime.UtcNow;
                        s6xFunction.DateCreated = existingObject.DateCreated;
                        s6xFunction.IdentificationDetails = existingObject.IdentificationDetails;
                        s6xFunction.IdentificationStatus = existingObject.IdentificationStatus;
                        s6xFunction.OutputComments = existingObject.OutputComments;

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
                        existingObject = (S6xFunction)slDupFunctions[s6xFunction.DuplicateAddress];
                        s6xFunction.DateUpdated = DateTime.UtcNow;
                        s6xFunction.DateCreated = existingObject.DateCreated;
                        s6xFunction.IdentificationDetails = existingObject.IdentificationDetails;
                        s6xFunction.IdentificationStatus = existingObject.IdentificationStatus;
                        s6xFunction.OutputComments = existingObject.OutputComments;

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

            if (xlsFile.Tables != null)
            {
                foreach (XlsTable xlsObject in xlsFile.Tables.Values)
                {
                    S6xTable s6xObject = null;
                    int bankNum = -1;
                    int address = -1;
                    int addressBin = Tools.binAddressFromXlsAddress(xlsObject.AddressInt, Properties.XdfBaseOffsetInt);
                    if (sadBin.isCalibrationAddress(addressBin))
                    {
                        bankNum = sadBin.Calibration.BankNum;
                        address = addressBin - sadBin.Calibration.BankAddressBinInt;
                        s6xObject = new S6xTable(xlsObject, bankNum, address, addressBin, true, ref slFunctions);

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
                            s6xObject = new S6xTable(xlsObject, bankNum, address, addressBin, false, ref slFunctions);
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
                S6xTable existingObject = null;

                // Main Item
                if (s6xTable.DuplicateNum == 0)
                {
                    if (slTables.ContainsKey(s6xTable.UniqueAddress))
                    {
                        existingObject = (S6xTable)slTables[s6xTable.UniqueAddress];
                        s6xTable.DateUpdated = DateTime.UtcNow;
                        s6xTable.DateCreated = existingObject.DateCreated;
                        s6xTable.IdentificationDetails = existingObject.IdentificationDetails;
                        s6xTable.IdentificationStatus = existingObject.IdentificationStatus;
                        s6xTable.OutputComments = existingObject.OutputComments;

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
                        existingObject = (S6xTable)slDupTables[s6xTable.DuplicateAddress];
                        s6xTable.DateUpdated = DateTime.UtcNow;
                        s6xTable.DateCreated = existingObject.DateCreated;
                        s6xTable.IdentificationDetails = existingObject.IdentificationDetails;
                        s6xTable.IdentificationStatus = existingObject.IdentificationStatus;
                        s6xTable.OutputComments = existingObject.OutputComments;

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

            if (xlsFile.Scalars != null)
            {
                foreach (XlsScalar xlsObject in xlsFile.Scalars.Values)
                {
                    S6xScalar s6xObject = null;
                    int bankNum = -1;
                    int address = -1;
                    int addressBin = Tools.binAddressFromXlsAddress(xlsObject.AddressInt, Properties.XdfBaseOffsetInt);
                    if (sadBin.isCalibrationAddress(addressBin))
                    {
                        bankNum = sadBin.Calibration.BankNum;
                        address = addressBin - sadBin.Calibration.BankAddressBinInt;
                        s6xObject = new S6xScalar(xlsObject, bankNum, address, addressBin, true);
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
                            s6xObject = new S6xScalar(xlsObject, bankNum, address, addressBin, false);
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
                S6xScalar existingObject = null;

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
                            s6xScalar.Category = s6xScalar.BitFlags[0].Category;
                            s6xScalar.Category2 = s6xScalar.BitFlags[0].Category2;
                            s6xScalar.Category3 = s6xScalar.BitFlags[0].Category3;
                            s6xScalar.BitFlags = null;
                        }
                    }
                }

                // Main Item
                if (s6xScalar.DuplicateNum == 0)
                {
                    if (slScalars.ContainsKey(s6xScalar.UniqueAddress))
                    {
                        existingObject = (S6xScalar)slScalars[s6xScalar.UniqueAddress];
                        s6xScalar.DateUpdated = DateTime.UtcNow;
                        s6xScalar.DateCreated = existingObject.DateCreated;
                        s6xScalar.IdentificationDetails = existingObject.IdentificationDetails;
                        s6xScalar.IdentificationStatus = existingObject.IdentificationStatus;
                        s6xScalar.OutputComments = existingObject.OutputComments;
                        s6xScalar.InlineComments = existingObject.InlineComments;

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
                        existingObject = (S6xScalar)slDupScalars[s6xScalar.DuplicateAddress];
                        s6xScalar.DateUpdated = DateTime.UtcNow;
                        s6xScalar.DateCreated = existingObject.DateCreated;
                        s6xScalar.IdentificationDetails = existingObject.IdentificationDetails;
                        s6xScalar.IdentificationStatus = existingObject.IdentificationStatus;
                        s6xScalar.OutputComments = existingObject.OutputComments;
                        s6xScalar.InlineComments = existingObject.InlineComments;

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

            if (xlsFile.PayLoads != null)
            {
                foreach (XlsPayLoad xlsObject in xlsFile.PayLoads.Values)
                {
                    S6xRegister s6xObject = new S6xRegister(xlsObject);
                    
                    // Ignored
                    if (s6xObject.AddressInt < 0 || (s6xObject.AddressInt >= 0x2000 && s6xObject.AddressInt < 0xf000) || slProcessed.ContainsKey(s6xObject.UniqueAddress))
                    {
                        if (!alIgnored.Contains(s6xObject.UniqueAddress)) alIgnored.Add(s6xObject.UniqueAddress);
                        continue;
                    }
                    alRegisters.Add(s6xObject);
                    s6xObject = null;
                }
            }

            // Real Import
            foreach (S6xRegister s6xRegister in alRegisters)
            {
                S6xRegister existingObject = null;

                // Main Item
                if (slRegisters.ContainsKey(s6xRegister.UniqueAddress))
                {
                    existingObject = (S6xRegister)slRegisters[s6xRegister.UniqueAddress];
                    s6xRegister.DateUpdated = DateTime.UtcNow;
                    s6xRegister.DateCreated = existingObject.DateCreated;
                    s6xRegister.IdentificationDetails = existingObject.IdentificationDetails;
                    s6xRegister.IdentificationStatus = existingObject.IdentificationStatus;
                    
                    s6xRegister.AutoConstValue = existingObject.AutoConstValue;
                    s6xRegister.isRBase = existingObject.isRBase;
                    s6xRegister.isRConst = existingObject.isRConst;
                    s6xRegister.ConstValue = existingObject.ConstValue;
                    s6xRegister.ByteLabel = existingObject.ByteLabel;
                    s6xRegister.WordLabel = existingObject.WordLabel;
                    s6xRegister.Category = existingObject.Category;
                    s6xRegister.Category2 = existingObject.Category2;
                    s6xRegister.Category3 = existingObject.Category3;

                    slRegisters[s6xRegister.UniqueAddress] = s6xRegister;
                    alS6xUpdates.Add(s6xRegister.UniqueAddress);
                }
                else
                {
                    slRegisters.Add(s6xRegister.UniqueAddress, s6xRegister);
                    alS6xCreations.Add(s6xRegister.UniqueAddress);
                }
            }
            alRegisters = null;

            ArrayList alConflicts = new ArrayList();
            alConflicts.AddRange(alTablesConflicts);
            alConflicts.AddRange(alFunctionsConflicts);
            alConflicts.AddRange(alScalarsConflicts);
            alConflicts.AddRange(alRegistersConflicts);

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
            alRegistersConflicts = null;
            slProcessed = null;
            alIgnored = null;
            alS6xDupUpdates = null;
            alS6xDupCreations = null;

            isSaved = false;

            GC.Collect();

            return arrResult;
        }

        public object[] writeToFileObject(ref XlsFile xlsFile, ref SADBin sadBin)
        {
            ArrayList alS6xUpdates = new ArrayList();
            ArrayList alS6xCreations = new ArrayList();
            ArrayList alDuplicates = new ArrayList();
            ArrayList alS6xDupUpdates = new ArrayList();
            SortedList slProcessed = new SortedList();
            ArrayList alXlsObjects = null;
            ArrayList alS6xDupUniqueAddresses = null;
            ArrayList alS6xScalersAddresses = null;
            string uniqueAddress = string.Empty;
            int xlsBaseOffset = 0;

            bool mngtAutoDetectedTables = false;
            bool mngtAutoDetectedFunctions = false;
            bool mngtAutoDetectedScalars = false;
            bool mngtAutoDetectedRegisters = false;

            //mngtAutoDetectedTables = lstSettings.Get<bool>("AUTODETECT_MNGT_TABLES", mngtAutoDetectedTables);
            //mngtAutoDetectedFunctions = lstSettings.Get<bool>("AUTODETECT_MNGT_FUNCTIONS", mngtAutoDetectedFunctions);
            //mngtAutoDetectedScalars = lstSettings.Get<bool>("AUTODETECT_MNGT_SCALARS", mngtAutoDetectedScalars);

            // 0 S6x Updates
            // 1 S6x Creations
            // 2 Duplicates
            // 3 Conflicts  - No Mngt needed for writeToFileObject
            // 4 Ignored  - No Mngt needed for writeToFileObject
            // 5 S6x Duplicates Updates
            // 6 S6x Duplicates Creations  - No Mngt needed for writeToFileObject
            object[] arrResult = new object[7];

            xlsBaseOffset = Properties.XdfBaseOffsetInt;
            
            // Updating xlsFile.Levels based on S6x elements
            SortedList<string, string[]> slS6xCategoriesByName = getAllElementsCategories();
            int lastIndex = 0;
            foreach (XlsLevel xlsLevel in xlsFile.Levels.Values)
            {
                int index = -1;
                if (xlsLevel.Value == null) continue;
                if (xlsLevel.Value.Contains("."))
                {
                    try { index = Convert.ToInt32(xlsLevel.Value.Split('.')[0], 16); }
                    catch { continue; }
                }
                else
                {
                    try { index = Convert.ToInt32(xlsLevel.Value, 16); }
                    catch { continue; }
                }

                if (slS6xCategoriesByName.ContainsKey(xlsLevel.Label.ToUpper())) slS6xCategoriesByName[xlsLevel.Label.ToUpper()][1] = xlsLevel.Value;
                else slS6xCategoriesByName.Add(xlsLevel.Label.ToUpper(), new string[] { xlsLevel.Label, xlsLevel.Value });
                if (index > lastIndex) lastIndex = index;
            }

            SortedList<string, string> slS6xCategoriesByIndex = new SortedList<string, string>();
            foreach (string key in slS6xCategoriesByName.Keys)
            {
                string[] s6xCategory = slS6xCategoriesByName[key];
                if (s6xCategory[1] == string.Empty)
                {
                    lastIndex++;
                    s6xCategory[1] = lastIndex.ToString();
                }
                if (!slS6xCategoriesByIndex.ContainsKey(s6xCategory[1])) slS6xCategoriesByIndex.Add(s6xCategory[1], s6xCategory[0]);
            }
            slS6xCategoriesByName = null;

            xlsFile.Levels = new SortedList<string, XlsLevel>();
            for (int iIndex = 0; iIndex < slS6xCategoriesByIndex.Count; iIndex++) xlsFile.Levels.Add(slS6xCategoriesByIndex.Keys[iIndex], new XlsLevel() { Value = slS6xCategoriesByIndex.Keys[iIndex], Label = slS6xCategoriesByIndex.Values[iIndex] });
            slS6xCategoriesByIndex = null;

            // Existing S6x Functions to Prepare Scalers
            if (xlsFile.Functions != null)
            {
                foreach (XlsFunction xlsObject in xlsFile.Functions.Values)
                {
                    uniqueAddress = Tools.UniqueAddress(sadBin.Calibration.BankNum, Tools.binAddressFromXlsAddress(xlsObject.AddressInt, Properties.XdfBaseOffsetInt), sadBin.Calibration.BankAddressBinInt);
                    if (!slFunctions.ContainsKey(uniqueAddress)) continue;

                    if (((S6xFunction)slFunctions[uniqueAddress]).ShortLabel == xlsObject.PID)
                    {
                        // Matching on ShortLabel/PID
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
                        xlsObject.Import((S6xFunction)slFunctions[uniqueAddress], xlsBaseOffset, xlsFile.Levels);
                        continue;
                    }

                    // Real Duplicates
                    foreach (S6xFunction s6xDup in slDupFunctions)
                    {
                        if (uniqueAddress != s6xDup.UniqueAddress) continue;

                        if (((S6xFunction)slDupFunctions[s6xDup.DuplicateAddress]).ShortLabel == xlsObject.PID)
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

                        xlsObject.Import((S6xFunction)slDupFunctions[s6xDup.DuplicateAddress], xlsBaseOffset, xlsFile.Levels);

                        break;
                    }
                }
            }
            // Missing Xdf Functions Creation to Prepare Scalers
            alXlsObjects = new ArrayList();
            if (xlsFile.Functions != null) foreach (XlsFunction xlsObject in xlsFile.Functions.Values) alXlsObjects.Add(xlsObject);

            // To be sure to export Scalers
            alS6xScalersAddresses = new ArrayList();
            alS6xDupUniqueAddresses = new ArrayList();
            foreach (S6xTable s6xObject in slDupTables.Values) if (!alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress)) alS6xDupUniqueAddresses.Add(s6xObject.UniqueAddress);

            foreach (S6xTable s6xObject in slTables.Values)
            {
                if (!s6xObject.Skip && (s6xObject.Store))
                {
                    if (s6xObject.ColsScalerAddress != null && s6xObject.ColsScalerAddress != string.Empty)
                    {
                        if (!alS6xScalersAddresses.Contains(s6xObject.ColsScalerAddress)) alS6xScalersAddresses.Add(s6xObject.ColsScalerAddress);
                    }
                    if (s6xObject.RowsScalerAddress != null && s6xObject.RowsScalerAddress != string.Empty)
                    {
                        if (!alS6xScalersAddresses.Contains(s6xObject.RowsScalerAddress)) alS6xScalersAddresses.Add(s6xObject.RowsScalerAddress);
                    }
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xTable s6xDupObject in slDupTables.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && (s6xDupObject.Store))
                        {
                            if (s6xDupObject.ColsScalerAddress != null && s6xDupObject.ColsScalerAddress != string.Empty)
                            {
                                if (!alS6xScalersAddresses.Contains(s6xDupObject.ColsScalerAddress)) alS6xScalersAddresses.Add(s6xDupObject.ColsScalerAddress);
                            }
                            if (s6xDupObject.RowsScalerAddress != null && s6xDupObject.RowsScalerAddress != string.Empty)
                            {
                                if (!alS6xScalersAddresses.Contains(s6xDupObject.RowsScalerAddress)) alS6xScalersAddresses.Add(s6xDupObject.RowsScalerAddress);
                            }
                        }
                    }
                }
            }

            alS6xDupUniqueAddresses = new ArrayList();
            foreach (S6xFunction s6xObject in slDupFunctions.Values) if (!alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress)) alS6xDupUniqueAddresses.Add(s6xObject.UniqueAddress);

            foreach (S6xFunction s6xObject in slFunctions.Values)
            {
                if (!s6xObject.Skip && (s6xObject.Store || alS6xScalersAddresses.Contains(s6xObject.UniqueAddress)) && !alS6xUpdates.Contains(s6xObject.UniqueAddress) && !alS6xCreations.Contains(s6xObject.UniqueAddress) && s6xObject.AddressBinInt >= xlsBaseOffset)
                {
                    XlsFunction xlsObject = new XlsFunction();
                    xlsObject.Import(s6xObject, xlsBaseOffset, xlsFile.Levels);
                    alXlsObjects.Add(xlsObject);
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xFunction s6xDupObject in slDupFunctions.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && (s6xDupObject.Store || mngtAutoDetectedFunctions || alS6xScalersAddresses.Contains(s6xObject.DuplicateAddress)) && !alS6xDupUpdates.Contains(s6xDupObject.DuplicateAddress) && s6xDupObject.AddressBinInt >= xlsBaseOffset)
                        {
                            XlsFunction xlsObject = new XlsFunction();
                            xlsObject.Import(s6xDupObject, xlsBaseOffset, xlsFile.Levels);
                            alXlsObjects.Add(xlsObject);
                        }
                    }
                }
            }
            xlsFile.Functions = new SortedList<string, XlsFunction>();
            foreach (XlsFunction xlsObject in alXlsObjects) if (!xlsFile.Functions.ContainsKey(xlsObject.PID)) xlsFile.Functions.Add(xlsObject.PID, xlsObject);
            alXlsObjects = null;
            alS6xDupUniqueAddresses = null;

            // Xdf other Objects Update from S6x other Objects
            if (xlsFile.Tables != null)
            {
                foreach (XlsTable xlsObject in xlsFile.Tables.Values)
                {
                    uniqueAddress = Tools.UniqueAddress(sadBin.Calibration.BankNum, Tools.binAddressFromXlsAddress(xlsObject.AddressInt, xlsBaseOffset), sadBin.Calibration.BankAddressBinInt);
                    if (!slTables.ContainsKey(uniqueAddress)) continue;

                    if (((S6xTable)slTables[uniqueAddress]).ShortLabel == xlsObject.PID)
                    {
                        // Matching on ShortLabel/PID
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
                        // Trying to find Scalers
                        S6xTable s6xTable = (S6xTable)slTables[uniqueAddress];
                        S6xFunction s6xFunction = null;
                        if (s6xTable.ColsScalerAddress != null && s6xTable.ColsScalerAddress != string.Empty)
                        {
                            xlsObject.XFunction = null;
                            s6xFunction = (S6xFunction)slFunctions[s6xTable.ColsScalerAddress];
                            if (s6xFunction == null) s6xFunction = (S6xFunction)slDupFunctions[s6xTable.ColsScalerAddress];
                            if (s6xFunction != null)
                            {
                                foreach (XlsFunction xlsFunction in xlsFile.Functions.Values)
                                {
                                    if (s6xFunction.ShortLabel == xlsFunction.PID)
                                    {
                                        xlsObject.XFunction = xlsFunction;
                                        break;
                                    }
                                }
                            }
                        }
                        if (s6xTable.RowsScalerAddress != null && s6xTable.RowsScalerAddress != string.Empty)
                        {
                            xlsObject.YFunction = null;
                            s6xFunction = (S6xFunction)slFunctions[s6xTable.RowsScalerAddress];
                            if (s6xFunction == null) s6xFunction = (S6xFunction)slDupFunctions[s6xTable.RowsScalerAddress];
                            if (s6xFunction != null)
                            {
                                foreach (XlsFunction xlsFunction in xlsFile.Functions.Values)
                                {
                                    if (s6xFunction.ShortLabel == xlsFunction.PID)
                                    {
                                        xlsObject.YFunction = xlsFunction;
                                        break;
                                    }
                                }
                            }
                        }

                        xlsObject.Import(s6xTable, xlsBaseOffset, xlsFile.Levels);
                        s6xTable = null;
                        continue;
                    }

                    // Real Duplicates
                    foreach (S6xTable s6xDup in slDupTables.Values)
                    {
                        if (uniqueAddress != s6xDup.UniqueAddress) continue;

                        if (((S6xTable)slDupTables[s6xDup.DuplicateAddress]).ShortLabel == xlsObject.PID)
                        {
                            // Matching on ShortLabel/PID
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

                        // Trying to find Scalers
                        S6xTable s6xTable = (S6xTable)slDupTables[s6xDup.DuplicateAddress];
                        S6xFunction s6xFunction = null;
                        if (s6xTable.ColsScalerAddress != null && s6xTable.ColsScalerAddress != string.Empty)
                        {
                            xlsObject.XFunction = null;
                            s6xFunction = (S6xFunction)slFunctions[s6xTable.ColsScalerAddress];
                            if (s6xFunction == null) s6xFunction = (S6xFunction)slDupFunctions[s6xTable.ColsScalerAddress];
                            if (s6xFunction != null)
                            {
                                foreach (XlsFunction xlsFunction in xlsFile.Functions.Values)
                                {
                                    if (s6xFunction.ShortLabel == xlsFunction.PID)
                                    {
                                        xlsObject.XFunction = xlsFunction;
                                        break;
                                    }
                                }
                            }
                        }
                        if (s6xTable.RowsScalerAddress != null && s6xTable.RowsScalerAddress != string.Empty)
                        {
                            xlsObject.YFunction = null;
                            s6xFunction = (S6xFunction)slFunctions[s6xTable.RowsScalerAddress];
                            if (s6xFunction == null) s6xFunction = (S6xFunction)slDupFunctions[s6xTable.RowsScalerAddress];
                            if (s6xFunction != null)
                            {
                                foreach (XlsFunction xlsFunction in xlsFile.Functions.Values)
                                {
                                    if (s6xFunction.ShortLabel == xlsFunction.PID)
                                    {
                                        xlsObject.YFunction = xlsFunction;
                                        break;
                                    }
                                }
                            }
                        }

                        xlsObject.Import(s6xTable, xlsBaseOffset, xlsFile.Levels);
                        s6xTable = null;

                        break;
                    }
                }
            }

            if (xlsFile.Scalars != null)
            {
                foreach (XlsScalar xlsObject in xlsFile.Scalars.Values)
                {
                    uniqueAddress = Tools.UniqueAddress(sadBin.Calibration.BankNum, Tools.binAddressFromXlsAddress(xlsObject.AddressInt, xlsBaseOffset), sadBin.Calibration.BankAddressBinInt);
                    if (!slScalars.ContainsKey(uniqueAddress)) continue;

                    if (((S6xScalar)slScalars[uniqueAddress]).ShortLabel == xlsObject.PID)
                    {
                        // Matching on ShortLabel/PID
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
                        xlsObject.Import((S6xScalar)slScalars[uniqueAddress], xlsBaseOffset, xlsFile.Levels);
                        continue;
                    }

                    // Real Duplicates
                    foreach (S6xScalar s6xDup in slDupScalars.Values)
                    {
                        if (uniqueAddress != s6xDup.UniqueAddress) continue;

                        if (((S6xScalar)slDupScalars[s6xDup.DuplicateAddress]).ShortLabel == xlsObject.PID)
                        {
                            // Matching on ShortLabel/PID
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

                        xlsObject.Import((S6xScalar)slDupScalars[s6xDup.DuplicateAddress], xlsBaseOffset, xlsFile.Levels);

                        break;
                    }
                }
            }

            if (xlsFile.PayLoads != null)
            {
                foreach (XlsPayLoad xlsObject in xlsFile.PayLoads.Values)
                {
                    uniqueAddress = Tools.RegisterUniqueAddress(xlsObject.AddressInt);
                    if (!slRegisters.ContainsKey(uniqueAddress)) continue;

                    if (slProcessed.ContainsKey(uniqueAddress)) continue;

                    slProcessed.Add(uniqueAddress, 1);

                    xlsObject.Import((S6xRegister)slRegisters[uniqueAddress]);
                }
            }

            // Missing Xls other Objects Creation
            alXlsObjects = new ArrayList();
            if (xlsFile.Tables != null) foreach (XlsTable xlsObject in xlsFile.Tables.Values) alXlsObjects.Add(xlsObject);

            alS6xDupUniqueAddresses = new ArrayList();
            foreach (S6xTable s6xObject in slDupTables.Values) if (!alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress)) alS6xDupUniqueAddresses.Add(s6xObject.UniqueAddress);

            foreach (S6xTable s6xObject in slTables.Values)
            {
                if (!s6xObject.Skip && (s6xObject.Store || mngtAutoDetectedTables) && !alS6xUpdates.Contains(s6xObject.UniqueAddress) && !alS6xCreations.Contains(s6xObject.UniqueAddress) && s6xObject.AddressBinInt >= xlsBaseOffset)
                {
                    XlsTable xlsObject = new XlsTable();
                    
                    // Trying to find Scalers
                    S6xFunction s6xFunction = null;
                    if (s6xObject.ColsScalerAddress != null && s6xObject.ColsScalerAddress != string.Empty)
                    {
                        xlsObject.XFunction = null;
                        s6xFunction = (S6xFunction)slFunctions[s6xObject.ColsScalerAddress];
                        if (s6xFunction == null) s6xFunction = (S6xFunction)slDupFunctions[s6xObject.ColsScalerAddress];
                        if (s6xFunction != null)
                        {
                            foreach (XlsFunction xlsFunction in xlsFile.Functions.Values)
                            {
                                if (s6xFunction.ShortLabel == xlsFunction.PID)
                                {
                                    xlsObject.XFunction = xlsFunction;
                                    break;
                                }
                            }
                        }
                    }
                    if (s6xObject.RowsScalerAddress != null && s6xObject.RowsScalerAddress != string.Empty)
                    {
                        xlsObject.YFunction = null;
                        s6xFunction = (S6xFunction)slFunctions[s6xObject.RowsScalerAddress];
                        if (s6xFunction == null) s6xFunction = (S6xFunction)slDupFunctions[s6xObject.RowsScalerAddress];
                        if (s6xFunction != null)
                        {
                            foreach (XlsFunction xlsFunction in xlsFile.Functions.Values)
                            {
                                if (s6xFunction.ShortLabel == xlsFunction.PID)
                                {
                                    xlsObject.YFunction = xlsFunction;
                                    break;
                                }
                            }
                        }
                    }

                    xlsObject.Import(s6xObject, xlsBaseOffset, xlsFile.Levels);
                    alXlsObjects.Add(xlsObject);
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xTable s6xDupObject in slDupTables.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && (s6xDupObject.Store || mngtAutoDetectedTables) && !alS6xDupUpdates.Contains(s6xDupObject.DuplicateAddress) && s6xDupObject.AddressBinInt >= xlsBaseOffset)
                        {
                            XlsTable xlsObject = new XlsTable();

                            // Trying to find Scalers
                            S6xFunction s6xFunction = null;
                            if (s6xDupObject.ColsScalerAddress != null && s6xDupObject.ColsScalerAddress != string.Empty)
                            {
                                xlsObject.XFunction = null;
                                s6xFunction = (S6xFunction)slFunctions[s6xDupObject.ColsScalerAddress];
                                if (s6xFunction == null) s6xFunction = (S6xFunction)slDupFunctions[s6xDupObject.ColsScalerAddress];
                                if (s6xFunction != null)
                                {
                                    foreach (XlsFunction xlsFunction in xlsFile.Functions.Values)
                                    {
                                        if (s6xFunction.ShortLabel == xlsFunction.PID)
                                        {
                                            xlsObject.XFunction = xlsFunction;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (s6xDupObject.RowsScalerAddress != null && s6xDupObject.RowsScalerAddress != string.Empty)
                            {
                                xlsObject.YFunction = null;
                                s6xFunction = (S6xFunction)slFunctions[s6xDupObject.RowsScalerAddress];
                                if (s6xFunction == null) s6xFunction = (S6xFunction)slDupFunctions[s6xDupObject.RowsScalerAddress];
                                if (s6xFunction != null)
                                {
                                    foreach (XlsFunction xlsFunction in xlsFile.Functions.Values)
                                    {
                                        if (s6xFunction.ShortLabel == xlsFunction.PID)
                                        {
                                            xlsObject.YFunction = xlsFunction;
                                            break;
                                        }
                                    }
                                }
                            }

                            xlsObject.Import(s6xDupObject, xlsBaseOffset, xlsFile.Levels);
                            alXlsObjects.Add(xlsObject);
                        }
                    }
                }
            }
            xlsFile.Tables = new SortedList<string, XlsTable>();
            foreach (XlsTable xlsObject in alXlsObjects) if (!xlsFile.Tables.ContainsKey(xlsObject.PID)) xlsFile.Tables.Add(xlsObject.PID, xlsObject);
            alXlsObjects = null;
            alS6xDupUniqueAddresses = null;

            alXlsObjects = new ArrayList();
            if (xlsFile.Scalars != null) foreach (XlsScalar xlsObject in xlsFile.Scalars.Values) alXlsObjects.Add(xlsObject);

            alS6xDupUniqueAddresses = new ArrayList();
            foreach (S6xScalar s6xObject in slDupScalars.Values) if (!alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress)) alS6xDupUniqueAddresses.Add(s6xObject.UniqueAddress);

            foreach (S6xScalar s6xObject in slScalars.Values)
            {
                if (!s6xObject.Skip && (s6xObject.Store || mngtAutoDetectedScalars) && !alS6xUpdates.Contains(s6xObject.UniqueAddress) && !alS6xCreations.Contains(s6xObject.UniqueAddress) && s6xObject.AddressBinInt >= xlsBaseOffset)
                {
                    XlsScalar xlsObject = new XlsScalar();
                    xlsObject.Import(s6xObject, xlsBaseOffset, xlsFile.Levels);
                    alXlsObjects.Add(xlsObject);
                }

                if (alS6xDupUniqueAddresses.Contains(s6xObject.UniqueAddress))
                {
                    foreach (S6xScalar s6xDupObject in slDupScalars.Values)
                    {
                        if (s6xDupObject.UniqueAddress != s6xObject.UniqueAddress) continue;
                        if (!s6xDupObject.Skip && (s6xDupObject.Store || mngtAutoDetectedScalars) && !alS6xDupUpdates.Contains(s6xDupObject.DuplicateAddress) && s6xDupObject.AddressBinInt >= xlsBaseOffset)
                        {
                            XlsScalar xlsObject = new XlsScalar();
                            xlsObject.Import(s6xDupObject, xlsBaseOffset, xlsFile.Levels);
                            alXlsObjects.Add(xlsObject);
                        }
                    }
                }
            }
            xlsFile.Scalars = new SortedList<string, XlsScalar>();
            foreach (XlsScalar xlsObject in alXlsObjects) if (!xlsFile.Scalars.ContainsKey(xlsObject.PID)) xlsFile.Scalars.Add(xlsObject.PID, xlsObject);
            alXlsObjects = null;
            alS6xDupUniqueAddresses = null;

            alXlsObjects = new ArrayList();
            if (xlsFile.PayLoads != null) foreach (XlsPayLoad xlsObject in xlsFile.PayLoads.Values) alXlsObjects.Add(xlsObject);

            foreach (S6xRegister s6xObject in slRegisters.Values)
            {
                if (!s6xObject.Skip && !s6xObject.isDual && (s6xObject.Store || mngtAutoDetectedRegisters) && !alS6xUpdates.Contains(s6xObject.UniqueAddress) && !alS6xCreations.Contains(s6xObject.UniqueAddress))
                {
                    XlsPayLoad xlsObject = new XlsPayLoad();
                    xlsObject.Import(s6xObject);
                    alXlsObjects.Add(xlsObject);
                }
            }
            xlsFile.PayLoads = new SortedList<int, XlsPayLoad>();
            foreach (XlsPayLoad xlsObject in alXlsObjects) if (!xlsFile.PayLoads.ContainsKey(xlsObject.AddressInt)) xlsFile.PayLoads.Add(xlsObject.AddressInt, xlsObject);
            alXlsObjects = null;

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
        [XmlAttribute]
        public int CellsScalePrecision { get; set; }

        [XmlAttribute]
        public string CellsMin { get; set; }
        [XmlAttribute]
        public string CellsMax { get; set; }
        
        public string ColsScalerAddress { get; set; }
        public string RowsScalerAddress { get; set; }

        public string ColsScalerXdfUniqueId { get; set; }
        public string RowsScalerXdfUniqueId { get; set; }

        public string ColsUnits { get; set; }
        public string RowsUnits { get; set; }
        public string CellsUnits { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        
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
            CellsScalePrecision = SADDef.DefaultScalePrecision;
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
            if (tableCalElem.TableElem.Comments == string.Empty) Comments = tableCalElem.TableElem.FullLabel;
            else Comments = tableCalElem.TableElem.Comments;
            ColsScalerAddress = tableCalElem.TableElem.ColsScalerUniqueAddress;  // From Forced Values at lower level
            RowsScalerAddress = tableCalElem.TableElem.RowsScalerUniqueAddress;  // From Forced Values at lower level
            CellsScaleExpression = tableCalElem.TableElem.CellsScaleExpression;  // From Forced Values at lower level
            CellsScalePrecision = tableCalElem.TableElem.CellsScalePrecision;
            ColsUnits = tableCalElem.TableElem.ColsUnits;
            RowsUnits = tableCalElem.TableElem.RowsUnits;
            CellsUnits = tableCalElem.TableElem.CellsUnits;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

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

                    OutputComments = tableCalElem.TableElem.S6xElementSignatureSource.Table.OutputComments;

                    CellsMin = tableCalElem.TableElem.S6xElementSignatureSource.Table.CellsMin;
                    CellsMax = tableCalElem.TableElem.S6xElementSignatureSource.Table.CellsMax;

                    IdentificationStatus = tableCalElem.TableElem.S6xElementSignatureSource.Table.IdentificationStatus;
                    IdentificationDetails = tableCalElem.TableElem.S6xElementSignatureSource.Table.IdentificationDetails;
                    Category = tableCalElem.TableElem.S6xElementSignatureSource.Table.Category;
                    Category2 = tableCalElem.TableElem.S6xElementSignatureSource.Table.Category2;
                    Category3 = tableCalElem.TableElem.S6xElementSignatureSource.Table.Category3;
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
            Comments = table.Comments;
            ColsScalerAddress = table.ColsScalerUniqueAddress;  // From Forced Values at lower level
            RowsScalerAddress = table.RowsScalerUniqueAddress;  // From Forced Values at lower level
            CellsScaleExpression = table.CellsScaleExpression;  // From Forced Values at lower level
            CellsScalePrecision = table.CellsScalePrecision;
            ColsUnits = table.ColsUnits;
            RowsUnits = table.RowsUnits;
            CellsUnits = table.CellsUnits;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

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

                    OutputComments = table.S6xElementSignatureSource.Table.OutputComments;

                    CellsMin = table.S6xElementSignatureSource.Table.CellsMin;
                    CellsMax = table.S6xElementSignatureSource.Table.CellsMax;

                    IdentificationStatus = table.S6xElementSignatureSource.Table.IdentificationStatus;
                    IdentificationDetails = table.S6xElementSignatureSource.Table.IdentificationDetails;
                    Category = table.S6xElementSignatureSource.Table.Category;
                    Category2 = table.S6xElementSignatureSource.Table.Category2;
                    Category3 = table.S6xElementSignatureSource.Table.Category3;
                }
            }
        }

        public S6xTable(XdfTable xdfTable, int bankNum, int address, int addressBin, bool isCalElem, ref SortedList slFunctions, XdfHeaderCategory[] xdfHeaderCategories)
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

            CellsScalePrecision = SADDef.DefaultScalePrecision;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;
            string[] arrCategories = Tools.S6xElementCategories(xdfHeaderCategories, xdfTable.xdfCategories);
            if (arrCategories != null)
            {
                if (arrCategories.Length > 0) Category = arrCategories[0];
                if (arrCategories.Length > 1) Category2 = arrCategories[1];
                if (arrCategories.Length > 2) Category3 = arrCategories[2];
            }
            arrCategories = null;

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
                        CellsScalePrecision = SADDef.DefaultScalePrecision;
                        if (xdfAxis.decimalpl != null)
                        {
                            try { CellsScalePrecision = Convert.ToInt32(xdfAxis.decimalpl); }
                            catch { }
                            if (CellsScalePrecision < SADDef.DefaultScaleMinPrecision) CellsScalePrecision = SADDef.DefaultScaleMinPrecision;
                            if (CellsScalePrecision > SADDef.DefaultScaleMaxPrecision) CellsScalePrecision = SADDef.DefaultScaleMaxPrecision;
                        }

                        CellsUnits = xdfAxis.units;

                        CellsMin = xdfAxis.min;
                        CellsMax = xdfAxis.max;

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

        public S6xTable(XlsTable xlsTable, int bankNum, int address, int addressBin, bool isCalElem, ref SortedList slFunctions)
        {
            BankNum = bankNum;
            AddressInt = address;
            AddressBinInt = addressBin;
            isCalibrationElement = isCalElem;
            Store = true;
            Skip = false;
            Label = xlsTable.Title;
            Comments = xlsTable.Comments;
            ShortLabel = xlsTable.PID;

            ColsScalerAddress = string.Empty;
            RowsScalerAddress = string.Empty;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

            if (xlsTable.Level != null) Category = xlsTable.Level.Label;
            if (xlsTable.Level2 != null) Category2 = xlsTable.Level2.Label;

            ColsNumber = xlsTable.Cols;
            RowsNumber = xlsTable.Rows;
            WordOutput = xlsTable.Bytes == 2;
            SignedOutput = xlsTable.Signed;

            CellsScaleExpression = xlsTable.Equation;
            CellsScalePrecision = xlsTable.Digits;
            if (CellsScalePrecision < SADDef.DefaultScaleMinPrecision) CellsScalePrecision = SADDef.DefaultScaleMinPrecision;
            if (CellsScalePrecision > SADDef.DefaultScaleMaxPrecision) CellsScalePrecision = SADDef.DefaultScaleMaxPrecision;
            CellsUnits = xlsTable.Units;

            CellsMin = xlsTable.Min;
            CellsMax = xlsTable.Max;

            ColsUnits = xlsTable.XUnits;
            if (xlsTable.XFunction != null)
            {
                foreach (S6xFunction s6xFunction in slFunctions.Values)
                {
                    if (s6xFunction.ShortLabel == xlsTable.XFunction.PID)
                    {
                        ColsScalerAddress = s6xFunction.UniqueAddress;
                        if (ColsUnits == null || ColsUnits == string.Empty) ColsUnits = s6xFunction.InputUnits;
                        break;
                    }
                }
            }

            RowsUnits = xlsTable.YUnits;
            if (xlsTable.YFunction != null)
            {
                foreach (S6xFunction s6xFunction in slFunctions.Values)
                {
                    if (s6xFunction.ShortLabel == xlsTable.YFunction.PID)
                    {
                        RowsScalerAddress = s6xFunction.UniqueAddress;
                        if (RowsUnits == null || RowsUnits == string.Empty) RowsUnits = s6xFunction.InputUnits;
                        break;
                    }
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
            clone.CellsScalePrecision = CellsScalePrecision;

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

            clone.CellsMin = CellsMin;
            clone.CellsMax = CellsMax;

            clone.Information = Information;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

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
                if (CellsScalePrecision != SADDef.DefaultScalePrecision) return true;

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

        [XmlAttribute]
        public int InputScalePrecision { get; set; }
        [XmlAttribute]
        public int OutputScalePrecision { get; set; }

        public string InputUnits { get; set; }
        public string OutputUnits { get; set; }

        [XmlAttribute]
        public string InputMin { get; set; }
        [XmlAttribute]
        public string InputMax { get; set; }
        [XmlAttribute]
        public string OutputMin { get; set; }
        [XmlAttribute]
        public string OutputMax { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

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
            InputScalePrecision = SADDef.DefaultScalePrecision;
            OutputScalePrecision = SADDef.DefaultScalePrecision;
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
            if (functionCalElem.FunctionElem.Comments == string.Empty) Comments = functionCalElem.FunctionElem.FullLabel;
            else Comments = functionCalElem.FunctionElem.Comments;
            InputScaleExpression = functionCalElem.FunctionElem.InputScaleExpression;       // From Forced Values at lower level
            OutputScaleExpression = functionCalElem.FunctionElem.OutputScaleExpression;     // From Forced Values at lower level
            InputScalePrecision = functionCalElem.FunctionElem.InputScalePrecision;
            OutputScalePrecision = functionCalElem.FunctionElem.OutputScalePrecision;
            InputUnits = functionCalElem.FunctionElem.InputUnits;
            OutputUnits = functionCalElem.FunctionElem.OutputUnits;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

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

                    OutputComments = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.OutputComments;

                    InputMin = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.InputMin;
                    InputMax = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.InputMax;
                    OutputMin = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.OutputMin;
                    OutputMax = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.OutputMax;

                    IdentificationStatus = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.IdentificationStatus;
                    IdentificationDetails = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.IdentificationDetails;
                    Category = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.Category;
                    Category2 = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.Category2;
                    Category3 = functionCalElem.FunctionElem.S6xElementSignatureSource.Function.Category3;
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
            Comments = function.Comments;
            InputScaleExpression = function.InputScaleExpression;         // From Forced Values at lower level
            OutputScaleExpression = function.OutputScaleExpression;       // From Forced Values at lower level
            InputScalePrecision = function.InputScalePrecision;
            OutputScalePrecision = function.OutputScalePrecision;
            InputUnits = function.InputUnits;
            OutputUnits = function.OutputUnits;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

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

                    OutputComments = function.S6xElementSignatureSource.Function.OutputComments;

                    InputMin = function.S6xElementSignatureSource.Function.InputMin;
                    InputMax = function.S6xElementSignatureSource.Function.InputMax;
                    OutputMin = function.S6xElementSignatureSource.Function.OutputMin;
                    OutputMax = function.S6xElementSignatureSource.Function.OutputMax;

                    IdentificationStatus = function.S6xElementSignatureSource.Function.IdentificationStatus;
                    IdentificationDetails = function.S6xElementSignatureSource.Function.IdentificationDetails;
                    Category = function.S6xElementSignatureSource.Function.Category;
                    Category2 = function.S6xElementSignatureSource.Function.Category2;
                    Category3 = function.S6xElementSignatureSource.Function.Category3;
                }
            }
        }

        public S6xFunction(XdfFunction xdfFunction, int bankNum, int address, int addressBin, bool isCalElem, XdfHeaderCategory[] xdfHeaderCategories)
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
            InputScalePrecision = SADDef.DefaultScalePrecision;
            OutputScalePrecision = SADDef.DefaultScalePrecision;
            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

            string[] arrCategories = Tools.S6xElementCategories(xdfHeaderCategories, xdfFunction.xdfCategories);
            if (arrCategories != null)
            {
                if (arrCategories.Length > 0) Category = arrCategories[0];
                if (arrCategories.Length > 1) Category2 = arrCategories[1];
                if (arrCategories.Length > 2) Category3 = arrCategories[2];
            }
            arrCategories = null;

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
                    InputScalePrecision = SADDef.DefaultScalePrecision;
                    if (xdfAxis.decimalpl != null)
                    {
                        try { InputScalePrecision = Convert.ToInt32(xdfAxis.decimalpl); }
                        catch {}
                        if (InputScalePrecision < SADDef.DefaultScaleMinPrecision) InputScalePrecision = SADDef.DefaultScaleMinPrecision;
                        if (InputScalePrecision > SADDef.DefaultScaleMaxPrecision) InputScalePrecision = SADDef.DefaultScaleMaxPrecision;
                    }
                    InputUnits = xdfAxis.units;

                    InputMin = xdfAxis.min;
                    InputMax = xdfAxis.max;
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
                    if (xdfAxis.decimalpl != null)
                    {
                        try { OutputScalePrecision = Convert.ToInt32(xdfAxis.decimalpl); }
                        catch { }
                        if (OutputScalePrecision < SADDef.DefaultScaleMinPrecision) OutputScalePrecision = SADDef.DefaultScaleMinPrecision;
                        if (OutputScalePrecision > SADDef.DefaultScaleMaxPrecision) OutputScalePrecision = SADDef.DefaultScaleMaxPrecision;
                    }
                    OutputUnits = xdfAxis.units;

                    OutputMin = xdfAxis.min;
                    OutputMax = xdfAxis.max;
                }
            }
        }

        public S6xFunction(XlsFunction xlsFunction, int bankNum, int address, int addressBin, bool isCalElem)
        {
            BankNum = bankNum;
            AddressInt = address;
            AddressBinInt = addressBin;
            isCalibrationElement = isCalElem;
            Store = true;
            Skip = false;
            Label = xlsFunction.Title;
            Comments = xlsFunction.Comments;
            ShortLabel = xlsFunction.PID;
            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

            if (xlsFunction.Level != null) Category = xlsFunction.Level.Label;
            if (xlsFunction.Level2 != null) Category2 = xlsFunction.Level2.Label;

            RowsNumber = xlsFunction.Rows;
            ByteInput = xlsFunction.Bytes == 1;
            ByteOutput = xlsFunction.Bytes == 1;

            InputScaleExpression = xlsFunction.XEquation;
            InputScalePrecision = xlsFunction.XDigits;
            if (InputScalePrecision < SADDef.DefaultScaleMinPrecision) InputScalePrecision = SADDef.DefaultScaleMinPrecision;
            if (InputScalePrecision > SADDef.DefaultScaleMaxPrecision) InputScalePrecision = SADDef.DefaultScaleMaxPrecision;
            InputUnits = xlsFunction.XUnits;

            InputMin = xlsFunction.XMin;
            InputMax = xlsFunction.XMax;

            OutputScaleExpression = xlsFunction.YEquation;
            OutputScalePrecision = xlsFunction.YDigits;
            if (OutputScalePrecision < SADDef.DefaultScaleMinPrecision) OutputScalePrecision = SADDef.DefaultScaleMinPrecision;
            if (OutputScalePrecision > SADDef.DefaultScaleMaxPrecision) OutputScalePrecision = SADDef.DefaultScaleMaxPrecision;
            OutputUnits = xlsFunction.YUnits;

            OutputMin = xlsFunction.YMin;
            OutputMax = xlsFunction.YMax;
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
            clone.InputScalePrecision = InputScalePrecision;
            clone.OutputScalePrecision = OutputScalePrecision;

            clone.InputUnits = InputUnits;
            clone.OutputUnits = OutputUnits;

            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.Label = Label;
            clone.Comments = Comments;

            clone.InputMin = InputMin;
            clone.InputMax = InputMax;
            clone.OutputMin = OutputMin;
            clone.OutputMax = OutputMax;

            clone.Information = Information;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

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
                if (InputScalePrecision != SADDef.DefaultScalePrecision) return true;
                if (OutputScalePrecision != SADDef.DefaultScalePrecision) return true;
                if (InputMin != null && InputMin != string.Empty) return true;
                if (InputMax != null && InputMax != string.Empty) return true;
                if (OutputMin != null && OutputMin != string.Empty) return true;
                if (OutputMax != null && OutputMax != string.Empty) return true;

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
                    if (ShortLabel != SADDef.ShortExtFunctionPrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator) && ShortLabel != SADDef.ShortExtFunctionPrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", string.Empty) && !ShortLabel.StartsWith(SADDef.ShortExtFunctionPrefix + BankNum.ToString() + SADDef.NamingShortBankSeparator)) return true;
                    if (Label != SADDef.LongExtFunctionPrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", SADDef.NamingShortBankSeparator) && Label != SADDef.LongExtFunctionPrefix + SADDef.NamingShortBankSeparator + UniqueAddressHex.Replace(" ", string.Empty) && !Label.StartsWith(SADDef.LongExtFunctionPrefix + BankNum.ToString() + SADDef.NamingShortBankSeparator)) return true;
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

        [XmlAttribute]
        public bool HideParent { get; set; }

        public string Label { get; set; }

        public string Comments { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

        [XmlIgnore]
        public string UniqueKey { get { return string.Format("{0:d2}", Position); } }

        public S6xBitFlag Clone()
        {
            S6xBitFlag clone = new S6xBitFlag();
            clone.Position = Position;
            clone.Skip = Skip;

            clone.ShortLabel = Label;
            clone.Label = Label;
            clone.Comments = Comments;

            clone.SetValue = SetValue;
            clone.NotSetValue = NotSetValue;
            clone.HideParent = HideParent;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

            return clone;
        }
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
        [XmlAttribute]
        public bool InlineComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        public string ScaleExpression { get; set; }
        [XmlAttribute]
        public int ScalePrecision { get; set; }

        public string Units { get; set; }

        [XmlAttribute]
        public string Min { get; set; }
        [XmlAttribute]
        public string Max { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

        [XmlIgnore]
        public bool Store { get; set; }

        [XmlIgnore]
        public string Information { get; set; }

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
            ScalePrecision = SADDef.DefaultScalePrecision;
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
            if (scalarCalElem.ScalarElem.Comments == string.Empty) Comments = scalarCalElem.ScalarElem.Address;
            else Comments = scalarCalElem.ScalarElem.Comments;
            ScaleExpression = scalarCalElem.ScalarElem.ScaleExpression;  // From Forced Values at lower level
            ScalePrecision = scalarCalElem.ScalarElem.ScalePrecision;
            Units = scalarCalElem.ScalarElem.Units;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

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
                    bitFlag.DateCreated = DateTime.UtcNow;
                    bitFlag.DateUpdated = DateTime.UtcNow;
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

                    OutputComments = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.OutputComments;
                    InlineComments = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.InlineComments;

                    Min = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Min;
                    Max = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Max;

                    IdentificationStatus = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.IdentificationStatus;
                    IdentificationDetails = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.IdentificationDetails;
                    Category = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Category;
                    Category2 = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Category2;
                    Category3 = scalarCalElem.ScalarElem.S6xElementSignatureSource.Scalar.Category3;
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
            Comments = scalar.Comments;
            ScaleExpression = scalar.ScaleExpression;  // From Forced Values at lower level
            ScalePrecision = scalar.ScalePrecision;
            Units = scalar.Units;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

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
                    bitFlag.DateCreated = DateTime.UtcNow;
                    bitFlag.DateUpdated = DateTime.UtcNow;
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

                    OutputComments = scalar.S6xElementSignatureSource.Scalar.OutputComments;
                    InlineComments = scalar.S6xElementSignatureSource.Scalar.InlineComments;

                    Min = scalar.S6xElementSignatureSource.Scalar.Min;
                    Max = scalar.S6xElementSignatureSource.Scalar.Max;

                    IdentificationStatus = scalar.S6xElementSignatureSource.Scalar.IdentificationStatus;
                    IdentificationDetails = scalar.S6xElementSignatureSource.Scalar.IdentificationDetails;
                    Category = scalar.S6xElementSignatureSource.Scalar.Category;
                    Category2 = scalar.S6xElementSignatureSource.Scalar.Category2;
                    Category3 = scalar.S6xElementSignatureSource.Scalar.Category3;
                }
            }
        }

        public S6xScalar(XdfScalar xdfScalar, int bankNum, int address, int addressBin, bool isCalElem, XdfHeaderCategory[] xdfHeaderCategories)
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
            ScalePrecision = SADDef.DefaultScalePrecision;
            if (xdfScalar.decimalpl != null)
            {
                try { ScalePrecision = Convert.ToInt32(xdfScalar.decimalpl); }
                catch { }
                if (ScalePrecision < SADDef.DefaultScaleMinPrecision) ScalePrecision = SADDef.DefaultScaleMinPrecision;
                if (ScalePrecision > SADDef.DefaultScaleMaxPrecision) ScalePrecision = SADDef.DefaultScaleMaxPrecision;
            }

            Units = xdfScalar.units;
            Min = xdfScalar.min;
            Max = xdfScalar.max;
            // Version 1.60
            if (xdfScalar.rangelow != null && xdfScalar.rangelow != string.Empty) Min = xdfScalar.rangelow;
            if (xdfScalar.rangehigh != null && xdfScalar.rangehigh != string.Empty) Max = xdfScalar.rangehigh;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;
            string[] arrCategories = Tools.S6xElementCategories(xdfHeaderCategories, xdfScalar.xdfCategories);
            if (arrCategories != null)
            {
                if (arrCategories.Length > 0) Category = arrCategories[0];
                if (arrCategories.Length > 1) Category2 = arrCategories[1];
                if (arrCategories.Length > 2) Category3 = arrCategories[2];
            }
            arrCategories = null;
        }

        public S6xScalar(XdfFlag xdfFlag, int bankNum, int address, int addressBin, bool isCalElem, XdfHeaderCategory[] xdfHeaderCategories)
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
            ScalePrecision = SADDef.DefaultScalePrecision;
            Units = string.Empty;

            AddBitFlag(xdfFlag, xdfHeaderCategories);

            Label = "Bit Flags " + UniqueAddressHex;
            ShortLabel = "TpBf" + UniqueAddressHex.Replace(" ", "_");

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;
        }

        public S6xScalar(XlsScalar xlsScalar, int bankNum, int address, int addressBin, bool isCalElem)
        {
            BankNum = bankNum;
            AddressInt = address;
            AddressBinInt = addressBin;
            isCalibrationElement = isCalElem;
            Store = true;
            Skip = false;
            Byte = xlsScalar.Bytes == 1;
            Signed = xlsScalar.Signed;
            Label = xlsScalar.Parameter;
            Comments = xlsScalar.Comments;
            ShortLabel = xlsScalar.PID;
            ScaleExpression = xlsScalar.Equation;
            ScalePrecision = xlsScalar.Digits;
            if (ScalePrecision < SADDef.DefaultScaleMinPrecision) ScalePrecision = SADDef.DefaultScaleMinPrecision;
            if (ScalePrecision > SADDef.DefaultScaleMaxPrecision) ScalePrecision = SADDef.DefaultScaleMaxPrecision;

            Units = xlsScalar.Units;

            Min = xlsScalar.Min;
            Max = xlsScalar.Max;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

            if (xlsScalar.Level != null) Category = xlsScalar.Level.Label;
            if (xlsScalar.Level2 != null) Category2 = xlsScalar.Level2.Label;
        }

        public void AddBitFlag(XdfFlag xdfFlag, XdfHeaderCategory[] xdfHeaderCategories)
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

            bitFlag.DateCreated = DateTime.UtcNow;

            existingBitFlag = slBitFlags.ContainsKey(bitFlag.UniqueKey);
            if (existingBitFlag) bitFlag = (S6xBitFlag)slBitFlags[bitFlag.UniqueKey];
            else slBitFlags.Add(bitFlag.UniqueKey, bitFlag);

            bitFlag.DateUpdated = DateTime.UtcNow;

            bitFlag.Label = xdfFlag.titleXmlValid;
            bitFlag.Comments = xdfFlag.descriptionXmlValid;

            if (!existingBitFlag)
            {
                bitFlag.ShortLabel = Tools.XDFLabelComToShortLabel(bitFlag.Label, bitFlag.Comments, "B" + bitFlag.Position.ToString());
                bitFlag.Label = Tools.XDFLabelReview(bitFlag.Label, bitFlag.ShortLabel);
                bitFlag.SetValue = "1";
                bitFlag.NotSetValue = "0";
            }

            string[] arrCategories = Tools.S6xElementCategories(xdfHeaderCategories, xdfFlag.xdfCategories);
            if (arrCategories != null)
            {
                if (arrCategories.Length > 0) bitFlag.Category = arrCategories[0];
                if (arrCategories.Length > 1) bitFlag.Category2 = arrCategories[1];
                if (arrCategories.Length > 2) bitFlag.Category3 = arrCategories[2];
            }
            arrCategories = null;

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
            bitFlag.HideParent = s6xBitFlag.HideParent;

            bitFlag.DateCreated = s6xBitFlag.DateCreated;
            bitFlag.DateUpdated = s6xBitFlag.DateUpdated;
            bitFlag.IdentificationStatus = s6xBitFlag.IdentificationStatus;
            bitFlag.IdentificationDetails = s6xBitFlag.IdentificationDetails;
            bitFlag.Category = s6xBitFlag.Category;
            bitFlag.Category2 = s6xBitFlag.Category2;
            bitFlag.Category3 = s6xBitFlag.Category3;

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

            bitFlag = null;
            slBitFlags = null;
        }

        public void DelBitFlag(S6xBitFlag s6xBitFlag)
        {
            if (s6xBitFlag == null) return;
            
            SortedList slBitFlags = new SortedList();

            if (BitFlags != null) foreach (S6xBitFlag bF in BitFlags) if (bF.Position != s6xBitFlag.Position) slBitFlags.Add(bF.UniqueKey, bF);

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

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
            clone.ScalePrecision = ScalePrecision;
            clone.Units = Units;

            clone.ShortLabel = ShortLabel;
            clone.OutputComments = OutputComments;
            clone.InlineComments = InlineComments;
            clone.Label = Label;
            clone.Comments = Comments;

            clone.Min = Min;
            clone.Max = Max;

            clone.Information = Information;

            if (BitFlags != null) clone.BitFlags = (S6xBitFlag[])BitFlags.Clone();

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;
            
            return clone;
        }

        public bool isUserDefined
        {
            get
            {
                if (Units != null && Units != string.Empty) return true;
                if (OutputComments) return true;
                if (ScaleExpression != null) if (ScaleExpression.ToUpper() != "X") return true;
                if (ScalePrecision != SADDef.DefaultScalePrecision) return true;

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

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

        [XmlIgnore]
        public Structure Structure { get; set; }
        
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
            if (structureCalElem.StructureElem.Comments == string.Empty) Comments = (structureCalElem.StructureElem.Defaulted) ? "Structure definition was defaulted" : string.Empty;
            else Comments = structureCalElem.StructureElem.Comments;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

            // Forced Values (Initially from S6x Internals)
            if (structureCalElem.StructureElem.S6xElementSignatureSource != null)
            {
                if (structureCalElem.StructureElem.S6xElementSignatureSource.Structure != null)
                {
                    if (structureCalElem.StructureElem.S6xElementSignatureSource.Structure.Comments != null && structureCalElem.StructureElem.S6xElementSignatureSource.Structure.Comments != string.Empty)
                    {
                        Comments = structureCalElem.StructureElem.S6xElementSignatureSource.Structure.Comments;
                    }

                    OutputComments = structureCalElem.StructureElem.S6xElementSignatureSource.Structure.OutputComments;

                    IdentificationStatus = structureCalElem.StructureElem.S6xElementSignatureSource.Structure.IdentificationStatus;
                    IdentificationDetails = structureCalElem.StructureElem.S6xElementSignatureSource.Structure.IdentificationDetails;
                    Category = structureCalElem.StructureElem.S6xElementSignatureSource.Structure.Category;
                    Category2 = structureCalElem.StructureElem.S6xElementSignatureSource.Structure.Category2;
                    Category3 = structureCalElem.StructureElem.S6xElementSignatureSource.Structure.Category3;
                }
            }
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
            if (structure.Comments == string.Empty) Comments = (structure.Defaulted) ? "Structure definition was defaulted" : string.Empty;
            else Comments = structure.Comments;

            DateCreated = DateTime.UtcNow;
            DateUpdated = DateTime.UtcNow;

            // Forced Values (Initially from S6x Internals)
            if (structure.S6xElementSignatureSource != null)
            {
                if (structure.S6xElementSignatureSource.Structure != null)
                {
                    if (structure.S6xElementSignatureSource.Structure.Comments != null && structure.S6xElementSignatureSource.Structure.Comments != string.Empty)
                    {
                        Comments = structure.S6xElementSignatureSource.Structure.Comments;
                    }

                    OutputComments = structure.S6xElementSignatureSource.Structure.OutputComments;

                    IdentificationStatus = structure.S6xElementSignatureSource.Structure.IdentificationStatus;
                    IdentificationDetails = structure.S6xElementSignatureSource.Structure.IdentificationDetails;
                    Category = structure.S6xElementSignatureSource.Structure.Category;
                    Category2 = structure.S6xElementSignatureSource.Structure.Category2;
                    Category3 = structure.S6xElementSignatureSource.Structure.Category3;
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

            DateCreated = clipBoardClone.DateCreated;
            DateUpdated = clipBoardClone.DateUpdated;
            IdentificationStatus = clipBoardClone.IdentificationStatus;
            IdentificationDetails = clipBoardClone.IdentificationDetails;
            Category = clipBoardClone.Category;
            Category2 = clipBoardClone.Category2;
            Category3 = clipBoardClone.Category3;
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

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

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

            clone.Information = Information;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

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

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

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
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

        [XmlAttribute]
        public bool Skip { get; set; }
        [XmlIgnore]
        public bool Store { get; set; }
        [XmlIgnore]
        public bool SignatureForced { get; set; }
        [XmlIgnore]
        public string SignatureKey { get; set; }

        [XmlIgnore]
        public string Information { get; set; }

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
            ByteArgumentsNum = cCall.ByteArgsNum;
            if (cCall.ArgsCondValidated) ByteArgumentsNum += cCall.ArgsNumCondAdder;
            if (ByteArgumentsNum < 0) ByteArgumentsNum = 0;
            if (cCall.ArgsStackDepthMax < 1) ByteArgumentsNum = 0;
            ByteArgumentsNumOverride = false;
            Label = cCall.Label;
            ShortLabel = cCall.ShortLabel;
            if (Label == string.Empty) Label = cCall.UniqueAddressHex;
            if (rRoutine == null) Comments = cCall.UniqueAddressHex;
            else Comments = rRoutine.Comments;

            if (cCall.Arguments != null && ByteArgumentsNum > 0)
            {
                int iMatchingArgs = 0;
                foreach (CallArgument cArg in cCall.Arguments) if (cArg.StackDepth == 1) iMatchingArgs++;
                if (iMatchingArgs > 0)
                {
                    ByteArgumentsNum = 0;
                    if (cCall.ArgsCondValidated) ByteArgumentsNum += cCall.ArgsNumCondAdder;
                    int iCallArg = 0;
                    InputArguments = new S6xRoutineInputArgument[iMatchingArgs];
                    foreach (CallArgument cArg in cCall.Arguments)
                    {
                        if (cArg.StackDepth != 1) continue;
                        InputArguments[iCallArg] = new S6xRoutineInputArgument();
                        InputArguments[iCallArg].Position = iCallArg + 1;
                        InputArguments[iCallArg].UniqueKey = string.Format("Ra{0:d3}", InputArguments[iCallArg].Position);
                        InputArguments[iCallArg].Encryption = (int)cArg.Mode;
                        InputArguments[iCallArg].Word = cArg.Word;
                        InputArguments[iCallArg].Pointer = cArg.Word;
                        ByteArgumentsNum++;
                        if (cArg.Word) ByteArgumentsNum++;
                        iCallArg++;
                    }
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

        public void DelInputArgument(S6xRoutineInputArgument s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InputArguments != null) foreach (S6xRoutineInputArgument sObject in InputArguments) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InputArguments = new S6xRoutineInputArgument[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InputArguments, 0);

            slSubObjects = null;
        }

        public void DelInputStructure(S6xRoutineInputStructure s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InputStructures != null) foreach (S6xRoutineInputStructure sObject in InputStructures) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InputStructures = new S6xRoutineInputStructure[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InputStructures, 0);

            slSubObjects = null;
        }

        public void DelInputTable(S6xRoutineInputTable s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InputTables != null) foreach (S6xRoutineInputTable sObject in InputTables) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InputTables = new S6xRoutineInputTable[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InputTables, 0);

            slSubObjects = null;
        }

        public void DelInputFunction(S6xRoutineInputFunction s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InputFunctions != null) foreach (S6xRoutineInputFunction sObject in InputFunctions) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InputFunctions = new S6xRoutineInputFunction[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InputFunctions, 0);

            slSubObjects = null;
        }

        public void DelInputScalar(S6xRoutineInputScalar s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InputScalars != null) foreach (S6xRoutineInputScalar sObject in InputScalars) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InputScalars = new S6xRoutineInputScalar[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InputScalars, 0);

            slSubObjects = null;
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

            clone.Information = Information;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

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

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

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

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

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
        [XmlAttribute]
        public int CellsScalePrecision { get; set; }

        public string ColsUnits { get; set; }
        public string RowsUnits { get; set; }
        public string CellsUnits { get; set; }

        [XmlAttribute]
        public string CellsMin { get; set; }
        [XmlAttribute]
        public string CellsMax { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

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
            clone.CellsScalePrecision = CellsScalePrecision;
            clone.ColsUnits = ColsUnits;
            clone.RowsUnits = RowsUnits;
            clone.CellsUnits = CellsUnits;

            clone.CellsMin = CellsMin;
            clone.CellsMax = CellsMax;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

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
        [XmlAttribute]
        public int InputScalePrecision { get; set; }
        [XmlAttribute]
        public int OutputScalePrecision { get; set; }

        public string InputUnits { get; set; }
        public string OutputUnits { get; set; }

        [XmlAttribute]
        public string InputMin { get; set; }
        [XmlAttribute]
        public string InputMax { get; set; }
        [XmlAttribute]
        public string OutputMin { get; set; }
        [XmlAttribute]
        public string OutputMax { get; set; }
        
        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

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
            clone.InputScalePrecision = InputScalePrecision;
            clone.OutputScalePrecision = OutputScalePrecision;
            clone.InputUnits = InputUnits;
            clone.OutputUnits = OutputUnits;

            clone.InputMin = InputMin;
            clone.InputMax = InputMax;
            clone.OutputMin = OutputMin;
            clone.OutputMax = OutputMax;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

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
        [XmlAttribute]
        public bool InlineComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        public string ScaleExpression { get; set; }
        [XmlAttribute]
        public int ScalePrecision { get; set; }

        public string Units { get; set; }

        [XmlAttribute]
        public string Min { get; set; }
        [XmlAttribute]
        public string Max { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

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
            clone.InlineComments = InlineComments;
            clone.Label = Label;
            clone.Comments = Comments;
            clone.ScaleExpression = ScaleExpression;
            clone.ScalePrecision = ScalePrecision;
            clone.Units = Units;

            clone.Min = Min;
            clone.Max = Max;
            
            if (BitFlags != null) clone.BitFlags = (S6xBitFlag[])BitFlags.Clone();

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

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
            bitFlag.HideParent = s6xBitFlag.HideParent;

            bitFlag.DateCreated = s6xBitFlag.DateCreated;
            bitFlag.DateUpdated = s6xBitFlag.DateUpdated;
            bitFlag.IdentificationStatus = s6xBitFlag.IdentificationStatus;
            bitFlag.IdentificationDetails = s6xBitFlag.IdentificationDetails;
            bitFlag.Category = s6xBitFlag.Category;
            bitFlag.Category2 = s6xBitFlag.Category2;
            bitFlag.Category3 = s6xBitFlag.Category3;

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

            bitFlag = null;
            slBitFlags = null;
        }

        public void DelBitFlag(S6xBitFlag s6xBitFlag)
        {
            if (s6xBitFlag == null) return;

            SortedList slBitFlags = new SortedList();

            if (BitFlags != null) foreach (S6xBitFlag bF in BitFlags) if (bF.Position != s6xBitFlag.Position) slBitFlags.Add(bF.UniqueKey, bF);

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

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

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }

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

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;

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

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }

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

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;

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
        [XmlAttribute]
        public int ForcedCellsScalePrecision { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }

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
            clone.ForcedCellsScalePrecision = ForcedCellsScalePrecision;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;

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
        [XmlAttribute]
        public int ForcedInputScalePrecision { get; set; }
        [XmlAttribute]
        public int ForcedOutputScalePrecision { get; set; }

        public string ForcedInputUnits { get; set; }
        public string ForcedOutputUnits { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }

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
            clone.ForcedInputScalePrecision = ForcedInputScalePrecision;
            clone.ForcedOutputScalePrecision = ForcedOutputScalePrecision;

            clone.ForcedInputUnits = ForcedInputUnits;
            clone.ForcedOutputUnits = ForcedOutputUnits;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;

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
        [XmlAttribute]
        public int ForcedScalePrecision { get; set; }

        public string ForcedUnits { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }

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
            clone.ForcedScalePrecision = ForcedScalePrecision;

            clone.ForcedUnits = ForcedUnits;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;

            return clone;
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
        [XmlAttribute]
        public bool InlineComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

        [XmlIgnore]
        public string Information { get; set; }

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

        // 20210421 - PYM - Information and Synchro only, so nullable
        [XmlAttribute]
        public string SizeStatus { get; set; }
        [XmlAttribute]
        public string SignedStatus { get; set; }

        [XmlAttribute]
        public bool isRBase { get; set; }
        [XmlAttribute]
        public bool isRConst { get; set; }
        [XmlAttribute]
        public string ConstValue { get; set; }
        [XmlIgnore]
        public bool AutoConstValue { get; set; }

        public string Label { get; set; }
        public string ByteLabel { get; set; }
        public string WordLabel { get; set; }
        public string Comments { get; set; }

        [XmlArray(ElementName = "BitFlags")]
        [XmlArrayItem(ElementName = "BitFlag")]
        public S6xBitFlag[] BitFlags { get; set; }

        public string ScaleExpression { get; set; }
        [XmlAttribute]
        public int ScalePrecision { get; set; }

        public string Units { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

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
            ScalePrecision = SADDef.DefaultScalePrecision;
        }

        public S6xRegister(int addressInt)
        {
            AddressInt = addressInt;

            ScalePrecision = SADDef.DefaultScalePrecision;
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

            ScalePrecision = SADDef.DefaultScalePrecision;
        }

        public S6xRegister(XlsPayLoad xlsPayLoad)
        {
            AddressInt = xlsPayLoad.AddressInt;
            Store = true;
            Label = xlsPayLoad.Label;
            Comments = xlsPayLoad.Description;
            if (xlsPayLoad.Comments != null && xlsPayLoad.Comments != string.Empty)
            {
                if (Comments == null || Comments == string.Empty) Comments = xlsPayLoad.Comments;
                else Comments += "\r\n" + xlsPayLoad.Comments;
            }
            ScaleExpression = xlsPayLoad.Equation;
            ScalePrecision = xlsPayLoad.Digits;
            if (ScalePrecision < SADDef.DefaultScaleMinPrecision) ScalePrecision = SADDef.DefaultScaleMinPrecision;
            if (ScalePrecision > SADDef.DefaultScaleMaxPrecision) ScalePrecision = SADDef.DefaultScaleMaxPrecision;
            Units = xlsPayLoad.Units;
            SizeStatus = xlsPayLoad.Bytes == 1 ? "Byte" : "Word";
            SignedStatus = xlsPayLoad.Signed ? "Signed" : "Unsigned";

            if (xlsPayLoad.BitsArray != null)
            {
                for (int iBit = 0; iBit < xlsPayLoad.BitsArray.Count; iBit++)
                {
                    if (iBit > 7) break;
                    if (xlsPayLoad.BitsArray[iBit] == null) continue;
                    if (xlsPayLoad.BitsArray[iBit] == string.Empty) continue;
                    AddBitFlag(new S6xBitFlag() {Position = iBit, ShortLabel = xlsPayLoad.BitsArray[iBit], Label = xlsPayLoad.BitsArray[iBit]});
                }
            }
        }

        public S6xBitFlag GetBitFlag(int bitFlagPosition)
        {
            if (BitFlags == null) return null;
            foreach (S6xBitFlag bF in BitFlags) if (bF.Position == bitFlagPosition) return bF;
            return null;
        }

        public void RemoveBitFlag(int bitFlagPosition)
        {
            SortedList slBitFlags = new SortedList();
            if (BitFlags != null) foreach (S6xBitFlag bF in BitFlags) if (bF.Position != bitFlagPosition) slBitFlags.Add(bF.UniqueKey, bF);

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

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
            bitFlag.HideParent = s6xBitFlag.HideParent;

            bitFlag.DateCreated = s6xBitFlag.DateCreated;
            bitFlag.DateUpdated = s6xBitFlag.DateUpdated;
            bitFlag.IdentificationStatus = s6xBitFlag.IdentificationStatus;
            bitFlag.IdentificationDetails = s6xBitFlag.IdentificationDetails;
            bitFlag.Category = s6xBitFlag.Category;
            bitFlag.Category2 = s6xBitFlag.Category2;
            bitFlag.Category3 = s6xBitFlag.Category3;

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

            bitFlag = null;
            slBitFlags = null;
        }

        public void DelBitFlag(S6xBitFlag s6xBitFlag)
        {
            if (s6xBitFlag == null) return;

            SortedList slBitFlags = new SortedList();
            if (BitFlags != null) foreach (S6xBitFlag bF in BitFlags) if (bF.Position != s6xBitFlag.Position) slBitFlags.Add(bF.UniqueKey, bF);

            BitFlags = new S6xBitFlag[slBitFlags.Count];
            slBitFlags.Values.CopyTo(BitFlags, 0);

            slBitFlags = null;
        }

        public S6xRegister Clone()
        {
            S6xRegister clone = new S6xRegister();
            clone.AddressInt = AddressInt;
            clone.AdditionalAddress10 = AdditionalAddress10;
            clone.Skip = Skip;
            clone.Store = Store;

            clone.isRBase = isRBase;
            clone.isRConst = isRConst;
            clone.ConstValue = ConstValue;
            clone.AutoConstValue = AutoConstValue;

            clone.Label = Label;
            clone.ByteLabel = ByteLabel;
            clone.WordLabel = WordLabel;
            clone.Comments = Comments;

            clone.ScaleExpression = ScaleExpression;
            clone.ScalePrecision = ScalePrecision;

            clone.Units = Units;

            clone.SizeStatus = SizeStatus;
            clone.SignedStatus = SignedStatus;

            clone.Information = Information;

            if (BitFlags != null) clone.BitFlags = (S6xBitFlag[])BitFlags.Clone();

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;
            clone.Category = Category;
            clone.Category2 = Category2;
            clone.Category3 = Category3;

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
        public bool OutputLabel { get; set; }
        [XmlAttribute]
        public bool OutputComments { get; set; }
        [XmlAttribute]
        public bool InlineComments { get; set; }

        public string Label { get; set; }
        public string Comments { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        public string Category { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

        [XmlIgnore]
        public string Information { get; set; }

        [XmlIgnore]
        public string Address { get { return string.Format("{0:x4}", AddressInt + SADDef.EecBankStartAddress); } }

        [XmlIgnore]
        public string UniqueAddress { get { return string.Format("{0,1} {1,5}", BankNum, AddressInt); } }
        [XmlIgnore]
        public string UniqueAddressHex { get { return string.Format("{0,1} {1,4}", BankNum, Address); } }

        [XmlIgnore]
        public string DefaultLabel { get { return SADDef.ShortOtherAddressPrefix + UniqueAddressHex.Replace(" ", "_"); } }

        [XmlIgnore]
        public bool hasDefaultLabel { get { return Label == DefaultLabel; } }
        
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

        [XmlAttribute]
        public bool Forced { get; set; }

        public string Signature { get; set; }

        // Routine Dedicated
        [XmlAttribute]
        public string ShortLabel { get; set; }  //  Values from V0
        [XmlAttribute]
        public bool OutputComments { get; set; }
        public string Label { get; set; }
        public string Comments { get; set; }

        // Routine Dedicated
        [XmlAttribute]
        public DateTime RoutineDateCreated { get; set; }
        [XmlAttribute]
        public DateTime RoutineDateUpdated { get; set; }
        public string RoutineCategory { get; set; }
        public string RoutineCategory2 { get; set; }
        public string RoutineCategory3 { get; set; }
        [XmlAttribute]
        public int RoutineIdentificationStatus { get; set; }
        public string RoutineIdentificationDetails { get; set; }

        // Signature Dedicated
        public string SignatureLabel { get; set; }
        public string SignatureCategory { get; set; }
        public string SignatureCategory2 { get; set; }
        public string SignatureCategory3 { get; set; }
        public string SignatureComments { get; set; }

        // Signature Dedicated
        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        [XmlAttribute]
        public string for806x { get; set; }
        [XmlAttribute]
        public string forBankNum { get; set; }

        [XmlIgnore]
        public bool Ignore { get; set; }
        [XmlIgnore]
        public string Information { get; set; }
        [XmlIgnore]
        public bool Found { get; set; }

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

        public S6xSignature(object[] routineSignature)
        {
            Forced = true;

            try
            {
                // routineSignature
                // Fixed Routines Signatures Array
                // UniqueKey, Calibration Element Enum (Enum), Bytes Signature (String), Signature Label (String), Signature Categ (String), Signature Categ 2 (String), Signature Categ 2 (String), Signature Comments (String)

                UniqueKey = (string)routineSignature[0];
                Signature = (string)routineSignature[2];
                SignatureLabel = (string)routineSignature[3];
                SignatureCategory = (string)routineSignature[4];
                SignatureCategory2 = (string)routineSignature[5];
                SignatureCategory3 = (string)routineSignature[6];
                SignatureComments = (string)routineSignature[7];

                S6xSignature rSig = SADFixedSigs.GetFixedRoutineSignatureTemplate((SADFixedSigs.Fixed_Routines)routineSignature[1]);
                if (rSig == null)
                {
                    Skip = true;
                    return;
                }

                // Routine part
                ShortLabel = rSig.ShortLabel;
                Label = rSig.Label;
                Comments = rSig.Comments;
                OutputComments = rSig.OutputComments;
                
                for806x = rSig.for806x;
                forBankNum = rSig.forBankNum;

                if (rSig.InputArguments != null)
                {
                    InputArguments = new S6xRoutineInputArgument[rSig.InputArguments.Length];
                    for (int iInputO = 0; iInputO < InputArguments.Length; iInputO++)
                    {
                        InputArguments[iInputO] = rSig.InputArguments[iInputO].Clone();
                        InputArguments[iInputO].UniqueKey = string.Format("Ra{0:d3}", InputArguments[iInputO].Position);
                    }
                }
                // ...
                if (rSig.InternalStructures != null)
                {
                    InternalStructures = new S6xRoutineInternalStructure[rSig.InternalStructures.Length];
                    for (int iInterO = 0; iInterO < InternalStructures.Length; iInterO++)
                    {
                        InternalStructures[iInterO] = rSig.InternalStructures[iInterO].Clone();
                        InternalStructures[iInterO].UniqueKey = getNewElemUniqueKey();
                    }
                }
                rSig = null;

                Found = false;
                Ignore = false;

                Skip = false;
            }
            catch
            {
                Skip = true;
            }
        }

        public void DelInputArgument(S6xRoutineInputArgument s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InputArguments != null) foreach (S6xRoutineInputArgument sObject in InputArguments) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InputArguments = new S6xRoutineInputArgument[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InputArguments, 0);

            slSubObjects = null;
        }

        public void DelInputStructure(S6xRoutineInputStructure s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InputStructures != null) foreach (S6xRoutineInputStructure sObject in InputStructures) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InputStructures = new S6xRoutineInputStructure[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InputStructures, 0);

            slSubObjects = null;
        }

        public void DelInputTable(S6xRoutineInputTable s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InputTables != null) foreach (S6xRoutineInputTable sObject in InputTables) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InputTables = new S6xRoutineInputTable[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InputTables, 0);

            slSubObjects = null;
        }

        public void DelInputFunction(S6xRoutineInputFunction s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InputFunctions != null) foreach (S6xRoutineInputFunction sObject in InputFunctions) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InputFunctions = new S6xRoutineInputFunction[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InputFunctions, 0);

            slSubObjects = null;
        }

        public void DelInputScalar(S6xRoutineInputScalar s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InputScalars != null) foreach (S6xRoutineInputScalar sObject in InputScalars) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InputScalars = new S6xRoutineInputScalar[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InputScalars, 0);

            slSubObjects = null;
        }

        public void DelInternalStructure(S6xRoutineInternalStructure s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InternalStructures != null) foreach (S6xRoutineInternalStructure sObject in InternalStructures) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InternalStructures = new S6xRoutineInternalStructure[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InternalStructures, 0);

            slSubObjects = null;
        }

        public void DelInternalTable(S6xRoutineInternalTable s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InternalTables != null) foreach (S6xRoutineInternalTable sObject in InternalTables) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InternalTables = new S6xRoutineInternalTable[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InternalTables, 0);

            slSubObjects = null;
        }

        public void DelInternalFunction(S6xRoutineInternalFunction s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InternalFunctions != null) foreach (S6xRoutineInternalFunction sObject in InternalFunctions) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InternalFunctions = new S6xRoutineInternalFunction[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InternalFunctions, 0);

            slSubObjects = null;
        }

        public void DelInternalScalar(S6xRoutineInternalScalar s6xSubObject)
        {
            if (s6xSubObject == null) return;

            SortedList slSubObjects = new SortedList();
            if (InternalScalars != null) foreach (S6xRoutineInternalScalar sObject in InternalScalars) if (sObject.UniqueKey != s6xSubObject.UniqueKey) slSubObjects.Add(sObject.UniqueKey, sObject);

            InternalScalars = new S6xRoutineInternalScalar[slSubObjects.Count];
            slSubObjects.Values.CopyTo(InternalScalars, 0);

            slSubObjects = null;
        }

        public S6xSignature Clone()
        {
            S6xSignature clone = new S6xSignature();
            clone.UniqueKey = UniqueKey;
            clone.Skip = Skip;
            clone.Signature = Signature;
            clone.SignatureLabel = SignatureLabel;
            clone.SignatureCategory = SignatureCategory;
            clone.SignatureCategory2 = SignatureCategory2;
            clone.SignatureCategory3 = SignatureCategory3;
            clone.SignatureComments = SignatureComments;

            // Routine part
            clone.ShortLabel = ShortLabel;
            clone.Label = Label;
            clone.Comments = Comments;
            clone.OutputComments = OutputComments;

            clone.for806x = for806x;
            clone.forBankNum = forBankNum;

            clone.Found = Found;
            clone.Ignore = Ignore;

            clone.Information = Information;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;

            clone.RoutineDateCreated = RoutineDateCreated;
            clone.RoutineDateUpdated = RoutineDateUpdated;
            clone.RoutineCategory = RoutineCategory;
            clone.RoutineCategory2 = RoutineCategory2;
            clone.RoutineCategory3 = RoutineCategory3;
            clone.RoutineIdentificationStatus = RoutineIdentificationStatus;
            clone.RoutineIdentificationDetails = RoutineIdentificationDetails;

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

        public string getNewElemUniqueKey()
        {
            int cnt = 0;
            ArrayList alKeys = new ArrayList();

            if (InternalStructures != null)
            {
                cnt += InternalStructures.Length;
                foreach (S6xRoutineInternalStructure s6xObject in InternalStructures)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.UniqueKey == null) continue;
                    if (alKeys.Contains(s6xObject.UniqueKey)) continue;
                    alKeys.Add(s6xObject.UniqueKey);
                }
            }
            if (InternalTables != null)
            {
                cnt += InternalTables.Length;
                foreach (S6xRoutineInternalTable s6xObject in InternalTables)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.UniqueKey == null) continue;
                    if (alKeys.Contains(s6xObject.UniqueKey)) continue;
                    alKeys.Add(s6xObject.UniqueKey);
                }
            }
            if (InternalFunctions != null)
            {
                cnt += InternalFunctions.Length;
                foreach (S6xRoutineInternalFunction s6xObject in InternalFunctions)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.UniqueKey == null) continue;
                    if (alKeys.Contains(s6xObject.UniqueKey)) continue;
                    alKeys.Add(s6xObject.UniqueKey);
                }
            }
            if (InternalScalars != null)
            {
                cnt += InternalScalars.Length;
                foreach (S6xRoutineInternalScalar s6xObject in InternalScalars)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.UniqueKey == null) continue;
                    if (alKeys.Contains(s6xObject.UniqueKey)) continue;
                    alKeys.Add(s6xObject.UniqueKey);
                }
            }
            if (InputArguments != null)
            {
                cnt += InputArguments.Length;
                foreach (S6xRoutineInputArgument s6xObject in InputArguments)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.UniqueKey == null) continue;
                    if (alKeys.Contains(s6xObject.UniqueKey)) continue;
                    alKeys.Add(s6xObject.UniqueKey);
                }
            }
            if (InputStructures != null)
            {
                cnt += InputStructures.Length;
                foreach (S6xRoutineInputStructure s6xObject in InputStructures)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.UniqueKey == null) continue;
                    if (alKeys.Contains(s6xObject.UniqueKey)) continue;
                    alKeys.Add(s6xObject.UniqueKey);
                }
            }
            if (InputTables != null)
            {
                cnt += InputTables.Length;
                foreach (S6xRoutineInputTable s6xObject in InputTables)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.UniqueKey == null) continue;
                    if (alKeys.Contains(s6xObject.UniqueKey)) continue;
                    alKeys.Add(s6xObject.UniqueKey);
                }
            }
            if (InputFunctions != null)
            {
                cnt += InputFunctions.Length;
                foreach (S6xRoutineInputFunction s6xObject in InputFunctions)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.UniqueKey == null) continue;
                    if (alKeys.Contains(s6xObject.UniqueKey)) continue;
                    alKeys.Add(s6xObject.UniqueKey);
                }
            }
            if (InputScalars != null)
            {
                cnt += InputScalars.Length;
                foreach (S6xRoutineInputScalar s6xObject in InputScalars)
                {
                    if (s6xObject == null) continue;
                    if (s6xObject.UniqueKey == null) continue;
                    if (alKeys.Contains(s6xObject.UniqueKey)) continue;
                    alKeys.Add(s6xObject.UniqueKey);
                }
            }

            string uniqueKey = string.Empty;
            while (true)
            {
                uniqueKey = string.Format("Sa{0:d3}", cnt);
                if (!alKeys.Contains(uniqueKey)) break;
                cnt++;
            }
            return uniqueKey;
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
        public string SignatureCategory { get; set; }
        public string SignatureCategory2 { get; set; }
        public string SignatureCategory3 { get; set; }
        public string SignatureComments { get; set; }

        public string SignatureOpeIncludingElemAddress { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        [XmlAttribute]
        public bool for8061 { get; set; }
        [XmlAttribute]
        public string forBankNum { get; set; }

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
        public bool Found { get; set; }
        [XmlIgnore]
        public bool Ignore { get; set; }

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
                // elementSignature
                // Fixed Elements Signatures Array
                // UniqueKey, Label, Signature Categ (String), Signature Categ 2 (String), Signature Categ 3 (String), Comments, Is 8061 (True, False), Fixed Bank (-1, 0, 1, 8, 9), Calibration Element Enum (Enum), Bytes Signature (String)

                UniqueKey = (string)elementSignature[0];
                SignatureLabel = (string)elementSignature[1];
                SignatureCategory = (string)elementSignature[2];
                SignatureCategory2 = (string)elementSignature[3];
                SignatureCategory3 = (string)elementSignature[4];
                SignatureComments = (string)elementSignature[5];

                for8061 = (bool)elementSignature[6];
                forBankNum = (string)elementSignature[7];

                object oElement = SADFixedSigs.GetFixedElementS6xRoutineInternalTemplate((SADFixedSigs.Fixed_Elements)elementSignature[8]);
                if (oElement.GetType() == typeof(S6xRoutineInternalStructure)) Structure = (S6xRoutineInternalStructure)oElement;
                else if (oElement.GetType() == typeof(S6xRoutineInternalTable)) Table = (S6xRoutineInternalTable)oElement;
                else if (oElement.GetType() == typeof(S6xRoutineInternalFunction)) Function = (S6xRoutineInternalFunction)oElement;
                else if (oElement.GetType() == typeof(S6xRoutineInternalScalar)) Scalar = (S6xRoutineInternalScalar)oElement;
                oElement = null;

                Signature = (string)elementSignature[9];

                Found = false;
                Ignore = false;

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
            clone.SignatureCategory = SignatureCategory;
            clone.SignatureCategory2 = SignatureCategory2;
            clone.SignatureCategory3 = SignatureCategory3;
            clone.SignatureComments = SignatureComments;

            clone.for8061 = for8061;
            clone.forBankNum = forBankNum;
            clone.Found = Found;
            clone.Ignore = Ignore;

            clone.Information = Information;

            clone.DateCreated = DateCreated;
            clone.DateUpdated = DateUpdated;
            clone.IdentificationStatus = IdentificationStatus;
            clone.IdentificationDetails = IdentificationDetails;

            if (Structure != null) clone.Structure = Structure.Clone();
            if (Table != null) clone.Table = Table.Clone();
            if (Function != null) clone.Function = Function.Clone();
            if (Scalar != null) clone.Scalar = Scalar.Clone();

            return clone;
        }

        // Match Signature for each operation related with an element
        // Normally not used anymore and replaced with a global process
        public bool matchSignature(ref Operation elementUseOpe, SADBank elementUseOpeBank, bool is8061)
        {
            if (Skip || Found || Ignore) return false;
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

            string preparedSignature = Tools.getPreparedSignature(Signature.ToUpper());

            int startPos = elementUseOpe.AddressInt;
            if (preparedSignature.StartsWith(SignatureOpeIncludingElemAddress.ToUpper())) startPos -= preparedSignature.IndexOf(SignatureOpeIncludingElemAddress) / 2;

            preparedSignature = preparedSignature.Replace(SignatureOpeIncludingElemAddress.ToUpper(), elementUseOpe.OriginalOp.Replace(SADDef.GlobalSeparator, string.Empty).ToUpper());

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
    [XmlRoot("SyncState")]
    public class S6xSyncState
    {
        [XmlAttribute]
        public string SyncType { get; set; }
        [XmlAttribute]
        public string SyncId { get; set; }
        [XmlAttribute]
        public DateTime DateFirstSync { get; set; }
        [XmlAttribute]
        public DateTime DateLastSync { get; set; }

        public S6xSyncState()
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
        public bool NoNumberingShortFormat { get; set; }

        [XmlAttribute]
        public bool RegListOutput { get; set; }

        // 20210406 - PYM - 0x100 Register Shortcut & SFR Mngt
        [XmlAttribute]
        public bool Ignore8065RegShortcut0x100 { get; set; }
        [XmlAttribute]
        public bool Ignore8065RegShortcut0x100SFR { get; set; }

        [XmlAttribute]
        public bool OutputHeader { get; set; }

        public string Header { get; set; }

        [XmlAttribute]
        public string XdfBaseOffset { get; set; }
        [XmlAttribute]
        public bool XdfBaseOffsetSubtract { get; set; }

        [XmlArray(ElementName = "BanksProperties")]
        [XmlArrayItem(ElementName = "BankProperties")]
        public S6xBankProperties[] BanksProperties { get; set; }

        [XmlAttribute]
        public DateTime DateCreated { get; set; }
        [XmlAttribute]
        public DateTime DateUpdated { get; set; }
        [XmlAttribute]
        public int IdentificationStatus { get; set; }
        public string IdentificationDetails { get; set; }

        [XmlArray(ElementName = "SyncStates")]
        [XmlArrayItem(ElementName = "SyncState")]
        public S6xSyncState[] SyncStates { get; set; }

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

        public void SyncStateUpdate(string syncType, string syncId)
        {
            if (SyncStates == null) SyncStates = new S6xSyncState[] {};

            foreach (S6xSyncState sState in SyncStates)
            {
                if (sState.SyncType == syncType && sState.SyncId == syncId)
                {
                    sState.DateLastSync = DateTime.UtcNow;
                    return;
                }
            }

            S6xSyncState syncState = new S6xSyncState();
            syncState.SyncType = syncType;
            syncState.SyncId = syncId;
            syncState.DateFirstSync = DateTime.UtcNow;
            syncState.DateLastSync = syncState.DateFirstSync;

            ArrayList alSyncStates = new ArrayList(SyncStates);
            alSyncStates.Add(syncState);
            SyncStates = (S6xSyncState[])alSyncStates.ToArray(typeof(S6xSyncState));
            
            syncState = null;
            alSyncStates = null;
        }

        public DateTime SyncStateLastDate(string syncType, string syncId)
        {
            DateTime dtDT = new DateTime(2000, 1, 1, 0, 0, 0);

            if (SyncStates != null)
            {
                foreach (S6xSyncState sState in SyncStates)
                {
                    if (sState.SyncType == syncType && sState.SyncId == syncId)
                    {
                        return sState.DateLastSync > dtDT ? sState.DateLastSync : dtDT;
                    }
                }
            }

            return dtDT;
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
