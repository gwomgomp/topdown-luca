using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Create new car option")]
public class CarOption : ScriptableObject
{
    [field: SerializeField]
    public float Acceleration { get; private set; }
    [field: SerializeField]
    public float ReverseAcceleration { get; private set; }
    [field: SerializeField]
    public float BreakingPower { get; private set; }
    [field: SerializeField]
    public float TurnRate { get; private set; }
    [field: SerializeField]
    public float MaxSpeed { get; private set; }
    [field: SerializeField]
    public float BoostDecay { get; private set; }
    [field: SerializeField]
    public float BoostAcceleration { get; private set; }
    [field: SerializeField]
    public float BoostMaxSpeedMultiplier { get; private set; }
    [field: SerializeField]
    public float JumpStrength { get; private set; }
    [field: SerializeField]
    public string TiltAxis { get; private set; }
    [field: SerializeField]
    public GameObject Car { get; private set; }
}
