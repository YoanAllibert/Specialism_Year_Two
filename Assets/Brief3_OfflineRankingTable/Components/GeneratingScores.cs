using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Linq;

public class GeneratingScores : MonoBehaviour
{
    [SerializeField] private List<string> initials; //List of base Initials

    private List<int> averageScore; //One entry per level
    private List<int> standardDeviation; //One entry per level

    public List<int> savedScores; //Full list of saved scores (300)
    public List<string> savedNames; //Full list of saved names (300)

    void Start()
    {
        if (!(File.Exists(Application.persistentDataPath + "/saves/RankingTable.save"))) //First time running, we generate new lists
        {
            PopulateLists();
            GenerateScores();
            GenerateNames();
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
            averageScore.Add(UnityEngine.Random.Range(1000, 8000)); //Average scores
            standardDeviation.Add(UnityEngine.Random.Range(100, 600)); //Generate a random standard deviation
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

    private void GenerateNames()
    {
        for (int i = 0; i < 60 * 5; i++) //60 levels, saving random initials for 5 scores each
        {
            savedNames.Add(initials[UnityEngine.Random.Range(0, initials.Count)]); 
        }
    }

    public void OrderingScores()
    {
        //Access each level:
        for (int i = 0; i < 60; i++)
        {
            //Create buffer dictionary
            Dictionary<int, int> buffer = new Dictionary<int, int>();
            List<string> bufferNames = new List<string>();

            //populate buffer dictionary
            for (int j = 0; j < 5; j++)
            {
                int index = (i * 5) + j; //calculate index of entry
                buffer.Add(j, savedScores[index]);
                bufferNames.Add(savedNames[index]);
            }

            //Order scores in descending order using Linq
            var scores = from pair in buffer
                    orderby pair.Value descending
                    select pair;

            int increment = 0;
            foreach (KeyValuePair<int, int> pair in scores)
            {
                savedScores[(i * 5) + increment] = pair.Value;
                savedNames[(i * 5) + increment] = bufferNames[pair.Key];
                increment++;
            }

            buffer.Clear(); //Clear buffer for next level
            bufferNames.Clear();
        }
    }

    private SaveToFile CreateSaveGameObject()
    {
        SaveToFile save = new SaveToFile();

        foreach (int score in savedScores)
        {
            save.allScores.Add(score);
        }
        foreach (string name in savedNames)
        {
            save.allInitials.Add(name);
        }

        return save;
    }

    public void SaveGame()
    {
        SaveToFile save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        FileStream file = File.Create(Application.persistentDataPath + "/saves/RankingTable.save");
        bf.Serialize(file, save);
        file.Close();

        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/saves/RankingTable.save")) //If we have a saved file
        {
            savedScores.Clear();
            savedNames.Clear();

            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/saves/RankingTable.save", FileMode.Open);
            SaveToFile save = (SaveToFile)bf.Deserialize(file);
            file.Close();

            for (int i = 0; i < save.allScores.Count; i++) 
            {
                savedScores.Add(save.allScores[i]);
                savedNames.Add(save.allInitials[i]);
            }
        }
        else
        {
            Debug.Log("No Save to Load");
        }
    }
}
