using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Encounter {
    public Enemy[] enemies;
    public float percentage;
    public int levelLow;
    public int levelHigh;
    public bool mandatory = false;

    public bool hasPercentage() {
        return percentage != 0;
    }
}
