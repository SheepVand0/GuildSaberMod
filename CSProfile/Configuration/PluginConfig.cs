
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

        [JsonProperty("showCard")]
        public virtual bool ShowCard { get; set; } = true;
        [JsonProperty("cardPos")]
        public virtual Vector3 CardPosition { get; set; } = new Vector3(2.05f,2.77f,3.82f);
        [JsonProperty("cardRot")]
        public virtual Quaternion CardRotation { get; set; } = Quaternion.Euler(346.4417f,33.06f,358.6063f);
        [JsonProperty("cardHandleVisible")]
        public virtual bool CardHandleVisible { get; set; } = false;
        [JsonProperty("showDetailsLevels")]
        public virtual bool ShowDetaislLevels { get; set; } = false;
        [JsonProperty("ShowPlayTime")]
        public virtual bool ShowPlayTime { get; set; } = true;

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }
    }
}
