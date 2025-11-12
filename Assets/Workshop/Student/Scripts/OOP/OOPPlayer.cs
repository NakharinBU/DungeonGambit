using System;
using System.Collections;
using UnityEngine;

namespace Solution
{

    public class OOPPlayer : OldCharacter
    {
        public Inventory inventory;
        public ActionHistoryManager actionHistoryManager;
        public TileHighlighter tileHighlighter;

        public bool isAutoMoving = false; // Flag to control auto-movement

        public override void SetUP()
        {
            tileHighlighter = FindFirstObjectByType<TileHighlighter>();
            base.SetUP();
            PrintInfo();
            GetRemainEnergy();
            inventory = GetComponent<Inventory>();
            // Initialize the action history manager and save the starting position

        }

        public void Update()
        {
            if (!isAutoMoving)
            {
                // อัปเดตช่อง highlight รอบตัวทุกเฟรม
                tileHighlighter.ShowHighlights(new Vector2Int(positionX, positionY), mapGenerator);

                // คลิกเพื่อเดิน
                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    int gridX = Mathf.RoundToInt(mouseWorldPos.x);
                    int gridY = Mathf.RoundToInt(mouseWorldPos.y);
                    Vector2Int target = new Vector2Int(gridX, gridY);
                    Vector2Int playerPos = new Vector2Int(positionX, positionY);
                    Vector2Int delta = target - playerPos;

                    // ตรวจว่าช่องอยู่รอบตัว (8 ทิศทาง)
                    bool isAdjacent = Mathf.Abs(delta.x) <= 1 && Mathf.Abs(delta.y) <= 1 && (delta != Vector2Int.zero);
                    if (!isAdjacent) return;

                    // ตรวจว่าว่าง
                    if (mapGenerator.GetMapData(gridX, gridY) == null)
                    {
                        Move(delta);
                    }
                }

                // ปุ่มสำรอง
                if (Input.GetKeyDown(KeyCode.W)) Move(Vector2.up);
                if (Input.GetKeyDown(KeyCode.S)) Move(Vector2.down);
                if (Input.GetKeyDown(KeyCode.A)) Move(Vector2.left);
                if (Input.GetKeyDown(KeyCode.D)) Move(Vector2.right);
                if (Input.GetKeyDown(KeyCode.Q)) Move(new Vector2(1, 1));   // ทดสอบเฉียง
            }
        }


        public override void Move(Vector2 direction)
        {
            int targetX = positionX + (int)direction.x;
            int targetY = positionY + (int)direction.y;

            if (targetX < 0 || targetX >= mapGenerator.X || targetY < 0 || targetY >= mapGenerator.Y)
                return;

            if (mapGenerator.mapdata[targetX, targetY] != null)
                return;

            // อัปเดต mapdata
            mapGenerator.mapdata[positionX, positionY] = null;
            mapGenerator.mapdata[targetX, targetY] = this;

            positionX = targetX;
            positionY = targetY;

            // เดินแบบนุ่ม
            StartCoroutine(MoveSmoothly(new Vector3(positionX, positionY, -0.1f)));

            // สั่งศัตรูขยับ
            mapGenerator.MoveEnemies();
        }

        IEnumerator MoveSmoothly(Vector3 target)
        {
            Vector3 start = transform.position;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 5f; // ปรับความเร็วได้
                transform.position = Vector3.Lerp(start, target, t);
                yield return null;
            }
        }


        public void UseFireStorm()
        {
            if (inventory.HasItem("FireStorm",1))
            {
                inventory.UseItem("FireStorm",1);
                OOPEnemy[] enemies = UtilitySortEnemies.SortEnemiesByRemainningEnergy1(mapGenerator);
                int count = 3;
                if (count > enemies.Length)
                {
                    count = enemies.Length;
                }
                for (int i = 0; i < count; i++)
                {
                    enemies[i].TakeDamage(10);
                }
            }
            else
            {
                Debug.Log("No FireStorm in inventory");
            }
        }
        
        public void Attack(OOPEnemy _enemy)
        {
            _enemy.TakeDamage(AttackPoint);
            Debug.Log(_enemy.name + " is energy " + _enemy.energy);
        }
        protected override void CheckDead()
        {
            base.CheckDead();
            if (energy <= 0)
            {
                Debug.Log("Player is Dead");
            }
        }

        // ตรวจสอบการคลิกตำแหน่งไหนในแมพ
        public Vector2Int GetGridPositionFromMouse()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int gridX = Mathf.RoundToInt(mouseWorldPos.x);
            int gridY = Mathf.RoundToInt(mouseWorldPos.y);
            return new Vector2Int(gridX, gridY);
        }

    }

}