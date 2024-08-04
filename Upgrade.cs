using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    private GM gm;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GM>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Gun g = other.GetComponent<Gun>();
        if (g)
        {
            Destroy(gameObject);
            gm.Upgrade();
        }
    }
}
