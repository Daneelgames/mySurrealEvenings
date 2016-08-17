using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StageRandomController : MonoBehaviour
{

    public int curStageIndex = 0;

    public float stageDifficulty = 0;

    public GameObject player;

    public List<NpcController> npcList;

    public List<NpcController> newNpcList = new List<NpcController>();
    public List<Transform> newCellsList = new List<Transform>();
    public List<NpcController> npcOnStage = new List<NpcController>();

    public void BuildStage()
    {
        curStageIndex = GameManager.Instance.levelMovementController.activeRoom.roomIndex;

        ClearNpc();
        SetDifficulty();

        SpawnNpc();
    }

    public void ClearNpc()
    {
        if (GameManager.Instance.objectList.Count > 0)
        {
            for (int i = GameManager.Instance.objectList.Count - 1; i >= 0; i--)
            {
                if (!GameManager.Instance.objectList[i].inParty)
                {
                    //                    print(GameManager.Instance.objectList[i].name);
                    Destroy(GameManager.Instance.objectList[i].gameObject);
                    GameManager.Instance.objectList.RemoveAt(i);
                }
            }
        }
    }

    void SetDifficulty() // based on levelIndex + random
    {
        if (!GameManager.Instance.levelMovementController.activeRoom.safeRoom)
        {
            float random = Random.value;
            if (GameManager.Instance.levelMovementController.activeRoom.spawnRate >= random)
                stageDifficulty = GameManager.Instance.levelMovementController.activeRoom.roomDifficulty;
            else
                stageDifficulty = 0;
        }
        else
            stageDifficulty = 0;
    }

    void SpawnNpc()
    {
        float curDif = 0;
        newNpcList = new List<NpcController>(npcList);
        newCellsList = new List<Transform>(GameManager.Instance.npcCells);

        npcOnStage = new List<NpcController>();

        while (curDif < stageDifficulty && newCellsList.Count > 0)
        {
            int randomNpc = Random.Range(0, newNpcList.Count);

            if (npcList[randomNpc].overallDifficulty < stageDifficulty)
            {
                curDif += newNpcList[randomNpc].overallDifficulty;

                Transform randomCell = newCellsList[Random.Range(0, newCellsList.Count)];

                GameObject go = Instantiate(newNpcList[randomNpc].gameObject, randomCell.position, newNpcList[randomNpc].transform.rotation) as GameObject;
                go.tag = "Enemy";
                npcOnStage.Add(go.GetComponent<NpcController>());

                newCellsList.Remove(randomCell);
            }
        }

        if (curDif < stageDifficulty && newCellsList.Count == 0) // second iteration - replace lowest level npcs
        {
            npcOnStage = npcOnStage.OrderByDescending(w => w.overallDifficulty).ToList(); //sort by diff. lowlevels at the end

            for (int i = npcOnStage.Count - 1; i >= 0; i--)
            {
                int randomNpc = Random.Range(0, newNpcList.Count);

                //              print("randomNpc " + randomNpc);
                //                print( "npconstage " + i);
                if (npcOnStage[i].overallDifficulty < newNpcList[randomNpc].overallDifficulty)
                {
                    curDif -= npcOnStage[i].overallDifficulty;

                    GameObject go = Instantiate(newNpcList[randomNpc].gameObject, npcOnStage[i].transform.position, newNpcList[randomNpc].transform.rotation) as GameObject;

                    curDif += newNpcList[randomNpc].overallDifficulty;
                    GameManager.Instance.objectList.Remove(npcOnStage[i].objectController);
                    Destroy(npcOnStage[i].gameObject);
                    npcOnStage.Remove(npcOnStage[i]);

                    npcOnStage.Add(go.GetComponent<NpcController>());
                    go.tag = "Enemy";
                }

                if (curDif >= stageDifficulty)
                    break;
            }
        }
    }
}