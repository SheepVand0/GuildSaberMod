using BeatSaberMarkupLanguage.TypeHandlers;
using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.UI.CustomLevelSelectionMenu;
using GuildSaber.Utils;
using PlaylistManager.HarmonyPatches;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

namespace GuildSaber.UI.Defaults
{
    internal class GSVClosable : XUIVLayout
    {
        public const float BUTTON_SIZE = 5;

        protected GSVClosable(string p_Name, int p_Width, int p_Height, EDirection p_Direction, params IXUIElement[] p_Childs) : base(p_Name, new IXUIElement[] { })
        {
            ContainedInContainer = p_Childs;
            m_Width = p_Width;
            m_Height = p_Height;
            if (p_Direction == EDirection.Horizontal)
                m_IsHorizontal = true;
            OnReady(OnCreationReady);
        }

        /*public static GSVClosable Create(IXUIElement p_ContainedInContainer, int p_Width, int p_Height)
        {
            var l_Value = p_ContainedInContainer;
            var l_Me = new GSVClosable("Closable", l_Value, p_Width, p_Height);
            return l_Me;
        }*/

        public static GSVClosable Create(int p_Width, int p_Height, EDirection p_Direction, params IXUIElement[] p_ContainedInContainer)
        {
            var l_Childs = p_ContainedInContainer;
            var l_Me = new GSVClosable("Closable", p_Width, p_Height, p_Direction, l_Childs);
            return l_Me;
        }
        

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        protected XUIVLayout TopButtonTransform;
        protected XUIVLayout BottomButtonTransform;
        protected XUIVLayout VerticalContainer;
        protected GSIconButtonWithBackground Button;
        protected XUIHLayout LeftButtonTransform;
        protected XUIHLayout RightButtonTransform;
        protected XUIHLayout HorizontalContainer;
        protected GSIconButtonWithBackground HorizontalButton;
        protected IXUIElement[] ContainedInContainer;

        protected int m_Width;
        protected int m_Height;

        public XUIVLayout GetVerticalContainer() => VerticalContainer;

        public XUIHLayout GetHorizontalContainer() => HorizontalContainer;

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        protected void OnCreationReady(CVLayout p_Layout)
        {
            GSIconButtonWithBackground.Make()
                        .Bind(ref Button);

            if (!m_IsHorizontal)
            {

                XUIVLayout.Make(
                    Button.SetWidth(m_Width)
                          .SetHeight(BUTTON_SIZE, true)
                    ).Bind(ref TopButtonTransform)
                    .SetWidth(m_Width)
                    .SetMinWidth(m_Width)
                    .SetMinHeight(BUTTON_SIZE)
                    .SetHeight(BUTTON_SIZE)
                    .SetMinHeight(BUTTON_SIZE)
                    .BuildUI(Element.transform);
                XUIVLayout.Make(ContainedInContainer)
                    .SetWidth(m_Width)
                    .SetMinWidth(m_Width)
                    .SetHeight(m_Height)
                    .SetMinHeight(m_Height)
                    .Bind(ref VerticalContainer).BuildUI(Element.transform);
                XUIVLayout.Make()
                    .SetWidth(m_Width)
                    .SetMinWidth(m_Width)
                    .SetHeight(BUTTON_SIZE)
                    .SetMinHeight(BUTTON_SIZE)
                    .SetActive(false)
                    .Bind(ref BottomButtonTransform).BuildUI(Element.transform);
            }

            /////////////////////////////////////////////////////////////////
            ////////////////////////////////////////////////////////////////

            if (m_IsHorizontal)
            {

                XUIHLayout.Make(
                    XUIHLayout.Make(
                        Button.SetWidth(BUTTON_SIZE, true)
                              .SetHeight(m_Height)
                        )
                        .Bind(ref LeftButtonTransform)
                        .SetHeight(m_Height)
                        .SetWidth(BUTTON_SIZE)
                        .SetMinHeight(m_Height)
                        .SetMinWidth(BUTTON_SIZE),

                    XUIHLayout.Make(ContainedInContainer)
                        .Bind(ref HorizontalContainer)
                        .SetHeight(m_Height)
                        .SetWidth(m_Width)
                        .SetMinWidth(m_Width)
                        .SetMinHeight(m_Height),

                    XUIHLayout.Make()
                        .SetWidth(BUTTON_SIZE)
                        .SetMinWidth(BUTTON_SIZE)
                        .SetMinHeight(m_Height)
                        .SetHeight(m_Height)
                        .Bind(ref RightButtonTransform)
                        .SetActive(false)
                ).BuildUI(Element.transform);
            }

            SetSpacing(0.05f);

            Button.OnClick(OnButtonClick);

            Button.SetIcon(CustomLevelSelectionMenuReferences.ArrowImage);

            Show(false);
        }

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        protected EButtonSide m_Side = EButtonSide.Top;
        protected bool m_IsShown;
        protected bool m_IsHorizontal = false;
        protected List<GSVClosable> m_LinkedWidgets = new List<GSVClosable>();

        public enum EButtonSide
        {
            Top,
            Bottom
        }

        public bool IsShown() => m_IsShown;

        public bool IsHorizontal() => m_IsHorizontal;

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        public void Show(bool p_Show, bool p_ModifyLinkedWidgets = true)
        {
            m_IsShown = p_Show;
            SetContainerActive(true);
            float l_RotationValue = 0;
            if (m_IsShown)
            {
                l_RotationValue = 180;
                
            } else
            {
                l_RotationValue = 0;
            }
            if (m_IsHorizontal)  l_RotationValue += (m_Side == EButtonSide.Top) ? 90 : -90;

            Button.GetIconButton().Element.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, l_RotationValue));
            FastAnimator.Animate(new List<FastAnimator.FloatAnimKey>() {
                new FastAnimator.FloatAnimKey((p_Show) ? 0 : 1, 0),
                new FastAnimator.FloatAnimKey(p_Show ? 1 : 0, 0.2f)
            }, (x) => {

                if (m_IsHorizontal)
                {
                    HorizontalContainer.Element.transform.localScale
                    = new Vector3(x, 1, 1);
                }
                else
                {
                    VerticalContainer.Element.transform.localScale =
                    new Vector3(1, x, 1);
                }
                
            }, () =>
            {
                if (!m_IsShown)
                    SetContainerActive(false);
            });

            if (!p_ModifyLinkedWidgets) return;

            foreach (var l_Linked in m_LinkedWidgets)
            {
                if ((p_Show) ? !l_Linked.IsShown() : l_Linked.IsShown()) continue;

                l_Linked.Show(!p_Show, false);
            }
        }

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        protected void SetContainerActive(bool p_Active) 
        {
            if (!m_IsHorizontal)
            {
                VerticalContainer.SetActive(p_Active);
            } else
            {
                HorizontalContainer.SetActive(p_Active);
            }
        }

        public GSVClosable LinkClosable(GSVClosable p_Closable)
        {
            m_LinkedWidgets.Add(p_Closable);
            return this;
        }

        public GSVClosable Bind(ref GSVClosable p_Ref)
        {
            p_Ref = this;
            return this;
        }

        public new GSVClosable SetSpacing(float p_Spacing)
        {
            OnReady(x =>
            {
                if (!m_IsHorizontal)
                {
                    VerticalContainer.SetSpacing(p_Spacing);
                } else
                {
                    HorizontalContainer.SetSpacing(p_Spacing);
                }
            });
            return this;
        }
        
        public GSVClosable OnVerticalContainerReady(Action<CVLayout> p_Action)
        {
            OnReady(x =>
            {
                p_Action.Invoke(GetVerticalContainer().Element);
            });
            return this;
        }

        public GSVClosable OnHorizontalContainerReady(Action<CHLayout> p_Action)
        {
            OnReady(x =>
            {
                p_Action.Invoke(GetHorizontalContainer().Element);
            });
            return this;
        }

        public GSVClosable SetButtonVisible(bool p_Visible)
        {
            OnReady(x =>
            {
                if (!m_IsHorizontal)
                {
                    TopButtonTransform.SetActive(m_Side == EButtonSide.Top ? p_Visible : false);
                    BottomButtonTransform.SetActive(m_Side == EButtonSide.Bottom ? p_Visible : false);
                } else
                {
                    LeftButtonTransform.SetActive(m_Side == EButtonSide.Top ? p_Visible : false);
                    RightButtonTransform.SetActive(m_Side == EButtonSide.Bottom ? p_Visible : false);
                }
            });
            return this;
        }

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////

        private void OnButtonClick()
        {
            if (m_IsShown)
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

        ////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        
        public enum EDirection
        {
            Vertical,
            Horizontal
        }
    }
}
