using System;
using UnityEngine;

namespace Assets.Scripts.Utils.UI
{
    [RequireComponent(typeof(BoxCollider2D))]
	public class ButtonHandler : MonoBehaviourBase
	{
		public event EventHandler Click;

		public void OnMouseUpAsButton()
		{
			if (Click != null)
				Click(this, EventArgs.Empty);
		}
	}
}
