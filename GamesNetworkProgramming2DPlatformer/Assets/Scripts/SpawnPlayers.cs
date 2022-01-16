using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
public class SpawnPlayers : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public AIPatrol ai;
    //public int redScore = 0;
    public int startTime;
    public Transform[] spawnPoints;
    public Transform[] coinsSpawnPoints;
    public int Score = 0;
    public int lives = 6;
    public Text timer;
    public int timervalueminutes;
    public int timervalueseconds;
    public int numberOfPlayers = 0;
    public bool timeHasNotBeenSet = true;
    public PhotonView view;
    private Movement move;
    private GameObject enemy;
    public GameObject pausePanel;
    public GameObject gamePanel;
    public Text pausePanelTimer;
    private void Awake()
    {     
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        view = GetComponent<PhotonView>();
    }
    void Start()
    {
        startTime = (int)Time.time;
        Vector2 position = spawnPoints[PhotonNetwork.CountOfPlayers - 1].position;
        PhotonNetwork.Instantiate(playerPrefab.name, position, Quaternion.identity);

        if (PhotonNetwork.CountOfPlayers < 2)
        {       
           // enemy = PhotonNetwork.Instantiate(enemyPrefab.name, enemyStartPos, Quaternion.identity);
        }
        timer.text = "Timer: " + timervalueminutes.ToString() + "m :" + timervalueseconds.ToString() + "s";
    }
   

    void Update()
    {
        if (numberOfPlayers > 1)
        {
            if(timeHasNotBeenSet)
            {
                startTime = (int)Time.time;
                timeHasNotBeenSet = false;
            }
            timervalueminutes = (int)(Time.time - startTime) / 60;
            timervalueseconds = (int)(Time.time - startTime)%60;
        }
        timer.text = "Timer: " + timervalueminutes.ToString() + "m :" + timervalueseconds.ToString() + "s";
    }
    //Quit the application
    public void OnExitButtonClicked()
    {
        Application.Quit();
        numberOfPlayers--;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(timervalueminutes);
            stream.SendNext(timervalueseconds);


        }

        else
        {
            timervalueminutes = (int)stream.ReceiveNext();
            timervalueseconds = (int)stream.ReceiveNext();
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        pausePanelTimer.text = "The timer was: " + timervalueminutes + " minutes and " + timervalueseconds + " seconds ";
        numberOfPlayers = 1;
        timeHasNotBeenSet = true;
        Time.timeScale = 0;
        gamePanel.SetActive(false);
        pausePanel.SetActive(true);
    }
}
