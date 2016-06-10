using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageRandomController : MonoBehaviour {

    public int curStageIndex = 0;
    
    public float stageDifficulty = 0;

    public List<NpcController> npcList;

    public List<GameObject> backgrounds;

    public void BuildStage()
    {
        curStageIndex += 1;

        SetDifficulty();

        SpawnNpc();

        SetBackground();
    }

    void SetDifficulty() // based on levelIndex + random
    {
        switch (curStageIndex)
        {
            // I ACT
            case 1:
                stageDifficulty = 1;
                break;
            case 2:
                stageDifficulty = Random.Range(1f, 2f);
                break;
            case 3:
                stageDifficulty = Random.Range(2f, 4f);
                break;
            case 4:
                stageDifficulty = Random.Range(3f, 4f);
                break;
            case 5:
                print("BOSS 1 STAGE");
                stageDifficulty = 5;
                break;

            // II ACT
            case 6:
                stageDifficulty = Random.Range(3f, 5f);
                break;
            case 7:
                stageDifficulty = Random.Range(4f, 6f);
                break;
            case 8:
                stageDifficulty = Random.Range(5f, 7f);
                break;
            case 9:
                stageDifficulty = Random.Range(6f, 7f);
                break;
            case 10:
                print("BOSS 2 STAGE");
                stageDifficulty = 8;
                break;

            // III ACT
            case 11:
                stageDifficulty = Random.Range(5f, 7f);
                break;
            case 12:
                stageDifficulty = Random.Range(6f, 8f);
                break;
            case 13:
                stageDifficulty = Random.Range(7f, 9f);
                break;
            case 14:
                stageDifficulty = Random.Range(8f, 9f);
                break;
            case 15:
                print("BOSS 3 STAGE");
                stageDifficulty = 10;
                break;
        }
    }

    void SpawnNpc()
    {
        float curDif = 0;
        List<NpcController> newNpcList = new List<NpcController>(npcList);
        List<Transform> newCellsList = new List<Transform>(GameManager.Instance.npcCells);

        while (curDif < stageDifficulty)
        {
            int randomNpc = Random.Range(0, newNpcList.Count);

            if (npcList[randomNpc].overallDifficulty < stageDifficulty)
            {
                curDif += npcList[randomNpc].overallDifficulty;

                Transform randomCell = newCellsList[Random.Range(0, newCellsList.Count)];

                Instantiate(npcList[randomNpc].gameObject, randomCell.position, npcList[randomNpc].transform.rotation);

                newCellsList.Remove(randomCell);
            }
        }
    }

    void SetBackground()
    {
        Instantiate(backgrounds[Random.Range(0, backgrounds.Count)], Vector3.zero, Quaternion.identity);
    }
}