using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SIDES { Left = -3, Mid = 0, Right = 3 }

public class AiController : MonoBehaviour
{

    [SerializeField] private float JumpHeight;
     public float runSpeed = 7f;

    public CharacterController characterControllerAi;
    private Animator animator;
    public CharacterControllers rotchar;
    public SIDES m_Side = SIDES.Mid;

    public bool SwipeLeft, SwipeRight, SwipeDown, SwipeUp;
    public Transform Agent;
    public Canvas GameOver;
    public Canvas Completed;
    public float x;
    private float y;
    public bool InRoll;
    public bool StopAllState = false;
    public bool InJump;
    private float ColHeight;
    private float ColCenterY;
    public float Speed;
    public HitX hitX = HitX.None;
    public HitY hitY = HitY.None;
    public HitZ hitZ = HitZ.None;
    private SIDES LastSide;
    public Transform rot;
    // Start is called before the first frame update
    void Start()
    {
        characterControllerAi = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        ColHeight = characterControllerAi.height;
        ColCenterY = characterControllerAi.center.y;
        rotchar = GameObject.FindWithTag("Player").GetComponent<CharacterControllers>();

    } // Update is called once per frame
    void Update()
    {

        Swipe();
        Catched();
        Jump();

        if (runSpeed == 0)
        {
            gameObject.transform.rotation = rot.transform.rotation;
        }
        else
        {
            gameObject.transform.rotation = rotchar.transform.rotation;
        }

        animator.SetFloat("runSpeed", runSpeed);

        if (SwipeDown || SwipeLeft || SwipeRight || SwipeUp)
        {
            runSpeed = 10f;
        }
    }
    void Jump()
    {
        if (characterControllerAi.isGrounded)
        {
            if (SwipeUp)
            {
                y = JumpHeight;
                animator.CrossFadeInFixedTime("Jumping", 0.1f);
                InJump = true;
                SwipeUp = false;
            }
        }
        else
        {
            y -= JumpHeight * 2 * Time.deltaTime;
            if (characterControllerAi.velocity.y < -0.1f)
                PlayAnimation("Running");
        }
    }

    void Swipe()
    {

      /* SwipeLeft = Input.GetKeyDown(KeyCode.A);
        SwipeRight = Input.GetKeyDown(KeyCode.D);
        SwipeDown = Input.GetKeyDown(KeyCode.S);
        SwipeUp = Input.GetKeyDown(KeyCode.Space);*/

        if (SwipeLeft)
        {
            if (m_Side == SIDES.Mid)
            {
                
                LastSide = m_Side;
                m_Side = SIDES.Left;
                PlayAnimation("RighStep");
                SwipeLeft = false;

            }
            else if (m_Side == SIDES.Right)
            {
              
                LastSide = m_Side;
                m_Side = SIDES.Mid;
                PlayAnimation("RighStep");
                SwipeLeft = false;
            }


        }
        else if (SwipeRight)
        {
            if (m_Side == SIDES.Mid)
            {
                LastSide = m_Side;
                m_Side = SIDES.Right;
                PlayAnimation("LeftStep");
                SwipeRight = false;
            }
            else if (m_Side == SIDES.Left)
            {
                LastSide = m_Side;
                m_Side = SIDES.Mid;
                PlayAnimation("LeftStep");
                SwipeRight = false;
            }

        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            StopAllState = false;
        }
        x = Mathf.Lerp(x, (int)m_Side, Time.deltaTime * Speed);
        Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, runSpeed * Time.deltaTime);
        characterControllerAi.Move(moveVector);
        Slide();



    }

    internal float RollCounter;

    void Slide()
    {
        RollCounter -= Time.deltaTime;
        if (RollCounter <= 0)
        {
            RollCounter = 0f;
            characterControllerAi.center = new Vector3(0, ColCenterY, 0);
            characterControllerAi.height = ColHeight;
            InRoll = false;
        }
        if (SwipeDown)
        {
           
            RollCounter = 0.7f;
            y -= 15f;
            characterControllerAi.center = new Vector3(0, ColCenterY / 3f, 0);
            characterControllerAi.height = ColHeight / 2f;
            animator.CrossFadeInFixedTime("Sliding", 0.7f);
            InRoll = true; ;
            SwipeDown = false;
        }
    }
    public void PlayAnimation(string anim)
    {
        if (StopAllState) return;
        animator.Play(anim);
    }
    public void Stumble(string anim)
    {
        animator.ForceStateNormalizedTime(0.0f);
        StopAllState = true;
        animator.Play(anim);
    }
    private void ResetCollision()
    {
        hitX = HitX.None;
        hitY = HitY.None;
        hitZ = HitZ.None;
    }
    public void OnCharacterColliderHit(Collider col)
    {

        hitX = GetHitX(col);
        hitY = GetHitY(col);
        hitZ = GetHitZ(col);
        if (col.tag != "Ground")
        {
            if (hitY == HitY.Down && hitX == HitX.Mid)
            {

                PlayAnimation("Fall");
                runSpeed = 0;
                ResetCollision();
            }
            else if (hitY == HitY.Up && hitX == HitX.Mid)
            {

                PlayAnimation("Backwards");
                runSpeed = 0f;
                ResetCollision();
            }
            else if (hitY == HitY.Up || hitY == HitY.Mid && hitX == HitX.Left)
            {

                PlayAnimation("ShoulderL");
                runSpeed = 0f;
                ResetCollision();
            }

            else if (hitY == HitY.Up || hitY == HitY.Mid && hitX == HitX.Right)
            {

                PlayAnimation("ShoulderR");
                runSpeed = 0f;
                ResetCollision();
            }
        }


    }
    private void OnTriggerEnter(Collider way)
    {
        if (way.tag == "Slide")
        {
            
            SwipeDown = true;
        }
        if (way.tag == "Left")
        {
            SwipeLeft = true;
        }
        if (way.tag == "Right")
        {
            SwipeRight = true;
        }
        if (way.tag == "Up")
        {
            SwipeUp = true;
        }

        if(way.tag == "Finish")
        {
            Debug.Log("girdi");
            GameOver.gameObject.SetActive(true);
        }
        if(way.tag == "Ball")
        {
            animator.Play("Catched");
            Completed.gameObject.SetActive(true);
        }
    }
    void Catched()
    {
        float distance = transform.position.z - Agent.transform.position.z;
       
         if (distance<3)
        {

            animator.Play("Catched");
            runSpeed = 0.1f;
            

        }
    }
    public void AdjustSpeed(float newSpeed)
    {
        runSpeed = newSpeed;
    }
    public HitX GetHitX(Collider col)
    {
        Bounds char_bounds = characterControllerAi.bounds;
        Bounds col_bounds = col.bounds;
        float min_x = Mathf.Max(col.bounds.min.x, char_bounds.min.x);
        float max_x = Mathf.Min(col.bounds.max.x, char_bounds.max.x);

        float average = (min_x + max_x) / 2 - col.bounds.min.x;

        HitX hit;
        if (average > col_bounds.size.x - 0.33f)
        {
            hit = HitX.Right;
        }
        else if (average < 0.33f)
        {
            hit = HitX.Left;
        }
        else
        {
            hit = HitX.Mid;
        }

        return hit;
    }

    public HitY GetHitY(Collider col)
    {

        Bounds char_bounds = characterControllerAi.bounds;
        Bounds col_bounds = col.bounds;
        float min_y = Mathf.Max(col.bounds.min.y, char_bounds.min.y);
        float max_y = Mathf.Min(col.bounds.max.y, char_bounds.max.y);

        float average = ((min_y + max_y) / 2 - char_bounds.min.y) / char_bounds.size.y;

        HitY hit;

        if (average < 0.17f)
        {
            hit = HitY.Low;
        }
        else if (average < 0.33f)
        {
            hit = HitY.Down;
        }
        else if (average < 0.66f)
        {
            hit = HitY.Mid;
        }
        else
        {
            hit = HitY.Up;
        }
        return hit;


    }

    public HitZ GetHitZ(Collider col)
    {
        Bounds char_bounds = characterControllerAi.bounds;
        Bounds col_bounds = col.bounds;
        float min_z = Mathf.Max(col.bounds.min.z, char_bounds.min.z);
        float max_z = Mathf.Min(col.bounds.max.z, char_bounds.max.z);

        float average = ((min_z + max_z) / 2 - char_bounds.min.z) / char_bounds.size.z;

        HitZ hit;

        if (average < col_bounds.size.z - 0.33f)
        {
            hit = HitZ.Backward;
        }
        else if (average < 0.66f)
        {
            hit = HitZ.Mid;
        }
        else
        {
            hit = HitZ.Forward;
        }

        return hit;
    }
}
    