using UnityEngine;
using System;
using System.Collections;

public class RandomLipSync : MonoBehaviour
{
    [SerializeField]
    private float[] m_BlendshapeWeightList;

    [SerializeField]
    private string m_LipsyncBlendshape;

    [SerializeField]
    private SkinnedMeshRenderer m_TargetFace;

    [SerializeField]
    private float m_BlendshapeScale = 100f;

    [SerializeField]
    private float m_LipsyncSpeedSec = 0.02f;

    private int blendshapeIndex;

    private bool lipSyncing = false;

    private void Start()
    {
        blendshapeIndex = m_TargetFace.sharedMesh.GetBlendShapeIndex(m_LipsyncBlendshape);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StartCoroutine(ChooseRandomLipsync());
        }
        else
        {
            CloseLip();
        }
    }

    private IEnumerator ChooseRandomLipsync()
    {
        if (lipSyncing) yield break;
        lipSyncing = true;
        var rand = UnityEngine.Random.Range(0, m_BlendshapeWeightList.Length);
        var weight = m_BlendshapeWeightList[rand];
        m_TargetFace.SetBlendShapeWeight(blendshapeIndex, weight * m_BlendshapeScale);

        yield return new WaitForSeconds(m_LipsyncSpeedSec);
        lipSyncing = false;
    }

    private void CloseLip()
    {
        m_TargetFace.SetBlendShapeWeight(blendshapeIndex, 0f);
    }
}
