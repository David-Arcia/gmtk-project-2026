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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EnergyAmount = MaxEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        EnergyAmount -= energyDepletionRate * Time.deltaTime;
        if (EnergyAmount < 0)
        {
            EnergyAmount = 0;
        }
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
}
