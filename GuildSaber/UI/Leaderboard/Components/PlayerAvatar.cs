using System;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.API;
using GuildSaber.AssetBundles;
using GuildSaber.Logger;
using HMUI;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.UI.Leaderboard.Components
{
    class PlayerAvatar : CustomUIComponent
    {

        protected override string ViewResourceName => "GuildSaber.UI.Leaderboard.Components.Views.PlayerAvatar.bsml";

        private Material _PlayerAvatarMaskInstance = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("AvatarImage")] private ImageView m_Avatar = null;
        [UIComponent("AvatarGrid")] private GridLayoutGroup m_AvatarGrid = null;

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// After View Creation
        /// </summary>
        protected override void AfterViewCreation()
        {
            ApiPlayerData l_Player = GuildSaberLeaderboardPanel.PanelInstance.m_PlayerData;
            if (string.IsNullOrEmpty(l_Player.Avatar)) return;

            m_AvatarGrid.cellSize = new Vector2(17, 17);
            Setup(l_Player.Avatar, l_Player.Color.ToUnityColor32());
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set defaults
        /// </summary>
        /// <param name="p_AvatarLink"></param>
        /// <param name="p_ProfileColor"></param>
        public void Setup(string p_AvatarLink, Color p_ProfileColor)
        {
            m_Avatar.SetImage(p_AvatarLink);

            UpdateShader(p_ProfileColor);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Update profile color
        /// </summary>
        /// <param name="p_ProfileColor"></param>
        public void UpdateShader(Color p_ProfileColor)
        {
            if (m_Avatar == null) { GSLogger.Instance.Error(new Exception("Avatar Is Null"), nameof(PlayerAvatar), nameof(UpdateShader)); return; }

            //_PlayerAvatarMaskInstance = Object.Instantiate(AssetBundleLoader.LoadElement<Material>("Mat_AvatarMask"));

            Material l_Mat = AssetBundleLoader.LoadElement<Material>("Mat_AvatarMask");
            _PlayerAvatarMaskInstance = l_Mat;

            Texture l_PlayerAvatar = m_Avatar.material.mainTexture;

            m_Avatar.material = _PlayerAvatarMaskInstance;

            _PlayerAvatarMaskInstance.SetTexture(Shader.PropertyToID("_MainTex"), l_PlayerAvatar);
            _PlayerAvatarMaskInstance.SetColor(Shader.PropertyToID("_HaloColor"), p_ProfileColor);
            _PlayerAvatarMaskInstance.SetFloat(Shader.PropertyToID("_HaloIntensity"), 0.0f);
            _PlayerAvatarMaskInstance.SetFloat(Shader.PropertyToID("_Scale"), 0.8f);
            _PlayerAvatarMaskInstance.SetFloat(Shader.PropertyToID("_FadeStart"), 1f);
            _PlayerAvatarMaskInstance.SetFloat(Shader.PropertyToID("_FadeEnd"), 0.97f);
        }
    }
}
