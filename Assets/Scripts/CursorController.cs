using UnityEngine;
using System.Collections;

public class CursorController : MonoBehaviour
{

    public Texture2D cursorSprite;

    // Use this for initialization
    void Start()
    {
        Cursor.SetCursor(cursorSprite, Vector2.zero, CursorMode.Auto);
    }
}
