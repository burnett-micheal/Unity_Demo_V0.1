using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfind {
	private Dictionary<Node, Node> previousNodes = new Dictionary<Node, Node>();
	private List<Node> checkNodes = new List<Node> ();
	private List<Node> checkedNodes = new List<Node> ();
	private Node start;
	private Node end;

	public Pathfind(Node Start, Node End){
		start = Start;
		end = End;
	}

	public List<Node> getPath(){
		//Add Neighbors, Get Closest, Set Prev
		Node baseNode = start;
		Node nextNode = null;
		bool isEndNeighbor = false;

		while (isEndNeighbor == false) {
			isEndNeighbor = baseNode.neighbors.Contains (end);
			if (isEndNeighbor) {break;}
			addNeighbors (baseNode);
			nextNode = getClosest ();
			if (nextNode == null) {
				break;
			}
			previousNodes.Add (nextNode, baseNode);
			baseNode = nextNode;
			nextNode = null;
		}

		return isEndNeighbor ? prevToPath (baseNode) : null;
	}

	private void addNeighbors(Node node){
		node.neighbors.ForEach (n => {
			if(n.type == "space" && !checkNodes.Contains(n) && !checkedNodes.Contains(n)){
				checkNodes.Add(n);
			}
		});
	}

	private Node getClosest(){
		float dis = Mathf.Infinity;
		Node winner = null;
		checkNodes.ForEach (node => {
			Vector2 nPos = node.location;
			Vector2 gPos = end.location;
			float disToGoal = Vector2.Distance(nPos, gPos);
			if(disToGoal < dis){
				winner = node; 
				dis = disToGoal;
			}
		});
		checkNodes.Remove (winner);
		checkedNodes.Add (winner);
		return winner;
	}

	private List<Node> prevToPath(Node last){
		Node baseNode = last;
		List<Node> path = new List<Node> ();
		while (baseNode != start) {
			path.Add (baseNode);
			baseNode = previousNodes [baseNode];
		}
		path.Reverse ();
		return path;
	}
}
