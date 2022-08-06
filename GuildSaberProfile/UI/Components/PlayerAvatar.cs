using UnityEngine;
using GuildSaberProfile.AssetBundles;
using GuildSaberProfile.UI.GuildSaber.Leaderboard;
using UnityEngine.UI;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;

namespace GuildSaberProfile.UI.GuildSaber.Components
{
    class PlayerAvatar : CustomUIComponent
    {
        #region Defaults
        protected override string m_ViewResourceName { get => "GuildSaberProfile.UI.Components.Views.PlayerAvatar.bsml"; }

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
            if (m_Avatar == null) { Plugin.Log.Info("Is Null"); return; }

            //_PlayerAvatarMaskInstance = Object.Instantiate(AssetBundleLoader.LoadElement<Material>("Mat_AvatarMask"));
            _PlayerAvatarMaskInstance = AssetBundleLoader.LoadElement<Material>("Mat_AvatarMask");
            Texture l_PlayerAvatar = m_Avatar.material.mainTexture;

            m_Avatar.material = _PlayerAvatarMaskInstance;

            _PlayerAvatarMaskInstance.SetTexture(Shader.PropertyToID("_MainTex"), l_PlayerAvatar);
            _PlayerAvatarMaskInstance.SetColor(Shader.PropertyToID("_HaloColor"),p_ProfileColor);
        }
        #endregion

        #region Events
        [UIAction("#post-parse")] private void PostParse()
        {
            GuildSaberLeaderboardPanel l_Panel = Resources.FindObjectsOfTypeAll<GuildSaberLeaderboardPanel>()[0];
            PlayerGuildsInfo l_Player = l_Panel.m_PlayerGuildsInfo;
            if (string.IsNullOrEmpty(l_Player.m_ReturnPlayer.ProfilePicture)) return;

            Setup(l_Player.m_ReturnPlayer.ProfilePicture, l_Player.m_ReturnPlayer.ProfileColor.ToUnityColor());
        }

        public override void OnCreate()
        {
        }

        public override void PostCreate()
        {
            m_AvatarGrid.cellSize = new Vector2(17, 17);
        }
        #endregion
    }
}
