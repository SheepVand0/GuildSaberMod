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

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);

        if (!firstActivation) return;

        OnViewCreation();
    }

    protected async void OnViewCreation()
    {
        XUIVLayout l_Layout = null;

        Templates.FullRectLayout(
            XUIVLayout.Make(
                XUIText.Make("Loading...")
                )
                .SetWidth(65)
                .SetHeight(65)
            ).Bind(ref l_Layout).BuildUI(transform);

        List<GuildSelectionButton> l_Buttons = new List<GuildSelectionButton>();
        foreach (GuildData l_Guild in GuildSaberModule.AvailableGuilds)
        {
            GuildSelectionButton l_But = new GuildSelectionButton(l_Guild);
            l_Buttons.Add(l_But);
            //GSLogger.Instance.Log("Continuing", IPA.Logging.Logger.LogLevel.InfoUp);
        }

        l_Layout.Element.transform.gameObject.SetActive(false);

        Templates.FullRectLayout(
            XUIHLayout.Make(
                XUIVScrollView.Make(
                    l_Buttons.ToArray()
                    ).Bind(ref m_GuildsScrollView)
                ).
                SetHeight(65).
                SetSpacing(0).
                SetPadding(0).
                SetBackground(true).
                OnReady(p_X => p_X.CSizeFitter.horizontalFit = p_X.CSizeFitter.verticalFit = ContentSizeFitter.FitMode.Unconstrained).
                OnReady(p_X => p_X.HOrVLayoutGroup.childForceExpandHeight = true).
                OnReady(p_X => p_X.HOrVLayoutGroup.childForceExpandWidth = true)
         ).BuildUI(transform);
    }
}