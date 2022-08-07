namespace GuildSaberProfile.UI.GuildSaber;

public interface IRefresh
{
    public void RefreshCard();

    public void RefreshColors();
}

public class Refresher : IRefresh
{
    void IRefresh.RefreshCard()
    {
        Plugin.DestroyCard();
        Plugin.CreateCard();
    }

    async void IRefresh.RefreshColors()
    {
        await Plugin.PlayerCard.CardViewController.UpdateCardColor();
        Plugin.PlayerCard.CardViewController.m_SettingsModal.UpdateShowColors();
    }
}
