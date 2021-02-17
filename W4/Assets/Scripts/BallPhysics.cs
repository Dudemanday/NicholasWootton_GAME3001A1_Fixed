using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    //vectors used for calculations of direction and velocity
    private Vector3 m_vPlayerPos;
    private Vector3 m_vTargetPos;
    private Vector3 m_vInitialVel;

    //kick power of the player to the ball
    private float m_fKickPower;

    //player needs to be within this number of units to be able to kick the ball
    private float m_fPlayerKickMaxDistance;

    //debug mode
    private bool m_debugMode = false;

    //reference to the player, used to calc direction when kicked
    [SerializeField]
    private GameObject m_PlayerRef = null;

    //reference to an empty game object set in the middle of the field, on ball reset, it is teleport to this object
    [SerializeField]
    private GameObject m_CenterFieldRef = null;

    //rigid body of the ball
    private Rigidbody m_rb = null;

    //target reference for debug mode for where the ball is travelling
    private GameObject m_targetDisplay = null;

    //used for checking if ball is in air or on the ground
    //used for preventing the ball from being kicked if its in the air
    private bool m_bIsGrounded = true;
    private float m_fTimeInAir = 0;

    //debug mode for where the ball is aiming
    private Vector3 vDebugHeading;


    // Start is called before the first frame update
    void Start()
    {
        //get rigib body from ball
        m_rb = GetComponent<Rigidbody>();
        Assert.IsNotNull(m_rb, "projectileComp is null");

        //does debug mode things, if enabled
        CreateLandingDisplay();

        //sets up initial value
        m_fPlayerKickMaxDistance = 10;
    }

    // Update is called once per frame
    void Update()
    {
        //gets position of the player and sets to the variable, for future calcs
        m_vPlayerPos = m_PlayerRef.transform.position;

        //if the ball has been in the air long enough and has gone back down the ground (y = 0.6)
        //then reset target to under the ground
        if (m_fTimeInAir == 20 && transform.position.y <= 0.6f)
        {
            m_bIsGrounded = true;
            m_targetDisplay.transform.position = new Vector3(0.0f, -1.0f, 0.0f);
        }
        else if (m_fTimeInAir < 20) m_fTimeInAir++;

        //reset ball if it goes out of bounds
        if (transform.position.y <= 0)
        {
            Reset();
        }

        //sets target offscreen if debug mode is off
        if (!m_debugMode)
        {
            m_targetDisplay.transform.position = new Vector3(0.0f, -1.0f, 0.0f);
        }
    }

    //creates sphere for debug, to see where the ball is going
    private void CreateLandingDisplay()
    {
        m_targetDisplay = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_targetDisplay.transform.position = new Vector3(0.0f, -1.0f, 0.0f);
        m_targetDisplay.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        m_targetDisplay.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        m_targetDisplay.GetComponent<Renderer>().material.color = Color.red;
        m_targetDisplay.GetComponent<Collider>().enabled = false;
    }

    //reset ball, called from FirstPerson.cs, from the 'R' key
    public void resetBall()
    {
        Reset();
    }

    //returns kick power, used in UI
    public float getKickPower()
    {
        return m_fKickPower;
    }

    //sets kick power from UI
    public void setKickPower(float kickPower)
    {
        m_fKickPower = kickPower;
    }

    //toggles debug, called from FirstPerson.cs, from the 'T' key
    public void toggleDebugMode()
    {
        m_debugMode = !m_debugMode;
    }

    //private reset func, resets ball
    private void Reset()
    {
        transform.position = m_CenterFieldRef.transform.position;
        m_rb.velocity = Vector3.zero;
    }

    //Kicks ball
    public void OnKickBall()
    {
        //checks if player is close enough to the ball
        if ((transform.position - m_vPlayerPos).magnitude < m_fPlayerKickMaxDistance)
        {
            //checks if ball is on the ground
            if (m_bIsGrounded)
             {  
                //resets vars used to check if ball is in air or not
                m_bIsGrounded = false;
                m_fTimeInAir = 0;
                
                //Setup target position
                m_vTargetPos = transform.position + (transform.position - m_vPlayerPos);
                m_targetDisplay.transform.position = m_vTargetPos;
                m_vTargetPos.y = 1.0f;

                //setup debug heading line
                vDebugHeading = m_vTargetPos - transform.position;

                //-------------------------------------------------------
                //make velocity from distance the ball needs to travel
                //-------------------------------------------------------

                //max height
                float fMaxHeight = m_targetDisplay.transform.position.y;

                //-----------
                //For Y-Axis and Z-Axis
                //-----------

                //distance ball needs to travel on the YZ axis, X axis is done seperately
                Vector3 distance = m_targetDisplay.transform.position - transform.position;
                distance.x = 0.0f;
                float m_fDistanceToTargetYZ = (distance).magnitude;

                //total range of motion for YZ axis
                float fRange = (m_fDistanceToTargetYZ * 2);

                //theta = tan^-1(4h/r)
                float fTheta = Mathf.Atan((4 * fMaxHeight) / fRange);

                //Vi = sqrt(2gh) / sin(tan^-1(4h/r))
                float fInitVelMagYZ = Mathf.Sqrt((2 * Mathf.Abs(Physics.gravity.y) * fMaxHeight)) / Mathf.Sin(fTheta);

                // Vy = V * sin(theta)
                m_vInitialVel.y = fInitVelMagYZ * Mathf.Sin(fTheta);
                // Vx = V * cos(theta)
                m_vInitialVel.z = fInitVelMagYZ * Mathf.Cos(fTheta);

                //swaps z velocity if the ball needs to go the other way
                if (transform.position.z < m_vPlayerPos.z)
                {
                    m_vInitialVel.z = -m_vInitialVel.z;
                }

                //-----------
                //For X-Axis
                //-----------

                //distance ball needs to travel
                distance = m_targetDisplay.transform.position - transform.position;
                distance.z = 0.0f;
                float m_fDistanceToTargetXY = (distance).magnitude;

                //total range of motion for XY axir
                float fRangeXY = (m_fDistanceToTargetXY * 2);
                //theta = tan^-1(4h/r)
                float fThetaXY = Mathf.Atan((4 * fMaxHeight) / fRangeXY);
                //Vi = sqrt(2gh) / sin(tan^-1(4h/r))
                float fInitVelMagXY = Mathf.Sqrt((2 * Mathf.Abs(Physics.gravity.y) * fMaxHeight)) / Mathf.Sin(fThetaXY);

                // Vx = V * cos(theta)
                m_vInitialVel.x = fInitVelMagXY * Mathf.Cos(fThetaXY);

                //swaps x velocity if the ball needs to go the other way
                if (transform.position.x < m_vPlayerPos.x)
                {
                    m_vInitialVel.x = -m_vInitialVel.x;
                }

                //-----------

                //sets the balls velocity to the velocity that was just calculated
                m_rb.velocity = m_vInitialVel;
            }
        }
    }

    //draws debug line 
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + vDebugHeading, transform.position);
    }
}