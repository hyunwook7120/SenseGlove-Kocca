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

    string currentButton;



    void Start()
    {
        setMaxForceButton1.onClick.AddListener(() => { SetMaxForce(0.0f); currentButton = "1"; });
        setMaxForceButton2.onClick.AddListener(() => { SetMaxForce(0.1f); currentButton = "2"; });
        setMaxForceButton3.onClick.AddListener(() => { SetMaxForce(0.3f); currentButton = "3"; });
        setMaxForceButton4.onClick.AddListener(() => { SetMaxForce(0.5f); currentButton = "4"; });
        setMaxForceButton5.onClick.AddListener(() => { SetMaxForce(1.0f); currentButton = "5"; });
    }

    void SetMaxForce(float value)
    {
        if (material != null)
        {
            material.SetMaxForce(value);
        }
    }

    public string GetCurrentButton()
    {
        return currentButton;
    }
}