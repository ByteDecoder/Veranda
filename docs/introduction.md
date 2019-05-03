---
layout: default
nav_order: 2
---

## Introduction
---

The best way to get started is to have a look at some example code.  Once you have installed the package there is an Examples.unitypackage file that you can import into your project.  This will create `Assets/Examples/RedOwl/DemoGraph` that has a simple arithmatic graph tool setup.  Start by clicking on the `DemoGraph.asset` file and opening the graph editor.  From there you can right click (in some empty space) to add more nodes or play around with the nodes that are there.  Once you are familiar with the graph tool and its node then you can start looking over the code.  The classes that are important for graph tool developers are lightly documented below.

## Classes

### Node

This is the base class all nodes must inherit from - its the workhorse of the group and there is alot of boilerplate functionality encapsulated in it that you don't need to worry about you just inherit from this class and worry about your business logic and data.  Checkout the demo code to see how to write nodes more indepth.

For example
```cs
using RedOwl.GraphFramework;

namespace RedOwl.Demo
{
    public class MultiplyNode : DemoNode
    {
        public float factor;

        public InOutPort<float> Data = new InOutPort<float>();

        public override void OnExecute()
        {
            Data.value *= factor;
        }
    }
}
```

### Graph<T>

This is the base graph class that you must define a specific class for and the type argument passed is the kind of nodes you can instantiate inside the graph.

### InputPort<T>

Define this as a field with a given type argument on your node and it will expose an input that you can hookup from another nodes output

### OutputPort<T>

Define this as a field with a given type argument on your node and it will expose an output that you can hookup to another nodes input

### InOutPort<T>

Its as if you combind Input and Output ports into one thing
