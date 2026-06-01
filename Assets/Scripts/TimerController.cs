using System.Collections;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    public delegate void TimerExpired();
    public event TimerExpired OnTimerExpired;

    public delegate void TimerTick(int currentValue);
    public event TimerTick OnTimerTick;

    private const int K_TIMER_DURATION = 40;

    private Coroutine m_counterCoroutine;
    private bool m_isPaused;
    private int m_currentCounter;

    public int CurrentCounter => m_currentCounter;

    private void Awake()
    {
        m_currentCounter = K_TIMER_DURATION;
    }

    public void StartTimer()
    {
        if (m_counterCoroutine != null)
            return;

        m_counterCoroutine = StartCoroutine(UpdateCounter());
    }

    public void StopTimer()
    {
        if (m_counterCoroutine == null)
            return;

        StopCoroutine(m_counterCoroutine);
        m_counterCoroutine = null;
    }

    public void ResetTimer()
    {
        m_currentCounter = K_TIMER_DURATION;
        OnTimerTick?.Invoke(m_currentCounter);
    }

    public void SetPaused(bool isPaused)
    {
        m_isPaused = isPaused;
    }

    private IEnumerator UpdateCounter()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1f);

            if (m_isPaused) continue;

            m_currentCounter--;

            if (m_currentCounter <= 0)
            {
                m_currentCounter = 0;
                OnTimerTick?.Invoke(m_currentCounter);
                OnTimerExpired?.Invoke();
                yield break;
            }

            OnTimerTick?.Invoke(m_currentCounter);
        }
    }
}