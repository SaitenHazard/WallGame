using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class MockupBoltReceiver : MonoBehaviour
{
    private Queue<DamageDealer> damageQueue;

    private int armyRemaining = 300;



    private Text text;

    private float springStiffnes = 1.0f;
    private float restingSize = 1.0f;
    private float currentScale = 1.0f;

    void Start()
    {
        damageQueue = new Queue<DamageDealer>();    
        text = GetComponent<Text>();
        updateText();
    }

    public void EnqueueDamage(int damage)
    {
        damageQueue.Enqueue(new DamageDealer(damage));
    }

    public void Update()
    {
        if (damageQueue.Count != 0)
        {
            while (damageQueue.Count > 0)
            {
                armyRemaining -= damageQueue.Dequeue().damage;
            }
            updateText();
            currentScale += 0.1f;
        }
        float desireddisplacement = 1 - currentScale;
        currentScale += desireddisplacement * 0.1f;
        transform.localScale = Vector3.one * currentScale;
    }

    private void updateText()
    {
        text.text = armyRemaining + " enemies remaining!";
    }

    private class DamageDealer
    {
        public DamageDealer(int damage)
        {
            this.damage = damage;
        }

        public int damage;
    }
}


