using CP_SDK.XUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.Defaults
{
    internal class GSHorizontalList<t_DataType, t_UiElemType> : XUIHLayout where t_UiElemType : IXUIElement
    {
        protected GSHorizontalList(params IXUIElement[] p_Childs) : base("GSHorizontalList", p_Childs)
        {
        }

        public static GSHorizontalList<t_Data, t_UiType> Make<t_Data, t_UiType>
            (List<t_Data> p_Elems, Action<t_UiType, bool> p_SetActiveMethod, Action<t_Data, t_UiType> p_OnNeedToUpdateCell,
            Func<Transform, t_UiType> p_UiElemCreateMethod) where t_UiType : IXUIElement
        {
            var l_New = new GSHorizontalList<t_Data, t_UiType>();
            l_New.m_SetActiveFunction = p_SetActiveMethod;
            l_New.m_OnNeedToUpdateCell = p_OnNeedToUpdateCell;
            l_New.m_UiElemCreateMethod = p_UiElemCreateMethod;
            l_New.m_Elements = p_Elems;
            return l_New;
        }

        //////////////////////////////////////////////////////
        /////////////////////////////////////////////////////

        protected Action<t_UiElemType, bool> m_SetActiveFunction;
        protected Action<t_DataType, t_UiElemType> m_OnNeedToUpdateCell;
        protected Func<Transform, t_UiElemType> m_UiElemCreateMethod;
        protected List<t_DataType> m_Elements;
        protected List<t_UiElemType> m_VisualElements;

        protected int m_DisplayedElementsCount = 5;
        protected int m_Page = 0;

        //////////////////////////////////////////////////////
        /////////////////////////////////////////////////////

        public GSHorizontalList<t_DataType, t_UiElemType> SetElems(List<t_DataType> p_Elems)
        {
            m_Elements = p_Elems;
            OnReady(x =>
            {
                UpdateElements();
            });

            return this;
        }

        protected void UpdateElements()
        {
            for (int l_i = m_Page; l_i < m_Elements.Count();l_i++)
            {
                if (l_i - m_Page > m_DisplayedElementsCount) break;

                var l_Item = m_Elements[l_i];
                int l_VisualElementIndex = l_i - m_Page;
                if (l_VisualElementIndex > m_VisualElements.Count() - 1)
                    m_VisualElements.Add(m_UiElemCreateMethod.Invoke(Element.transform));

                m_OnNeedToUpdateCell.Invoke(m_Elements[l_i], m_VisualElements[l_i - m_Page]);
            }
        }
    }
}
