using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ActiveNodes : MonoBehaviour {

	public Node player;
	public Nodes nodesClass;
	public List<string> directions = new List<string> ();
	public bool updateOnAction;

	public ActiveNodeData activeNodeData;

	void Start(){
		directions.Add ("left");
		directions.Add ("right");
		directions.Add ("up");
		directions.Add ("down");

		if (!updateOnAction) {StartCoroutine (updateLoop ());}
	}

	void Update(){
		if (Input.GetKey (KeyCode.LeftShift)) {
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				shoot ("down");
			}
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				shoot ("up");
			}
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				shoot ("left");
			}
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				shoot ("right");
			}
		} else {
			if (Input.GetKeyDown (KeyCode.DownArrow)) {
				move ("down");
			}
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				move ("up");
			}
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				move ("left");
			}
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				move ("right");
			}
		}
	}


	void move (string dir) {
		Node prevNode = player;
		Node newNode = nodesClass.getNeighbor (prevNode, dir);
		moveHandler (player, newNode);

		onAction ();
	}

	void shoot(string dir){
		Node newNode = nodesClass.getNeighbor (player, dir);

		if (newNode.vulnerable == false && newNode.traversable == true) {
			activeNodeData.bullets.Create(newNode, dir);
			onAction ();
			return;
		}

		if (newNode.vulnerable == true) {
			killHandler (newNode);
			onAction ();
			return;
		}
	}

	void onAction(){
		if (updateOnAction) {updateNodes ();}
	}

	IEnumerator updateLoop(){
		while (!updateOnAction) {
			updateNodes ();
			yield return new WaitForSeconds (0.2f);
		}
	}

	void updateNodes(){
		// Can Each Update Be Handled In Its Respective Class?
		// No --- Due To It Needing To Interact With Other Classes, Bullets With Enemies And Spawners
		// However We Can Get Data From The Respective Class Like Spawntime And MoveTime

		activeNodeData.bullets.bullets.ForEach (bullet => {
			string dir = bullet.direction;
			Node n = nodesClass.getNeighbor(bullet.node, dir);
			if(bullet.moveTime <= 0){
				activeNodeData.bullets.resetMoveTime(bullet);
				if(n == null){killHandler(bullet.node); return;}
				if(n.vulnerable == true){killHandler(bullet.node); killHandler(n); return;}
				if(n.vulnerable == false && n.traversable == false){killHandler(bullet.node); return;}
				if(n.vulnerable == false && n.traversable == true){moveHandler(bullet.node, n); return;}
			}
			bullet.moveTime -= 1;
		});

		activeNodeData.spawners.spawners.ForEach (spawner => {
			if(spawner.spawnTime <= 0){
				activeNodeData.spawners.resetSpawnTime(spawner);
				for(int i = 0; i < directions.Count; i++){
					string dir = directions[i];
					Node n = nodesClass.getNeighbor(spawner.node, dir);
					if(n != null && n.traversable == true){activeNodeData.enemies.Create(n); break;}
				}
			}
			spawner.spawnTime -= 1;
		});

		activeNodeData.enemies.enemies.ForEach (enemy => {
			if(enemy.node.neighbors.Contains(player)){
				killHandler(player);
				killHandler(enemy.node);
				return;
			}

			if(enemy.moveTime <= 0){
				activeNodeData.enemies.resetMoveTime(enemy);
				Pathfind p = new Pathfind(enemy.node, player);
				enemy.path = p.getPath(); // Sometimes p.getPath Will Return Null If No Path Can Be Found --- Enemy Is Surrounded By Non-Traversable Nodes, Other Enemies Walls Bullets Etc
				if(enemy.path.Count > 0){
					moveHandler(enemy.node, enemy.path[0]);
					enemy.path.RemoveAt(0);
				}
			}
			enemy.moveTime -= 1;
		});
	}

	void killHandler(Node killed){
		string t = killed.type;
		switch (t) {
		case "enemy":
			activeNodeData.enemies.Delete (killed);
			break;
		case "spawner":
			activeNodeData.spawners.Delete (killed);
			break;
		case "bullet":
			activeNodeData.bullets.Delete (killed);
			break;
		case "player":
			print ("PLAYER DIED");
			break;
		}
	}

	bool moveHandler(Node node, Node newNode){
		if (newNode.traversable == false) {
			return false;
		}

		string t = node.type;
		switch (t) {
		case "enemy":
			activeNodeData.enemies.Read (node).node = newNode;
			node.type = "space";
			newNode.type = "enemy";
			break;
		case "bullet":
			activeNodeData.bullets.Read (node).node = newNode;
			node.type = "space";
			newNode.type = "bullet";
			break;
		case "player":
			node.type = "space";
			newNode.type = "player";
			player = newNode;
			break;
		}

		return true;
	}
}
