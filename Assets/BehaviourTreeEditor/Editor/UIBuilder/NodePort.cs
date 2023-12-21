using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class DefaultEdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange graphViewChange;
        private readonly List<Edge> edgesToCreate;
        private readonly List<GraphElement> edgesToDelete;

        public DefaultEdgeConnectorListener()
        {
            edgesToCreate = new List<Edge>();
            edgesToDelete = new List<GraphElement>();

            graphViewChange.edgesToCreate = edgesToCreate;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            this.edgesToCreate.Clear();
            this.edgesToCreate.Add(edge);
            edgesToDelete.Clear();
          
            CheckIfSingleCapacity( edge.input, edge );
            CheckIfSingleCapacity( edge.output, edge );
            if (edgesToDelete.Count > 0)
                graphView.DeleteElements(edgesToDelete);

            var edgesToCreate = this.edgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(graphViewChange).edgesToCreate;
            }

            foreach (Edge e in edgesToCreate)
            {
                graphView.AddElement(e);
                edge.input.Connect(e);
                edge.output.Connect(e);
            }
        }
        
        void CheckIfSingleCapacity(Port port, Edge edge)
        {
            if (port.capacity != Port.Capacity.Single)
            {
                return;
            }
            foreach (Edge edgeToDelete in port.connections)
            {
                if (edgeToDelete != edge)
                {
                    edgesToDelete.Add(edgeToDelete);
                }
            }
        }
    }


    public class NodePort : Port
    {
        public NodePort(Direction direction, Capacity capacity) : base(Orientation.Vertical, direction, capacity,
            typeof(bool))
        {
            this.AddManipulator(new EdgeConnector<Edge>(BtreeView.listener));
            var connectorListener = new DefaultEdgeConnectorListener();
            m_EdgeConnector = new EdgeConnector<Edge>(connectorListener);
            this.AddManipulator(m_EdgeConnector);
            style.width = 50;
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            Rect rect = new Rect(0, 0, layout.width - 5f, layout.height - 2f);
            return rect.Contains(localPoint);
        }
    }
}