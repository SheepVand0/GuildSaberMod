using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using Newtonsoft.Json;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CSProfile.Configuration;

internal class PluginConfig
{
    public static PluginConfig Instance { get; set; }

    [JsonProperty("ShowCard")] public bool ShowCard { get; set; } = true;
    [JsonProperty("CardPos")] public Vector3 CardPosition { get; set; } = new Vector3(0, 3, 5);
    [JsonProperty("CardRot")] public Quaternion CardRotation { get; set; } = Quaternion.Euler(0, 0, 0);
    [JsonProperty("CardHandleVisible")] public bool CardHandleVisible { get; set; } = true;
    [JsonProperty("ShowDetailsLevels")] public bool ShowDetailsLevels { get; set; } = true;
    [JsonProperty("ShowPlayTime")] public bool ShowPlayTime { get; set; } = true;

    /// <summary>
    ///     This is called whenever BSIPA reads the config from disk (including when file changes are detected).
    /// </summary>
    public void OnReload()
    {
        // Do stuff after config is read from disk.
    }

    /// <summary>
    ///     Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
    /// </summary>
    public void Changed()
    {
        // Do stuff when the config is changed.
    }

    /// <summary>
    ///     Call this to have BSIPA copy the values from <paramref name="p_Other" /> into this config.
    /// </summary>
    public void CopyFrom(PluginConfig p_Other)
    {
        // This instance's members populated from other
    }
}
