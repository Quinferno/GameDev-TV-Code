using System.Collections.Generic;

namespace RPG.Attributes
{
    [System.Serializable] public struct ElementalDamage
    {
        public DamageType damageType;
        public float amount;
    }

    public enum DamageType
    {
        Fire,
        Lightning,
        Acid,
        Divine,
        Myth
    }

    public interface IElementalResistanceProvider
    {
        public IEnumerable<float> GetElementalResistance(DamageType damageType);
    }

    public interface IElementalDamageProvider
    {
        public IEnumerable<float> GetElementalDamageBoost(DamageType damageType);
    }
}
