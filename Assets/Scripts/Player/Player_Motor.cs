using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Player_Motor : MonoBehaviour
{
    //Movement Logic
    private Rigidbody Rig;
    private Vector3 mov;
    public float Speed;

    //Fall Logic
    public int isGrounded;
    private bool canJump, canRoll;

    //Network Logic
    public PhotonView View;

    public bool isPlayer;

    //Animation Logic
    private Animator Anim;
    private AudioSource Aud;

    //misc
    //X Grounding, Y - Animation, Z - Collision
    private Quaternion Timers;
    private TextMesh Text;
    private CapsuleCollider Col;

    public Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        //important components
        Anim = GetComponent<Animator>();
        Rig = GetComponent<Rigidbody>();
        Aud = GetComponent<AudioSource>();
        Text = GetComponentInChildren<TextMesh>();
        Col = GetComponent<CapsuleCollider>();
        cam = GameObject.Find("MainCamera").transform;
        Circle = transform.Find("LandRing");
        Circle.gameObject.SetActive(false);

        if (View.IsMine)
        {
            GameObject.Find("Effects").GetComponent<EffectFollow>().Local = transform;
        }

    }
    //updated every few miliseconds
    private void Update()
    {
        //changing collider for the animation
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName("DoubleJump") ||
            Anim.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            Col.height = 1;
            Col.center = new Vector3(0, 0.5f, 0);
        }
        else if (Timers.y == 0)
        {
            Col.height = 1.5f;
            Col.center = new Vector3(0, 0.72f, 0);
        }

        if (View.IsMine && isPlayer)
        {
            //setting player speed
            if (Input.GetAxis("Sprint") > 0.25 || Input.GetAxis("Sprint") < -0.25)
            {
                if (Speed < 3) Speed = 3;
                else if (Speed < 6) Speed += Speed / 1000;
            }
            else Speed = 1.5f;

            //getting player movement input
            mov = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (mov.x != 0 || mov.z != 0)
            {
                if (Input.GetAxis("Jump/Roll") < -0.25f && isGrounded == 2 && canRoll) Roll();
                else if (Input.GetAxis("Jump/Roll") == 0 && isGrounded == 2 && !Anim.GetCurrentAnimatorStateInfo(0).IsName("Roll")) canRoll = true;
                Anim.SetFloat("Speed", Speed);
                Move();
            }
            else Anim.SetFloat("Speed", 0);

            //Jumping
            if (Input.GetAxis("Jump/Roll") > 0.25f && isGrounded > 0 && canJump) Jump();
            else if (Input.GetAxis("Jump/Roll") == 0 && isGrounded > 0) canJump = true;
        }
    }

    //help players figure where they are
    private RaycastHit Floor;
    private Transform Circle;

    //updated every few miliseconds
    private void FixedUpdate()
    {
        if (View.IsMine && isPlayer)
        {
            //action timer
            if (Timers.x > 0) Timers.x--;
            if (Timers.z > 0) Timers.z--;
            if (Timers.y > 0) Timers.y--;
            if (Timers.y == 1)
            {
                Anim.SetBool("Roll", false);
                Anim.SetBool("Jump", false);
                Anim.SetBool("DoubleJump", false);
            }
            //safety net
            if (transform.position.y < -20)
            {
                transform.position = new Vector3(0, 20, 0);
            }

            if (isGrounded != 2)
            {
                Physics.Raycast(transform.position, Vector3.down, out Floor, Mathf.Infinity);
                Circle.position = Floor.point;
                if (Vector3.Distance(transform.position, Circle.position) > 0.25f) Circle.gameObject.SetActive(true);
                else Circle.gameObject.SetActive(false);
            }
            else Circle.gameObject.SetActive(false);

        }
    }

    private float curSpeed;
    private float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    //WASD Movement
    void Move()
    {
        //slowing player while in air
        if (isGrounded == 2)
        {
            curSpeed = Speed; 
        }
        else if(curSpeed > 1.5)
        {
            curSpeed *= 0.999f;
        }
        //moving player relative of camera 
        Vector3 direction = mov.normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0f);
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * curSpeed;
            //moving with rigidbody
            Rig.MovePosition(Rig.position + moveDir * Time.deltaTime);
        }
    }

    //when player trys to roll
    void Roll()
    {
        if (View.IsMine && isPlayer)
        {
            Timers.y = 10;
            Anim.SetBool("Roll", true);
            Rig.AddForce(transform.forward * 4, ForceMode.Impulse);
            canRoll = false;
        }
    }

    //when player trys to jump
    void Jump()
    {
        if (View.IsMine && isPlayer)
        {
            //animation
            if (isGrounded == 1)
            {
                Rig.AddForce(transform.up * 2.5f + transform.forward * 2, ForceMode.Impulse);
                Timers.y = 30;
                Anim.SetBool("DoubleJump", true);
            }
            else
            {
                Anim.SetBool("Roll", false);
                Timers.y = 30;
                Rig.AddForce(transform.up * 4.5f, ForceMode.Impulse);

                Anim.SetBool("Jump", true);
            }

            isGrounded--; canJump = false;
            Anim.SetBool("isGrounded", false);
        }
    }

    //collision handling
    private Vector3 LandedPos;
    private void OnCollisionEnter(Collision collision)
    {
        if (View.IsMine && isPlayer)
        {
            Anim.SetBool("isGrounded", true);
            if (Physics.Raycast(transform.position, Vector3.down, 0.2f))
            {
                isGrounded = 2;
            }
            else if (Timers.x == 0)
            {
                Timers.x = 4;
                LandedPos = transform.position;
            }
        }
    }
    //collision handling
    private void OnCollisionStay(Collision collision)
    {
        if (View.IsMine && isPlayer && isGrounded != 2)
        {
            Anim.SetBool("isGrounded", true);
            if (Physics.Raycast(transform.position, Vector3.down, 0.2f) ||
                Timers.x == 1 && Vector3.Distance(transform.position, LandedPos) < 0.25f)
            {
                isGrounded = 2;
            }
        }
        if (collision.gameObject.tag == "Wall") Rig.velocity = new Vector2(0, Rig.velocity.y);
    }
    //When exiting a Collider
    private void OnCollisionExit(Collision collision)
    {
        if (View.IsMine && isPlayer)
        {
            if(isGrounded == 2) isGrounded = 1;
            Anim.SetBool("isGrounded", false);
        }
    }

    //recieving a message
    void Chat(string Msg)
    {
        Aud.Play();
        Text.text = Msg;
        Text.GetComponent<TextToCamera>().Timer = Msg.Length * 40;
    }
}