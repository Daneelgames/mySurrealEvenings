using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillRelationController : MonoBehaviour
{
    public Animator anim;

    public void SetFeedback()
    {
        anim.SetTrigger("Update");
    }
}