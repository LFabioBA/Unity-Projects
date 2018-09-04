using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public float speed;      //Velocidade do Jogador
	private float move;		 //Controlador de velocidade do Jogador
	public float jumpForce;  //Força do Pulo
	public int QuantidadeBullet; //Quantidade de tiros

	private bool grounded;   //Verifica se esta no chão
	private bool jumping;    //Verifica se esta pulando

	public bool isAlive=true;      //Verifica se esta vivo 
	public int life;               //Vidas do jogador
	public bool invunerable=false; //invunerabilidade do jogador

	AudioSource soundJump; //Som de pulo
	AudioSource soundFire;

	public float nextFire;   //Quantidade de tiros
	public float fireRate;   //Intervalo de tempo entre tiros
	public GameObject bullet; //Bala
	public Transform spanwBullet; //Lugar onde a bala é criada

	public Transform groundCheck; //Local de verificação se o jogador toca no chão
	SpriteRenderer sprite;		  //Controladora do SpriteRenderer do jogador
	Animator anim;				  //Controladora de animção do jogador
	Rigidbody2D rb2d;			  //Controladora do Rigidbody(fisica) do jogador

	Rigidbody2D rb2d2; 	  //Controladora do Rigidbody(fisica) do jogador - Auxiliar

	public Transform attackCheck;
	public float radiusAttack;
	public LayerMask layerEnemy;
	float timeNextAttack;

	void Awake(){
		//Pega componentes
		sprite = GetComponent<SpriteRenderer> ();
		anim = GetComponent<Animator> ();
		rb2d = GetComponent<Rigidbody2D> ();
		soundJump = GetComponent<AudioSource> ();
		soundFire = GameObject.Find ("Main Camera").GetComponent<AudioSource> ();
		rb2d2 = GameObject.Find ("BG").GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {
		//Se estiver morto. Fica parado
		if (!isAlive) {
			rb2d.bodyType = rb2d2.bodyType;
		}
		//Verifica se o GroundCheck esta na layer "Ground"
		grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		//Toca som de pulo
		if (Input.GetButtonDown ("Jump") && grounded) {
			jumping = true;
			soundJump.Play ();
		}

	}
	void FixedUpdate(){
		if (isAlive) {
			//Pega a direção da movimentação
			move = Input.GetAxis ("Horizontal");
			//Adiciona velocidade
			rb2d.velocity = new Vector2 (speed * move, rb2d.velocity.y);
			//Vira jogador
			if ((move > 0f && sprite.flipX) || (move < 0f && !sprite.flipX)) {
				Flip ();
			}
			//Muda animaçoes
			anim.SetBool ("JumpFall", rb2d.velocity.y != 0f);
			anim.SetFloat ("Speed", Mathf.Abs (move));
			//Pula
			if (jumping) {
				rb2d.AddForce (new Vector2 (0f, jumpForce));
				jumping = false;
			}
			//Atira
			if (Input.GetButton ("Fire1") && Time.time > nextFire && QuantidadeBullet>0) {
				Fire ();
			}
			if (Input.GetButtonDown ("Fire2")) {
				anim.SetTrigger ("AtackSword");
			}
		} 
	}

	void Fire(){
		//Animação de tiro
		QuantidadeBullet--;
		anim.SetTrigger ("Fire");
		soundFire.Play ();
		nextFire = Time.time+fireRate;
		//Cria tiro
		GameObject cloneBullet = Instantiate (bullet, spanwBullet.position, spanwBullet.rotation);
		//Vira a bala
		if (sprite.flipX) {
			cloneBullet.transform.eulerAngles = new Vector3 (0,0,180);
		}

	}
	void Flip (){
		//Vira jogador e local onde bala é criada
		sprite.flipX = !sprite.flipX;
		if (!sprite.flipX) {
			spanwBullet.position = new Vector3 (this.transform.position.x + 0.48f, spanwBullet.position.y, spanwBullet.position.z);
			attackCheck.position = new Vector3 (this.transform.position.x + 0.60f, attackCheck.position.y, attackCheck.position.z);
		} else {
			spanwBullet.position = new Vector3 (this.transform.position.x - 0.48f, spanwBullet.position.y, spanwBullet.position.z);
			attackCheck.position = new Vector3 (this.transform.position.x - 0.60f, attackCheck.position.y, attackCheck.position.z);
		}
	}

	IEnumerator Damage(){
		//Jogador fica invuneravel
		invunerable = true;
		//Jogador pisca
		for (float i = 0f; i < 1f; i += 0.1f) {
			sprite.enabled = false;
			yield return new WaitForSeconds (0.1f);
			sprite.enabled = true;
			yield return new WaitForSeconds (0.1f);
		}
		//Jogador fica vuneravel
		invunerable = false;
	}

	public void DamagePlayer(){

		if(isAlive){
			//Ativa a vunerabilidade
			invunerable = true;
			//Perde vida
			life--;

			StartCoroutine (Damage());

			if(life < 1){
				//Morte
				isAlive = false;
				anim.SetTrigger ("Dead");
				Invoke ("ReloadLevel",3f);
			}
		}
	}

	void ReloadLevel(){
		//Recarrega fase atual
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}

	void OnCollisionEnter2D(Collision2D obj){
		if (obj.transform.tag == "Plataforma") {
			transform.parent = obj.transform;

		}

	}
	void OnCollisionExit2D(Collision2D obj){
		if (obj.transform.tag == "Plataforma") {
			transform.parent = null;

		}
	}
	/*void OnDrawGizmosSelected(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (attackCheck.position, radiusAttack);
	}
	void PlayerAttackSword(){
		Collider2D[] enemiesAtack = Physics2D.OverlapCircleAll (attackCheck.position, radiusAttack, layerEnemy);

		for(int i=0; i < enemiesAtack.Length;i++){
			enemiesAtack [i].SendMessage ("EnemyHit");
			Debug.Log (enemiesAtack [i].name);
		}
	}
	*/
}
