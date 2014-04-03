using System;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class SiteLocker : MonoBehaviourBase
    {
        public string LockUrl;
        public bool IsShowDebugInfo;

        public void Awake()
        {
            if (Application.isWebPlayer)
            {
                string url = Application.absoluteURL;
                if (string.IsNullOrEmpty(url))
                {
                    NavigationToLockUrl();
                }
                else
                {
                    Uri uri = new Uri(url);
                    if (uri.Host.ToLower() != LockUrl.ToLower())
                    {
                        NavigationToLockUrl();
                    }
                }
            }
        }

        public void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Absolute url:" + Application.absoluteURL);
            Uri uri = new Uri(Application.absoluteURL);
            GUILayout.Label("Host:" + uri.Host);
            GUILayout.EndVertical();
        }

        private void NavigationToLockUrl()
        {
            string javaScript = string.Format("document.location='http://{0}';", LockUrl);
            Application.ExternalEval(javaScript);
        }

    }
}
