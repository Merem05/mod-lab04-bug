using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using System;

namespace BugTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_InitialState_IsNew()
        {
            Bug bug = new Bug();
            Assert.AreEqual(BugState.New, bug.CurrentState);
        }

        [TestMethod]
        public void Test_Submit_ChangesToTriage()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            Assert.AreEqual(BugState.Triage, bug.CurrentState);
        }

        [TestMethod]
        public void Test_NormalPath_FixedAndClosed()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.VerifyAndCloseBug();
            Assert.AreEqual(BugState.Closed, bug.CurrentState);
        }

        [TestMethod]
        public void Test_NotABugPath_Closed()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.MarkAsNotABug();
            bug.VerifyAndCloseBug();
            Assert.AreEqual(BugState.Closed, bug.CurrentState);
        }

        [TestMethod]
        public void Test_NotReproducible_ThenReopen_Fixed()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.AssignToDeveloper();
            bug.MarkAsNotReproducible();
            bug.ReopenBug();
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.VerifyAndCloseBug();
            Assert.AreEqual(BugState.Closed, bug.CurrentState);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_CannotAssignBeforeSubmit()
        {
            Bug bug = new Bug();
            bug.AssignToDeveloper();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_CannotFixWithoutAssignment()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.MarkAsFixed();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_CannotVerifyAndCloseFromTriage()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.VerifyAndCloseBug();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_CannotVerifyAndCloseFromInProgress()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.AssignToDeveloper();
            bug.VerifyAndCloseBug();
        }

        [TestMethod]
        public void Test_ReopenFromClosed_Works()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.VerifyAndCloseBug();
            bug.ReopenFromClosed();
            Assert.AreEqual(BugState.Reopened, bug.CurrentState);
        }

        [TestMethod]
        public void Test_AfterReopen_SendToTriage_Works()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.VerifyAndCloseBug();
            bug.ReopenFromClosed();
            bug.SendToTriage();
            Assert.AreEqual(BugState.Triage, bug.CurrentState);
        }

        [TestMethod]
        public void Test_ReopenFromFixed_Works()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.ReopenBug();
            Assert.AreEqual(BugState.Triage, bug.CurrentState);
        }

        [TestMethod]
        public void Test_RequestMoreInfo_ThenProvide_ReturnsToTriage()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.RequestMoreInformation();
            Assert.AreEqual(BugState.NeedsMoreInfo, bug.CurrentState);
            bug.ProvideMoreInformation();
            Assert.AreEqual(BugState.Triage, bug.CurrentState);
        }

        [TestMethod]
        public void Test_NoTimePath_ThenAssignLater()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.MarkAsNoTime();
            Assert.AreEqual(BugState.NoTime, bug.CurrentState);
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.VerifyAndCloseBug();
            Assert.AreEqual(BugState.Closed, bug.CurrentState);
        }

        [TestMethod]
        public void Test_MultipleReopenCycles_Works()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            for (int i = 0; i < 3; i++)
            {
                bug.AssignToDeveloper();
                bug.MarkAsFixed();
                bug.VerifyAndCloseBug();
                bug.ReopenFromClosed();
                bug.SendToTriage();
            }
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.VerifyAndCloseBug();
            Assert.AreEqual(BugState.Closed, bug.CurrentState);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_CannotReopenFromClosedWithReopenBug()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.VerifyAndCloseBug();
            bug.ReopenBug();
        }

        [TestMethod]
        public void Test_NotABugPath_ThenReopen_ReturnsToTriage()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.MarkAsNotABug();
            bug.ReopenBug();
            Assert.AreEqual(BugState.Triage, bug.CurrentState);
        }

        [TestMethod]
        public void Test_NotReproducible_ThenVerifyAndClose_Closes()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.AssignToDeveloper();
            bug.MarkAsNotReproducible();
            bug.VerifyAndCloseBug();
            Assert.AreEqual(BugState.Closed, bug.CurrentState);
        }

        [TestMethod]
        public void Test_StressTest_ManyBugsInSequence()
        {
            for (int i = 0; i < 50; i++)
            {
                Bug bug = new Bug();
                bug.SubmitNewBug();
                bug.AssignToDeveloper();
                bug.MarkAsFixed();
                bug.VerifyAndCloseBug();
                Assert.AreEqual(BugState.Closed, bug.CurrentState);
            }
        }

        [TestMethod]
        public void Test_AllStatesReachable()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.RequestMoreInformation();
            bug.ProvideMoreInformation();
            bug.MarkAsNoTime();
            bug.AssignToDeveloper();
            bug.MarkAsNotReproducible();
            bug.ReopenBug();
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.VerifyAndCloseBug();
            bug.ReopenFromClosed();
            bug.SendToTriage();
            Assert.AreEqual(BugState.Triage, bug.CurrentState);
        }

        [TestMethod]
        public void Test_CompleteCycle_NewToClosedToReopenedToClosed()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.VerifyAndCloseBug();
            bug.ReopenFromClosed();
            bug.SendToTriage();
            bug.AssignToDeveloper();
            bug.MarkAsFixed();
            bug.VerifyAndCloseBug();
            Assert.AreEqual(BugState.Closed, bug.CurrentState);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_CannotSubmitFromTriage()
        {
            Bug bug = new Bug();
            bug.SubmitNewBug();
            bug.SubmitNewBug();
        }
    }
}