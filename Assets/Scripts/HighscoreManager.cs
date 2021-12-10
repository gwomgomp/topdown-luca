using System;
using System.IO;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{
    private float[] highscores = new float[10];
    private bool initialized = false;


    private void LoadHighscores()
    {
        System.IO.Directory.CreateDirectory(GetHighscoreFolderPath());

        using (StreamReader reader = new StreamReader(File.Open(GetHighscoreFilePath(), FileMode.OpenOrCreate), true)) {
            string line;
            int ranking = 0;
            while (ranking < 10 && (line = reader.ReadLine()) != null)
            {
                highscores[ranking] = (float) Convert.ToDouble(line);
                ranking++;
            }
        }

        initialized = true;
    }

    internal void StoreLap(float lapTime)
    {
        if (!initialized) {
            LoadHighscores();
        }
        for (int ranking = 0; ranking < highscores.Length; ranking++)
        {
            if (highscores[ranking] < lapTime) {
                Array.Copy(highscores, ranking, highscores, ranking + 1, highscores.Length - ranking - 1);
                highscores[ranking] = lapTime;
                break;
            }
        }
        
        using (StreamWriter writer = new StreamWriter(File.Open(GetHighscoreFilePath(), FileMode.OpenOrCreate))) {
            foreach (float highscore in highscores)
            {
                writer.WriteLine(highscore);
            }
        }
    }

    private string GetHighscoreFolderPath() {
        return Application.persistentDataPath + "\\highscores\\";
    }

    private string GetHighscoreFilePath() {
        return GetHighscoreFolderPath() + MenuManager.CurrentMap + "_highscores";
    }

    internal bool HasHighscore()
    {
        if (!initialized) {
            LoadHighscores();
        }
        return highscores[0] > 0;
    }

    internal float GetHighscore()
    {
        if (!initialized) {
            LoadHighscores();
        }
        return highscores[0];
    }
}
