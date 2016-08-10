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
    public bool safeRoom = false;
    public List<MapPassageController> passages;
    public MapRoomController neighbourLeft;
    public MapRoomController neighbourUp;
    public MapRoomController neighbourRight;
    public MapRoomController neighbourDown;

    public GameObject playerMark;

    public float roomDifficulty = 1;
    public float spawnRate = 1;
    public bool roomCleared = false;

    public SpriteRenderer roomSprite;
    public List<SpriteRenderer> passageSprites;
    public GameObject treasureChest;
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
    public void SetRoomCleared(bool cleared) // GIVE REWARD FOR CLEAINING ROOM
    {
        roomCleared = cleared;

        GameManager.Instance.AddEscapeChance(0.25f);
        print("Room cleared");
        GiveReward();
    }
    public void GiveReward()
    {
        int moneyReward = 0;
        bool key = false;
        GameObject treasure = null;
        print("Give reward");
        switch (roomType)
        {
            case Type.Default:
                float random = Random.value;
                if (random > 0.33f) // GIVE MONEY
                {
                    // GENERATE MONEY
                    int randomMoney = 1;
                    if (random > 0.5f)
                        randomMoney = 2;
                    if (random > 0.75f)
                        randomMoney = 3;
                    if (random > 0.85f)
                        randomMoney = 4;
                    if (random > 0.95f)
                        randomMoney = 5;
                    moneyReward = randomMoney;
                }
                switch (GameManager.Instance.inventoryController.keys)
                {
                    case 0:
                        if (random > 0.1f)
                        {
                            // GENERATE Key
                            key = true;
                        }
                        break;
                    case 1:
                        if (random > 0.5f)
                        {
                            // GENERATE Key
                            key = true;
                        }
                        break;
                    default:
                        if (random > 0.9f)
                        {
                            // GENERATE Key
                            key = true;
                        }
                        break;
                }
                break;
        }

        if (moneyReward != 0 || key || treasure != null)
        {
            // Instantiate treasure chest
            GameObject chest = Instantiate(treasureChest, Vector3.zero, Quaternion.identity) as GameObject;
            TreasureChestController chestController = chest.GetComponent<TreasureChestController>();

            chestController.moneyDrop = moneyReward;
            chestController.keyDrop = key;
            if (treasure != null)
                chestController.treasure = treasure;
        }
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

    public void ShowRoom(bool active)
    {
        roomSprite.enabled = active;

    }
    public void ShowPassages(bool active)
    {
        foreach (SpriteRenderer spr in passageSprites)
        {
            spr.enabled = active;
        }
    }
    public void AddNeighbourLeft(MapRoomController room)
    {
        neighbourLeft = room;
    }
    public void AddNeighbourUp(MapRoomController room)
    {
        neighbourUp = room;
    }
    public void AddNeighbourRight(MapRoomController room)
    {
        neighbourRight = room;
    }
    public void AddNeighbourDown(MapRoomController room)
    {
        neighbourDown = room;
    }
    public void UpdateNeighbours()
    {
        if (neighbourLeft != null && wallLeft != Wall.Solid)
        {
            neighbourLeft.ShowRoom(true);
        }
        if (neighbourUp != null && wallUp != Wall.Solid)
        {
            neighbourUp.ShowRoom(true);
        }
        if (neighbourRight != null && wallRight != Wall.Solid)
        {
            neighbourRight.ShowRoom(true);
        }
        if (neighbourDown != null && wallDown != Wall.Solid)
        {
            neighbourDown.ShowRoom(true);
        }
    }
    public void EscapedFromRoom()
    {
        if (!roomCleared && !safeRoom && spawnRate != 0)
            spawnRate = 0.5f;
    }
}