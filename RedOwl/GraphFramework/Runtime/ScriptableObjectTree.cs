using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
		private bool IsInitialized;
        public void Initialize()
        {
            if (IsInitialized) return;
            //UnityEngine.Debug.LogFormat("Initializing {0}", this);
            this.OnInit();
            IsInitialized = true;
        }
        internal virtual void OnInit() {}
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
            foreach (var item in children.Values)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var item in children.Values)
            {
                yield return item;
            }
        }

        internal override void OnInit()
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
			MarkDirty();
		}

        /// <summary>
        /// Add a child to this node in the tree
        /// </summary>
        /// <param name="child">The T object child to add</param>
        public void AddChild(T child)
        {
            child.id = Guid.NewGuid();
			child.parent = this;
            child.Initialize();
            children.Add(child.id, child);
            AddSubAsset(child);
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
			RemoveSubAsset(child.id.ToString());
        }

        /// <summary>
        /// Tell unity to mark this asset as dirty
        /// </summary>
        /// <param name="children">If true all the children will be marked dirty too</param>
        /// <param name="all">If true all the children will be marked dirty too</param>
        [Conditional("UNITY_EDITOR")]
		public void MarkDirty(bool children = false, bool all = false)
		{
			EditorUtility.SetDirty(this);
            if (all)
            {
                var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
                for (int i = 0; i < subAssets.Length; i++)
                {
                    EditorUtility.SetDirty(subAssets[i]);
                }
            } else if (children)
            {
                foreach (var item in this.children.Values)
                {
                    EditorUtility.SetDirty(item);
                }
            }
		}

		[Conditional("UNITY_EDITOR")]
		internal void ClearSubAssets<TAsset>()
		{
			var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
			for (int i = subAssets.Length; i >= 0; i--)
			{ // Delete all subassets except the main one to preserve references
				if (subAssets[i] != this && subAssets[i].GetType() == typeof(TAsset)) DestroyImmediate(subAssets[i], true);
			}
			EditorUtility.SetDirty(this);
		}
		
		[Conditional("UNITY_EDITOR")]
		internal void AddSubAsset(T obj)
		{
			obj.name = obj.id.ToString();
			AssetDatabase.AddObjectToAsset(obj, this);
			obj.hideFlags = HideFlags.HideInHierarchy;
			EditorUtility.SetDirty(this);
			EditorUtility.SetDirty(obj);
		}

		[Conditional("UNITY_EDITOR")]
		internal void RemoveSubAsset(string name)
		{
			var subAssets = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(this));
			for (int i = 0; i < subAssets.Length; i++)
			{
				if (subAssets[i] != this && subAssets[i].name == name) DestroyImmediate(subAssets[i], true);
			}
			EditorUtility.SetDirty(this);
		}
    }
}
