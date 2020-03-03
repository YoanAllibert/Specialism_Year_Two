using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderVariantListGenerator : MonoBehaviour
{
    [SerializeField] private List<ListOfKeywords> variants =new List<ListOfKeywords>();

    private List<string> output = new List<string>();

    private int currentIndex = 0;

    void Start()
    {
        GenerateList();
    }

    private void GenerateList()
    {
        foreach (ListOfKeywords variant in variants)
        {
            //if (variant.keywords.Count > 0)                 //If The Keywords list is not empty
            foreach (string keyword in variant.keywords)
            {
                //if (list.IndexOf (item))
                output.Add(keyword);
            }
        }
        


        //SHOW OUTPUT IN CONSOLE
        foreach (string result in output)
        {
            Debug.Log(result);
        }
    }
}

//SERIALIZABLE LIST OF KEYWORDS
[System.Serializable]
public class ListOfKeywords
{
    public List<string> keywords;
}