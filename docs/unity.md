# Experiences with Unity

This project was developed with the [Unity Engine](https://unity.com/) version 6000.0.26.

Alas my experience of Unity was far from universally positive.   The software seems to be spattered with bugs and design issues.   

## Bugs and Annoyances

Here are a few of the issues I experienced as a new user, but also as an experienced developer, in the few days spent thus far creating this project:

* *Building* - In releasing v0.0.2 the build failed even though the game ran perfectly in the editor and the C# compiled without issue in VSCode.   The build process displayed some errors in the editor which vanished from the screen as soon as the build completed - so it was a case of trying to grab the list of issues in the brief window of time where they were visible.
* *Importing Fonts* - Importing of fonts outright crashed the Unity editor.
* *Synchronization Errors* - I had several occasions where Unity's view of the world seemed be in conflict with that of Visual Studio/VSCode leading to errors being reported in the editor.
* *Random Script Failures* - In Visual Studio, the C# project would sometimes fail to build in a manner suggestive of it having lost track of the Unity packages.   Simply restarting Visual Studio always fixed the problem.   While this could well be an bug in Visual Studio rather than with Unity itself, Visual Studio is Unity's recommendation for use on Windows.  These issues vanished when I switched to VSCode.
* *Random Game Failures* - The Unity editor flagged errors in the project, which on restarting the editor magically disappeared.
* *Many Miscellaneous Issues* - I suffered numerous miscellaneous issues, here are a few I can bring to mind:
  * Animations which failed to preview in the editor even though they worked perfectly in the game.
  * A game object that just stopped working between runs when no changes to the object had been made.   Creating a new object with identical components and settings and switching out for the original fixed the problem.
  * I accidentally created a legacy version of an animation, there was nothing to flag this as I created it.  As a new used a hour was lost diagnosing and fixing the issue. 
  * Options to create the game UI seem to a mess of confusion as to which of the various packages to use.  A small point, but the the design of the `Rect Transform` anchoring mechanism is unnecessarily obfuscated - surely this is a solved problem already (define where to anchor to in the parent, what to anchor in the child and the offset).
  
## Design Issues

The dominant abstraction in Unity is that of the `GameObject` which is in turn composed of `Components`. Each `Component` type adds particular functionality to its associated `GameObject`, for example, one might add rigid body physics and another the ability to render a sprite image.  A script (urgh! it is a class), derived from `MonoBehaviour`, may be associated with a `GameObject` to add behaviour.  All `GameObjects` seem to be registered in a custom DI container.   

`Components` associated with a `GameObject` are publicly available via the DI container using `GetComponent<>` calls.  This mechanism is ubiquitous.  There is no encapsulation whatsoever - for example for a script to change the linear velocity of *any* `GameObject` in the entire project `GetComponent<Rigidbody2D>().linearVelocity = value` does the job, we'll find out whether that `GameObject` has a `RigidBody2D` at runtime!  This might be fine for a small project but I'd hate to see the chaos that could ensue when developing a complex game.

Unity seems encourage inter-object references defined via member variables that are exposed in the Unit editor, such as:

```
[SerializeField]
GameObject _someOtherObject
```

This very quickly becomes a *big-ball-of-string* of coupling. 

*Aside:* In my game I landed on creating an `EventHub` injected into most scripts to alleviate the direct coupling somewhat.   

Also there is zero typing here - that `GameObject` could be anything, an asteroid or player ship, and the reference will happily be made and issues only become apparent when the relevant code happens to be executed at run time.

In short, C# is a highly capably language with excellent support for complex projects, encapsulation and strong typing.  Unity seems to actively encourage the developer to adopt bad practices in these respects, leading to highly coupled and brittle code together errors that could be trapped at compile time only being reveals at run time.

## In Conclusion

I can't say I'd recommend Unity.  

Most of the issues I suffered can be worked around, and likely some were down to my being new to the tool. However the discovery of so many problems while developing a very simple game gives me cause for concern as to the quality of the Unity's core code.  This core code will course form an underpinning to every Unity game.

My plan is to next take a look the [Godot](https://godotengine.org/) game engine.
