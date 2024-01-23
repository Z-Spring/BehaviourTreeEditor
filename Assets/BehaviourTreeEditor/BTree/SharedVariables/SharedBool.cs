
namespace BehaviourTreeEditor.BTree.SharedVariables
{
    public class SharedBool : SharedVariable<bool>
    {
        public static implicit operator SharedBool(bool value)
        {
            SharedBool sharedBool = CreateInstance<SharedBool>();
            sharedBool.sharedValue = value;
            return sharedBool;
        }
    }
}