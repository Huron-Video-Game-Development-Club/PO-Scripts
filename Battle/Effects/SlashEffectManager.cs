using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using Unity.

public class SlashEffectManager : MonoBehaviour
{
    //public Animator animator;
    //public GameObject slash;


    public static SlashEffectManager Spawn(GameObject enemy, CharacterRuntime attacker, int damage, bool multiple)
    {
        Transform slashEffectObject = Instantiate(GameAsset.Instance.slashEffect, enemy.transform.position, Quaternion.identity);
        SlashEffectManager slashEffect = slashEffectObject.gameObject.GetComponent<SlashEffectManager>();

        slashEffect.Setup(enemy, attacker.gameObject, damage, multiple);

        return slashEffect;
    }

    public static SlashEffectManager Spawn(GameObject enemy, EnemyRuntime attacker, int damage, bool multiple)
    {
        Transform slashEffectObject = Instantiate(GameAsset.Instance.slashEffect, enemy.transform.position, Quaternion.identity);
        SlashEffectManager slashEffect = slashEffectObject.gameObject.GetComponent<SlashEffectManager>();

        slashEffect.Setup(enemy, attacker.gameObject, damage, multiple);

        Vector3 flipScale = slashEffect.transform.localScale;
        flipScale.x *= -1;
        slashEffect.transform.localScale = flipScale;

        slashEffect.GetComponent<SpriteRenderer>().sortingOrder = enemy.GetComponent<SpriteRenderer>().sortingOrder + 1;

        return slashEffect;
    }

    /*public static SlashEffectManager Spawn(List<GameObject> enemies, int damage,  UnityEvent uEvent)
    {
        uEvent.Invoke();
        GameObject enemy = enemies[enemies.Count - 1];
        Transform slashEffectObject = Instantiate(GameAsset.Instance.slashEffect, enemy.transform.position, Quaternion.identity);
        SlashEffectManager slashEffect = slashEffectObject.gameObject.GetComponent<SlashEffectManager>();

        slashEffect.Setup(damage, enemy, enemies, uEvent);

        return slashEffect;
    }*/

    private int damage_output;
    private GameObject target;
    private bool spawnMultiple;
    private GameObject mover;
    //private List<GameObject> targets;
    //private int times_played;
    //UnityEvent actSkills;

    void Setup(GameObject enemy, GameObject attacker, int damage, bool multiple)
    {
        target = enemy;
        mover = attacker;
        damage_output = damage;
        spawnMultiple = multiple;
    }



    /*void Setup(int damage, GameObject enemy, List<GameObject> enemies, UnityEvent unityEvent)
    {

        damage_output = damage;
        target = enemy;
        targets = enemies;
        actSkills = unityEvent;
    }*/
    //Set target and damage_output
    /*public void SetDamageOutput(int damage, GameObject enemy)
    {
        damage_output = damage;
        target = enemy;
    }*/

    //End Slash Animation
    public void EndAnimation()
    {
        if(mover == null) {
            BattleManager.Instance.allowMovement();
            Destroy(gameObject);
            return;
        }
        
        if(!spawnMultiple) {
            if (mover.GetComponent<CharacterRuntime>() != null)
            {
                mover.GetComponent<CharacterRuntime>().FinishAttack();
                BattleManager.Instance.allowMovement();
            }
            else if (mover.GetComponent<EnemyRuntime>() != null)
            {
                mover.GetComponent<EnemyRuntime>().FinishAttack();
                BattleManager.Instance.allowMovement();
            }
            
        }
        Destroy(gameObject);
    }

    //Output damage near target
    public void OutputDamage()
    {
        if(target == null) {
            if(!spawnMultiple && BattleManager.Instance.InBreak()) {
                BattleManager.Instance.EndBreak();
            }
            return;
        }
        DamageEffect.Spawn(target.transform.position, damage_output);
        if (target.GetComponent<EnemyRuntime>())
        {
            target.GetComponent<EnemyRuntime>().UpdateTag(spawnMultiple);
        }
        else if (target.GetComponent<CharacterRuntime>())
        {
            target.GetComponent<CharacterRuntime>().UpdateHP();
        }

        try {
            AudioManager.Instance.PlayNoNext("Hit");
        } catch {
            return;
        }
    }

    public void SpawnNext()
    {
        if (spawnMultiple)
        {
            //Destroy(gameObject);
            if(mover.GetComponent<CharacterRuntime>() != null) {
                mover.GetComponent<CharacterRuntime>().character.skillList.ContinueSkill();
            } else if(mover.GetComponent<EnemyRuntime>() != null) {
                mover.GetComponent<EnemyRuntime>().ContinueSkill();
            }
            
        }
        
    }

    /*public void UpdateEnemyTag()
    {

    }*/
}
