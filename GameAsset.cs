using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAsset : MonoBehaviour
{
    private static GameAsset _object;


    public static GameAsset Instance
    {
        get
        {
            if(_object == null)
            {
                _object = Instantiate(Resources.Load<GameAsset>("GameAsset"));
            }

            return _object;
        }
    }

    public Transform battleSceneManager;

    public Transform enemyRuntime;
    public Transform enemy_tag;
    public Transform tagUI;
    public Transform buttonWrapper;
    public Transform menuCharacterTag;
    public Transform menuItem;
    public Transform battleAlert;
    public Transform victoryTag;
    public Transform victoryScreen;

    public Transform slashEffect;
    public Transform damage_effect;
    public Transform break_effect;
    public Transform potionHealEffect;
    public Transform assasinationEffect;
}
