using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GeneratingScores : MonoBehaviour
{
    [SerializeField] private List<string> Initials; //List of base Initials
    [SerializeField] private List<int> averageScore; //One entry per level
    [SerializeField] private List<int> standardDeviation; //One entry per level

    [SerializeField] private List<int> savedScores; //Full list of saved scores

    void Start()
    {
        int i = 10/5;
        Debug.Log(Mathf.FloorToInt(i));

        if ((File.Exists(Application.persistentDataPath + "/saves/RankingTable.save"))) //First time running, we generate new lists
        {
            PopulateLists();
            GenerateScores();
            OrderingScores();
            SaveGame();
        }
        else
        {
            LoadGame();
        }
    }

    private void PopulateLists()
    {
        for (int i = 0; i < 60; i++)
        {
            averageScore.Add(UnityEngine.Random.Range(1000, 6000)); //Average scores
            standardDeviation.Add(UnityEngine.Random.Range(200, 500)); //Generate a random standard deviation
        }
    }

    private void GenerateScores()
    {
        for (int i = 0; i < 60 * 5; i++) //60 levels, saving the top 5 scores for each
        {
            int levelIndex = Mathf.FloorToInt(i / 5); //return index of level
            int score = averageScore[levelIndex]; //return average score of level
            int deviation = standardDeviation[levelIndex]; //get standard deviation of level

            int newScore = UnityEngine.Random.Range(score - deviation, score + deviation); //create a new random score of level with deviation
            newScore = Mathf.FloorToInt(newScore / 10) * 10; //create score as a multiply of 10

            savedScores.Add(newScore);
        }
    }

    private void OrderingScores()
    {
        //create buffer list:
        List<int> bufferScore = new List<int>();

        //Access each level:
        for (int i = 0; i < 60; i++)
        {
            //populate buffer list
            for (int j = 0; j < 5; j++)
            {
                int idOfScore = (i * 5) + j; //calculate index of score 
                bufferScore.Add(savedScores[idOfScore]);
            }

            bufferScore.Sort(); //Sorting score in ascending order
            bufferScore.Reverse(); //sorting in descending order

            //Change Saved Score for new order
            for (int j = 0; j < 5; j++)
            {
                int idOfScore = (i * 5) + j; //calculate index of score 
                savedScores[idOfScore] = bufferScore[j];
            }

            bufferScore.Clear(); //Clear buffer for next level
        }
    }

    private SaveToFile CreateSaveGameObject()
    {
        SaveToFile save = new SaveToFile();
        int i = 0;
        foreach (int score in averageScore)
        {
            save.livingTargetPositions.Add(score);
            save.livingTargetsTypes.Add(score);
            i++;
        }

        save.hits = 2;
        save.shots = 4;

        return save;
    }

    public void SaveGame()
    {
        // 1
        SaveToFile save = CreateSaveGameObject();

        // 2
        BinaryFormatter bf = new BinaryFormatter();
        Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        FileStream file = File.Create(Application.persistentDataPath + "/saves/RankingTable.save");
        bf.Serialize(file, save);
        file.Close();

        // 3
        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        // 1
        if (File.Exists(Application.persistentDataPath + "/saves/RankingTable.save"))
        {
            /* ClearBullets();
            ClearRobots();
            RefreshRobots(); */

            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/saves/RankingTable.save", FileMode.Open);
            SaveToFile save = (SaveToFile)bf.Deserialize(file);
            file.Close();

            // 3
            for (int i = 0; i < save.livingTargetPositions.Count; i++)
            {
                int position = save.livingTargetPositions[i];
                //Target target = targets[position].GetComponent<Target>();
                //target.ActivateRobot((RobotTypes)save.livingTargetsTypes[i]);
                //target.GetComponent<Target>().ResetDeathTimer();
            }

            // 4
            //shotsText.text = "Shots: " + save.shots;
            //hitsText.text = "Hits: " + save.hits;
            int shots = save.shots;
            int hits = save.hits;

            Debug.Log("shots" +shots);
            Debug.Log("hits" +hits);

            //Unpause();
        }
        else
        {
            Debug.Log("No game saved!");
        }
    }
}
