using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillList : MonoBehaviour {
    
    public List<GameObject> allSkills;

    void Start()
    {
        foreach (GameObject go in allSkills)
            print(go.name);
    }
}