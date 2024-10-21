using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public GameObject playerCamera;
    public GameObject spectatorCamera;
    public GameObject[] playerUIElements;
    public GameObject hitText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Death()
    {
        playerCamera.SetActive(false);
        spectatorCamera.SetActive(true);

        for (int i = 0; i < playerUIElements.Length; i++)
        {
            playerUIElements[i].SetActive(false);
        }
        hitText.SetActive(true);

        GetComponent<Transform>().position = new Vector3(0, 200, 0);
        GetComponent<PlayerMovement>().enabled = false;
    }
}
