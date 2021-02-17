using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class ProjectileComponent : MonoBehaviour
{
    [SerializeField]
    private Vector3 m_intitialVeclocity = Vector3.zero;

    private Rigidbody m_rb = null;
    private GameObject m_landingDisplay = null;

    private bool m_isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        Assert.IsNotNull(m_rb, "projectileComp is null");

        CreateLandingDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLandingPosition();
    }

    private void CreateLandingDisplay()
    {
        m_landingDisplay = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        m_landingDisplay.transform.position = Vector3.zero;
        m_landingDisplay.transform.localScale = new Vector3(1.0f, 0.1f, 1.0f);

        m_landingDisplay.GetComponent<Renderer>().material.color = Color.blue;
        m_landingDisplay.GetComponent<Collider>().enabled = false;
    }

    public void OnLaunchProjectile()
    {
        if(!m_isGrounded)
        {
            return;
        }

        m_landingDisplay.transform.position = GetLandingPosition();
        m_isGrounded = false;

        transform.LookAt(m_landingDisplay.transform.position, Vector3.up);

        m_rb.velocity = m_intitialVeclocity;
    }

    private void UpdateLandingPosition()
    {
        if(m_landingDisplay != null && m_isGrounded)
        {
            m_landingDisplay.transform.position = GetLandingPosition();
        }

    }

    private Vector3 GetLandingPosition()
    {
        float fTime = 2f * (0.0f - m_intitialVeclocity.y / Physics.gravity.y);

        Vector3 vFlatVel = m_intitialVeclocity;
        vFlatVel.y = 0.0f;
        vFlatVel *= fTime;

        return transform.position + vFlatVel;
    }

    public void OnMoveForward(float fDelta)
    {
        m_intitialVeclocity.z += fDelta;
    }
    public void OnMoveBackward(float fDelta)
    {
        m_intitialVeclocity.z -= fDelta;
    }
    public void OnMoveLeft(float fDelta)
    {
        m_intitialVeclocity.x += fDelta;
    }
    public void OnMoveRight(float fDelta)
    {
        m_intitialVeclocity.x -= fDelta;
    }
}
