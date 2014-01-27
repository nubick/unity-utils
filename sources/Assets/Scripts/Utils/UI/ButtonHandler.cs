using System;

namespace Assets.Scripts.Utils.UI
{
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
