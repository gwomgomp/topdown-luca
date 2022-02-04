using TMPro;
using UnityEngine;
using System.Linq;

public class HighscoreFormatter : MonoBehaviour
{
    public void loadAndFormatHighscores() {
        HighscoreManager highscoreManager = GameObject.FindObjectOfType<HighscoreManager>();
        float[] highscores = highscoreManager.GetHighscores();

        string formattedHighscores = highscores.Where(highscore => highscore > 0)
            .Zip(Enumerable.Range(1, highscores.Length), (highscore, ranking) => string.Format("{0}: {1:N2}", ranking, highscore))
            .Prepend("<b>Highscores</b>")
            .Aggregate((first, second) => $"{first}\n{second}");

        gameObject.GetComponent<TextMeshProUGUI>().text = formattedHighscores;
    }
}
