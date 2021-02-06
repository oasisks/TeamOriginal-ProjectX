using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float playerSpeed; //30
    //TODO: different move speed in air / water / spiderweb / ice
    //[SerializeField] float airSpeed; //The movement speed when in the air
    [SerializeField] float movAccel; //The maximum change in velocity the player can do on the ground. This determines how responsive the character will be when on the ground.
    //[SerializeField] float airMovAccel; //The maximum change in velocity the player can do in the air. This determines how responsive the character will be in the air.
    //[SerializeField] private float smoothTime; //0.05

    [Header("Jump")]
    [SerializeField] float initialJumpForce;    //The force applied to the player when starting to jump
    [SerializeField] float holdJumpForce;       //The force applied to the character when holding the jump button
    [SerializeField] float maxJumpTime;         //The maximum amount of time the player can hold the jump button
    [SerializeField] float gravityMultiplier;   // 2.7?

    [Header("Ground detection")]
    [SerializeField] float groundCastRadius;
    [SerializeField] float groundCastDist;
    [SerializeField] ContactFilter2D groundFilter;

    [Header("Misc")]
    [SerializeField] public int health; // we may change this to hearts (i.e. a player has 3 hearts and if all three hearts are gone then you die)
    [SerializeField] private float invincibleTime;

    [Header("Tiles")]
    [SerializeField] private TileBase iceTile;
    [SerializeField] private TileBase slimeTile;
    [SerializeField] private TileBase spiderwebTile;
    [SerializeField] private TileBase conveyorLeft;
    [SerializeField] private TileBase conveyorRight;

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

    // audio
    public AudioSource walkAudioSource;
    public AudioSource jumpAudioSource;
    public float walkVol;

    // canvas
    private CanvasManager canvas;

    private GameManager gm;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //jumpForceVector = new Vector2(0, jumpForce);
        animator = GetComponent<Animator>();
        World = FindObjectOfType<worldHandler>(); //TODO: Find by tag?
        canvas = GameObject.FindGameObjectWithTag("canvas").GetComponent<CanvasManager>();
        gm = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        rb.AddForce(gravityMultiplier * Physics2D.gravity * rb.mass, ForceMode2D.Force);
        belowTile = CheckGround();
        withinTile = CheckTileWithin(); //get squished
        Death();
        ResetInvincibleAnim();

        if(!canvas.pauseMenuOn && !canvas.levelFinishedPanelOn)
            Move(); // move when pause menu is off and when the level finished panel is off
    }

    private void Move()
    {
        Vector2 velocity = rb.velocity;

        //calculate the ground direction based on the ground normal
        //Vector2 groundDir = Vector2.Perpendicular(DoGroundCast().normal).normalized;
        //groundDir.x = -groundDir.x; //Vector2.Perpendicular rotates the vector 90 degrees counter clockwise, inverting X. So here we invert X back to normal

        //The velocity we want our character to have. We get the movement direction, the ground direction and the speed we want (ground speed or air speed)
        Vector2 keyVelocity = new Vector2(0, 0);
        if(Input.GetKey(KeyCode.A)) { //don't use getaxis horizontal
            keyVelocity.x = -1.0f;
        } else if(Input.GetKey(KeyCode.D)) {
            keyVelocity.x = 1.0f;
        }

        Vector2 targetVelocity = /*groundDir * */keyVelocity * playerSpeed; // * (isGrounded ? movSpeed : airMovSpeed);


        if (isGrounded) walkAudioSource.volume = velocity.x*walkVol;
        //The change in velocity we need to perform to achieve our target velocity
        Vector2 velocityDelta = targetVelocity - velocity;

        //The maximum change in velocity we can do
        float maxDelta = movAccel; /*isGrounded ? movAccel : airMovAccel*/

        if(isGrounded) {
            if(belowTile == slimeTile) {
                rb.AddForce(Vector3.up * 2 * rb.mass, ForceMode2D.Impulse); // change force strength?
            } else if(belowTile == iceTile) {
                if(targetVelocity.magnitude < velocity.magnitude) { //} || targetVelocity.x*velocity.x <= 0) {
                    maxDelta = 0.05f;
                }
            } else if(belowTile == conveyorLeft) {
                rb.AddForce(new Vector2(-30, 0) * rb.mass, ForceMode2D.Force);
            }  else if(belowTile == conveyorRight) {
                rb.AddForce(new Vector2(30, 0) * rb.mass, ForceMode2D.Force);
            }
        }

        if(withinTile == spiderwebTile) {
            rb.drag = 100;
        } else {
            rb.drag = 0;
        }

        //Clamp the velocity delta to our maximum velocity change, y = 0 because we don't want to move the character vertically
        velocityDelta = new Vector2(Mathf.Clamp(velocityDelta.x, -maxDelta, maxDelta), 0);

        //Apply the velocity change to the character
        rb.AddForce(velocityDelta * rb.mass, ForceMode2D.Impulse);
        animator.SetFloat("Speed", Mathf.Abs(velocityDelta.magnitude));

        //obsolete movement commands (dont use these)
        //keyVelocity = keyVelocity.normalized * playerSpeed;
        //rb.velocity = Vector3.SmoothDamp(rb.velocity, keyVelocity*playerSpeed, ref referencedVelocity, 0.15f);//smoothTime);
        //rb.MovePosition(rb.position + keyVelocity * playerSpeed * Time.deltaTime);
        //transform.Translate(keyVelocity * playerSpeed * Time.deltaTime);

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
            jumpAudioSource.Play();
            //rb.AddForce(jumpForceVector);
            isGrounded = false;

            rb.AddForce(Vector3.up * initialJumpForce * rb.mass, ForceMode2D.Impulse);

            StartCoroutine(JumpCoroutine());
        }
    }

    IEnumerator JumpCoroutine()
    {
        //Counts for how long we've been jumping
        float jumpTimeCounter = 0;

        walkAudioSource.volume = 0;

        while (Input.GetKey(KeyCode.W) && jumpTimeCounter < maxJumpTime)
        {
            jumpTimeCounter += Time.deltaTime;

            rb.AddForce(Vector3.up * holdJumpForce * rb.mass * (1- jumpTimeCounter / maxJumpTime), ForceMode2D.Impulse);

            yield return null;
        }
    }

    private void Death()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            gm.playerKilled = true;
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
        // entityWithinTetrisBlock
        // harmful objects
        if (!invincible)
        {
            if (collision.collider.tag == "harmfulObjects")
            {
                // minus however the damage is
                // this will vary base on the object type (i.e. lava, enemies)
                // this is assuming we are not using a discrete health system
                UpdateHealth();
                Debug.Log("I hit harmful object");
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("enemy"))
            {
                // for now we are assuming that Goomba is the only enemy
                Enemy goomba = collision.collider.GetComponent<Enemy>();

                if (goomba != null && !goomba.hasDied)
                {
                    UpdateHealth();
                    Debug.Log("I hit harmful object");
                }
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("spikes")) // TODO: check if this is a spike
            {
                SpikeBehavior spike = collision.collider.GetComponent<SpikeBehavior>();
                if (spike.dangerous)
                {
                    UpdateHealth();
                    Debug.Log("I hit a spike");
                }
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("powerups"))
            {
                Powerup powerup = collision.gameObject.GetComponent<Powerup>();
                powerup.affectPlayer(gameObject);
            }
        }
    }

    private void UpdateHealth()
    {
        health -= 1;
        animator.SetBool("HitHarmfulObjects", true);
        invincible = true;
        previousTime = Time.time;
        gm.loseLife(health);
    }

    private TileBase CheckGround()
    {
        //If DoGroundCast returns Vector2.zero (it's the same as Vector2(0, 0)) it means it didn't hit the ground and therefore we are not grounded.
        RaycastHit2D hit = DoGroundCast();
        if(isGrounded) {
            Vector3 pos = hit.point;
            pos.y -= 0.5f;
            pos.z = 0;
            return World.getTile_GlobalPos(pos); // Get Tile Below
        } else {
            return null;
        }
    }

    private RaycastHit2D DoGroundCast()
    {
        //We will use this array to get what the CircleCast returns. The size of this array determines how many results we will get.
        //Note that we have a size of 2, that's because we are always going to get the player as the first element, since the cast
        //has its origin inside the player's collider.
        RaycastHit2D[] hits = new RaycastHit2D[2];

        if (Physics2D.CircleCast(transform.position, groundCastRadius, Vector3.down, new ContactFilter2D(), hits, groundCastDist) > 1)
        {
            isGrounded = true;
            return hits[1];
        }

        return new RaycastHit2D();
    }

    private TileBase CheckTileWithin() {
        if(World.checkSolidTile_GlobalPos(transform.position)) { // player is inside a solid block, die
            health = 0;
        }

        return World.getTile_GlobalPos(transform.position);
    }

    public void StopAllAudio()
    {
        walkAudioSource.Stop();
        jumpAudioSource.Stop();
    }
}
