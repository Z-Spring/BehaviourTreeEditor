using UnityEngine;

namespace BehaviourTreeEditor.BTree.SharedVariables
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