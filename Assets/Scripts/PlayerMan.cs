using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMan : MonoBehaviour
{
    public GameObject PlayerPrefab;
    PhotonView PV;

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
            {
            CreateController();
        }
    }

    private void Awake()
    {
        PV=GetComponent<PhotonView>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateController()
    {
        float randomValue = Random.Range(0f, 1f);
        PhotonNetwork.Instantiate(PlayerPrefab.name, new Vector3(0f, 50f * randomValue, 50f * randomValue), Quaternion.identity, 0);

    }
}
