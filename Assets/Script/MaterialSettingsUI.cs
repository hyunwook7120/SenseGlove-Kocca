using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SG;
using Unity.VisualScripting;

public class MaterialSettingsUI : MonoBehaviour
{
    public enum ExperimentCase
    {
        Test,
        Static,
        Dynamic,
        Mix,
        HeatForce
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
    public Button LowButton;
    public Button MediumButton;
    public Button HighButton;
    public Button ChangeButton;
    public CurveController CurveController;
    int count = -1;
    List<Vector2> testCase;
    List<Vector3> testCase1;
    SG.SG_MaterialProperties materialProperties;
    AnimationCurve s1, s2,s3, s4, s5, i;

    void Start()
    {
        materialProperties = material.GetComponent<SG_Material>().materialProperties;

        s1 = CurveController.shift_1;
        s2 = CurveController.shift_2;
        s3 = CurveController.shift_3;
        s4 = CurveController.shift_4;
        s5 = CurveController.shift_5;
        i  = CurveController.impedence;

        SetMaxForce(0.0f);
        SetMaxForceDist(0.06f);
        materialProperties.forceRepsonse=s1;

        prevButton.onClick.AddListener(() => PrevForce());
        nextButton.onClick.AddListener(() => NextForce());
        ChangeButton.onClick.AddListener(() => ChangeCase());

        // string tempPath = Path.Combine(Application.dataPath, "Resources", "temperature.txt");
        // StreamWriter swt = new StreamWriter(tempPath);
        LowButton.onClick.AddListener(() => ChangeTemp(24f));
        MediumButton.onClick.AddListener(() => ChangeTemp(32f));
        HighButton.onClick.AddListener(() => ChangeTemp(40f));
        ChangeTemp(32f);
        if (experiment == ExperimentCase.Test)
        {
            testCase = ShuffleTestCase(MakeTestCase1(1));
        }
        else if (experiment == ExperimentCase.Static)
        {
            testCase = ShuffleTestCase(MakeTestCase1(5));
            setMaxForceButton1.onClick.AddListener(() => {SetMaxForce(0.0f); materialProperties.forceRepsonse=i;});
            setMaxForceButton2.onClick.AddListener(() => {SetMaxForce(0.1f); materialProperties.forceRepsonse=i;});
            setMaxForceButton3.onClick.AddListener(() => {SetMaxForce(0.3f); materialProperties.forceRepsonse=i;});
            setMaxForceButton4.onClick.AddListener(() => {SetMaxForce(0.5f); materialProperties.forceRepsonse=i;});
            setMaxForceButton5.onClick.AddListener(() => {SetMaxForce(1.0f); materialProperties.forceRepsonse=i;});
        }
        else if (experiment == ExperimentCase.Dynamic)
        {
            testCase = ShuffleTestCase(MakeTestCase2(5));
            setMaxForceButton1.onClick.AddListener(() => {SetMaxForce(0.0f); materialProperties.forceRepsonse=s1;});
            setMaxForceButton2.onClick.AddListener(() => {SetMaxForce(0.1f); materialProperties.forceRepsonse=s2;});
            setMaxForceButton3.onClick.AddListener(() => {SetMaxForce(0.3f); materialProperties.forceRepsonse=s3;});
            setMaxForceButton4.onClick.AddListener(() => {SetMaxForce(0.5f); materialProperties.forceRepsonse=s4;});
            setMaxForceButton5.onClick.AddListener(() => {SetMaxForce(1.0f); materialProperties.forceRepsonse=s5;});
        }
        else if (experiment == ExperimentCase.Mix)
        {
            testCase = ShuffleTestCase(MakeTestCase3(5));
        }
        else if (experiment == ExperimentCase.HeatForce)
        {
            testCase1 = ShuffleTestCase1(MakeTestCase4(1));
        }
        
        string filePath = Path.Combine(Application.dataPath, "Resources", "Log.txt");

        using (StreamWriter sw = new StreamWriter(filePath))
        {
            if (experiment != ExperimentCase.HeatForce)
            {
                foreach (var item in testCase)
                {
                    sw.WriteLine(item);
                }
            }
            else if (experiment == ExperimentCase.HeatForce)
            {
                foreach (var item in testCase1)
                {
                    sw.WriteLine(item);
                }
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
    }

    void NextForce()
    {
        count++;

        if ((experiment != ExperimentCase.HeatForce) && (count >= testCase.Count))
        {
            Debug.LogError("Finish");
        }
        else if (count >= testCase1.Count)
        {
            Debug.LogError("Finish");
        }
        else
        {
            Debug.Log(count+1);
            
            if (experiment == ExperimentCase.HeatForce)
            {
                SetMaxForce(testCase1[count][0]);
                ChangeTemp(testCase1[count][2]);
                if (testCase1[count][1] == 0.0f)
                {
                    materialProperties.forceRepsonse = i;
                }
                else if (testCase1[count][1] == 1.0f)
                {
                    materialProperties.forceRepsonse = s1;
                }
                else if (testCase1[count][1] == 2.0f)
                {
                    materialProperties.forceRepsonse = s2;
                }
                else if (testCase1[count][1] == 3.0f)
                {
                    materialProperties.forceRepsonse = s3;
                }
                else if (testCase1[count][1] == 4.0f)
                {
                    materialProperties.forceRepsonse = s4;
                }
                else if (testCase1[count][1] == 5.0f)
                {
                    materialProperties.forceRepsonse = s5;
                }
            }
            else
            {
                SetMaxForce(testCase[count][0]);
                if (testCase[count][1] == 0.0f)
                {
                    materialProperties.forceRepsonse = i;
                }
                else if (testCase[count][1] == 1.0f)
                {
                    materialProperties.forceRepsonse = s1;
                }
                else if (testCase[count][1] == 2.0f)
                {
                    materialProperties.forceRepsonse = s2;
                }
                else if (testCase[count][1] == 3.0f)
                {
                    materialProperties.forceRepsonse = s3;
                }
                else if (testCase[count][1] == 4.0f)
                {
                    materialProperties.forceRepsonse = s4;
                }
                else if (testCase[count][1] == 5.0f)
                {
                    materialProperties.forceRepsonse = s5;
                }
            }
        }
    }

    void ChangeCase()
    {
        if (experiment == ExperimentCase.Static)
        {
            setMaxForceButton1.onClick.AddListener(() => {SetMaxForce(0.0f); materialProperties.forceRepsonse=s1;});
            setMaxForceButton2.onClick.AddListener(() => {SetMaxForce(0.1f); materialProperties.forceRepsonse=s2;});
            setMaxForceButton3.onClick.AddListener(() => {SetMaxForce(0.3f); materialProperties.forceRepsonse=s3;});
            setMaxForceButton4.onClick.AddListener(() => {SetMaxForce(0.5f); materialProperties.forceRepsonse=s4;});
            setMaxForceButton5.onClick.AddListener(() => {SetMaxForce(1.0f); materialProperties.forceRepsonse=s5;});
            Debug.Log("D");
            experiment = ExperimentCase.Dynamic;
        }
        else if (experiment == ExperimentCase.Dynamic)
        {
            setMaxForceButton1.onClick.AddListener(() => {SetMaxForce(0.0f); materialProperties.forceRepsonse=i;});
            setMaxForceButton2.onClick.AddListener(() => {SetMaxForce(0.1f); materialProperties.forceRepsonse=i;});
            setMaxForceButton3.onClick.AddListener(() => {SetMaxForce(0.3f); materialProperties.forceRepsonse=i;});
            setMaxForceButton4.onClick.AddListener(() => {SetMaxForce(0.5f); materialProperties.forceRepsonse=i;});
            setMaxForceButton5.onClick.AddListener(() => {SetMaxForce(1.0f); materialProperties.forceRepsonse=i;});
            Debug.Log("S");
            experiment = ExperimentCase.Static;
        }
    }

    void ChangeTemp(float temp)
    {       
        string filePath = Path.Combine(Application.dataPath, "Resources", "temperature.txt");

        StreamWriter swt = new StreamWriter(filePath);
        swt.WriteLine(temp);
        swt.Close();
    }

    List<Vector2> MakeTestCase1(int repeat)
    {
        List<Vector2> originalTestCase = new List<Vector2>
        {
            new Vector2(0.0f, 0.0f),
            new Vector2(0.1f, 0.0f),
            new Vector2(0.3f, 0.0f),
            new Vector2(0.5f, 0.0f),
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
            new Vector2(0.0f, 1.0f),
            new Vector2(0.1f, 2.0f),
            new Vector2(0.3f, 3.0f),
            new Vector2(0.5f, 4.0f),
            new Vector2(1.0f, 5.0f)
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
            new Vector2(0.5f, 0.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(0.1f, 2.0f),
            new Vector2(0.3f, 3.0f),
            new Vector2(0.5f, 4.0f),
            new Vector2(1.0f, 5.0f)
        };
        List<Vector2> testCase = new List<Vector2>();

        for (int i = 0; i < repeat; i++)
        {
            testCase.AddRange(originalTestCase);
        }

        return testCase;
    }
    List<Vector3> MakeTestCase4(int repeat)
    {
        List<Vector3> originalTestCase = new List<Vector3>
        {
            new Vector3(0.0f, 0.0f, 24f),
            new Vector3(0.1f, 0.0f, 24f),
            new Vector3(0.3f, 0.0f, 24f),
            new Vector3(0.5f, 0.0f, 24f),
            new Vector3(1.0f, 0.0f, 24f),
            new Vector3(0.1f, 2.0f, 24f),
            new Vector3(0.3f, 3.0f, 24f),
            new Vector3(0.5f, 4.0f, 24f),
            new Vector3(1.0f, 5.0f, 24f),
            new Vector3(0.0f, 0.0f, 32f),
            new Vector3(0.1f, 0.0f, 32f),
            new Vector3(0.3f, 0.0f, 32f),
            new Vector3(0.5f, 0.0f, 32f),
            new Vector3(1.0f, 0.0f, 32f),
            new Vector3(0.1f, 2.0f, 32f),
            new Vector3(0.3f, 3.0f, 32f),
            new Vector3(0.5f, 4.0f, 32f),
            new Vector3(1.0f, 5.0f, 32f),
            new Vector3(0.0f, 0.0f, 40f),
            new Vector3(0.1f, 0.0f, 40f),
            new Vector3(0.3f, 0.0f, 40f),
            new Vector3(0.5f, 0.0f, 40f),
            new Vector3(1.0f, 0.0f, 40f),
            new Vector3(0.1f, 2.0f, 40f),
            new Vector3(0.3f, 3.0f, 40f),
            new Vector3(0.5f, 4.0f, 40f),
            new Vector3(1.0f, 5.0f, 40f)
        };
        List<Vector3> testCase = new List<Vector3>();

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

        public static List<Vector3> ShuffleTestCase1(List<Vector3> testCase)
    {
        var rng = new System.Random();
        int n = testCase.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Vector3 value = testCase[k];
            testCase[k] = testCase[n];
            testCase[n] = value;
        }
        return testCase;
    }
}