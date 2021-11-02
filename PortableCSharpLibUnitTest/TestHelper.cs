using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    public class TestHelper
    {
        // 注册一个委托，在该委托中抛出异常。
        // 在action中调用可能会抛出异常的方法
        // e为null表示任何异常。一般用于测试事件触发
        public static void AssertException(Action action, Type e = null)
        {
            bool isSuccessed = false;
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                if (e == null || e == ex.GetType())
                {
                    isSuccessed = true;
                }
            }
            if (!isSuccessed)
            {
                Assert.Fail();
            }
        }

        // 测试没有抛出指定的异常
        public static void AssertNoException(Action action, Type e = null)
        {
            bool isSuccessed = false;
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                if (e == null || e == ex.GetType())
                {
                    isSuccessed = true;
                }
            }
            if (isSuccessed)
            {
                Assert.Fail();
            }
        }
    }
}
