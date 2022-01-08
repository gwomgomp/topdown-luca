using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{
    public int highscoreCount = 10;

    private float[] highscores;
    private bool initialized = false;

    private void LoadHighscores()
    {
        if (highscores == null) {
            highscores = new float[highscoreCount];
        }

        System.IO.Directory.CreateDirectory(GetHighscoreFolderPath());

        using (StreamReader reader = new StreamReader(File.Open(GetHighscoreFilePath(), FileMode.OpenOrCreate), true)) {
            string line;
            int ranking = 0;
            while (ranking < highscoreCount && (line = reader.ReadLine()) != null)
            {
                highscores[ranking] = (float) Convert.ToDouble(line);
                ranking++;
            }
        }

        if (File.Exists(GetGhostFilePath())) {
            LoadGhost();
        }

        initialized = true;
    }

    private void LoadGhost() {
        string ghostJson = File.ReadAllText(GetGhostFilePath());
        GhostRecording ghostRecording = JsonUtility.FromJson<GhostRecording>(ghostJson);
        if (ghostRecording.Positions != null && ghostRecording.Positions.Count > 0 && ghostRecording.Positions.Count == ghostRecording.Rotations.Count) {
            List<(Vector3, Quaternion)> ghostValues = ghostRecording.Positions.Zip(ghostRecording.Rotations, (p, r) => (p, r)).ToList();
            GhostController ghostController = GameObject.FindObjectOfType<GhostController>() as GhostController;
            ghostController.PlayGhost(ghostValues);
        } else {
            Debug.LogError("Could not load ghost data");
        }
    }

    internal void StoreLap(float lapTime)
    {
        if (!initialized) {
            LoadHighscores();
        }
        for (int ranking = 0; ranking < highscores.Length; ranking++)
        {
            if (highscores[ranking] == 0 || highscores[ranking] > lapTime) {
                Array.Copy(highscores, ranking, highscores, ranking + 1, highscores.Length - ranking - 1);
                highscores[ranking] = lapTime;

                if (ranking == 0) {
                    StartCoroutine(SaveGhostRecording());
                }

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

    private IEnumerator SaveGhostRecording()
    {
        GhostController ghostController = GameObject.FindObjectOfType<GhostController>() as GhostController;
        List<(Vector3, Quaternion)> recording = ghostController.GetLastRecording();
        while (recording.Count == 0) {
            yield return new WaitForSeconds(1);
            recording = ghostController.GetLastRecording();
        }
        ghostController.PlayGhost(recording);

        string jsonRecording = JsonUtility.ToJson(new GhostRecording(
            recording.Select(x => x.Item1).ToList(), 
            recording.Select(x => x.Item2).ToList()
        ));
        File.WriteAllText(GetGhostFilePath(), jsonRecording);
    }

    private string GetHighscoreFolderPath() {
        return Application.persistentDataPath + "\\highscores\\";
    }

    private string GetHighscoreFilePath() {
        return GetHighscoreFolderPath() + MenuManager.CurrentMap + "_highscores";
    }

    private string GetGhostFilePath() {
        return GetHighscoreFolderPath() + MenuManager.CurrentMap + "_ghost";
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

    internal float[] GetHighscores()
    {
        if (!initialized) {
            LoadHighscores();
        }
        return highscores;
    }

    private class GhostRecording {
        [field: SerializeField]
        public List<Vector3> Positions { get; private set; }
        [field: SerializeField]
        public List<Quaternion> Rotations { get; private set; }

        public GhostRecording(List<Vector3> positions, List<Quaternion> rotations)
        {
            this.Positions = positions;
            this.Rotations = rotations;
        }
    }
}
