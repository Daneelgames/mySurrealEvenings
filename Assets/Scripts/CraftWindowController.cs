using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CraftWindowController : MonoBehaviour
{

    public List<Image> recipeIcons;
    public Animator _anim;
    public int activeRecipeIndex = 0;
    public Text title;
    public Text description;

    void Start()
    {
        HideWindow();
    }

    public void ShowWindow()
    {
        _anim.SetBool("Active", true);
        SetActiveRecipe(0);

        for (int i = 0; i < 4; i++)
        {
            if (i < GameManager.Instance.recipes.dayDecor.Count)
            {
                recipeIcons[i].gameObject.SetActive(true);
                recipeIcons[i].sprite = GameManager.Instance.recipes.dayDecor[i].decorIcon;
            }
            else
            {
                recipeIcons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetActiveRecipe(int index)
    {
        activeRecipeIndex = index;
        UpdateWindow();

        title.text = GameManager.Instance.recipes.dayDecor[activeRecipeIndex].decorTitle;
        description.text = GameManager.Instance.recipes.dayDecor[activeRecipeIndex].decorDescription;
    }

    public void UpdateWindow()
    {
        _anim.SetTrigger("Update");
    }

    public void HideWindow()
    {
        _anim.SetBool("Active", false);
        StartCoroutine("DisableWindow");
    }

    IEnumerator DisableWindow()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }

    public void StartCraft()
    {
        GameManager.Instance.recipes.sessionDecor.RemoveAt(activeRecipeIndex);
        GameManager.Instance.HideDayIcons();
    }
}