using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMapGenerator : MonoBehaviour
{

    public enum Direction { Left, Up, Right, Down, No };
    public Direction excludeDirecion = Direction.Left;
    public int maxRepeats = 3;
    public GameObject roomImage;
    public GameObject passageImage;

    public List<Vector3> rooms;
    public List<Vector3> placesForSecretRooms;
    int roomsRemaining = 6;
    public List<Direction> newDirection = new List<Direction>();
    public int npcAmount = 0;

    public void GenerateMap(int rooms)
    {
        roomsRemaining = rooms;
        PickLevelDirection();
        SpawnStartRoom();
        SpawnMainRooms();
        SpawnExtraRooms();
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
        rooms.Add(room.transform.position);
        roomsRemaining -= 1;
    }
    void SpawnMainRooms()
    {
        Vector3 lastRoomPosition = rooms[0];

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
                print(hitLeft.collider.name);
                if (hitLeft.collider.gameObject.tag == "MapRoom" && excludeDirecion != Direction.Left)
                    newDirection.Remove(Direction.Left);
            }

            RaycastHit2D hitUp = Physics2D.Raycast(lastRoomPosition, Vector2.up, 0.72f, 1 << 9);
            if (hitUp.collider != null)
            {
                print(hitUp.collider.name);
                if (hitUp.collider.gameObject.tag == "MapRoom" && excludeDirecion != Direction.Up)
                    newDirection.Remove(Direction.Up);
            }

            RaycastHit2D hitRight = Physics2D.Raycast(lastRoomPosition, Vector2.right, 1.28f, 1 << 9);
            if (hitRight.collider != null)
            {
                print(hitRight.collider.name);
                if (hitRight.collider.gameObject.tag == "MapRoom" && excludeDirecion != Direction.Right)
                    newDirection.Remove(Direction.Right);
            }

            RaycastHit2D hitDown = Physics2D.Raycast(lastRoomPosition, Vector2.down, 0.72f, 1 << 9);
            if (hitDown.collider != null)
            {
                print(hitDown.collider.name);
                if (hitDown.collider.gameObject.tag == "MapRoom" && excludeDirecion != Direction.Down)
                    newDirection.Remove(Direction.Down);
            }
            newDirection.Sort();
            print(newDirection.Count);

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

            /*
            // ADD PASSAGE HERE
            GameObject pass = Instantiate(passageImage, lastRoomPosition, Quaternion.Euler(0, 0, 0)) as GameObject;
            pass.GetComponent<MapPassageController>().SetType("Passage");
            pass.transform.SetParent(transform);
            pass.name = "Passage_" + i;
            */

            switch (newDirection[random])
            {
                case Direction.Left:
                    lastDirection = Direction.Left;
                    newRoomPosOffset = new Vector3(-1.28f, 0, 0);
             //       pass.transform.position = new Vector3(lastRoomPosition.x - 0.64f, lastRoomPosition.y, 0);
             //       pass.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Up:
                    lastDirection = Direction.Up;
                    newRoomPosOffset = new Vector3(0, 0.72f, 0);
             //       pass.transform.position = new Vector3(lastRoomPosition.x, lastRoomPosition.y + 0.36f, 0);
             //       pass.transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
                case Direction.Right:
                    lastDirection = Direction.Right;
                    newRoomPosOffset = new Vector3(1.28f, 0, 0);
             //       pass.transform.position = new Vector3(lastRoomPosition.x + 0.64f, lastRoomPosition.y, 0);
             //       pass.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Direction.Down:
                    lastDirection = Direction.Down;
                    newRoomPosOffset = new Vector3(0, -0.72f, 0);
             //       pass.transform.position = new Vector3(lastRoomPosition.x, lastRoomPosition.y - 0.36f, 0);
             //       pass.transform.rotation = Quaternion.Euler(0, 0, 90);
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
                room.name = "bossRoom";
                room.GetComponent<MapRoomController>().SetRoomType("Boss");
            }
            room.transform.SetParent(transform);
            room.transform.localScale = Vector3.one;
            room.transform.localPosition += newRoomPosOffset;
            room.GetComponent<MapRoomController>().SetRoomIndex(i);
            rooms.Add(room.transform.position);


            lastRoomPosition = room.transform.position;
            roomsRemaining -= 1;
        }
    }
    void SpawnExtraRooms()
    {
        // 2 MAIN SECRET ROOMS !!!!!
        //check every room except bossRoom
        placesForSecretRooms = new List<Vector3>();

        AddRoomsToList(rooms, placesForSecretRooms);
        List<Vector3> secretRooms = new List<Vector3>();

        int secretIndex = Random.Range(0, placesForSecretRooms.Count);
        GameObject secretRoom = Instantiate(roomImage, placesForSecretRooms[secretIndex], Quaternion.identity) as GameObject;
        secretRoom.name = "secretRoom";
        secretRoom.transform.SetParent(transform);
        SetExtraRoomType(secretRoom, "Secret");
        secretRooms.Add(placesForSecretRooms[secretIndex]);
        placesForSecretRooms.RemoveAt(secretIndex);
        //placesForSecretRooms.Sort();

        int treasureIndex = Random.Range(0, placesForSecretRooms.Count);
        GameObject treasureRoom = Instantiate(roomImage, placesForSecretRooms[treasureIndex], Quaternion.identity) as GameObject;
        treasureRoom.name = "treasureRoom";
        treasureRoom.transform.SetParent(transform);
        SetExtraRoomType(treasureRoom, "Treasure");
        secretRooms.Add(placesForSecretRooms[treasureIndex]);
        placesForSecretRooms.RemoveAt(treasureIndex);
        //placesForSecretRooms.Sort();

        // EXTRA ROOMS
        AddRoomsToList(secretRooms, placesForSecretRooms);
        int extraRoomsCount = 0;
        foreach (Vector3 pos in placesForSecretRooms)
        {
            float random = Random.value;
            if (random > 0.66)
            {
                GameObject room = Instantiate(roomImage, pos, Quaternion.identity) as GameObject;
                room.name = "extraRoom";
                room.transform.SetParent(transform);
                extraRoomsCount += 1;
                float randomChance = Random.value;
                print("extra room chance is " + randomChance);

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
                Vector3 secretRoomPlace = new Vector3(rooms[i].x, rooms[i].y + 0.72f, 0);
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
                Vector3 secretRoomPlace = new Vector3(rooms[i].x + 1.28f, rooms[i].y, 0);
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
                Vector3 secretRoomPlace = new Vector3(rooms[i].x, rooms[i].y - 0.72f, 0);
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
        room.GetComponent<MapRoomController>().SetRoomType(roomType);

        float random = Random.value;
        switch (roomType)
        {
            case "Treasure":
                SpawnPass(room, true);
                break;
            case "Npc":
                if (random > 0 && random <= 0.33f)
                    SpawnPass(room, true);
                else if (random > 0.33f && random < 0.66f)
                    SpawnPass(room, false);
                break;
            case "Default":
                if (random > 0.1f && random <= 0.3f)
                    SpawnPass(room, true);
                else if (random > 0.3f)
                    SpawnPass(room, false);
                break;
            case "Secret":
                break;
        }
    }


    void SpawnPass(GameObject room, bool door)
    {
        /*
        RaycastHit2D hitLeft = Physics2D.Raycast(room.transform.position, Vector2.left, 1.28f, 1 << 9);
        RaycastHit2D hitUp = Physics2D.Raycast(room.transform.position, Vector2.up, 0.72f, 1 << 9);
        RaycastHit2D hitRight = Physics2D.Raycast(room.transform.position, Vector2.right, 1.28f, 1 << 9);
        RaycastHit2D hitDown = Physics2D.Raycast(room.transform.position, Vector2.down, 0.72f, 1 << 9);

        Vector3 passageOffset = Vector3.zero;
        Quaternion passageRotation = Quaternion.Euler(Vector3.zero);

        List<Vector3> positions = new List<Vector3>();

        if (hitLeft.collider != null && hitLeft.collider.gameObject.tag == "MapRoom")
        {
            MapRoomController roomController = hitLeft.collider.gameObject.GetComponent<MapRoomController>() as MapRoomController;
            if (roomController.roomIndex > 0)
            {
                positions.Add(new Vector3(-0.64f, 0, 0));
                passageRotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
        }
        if (hitUp.collider != null && hitUp.collider.gameObject.tag == "MapRoom")
        {
            MapRoomController roomController = hitUp.collider.gameObject.GetComponent<MapRoomController>() as MapRoomController;
            if (roomController.roomIndex > 0)
            {
                positions.Add(new Vector3(0, 0.36f, 0));
                passageRotation = Quaternion.Euler(Vector3.zero);
            }
        }
        if (hitRight.collider != null && hitRight.collider.gameObject.tag == "MapRoom")
        {
            MapRoomController roomController = hitRight.collider.gameObject.GetComponent<MapRoomController>() as MapRoomController;
            if (roomController.roomIndex > 0)
            {
                positions.Add(new Vector3(0.64f, 0, 0));
                passageRotation = Quaternion.Euler(new Vector3(0, 0, 90));
            }
        }
        if (hitDown.collider != null && hitDown.collider.gameObject.tag == "MapRoom")
        {
            MapRoomController roomController = hitDown.collider.gameObject.GetComponent<MapRoomController>() as MapRoomController;
            if (roomController.roomIndex > 0)
            {
                positions.Add(new Vector3(0, -0.36f, 0));
                passageRotation = Quaternion.Euler(Vector3.zero);
            }
        }
        int randomIndex = Random.Range(0, positions.Count);

        Vector3 passagePos = Vector3.zero;
        if (positions.Count > randomIndex)
            passagePos = room.transform.position + positions[randomIndex];

        if (positions.Count > 0)
        {
            GameObject passage = Instantiate(passageImage, passagePos, Quaternion.identity) as GameObject;
            passage.transform.rotation = passageRotation;
            passage.name = "passage";
            passage.transform.SetParent(transform);

            if (door)
                passage.GetComponent<MapPassageController>().SetType("Door");
            else
                passage.GetComponent<MapPassageController>().SetType("Passage");

            // NEED TO SEND DOOR INFO TO NEIGHBOUR ROOM
        }
        */
    }
}