using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCChecker : MonoBehaviour
{
    private int npcCount; // 현재 게임에 존재하는 NPC의 수

    void Update()
    {
        CheckNPCCount(); // 매 프레임마다 NPC 수를 확인
    }

    // NPC 태그가 붙은 오브젝트의 수를 확인하는 함수
    void CheckNPCCount()
    {
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC"); // "NPC" 태그가 붙은 오브젝트를 배열로 가져옴
        npcCount = npcs.Length; // 배열의 길이로 NPC의 개수를 설정
        Debug.Log("Current NPC count: " + npcCount); // 콘솔에 NPC의 개수를 출력
    }

    // NPC의 개수를 반환하는 함수
    public int GetNPCCount()
    {
        return npcCount; // NPC의 수를 반환
    }
}
