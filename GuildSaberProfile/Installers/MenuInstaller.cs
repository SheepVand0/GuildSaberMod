using Zenject;
using GuildSaber.UI.GuildSaber.Leaderboard;
using GuildSaber.Managers;
using UnityEngine;
using GuildSaber.Utils;

namespace GuildSaber.Installers
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
