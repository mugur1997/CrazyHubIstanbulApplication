using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SIDE { Left=-3, Mid=0 ,Right=3}
public enum HitX {Left,Mid,Right,None }
public enum HitY { Up, Mid, Down,Low, None }
public enum HitZ { Forward, Mid, Backward, None }

public class CharacterControllers : MonoBehaviour
{

    [SerializeField] private float JumpHeight = 2f;
     public float runSpeed = 7f;
    public AudioSource audio;
    public CharacterController characterController;
    private Animator animator;
    public Transform AI;
    public Decision dec;
    public SIDE m_Side = SIDE.Mid;
    public Canvas Fire;
    public bool SwipeLeft, SwipeRight, SwipeDown, SwipeUp;
    public bool LeftFire = false;
    public bool RightFire = false;
    public Canvas panel;
    public Canvas Completed;
    
    public float x;
    private float y;

    public Transform door;
  
    public Transform Pos;

    public static bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    private bool isDraging = false;
    private Vector2 startTouch, swipeDelta;

    public bool InRoll;
    public bool StopAllState = false;
    public bool InJump;
    private float ColHeight;
    private float ColCenterY;
    public float Speed;
    public HitX hitX = HitX.None;
    public HitY hitY = HitY.None;
    public HitZ hitZ = HitZ.None;
    private SIDE LastSide;
    // Start is called before the first frame update
    void Start()
    {

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        ColHeight = characterController.height;
        ColCenterY = characterController.center.y;
        dec = GameObject.FindWithTag("MainCamera").GetComponent<Decision>();
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
       
      
        Catch();
        Swipe();
        SwipeInput();
        Jump();
        if (runSpeed > 0)
        {

           
            animator.SetFloat("runSpeed", runSpeed);
        }

     
        if (SwipeDown || SwipeLeft || SwipeRight || SwipeUp)
        {
            runSpeed = 30f;
        }

    }
    void Jump()
    {
        if (characterController.isGrounded)
        {
            if (swipeUp)
            {
                y = JumpHeight;
                animator.CrossFadeInFixedTime("Jumping", 0.1f);
                InJump = true;
            }
        }
        else
        {
            y -= JumpHeight * 2 * Time.deltaTime;
            if (characterController.velocity.y < -0.1f)
                PlayAnimation("Running");
        }
    }

    void Catch()
    {
        float distance = AI.transform.position.z - transform.position.z;
;        if(distance < 3)
        {
           
            animator.Play("Catch");
            runSpeed = 0;
            Completed.gameObject.SetActive(true);
            
        }
    }

    void SwipeInput()
    {

        tap = swipeDown = swipeUp = swipeLeft = swipeRight = false;
        #region Standalone Inputs
        if (Input.GetMouseButtonDown(0))
        {
            tap = true;
            isDraging = true;
            startTouch = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDraging = false;
            Reset();
        }
        #endregion

        #region Mobile Input
        if (Input.touches.Length > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                tap = true;
                isDraging = true;
                startTouch = Input.touches[0].position;
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                isDraging = false;
                Reset();
            }
        }
        #endregion

        //Calculate the distance
        swipeDelta = Vector2.zero;
        if (isDraging)
        {
            if (Input.touches.Length < 0)
                swipeDelta = Input.touches[0].position - startTouch;
            else if (Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startTouch;
        }

        //Did we cross the distance?
        if (swipeDelta.magnitude > 100)
        {
            //Which direction?
            float x = swipeDelta.x;
            float y = swipeDelta.y;
            if (Mathf.Abs(x) > Mathf.Abs(y))
            {
                //Left or Right
                if (x < 0)
                    swipeLeft = true;
                else
                    swipeRight = true;
            }
            else
            {
                //Up or Down
                if (y < 0)
                    swipeDown = true;
                else
                    swipeUp = true;
            }

            Reset();
        }
    }
    private void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }
    void Swipe()
    {

       /* SwipeLeft = Input.GetKeyDown(KeyCode.A);
        SwipeRight = Input.GetKeyDown(KeyCode.D);
        SwipeDown = Input.GetKeyDown(KeyCode.S);
        SwipeUp = Input.GetKeyDown(KeyCode.Space);*/

        if (swipeLeft)
        {
            if (m_Side == SIDE.Mid)
            {
                LastSide = m_Side;
                m_Side = SIDE.Left;
                PlayAnimation("RighStep");

            }
            else if (m_Side == SIDE.Right)
            {
                LastSide = m_Side;
                m_Side = SIDE.Mid;
                PlayAnimation("RighStep");
            }


        }
        else if (swipeRight)
        {
            if (m_Side == SIDE.Mid)
            {
                LastSide = m_Side;
                m_Side = SIDE.Right;
                PlayAnimation("LeftStep");
            }
            else if (m_Side == SIDE.Left)
            {
                LastSide = m_Side;
                m_Side = SIDE.Mid;
                PlayAnimation("LeftStep");
            }

        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            StopAllState = false;
        }
        x = Mathf.Lerp(x, (int)m_Side, Time.deltaTime * Speed);
        Vector3 moveVector = new Vector3(x - transform.position.x, y * Time.deltaTime, runSpeed * Time.deltaTime);
        characterController.Move(moveVector);
        Slide();



    }
    
    internal float RollCounter;

    private void OnTriggerEnter(Collider decision)
    {
        if (decision.tag == "Decision")
        {
            runSpeed = 0;
            panel.gameObject.SetActive(true);
            audio.volume = 0f;
            dec.indexs = 0;
        }
        if (decision.tag == "Start")
        {
            door.transform.position = Vector3.Lerp(door.transform.position, Pos.transform.position, Time.deltaTime);
        }

        if (decision.gameObject.tag == "RightFire")
        {
            Fire.gameObject.SetActive(true);
            
            
            RightFire = true;
        }
        if (decision.gameObject.tag == "LeftFire")
        {
            Fire.gameObject.SetActive(true);
           
            LeftFire = true;
        }
        else if (decision.gameObject.tag == "Enable")
        {
            runSpeed = 30f;
            Fire.gameObject.SetActive(false);
        }
               
           
        
    }
    public void AdjustSpeed(float newSpeed) 
    {
        runSpeed = newSpeed;
    }
    void Slide()
    {
        RollCounter -= Time.deltaTime;
        if (RollCounter <= 0)
        {
            RollCounter = 0f;
            characterController.center = new Vector3(0, ColCenterY, 0);
            characterController.height = ColHeight;
            InRoll = false;
        }
        if (swipeDown)
        {
            RollCounter = 0.6f;
            y -= 12f;
            characterController.center = new Vector3(0, ColCenterY/3f, 0);
            characterController.height = ColHeight/2f;
            animator.CrossFadeInFixedTime("Sliding", 0.6f);
            FindObjectOfType<AudioManager>().Play("Slide");
            InRoll = true; ;
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
        if(col.tag != "Ground")
        {
            if (hitY == HitY.Down  && hitX == HitX.Mid)           
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

            else if(hitY == HitY.Up || hitY == HitY.Mid && hitX == HitX.Right)
            {

                PlayAnimation("ShoulderR");
                runSpeed = 0f;
                ResetCollision();
            }
        }     
        

    }

    public HitX GetHitX(Collider col)
    {
        Bounds char_bounds = characterController.bounds;
        Bounds col_bounds = col.bounds;
        float min_x = Mathf.Max(col.bounds.min.x, char_bounds.min.x);
        float max_x = Mathf.Min(col.bounds.max.x, char_bounds.max.x);

        float average = (min_x + max_x ) / 2 -col.bounds.min.x;

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

        Bounds char_bounds = characterController.bounds;
        Bounds col_bounds = col.bounds;
        float min_y = Mathf.Max(col.bounds.min.y, char_bounds.min.y);
        float max_y = Mathf.Min(col.bounds.max.y, char_bounds.max.y);

        float average = ((min_y + max_y) / 2 - char_bounds.min.y) / char_bounds.size.y ;

        HitY hit;

            if (average <  0.17f)
            {
                hit = HitY.Low;
            }
            else if (average <  0.33f)
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
        Bounds char_bounds = characterController.bounds;
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
