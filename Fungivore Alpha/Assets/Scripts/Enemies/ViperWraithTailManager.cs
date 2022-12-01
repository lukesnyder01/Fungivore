using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViperWraithTailManager : MonoBehaviour
{
    public GameObject segmentPrefab;
    public Transform tailStart;

    public int numberOfSegments = 10;

    public float spawnSeparation = 0.4f;

    public List<GameObject> tailSegmentPrefabs = new List<GameObject>();
    public List<ViperWraithBodySegment> tailSegments = new List<ViperWraithBodySegment>();


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfSegments; i++)
        {
            var targetLocation = transform.position - new Vector3(0, 0, spawnSeparation * i + spawnSeparation);
            var prefab = Instantiate(segmentPrefab, targetLocation, Quaternion.identity);
            var segment = prefab.GetComponent<ViperWraithBodySegment>();

            tailSegments.Add(segment);

            //tailSegmentPrefabs.Add(prefab);

            

            segment.listPosition = i;
            segment.tailManager = this;

            if (i == 0)
            {
                segment.followTransform = tailStart;
                segment.minFollowDistance = 0.5f;
                
            }
            else
            {
                segment.followTransform = tailSegments[i - 1].gameObject.transform;
            }
        }

    }


    public void RemoveFromList(int segmentIndex)
    {
        if (segmentIndex == 0)
        {
            tailSegments[segmentIndex + 1].followTransform = tailStart;
            tailSegments.RemoveAt(segmentIndex);
            
        }
        else if (segmentIndex < tailSegments.Count - 1)
        {
            tailSegments[segmentIndex + 1].followTransform = tailSegments[segmentIndex].followTransform;
            tailSegments.RemoveAt(segmentIndex);
        }
        else
        {
            tailSegments.RemoveAt(segmentIndex);
        }

        UpdateListPositions(segmentIndex);
    }


    void UpdateListPositions(int startIndex)
    {
        for (int i = startIndex; i < tailSegments.Count; i++)
        {
            tailSegments[i].listPosition -= 1;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
