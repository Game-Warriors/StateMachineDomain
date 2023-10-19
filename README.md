# StateMachineDomain
![GitHub](https://img.shields.io/github/license/svermeulen/Extenject)
## Table Of Contents

<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
<summory>

  - [Introduction](#introduction)
  - [Features](#features)
  - [Installation](#installation)
  - [Startup](#startup)
  - [How To Use](#how-to-use)
</summory>

## Introduction
This library provides the simple state machine system which define state and each state has collection of transition to move to another state. each transition has condition to trigger the state changing. this package implemented by C# language and can use in all .NET standard environments.

Support platforms: 
* PC/Mac/Linux
* iOS
* Android
* WebGL
* UWP App

```text
* Note: The library may work on other platforms, the source code just used C# code and .net standard version 2.0.
        this library has some feature which use Threading.Task library and it doesn't supported in WebGL.
```

* This library is design to be dependecy injection friendly, the recommended DI library is the [Dependency Injection](https://github.com/Game-Warriors/DependencyInjection-Unity3d) to be used.

This library used in the following game:
</br>
[Street Cafe](https://play.google.com/store/apps/details?id=com.aredstudio.streetcafe.food.cooking.tycoon.restaurant.idle.game.simulation)

## Features
 

## Installation
This library can be added by unity package manager form git repository or could be downloaded.

For more information about how to install a package by unity package manager, please read the manual in this link:
[Install a package from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html)

## Startup
After adding package to accessing state machine instance main features, the “StateMachineSystem” should be initialize.
```csharp
private void Awake()
{
    IStateMachine stateMachine = new StateMachineSystem();
}
```
If the dependency injection library is used, the initialization process could be like following sample.
```csharp
private void Awake()
{
    serviceCollection.AddTransient<IStateMachine, StateMachineSystem>();
}
```
The “IStateMachine“ abstraction could be used to break the dependency. the “IStateMachine“ is base abstraction for accessing the state machine system features like adding or force changing states.
```csharp
public interface IStateMachine
{
    public event Action<IState,IState> OnStateChanged;
    IState CurrentState { get; }
    void AddState(IState state, params ITransition[] transitions);
    void ForceChangeState(string id);
    void UpdateMachine();
}
```

## How To Use
There are three abstraction which present system features. the “TaskSystem” class Implement all these abstractions.

The state machine system has variant and multiply usage among the project businesses, so may every logic will require its own state machine. each state machine just has one active state. overall, the best practice for state machine is may transient or per scope instance. the state machine library has three main abstractions.

1.  __StateMachineSystem:__ The main Implementation of system, which is States container, update the current state and check its transitions condition to switch to transition target state when the transition condition was met.
    * OnStateChanged: This is an event callback which is triggered when system change states, the first argument state is old state and second is new state.

    * CurrentState: The current state that already update and running in state machine.

    * AddState: Adding new state to state container. the related transitions from this state to another state should pass when adding.

    * ForceChangeState: Force the system to change current state to requested state by state id, regardless of transitions. obviously, the new request state should exist in state container. all related callback on old state and new state will call in this situation.

    * UpdateMachine: The main update loop of system. all system operations and checking will process in this method and the method should called by program core update loop.

2.  __IState:__ The base abstraction of states objects which should have unique Id over system. the state has three callbacks that system will call in specific conditions.

    * Id: The unique id of the state in state machine system.

    * OnEnterState: Calling when the system switch to this state each time.

    * OnStateUpdate: Calling on each system update call when the state is active and is as current state in system.

    * OnExitState: Calling when the system switch from this state to another state.

```csharp
public interface IState
{
    string Id { get; }
    void OnEnterState(IState pastState);
    void OnStateUpdate();
    void OnExitState(IState newState);
}
```

2.  __ITransition:__ The base abstraction of transition objects

    * TargetState: The target state which will move to when condition was met.

    * OnTransitionActivate: Calling each time, when the host state selects as current state, and the state transitions consider in possible transition.

    * OnTransitionDeactivate: Calling each time, when the host state end or exit and current state changed.

    * TransitionUpdate: Calling on each system update call when the transition bind to active state which state is current state in system. the system will apply transition and switching to target state when this method return true.

```csharp
public interface ITransition
{
    IState TargetState { get; }
    void OnTransitionActivate();
    bool TransitionUpdate();
}
```
<h3>Using in code</h3>
Usually a manager class has duty to create and setup and running specific state machine which may call “StateManager“. in following provided a “StateManager“ class and using a sample connection flow by State Machine.

In the beginning the state machine abstraction passed to manager class somehow by some scope managing structure. the manager has duty to first, instantiate and setup the states and transitions. after that the states should bind to the appropriate transition by adding to state machine. finally, manager start state machine process by calling the object update method and by stopping calling this method, state machine process will stoped.

```csharp
public sealed class StateManager
{
    private readonly IEvent _event;
    private readonly IRealTimeService _realTimeService;
    private readonly IStateMachine _stateMachine;

    public StateManager(IStateMachine stateMachine, IEvent @event, IUpdateTask updateTask, IRealTimeService realTimeService)
    {
        _event = @event;
        _realTimeService = realTimeService;
        _updateTask.RegisterUpdateTask(Update);
        _stateMachine = stateMachine;
    }

    public void Initialization(IAppService appService)
    {
        int checkForDataLostIntervalInRunMode = 7;
        int checkConnectionIntervalInDisconnectedMode = 10;
        NetRunningState runningState = new(appService);
        NetDisconnectState disconnectState = new(appService, _realTimeService);
        NetConnectingState connectingState = new(appService, _realTimeService);
        NetDataLostState dataLostState = new(appService);
        RunningSimulationTransition runningSimulationTransition = new(_event, runningState);
        NetworkDisconnectTransition networkDisconnectTransition = new(appService, _realTimeService, disconnectState);
        DataLostDisconnectTransition timeoutDisconnectTransition = new(disconnectState, checkConnectionIntervalInDisconnectedMode);

        _stateMachine.AddState(connectingState, networkDisconnectTransition, timeoutDisconnectTransition, runningSimulationTransition);
        _stateMachine.AddState(runningState, new DataLostTransition(dataLostState, _event, checkForDataLostIntervalInRunMode));
        _stateMachine.AddState(dataLostState, networkDisconnectTransition, timeoutDisconnectTransition, runningSimulationTransition);
        _stateMachine.AddState(disconnectState, new ConnectingTransition(appService, connectingState));
    }

    private void Update()
    {
        _stateMachine.UpdateMachine();
    }
}
```