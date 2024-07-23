using UnityEngine;
using UnityEngine.UI;

public class MaterialSettingsUI : MonoBehaviour
{
    public SG.SG_Material material; // SG_Material 스크립트가 연결된 게임 오브젝트

    public Button setMaxForceButton1;
    public Button setMaxForceButton2;
    public Button setMaxForceDistanceButton1;
    public Button setMaxForceDistanceButton2;

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        setMaxForceButton1.onClick.AddListener(() => SetMaxForce(0.0f));
        setMaxForceButton2.onClick.AddListener(() => SetMaxForce(0.1f));
        setMaxForceDistanceButton1.onClick.AddListener(() => SetMaxForceDistance(0.0f));
        setMaxForceDistanceButton2.onClick.AddListener(() => SetMaxForceDistance(0.0f));
    }

    void SetMaxForce(float value)
    {
        if (material != null)
        {
            material.SetMaxForce(value);
        }
    }

    void SetMaxForceDistance(float value)
    {
        if (material != null)
        {
            material.SetMaxForceDistance(value);
        }
    }
}