using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class NpcDropFeedbackController : MonoBehaviour
{

    public GameObject candiesGo;
    public GameObject trashGo;

    public Text candiesCounter;
    public Text trashCounter;
    public void SetValues(int candies, int trash)
    {
        if (candies > 0)
        {
            candiesCounter.text = "" + candies;
        }
        else
        {
            candiesGo.SetActive(false);
        }

		if (trash > 0)
		{
			trashCounter.text = "" + trash;
		}
		else
		{
			trashGo.SetActive(false);
		}

		Destroy(gameObject, 2f);
    }
}
