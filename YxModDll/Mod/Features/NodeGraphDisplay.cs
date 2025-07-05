using System.Collections.Generic;
using System.Linq;
using HumanAPI;
using UnityEngine;

namespace YxModDll.Mod.Features;

public class NodeGraphDisplay(NodeGraph graph, bool isInput) : INodeDisplay
{
	public NodeGraph graph = graph;

	public bool isInput = isInput;

	public Node Node => graph;

	public GameObject Parent => graph.transform.parent.GetComponentInParent<NodeGraph>().gameObject;

	public string Title => graph.Title;

	public Vector2 Pos
	{
		get
		{
			return isInput ? graph.inputsPos : graph.outputsPos;
		}
		set
		{
			if (isInput)
			{
				graph.inputsPos = value;
			}
			else
			{
				graph.outputsPos = value;
			}
		}
	}

	public Color NodeColour => graph.nodeColour;

	public List<NodeSocket> ListNodeSockets()
	{
		return (isInput ? graph.inputs.Select((NodeGraphInput x) => x.inputSocket).OfType<NodeSocket>() : graph.outputs.Select((NodeGraphOutput x) => x.outputSocket).OfType<NodeSocket>()).ToList();
	}
}
