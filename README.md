# UnityUtils
Various utilities for working with Unity

This project currently contains extensions to the MonoBehaviour class to make programming in a functional style with coroutines easier. All methods take two arguments: the first, a Func<bool>, is a function that returns a bool value, the second is an Action that is executed when this function returns true.

## While

```csharp
using JMP;
public class MyComponent : MonoBehaviour {

  public int life = 100;

  void OnEnable() {
    this.While(() => life < 10, () => {
      Debug.Log("Life is critical!");
    });
  }
}
```

## When

```csharp
using JMP;
public class MyComponent : MonoBehaviour {

  public int life = 100;

  void OnEnable() {
    this.When(() => life <= 0, () => {
      Debug.Log("Player died!");
    });
  }
}
```

## Whenever

```csharp
using JMP;
public class MyComponent : MonoBehaviour {

  public int life = 100;

  void OnEnable() {
    this.Whenever(() => life < 10, () => {
      Debug.Log("Player life became critical!");
    });
  }
}
```

## Watch

```csharp
using JMP;
public class MyComponent : MonoBehaviour {

  public int life = 100;

  void OnEnable() {
    this.Watch(() => life, (val) => {
      Debug.Log("Player life changed to: " + val);
    });
  }
}
```

## Specifying execution timing
Unity coroutines can be executed at these times:
- After Update has been called
- After LateUpdate has been called
- After FixedUpdate has been called

By default, the methods described above are executed after Update has been called. If, instead you wish to execute both the test expression and the conditional action at another time, you can use the following syntax:

```csharp
this.Watch(() => life, (val) => {
  Debug.Log("Player life changed to: " + val);
}, ExecutionStage.LateUpdate);

this.Watch(() => life, (val) => {
  Debug.Log("Player life changed to: " + val);
}, ExecutionStage.FixedUpdate);
```

You can pass the same values as third argument to all the methods. Note that if the condition expression is true when the methods are called, the action is executed immediately.

## Beware of Start and Awake
In Unity, when a GameObject is made inactive, all associated coroutines are cancelled. This means that if you call Watch inside the Start method and the game object is made inactive, the action will not be executed even after the game object is made active again. If that is not what you want, use OnEnable instead as in the examples above.

## Cancelling statements
While, When, Whenever and Watch all return Coroutine instances. You can use these to stop their execution using standard Unity mechanisms.

```csharp
Coroutine lifeWatch = this.Watch(() => life, (val) => {
  Debug.Log("Player life changed to: " + val);
});

this.When(() => life <= 0, (val) => {
  Debug.Log("Player is dead.");
  StopCoroutine(lifeWatch);
});
```


