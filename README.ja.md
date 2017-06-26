# UnityUtils
Unityのための便利なツール

現時点でこのプロジェクトが実装しているのは、より簡単にCoroutineを使って関数型プログラミングを手伝ういくつかのMonoBehaviourの拡張メソッド。

## While

メソッドシグネチャ:

```csharp
Coroutine While(Func<bool> expression, Action action, ExecutionStage stage = ExecutionStage.Update)
```

2つ目の引数の「action」は1つ目の引数のexpressionの返り値がtrueになっているすべてのフレームで実行される。

使用例:

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

メソッドシグネチャ:

```csharp
Coroutine When(Func<bool> expression, Action action, ExecutionStage stage = ExecutionStage.Update)
```

2つ目の引数の「action」は1つ目の引数のexpressionの返り値がtrueになれば１回だけ実行される。最初からtrueになっている場合も実行される。

使用例:

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

メソッドシグネチャ:

```csharp
Coroutine Whenever(Func<bool> expression, Action action, ExecutionStage stage = ExecutionStage.Update)
```

2つ目の引数の「action」は1つ目の引数のexpressionの返り値がfalseからtrueに代わるたびに実行される。最初からtrueになっている場合も実行される。

使用例:

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

メソッドシグネチャ:

```csharp
Coroutine Watch<T>(Func<T> expression, Action<T> action, ExecutionStage stage = ExecutionStage.Update)
```

このメソッドの最初の引数はboolだけでなくどの値でも返すことができる。expressionの戻り値が前のフレームの戻り値と違う時にactionが実行される。比較はSystem.Collections.Generic.EqualityComparer<T>.Default.Equalsで行われる。

使用例:

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

## 実行のタイミング
UnityのCoroutineは以下のタイミングで実行される:
- Updateが呼び出された後
- LateUpdateが呼び出された後
- FixedUpdateが呼び出された後

デフォルトでは、上記のメソッドはUpdateが実行された後に呼び出される。もし、別のタイミングで呼び出す必要があれば、以下の構文でその指定ができる。

```csharp
this.Watch(() => life, (val) => {
  Debug.Log("Player life changed to: " + val);
}, ExecutionStage.LateUpdate);

this.Watch(() => life, (val) => {
  Debug.Log("Player life changed to: " + val);
}, ExecutionStage.FixedUpdate);
```

すべてのメソッドにこのように実行のタイミングを第３の引数として渡すことができる。しかし、最初の引数のexpressionが最初からtrueなら次のフレームを待たず即座に実行される。

## StartとAwakeに要注意
UnityではGameObjectが無効（SetActive(false)）になると関連しているすべてのCoroutineがキャンセルされる。その結果、もしStartまたはAwakeの中でWatchなどを実行すれば、GameObjectが無効になった場合、また有効になってもactionが呼び出されなくかる。もしこの挙動が好ましくないなら、上記の使用例のようにOnEnableを使用するといい。

## 実行をキャンセルする
While、 When、 WheneverとWatchはすべてCoroutineのインスタンスを返す。Unityの標準的な機能を使えば、実行を止めることができる。

```csharp
Coroutine lifeWatch = this.Watch(() => life, (val) => {
  Debug.Log("Player life changed to: " + val);
});

this.When(() => life <= 0, (val) => {
  Debug.Log("Player is dead.");
  StopCoroutine(lifeWatch);
});
```


