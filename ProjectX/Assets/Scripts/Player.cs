using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float playerSpeed;
    //TODO: different move speed in air?
    //[SerializeField] float airSpeed; //The movement speed when in the air
    //[SerializeField] float movAccel; //The maximum change in velocity the player can do on the ground. This determines how responsive the character will be when on the ground.
    //[SerializeField] float airMovAccel; //The maximum change in velocity the player can do in the air. This determines how responsive the character will be in the air.
    [SerializeField] private float smoothTime;

    [Header("Jump")]
    [SerializeField] float initialJumpForce;        //The force applied to the player when starting to jump
    [SerializeField] float holdJumpForce;           //The force applied to the character when holding the jump button
    [SerializeField] float maxJumpTime;             //The maximum amount of time the player can hold the jump button
    [SerializeField] float gravityMultiplier = 2.7f;

    [Header("Ground detection")]
    [SerializeField] float groundCastRadius;
    [SerializeField] float groundCastDist;
    [SerializeField] ContactFilter2D groundFilter;

    [Header("Misc")]
    [SerializeField] private float health; // we may change this to hearts (i.e. a player has 3 hearts and if all three hearts are gone then you die)
    [SerializeField] private float invincibleTime;

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private bool invincible = false;
    //private Vector2 jumpForceVector;
    private Vector3 scaleVector = new Vector3(1, 1, 1);
    private Vector3 referencedVelocity = Vector3.zero;
    private float previousTime;
    private float horizontalScalar;
    private Animator animator;

    private TileBase belowTile;
    private TileBase withinTile;

    worldHandler World;

    public AudioSource walkAudioSource;
    public AudioSource jumpAudioSource;
    public float walkVol;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //jumpForceVector = new Vector2(0, jumpForce);
        animator = GetComponent<Animator>();
        World = FindObjectOfType<worldHandler>(); //TODO: Find by tag?
    }

    private void Update()
    {
        rb.AddForce(gravityMultiplier * Physics2D.gravity * rb.mass, ForceMode2D.Force);
        belowTile = CheckGround();
        withinTile = CheckTileWithin(); //get squished
        Death();

        PlayerMovement();
        ResetInvincibleAnim();
    }

    private void PlayerMovement()
    {
        // basic movement
        Vector2 velocity = Vector2.zero;
        if(Input.GetKey(KeyCode.A)) { //don't use getaxis horizontal
            velocity.x -= 1;
        } else if(Input.GetKey(KeyCode.D)) {
            velocity.x += 1;
        }

        if (isGrounded) walkAudioSource.volume = velocity.magnitude*walkVol;

        rb.MovePosition(rb.position + velocity * playerSpeed * Time.deltaTime);
        //transform.Translate(velocity * playerSpeed * Time.deltaTime);
        animator.SetFloat("Speed", Mathf.Abs(velocity.magnitude));

        //velocity = velocity.normalized * playerSpeed;
        //rb.velocity = Vector3.SmoothDamp(rb.velocity, velocity, ref referencedVelocity, smoothTime);

        // flip the duck towards the direction the duck is moving
        if (velocity.x < 0)
        {
            scaleVector.x = -1;
        }
        else if (velocity.x > 0)
        {
            scaleVector.x = 1;
        }
        transform.localScale = scaleVector;

        // jumping
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            //rb.AddForce(jumpForceVector);
            isGrounded = false;

            rb.AddForce(Vector3.up * initialJumpForce * rb.mass);

            StartCoroutine(JumpCoroutine());
        }
    }

    IEnumerator JumpCoroutine()
    {
        //Counts for how long we've been jumping
        float jumpTimeCounter = 0;

        walkAudioSource.volume = 0;
        jumpAudioSource.Play();

        while (Input.GetKey(KeyCode.W) && jumpTimeCounter < maxJumpTime)
        {
            jumpTimeCounter += Time.deltaTime;

            rb.AddForce(Vector3.up * holdJumpForce * rb.mass * (1- jumpTimeCounter / maxJumpTime), ForceMode2D.Force);

            yield return null;
        }
    }

    private void Death()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ResetInvincibleAnim()
    {
        if (Time.time - previousTime > invincibleTime)
        {
            animator.SetBool("HitHarmfulObjects", false);
            invincible = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.collider.name);
        /*
        // ground collision for jumping
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            isGrounded = true;
        }*/

        // harmful objects
        if (collision.collider.tag == "harmfulObjects" && !invincible)
        {
            // minus however the damage is
            // this will vary base on the object type (i.e. lava, enemies)
            // this is assuming we are not using a discrete health system
            health -= 1;
            animator.SetBool("HitHarmfulObjects", true);
            invincible = true;
            previousTime = Time.time;
            Debug.Log("I hit harmful object");

        }
    }

    private TileBase CheckGround()
    {
        //If DoGroundCast returns Vector2.zero (it's the same as Vector2(0, 0)) it means it didn't hit the ground and therefore we are not grounded.
        isGrounded = DoGroundCast() != Vector2.zero;
        if(isGrounded) {
            Vector3 pos = transform.position;
            pos.y -= 1;
            pos.z = 0;
            return World.getTile_GlobalPos(pos); // Get Tile Below
        } else {
            return null;
        }
    }

    Vector2 DoGroundCast()
    {
        //We will use this array to get what the CircleCast returns. The size of this array determines how many results we will get.
        //Note that we have a size of 2, that's because we are always going to get the player as the first element, since the cast
        //has its origin inside the player's collider.
        RaycastHit2D[] hits = new RaycastHit2D[2];

        if (Physics2D.CircleCast(transform.position, groundCastRadius, Vector3.down, new ContactFilter2D(), hits, groundCastDist) > 1)
        {
            return hits[1].normal;
        }

        return Vector2.zero;
    }

    private TileBase CheckTileWithin() {
        if(World.checkSolidTile_GlobalPos(transform.position)) { // player is inside a solid block, die
            health = 0;
        }

        return World.getTile_GlobalPos(transform.position);
    }
}
