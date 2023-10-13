using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.Logger;
using GuildSaber.UI.Defaults;
using GuildSaber.Utils;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardScopeSelector : XUIVLayout
    {
        private static readonly Color m_Blue = new Color(0f, 0.7f, 1f, 0.8f);
        private static readonly Color m_Grey = new Color(1, 1, 1, 0.8f);

        public event Action<ELeaderboardScope> eOnScopeChanged;
        public event Action eOnPageChanged;

        protected LeaderboardScopeSelector(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            OnReady(OnCreation);
        }

        public static LeaderboardScopeSelector Make()
        {
            return new LeaderboardScopeSelector("GuildSaberLeaderboardScopeSelector");
        }

        protected ELeaderboardScope m_SelectedScope;
        protected int m_Page;
        protected int m_MaxPage;

        protected XUISecondaryButton m_PageUpButton;
        protected XUISecondaryButton m_PageDownButton;

        protected XUIIconButton m_WorldButton;
        protected XUIIconButton m_MeButton;
        protected XUIIconButton m_CountryButton;

        Dictionary<ELeaderboardScope, XUIIconButton> m_ScopeButtons = new Dictionary<ELeaderboardScope, XUIIconButton>();

        public event Action PageUp;
        public event Action PageDown;

        private void OnCreation(CVLayout p_Layout)
        {

            XUISecondaryButton.Make("^")
                .OnClick(OnPageUpPressed)
                .Bind(ref m_PageUpButton).BuildUI(Element.LElement.transform);

            XUIVLayout.Make(
                XUIIconButton.Make()
                    .OnClick(() =>
                    {
                        SetScope(ELeaderboardScope.Global);
                    })
                    .Bind(ref m_WorldButton),
                XUIIconButton.Make()
                    .OnClick(() =>
                    {
                        SetScope(ELeaderboardScope.Around);
                    })
                    .Bind(ref m_MeButton),
                XUIIconButton.Make()
                    .OnClick(() =>
                    {
                        SetScope(ELeaderboardScope.Country);
                    })
                    .Bind(ref m_CountryButton)
            )
            .SetBackground(true)
            .SetBackgroundColor(new Color(0, 0, 0, 0.7f))
            .BuildUI(Element.LElement.transform);

            XUISecondaryButton.Make("v")
                .OnClick(OnPageDownPressed)
                .Bind(ref m_PageDownButton)
                .BuildUI(Element.LElement.transform);

            Texture2D l_GlobalIcon = AssemblyUtils.LoadTextureFromAssembly("GuildSaber.Resources.GlobalIcon.png");
            Texture2D l_AroundMeIcon = AssemblyUtils.LoadTextureFromAssembly("GuildSaber.Resources.AroundMeIcon.png");
            Texture2D l_CountryIcon = AssemblyUtils.LoadTextureFromAssembly("GuildSaber.Resources.CountryIcon.png");

            m_WorldButton.SetSprite(Sprite.Create(l_GlobalIcon, new Rect(0, 0, l_GlobalIcon.width, l_GlobalIcon.height), new Vector2()));
            m_MeButton.SetSprite(Sprite.Create(l_AroundMeIcon, new Rect(0, 0, l_AroundMeIcon.width, l_AroundMeIcon.height), new Vector2()));
            m_CountryButton.SetSprite(Sprite.Create(l_CountryIcon, new Rect(0, 0, l_CountryIcon.width, l_CountryIcon.height), new Vector2()));

            m_ScopeButtons.Add(ELeaderboardScope.Global, m_WorldButton);
            m_ScopeButtons.Add(ELeaderboardScope.Around, m_MeButton);
            m_ScopeButtons.Add(ELeaderboardScope.Country, m_CountryButton);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void SetScope(ELeaderboardScope p_Scope)
        {
            m_SelectedScope = p_Scope;

            foreach (var l_Index in m_ScopeButtons)
            {
                try
                {
                    if (l_Index.Key == p_Scope)
                    {
                        l_Index.Value.SetColor(m_Blue);
                    }
                    else
                    {
                        l_Index.Value.SetColor(m_Grey);
                    }
                }
                catch (Exception ex)
                {
                    GSLogger.Instance.Error(ex, nameof(LeaderboardScopeSelector), nameof(SetScope));
                }
            }

            eOnScopeChanged?.Invoke(m_SelectedScope);
        }

        public void SetPage(int p_Page, int p_MaxPage)
        {
            m_Page = p_Page;
            m_MaxPage = p_MaxPage;
            UpdatePageButtons();
        }  

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void UpdatePageButtons()
        {
            m_PageUpButton.SetInteractable(m_Page > 1);
            m_PageDownButton.SetInteractable(m_Page < m_MaxPage);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        protected void OnPageDownPressed()
        {
            m_Page += 1;
            eOnPageChanged?.Invoke();
            UpdatePageButtons();
        }

        protected void OnPageUpPressed()
        {
            if (m_Page == 1)
            {
                UpdatePageButtons();
                return;
            }

            m_Page -= 1;
            UpdatePageButtons();
            eOnPageChanged?.Invoke();
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public ELeaderboardScope GetSelectedScope() => m_SelectedScope;

        public int GetPage() => m_Page;

        public int GetMaxPage() => m_MaxPage;

    }
}
