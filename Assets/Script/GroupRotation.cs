using UnityEngine;

public class GroupRotation : MonoBehaviour
{
    public Transform handTransform; // 손의 Transform
    public Transform forearmTransform; // 팔뚝의 Transform
    public Transform upperArmTransform; // 상완의 Transform

    void Update()
    {
        // 팔뚝과 상완이 손의 회전을 따라가도록 설정
        forearmTransform.rotation = Quaternion.Lerp(forearmTransform.rotation, handTransform.rotation, Time.deltaTime * 5);
        upperArmTransform.rotation = Quaternion.Lerp(upperArmTransform.rotation, handTransform.rotation, Time.deltaTime * 5);
    }
}