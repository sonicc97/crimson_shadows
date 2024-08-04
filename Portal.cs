using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject upgrade;

    private void OnTriggerEnter(Collider other)
    {
        Gun g = other.GetComponent<Gun>();
        if (g)
        {
            upgrade.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
