using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> mapSegments; // 타일맵 세그먼트 프리팹 리스트
    [SerializeField] private GameObject carrotPrefab; // 타일맵 세그먼트 프리팹 리스트
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float segmentLength = 1.0f; // 각 세그먼트의 길이
    [SerializeField] private float carrotSpawnChance = 0.1f; // 각 세그먼트의 길이
    
    private List<GameObject> _activeSegments = new List<GameObject>();
    private float _spawnX = -1.5f;
    private int _initSegmentsOnScreen = 10;
    private int _nextSquare = 0; // [0 : Green, 1 : Orange]

    private Vector3 _startPos = new Vector3(-1.5f, 0, 0);

    private void Awake()
    {
        playerTransform.position = new Vector3(-1.5f, 1f, 0);
    }

    void Start()
    {
        for (int i = 0; i < _initSegmentsOnScreen; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if ( playerTransform.position.x > _spawnX -(segmentLength * 3) - segmentLength + 1.0f)
        {
            SpawnSegment();
            DeleteSegment();
        }
        gameManager.score = (int)(playerTransform.position.x - _startPos.x) * 10 + (gameManager.carrotCnt * 100);
    }

    private void SpawnSegment()
    {
        GameObject segment = Instantiate(mapSegments[_nextSquare]);
        _nextSquare = (_nextSquare + 1) % 2;
        
        Vector3 tempScale = segment.transform.localScale;
        tempScale.x = Random.Range(0.5f, 1.5f);
        segment.transform.localScale = tempScale; // x 길이 랜덤
        
        _spawnX += tempScale.x * 0.5f;
        segment.transform.position = Vector3.right * _spawnX;
        _spawnX += segment.transform.localScale.x * 0.5f;
        segment.transform.SetParent(transform);
        _activeSegments.Add(segment);

        if (Random.value < carrotSpawnChance)
        {
            SpawnCarrotNearSegment(segment);
        }
    }

    private void DeleteSegment()
    {
        Destroy(_activeSegments[0]);
        _activeSegments.RemoveAt(0);
    }
    
    private void SpawnCarrotNearSegment(GameObject segment)
    {
        Vector3 carrotPosition = new Vector3(_spawnX, 3f, 0); // 당근의 위치 설정
        GameObject carrot = Instantiate(carrotPrefab, carrotPosition, Quaternion.identity);
        carrot.transform.SetParent(segment.transform);
    }
}