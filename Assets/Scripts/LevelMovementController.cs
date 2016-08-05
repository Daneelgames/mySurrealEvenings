using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMovementController : MonoBehaviour
{

    public MapRoomController activeRoom;
    public List<GameObject> rooms;

    public void SetStartRoom(GameObject startRoom)
    {
        rooms = new List<GameObject>(GameObject.FindGameObjectsWithTag("MapRoom"));

        SetActiveRoom(startRoom);
    }

    void SetActiveRoom(GameObject room)
    {
        foreach (GameObject _room in rooms)
        {
            MapRoomController roomController = room.GetComponent<MapRoomController>();
            roomController.ActiveRoom(false);
            /*if (_room == room)
            {
                activeRoom = roomController;
                roomController.ActiveRoom(true);
            }
            else
            {
                roomController.ActiveRoom(false);
            }*/
        }
    }

    public void ButtonLeft()
    {
        switch (activeRoom.wallLeft)
        {
            case MapRoomController.Wall.Passage:
                RaycastHit2D hit = Physics2D.Raycast(activeRoom.transform.position, Vector2.left, 1.28f, 1 << 9);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "MapRoom")
                        EnterRoom(hit.collider.gameObject);
                }
                break;
        }

    }
    public void ButtonUp()
    {
        switch (activeRoom.wallUp)
        {
            case MapRoomController.Wall.Passage:
                RaycastHit2D hit = Physics2D.Raycast(activeRoom.transform.position, Vector2.up, 0.72f, 1 << 9);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "MapRoom")
                        EnterRoom(hit.collider.gameObject);
                }
                break;
        }
    }
    public void ButtonRight()
    {
        switch (activeRoom.wallRight)
        {
            case MapRoomController.Wall.Passage:
                RaycastHit2D hit = Physics2D.Raycast(activeRoom.transform.position, Vector2.right, 1.28f, 1 << 9);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "MapRoom")
                        EnterRoom(hit.collider.gameObject);
                }
                break;
        }
    }
    public void ButtonDown()
    {
        switch (activeRoom.wallDown)
        {
            case MapRoomController.Wall.Passage:
                RaycastHit2D hit = Physics2D.Raycast(activeRoom.transform.position, Vector2.down, 0.72f, 1 << 9);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "MapRoom")
                        EnterRoom(hit.collider.gameObject);
                }
                break;
        }
    }

    void EnterRoom(GameObject room)
    {
        SetActiveRoom(room);
    }
}
