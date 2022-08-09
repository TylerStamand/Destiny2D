using UnityEngine.UIElements;
using UnityEditor;
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

    Editor editor;

    public InspectorView() {

    }

    public void UpdateSelection(NodeView nodeView) {
        Clear();

        UnityEngine.Object.DestroyImmediate(editor);
        editor = Editor.CreateEditor(nodeView.Node);
        IMGUIContainer container = new IMGUIContainer(() => { 
            //This check is for deleting node with undoredo
            if(editor.target)
                editor.OnInspectorGUI();
        });
        Add(container);
    }
}
