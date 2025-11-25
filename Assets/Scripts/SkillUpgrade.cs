using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static EnumData;

public class SkillUpgrade : MonoBehaviour
{
    public static SkillUpgrade Instance { get; private set; }

    [Header("UI References")]
    public GameObject cardSelectionPanel;
    public List<SkillCardUI> cardSlots;
    public GameObject slotSelectionOverlay;

    [Header("Data References")]
    public List<SkillCardData> availableCards;
    public int cardsToShow = 3;

    private ActiveSkill skillToInstall;
    private Player currentPlayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ShowUpgradeSelection()
    {
        currentPlayer = Player.Instance;
        if (currentPlayer == null) return;

        int playerSouls = currentPlayer.currencies.Get(CurrencyType.SoulPoint);

        List<SkillCardData> currentPool = GetRandomCards(availableCards, cardsToShow);

        for (int i = 0; i < cardsToShow; i++)
        {
            if (i < currentPool.Count)
            {
                cardSlots[i].gameObject.SetActive(true);
                cardSlots[i].SetupCard(currentPool[i], playerSouls);
            }
            else
            {
                cardSlots[i].gameObject.SetActive(false);
            }
        }

        cardSelectionPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private List<SkillCardData> GetRandomCards(List<SkillCardData> pool, int count)
    {
        return pool.OrderBy(x => Random.value).Take(count).ToList();
    }

    public void SelectCard(SkillCardData card)
    {
        if (currentPlayer == null) return;

        bool spendSuccess = currentPlayer.currencies.Spend(CurrencyType.SoulPoint, card.soulCost);

        if (!spendSuccess)
        {
            Debug.Log("Not enough Soul Points!");
            return;
        }

        if (card.skillType == SkillCardData.SkillType.Passive)
        {
            currentPlayer.AddPassiveSkill((PassiveSkill)card.skillAsset);
            FinishSelection();
        }
        else
        {
            EnterSlotSelectionMode((ActiveSkill)card.skillAsset);
        }
    }

    private void EnterSlotSelectionMode(ActiveSkill skill)
    {
        skillToInstall = skill;

        cardSelectionPanel.SetActive(false);

        slotSelectionOverlay.SetActive(true);
        Debug.Log($"Selected Active Skill: {skill.skillName}. Please choose a slot (Q, W, E).");
    }

    public void InstallSkillToSlot(int slotIndex)
    {
        if (skillToInstall == null || currentPlayer == null) return;

        currentPlayer.ReplaceActiveSkill(slotIndex, skillToInstall);

        SkillUIManager.Instance.SetupUI(currentPlayer); 

        slotSelectionOverlay.SetActive(false);
        skillToInstall = null;
        FinishSelection();
    }

    public void FinishSelection()
    {
        cardSelectionPanel.SetActive(false);
        slotSelectionOverlay.SetActive(false);

        Time.timeScale = 1f;

        Debug.Log("Upgrade selection finished. Game Resumed.");
    }
}