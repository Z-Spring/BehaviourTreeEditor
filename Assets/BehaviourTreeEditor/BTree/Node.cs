using MurphyEditor.BTree.RunTime;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTreeEditor.BTree
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Success,
            Failure
        }

        public string nodeName;
        public State state = State.Running;
        
        [HideInInspector] public bool started;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public CurrentGameContext currentGameContext;
        [HideInInspector] public Node parent;

        public State Update()
        {
            if (!started)
            {
                OnEnter();
                started = true;
            }

            state = OnUpdate();
            if (state is State.Failure or State.Success)
            {
                OnExit();
                started = false;
            }

            return state;
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        protected abstract void OnEnter();
        protected abstract void OnExit();
        protected abstract State OnUpdate();
    }
}