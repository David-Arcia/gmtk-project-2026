using UnityEngine;
using UnityEngine.UI;

public class EnergyController : MonoBehaviour
{
    [SerializeField]
    public float MaxEnergy = 100;
    public float EnergyAmount;
    [SerializeField]
    public float energyDepletionRate = 2;
    [SerializeField]
    public float enemyEnergyReplenishment = 15;
    [SerializeField]
    public float enemyEnergyDamage = 25;
    [SerializeField]
    public GameObject energyFillBarUI;
    private RectTransform fillBarTransform;
    private Vector2 initfillBarPos;
    private Vector2 initFillBarScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnergyAmount = MaxEnergy;
        fillBarTransform = energyFillBarUI.GetComponent<RectTransform>();
        initfillBarPos = new Vector2(fillBarTransform.position.x, fillBarTransform.position.y);
        initFillBarScale = new Vector2(fillBarTransform.rect.width, fillBarTransform.rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        EnergyAmount -= energyDepletionRate * Time.deltaTime;
        if (EnergyAmount < 0)
        {
            EnergyAmount = 0;
        }
        SetEnergyBarUI(EnergyAmount);
    }

    public void AddEnergyFromFallenFoe()
    {
        EnergyAmount += enemyEnergyReplenishment;
        if (EnergyAmount > MaxEnergy)
        {
            EnergyAmount = MaxEnergy;
        }
    }

    public void RemoveEnergyFromCollisionWithFoe()
    {
        EnergyAmount -= enemyEnergyDamage;
        if (EnergyAmount < 0)
        {
            EnergyAmount = 0;
        }
    }

    void SetEnergyBarUI(float energyAmount)
    {
        float energyPercent = energyAmount / MaxEnergy;
        //Set dimensions
        fillBarTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, initFillBarScale.x * energyPercent);
        //Set translation
        float sizeDiff = initFillBarScale.x - fillBarTransform.rect.width;
        fillBarTransform.position = new Vector2(initfillBarPos.x - (0.5f * sizeDiff), initfillBarPos.y);
    }
}
