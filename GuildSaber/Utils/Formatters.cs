using PlaylistManager.HarmonyPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.Utils
{
    internal static class Formatters
    {
        public const int SECONDS_IN_A_YEAR = 60 * 60 * 24 * 365;
        public const int SECONDS_IN_A_MONTH = 60 * 60 * 24 * 30;
        public const int SECONDS_IN_A_WEEK = 60 * 60 * 24 * 7;
        public const int SECONDS_IN_A_DAY = 60 * 60 * 24;

        private static string[] DatesNames = new string[6] { "Year", "Month", "Day", "Hour", "Minute", "Second" };

        public static string TimeFormatFromUnix(long p_Duration)
        {
            /*int l_Duration = (int)p_Duration;
            int l_Years = (l_Duration / SECONDS_IN_A_YEAR);
            int l_Months = (l_Duration - (l_Years * SECONDS_IN_A_YEAR)) / SECONDS_IN_A_MONTH;
            int l_Weeks = (l_Duration - (l_Years * SECONDS_IN_A_YEAR) - (l_Months * SECONDS_IN_A_MONTH)) / SECONDS_IN_A_WEEK;
            int l_Days = (l_Duration - (l_Years * SECONDS_IN_A_YEAR) - (l_Months * SECONDS_IN_A_MONTH) - (l_Weeks * SECONDS_IN_A_WEEK)) / SECONDS_IN_A_DAY;*/
            /*int l_Hours = l_Duration / (60 * 60);
            int l_Minutes = l_Duration / 60;
            int l_Seconds = l_Duration - (l_Minutes * 60) - (l_Hours * 60);*/

            DateTime l_Time = CP_SDK.Misc.Time.FromUnixTime(CP_SDK.Misc.Time.UnixTimeNow() - p_Duration);

            string l_Result = string.Empty;

            int[] l_Values = { l_Time.Year - 1970, l_Time.Month, l_Time.Day, l_Time.Hour, l_Time.Minute, l_Time.Second };
            int l_NumberOfAddedElements = 0;

            for (int l_i = 0; l_i < 7; l_i++)
            {
                int l_Value = l_Values[l_i];

                if (l_Value == 0) continue;

                string l_DateName = DatesNames[l_i];
                if (l_Value > 1)
                    l_DateName += "s";
                l_Result += $"{l_Value} {l_DateName}" + (l_i == 6 ? string.Empty : " ");
                l_NumberOfAddedElements += 1;

                if (l_NumberOfAddedElements == 3)
                    break;
            }

            return l_Result;
        }

        public static string SimpleTimeFormat(float p_Duration)
        {
            int l_Minutes = (int)(p_Duration / 60);
            int l_Hours = (int)(p_Duration / (60 * 60));
            int l_Seconds = (int)p_Duration - (l_Minutes * 60) - (l_Hours * 60);

            string l_SHours = (l_Hours > 0) ? $"{l_Hours:00}:" : string.Empty;

            return $"{l_SHours}{l_Minutes:00}:{l_Seconds:00}";
        }
    }
}
