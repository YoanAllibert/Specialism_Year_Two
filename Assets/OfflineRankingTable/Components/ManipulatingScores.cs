using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManipulatingScores : MonoBehaviour
{
    [SerializeField] private Text[] textScores = new Text[5];
    [SerializeField] private Text[] textInitials = new Text[5];

    [SerializeField] private InputField addingScore;
    [SerializeField] private InputField addingInitials;
    [SerializeField] private Text textShowLevel;

    private GeneratingScores scores;
    private int chosenLevel = 1;
    private int newScore = 0;
    private string newInitials = "";

    void Start()
    {
        scores = GetComponent<GeneratingScores>();
        ShowScores();

        for (int i = 0; i < 5; i++)
        {
            
        }
    }

    private void TryAddNewScore(int level, int score, string initials)
    {
        for (int i = (level - 1) * 5; i < ((level - 1) * 5) + 5; i++) //Go through each score at level
        {
            if (score > scores.savedScores[i]) //new score is bigger than saved scores
            {
                //take place of last one, then reorder

                int indexOfSmallestScore = ((level - 1) * 5) + 4;
                scores.savedScores[indexOfSmallestScore] = score;
                scores.savedNames[indexOfSmallestScore] = initials;
                scores.OrderingScores();
                return;
            }
        }
    }

    public void SetLevel(int level)
    {
        if (level > 0 && level < 61)
            chosenLevel = level;
        else
            Debug.Log("Level must be between 1 and 60");

        textShowLevel.text = "Level " + chosenLevel;
        ShowScores();
    }

    private void ShowScores()
    {
        for (int i = 0; i < 5; i++)
        {
            textScores[i].text = scores.savedScores[((chosenLevel - 1) * 5) + i].ToString();
        }
        for (int i = 0; i < 5; i++)
        {
            textInitials[i].text = scores.savedNames[((chosenLevel - 1) * 5) + i].ToString();
        }
    }

    public void SetScore(int score)
    {
        newScore = score;
    }

    public void SetInitials(string name)
    {
        newInitials = name;
    }

    public void AddScore()
    {
        int tryingScore;
        if (int.TryParse(addingScore.text, out tryingScore))
            newScore = tryingScore;
        else
        {
            Debug.Log("Not a valid number");
            return;
        }
        
        newInitials = addingInitials.text;

        TryAddNewScore(chosenLevel, newScore, newInitials);
        ShowScores();
    }

    public void SaveScore()
    {
        scores.SaveGame();
    }

    public void LoadScore()
    {
        scores.LoadGame();
        ShowScores();
    }
}