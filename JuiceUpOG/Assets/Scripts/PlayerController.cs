using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // PLEASE READ THE GUIDE BEFORE USING THE SCRIPT //

    [Header("Movement")]
    public float Speed = 450;
    public bool RotateToDirection = true; // Rotate To The Movement Direction
    public bool RotateWithMouseClick = false; // Rotate To The Direction Of The Mouse When Click , Usefull For Attacking

    [Header("Jumping")]
    public float JumpPower = 22; // How High The Player Can Jump
    public float Gravity = 6; // How Fast The Player Will Pulled Down To The Ground, 6 Feels Smooth
    public int AirJumps = 1; // Max Amount Of Air Jumps, Set It To 0 If You Dont Want To Jump In The Air
    public LayerMask groundLayer; // The Layers That Represent The Ground, Any Layer That You Want The Player To Be Able To Jump In

    [Header("Dashing")]
    public float DashPower = 3; // It Is A Speed Multiplyer, A Value Of 2 - 3 Is Recommended.
    public float DashDuration = 0.20f; // Duration Of The Dash In Seconds, Recommended 0.20f.
    public float DashCooldown = 0.5f; // Duration To Be Able To Dash Again.
    public bool AirDash = true; // Can Dash In Air ?

    [Header("Attacking")]
    public GameObject BulletPrefab;

    [Header("Trail")]
    public GameObject TrailPrefab; // The trail prefab to instantiate when dashing
    private GameObject trailInstance; // Reference to the instantiated trail object

    // Private Variables
    bool canMove = true;
    bool canDash = true;

    float MoveDirection;
    int currentJumps = 0;

    bool isShooting = false;

    float TimeLastKill;
    public bool bLoosingCombo = false;
    public float TimeLoose = 10f;
    float LastTimeLoose=0;
    public float ScrapComboTime = 10f;

    UIsScript UIRef;
    public int EnemyKilledCount;

    public bool Passed10Kills;
    public float FireRateOG = 0.5f;
    public float FireRate;

    
 
    Rigidbody2D rb;
    BoxCollider2D col; // Change It If You Use Something Else That Box Collider, Make Sure You Update The Reference In Start Function


    ////// START & UPDATE :

    void Start()
    {
        canMove = true;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.gravityScale = Gravity;

        TimeLoose = 10f;
        ScrapComboTime = 10f;
        FireRate = FireRateOG;
        UIRef = GameObject.Find("UIs Player").GetComponent<UIsScript>();

    }   
    void Update()
    {

        if (Time.time >= TimeLastKill + 10 && bLoosingCombo == false)
        {
            bLoosingCombo = true;
        }

         // si le combo est perdu et que c'est un combo supérieur à 1 il décrémente 
        //jusqu'au mini tant qu'on ramasse pas un nouveau scrap 
        if(bLoosingCombo && LastTimeLoose+TimeLoose<Time.time)
        {
            LastTimeLoose = Time.time;
            UIRef.SetCombo(EnemyKilledCount);
            //UIRef.ResetTimerDecrement();
        }
        

        // Get Player Movement Input
        MoveDirection = (Input.GetAxisRaw("Horizontal")); 
        // Rotation
        RotateToMoveDirection();

        // Rotate and Attack When Click Left Mouse Button
        if (Input.GetMouseButtonDown(0))
        {
            StartShooting();
            Debug.Log("Down");
        }

        // Stop shooting when the left mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            StopShooting();
            Debug.Log("Up");
        }

        // Rotate to mouse direction when shooting
        if (isShooting)
        {
            RotateToMouse();
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Dashing
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (MoveDirection != 0 && canDash)
            {
                if (!AirDash && !InTheGround())
                    return;


                StartCoroutine(Dash());
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneManager.LoadScene("OldScene");
        }
    }

    void StartShooting()
    {
        if (isShooting)
            return;

        isShooting = true;
        Debug.Log("is Shooting");
        InvokeRepeating("Attack", 0f, FireRate); // Adjust the delay between shots as needed
    }
    void StopShooting()
    {
        if (!isShooting)
            return;

        isShooting = false;
        CancelInvoke("Attack");
    }
    void FixedUpdate()
    {
        Move();
    } 

    ///// MOVEMENT FUNCTIONS :

    void Move()
    {
        if (canMove)
        {
            rb.velocity = new Vector2(MoveDirection * Speed * Time.fixedDeltaTime, rb.velocity.y);
        }

    } 
    bool InTheGround()
    {
        // Make sure you set the ground layer to the ground
        RaycastHit2D ray;

         if (transform.rotation.y == 0)
         {
            Vector2 position = new Vector2(col.bounds.center.x - col.bounds.extents.x, col.bounds.min.y);
             ray = Physics2D.Raycast(position, Vector2.down, col.bounds.extents.y + 0.2f, groundLayer);
         }
         else
         {
            Vector2 position = new Vector2(col.bounds.center.x + col.bounds.extents.x, col.bounds.min.y);
            ray = Physics2D.Raycast(position, Vector2.down, col.bounds.extents.y + 0.2f, groundLayer);
         }       

        if (ray.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void Jump()
    {

        if (InTheGround())
        {
            rb.velocity = Vector2.up * JumpPower;
        }
        else
        {
            if (currentJumps >= AirJumps)
                return;

            currentJumps ++;
            rb.velocity = Vector2.up * JumpPower;
        }

    }

    void Attack()
    {
        Instantiate(BulletPrefab, transform.position, transform.rotation);
    }

    void RotateToMoveDirection()
    {
        if (!RotateToDirection)
            return;

        if (MoveDirection != 0 && canMove)
        {
            if (MoveDirection > 0)
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
                
            }
            else
            {
                transform.rotation = new Quaternion(0, 180, 0, 0);
            }
        }
    }

    ///// SPECIAL  FUNCTIONS : 

    // Multiply The Speed With Certain Amount For A Certain Duration
    IEnumerator Dash()
    {
        canDash = false;
        float originalSpeed = Speed; 
       
        Speed *= DashPower;
        rb.gravityScale = 0f; // You can delete this line if you don't want the player to freez in the air when dashing
        rb.velocity = new Vector2(rb.velocity.x, 0);

        trailInstance = Instantiate(TrailPrefab, transform.position, Quaternion.identity); // Instantiate the trail prefab
        trailInstance.transform.parent = transform; // Set the trail's parent to the player to follow its movement

        //  You Can Add A Camera Shake Function here

        yield return new WaitForSeconds(DashDuration); 

        rb.gravityScale = Gravity;
        Speed = originalSpeed;

        Destroy(trailInstance); // Destroy the trail object

        yield return new WaitForSeconds(DashCooldown - DashDuration);

        canDash = true;
    }

    // Make Player Facing The Mouse Cursor , Can Be Called On Update , Or When The Player Attacks He Will Turn To The Mouse Direction
    void RotateToMouse()
    {
        if (!RotateWithMouseClick)
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        Vector2 myPos = transform.position;

        Vector2 dir = mousePos - myPos;  

        if (dir.x < 0)
        {
            transform.rotation = new Quaternion(0, 180, 0, 0);
        }
        else
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }

    // Reset Jump Counts When Collide With The Ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            RaycastHit2D ray;
            ray = Physics2D.Raycast(col.bounds.center, Vector2.down, col.bounds.extents.y + 0.2f, groundLayer);

            if (ray.collider != null)
            {
                currentJumps = 0;
            }

        }
    }

    public void EnemyKilled()
    {
        bLoosingCombo = false;
        UIRef.ActivateComboUI();
        UIRef.StartCount();
        // UIRef.test = true;
        UIRef.ComboAnimStart("+1");
        TimeLastKill = Time.time;
        EnemyKilledCount++;
        UIRef.SetCombo(EnemyKilledCount);

    }

    public void UpgradeBulletsLVL1()
    {
        FireRate = 0.3f;
        if (isShooting == true)
        {
            StartCoroutine(StartShootingAgain());
        }
    }

    public void UpgradeBulletsLVL2()
    {
        FireRate = 0.1f;
        Speed = 600;
        if (isShooting == true)
        {
            StartCoroutine(StartShootingAgain());
        }
    }

    public void UpgradeBulletsLVL3()
    {
        Passed10Kills = true;
        canDash = false;
        if (isShooting == true)
        {
            StartCoroutine(StartShootingAgain());
        }
    }

    public void ResetCombo()
    {
        EnemyKilledCount = 0;
        Passed10Kills = false;
        FireRate = FireRateOG;
        Speed = 450;
        canDash = true;
    }

    IEnumerator StartShootingAgain()
    {
        Debug.Log("Is Shooting, Making him stop");
        StopShooting();
        yield return new WaitForSeconds(FireRate);  
        StartShooting();
        Debug.Log("Stopped shooting, Making him shoot again");
    }
}
