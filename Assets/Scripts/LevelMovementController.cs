using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelMovementController : MonoBehaviour
{
    public MapRoomController activeRoom;
    public List<GameObject> rooms;
    public List<Image> buttons; // 0 - left
    public List<Sprite> buttonIcons; // 0 move, 1 punch, 2 key, 3 none

    public void SetStartRoom(GameObject startRoom)
    {
        rooms = new List<GameObject>(GameObject.FindGameObjectsWithTag("MapRoom"));

        SetActiveRoom(startRoom);
    }

    void SetActiveRoom(GameObject room)
    {
        MapRoomController roomController = room.GetComponent<MapRoomController>();
        foreach (GameObject _room in rooms)
        {
            MapRoomController _roomController = _room.GetComponent<MapRoomController>();
            _roomController.ActiveRoom(false);
            if (_roomController == roomController)
            {
                activeRoom = _roomController;
                _roomController.ActiveRoom(true);
            }
        }
        //set buttons types
        SetButtonsTypes();
    }

    void SetButtonsTypes()
    {
        switch (activeRoom.wallLeft)
        {
            case MapRoomController.Wall.Passage:
                buttons[0].sprite = buttonIcons[0];
                break;
            case MapRoomController.Wall.Solid:
                buttons[0].sprite = buttonIcons[1];
                break;
            case MapRoomController.Wall.DoorLocked:
                buttons[0].sprite = buttonIcons[2];
                break;
        }
        switch (activeRoom.wallUp)
        {
            case MapRoomController.Wall.Passage:
                buttons[1].sprite = buttonIcons[0];
                break;
            case MapRoomController.Wall.Solid:
                buttons[1].sprite = buttonIcons[1];
                break;
            case MapRoomController.Wall.DoorLocked:
                buttons[1].sprite = buttonIcons[2];
                break;
        }
        switch (activeRoom.wallRight)
        {
            case MapRoomController.Wall.Passage:
                buttons[2].sprite = buttonIcons[0];
                break;
            case MapRoomController.Wall.Solid:
                buttons[2].sprite = buttonIcons[1];
                break;
            case MapRoomController.Wall.DoorLocked:
                buttons[2].sprite = buttonIcons[2];
                break;
        }
        switch (activeRoom.wallDown)
        {
            case MapRoomController.Wall.Passage:
                buttons[3].sprite = buttonIcons[0];
                break;
            case MapRoomController.Wall.Solid:
                buttons[3].sprite = buttonIcons[1];
                break;
            case MapRoomController.Wall.DoorLocked:
                buttons[3].sprite = buttonIcons[2];
                break;
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
    
    public void ToggleMapTraverseIcons(bool active)
    {
        foreach(Image img in buttons)
        {
            img.gameObject.SetActive(active);
        }
        foreach(GameObject room in rooms)
        {
            room.gameObject.SetActive(active);
        }
    }
}