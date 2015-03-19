using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeInChildrenTween : TweenBase
    {
        private Dictionary<Image, float> _imageAlphaDic;
        private List<Text> _texts;

        public static FadeInChildrenTween Run(GameObject item, float duration)
        {
            FadeInChildrenTween tween = Create<FadeInChildrenTween>(item, duration);
            tween.Init();
            return tween;
        }

        private void Init()
        {
            _imageAlphaDic = new Dictionary<Image, float>();
            foreach (Image image in GetComponentsInChildren<Image>())
                _imageAlphaDic.Add(image, image.color.a);

            _texts = GetComponentsInChildren<Text>().ToList();            
            UpdateValue(0f);
        }

        protected override void OnStart()
        {

        }

        protected override void UpdateValue(float time)
        {
            foreach (Image image in _imageAlphaDic.Keys)
                image.color = image.color.SetA(EaseFunc(0f, _imageAlphaDic[image], time));

            float alpha = EaseFunc(0f, 1f, time);
            foreach (Text text in _texts)
                text.color = text.color.SetA(alpha);
        }
    }
}
