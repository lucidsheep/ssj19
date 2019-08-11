using UnityEngine.Events;

public class OnButtonDown : UnityEvent<Controller.Command> { }
public class TimeTickEvent : UnityEvent<float> { }
public class FoodGatheredEvent : UnityEvent<int, int> { }
public class EvolutionEvent : UnityEvent<Evolution> { }
public class GatheringPhaseStart : UnityEvent { }
public class MatingPhaseStart : UnityEvent { }