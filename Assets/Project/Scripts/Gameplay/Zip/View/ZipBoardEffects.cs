using System.Collections;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipBoardEffects:MonoBehaviour
    {
        [Header("Glare")]
        [SerializeField] private Transform _glare;
        [SerializeField] private float _glareDuration = 0.2f;
        [SerializeField] private Transform _glareFrom;
        [SerializeField] private Transform _glareTo;
        
        [Header("Board Rotation")]
        [SerializeField] private Transform _board;
        [SerializeField] private float _rotationDuration = 0.2f;
        [SerializeField] private AnimationCurve _rotationCurve;
        [SerializeField] private Vector3 _rotation;
        [Header("Finish Animation")]
        [SerializeField] private float _preGlareDelay = 0.2f;
        [SerializeField] private float _preRotationDelay = 0.2f;
        [SerializeField] private float _postRotationDelay = 0.2f;

        public IEnumerator PlayFinishAnimation()
        {
            yield return new WaitForSeconds(_preGlareDelay);
            yield return PlayGlareAnimation();
            yield return new WaitForSeconds(_preRotationDelay);
            yield return PlayBoardRotationAnimation();

            yield return new WaitForSeconds(_postRotationDelay);
        }

        private IEnumerator PlayGlareAnimation()
        {
            float elapsedTime = 0;
            while (elapsedTime < _glareDuration)
            {
                elapsedTime += Time.deltaTime;
                _glare.position = Vector3.Lerp(_glareFrom.position, _glareTo.position, elapsedTime / _glareDuration);
                yield return null;
            }
            
        }

        private IEnumerator PlayBoardRotationAnimation()
        {
            float elapsedTime = 0;
            while (elapsedTime < _rotationDuration)
            {
                elapsedTime += Time.deltaTime;
                _board.eulerAngles = Vector3.Lerp(Vector3.zero, _rotation, _rotationCurve.Evaluate(elapsedTime / _rotationDuration));
                yield return null;
            }
            _board.eulerAngles = Vector3.zero;
        }
    }
}