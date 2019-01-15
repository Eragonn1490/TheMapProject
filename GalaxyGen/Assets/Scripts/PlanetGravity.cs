using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGravity : MonoBehaviour {
    public float pullRadius = 2;
    public float pullForce = 1;

    private void Start () {
        CircleCollider2D _CC2D = GetComponent<CircleCollider2D> ();

        _CC2D.radius = transform.GetComponentInParent<CapsuleCollider2D> ().size.x / 2 + pullRadius;
        _CC2D.isTrigger = true;
    }

    private void OnTriggerStay2D (Collider2D collision) {
        /*
        if (GameProps.m_NetPC.photonView.isMine) {
            Vector3 forceDirection = transform.position - collision.transform.position;
            collision.GetComponent<Rigidbody2D> ().AddForce (forceDirection.normalized * pullForce * Time.fixedDeltaTime);
        }
        */
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (GameProps.m_NetPC.photonView.isMine)
    //    {
    //        collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    //    }
    //}
}