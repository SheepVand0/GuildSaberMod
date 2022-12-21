using CP_SDK;
using CP_SDK.Config;
using Newtonsoft.Json;
using UnityEngine;

namespace GuildSaber.Configuration;

internal class GSConfig : JsonConfig<GSConfig>
{
    internal static class ConfigDefaults
    {
        internal static readonly Vector3    s_DefaultCardPosition       = new(0.0f, 0.02f, 1.0f);
        internal static readonly Vector3    s_DefaultCardRotation       = new(90.0f, 0.0f, 0.0f);
        internal static readonly Vector3    s_DefaultInGameCardPosition = new(1.8f, 1, -1.5f);
        internal static readonly Vector3    s_DefaultInGameCardRotation = new(20, 135, 0);
    }

    [JsonProperty] internal bool Enabled = true;
    [JsonProperty] internal bool CardEnabled = true;
    [JsonProperty] internal bool LeaderboardEnabled = true;

    [JsonProperty] internal bool ShowCardInMenu = true;
    [JsonProperty] internal bool ShowCardInGame = true;

    [JsonProperty] internal Vector3    CardPosition       = ConfigDefaults.s_DefaultCardPosition;
    [JsonProperty] internal Vector3    CardRotation       = ConfigDefaults.s_DefaultCardRotation;
    [JsonProperty] internal Vector3    InGameCardPosition = ConfigDefaults.s_DefaultInGameCardPosition;
    [JsonProperty] internal Vector3    InGameCardRotation = ConfigDefaults.s_DefaultInGameCardRotation;

    [JsonProperty] internal bool ShowSettingsModal = true;
    [JsonProperty] internal bool CardHandleVisible = false;
    [JsonProperty] internal bool ShowDetailsLevels = true;
    [JsonProperty] internal bool ShowPlayTime      = true;

    [JsonProperty] internal int SelectedGuild = 0;

    [JsonProperty] internal bool UseCustomColor = false;
    [JsonProperty] internal bool UseCustomColorGradient = false;
    [JsonProperty] internal Color CustomColor = new(0.7f, 0.7f, 0);
    [JsonProperty] internal Color CustomColor1 = new(0.7f, 0.2f, 0);
    [JsonProperty] internal bool InvertGradient = false;
    [JsonProperty] internal float GradientColor1Multiplier = 1.0f;

    [JsonProperty] internal bool UseCustomPointsColor = false;
    [JsonProperty] internal Color CustomPointsColor = Color.red;

    [JsonProperty] internal bool UseCustomNameGradientColor = false;
    [JsonProperty] internal Color CustomNameGradientColor = Color.red;
    [JsonProperty] internal float NameGradientColor0Multiplier = 1f;

    [JsonProperty] internal bool EnableCheeseDetection = false;

    [JsonProperty] internal bool  UwUMode     = false;

    public override string GetRelativePath()
        => $"{ChatPlexSDK.ProductName}/GuildSaber/Config";

    protected override void OnInit(bool p_OnCreation)
    {
        Save();
    }


}
