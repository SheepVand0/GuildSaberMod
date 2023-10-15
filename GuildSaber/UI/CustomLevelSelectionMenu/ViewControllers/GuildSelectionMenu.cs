using System.Collections.Generic;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberPlus.SDK.UI;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.UI.CustomLevelSelectionMenu.Components;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using GuildSaber.UI.Defaults;
using GuildSaber.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GuildSaber.UI.CustomLevelSelectionMenu;

internal class GuildSelectionMenu : HMUI.ViewController
{
    internal const string VIEW_CONTROLLERS_PATH = "GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Views";

    internal static CategorySelectionFlowCoordinator m_CategorySelectionFlowCoordinator;

    XUIVScrollView m_GuildsScrollView = null;
    XUISecondaryButton m_RefreshButton = null;

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

        if (!firstActivation) return;

        OnViewCreation();
    }

    protected void OnViewCreation()
    {
        XUIVLayout l_Layout = null;

        Templates.FullRectLayout(
            XUIVLayout.Make(
                GSText.Make("Loading...")
                )
                .SetWidth(65)
                .SetHeight(65)
            ).Bind(ref l_Layout).BuildUI(transform);

        

        l_Layout.Element.transform.gameObject.SetActive(false);

        Templates.FullRectLayout(
            XUIHLayout.Make(
                XUIVScrollView.Make(
                    ).Bind(ref m_GuildsScrollView)
                ).
                SetHeight(65).
                SetSpacing(0).
                SetPadding(0).
                SetBackground(true).
                OnReady(p_X => p_X.CSizeFitter.horizontalFit = p_X.CSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained).
                OnReady(p_X => p_X.HOrVLayoutGroup.childForceExpandHeight = true).
                OnReady(p_X => p_X.HOrVLayoutGroup.childForceExpandWidth = true),
           GSSecondaryButton.Make("Refresh", 15, 2, p_OnClick: Refresh).Bind(ref m_RefreshButton)
         ).BuildUI(transform);

         Refresh();
    }

    public void Refresh()
    {
        List<GuildSelectionButton> l_Buttons = new List<GuildSelectionButton>();
        foreach (GuildData l_Guild in GuildSaberModule.AvailableGuilds)
        {
            GuildSelectionButton l_But = new GuildSelectionButton(l_Guild);
            l_Buttons.Add(l_But);
            //GSLogger.Instance.Log("Continuing", IPA.Logging.Logger.LogLevel.InfoUp);
        }

        m_RefreshButton.SetActive(l_Buttons.Count == 0);

        foreach (var l_Index in l_Buttons)
        {
            l_Index.BuildUI(m_GuildsScrollView.Element.Container.transform);
        }
    }
}