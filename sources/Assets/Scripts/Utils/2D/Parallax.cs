using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class Parallax : MonoBehaviourBase
    {
        private GameObject[] _items;

        public GameObject ItemPrefab;
        public int ItemsCount;
        public float ItemWidth;
        public float Speed;
        public float LeftBoundX;

        public void Awake()
        {
            _items = new GameObject[ItemsCount];
            for (int i = 0; i < ItemsCount; i++)
            {
                _items[i] = Instantiate<GameObject>(ItemPrefab);
                _items[i].transform.parent = Transform;
                _items[i].transform.SetLocalXY(LeftBoundX, 0f);
            }
        }

        public void Update()
        {
            if (_items[0].transform.position.x < LeftBoundX)
                MoveFirstPart();

            _items[0].transform.Translate(Vector3.left*Time.deltaTime*Speed);
            for (int i = 1; i < ItemsCount; i++)
                _items[i].transform.SetLocalX(_items[0].transform.localPosition.x + i*ItemWidth);
        }

        private void MoveFirstPart()
        {
            GameObject temp = _items[0];
            for (int i = 0; i < ItemsCount - 1; i++)
                _items[i] = _items[i + 1];
            _items[ItemsCount - 1] = temp;
        }
    }
}
