using UnityEngine;
using System.Collections;

public class SkillEffectController : MonoBehaviour
{

    Animator anim;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine("SetSpeed");
    }

    IEnumerator SetSpeed()
    {
        anim.speed = 0;
        yield return new WaitForSeconds(0.25f);
        anim.speed = 1;
    }
}
