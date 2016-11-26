using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CharacterCustomization : Singleton<CharacterCustomization> {

    public SkinnedMeshRenderer target;
    public string suffixMax = "Max", suffixMin = "Min";

    private CharacterCustomization() { }

    private SkinnedMeshRenderer skmr;
    private Mesh mesh;

    private Dictionary<string, Blendshape> blendShapeDatabase = new Dictionary<string, Blendshape>();

    private void Start()
    {
        Initialize();
    }


    #region Public Functions

    public void ChangeBlendshapeValue(string blendshapeName, float value)
    {
        if (!blendShapeDatabase.ContainsKey(blendshapeName)) { Debug.LogError("Blendshape " + blendshapeName + " does not exist!"); return; }

        value = Mathf.Clamp(value, -100, 100);

        Blendshape blendshape = blendShapeDatabase[blendshapeName];

        if (value >= 0)
        {
            if (blendshape.positiveIndex == -1) return;
            skmr.SetBlendShapeWeight(blendshape.positiveIndex, value);
            if (blendshape.negativeIndex == -1) return;
            skmr.SetBlendShapeWeight(blendshape.negativeIndex, 0);
        }

        else
        {
            if (blendshape.negativeIndex == -1) return;
            skmr.SetBlendShapeWeight(blendshape.negativeIndex, -value);
            if (blendshape.positiveIndex == -1) return;
            skmr.SetBlendShapeWeight(blendshape.positiveIndex, 0);
        }

    }

    #endregion

    #region Private Functions

    private void Initialize()
    {
        skmr = target;
        mesh = skmr.sharedMesh;

        ParseBlendShapesToDictionary();
    }

    private SkinnedMeshRenderer GetSkinnedMeshRenderer()
    {
        SkinnedMeshRenderer _skmr = target.GetComponentInChildren<SkinnedMeshRenderer>();

        return _skmr;
    }

    private void ParseBlendShapesToDictionary()
    {
        List<string> blendshapeNames = Enumerable.Range(0, mesh.blendShapeCount).Select(x => mesh.GetBlendShapeName(x)).ToList();

        for (int i = 0; blendshapeNames.Count > 0;)
        {
            string altSuffix, noSuffix;
            noSuffix = blendshapeNames[i].TrimEnd(suffixMax.ToCharArray()).TrimEnd(suffixMin.ToCharArray()).Trim();

            string positiveName = string.Empty, negativeName = string.Empty;
            bool exists = false;

            int postiveIndex = -1, negativeIndex = -1;

            //If Suffix is Postive
            if (blendshapeNames[i].EndsWith(suffixMax))
            {
                altSuffix = noSuffix + " " + suffixMin;

                positiveName = blendshapeNames[i];
                negativeName = altSuffix;

                if (blendshapeNames.Contains(altSuffix)) exists = true;

                postiveIndex = mesh.GetBlendShapeIndex(positiveName);

                if (exists)
                    negativeIndex = mesh.GetBlendShapeIndex(altSuffix);
            }

            //If Suffix is Negative
            else if (blendshapeNames[i].EndsWith(suffixMin))
            {
                altSuffix = noSuffix + " " + suffixMax;

                negativeName = blendshapeNames[i];
                positiveName = altSuffix;

                if (blendshapeNames.Contains(altSuffix)) exists = true;

                negativeIndex = mesh.GetBlendShapeIndex(negativeName);

                if (exists)
                    postiveIndex = mesh.GetBlendShapeIndex(altSuffix);
            }
            else
                postiveIndex = mesh.GetBlendShapeIndex(blendshapeNames[i]);

            blendShapeDatabase.Add(noSuffix, new Blendshape(postiveIndex, negativeIndex));

            //Remove Selected Indexes From the List
            if (positiveName != string.Empty) blendshapeNames.Remove(positiveName);
            if (negativeName != string.Empty) blendshapeNames.Remove(negativeName);

        }//End of Loop
    }//End of Class

    #endregion


}
