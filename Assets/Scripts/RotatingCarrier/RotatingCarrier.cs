// Ignore Spelling: Rigidbodies Rigidbody

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCarrier : MonoBehaviour
{
    public float RotationSpeed;
    public bool UseTriggerAsSensor;
    public bool OnlyRotate;
    public Rigidbody CarrierRigidbody;
    public List<Rigidbody> Rigidbodies = new List<Rigidbody>();
    //public List<PlayerMovementController> PlayerMovementControllers = new List<PlayerMovementController>();

    private Vector3 _lastEulerAngles;
    private List<CarrierSensors> _sensors = new List<CarrierSensors>();
    private float _CurrentYRotation;
    [HideInInspector] public float AngularVelocity;

    // Start is called before the first frame update
    void Start()
    {
        float xSign = Mathf.Sign(transform.localScale.x);
        float ySign = Mathf.Sign(transform.localScale.y);
        float zSign = Mathf.Sign(transform.localScale.z);

        RotationSpeed *= -1 * xSign * ySign * zSign;

        _lastEulerAngles = transform.eulerAngles;

        if (OnlyRotate) return;
        if (!UseTriggerAsSensor) return;
        foreach (CarrierSensors sensor in GetComponentsInChildren<CarrierSensors>())
        {
            sensor.Carrier = this;
            _sensors.Add(sensor);
        }

        if (_sensors.Count == 0)
        {
            Debug.LogWarning("Carrier "+name+ " has UseTriggerAsSensor set to true, but no sensors found");
        }
    }

    private void FixedUpdate()
    {
        //transform.Rotate(Vector3.up, RotationSpeed * Time.deltaTime);
        _CurrentYRotation += RotationSpeed * Time.fixedDeltaTime;
        CarrierRigidbody.MoveRotation(Quaternion.Euler(0,_CurrentYRotation,0));
    }

    private void LateUpdate()
    {
        if (Rigidbodies.Count <= 0) return;
        if (OnlyRotate) return;

        Vector3 angularVelocity = (transform.eulerAngles - _lastEulerAngles);
        AngularVelocity = angularVelocity.y / Time.deltaTime;
        foreach (Rigidbody rb in Rigidbodies)
        {
            //RotateRigidbody(rb, Vector3.Distance(CarrierRigidbody.position, rb.position), angularVelocity.y);
        }

        _lastEulerAngles = transform.eulerAngles;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (OnlyRotate) return;
        if (UseTriggerAsSensor) return;
        Rigidbody rb = collision.collider.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            Add(rb);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (OnlyRotate) return;
        if (UseTriggerAsSensor) return;
        Rigidbody rb = collision.collider.GetComponentInParent<Rigidbody>();
        if (rb != null)
        {
            Remove(rb);
        }
    }

    public void Add(Rigidbody rb)
    {
        if (Rigidbodies.Contains(rb)) return;
        Rigidbodies.Add(rb);
        if (rb.gameObject.TryGetComponent<PlayerMovementController>(out var playerMovementScript))
        {
            playerMovementScript.RotatingCarrier = this;
        }
    }

    public void Remove(Rigidbody rb)
    {
        if (!Rigidbodies.Contains(rb)) return;
        Rigidbodies.Remove(rb);
        if (rb.gameObject.TryGetComponent<PlayerMovementController>(out var playerMovementScript))
        {
            playerMovementScript.RotatingCarrier = null;
        }
    }

    public bool TryRemoveBasedOnSensors(Rigidbody rb)
    {
        foreach (CarrierSensors sensor in _sensors)
        {
            if (sensor.Rigidbodies.Contains(rb))
            {
                return false;
            }
        }

        Remove(rb);
        if (rb.gameObject.TryGetComponent<PlayerMovementController>(out var playerMovementScript))
        {
            playerMovementScript.RotatingCarrier = null;
        }
        //PlayerMovementControllers.Remove(rb.gameObject.GetComponent<PlayerMovementController>());
        return true;
    }

    private Vector3 _debug;

    public void RotateRigidbody(Rigidbody rb, float distanceFromCenter, float amount)
    {
        Vector3 debug = rb.position;

        float currentAngle = Mathf.Atan2(rb.position.z - CarrierRigidbody.position.z, rb.position.x - CarrierRigidbody.position.x) * Mathf.Rad2Deg;
        float newAngle = currentAngle + amount;
        float newAngleInRad = newAngle * Mathf.Deg2Rad;
        Vector3 newLocalPosition = new Vector3(Mathf.Cos(newAngleInRad), rb.position.y, Mathf.Sin(newAngleInRad)) *
                                   distanceFromCenter;
        _debug = CarrierRigidbody.position + newLocalPosition;

        Debug.DrawLine(debug, rb.position, Color.black, 15f);
    }
}
