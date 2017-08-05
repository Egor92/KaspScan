using System;

namespace KaspScan.Helpers
{
    public static class RandomStringHelper
    {
        public static string GetWord(int length, WordCase wordCase = WordCase.WithACapitalLetter)
        {
            return GetWord(length, length, wordCase);
        }

        public static string GetWord(int minLength, int maxLength, WordCase wordCase = WordCase.WithACapitalLetter)
        {
            if (minLength <= 0)
                throw new ArgumentException("minLength must be positive", nameof(minLength));
            if (maxLength <= 0)
                throw new ArgumentException("maxLength must be positive", nameof(maxLength));
            if (minLength > maxLength)
                throw new ArgumentException("maxLength must be more than minLength");
            var length = RandomHelper.GetInt(minLength, maxLength);
            var signs = new char[length];
            for (int i = 0; i < length; i++)
                signs[i] = GetChar(i, wordCase);
            return new string(signs);
        }

        private static char GetChar(int index, WordCase wordCase)
        {
            char minChar;
            char maxChar;
            switch (wordCase)
            {
                case WordCase.LowerCase:
                    minChar = 'a';
                    maxChar = 'z';
                    break;
                case WordCase.UpperCase:
                    minChar = 'A';
                    maxChar = 'Z';
                    break;
                case WordCase.Mixed:
                    if (RandomHelper.GetBool())
                    {
                        minChar = 'a';
                        maxChar = 'z';
                    }
                    else
                    {
                        minChar = 'A';
                        maxChar = 'Z';
                    }
                    break;
                case WordCase.Default:
                case WordCase.WithACapitalLetter:
                    minChar = index == 0
                        ? 'A'
                        : 'a';
                    maxChar = index == 0
                        ? 'Z'
                        : 'z';
                    break;
                default:
                    throw new Exception();
            }
            return RandomHelper.GetChar(minChar, maxChar);
        }

        public static string GetHexColor(object arg)
        {
            if (arg == null)
                return null;
            var argb = arg.GetHashCode();
            return $"#{argb,8:X8}";
        }

        public static string GetHexColor(object arg, byte alfa)
        {
            if (arg == null)
                return null;
            var rgb = Math.Abs(arg.GetHashCode()) % (255 * 255 * 255);
            return $"#{alfa,2:X2}{rgb,6:X6}";
        }
    }
}
