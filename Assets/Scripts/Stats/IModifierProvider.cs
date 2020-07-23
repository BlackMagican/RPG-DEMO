using System.Collections;
using System.Collections.Generic;

namespace Stats
{
    /// <summary>
    /// Can get information from weapon.
    /// </summary>
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifier(Stat stat);
        IEnumerable<float> GetPercentageModifier(Stat stat);
    }
}
