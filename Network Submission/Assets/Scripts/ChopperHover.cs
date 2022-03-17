using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopperHover : MonoBehaviour
{
    //Hover Counter
    int count = 0;

    //Hover move amount
    float hoverMove = 10f;

    //Hover Variables
    //Hover points
    public GameObject chopperHoverPointsObj;
    public List<Transform> hoverPoints;

    int currentTarget = 0;

    float speed = 0.5f;
    float highSpeed = 3f;
    public float chopperSpeed;

    bool isArrived = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Hover();
    }

    //Hover
    //Move
    void Hover()
    {
        //Get the transform of the target point
        Transform targetPoint = hoverPoints[currentTarget];

        //Set speed for start of level
        if (!isArrived)
            chopperSpeed = highSpeed;
        else
            chopperSpeed = speed;

        //Move the chopper towards the next point
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, chopperSpeed * Time.deltaTime); //speed * Time.deltaTime ensures speed is constant over time

        //Check if chopper has reached target (or close enough) and change direction
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            //Iterate current target
            currentTarget++;

            //reset current target if out of bounds
            if (currentTarget > hoverPoints.Count - 1)
                currentTarget = 0;

            isArrived = true;
        }
    }
}
