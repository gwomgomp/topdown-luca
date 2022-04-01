using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLayerManager : MonoBehaviour {
    [Serializable]
    public struct MusicLayerDefinition {
        public string name;
        public AudioClip music;
        public bool isDefault;
        public float targetVolume;
    }
    public MusicLayerDefinition[] layers;

    [Serializable]
    public struct FadeDefinition {
        public string name;
        public string syncName;
        public float fadeDuration;
        public float fadeInThreshold;
        public float fadeOutThreshold;
    }
    public FadeDefinition[] fadeDefinitions;

    public float syncTime = 30;
    public float layerUpdateTime = 5;

    private struct MusicLayer {
        public string name;
        public AudioSource source;
        public float targetVolume;

        public MusicLayer(AudioSource source, MusicLayerDefinition layer) {
            name = layer.name;
            this.source = source;
            targetVolume = layer.targetVolume;
        }
    }
    private Dictionary<string, MusicLayer> audioLayers;

    private readonly Dictionary<string, IEnumerator> runningFades = new Dictionary<string, IEnumerator>();

    private float timeSinceSync = 0f;

    private float timeSinceLayerUpdate = 0f;
    private float previousSpeed = 0f;

    public void Start() {
        audioLayers = new Dictionary<string, MusicLayer>();
        foreach (var layerDefinition in layers) {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.clip = layerDefinition.music;
            audioSource.volume = 0;
            audioSource.spatialize = false;
            audioSource.playOnAwake = false;
            MusicLayer layer = new MusicLayer(audioSource, layerDefinition);
            audioLayers.Add(layerDefinition.name, layer);

            if (layerDefinition.isDefault) {
                StartCoroutine(FadeIn(layer, 1));
            }
            SyncPlayingLayers();
        }

        PlayerController.OnSpeedChange += HandleSpeed;
    }

    public void Update() {
        if (timeSinceSync > syncTime) {
            SyncPlayingLayers();
            timeSinceSync = 0;
        } else {
            timeSinceSync += Time.deltaTime;
        }
    }

    private void HandleSpeed(float speed) {
        if (timeSinceLayerUpdate > layerUpdateTime) {
            timeSinceLayerUpdate = 0;
            foreach (var fade in fadeDefinitions) {
                if (CheckThreshold(fade, previousSpeed, speed, out FadeDirection direction)) {
                    SyncLayers(fade.syncName, fade.name);
                    StartFade(fade.name, fade.fadeDuration, direction);
                }
            }
            previousSpeed = speed;
        } else {
            timeSinceLayerUpdate += Time.deltaTime;
        }
    }

    private bool CheckThreshold(FadeDefinition fadeDefinition, float start, float end, out FadeDirection direction) {
        if (start < end && start <= fadeDefinition.fadeInThreshold && fadeDefinition.fadeInThreshold <= end) {
            direction = FadeDirection.IN;
            return true;
        } else if (start > end && start >= fadeDefinition.fadeOutThreshold && fadeDefinition.fadeOutThreshold >= end) {
            direction = FadeDirection.OUT;
            return true;
        } else {
            direction = FadeDirection.NONE;
            return false;
        }
    }

    void SyncPlayingLayers() {
        float timeToSync = -1;
        foreach (var layer in audioLayers.Values) {
            if (!layer.source.isPlaying) {
                continue;
            }

            if (timeToSync < 0) {
                timeToSync = layer.source.time;
            } else {
                layer.source.time = timeToSync;
            }
        }
    }

    void StartFade(string layerName, float duration, FadeDirection direction) {
        if (runningFades.TryGetValue(layerName, out IEnumerator runningFade)) {
            StopCoroutine(runningFade);
            runningFades.Remove(layerName);
        }

        MusicLayer layer = audioLayers[layerName];
        IEnumerator enumerator;
        if (direction == FadeDirection.IN) {
            enumerator = FadeIn(layer, duration);
        } else if (direction == FadeDirection.OUT) {
            enumerator = FadeOut(layer, duration);
        } else {
            return;
        }

        StartCoroutine(enumerator);
        runningFades.Add(layerName, enumerator);
    }

    void SyncLayers(string source, string target) {
        AudioSource targetSource = audioLayers[target].source;
        targetSource.Play();
        targetSource.time = audioLayers[source].source.time;
    }

    IEnumerator FadeIn(MusicLayer layer, float duration) {
        float passedTime = 0f;
        float startingVolume = layer.source.volume;
        layer.source.Play();
        while (passedTime <= duration) {
            float percentage = passedTime / duration;
            layer.source.volume = startingVolume + percentage * layer.targetVolume;
            float time = Time.time;
            yield return new WaitForSeconds(0.1f);
            passedTime += Time.time - time;
        }
        runningFades.Remove(layer.name);
    }

    IEnumerator FadeOut(MusicLayer layer, float duration) {
        float passedTime = 0f;
        float startingVolume = layer.source.volume;
        while (passedTime <= duration) {
            float percentage = 1 - passedTime / duration;
            layer.source.volume = percentage * startingVolume;
            float time = Time.time;
            yield return new WaitForSeconds(0.1f);
            passedTime += Time.time - time;
        }
        layer.source.Stop();
        runningFades.Remove(layer.name);
    }

    enum FadeDirection {
        IN, OUT, NONE
    }
}
