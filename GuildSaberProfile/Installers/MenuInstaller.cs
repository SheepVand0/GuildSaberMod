using Zenject;
using GuildSaberProfile.UI.GuildSaber.Leaderboard;

namespace GuildSaberProfile.Installers
{
    class MenuInstaller : Installer<MenuInstaller>
    {
        public override void InstallBindings()
        {
            Plugin.Log.Notice("Installing leaderboard Bindings (GuildSaber)");
            Container.BindInterfacesAndSelfTo<GuildSaberLeaderboardView>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<GuildSaberLeaderboardPanel>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<GuildSaberCustomLeaderboard>().AsSingle();
        }
    }
}
