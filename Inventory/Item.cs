using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : InventoryType
{
    public string item_name;
    public bool usableOutsideOfBattle;
    public Use_Type use_type;
    public UnityEvent use_item;
    public Sprite icon;
    public void Use() {
        use_item.Invoke();
    }
}
