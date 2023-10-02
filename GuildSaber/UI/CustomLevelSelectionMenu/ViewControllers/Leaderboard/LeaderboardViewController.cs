using BeatSaberPlus.SDK.UI;
using CP_SDK.XUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.UI.CustomLevelSelectionMenu.ViewControllers.Leaderboard
{
    internal class LeaderboardViewController : ViewController<LeaderboardViewController>
    {
        protected XUIHLayout m_LeaderboardPanelContainer;

        protected override void OnViewCreation()
        {
            Templates.FullRectLayout(
                XUIHLayout.Make(
                    
                    ).Bind(ref m_LeaderboardPanelContainer),
                XUIHLayout.Make(
                    
                    )
                ).BuildUI(transform);
        }



    }
}
