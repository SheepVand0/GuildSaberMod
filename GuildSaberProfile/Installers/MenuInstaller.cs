using Zenject;
using GuildSaberProfile.UI.GuildSaber.Leaderboard;
using GuildSaberProfile.Managers;
using UnityEngine;
using GuildSaberProfile.Utils;

namespace GuildSaberProfile.Installers
{
    class MenuInstaller : Installer<MenuInstaller>
    {
        public override void InstallBindings()
        {
            Plugin.Log.Notice("Installing GuildSaber Bindings");
            Container.BindInterfacesAndSelfTo<Events>().AsSingle();

            Container.BindInterfacesAndSelfTo<GuildSaberLeaderboardView>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<GuildSaberLeaderboardPanel>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<GuildSaberCustomLeaderboard>().AsSingle();

            Container.BindInterfacesAndSelfTo<LeaderboardManager>().AsSingle();
        }
    }
}
