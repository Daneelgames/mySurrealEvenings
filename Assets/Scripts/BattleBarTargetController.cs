using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleBarTargetController : MonoBehaviour
{

    public List<Animator> targets;
    public List<float> damagePercentages;

    BattleBarController barController;

    public GameObject crossMiss;


    public void SetBar(BattleBarController _bar)
    {
        barController = _bar;
    }


    public void CheckHit(Vector2 crossPos)
    {
        bool hit = false;
        for (int i = 0; i < targets.Count; i++)
        {
            float distance = Vector2.Distance(crossPos, targets[i].transform.position);

            if (distance < 2)
            {
                hit = true;
                targets[i].SetTrigger("Hit");

                barController.TargetHit(damagePercentages[i]);

                damagePercentages.RemoveAt(i);
                targets.RemoveAt(i);
                break;
            }
        }
        if (!hit)
        {

            barController.missesAmount += 1;

            GameObject miss = Instantiate(crossMiss, crossPos, Quaternion.identity) as GameObject;

            miss.transform.SetParent(transform, false);
            miss.transform.position = crossPos;

            var euler = miss.transform.eulerAngles;
            euler.z = Random.Range(0.0f, 360.0f);
            miss.transform.eulerAngles = euler;
        }
    }
}
