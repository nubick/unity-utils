namespace Assets.Scripts.Utils.Tweens
{
    public class Ease
    {
        public static float OutBack(float start, float end, float time)
        {
            float s = 1.70158f;
            end -= start;
            time = (time) - 1;
            return end * ((time) * time * ((s + 1) * time + s) + 1) + start;
        }

        public static float InBack(float start, float end, float time)
        {
            //return OutBack(end, start, 1f - time);
            end -= start;
            time /= 1;
            float s = 1.70158f;
            return end * (time) * time * ((s + 1) * time - s) + start;
        }
    }
}
