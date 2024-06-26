using UnityEngine;

/// <summary>
/// GameObjectの生成を行うクラス
/// </summary>
public class ObjectCreator : MonoBehaviour
{
    [Header("生成したいオブジェクト")]
    [SerializeField]
    private GameObject[] _createObjects = default;

    [Header("オブジェクトの種類をランダムに生成するか")]
    [SerializeField]
    private bool _isRandom = false;

    [Header("自動で生成を行い続けるか")]
    [SerializeField]
    private bool _isAutoCreate = true;

    [Header("生成のインターバル")]
    [SerializeField]
    private float _createInterval = 1.0f;
    [SerializeField]
    private float _createTresureInterval = 1.0f;
    [SerializeField]
    private float _intervalDecrease = 0.01f;
    private float _intervalDecreaseCount = 1;
    [Header("フィーバー中の生成のインターバル")]
    [SerializeField]
    private float _createIntervalFever = 1.0f;
    [SerializeField]
    private float _createTresureIntervalFever = 1.0f;


    [Header("生成する個数制限")]
    [SerializeField]
    private bool _isCreateCountLimit = false;
    [SerializeField]
    private int _createCountNum = 10;

    [Header("以下、生成位置の指定に関するパラメーター")]
    [SerializeField]
    private CreatePositionType _createPositionType = CreatePositionType.RandomValue;

    [Header("Randomに生成する場合の範囲")]
    [SerializeField]
    private Vector3 _createRange = new(6, 0, 0);

    [Header("Transformで指定する場合の座標の設定")]
    [SerializeField]
    private Transform[] _createPositions = default;

    [Header("生成したオブジェクトの親オブジェクト")]
    [SerializeField]
    private Transform _parentObject = default;

    [Header("生成したオブジェクトの角度の範囲")]
    [SerializeField]
    private float SpawnAngle = default;

    [Header("生成したオブジェクトの速度")]
    [SerializeField]
    private float _spawnSpeed = default;

    /// <summary>
    /// 生成時の座標指定形式の種類
    /// </summary>
    private enum CreatePositionType
    {
        RandomValue,
        RandomTransform,
    }


    //========システム用メンバー変数========

    // インターバル計測用
    private float _timeCount = 0.0f;
    private float _itemTimeCount = 0.0f;
    // 生成したオブジェクトの個数カウント用
    private int _createCount = 0;
    // オブジェクトを生成するかどうか
    private bool _isCreate = false;

    int index;


    private void Start()
    {
        if (_createObjects.Length == 0)
        {
            Debug.LogError("CreateParamSOにCreateObjectsが設定されていません");
            return;
        }

        // 生成するフラグを立てる
        if (_isAutoCreate)
        {
            _isCreate = true;
        }
    }

    private void Update()
    {
        // 生成するかどうかのフラグが立っていない場合は何もしない
        if (_isCreate == false)
        {
            return;
        }

        // インターバル時間を加算
        _timeCount += Time.deltaTime;
        _itemTimeCount += Time.deltaTime;
        _intervalDecreaseCount += _intervalDecrease * Time.deltaTime;

        // インターバル時間に達したらオブジェクトを生成

        if(GameManager.Feverbool)
        {
            if (_timeCount >= _createIntervalFever / _intervalDecreaseCount)
            {
                CreateObject(false);
                _timeCount = 0.0f;
            }
            if (_itemTimeCount >= _createTresureIntervalFever)
            {
                CreateObject(true);
                _itemTimeCount = 0.0f;
            }
        } else
        {
            if (_timeCount >= _createInterval / _intervalDecreaseCount)
            {
                CreateObject(false);
                _timeCount = 0.0f;
            }
            if (_itemTimeCount >= _createTresureInterval)
            {
                CreateObject(true);
                _itemTimeCount = 0.0f;
            }
        }

    }

    private void CreateObject(bool _isTreasure)
    {
        // 生成する個数制限があるかチェック
        if (_isCreateCountLimit)
        {
            if (_createCountNum <= 0)
            {
                return;
            }
            _createCountNum--;
        }

        // 生成時の座標指定形式に応じて座標を設定
        Vector3 createPos = Vector3.zero;
        switch (_createPositionType)
        {
            case CreatePositionType.RandomValue:
                // ランダムな範囲にオブジェクトを生成
                createPos = new Vector3(
                    Random.Range(-_createRange.x, _createRange.x),
                    Random.Range(-_createRange.y, _createRange.y),
                    Random.Range(-_createRange.z, _createRange.z));
                break;

            case CreatePositionType.RandomTransform:
                // Transformで指定した座標にランダムにオブジェクトを生成
                int index = Random.Range(0, _createPositions.Length);

                createPos = new Vector3(
                    _createPositions[index].position.x,
                    _createPositions[index].position.y,
                    _createPositions[index].position.z);
                break;
        }
        createPos += transform.position;


        GameObject createObj = null;

        if (_isTreasure)
        {
            int index = 1;
            createObj = _createObjects[index];
        } else
        {
            // 生成するオブジェクトをランダムにするかチェック
            if (_isRandom)
            {
                // CreateObjectの配列の中からランダムに選択
                int randomIndex = Random.Range(1, 101);
                if(randomIndex <= 80)
                {
                    index = 0;
                }
                else
                {
                    index = 1;
                }

                index = Random.Range(0, _createObjects.Length);
                createObj = _createObjects[index];
            }
            else
            {
                // CreateObjectの配列の中から順番に選択
                int index = _createCount % _createObjects.Length;
                createObj = _createObjects[index];

                // 生成したオブジェクトの個数カウントを加算
                _createCount++;
            }
        }

        
        

        //ランダムな角度を計算
        float AngleIndex = Random.Range(-SpawnAngle, SpawnAngle) -90;

        // オブジェクトを生成
        createObj = Instantiate(createObj, createPos, Quaternion.identity);

        // 角度をラジアンに変換
        float radians = AngleIndex * Mathf.Deg2Rad;

        // xとyの速度成分を計算
        float velocityX = Mathf.Cos(radians) * _spawnSpeed;
        float velocityY = Mathf.Sin(radians) * _spawnSpeed;

        // Rigidbody2Dの速度を設定
     createObj.GetComponent<Rigidbody2D>().velocity = new Vector2(velocityX, velocityY);

        if (_parentObject != null)
        {
            createObj.transform.SetParent(_parentObject);
        }

        if(_isTreasure)
        {
            createObj.GetComponent<ItemManager>().Treasure = true;
        }

        // 自動で生成を行い続ける設定ではなかった場合、生成時にフラグを下げる
        if (_isAutoCreate == false)
        {
            _isCreate = false;
        }
    }

    /// <summary>
    /// 生成フラグを変更する関数
    /// </summary>
    public void SetCreateFlag(bool isCreate)
    {
        _isCreate = isCreate;
    }

    /// <summary>
    /// 生成インターバルを変更する関数
    /// </summary>
    public void SetCreateInterval(float interval)
    {
        _createInterval = interval;
    }
}
