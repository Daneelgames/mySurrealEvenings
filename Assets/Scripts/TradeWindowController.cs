using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TradeWindowController : MonoBehaviour {

    public NpcController npc;

    public List<SkillController> items = new List<SkillController>();

    public List<InventorySlotController> slots;
    public List<Animator> slotAnimators;

    public Animator _anim;

    public int emptySlots = 5;

    public void OpenTradeWindow()
    {
        //clear lists
        items.Clear();
        foreach (InventorySlotController slot in slots)
        {
            slot.itemInSlot = null;

            slot.RemoveItem();
            slot.GetComponent<Image>().color = Color.clear;
        }

        foreach (GameObject skill in npc.skills)
        {
            SkillController skillController = skill.GetComponent<SkillController>();
            ItemGet(skillController);
        }
        GetEmptySlots();
    }

    void GetEmptySlots()
    {
        emptySlots = 5 - items.Count;

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

    public void PointerEnterButton(int skill)
    {
        if (skill < items.Count)
        {
            string sendDescription = "";

            SkillController skillToSell = items[skill];
            int price = skillToSell.price;

            sendDescription = skillToSell.description + " Buy for " + price + " moneye.";

            if (items.Count > 1 && GameManager.Instance.inventoryController.candies >= items[skill].price && GameManager.Instance.inventoryController.emptySlots > 0)
                slotAnimators[skill].SetBool("ShowIcon", true);

            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, false, true);

        }
        GameManager.Instance.mouseOverButton = true;
    }

    public void PointerExitButton()
    {
        GameManager.Instance.mouseOverButton = false;
        GameManager.Instance.HideTextManually();

        foreach (Animator anim in slotAnimators)
        {
            anim.SetBool("ShowIcon", false);
        }
    }

    public void BuyItem(int item)
    {
        _anim.SetTrigger("Update");
        
        GameManager.Instance.skills_1.Add(items[item].gameObject);
        GameManager.Instance.inventoryController.ItemGet(items[item]);
        GameManager.Instance.inventoryController.CandyLose(items[item].price);

        slots[item].RemoveItem();
        slots[item].GetComponent<Image>().color = Color.clear;
        npc.skills.RemoveAt(item);
        items.RemoveAt(item);

        SortSlots();
    }

    public void SellItem(SkillController item)
    {
        npc.skills.Add(item.gameObject);
        OpenTradeWindow();
        _anim.SetTrigger("Update");
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
}