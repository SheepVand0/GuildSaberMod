using BeatSaberPlus.SDK.UI;
using CP_SDK.XUI;
using GuildSaber.UI.Beta;
using GuildSaber.UI.Defaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.UI
{
    internal class GuildSaberBetaViewController : ViewController<GuildSaberBetaViewController>
    {

        protected override void OnViewCreation()
        {
            List<int> l_Items = new List<int>();

            while (l_Items.Count() != 10)
            {
                l_Items.Add(UnityEngine.Random.Range(0, 100));
            }

            XUIVLayout.Make(
                GSHorizontalList<int, TestListButton>.Make(l_Items, (x, y) => {
                    x.SetActive(y);
                },
                (x, y) =>
                {
                    y.SetText(x.ToString("0"));
                },(x) =>
                {
                    var l_Item = TestListButton.Make();
                    l_Item.BuildUI(x);
                    return l_Item;
                },
                100, 20
                )
            ).BuildUI(transform);
        }

    }
}
