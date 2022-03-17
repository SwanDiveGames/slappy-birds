using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class Attack : NetworkBehaviour
{
    //Reference to Animator
    public Animator animator;

    //Point for attacks to activate
    public Transform attackPoint;

    //Range from attack point hits are detected
    public float attackRange = 0.2f;

    //Variables for setting Attack Rate
    public float attackRate = 2f;
    float nextAttackTime = 0f;

    //LayerMask for players so hits are detected
    public LayerMask playerLayer;

    //X and Y launch speeds for hitting players
    public float xLaunch = 100;
    public float yLaunch = 400;

    //Check if player is facing left or right
    public bool playerFacingRight = true;

    //Check if enemy is facing right
    public bool enemyFacingRight = true;

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            //Check how long it's been since the player last attacked. If long enough, can attack again.
            if (Time.time >= nextAttackTime)
            {
                if (Input.GetButtonDown("Attack"))
                {
                    HitPlayer();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
            
            //Check player rotation
            if (GetComponent<Transform>().localScale.x < 0)
                playerFacingRight = false;
            else
                playerFacingRight = true;
        }
        
        
    }

    void HitPlayer()
    {
        //Attack Animation
        animator.SetTrigger("Attack");

        //See if other players are in range
        //Creates an array of enemy colliders in the attack radius at the time the player attacked
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        //Stun Players
        //For all players in the attack radius at the time of attack, do the following
        foreach (Collider2D enemy in hitEnemies)
        {
            //Knock player away
            enemy.GetComponent<Attack>().TakeHitFromPlayer(playerFacingRight);
        }
    }

    //Hit by Player
    public void TakeHitFromPlayer(bool facingRight)
    {
        if (facingRight)
            GetComponent<CharacterController2D>().m_Rigidbody2D.AddForce(new Vector2(xLaunch, yLaunch));
        else if (!facingRight)
            GetComponent<CharacterController2D>().m_Rigidbody2D.AddForce(new Vector2(-xLaunch, yLaunch));
    }

    private void checkRight(bool checkRight)
    {
        if (playerFacingRight)
            checkRight = true;
        else
            checkRight = false;
    }

    //View attackRange in scene editor
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
