using UnityEngine;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Parser;
using System.Reflection;

namespace GuildSaberProfile.UI
{
    class CustomUIComponent : MonoBehaviour
    {
        #region Defaults
        protected virtual string m_ViewResourceName { get; }

        public BSMLParserParams m_ParserParams;
        #endregion

        #region Creation
        public static TItem CreateItem<TItem>(Transform p_Parent, bool p_UnderParent, bool p_NeedParse) where TItem : CustomUIComponent
        {
            TItem l_Item = new GameObject($"Parent_{typeof(TItem).Name}").AddComponent<TItem>();
            l_Item.Init(p_Parent, p_UnderParent, p_NeedParse);
            return l_Item;
        }

        public TComponent CreateSubComponent<TComponent>(bool p_Under) where TComponent : MonoBehaviour
        {
            TComponent l_Comp = gameObject.AddComponent<TComponent>();
            l_Comp.name = $"Sub_{l_Comp.name}_{typeof(TComponent).Name}";
            l_Comp.transform.SetParent(p_Under ? transform : transform.parent, false);
            if (l_Comp.GetType() == typeof(CustomUIComponent)) (l_Comp as CustomUIComponent).OnCreate();
            return l_Comp;
        }
        #endregion

        #region Events
        public virtual void OnCreate()
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
            if (p_Parse)
            {
                m_ParserParams = BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), m_ViewResourceName), p_Parent.gameObject, this);
                if (m_ParserParams == null) DestroyImmediate(this);
            }
            OnCreate();
        }
        #endregion
    }
}
