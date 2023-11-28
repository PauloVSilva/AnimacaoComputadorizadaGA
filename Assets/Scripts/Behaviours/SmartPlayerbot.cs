using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[CreateAssetMenu(menuName = "AIBehaviours/SmartPlayerbot")]
public class SmartPlayerbot : AIBehaviour
{
    public SnakeMovement enemyTarget = null;
    public Transform target = null;
    public float searchTime = 0.0f;

    public override void Init(GameObject own, SnakeMovement ownMove)
    {
        searchTime = 0f;

        base.Init(own, ownMove);
        ownerMovement.StartCoroutine(UpdateDirEveryXSeconds(timeChangeDir));
    }

    //seria interessante ter um controlador com o colisor que define o mundo pra poder gerar pontos dentro desse colisor

    public override void Execute()
    {
        /*
         * if enemy is null, look for enemies within short range
         * if enemy is not null, check their size
         * if enemy is bigger, look for food
         * if enemy is smaller, move towards them
         * if too much time elapses without a target, move randomly
         * also, check if it's about to collide with an enemy and stop
         * after all that, move forward
        */

        if (enemyTarget == null && searchTime < 10f)
        {
            Debug.Log("Enemy is null. Looking for enemy");
            ownerMovement.StartCoroutine(CheckForEnemiesEveryXSeconds(0.5f));
        }
        else
        {
            if (enemyTarget.bodyParts.Count >= ownerMovement.bodyParts.Count)
            {
                Debug.Log("Enemy is bigger. Looking for food");
                ownerMovement.StartCoroutine(CheckForFoodEveryXSeconds(0.5f));
            }
            else
            {
                Debug.Log("Enemy is smaller. Attacking");

                SetTarget(enemyTarget.transform);
            }
        }
        if (target == null && searchTime < 10f)
        {
            Debug.Log("Target is null. Looking for food");
            ownerMovement.StartCoroutine(CheckForFoodEveryXSeconds(0.5f));
        }
        if (target == null && searchTime >= 10f)
        {
            Debug.Log("Too much time has elapsed. Moving randomly");
            ownerMovement.StartCoroutine(UpdateDirEveryXSeconds(timeChangeDir));
        }

        CheckForEnemiesInFront();
        MoveForward();

        searchTime += Time.deltaTime;
    }

    private void CheckForEnemiesInFront()
    {
        Vector2 characterPosition = owner.transform.position;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(characterPosition, 2f);

        foreach (Collider2D collider in colliders)
        {
            if (!collider.transform.IsChildOf(owner.transform.parent) && collider.CompareTag("Body"))
            {
                Debug.Log("Found " + collider.gameObject);

                //SetTarget(owner.transform);
                ownerMovement.Stop();
            }
        }
    }

    //ia basica, move, muda de direcao e move
    private void MoveForward()
    {
        //MouseRotationSnake();
        //owner.transform.position = Vector2.MoveTowards(owner.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), ownerMovement.speed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, ownerMovement.FinalSpeed * Time.deltaTime);

        owner.transform.position = Vector2.MoveTowards(owner.transform.position, randomPoint, ownerMovement.FinalSpeed * Time.deltaTime);
    }

    private void MouseRotationSnake()
    {
        direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - owner.transform.position;
        direction.z = 0.0f;

        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(-angle, Vector3.forward);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, rotation, ownerMovement.speed * Time.deltaTime);
    }

    private void SetTarget(Transform transform)
    {
        target = transform;
        randomPoint = target.position;
        
        direction = randomPoint - owner.transform.position;
        direction.z = 0.0f;
    }

    private IEnumerator UpdateDirEveryXSeconds(float x)
    {
        yield return new WaitForSeconds(x);
        ownerMovement.StopCoroutine(UpdateDirEveryXSeconds(x));

        randomPoint = new Vector3(
                Random.Range(
                    Random.Range(owner.transform.position.x - 10, owner.transform.position.x - 5),
                    Random.Range(owner.transform.position.x + 5, owner.transform.position.x + 10)
                ),
                Random.Range(
                    Random.Range(owner.transform.position.y - 10, owner.transform.position.y - 5),
                    Random.Range(owner.transform.position.y + 5, owner.transform.position.y + 10)
                ),
                0
            );
        direction = randomPoint - owner.transform.position;
        direction.z = 0.0f;

        searchTime = 0.0f;

        ownerMovement.StartCoroutine(UpdateDirEveryXSeconds(x));
    }

    private IEnumerator CheckForFoodEveryXSeconds(float x)
    {
        yield return new WaitForSeconds(x);
        ownerMovement.StopCoroutine(CheckForFoodEveryXSeconds(x));

        Vector2 center = owner.transform.position;
        float radius = 5f;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

        foreach (var collider in hitColliders)
        {
            if (collider.gameObject.GetComponent<OrbBehavior>())
            {
                SetTarget(collider.transform);

                searchTime = 0.0f;
            }
        }
    }

    private IEnumerator CheckForEnemiesEveryXSeconds(float x)
    {
        yield return new WaitForSeconds(x);

        Vector2 center = owner.transform.position;
        float radius = 30f;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

        foreach (var collider in hitColliders)
        {
            if (collider.gameObject != owner.gameObject && collider.gameObject.GetComponent<SnakeMovement>())
            {
                enemyTarget = collider.gameObject.GetComponent<SnakeMovement>();

                searchTime = 0.0f;
            }
        }
    }
}
