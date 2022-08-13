using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Haven't touched this class since it was made
// Spawns and manages break effect
public class BreakEffect : MonoBehaviour {
    public static BreakEffect Spawn(Vector3 position, GameObject canvas, BattleManager battleManager) {
        Transform BreakEffectObject = Instantiate(GameAsset.Instance.break_effect, position, Quaternion.identity);
        BreakEffectObject.SetParent(canvas.transform, false);
        BreakEffect break_effect = BreakEffectObject.gameObject.GetComponent<BreakEffect>();

        break_effect.Setup(battleManager);

        return break_effect;
    }

    private BattleManager myManager; // just use singleton?

    public void Setup(BattleManager manager) {
        myManager = manager;
        myManager.SetBreak();
    }

    // Resets manager to normal time and destroys the object
    // Should've called this Die too lol
    public void Despawn() {
        myManager.NormalTime();
        Destroy(gameObject);
    }
}
