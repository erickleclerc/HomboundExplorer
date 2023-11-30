using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

//NetworkBeahviour is a class that inherits from MonoBehaviour plus some extra networking functionality like Ownership and Spawn individual controls as well as properties like IsServer, IsClient, IsOwner, etc.
public class PlayerNetwork : NetworkBehaviour
{
    private float moveSpeed = 3f;

    //must be used in a NetworkBehaviour. Must be initialized. Everyone can read it, only the owner can write it
    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(new MyCustomData { someInt = 99, someFloat = 127.99f, someString = "Erick" }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    //structs are value types, so they are copied when passed as arguments to methods or when assigned to other variables. They are not passed by reference like classes. INetworkSerializable is an interface that allows us to serialize and deserialize data to send it over the network
    public struct MyCustomData : INetworkSerializable
    {
        public int someInt;
        public float someFloat;
        public FixedString128Bytes someString;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref someInt);
            serializer.SerializeValue(ref someFloat);
            serializer.SerializeValue(ref someString);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + " Random Number is: " + newValue.someInt + " and " + newValue.someString);
        };
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            //get the audiosource of this gameobject and play it
            GetComponent<AudioSource>().Play();
        }

        //If this is not the owner of this object, then don't do anything. Doesn't matter if it's a host or client
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            randomNumber.Value = new MyCustomData
            {
                someInt = Random.Range(1, 100),
                someFloat = Random.Range(25f, 75f),
                someString = "Erickson" + Random.Range(1, 10).ToString()
            }
                 ;
        }


        //only the owner of this object can move it
        Vector3 moveDirection = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection.z = +1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection.z = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.x = +1;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

    }
}
