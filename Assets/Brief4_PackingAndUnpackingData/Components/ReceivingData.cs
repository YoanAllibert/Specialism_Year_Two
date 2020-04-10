using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceivingData : MonoBehaviour
{
    [Header("Host Connection")]
    public DataPacking host;
    private int numberOfPlayers;
    private int numberOfPickups;

    [Header("Datas Reconstructed from Host")]
    public byte[] playersId;
    public bool[] playerState;
    public Vector2[] playerPosition;
    public Vector2[] playerVelocity;

    [Space(20f)]

    public byte[] pickupId;
    public bool[] pickupState;
    public Vector2[] pickupPosition;

    [Space(20f)]

    public List<Vector2> missilePosition;
    public List<Vector2> missileVelocity;

    [HideInInspector]
    public struct Missile
    {
        public Vector2 PositionMissile;
        public Vector2 VelocityMissile;
    }
    [HideInInspector]
    public Missile lastMissile = new Missile();

    void OnEnable() 
    {
        host.OnEndTickServer += TickServer;
    }

    void Start()
    {
        // Set Data From Host
        numberOfPlayers = host.numberOfPlayers;
        numberOfPickups = host.numberOfPickups;

        // Initialise Size of Data
        playersId = new byte[numberOfPlayers];
        playerState = new bool[numberOfPlayers];
        playerPosition = new Vector2[numberOfPlayers];
        playerVelocity = new Vector2[numberOfPlayers];

        pickupId = new byte[numberOfPickups];
        pickupState = new bool[numberOfPickups];
        pickupPosition = new Vector2[numberOfPickups];

        // Get all pickups positions on Start
        GetPickUpPosition();

        // Initialize Player ID Once
        InitPlayerID();

        // Call a Tick On Start
        TickServer();

        // Initialize last missile struct
        lastMissile.PositionMissile = Vector2.zero;
        lastMissile.VelocityMissile = Vector2.zero;
    }

    private void TickServer()
    {
        UpdatePlayersData();
        UpdatePickUpData();
        UpdateMissileData();
    }

    private void GetPickUpPosition()
    {
        for (int i = 0; i < numberOfPickups; i++) // Go through each pickups
        {
            BitArray pickUpData = host.pickupStartPosition[i]; // Copy whole array (include Id, State, Position)

            BitArray pickupArrayID = new BitArray(8); //Copy Id
            BitArray pickupArrayPosition = new BitArray(32);
            
            for (int j = 0; j < 8; j++) // Unpack ID (bits 0 to 8)
            {
                pickupArrayID[j] = pickUpData[j];
            }
            for (int k = 0; k < 32; k++) // Unpack Position (bits 9 to 41)
            {
                pickupArrayPosition[k] = pickUpData[9 + k];
            }

            // Convert and save as data
            pickupId[i] = host.BitarrayToByte(pickupArrayID);
            pickupPosition[i] = host.BitarrayToVector2Short(pickupArrayPosition);
        }
    }

    private void InitPlayerID()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            playersId[i] = (byte)i;
        }
    }

    private void UpdatePlayersData()
    {
        // Divide Host Bitarray into state, position, velocity
        for (int i = 0; i < numberOfPlayers; i++)
        {
            BitArray playerData = host.playerBitarray[i]; // 57 bits with all data

            BitArray playerNewPosition = new BitArray(32); // 32 bits for position
            BitArray playerNewVelocity = new BitArray(16); // 16 bits for velocity

            if (!playerData[8]) //if player is alive
            {
                for (int j = 0; j < 32; j++) // Unpack Positions
                {
                    playerNewPosition[j] = playerData[9 + j];
                }
                for (int k = 0; k < 16; k++) // Unpack Velocities
                {
                    playerNewVelocity[k] = playerData[41 + k];
                }

                playerPosition[i] = host.BitarrayToVector2Short(playerNewPosition);
                playerVelocity[i] = host.BitarrayToVector2Byte(playerNewVelocity);
            }

            // Convert state
            playerState[i] = playerData[8];
        }
    }

    private void UpdatePickUpData()
    {
        //Copy each Bitarray with possibly new State
        for (int i = 0; i < numberOfPickups; i++)
        {
            BitArray pickUpUpdated = host.pickupBitarray[i];
            pickupState[i] = pickUpUpdated[8]; // Copy bit index 8 (state)
        }
    }

    private void UpdateMissileData()
    {
        if (host.missileBitarray.Count != 0) // If missiles have been fired this tick
        {
            for (int i = 0; i < host.missileBitarray.Count; i++)
            {
                BitArray missileData = host.missileBitarray[i]; // Copy Data from Host

                BitArray missileNewPosition = new BitArray(32); // 32 bits for position
                BitArray missileNewVelocity = new BitArray(16); // 16 bits for velocity

                for (int j = 0; j < 32; j++) // Unpack Positions
                {
                    missileNewPosition[j] = missileData[j];
                }
                for (int k = 0; k < 16; k++) // Unpack Velocities
                {
                    missileNewVelocity[k] = missileData[32 + k];
                }

                missilePosition.Add(host.BitarrayToVector2Short(missileNewPosition));
                missileVelocity.Add(host.BitarrayToVector2Byte(missileNewVelocity));
            }

            // Update last missile struct
            lastMissile.PositionMissile = missilePosition[missilePosition.Count - 1];
            lastMissile.VelocityMissile = missileVelocity[missileVelocity.Count - 1];
        }
        else // Otherwise clear list
        {
            missilePosition.Clear();
            missileVelocity.Clear();
        }
    }

    void OnDisable() 
    {
        host.OnEndTickServer -= TickServer;
    }
}