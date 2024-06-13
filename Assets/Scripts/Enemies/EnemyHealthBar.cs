using Enemies;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject bar;

    private Vector2 fullSizeDelta;

    [SerializeField] private float maxHealth;

    [SerializeField] private float health;


    // Start is called before the first frame update
    void Start()
    {
        maxHealth = (float)ArmyController.instance.enemyCount;
        fullSizeDelta = bar.GetComponent<RectTransform>().sizeDelta;
    }

    // Update is called once per frame
    void Update()
    {
        health = (float)ArmyController.instance.enemyCount;

        Vector2 width = fullSizeDelta;

        width.x = fullSizeDelta.x * health / maxHealth;

        bar.GetComponent<RectTransform>().sizeDelta = width;
    }
}
