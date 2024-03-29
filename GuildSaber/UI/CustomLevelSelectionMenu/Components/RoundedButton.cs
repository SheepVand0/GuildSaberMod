using System;
using System.Linq;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using GuildSaber.AssetBundles;
using GuildSaber.Utils;
using HMUI;
<<<<<<< Updated upstream
using OVR.OpenVR;
=======
using IPA.Utilities;
>>>>>>> Stashed changes
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.UI.CustomLevelSelectionMenu.Components
{
<<<<<<< Updated upstream
    public class RoundedButton : CustomUIComponent
    {
        protected override string ViewResourceName => string.Empty;
=======

    [UIComponent("Description")] private readonly TextMeshProUGUI m_DescriptionText = null;
>>>>>>> Stashed changes

        protected override string GetViewDescription() => "";

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        [UIComponent("MainLayout")] private readonly HorizontalLayoutGroup m_MainLayout = null;
        [UIComponent("Title")] private readonly TextMeshProUGUI m_TitleText = null;
        [UIComponent("Description")] private readonly TextMeshProUGUI m_DescriptionText = null;

<<<<<<< Updated upstream
        private string m_Title = String.Empty;
        private string m_Description = string.Empty;
        private string? m_Image = null;
        private Action m_OnClick = null;

        private Button m_Button = null;
=======
    private string m_Title = string.Empty;
    protected override string ViewResourceName => "GuildSaber.UI.CustomLevelSelectionMenu.Components.Views.RoundedButton.bsml";
>>>>>>> Stashed changes

        protected override async void AfterViewCreation()
        {
            m_Button = BeatSaberPlus.SDK.UI.Button.Create(m_MainLayout.transform, string.Empty, m_OnClick);
            m_TitleText.text = m_Title;
            m_TitleText.fontSize = 4.5f;
            m_DescriptionText.text = m_Description;
            m_DescriptionText.fontSize = 2.5f;

            if (m_Image == null) return;
            try
            {
                Material l_RoundedShader = AssetBundleLoader.LoadElement<Material>("Mat_AvatarMask");
                l_RoundedShader.SetFloat(Shader.PropertyToID("_FadeStart"), 1.48f);
                l_RoundedShader.SetFloat(Shader.PropertyToID("_FadeEnd"), 1.45f);
                Texture l_Texture = await GuildSaberUtils.GetImage(m_Image);
                if (l_Texture == null) return;

                l_RoundedShader.SetTexture(Shader.PropertyToID("_MainTex"), l_Texture);
                m_Button.GetComponentsInChildren<ImageView>().ElementAt(0).material = l_RoundedShader;
                m_Button.GetComponentsInChildren<ImageView>().ElementAt(1).gameObject.SetActive(false);
            }
            catch
            {
                return;
            }
        }

        private void Setup(Action p_OnClick, string p_Text, string p_Description, string? p_Image)
        {
            m_OnClick = p_OnClick;
            m_Title = p_Text;
            m_Description = p_Description;
            m_Image = p_Image;
        }

        public static void Create(Transform p_Transform, string p_Text, string p_Description, Action p_OnClick, string? p_Image)
        {
            RoundedButton l_Button = CreateItem<RoundedButton>(p_Transform, true, false);
            l_Button.Setup(p_OnClick, p_Text, p_Description, p_Image);
            BSMLParser.instance.Parse("<horizontal id=\"MainLayout\"><vertical><text id=\"Title\" /><text id=\"Description\"/></vertical></horizontal>", p_Transform.gameObject, l_Button);
        }

<<<<<<< Updated upstream
=======
    public static void Create(Transform p_Transform, string p_Text, string p_Description, float p_Width, float p_Height, Action p_OnClick, string? p_Image)
    {
        var l_Button = CreateItem<RoundedButton>(p_Transform, true, false, p_Callback: (p_Object) =>
        {
            p_Object.m_MainLayout.SetField("preferredHeight", p_Height);
            p_Object.m_MainLayout.SetField("preferredWidth", p_Width);
        });
        l_Button.Setup(p_OnClick, p_Text, p_Description, p_Image);
>>>>>>> Stashed changes
    }
}
