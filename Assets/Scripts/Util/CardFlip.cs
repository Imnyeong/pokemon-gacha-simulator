using UnityEngine;
using DG.Tweening;

public class CardFlip : MonoBehaviour
{
    public GameObject cardFront; // ¾Õ¸é
    public GameObject cardBack;  // µÞ¸é

    private bool isFlipped = false;

    public void FlipCard()
    {
        if (isFlipped) return;

        isFlipped = true;
        cardBack.SetActive(false);
        cardFront.SetActive(false);

        transform.DORotate(new Vector3(0, 90, 0), 0.2f)
            .OnComplete(() =>
            {
                cardBack.SetActive(true);
                cardFront.SetActive(true);
                transform.DORotate(new Vector3(0, 360, 0), 0.3f);
                isFlipped = false;
            });
    }
}
