using UnityEngine;
using GuildSaber.AssetBundles;
using GuildSaber.UI.GuildSaber.Leaderboard;
using UnityEngine.UI;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using GuildSaber.API;

namespace GuildSaber.UI.GuildSaber.Components
{
    class PlayerAvatar : CustomUIComponent
    {
        #region Defaults
        protected override string m_ViewResourceName => "GuildSaber.UI.Components.Views.PlayerAvatar.bsml";

        private Material _PlayerAvatarMaskInstance;

        [UIComponent("AvatarImage")] private ImageView m_Avatar = null;
        [UIComponent("AvatarGrid")] private GridLayoutGroup m_AvatarGrid = null;
        #endregion

        #region Setup
        public void Setup(string p_AvatarLink, Color p_ProfileColor)
        {
            m_Avatar.SetImage(p_AvatarLink);

            UpdateShader(p_ProfileColor);
        }
        #endregion

        #region Update
        public void UpdateShader(Color p_ProfileColor)
        {
            if (m_Avatar == null) { Plugin.Log.Error("Avatar Is Null"); return; }

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
        #endregion

        #region Events
        protected override void AfterViewCreation()
        {
            ApiPlayerData l_Player = GuildSaberLeaderboardPanel.Instance.m_PlayerData;
            if (string.IsNullOrEmpty(l_Player.Avatar)) return;

            m_AvatarGrid.cellSize = new Vector2(17, 17);
            Setup(l_Player.Avatar, l_Player.Color.ToUnityColor32());
        }
        #endregion
    }
}
