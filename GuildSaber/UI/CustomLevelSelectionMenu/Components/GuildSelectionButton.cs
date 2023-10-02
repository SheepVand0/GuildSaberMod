using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using CP_SDK.Chat.Models;
using CP_SDK.UI.Components;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.Logger;
using GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;
using GuildSaber.Utils;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu.Components;

public class GuildSelectionButton : CP_SDK.XUI.XUIPrimaryButton
{
    private GuildData m_GuildData = default;

    public static Texture2D DefaultLogo = AssemblyUtils.LoadTextureFromAssembly("GuildSaber.Resources.GuildSaberLogoOrange.png");

    public GuildSelectionButton(GuildData p_GuildData) : base("GuildSelectionButton", string.Empty, null)
    {
        m_GuildData = p_GuildData;
        OnClick(OnButtonClicked);
        OnReady(OnButtonReady);
    }

    private async void OnButtonReady(CPrimaryButton p_Button)
    {
        SetWidth(80);
        SetHeight(20);
        await UpdateTexture();

        if (m_GuildData.Equals(default))
        {
            SetText("GuildName not defined");
            return;
        }

        int l_Counter = 0;
        string l_Result = string.Empty;
        for (int l_i = 0; l_i < m_GuildData.Description.Count();l_i++)
        {
            if (l_Counter == 50) {
                l_Counter = 0;
                char l_CurrentChar = m_GuildData.Description[l_i];
                if (l_CurrentChar == ' ')
                    l_Result += l_CurrentChar + "\n";
                else
                    l_Result += l_CurrentChar + "-\n";

                continue;
            }

            l_Result += m_GuildData.Description[l_i];
            l_Counter++;
        }
        SetText(m_GuildData.Name);
        SetTooltip(l_Result);
    }

    public async void SetGuildData(GuildData p_GuildData)
    {
        m_GuildData = p_GuildData;
        await UpdateTexture();
    }

    public async Task UpdateTexture(bool p_WithDefaultLogo = false)
    {
        if (m_GuildData.Equals(default)) return;
        try
        {
            GuildSaberUtils.ImageResult l_TextureResult = await GuildSaberUtils.GetImage(m_GuildData.Banner);
            if (l_TextureResult.IsError || p_WithDefaultLogo) l_TextureResult.Texture = DefaultLogo;

            Texture2D l_Texture = l_TextureResult.Texture;

            int l_FixedHeight = l_Texture.width / (80 / 20);

            int l_Radius = (int)(l_Texture.width * 0.01f);
            Texture2D l_FixedTexture = TextureUtils.CreateRoundedTexture(l_Texture, l_Radius, (int)(l_FixedHeight / 1.5f));

            Element.SetBackgroundSprite(
                Sprite.Create(
                    l_FixedTexture,
                    new Rect(0, l_FixedHeight / (l_Texture.width / (l_Texture.height / 0.9f)), l_Texture.width, l_FixedHeight),
                    new Vector2())
                );
            Element.SetBackgroundColor(new UnityEngine.Color(0.4f, 0.4f, 0.4f));
        } catch
        {
            await UpdateTexture(true);
        }
    }

    private async void OnButtonClicked()
    {
        if (m_GuildData.Equals(default)) return;

        try
        {
            string l_Serialized = await new HttpClient().GetStringAsync($"https://api.guildsaber.com/categories/data/all?guild-id={m_GuildData.ID}");

            List<ApiCategory> l_Categories = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ApiCategory>>(l_Serialized);

            if (CategorySelectionFlowCoordinator.Instance == null)
                CategorySelectionFlowCoordinator.Instance = BeatSaberUI.CreateFlowCoordinator<CategorySelectionFlowCoordinator>();

            GSLogger.Instance.Log(m_GuildData.Name, IPA.Logging.Logger.LogLevel.InfoUp);
            CategorySelectionFlowCoordinator.Instance.ShowWithCategories(m_GuildData.ID, l_Categories);

        } catch
        {

        }
    }
}