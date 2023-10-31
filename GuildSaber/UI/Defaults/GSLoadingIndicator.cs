using CP_SDK.UI.Components;
using CP_SDK.XUI;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GuildSaber.UI.Defaults
{
    internal class GSLoadingIndicator : XUIVLayout
    {
        protected GSLoadingIndicator(string p_Name, params IXUIElement[] p_Childs) : base(p_Name, p_Childs)
        {
            OnReady(OnCreation);
        }

        public static GSLoadingIndicator Make()
        {
            return new GSLoadingIndicator("GuildSaberLoadingContainer");
        }

        static GameObject m_LoadingContainerObject;

        private void OnCreation(CHOrVLayout p_Layout)
        {
            if (m_LoadingContainerObject == null)
            {
                m_LoadingContainerObject = Resources.FindObjectsOfTypeAll<LoadingControl>().First().GetField<GameObject, LoadingControl>("_loadingContainer"); 
            }

            GameObject l_LoadingContainer = GameObject.Instantiate(m_LoadingContainerObject, Vector3.zero, Quaternion.Euler(new Vector3(0, 0, 0)));
            l_LoadingContainer.transform.SetParent(Element.transform, false);
            l_LoadingContainer.GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);

            Element.CSizeFitter.horizontalFit = Element.CSizeFitter.verticalFit = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}
