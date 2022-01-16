using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Movement : MonoBehaviourPun, IPunObservable
{
    private Animator anim;

    private Rigidbody2D rig;

    private BoxCollider2D box;

    private float jumpSpeed = 18;

    [SerializeField] private float speed;
    private float wallJumpCD;
    private float horizontalInput;
    private float direction = 1;

    private int team = 0;
     
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;


    public PhotonView view;

    public GameObject playerCamera;

    public SpawnPlayers spawn;
    public Vector2 healthRotation;

    public float playerHealth = 100;


    public Text livesText;
    public Text healthText;

    public Vector3 start_pos;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        view = GetComponent<PhotonView>();
        spawn = FindObjectOfType<SpawnPlayers>();
    }
  
    private void Start()
    {
        
        spawn.numberOfPlayers++;
        spawn.gamePanel.SetActive(true);
        livesText.text = "Team lives : " + spawn.lives.ToString();
        view = GetComponent<PhotonView>();
        if(view.IsMine)
        {
            playerCamera.SetActive(true);
           
        }
        else
        {
            playerCamera.SetActive(false);
            livesText.enabled = false;
        }
      
        gameObject.tag = "Player";
        start_pos = gameObject.transform.position;
    }
    private void Update()
    {
        healthText.text = playerHealth.ToString();
        if (view.IsMine)
        {
           
            Move();
        
        }
        if(transform.localScale == Vector3.one)
        {
            healthText.transform.localScale = Vector3.one * 2;
        }
        else
        {
            healthText.transform.localScale = new Vector3(-1, 1, 1)*2;
        }
        if (spawn.lives == 0)
        {
            SceneManager.LoadScene("EndScene");
        }
    }
    private void FixedUpdate()
    {
        livesText.text = "Team lives : " + spawn.lives.ToString();
    }
    //Letting the player Jump and to jump on walls using wall jump
    private void Jump()
    {
        if (view.IsMine)
        {
            if (isGrounded())
            {
                rig.velocity = new Vector2(rig.velocity.x, jumpSpeed);
                anim.SetTrigger("jump");
            }
            else if (holdingOnWall() && !isGrounded())
            {
                if (horizontalInput == 0)
                {
                    rig.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0);
                    transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    rig.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
                }
                wallJumpCD = 0;

            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Option for PVP
        if (other.gameObject.tag == "Fireball")
        {           
                view.RPC("Damage", RpcTarget.All);
     
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Dealing damage to the enemy
        if(collision.gameObject.tag == "Enemy" )
        {
            playerHealth = playerHealth - 50;            
            if (playerHealth <= 0)
            {
                spawn.lives--;
                view.RPC("FatalDamage", RpcTarget.All);
                //  view.RPC("RPC_PlayerKilled", RpcTarget.All, team);
                view.RPC("Respawn", RpcTarget.All);
            }
            else
            {
                anim.SetTrigger("isHurting");
            }
        }
        if(collision.gameObject.tag == "DeathLine")
        {
            spawn.lives--;
            view.RPC("FatalDamage", RpcTarget.All);
            view.RPC("Respawn", RpcTarget.All);
        }
    }

    //Letting the player move using horizontalInput 
    public void Move()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if (horizontalInput > 0.01f)
        {
            transform.localScale = Vector3.one;
            healthText.transform.localScale = Vector3.one;
       

        }
        else if (horizontalInput < -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            healthText.transform.localScale = new Vector3(-1, 1, 1);

        }


        anim.SetBool("isRunning", horizontalInput != 0);
        anim.SetBool("isGrounded", isGrounded());

        if (wallJumpCD > 0.2f)
        {

            rig.velocity = new Vector2(horizontalInput * speed, rig.velocity.y);
            if (holdingOnWall() && isGrounded())
            {
                rig.gravityScale = 0;
                rig.velocity = Vector2.zero;
            }
            else
            {
                rig.gravityScale = 3;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            wallJumpCD += Time.deltaTime;
        }
        direction = Mathf.Sign(transform.localScale.x);
     
    }
    //Checks if the player is grounded
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    //Responsible for player holding on wall if they are holding the correct button
    private bool holdingOnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0,new Vector2(transform.localScale.x,0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    //Sending information to other clients about player's direction and team's lives
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(direction);
            stream.SendNext(spawn.lives);
            stream.SendNext(playerHealth);
        }
      
        else
        {           
            direction = (float)stream.ReceiveNext();
            spawn.lives = (int)stream.ReceiveNext();
            playerHealth = (float)stream.ReceiveNext();

        }
    }
    //Checking if the player is grounded and not holding on wall
    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !holdingOnWall();
    }
    //Player taking Damage if hit by another player. /PVP
    [PunRPC]
    public void Damage()
    {      
        playerHealth -= 10;
    }
    //Player losing all their health;
    [PunRPC]
    public void FatalDamage()
    {
        playerHealth -= 100;
 
        anim.SetTrigger("isDead");     
    }
    [PunRPC]
    //Player respawning if they are killed
    public void Respawn()
    {
        playerHealth = 100;
        gameObject.transform.position = start_pos;
        anim.SetTrigger("notDead");        
    }

   
   
}
