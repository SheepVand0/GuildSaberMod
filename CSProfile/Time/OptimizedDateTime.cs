using System.Collections.Generic;

namespace CSProfile.Time;

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
<<<<<<< Updated upstream
    public int m_Day;
    public int m_Hours;
    public int m_Minutes;
    public int m_Month;
    public float m_Seconds;
    public int m_Year;

    public void Init(int p_Year, int p_Month, int p_Day, int p_Hour, int p_Minutes, int p_Seconds)
    {
        m_Year = p_Year;
        m_Month = p_Month;
        m_Day = p_Day;
        m_Hours = p_Hour;
        m_Minutes = p_Minutes;
        m_Seconds = p_Seconds;
    }

    public bool AddSecondAndUpdateClock(float p_Value)
    {
        bool l_ShouldUpdate = m_Seconds + p_Value > (int)m_Seconds + 1;
        m_Seconds = m_Seconds + p_Value;

        #region Recalcul time

        if (!(m_Seconds >= 60)) return l_ShouldUpdate;

        m_Minutes = m_Minutes + (int)(m_Seconds / 60);
        m_Seconds = 0;

        if (m_Minutes < 60) return l_ShouldUpdate;

        m_Hours = m_Hours + m_Minutes / 60;
        m_Minutes = 0;

        if (m_Hours < 24) return l_ShouldUpdate;

        m_Day = m_Day + m_Hours / 24;
        m_Hours = 0;

        if (m_Day < GetCurrentMonthDayCount()) return l_ShouldUpdate;

        m_Month = m_Month + m_Day / GetCurrentMonthDayCount();
        m_Day = 0;

        if (m_Month < 12) return l_ShouldUpdate;

        m_Year = m_Year + m_Month / 12;
        m_Month = 0;

        #endregion

        return l_ShouldUpdate;
    }

    public int GetCurrentMonthDayCount()
    {
        return m_DaysInMonth[m_Month - 1];
=======
    public int Day;
    public int Hours;
    public int Minutes;
    public int Month;
    public float Seconds;
    public int Year;

    public void Init(int p_Year, int p_Month, int p_Day, int p_Hour, int p_Minutes, int p_Seconds)
    {
        Year = p_Year;
        Month = p_Month;
        Day = p_Day;
        Hours = p_Hour;
        Minutes = p_Minutes;
        Seconds = p_Seconds;
    }

    public bool AddSecondAndUpdateClock(float p_Value)
    {
        bool l_ShouldUpdate = Seconds + p_Value > (int)Seconds + 1;

        Seconds = Seconds + p_Value;

        #region Recalcul time

        if (!(Seconds >= 60)) return l_ShouldUpdate;

        Minutes = Minutes + (int)(Seconds / 60);
        Seconds = 0;

        if (!(Minutes >= 60))
            return l_ShouldUpdate;

        Hours = Hours + Minutes / 60;
        Minutes = 0;

        if (!(Hours >= 24))
            return l_ShouldUpdate;

        Day = Day + Hours / 24;
        Hours = 0;

        if (!(Day >= GetCurrentMonthDayCount()))
            return l_ShouldUpdate;

        Month = Month + Day / GetCurrentMonthDayCount();
        Day = 0;

        if (!(Month >= 12))
            return l_ShouldUpdate;

        Year = Year + Month / 12;
        Month = 0;

        #endregion

        return l_ShouldUpdate;
    }
    public int GetCurrentMonthDayCount()
    {
        return m_DaysInMonth[Month];
>>>>>>> Stashed changes
    }
}