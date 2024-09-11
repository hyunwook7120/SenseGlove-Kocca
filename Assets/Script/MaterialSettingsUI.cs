using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSettingsUI : MonoBehaviour
{
    public enum ExperimentCase
    {
        Test,
        Static,
        Dynamic,
        Mix
    }
    public ExperimentCase experiment;
    public SG.SG_Material material; // SG_Material 스크립트가 연결된 게임 오브젝트

    public Button setMaxForceButton1;
    public Button setMaxForceButton2;
    public Button setMaxForceButton3;
    public Button setMaxForceButton4;
    public Button setMaxForceButton5;
    public Button prevButton;
    public Button nextButton;
    int count = -1;
    int totalCount = 0;
    List<Vector2> testCase;

    void Start()
    {
        SetMaxForce(0.0f);
        SetMaxForceDist(0.0f);
        setMaxForceButton1.onClick.AddListener(() => SetMaxForce(0.0f));
        setMaxForceButton2.onClick.AddListener(() => SetMaxForce(0.1f));
        setMaxForceButton3.onClick.AddListener(() => SetMaxForce(0.3f));
        setMaxForceButton4.onClick.AddListener(() => SetMaxForce(0.6f));
        setMaxForceButton5.onClick.AddListener(() => SetMaxForce(1.0f));
        prevButton.onClick.AddListener(() => PrevForce());
        nextButton.onClick.AddListener(() => NextForce());

        if (experiment == ExperimentCase.Test)
        {
            testCase = ShuffleTestCase(MakeTestCase1(1));
        }
        else if (experiment == ExperimentCase.Static)
        {
            testCase = ShuffleTestCase(MakeTestCase1(5));
        }
        else if (experiment == ExperimentCase.Dynamic)
        {
            testCase = ShuffleTestCase(MakeTestCase2(5));
        }
        else if (experiment == ExperimentCase.Mix)
        {
            testCase = ShuffleTestCase(MakeTestCase3(5));
        }

        string filePath = Path.Combine(Application.dataPath, "Resources", "Log.txt");

        using (StreamWriter sw = new StreamWriter(filePath))
        {
            foreach (var item in testCase)
            {
                sw.WriteLine(item);
            }
        }

        // 파일이 저장된 경로를 디버깅 로그로 출력
        Debug.Log($"Test cases have been saved to: {filePath}");
    }

    void SetMaxForce(float value)
    {
        if (material != null)
        {
            material.SetMaxForce(value);
        }
    }

    void SetMaxForceDist(float value)
    {

        if (material != null)
        {
            material.SetMaxForceDistance(value);
        }
    }

    // 8번의 연속된 prev까지만 실제 이전 케이스
    void PrevForce()
    {
        count--;

        Debug.Log(count+1);
        SetMaxForce(testCase[count][0]);
        SetMaxForceDist(testCase[count][1]);
    }

    void NextForce()
    {
        count++;

        if (count >= testCase.Count)
        {
            Debug.LogError("Finish");
        }
        else
        {
            Debug.Log(count+1);
            SetMaxForce(testCase[count][0]);
            SetMaxForceDist(testCase[count][1]);
        }
    }

    List<Vector2> MakeTestCase1(int repeat)
    {
        List<Vector2> originalTestCase = new List<Vector2>
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.1f, 0.0f),
            new Vector2(0.3f, 0.0f),
            new Vector2(0.6f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(0.0f, 0.0f),
            new Vector2(0.1f, 0.0f),
            new Vector2(0.3f, 0.0f),
            new Vector2(0.6f, 0.0f),  
            new Vector2(1.0f, 0.0f),
        };

        List<Vector2> testCase = new List<Vector2>();

        for (int i = 0; i < repeat; i++)
        {
            testCase.AddRange(originalTestCase);
        }

        return testCase;
    }
    
        List<Vector2> MakeTestCase2(int repeat)
    {
        List<Vector2> originalTestCase = new List<Vector2>
        {
            new Vector2(0.0f, 0.04f),
            new Vector2(0.1f, 0.04f),
            new Vector2(0.3f, 0.04f),
            new Vector2(0.6f, 0.04f),
            new Vector2(1.0f, 0.04f),
            new Vector2(0.0f, 0.04f),
            new Vector2(0.1f, 0.04f),
            new Vector2(0.3f, 0.04f),
            new Vector2(0.6f, 0.04f),
            new Vector2(1.0f, 0.04f)
        };

        List<Vector2> testCase = new List<Vector2>();

        for (int i = 0; i < repeat; i++)
        {
            testCase.AddRange(originalTestCase);
        }

        return testCase;
    }

    List<Vector2> MakeTestCase3(int repeat)
    {
        List<Vector2> originalTestCase = new List<Vector2>
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.1f, 0.0f),
            new Vector2(0.3f, 0.0f),
            new Vector2(0.6f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(0.0f, 0.04f),
            new Vector2(0.1f, 0.04f),
            new Vector2(0.3f, 0.04f),
            new Vector2(0.6f, 0.04f),
            new Vector2(1.0f, 0.04f)
        };
        List<Vector2> testCase = new List<Vector2>();

        for (int i = 0; i < repeat; i++)
        {
            testCase.AddRange(originalTestCase);
        }

        return testCase;
    }

    public static List<Vector2> ShuffleTestCase(List<Vector2> testCase)
    {
        var rng = new System.Random();
        int n = testCase.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Vector2 value = testCase[k];
            testCase[k] = testCase[n];
            testCase[n] = value;
        }
        return testCase;
    }

}