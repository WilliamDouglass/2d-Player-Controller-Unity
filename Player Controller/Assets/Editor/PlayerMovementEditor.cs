using UnityEditor;

[CustomEditor(typeof(PlayerMovementScript))]

public class PlayerMovementEditor : Editor
{
    #region SerializedProperties 
    SerializedProperty rb;
    SerializedProperty objectCollider;
    SerializedProperty groundLayer;
    SerializedProperty groundCheckSize;
    SerializedProperty groundDistance;
    SerializedProperty isGrounded;
    SerializedProperty runAcceloration;
    SerializedProperty runDeceloration;
    SerializedProperty runMaxSpeed;
    SerializedProperty runMinSpeed;
    SerializedProperty horizontalInput;
    SerializedProperty groundGravity;
    SerializedProperty fallingGravity;
    SerializedProperty terminalFall;
    SerializedProperty jumpHeight;
    SerializedProperty jumpAcceloration;
    SerializedProperty canJump;
    SerializedProperty jumpForce;
    SerializedProperty jumpInputPressed;
    SerializedProperty jumpEnded;
    SerializedProperty coyoteTime;
    SerializedProperty coyoteCounter;
    SerializedProperty jumpBufferTime;
    SerializedProperty jumpBufferCoutner;
    SerializedProperty frameVelocity;

    #endregion

    #region Headder Bools
    private bool OnEnableGroup;
    private bool GroundCheckGroup;
    private bool WalkRunGroup;
    private bool GravityGroup;
    private bool JumpGroup;
    private bool CoyoteAndBufferGroup;
    private bool DebugerGroup;
    #endregion
    private void OnEnable()
    {
        rb = serializedObject.FindProperty("rb");
        objectCollider = serializedObject.FindProperty("objectCollider");
        groundLayer = serializedObject.FindProperty("groundLayer");
        groundCheckSize = serializedObject.FindProperty("groundCheckSize");
        groundDistance = serializedObject.FindProperty("groundDistance");
        isGrounded = serializedObject.FindProperty("isGrounded");
        runAcceloration = serializedObject.FindProperty("runAcceloration");
        runDeceloration = serializedObject.FindProperty("runDeceloration");
        runMaxSpeed = serializedObject.FindProperty("runMaxSpeed");
        runMinSpeed = serializedObject.FindProperty("runMinSpeed");
        horizontalInput = serializedObject.FindProperty("horizontalInput");
        groundGravity = serializedObject.FindProperty("groundGravity");
        fallingGravity = serializedObject.FindProperty("fallingGravity");
        terminalFall = serializedObject.FindProperty("terminalFall");
        jumpHeight = serializedObject.FindProperty("jumpHeight");
        jumpAcceloration = serializedObject.FindProperty("jumpAcceloration");
        canJump = serializedObject.FindProperty("canJump");
        jumpForce = serializedObject.FindProperty("jumpForce");
        jumpInputPressed = serializedObject.FindProperty("jumpInputPressed");
        jumpEnded = serializedObject.FindProperty("jumpEnded");
        coyoteTime = serializedObject.FindProperty("coyoteTime");
        coyoteCounter = serializedObject.FindProperty("coyoteCounter");
        jumpBufferTime = serializedObject.FindProperty("jumpBufferTime");
        jumpBufferCoutner = serializedObject.FindProperty("jumpBufferCoutner");
        frameVelocity = serializedObject.FindProperty("frameVelocity");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        OnEnableGroup = EditorGUILayout.BeginFoldoutHeaderGroup(OnEnableGroup, "Game Object");
        if (OnEnableGroup)
        {
            EditorGUILayout.PropertyField(rb);
            EditorGUILayout.PropertyField(objectCollider);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        GroundCheckGroup = EditorGUILayout.BeginFoldoutHeaderGroup(GroundCheckGroup, "Ground Check");
        if (GroundCheckGroup)
        {
            EditorGUILayout.PropertyField(groundLayer);
            EditorGUILayout.PropertyField(groundCheckSize);
            EditorGUILayout.PropertyField(groundDistance);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        WalkRunGroup = EditorGUILayout.BeginFoldoutHeaderGroup(WalkRunGroup, "Walking and Running");
        if (WalkRunGroup)
        {
            EditorGUILayout.PropertyField(runMaxSpeed);
            EditorGUILayout.PropertyField(runMinSpeed);
            EditorGUILayout.PropertyField(runAcceloration);
            EditorGUILayout.PropertyField(runDeceloration);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        GravityGroup = EditorGUILayout.BeginFoldoutHeaderGroup(GravityGroup, "Gravity");
        if (GravityGroup)
        {
            EditorGUILayout.PropertyField(groundGravity);
            EditorGUILayout.PropertyField(fallingGravity);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        JumpGroup = EditorGUILayout.BeginFoldoutHeaderGroup(JumpGroup, "Jumping and Falling");
        if (JumpGroup)
        {
            EditorGUILayout.PropertyField(terminalFall);
            EditorGUILayout.PropertyField(jumpHeight);
            EditorGUILayout.PropertyField(jumpAcceloration);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        CoyoteAndBufferGroup = EditorGUILayout.BeginFoldoutHeaderGroup(CoyoteAndBufferGroup, "Coyote and Buffer");
        if (CoyoteAndBufferGroup)
        {
            EditorGUILayout.PropertyField(coyoteTime);
            EditorGUILayout.PropertyField(jumpBufferTime);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();


        DebugerGroup = EditorGUILayout.BeginFoldoutHeaderGroup(DebugerGroup, "Debug Help");
        if (DebugerGroup)
        {
            EditorGUILayout.PropertyField(isGrounded);
            EditorGUILayout.PropertyField(horizontalInput);
            EditorGUILayout.PropertyField(canJump);
            EditorGUILayout.PropertyField(jumpForce);
            EditorGUILayout.PropertyField(jumpInputPressed);
            EditorGUILayout.PropertyField(jumpEnded);
            EditorGUILayout.PropertyField(coyoteCounter);
            EditorGUILayout.PropertyField(jumpBufferCoutner);
            EditorGUILayout.PropertyField(frameVelocity);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        serializedObject.ApplyModifiedProperties();
    }
}
