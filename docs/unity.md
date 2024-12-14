# Experiences with Unity

This project was developed with the [Unity Engine](https://unity.com/) version 6000.0.26.

Alas my experience of Unity was far from universally positive.   The software seems to be spattered with bugs and design issues.   

## Bugs and Annoyances

Here are a few of the issues I experienced as a new user, but also as an experienced developer, in the few days spent developing this project thus far:

* *Building* - In releasing v0.0.2 the build failed even though the game ran perfectly in editor and all of the C# compiled without issue in VSCode.   The build process did display some issues in the UI which vanished from the screen as soon as the build completed - so it was a case of trying to grab the list of issues in the brief window of time where they were visible.
* *Importing Fonts* - Importing of fonts outright crashed the Unity Editor.
* *Synchronization Errors* - I had several occasions where Unity's view of the world seemed become in conflict with that of Visual Studio/VSCode.
* *Random Script Failures* - In Visual Studio, the C# project would sometimes fail to build in a manner suggestive of it having lost track of the Unity packages.   Simply restarting Visual Studio always fixed the problem.   While this could well be an bug in Visual Studio rather than with Unity itself, Visual Studio is Unity's recommendation for use on Windows.  These issues vanished when I switched to VSCode.
* *Random Game Failures* - The Unity editor flagged errors in the project, which on restarting the editor magically disappeared.
* *Many Miscellaneous Issues* - I suffered numerous miscellaneous issues, here are a couple of random problems I can bring to mind.
  * Animations which failed to preview in the editor even though they worked perfectly in the game.
  * A game object that just stopped working between runs when no changes to the object had been made.   Creating a new object with identical components and settings and switching out for the original fixed the problem.
  * I accidentally created a legacy version of an animation, there was nothing to flag this as I created it.  As a new used a hour was lost diagnosing and fixing the issue. 
  
## Design Issues

The dominant abstraction in Unity is that of a `GameObject` which is in turn composed of `Components`. Each `Component` type particular functionality to its associated `GameObject`, for example, rigid body physics or the rendering of a sprite image.  A script (urgh! it is a class), derived from `MonoBehaviour`, may associated each `GameObject` to add custom behaviour.  All `GameObjects` seem to be registered in a custom DI container.   

`Components` associated with a `GameObject` are publicly available via the DI container via `GetComponent<>` calls.  This mechanism is commonly used.  There is no encapsulation whatsoever - for a script to change the linear velocity of any `GameObject` in the entire project `GetComponent<Rigidbody2D>().linearVelocity = value` does the job, we'll find out whether that `GameObject` has a `RigidBody2D` at runtime!  This might be fine for a small project but I'd hate to see the chaos that could ensure when developing a complex game.

The model seems encourage nter-object references defined via member variables that are exposed in the Unit editor, such as:

```
[SerializeField]
GameObject _someOtherObject
```

This very easily becomes a big ball of string of coupling. 

Aside: In my game I landed on creating a `EventHub` injected into most scripts to alleviate the direct coupling somewhat.   

Also there is zero typing here - that `GameObject` could be anything, an asteroid or player ship, and the reference will happily be made and issues will not appear until run-time.

In short C# is a highly capably language with excellent support for complex projects, encapsulation and strong typing.  Unity seems to actively encourage the developer to adopt bad practices in these regards leading to brittle code and errors that could be trapped at compile time only becoming apparent at run time.

## In Conclusion

I can't say I'd recommend Unity.  

While most of the issues I suffered can be worked around, and likely some were down to my being new to the tool. However the discovery of so man issues in developing a very simple game give me cause for concern as to the quality of the Unity's underlying code, which of course forms an underpinning to any completed games.

My plan is to next take a look the [Godot](https://godotengine.org/) game engine.
