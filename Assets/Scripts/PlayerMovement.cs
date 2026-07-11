using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float laneChangeSpeed = 10f;
    private float targetX = 0f;

    public bool canMove = true;

    [Header("Puan Bazlı Hızlanma")]
    public float baseSpeed = 8f;               
    public float speedAt1K   = 10f;            
    public float speedAt10K  = 13f;            
    public float speedBoostPer10K = 2f;        
    public float maxSpeed = 30f;               

    [HideInInspector] public float forwardSpeed;

    [Header("Zıplama & Eğilme")]
    public float jumpForce = 6f;
    public float fastFallForce = 30f;          
    private bool isFastFalling = false;        

    private Vector3 normalScale;
    private Vector3 crouchScale;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        normalScale = transform.localScale;
        crouchScale = new Vector3(normalScale.x, normalScale.y / 2f, normalScale.z);

        forwardSpeed = baseSpeed;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!canMove) return;

        UpdateSpeedByScore();

        transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

        if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) && targetX < 2.5f) targetX += 2.5f;
        if ((Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) && targetX > -2.5f) targetX -= 2.5f;

        Vector3 newPos = transform.position;
        newPos.x = Mathf.MoveTowards(newPos.x, targetX, laneChangeSpeed * Time.deltaTime);
        transform.position = newPos;

        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (!isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -fastFallForce, rb.linearVelocity.z);
                isFastFalling = true;
                transform.localScale = crouchScale;
            }
            else
            {
                transform.localScale = crouchScale;
            }
        }

        if (isGrounded && isFastFalling)
        {
            isFastFalling = false;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            transform.localScale = normalScale;
        }
    }

    void UpdateSpeedByScore()
    {
        if (ScoreManager.Instance == null) return;

        float score = ScoreManager.Instance.score;
        float targetSpeed;

        if (score < 1000f)
        {
            targetSpeed = baseSpeed;
        }
        else if (score < 10000f)
        {
            targetSpeed = speedAt1K;
        }
        else
        {
            int tenKCount = Mathf.FloorToInt(score / 10000f); 
            targetSpeed = speedAt10K + (tenKCount - 1) * speedBoostPer10K;
        }

        forwardSpeed = Mathf.Min(targetSpeed, maxSpeed);
    }

    public void BounceBack(float normalX)
    {
        if (normalX > 0.3f)
            targetX += 2.5f;
        else if (normalX < -0.3f)
            targetX -= 2.5f;

        targetX = Mathf.Clamp(targetX, -2.5f, 2.5f);
    }
}
