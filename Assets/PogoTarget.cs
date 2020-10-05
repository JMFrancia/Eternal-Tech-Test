using UnityEngine;

/*
 * PogoTarget component just set up to ensure pogo target pointed correctly and spins
 */
public class PogoTarget : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;

    private void Awake()
    {
        //Point up!
        Vector3 worldPos = GameObject.FindGameObjectWithTag("World").GetComponent<CenterOfMass>().transform.position;
        transform.LookAt(worldPos - transform.position);
        transform.Rotate(90f, 0f, 0f);
    }

    private void Update()
    {
        transform.Rotate(0f, rotationSpeed, 0f);
    }
}
