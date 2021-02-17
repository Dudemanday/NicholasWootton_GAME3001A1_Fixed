using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    //ball reference
    [SerializeField]
    private BallPhysics m_pBallRef = null;

    //kick power value from slider on UI
    [SerializeField]
    private TMP_Text KickPowerValue; 

    //slider reference
    Slider kickPowerSlider;

    // Use this for initialization
    void Start()
    {
        //gets slider object and checks if valid
        GameObject slider = GameObject.Find("KickPowerSlider");
        if (slider != null)
        {
            kickPowerSlider = slider.GetComponent<Slider>();
        }
        //sets initial value ot value from the ball
        kickPowerSlider.value = 1.0f;// m_pBallRef.getKickPower();
        m_pBallRef.setKickPower(1.0f);// kickPowerSlider.value);
    }

    // Update is called once per frame
    void Update()
    {
        //sets text to slider value
        KickPowerValue.SetText("" + kickPowerSlider.value);
        //update the balls kick power based on slider
        m_pBallRef.setKickPower(1.0f);// kickPowerSlider.value);
    }
}
