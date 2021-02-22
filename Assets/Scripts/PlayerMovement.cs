using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float footstepTime = 1f;
    [SerializeField] private TerrainType defaultTerrain = default;
    private float _timeSinceFootstep;
    private SoundInstance _footstepSound;
    private SoundInstance _jumpSound;
    private SoundInstance _jumpLandSound;
    private AudioManager _audioManager;
    private TerrainType _currentTerrain;

    public TerrainType Terrain => _currentTerrain;

    public float moveSpeed = 5f;
    public float rotateVerticleSpeed = 5f;
    public float rotateHorizontalSpeed = 5f;
    public float jumpSpeed = 8f;
    [Range(0, 100)] public int jumpFrames = 15;
    public Rigidbody body;
    public Camera camera;

    public bool IsActive => inputX != 0 || inputZ != 0;
    float inputX;
    float inputZ;

    private bool[] jumpBuffer;
    private bool isGrounded;
    private int jumpCooldown;

    void Start()
    {
        _audioManager = AudioManager.Instance();
        _currentTerrain = defaultTerrain;
        _footstepSound = Terrain.WalkSound.GenerateInstance();
        _jumpSound = Terrain.JumpSound.GenerateInstance();
        _jumpLandSound = Terrain.JumpLandSound.GenerateInstance();

        jumpBuffer = new bool[jumpFrames];
        jumpCooldown = 0;
        isGrounded = true;
    }

    void Update()
    {
        body.velocity = new Vector3(0, body.velocity.y, 0);

        // Movement input
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CanJump())
            {
                jumpCooldown = jumpBuffer.Length;
                body.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);

                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3f))
                {
                    // If the terrain changed, update the sound and terrain variables
                    UpdateTerrain(hit.transform.GetComponent<Terrain>());
                    _audioManager.PlaySound(_jumpSound);
                }
            }
        }

        // Plays footstep sound if we exceed the footstep time
        if (_timeSinceFootstep > footstepTime && jumpBuffer[0] && IsActive)
        {
            _timeSinceFootstep = 0f;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3f))
            {
                // If the terrain changed, update the sound and terrain variables
                UpdateTerrain(hit.transform.GetComponent<Terrain>());
                _audioManager.PlaySound(_footstepSound);
            }
        }
        else if (IsActive)
        {
            _timeSinceFootstep += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        // Movement
        body.position += (transform.forward * inputZ + transform.right * inputX) * (moveSpeed * Time.deltaTime);

        // Ground check        
        RaycastHit hit;
        bool tempGrounded = false;
        for (int i = jumpBuffer.Length - 1; i > 0; i--)
        {
            jumpBuffer[i] = jumpBuffer[i - 1];
            tempGrounded |= jumpBuffer[i];
        }
        jumpBuffer[0] = Physics.SphereCast(transform.position, 0.35f, Vector3.down, out hit, 0.1f);
        isGrounded = jumpBuffer[0] | tempGrounded;
        if (jumpCooldown > 0)
        {
            jumpCooldown--;

            if (jumpBuffer[0] && !jumpBuffer[1])
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f))
                {
                    // If the terrain changed, update the sound and terrain variables
                    UpdateTerrain(hit.transform.GetComponent<Terrain>());
                    _audioManager.PlaySound(_jumpLandSound);
                }
            }
        }
    }

    private void UpdateTerrain(Terrain newTerrain)
    {
        if (newTerrain != null && newTerrain.terrainType != Terrain)
        {
            _currentTerrain = newTerrain.terrainType;
            _footstepSound = Terrain.WalkSound.GenerateInstance();
            _jumpSound = Terrain.JumpSound.GenerateInstance();
            _jumpLandSound = Terrain.JumpLandSound.GenerateInstance();
        }
        else if (newTerrain == null)
        {
            _currentTerrain = defaultTerrain;
            _footstepSound = Terrain.WalkSound.GenerateInstance();
            _jumpSound = Terrain.JumpSound.GenerateInstance();
            _jumpLandSound = Terrain.JumpLandSound.GenerateInstance();
        }
    }

    private bool CanJump()
    {
        return isGrounded && jumpCooldown == 0;
    }
}
