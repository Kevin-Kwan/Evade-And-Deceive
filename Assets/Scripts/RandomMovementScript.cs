using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovementScript : MonoBehaviour
{
    private Vector2 _newPos;
    private Quaternion _newRot;
    public Vector2 min;
    public Vector2 max;
    public Vector2 yRotationRange;
    public float lerpSpeed = 0.05f;
    
    private void Awake()
    {
        _newPos = transform.position;
        _newRot = transform.rotation;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _newPos, Time.deltaTime * lerpSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, _newRot, Time.deltaTime * lerpSpeed);

       
    }

    void GetNewPos()
    {
        var xPos = Random.Range(min.x, min.y);
        var zPos = Random.Range(min.y, min.y);
        _newRot = Quaternion.Euler(0, Random.Range(yRotationRange.x, yRotationRange.y), 0);
        _newPos = new Vector3(xPos, 0, zPos);
    }
}
