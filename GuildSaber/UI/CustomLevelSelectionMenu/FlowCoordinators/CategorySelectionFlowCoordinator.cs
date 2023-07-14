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
    int m_GuildId;

    public override string Title => "Categories";

    public override void Init()
    {
        base.Init();
        Instance = this;
    }

    public void ShowWithCategories(int p_GuildId, List<ApiCategory> p_Categories)
    {
        m_Categories = p_Categories;
        m_GuildId = p_GuildId;
        Present();
        m_GuildcategorySelection.SetGuildId(p_GuildId);
        m_GuildcategorySelection.SetCategories(m_Categories);
    }

    protected override (IViewController, IViewController, IViewController) GetInitialViewsController()
    {
        return (m_GuildcategorySelection, null, null);
    }
}