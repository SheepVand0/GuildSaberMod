using UnityEngine;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.UI.Card;
using UnityEngine.UI;

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
        #region Defaults
        protected abstract string m_ViewResourceName { get; }
        public BSMLParserParams m_ParserParams;
        public Action OnPostParse;
        #endregion

        #region Creation
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

        public static TItem CreateItemWithParams<TItem>(Transform p_Parent, bool p_UnderParent, bool p_NeedParse, List<ItemParam> p_Params, Action<TItem> p_Callback = null) where TItem : CustomUIComponent
        {
            TItem l_Item = CreateItem<TItem>(p_Parent, p_UnderParent, p_NeedParse, false);
            foreach (ItemParam l_Param in p_Params)
            {
                PropertyInfo p_PropertyInfo = typeof(TItem).GetProperty(l_Param.m_ParamName, BindingFlags.Public | BindingFlags.Instance);
                if (p_PropertyInfo != null && p_PropertyInfo.CanWrite && p_PropertyInfo.PropertyType == l_Param.m_Value.GetType())
                    p_PropertyInfo.SetValue(l_Item, l_Param.m_Value, null);
                else Plugin.Log.Error($"Property not valid -> Gived Name : {l_Param.m_ParamName}, Type : {l_Param.m_Value.GetType()}, Value : {l_Param.m_Value}");
            }
            l_Item.Init(true, p_Parent, p_UnderParent, p_NeedParse);
            l_Item.OnPostParse += () =>
            {
                p_Callback?.Invoke(l_Item);
            };
            return l_Item;
        }
        public TComponent CreateSubComponent<TComponent>(bool p_Under, bool p_NeedParse = false, Action<TComponent> p_Callback = null) where TComponent : MonoBehaviour
        {
            TComponent l_Comp = gameObject.AddComponent<TComponent>();
            l_Comp.name = $"Sub_{l_Comp.name}_{nameof(TComponent)}";
            l_Comp.transform.SetParent(p_Under ? transform : transform.parent, false);
            if (l_Comp.GetType() == typeof(CustomUIComponent))
            {
                CustomUIComponent l_CustomComp = l_Comp as CustomUIComponent;
                l_CustomComp.OnCreate();
                if (p_NeedParse) l_CustomComp.Parse(l_Comp.transform.parent);
            }
            p_Callback?.Invoke(l_Comp);
            return l_Comp;
        }
        public static CustomUIComponent RecreateUIComponent<TType>(CustomUIComponent p_Component, bool p_UnderParent, bool p_NeedParse, Action<TType> p_Callback = null) where TType : CustomUIComponent
        {
            Transform l_CurrentTransform = p_Component.transform.parent;
            DestroyImmediate(p_Component.gameObject);
            p_Component = CreateItem<TType>(l_CurrentTransform, p_UnderParent, p_NeedParse);
            p_Callback?.Invoke((TType)p_Component);
            return p_Component;
        }
        #endregion

        #region Destroy
        public void DestroyItem()
        {
            GameObject.Destroy(gameObject);
        }
        #endregion

        #region Events
        protected virtual void OnCreate() { }
        protected virtual void PostCreate() { }
        protected virtual void OnDestroy() { }
        private void PostParse() { OnPostParse?.Invoke(); AfterViewCreation();   }
        protected virtual void AfterViewCreation() { }
        #endregion

        #region Init
        public void Init(bool p_Enable, Transform p_Parent, bool p_UnderParent, bool p_Parse)
        {
            try {
                //if (p_Enable == false) { PostParse(); return; }

                name = GetType().Name;
                if (p_Parse) m_ParserParams = Parse(p_Parent);
                gameObject.transform.SetParent(p_UnderParent ? p_Parent : p_Parent.parent, false);
                PostCreate();
            }
            catch (Exception l_E)
            {
                Plugin.Log.Error(l_E);
            }
        }

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
                Plugin.Log.Error($"Error during parsing, here the Complete error : {l_Ex.StackTrace}");
                Plugin.Log.Error($"Host name : {name}");
                Exception l_NewEx = new("Error during parsing bsml : maybe path is invalid ?"); throw l_NewEx;
            }
        }
        #endregion
    }
}
