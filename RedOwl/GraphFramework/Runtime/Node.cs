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

    public abstract class Node : RedOwl.Serialization.SerializedScriptableObject, IEnumerable<IPort>
    {
        [HideInInspector]
        public Guid id;
        [HideInInspector]
        public Graph graph;

        [HideInInspector]
        public NodeViewData view;

        [NonSerialized]
        public Dictionary<Guid, PortInfo> portInfos;

        private IPort GetPort(PortInfo info)
        {
            IPort port = info.Get(this);
            port.name = info.Name;
            return port;
        }

        /// <summary>
        /// Returns the port with the given GUID
        /// </summary>
        /// <value>GUID</value>
        public IPort this[Guid key]
        {
            get
            {
                return GetPort(portInfos[key]);
            }
        }

        IEnumerator<IPort> IEnumerable<IPort>.GetEnumerator()
        {
            foreach (var port in portInfos.Values)
            {
                yield return GetPort(port);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var port in portInfos.Values)
            {
                yield return GetPort(port);
            }
        }

        [NonSerialized]
        private bool IsInitialized;

        internal void Initialize()
        {
            if (IsInitialized) return;
            Type type = this.GetType();
            view.title = type.Name.Replace("Node", "");
            view.labelWidth = 80;
            view.color = Color.gray;
            type.WithAttr<NodeColorAttribute>(a => { view.color = a.color; });
            type.WithAttr<NodeTitleAttribute>(a => { view.title = a.title; });
            type.WithAttr<NodeWidthAttribute>(a => { view.layout.width = a.width; });
            type.WithAttr<NodeLabelWidthAttribute>(a => { view.labelWidth = a.width; });
            var infos = PortCache.Get(this.GetType());
            portInfos = new Dictionary<Guid, PortInfo>(infos.Count);
            IPort port;
            foreach (PortInfo info in infos)
            {
                port = info.Get(this);
                portInfos.Add(port.id, info);
            }
            IsInitialized = true;
        }

        /// <summary>
        /// Call this function to collapse the node
        /// </summary>
        /// <param name="collapse">If true node will be collapsed</param>
        public void Collapse(bool collapse = true)
        {
            view.collapsed = collapse;
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }

        //Contract
        public virtual void OnExecute() { }
    }
}
