using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour {

    public List<SkillController> items = new List<SkillController>();

    public List<InventorySlotController> slots;

    public List<GameObject> uniqueItemsDropped = new List<GameObject>();

    public int money = 0;

    void Start()
    {
        foreach (GameObject skill in GameManager.Instance.skillsCurrent)
        {
            SkillController skillController = skill.GetComponent<SkillController>();
            ItemGet(skillController);
        }
    }

    public void MoneyGet(int amount)
    {
        money += amount;
    }

    public void MoneyLose(int amount)
    {
        money -= amount;
    }

    public void ItemGet(SkillController skill)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].itemInSlot == null)
            {
                Image slotImage = slots[i].GetComponent<Image>();
                slotImage.sprite = skill.skillSprite;
                slotImage.color = Color.white;
                items.Add(skill);
                slots[i].SetItem(skill);
                break;
            }
        }
    }

    public void ItemLost(SkillController item)
    {
        foreach (InventorySlotController slot in slots)
        {
            if (slot.itemInSlot == item)
            {
                GameManager.Instance.inventory.SetTrigger("Update");
                slot.RemoveItem();
                items.Remove(item);
                SortSlots();
                break;
            }
        }
    }

    public void DeleteItem(int slotNumber)
    {
        if (items.Count > slotNumber)
        {
            GameManager.Instance.inventory.SetTrigger("Update");
            slots[slotNumber].RemoveItem();
            slots[slotNumber].GetComponent<Image>().color = Color.clear;
            items.RemoveAt(slotNumber);
            GameManager.Instance.skills_1.RemoveAt(slotNumber);
        }
            SortSlots();
    }

    public void SortSlots()
    {
        for (int i = 0; i < 4; i++)
        {
            if (slots[i].itemInSlot == null && slots[i + 1].itemInSlot != null)
            {
                slots[i].SetItem(slots[i + 1].itemInSlot);
                slots[i].GetComponent<Image>().color = Color.white;
                slots[i].GetComponent<Image>().sprite = slots[i].itemInSlot.skillSprite;
                slots[i].itemInSlot.transform.position = slots[i].transform.position;

                slots[i + 1].RemoveItem();
                slots[i + 1].GetComponent<Image>().color = Color.clear;
            }
        }
    }

    public void PointerEnterButton(int skill)
    {
        if (skill >= 0 && slots[skill].itemInSlot != null)
        {
            string sendDescription = GameManager.Instance.skillsCurrent[skill].GetComponent<SkillController>().description;
            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, false, true);
        }
        GameManager.Instance.mouseOverButton = true;
    }

    public void PointerExitButton()
    {
        GameManager.Instance.mouseOverButton = false;
        GameManager.Instance.HideTextManually();
    }
}