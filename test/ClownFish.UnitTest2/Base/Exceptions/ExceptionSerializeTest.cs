//using System;
//using System.Collections.Generic;
//using System.Text;
//using ClownFish.Base;
//using ClownFish.Base.Exceptions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace ClownFish.UnitTest.Base.Exceptions
//{
//    [TestClass]
//    public class ExceptionSerializeTest
//    {
//        private void TestCloneObject<T>(T ex) where T : Exception
//        {
//            T ex2 = ex.CloneObject();
//            Assert.AreEqual(ex2.Message, ex.Message);
//        }


//        [TestMethod]
//        public void Test()
//        {
//            var ex0 = ExceptionHelper.CreateException();

//            var ex1 = new BusinessLogicException("xx");
//            TestCloneObject(ex1);
//            var ex2 = new BusinessLogicException("xx", ex0);
//            TestCloneObject(ex2);


//            var ex3 = new ConfigurationErrorsException("xx");
//            TestCloneObject(ex3);
//            var ex4 = new ConfigurationErrorsException("xx", ex0);
//            TestCloneObject(ex4);

//            var ex5 = new DuplicateInsertException("xx", ex0);
//            TestCloneObject(ex5);

//            var ex6 = new HttpException(500, "ServerError");
//            Assert.AreEqual(500, ex6.StatusCode);
//            TestCloneObject(ex6);


//            var ex7 = new InvalidCodeException("xx");
//            TestCloneObject(ex7);

//            var ex8 = new MessageException("xx");
//            TestCloneObject(ex8);
//            var ex9 = new MessageException("xx", ex0);
//            TestCloneObject(ex9);


//            var ex12 = new ForbiddenException("xx");
//            TestCloneObject(ex12);
//            var ex13 = new ForbiddenException("xx", ex0);
//            TestCloneObject(ex13);


//            var ex14 = new ValidationException("xx");
//            TestCloneObject(ex14);
//            var ex15 = new ValidationException("xx", ex0);
//            TestCloneObject(ex15);


//            var ex16 = new ClientDataException("xx");
//            TestCloneObject(ex16);
//            var ex17 = new ClientDataException("xx", ex0);
//            TestCloneObject(ex17);


//            var ex18 = new DatabaseNotFoundException("xx");
//            TestCloneObject(ex18);
//            var ex19 = new DatabaseNotFoundException("xx", ex0);
//            TestCloneObject(ex19);
//        }

        


//    }
//}
