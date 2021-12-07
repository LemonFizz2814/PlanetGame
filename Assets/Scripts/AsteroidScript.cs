using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AsteroidScript : MonoBehaviour
{
    [SerializeField] float speedMin;
    [SerializeField] float speedMax;
    [SerializeField] GameObject asteroidGained;

    UIManager uiManager;
    
    float speed;

    int[] currencyReward = { 2, 5, 10, 20, 50 };
    int[] currencyPrecentage = { 20, 40, 15, 15, 10 };

    int reward;

    private void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();

        speed = Random.Range(speedMin, speedMax);

        //set up percentage of reward
        int prec = Random.Range(0, 100);
        int add = 0;

        for(int i = 0; i < currencyPrecentage.Length; i++)
        {
            add += currencyPrecentage[i];

            if (add >= prec)
            {
                reward = i;
                return;
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position += transform.right * speed * Time.deltaTime;
    }

    void OnMouseDown()
    {
        //grant player reward
        uiManager.UpdateCurrencyText(currencyReward[reward]);
        GameObject gained = Instantiate(asteroidGained, transform.position, Quaternion.identity);
        gained.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+" + currencyReward[reward];
        Destroy(gained, 2);
        Destroy(gameObject);
    }

    public void SetLookAt(GameObject _camera)
    {
        Vector3 diff = _camera.transform.position - transform.position;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
    }
}
