using BehaviourTreeEditor.RunTime;
using UnityEngine;

namespace BehaviourTreeEditor.SharedVariables
{
    public class SharedRect : SharedVariable<Rect>
    {
        public static implicit operator SharedRect(Rect value)
        {
            SharedRect sharedRect = CreateInstance<SharedRect>();
            sharedRect.sharedValue = value;
            return sharedRect;
        }
    }
}