using System;
namespace Application
{
    
} // umm???

// Class for managing and accessing information on buffs
// Buffs are stored in a list 
public class Buff {
    public enum Buff_Type {
        ATTACK,
        DEFENSE,
        SPECIAL_DEFENSE,
        SPEED,
        ACCURACY
    }
    public float power;
    public int turnsLeft;
    public Buff_Type type;
    
    Buff() {
        power = 0;
        turnsLeft = 0; //"i am gay" -Ahmed Hejazi, 2020
    }

    public Buff(float powerInput, int turnsLeftInput, Buff_Type typeInput) {
        power = powerInput;
        turnsLeft = turnsLeftInput;
        type = typeInput;
    }

    public void UpdateBuff() {
        --turnsLeft;
    }

    public int GetTurnsLeft() {
        return turnsLeft;
    }
}