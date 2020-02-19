using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SAD806x
{
    public static class ToolsSettings
    {
        public static void Update(ref SettingsLst setSettings, string specType)
        {
            string currentVersion = string.Empty;
            
            switch (specType)
            {
                case "TEXTOUTPUT":
                    currentVersion = "0.2";
                    if (setSettings.version != currentVersion) UpdateTextOuput(ref setSettings);
                    break;
            }
        }

        private static void UpdateTextOuput(ref SettingsLst setSettings)
        {
            SortedList slDefaultSettings = new SortedList();
            SettingItem defItem = null;

            string defaultSampleOps = "8 92be: 37,33,0a             jnb   B7,R33,92cb        if (!B7_R33) goto 92cb;";
            defaultSampleOps += "\r\n8 92c1: 02,32                cplw  R32                R32 = ~R32;            ";
            defaultSampleOps += "\r\n8 92c3: 03,30                negw  R30                R30 = -R30;            ";
            defaultSampleOps += "\r\n8 92c5: a4,00,32             adcw  R32,0              R32 += 0 + CY;         ";
            
            string defaultSampleFill = "8 23d2 -> 23ff                     fill               ff";

            string defaultSampleOpsUnknown = "8 3bd8: 00,00,4c,01,00,00,8a,00                       Unknown Operation/Structure";
            defaultSampleOpsUnknown += "\r\n8 3be0: 00,00,00,00,00,00                             Unknown Operation/Structure";

            string defaultSampleData = "                                                         B7 B6 B5 B4 B3 B2 B1 B0                                                                                 ";
            defaultSampleData += "\r\n                                                          0  0  0  0  0  0  0  1                                                                                 ";
            defaultSampleData += "\r\n8 2440: 01                R7c+40 Sc0028       byte         1                   1                                                                                 ";
            defaultSampleData += "\r\n8 2441: ff                                    Unknown Calibration               ff                                  255                                          ";
            defaultSampleData += "\r\n8 2442: 00,01             R7c+42 Sc0029       word       100                 256                                                                                 ";
            defaultSampleData += "\r\n8 2444: 00,fe             R7c+44 Sc0030       word      fe00               65024                                                                                 ";

            string defaultSampleDataUnknown = "8 2f17: a1,af,bd,ca,c3,d1,da,d2               Unknown Calibration        a1, af, bd, ca, c3, d1, da, d2        161, 175, 189, 202, 195, 209, 218, 210";
            defaultSampleDataUnknown += "\r\n8 2f1f: da,e5,ef,fa,e9                        Unknown Calibration        da, e5, ef, fa, e9                    218, 229, 239, 250, 233               ";

            string defaultSampleFunc = "8 24be: ff,ff,00,0c       func                ffff,      c00               65535,    12,00";
            defaultSampleFunc += "\r\n8 24c2: e8,03,00,0c       func                 3e8,      c00                1000,    12,00";
            defaultSampleFunc += "\r\n8 24c6: c8,00,00,07       func                  c8,      700                 200,     7,00";
            defaultSampleFunc += "\r\n8 24ca: 0a,00,00,00       func                   a,        0                  10,     0,00";
            defaultSampleFunc += "\r\n8 24ce: 00,00,00,00       func                   0,        0                   0,     0,00";

            string defaultSampleTableByteScaled = "8 fbea: 0e,0f,12,13,14,15       table        e,  f, 12, 13, 14, 15           28,00,   30,00,   36,00,   38,00,   40,00,   42,00        ";
            defaultSampleTableByteScaled += "\r\n8 fbf0: 29,2f,31,32,33,34       table       29, 2f, 31, 32, 33, 34           82,00,   94,00,   98,00,  100,00,  102,00,  104,00        ";
            defaultSampleTableByteScaled += "\r\n8 fbf6: 45,4c,4f,51,52,54       table       45, 4c, 4f, 51, 52, 54          138,00,  152,00,  158,00,  162,00,  164,00,  168,00        ";
            defaultSampleTableByteScaled += "\r\n8 fbfc: 66,6a,6d,6e,70,74       table       66, 6a, 6d, 6e, 70, 74          204,00,  212,00,  218,00,  220,00,  224,00,  232,00        ";
            defaultSampleTableByteScaled += "\r\n8 fc02: 7e,84,8a,8b,8f,93       table       7e, 84, 8a, 8b, 8f, 93          252,00,  264,00,  276,00,  278,00,  286,00,  294,00        ";
            defaultSampleTableByteScaled += "\r\n8 fc08: 96,9e,a6,a8,ad,b3       table       96, 9e, a6, a8, ad, b3          300,00,  316,00,  332,00,  336,00,  346,00,  358,00        ";

            string defaultSampleTableByteUnScaled = "8 2bfe: 83,7a,5a,53,50,46,46       table       83, 7a, 5a, 53, 50, 46, 46        131, 122,  90,  83,  80,  70,  70";
            defaultSampleTableByteUnScaled += "\r\n8 2c05: 83,7a,56,4a,46,43,43       table       83, 7a, 56, 4a, 46, 43, 43        131, 122,  86,  74,  70,  67,  67";
            defaultSampleTableByteUnScaled += "\r\n8 2c0c: 7a,70,4d,40,40,40,40       table       7a, 70, 4d, 40, 40, 40, 40        122, 112,  77,  64,  64,  64,  64";
            defaultSampleTableByteUnScaled += "\r\n8 2c13: 6d,63,4a,40,40,40,40       table       6d, 63, 4a, 40, 40, 40, 40        109,  99,  74,  64,  64,  64,  64";
            defaultSampleTableByteUnScaled += "\r\n8 2c1a: 5a,53,43,40,40,40,40       table       5a, 53, 43, 40, 40, 40, 40         90,  83,  67,  64,  64,  64,  64";
            defaultSampleTableByteUnScaled += "\r\n8 2c21: 4a,43,40,40,40,40,40       table       4a, 43, 40, 40, 40, 40, 40         74,  67,  64,  64,  64,  64,  64";

            string defaultSampleTableWordScaled = "1 9296: 30,03,5d,03,b3,03,e0,03,23,04,6a,05       table        330,  35d,  3b3,  3e0,  423,  56a            25,50,     26,91,     29,59,     31,00,     33,09,     43,31        ";
            defaultSampleTableWordScaled += "\r\n1 92a2: 76,05,5a,06,76,06,86,06,33,06,90,06       table        576,  65a,  676,  686,  633,  690            43,69,     50,81,     51,69,     52,19,     49,59,     52,50        ";
            defaultSampleTableWordScaled += "\r\n1 92ae: ea,07,b3,08,3d,09,7a,09,20,09,76,09       table        7ea,  8b3,  93d,  97a,  920,  976            63,31,     69,59,     73,91,     75,81,     73,00,     75,69        ";
            defaultSampleTableWordScaled += "\r\n1 92ba: d3,0c,33,0e,c3,0e,3d,0f,73,0e,2a,0f       table        cd3,  e33,  ec3,  f3d,  e73,  f2a           102,59,    113,59,    118,09,    121,91,    115,59,    121,31        ";
            defaultSampleTableWordScaled += "\r\n1 92c6: dd,11,06,13,5d,14,8a,14,fd,13,43,14       table       11dd, 1306, 145d, 148a, 13fd, 1443           142,91,    152,19,    162,91,    164,31,    159,91,    162,09        ";
            defaultSampleTableWordScaled += "\r\n1 92d2: f6,14,30,17,b3,17,2a,19,33,18,4a,18       table       14f6, 1730, 17b3, 192a, 1833, 184a           167,69,    185,50,    189,59,    201,31,    193,59,    194,31        ";

            string defaultSampleTableWordUnScaled = "1 91f8: 30,03,5d,03,b3,03,e0,03,23,04,6a,05       table        330,  35d,  3b3,  3e0,  423,  56a          816,   861,   947,   992,  1059,  1386        ";
            defaultSampleTableWordUnScaled += "\r\n1 9204: 76,05,5a,06,76,06,86,06,33,06,90,06       table        576,  65a,  676,  686,  633,  690         1398,  1626,  1654,  1670,  1587,  1680        ";
            defaultSampleTableWordUnScaled += "\r\n1 9210: ea,07,b3,08,3d,09,7a,09,20,09,76,09       table        7ea,  8b3,  93d,  97a,  920,  976         2026,  2227,  2365,  2426,  2336,  2422        ";
            defaultSampleTableWordUnScaled += "\r\n1 921c: d3,0c,33,0e,c3,0e,3d,0f,73,0e,2a,0f       table        cd3,  e33,  ec3,  f3d,  e73,  f2a         3283,  3635,  3779,  3901,  3699,  3882        ";
            defaultSampleTableWordUnScaled += "\r\n1 9228: dd,11,06,13,5d,14,8a,14,fd,13,43,14       table       11dd, 1306, 145d, 148a, 13fd, 1443         4573,  4870,  5213,  5258,  5117,  5187        ";
            defaultSampleTableWordUnScaled += "\r\n1 9234: f6,14,30,17,b3,17,2a,19,33,18,4a,18       table       14f6, 1730, 17b3, 192a, 1833, 184a         5366,  5936,  6067,  6442,  6195,  6218        ";

            string defaultSampleVector = "8 4296: 41,44                4441  Bank 8 Vector      Sub0038";
            defaultSampleVector += "\r\n8 4298: b7,44                44b7  Bank 8 Vector      Sub0039";
            defaultSampleVector += "\r\n8 429a: 52,45                4552  Bank 8 Vector      Sub0040";


            defItem = new SettingItem("ACTIVE", "00000", "Activate Settings", SettingType.Boolean);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Define if settings have to be used, instead of original fixed behaviour.";
            defItem.DefaultValue = "0";
            defItem.Value = defItem.DefaultValue;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_ADDRESS_WIDTH", "00001", "Global Line Address Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Beginning for each line.\r\nIt is a minimum width.";
            defItem.DefaultValue = "6";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "6";
            defItem.MaxValue = "10";
            defItem.Sample = defaultSampleOps;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 0;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_OPS_CENTER_WIDTH", "00010", "Global Line Operations Like Center Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Center part for each line, related with operations or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "46";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "46";
            defItem.MaxValue = "100";
            defItem.Sample = defaultSampleOps;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 8;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_OPS_CENTER_PART1_WIDTH", "00011", "Global Line Operations Like Center Part 1 Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Center part, part 1 for each line, related with operations or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "21";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "21";
            defItem.MaxValue = "30";
            defItem.Sample = defaultSampleOps;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 8;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_OPS_CENTER_PART2_WIDTH", "00012", "Global Line Operations Like Center Part 2 Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Center part, part 2 for each line, related with operations or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "6";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "6";
            defItem.MaxValue = "20";
            defItem.Sample = defaultSampleOps;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 29;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_OPS_RIGHT_WIDTH", "00020", "Global Line Operations Like Right Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Right part for each line, related with operations or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "100";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "100";
            defItem.MaxValue = "200";
            defItem.Sample = defaultSampleOps;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 54;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_OPS_RIGHT_PART1_WIDTH", "00021", "Global Line Operations Like Right Part 1 Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Right part, part 1 for each line, related with operations or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "21";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "21";
            defItem.MaxValue = "50";
            defItem.Sample = defaultSampleOps;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 54;
            slDefaultSettings.Add(defItem.UniqueId, defItem);
            
            defItem = new SettingItem("GLOBAL_LINE_CAL_CENTER_WIDTH", "00030", "Global Line Data Like Center Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Center part for each line, related with data or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "52";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "52";
            defItem.MaxValue = "100";
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 8;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_CAL_CENTER_PART1_WIDTH", "00031", "Global Line Data Like Center Part 1 Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Center part, part 1 for each line, related with data or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "18";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "18";
            defItem.MaxValue = "30";
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 8;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_CAL_CENTER_PART2_WIDTH", "00032", "Global Line Data Like Center Part 2 Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Center part, part 2 for each line, related with data or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "20";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "20";
            defItem.MaxValue = "40";
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 26;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_CAL_CENTER_PART3_WIDTH", "00033", "Global Line Data Like Center Part 3 Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Center part, part 3 for each line, related with data or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "8";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "8";
            defItem.MaxValue = "20";
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 46;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_CAL_RIGHT_WIDTH", "00040", "Global Line Data Like Right Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Right part for each line, related with data or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "83";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "83";
            defItem.MaxValue = "200";
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 60;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_CAL_RIGHT_PART1_WIDTH", "00041", "Global Line Data Like Right Part 1 Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Right part, part 1 for each line, related with data or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "20";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "20";
            defItem.MaxValue = "40";
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 60;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_CAL_RIGHT_PART2_WIDTH", "00042", "Global Line Data Like Right Part 2 Width", SettingType.Number);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Right part, part 2 for each line, related with data or equivalent.\r\nIt is a minimum width.";
            defItem.DefaultValue = "10";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "10";
            defItem.MaxValue = "40";
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 80;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TABLE_SEPARATOR1_WIDTH", "00050", "Tables/Structures First Separator Width", SettingType.Number);
            defItem.Category = "Table/Structure";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "First separator width, related with tables or structures.\r\nIt is a minimum width.";
            defItem.DefaultValue = "7";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "7";
            defItem.MaxValue = "20";
            defItem.Sample = defaultSampleTableByteScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 25;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TABLE_SEPARATOR2_WIDTH", "00051", "Tables/Structures Second Separator Width", SettingType.Number);
            defItem.Category = "Table/Structure";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Second separator width, related with tables or structures.\r\nIt is a minimum width.";
            defItem.DefaultValue = "7";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "7";
            defItem.MaxValue = "20";
            defItem.Sample = defaultSampleTableByteScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 37;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TABLE_SEPARATOR3_WIDTH", "00052", "Tables/Structures Third Separator Width", SettingType.Number);
            defItem.Category = "Table/Structure";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Third separator width, related with tables or structures.\r\nIt is a minimum width.";
            defItem.DefaultValue = "8";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "8";
            defItem.MaxValue = "20";
            defItem.Sample = defaultSampleTableByteScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 66;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TABLE_BYTE_HEX_WIDTH", "00055", "Tables Byte Hex Cell Width", SettingType.Number);
            defItem.Category = "Table/Structure";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Cell Width for hex byte values only.\r\nIt is a minimum width.";
            defItem.DefaultValue = "3";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "3";
            defItem.MaxValue = "10";
            defItem.Sample = defaultSampleTableByteScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 47;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TABLE_WORD_HEX_WIDTH", "00056", "Tables Word Hex Cell Width", SettingType.Number);
            defItem.Category = "Table/Structure";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Cell Width for hex word values only.\r\nIt is a minimum width.";
            defItem.DefaultValue = "5";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "5";
            defItem.MaxValue = "10";
            defItem.Sample = defaultSampleTableWordScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 67;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TABLE_BYTE_DEC_UNSCALED_WIDTH", "00060", "Tables Byte Decimal Cell Width (Not Scaled)", SettingType.Number);
            defItem.Category = "Table/Structure";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Cell Width for non scaled byte values only.\r\nIt is a minimum width.";
            defItem.DefaultValue = "4";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "4";
            defItem.MaxValue = "10";
            defItem.Sample = defaultSampleTableByteUnScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 85;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TABLE_WORD_DEC_UNSCALED_WIDTH", "00061", "Tables Word Decimal Cell Width (Not Scaled)", SettingType.Number);
            defItem.Category = "Table/Structure";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Cell Width for non scaled word values only.\r\nIt is a minimum width.";
            defItem.DefaultValue = "6";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "6";
            defItem.MaxValue = "10";
            defItem.Sample = defaultSampleTableWordUnScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 110;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TABLE_BYTE_DEC_SCALED_WIDTH", "00062", "Tables Byte Decimal Cell Width (Scaled)", SettingType.Number);
            defItem.Category = "Table/Structure";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Cell Width for scaled byte values only.\r\nIt is a minimum width.";
            defItem.DefaultValue = "8";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "8";
            defItem.MaxValue = "20";
            defItem.Sample = defaultSampleTableByteScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 83;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TABLE_WORD_DEC_SCALED_WIDTH", "00063", "Tables Word Decimal Cell Width (Scaled)", SettingType.Number);
            defItem.Category = "Table/Structure";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Cell Width for scaled word values only.\r\nIt is a minimum width.";
            defItem.DefaultValue = "10";
            defItem.Value = defItem.DefaultValue;
            defItem.MinValue = "10";
            defItem.MaxValue = "20";
            defItem.Sample = defaultSampleTableWordScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 114;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_BASE_FOLLOWER", "00100", "Global Line Address Follower", SettingType.Text);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Line address follower and separator for all lines.";
            defItem.DefaultValue = ": ";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleOps;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 6;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("GLOBAL_LINE_FILL_FOLLOWER", "00101", "Global Line Address Follower for Fill", SettingType.Text);
            defItem.Category = "Global";
            defItem.Information = defItem.Label + "\r\n\r\n";
            defItem.Information += "Line address follower and separator for all fill lines.";
            defItem.DefaultValue = " -> ";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleFill;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 6;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_UNKNOWNOPERATION", "00200", "Type for Unknown Operation line", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "Unknown Operation/Structure";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleOpsUnknown;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 54;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_UNKNOWNOPERATIONFILL", "00201", "Type for Unknown Operation fill line", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "fill";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleFill;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 35;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_UNKNOWNCALIBRATION", "00202", "Type for Unknown Data line", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "Unknown Calibration";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleDataUnknown;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 46;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_UNKNOWNCALIBRATIONFILL", "00203", "Type for Unknown Data fill line", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "fill";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleFill;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 35;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_CALELEM_SCALAR_BYTE", "00204", "Type for Data Byte Scalar", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "byte";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 46;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_CALELEM_SCALAR_WORD", "00205", "Type for Data Word Scalar", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "word";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 46;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_EXT_SCALAR_BYTE", "00206", "Type for Other Byte Scalar", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "obyte";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 46;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_EXT_SCALAR_WORD", "00207", "Type for Other Word Scalar", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "oword";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleData;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 46;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_CALELEM_FUNCTION", "00208", "Type for Data Function", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "func";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleFunc;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 26;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_EXT_FUNCTION", "00209", "Type for Other Function", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "ofunc";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleFunc;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 26;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_CALELEM_TABLE", "00210", "Type for Data Table", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "table";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleTableByteScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 32;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_EXT_TABLE", "00211", "Type for Other Table", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "otable";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleTableByteScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 32;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_CALELEM_STRUCT", "00212", "Type for Data Structure", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "struct";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleTableByteScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 32;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_EXT_STRUCT", "00213", "Type for Other Structure", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "ostruct";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleTableByteScaled;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 32;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            defItem = new SettingItem("TYPE_VECTOR", "00214", "Type for Vector", SettingType.Text);
            defItem.Category = "Types";
            defItem.Information = defItem.Label + ".";
            defItem.DefaultValue = "Vector";
            defItem.Value = defItem.DefaultValue;
            defItem.Sample = defaultSampleVector;
            defItem.SampleMode = SettingSampleMode.StartAndValue;
            defItem.SampleSelectionStart = 42;
            slDefaultSettings.Add(defItem.UniqueId, defItem);

            SortedList slProvidedSettings = new SortedList();
            foreach (SettingItem sItem in setSettings.Items)
            {
                try { slProvidedSettings.Add(sItem.UniqueId, sItem); }
                catch {}
            }

            bool updateSettings = false;
            foreach (string sKey in slDefaultSettings.Keys)
            {
                if (slProvidedSettings.ContainsKey(sKey))
                {
                    updateSettings |= ((SettingItem)slProvidedSettings[sKey]).Refresh((SettingItem)slDefaultSettings[sKey]);
                }
                else
                {
                    slProvidedSettings.Add(sKey, slDefaultSettings[sKey]);
                    updateSettings = true;
                }
            }

            if (updateSettings)
            {
                setSettings.Items.Clear();
                foreach (SettingItem sItem in slProvidedSettings.Values) setSettings.Items.Add(sItem);
            }

            setSettings.version = "0.2";
        }
    }
}
