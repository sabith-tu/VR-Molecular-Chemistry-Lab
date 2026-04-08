using System;
using UnityEngine;

public class MoleculeGenerator : MonoBehaviour
{
    [Header("Atom Materials")]
    public Material matH; // White
    public Material matO; // Red
    public Material matC; // Black
    public Material matN; // Blue

    private Vector3 currentSpawnOffset = Vector3.zero;

    [ContextMenu("== GENERATE ALL MOLECULES ==")]
    public void GenerateAll()
    {
        float spacing = 4.0f;
        int columns = 5;
        int currentIndex = 0;

        Action[] generators =
        {
            GenerateWater,
            GenerateAmmonia,
            GenerateHydrogenGas,
            GenerateOxygenGas,
            GenerateNitrogenGas,
            GenerateNitricOxide,
            GenerateHydroxylRadical,
            GenerateCarbonMonoxide,
            GenerateCarbonDioxide,
            GenerateHydrogenCyanide,
            GenerateCyanogen,
            GenerateFormaldehyde,
            GenerateMethane,
            GenerateMethanol,
            GenerateDiazene,
            GenerateHydrazine,
            GenerateUrea,
            GenerateGlycine,
        };

        foreach (var generate in generators)
        {
            int row = currentIndex / columns;
            int col = currentIndex % columns;

            currentSpawnOffset = new Vector3(col * spacing, 0, -row * spacing);
            generate.Invoke();
            currentIndex++;
        }

        currentSpawnOffset = Vector3.zero;
        Debug.Log($"Successfully generated {generators.Length} molecules.");
    }

    // --- ORIGINAL MOLECULES ---

    [ContextMenu("Generate Water (H2O)")]
    public void GenerateWater()
    {
        Transform root = CreateRoot("Water_H2O");
        Vector3 center = Vector3.zero;
        SpawnAtom("O", matO, 1.0f, center, root);

        float bondLength = 1.0f;
        float halfAngle = 104.5f / 2f;

        Vector3 h1Pos = Quaternion.Euler(0, 0, halfAngle) * Vector3.down * bondLength;
        Vector3 h2Pos = Quaternion.Euler(0, 0, -halfAngle) * Vector3.down * bondLength;

        SpawnAtom("H", matH, 0.6f, h1Pos, root);
        SpawnAtom("H", matH, 0.6f, h2Pos, root);

        CreateBond(center, 1.0f, h1Pos, 0.6f, matO, matH, root);
        CreateBond(center, 1.0f, h2Pos, 0.6f, matO, matH, root);
    }

    [ContextMenu("Generate Ammonia (NH3)")]
    public void GenerateAmmonia()
    {
        Transform root = CreateRoot("Ammonia_NH3");
        Vector3 center = Vector3.zero;
        SpawnAtom("N", matN, 1.0f, center, root);

        float bondLength = 1.0f;
        float angleFromBottom = 68.16f;

        for (int i = 0; i < 3; i++)
        {
            Quaternion yRotation = Quaternion.Euler(0, i * 120f, 0);
            Vector3 direction =
                yRotation * (Quaternion.Euler(angleFromBottom, 0, 0) * Vector3.down);
            Vector3 hPos = direction * bondLength;

            SpawnAtom("H", matH, 0.6f, hPos, root);
            CreateBond(center, 1.0f, hPos, 0.6f, matN, matH, root);
        }
    }

    [ContextMenu("Generate Gas: Hydrogen (H2)")]
    public void GenerateHydrogenGas() =>
        GenerateHomoDiatomic("Hydrogen_H2", "H", matH, 0.6f, 0.74f, 1);

    [ContextMenu("Generate Gas: Oxygen (O2)")]
    public void GenerateOxygenGas() => GenerateHomoDiatomic("Oxygen_O2", "O", matO, 1.0f, 1.21f, 2);

    [ContextMenu("Generate Gas: Nitrogen (N2)")]
    public void GenerateNitrogenGas() =>
        GenerateHomoDiatomic("Nitrogen_N2", "N", matN, 1.0f, 1.10f, 3);

    // --- DIATOMIC MOLECULES ---

    [ContextMenu("Generate Nitric Oxide (NO)")]
    public void GenerateNitricOxide() =>
        GenerateHeteroDiatomic("NitricOxide_NO", "N", matN, 1.0f, "O", matO, 1.0f, 1.15f, 2);

    [ContextMenu("Generate Hydroxyl Radical (OH)")]
    public void GenerateHydroxylRadical() =>
        GenerateHeteroDiatomic("HydroxylRadical_OH", "O", matO, 1.0f, "H", matH, 0.6f, 0.98f, 1);

    [ContextMenu("Generate Carbon Monoxide (CO)")]
    public void GenerateCarbonMonoxide() =>
        GenerateHeteroDiatomic("CarbonMonoxide_CO", "C", matC, 1.0f, "O", matO, 1.0f, 1.13f, 3);

    // --- LINEAR & PLANAR MOLECULES ---

    [ContextMenu("Generate Carbon Dioxide (CO2)")]
    public void GenerateCarbonDioxide()
    {
        Transform root = CreateRoot("CarbonDioxide_CO2");
        Vector3 cPos = Vector3.zero;
        Vector3 o1Pos = Vector3.left * 1.16f;
        Vector3 o2Pos = Vector3.right * 1.16f;

        SpawnAtom("C", matC, 1.0f, cPos, root);
        SpawnAtom("O", matO, 1.0f, o1Pos, root);
        SpawnAtom("O", matO, 1.0f, o2Pos, root);

        CreateBond(cPos, 1.0f, o1Pos, 1.0f, matC, matO, root, 2);
        CreateBond(cPos, 1.0f, o2Pos, 1.0f, matC, matO, root, 2);
    }

    [ContextMenu("Generate Hydrogen Cyanide (HCN)")]
    public void GenerateHydrogenCyanide()
    {
        Transform root = CreateRoot("HydrogenCyanide_HCN");
        Vector3 cPos = Vector3.zero;
        Vector3 hPos = Vector3.left * 1.06f;
        Vector3 nPos = Vector3.right * 1.15f;

        SpawnAtom("C", matC, 1.0f, cPos, root);
        SpawnAtom("H", matH, 0.6f, hPos, root);
        SpawnAtom("N", matN, 1.0f, nPos, root);

        CreateBond(cPos, 1.0f, hPos, 0.6f, matC, matH, root, 1);
        CreateBond(cPos, 1.0f, nPos, 1.0f, matC, matN, root, 3);
    }

    [ContextMenu("Generate Cyanogen (C2N2)")]
    public void GenerateCyanogen()
    {
        Transform root = CreateRoot("Cyanogen_C2N2");
        Vector3 c1 = Vector3.left * 0.69f;
        Vector3 c2 = Vector3.right * 0.69f;
        Vector3 n1 = Vector3.left * 1.85f;
        Vector3 n2 = Vector3.right * 1.85f;

        SpawnAtom("C", matC, 1.0f, c1, root);
        SpawnAtom("C", matC, 1.0f, c2, root);
        SpawnAtom("N", matN, 1.0f, n1, root);
        SpawnAtom("N", matN, 1.0f, n2, root);

        CreateBond(c1, 1.0f, c2, 1.0f, matC, matC, root, 1);
        CreateBond(c1, 1.0f, n1, 1.0f, matC, matN, root, 3);
        CreateBond(c2, 1.0f, n2, 1.0f, matC, matN, root, 3);
    }

    [ContextMenu("Generate Formaldehyde (CH2O)")]
    public void GenerateFormaldehyde()
    {
        Transform root = CreateRoot("Formaldehyde_CH2O");
        Vector3 cPos = Vector3.zero;
        Vector3 oPos = Vector3.up * 1.2f;
        Vector3 h1Pos = Quaternion.Euler(0, 0, 120) * Vector3.up * 1.1f;
        Vector3 h2Pos = Quaternion.Euler(0, 0, -120) * Vector3.up * 1.1f;

        SpawnAtom("C", matC, 1.0f, cPos, root);
        SpawnAtom("O", matO, 1.0f, oPos, root);
        SpawnAtom("H", matH, 0.6f, h1Pos, root);
        SpawnAtom("H", matH, 0.6f, h2Pos, root);

        CreateBond(cPos, 1.0f, oPos, 1.0f, matC, matO, root, 2);
        CreateBond(cPos, 1.0f, h1Pos, 0.6f, matC, matH, root, 1);
        CreateBond(cPos, 1.0f, h2Pos, 0.6f, matC, matH, root, 1);
    }

    // --- 3D / COMPLEX MOLECULES ---

    [ContextMenu("Generate Methane (CH4)")]
    public void GenerateMethane()
    {
        Transform root = CreateRoot("Methane_CH4");
        Vector3 cPos = Vector3.zero;
        SpawnAtom("C", matC, 1.0f, cPos, root);

        Vector3[] hDirs =
        {
            new Vector3(1, 1, 1).normalized,
            new Vector3(1, -1, -1).normalized,
            new Vector3(-1, 1, -1).normalized,
            new Vector3(-1, -1, 1).normalized,
        };

        float bondLength = 1.09f;
        foreach (Vector3 dir in hDirs)
        {
            Vector3 hPos = dir * bondLength;
            SpawnAtom("H", matH, 0.6f, hPos, root);
            CreateBond(cPos, 1.0f, hPos, 0.6f, matC, matH, root);
        }
    }

    [ContextMenu("Generate Methanol (CH3OH)")]
    public void GenerateMethanol()
    {
        Transform root = CreateRoot("Methanol_CH3OH");
        Vector3 cPos = Vector3.zero;
        SpawnAtom("C", matC, 1.0f, cPos, root);

        Vector3 oDir = new Vector3(1, 1, 1).normalized;
        Vector3[] hDirs =
        {
            new Vector3(1, -1, -1).normalized,
            new Vector3(-1, 1, -1).normalized,
            new Vector3(-1, -1, 1).normalized,
        };

        Vector3 oPos = oDir * 1.42f;
        SpawnAtom("O", matO, 1.0f, oPos, root);
        CreateBond(cPos, 1.0f, oPos, 1.0f, matC, matO, root);

        foreach (Vector3 dir in hDirs)
        {
            Vector3 hPos = dir * 1.09f;
            SpawnAtom("H", matH, 0.6f, hPos, root);
            CreateBond(cPos, 1.0f, hPos, 0.6f, matC, matH, root);
        }

        Vector3 rotAxis = Vector3.Cross(-oDir, Vector3.up).normalized;
        if (rotAxis.sqrMagnitude < 0.1f)
            rotAxis = Vector3.right;

        Vector3 bentDir = Quaternion.AngleAxis(104.5f, rotAxis) * (-oDir);
        Vector3 hOHPos = oPos + (bentDir * 0.96f);
        SpawnAtom("H", matH, 0.6f, hOHPos, root);
        CreateBond(oPos, 1.0f, hOHPos, 0.6f, matO, matH, root);
    }

    [ContextMenu("Generate Diazene (N2H2)")]
    public void GenerateDiazene()
    {
        Transform root = CreateRoot("Diazene_N2H2");
        Vector3 n1 = Vector3.left * 0.62f;
        Vector3 n2 = Vector3.right * 0.62f;

        Vector3 h1 = n1 + (Quaternion.Euler(0, 0, 107) * Vector3.right * 1.0f);
        Vector3 h2 = n2 + (Quaternion.Euler(0, 0, -107) * Vector3.left * 1.0f);

        SpawnAtom("N", matN, 1.0f, n1, root);
        SpawnAtom("N", matN, 1.0f, n2, root);
        SpawnAtom("H", matH, 0.6f, h1, root);
        SpawnAtom("H", matH, 0.6f, h2, root);

        CreateBond(n1, 1.0f, n2, 1.0f, matN, matN, root, 2);
        CreateBond(n1, 1.0f, h1, 0.6f, matN, matH, root, 1);
        CreateBond(n2, 1.0f, h2, 0.6f, matN, matH, root, 1);
    }

    [ContextMenu("Generate Hydrazine (N2H4)")]
    public void GenerateHydrazine()
    {
        Transform root = CreateRoot("Hydrazine_N2H4");
        Vector3 n1 = Vector3.down * 0.72f;
        Vector3 n2 = Vector3.up * 0.72f;

        SpawnAtom("N", matN, 1.0f, n1, root);
        SpawnAtom("N", matN, 1.0f, n2, root);
        CreateBond(n1, 1.0f, n2, 1.0f, matN, matN, root, 1);

        float dropAngle = 180f - 107f;

        for (int i = 0; i < 2; i++)
        {
            Quaternion rot1 = Quaternion.Euler(0, i * 180f + 45f, 0);
            Vector3 hPos1 = n1 + (rot1 * (Quaternion.Euler(dropAngle, 0, 0) * Vector3.down) * 1.0f);
            SpawnAtom("H", matH, 0.6f, hPos1, root);
            CreateBond(n1, 1.0f, hPos1, 0.6f, matN, matH, root);

            Quaternion rot2 = Quaternion.Euler(0, i * 180f - 45f, 0);
            Vector3 hPos2 = n2 + (rot2 * (Quaternion.Euler(-dropAngle, 0, 0) * Vector3.up) * 1.0f);
            SpawnAtom("H", matH, 0.6f, hPos2, root);
            CreateBond(n2, 1.0f, hPos2, 0.6f, matN, matH, root);
        }
    }

    [ContextMenu("Generate Urea (CH4N2O)")]
    public void GenerateUrea()
    {
        Transform root = CreateRoot("Urea_CH4N2O");
        Vector3 cPos = Vector3.zero;
        Vector3 oPos = Vector3.up * 1.26f;

        Vector3 n1Pos = Quaternion.Euler(0, 0, 121f) * Vector3.up * 1.34f;
        Vector3 n2Pos = Quaternion.Euler(0, 0, -121f) * Vector3.up * 1.34f;

        SpawnAtom("C", matC, 1.0f, cPos, root);
        SpawnAtom("O", matO, 1.0f, oPos, root);
        SpawnAtom("N", matN, 1.0f, n1Pos, root);
        SpawnAtom("N", matN, 1.0f, n2Pos, root);

        CreateBond(cPos, 1.0f, oPos, 1.0f, matC, matO, root, 2);
        CreateBond(cPos, 1.0f, n1Pos, 1.0f, matC, matN, root, 1);
        CreateBond(cPos, 1.0f, n2Pos, 1.0f, matC, matN, root, 1);

        float nhBondLength = 1.01f;

        Vector3 n1ToC = (cPos - n1Pos).normalized;
        Vector3 h1Pos = n1Pos + (Quaternion.Euler(0, 0, 120f) * n1ToC * nhBondLength);
        Vector3 h2Pos = n1Pos + (Quaternion.Euler(0, 0, -120f) * n1ToC * nhBondLength);

        SpawnAtom("H", matH, 0.6f, h1Pos, root);
        SpawnAtom("H", matH, 0.6f, h2Pos, root);
        CreateBond(n1Pos, 1.0f, h1Pos, 0.6f, matN, matH, root);
        CreateBond(n1Pos, 1.0f, h2Pos, 0.6f, matN, matH, root);

        Vector3 n2ToC = (cPos - n2Pos).normalized;
        Vector3 h3Pos = n2Pos + (Quaternion.Euler(0, 0, 120f) * n2ToC * nhBondLength);
        Vector3 h4Pos = n2Pos + (Quaternion.Euler(0, 0, -120f) * n2ToC * nhBondLength);

        SpawnAtom("H", matH, 0.6f, h3Pos, root);
        SpawnAtom("H", matH, 0.6f, h4Pos, root);
        CreateBond(n2Pos, 1.0f, h3Pos, 0.6f, matN, matH, root);
        CreateBond(n2Pos, 1.0f, h4Pos, 0.6f, matN, matH, root);
    }

    [ContextMenu("Generate Glycine (C2H5NO2)")]
    public void GenerateGlycine()
    {
        Transform root = CreateRoot("Glycine_C2H5NO2");

        Vector3 cAlpha = Vector3.zero;
        SpawnAtom("C", matC, 1.0f, cAlpha, root);

        Vector3 dirCarboxyl = Vector3.right;
        Vector3 cCarboxyl = cAlpha + dirCarboxyl * 1.52f;

        Vector3 baseDirLeft = Quaternion.Euler(0, 0, 109.5f) * Vector3.right;
        Vector3 dirN = baseDirLeft;
        Vector3 nAmino = cAlpha + dirN * 1.45f;

        Vector3 dirHAlpha1 = Quaternion.AngleAxis(120, Vector3.right) * baseDirLeft;
        Vector3 dirHAlpha2 = Quaternion.AngleAxis(-120, Vector3.right) * baseDirLeft;

        Vector3 hAlpha1 = cAlpha + dirHAlpha1 * 1.09f;
        Vector3 hAlpha2 = cAlpha + dirHAlpha2 * 1.09f;

        SpawnAtom("C", matC, 1.0f, cCarboxyl, root);
        SpawnAtom("N", matN, 1.0f, nAmino, root);
        SpawnAtom("H", matH, 0.6f, hAlpha1, root);
        SpawnAtom("H", matH, 0.6f, hAlpha2, root);

        CreateBond(cAlpha, 1.0f, cCarboxyl, 1.0f, matC, matC, root);
        CreateBond(cAlpha, 1.0f, nAmino, 1.0f, matC, matN, root);
        CreateBond(cAlpha, 1.0f, hAlpha1, 0.6f, matC, matH, root);
        CreateBond(cAlpha, 1.0f, hAlpha2, 0.6f, matC, matH, root);

        Vector3 dirCAlpha = Vector3.left;
        Vector3 dirOCarbonyl = Quaternion.Euler(0, 0, 120) * dirCAlpha;
        Vector3 dirOHydroxyl = Quaternion.Euler(0, 0, -120) * dirCAlpha;

        Vector3 oCarbonyl = cCarboxyl + dirOCarbonyl * 1.20f;
        Vector3 oHydroxyl = cCarboxyl + dirOHydroxyl * 1.30f;

        SpawnAtom("O", matO, 1.0f, oCarbonyl, root);
        SpawnAtom("O", matO, 1.0f, oHydroxyl, root);

        CreateBond(cCarboxyl, 1.0f, oCarbonyl, 1.0f, matC, matO, root, 2);
        CreateBond(cCarboxyl, 1.0f, oHydroxyl, 1.0f, matC, matO, root, 1);

        Vector3 dirHO = Quaternion.Euler(0, 0, 109.5f) * (-dirOHydroxyl);
        Vector3 hHydroxyl = oHydroxyl + dirHO * 0.96f;

        SpawnAtom("H", matH, 0.6f, hHydroxyl, root);
        CreateBond(oHydroxyl, 1.0f, hHydroxyl, 0.6f, matO, matH, root);

        Vector3 tiltAxis = Vector3.Cross(dirN, Vector3.forward).normalized;
        if (tiltAxis.sqrMagnitude < 0.1f)
            tiltAxis = Vector3.up;

        Vector3 nHBase = Quaternion.AngleAxis(73, tiltAxis) * dirN;
        Vector3 dirNH1 = Quaternion.AngleAxis(120, dirN) * nHBase;
        Vector3 dirNH2 = Quaternion.AngleAxis(-120, dirN) * nHBase;

        Vector3 hN1 = nAmino + dirNH1 * 1.01f;
        Vector3 hN2 = nAmino + dirNH2 * 1.01f;

        SpawnAtom("H", matH, 0.6f, hN1, root);
        SpawnAtom("H", matH, 0.6f, hN2, root);

        CreateBond(nAmino, 1.0f, hN1, 0.6f, matN, matH, root);
        CreateBond(nAmino, 1.0f, hN2, 0.6f, matN, matH, root);
    }

    // --- HELPER METHODS ---

    private void GenerateHomoDiatomic(
        string molName,
        string atomName,
        Material mat,
        float scale,
        float distance,
        int bondOrder = 1
    )
    {
        GenerateHeteroDiatomic(
            molName,
            atomName,
            mat,
            scale,
            atomName,
            mat,
            scale,
            distance,
            bondOrder
        );
    }

    private void GenerateHeteroDiatomic(
        string molName,
        string aName,
        Material aMat,
        float aScale,
        string bName,
        Material bMat,
        float bScale,
        float distance,
        int bondOrder = 1
    )
    {
        Transform root = CreateRoot(molName);
        Vector3 posA = Vector3.left * (distance / 2f);
        Vector3 posB = Vector3.right * (distance / 2f);

        SpawnAtom(aName, aMat, aScale, posA, root);
        SpawnAtom(bName, bMat, bScale, posB, root);

        CreateBond(posA, aScale, posB, bScale, aMat, bMat, root, bondOrder);
    }

    private Transform CreateRoot(string name)
    {
        GameObject root = new GameObject(name);
        root.transform.position = transform.position + currentSpawnOffset;
        root.transform.SetParent(transform);
        return root.transform;
    }

    private void SpawnAtom(
        string name,
        Material mat,
        float scale,
        Vector3 localPosition,
        Transform parent
    )
    {
        GameObject atom = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        atom.name = name;
        atom.transform.SetParent(parent);
        atom.transform.localPosition = localPosition;
        atom.transform.localScale = Vector3.one * scale;

        if (mat != null)
            atom.GetComponent<Renderer>().sharedMaterial = mat;
    }

    private void CreateBond(
        Vector3 start,
        float scaleStart,
        Vector3 end,
        float scaleEnd,
        Material matStart,
        Material matEnd,
        Transform parent,
        int bondOrder = 1
    )
    {
        float radiusStart = scaleStart / 2f;
        float radiusEnd = scaleEnd / 2f;
        Vector3 dir = (end - start).normalized;

        // FIXED: Pushing the start and end targets deeper into the spheres mathematically,
        // ensuring the split point perfectly aligns without overlap.
        float overlap = 0.08f;

        Vector3 visibleStart = start + dir * Mathf.Max(0, radiusStart - overlap);
        Vector3 visibleEnd = end - dir * Mathf.Max(0, radiusEnd - overlap);

        Vector3 splitPoint = (visibleStart + visibleEnd) / 2f;

        Vector3 refAxis = Vector3.forward;
        if (Mathf.Abs(Vector3.Dot(dir, refAxis)) > 0.95f)
            refAxis = Vector3.up;
        Vector3 offsetDir = Vector3.Cross(dir, refAxis).normalized;

        CreateHalfBond(visibleStart, splitPoint, matStart, parent, bondOrder, offsetDir);
        CreateHalfBond(splitPoint, visibleEnd, matEnd, parent, bondOrder, offsetDir);
    }

    private void CreateHalfBond(
        Vector3 start,
        Vector3 end,
        Material mat,
        Transform parent,
        int bondOrder,
        Vector3 offsetDir
    )
    {
        float spacing = 0.16f;
        float radius = bondOrder > 1 ? 0.06f : 0.15f;

        for (int i = 0; i < bondOrder; i++)
        {
            Vector3 offset = Vector3.zero;

            if (bondOrder == 2)
            {
                offset = (i == 0) ? (offsetDir * spacing / 2f) : (-offsetDir * spacing / 2f);
            }
            else if (bondOrder == 3)
            {
                if (i == 1)
                    offset = offsetDir * spacing;
                if (i == 2)
                    offset = -offsetDir * spacing;
            }

            GameObject halfBond = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            halfBond.name = $"HalfBond_{i + 1}";
            halfBond.transform.SetParent(parent);

            halfBond.transform.localPosition = ((start + end) / 2f) + offset;
            halfBond.transform.localRotation = Quaternion.FromToRotation(
                Vector3.up,
                (end - start).normalized
            );

            float distance = Vector3.Distance(start, end);

            // FIXED: Reverted back to exact distance mapping to prevent z-fighting in the middle
            halfBond.transform.localScale = new Vector3(radius, distance / 2f, radius);

            if (mat != null)
                halfBond.GetComponent<Renderer>().sharedMaterial = mat;

            DestroyImmediate(halfBond.GetComponent<Collider>());
        }
    }
}
