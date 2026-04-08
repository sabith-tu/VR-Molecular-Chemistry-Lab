using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMolecule", menuName = "Chemistry/MoleculeSO")]
public class MoleculeSO : ScriptableObject
{
    public string moleculeName;
    public string formula;
    public BondType bondType;

    public List<AtomSO> requiredAtoms; // Now uses the data objects directly
    public GameObject moleculePrefab;
}
