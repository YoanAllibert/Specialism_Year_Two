using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderVariantListGenerator : MonoBehaviour
{
    [SerializeField] private List<ListOfKeywords> variants =new List<ListOfKeywords>();

    private List<string> output = new List<string>();
    private int nbOfOutput = 1;

    private int[] nbOfWordsInNextLists;

    void Start()
    {
        GenerateList();
    }

    private void GenerateList()
    {
        //Get number of variant---------------------------------------------------------------------
        foreach (ListOfKeywords variant in variants)
        {
            nbOfOutput *= variant.keywords.Count;
        }

        //Calculate number of words in each next lists----------------------------------------------
        nbOfWordsInNextLists = new int[variants.Count];
        for (int i = 0; i < variants.Count - 1; i++)
        {
            int mult = 1;
            for (int j = variants.Count - 1; j > i; j--) //grab next variants
            {
                mult *= variants[j].keywords.Count;
            }
            nbOfWordsInNextLists[i] = mult;
        }
        nbOfWordsInNextLists[variants.Count - 1] = 1; //for last variant, set to one

        //Populate List of words----------------------------------------------------------------------
        for (int i = 0; i < nbOfOutput; i++)
        {
            string currentLine = "";
            for (int j = 0; j < variants.Count; j++)
            {
                string wordToAdd = variants[j].keywords[(i / nbOfWordsInNextLists[j]) % variants[j].keywords.Count]; //find keyword
                if (wordToAdd == "null") //check if keyword is "null"
                    wordToAdd = "";
                currentLine += " "+wordToAdd;
            }
            output.Add(currentLine);
        }

        //SHOW OUTPUT IN CONSOLE---------------------------------------------------------------------------
        foreach (string result in output)
        {
            Debug.Log(result);
        }


        //EXAMPLE-----------------------------------------------------------------------------------------

        //(DIRECTIONAL, POINT)                     0,0 - 0,1
        //(LIGHTMAP_ON, LIGHTMAP_SH, null)         1,0 - 1,1 - 1,2
        //(FOG_LINEAR, null)                       2,0 - 2,1
        
        //First word:   (indexOfOutput / list2*list3..) % nbOfWordInFirstList
        //Second word:  (indexOfOutput / list3..) % nbOfWordInSecondList
        //Third word:   (indexOfOutput) % nbOfWordInThridList

        // n 0 -> 2 * 3 * 2

        //Grabing word in third list
        // 0 % 2 = 0
        // 1 % 2 = 1
        // 2 % 2 = 0
        // 3 % 2 = 1
        // 4 % 2 = 0
        // 5 % 2 = 1
        // 6 % 2 = 0
        // 7 % 2 = 1
        // 8 % 2 = 0
        // 9 % 2 = 1
        // 10 % 2 = 0
        // 11 % 2 = 1

        //Grabing word in second list
        // (0 / 2) % 3 = 0
        // (1 / 2) % 3 = 0
        // (2 / 2) % 3 = 1
        // (3 / 2) % 3 = 1
        // (4 / 2) % 3 = 2
        // (5 / 2) % 3 = 2
        // (6 / 2) % 3 = 0
        // (7 / 2) % 3 = 0
        // (8 / 2) % 3 = 1
        // (9 / 2) % 3 = 1
        // (10 / 2) % 3 = 2
        // (11 / 2) % 3 = 2

        //Grabing word in first list
        // ((0 / 3) /2) % 2 = 0
        // ((1 / 3) /2) % 2 = 0
        // ((2 / 3) /2) % 2 = 0
        // ((3 / 3) /2) % 2 = 0
        // ((4 / 3) /2) % 2 = 0
        // ((5 / 3) /2) % 2 = 0
        // ((6 / 3) /2) % 2 = 1
        // ((7 / 3) /2) % 2 = 1
        // ((8 / 3) /2) % 2 = 1
        // ((9 / 3) /2) % 2 = 1
        // ((10 / 3) /2) % 2 = 1
        // ((11 / 3) /2) % 2 = 1
    }
}

//SERIALIZABLE LIST OF KEYWORDS
[System.Serializable]
public class ListOfKeywords
{
    public List<string> keywords;
}