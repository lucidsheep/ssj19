using UnityEngine.Events;

public class OnButtonDown : UnityEvent<Controller.Command> { }
public class TimeTickEvent : UnityEvent<float> { }
public class FoodGatheredEvent : UnityEvent<int, int> { }
public class EvolutionEvent : UnityEvent<Evolution> { }
public class GatheringPhaseStart : UnityEvent { }
public class MatingPhaseStart : UnityEvent { }
public class HPChangeEvent : UnityEvent<int> { }
public class HPUpdateEvent : UnityEvent<IntRange> { }
public class SPChangeEvent : UnityEvent<int> { }
public class SPUpdateEvent : UnityEvent<IntRange> { }
public class InteractionEvent : UnityEvent<Creature> { }
public class HPDepletedEvent : UnityEvent { }
public class InvincibilityChangeEvent : UnityEvent<bool> { }
public class BiomeChangeEvent : UnityEvent<Biome> { }