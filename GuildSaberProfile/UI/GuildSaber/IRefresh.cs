namespace GuildSaberProfile.UI.GuildSaber;

public interface IRefresh
{
    public void RefreshCard();
}

public class Refresher : IRefresh
{

    void IRefresh.RefreshCard()
    {
        Plugin.DestroyCard();
        Plugin.CreateCard();
    }
}
