using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcController : MonoBehaviour {

    public bool agressive = false;

    [SerializeField]
    InteractiveObject objectController;

    public List<GameObject> skills = new List<GameObject>();

    public int skillLevelPreffered = 1;

    public void ChooseAction()
    {
        if (!agressive)
        {
            print(objectController._name + " is lazy.");
            GameManager.Instance.SetTurn();
        }
        else
        {
            int randomSkill = Random.Range(0, skills.Count);


        }
    }
}