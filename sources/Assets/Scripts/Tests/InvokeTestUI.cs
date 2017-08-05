using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class InvokeTestUI : MonoBehaviour
    {
        public void OnGUI()
        {
            GUILayout.BeginVertical();
            
            if (GUILayout.Button("Invoke"))
            {
                this.Invoke(() => InvokedFunc(), 3f);
            }
            
            if (GUILayout.Button("IsInvoking"))
            {
                bool isInvoking = this.IsInvoking(() => InvokedFunc());
                Debug.Log("IsInvoking:" + isInvoking);
            }
            
            if (GUILayout.Button("InvokeRepeating"))
            {
                this.InvokeRepeating(() => InvokedFunc(), 1f, 1f);
            }

            if (GUILayout.Button("CancelInvoke"))
            {
                this.CancelInvoke(() => InvokedFunc());
            }

            GUILayout.EndVertical();
        }

        private void InvokedFunc()
        {
            Debug.Log("Invoked func executed.");
        }
    }
}
