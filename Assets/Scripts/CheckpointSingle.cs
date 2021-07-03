using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSingle : MonoBehaviour
{
    ThirdPersonCharacterController trackCheckpoints;
    // Start is called before the first frame update

    void Start()
    { 
        GameObject[] checks = GameObject.FindGameObjectsWithTag("checkpoint");

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider collision) //talk about displaytime first timer
                                                    //for nextLevel, which is the script attached to the end area, when the player enters the finish area and completes the level, finished is true
    {
        if (collision.gameObject.name=="Car")
        {
            //trackCheckpoints.PlayerThroughCheckpoint(this);
            
        }
    }
    //public void SetTrackCheckpoints(ThirdPersonCharacterController trackCheckpoints)
    //{
       // this.trackCheckpoints = trackCheckpoints;
    //}
}
