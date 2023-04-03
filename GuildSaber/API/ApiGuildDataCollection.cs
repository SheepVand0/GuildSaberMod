using System.Collections.Generic;
using GuildSaber.Utils;

namespace GuildSaber.API;

public struct GuildData
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string? SmallName { get; set; }
    public string Logo { get; set; }
    public string Banner { get; set; }
    public string Description { get; set; }
    public GuildSaber.Utils.DiscordColor.Color Color { get; set; }
    public GuildSaber.Utils.DiscordColor.Color RankingTeamColor { get; set; }
    public GuildSaber.Utils.DiscordColor.Color ScoringTeamColor { get; set; }
    public GuildSaber.Utils.DiscordColor.Color ManagerTeamColor { get; set; }
    public int MemberCount { get; set; }
    public int RankedDiffCount { get; set; }
    public int PlaylistCount { get; set; }
    public int CategoryCount { get; set; }
    public int PatreonBenefits { get; set; }
}

public struct GuildConfig
{
    public int ID { get; set; }
    public int MainDiscordServerID { get; set; }
    public bool Pass_Enabled { get; set; }
    public bool Acc_Enabled { get; set; }
    public bool Pure_Enabled { get; set; }
    public string RolePrefix { get; set; }
    public bool GiveOldRoles { get; set; }
    public bool EnableCategoryLeveling { get; set; }
    public int MaximumNumberOfMapInGetInfo { get; set; }
    public bool DisplayCustomPassTextInGetInfo { get; set; }
    public bool DisplayCategoryEdit { get; set; }
    public bool DisplaySubCategoryEdit { get; set; }
    public bool DisplayCustomPassTextEdit { get; set; }
    public string Pass_PointName { get; set; }
    public string Acc_PointName { get; set; }
    public string Pure_PointName { get; set; }
    public float Pass_PointMultiplier { get; set; }
    public float Acc_PointMultiplier { get; set; }
    public float Pure_PointMultiplier { get; set; }
    public bool EnableAutomaticWeightCalculation { get; set; }
    public int MinimumNumberOfScoreForAutoWeight { get; set; }
    public bool Pass_AllowAutoWeight { get; set; }
    public bool Acc_AllowAutoWeight { get; set; }
    public bool Pure_AllowAutoWeight { get; set; }
    public bool Pass_AllowForceManualWeight { get; set; }
    public bool Acc_AllowForceManualWeight { get; set; }
    public bool Pure_AllowForceManualWeight { get; set; }
    public bool Pass_OnlyAutoWeight { get; set; }
    public bool Acc_OnlyAutoWeight { get; set; }
    public bool Pure_OnlyAutoWeight { get; set; }
    public bool GGP_ShowAccPoint { get; set; }
}

public struct GuildLightData
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Logo { get; set; }
    public string Banner { get; set; }
    public string Description { get; set; }
    public GuildSaber.Utils.DiscordColor.Color Color { get; set; }
    public int MemberCount { get; set; }
    public int RankedDiffCount { get; set; }
    public int PlaylistCount { get; set; }
    public int CategoryCount { get; set; }
    public int PatreonBenefits { get; set; }
}

public struct GuildMetadata
{
    public int Count { get; set; }
}

public struct GuildLightDataCollection
{
    public List<GuildLightData> Guilds { get; set; }
    public GuildMetadata Metadata { get; set; }
}

public struct ApiGuildCollection
{
    public List<GuildData> Guilds { get; set; }
    public GuildMetadata Metadata { get; set; }
}

public struct ApiGuildDataCollection
{
    public GuildData Guild { get; set; }
    public GuildConfig GuildConfig { get; set; }
}