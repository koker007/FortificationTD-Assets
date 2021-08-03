using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FonSilhouette : MonoBehaviour
{
    [SerializeField]
    Texture2D[] images;
    [SerializeField]
    RawImage raw;

    // Update is called once per frame
    void Update()
    {
        testLive();
    }

    bool isFromLeft = false;
    float startLive = 0;

    public void iniSilhouette()
    {
        startLive = Time.unscaledTime;
        RectTransform rect = GetComponent<RectTransform>();
        if (rect && raw && images != null && images.Length > 0)
        {

            if (Random.Range(0.0f, 100f) < 50)
                isFromLeft = true;

            if (isFromLeft)
            {
                rect.pivot = new Vector2(-5, 0.5f);
            }
            else
            {
                rect.pivot = new Vector2(5, 0.5f);
                Rect imageRect = raw.uvRect;
                imageRect.x = 1;
                raw.uvRect = imageRect;
            }

            raw.texture = images[Random.Range(0, images.Length)];
        }
    }

    float timeFromOldUpdate = 0;
    void testLive() {
        float timeLive = Time.unscaledTime - startLive;
        if (timeLive > 15f) {
            Destroy(gameObject);
        }

        //Для эфекта кадров
        timeFromOldUpdate += Time.unscaledDeltaTime;

        if (timeFromOldUpdate > 0.10f) {
            RectTransform rect = GetComponent<RectTransform>();
            if (isFromLeft)
                rect.pivot = new Vector2(rect.pivot.x + timeFromOldUpdate * 0.8f, 0.5f + Mathf.Sin(Time.unscaledTime * 7) / 40);
            else rect.pivot = new Vector2(rect.pivot.x - timeFromOldUpdate * 0.8f, 0.5f + Mathf.Sin(Time.unscaledTime * 7) / 40);

            timeFromOldUpdate = 0;
        }
    }
}
