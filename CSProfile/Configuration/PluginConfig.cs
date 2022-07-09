using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using Newtonsoft.Json;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CSProfile.Configuration;

<<<<<<< Updated upstream
=======
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
>>>>>>> Stashed changes
internal class PluginConfig
{
    public static PluginConfig Instance { get; set; }

<<<<<<< Updated upstream
    [JsonProperty("ShowCard")] public bool ShowCard { get; set; } = true;
    [JsonProperty("CardPos")] public Vector3 CardPosition { get; set; } = new Vector3(0, 3, 5);
    [JsonProperty("CardRot")] public Quaternion CardRotation { get; set; } = Quaternion.Euler(0, 0, 0);
    [JsonProperty("CardHandleVisible")] public bool CardHandleVisible { get; set; } = true;
    [JsonProperty("ShowDetailsLevels")] public bool ShowDetailsLevels { get; set; } = true;
    [JsonProperty("ShowPlayTime")] public bool ShowPlayTime { get; set; } = true;
=======
    [JsonProperty("ShowCard")] public virtual bool ShowCard { get; set; } = true;

    [JsonProperty("CardPos")] public virtual Vector3 CardPosition { get; set; } = new Vector3(2.05f, 2.77f, 3.82f);

    [JsonProperty("CardRot")] public virtual Quaternion CardRotation { get; set; } = Quaternion.Euler(346.4417f, 33.06f, 358.6063f);

    [JsonProperty("CardHandleVisible")] public virtual bool CardHandleVisible { get; set; }

    [JsonProperty("ShowDetailsLevels")] public virtual bool ShowDetailsLevels { get; set; }

    [JsonProperty("ShowPlayTime")] public virtual bool ShowPlayTime { get; set; } = true;
>>>>>>> Stashed changes

    /// <summary>
    ///     This is called whenever BSIPA reads the config from disk (including when file changes are detected).
    /// </summary>
<<<<<<< Updated upstream
    public void OnReload()
=======
    public virtual void OnReload()
>>>>>>> Stashed changes
    {
        // Do stuff after config is read from disk.
    }

    /// <summary>
    ///     Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
    /// </summary>
<<<<<<< Updated upstream
    public void Changed()
=======
    public virtual void Changed()
>>>>>>> Stashed changes
    {
        // Do stuff when the config is changed.
    }

    /// <summary>
    ///     Call this to have BSIPA copy the values from <paramref name="p_Other" /> into this config.
    /// </summary>
<<<<<<< Updated upstream
    public void CopyFrom(PluginConfig p_Other)
=======
    public virtual void CopyFrom(PluginConfig p_Other)
>>>>>>> Stashed changes
    {
        // This instance's members populated from other
    }
}