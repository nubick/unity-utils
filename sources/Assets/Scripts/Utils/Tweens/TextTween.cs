using UnityEngine;
using System.Collections;
using Assets.Scripts.Utils.Tweens;
using UnityEngine.UI;
using Assets.Scripts.Utils;

namespace Assets.Scripts
{
	public class TextTween : MonoBehaviourBase 
	{
		private Text _textComponent;
		private string _text;
		private int _ind;
		private float _speed;
		private string[] _parts;

		public static TextTween Run(Text textComponent, string text, float speed)
		{
			TextTween tween = textComponent.gameObject.AddComponent<TextTween>();
			tween._textComponent = textComponent;
			tween._text = text;
			tween._speed = speed;
			return tween;
		}

		public void Start()
		{
			_parts = _text.Split(' ');
			_ind = 0;
			_textComponent.text = string.Empty;
			UpdateNext();
		}

		private void UpdateNext()
		{
			_textComponent.text =
				_ind == 0 ?
					_parts[0]:
					_textComponent.text + " " + _parts[_ind];

			_ind++;
			if(_ind == _parts.Length)
				Destroy(this);
			else
				Invoke(() => UpdateNext(), _speed);
		}

		public void Finish()
		{
			CancelInvoke(() => UpdateNext());
			_textComponent.text = _text;
			Destroy(this);
		}
	}
}
