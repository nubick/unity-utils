using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class InvokeTestUI : MonoBehaviourBase
    {
        public void OnGUI()
        {
            GUILayout.BeginVertical();
            
            if (GUILayout.Button("Invoke"))
            {
                Invoke(() => InvokedFunc(), 3);
            }
            
            if (GUILayout.Button("IsInvoking"))
            {
                bool isInvoking = IsInvoking(() => InvokedFunc());
                Debug.Log("IsInvoking:" + isInvoking);
            }
            
            if (GUILayout.Button("InvokeRepeating"))
            {
                InvokeRepeating(() => InvokedFunc(), 1, 1);
            }

            if (GUILayout.Button("CancelInvoke"))
            {
                CancelInvoke(() => InvokedFunc());
            }

            GUILayout.EndVertical();
        }

        private void InvokedFunc()
        {
            Debug.Log("Invoked func executed.");
        }

    }
}
