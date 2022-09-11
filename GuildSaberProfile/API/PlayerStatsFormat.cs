using System.Collections.Generic;

namespace GuildSaberProfile.API;

// ReSharper disable once ClassNeverInstantiated.Global
public class PlayerStatsFormat
{
    public List<PassedLevel> Levels { get; set; }
    public int TotalNumberOfPass { get; set; }

    public float PassPoints { get; set; }
    public float AccPoints { get; set; }
    public List<ApiBadge> Badges { get; set; }
    public bool IsFirstScan { get; set; }
    public bool IsMapLeaderboardBanned { get; set; }
    public bool IsScanBanned { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class PassedLevel
{
    public int LevelID { get; set; }
    public bool Passed { get; set; }
    public int NumberOfPass { get; set; }
    public int TotalNumberOfMaps { get; set; }
    public Trophy Trophy { get; set; }
    public List<CategoryPassed> Categories { get; set; }
}

// ReSharper disable once ClassNeverInstantiated.Global
public class CategoryPassed
{
    public string Category { get; set; }
    public bool Passed { get; set; }
    public int NumberOfPass { get; set; }
    public int TotalNumberOfMaps { get; set; }
    public Trophy Trophy { get; set; }
}