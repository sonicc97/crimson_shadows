using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public int size = 10;
    public float range = 100;
    // Start is called before the first frame update
    void Start()
    {
        Reposition();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reposition()
    {
        Vector3 pos = new Vector3(0, 6, 0);
        pos.x = Random.Range(-range, range);
        pos.z = Random.Range(-range, range);
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        Gun g = other.GetComponent<Gun>();

        if (g)
        {
            Reposition();
            g.AmmoPickUp(size);
        }
    }
}
