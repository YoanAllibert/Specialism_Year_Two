using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowingData : MonoBehaviour
{
    public DataPacking host;
    public ReceivingData client;

    public Text[] playersId;
    public Text[] playersState;
    public Text[] playersPosition;
    public Text[] playersVelocity;

    public Text[] pickupId;
    public Text[] pickupState;

    public Text missileText;

    public Text bitsPerTickText;
    public Text kbPerSecText;

    int[] players = new int[3];
    int[] pickups = new int[3];

    private int cumulatedBits = 0;
    private int numberBitsAdded = 0;
    private float kbytesPerSec;

    void OnEnable() 
    {
        host.OnEndTickServer += TickServer;
        InvokeRepeating("EverySeconds", 1f, 1f);
    }

    void EverySeconds()
    {
        kbytesPerSec = ((float)cumulatedBits / 8f) / 1000f;
        Debug.Log(kbytesPerSec);
        kbPerSecText.text = kbytesPerSec.ToString();

        cumulatedBits = 0;
        numberBitsAdded = 0;
    }

    void TickServer()
    {
        UpdateBitsData();
    }

    void OnDisable() 
    {
        host.OnEndTickServer -= TickServer;
    }

    public void RandomData()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i] = UnityEngine.Random.Range(0, host.numberOfPlayers);
        }

        for (int i = 0; i < pickups.Length; i++)
        {
            pickups[i] = UnityEngine.Random.Range(0, host.numberOfPickups);
        }

        UpdateText();
    }

    private void UpdateText()
    {
        for (int i = 0; i < 3; i++) // Show 3 Random Players and PickUps
        {
            // Update ID
            playersId[i].text = host.playersId[(byte)players[i]].ToString();
            playersId[3 + i].text = client.playersId[(byte)players[i]].ToString();

            pickupId[i].text = host.pickupId[(byte)pickups[i]].ToString();
            pickupId[3 + i].text = client.pickupId[(byte)pickups[i]].ToString();

            // Update State
            if(host.playersState[(byte)players[i]])
                playersState[i].text = "Dead";
            else
                playersState[i].text = "Alive";
            if (client.playerState[(byte)players[i]])
                playersState[3 + i].text = "Dead";
            else
                playersState[3 + i].text = "Alive";
            if (host.pickupState[(byte)pickups[i]])
                pickupState[i].text = "Picked Up";
            else
                pickupState[i].text = "Available";
            if (client.pickupState[(byte)pickups[i]])
                pickupState[3 + i].text = "Picked Up";
            else
                pickupState[3 + i].text = "Available";

            // Update Position And Velocity
            if(host.playersState[(byte)players[i]])
            {
                playersPosition[i].text = "";
                playersVelocity[i].text = "";
            }
            else
            {
                playersPosition[i].text = host.playerPosition[(byte)players[i]].ToString();
                playersVelocity[i].text = host.playerVelocity[(byte)players[i]].ToString();
            }
            if (client.playerState[(byte)players[i]])
            {
                playersPosition[3 + i].text = "";
                playersVelocity[3 + i].text = "";
            }
            else
            {
                playersPosition[3 + i].text = client.playerPosition[(byte)players[i]].ToString();
                playersVelocity[3 + i].text = client.playerVelocity[(byte)players[i]].ToString();
            }

            // Update Last Missile
            missileText.text = "Position: " + client.lastMissile.PositionMissile.ToString() + ", Velocity: " + client.lastMissile.VelocityMissile.ToString();
        }
    }

    private void UpdateBitsData()
    {
        bitsPerTickText.text = host.bitsPerTick.ToString();
        cumulatedBits += host.bitsPerTick;
        numberBitsAdded++;
    }
}