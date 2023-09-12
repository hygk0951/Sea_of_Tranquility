using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementLegacy : MonoBehaviour
{
    public static Animator animator;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float turnSpeed = 600f;
    [SerializeField] float surfaceJump = 10f;
    [SerializeField] float jumpThrust = 100f;
    [SerializeField] float maxHeight = 4.0f;
    [SerializeField] float fuel = 4.0f;
    [SerializeField] float maxFuel = 4.0f;
    public static float startIdleTime = 10.0f;
    float walkSpeed = 0.5f;
    bool isJumping = false;
    [SerializeField] bool debugHeightDistance = false;
    int randomSelectedIdlePose = 0;
    float idleTime = 0f;
    float idlePoseNumber = -1f;
    float lerpPoseTime = 0f;
    float waitTime = 0f;
    public float turnDirection = 0f;
    List<IdlePose> idlePoses = new List<IdlePose>();
    bool readyToChangePose = true;
    RaycastHit hitInfo;
    float groundBoundary = 0.1f;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem leftEngineParticles;
    [SerializeField] ParticleSystem rightEngineParticles;
    [SerializeField] LayerMask ground;

    Rigidbody rigidBody;

    private void OnEnable()
    {
        animator = transform.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        idlePoses.Add(new IdlePose("Neutral Idle", 0f, 10f));
        idlePoses.Add(new IdlePose("Ground Idle", 0.5f, 3.5f));
        idlePoses.Add(new IdlePose("Stretching Idle", 1f, 11f));

        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        CalculateHeight();
        RefillFuel();
        PlayerMove();
        // PlayerJump();
        // PlayerThrust();
        // PlayerRandomIdlePose();
    }

    private void RefillFuel()
    {
        if (fuel < maxFuel && hitInfo.distance < groundBoundary)
        {
            fuel += Time.deltaTime;
            if (fuel < maxFuel && GetMoveDirection().magnitude == 0)
            {
                fuel += Time.deltaTime;
            }
        }
        return;
    }

    private void CalculateHeight()
    {
        Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, Mathf.Infinity, ground);
        Debug.DrawRay(transform.position, Vector3.down, new Color(0, 1, 0), hitInfo.distance);
        this.hitInfo = hitInfo;

        if (debugHeightDistance)
            Debug.Log(hitInfo.distance);

        if (hitInfo.distance > groundBoundary)
        {
            float jumpAnimationMultiplier = 1 - hitInfo.distance;
            if (jumpAnimationMultiplier < 0) jumpAnimationMultiplier = 0f;
            animator.SetFloat("SurfaceJump", jumpAnimationMultiplier);
        }
        else
        {
            animator.SetFloat("SurfaceJump", 1.0f);
        }
    }

    private void SurfaceMovementStep()
    {
        isJumping = true;
    }

    private void PlayerJump()
    {
        if (isJumping)
        {
            float walkSpeed = animator.GetFloat("WalkSpeed");
            if (hitInfo.distance < groundBoundary)
            {
                // Walk
                if (walkSpeed - 0.75f < 0)
                {
                    // rigidBody.AddRelativeForce((Vector3.up + Vector3.forward).normalized / 2.0f
                    // * surfaceJump * Time.deltaTime, ForceMode.Force);
                }
                // Run
                else
                {
                    rigidBody.AddRelativeForce((Vector3.up + Vector3.forward).normalized
                    * surfaceJump * Time.deltaTime, ForceMode.Force);
                }
            }
        }
        Invoke("JumpReset", 1.0f);
    }

    private void JumpReset()
    {
        isJumping = false;
    }

    private void PlayerMove()
    {
        PlayerWalkAround();
        // PlayerRotate(GetMoveDirection());
    }

    private Vector3 GetMoveDirection()
    {
        float xValue = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        float zValue = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;
        Vector3 moveDirection = new Vector3(xValue, 0, zValue);
        return moveDirection;
    }

    private void PlayerWalkAround()
    {
        Vector3 moveDirection = GetMoveDirection();
        moveDirection.Normalize();
        WalkOrRun(moveDirection);
    }

    private void WalkOrRun(Vector3 moveDirection)
    {
        float blendSpeed = 0.05f;
        bool isRun = false;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }
        // Movement Detected
        if (moveDirection.magnitude != 0)
        {
            // Run
            if (isRun)
            {
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime * 1.5f, Space.World);
                // Run = 1.0f
                if (walkSpeed < 1.0f) walkSpeed += blendSpeed;
            }
            // Walk
            else
            {
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
                // Walk = 0.5f
                if (walkSpeed < 0.5f) walkSpeed += blendSpeed;
                else if (walkSpeed > 0.5f) walkSpeed -= blendSpeed;
            }
            idleTime = 0f;
        }
        // Movement Not Detected
        else
        {
            if (walkSpeed > 0f) walkSpeed -= blendSpeed;
            idleTime += Time.deltaTime;
        }
        animator.SetFloat("WalkSpeed", walkSpeed);
        animator.SetFloat("IdleTime", idleTime);
    }

    private void PlayerRotate(Vector3 moveDirection)
    {
        if (moveDirection != Vector3.zero)
        {
            CheckRotateDirection(moveDirection);
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }
    }

    private void CheckRotateDirection(Vector3 moveDirection)
    {
        // turnDirection < 0 = turn right
        // turnDirection > 0 = turn left
        turnDirection = Vector3.SignedAngle(moveDirection, transform.forward, Vector3.up);
    }

    private void PlayerThrust()
    {
        // Main Engine
        if (Input.GetKey(KeyCode.Space) && fuel > 0f)
        {
            fuel -= Time.deltaTime;
            if (hitInfo.distance < maxHeight)
                rigidBody.AddRelativeForce(Vector3.up * jumpThrust * Time.deltaTime);
            EngineControl(mainEngineParticles, 5, new ParticleSystem.MinMaxCurve(0.4f, 0.8f));
        }
        else
        {
            // 아직 공중에 있으면
            if (hitInfo.distance > groundBoundary)
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
        if (hitInfo.distance > groundBoundary)
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

    private void PlayerRandomIdlePose()
    {
        if (idleTime < startIdleTime)
        {
            return;
        }
        if (readyToChangePose)
        {
            randomSelectedIdlePose = UnityEngine.Random.Range(0, 3);
            lerpPoseTime = 0f;
            waitTime = 0f;
            readyToChangePose = false;

            // Debug.Log("선택된 포즈 : " + idlePoses[randomSelectedIdlePose].IdlePoseName);
            // Debug.Log("포즈 값 : " + idlePoses[randomSelectedIdlePose].PoseNumber);
            // Debug.Log("포즈 타임 : " + idlePoses[randomSelectedIdlePose].PoseTime);
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
