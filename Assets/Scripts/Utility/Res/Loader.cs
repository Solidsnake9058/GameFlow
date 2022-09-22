using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace UnityGame.App.Game.Res
{
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.AsyncOperations;

    public class Loader
    {
        public static IGameItem Load(string path)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(path);
            var prefab = handle.WaitForCompletion();
            var item = prefab.GetComponent<IGameItem>();
            item.SetAsyncOperationHandle(handle);
            return item;
        }

        public static async Task<IGameItem> LoadAsync(string path)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(path);
            await handle.Task;
            var prefab = handle.Result;
            var item = prefab.GetComponent<IGameItem>();
            item.SetAsyncOperationHandle(handle);
            return item;
        }

        public static GameObject LoadGO(string path, out AsyncOperationHandle? asyncHandle)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(path);
            var prefab = handle.WaitForCompletion();
            asyncHandle = handle;
            return prefab;
        }

        public static T Load<T>(string path)
        {
            var handle = Addressables.LoadAssetAsync<T>(path);
            var prefab = handle.WaitForCompletion();
            return prefab;
        }
    }
}
