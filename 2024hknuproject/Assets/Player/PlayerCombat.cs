using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour, ICombatant
{
    public GameObject projectilePrefab; // 발사체 Prefab
    public float projectileSpeed = 5f; // 발사체 속도
    public int maxProjectilesPerTurn = 6; // 턴당 최대 발사체 수
    private int projectilesFiredThisTurn = 0; // 이번 턴에 발사한 발사체 수
    private int projectilesOnField = 0; // 필드에 존재하는 발사체 수
    public int maxActionPoints = 10; // 최대 행동 포인트
    private int currentActionPoints;
    private LineRenderer lineRenderer;
    private Vector2 aimDirection;
    private bool isTurnComplete = false;
    private List<Ammo> currentLoadedAmmo = new List<Ammo>(); // 현재 장전된 탄약 리스트
    private PlayerTurnManager playerTurnManager;
    private PlayerMovement playerMovement;
    private bool CanCombat = true;
    public Animator animator;

    private Ammo currentAmmo; // 현재 사용 중인 탄약

    public int ProjectileDamage => currentLoadedAmmo.Count > 0 ? currentLoadedAmmo[0].damage : 0;
    // 탄약이 있을 때 첫 번째 탄약의 데미지를 반환

    void Start()
    {
        playerTurnManager = FindObjectOfType<PlayerTurnManager>();
        playerMovement = GetComponent<PlayerMovement>();
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 0; // 초기에는 라인을 그리지 않음
                                        // 선의 색상 설정
        lineRenderer.startColor = Color.red; // 시작 색상
        lineRenderer.endColor = Color.red;   // 끝 색상

        // 선을 그릴 때 사용하는 Material이 필요
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 단순한 색상 적용

        // LineRenderer의 Z축을 통해 2D에서 앞뒤 조정
        lineRenderer.sortingOrder = 2; // 캐릭터보다 위에 그리기 위해 설정

    }

    void Update()
    {
        HUDManager.Instance.UpdateBulletSlots(currentLoadedAmmo.Count);
        if (!CanCombat) // Combat이 불가능할 때는 아무 작업도 하지 않음
            return;

        if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.TurnBased && TurnManager.Instance.CurrentTurnTaker == GetComponent<PlayerTurnManager>())
        {
            if (Input.GetMouseButton(0)) // 마우스 왼쪽 버튼을 클릭했을 때
            {
                playerMovement.DisableMovement();
                ShowAim(); // 조준선 표시
                animator.SetBool("IsAim", true);
                UpdateCharacterDirection(); // 조준 방향에 따른 좌우 반전
            }

            if (Input.GetMouseButtonUp(0)) // 마우스 버튼을 놓았을 때
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = (mousePosition - transform.position).normalized;
                Attack(direction);

                animator.SetBool("IsAim", false);
                playerMovement.EnableMovement();
                lineRenderer.positionCount = 0; // 라인 초기화
            }
        }
        else if (GameModeManager.Instance.currentMode == GameModeManager.GameMode.RealTime)
        {
            if (Input.GetMouseButton(0) && projectilesFiredThisTurn < maxProjectilesPerTurn) // 마우스 왼쪽 버튼을 클릭했을 때
            {
                playerMovement.DisableMovement();
                ShowAim(); // 조준선 표시
                animator.SetBool("IsAim", true);
                UpdateCharacterDirection(); // 조준 방향에 따른 좌우 반전
            }

            if (Input.GetMouseButtonUp(0) && projectilesFiredThisTurn < maxProjectilesPerTurn) // 마우스 버튼을 놓았을 때
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = (mousePosition - transform.position).normalized;
                Attack(direction);

                animator.SetBool("IsAim", false);
                playerMovement.EnableMovement();
                lineRenderer.positionCount = 0; // 라인 초기화
            }
        }
    }

    // 캐릭터 방향 업데이트 함수
    void UpdateCharacterDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 마우스 위치가 캐릭터의 오른쪽에 있으면 오른쪽을, 왼쪽에 있으면 왼쪽을 바라봄
        if (mousePosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(2.3f, 2.3f, 1); // 오른쪽을 바라봄
        }
        else
        {
            transform.localScale = new Vector3(-2.3f, 2.3f, 1); // 왼쪽을 바라봄
        }
    }


    void ShowAim()
    {
        // 항상 플레이어의 위치에서 마우스 위치로 조준
        
        Vector3 playerPosition = transform.position;
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        aimDirection = (mousePosition - playerPosition).normalized;

        // 조준선을 그린다
        lineRenderer.positionCount = 2;


        lineRenderer.SetPosition(0, new Vector3(playerPosition.x, playerPosition.y - 0.05f, -1)); // 캐릭터보다 약간 아래로 설정
        lineRenderer.SetPosition(1, new Vector3(playerPosition.x + aimDirection.x * 10f, playerPosition.y + aimDirection.y * 10f - 0.02f, -1));
        // Z축을 통해 2D 화면에서의 라인 깊이 설정
        lineRenderer.SetPosition(0, new Vector3(lineRenderer.GetPosition(0).x, lineRenderer.GetPosition(0).y, -1));
        lineRenderer.SetPosition(1, new Vector3(lineRenderer.GetPosition(1).x, lineRenderer.GetPosition(1).y, -1));
    }

    public void LogLoadedAmmo()
    {
        Debug.Log("현재 장전된 탄약:");
        for (int i = 0; i < currentLoadedAmmo.Count; i++)
        {
            Debug.Log($"슬롯 {i + 1}: {currentLoadedAmmo[i].itemName}, 수량: {currentLoadedAmmo[i].quantity}");
        }
    }

    public void Attack(Vector2 direction)
    {
        Debug.Log(currentLoadedAmmo.Count);
        if (projectilesFiredThisTurn >= maxProjectilesPerTurn || currentLoadedAmmo.Count == 0) // 발사체 최대치 또는 탄약이 없을 때
            return;

        lineRenderer.positionCount = 0; // 조준선 제거

        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb == null)
        {
            projectileRb = projectile.AddComponent<Rigidbody2D>();
        }
        projectileRb.isKinematic = true; // 발사체는 Kinematic 설정
        projectileRb.velocity = direction * projectileSpeed; // 발사체 속도 설정

        // 발사체와 플레이어 간의 충돌 방지
        Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        // 발사체의 충돌 처리 및 데미지 설정
        ProjectileCollisionHandler collisionHandler = projectile.AddComponent<ProjectileCollisionHandler>();
        collisionHandler.damage = currentLoadedAmmo[0].damage; // 장전된 탄약의 데미지 적용
        collisionHandler.projectileOwner = this;

        projectilesFiredThisTurn++; // 발사한 발사체 수 증가
        projectilesOnField++; // 필드에 존재하는 발사체 수 증가

        currentLoadedAmmo.RemoveAt(0); // 첫 번째 탄약 사용 후 제거
        HUDManager.Instance.UpdateBulletSlots(currentLoadedAmmo.Count);


        StartCoroutine(DestroyProjectileAfterTime(projectile, 5f)); // 5초 후 발사체 제거
    }

    IEnumerator DestroyProjectileAfterTime(GameObject projectile, float time)
    {
        yield return new WaitForSeconds(time);
        if (projectile != null)
        {
            Destroy(projectile);
            NotifyProjectileDestroyed();
        }
    }

    public void NotifyProjectileDestroyed()
    {
        projectilesOnField--; // 필드에 존재하는 발사체 수 감소
    }

    public void ResetProjectilesFired()
    {
        projectilesFiredThisTurn = 0; // 발사체 수 초기화
    }

    public void LoadAmmo(Ammo ammo)
    {
        if (currentLoadedAmmo.Count < maxProjectilesPerTurn)
        {
            Debug.Log($"탄약 장전: {ammo.itemName}");
            currentLoadedAmmo.Add(ammo); // 탄약 장전
            Debug.Log($"현재 장전된 탄약 수: {currentLoadedAmmo.Count}");
        }
        else
        {
            Debug.LogError("약실이 가득 찼습니다! 더 이상 장전할 수 없습니다.");
        }
    }

    public void StartTurn()
    {
        // 턴 시작 시 필요한 로직 추가 가능
    }

    public void EndTurn()
    {
        // 턴 종료 시 필요한 로직 추가 가능
    }

    public int MaxActionPoints => maxActionPoints;
    public int CurrentActionPoints
    {
        get => currentActionPoints;
        set => currentActionPoints = value;
    }

    public int MaxProjectilesPerTurn => maxProjectilesPerTurn;
    public int ProjectilesFiredThisTurn
    {
        get => projectilesFiredThisTurn;
        set => projectilesFiredThisTurn = value;
    }

    public GameObject ProjectilePrefab => projectilePrefab;
    public float ProjectileSpeed => projectileSpeed;

    public int ProjectilesOnField
    {
        get => projectilesOnField;
        set => projectilesOnField = value;
    }

    public void SetLoadedAmmo(List<Ammo> loadedAmmo)
    {
        Debug.Log("SetLoadedAmmo 호출됨");
        currentLoadedAmmo.Clear(); // 기존 탄약 제거
        foreach (Ammo ammo in loadedAmmo)
        {
            // 각 탄약 객체를 복사하여 리스트에 추가
            Ammo clonedAmmo = new Ammo(ammo.itemName, ammo.damage, ammo.effect, ammo.icon, ammo.quantity);
            currentLoadedAmmo.Add(clonedAmmo);
        }
        loadedAmmo.Clear(); // 전달된 리스트 초기화
        projectilesFiredThisTurn = 0; // 발사체 수 초기화
    }
    // 전투 가능 상태로 전환
    public void EnableCombat()
    {
        CanCombat = true;
        Debug.Log("전투 가능 상태로 전환됨");
    }

    // 전투 불가능 상태로 전환
    public void DisableCombat()
    {
        CanCombat = false;
        Debug.Log("전투 불가능 상태로 전환됨");
    }
}
