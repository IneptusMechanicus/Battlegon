using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    public float smoothing = 6f;
    private Vector3 offset;
     
	// Use this for initialization
	void Start ()
    {
        transform.position = new Vector3(target.position.x, target.position.y + 20, target.position.z - 8);
        transform.rotation = Quaternion.Euler(70, 0, 0);
        offset = transform.position - target.position;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
	} 
}
