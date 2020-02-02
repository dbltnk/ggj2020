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
    public float LengthSnapStart;
    public float LengthTarget;
    public float LengthSnapEnd;
    public float LengthMax;
    private AudioSource audioSource;
    public string PositionEffectName = "DistortionBeat";
    public string RotationEffectName = "CutoffBeat";
    public float StartRotationZ;
    public float DeltaToStartRotationZ;
    public float RotationSnap = 30f;
    private float OriginalScaleX;
    private float OriginalScaleZ;
    private float OriginalPositionZ;

    void Start ()
    {
        audioSource = GetComponent<AudioSource>();

        // Length and pitch
        if (Random.Range(0f, 1f) >= 0.5f) {
            DeltaLength = Random.Range(LengthMin, LengthSnapStart);
        }
        else {
            DeltaLength = Random.Range(LengthSnapEnd, LengthMax);
        }
        transform.localScale = new Vector3(transform.localScale.x, DeltaLength, transform.localScale.z);
        OriginalScaleX = transform.localScale.x;
        OriginalScaleZ = transform.localScale.z;

        // Position and Distortion
        TargetPosition = transform.position;
        DeltaDistance = Vector3.Distance(transform.position, TargetPosition);
        while (DeltaDistance <= PositionSnappingDistance) {
            float x = Random.Range(-MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
            float y = Random.Range(-MaxDistanceToTargetPosition, MaxDistanceToTargetPosition);
            DeltaPosition = new Vector3(x, y, transform.position.z);
            transform.position += DeltaPosition;
            DeltaDistance = Vector3.Distance(transform.position, TargetPosition);
        }
        OriginalPositionZ = transform.position.z;

        // Rotation and Cutoff
        StartRotationZ = transform.rotation.eulerAngles.z;
        DeltaToStartRotationZ = StartRotationZ - transform.rotation.eulerAngles.z;
        float deltaAbs = Mathf.Abs(DeltaToStartRotationZ);
        while (deltaAbs <= RotationSnap) {
            float r = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(0f, 0f, r);
            DeltaToStartRotationZ = StartRotationZ - transform.rotation.eulerAngles.z;
            deltaAbs = Mathf.Abs(DeltaToStartRotationZ);
        }
    }

    void Update()
    {
        // Length and pitch
        float sY = Mathf.Clamp(transform.localScale.y, LengthMin, LengthMax);
        if (sY <= LengthSnapEnd && sY >= LengthSnapStart) sY = LengthTarget;
        transform.localScale = new Vector3(OriginalScaleX, sY, OriginalScaleZ);
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
        transform.position = new Vector3(pX, pY, OriginalPositionZ);
        DeltaDistance = Vector3.Distance(transform.position, TargetPosition);
        if (DeltaDistance <= PositionSnappingDistance) transform.position = TargetPosition;
        float distort = Remap(DeltaDistance, 0f, 17.32f, 0f, 2f);
        audioSource.outputAudioMixerGroup.audioMixer.SetFloat(PositionEffectName, distort);

        // Rotation and Cutoff
        DeltaToStartRotationZ = StartRotationZ - transform.rotation.eulerAngles.z;
        float deltaAbs = Mathf.Abs(DeltaToStartRotationZ);
        if (deltaAbs <= RotationSnap) transform.rotation = Quaternion.Euler(0f, 0f, StartRotationZ);
        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z);
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
