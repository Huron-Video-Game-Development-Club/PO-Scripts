using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssassinationEffect : MonoBehaviour {
    public static AssassinationEffect Spawn(GameObject canvas) {
        Transform assasinationEffectObject = Instantiate(GameAsset.Instance.assasinationEffect, new Vector2(0, 0), Quaternion.identity);
        
        AssassinationEffect assasinationEffect = assasinationEffectObject.gameObject.GetComponentInChildren<AssassinationEffect>();

        return assasinationEffect;
    }


    public void EndAnimation() { 
        if(CutsceneManager.Instance == null) return;

        CutsceneManager.Instance.PlayNextEvent();
    }

    public void PlaySFX(string sfx) {
        try {
            AudioManager.Instance.PlayNoNext(sfx);
            Debug.Log("HERE");
        } catch {
            return;
        }
    }
}
