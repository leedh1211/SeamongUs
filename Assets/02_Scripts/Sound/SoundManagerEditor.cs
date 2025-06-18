#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundManager))]
public class SoundManagerEditor : Editor
{
    private SerializedProperty _sfxClipsProp;
    private SerializedProperty _bgmClipsProp;

    private void OnEnable()
    {
        _sfxClipsProp = serializedObject.FindProperty("sfxClips");
        _bgmClipsProp = serializedObject.FindProperty("bgmClips");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 효과음 목록도 Element가 아니라 enum 이름으로 보이게 하기
        EditorGUILayout.LabelField("SFX Clips", EditorStyles.boldLabel);
        var sfxNames = System.Enum.GetNames(typeof(SFXType));
        EnsureArraySize(_sfxClipsProp, sfxNames.Length);
        for (int i = 0; i < sfxNames.Length; i++)
        {
            var element = _sfxClipsProp.GetArrayElementAtIndex(i);
            element.objectReferenceValue = (AudioClip)EditorGUILayout.ObjectField(
                sfxNames[i],
                element.objectReferenceValue,
                typeof(AudioClip),
                false
            );
        }

        EditorGUILayout.Space();

        // BGM 목록도 enum 이름으로 보이게
        EditorGUILayout.LabelField("BGM Clips", EditorStyles.boldLabel);
        var bgmNames = System.Enum.GetNames(typeof(BGMType));
        EnsureArraySize(_bgmClipsProp, bgmNames.Length);
        for (int i = 0; i < bgmNames.Length; i++)
        {
            var element = _bgmClipsProp.GetArrayElementAtIndex(i);
            element.objectReferenceValue = (AudioClip)EditorGUILayout.ObjectField(
                bgmNames[i],
                element.objectReferenceValue,
                typeof(AudioClip),
                false
            );
        }

        serializedObject.ApplyModifiedProperties();
    }

    // 배열을 enum 수에 맞춰 자동으로 조정
    private static void EnsureArraySize(SerializedProperty prop, int size)
    {
        if (prop.arraySize != size)
            prop.arraySize = size;
    }
}
#endif