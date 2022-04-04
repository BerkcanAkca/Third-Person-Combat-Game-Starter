using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class ThirdPersonMovement : StateMachine
{
    public CharacterController controller;
    public Transform cam;
    [SerializeField] WeaponDisplay myWeapon;
    private float speed = 6f;
    [SerializeField] float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public bool isGrounded;
    Vector3 velocity = Vector3.zero;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public LayerMask enemyMask;
    public float gravity = -9.81f;
    [SerializeField] float jumpHeight = 3;
    public float timeBetweenAttacks = 1;
    public float attackOffset = 15;
    float timeStamp = 0;
    public bool isAttacking = false;
    public float returnIdleTime = 5;
    public static ThirdPersonMovement instance;
    UIManager ui;
    public int currentWeapon = 0; //Punch
    public bool isRunning = false;
    public float walkingSpeed = 6f;
    public float runningSpeed = 10f;
    public bool inCombat = false;
    float combatTimer = 0;
    public float combatRollCD = 3;
    [SerializeField] float rollDistance = 12;
    float lastRoll = 0;
    public Animator animator;
    public Collider[] nearbyEnemies;
    public List<GameObject> enemyList;
    public GameObject targetEnemy = null;
    [SerializeField] float findTargetRange = 18;
    int enemyIndex = 0;
    [SerializeField] CinemachineFreeLook targetedCam;
    public bool targetMode = false;
    //smooth target change during combat; variables here;
    [SerializeField] Transform targeter;
    [SerializeField] float posSpeed = 0.8f;
    [SerializeField] float rotSpeed = 1;
    Vector3 targetPositionDifference;
    //Vector3 targetRotationDifference;
    float targetModeCD = 0.6f;
    float timeSinceLastTargetMode;
    Vector3 playerPosition;
    [SerializeField] GameObject combatRectangle;
    
    public Volume volume;

    

    //animator floats here
    
    float velX;
    float velY;

    //player stats here
    public int maxHP = 100;
    public int currentHP;
    public int level;
    
    

     void Awake()
    {
       
        instance = this;
    }

    private void Start()
    {
        SetState(new Roam(this));
        Cursor.lockState = CursorLockMode.Locked;
        myWeapon = WeaponDisplay.instance;
        speed = walkingSpeed;
        ui = UIManager.instance;
        enemyList = new List<GameObject>();
        currentHP = maxHP;
       
        
    }

    // Update is called once per frame
    void Update()
    {
        SetWeaponVariables();
        OnAttackKey();
        Movement();
        OnDefendKey();
        SelectWeapon();
        Run();
        CombatTracker();
        DetectEnemies();
        //FocusTargetedEnemy();
        playerPosition = transform.position;
        
        
    }
    
    

    
    void DetectEnemies()
    {
        if (Input.GetKeyDown(KeyCode.E) && Physics.CheckSphere(transform.position, findTargetRange, enemyMask) && targetMode == false)
        {
            timeSinceLastTargetMode = Time.time + targetModeCD;
            targetMode = true;
            //Create a list of enemies and target the closest one
            targetEnemy = null;
            enemyList.Clear();

            nearbyEnemies = Physics.OverlapSphere(transform.position, findTargetRange, enemyMask);
            foreach (Collider enemy in nearbyEnemies)
            {

                enemyList.Add(enemy.gameObject);
                
            }

            enemyList = enemyList.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToList();
            targetEnemy = enemyList[0];
            
        }

        if (Input.GetKeyDown(KeyCode.E) && targetMode == true && Time.time > timeSinceLastTargetMode)
            targetMode = false;

        if (!Physics.CheckSphere(transform.position, findTargetRange, enemyMask))
        {
            targetEnemy = null;
            enemyList.Clear();
            targetMode = false;
        }

        //if there are more than 1 enemy nearby switch targets with Scroll

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            
            if (enemyList.Count > 1 && targetEnemy != null)
            {

                TargetNextEnemy();

            }

        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {

            if (enemyList.Count > 1 && targetEnemy != null)
            {

                TargetPrevEnemy();

            }
        }

        if(targetEnemy == null && enemyList.Count > 0)
        {
            TargetNextEnemy();
        }

        if (targetEnemy == null && enemyList.Count == 0)
        {
            targetedCam.m_LookAt = transform;
        }





    }

    void TargetNextEnemy()
    {
        

            
            enemyIndex++;

            if (enemyIndex >= enemyList.Count)
            {
                enemyIndex = 0;
            }
        targetEnemy = enemyList[enemyIndex];
        


    }

    void TargetPrevEnemy()
    {
        

            
            enemyIndex--;

            if (enemyIndex < 0)
            {
                enemyIndex = enemyList.Count - 1;
            }
        targetEnemy = enemyList[enemyIndex];
        

    }

    void FocusTargetedEnemy()
    {
        if (targetMode == false) { targetedCam.m_LookAt = transform; return; }
        if (targetEnemy == null && !inCombat && targetedCam.m_LookAt != transform) { targetedCam.m_LookAt = transform; return; }


        if (targetEnemy != null && inCombat && targetMode == true)
        {
            //adding targeter here for smooth transition
            if (targetedCam.m_LookAt != targeter.transform)
            targetedCam.m_LookAt = targeter.transform;

            if (Vector3.Distance(targetEnemy.transform.position, targeter.transform.position) >= 0.7f)
            {
                targetPositionDifference = (targetEnemy.transform.position - targeter.transform.position);
                targeter.transform.position += targetPositionDifference * Time.deltaTime * posSpeed;
            }

            //Vector3 yFixedTargeterPosition = new Vector3(targeter.transform.position.x, transform.position.y, targeter.transform.position.z);
            Vector3 yFixedTargetPosition = new Vector3(targetEnemy.transform.position.x, transform.position.y, targetEnemy.transform.position.z);
            Vector3 direction = yFixedTargetPosition - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);
            //targetRotationDifference = (yFixedTargetPosition - yFixedTargeterPosition);
            
            //gameObject.transform.LookAt(transform.forward += targetRotationDifference * Time.deltaTime * rotSpeed);


            Vector3 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);

            if (screenPos.x < 0 || screenPos.y < 0 ||
                screenPos.x > Screen.width || screenPos.y > Screen.height)
            {
                Debug.Log("out of bounds"); 
                //-20 +20
            }
            if (screenPos.x < 1000)
            {
                targetedCam.m_XAxis.Value += 15 * Time.deltaTime;
            }
            if (screenPos.x > Screen.width - 1000)
            {
                targetedCam.m_XAxis.Value -= 15 * Time.deltaTime;
            }
            if (screenPos.y < 200)
            {
                targetedCam.m_YAxis.Value += 0.1f * Time.deltaTime;
            }
            if (screenPos.y > Screen.height - 200)
            {
                targetedCam.m_YAxis.Value -= 0.1f * Time.deltaTime;
            }


        }


    }

    private void FixedUpdate()
    {
        Animate();
      
    }

    void LateUpdate()
    {
        FocusTargetedEnemy();
    }
   

    
    

    void Animate()
    {
        if (Input.GetAxisRaw("Vertical") > 0 && velY <= 1)
        {
            while (velY < 1)
            {
                velY += 0.1f * Time.deltaTime;
            }
            if(velY >= 1)
            velY = 1;
        }
        if (Input.GetAxisRaw("Vertical") < 0 && velY >= -1 && targetMode == true)
        {
            while (velY > -1)
            {
                velY -= 0.1f * Time.deltaTime;
            }
            if (velY <= -1)
                velY = -1;
        }
        if (Input.GetAxisRaw("Vertical") < 0 && velY <= 1 && targetMode == false)
        {
            while (velY < 1)
            {
                velY += 0.1f * Time.deltaTime;
            }
            if (velY >= 1)
                velY = 1;
        }


        if (Input.GetAxisRaw("Horizontal") > 0 && velX <= 1)
        {
            while (velX < 1)
            {
                velX += 0.1f * Time.deltaTime;
            }
            if (velX >= 1)
                velX = 1;
        }
        if (Input.GetAxisRaw("Horizontal") < 0 && velX >= -1)
        {
            while (velX > -1)
            {
                velX -= 0.1f * Time.deltaTime;
            }
            if (velX <= -1)
                velX = -1;
        }

        if (Input.GetAxisRaw("Horizontal") == 0)
            velX = 0;
        if (Input.GetAxisRaw("Vertical") == 0)
            velY = 0;

        if (Input.GetKey(KeyCode.LeftShift) && inCombat == false)
        {
            velX = 2;
            velY = 2;
        }
        animator.SetBool("inCombat", inCombat);
        animator.SetFloat("velX", velX);
        animator.SetFloat("velY", velY);

    }

    public void OnPauseButton()
    {
        SetState(new Pause(this));
    }

    public void OnResumeButton()
    {
        SetState(new Roam(this));
    }




    void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f && !isAttacking)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            if (!targetMode)
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
            
            

        }

        //jump

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);



        if (isGrounded && velocity.y < 0)

        {

            velocity.y = -2f;

        }



        if (Input.GetButtonDown("Jump") && isGrounded)

        {

            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        }

        //gravity

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void OnAttackKey()
    {

        

        if (Input.GetMouseButtonDown(0) && timeStamp <= Time.time)
        {
            combatTimer = 0;
            inCombat = true;
            isAttacking = true;
            animator.SetBool("isAttacking", true);
            timeStamp = Time.time + timeBetweenAttacks;
            StartCoroutine(State.Attack());
            StartCoroutine(AttackMovement());
            

        }

        
       
    }

    IEnumerator AttackMovement()
    {
        float time = 0;
        while (time <= timeBetweenAttacks)
        {
            time += Time.deltaTime;
            if (targetEnemy == null)
            {
                Vector3 transformDirection = cam.rotation * Vector3.forward;
                controller.Move(new Vector3(transformDirection.normalized.x * attackOffset, 0, transformDirection.normalized.z * attackOffset) * Time.deltaTime);
                transform.LookAt(transform.position + new Vector3(transformDirection.normalized.x * 5, 0, transformDirection.normalized.z * 5));
            }

            if (targetEnemy != null)
            {
                Vector3 transformDirection = transform.rotation * Vector3.forward;
                controller.Move(new Vector3(transformDirection.normalized.x * attackOffset, 0, transformDirection.normalized.z * attackOffset) * Time.deltaTime);
            }
            
            yield return null;
        }
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }
    IEnumerator Roll()
    {
        float time = 0;
        while (time <= 0.4f)
        {
            time += Time.deltaTime;
            Vector3 transformDirection =  transform.localRotation.normalized * Vector3.forward;
            if (!targetMode)
            controller.Move(new Vector3(transformDirection.normalized.x * rollDistance, 0, transformDirection.normalized.z * rollDistance) * Time.deltaTime);
            if (targetMode)
                controller.Move(-new Vector3(transformDirection.normalized.x * rollDistance, 0, transformDirection.normalized.z * rollDistance) * Time.deltaTime);
            //transform.LookAt(transform.position + new Vector3(transformDirection.normalized.x * 5, 0, transformDirection.normalized.z * 5));
            yield return null;
        }
        
    }


    void OnDefendKey()
    {


        if (Input.GetMouseButton(1))
        {
            combatTimer = 0;
            inCombat = true;           
            StartCoroutine(State.Defend());
        }



    }

    void SelectWeapon()
    {
        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            currentWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            currentWeapon = 1;
    }

    void SetWeaponVariables()
    {
        timeBetweenAttacks = myWeapon.weaponCooldown;
        attackOffset = myWeapon.weaponMoveForce;
    }

    void Run()
    {
        if (Input.GetKeyDown(KeyCode.Q) && lastRoll <= Time.time)
        {
            lastRoll = Time.time + combatRollCD;
            StartCoroutine(Roll());

        }

        if (inCombat) return;

        if (Input.GetKey(KeyCode.LeftShift))
        {

            isRunning = true;
            speed = runningSpeed;
        }
        else
        {
            if (!isRunning) return;
            isRunning = false;
            speed = walkingSpeed;
        }
        
        

    }

    void CombatTracker()
    {
        if (inCombat)
        {
            combatTimer += Time.deltaTime;
            speed = walkingSpeed / 1.5f;
            
        }
           

        if (combatTimer >= 5)
        {
            combatTimer = 0;
            inCombat = false;
            speed = walkingSpeed;
        }

        if (!inCombat)
        {
            StartCoroutine(State.Roam());
            combatRectangle.SetActive(false);
        }

        if (targetMode && inCombat)
            combatRectangle.SetActive(true);

        if (!targetMode)
        {
            combatRectangle.SetActive(false);
        }


    }

}
