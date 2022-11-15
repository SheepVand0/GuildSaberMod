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

        protected abstract string m_ViewResourceName { get; }
        public BSMLParserParams m_ParserParams;
        public Action OnPostParse;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Add Item to specific Transform
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
            TItem l_Item = new GameObject($"Parent_{nameof(TItem)}").AddComponent<TItem>();
            l_Item.OnCreate();
            l_Item.Init(p_Init, p_Parent, p_UnderParent, p_NeedParse);
            l_Item.OnPostParse += () =>
            {
                p_Callback?.Invoke(l_Item);
            };
            return l_Item;
        }

        /// <summary>
        /// Private postcreate
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
        /// Create item with predefined params
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
            TItem l_Item = CreateItem<TItem>(p_Parent, p_UnderParent, p_NeedParse, false);
            foreach (ItemParam l_Param in p_Params)
            {
                PropertyInfo p_PropertyInfo = typeof(TItem).GetProperty(l_Param.m_ParamName, BindingFlags.Public | BindingFlags.Instance);
                if (p_PropertyInfo != null && p_PropertyInfo.CanWrite && p_PropertyInfo.PropertyType == l_Param.m_Value.GetType())
                    p_PropertyInfo.SetValue(l_Item, l_Param.m_Value, null);
                else GSLogger.Instance.Error(new Exception($"Property not valid -> Gived Name : {l_Param.m_ParamName}, Type : {l_Param.m_Value.GetType()}, Value : {l_Param.m_Value}"), nameof(CustomUIComponent), nameof(CreateItemWithParams));
            }
            l_Item.Init(true, p_Parent, p_UnderParent, p_NeedParse);
            l_Item.OnPostParse += () =>
            {
                p_Callback?.Invoke(l_Item);
            };
            return l_Item;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        private void PostParse() { OnPostParse?.Invoke(); AfterViewCreation(); }
        protected virtual void OnCreate() { }
        protected virtual void PostCreate() { }
        protected virtual void AfterViewCreation() { }
        protected virtual void OnDestroy() { }
        public virtual void DestroyItem() { }
        public virtual void ResetComponent() { }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Init
        /// </summary>
        /// <param name="p_Enable"></param>
        /// <param name="p_Parent"></param>
        /// <param name="p_UnderParent"></param>
        /// <param name="p_Parse"></param>
        public void Init(bool p_Enable, Transform p_Parent, bool p_UnderParent, bool p_Parse)
        {
            try {
                name = GetType().Name;
                if (p_Parse) m_ParserParams = Parse(p_Parent);
                gameObject.transform.SetParent(p_UnderParent ? p_Parent : p_Parent.parent, false);
                MTCoroutineStarter.Start(_PostCreate(this));
            }
            catch (Exception l_E)
            {
                GSLogger.Instance.Error(l_E, nameof(CustomUIComponent), nameof(Init));
            }
        }

        /// <summary>
        /// Parse current component from m_ViewResourceName
        /// </summary>
        /// <param name="p_Parent"></param>
        /// <returns></returns>
        public BSMLParserParams Parse(Transform p_Parent)
        {
            try
            {
                BSMLParserParams l_ParserParams = BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), m_ViewResourceName), p_Parent.gameObject, this);
                PostParse();
                return l_ParserParams;
            }
            catch (Exception l_Ex)
            {
                GSLogger.Instance.Error(l_Ex, nameof(CustomUIComponent), nameof(Parse));
                Exception l_NewEx = new("Error during parsing bsml : maybe path is invalid ?"); throw l_NewEx;
            }
        }

    }
}
