namespace GuildSaber.API
{
    public struct ApiCategory
    {
        public int ID { get; set; }
        public int GuildID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Logo { get; set; }
    }
}
