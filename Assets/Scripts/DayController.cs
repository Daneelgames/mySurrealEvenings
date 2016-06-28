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

    public Text toyFeedbackText;
    public Text childFeedbackText;
    public Animator feedbackAnimator;

    public void NightOver()
    {
        UpdateTrash();
        UpdateCandy();

        int curDay = GameManager.Instance.stageRandomController.curStageIndex - 1;

        crosses[curDay].SetActive(true);
    }

    void UpdateToy()
    {
        feedbackAnimator.SetTrigger("UpdateToy");

		float healthPercent = GameManager.Instance.party[0].health / GameManager.Instance.party[0].maxHealth;
		string name = GameManager.Instance.party[0]._name;
		string newText = "";
		if (healthPercent >= 0.75f)
		{
			newText = name + " as good as new!";
		}
		else if (healthPercent < 0.75f && healthPercent >= 0.5f)
		{
			newText = name + " is fine.";
		}
		else if (healthPercent < 0.5f && healthPercent >= 0.25f)
		{
			newText =  "Need to fix " + name + ".";
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

    void UpdateChild()
    {
        feedbackAnimator.SetTrigger("UpdateChild");

		float sanityPercent = GameManager.Instance.curSanity / 100;
		string newText = "";
		if (sanityPercent >= 0.75f)
		{
			newText = "I feel great!";
		}
		else if (sanityPercent < 0.75f && sanityPercent >= 0.5f)
		{
			newText = "i'm ok";
		}
		else if (sanityPercent < 0.5f && sanityPercent >= 0.25f)
		{
			newText =  "I'm scared...";
		}
		else if (sanityPercent < 0.25f && sanityPercent >= 0.1f)
		{
			newText = "I'm trembling!";
		}
		else if (sanityPercent < 0.1f)
		{
			newText = "I AM TERRIFIED";
		}
		toyFeedbackText.text = newText;
    }

    void UpdateTrash()
    {
        trashCounterAnimator.SetTrigger("Update");
        trashCounter.text = "" + GameManager.Instance.inventoryController.trash;
    }
    void UpdateCandy()
    {
        candyCounterAnimator.SetTrigger("Update");
        candyCounter.text = "" + GameManager.Instance.inventoryController.candies;
    }
}
