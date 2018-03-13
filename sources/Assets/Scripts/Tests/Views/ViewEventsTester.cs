using Assets.Scripts.Utils.Views;
using UnityEngine;

namespace Assets.Scripts.Tests.Views
{
    public class ViewEventsTester : MonoBehaviour
    {
        public ViewBase[] Views;

        public void OnEnable()
        {
            foreach (ViewBase view in Views)
            {
                view.OnShowEvent += OnViewShow;
                view.OnHideEvent += OnViewHide;
            }
        }

        public void OnDisable()
        {
            foreach(ViewBase view in Views)
            {
                view.OnShowEvent -= OnViewShow;
                view.OnHideEvent -= OnViewHide;
            }
        }

        private void OnViewShow(ViewBase view)
        {
            Debug.Log("View is shown: " + view.name);
        }

        private void OnViewHide(ViewBase view)
        {
            Debug.Log("View is hidden: " + view.name);
        }
    }
}