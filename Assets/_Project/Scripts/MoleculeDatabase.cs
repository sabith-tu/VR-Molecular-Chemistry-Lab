using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MoleculeDatabase", menuName = "SO/MoleculeDatabase")]
public class MoleculeDatabase : ScriptableObject
{
    public List<MoleculeSO> validMolecules;
}
