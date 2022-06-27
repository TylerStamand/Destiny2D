using System;
using System.Globalization;

[Serializable]
public class WeaponItem : Item {

    private float damage;
    private float coolDown;

    public float Damage {get => damage; private set => damage = value;}
    public float CoolDown {get => coolDown; private set => coolDown = value; }
    public float ProjectileSpeed {get; private set;}

    public WeaponItem(string name, float damage, float coolDown, float projectileSpeed) : base(name) {
        Damage = damage;
        CoolDown = coolDown;
        ProjectileSpeed = projectileSpeed;
    }

    public override string GetDescription() {
        string description = $"Damage: {Damage.ToString("N", CultureInfo.CurrentCulture)} \n";
        description += $"CoolDown: {CoolDown}\n";
        WeaponData weaponData = (WeaponData)ResourceManager.Instance.GetItemData(ItemName);
        if(weaponData.WeaponType == WeaponType.Projectile) {
            description += $"Projectile Speed: {ProjectileSpeed}\n";
        }
        return description;
    }

 
}
