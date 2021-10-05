using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target = null;
   
    public float speed;
    public GameObject[] angles;
    public int index=-1;
    private Vector3 offset;
    private float y;
    public float SpeedVertical=5f;
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position;
     
        
    }

    // Update is called once per frame

    void LateUpdate()
    {
        
        if(index == -1)
        {
            Vector3 followPos = target.position + offset;

            RaycastHit hit;
            if (Physics.Raycast(target.position, Vector3.down, out hit, 2.5f))
            {
                y = Mathf.Lerp(y, hit.point.y, Time.deltaTime * SpeedVertical);
            }
            else
            {
                y = Mathf.Lerp(y, target.position.y, Time.deltaTime * SpeedVertical);
            }
            followPos.y = offset.y + y;
            transform.position = followPos;
        }

        else
        {
            transform.position = angles[index].transform.position;
        }
    }

    
}
