using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMapGenerator : MonoBehaviour
{

    public enum Direction { Left, Up, Right, Down };
    public Direction excludeDirecion = Direction.Left;

    public GameObject roomImage;
    public GameObject passageImage;

    public List<Transform> rooms;
    int roomsRemaining = 6;

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
        rooms.Add(room.transform);
        roomsRemaining -= 1;
    }
    void SpawnMainRooms()
    {
        Vector3 lastRoomPosition = rooms[0].position;

        Vector3 newRoomPosOffset = Vector3.zero;

        for (int i = roomsRemaining; i > 0; i--)
        {
            List<Direction> newDirection = new List<Direction>();

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


            RaycastHit2D hitLeft = Physics2D.Raycast(lastRoomPosition, Vector2.left, 1.28f);
            if (hitLeft.collider != null && excludeDirecion != Direction.Left)
            {
                if (hitLeft.collider.tag == "MapRoom")
                    newDirection.Remove(Direction.Left);
            }

            RaycastHit2D hitUp = Physics2D.Raycast(lastRoomPosition, Vector2.up, 0.72f);
            if (hitUp.collider != null && excludeDirecion != Direction.Up)
            {
                if (hitLeft.collider.tag == "MapRoom")
                    newDirection.Remove(Direction.Up);
            }

            RaycastHit2D hitRight = Physics2D.Raycast(lastRoomPosition, Vector2.right, 1.28f);
            if (hitRight.collider != null && excludeDirecion != Direction.Right)
            {
                if (hitLeft.collider.tag == "MapRoom")
                    newDirection.Remove(Direction.Right);
            }

            RaycastHit2D hitDown = Physics2D.Raycast(lastRoomPosition, Vector2.down, 0.72f);
            if (hitDown.collider != null && excludeDirecion != Direction.Down)
            {
                if (hitLeft.collider.tag == "MapRoom")
                    newDirection.Remove(Direction.Down);
            }
            newDirection.Sort();
            print(newDirection.Count);

            int random = Random.Range(0, newDirection.Count);

            switch (newDirection[random])
            {
                case Direction.Left:
                    newRoomPosOffset = new Vector3(-1.28f, 0, 0);
                    break;
                case Direction.Up:
                    newRoomPosOffset = new Vector3(0, 0.72f, 0);
                    break;
                case Direction.Right:
                    newRoomPosOffset = new Vector3(1.28f, 0, 0);
                    break;
                case Direction.Down:
                    newRoomPosOffset = new Vector3(0, -0.72f, 0);
                    break;
            }

            GameObject room = Instantiate(roomImage, lastRoomPosition, Quaternion.identity) as GameObject;

            room.name = "mainRoom";
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
}