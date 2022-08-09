using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

using Direction = UnityEditor.Experimental.GraphView.Direction;
using System;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node {
    public Action<NodeView> OnNodeSelected;
    public Node Node;
    public Port Input;
    public Port Output;

    public NodeView(Node node) : base("Assets/Editor/BehaviourTree/NodeView.uxml") {
        Node = node;
        title = node.name;
        viewDataKey = node.GUID;

        style.left = node.Position.x;
        style.top = node.Position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
    }

    void SetupClasses() {
        switch(Node) {
            case ActionNode:
                AddToClassList("action");
                break;
            case CompositeNode:
                AddToClassList("composite");
                break;
            case DecoratorNode:
                AddToClassList("decorator");
                break;
            case RootNode:
                AddToClassList("root");
                break;
        }
    }

    public override void SetPosition(Rect newPos) {
        base.SetPosition(newPos);
        Undo.RecordObject(Node, "Behaviour Tree (Set Position)");
        Node.Position.x = newPos.xMin;
        Node.Position.y = newPos.yMin;
        EditorUtility.SetDirty(Node);
    }

    void CreateInputPorts() {
        if(Node is ActionNode) {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        } else if( Node is CompositeNode) {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        } else if (Node is DecoratorNode) {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        } else if (Node is RootNode) {

        }

        if(Input != null) {
            Input.portName = "";
            Input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(Input);
        }
    }

    void CreateOutputPorts() {
        if (Node is ActionNode) {
        }
        else if (Node is CompositeNode) {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        } else if (Node is DecoratorNode) {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        } else if (Node is RootNode) {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (Output != null) {
            Output.portName = "";
            Output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(Output);
        }
    }

    public override void OnSelected() {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    public void SortChildren() {
        CompositeNode composite = Node as CompositeNode;
        if(composite) {
            composite.Children.Sort(SortByHorizontalPosition);
        }
    }

    int SortByHorizontalPosition(Node left, Node right) {
        return left.Position.x < right.Position.x ? -1 : 1;
    }

    public void UpdateState() {


        if(Application.isPlaying) {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            Debug.Log("Updating state class");
            
            switch(Node.CurrentState) {
                case Node.State.Running:
                    if(Node.Started) {
                        AddToClassList("running");
                    }
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
            }
            
        }
    }
}
