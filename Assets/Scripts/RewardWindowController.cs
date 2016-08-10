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
    public void SetReward(int moneyDrop, bool keyDrop, GameObject treasure)
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
            rewardsAmount += 1;
        if (keyDrop)
            rewardsAmount += 1;
        if (treasure != null)
            rewardsAmount += 1;

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
                else if (treasure != null)
                {
                    rewardPlaces[2].sprite = treasure.GetComponent<SpriteRenderer>().sprite;
                }
                break;
            case 2:
                if (moneyDrop > 0 && keyDrop)
                {
                    rewardPlaces[1].sprite = moneySprite;
                    rewardCounters[1].text = "" + moneyDrop;

                    rewardPlaces[3].sprite = keySprite;
                }
                else if (moneyDrop > 0 && treasure != null)
                {
                    rewardPlaces[1].sprite = moneySprite;
                    rewardCounters[1].text = "" + moneyDrop;

                    rewardPlaces[3].sprite = treasure.GetComponent<SpriteRenderer>().sprite;
                }
                else if (keyDrop && treasure != null)
                {
                    rewardPlaces[1].sprite = keySprite;

                    rewardPlaces[3].sprite = treasure.GetComponent<SpriteRenderer>().sprite;
                }
                break;
            case 3:
                rewardPlaces[0].sprite = moneySprite;
                rewardCounters[0].text = "" + moneyDrop;

                rewardPlaces[2].sprite = keySprite;

                rewardPlaces[4].sprite = treasure.GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }
    public void ToggleWindow(bool active)
    {
        anim.SetBool("Active", active);
    }
}