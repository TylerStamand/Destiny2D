using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;


public struct ItemInfo : IEquatable<ItemInfo> , INetworkSerializable {
    public FixedString64Bytes ItemID;
    public FixedString32Bytes Name;
    public FixedString32Bytes Description;




    public bool Equals(ItemInfo other) {
        if(other.ItemID.Value.ToString() == ItemID.Value.ToString()) 
            return true;
        return false;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        
        serializer.SerializeValue(ref ItemID);
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref Description);

    }
}
