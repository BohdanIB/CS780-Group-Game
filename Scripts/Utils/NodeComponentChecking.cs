
using Godot;
using System.Collections.Generic;

namespace CS780GroupProject.Scripts.Utils
{
	public static class NodeComponentChecking
	{
		/// <summary>
		/// Check whether given node has specified component type, then returns the component or null.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="n"></param>
		/// <returns></returns>
		public static T GetComponentOrNull<T>(Node n) where T : Node
		{
			if (GodotObject.IsInstanceValid(n) && n is T component)
			{
				return component;
			}
			return null;
		}

		/// <summary>
		/// Check current node, then iteratively look through all children of given node for 
		/// specified type.
		/// <br/>
		/// Potentially expensive call.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="n"></param>
		/// <returns></returns>
		public static T GetComponentInChildrenOrNull<T>(Node n) where T : Node
		{
			if (GodotObject.IsInstanceValid(n))
			{
				// Check for component in current node.
				if (GetComponentOrNull<T>(n) is T component && component != null) { return component; }

				// Check children of current node.
				List<Node> unexploredChildren = [];
				foreach (var child in n.GetChildren())
				{
					if (GetComponentOrNull<T>(child) is T childComponent && childComponent != null) { return childComponent; }
					unexploredChildren.Add(child);
				}
				// search rest of layers in children.
				foreach (var child in unexploredChildren)
				{
					if (GetComponentInChildrenOrNull<T>(child) is T childComponent && childComponent != null) { return childComponent; }
				}
			}
			return null;
		}

		/// <summary>
		/// Does the given node or its owner have the specified component?
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="n"></param>
		/// <returns></returns>
		public static bool HasComponent<T>(Node n) where T : Node
		{
			return GodotObject.IsInstanceValid(GetComponentOrNull<T>(n));
		}
	}
}
