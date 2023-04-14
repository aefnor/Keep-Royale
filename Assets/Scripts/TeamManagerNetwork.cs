using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TeamManagerNetwork : NetworkBehaviour
{
    public NetworkVariable<int> data = new NetworkVariable<int>(1);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
