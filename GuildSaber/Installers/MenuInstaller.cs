using GuildSaber.Logger;
using GuildSaber.UI.Leaderboard;
using Zenject;

namespace GuildSaber.Installers
{
    class MenuInstaller : Installer<MenuInstaller>
    {
        public override void InstallBindings()
        {
            if (GuildSaberCustomLeaderboard.Initialized == true) { return; }

            GSLogger.Instance.Log("Installing GuildSaber Bindings", IPA.Logging.Logger.LogLevel.NoticeUp);

            Container.BindInterfacesAndSelfTo<GuildSaberLeaderboardView>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<GuildSaberLeaderboardPanel>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<GuildSaberCustomLeaderboard>().AsSingle();


        }
    }
}
