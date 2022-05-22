/* Author:          ezhex1991@outlook.com
 * CreateTime:      2019-12-12 10:54:28
 * Organization:    #ORGANIZATION#
 * Description:     
 */
using UnityEditor;
using UnityEngine;
using static EZhex1991.EZSoftBone.EZSoftBone;

namespace EZhex1991.EZSoftBone
{
    [CustomEditor(typeof(EZSoftBone))]
    public class EZSoftBoneInspector : Editor
    {
        private EZSoftBone softBone;

        private SerializedProperty m_RootBones;
        private SerializedProperty m_EndBones;
        private SerializedProperty m_Material;

        private SerializedProperty m_StartDepth;
        private SerializedProperty m_SiblingConstraints;
        private SerializedProperty m_LengthUnification;
        private SerializedProperty m_SiblingRotationConstraints;
        private SerializedProperty m_ClosedSiblings;

        private SerializedProperty m_CollisionLayers;
        private SerializedProperty m_ExtraColliders;
        private SerializedProperty m_Radius;
        private SerializedProperty m_RadiusCurve;

        private SerializedProperty m_DeltaTimeMode;
        private SerializedProperty m_ConstantDeltaTime;
        private SerializedProperty m_Iterations;
        private SerializedProperty m_SleepThreshold;

        private SerializedProperty m_Gravity;
        private SerializedProperty m_GravityAligner;

        private SerializedProperty m_ForceModule;
        private SerializedProperty m_ForceSpace;
        private SerializedProperty m_ForceScale;

        private SerializedProperty m_SimulateSpace;

        private void OnEnable()
        {
            softBone = target as EZSoftBone;

            m_RootBones = serializedObject.FindProperty("m_RootBones");
            m_EndBones = serializedObject.FindProperty("m_EndBones");
            m_Material = serializedObject.FindProperty("m_Material");

            m_StartDepth = serializedObject.FindProperty("m_StartDepth");
            m_SiblingConstraints = serializedObject.FindProperty("m_SiblingConstraints");
            m_LengthUnification = serializedObject.FindProperty("m_LengthUnification");
            m_SiblingRotationConstraints = serializedObject.FindProperty("m_SiblingRotationConstraints");
            m_ClosedSiblings = serializedObject.FindProperty("m_ClosedSiblings");

            m_CollisionLayers = serializedObject.FindProperty("m_CollisionLayers");
            m_ExtraColliders = serializedObject.FindProperty("m_ExtraColliders");
            m_Radius = serializedObject.FindProperty("m_Radius");
            m_RadiusCurve = serializedObject.FindProperty("m_RadiusCurve");

            m_DeltaTimeMode = serializedObject.FindProperty("m_DeltaTimeMode");
            m_ConstantDeltaTime = serializedObject.FindProperty("m_ConstantDeltaTime");
            m_Iterations = serializedObject.FindProperty("m_Iterations");
            m_SleepThreshold = serializedObject.FindProperty("m_SleepThreshold");

            m_Gravity = serializedObject.FindProperty("m_Gravity");
            m_GravityAligner = serializedObject.FindProperty("m_GravityAligner");

            m_ForceModule = serializedObject.FindProperty("m_ForceModule");
            m_ForceSpace = serializedObject.FindProperty("m_ForceSpace");
            m_ForceScale = serializedObject.FindProperty("m_ForceScale");

            m_SimulateSpace = serializedObject.FindProperty("m_SimulateSpace");
        }

        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(target as MonoBehaviour), typeof(MonoScript), false);
            GUI.enabled = true;
            serializedObject.Update();
            bool initRequired = false;

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(m_RootBones, true);
                EditorGUILayout.PropertyField(m_EndBones, true);
            }
            if (EditorGUI.EndChangeCheck())
            {
                initRequired = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_Material);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Structure", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(m_StartDepth);
                EditorGUILayout.PropertyField(m_SiblingConstraints);
                EditorGUILayout.PropertyField(m_LengthUnification);
                EditorGUILayout.PropertyField(m_SiblingRotationConstraints);
                EditorGUILayout.PropertyField(m_ClosedSiblings);
            }
            if (EditorGUI.EndChangeCheck())
            {
                initRequired = true;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Collision", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_CollisionLayers);
            EditorGUILayout.PropertyField(m_ExtraColliders, true);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_Radius);
            EditorGUILayout.PropertyField(m_RadiusCurve);
            if (EditorGUI.EndChangeCheck())
            {
                softBone.RefreshRadius();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Performance", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_DeltaTimeMode);
            if (m_DeltaTimeMode.intValue == (int)DeltaTimeMode.Constant)
            {
                EditorGUILayout.PropertyField(m_ConstantDeltaTime);
            }
            EditorGUILayout.PropertyField(m_Iterations);
            EditorGUILayout.PropertyField(m_SleepThreshold);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Gravity", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_Gravity);
            EditorGUILayout.PropertyField(m_GravityAligner);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Force", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_ForceModule);
            EditorGUILayout.PropertyField(m_ForceSpace);
            EditorGUILayout.PropertyField(m_ForceScale);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_SimulateSpace);
            if (EditorGUI.EndChangeCheck())
            {
                initRequired = true;
            }

            serializedObject.ApplyModifiedProperties();

            if (initRequired)
            {
                if (Application.isPlaying)
                {
                    softBone.RevertTransforms();
                    softBone.InitStructures();
                }
                else
                {
                    softBone.InitStructures();
                }
            }
        }
    }
}
