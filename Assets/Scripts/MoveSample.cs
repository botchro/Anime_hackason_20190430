using UnityEngine;
using System.Collections;

public class MoveSample : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        System.Random rand = new System.Random();
        this.transform.position = this.transform.position + new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble());
    }
}
