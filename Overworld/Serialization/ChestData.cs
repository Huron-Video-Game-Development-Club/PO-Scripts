using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used for serializing Chests
// Literally just stores whether chest is opened, 
// so this information can be serialized
[System.Serializable]
public class ChestData {
    public bool opened;

    public ChestData(Chest chest) {
        opened = chest.IsOpened();
    } // ChestData()
}
