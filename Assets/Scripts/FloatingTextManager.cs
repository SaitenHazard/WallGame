using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager instance;
    private float cloneDeathTimer = 1f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        EventManager.OnReplenishWood += ReplenishWood;
        EventManager.OnReplenishStone += ReplenishStone;
        EventManager.OnEnterCatapult += EnterCatapult;
        EventManager.OnCatapultFire += GetLaunched;
    }

    private void OnDestroy()
    {
        EventManager.OnReplenishWood -= ReplenishWood;
        EventManager.OnReplenishStone -= ReplenishStone;
        EventManager.OnEnterCatapult -= EnterCatapult;
        EventManager.OnCatapultFire -= GetLaunched;
    }

    private void ReplenishWood(int i)
    {
        DoFloatingText("+ " + i + " Wood!");
    }

    private void ReplenishStone(int i)
    {
        DoFloatingText("+ " + i + " Stone!");
    }

    private void EnterCatapult(Transform catapultBowl)
    {
        DoFloatingText("Set!");
    }

    private void GetLaunched(Vector3[] path, int vertexCount)
    {
        DoFloatingText("Weee!");
    }

    public void DoFloatingText(string str)
    {
        GameObject duplicate = Instantiate(gameObject.transform.GetChild(0).gameObject);
        Destroy(duplicate, cloneDeathTimer);
        duplicate.transform.GetChild(0).GetComponent<TextMeshPro>().text = str;
        duplicate.transform.GetChild(0).GetComponent<Animator>().enabled = true;
        duplicate.transform.position = gameObject.transform.position;
    }
}
