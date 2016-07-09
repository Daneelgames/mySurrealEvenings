using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DayEventsController : MonoBehaviour
{
    public enum DayEventTypes { SanityHeal, SanityDamage, ToyDamage, CandiesAdd, CandiesRemove, TrashAdd, TrashRemove, Neutral };

    public DayEventTypes[] dayEvents = new DayEventTypes[] { DayEventTypes.Neutral, DayEventTypes.Neutral, DayEventTypes.Neutral };

    public List<int> timesOfDay = new List<int>();

    public List<Text> eventTexts;

    public List<string> morningTexts;
    public List<string> afternoonTexts;
    public List<string> eveningTexts;

    public List<Color> colors;

    public void GenerateEvents()
    {
        timesOfDay = new List<int> { 0, 1, 2 };
        dayEvents = new DayEventTypes[] { DayEventTypes.Neutral, DayEventTypes.Neutral, DayEventTypes.Neutral };

        ChangeSanity();
        ChangeToy();
        ChangeCandies();
        ChangeTrash();

        ProduceEvents();
    }

    void ProduceEvents()
    {
        for (int i = 0; i < 3; i++)
        {
            string newText = "";

            switch (dayEvents[i])
            {
                case DayEventTypes.SanityHeal:
                    GameManager.Instance.RecoverSanity(25f);
                    GameManager.Instance.crossesController.UpdateChild();

                    switch (i)
                    {
                        case 0:
                            newText = morningTexts[0];
                            eventTexts[0].text = newText;
                            break;
                        case 1:
                            newText = afternoonTexts[0];
                            eventTexts[1].text = newText;
                            break;
                        case 2:
                            newText = eveningTexts[0];
                            eventTexts[2].text = newText;
                            break;
                        default:
                            break;
                    }
                    eventTexts[i].color = colors[0];
                    break;

                case DayEventTypes.SanityDamage:
                    GameManager.Instance.FrenzyDamage(25f);
                    GameManager.Instance.crossesController.UpdateChild();

                    switch (i)
                    {
                        case 0:
                            newText = morningTexts[3];
                            eventTexts[0].text = newText;
                            break;
                        case 1:
                            newText = afternoonTexts[3];
                            eventTexts[1].text = newText;
                            break;
                        case 2:
                            newText = eveningTexts[3];
                            eventTexts[2].text = newText;
                            break;
                        default:
                            break;
                    }
                    eventTexts[i].color = colors[1];
                    break;

                case DayEventTypes.ToyDamage:
                    GameManager.Instance.player.Damage(0.25f * GameManager.Instance.player.maxHealth, GameManager.Instance.player);
                    GameManager.Instance.crossesController.UpdateToy();

                    switch (i)
                    {
                        case 0:
                            newText = morningTexts[6];
                            eventTexts[0].text = newText;
                            break;
                        case 1:
                            newText = afternoonTexts[6];
                            eventTexts[1].text = newText;
                            break;
                        case 2:
                            newText = eveningTexts[6];
                            eventTexts[2].text = newText;
                            break;
                        default:
                            break;
                    }
                    eventTexts[i].color = colors[1];
                    break;

                case DayEventTypes.CandiesAdd:
                    GameManager.Instance.inventoryController.CandyGet(Random.Range(1, 4));
                    GameManager.Instance.crossesController.UpdateCandy();

                    switch (i)
                    {
                        case 0:
                            newText = morningTexts[1];
                            eventTexts[0].text = newText;
                            break;
                        case 1:
                            newText = afternoonTexts[1];
                            eventTexts[1].text = newText;
                            break;
                        case 2:
                            newText = eveningTexts[1];
                            eventTexts[2].text = newText;
                            break;
                        default:
                            break;
                    }
                    eventTexts[i].color = colors[0];
                    break;

                case DayEventTypes.CandiesRemove:
                    GameManager.Instance.inventoryController.CandyLose(Random.Range(1, GameManager.Instance.inventoryController.candies));
                    GameManager.Instance.crossesController.UpdateCandy();

                    switch (i)
                    {
                        case 0:
                            newText = morningTexts[4];
                            eventTexts[0].text = newText;
                            break;
                        case 1:
                            newText = afternoonTexts[4];
                            eventTexts[1].text = newText;
                            break;
                        case 2:
                            newText = eveningTexts[4];
                            eventTexts[2].text = newText;
                            break;
                        default:
                            break;
                    }
                    eventTexts[i].color = colors[1];
                    break;

                case DayEventTypes.TrashAdd:
                    GameManager.Instance.inventoryController.TrashGet(Random.Range(1, 4));
                    GameManager.Instance.crossesController.UpdateTrash();

                    switch (i)
                    {
                        case 0:
                            newText = morningTexts[2];
                            eventTexts[0].text = newText;
                            break;
                        case 1:
                            newText = afternoonTexts[2];
                            eventTexts[1].text = newText;
                            break;
                        case 2:
                            newText = eveningTexts[2];
                            eventTexts[2].text = newText;
                            break;
                        default:
                            break;
                    }
                    eventTexts[i].color = colors[0];
                    break;

                case DayEventTypes.TrashRemove:
                    GameManager.Instance.inventoryController.TrashLose(Random.Range(1, GameManager.Instance.inventoryController.pills));
                    GameManager.Instance.crossesController.UpdateTrash();

                    switch (i)
                    {
                        case 0:
                            newText = morningTexts[5];
                            eventTexts[0].text = newText;
                            break;
                        case 1:
                            newText = afternoonTexts[5];
                            eventTexts[1].text = newText;
                            break;
                        case 2:
                            newText = eveningTexts[5];
                            eventTexts[2].text = newText;
                            break;
                        default:
                            break;
                    }
                    eventTexts[i].color = colors[1];
                    break;

                default: // neutral texts

                    switch (i)
                    {
                        case 0:
                            newText = morningTexts[Random.Range(7, 10)];
                            eventTexts[0].text = newText;
                            break;
                        case 1:
                            newText = afternoonTexts[Random.Range(7, 10)];
                            eventTexts[1].text = newText;
                            break;
                        case 2:
                            newText = eveningTexts[Random.Range(7, 10)];
                            eventTexts[2].text = newText;
                            break;
                        default:
                            break;
                    }
                    eventTexts[i].color = colors[2];
                    break;
            }
        }
    }

    void ChangeSanity()
    {
        float sanity = GameManager.Instance.curSanity;

        if (sanity < 30)  // heal
        {
            if (sanity < Random.Range(0f, 100f))
            {
                int dayTimeIndex = Random.Range(0, timesOfDay.Count); // 0 = morning,1 = afternoon, 2 = evening
                //print(timesOfDay[dayTimeIndex]);
                dayEvents[dayTimeIndex] = DayEventTypes.SanityHeal;
                timesOfDay.RemoveAt(dayTimeIndex);
            }
        }
        else if (sanity > 70) //damage
        {
            if (sanity > Random.Range(0f, 100f))
            {
                int dayTimeIndex = Random.Range(0, timesOfDay.Count); // 0 = morning,1 = afternoon, 2 = evening
                //print(timesOfDay[dayTimeIndex]);
                dayEvents[dayTimeIndex] = DayEventTypes.SanityDamage;
                timesOfDay.RemoveAt(dayTimeIndex);
            }
        }
    }
    void ChangeToy()
    {
        float toyPercentage = GameManager.Instance.player.health / GameManager.Instance.player.maxHealth;

        if (toyPercentage > 0.7) // damage
        {
            if (toyPercentage > Random.Range(0f, 1f))
            {
                int dayTimeIndex = Random.Range(0, timesOfDay.Count); // 0 = morning,1 = afternoon, 2 = evening
                //print(timesOfDay[dayTimeIndex]);
                dayEvents[dayTimeIndex] = DayEventTypes.ToyDamage;
                timesOfDay.RemoveAt(dayTimeIndex);
            }
        }
    }
    void ChangeCandies()
    {
        int candies = GameManager.Instance.inventoryController.candies;

        if (candies < 3) // add
        {
            if (candies < Random.Range(0f, 10f))
            {
                int dayTimeIndex = Random.Range(0, timesOfDay.Count); // 0 = morning,1 = afternoon, 2 = evening
                //print(timesOfDay[dayTimeIndex]);
                dayEvents[dayTimeIndex] = DayEventTypes.CandiesAdd;
                timesOfDay.RemoveAt(dayTimeIndex);
            }
        }
        else if (candies > 7) // remove
        {
            if (candies > Random.Range(0f, 10f))
            {
                int dayTimeIndex = Random.Range(0, timesOfDay.Count); // 0 = morning,1 = afternoon, 2 = evening
                //print(timesOfDay[dayTimeIndex]);
                dayEvents[dayTimeIndex] = DayEventTypes.CandiesRemove;
                timesOfDay.RemoveAt(dayTimeIndex);
            }
        }
    }
    void ChangeTrash()
    {
        if (timesOfDay.Count > 0)
        {
            int trash = GameManager.Instance.inventoryController.pills;

            if (trash < 3) // add
            {
                if (trash < Random.Range(0f, GameManager.Instance.player.maxHealth))
                {
                    int dayTimeIndex = Random.Range(0, timesOfDay.Count); // 0 = morning,1 = afternoon, 2 = evening
                    //print(timesOfDay[dayTimeIndex]);
                    dayEvents[dayTimeIndex] = DayEventTypes.TrashAdd;
                    timesOfDay.RemoveAt(dayTimeIndex);
                }
            }
            else if (trash > 7) // remove
            {
                if (trash > Random.Range(0f, GameManager.Instance.player.maxHealth))
                {
                    int dayTimeIndex = Random.Range(0, timesOfDay.Count); // 0 = morning,1 = afternoon, 2 = evening
                    //print(timesOfDay[dayTimeIndex]);
                    dayEvents[dayTimeIndex] = DayEventTypes.TrashRemove;
                    timesOfDay.RemoveAt(dayTimeIndex);
                }
            }
        }
    }
}