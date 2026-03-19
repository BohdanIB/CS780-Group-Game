
using Godot;
using System.Collections.Generic;

namespace CS780GroupProject.Scripts.Utils
{
	public static class NodeComponentChecking
	{
		/// <summary>
		/// Check whether given node or its owner has specified component type, then returns the component or null.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="n"></param>
		/// <returns></returns>
		public static T GetComponentOrNull<T>(Node n) where T : Node
		{
			if (GodotObject.IsInstanceValid(n))
			{
				// Check children of given node, node is the root of the scene.
				foreach (var child in n.GetChildren())
				{
					if (child is T component) { return component; }
				}
				// Check children of node's owner, node is a component or some child of the main entity.
				foreach (var child in n.Owner.GetChildren())
				{
					if (child is T component) { return component; }
				}
			}
			return null;
		}

		/// <summary>
		/// Iteratively look through all children in given node to look for specified type.
		/// <br/>
		/// Potentially expensive call.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="n"></param>
		/// <returns></returns>
		public static T GetComponentInChildrenOrNull<T>(Node n) where T : Node
		{
			List<Node> unexploredChildren = [];
			if (GodotObject.IsInstanceValid(n))
			{
				foreach (var child in n.GetChildren())
				{
					if (child is T component) { return component; }
					unexploredChildren.Add(child);
				}
				// search rest of layers in children.
				foreach (var child in unexploredChildren)
				{
					var component = GetComponentInChildrenOrNull<T>(child);
					if (component != null) { return component; }
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
			return GetComponentOrNull<T>(n) != null;
		}
	}
}
