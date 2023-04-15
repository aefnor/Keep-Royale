using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System;


public struct PlayerData : INetworkSerializable, IEquatable<PlayerData> {
    public FixedString512Bytes playerName;
    public ulong clientId;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref clientId);
    }
    public bool Equals(PlayerData other)
    {
        return playerName == other.playerName && clientId.Equals(other.clientId);
    }
}
public class TeamManagerNetwork : NetworkBehaviour
{
    public NetworkList<PlayerData> playerData;

    public override void OnNetworkSpawn () {
        playerData.Initialize(this);
        playerData = new NetworkList<PlayerData>();
    }

    // Use this method to add a new PlayerData to the list
    public void AddPlayerData(PlayerData data)
    {
        playerData.Add(data);
    }

    // Use this method to remove a PlayerData from the list
    public void RemovePlayerData(PlayerData data)
    {
        playerData.Remove(data);
    }
    void Awake () {
        playerData = new NetworkList<PlayerData>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
