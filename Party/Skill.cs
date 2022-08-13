using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public enum Use_Type
{
    Self,
    Party,
    Enemy,
    Both,
    Party_All,
    Enemy_All
}

public enum Affinity
{
    Simple,
    Fire,
    Water,
    Earth,
    Wind,
    Forest,
    Lightning,
    Ice,
    Light,
    Dark,
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string skill_name;
    public string short_name;
    public UnityEvent use_skill;
    public int spCost;
    public bool target_skill;
    public bool randomize_targets;
    public int attack_times;
    public Use_Type use_type;
    public Affinity affinity;

    public int breakPower;
    public int shieldRepair;

    public void Use(CharacterRuntime character)
    {
        use_skill.Invoke();
        character.sp -= spCost;
        character.UpdateSP();
    }

}
