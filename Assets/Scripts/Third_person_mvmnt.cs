using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;

public class Third_person_mvmnt : MonoBehaviour
{
    public CharacterController controller;

    [SerializeField] private Transform respawnPoint;

    public float speed = 6f;
    public float jumpForce;
    public float gravityScale;
    public float yvelocity;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public List<string> listMisteryPower = new List<string>(){"SpeedUp","SpeedDown","ArmorUp","ArmorDown","AttackUp","AttackDown","ChangeGuns"};
    public Transform cam;
    private Animator animator;
    private CharacterController charController;
    private CapsuleCollider capsCollider;
    public bool dead;
    private bool ragdoll = false;
    private bool hasPower;
    private float speedPowerFactor;
    private float armorPowerFactor = 1;
    private float attackPowerFactor = 1;
    private float powerUpTimer;
    private float powerUpEffectTime;
    TPCamController cameraController;

    Vector2 i_movement = Vector2.zero;
    bool jumped = false;

    //Test Ragdoll
    public GameObject weapon;
    public bool modeBot = false;

    private void Start()
    {
        respawnPoint = GameObject.Find("RespawnCube").transform; // Peut être changer ça car trop sale
        animator = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        capsCollider = GetComponent<CapsuleCollider>();
        cameraController = cam.GetComponent<TPCamController>();
        initialisePlayerProperties();
        dead = false;

        if (weapon)
            weapon.SetActive(!dead);
    }

    public void Ragdoll()
    {
        charController.enabled = !charController.enabled;
        capsCollider.enabled = !capsCollider.enabled;
        animator.enabled = !animator.enabled;
        dead = !dead;

        if (weapon)
            weapon.SetActive(!dead);
        cameraController.deadChar = dead;
    }


    // Update is called once per frame
    void Update()
    {
        if (modeBot) //Teste avec un bot joueur (en solo)
            return;

        //TEMP keyboard movement
        //float horizontalTEMP = Input.GetAxisRaw("Horizontal");
        //float verticalTEMP = Input.GetAxisRaw("Vertical");
        if(hasPower == true){
            powerUpTimer += Time.deltaTime;
            if(powerUpTimer > powerUpEffectTime){
                initialisePlayerProperties();
            }
        }
        if (ragdoll)
        {
            initialisePlayerProperties();
            charController.enabled = !charController.enabled;
            capsCollider.enabled = !capsCollider.enabled;
            animator.enabled = !animator.enabled;
            dead = !dead;
            cameraController.deadChar = dead;
            Ragdoll();

            if (dead)
            {
                cameraController.CamFocus = cameraController.RagdollTarget;
            }
            else
            {
                cameraController.CamFocus = cameraController.Target;
            }
            ragdoll = false;
        }

        if (dead)
        {
            return;
        }
        float horizontal = i_movement.x;
        float vertical = i_movement.y;

        //if(vertical > 0 || verticalTEMP > 0)
        //{
        //    animator.SetBool("IsRunning", true);
        //    animator.SetBool("IsBacking", false);
        //}
        //else if(vertical < 0 || verticalTEMP < 0)
        //{
        //    animator.SetBool("IsBacking", true);
        //    animator.SetBool("IsRunning", false);
        //}
        //else
        //{
        //    animator.SetBool("IsRunning", false);
        //    animator.SetBool("IsBacking", false);
        //}

        //if (horizontal > 0 || horizontalTEMP > 0)
        //{
        //    animator.SetBool("IsRight", true);
        //    animator.SetBool("IsLeft", false);
        //}
        //else if (horizontal < 0 || horizontalTEMP < 0)
        //{
        //    animator.SetBool("IsLeft", true);
        //    animator.SetBool("IsRight", false);
        //}
        //else
        //{
        //    animator.SetBool("IsRight", false);
        //    animator.SetBool("IsLeft", false);
        //}
        if (vertical > 0)
        {
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsBacking", false);
        }
        else if (vertical < 0)
        {
            animator.SetBool("IsBacking", true);
            animator.SetBool("IsRunning", false);
        }
        else
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsBacking", false);
        }

        if (horizontal > 0)
        {
            animator.SetBool("IsRight", true);
            animator.SetBool("IsLeft", false);
        }
        else if (horizontal < 0)
        {
            animator.SetBool("IsLeft", true);
            animator.SetBool("IsRight", false);
        }
        else
        {
            animator.SetBool("IsRight", false);
            animator.SetBool("IsLeft", false);
        }



        //Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 direction = (cam.forward * vertical) + (cam.right * horizontal); 
        //Vector3 directionTEMP = (cam.forward * verticalTEMP) + (cam.right * horizontalTEMP); 
        direction.Normalize();
        //directionTEMP.Normalize();
        direction = direction * (speed*speedPowerFactor);
        //directionTEMP = directionTEMP * speed;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        //else if (directionTEMP.magnitude >= 0.1f)
        //{
        //    float targetAngle = Mathf.Atan2(directionTEMP.x, directionTEMP.z) * Mathf.Rad2Deg;
        //    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        //    transform.rotation = Quaternion.Euler(0f, angle, 0f);
        //}


        if (controller.isGrounded)
        {
            if (jumped)
            {
                yvelocity = jumpForce;
                //Invoke("stopVelocity", 0.6f);
            }
            //else if (Input.GetKey("space"))
            //{
            //    yvelocity = jumpForce;
            //    //Invoke("stopVelocity", 0.6f);
            //}
            else yvelocity = 0;
        }
        else
        {
            yvelocity += Physics.gravity.y * Time.deltaTime * gravityScale;
        }

        direction.y = yvelocity;
        //directionTEMP.y = yvelocity;
        
        //ceci enleve le jitter du saut
        transform.position += direction * Time.deltaTime;
        //transform.position += directionTEMP * Time.deltaTime;



        //controller.Move(direction * Time.deltaTime);

    }

    public void OnMove(InputValue value) {
        i_movement = value.Get<Vector2>();
    }

    public void OnMoveKey(InputValue value) {
        i_movement = value.Get<Vector2>();
    }

    public void OnJumpPress(InputValue value) {
        jumped = true;
    }

    public void OnJumpRelease(InputValue value) {
        jumped = false;
    }

    public void OnCameraH(InputValue value) {
        cameraController.OnCameraH(value);
    }

    public void OnCameraV(InputValue value) {
        cameraController.OnCameraV(value);
    }

    public void OnRagdoll() {
        ragdoll = !ragdoll;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Respawn"))
        {
            initialisePlayerProperties();
            Transform location = respawnPoint.transform;
            float rangex = Random.Range(-(location.localScale.x / 2), location.localScale.x / 2);
            float rangez = Random.Range(-(location.localScale.z / 2), location.localScale.z / 2);
            Vector3 spawnPoint = new Vector3(location.position.x + rangex, location.position.y, location.position.z + rangez);
            transform.position = spawnPoint;
            ScoreManager sM = GetComponent<ScoreManager>();
            if(!ragdoll){
                sM.ScoreDown(); //diminue le score du joueur qui tombe, utilisé lors d'une chute sans ragdoll
                //sM.GetLastShooter().GetComponentInParent(typeof(ScoreManager)).GetComponent<ScoreManager>().ScoreUp();
            }
            else
            {
                sM.GetLastShooter().GetComponentInParent(typeof(ScoreManager)).GetComponent<ScoreManager>().ScoreUp();//ligne pour augmenter le score du joueur qui a tiré en dernier sur la victime

            }
            
            
        }
    }

    private void stopVelocity()
    {
        yvelocity = 0;
    }


    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "MisteryBox"){
            Destroy(collision.gameObject);
            var nbPower = listMisteryPower.Count;
            var randomPower = listMisteryPower[Random.Range(0,nbPower)];
            activePower(randomPower);
        }
    }

    public float getAttackPowerFactor(){
        return attackPowerFactor;
    }
    public float getArmorPowerFactor(){
        return armorPowerFactor;
    }

    void setArmorPowerFactor(float armorFactor){
        armorPowerFactor = armorFactor;
    }

    void setAttackPowerFactor(float attackFactor){
        attackPowerFactor = attackFactor;
    }

    void setSpeedPowerFactor(float speedFactor){
        speedPowerFactor = speedFactor;
    }

    void resetArmorPower(){
        setArmorPowerFactor(1f);
    }
    void resetAttackPower(){
        setAttackPowerFactor(1f);
    }

    void resetSpeedPower(){
        setSpeedPowerFactor(1f);
    }

    void activePower(string powerName){
        powerUpTimer = 0;
        hasPower = true;
        powerUpEffectTime = 10f; // A choisir si l'onn souhaite accumuler le temps des effets (+=) ou bien le réinitialiser (=)
        print(powerName);
        switch (powerName){
            case "SpeedUp":
            {   
                setSpeedPowerFactor(3f);
                Invoke("resetSpeedPower", powerUpEffectTime);
                break;
            }
            case "SpeedDown":
            {   
                setSpeedPowerFactor(0.5f);
                Invoke("resetSpeedPower", powerUpEffectTime);
                break;
            }
            case "ArmorUp":
            {   
                setArmorPowerFactor(2f);
                Invoke("resetArmorPower", powerUpEffectTime);
                break;
            }
            case "ArmorDown":
            {   
                setArmorPowerFactor(0.5f);
                Invoke("resetArmorPower", powerUpEffectTime);
                break;
            }
            case "AttackUp":
            {   
                setAttackPowerFactor(2f);
                Invoke("resetAttackPower", powerUpEffectTime);
                break;
            }
            case "AttackDown":
            {   
                setAttackPowerFactor(0.5f);
                Invoke("resetAttackPower", powerUpEffectTime);
                break;
            }
            case "ChangeGuns":
            {   
                speedPowerFactor = 5f;
                break;
            }
            default: break;
            
        }
        print("Attack" + attackPowerFactor);
        print("Armor" + armorPowerFactor);
        print("Speed" + speedPowerFactor);
    }
    void initialisePlayerProperties(){
        hasPower = false;
        setSpeedPowerFactor(1f);
        setArmorPowerFactor(1f);
        setAttackPowerFactor(1f);
    }
}