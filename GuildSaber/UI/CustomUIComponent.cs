using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using CP_SDK.Unity;
using GuildSaber.Logger;
using UnityEngine;

namespace GuildSaber.UI
{
    public struct ItemParam
    {
        public ItemParam(string p_Param, object p_Value)
        {
            m_ParamName = p_Param;
            m_Value = p_Value;
        }
        public string m_ParamName { get; set; }
        public object m_Value { get; set; }
    }


    public abstract class CustomUIComponent : MonoBehaviour
    {
        public BSMLParserParams m_ParserParams;
        public Action OnPostParse;

        protected abstract string ViewResourceName { get; }
        protected virtual void OnDestroy()
        {
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Add Item to specific Transform
        /// </summary>
        /// <typeparam name="TItem">Item type</typeparam>
        /// <param name="p_Parent"></param>
        /// <param name="p_UnderParent"></param>
        /// <param name="p_NeedParse"></param>
        /// <param name="p_Init"></param>
        /// <param name="p_Callback"></param>
        /// <returns></returns>
        public static TItem CreateItem<TItem>(Transform p_Parent, bool p_UnderParent, bool p_NeedParse, bool p_Init = true, Action<TItem> p_Callback = null) where TItem : CustomUIComponent
        {
            var l_Item = new GameObject($"Parent_{nameof(TItem)}").AddComponent<TItem>();
            l_Item.OnCreate();
            l_Item.Init<TItem>(p_Init, p_Parent, p_UnderParent, p_NeedParse);
            l_Item.OnPostParse += () => { p_Callback?.Invoke(l_Item); };
            return l_Item;
        }

        /// <summary>
        ///     Private postcreate
        /// </summary>
        /// <param name="p_Item"></param>
        /// <returns></returns>
        private static IEnumerator _PostCreate(CustomUIComponent p_Item)
        {
            yield return new WaitForSeconds(0.1f);

            p_Item.PostCreate();

            yield return null;
        }

        /// <summary>
        ///     Create item with predefined params
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="p_Parent"></param>
        /// <param name="p_UnderParent"></param>
        /// <param name="p_NeedParse"></param>
        /// <param name="p_Params"></param>
        /// <param name="p_Callback"></param>
        /// <returns></returns>
        public static TItem CreateItemWithParams<TItem>(Transform p_Parent, bool p_UnderParent, bool p_NeedParse, List<ItemParam> p_Params, Action<TItem> p_Callback = null) where TItem : CustomUIComponent
        {
            var l_Item = CreateItem<TItem>(p_Parent, p_UnderParent, p_NeedParse, false);
            foreach (ItemParam l_Param in p_Params)
            {
                PropertyInfo l_PropertyInfo = typeof(TItem).GetProperty(l_Param.m_ParamName, BindingFlags.Public | BindingFlags.Instance) ?? null;
                if (l_PropertyInfo != null && l_PropertyInfo.CanWrite && l_PropertyInfo.PropertyType == l_Param.m_Value.GetType())
                {
                    l_PropertyInfo.SetValue(l_Item, l_Param.m_Value, null);
                }
                else
                {
                    GSLogger.Instance.Error(new Exception($"Property not valid -> Given Name : {l_Param.m_ParamName}, Type : {l_Param.m_Value.GetType()}, Value : {l_Param.m_Value}"), nameof(CustomUIComponent), nameof(CreateItemWithParams));
                }
            }
            l_Item.Init<TItem>(true, p_Parent, p_UnderParent, p_NeedParse);
            l_Item.OnPostParse += () => { p_Callback?.Invoke(l_Item); };
            return l_Item;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private void PostParse()
        {
            OnPostParse?.Invoke();
            AfterViewCreation();
        }
        protected virtual void OnCreate()
        {
        }
        protected virtual void PostCreate()
        {
        }
        protected virtual void AfterViewCreation()
        {
        }
        public virtual void DestroyItem()
        {
        }
        public virtual void ResetComponent()
        {
        }
        protected virtual string GetViewDescription()
        {
            return string.Empty;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///     Init
        /// </summary>
        /// <param name="p_Enable"></param>
        /// <param name="p_Parent"></param>
        /// <param name="p_UnderParent"></param>
        /// <param name="p_Parse"></param>
        public void Init<T>(bool p_Enable, Transform p_Parent, bool p_UnderParent, bool p_Parse) where T : CustomUIComponent
        {
            try
            {
                name = $"{typeof(T)}_{Resources.FindObjectsOfTypeAll<T>().Length}";
                if (p_Parse)
                {
                    m_ParserParams = Parse(p_Parent);
                }
                gameObject.transform.SetParent(p_UnderParent ? p_Parent : p_Parent.parent, false);
                MTCoroutineStarter.Start(_PostCreate(this));
            }
            catch (Exception l_E)
            {
                GSLogger.Instance.Error(l_E, nameof(CustomUIComponent), nameof(Init));
                GSLogger.Instance.Error(new Exception($"Component name = {name}"), nameof(CustomUIComponent), nameof(Init));
            }
        }

        /// <summary>
        ///     Parse current component from m_ViewResourceName
        /// </summary>
        /// <param name="p_Parent"></param>
        /// <returns></returns>
        public BSMLParserParams Parse(Transform p_Parent)
        {
            try
            {
                string l_Resource = ViewResourceName;
                if (l_Resource == string.Empty)
                {
                    l_Resource = GetViewDescription();
                }
                if (l_Resource == string.Empty)
                {
                    throw new Exception($"No Valid bsml was found for the ui component {name}");
                }

                BSMLParserParams l_ParserParams = BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), l_Resource), p_Parent.gameObject, this);
                PostParse();
                return l_ParserParams;
            }
            catch (Exception l_Ex)
            {
                GSLogger.Instance.Error(l_Ex, nameof(CustomUIComponent), nameof(Parse));
                Exception l_NewEx = new Exception($"Error during parsing bsml of component {name} : maybe path is invalid ?");
                throw l_NewEx;
            }
        }
    }
}