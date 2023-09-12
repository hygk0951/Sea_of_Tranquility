using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerMovement : Singleton<PlayerMovement>
{
    public static Animator animator;
    List<IdlePose> idlePoses = new List<IdlePose>();
    Rigidbody rigidBody;

    [Header("Debugs")]
    [SerializeField] bool debugLogTest = false;
    [SerializeField] bool jumpStatus = false;
    [SerializeField] public bool indoor = false;
    [SerializeField] bool artificialGravity = true;
    [SerializeField] bool cursorLock = true;

    [Header("Camera")]
    [SerializeField] Transform thirdPersonCamera;
    [SerializeField] public CinemachineFreeLook freeLockCamera;
    [SerializeField] CameraControls cameraControls;
    Vector3 lastPosition;

    [Header("Player Move")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float turnSpeed = 600f;
    float turnDirection = 0f;

    [Header("Player Jump")]
    public RaycastHit hitInfoDown;
    public RaycastHit hitInfoUp;
    [SerializeField] LayerMask indoorCheckRoof;
    [SerializeField] float surfaceJump = 10f;
    [SerializeField] float jumpThrust = 100f;
    [SerializeField] float maxHeight = 4.0f;
    [SerializeField] float groundBoundary = 0.1f;

    [Header("Idle Pose Settings")]
    bool readyToChangePose = false;
    int randomSelectedIdlePose;
    float idlePoseNumber = -1f;
    float lerpPoseTime = 0f;
    float waitTime = 0f;

    [Header("Fuel")]
    [SerializeField] public float currentFuel = 400.0f;
    [SerializeField] public float maxFuel = 400.0f;

    [Header("Points")]
    public int playerLevel = 1;
    public float exp = 0f;
    public float maxExp = 300f;

    [Header("Engines")]
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem leftEngineParticles;
    [SerializeField] ParticleSystem rightEngineParticles;

    [Header("Flash Light")]
    [SerializeField] List<Light> flash;

    public Vector3 outdoorGravity = new Vector3(0, -1.62f, 0);
    public Vector3 indoorGravity = new Vector3(0, -9.8f, 0);

    public float idleTime = 0f;

    private void OnEnable()
    {
        animator = transform.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = Input.mousePosition;

        idlePoses.Add(new IdlePose("Neutral Idle", 0f, 10f));
        idlePoses.Add(new IdlePose("Ground Idle", 0.5f, 7.0f));
        idlePoses.Add(new IdlePose("Stretching Idle", 1f, 11f));

        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        DebugLogs(debugLogTest);
        IndoorControls();
        CheckThirdPersonCameraPosition();
        PlayerRandomIdlePose();
        PlayerWalk();
        SurfaceJump();
        PlayerThrust();
        RefillFuel();
        CalculateHeight();
        ExpControl();
    }

    private void ExpControl()
    {
        if (playerLevel < 4)
        {
            exp += Time.deltaTime;
            if (exp >= maxExp)
            {
                playerLevel++;
                exp = 0f;
                maxExp *= 2;
            }
        }
    }

    private void CheckThirdPersonCameraPosition()
    {
        if (lastPosition != Input.mousePosition)
        {
            lastPosition = Input.mousePosition;
            idleTime = 0;
            animator.SetFloat("IdleTime", idleTime);
        }
    }

    void DebugLogs(bool DebugMode)
    {
        if (!DebugMode) return;
        // Debug.Log("idleTime : " + idleTime.ToString("0.00") + " | hitDistance : " + hitInfoDown.distance.ToString("0.00") + " | rotation : " + turnDirection);
        // Debug.Log("mousePosition : " + lastPosition);
        // Debug.Log("선택된 포즈 : " + idlePoses[randomSelectedIdlePose].IdlePoseName);
        // Debug.Log("포즈 값 : " + idlePoses[randomSelectedIdlePose].PoseNumber);
        // Debug.Log("포즈 타임 : " + idlePoses[randomSelectedIdlePose].PoseTime);
        Debug.Log("현재 높이 : " + hitInfoDown.distance);
        Debug.DrawLine(this.transform.position, this.transform.position + this.transform.forward, Color.red);
        Debug.DrawRay(transform.position, Vector3.down, new Color(0, 1, 0), hitInfoDown.distance);
    }

    private void CalculateHeight()
    {
        Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.down, out RaycastHit hitInfoDown, Mathf.Infinity);
        Physics.Raycast(transform.position, Vector3.up, out RaycastHit hitInfoUp, Mathf.Infinity, indoorCheckRoof);
        this.hitInfoDown = hitInfoDown;
        this.hitInfoUp = hitInfoUp;
    }

    void IndoorControls()
    {
        IndoorCheck();
        AnimationSpeedControl();
        GravityControl();
        FlashControl();
    }

    private void GravityControl()
    {
        if (artificialGravity)
        {
            if (indoor)
            {
                if (Physics.gravity != indoorGravity)
                    Physics.gravity = indoorGravity;
            }
            else
            {
                if (Physics.gravity != outdoorGravity)
                    Physics.gravity = outdoorGravity;
            }
        }
        else
        {
            if (Physics.gravity != outdoorGravity)
                Physics.gravity = outdoorGravity;
        }
    }

    private void IndoorCheck()
    {
        if (hitInfoUp.distance > 0)
            indoor = true;
        else
            indoor = false;
    }

    private void FlashControl()
    {
        if (indoor)
        {
            for (int i = 0; i < flash.Count; i++)
            {
                flash[i].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < flash.Count; i++)
            {
                flash[i].enabled = true;
            }
        }
    }

    private void AnimationSpeedControl()
    {
        if (hitInfoDown.distance > groundBoundary)
        {
            float jumpAnimationMultiplier = 0.8f - hitInfoDown.distance;
            if (jumpAnimationMultiplier < 0) jumpAnimationMultiplier = 0f;
            animator.SetFloat("SurfaceJump", jumpAnimationMultiplier);
        }
        else
        {
            if (artificialGravity)
            {
                if (!indoor)
                {
                    animator.SetFloat("SurfaceJump", 1.0f);
                }
                else
                {
                    animator.SetFloat("SurfaceJump", 2.0f);
                }
            }
            else
            {
                animator.SetFloat("SurfaceJump", 1.0f);
            }
        }

    }

    private Vector3 GetMoveDirection()
    {
        float playerVerticalInput = Input.GetAxis("Vertical");
        float playerHorizontalInput = Input.GetAxis("Horizontal");

        Vector3 forward = thirdPersonCamera.transform.forward;
        Vector3 right = thirdPersonCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeVerticalInput = playerVerticalInput * forward;
        Vector3 rightRelativeVerticalInput = playerHorizontalInput * right;

        Vector3 cameraRelativeMovement = forwardRelativeVerticalInput + rightRelativeVerticalInput;

        return cameraRelativeMovement.normalized;
    }

    private void PlayerRotate(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            CheckRotateDirection(moveDirection);
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }
        // Vector3 dir = test.transform.position - transform.position;
        // dir.y = 0;
        // Quaternion rot = Quaternion.LookRotation(dir.normalized);
        // transform.rotation = rot;
    }

    private void CheckRotateDirection(Vector3 moveDirection)
    {
        // turnDirection < 0 = turn right
        // turnDirection > 0 = turn left
        turnDirection = Vector3.SignedAngle(moveDirection, transform.forward, Vector3.up);
    }

    private void PlayerWalk()
    {
        Vector3 moveDirection = GetMoveDirection();
        PlayerRotate(moveDirection);
        float currentWalkSpeed = animator.GetFloat("WalkSpeed");
        if (moveDirection.magnitude != 0)
        {
            idleTime = 0;
            if (
            currentWalkSpeed < 0.5f)
            {
                currentWalkSpeed = 0.5f;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                transform.Translate(moveDirection * moveSpeed * 1.5f * Time.deltaTime, Space.World);

                if (currentWalkSpeed < 1.0f)
                {
                    currentWalkSpeed += Time.deltaTime / 2;
                }
            }
            else
            {
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
                if (currentWalkSpeed < 0.5f) currentWalkSpeed += Time.deltaTime / 2;
                else if (currentWalkSpeed > 0.5f) currentWalkSpeed -= Time.deltaTime / 2;
            }
        }
        else
        {
            idleTime += Time.deltaTime;
            if (currentWalkSpeed > 0) currentWalkSpeed -= Time.deltaTime / 2;
            if (currentWalkSpeed < 0) currentWalkSpeed = 0;
        }
        animator.SetFloat("WalkSpeed", currentWalkSpeed);
        animator.SetFloat("IdleTime", idleTime);
    }

    // walkSpeed - 0.75f < 0 = walk else run
    // hitInfo.distance < groundBoundary
    private void MoonStep()
    {
        float walkSpeed = animator.GetFloat("WalkSpeed");
        if (walkSpeed > 0.75f && hitInfoDown.distance < groundBoundary)
        {
            jumpStatus = true;
        }
    }

    private void SurfaceJump()
    {
        if (jumpStatus)
        {
            rigidBody.AddRelativeForce((Vector3.up + Vector3.forward).normalized * surfaceJump * Time.deltaTime, ForceMode.Force);
            jumpStatus = false;
        }
    }

    private void PlayerThrust()
    {
        // Main Engine
        if (Input.GetKey(KeyCode.Space) && currentFuel > 0f)
        {
            currentFuel -= Time.deltaTime * 100;
            if (hitInfoDown.distance < maxHeight)
                rigidBody.AddRelativeForce(Vector3.up * jumpThrust * Time.deltaTime);
            EngineControl(mainEngineParticles, 5, new ParticleSystem.MinMaxCurve(0.4f, 0.8f));
        }
        else
        {
            // 아직 공중에 있으면
            if (hitInfoDown.distance > groundBoundary)
            {
                EngineControl(mainEngineParticles, 2, new ParticleSystem.MinMaxCurve(0.2f, 0.4f));
            }
            // 땅에 내려왔으면
            else
            {
                mainEngineParticles.Stop();
            }
        }
        EngineControl(leftEngineParticles, rightEngineParticles);
    }

    private void EngineControl(ParticleSystem engine, int startSpeed, ParticleSystem.MinMaxCurve startSize)
    {
        idleTime = 0;
        engine.Play();
        var psMain = engine.main;
        psMain.startSpeed = startSpeed;
        psMain.startSize = startSize;
    }

    private void EngineControl(ParticleSystem leftEngine, ParticleSystem rightEngine)
    {
        // Side Engine
        if (hitInfoDown.distance > groundBoundary)
        {
            if (turnDirection < -0.5)
            {
                // turn right
                leftEngineParticles.Play();
                rightEngineParticles.Stop();
            }
            else if (turnDirection > 0.5)
            {
                // turn left
                leftEngineParticles.Stop();
                rightEngineParticles.Play();
            }
            else
            {
                // no turn
                leftEngineParticles.Stop();
                rightEngineParticles.Stop();
            }
        }
        else
        {
            leftEngineParticles.Stop();
            rightEngineParticles.Stop();
        }
    }

    private void RefillFuel()
    {
        if (currentFuel < maxFuel && hitInfoDown.distance < groundBoundary)
        {
            currentFuel += Time.deltaTime * 100;
            if (currentFuel < maxFuel && GetMoveDirection().magnitude == 0)
            {
                currentFuel += Time.deltaTime * 100;
            }
        }
        return;
    }

    private void PlayerRandomIdlePose()
    {
        if (idleTime < cameraControls.idlePoseStartTime)
        {
            return;
        }
        if (readyToChangePose)
        {
            int beforePose = randomSelectedIdlePose;
            while (beforePose == randomSelectedIdlePose)
            {
                randomSelectedIdlePose = UnityEngine.Random.Range(0, 3);
            }

            lerpPoseTime = 0f;
            waitTime = 0f;
            readyToChangePose = false;
        }
        ChangePose(idlePoses[randomSelectedIdlePose]);
    }

    private void ChangePose(IdlePose idlePose)
    {
        if (idlePoseNumber < 0)
        {
            idlePoseNumber = animator.GetFloat("IdlePose");
        }

        lerpPoseTime += 0.3f * Time.deltaTime;
        float lerpPoseNumber = Mathf.Lerp(idlePoseNumber, idlePose.PoseNumber, lerpPoseTime);
        if (Mathf.Abs(idlePose.PoseNumber - lerpPoseNumber) > 0.05f)
        {
            animator.SetFloat("IdlePose", lerpPoseNumber);
        }
        else
        {
            TakeIdlePoseTime(idlePose.PoseTime);
        }
    }

    private void TakeIdlePoseTime(float poseTime)
    {
        waitTime += Time.deltaTime;
        // Debug.Log("Wait Time : " + waitTime);
        if (waitTime > poseTime)
        {
            readyToChangePose = true;
            idlePoseNumber = -1f;
        }
    }
}
