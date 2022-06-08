using Unity.Netcode;

[System.Serializable]
public struct WeaponStats : INetworkSerializable {
    public string WeaponName;
    public float Damage;
    public float CoolDown;
    public float ProjectileSpeed;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref WeaponName);
        serializer.SerializeValue(ref Damage);
        serializer.SerializeValue(ref CoolDown);
        serializer.SerializeValue(ref ProjectileSpeed);
    }
}
