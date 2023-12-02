using UnityEngine;

namespace BehaviourTreeEditor.SharedVariables
{
    [CreateAssetMenu(fileName = "SharedString", menuName = "Behavior Tree/Shared Variables/string", order = 0)]
    public class SharedString : SharedVariable
    {
        public string sharedValue;
    }
}