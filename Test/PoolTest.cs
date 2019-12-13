#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Fusion;
using System.Collections.Generic;
using Nirvana;

namespace Fusion
{
    public class PoolTest
    {
        [Test]
        public void CreatePoolTest()
        {
            var poolName = "Test Pool";
            var original = new GameObject("Test Original Object");
            PoolFactory.Create(poolName, original);
            Assert.AreNotEqual(PoolManager.Instance().GetPool(poolName), null, "对象池创建失败");
            PoolFactory.Destroy(poolName);
            Assert.AreEqual(PoolManager.Instance().GetPool(poolName), null, "对象池删除失败");
        }

        [Test]
        public void PoolNameTest()
        {
            var poolName = "Test Pool";
            var original = new GameObject("Test Original Object");
            PoolFactory.Create(poolName, original);
            Assert.AreEqual(PoolManager.Instance().GetPool(poolName).Name, poolName, "对象池创建失败");
            PoolFactory.Destroy(poolName);
        }

        [Test]
        public void GetObjectTest()
        {
            var poolName = "Test Pool";
            var original = new GameObject("Test Original Object");
            PoolFactory.Create(poolName, original);
            var objHandler = PoolManager.Instance().GetObject(poolName);
            Assert.AreNotEqual(objHandler, null, "对象创建失败");
            Assert.AreNotEqual(objHandler.valid, false, "对象创建失败");
            PoolManager.Instance().ReleaseObject(objHandler);
            PoolFactory.Destroy(poolName);
        }

        [Test]
        public void PoolObjectCountTest()
        {
            var poolName = "Test Pool";
            var original = new GameObject("Test Original Object");
            PoolFactory.Create(poolName, original);
            var list = new List<PoolObjectHolder>();
            int testCount = 10;
            for (var i = 0; i < testCount; ++i)
            {
                var objHandler = PoolManager.Instance().GetObject(poolName);
                Assert.AreNotEqual(objHandler, null, "对象创建失败");
                Assert.AreNotEqual(objHandler.valid, false, "对象创建失败");
                list.Add(objHandler);
            }

            for (var i = 0; i < testCount; ++i)
            {
                PoolManager.Instance().ReleaseObject(list[i]);
            }
            list.Clear();

            var pool = PoolManager.Instance().GetPool(poolName);
            Assert.AreEqual(pool.Count, 10, "对象数量错误");
            PoolFactory.Destroy(poolName);
            Assert.AreEqual(pool.Count, 0, "对象数量错误");
        }

        [Test]
        public void PoolObjectHandlerTest()
        {
            var poolName = "Test Pool";
            var original = new GameObject("Test Original Object");
            PoolFactory.Create(poolName, original);
            var objHandler = PoolManager.Instance().GetObject(poolName);
            PoolManager.Instance().ReleaseObject(objHandler);
            Assert.AreEqual(objHandler.valid, false, "释放错误");
            PoolManager.Instance().ReleaseObject(objHandler);
            var pool = PoolManager.Instance().GetPool(poolName);
            Assert.AreEqual(pool.Count, 1, "对象数量错误");
            PoolFactory.Destroy(poolName);
        }

        [Test]
        public void PoolObjectParentTest()
        {
            var poolName = "Test Pool";
            var original = new GameObject("Test Original Object");
            PoolFactory.Create(poolName, original);
            var objHandler = PoolManager.Instance().GetObject(poolName);
            Assert.AreEqual(objHandler.transform.parent, PoolManager.Root, "父节点错误");
            Assert.AreEqual(objHandler.gameObject.activeInHierarchy, false, "Active状态错误");
            var parent = new GameObject("parent").transform;
            objHandler.transform.SetParent(parent);
            Assert.AreEqual(objHandler.target.gameObject.activeInHierarchy, true, "Active状态错误");
            var target = objHandler.target;
            PoolManager.Instance().ReleaseObject(objHandler);
            Assert.AreEqual(target.transform.parent, PoolManager.Root, "父节点错误");
            Assert.AreEqual(target.gameObject.activeInHierarchy, false, "Active状态错误");
            PoolFactory.Destroy(poolName);
        }

        [Test]
        public void PoolObjectResetTransformTest()
        {
            var poolName = "Test Pool";
            var original = new GameObject("Test Original Object");
            PoolFactory.Create(poolName, original);
            var objHandler = PoolManager.Instance().GetObject(poolName);
            objHandler.transform.position = Vector3.one;
            objHandler.transform.rotation = Quaternion.Euler(100, 200, 300);
            objHandler.transform.localScale = new Vector3(2, 3, 4);
            var transform = objHandler.transform;
            PoolManager.Instance().ReleaseObject(objHandler);
            var objHandler2 = PoolManager.Instance().GetObject(poolName);
            Assert.AreEqual(objHandler2.transform, transform, "取出了错误对象");
            Assert.AreEqual(transform.position, Vector3.zero, "位置没有重置");
            Assert.AreEqual(transform.rotation, Quaternion.identity, "旋转没有重置");
            Assert.AreEqual(transform.localScale, Vector3.one, "缩放没有重置");
            PoolManager.Instance().ReleaseObject(objHandler);
            PoolFactory.Destroy(poolName);
        }

        [Test]
        public void PoolObjectRecursiveReleaseTest()
        {
            var poolName1 = "Test Pool";
            var poolName2 = "Test Pool2";
            var original1 = new GameObject("Test Original Object");
            var original2 = new GameObject("Test Original Object2");
            PoolFactory.Create(poolName1, original1);
            PoolFactory.Create(poolName2, original2);
            var objHandler1 = PoolManager.Instance().GetObject(poolName1);
            var objHandler2 = PoolManager.Instance().GetObject(poolName2);
            objHandler2.transform.SetParent(objHandler1.transform);
            var pool1 = PoolManager.Instance().GetPool(poolName1);
            var pool2 = PoolManager.Instance().GetPool(poolName2);
            Assert.AreEqual(pool1.Count, 0, "对象数量错误");
            Assert.AreEqual(pool2.Count, 0, "对象数量错误");

            PoolManager.Instance().ReleaseObject(objHandler1);


            Assert.AreEqual(pool1.Count, 1, "对象没有正确释放");
            Assert.AreEqual(pool2.Count, 1, "对象没有正确释放");

            PoolFactory.Destroy(poolName1);
            PoolFactory.Destroy(poolName2);
        }

        [Test]
        public void PoolObjectReleaseCallbackTest()
        {
            var poolName = "Test Pool";
            var original = new GameObject("Test Original Object");
            PoolFactory.Create(poolName, original);
            var objHandler = PoolManager.Instance().GetObject(poolName);
            bool callback = false;
            objHandler.onReleaseCallback = (name, obj) =>
            {
                Assert.AreEqual(name, poolName, "回调名称错误");
                Assert.AreEqual(obj, objHandler.target, "回调对象错误");
                callback = true;
            };
            PoolManager.Instance().ReleaseObject(objHandler);
            Assert.AreEqual(callback, true, "没有正确回调");
            PoolFactory.Destroy(poolName);
        }

        [Test]
        public void PoolObjectReleaseCallbackTest2()
        {
            var poolName = "Test Pool";
            var original = new GameObject("Test Original Object");
            PoolFactory.Create(poolName, original);
            var objHandler = PoolManager.Instance().GetObject(poolName);
            var objHandler2 = PoolManager.Instance().GetObject(poolName);
            objHandler2.transform.SetParent(objHandler.transform);
            int callback = 0;
            objHandler.onReleaseCallback = (name, obj) =>
            {
                Assert.AreEqual(name, poolName, "回调名称错误");
                Assert.AreEqual(obj, objHandler.target, "回调对象错误");
                callback++;
            };
            objHandler2.onReleaseCallback = (name, obj) =>
            {
                Assert.AreEqual(name, poolName, "回调名称错误");
                Assert.AreEqual(obj, objHandler2.target, "回调对象错误");
                callback++;
            };
            PoolManager.Instance().ReleaseObject(objHandler);
            Assert.AreEqual(callback, 2, "没有正确回调");
            PoolFactory.Destroy(poolName);
        }
    }
}

#endif