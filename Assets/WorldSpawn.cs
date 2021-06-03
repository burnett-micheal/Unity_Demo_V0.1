using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawn : MonoBehaviour {

	public int size;
	public int wallChance;
	public float spacing;
	public int spawnerCount;
	public GameObject node;

	void Start () {
		Nodes _nodesClass = new Nodes ();
		ActiveNodeData activeNodeData = new ActiveNodeData ();

		ActiveNodes activeNodesScr = gameObject.GetComponent<ActiveNodes> ();
		activeNodesScr.nodesClass = _nodesClass;
		activeNodesScr.activeNodeData = activeNodeData;

		Dictionary<Vector2, Node> nodes = new Dictionary<Vector2, Node>();
		List<Node> spaceNodes = new List<Node> ();

		//Set InActive
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				Vector2 loc = new Vector2 (x, y);
				GameObject n = Instantiate (node, loc * spacing, Quaternion.identity);
				string type = rand(0, 100) > wallChance ? "space" : "wall";
				Node nodeClass = _nodesClass.newNode (n, loc, type, null);
				nodes.Add (loc, nodeClass);
				if (type == "space") {spaceNodes.Add (nodeClass);}
			}
		}


		//Set Active
		int index = 0;
		for(int i = 0; i < spawnerCount; i++){
			index = rand (0, spaceNodes.Count - 1);
			Node item = spaceNodes [index];
			activeNodeData.spawners.Create(item);
			spaceNodes.RemoveAt (index);
		}
		index = rand(0, spaceNodes.Count - 1);
		Node playerNode = spaceNodes [index];
		playerNode.type = "player";
		activeNodesScr.player = playerNode;
		spaceNodes.Clear();


		//Set Neighbors
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				Vector2 loc = new Vector2 (x, y);
				Node _node = nodes [loc];
				Vector2 left = new Vector2 (x-1, y);
				Vector2 right = new Vector2 (x+1, y);
				Vector2 up = new Vector2 (x, y+1);
				Vector2 down = new Vector2 (x, y-1);
				List<Node> neighbors = new List<Node> ();
				if(nodes.ContainsKey(left)){neighbors.Add (nodes [left]);}
				if(nodes.ContainsKey(right)){neighbors.Add (nodes [right]);}
				if(nodes.ContainsKey(up)){neighbors.Add (nodes [up]);}
				if(nodes.ContainsKey(down)){neighbors.Add (nodes [down]);}
				_node.neighbors = neighbors;
			}
		}
			
		Destroy(gameObject.GetComponent<WorldSpawn> ());
	}

	private int rand(int min, int max){
		return Random.Range (min, max);
	}
}
