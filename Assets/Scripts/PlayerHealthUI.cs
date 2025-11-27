using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth2D playerHealth;
    public Image[] lifeIcons;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateUI;
            UpdateUI(playerHealth.CurrentHP, playerHealth.MaxHP);
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateUI;
    }

    void UpdateUI(int current, int max)
    {
        if (lifeIcons == null) return;

        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].enabled = (i < current);
    }
}
