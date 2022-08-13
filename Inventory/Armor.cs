using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Armor is sorted in inventory based on type
[System.Serializable]
public enum Armor_Type {
    Head,
    Body,
    Arm,
    Accessory
}

// Class that stores attributes about armor equiptables
[CreateAssetMenu(fileName = "New Armor", menuName = "Armor")]
public class Armor : InventoryType {
    public Character equippedBy; // deprecated?
    public string armorName;
    public Armor_Type type;
    
    public int weight;

    public int attackUp;
    public int defenseUp;
    public int magicUp;
    public int magicDefenseUp;
    public int speedUp;
    public int critUp;
    public int evasionUp;
    public int dexterityUp;

    public int attackBonus;
    public int defenseBonus;
    public int magicBonus;
    public int magicDefenseBonus;
    public int speedBonus;
    public int critBonus;
    public int evasionBonus;
}
