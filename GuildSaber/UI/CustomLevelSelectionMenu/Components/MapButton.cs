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

        public string GetMapName() => m_Beatmap.SerializedName();

        public static MapButton Make(IDifficultyBeatmap p_Beatmap)
        {
            return new MapButton(p_Beatmap);
        }

        public void SetBeatmap(IDifficultyBeatmap p_Beatmap)
        {
            m_Beatmap = p_Beatmap;
            OnReady(x =>
            {
                UpdateVisuals();
            });
        }

        public async void UpdateVisuals()
        {
            Element.GetComponentInChildren<DefaultCText>().TMProUGUI.richText = true;
            SetText($"{Utils.GuildSaberUtils.GetPlayerNameToFit(m_Beatmap.level.songName, 16)} {MapDetails.DurationFormat(m_Beatmap.level.songDuration)}\n<size=3>{m_Beatmap.level.songAuthorName}");

            Sprite l_MapCover = await m_Beatmap.level.GetCoverImageAsync(new System.Threading.CancellationToken());

            Texture2D l_Texture = l_MapCover.texture.GetCopy();

            int l_FixedHeight = l_Texture.width / (4);

            float l_Radius = (l_Texture.width * 0.05f);
            Texture2D l_FixedTexture = CreateRoundedTexture(l_Texture, l_Radius, (int)(l_FixedHeight / 1.5f));

            Element.SetBackgroundSprite(
                Sprite.Create(
                    l_FixedTexture,
                    new Rect(0, l_FixedHeight / (l_Texture.width / (l_Texture.height / 0.9f)), l_Texture.width, l_FixedHeight),
                    new Vector2())
                );
            Element.SetBackgroundColor(new Color(1f, 1f, 1f));
        }

        private void OnClicked()
        {
            LevelSelectionViewController.Instance.SetSelectedMap(m_Beatmap);
        }
    }
}
