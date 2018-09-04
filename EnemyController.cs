using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	public int life;                 //Vida do inimigo
	public float speed;              //Velocidade do inimigo
	public float distance;           //Distancia auxiliar de detecção do personagem
	protected bool isMoving=false;   //Verifica movimentação
	public float distanceAttack;     //Zona de detecção do jogador
	protected Rigidbody2D rb2d;      //Controladora do Rigidbody(fisica) do inimigo
	protected Animator anim;         //Controladora de animações do inimigo
	protected Transform player;      //Jogador
	protected SpriteRenderer sprite; //Pega SpriteRenderer do inimigo

	public void Awake (){
		//Pega componentes
		rb2d = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		sprite = GetComponent<SpriteRenderer> ();
		player = GameObject.Find ("Player").GetComponent<Transform> ();
	}

	protected float PlayerDistance(){
		//Distancia de detecção do jogador
		return Vector2.Distance(player.position, transform.position);
	}

	protected void Flip(){
		//Vira personagem
		sprite.flipX = !sprite.flipX;
		speed *= -1;
	}
	//Função Update padrão
	protected virtual void Update(){
		distance = PlayerDistance ();
		isMoving = (distance < distanceAttack);
		//Vira personagem
		if (isMoving) {
			if((player.position.x > transform.position.x && sprite.flipX) || (player.position.x < transform.position.x && !sprite.flipX)){
				Flip ();
			}
		}
	}

	public void DamageEnemy(int damageBullet){
		//Diminui vida do inimigo
		life -= damageBullet;
		StartCoroutine (Damage());
		//Morte
		if (life < 1) {
			Destroy (gameObject);
		}
	}
	//Muda coloração
	IEnumerator Damage(){
		sprite.color = Color.red;
		yield return new WaitForSeconds (0.1f);
		sprite.color = Color.white;

	}

}
