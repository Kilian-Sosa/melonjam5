using UnityEngine;

public class PlayerController : MonoBehaviour {
    [SerializeField] float _speed = 5f;
    [SerializeField] float _jumpForce = 5f;
    Rigidbody _rb;
    bool _isGrounded;

    void Start() {
        _rb = GetComponent<Rigidbody>();
    }

    void Update() {
        if (!GameManager.Instance.isActive) { FreezePosition(); return; }

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f)) {
            if (hit.collider.CompareTag("Ground")) _isGrounded = true;
        } else _isGrounded = false;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
        // Make the player look at the direction it's moving
        if (moveDirection != Vector3.zero) {
           Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2*Time.deltaTime);
        }

        // Mover el objeto en la dirección de movimiento
       //transform.Translate(moveDirection * _speed * Time.deltaTime, Space.World);

          // Move the player using the Rigidbody
        Vector3 targetPosition = _rb.position + moveDirection * _speed * Time.deltaTime;
        _rb.MovePosition(targetPosition);

        if (Input.GetButtonDown("Jump") && _isGrounded) {
            if (!AudioManager.Instance.IsPlayingCountDown()) AudioManager.Instance.PlaySFX("jump");
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode.Impulse);
            _isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) _isGrounded = true;
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) _isGrounded = false;
    }

    public float GetSpeed() => _speed;

    public void SetSpeed(float speed) => _speed = speed;

    public float GetJumpForce() => _jumpForce;

    public void SetJumpForce(float jumpForce) => _jumpForce = jumpForce;

    public void SetAttributes(float speed, float jumpForce) {
        _speed = speed;
        _jumpForce = jumpForce;
    }

    void OnDestroy() {
        if (!AudioManager.Instance.IsPlayingCountDown()) AudioManager.Instance.PlaySFX("fall");
        if (name == "PlayerObj") GameManager.Instance.GameOver();
    }

    void FreezePosition() {
        _rb.constraints = RigidbodyConstraints.FreezePosition;
    }
}
