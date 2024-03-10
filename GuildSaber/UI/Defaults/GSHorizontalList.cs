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
        protected List<XUIHSpacer> m_EmptyElementSpacer = new List<XUIHSpacer>();

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

            (m_LeftPageButton = XUIIconButton.Make()
                .SetSprite(l_ArrowSprite)
                .SetHeight(m_Height / 2)
                .SetWidth(m_Height / 2)
                .OnClick(GoLeft))
                .BuildUI(Element.transform);
             XUIHLayout.Make()
                .SetWidth(m_Width)
                .SetHeight(m_Height)
                .Bind(ref m_ElementsContainer)
                .BuildUI(Element.transform);
            (m_RightPageButton = XUIIconButton.Make()
                .SetSprite(l_ArrowSprite)
                .SetHeight(m_Height / 2)
                .SetWidth(m_Height / 2)
                .OnClick(GoRight)
                )
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
            foreach (var l_Item in m_VisualElements)
            {
                m_SetActiveFunction.Invoke(l_Item, false);
            }

            foreach (var l_Item in m_EmptyElementSpacer)
            {
                l_Item.SetActive(true);
            }

            for (int l_i = (m_Page * m_DisplayedElementsCount); l_i < m_Elements.Count();l_i++)
            {
                if (l_i - (m_Page * m_DisplayedElementsCount) >= m_DisplayedElementsCount) break;

                var l_Item = m_Elements[l_i];
                int l_VisualElementIndex = l_i - (m_Page * m_DisplayedElementsCount);
                if (l_VisualElementIndex > m_VisualElements.Count() - 1)
                {
                    m_VisualElements.Add(m_UiElemCreateMethod.Invoke(m_ElementsContainer.Element.transform));
                    var l_Spacer = XUIHSpacer.Make(m_Height);
                    l_Spacer.BuildUI(m_ElementsContainer.Element.transform);
                    l_Spacer.SetActive(false);
                    m_EmptyElementSpacer.Add(l_Spacer);
                }

                m_SetActiveFunction.Invoke(m_VisualElements[l_VisualElementIndex], true);
                m_EmptyElementSpacer[l_VisualElementIndex].SetActive(false);
                m_OnNeedToUpdateCell.Invoke(m_Elements[l_i], m_VisualElements[l_VisualElementIndex]);
            }
        }
    }
}
