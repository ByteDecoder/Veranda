<h1 align="center">Sleipnir</h1>
<h4 align="center">A graph editor framework for Unity's new UIElements system.</h4>

<p align="center">
    <a href="#introduction">Introduction</a> •
    <a href="#installation">Installation</a> •
    <a href="#documentation">Documentation</a> •
</p>

# Key Features

* Easier to use then Unity's UIElements graph framework
* Focus on the logic of your graph based tool not the graph editor code that enables it
* Built on top of Unity's UIElements framework 
* Graph data is available for runtime use (Runtime UI will come when unity makes UIElements work in the runtime - 2020)

#### NOTE: This is a library for coders to help them make graph based tools in Unity easier and faster

<h2 align="center">
	If this library helps you out consider 
<link href="https://fonts.googleapis.com/css?family=Lato&subset=latin,latin-ext" rel="stylesheet"><a class="bmc-button" target="_blank" href="https://www.buymeacoffee.com/hu2HD8AkM"><span style="margin-left:5px">buying me a coffee!</span><img src="https://www.buymeacoffee.com/assets/img/BMC-btn-logo.svg" alt="Buy me a coffee"></a>	
</h2>

# Introduction

Lets start out with an example graph tool that does some simple math to show you how to get started.

First you must define a base node all your nodes will inhert from and a graph class for those nodes

<details>
  <summary>Example Arthmetic Graph Code (click to open)</summary><p>

```cs
using RedOwl.GraphFramework;

namespace RedOwl.Demo
{
	public abstract class DemoNode : Node {}
}
```

```cs
using RedOwl.GraphFramework;

namespace RedOwl.Demo
{
	[CreateAssetMenu(menuName="Demo/Graph", fileName="Graph")]
	public class DemoGraph : Graph<DemoNode> {}
}
```

</p></details>

Once have you defined the base classes now you can being designing your node classes

<details>
  <summary>Example Arthmetic Nodes Code (click to open)</summary><p>

```cs
using RedOwl.GraphFramework;

namespace RedOwl.Demo
{
    public class ValueNode : DemoNode
    {
        public OutputPort<float> Value = new OutputPort<float>(1f);
    }
}
```

```cs
using RedOwl.GraphFramework;

namespace RedOwl.Demo
{
    public class AdditionNode : DemoNode
    {
        public float factor;

        public InOutPort<float> Data = new InOutPort<float>();

        public override void OnExecute()
        {
            Data.value += factor;
        }
    }
}
```

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

```cs
using RedOwl.GraphFramework;

namespace RedOwl.Demo
{
    public class DebugNode : DemoNode
    {
        public InputPort<string> Data = new InputPort<string>();

        public override void OnExecute()
        {
            Debug.Log(Data.value);
        }
    }
}
```

</p></details>

Now with this sample code you can build graphs that when executed will add numbers togeather in a chain and eventually log them out to the editor console.  See the example image linked below

![Demo](./Demo.gif)

# Installation

The best method if you are using Unity > 2018.3 is via the new package manager.

- In your unity project root open `Packages/manifest.json`
- Add the following line to the dependencies section `"com.redowl.sleipnir": "https://github.com/rocktavious/Sleipnir.git",`
- Open Unity and the package should download automatically

If you are using Unity < 2018.3 - i'm sorry you are out of luck, UIElements is only useable in this version of unity or higher

# Documentation

## Classes

### Node

This is the base class all nodes must inherit from - its the workhorse of the group and there is alot of boilerplate functionality encapsulated in it that you don't need to worry about you just inherit from this class and worry about your business logic and data

### Graph<T>

This is the base graph class that you must define a specific class for and the type argument passed is the kind of nodes you can instantiate inside the graph.

### InputPort<T>

Define this as a field with a given type argument on your node and it will expose an input that you can hookup from another nodes output

### OutputPort<T>

Define this as a field with a given type argument on your node and it will expose an output that you can hookup to another nodes input

### InOutPort<T>

Its as if you combind Input and Output ports into one thing
