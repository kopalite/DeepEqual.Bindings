using DeepEqual.Bindings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace DeepEquals.Bindings.Test
{
    [TestClass]
    public class ExtendedComparerTest
    {
        [TestMethod]
        public void ExtendedComparer_Compare_CustomComparison()
        {
            //Arrange

            var a =  new A  { B = new B  { TestDeep = "x", TestBind = "y" }};
            var a1 = new A1 { B = new B1 { TestDeep = "x", TestBind1 = "y" } };

            //Act

            var comparer = ExtendedComparer<A, A1>.New()
                                .Bind(src => src.B, dest => dest.B,
                                     (src, dest) => (src.B == null && dest.B == null) || src.B.TestBind == dest.B.TestBind1);

            string difference = null;
            var result = comparer.AreEqual(a, a1, out difference);


            //Assert

            Assert.IsTrue(result);
            Assert.IsNull(difference);
        }

        [TestMethod]
        public void ExtendedComparer_Compare_DefaultComparison()
        {
            //Arrange

            var a = new A { B = new B { TestDeep = "x", TestBind = "y" } };
            var a1 = new A1 { B = new B1 { TestDeep = "x", TestBind1 = "y" } };

            //Act

            var comparer = ExtendedComparer<A, A1>.New().Bind(src => src.B, dest => dest.B);

            string difference = null;
            var result = comparer.AreEqual(a, a1, out difference);


            //Assert

            Assert.IsTrue(result);
            Assert.IsNull(difference);
        }
    }

    public class A
    {
        public B B { get; set; }
    }

    public class B
    {
        public string TestDeep { get; set; }

        public string TestBind { get; set; }
    }

    public class A1
    {
        public B1 B { get; set; }
    }

    public class B1
    {
        public string TestDeep { get; set; }

        public string TestBind1 { get; set; }

        
    }
}
