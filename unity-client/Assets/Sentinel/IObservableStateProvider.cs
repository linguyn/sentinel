using System.Collections.Generic;

public interface IObservableStateProvider {
    List<StateEntry> GetState();
}