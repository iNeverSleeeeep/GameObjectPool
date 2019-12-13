#if UNITY_EDITOR
using Nirvana;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Fusion
{
    public class PoolPresetTest
    {
        [UnityTest]
        public IEnumerator LoadPoolsTest()
        {
            var preset = ScriptableObject.CreateInstance(typeof(PoolObjectPreset)) as PoolObjectPreset;
            preset.name = "Test";
            var assets = new List<AssetID>();

            assets.Add(new AssetID("arts/actors/1001/1001001", "1001001"));
            assets.Add(new AssetID("arts/actors/1002/1002001", "1002001"));
            assets.Add(new AssetID("arts/actors/1003/1003001", "1003001"));

            preset.assets = assets.ToArray();
            yield return PoolLoaderByPreset.CreatePoolsCoroutine(preset);
            var pool1 = PoolManager.Instance().GetPool("Test.1001001");
            var pool2 = PoolManager.Instance().GetPool("Test.1002001");
            var pool3 = PoolManager.Instance().GetPool("Test.1003001");
            Assert.AreNotEqual(pool1, null, "对象池生成失败");
            Assert.AreNotEqual(pool2, null, "对象池生成失败");
            Assert.AreNotEqual(pool3, null, "对象池生成失败");
            var objHandler1 = PoolManager.Instance().GetObject("Test.1001001");
            var objHandler2 = PoolManager.Instance().GetObject("Test.1002001");
            var objHandler3 = PoolManager.Instance().GetObject("Test.1003001");
            Assert.AreNotEqual(objHandler1, null, "对象生成失败");
            Assert.AreNotEqual(objHandler2, null, "对象生成失败");
            Assert.AreNotEqual(objHandler3, null, "对象生成失败");
            Assert.AreNotEqual(objHandler1.valid, false, "对象生成失败");
            Assert.AreNotEqual(objHandler2.valid, false, "对象生成失败");
            Assert.AreNotEqual(objHandler3.valid, false, "对象生成失败");
            Assert.AreEqual(objHandler1.target.name, "1001001(Clone)", "对象生成失败");
            Assert.AreEqual(objHandler2.target.name, "1002001(Clone)", "对象生成失败");
            Assert.AreEqual(objHandler3.target.name, "1003001(Clone)", "对象生成失败");
            PoolManager.Instance().ReleaseObject(objHandler1);
            PoolManager.Instance().ReleaseObject(objHandler2);
            PoolManager.Instance().ReleaseObject(objHandler3);

            PoolFactory.Destroy("Test.1001001");
            PoolFactory.Destroy("Test.1002001");
            PoolFactory.Destroy("Test.1003001");

            Assert.AreEqual(PoolManager.Instance().PoolCount, 0, "对象池删除失败");
        }


        [UnityTest]
        public IEnumerator PreloadDecoratorTest()
        {
            var poolPreset = ScriptableObject.CreateInstance(typeof(PoolObjectPreset)) as PoolObjectPreset;
            poolPreset.name = "Test";
            var assets = new List<AssetID>();
            var preset = new PoolPreset();
            preset.preloadCount = 10;
            preset.preloadPerFrame = 2;
            assets.Add(new AssetID("arts/actors/1001/1001001", "1001001"));

            poolPreset.assets = assets.ToArray();
            poolPreset.preset = preset;

            yield return PoolLoaderByPreset.CreatePoolsCoroutine(poolPreset);
            var pool = PoolManager.Instance().GetPool("Test.1001001");
            Assert.AreEqual(pool.Count, 0, "缓存数量错误");
            yield return null;
            Assert.AreEqual(pool.Count, 2, "缓存数量错误");
            yield return null;
            Assert.AreEqual(pool.Count, 4, "缓存数量错误");
            yield return null;
            Assert.AreEqual(pool.Count, 6, "缓存数量错误");
            yield return null;
            Assert.AreEqual(pool.Count, 8, "缓存数量错误");
            yield return null;
            Assert.AreEqual(pool.Count, 10, "缓存数量错误");
            yield return null;
            Assert.AreEqual(pool.Count, 10, "缓存数量错误");
            yield return null;
            PoolFactory.Destroy("Test.1001001");
        }

        [UnityTest]
        public IEnumerator AutoReleaseDecoratorTest()
        {
            var poolPreset = ScriptableObject.CreateInstance(typeof(PoolObjectPreset)) as PoolObjectPreset;
            poolPreset.name = "Test";
            var assets = new List<AssetID>();
            var preset = new PoolPreset();
            preset.autoRelease = 5;
            assets.Add(new AssetID("arts/actors/1001/1001001", "1001001"));

            poolPreset.assets = assets.ToArray();
            poolPreset.preset = preset;

            yield return PoolLoaderByPreset.CreatePoolsCoroutine(poolPreset);

            var testCount = 20;
            var list = new List<PoolObjectHolder>();
            for (var i = 0; i < testCount; ++i)
                list.Add(PoolManager.Instance().GetObject("Test.1001001"));

            foreach (var holder in list)
                PoolManager.Instance().ReleaseObject(holder);

            var pool = PoolManager.Instance().GetPool("Test.1001001");
            Assert.AreEqual(pool.Count, testCount, "对象数量不匹配");
            yield return new WaitForSecondsRealtime(14);
            Assert.AreEqual(pool.Count, testCount, "对象数量不匹配");
            yield return new WaitForSecondsRealtime(2);
            Assert.AreEqual(pool.Count, preset.autoRelease, "对象数量不匹配");
            PoolFactory.Destroy("Test.1001001");
        }

        [UnityTest]
        public IEnumerator MaxActiveObjectsDecoratorTest()
        {
            var maxActiveObjects = 5;
            var poolPreset = ScriptableObject.CreateInstance(typeof(PoolObjectPreset)) as PoolObjectPreset;
            poolPreset.name = "Test";
            var assets = new List<AssetID>();
            var preset = new PoolPreset();
            preset.maxActiveObjects = maxActiveObjects;
            assets.Add(new AssetID("arts/actors/1001/1001001", "1001001"));

            poolPreset.assets = assets.ToArray();
            poolPreset.preset = preset;

            yield return PoolLoaderByPreset.CreatePoolsCoroutine(poolPreset);

            var testCount = 20;
            var list = new List<PoolObjectHolder>();
            for (var i = 0; i < testCount; ++i)
                list.Add(PoolManager.Instance().GetObject("Test.1001001"));

            for (var i = 0; i < testCount - maxActiveObjects; ++i)
                Assert.AreEqual(list[i].valid, false, "旧的节点回收错误");

            for (var i = testCount - maxActiveObjects; i < testCount; ++i)
                Assert.AreEqual(list[i].valid, true, "新的节点生成错误");

            for (var i = 0; i < testCount; ++i)
                PoolManager.Instance().ReleaseObject(list[i]);

            var pool = PoolManager.Instance().GetPool("Test.1001001");
            Assert.AreEqual(pool.Count, maxActiveObjects, "节点数量错误");

            PoolFactory.Destroy("Test.1001001");
        }
    }
}

#endif