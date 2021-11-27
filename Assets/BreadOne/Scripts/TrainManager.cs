using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

namespace cngamejam
{

    public class TrainManager : MonoBehaviour
    {
        [SerializeField]
        Player playerRef;

        [SerializeField]
        GameObject trainPrefab;

        [SerializeField]
        float spacingX;

        [SerializeField]
        float yPos;

        [SerializeField]
        int createCount;

        SortedList<int, GameObject> sortedTrain = new SortedList<int, GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            CreateTrain(0);
        }

        // Update is called once per frame
        void Update()
        {
            int currentPlayerIndex = GetPlayerTrainIndex();

            CreateTrain(currentPlayerIndex);

            transform.Translate(-playerRef.MoveValuePerFrame, 0f, 0f); 
            
        }

        int GetPlayerTrainIndex()
        {
            float X = -transform.position.x / spacingX;
            int index = Mathf.RoundToInt(X);

            return index;
        }

        void CreateTrain(int idx)
        {

            int val = (int)(createCount / 2);


            for (int i = idx -val; i <= idx +val; i++)
            {
                if(!sortedTrain.ContainsKey(i))
                {
                    GameObject train = Instantiate(trainPrefab, transform);
                    train.name = $"train_{i}";
                    train.transform.localPosition = new Vector2(i * spacingX, yPos);

                    sortedTrain.Add(i, train);
                }
            }
        }
    }

}