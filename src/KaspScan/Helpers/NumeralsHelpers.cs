namespace KaspScan.Helpers
{
    public static class NumeralsHelpers
    {
        public static string GetFeminineWordInNominativeCase(string wordWithoutEnd, int number)
        {
            var units = number % 10;
            var tens = number / 10 % 10;

            if (tens != 1 && units == 1)
                return $"{wordWithoutEnd}а";

            if (tens != 1 && units >= 2 && units <= 4)
                return $"{wordWithoutEnd}ы";

            return $"{wordWithoutEnd}";
        }
        public static string GetMasculineWordInDativeCase(string wordWithoutEnd, int number)
        {
            var units = number % 10;
            var tens = number / 10 % 10;

            if (tens != 1 && units == 1)
                return wordWithoutEnd;

            if (tens != 1 && units >= 2 && units <= 4)
                return $"{wordWithoutEnd}а";

            return $"{wordWithoutEnd}ов";
        }


        public static string GetFeminineWordInDativeCase(string wordWithoutEnd, int number)
        {
            var units = number % 10;
            var tens = number / 10 % 10;

            if (tens != 1 && units == 1)
                return $"{wordWithoutEnd}у";

            if (tens != 1 && units >= 2 && units <= 4)
                return $"{wordWithoutEnd}ы";

            return $"{wordWithoutEnd}";
        }
    }
}
