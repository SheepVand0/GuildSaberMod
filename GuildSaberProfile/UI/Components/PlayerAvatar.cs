using UnityEngine;
using GuildSaberProfile.AssetBundles;
using UnityEngine.UI;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;

namespace GuildSaberProfile.UI.GuildSaber.Components
{
    sealed class PlayerAvatar : CustomUIComponent
    {
        #region Defaults
        protected override string m_ViewResourceName { get => "GuildSaberProfile.UI.GuildSaber.View.PlayerAvatar.bsml"; }

        private Material _PlayerAvatarMaskInstance;

        private static readonly int AvatarTexturePropertyId = Shader.PropertyToID("_AvatarTexture");
        private static readonly int FadeValuePropertyId = Shader.PropertyToID("_FadeValue");

        [UIComponent("AvatarImage")] private Image m_Avatar = null;
        #endregion

        #region Setup
        public void Setup(string p_AvatarLink)
        {
            m_Avatar.SetImage("GuildSaberProfile.Resources.BSCCIconBlue.png");
            UpdateShader();
        }
        #endregion

        #region Update
        public void UpdateShader()
        {
            if (m_Avatar == null) return;

            _PlayerAvatarMaskInstance = Object.Instantiate(AssetBundleLoader.LoadElement<Material>("Mat_AvatarMask"));
            //_PlayerAvatarMaskInstance = Object.Instantiate(AssetBundleLoader.LoadElement<Material>("PlayerAvatarMaterial"));

            Texture l_PlayerAvatar = m_Avatar.sprite.texture;

            m_Avatar.material = _PlayerAvatarMaskInstance;

            //_PlayerAvatarMaskInstance.SetFloat(FadeValuePropertyId, 1);
            _PlayerAvatarMaskInstance.SetTexture(Shader.PropertyToID("_MainTex"), l_PlayerAvatar);
            _PlayerAvatarMaskInstance.SetColor(Shader.PropertyToID("_HaloColor"),Plugin.GetPlayerInfoFromCurrent().m_ReturnPlayer.ProfileColor.ToUnityColor());
            //_PlayerAvatarMaskInstance.SetTexture(AvatarTexturePropertyId, Utilities.FindTextureInAssembly("GuildSaberProfile.Resources.BSCCIconBlue.png"));
        }
        #endregion

        #region Events
        [UIAction("#post-parse")] private void PostParse()
        {
            PlayerGuildsInfo l_Player = Plugin.GetPlayerInfoFromCurrent();
            if (string.IsNullOrEmpty(l_Player.m_ReturnPlayer.ProfilePicture)) return;

            Setup(l_Player.m_ReturnPlayer.ProfilePicture);
        }
        #endregion
    }
}
