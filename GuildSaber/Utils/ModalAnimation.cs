using CP_SDK.UI;
using OVR.OpenVR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GuildSaber.Utils
{
    internal class ModalAnimation : FloatAnimation
    {
        public const float MODAL_ANIMATION_DURATION = 0.1f;

        protected IModal m_Modal;

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void SetModal(IModal p_Modal)
        {
            m_Modal = p_Modal;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        protected float m_BaseHeight;

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        protected override void OnInit()
        {
            OnChange += OnValueChange;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        protected override void OnPlay()
        {
            if (m_Modal == null) return;

            if (m_BaseHeight == 0)
                m_BaseHeight = m_Modal.gameObject.transform.localScale.y;
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        private void OnValueChange(float p_Value)
        {
            if (m_Modal == null) return;

            var l_Current = m_Modal.gameObject.transform.localScale;
            var l_New = new Vector3(l_Current.x, m_BaseHeight * p_Value, l_Current.z);

            m_Modal.gameObject.transform.localScale = l_New;
            //m_Modal.RTransform.rect.Set(m_Modal.RTransform.rect.position.x, m_Modal.RTransform.rect.position.y, m_StartWidth, m_StartHeight * p_Value);

            //m_Modal.RTransform.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Horizontal, m_StartWidth * p_Value);
            //m_Modal.RTransform.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Vertical, m_StartHeight * p_Value);
        }

        private void Finished(float p_Value)
        {
            OnFinished -= Finished;
            m_Modal.gameObject.SetActive(false);
            foreach (var l_Item in m_Modal.gameObject.GetComponents<Image>())
            {
                l_Item.color = Color.black.ColorWithAlpha(0.95f);
            };
        }

        ////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        public void ShowModal()
        {
            Init(0, 1, MODAL_ANIMATION_DURATION);
            Play();
        }

        public void HideModal()
        {
            Init(1, 0, MODAL_ANIMATION_DURATION);
            OnFinished += Finished;
            Play();
        }
    }
}
