using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;


public struct ItemInfo : IEquatable<ItemInfo> , INetworkSerializable {
    public ForceNetworkSerializeByMemcpy<FixedString64Bytes> ItemID;
    public ForceNetworkSerializeByMemcpy<FixedString64Bytes> Name;
    public ForceNetworkSerializeByMemcpy<FixedString512Bytes> Description;

    public bool Equals(ItemInfo other) {
        if(other.ItemID.Value == ItemID.Value) 
            return true;
        return false;
    }

    void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer) {
        
        serializer.SerializeValue(ref ItemID);
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref Description);
    }
}
