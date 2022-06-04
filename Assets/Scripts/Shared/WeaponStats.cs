using Unity.Netcode;

public struct WeaponStats : INetworkSerializable
{
    public float Damage;
    public float CoolDown;
    public float ProjectileSpeed;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref Damage);
        serializer.SerializeValue(ref CoolDown);
        serializer.SerializeValue(ref ProjectileSpeed);
    }
}
