using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using UnityEngine;
using GuildSaber.UI.CustomLevelSelection;
using HMUI;
using GuildSaber.AssetBundles;

namespace GuildSaber.UI.Components
{
    internal class LevelSelectionCustomCategory : CustomUIComponent
    {
        protected override string m_ViewResourceName => "GuildSaber.UI.Components.Views.LevelSelectionCustomCategory.bsml";

        [UIComponent("PlaylistImage")] ClickableImage m_PlaylistImage = null;

        [UIAction("OnClick")] private void OnPointerClick()
        {

        }

        public void Setup(Sprite p_PlaylistCover)
        {
            m_PlaylistImage.GetComponentInChildren<ImageView>();
            m_PlaylistImage.material = AssetBundleLoader.LoadElement<Material>("Mat_AvatarMask");
            m_PlaylistImage.material.SetTexture("_MainTex", p_PlaylistCover.texture);
        }

        public void DestroyCategory()
        {
            foreach (GameObject l_Current in transform.GetComponents<GameObject>())
                GameObject.DestroyImmediate(l_Current.gameObject);
        }
    }
}
