# MonoGraph
A simple graph data structure specifically written to be compatible with the Unity game engine, but perfectly usable on it's own.

It's been compiled under the .Net 3.5 runtime

In trying to find a good graph library for Unity I found many that were compatible with Mono but wouldn't work with Unity. So I decided to create yet another one, but make sure it was compatible in the hopes that the next poor soul that comes along looking for a graph library won't have to go through what I did.

This library is in its early stages, but the functionality that's here is well tested.

### Download/Installation
You can find the releases [here](https://github.com/jpinsonault/monograph/releases).

To make it available to your Unity project, download the `MonoGraph.dll` and place it somewhere in your project's `Assets` folder.

* [Quickstart](#quickstart)
* [Description/Features](#descriptionfeatures)
    * [AdjacencyListGraph](#adjacencylistgraph)
* [Usage](#usage)
    * [Creating a graph](#creating-a-graph)
    * [Adding vertices](#adding-vertices)
    * [Adding edges](#adding-edges)
    * [Iterating over vertices](#iterating-over-vertices)
    * [Iterating over edges](#iterating-over-edges)
    * [Iterating over all edges](#iterating-over-all-edges)
    * [Vertices](#vertices)
    * [Edges](#edges)
    * [IEdgeInterface](#iedgeinterface)
* [Algorithms](#algorithms)
    * [Dijkstra's shortest path](#dijkstras-shortest-path)


### Quickstart
```c#
using MonoGraph;
using System;

var stringGraph = new AdjacencyListGraph<string, Edge<string>>();

// Add vertices to graph
stringGraph.AddVertex("a");
stringGraph.AddVertex("b");
stringGraph.AddVertex("d");
stringGraph.AddVertex("e");

// Add edges to graph: a->b, a->c, a->d, b->c, b->e
stringGraph.AddDirectedEdge(new Edge<string>("a", "b"));
stringGraph.AddDirectedEdge(new Edge<string>("a", "c"));
stringGraph.AddDirectedEdge(new Edge<string>("a", "d"));
stringGraph.AddDirectedEdge(new Edge<string>("b", "c"));
stringGraph.AddDirectedEdge(new Edge<string>("b", "e"));

// Find all edges going out from "a"
foreach(var edge in stringGraph.EdgeIterator("a"){
    System.WriteLine(string.Format("{0} goes to {1}", edge.Start, edge.End);
}
// Outputs:
// a goes to b
// a goes to c
// a goes to d
```
### Description/Features

The libary tends toward being simple and modular. It doesn't do a whole lot for you, but provides a base you can build off of.

#### AdjacencyListGraph
Currently the only implementation. It might more accurately be called an AdjacencyDictionaryGraph as it stores vertices and their edges as `TVertex, List<TEdge>>` pairs in a dictionary, where TEdge and TVertex are the types chosen for your vertices and edges. 

### Usage
#### Creating a graph
`AdjacencyListGraph<TVertex, TEdge>()`
In general TVertex will be a `string` or `int` and TEdge will be the `Edge<TVertex>` class provided by this library
```c#
var stringGraph = new AdjacencyListGraph<string, Edge<string>>();
var intGraph = new AdjacencyListGraph<int, Edge<int>>();
```
Note that the type of vertex and the type of the `Edge` must match

#### Adding vertices
Vertices are added using `AdjacencyListGraph.AddVertex(TVertex vertex)`
```c#
stringGraph.AddVertex("a");
intGraph.AddVertex(42);
```
#### Adding edges
Note: Edges must be between vertices that already exist in the graph or an `VertexNotFoundException` will be thrown.
Edges are added using either `AdjacencyListGraph.AddDirectedEdge(TEdge edge)` or `AdjacencyListGraph.AddBidirectionalEdge(TEdge edge)`

`AddBidirectionalEdge` creates 2 edges, 1 in both directions
```c#
// Assume both vertices already both exist in the graph
stringGraph.AddDirectedEdge(new Edge<string>("a", "b"));

stringGraph.AddBidirectionalEdge(new Edge<string>("c", "d"));
// Equivalent to:
stringGraph.AddDirectedEdge(new Edge<string>("c", "d"));
stringGraph.AddDirectedEdge(new Edge<string>("d", "c"));
```
#### Iterating over vertices
Using  `AdjacencyListGraph.VertexIterator()`
```c#
foreach(string vertex in stringGraph.VertexIterator()){
    Console.WriteLine(vertex);
}
```
#### Iterating over edges
Using `AdjacencyListGraph.EdgeIterator(TVertex vertex)`
```c#
// Find all neighbors of "a"
Console.WriteLine("Neighbors of a:");
foreach(var edge in stringGraph.EdgeIterator("a")){
    Console.WriteLine(string.Format("a leads to {0}", edge.End));
}
// Outputs:
// Neighbors of a:
// a leads to b
// a leads to c
// etc
```
#### Iterating over all edges
Using `AdjacencyListGraph.AllEdgeIterator()`
```c#
// Assume graph has edges a->b, a->c, b->c, c->a
Console.WriteLine("All edges:");
foreach(var edge in stringGraph.AllEdgeIterator()){
    Console.WriteLine(string.Format("{0}->{1}"));
}
// Outputs:
// All edges:
// b->c
// c->a
// a->b
// a->c
```
#### Vertices
Vertices can be any hashable type (`int`, `string`, user defined classes, etc). 

#### Edges
Edges can any hashable type that implements the `IEdgeInterface` from this libaray. It's essentially a tuple of two vertices, `Start`, and `End`.

An `Edge` type is included that should cover your basic needs

#### IEdgeInterface
A type that implements the `IEdgeInterface` must have properties `Start` and `End`
It must also implement a method `Reversed` that returns a new copy of itself with `Start` and `End` reversed so that 
```c#
new CustomEdge<int>(1, 2).Reversed() == new CustomEdge<int>(2, 1) // => true
```

### Algorithms
Right now, only an implementation of Dijkstra's shortest path algorithm comes with the libary. More algorithms will be added as I make them or get PRs.

Algorithms are implemented as classes in the `MonoGraph.Algorithms` namespace

#### Dijkstra's shortest path
This computes the shortest path from a starting vertex to all other vertices in the graph.
It takes in a graph object and a dictionary mapping edges to their costs.
After creating the `DijkstraShortestPath` object, run `DijkstraShortestPath.ComputeAllFromVertex(TVertex startVertex)`. Results will be stored in the fields:
```c#
Dictionary<TVertex, double> ComputedCosts;
Dictionary<TVertex, TVertex> ComputedPaths;
```
and will only contain valid data after the computation has finished
```c#
// Create the graph
var testGraph = new AdjacencyListGraph<string, Edge<string>>;

// List of all vertices in graph
var vertices = new List<string> {"a", "b", "c", "d", "e"};

// Costs for crossing an edge
// This example shows that when getting a to b it's quicker to go the path a->d->c->b
// than it is to go a->b because of the higher cost of a->b
var edgesCosts = new Dictionary<Edge<string>, double> {
    {new Edge<string>("a", "d"), 3.0},
    {new Edge<string>("a", "b"), 20.0},
    {new Edge<string>("a", "e"), 2.0},
    {new Edge<string>("d", "c"), 2.0},
    {new Edge<string>("c", "b"), 2.0},
    {new Edge<string>("e", "d"), 2.0}
};

// Add each vertex to the graph
foreach(var vertex in vertices){
    testGraph.AddVertex(vertex);
}
// Add each edge to the graph
foreach(var edge in edgesCosts.Keys){
    testGraph.AddDirectedEdge(edge);
}

// Create shortest path object
var shortestPath = new Algorithms.DijkstraShortestPath<string, Edge<string>>;

// Run the computation
shortestPath.ComputeAllFromVertex("a");
// Cost from "a" to "b" is 7
shortestPath.ComputedCosts["b"]; // => 7

// Paths are stored in shortestPath.ComputedPaths
// Path from a to b is determined in reverse order
shortestPath.ComputedPaths["b"]; // -> c
shortestPath.ComputedPaths["c"]; // -> d
shortestPath.ComputedPaths["d"]; // -> a
// a->d->c->b
```
