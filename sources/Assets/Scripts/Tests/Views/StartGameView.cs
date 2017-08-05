using Assets.Scripts.Utils.Views;

namespace Assets.Scripts.Tests.Views
{
	public class StartGameView : ViewBase
	{
        public MainMenuView MainMenuView;
        public GameView GameView;

        public void OnStart()
        {
            SwitchTo(GameView);
        }

        public void OnBack()
        {
            SwitchTo(MainMenuView);
        }

        protected override void OnBackKey()
        {
            OnBack();
        }
	}
}