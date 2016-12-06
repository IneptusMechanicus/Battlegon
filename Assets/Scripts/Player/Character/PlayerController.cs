using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float speed;
    public bool onGamepad = false;

    private Vector3 moveData = Vector3.zero;
    private Rigidbody playerRB;
    private Camera mainCam;

    [SerializeField]
    public CombatItem primary;
    public CombatItem secondary;
    private CombatItem current;
    private bool onPrimary = true;
   

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        mainCam = FindObjectOfType<Camera>();
    }

    
    void FixedUpdate ()
    {                         
        checkConrolMethod();
        setCurrentItem();
        Move();                        

        if (onGamepad)
        {
            GamepadRotate();
            GPAction1(current);       
        }
        else
        {
            Rotate();
            Action1(current);                 
        }                                        
    }       

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        moveData.Set(x, 0f, z);
        moveData = moveData.normalized * speed;
        playerRB.MovePosition(playerRB.position + moveData * Time.deltaTime);
    }

    void Rotate()
    {
        Ray camRay = mainCam.ScreenPointToRay(Input.mousePosition);
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
            onGamepad = true;
        if (Input.GetAxisRaw("Fire") != 0
            || Input.GetAxisRaw("Mouse X") != 0
            || Input.GetAxisRaw("Mouse X") != 0)
            onGamepad = false;
    }

    void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.name == "laserBullet(Clone)")
        {
            transform.position = new Vector3(0, transform.position.y, 0);
        }
    }

    void Action1(CombatItem c)
    {
        if (Input.GetAxisRaw("Fire") != 0) c.Action1(true);      
        else c.Action1(false);
    }

    void GPAction1(CombatItem c)
    {
        if (Input.GetAxisRaw("GPFire") == -1) c.Action1(true);
        else c.Action1(false);
    }

    void setCurrentItem()
    {
        if(!onPrimary) current = secondary;
        else current = primary;
    }
}
