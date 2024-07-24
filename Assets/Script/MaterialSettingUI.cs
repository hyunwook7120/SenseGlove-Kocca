using UnityEngine;
using UnityEngine.UI;

public class MaterialSettingsUI : MonoBehaviour
{
    public SG.SG_Material material; // SG_Material 스크립트가 연결된 게임 오브젝트

    public Button setMaxForceButton1;
    public Button setMaxForceButton2;
    public Button setMaxForceButton3;
    public Button setMaxForceButton4;
    public Button setMaxForceButton5;
    public Button setMaxForceButton6;



    void Start()
    {
        setMaxForceButton1.onClick.AddListener(() => SetMaxForce(0.0f));
        setMaxForceButton2.onClick.AddListener(() => SetMaxForce(0.1f));
        setMaxForceButton3.onClick.AddListener(() => SetMaxForce(0.3f));
        setMaxForceButton4.onClick.AddListener(() => SetMaxForce(0.4f));
        setMaxForceButton5.onClick.AddListener(() => SetMaxForce(0.6f));
        setMaxForceButton6.onClick.AddListener(() => SetMaxForce(1.0f));

    }

    void SetMaxForce(float value)
    {
        if (material != null)
        {
            material.SetMaxForce(value);
        }
    }
}