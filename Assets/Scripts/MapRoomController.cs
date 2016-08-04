using UnityEngine;
using System.Collections;

public class MapRoomController : MonoBehaviour
{
    public enum Type { Default, Start, Boss, Npc, Treasure, Secret };
    public Type roomType = Type.Default;
	public int roomIndex = -1;

    public void SetRoomIndex(int index)
    {
		roomIndex = index;
    }
    public void SetRoomType(string stringType)
    {
        switch (stringType)
        {
            case "Default":
                roomType = Type.Default;
                break;
            case "Start":
                roomType = Type.Start;
                break;
            case "Boss":
                roomType = Type.Boss;
                break;
            case "Npc":
                roomType = Type.Npc;
                break;
            case "Treasure":
                roomType = Type.Treasure;
                break;
            case "Secret":
                roomType = Type.Secret;
                break;
            default:
                roomType = Type.Default;
                break;
        }
    }
}