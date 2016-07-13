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

    public List<string> riddlesDynamicList;
    private List<bool> answersDynamicList;

    public void GenerateRiddles()
    {
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

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            UpdateCurrentRiddles(1);
        }
    }

    public void UpdateCurrentRiddles(int riddleIndex) // call this on correct answer
    {
        rightAnswers[riddleIndex] += 1;

        if (rightAnswers[riddleIndex] > 2) // if answered 3 times
        {
            riddlesCurrent.RemoveAt(riddleIndex);
            answersCurrent.RemoveAt(riddleIndex);

            if (riddlesDynamicList.Count <= 0)
            {
                print("Shuffle");
                GenerateCurrentRiddles();
            }
            else
            {
                int randomIndex = Random.Range(0, riddlesDynamicList.Count);

                riddlesCurrent.Add(riddlesDynamicList[randomIndex]);
                answersCurrent.Add(answersDynamicList[randomIndex]);

                riddlesDynamicList.RemoveAt(randomIndex);
                answersDynamicList.RemoveAt(randomIndex);

                rightAnswers[riddleIndex] = 0;
            }
            //print(riddlesDynamicList.Count);
        }
    }
}