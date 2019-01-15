using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float m_Speed;
    public float m_RotationSpeed;
    public Rigidbody2D m_Rbody;

    public Transform m_BulletSpawnPoint;
    public float m_FireRate;

    void Start()
    {
        GameProps.m_PC = this;
        GameProps.PLAYER_OBJECT = this.gameObject;

        GameObject temp = GameObject.Find("Loading_MAIN_TXT");

        if (temp != null)
            temp.SetActive(false);
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        m_Rbody.AddRelativeForce(Vector2.up * v * m_Speed);
        transform.Rotate(new Vector3(0, 0, -h) * Time.deltaTime * m_RotationSpeed);
        /*
        if (Input.GetKey(DEFAULT_VALUES.KEY_FIRE))
        {
            if (!IsInvoking("Fire"))
                Invoke("Fire", m_FireRate);
        }
        */
    }

    /*
    private void Fire()
    {
        PhotonNetwork.Instantiate("NetBullet", m_BulletSpawnPoint.position, transform.rotation, 0);
    }
    */
}
