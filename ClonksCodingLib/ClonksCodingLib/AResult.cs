using System;

namespace CCL {
    public struct AResult<T> {

        #region Properties
        public Exception Exception { get; private set; }
        public T Result { get; private set; }
        #endregion

        #region Constructor
        public AResult(Exception ex, T result)
        {
            Exception = ex;
            Result = result;
        }
        #endregion

    }
}
