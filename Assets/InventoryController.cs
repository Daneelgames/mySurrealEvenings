using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

    public List<GameObject> items;

    public int money = 0;


    public void MoneyGet(int amount)
    {
        money += amount;
    }

    public void MoneyLose(int amount)
    {
        money -= amount;
    }
}