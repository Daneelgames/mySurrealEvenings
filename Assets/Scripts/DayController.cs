using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DayController : MonoBehaviour {

	public List<GameObject> crosses; 

	public Text candyCounter;
	public Text trashCounter;
	public Animator candyCounterAnimator;
	public Animator trashCounterAnimator;

	public void NightOver()
	{
		UpdateTrash();
		UpdateCandy();
		
		int curDay = GameManager.Instance.stageRandomController.curStageIndex - 1;

		crosses[curDay].SetActive(true);
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
