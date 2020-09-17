using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Globalization;

namespace SAD806x
{
    public static class TfSADTools
    {
        private const string TFST_SAD_VER_0 = "SAD 0.x";
        private const string TFST_SAD_VER_3 = "SAD 3.x";
        private const string TFST_SAD_VER_4 = "SAD 4.x";

        public static string[] TFST_SAD_VERSIONS
        {
            get { return new string[] { TFST_SAD_VER_0, TFST_SAD_VER_3, TFST_SAD_VER_4 }; }
        }
        
        // Check if new Elem is in conflict
        //      Returns true when new object exists with a different type
        //      Returns true when new object exists as a reserved address
        //      Returns false when an Other address is defined for this object (Other address can have same address)
        private static bool isTypeConflict(object newElem, ref SADBin sadBin, ref SADS6x sadS6x)
        {
            if (newElem == null) return true;
            if (sadBin == null) return true;
            if (sadS6x == null) return true;

            string uniqueAddress = string.Empty;
            if (newElem.GetType() == typeof(S6xStructure)) uniqueAddress = ((S6xStructure)newElem).UniqueAddress;
            else if (newElem.GetType() == typeof(S6xTable)) uniqueAddress = ((S6xTable)newElem).UniqueAddress;
            else if (newElem.GetType() == typeof(S6xFunction)) uniqueAddress = ((S6xFunction)newElem).UniqueAddress;
            else if (newElem.GetType() == typeof(S6xScalar)) uniqueAddress = ((S6xScalar)newElem).UniqueAddress;
            else if (newElem.GetType() == typeof(S6xRoutine)) uniqueAddress = ((S6xRoutine)newElem).UniqueAddress;
            else if (newElem.GetType() == typeof(S6xOperation)) uniqueAddress = ((S6xOperation)newElem).UniqueAddress;

            if (uniqueAddress == string.Empty) return true;
            
            // Calibration Load conflict
            if (sadBin.Calibration.isLoadCreated(uniqueAddress)) return true;

            bool bConflict = false;

            // Reserved Addresses
            ArrayList arrBanks = new ArrayList();
            if (sadBin.Bank8 != null) arrBanks.Add(sadBin.Bank8);
            if (sadBin.Bank1 != null) arrBanks.Add(sadBin.Bank1);
            if (sadBin.Bank9 != null) arrBanks.Add(sadBin.Bank9);
            if (sadBin.Bank0 != null) arrBanks.Add(sadBin.Bank0);
            bConflict = false;
            foreach (SADBank sBank in arrBanks)
            {
                bConflict = sBank.slReserved.ContainsKey(uniqueAddress);
                if (bConflict) break;
            }
            arrBanks = null;
            
            if (bConflict) return bConflict;

            ArrayList arrSList = new ArrayList();
            if (newElem.GetType() != typeof(S6xStructure)) arrSList.Add(sadS6x.slStructures);
            if (newElem.GetType() != typeof(S6xTable)) arrSList.Add(sadS6x.slTables);
            if (newElem.GetType() != typeof(S6xFunction)) arrSList.Add(sadS6x.slFunctions);
            if (newElem.GetType() != typeof(S6xScalar)) arrSList.Add(sadS6x.slScalars);
            if (newElem.GetType() != typeof(S6xRoutine)) arrSList.Add(sadS6x.slRoutines);
            if (newElem.GetType() != typeof(S6xOperation)) arrSList.Add(sadS6x.slOperations);
            //arrSList.Add(sadS6x.slOtherAddresses);        Other address can have same address

            bConflict = false;
            foreach (SortedList sList in arrSList)
            {
                bConflict = sList.ContainsKey(uniqueAddress);
                if (bConflict) break;
            }
            arrSList = null;
            
            return bConflict;
        }

        private static string getCategNameFromS6x(object s6xElem)
        {
            if (s6xElem.GetType() == typeof(S6xStructure)) return S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES);
            if (s6xElem.GetType() == typeof(S6xTable)) return S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES);
            if (s6xElem.GetType() == typeof(S6xFunction)) return S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS);
            if (s6xElem.GetType() == typeof(S6xScalar)) return S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS);
            if (s6xElem.GetType() == typeof(S6xRoutine)) return S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES);
            if (s6xElem.GetType() == typeof(S6xOperation)) return S6xNav.getHeaderCategName(S6xNavHeaderCategory.OPERATIONS);
            if (s6xElem.GetType() == typeof(S6xRegister)) return S6xNav.getHeaderCategName(S6xNavHeaderCategory.REGISTERS);
            if (s6xElem.GetType() == typeof(S6xOtherAddress)) return S6xNav.getHeaderCategName(S6xNavHeaderCategory.OTHER);

            return string.Empty;
        }

        private static void initAllElementsArr(ref SortedList allElements, ref SADBin sadBin, ref SADS6x sadS6x)
        {
            allElements = new SortedList();

            // Banks List
            ArrayList alBankList = new ArrayList();
            if (sadBin.Bank0 != null) alBankList.Add(sadBin.Bank0);
            if (sadBin.Bank1 != null) alBankList.Add(sadBin.Bank1);
            if (sadBin.Bank8 != null) alBankList.Add(sadBin.Bank8);
            if (sadBin.Bank9 != null) alBankList.Add(sadBin.Bank9);
            foreach (SADBank Bank in alBankList)
            {
                object lastElement = null;
                string lastElementUniqueAddress = string.Empty;
                for (int iAddress = Bank.AddressInternalInt; iAddress < Bank.AddressInternalEndInt; iAddress++)
                {
                    string uAddress = Tools.UniqueAddress(Bank.Num, iAddress);
                    object newElement = null;

                    if (newElement == null) newElement = Bank.slReserved[uAddress];
                    if (sadBin.isDisassembled)
                    {
                        if (newElement == null) newElement = Bank.slOPs[uAddress];
                        if (newElement == null) newElement = Bank.slUnknownOpParts[uAddress];

                        if (newElement == null) newElement = sadBin.Calibration.slExtStructures[uAddress];
                        if (newElement == null) newElement = sadBin.Calibration.slExtTables[uAddress];
                        if (newElement == null) newElement = sadBin.Calibration.slExtFunctions[uAddress];
                        if (newElement == null) newElement = sadBin.Calibration.slExtScalars[uAddress];

                        if (newElement == null)
                        {
                            if (sadBin.Calibration.BankNum == Bank.Num)
                            {
                                if (sadBin.Calibration.isCalibrationAddress(iAddress))
                                {
                                    newElement = sadBin.Calibration.slCalibrationElements[uAddress];
                                    if (newElement == null) newElement = sadBin.Calibration.slUnknownCalibParts[uAddress];
                                }
                            }
                        }
                    }
                    else
                    {
                        if (newElement == null) newElement = sadS6x.slStructures[uAddress];
                        if (newElement == null) newElement = sadS6x.slTables[uAddress];
                        if (newElement == null) newElement = sadS6x.slFunctions[uAddress];
                        if (newElement == null) newElement = sadS6x.slScalars[uAddress];
                        if (newElement == null) newElement = sadS6x.slRoutines[uAddress];
                        if (newElement == null) newElement = sadS6x.slOperations[uAddress];
                    }
                    if (newElement == null) newElement = sadS6x.slOtherAddresses[uAddress];

                    if (newElement != null)
                    {
                        lastElement = newElement;
                        lastElementUniqueAddress = uAddress;
                    }

                    allElements.Add(uAddress, new object[] { lastElementUniqueAddress, lastElement });
                }
            }
            alBankList = null;
        }
        
        private static string getPrevElemAddress(string uniqueAddress, ref SortedList allElements, ref SADBin sadBin, ref SADS6x sadS6x)
        {
            object[] prevElemArr = getPrevElemArrByAddress(uniqueAddress, ref allElements, ref sadBin, ref sadS6x);

            if (prevElemArr == null) return null;

            return prevElemArr[0].ToString();
        }

        private static object[] getPrevElemArrByAddress(string uniqueAddress, ref SortedList allElements, ref SADBin sadBin, ref SADS6x sadS6x)
        {
            if (allElements == null) initAllElementsArr(ref allElements, ref sadBin, ref sadS6x);

            // Impossible
            if (!allElements.ContainsKey(uniqueAddress)) return null;

            int uniqueAddressIndex = allElements.IndexOfKey(uniqueAddress);
            while (true)
            {
                // First Index
                if (uniqueAddressIndex <= 0) return null;

                string prevUniqueAddress = allElements.GetKey(uniqueAddressIndex - 1).ToString();
                // Different Bank
                if (prevUniqueAddress.Substring(0, 1) != uniqueAddress.Substring(0, 1)) return null;

                object[] arrElem = (object[])allElements[prevUniqueAddress];
                if (arrElem == null) return null;
                if (arrElem[1] == null) return null;

                if (arrElem[0].ToString() != uniqueAddress) return arrElem;

                uniqueAddressIndex--;
            }
        }

        private static string getNextElemAddress(string uniqueAddress, ref SortedList allElements, ref SADBin sadBin, ref SADS6x sadS6x)
        {
            object[] nextElemArr = getNextElemArrByAddress(uniqueAddress, ref allElements, ref sadBin, ref sadS6x);

            if (nextElemArr == null) return null;
            
            return nextElemArr[0].ToString();
        }

        private static object[] getNextElemArrByAddress(string uniqueAddress, ref SortedList allElements, ref SADBin sadBin, ref SADS6x sadS6x)
        {
            if (allElements == null) initAllElementsArr(ref allElements, ref sadBin, ref sadS6x);

            // Impossible
            if (!allElements.ContainsKey(uniqueAddress)) return null;

            int uniqueAddressIndex = allElements.IndexOfKey(uniqueAddress);
            while (true)
            {
                // Last Index
                if (uniqueAddressIndex + 1 >= allElements.Count) return null;

                string nextUniqueAddress = allElements.GetKey(uniqueAddressIndex + 1).ToString();
                // Different Bank
                if (nextUniqueAddress.Substring(0, 1) != uniqueAddress.Substring(0, 1)) return null;

                object[] arrElem = (object[])allElements[nextUniqueAddress];
                if (arrElem == null) return null;
                if (arrElem[1] == null) return null;

                if (arrElem[0].ToString() != uniqueAddress) return arrElem;

                uniqueAddressIndex++;
            }
        }
        
        public static void ExportDirFile(string dirFilePath, string tfSADVersion, ref SADBin sadBin, ref SADS6x sadS6x, ref SADProcessManager sadProcessManager)
        {
            StreamWriter sWri = null;
            SortedList slElements = new SortedList();
            bool singleBank = false;
            string elemLine = string.Empty;
            string symLine = string.Empty;
            int endAddress = -1;
            string opt1 = string.Empty;
            string opt2 = string.Empty;
            string scale1 = string.Empty;
            string scale2 = string.Empty;
            CultureInfo scCi = null;

            string sFormat = string.Empty;
            string sSymFormat = string.Empty;
            string sAddSymbol = string.Empty;

            if (sadBin == null) return;
            if (sadS6x == null) return;

            if (sadProcessManager != null) sadProcessManager.ProcessProgressLabel = "Export initialization.";

            if (File.Exists(dirFilePath))
            {
                // Dir Original file automatic Backup
                try
                {
                    File.Copy(dirFilePath, dirFilePath + DateTime.Now.ToString(".yyyyMMdd.HHmmss.") + "bak", true);
                }
                catch
                {
                    throw new Exception("File backup has failed.\r\nNo other action will be managed.");
                }
            }

            try
            {
                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nWriting directives.";
                    sadProcessManager.ProcessProgressStatus = 10;
                }

                sWri = new StreamWriter(dirFilePath, false, new UTF8Encoding(false));

                sWri.WriteLine("----- Commands ------");
                if (sadBin.Calibration.Info.is8061) sWri.WriteLine("opts   :C P N S"); else sWri.WriteLine("opts   :C P N S H");

                singleBank = (sadBin.Calibration.Info.slBanksInfos.Count == 1);
                // 20200711 - PYM - Format as changed for Banks directive since version 3
                if (tfSADVersion == TFST_SAD_VER_0) sFormat = "{0} {1,4} {2,4} {3}";
                else sFormat = "{0} {3} {1,4} {2,4}";
                foreach (string[] bankInfos in sadBin.Calibration.Info.slBanksInfos.Values) sWri.WriteLine(string.Format(sFormat, "bank", bankInfos[1], bankInfos[2], bankInfos[0]));
                sWri.WriteLine();

                foreach (RBase rBase in sadBin.Calibration.slRbases.Values) sWri.WriteLine(string.Format("{0} {1} {2,4}", "rbase", rBase.Code, rBase.AddressBank));
                sWri.WriteLine();
                if (sadBin.Calibration.slRconst.Count > 0)
                {
                    foreach (RConst rConst in sadBin.Calibration.slRconst.Values) sWri.WriteLine(string.Format("{0} {1} {2,4}", "rbase", rConst.Code, rConst.Value));
                    sWri.WriteLine();
                }

                if (singleBank) sSymFormat = "{0} {1} {3,-30}";
                else if (tfSADVersion == TFST_SAD_VER_4) sSymFormat = "{0} {2}{1} {3,-30}";
                else sSymFormat = "{0} {1} {2} {3,-30}";

                sAddSymbol = "+";
                if (tfSADVersion == TFST_SAD_VER_4) sAddSymbol = string.Empty;

                scCi = new CultureInfo("en-us");
                
                slElements = new SortedList();

                foreach (S6xScalar s6xElem in sadS6x.slScalars.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format(sSymFormat, "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    endAddress = SADDef.EecBankStartAddress + s6xElem.AddressInt - 1;
                    scale1 = string.Empty;
                    if (s6xElem.ScaleExpression != null && s6xElem.ScaleExpression != string.Empty)
                    {
                        if (s6xElem.ScaleExpression.Contains("/") && s6xElem.ScaleExpression.ToLower() != "x/1")
                        {
                            try { scale1 = string.Format(scCi, "{0:0.00}", Convert.ToDouble(s6xElem.ScaleExpression.ToLower().Replace("x", "").Replace("/", ""))); }
                            catch { scale1 = string.Empty; }
                        }
                    }
                    if (s6xElem.Byte)
                    {
                        endAddress++;
                        opt1 = ":";
                        if (scale1 != string.Empty) opt1 += "V " + sAddSymbol + scale1 + " ";
                        opt1 += "P 3";
                        
                        if (singleBank) sFormat = "{0} {1} {2:x4} {4}";
                        else if (tfSADVersion == TFST_SAD_VER_4) sFormat = "{0} {3}{1} {2:x4} {4}";
                        else sFormat = "{0} {1} {2:x4} {3} {4}";

                        elemLine = string.Format(sFormat, "byte", s6xElem.Address, endAddress, s6xElem.BankNum, opt1);
                        
                        if (s6xElem.isBitFlags)
                        {
                            // Not Managed
                        }
                    }
                    else
                    {
                        endAddress += 2;
                        opt1 = ":";
                        if (scale1 != string.Empty) opt1 += "V " + sAddSymbol + scale1 + " ";
                        opt1 += "P 5";

                        if (singleBank) sFormat = "{0} {1} {2:x4} {4}";
                        else if (tfSADVersion == TFST_SAD_VER_4) sFormat = "{0} {3}{1} {2:x4} {4}";
                        else sFormat = "{0} {1} {2:x4} {3} {4}";

                        elemLine = string.Format(sFormat, "word", s6xElem.Address, endAddress, s6xElem.BankNum, opt1);
                        
                        if (s6xElem.isBitFlags)
                        {
                            // Not Managed
                        }
                    }

                    slElements.Add(s6xElem.UniqueAddress, new string[] { elemLine, symLine });
                }
                foreach (S6xFunction s6xElem in sadS6x.slFunctions.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format(sSymFormat, "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    if (s6xElem.RowsNumber > 0)
                    {
                        endAddress = SADDef.EecBankStartAddress + s6xElem.AddressInt - 1;
                        if (s6xElem.ByteInput) endAddress += s6xElem.RowsNumber; else endAddress += s6xElem.RowsNumber * 2;
                        if (s6xElem.ByteOutput) endAddress += s6xElem.RowsNumber; else endAddress += s6xElem.RowsNumber * 2;
                        scale1 = string.Empty;
                        if (s6xElem.InputScaleExpression != null && s6xElem.InputScaleExpression != string.Empty)
                        {
                            if (s6xElem.InputScaleExpression.Contains("/") && s6xElem.InputScaleExpression.ToLower() != "x/1")
                            {
                                try { scale1 = string.Format(scCi, "{0:0.00}", Convert.ToDouble(s6xElem.InputScaleExpression.ToLower().Replace("x", "").Replace("/", ""))); }
                                catch { scale1 = string.Empty; }
                            }
                        }
                        scale2 = string.Empty;
                        if (s6xElem.OutputScaleExpression != null && s6xElem.OutputScaleExpression != string.Empty)
                        {
                            if (s6xElem.OutputScaleExpression.Contains("/") && s6xElem.OutputScaleExpression.ToLower() != "x/1")
                            {
                                try { scale2 = string.Format(scCi, "{0:0.00}", Convert.ToDouble(s6xElem.OutputScaleExpression.ToLower().Replace("x", "").Replace("/", ""))); }
                                catch { scale2 = string.Empty; }
                            }
                        }

                        if (s6xElem.ByteInput) opt1 = ":Y"; else opt1 = ":W";
                        if (s6xElem.SignedInput) opt1 += " S";
                        if (scale1 != string.Empty) opt1 += " V " + sAddSymbol + scale1;
                        if (s6xElem.ByteInput) opt1 += " P 3"; else opt1 += " P 5";
                        if (s6xElem.ByteOutput) opt2 = ":Y"; else opt2 = ":W";
                        if (s6xElem.SignedOutput) opt2 += " S";
                        if (scale2 != string.Empty) opt2 += " V " + sAddSymbol + scale2;
                        if (s6xElem.ByteInput) opt2 += " P 3"; else opt2 += " P 5";

                        if (singleBank) sFormat = "{0} {1} {2:x4} {4} {5}";
                        else if (tfSADVersion == TFST_SAD_VER_4) sFormat = "{0} {3}{1} {2:x4} {4} {5}";
                        else sFormat = "{0} {1} {2:x4} {3} {4} {5}";

                        elemLine = string.Format(sFormat, "function", s6xElem.Address, endAddress, s6xElem.BankNum, opt1, opt2);
                    }

                    slElements.Add(s6xElem.UniqueAddress, new string[] { elemLine, symLine });
                }
                foreach (S6xTable s6xElem in sadS6x.slTables.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format(sSymFormat, "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    if (s6xElem.ColsNumber > 0 && s6xElem.RowsNumber > 0)
                    {
                        endAddress = SADDef.EecBankStartAddress + s6xElem.AddressInt - 1;
                        if (s6xElem.WordOutput) endAddress += s6xElem.ColsNumber * s6xElem.RowsNumber * 2; else endAddress += s6xElem.ColsNumber * s6xElem.RowsNumber;
                        scale1 = string.Empty;
                        if (s6xElem.CellsScaleExpression != null && s6xElem.CellsScaleExpression != string.Empty)
                        {
                            if (s6xElem.CellsScaleExpression.Contains("/") && s6xElem.CellsScaleExpression.ToLower() != "x/1")
                            {
                                try { scale1 = string.Format(scCi, "{0:0.00}", Convert.ToDouble(s6xElem.CellsScaleExpression.ToLower().Replace("x", "").Replace("/", ""))); }
                                catch { scale1 = string.Empty; }
                            }
                        }
                        opt1 = ":O " + sAddSymbol + s6xElem.ColsNumber.ToString();
                        if (s6xElem.WordOutput) opt1 += " W"; else opt1 += " Y";
                        if (s6xElem.SignedOutput) opt1 += " S";
                        if (scale1 != string.Empty) opt1 += " V " + sAddSymbol + scale1;
                        if (s6xElem.WordOutput) opt1 += " P 5"; else opt1 += " P 3";

                        if (singleBank) sFormat = "{0} {1} {2:x4} {4}";
                        else if (tfSADVersion == TFST_SAD_VER_4) sFormat = "{0} {3}{1} {2:x4} {4}";
                        else sFormat = "{0} {1} {2:x4} {3} {4}";

                        elemLine = string.Format(sFormat, "table", s6xElem.Address, endAddress, s6xElem.BankNum, opt1);
                    }

                    slElements.Add(s6xElem.UniqueAddress, new string[] { elemLine, symLine });
                }

                foreach (S6xStructure s6xElem in sadS6x.slStructures.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format(sSymFormat, "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    if (s6xElem.Structure == null) s6xElem.Structure = new Structure(s6xElem);
                    if (s6xElem.Structure.isValid && !s6xElem.Structure.isEmpty)
                    {
                        endAddress = SADDef.EecBankStartAddress + s6xElem.Structure.AddressEndInt;

                        if (singleBank) sFormat = "{0} {1} {2:x4} {4}";
                        else if (tfSADVersion == TFST_SAD_VER_4) sFormat = "{0} {3}{1} {2:x4} {4}";
                        else sFormat = "{0} {1} {2:x4} {3} {4}";

                        if (s6xElem.isVectorsList)
                        {
                            opt1 = "D: " + s6xElem.VectorsBankNum.ToString();
                            elemLine = string.Format(sFormat, "vect", s6xElem.Address, endAddress, s6xElem.BankNum, opt1);
                        }
                        else
                        {
                            // To Be Reviewed
                            opt1 = ":Y X O " + sAddSymbol + s6xElem.Structure.Size.ToString();
                            elemLine = string.Format(sFormat, "struct", s6xElem.Address, endAddress, s6xElem.BankNum, opt1);
                        }
                    }

                    slElements.Add(s6xElem.UniqueAddress, new string[] { elemLine, symLine });
                }

                foreach (S6xOperation s6xElem in sadS6x.slOperations.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format(sSymFormat, "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    slElements.Add(s6xElem.UniqueAddress, new string[] { elemLine, symLine });
                }

                foreach (S6xRoutine s6xElem in sadS6x.slRoutines.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.ShortLabel == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format(sSymFormat, "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.ShortLabel.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    slElements.Add(s6xElem.UniqueAddress, new string[] { elemLine, symLine });
                }

                foreach (S6xOtherAddress s6xElem in sadS6x.slOtherAddresses.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (!s6xElem.OutputLabel) continue;
                    if (s6xElem.Label == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    symLine = string.Format(sSymFormat, "sym", s6xElem.Address, s6xElem.BankNum, "\"" + s6xElem.Label.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    slElements.Add(s6xElem.UniqueAddress, new string[] { elemLine, symLine });
                }

                foreach (S6xRegister s6xElem in sadS6x.slRegisters.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.isDual) continue;   // Not Managed
                    if (s6xElem.Label == null) continue;
                    if (slElements.ContainsKey(s6xElem.UniqueAddress)) continue;

                    if (singleBank) sSymFormat = "{0} {1} {2,-30}";
                    else if (tfSADVersion == TFST_SAD_VER_4) sSymFormat = "{0} {1} {2,-30}";
                    else sSymFormat = "{0} {1} {2,-30}";

                    symLine = string.Format(sSymFormat, "sym", s6xElem.Address, "\"" + s6xElem.Label.Replace(" ", "_") + "\"").Trim();
                    elemLine = string.Empty;
                    slElements.Add(s6xElem.UniqueAddress, new string[] { elemLine, symLine });
                    if (s6xElem.isBitFlags)
                    {
                        // Not Managed
                    }
                }

                foreach (string[] lineArray in slElements.Values)
                {
                    foreach (string sLine in lineArray) if (sLine != string.Empty) sWri.WriteLine(sLine);
                    sWri.WriteLine();
                }

                if (sadProcessManager != null) sadProcessManager.SetProcessFinished("Export is done.");
            }
            catch (Exception ex)
            {
                throw new Exception("Directive file export has failed.\r\n" + ex.Message);
            }
            finally
            {
                slElements = null;

                try { sWri.Close(); }
                catch { }
                try { sWri.Dispose(); }
                catch { }
                sWri = null;

                GC.Collect();
            }
        }

        public static void ImportDirFile(string dFilePath, string tfSADVersion, ref ArrayList alNewTreeNodesInfos, ref SADBin sadBin, ref SADS6x sadS6x, ref SADProcessManager sadProcessManager)
        {
            if (dFilePath == string.Empty) return;
            if (!File.Exists(dFilePath)) return;
            if (alNewTreeNodesInfos == null) return;
            if (sadBin == null) return;
            if (sadS6x == null) return;

            // alNewTreeNodesInfos - ArrayList containing N string[] arrays
            // string[] array definition
            //  0 : Node Categ Name
            //  1 : Node Name (UniqueAddress)
            //  2 : Node ToolTipText
            //  3 : Node Text (OTHER categ only)

            StreamReader sReader = null;
            string sLine = string.Empty;
            string[] arrLine = null;
            int bankColAdder = 0;
            int bankNum = -1;
            int address = -1;
            int addressEnd = -1;
            string uniqueAddress = string.Empty;
            string sValue = string.Empty;
            int bitFlag = -1;
            object[] bitFlags = null;

            SortedList slScalars = new SortedList();
            SortedList slFunctions = new SortedList();
            SortedList slTables = new SortedList();
            SortedList slStructures = new SortedList();
            SortedList slRoutines = new SortedList();
            SortedList slOperations = new SortedList();
            SortedList slLabels = new SortedList();
            SortedList slBitFlags = new SortedList();

            try
            {
                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nReading directives.";
                    sadProcessManager.ProcessProgressStatus = 10;
                }

                sReader = File.OpenText(dFilePath);

                while (!sReader.EndOfStream)
                {
                    string sLabel = string.Empty;
                    string sUpdatedLabel = string.Empty;

                    sLine = sReader.ReadLine();
                    if (sLine != null)
                    {
                        if (sLine != string.Empty)
                        {
                            sLine = sLine.Replace(":", " ").Trim();
                            while (sLine.Contains("  ")) sLine = sLine.Replace("  ", " ");
                            sLine = sLine.Replace(" +", "+").Replace(" -", "-");
                            if (sLine.Contains("\""))
                            {
                                if (sLine.IndexOf('\"') != sLine.LastIndexOf('\"'))
                                {
                                    sLabel = sLine.Substring(sLine.IndexOf('\"'));
                                    if (sLabel.Length > 0) sLabel = sLabel.Substring(1);
                                    sLabel = sLabel.Substring(0, sLabel.LastIndexOf('\"'));
                                    sUpdatedLabel = sLabel;
                                    while (sUpdatedLabel.Contains("\"")) sUpdatedLabel = sUpdatedLabel.Replace("\"", "_");
                                    while (sUpdatedLabel.Contains(" ")) sUpdatedLabel = sUpdatedLabel.Replace(" ", "_");
                                    if (sLabel != sUpdatedLabel) sLine = sLine.Replace("\"" + sLabel + "\"", "\"" + sUpdatedLabel + "\"");
                                }
                            }
                            arrLine = sLine.Split(' ');
                            switch (arrLine[0].ToLower())
                            {
                                case "sym":
                                case "sub":
                                case "subr":
                                    try
                                    {
                                        if (tfSADVersion == TFST_SAD_VER_4)
                                        {
                                            if (arrLine[1].Length == 5)
                                            {
                                                bankColAdder = 0;
                                                bankNum = Convert.ToInt32(arrLine[1].Substring(0, 1));
                                                address = Convert.ToInt32(arrLine[1].Substring(1), 16) - SADDef.EecBankStartAddress;
                                            }
                                            else
                                            {
                                                bankColAdder = 0;
                                                bankNum = 8;
                                                address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                            }
                                        }
                                        else
                                        {
                                            if (arrLine[2].Length == 1)
                                            {
                                                bankColAdder = 1;
                                                bankNum = Convert.ToInt32(arrLine[2]);
                                                address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                            }
                                            else
                                            {
                                                bankColAdder = 0;
                                                bankNum = 8;
                                                address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                            }
                                        }
                                        // Register Case
                                        if (address < 0)
                                        {
                                            bankNum = 2;
                                            address += SADDef.EecBankStartAddress;
                                        }
                                        uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                    }
                                    catch
                                    {
                                        bankNum = -1;
                                        address = -1;
                                        uniqueAddress = string.Empty;
                                    }
                                    break;
                                case "code":
                                case "text":
                                case "byte":
                                case "word":
                                case "function":
                                case "func":
                                case "table":
                                case "struct":
                                case "vect":
                                case "vector":
                                    try
                                    {
                                        if (tfSADVersion == TFST_SAD_VER_4)
                                        {
                                            try
                                            {
                                                if (arrLine[1].Length == 5)
                                                {
                                                    bankColAdder = 0;
                                                    bankNum = Convert.ToInt32(arrLine[1].Substring(0, 1));
                                                    address = Convert.ToInt32(arrLine[1].Substring(1), 16) - SADDef.EecBankStartAddress;
                                                    addressEnd = Convert.ToInt32(arrLine[2], 16) - SADDef.EecBankStartAddress;
                                                    uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                                }
                                                else
                                                {
                                                    bankColAdder = 0;
                                                    bankNum = 8;
                                                    address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                                    addressEnd = Convert.ToInt32(arrLine[2], 16) - SADDef.EecBankStartAddress;
                                                    uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                                }
                                            }
                                            catch
                                            {
                                                if (arrLine[1].Length == 5)
                                                {
                                                    bankColAdder = 0;
                                                    bankNum = Convert.ToInt32(arrLine[1].Substring(0, 1));
                                                    address = Convert.ToInt32(arrLine[1].Substring(1), 16) - SADDef.EecBankStartAddress;
                                                    addressEnd = -1;
                                                    uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                                }
                                                else
                                                {
                                                    bankColAdder = 0;
                                                    bankNum = 8;
                                                    address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                                    addressEnd = -1;
                                                    uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                bankColAdder = 1;
                                                bankNum = Convert.ToInt32(arrLine[3]);
                                                address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                                addressEnd = Convert.ToInt32(arrLine[2], 16) - SADDef.EecBankStartAddress;
                                                uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                            }
                                            catch
                                            {
                                                if (arrLine[2].Length == 1)
                                                {
                                                    bankColAdder = 0;
                                                    bankNum = Convert.ToInt32(arrLine[2]);
                                                    address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                                    addressEnd = -1;
                                                    uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                                }
                                                else
                                                {
                                                    bankColAdder = 0;
                                                    bankNum = 8;
                                                    address = Convert.ToInt32(arrLine[1], 16) - SADDef.EecBankStartAddress;
                                                    addressEnd = Convert.ToInt32(arrLine[2], 16) - SADDef.EecBankStartAddress;
                                                    uniqueAddress = Tools.UniqueAddress(bankNum, address);
                                                }
                                            }
                                            if (addressEnd < address)
                                            {
                                                // To be managed like a Label only
                                                arrLine[0] = "sym";
                                                //bankNum = -1;
                                                //address = -1;
                                                //uniqueAddress = string.Empty;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        bankNum = -1;
                                        address = -1;
                                        uniqueAddress = string.Empty;
                                    }
                                    break;
                                default:
                                    bankNum = -1;
                                    address = -1;
                                    uniqueAddress = string.Empty;
                                    break;
                            }
                            if (bankNum >= 0 && address >= 0 && uniqueAddress != string.Empty)
                            {
                                try
                                {
                                    /*
                                    word 247e 249d :P 5
                                    SYM 247e "[15a]_[15e]_Init"

                                    function 2e20 2e29 :Y S P 3 :Y S P 2
                                    SYM 2e20 "Func9_continues"

                                    table 34d0 3520 :Y O +9 
                                    SYM 34d0 "Table51"

                                    byte 37a3 37a3 :P 3

                                    SYM 4060 "AD_Chan_Reg_4064_Ref_Somewhere"

                                    code 727e 739b
                                    SYM 727e "Unused_Code_To_739b"

                                    text 9fe0 9fff
                                    SYM 9fe0 "ASCII_Catch_Comments"
                                     
                                    SYM 4060 "AD_Chan_Reg_4064_Ref_Somewhere" :T +4
                                    */
                                    switch (arrLine[0].ToLower())
                                    {
                                        case "sym":
                                            sValue = string.Empty;
                                            bitFlag = -1;
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower().StartsWith("t"))
                                                {
                                                    bitFlag = Convert.ToInt32(arrLine[iCol].ToLower().Replace("t", "").Replace("+", ""));
                                                }
                                                else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                {
                                                    sValue = arrLine[iCol].Replace("\"", "");
                                                }
                                            }
                                            if (sValue != string.Empty)
                                            {
                                                if (bitFlag == -1)
                                                {
                                                    if (!slLabels.ContainsKey(uniqueAddress)) slLabels.Add(uniqueAddress, new object[] { bankNum, address, sValue });
                                                }
                                                else
                                                {
                                                    if (slBitFlags.ContainsKey(uniqueAddress))
                                                    {
                                                        bitFlags = new object[((object[])((object[])slBitFlags[uniqueAddress])[2]).Length + 1];
                                                        for (int bF = 0; bF < bitFlags.Length - 1; bF++) bitFlags[bF] = ((object[])((object[])slBitFlags[uniqueAddress])[2])[bF];
                                                        bitFlags[bitFlags.Length - 1] = new object[] { bitFlag, sValue };
                                                        ((object[])slBitFlags[uniqueAddress])[2] = bitFlags;
                                                        bitFlags = null;
                                                    }
                                                    else
                                                    {
                                                        slBitFlags.Add(uniqueAddress, new object[] { bankNum, address, new object[] { new object[] { bitFlag, sValue } } });
                                                    }
                                                }
                                            }
                                            break;
                                        case "code":
                                            S6xOperation ope = new S6xOperation();
                                            ope.BankNum = bankNum;
                                            ope.AddressInt = address;
                                            if (!slOperations.ContainsKey(uniqueAddress)) slOperations.Add(uniqueAddress, ope);
                                            ope = null;
                                            break;
                                        case "text":
                                            S6xStructure tStruct = new S6xStructure();
                                            tStruct.BankNum = bankNum;
                                            tStruct.AddressInt = address;
                                            tStruct.Store = true;
                                            tStruct.Number = 1;
                                            tStruct.StructDef = "Ascii:" + (addressEnd - address + 1).ToString();
                                            if (!slStructures.ContainsKey(uniqueAddress)) slStructures.Add(uniqueAddress, tStruct);
                                            tStruct = null;
                                            break;
                                        case "sub":
                                        case "subr":
                                            S6xRoutine routine = new S6xRoutine();
                                            routine.BankNum = bankNum;
                                            routine.AddressInt = address;
                                            routine.Store = true;
                                            routine.ByteArgumentsNum = 0;
                                            int iNum = 0;
                                            string lastArgType = string.Empty;
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower() == "y")
                                                // Byte
                                                {
                                                    lastArgType = "y";
                                                    iNum += 1;
                                                    routine.ByteArgumentsNumOverride = true;
                                                }
                                                else if (arrLine[iCol].ToLower() == "w" || arrLine[iCol].ToLower() == "r")
                                                // Word or Routine Address
                                                {
                                                    lastArgType = "w";
                                                    iNum += 2;
                                                    routine.ByteArgumentsNumOverride = true;
                                                }
                                                else if (arrLine[iCol].ToLower().StartsWith("o"))
                                                {
                                                    switch (lastArgType)
                                                    {
                                                        case "y":
                                                            iNum += Convert.ToInt32(arrLine[iCol].ToLower().Replace("o", "").Replace("+", "")) - 1;
                                                            routine.ByteArgumentsNumOverride = true;
                                                            break;
                                                        case "w":
                                                            iNum += (2 * Convert.ToInt32(arrLine[iCol].ToLower().Replace("o", "").Replace("+", ""))) - 2;
                                                            routine.ByteArgumentsNumOverride = true;
                                                            break;
                                                    }
                                                }
                                                else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                {
                                                    routine.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                    routine.Label = routine.ShortLabel;
                                                }
                                            }
                                            routine.ByteArgumentsNum = iNum;
                                            if (!slRoutines.ContainsKey(uniqueAddress)) slRoutines.Add(uniqueAddress, routine);
                                            routine = null;
                                            break;
                                        case "byte":
                                            for (int iByte = 0; iByte < addressEnd - address + 1; iByte++)
                                            {
                                                S6xScalar scalar = new S6xScalar();
                                                scalar.BankNum = bankNum;
                                                scalar.AddressInt = address + iByte;
                                                scalar.Store = true;
                                                scalar.Byte = true;
                                                scalar.Signed = false;
                                                scalar.ScaleExpression = "X";
                                                for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                                {
                                                    if (arrLine[iCol].ToLower() == "s") scalar.Signed = true;
                                                    else if (arrLine[iCol].ToLower().StartsWith("v")) scalar.ScaleExpression = "X/" + arrLine[iCol].ToLower().Replace("v", "").Replace("+", "");
                                                    else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                    {
                                                        scalar.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                        scalar.Label = scalar.ShortLabel;
                                                    }
                                                }
                                                if (!slScalars.ContainsKey(Tools.UniqueAddress(bankNum, address + iByte))) slScalars.Add(Tools.UniqueAddress(bankNum, address + iByte), scalar);
                                                scalar = null;
                                            }
                                            break;
                                        case "word":
                                            for (int iByte = 0; iByte < addressEnd - address + 1; iByte += 2)
                                            {
                                                S6xScalar scalar = new S6xScalar();
                                                scalar.BankNum = bankNum;
                                                scalar.AddressInt = address + iByte;
                                                scalar.Store = true;
                                                scalar.Byte = false;
                                                scalar.Signed = false;
                                                scalar.ScaleExpression = "X";
                                                for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                                {
                                                    if (arrLine[iCol].ToLower() == "s") scalar.Signed = true;
                                                    else if (arrLine[iCol].ToLower().StartsWith("v")) scalar.ScaleExpression = "X/" + arrLine[iCol].ToLower().Replace("v", "").Replace("+", "");
                                                    else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                    {
                                                        scalar.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                        scalar.Label = scalar.ShortLabel;
                                                    }
                                                }
                                                if (!slScalars.ContainsKey(Tools.UniqueAddress(bankNum, address + iByte))) slScalars.Add(Tools.UniqueAddress(bankNum, address + iByte), scalar);
                                            }
                                            break;
                                        case "function":
                                        case "func":
                                            bool inputPartStarted = false;
                                            bool inputPartEnded = false;
                                            int lineSize = -1;

                                            S6xFunction func = new S6xFunction();
                                            func.BankNum = bankNum;
                                            func.AddressInt = address;
                                            func.Store = true;
                                            func.RowsNumber = 1;
                                            func.ByteInput = true;
                                            func.ByteOutput = true;
                                            func.SignedInput = false;
                                            func.SignedOutput = false;
                                            func.InputScaleExpression = "X";
                                            func.OutputScaleExpression = "X";
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower() == "w")
                                                {
                                                    if (inputPartStarted) inputPartEnded = true;
                                                    else inputPartStarted = true;
                                                    if (inputPartEnded) func.ByteOutput = false;
                                                    else
                                                    {
                                                        func.ByteInput = false;
                                                        func.ByteOutput = false;
                                                    }
                                                }
                                                else if (arrLine[iCol].ToLower() == "y")
                                                {
                                                    if (inputPartStarted) inputPartEnded = true;
                                                    else inputPartStarted = true;
                                                    if (inputPartEnded) func.ByteOutput = false;
                                                    else
                                                    {
                                                        func.ByteInput = true;
                                                        func.ByteOutput = true;
                                                    }
                                                }
                                                else if (arrLine[iCol].ToLower() == "s")
                                                {
                                                    if (inputPartEnded) func.SignedOutput = true;
                                                    else
                                                    {
                                                        func.SignedInput = true;
                                                        func.SignedOutput = true;
                                                    }
                                                }
                                                else if (arrLine[iCol].ToLower().StartsWith("v"))
                                                {
                                                    if (inputPartEnded) func.OutputScaleExpression = "X/" + arrLine[iCol].ToLower().Replace("v", "").Replace("+", "");
                                                    else func.InputScaleExpression = "X/" + arrLine[iCol].ToLower().Replace("v", "").Replace("+", "");

                                                }
                                                else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                {
                                                    func.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                    func.Label = func.ShortLabel;
                                                }
                                            }

                                            lineSize = 2;
                                            if (!func.ByteInput) lineSize++;
                                            if (!func.ByteOutput) lineSize++;
                                            if ((addressEnd - address + 1) % lineSize == 0) func.RowsNumber = (addressEnd - address + 1) / lineSize;
                                            else func.RowsNumber = (addressEnd - address + 1 - ((addressEnd - address + 1) % lineSize)) / lineSize;

                                            if (func.RowsNumber > 0)
                                            {
                                                if (!slFunctions.ContainsKey(uniqueAddress)) slFunctions.Add(uniqueAddress, func);
                                            }
                                            else if (!slLabels.ContainsKey(uniqueAddress) && func.ShortLabel != null && func.ShortLabel != string.Empty)
                                            {
                                                slLabels.Add(uniqueAddress, new object[] { bankNum, address, func.ShortLabel });
                                            }
                                            break;
                                        case "table":
                                            S6xTable table = new S6xTable();
                                            table.BankNum = bankNum;
                                            table.AddressInt = address;
                                            table.Store = true;
                                            table.ColsNumber = -1;
                                            table.SignedOutput = false;
                                            table.CellsScaleExpression = "X";
                                            table.ColsScalerAddress = string.Empty;
                                            table.RowsScalerAddress = string.Empty;
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower().StartsWith("o")) table.ColsNumber = Convert.ToInt32(arrLine[iCol].ToLower().Replace("o", "").Replace("+", ""));
                                                else if (arrLine[iCol].ToLower() == "s") table.SignedOutput = true;
                                                else if (arrLine[iCol].ToLower().StartsWith("v")) table.CellsScaleExpression = "X/" + arrLine[iCol].ToLower().Replace("v", "").Replace("+", "");
                                                else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                {
                                                    table.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                    table.Label = table.ShortLabel;
                                                }
                                            }
                                            if (table.ColsNumber > 0)
                                            {
                                                if ((addressEnd - address + 1) % table.ColsNumber == 0) table.RowsNumber = (addressEnd - address + 1) / table.ColsNumber;
                                                else table.RowsNumber = (addressEnd - address + 1 - ((addressEnd - address + 1) % table.ColsNumber)) / table.ColsNumber;
                                            }
                                            if (table.ColsNumber > 0 && table.RowsNumber > 0)
                                            {
                                                if (!slTables.ContainsKey(uniqueAddress)) slTables.Add(uniqueAddress, table);
                                            }
                                            else if (!slLabels.ContainsKey(uniqueAddress) && table.ShortLabel != null && table.ShortLabel != string.Empty)
                                            {
                                                slLabels.Add(uniqueAddress, new object[] { bankNum, address, table.ShortLabel });
                                            }
                                            break;
                                        case "struct":
                                            S6xStructure sStruct = new S6xStructure();
                                            sStruct.BankNum = bankNum;
                                            sStruct.AddressInt = address;
                                            sStruct.Store = true;
                                            sStruct.Number = 1; // To Be Recalculated
                                            sStruct.StructDef = string.Empty;
                                            int iSize = 0;
                                            string lastDataType = string.Empty;
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower() == "r")
                                                // Routine Pointer managed as Word
                                                {
                                                    lastDataType = "r";
                                                    iSize += 2;
                                                    if (sStruct.StructDef != string.Empty) sStruct.StructDef += ",";
                                                    sStruct.StructDef += "Word";
                                                }
                                                else if (arrLine[iCol].ToLower() == "y")
                                                // Byte
                                                {
                                                    lastDataType = "y";
                                                    iSize += 1;
                                                    if (sStruct.StructDef != string.Empty) sStruct.StructDef += ",";
                                                    sStruct.StructDef += "Byte";
                                                }
                                                else if (arrLine[iCol].ToLower() == "w")
                                                // Word
                                                {
                                                    lastDataType = "w";
                                                    iSize += 2;
                                                    if (sStruct.StructDef != string.Empty) sStruct.StructDef += ",";
                                                    sStruct.StructDef += "Word";
                                                }
                                                else if (arrLine[iCol].ToLower() == "x")
                                                {
                                                    switch (lastDataType)
                                                    {
                                                        case "y":
                                                            sStruct.StructDef = sStruct.StructDef.Substring(0, sStruct.StructDef.LastIndexOf("Byte")) + sStruct.StructDef.Substring(sStruct.StructDef.LastIndexOf("Byte")).Replace("Byte", "ByteHex");
                                                            break;
                                                        case "w":
                                                        case "r":
                                                            sStruct.StructDef = sStruct.StructDef.Substring(0, sStruct.StructDef.LastIndexOf("Word")) + sStruct.StructDef.Substring(sStruct.StructDef.LastIndexOf("Word")).Replace("Word", "WordHex");
                                                            break;
                                                    }
                                                }
                                                else if (arrLine[iCol].ToLower() == "s")
                                                {
                                                    switch (lastDataType)
                                                    {
                                                        case "y":
                                                            sStruct.StructDef = sStruct.StructDef.Substring(0, sStruct.StructDef.LastIndexOf("Byte")) + sStruct.StructDef.Substring(sStruct.StructDef.LastIndexOf("Byte")).Replace("Byte", "SByte");
                                                            break;
                                                        case "w":
                                                        case "r":
                                                            sStruct.StructDef = sStruct.StructDef.Substring(0, sStruct.StructDef.LastIndexOf("Word")) + sStruct.StructDef.Substring(sStruct.StructDef.LastIndexOf("Word")).Replace("Word", "SWord");
                                                            break;
                                                    }
                                                }
                                                else if (arrLine[iCol].ToLower().StartsWith("o"))
                                                {
                                                    sStruct.StructDef += arrLine[iCol].ToLower().Replace("o", "").Replace("+", ":");
                                                    switch (lastDataType)
                                                    {
                                                        case "y":
                                                            iSize += Convert.ToInt32(arrLine[iCol].ToLower().Replace("o", "").Replace("+", "")) - 1;
                                                            break;
                                                        case "w":
                                                        case "r":
                                                            iSize += (2 * Convert.ToInt32(arrLine[iCol].ToLower().Replace("o", "").Replace("+", ""))) - 2;
                                                            break;
                                                    }
                                                }
                                                else if (arrLine[iCol].StartsWith("\"") && arrLine[iCol].EndsWith("\""))
                                                {
                                                    sStruct.ShortLabel = arrLine[iCol].Replace("\"", "");
                                                    sStruct.Label = sStruct.ShortLabel;
                                                }
                                            }
                                            if (iSize > 0)
                                            {
                                                if ((addressEnd - address + 1) % iSize == 0) sStruct.Number = (addressEnd - address + 1) / iSize;
                                                else sStruct.Number = (addressEnd - address + 1 - ((addressEnd - address + 1) % iSize)) / iSize;
                                            }
                                            if (!slStructures.ContainsKey(uniqueAddress)) slStructures.Add(uniqueAddress, sStruct);
                                            sStruct = null;
                                            break;
                                        case "vect":
                                        case "vector":
                                            S6xStructure vList = new S6xStructure();
                                            vList.BankNum = bankNum;
                                            vList.AddressInt = address;
                                            vList.Number = (addressEnd + 1 - address) / 2;
                                            vList.Store = true;
                                            vList.StructDef = string.Empty;
                                            for (int iCol = 2 + bankColAdder; iCol < arrLine.Length; iCol++)
                                            {
                                                if (arrLine[iCol].ToLower().StartsWith("d"))
                                                {
                                                    vList.StructDef = "Vect" + arrLine[iCol].ToLower().Replace("d", "");
                                                    break;
                                                }
                                            }
                                            if (bankColAdder == 0) vList.StructDef = "Vect8";

                                            if (vList.StructDef != string.Empty && vList.Number * 2 == addressEnd + 1 - address)
                                            {
                                                if (!slStructures.ContainsKey(vList.UniqueAddress)) slStructures.Add(vList.UniqueAddress, vList);
                                            }
                                            vList = null;
                                            break;
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }

                arrLine = null;
                try { sReader.Close(); }
                catch { }
                try { sReader.Dispose(); }
                catch { }
                sReader = null;

                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nDirectives loaded.\r\nGenerating updates.";
                    sadProcessManager.ProcessProgressStatus = 40;
                }

                string defaultNodeToolTipText = "SAD Directive Import";
                
                int iCount = 1;
                foreach (S6xScalar s6xObject in slScalars.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slScalars.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = SADDef.ShortScalarPrefix + "ScSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }
                    if (sadBin != null)
                    {
                        s6xObject.AddressBinInt = s6xObject.AddressInt + sadBin.getBankBinAddress(s6xObject.BankNum);
                        if (sadBin.Calibration != null)
                        {
                            s6xObject.isCalibrationElement = (s6xObject.AddressBinInt >= sadBin.Calibration.AddressBinInt && s6xObject.AddressBinInt <= sadBin.Calibration.AddressBinEndInt);
                        }
                    }

                    if (!isTypeConflict(s6xObject, ref sadBin, ref sadS6x))
                    {
                        sadS6x.isSaved = false;
                        if (sadS6x.slScalars.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slScalars[s6xObject.UniqueAddress] = s6xObject;
                        else sadS6x.slScalars.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), s6xObject.UniqueAddress, s6xObject.Label, defaultNodeToolTipText });
                    }
                }

                iCount = 1;
                foreach (S6xFunction s6xObject in slFunctions.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slFunctions.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = SADDef.ShortFunctionPrefix + "FcSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }
                    if (sadBin != null)
                    {
                        s6xObject.AddressBinInt = s6xObject.AddressInt + sadBin.getBankBinAddress(s6xObject.BankNum);
                        if (sadBin.Calibration != null)
                        {
                            s6xObject.isCalibrationElement = (s6xObject.AddressBinInt >= sadBin.Calibration.AddressBinInt && s6xObject.AddressBinInt <= sadBin.Calibration.AddressBinEndInt);
                        }
                    }

                    if (!isTypeConflict(s6xObject, ref sadBin, ref sadS6x))
                    {
                        sadS6x.isSaved = false;
                        if (sadS6x.slFunctions.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slFunctions[s6xObject.UniqueAddress] = s6xObject;
                        else sadS6x.slFunctions.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), s6xObject.UniqueAddress, s6xObject.Label, defaultNodeToolTipText });
                    }
                }

                iCount = 1;
                foreach (S6xTable s6xObject in slTables.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slTables.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = SADDef.ShortTablePrefix + "TbSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }
                    if (sadBin != null)
                    {
                        s6xObject.AddressBinInt = s6xObject.AddressInt + sadBin.getBankBinAddress(s6xObject.BankNum);
                        if (sadBin.Calibration != null)
                        {
                            s6xObject.isCalibrationElement = (s6xObject.AddressBinInt >= sadBin.Calibration.AddressBinInt && s6xObject.AddressBinInt <= sadBin.Calibration.AddressBinEndInt);
                        }
                    }

                    if (!isTypeConflict(s6xObject, ref sadBin, ref sadS6x))
                    {
                        sadS6x.isSaved = false;
                        if (sadS6x.slTables.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slTables[s6xObject.UniqueAddress] = s6xObject;
                        else sadS6x.slTables.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), s6xObject.UniqueAddress, s6xObject.Label, defaultNodeToolTipText });
                    }
                }

                iCount = 1;
                foreach (S6xStructure s6xObject in slStructures.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slStructures.ContainsKey(s6xObject.UniqueAddress))
                    {
                        if (s6xObject.isVectorsList) s6xObject.ShortLabel = SADDef.ShortStructurePrefix + "VlSadDir" + string.Format("{0:d3}", iCount);
                        else s6xObject.ShortLabel = SADDef.ShortStructurePrefix + "StSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }
                    if (sadBin != null)
                    {
                        s6xObject.AddressBinInt = s6xObject.AddressInt + sadBin.getBankBinAddress(s6xObject.BankNum);
                    }

                    if (!isTypeConflict(s6xObject, ref sadBin, ref sadS6x))
                    {
                        sadS6x.isSaved = false;
                        if (sadS6x.slStructures.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slStructures[s6xObject.UniqueAddress] = s6xObject;
                        else sadS6x.slStructures.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), s6xObject.UniqueAddress, s6xObject.Label, defaultNodeToolTipText });
                    }
                }

                iCount = 1;
                foreach (S6xRoutine s6xObject in slRoutines.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = "RtSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }

                    if (!isTypeConflict(s6xObject, ref sadBin, ref sadS6x))
                    {
                        sadS6x.isSaved = false;
                        // No direct override, using only Labels and ByteArgumentsNum when provided
                        if (sadS6x.slRoutines.ContainsKey(s6xObject.UniqueAddress))
                        {
                            S6xRoutine existingRoutine = (S6xRoutine)sadS6x.slRoutines[s6xObject.UniqueAddress];
                            if (s6xObject.ByteArgumentsNumOverride)
                            {
                                existingRoutine.ByteArgumentsNumOverride = s6xObject.ByteArgumentsNumOverride;
                                existingRoutine.ByteArgumentsNum = s6xObject.ByteArgumentsNum;
                            }
                            existingRoutine.ShortLabel = s6xObject.ShortLabel;
                            existingRoutine.Label = s6xObject.Label;
                            existingRoutine = null;
                        }
                        else sadS6x.slRoutines.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), s6xObject.UniqueAddress, s6xObject.Label, defaultNodeToolTipText });
                    }
                }

                iCount = 1;
                foreach (S6xOperation s6xObject in slOperations.Values)
                {
                    iCount++;
                    if (slLabels.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = ((object[])slLabels[s6xObject.UniqueAddress])[2].ToString();
                        s6xObject.Label = s6xObject.ShortLabel;
                        slLabels.Remove(s6xObject.UniqueAddress);
                    }
                    // ShortLabel generated for new items only
                    if (s6xObject.ShortLabel == null && !sadS6x.slOperations.ContainsKey(s6xObject.UniqueAddress))
                    {
                        s6xObject.ShortLabel = "OpSadDir" + string.Format("{0:d3}", iCount);
                        s6xObject.Label = s6xObject.ShortLabel;
                    }

                    if (!isTypeConflict(s6xObject, ref sadBin, ref sadS6x))
                    {
                        sadS6x.isSaved = false;
                        if (sadS6x.slOperations.ContainsKey(s6xObject.UniqueAddress)) sadS6x.slOperations[s6xObject.UniqueAddress] = s6xObject;
                        else sadS6x.slOperations.Add(s6xObject.UniqueAddress, s6xObject);
                        alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), s6xObject.UniqueAddress, s6xObject.Label, defaultNodeToolTipText });
                    }
                }

                // Remaining Labels created as Other Addresses when Address, Register when Register or Update existing objects
                foreach (object[] oLabel in slLabels.Values)
                {
                    if ((int)oLabel[0] == 2)
                    // Bank Num was set to 2 to identify short addresses (<0x2000) and to manage them as Registers
                    {
                        S6xRegister s6xReg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress((int)oLabel[1])];
                        if (s6xReg == null)
                        {
                            s6xReg = new S6xRegister((int)oLabel[1]);
                            s6xReg.Comments = string.Empty;
                        }
                        s6xReg.Label = oLabel[2].ToString();
                        s6xReg.Store = true;
                        if (sadS6x.slRegisters.ContainsKey(s6xReg.UniqueAddress)) sadS6x.slRegisters[s6xReg.UniqueAddress] = s6xReg;
                        else sadS6x.slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                        alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xReg), s6xReg.UniqueAddress, s6xReg.Label, defaultNodeToolTipText });
                        s6xReg = null;
                    }
                    else
                    {
                        object s6xObject = null;
                        uniqueAddress = Tools.UniqueAddress((int)oLabel[0], (int)oLabel[1]);
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slScalars[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xScalar)s6xObject).Label = oLabel[2].ToString();
                                ((S6xScalar)s6xObject).ShortLabel = oLabel[2].ToString();
                                ((S6xScalar)s6xObject).Store = true;
                                alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), uniqueAddress, oLabel[2].ToString(), defaultNodeToolTipText });
                            }
                        }
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slFunctions[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xFunction)s6xObject).Label = oLabel[2].ToString();
                                ((S6xFunction)s6xObject).ShortLabel = oLabel[2].ToString();
                                ((S6xFunction)s6xObject).Store = true;
                                alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), uniqueAddress, oLabel[2].ToString(), defaultNodeToolTipText });
                            }
                        }
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slTables[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xTable)s6xObject).Label = oLabel[2].ToString();
                                ((S6xTable)s6xObject).ShortLabel = oLabel[2].ToString();
                                ((S6xTable)s6xObject).Store = true;
                                alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), uniqueAddress, oLabel[2].ToString(), defaultNodeToolTipText });
                            }
                        }
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slStructures[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xStructure)s6xObject).Label = oLabel[2].ToString();
                                ((S6xStructure)s6xObject).ShortLabel = oLabel[2].ToString();
                                ((S6xStructure)s6xObject).Store = true;
                                alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), uniqueAddress, oLabel[2].ToString(), defaultNodeToolTipText });
                            }
                        }
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slRoutines[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xRoutine)s6xObject).Label = oLabel[2].ToString();
                                ((S6xRoutine)s6xObject).ShortLabel = oLabel[2].ToString();
                                ((S6xRoutine)s6xObject).Store = true;
                                alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), uniqueAddress, oLabel[2].ToString(), defaultNodeToolTipText });
                            }
                        }
                        if (s6xObject == null)
                        {
                            s6xObject = sadS6x.slOperations[uniqueAddress];
                            if (s6xObject != null)
                            {
                                ((S6xOperation)s6xObject).Label = oLabel[2].ToString();
                                ((S6xOperation)s6xObject).ShortLabel = oLabel[2].ToString();
                                alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xObject), uniqueAddress, oLabel[2].ToString(), defaultNodeToolTipText });
                            }
                        }
                        if (s6xObject == null)
                        {
                            S6xOtherAddress other = new S6xOtherAddress();
                            other.BankNum = (int)oLabel[0];
                            other.AddressInt = (int)oLabel[1];
                            other.Label = oLabel[2].ToString();
                            other.OutputLabel = true;
                            other.Comments = string.Empty;
                            other.InlineComments = false;
                            other.OutputComments = false;
                            if (sadS6x.slOtherAddresses.ContainsKey(other.UniqueAddress))
                            {
                                ((S6xOtherAddress)sadS6x.slOtherAddresses[other.UniqueAddress]).Label = other.Label;
                            }
                            else
                            {
                                sadS6x.slOtherAddresses.Add(other.UniqueAddress, other);
                            }
                            alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(other), uniqueAddress, oLabel[2].ToString(), defaultNodeToolTipText });
                            other = null;
                        }
                        s6xObject = null;
                    }
                    sadS6x.isSaved = false;
                }

                // Scalars / Registers Bit Flags
                foreach (object[] bFs in slBitFlags.Values)
                {
                    S6xScalar s6xScalar = (S6xScalar)sadS6x.slScalars[Tools.UniqueAddress((int)bFs[0], (int)bFs[1])];
                    if (s6xScalar != null)
                    {
                        bitFlags = (object[])bFs[2];
                        foreach (object[] bF in bitFlags)
                        {
                            S6xBitFlag s6xBitFlag = new S6xBitFlag();
                            s6xBitFlag.Position = (int)bF[0];
                            if ((s6xScalar.Byte && s6xBitFlag.Position > 7) || s6xBitFlag.Position > 15)
                            {
                                s6xBitFlag = null;
                                continue;
                            }
                            s6xBitFlag.ShortLabel = bF[1].ToString();
                            string label = bF[1].ToString();
                            string setValue = "1";
                            string notSetValue = "0";
                            // Not overriding specific existing S6x Information
                            if (s6xScalar.BitFlags != null)
                            {
                                foreach (S6xBitFlag s6xBF in s6xScalar.BitFlags)
                                {
                                    if (s6xBF.Position == s6xBitFlag.Position)
                                    {
                                        label = s6xBF.Label;
                                        setValue = s6xBF.SetValue;
                                        notSetValue = s6xBF.NotSetValue;
                                        break;
                                    }
                                }
                            }
                            s6xBitFlag.Label = label;
                            s6xBitFlag.SetValue = setValue;
                            s6xBitFlag.NotSetValue = notSetValue;
                            s6xScalar.AddBitFlag(s6xBitFlag);
                            s6xBitFlag = null;

                            sadS6x.isSaved = false;
                            alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xScalar), s6xScalar.UniqueAddress, s6xScalar.Label, defaultNodeToolTipText });
                        }
                        bitFlags = null;

                        s6xScalar = null;

                        continue;
                    }

                    if ((int)bFs[0] == 2)
                    // Bank Num was set to 2 to identify short addresses (<0x2000) and to manage them as Registers
                    {
                        S6xRegister s6xReg = (S6xRegister)sadS6x.slRegisters[Tools.RegisterUniqueAddress((int)bFs[1])];
                        if (s6xReg == null)
                        // Register Creation based on BitFlag Information
                        {
                            s6xReg = new S6xRegister((int)bFs[1]);
                            sadS6x.slRegisters.Add(s6xReg.UniqueAddress, s6xReg);
                            s6xReg.Comments = string.Empty;
                            s6xReg.Label = Tools.RegisterInstruction(s6xReg.Address);
                            s6xReg.Store = true;

                            alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xReg), s6xReg.UniqueAddress, s6xReg.Label, defaultNodeToolTipText });
                        }

                        bitFlags = (object[])bFs[2];
                        foreach (object[] bF in bitFlags)
                        {
                            S6xBitFlag s6xBitFlag = new S6xBitFlag();
                            s6xBitFlag.Position = (int)bF[0];
                            if (s6xBitFlag.Position > 15)
                            {
                                s6xBitFlag = null;
                                continue;
                            }
                            s6xBitFlag.ShortLabel = bF[1].ToString();
                            string label = bF[1].ToString();
                            string setValue = "1";
                            string notSetValue = "0";
                            // Not overriding specific existing S6x Information
                            if (s6xReg.BitFlags != null)
                            {
                                foreach (S6xBitFlag s6xBF in s6xReg.BitFlags)
                                {
                                    if (s6xBF.Position == s6xBitFlag.Position)
                                    {
                                        label = s6xBF.Label;
                                        setValue = s6xBF.SetValue;
                                        notSetValue = s6xBF.NotSetValue;
                                        break;
                                    }
                                }
                            }
                            s6xBitFlag.Label = label;
                            s6xBitFlag.SetValue = setValue;
                            s6xBitFlag.NotSetValue = notSetValue;
                            s6xReg.AddBitFlag(s6xBitFlag);
                            s6xBitFlag = null;

                            sadS6x.isSaved = false;
                            alNewTreeNodesInfos.Add(new string[] { getCategNameFromS6x(s6xReg), s6xReg.UniqueAddress, s6xReg.Label, defaultNodeToolTipText });
                        }
                        bitFlags = null;

                        s6xReg = null;

                        continue;
                    }
                }

                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nDirectives loaded.\r\nApplying updates.";
                    sadProcessManager.ProcessProgressStatus = 90;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Directive file import has failed.\r\n" + ex.Message);
            }
            finally
            {
                arrLine = null;
                try { sReader.Close(); }
                catch { }
                try { sReader.Dispose(); }
                catch { }
                sReader = null;

                slScalars = null;
                slFunctions = null;
                slTables = null;
                slStructures = null;
                slRoutines = null;
                slOperations = null;
                slLabels = null;
                slBitFlags = null;

                GC.Collect();
            }
        }

        public static void ExportCmtFile(string cmtFilePath, string tfSADVersion, ref SADBin sadBin, ref SADS6x sadS6x, ref SADProcessManager sadProcessManager)
        {
            StreamWriter sWri = null;
            SortedList slComments = null;
            SortedList slAllElements = null;
            bool singleBank = false;

            if (sadBin == null) return;
            if (sadS6x == null) return;

            if (sadProcessManager != null) sadProcessManager.ProcessProgressLabel = "Export initialization.";

            if (File.Exists(cmtFilePath))
            {
                // Dir Original file automatic Backup
                try
                {
                    File.Copy(cmtFilePath, cmtFilePath + DateTime.Now.ToString(".yyyyMMdd.HHmmss.") + "bak", true);
                }
                catch
                {
                    throw new Exception("File backup has failed.\r\nNo other action will be managed.");
                }
            }

            SettingsLst sadSettings = (SettingsLst)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.settingsSADImpExpFileName, typeof(SettingsLst));
            if (sadSettings == null) sadSettings = new SettingsLst();
            ToolsSettings.Update(ref sadSettings, "SADIMPEXP");

            string setAddressShortcut = "1";
            string setNewLineChar = "|";
            string setNewLineInLineChar = "^";
            string setStartString = "#";
            string setStartStringAlt = "//";
            string setImportHeaderToRepChar = "#";
            string setImportHeaderRepChar = " ";
            string setExportHeaderToRepChar = " ";
            string setExportHeaderRepChar = "#";

            setAddressShortcut = sadSettings.Get<string>("COMMENTS_ADDRESS_SHORTCUT", setAddressShortcut);
            setNewLineChar = sadSettings.Get<string>("COMMENTS_NEW_LINE_CHAR", setNewLineChar);
            setNewLineInLineChar = sadSettings.Get<string>("COMMENTS_INLINE_NEW_LINE_CHAR", setNewLineInLineChar);
            setStartString = sadSettings.Get<string>("COMMENTS_START_STRING", setStartString);
            setStartStringAlt = sadSettings.Get<string>("COMMENTS_START_STRING_ALT", setStartStringAlt);
            setImportHeaderToRepChar = sadSettings.Get<string>("COMMENTS_IMPORT_HEADER_CHAR_TO_REPLACE", setImportHeaderToRepChar);
            setImportHeaderRepChar = sadSettings.Get<string>("COMMENTS_IMPORT_HEADER_CHAR_REPLACEMENT", setImportHeaderRepChar);
            setExportHeaderToRepChar = sadSettings.Get<string>("COMMENTS_IMPORT_HEADER_CHAR_TO_REPLACE", setExportHeaderToRepChar);
            setExportHeaderRepChar = sadSettings.Get<string>("COMMENTS_IMPORT_HEADER_CHAR_REPLACEMENT", setExportHeaderRepChar);

            if (setNewLineChar.Length != 1) setNewLineChar = "|";
            if (setNewLineInLineChar.Length != 1) setNewLineInLineChar = "^";

            try
            {
                // Time consuming process
                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nAddresses initialization.";
                    sadProcessManager.ProcessProgressStatus = 10;
                }
                initAllElementsArr(ref slAllElements, ref sadBin, ref sadS6x);
                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nAddresses initialized.\r\nSelecting comments.";
                    sadProcessManager.ProcessProgressStatus = 80;
                }
                
                slComments = new SortedList();
                
                // Global Header
                if (sadS6x.Properties.OutputHeader && sadS6x.Properties.Header != null && sadS6x.Properties.Header != string.Empty)
                {
                    ArrayList alCommentsDefsGHeader = new ArrayList();
                    alCommentsDefsGHeader.Add(new object[] { -1, -1, sadS6x.Properties.Header, -1, -1 });
                    slComments.Add("0", alCommentsDefsGHeader);
                    alCommentsDefsGHeader = null;
                }

                foreach (S6xScalar s6xElem in sadS6x.slScalars.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.OutputComments == false) continue;
                    string sComments = Tools.CommentsFirstLineShortLabelLabelRemoval(s6xElem.Comments, s6xElem.ShortLabel, s6xElem.Label);
                    if (s6xElem.Comments == null || s6xElem.Comments == string.Empty) continue;

                    string previousUniqueAddress = string.Empty;
                    int previousBankNum = -1;
                    int previousAddress = -1;
                    if (!s6xElem.InlineComments)
                    {
                        previousUniqueAddress = getPrevElemAddress(s6xElem.UniqueAddress, ref slAllElements, ref sadBin, ref sadS6x);
                        if (previousUniqueAddress == null || previousUniqueAddress == string.Empty) continue;
                        previousBankNum = Convert.ToInt32(previousUniqueAddress.Substring(0, 1));
                        previousAddress = Convert.ToInt32(previousUniqueAddress.Substring(1).Trim());
                    }

                    ArrayList alCommentsDefs = (ArrayList)slComments[s6xElem.UniqueAddress];
                    if (alCommentsDefs == null)
                    {
                        alCommentsDefs = new ArrayList();
                        slComments.Add(s6xElem.UniqueAddress, alCommentsDefs);
                    }
                    alCommentsDefs.Add(new object[] { s6xElem.BankNum, s6xElem.AddressInt, sComments, previousBankNum, previousAddress });
                }
                foreach (S6xFunction s6xElem in sadS6x.slFunctions.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.OutputComments == false) continue;

                    string sComments = Tools.CommentsFirstLineShortLabelLabelRemoval(s6xElem.Comments, s6xElem.ShortLabel, s6xElem.Label);
                    if (s6xElem.Comments == null || s6xElem.Comments == string.Empty) continue;

                    int previousBankNum = -1;
                    int previousAddress = -1;
                    string previousUniqueAddress = getPrevElemAddress(s6xElem.UniqueAddress, ref slAllElements, ref sadBin, ref sadS6x);
                    if (previousUniqueAddress == null || previousUniqueAddress == string.Empty) continue;
                    previousBankNum = Convert.ToInt32(previousUniqueAddress.Substring(0, 1));
                    previousAddress = Convert.ToInt32(previousUniqueAddress.Substring(1).Trim());

                    ArrayList alCommentsDefs = (ArrayList)slComments[s6xElem.UniqueAddress];
                    if (alCommentsDefs == null)
                    {
                        alCommentsDefs = new ArrayList();
                        slComments.Add(s6xElem.UniqueAddress, alCommentsDefs);
                    }
                    alCommentsDefs.Add(new object[] { s6xElem.BankNum, s6xElem.AddressInt, sComments, previousBankNum, previousAddress });
                }
                foreach (S6xTable s6xElem in sadS6x.slTables.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.OutputComments == false) continue;

                    string sComments = Tools.CommentsFirstLineShortLabelLabelRemoval(s6xElem.Comments, s6xElem.ShortLabel, s6xElem.Label);
                    if (s6xElem.Comments == null || s6xElem.Comments == string.Empty) continue;

                    int previousBankNum = -1;
                    int previousAddress = -1;
                    string previousUniqueAddress = getPrevElemAddress(s6xElem.UniqueAddress, ref slAllElements, ref sadBin, ref sadS6x);
                    if (previousUniqueAddress == null || previousUniqueAddress == string.Empty) continue;
                    previousBankNum = Convert.ToInt32(previousUniqueAddress.Substring(0, 1));
                    previousAddress = Convert.ToInt32(previousUniqueAddress.Substring(1).Trim());

                    ArrayList alCommentsDefs = (ArrayList)slComments[s6xElem.UniqueAddress];
                    if (alCommentsDefs == null)
                    {
                        alCommentsDefs = new ArrayList();
                        slComments.Add(s6xElem.UniqueAddress, alCommentsDefs);
                    }
                    alCommentsDefs.Add(new object[] { s6xElem.BankNum, s6xElem.AddressInt, sComments, previousBankNum, previousAddress });
                }

                foreach (S6xStructure s6xElem in sadS6x.slStructures.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.OutputComments == false) continue;

                    string sComments = Tools.CommentsFirstLineShortLabelLabelRemoval(s6xElem.Comments, s6xElem.ShortLabel, s6xElem.Label);
                    if (s6xElem.Comments == null || s6xElem.Comments == string.Empty) continue;

                    int previousBankNum = -1;
                    int previousAddress = -1;
                    string previousUniqueAddress = getPrevElemAddress(s6xElem.UniqueAddress, ref slAllElements, ref sadBin, ref sadS6x);
                    if (previousUniqueAddress == null || previousUniqueAddress == string.Empty) continue;
                    previousBankNum = Convert.ToInt32(previousUniqueAddress.Substring(0, 1));
                    previousAddress = Convert.ToInt32(previousUniqueAddress.Substring(1).Trim());

                    ArrayList alCommentsDefs = (ArrayList)slComments[s6xElem.UniqueAddress];
                    if (alCommentsDefs == null)
                    {
                        alCommentsDefs = new ArrayList();
                        slComments.Add(s6xElem.UniqueAddress, alCommentsDefs);
                    }
                    alCommentsDefs.Add(new object[] { s6xElem.BankNum, s6xElem.AddressInt, sComments, previousBankNum, previousAddress });
                }

                foreach (S6xOperation s6xElem in sadS6x.slOperations.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.OutputComments == false) continue;

                    string sComments = Tools.CommentsFirstLineShortLabelLabelRemoval(s6xElem.Comments, s6xElem.ShortLabel, s6xElem.Label);
                    if (s6xElem.Comments == null || s6xElem.Comments == string.Empty) continue;

                    int previousBankNum = -1;
                    int previousAddress = -1;
                    string previousUniqueAddress = string.Empty;
                    if (!s6xElem.InlineComments)
                    {
                        previousUniqueAddress = getPrevElemAddress(s6xElem.UniqueAddress, ref slAllElements, ref sadBin, ref sadS6x);
                        if (previousUniqueAddress == null || previousUniqueAddress == string.Empty) continue;
                        previousBankNum = Convert.ToInt32(previousUniqueAddress.Substring(0, 1));
                        previousAddress = Convert.ToInt32(previousUniqueAddress.Substring(1).Trim());
                    }

                    ArrayList alCommentsDefs = (ArrayList)slComments[s6xElem.UniqueAddress];
                    if (alCommentsDefs == null)
                    {
                        alCommentsDefs = new ArrayList();
                        slComments.Add(s6xElem.UniqueAddress, alCommentsDefs);
                    }
                    alCommentsDefs.Add(new object[] { s6xElem.BankNum, s6xElem.AddressInt, sComments, previousBankNum, previousAddress });
                }

                foreach (S6xRoutine s6xElem in sadS6x.slRoutines.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.OutputComments == false) continue;

                    string sComments = Tools.CommentsFirstLineShortLabelLabelRemoval(s6xElem.Comments, s6xElem.ShortLabel, s6xElem.Label);
                    if (s6xElem.Comments == null || s6xElem.Comments == string.Empty) continue;

                    int previousBankNum = -1;
                    int previousAddress = -1;
                    string previousUniqueAddress = getPrevElemAddress(s6xElem.UniqueAddress, ref slAllElements, ref sadBin, ref sadS6x);
                    if (previousUniqueAddress == null || previousUniqueAddress == string.Empty) continue;
                    previousBankNum = Convert.ToInt32(previousUniqueAddress.Substring(0, 1));
                    previousAddress = Convert.ToInt32(previousUniqueAddress.Substring(1).Trim());

                    ArrayList alCommentsDefs = (ArrayList)slComments[s6xElem.UniqueAddress];
                    if (alCommentsDefs == null)
                    {
                        alCommentsDefs = new ArrayList();
                        slComments.Add(s6xElem.UniqueAddress, alCommentsDefs);
                    }
                    alCommentsDefs.Add(new object[] { s6xElem.BankNum, s6xElem.AddressInt, sComments, previousBankNum, previousAddress });
                }

                foreach (S6xOtherAddress s6xElem in sadS6x.slOtherAddresses.Values)
                {
                    if (s6xElem.Skip) continue;
                    if (s6xElem.OutputComments == false) continue;

                    string sComments = s6xElem.Comments;
                    if (s6xElem.Comments == null || s6xElem.Comments == string.Empty) continue;

                    int previousBankNum = -1;
                    int previousAddress = -1;
                    string previousUniqueAddress = string.Empty;
                    if (!s6xElem.InlineComments)
                    {
                        previousUniqueAddress = getPrevElemAddress(s6xElem.UniqueAddress, ref slAllElements, ref sadBin, ref sadS6x);
                        if (previousUniqueAddress == null || previousUniqueAddress == string.Empty) continue;
                        previousBankNum = Convert.ToInt32(previousUniqueAddress.Substring(0, 1));
                        previousAddress = Convert.ToInt32(previousUniqueAddress.Substring(1).Trim());
                    }

                    ArrayList alCommentsDefs = (ArrayList)slComments[s6xElem.UniqueAddress];
                    if (alCommentsDefs == null)
                    {
                        alCommentsDefs = new ArrayList();
                        slComments.Add(s6xElem.UniqueAddress, alCommentsDefs);
                    }
                    alCommentsDefs.Add(new object[] { s6xElem.BankNum, s6xElem.AddressInt, sComments, previousBankNum, previousAddress });
                }

                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Export initialized.\r\nAddresses initialized.\r\nComments selected.\r\nWriting comments.";
                    sadProcessManager.ProcessProgressStatus = 90;
                }

                sWri = new StreamWriter(cmtFilePath, false, new UTF8Encoding(false));

                singleBank = (sadBin.Calibration.Info.slBanksInfos.Count == 1);

                string prevUniqueAddress = string.Empty;
                foreach (string uniqueAddress in slComments.Keys)
                {
                    ArrayList alCommentsDef = (ArrayList)slComments[uniqueAddress];
                    if (alCommentsDef == null) continue;

                    SortedList slCommentsODef = new SortedList();
                    for (int iDef = 0; iDef < alCommentsDef.Count; iDef++)
                    {
                        object[] oCommentsDef = (object[])alCommentsDef[iDef];
                        slCommentsODef.Add(string.Format("{0} {1:4}", ((int)oCommentsDef[3] == -1 ? "0" : "1"), iDef), oCommentsDef);
                    }

                    foreach (object[] oCommentsDef in slCommentsODef.Values)
                    {
                        string commentsLines = (string)oCommentsDef[2];
                        string headerPreviousUniqueAddress = string.Empty;
                        if ((int)oCommentsDef[3] != -1 && (int)oCommentsDef[4] != -1)
                        {
                            if (singleBank) headerPreviousUniqueAddress = string.Format("{1:x4}", (int)oCommentsDef[3], (int)oCommentsDef[4] + SADDef.EecBankStartAddress);
                            else if (tfSADVersion == TFST_SAD_VER_4) headerPreviousUniqueAddress = string.Format("{0}{1:x4}", (int)oCommentsDef[3], (int)oCommentsDef[4] + SADDef.EecBankStartAddress);
                            else headerPreviousUniqueAddress = string.Format("{0} {1:x4}", (int)oCommentsDef[3], (int)oCommentsDef[4] + SADDef.EecBankStartAddress);
                        }
                        if (commentsLines == null) continue;

                        commentsLines = commentsLines.Replace('\r', '\n');
                        while (commentsLines.Contains("\n\n")) commentsLines = commentsLines.Replace("\n\n", "\n");
                        string[] linesArray = commentsLines.Split('\n');

                        string outAddress = string.Empty;
                        if (singleBank) outAddress = string.Format("{1:x4}", (int)oCommentsDef[0], (int)oCommentsDef[1] + SADDef.EecBankStartAddress);
                        else if (tfSADVersion == TFST_SAD_VER_4) outAddress = string.Format("{0}{1:x4}", (int)oCommentsDef[0], (int)oCommentsDef[1] + SADDef.EecBankStartAddress);
                        else outAddress = string.Format("{0} {1:x4}", (int)oCommentsDef[0], (int)oCommentsDef[1] + SADDef.EecBankStartAddress);
                        if (headerPreviousUniqueAddress != string.Empty) outAddress = headerPreviousUniqueAddress;

                        //Global Header
                        if (uniqueAddress == "0") outAddress = string.Empty;
                        
                        int iLine = 0;
                        foreach (string sLine in linesArray)
                        {
                            iLine++;
                            string addressPart = outAddress;
                            if (prevUniqueAddress == outAddress) addressPart = setAddressShortcut;
                            else prevUniqueAddress = outAddress;

                            string outLine = sLine;

                            string instructionPart = string.Empty;
                            instructionPart = setStartString;
                            if (((int)oCommentsDef[3] != -1 && (int)oCommentsDef[4] != -1) || addressPart == setAddressShortcut || iLine > 1)
                            {
                                instructionPart = setNewLineChar + instructionPart;
                                // Header Mode
                                if (outLine.Length > 0 && outLine.Trim().Length == 0)
                                {
                                    instructionPart = setNewLineChar;
                                    if (setStartString != string.Empty) instructionPart += new string(' ', setStartString.Length).Replace(" ", setExportHeaderRepChar);
                                    outLine = outLine.Replace(setExportHeaderToRepChar, setExportHeaderRepChar);
                                }
                            }

                            sWri.WriteLine(string.Format("{0} {1}{2}", addressPart, instructionPart, outLine));
                        }
                    }
                }

                if (sadProcessManager != null) sadProcessManager.SetProcessFinished("Export is done.");
            }
            catch (Exception ex)
            {
                throw new Exception("Comments file export has failed.\r\n" + ex.Message);
            }
            finally
            {
                slComments = null;
                slAllElements = null;

                try { sWri.Close(); }
                catch { }
                try { sWri.Dispose(); }
                catch { }
                sWri = null;

                GC.Collect();
            }
        }
        
        public static void ImportCmtFile(string cFilePath, string tfSADVersion, ref ArrayList alNewTreeNodesInfos, ref SADBin sadBin, ref SADS6x sadS6x, ref SADProcessManager sadProcessManager)
        {
            if (cFilePath == string.Empty) return;
            if (!File.Exists(cFilePath)) return;
            if (alNewTreeNodesInfos == null) return;
            if (sadBin == null) return;
            if (sadS6x == null) return;

            // alNewTreeNodesInfos - ArrayList containing N string[] arrays
            // string[] array definition
            //  0 : Node Categ Name
            //  1 : Node Name (UniqueAddress)
            //  2 : Node ToolTipText
            //  3 : Node Text (OTHER categ only)

            StreamReader sReader = null;
            bool bankModeSet = false;
            bool bankMode = false;

            bool bMainHeaderMode = true;

            SortedList slComments = new SortedList();
            SortedList slHeaderComments = new SortedList();
            SortedList slPossibleNextHeaders = new SortedList();

            if (sadProcessManager != null) sadProcessManager.ProcessProgressLabel = "Import initialization.";
            
            SettingsLst sadSettings = (SettingsLst)ToolsXml.DeserializeFile(Application.StartupPath + "\\" + SADDef.settingsSADImpExpFileName, typeof(SettingsLst));
            if (sadSettings == null) sadSettings = new SettingsLst();
            ToolsSettings.Update(ref sadSettings, "SADIMPEXP");

            string setAddressShortcut = "1";
            string setNewLineChar = "|";
            string setNewLineInLineChar = "^";
            string setStartString = "#";
            string setStartStringAlt = "//";
            string setImportHeaderToRepChar = "#";
            string setImportHeaderRepChar = " ";
            string setExportHeaderToRepChar = " ";
            string setExportHeaderRepChar = "#";

            setAddressShortcut = sadSettings.Get<string>("COMMENTS_ADDRESS_SHORTCUT", setAddressShortcut);
            setNewLineChar = sadSettings.Get<string>("COMMENTS_NEW_LINE_CHAR", setNewLineChar);
            setNewLineInLineChar = sadSettings.Get<string>("COMMENTS_INLINE_NEW_LINE_CHAR", setNewLineInLineChar);
            setStartString = sadSettings.Get<string>("COMMENTS_START_STRING", setStartString);
            setStartStringAlt = sadSettings.Get<string>("COMMENTS_START_STRING_ALT", setStartStringAlt);
            setImportHeaderToRepChar = sadSettings.Get<string>("COMMENTS_IMPORT_HEADER_CHAR_TO_REPLACE", setImportHeaderToRepChar);
            setImportHeaderRepChar = sadSettings.Get<string>("COMMENTS_IMPORT_HEADER_CHAR_REPLACEMENT", setImportHeaderRepChar);
            setExportHeaderToRepChar = sadSettings.Get<string>("COMMENTS_IMPORT_HEADER_CHAR_TO_REPLACE", setExportHeaderToRepChar);
            setExportHeaderRepChar = sadSettings.Get<string>("COMMENTS_IMPORT_HEADER_CHAR_REPLACEMENT", setExportHeaderRepChar);

            if (setNewLineChar.Length != 1) setNewLineChar = "|";
            if (setNewLineInLineChar.Length != 1) setNewLineInLineChar = "^";

            // Outside loop for Shortcut mode "1 ..."
            int bankNum = -1;
            int address = -1;
            string uniqueAddress = string.Empty;

            try
            {
                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nReading comments.";
                    sadProcessManager.ProcessProgressStatus = 10;
                }

                sReader = File.OpenText(cFilePath);

                while (!sReader.EndOfStream)
                {
                    string sLine = sReader.ReadLine();

                    if (sLine == null) continue;
                    if (sLine == string.Empty) continue;
                    sLine = sLine.Trim();
                    if (sLine == string.Empty) continue;

                    // Define Bank Mode or Not and BankModeSet for global header end
                    if (!bankModeSet && sLine.Length > 4)
                    {
                        if (!sLine.StartsWith(setAddressShortcut + " ")) //Shortcut Mode
                        {
                            bankNum = -1;
                            address = -1;
                            try
                            {
                                switch (tfSADVersion)
                                {
                                    case TFST_SAD_VER_0:
                                    case TFST_SAD_VER_3:
                                        if (sLine.Substring(4, 1) == " ")
                                        {
                                            bankNum = -1;
                                            address = Convert.ToInt32(sLine.Substring(0, 4), 16);
                                        }
                                        else if (sLine.Length > 6)
                                        {
                                            if (sLine.Substring(1, 1) == " " && sLine.Substring(6, 1) == " ")
                                            {
                                                bankNum = Convert.ToInt32(sLine.Substring(0, 1));
                                                address = Convert.ToInt32(sLine.Substring(2, 4), 16);
                                            }
                                        }
                                        break;
                                    case TFST_SAD_VER_4:
                                        if (sLine.Substring(4, 1) == " ")
                                        {
                                            bankNum = -1;
                                            address = Convert.ToInt32(sLine.Substring(0, 4), 16);
                                        }
                                        else if (sLine.Length > 5)
                                        {
                                            if (sLine.Substring(5, 1) == " ")
                                            {
                                                bankNum = Convert.ToInt32(sLine.Substring(0, 1));
                                                address = Convert.ToInt32(sLine.Substring(1, 4), 16);
                                            }
                                        }
                                        break;
                                }
                            }
                            catch
                            {
                                bankNum = int.MinValue;
                            }

                            switch (bankNum)
                            {
                                case 8:
                                case 1:
                                case 9:
                                case 0:
                                    bankMode = true;
                                    break;
                            }

                            bankModeSet = address != -1 && (bankNum == -1 || bankMode);
                        }
                    }

                    if (sLine.Length == 0) continue;

                    if (sLine.StartsWith(setAddressShortcut + " "))
                    {
                        //Shortcut Mode, Bank, Address and UniqueAddress will be reused
                        sLine = sLine.Substring(setAddressShortcut.Length + 1).Trim();
                    }
                    else if (bankModeSet)
                    {
                        try
                        {
                            // Works for all SAD versions
                            bankNum = 8;
                            if (bankMode)
                            {
                                bankNum = Convert.ToInt32(sLine.Substring(0, 1));
                                sLine = sLine.Substring(1).Trim();
                            }
                            address = Convert.ToInt32(sLine.Substring(0, 4), 16) - SADDef.EecBankStartAddress;
                            sLine = sLine.Substring(4).Trim();
                            uniqueAddress = Tools.UniqueAddress(bankNum, address);
                        }
                        catch
                        {
                            bankNum = -1;
                            address = -1;
                            uniqueAddress = string.Empty;
                        }
                    }

                    if (sLine.Length == 0) continue;
                    if (uniqueAddress == string.Empty && !bMainHeaderMode) continue;

                    // Storing first char
                    string specialLineStart = sLine.Substring(0, 1);
                    if (specialLineStart != setNewLineChar && specialLineStart != setNewLineInLineChar) specialLineStart = string.Empty;
                    if (specialLineStart != string.Empty) sLine = sLine.Substring(specialLineStart.Length).Trim();
                    
                    // Managing "^# " sequence at start
                    // Inline Carrier Return
                    /*
                    244C # Flags (when set)^# B0 (set)   but not used anywhere ?
                    244c ^# B2 (clear) Force bank 2 = Bank 1 calculated injection time
                    244a ^# B3 (Set)   Average the cyl mass calcs unless cranking ^#(otherwise done separately per bank - for filtering ?)
                    244a ^# B4 (Clear) Allow Tmr9 flag [from NDS, affects idle/ISC]^#    (3 NDS flags total)
                    */
                    if (specialLineStart == setNewLineInLineChar)
                    {
                        if (setStartString != string.Empty && sLine.StartsWith(setStartString)) sLine = sLine.Substring(setStartString.Length);
                        else if (setStartStringAlt != string.Empty && sLine.StartsWith(setStartStringAlt)) sLine = sLine.Substring(setStartStringAlt.Length);
                        
                        sLine = "\r\n" + sLine;
                        specialLineStart = string.Empty;   // Standard Start
                    }
                    // Other occurences
                    if (sLine.Contains(setNewLineInLineChar))
                    {
                        if (setStartString != string.Empty && sLine.Contains(setNewLineInLineChar + setStartString)) sLine = sLine.Replace(setNewLineInLineChar + setStartString, "\r\n");
                        else if (setStartStringAlt != string.Empty && sLine.Contains(setNewLineInLineChar + setStartStringAlt)) sLine = sLine.Replace(setNewLineInLineChar + setStartStringAlt, "\r\n");
                        else sLine = sLine.Replace(setNewLineInLineChar, "\r\n");
                    }

                    // Carrier Returns counting
                    int iLineStartAddCR = 0;
                    while (sLine.StartsWith(setNewLineChar))
                    {
                        sLine = sLine.Substring(setNewLineChar.Length);
                        iLineStartAddCR++;
                    }
                    if (iLineStartAddCR > 0) sLine = sLine.Trim();

                    // Other occurences
                    if (sLine.Contains(setNewLineChar))
                    {
                        if (setStartString != string.Empty && sLine.Contains(setNewLineChar + setStartString)) sLine = sLine.Replace(setNewLineChar + setStartString, "\r\n");
                        else if (setStartStringAlt != string.Empty && sLine.Contains(setNewLineChar + setStartStringAlt)) sLine = sLine.Replace(setNewLineChar + setStartStringAlt, "\r\n");
                        else sLine = sLine.Replace(setNewLineChar, "\r\n");
                    }

                    // Header Footer Mngt
                    if (setImportHeaderToRepChar != string.Empty)
                    {
                        if (sLine.StartsWith(setImportHeaderToRepChar + setImportHeaderToRepChar)) sLine = sLine.Replace(setImportHeaderToRepChar, setImportHeaderRepChar);
                        if (sLine == setImportHeaderToRepChar) sLine = string.Empty;
                    }

                    // Ignoring Start String sequence
                    if (setStartString != string.Empty && sLine.StartsWith(setStartString)) sLine = sLine.Substring(setStartString.Length);
                    if (setStartStringAlt != string.Empty && sLine.StartsWith(setStartStringAlt)) sLine = sLine.Substring(setStartStringAlt.Length);

                    if (uniqueAddress != string.Empty) bMainHeaderMode = false;
                    if (bMainHeaderMode) uniqueAddress = "0";

                    ArrayList alCommentsDefs = (ArrayList)slComments[uniqueAddress];
                    if (alCommentsDefs == null)
                    {
                        alCommentsDefs = new ArrayList();
                        slComments.Add(uniqueAddress, alCommentsDefs);
                    }

                    if (!bMainHeaderMode)
                    {
                        // Next Element Header
                        if (specialLineStart == setNewLineChar)
                        {
                            // 0 for first item of comments defs when comments defs count is 0
                            if (!slPossibleNextHeaders.ContainsKey(uniqueAddress)) slPossibleNextHeaders.Add(uniqueAddress, alCommentsDefs.Count);
                        }
                    }
                    
                    alCommentsDefs.Add(sLine);
                }

                try { sReader.Close(); }
                catch { }
                try { sReader.Dispose(); }
                catch { }
                sReader = null;

                // Main Header Mode - slComments Key is "0"
                ArrayList alCommentsMHDefs = (ArrayList)slComments["0"];
                if (alCommentsMHDefs != null)
                {
                    if (alCommentsMHDefs.Count > 0)
                    {
                        sadS6x.Properties.Header = string.Join("\r\n", (string[])alCommentsMHDefs.ToArray(typeof(string)));
                        sadS6x.Properties.OutputHeader = true;
                    }
                    alCommentsMHDefs = null;
                    slComments.Remove("0");
                }

                // Possible Next Header mngt with Comments cleanup
                SortedList slAllElements = null;
                // Time consuming process
                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nComments loaded.\r\nAddresses initialization.";
                    sadProcessManager.ProcessProgressStatus = 20;
                }
                initAllElementsArr(ref slAllElements, ref sadBin, ref sadS6x);
                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nComments loaded.\r\nAddresses initialized.\r\nGenerating updates.";
                    sadProcessManager.ProcessProgressStatus = 80;
                }
                
                foreach (string uAddr in slPossibleNextHeaders.Keys)
                {
                    ArrayList alAddrCommentsDefs = (ArrayList)slComments[uAddr];
                    if (alAddrCommentsDefs == null) continue;
                    if (alAddrCommentsDefs.Count == 0) continue;
                    
                    int iNextAddrCommentsDefStartIndex = (int)slPossibleNextHeaders[uAddr];
                    if (iNextAddrCommentsDefStartIndex < 0 || iNextAddrCommentsDefStartIndex >= alAddrCommentsDefs.Count) continue;

                    ArrayList alNextAddrCommentsDefs = new ArrayList();
                    for (int iDef = iNextAddrCommentsDefStartIndex; iDef < alAddrCommentsDefs.Count; iDef++) alNextAddrCommentsDefs.Add(alAddrCommentsDefs[iDef]);
                    
                    alAddrCommentsDefs.RemoveRange(iNextAddrCommentsDefStartIndex, alNextAddrCommentsDefs.Count);
                    alAddrCommentsDefs = null;

                    if (alNextAddrCommentsDefs.Count == 0) continue;

                    string nextElemUniqueAddress = getNextElemAddress(uAddr, ref slAllElements, ref sadBin, ref sadS6x);
                    if (nextElemUniqueAddress == null || nextElemUniqueAddress == string.Empty) continue;

                    ArrayList alCommentsDefs = (ArrayList)slHeaderComments[nextElemUniqueAddress];
                    if (alCommentsDefs == null) slHeaderComments.Add(nextElemUniqueAddress, alNextAddrCommentsDefs);
                    else alCommentsDefs.AddRange(alNextAddrCommentsDefs);
                }

                // Header Comments
                //      All Elements can be concerned
                foreach (string uAddr in slHeaderComments.Keys)
                {
                    ArrayList alCommentsDefs = (ArrayList)slHeaderComments[uAddr];
                    if (alCommentsDefs == null) continue;
                    if (alCommentsDefs.Count == 0) continue;

                    string sComments = string.Join("\r\n", (string[])alCommentsDefs.ToArray(typeof(string)));
                    alCommentsDefs = null;

                    if (sComments == string.Empty) continue;

                    if (sadS6x.slFunctions.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xFunction)sadS6x.slFunctions[uAddr]).Comments = sComments;
                        ((S6xFunction)sadS6x.slFunctions[uAddr]).OutputComments = true;
                        ((S6xFunction)sadS6x.slFunctions[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.FUNCTIONS), uAddr, string.Empty, sComments });
                    }
                    else if (sadS6x.slOperations.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xOperation)sadS6x.slOperations[uAddr]).Comments = sComments;
                        ((S6xOperation)sadS6x.slOperations[uAddr]).OutputComments = false;
                        ((S6xOperation)sadS6x.slOperations[uAddr]).InlineComments = false;
                        alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OPERATIONS), uAddr, string.Empty, sComments });
                    }
                    else if (sadS6x.slRoutines.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xRoutine)sadS6x.slRoutines[uAddr]).Comments = sComments;
                        ((S6xRoutine)sadS6x.slRoutines[uAddr]).OutputComments = true;
                        ((S6xRoutine)sadS6x.slRoutines[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.ROUTINES), uAddr, string.Empty, sComments });
                    }
                    else if (sadS6x.slScalars.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xScalar)sadS6x.slScalars[uAddr]).Comments = sComments;
                        ((S6xScalar)sadS6x.slScalars[uAddr]).OutputComments = false;
                        ((S6xScalar)sadS6x.slScalars[uAddr]).InlineComments = false;
                        ((S6xScalar)sadS6x.slScalars[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), uAddr, string.Empty, sComments });
                    }
                    else if (sadS6x.slStructures.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xStructure)sadS6x.slStructures[uAddr]).Comments = sComments;
                        ((S6xStructure)sadS6x.slStructures[uAddr]).OutputComments = true;
                        ((S6xStructure)sadS6x.slStructures[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.STRUCTURES), uAddr, string.Empty, sComments });
                    }
                    else if (sadS6x.slTables.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xTable)sadS6x.slTables[uAddr]).Comments = sComments;
                        ((S6xTable)sadS6x.slTables[uAddr]).OutputComments = true;
                        ((S6xTable)sadS6x.slTables[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.TABLES), uAddr, string.Empty, sComments });
                    }
                    else
                    {
                        int oaBankNum = -1;
                        int oaAddress = -1;
                        try
                        {
                            oaBankNum = Convert.ToInt32(uAddr.Substring(0, 1));
                            oaAddress = Convert.ToInt32(uAddr.Substring(2).Replace(" ", ""));
                        }
                        catch
                        {
                            oaBankNum = -1;
                            oaAddress = -1;
                        }
                        if (oaBankNum >= 0 && oaAddress >= 0)
                        {
                            S6xOtherAddress other = new S6xOtherAddress();
                            other.BankNum = oaBankNum;
                            other.AddressInt = oaAddress;
                            other.Label = other.DefaultLabel;
                            other.OutputLabel = false;
                            other.Comments = sComments;
                            other.InlineComments = false;
                            other.OutputComments = true;

                            if (sadS6x.slOtherAddresses.ContainsKey(other.UniqueAddress))
                            {
                                ((S6xOtherAddress)sadS6x.slOtherAddresses[other.UniqueAddress]).Comments = other.Comments;
                                ((S6xOtherAddress)sadS6x.slOtherAddresses[other.UniqueAddress]).InlineComments = other.InlineComments;
                                ((S6xOtherAddress)sadS6x.slOtherAddresses[other.UniqueAddress]).OutputComments = other.OutputComments;
                            }
                            else
                            {
                                sadS6x.slOtherAddresses.Add(other.UniqueAddress, other);
                            }
                            alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OTHER), other.UniqueAddress, other.UniqueAddressHex, other.Comments });

                            other = null;
                            sadS6x.isSaved = false;
                        }
                    }
                }
                
                // Inline Comments
                //      Only specific elements
                foreach (string uAddr in slComments.Keys)
                {
                    ArrayList alCommentsDefs = (ArrayList)slComments[uAddr];
                    if (alCommentsDefs == null) continue;
                    if (alCommentsDefs.Count == 0) continue;

                    string sComments = string.Join("\r\n", (string[])alCommentsDefs.ToArray(typeof(string)));
                    alCommentsDefs = null;

                    if (sComments == string.Empty) continue;
                    
                    if (sadS6x.slOperations.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xOperation)sadS6x.slOperations[uAddr]).Comments = sComments;
                        ((S6xOperation)sadS6x.slOperations[uAddr]).InlineComments = true;
                        ((S6xOperation)sadS6x.slOperations[uAddr]).OutputComments = true;
                        alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OPERATIONS), uAddr, string.Empty, sComments });
                    }
                    if (sadS6x.slScalars.ContainsKey(uAddr))
                    {
                        sadS6x.isSaved = false;
                        ((S6xScalar)sadS6x.slScalars[uAddr]).Comments = sComments;
                        ((S6xScalar)sadS6x.slScalars[uAddr]).InlineComments = true;
                        ((S6xScalar)sadS6x.slScalars[uAddr]).OutputComments = true;
                        ((S6xScalar)sadS6x.slScalars[uAddr]).Store = true;
                        alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.SCALARS), uAddr, string.Empty, sComments });
                    }
                    else
                    {
                        int oaBankNum = -1;
                        int oaAddress = -1;
                        try
                        {
                            oaBankNum = Convert.ToInt32(uAddr.Substring(0, 1));
                            oaAddress = Convert.ToInt32(uAddr.Substring(2).Replace(" ", ""));
                        }
                        catch
                        {
                            oaBankNum = -1;
                            oaAddress = -1;
                        }
                        if (oaBankNum >= 0 && oaAddress >= 0)
                        {
                            S6xOtherAddress other = new S6xOtherAddress();
                            other.BankNum = oaBankNum;
                            other.AddressInt = oaAddress;
                            other.Label = other.DefaultLabel;
                            other.OutputLabel = false;
                            other.Comments = sComments;
                            other.InlineComments = true;
                            other.OutputComments = true;

                            if (sadS6x.slOtherAddresses.ContainsKey(other.UniqueAddress))
                            {
                                ((S6xOtherAddress)sadS6x.slOtherAddresses[other.UniqueAddress]).Comments = other.Comments;
                                ((S6xOtherAddress)sadS6x.slOtherAddresses[other.UniqueAddress]).InlineComments = other.InlineComments;
                                ((S6xOtherAddress)sadS6x.slOtherAddresses[other.UniqueAddress]).OutputComments = other.OutputComments;
                            }
                            else
                            {
                                sadS6x.slOtherAddresses.Add(other.UniqueAddress, other);
                            }
                            alNewTreeNodesInfos.Add(new string[] { S6xNav.getHeaderCategName(S6xNavHeaderCategory.OTHER), other.UniqueAddress, other.UniqueAddressHex, other.Comments });

                            other = null;
                            sadS6x.isSaved = false;
                        }
                    }
                }

                if (sadProcessManager != null)
                {
                    sadProcessManager.ProcessProgressLabel = "Import initialized.\r\nComments loaded.\r\nAddresses initialized.\r\nApplying updates.";
                    sadProcessManager.ProcessProgressStatus = 90;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Comments file import has failed.\r\n" + ex.Message);
            }
            finally
            {
                try { sReader.Close(); }
                catch { }
                try { sReader.Dispose(); }
                catch { }
                sReader = null;

                slComments = null;
                slHeaderComments = null;
                slPossibleNextHeaders = null;

                GC.Collect();
            }
        }

    }
}
