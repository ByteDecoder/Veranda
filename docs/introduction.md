# Documentation

Heavily WIP!!!

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
