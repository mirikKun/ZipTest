using TMPro;
using UnityEngine;

namespace Project.Scripts.Gameplay.Zip.View
{
    public class ZipCheckPointView:MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _checkPoint;
        [SerializeField] private TextMeshProUGUI _index;
        public void SetIndex(int index)
        {
            _index.text = index.ToString();
        }
    }
}