using System.Collections.Generic;
using UnityEngine;

public class Arsenal : MonoBehaviour
{
    [field:SerializeField] public List<EquipmentScriptableObject> AllEquipment { get; private set; } = new();
}
