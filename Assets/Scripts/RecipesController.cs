using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RecipesController : MonoBehaviour
{
    public List<DecorationController> decorList;
    public List<DecorationController> dayDecor;
    public List<DecorationController> sessionDecor;

    public void InitialRecipes() //called on start of session
    {
        sessionDecor = new List<DecorationController>(decorList);
    }

    public void GenerateDayDecor() // called on start of day
    {
        dayDecor.Clear();
        List<DecorationController> dayTempDecor = new List<DecorationController>(sessionDecor);

        for (int i = dayTempDecor.Count - 1; i >= 0; i--)
        {
            int randomDecor = Random.Range(0, dayTempDecor.Count);
            dayDecor.Add(dayTempDecor[randomDecor]);
            dayTempDecor.RemoveAt(randomDecor);
        }
    }
}