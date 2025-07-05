using System.Collections.Generic;
using HumanAPI;
using UnityEngine;

namespace YxModDll.Mod.Features
{
    public class NodeDisplay(Node node) : INodeDisplay
    {
    	public Node node = node;

    	public Node Node => node;

    	public GameObject Parent => node.gameObject;

    	public string Title => node.Title;

    	public Vector2 Pos
    	{
    		get
    		{
    			return node.pos;
    		}
    		set
    		{
    			node.pos = value;
    		}
    	}

    	public Color NodeColour => (node is NodeGraph && node.nodeColour == Color.white) ? Color.yellow : node.nodeColour;

    	public List<NodeSocket> ListNodeSockets()
    	{
    		return node.ListNodeSockets();
    	}
    }
}
