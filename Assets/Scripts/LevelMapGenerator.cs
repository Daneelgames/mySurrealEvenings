using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMapGenerator : MonoBehaviour
{

    public enum Direction { UR, DR, DL, UL };
    public enum ExtraRoomsDirection { HOR, VER };
    public Direction levelDirection = Direction.UR;
    public ExtraRoomsDirection _extraRoomsDirection = ExtraRoomsDirection.HOR;

    public GameObject roomImage;
    public GameObject passageImage;

    public List<Transform> startRoomPositions;
    public List<Transform> rooms;
    int roomsRemaining = 6;

    public List<Vector3> suitableRoomPositive = new List<Vector3>();
    public List<Vector3> suitableRoomNegative = new List<Vector3>();

	public int maxRepeats = 1;
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
        GameObject room = new GameObject();
        switch (levelDirection)
        {
            case Direction.DL:
                room = Instantiate(roomImage, startRoomPositions[0].position, Quaternion.identity) as GameObject;
                break;
            case Direction.UL:
                room = Instantiate(roomImage, startRoomPositions[1].position, Quaternion.identity) as GameObject;
                break;
            case Direction.UR:
                room = Instantiate(roomImage, startRoomPositions[2].position, Quaternion.identity) as GameObject;
                break;
            case Direction.DR:
                room = Instantiate(roomImage, startRoomPositions[3].position, Quaternion.identity) as GameObject;
                break;
        }
        room.transform.SetParent(transform);
        room.transform.localScale = Vector3.one;
        rooms.Add(room.transform);
        roomsRemaining -= 1;
    }
    void SpawnMainRooms()
    {
        Vector3 lastRoomPosition = rooms[0].position;

        Vector3 newRoomPosOffset = Vector3.zero;

        int repeated0 = 0;
        int repeated1 = 0;
        int lastDirection = -1;
        for (int i = roomsRemaining; i > 0; i--)
        {
            GameObject room = new GameObject();
            int random = Random.Range(0, 2);

            switch (random)
            {
                case 0:
                    repeated0 += 1;
                    break;
                case 1:
                    repeated1 += 1;
                    break;
            }
            if (lastDirection == random)
            {
                print(random);
                if (repeated0 > maxRepeats)
                {
                    random = 1;
                    repeated0 = 0;
                }
                else if (repeated1 > maxRepeats)
                {
                    random = 0;
                    repeated1 = 0;
                }

            }
            lastDirection = random;

            switch (levelDirection)
            {
                case Direction.UR:
                    if (random == 0)
                        newRoomPosOffset = new Vector3(0, 72, 0);
                    else
                        newRoomPosOffset = new Vector3(128, 0, 0);
                    break;
                case Direction.DR:
                    if (random == 0)
                        newRoomPosOffset = new Vector3(0, -72, 0);
                    else
                        newRoomPosOffset = new Vector3(128, 0, 0);
                    break;
                case Direction.DL:
                    if (random == 0)
                        newRoomPosOffset = new Vector3(0, -72, 0);
                    else
                        newRoomPosOffset = new Vector3(-128, 0, 0);
                    break;
                case Direction.UL:
                    if (random == 0)
                        newRoomPosOffset = new Vector3(0, 72, 0);
                    else
                        newRoomPosOffset = new Vector3(-128, 0, 0);
                    break;
            }

            room = Instantiate(roomImage, lastRoomPosition, Quaternion.identity) as GameObject;

            room.transform.SetParent(transform);
            room.transform.localScale = Vector3.one;
            room.transform.localPosition += newRoomPosOffset;
            rooms.Add(room.transform);
            lastRoomPosition = room.transform.position;
            roomsRemaining -= 1;
        }
    }
    void SpawnExtraRooms()
    {
        // get suitable rooms
        switch (_extraRoomsDirection)
        {
            case ExtraRoomsDirection.HOR:
                for (int i = 1; i < rooms.Count - 1; i++)
                {
                    if (rooms[i].localPosition.x + 128 != rooms[i + 1].localPosition.x && rooms[i].localPosition.x + 128 != rooms[i - 1].localPosition.x)
                    {
                        suitableRoomPositive.Add(rooms[i].position);
                        print(rooms[i - 1].localPosition.x + " " + rooms[i].localPosition.x + " " + rooms[i + 1].localPosition.x);
                    }
                    if (rooms[i].localPosition.x - 128 != rooms[i + 1].localPosition.x && rooms[i].localPosition.x - 128 != rooms[i - 1].localPosition.x)
                    {
                        suitableRoomNegative.Add(rooms[i].position);
                        print(rooms[i - 1].localPosition.x + " " + rooms[i].localPosition.x + " " + rooms[i + 1].localPosition.x);
                    }
                }
                break;
            case ExtraRoomsDirection.VER:
                for (int i = 1; i < rooms.Count - 1; i++)
                {
                    if (rooms[i].localPosition.y + 72 != rooms[i + 1].localPosition.y && rooms[i].localPosition.y + 72 != rooms[i - 1].localPosition.y)
                    {
                        suitableRoomPositive.Add(rooms[i].position);
                        print(rooms[i - 1].localPosition.y + " " + rooms[i].localPosition.y + " " + rooms[i + 1].localPosition.y);
                    }
                    if (rooms[i].localPosition.y - 72 != rooms[i + 1].localPosition.y && rooms[i].localPosition.y - 72 != rooms[i - 1].localPosition.y)
                    {
                        suitableRoomNegative.Add(rooms[i].position);
                        print(rooms[i - 1].localPosition.y + " " + rooms[i].localPosition.y + " " + rooms[i + 1].localPosition.y);
                    }
                }
                break;
        }
    }
    void PickLevelDirection()
    {
        int random = Random.Range(0, 4);
        switch (random)
        {
            case 0:
                levelDirection = Direction.UR;
                break;
            case 1:
                levelDirection = Direction.DR;
                break;
            case 2:
                levelDirection = Direction.DL;
                break;
            case 3:
                levelDirection = Direction.UL;
                break;
        }

        int random2 = Random.Range(0, 2);
        switch (random2)
        {
            case 0:
                _extraRoomsDirection = ExtraRoomsDirection.HOR;
                break;
            case 1:
                _extraRoomsDirection = ExtraRoomsDirection.VER;
                break;
        }
    }
}