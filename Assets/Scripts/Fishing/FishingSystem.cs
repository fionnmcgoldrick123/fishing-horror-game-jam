using System.Collections.Generic;
using UnityEngine;

public class FishingSystem : MonoBehaviour
{
    public List<FishScriptableObject> fishList; 

    public FishScriptableObject GetRandomFish()
    {
        float luckMultiplier = UpgradeManager.GetLuckMultiplier();

        float totalWeight = 0;
        foreach (var fish in fishList)
        {
            float weight = fish.rarityWeight;
            if (fish.rarity != Rarity.Common)
                weight *= luckMultiplier;
            totalWeight += weight;
        }

        float randomWeight = Random.Range(0f, totalWeight);
        float currentWeight = 0;

        foreach (var fish in fishList)
        {
            float weight = fish.rarityWeight;
            if (fish.rarity != Rarity.Common)
                weight *= luckMultiplier;
            currentWeight += weight;
            if (randomWeight < currentWeight)
            {
                return fish;
            }
        }

        return fishList[fishList.Count - 1];
    }
}
