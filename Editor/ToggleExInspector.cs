using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(ToggleEx), true)]
public class ToggleExInspector : ToggleEditor
{
    private SerializedProperty useBlockBlank;

    private SerializedProperty soundType;

    private SerializedProperty hoverSound;
    private SerializedProperty useHoverSound;

    private SerializedProperty pressedSound;
    private SerializedProperty usePressedSound;

    public SerializedProperty useTransformEffect;
    public SerializedProperty variationTransform;
    public SerializedProperty variationDuration;
    public SerializedProperty variationScale;
    public SerializedProperty variationDistance;

    protected override void OnEnable()
    {
        base.OnEnable();

        useBlockBlank = serializedObject.FindProperty("useBlockBlank");

        soundType = serializedObject.FindProperty("soundEffectType");

        useHoverSound = serializedObject.FindProperty("useHoverSoundClip");
        hoverSound = serializedObject.FindProperty("hoverSoundClip");

        usePressedSound = serializedObject.FindProperty("usePressedSoundClip");
        pressedSound = serializedObject.FindProperty("pressedSoundClip");

        useTransformEffect = serializedObject.FindProperty("useTransformEffect");
        variationTransform = serializedObject.FindProperty("variationTransform");
        variationDuration = serializedObject.FindProperty("variationDuration");
        variationScale = serializedObject.FindProperty("variationScale");
        variationDistance = serializedObject.FindProperty("variationDistance");
    }

    //并且特别注意，如果用这种序列化方式，需要在 OnInspectorGUI 开头和结尾各加一句 serializedObject.Update();  serializedObject.ApplyModifiedProperties();
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space(-18);//空行
        serializedObject.Update();
        EditorGUILayout.PropertyField(useBlockBlank);

        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(soundType);

        if (soundType.enumValueIndex == (int)ButtonExSoundEffectType.通用音效)
        {
            EditorGUILayout.LabelField(" ", "默认播放“AudioManager”设置的音效，为null时不播");
            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(useHoverSound);

            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(usePressedSound);
        }
        if (soundType.enumValueIndex == (int)ButtonExSoundEffectType.单独音效)
        {
            EditorGUILayout.Space(4);
            if (useHoverSound.boolValue)
            {
                GUILayout.BeginHorizontal();
            }
            EditorGUILayout.PropertyField(useHoverSound);
            if (useHoverSound.boolValue)
            {
                EditorGUILayout.LabelField("悬浮音效为null时不播", " ");
                GUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(hoverSound);
            }

            EditorGUILayout.Space(4);
            if (usePressedSound.boolValue)
            {
                GUILayout.BeginHorizontal();
            }
            EditorGUILayout.PropertyField(usePressedSound);
            if (usePressedSound.boolValue)
            {
                EditorGUILayout.LabelField("点击音效为null时不播", " ");
                GUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(pressedSound);
            }
        }

        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(useTransformEffect);
        if (useTransformEffect.boolValue)
        {
            EditorGUILayout.PropertyField(variationTransform);
            EditorGUILayout.PropertyField(variationDuration);
            EditorGUILayout.PropertyField(variationScale);
            EditorGUILayout.PropertyField(variationDistance);
        }

        EditorGUILayout.Space(10);

        serializedObject.ApplyModifiedProperties();
    }
}
