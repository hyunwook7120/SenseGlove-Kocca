using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveController : MonoBehaviour
{
    public AnimationCurve shift_1 = AnimationCurve.Constant(0, 1, 1);
    public AnimationCurve shift_2 = AnimationCurve.Constant(0, 1, 1);
    public AnimationCurve shift_3 = AnimationCurve.Constant(0, 1, 1);
    public AnimationCurve shift_4 = AnimationCurve.Constant(0, 1, 1);
    public AnimationCurve shift_5 = AnimationCurve.Constant(0, 1, 1);

    public AnimationCurve impedence = AnimationCurve.Constant(0, 1, 1);
}
