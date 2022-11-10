using Zenject;
using GuildSaber.UI.GuildSaber.Leaderboard;
using GuildSaber.Managers;
using UnityEngine;
using GuildSaber.Utils;
using GuildSaber.Logger;

namespace GuildSaber.Installers
{
    class MenuInstaller : Installer<MenuInstaller>
    {
        public override void InstallBindings()
        {
            GSLogger.Instance.Log("Installing GuildSaber Bindings", IPA.Logging.Logger.LogLevel.NoticeUp);
            Container.BindInterfacesAndSelfTo<Events>().AsSingle();

            Container.BindInterfacesAndSelfTo<GuildSaberLeaderboardView>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<GuildSaberLeaderboardPanel>().FromNewComponentAsViewController().AsSingle();
            Container.BindInterfacesAndSelfTo<GuildSaberCustomLeaderboard>().AsSingle();

            Container.BindInterfacesAndSelfTo<LeaderboardManager>().AsSingle();
        }
    }
}
