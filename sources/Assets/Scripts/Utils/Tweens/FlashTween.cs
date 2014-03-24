using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FlashTween : MonoBehaviour
    {
        public static void Run(GameObject item, float duration)
        {
            FlashTween tween = item.GetComponent<FlashTween>();
            if (tween == null)
                tween = item.AddComponent<FlashTween>();
            else
                tween.StopAllCoroutines();
            tween.Run(duration);
        }

        private void Run(float duration)
        {
            gameObject.SetActive(true);
            StartCoroutine(FlashCoroutine(duration));
        }

        private IEnumerator FlashCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            gameObject.SetActive(false);
            Destroy(this);
        }

    }
}
