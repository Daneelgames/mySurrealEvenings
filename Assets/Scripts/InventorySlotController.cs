using UnityEngine;
using System.Collections;

public class InventorySlotController : MonoBehaviour {

    public SkillController itemInSlot;

    public void SetItem(SkillController item)
    {
        itemInSlot = item;
    }

    public void RemoveItem()
    {
        itemInSlot = null;
    }
}