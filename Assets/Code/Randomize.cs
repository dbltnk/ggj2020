using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Randomize : MonoBehaviour
{
    public Vector3 TargetPosition;
    public Vector3 DeltaPosition;
    public float MaxDistanceToTargetPosition;
    public float DeltaDistance;
    public float PositionSnappingDistance;
    public Quaternion DeltaRotation;
    public float DeltaLength;
    public float LengthMin;
    public float LengthTarget;
    public float LengthMax;
    private AudioSource audioSource;
    public string PositionEffectName = "DistortionBeat";
    public string RotationEffectName = "CutoffBeat";
    public float StartRotationZ;
    public float DeltaToStartRotationZ;

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
        // Rotation and Cutoff
        StartRotationZ = transform.rotation.eulerAngles.z;
        float r = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, r);
    }

    void Update()
    {
        // Length and pitch
        float sY = Mathf.Clamp(transform.localScale.y, LengthMin, LengthMax);
        if (sY <= LengthTarget * 1.5f && sY >= LengthTarget * 0.75f) sY = LengthTarget;
        transform.localScale = new Vector3(transform.localScale.x, sY, transform.localScale.z);
        DeltaLength = transform.localScale.y;
        if (DeltaLength == LengthTarget) {
            SyncAudioSources();
            audioSource.pitch = 1f;
        } else {
            audioSource.pitch = DeltaLength / 3f;
        }

        // Position and Distortion
        float pX = Mathf.Clamp(transform.position.x, -MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
        float pY = Mathf.Clamp(transform.position.y, -MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
        float pZ = Mathf.Clamp(transform.position.z, -MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
        transform.position = new Vector3(pX, pY, pZ);
        DeltaDistance = Vector3.Distance(transform.position, TargetPosition);
        if (DeltaDistance <= PositionSnappingDistance) transform.position = TargetPosition;
        float distort = Remap(DeltaDistance, 0f, 17.32f, 0f, 2f);
        audioSource.outputAudioMixerGroup.audioMixer.SetFloat(PositionEffectName, distort);

        // Rotation and Cutoff
        DeltaToStartRotationZ = StartRotationZ - transform.rotation.eulerAngles.z;
        float deltaAbs = Mathf.Abs(DeltaToStartRotationZ);
        if (deltaAbs <=30f) transform.rotation = Quaternion.Euler(0f, 0f, StartRotationZ);
        float cutoff = Remap(deltaAbs, 0f, 360f, 22000f, 0f);
        audioSource.outputAudioMixerGroup.audioMixer.SetFloat(RotationEffectName, cutoff);
    }

    float Remap (float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    void SyncAudioSources() {
        List<AudioSource> audiosources = new List<AudioSource>();
        audiosources = FindObjectsOfType<AudioSource>().ToList();
        int lastSample = 99999999;
        foreach (AudioSource a in audiosources) {
            if (lastSample == 99999999) {
                lastSample = a.timeSamples;
                a.timeSamples = 0;
            }
            if (a.timeSamples != lastSample) {
                a.timeSamples = lastSample;
            }
        }
    }

}
