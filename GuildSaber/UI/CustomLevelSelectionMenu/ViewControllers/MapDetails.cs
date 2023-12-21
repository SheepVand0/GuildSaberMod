using BeatSaberMarkupLanguage;
using BeatSaberPlus.SDK.Game;
using CP_SDK.UI.Components;
using CP_SDK.Unity;
using CP_SDK.XUI;
using GuildSaber.Logger;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using GuildSaber.UI.Defaults;
using GuildSaber.Utils;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers
{
    internal class MapDetails : XUIVLayout
    {
        protected MapDetails(params IXUIElement[] p_Childs) : base("Map Details", p_Childs)
        {
            OnReady(OnLayoutReady);
        }

        protected int m_OldWidth = 0;

        protected GSText m_MapName;
        protected GSText m_MapAuthor;
        protected GSText m_MapDuration;
        protected GSText m_Modifiers;
        protected GSText m_MapMapper;
        protected GSText m_BPM;

        protected GSText m_NPS;
        protected GSText m_Bombs;
        protected GSText m_Walls;

        protected XUIImage m_MapCover;
        protected GSText m_DifficultyLabel;
        protected GSSecondaryButton m_PracticeButton;
        protected GSSecondaryButton m_PlayButton;

        internal static SongPreviewPlayer m_MapPreviewAudio;

        internal static PlayerData m_PlayerData;

        protected GSSecondaryButton m_ScoreSaberButton;

        protected IDifficultyBeatmap m_Beatmap = null;

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public static MapDetails Make()
        {
            m_PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;

            return new MapDetails();
        }

        public MapDetails Bind(ref MapDetails p_Ref)
        {
            p_Ref = this;
            return this;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public GSSecondaryButton GetShowScoreSaberButton() => m_ScoreSaberButton;

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        private void OnLayoutReady(CVLayout p_Layout)
        {

            XUIHLayout.Make(
                XUIVLayout.Make(
                    XUIImage.Make()
                        .Bind(ref m_MapCover)
                ).SetWidth(15)
                 .SetHeight(15),
                XUIVLayout.Make(
                    XUIHLayout.Make(
                            m_MapName = (GSText)GSText.Make(string.Empty)
                                .SetMargins(-5, 0, 0, 0)
                                .SetAlign(TMPro.TextAlignmentOptions.Left),
                            m_MapDuration = (GSText)GSText.Make(string.Empty)
                                .SetAlign(TMPro.TextAlignmentOptions.Left)
                     )
                    .SetHeight(4)
                    .SetMinHeight(4),//.OnReady(x => x.CSizeFitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained),
                    XUIHLayout.Make(
                        GSText.Make(string.Empty)
                            .SetMargins(-5, 0, 0, 0)
                            .Bind(ref m_MapAuthor),
                        m_MapMapper = (GSText)GSText.Make(string.Empty)
                            .SetColor(new Color(0, 0.8f, 1, 0.7f)),
                        GSText.Make(string.Empty).Bind(ref m_BPM)
                            .SetFontSize(2)
                            .SetAlign(TMPro.TextAlignmentOptions.Right)
                    ).ForEachDirect<XUIText>((x) => x.SetFontSize(2))
                    .SetMinHeight(3)
                ).SetWidth(40)
                .OnReady(x => x.LElement.preferredWidth = 40)
            ).SetMinWidth(60)
             //.SetMinHeight(20)
             .SetHeight(15)
             .SetSpacing(0)
             .BuildUI(Element.transform);

            if (m_MapPreviewAudio == null)
                m_MapPreviewAudio = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().First();

            XUIVLayout.Make(
                    XUIHLayout.Make(
                        GSText.Make(string.Empty).Bind(ref m_NPS),
                        GSText.Make(string.Empty).Bind(ref m_Bombs),
                        GSText.Make(string.Empty).Bind(ref m_Walls)
                        ).SetMinHeight(2.5f).SetBackground(true)
            ).BuildUI(Element.transform);

            XUIHLayout.Make(
                m_DifficultyLabel = GSText.Make("")
                )
                .SetBackground(true)
                .SetActive(true)
                //.SetMinHeight(5)
                .SetHeight(5)
                .BuildUI(Element.transform);

            XUIHLayout.Make(
                m_PracticeButton = (GSSecondaryButton)GSSecondaryButton.Make("Practice", 24, 10).OnClick(OnPracticeClicked),
                m_PlayButton = (GSSecondaryButton)GSSecondaryButton.Make("Play", 24, 10).OnClick(PlayMap)
            )
            //.SetMinHeight(15)
            .SetHeight(15)
            .BuildUI(Element.transform);

            (m_ScoreSaberButton = GSSecondaryButton.Make("Show ScoreSaber", 50, 5, p_OnClick: GuildSaberLeaderboardViewController.Instance.OnScoreSaberButton))
                .BuildUI(Element.transform);

            SetSpacing(0);
            SetActive(false);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public MapDetails SetMap(IDifficultyBeatmap p_Beatmap, string p_Hash)
        {
            m_Beatmap = p_Beatmap;
            OnReady(async x =>
            {
                Sprite l_Sprite = await p_Beatmap.level.GetCoverImageAsync(new CancellationToken());

                m_MapName.SetText(GuildSaberUtils.GetPlayerNameToFit(p_Beatmap.level.songName, 16));
                m_MapAuthor.SetText(GuildSaberUtils.GetPlayerNameToFit(p_Beatmap.level.songAuthorName, 14));
                m_MapMapper.SetText($"[{GuildSaberUtils.GetPlayerNameToFit((p_Beatmap.level as CustomBeatmapLevel).levelAuthorName, 12)}]");
                m_MapDuration.SetText(Formatters.SimpleTimeFormat(p_Beatmap.level.songDuration));
                m_BPM.SetText($"{p_Beatmap.level.beatsPerMinute:0.0} BPM");

                Texture2D l_Tex = l_Sprite.texture;
                l_Tex = await TextureUtils.CreateRoundedTexture(l_Tex, l_Tex.width * 0.05f);

                m_MapCover.SetSprite(Sprite.Create(l_Tex, new Rect(0, 0, l_Tex.width, l_Tex.height), new Vector2()));

                var l_CustomBeatmapLevel = (CustomDifficultyBeatmap)p_Beatmap;
                var l_ExtraSongData = SongCore.Collections.RetrieveExtraSongData(p_Hash)._difficulties.Where((x) => x._difficulty == p_Beatmap.difficulty).First();
                if (l_ExtraSongData._difficultyLabel != string.Empty && l_ExtraSongData._difficultyLabel != null)
                {
                    m_DifficultyLabel.SetText(l_ExtraSongData._difficultyLabel);
                } else
                {
                    m_DifficultyLabel.SetText(GSBeatmapUtils.DifficultyToSerialized(p_Beatmap.difficulty));
                }

                m_NPS.SetText(GuildSaberUtils.GetMapNotesPerSecondsAvg((CustomDifficultyBeatmap)p_Beatmap).ToString("0.00"));
                m_Bombs.SetText(l_CustomBeatmapLevel.beatmapSaveData.bombNotes.Count.ToString("0"));
                m_Walls.SetText(l_CustomBeatmapLevel.beatmapSaveData.obstacles.Count.ToString("0"));
                //m_DifficultyLabel.SetText(l_CustomBeatmapLevel.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName + "\n" + GSBeatmapUtils.DifficultyToSerialized(l_CustomBeatmapLevel.difficulty));

                PlaySongPreview();
                SetActive(true);
            });
            return this;
        }

        public void PlayMap()
        {
            PlayerData l_PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;

            OverrideEnvironmentSettings l_OverrideEnvironmentSettings = l_PlayerData.overrideEnvironmentSettings;

            Levels.PlaySong(m_Beatmap.level,
                m_Beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic,
                m_Beatmap.difficulty,
                l_OverrideEnvironmentSettings,
                l_PlayerData.colorSchemesSettings.GetSelectedColorScheme(), l_PlayerData.gameplayModifiers, l_PlayerData.playerSpecificSettings,
                (p_SceneSetupData, p_LevelCompletionResults, p_Beatmap) =>
                {

                    if (MapResultsFlowCoordinator.Instance == null)
                        MapResultsFlowCoordinator.Instance = BeatSaberUI.CreateFlowCoordinator<MapResultsFlowCoordinator>();

                    if (p_LevelCompletionResults.levelEndAction == LevelCompletionResults.LevelEndAction.Quit)
                        return;

                    MapResultsFlowCoordinator.Instance.ShowWithData(p_SceneSetupData, p_LevelCompletionResults, p_Beatmap, l_PlayerData);
                });
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        internal void OnResultsContinueButtonPressed(ResultsViewController p_Controller)
        {
            if (CustomLevelSelectionMenuReferences.IsInGuildSaberLevelSelectionMenu == false) return;

            MapResultsFlowCoordinator.Instance.Dismiss();
        }

        internal void OnResultsRestartButtonPressed(ResultsViewController p_Controller)
        {
            if (CustomLevelSelectionMenuReferences.IsInGuildSaberLevelSelectionMenu == false) return;

            PlayMap();
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void PlaySongPreview()
        {
            m_MapPreviewAudio.FadeOut(0.25f);

            m_MapPreviewAudio.CrossfadeTo(m_Beatmap.level.beatmapLevelData.audioClip, 1, m_Beatmap.level.previewStartTime, m_Beatmap.level.previewDuration, null) ;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        protected void OnPracticeClicked()
        {
            if (PracticeMenuFlowCoordinator.Instance == null)
                PracticeMenuFlowCoordinator.Instance = BeatSaberUI.CreateFlowCoordinator<PracticeMenuFlowCoordinator>();

            PracticeMenuFlowCoordinator.Instance.ShowWithBeatmap(m_Beatmap);
        }
    }
}
