using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text; // StringBuilder용도
using SG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CSVManager_modified : MonoBehaviour
{
    public enum SelectTouchMode
    {
        Always,         // 항상 저장
        OnlyTouched     // 터치된 경우에만 저장
    }

    public enum FindRenderingMode
    {
        Use,
        Unuse
    }
    
    [Header("Select Mode")]
    public SelectTouchMode selectTouchMode;
    public FindRenderingMode findRenderingMode;



    // 코루틴 초기화
    private bool isCoroutineRunning = false;
    //------------------------------------------------------------------------------------------------------------------------
    // Member Variables
    [Header("Necessary Object")]
    public GameObject nova2Glove;

    [Header("Optional Object")]
    public GameObject UIManager;

    [Header("CSV")]
    public string nameText;
    public float minSaveInterval = 0.02f;

    // Nova2Glove Variable
    SG_FingerFeedback[] fingerFeedbackScripts;
    Transform[][] fingerJoints;
    bool touching;
    SG_Material touchedMaterial;

    // Our Scene Variable
    MaterialSettingsUI UISetting;

    // CSV Variable
    List<string[]> data = new List<string[]>();
    string[] tempData;

    private string fileName;

    public void Start()
    {
        
        fileName = nameText + ".csv";
        // nova2Glove 오브젝트를 넣었는지 확인
        if (nova2Glove == null)
        {
            Debug.Log("Nova2Glove is Null.");
            return; // nova2Glove 오브젝트를 찾을 수 없으므로 종료
        }

        // SG_TrackedHand 컴포넌트 가져오기
        SG_TrackedHand trackedHand = nova2Glove.GetComponent<SG_TrackedHand>();
        if (trackedHand == null)
        {
            Debug.LogError("SG_TrackedHand component not found on nova2Glove.");
            return; // SG_TrackedHand 컴포넌트를 찾을 수 없으므로 종료
        }

        // Hand Model 오브젝트 가져오기
        GameObject handModelObject = trackedHand.handModel.gameObject;
        if (handModelObject == null)
        {
            Debug.LogError("Hand Model object not found on SG_TrackedHand.");
            return; // Hand Model 오브젝트를 찾을 수 없으므로 종료
        }

        // Hand Model 오브젝트에서 SG_HandMModelInfo 컴포넌트 가져오기
        SG_HandModelInfo handModel = handModelObject.GetComponent<SG_HandModelInfo>();
        if (handModel == null)
        {
            Debug.LogError("SG_HandModelInfo component not found on Hand Model object.");
            return; // SG_HandModelInfo 컴포넌트를 찾을 수 없으므로 종료
        }

        // fingerJoints 초기화
        fingerJoints = handModel.FingerJoints;
        if (fingerJoints == null || fingerJoints.Length == 0)
        {
            Debug.LogError("fingerJoints array is null or empty.");
            return; // fingerJoints가 초기화되지 않았거나 비어 있으므로 종료
        }
        
        for (int i = 0; i < fingerJoints.Length; i++)
        {
            if (fingerJoints[i] == null)
            {
                Debug.LogError($"fingerJoints[{i}] is null.");
                return; // 특정 손가락 transform이 null이므로 종료
            }
        }

        // Feedback Layer 오브젝트 가져오기
        GameObject feedbackLayerObject = trackedHand.feedbackLayer.gameObject;
        if (feedbackLayerObject == null)
        {
            Debug.LogError("Feedback Layer object not found on SG_TrackedHand.");
            return; // Feedback Layer 오브젝트를 찾을 수 없으므로 종료
        }

        // Feedback Layer 오브젝트에서 SG_HandFeedback 컴포넌트 가져오기
        SG_HandFeedback handFeedback = feedbackLayerObject.GetComponent<SG_HandFeedback>();
        if (handFeedback == null)
        {
            Debug.LogError("SG_HandFeedback component not found on FeedbackLayer object.");
            return; // SG_HandFeedback 컴포넌트를 찾을 수 없으므로 종료
        }

        // fingerFeedbackScripts 초기화
        fingerFeedbackScripts = handFeedback.fingerFeedbackScripts;
        if (fingerFeedbackScripts == null || fingerFeedbackScripts.Length == 0)
        {
            Debug.LogError("fingerFeedbackScripts array is null or empty.");
            return; // fingerFeedbackScripts가 초기화되지 않았거나 비어 있으므로 종료
        }

        // 각 손가락의 피드백 스크립트가 null이 아닌지 확인
        for (int i = 0; i < fingerFeedbackScripts.Length; i++)
        {
            if (fingerFeedbackScripts[i] == null)
            {
                Debug.LogError($"fingerFeedbackScripts[{i}] is null.");
                return; // 특정 손가락 피드백 스크립트가 null이므로 종료
            }
        }

        // 디버깅: 각 손가락의 ForceLevel 값을 출력
        for (int f = 0; f < fingerFeedbackScripts.Length; f++)
        {
            Debug.Log($"Finger {f} ForceLevel: {fingerFeedbackScripts[f].ForceLevel}");
        }
    }
    void Update()
    {
        if (!isCoroutineRunning)
        {
            if (fingerFeedbackScripts[0].IsTouching() || fingerFeedbackScripts[1].IsTouching() || fingerFeedbackScripts[2].IsTouching() || fingerFeedbackScripts[3].IsTouching())
            {
                for (int i = 0; i < 4; i++)
                {
                    if (fingerFeedbackScripts[i].IsTouching())
                    {
                        touchedMaterial = fingerFeedbackScripts[i].TouchedMaterialScript;
                        break;
                    }
                }
                // Debug.Log("Touching");
                touching = true;
                StartCoroutine(SaveCSVFile());
            }
            else
            {
                if (selectTouchMode == SelectTouchMode.Always)
                {
                    touchedMaterial = null;
                    // Debug.Log("UnTouching");
                    touching = false;
                    StartCoroutine(SaveCSVFile());
                }
            }
        }
    }

    void Awake()
    {
        fileName = nameText + ".csv";
        tempData = new string[31];
        tempData[0] = "Name";
        tempData[1] = "FFB_Thumb";
        tempData[2] = "FFB_Index";
        tempData[3] = "FFB_Middle";
        tempData[4] = "FFB_Ring";
        tempData[5] = "FFB_Pinky";

        tempData[6] = "Thumb_CMC";
        tempData[7] = "Thumb_MCP";
        tempData[8] = "Thumb_IP";
        tempData[9] = "Thumb_FingerTip";

        tempData[10] = "Index_MCP";
        tempData[11] = "Index_PIP";
        tempData[12] = "Index_DIP";
        tempData[13] = "Index_FingerTip";

        tempData[14] = "Middle_MCP";
        tempData[15] = "Middle_PIP";
        tempData[16] = "Middle_DIP";
        tempData[17] = "Middle_FingerTip";

        tempData[18] = "Ring_MCP";
        tempData[19] = "Ring_PIP";
        tempData[20] = "Ring_DIP";
        tempData[21] = "Ring_FingerTip";

        tempData[22] = "Pinky_MCP";
        tempData[23] = "Pinky_PIP";
        tempData[24] = "Pinky_DIP";
        tempData[25] = "Pinky_FingerTip";
        
        if (selectTouchMode == SelectTouchMode.Always)
        {
            tempData[26] = "IsTouching";
        }
        tempData[27] = "MaxForce";
        tempData[28] = "Material";
        tempData[29] = "Rendering Method";
        tempData[30] = "TimeStamp";

        
        data.Add((string[])tempData.Clone());
        string[][] output = new string[data.Count][];
 
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = data[i];
        }

        StringBuilder sb = new StringBuilder();

        int length = output.GetLength(0);
        string delimiter = ",";

        for (int i = 0; i < length; i++)
        {
            sb.AppendLine(string.Join(delimiter, output[i]));
        }
 
        string filepath = SystemPath.GetPath();
        string fullPath = Path.Combine(filepath, fileName);

        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        using (StreamWriter outStream = System.IO.File.CreateText(fullPath))
        {
            outStream.Write(sb.ToString());  // StringBuilder 내용을 문자열로 변환하여 작성
        }

        // 파일 저장 경로를 로그로 출력
        Debug.Log($"File saved to: {fullPath}");
    }

    IEnumerator SaveCSVFile()
    {
        isCoroutineRunning = true;
        
        tempData[0] = nameText;
        float[] forceLevels = new float[5];
        for (int f = 0; f < forceLevels.Length; f++)
        {
            tempData[f+1] = (fingerFeedbackScripts[f].ForceLevel / 100.0f).ToString();
            // Debug.Log(tempData[f+1]);
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                tempData[(i*4)+j+6] = $"Pos(x:{fingerJoints[i][j].position.x}|y:{fingerJoints[i][j].position.y}|z:{fingerJoints[i][j].position.z})|Rot(x:{fingerJoints[i][j].rotation.eulerAngles.x}|y:{fingerJoints[i][j].rotation.eulerAngles.y}|z:{fingerJoints[i][j].rotation.eulerAngles.z})";
            }
        }

        if (selectTouchMode == SelectTouchMode.Always)
        {
            if (touching)
            {
                tempData[26] = "Yes";
            }
            else
            {
                tempData[26] = "No";
            }
        }
        
        if (touching)
        {
            tempData[27] = touchedMaterial.materialProperties.maxForce.ToString();
            tempData[28] = touchedMaterial.gameObject.ToString();
        }
        else
        {
            tempData[27] = null;
            tempData[28] = null;
        }

        if (findRenderingMode == FindRenderingMode.Use)
        {
            if (touching)
            {
                if (touchedMaterial.materialProperties.maxForceDist == 0.0f || this.isLinear(touchedMaterial.materialProperties.forceRepsonse))
                {
                    tempData[29] = "1";
                }
                else
                {
                    tempData[29] = "2";
                }
            }
            else
            {
                tempData[29] = null;
            }
        }
        tempData[30] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        data.Add((string[])tempData.Clone());

        string[][] output = new string[data.Count][];
 
        for (int i = 0; i < output.Length; i++)
        {
            output[i] = data[i];
        }

        StringBuilder sb = new StringBuilder();

        int length = output.GetLength(0);
        string delimiter = ",";

        for (int i = 0; i < length; i++)
        {
            sb.AppendLine(string.Join(delimiter, output[i]));
        }
 
        string filepath = SystemPath.GetPath();

        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        StreamWriter outStream = System.IO.File.CreateText(filepath + fileName);
        outStream.Write(sb);
        outStream.Close();
        
        yield return new WaitForSecondsRealtime(minSaveInterval);
        isCoroutineRunning = false;
    }

    bool isLinear(AnimationCurve curve)
    {
        if (curve.keys.Length == 0)
        {
            // 키가 없으면 일정한 값이 없으므로 false를 반환
            return false;
        }

        // 첫 번째 키프레임의 값을 기준으로 설정
        float firstValue = curve.keys[0].value;

        // 곡선을 일정 간격으로 샘플링하여 값이 동일한지 확인
        int samplePoints = 10;
        for (int i = 0; i <= samplePoints; i++)
        {
            float t = (float)i / samplePoints; // 0.0f에서 1.0f 사이의 값을 얻음
            float sampledValue = curve.Evaluate(t);
            if (Mathf.Abs(sampledValue - firstValue) > 0)
            {
                // 샘플링된 값이 기준값과 다르면 일정한 곡선이 아님
                return false;
            }
        }
        return true;
    }

}