using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get all children as a list
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        // Shuffle the list
        children.Shuffle();

        // Show only the first game object
        for (int i = 0; i < children.Count; i++)
        {
            GameObject child = children[i];
            child.SetActive(i == 0);
        }
    }
}
