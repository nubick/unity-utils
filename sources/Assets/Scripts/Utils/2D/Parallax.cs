using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class Parallax : MonoBehaviourBase
    {
        private Transform[] _items;

        public Transform ItemPrefab;
        public int ItemsCount;
        public float ItemWidth;
        public float Speed;
        public float LeftBoundX;

        public void Awake()
        {
            _items = new Transform[ItemsCount];
            for (int i = 0; i < ItemsCount; i++)
            {
                _items[i] = Instantiate<Transform>(ItemPrefab);
                _items[i].parent = Transform;
                _items[i].SetLocalXY(LeftBoundX, 0f);
            }
        }

        public void Update()
        {
            if (_items[0].position.x < LeftBoundX)
                MoveFirstPart();

            _items[0].Translate(Vector3.left*Time.deltaTime*Speed);
            for (int i = 1; i < ItemsCount; i++)
                _items[i].SetLocalX(_items[0].localPosition.x + i*ItemWidth);
        }

        private void MoveFirstPart()
        {
            Transform temp = _items[0];
            for (int i = 0; i < ItemsCount - 1; i++)
                _items[i] = _items[i + 1];
            _items[ItemsCount - 1] = temp;
        }
    }
}
