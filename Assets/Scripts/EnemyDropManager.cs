using UnityEngine;

public class EnemyDropManager : MonoBehaviour
{
    #region Singleton
    public static EnemyDropManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion
    public GameObject[] drops;

    public GameObject DropRandomDrop(float percentChance)
    {
        float rand = Random.value;
        if(rand <= percentChance)
        {
            foreach (GameObject drop in drops)
            {
                rand = Random.value;

                if(rand <= .4f)
                {
                    return drop;
                }
            }
        }

        return null;
    }
}
