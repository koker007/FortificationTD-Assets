using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using Mirror;

public class BuildPanelCTRL : MonoBehaviour
{
    [Header("ImagesButton")]
    [SerializeField]
    Texture2D DestroyBuild;
    [SerializeField]
    Texture2D TerraformingUP;
    [SerializeField]
    Texture2D TerraformingDown;
    [SerializeField]
    Texture2D[] UpgradeTechAccuracy;
    [SerializeField]
    Texture2D[] UpgradeTechDamage;
    [SerializeField]
    Texture2D[] UpgradeTechDistance;
    [SerializeField]
    Texture2D[] UpgradeTechRotate;
    [SerializeField]
    Texture2D[] UpgradeTechSpeedFire;


    [Header("Other")]
    [SerializeField]
    GameObject content;

    [SerializeField]
    GameObject buttonPrefab;
    [SerializeField]
    GameObject buttonTestObj;
    void DelTestButton() {
        if (buttonTestObj != null) {
            Destroy(buttonTestObj);
        }
    }

    [SerializeField]
    GameplayCTRL gameplayCTRL; //для доступа к списку строительства
    void iniGameplay() {
        if (gameplayCTRL == null) {
            GameObject gameplayObj = GameObject.FindGameObjectWithTag("Gameplay");

            if (gameplayObj != null)
                gameplayCTRL = gameplayObj.GetComponent<GameplayCTRL>();
        }
    }

    [SerializeField]
    Player playerMe;

    [SerializeField]
    Vector2Int SelectCellOld = new Vector2Int(0, 0);
    string typeBuildOld = "None";
    int SelectCellHeightOld = 0;

    float timeLastUpdate = 0;
    // Start is called before the first frame update
    void Start()
    {
        DelTestButton();
    }

    // Update is called once per frame
    void Update()
    {
        iniGameplay();
        testPlayer();

        testBildList();
        testButtonsList();
    }

    void testPlayer() {
        if (playerMe == null && gameplayCTRL != null) {
            foreach (Player player in gameplayCTRL.players) {
                if (player.isLocalPlayer) {
                    playerMe = player;

                    if (buyFunc.player == null) {
                        buyFunc.player = player;
                    }
                }
            }
        }
    }

    List<BuildButton> buildButtons = new List<BuildButton>();


    void AddButtonPlatform(Vector3 posPressent)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildPlatform != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.building;
            buildButton.KeyText = "buttonInfoPlatform";
            //создаем презентационнус картинку
            GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            Camera camComponent = camPresent.GetComponent<Camera>();
            buildButton.cameraComponet = camComponent;
            buildButton.renderTexture = new RenderTexture(200, 200, 0);
            buildButton.renderTexture.filterMode = FilterMode.Point;
            camComponent.targetTexture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = buildButton.renderTexture;

            //создаем презентационную модель
            buildButton.buildPresent = Instantiate(gameplayCTRL.buildPlatform);
            buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Platform";
            buildButton.PriceText.text = System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.Platform);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonBlockCar(Vector3 posPressent)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildBlockCrab != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.building;
            buildButton.KeyText = "buttonInfoBlockCar";
            //создаем презентационнус картинку
            GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            Camera camComponent = camPresent.GetComponent<Camera>();
            buildButton.cameraComponet = camComponent;
            buildButton.renderTexture = new RenderTexture(200, 200, 0);
            buildButton.renderTexture.filterMode = FilterMode.Point;
            camComponent.targetTexture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = buildButton.renderTexture;

            //создаем презентационную модель
            buildButton.buildPresent = Instantiate(gameplayCTRL.buildBlockCar);
            buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Hedgehog";
            buildButton.PriceText.text = System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.BlockerCar);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonBlockCrab(Vector3 posPressent) {
        //Создаем кнопку
        if (gameplayCTRL.buildBlockCrab != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.building;
            buildButton.KeyText = "buttonInfoBlockCrab";
            //создаем презентационнус картинку
            GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            Camera camComponent = camPresent.GetComponent<Camera>();
            buildButton.cameraComponet = camComponent;
            buildButton.renderTexture = new RenderTexture(200, 200, 0);
            buildButton.renderTexture.filterMode = FilterMode.Point;
            camComponent.targetTexture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = buildButton.renderTexture;

            //создаем презентационную модель
            buildButton.buildPresent = Instantiate(gameplayCTRL.buildBlockCrab);
            buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Сorridor";
            buildButton.PriceText.text = System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.BlockerCrab);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonDestroy(Vector3 posPressent)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.KeyText = "buttonInfoDestroy";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = DestroyBuild;

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Destroy";
            buildButton.PriceText.text = " "; //System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.Destroy);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonPillBox(Vector3 posPressent)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildPillBox != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.building;
            buildButton.KeyText = "buttonInfoPillBox";
            //создаем презентационнус картинку
            GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            Camera camComponent = camPresent.GetComponent<Camera>();
            buildButton.cameraComponet = camComponent;
            buildButton.renderTexture = new RenderTexture(200, 200, 0);
            buildButton.renderTexture.filterMode = FilterMode.Point;
            camComponent.targetTexture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = buildButton.renderTexture;

            //создаем презентационную модель
            buildButton.buildPresent = Instantiate(gameplayCTRL.buildPillBox);
            buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Pillbox";
            buildButton.PriceText.text = System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.Pillbox);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonTurret(Vector3 posPressent)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.building;
            buildButton.KeyText = "buttonInfoTurret";
            //создаем презентационнус картинку
            GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            Camera camComponent = camPresent.GetComponent<Camera>();
            buildButton.cameraComponet = camComponent;
            buildButton.renderTexture = new RenderTexture(200, 200, 0);
            buildButton.renderTexture.filterMode = FilterMode.Point;
            camComponent.targetTexture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = buildButton.renderTexture;

            //создаем презентационную модель
            buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Turret";
            buildButton.PriceText.text = System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.Turret);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonArtillery(Vector3 posPressent)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildLaser != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.building;
            buildButton.KeyText = "buttonInfoArtillery";
            //создаем презентационнус картинку
            GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            Camera camComponent = camPresent.GetComponent<Camera>();
            buildButton.cameraComponet = camComponent;
            buildButton.renderTexture = new RenderTexture(200, 200, 0);
            buildButton.renderTexture.filterMode = FilterMode.Point;
            camComponent.targetTexture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = buildButton.renderTexture;

            //создаем презентационную модель
            buildButton.buildPresent = Instantiate(gameplayCTRL.buildArtillery);
            buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Artillery";
            buildButton.PriceText.text = System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.Artillery);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonMinigun(Vector3 posPressent) {
        //Создаем кнопку
        if (gameplayCTRL.buildMinigun != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1,1,1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.building;
            buildButton.KeyText = "buttonInfoMinigun";
            //создаем презентационнус картинку
            GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            Camera camComponent = camPresent.GetComponent<Camera>();
            buildButton.cameraComponet = camComponent;
            buildButton.renderTexture = new RenderTexture(200, 200, 0);
            buildButton.renderTexture.filterMode = FilterMode.Point;
            camComponent.targetTexture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = buildButton.renderTexture;

            //создаем презентационную модель
            buildButton.buildPresent = Instantiate(gameplayCTRL.buildMinigun);
            buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Minigun";
            buildButton.PriceText.text = System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();
            
            button.onClick.AddListener(buyFunc.Minigun);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonLasergun(Vector3 posPressent)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildLaser != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.building;
            buildButton.KeyText = "buttonInfoLasergun";
            //создаем презентационнус картинку
            GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            Camera camComponent = camPresent.GetComponent<Camera>();
            buildButton.cameraComponet = camComponent;
            buildButton.renderTexture = new RenderTexture(200, 200, 0);
            buildButton.renderTexture.filterMode = FilterMode.Point;
            camComponent.targetTexture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = buildButton.renderTexture;

            //создаем презентационную модель
            buildButton.buildPresent = Instantiate(gameplayCTRL.buildLaser);
            buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Lasergun";
            buildButton.PriceText.text = System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.LaserGun);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonThunder(Vector3 posPressent)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildLaser != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.building;
            buildButton.KeyText = "buttonInfoThunder";
            //создаем презентационнус картинку
            GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            Camera camComponent = camPresent.GetComponent<Camera>();
            buildButton.cameraComponet = camComponent;
            buildButton.renderTexture = new RenderTexture(200, 200, 0);
            buildButton.renderTexture.filterMode = FilterMode.Point;
            camComponent.targetTexture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = buildButton.renderTexture;

            //создаем презентационную модель
            buildButton.buildPresent = Instantiate(gameplayCTRL.buildThunder);
            buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Thunder";
            buildButton.PriceText.text = System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.Thunder);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonRocketLauncher(Vector3 posPressent) {
        //Создаем кнопку
        if (gameplayCTRL.buildRocketLauncher != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.building;
            buildButton.KeyText = "buttonInfoRocketLauncher";
            //создаем презентационнус картинку
            GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            Camera camComponent = camPresent.GetComponent<Camera>();
            buildButton.cameraComponet = camComponent;
            buildButton.renderTexture = new RenderTexture(200, 200, 0);
            buildButton.renderTexture.filterMode = FilterMode.Point;
            camComponent.targetTexture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = buildButton.renderTexture;

            //создаем презентационную модель
            buildButton.buildPresent = Instantiate(gameplayCTRL.buildRocketLauncher);
            buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Rockets";
            buildButton.PriceText.text = System.Convert.ToString(buildButton.building.price);

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.RocketLauncher);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonTerraformUP(Vector3 posPressent, Vector2Int SelectCellNow) {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.KeyText = "buttonInfoTerraformingUP";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = TerraformingUP;

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Terra UP";
            buildButton.PriceText.text = System.Convert.ToString(gameplayCTRL.CalcTerraforming(true, SelectCellNow, false));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.TerraformingUP);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonTerraformDown(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.KeyText = "buttonInfoTerraformingDown";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = TerraformingDown;

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Terra Down";
            buildButton.PriceText.text = System.Convert.ToString(gameplayCTRL.CalcTerraforming(false, SelectCellNow, false));

            Button button = buildButton.GetComponent<Button>();
            button.onClick.AddListener(buyFunc.TerraformingDOWN);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }


    void AddButtonResAccuracy1(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResAccuracy1";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = UpgradeTechAccuracy[0];

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Accuracy I";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.accuracy1Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllAccuracy1);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonResAccuracy2(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResAccuracy2";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = UpgradeTechAccuracy[1];

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Accuracy II";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.accuracy2Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllAccuracy2);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonResAccuracy3(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResAccuracy3";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = UpgradeTechAccuracy[2];

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Accuracy III";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.accuracy3Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllAccuracy3);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonResSpeed1(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResSpeed1";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = UpgradeTechSpeedFire[0];

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Speed I";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.speed1Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllSpeed1);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonResSpeed2(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResSpeed2";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = UpgradeTechSpeedFire[1];

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Speed II";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.speed2Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllSpeed2);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonResSpeed3(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResSpeed3";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = UpgradeTechSpeedFire[2];

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Speed III";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.speed3Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllSpeed3);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonResDamage1(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResDamage1";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = UpgradeTechDamage[0];

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Damage I";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.damage1Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllDamage1);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonResDamage2(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResDamage2";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = UpgradeTechDamage[1];

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Damage II";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.damage2Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllDamage2);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonResDamage3(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResDamage3";
            //создаем презентационнус картинку
            //GameObject camPresent = Instantiate(buildButton.PrefabCamPresent); //создаем камеру
            //camPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10 + 1, posPressent.y + 1f, posPressent.z + 1);
            //Camera camComponent = camPresent.GetComponent<Camera>();
            //buildButton.cameraComponet = camComponent;
            //buildButton.renderTexture = new RenderTexture(200, 200, 0);
            //buildButton.renderTexture.filterMode = FilterMode.Point;
            //camComponent.targetTexture = buildButton.renderTexture;
            //buildButton.rawImagePresent.texture = buildButton.renderTexture;
            buildButton.rawImagePresent.texture = UpgradeTechDamage[2];

            //создаем презентационную модель
            //buildButton.buildPresent = Instantiate(gameplayCTRL.buildTurret);
            //buildButton.buildPresent.transform.position = new Vector3(posPressent.x + buildButtons.Count * 10, posPressent.y, posPressent.z);

            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Damage III";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.damage3Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllDamage3);

            //Удаляем сетевые обьекты из преваба поскольку это лишь презентация
            //NetworkTransform networkTransform = buildButton.buildPresent.GetComponent<NetworkTransform>();
            //if (networkTransform != null) Destroy(networkTransform);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonResDistance1(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResDistance1";


            buildButton.rawImagePresent.texture = UpgradeTechDistance[0];


            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Distance I";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.dist1Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllDistance1);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonResDistance2(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResDistance2";


            buildButton.rawImagePresent.texture = UpgradeTechDistance[1];


            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Distance II";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.dist2Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllDistance2);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonResDistance3(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResDistance3";


            buildButton.rawImagePresent.texture = UpgradeTechDistance[2];


            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Distance III";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.dist3Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllDistance3);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    void AddButtonResRotate1(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResRotate1";


            buildButton.rawImagePresent.texture = UpgradeTechRotate[0];


            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Rotate I";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.rotate1Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllRotate1);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonResRotate2(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResRotate2";


            buildButton.rawImagePresent.texture = UpgradeTechRotate[1];


            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Rotate II";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.rotate2Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllRotate2);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }
    void AddButtonResRotate3(Vector3 posPressent, Vector2Int SelectCellNow)
    {
        //Создаем кнопку
        if (gameplayCTRL.buildTurret != null)
        {
            //Создаем кнопку
            GameObject buttonObj = Instantiate(buttonPrefab);
            buttonObj.transform.parent = content.transform;
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.localPosition = new Vector3(4 + buttonRect.sizeDelta.x * buildButtons.Count, buttonRect.position.y);
            buttonRect.localScale = new Vector3(1, 1, 1);

            BuildButton buildButton = buttonObj.GetComponent<BuildButton>();
            buildButton.SetKeyPress(buildButtons.Count + 1);
            buildButton.type = BuildButton.Type.upgrade;
            buildButton.KeyText = "buttonInfoResRotate3";


            buildButton.rawImagePresent.texture = UpgradeTechRotate[2];


            //buildButton.building = buildButton.buildPresent.GetComponent<Building>();
            buildButton.NameText.text = "Rotate III";
            buildButton.PriceText.text = System.Convert.ToString((int)(GPResearch.Allgun.rotate3Cost * gameplayCTRL.cellsData[(int)SelectCellNow.x, (int)SelectCellNow.y].cost * 0.01f));

            Button button = buildButton.GetComponent<Button>();

            button.onClick.AddListener(buyFunc.ResAllRotate3);

            //Сохраняем в список для дальнейшего использования
            buildButtons.Add(buildButton);
        }
    }

    //Функция создающая кнопки иконки построек и прочее
    void testBildList() {
        //Выходим из функции если хотя бы что-то не было инициализировано
        if (buttonPrefab == null || gameplayCTRL == null || playerMe == null) {
            return;
        }

        //Проверяем какая ячейка выделена
        Vector2Int SelectCellNow = new Vector2Int(0, 0);
        string typeBuildNow = "";
        if (playerMe.controlsMouse.SelectCellCTRL != null) {
            SelectCellNow = new Vector2Int(playerMe.controlsMouse.SelectCellCTRL.posX, playerMe.controlsMouse.SelectCellCTRL.posY);
            typeBuildNow = gameplayCTRL.cellsData[(int)playerMe.controlsMouse.SelectCellCTRL.posX, (int)playerMe.controlsMouse.SelectCellCTRL.posY].build;
        }

        //Если нужно перерисовать
        if (SelectCellOld != SelectCellNow ||
            typeBuildOld != typeBuildNow ||
            SelectCellHeightOld != gameplayCTRL.cellsData[SelectCellNow.x,SelectCellNow.y].height ||
            timeLastUpdate < gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].timeLastUpdate) {

            SelectCellOld = SelectCellNow;
            typeBuildOld = typeBuildNow;
            SelectCellHeightOld = gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].height;
            timeLastUpdate = Time.unscaledTime;

            buyFunc.buildPos = new Vector2Int(SelectCellNow.x, SelectCellNow.y);


            //Чистим предыдущие ячейки
            foreach (BuildButton buildButton in buildButtons) {
                if (buildButton != null) {
                    if (buildButton.buildPresent != null)
                        Destroy(buildButton.buildPresent);
                    if (buildButton.cameraComponet != null)
                        Destroy(buildButton.cameraComponet.gameObject);

                    Destroy(buildButton.gameObject);

                }
            }
            buildButtons.Clear();

            //Проверка что строительство не сломает маршрут

            //Ячейка есть, проверяем ее статус

            Vector3 posPressent = new Vector3(500, -500, 500); //стартовая позиция для презентации моделей построек
            int buttonsSum = 0; //Количество созданных кнопок

            //Если свободная
            if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].build == Building.getTypeName(Building.Type.None) && SelectCellNow != new Vector2Int(0, 0))
            {
                //Проверяем можно ли строить на этой ячейке
                if (gameplayCTRL.CalcTraficAll(false, new Vector2Int(SelectCellNow.x, SelectCellNow.y), false) && gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].height > 1)
                {
                    //Создаем кнопки
                    AddButtonPlatform(posPressent);
                    AddButtonBlockCar(posPressent);
                    AddButtonBlockCrab(posPressent);

                    if (gameplayCTRL.CalcTerraforming(true, SelectCellNow, false) != 0)
                        AddButtonTerraformUP(posPressent, SelectCellNow);
                    if (gameplayCTRL.CalcTerraforming(false, SelectCellNow, false) != 0)
                        AddButtonTerraformDown(posPressent, SelectCellNow);
                    //AddButtonTurret(posPressent);
                    //AddButtonMinigun(posPressent);
                    //AddButtonLasergun(posPressent);
                }
            }
            //Если ограничитель периметра
            else if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].build == Building.getTypeName(Building.Type.TraficBlocker))
            {
                //Если платформа принадлежит нам
                if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ownerSteamID == playerMe.SteamID)
                {
                    AddButtonDestroy(posPressent);
                }
            }

            //Если платформа
            else if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].build == Building.getTypeName(Building.Type.Platform))
            {
                //Если платформа принадлежит нам
                if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ownerSteamID == playerMe.SteamID)
                {
                    AddButtonDestroy(posPressent);
                    AddButtonPillBox(posPressent);
                    AddButtonTurret(posPressent);

                    if(GPResearch.allGun.buildMinigun)
                        AddButtonMinigun(posPressent);
                    if(GPResearch.allGun.buildLaser)
                        AddButtonLasergun(posPressent);
                    if(GPResearch.allGun.buildRocket)
                        AddButtonRocketLauncher(posPressent);
                }
            }

            else if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].build == Building.getTypeName(Building.Type.PillBox))
            {
                AddButtonDestroy(posPressent);

                //Точность
                accuracy();

                //скорость
                speedFire();
                //урон
                damage();
                distance();
                rotate();

            }
            //Если обычная турель
            else if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].build == Building.getTypeName(Building.Type.Turret))
            {
                AddButtonDestroy(posPressent);

                //Точность
                accuracy();

                //скорость
                speedFire();
                //урон
                damage();
                distance();
                rotate();

                //Если мы владелец
                if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ownerSteamID == playerMe.SteamID)
                {
                    if(GPResearch.allGun.buildArtillery)
                        AddButtonArtillery(posPressent);
                }
            }
            else if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].build == Building.getTypeName(Building.Type.Artillery))
            {
                AddButtonDestroy(posPressent);

                accuracy();
                //скорость
                speedFire();
                //урон
                damage();
                distance();
                rotate();

            }
            //Если миниган
            else if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].build == Building.getTypeName(Building.Type.Minigun))
            {
                AddButtonDestroy(posPressent);

                accuracy();
                //скорость
                speedFire();
                //урон
                damage();
                distance();
                rotate();

            }
            //Лазер
            else if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].build == Building.getTypeName(Building.Type.Laser))
            {
                AddButtonDestroy(posPressent);

                accuracy();
                //скорость
                speedFire();
                //урон
                damage();
                distance();
                rotate();

                //Если лазер принадлежит нам
                if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ownerSteamID == playerMe.SteamID)
                {
                    if(GPResearch.allGun.buildThunder)
                        AddButtonThunder(posPressent);
                }
            }
            //Гром
            else if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].build == Building.getTypeName(Building.Type.Thunder))
            {
                AddButtonDestroy(posPressent);

                accuracy();
                //скорость
                speedFire();
                //урон
                damage();
                distance();
                rotate();
            }
            else if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].build == Building.getTypeName(Building.Type.Rocket))
            {
                AddButtonDestroy(posPressent);

                accuracy();
                //скорость
                speedFire();
                //урон
                damage();
                distance();
                rotate();

            }

            void accuracy() {
                if (GPResearch.allGun.accuracy1 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.accuracy1)
                    AddButtonResAccuracy1(posPressent, SelectCellNow);
                else if (GPResearch.allGun.accuracy2 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.accuracy2)
                    AddButtonResAccuracy2(posPressent, SelectCellNow);
                else if (GPResearch.allGun.accuracy3 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.accuracy3)
                    AddButtonResAccuracy3(posPressent, SelectCellNow);
            }
            void speedFire() {
                if (GPResearch.allGun.speed1 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.speed1)
                    AddButtonResSpeed1(posPressent, SelectCellNow);
                else if (GPResearch.allGun.speed2 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.speed2)
                    AddButtonResSpeed2(posPressent, SelectCellNow);
                else if (GPResearch.allGun.speed3 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.speed3)
                    AddButtonResSpeed3(posPressent, SelectCellNow);
            }
            void damage() {
                if (GPResearch.allGun.damage1 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.damage1)
                    AddButtonResDamage1(posPressent, SelectCellNow);
                else if (GPResearch.allGun.damage2 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.damage2)
                    AddButtonResDamage2(posPressent, SelectCellNow);
                else if (GPResearch.allGun.damage3 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.damage3)
                    AddButtonResDamage3(posPressent, SelectCellNow);
            }
            void rotate()
            {
                if (GPResearch.allGun.rotate1 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.rotate1)
                    AddButtonResRotate1(posPressent, SelectCellNow);
                else if (GPResearch.allGun.rotate2 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.rotate2)
                    AddButtonResRotate2(posPressent, SelectCellNow);
                else if (GPResearch.allGun.rotate3 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.rotate3)
                    AddButtonResRotate3(posPressent, SelectCellNow);
            }
            void distance()
            {
                if (GPResearch.allGun.dist1 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.dist1)
                    AddButtonResDistance1(posPressent, SelectCellNow);
                else if (GPResearch.allGun.dist2 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.dist2)
                    AddButtonResDistance2(posPressent, SelectCellNow);
                else if (GPResearch.allGun.dist3 && !gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].ResearchAllGun.dist3)
                    AddButtonResDistance3(posPressent, SelectCellNow);
            }
        }
    }

    //Функция проверяющее текущее состояние для созданных кнопок
    void testButtonsList() {
        if (buildButtons == null || playerMe == null || gameplayCTRL == null) {
            return;
        }

        //Проверяем какая ячейка выделена
        Vector2Int SelectCellNow = new Vector2Int(0, 0);
        if (playerMe.controlsMouse.SelectCellCTRL != null)
        {
            SelectCellNow = new Vector2Int(playerMe.controlsMouse.SelectCellCTRL.posX, playerMe.controlsMouse.SelectCellCTRL.posY);
        }

        //Считаем время до конца кд
        float cooldownTime = (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].timeLastBuilding + gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].timeoutBuild) - gameplayCTRL.timeGamePlay;
        float nextMaximumTech = (60 * 10) - gameplayCTRL.timeGamePlay % (60 * 10); //время до следуюущего увеличения апгрейдов
        //Debug.Log("cooldownTime:" + cooldownTime + "  nextMaximumTech:" + nextMaximumTech);
        foreach (BuildButton buildButton in buildButtons)
        {

            //Если это строение и денег не хватает
            if (buildButton.building != null && playerMe.Money < buildButton.building.price)
            {
                setLock();
            }
            //Если это апгрейд и денег не хватает
            else if (buildButton.type == BuildButton.Type.upgrade && playerMe.Money < System.Convert.ToDouble(buildButton.PriceText.text)) {
                setLock();
            }
            //Если это апгрейд
            else if (buildButton.type == BuildButton.Type.upgrade
                //и количество апгрейдов равняется максимуму
                && gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].getCountTech() == Tech.getMaxCountTechThisGameplayTime() + gameplayCTRL.buildings[SelectCellNow.x, SelectCellNow.y].StartUpgrages
                //и время до увеличения максимума апгрейдов больше чем кулдаун
                && nextMaximumTech > cooldownTime) {
                buildButton.Lock.enabled = true;
                buildButton.CoolDown.enabled = true;
                //Общая длина задержки
                float lenghtCooldown = (gameplayCTRL.timeGamePlay + nextMaximumTech) - gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].timeLastBuilding;

                float amount = nextMaximumTech / lenghtCooldown;
                buildButton.CoolDown.fillAmount = amount;
            }
            //Если кд
            else if (gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].timeLastBuilding + gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].timeoutBuild > gameplayCTRL.timeGamePlay &&
                buildButton.type != BuildButton.Type.none) {
                buildButton.Lock.enabled = true;
                buildButton.CoolDown.enabled = true;

                float amount = (gameplayCTRL.timeGamePlay - gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].timeLastBuilding) / gameplayCTRL.cellsData[SelectCellNow.x, SelectCellNow.y].timeoutBuild;
                buildButton.CoolDown.fillAmount = 1 - amount;
            }
            else {
                buildButton.Lock.enabled = false;
                buildButton.CoolDown.enabled = false;
            }

            void setLock() {
                buildButton.Lock.enabled = true;
                buildButton.CoolDown.enabled = true;
                buildButton.CoolDown.fillAmount = 1;
            }
        }
    }

    public struct BuyFunc
    {
        public Vector2 buildPos;
        public Player player;

        public void Destroy() {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.None));
            }
            Debug.Log("buy destroy");
        }
        public void Platform()
        {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.Platform));
            }
            Debug.Log("buy platform");
        }

        public void BlockerCar()
        {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.TraficBlocker) + "Car");
            }
            Debug.Log("buy block car");
        }

        public void BlockerCrab() {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.TraficBlocker)+"Crab");
            }
            Debug.Log("buy block crab");
        }

        public void Pillbox()
        {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.PillBox));
            }
            Debug.Log("buy pillbox");
        }
        public void Turret()
        {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.Turret));
            }
            Debug.Log("buy turret");
        }
        public void Artillery()
        {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.Artillery));
            }
            Debug.Log("buy Artillery");
        }
        public void Minigun() {
            if (player != null && player.isLocalPlayer) {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.Minigun));
            }
            Debug.Log("buy minigun");
        }

        public void LaserGun()
        {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.Laser));
            }
            Debug.Log("buy lasergun");
        }

        public void Thunder()
        {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.Thunder));
            }
            Debug.Log("buy thunder");
        }

        public void RocketLauncher()
        {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdBuild(buildPos, Building.getTypeName(Building.Type.Rocket));
            }
            Debug.Log("buy rocket");
        }

        public void TerraformingUP()
        {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdTerraform(buildPos, true);
            }
            Debug.Log("buy terraform UP");
        }
        public void TerraformingDOWN()
        {
            if (player != null && player.isLocalPlayer)
            {
                player.CmdTerraform(buildPos, false);
            }
            Debug.Log("buy terraform DOWN");
        }

        public void ResAllAccuracy1() {
            string researchName = GPResearch.Allgun.accuracy1STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }
        public void ResAllAccuracy2()
        {
            string researchName = GPResearch.Allgun.accuracy2STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }
        public void ResAllAccuracy3()
        {
            string researchName = GPResearch.Allgun.accuracy3STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }

        public void ResAllDamage1()
        {
            string researchName = GPResearch.Allgun.damage1STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }
        public void ResAllDamage2()
        {
            string researchName = GPResearch.Allgun.damage2STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }
        public void ResAllDamage3()
        {
            string researchName = GPResearch.Allgun.damage3STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }

        public void ResAllDistance1()
        {
            string researchName = GPResearch.Allgun.dist1STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }
        public void ResAllDistance2()
        {
            string researchName = GPResearch.Allgun.dist2STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }
        public void ResAllDistance3()
        {
            string researchName = GPResearch.Allgun.dist3STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }

        public void ResAllRotate1()
        {
            string researchName = GPResearch.Allgun.rotate1STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }
        public void ResAllRotate2()
        {
            string researchName = GPResearch.Allgun.rotate2STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }
        public void ResAllRotate3()
        {
            string researchName = GPResearch.Allgun.rotate3STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }

        public void ResAllSpeed1()
        {
            string researchName = GPResearch.Allgun.speed1STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }
        public void ResAllSpeed2()
        {
            string researchName = GPResearch.Allgun.speed2STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }
        public void ResAllSpeed3()
        {
            string researchName = GPResearch.Allgun.speed3STR;

            if (player != null && player.isLocalPlayer)
            {
                player.CmdResearch(buildPos, researchName);
            }
            Debug.Log("buy research " + researchName);
        }

    }

    BuyFunc buyFunc = new BuyFunc();
}
