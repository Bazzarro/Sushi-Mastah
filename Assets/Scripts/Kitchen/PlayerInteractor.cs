using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1.8f;

    private readonly Collider[] nearby = new Collider[32];
    private PlayerInventory inventory;
    private IInteractable current;

    public string CurrentPrompt => current != null ? current.GetPrompt(inventory) : string.Empty;
    public bool HasTarget => current != null;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        current = FindClosestInteractable();

        if (Input.GetKeyDown(KeyCode.Q))
            inventory.DiscardLast();

        if (current != null && Input.GetKeyDown(KeyCode.E))
            current.Interact(inventory);
    }

    private IInteractable FindClosestInteractable()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, interactionRadius, nearby);
        float bestDistance = float.MaxValue;
        IInteractable best = null;

        for (int i = 0; i < count; i++)
        {
            MonoBehaviour[] behaviours = nearby[i].GetComponentsInParent<MonoBehaviour>(true);
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (!(behaviour is IInteractable interactable))
                    continue;

                float distance = (nearby[i].ClosestPoint(transform.position) - transform.position).sqrMagnitude;
                if (distance >= bestDistance)
                    continue;

                bestDistance = distance;
                best = interactable;
            }
        }

        return best;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 1f, 0.7f, 0.45f);
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
