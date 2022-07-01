using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSProfile.Time
{
    public class OptimizedDateTime
    {
        public int m_Year;
        public int m_Month;
        public int m_Day;
        public int m_Hours;
        public int m_Minutes;
        public float m_Seconds;
        List<int> daysInMonth = new List<int>()
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

        public void Init(int p_Year, int p_Month, int p_Day, int p_Hour, int p_Minutes, int p_Seconds)
        {
            m_Year = p_Year;
            m_Month = p_Month;
            m_Day = p_Day;
            m_Hours = p_Hour;
            m_Minutes = p_Minutes;
            m_Seconds = p_Seconds;
        }

        public bool AddSecondAndUpdateclock(float p_value)
        {
            bool l_shouldUpdate = false;

            if (m_Seconds + p_value > (int)m_Seconds + 1)
            {
                l_shouldUpdate = true;
            }

            m_Seconds = m_Seconds + p_value;

            #region Recalcul time
            if (m_Seconds >= 60)
            {
                m_Minutes = m_Minutes + (int)(m_Seconds / 60);
                m_Seconds = 0;
            }

            if (m_Minutes >= 60)
            {
                m_Hours = m_Hours + m_Minutes / 60;
                m_Minutes = 0;
            }

            if (m_Hours >= 24)
            {
                m_Day = m_Day + m_Hours / 24;
                m_Hours = 0;
            }

            if (m_Day >= GetCurrentMonthDayCount())
            {
                m_Month = m_Month + (m_Day / GetCurrentMonthDayCount());
                m_Day = 0;
            }

            if (m_Month >= 12)
            {
                m_Year = m_Year + (m_Month / 12);
                m_Month = 0;
            }

            #endregion
            return l_shouldUpdate;
        }
        public int GetCurrentMonthDayCount()
        {
            return daysInMonth[m_Month];
        }
    }
}
