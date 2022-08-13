using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Wrapper class for spawning buttons
// It literally just spawns and returns a button
public class ButtonWrapper : MonoBehaviour {
    public static ButtonWrapper Spawn(Vector3 position, GameObject reference) {
        Transform buttonObject = Instantiate(GameAsset.Instance.buttonWrapper, position, Quaternion.identity);
        buttonObject.SetParent(reference.transform, false);
        if (reference.GetComponent<CharacterRuntime>() != null)
        {
            buttonObject.GetChild(0).localPosition = new Vector2(-0.05f, 1.4f);
        }
        else if (reference.GetComponent<EnemyRuntime>() != null)
        {
            buttonObject.GetChild(0).localPosition = reference.GetComponent<EnemyRuntime>().GetButtonLocation();
        }

        ButtonWrapper button = buttonObject.gameObject.GetComponent<ButtonWrapper>();

        return button;
    }
}
