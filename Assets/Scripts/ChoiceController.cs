using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChoiceController : MonoBehaviour
{
    public enum ChoiceType { trade, repel, sleep };
    private ChoiceType choice = ChoiceType.sleep;
    private Animator _anim;

    public Text title;
    public Text description;

    private InteractiveObject npc;

    public GameObject tradeArrows;
    public List<Image> tradeItems; //0 left; 1 right
    public List<Text> tradeItemsCounters; //0 left; 1 right
    string tradeItemLeft = "Skill"; // Skill, Candy, Pill, Health
    string tradeItemRight = "Candy";
    int tradeItemLeftAmount = 0;
    int tradeItemRightAmount = 0;

    SkillController leftSkillToSell;
    SkillController rightSkillToSell;

    public List<Sprite> itemTradeSprites; // 0 Pill, 1 Candy, 2 Health

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    void GenerateTradeWindow()
    {
        rightSkillToSell = null;
        tradeArrows.SetActive(true);

        float pillChanceRight = 0;
        float skillChanceRight = 0;
        float candyChanceRight = 0;

        // check players needs. gets right item
        if (GameManager.Instance.inventoryController.pills < 4)
        {
            pillChanceRight = 0.6f;
            skillChanceRight = 0.2f;
            candyChanceRight = 0.2f;
        }
        else if (GameManager.Instance.skillsCurrent.Count < GameManager.Instance.objectList.Count / 2 && GameManager.Instance.skillsCurrent.Count < 8)
        {
            pillChanceRight = 0.2f;
            skillChanceRight = 0.6f;
            candyChanceRight = 0.2f;
        }
        else
        {
            pillChanceRight = 0.2f;
            skillChanceRight = 0.2f;
            candyChanceRight = 0.6f;
        }

        float pillV = pillChanceRight;
        float skillV = pillV + skillChanceRight;
        float candyV = skillV + candyChanceRight;

        float random = Random.Range(0f, 1f);

        if (random <= pillV) // right item = pill
        {
            tradeItemRight = "Pill";
            tradeItemRightAmount = Random.Range(1, 3);
        }
        else if (random <= skillV)// right item = skill
        {
            tradeItemRight = "Skill";
            tradeItemRightAmount = 1;
        }
        else // right item = candies
        {
            tradeItemRight = "Candy";
            tradeItemRightAmount = Random.Range(1, 4);
        }

        // LEFT ITEM
        if (GameManager.Instance.inventoryController.candies > 3)
        {
            tradeItemLeft = "Candy";
            tradeItemLeftAmount = Random.Range(1, 4);
        }
        else if (GameManager.Instance.skillsCurrent.Count > 1)
        {
            tradeItemLeft = "Skill";
            tradeItemLeftAmount = 1;
        }
        else if (GameManager.Instance.inventoryController.pills > 2)
        {
            tradeItemLeft = "Pill";
            tradeItemLeftAmount = Random.Range(1, 3);
        }
        else
        {
            tradeItemLeft = "Health";
            tradeItemLeftAmount = Random.Range(1, Mathf.RoundToInt(GameManager.Instance.player.health / 0.25f));
        }

        switch (tradeItemLeft)
        {
            case "Pill":
                tradeItems[0].sprite = itemTradeSprites[0];
                if (tradeItemLeftAmount > 1)
                    tradeItemsCounters[0].text = tradeItemLeftAmount + "";
                else
                    tradeItemsCounters[0].text = "";
                break;

            case "Candy":
                tradeItems[0].sprite = itemTradeSprites[1];
                if (tradeItemLeftAmount > 1)
                    tradeItemsCounters[0].text = tradeItemLeftAmount + "";
                else
                    tradeItemsCounters[0].text = "";
                break;

            case "Health":
                tradeItems[0].sprite = itemTradeSprites[2];
                if (tradeItemLeftAmount > 1)
                    tradeItemsCounters[0].text = tradeItemLeftAmount + "";
                else
                    tradeItemsCounters[0].text = "";
                break;

            case "Skill":
                int skillIndex = Random.Range(0, GameManager.Instance.skillsCurrent.Count);
                tradeItems[0].sprite = GameManager.Instance.inventoryController.items[skillIndex].skillSprite;
                leftSkillToSell = GameManager.Instance.inventoryController.items[skillIndex];
                tradeItemsCounters[0].text = "";
                break;

            default:
                tradeItems[0].sprite = itemTradeSprites[1];
                tradeItemsCounters[0].text = "";
                break;
        }

        switch (tradeItemRight)
        {
            case "Pill":
                tradeItems[1].sprite = itemTradeSprites[0];
                if (tradeItemLeftAmount > 1)
                    tradeItemsCounters[1].text = tradeItemLeftAmount + "";
                else
                    tradeItemsCounters[1].text = "";
                break;

            case "Candy":
                tradeItems[1].sprite = itemTradeSprites[1];
                if (tradeItemLeftAmount > 1)
                    tradeItemsCounters[1].text = tradeItemLeftAmount + "";
                else
                    tradeItemsCounters[1].text = "";
                break;

            case "Health":
                tradeItems[1].sprite = itemTradeSprites[2];
                if (tradeItemLeftAmount > 1)
                    tradeItemsCounters[1].text = tradeItemLeftAmount + "";
                else
                    tradeItemsCounters[1].text = "";
                break;

            case "Skill":
                int skillIndex = Random.Range(0, npc.npcControl.skills.Count);
                tradeItems[1].sprite = npc.npcControl.skills[skillIndex].GetComponent<SkillController>().skillSprite;
                rightSkillToSell = npc.npcControl.skills[skillIndex].GetComponent<SkillController>();
                tradeItemsCounters[1].text = "";
                break;

            default:
                tradeItems[1].sprite = itemTradeSprites[1];
                tradeItemsCounters[1].text = "";
                break;
        }
    }
    public void ShowWindow(InteractiveObject _npc, bool sleepNearEnemy, bool outOfPills, bool trade, bool repel)
    {
        tradeArrows.SetActive(false);

        if (!sleepNearEnemy && !outOfPills)
        {
            npc = _npc;
            if (trade) // trade RANDOM ITEMS HERE
            {
                choice = ChoiceType.trade;
                title.text = npc._name + " offers a trade:";
                description.text = "";
                GenerateTradeWindow();
            }
            else if (repel) //repel RIDDLE HERE
            {
                choice = ChoiceType.repel;
                title.text = "Yes or no?";
                description.text = GameManager.Instance._riddleController.activeRiddle;
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
            case (ChoiceType.trade):
                
                // RIGHT
                if (tradeItemRight == "Skill" && rightSkillToSell != null) // get skill
                {
                    if (GameManager.Instance.skillsCurrent.Count < 8)
                    {
                        GameManager.Instance.skills.Add(rightSkillToSell.gameObject);
                        GameManager.Instance.inventoryController.ItemGet(rightSkillToSell);
                    }
                    else
                    {

                    }
                    npc.npcControl.RemoveSkill(rightSkillToSell);
                }
                else if (tradeItemRight == "Pill")
                {
                    GameManager.Instance.inventoryController.PillGet(tradeItemRightAmount);
                }
                else if (tradeItemRight == "Candy")
                {
                    GameManager.Instance.inventoryController.CandyGet(tradeItemRightAmount);
                }
                
                // LEFT
                if (tradeItemLeft == "Skill" && leftSkillToSell != null) // lose skill
                {
                    print(leftSkillToSell.skillName);
                    GameManager.Instance.skills.Remove(leftSkillToSell.gameObject);
                    GameManager.Instance.inventoryController.ItemLost(leftSkillToSell);
                }
                else if (tradeItemLeft == "Pill")
                {
                    GameManager.Instance.inventoryController.PillLose(tradeItemLeftAmount);
                }
                else if (tradeItemLeft == "Candy")
                {
                    GameManager.Instance.inventoryController.CandyLose(tradeItemLeftAmount);
                }


                break;

            case (ChoiceType.repel):
                /* make repel
                GameManager.Instance.inventoryController.CandyLose(npc.calmMoney);
                GameManager.Instance.inventoryController.ItemLost(npc.calmItem.GetComponent<SkillController>());
                npc.npcControl.agressiveTo = NpcController.Target.none;
                npc.actionOnDialog = InteractiveObject.DialogAction.none;
                */
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
        GameManager.Instance.ChoiceInactive(choice, true, npc);
        _anim.SetTrigger("Yes");
    }

    public void No()
    {
        GameManager.Instance.ChoiceInactive(choice, false, npc);
        _anim.SetTrigger("No");
    }

    public void PointerEnterButton(string itemSide)
    {
        string sendDescription = "";
        switch (itemSide)
        {
            case "Left":
                if (tradeItemLeft == "Candy")
                    sendDescription = "Candy. Valuable sweet.";
                else if (tradeItemLeft == "Pill")
                    sendDescription = "Pill. Use it to sleep well.";
                else if (tradeItemLeft == "Skill")
                    sendDescription = leftSkillToSell.description;
                break;

            case "Right":
                if (tradeItemRight == "Candy")
                    sendDescription = "Candy. Valuable sweet.";
                else if (tradeItemRight == "Pill")
                    sendDescription = "Pill. Use it to sleep well.";
                else if (tradeItemRight == "Skill")
                    sendDescription = rightSkillToSell.description;
                break;

            default:
                sendDescription = "Item for trade.";
                break;
        }

        GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, false, true);
        GameManager.Instance.mouseOverButton = true;
    }

    public void PointerExitButton()
    {
        GameManager.Instance.mouseOverButton = false;
        GameManager.Instance.HideTextManually();
    }
}