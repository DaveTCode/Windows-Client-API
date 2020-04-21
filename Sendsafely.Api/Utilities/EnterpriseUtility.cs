using Sendsafely.Api.Objects;

namespace Sendsafely.Api.Utilities
{
    internal class EnterpriseUtility
    {
        private readonly Connection _connection;

        #region Constructors

        public EnterpriseUtility(Connection connection)
        {
            _connection = connection;
        }

        #endregion

        #region Public Functions

        public EnterpriseInformation GetInformation()
        {
            var p = ConnectionStrings.Endpoints["enterpriseInfo"].Clone();

            var response = _connection.Send<EnterpriseInformationResponse>(p);

            var info = new EnterpriseInformation
            {
                Host = response.Host,
                SystemName = response.SystemName,
                AllowUndisclosedRecipients = response.AllowUndisclosedRecipients,
                OutlookBeta = response.OutlookBeta,
                MessageEncryption = response.MessageEncryption
            };

            return info;
        }

        #endregion
    }
}
