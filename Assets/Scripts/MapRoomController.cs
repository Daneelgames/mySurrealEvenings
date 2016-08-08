﻿using UnityEngine;
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
    public bool safeRoom = false;
    public List<MapPassageController> passages;

    public GameObject playerMark;

    public float roomDifficulty = 1;
    public float spawnRate = 1;
    public bool roomCleared = false;
    public void SetRoomDiffuculty(float diff)
    {
        roomDifficulty = diff;
    }

    public void SetSpawnRate(float rate)
    {
        spawnRate = rate;
    }
    public void SetRoomIndex(int index)
    {
        roomIndex = index;
    }
    public void SetRoomCleared(bool cleared)
    {
        roomCleared = cleared;
    }
    public void SetCoreRoom(bool core)
    {
        coreRoom = core;
    }
    public void SetRoomType(string stringType)
    {
        float random = Random.value;

        switch (stringType)
        {
            case "Default":
                roomType = Type.Default;
                break;
            case "Start":
                roomType = Type.Start;
                safeRoom = true;
                break;
            case "Boss":
                roomType = Type.Boss;
                break;
            case "Npc":
                roomType = Type.Npc;
                safeRoom = true;
                break;
            case "Treasure":
                if (random > 0.66)
                    safeRoom = true;
                roomType = Type.Treasure;
                break;
            case "Secret":
                if (random > 0.15)
                    safeRoom = true;
                roomType = Type.Secret;
                break;
            default:
                roomType = Type.Default;
                break;
        }
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
    public void ActiveRoom(bool active)
    {
        playerMark.SetActive(active);
    }
}