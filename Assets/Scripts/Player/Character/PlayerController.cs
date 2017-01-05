using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float Speed;
    public bool OnGamepad = false;
    public bool OnPrimary = true;

    private Vector3 MoveData = Vector3.zero;
    private Rigidbody PlayerRB;
    public Camera PlayerCam;
    public Transform gunOrigin;
    
    public CombatItem Primary;
    public CombatItem Secondary;
   

    void Start()
    {
       
        PlayerRB = GetComponent<Rigidbody>();
        PlayerCam = Instantiate(PlayerCam, transform.position, transform.rotation) as Camera;
        PlayerCam.GetComponent<CameraControl>().target = transform;
        PlayerCam.transform.parent = transform.parent;
    }

    
    void FixedUpdate ()
    {
        gameObject.SetActive(true);
        checkConrolMethod();  
        Move();
        if (OnPrimary && Primary != null) Action1(Primary);
        else if(!OnPrimary && Secondary != null) Action1(Secondary);
        if (OnGamepad)
        {
            GamepadRotate();     
        }
        else
        {
            Rotate();                 
        }                                        
    }       

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        MoveData.Set(x, 0f, z);
        MoveData = MoveData.normalized * Speed;
        PlayerRB.MovePosition(PlayerRB.position + MoveData * Time.deltaTime);
    }

    void Rotate()
    {
        Ray camRay = PlayerCam.ScreenPointToRay(Input.mousePosition);
        Plane floor = new Plane(Vector3.up, Vector3.zero);
        float rayL;

        if (floor.Raycast(camRay, out rayL))
        {
            Vector3 lookPoint = camRay.GetPoint(rayL);
            transform.LookAt(new Vector3(lookPoint.x, transform.position.y, lookPoint.z));
        }
    }

    void GamepadRotate()
    {
        Vector3 direction = Vector3.right * Input.GetAxisRaw("GPLookHorizontal") 
                          + Vector3.forward * -Input.GetAxisRaw("GPLookVertical");
        if(direction.sqrMagnitude > 0.0f)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }

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
        if(c.gameObject.tag == "Bullet")
        {
            transform.position = new Vector3(0, transform.position.y, 0);
        }
        else if(c.gameObject.tag == "CombatItem")
        {               
            PickUp(c.gameObject);
        }
    }

    void PickUp(GameObject g)
    {
        g.transform.parent = transform;
        g.transform.position = gunOrigin.transform.position;
        g.transform.rotation = gunOrigin.transform.rotation;
        CombatItem c = g.GetComponent<CombatItem>();
        c.equiped = true;
        if (OnPrimary)
        {
            if (Primary != null)
            {
                Drop(Primary.gameObject);
            }
            Primary = c;
        }
        else
        {
            Drop(Secondary.gameObject);
            Secondary = c;
        }
    }

    void Drop(GameObject g)
    {
        g.transform.parent = null;
        if (OnPrimary) Primary = null;
        else Secondary = null;
    }

    void Action1(CombatItem c)
    {
        if (Input.GetAxisRaw("Fire") != 0 || Input.GetAxisRaw("GPFire") == -1) c.Action1(true);      
        else c.Action1(false);
    }                                                                         
          

    void OnDestroy()
    {
        PlayerCam.GetComponent<CameraControl>().enabled = false;
        Drop(Primary.gameObject);
        Drop(Secondary.gameObject);
    }
}
