using Assets.Scripts.Utils.Views;

namespace Assets.Scripts.Tests.Views
{
    public class GameView : ViewBase
    {
        public MainMenuView MainMenuView;

        public override void OnBackKey()
        {
            SwitchTo(MainMenuView);
        }
	}
}