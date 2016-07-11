using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleBarController : MonoBehaviour
{

    enum PlayerAction { Attack, Defence };

    PlayerAction currentAction = PlayerAction.Attack;

    public Transform bar;
    public Animator anim;

    public Animator cross;
    public int missesAmount;

    public Animator def;
    public Animator dmg;

    BattleBarTargetController newBar;
    public void StartAttack(GameObject targets)
    {
        missesAmount = 0;
        currentAction = PlayerAction.Attack;
        anim.SetTrigger("Attack");
        GameObject attackTargets = Instantiate(targets, transform.position, Quaternion.identity) as GameObject;

        newBar = attackTargets.GetComponent<BattleBarTargetController>();
        newBar.SetBar(this);

        attackTargets.transform.SetParent(bar, false);
        attackTargets.transform.localPosition = Vector3.zero;
        attackTargets.transform.SetAsFirstSibling();
    }

    public void StartDefence(GameObject targets)
    {
        missesAmount = 0;
        currentAction = PlayerAction.Defence;
        anim.SetTrigger("Defence");
        GameObject defenceTargets = Instantiate(targets, transform.position, Quaternion.identity) as GameObject;

        newBar = defenceTargets.GetComponent<BattleBarTargetController>();
        newBar.SetBar(this);

        defenceTargets.transform.SetParent(bar, false);
        defenceTargets.transform.localPosition = Vector3.zero;
        defenceTargets.transform.localScale = new Vector3(-1, 1, 1);
        defenceTargets.transform.SetAsFirstSibling();
    }

    public void Click()
    {
        cross.SetTrigger("Hit");
        newBar.CheckHit(cross.transform.position);
    }

    public void TargetHit(float dmgPercent)
    {
        float totalDamage = 0;
        totalDamage = GameManager.Instance.skillInAction.damageTarget / 100 * dmgPercent - 10 * missesAmount;
        if (totalDamage < 0)
            totalDamage = 10;

        if (currentAction == PlayerAction.Attack)
        {
            dmg.SetTrigger("Update");
            GameManager.Instance.attackTarget.ReduceHealth(totalDamage);
        }
        else
        {
            def.SetTrigger("Update");
            GameManager.Instance.attackTarget.ReturnHealth(totalDamage);
        }
    }
}