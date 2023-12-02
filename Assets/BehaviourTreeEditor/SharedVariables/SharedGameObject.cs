using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTreeEditor.SharedVariables
{
    [CreateAssetMenu(fileName = "SharedGameObject", menuName = "Behavior Tree/Shared Variables/GameObject", order = 0)]
    public class SharedGameObject : SharedVariable
    {
        [FormerlySerializedAs("SharedValue")] public GameObject sharedValue;
    }
}