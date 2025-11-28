using UnityEditor;

[CustomEditor(typeof(CollisionNotifier))]
public class CollisionNotifierEditor : Editor
{
    SerializedProperty enableTagFilterProp;
    SerializedProperty allowedTagsProp;
    SerializedProperty enableLayerFilterProp;
    SerializedProperty allowedLayersProp;

    SerializedProperty collisionEnterProp;
    SerializedProperty collisionExitProp;
    SerializedProperty collisionStayProp;
    SerializedProperty triggerEnterProp;
    SerializedProperty triggerExitProp;
    SerializedProperty triggerStayProp;

    SerializedProperty parameterCollisionEnterProp;
    SerializedProperty parameterCollisionExitProp;
    SerializedProperty parameterCollisionStayProp;
    SerializedProperty parameterTriggerEnterProp;
    SerializedProperty parameterTriggerExitProp;
    SerializedProperty parameterTriggerStayProp;

    SerializedProperty voidCollisionEnterProp;
    SerializedProperty voidCollisionExitProp;
    SerializedProperty voidCollisionStayProp;
    SerializedProperty voidTriggerEnterProp;
    SerializedProperty voidTriggerExitProp;
    SerializedProperty voidTriggerStayProp;

    SerializedProperty[] collisionEventProps;
    SerializedProperty[] collisionSimpleEventProps;
    SerializedProperty[] triggerEventProps;
    SerializedProperty[] triggerSimpleEventProps;

    void OnEnable()
    {
        enableTagFilterProp = serializedObject.FindProperty("enableTagFilter");
        allowedTagsProp = serializedObject.FindProperty("allowedTags");
        enableLayerFilterProp = serializedObject.FindProperty("enableLayerFilter");
        allowedLayersProp = serializedObject.FindProperty("allowedLayers");

        collisionEnterProp = serializedObject.FindProperty("collisionEnter");
        collisionExitProp = serializedObject.FindProperty("collisionExit");
        collisionStayProp = serializedObject.FindProperty("collisionStay");
        triggerEnterProp = serializedObject.FindProperty("triggerEnter");
        triggerExitProp = serializedObject.FindProperty("triggerExit");
        triggerStayProp = serializedObject.FindProperty("triggerStay");

        parameterCollisionEnterProp = serializedObject.FindProperty("parameterCollisionEnterEvents");
        parameterCollisionExitProp = serializedObject.FindProperty("parameterCollisionExitEvents");
        parameterCollisionStayProp = serializedObject.FindProperty("parameterCollisionStayEvents");
        parameterTriggerEnterProp = serializedObject.FindProperty("parameterTriggerEnterEvents");
        parameterTriggerExitProp = serializedObject.FindProperty("parameterTriggerExitEvents");
        parameterTriggerStayProp = serializedObject.FindProperty("parameterTriggerStayEvents");

        voidCollisionEnterProp = serializedObject.FindProperty("voidCollisionEnterEvents");
        voidCollisionExitProp = serializedObject.FindProperty("voidCollisionExitEvents");
        voidCollisionStayProp = serializedObject.FindProperty("voidCollisionStayEvents");
        voidTriggerEnterProp = serializedObject.FindProperty("voidTriggerEnterEvents");
        voidTriggerExitProp = serializedObject.FindProperty("voidTriggerExitEvents");
        voidTriggerStayProp = serializedObject.FindProperty("voidTriggerStayEvents");

        collisionEventProps = new SerializedProperty[]
        {
            serializedObject.FindProperty("OnCollisionEntered"),
            serializedObject.FindProperty("OnCollisionExited"),
            serializedObject.FindProperty("OnCollisionStayed")
        };

        collisionSimpleEventProps = new SerializedProperty[]
        {
            serializedObject.FindProperty("OnCollisionEnteredSimple"),
            serializedObject.FindProperty("OnCollisionExitedSimple"),
            serializedObject.FindProperty("OnCollisionStayedSimple")
        };

        triggerEventProps = new SerializedProperty[]
        {
            serializedObject.FindProperty("OnTriggerEntered"),
            serializedObject.FindProperty("OnTriggerExited"),
            serializedObject.FindProperty("OnTriggerStayed")
        };

        triggerSimpleEventProps = new SerializedProperty[]
        {
            serializedObject.FindProperty("OnTriggerEnteredSimple"),
            serializedObject.FindProperty("OnTriggerExitedSimple"),
            serializedObject.FindProperty("OnTriggerStayedSimple")
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // --- Collision Events ---
        bool showCollision = EditorPrefs.GetBool("CollisionNotifier_ShowCollision", true);
        showCollision = EditorGUILayout.BeginFoldoutHeaderGroup(showCollision, "Collision Events");
        EditorPrefs.SetBool("CollisionNotifier_ShowCollision", showCollision);

        if (showCollision)
        {
            EditorGUILayout.PropertyField(collisionEnterProp);
            if (collisionEnterProp.boolValue)
            {
                EditorGUILayout.PropertyField(parameterCollisionEnterProp);
                EditorGUILayout.PropertyField(voidCollisionEnterProp);

                if (parameterCollisionEnterProp.boolValue)
                    EditorGUILayout.PropertyField(collisionEventProps[0]);
                if (voidCollisionEnterProp.boolValue)
                    EditorGUILayout.PropertyField(collisionSimpleEventProps[0]);
            }

            EditorGUILayout.PropertyField(collisionExitProp);
            if (collisionExitProp.boolValue)
            {
                EditorGUILayout.PropertyField(parameterCollisionExitProp);
                EditorGUILayout.PropertyField(voidCollisionExitProp);

                if (parameterCollisionExitProp.boolValue)
                    EditorGUILayout.PropertyField(collisionEventProps[1]);
                if (voidCollisionExitProp.boolValue)
                    EditorGUILayout.PropertyField(collisionSimpleEventProps[1]);
            }

            EditorGUILayout.PropertyField(collisionStayProp);
            if (collisionStayProp.boolValue)
            {
                EditorGUILayout.PropertyField(parameterCollisionStayProp);
                EditorGUILayout.PropertyField(voidCollisionStayProp);

                if (parameterCollisionStayProp.boolValue)
                    EditorGUILayout.PropertyField(collisionEventProps[2]);
                if (voidCollisionStayProp.boolValue)
                    EditorGUILayout.PropertyField(collisionSimpleEventProps[2]);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // --- Trigger Events ---
        bool showTrigger = EditorPrefs.GetBool("CollisionNotifier_ShowTrigger", true);
        showTrigger = EditorGUILayout.BeginFoldoutHeaderGroup(showTrigger, "Trigger Events");
        EditorPrefs.SetBool("CollisionNotifier_ShowTrigger", showTrigger);

        if (showTrigger)
        {
            EditorGUILayout.PropertyField(triggerEnterProp);
            if (triggerEnterProp.boolValue)
            {
                EditorGUILayout.PropertyField(parameterTriggerEnterProp);
                EditorGUILayout.PropertyField(voidTriggerEnterProp);

                if (parameterTriggerEnterProp.boolValue)
                    EditorGUILayout.PropertyField(triggerEventProps[0]);
                if (voidTriggerEnterProp.boolValue)
                    EditorGUILayout.PropertyField(triggerSimpleEventProps[0]);
            }

            EditorGUILayout.PropertyField(triggerExitProp);
            if (triggerExitProp.boolValue)
            {
                EditorGUILayout.PropertyField(parameterTriggerExitProp);
                EditorGUILayout.PropertyField(voidTriggerExitProp);

                if (parameterTriggerExitProp.boolValue)
                    EditorGUILayout.PropertyField(triggerEventProps[1]);
                if (voidTriggerExitProp.boolValue)
                    EditorGUILayout.PropertyField(triggerSimpleEventProps[1]);
            }

            EditorGUILayout.PropertyField(triggerStayProp);
            if (triggerStayProp.boolValue)
            {
                EditorGUILayout.PropertyField(parameterTriggerStayProp);
                EditorGUILayout.PropertyField(voidTriggerStayProp);

                if (parameterTriggerStayProp.boolValue)
                    EditorGUILayout.PropertyField(triggerEventProps[2]);
                if (voidTriggerStayProp.boolValue)
                    EditorGUILayout.PropertyField(triggerSimpleEventProps[2]);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // --- Filtering (no foldout) ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Filtering Options", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(enableTagFilterProp);
        if (enableTagFilterProp.boolValue)
            EditorGUILayout.PropertyField(allowedTagsProp, true);

        EditorGUILayout.PropertyField(enableLayerFilterProp);
        if (enableLayerFilterProp.boolValue)
            EditorGUILayout.PropertyField(allowedLayersProp);

        serializedObject.ApplyModifiedProperties();
    }
}