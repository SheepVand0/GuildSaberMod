using CP_SDK.UI.Components;
using GuildSaber.UI.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.UI.Beta
{
    internal class TestListButton : GSSecondaryButton
    {
        protected TestListButton(string p_Name, string p_Label, Action p_OnClick = null) 
            : base(p_Name, p_Label, 15, 15, p_OnClick)
        {
            OnClick(OnButtonClick);
            OnReady(OnCreation);
        }

        public static TestListButton Make()
        {
            return new TestListButton("GSTestBut", "AH");
        }

        private void OnCreation(CSecondaryButton p_Button)
        {
        }

        private void OnButtonClick()
        {

        }
    }
}
