using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class DataContext {
    public Dictionary<string, object> data = new Dictionary<string, object>();
    

    public void SetValue(string name, object value) {
        if(data.ContainsKey(name)) {
            if(data[name] != value) {
            }
            data[name] = value;
        }
        else {
            data.Add(name, value);
        }
    }

    public object GetValue(string name) {
        if(data.ContainsKey(name)) {
            return data[name];
        }
        return null;
    }
}

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node RootNode;
    public Node.State TreeState = Node.State.Running;
    public List<Node> nodes = new List<Node>();

    DataContext dataContext;

    public Node.State Update() {
        if(RootNode.CurrentState == Node.State.Running ) {
            TreeState = RootNode.Update();
        }
        return TreeState;
    }
  

    public Node CreateNode(System.Type type) {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.GUID = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
        nodes.Add(node);

        //Not allowed to make assets in playmode
        if(!Application.isPlaying)
            AssetDatabase.AddObjectToAsset(node, this);
        
        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

        AssetDatabase.SaveAssets();
        
        return node;
    }

    public void DeleteNode(Node node) {
        Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
        nodes.Remove(node);

        // AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child) {
        DecoratorNode decorator = parent as DecoratorNode;
        if(decorator) {
            Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
            decorator.child = child;
            EditorUtility.SetDirty(decorator);
        }

        RootNode rootNode = parent as RootNode;
        if (rootNode) {
            Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
            rootNode.child = child;
            EditorUtility.SetDirty(rootNode);

        }

        CompositeNode composite = parent as CompositeNode;
        if(composite) {
            Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
            composite.Children.Add(child);
            EditorUtility.SetDirty(composite);

        }
    }

    public void RemoveChild(Node parent, Node child) {
        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator) {
            Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
            decorator.child = null;
            EditorUtility.SetDirty(decorator);
        }


        RootNode rootNode = parent as RootNode;
        if (rootNode) {
            Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
            rootNode.child = null;
            EditorUtility.SetDirty(rootNode);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) {
            Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
            composite.Children.Remove(child);
            EditorUtility.SetDirty(composite);
        }
    }

    public List<Node> GetChildren(Node parent) {
        List<Node> children = new List<Node>();

        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator && decorator.child != null) {
            children.Add(decorator.child);
        }


        RootNode rootNode = parent as RootNode;
        if (rootNode && rootNode.child != null) {
            children.Add(rootNode.child);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite) {
            return composite.Children;
        }
        return children;
    }

    public void Traverse(Node node, System.Action<Node> visiter) {
        if(node) {
            visiter.Invoke(node);
            List<Node> children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }

    public BehaviourTree Clone(Enemy enemy) {
        BehaviourTree tree = Instantiate(this);
        tree.RootNode = tree.RootNode.Clone();
        tree.nodes = new List<Node>();
        tree.dataContext = new DataContext();
        Traverse(tree.RootNode, (n) => {
            n.Agent = enemy;
            n.DataContext = tree.dataContext;
            tree.nodes.Add(n);
        });
        return tree;
    }
}
