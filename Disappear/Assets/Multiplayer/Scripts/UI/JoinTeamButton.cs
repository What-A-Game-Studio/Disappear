using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


    public class JoinTeamButton : MonoBehaviour
    {
        private TMP_Text textTeam;
        private int maxTeam;
        private Button teamButton;
        private ColorBlock colors;
        private Color baseColor;
        
        
        public void Init(int current, int max)
        {
            textTeam = GetComponentInChildren<TextMeshProUGUI>();
            teamButton = GetComponent<Button>();
            colors = teamButton.colors;
            baseColor = teamButton.colors.normalColor;
            maxTeam = max;
            UpdateTeamCount(current);
        }

        public void UpdateTeamCount(int current)
        {
            textTeam.text = current + " / " + maxTeam;
            if (current >= maxTeam)
            {
                SetButtonState(false, Color.red);
            }
            else
            {
                SetButtonState(true, baseColor);
            }
        }

        private void SetButtonState(bool state, Color buttonColor)
        {
            teamButton.interactable = state;
            colors.normalColor = buttonColor;
        }
    }
