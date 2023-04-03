using System;
using System.Collections.Generic;
using GuildSaber.Utils;

namespace GuildSaber.API;
#nullable enable
public struct ApiMapLeaderboardCollectionStruct
{
    //TODO: Make some adjust inside ContentStruct, with a new struct for only the player info + add the GuildRankedDifficultyID
    public List<ApiMapLeaderboardContentStruct>? Leaderboards { get; set; }
    public ApiPageMetadataStruct Metadata { get; set; }
    public ApiCustomDataStruct CustomData { get; set; }
    public ApiMapLeaderboardContentStruct? PlayerScore { get; set; }
}

public struct PointsData
{
    public string PointsType { get; set; }
    public string PointsName { get; set; }
    public byte PointsIndex { get; set; }
    public float Points { get; set; }
}

public struct ApiMapLeaderboardContentStruct
{
    public UInt32 ID { get; set; }
    public string? ScoreSaberID { get; set; }
    public string? BeatLeaderID { get; set; }
    public UInt32 Rank { get; set; }
    public string Name { get; set; }
    public string? Country { get; set; }
    public string Avatar { get; set; }
    public UInt32 State { get; set; }
    public List<PointsData> PointsData;
    /// <summary>
    ///     The weight is used to display how much points it actually really gave to the player (=> Earned Points = Points * Weight) (not used yet on BSDR, but it's something used on ScoreSaber)
    /// </summary>
    public float Weight { get; set; }
    public UInt32 BaseScore { get; set; }
    public UInt32 ModifiedScore { get; set; }
    public string Modifiers { get; set; }
    public float Multiplier { get; set; }
    public UInt32 BadCuts { get; set; }
    public UInt32 MissedNotes { get; set; }
    public UInt32 MaxCombo { get; set; }
    public bool FullCombo { get; set; }
    public UInt32 HMD { get; set; }
    public string UnixTimeSet { get; set; }
    public bool HasReplay { get; set; }
    public string? ReplayURL { get; set; }
    public bool HasScoreStatistic { get; set; }
    public ApiScoreStatistic? ScoreStatistic { get; set; }
}

public struct ApiScoreStatistic
{
    public float LeftPreSwing { get; set; }
    public float LeftPureAcc { get; set; }
    public float LeftPostSwing { get; set; }
    public float RightPreSwing { get; set; }
    public float RightPureAcc { get; set; }
    public float RightPostSwing { get; set; }
    public UInt32 PauseCount { get; set; }
}

public struct ApiPageMetadataStruct
{
    public UInt32 Page { get; set; }
    public UInt32 MaxPage { get; set; }
    public UInt32 CountPerPage { get; set; }
    public UInt32 TotalCount { get; set; }
}

public struct ApiCustomDataStruct
{
    public UInt32 RankedDifficultyID { get; set; }
    public UInt32 LevelID { get; set; }
    public string LevelName { get; set; }
    public float LevelValue { get; set; }

    public GuildSaber.Utils.DiscordColor.Color LevelColor { get; set; }

    public UInt32? CategoryID { get; set; }
    public string? CategoryName { get; set; }
    public float PassPoints { get; set; }
    public UInt32 MaxScore { get; set; }
}

public enum EHMD
{
    Unk = 0,
    CV1 = 1 << 0,
    Vive = 1 << 1,
    VivePro = 1 << 2,
    MixedReality = 1 << 3,
    RiftS = 1 << 4,
    Quest1 = 1 << 5,
    ValveIndex = 1 << 6,
    ViveCosmos = 1 << 7,
    Quest2 = 1 << 8,
    PicoNeo3 = 33,
    PicoNeo2 = 34,
    VivePro2 = 35,
    ViveElite = 36,
    Miramar = 37,
    Pimax8K = 38,
    Pimax5K = 39,
    PimaxArtisan = 40,
    HpReverb = 41,
    SamsungWmr = 42,
    QiyuDream = 43,
    Disco = 44,
    LenovoExplorer = 45,
    AcerWmr = 46,
    ViveFocus = 47,
    Arpara = 48,
    DellVisor = 49,
    E3 = 50,
    ViveDvt = 51,
    Glasses20 = 52,
    Hedy = 53,
    Vaporeon = 54,
    Huaweivr = 55,
    AsusWmr = 56,
    Cloudxr = 57,
    VRidge = 58,
    Medion = 59,
    PicoNeo4 = 60,
    QuestPro = 61
}