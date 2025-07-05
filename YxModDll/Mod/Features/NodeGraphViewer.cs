using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using HumanAPI;
using UnityEngine;

namespace YxModDll.Mod.Features;

public class NodeGraphViewer : MonoBehaviour
{
	public bool view;

	public NodeGraph curGraph;

	public List<INodeDisplay> nodes = new List<INodeDisplay>();

	public List<NodeOutput> outputs = new List<NodeOutput>();

	public Vector2 offset;

	public Rect viewRect;

	public Node detailed;

	public string[] detail;

	public bool draggingWindow;

	public INodeDisplay dragging;

	public NodeOutput selectedOutput;

	public NodeInput selectedInput;

	public string curValue;

	public HashSet<NodeOutput> inputs = new HashSet<NodeOutput>();

	public static readonly Color[] colors = new Color[13]
	{
		Color.red,
		new Color(1f, 0.5f, 0f),
		Color.yellow,
		Color.green,
		new Color(0f, 0.5f, 0f),
		Color.cyan,
		new Color(0f, 0.5f, 0.5f),
		Color.blue,
		new Color(0.625f, 0f, 1f),
		Color.magenta,
		new Color(1f, 0.5f, 1f),
		new Color(1f, 0.75f, 0.75f),
		new Color(0.75f, 0.25f, 0f)
	};

	public void Start()
	{
	}

	public void Enable()
	{
		NodeGraph nodeGraph = FeatureManager.selected?.GetComponentInParent<NodeGraph>();
		if (!(nodeGraph != curGraph))
		{
			return;
		}
		curGraph = nodeGraph;
		if (curGraph != null)
		{
			nodes.Clear();
			CollectionExtensions.Do<NodeGraphDisplay>((IEnumerable<NodeGraphDisplay>)new NodeGraphDisplay[2]
			{
				new NodeGraphDisplay(curGraph, isInput: true),
				new NodeGraphDisplay(curGraph, isInput: false)
			}, (Action<NodeGraphDisplay>)AddNode);
			GetNodes(curGraph.transform);
			outputs = outputs.Intersect(inputs).ToList();
			offset = Vector2.zero;
			float[] ts = nodes.Select((INodeDisplay x) => x.Pos.x).ToArray();
			float[] ts2 = nodes.Select((INodeDisplay x) => x.Pos.y).ToArray();
			viewRect = Rect.MinMaxRect(ts.MinOr(0f) - 50f, ts2.MinOr(0f) - 50f, ts.MaxOr(600f) + 200f, ts2.MaxOr(450f) + 150f);
		}
		else
		{
			view = false;
		}
	}

	public void GetAllNodes(Transform transform)
	{
		if (transform.TryGetComponent<NodeGraph>(out var component))
		{
			AddNode(new NodeDisplay(component));
		}
		else
		{
			GetNodes(transform);
		}
	}

	public void GetNodes(Transform transform)
	{
		Node[] components = transform.GetComponents<Node>();
		foreach (Node node in components)
		{
			if (!(node is NodeGraph))
			{
				AddNode(new NodeDisplay(node));
			}
		}
		CollectionExtensions.Do<Transform>((IEnumerable<Transform>)transform.Children(), (Action<Transform>)GetAllNodes);
	}

	public void AddNode(INodeDisplay node)
	{
		List<NodeSocket> list = node.ListNodeSockets();
		if (list.Count <= 0)
		{
			return;
		}
		nodes.Add(node);
		outputs.AddRange(list.OfType<NodeOutput>());
		CollectionExtensions.Do<NodeOutput>(list.OfType<NodeInput>().SelectMany(delegate(NodeInput x)
		{
			NodeOutput connectedOutput = x.GetConnectedOutput();
			IEnumerable<NodeOutput> result;
			if (connectedOutput == null)
			{
				IEnumerable<NodeOutput> enumerable = Array.Empty<NodeOutput>();
				result = enumerable;
			}
			else
			{
				IEnumerable<NodeOutput> enumerable = new _003C_003Ez__ReadOnlyArray<NodeOutput>(new NodeOutput[1] { connectedOutput });
				result = enumerable;
			}
			return result;
		}), (Action<NodeOutput>)delegate(NodeOutput x)
		{
			inputs.Add(x);
		});
	}

	public void OnGUI()
	{
		if (view && curGraph != null)
		{
			Rect clientRect = new Rect(Screen.width / 2 - 400, Screen.height / 2 - 300, 800f, 600f);
			Rect rect = GUI.Window(536705904, clientRect, WindowFunction, "节点图查看器 (" + curGraph.name + ")");
		}
	}

	public void WindowFunction(int windowId)
	{
		GUIStyle gUIStyle = new GUIStyle
		{
			normal = 
			{
				textColor = GUI.skin.label.normal.textColor
			}
		};
		GUIStyle gUIStyle2 = new GUIStyle();
		GUIStyle gUIStyle3 = new GUIStyle();
		Dictionary<NodeSocket, Vector2> dictionary = new Dictionary<NodeSocket, Vector2>();
		string text = ((selectedOutput?.node != null) ? (selectedOutput.node.name + "." + selectedOutput.node.GetType().Name + "." + selectedOutput.name) : "null");
		GUI.Label(new Rect(10f, 20f, 400f, 20f), "输出: " + text);
		string text2 = ((selectedInput?.node != null) ? (selectedInput.node.name + "." + selectedInput.node.GetType().Name + "." + selectedInput.name) : "null");
		GUI.Label(new Rect(10f, 40f, 400f, 20f), "输入: " + text2);
		if (GUI.Button(new Rect(610f, 30f, 80f, 20f), "清除选择"))
		{
			selectedOutput = null;
		}
		if (GUI.Button(new Rect(690f, 30f, 80f, 20f), "设为选择"))
		{
			selectedInput.Connect(selectedOutput);
		}
		curValue = GUI.TextField(new Rect(610f, 50f, 80f, 20f), curValue);
		if (GUI.Button(new Rect(690f, 50f, 80f, 20f), "设为数值") && float.TryParse(curValue, out var result))
		{
			selectedInput.Connect(null);
			selectedInput.value = result;
		}
		Vector2 vector = GUI.BeginScrollView(new Rect(10f, 60f, 780f, 530f), offset, viewRect);
		offset = new Vector2((int)vector.x, (int)vector.y);
		Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
		foreach (INodeDisplay node in nodes)
		{
			if (node.Node == null)
			{
				continue;
			}
			Vector2 size = rect.size;
			List<NodeSocket> list = node.ListNodeSockets();
			GUI.BeginGroup(new Rect(node.Pos.x - offset.x, node.Pos.y - offset.y, 150f, 50 + list.Count * 20));
			GUI.Box(new Rect(0f, 0f, 150f, 50 + list.Count * 20), "");
			gUIStyle3.normal.textColor = node.NodeColour;
			if (node.Node is SignalUnityEvent signalUnityEvent)
			{
				GUI.Label(new Rect(5f, 7f, 100f, 20f), node.Title, gUIStyle3);
				if (GUI.Button(new Rect(105f, 5f, 20f, 20f), "?"))
				{
					FeatureManager.ComponentInfo(node.Node);
				}
				if (GUI.Button(new Rect(125f, 5f, 20f, 20f), "!"))
				{
					signalUnityEvent.triggerEvent?.Invoke();
				}
			}
			else
			{
				GUI.Label(new Rect(5f, 7f, 120f, 20f), node.Title, gUIStyle3);
				if (GUI.Button(new Rect(125f, 5f, 20f, 20f), "?"))
				{
					FeatureManager.ComponentInfo(node.Node);
				}
			}
			if (GUI.Button(new Rect(5f, 25f, 140f, 20f), node.Node.name))
			{
				FeatureManager.selected = node.Parent.gameObject;
				Enable();
			}
			int num = 50;
			foreach (NodeInput item in list.OfType<NodeInput>())
			{
				int num2 = outputs.IndexOf(item.GetConnectedOutput());
				string text3;
				GUIStyle style;
				if (num2 != -1)
				{
					text3 = ">" + item.name;
					if (item == selectedInput)
					{
						text3 += $": {item.value}";
					}
					style = gUIStyle2;
					GUIStyleState normal = gUIStyle2.normal;
					GUIStyleState hover = gUIStyle2.hover;
					Color color = (gUIStyle2.active.textColor = colors[num2 % colors.Length]);
					Color textColor = (hover.textColor = color);
					normal.textColor = textColor;
				}
				else
				{
					text3 = $"{item.name}: {item.value}";
					style = gUIStyle;
				}
				if (GUI.Button(new Rect(10f, num, 130f, 20f), text3, style))
				{
					selectedInput = item;
				}
				num += 20;
			}
			foreach (NodeOutput item2 in list.OfType<NodeOutput>())
			{
				int num3 = outputs.IndexOf(item2);
				if (num3 != -1)
				{
					gUIStyle2.normal.textColor = colors[num3 % colors.Length];
				}
				string text4 = item2.name ?? "";
				if (item2 == selectedOutput)
				{
					text4 += $": {item2.value}";
				}
				if (GUI.Button(new Rect(10f, num, 130f, 20f), text4 + ">", (num3 != -1) ? gUIStyle2 : gUIStyle))
				{
					selectedOutput = item2;
				}
				num += 20;
			}
			if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
			{
				dragging = node;
				Event.current.Use();
			}
			GUI.EndGroup();
		}
		GUI.EndScrollView();
		if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
		{
			draggingWindow = true;
			Event.current.Use();
		}
		if (Event.current.type == EventType.MouseDrag)
		{
			if (draggingWindow)
			{
				offset -= Event.current.delta;
			}
			else if (dragging != null)
			{
				dragging.Pos += Event.current.delta;
			}
		}
		if (Event.current.type == EventType.MouseUp)
		{
			draggingWindow = false;
			dragging = null;
			Event.current.Use();
		}
	}
}
