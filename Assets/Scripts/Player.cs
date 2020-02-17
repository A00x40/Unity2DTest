using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float JumpVelocity;
    public Vector2 velocity;
    public float Gravity;

    public bool bounce = false;
    public float BounceVelocity = 3;

    private bool walk , walkL ,walkR , jump;
    
    public enum state
    {
        Idle,
        Walking,
        Jumping,
        Bouncing,
        Dead
    }
    private state playerState = state.Idle;
    private bool Grounded = false;

    public LayerMask wallMask;
    public LayerMask FloorMask;

   
    // Start is called before the first frame update
    void Start()
    {
        Fall();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState != state.Dead)
        {
            CheckPlayerInput();
            UpdatePlayerPos();
            UpdateAnimation();
        }
    }
    void UpdateAnimation()
    {
        if (Grounded && !walk)
        {
            GetComponent<Animator>().SetBool("isRunning", false);
            GetComponent<Animator>().SetBool("isJumping", false);
        }
        if (Grounded && walk)
        {
            GetComponent<Animator>().SetBool("isRunning", true);
            GetComponent<Animator>().SetBool("isJumping", false);
        }
        if (playerState == state.Jumping)
        {
            GetComponent<Animator>().SetBool("isRunning", false);
            GetComponent<Animator>().SetBool("isJumping", true);
        }
    }
    void CheckPlayerInput()
    {
       bool Jump =  Input.GetKey(KeyCode.Space);
       bool moveL = Input.GetKey(KeyCode.LeftArrow);
       bool moveR = Input.GetKey(KeyCode.RightArrow);
       bool Fire = Input.GetKey(KeyCode.S);

       walk = moveL || moveR;
       walkL = moveL && !moveR;
       walkR = moveR && !moveL;
       jump = Jump ;
    }
    void UpdatePlayerPos()
    {
        Vector3 pos = transform.localPosition;
        Vector3 dir = transform.localScale;
        if(walk)
        {
            if(walkL)
            {
                dir.x = -1;
                pos.x -= velocity.x * Time.deltaTime;
            }
            else if(walkR)
            {
                dir.x = 1;
                pos.x += velocity.x * Time.deltaTime;
            }
            pos = CheckWall(pos, dir.x);
        }
        if(jump && playerState != state.Jumping)
        {
            playerState = state.Jumping;
            velocity = new Vector2(velocity.x, JumpVelocity);
           
        }

        if(playerState == state.Jumping)
        {
            pos.y += velocity.y * Time.deltaTime;
            velocity.y -= Gravity * Time.deltaTime;
            
        }
        if(velocity.y <= 0)
        {
            pos = CheckFloor(pos);
        }
        else
        {
            pos = CheckCeiling(pos);
        }
        if(bounce && playerState != state.Bouncing)
        {
            playerState = state.Bouncing;
            velocity = new Vector2(velocity.x,BounceVelocity);
        }
        if (playerState == state.Bouncing)
        {
            pos.y += velocity.y * Time.deltaTime;
            velocity.y -= Gravity * Time.deltaTime;
        }
        transform.localPosition = pos;
        transform.localScale = dir;
    }
    
    Vector3 CheckWall(Vector3 pos,float dir)
    {
        Vector2 Top = new Vector2(pos.x + dir * .4f, pos.y + 1.0f - 0.2f);
        Vector2 Bottom = new Vector2(pos.x + dir * .4f, pos.y - 1.0f + 0.2f);
        Vector2 middle = new Vector2(pos.x + dir * .4f, pos.y);

        RaycastHit2D wallTop = Physics2D.Raycast(Top, new Vector2(dir,0), velocity.x * Time.deltaTime , wallMask);
        RaycastHit2D wallBottom = Physics2D.Raycast(Bottom, new Vector2(dir, 0), velocity.x * Time.deltaTime, wallMask);
        RaycastHit2D wallMiddle = Physics2D.Raycast(middle, new Vector2(dir, 0), velocity.x * Time.deltaTime, wallMask);

        if(wallTop.collider != null || wallBottom.collider != null || wallMiddle.collider != null)
        {
           
            RaycastHit2D wallHit = wallTop;

            if (wallBottom)
            {
                wallHit = wallBottom;
            }
            else if (wallMiddle)
            {
                wallHit = wallMiddle;
            }
            else if (wallTop)
            {
                wallHit = wallTop;
            }
            if (wallHit.collider.CompareTag("Enemy"))
            {
                playerState = state.Dead;
            }
            pos.x -= velocity.x * Time.deltaTime * dir;
        }
        return pos;
    }
    Vector3 CheckFloor(Vector3 pos)
    {
        Vector2 Left = new Vector2(pos.x - 0.5f + 0.2f, pos.y - 1f );
        Vector2 Right = new Vector2(pos.x + 0.5f - 0.2f, pos.y - 1f );
        Vector2 middle = new Vector2(pos.x , pos.y - 1f );

        RaycastHit2D FloorLeft = Physics2D.Raycast(Left, Vector2.down, velocity.y * Time.deltaTime, FloorMask);
        RaycastHit2D FloorRight = Physics2D.Raycast(Right, Vector2.down, velocity.y * Time.deltaTime, FloorMask);
        RaycastHit2D FloorMiddle = Physics2D.Raycast(middle, Vector2.down, velocity.y * Time.deltaTime, FloorMask);

        if (FloorLeft.collider != null || FloorRight.collider != null || FloorMiddle.collider != null)
        {
            RaycastHit2D FloorHit = FloorRight;

            if(FloorLeft)
            {
                FloorHit = FloorLeft;
            }
            else if(FloorMiddle)
            {
                FloorHit = FloorMiddle;
            }
            else if(FloorRight)
            {
                FloorHit = FloorRight;
            }

            if(FloorHit.collider.CompareTag("Enemy"))
            {
                FloorHit.collider.GetComponent<Enemy>().Crush();
                bounce = true;
            }
            playerState = state.Idle;
            Grounded = true;
            velocity.y = 0 ;

            pos.y = FloorHit.collider.bounds.center.y + FloorHit.collider.bounds.size.y / 2 + 1;
        }
        else
        {
            if (playerState != state.Jumping)
            {
                Fall();
            }
        }
        return pos;
    }
    Vector3 CheckCeiling(Vector3 pos)
    {
        Vector2 Left = new Vector2(pos.x - 0.5f + 0.2f, pos.y + 1f);
        Vector2 Right = new Vector2(pos.x + 0.5f - 0.2f, pos.y + 1f);
        Vector2 middle = new Vector2(pos.x, pos.y + 1f);

        RaycastHit2D CeilLeft = Physics2D.Raycast(Left, Vector2.up, velocity.y * Time.deltaTime, FloorMask);
        RaycastHit2D CeilRight = Physics2D.Raycast(Right, Vector2.up, velocity.y * Time.deltaTime, FloorMask);
        RaycastHit2D CeilMiddle = Physics2D.Raycast(middle, Vector2.up, velocity.y * Time.deltaTime, FloorMask);

        if (CeilLeft.collider != null || CeilRight.collider != null || CeilMiddle.collider != null)
        {
            RaycastHit2D CeilHit = CeilRight;

            if (CeilLeft)
            {
                CeilHit = CeilLeft;
            }
            else if (CeilMiddle)
            {
                CeilHit = CeilMiddle;
            }
            else if (CeilRight)
            {
                CeilHit = CeilRight;
            }
            if (CeilHit.collider.CompareTag("Enemy"))
            {
                playerState = state.Dead;
            }
            pos.y = CeilHit.collider.bounds.center.y - CeilHit.collider.bounds.size.y / 2 - 1f;

            Fall();
        }
        return pos;
    }
    
    void Fall()
    {
        velocity.y = 0;
        playerState = state.Jumping;
        Grounded = false;
        bounce = false;
    }
}
