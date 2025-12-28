using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotForever : MonoBehaviour
{
    public float wSpeed = 720f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        Vector3 rot = this.transform.localEulerAngles;
        rot.z += (this.wSpeed * Time.deltaTime);
        this.transform.localEulerAngles = rot;
    }
}
