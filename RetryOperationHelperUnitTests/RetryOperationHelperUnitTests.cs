using System;
using System.Threading.Tasks;
using Microsoft.Samples;
using RetryOperationHelperUnitTests.Helper;
using Xunit;

namespace RetryOperationHelperUnitTests
{
    public class RetryOperationHelperUnitTests
    {
        [Fact]
        public void TestExecuteWithRetry_OperationSucceeds()
        {
            //Arrange
            const int numberOfRetriesToAttempt = 3;
            const int numberOfFailuresToSimulate = 2;

            var operationSimulator = new OperationSimulator(numberOfFailuresToSimulate);
            Func<Task<bool>> func = () => operationSimulator.SimulateOperationWithFailures();

            //Act
            var result = RetryOperationHelper.ExecuteWithRetry(func, numberOfRetriesToAttempt).Result;

            //Assert 
            Assert.Equal(result, true);
        }

        [Fact]
        public void TestExecuteWithRetry_OperationFails()
        {
            //Arrange
            const int numberOfRetriesToAttempt = 3;
            const int numberOfFailuresToSimulate = 3;

            var operationSimulator = new OperationSimulator(numberOfFailuresToSimulate);
            Func<Task<bool>> func = () => operationSimulator.SimulateOperationWithFailures();

            //Act
            Exception ex = Assert.Throws<AggregateException>(() => RetryOperationHelper.ExecuteWithRetry(func, numberOfRetriesToAttempt).Wait());
            Assert.Equal(ex.InnerException.Message, "OperationSimulator: Simulating Operation Failure");
        }

        [Fact]
        public void TestExecuteWithRetry_OperationFails_VerifyOnFailureActionIsCalled()
        {
            //Arrange
            const int numberOfRetriesToAttempt = 3;
            const int numberOfFailuresToSimulate = 3;
            TimeSpan retryTimeSpan = new TimeSpan(0, 0, 0, 1);

            var operationSimulator = new OperationSimulator(numberOfFailuresToSimulate);
            Func<Task<bool>> func = () => operationSimulator.SimulateOperationWithFailures();
            Action<int, Exception> actionUponFailure = new Action<int, Exception>(operationSimulator.ThrowException);

            //Act
            Exception ex = Assert.Throws<AggregateException>(() => RetryOperationHelper.ExecuteWithRetry(func, numberOfRetriesToAttempt, retryTimeSpan, actionUponFailure).Wait());
            Assert.Equal(ex.InnerException.Message, "OperationSimulator: ThrowException: Exception thrown to identify method");
        }
    }
}