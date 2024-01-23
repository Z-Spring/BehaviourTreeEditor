using BehaviourTreeEditor.SharedVariables;

namespace BehaviourTreeEditor.RunTime
{
    public abstract class SharedVariable<T> : SharedVariable
    {
        public T sharedValue;

        public override void SetValue(object value) => sharedValue = (T)value;

        public override object GetValue() => sharedValue;
    }
}