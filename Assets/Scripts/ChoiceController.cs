using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChoiceController : MonoBehaviour
{
    public enum ChoiceType { trade, repel, sleep };
    private ChoiceType choice = ChoiceType.sleep;
    private Animator _anim;

    public Text title;
    public Text description;

    private InteractiveObject npc;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void ShowWindow(InteractiveObject _npc, bool sleepNearEnemy, bool outOfPills, bool trade, bool repel)
    {
        if (!sleepNearEnemy && !outOfPills)
        {
            npc = _npc;
            if (trade) // trade RANDOM ITEMS HERE
            {
                choice = ChoiceType.trade;
                title.text = npc._name + " offers a trade";
                description.text = "Trade description";
            }
            else if (repel) //repel RIDDLE HERE
            {
                choice = ChoiceType.repel;
                title.text = "Riddle title";
                description.text = "Riddle description";
            }
        }
        else if (sleepNearEnemy && !outOfPills)
        {
            choice = ChoiceType.sleep;
            npc = null;

            title.text = "Monsters around.";
            description.text = "Monsters will scare you and damage your toy while you're sleeping. Fall asleep?";
        }
        else if (!sleepNearEnemy && outOfPills)
        {
            choice = ChoiceType.sleep;
            npc = null;

            title.text = "No pills.";
            description.text = "The next night will be much worse. Fall asleep?";
        }
        else if (sleepNearEnemy && outOfPills)
        {
            choice = ChoiceType.sleep;
            npc = null;

            title.text = "Monsters and no pills";
            description.text = "You're going to bed with the monsters around. And you run out of pills. This is madness. Fall asleep?";
        }

        _anim.SetTrigger("Active");
    }

    public void Yes()
    {
        switch (choice)
        {
            case (ChoiceType.repel):
                /* make repel
                GameManager.Instance.inventoryController.CandyLose(npc.calmMoney);
                GameManager.Instance.inventoryController.ItemLost(npc.calmItem.GetComponent<SkillController>());
                npc.npcControl.agressiveTo = NpcController.Target.none;
                npc.actionOnDialog = InteractiveObject.DialogAction.none;
                */
                break;

            case (ChoiceType.trade):
                // make trade
                break;

            case (ChoiceType.sleep):
                //sleep damage need to fix
                GameManager.Instance.FrenzyDamage(25f);
                GameManager.Instance.player.Damage(0.25f * GameManager.Instance.player.maxHealth, GameManager.Instance.player);
                break;

            default:
                //something
                break;
        }
        GameManager.Instance.ChoiceInactive(choice, true);
        _anim.SetTrigger("Yes");
    }

    public void No()
    {
        GameManager.Instance.ChoiceInactive(choice, false);
        _anim.SetTrigger("No");
    }
}