namespace Banking.FanFinancing.Shared.Models
{
    public class CacheTimeConfig
    {
        public int UrlAbsoluteExpirationTime { get; set; } = 30;
        public int UrlSlidingExpiration { get; set; } = 30;
        public int HomeAbsoluteExpirationTime { get; set; } = 30;
        public int HomeSlidingExpiration { get; set; } = 30;
    }
}
