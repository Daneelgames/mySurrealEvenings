using UnityEngine;
using System.Collections;

public class InventorySlotController : MonoBehaviour {

    public GameObject itemInSlot;

    public void SetItem(GameObject item)
    {
        itemInSlot = item;
    }

    public void RemoveItem()
    {
        itemInSlot = null;
    }
}