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
    private PhotonView View;
    public bool isPlayer;

    //Animation Logic
    private Animator Anim;
    private AudioSource Aud;

    //misc
    private int timer, AniTimer;
    private TextMesh Text;
    private CapsuleCollider Col;

    // Start is called before the first frame update
    void Start()
    {
        //important components
        Rig = GetComponent<Rigidbody>();
        Anim = GetComponent<Animator>();
        Aud = GetComponent<AudioSource>();
        View = GetComponent<PhotonView>();
        Text = GetComponentInChildren<TextMesh>();
        Col = GetComponent<CapsuleCollider>();
        Circle = transform.Find("LandRing");
        Circle.gameObject.SetActive(false);

        //setting host character
        if (PhotonNetwork.IsMasterClient && gameObject.name == "Ink")
        {
            isPlayer = true;
            //grabbing ownwership
            View.RequestOwnership();

            //setting canvas tag
            GameObject.Find("Canvas").GetComponent<Master>().Cat = gameObject.name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //changing collider for the animation
        if (Anim.GetCurrentAnimatorStateInfo(0).IsName("DoubleJump") || Anim.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            Col.height = 1;
            Col.center = new Vector3(0, 0.5f, 0);
        }
        else
        {
            Col.height = 1.5f;
            Col.center = new Vector3(0, 0.72f, 0);
        }

        if (View.IsMine && isPlayer)
        {
            //getting player movement input
            mov = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (mov.x != 0 || mov.z != 0)
            {

                if (Input.GetAxis("Jump/Roll") < -0.25f && isGrounded == 2 && canRoll) Roll();
                else if (Input.GetAxis("Jump/Roll") == 0 && isGrounded == 2) canRoll = true;

                Anim.SetFloat("Speed", Speed);
                Move();
            }
            else Anim.SetFloat("Speed", 0);

            if (Input.GetAxis("Sprint") > 0)
            {
                if (Speed < 4)
                {
                    Speed += Speed / 100;
                }
                else if (Speed > 4) Speed = 4;
            }
            else Speed = 1.5f;

            //Jumping
            if (Input.GetAxis("Jump/Roll") > 0.25f && isGrounded > 0 && canJump) Jump();
            else if (Input.GetAxis("Jump/Roll") == 0 && isGrounded > 0) canJump = true;
        }

        if (transform.position.y < -20)
        {
            transform.position = new Vector3(0, 20, 0);
        }
    }

    //help players figure where they are
    private RaycastHit Floor;
    private Transform Circle;

    //updated every few miliseconds
    private void FixedUpdate()
    {

        //action timer
        if (timer > 0)
        {
            timer--;
        }
        
        if (isGrounded != 2)
        {
            Physics.Raycast(transform.position, Vector3.down, out Floor, Mathf.Infinity);
            Circle.position = Floor.point;
            if (Vector3.Distance(transform.position, Circle.position) > 0.25f) Circle.gameObject.SetActive(true);
            else Circle.gameObject.SetActive(false);
        }
        else Circle.gameObject.SetActive(false);

        //animation timer
        if (AniTimer > 0) AniTimer--;
        if (Anim.GetBool("Action") && AniTimer == 0) Anim.SetBool("Action", false);
    }

    public Transform cam;

    private float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    //WASD Movement
    void Move()
    {
        Vector3 direction = new Vector3(mov.x, 0f, mov.z).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * Speed;

            //moving with rigidbody
            Rig.MovePosition(Rig.position + moveDir * Time.deltaTime);
        }
    }

    //when player trys to roll
    void Roll()
    {
        Rig.velocity = new Vector3(0, 0, 0);
        Rig.AddForce(transform.forward * 4, ForceMode.Impulse);
        Anim.SetBool("isGrounded", true);
        canRoll = false;
        AniTimer = 20;
        Anim.SetBool("Action", true);
    }

    //when player trys to jump
    void Jump()
    {
        //animation
        if (isGrounded == 1)
        {
            AniTimer = 20;
            Anim.SetBool("Action", true);
            Rig.velocity = new Vector3(0, 0, 0);
            Rig.AddForce(transform.up * 3 + transform.forward * 2, ForceMode.Impulse);
        }
        else
        {
            //logic
            Rig.velocity = new Vector3(0, 0, Rig.velocity.z);
            Rig.AddForce(transform.up * 5, ForceMode.Impulse);
        }

        isGrounded --; canJump = false;
        Anim.SetBool("isGrounded", false);
    }

    //collision handling
    private void OnCollisionStay(Collision collision)
    {
        Anim.SetBool("isGrounded", true);

        if (View.IsMine && isPlayer)
        {
            if (Physics.Raycast(transform.position, Vector3.down, 0.1f))
            {
                isGrounded = 2;
            }
        }
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

    //Networking

    [PunRPC]
    public void RPC_Chat(string otherCat, string Msg)
    {
        if (otherCat == this.name)
        {
            Aud.Play();
            Text.text = Msg;
            Text.GetComponent<TextToCamera>().Timer = Msg.Length * 40;
        }
    }
}