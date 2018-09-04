using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public int damage=1;       //dano da bala
	public float speed;        //Velocidade da bala
	public float timeDestroy; //Tempo de destruição da bala

	// Use this for initialization
	void Start () {
		//Destroi balla
		Destroy(gameObject, timeDestroy);
	}
	
	// Update is called once per frame
	void Update () {
		//Direção da bala
		transform.Translate (Vector2.right*speed*Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other){
		//Verifica se a bala tocou com inimigo
		if (other.CompareTag ("Enemy")) {

			EnemyController enemy = other.GetComponent<EnemyController> ();
			//Tira vida do inimigo
			if(enemy != null){
				enemy.DamageEnemy (damage);
			}
		}
		Destroy(gameObject);
	}
}
