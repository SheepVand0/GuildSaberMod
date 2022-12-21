using GuildSaber.Logger;
using GuildSaber.UI.Leaderboard;
using Zenject;

namespace GuildSaber.Installers
{
    internal class MenuInstaller : Installer<MenuInstaller>
    {
        public override void InstallBindings()
        {
            GSLogger.Instance.Log("Installing GuildSaber Bindings", IPA.Logging.Logger.LogLevel.NoticeUp);

            Container.BindInterfacesAndSelfTo<GuildSaberLeaderboardView>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<GuildSaberLeaderboardPanel>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<GuildSaberCustomLeaderboard>().AsSingle();
        }
    }
}