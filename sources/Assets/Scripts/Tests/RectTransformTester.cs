using System.Collections;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class RectTransformTester : MonoBehaviour
    {
        public RectTransform RectLeftRightTopBottom1;
        public RectTransform RectLeftRightTopBottom2;

        public RectTransform RectPosXYWidthHeight1;
        public RectTransform RectPosXYWidthHeight2;
        public RectTransform RectPosXYWidthHeight3;

        public RectTransform RectBottom;

        public Transform Star1;
        public Transform Star2;
        public Transform Star3;
        public Transform Star4;
        public Transform Star5;
        public Transform Star6;
        public Transform Star7;

        public RectTransform AnchoredTopLeft;
        public RectTransform AnchoredTopRight;
        public RectTransform AnchoredBottomLeft;
        public RectTransform AnchoredBottomRight;

        public void Awake()
        {
            StartCoroutine(RunTestsCoroutine());
        }

        private IEnumerator RunTestsCoroutine()
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(0.5f);

            RectLeftRightTopBottom1.SetLeft(50f);
            yield return waitForSeconds;
            RectLeftRightTopBottom1.SetRight(60f);
            yield return waitForSeconds;
            RectLeftRightTopBottom1.SetTop(70f);
            yield return waitForSeconds;
            RectLeftRightTopBottom1.SetBottom(80f);

            yield return waitForSeconds;

            RectLeftRightTopBottom2.SetLeftTopRightBottom(10f, 20f, 30f, 40f);

            yield return waitForSeconds;

            RectPosXYWidthHeight1.SetPosX(100f);
            yield return waitForSeconds;
            RectPosXYWidthHeight1.SetPosY(150f);
            yield return waitForSeconds;
            RectPosXYWidthHeight1.SetWidth(50f);
            yield return waitForSeconds;
            RectPosXYWidthHeight1.SetHeight(75f);

            yield return waitForSeconds;
            RectPosXYWidthHeight2.SetPosXY(10f, 15f);
            yield return waitForSeconds;
            RectPosXYWidthHeight2.SetWidthHeight(20f, 30f);

            yield return waitForSeconds;
            RectPosXYWidthHeight3.SetPosAndSize(-10f, -15f, 30f, 20f);

            yield return waitForSeconds;
            RectBottom.SetHeight(100f);
            yield return waitForSeconds;
            RectBottom.SetLeft(30f);
            yield return waitForSeconds;
            RectBottom.SetRight(40f);
            yield return waitForSeconds;
            RectBottom.SetPosY(20f);

            yield return waitForSeconds;
            Star1.position = RectPosXYWidthHeight3.GetWorldCenter();

            yield return waitForSeconds;
            Star2.position = RectPosXYWidthHeight1.GetWorldTopLeft();

            yield return waitForSeconds;
            Star3.position = RectPosXYWidthHeight1.GetWorldTopRight();

            yield return waitForSeconds;
            Star4.position = RectPosXYWidthHeight1.GetWorldBottomLeft();

            yield return waitForSeconds;
            Star5.position = RectPosXYWidthHeight1.GetWorldBottomRight();

            yield return waitForSeconds;
            Star6.position = new Vector2(RectBottom.GetWorldLeft(), RectBottom.GetWorldTop());

            yield return waitForSeconds;
            Star7.position = new Vector2(RectBottom.GetWorldRight(), RectBottom.GetWorldBottom());

            yield return waitForSeconds;
            AnchoredTopLeft.SetPosX(30f);
            AnchoredTopLeft.SetPosY(-30f);
            yield return waitForSeconds;
            AnchoredTopRight.SetPosX(-30f);
            AnchoredTopRight.SetPosY(-30f);
            yield return waitForSeconds;
            AnchoredBottomLeft.SetPosX(30f);
            AnchoredBottomLeft.SetPosY(30f);
            yield return waitForSeconds;
            AnchoredBottomRight.SetPosX(-30f);
            AnchoredBottomRight.SetPosY(30f);
        }
    }
}