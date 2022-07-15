using GuildSaberProfile.Configuration;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using UnityEngine;
using System.Collections.Generic;

namespace GuildSaberProfile.UI.GuildSaber
{
    public struct GuildCategorys
    {
        public string GuildName { get; set; }
        public List<string> Categorys { get; set; }

        public GuildCategorys(string p_GuildName, List<string> p_Categorys)
        {
            GuildName = p_GuildName;
            Categorys = p_Categorys;
        }
    }

    [HotReload(RelativePathToLayout = @"PlayerCard_UI.bsml")]
    [ViewDefinition("GuildSaberProfile.UI.Card.View.PlayerCard_UI.bsml")]
    class ModFlowCoordinator : FlowCoordinator
    {
        public ModViewController _modViewController;

        public LeftModViewController _LeftModViewController;

        FlowCoordinator _LastFlow;

        public void Awake()
        {
            if (_modViewController == null)
                _modViewController = BeatSaberUI.CreateViewController<ModViewController>();

            if (_LeftModViewController == null)
                _LeftModViewController = BeatSaberUI.CreateViewController<LeftModViewController>();
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!firstActivation)
                return;

            SetTitle("GuildSaber");
            showBackButton = true;
            _modViewController.Init(PluginConfig.Instance.SelectedGuild, this);
            ProvideInitialViewControllers(_modViewController, _LeftModViewController);
        }

        public void ShowFlow(bool p_IsFromChildFlowCoordinator)
        {
            if (!p_IsFromChildFlowCoordinator)
                _LastFlow = BeatSaberUI.MainFlowCoordinator;
            else
                _LastFlow = BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();

            _LastFlow.PresentFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical, false, false);
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            base.BackButtonWasPressed(topViewController);
            _LastFlow.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Vertical, false);
        }
    }
}
