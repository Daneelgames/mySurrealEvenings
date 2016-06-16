using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageRandomController : MonoBehaviour {

    public int curStageIndex = 0;
    
    public float stageDifficulty = 0;

    public List<NpcController> npcList;

    public void BuildStage()
    {
        curStageIndex += 1;

        SetDifficulty();

        SpawnNpc();
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

        List<NpcController> npcOnStage = new List<NpcController>();

        while (curDif < stageDifficulty && newCellsList.Count > 0)
        {
            int randomNpc = Random.Range(0, newNpcList.Count);

            if (npcList[randomNpc].overallDifficulty < stageDifficulty)
            {
                curDif += newNpcList[randomNpc].overallDifficulty;

                Transform randomCell = newCellsList[Random.Range(0, newCellsList.Count)];

                GameObject go =  Instantiate(newNpcList[randomNpc].gameObject, randomCell.position, newNpcList[randomNpc].transform.rotation) as GameObject;

                npcOnStage.Add(go.GetComponent<NpcController>());

                newCellsList.Remove(randomCell);
            }
        }

        if (curDif < stageDifficulty && newCellsList.Count == 0) // second iteration - replace lowest level npcs
        {
            npcOnStage = npcOnStage.OrderByDescending(w => w.overallDifficulty).ToList(); //sort by diff. lowlevels at the end

            for (int i = npcOnStage.Count; i > 0; i --)
            {
                int randomNpc = Random.Range(0, newNpcList.Count);
                if (npcOnStage[i].overallDifficulty < newNpcList[randomNpc].overallDifficulty)
                {
                    curDif -= npcOnStage[i].overallDifficulty;

                    GameObject go = Instantiate(newNpcList[randomNpc].gameObject, npcOnStage[i].transform.position, newNpcList[randomNpc].transform.rotation) as GameObject;

                    curDif += newNpcList[randomNpc].overallDifficulty;

                    npcOnStage.Remove(npcOnStage[i]);
                    Destroy(npcOnStage[i].gameObject);

                    npcOnStage.Add(go.GetComponent<NpcController>());
                }

                if (curDif >= stageDifficulty)
                    break;
            }
        }
    }
}