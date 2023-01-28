using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.UI.Leaderboard.Managers;
using GuildSaber.Utils;
using HMUI;
using IPA.Loader;
using IPA.Utilities;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace GuildSaber.UI.Leaderboard.Components;

internal struct UpdateData
{
    public string Version { get; set; }
    public string FileLink { get; set; }
    public string BSPlusMinVersion { get; set; }
    public string BSPlusLink { get; set; }
    public string BeatLeaderMinVersion { get; set; }
    public string BeatLeaderLink { get; set; }
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

    private static bool s_HasBeenShow;

    public bool m_NeedUpdate;

    private readonly List<FileToDownload> m_FileToDownload = new List<FileToDownload>();
    [UIComponent("Horizontal")] private readonly HorizontalLayoutGroup m_Horizontal = null;

    [UIComponent("ShowUpdatesButton")] private readonly Button m_ShowUpdatesButton = null;
    [UIComponent("UpdatesModal")] private readonly ModalView m_UpdatesModal = null;
    [UIComponent("UpdateText")] private readonly TextMeshProUGUI m_UpdateText = null;

    private Button m_DirectDownloadButton;
    private Button m_GithubDownloadButton;

    protected override string ViewResourceName
    {
        get => "GuildSaber.UI.Leaderboard.Components.Views.UpdateView.bsml";
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    [UIAction("ShowUpdates")]
    private void Open() { m_UpdatesModal.Show(true, true); }

    /// <summary>
    ///     Close Modal
    /// </summary>
    [UIAction("CloseModal")]
    private void Close() { m_UpdatesModal.Hide(true); }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     Hide
    /// </summary>
    public async void Hide()
    {
        await WaitUtils.Wait(() => m_ShowUpdatesButton != null, 100, p_MaxTryCount: 10);

        if (m_ShowUpdatesButton == null) return;
        if (!m_ShowUpdatesButton.gameObject.activeSelf) return;

        m_ShowUpdatesButton.gameObject.SetActive(false);
    }

    /// <summary>
    ///     Show
    /// </summary>
    public async void Show()
    {
        if (s_HasBeenShow) return;

        await WaitUtils.Wait(() => LeaderboardHeaderManager.m_HeaderImageView != null, 100);
        await WaitUtils.Wait(() => m_ShowUpdatesButton != null, 100);

        m_ShowUpdatesButton.gameObject.SetActive(m_NeedUpdate);

        s_HasBeenShow = true;
    }

    ////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///     After view creation
    /// </summary>
    protected override void AfterViewCreation()
    {
        ImageView l_OtherBack = m_ShowUpdatesButton.GetComponentsInChildren<ImageView>()[1];
        l_OtherBack.transform.localScale = Vector3.zero;

        m_DirectDownloadButton = BeatSaberPlus.SDK.UI.Button.Create(m_Horizontal.transform, "Direct", DownloadUpdates, p_PreferedWidth: 20);
        m_DirectDownloadButton.interactable = false;
        m_GithubDownloadButton = BeatSaberPlus.SDK.UI.Button.Create(m_Horizontal.transform, "Github", () => { Process.Start("https://github.com/SheepVand0/GuildSaberProfile/releases"); }, p_PreferedWidth: 20);

        var l_ButtonText = m_ShowUpdatesButton.GetComponentInChildren<TextMeshProUGUI>();
        l_ButtonText.richText = true;
        l_ButtonText.text = "<color=#00F200>Update Available";

        /*Vector3 l_Position = transform.localPosition;

        transform.localPosition = new Vector3(l_Position.x, l_Position.y - 10, l_Position.z);*/
    }

    /// <summary>
    ///     Check GuildSaber and BSPlus updates
    /// </summary>
    public void CheckUpdates()
    {
        using var l_Client = new WebClient();
        l_Client.DownloadFileAsync(new Uri("https://github.com/SheepVand0/GuildSaberMod/raw/main/Updates.json"), UPDATE_FILE_LOCATION);
        l_Client.DownloadFileCompleted += ProcessFile;
    }

    /// <summary>
    ///     Process update file
    /// </summary>
    /// <param name="p_Sender"></param>
    /// <param name="p_EventArgs"></param>
    private void ProcessFile(object p_Sender, AsyncCompletedEventArgs p_EventArgs)
    {
        if (p_EventArgs.Error != null) {
            m_UpdateText.SetTextError(new Exception("Error during getting updates"), GuildSaberUtils.ErrorMode.Message);
            m_Horizontal.gameObject.SetActive(false);
            return;
        }

        string l_UpdatesText = string.Empty;
        using (var l_Reader = new StreamReader(UPDATE_FILE_LOCATION)) {
            while (l_Reader.ReadLine() is { } l_CurrentLine) { l_UpdatesText += l_CurrentLine; }
        }

        var l_UpdateData = JsonConvert.DeserializeObject<UpdateData>(l_UpdatesText);

        string l_CurrentVersion = PluginManager.GetPluginFromId("GuildSaber").HVersion.ToString();
        string l_BSPlusVersion = PluginManager.GetPluginFromId("BeatSaberPlusCORE").HVersion.ToString();
        string? l_BeatLeaderVersion = PluginManager.GetPluginFromId("BeatLeader")?.HVersion.ToString();

        m_FileToDownload.Clear();

        string l_NewText = string.Empty;

        if (l_UpdateData.Version != l_CurrentVersion) {
            l_NewText = "GuildSaber";
            m_FileToDownload.Add(new FileToDownload("GuildSaber.dll", l_UpdateData.FileLink));
        }

        if (ParseVersion(l_BSPlusVersion) < ParseVersion(l_UpdateData.BSPlusMinVersion)) {
            l_NewText += $"{(l_NewText.Length > 0 ? "," : string.Empty)} BeatSaberPlus";
            m_FileToDownload.Add(new FileToDownload("BeatSaberPlus.dll", l_UpdateData.BSPlusLink));
        }

        if (l_BeatLeaderVersion != null)
            if (ParseVersion(l_BeatLeaderVersion) < ParseVersion(l_UpdateData.BeatLeaderMinVersion)) {
                l_NewText += $"{(l_NewText.Length > 0 ? "," : string.Empty)} BeatLeader";
                m_FileToDownload.Add(new FileToDownload("BeatLeader.dll", l_UpdateData.BeatLeaderLink));
            }


        if (m_FileToDownload.Count > 0) {
            m_NeedUpdate = true;
            Show();
            m_UpdateText.text = l_NewText + " need to be updated";
            m_UpdateText.color = Color.yellow;
            m_DirectDownloadButton.interactable = true;
        }
        else {
            Hide();
            m_UpdateText.text = "No updates needed";
            m_UpdateText.color = Color.green;
            m_DirectDownloadButton.interactable = false;
        }

        if (!Directory.Exists("IPA/Pending/Plugins")) Directory.CreateDirectory("IPA/Pending/Plugins");

        File.Delete(UPDATE_FILE_LOCATION);
    }

    /// <summary>
    ///     Download new versions
    /// </summary>
    private async void DownloadUpdates()
    {
        m_DirectDownloadButton.interactable = false;
        using var l_Client = new WebClient();
        bool l_MoveNext = false;
        l_Client.DownloadFileCompleted += (p_Sender, p_EventArgs) => { l_MoveNext = true; };
        foreach (FileToDownload l_FileInfo in m_FileToDownload) {
            l_MoveNext = false;
            l_Client.DownloadFileAsync(new Uri(l_FileInfo.FileLink), $"IPA/Pending/Plugins/{l_FileInfo.FileName}");
            await WaitUtils.Wait(() => l_MoveNext, 100);
        }
    }

    /// <summary>
    ///     Transform version to int
    /// </summary>
    /// <param name="p_Version"></param>
    /// <returns></returns>
    private static int ParseVersion(string p_Version)
    {
        int l_Current = 0;
        for (int l_i = 0; l_i < p_Version.Length; l_i++) {
            if (p_Version[l_i] == '.') continue;

            l_Current += (int)(p_Version[p_Version.Length - 1 - l_i] * Math.Pow(10, l_i));
        }
        return l_Current;
    }
}