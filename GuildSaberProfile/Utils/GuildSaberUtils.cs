using GuildSaberProfile.API;
using GuildSaberProfile.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zenject;
using System.Reflection;
using UnityEngine;
using TMPro;
using System;

namespace GuildSaberProfile.Utils
{
    public static class GuildSaberUtils
    {
        #region Statics Utils
        public static int GetStaticPlayerLevel(string p_Category = null)
        {
            //Gettings all levels
            List<int> l_Levels = new List<int>() { 0 };
            l_Levels = GetLevelCache(PluginConfig.Instance.SelectedGuild, p_Category);
            int l_LastLevel = l_Levels[0];
            if (l_Levels.Count == 0)
                return -1;

            //Getting Max Level
            for (int l_i = 0; l_i < l_Levels.Count - 1;l_i++)
            {
                if (l_Levels.Count - 1 != l_i) {
                    if (l_LastLevel + 1 == l_Levels[l_i + 1])
                    {
                        l_LastLevel = l_Levels[l_i + 1];
                    }
                }
            }

            return l_LastLevel;
        }

        public static List<int> GetLevelCache(string p_Guild, string p_CategoryName = null)
        {
            List<int> l_Levels = new List<int>() { -1, -2 };
            try
            {
                using (HttpClient l_Client = new HttpClient())
                {
                    string l_Link = string.Empty;
                    switch (p_Guild)
                    {
                        case "CS":
                            l_Link = $"{ReturnLinkFromGuild("CS")}/levelcache/{p_CategoryName}";
                            break;
                        case "BSCC":
                            l_Link = $"{ReturnLinkFromGuild("BSCC")}/levelcache/";
                            break;
                        default: return null;
                    }

                    //Wait for result
                    Task<string> l_Result = l_Client.GetStringAsync(l_Link);
                    l_Result.Wait();
                    if (l_Result.Result == string.Empty) {
                        Plugin.Log.Info("Result is null");
                        return new List<int> { 0 };
                    }
                    //Function explicit
                    LevelIDs l_LevelIDs = JsonConvert.DeserializeObject<LevelIDs>(l_Result.Result);
                    l_Levels = l_LevelIDs.LevelID;
                }
            } catch (HttpRequestException p_E)
            {
                Plugin.Log.Error(p_E);
            }
            return l_Levels;
        }

        public static string ReturnLinkFromGuild(string p_Guild)
        {
            switch (p_Guild)
            {
                case "CS":
                    return "http://api.bsdr.fdom.eu";
                case "BSCC":
                    return "https://api.jupilian.me";
                default: return string.Empty;
            }
        }
        public static bool IsGuildValidForPlayer(string p_Guild)
        {
            bool l_IsValid = false;
            foreach (string l_Current in Plugin.AvailableGuilds)
            {
                if (l_Current == p_Guild)
                {
                    Plugin.Log.Info("Selected guild is valid for this player not changing");
                    l_IsValid = true;
                    break;
                }
            }
            return l_IsValid;
        }

        public static string GetPlayerNameToFit(string p_Name, int p_NumberOfChars)
        {
            if (p_Name.Length > p_NumberOfChars)
            {
                string l_NewName = string.Empty;
                for (int l_i = 0; l_i < p_Name.Length; l_i++)
                {
                    if (l_i <= p_NumberOfChars)
                        l_NewName += p_Name[l_i].ToString();
                    else
                    {
                        l_NewName += "...";
                        return l_NewName;
                    }
                }
            }
            return p_Name;
        }

        public static string VerifiedCategory(this string p_Cat)
        {
            if (p_Cat.StringIsNullOrEmpty()) return "Default";
            return p_Cat;
        }
        #endregion

        #region Zenject
        public static readonly PropertyInfo m_ContainerPropertyInfo = typeof(MonoInstallerBase).GetProperty("Container", BindingFlags.Instance | BindingFlags.NonPublic);
        public static DiContainer GetContainer(this MonoInstallerBase p_MonoInstallerBase)
        {
            return (DiContainer)m_ContainerPropertyInfo.GetValue(p_MonoInstallerBase);
        }
        #endregion

        #region Extensions
        public static bool ColorEquals(this UnityEngine.Color p_ColorBase, UnityEngine.Color p_ColorToCheck, float p_Tolerance)
        {
            bool l_Return = p_ColorToCheck.r.IsIn(p_ColorBase.r - p_Tolerance, p_ColorBase.r + p_Tolerance)
                         && p_ColorToCheck.g.IsIn(p_ColorBase.g - p_Tolerance, p_ColorBase.g + p_Tolerance)
                         && p_ColorToCheck.b.IsIn(p_ColorBase.b - p_Tolerance, p_ColorBase.b + p_Tolerance);
            return l_Return;
        }
        public static bool IsIn(this float l_Value, float l_Min, float l_Max)
        {
            return (l_Value < l_Max && l_Value > l_Min);
        }

        public static bool Greater(this UnityEngine.Color p_Base, UnityEngine.Color p_ColorToCheck)
        {
            return p_ColorToCheck.r > p_Base.r && p_ColorToCheck.g > p_Base.g && p_ColorToCheck.b > p_Base.b;
        }
        public static VertexGradient GenerateGradient(this UnityEngine.Color p_Base, float p_Difference)
        {
            UnityEngine.Color l_Color0 = new(p_Base.r * (1 + p_Difference), p_Base.g * (1 + p_Difference), p_Base.b * (1 + p_Difference));
            UnityEngine.Color l_Color1 = new(p_Base.r * (1 - p_Difference), p_Base.g * (1 - p_Difference), p_Base.b * (1 - p_Difference));
            return new(l_Color1, l_Color1, l_Color0, l_Color0);
        }

        public static void SetTextError(this TextMeshProUGUI p_Text, Exception p_Ex, ErrorMode p_Mode)
        {
            p_Text.color = UnityEngine.Color.red;
            switch (p_Mode)
            {
                case ErrorMode.StackTrace:
                    p_Text.text = p_Ex.StackTrace;
                    break;
                case ErrorMode.Message:
                    p_Text.text = p_Ex.Message;
                    break;
                default: return;
            }
        }
        public static int Clamp(this int p_Value, int p_Min, int p_Max)
        {
            if (p_Value < p_Min) p_Value = p_Min;
            else if (p_Value > p_Max) p_Value = p_Max;
            return p_Value;
        }

        public static bool StringIsNullOrEmpty(this string p_Value)
        {
            return p_Value == null || p_Value == string.Empty;
        }
        #endregion

        #region Enums
        public enum ErrorMode
        {
            StackTrace, Message
        }
        #endregion

    }
}
