using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetHitDetection : MonoBehaviour
{
    public string m_PlanetName = "_Planet";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameProps.m_NetPC.photonView.isMine)
        {
            if (m_PlanetName == "")
                return;

            if (GameProps.m_LastHitPlanet != null)
                GameProps.m_LastHitPlanet.SetActive(false);

            GameProps.m_LastHitPlanet = transform.GetChild(0).gameObject;
            GameProps.m_LastHitPlanet.SetActive(true);

            GameProps.m_IsPlanetChosen = true;
            GameProps.m_PlanetName = m_PlanetName;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKey(KeyCode.F))
            PhotonNetwork.LeaveRoom();
    }
}