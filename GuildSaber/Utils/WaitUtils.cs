using System;
using System.Threading.Tasks;

namespace GuildSaber.Utils
{

    internal class WaitUtils
    {
        public static async Task<Task> Wait(Func<bool> p_Func, int p_ToleranceMs, int p_DelayAfter, int p_MaxTryCount)
        {
            int l_TryCount = 0;
            await Task.Run(async delegate
            {
                bool l_ShouldTryCount = p_MaxTryCount > 0;

                while (p_Func.Invoke() == false && (!l_ShouldTryCount || l_TryCount < p_MaxTryCount))
                {
                    l_TryCount += 1;
                    await Task.Delay(p_ToleranceMs);
                }
            });
            if (p_DelayAfter != 0)
                await Task.Delay(p_DelayAfter);
            return Task.CompletedTask;
        }

        public static async Task<Task> Wait(Func<bool> p_Func, int p_ToleranceMs, int p_DelayAfter)
        {
            await Wait(p_Func, p_ToleranceMs, p_DelayAfter, -1);
            return Task.CompletedTask;
        }

        public static async Task<Task> Wait(Func<bool> p_Func, int p_ToleranceMs)
        {
            await Wait(p_Func, p_ToleranceMs, 0);
            return Task.CompletedTask;
        }
    }
}
