using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;

    private float attackCD = 0.25f;
    [SerializeField] private AudioClip fireballAudio;
   
    private Movement move;

    private float fireballMoveSpeed = 2f;
    private float cooldownTimer = Mathf.Infinity;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireball;

 
    private void Awake()
    {
        anim = GetComponent<Animator>();
        move = GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (move.view.IsMine)
        {
            if (Input.GetMouseButtonDown(0) && cooldownTimer > attackCD && move.canAttack())
            {
                Attack();                              
            }
            cooldownTimer += Time.deltaTime;
        }
    } 
    //Instantiating fireball when the player is attacking
    private void Attack()
    {
        cooldownTimer = 0;
        anim.SetTrigger("isAttacking");

        GameObject fireballshot = PhotonNetwork.Instantiate(fireball.name, firePoint.position, Quaternion.identity);

        fireballshot.SetActive(true);
        fireballshot.transform.position = firePoint.position;
        fireballshot.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
        move.view.RPC("PlayAudio", RpcTarget.All);
    } 
    [PunRPC]
    public void PlayAudio()
    {
        SoundManager.instance.PlaySound(fireballAudio);
    }

}
