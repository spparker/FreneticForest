using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGrowth : MonoBehaviour
{
    public enum TreeHealth
    {
        NORMAL,
        ROUGH,
        DISEASED,
    }

    public const float INITIAL_RADIUS = 0.7f;

    float _overgrow_stress = 0.05f;
    float _overgrow_rate = 0.05f;
    float _overgrow_visual_threshold = 0.8f;
    float _stress_visual_threshold = 0.5f;
    float _stress_per_invader = 0.01f;
    float _stress_rate = 0.0001f;
    float _trim_rate = -0.2f;
    float _calm_rate = -0.001f;

    private float _growRate;
    private float _currentAge;
    private TreeHealth _healthState;
    private float _currentSize = 1;
    private int _invaderCount;
    private float _overgrownLevel = 0;
    public CritterCommandControl OccupyingCritters{get; private set;}
    public CritterCommandControl BurrowedCritters{get; private set;}


    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _overgrowRenderer;

    public bool Invaded => _invaderCount > 0;
    public bool Overgrown => _overgrownLevel >= _overgrow_visual_threshold;
    public bool CanEnter => !OccupyingCritters;
    public bool CanBurrow => !BurrowedCritters;

    public bool CanGrow { get; private set; }
    public float StressLevel { get; private set; }
    public float Top => _currentSize;
    public float Radius => _currentSize * INITIAL_RADIUS;
    public float Age => _currentAge;
    public Roots Roots { get; private set; }

    void Start()
    {
        _overgrowRenderer.enabled = false;
        Roots = GetComponentInChildren<Roots>();

        _currentAge = 0f;
        CanGrow = true;
        _growRate = ForestManager.Instance.ForestSettings.treeGrowthRate;
        _healthState = TreeHealth.NORMAL;

        _overgrow_stress = ForestManager.Instance.TreeGrowthData.OvergrowStress;
        _overgrow_rate = ForestManager.Instance.TreeGrowthData.OvergrowRate;
        _overgrow_visual_threshold = ForestManager.Instance.TreeGrowthData.OvergrowVisualThreshold;
        _stress_visual_threshold = ForestManager.Instance.TreeGrowthData.StressVisualThreshold;
        _stress_per_invader = ForestManager.Instance.TreeGrowthData.StressPerInvader;
        _stress_rate = ForestManager.Instance.TreeGrowthData.StressRate;
        _trim_rate = ForestManager.Instance.TreeGrowthData.TrimRate;
        _calm_rate = ForestManager.Instance.TreeGrowthData.CalmRate;
    }

    public void SetRandomSpawnValues(float max_initial_size)
    {
        _currentSize = Random.Range(1.0f, max_initial_size);
        _overgrownLevel = Random.Range(0.0f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        _currentAge += Time.deltaTime;
        
        if(!Overgrown)
        {
            _overgrownLevel = Mathf.Clamp(_overgrownLevel + Time.deltaTime * _overgrow_rate, 0, 1);
            if(Overgrown)
                BecomeOvergrown();
        }

        AddCritterAssistance();
        AddStress(Time.deltaTime);
        UpdateTreeState();

        if(!CanGrow)
            return;

        _currentSize += Time.deltaTime * _growRate;
        transform.localScale = new Vector3(_currentSize, _currentSize, _currentSize);
    }

    public void EnterTree(CritterCommandControl ccc)
    {
        if(ccc.Pod.CritterData.enemy)
            _invaderCount++;
        else if(ccc.Pod.CritterData.type == CritterManager.CritterType.CHOPCHOP)
            OccupyingCritters = ccc;
        else if(ccc.Pod.CritterData.type == CritterManager.CritterType.DIGGIE)
            BurrowedCritters = ccc;
    }

    public void LeaveTree(CritterPod cp)
    {
        if(cp.CritterData.enemy)
            _invaderCount--;
        else if(cp.CritterData.type == CritterManager.CritterType.CHOPCHOP)
            OccupyingCritters = null;
        else if(cp.CritterData.type == CritterManager.CritterType.DIGGIE)
            BurrowedCritters = null;
    }

    public void AddCritterAssistance()
    {
        if(OccupyingCritters)
        {
            _overgrownLevel = Mathf.Clamp(_overgrownLevel + Time.deltaTime * _trim_rate, 0, 1);
            BecomeTrimmed();
        }
        
        if(BurrowedCritters)
        {
            StressLevel = Mathf.Clamp(StressLevel + Time.deltaTime * _calm_rate, 0, 1);
        }
    }

    private void UpdateTreeState()
    {
        if(StressLevel >= 1.0f)
            BecomeDiseased();
        else if(StressLevel >= _stress_visual_threshold)
            BecomeRough();
        else
            BecomeNormal();
    }

    private void BecomeDiseased()
    {
        if(_healthState == TreeHealth.ROUGH)
        {
            _healthState = TreeHealth.DISEASED;
            CanGrow = false;
            _spriteRenderer.sprite = ForestManager.Instance.TreeGrowthData.DiseasedImage;
            // Let the disease travel
            //Die();
        }
    }

    private void Die()
    {
        Debug.Log("DEAD");
        // Tell Manager
        // Kick out Critters
        // Destroy(gameObject);
    }

    private void BecomeRough()
    {
        if(_healthState == TreeHealth.NORMAL && StressLevel < _stress_visual_threshold)
        {
            //Debug.Log("Help Me");
            _healthState = TreeHealth.ROUGH;
            //_spriteRenderer.sprite = ForestManager.Instance.TreeGrowthData.RoughImage;
        }
    }

    private void BecomeOvergrown()
    {
        //Debug.Log("Trim Me");
        //_spriteRenderer.sprite = ForestManager.Instance.TreeGrowthData.OvergrowImage;
        _overgrowRenderer.enabled = true;
    }

    private void BecomeTrimmed()
    {
        _overgrowRenderer.enabled = false;
    }

    private void BecomeNormal()
    {
        if(_healthState == TreeHealth.ROUGH)
        {
            Debug.Log("Whew - Feeling Good!");
            _healthState = TreeHealth.NORMAL;
            _spriteRenderer.sprite = ForestManager.Instance.TreeGrowthData.HealthyImage;
        }
    }


    private void AddStress(float dt)
    {
        float stress_per_second = _stress_per_invader * _invaderCount
                                + _stress_rate;
        if(Overgrown)
            stress_per_second += _overgrow_stress * _overgrownLevel;

        StressLevel += stress_per_second * dt;
    }

    public void SetStartingSize(float size)
    {
        _currentSize = size;
    }

    public void SignalStopGrowth()
    {
        CanGrow = false;
    }
}
