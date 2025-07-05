using System.Collections.Generic;
using HumanAPI;
using UnityEngine;

namespace YxModDll.Mod.Features
{
    public interface INodeDisplay
    {
    	Node Node { get; }

    	GameObject Parent { get; }

    	string Title { get; }

    	Vector2 Pos { get; set; }

    	Color NodeColour { get; }

    	List<NodeSocket> ListNodeSockets();
    }
}
