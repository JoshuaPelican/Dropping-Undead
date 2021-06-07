using UnityEngine;
using UnityEngine.UI;

public class CharacterMovement : MonoBehaviour
{
    private CharacterController controller;
    public AnimationManager animManager;
    public Collider crawlCollider;
    public Collider crawlCollider2;
    public CharacterShooting shoot;

    public float moveSpeed;
    public float lookSpeed;
    public float jumpSpeed;
    public float gravity;

    public Transform spineBone;
    public float lookXLimit;
    public Transform aimLookPoint;

    private Vector3 moveDirection;

    private bool landing;
    public bool layingDown;

    private float verticalAxis;
    private float horizontalAxis;
    private float rotationX;
    public bool characterLocked;

    public Slider lookSpeedSlider;

    void Start()
    {
        //landingCollider.enabled = false;
        controller = GetComponent<CharacterController>();
        transform.GetChild(0).GetComponent<AnimationManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    public float VerticalAxis
    {
        get { return verticalAxis; }
        set
        {
            //Only updates when the variable is changed
            if (verticalAxis != value)
            {
                verticalAxis = value;

                //Give animator forward movement percentage
                animManager.UpdateAnimation("Forward Velocity", verticalAxis);
            }
        }
    }

    public float HorizontalAxis
    {
        get { return horizontalAxis; }
        set
        {
            //Only updates when the variable is changed
            if (horizontalAxis != value)
            {
                horizontalAxis = value;

                //Give animator forward movement percentage
                animManager.UpdateAnimation("Sideways Velocity", horizontalAxis);
            }
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            characterLocked = true;
        }
        else
        {
            characterLocked = false;
        }

        if (!characterLocked)
        {
            //Local Vector Variable
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);

            //local float variable to store the current veritcal direction of our player
            float jumpDirectionY = moveDirection.y;

            //Gets all inputs from the player
            VerticalAxis = Mathf.Clamp(Input.GetAxis("Vertical"), -.55f, 1);
            HorizontalAxis = Input.GetAxis("Horizontal");

            animManager.UpdateAnimation("Grounded", controller.isGrounded);

            moveDirection = Vector3.zero;


            //If there is any forward input, add it to the move direction
            if (VerticalAxis != 0)
            {
                moveDirection += VerticalAxis * moveSpeed * forward;
            }
            if (HorizontalAxis != 0)
            {
                moveDirection += HorizontalAxis * (moveSpeed / 1.5f) * right;
            }

            if (layingDown)
            {
                moveDirection /= 3;
            }
            else if (shoot.aiming)
            {
                moveDirection *= shoot.equippedGun.aimMoveSpeedMulti;
            }

            //If landing smooth out the landing animation over time and based on how fast the character is moving
            if (landing)
            {
                animManager.UpdateAnimation("JumpRunBlend", VerticalAxis / 1.3f);
            }

            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
            animManager.UpdateAnimation("Turn Velocity", Input.GetAxis("Mouse X"));

            if (Input.GetAxisRaw("Jump") == 1 && controller.isGrounded && !layingDown)
            {
                moveDirection.y = jumpSpeed;
                landing = false;
                animManager.UpdateAnimation("Landing", false);
            }
            else
            {
                moveDirection.y = jumpDirectionY;
            }

            if (Input.GetKey(KeyCode.LeftShift) && VerticalAxis < .1f && VerticalAxis > -.1f && !shoot.aiming)
            {
                animManager.UpdateAnimation("LayingDown", true);
                layingDown = true;

                controller.height = .5f;
                controller.radius = .25f;
                controller.center = new Vector3(controller.center.x, .25f, controller.center.z);

                crawlCollider.enabled = true;
                crawlCollider2.enabled = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                animManager.UpdateAnimation("LayingDown", false);
                layingDown = false;

                controller.height = 2;
                controller.radius = .25f;
                controller.center = new Vector3(controller.center.x, 1, controller.center.z);

                crawlCollider.enabled = false;
                crawlCollider2.enabled = false;
            }

            //Gravity
            if (!controller.isGrounded)
            {
                animManager.UpdateAnimation("Relative Jump Height", moveDirection.y / jumpSpeed);

                moveDirection.y -= gravity * Time.deltaTime;
            }

            controller.Move(moveDirection * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        if (!characterLocked)
        {
            if (shoot.aiming)
            {
                rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

                spineBone.rotation *= Quaternion.Euler(rotationX, 0, 0);

                Vector3 aimPos = aimLookPoint.localPosition;
                aimPos.y = -(rotationX / 30) + 1.5f;
                aimLookPoint.localPosition = aimPos;
            }
            else
            {

                Vector3 aimPos = aimLookPoint.localPosition;
                aimPos.y = .3f;
                aimLookPoint.localPosition = aimPos;
            }
        }
    }

    //Landing Collider
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            landing = true;
            animManager.UpdateAnimation("Landing", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            landing = false;
            animManager.UpdateAnimation("Landing", false);
        }
    }

    public void UpdateLookSpeed(float value)
    {
        lookSpeed = value;
    }
}
