using System;
using System.Collections.Generic;
using MurphyEditor.BTree.RunTime;
using UnityEngine;

namespace BehaviourTreeEditor.BTree
{
    public class Behaviour : MonoBehaviour
    {
        public BehaviourTree tree;
        Dictionary<Type, Dictionary<string, Delegate>> eventDic = new();
        CurrentGameContext context;

        protected void Start()
        {
            context = CreateBehaviourTreeContext();
            tree = tree.Clone();
            tree.Bind(context);
        }

        CurrentGameContext CreateBehaviourTreeContext()
        {
            return CurrentGameContext.CreateFromBehaviourTreeGameObject(gameObject);
        }

        void RegisterEvent(string eventName, Delegate handler)
        {
            Type type = handler.GetType();
            eventDic ??= new Dictionary<Type, Dictionary<string, Delegate>>();
            if (!eventDic.TryGetValue(type, out var handlerDic))
            {
                handlerDic = new Dictionary<string, Delegate>();
                eventDic.Add(type, handlerDic);
            }

            if (handlerDic.TryGetValue(eventName, out var handlerList))
            {
                handlerDic[eventName] = Delegate.Combine(handlerList, handler);
            }
            else
            {
                handlerDic.Add(eventName, handler);
            }
        }

        void UnRegisterEvent(string eventName, Delegate handler)
        {
            Type type = handler.GetType();
            if (eventDic != null && eventDic.TryGetValue(type, out var handlerDic) &&
                handlerDic.TryGetValue(eventName, out var handlerList))
            {
                handlerDic[eventName] = Delegate.Remove(handlerList, handler);
            }
        }

        Delegate GetDelegate(string eventName, Type type)
        {
            return eventDic != null && eventDic.TryGetValue(type, out var handlerDic) &&
                   handlerDic.TryGetValue(eventName, out var handlerList)
                ? handlerList
                : null;
        }

        public void RegisterEvent(string eventName, System.Action handler) =>
            RegisterEvent(eventName, (Delegate)handler);

        public void RegisterEvent<T>(string eventName, Action<T> handler) =>
            RegisterEvent(eventName, (Delegate)handler);

        public void RegisterEvent<T1, T2>(string eventName, Action<T1, T2> handler) =>
            RegisterEvent(eventName, (Delegate)handler);

        public void RegisterEvent<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler) =>
            RegisterEvent(eventName, (Delegate)handler);

        public void UnRegisterEvent(string eventName, System.Action handler) =>
            UnRegisterEvent(eventName, (Delegate)handler);

        public void UnRegisterEvent<T>(string eventName, Action<T> handler) =>
            UnRegisterEvent(eventName, (Delegate)handler);

        public void UnRegisterEvent<T1, T2>(string eventName, Action<T1, T2> handler) =>
            UnRegisterEvent(eventName, (Delegate)handler);

        public void UnRegisterEvent<T1, T2, T3>(string eventName, Action<T1, T2, T3> handler) =>
            UnRegisterEvent(eventName, (Delegate)handler);

        public void SendEvent(string eventName)
        {
            if (GetDelegate(eventName, typeof(Action)) is System.Action action)
            {
                action();
            }
        }

        public void SendEvent<T>(string eventName, T arg)
        {
            if (GetDelegate(eventName, typeof(Action<T>)) is Action<T> action)
            {
                action(arg);
            }
        }


        public void SendEvent<T1, T2>(string eventName, T1 arg1, T2 arg2)
        {
            if (GetDelegate(eventName, typeof(Action<T1, T2>)) is Action<T1, T2> action)
            {
                action(arg1, arg2);
            }
        }

        public void SendEvent<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
        {
            if (GetDelegate(eventName, typeof(Action<T1, T2, T3>)) is Action<T1, T2, T3> action)
            {
                action(arg1, arg2, arg3);
            }
        }
    }
}