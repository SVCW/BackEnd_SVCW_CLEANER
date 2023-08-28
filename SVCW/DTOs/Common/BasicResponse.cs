namespace SVCW.DTOs.Common
{
	public abstract class BasicResponse
	{
		public string data;
		public SVCWCode resultCode;
		public string resultMsg;
		public bool? isModer;
		public bool? isBan;
    }
}

