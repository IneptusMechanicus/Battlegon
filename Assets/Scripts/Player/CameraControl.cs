using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    Transform CameraTarget;
    [SerializeField]
    float smoothing = 6f;
    private Vector3 offset;
                                    
	void Start ()
    {
        transform.position = new Vector3(CameraTarget.position.x, CameraTarget.position.y + 10, CameraTarget.position.z - 4);
        transform.rotation = Quaternion.Euler(70, 0, 0);
        offset = transform.position - CameraTarget.position;
	}
	           
	void FixedUpdate ()
    {
        if (CameraTarget != null)
        {
            Vector3 targetCamPos = CameraTarget.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
        else
        {
            Destroy(this.gameObject);
        }
	} 
    
    public void SetTarget(Transform t)
    {
        CameraTarget = t;
    }                                 
}
