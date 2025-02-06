## Documentation

Soon :zap:

> [!NOTE] State instances are not cached and are created each time a new state is transitioned using `IStateFactory`

## Example usage

> [!NOTE] Zenject is used only for resolving the instance of IStateMachine and calling IStateMachine ticks

1. Get `IStateMachine` implementation instance via `Zenject` and move to the state `StateA`

```csharp
    public class ExampleStateMachine : MonoBehaviour
    {
        private IStateMachine m_stateMachine;
    
        [Inject]
        private void Construct(IStateMachine stateMachine)
        {
            m_stateMachine = stateMachine;
        }
    
        private void Start()
        {
            m_stateMachine.Enter<StateA>();
        }
    }
```

2. `StateA.cs`

```csharp
    public class StateA : State
    {
        public override async UniTask Enter()
        {
            Debug.Log("[StateA] Enter");
    
            //Example delay await 
            await UniTask.Delay(TimeSpan.FromSeconds(1));
    
            //Example await UniTasks<T>
            int value = await SomeOperation();
    
            //Direct access to State Machine
            StateMachine.Enter<StateB, StateBModel>(new StateBModel(value));
        }
    
        private UniTask<int> SomeOperation()
        {
            return UniTask.FromResult(Random.Range(0, 100));
        }
    
        public override UniTask Exit()
        {
            Debug.Log("[StateA] Exit");
            return UniTask.CompletedTask;
        }
    }
```

3. `StateBModel.cs`

```csharp
    public class StateBModel
    {
        public int Value { get; private set; }
    
        public StateBModel(int value)
        {
            Value = value;
        }
    
        public void Increase()
        {
            Value += 1;
        }
    }
```

4. `StateB.cs`

```csharp
    public class StateB : ModelState<StateBModel>
    {
        public override  async UniTask Enter()
        {
            //Get access to state model via Model property 
            Debug.Log($"[StateB] Enter with Model value: {Model.Value}");
    
            await UniTask.Delay(TimeSpan.FromSeconds(1));
    
            Debug.Log($"[StateB] Current Model value after 1s: {Model.Value}");
        }
    
        public override void Tick(float delta)
        {
            //Modify model every Tick like MonoBehaviour.Update
            
            Model.Increase();
        }
    }
```

---

## State Machine API Reference

### 1. `IStateMachine.cs`

```csharp
    public interface IStateMachine
    {
        ExitableState CurrentState { get; }
        UniTaskVoid Enter<TState>() where TState : State;
        UniTaskVoid Enter<TState, TModel>(TModel model) where TState : ModelState<TModel>;
    }
```

### 2. `IStateFactory.cs`

```csharp
    public interface IStateFactory
    {
        object Create(Type type);
    }
```

### 3. `IStateMachineEvents.cs`

```csharp
    public interface IStateMachineEvents
    {
        //Where Type = new state
        event Action<Type> StateChanged;
        
        //There first Type = old state, and second Type = new state
        event Action<Type, Type> StateChangedFrom;
    }
```

### 4. `IStateMachineTicks.cs`

```csharp
    public interface IStateMachineTicks
    {
        void Tick(float delta);
        void FixedTick(float delta);
    }
```

## State API Reference

### 1. `ExitableState.cs`

```csharp
    public abstract class ExitableState : IState
    {
        //Called when exiting a state
        public virtual UniTask Exit();
        
        //Calling manually
        public virtual void Tick(float delta);
        
        //Calling manually Fixed variant
        public virtual void FixedTick(float delta);
        
        //Reference to StateMachine
        public IStateMachine StateMachine{ get; set; }
        
        //CancellationTokenSources are managed by StateMachine
        internal CancellationTokenSource StateLifetimeTokenSource { set; get; }
        
        //.Cancel() called when exiting a state
        protected CancellationToken LifetimeToken { get; }
    }
```

### 2. `State.cs`

```csharp
    public abstract class State : ExitableState
    {
        //Enter the state
        public abstract UniTask Enter();
    }
```

### 3. `ModelState.cs`

```csharp
    public abstract class ModelState<TModel> : State
    {
        //Model required by State, initialized by StateMachine before state.Enter()
        public TModel Model { internal set; get; }
    }
```

---

## Integration with Zenject

1. `IStateFactory` example implementation

```csharp 
    public class ZenjectStateFactory : IStateFactory
    {
        private readonly IInstantiator m_instantiator;

        public ZenjectStateFactory(IInstantiator instantiator)
        {
            m_instantiator = instantiator;
        }

        public object Create(Type type)
        {
            return m_instantiator.Instantiate(type);
        }
    }
```

2. `IStateMachineTicks` `.Tick()` and `.FixedTick()` calling by custom class

```csharp
    public class ZenjectStateMachineTicks : ITickable, IFixedTickable
    {
        private readonly IStateMachineTicks m_ticks;
    
        public ZenjectStateMachineTicks(IStateMachineTicks ticks)
        {
            m_ticks = ticks;
        }
    
        public void Tick()
        {
            m_ticks.Tick(Time.deltaTime);
        }
    
        public void FixedTick()
        {
            m_ticks.FixedTick(Time.fixedDeltaTime);
        }
    }
```

3. Binding to `Container`

```csharp
    Container.BindInterfacesTo<AsyncStateMachine>().AsSingle();
    Container.BindInterfacesTo<ZenjectStateFactory>().AsSingle();
    Container.BindInterfacesTo<ZenjectStateMachineTicks>().AsSingle();
```

---

# ToDo

-[ ] Cleanup repeated parts inside `StateMachine.cs` `[DRY]`
