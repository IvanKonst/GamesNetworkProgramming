using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField createInput;
    public InputField joinInput;
    public GameObject lobbyCanvas;
    public GameObject howToPlayCanvas;
    //Create a room for the players to join
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(createInput.text);
    }
    //Join the room if it exists
    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }
    //When the room is joined load the Game level
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Game");
    }
    public void OnHowToPlayButtonPressed()
    {
        lobbyCanvas.SetActive(false);
        howToPlayCanvas.SetActive(true);
    }
    public void OnBackButtonpressed()
    {
        howToPlayCanvas.SetActive(false);
        lobbyCanvas.SetActive(true);
    }
}
