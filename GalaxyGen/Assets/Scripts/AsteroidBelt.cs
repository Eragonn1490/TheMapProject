using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Possible additions
/// - Object pooling for optimization for thousands of asteroids
/// - Adding opening randomization
/// - Complete runtime belt randomization
/// </summary>
public class AsteroidBelt : MonoBehaviour
{
    [Header("Belt Modifiers")]
    public bool GenerateOnStart;
    public List<GameObject> AsteroidPrefabs;
    [Tooltip("Specify where openings will start")]
    [Range(0f, 360f)] public float StartAngle;
    [Tooltip("Randomizes the starting angle of openings. Overrides StartAngle")]
    public bool IsRandomStartAngle;
    [Tooltip("How many openings will appear in the belt")]
    public int BeltOpenings;
    [Tooltip("How big the spacing is for each opening in percentages")]
    [Range(0f, 100f)] public float BeltOpeningSpacing;
    [Tooltip("How many asteroids will spawn in the entire belt")]
    public int AsteroidCount;
    [Tooltip("Determines if the spawning should be random or an instance of previous belt. Make this -1 to make this always random")]
    public int Seed = -1;
    [Tooltip("The distance between the center before asteroids start to spawn")]
    public float Radius;
    [Tooltip("The distance between the radius and the furthest extent")]
    public float Thickness;
    [Tooltip("Should the rotation be to the right or left")]
    public bool IsClockwiseOrbit;
    [Tooltip("Min orbit speed for all asteroids. Leave the same as Max to have uniformed orbiting")]
    public float MinOrbitSpeed;
    [Tooltip("Max orbit speed for all asteroids. Leave the same as Min to have uniformed orbiting")]
    public float MaxOrbitSpeed;
    [Tooltip("Min rotation speed for all asteroids. Leave the same as Max to have uniformed rotation")]
    public float MinRotationSpeed;
    [Tooltip("Max rotation speed for all asteroids. Leave the same as Min to have uniformed rotation")]
    public float MaxRotationSpeed;
    
    private void Start()
    {
        if(GenerateOnStart)
            Generate();
    }

    public void Generate()
    {
        if (Seed != -1)
            Random.InitState(Seed);

        BeltOpenings = Mathf.Max(1, BeltOpenings);
        BeltOpeningSpacing = Mathf.Clamp(BeltOpeningSpacing, 0f, 100f);

        var startAngle = 2 * Mathf.PI / 360 * (IsRandomStartAngle ? Random.Range(0f, 360f) : StartAngle);
        var segment = 2 * Mathf.PI / BeltOpenings;
        var spacing = 2 * Mathf.PI * BeltOpeningSpacing / 100f;
        var asteroidsPerSegment = AsteroidCount / BeltOpenings;

        for (var i = 0; i < BeltOpenings; i++)
        {
            var startRadian = (segment * i) + spacing + startAngle;
            var nextRadian = segment * (i + 1) - spacing + startAngle;

            for (var j = 0; j < asteroidsPerSegment; j++)
            {
                var _randomRadius = Random.Range(Radius, Radius + Thickness);
                var _randomRadian = Random.Range(startRadian, nextRadian);
                var _localPos = new Vector3(_randomRadius * Mathf.Cos(_randomRadian), _randomRadius * Mathf.Sin(_randomRadian), 0f);
                var _worldOffset = transform.rotation * _localPos;
                var _worldPos = transform.position + _worldOffset;

                var randomOrbitSpeed = Random.Range(MinOrbitSpeed, MaxOrbitSpeed);
                var randomRotationSpeed = Random.Range(MinRotationSpeed, MaxRotationSpeed);

                var newAsteroidObject = Instantiate(AsteroidPrefabs[Random.Range(0, AsteroidPrefabs.Count)], _worldPos, Quaternion.identity, transform);
                var newAsteroid = newAsteroidObject.AddComponent<Asteroid>();
                newAsteroid.Init(randomOrbitSpeed, randomRotationSpeed, transform, IsClockwiseOrbit);
            }
        }
    }
}