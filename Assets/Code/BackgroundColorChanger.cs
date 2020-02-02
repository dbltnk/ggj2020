using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BackgroundColorChanger : MonoBehaviour
{
    List<Randomize> objects = new List<Randomize>();
    Camera c;

    void Start()
    {
        objects = FindObjectsOfType<Randomize>().ToList();
        c = GetComponent<Camera>();
    }

    void Update()
    {
        float count = 0f;
        foreach (Randomize r in objects) {
            if (r.Done) count++;
        }
        float n = 1f / 3f;
        float m = (3f - count);
        c.backgroundColor = new Color(n * m, n * m, n * m);
    }
}
