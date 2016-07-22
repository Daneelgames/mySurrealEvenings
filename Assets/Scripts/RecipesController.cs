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

        for (int i = sessionDecor.Count - 1; i >= 0; i--)
        {
            int randomDecor = Random.Range(0, sessionDecor.Count);
            dayDecor.Add(sessionDecor[randomDecor]);
            sessionDecor.RemoveAt(randomDecor);
        }
    }
}