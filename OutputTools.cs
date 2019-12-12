using System;
using System.Text;

namespace SAD806x
{
    public static class OutputTools
    {
        public static int BorderedWidth = 120;
        public static int BorderedLeftMargin = 20;
        public static int BorderedRightMargin = 20;

        public static char BorderTopBottom = '═';
        public static char BorderLeftRight = '║';
        public static char BorderTopLeftCorner = '╔';
        public static char BorderTopRightCorner = '╗';
        public static char BorderBottomLeftCorner = '╚';
        public static char BorderBottomRightCorner = '╝';


        public static string BorderedHeader()
        {
            return BorderTopLeftCorner + new string(BorderTopBottom, BorderedWidth - 2) + BorderTopRightCorner;
        }

        public static string BorderedFooter()
        {
            return BorderBottomLeftCorner + new string(BorderTopBottom, BorderedWidth - 2) + BorderBottomRightCorner;
        }

        public static string BorderedTitle(string text)
        {
            if (text.Length > BorderedWidth - 2) text = text.Substring(0, BorderedWidth - 2);

            int leftMargin = (BorderedWidth - text.Length) / 2;
            int rightMargin = BorderedWidth - leftMargin - text.Length;
            string format = "{0,1}{1,1}{2,1}{3,1}{4,1}";
            return string.Format(format, BorderLeftRight, new string(' ', leftMargin - 1), text, new string(' ', rightMargin - 1), BorderLeftRight);
        }

        public static string BorderedText(string text)
        {
            if (text.Length > BorderedWidth - 2 - BorderedLeftMargin - BorderedRightMargin) text = text.Substring(0, BorderedWidth - 2 - BorderedLeftMargin - BorderedRightMargin);

            int leftMargin = BorderedLeftMargin;
            int rightMargin = BorderedWidth - leftMargin - text.Length;
            string format = "{0,1}{1,1}{2,1}{3,1}{4,1}";
            return string.Format(format, BorderLeftRight, new string(' ', leftMargin - 1), text, new string(' ', rightMargin - 1), BorderLeftRight);
        }

        public static string BorderedColumns(string[] arrText)
        {
            if (arrText == null) return BorderedEmpty();
            if (arrText.Length == 0) return BorderedEmpty();
            if (arrText.Length == 1) return BorderedText(arrText[0]);
            
            int totalSpace = BorderedWidth - BorderedLeftMargin - BorderedRightMargin - 2;
            
            int totalTextSpace = 0;
            foreach (string text in arrText) totalTextSpace += text.Length;

            int columnSize = totalSpace / arrText.Length;

            string lastColMargin = string.Empty;
            if (totalSpace % arrText.Length > 0) new string(' ', totalSpace % arrText.Length);
            
            string[] arrMarginedText = new string[arrText.Length];
            for (int iPos = 0; iPos < arrMarginedText.Length; iPos++)
            {
                string text = arrText[iPos];
                if (text.Length > columnSize - 1) text = text.Substring(0, columnSize - 1);
                string spaces = new string(' ', columnSize - text.Length);
                if (iPos % 2 == 0) arrMarginedText[iPos] = text + spaces;
                else arrMarginedText[iPos] = spaces + text;
                if (iPos == arrMarginedText.Length - 1) arrMarginedText[iPos] += lastColMargin;
            }

            return BorderedText(string.Join(string.Empty, arrMarginedText));
        }

        public static string BorderedEmpty()
        {
            return BorderLeftRight + new string(' ', BorderedWidth - 2) + BorderLeftRight;
        }

    }
}
