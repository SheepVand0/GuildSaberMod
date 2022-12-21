using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberPlus.SDK.UI;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers
{
    public class CategoriesSelectionViewController : ViewController<CategoriesSelectionViewController>
    {

        protected override string GetViewContentDescription()
        {
            return Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), $"{GuildSelectionMenu.VIEW_CONTROLLERS_PATH}.CategorySelectionViewController.bsml");
        }
    }
}
