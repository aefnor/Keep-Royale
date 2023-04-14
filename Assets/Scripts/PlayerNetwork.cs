using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

    struct MyCustomData : INetworkSerializable
    {
        public FixedString512Bytes message;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref message);
        }
    }

    [SerializeField] private Transform spawnObjectPrefab;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject teamManagerObject;
    private TeamManagerNetwork teamManager;
    private Transform spawnedObjectTransform;

    private NetworkVariable<MyCustomData> data = new NetworkVariable<MyCustomData>(new MyCustomData { message = "" });
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
        teamManager = teamManagerObject.GetComponent<TeamManagerNetwork>();
    }

    public override void OnNetworkSpawn()
    {
        playerCamera = Instantiate(playerCamera);
        playerCamera.GetComponent<NetworkObject>().Spawn(true);
        
        Debug.Log("PLAYER HAS SPAWNED: " + OwnerClientId);

        data.OnValueChanged = (MyCustomData previousValue, MyCustomData currentValue) =>
        {
            Debug.Log("PLAYER HAS SPAWNED: " + OwnerClientId + " " +currentValue.message);
        };
    }

    public void LateUpdate () {
        Vector3 offset = new Vector3(0,-2,3);
        if (!this.GetComponent<NetworkObject>().IsLocalPlayer)
        {
            Debug.Log("disabled camera" + OwnerClientId);
            playerCamera.GetComponent<Camera>().enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
        }
        else  {
            Debug.Log("enabled camera" + OwnerClientId);
            playerCamera.GetComponent<Camera>().enabled = true;
            playerCamera.GetComponent<AudioListener>().enabled = true;
        }
        playerCamera.transform.position = this.transform.position - offset;
    }

    // Update is called once per frame
    void Update()
    {
        //Cant run code if script not attached to the owner
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            teamManager.data.Value = 5;
            Debug.Log(teamManager.data.Value);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            TestDespawnServerRpc("Despawn", new ServerRpcParams());
        }

            if (Input.GetKeyDown(KeyCode.T)) 
        {
            TestSpawnServerRpc("Spawning ball", new ServerRpcParams());
            //RUNS ONLY ON SERVER
            TestServerRpc("test", new ServerRpcParams());
            //TestClientRpc();
            data.Value = new MyCustomData
            {
                message = "Test"
            };
        }


        Vector3 moveDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) { moveDir.z = +1f; }
        if (Input.GetKey(KeyCode.S)) { moveDir.z = -1f; }
        if (Input.GetKey(KeyCode.A)) { moveDir.x = -1f; }
        if (Input.GetKey(KeyCode.D)) { moveDir.x = +1f; }
        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    //Code called from the server to then run on the clients
    [ClientRpc]
    private void TestClientRpc()
    {
        Debug.Log("Test Client Rpc" + OwnerClientId);
        
    }

    [ClientRpc]
    private void TestDespawnClientRpc()
    {
    }


    //If a client ran this the server RPC ONLY runs on the server. A host can run this since it's both a client and a server
    [ServerRpc]
    private void TestServerRpc(string message, ServerRpcParams param)
    {
        Debug.Log("Test Server Rpc" + OwnerClientId + " message: " + message + " params: " + param.Receive.SenderClientId);
    }

    [ServerRpc]
    private void TestSpawnServerRpc(string message, ServerRpcParams param)
    {
        Debug.Log("Test Server Rpc" + OwnerClientId + " message: " + message + " params: " + param.Receive.SenderClientId);
        spawnedObjectTransform = Instantiate(spawnObjectPrefab);
        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        TestClientRpc();
    }

    [ServerRpc]
    private void TestDespawnServerRpc(string message, ServerRpcParams param)
    {
        Debug.Log("Test Server Rpc" + OwnerClientId + " message: " + message + " params: " + param.Receive.SenderClientId);
        Destroy(spawnedObjectTransform.gameObject);
        TestDespawnClientRpc();
    }
}
