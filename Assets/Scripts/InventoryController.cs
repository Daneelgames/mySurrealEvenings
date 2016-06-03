using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

    public List<GameObject> items = new List<GameObject>();

    public List<InventorySlotController> slots;

    public int money = 0;

    public void MoneyGet(int amount)
    {
        money += amount;
    }

    public void MoneyLose(int amount)
    {
        money -= amount;
    }

    public void ItemGet(GameObject item)
    {
        GameObject itemInstance = null;
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemInSlot == null)
            {
                itemInstance = Instantiate(item, slots[i].transform.position, slots[i].transform.rotation) as GameObject;
                itemInstance.transform.parent = slots[i].gameObject.transform;
                items.Add(item);
                slots[i].SetItem(item);
                break;
            }
        }
    }

    public void ItemLost(GameObject item)
    {
        foreach (InventorySlotController slot in slots)
        {
            if (slot.itemInSlot == item)

            {
                slot.RemoveItem();
                items.Remove(item);
                SortSlots();
                break;
            }
        }
    }

    void SortSlots()
    {
        for (int i = 4; i > 0; i--)
        {
            if (slots[i].itemInSlot != null && slots[i - 1].itemInSlot == null)
            {
                slots[i - 1].SetItem(slots[i].itemInSlot);
                slots[i].RemoveItem();
                slots[i - 1].itemInSlot.transform.position = slots[i - 1].transform.position;
            }
        }
    }
}