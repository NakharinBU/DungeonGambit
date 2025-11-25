using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    public KeyCode hotkey;
    public Image skillIcon;
    public TextMeshProUGUI manaCostText;
    public ActiveSkill assignedSkill;
    public Button skillButton;

    private readonly Color clearColor = new Color(1, 1, 1, 0);

    private readonly Color fullColor = new Color(1, 1, 1, 1);

    private readonly Color insufficientManaColor = Color.gray;

    public void Setup(ActiveSkill skill)
    {
        assignedSkill = skill;
        if (assignedSkill != null)
        {
            skillIcon.sprite = assignedSkill.icon;
            manaCostText.text = assignedSkill.mpCost.ToString();
            skillIcon.color = fullColor;
            manaCostText.color = fullColor;
        }
        else
        {
            skillIcon.sprite = null;
            skillIcon.color = clearColor;
            manaCostText.text = "";
            manaCostText.color = clearColor;
        }
    }

    public void HandleInput(Player player)
    {
        if (Input.GetKeyDown(hotkey) && assignedSkill != null)
        {

            if (player.stats.mp >= assignedSkill.mpCost) 
            {
            player.EnterTargetingMode(assignedSkill);
            }
            else 
            { 
                Debug.Log("Insufficient Mana!"); 
            }
        }
    }

    public void UpdateState(Player player)
    {
        if (assignedSkill != null)
        {
            if (assignedSkill != null && player != null && player.stats != null)
        {
                if (player.stats.mp < assignedSkill.mpCost)
                {
                    skillIcon.color = new Color(insufficientManaColor.r, insufficientManaColor.g, insufficientManaColor.b, 0.5f);
                    manaCostText.color = insufficientManaColor;
                }
                else
                {
                    skillIcon.color = fullColor;
                    manaCostText.color = fullColor;
                }
            }
        }
    }
}
