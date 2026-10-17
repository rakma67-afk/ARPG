using UnityEngine;
using UnityEngine.VFX;

public class VFXScript : MonoBehaviour
{
    [Header("Assign VFX Graph")]
    public VisualEffect slashVFXNormal; // ลากตัว VFX Graph มาใส่ในช่องนี้
    public VisualEffect slashVFXS; // ลากตัว VFX Graph มาใส่ในช่องนี้
    public VisualEffect slashVFXD; // ลากตัว VFX Graph มาใส่ในช่องนี้
    public VisualEffect slashVFXD1; // ลากตัว VFX Graph มาใส่ในช่องนี้


    // ฟังก์ชันสำหรับเรียกใช้งานผ่าน Animation Event
    public void PlaySlash()
    {
        if (slashVFXNormal != null)
        {
            slashVFXNormal.SendEvent("OnPlay"); // สั่งให้ VFX เล่นเอฟเฟกต์ 1 ครั้ง
        }
    }

    public void PlaySlashS()
    {
        if (slashVFXS != null)
        {
            slashVFXS.SendEvent("OnPlay"); // สั่งให้ VFX เล่นเอฟเฟกต์ 1 ครั้ง
        }
    }

    public void PlaySlashD()
    {
        if (slashVFXD != null)
        {
            slashVFXD.SendEvent("OnPlay"); // สั่งให้ VFX เล่นเอฟเฟกต์ 1 ครั้ง
            slashVFXD1.SendEvent("OnPlay"); // สั่งให้ VFX เล่นเอฟเฟกต์ 1 ครั้ง
        }
    }
}