using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;

    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits> {}
    BehaviourTree tree;

    public BehaviourTreeView() {
        Insert(0, new GridBackground());
        
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTree/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    NodeView FindNodeView(Node node) {
        return GetNodeByGuid(node.GUID) as NodeView;
    }

    public void PopulateView(BehaviourTree tree) {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if(tree.RootNode == null) {
            tree.RootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }


        //Create node view
        tree.nodes.ForEach(n => CreateNodeView(n));

        //Create edges
        tree.nodes.ForEach(n => {
            List<Node> children = tree.GetChildren(n);
            children.ForEach(c => {
                NodeView parentView = FindNodeView(n);
                NodeView childView = FindNodeView(c);

                UnityEditor.Experimental.GraphView.Edge edge = parentView.Output.ConnectTo(childView.Input);
                AddElement(edge);
            });
        });
    }

    //This is for a possible bug
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) {
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) {
        if(graphViewChange.elementsToRemove != null) {
            graphViewChange.elementsToRemove.ForEach( elem => {
                NodeView nodeView = elem as NodeView;
                if(nodeView != null) {
                    tree.DeleteNode(nodeView.Node);
                }

                //Fully qualify because i have my own edge type
                UnityEditor.Experimental.GraphView.Edge edge = elem as UnityEditor.Experimental.GraphView.Edge;
                if(edge != null) {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.Node, childView.Node);
                }
            });
        }

        if(graphViewChange.edgesToCreate != null) {
            graphViewChange.edgesToCreate.ForEach(edge => {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                tree.AddChild(parentView.Node, childView.Node);
            });
        }

        if(graphViewChange.movedElements != null) {
            nodes.ForEach((n) =>  {
                NodeView view = n as NodeView;
                view.SortChildren();
            });
        }
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) {
        // base.BuildContextualMenu(evt);
        var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
        foreach(var type in types) {
            evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
        }

        types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
        foreach (var type in types) {
            evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
        }

        types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
        foreach (var type in types) {
            evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
        }
    }

    void CreateNode(System.Type type) {
        Node node = tree.CreateNode(type);
        CreateNodeView(node);
    }

    void CreateNodeView(Node node) {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }
    
    public void UpdateNodeState() {
        nodes.ForEach(n => {
            NodeView view = n as NodeView;
            view.UpdateState();
        });
    }

    void OnUndoRedo() {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }
}
