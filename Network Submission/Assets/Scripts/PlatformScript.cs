using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    //Variables

    //Patrol points
    public GameObject movePointsObj;
    public List<Transform> movePoints;

    int currentTarget = 0;

    //Move Speed
    public float speed = 2f;

    // Update is called once per frame
    private void Update()
    {

    }

    //void FixedUpdate()
    //{
    //    MoveToPoint();
    //}

    ////Move
    //void MoveToPoint()
    //{
    //    //Get the transform of the target point
    //    Transform targetPoint = movePoints[currentTarget];

    //    //Move the platform towards the next point
    //    transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime); //speed * Time.deltaTime ensures speed is constant over time

    //    //Check if platform has reached target (or close enough) and change direction
    //    if (Vector2.Distance(transform.position, targetPoint.position) < 0.8f)
    //    {
    //        //Iterate current target
    //        currentTarget++;

    //        //reset current target if out of bounds
    //        if (currentTarget > movePoints.Count - 1)
    //            currentTarget = 0;
    //    }
    //}
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        collision.transform.parent = transform;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Player")
    //    {
    //        collision.transform.parent = null;
    //    }
    //}
}
