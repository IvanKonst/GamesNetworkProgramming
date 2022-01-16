using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlatformMovement : MonoBehaviourPun, IPunObservable
{
    public Animator anim;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);

        }

        else
        {
            transform.position = (Vector3)stream.ReceiveNext();

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            anim.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
