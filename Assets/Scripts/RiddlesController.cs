using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiddlesController : MonoBehaviour
{

    public List<int> rightAnswers;
    public int riddlesCurrentAmount = 3;
    public List<string> riddlesCurrent;
    public List<bool> answersCurrent;

    public int riddlesAllAmount = 10;
    public List<string> riddlesAll;
    public List<bool> answersAll;

    private List<string> riddlesDynamicList;
    private List<bool> answersDynamicList;

    public void GenerateRiddles()
    {
        for (int i = 0; i < riddlesAllAmount; i++)
        {
            float random = Random.value;

            if (random > 0.5f)
                answersAll[i] = true;
            else
                answersAll[i] = false;
        }

        riddlesDynamicList = new List<string>(riddlesAll);
        answersDynamicList = new List<bool>(answersAll);

        GenerateCurrentRiddles();

    }

    void GenerateCurrentRiddles()
    {
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, riddlesDynamicList.Count);
            riddlesCurrent.Add(riddlesDynamicList[randomIndex]);
            answersCurrent.Add(answersDynamicList[randomIndex]);

            riddlesDynamicList.RemoveAt(randomIndex);
			answersDynamicList.RemoveAt(randomIndex);

            rightAnswers.Add(0);
        }
    }

    public void UpdateCurrentRiddles()
    {
		
    }
}