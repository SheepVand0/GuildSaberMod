using System.Reflection;
using UnityEngine;
using System.IO;

namespace GuildSaberProfile.AssetBundles
{
    class AssetBundleLoader
    {
        public static AssetBundle LoadBundle()
        {
            AssetBundle l_Bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("GuildSaberProfile.AssetBundles.gsbundle"));
            return l_Bundle;
        }

        public static T LoadElement<T>(string p_Name) where T : Object
        {
            AssetBundle l_GSBundle = LoadBundle();
            T l_Object = l_GSBundle.LoadAsset<T>(p_Name);
            l_GSBundle.Unload(false);
            return l_Object ?? null;
        }
    }
}
