using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIHealthSlider : MonoBehaviour
{
    [SerializeField]
    Slider slider;
    [SerializeField]
    RectTransform rectTransform;

    public AICTRL AI;

    public void iniAIHealth(AICTRL AIfunc) {
        AI = AIfunc;
        if (AI && slider && EnemyIndicators.main)
        {
            slider.maxValue = AI.healthStart;
            if (AI.ID < EnemyIndicators.main.aIHealthSliders.Length)
            { 
                EnemyIndicators.main.aIHealthSliders[AI.ID] = this;
                slider.maxValue = AI.healthStart;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TestPosition();
    }

    bool RayOnEnemy = false;
    float RayTestOld = 0;
    void TestPosition() {
        if (MainCamera.main && AI && MainCamera.mainCamera && rectTransform)
        {
            //Узнаем направление взгляда камеры и вектор направления взгляда до врага
            Vector3 enemyNormal = (AI.gameObject.transform.position - MainCamera.main.transform.position).normalized;

            //Считаем разницу векторов чтобы понять взгляд камеры совпадает или нет с положением врага
            float ViewMagnitude = (enemyNormal - MainCamera.main.transform.forward).magnitude;

            bool sliderView = false;

            if (ViewMagnitude < Calc.hypotenuse1)
            {
                //Делаем тест луча если время пришло
                if ((Time.unscaledTime - RayTestOld) > 0)
                {
                    RayTestOld = Time.unscaledTime + Random.Range(0f, 1.5f);

                    Vector3 VecEnemyToAI = (AI.transform.position - MainCamera.main.transform.position).normalized;
                    Ray rayNow = new Ray(MainCamera.main.transform.position, VecEnemyToAI);
                    RaycastHit MouseRayHit = new RaycastHit();

                    //Проверяем луч на столкновение с чем либо                    
                    if (Physics.Raycast(rayNow, out MouseRayHit, 15f) && AI.gameObject == MouseRayHit.collider.gameObject)
                    {
                        RayOnEnemy = true;
                    }
                    else
                    {
                        RayOnEnemy = false;
                    }
                }


                //Пускаем луч во врага
                if (RayOnEnemy)
                {
                    sliderView = true;
                }
                else
                {
                    sliderView = false;
                }
            }
            else
            {
                sliderView = false;
            }

            slider.gameObject.SetActive(sliderView);
            if (slider.gameObject.active)
            {
                //Нужно получить 2d позицию на экране из 3д в пространсте
                Vector3 ScreenPos = MainCamera.mainCamera.WorldToScreenPoint(AI.gameObject.transform.position + new Vector3(0, 0.5f, 0) + MainCamera.main.transform.up / 4);
                rectTransform.position = ScreenPos;

                //Размер
                float distToCam = Vector3.Distance(MainCamera.mainCamera.transform.position, AI.transform.position);
                //Считаем размер
                float super = 1;
                if (AI.super) super = 2;

                float size = 1 / (distToCam / 3);
                if (size > (3 * super))
                    size = 3 * super;
                else if (size < (0.2f * super))
                    size = 0.2f * super;

                gameObject.transform.localScale = new Vector3(size*super, size, size);
            }
        }
        else {
            if (AI != null && AI.ID < EnemyIndicators.main.aIHealthSliders.Length)
            {
                EnemyIndicators.main.aIHealthSliders[AI.ID] = null;
            }

            Destroy(gameObject);
        }
    }

    public void SetHP() {
        if (AI && slider) {
            if (AI.healthNow > 0)
                slider.value = AI.healthNow;
            else {
                slider.value = 0;
                
            }
        }
    }
}
