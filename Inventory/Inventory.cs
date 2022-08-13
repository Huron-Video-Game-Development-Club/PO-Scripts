using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {
    private List<ItemWrapper> items = new List<ItemWrapper>();
    private List<ItemWrapper> keyItems = new List<ItemWrapper>();
    private List<WeaponWrapper>[] weapons = new List<WeaponWrapper>[6];
    private List<ArmorWrapper>[] armorList = new List<ArmorWrapper>[4];
    private int cash;

    // private Dictionary<Item, int> itemList; 

    public Inventory(List<Item> start_items, List<Weapon> start_weapons) {
        for(int i = 0; i < 6; ++i) { weapons[i] = new List<WeaponWrapper>(); }
        for(int i = 0; i < 4; ++i) { armorList[i] = new List<ArmorWrapper>(); }
        
        for(int i = 0; i < start_items.Count; ++i) {
            AddItem(start_items[i]);
        }

        for(int i = 0; i < start_weapons.Count; ++i) {
            AddWeapon(start_weapons[i]);
        }
    }

    // Increment item quantity if it is already there, otherwise, add item
    /*public void AddItem(Item item) {
        bool alreadyAdded = false;
        foreach(ItemWrapper wrappedItem in items) {
            Debug.Log("IN HERE: " + wrappedItem.GetName() + " " + item.item_name);
            if(wrappedItem.GetName() == item.item_name) {
                wrappedItem.Obtain();
                alreadyAdded = true;
                //Debug.Log("ITEM INCREMENTED");
                break;
            }
        }
        
        if (!alreadyAdded) {
            ItemWrapper wrappedItem = new ItemWrapper(item);
            items.Add(wrappedItem);
            //Debug.Log("ITEM ADDED");
        }
    }*/

    // Adds a inventory typed object to the inventory
    public void AddToInventory(InventoryType input, int quantity) {
        switch(input.itemType) {
            case InventoryType.Item_Type.item:
                AddItem((Item)input, quantity);
                break;
            case InventoryType.Item_Type.keyItem:
                AddKeyItem((Item)input, quantity);
                break;
            case InventoryType.Item_Type.weapon:
                AddWeapon((Weapon)input, quantity);
                break;
            case InventoryType.Item_Type.armor:
                AddArmor((Armor)input, quantity);
                break;
        }
    }

    // Adds an item to items based on item and quantity
    public void AddItem(Item item, int quantity = 1) {
        bool alreadyAdded = false;
        foreach (ItemWrapper wrappedItem in items) {
            if (wrappedItem.GetName() == item.item_name) {
                wrappedItem.Obtain(quantity);
                alreadyAdded = true;
                break;
            }
        }

        if (!alreadyAdded) {
            ItemWrapper wrappedItem = new ItemWrapper(item, quantity);
            items.Add(wrappedItem);
        }
    } // AddItem()

    // Adds an item to items based on item and quantity
    public void AddKeyItem(Item item, int quantity = 1) {
        bool alreadyAdded = false;
        foreach (ItemWrapper wrappedItem in keyItems) {
            if (wrappedItem.GetName() == item.item_name) {
                wrappedItem.Obtain(quantity);
                alreadyAdded = true;
                break;
            }
        }

        if (!alreadyAdded) {
            ItemWrapper wrappedItem = new ItemWrapper(item, quantity);
            keyItems.Add(wrappedItem);
        }
    } // AddItem()

    // Adds an weapon to weapons based on weapon and quantity
    public void AddWeapon(Weapon weapon, int quantity = 1) {
        bool alreadyAdded = false;
        foreach (WeaponWrapper wrappedWeapon in weapons[(int)weapon.type]) {
            if (wrappedWeapon.GetName() == weapon.weaponName) {
                wrappedWeapon.Obtain(quantity);
                alreadyAdded = true;
                break;
            }
        }

        if (!alreadyAdded) {
            WeaponWrapper wrappedItem = new WeaponWrapper(weapon, quantity);
            weapons[(int)weapon.type].Add(wrappedItem);
        }
    } // AddWeapon()

    // Adds an armor to armorList based on armor and quantity
    public void AddArmor(Armor armor, int quantity = 1) {
        bool alreadyAdded = false;
        foreach (ArmorWrapper wrappedArmor in armorList[(int)armor.type]) {
            if (wrappedArmor.GetName() == armor.armorName) {
                wrappedArmor.Obtain(quantity);
                alreadyAdded = true;
                break;
            }
        }

        if (!alreadyAdded) {
            ArmorWrapper wrappedArmor = new ArmorWrapper(armor, quantity);
            armorList[(int)armor.type].Add(wrappedArmor);
        }
    } // AddArmor

    public void UseItem(int index) {
        items[index].Use();
        if(items[index].GetQuantity() == 0) {
            items.RemoveAt(index);
        }
    }

    public bool HasKeyItem(Item item) {
        foreach (ItemWrapper wrappedItem in keyItems) {
            if (wrappedItem.GetName() != item.item_name) continue;
            return true;
        }
        return false;
    }

    public void EquipWeapon(int index, Weapon_Type type) {
        weapons[(int)type][index].Lose();
        if(weapons[(int)type][index].GetQuantity() == 0) {
            weapons[(int)type].RemoveAt(index);
        }
    }

    public void EquipArmor(int index, Armor_Type type) {
        armorList[(int)type][index].Lose();
        if(armorList[(int)type][index].GetQuantity() == 0) {
            armorList[(int)type].RemoveAt(index);
        }
    }

    public int GetNumItems() { return items.Count; }
    public int GetNumWeapons(Weapon_Type type) { return weapons[(int)type].Count; }
    public int GetNumArmor(Armor_Type type) { return armorList[(int)type].Count; }

    public ItemWrapper GetItem(int index) {
        Debug.Log(index);
        Debug.Log(items[index].GetName() + " out of range?");
        return items[index];
    }

    public WeaponWrapper GetWeapon(int index, Weapon_Type type) { return weapons[(int)type][index]; }
    public ArmorWrapper GetArmor(int index, Armor_Type type) { return armorList[(int)type][index]; }

    public void CollectMoney(int value) { cash += value; }
    public void SpendMoney(int value) { cash -= value; }
}
