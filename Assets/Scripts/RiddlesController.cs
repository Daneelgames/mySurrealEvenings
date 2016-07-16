using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RiddlesController : MonoBehaviour
{

    public int activeRiddle = 0;
    public bool activeAnswer = true;

    public List<int> rightAnswers;
    public int riddlesCurrentAmount = 5;
    public List<string> riddlesCurrent;
    public List<bool> answersCurrent;

    public int riddlesAllAmount = 0;
    public List<string> riddlesAll;
    public List<bool> answersAll;

    public List<string> riddlesDynamicList;
    private List<bool> answersDynamicList;

    public void GenerateRiddles()
    {
        riddlesAllAmount = riddlesAll.Count;

        for (int i = 0; i < riddlesAllAmount; i++)
        {
            float random = Random.value;

            if (random > 0.5f)
                answersAll.Add(true);
            else
                answersAll.Add(false);
        }

        GenerateCurrentRiddles();
    }

    void GenerateCurrentRiddles()
    {
        riddlesDynamicList = new List<string>(riddlesAll);
        answersDynamicList = new List<bool>(answersAll);

        riddlesCurrent.Clear();
        answersCurrent.Clear();
        rightAnswers.Clear();

        for (int i = 0; i < riddlesCurrentAmount; i++)
        {
            int randomIndex = Random.Range(0, riddlesDynamicList.Count);
            riddlesCurrent.Add(riddlesDynamicList[randomIndex]);
            answersCurrent.Add(answersDynamicList[randomIndex]);

            riddlesDynamicList.RemoveAt(randomIndex);
            answersDynamicList.RemoveAt(randomIndex);

            rightAnswers.Add(0);
        }
    }

    public void UpdateCurrentRiddles() // call this on correct answer
    {
        rightAnswers[activeRiddle] += 1;

        if (rightAnswers[activeRiddle] > 4) // if answered 5 times
        {
            riddlesCurrent.RemoveAt(activeRiddle);
            answersCurrent.RemoveAt(activeRiddle);

            if (riddlesDynamicList.Count <= 0)
            {
                //print("Shuffle");
                GenerateCurrentRiddles();
            }
            else
            {
                int randomIndex = Random.Range(0, riddlesDynamicList.Count);

                riddlesCurrent.Add(riddlesDynamicList[randomIndex]);
                answersCurrent.Add(answersDynamicList[randomIndex]);

                riddlesDynamicList.RemoveAt(randomIndex);
                answersDynamicList.RemoveAt(randomIndex);

                rightAnswers[activeRiddle] = 0;
            }
            //print(riddlesDynamicList.Count);
        }
    }

    public void SetActiveRiddle()
    {
        int random = Random.Range(0, riddlesCurrent.Count);
        activeRiddle = random;
        activeAnswer = answersCurrent[random];
    }
}