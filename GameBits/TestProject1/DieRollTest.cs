using GameBits;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for DieRollTest and is intended
    ///to contain all DieRollTest Unit Tests
    ///</summary>
	[TestClass()]
	public class DieRollTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///A test for DieRoll Constructor
		///</summary>
		[TestMethod()]
		public void DieRollConstructorTest()
		{
			int dice = 3;
			int sides = 8;
			int modifier = -1; // TODO: Initialize to an appropriate value
			DieRoll dr = new DieRoll(dice, sides, modifier);
			Assert.IsTrue(dr.Dice == dice && dr.Sides == sides && dr.Modifier == modifier);
		}

		/// <summary>
		///A test for FromString
		///</summary>
		[TestMethod()]
		public void FromStringTest()
		{
			string st = "2d8-1";
			DieRoll expected = new DieRoll(2, 8, -1);
			DieRoll actual = DieRoll.FromString(st);
			Assert.AreEqual(expected.ToString(), actual.ToString());
		}

		/// <summary>
		/// Test ToString
		/// </summary>
		[TestMethod()]
		public void ToStringTest()
		{
			DieRoll dr = new DieRoll(2, 8, -1);
			Assert.AreEqual("2d8-1", dr.ToString());
		}

		/// <summary>
		/// Test rolling from a string
		/// </summary>
		[TestMethod()]
		public void RollTestFromString()
		{
			string st = "3d8+1";
			int i = DieRoll.Roll(st);
			Assert.IsTrue(i >= 4 && i <= 25, "result: " + i.ToString());
			st = "d20";
			i = DieRoll.Roll(st);
			Assert.IsTrue(i >= 1 && i <= 20, "result: " + i.ToString());
			st = "300";
			i = DieRoll.Roll(st);
			Assert.IsTrue(i == 300, "result: " + i.ToString());
		}
	}
}
