using BeatSaberMarkupLanguage;
using HMUI;

namespace GuildSaber.UI.FlowCoordinator
{
    internal abstract class CustomFlowCoordinator : HMUI.FlowCoordinator
    {

        private HMUI.FlowCoordinator m_LastFlowCoordinator;

        protected abstract string Title { get; }

        /// <summary>
        ///     Get view controllers
        /// </summary>
        /// <returns>Center view controller, Left View Controller, Right ViewController</returns>
        protected abstract (ViewController?, ViewController?, ViewController?) GetUIImplementation();

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (!firstActivation)
            {
                return;
            }

            SetTitle(Title);
            (ViewController?, ViewController?, ViewController?) l_ViewControllers = GetUIImplementation();
            showBackButton = true;
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
            if (m_LastFlowCoordinator == null)
            {
                return;
            }
            m_LastFlowCoordinator.DismissFlowCoordinator(this);
            m_LastFlowCoordinator = null;
            OnHide();
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }
    }
}
