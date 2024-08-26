using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text; // StringBuilder용도
using SG;
using UnityEngine;
using UnityEngine.UI;

public class CSVManager : MonoBehaviour
{
    private bool isCoroutineRunning = false;
    //------------------------------------------------------------------------------------------------------------------------
    // Member Variables

    public GameObject nova2Glove;
    SG_FingerFeedback[] fingerFeedbackScripts;
    Transform[][] fingerJoints;

    public string fileName = "hand.csv";
    
    public string nameText;

    List<string[]> data = new List<string[]>();
    string[] tempData;

    public void Start()
    {
        // nova2Glove 오브젝트를 넣었는지 확인
        if (nova2Glove == null)
        {
            Debug.Log("Nova2Glove is Null.");
            return; // nova2Glove 오브젝트를 찾을 수 없으므로 종료
        }

        // SG_TrackedHand 컴포넌트를 가져옴
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
                Debug.Log("Touching");
                StartCoroutine(SaveCSVFile());
            }
        }
    }

    void Awake()
    {
        tempData = new string[26];
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

        //tempData[22] = "Wrist";
        
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
    }

    IEnumerator SaveCSVFile()
    {
        isCoroutineRunning = true;
        
        tempData[0] = nameText;
        float[] forceLevels = new float[5];
        for (int f = 0; f < forceLevels.Length; f++)
        {
            tempData[f+1] = (fingerFeedbackScripts[f].ForceLevel / 100.0f).ToString();
            Debug.Log(tempData[f+1]);
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                tempData[(i*4)+j+6] = $"Pos(x:{fingerJoints[i][j].position.x}|y:{fingerJoints[i][j].position.y}|z:{fingerJoints[i][j].position.z})|Rot(x:{fingerJoints[i][j].rotation.eulerAngles.x}|y:{fingerJoints[i][j].rotation.eulerAngles.y}|z:{fingerJoints[i][j].rotation.eulerAngles.z})";
            }
        }
        
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
        
        yield return new WaitForSecondsRealtime(5.0f);
        isCoroutineRunning = false;
    }

    
}
