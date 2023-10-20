using BeatLeader.Replayer;
using GuildSaber.API;
using GuildSaber.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI.CustomLevelSelectionMenu
{
    internal class CustomLevelSelectionMenuReferences
    {

        public static int SelectedGuildId;
        public static bool IsInPractice;
        public static Texture2D DefaultLogo = AssemblyUtils.LoadTextureFromAssembly("GuildSaber.Resources.GuildSaberLogoOrange.png");
        public static ApiCategory SelectedCategory;
        public static bool IsInGuildSaberLevelSelectionMenu;
    }
}
