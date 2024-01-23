using BehaviourTreeEditor.RunTime;

namespace BehaviourTreeEditor.SharedVariables
{
    public class SharedInt : SharedVariable<int>
    {
        public static implicit operator SharedInt(int value)
        {
            SharedInt sharedInt = CreateInstance<SharedInt>();
            sharedInt.sharedValue = value;
            return sharedInt;
        }
    }
}