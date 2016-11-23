using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float speed;
    private Vector3 moveData = Vector3.zero;
    private Rigidbody PlayerRB;
    private Camera mainCam;
    public GunController gun;
    public bool onGamepad = false;

    void Start()
    {
        PlayerRB = GetComponent<Rigidbody>();
        mainCam = FindObjectOfType<Camera>();   
    }

    // Update is called once per frame
    void FixedUpdate ()
    {                         
        checkConrolMethod();
        Move();

        if (onGamepad)
        {
            GamepadRotate();
            GamepadFire();
        }
        else
        {
            Rotate();
            Fire();
        }
	}

    void Fire()
    {
        float firing = Input.GetAxisRaw("Fire");
        if(firing != 0f)
        {
            gun.isFiring = true;
        }
        else
        {
            gun.isFiring = false;
        }
    }

    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        
        moveData.Set(x, 0f, z);
        moveData = moveData.normalized * speed;
        PlayerRB.MovePosition(PlayerRB.position + moveData * Time.deltaTime);
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
        Vector3 direction = Vector3.right * Input.GetAxisRaw("GPLookHorizontal") + Vector3.forward * -Input.GetAxisRaw("GPLookVertical");
        if(direction.sqrMagnitude > 0.0f)
        {
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }

    void GamepadFire()
    {
        float firing = Input.GetAxis("GPFire");
        if (firing == -1)
        {
            gun.isFiring = true;
        }
        else
        {
            gun.isFiring = false;
        }
    }

    void checkConrolMethod()
    {
        if (Input.GetJoystickNames().Length != 0)
            onGamepad = true;
        if (Input.GetMouseButtonDown(0)
            || Input.GetMouseButtonDown(1)
            || Input.GetMouseButtonDown(2)
            || Input.GetAxisRaw("Mouse X") != 0
            || Input.GetAxisRaw("Mouse X") != 0)
            onGamepad = false;
    }
}
