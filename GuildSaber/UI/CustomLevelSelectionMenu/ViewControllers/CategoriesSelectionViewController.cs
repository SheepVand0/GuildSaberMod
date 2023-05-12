using CP_SDK.UI;
using CP_SDK.XUI;
using GuildSaber.API;
using GuildSaber.UI.CustomLevelSelectionMenu.Components;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;


namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers;

public class CategoriesSelectionViewController : ViewController<CategoriesSelectionViewController>
{

    private string m_GuildName = string.Empty;

    private XUIGLayout ButtonsGrid = null;

    private List<CategoryButton> ExistingButtons = new List<CategoryButton>();

    protected override void OnViewCreation()
    {
        Templates.FullRectLayout(
            XUIGLayout.Make(
                )
            .SetWidth(85)
            .SetHeight(65)
            .SetCellSize(new UnityEngine.Vector2(22, 22))
            .SetConstraint(UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount)
            .SetConstraintCount(3)
            .Bind(ref ButtonsGrid)
        ).BuildUI(transform);
    }

    public void SetCategories(List<ApiCategory> p_Categories)
    {
        foreach (var l_Index in ExistingButtons)
            l_Index.Element.gameObject.SetActive(false);

        for (int l_i = 0; l_i < p_Categories.Count;l_i++)
        {
            if (l_i > ExistingButtons.Count - 1)
            {
                var l_Element = CategoryButton.Make(p_Categories[l_i]);
                l_Element.BuildUI(ButtonsGrid.Element.transform);
                ExistingButtons.Add(l_Element);
            } else
            {
                ExistingButtons[l_i].SetCategoryData(p_Categories[l_i]);
                ExistingButtons[l_i].Element.gameObject.SetActive(true);
            }
            ExistingButtons[l_i].SetGuildName(m_GuildName);
        }

    }

    public void SetGuildName(string p_GuildName)
    {
        m_GuildName = p_GuildName;
    }

}