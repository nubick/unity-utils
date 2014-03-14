using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utils.Social
{
    /// <summary>
    /// Try to open facebook or twitter application on device. If such application is not installed try to open
    /// default browser.
    ///
    /// Facebook: 
    /// URL: https://www.facebook.com/pages/name/id
    /// AppURL: fb://profile/id
    ///
    /// Twitter:
    /// URL: https://www.twitter.com/name
    /// AppURL: twitter://user?screen_name=name
    /// </summary>
    public class SocialLink : MonoBehaviourBase
    {
        private bool _isLeftApp;

        public string URL;
        public string AppURL;
        public float WaitingHackDelay;

        public void OpenUrl()
        {
            StartCoroutine(OpenUrlCoroutine());
        }

        private IEnumerator OpenUrlCoroutine()
        {
            _isLeftApp = false;
            Application.OpenURL(AppURL);
            yield return new WaitForSeconds(WaitingHackDelay);
            if (!_isLeftApp)
                Application.OpenURL(URL);
        }

        public void OnApplicationPause()
        {
            _isLeftApp = true;
        }

    }
}
