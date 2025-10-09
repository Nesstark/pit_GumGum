using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Player player;
    public UnityEngine.UI.Slider healthSlider;
    public Text healthText; // health as percentage

    private void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();

        if (player == null) {
            Debug.LogError("UIManager: No Player found in scene and none assigned.", this);
            enabled = false;
            return;
        }

        if (healthSlider != null) {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
            healthSlider.interactable = false;
        }

        UpdateHealthUI(player.Health, player.MaxHealth);

        player.OnHealthChanged += OnPlayerHealthChanged;
        player.OnDied += OnDied;
    }

    private void OnDestroy()
    {
        if (player != null)
            player.OnHealthChanged -= OnPlayerHealthChanged;
    }
    private void OnPlayerHealthChanged(int newHealth)
    {
        UpdateHealthUI(newHealth, player.MaxHealth);
    }

    private void UpdateHealthUI(int health, int maxHealth)
    {
        float pct = (maxHealth > 0) ? (float)health / maxHealth : 0f;
        pct = Mathf.Clamp01(pct);

        if (healthSlider != null)
            healthSlider.value = pct;

        if (healthText != null)
            healthText.text = $"{health} / {maxHealth} ({Mathf.RoundToInt(pct * 100)}%)";
    }

    private void OnDied() => this.gameObject.SetActive(false); // disable UI on death
}