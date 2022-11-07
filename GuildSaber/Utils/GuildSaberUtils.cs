using GuildSaber.API;
using GuildSaber.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zenject;
using System.Reflection;
using UnityEngine;
using TMPro;
using System;
using System.Security.Policy;
using System.Collections;
using System.Net;
using System.IO;
using GuildSaber.BSPModule;

namespace GuildSaber.Utils
{
    public static class GuildSaberUtils
    {
        #region Statics Utils
        public static bool GuildsListContainsId(List<GuildData> p_Guilds, int p_GuildId)
        {
            foreach (var l_Current in p_Guilds)
            {
                if (l_Current.ID != p_GuildId)
                    continue;

                return true;
            }

            return false;
        }
        public static GuildData GetGuildFromId(int p_Id)
        {
            foreach (GuildData l_Current in GuildSaberModule.AvailableGuilds)
            {
                if (l_Current.ID != p_Id)
                    continue;

                return l_Current;
            }
            return default(GuildData);
        }
        public static bool IsGuildValidForPlayer(int p_Guild)
        {
            bool l_IsValid = false;
            foreach (GuildData l_Current in GuildSaberModule.AvailableGuilds)
            {
                if (l_Current.ID == p_Guild)
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

        private static IEnumerator DelayCoroutine(IEnumerator p_NewCoroutine, float p_Seconds)
        {
            yield return new WaitForSeconds(p_Seconds);

            CP_SDK.Unity.MTCoroutineStarter.Start(p_NewCoroutine);

            yield return null;
        }

        public static Coroutine StartCoroutineWithDelay(IEnumerator p_Coroutine, float p_Seconds)
        {
            return CP_SDK.Unity.MTCoroutineStarter.Start(DelayCoroutine(p_Coroutine, p_Seconds));
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
        public static VertexGradient GenerateGradient(this UnityEngine.Color p_Base, float p_Difference, float p_Multiplier = 1f)
        {
            UnityEngine.Color l_Color0 = new((p_Base.r * (1 + p_Difference)) * p_Multiplier, (p_Base.g * (1 + p_Difference)) * p_Multiplier, (p_Base.b * (1 + p_Difference)) * p_Multiplier);
            UnityEngine.Color l_Color1 = new((p_Base.r * (1 - p_Difference)) * p_Multiplier, (p_Base.g * (1 - p_Difference)) * p_Multiplier, (p_Base.b * (1 - p_Difference)) * p_Multiplier);
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

        public static GameObject FindGm(string p_Query)
        {
            GameObject l_lastGM = null;
            string[] l_Gms = p_Query.Split('.');
            foreach (string l_Current in l_Gms)
            {
                if (l_lastGM == null)
                {
                    l_lastGM = GameObject.Find(l_Gms[0]);
                    continue;
                }

                l_lastGM = l_lastGM.transform.Find(l_Current).gameObject;

            }

            return l_lastGM;
        }
        #endregion

        #region Enums
        public enum ErrorMode
        {
            StackTrace, Message
        }
        #endregion

        public async static Task<Texture2D> GetImage(string p_Url)
        {
            Texture2D l_NewTexture = new Texture2D(100, 100);

            float l_Time = UnityEngine.Time.realtimeSinceStartup;
            string l_Link = $"UserData/BeatSaberPlus/GuildSaber/TempImg{l_Time.ToString("0")}.png";

            using (WebClient l_Client = new WebClient())
            {
                bool l_MoveNext = false;
                l_Client.DownloadFileAsync(new System.Uri(p_Url), l_Link);
                l_Client.DownloadFileCompleted += (p_Sender, p_EventArgs) =>
                {
                    if (p_EventArgs.Error != null)
                    {
                        l_MoveNext = true;
                        return;
                    }

                    using (FileStream l_Reader = new FileStream(l_Link, FileMode.Open)) {
                        byte[] l_StreamBytes = new byte[l_Reader.Length];
                        l_Reader.Read(l_StreamBytes, 0, (int)l_Reader.Length);
                        l_NewTexture.LoadImage(l_StreamBytes,false);
                        l_MoveNext = true;
                    }
                };

                await WaitUtils.WaitUntil(() => l_MoveNext, 10);
            }

            File.Delete(l_Link);

            return l_NewTexture;
        }

    }
}
