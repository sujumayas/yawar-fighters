using UnityEngine;
using System.Collections;


public class Player : MonoBehaviour {

	private Rigidbody2D myRigidbody;

	[SerializeField]
	private float movementSpeed;

	private bool facingRight;
	private bool keypressedq;
	private bool keypressedr;
	private bool keypressedspace;

	[SerializeField]
	private bool attacking;

	private Animator myAnimator;

	[SerializeField]
	private Transform[] groundPoints;

	[SerializeField]
	private float groundRadius;

	[SerializeField]
	private LayerMask whatIsGround;

	private bool isGrounded;

	[SerializeField]
	private bool airControl;

	private bool jump;
	[SerializeField]
	private float jumpForce;


	// Use this for initialization
	void Start () {
		//Start facing Right
		facingRight = true;

		myRigidbody = GetComponent<Rigidbody2D>();

		myAnimator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update(){
		HandleInput();
	}
	// You need to move the character in FixUpdate because of Rigidbody logics. 
	void FixedUpdate () {
		float horizontal = Input.GetAxis("Horizontal");

		isGrounded = IsGrounded();

		HandleMovement(horizontal);
		Flip(horizontal);
		HandleCombos();
		ResetValues();
	}

	private void HandleMovement(float horizontal){
		if(!this.myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("is_attacking")
			&& (isGrounded || airControl)
		){
			myRigidbody.velocity = new Vector2(horizontal * movementSpeed, myRigidbody.velocity.y);
		}
		if(isGrounded && jump){
			isGrounded = false;
			myRigidbody.AddForce(new Vector2(0, jumpForce));
		}
		myAnimator.SetFloat("speed", Mathf.Abs(horizontal));
	}

	private void Flip(float horizontal){
		if(horizontal > 0 && !facingRight || horizontal < 0 && facingRight){
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}

	private void HandleCombos(){
		if(!attacking){
			if(keypressedq){
				myAnimator.SetTrigger("b_attack");
				myRigidbody.velocity = Vector2.zero;
			}else if(keypressedr){
				myAnimator.SetTrigger("r_attack");
				myRigidbody.velocity = Vector2.zero;
			}
			attacking = true;		
		}else if (attacking && keypressedq){
			myAnimator.SetTrigger("combo_1");
		}
		keypressedq = false;
		keypressedr = false;
	}

	private void ResetValues(){
		if( !myAnimator.GetCurrentAnimatorStateInfo(0).IsTag("is_attacking") ){
			attacking = false;
		}
		jump = false;
	}

	private void HandleInput(){
		if(Input.GetKeyDown(KeyCode.Q)){
			keypressedq = true;
		}else if(Input.GetKeyDown(KeyCode.R)){
			keypressedr = true;
		}else if(Input.GetKeyDown(KeyCode.Space)){
			jump = true;
		}

	}

	private bool IsGrounded(){
		if(myRigidbody.velocity.y <= 0){
			foreach(Transform point in groundPoints){
				Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround); 
				for (int i = 0; i < colliders.Length; i++){
					if(colliders[i].gameObject != gameObject){
						return true;
					}
				}
			}
		}
		return false;
	}

}
