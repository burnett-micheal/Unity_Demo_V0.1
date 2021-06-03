using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveNodeData : MonoBehaviour {
	public Bullets bullets = new Bullets();
	public Enemies enemies = new Enemies();
	public Spawners spawners = new Spawners ();


	public class Bullets{
		public List<Bullet> bullets = new List<Bullet> ();
		public int maxMoveTime = 0;

		public void Create(Node node, string direction){
			node.type = "bullet";
			Bullet bullet = new Bullet (direction, node);
			bullet.moveTime = maxMoveTime;
			bullets.Add (bullet);
		}

		public Bullet Read(Node node){
			for (int i = 0; i < bullets.Count; i++) {
				Bullet bullet = bullets [i];
				if (bullet.node == node) {
					return bullet;
				}
			}
			return null;
		}

		public void Delete(Node node){
			node.type = "space";
			bullets.Remove (Read(node));
		}

		public void resetMoveTime(Bullet bullet){
			bullet.moveTime = maxMoveTime;
		}

		public class Bullet{
			public string direction;
			public int moveTime;
			public Node node;
			public Bullet(string Direction, Node _Node){
				direction = Direction;
				node = _Node;
			}
		}
	}

	public class Enemies{
		public List<Enemy> enemies = new List<Enemy> ();
		private int maxMoveTime = 3;

		public void Create(Node node){
			node.type = "enemy";
			Enemy enemy = new Enemy(node);
			enemy.moveTime = maxMoveTime;
			enemies.Add (enemy);
		}

		public Enemy Read(Node node){
			for (int i = 0; i < enemies.Count; i++) {
				Enemy enemy = enemies [i];
				if (enemy.node == node) {
					return enemy;
				}
			}
			return null;
		}

		public void Delete(Node node){
			node.type = "space";
			enemies.Remove (Read(node));
		}

		public void resetMoveTime(Enemy enemy){
			enemy.moveTime = maxMoveTime;
		}

		public class Enemy{
			public List<Node> path = new List<Node>();
			public int moveTime;
			public Node node;
			public Enemy(Node _Node){
				node = _Node;
			}
		}
	}

	public class Spawners{
		public List<Spawner> spawners = new List<Spawner> ();
		public int maxSpawnTime = 8;

		public void Create(Node node){
			node.type = "spawner";
			Spawner spawner = new Spawner (node);
			spawner.spawnTime = maxSpawnTime;
			spawners.Add (spawner);
		}

		public Spawner Read(Node node){
			for (int i = 0; i < spawners.Count; i++) {
				Spawner spawner = spawners [i];
				if (spawner.node == node) {
					return spawner;
				}
			}
			return null;
		}

		public void Delete(Node node){
			node.type = "space";
			spawners.Remove (Read(node));
		}

		public void resetSpawnTime(Spawner spawner){
			spawner.spawnTime = maxSpawnTime;
		}

		public class Spawner{
			public int spawnTime;
			public Node node;
			public Spawner(Node _Node){
				node = _Node;
			}
		}
	}
}