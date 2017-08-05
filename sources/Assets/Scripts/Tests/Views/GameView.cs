using Assets.Scripts.Utils.Views;

namespace Assets.Scripts.Tests.Views
{
    public class GameView : ViewBase
    {
        public MainMenuView MainMenuView;

        protected override void OnBackKey()
        {
            SwitchTo(MainMenuView);
        }
	}
}