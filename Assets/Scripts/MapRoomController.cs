using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapRoomController : MonoBehaviour
{
    public enum Type { Default, Start, Boss, Npc, Treasure, Secret };
    public enum Wall { Solid, Passage, DoorLocked };
    public Wall wallUp = Wall.Solid;
    public Wall wallRight = Wall.Solid;
    public Wall wallDown = Wall.Solid;
    public Wall wallLeft = Wall.Solid;
    public Type roomType = Type.Default;
    public int roomIndex = -1000;
    public bool coreRoom = false;
    public List<MapPassageController> passages;

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
    public void SetCoreRoom(bool core)
    {
        coreRoom = core;
    }

    public void SetWallType(string wallType, Wall passageType)
    {
        switch (wallType)
        {
            case "Left":
                wallLeft = passageType;
                passages[0].SetType(passageType);
                break;
            case "Up":
                wallUp = passageType;
                passages[1].SetType(passageType);
                break;
            case "Right":
                wallRight = passageType;
                passages[2].SetType(passageType);
                break;
            case "Down":
                wallDown = passageType;
                passages[3].SetType(passageType);
                break;
        }
    }
}