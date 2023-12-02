using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor
{
    public class MyEdgeConnectorListener : IEdgeConnectorListener

    {
        private BtreeView btreeView;
        public NodePort MyNodePort { get; set; }
        
        public MyEdgeConnectorListener(BtreeView btreeView)
        {
            this.btreeView = btreeView;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            MyNodePort = edge.output as NodePort;
            Vector2 screenPosition = GUIUtility.GUIToScreenPoint(position);
            btreeView.OpenSearchWindow(screenPosition);
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
        }
    }
}