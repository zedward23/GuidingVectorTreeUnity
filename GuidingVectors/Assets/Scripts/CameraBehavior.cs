using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Vector3 currPos = gameObject.transform.position;
            gameObject.transform.position = new Vector3(currPos.x+.2f, currPos.y, currPos.z);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 currPos = gameObject.transform.position;
            gameObject.transform.position = new Vector3(currPos.x - .2f, currPos.y, currPos.z);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector3 currPos = gameObject.transform.position;
            gameObject.transform.position = new Vector3(currPos.x, currPos.y - .2f, currPos.z);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Vector3 currPos = gameObject.transform.position;
            gameObject.transform.position = new Vector3(currPos.x, currPos.y + .2f, currPos.z);
        }

    }
}
