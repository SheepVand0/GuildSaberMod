// Decompiled with JetBrains decompiler
// Type: GuildSaber.Plugin
// Assembly: GuildSaber, Version=0.2.5.0, Culture=neutral, PublicKeyToken=null
// MVID: 2E8F8B5B-092B-47A3-9F65-4C90A48B7328
// Assembly location: C:\Users\user\Desktop\GuildSaber.dll

using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using GuildSaber.Logger;
using GuildSaber.UI;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.UI.Defaults;
using GuildSaber.UI.FlowCoordinator;
using IPA;
using System;
using UnityEngine;


#nullable enable
namespace GuildSaber
{
    [IPA.Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public const string NOT_DEFINED = "Undefined";
        internal static GuildSelectionFlowCoordinator SelectionFlowCoordinator;

        private static Plugin Instance { get; set; }

        [IPA.Init]
        public void Init(IPA.Logging.Logger p_Logger)
        {
            Plugin.Instance = this;
            GSLogger gsLogger = new GSLogger(p_Logger);
            GuildSaberModule.HarmonyInstance.PatchAll();
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += GuildSaberModule.StartupPatches;
        }



        private void ShowSelectionFlow()
        {
            if (SelectionFlowCoordinator == null)
                SelectionFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<GuildSelectionFlowCoordinator>();
            SelectionFlowCoordinator.Present();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            foreach (Component l_Index in Resources.FindObjectsOfTypeAll<CustomUIComponent>())
                GameObject.DestroyImmediate(l_Index.gameObject);
        }
    }
}
