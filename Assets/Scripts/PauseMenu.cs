using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    [SerializeField] private GameObject pauseGUI = default;
    [SerializeField] private Sound menuWhoosh = default;
    [SerializeField] private Sound menuClick = default;
    [SerializeField] private Sound menuHit = default;

    [SerializeField] private AudioMixerSnapshot pausedSnapshot = default;
    [SerializeField] private AudioMixerSnapshot unpausedSnapshot = default;

    private SoundInstance _menuWhoosh;
    private SoundInstance _menuClick;
    private SoundInstance _menuHit;
    private AudioManager _audioManager;

    private const float ClickFreq = 0.15f;
    private ReloadScene _reloadScene;
    private float _timeSinceClick = 0f;
    private bool _paused = false;
    private bool wasShooting;

    private void Start()
    {
        _menuWhoosh = menuWhoosh.GenerateInstance();
        _menuHit = menuHit.GenerateInstance();
        _menuClick = menuClick.GenerateInstance();
        _audioManager = AudioManager.Instance();

        _reloadScene = FindObjectOfType<ReloadScene>();
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Pause();
        _timeSinceClick += Time.unscaledDeltaTime;
    }

    private void Resume()
    {
        _audioManager.PlaySound(_menuWhoosh);
        unpausedSnapshot.TransitionTo(0.01f);
        PlayerManager.instance.stopShooting = wasShooting;
    }

    public void Pause()
    {

        if (_paused)
        {
            Resume();
        }
        else
        {
            _audioManager.PlaySound(_menuWhoosh);
            pausedSnapshot.TransitionTo(0.01f);
            wasShooting = PlayerManager.instance.stopShooting;
            PlayerManager.instance.stopShooting = true;
        }

        _paused = !_paused;
        Cursor.lockState = (_paused ? CursorLockMode.None : CursorLockMode.Locked);
        Time.timeScale = 1 * (_paused ? 0 : 1);
        pauseGUI.SetActive(_paused);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        Resume();
        _reloadScene.ReloadTheScene();
    }

    public void Quit()
    {
        _audioManager.StopSound(MusicTrigger._currentPlaying);
        MusicTrigger._currentPlaying = null;
        Resume();
        SceneManager.LoadScene("MainMenu");
    }

    public void PlaySliderClick()
    {
        if (_timeSinceClick > ClickFreq)
        {
            _audioManager.PlaySound(_menuClick);
            _timeSinceClick = 0f;
        }
    }

    public void PlayHoverSound()
    {
        _audioManager.PlaySound(_menuHit);
    }
}
