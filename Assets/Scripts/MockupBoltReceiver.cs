using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MockupBoltReceiver : MonoBehaviour
{
    private int _armyRemaining = 300;

    private float _currentScale = 1.0f;
    private Queue<DamageDealer> _damageQueue;

    private Text _text;

    private void Start()
    {
        _damageQueue = new Queue<DamageDealer>();
        _text = GetComponent<Text>();
        UpdateText();
    }

    public void Update()
    {
        if (_damageQueue.Count != 0)
        {
            while (_damageQueue.Count > 0) _armyRemaining -= _damageQueue.Dequeue().Damage;
            UpdateText();
            _currentScale += 0.1f;
        }

        var desireddisplacement = 1 - _currentScale;
        _currentScale += desireddisplacement * 0.1f;
        transform.localScale = Vector3.one * _currentScale;
    }

    public void EnqueueDamage(int damage)
    {
        _damageQueue.Enqueue(new DamageDealer(damage));
    }

    private void UpdateText()
    {
        _text.text = _armyRemaining + " enemies remaining!";
    }

    private class DamageDealer
    {
        public readonly int Damage;

        public DamageDealer(int damage)
        {
            this.Damage = damage;
        }
    }
}