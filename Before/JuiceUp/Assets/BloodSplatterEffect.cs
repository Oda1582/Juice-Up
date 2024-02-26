using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodSplatterEffect : MonoBehaviour
{
    public Sprite[] bloodSprites; // Array of blood splatter sprites
    public float fadeDuration = 1f; // Duration of the fade effect
    public float maxAlpha = 0.8f; // Maximum alpha value for the blood splatter

    private Image bloodImage;
    private float currentAlpha;

    private void Start()
    {
        bloodImage = GetComponent<Image>();
        bloodImage.enabled = false;
        currentAlpha = 0f;
    }

    public void ShowBloodSplatter()
    {
        if (bloodSprites.Length == 0)
            return;

        // Randomly select a blood splatter sprite
        Sprite randomSprite = bloodSprites[Random.Range(0, bloodSprites.Length)];

        // Set the selected sprite to the blood image
        bloodImage.sprite = randomSprite;

        // Reset alpha value
        currentAlpha = maxAlpha;

        // Enable the blood image and start fading
        bloodImage.enabled = true;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        while (currentAlpha > 0f)
        {
            currentAlpha -= Time.deltaTime / fadeDuration;
            bloodImage.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }

        bloodImage.enabled = false; // Disable the blood image after fading out
    }
}
