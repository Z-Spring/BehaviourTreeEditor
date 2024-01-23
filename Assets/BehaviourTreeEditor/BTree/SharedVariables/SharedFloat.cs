namespace BehaviourTreeEditor.BTree.SharedVariables
{
    public class SharedFloat : SharedVariable<float>
    {
        public static implicit operator SharedFloat(float value)
        {
            SharedFloat sharedFloat = CreateInstance<SharedFloat>();
            sharedFloat.sharedValue = value;
            return sharedFloat;
        }
    }
}