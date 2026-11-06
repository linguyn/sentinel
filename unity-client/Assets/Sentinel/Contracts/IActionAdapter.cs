public interface IActionAdapter {
     string ActionName { get; }

     void Begin(ActionResponse request, ActionTracker tracker);

     void Tick(float deltaTime);

     void Cancel(); 

}