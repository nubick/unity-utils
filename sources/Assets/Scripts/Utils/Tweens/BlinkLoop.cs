using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class BlinkLoop : MonoBehaviour
    {
        public float BlinkDelay;

        public void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(BlinkCoroutine());
        }

        private IEnumerator BlinkCoroutine()
        {
            for (;;)
            {
				//yield return new WaitForSeconds(BlinkDelay);
				//GetComponent<Renderer>().enabled = false;
				//yield return new WaitForSeconds(BlinkDelay);
				//GetComponent<Renderer>().enabled = true;

				//yield return new WaitForSeconds(BlinkDelay);
				//renderer.enabled = false;
				//yield return new WaitForSeconds(BlinkDelay);
				//renderer.enabled = true;
            }
        }

    }
}
