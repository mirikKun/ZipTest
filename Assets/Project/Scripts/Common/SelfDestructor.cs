using UnityEngine;

namespace Code.Common
{
    public class SelfDestructor : MonoBehaviour
    {
        [SerializeField] private float _countdown = 3.0f;

        public void SetCountdown(float countdown)
        {
            _countdown = countdown;
        }
        private void Update()
        {
            _countdown -= UnityEngine.Time.deltaTime;
            if (_countdown <= 0)
                Destroy(gameObject);
        }
    }
}