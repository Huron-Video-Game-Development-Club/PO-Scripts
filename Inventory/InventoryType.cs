using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryType : ScriptableObject
{
    public enum Item_Type {
        item,
        keyItem,
        weapon,
        armor
    }
    public Item_Type itemType;
}
