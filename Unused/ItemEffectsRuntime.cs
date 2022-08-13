using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectsRuntime : MonoBehaviour
{
    public ItemEffects item_effects;
    public BattleManager battle_manager;
    // Start is called before the first frame update
    void Start()
    {
        item_effects.Setup(battle_manager);
    }

    /*void SetTarget(GameObject target)
    {
        item_effects.SetTarget(target);
    }*/
}
