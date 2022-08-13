using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class Enemy : ScriptableObject
{
    public string enemyName;
    public int hp_stat;
    public int defense_stat;
    public int attack_stat;
    public int speed_stat;
    public int level;
    public int shields;
    public int attack_break;
    public int exp;

    public RuntimeAnimatorController animtorController;
    public Vector2 tagLocation;
    public Vector2 buttonLocation;

    //active time variables
    private float wait_time;
    private float start_time;

    public List<Affinity> weaknesses;

    public void calculateWaitTime()
    {
        wait_time = 50f / speed_stat;
        start_time = Time.deltaTime;
    }

    //adjust wait time so that it can be measured
    public void adjustWaitTime()
    {
        //Debug.Log(Time.deltaTime - start_time);
        //wait_time -= Time.deltaTime - start_time;
        wait_time -= 0.01f;
    }

    //returns wait time
    public float getWaitTime()
    {
        return wait_time;
    }


    /*public void Attack(GameObject character)
    {
        int enemy_defense = character.GetComponent<Character>().defense_stat;
        float damage = attack_stat * (attack_stat + level) / enemy_defense;
        character.GetComponent<Character>().hp_stat -= Mathf.RoundToInt(damage);
    }*/

}
