using UnityEngine;

public class ApplovinManager : MonoBehaviour
{
    void Awake()
    {
        MaxSdk.SetSdkKey("SkwvnkQqr_6nRgTxzKfWoRVNNeiS5rVRZuxLLeCfOA21seXZ_7ZZ2Um8ynwrGqfJpfl-Fid2fExGmb9lT1QDHp");
        MaxSdk.SetUserId(SystemInfo.deviceUniqueIdentifier);
        MaxSdk.SetVerboseLogging(true);
        MaxSdk.InitializeSdk();
    }
}
