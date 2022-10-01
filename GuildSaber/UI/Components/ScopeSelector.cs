using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.UI.Components
{
    class ScopeSelector : CustomUIComponent
    {
        protected override string m_ViewResourceName => "GuildSaber.UI.Components.Views.ScopeSelector.bsml";

        #region UIComponents
        [UIComponent("ScopesLayout")] VerticalLayoutGroup m_Layout = null;
        [UIComponent("Global")] ClickableImage m_GlobalImage = null;
        [UIComponent("Around")] ClickableImage m_AroundImage = null;
        //[UIComponent("Friends")] ClickableImage m_FriendsImage = null;
        [UIComponent("Country")] ClickableImage m_CountryImage = null;
        #endregion

        #region Properties
        private float IconHeight { get => 5; set { } }

        private static Color m_Blue = new Color(0f, 0.7f, 1f, 0.8f);
        private static Color m_Grey = new Color(1, 1, 1, 0.8f);
        private List<ClickableImage> m_Scopes = new List<ClickableImage>();
        #endregion

        [UIAction("#post-parse")]
        public void PostParse()
        {
            m_GlobalImage.name = "Global";
            m_AroundImage.name = "Around";
            //m_FriendsImage.name = "Friends";
            m_CountryImage.name = "Country";
            m_Scopes.Add(m_GlobalImage);
            m_Scopes.Add(m_AroundImage);
            //m_Scopes.Add(m_FriendsImage);
            m_Scopes.Add(m_CountryImage);

            m_GlobalImage.DefaultColor = m_Blue;
        }

        public void SelectScope(ELeaderboardScope p_Scope)
        {
            bool l_IsScopeValid = false;
            foreach (ClickableImage l_Current in m_Scopes)
                if (l_Current.name == p_Scope.ToString())
                {
                    if (l_Current.gradient) l_Current.gradient = false;
                    l_Current.DefaultColor = m_Blue;
                    l_IsScopeValid = true;
                }
                else
                    l_Current.DefaultColor = m_Grey;

            if (l_IsScopeValid)
                Events.m_Instance.SelectScope(p_Scope);
            else
                Plugin.Log.Error($"Invalid scope provided : {p_Scope}");
        }

        #region UIActions
        [UIAction("GlobalClick")] private void GlobalOnClick() { SelectScope(ELeaderboardScope.Global); }
        [UIAction("AroundClick")] private void AroundOnClick() { SelectScope(ELeaderboardScope.Around); }
        //[UIAction("FriendsClick")] private void FriendsOnClick() { SelectScope("Friends"); }
        [UIAction("LocationClick")] private void LocationOnClick() { SelectScope(ELeaderboardScope.Country); }
        #endregion
    }
}

public enum ELeaderboardScope
{
    None     = 0,
    Global   = 1 << 0,
    Around   = 1 << 1,
    Country  = 1 << 2
}