using System.Collections.Generic;
using UnityEngine;

namespace VioletSolver.Debug
{
    internal struct NodeVisualOption
    {
        internal PrimitiveType ShapeType { get; }
        internal float NodeSize { get; }

        internal NodeVisualOption(PrimitiveType shapeType, float nodeSize)
        {
            ShapeType = shapeType;
            NodeSize = nodeSize;
        }
    }

    internal class GraphVisualizer
    {
        Graph _graph;

        LineRenderer _line;
        GameObject _nodes;

        Dictionary<Node[], GameObject> _linkMap;

        internal float LineWidth { get; set; } = 0.02f;
        internal NodeVisualOption VisualOption { get; set; }

        internal GraphVisualizer(Graph linkGraph, string objectName = "Graph Visual")
        {
            _nodes = new GameObject(objectName);
            SetupGraph(_nodes);
            VisualOption = new(PrimitiveType.Sphere, 0.02f);
            _linkMap = new();
        }
        internal GraphVisualizer(Graph linkGraph, NodeVisualOption option, string objectName = "Graph Visual")
        {
            _nodes = new GameObject(objectName);
            SetupGraph(_nodes);
            VisualOption = option;
            _linkMap = new();
        }

        GameObject GenerateNodeObject(NodeVisualOption option)
        {
            var nodeObject = GameObject.CreatePrimitive(option.ShapeType);
            nodeObject.transform.localScale = Vector3.one * option.NodeSize;
            return nodeObject;
        }

        internal void SetupGraph(GameObject rootObject)
        {
            for (var i = 0; i < _graph.Count; i++)
            {
                var linkRootObject = new GameObject();
                _linkMap.Add(_graph[i], linkRootObject);
                linkRootObject.transform.parent = rootObject.transform;

                // Setup LineRenderer
                var line = linkRootObject.AddComponent<LineRenderer>();
                line.startWidth = LineWidth;
                line.endWidth = LineWidth;

                foreach (var _ in _graph[i]) 
                {
                    GameObject node = GenerateNodeObject(VisualOption);
                    node.transform.parent = linkRootObject.transform;
                }
            }
        }

        internal void UpdateGraphVisual(GameObject visualizerObject, ref Vector3[] referencePositions)
        {
            for (var i = 0; i < _graph.Count; i++)
            {
                UpdateLinkVisual(_graph[i], ref referencePositions);
            }
        }
        internal void UpdateGraphVisual(GameObject visualizerObject, Vector3 posOffset, ref Vector3[] referencePositions)
        {
            for (var i = 0; i < _graph.Count; i++)
            {
                UpdateLinkVisual(_graph[i], posOffset, ref referencePositions);
            }
        }

        void UpdateLinkVisual(Node[] link, ref Vector3[] referencePositions)
        {
            var linkRootObj = _linkMap[link];
            var line = linkRootObj.GetComponent<LineRenderer>();

            // Update each of node position
            foreach (var node in link)
            {
                var i = node.Index;
                var pos = referencePositions[i];
                node.Position = pos;
            }

            // Update LineRenderer
            var posArray = LinkUtility.ExtractPositions(link);
            line.SetPositions(posArray);
        }

        void UpdateLinkVisual(Node[] link, Vector3 posOffset, ref Vector3[] referencePositions)
        {
            var linkRootObj = _linkMap[link];
            var line = linkRootObj.GetComponent<LineRenderer>();

            // Update each of node position
            foreach (var node in link)
            {
                var i = node.Index;
                var pos = referencePositions[i] + posOffset;
                node.Position = pos;
            }

            // Update LineRenderer
            var posArray = LinkUtility.ExtractPositions(link);
            line.SetPositions(posArray);
        }
    }

    internal class Node
    {
        internal int Index { get; }
        internal Vector3 Position { get; set; }

        internal Node(int index)
        {
            Index = index;
            Position = Vector3.zero;
        }
    }

    internal static class LinkUtility
    {
        internal static Vector3[] ExtractPositions(Node[] link)
        {
            var pos = new Vector3[link.Length];
            for(var i =0;i < link.Length; i++)
            {
                pos[i] = link[i].Position;
            }
            return pos;
        }
    }

    internal class Graph
    {
        List<Node[]> _graph;
        internal Node[] this[int i] => _graph[i];
        internal int Count { get; private set; }

        internal Graph(params Node[][] link)
        {
            _graph = new();
            foreach(var l in link) 
                _graph.Add(l);

            Count = _graph.Count;
        }
    }
}
