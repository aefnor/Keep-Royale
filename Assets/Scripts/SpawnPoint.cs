using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpawnPoint : NetworkBehaviour

{
    public struct MyCustomData : INetworkSerializable
    {
        public bool hasPlayerSpawned;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref hasPlayerSpawned);
        }
    }
    [SerializeField] public NetworkVariable<MyCustomData> spawnData = new NetworkVariable<MyCustomData>(new MyCustomData { hasPlayerSpawned = false});

    public override void OnNetworkSpawn()
    {

        Debug.Log("PLAYER HAS SPAWNED: " + OwnerClientId);

        spawnData.OnValueChanged = (MyCustomData previousValue, MyCustomData currentValue) =>
        {
            Debug.Log("PLAYER HAS SPAWNED: " + OwnerClientId + " " + currentValue.hasPlayerSpawned);
        };
    }
    // Start is called before the first frame update
    void Start()
    {
        // spawnData.Value = new MyCustomData() { hasPlayerSpawned = false};
        // Debug.Log("HAS A PLAYER SPAWNED: " + this.gameObject.name + " " + spawnData.Value.hasPlayerSpawned);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
