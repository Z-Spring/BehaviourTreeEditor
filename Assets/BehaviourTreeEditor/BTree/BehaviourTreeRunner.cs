using MurphyEditor.BTree.RunTime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BehaviourTreeEditor.BTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public BehaviourTree tree;
        public string treeName;
        public string treeDescription;
        public Node.State treeState;
        
        CurrentGameContext context; 
        float waitTime;
        WaitForSeconds wait;

        private void Start()
        {
            Debug.Log("BehaviourTreeRunner Start");
            tree = tree.Clone();
            context = CreateBehaviourTreeContext();
            tree.Bind(context);
            waitTime = Random.Range(0.1f, 0.7f);
            wait = new WaitForSeconds(waitTime);
            
            // SharedGameObject sharedGameObject = new SharedGameObject();
            // InitializeSharedVariable("Cop", "Target", sharedGameObject);
            // StartCoroutine(StartRunning());
        }
        
        


        CurrentGameContext CreateBehaviourTreeContext()
        {
            return CurrentGameContext.CreateFromBehaviourTreeGameObject(gameObject);
        }

        private void Update()
        {
            //
            treeState =  tree.Update();
        }
        
        // IEnumerator StartRunning()
        // {
        //     while (true)
        //     {
        //         tree.Update();
        //         yield return wait;
        //     }
        // }
    }
}