using System;

namespace Assets.Scripts.Utils.Tweens
{
    class TweenSequence
    {
        public static void Run(Func<TweenBase> action1, Func<TweenBase> action2, Action onFinishAction = null)
        {
            TweenBase tween1 = action1();
            Action tween1FinishAction = tween1.FinishAction;
            tween1.OnFinish(() =>
            {
                if (tween1FinishAction != null)
                    tween1FinishAction();

                TweenBase tween2 = action2();
                Action tween2FinishAction = tween2.FinishAction;
                tween2.OnFinish(() =>
                {
                    if (tween2FinishAction != null)
                        tween2FinishAction();

                    if (onFinishAction != null)
                        onFinishAction();
                });
            });
        }
    }
}
