namespace PolarBearEapApi.Commons
{
    public class LogMessageGenerator
    {
        public static string GetErrorMessage(string serializeData, string errorMessage) { 
            return serializeData + ">>>" + errorMessage;
        }
    }
}
