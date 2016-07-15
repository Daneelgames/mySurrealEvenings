using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillRelationController : MonoBehaviour {
	public Animator anim;
	public Text _text;

	public void SetFeedback(string newText)
	{
		anim.SetTrigger("Update");
		_text.text = newText;
	}
}
