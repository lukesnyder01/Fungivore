using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailManager : MonoBehaviour
{
    public bool generateSegments = true;

    public GameObject segmentPrefab;
    public Transform tailStart;

    public int numberOfSegments = 10;

    public float spawnSeparation = 0.4f;

    public List<TailSegment> tailSegments = new List<TailSegment>();


    // Start is called before the first frame update
    void Start()
    {
        if (generateSegments)
        {
            GenerateSegments();
        }
        else
        {
         // unparent tail segments
        }
    }

    private void GenerateSegments()
    {
        for (int i = 0; i < numberOfSegments; i++)
        {
            var targetLocation = transform.position + new Vector3(0, 0, spawnSeparation * i + spawnSeparation);
            var prefab = Instantiate(segmentPrefab, targetLocation, Quaternion.identity);
            var segment = prefab.GetComponent<TailSegment>();

            tailSegments.Add(segment);

            //tailSegmentPrefabs.Add(prefab);



            segment.listPosition = i;
            segment.tailManager = this;

            if (i == 0)
            {
                segment.followTransform = tailStart;
                segment.followDistance = 0.5f;

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

}
