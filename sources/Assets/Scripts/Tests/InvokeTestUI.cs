using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class InvokeTestUI : MonoBehaviour
    {
        public void OnInvokeButtonClick()
        {
            this.Invoke(() => InvokedFunc(), 3f);
        }

        public void OnIsInvokingButtonClick()
        {
            bool isInvoking = this.IsInvoking(() => InvokedFunc());
            Debug.Log("IsInvoking:" + isInvoking);
        }

        public void OnInvokeRepeatingButtonClick()
        {
            this.InvokeRepeating(() => InvokedFunc(), 1f, 1f);
        }

        public void OnCancelInvokeButtonClick()
        {
            this.CancelInvoke(() => InvokedFunc());
        }

        public void OnInvokeWithParametersButtonClick()
        {
            this.Invoke(() => InvokedFuncWithParameter(123), 3f);
        }
        
        private void InvokedFunc()
        {
            Debug.Log("Invoked func executed.");
        }

        private void InvokedFuncWithParameter(int intParameter)
        {
            Debug.Log($"Invoke method with parameter '{intParameter}'.");
        }
    }
}
