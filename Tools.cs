using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using NCalc;

namespace SAD806x
{
    public static class Tools
    {
        public static bool checkBit(int bitPos, bool bSet, byte iByte)
        {
            if (bitPos < 0 || bitPos > 7) return false;
            
            byte[] arBits = byteToBits(iByte);
            return ((arBits[bitPos] == 1) == bSet);
        }

        public static byte byteToBitOpposite(byte iByte)
        {
            BitArray baBits = new BitArray(new byte[] { iByte });
            byte[] arBytes = new byte[1];
            baBits.Not().CopyTo(arBytes, 0);
            return arBytes[0];
        }

        public static byte[] byteToBits(byte iByte)
        {
            BitArray baBits = new BitArray(new byte[] { iByte });
            byte[] arBits = new byte[8];
            for (int iBit = 0; iBit < arBits.Length; iBit++) arBits[iBit] = 0;
            for (int iBit = 0; iBit < arBits.Length; iBit++) arBits[iBit] = Convert.ToByte(baBits.Get(iBit));
            return arBits;
        }
        
        public static string UniqueAddress(int bankNum, int address)
        {
            return string.Format("{0,1} {1,5}", bankNum, address);
        }

        public static string UniqueAddress(int bankNum, int binAddress, int bankBinAddress)
        {
            return UniqueAddress(bankNum, binAddress - bankBinAddress);
        }

        public static string UniqueAddressHex(int bankNum, int address)
        {
            return string.Format("{0,1} {1:x4}", bankNum, SADDef.EecBankStartAddress + address);
        }

        public static string UniqueAddressHex(string uniqueAddress)
        {
            try
            {
                return string.Format("{0,1} {1:x4}", uniqueAddress.Substring(0, 1), SADDef.EecBankStartAddress + Convert.ToInt32(uniqueAddress.Substring(1).Trim()));
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string RegisterUniqueAddress(int address)
        {
            return string.Format("R {0,4}", address);
        }

        public static string RegisterUniqueAddress(string address)
        {
            try
            {
                if (address.Contains(SADDef.AdditionSeparator))
                {
                    string regPart1 = address.Substring(0, address.IndexOf(SADDef.AdditionSeparator));
                    string regPart2 = address.Replace(regPart1 + SADDef.AdditionSeparator, string.Empty);
                    if (regPart1.Length > 2 || regPart2.Length > 2) return "R XXXX";

                    int iRegPart1 = Convert.ToInt32(regPart1, 16);
                    int iRegPart2 = Convert.ToInt32(regPart2, 16);

                    return Tools.RegisterUniqueAddress(iRegPart1) + SADDef.AdditionSeparator + Convert.ToString(iRegPart2);
                }
                else
                {
                    return RegisterUniqueAddress(Convert.ToInt32(address, 16));
                }
            }
            catch
            {
                return "R XXXX";
            }
        }

        public static int RegisterUniqueAddressInt(string address)
        {
            try
            {
                if (address.Contains(SADDef.AdditionSeparator))
                {
                    string regPart1 = address.Substring(0, address.IndexOf(SADDef.AdditionSeparator));
                    string regPart2 = address.Replace(regPart1 + SADDef.AdditionSeparator, string.Empty);
                    if (regPart1.Length > 2 || regPart2.Length > 2) return 0;

                    int iRegPart1 = Convert.ToInt32(regPart1, 16);
                    int iRegPart2 = Convert.ToInt32(regPart2, 16);

                    return Convert.ToInt32(regPart1, 16) + Convert.ToInt32(regPart2, 16);
                }
                else
                {
                    return Convert.ToInt32(address, 16);
                }
            }
            catch
            {
                return 0;
            }
        }

        public static int binAddressCorrected(int bankNum, int address, ref SADBin sadBin, int addressBin)
        {
            if (sadBin == null) return addressBin;
            if (!sadBin.isLoaded || !sadBin.isValid) return addressBin;

            SADBank sadBank = null;
            int newAddressBin = 0;

            switch (bankNum)
            {
                case 8:
                    sadBank = sadBin.Bank8;
                    break;
                case 1:
                    sadBank = sadBin.Bank1;
                    break;
                case 9:
                    sadBank = sadBin.Bank9;
                    break;
                case 0:
                    sadBank = sadBin.Bank0;
                    break;
            }

            if (sadBank == null) return newAddressBin;
            if (address < sadBank.Size) newAddressBin = sadBank.AddressBinInt + address;
            sadBank = null;
            return newAddressBin;
        }

        public static int binAddressFromXdfAddress(string xdfAddress, string s6xOffset, bool s6xOffsetSubtract)
        {
            int xdfOffset = 0;

            try
            {
                xdfOffset = Convert.ToInt32(s6xOffset, 16);
                if (s6xOffsetSubtract) xdfOffset *= -1;
            }
            catch { }

            return binAddressFromXdfAddress(xdfAddress, xdfOffset);
        }

        public static int binAddressFromXdfAddress(string xdfAddress, string xdfOffset, string xdfOffsetSubtract)
        {
            int iXdfOffset = 0;

            try
            {
                iXdfOffset = Convert.ToInt32(xdfOffset);
                if (xdfOffsetSubtract == "1") iXdfOffset *= -1;
            }
            catch { }

            return binAddressFromXdfAddress(xdfAddress, iXdfOffset);
        }

        public static int binAddressFromXdfAddress(string xdfAddress, int xdfOffset)
        {
            if (xdfAddress == null) xdfAddress = "0";
            return Convert.ToInt32(xdfAddress.ToLower().Replace("0x", ""), 16) + xdfOffset;
        }

        public static string xdfAddressFromBinAddress(int binAddress, int xdfOffset)
        {
            return "0x" + string.Format("{0:x4}", binAddress - xdfOffset);
        }

        public static string PointerTranslation(string sPointer)
        {
            if (sPointer.Length == 1) sPointer = "0" + sPointer;
            switch (sPointer.Length)
            {
                case 2:
                    return SADDef.ShortPointerPrefix + sPointer;
                default:
                    return SADDef.LongPointerTemplate.Replace("%POINTER%", sPointer);
            }
        }

        public static string RegisterInstruction(string sRegister)
        {
            if (sRegister == string.Empty) return "?";
            if (sRegister.Contains("?")) return sRegister;

            if (sRegister.Contains(SADDef.AdditionSeparator))
            {
                sRegister = SADDef.ShortRegisterPrefix + sRegister;
            }
            else
            {
                sRegister = Convert.ToString(Convert.ToInt32(sRegister, 16), 16).ToLower();
                if (sRegister.Length == 1) sRegister = "0" + sRegister;
            }
            switch (sRegister.Length)
            {
                case 2:
                    return SADDef.ShortRegisterPrefix + sRegister;
                default:
                    return SADDef.LongRegisterTemplate.Replace("%LREG%", sRegister);
            }
        }

        public static int InstructionRegisterAddress(string sInstruction)
        {
            if (sInstruction == string.Empty) return -1;
            if (sInstruction.Contains("?")) return -1;

            sInstruction = sInstruction.Replace(SADDef.ShortRegisterPrefix, "").Replace(SADDef.LongRegisterPointerPrefix, "").Replace(SADDef.LongRegisterPointerSuffix, "");

            try { return Convert.ToInt32(sInstruction, 16); }
            catch { return -1; } 
        }

        public static string VariableRegisterAddressHex(string variableValue)
        {
            int regRes = VariableRegisterAddress(variableValue);

            if (regRes == -1) return string.Empty;
            else return Convert.ToString(regRes, 16);
        }

        public static int VariableRegisterAddress(string variableValue)
        {
            if (variableValue != null)
            {
                if (variableValue.Contains(SADDef.VariableValuesSeparator))
                // Variable Value contains Argument and Register
                {
                    foreach (string sPart in variableValue.Trim().Split(SADDef.VariableValuesSeparator.ToCharArray()))
                    {
                        if (!sPart.Trim().ToLower().StartsWith("ar")) return Tools.InstructionRegisterAddress(sPart.Trim().ToUpper());
                    }
                }
                else if (!variableValue.ToLower().Contains("ar"))
                // Variable Address is Register
                {
                    return Tools.InstructionRegisterAddress(variableValue.Trim().ToUpper());
                }
            }

            return -1;
        }

        public static int VariableRegisterAddressByEquivalent(string eqCode, string variableValue)
        {
            if (variableValue != null)
            {
                if (variableValue.Contains(SADDef.VariableValuesSeparator))
                {
                    if (variableValue.ToLower().Contains(eqCode.ToLower()))
                    {
                        foreach (string sPart in variableValue.Trim().Split(SADDef.VariableValuesSeparator.ToCharArray()))
                        {
                            if (sPart.Trim().ToLower() == eqCode.ToLower()) continue;
                            return Tools.InstructionRegisterAddress(sPart.Trim().ToUpper());
                        }
                    }
                }
            }

            return -1;
        }

        public static string ArgumentCode(string Position)
        {
            try { return ArgumentCode(Convert.ToInt32(Position)); }
            catch { return SADDef.ArgumentCodePrefix; }
        }

        public static string ArgumentCode(int Position)
        {
            if (Position < 0 || Position > 100) return SADDef.ArgumentCodePrefix;

            return string.Format("{0,1}{1:d2}", SADDef.ArgumentCodePrefix, Position);
        }

        // Get Pointers and/or Values from Intruction Parameter
        //  Returns True, False - Pointer, Not Pointer
        //          Pointer / Value 1   - String format
        //          Pointer / Value 1   - Integer format
        //          Pointer / Value 2   - String if exists
        //          Pointer / Value 2   - Integer format if exists
        public static object[] InstructionPointersValues(string instructionParam)
        {
            bool isPointer = false;
            object[] arrRes = null;
            string[] arrParam = null;

            if (instructionParam == string.Empty) return new object[] { false, "00", 0 };

            if (instructionParam.Contains(SADDef.ShortPointerPrefix))
            {
                isPointer = true;
                instructionParam = instructionParam.Replace(SADDef.ShortPointerPrefix, "");
            }
            if (instructionParam.StartsWith(SADDef.LongRegisterPointerPrefix))
            {
                instructionParam = instructionParam.Replace(SADDef.LongRegisterPointerPrefix, "").Replace(SADDef.LongRegisterPointerSuffix, "");
                isPointer = true;
            }
            if (instructionParam.Contains(SADDef.ShortRegisterPrefix))
            {
                instructionParam = instructionParam.Replace(SADDef.ShortRegisterPrefix, "");
                isPointer = true;
            }

            if (instructionParam.Contains(SADDef.IncrementSuffix)) instructionParam = instructionParam.Replace(SADDef.IncrementSuffix, "");
            if (instructionParam.Contains(SADDef.AdditionSeparator)) arrParam = instructionParam.Replace(SADDef.AdditionSeparator, "=").Split('=');
            else arrParam = new string[] { instructionParam };
            
            arrRes = new object[arrParam.Length * 2 + 1];
            arrRes[0] = isPointer;
            for (int iPos = 0; iPos < arrParam.Length; iPos++)
            {
                arrRes[iPos * 2 + 1] = arrParam[iPos];
                arrRes[iPos * 2 + 2] = Convert.ToInt32(arrParam[iPos], 16);
            }
            
            return arrRes;
        }

        public static string LsbFirst(int iWord)
        {
            return LsbFirst(string.Format("{0:x4}", iWord));
        }

        public static string LsbFirst(string sWord)
        {
            switch (sWord.Length)
            {
                case 4:
                    return sWord.Substring(2, 2) + sWord.Substring(0, 2);
                case 3:
                    return sWord.Substring(2, 1) + sWord.Substring(0, 2);
                default:
                    return sWord;
            }
        }

        public static string LsbFirst(string[] arr2Bytes)
        {
            return arr2Bytes[1] + arr2Bytes[0];
        }

        public static string getBytes(int startPos, int len, ref string[] arrBytes)
        {
            string sResult = string.Empty;
            for (int iPos = startPos; iPos < startPos + len && iPos < arrBytes.Length; iPos++) sResult += arrBytes[iPos];
            return sResult;
        }

        public static string[] getBytesArray(int startPos, int len, ref string[] arrBytes)
        {
            string[] arrResult = null;
            int lPos = -1;
            
            // Security
            if (len < 0) return new string[] { };
            if (startPos < 0) return new string[] { };

            arrResult = new string[len];

            for (int iPos = 0; iPos < arrResult.Length && startPos + iPos < arrBytes.Length; iPos++)
            {
                lPos = iPos;
                arrResult[iPos] = arrBytes[iPos + startPos];
            }

            // CleanUp
            for (int iPos = lPos + 1; iPos < arrResult.Length; iPos++) arrResult[iPos] = string.Empty;

            return arrResult;
        }

        public static string getPreparedSignature(string sSignature)
        {
            string preparedSignature = sSignature;

            preparedSignature = preparedSignature.Replace(SADDef.GlobalSeparator, string.Empty).Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
            // * char is for maximum 10 instructions of 5 bytes
            preparedSignature = preparedSignature.Replace("*", ".{0,100}");

            return preparedSignature;
        }

        public static object[] getBytesFromSignature(string sSignature, ref string sBytes)
        {
            Regex regExp = null;
            ArrayList alMatches = null;
            Match reSig = null;
            string preparedSignature = string.Empty;

            preparedSignature = getPreparedSignature(sSignature);

            alMatches = new ArrayList();

            regExp = new Regex(preparedSignature, RegexOptions.IgnoreCase);
            reSig = regExp.Match(sBytes);

            while (reSig.Success)
            {
                string[] arrParams = new string[reSig.Groups.Count - 1];
                for (int iParam = 0; iParam < arrParams.Length; iParam++) arrParams[iParam] = reSig.Groups[iParam + 1].Value;
                alMatches.Add(new object[] { reSig.Index / 2, reSig.Value, arrParams });

                reSig = reSig.NextMatch();
            }
            reSig = null;
            regExp = null;

            return alMatches.ToArray();
        }

        public static string getByte(int startPos, ref string[] arrBytes)
        {
            string sResult = string.Empty;
            sResult = getBytes(startPos, 1, ref arrBytes);
            if (sResult.Length != 2) return "ff";
            return sResult;
        }

        public static string getByte(string startPos, ref string[] arrBytes)
        {
            return getByte(Convert.ToInt32(startPos, 16) - SADDef.EecBankStartAddress, ref arrBytes);
        }

        public static string getWord(int startPos, bool lsbFirst, ref string[] arrBytes)
        {
            string sResult = string.Empty;
            sResult = getBytes(startPos, 2, ref arrBytes);
            if (sResult.Length != 4) return "ffff";
            if (lsbFirst) return LsbFirst(sResult);
            return sResult;
        }

        public static string getWord(string startPos, bool lsbFirst, ref string[] arrBytes)
        {
            return getWord(Convert.ToInt32(startPos, 16) - SADDef.EecBankStartAddress, lsbFirst, ref arrBytes);
        }

        public static int getByteInt(string sByte, bool signed)
        {
            if (signed) return Convert.ToSByte(sByte, 16);
            else return Convert.ToInt16(sByte, 16);
        }
        
        public static int getByteInt(int startPos, bool signed, ref string[] arrBytes)
        {
            return getByteInt(getByte(startPos, ref arrBytes), signed);
        }

        public static int getByteInt(string startPos, bool signed, ref string[] arrBytes)
        {
            return getByteInt(getByte(startPos, ref arrBytes), signed);
        }

        public static int getWordInt(string sWord, bool signed)
        {
            if (signed) return Convert.ToInt16(sWord, 16);
            else return Convert.ToInt32(sWord, 16);
        }

        public static int getWordInt(string[] arr2Bytes, bool signed, bool lsbFirst)
        {
            if (lsbFirst) return getWordInt(LsbFirst(arr2Bytes), signed);
            else return getWordInt(arr2Bytes[0] + arr2Bytes[1], signed);
        }

        public static int getWordInt(int startPos, bool signed, bool lsbFirst, ref string[] arrBytes)
        {
            return getWordInt(getWord(startPos, lsbFirst, ref arrBytes), signed);
        }

        public static int getWordInt(string startPos, bool signed, bool lsbFirst, ref string[] arrBytes)
        {
            return getWordInt(getWord(startPos, lsbFirst, ref arrBytes), signed);
        }

        public static int matchOpsOpCodes(ref Operation[] opsResult, string[] opsCodes)
        {
            Match reSig = null;
            int sigPos = -1;
            string sOpsCodes = string.Empty;
            string sOpsResultCodes = string.Empty;

            sOpsCodes = string.Join("", opsCodes);
            foreach (Operation ope in opsResult)
            {
                if (ope != null) sOpsResultCodes += ope.OriginalOpArr[0];
            }
            reSig = Regex.Match(sOpsResultCodes, sOpsCodes);
            if (reSig.Success) sigPos = reSig.Index;
            reSig = null;

            if (sigPos < 0) return -1;
            else return sigPos / 2;
        }

        public static int matchOpsOriginalOps(ref Operation[] opsResult, string[] originalOps)
        {
            Match reSig = null;
            int sigPos = -1;
            string sOriginalOps = string.Empty;
            string sOpsResultOriginalOps = string.Empty;

            sOriginalOps = string.Join("Z", originalOps);
            foreach (Operation ope in opsResult)
            {
                if (ope != null) sOpsResultOriginalOps += ope.OriginalOp + "Z";
            }
            reSig = Regex.Match(sOpsResultOriginalOps, sOriginalOps);
            if (reSig.Success) sigPos = reSig.Index;
            reSig = null;

            if (sigPos < 0)
            {
                return -1;
            }
            else
            {
                return sOpsResultOriginalOps.Substring(0, sigPos).Split('Z').Length - 1;
            }
        }

        public static object[] splitPartsLines(int startAddress, ref string[] initialValues)
        {
            ArrayList alLines = null;
            object[] arrResult = null;
            bool bDupFlag = false;
            string sDupValue = string.Empty;
            string[] partValues = null;

            int endAddress = -1;
            int iSize = -1;
            int iSizeRest = -1;
            int iLines = -1;
            string[] lineValues = null;

            object[] arrParts = splitParts(ref initialValues);

            alLines = new ArrayList();            
            foreach (object[] arrPart in arrParts)
            {
                bDupFlag = Convert.ToBoolean(arrPart[0]);
                partValues = (string[])arrPart[1];

                if (bDupFlag)
                {
                    sDupValue = partValues[0];
                    endAddress = startAddress + partValues.Length - 1;
                    alLines.Add(new object[] { startAddress, endAddress, bDupFlag, sDupValue });
                    startAddress += partValues.Length;
                }
                else
                {
                    iSizeRest = partValues.Length;
                    iSize = SADDef.UnknownCalibPartsAggregationSize;
                    iLines = (iSizeRest - iSizeRest % iSize) / iSize;
                    if (iSizeRest % iSize > 0) iLines++;
                    for (int iLine = 0; iLine < iLines; iLine++)
                    {
                        if (iSizeRest < iSize) iSize = iSizeRest;
                        endAddress = startAddress + iSize - 1;
                        lineValues = new string[iSize];
                        for (int iPos = 0; iPos < lineValues.Length; iPos++) lineValues[iPos] = partValues[iPos + iLine * SADDef.UnknownCalibPartsAggregationSize];
                        alLines.Add(new object[] { startAddress, endAddress, bDupFlag, lineValues });
                        lineValues = null;
                        startAddress += iSize;
                        iSizeRest -= iSize;
                    }
                }
                partValues = null;
            }
            arrResult = (object[]) alLines.ToArray(typeof(object));
            alLines = null;

            return arrResult;
        }
        
        public static object[] splitParts(ref string[] initialValues)
        {
            ArrayList alDuplicates = null;
            ArrayList alNonDuplicates = null;
            SortedList slSplit = null;
            object[] arrDuplicate = null;
            object[] arrPrevDuplicate = null;
            object[] arrResult = null;
            object[] arrValues = null;
            int startPos = -1;
            int endPos = -1;
            bool dupFlag = false;
            string dupValue = string.Empty;
            int iCount = -1;

            if (initialValues.Length < SADDef.UnknownCalibPartsAggregationSize)
            {
                arrValues = new string[initialValues.Length];
                for (int iPos = 0; iPos < arrValues.Length; iPos++) arrValues[iPos] = initialValues[iPos];
                return new object[] { new object[] { false, arrValues } };
            }

            alDuplicates = new ArrayList();
            arrDuplicate = null;
            for (int iPos = 1; iPos < initialValues.Length; iPos++)
            {
                if (initialValues[iPos] == initialValues[iPos - 1])
                {
                    if (arrDuplicate == null) arrDuplicate = new object[] { iPos - 1, iPos, true, initialValues[iPos] };
                    else arrDuplicate[1] = iPos;
                }
                else
                {
                    if (arrDuplicate != null)
                    {
                        if (Convert.ToInt32(arrDuplicate[1]) - Convert.ToInt32(arrDuplicate[0]) + 1 >= SADDef.UnknownCalibPartsAggregationSize)
                        {
                            alDuplicates.Add(arrDuplicate);
                        }
                        arrDuplicate = null;
                    }
                }
            }
            if (arrDuplicate != null)
            {
                if (Convert.ToInt32(arrDuplicate[1]) - Convert.ToInt32(arrDuplicate[0]) + 1 >= SADDef.UnknownCalibPartsAggregationSize)
                {
                    alDuplicates.Add(arrDuplicate);
                }
                arrDuplicate = null;
            }

            alNonDuplicates = new ArrayList();
            if (alDuplicates.Count == 0)
            {
                alNonDuplicates.Add(new object[] { 0, initialValues.Length - 1, false });
            }
            else
            {
                arrDuplicate = null;
                arrPrevDuplicate = null;
                for (int iDup = 0; iDup < alDuplicates.Count; iDup++)
                {
                    arrDuplicate = (object[])alDuplicates[iDup];
                    if (iDup == 0 && Convert.ToInt32(arrDuplicate[0]) > 0)
                    {
                        alNonDuplicates.Add(new object[] { 0, Convert.ToInt32(arrDuplicate[0]) - 1, false });
                    }
                    else if (arrPrevDuplicate != null)
                    {
                        if (Convert.ToInt32(arrPrevDuplicate[1]) + 1 < Convert.ToInt32(arrDuplicate[0]))
                        {
                            alNonDuplicates.Add(new object[] { Convert.ToInt32(arrPrevDuplicate[1]) + 1, Convert.ToInt32(arrDuplicate[0]) - 1, false });
                        }
                    }
                    arrPrevDuplicate = arrDuplicate;
                }
                arrPrevDuplicate = null;
                if (Convert.ToInt32(arrDuplicate[1]) < initialValues.Length - 1)
                {
                    alNonDuplicates.Add(new object[] { Convert.ToInt32(arrDuplicate[1]) + 1, initialValues.Length - 1, false });
                }
                arrDuplicate = null;
            }

            slSplit = new SortedList();
            foreach (object[] oDup in alDuplicates) slSplit.Add(Convert.ToInt32(oDup[0]), oDup);
            foreach (object[] oNDup in alNonDuplicates) slSplit.Add(Convert.ToInt32(oNDup[0]), oNDup);
            alDuplicates = null;
            alNonDuplicates = null;

            arrResult = new object[slSplit.Count];
            iCount = -1;
            foreach (object[] oSplit in slSplit.Values)
            {
                iCount++;
                startPos = Convert.ToInt32(oSplit[0]);
                endPos = Convert.ToInt32(oSplit[1]);
                dupFlag = Convert.ToBoolean(oSplit[2]);
                if (dupFlag) dupValue = oSplit[3].ToString();

                arrValues = new string[endPos - startPos + 1];

                if (dupFlag)
                {
                    for (int iPos = 0; iPos < arrValues.Length; iPos++) arrValues[iPos] = dupValue;
                }
                else
                {
                    for (int iPos = 0; iPos < arrValues.Length; iPos++) arrValues[iPos] = initialValues[startPos + iPos];
                }

                arrResult[iCount] = new object[] { dupFlag, arrValues };
                arrValues = null;
            }
            slSplit = null;

            return arrResult;
        }

        public static double ScaleValue(int iValue, string scaleExpression, bool throwException)
        {
            double result = 0;
            
            if (scaleExpression == string.Empty) return (double)iValue;

            Expression exp = new Expression(scaleExpression.ToLower().Replace("x", iValue.ToString()));
            try { result = Convert.ToDouble(exp.Evaluate()); }
            catch (Exception ex)
            {
                if (throwException) throw ex;
                else result = (double)iValue;
            }
            exp = null;

            return result;
        }

        public static string ScaleValue(int iValue, string scaleExpression, int scalePrecision, bool throwException)
        {
            string sFormat = "{0:0}";
            bool bInteger = false;

            if (!bInteger) bInteger = scaleExpression == null;
            if (!bInteger) bInteger = scaleExpression == string.Empty || scaleExpression.ToLower().Trim() == "x";
            if (!bInteger) bInteger = scalePrecision < 1 || scalePrecision > 8;

            if (!bInteger) sFormat = "{0:0." + new string('0', scalePrecision) + "}";
            
            return string.Format(sFormat, ScaleValue(iValue, scaleExpression, throwException));
        }

        public static bool ScaleExpressionCheck(string scaleExpression)
        {
            Expression exp = null;

            if (scaleExpression == string.Empty) return true;

            try
            {
                exp = new Expression(scaleExpression.ToLower().Replace("x", "1"));
                exp.Evaluate();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                exp = null;
            }
        }

        public static string CommentsReviewed(string sComments, string sShortLabel, string sLabel, ref List<string> lReplacementBaseStrings, ref SortedList<string, string> slReplacements, bool firstLineShortLabelLabelRemoval, bool doReplacements)
        {
            if (sComments == null) return string.Empty;

            if (firstLineShortLabelLabelRemoval) sComments = CommentsFirstLineShortLabelLabelRemoval(sComments, sShortLabel, sLabel);

            if (doReplacements) sComments = CommentsReplaced(sComments, ref lReplacementBaseStrings, ref slReplacements);

            return sComments;
        }

        public static string CommentsReplaced(string sComments, ref List<string> lReplacementBaseStrings, ref SortedList<string, string> slReplacements)
        {
            if (sComments == null) return string.Empty;
            if (sComments == string.Empty) return string.Empty;
            
            if (lReplacementBaseStrings == null) return sComments;
            if (slReplacements == null) return sComments;

            // Process first occurence of each ReplacementBaseString ($)
            foreach (string rBaseString in lReplacementBaseStrings)
            {
                // Max Length for search $ + 9 : Scalar with BitFlags $X_XXXX:XX
                // Min Length for search $ + 2 : Register $XX
                int searchMaxLength = 9;
                int searchMinLength = 2;   
                int iStartIndex = 0;
                int iIndex = sComments.IndexOf(rBaseString, iStartIndex);
                while (iIndex >= 0)
                {
                    int sLength = sComments.Length - iIndex;
                    if (sLength > rBaseString.Length + searchMaxLength) sLength = rBaseString.Length + searchMaxLength;
                    string rSearch = sComments.Substring(iIndex, sLength);
                    if (rSearch.Length >= rBaseString.Length + searchMinLength)
                    {
                        //rBaseString removal, it is not present in slReplacements keys
                        rSearch = rSearch.Substring(rBaseString.Length);
                        // Min Length for search
                        while (rSearch.Length >= searchMinLength)
                        {
                            if (slReplacements.ContainsKey(rSearch.ToLower()))
                            {
                                sComments = sComments.Replace(rBaseString + rSearch, slReplacements[rSearch.ToLower()]);
                                break;
                            }
                            rSearch = rSearch.Substring(0, rSearch.Length - 1);
                        }
                    }
                    iStartIndex = iIndex + rBaseString.Length + searchMinLength;
                    if (iStartIndex >= sComments.Length) break;
                    iIndex = sComments.IndexOf(rBaseString, iStartIndex);
                }
            }

            return sComments;
        }

        public static string CommentsFirstLineShortLabelLabel(string sComments, string sShortLabel, string sLabel)
        {
            if (sShortLabel == null) sShortLabel = string.Empty;
            else sShortLabel = sShortLabel.Trim();

            if (sLabel == null) sLabel = string.Empty;
            else sLabel = sLabel.Trim();

            if (sComments == null) sComments = string.Empty;

            if (sShortLabel == string.Empty && sLabel == string.Empty) return sComments;

            if (sComments.StartsWith(sShortLabel + " - " + sLabel)) return sComments;

            if (sShortLabel != string.Empty || sLabel != string.Empty) sComments = sShortLabel + " - " + sLabel + "\r\n" + sComments;

            return sComments;
        }

        public static string CommentsFirstLineShortLabelLabelRemoval(string sComments, string sShortLabel, string sLabel)
        {
            if (sShortLabel == null) sShortLabel = string.Empty;
            else sShortLabel = sShortLabel.Trim();

            if (sLabel == null) sLabel = string.Empty;
            else sLabel = sLabel.Trim();

            if (sComments == null) sComments = string.Empty;

            if (sShortLabel == string.Empty && sLabel == string.Empty) return sComments;

            if (!sComments.StartsWith(sShortLabel + " - " + sLabel + "\r\n")) return sComments;

            if (sShortLabel != string.Empty || sLabel != string.Empty) sComments = sComments.Substring((sShortLabel + " - " + sLabel + "\r\n").Length);

            return sComments;
        }

        public static string XDFLabelSLabelComXdfComment(string sLabel, string sShortLabel, string sComment)
        {
            if (sLabel == null) sLabel = string.Empty;
            if (sShortLabel == null) sShortLabel = string.Empty;

            if (sComment == null) sComment = string.Empty;
            if (sLabel.Contains(sShortLabel.Trim())) return sComment;
            if (sComment.Trim().StartsWith(sShortLabel.Trim())) return sComment;
            if (sComment.Trim() == string.Empty) return sShortLabel;
            return sShortLabel + " - " + sComment;
        }

        public static string XDFLabelComToShortLabel(string sLabel, string sComment, string sDefaultSLabel)
        {
            Regex regExp = null;
            Match reSig = null;
            string sResult = string.Empty;

            if (sLabel == null) sLabel = string.Empty;
            if (sComment == null) sComment = string.Empty;
            if (sDefaultSLabel == null) sDefaultSLabel = string.Empty;
            
            regExp = new Regex("^(.*?)\\s-\\s", RegexOptions.None);

            reSig = regExp.Match(sLabel);
            if (!reSig.Success) reSig = regExp.Match(sComment);
            if (reSig.Success) sResult = reSig.Groups[1].Value;
            else if (!sLabel.Contains(" ")) sResult = sLabel;
            else if (!sComment.Contains(" ")) sResult = sComment;
            if (sResult != sResult.ToUpper()) sResult = string.Empty;
            reSig = null;
            regExp = null;

            if (sResult == string.Empty) sResult = sDefaultSLabel;

            return sResult;
        }

        public static string XDFLabelReview(string sLabel, string sShortLabel)
        {
            if (sLabel == null) sLabel = string.Empty;
            if (sShortLabel == null) sShortLabel = string.Empty;

            if (sLabel.StartsWith(sShortLabel + " - ") && sLabel.Length > sShortLabel.Length + 3) return sLabel.Substring(sShortLabel.Length + 3);
            else return sLabel;
        }

        public static string FNLabelToShortLabel(string sLabel, string sDefaultSLabel)
        {
            Regex regExp = null;
            Match reSig = null;
            string sResult = string.Empty;

            if (sLabel == null) sLabel = string.Empty;
            if (sDefaultSLabel == null) sDefaultSLabel = string.Empty;

            regExp = new Regex("(FN[^\\s]+)", RegexOptions.None);
            reSig = regExp.Match(sLabel);

            if (reSig.Success) sResult = reSig.Groups[1].Value;
            reSig = null;
            regExp = null;

            if (sResult == string.Empty) sResult = sDefaultSLabel;

            return sResult;
        }

        public static int CompareS6xSignatureCateg(S6xSignature s6xSig1, S6xSignature s6xSig2)
        {
            if (s6xSig1 == null && s6xSig2 == null) return 0;
            if (s6xSig1 == null) return -1;
            if (s6xSig2 == null) return 1;

            if ((s6xSig1.SignatureCategory == null || s6xSig1.SignatureCategory == string.Empty) && (s6xSig2.SignatureCategory == null || s6xSig2.SignatureCategory == string.Empty)) return s6xSig1.UniqueKey.CompareTo(s6xSig2.UniqueKey);
            if (s6xSig1.SignatureCategory == null || s6xSig1.SignatureCategory == string.Empty) return 1;   // Empty Categ appears at the end
            if (s6xSig2.SignatureCategory == null || s6xSig2.SignatureCategory == string.Empty) return -1;  // Empty Categ appears at the end

            if (s6xSig1.SignatureCategory != s6xSig2.SignatureCategory) return s6xSig1.SignatureCategory.CompareTo(s6xSig2.SignatureCategory);

            return s6xSig1.UniqueKey.CompareTo(s6xSig2.UniqueKey);
        }

        public static int CompareS6xElementSignatureCateg(S6xElementSignature s6xESig1, S6xElementSignature s6xESig2)
        {
            if (s6xESig1 == null && s6xESig2 == null) return 0;
            if (s6xESig1 == null) return -1;
            if (s6xESig2 == null) return 1;

            if ((s6xESig1.SignatureCategory == null || s6xESig1.SignatureCategory == string.Empty) && (s6xESig2.SignatureCategory == null || s6xESig2.SignatureCategory == string.Empty)) return s6xESig1.UniqueKey.CompareTo(s6xESig2.UniqueKey);
            if (s6xESig1.SignatureCategory == null || s6xESig1.SignatureCategory == string.Empty) return 1;   // Empty Categ appears at the end
            if (s6xESig2.SignatureCategory == null || s6xESig2.SignatureCategory == string.Empty) return -1;  // Empty Categ appears at the end

            if (s6xESig1.SignatureCategory != s6xESig2.SignatureCategory) return s6xESig1.SignatureCategory.CompareTo(s6xESig2.SignatureCategory);

            return s6xESig1.UniqueKey.CompareTo(s6xESig2.UniqueKey);
        }
    }
}
