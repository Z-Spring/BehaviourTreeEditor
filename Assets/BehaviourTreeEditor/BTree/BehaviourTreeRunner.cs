using UnityEngine;

namespace BehaviourTreeEditor.BTree
{
    public class BehaviourTreeRunner : Behaviour
    {
        public string treeName;
        public string treeDescription;
        
        [SerializeField] Node.State treeState;
        

        new void Start()
        {
            base.Start();
        }


        private void Update()
        {
            treeState = tree.Update();
        }
        
    }
}