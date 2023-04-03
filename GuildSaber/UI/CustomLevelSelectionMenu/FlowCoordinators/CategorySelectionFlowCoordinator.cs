using BeatSaberMarkupLanguage;
using GuildSaber.API;
using GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers;
using GuildSaber.UI.FlowCoordinator;
using GuildSaber.Utils;
using HMUI;
using System.Collections.Generic;

namespace GuildSaber.UI.CustomLevelSelectionMenu.FlowCoordinators;

internal class CategorySelectionFlowCoordinator : CustomFlowCoordinator
{
    public CategoriesSelectionViewController m_GuildcategorySelection;

    public static CategorySelectionFlowCoordinator Instance;

    protected override string Title
    {
        get => "Categories";
    }

    protected override (ViewController?, ViewController?, ViewController?) GetUIImplementation()
    {
        if (m_GuildcategorySelection == null) m_GuildcategorySelection = BeatSaberUI.CreateViewController<CategoriesSelectionViewController>();

        return (m_GuildcategorySelection, null, null);
    }

    List<ApiCategory> m_Categories;

    public void ShowWithCategories(List<ApiCategory> p_Categories)
    {
        m_Categories = p_Categories;
        Show();
    }

    protected override void TransitionDidFinish()
    {
        base.TransitionDidFinish();

        if (!gameObject.activeInHierarchy) return;

        m_GuildcategorySelection.SetCategories(m_Categories);
    }

}