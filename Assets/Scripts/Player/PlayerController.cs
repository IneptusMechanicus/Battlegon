using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{                   
    // The movement speed of the player    
    [SerializeField]
    float Speed;

    //Reference to the player camera
    [SerializeField]
    Camera PlayerCam;
    
    // The transform of the gun origin
    // That is the position, where the gun will be in relation to the player
    [SerializeField]
    Transform GunOrigin;

    //The upper body bone of the player animation rig. Used to rotate player upper body on aim
    [SerializeField]
    Transform LowerBody;
    
    //Checks whether the player is using a gamepad controller or not
    bool OnGamepad;
    
    //Vector3 variable used in the movement method
    Vector3 MoveData = Vector3.zero;
    
    //The player's RidgidBody component
    //This is used to apply physics
    Rigidbody PlayerRB;          
    
    //Gun that the player has picked up
    public CombatItem Gun;
	
    //The players sword and shield
	[SerializeField]
    CombatItem Sword;

    [SerializeField]
    CombatItem Shield;

    //The animator controller of the legs
    [SerializeField]
    Animator legsAnimator;

    //The animator controller of the torso
    [SerializeField]
    Animator torsoAnimator;

    public bool isOnGun;


    void Start()
    {  
        isOnGun = false;
        PlayerRB = GetComponent<Rigidbody>();  
        PlayerCam = Instantiate(PlayerCam, transform.position, transform.rotation) as Camera;
        PlayerCam.GetComponent<CameraControl>().SetTarget(transform);  
    }
                        
    void FixedUpdate ()
    {
        legsAnimator.SetBool("isWalking", false);
        checkConrolMethod();
        Move();

        if (OnGamepad)
        {
            GamepadRotate ();
        }
        else
        {
            Rotate ();
        }

        if ((Input.GetAxisRaw("Fire") != 0 || Input.GetAxisRaw("GPFire") == -1)) //&& isOnGun
        {
            CmdAction1(true);
            Debug.Log("Fireeeeeeeeeeeeee");
            Debug.Log(isOnGun);
        }
        else
        {
            CmdAction1(false);
        }

        if(Input.GetAxisRaw("Drop") != 0)
            Drop(Gun.gameObject);
    }  

    //Command, which spawns the bullet on the network
    [Command(channel=1)]                                                                         
    void CmdAction1(bool on)
    {
       if (Gun != null)
       {
            Gun.Action1(on); 
            Debug.Log("Player Fired");
       }    
    } 

	//Basic movement function. Tracks Input Manager Axis input and moves the players rigid body accordingly 
    void Move()
    {
        
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        if(x != 0f || z != 0f)
        {
            legsAnimator.SetBool("isWalking", true);
        }

        MoveData.Set(x, 0f, z);
        MoveData = MoveData.normalized * Speed;
        PlayerRB.MovePosition(PlayerRB.position + MoveData * Time.deltaTime);
        
        Vector3 direction = Vector3.right * Input.GetAxisRaw("Horizontal") 
                          + Vector3.forward * Input.GetAxisRaw("Vertical");
        if(direction.sqrMagnitude > 0.0f)
        {
            LowerBody.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            LowerBody.transform.Rotate(0, 90, 0);
        }
    }

	//Rotates the player depending on the position of the mouse via raycasting
	//A ray is cast from the player camera to the ground
	//When it is cast, the method tracks where the ray hits the floor.
	// The player is then rotated toward that points X and Z coordinates.
    void Rotate()
    {              
        float rayL;
        Ray camRay = PlayerCam.ScreenPointToRay(Input.mousePosition);
        Plane floor = new Plane(Vector3.up, Vector3.zero);

        if (floor.Raycast(camRay, out rayL))
        {
            Vector3 lookPoint = camRay.GetPoint(rayL);
            transform.LookAt(new Vector3(lookPoint.x, transform.position.y, lookPoint.z));
            transform.Rotate(0, 90, 0);
            
        }
    }

	//Controller rotation for the player.
	//The player rotates to the direction, in which you push the left stick
    void GamepadRotate()
    {
        Vector3 direction = Vector3.right * Input.GetAxisRaw("GPLookHorizontal") 
                          + Vector3.forward * -Input.GetAxisRaw("GPLookVertical");
        if(direction.sqrMagnitude > 0.0f)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.Rotate(0, 90, 0);
        }
    }

	//Checks whether the player uses mouse and keyboard or a controller
    void checkConrolMethod()
    {
        if (Input.GetJoystickNames().Length != 0)
            OnGamepad = true;

        if (Input.GetAxisRaw("Fire") != 0
            || Input.GetAxisRaw("Mouse X") != 0
            || Input.GetAxisRaw("Mouse X") != 0)
            OnGamepad = false;
    }
		
    void OnCollisionEnter(Collision c)
    {
		if(c.gameObject.tag == "Gun")
        {              
            PickUpGun(c.gameObject);
        }   
    }

    //Picks up a GunController CombatItem object
    //Sets it up as the players equiped gun
    void PickUpGun(GameObject g)
    {
        torsoAnimator.SetTrigger("PickUpGun");
        torsoAnimator.SetBool("isGunUp", true);
		CombatItem c = g.GetComponent<CombatItem>(); 
		if (Gun != null) Drop(Gun.gameObject);

		c.SetEquiped(true);
        g.GetComponent<GunController>().EquipSetup();
		Gun = c;

        g.transform.parent = GunOrigin.transform;
        g.transform.position = GunOrigin.transform.position;
        g.transform.rotation = GunOrigin.transform.rotation;
        g.SetActive(true);
        g.GetComponent<CombatItem>().enabled = true;

        g.GetComponent<CombatItem>().player = GetComponent<PlayerStats>();
        isOnGun = true;
    }

    //Drops the currently equipped gun
    public void Drop(GameObject g)
    {
        torsoAnimator.SetBool("isGunUp", false);
        torsoAnimator.SetTrigger("DropGun");
        g.transform.parent = null;
        g.GetComponent<CombatItem>().SetEquiped(false);
        g.GetComponent<GunController>().EquipSetup();
        g.GetComponent<GunController>().Drop();
        Gun = null;
        isOnGun = false;
    }

    public void CamSwitch(bool on)
    {
        PlayerCam.gameObject.SetActive(on);
    }
}
