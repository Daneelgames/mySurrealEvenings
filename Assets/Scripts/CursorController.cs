﻿using UnityEngine;
using System.Collections;

public class CursorController : MonoBehaviour
{

    public GameObject cursorSprite;
    public Canvas myCanvas;

    // Use this for initialization
    void Awake()
    {
        Cursor.visible = false;
    }

    void Update()
    {  Vector2 pos;
         RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out pos);
         cursorSprite.transform.position = myCanvas.transform.TransformPoint(pos);
    }
}