using System;
using System.Threading.Tasks;

namespace RetryOperationHelperUnitTests.Helper
{
    /// <summary>
    /// A class which simulates a given number of failures before succeeding
    /// </summary>
    public class OperationSimulator
    {
        private readonly int _numberOfFailuresToSimulateBeforeSuccess;
        private int _currentNumberOfFailuresSimulated = 0;

        public OperationSimulator(int numberOfFailuresToSimulateBeforeSuccess)
        {
            this._numberOfFailuresToSimulateBeforeSuccess = numberOfFailuresToSimulateBeforeSuccess;
        }

        public async Task<bool> SimulateOperationWithFailures()
        {
            if (_currentNumberOfFailuresSimulated < _numberOfFailuresToSimulateBeforeSuccess)
            {
                _currentNumberOfFailuresSimulated++;
                throw new ApplicationException("OperationSimulator: Simulating Operation Failure");
            }

            return true;
        }

        public void ThrowException(int numberOfPreviousFailures, Exception exception)
        {
            throw new ApplicationException("OperationSimulator: ThrowException: Exception thrown to identify method");
        }
    }
}