using BehaviourTreeEditor.RunTime;
using UnityEngine;

namespace BehaviourTreeEditor.SharedVariables
{
    public class SharedTexture2D : SharedVariable<Texture2D>
    {
        public static implicit operator SharedTexture2D(Texture2D value)
        {
            SharedTexture2D sharedTexture2D = CreateInstance<SharedTexture2D>();
            sharedTexture2D.sharedValue = value;
            return sharedTexture2D;
        }
    }
}