using System;
using System.Collections.Generic;
using System.Text;

namespace Tools
{
    class KeyGen
    {
        private const string UpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowerChars = "abcdefghijklmnopqrstuvwxyz";
        private const string DigitChars = "01234567890123456789";

        public static string Generate(int bitdepth = 64, GenFlags flags = GenFlags.LOWER_CH | GenFlags.DIGIT_CH)
        {
            string source = null;

            switch ((int)flags)
            {
                case 1:
                    source = UpperChars;
                    break;
                case 2:
                    source = LowerChars;
                    break;
                case 3:
                    source = UpperChars + LowerChars;
                    break;
                case 4:
                    source = DigitChars;
                    break;
                case 5:
                    source = UpperChars + DigitChars;
                    break;
                case 6:
                    source = LowerChars + DigitChars;
                    break;
                case 7:
                    source = UpperChars + LowerChars + DigitChars;
                    break;
                default:
                    break;
            }
            
            StringBuilder builder = new StringBuilder(bitdepth);
            Random random = new Random();

            for (int i = 0; i < bitdepth; i++)
            {
                builder.Append(source[random.Next(0, source.Length)]);
            }
            return builder.ToString();
        }   
    }
    enum GenFlags : int
    {
        /// <summary>
        /// For uper string chars.
        /// </summary>
        UPPER_CH = 1,
        /// <summary>
        /// For lower string chars.
        /// </summary>
        LOWER_CH = 2,
        /// <summary>
        /// For available digital chars.
        /// </summary>
        DIGIT_CH = 4,
    }
}
