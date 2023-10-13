using BeatSaberMarkupLanguage;
using BeatSaberPlus.SDK.Game;
using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
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

        protected XUIText m_MapName;
        protected XUIText m_MapAuthor;
        protected XUIText m_MapDuration;
        protected XUIText m_Modifiers;
        protected XUIText m_MapMapper;
        protected XUIImage m_MapCover;
        protected GSSecondaryButton m_PracticeButton;
        protected GSSecondaryButton m_PlayButton;

        internal static SongPreviewPlayer m_MapPreviewAudio;

        internal static PlayerData m_PlayerData;

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

        private void OnLayoutReady(CVLayout p_Layout)
        {

            XUIHLayout.Make(
                XUIVLayout.Make(
                XUIImage.Make()
                    //.SetType(UnityEngine.UI.Image.Type.Simple)
                    .Bind(ref m_MapCover)
                ).SetWidth(20)
                    .SetHeight(20),
                XUIVLayout.Make(
                    XUIHLayout.Make(
                            XUIText.Make(string.Empty)
                                .SetAlign(TMPro.TextAlignmentOptions.Left)
                                .Bind(ref m_MapName),
                            XUIText.Make(string.Empty)
                                .SetAlign(TMPro.TextAlignmentOptions.Right)
                                .Bind(ref m_MapDuration)
                     ).OnReady(x => x.CSizeFitter.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained),
                    XUIHLayout.Make(
                        XUIText.Make(string.Empty)
                            .Bind(ref m_MapAuthor),
                        XUIText.Make(string.Empty)
                            .SetColor(new Color(0, 0.8f, 1, 0.7f))
                            .Bind(ref m_MapMapper)
                    ).ForEachDirect<XUIText>((x) => x.SetFontSize(2))
                ).SetWidth(40)
            ).SetWidth(60)
             .SetHeight(20)
             .BuildUI(Element.transform);

            if (m_MapPreviewAudio == null)
                m_MapPreviewAudio = Resources.FindObjectsOfTypeAll<SongPreviewPlayer>().First();

            XUIHLayout.Make(
                GSSecondaryButton.Make("Practice", 24, 15).OnClick(OnPracticeClicked),
                GSSecondaryButton.Make("Play", 24, 15).OnClick(async () =>
                {
                    PlayerData l_PlayerData = Resources.FindObjectsOfTypeAll<PlayerDataModel>().First().playerData;
                    await Task.Delay(500);
                    OverrideEnvironmentSettings l_OverrideEnvironmentSettings = l_PlayerData.overrideEnvironmentSettings;
                    
                    Levels.PlaySong(m_Beatmap.level, 
                        m_Beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic, 
                        m_Beatmap.difficulty,
                        l_OverrideEnvironmentSettings,
                        l_PlayerData.colorSchemesSettings.GetSelectedColorScheme(), l_PlayerData.gameplayModifiers, l_PlayerData.playerSpecificSettings);
                })
            ).BuildUI(Element.transform);

            SetActive(false);
        }

        protected IDifficultyBeatmap m_Beatmap = null;

        

        public MapDetails SetMap(IDifficultyBeatmap p_Beatmap)
        {
            m_Beatmap = p_Beatmap;
            OnReady(async x =>
            {
                Sprite l_Sprite = await p_Beatmap.level.GetCoverImageAsync(new CancellationToken());

                m_MapName.SetText(GuildSaberUtils.GetPlayerNameToFit(p_Beatmap.level.songName, 16));
                m_MapAuthor.SetText(GuildSaberUtils.GetPlayerNameToFit(p_Beatmap.level.songAuthorName, 14));
                m_MapMapper.SetText($"[{GuildSaberUtils.GetPlayerNameToFit((p_Beatmap.level as CustomBeatmapLevel).levelAuthorName, 12)}]");
                m_MapDuration.SetText(Formatters.SimpleTimeFormat(p_Beatmap.level.songDuration));

                Texture2D l_Tex = l_Sprite.texture;
                l_Tex = await TextureUtils.CreateRoundedTexture(l_Tex, l_Tex.width * 0.05f);

                m_MapCover.SetSprite(Sprite.Create(l_Tex, new Rect(0, 0, l_Tex.width, l_Tex.height), new Vector2()));
                PlaySongPreview();
                SetActive(true);
            });
            return this;
        }

        public void PlaySongPreview()
        {
            m_MapPreviewAudio.FadeOut(0.25f);

            m_MapPreviewAudio.CrossfadeTo(m_Beatmap.level.beatmapLevelData.audioClip, 1, m_Beatmap.level.previewStartTime, m_Beatmap.level.previewDuration, null) ;
        }

        protected void OnPracticeClicked()
        {
            if (PracticeMenuFlowCoordinator.Instance == null)
                PracticeMenuFlowCoordinator.Instance = BeatSaberUI.CreateFlowCoordinator<PracticeMenuFlowCoordinator>();
            PracticeMenuFlowCoordinator.Instance.ShowWithBeatmap(m_Beatmap);
        }
    }
}
