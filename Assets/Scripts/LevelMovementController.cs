﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelMovementController : MonoBehaviour
{
    public MapRoomController activeRoom;
    public GameObject newRoom;
    public MapRoomController lastRoom;
    public List<GameObject> rooms;
    public List<Image> buttons; // 0 - left
    public List<Sprite> buttonIcons; // 0 move, 1 punch, 2 key, 3 none
    public float buttonCooldown = 0.5f;
    public LevelMapGenerator mapGenController;

    void Update()
    {
        if (buttonCooldown > 0)
            buttonCooldown -= 1 * Time.deltaTime;
    }
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

        activeRoom.SpawnChest();
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

    void HitWall(MapRoomController room, string direction)
    {
        if (buttonCooldown <= 0)
        {
            buttonCooldown = 0.5f;
            GameManager.Instance.CameraShake(0.3f);
            if (room == null)
            {
                GameManager.Instance.HitWall(false);
            }
            else
            {
                switch (direction)
                {
                    case "Left":
                        activeRoom.SetWallType("Left", MapRoomController.Wall.Passage);
                        room.SetWallType("Right", MapRoomController.Wall.Passage);
                        break;
                    case "Up":
                        activeRoom.SetWallType("Up", MapRoomController.Wall.Passage);
                        room.SetWallType("Down", MapRoomController.Wall.Passage);
                        break;
                    case "Right":
                        activeRoom.SetWallType("Right", MapRoomController.Wall.Passage);
                        room.SetWallType("Left", MapRoomController.Wall.Passage);
                        break;
                    case "Down":
                        activeRoom.SetWallType("Down", MapRoomController.Wall.Passage);
                        room.SetWallType("Up", MapRoomController.Wall.Passage);
                        break;
                }
                GameManager.Instance.HitWall(true);
                activeRoom.UpdateNeighbours();
                SetButtonsTypes();
            }
        }
    }
    void OpenDoor(MapRoomController room, string direction)
    {
        if (buttonCooldown <= 0)
        {
            buttonCooldown = 0.5f;
            if (GameManager.Instance.inventoryController.keys > 0)
            {
                switch (direction)
                {
                    case "Left":
                        activeRoom.SetWallType("Left", MapRoomController.Wall.Passage);
                        room.SetWallType("Right", MapRoomController.Wall.Passage);
                        break;
                    case "Up":
                        activeRoom.SetWallType("Up", MapRoomController.Wall.Passage);
                        room.SetWallType("Down", MapRoomController.Wall.Passage);
                        break;
                    case "Right":
                        activeRoom.SetWallType("Right", MapRoomController.Wall.Passage);
                        room.SetWallType("Left", MapRoomController.Wall.Passage);
                        break;
                    case "Down":
                        activeRoom.SetWallType("Down", MapRoomController.Wall.Passage);
                        room.SetWallType("Up", MapRoomController.Wall.Passage);
                        break;
                }
                GameManager.Instance.inventoryController.KeyLose();
                GameManager.Instance.PrintActionFeedback(null, "You used the key and opened the door.", null, false, true);
            }
            else
            {
                GameManager.Instance.PrintActionFeedback(null, "You have no keys!", null, false, true);
            }
            SetButtonsTypes();
        }
    }
    public void ButtonLeft()
    {
        switch (activeRoom.wallLeft)
        {
            case MapRoomController.Wall.Passage:
                RaycastHit2D hitPassage = Physics2D.Raycast(activeRoom.transform.position, Vector2.left, 1.28f, 1 << 9);
                if (hitPassage.collider != null)
                {
                    if (hitPassage.collider.gameObject.tag == "MapRoom")
                        EnterRoom(hitPassage.collider.gameObject);
                }
                break;
            case MapRoomController.Wall.Solid:
                RaycastHit2D hitSolid = Physics2D.Raycast(activeRoom.transform.position, Vector2.left, 1.28f, 1 << 9);
                if (hitSolid.collider != null)
                {
                    if (hitSolid.collider.gameObject.tag == "MapRoom")
                        HitWall(hitSolid.collider.GetComponent<MapRoomController>(), "Left");
                    else
                        HitWall(null, "Left");
                }
                else
                    HitWall(null, "Left");
                break;
            case MapRoomController.Wall.DoorLocked:
                RaycastHit2D hitDoor = Physics2D.Raycast(activeRoom.transform.position, Vector2.left, 1.28f, 1 << 9);
                if (hitDoor.collider != null)
                {
                    if (hitDoor.collider.gameObject.tag == "MapRoom")
                        OpenDoor(hitDoor.collider.GetComponent<MapRoomController>(), "Left");
                    else
                        OpenDoor(null, "Left");
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
            case MapRoomController.Wall.Solid:
                RaycastHit2D hitSolid = Physics2D.Raycast(activeRoom.transform.position, Vector2.up, 0.72f, 1 << 9);
                if (hitSolid.collider != null)
                {
                    if (hitSolid.collider.gameObject.tag == "MapRoom")
                        HitWall(hitSolid.collider.GetComponent<MapRoomController>(), "Up");
                    else
                        HitWall(null, "Up");
                }
                else
                    HitWall(null, "Up");
                break;
            case MapRoomController.Wall.DoorLocked:
                RaycastHit2D hitDoor = Physics2D.Raycast(activeRoom.transform.position, Vector2.up, 0.72f, 1 << 9);
                if (hitDoor.collider != null)
                {
                    if (hitDoor.collider.gameObject.tag == "MapRoom")
                        OpenDoor(hitDoor.collider.GetComponent<MapRoomController>(), "Up");
                    else
                        OpenDoor(null, "Up");
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
            case MapRoomController.Wall.Solid:
                RaycastHit2D hitSolid = Physics2D.Raycast(activeRoom.transform.position, Vector2.right, 1.28f, 1 << 9);
                if (hitSolid.collider != null)
                {
                    if (hitSolid.collider.gameObject.tag == "MapRoom")
                        HitWall(hitSolid.collider.GetComponent<MapRoomController>(), "Right");
                    else
                        HitWall(null, "Right");
                }
                else
                    HitWall(null, "Right");
                break;
            case MapRoomController.Wall.DoorLocked:
                RaycastHit2D hitDoor = Physics2D.Raycast(activeRoom.transform.position, Vector2.right, 1.28f, 1 << 9);
                if (hitDoor.collider != null)
                {
                    if (hitDoor.collider.gameObject.tag == "MapRoom")
                        OpenDoor(hitDoor.collider.GetComponent<MapRoomController>(), "Right");
                    else
                        OpenDoor(null, "Right");
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
            case MapRoomController.Wall.Solid:
                RaycastHit2D hitSolid = Physics2D.Raycast(activeRoom.transform.position, Vector2.down, 0.72f, 1 << 9);
                if (hitSolid.collider != null)
                {
                    if (hitSolid.collider.gameObject.tag == "MapRoom")
                        HitWall(hitSolid.collider.GetComponent<MapRoomController>(), "Down");
                    else
                        HitWall(null, "Down");
                }
                else
                    HitWall(null, "Down");
                break;
            case MapRoomController.Wall.DoorLocked:
                RaycastHit2D hitDoor = Physics2D.Raycast(activeRoom.transform.position, Vector2.down, 0.72f, 1 << 9);
                if (hitDoor.collider != null)
                {
                    if (hitDoor.collider.gameObject.tag == "MapRoom")
                        OpenDoor(hitDoor.collider.GetComponent<MapRoomController>(), "Down");
                    else
                        OpenDoor(null, "Down");
                }
                break;
        }
    }

    void EnterRoom(GameObject room)
    {
        if (buttonCooldown <= 0)
        {
            buttonCooldown = 0.5f;
            newRoom = room;
            GameManager.Instance.ChangeRoom();
        }
    }

    public void ChangeRoom()
    {
        Destroy(activeRoom.chestInRoom); //remove chest
        activeRoom.chestInRoom = null;  //remove chest

        lastRoom = activeRoom;
        SetActiveRoom(newRoom.gameObject);
    }

    public void RunFromBattle()
    {
        activeRoom.EscapedFromRoom(); //change rooms spawn rate
        EnterRoom(lastRoom.gameObject);
    }

    public void ToggleMapTraverseIcons(bool active)
    {
        mapGenController.mapBack.SetActive(active);
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