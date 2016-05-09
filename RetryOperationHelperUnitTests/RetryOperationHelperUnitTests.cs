using System;
using System.Threading.Tasks;
using Microsoft.Data.Helpers;
using Microsoft.Data.Helpers.Helper;
using Microsoft.Samples;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Assert = Xunit.Assert;

namespace Microsoft.Samples
{
    [TestClass]
    public class RetryOperationHelperUnitTests
    {
        [Fact]
        public void TestExecuteWithRetry_OperationSucceeds()
        {
            //Arrange
            const int numberOfRetriesToAttempt = 3;
            const int numberOfFailuresToSimulate = 2;

            var retryLogic = new RetryOperationHelper();
            var operationSimulator = new OperationSimulator(numberOfFailuresToSimulate);
            Func<Task<bool>> func = () => operationSimulator.SimulateOperationWithFailures();

            //Act
            var result = retryLogic.ExecuteWithRetry(func, numberOfRetriesToAttempt).Result;

            //Assert 
            Assert.Equal(result, true);
        }

        [Fact]
        public void TestExecuteWithRetry_OperationFails()
        {
            //Arrange
            const int numberOfRetriesToAttempt = 3;
            const int numberOfFailuresToSimulate = 3;

            var retryLogic = new RetryOperationHelper();
            var operationSimulator = new OperationSimulator(numberOfFailuresToSimulate);
            Func<Task<bool>> func = () => operationSimulator.SimulateOperationWithFailures();

            //Act
            Exception ex = Assert.Throws<AggregateException>(() => retryLogic.ExecuteWithRetry(func, numberOfRetriesToAttempt).Wait());
            Assert.Equal(ex.InnerException.Message, "OperationSimulator: Simulating Operation Failure");
        }

        [Fact]
        public void TestExecuteWithRetry_OperationFails_VerifyOnFailureActionIsCalled()
        {
            //Arrange
            const int numberOfRetriesToAttempt = 3;
            const int numberOfFailuresToSimulate = 3;
            TimeSpan retryTimeSpan = new TimeSpan(0, 0, 0, 1);

            var retryLogic = new RetryOperationHelper();
            var operationSimulator = new OperationSimulator(numberOfFailuresToSimulate);
            Func<Task<bool>> func = () => operationSimulator.SimulateOperationWithFailures();
            Action<int, Exception> actionUponFailure = new Action<int, Exception>(operationSimulator.ThrowException);

            //Act
            Exception ex = Assert.Throws<AggregateException>(() => retryLogic.ExecuteWithRetry(func, numberOfRetriesToAttempt, retryTimeSpan, actionUponFailure).Wait());
            Assert.Equal(ex.InnerException.Message, "OperationSimulator: ThrowException: Exception thrown to identify method");
        }
    }
}
