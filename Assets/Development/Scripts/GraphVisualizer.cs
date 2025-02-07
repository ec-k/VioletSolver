using System.Collections.Generic;
using UnityEngine;

namespace VioletSolver.Development
{
    internal struct NodeVisualOption
    {
        internal PrimitiveType NodeShape { get; }
        internal float NodeSize { get; }
        internal NodeVisualOption(float nodeSize)
        {
            NodeShape = PrimitiveType.Sphere;
            NodeSize = nodeSize;
        }
    }

    internal class GraphVisualizer
    {
        Graph _graph;
        internal GameObject RootNode { get; private set; }
        Dictionary<Node[], GameObject> _linkMap;
        float _lineWidth = 0.005f;
        NodeVisualOption _visualOption;

        internal GraphVisualizer(in Graph linkGraph, NodeVisualOption option, in Material lineMaterial, string objectName = "Graph Visual")
        {
            _linkMap = new();
            _graph = linkGraph;
            RootNode = new GameObject(objectName);
            SetupGraphVisual(RootNode, lineMaterial);
            _visualOption = option;
        }

        GameObject GenerateNodeObject(NodeVisualOption option)
        {
            var nodeObject = GameObject.CreatePrimitive(option.NodeShape);
            nodeObject.transform.localScale = Vector3.one * option.NodeSize;
            return nodeObject;
        }

        internal void SetupGraphVisual(in GameObject rootObject, in Material lineMaterial)
        {
            for (var i = 0; i < _graph.Count; i++)
            {
                var linkRootObject = new GameObject($"Link {i.ToString()}");
                _linkMap.Add(_graph[i], linkRootObject);
                linkRootObject.transform.parent = rootObject.transform;

                // Setup LineRenderer
                var line = linkRootObject.AddComponent<LineRenderer>();
                line.positionCount = _graph[i].Length;
                line.startWidth = _lineWidth;
                line.endWidth = _lineWidth;
                line.material = lineMaterial;

                foreach (var _ in _graph[i]) 
                {
                    GameObject node = GenerateNodeObject(_visualOption);
                    node.transform.parent = linkRootObject.transform;
                }
            }
        }

        internal void UpdateGraphVisual(in Vector3[] referencePositions)
        {
            for (var i = 0; i < _graph.Count; i++)
            {
                UpdateLinkVisual(_graph[i], in referencePositions);
            }
        }
        internal void UpdateGraphVisual(in Vector3[] referencePositions, Vector3 posOffset)
        {
            for (var i = 0; i < _graph.Count; i++)
            {
                UpdateLinkVisual(_graph[i], referencePositions, posOffset);
            }
        }

        void UpdateLinkVisual(Node[] link, in Vector3[] referencePositions)
        {
            var linkRootObj = _linkMap[link];
            var line = linkRootObj.GetComponent<LineRenderer>();

            // Update each of node position
            foreach (var node in link)
            {
                var i = node.Index;
                if (referencePositions.Length > i)
                {
                    var pos = referencePositions[i];
                    node.Position = pos;
                }
            }

            // Update LineRenderer
            var posArray = LinkUtility.ExtractPositions(link);
            line.SetPositions(posArray);
        }

        void UpdateLinkVisual(Node[] link, in Vector3[] referencePositions, Vector3 posOffset)
        {
            var linkRootObj = _linkMap[link];
            var line = linkRootObj.GetComponent<LineRenderer>();

            // Update each of node position
            foreach (var node in link)
            {
                var i = node.Index;
                if (referencePositions.Length > i)
                {
                    var pos = referencePositions[i] + posOffset;
                    node.Position = pos;
                }
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
