using CP_SDK.XUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard.Components
{
    internal class LeaderboardPanel : XUIVLayout
    {
        protected LeaderboardPanel(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
        }
    }
}
