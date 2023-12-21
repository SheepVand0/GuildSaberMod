using CP_SDK.UI.Components;
using CP_SDK.UI.DefaultComponents;
using CP_SDK.XUI;
using GuildSaber.Logger;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components;
using GuildSaber.Utils;
using IPA.Utilities;
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
        public const int MAP_BUTTON_HEIGHT = 20;

        IDifficultyBeatmap m_Beatmap;
        Sprite m_BeatmapCover;

        protected bool m_IsSelected = false;

        public static event Action<IDifficultyBeatmap> eOnMapSelected;

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        protected MapButton(IDifficultyBeatmap p_Beatmap, Action p_OnClick = null) : base("MapButton", string.Empty, p_OnClick)
        {
            SetWidth(45);
            SetHeight(MAP_BUTTON_HEIGHT);
            
            if (p_Beatmap != null)
                SetBeatmap(p_Beatmap);
            
            OnClick(OnClicked);
            OnReady(x =>
            {
                GSText.PatchText(Element.gameObject.GetComponentInChildren<TextMeshProUGUI>());
            });
            eOnMapSelected += (x) => m_IsSelected = false;
        }

        public static MapButton Make()
        {
            return new MapButton(null);
        }

        public static MapButton Make(IDifficultyBeatmap p_Beatmap)
        {
            return new MapButton(p_Beatmap);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public string GetMapName() => m_Beatmap.level.songName;

        public IDifficultyBeatmap GetBeatmap() => m_Beatmap;

        public bool IsSelected() => m_IsSelected;

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public async void SetBeatmap(IDifficultyBeatmap p_Beatmap)
        {
            m_Beatmap = p_Beatmap;
            if (m_Beatmap == null)
            {
                Hide();
                return;
            }

            if (m_Beatmap.level == default(IBeatmapLevel))
            {
                Hide();
                return;
            }

            //GSLogger.Instance.Log(p_Beatmap == null, IPA.Logging.Logger.LogLevel.InfoUp);

            m_BeatmapCover = await m_Beatmap.level.GetCoverImageAsync(new());
            if (m_BeatmapCover == null)
            {
                var l_MapCoverTexture = CustomLevelSelectionMenuReferences.DefaultLogo;
                m_BeatmapCover = Sprite.Create(l_MapCoverTexture, new Rect(0, 0, l_MapCoverTexture.width, l_MapCoverTexture.height),
                    new Vector2());
            }

            Show();
            UpdateVisuals();

            return;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void UpdateVisuals()
        {
            Element.GetComponentInChildren<DefaultCText>().TMProUGUI.richText = true;
            SetText($"{Utils.GuildSaberUtils.GetPlayerNameToFit(m_Beatmap.level.songName, 14)} {Formatters.SimpleTimeFormat(m_Beatmap.level.songDuration)}\n<size=3>{m_Beatmap.level.songAuthorName}");

            Sprite l_MapCover = m_BeatmapCover;

            Texture2D l_Texture = l_MapCover.texture;

            FixedHeight l_FixedHeight = GetHeight(45, MAP_BUTTON_HEIGHT, l_Texture.width, l_Texture.height);

            //Texture2D l_FixedTexture = await AddOffset(l_Texture, l_FixedHeight.TextureOffset);

            Element.SetBackgroundSprite(
                Sprite.Create(
                    l_Texture,
                    new Rect(0, l_FixedHeight.TextureOffset, l_Texture.width, l_FixedHeight.NewHeight),
                    new Vector2())
                );
            Element.SetBackgroundColor(new Color(1f, 1f, 1f, 0.5f));
            Element.BackgroundImageC.type = UnityEngine.UI.Image.Type.Simple;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        private void OnClicked()
        {
            if (m_IsSelected == true)
                return;

            LevelSelectionViewController.Instance.SetSelectedMap(m_Beatmap);
            eOnMapSelected?.Invoke(m_Beatmap);
            m_IsSelected = true;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void Hide()
        {
            SetActive(false);
        }

        public void Show()
        {
            SetActive(true);
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void SetSelected(bool p_IsSelected)
        {
            m_IsSelected = p_IsSelected;
        }
    }
}
