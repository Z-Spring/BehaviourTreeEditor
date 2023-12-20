﻿using BehaviourTreeEditor.RunTime;
using UnityEngine;

namespace BehaviourTreeEditor.SharedVariables
{
    public class SharedMaterial : SharedVariable<Material>
    {
        public static implicit operator SharedMaterial(Material value)
        {
            var sharedVariable = CreateInstance<SharedMaterial>();
            sharedVariable.SetValue(value);
            return sharedVariable;
        }
    }
}