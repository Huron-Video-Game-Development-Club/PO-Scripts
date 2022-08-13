using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon_Type
{
    Sword,
    Axe,
    Dagger,
    Bow,
    Spear,
    Katana
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : InventoryType
{
    public string weaponName;
    public int strength;
    public int magicStrength;
    public int weight;

    public Weapon_Type type;
    public Character equippedBy;
    
    public int attack_boost;
    public int defense_boost;
    public int magic_boost;
    public int magic_defense_boost;
    public int speed_boost;
    public int crit_boost;
    public int evasion_boost;
}
