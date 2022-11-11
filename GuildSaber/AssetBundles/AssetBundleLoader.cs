using System.Reflection;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;
using GuildSaber.Logger;

namespace GuildSaber.AssetBundles
{
    class AssetBundleLoader
    {
        /// <summary>
        /// Load Bundle
        /// </summary>
        /// <returns></returns>
        public static AssetBundle LoadBundle()
        {
            try
            {
                AssetBundle l_Bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("GuildSaber.AssetBundles.gsbundle"));
                return l_Bundle;
            } catch (System.Exception l_E)
            {
                GSLogger.Instance.Error(l_E, nameof(AssetBundleLoader), nameof(LoadBundle));
            }
            return null;
        }

        /// <summary>
        /// Load one element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_Name"></param>
        /// <returns></returns>
        public static T LoadElement<T>(string p_Name) where T : Object
        {
            AssetBundle l_GSBundle = LoadBundle();
            if (l_GSBundle == null) { GSLogger.Instance.Error(new System.Exception("GuildSaber Bundle is null"), nameof(AssetBundleLoader), nameof(LoadElement)); return null; }
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
