using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{

    public List<SkillController> items = new List<SkillController>();

    public List<InventorySlotController> slots;
    public List<Animator> slotAnimators;

    public List<GameObject> uniqueItemsDropped = new List<GameObject>();

    public int candies = 0;
    public int pills = 0;

    public Text candyCounter;
    public Text pillsCounter;

    [SerializeField]
    private Sprite sellIcon;
    [SerializeField]
    private Sprite trashIcon;

    public int emptySlots = 5;

    public int maxItems = 8;

    void GetEmptySlots()
    {
        emptySlots = 5;

        foreach (InventorySlotController slot in slots)
        {
            if (slot.itemInSlot != null)
                emptySlots -= 1;
        }
    }

    void Start()
    {
        foreach (GameObject skill in GameManager.Instance.skillsCurrent)
        {
            SkillController skillController = skill.GetComponent<SkillController>();
            ItemGet(skillController);
        }

        SetResourcesFeedback();
    }

    public void CandyGet(int amount)
    {
        candies += amount;
        SetResourcesFeedback();
    }

    public void CandyLose(int amount)
    {
        candies -= amount;
        SetResourcesFeedback();
    }

    public void TrashGet(int amount)
    {
        pills += amount;
        SetResourcesFeedback();
    }

    public void TrashLose(int amount)
    {
        pills -= amount;
        SetResourcesFeedback();
    }

    public void SetResourcesFeedback()
    {
        candyCounter.text = "" + candies;
        pillsCounter.text = "" + pills;
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
                slot.GetComponent<Image>().color = Color.clear;
                items.Remove(item);
                GameManager.Instance.skills.Remove(item.gameObject);
                SortSlots();
                break;
            }
        }
    }

    public void DeleteItem(int slotNumber) // USED FOR SELLING AND THROW ITEMS AWAY
    {
        if (items.Count > slotNumber)
        {
            /*
            if (GameManager.Instance.tradeActive)
            {
                GameManager.Instance.tradeController.SellItem(slots[slotNumber].itemInSlot);

                int moneyGet = 0;
                if (slots[slotNumber].itemInSlot.price > 0)
                {
                    moneyGet = Mathf.RoundToInt(slots[slotNumber].itemInSlot.price / 2);
                    if (moneyGet < 1)
                        moneyGet = 1;
                }
                int trashGet = 0;
                if (slots[slotNumber].itemInSlot.priceTrash > 0)
                {
                    trashGet = Mathf.RoundToInt(slots[slotNumber].itemInSlot.priceTrash / 2);
                    print(trashGet + " trash");
                    if (trashGet < 1)
                        trashGet = 1;
                }

                //print("sell for " + moneyGet);

                CandyGet(moneyGet);
                TrashGet(trashGet);
            }
            */

            GameManager.Instance.inventory.SetTrigger("Update");
            slots[slotNumber].RemoveItem();
            slots[slotNumber].GetComponent<Image>().color = Color.clear;
            items.RemoveAt(slotNumber);
            GameManager.Instance.skills.RemoveAt(slotNumber);
        }
        SortSlots();
    }

    public void SortSlots()
    {
        for (int i = 0; i < maxItems - 1; i++)
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
            if (!GameManager.Instance.choiceActive)
            {
                if (items.Count > 1)
                    slotAnimators[skill].SetBool("ShowTrash", true);

                slots[skill].transform.FindChild("DeleteSkillIcon").GetComponent<Image>().sprite = trashIcon;
            }
            string sendDescription = "";

            sendDescription = GameManager.Instance.skillsCurrent[skill].GetComponent<SkillController>().description;


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
            anim.SetBool("ShowTrash", false);
        }
    }
}