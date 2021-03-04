using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNodeBase {
}

public class Tree<T> where T : TreeNodeBase {
	T holder;
	List<Tree<T>> parentList;

	void AddParent(Tree<T> parent) {

		parentList.Add(parent);
	}
	void RemoveParent(Tree<T> parent) {
		parentList.Remove(parent);
	}
	void RemoveParent(int index) {
		parentList.RemoveAt(index);
	}

	void AddChild(ref Tree<T> child) {
		child.AddParent(this);
	}
}


