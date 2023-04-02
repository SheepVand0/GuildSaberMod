using BeatSaberMarkupLanguage;
using CP_SDK.Config;
using GuildSaber.Configuration;
using GuildSaber.UI.GuildSaber;
using GuildSaber.UI.Settings;
using HMUI;
using UnityEngine;


#nullable enable
namespace GuildSaber.UI.FlowCoordinator
{
    internal class ModFlowCoordinator : CustomFlowCoordinator
    {
        public CustomLevelSelectionMenu.GuildSelectionMenu _modViewController;
        //public LeftModViewController _LeftModViewController;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override string Title => "Guild Saber";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override (ViewController?, ViewController?, ViewController?) GetUIImplementation()
        {
            if (_modViewController == null)
                _modViewController = BeatSaberUI.CreateViewController<CustomLevelSelectionMenu.GuildSelectionMenu>();
            //if (_LeftModViewController == null)
            //    _LeftModViewController = BeatSaberUI.CreateViewController<LeftModViewController>();
            return (_modViewController, null /*_LeftModViewController*/, null);
        }

        //protected override void OnShow() => _modViewController.Init(JsonConfig<GSConfig>.Instance.SelectedGuild);
    }
}