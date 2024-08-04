using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GM : MonoBehaviour
{
    public Gun gun;
    public GameObject cam;
    public GameObject enemy;
    public Transform enemiesParent;
    public int currEnemies = 0;
    public int maxenemies = 50;
    public int range = 120;
    private bool pause = false;
    private bool over = false;

    [Header("UI")]
    public RectTransform imageAmmo;
    public RectTransform imageHealth;
    public Text textAmmo;
    public Text textHealth;
    public RectTransform coinImage;
    public Text textMission;
    public GameObject pauseUI;
    public GameObject winUI;
    public GameObject loseUI;

    [Header("Fog")]
    public float minFog = 0.1f;
    public float maxFog = 0.4f;
    public float fog = 0.1f;
    public float speedFog = 0.1f;

    [Header("Logic")]
    public Door[] doors;
    public int currCoins = 0;
    public int maxCoins;
    public GameObject[] coins;

    [Header("Stages")]
    public int stage = 0;
    public GameObject[] enemyStages;
    public string[] coinMessages;

    // Start is called before the first frame update
    void Start()
    {
        coins = GameObject.FindGameObjectsWithTag("Coin");
        maxCoins = coins.Length;
        enemy = enemyStages[stage];
        textMission.text = coinMessages[stage];
    }

    // Update is called once per frame
    void Update()
    {
        //Enemies
        if ( currEnemies < maxenemies )
        {
            SpawnEnemies(maxenemies - currEnemies);
            currEnemies = maxenemies;
        }
        //fog
        fog = Mathf.Min(maxFog, fog + speedFog * Time.deltaTime);
        RenderSettings.fogDensity = fog;

        UpdateTracker();

        if (Input.GetKeyDown(KeyCode.Escape) && !over)
        {
            Pause();
        }
    }

    private void UpdateTracker()
    {
        if (!gun) return;
        Vector3 look = gun.transform.forward;
        Vector3 pos = gun.transform.position;
        Vector3 track;
        float angle = 180;
        look.y = 0;
        pos.y = 0;

        if (currCoins < maxCoins)
        {
            for (int i = 0; i < coins.Length; i++)
            {
                if (!coins[i].activeSelf) continue;
                track = coins[i].transform.position;
                track.y = 0;
                float a = Mathf.Abs(Vector3.Angle(look, track - pos));
                if (a < angle) angle = a;
            }
        }
        else
        {
            track = doors[stage].transform.position;
            track.y = 0;
            angle = Mathf.Abs(Vector3.Angle(look, track - pos));
        }

        if (angle < 20)
        {
            textMission.color = Color.white;
        }
        else
        {
            textMission.color = Color.black;
        }
    }

    public void EnemyDied()
    {
        currEnemies--;
        DOTween.To(() => fog, x => fog = x, minFog, 0.3f);
    }

    private void SpawnEnemies (int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(0, 0.1f, 0);
            pos.x = UnityEngine.Random.Range(-range, range);
            pos.z = UnityEngine.Random.Range(-range, range);
            Instantiate(enemy, pos, Quaternion.identity, enemiesParent);
        }
    }

    public void UpdateStats(float health, float maxHeatlh, float ammo, float maxAmmo)
    {
        textHealth.text = ((int)health).ToString();
        textAmmo.text = ((int)ammo).ToString();
        imageHealth.localScale = new Vector3((1.0f * health) / maxHeatlh, 1, 1);
        imageAmmo.localScale = new Vector3((1.0f * ammo) / maxAmmo, 1, 1);
    }

    public void CoinCollected()
    {
        currCoins++;
        UpdateCoins();
        if (currCoins == maxCoins)
        {
            doors[stage].Unlock();
            textMission.text = "ESCAPE";
        }
    }
    private void UpdateCoins()
    {
        // Prilagođava širinu UI elementa coinImage tako da odražava procenat preostalih kovanica koje treba prikupiti.
        coinImage.localScale = new Vector3(1 - (1.0f * currCoins) / maxCoins, 1, 1);
    }

    public void Upgrade()
    {
        if ( stage == 2)
        {
            // win
            Win();
        }
        else
        {
            stage++;
            enemy = enemyStages[stage];
            // coins
            for (int i = 0; i < coins.Length; i++)
            {
                coins[i].GetComponent<Coin>().NextStage(stage);
            }
            currCoins = 0;
            // enemies
            GameObject[] objects = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < objects.Length; i++)
            {
                Destroy(objects[i]);
            }
            currEnemies = 0;

            UpdateCoins();
        }

        gun.Upgrade(stage);
        textMission.text = coinMessages[stage];
        DOTween.To(() => fog, x => fog = x, minFog, 0.3f);
    }

    public void Pause()
    {
        pause = !pause;
        gun.enabled = !pause;
        Time.timeScale = pause ? 0 : 1;
        pauseUI.SetActive(pause);
        CursorLock(pause);
    }

    private void CursorLock(bool state)
    {
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Win()
    {
        OverCam();
        winUI.SetActive(true);
    }

    public void Lose()
    {
        OverCam();
        loseUI.SetActive(true);
    }

    private void OverCam()
    {
        over = true;
        CloseUI();

        gun.gameObject.SetActive(false);
        cam.GetComponent<FirstPersonLook>().enabled = false;
        cam.transform.GetChild(0).gameObject.SetActive(false);

        cam.transform.parent = null;
        cam.transform.DORotate(new Vector3(0, 90, 0), 3, RotateMode.FastBeyond360);
        cam.transform.DOMoveY(6, 3);
    }

    private void CloseUI()
    {
        CursorLock(true);
        textMission.gameObject.SetActive(false);
        imageHealth.gameObject.SetActive(false);
        imageAmmo.gameObject.SetActive(false);
        textHealth.gameObject.SetActive(false);
        textAmmo.gameObject.SetActive(false);
        coinImage.gameObject.SetActive(false);
    }
}
