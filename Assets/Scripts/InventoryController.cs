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
                slot.RemoveItem();
                items.Remove(item);
                SortSlots();
                break;
            }
        }
    }

    public void SortSlots()
    {
        for (int i = 4; i > 0; i--)
        {
            if (slots[i].itemInSlot != null && slots[i - 1].itemInSlot == null)
            {
                slots[i - 1].SetItem(slots[i].itemInSlot);
                slots[i].RemoveItem();
                slots[i].GetComponent<Image>().color = Color.clear;
                slots[i - 1].itemInSlot.transform.position = slots[i - 1].transform.position;
            }
        }
    }

    public void PointerEnterButton(int skill)
    {
        if (skill >= 0)
        {
            print("feedback");
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