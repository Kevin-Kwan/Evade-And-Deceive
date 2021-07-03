using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileScript : MonoBehaviour
{
    public float expiryTime = 0f;
    // public GameObject Shooter;
    // public GameObject Shell;

    void Start()
    {
        
        //Destroy(gameObject, expiryTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
       this.GetComponent<Rigidbody>().velocity = 50 * (this.GetComponent<Rigidbody>().velocity.normalized);
        GetComponent<Rigidbody>().AddForce(Physics.gravity*5f, ForceMode.Acceleration);
    }
    // public void Shoot()
    // {

    // }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "funobject")
        {
            //UnityEngine.Debug.Log("what");
            

        }
    }
    // // [PunRPC]
    // public void RpcShoot()
    // {
    //     // GameObject newShell = Instantiate((GameObject)));
    // }
}
