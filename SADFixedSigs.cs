using System;
using System.Collections;
using System.Text;

namespace SAD806x
{
    // Hard Coded Signature, Will be reused at S6x Definition Creation / Update
    public static class SADFixedSigs
    {
        // Fixed Routines Signatures Array
        // UniqueKey, Calibration Element Enum (Enum), Bytes Signature (String), Signature Label (String), Signature Categ (String), Signature Categ 2 (String), Signature Categ 2 (String), Signature Comments (String)
        public static object[] Fixed_Routines_Signatures = new object[] {
            new object[] { "CORE_REG_INIT_8065_01", Fixed_Routines.CORE_REG_INIT_8065, Fixed_Routine_Signature_REG_INIT_8065_01, "Core Registers Initialization (01 forced signature)", "8065_Registers", "", "", "Automatically added for signatures processing.\r\nSpecific to some strategies which use it for constant registers." },
            new object[] { "OBDII_REG_INIT_01", Fixed_Routines.OBDII_REG_INIT, Fixed_Routine_Signature_OBDII_MIS_Init_01, "OBDII Registers Initialization (01 forced signature)", "8065_OBDII", "", "", "Automatically added for signatures post processing.\r\nFirst part for OBDII codes identification." },
            new object[] { "OBDII_REG_INIT_01_01", Fixed_Routines.OBDII_REG_INIT, Fixed_Routine_Signature_OBDII_MIS_Init_01_01, "OBDII Registers Initialization (01bis Forced signature)", "8065_OBDII", "", "", "Automatically added for signatures post processing.\r\nFirst part for OBDII codes identification." },
            new object[] { "OBDII_REG_INIT_02", Fixed_Routines.OBDII_REG_INIT, Fixed_Routine_Signature_OBDII_MIS_Init_02, "OBDII Registers Initialization (02 forced signature)", "8065_OBDII", "", "", "Automatically added for signatures post processing.\r\nFirst part for OBDII codes identification."},
            new object[] { "OBDII_REG_RESET_01", Fixed_Routines.OBDII_REG_RESET, Fixed_Routine_Signature_OBDII_MIS_Reset_01, "OBDII Registers Reset Signature (01 forced signature)", "8065_OBDII", "", "", "Automatically added for signatures post processing.\r\nSecond part for OBDII codes identification." },
            new object[] { "OBDII_REG_RESET_01_01", Fixed_Routines.OBDII_REG_RESET, Fixed_Routine_Signature_OBDII_MIS_Reset_01_01, "OBDII Registers Reset Signature (01bis forced signature)", "8065_OBDII", "", "", "Automatically added for signatures post processing.\r\nSecond part for OBDII codes identification." },
            new object[] { "OBDII_REG_RESET_02", Fixed_Routines.OBDII_REG_RESET, Fixed_Routine_Signature_OBDII_MIS_Reset_02, "OBDII Registers Reset Signature (02 forced signature)", "8065_OBDII", "", "", "Automatically added for signatures post processing.\r\nSecond part for OBDII codes identification." },
            new object[] { "OBDII_REG_FLAGS_02", Fixed_Routines.OBDII_REG_FLAGS, Fixed_Routine_Signature_OBDII_MIS_Flags_02, "OBDII Registers Flags Signature (02 forced signature)", "8065_OBDII", "", "", "Automatically added for signatures post processing.\r\nThird part for OBDII codes identification." },
            new object[] { "OBDII_REG_FLAGS_02_01", Fixed_Routines.OBDII_REG_FLAGS, Fixed_Routine_Signature_OBDII_MIS_Flags_02_01, "OBDII Registers Flags Signature (02bis forced signature)", "8065_OBDII", "", "", "Automatically added for signatures post processing.\r\nThird part for OBDII codes identification." },
            new object[] { "OBDI_COD_3D_01", Fixed_Routines.OBDI_COD_3D, Fixed_Routine_Signature_OBDI_Cod_3d_01, "OBDI Codes 3 Digits (01 forced signature)", "8061_OBDI", "", "", "Automatically added for signatures post processing.\r\nFirst part for OBDI 3 Digits codes identification." },
            new object[] { "OBDI_CNT_3D_01", Fixed_Routines.OBDI_CNT_3D, Fixed_Routine_Signature_OBDI_Cnt_3d_01, "OBDI Count 3 Digits (01 forced signature)", "8061_OBDI", "", "", "Automatically added for signatures post processing.\r\nSecond part for OBDI 3 Digits codes identification." },
            new object[] { "OBDI_COD_2D_01", Fixed_Routines.OBDI_COD_2D, Fixed_Routine_Signature_OBDI_Cod_2d_01, "OBDI Codes 2 Digits (01 forced signature)", "8061_OBDI", "", "", "Automatically added for signatures post processing.\r\nFirst part for OBDI 2 Digits codes identification. European version." },
            new object[] { "OBDI_TIM_2D_01", Fixed_Routines.OBDI_TIM_2D, Fixed_Routine_Signature_OBDI_Tim_2d_01, "OBDI Timings 2 Digits (01 forced signature)", "8061_OBDI", "", "", "Automatically added for signatures post processing.\r\nSecond part for OBDI 2 Digits codes identification. European version." },
            new object[] { "OBDI_MALFUNC", Fixed_Routines.OBDI_MALFUNC, Fixed_Routine_Signature_OBDI_MALFUNC, "OBDI Malfunction 3 Digits (01 forced signature)", "8061_OBDI", "Registers_Args", "", "Automatically added for signatures post processing.\r\nPart of OBDI 3 Digits registers identification." },
            new object[] { "OBDI_MALFUNC_LW", Fixed_Routines.OBDI_MALFUNC_LW, Fixed_Routine_Signature_OBDI_MALFUNC_LW, "OBDI Malfunction Lower 3 Digits (01 forced signature)", "8061_OBDI", "Registers_Args", "", "Automatically added for signatures post processing.\r\nPart of OBDI 3 Digits registers identification." },
            new object[] { "OBDI_MALFUNC_BT", Fixed_Routines.OBDI_MALFUNC_BT, Fixed_Routine_Signature_OBDI_MALFUNC_BT, "OBDI Malfunction Between 3 Digits (01 forced signature)", "8061_OBDI", "Registers_Args", "", "Automatically added for signatures post processing.\r\nPart of OBDI 3 Digits registers identification." },
            new object[] { "OBDI_MALFUNC_SB", Fixed_Routines.OBDI_MALFUNC_SB, Fixed_Routine_Signature_OBDI_MALFUNC_SB, "OBDI Malfunction Sub 3 Digits (01 forced signature)", "8061_OBDI", "", "Registers_Args", "Automatically added for signatures post processing.\r\nPart of OBDI 3 Digits registers identification." },
            new object[] { "OBDI_MALFUNC_EX", Fixed_Routines.OBDI_MALFUNC_EX, Fixed_Routine_Signature_OBDI_MALFUNC_EX, "OBDI Malfunction Extended 3 Digits (01 forced signature)", "8061_OBDI", "Registers_Args", "", "Automatically added for signatures post processing.\r\nPart of OBDI 3 Digits registers identification." },
            new object[] { "OBDII_CLEAR_MALF_01", Fixed_Routines.OBDII_CLEAR_MALF, Fixed_Routine_Signature_OBDII_CLEAR_MALF_01, "OBDII Clear Malfunction (01 forced signature)", "8065_OBDII", "Registers_Args", "", "Automatically added for signatures post processing.\r\nPart of OBDII registers identification." },
            new object[] { "OBDII_CLEAR_MALF_02", Fixed_Routines.OBDII_CLEAR_MALF, Fixed_Routine_Signature_OBDII_CLEAR_MALF_02, "OBDII Clear Malfunction (02 forced signature)", "8065_OBDII", "Registers_Args", "", "Automatically added for signatures post processing.\r\nPart of OBDII registers identification." },
            new object[] { "OBDII_MALFUNCTION_01", Fixed_Routines.OBDII_MALFUNCTION, Fixed_Routine_Signature_OBDII_MALFUNCTION_01, "OBDII Malfunction (01 forced signature)", "8065_OBDII", "Registers_Args", "", "Automatically added for signatures post processing.\r\nPart of OBDII registers identification." },
            new object[] { "OBDII_MALFUNCTION_02", Fixed_Routines.OBDII_MALFUNCTION, Fixed_Routine_Signature_OBDII_MALFUNCTION_02, "OBDII Malfunction (02 forced signature)", "8065_OBDII", "Registers_Args", "", "Automatically added for signatures post processing.\r\nPart of OBDII registers identification." },
            new object[] { "OBDII_MALFUNCTION_02_01", Fixed_Routines.OBDII_MALFUNCTION, Fixed_Routine_Signature_OBDII_MALFUNCTION_02_01, "OBDII Malfunction (02bis forced signature)", "8065_OBDII", "Registers_Args", "", "Automatically added for signatures post processing.\r\nPart of OBDII registers identification." },
            new object[] { "RPM_MNGT_8061_01", Fixed_Routines.RPM_MNGT_8061_01, Fixed_Routine_Signature_RPM_MNGT_8061_01, "RPM Management 8061 (01 forced signature)", "Registers_RPM", "8061", "", "Automatically added for signatures post processing.\r\nFor RPM register identification." },
            new object[] { "RPM_MNGT_8061_02", Fixed_Routines.RPM_MNGT_8061_02, Fixed_Routine_Signature_RPM_MNGT_8061_02, "RPM Management 8061 (02 forced signature)", "Registers_RPM", "8061", "", "Automatically added for signatures post processing.\r\nFor RPM register identification." },
            new object[] { "RPM_MNGT_8065_01", Fixed_Routines.RPM_MNGT_8065_01, Fixed_Routine_Signature_RPM_MNGT_8065_01, "RPM Management 8065 (01 forced signature)", "Registers_RPM", "8065", "", "Automatically added for signatures post processing.\r\nFor RPM register identification." },
            new object[] { "RPM_MNGT_8065_01_01", Fixed_Routines.RPM_MNGT_8065_01, Fixed_Routine_Signature_RPM_MNGT_8065_01_01, "RPM Management 8065 (01bis forced signature)", "Registers_RPM", "8065", "", "Automatically added for signatures post processing.\r\nFor RPM register identification." },
            new object[] { "RPM_MNGT_8065_02", Fixed_Routines.RPM_MNGT_8065_02, Fixed_Routine_Signature_RPM_MNGT_8065_02, "RPM Management 8065 (02 forced signature)", "Registers_RPM", "8065", "", "Automatically added for signatures post processing.\r\nFor RPM register identification." },
            new object[] { "RPM_MNGT_8065_03", Fixed_Routines.RPM_MNGT_8065_03, Fixed_Routine_Signature_RPM_MNGT_8065_03, "RPM Management 8065 (03 forced signature)", "Registers_RPM", "8065", "", "Automatically added for signatures post processing.\r\nFor RPM register identification." },
            new object[] { "RPM_MNGT_8065_03_01", Fixed_Routines.RPM_MNGT_8065_03, Fixed_Routine_Signature_RPM_MNGT_8065_03_01, "RPM Management 8065 (03bis forced signature)", "Registers_RPM", "8065", "", "Automatically added for signatures post processing.\r\nFor RPM register identification." },
            new object[] { "ECT_INIT_8061_01", Fixed_Routines.ECT_INIT_8061_01, Fixed_Routine_Signature_ECT_INIT_8061_01, "ECT Initialization 8061 (01 forced signature)", "Registers_ECT_ACT", "", "", "Automatically added for signatures post processing.\r\nFor ECT/ACT registers identification and other related elements." },
            new object[] { "ECT_INIT_8061_02", Fixed_Routines.ECT_INIT_8061_02, Fixed_Routine_Signature_ECT_INIT_8061_02, "ECT Initialization 8061 (02 forced signature)", "Registers_ECT_ACT", "", "", "Automatically added for signatures post processing.\r\nFor ECT/ACT registers identification and other related elements." },
            new object[] { "ECT_CLIP_8065_01", Fixed_Routines.ECT_CLIP_8065_01, Fixed_Routine_Signature_ECT_CLIP_8065_01, "ECT Clip 8065 (01 forced signature)", "Registers_ECT_ACT", "", "", "Automatically added for signatures post processing.\r\nFor ECT/ACT registers identification and other related elements." },
            new object[] { "ECT_CLIP_8065_02", Fixed_Routines.ECT_CLIP_8065_02, Fixed_Routine_Signature_ECT_CLIP_8065_02, "ECT Clip 8065 (02 forced signature)", "Registers_ECT_ACT", "", "", "Automatically added for signatures post processing.\r\nFor ECT/ACT registers identification and other related elements." },
            new object[] { "ECT_CLIPW_8065_02", Fixed_Routines.ECT_CLIPW_8065_02, Fixed_Routine_Signature_ECT_CLIPW_8065_02, "ECT ClipW 8065 (02 forced signature)", "Registers_ECT_ACT", "", "", "Automatically added for signatures post processing.\r\nFor ECT/ACT registers identification and other related elements." },
            new object[] { "CHT_CLIPW_8065_02", Fixed_Routines.CHT_CLIPW_8065_02, Fixed_Routine_Signature_CHT_CLIPW_8065_02, "CHT ClipW 8065 (02 forced signature)", "Registers_ECT_ACT", "", "", "Automatically added for signatures post processing.\r\nFor ECT/ACT registers identification and other related elements." }
        };

        // Fixed Elements Signatures Array
        // UniqueKey, Label, Signature Categ (String), Signature Categ 2 (String), Signature Categ 3 (String), Comments, Is 8061 (True, False), Fixed Bank (-1, 0, 1, 8, 9), Calibration Element Enum (Enum), Bytes Signature (String)
        public static object[] Fixed_Elements_Signatures = new object[] {
            new object[] { "FN036_61", "MAF Transfer 8061", "FN036", "8061", "", "MAF Transfer Signature for 8061", true, string.Empty,  Fixed_Elements.MAF_TRANSFER, Fixed_8061_MAF_Transfer_Use_Following_Ops },
            new object[] { "FN036_65_01", "MAF Transfer 8065 01", "FN036", "8065", "", "MAF Transfer Signature for 8065 01", false, string.Empty,  Fixed_Elements.MAF_TRANSFER, Fixed_8065_MAF_Transfer_Use_Following_Ops_1 },
            new object[] { "FN036_65_02", "MAF Transfer 8065 02", "FN036", "8065", "", "MAF Transfer Signature for 8065 02", false, string.Empty,  Fixed_Elements.MAF_TRANSFER, Fixed_8065_MAF_Transfer_Use_Following_Ops_2 },
            new object[] { "FN036_65_03", "MAF Transfer 8065 03", "FN036", "8065", "", "MAF Transfer Signature for 8065 03", false, string.Empty,  Fixed_Elements.MAF_TRANSFER, Fixed_8065_MAF_Transfer_Use_Following_Ops_3 }
        };

        // Fixed Routines Enums
        public enum Fixed_Routines
        {
            UNKNOWN,
            CORE_REG_INIT_8065,
            OBDII_REG_INIT,
            OBDII_REG_RESET,
            OBDII_REG_FLAGS,
            OBDI_COD_3D,
            OBDI_CNT_3D,
            OBDI_COD_2D,
            OBDI_TIM_2D,
            OBDI_MALFUNC,
            OBDI_MALFUNC_LW,
            OBDI_MALFUNC_BT,
            OBDI_MALFUNC_SB,
            OBDI_MALFUNC_EX,
            OBDII_CLEAR_MALF,
            OBDII_MALFUNCTION,
            RPM_MNGT_8061_01,
            RPM_MNGT_8061_02,
            RPM_MNGT_8065_01,
            RPM_MNGT_8065_02,
            RPM_MNGT_8065_03,
            ECT_INIT_8061_01,
            ECT_INIT_8061_02,
            ECT_CLIP_8065_01,
            ECT_CLIP_8065_02,
            ECT_CLIPW_8065_02,
            CHT_CLIPW_8065_02
        }

        // Fixed Elements Enums
        public enum Fixed_Elements
        {
            UNKNOWN = 0,
            MAF_TRANSFER = 1
        }

        // Fixed Routines Signatures
        public const string Fixed_Routine_Signature_REG_INIT_8065_01 = "f2\na1,#01#,#02#,24\n46,25,00,26\ndf,2c\na2,25,2a\n30,26,03\nc6,27,00\n41,fe,ff,2a,28\n88,20,26\nd7,04\na1,..,..,26\n88,28,26\ndb,07\n17,05\nc2,27,00\n27,eb\n30,2a,04\nc7,2a,ff,00\n27,ce\na1,#03#,#04#,26\nb2,27,29\n99,ff,29\ndf,13\nb2,27,2a\n17,05\nb2,27,28\nb2,27,2b\nc6,28,2b\ne0,2a,f4\n27,e5\nf3\nf0";

        public const string Fixed_Routine_Signature_OBDII_MIS_Init_01 = "c7,..,..,00\n71,..,..\nc3,..,..,00\na1,..,..,..\n89,..,..,..\ndb,..";
        public const string Fixed_Routine_Signature_OBDII_MIS_Init_01_01 = "c7,..,..,..,00\n71,..,..\nc3,..,..,..,00\na1,..,..,..\n89,..,..,..\ndb,..";
        public const string Fixed_Routine_Signature_OBDII_MIS_Init_02 = "f2\nb3,01,..,..,..\n71,7f,..\nc7,01,..,..,..\nc3,01,..,..,00\nc3,01,..,..,00\nc7,01,..,..,00\nb3,01,..,..,..\n71,..,..\nc7,01,..,..,..\nb3,01,..,..,..\n71,..,..\nc7,01,..,..,..\n*\n11,3e\nb1,28,3f\na1,..,..,2a";
        public const string Fixed_Routine_Signature_OBDII_MIS_Reset_01 = "a2,..,..\n3f,..,..\n91,..,..\nc2,..,..\n49,..,..,..,..\n65,..,..,..\na2,..,..\n99,..,..\nd7,..\n8b,..,..,..\nd7,..\n8b,..,..,..";
        public const string Fixed_Routine_Signature_OBDII_MIS_Reset_01_01 = "b1,..,..\n49,..,..,..,..\n65,..,..,..\na2,..,..\nc6,..,..\nc6,..,..\n79,..,..\ndf,..\n88,..,..\nd9,..\nc6,..,00";
        public const string Fixed_Routine_Signature_OBDII_MIS_Reset_02 = "f2\n28,..\n49,..,..,..,..\n65,..,..,..\na2,..,..\nb2,..,..\n99,..,..\nd7,..\na2,..,..\nef,..,..";
        public const string Fixed_Routine_Signature_OBDII_MIS_Flags_02 = "49,..,..,..,..\n08,01,..\n65,..,..,..\nb2,..,..\n99,..,..\nd7,..\nb3,01,..,..,..\n91,..,..\nc7,01,..,..,..\n20,..";
        public const string Fixed_Routine_Signature_OBDII_MIS_Flags_02_01 = "49,..,..,..,..\n08,01,..\n65,..,..,..\n99,..,..\nd7,..\nb3,01,..,..,..\n91,..,..\nc7,01,..,..,..\n20,..";
        
        public const string Fixed_Routine_Signature_OBDI_Cod_3d_01 = "01,..\n89,..,..,..\ndb,..\n88,..,00\nd7,..\na1,12,05,..\n3e,..,..\na1,11,01,..\na0,..,..\n08,03,..\nb3,..,..,..,..\n51,07,..,..\nb1,08,..\n78,..,..18,..,..";
        public const string Fixed_Routine_Signature_OBDI_Cnt_3d_01 = "fa\nff\na2,..,..\nc2,..,00\nfb45,..,..,..,..\n64,..,..\n5e,..,..,..\nac,..,..\n68,..,..\ndb,..";
        public const string Fixed_Routine_Signature_OBDI_Cod_2d_01 = "af,01,..,..,..\na0,..,..\n08,03,..\naf,..,..,..,..\n3f,..,..\naf,..,..,..,..\nb2,..,..\na0,..,..\n9d,08,..\n18,..,..";
        //public const string Fixed_Routine_Signature_OBDI_Tim_2d_01 = "36,..,09\na3,..,..,..,..\nef,..,..\nf0\n38,..,15\n3a,..,03\ne7,..,..\n45,0a,00,..,..\n4b,..,..,..,..,..\n37,..,09\nf0\n48,..,06,..\n37,..,01\nf0\n37,..,0d\na3,..,..,..,...\nef,..,..\n3f,..,66\n21,..\nac,..,..\n64,..,..\n38,..,19\n3a,..,0b\n4f,..,..,..,..,..\n0d,06,..\n20,11\n4f,..,..,..,..,..\n0d,06,..\n20,06\n4f,..,..,..,..,..\nc4,..,..\ncb,..,..,..";
        public const string Fixed_Routine_Signature_OBDI_Tim_2d_01 = "ac,..,..\n64,..,..\n38,..,..\n3a,..,..\n4f,..,..,..,..,..\n0d,..,..\n20,..\n4f,..,..,..,..,..\n0d,06,..\n20,..\n4f,..,..,..,..,..";

        public const string Fixed_Routine_Signature_OBDI_MALFUNC = "ef,..,..\n02\nc3,31,..,..,1a\n89,26,00,30\ndb,04\n07,30\n07,30\nf0";
        public const string Fixed_Routine_Signature_OBDI_MALFUNC_LW = "2d,..\n06\n71,fd,2c\n20,1e\nf8\n20,01\nf9\ncc,18\nb2,19,1a\nb2,19,1b\nb2,19,1c\nb2,19,1d\nd3,05\nc9,..,..\n20,02";
        public const string Fixed_Routine_Signature_OBDI_MALFUNC_BT = "ef,..,..\n08\n2.,..\n1c,00\n2.,..\n1e,00\na2,1a,42\n8a,1c,42\nd3,05\n8a,1e,42\nd1,05\na0,20,1a\n2f,d2\nf0";
        public const string Fixed_Routine_Signature_OBDI_MALFUNC_SB = "c3,31,..,..,1a\n89,26,00,30\ndb,04\n07,30\n07,30\nf0";
        public const string Fixed_Routine_Signature_OBDI_MALFUNC_EX = "ef,..,..\n0a\nef,..,..\n1c,00\nef,..,..\n1e,00\na2,1a,42\n01,1a\n8a,1d,42\ndb,0e\na1,27,03,1a\n8a,1e,42\nd1,17\na0,20,1a\n20,12";

        public const string Fixed_Routine_Signature_OBDII_CLEAR_MALF_01 = "f2\n2.,..\n00\nf2\na2,2a,3e\n37,3e,06\n71,7f,3e\nc2,2a,3e\nf3\nf0";
        public const string Fixed_Routine_Signature_OBDII_CLEAR_MALF_02 = "f2\n2.,..\n2.,..\n28,0d\n2.,..\nf3\nf0\nf2\n2.,..\n28,04\n2.,..\nf3\nf0\na2,2a,3e\n37,3e,06\n71,7f,3e\nc2,2a,3e\nf0\n";

        public const string Fixed_Routine_Signature_OBDII_MALFUNCTION_01 = "f2\n2f,de\n00\nf2\na2,2a,3e\n28,05\nc2,2a,3e\nf3\nf0\nf2\n";
        public const string Fixed_Routine_Signature_OBDII_MALFUNCTION_02 = "f2\n2.,..\n2.,..\n20,03\nf2\n2.,..\nb3,..,..,..,3e\n3.,3e,02\n20,26\n49,..,..,2a,3e\n45,..,..,3e,26\na2,26,26\nb2,26,26\n99,02,26\ndf,0f\n99,01,26\nd7,0c\na2,2a,3e\n2.,..\nc2,2a,3e\n20,02\n28,04\n2f,..\nf3\nf0\n";
        public const string Fixed_Routine_Signature_OBDII_MALFUNCTION_02_01 = "f2\n2.,..\n2.,..\n20,03\nf2\n2.,..\nb0,..,3e\n3.,3e,02\n20,26\n49,..,..,2a,3e\n45,..,..,3e,26\na2,26,26\nb2,26,26\n99,02,26\ndf,0f\n99,01,26\nd7,0c\na2,2a,3e\n2.,..\nc2,2a,3e\n20,02\n28,04\n2f,..\nf3\nf0\n";

        public const string Fixed_Routine_Signature_RPM_MNGT_8061_01 = "01,..\n37,..,..\n3a,..,..\n20,..\n71,..,..\na1,..,..,..\n01,..\n8f,..,..,..\n09,..,..\na3,..,..,..,..\n89,..,..,..\ndb,..";
        public const string Fixed_Routine_Signature_RPM_MNGT_8061_02 = "a3,..,..,..\n89,..,..,..\nd3,..\nfa\nb1,01,32\nc7,..,..,32\nc7,..,..,32\n71,..,..\n71,..,..\n71,..,..\n01,..\nfb";
        public const string Fixed_Routine_Signature_RPM_MNGT_8065_01 = "f2\nb0,..,..\na3,..,..,..,..\n89,..,..,..\nd3,..\n91,..,..\nfa\n71,..,..\n71,..,..\n71,..,..\n01,..\nfb";
        public const string Fixed_Routine_Signature_RPM_MNGT_8065_01_01 = "f2\nb0,..,..\na3,..,..,..,..\n89,..,..,..\nd3,..\n91,..,..\nfa\n71,..,..\n71,..,..\n01,..\nfb";
        public const string Fixed_Routine_Signature_RPM_MNGT_8065_02 = "f2\nb0,..,..\na3,..,..,..,..\n89,..,..,..\ndb,..\n99,..,..\ndf,..\n48,..,..,..\n89,..,..,..\nd1,..\n91,..,..\nfa\n71,..,..\n71,..,..\n*\n01,..\nfb";
        public const string Fixed_Routine_Signature_RPM_MNGT_8065_03 = "f2\nb3,..,..,..\na3,..,..,..\n89,..,..,..\ndb,..\n99,..,..\ndf,..\n48,..,..,..\n89,..,..,..\nd1,..\n91,..,..\nfa\n71,..,..\n71,..,..\n*\n01,..\nfb";
        public const string Fixed_Routine_Signature_RPM_MNGT_8065_03_01 = "f2\nb3,..,..,..\na3,..,..,..,..\n89,..,..,..\nd3,..\n91,..,..\nfa\n71,..,..\n71,..,..\n*\n01,..\nfb";

        public const string Fixed_Routine_Signature_ECT_INIT_8061_01 = "c7,..,..,..,..\nb0,..,..\nb3,..,..,..\nfe,5c,..,..,..\n13,..\n75,..,..\nfe,7c,..,..\n64,..,..\nc3,..,..,..,..";
        public const string Fixed_Routine_Signature_ECT_INIT_8061_02 = "ef,..,..\n3d,..,19\na3,..,..,..\n8b,..,..,..\nd9,0f\n8b,..,..,..\nd3,09\nef,..,..\n..,..,..,..\n20,16\n3a,..,..\n9b,..,..,..\ndf,05\nb0,..,..\n20,08";
        public const string Fixed_Routine_Signature_ECT_CLIP_8065_01 = "f2\n89,..,..,..\nd6,05\nb1,..,..\n20,09\n89,..,..,..\nda,03\nb1,..,..\nf3\nf0";
        public const string Fixed_Routine_Signature_ECT_CLIP_8065_02 = "f2\n89,..,..,..\nda,06\na1,..,..,..\n20,0b\n89,c1,ff,..\nd6,05\nb1,..,..\n11,..\nf3\nf0";
        public const string Fixed_Routine_Signature_ECT_CLIPW_8065_02 = "f2\n89,..,..,..\nda,06\na1,..,..,..\n20,0a\n89,c1,ff,..\nd6,04\na1,..,..,..\nf3\nf0";
        public const string Fixed_Routine_Signature_CHT_CLIPW_8065_02 = "f2\n89,..,..,..\nda,06\na1,..,..,..\n20,0a\n89,00,ff,..\nd6,04\na1,..,..,..\nf3\nf0";

        // Fixed Elements Bytes Signatures
        public const string Fixed_String_OpeIncludingElemAddress = "#EAOP#";
        
        // Only .. have to be used
        public const string Fixed_8061_MAF_Transfer_Use_Following_Ops = "#EAOP#\nA3,..,..,..\n88,..,..\nD9,12\n45,78,00,..,..\n88,..,..\nD3,08\n94,..,..\n71,03,..\nDF,04\nC3,..,..,..\n71,..,..\nF0";
        public const string Fixed_8065_MAF_Transfer_Use_Following_Ops_1 = "#EAOP#\nA3,..,..,..\n88,..,..\nD9,12\n45,78,00,..,..\n88,..,..\nD3,08\n94,..,..\n71,03,..\nDF,04\nC3,..,..,..\n71,..,..\nF0";
        public const string Fixed_8065_MAF_Transfer_Use_Following_Ops_2 = "#EAOP#\nA3,..,..,..,..\n88,..,..\nD9,14\n45,78,00,..,..\n8B,..,..,..,..\nD3,08\n94,..,..\n71,03,..\nDF,05\nC3,..,..,..,..\n71,..,..\nF0";
        public const string Fixed_8065_MAF_Transfer_Use_Following_Ops_3 = "#EAOP#\nA3,..,..,..\n88,..,..\nD9,13\n45,78,00,..,..\n8B,..,..,..\nD3,08\n94,..,..\n71,03,..\nDF,04\nC3,..,..,..\nF0";

        // Get Fixed Routines Definitions
        public static S6xSignature GetFixedRoutineSignatureTemplate(Fixed_Routines fixedRoutine)
        {
            S6xSignature oRes = null;
            S6xStructure s6xStruct = null;

            switch (fixedRoutine)
            {
                case Fixed_Routines.CORE_REG_INIT_8065:
                    oRes = new S6xSignature();
                    oRes.for806x = Signature806xOptions.for8065Only.ToString();
                    oRes.ShortLabel = "CORE_REG_INIT";
                    oRes.Label = "Core Registers Initialization";
                    oRes.Comments = "CORE_REG_INIT - Core Registers Initialization";
                    oRes.InternalStructures = new S6xRoutineInternalStructure[2];
                    
                    s6xStruct = SADFixedStructures.GetFixedStructureTemplate(SADFixedStructures.FixedStructures.CORE_REG_INIT_ST1);
                    oRes.InternalStructures[0] = new S6xRoutineInternalStructure();
                    oRes.InternalStructures[0].VariableAddress = "#02##01#";
                    oRes.InternalStructures[0].VariableBankNum = "1";           // To be updated by processing
                    oRes.InternalStructures[0].ShortLabel = s6xStruct.ShortLabel;
                    oRes.InternalStructures[0].Label = s6xStruct.Label;
                    oRes.InternalStructures[0].Comments = s6xStruct.Comments;
                    oRes.InternalStructures[0].Number = s6xStruct.Number;       // To be updated by processing
                    oRes.InternalStructures[0].StructDef = s6xStruct.StructDef; // To be updated by processing

                    s6xStruct = SADFixedStructures.GetFixedStructureTemplate(SADFixedStructures.FixedStructures.CORE_REG_INIT_ST2);
                    oRes.InternalStructures[1] = new S6xRoutineInternalStructure();
                    oRes.InternalStructures[1].VariableAddress = "#04##03#";
                    oRes.InternalStructures[1].VariableBankNum = "1";     // To be updated by processing
                    oRes.InternalStructures[1].ShortLabel = s6xStruct.ShortLabel;
                    oRes.InternalStructures[1].Label = s6xStruct.Label;
                    oRes.InternalStructures[1].Comments = s6xStruct.Comments;
                    oRes.InternalStructures[1].Number = s6xStruct.Number;       // To be updated by processing
                    oRes.InternalStructures[1].StructDef = s6xStruct.StructDef; // To be updated by processing

                    s6xStruct = null;
                    break;
                case Fixed_Routines.OBDII_REG_INIT:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "OBDII_REG_INIT";
                    oRes.Label = "OBDII Registers Initialization";
                    oRes.Comments = "OBDII_REG_INIT - OBDII Registers Initialization";
                    break;
                case Fixed_Routines.OBDII_REG_RESET:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "OBDII_REG_RESET";
                    oRes.Label = "OBDII Registers Reset";
                    oRes.Comments = "OBDII_REG_RESET - OBDII Registers Reset";
                    break;
                case Fixed_Routines.OBDII_REG_FLAGS:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "OBDII_REG_FLAGS";
                    oRes.Label = "OBDII Registers Flags";
                    oRes.Comments = "OBDII_REG_FLAGS - OBDII Registers Flags";
                    break;
                case Fixed_Routines.OBDI_COD_3D:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "OBDI_CODES_3D";
                    oRes.Label = "OBDI Codes 3 Digits";
                    oRes.Comments = "OBDI_CODES_3D - OBDI Codes 3 Digits";
                    break;
                case Fixed_Routines.OBDI_CNT_3D:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "OBDI_COUNT_3D";
                    oRes.Label = "OBDI Count 3 Digits";
                    oRes.Comments = "OBDI_COUNT_3D - OBDI Count 3 Digits";
                    break;
                case Fixed_Routines.OBDI_COD_2D:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "OBDI_CODES_2D";
                    oRes.Label = "OBDI Codes 2 Digits";
                    oRes.Comments = "OBDI_CODES_2D - OBDI Codes 2 Digits";
                    break;
                case Fixed_Routines.OBDI_TIM_2D:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "OBDI_TIMINGS_2D";
                    oRes.Label = "OBDI Timings 2 Digits";
                    oRes.Comments = "OBDI_TIMINGS_3D - OBDI Timings 2 Digits";
                    break;
                case Fixed_Routines.OBDI_MALFUNC:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "MALFUNC";
                    oRes.Label = "Malfunction Routine";
                    oRes.Comments = "MALFUNC - Malfunction Routine";
                    oRes.Comments += "\r\n1st Param is OBD Error Code to be pushed.";
                    oRes.InputArguments = new S6xRoutineInputArgument[1];
                    oRes.InputArguments[0] = new S6xRoutineInputArgument();
                    oRes.InputArguments[0].Position = 1;
                    oRes.InputArguments[0].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[0].Position);
                    oRes.InputArguments[0].Encryption = (int)CallArgsMode.Standard;
                    oRes.InputArguments[0].Word = true;
                    oRes.InputArguments[0].Pointer = false;
                    break;
                case Fixed_Routines.OBDI_MALFUNC_LW:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "MALFUNC_LW";
                    oRes.Label = "Malfunction Lower Routine";
                    oRes.Comments = "MALFUNC_LW - Malfunction Lower Routine";
                    oRes.Comments += "\r\n1st Param should be lower than 2nd value (scalar or register)";
                    oRes.Comments += "\r\n2nd Param should be higher than 1st value (scalar or register)";
                    oRes.Comments += "\r\n3rd Param is OBD Error Code to be pushed.";
                    oRes.InputArguments = new S6xRoutineInputArgument[3];
                    oRes.InputArguments[0] = new S6xRoutineInputArgument();
                    oRes.InputArguments[0].Position = 1;
                    oRes.InputArguments[0].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[0].Position);
                    oRes.InputArguments[0].Encryption = (int)CallArgsMode.Mode3;
                    oRes.InputArguments[0].Word = true;
                    oRes.InputArguments[0].Pointer = true;
                    oRes.InputArguments[1] = new S6xRoutineInputArgument();
                    oRes.InputArguments[1].Position = 2;
                    oRes.InputArguments[1].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[1].Position);
                    oRes.InputArguments[1].Encryption = (int)CallArgsMode.Mode3;
                    oRes.InputArguments[1].Word = true;
                    oRes.InputArguments[1].Pointer = true;
                    oRes.InputArguments[2] = new S6xRoutineInputArgument();
                    oRes.InputArguments[2].Position = 3;
                    oRes.InputArguments[2].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[2].Position);
                    oRes.InputArguments[2].Encryption = (int)CallArgsMode.Standard;
                    oRes.InputArguments[2].Word = true;
                    oRes.InputArguments[2].Pointer = false;
                    break;
                case Fixed_Routines.OBDI_MALFUNC_BT:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "MALFUNC_BT";
                    oRes.Label = "Malfunction Between Routine";
                    oRes.Comments = "MALFUNC_BT - Malfunction Between Routine";
                    oRes.Comments += "\r\n1st Param should be between 2nd and 3rd values (scalar or register)";
                    oRes.Comments += "\r\n2nd Param should be the lowest value for comparison (scalar or register)";
                    oRes.Comments += "\r\n3rd Param should be the highest value for comparison (scalar or register)";
                    oRes.Comments += "\r\n4th Param is OBD Error Code to be pushed.";
                    oRes.InputArguments = new S6xRoutineInputArgument[4];
                    oRes.InputArguments[0] = new S6xRoutineInputArgument();
                    oRes.InputArguments[0].Position = 1;
                    oRes.InputArguments[0].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[0].Position);
                    oRes.InputArguments[0].Encryption = (int)CallArgsMode.Standard;
                    oRes.InputArguments[0].Word = true;
                    oRes.InputArguments[0].Pointer = true;
                    oRes.InputArguments[1] = new S6xRoutineInputArgument();
                    oRes.InputArguments[1].Position = 2;
                    oRes.InputArguments[1].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[1].Position);
                    oRes.InputArguments[1].Encryption = (int)CallArgsMode.Mode3;
                    oRes.InputArguments[1].Word = true;
                    oRes.InputArguments[1].Pointer = true;
                    oRes.InputArguments[2] = new S6xRoutineInputArgument();
                    oRes.InputArguments[2].Position = 3;
                    oRes.InputArguments[2].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[2].Position);
                    oRes.InputArguments[2].Encryption = (int)CallArgsMode.Mode3;
                    oRes.InputArguments[2].Word = true;
                    oRes.InputArguments[2].Pointer = true;
                    oRes.InputArguments[3] = new S6xRoutineInputArgument();
                    oRes.InputArguments[3].Position = 4;
                    oRes.InputArguments[3].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[3].Position);
                    oRes.InputArguments[3].Encryption = (int)CallArgsMode.Standard;
                    oRes.InputArguments[3].Word = true;
                    oRes.InputArguments[3].Pointer = false;
                    break;
                case Fixed_Routines.OBDI_MALFUNC_SB:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "MALFUNC_SB";
                    oRes.Label = "Malfunction Sub Routine";
                    oRes.Comments = "MALFUNC_SB - Malfunction Sub Routine";
                    oRes.Comments += "\r\nUsed to push an OBD Error Code into a rolling error register.";
                    break;
                case Fixed_Routines.OBDI_MALFUNC_EX:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "MALFUNC_EX";
                    oRes.Label = "Malfunction Extended Routine";
                    oRes.Comments = "MALFUNC_EX - Malfunction Extended Routine";
                    oRes.Comments += "\r\n1st Param should be higher than 2nd and 3rd values (scalar or register)";
                    oRes.Comments += "\r\n2nd Param should be the first lowest value for comparison (scalar or register)";
                    oRes.Comments += "\r\n3rd Param should be the second lowest value for comparison (scalar or register)";
                    oRes.Comments += "\r\n4th & 5th Params are OBD Error Code to be pushed.";
                    oRes.InputArguments = new S6xRoutineInputArgument[5];
                    oRes.InputArguments[0] = new S6xRoutineInputArgument();
                    oRes.InputArguments[0].Position = 1;
                    oRes.InputArguments[0].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[0].Position);
                    oRes.InputArguments[0].Encryption = (int)CallArgsMode.Standard;
                    oRes.InputArguments[0].Word = true;
                    oRes.InputArguments[0].Pointer = true;
                    oRes.InputArguments[1] = new S6xRoutineInputArgument();
                    oRes.InputArguments[1].Position = 2;
                    oRes.InputArguments[1].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[1].Position);
                    oRes.InputArguments[1].Encryption = (int)CallArgsMode.Mode3;
                    oRes.InputArguments[1].Word = true;
                    oRes.InputArguments[1].Pointer = true;
                    oRes.InputArguments[2] = new S6xRoutineInputArgument();
                    oRes.InputArguments[2].Position = 3;
                    oRes.InputArguments[2].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[2].Position);
                    oRes.InputArguments[2].Encryption = (int)CallArgsMode.Mode3;
                    oRes.InputArguments[2].Word = true;
                    oRes.InputArguments[2].Pointer = true;
                    oRes.InputArguments[3] = new S6xRoutineInputArgument();
                    oRes.InputArguments[3].Position = 4;
                    oRes.InputArguments[3].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[3].Position);
                    oRes.InputArguments[3].Encryption = (int)CallArgsMode.Standard;
                    oRes.InputArguments[3].Word = true;
                    oRes.InputArguments[3].Pointer = false;
                    oRes.InputArguments[4] = new S6xRoutineInputArgument();
                    oRes.InputArguments[4].Position = 5;
                    oRes.InputArguments[4].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[4].Position);
                    oRes.InputArguments[4].Encryption = (int)CallArgsMode.Standard;
                    oRes.InputArguments[4].Word = true;
                    oRes.InputArguments[4].Pointer = false;
                    break;
                case Fixed_Routines.OBDII_CLEAR_MALF:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "CLEAR_MALF";
                    oRes.Label = "Clear Malfunction Routine";
                    oRes.Comments = "CLEAR_MALF - Clear Malfunction Routine";
                    oRes.Comments += "\r\n1st Param is OBD Error Code to be deleted.";
                    oRes.InputArguments = new S6xRoutineInputArgument[1];
                    oRes.InputArguments[0] = new S6xRoutineInputArgument();
                    oRes.InputArguments[0].Position = 1;
                    oRes.InputArguments[0].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[0].Position);
                    oRes.InputArguments[0].Encryption = (int)CallArgsMode.Standard;
                    oRes.InputArguments[0].Word = true;
                    oRes.InputArguments[0].Pointer = false;
                    break;
                case Fixed_Routines.OBDII_MALFUNCTION:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "MALFUNCTION";
                    oRes.Label = "Malfunction Routine";
                    oRes.Comments = "MALFUNCTION - Malfunction Routine";
                    oRes.Comments += "\r\n1st Param is OBD Error Code to be pushed.";
                    oRes.InputArguments = new S6xRoutineInputArgument[1];
                    oRes.InputArguments[0] = new S6xRoutineInputArgument();
                    oRes.InputArguments[0].Position = 1;
                    oRes.InputArguments[0].UniqueKey = string.Format("Ra{0:d3}", oRes.InputArguments[0].Position);
                    oRes.InputArguments[0].Encryption = (int)CallArgsMode.Standard;
                    oRes.InputArguments[0].Word = true;
                    oRes.InputArguments[0].Pointer = false;
                    break;
                case Fixed_Routines.RPM_MNGT_8061_01:
                case Fixed_Routines.RPM_MNGT_8061_02:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "RPM_MNGT";
                    oRes.Label = "RPM Management";
                    oRes.for806x = Signature806xOptions.for8061Only.ToString();
                    oRes.Comments = "RPM_MNGT - RPM Management";
                    break;
                case Fixed_Routines.RPM_MNGT_8065_01:
                case Fixed_Routines.RPM_MNGT_8065_02:
                case Fixed_Routines.RPM_MNGT_8065_03:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "RPM_MNGT";
                    oRes.Label = "RPM Management";
                    oRes.for806x = Signature806xOptions.for8065Only.ToString();
                    oRes.Comments = "RPM_MNGT - RPM Management";
                    break;
                case Fixed_Routines.ECT_INIT_8061_01:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "ECT_INIT";
                    oRes.Label = "ECT Initialization";
                    oRes.for806x = Signature806xOptions.for8061Only.ToString();
                    oRes.Comments = "ECT_INIT - ECT Initialization";
                    break;
                case Fixed_Routines.ECT_INIT_8061_02:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "BASE_INIT";
                    oRes.Label = "Base Initialization";
                    oRes.for806x = Signature806xOptions.for8061Only.ToString();
                    oRes.Comments = "BASE_INIT - Base Initialization";
                    break;
                case Fixed_Routines.ECT_CLIP_8065_01:
                case Fixed_Routines.ECT_CLIP_8065_02:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "ECT_CLIP";
                    oRes.Label = "ECT Clip";
                    oRes.for806x = Signature806xOptions.for8065Only.ToString();
                    oRes.Comments = "ECT_CLIP - ECT Clip";
                    break;
                case Fixed_Routines.ECT_CLIPW_8065_02:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "ECT_CLIPW";
                    oRes.Label = "ECT Clip W";
                    oRes.for806x = Signature806xOptions.for8065Only.ToString();
                    oRes.Comments = "ECT_CLIPW - ECT Clip W";
                    break;
                case Fixed_Routines.CHT_CLIPW_8065_02:
                    oRes = new S6xSignature();
                    oRes.ShortLabel = "CHT_CLIPW";
                    oRes.Label = "CHT Clip W";
                    oRes.for806x = Signature806xOptions.for8065Only.ToString();
                    oRes.Comments = "CHT_CLIPW - CHT Clip W";
                    break;
                default:
                    break;
            }

            return oRes;
        }

        // Get Fixed Elements Definitions
        public static object GetFixedElementS6xRoutineInternalTemplate(Fixed_Elements fixedElement)
        {
            object oRes = null;

            switch (fixedElement)
            {
                case Fixed_Elements.MAF_TRANSFER:
                    oRes = new S6xRoutineInternalFunction();
                    ((S6xRoutineInternalFunction)oRes).ShortLabel = "FN036";
                    ((S6xRoutineInternalFunction)oRes).Label = "MAF Transfer";
                    ((S6xRoutineInternalFunction)oRes).Comments = "";
                    ((S6xRoutineInternalFunction)oRes).ByteInput = false;
                    ((S6xRoutineInternalFunction)oRes).ByteOutput = false;
                    ((S6xRoutineInternalFunction)oRes).SignedInput = false;
                    ((S6xRoutineInternalFunction)oRes).SignedOutput = false;
                    ((S6xRoutineInternalFunction)oRes).RowsNumber = 0;
                    ((S6xRoutineInternalFunction)oRes).InputScaleExpression = "X";
                    ((S6xRoutineInternalFunction)oRes).OutputScaleExpression = "X";
                    ((S6xRoutineInternalFunction)oRes).InputScalePrecision = SADDef.DefaultScalePrecision;
                    ((S6xRoutineInternalFunction)oRes).OutputScalePrecision = SADDef.DefaultScalePrecision;
                    break;
                default:
                    break;
            }

            return oRes;
        }

    }
}
