using UnityEngine;
using System.Collections;

public class SkillController : MonoBehaviour {

    public enum Type { offensive, defensive, recover }
    public Type skillType = Type.offensive; 

    public Sprite skillSprite;

    public string description;

    public int skillLevel = 1;
}
