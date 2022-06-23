using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class NetworkEffectsSync : NetworkBehaviour
{
    [SerializeField] private List<ScriptableEffect> _allEffects = new List<ScriptableEffect>();
    public static NetworkEffectsSync get;

    public List<string> netIds = new List<string>();

    private void Awake()
    {
        get = this;
    }

    private void Update()
    {
        netIds.Clear();
        foreach (KeyValuePair<uint, NetworkIdentity> item in NetworkIdentity.spawned)
        {
            netIds.Add(item.Key.ToString());
        }

    }

    public void SyncEffect(string effectName, uint targetID)
    {
        CmdInitializeEffect(effectName, targetID);
    }

    [Command(requiresAuthority = false)]
    void CmdInitializeEffect(string effectName, uint targetID)
    {
        RpcInitializeEffect(effectName, targetID);
    }

    [ClientRpc(includeOwner = false)]
    void RpcInitializeEffect(string effectName, uint targetID)
    {
        Debug.Log("Rpc init effect");
        StartCoroutine(IEInitializeEffect(effectName, targetID));
    }

    IEnumerator IEInitializeEffect(string effectName, uint targetID)
    {
        Debug.Log("Start init effect");
        NetworkIdentity target = null;


        while (target == null)
        {
            NetworkIdentity.spawned.TryGetValue(targetID, out target);
            Debug.Log($"Try find network identity with ID : {targetID}");
            yield return null;
        }
        var targetPawn = target.GetComponent<Pawn>();

        var effect = _allEffects.FirstOrDefault(af => af.effectName == effectName);

        if (effect == null)
        {
            Debug.LogError($"Cant find effect :{effectName} for sync ");
            yield break;
        }
        var createdEffect = effect.InitializeEffect(target);

        targetPawn.AddEffect(createdEffect);
    }
}
