using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(10f, 20f, -10f); // ตำแหน่งมุมสูงเฉียงเล็กน้อย
    [SerializeField] private float smoothSpeed = 100f;                    // ความนุ่มนวลในการติดตาม

    private void LateUpdate()
    {
        if (target == null) return;

        // ตำแหน่งปลายทางที่กล้องต้องไป
        Vector3 targetPosition = target.position + offset;

        // เคลื่อนที่ตามอย่างนุ่มนวลด้วย Lerp
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // ปรับองศากล้องให้ก้มมองที่ตัวละครเสมอ
        transform.LookAt(target.position);
    }
}