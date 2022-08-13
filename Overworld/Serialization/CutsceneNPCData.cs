using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutsceneNPCData
{
    public float posX;
    public float posY;
    public CutsceneNPC.Direction_Facing status;

    public CutsceneNPCData(CutsceneNPC NPC)
    {
        posX = NPC.transform.position.x;
        posY = NPC.transform.position.y;

        status = NPC.directionFacing;
    }
}
