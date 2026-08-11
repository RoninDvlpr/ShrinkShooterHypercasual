using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPath : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [Tooltip("The coefficient that determines the width based on the player's scale.")]
    [SerializeField] float widthCoefficient = 0.85f;

    void OnEnable()
    {
        player.OnScaleUpdated += UpdateWidth;
    }

    void OnDisable()
    {
        player.OnScaleUpdated -= UpdateWidth;
    }

    void UpdateWidth()
    {
        float width = player.transform.localScale.x * widthCoefficient;
        transform.localScale = new Vector3(transform.localScale.x, width, transform.localScale.z);
    }
}
