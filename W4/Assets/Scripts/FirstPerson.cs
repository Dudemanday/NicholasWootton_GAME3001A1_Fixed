using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;

public class FirstPerson : MonoBehaviour
{
    [SerializeField]
    private BallPhysics m_ballRef = null;

    [SerializeField]
    private bool controllerEnabled = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        checkKeys();
        GameObject.Find("Player").GetComponent<FirstPersonController>().enabled = controllerEnabled;
    }

    //keyboard input
    private void checkKeys()
    {
        if (Input.GetKeyDown("space"))
        {
            m_ballRef.OnKickBall();
        }
        if (Input.GetKeyDown("r"))
        {
            m_ballRef.resetBall();
        }
        if (Input.GetKeyDown("t"))
        {
            m_ballRef.toggleDebugMode();
        }
    }
}
