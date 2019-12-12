using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SAD806x
{
    public static class ToolsDebug
    {
        public static void WriteLog(string filePath, string sText)
        {
            WriteLog(filePath, sText, false);
        }

        public static void WriteLog(string filePath, string sText, bool createFile)
        {
            TextWriter txWriter = null;
            txWriter = new StreamWriter(filePath, !createFile, Encoding.UTF8);
            txWriter.WriteLine(sText);
            txWriter.Close();
            txWriter.Dispose();
            txWriter = null;
        }
    }
}
