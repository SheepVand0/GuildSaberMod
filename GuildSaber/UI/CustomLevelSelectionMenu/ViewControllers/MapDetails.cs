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
        protected XUIImage m_MapCover;
        protected GSSecondaryButton m_PracticeButton;
        protected GSSecondaryButton m_PlayButton;

        internal static AudioSource m_MapPreviewAudio;

        public static MapDetails Make()
        {
            /*XUIImage l_Cover = XUIImage.Make();
            l_Cover.SetWidth(20).SetHeight(20);

            XUIText l_MapName = XUIText.Make(string.Empty);
            XUIText l_MapAuthor = XUIText.Make(string.Empty);
            XUIText l_MapDuration = XUIText.Make(string.Empty);

            XUIHLayout l_NamesLayout = XUIHLayout.Make(
                l_Cover,
                XUIVLayout.Make(
                    XUIHLayout.Make(
                            l_MapName,
                            l_MapDuration
                        ),
                    l_MapAuthor
                    )
                )
                .SetHeight(20)
                .SetWidth(50);*/

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
                XUIImage.Make()
                    .SetWidth(20)
                    .SetHeight(20)
                    .SetType(UnityEngine.UI.Image.Type.Simple)
                    .Bind(ref m_MapCover),
                XUIVLayout.Make(
                    XUIHLayout.Make(
                            XUIText.Make(string.Empty)
                                .Bind(ref m_MapName),
                            XUIText.Make(string.Empty)
                                .Bind(ref m_MapDuration)
                        ),
                    XUIText.Make(string.Empty)
                        .Bind(ref m_MapAuthor)
                )
            ).SetWidth(60)
             .SetHeight(20)
             .BuildUI(Element.transform);

            if (m_MapPreviewAudio == null)
                m_MapPreviewAudio = Resources.FindObjectsOfTypeAll<AudioSource>().First();

            XUIHLayout.Make(
                GSSecondaryButton.Make("Practice", 24, 15).OnClick(OnPracticeClicked),
                GSSecondaryButton.Make("Play", 24, 15).OnClick(async () =>
                {
                    //LevelsFlowCoordinator.Instance.Dismiss();
                    //CategorySelectionFlowCoordinator.Instance.Dismiss();
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

        public static string DurationFormat(float p_Duration)
        {
            int l_Minutes = (int)(p_Duration / 60);
            int l_Hours = (int)(p_Duration / (60*60));
            int l_Seconds = (int)p_Duration - (l_Minutes * 60) - (l_Hours * 60);

            string l_SHours = (l_Hours > 0) ? $"{l_Hours:00}:" : string.Empty;

            return $"{l_SHours}{l_Minutes:00}:{l_Seconds:00}";
        }

        public MapDetails SetMap(IDifficultyBeatmap p_Beatmap)
        {
            m_Beatmap = p_Beatmap;
            OnReady(async x =>
            {

                Sprite l_Sprite = (await p_Beatmap.level.GetCoverImageAsync(new CancellationToken()));

                m_MapName.SetText(GuildSaberUtils.GetPlayerNameToFit(p_Beatmap.level.songName, 20));
                m_MapAuthor.SetText(p_Beatmap.level.songAuthorName);
                m_MapDuration.SetText(DurationFormat(p_Beatmap.level.songDuration));

                Texture2D l_Tex = l_Sprite.texture.GetCopy();
                l_Tex = await TextureUtils.CreateRoundedTexture(l_Tex, l_Tex.width * 0.05f);

                m_MapCover.SetSprite(Sprite.Create(l_Tex, new Rect(0, 0, l_Tex.width, l_Tex.height), new Vector2()));
                PlaySongPreview();
                SetActive(true);
                
            });
            return this;
        }

        public void PlaySongPreview()
        {
            m_MapPreviewAudio.Stop();
            m_MapPreviewAudio.clip = m_Beatmap.level.beatmapLevelData.audioClip;
            m_MapPreviewAudio.time = m_Beatmap.level.previewStartTime;
            m_MapPreviewAudio.Play();
            m_MapPreviewAudio.volume = 1;
        }

        protected void OnPracticeClicked()
        {
            if (PracticeMenuFlowCoordinator.Instance == null)
                PracticeMenuFlowCoordinator.Instance = BeatSaberUI.CreateFlowCoordinator<PracticeMenuFlowCoordinator>();
            PracticeMenuFlowCoordinator.Instance.ShowWithBeatmap(m_Beatmap);
        }
    }
}
