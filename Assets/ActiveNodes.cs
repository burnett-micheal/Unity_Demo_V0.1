using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ActiveNodes : MonoBehaviour {

	public class Bullet{
		public ActiveNodes activeNodes; // Use activeNodes As A Reference To This Script To Add Create Read And Delete Within The Class
		public string direction;
		public Node node;
		public Bullet(string Direction, Node _Node){
			direction = Direction;
			node = _Node;
		}
	}

	public class Enemy{
		public ActiveNodes activeNodes;
		public List<Node> path = new List<Node>();
		public int moveTime = 3;
		public Node node;
		public Enemy(Node _Node){
			node = _Node;
		}
	}

	public class Spawner{
		public ActiveNodes activeNodes;
		public int spawnTime = 8;
		public Node node;
		public Spawner(Node _Node){
			node = _Node;
		}
	}

	public Node player;
	public Nodes nodesClass;
	public List<Bullet> bullets = new List<Bullet> ();
	public List<Enemy> enemies = new List<Enemy> ();
	public List<Spawner> spawners = new List<Spawner> ();
	public List<string> directions = new List<string> ();

	public void newSpawner(Node node){
		node.type = "spawner";
		Spawner _spawner = new Spawner (node);
		spawners.Add (_spawner);
	}

	public void delSpawner(Node node){
		node.type = "space";
		spawners.Remove (getSpawner(node));
	}

	public Spawner getSpawner(Node node){
		for (int i = 0; i < spawners.Count; i++) {
			Spawner spawner = spawners [i];
			if (spawner.node == node) {
				return spawner;
			}
		}
		return null;
	}

	public void newBullet(Node node, string direction){
		node.type = "bullet";
		Bullet bullet = new Bullet (direction, node);
		bullets.Add (bullet);
	}

	public void delBullet(Node node){
		node.type = "space";
		bullets.Remove (getBullet(node));
	}

	public Bullet getBullet(Node node){
		for (int i = 0; i < bullets.Count; i++) {
			Bullet bullet = bullets [i];
			if (bullet.node == node) {
				return bullet;
			}
		}
		return null;
	}

	public void newEnemy(Node node){
		node.type = "enemy";
		Enemy enemy = new Enemy(node);
		enemies.Add (enemy);
	}

	public void delEnemy(Node node){
		node.type = "space";
		enemies.Remove (getEnemy(node));
	}

	public Enemy getEnemy(Node node){
		for (int i = 0; i < enemies.Count; i++) {
			Enemy enemy = enemies [i];
			if (enemy.node == node) {
				return enemy;
			}
		}
		return null;
	}

	void Start(){
		directions.Add ("left");
		directions.Add ("right");
		directions.Add ("up");
		directions.Add ("down");
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
			newBullet (newNode, dir);
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
		bullets.ForEach (bullet => {
			string dir = bullet.direction;
			for(int i = 0; i < 2; i++){
				Node n = nodesClass.getNeighbor(bullet.node, dir);
				if(n == null){killHandler(bullet.node); continue;}
				if(n.vulnerable == true){killHandler(bullet.node); killHandler(n); continue;}
				if(n.vulnerable == false && n.traversable == false){killHandler(bullet.node); continue;}
				if(n.vulnerable == false && n.traversable == true){moveHandler(bullet.node, n); continue;}
			}
		});

		spawners.ForEach (spawner => {
			if(spawner.spawnTime == 0){
				for(int i = 0; i < directions.Count; i++){
					string dir = directions[i];
					Node n = nodesClass.getNeighbor(spawner.node, dir);
					if(n != null && n.traversable == true){newEnemy(n); break;}
				}
				spawner.spawnTime = 8;
			} else {
				spawner.spawnTime -= 1;
			}
		});

		enemies.ForEach (enemy => {
			if(enemy.node.neighbors.Contains(player)){
				killHandler(player);
				killHandler(enemy.node);
				return;
			}

			if(enemy.moveTime == 0){
				if(enemy.path.Count == 0){
					Pathfind p = new Pathfind(enemy.node, player);
					enemy.path = p.getPath();
				}

				moveHandler(enemy.node, enemy.path[0]);
				enemy.path.RemoveAt(0);
				enemy.moveTime = 3;
			}
			enemy.moveTime -= 1;
		});
	}

	void killHandler(Node killed){
		string t = killed.type;
		switch (t) {
		case "enemy":
			delEnemy (killed);
			break;
		case "spawner":
			delSpawner (killed);
			break;
		case "bullet":
			delBullet (killed);
			break;
		case "player":
			print ("PLAYER DIED");
			break;
		}
	}

	void moveHandler(Node node, Node newNode){
		if (newNode.traversable == false) {
			return;
		}

		string t = node.type;
		switch (t) {
		case "enemy":
			Enemy enemy = getEnemy (node);
			node.type = "space";
			newNode.type = "enemy";
			enemy.node = newNode;
			break;
		case "bullet":
			Bullet bullet = getBullet (node);
			node.type = "space";
			newNode.type = "bullet";
			bullet.node = newNode;
			break;
		case "player":
			node.type = "space";
			newNode.type = "player";
			player = newNode;
			break;
		}
	}
}
