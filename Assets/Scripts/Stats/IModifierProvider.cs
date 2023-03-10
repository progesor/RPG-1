using System.Collections;

namespace RPG.Stats
{
    public interface IModifierProvider
    {
        IEnumerable GetAdditeveModifiers(Stat stat);
        IEnumerable GetPercentageModifiers(Stat stat);
    }
}