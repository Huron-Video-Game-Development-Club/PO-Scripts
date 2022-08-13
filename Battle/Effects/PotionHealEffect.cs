using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionHealEffect : MonoBehaviour
{
    public static PotionHealEffect Spawn(CharacterRuntime inputCharacter)
    {
        Transform potionHealEffectObject = Instantiate(GameAsset.Instance.potionHealEffect, inputCharacter.transform.position + new Vector3(0, 0.35f), Quaternion.identity);
        PotionHealEffect potionHealEffect = potionHealEffectObject.gameObject.GetComponent<PotionHealEffect>();

        potionHealEffect.Setup(inputCharacter);
        potionHealEffect.GetComponent<SpriteRenderer>().sortingOrder = inputCharacter.GetComponent<SpriteRenderer>().sortingOrder + 1;
        return potionHealEffect;
    }

    CharacterRuntime character;
    void Setup(CharacterRuntime inputCharacter)
    {
        character = inputCharacter;
    }

    public void UpdateHealth()
    {
        character.UpdateHP();
        character.UpdateSP();
    }

    public void EndAnimation()
    {
        BattleManager.Instance.allowMovement();
        Destroy(gameObject);
    }
}
