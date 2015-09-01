using System;
using System.Collections.Generic;
using System.Linq;

namespace HeterogeneousDataSources {
    public class Tree<T>{
        public Tree(T node, List<Tree<T>> children){
            Node = node;
            Children = children;
        }

        public T Node { get; private set; }
        public List<Tree<T>> Children { get; private set; }

        public Tree<TResult> Projection<TResult>(Func<T, TResult> selector)
        {
            return new Tree<TResult>(
                node: selector(Node),
                children: Children
                    .Select(child=>child.Projection(selector))
                    .ToList()
            );
        }
    }
}
