using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    None, Login, InSession,
}
public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private GameState gameState = GameState.None;
    public GameState GameState { set { gameState = value; } get { return gameState; } }

    public List<BaseManager> baseManagers = new List<BaseManager>();
    public List<BaseSystem> baseSystems = new List<BaseSystem>();

    public void AddSystem(BaseSystem system)
    {
        baseSystems.Add(system);
    }
    public void RemoveSystem(BaseSystem system)
    {
        baseSystems.Remove(system);
    }

    public T GetSystem<T>() where T : BaseSystem
    {
        foreach (var system in baseSystems)
        {
            if (system is T)
            {
                return system as T;
            }
        }
        return default;
    }

    public T GetManager<T>() where T : BaseManager
    {
        foreach (var manager in baseManagers)
        {
            if (manager is T)
            {
                return manager as T;
            }
        }
        return default;
    }
}
