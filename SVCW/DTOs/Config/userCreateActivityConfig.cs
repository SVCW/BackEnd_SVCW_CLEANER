namespace SVCW.DTOs.Config
{
    public class userCreateActivityConfig
    {
        public bool isDonatable { get; set; }
        public bool isFanpage { get; set; }
        public bool isValidCreate { get; set; }
        public string? message { get; set; }
        public float maxDonate { get; set; }
        public int target { get; set; }
        public int activityJoin { get; set; }
    }
}
