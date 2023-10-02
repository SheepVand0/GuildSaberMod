using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GuildSaber.Bot
{
    internal class BotManager
    {

        internal struct BotNoteData
        {
            public Vector3 HitPosition;
            public ColorType NoteType;
            public NoteCutDirection NoteDirection;

            public BotNoteData(Vector3 p_HitPosition, ColorType p_NoteType, NoteCutDirection p_Direction)
            {
                HitPosition = p_HitPosition;
                NoteType = p_NoteType;
                NoteDirection = p_Direction;
            }

        }

        protected GameObject m_LeftSaber;
        protected GameObject m_RightSaber;

        protected bool CanPlay = false;

        public void Check()
        {
            CanPlay = m_LeftSaber != null && m_RightSaber != null;
        }

        public void Setup()
        {
            m_LeftSaber = GameObject.Find("LeftSaber");
            m_RightSaber = GameObject.Find("RightSaber");

            Check();
        }

        protected Queue<BotNoteData> m_WaitingNotes = new Queue<BotNoteData>();

        public void HandleNote(NoteData p_NoteData)
        {
            if (!CanPlay) return;

            
        }

    }
}
