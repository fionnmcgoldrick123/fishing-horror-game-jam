using System.Collections.Generic;
using UnityEngine;

public class FishingSystem : MonoBehaviour
{
    public List<FishScriptableObject> fishList; 

    public FishScriptableObject GetRandomFish()
    {
        int totalWeight = 0;
        foreach (var fish in fishList)
        {
            totalWeight += fish.rarityWeight;
        }

        int randomWeight = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var fish in fishList)
        {
            currentWeight += fish.rarityWeight;
            if (randomWeight < currentWeight)
            {
                return fish;
            }
        }

        return null; // Should never reach here
    }
}
