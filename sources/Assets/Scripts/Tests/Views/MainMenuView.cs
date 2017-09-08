using Assets.Scripts.Utils.Views;

namespace Assets.Scripts.Tests.Views
{
    public class MainMenuView : ViewBase
    {
		public StartGameView StartGameView;
		public OptionsView OptionsView;

		public void OnStart()
		{
			SwitchTo(StartGameView);
		}

		public void OnOptions()
		{
			SwitchTo(OptionsView);
		}

        public override void OnBackKey()
        {
            Exit();
        }

		public void Exit()
		{
#if UNITY_ANDROID
			AndroidMoveGameToBack();
#endif
		}
    }
}