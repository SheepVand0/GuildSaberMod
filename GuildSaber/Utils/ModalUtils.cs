using CP_SDK.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuildSaber.Utils
{
    internal static class ModalUtils
    {
        

        public static ModalAnimation GetModalAnimation(this IModal p_Modal)
        {
            var l_Animation = p_Modal.gameObject.GetComponent<ModalAnimation>();
            if (l_Animation == null)
            {
                l_Animation = p_Modal.gameObject.AddComponent<ModalAnimation>();
                l_Animation.SetModal(p_Modal);
            }
            return l_Animation;
        }

        public static void OnModalShow(IModal p_Modal)
        {
            var l_Animation = p_Modal.GetModalAnimation();

            l_Animation.ShowModal();
        }

        public static void OnModalHide(IModal p_Modal)
        {
            var l_Animation = p_Modal.GetModalAnimation();

            l_Animation.HideModal();
        }

    }
}
