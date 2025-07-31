using Banking.FanFinancing.Shared.Services.Interface;

namespace Banking.FanFinancing.Shared.Services
{
    public class GuidService : IGuidService
    {
        private string guid;

        public GuidService()
        {
            guid = Guid.NewGuid().ToString();
        }

        public string GUID()
        {
            return guid;
        }
        public void SetGUID(string Guid)
        {
            this.guid = Guid;
        }
    }
}
