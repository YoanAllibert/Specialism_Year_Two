using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DataPacking : MonoBehaviour
{
    [Header("Host Settings")]
    public int tickPerSeconds = 30;

    [Header("Game Settings")]
    public int numberOfPlayers = 8;
    public int numberOfPickups = 25;

    [Header("Datas (Read Only)")]
    public byte[] playersId;
    public bool[] playersState;
    public Vector2[] playerPosition;
    public Vector2[] playerVelocity; 
    public BitArray[] playerBitarray;

    [Space(20f)]

    public byte[] pickupId;
    public Vector2[] pickupPosition;
    public bool[] pickupState;
    public BitArray[] pickupStartPosition;
    public BitArray[] pickupBitarray;

    [Space(20f)]

    public List<int> newMissiles = new List<int>();
    public List<Vector2> missilePosition = new List<Vector2>();
    public List<Vector2> missileVelocity = new List<Vector2>();
    public List<BitArray> missileBitarray = new List<BitArray>();

    public event Action OnStartTickServer;
    public event Action OnEndTickServer;

    [Header("Bits sent statistic")]
    public int bitsPerTick = 0;

    void Awake()
    {
        // Start Host Tick rate
        float repeat = 1f / tickPerSeconds;
        InvokeRepeating("Tick", repeat, repeat);

        // Create Arrays for players and Pickups
        playersId = new byte[numberOfPlayers];
        playersState = new bool[numberOfPlayers];
        playerPosition = new Vector2[numberOfPlayers];
        playerVelocity = new Vector2[numberOfPlayers];
        playerBitarray = new BitArray[numberOfPlayers];

        pickupId = new byte[numberOfPickups];
        pickupState = new bool[numberOfPickups];
        pickupPosition = new Vector2[numberOfPickups];
        pickupStartPosition = new BitArray[numberOfPickups];
        pickupBitarray = new BitArray[numberOfPickups];

        // Send PickUps Location
        CreatePickUpPosition();

        // Initialize Players
        CreatePlayersArray();

        // Call a first Tick early
        Tick();
    }

    private void Tick()
    {
        bitsPerTick = 0;

        // Call the Tick Event Before Update
        if (OnStartTickServer != null)
            OnStartTickServer();

        // Clear and add new Missiles
        ClearAllMissile();
        AddMissiles();

        // Update each Bitarray
        UpdateBitarrayOfPlayers();
        UpdateBitarrayOfPickup();
        UpdateBitarrayNewMissile();

        // Send Event to Clients After update
        if (OnEndTickServer != null)
            OnEndTickServer();
    }

    

    private void CreatePickUpPosition()
    {
        for (byte i = 0; i < numberOfPickups; i++) // Go through each pickups
        {
            pickupId[i] = i; // Initialize ID array
            pickupState[i] = false;

            pickupStartPosition[i] = new BitArray(new byte[] { i }); // Create bitarray with byte ID for each pickup
            pickupStartPosition[i].Length = 41; // Add space for state and Position (state 1 bit, position 2*16 bits)
            pickupBitarray[i] = new BitArray(new byte[] { i });
            pickupBitarray[i].Length = 9;

            //----------------------------------------------------------------------------------------
            // Create random position for each pickup, for demonstration only. These should be chosen from a predefined array.
            // Position is a Vector2 of 2 "short", so 16 bits for each value.
            // X and Y are stored in bitarray respectively from positions (9-24) & (25-40)
            //----------------------------------------------------------------------------------------

            // Get random Vector2 in range of Short, for demonstration only
            Vector2 randomPos = new Vector2(
                UnityEngine.Random.Range(short.MinValue, short.MaxValue), 
                UnityEngine.Random.Range(short.MinValue, short.MaxValue));

            // Save Position
            pickupPosition[i] = randomPos;

            // Get Bitarray for this Vector2
            BitArray vectPositionBitarray = Vector2ShortToBitArray(randomPos);

            //Complete the pickup bitarrays with new values
            int index = 0;
            for (int j = 9; j < 41; j++)
            {
                pickupStartPosition[i][j] = vectPositionBitarray[index];
                index++;
            }
        }
    }

    private void CreatePlayersArray()
    {
        for (byte i = 0; i < numberOfPlayers; i++) // Go through each players
        {
            playersId[i] = i; // Initialize ID array
            playersState[i] = false; //false = alive, true = dead

            playerBitarray[i] = new BitArray(new byte[] { i }); // Create bitarray with byte ID for each players
            playerBitarray[i].Length = 57; // Add space for state, Position, Velocity (state 1 bit, position 2*16 bits, velocity 2*8 bits)

            BitArray vectPositionBitarray = Vector2ShortToBitArray(playerPosition[i]);
            BitArray vectVelocityBitarray = Vector2ByteToBitArray(playerVelocity[i]);

            //Complete the player bitarrays with new values
            int index = 0;
            for (int x = 9; x < 41; x++) //Add Position values
            {
                playerBitarray[i][x] = vectPositionBitarray[index];
                index++;
            }
            index = 0;
            for (int y = 41; y < 57; y++) //Add Velocity values
            {
                playerBitarray[i][y] = vectVelocityBitarray[index];
                index++;
            }
        }
    }

    private void UpdateBitarrayOfPlayers()
    {
        for (byte i = 0; i < numberOfPlayers; i++) // Go through each players
        {
            playerBitarray[i][8] = playersState[i]; // Update state

            if (playersState[i]) // If state is true, player is dead, we remove position and velocity
            {
                playerBitarray[i].Length = 9;
                bitsPerTick += 9;
            }
            else // Player is alive, we update position and velocity
            {
                playerBitarray[i].Length = 57;
                bitsPerTick += 57;

                BitArray vectPositionBitarray = Vector2ShortToBitArray(playerPosition[i]);
                BitArray vectVelocityBitarray = Vector2ByteToBitArray(playerVelocity[i]);

                int index = 0;
                for (int x = 9; x < 41; x++) //Update Position values
                {
                    playerBitarray[i][x] = vectPositionBitarray[index];
                    index++;
                }
                index = 0;
                for (int y = 41; y < 57; y++) //Update Velocity values
                {
                    playerBitarray[i][y] = vectVelocityBitarray[index];
                    index++;
                }
            }
        }
    }

    private void UpdateBitarrayOfPickup()
    {
        for (byte i = 0; i < numberOfPickups; i++) // Go through each pickups
        {
            // Update state to match any changes made, position 8 is State data (true = picked up, false = available)
            pickupBitarray[i][8] = pickupState[i]; 
            bitsPerTick += 9;
        }
    }

    private void UpdateBitarrayNewMissile()
    {
        // Check if List of missiles is not empty
        if (newMissiles.Count == 0)
            return;
        
        // Create List of Bitarray of missiles
        foreach (int idMissile in newMissiles)
        {
            BitArray missile = new BitArray(48); //Each missile has 32 bits position + 16 bits velocity

            BitArray posArray = Vector2ShortToBitArray(missilePosition[idMissile - 1]);
            BitArray velArray = Vector2ByteToBitArray(missileVelocity[idMissile - 1]);

            int index = 0;
            for (int j = 0; j < 32; j++) //Update Position values
            {
                missile[j] = posArray[index];
                index++;
            }
            index = 0;
            for (int k = 32; k < 48; k++) //Update Velocity values
            {
                missile[k] = velArray[index];
                index++;
            }

            missileBitarray.Add(missile);
            bitsPerTick += 48;
        }


    }

    //***********************************************************************************************
    // Below are the functions used to convert different datas to Bitarray, and convert back.
    // Some are public to be used by clients to unpack data
    //***********************************************************************************************

    public byte BitarrayToByte(BitArray array)
    {
        if (array.Count != 8)
        {
            throw new ArgumentException("Cannot Convert: Number of bits != 8");
        }
        byte[] bytes = new byte[1];
        array.CopyTo(bytes, 0);
        return bytes[0];
    }

    public short BitarrayToShort(BitArray array)
    {
        if (array.Count != 16)
        {
            throw new ArgumentException("Cannot Convert: Number of bits != 16");
        }
        byte[] bytes = new byte[2];
        array.CopyTo(bytes, 0);
        return BitConverter.ToInt16(bytes, 0);
    }

    public Vector2 BitarrayToVector2Short(BitArray array)
    {
        if (array.Count != 32)
        {
            throw new ArgumentException("Cannot Convert: Number of bits != 32");
        }
        BitArray BitX = new BitArray(16);
        BitArray BitY = new BitArray(16);

        for (int i = 0; i < 16; i++)
        {
            BitX[i] = array[i];
        }
        for (int i = 0; i < 16; i++)
        {
            BitY[i] = array[16 + i];
        }

        short shortX = BitarrayToShort(BitX);
        short shortY = BitarrayToShort(BitY);

        return new Vector2(shortX, shortY);
    }

    public Vector2 BitarrayToVector2Byte(BitArray array)
    {
        if (array.Count != 16)
        {
            throw new ArgumentException("Cannot Convert: Number of bits != 16");
        }
        BitArray BitX = new BitArray(8);
        BitArray BitY = new BitArray(8);

        for (int i = 0; i < 8; i++)
        {
            BitX[i] = array[i];
        }
        for (int i = 0; i < 8; i++)
        {
            BitY[i] = array[8 + i];
        }

        short shortX = BitarrayToByte(BitX);
        short shortY = BitarrayToByte(BitY);

        return new Vector2(shortX, shortY);
    }

    private BitArray Vector2ByteToBitArray(Vector2 vector)
    {
        byte[] vectorByteArray = new byte[2] {(byte)vector.x, (byte)vector.y}; //Create array of 2 bytes from vector2
        return new BitArray(vectorByteArray);
    }

    private BitArray Vector2ShortToBitArray(Vector2 vector)
    {
            // Return array of byte for that short
            byte[] positionInByteX = BitConverter.GetBytes((short)vector.x);
            byte[] positionInByteY = BitConverter.GetBytes((short)vector.y);

            //Create temporary bitarray with the full short
            BitArray fullShortXBitarray = new BitArray(positionInByteX);
            BitArray fullShortYBitarray = new BitArray(positionInByteY);

            //create combined Bitarray
            BitArray toReturn = new BitArray(32);
            int index = 0;
            for (int x = 0; x < 16; x++) //Add X of the shorts position
            {
                toReturn[x] = fullShortXBitarray[index];
                index++;
            }
            index = 0;
            for (int y = 16; y < 32; y++) //Add Y of the shorts position
            {
                toReturn[y] = fullShortYBitarray[index];
                index++;
            }
            return toReturn;
    }

    // **************************************************************************
    // Methods to modify the missile list
    // **************************************************************************

    private void ClearAllMissile()
    {
        missileBitarray.Clear();
        newMissiles.Clear();
        missilePosition.Clear();
        missileVelocity.Clear();
    }

    private void AddMissiles()
    {
        // Randomly check if we have new missile, and how many
        if (UnityEngine.Random.Range(0, 100) < 20)
        {
            int numberOfMissiles = UnityEngine.Random.Range(1, 4);
            for (int i = 1; i < numberOfMissiles; i++)
            {
                AddNewMissile(new Vector2(
                    UnityEngine.Random.Range(short.MinValue, short.MaxValue),
                    UnityEngine.Random.Range(short.MinValue, short.MaxValue)), 
                            new Vector2(
                    UnityEngine.Random.Range(byte.MinValue, byte.MaxValue),
                    UnityEngine.Random.Range(byte.MinValue, byte.MaxValue))
                    );
            }
        }
    }

    private void AddNewMissile(Vector2 newPosition, Vector2 newVelocity)
    {
        newMissiles.Add(newMissiles.Count + 1);
        missilePosition.Add(newPosition);
        missileVelocity.Add(newVelocity);
    }
}