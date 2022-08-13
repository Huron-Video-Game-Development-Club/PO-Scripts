using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Another bad design choice by Mr. Peter Schwendeman
 * --------------------------------------------------
 * This class takes variable inputs when a battle begins and stores them 
 * statically (through a singleton) for the BattleManager to access on start. 
 * 
 * Why is this bad? The indirect method of storing and accessing is done 
 * in very close proximity to one other. An instance of the BattleSceneManager is created 
 * and the variables are stored just before loading the BattleScene 
 *   > (possibly asynchronously? Not sure exactly how unity works)
 * 
 * I don't know why I didn't just have a persistant BattleManager 
 * that takes in these inputs directly in some kind of EnterBattle() method,
 * but I had some kind of reason.
 *   > Maybe for testing Battles directly? I'm not sure
 *
 * Anyway, this works for now and it allows me to work with BossBattles easily
 */

// Manager class for changing scenes between overworld and battle
// Takes variable inputs when a battle begins and stores them 
// statically (through a singleton) for the BattleManager to access on start. 
public class BattleSceneManager : MonoBehaviour {
    public static BattleSceneManager Spawn(Enemy[] enemiesInit, List<int> levels, string returnScene, string battleMusic, bool mandatory) {
        Transform battleSceneManagerObject = Instantiate(GameAsset.Instance.battleSceneManager, new Vector3(0,0), Quaternion.identity);
        BattleSceneManager battleSceneManager = battleSceneManagerObject.gameObject.GetComponent<BattleSceneManager>();
        
        battleSceneManager.Setup(enemiesInit, levels, returnScene, battleMusic, mandatory);

        return battleSceneManager;
    } // Spawn()

    private static BattleSceneManager instance;

    // Static reference to the BattleSceneManager
    // Allows battle manager to access variables
    public static BattleSceneManager Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<BattleSceneManager>();
            }
            return instance;
        }
    }

    // Declare this object as a Singleton
    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    } // Awake()

    public Enemy[] enemiesInit;
    public bool ableToFlee = true;
    List<int> levelsInit = new List<int>();
    private List<Vector3> returnPositions = new List<Vector3> { };
    string reScene;
    string bMusic;

    void Setup(Enemy[] enemiesInput, List<int> levelsInput, string returnSceneInit, string battleMusic, bool mandatory) {
        AudioManager.Instance.Pause();
        AudioManager.Instance.PlayNoNext(battleMusic);
    
        enemiesInit = enemiesInput;
        levelsInit = levelsInput;
        bMusic = battleMusic;
        ableToFlee = !mandatory;

        Debug.Log("enemiesInit: " + enemiesInit.Length);
        Debug.Log("enemiesInput: " + enemiesInput.Length);
        
        reScene = returnSceneInit;
        
        for (int i = 0; i < Party.Instance.current_party.Count; ++i) {
            Debug.Log(Party.Instance.current_party[i].transform.position);
            Vector3 returnPos = Party.Instance.current_party[i].transform.position;
            returnPositions.Add(returnPos);
            Party.Instance.current_party[i].transform.position.Set(0, 0, 0);
            Party.Instance.current_party[i].GetComponent<CharacterRuntime>().EnterBattle();
            Party.Instance.current_party[i].GetComponent<SpriteRenderer>().sortingOrder = 1 + (2 * i);
        }
    } // Setup()

    public Enemy[] GetEnemiesInit() {
        return enemiesInit;
    } // GetEnemiesInit()

    public List<int> GetLevelsInit() {
        return levelsInit;
    } // GetEnemiesInit()

    public string GetBattleMusicName() {
        return bMusic;
    } // GetBattleMusicName()

    // Calls all environmental factors to exit battle modes
    // Input: stopMusic = false -> specifies whether the AudioManager needs to stop the battle music
    public void EndBattle() {
        SwitchToBattle.Instance.new_scene = reScene;
        SwitchToBattle.Instance.FadeToLevel();
        MainVCam.Instance.ExitBattle();
        CameraController.Instance.ExitBattle();

        for (int i = 0; i < Party.Instance.current_party.Count; ++i) {
            Debug.Log(Party.Instance.current_party.Count);
            Party.Instance.current_party[i].GetComponent<CharacterRuntime>().ExitBattle();
            Party.Instance.current_party[i].transform.position = returnPositions[i];
            Party.Instance.current_party[i].GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        
        AudioManager.Instance.Stop(bMusic, true);
        AudioManager.Instance.UnPause();

        Destroy(gameObject);
    } // EndBattle()

    private IEnumerator ExitBattleScene() {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(reScene);
    } // ExitBattleScene()
}
