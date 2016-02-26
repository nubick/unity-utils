using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils
{
    [RequireComponent(typeof(Button))]
    public class ButtonClickSound : MonoBehaviour
    {
        public AudioSource AudioSource;
        public AudioClip ClickSound;

        public void Awake()
        {
            GetComponent<Button>().onClick.AddListener(PlayClickSound);
        }

        private void PlayClickSound()
        {
            AudioSource.PlayOneShot(ClickSound);
        }
    }

}
