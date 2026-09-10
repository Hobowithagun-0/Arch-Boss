using System;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UIElements;

public class BossHUD : MonoBehaviour {

    [SerializeField] private GameObject target; // only for unity editor to use
    [SerializeField] private Health[] players;
    private Health targetHealth;
    private BossBehaviour targetScript;
    private ProgressBar bossBar;
    private readonly VisualElement[] playerBar = new VisualElement[3];
    [SerializeField] private readonly Health[] playerHealth = new Health[3];
    private Action<Health>[] playerBarUpdaters;
    private VisualElement attackButton;
    private VisualElement specialButton;

    private void Start() {
        playerBarUpdaters = new Action<Health>[] {
            UpdatePlayerBar1, UpdatePlayerBar2, UpdatePlayerBar3
        };
        var panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void Update() {
        specialButton.style.height = Mathf.RoundToInt(specialButton.resolvedStyle.width * 
            Mathf.Max(1 - targetScript.SpecialDelta / targetScript.SpecialCooldown, 0f));
        attackButton.style.height = Mathf.RoundToInt(attackButton.resolvedStyle.width *
            Mathf.Max(1 - targetScript.AttackDelta / targetScript.AttackCooldown, 0f));
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement root, int version) {
        bossBar = root.Q<ProgressBar>("BossHealth");
        attackButton = root.Q<Image>("Button1").Q("ButtonOverlay");
        specialButton = root.Q<Image>("Button2").Q("ButtonOverlay");
        var playerHealthBars = root.Q<VisualElement>("PlayerHealth");
        for (int i = 0; i < playerBar.Length; i++) {
            playerBar[i] = playerHealthBars.Q($"PlayerHealth{i + 1}");
        }
        Debug.Log("UI RELOADED");
    }

    private void UpdateBossBar(Health hp) {
        UpdateHealthBar(bossBar, hp);
    }


    // Surely there is a better way to do this instead of coding 3 different functions
    private void UpdatePlayerBar1(Health hp) {
        UpdateHealthBar(playerBar[0].Q<ProgressBar>("HealthBar"), hp);
    }
    private void UpdatePlayerBar2(Health hp) {
        UpdateHealthBar(playerBar[1].Q<ProgressBar>("HealthBar"), hp);
    }
    private void UpdatePlayerBar3(Health hp) {
        UpdateHealthBar(playerBar[2].Q<ProgressBar>("HealthBar"), hp);
    }

    private void UpdateHealthBar(ProgressBar healthBar, Health hp) {
        healthBar.title = $"Health: {hp.Value}";
        healthBar.highValue = hp.MaxHealth;
        healthBar.value = hp.Value;
    }

    [ContextMenu("Update Target/Players")]
    private void UpdateTarget() {
        ChangeTargetBoss(target);
        ChangeTargetPlayers(players);
    }
    /// <summary> Changes the BossHUD to track this boss's health </summary>
    public void ChangeTargetBoss(GameObject newTarget) {
        if (targetHealth) { // already has a target assigned => unsubscribe from previous target
            targetHealth.OnHealthChanged -= UpdateBossBar;
        }
        targetHealth = newTarget.GetComponent<Health>();
        targetScript = newTarget.GetComponent<BossBehaviour>();
        targetHealth.OnHealthChanged -= UpdateBossBar; // this line is only needed when reload on playmode is off i think
        targetHealth.OnHealthChanged += UpdateBossBar;
        targetHealth.Heal(0);
    }

    /// <summary> Changes the BossHUD to show these player's healths </summary>
    public void ChangeTargetPlayers(Health[] newPlayers) {
        // unsubscribe from previous targets
        for (int i = 0; i < playerHealth.Length; i++) {
            if (playerHealth[i]) {
                playerHealth[i].OnHealthChanged -= playerBarUpdaters[i];
                playerHealth[i] = null;
            }  
        }
        int playerNo = 0;
        foreach (Health player in newPlayers) {
            if (player) { // subscribe up to 3 players from given list
                playerHealth[playerNo] = player;
                player.OnHealthChanged -= playerBarUpdaters[playerNo];
                player.OnHealthChanged += playerBarUpdaters[playerNo];
                player.Heal(0);
                if (++playerNo >= 3) {
                    break;
                }
            }
        }
        // only display bars that are tracking players
        foreach (var bar in playerBar) {
            if (playerNo-- > 0) {
                bar.style.visibility = Visibility.Visible;
            } else { 
                bar.style.visibility = Visibility.Hidden;
            }
        }
    }
}
