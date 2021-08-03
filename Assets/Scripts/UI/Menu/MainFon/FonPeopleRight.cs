using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FonPeopleRight : MonoBehaviour
{
    [SerializeField]
    AnimatorPlay Point1;
    [SerializeField]
    AnimatorPlay Point2;
    [SerializeField]
    AnimatorPlay Point3;
    [SerializeField]
    AnimatorPlay Point4;

    void playPoint1() {
        if (Point1) Point1.play();
    }
    void playPoint2() {
        if (Point2) Point2.play();
    }
    void playPoint3()
    {
        if (Point3) Point3.play();
    }
    void playPoint4()
    {
        if (Point4) Point4.play();
    }
}
