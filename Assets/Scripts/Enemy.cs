using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
   public Vector2 velocity;
    public float Gravity;

    public bool Rightdirection = false;
    public bool Grounded = false;

    public enum state
    {
        Walking,
        Falling,
        dead
    }
    state EnemyState = state.Falling;

    public LayerMask FloorMask,WallMask;

    private bool EnemyDie = false;
    public float DeathTime = 0.25f;

    public float TimeOfDeath = 0;
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
        Fall();
    }

    // Update is called once per frame
    void Update()
    {
         UpdateEnemyPos();
         checkCrushed();
    }
    private void OnBecameVisible()
    {
         enabled = true;
    }

    void UpdateEnemyPos()
    {
        if (EnemyState != state.dead)
        {
            Vector3 pos = transform.localPosition;
            Vector3 Scale = transform.localScale;
            if(EnemyState == state.Falling)
            {
                pos.y += velocity.y * Time.deltaTime;
                velocity.y -= Gravity * Time.deltaTime;
            }
            
            if (EnemyState == state.Walking)
            {
                if(Rightdirection == false)
                {
                    Scale.x = -1;
                    pos.x -= velocity.x * Time.deltaTime;
                }
                else
                {
                    pos.x += velocity.x * Time.deltaTime;
                    Scale.x = 1;
                }
                
            }

            if (velocity.y <= 0)
            {
                pos = CheckFloor(pos);
            }
            pos = CheckWall(pos, Scale.x);

            transform.localPosition = pos;
            transform.localScale = Scale;
        }
    }
    Vector3 CheckFloor(Vector3 pos)
    {
        Vector2 Left = new Vector2(pos.x - 0.5f + 0.2f, pos.y - 0.5f);
        Vector2 Right = new Vector2(pos.x + 0.5f - 0.2f, pos.y - 0.5f);
        Vector2 middle = new Vector2(pos.x, pos.y - 0.5f);

        RaycastHit2D FloorLeft = Physics2D.Raycast(Left, Vector2.down, velocity.y * Time.deltaTime, FloorMask);
        RaycastHit2D FloorRight = Physics2D.Raycast(Right, Vector2.down, velocity.y * Time.deltaTime, FloorMask);
        RaycastHit2D FloorMiddle = Physics2D.Raycast(middle, Vector2.down, velocity.y * Time.deltaTime, FloorMask);

        if (FloorLeft.collider != null || FloorRight.collider != null || FloorMiddle.collider != null)
        {
            RaycastHit2D FloorHit = FloorRight;

            if (FloorLeft)
            {
                FloorHit = FloorLeft;
            }
            else if (FloorMiddle)
            {
                FloorHit = FloorMiddle;
            }
            else if (FloorRight)
            {
                FloorHit = FloorRight;
            }
            if(FloorHit.collider.CompareTag("Player"))
            {
                Application.LoadLevel("End");
            }
            EnemyState = state.Walking;
            Grounded = true;
            velocity.y = 0;

            pos.y = FloorHit.collider.bounds.center.y + FloorHit.collider.bounds.size.y / 2 + 0.5f;
        }
        else
        {
            if(EnemyState != state.Falling)
            {
                Fall();
            }
        }
        return pos;
    }
    Vector3 CheckWall(Vector3 pos, float dir)
    {
        Vector2 Top = new Vector2(pos.x + dir * .4f, pos.y + 0.5f - 0.2f);
        Vector2 Bottom = new Vector2(pos.x + dir * .4f, pos.y - 0.5f + 0.2f);
        Vector2 middle = new Vector2(pos.x + dir * .4f, pos.y);

        RaycastHit2D wallTop = Physics2D.Raycast(Top, new Vector2(dir, 0), velocity.x * Time.deltaTime, WallMask);
        RaycastHit2D wallBottom = Physics2D.Raycast(Bottom, new Vector2(dir, 0), velocity.x * Time.deltaTime, WallMask);
        RaycastHit2D wallMiddle = Physics2D.Raycast(middle, new Vector2(dir, 0), velocity.x * Time.deltaTime, WallMask);

        if (wallTop.collider != null || wallBottom.collider != null || wallMiddle.collider != null)
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
            if (wallHit.collider.CompareTag("Player") )
            {
                Application.LoadLevel("End");
            }
            Rightdirection = !Rightdirection;   
        }
        return pos;
    }
    public void Crush()
    {
        EnemyState = state.dead;
        GetComponent<Animator>().SetBool("isCrushed", true);
        GetComponent<Collider2D>().enabled = false;
        EnemyDie = true;
    }
    private void checkCrushed()
    {
        if(EnemyDie)
        {
            if (TimeOfDeath <= DeathTime)
            {
                TimeOfDeath += Time.deltaTime;
            }
            else
            {
                EnemyDie = false;
                Destroy(this.gameObject);
            }
        }
    }
    void Fall()
    {
        velocity.y = 0;
        EnemyState = state.Falling;
        Grounded = false;
    }
    
}
