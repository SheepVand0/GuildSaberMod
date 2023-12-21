using BeatSaberMarkupLanguage.TypeHandlers;
using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.UI.CustomLevelSelectionMenu;
using GuildSaber.Utils;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace GuildSaber.UI.Defaults
{
    internal class GSVClosable : XUIVLayout
    {
        protected GSVClosable(string p_Name, IXUIElement p_ContainedInContainer, int p_Width, int p_Height, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            ContainedInContainer = p_ContainedInContainer;
            m_Width = p_Width;
            m_Height = p_Height;
            OnReady(OnCreationReady);
        }

        public static GSVClosable Create(IXUIElement p_ContainedInContainer, int p_Width, int p_Height)
        {
            var l_Value = p_ContainedInContainer;
            var l_Me = new GSVClosable("Closable", l_Value, p_Width, p_Height);
            return l_Me;
        }

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        protected XUIVLayout TopButtonTransform;
        protected XUIVLayout BottomButtonTransform;
        protected XUIVLayout Container;
        protected GSIconButtonWithBackground Button;
        protected IXUIElement ContainedInContainer;

        protected int m_Width;
        protected int m_Height;

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        protected async void OnCreationReady(CVLayout p_Layout)
        {
            XUIVLayout.Make(
                GSIconButtonWithBackground.Make()
                    .Bind(ref Button)
                    .SetWidth(m_Width)
                    .SetHeight(7)
                    .SetMinWidth(m_Width)
                ).Bind(ref TopButtonTransform)
                .SetWidth(m_Width)
                .SetMinWidth(m_Width)
                .SetHeight(7)
                .BuildUI(Element.transform);
            XUIVLayout.Make(ContainedInContainer)
                .SetWidth(m_Width)
                .SetMinWidth(m_Width)
                .SetHeight(m_Height)
                .Bind(ref Container).BuildUI(Element.transform);
            XUIVLayout.Make()
                .SetWidth(m_Width)
                .SetMinWidth(m_Width)
                .SetHeight(7)
                .Bind(ref BottomButtonTransform).BuildUI(Element.transform);

            Button.OnClick(OnButtonClick);

            Button.SetBackground(await GSSecondaryButton.GetBackground(m_Width, m_Height));
            Button.SetIcon(CustomLevelSelectionMenuReferences.ArrowImage);
            Button.SetHeight(7);

            Show(false);
        }

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        protected EButtonSide m_Side = EButtonSide.Top;
        protected bool IsShown;
        protected bool IsHorizontal = false;
        protected List<GSVClosable> LinkedWidgets = new List<GSVClosable>();

        public enum EButtonSide
        {
            Top,
            Bottom
        }

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        public void Show(bool p_Show)
        {
            IsShown = p_Show;
            Container.SetActive(true);
            float l_RotationValue = 0;
            if (IsShown)
            {
                l_RotationValue = 180;
                
            } else
            {
                l_RotationValue = 0;
            }
            if (IsHorizontal)  l_RotationValue += (m_Side == EButtonSide.Top) ? 90 : -90;

            Button.GetIconButton().Element.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            FastAnimator.Animate(new List<FastAnimator.FloatAnimKey>() {
                new FastAnimator.FloatAnimKey((p_Show) ? 0 : 1, 0),
                new FastAnimator.FloatAnimKey(p_Show ? 1 : 0, 0.2f)
            }, (x) => Container.Element.transform.localScale = new Vector3(IsHorizontal ? x : 1, IsHorizontal == false ? x : 1, 1), () =>
            {
                if (!IsShown)
                    Container.SetActive(false);
            });
        }

        public GSVClosable SetHorizontal()
        {

            if (IsHorizontal) return this;
            
            IsHorizontal = true;
            OnReady(x =>
            {
                Element.HOrVLayoutGroup.SetLayoutHorizontal();
                Show(false);
            });

            return this;
        }

        public GSVClosable SetVertical()
        {
            if (!IsHorizontal) return this;

            IsHorizontal = false;

            OnReady(x => {
                Element.HOrVLayoutGroup.SetLayoutVertical();
                Show(false);
            });

            return this;
        }

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        private void OnButtonClick()
        {
            if (IsShown)
            {
                Show(false);
            } else
            {
                Show(true);
            }
        }

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        public GSVClosable SetSide(EButtonSide p_Side)
        {
            OnReady(x =>
            {
                m_Side = p_Side;
                if (m_Side == EButtonSide.Top)
                {
                    TopButtonTransform.SetActive(true);
                    Button.Element.transform.parent.SetParent(TopButtonTransform.Element.transform, false);
                    Button.GetIconButton().Element.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    BottomButtonTransform.SetActive(false);
                }
                else
                {
                    BottomButtonTransform.SetActive(true);
                    Button.Element.transform.parent.SetParent(BottomButtonTransform.Element.transform, false);
                    Button.GetIconButton().Element.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    BottomButtonTransform.SetActive(false);
                }
            });
            return this;
        }
    }
}
