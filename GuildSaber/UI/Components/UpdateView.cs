using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Net;
using GuildSaber.Utils;
using IPA.Utilities;

namespace GuildSaber.UI.Components
{
    internal struct UpdateData
    {
        public string Version { get; set; }
        public string FileLink { get; set; }
        public string BSPlusMinVersion { get; set; }
        public string BSPlusLink { get; set; }
    }

    internal class UpdateView : CustomUIComponent
    {
        public const string UPDATE_FILE_LOCATION = "UserData/BeatSaberPlus/GuildSaber/Updates.json";

        protected override string m_ViewResourceName => "GuildSaber.UI.Components.Views.UpdateView.bsml";

        [UIComponent("UpdatesModal")] ModalView m_UpdatesModal = null;
        [UIComponent("Horizontal")] HorizontalLayoutGroup m_Horizontal = null;
        [UIComponent("UpdateText")] TextMeshProUGUI m_UpdateText = null;

        Button m_DirectDownloadButton = null;
        Button m_GithubDownloadButton = null;

        private List<string> m_FileToDownload = new List<string>();

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// After view creation
        /// </summary>
        protected override void AfterViewCreation()
        {
            m_DirectDownloadButton = BeatSaberPlus.SDK.UI.Button.Create(m_Horizontal.transform, "Direct download", Download, p_PreferedWidth: 40f);
            m_GithubDownloadButton = BeatSaberPlus.SDK.UI.Button.Create(m_Horizontal.transform, "Github download", () =>
            {
                Process.Start("https://github.com/SheepVand0/GuildSaberProfile/releases");
            }, p_PreferedWidth: 40f);
        }

        private void Download()
        {
            using (WebClient l_Client = new WebClient())
            {
                l_Client.DownloadFileAsync(new System.Uri("https://github.com/SheepVand0/GuildSaberMod/raw/main/Updates.json"), UPDATE_FILE_LOCATION);
                l_Client.DownloadFileCompleted += ProcessFile;
            }
        }

        private void ProcessFile(object p_Sender, System.ComponentModel.AsyncCompletedEventArgs p_EventArgs)
        {
            if (p_EventArgs.Error != null)
            {
                m_UpdateText.SetTextError(new Exception("Error during getting updates"), GuildSaberUtils.ErrorMode.Message);
                m_Horizontal.gameObject.SetActive(false);
                return;
            }

            UpdateData l_UpdateData = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateData>(System.IO.File.ReadAllText(UPDATE_FILE_LOCATION));

            string l_CurrentVersion = IPA.Loader.PluginManager.GetPluginFromId("GuildSaber").HVersion.ToString();
            string l_BSPlusVersion = IPA.Loader.PluginManager.GetPluginFromId("BeatSaberPlusCORE").HVersion.ToString();

            m_FileToDownload.Clear();

            if (l_UpdateData.Version != l_CurrentVersion)
                m_FileToDownload.Add(l_UpdateData.FileLink);


        }

    }
}
