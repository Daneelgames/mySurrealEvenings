using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RewardWindowController : MonoBehaviour
{
    public Animator anim;
    public List<Image> rewardPlaces;
    public List<Text> rewardCounters;

    public Sprite moneySprite;
    public Sprite keySprite;

    public int money = 0;
    public bool key = false;
    public GameObject treasure = null;
    public void SetReward(int moneyDrop, bool keyDrop, GameObject treasureDrop)
    {
        foreach (Text tx in rewardCounters) // CLEAR COUNTERS
        {
            tx.text = "";
        }
        foreach (Image img in rewardPlaces) // CLEAR SPRITES
        {
            img.sprite = null;
        }

        int rewardsAmount = 0;
        if (moneyDrop > 0)
        {
            money = moneyDrop;
            rewardsAmount += 1;
        }
        if (keyDrop)
        {
            key = true;
            rewardsAmount += 1;
        }
        if (treasureDrop != null)
        {
            treasure = treasureDrop;
            rewardsAmount += 1;
        }

        switch (rewardsAmount)
        {
            case 1:
                if (moneyDrop > 0)
                {
                    rewardPlaces[2].sprite = moneySprite;
                    rewardCounters[2].text = "" + moneyDrop;
                }
                else if (keyDrop)
                {
                    rewardPlaces[2].sprite = keySprite;
                }
                else if (treasureDrop != null)
                {
                    rewardPlaces[2].sprite = treasureDrop.GetComponent<SpriteRenderer>().sprite;
                }
                break;
            case 2:
                if (moneyDrop > 0 && keyDrop)
                {
                    rewardPlaces[1].sprite = moneySprite;
                    rewardCounters[1].text = "" + moneyDrop;

                    rewardPlaces[3].sprite = keySprite;
                }
                else if (moneyDrop > 0 && treasureDrop != null)
                {
                    rewardPlaces[1].sprite = moneySprite;
                    rewardCounters[1].text = "" + moneyDrop;

                    rewardPlaces[3].sprite = treasureDrop.GetComponent<SpriteRenderer>().sprite;
                }
                else if (keyDrop && treasureDrop != null)
                {
                    rewardPlaces[1].sprite = keySprite;

                    rewardPlaces[3].sprite = treasureDrop.GetComponent<SpriteRenderer>().sprite;
                }
                break;
            case 3:
                rewardPlaces[0].sprite = moneySprite;
                rewardCounters[0].text = "" + moneyDrop;

                rewardPlaces[2].sprite = keySprite;

                rewardPlaces[4].sprite = treasureDrop.GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }
    public void ToggleWindow(bool active)
    {
        anim.SetBool("Active", active);

        if (active == false) // Window closed, get reward
        {
            GameManager.Instance.inventoryController.KeyGet();
			GameManager.Instance.inventoryController.CandyGet(money);
			// TREASURE GET
        }
    }
}