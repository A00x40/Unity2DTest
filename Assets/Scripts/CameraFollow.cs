using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform TargetPlayer;
    public Transform Leftbound;
    public Transform Rightbound;

    private Vector3 SmoothDampVelocity = Vector3.zero;
    public float SmoothDampTime = 0.15f;

    public float camWidth, camHeight , minLevel , maxLevel;
    // Start is called before the first frame update

 
    void Start()
    {
        camHeight = Camera.main.orthographicSize * 2;
        camWidth = camHeight * Camera.main.aspect;
        float Leftboundswidth = Leftbound.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;
        float Rightboundswidth = Rightbound.GetComponentInChildren<SpriteRenderer>().bounds.size.x / 2;

        minLevel = Leftbound.position.x + Leftboundswidth + camWidth/2;
        maxLevel = Rightbound.position.x - Rightboundswidth - camWidth/2;
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetPlayer == true)
        {
            float Target = Mathf.Max(minLevel, Mathf.Min(maxLevel,TargetPlayer.transform.position.x));
            float xpos = Mathf.SmoothDamp(transform.position.x, Target,ref SmoothDampVelocity.x, SmoothDampTime);

            transform.position = new Vector3(xpos, transform.position.y, transform.position.z);
       
        }
    }
}
