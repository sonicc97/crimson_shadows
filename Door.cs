using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameObject door;
    public GameObject portal;
    // Start is called before the first frame update
    void Start()
    {
        //Unlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Unlock()
    {
        portal.SetActive(true);
        door.transform.Rotate(0, 120, 0, Space.World);
    }
}
