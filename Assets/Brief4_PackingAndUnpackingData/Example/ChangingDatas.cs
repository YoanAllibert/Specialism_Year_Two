using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// *****************************************************
// This Script simulate gameplay by changing datas every tick
//******************************************************

public class ChangingDatas : MonoBehaviour
{
    [SerializeField] private DataPacking host;

    void OnEnable() 
    {
        host.OnStartTickServer += TickServer;
    }

    private void TickServer() //This will be called every tick
    {
        // Change Players position and velocity if alive
        for (int i = 0; i < host.numberOfPlayers; i++)
        {
            if (!host.playersState[i])
            {
                host.playerPosition[i] = new Vector2(
                    UnityEngine.Random.Range(short.MinValue, short.MaxValue),
                    UnityEngine.Random.Range(short.MinValue, short.MaxValue)
                );

                host.playerVelocity[i] = new Vector2(
                    UnityEngine.Random.Range(byte.MinValue, byte.MaxValue),
                    UnityEngine.Random.Range(byte.MinValue, byte.MaxValue)
                );
            }

            if (UnityEngine.Random.Range(0, 100) < 1) // 1% chance every tick to change status (die or respawn)
            {
                host.playersState[i] = !host.playersState[i];
            }
        }

        // Change Pickups State every Tick
        for (int i = 0; i < host.numberOfPickups; i++)
        {
            if (UnityEngine.Random.Range(0f, 100f) < 0.2f) // 0.2% chance every tick to change state (picked or available)
            {
                host.pickupState[i] = !host.pickupState[i];
            }
        }
    }

    void OnDisable() 
    {
        host.OnStartTickServer -= TickServer;
    }
}