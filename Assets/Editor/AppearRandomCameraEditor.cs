using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Unity3dModelControl
{
    public class AppearRandomCameraEditor : EditorWindow
    {
        private Vector3 targetPosition = Vector3.zero;
        private Transform targetObject = null;
        private float targetRadius = 3f;

        [MenuItem("Tools/AppearRandomCameraEditor")]
        static void ShowSettingWindow()
        {
            EditorWindow.GetWindow(typeof(AppearRandomCameraEditor));
        }

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TargetPosition");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            float newX = EditorGUILayout.FloatField("x:", targetPosition.x);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            float newY = EditorGUILayout.FloatField("y:", targetPosition.y);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            float newZ = EditorGUILayout.FloatField("z:", targetPosition.z);
            EditorGUILayout.EndHorizontal();
            targetPosition = new Vector3(newX, newY, newZ);
            EditorGUILayout.BeginHorizontal();
            Transform updateTargetTransform = EditorGUILayout.ObjectField(
                targetObject,
                typeof(Transform),
                true
            ) as Transform;
            if (updateTargetTransform != targetObject)
            {
                targetPosition = updateTargetTransform.transform.position;
                targetObject = updateTargetTransform;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("targetRadius");
            targetRadius = EditorGUILayout.FloatField(targetRadius);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("GenerateRandomCamera");
            if (GUILayout.Button(new GUIContent("CreateCamera"))){
                AddCamera();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddCamera()
        {
            GameObject newCameraObject = new GameObject("Camera");
            Camera newCamera = newCameraObject.AddComponent<Camera>();
            newCamera.transform.position = RandomCameraPosition(targetPosition);
            newCamera.transform.LookAt(targetPosition);
            Selection.activeGameObject = newCamera.gameObject;
        }

        private Vector3 RandomCameraPosition(Vector3 centerPosition)
        {
            System.Random rand = new System.Random();
            float unitZ = UnityEngine.Random.Range(-1f, 1f);
            float radianT = (float)(Mathf.Deg2Rad * rand.NextDouble() * 360f);
            float x = targetRadius * Mathf.Sqrt(1f - unitZ * unitZ) * Mathf.Cos(radianT);
            float y = targetRadius * Mathf.Sqrt(1f - unitZ * unitZ) * Mathf.Sin(radianT);
            float z = targetRadius * unitZ;
            return centerPosition + new Vector3(x, y, z);
        }

        private Scene currentScene()
        {
            return SceneManager.GetActiveScene();
        }
    }
}