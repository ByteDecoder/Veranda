<h1 align="center">Sleipnir</h1>
<h4 align="center">A graph editor framework for Unity's new UIElements system.</h4>

<p align="center">
    <a href="#introduction">Introduction</a> •
    <a href="#installation">Installation</a> •
    <a href="https://redowlgames.com/Sleipnir">Documentation</a>
</p>

# Key Features

* Easier to use then Unity's graph framework
* Focus on the logic of your graph based tool not the graph editor code that enables it
* Built on top of Unity's UI Toolkit framework 
* Graph data is available for runtime use (Runtime UI will come when unity makes UI Toolkit work in the runtime - 2020 & Beyound)

#### NOTE: This is a library for coders to help them make graph based tools in Unity easier and faster

<h2 align="center">
	If this library helps you out consider 
<link href="https://fonts.googleapis.com/css?family=Lato&subset=latin,latin-ext" rel="stylesheet"><a class="bmc-button" target="_blank" href="https://www.buymeacoffee.com/redowlgames"><span style="margin-left:5px">buying me a coffee!</span><img src="https://www.buymeacoffee.com/assets/img/BMC-btn-logo.svg" alt="Buy me a coffee"></a>	
</h2>

# Introduction

![Demo](./Demo.gif)

If you want to learn more go check out our documentation [here](https://redowlgames.com/Sleipnir/introduction.html)

# Installation

If you are unsure how to use the new unity package manager with git check it out on our documentation [here](https://redowlgames.com/Sleipnir/installation.html)


# Planning

A Node has N ports
A Graph is an Adjacency list of Ports
Ports are dynamically initialized
but something has to be serialized - maybe a port ID or something
the graphs adjacency list details how/which ports connect to which
if a node definition changes the ports have to be reconciled and the adjacency list has to be updated
A GraphControl performs port initialization