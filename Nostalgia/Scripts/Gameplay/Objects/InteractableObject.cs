using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class InteractableObject : NetworkBehaviour
{
    //interact를 시도한 player에게서 호출되는 virtual 함수
    public virtual void OnInteract(NetworkObject playerObject)
    {
        
    }
}
