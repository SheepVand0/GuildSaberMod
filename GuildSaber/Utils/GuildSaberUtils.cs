using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using CP_SDK.Network;
using GuildSaber.API;
using GuildSaber.Logger;
using TMPro;
using UnityEngine;
using Zenject;

namespace GuildSaber.Utils;

public static class GuildSaberUtils
{

    public enum ErrorMode
    {
        StackTrace,
        Message
    }

    // ReSharper disable once AssignNullToNotNullAttribute
    public static readonly PropertyInfo s_ContainerPropertyInfo = typeof(MonoInstallerBase).GetProperty("Container", BindingFlags.Instance | BindingFlags.NonPublic);
    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////
    /// Statics Utils
    /// <summary>
    ///     Guild List contains this guild id ?
    /// </summary>
    public static bool GuildsListContainsId(List<GuildData> p_Guilds, int p_GuildId)
    {
        foreach (GuildData l_Current in p_Guilds) {
            if (l_Current.ID != p_GuildId) continue;

            return true;
        }

        return false;
    }
    public static GuildData GetGuildFromId(int p_Id)
    {
        foreach (GuildData l_Current in GuildSaberModule.AvailableGuilds) {
            if (l_Current.ID != p_Id) continue;

            return l_Current;
        }
        return default(GuildData);
    }
    public static bool IsGuildValidForPlayer(int p_Guild)
    {
        bool l_IsValid = false;
        foreach (GuildData l_Current in GuildSaberModule.AvailableGuilds)
            if (l_Current.ID == p_Guild) {
                l_IsValid = true;
                break;
            }
        return l_IsValid;
    }

    public static string GetPlayerNameToFit(string p_Name, int p_NumberOfChars)
    {
        if (p_Name.Length > p_NumberOfChars) {
            string l_NewName = string.Empty;
            for (int l_i = 0; l_i < p_Name.Length; l_i++)
                if (l_i <= p_NumberOfChars) { l_NewName += p_Name[l_i].ToString(); }
                else {
                    l_NewName += "...";
                    return l_NewName;
                }
        }
        return p_Name;
    }

    public static string VerifiedCategory(this string p_Cat)
    {
        if (string.IsNullOrEmpty(p_Cat)) return "Default";
        return p_Cat;
    }
    public static DiContainer GetContainer(this MonoInstallerBase p_MonoInstallerBase) { return (DiContainer)s_ContainerPropertyInfo.GetValue(p_MonoInstallerBase); }

    public static bool ColorEquals(this UnityEngine.Color p_ColorBase, UnityEngine.Color p_ColorToCheck, float p_Tolerance)
    {
        bool l_Return = p_ColorToCheck.r.IsIn(p_ColorBase.r - p_Tolerance, p_ColorBase.r + p_Tolerance)
                        && p_ColorToCheck.g.IsIn(p_ColorBase.g - p_Tolerance, p_ColorBase.g + p_Tolerance)
                        && p_ColorToCheck.b.IsIn(p_ColorBase.b - p_Tolerance, p_ColorBase.b + p_Tolerance);
        return l_Return;
    }
    public static bool IsIn(this float p_Value, float p_Min, float p_Max) { return p_Value < p_Max && p_Value > p_Min; }

    public static bool Greater(this UnityEngine.Color p_Base, UnityEngine.Color p_ColorToCheck) { return p_ColorToCheck.r > p_Base.r && p_ColorToCheck.g > p_Base.g && p_ColorToCheck.b > p_Base.b; }
    public static VertexGradient GenerateGradient(this UnityEngine.Color p_Base, float p_Difference, float p_Multiplier = 1f)
    {
        var l_Color0 = new UnityEngine.Color(p_Base.r * (1 + p_Difference) * p_Multiplier, p_Base.g * (1 + p_Difference) * p_Multiplier, p_Base.b * (1 + p_Difference) * p_Multiplier);
        var l_Color1 = new UnityEngine.Color(p_Base.r * (1 - p_Difference) * p_Multiplier, p_Base.g * (1 - p_Difference) * p_Multiplier, p_Base.b * (1 - p_Difference) * p_Multiplier);
        return new VertexGradient(l_Color1, l_Color1, l_Color0, l_Color0);
    }

    public static UnityEngine.Color Add(this UnityEngine.Color p_Color, float p_Value) { return new UnityEngine.Color(p_Color.r + p_Value, p_Color.g + p_Value, p_Color.b + p_Value); }
    public static VertexGradient GetVerticalGradient(UnityEngine.Color p_Color0, UnityEngine.Color p_Color1) { return new VertexGradient(p_Color0, p_Color0, p_Color1, p_Color1); }

    public static UnityEngine.Color Equilibrate(UnityEngine.Color p_Color0, UnityEngine.Color p_Color1, float p_Color0Multiplier = 1) { return new UnityEngine.Color(p_Color0.r * p_Color0Multiplier + p_Color1.r / 2, p_Color0.g * p_Color0Multiplier + p_Color1.g / 2, p_Color0.b * p_Color0Multiplier + p_Color1.b / 2); }


    public static bool IsIn(this UnityEngine.Color p_Color, UnityEngine.Color p_Color1) { return p_Color.r >= p_Color1.r && p_Color.g >= p_Color1.g && p_Color.b >= p_Color1.b; }

    public static UnityEngine.Color FloorTo(this UnityEngine.Color p_Color, float p_Floor) { return new UnityEngine.Color(p_Color.r < p_Floor ? p_Floor : p_Color.r, p_Color.g < p_Floor ? p_Floor : p_Color.g, p_Color.g < p_Floor ? p_Floor : p_Color.g); }

    public static void SetTextError(this TextMeshProUGUI p_Text, Exception p_Ex, ErrorMode p_Mode)
    {
        p_Text.color = UnityEngine.Color.red;
        switch (p_Mode) {
            case ErrorMode.StackTrace:
                p_Text.text = "<i>" + p_Ex.StackTrace;
                break;
            case ErrorMode.Message:
                p_Text.text = "<i>" + p_Ex.Message;
                break;
            default:
                return;
        }
    }
    public static int Clamp(this int p_Value, int p_Min, int p_Max)
    {
        if (p_Value < p_Min)
            p_Value = p_Min;
        else if (p_Value > p_Max) p_Value = p_Max;
        return p_Value;
    }

    public static GameObject? FindGm(string p_Query)
    {
        try {
            GameObject l_LastGM = null;
            string[] l_Gms = p_Query.Split('.');
            bool l_IsFirst = true;
            foreach (string l_Current in l_Gms) {
                if (l_IsFirst) {
                    l_LastGM = GameObject.Find(l_Gms[0]);
                    l_IsFirst = false;
                    continue;
                }

                if (l_LastGM == null) return null;

                GameObject l_CurrentGM = l_LastGM.transform.Find(l_Current).gameObject;
                if (l_CurrentGM == null) return null;
                l_LastGM = l_CurrentGM;
            }

            return l_LastGM;
        }
        catch { return null; }
    }

    /// <summary>
    ///     Get a page from a rank
    /// </summary>
    /// <param name="p_Rank"></param>
    /// <returns></returns>
    public static int CalculatePageByRank(int p_Rank)
    {
        if (p_Rank % GuildSaberModule.SCORES_BY_PAGE != 0) return p_Rank / GuildSaberModule.SCORES_BY_PAGE + 1;

        return p_Rank / GuildSaberModule.SCORES_BY_PAGE;
    }

    public struct ImageResult
    {
        public ImageResult(Texture2D p_Texture, bool p_IsError)
        {
            IsError = p_IsError;
            Texture = p_Texture;
        }

        public bool IsError;
        public Texture2D Texture;
    }

    public static async Task<ImageResult> GetImage(string p_Url, bool p_LogOnError = false)
    {
        var l_NewTexture = new Texture2D(100, 100);

        bool l_IsError = false;

        try {
            using (var l_Client = new System.Net.WebClient()) {
                bool l_MoveNext = false;
                byte[] l_Bytes = await l_Client.DownloadDataTaskAsync(new Uri(p_Url));

                l_NewTexture.LoadImage(l_Bytes, false);
                l_MoveNext = true;

                await WaitUtils.Wait(() => l_MoveNext, 10);
            }
        }
        catch (Exception l_E) {
            l_IsError = true;
            if (p_LogOnError) GSLogger.Instance.Error(l_E, nameof(GuildSaberUtils), nameof(GetImage));
        }

        return new ImageResult(l_NewTexture, l_IsError);
    }

    public static int Diff(int p_Value1, int p_Value2)
    {
        if (p_Value1 < p_Value2)
            return p_Value2 - p_Value1;
        else
            return p_Value1 - p_Value2;
    }

    public static async Task<CP_SDK.Network.WebResponse> GetStringAsync(string p_Url)
    {
        bool l_Finished = false;
        CP_SDK.Network.WebResponse l_Result = null;
        var l_Client = CP_SDK.Network.WebClientCore.GlobalClient;
        l_Client.GetAsync(p_Url, new System.Threading.CancellationToken(), (x) =>
        {
            l_Finished = true;
            l_Result = x;
        });
        await WaitUtils.Wait(() => l_Finished, 1);
        return l_Result;
    }
}
