using UnityEngine;

namespace Assets.Scripts.Utils.Views
{
    public abstract class ViewBase : MonoBehaviour
    {
        public GameObject Content;

        public bool IsActive { get { return Content.activeSelf; } }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        public void Show()
        {
            Content.SetActive(true);
            OnShow();
        }

        public void Hide()
        {
            Content.SetActive(false);
            OnHide();
        }

        protected void SwitchTo(ViewBase otherView)
        {
            Hide();
            otherView.Show();
        }

        #region Back button

		public virtual void OnBackKey()
		{
			Debug.Log("OnBackKey is not implemented for View: " + name);
		}

#if UNITY_ANDROID
        //This method can be used on Android platform. It moves application to background, don't close application.
        //Application just looses focus after that. It can be useful when back button clicked in main menu, for instance.
        protected void AndroidMoveGameToBack()
        {
            AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            activity.Call<bool>("moveTaskToBack", true);
        }
#endif

        #endregion
    }
}
