using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapZone
{
    public string zoneName; // 구역 이름
    public GameObject zoneTrigger; // 구역을 나타내는 트리거 오브젝트
    public Vector2 spawnAreaMin; // NPC 스폰 범위의 최소 좌표
    public Vector2 spawnAreaMax; // NPC 스폰 범위의 최대 좌표
}
