using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomize : MonoBehaviour
{
    public Vector3 TargetPosition;
    public Vector3 DeltaPosition;
    public float MaxDistanceToTargetPosition = 5f;
    public float DeltaDistance;
    public float PositionSnappingDistance = 1.5f;
    public Quaternion DeltaRotation;
    public float DeltaLength;
    public float LengthMin = 0.5f;
    public float LengthTarget = 1f;
    public float LengthMax = 3f;
    private AudioSource audioSource;
    public string EffectName = "DistortionBeat";

    void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        // Length and pitch
        DeltaLength = Random.Range(LengthMin, LengthMax);
        transform.localScale = new Vector3(transform.localScale.x, DeltaLength, transform.localScale.z);
        // Position and Distortion
        TargetPosition = transform.position;
        float x = Random.Range(-MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
        float y = Random.Range(-MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
        float z = Random.Range(-MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
        DeltaPosition = new Vector3(x, y, z);
        transform.position += DeltaPosition;
    }

    void Update()
    {
        // Length and pitch
        float sY = Mathf.Clamp(transform.localScale.y, LengthMin, LengthMax);
        if (sY <= LengthTarget * 1.5f && sY >= LengthTarget * 0.75f) sY = LengthTarget;
        transform.localScale = new Vector3(transform.localScale.x, sY, transform.localScale.z);
        DeltaLength = transform.localScale.y;
        audioSource.pitch = Remap(DeltaLength, 0.5f, 15f, 0.25f, 3f);
        // Position and Distortion
        float pX = Mathf.Clamp(transform.position.x, -MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
        float pY = Mathf.Clamp(transform.position.y, -MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
        float pZ = Mathf.Clamp(transform.position.z, -MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
        transform.position = new Vector3(pX, pY, pZ);
        DeltaDistance = Vector3.Distance(transform.position, TargetPosition);
        if (DeltaDistance <= PositionSnappingDistance) transform.position = TargetPosition;
        float distort = Remap(DeltaDistance, 0f, 17.32f, 0f, 2f);
        audioSource.outputAudioMixerGroup.audioMixer.SetFloat(EffectName, distort);
    }

    float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}
