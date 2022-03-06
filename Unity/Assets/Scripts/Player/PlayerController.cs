using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sneakSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float smoothTime;

    [SerializeField] private float lookAngle = 90f;

    [SerializeField] private GameObject cameraHolder;

    [SerializeField] private bool grounded;
    private Vector3 moveAmount;

    private Rigidbody rb;

    private Vector3 soomthMoveVelocity;

    private float verticalLookRotation;

    #region Public

    public void SetGroundedState(bool isGrounded)
    {
        grounded = isGrounded;
    }

    #endregion Public
    
    #region Keys

    private readonly KeyCode sprintKeyCode = KeyCode.LeftShift;
    private readonly KeyCode sneakKeyCode = KeyCode.C;
    private readonly KeyCode jumpKeyCode = KeyCode.Space;

    #endregion Keys

    #region Overrides

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Look();
        Move();
        Jump();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    #endregion Overrides

    #region Private

    private void Jump()
    {
        if (Input.GetKeyDown(jumpKeyCode) && grounded)
        {
            Debug.Log("Jump !");
            rb.AddForce(transform.up * jumpForce);
        }
    }

    private void Move()
    {
        var moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        float speed = 0;

        if (Input.GetKey(sprintKeyCode))
            speed = sprintSpeed;
        else if (Input.GetKey(sneakKeyCode))
            speed = sneakSpeed;
        else
            speed = walkSpeed;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * speed, ref soomthMoveVelocity, smoothTime);
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -lookAngle, lookAngle);
        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    #endregion Private
}