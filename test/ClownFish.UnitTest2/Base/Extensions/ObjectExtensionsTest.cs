using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Extensions;
[TestClass]
public class ObjectExtensionsTest
{
    [TestMethod]
    public void Test_CopyData()
    {
        X1 x1 = new X1 {
            A = 1,
            B = "abc",
            C = Guid.NewGuid(),
            D = DateTime.Now,
            E = 123,
            W = 111,
            X = 111,
            Z = 222
        };

        X2 x2 = new X2();
        x1.CopyData(x2);

        Assert.AreEqual(x1.A, x2.A);
        Assert.AreEqual(x1.B, x2.B);
        Assert.AreEqual(x1.C, x2.C);
        Assert.AreEqual(x1.D, x2.D);
        Assert.IsNull(x2.E);   // 类型不匹配，不复制
        Assert.IsNull(x2.F);   // 没有 “源” 属性
        Assert.AreEqual(0, x2.W);

        MyAssert.IsError<ArgumentNullException>(()=> {
            ObjectExtensions.CopyData(null, x2);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            ObjectExtensions.CopyData(x1, null);
        });
    }


    [TestMethod]
    public void Test_ConvertTo()
    {
        X1 x1 = new X1 {
            A = 1,
            B = "abc",
            C = Guid.NewGuid(),
            D = DateTime.Now,
            E = 123,
            W = 111,
            X = 111,
            Z = 222
        };

        X2 x2 = x1.ConvertTo<X2>();

        Assert.AreEqual(x1.A, x2.A);
        Assert.AreEqual(x1.B, x2.B);
        Assert.AreEqual(x1.C, x2.C);
        Assert.AreEqual(x1.D, x2.D);
        Assert.IsNull(x2.E);   // 类型不匹配，不复制
        Assert.IsNull(x2.F);   // 没有 “源” 属性
        Assert.AreEqual(0, x2.W);
    }





    public class X1
    {
        public int A { get; set; }
        public string B { get; set; }
        public Guid C { get; set; }
        public DateTime D { get; set; }

        public long E { get; set; }

        public long W {
            set { E = value; }
        }

        public long X { get; set; }

        public long Z { get; set; }
    }

    public class X2
    {
        public int A { get; set; }
        public string B { get; set; }
        public Guid C { get; set; }
        public DateTime D { get; set; }

        public string E { get; set; }

        public string F { get; set; }

        public long W { get; set; }

        public long X {
            get {
                return W;
            }
        }
    }

}
