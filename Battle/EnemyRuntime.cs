using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyRuntime : MonoBehaviour
{
    public static EnemyRuntime Spawn(Vector3 position, Enemy enemyInput, int levelIn)
    {
        Transform enemyObject = Instantiate(GameAsset.Instance.enemyRuntime, position, Quaternion.identity);
        enemyObject.gameObject.GetComponent<Animator>().runtimeAnimatorController = enemyInput.animtorController;
        EnemyRuntime enemyRuntime = enemyObject.gameObject.GetComponent<EnemyRuntime>();
        enemyRuntime.animator = enemyObject.gameObject.GetComponent<Animator>();

        enemyRuntime.Setup(enemyInput, levelIn);
        return enemyRuntime;
    }

    protected void Setup(Enemy enemyInput, int levelIn) {
        Debug.Log("LEVEL IN: " + levelIn);
        enemy = enemyInput;
        attack = enemy.attack_stat + levelIn;
        defense = enemy.defense_stat + levelIn;
        hp = enemy.hp_stat + levelIn * 10;
        speed = enemy.speed_stat + levelIn;
        shields = enemy.shields;
        level = levelIn;
        attack_break = enemy.attack_break + levelIn;
        exp = enemyInput.exp;
        enemy_name = enemy.enemyName;
        broke = false;
        battleManager = BattleManager.Instance;
        buffs = new List<Buff>();
    }

    public Enemy enemy;
    protected GameObject target;
    public Animator animator;
    public UnityEvent set_no_one_attacking;
    protected EnemyTag enemyTag;
    protected ButtonWrapper enemyButton;
    public BattleManager battleManager;
    protected int attack;
    public int defense;
    protected int speed;
    public int hp;
    public int shields;
    protected int attack_break;
    public int level;
    protected string enemy_name;
    protected int exp;
    protected bool broke;
    //private Vector2 tagLocation;

    protected float wait_time;
    protected float start_time;
    protected int moveGauge;

    //checks if the enemy is attack ready
    protected bool attack_ready = false;
    public int damage_out;
    public Queue<int> damageOut;
    public List<Buff> buffs;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        attack = enemy.attack_stat + level;
        defense = enemy.defense_stat + level;
        hp = enemy.hp_stat + level * 10;
        speed = enemy.speed_stat + level;
        shields = enemy.shields;
        // level = enemy.level;
        attack_break = enemy.attack_break;
        enemy_name = enemy.enemyName;
        broke = false;
        battleManager = BattleManager.Instance;
        buffs = new List<Buff>();
    }

    //attacks character game object
    public virtual void Attack(GameObject character)
    {
        int enemy_defense = character.GetComponent<CharacterRuntime>().defense;
        int damage_upper = Mathf.RoundToInt(attack * (attack + level + 1) / enemy_defense);
        int damage_lower = Mathf.RoundToInt(attack * (attack + level - 1) / enemy_defense);
        int damage = Random.Range(damage_lower, damage_upper);
        if (character.GetComponent<CharacterRuntime>().isDefending())
        {
            float damage_buffer = damage / 2f;
            damage = Mathf.RoundToInt(damage_buffer);
        }
        CharacterRuntime enemy_stats = character.GetComponent<CharacterRuntime>();
        enemy_stats.hp -= damage;
        enemy_stats.shields -= attack_break;
        //set_no_one_attacking.Invoke();
        
        damage_out = damage;
        target = character;
        //enemy_stats.UpdateHP();
        Debug.Log("HERE");
        //character.GetComponent<CharacterRuntime>().hp -= Mathf.RoundToInt(damage);
        animator.SetBool("isAttacking", true);
    }

    public void ResetMoveGauge()
    {
        moveGauge = 0;
    }
    public void AdjustMoveGauge()
    {
        moveGauge += (int)Mathf.Ceil((speed + 20) * 6 * Time.deltaTime);
        // Debug.Log(moveGauge + " out of " + BattleManager.Instance.MAX_MOVE_GAUGE);
    }
    public int GetMoveGauge()
    {
        return moveGauge;
    }

    //calculates wait time
    public void calculateWaitTime()
    {
        wait_time = 50f / speed;
        start_time = Time.deltaTime;
    }

    //updates wait time
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

    //ends animation by setting the transition bool to false
    void EndAnimation(string transition_bool)
    {
        animator.SetBool(transition_bool, false);
        //set_no_one_attacking.Invoke();
    }

    void StartSlashAnimation()
    {
        SlashEffectManager.Spawn(target, this, damage_out, false);
    }

    //makes the enemy ready to attack
    public void AttackReady()
    {
        attack_ready = true;
        for(int i = 0; i < buffs.Count; ++i) {
            buffs[i].UpdateBuff();
            if(buffs[i].GetTurnsLeft() <= 0) {
                int buffIndex = i;
                this.UnBuff(buffs[i], buffIndex);
            }
        }
    }

    //finishes attack, making enemy unable to attack
    public void FinishAttack()
    {
        attack_ready = false;

    }

    //checks if enemy is ready to attack
    public bool isAttackReady()
    {
        return attack_ready;
    }

    public void LoseShields(int shields_broken)
    {
        shields -= shields_broken;
        if(shields < 0)
        {
            shields = 0;
        }
    }

    public int GetShields()
    {
        return shields;
    }

    public int GetMaxShields()
    {
        return enemy.shields;
    }

    public void RestoreShields()
    {
        shields = enemy.shields;
        broke = false;
    }



    public void SetTarget(GameObject target_input)
    {
        target = target_input;
    }

    public void UpdateHP()
    {
        target.GetComponent<CharacterRuntime>().UpdateHP();
    }

    public Vector2 GetTagLocation()
    {
        return enemy.tagLocation;
    }

    public Vector2 GetButtonLocation()
    {
        return enemy.buttonLocation;
    }

    public string GetName()
    {
        return enemy_name;
    }

    public void SetEnemyTag(EnemyTag tag)
    {
        enemyTag = tag;
    }

    public void UpdateTag(bool noBreakUpdateInBreak)
    {
        enemyTag.UpdateGauge(noBreakUpdateInBreak);
    }

    public void SetButton(ButtonWrapper button)
    {
        enemyButton = button;
    }
    public Button GetButton()
    {
        //Debug.Log(enemyButton.gameObject.GetComponentInChildren<Button>() == null);
        return enemyButton.gameObject.GetComponentInChildren<Button>();
    }

    //specify directional input
    public void ButtonSetRightAndUp(EnemyRuntime enemy_ref)
    {
        Navigation nav = enemyButton.gameObject.GetComponentInChildren<Button>().navigation;
        nav.selectOnRight = enemy_ref.GetButton();
        nav.selectOnUp = enemy_ref.GetButton();
        enemyButton.gameObject.GetComponentInChildren<Button>().navigation = nav;
    }

    public void ButtonSetLeftAndDown(EnemyRuntime enemy_ref)
    {
        Navigation nav = enemyButton.gameObject.GetComponentInChildren<Button>().navigation;
        nav.selectOnLeft = enemy_ref.GetButton();
        nav.selectOnDown = enemy_ref.GetButton();
        enemyButton.gameObject.GetComponentInChildren<Button>().navigation = nav;
    }



    public bool hasWeakness(Affinity affinity)
    {
        foreach(Affinity weakness in enemy.weaknesses)
        {
            if(affinity.Equals(weakness))
            {
                return true;
            }
        }
        return false;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public int GetLevel()
    {
        return level;
    }

    public bool Broke()
    {
        return broke;
    }
    
    public virtual void Break() {
        broke = true;
    }

    public int GetExp() { return exp; }

    // Applies a buff to character
    // Buff buff -> buff being applied
    public void ApplyBuff(Buff buff)
    {
        if (buff.type == Buff.Buff_Type.ATTACK)
        {
            attack = (int)(attack*buff.power);
        }
        else if (buff.type == Buff.Buff_Type.DEFENSE)
        {
            defense = (int)(defense*buff.power);
        }
        else if (buff.type == Buff.Buff_Type.SPEED)
        {
            speed = (int)(speed*buff.power);
        }
        buffs.Add(buff);
    }

    // Undos buff once turn limit is up and removes it from "buffs"
    // Buff buff -> buff being removed
    // int buffIndex -> index of buff
    public void UnBuff(Buff buff, int buffIndex)
    {
        if (buff.type == Buff.Buff_Type.ATTACK)
        {
            attack = (int)(attack/buff.power);
        }
        else if (buff.type == Buff.Buff_Type.DEFENSE)
        {
            defense = (int)(defense/buff.power);
        }
        else if (buff.type == Buff.Buff_Type.SPEED)
        {
            speed = (int)(speed/buff.power);
        }
        buffs.RemoveAt(buffIndex);
    }

    public virtual void ContinueSkill() {
        FinishAttack();
        BattleManager.Instance.allowMovement();
    }
}
