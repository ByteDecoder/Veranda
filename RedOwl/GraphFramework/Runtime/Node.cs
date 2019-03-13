using System;
using System.Collections.Generic;
using UnityEngine;
using RedOwl.Serialization;
using System.Collections;

namespace RedOwl.GraphFramework
{
    [Serializable]
    public struct NodeViewData
    {
        public string title;
        public float labelWidth;
		public Color color;
        public bool collapsed;
		public Rect layout;
    }

    public abstract class Node : ScriptableObjectTree<Node>
    {
        //[HideInInspector]
        //public Graph graph;

        [HideInInspector]
        public NodeViewData view;

        [NonSerialized]
        public Dictionary<Guid, PortInfo> portInfos;

        public Port GetPort(Guid portId)
        {
            PortInfo info = portInfos[portId];
            Port port = info.Get(this);
            port.name = info.name;
            port.direction = info.direction;
            //Debug.LogFormat("Port {0}.{1} has {2} | {3}", GetType().Name, port.name, port.direction, port.style);
            return port;
        }

        public IEnumerable<Node> nodes {
            get {
                foreach (var node in children.Values)
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<Port> ports {
            get {
                foreach (var id in portInfos.Keys)
                {
                    yield return GetPort(id);
                }
            }
        }

        internal override void OnInit()
        {
            base.OnInit();
            Type type = this.GetType();
            view.title = type.Name.Replace("Node", "");
            view.labelWidth = 80;
            view.color = Color.gray;
            type.WithAttr<NodeTitleAttribute>(a => { view.title = a.title; });
            type.WithAttr<NodeLabelWidthAttribute>(a => { view.labelWidth = a.width; });
            var infos = PortCache.Get(this.GetType());
            portInfos = new Dictionary<Guid, PortInfo>(infos.Count);
            Port port;
            foreach (PortInfo info in infos)
            {
                port = info.Get(this);
                portInfos.Add(port.id, info);
            }
        }

        public void Duplicate(Node orig)
        {
            // TODO: copy data from one node to another - how?
        }

        /// <summary>
        /// Call this function to collapse the node
        /// </summary>
        /// <param name="collapse">If true node will be collapsed</param>
        public void Collapse(bool collapse = true)
        {
            view.collapsed = collapse;
        }

        public override string ToString() => this.GetType().Name;

        //Contract
        public virtual void OnExecute() { }
    }
}
