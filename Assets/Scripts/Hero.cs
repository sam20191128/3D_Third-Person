using UnityEngine;

public class Hero : MonoBehaviour
{
    public Rigidbody rb;
    public float gravityModifier;//重力
    public float jumpForce;//弹跳力
    public bool isGround;

    public Camera mapCamera;

    public Animator anim;

    //WSAD
    public float speed;
    public float rotationSpeed;

    public float translation;
    public float rotation;

    public Camera cam;
    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public float desiredRotationSpeed;
    public bool blockRotationPlayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity *= gravityModifier;

        anim = GetComponent<Animator>();

        cam = Camera.main;
    }

    private void Update()
    {
        PlayerMoveAndRotation();
        Dash();
        Jump();
        Attack();
        SwitchAnim();
    }

    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");
        transform.Translate(InputX * speed * Time.deltaTime, 0, InputZ * speed * Time.deltaTime); //沿着Z轴移动
    }

    public void RotateToCamera(Transform t)
    {

        var forward = cam.transform.forward;
        forward.y = 0f;
        desiredMoveDirection = forward;
        t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetTrigger("jump");
            isGround = false;
        }
    }

    private void Dash()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(0, 0, InputZ * speed * 2 * Time.deltaTime); //沿着Z轴移动
            anim.SetBool("running", true);
        }
        else
        {
            anim.SetBool("running", false);
            //processVolume.enabled = false;
        }
    }

    void Attack()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    anim.SetTrigger("attack");
        //}
        //if (Input.GetMouseButtonDown(1))
        //{
        //    anim.SetTrigger("attack2");
        //}
        //if (Input.GetMouseButtonDown(2))
        //{
        //    anim.SetTrigger("attack3");
        //}
    }

    void SwitchAnim()//切换动画效果
    {
        anim.SetFloat("InputZ", InputZ);
        anim.SetFloat("InputX", InputX);

    }

    private void OnCollisionEnter(Collision collision)
    {
        isGround = true;
        anim.SetBool("isGround", true);
    }
}