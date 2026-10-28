
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingGraphUI : MaskableGraphic {
    [Header("Data Source")]
    [SerializeField] private AIIntent aiIntent;

    [Header("Graph Settings")]
    [SerializeField] private float windowSeconds = 15f;
    [SerializeField] private float sampleInterval = 0.1f;
    [SerializeField] private float maxValue = 3f;
    [SerializeField] private float lineThickness = 2f;

    private struct Sample {
        public float time;
        public float value;

        public Sample(float time, float value) {
            this.time = time;
            this.value = value;
        }
    }

    private readonly List<Sample> samples = new();
    private float lastSampleTime;

    protected override void Start() {
        base.Start();

        // Fill the initial window with a flat line.
        float now = Time.unscaledTime;
        float interval = Mathf.Max(0.02f, sampleInterval);
        float duration = Mathf.Max(interval, windowSeconds);

        for (float t = now - duration; t < now; t += interval) {
            samples.Add(new Sample(t, 0f));
        }

        samples.Add(new Sample(now, ReadValue()));
        lastSampleTime = now;
        SetVerticesDirty();
    }

    private void Update() {
        float now = Time.unscaledTime;
        float interval = Mathf.Max(0.02f, sampleInterval);

        if (now - lastSampleTime >= interval) {
            samples.Add(new Sample(now, ReadValue()));
            lastSampleTime = now;
        }

        // Keep only the visible time window.
        while (samples.Count > 2 &&
               samples[1].time < now - windowSeconds) {
            samples.RemoveAt(0);
        }

        // Request a redraw every frame for smooth scrolling.
        SetVerticesDirty();
    }

    private float ReadValue() {
        if (aiIntent == null || aiIntent.Tracker == null)
            return 0f;

        if (aiIntent.Tracker.Status != ActionStatus.Running)
            return 0f;

        return aiIntent.Tracker.TimeWithoutProgress;
    }

    protected override void OnPopulateMesh(VertexHelper vh) {
        vh.Clear();

        if (samples.Count == 0)
            return;

        Rect rect = rectTransform.rect;
        float now = Time.unscaledTime;

        Vector2 previous = ToGraphPosition(samples[0], rect, now);

        for (int i = 1; i < samples.Count; i++) {
            Vector2 current = ToGraphPosition(samples[i], rect, now);
            AddLine(vh, previous, current);
            previous = current;
        }

        // Extend the latest value to the right edge.
        Vector2 livePoint = ToGraphPosition(
            new Sample(now, ReadValue()), rect, now);

        AddLine(vh, previous, livePoint);
    }

    private Vector2 ToGraphPosition(Sample sample, Rect rect, float now) {
        float duration = Mathf.Max(0.1f, windowSeconds);

        float normalizedX = Mathf.Clamp01(
            (sample.time - (now - duration)) / duration
        );

        float normalizedY = Mathf.Clamp01(
            sample.value / Mathf.Max(0.01f, maxValue)
        );

        float x = Mathf.Lerp(rect.xMin, rect.xMax, normalizedX);
        float y = Mathf.Lerp(rect.yMin + 5f, rect.yMax - 5f, normalizedY);

        return new Vector2(x, y);
    }

    private void AddLine(VertexHelper vh, Vector2 a, Vector2 b) {
        Vector2 direction = b - a;

        if (direction.sqrMagnitude < 0.000001f)
            return;

        Vector2 normal = new Vector2(
            -direction.y, direction.x
        ).normalized * (lineThickness / 2f);

        int index = vh.currentVertCount;

        AddVertex(vh, a + normal);
        AddVertex(vh, a - normal);
        AddVertex(vh, b - normal);
        AddVertex(vh, b + normal);

        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index, index + 2, index + 3);
    }

    private void AddVertex(VertexHelper vh, Vector2 position) {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;
        vertex.position = new Vector3(position.x, position.y, 0f);

        vh.AddVert(vertex);
    }
}
