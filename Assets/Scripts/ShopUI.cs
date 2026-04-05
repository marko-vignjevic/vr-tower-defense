using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("References")]
    public TowerPlacer towerPlacer;
    public GameObject shopCanvas;

    [Header("Coins Display")]
    public TextMeshProUGUI coinsText;

    [Header("Tower Buttons")]
    public Button[] towerButtons;
    public TextMeshProUGUI[] towerButtonTexts;

    [Header("Tower Data")]
    public TowerData[] towers;

    [System.Serializable]
    public class TowerData
    {
        public string towerName;
        public GameObject towerPrefab;
        public int cost;
    }

    private bool isOpen = false;

    void Start()
    {
        shopCanvas.SetActive(false);

        for (int i = 0; i < towerButtons.Length; i++)
        {
            if (i < towers.Length)
                towerButtonTexts[i].text = towers[i].towerName + "\n" + towers[i].cost + " coins";
        }
    }

    void Update()
    {
        // toggle shop with Y button on left controller
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
            ToggleShop();


        UpdateCoinsDisplay();
    }

    void ToggleShop()
    {
        // Toggle the shop UI visibility
        isOpen = !isOpen;
        shopCanvas.SetActive(isOpen);

        if (isOpen)
            UpdateCoinsDisplay();
    }

    void UpdateCoinsDisplay()
    {
        // Update the coins display text
        coinsText.text = "Coins: " + PlayerManager.Instance.GetCoins();
    }

    public void SelectTower(int index)
    {
        // Start placing the selected tower
        if (index >= towers.Length) return;

        TowerData data = towers[index];

        towerPlacer.StartPlacing(data.towerPrefab, data.cost);

        // close shop after selecting
        isOpen = false;
        shopCanvas.SetActive(false);
    }
}