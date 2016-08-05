using UnityEngine;
using System.Collections;

public class MapPassageController : MonoBehaviour
{
    public MapRoomController.Wall passageType = MapRoomController.Wall.Passage;
    public SpriteRenderer locker;

    public void SetType(MapRoomController.Wall passType)
    {
        switch (passType)
        {
            case MapRoomController.Wall.Passage:
                passageType = MapRoomController.Wall.Passage;
                locker.color = Color.white;
                break;

            case MapRoomController.Wall.DoorLocked:
                passageType = MapRoomController.Wall.DoorLocked;
                locker.color = Color.red;
                break;

            case MapRoomController.Wall.Solid:
                passageType = MapRoomController.Wall.Solid;
                locker.color = Color.clear;
                break;
        }
    }
}