using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finish : MonoBehaviour
{
    GameObject[] checks;
    
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider collision) //talk about displaytime first timer
                                                    //for nextLevel, which is the script attached to the end area, when the player enters the finish area and completes the level, finished is true
    {
        if (collision.gameObject.name == "Car")
        {

        }
    }

}
