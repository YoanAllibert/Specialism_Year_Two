using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequencyDetection : MonoBehaviour
{
    [Header("Audio Source And Visual")]
    [SerializeField] [Tooltip("Source of audio to visualize")] private AudioSource sourceAudio;
    [SerializeField] [Tooltip("Object to turn On and Off")] private GameObject[] objects = new GameObject[4];

    [Header("Frequency Band 1")]
    [SerializeField] [Range(20, 20000)] private int frequencyOneFrom = 20;
    [SerializeField] [Range(20, 20000)] private int frequencyOneTo = 20000;
    [SerializeField]  private float thresholdOne;

    [Header("Frequency Band 2")]
    [SerializeField] [Range(20, 20000)] private int frequencTwoFrom = 20;
    [SerializeField] [Range(20, 20000)] private int frequencTwoTo = 20000;
    [SerializeField]  private float thresholdTwo;

    [Header("Frequency Band 3")]
    [SerializeField] [Range(20, 20000)] private int frequencyThreeFrom = 20;
    [SerializeField] [Range(20, 20000)] private int frequencyThreeTo = 20000;
    [SerializeField]  private float thresholdThree;

    [Header("Frequency Band 4")]
    [SerializeField] [Range(20, 20000)] private int frequencyFourFrom = 20;
    [SerializeField] [Range(20, 20000)] private int frequencyFourTo = 20000;
    [SerializeField]  private float thresholdFour;

    private int numberOfSamples = 1024;
    private float[] samples;

    private int[] frequencyFrom = new int[4];
    private int[] frequencyTo = new int[4];

    private int[] samplesFrom = new int[4];
    private int[] samplesTo = new int[4];

    private float[] threshold = new float[4];

    private float[] frequenciesAverage = new float[4];

    private void Start()
    {
        if (objects.Length != 4)
        {
            Debug.Log("Warning: Please populate the Objects array with 4 GameObjects!");
        }

        samples = new float[numberOfSamples];
        PopulateFrequencyValue();
        GetClosestSamples();
    }

    private void Update()
    {
        if (sourceAudio != null)
        {
            GetSpectrumAudioSource();
            CalculateAveragePerBand();

            for (int i = 0; i < 4; i++)
            {
                if (frequenciesAverage[i] >= threshold[i] / 1000f)
                {
                    objects[i].SetActive(true);
                }
                else
                    objects[i].SetActive(false);
            }
        }
    }

    private void GetSpectrumAudioSource()
    {
        sourceAudio.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    private void CalculateAveragePerBand()
    {
        for (int i = 0; i < 4; i++)
        {
            float average = 0f;
            float totalAdded = 0f;
            for (int j = samplesFrom[i]; j < samplesTo[i]; j++)
            {
                totalAdded += samples[j];
            }
            average = totalAdded / ((samplesTo[i] - samplesFrom[i]) + 1f);
            frequenciesAverage[i] = average;
        }
    }

    private void PopulateFrequencyValue()
    {
        frequencyFrom[0] = frequencyOneFrom;
        frequencyTo[0] = frequencyOneTo;

        frequencyFrom[1] = frequencTwoFrom;
        frequencyTo[1] = frequencTwoTo;

        frequencyFrom[2] = frequencyThreeFrom;
        frequencyTo[2] = frequencyThreeTo;

        frequencyFrom[3] = frequencyFourFrom;
        frequencyTo[3] = frequencyFourTo;

        threshold[0] = thresholdOne;
        threshold[1] = thresholdTwo;
        threshold[2] = thresholdThree;
        threshold[3] = thresholdFour;
    }

    private void GetClosestSamples()
    {
        for (int i = 0; i < 4; i++)
        {
            samplesFrom[i] = Mathf.RoundToInt((numberOfSamples * frequencyFrom[i]) / 24000);
            samplesTo[i] = Mathf.RoundToInt((numberOfSamples * frequencyTo[i]) / 24000);
        }
    }
}