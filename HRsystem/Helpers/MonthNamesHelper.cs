using System;

namespace HRsystem.Helpers
{
    public static class MonthNamesHelper
    {
        /// <summary>Gregorian (miladi) month name for 1..12, e.g. 1 = يناير</summary>
        public static string GetGregorianMonthName(int month)
        {
            string[] arabicMonths = { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };
            return arabicMonths[month - 1];
        }
    }
}