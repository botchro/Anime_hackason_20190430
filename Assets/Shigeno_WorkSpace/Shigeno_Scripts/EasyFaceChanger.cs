using System.Collections;
using System.Collections.Generic;
using DepthFirstScheduler;
using shigeno_EditorUtility;
using UnityEngine;

public class EasyFaceChanger : MonoBehaviour
{
    [CustomLabel("対象となるAnimator")]
    [SerializeField] private Animator targetAnimator;
    [CustomLabel("表情遷移にかかる時間")]
    [SerializeField] private float facialTransitionSeconds;
    [CustomLabel("表情レイヤーの名前")]
    [SerializeField] private string faceLayerName = "FaceLayer";
    [CustomLabel("デフォルトの表情の名前")]
    [SerializeField] private string defaultAnimationClip;
    [CustomLabel("デフォルト表情に戻すキー")]
    [SerializeField] private KeyCode defaultKey = KeyCode.Space;

    
    
    [System.Serializable]
    public class FaceSetting
    {
        public string FaceName;
        public KeyCode ChangeKey;
    }
    
    [SerializeField] private List<FaceSetting> faceSettings = new List<FaceSetting>();

    private int faceLayerIdx = -1;

    private int faceCount = 0;

    private string currentFaceName;

    private bool isChanging = false;


    private void Start()
    {
        if (targetAnimator == null)
        {
            string errorMessage = "エラー。Animatorが指定されてないです。";
            dispose(errorMessage);
            return;
        }
        
        currentFaceName = defaultAnimationClip;
        faceLayerIdx = targetAnimator.GetLayerIndex(faceLayerName);
        if (faceLayerIdx < 0)
        {
            string errorMessage = "エラー。表情レイヤーの名前が間違っています。";
            dispose(errorMessage);
            return;
        }

        faceCount = faceSettings.Count;
        if (faceCount == 0)
        {
            dispose("エラー。FaceSettingsに表情が一つも登録されていないっぽいです。");
            return;
        }
    }

    private void Update()
    {
        if (isChanging)
        {
            return;
        }
        
        if (Input.GetKeyDown(defaultKey))
        {
            if (currentFaceName.Equals(defaultAnimationClip))
            {
                isChanging = false;
                return;
            }

            if (targetAnimator.IsInTransition(faceLayerIdx))
            {
                isChanging = false;
                return;
            }
        
            currentFaceName = defaultAnimationClip;  
            targetAnimator.CrossFadeInFixedTime(defaultAnimationClip, facialTransitionSeconds, faceLayerIdx);                
        }
            
        for (int i = 0; i < faceCount; i++)
        {
            if (Input.GetKeyDown(faceSettings[i].ChangeKey))
            {
                isChanging = true;
                var faceName = faceSettings[i].FaceName;
                StartCoroutine(faceChange(faceName));
            }
        }
    }

    private IEnumerator faceChange(string faceName)
    {
        if (currentFaceName.Equals(faceName))
        {
            isChanging = false;
            yield break;
        }

        if (targetAnimator.IsInTransition(faceLayerIdx))
        {
            isChanging = false;
            yield break;
        }
        
        currentFaceName = faceName;        
        
        targetAnimator.CrossFadeInFixedTime(defaultAnimationClip, facialTransitionSeconds, faceLayerIdx);    
        yield return new WaitForSeconds(facialTransitionSeconds );
        targetAnimator.CrossFadeInFixedTime(currentFaceName,facialTransitionSeconds,faceLayerIdx);

        isChanging = false;
    }
    
    
    

    private void dispose(string message = "エラー")
    {
        Debug.LogError(message);
        enabled = false;
    }
}
