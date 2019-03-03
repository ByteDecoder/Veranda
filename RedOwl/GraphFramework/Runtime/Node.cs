using System;
using System.Collections.Generic;
using UnityEngine;
using RedOwl.Serialization;
using System.Collections;

namespace RedOwl.GraphFramework
{
    public struct NodeViewData
    {
        public string title;
		public Color color;
        public bool collapsed;
		public Rect layout;

        private bool hasSizeFix;
        public void SetHeight(float height)
		{
			if (!hasSizeFix)
			{
				layout.height = height;
				hasSizeFix = true;
			}
		}
    }

    public abstract class Node : SerializedScriptableObject, IEnumerable<IPort>
    {
        public Guid id;
        public Graph graph;

        public NodeViewData view;

        public Dictionary<Guid, IPort> ports { get; protected set; }

        /// <summary>
        /// Returns the port with the given GUID
        /// </summary>
        /// <value>GUID</value>
        public IPort this[Guid key]
        {
            get
            {
                return ports[key];
            }
        }

        IEnumerator<IPort> IEnumerable<IPort>.GetEnumerator()
        {
            foreach (var port in ports.Values)
            {
                yield return port;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var port in ports.Values)
            {
                yield return port;
            }
        }

        [NonSerialized]
        private bool IsInitialized;
        private Dictionary<Guid, IPort> _ports;

        internal void Initialize()
        {
            if (IsInitialized) return;
            Type type = this.GetType();
            view.title = type.Name.Replace("Node", "");
            view.color = Color.gray;
            type.WithAttr<NodeColorAttribute>(a => { view.color = a.color; });
            type.WithAttr<NodeTitleAttribute>(a => { view.title = a.title; });
            type.WithAttr<NodeWidthAttribute>(a => { view.layout.width = a.width; });
            var infos = PortCache.Get(this.GetType());
            ports = new Dictionary<Guid, IPort>(infos.Count);
            IPort port;
            foreach (PortInfo info in infos)
            {
                port = info.Get(this);
                port.name = info.Name;
                ports.Add(port.id, port);
            }
            IsInitialized = true;
        }

        public override string ToString()
        {
            return this.GetType().Name;
        }

        //Contract
        public virtual void OnExecute() { }
    }
}
