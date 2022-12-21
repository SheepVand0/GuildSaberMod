using CP_SDK;
using CP_SDK.Config;
using Newtonsoft.Json;
using UnityEngine;

namespace GuildSaber.Configuration;

internal class GSConfig : JsonConfig<GSConfig>
{
    [JsonProperty] internal bool CardEnabled = true;
    [JsonProperty] internal bool CardHandleVisible;

    [JsonProperty] internal Vector3 CardPosition = ConfigDefaults.s_DefaultCardPosition;
    [JsonProperty] internal Vector3 CardRotation = ConfigDefaults.s_DefaultCardRotation;
    [JsonProperty] internal Color CustomColor = new Color(0.7f, 0.7f, 0);
    [JsonProperty] internal Color CustomColor1 = new Color(0.7f, 0.2f, 0);
    [JsonProperty] internal Color CustomNameGradientColor = Color.red;
    [JsonProperty] internal Color CustomPointsColor = Color.red;

    [JsonProperty] internal bool EnableCheeseDetection;

    [JsonProperty] internal bool Enabled = true;
    [JsonProperty] internal float GradientColor1Multiplier = 1.0f;
    [JsonProperty] internal Vector3 InGameCardPosition = ConfigDefaults.s_DefaultInGameCardPosition;
    [JsonProperty] internal Vector3 InGameCardRotation = ConfigDefaults.s_DefaultInGameCardRotation;
    [JsonProperty] internal bool InvertGradient;
    [JsonProperty] internal bool LeaderboardEnabled = true;
    [JsonProperty] internal float NameGradientColor0Multiplier = 1f;

    [JsonProperty] internal int SelectedGuild;
    [JsonProperty] internal bool ShowCardInGame = true;

    [JsonProperty] internal bool ShowCardInMenu = true;
    [JsonProperty] internal bool ShowDetailsLevels = true;
    [JsonProperty] internal bool ShowPlayTime = true;

    [JsonProperty] internal bool ShowSettingsModal = true;

    [JsonProperty] internal bool UseCustomColor;
    [JsonProperty] internal bool UseCustomColorGradient;

    [JsonProperty] internal bool UseCustomNameGradientColor;

    [JsonProperty] internal bool UseCustomPointsColor;

    [JsonProperty] internal bool UwUMode;

    public override string GetRelativePath() { return $"{ChatPlexSDK.ProductName}/GuildSaber/Config"; }

    protected override void OnInit(bool p_OnCreation) { Save(); }

    internal static class ConfigDefaults
    {
        internal static readonly Vector3 s_DefaultCardPosition = new Vector3(0.0f, 0.02f, 1.0f);
        internal static readonly Vector3 s_DefaultCardRotation = new Vector3(90.0f, 0.0f, 0.0f);
        internal static readonly Vector3 s_DefaultInGameCardPosition = new Vector3(1.8f, 1, -1.5f);
        internal static readonly Vector3 s_DefaultInGameCardRotation = new Vector3(20, 135, 0);
    }
}