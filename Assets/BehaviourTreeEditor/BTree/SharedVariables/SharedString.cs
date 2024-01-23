﻿namespace BehaviourTreeEditor.BTree.SharedVariables
{
    public class SharedString : SharedVariable<string>
    {
        public static implicit operator SharedString(string value)
        {
            SharedString sharedString = CreateInstance<SharedString>();
            sharedString.sharedValue = value;
            return sharedString;
        }
    }
}