using UnityEngine;

namespace Enemies
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private GameObject bar;

        private RectTransform _rectTransform;
        private Vector2 _fullSizeDelta;

        private float _health;

        private float _maxHealth;


        // Start is called before the first frame update
        private void Start()
        {
            _maxHealth = ArmyController.instance.enemyCount;
            _rectTransform = bar.GetComponent<RectTransform>();
            _fullSizeDelta = _rectTransform.sizeDelta;
        }

        // Update is called once per frame
        private void Update()
        {
            _health = ArmyController.instance.enemyCount;

            var width = _fullSizeDelta;

            width.x = _fullSizeDelta.x * _health / _maxHealth;

            // Let's not call GetComponent each frame, especially when we already have the component
            // bar.GetComponent<RectTransform>().sizeDelta = width;
            _rectTransform.sizeDelta = width;
        }
    }
}