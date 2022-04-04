using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Motion : MonoBehaviour{
    
    public EventSystem eventSystem;

    [SerializeField] float feedValue=0.1f;

    public IEnumerator CardPotionMotion(Transform transform, float time, Vector3? position) {
        
        eventSystem.enabled=false;

        var currentPosition = transform.position;

        var targetPosition = position ?? currentPosition;

        var sumTime = 0f;
        while (true) {
            // Coroutine開始フレームから何秒経過したか
            sumTime += Time.deltaTime;
            // 指定された時間に対して経過した時間の割合
            var ratio = sumTime / time;

            transform.position = Vector3.Lerp(currentPosition, targetPosition, ratio);

            if (ratio > 1.0f) {
                // 目標の値に到達したらこのCoroutineを終了する
                // ~.Lerpは割合を示す引数は0 ~ 1の間にClampされるので1より大きくても問題なし
                break;
            }

            yield return null;
        }
        eventSystem.enabled=true;
    }

    public IEnumerator CardRotationMotion(Transform transform, float time, Quaternion? rotation){
        eventSystem.enabled=false;

        var currentRotation = transform.localRotation;

        var targetRotation = rotation ?? currentRotation;

        var sumTime = 0f;
        while (true) {
            // Coroutine開始フレームから何秒経過したか
            sumTime += Time.deltaTime;
            // 指定された時間に対して経過した時間の割合
            var ratio = sumTime / time;

            transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, ratio);

            if (ratio > 1.0f) {
                // 目標の値に到達したらこのCoroutineを終了する
                // ~.Lerpは割合を示す引数は0 ~ 1の間にClampされるので1より大きくても問題なし
                break;
            }

            yield return null;
        }
        eventSystem.enabled=true;
    }   
    public IEnumerator DisplayFeedIn(Transform transform){
        while(transform.localScale.x<1&&transform.localScale.y<1){
            transform.localScale=transform.localScale+new Vector3(feedValue,feedValue,0);
            yield return null;
        }
    }
    public IEnumerator DisplayFeedOut(Transform transform){
        while(transform.localScale.x>0&&transform.localScale.y>0){
            transform.localScale=transform.localScale-new Vector3(feedValue,feedValue,0);
            yield return null;
        }
    }
}
