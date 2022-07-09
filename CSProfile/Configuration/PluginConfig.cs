
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using UnityEngine;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CSProfile.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }

        [JsonProperty("ShowCard")] public virtual bool ShowCard { get; set; } = true;
        [JsonProperty("CardPos")] public virtual Vector3 CardPosition { get; set; } = new Vector3(2.05f, 2.77f, 3.82f);
        [JsonProperty("CardRot")] public virtual Quaternion CardRotation { get; set; } = Quaternion.Euler(346.4417f, 33.06f, 358.6063f);
        [JsonProperty("CardHandleVisible")] public virtual bool CardHandleVisible { get; set; } = false;
        [JsonProperty("ShowDetailsLevels")] public virtual bool ShowDetailsLevels { get; set; } = false;
        [JsonProperty("ShowPlayTime")] public virtual bool ShowPlayTime { set; get; } = true;
    }
}
