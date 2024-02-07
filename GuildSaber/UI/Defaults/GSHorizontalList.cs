using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.UI.CustomLevelSelectionMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GuildSaber.UI.Defaults
{
    internal class GSHorizontalList<t_DataType, t_UiElemType> : XUIHLayout where t_UiElemType : IXUIElement
    {
        protected GSHorizontalList(params IXUIElement[] p_Childs) : base("GSHorizontalList", p_Childs)
        {
            OnReady(OnCreation);
        }

        public static GSHorizontalList<t_Data, t_UiType> Make<t_Data, t_UiType>
            (List<t_Data> p_Elems, Action<t_UiType, bool> p_SetActiveMethod, Action<t_Data, t_UiType> p_OnNeedToUpdateCell,
            Func<Transform, t_UiType> p_UiElemCreateMethod, int p_Width, int p_Height) where t_UiType : IXUIElement
        {
            var l_New = new GSHorizontalList<t_Data, t_UiType>();
            l_New.m_SetActiveFunction = p_SetActiveMethod;
            l_New.m_OnNeedToUpdateCell = p_OnNeedToUpdateCell;
            l_New.m_UiElemCreateMethod = p_UiElemCreateMethod;
            l_New.m_Elements = p_Elems;
            l_New.m_Width = p_Width;
            l_New.m_Height = p_Height;
            return l_New;
        }

        //////////////////////////////////////////////////////
        /////////////////////////////////////////////////////

        protected Action<t_UiElemType, bool> m_SetActiveFunction;
        protected Action<t_DataType, t_UiElemType> m_OnNeedToUpdateCell;
        protected Func<Transform, t_UiElemType> m_UiElemCreateMethod;
        protected List<t_DataType> m_Elements = new List<t_DataType>();
        protected List<t_UiElemType> m_VisualElements = new List<t_UiElemType>();

        protected int m_DisplayedElementsCount = 5;
        protected int m_Page = 0;

        protected XUIIconButton m_LeftPageButton;
        protected XUIIconButton m_RightPageButton;

        protected XUIHLayout m_ElementsContainer;

        protected int m_Height;
        protected int m_Width;

        //////////////////////////////////////////////////////
        /////////////////////////////////////////////////////

        private void OnCreation(CHLayout p_Layout)
        {
            var l_ArrowTexture = CustomLevelSelectionMenuReferences.ArrowImage;
            Sprite l_ArrowSprite = Sprite.Create(l_ArrowTexture, new Rect(0, 0, l_ArrowTexture.width, l_ArrowTexture.height), Vector2.zero);

            XUIIconButton.Make()
                .SetSprite(l_ArrowSprite)
                .SetHeight(m_Height)
                .SetWidth(m_Height)
                .BuildUI(Element.transform);
            XUIHLayout.Make(
                
            )
                .SetWidth(m_Width)
                .SetHeight(m_Height)
                .Bind(ref m_ElementsContainer);            
            XUIIconButton.Make()
                .SetSprite(l_ArrowSprite)
                .SetHeight(m_Height)
                .SetWidth(m_Height)
                .BuildUI(Element.transform);

            UpdateElements();
        }

        private int GetMaxPage() => (int)(m_Elements.Count() / m_DisplayedElementsCount);

        private void GoLeft()
        {
            if (m_Page == 0) return;

            m_Page -= 1;
            UpdateElements();
        }

        private void GoRight()
        {
            if (m_Page == GetMaxPage()) return;

            m_Page += 1;
            UpdateElements();

        }

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
            for (int l_i = (m_Page * m_DisplayedElementsCount); l_i < m_Elements.Count();l_i++)
            {
                if (l_i - m_Page > m_DisplayedElementsCount) break;

                var l_Item = m_Elements[l_i];
                int l_VisualElementIndex = l_i - (m_Page * m_DisplayedElementsCount);
                if (l_VisualElementIndex > m_VisualElements.Count() - 1)
                    m_VisualElements.Add(m_UiElemCreateMethod.Invoke(m_ElementsContainer.Element.transform));

                m_OnNeedToUpdateCell.Invoke(m_Elements[l_i], m_VisualElements[l_i - (m_Page * m_DisplayedElementsCount)]);
            }
        }
    }
}
