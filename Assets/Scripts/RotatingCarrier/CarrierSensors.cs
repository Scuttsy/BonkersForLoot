// Ignore Spelling: Rigidbodies

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierSensors : MonoBehaviour
{
    [HideInInspector] public RotatingCarrier Carrier;
    [HideInInspector] public List<Rigidbody> Rigidbodies = new List<Rigidbody>();

    void OnTriggerEnter(Collider obj)
    {
        Rigidbody rb = obj.gameObject.GetComponentInParent<Rigidbody>();
        if (rb == null || rb.Equals(Carrier.CarrierRigidbody)) return;

        if (!Rigidbodies.Contains(rb)) Rigidbodies.Add(rb);
        Carrier.Add(rb);
    }

    void OnTriggerExit(Collider obj)
    {
        Rigidbody rb = obj.gameObject.GetComponentInParent<Rigidbody>();
        if (rb == null || rb.Equals(Carrier.CarrierRigidbody)) return;

        if (Rigidbodies.Contains(rb)) Rigidbodies.Remove(rb);
        Carrier.TryRemoveBasedOnSensors(rb);
    }
}
