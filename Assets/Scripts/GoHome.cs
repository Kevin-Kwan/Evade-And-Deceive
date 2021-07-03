using System.Collections;
using System.Collections.Generic;
using UnityEngine;using Photon.Pun;
using UnityEngine.SceneManagement;

public class GoHome : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
