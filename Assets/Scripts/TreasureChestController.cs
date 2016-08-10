using UnityEngine;
using System.Collections;

public class TreasureChestController : MonoBehaviour
{
    public int moneyDrop = 0;
    public bool keyDrop = false;
    public GameObject treasure = null;

	public Animator anim;

    float cooldown = 0.75f;
	bool opened = false;

    void Update()
    {
        if (cooldown > 0)
            cooldown -= Time.deltaTime;
    }
    void OnMouseUpAsButton()
    {
        // click on chest
        if (cooldown <= 0 && !opened)
        {
			opened = true;
			anim.SetTrigger("Update");
			StartCoroutine("OpenChest");
        }
    }
	
	IEnumerator OpenChest()
	{
		yield return new WaitForSeconds (0.25f);
		GameManager.Instance.ShowRewardWindow(moneyDrop, keyDrop, treasure);
		Destroy(gameObject);
	}
}
