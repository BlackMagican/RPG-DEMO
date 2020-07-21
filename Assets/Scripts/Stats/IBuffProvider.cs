using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    public interface IBuffProvider
    {
        IEnumerable<float> GetAdditiveBuff(Stat stat);
        IEnumerable<float> GetPercentageBuff(Stat stat);
    }
}
