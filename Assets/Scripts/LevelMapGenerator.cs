using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMapGenerator : MonoBehaviour
{
    public enum Direction { Left, Up, Right, Down, No };
    public Direction excludeDirecion = Direction.Left;
    public int maxRepeats = 3;
    public GameObject roomImage;

    public List<Vector3> roomsPositions;
    public List<Vector3> placesForSecretRooms;
    int roomsRemaining = 6;
    public List<Direction> newDirection = new List<Direction>();
    public int npcAmount = 0;
    public List<GameObject> rooms;
    public LevelMovementController _levelMovementController;
    public int dungeonLevel = 1;

    public GameObject mapBack;
    public Renderer mapBackMaterial;
    public void GenerateMap(int _rooms)
    {
        rooms = new List<GameObject>();
        roomsRemaining = _rooms;
        PickLevelDirection();
        SpawnStartRoom();
        SpawnMainRooms();
        SpawnExtraRooms();

        MakePassages();

        MakeDifficulties();

        _levelMovementController.SetStartRoom(rooms[0]);

        SetMapPosition();
        GameManager.Instance.NewStage();
    }

    void SetMapPosition()
    {
        float maxX = 0;
        float minX = 0;
        float minY = 0;
        float maxY = 0;

        foreach (GameObject rm in rooms)
        {
            if (rm.transform.localPosition.x > maxX)
                maxX = rm.transform.localPosition.x;
            if (rm.transform.localPosition.x < minX)
                minX = rm.transform.localPosition.x;
            if (rm.transform.localPosition.y > maxY)
                maxY = rm.transform.localPosition.y;
            if (rm.transform.localPosition.y < minY)
                minY = rm.transform.localPosition.y;
        }

        print(maxX);
        print(minX);
        print(maxY);
        print(minY);
        transform.position = new Vector3(-13 - minX, -7.5f - minY, 0);
        mapBack.transform.localScale = new Vector3(maxX - minX + 3, maxY - minY + 2, 1);
        Vector3 newPos = new Vector3(minX, maxY, 1) + new Vector3(maxX, maxY, 1) + new Vector3(maxX, minY, 1) + new Vector3(minX, minY, 1);
        mapBack.transform.localPosition = newPos / 4;
        mapBackMaterial.material.mainTextureScale = new Vector2(maxX - minX + 3f, maxY - minY + 2);
    }

    void MakeDifficulties()
    {
        float lastDiff = dungeonLevel;
        foreach (GameObject rm in rooms)
        {
            float newDiff = lastDiff + Random.Range(0.05f, 0.25f);
            MapRoomController rmCntrl = rm.GetComponent<MapRoomController>();
            rmCntrl.SetRoomDiffuculty(newDiff);
            lastDiff = newDiff;

            if (rmCntrl.roomIndex != 0)
            {
                rmCntrl.ShowRoom(false);
                rmCntrl.ShowPassages(false);
            }
        }
    }

    void SpawnStartRoom()
    {
        GameObject room = Instantiate(roomImage, transform.position, Quaternion.identity) as GameObject;
        room.name = "startRoom";
        room.transform.SetParent(transform);
        room.transform.localPosition = Vector3.zero;
        MapRoomController roomController = room.GetComponent<MapRoomController>();
        roomController.SetRoomType("Start");
        roomController.SetRoomIndex(0);
        roomController.SetCoreRoom(true);
        roomsPositions.Add(room.transform.position);
        roomsRemaining -= 1;
        rooms.Add(room);
    }
    void SpawnMainRooms()
    {
        Vector3 lastRoomPosition = roomsPositions[0];

        Vector3 newRoomPosOffset = Vector3.zero;
        Direction lastDirection = Direction.No;

        int repeats = 0;

        for (int i = roomsRemaining; i > 0; i--)
        {
            newDirection = new List<Direction>();

            switch (excludeDirecion)
            {
                case Direction.Left:
                    newDirection.Add(Direction.Down);
                    newDirection.Add(Direction.Right);
                    newDirection.Add(Direction.Up);
                    break;
                case Direction.Up:
                    newDirection.Add(Direction.Down);
                    newDirection.Add(Direction.Right);
                    newDirection.Add(Direction.Left);
                    break;
                case Direction.Right:
                    newDirection.Add(Direction.Down);
                    newDirection.Add(Direction.Left);
                    newDirection.Add(Direction.Up);
                    break;
                case Direction.Down:
                    newDirection.Add(Direction.Left);
                    newDirection.Add(Direction.Right);
                    newDirection.Add(Direction.Up);
                    break;
            }


            RaycastHit2D hitLeft = Physics2D.Raycast(lastRoomPosition, Vector2.left, 1.28f, 1 << 9);
            if (hitLeft.collider != null)
            {
                //                print(hitLeft.collider.name);
                if (hitLeft.collider.gameObject.tag == "MapRoom" && excludeDirecion != Direction.Left)
                    newDirection.Remove(Direction.Left);
            }

            RaycastHit2D hitUp = Physics2D.Raycast(lastRoomPosition, Vector2.up, 0.72f, 1 << 9);
            if (hitUp.collider != null)
            {
                //              print(hitUp.collider.name);
                if (hitUp.collider.gameObject.tag == "MapRoom" && excludeDirecion != Direction.Up)
                    newDirection.Remove(Direction.Up);
            }

            RaycastHit2D hitRight = Physics2D.Raycast(lastRoomPosition, Vector2.right, 1.28f, 1 << 9);
            if (hitRight.collider != null)
            {
                //               print(hitRight.collider.name);
                if (hitRight.collider.gameObject.tag == "MapRoom" && excludeDirecion != Direction.Right)
                    newDirection.Remove(Direction.Right);
            }

            RaycastHit2D hitDown = Physics2D.Raycast(lastRoomPosition, Vector2.down, 0.72f, 1 << 9);
            if (hitDown.collider != null)
            {
                //              print(hitDown.collider.name);
                if (hitDown.collider.gameObject.tag == "MapRoom" && excludeDirecion != Direction.Down)
                    newDirection.Remove(Direction.Down);
            }
            newDirection.Sort();
            //           print(newDirection.Count);

            int random = Random.Range(0, newDirection.Count);

            if (newDirection[random] == lastDirection)
            {
                if (repeats < maxRepeats)
                    repeats += 1;
                else
                {
                    switch (random)
                    {
                        case 0:
                            float localRandom0 = Random.value;
                            if (localRandom0 > 0.5f)
                            {
                                if (newDirection.Count > 1)
                                    random = 1;
                            }
                            else if (newDirection.Count > 2)
                                random = 2;
                            break;

                        case 1:
                            float localRandom1 = Random.value;
                            if (localRandom1 > 0.5f)
                                random = 0;
                            else if (newDirection.Count > 2)
                                random = 2;
                            break;
                        case 2:
                            float localRandom2 = Random.value;
                            if (localRandom2 > 0.5f)
                                random = 0;
                            else if (newDirection.Count > 1)
                                random = 1;
                            break;
                    }
                    repeats = 0;
                }
            }
            else
            {
                repeats = 0;
            }

            switch (newDirection[random])
            {
                case Direction.Left:
                    lastDirection = Direction.Left;
                    newRoomPosOffset = new Vector3(-1.28f, 0, 0);
                    break;
                case Direction.Up:
                    lastDirection = Direction.Up;
                    newRoomPosOffset = new Vector3(0, 0.72f, 0);
                    break;
                case Direction.Right:
                    lastDirection = Direction.Right;
                    newRoomPosOffset = new Vector3(1.28f, 0, 0);
                    break;
                case Direction.Down:
                    lastDirection = Direction.Down;
                    newRoomPosOffset = new Vector3(0, -0.72f, 0);
                    break;
            }

            GameObject room = Instantiate(roomImage, lastRoomPosition, Quaternion.identity) as GameObject;

            if (i > 1)
            {
                room.name = "mainRoom_" + i;
                if (npcAmount < 2)
                {
                    float randomChance = Random.value; // NPC SPAWN CHANCE
                    if (randomChance > 0.75)
                    {
                        room.GetComponent<MapRoomController>().SetRoomType("Npc");
                        npcAmount += 1;
                    }
                    else
                        room.GetComponent<MapRoomController>().SetRoomType("Default");
                }
                else
                    room.GetComponent<MapRoomController>().SetRoomType("Default");
            }
            else
            {
                room.name = "mainRoom_1_bossRoom";
                room.GetComponent<MapRoomController>().SetRoomType("Boss");
            }
            room.transform.SetParent(transform);
            room.transform.localScale = Vector3.one;
            room.transform.localPosition += newRoomPosOffset;
            MapRoomController roomController = room.GetComponent<MapRoomController>();
            roomController.SetRoomIndex(i);
            roomController.SetCoreRoom(true);
            roomsPositions.Add(room.transform.position);
            rooms.Add(room);


            lastRoomPosition = room.transform.position;
            roomsRemaining -= 1;
        }
    }
    void SpawnExtraRooms()
    {
        // 2 MAIN SECRET ROOMS !!!!!
        //check every room except bossRoom
        placesForSecretRooms = new List<Vector3>();

        AddRoomsToList(roomsPositions, placesForSecretRooms);
        List<Vector3> secretRooms = new List<Vector3>();

        int secretIndex = Random.Range(0, placesForSecretRooms.Count);
        GameObject secretRoom = Instantiate(roomImage, placesForSecretRooms[secretIndex], Quaternion.identity) as GameObject;
        secretRoom.name = "secretRoom";
        secretRoom.transform.SetParent(transform);
        SetExtraRoomType(secretRoom, "Secret");
        secretRooms.Add(placesForSecretRooms[secretIndex]);
        placesForSecretRooms.RemoveAt(secretIndex);
        rooms.Add(secretRoom);

        AddRoomsToList(secretRooms, placesForSecretRooms);

        int treasureIndex = Random.Range(0, placesForSecretRooms.Count);
        GameObject treasureRoom = Instantiate(roomImage, placesForSecretRooms[treasureIndex], Quaternion.identity) as GameObject;
        treasureRoom.name = "treasureRoom";
        treasureRoom.transform.SetParent(transform);
        SetExtraRoomType(treasureRoom, "Treasure");
        secretRooms.Add(placesForSecretRooms[treasureIndex]);
        placesForSecretRooms.RemoveAt(treasureIndex);
        rooms.Add(treasureRoom);

        // EXTRA ROOMS
        AddRoomsToList(secretRooms, placesForSecretRooms);
        int extraRoomsCount = 0;
        foreach (Vector3 pos in placesForSecretRooms)
        {
            float random = Random.value;
            if (random > 0.66)
            {
                bool noDouble = true;
                foreach (GameObject rm in rooms) // check here if room already there
                {
                    if (rm.transform.position == pos)
                    {
                        noDouble = false;
                        break;
                    }
                }
                print(noDouble);
                if (noDouble)
                {
                    GameObject room = Instantiate(roomImage, pos, Quaternion.identity) as GameObject;
                    rooms.Add(room);
                    room.name = "extraRoom";
                    room.transform.SetParent(transform);
                    extraRoomsCount += 1;
                    float randomChance = Random.value;
                    //print("extra room chance is " + randomChance);

                    // SET ROOM TYPE
                    SetExtraRoomType(room, "Default");
                    if (randomChance < 0.75f && npcAmount < 3)
                    {
                        SetExtraRoomType(room, "Npc");
                        npcAmount += 1;
                    }
                    if (randomChance < 0.6f)
                        SetExtraRoomType(room, "Treasure");
                    if (randomChance < 0.3f)
                        SetExtraRoomType(room, "Secret");
                }
            }
            if (extraRoomsCount > GameManager.Instance.roomsMinimum / 2)
                break;
        }
    }
    void AddRoomsToList(List<Vector3> fromList, List<Vector3> toList)
    {
        for (int i = 0; i < fromList.Count - 1; i++)
        {
            RaycastHit2D hitLeft = Physics2D.Raycast(fromList[i], Vector2.left, 1.28f, 1 << 9);
            RaycastHit2D hitUp = Physics2D.Raycast(fromList[i], Vector2.up, 0.72f, 1 << 9);
            RaycastHit2D hitRight = Physics2D.Raycast(fromList[i], Vector2.right, 1.28f, 1 << 9);
            RaycastHit2D hitDown = Physics2D.Raycast(fromList[i], Vector2.down, 0.72f, 1 << 9);

            if (!hitLeft)
            {
                Vector3 secretRoomPlace = new Vector3(fromList[i].x - 1.28f, fromList[i].y, 0);
                bool noDouble = true;
                if (toList.Count > 0)
                {
                    foreach (Vector3 pos in toList)
                        if (pos == secretRoomPlace)
                        {
                            noDouble = false;
                            break;
                        }
                }
                if (noDouble)
                    toList.Add(secretRoomPlace);
            }
            if (!hitUp)
            {
                Vector3 secretRoomPlace = new Vector3(roomsPositions[i].x, roomsPositions[i].y + 0.72f, 0);
                bool noDouble = true;
                if (toList.Count > 0)
                {
                    foreach (Vector3 pos in toList)
                        if (pos == secretRoomPlace)
                        {
                            noDouble = false;
                            break;
                        }
                }
                if (noDouble)
                    toList.Add(secretRoomPlace);
            }
            if (!hitRight)
            {
                Vector3 secretRoomPlace = new Vector3(roomsPositions[i].x + 1.28f, roomsPositions[i].y, 0);
                bool noDouble = true;
                if (toList.Count > 0)
                {
                    foreach (Vector3 pos in toList)
                        if (pos == secretRoomPlace)
                        {
                            noDouble = false;
                            break;
                        }
                }
                if (noDouble)
                    toList.Add(secretRoomPlace);
            }
            if (!hitDown)
            {
                Vector3 secretRoomPlace = new Vector3(roomsPositions[i].x, roomsPositions[i].y - 0.72f, 0);
                bool noDouble = true;
                if (toList.Count > 0)
                {
                    foreach (Vector3 pos in toList)
                        if (pos == secretRoomPlace)
                        {
                            noDouble = false;
                            break;
                        }
                }
                if (noDouble)
                    toList.Add(secretRoomPlace);
            }
        }
    }

    void PickLevelDirection()
    {
        int random = Random.Range(0, 4);
        switch (random)
        {
            case 0:
                excludeDirecion = Direction.Left;
                break;
            case 1:
                excludeDirecion = Direction.Up;
                break;
            case 2:
                excludeDirecion = Direction.Right;
                break;
            case 3:
                excludeDirecion = Direction.Down;
                break;
        }

    }

    void SetExtraRoomType(GameObject room, string roomType)
    {
        MapRoomController roomController = room.GetComponent<MapRoomController>();
        roomController.SetRoomType(roomType);
        roomController.SetCoreRoom(false);
    }

    void MakePassages() // COMON LETS DO SOM PASAGES
    {
        rooms = new List<GameObject>(GameObject.FindGameObjectsWithTag("MapRoom"));

        List<GameObject> roomsTempList = new List<GameObject>(rooms);
        for (int i = roomsTempList.Count - 1; i > 0; i--)
        {
            MapRoomController roomController = roomsTempList[i].GetComponent<MapRoomController>();

            if (roomController.roomType != MapRoomController.Type.Secret)
            {
                RaycastHit2D hitLeft = Physics2D.Raycast(roomsTempList[i].transform.position, Vector2.left, 1.28f, 1 << 9);
                GeneratePass(roomsTempList, hitLeft, roomController, "Left", "Right");

                RaycastHit2D hitUp = Physics2D.Raycast(roomsTempList[i].transform.position, Vector2.up, 0.72f, 1 << 9);
                GeneratePass(roomsTempList, hitUp, roomController, "Up", "Down");

                RaycastHit2D hitRight = Physics2D.Raycast(roomsTempList[i].transform.position, Vector2.right, 1.28f, 1 << 9);
                GeneratePass(roomsTempList, hitRight, roomController, "Right", "Left");

                RaycastHit2D hitDown = Physics2D.Raycast(roomsTempList[i].transform.position, Vector2.down, 0.72f, 1 << 9);
                GeneratePass(roomsTempList, hitDown, roomController, "Down", "Up");
            }

            //REMOVE ROOM FROM LIST
            roomsTempList.RemoveAt(i);
        }
    }
    void GeneratePass(List<GameObject> roomsTempList, RaycastHit2D hit, MapRoomController roomController, string roomWall_1, string roomWall_2)
    {
        if (hit.collider != null && hit.collider.gameObject.tag == "MapRoom")
        {
            foreach (GameObject rm in roomsTempList)
            {
                if (rm == hit.collider.gameObject)
                {
                    MapRoomController neighbourRoomController = hit.collider.gameObject.GetComponent<MapRoomController>();

                    // TYPES //////////////////////////
                    if (roomController.roomType == MapRoomController.Type.Start) // START ROOM
                    {
                        if (neighbourRoomController.roomType == MapRoomController.Type.Default || neighbourRoomController.roomType == MapRoomController.Type.Npc) // DEFAULT OR NPC
                        {
                            if (neighbourRoomController.coreRoom) // CORE ROOM
                            {
                                roomController.SetWallType(roomWall_1, MapRoomController.Wall.Passage);
                                neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Passage);
                            }
                            else // NOT CORE ROOM
                            {
                                float random = Random.value;
                                if (random > 0.3f) //  passage
                                {
                                    roomController.SetWallType(roomWall_1, MapRoomController.Wall.Passage);
                                    neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Passage);
                                }
                                else if (random > 0.6f) //  locker
                                {
                                    roomController.SetWallType(roomWall_1, MapRoomController.Wall.DoorLocked);
                                    neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.DoorLocked);
                                }
                                else //  SOLID WALL
                                {
                                    roomController.SetWallType(roomWall_1, MapRoomController.Wall.Solid);
                                    neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Solid);
                                }
                            }
                        }
                        else if (neighbourRoomController.roomType == MapRoomController.Type.Secret || neighbourRoomController.roomType == MapRoomController.Type.Boss) // SECRET OR BOSS
                        {
                            roomController.SetWallType(roomWall_1, MapRoomController.Wall.Solid);
                            neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Solid);
                        }
                        else if (neighbourRoomController.roomType == MapRoomController.Type.Treasure) // TREASURE
                        {
                            roomController.SetWallType(roomWall_1, MapRoomController.Wall.Solid);
                            neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Solid);
                        }
                    }
                    else if (roomController.roomType == MapRoomController.Type.Boss) // BOSS ROOM
                    {
                        if (neighbourRoomController.roomIndex == 2 && neighbourRoomController.coreRoom) // BOSS ENTRANCE PASSAGE
                        {
                            roomController.SetWallType(roomWall_1, MapRoomController.Wall.Passage);
                            neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Passage);
                        }
                        else // OTHER - SOLID WALL
                        {
                            roomController.SetWallType(roomWall_1, MapRoomController.Wall.Solid);
                            neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Solid);
                        }
                    }
                    else if (roomController.roomType == MapRoomController.Type.Default || roomController.roomType == MapRoomController.Type.Npc) // DEFAULT OR NPC
                    {
                        if (neighbourRoomController.coreRoom) // CORE PASSAGE
                        {
                            int roomIndexesOdds = Mathf.Abs(neighbourRoomController.roomIndex - roomController.roomIndex);

                            if (roomIndexesOdds == 1 || neighbourRoomController.roomType == MapRoomController.Type.Start)
                            {
                                roomController.SetWallType(roomWall_1, MapRoomController.Wall.Passage);
                                neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Passage);
                            }
                            else if (neighbourRoomController.roomType == MapRoomController.Type.Boss || roomIndexesOdds != 1)
                            {
                                if (neighbourRoomController.roomType != MapRoomController.Type.Start)
                                {
                                    roomController.SetWallType(roomWall_1, MapRoomController.Wall.Solid);
                                    neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Solid);
                                }
                            }
                        }
                        else // OTHER
                        {
                            float random = Random.value;
                            if (random > 0.3f) //  passage
                            {
                                roomController.SetWallType(roomWall_1, MapRoomController.Wall.Passage);
                                neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Passage);
                            }
                            else if (random > 0.6f) //  locker
                            {
                                roomController.SetWallType(roomWall_1, MapRoomController.Wall.DoorLocked);
                                neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.DoorLocked);
                            }
                            else //  SOLID WALL
                            {
                                roomController.SetWallType(roomWall_1, MapRoomController.Wall.Solid);
                                neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Solid);
                            }
                        }
                    }
                    else if (roomController.roomType == MapRoomController.Type.Treasure) // TREASURE
                    {
                        if (neighbourRoomController.roomType == MapRoomController.Type.Boss)
                        {
                            roomController.SetWallType(roomWall_1, MapRoomController.Wall.Solid);
                            neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Solid);
                        }
                        else
                        {
                            float random = Random.value;
                            if (random > 0.3f) // locker
                            {
                                roomController.SetWallType(roomWall_1, MapRoomController.Wall.DoorLocked);
                                neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.DoorLocked);
                            }
                            else // SOLID WALL
                            {
                                roomController.SetWallType(roomWall_1, MapRoomController.Wall.Solid);
                                neighbourRoomController.SetWallType(roomWall_2, MapRoomController.Wall.Solid);
                            }
                        }
                    }
                }
            }
        }
    }
}