using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreFormatter : MonoBehaviour
{
    [SerializeField]
    private HighscoreManager highscoreManager;

    public void loadAndFormatHighscores() {
        float[] highscores = highscoreManager.GetHighscores();
        string formattedHighscores = "<b>Highscores</b>\n";
        int ranking = 1;
        foreach (float highscore in highscores)
        {
            if (highscore > 0) {
                formattedHighscores += string.Format("{0}: {1:N2}\n", ranking++, highscore);
            }
        }
        gameObject.GetComponent<TextMeshProUGUI>().text = formattedHighscores;
    }
}
