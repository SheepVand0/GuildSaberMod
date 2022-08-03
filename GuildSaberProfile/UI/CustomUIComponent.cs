using UnityEngine;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using System.Reflection;

namespace GuildSaberProfile.UI
{
    public abstract class CustomUIComponent : MonoBehaviour
    {
        #region Defaults
        protected abstract string m_ViewResourceName { get; }

        public BSMLParserParams m_ParserParams;
        #endregion

        #region Creation
        public static TItem CreateItem<TItem>(Transform p_Parent, bool p_UnderParent, bool p_NeedParse) where TItem : CustomUIComponent
        {
            Plugin.Log.Info("Creating avatar");
            TItem l_Item = new GameObject($"Parent_{nameof(TItem)}").AddComponent<TItem>();
            l_Item.OnCreate();
            l_Item.Init(p_Parent, p_UnderParent, p_NeedParse);
            return l_Item;
        }

        public TComponent CreateSubComponent<TComponent>(bool p_Under, bool p_NeedParse = false) where TComponent : MonoBehaviour
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
            return l_Comp;
        }
        #endregion

        #region Events
        public abstract void OnCreate();

        public virtual void PostCreate()
        {

        }

        public virtual void OnDestroy()
        {

        }
        #endregion

        #region Init
        public void Init(Transform p_Parent, bool p_UnderParent, bool p_Parse)
        {
            name = GetType().Name;
            gameObject.transform.SetParent(p_UnderParent ? p_Parent : p_Parent.parent, false);
            if (p_Parse) m_ParserParams = Parse(p_Parent);
            PostCreate();
        }

        public BSMLParserParams Parse(Transform p_Parent)
        {
            BSMLParserParams l_ParserParams = BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), m_ViewResourceName), p_Parent.gameObject, this);
            if (l_ParserParams == null) { Plugin.Log.Info("Destroying"); DestroyImmediate(this); }
            return l_ParserParams;
        }
        #endregion
    }
}
