using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DayController : MonoBehaviour
{

    public List<GameObject> crosses;

    public Text candyCounter;
    public Text trashCounter;
    public Animator candyCounterAnimator;
    public Animator trashCounterAnimator;

    public Animator toyButtonAnimator;
    public Animator childButtonAnimator;

    public Text toyFeedbackText;
    public Text childFeedbackText;
    public Animator feedbackAnimator;
    public DayEventsController dayEvents;
    public void NightOver()
    {
        UpdateTrash();
        UpdateCandy();
        UpdateToy();
        UpdateChild();

        int curDay = GameManager.Instance.stageRandomController.curStageIndex - 1;
        
        dayEvents.GenerateEvents();

        crosses[curDay].SetActive(true);
    }

    public void UpdateToy()
    {
        feedbackAnimator.SetTrigger("UpdateToy");

        float healthPercent = GameManager.Instance.player.health / GameManager.Instance.player.maxHealth;
        string name = GameManager.Instance.player._name;
        string newText = "";
        if (healthPercent >= 0.9f)
        {
            newText = name + " is not damaged.";
        }
        else if (healthPercent >= 0.75f && healthPercent < 0.9f)
        {
            newText = name + " is almost as good as new!";
        }
        else if (healthPercent < 0.75f && healthPercent >= 0.5f)
        {
            newText = name + " is a bit battered.";
        }
        else if (healthPercent < 0.5f && healthPercent >= 0.25f)
        {
            newText = "Need to fix " + name + ".";
        }
        else if (healthPercent < 0.25f && healthPercent >= 0.1f)
        {
            newText = name + " almost broke.";
        }
        else if (healthPercent < 0.1f)
        {
            newText = name + " is CRITICAL !!!";
        }

        toyFeedbackText.text = newText;
    }

    public void UpdateChild()
    {
        feedbackAnimator.SetTrigger("UpdateChild");

        float sanityPercent = GameManager.Instance.curSanity / 100;
        string newText = "";
        if (sanityPercent >= 0.9f)
        {
            newText = "I'm perfectly fine!";
        }
        else if (sanityPercent >= 0.75f && sanityPercent < 0.9f)
        {
            newText = "I feel almost great!";
        }
        else if (sanityPercent < 0.75f && sanityPercent >= 0.5f)
        {
            newText = "I'm ok.";
        }
        else if (sanityPercent < 0.5f && sanityPercent >= 0.25f)
        {
            newText = "I'm scared...";
        }
        else if (sanityPercent < 0.25f && sanityPercent >= 0.1f)
        {
            newText = "I'm trembling!";
        }
        else if (sanityPercent < 0.1f)
        {
            newText = "I AM TERRIFIED";
        }
        childFeedbackText.text = newText;
    }

    public void UpdateTrash()
    {
        trashCounterAnimator.SetTrigger("Update");
        trashCounter.text = "" + GameManager.Instance.inventoryController.pills;
    }
    public void UpdateCandy()
    {
        candyCounterAnimator.SetTrigger("Update");
        candyCounter.text = "" + GameManager.Instance.inventoryController.candies;
    }

    public void HealToy()
    {
        if (GameManager.Instance.player.health / GameManager.Instance.player.maxHealth < 0.9f && GameManager.Instance.inventoryController.pills > 0)
        {
            float healAmount = Random.Range(0.75f, 1.25f);
            GameManager.Instance.player.Recover(healAmount);

            GameManager.Instance.inventoryController.PillLose(1);

            UpdateToy();
            UpdateTrash();
            toyButtonAnimator.SetTrigger("Active");
        }
    }

    public void HealChild()
    {
        if (GameManager.Instance.curSanity / 100 < 0.9f && GameManager.Instance.inventoryController.candies > 0)
        {
            float healAmount = Random.Range(7.5f, 12.5f);
            if (100 - GameManager.Instance.curSanity >= healAmount)
                GameManager.Instance.curSanity += healAmount;
            else
                GameManager.Instance.curSanity = 100;

            GameManager.Instance.inventoryController.CandyLose(1);

            UpdateChild();
            UpdateCandy();
            childButtonAnimator.SetTrigger("Active");
        }
    }
}
