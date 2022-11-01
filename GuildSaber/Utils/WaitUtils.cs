using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace GuildSaber.Utils
{

    internal class WaitUtils
    {
        public async static Task<Task> WaitUntil(Func<bool> p_Func, int p_ToleranceMs, int p_DelayAfter, int p_MaxTryCount)
        {
            int l_TryCount = p_MaxTryCount;
            await Task.Run(async delegate
            {
                while (p_Func() == false)
                {
                    if (l_TryCount != 0)
                    l_TryCount -= 1;
                    await Task.Delay(p_ToleranceMs);
                }
            });
            if (p_DelayAfter != 0)
                await Task.Delay(p_DelayAfter);
            return Task.CompletedTask;
        }

        public async static Task<Task> WaitUntil(Func<bool> p_Func, int p_ToleranceMs, int p_DelayAfter)
        {
            await WaitUntil(p_Func, p_ToleranceMs, p_DelayAfter, -1);
            return Task.CompletedTask;
        }

        public async static Task<Task> WaitUntil(Func<bool> p_Func, int p_ToleranceMs)
        {
            await WaitUntil(p_Func, p_ToleranceMs, 0);
            return Task.CompletedTask;
        }
    }
}
