using UnityEngine;
using System.Collections;

public class CraftWindowController : MonoBehaviour
{
    public Animator _anim;

    void Start()
    {
        HideWindow();
    }

    public void ShowWindow()
    {
        _anim.SetBool("Active", true);
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

	public void SetRecipe(int recipeIndex)
	{

	}
}