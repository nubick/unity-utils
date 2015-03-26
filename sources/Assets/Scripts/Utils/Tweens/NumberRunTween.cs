using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils.Tweens
{
    public class NumberRunTween : TweenBase
    {
        private int _from;
        private int _to;
        private Text _text;

        public static NumberRunTween Run(GameObject item, int from, int to, float duration)
        {
            NumberRunTween tween = Create<NumberRunTween>(item, duration);
            tween._from = from;
            tween._to = to;
            return tween;
        }
      
        protected override void OnStart()
        {
            _text = GetComponent<Text>();
        }

        protected override void UpdateValue(float time)
        {
	        _text.text = ((int) EaseFunc(_from, _to, time)).ToString();
        }
    }
}
