using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Joining lobby once connected to master
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    //When the lobby is joined, the MainMenu scene is loaded.
    public override void OnJoinedLobby()
    {
          SceneManager.LoadScene("MainMenu");
        print("Player has succesfully joined the lobby.");
    }
    //If the player coldn't connect to the lobby, a disconnection reason is sent
    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server for reason:" + cause.ToString());
    }
  
}
