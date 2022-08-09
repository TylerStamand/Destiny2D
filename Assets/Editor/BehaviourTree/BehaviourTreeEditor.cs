using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

public class BehaviourTreeEditor : EditorWindow
{

    BehaviourTreeView treeView;
    InspectorView inspectorView;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("BehaviourTreeEditor/Editor ...")]
    public static void OpenWindow()
    {
        BehaviourTreeEditor wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    //For when you double click a behaviour tree in files, it opens the editor automatically
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line) {
        if(Selection.activeObject is BehaviourTree) {
            OpenWindow();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        m_VisualTreeAsset.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTree/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviourTreeView>();
        treeView.OnNodeSelected = OnNodeSelectionChanged;
        inspectorView = root.Q<InspectorView>();

        OnSelectionChange();
    }

    void OnEnable() {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    void OnDisable() {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    void OnPlayModeStateChanged(PlayModeStateChange obj) {
        switch(obj) {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
        }
    }

    void OnSelectionChange() {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;

        if(!tree) {
            if(Selection.activeGameObject) {
                BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if(runner) {
                    tree = runner.tree;
                } 
            }
        }

        //Weird fix for cloning or something idk
        if(Application.isPlaying) {
            if(tree) {
                treeView.PopulateView(tree);
            }
        }
        else {
            if(tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())) {
                treeView.PopulateView(tree);
            }

        }
    }

    void OnNodeSelectionChanged(NodeView nodeView) {
        inspectorView.UpdateSelection(nodeView);
    }

    void OnInspectorUpdate() {
        treeView?.UpdateNodeState();
    }
}
