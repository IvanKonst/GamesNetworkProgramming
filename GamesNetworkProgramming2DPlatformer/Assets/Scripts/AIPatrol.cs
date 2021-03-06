using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class AIPatrol : MonoBehaviourPun, IPunObservable
{
    [HideInInspector]
    public bool mustPatrol;
    private bool mustTurn;
   
    public SpawnPlayers spawn;
    public Rigidbody2D rb;
    public Transform groundCheckPos;
    public float enemyHealth = 200;
    public float walkSpeed;
    public LayerMask groundLayer;
    public Text enemyHealthText;
    public Transform enemySpawnPos;
    public Text ScoreText;
    public Animator anim;

    //Setting Player Score;
    void SetScoreText()
    {
        ScoreText.text = "Score : " + spawn.Score.ToString();
    }
    
    void Awake()
    {
 

    }
    void Start()
    {
       
        mustPatrol = true;
    }    

    void Update()
    {
        
            SetScoreText();
        
       
        if (PhotonNetwork.IsMasterClient)
        {          
            if (mustPatrol)
            {
                Patrol();
            }       
        }
        if(photonView.IsMine)
        { 
            if (enemyHealth == 0)
            {

            photonView.RPC("Die", RpcTarget.All);
            Die();

            }
        }
        if (spawn.Score == 10)
        {
            photonView.RPC("VictoryScreen", RpcTarget.All);
        }
    }
    private void FixedUpdate()
    {
        enemyHealthText.text = enemyHealth.ToString();
        if (mustPatrol)
        {
            mustTurn = !Physics2D.OverlapCircle(groundCheckPos.position, 0.5f, groundLayer);
        }     
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Option to add coins to the game
        //if (other.gameObject.CompareTag("Coins"))
        //{
        //    Destroy(other.gameObject);
        //    spawn.coins--;
        //    Debug.Log(spawn.coins);
        //}

        //Triggers when the enemy is colliding with Fireball
        if (other.gameObject.tag == "Fireball")
        {
            if (photonView.IsMine)
            {
                photonView.RPC("EnemyHit", RpcTarget.All);
            }
        }
      

    }
    //Making the enemy patrol from left to right with certain velocity
    void Patrol()
    {
        if(mustTurn)
        {
            Flip();
        }
        rb.velocity = new Vector2(walkSpeed * Time.fixedDeltaTime, rb.velocity.y);
    }
    [PunRPC]
    void EnemyHit()
    {
        enemyHealth -= 20;
    }
    //Destroying the enemy
    [PunRPC]
    void Die()
    {
        
        Destroy(gameObject);
      
            spawn.Score++;
        
      
    }
    //Changing enemy walk direction and rotation
    void Flip()
    {
        mustPatrol = false;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        enemyHealthText.transform.localScale = new Vector3(transform.localScale.x / 10 , 1, 1);
        walkSpeed *= -1;
        mustPatrol = true;
    }

    //Sending information about the transform.position of the enemy, the health and the score to all the clients
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(enemyHealth);
            stream.SendNext(spawn.Score);
            stream.SendNext(enemyHealthText.transform.localScale);
        }

        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            enemyHealth = (float)stream.ReceiveNext();
            spawn.Score = (int)stream.ReceiveNext();
            enemyHealthText.transform.localScale = (Vector3)stream.ReceiveNext();
        }
    }

    //Ending the game
    [PunRPC]
    public void VictoryScreen()
    {
        SceneManager.LoadScene("VictoryScreen");
    }    

}
