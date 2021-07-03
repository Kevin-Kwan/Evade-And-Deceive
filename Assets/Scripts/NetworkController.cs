using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkController : MonoBehaviourPun
{
    private string VersionName = "0.0.1";
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connected");
    }
    private void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
