using System.Collections.Generic;
using System;

[Serializable]
public class PlayerSaveData
{
    public string PlayerID;
    public List<Item> Items;
    public WeaponItem Weapon;
}
