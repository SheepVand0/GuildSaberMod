using BeatSaberMarkupLanguage;
using GuildSaber.Configuration;
using GuildSaber.UI.GuildSaber;
using GuildSaber.UI.Settings;
using HMUI;

namespace GuildSaber.UI.FlowCoordinator
{
    internal class ModFlowCoordinator : CustomFlowCoordinator
    {
        public PlaylistViewController _modViewController;

        public LeftModViewController _LeftModViewController;

        private HMUI.FlowCoordinator _LastFlow = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        protected override string Title => "Guild Saber";
        protected override (ViewController?, ViewController?, ViewController?) GetUIImplementation()
        {
            if (_modViewController == null)
            {
                _modViewController = BeatSaberUI.CreateViewController<PlaylistViewController>();
            }

            if (_LeftModViewController == null)
            {
                _LeftModViewController = BeatSaberUI.CreateViewController<LeftModViewController>();
            }

            return (_modViewController, _LeftModViewController, null);
        }

        protected override void OnShow()
        {
            _modViewController.Init(GSConfig.Instance.SelectedGuild);
        }
    }
}
