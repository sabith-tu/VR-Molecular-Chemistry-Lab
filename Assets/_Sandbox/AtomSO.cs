using UnityEngine;

[CreateAssetMenu(fileName = "NewAtom", menuName = "Chemistry/AtomSO")]
public class AtomSO : ScriptableObject
{
    public ElementType elementType;
    public string symbol;
    public GameObject atomPrefab; // The 3D model for this specific atom
}
