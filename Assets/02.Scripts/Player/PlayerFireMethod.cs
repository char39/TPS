using System.Collections;
using UnityEngine;

public partial class Player
{
/*     private bool Check_isFireAuto()
    {
        RaycastHit hit;
        RaycastHit obstacleHit;
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 25f, 1 << enemyLayer))      // Raycast가 적에 닿았을 때
        {
            if (Physics.Raycast(firePos.position, (hit.point - firePos.position).normalized, out obstacleHit, Vector3.Distance(firePos.position, hit.point)))   // Raycast가 장애물에 충돌했을 때
            {
                if (obstacleHit.collider.gameObject.layer != enemyLayer) return false;
                else return true;
            }
            else return true;
        }
        else return false;
    } */
    private void PlayerGunFire()
    {
        if (Input.GetMouseButtonUp(0) || !isFire || isRun)
            muzzFlash.Stop();
        if (Input.GetMouseButton(0) && !isFire && !isRun)
        {
            if (currentBullet <= 0 && !isReload)
                StartCoroutine(Reload());
            else if (currentBullet > 0 && !isReload)
                StartCoroutine(Fire());
        }
    }
/*     private void PLayerGunFireAuto()
    {
        if (!isFire || isRun)
            muzzFlash.Stop();
        if (isFireAuto && !isFire && !isRun)
        {
            if (currentBullet <= 0 && !isReload)
                StartCoroutine(ReloadAuto());
            else if (currentBullet > 0 && !isReload)
                StartCoroutine(FireAuto());
        }
    } */

    IEnumerator Fire()
    {
        currentBullet--;
        isFire = true;
        muzzFlash.Play();
        LazerBeam.instance.PlayerLazerBeam();
        RaycastHit hit; // 광선이 오브젝트에 충돌할 경우 충돌 지점이나 거리 등을 알려주는 광선 구조체
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f))    // 광선을 쐈을 때 반경 안에서 맞았는지 여부
        {
            if (hit.collider.gameObject.tag == enemyTag || hit.collider.gameObject.tag == enemyTag2)
            {
                //Debug.Log("적 hit");
                object[] paramsObj = new object[2];
                paramsObj[0] = hit.point;       // 첫 번째 배열에 맞은 위치값을 전달
                paramsObj[1] = 25f;             // 두 번째 배열에 총알 데미지값을 전달
                hit.collider.gameObject.SendMessage("OnDamage", paramsObj, SendMessageOptions.DontRequireReceiver); // public이 아니어도 호출 가능
            }
            if (hit.collider.gameObject.tag == barrelTag)
            {
                //Debug.Log("배럴 hit");
                object[] paramsObj = new object[3];
                paramsObj[0] = 1;
                paramsObj[1] = firePos.position;
                paramsObj[2] = hit.point;
                hit.collider.gameObject.SendMessage("OnDamage", paramsObj, SendMessageOptions.DontRequireReceiver);
            }
            if (hit.collider.gameObject)
            {
                //Debug.Log("벽, 바닥 hit");
                object[] paramsObjs = new object[1];
                paramsObjs[0] = hit.point;
                hit.collider.gameObject.SendMessage("BulletHitEffect", paramsObjs, SendMessageOptions.DontRequireReceiver);
            }
        }
        SoundManager.Instance.PlaySound(firePos.position, playerSound.fire[(int)weaponType]);
        yield return new WaitForSeconds(0.1f);
        UpdateBulletText();
        isFire = false;
    }
    IEnumerator Reload()
    {
        isReload = true;
        SoundManager.Instance.PlaySound(tr.position, playerSound.reload[(int)weaponType]);
        yield return new WaitForSeconds(playerSound.reload[(int)weaponType].length + 0.3f);
        currentBullet = maxBullet;
        magazineImage.fillAmount = 1.0f;
        UpdateBulletText();
        isReload = false;
    }

/*     IEnumerator FireAuto()
    {
        currentBullet--;
        isFire = true;
        muzzFlash.Play();
        LazerBeam.instance.PlayerLazerBeam();
        RaycastHit hit; // 광선이 오브젝트에 충돌할 경우 충돌 지점이나 거리 등을 알려주는 광선 구조체
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f))    // 광선을 쐈을 때 반경 안에서 맞았는지 여부
        {
            if (hit.collider.gameObject.tag == enemyTag || hit.collider.gameObject.tag == enemyTag2)
            {
                //Debug.Log("적 hit");
                object[] paramsObj = new object[2];
                paramsObj[0] = hit.point;       // 첫 번째 배열에 맞은 위치값을 전달
                paramsObj[1] = 25f;             // 두 번째 배열에 총알 데미지값을 전달
                hit.collider.gameObject.SendMessage("OnDamage", paramsObj, SendMessageOptions.DontRequireReceiver); // public이 아니어도 호출 가능
            }
            if (hit.collider.gameObject.tag == barrelTag)
            {
                //Debug.Log("배럴 hit");
                object[] paramsObj = new object[3];
                paramsObj[0] = 1;
                paramsObj[1] = firePos.position;
                paramsObj[2] = hit.point;
                hit.collider.gameObject.SendMessage("OnDamage", paramsObj, SendMessageOptions.DontRequireReceiver);
            }
            if (hit.collider.gameObject)
            {
                //Debug.Log("벽, 바닥 hit");
                object[] paramsObjs = new object[1];
                paramsObjs[0] = hit.point;
                hit.collider.gameObject.SendMessage("BulletHitEffect", paramsObjs, SendMessageOptions.DontRequireReceiver);
            }
        }
        SoundManager.Instance.PlaySound(firePos.position, playerSound.fire[(int)weaponType]);
        yield return new WaitForSeconds(fireRate);
        UpdateBulletText();
        isFire = false;
    } */
/*     IEnumerator ReloadAuto()
    {
        isReload = true;
        SoundManager.Instance.PlaySound(tr.position, playerSound.reload[(int)weaponType]);
        yield return new WaitForSeconds(playerSound.reload[(int)weaponType].length + 0.3f);
        currentBullet = maxBullet;
        magazineImage.fillAmount = 1.0f;
        UpdateBulletText();
        isReload = false;
    } */

    private void UpdateBulletText() // 총알 발사할 때마다 UI 왼쪽상단 탄창 총알 개수 갱신
    {
        magazineImage.fillAmount = (float)currentBullet / (float)maxBullet;
        magazineText.text = string.Format($"<color=#FFAAAA>{currentBullet}</color> / {maxBullet}");
    }
    public void OnChangeWeapon()
    {
        //if (GameManager.instance.isPaused)  return;
        weaponType = (WeaponType)((int)++weaponType % 2);   // enum 순서 변경
        weaponImage.sprite = weaponIcons[(int)weaponType];  // weaponImage에 enum index 값과 동일한 weaponIcons sprite를 할당
    }
}