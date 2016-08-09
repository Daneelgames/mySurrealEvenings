using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelMovementController : MonoBehaviour
{
    public MapRoomController activeRoom;
    public MapRoomController lastRoom;
    public List<GameObject> rooms;
    public List<Image> buttons; // 0 - left
    public List<Sprite> buttonIcons; // 0 move, 1 punch, 2 key, 3 none

    public void SetStartRoom(GameObject startRoom)
    {
        rooms = new List<GameObject>(GameObject.FindGameObjectsWithTag("MapRoom"));

        SetActiveRoom(startRoom);
        lastRoom = activeRoom;
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
        roomController.ShowRoom(true);
        roomController.ShowPassages(true);
        GetRoomNeighbours(roomController);
        roomController.UpdateNeighbours();

    }

    void GetRoomNeighbours(MapRoomController room)
    {
        RaycastHit2D hitL = Physics2D.Raycast(activeRoom.transform.position, Vector2.left, 1.28f, 1 << 9);
        if (hitL.collider != null)
        {
            if (hitL.collider.gameObject.tag == "MapRoom")
                room.AddNeighbourLeft(hitL.collider.gameObject.GetComponent<MapRoomController>());
        }
        RaycastHit2D hitU = Physics2D.Raycast(activeRoom.transform.position, Vector2.up, 0.72f, 1 << 9);
        if (hitU.collider != null)
        {
            if (hitU.collider.gameObject.tag == "MapRoom")
                room.AddNeighbourUp(hitU.collider.gameObject.GetComponent<MapRoomController>());
        }
        RaycastHit2D hitR = Physics2D.Raycast(activeRoom.transform.position, Vector2.right, 1.28f, 1 << 9);
        if (hitR.collider != null)
        {
            if (hitR.collider.gameObject.tag == "MapRoom")
                room.AddNeighbourRight(hitR.collider.gameObject.GetComponent<MapRoomController>());
        }
        RaycastHit2D hitD = Physics2D.Raycast(activeRoom.transform.position, Vector2.down, 0.72f, 1 << 9);
        if (hitD.collider != null)
        {
            if (hitD.collider.gameObject.tag == "MapRoom")
                room.AddNeighbourDown(hitD.collider.gameObject.GetComponent<MapRoomController>());
        }

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
        lastRoom = activeRoom;
        SetActiveRoom(room);
        GameManager.Instance.ChangeRoom();
    }

    public void RunFromBattle()
    {
        activeRoom.EscapedFromRoom(); // change rooms spawn rate
        EnterRoom(lastRoom.gameObject);
    }

    public void ToggleMapTraverseIcons(bool active)
    {
        foreach (Image img in buttons)
        {
            img.gameObject.SetActive(active);
        }
        foreach (GameObject room in rooms)
        {
            room.gameObject.SetActive(active);
        }
    }
}