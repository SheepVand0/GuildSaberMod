using System;
using System.Reflection;
using GuildSaber.Logger;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GuildSaber.AssetBundles
{
    internal class AssetBundleLoader
    {
        /// <summary>
        ///     Load Bundle
        /// </summary>
        /// <returns></returns>
        public static AssetBundle LoadBundle()
        {
            try
            {
                AssetBundle l_Bundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("GuildSaber.AssetBundles.gsbundle"));
                return l_Bundle;
            }
            catch (Exception l_E)
            {
                GSLogger.Instance.Error(l_E, nameof(AssetBundleLoader), nameof(LoadBundle));
            }
            return null;
        }

        /// <summary>
        ///     Load one element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_Name"></param>
        /// <returns></returns>
        public static T LoadElement<T>(string p_Name) where T : Object
        {
            AssetBundle l_GSBundle = LoadBundle();
            if (l_GSBundle == null)
            {
                GSLogger.Instance.Error(new Exception("GuildSaber Bundle is null"), nameof(AssetBundleLoader), nameof(LoadElement));
                return null;
            }
            T[] l_Objects = l_GSBundle.LoadAllAssets<T>();
            T l_Object = null;
            foreach (T l_Current in l_Objects)
            {
                if (l_Current.name != p_Name)
                {
                    continue;
                }
                l_Object = l_Current;
            }
            l_GSBundle.Unload(false);
            return l_Object;
        }
    }
}