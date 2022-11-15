﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.Utils;
using HMUI;
using IPA.Loader;
using IPA.Utilities;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.UI.Leaderboard.Components
{
    internal struct UpdateData
    {
        public string Version { get; set; }
        public string FileLink { get; set; }
        public string BSPlusMinVersion { get; set; }
        public string BSPlusLink { get; set; }
    }

    internal struct FileToDownload
    {
        public string FileName { get; set; }
        public string FileLink { get; set; }

        public FileToDownload(string p_FileName, string p_FileLink)
        {
            FileName = p_FileName;
            FileLink = p_FileLink;
        }
    }
    internal class UpdateView : CustomUIComponent
    {
        public const string UPDATE_FILE_LOCATION = "UserData/BeatSaberPlus/GuildSaber/Updates.json";

        protected override string m_ViewResourceName => "GuildSaber.UI.Leaderboard.Components.Views.UpdateView.bsml";

        [UIComponent("ShowUpdatesButton")] Button m_ShowUpdatesButton = null;
        [UIComponent("UdpatesModal")] private ModalView m_UpdatesModal = null;
        [UIComponent("Horizontal")] HorizontalLayoutGroup m_Horizontal = null;
        [UIComponent("UdpateText")] TextMeshProUGUI m_UpdateText = null;

        Button m_DirectDownloadButton = null;
        Button m_GithubDownloadButton = null;

        private List<FileToDownload> m_FileToDownload = new List<FileToDownload>();

        public bool m_NeedUpdate = false;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Close Modal
        /// </summary>
        [UIAction("CloseModal")]
        private void Close()
        {
            m_UpdatesModal.Hide(true);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Hide
        /// </summary>
        public async void Hide()
        {
            await WaitUtils.Wait(() => m_ShowUpdatesButton != null, 100);

            m_ShowUpdatesButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Show
        /// </summary>
        public async void Show()
        {
            await WaitUtils.Wait(() => LeaderboardHeaderManager.m_HeaderImageView != null, 100);
            await WaitUtils.Wait(() => m_ShowUpdatesButton != null, 100);

            m_ShowUpdatesButton.gameObject.SetActive(m_NeedUpdate);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// After view creation
        /// </summary>
        protected override void AfterViewCreation()
        {
            ImageView l_Back = m_ShowUpdatesButton.GetComponentsInChildren<ImageView>()[0];
            l_Back.gradient = false;

            ImageView l_NewBack = Instantiate(l_Back);
            l_NewBack.transform.SetParent(l_Back.transform.parent);
            l_NewBack.transform.localScale = l_Back.transform.localScale;
            l_NewBack.transform.localPosition = l_Back.transform.localPosition;

            l_Back.transform.localScale = Vector3.zero;

            l_NewBack.SetImage("GuildSaber.Resources.Arrow.png");
            l_NewBack.gradient = false;
            l_NewBack.color = Color.white.ColorWithAlpha(0.7f);
            l_NewBack.SetField("_skew", 0f);

            ImageView l_OtherBack = m_ShowUpdatesButton.GetComponentsInChildren<ImageView>()[1];
            l_OtherBack.transform.localScale = Vector3.zero;

            m_DirectDownloadButton = BeatSaberPlus.SDK.UI.Button.Create(m_Horizontal.transform, "Direct", DownloadUpdates, p_PreferedWidth: 20);
            m_DirectDownloadButton.interactable = false;
            m_GithubDownloadButton = BeatSaberPlus.SDK.UI.Button.Create(m_Horizontal.transform, "Github", () =>
            {
                Process.Start("https://github.com/SheepVand0/GuildSaberProfile/releases");
            }, p_PreferedWidth: 20);

            Events.e_OnLeaderboardShown += (p_First) =>
            {
                Show();
            };

            Events.e_OnLeaderboardHide += () =>
            {
                Hide();
            };
        }

        /// <summary>
        /// Check GuildSaber and BSPlus updates
        /// </summary>
        public void CheckUpdates()
        {
            using (WebClient l_Client = new WebClient())
            {
                l_Client.DownloadFileAsync(new Uri("https://github.com/SheepVand0/GuildSaberMod/raw/main/Updates.json"), UPDATE_FILE_LOCATION);
                l_Client.DownloadFileCompleted += ProcessFile;
            }
        }

        /// <summary>
        /// Process update file
        /// </summary>
        /// <param name="p_Sender"></param>
        /// <param name="p_EventArgs"></param>
        private void ProcessFile(object p_Sender, AsyncCompletedEventArgs p_EventArgs)
        {
            if (p_EventArgs.Error != null)
            {
                m_UpdateText.SetTextError(new Exception("Error during getting updates"), GuildSaberUtils.ErrorMode.Message);
                m_Horizontal.gameObject.SetActive(false);
                return;
            }

            string l_UpdatesText = string.Empty;
            using (StreamReader l_Reader = new StreamReader(UPDATE_FILE_LOCATION))
            {
                string l_CurrentLine = string.Empty;
                while ((l_CurrentLine = l_Reader.ReadLine()) != null)
                    l_UpdatesText += l_CurrentLine;
            }

            UpdateData l_UpdateData = JsonConvert.DeserializeObject<UpdateData>(l_UpdatesText);

            string l_CurrentVersion = PluginManager.GetPluginFromId("GuildSaber").HVersion.ToString();
            string l_BSPlusVersion = PluginManager.GetPluginFromId("BeatSaberPlusCORE").HVersion.ToString();

            m_FileToDownload.Clear();

            string l_NewText = string.Empty;

            if (l_UpdateData.Version != l_CurrentVersion)
            {
                l_NewText = "GuildSaber";
                m_FileToDownload.Add(new FileToDownload("GuildSaber.dll", l_UpdateData.FileLink));
            }

            if (ParseVersion(l_BSPlusVersion) < ParseVersion(l_UpdateData.BSPlusMinVersion))
            {
                l_NewText += " and BeatSaberplus";
                m_FileToDownload.Add(new FileToDownload("BeatSaberPlus.dll", l_UpdateData.BSPlusLink));
            }

            if (m_FileToDownload.Count > 0)
            {
                m_NeedUpdate = true;
                Show();
                m_UpdateText.text = l_NewText + " need to be updated";
                m_UpdateText.color = Color.yellow;
                m_DirectDownloadButton.interactable = true;
            }
            else
            {
                Hide();
                m_UpdateText.text = "No updates needed";
                m_UpdateText.color = Color.green;
                m_DirectDownloadButton.interactable = false;
            }

            if (!Directory.Exists("IPA/Pending/Plugins"))
                Directory.CreateDirectory("IPA/Pending/Plugins");

            File.Delete(UPDATE_FILE_LOCATION);
        }

        /// <summary>
        /// Download new versions
        /// </summary>
        private async void DownloadUpdates()
        {
            m_DirectDownloadButton.interactable = false;
            using WebClient l_Client = new();
            bool l_MoveNext = false;
            l_Client.DownloadFileCompleted += (p_Sender, p_EventArgs) => { l_MoveNext = true; };
            foreach (var l_FileInfo in m_FileToDownload)
            {
                l_MoveNext = false;
                l_Client.DownloadFileAsync(new Uri(l_FileInfo.FileLink), $"IPA/Pending/Plugins/{l_FileInfo.FileName}");
                await WaitUtils.Wait(() => l_MoveNext, 100);
                continue;
            }
        }

        /// <summary>
        /// Transform version to int
        /// </summary>
        /// <param name="p_Version"></param>
        /// <returns></returns>
        private static int ParseVersion(string p_Version)
        {
            int l_Current = 0;
            for (int l_i = 0; l_i < p_Version.Length; l_i++)
            {
                if (p_Version[l_i] == '.') continue;

                l_Current += p_Version[l_i];
            }
            return l_Current;
        }
    }
}