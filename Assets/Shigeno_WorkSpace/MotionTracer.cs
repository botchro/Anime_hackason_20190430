/**
[MotionTracer]
Copyright (c) https://twitter.com/izm
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IzmToyBox.MotionTracer
{

    /// <summary>
    /// Humanoid同士の間でMuscle値のコピーによって動きをコピーするスクリプト
    /// 例えばIKinemaOrionのリグから、他のモデルに動きをコピーさせたり
    /// Neuronのロボットのアニメーションをユニティちゃんに動きをコピーさせたりする
    /// ScriptExecutionOrderは普段気にしなくて良いはず（コピー元のモーキャプデータはUpdateで更新されるので、このスクリプトのLateUpdateでコピーしている）
    /// 強いて言うなら揺れモノ系のスクリプトよりは実行順を前にして下さい
    /// </summary>
    public class MotionTracer : MonoBehaviour
    {
        public enum CopyMode
        {
            All, WithoutFinger
        }

        [Header("モード、srcがIKinema OrionだったらWithoutFingerがお勧めです。NeuronならAll")]
        [SerializeField] [Tooltip("モーキャプがOrionだったらWithouFingerおすすめ")] private CopyMode _copyMode = CopyMode.All;
        [Header("モーションコピー元:IKinema MaleやNeuron Robotを指定")]
        [SerializeField] [Tooltip("IKinema MaleやNeuron Robotを指定してください")] private Animator _src;
        [Header("モーションコピー先:対象キャラを指定")]
        [SerializeField] [Tooltip("モーションコピー先のキャラクターを指定してください")] private Animator _dst;
        HumanPoseHandler _poseHandlerSrc = null;
        HumanPoseHandler _poseHandlerDst = null;

        private HumanPose _currentPose = new HumanPose();

        [SerializeField] bool _initialized = false;

        // Use this for initialization
        IEnumerator Start()
        {
            if (_src == null || _dst == null)
            {
                Debug.LogError("MotionTracer animator null");
                Debug.LogError("[ERROR] MotionTracer.cs　のコピー元かコピー先のキャラクター指定がされていません。エディタ再生を停止して確認して下さい");

                Destroy(this);
            }

            try
            {
                _poseHandlerSrc = new HumanPoseHandler(_src.avatar, _src.transform);
            }
            catch (Exception e)
            {
                Debug.LogError("[ERROR] MotionTracer.cs　のsrc(コピー元)のキャラクターがHumanoidじゃないみたいです。確認して下さい");
            }

            yield return null;

            try
            {
                _poseHandlerDst = new HumanPoseHandler(_dst.avatar, _dst.transform);
            }
            catch (Exception e)
            {
                Debug.LogError("[ERROR] MotionTracer.cs　のdsc(コピー先)のキャラクターがHumanoidじゃないみたいです。確認して下さい");
            }

            if (_poseHandlerSrc != null && _poseHandlerDst != null)
            {
                _initialized = true;
            }
            else
            {
                Debug.LogError("[ERROR] MotionTracer.csの初期化に失敗しています。たぶんキャラのモーションがＴポーズのですよね。上のエラーを確認して下さい");
            }


            if (_src.applyRootMotion == false || _dst.applyRootMotion == false)
            {
                Debug.LogError("[WARNING] AnimatorのApplyRootMotionがfalseになってますが、足が滑りませんか。大丈夫ですか？");
            }

            if (_src.cullingMode != AnimatorCullingMode.AlwaysAnimate)
            {
                Debug.Log("[INFO] コピー元のNeuronロボットとかIKinemaMaleがAnimatorのcullingModeがAlwaysAnimateじゃなかったので直しておきます");
                _src.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!_initialized)
            {
                return;
            }

            _poseHandlerSrc.GetHumanPose(ref _currentPose);

            //指以外をコピーする
            //逆に言うと、現在のコピー先のposeを取得して、指部分のmuscleをcurrentPoseのmuscleに移植しておく
            if (_copyMode == CopyMode.WithoutFinger)
            {
                /*
                for (int i = 0; i < HumanTrait.MuscleCount; i++)
                {
                    Debug.Log(i + " name:" + HumanTrait.MuscleName[i]);
                }
                Debug.Log(_currentPose.muscles.Length);
                */

                var dstPose = new HumanPose();
                _poseHandlerDst.GetHumanPose(ref dstPose);
                int leftFingerStart = 55;

                int rightFingerEnd = 94;
                for (int i = leftFingerStart; i <= rightFingerEnd; i++)
                {
                    _currentPose.muscles[i] = dstPose.muscles[i];
                }

            }

            _poseHandlerDst.SetHumanPose(ref _currentPose);
        }
    }
}