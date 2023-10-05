using CP_SDK.UI.Components;
using CP_SDK.UI.DefaultComponents;
using CP_SDK.XUI;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static GuildSaber.Utils.TextureUtils;

namespace GuildSaber.UI.CustomLevelSelectionMenu.Components
{
    internal class MapButton : XUISecondaryButton
    {
        IDifficultyBeatmap m_Beatmap;
        Sprite m_BeatmapCover;

        public static event Action<IDifficultyBeatmap> eOnMapSelected;

        protected MapButton(IDifficultyBeatmap p_Beatmap, Action p_OnClick = null) : base("MapButton", string.Empty, p_OnClick)
        {
            SetWidth(45);
            SetHeight(20);
            OnReady(x =>
            {
                SetBeatmap(p_Beatmap);
                UpdateVisuals();
            });
            OnClick(OnClicked);
        }

        public string GetMapName() => m_Beatmap.level.songName;

        public static MapButton Make(IDifficultyBeatmap p_Beatmap)
        {
            return new MapButton(p_Beatmap);
        }

        public void SetBeatmap(IDifficultyBeatmap p_Beatmap)
        {
            m_Beatmap = p_Beatmap;
            OnReady(async x =>
            {
                m_BeatmapCover = await m_Beatmap.level.GetCoverImageAsync(new System.Threading.CancellationToken());
                UpdateVisuals();
            });
        }

        public async void UpdateVisuals()
        {
            Element.GetComponentInChildren<DefaultCText>().TMProUGUI.richText = true;
            SetText($"{Utils.GuildSaberUtils.GetPlayerNameToFit(m_Beatmap.level.songName, 16)} {MapDetails.DurationFormat(m_Beatmap.level.songDuration)}\n<size=3>{m_Beatmap.level.songAuthorName}");

            Sprite l_MapCover = m_BeatmapCover;

            if (l_MapCover == null)
            {
                var l_MapCoverTexture = CustomLevelSelectionMenuReferences.DefaultLogo;
                l_MapCover = Sprite.Create(l_MapCoverTexture, new Rect(0, 0, l_MapCoverTexture.width, l_MapCoverTexture.height), new Vector2());
            }

            Texture2D l_Texture = l_MapCover.texture;

            //int l_FixedHeight = l_Texture.width / (4);
            TextureUtils.FixedHeight l_FixedHeight = TextureUtils.GetHeight(45, 20, l_Texture.width, l_Texture.height);

            float l_Radius = (l_Texture.width * 0.05f);
            Texture2D l_FixedTexture = await AddOffset(l_Texture, l_FixedHeight.TextureOffset);

            Element.SetBackgroundSprite(
                Sprite.Create(
                    l_FixedTexture,
                    new Rect(0, 0, l_Texture.width, l_FixedTexture.height),
                    new Vector2())
                );
            Element.SetBackgroundColor(new Color(1f, 1f, 1f, 0.5f));
        }

        private void OnClicked()
        {
            LevelSelectionViewController.Instance.SetSelectedMap(m_Beatmap);
            eOnMapSelected?.Invoke(m_Beatmap);
        }
    }
}
