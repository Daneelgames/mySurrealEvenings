using UnityEngine;
using System.Collections;

public class DecorationController : MonoBehaviour {

public enum Decoration {chair, couch};
public Decoration decorType = Decoration.chair;

public string decorTitle;
public string decorDescription;
public Sprite decorIcon;
public int craftTime;


}