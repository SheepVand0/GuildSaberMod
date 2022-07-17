using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaberProfile.UI.GuildSaber;

public interface IRefresh
{
    public void RefreshCard();
}

public class Refresher : IRefresh
{
    public Refresher()
    {

    }

    void IRefresh.RefreshCard()
    {
        Plugin.DestroyCard();
        Plugin.CreateCard();
    }
}

