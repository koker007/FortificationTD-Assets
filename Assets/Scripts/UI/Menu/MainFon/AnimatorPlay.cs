using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatorPlay : MonoBehaviour
{

    [SerializeField]
    Animation animation;
    [SerializeField]
    Image image;

    // Start is called before the first frame update
    void Start()
    {
        iniImage();
    }

    void iniImage() {
        if (image) {
            image.color = new Color(image.color.r,  image.color.g, image.color.b, 0);
        }
    }

    public void play() {
        if (animation) {
            animation.Play();
        }
    }
}
