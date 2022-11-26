using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberPlus.SDK.UI;

// ReSharper disable once CheckNamespace
namespace GuildSaber.UI.CustomLevelSelectionMenu
{
    public class CategoriesSelectionViewController : ViewController<CategoriesSelectionViewController>
    {

        protected override string GetViewContentDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), $"{GuildSelectionMenu.VIEW_CONTROLLERS_PATH}.CategorySelectionViewController.bsml");
        }
    }
}
