using UnityEngine;

namespace Assets.Scripts.Utils.Loops
{
    public class StateLoop : Loop
    {
        private int _currentStateIndex;
        private float _nextStateTime;

        public GameObject[] States;
        public float StateDuration;

        public void Awake()
        {
            _currentStateIndex = 0;
            ShowState(_currentStateIndex);
            _nextStateTime = Time.time + StateDuration;
        }

        public void Update()
        {
            if (_nextStateTime <= Time.time)
            {
                _currentStateIndex = (_currentStateIndex + 1)%States.Length;
                ShowState(_currentStateIndex);
                _nextStateTime = Time.time + StateDuration;
            }
        }

        private void ShowState(int stateIndex)
        {
            for (int i = 0; i < States.Length; i++)
                States[i].SetActive(i == stateIndex);
        }
    }
}
