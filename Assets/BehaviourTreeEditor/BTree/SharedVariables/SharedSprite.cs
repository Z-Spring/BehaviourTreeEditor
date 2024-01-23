using UnityEngine;

namespace BehaviourTreeEditor.BTree.SharedVariables
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