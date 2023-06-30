using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public interface IModifierProvider
    {
        IEnumerable<float> GetAdditiveModifiers (Stats stat);
        IEnumerable<float> GetPercentageModifiers (Stats stat);
    }
}
