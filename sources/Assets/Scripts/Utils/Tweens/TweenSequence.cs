using System;

namespace Assets.Scripts.Utils.Tweens
{
    class TweenSequence
    {
        public static void Run2(
            Func<TweenBase> action1, Func<TweenBase> action2, 
            Action onFinishAction = null)
        {
            RunNextAction(action1, () => RunNextAction(action2, onFinishAction));
        }

        public static void Run3(
            Func<TweenBase> action1, Func<TweenBase> action2, Func<TweenBase> action3, 
            Action onFinishAction = null)
        {
            Run2(action1, action2, () => { RunNextAction(action3, onFinishAction); });
        }

        public static void Run4(
            Func<TweenBase> action1, Func<TweenBase> action2, Func<TweenBase> action3,Func<TweenBase> action4, 
            Action onFinishAction = null)
        {
            Run3(action1, action2, action3, () => { RunNextAction(action4, onFinishAction); });
        }

        public static void Run5(
            Func<TweenBase> action1, Func<TweenBase> action2, Func<TweenBase> action3, Func<TweenBase> action4, 
            Func<TweenBase> action5, 
            Action onFinishAction = null)
        {
            Run4(action1, action2, action3, action4, () => { RunNextAction(action5, onFinishAction); });
        }

        public static void Run6(
            Func<TweenBase> action1, Func<TweenBase> action2, Func<TweenBase> action3, Func<TweenBase> action4, 
            Func<TweenBase> action5, Func<TweenBase> action6, 
            Action onFinishAction = null)
        {
            Run5(action1, action2, action3, action4, action5, () => { RunNextAction(action6, onFinishAction); });
        }

        public static void Run7(
            Func<TweenBase> action1, Func<TweenBase> action2, Func<TweenBase> action3, Func<TweenBase> action4, 
            Func<TweenBase> action5, Func<TweenBase> action6, Func<TweenBase> action7, 
            Action onFinishAction = null)
        {
            Run6(action1, action2, action3, action4, action5, action6, () => { RunNextAction(action7, onFinishAction); });
        }

        public static void Run8(
            Func<TweenBase> action1, Func<TweenBase> action2, Func<TweenBase> action3, Func<TweenBase> action4, 
            Func<TweenBase> action5, Func<TweenBase> action6, Func<TweenBase> action7, Func<TweenBase> action8, 
            Action onFinishAction = null)
        {
            Run7(action1, action2, action3, action4, action5, action6, action7, () => { RunNextAction(action8, onFinishAction); });
        }

        public static void Run9(
            Func<TweenBase> action1, Func<TweenBase> action2, Func<TweenBase> action3, Func<TweenBase> action4,
            Func<TweenBase> action5, Func<TweenBase> action6, Func<TweenBase> action7, Func<TweenBase> action8,
            Func<TweenBase> action9, 
            Action onFinishAction = null)
        {
            Run8(action1, action2, action3, action4, action5, action6, action7, action8, () => { RunNextAction(action9, onFinishAction); });
        }

        public static void Run10(
            Func<TweenBase> action1, Func<TweenBase> action2, Func<TweenBase> action3, Func<TweenBase> action4,
            Func<TweenBase> action5, Func<TweenBase> action6, Func<TweenBase> action7, Func<TweenBase> action8,
            Func<TweenBase> action9, Func<TweenBase> action10, 
            Action onFinishAction = null)
        {
            Run9(action1, action2, action3, action4, action5, action6, action7, action8, action9, () => { RunNextAction(action10, onFinishAction); });
        }

        private static void RunNextAction(Func<TweenBase> nextAction, Action onFinishAction)
        {
            TweenBase nextTween = nextAction();
            Action nextTweenFinishAction = nextTween.FinishAction;
            nextTween.OnFinish(() =>
            {
                if (nextTweenFinishAction != null)
                    nextTweenFinishAction();

                if (onFinishAction != null)
                    onFinishAction();
            });
        }
    }
}
