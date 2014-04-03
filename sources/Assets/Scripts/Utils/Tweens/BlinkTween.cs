using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class BlinkTween : MonoBehaviour
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
                yield return new WaitForSeconds(BlinkDelay);
                renderer.enabled = false;
                yield return new WaitForSeconds(BlinkDelay);
                renderer.enabled = true;
            }
        }

    }
}
