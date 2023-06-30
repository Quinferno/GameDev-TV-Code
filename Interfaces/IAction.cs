using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public interface IAction
    {
        void Cancel();//note that everything in interfaces is public
    }
}
