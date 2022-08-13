using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemEffects", menuName = "ItemEffects")]
public class ItemEffects : ScriptableObject
{
    private BattleManager battleManager;
    /*private GameObject target;
    private List<GameObject> party;
    private List<GameObject> enemies;*/

    // Start is called before the first frame update
    public void Setup(BattleManager battleManagerInput) {
        battleManager = battleManagerInput;
    }

    /*public void SetTarget(GameObject target_input)
    {
        target = target_input;
    }*/

    // Method for Potions
    public void Potion() {
        if(BattleManager.Instance == null) {
            Debug.Log("IN THIS PART");
            if(Party.Instance.current_party[GameMenu.Instance.GetTargetIndex()].GetComponent<CharacterRuntime>().hp + 300 > Party.Instance.current_party[GameMenu.Instance.GetTargetIndex()].GetComponent<CharacterRuntime>().GetMaxHP()) {
                Party.Instance.current_party[GameMenu.Instance.GetTargetIndex()].GetComponent<CharacterRuntime>().hp = Party.Instance.current_party[GameMenu.Instance.GetTargetIndex()].GetComponent<CharacterRuntime>().GetMaxHP();
            } else {
                Party.Instance.current_party[GameMenu.Instance.GetTargetIndex()].GetComponent<CharacterRuntime>().hp += 300;
            }
            return;
        }

        if (BattleManager.Instance.InBreak()) {
            if (BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().hp + 300 > BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().GetMaxHP()) {
                BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().hp += BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().GetMaxHP() - BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().hp;
            } else {
                BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().hp += 300;
            }
            PotionHealEffect.Spawn(BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>());
        } else {
            if (BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().hp + 300 > BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().GetMaxHP()) {
                BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().hp += BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().GetMaxHP() - BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().hp;
            } else {
                BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().hp += 300;
            }
            PotionHealEffect.Spawn(BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>());
        }
    }

    // Method for Elixirs
    public void Elixir() {
        if(BattleManager.Instance == null) {
            if(Party.Instance.current_party[GameMenu.Instance.GetTargetIndex()].GetComponent<CharacterRuntime>().sp + 50 > Party.Instance.current_party[GameMenu.Instance.GetTargetIndex()].GetComponent<CharacterRuntime>().GetMaxSP()) {
                Party.Instance.current_party[GameMenu.Instance.GetTargetIndex()].GetComponent<CharacterRuntime>().sp = Party.Instance.current_party[GameMenu.Instance.GetTargetIndex()].GetComponent<CharacterRuntime>().GetMaxSP();
            } else {
                Party.Instance.current_party[GameMenu.Instance.GetTargetIndex()].GetComponent<CharacterRuntime>().sp += 50;
            }
            return;
        }

        if (BattleManager.Instance.InBreak()) {
            if (BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().sp + 50 > BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().GetMaxSP()) {
                BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().sp += BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().GetMaxSP() - BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().sp;
            } else {
                BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().sp += 50;
            }
            PotionHealEffect.Spawn(BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>());
        } else {
            if (BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().sp + 50 > BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().GetMaxSP()) {
                BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().sp += BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().GetMaxSP() - BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().sp;
            } else {
                BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().sp += 50;
            }
            PotionHealEffect.Spawn(BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>());
        }

    }

    // Method for Fermented Grape Juice
    public void FermentedGrapeJuice() {
        Buff strength = new Buff(2.0f, 5, Buff.Buff_Type.ATTACK);
        Buff accuracy = new Buff(0.5f, 5, Buff.Buff_Type.ACCURACY);
        if (BattleManager.Instance.InBreak()) {
            BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().ApplyBuff(strength);
            BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>().ApplyBuff(accuracy);
            PotionHealEffect.Spawn(BattleManager.Instance.GetParty()[BattleManager.Instance.GetTargetIndexDuringBreak()].GetComponent<CharacterRuntime>());
        } else {
            BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().ApplyBuff(strength);
            BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>().ApplyBuff(accuracy);
            
            PotionHealEffect.Spawn(BattleManager.Instance.GetTarget().GetComponent<CharacterRuntime>());
        }
    }
}
