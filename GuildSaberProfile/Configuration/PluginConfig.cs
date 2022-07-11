using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using Newtonsoft.Json;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace GuildSaberProfile.Configuration;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
internal class PluginConfig
{
    public static PluginConfig Instance { get; set; }

    public static readonly Vector3 DefaultCardPosition = new Vector3(0.0f, 0.02f, 1.0f);
    public static readonly Quaternion DefaultCardRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);

    [JsonProperty("ShowCardInMenu")] public virtual bool ShowCardInMenu { get; set; } = true;
    [JsonProperty("ShowCardInGame")] public virtual bool ShowCardInGame { get; set; } = false;
    [JsonProperty("CardPos")] public virtual Vector3 CardPosition { get; set; } = DefaultCardPosition;
    [JsonProperty("CardRot")] public virtual Quaternion CardRotation { get; set; } = DefaultCardRotation;

    [JsonProperty("CardHandleVisible")] public virtual bool CardHandleVisible { get; set; }

    [JsonProperty("ShowDetailsLevels")] public virtual bool ShowDetailsLevels { get; set; } = true;

    [JsonProperty("ShowPlayTime")] public virtual bool ShowPlayTime { get; set; } = true;
    [JsonProperty("SelectedGuild")] public virtual string SelectedGuild { get; set; } = "CS";

    /// <summary>
    ///     This is called whenever BSIPA reads the config from disk (including when file changes are detected).
    /// </summary>
    public virtual void OnReload()
    {
        // Do stuff after config is read from disk.
    }

    /// <summary>
    ///     Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
    /// </summary>
    public virtual void Changed()
    {
        // Do stuff when the config is changed.
    }

    /// <summary>
    ///     Call this to have BSIPA copy the values from <paramref name="p_Other" /> into this config.
    /// </summary>
    public virtual void CopyFrom(PluginConfig p_Other)
    {
        // This instance's members populated from other
    }
}
