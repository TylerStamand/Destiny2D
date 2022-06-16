using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;


public struct ItemInfo : IEquatable<ItemInfo> {
    public FixedString64Bytes ItemID {get; set;}
    public FixedString64Bytes Name {get; set;}
    public FixedString64Bytes Description {get; set;}

    public bool Equals(ItemInfo other) {
        if(other.ItemID == ItemID) 
            return true;
        return false;
    }
}
