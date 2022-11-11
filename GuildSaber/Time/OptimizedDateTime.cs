using System.Collections.Generic;

namespace GuildSaber.Time;

public class OptimizedDateTime
{
    private readonly List<int> m_DaysInMonth = new List<int>
    {
        31,
        28,
        31,
        30,
        31,
        30,
        31,
        31,
        30,
        31,
        30,
        31
    };
    public int Day;
    public int Hours;
    public int Minutes;
    public int Month;
    public float Seconds;
    public int Year;

    /// <summary>
    /// Init
    /// </summary>
    /// <param name="p_Year"></param>
    /// <param name="p_Month"></param>
    /// <param name="p_Day"></param>
    /// <param name="p_Hour"></param>
    /// <param name="p_Minutes"></param>
    /// <param name="p_Seconds"></param>
    public void Init(int p_Year, int p_Month, int p_Day, int p_Hour, int p_Minutes, int p_Seconds)
    {
        Year = p_Year;
        Month = p_Month;
        Day = p_Day;
        Hours = p_Hour;
        Minutes = p_Minutes;
        Seconds = p_Seconds;
    }

    /// <summary>
    /// Add Second and update
    /// </summary>
    /// <param name="p_Value"></param>
    /// <returns></returns>
    public bool AddSecondAndUpdateClock(float p_Value)
    {
        bool l_ShouldUpdate = Seconds + p_Value > (int)Seconds + 1;

        Seconds += p_Value;

        if (!(Seconds >= 60)) return l_ShouldUpdate;

        Minutes += (int)(Seconds / 60);
        Seconds = 0;

        if (!(Minutes >= 60))
            return l_ShouldUpdate;

        Hours += Minutes / 60;
        Minutes = 0;

        if (!(Hours >= 24))
            return l_ShouldUpdate;

        Day += Hours / 24;
        Hours = 0;

        if (!(Day >= GetCurrentMonthDayCount()))
            return l_ShouldUpdate;

        Month += Day / GetCurrentMonthDayCount();
        Day = 0;

        if (!(Month >= 12))
            return l_ShouldUpdate;

        Year += Month / 12;
        Month = 0;

        return l_ShouldUpdate;
    }

    /// <summary>
    /// Get the day count in current month
    /// </summary>
    /// <returns></returns>
    private int GetCurrentMonthDayCount()
    {
        return m_DaysInMonth[Month];
    }
}