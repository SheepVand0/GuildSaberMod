using BeatSaberMarkupLanguage;
using HMUI;

// ReSharper disable once CheckNamespace
namespace GuildSaber.UI
{
    internal abstract class CustomFlowCoordinator : FlowCoordinator
    {

        protected abstract string Title { get; }

        private FlowCoordinator m_LastFlowCoordinator = null;

        /// <summary>
        /// Get view controllers
        /// </summary>
        /// <returns>Center view controller, Left View Controller, Right ViewController</returns>
        protected abstract (ViewController?, ViewController?, ViewController?) GetUIImplementation();

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!firstActivation) return;

            SetTitle(Title);
            var l_ViewControllers = GetUIImplementation();
            ProvideInitialViewControllers(l_ViewControllers.Item1, l_ViewControllers.Item2, l_ViewControllers.Item3);
        }

        // ReSharper disable once ParameterHidesMember
        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            base.BackButtonWasPressed(topViewController);

            Hide();
        }

        public void Show()
        {
            m_LastFlowCoordinator = BeatSaberUI.MainFlowCoordinator.YoungestChildFlowCoordinatorOrSelf();
            m_LastFlowCoordinator.PresentFlowCoordinator(this);
            OnShow();
        }

        public void Hide()
        {
            if (m_LastFlowCoordinator == null) return;
            m_LastFlowCoordinator.DismissFlowCoordinator(this);
            m_LastFlowCoordinator = null;
            OnHide();
        }

        protected virtual void OnShow() {}

        protected virtual void OnHide() {}
    }
}
