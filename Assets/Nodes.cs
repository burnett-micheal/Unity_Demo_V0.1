using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTypes{
	public List<string> types = new List<string> ();
	public List<string> activeTypes = new List<string> ();
	public NodeTypes(){
		types.Add ("player");
		types.Add ("enemy");
		types.Add ("spawner");
		types.Add ("bullet");
		types.Add ("space");
		types.Add ("wall");

		activeTypes.Add ("player");
		activeTypes.Add ("enemy");
		activeTypes.Add ("spawner");
		activeTypes.Add ("bullet");
	}
};

public class Nodes {
	
	public Node newNode(GameObject self, Vector2 location, string type, List<Node> neighbors){
		Node n = new Node (self, location, type, neighbors);
		return n;
	}

	public Node getNeighbor(Node node, string direction){
		Vector2 nodeLoc = node.location;
		Vector2 left = new Vector2 (nodeLoc.x - 1, nodeLoc.y);
		Vector2 right = new Vector2 (nodeLoc.x + 1, nodeLoc.y);
		Vector2 up = new Vector2 (nodeLoc.x, nodeLoc.y + 1);
		Vector2 down = new Vector2 (nodeLoc.x, nodeLoc.y - 1);

		List<Node> neighbors = node.neighbors;
		Node result = null;
		for (int i = 0; i < neighbors.Count; i++) {
			Node neighbor = neighbors [i];
			Vector2 nLoc = neighbor.location;
			if(nLoc == left && direction == "left"){result = neighbor;}
			if(nLoc == right && direction == "right"){result = neighbor;}
			if(nLoc == up && direction == "up"){result = neighbor;}
			if(nLoc == down && direction == "down"){result = neighbor;}

			if (result != null) {break;}
		}

		return result;
	}
};

public class Node {

	private GameObject SelfObj;
	private List<Node> Neighbors;
	private Vector2 Location;

	private string Type;
	private bool Vulnerable;
	private bool Traversable;
	private Color Color;

	public Node(GameObject _selfObj, Vector2 _location, string _type, List<Node> _neighbors){
		selfObj = _selfObj;
		location = _location;
		type = _type;
		neighbors = _neighbors;
	}

	public GameObject selfObj {
		get { return SelfObj; }
		set { 
			if (SelfObj == value) {return;}
			onSelfObjChange (value);
			SelfObj = value;
		}
	}

	public Vector2 location {
		get { return Location; }
		set { 
			if (Location == value) {return;}
			onLocationChange (value);
			Location = value;
		}
	}

	public string type {
		get{ return Type; }
		set{ 
			if (Type == value) { return; }
			onTypeChange (value);
			Type = value;
		}
	}

	public List<Node> neighbors {
		get { return Neighbors; }
		set { 
			if (Neighbors == value) {return;}
			onNeighborsChange (value);
			Neighbors = value;
		}
	}

	public bool vulnerable{
		get{ return Vulnerable; }	
	}

	public bool traversable{
		get{ return Traversable; }	
	}

	public Color color{
		get{ return Color; }	
	}

	private void universalProps(bool vulnerable, bool traversable, Color color){
		Vulnerable = vulnerable;
		Traversable = traversable;
		Color = color;
	}

	private void onTypeChange(string newType){
		NodeTypes nodeTypesClass = new NodeTypes ();
		List<string> types = nodeTypesClass.types;
		if (!types.Contains (newType)) {throw new UnityException ("Invalid Node Type");}

		switch (newType) {
		case "spawner":
			universalProps (true, false, Color.green);
			break;
		case "enemy":
			universalProps (true, false, Color.red);
			break;
		case "wall":
			universalProps (false, false, Color.black);
			break;
		case "bullet":
			universalProps (true, false, Color.yellow);
			break;
		case "space":
			universalProps (false, true, Color.white);
			break;
		case "player":
			universalProps (true, false, Color.blue);
			break;
		}

		SelfObj.GetComponent<SpriteRenderer> ().material.SetColor ("_Color", color);
	}

	private void onSelfObjChange(GameObject newSelfObj){
	}

	private void onLocationChange(Vector2 newLocation){
	}

	private void onNeighborsChange(List<Node> newNeighbors){
	}
}
