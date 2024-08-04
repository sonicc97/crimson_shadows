using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float range = 100;
    private GM gm;

    public Material[] colors;
    // Start is called before the first frame update
    void Start()
    {
        Reposition();
        gm = GameObject.FindGameObjectWithTag("GM").GetComponent<GM>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Reposition()
    {
        //return;
        Vector3 pos = new Vector3(0, 6, 0);
        pos.x = Random.Range(-range, range);
        pos.z = Random.Range(-range, range);
        transform.position = pos;
    }

    public void NextStage(int stage)
    {
        Reposition();
        GetComponent<MeshRenderer>().material = colors[stage];
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        Gun g = other.GetComponent<Gun>();

        if (g)
        {
            gameObject.SetActive(false);
            gm.CoinCollected();
        }
    }
}
