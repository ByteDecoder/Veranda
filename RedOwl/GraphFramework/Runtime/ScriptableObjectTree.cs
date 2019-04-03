using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using RedOwl.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RedOwl.GraphFramework
{
    public interface IInitializable
    {
        void Initialize();
    }

    public abstract class ScriptableObjectTree : SerializedScriptableObject, IInitializable
    {
        /// <summary>
        /// The unique identifier for this specific node in the tree
        /// </summary>
        [HideInInspector]
        public Guid id;

        /// <summary>
        /// The parent which this is a child of - null means its a top level node in the tree
        /// </summary>
        [HideInInspector]
        public ScriptableObjectTree parent;

        public override string ToString() => this.name;

        [NonSerialized]
		internal bool IsInitialized;
        public void Initialize()
        {
            if (IsInitialized) return;
            //UnityEngine.Debug.LogFormat("Initializing {0}", this);
            this.InternalInit();
            this.OnInit();
            IsInitialized = true;
        }
        internal virtual void InternalInit() {}

        /// <summary>
        /// Tell unity to mark this asset as dirty and all parent objects up the tree
        /// </summary>
        [Conditional("UNITY_EDITOR")]
		public void MarkDirty()
		{
			EditorUtility.SetDirty(this);
            OnDirty();
            if (parent != null) parent.MarkDirty();
		}
        public virtual void OnDirty() {}

		public void Execute()
		{
			if (parent == null)
			{
				InternalExecute();
			} else {
				parent.Execute();
			}
		}
        protected virtual void InternalExecute() {}

        // Contract
        public virtual void OnCreate() {}
        public virtual void OnInit() {}
        public virtual void OnExecute() {}
        public virtual void OnDelete() {}
    }

    public abstract class ScriptableObjectTree<T> : ScriptableObjectTree, IEnumerable<T> where T : ScriptableObjectTree
    {
        /// <summary>
        /// The list of children which this is a parent for
        /// </summary>
        /// <typeparam name="Guid">The id of the child</typeparam>
        /// <typeparam name="T">The type of the children</typeparam>
        /// <returns></returns>
        public Dictionary<Guid, T> children = new Dictionary<Guid, T>();

		public delegate void Cleared();
		public event Cleared OnCleared;

		public delegate void ChildAdded(T child);
		public event ChildAdded OnChildAdded;

		public delegate void ChildRemoved(T child);
		public event ChildRemoved OnChildRemoved;

		/// <summary>
		/// Returns the number of nodes in the graph
		/// </summary>
		/// <value></value>
		public int Count {
			get { return children.Count; }
		}

        /// <summary>
        /// Returns the child that matches the given Guid ID
        /// </summary>
        /// <value>GUID</value>
        public T this[Guid key] {
			get {
				return children[key];
			}
		}

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            foreach (var key in children.Keys.ToList())
            {
                if (children[key].IsInitialized) yield return children[key];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var key in children.Keys.ToList())
            {
                if (children[key].IsInitialized) yield return children[key];
            }
        }

        internal override void InternalInit()
        {
            foreach (var item in children.Values)
            {
                item.Initialize();
            }
        }

        /// <summary>
        /// Removes all data from this node in the tree
        /// </summary>
		public virtual void Clear()
		{
			children.Clear();
			ClearSubAssets<T>();
            OnCleared?.Invoke();
		}

        /// <summary>
        /// Add a child to this node in the tree
        /// </summary>
        /// <param name="child">The T object child to add</param>
        public void AddChild(T child)
        {
            child.id = Guid.NewGuid();
            child.name = child.id.ToString();
			child.parent = this;
            children.Add(child.id, child);
            AddSubAsset(child);
            child.OnCreate();
            child.Initialize();
            OnChildAdded?.Invoke(child);
        }

        /// <summary>
        /// Removes a child from this node in the tree
        /// </summary>
        /// <param name="id">The Guid ID of the child to remove</param>
        public void RemoveChild(Guid id)
        {
			RemoveChild(this[id]);
        }

        /// <summary>
        /// Removes the T child from this node in the tree
        /// </summary>
        /// <param name="child">The T object child to remove</param>
        public void RemoveChild(T child)
        {
			children.Remove(child.id);
            child.OnDelete();
			RemoveSubAsset(child);
            OnChildRemoved?.Invoke(child);
        }

		[Conditional("UNITY_EDITOR")]
		protected void ClearSubAssets<TAsset>() where TAsset : UnityEngine.Object
		{
			var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
			for (int i = subAssets.Length; i >= 0; i--)
			{ // Delete all subassets except the main one to preserve references
				if (subAssets[i] != this && subAssets[i].GetType() == typeof(TAsset)) DestroyImmediate(subAssets[i], true);
			}
			MarkDirty();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
		}
		
		[Conditional("UNITY_EDITOR")]
		protected void AddSubAsset<TAsset>(TAsset obj) where TAsset : UnityEngine.Object
		{
            if (string.IsNullOrEmpty(obj.name)) obj.name = string.Format("{0}_{1}", id.ToString(), typeof(TAsset).Name);
			AssetDatabase.AddObjectToAsset(obj, this);
			obj.hideFlags = HideFlags.HideInHierarchy;
            EditorUtility.SetDirty(obj);
			MarkDirty();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
		}

		[Conditional("UNITY_EDITOR")]
		protected void RemoveSubAsset(string name)
		{
			var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
			for (int i = subAssets.Length - 1; i >= 0 ; i--)
			{
                var asset = subAssets[i];
				if (asset != this && asset.name == name) RemoveSubAsset(asset);
			}
		}

		[Conditional("UNITY_EDITOR")]
		protected void RemoveSubAsset<TAsset>(TAsset obj) where TAsset : UnityEngine.Object
		{
            MarkDirty();
			EditorApplication.delayCall += () => {
                DestroyImmediate(obj, true);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            };
		}
    }
}
