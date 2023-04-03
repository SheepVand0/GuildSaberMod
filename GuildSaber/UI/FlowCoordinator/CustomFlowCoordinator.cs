using BeatSaberMarkupLanguage;
using HMUI;
using UnityEngine;


#nullable enable
namespace GuildSaber.UI.FlowCoordinator
{
    internal abstract class CustomFlowCoordinator : HMUI.FlowCoordinator
    {
        private HMUI.FlowCoordinator m_LastFlowCoordinator;

        protected abstract string Title { get; }

        protected virtual void OnCreation() { }

        protected abstract (ViewController?, ViewController?, ViewController?) GetUIImplementation();

        public void Awake()
        {
            OnCreation();
        }

        protected override void DidActivate(
          bool firstActivation,
          bool addedToHierarchy,
          bool screenSystemEnabling)
        {
            if (!firstActivation)
                return;
            SetTitle(Title, (ViewController.AnimationType)1);
            (ViewController?, ViewController?, ViewController?) l_ViewControllers = GetUIImplementation();
            showBackButton = true;
            ProvideInitialViewControllers(l_ViewControllers.Item1, l_ViewControllers.Item2, l_ViewControllers.Item3, null, null);
        }

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
                return;

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
