using UnityEngine;
using System.Collections;

public class MapPassageController : MonoBehaviour
{
    public enum Type { Passage, Door };
    public Type passageType = Type.Passage;
    public GameObject locker;

    public void SetType(string passType)
    {
        switch (passType)
        {
            case "Passage":
                locker.SetActive(false);
                passageType = Type.Passage;
                break;
            case "Door":
                locker.SetActive(true);
                passageType = Type.Door;
                break;
            default:
                locker.SetActive(false);
                passageType = Type.Passage;
                break;
        }
    }
}
