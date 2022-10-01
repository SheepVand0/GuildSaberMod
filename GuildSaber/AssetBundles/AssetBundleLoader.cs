using System.Reflection;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;

namespace GuildSaber.AssetBundles
{
    class AssetBundleLoader
    {

        public static AssetBundle LoadBundle()
        {
            try
            {
                AssetBundle l_Bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("GuildSaber.AssetBundles.gsbundle"));
                return l_Bundle;
            } catch (System.Exception l_E)
            {
                Plugin.Log.Error(l_E);
            }
            return null;
        }

        public static T LoadElement<T>(string p_Name) where T : Object
        {
            AssetBundle l_GSBundle = LoadBundle();
            if (l_GSBundle == null) { Plugin.Log.Info("GuildSaber Bundle is null"); return null; }
            T[] l_Objects = l_GSBundle.LoadAllAssets<T>();
            T l_Object = null;
            foreach (var l_Current in l_Objects)
            {
                if (l_Current.name != p_Name) continue;
                l_Object = l_Current;
            }
            l_GSBundle.Unload(false);
            return l_Object;
        }
    }
}
