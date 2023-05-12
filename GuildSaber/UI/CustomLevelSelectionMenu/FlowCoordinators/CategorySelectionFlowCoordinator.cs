using BeatSaberMarkupLanguage;
using CP_SDK.UI;
using GuildSaber.API;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers;
using GuildSaber.UI.FlowCoordinator;
using GuildSaber.Utils;
using HMUI;
using System.Collections.Generic;

namespace GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;

internal class CategorySelectionFlowCoordinator : CP_SDK.UI.FlowCoordinator<CategorySelectionFlowCoordinator>
{
    public CategoriesSelectionViewController m_GuildcategorySelection = UISystem.CreateViewController<CategoriesSelectionViewController>();

    public static CategorySelectionFlowCoordinator Instance;

    List<ApiCategory> m_Categories;
    string m_GuildName;

    public override string Title => "Categories";

    public override void Init()
    {
        base.Init();
        Instance = this;
    }

    public void ShowWithCategories(string p_GuildName, List<ApiCategory> p_Categories)
    {
        m_Categories = p_Categories;
        m_GuildName = p_GuildName;
        Present();
        m_GuildcategorySelection.SetCategories(m_Categories);
        m_GuildcategorySelection.SetGuildName(m_GuildName);
    }

    protected override (IViewController, IViewController, IViewController) GetInitialViewsController()
    {
        return (m_GuildcategorySelection, null, null);
    }
}