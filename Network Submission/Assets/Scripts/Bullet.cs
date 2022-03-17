using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public Rigidbody2D bulletBody;
    
    // Start is called before the first frame update
    void Start()
    {
        bulletBody.velocity = transform.right * speed;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Background")
            Object.Destroy(gameObject);

    }
}
