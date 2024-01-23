using BehaviourTreeEditor.RunTime;
using UnityEngine;

namespace BehaviourTreeEditor.SharedVariables
{
    public class SharedSprite : SharedVariable<Sprite>
    {
        public static implicit operator SharedSprite(Sprite value)
        {
            SharedSprite sharedSprite = CreateInstance<SharedSprite>();
            sharedSprite.sharedValue = value;
            return sharedSprite;
        }
    }
}