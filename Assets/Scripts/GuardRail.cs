using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardRail : MonoBehaviour
{
    public Transform RailPivot;
    public Transform EntryPoint1;
    public Transform EntryPoint2;
    [HideInInspector]
    public float AnimSpeed;
    public float AnimSpeedMultiplier = 15f;

    [HideInInspector]
    public bool StartsAt1;
    [HideInInspector]
    public bool StartAnimation;
    [HideInInspector]
    public Transform PlayerTransform;

    [SerializeField] private float _startAngle = -3f;
    [SerializeField] private float _endAngle = -3f;
    [SerializeField] private Transform _distanceDecider;

    private void Start()
    {
        _startAngle += 180f + transform.eulerAngles.y;
        _endAngle += 180f + transform.eulerAngles.y;
    }

    public void SetStartingAngle()
    {
        RailPivot.eulerAngles = StartsAt1 ? new Vector3(0, _startAngle, 0) : new Vector3(0, _endAngle, 0);
    }

    private void Update()
    {
        if (!StartAnimation) return;
        RailPivot.Rotate(Vector3.up, (StartsAt1 ? 1 : -1) * Time.deltaTime * AnimSpeed);

        if (PlayerTransform == null)
        {
            Debug.LogError("PlayerTransform is not set!");
            return;
        }

        if (transform.eulerAngles.y < 45f+180f)
        {

        }

        PlayerTransform.position = RailPivot.position
              + (new Vector3(
                  Mathf.Sin(RailPivot.eulerAngles.y * Mathf.Deg2Rad),
                  0,
                  Mathf.Cos(RailPivot.eulerAngles.y * Mathf.Deg2Rad))) * -_distanceDecider.localPosition.z * transform.localScale.x;
    }
}
