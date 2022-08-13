using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Wrapper class for Items
//Contains quantity of item and item itself
public class ItemWrapper
{
    Item item;
    int quantity;

    //Constructor - Initializes item as item_input and sets quantity to 1
    public ItemWrapper(Item item_input)
    {
        item = item_input;
        quantity = 1;
    }

    //Constructor - Initializes item as item_input and quantity as quantity_input
    public ItemWrapper(Item item_input, int quantity_input)
    {
        item = item_input;
        quantity = quantity_input;
    }

    public Use_Type GetUseType()
    {
        return item.use_type;
    }
    
    // Use an item and decrease the quantity of that item
    public void Use() {
        if(BattleManager.Instance == null && !item.usableOutsideOfBattle) 
            return;
        
        item.Use();
        --quantity;
    }

    //Returns quantity of item
    public int GetQuantity()
    {
        return quantity;
    }

    //Obtain an item by increasing its quantity
    public void Obtain()
    {
        ++quantity;
    }

    public void Obtain(int numObtained)
    {
        quantity += numObtained;
    }

    public string GetName()
    {
        return item.item_name;
    }

    public Sprite GetIcon() {
        return item.icon;
    }
}
