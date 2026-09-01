using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VolumeController : MonoBehaviour
{
    // Start is called before the first frame update
    public Volume volume;
    public DepthOfField dof;

    public float focusDistance;

    void Start()
    {
        volume.profile.TryGet(out dof);
    }

    void Update()
    {
        if (dof != null)
        {
            dof.focusDistance.value = focusDistance;
        }
    }

}
