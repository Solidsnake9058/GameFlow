using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClickableItem : IGameItem
{
    public abstract void ClickAction(Vector3 pos);
}