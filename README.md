# UnityUtils
Various utilities for working with Unity

This project currently contains extensions to the MonoBehaviour class to make programming in a functional style with coroutines easier. All methods take two arguments: the first, a Func<bool>, is a function that returns a bool value, the second is an Action that is executed when this function returns true.

## While

Method signature:

```csharp
Coroutine While(Func<bool> expression, Action action, ExecutionStage stage = ExecutionStage.Update)
```

The second argument Action will be execute at every frame where the first argument expression is true.

Usage example:

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

Method signature:

```csharp
Coroutine When(Func<bool> expression, Action action, ExecutionStage stage = ExecutionStage.Update)
```

The second argument Action will be executed once, when the first argument expression becomes true. If the expression is already true, action is executed immediately.

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

Method signature:

```csharp
Coroutine Whenever(Func<bool> expression, Action action, ExecutionStage stage = ExecutionStage.Update)
```

The second argument Action will be executed every time the first argument expression changes from false to true. If the expression is already true, action is executed immediately.

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

Method signature:

```csharp
Coroutine Watch<T>(Func<T> expression, Action<T> action, ExecutionStage stage = ExecutionStage.Update)
```

This method is slightly different from the other in that the first argument expression can return a value of any type. The second argument action is called at every frame where the value returned by the expression is different from that of the previous frame. Equality comparison is performed with System.Collections.Generic.EqualityComparer<T>.Default.Equals.

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


