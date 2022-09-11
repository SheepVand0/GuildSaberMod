using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GuildSaberProfile.Utils.Color;

namespace GuildSaberProfile.API;

// ReSharper disable once ClassNeverInstantiated.Global
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public struct PlayerApiReworkOutput
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public string ProfilePicture { get; set; }
    public Color ProfileColor { get; set; }
    public List<ApiBadge> Badges { get; set; }
    public Trophy Trophy { get; set; }
    public int Level { get; set; }
    public int PassCount { get; set; }
    public bool IsMapLeaderboardBanned { get; set; }
    public bool IsScanBanned { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<RankData> RankData { get; set; }
    // ReSharper disable once CollectionNeverUpdated.Global
    public List<CustomApiPlayerCategory> CategoryData { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public struct RankData
{
    public string PointsType { get; set; }
    public string PointsName { get; set; }
    public float Points { get; set; }
    public int Rank { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class CustomApiPlayerCategory
{
    private string m_Category;
    public string Category
    {
        get => m_Category;
        set => m_Category = string.IsNullOrEmpty(value) ? null : value;
    }
    public int Level { get; set; }
    public int MaxLevel { get; set; }
    public int NumberOfPass { get; set; }
    public int TotalNumberOfMaps { get; set; }
    public Trophy Trophy { get; set; }
}